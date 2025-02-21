using Guga.Collector.ConfigModel;
using Microsoft.Extensions.Options;
using Guga.Collector.Interfaces;
using Guga.Core.PLCLinks;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Core.PlcSignals;
using Guga.Redis.ConfigModels;
using Newtonsoft.Json;
using ColinChang.RedisHelper;
using PLCCollect.Collector.Interfaces;
using Guga.Core.Enums;

namespace Guga.Collector.Services
{


    /// <summary>
    /// 多服务模式下的竞选服务
    /// </summary>
    public class MasterElectionService : IMasterElectionService
    {
        #region Attribute fields
        private readonly ServerOptions _serverOptions;
        private readonly ICollectorRedisService _collectorRedisService;
        private readonly ISignalCollector _signalCollector;
        private readonly ISignalWriter _signalWriter;


 
        private readonly RedisKeyOptions _redisKeyOptions;
        private readonly IPLCLinkFactory _pLCLinkFactory;
        private readonly IMasterServeStatus _masterServeStatus;
        private ILogService _logService;
        #endregion
        #region Constructor function
        public MasterElectionService(IOptions<ServerOptions> serverOptions, ICollectorRedisService collectorRedisService,
            ISignalCollector signalCollector,
            ISignalWriter signalWriter, IRedisHelper redisHelper,
            IOptions<RedisKeyOptions> redisKeyOptions,
            IPLCLinkFactory pLCLinkFactory, IMasterServeStatus masterServeStatus,
            ILogService logService)
        {
            _serverOptions = serverOptions.Value;
            _collectorRedisService = collectorRedisService;
            _signalCollector = signalCollector;
            _signalWriter = signalWriter;
         
            _redisKeyOptions = redisKeyOptions.Value;
            _pLCLinkFactory = pLCLinkFactory;
            _masterServeStatus = masterServeStatus;
            _logService = logService;
        }
        public void StartMasterElection(CancellationToken cancellationToken)
        {
            _logService.Log($"{_serverOptions.ServerCode}竞选服务启动", LogCategory.ServiceElection, LogLevel.Info);
            Task.Run(() => StartMasterElectionAsync(cancellationToken), cancellationToken);
        }
        private async Task StartMasterElectionAsync(CancellationToken cancellationToken)
        {


            while (!cancellationToken.IsCancellationRequested)
            {

                try
                {
                    _logService.Log($"{_serverOptions.ServerCode}尝试竞选成为主服务",LogCategory.ServiceElection, LogLevel.Info);
                    // 尝试竞选成为主服务
                    var isMaster = await _collectorRedisService.RetryRegisterServiceAsync(
                        _serverOptions.ServerCode!,
                        TimeSpan.FromSeconds(_serverOptions.LockSeconds));

                    if (isMaster)
                    {
                       
                        await PerformMasterRoleAsync(cancellationToken);
                    }
                    else
                    {
                        await StopService(cancellationToken);
                        _masterServeStatus.IsMaster = false;
                    }

                    // 等待一段时间后重新竞选
                    await Task.Delay(TimeSpan.FromSeconds(_serverOptions.RenewalLockSeconds));
                }
                catch (Exception ex)
                {
                    _logService.Log($"服务竞选发生错误{ex.Message}", LogCategory.ServiceElection, LogLevel.Error);
                  
                }

            }
            if (cancellationToken.IsCancellationRequested)
            {
                await StopService(cancellationToken);
            }
        }

        private async Task<bool> PerformMasterRoleAsync(CancellationToken cancellationToken)
        {
            var result = false;

            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    // 检查当前服务是否仍是主服务
                    var isStillMaster = await _collectorRedisService.IsCurrentServiceRegisteredAsync(_serverOptions.ServerCode!);

                    if (isStillMaster)
                    {
                        // 续约锁
                        var isRenewalSeccess = await _collectorRedisService.RetryRegisterServiceAsync(
                             _serverOptions.ServerCode!,
                             TimeSpan.FromSeconds(_serverOptions.LockSeconds));
                        if (isRenewalSeccess)
                        {
                            result = true;
                          
                            await StartService(cancellationToken);
                            // 执行主服务逻辑
                            var logmsg = $"{_serverOptions.ServerCode}正在作为主服务运行...";
                            if (_masterServeStatus.IsMaster)
                            {
                                logmsg= $"{_serverOptions.ServerCode}服务续约成功...";
                            }
                              
                              

                      _logService.Log(logmsg, LogCategory.ServiceElection, LogLevel.Info);


                            _masterServeStatus.IsMaster = true;
                        }
                        else
                        {
                            _masterServeStatus.IsMaster = false;
                            await StopService(cancellationToken);

                            _logService.Log($"{_serverOptions.ServerCode}续约失败， 已失去主服务身份", LogCategory.ServiceElection, LogLevel.Warning);

                        }

                    }
                    else
                    {
                        await StopService(cancellationToken);
                        // 如果锁不再属于当前服务，退出主服务逻辑
                        _logService.Log($"{_serverOptions.ServerCode}续约失败， 已失去主服务身份", LogCategory.ServiceElection, LogLevel.Warning);

                    }

                    
                }
                else
                {
                    await StopService(cancellationToken);

                }
            }
            catch (Exception ex)
            {
                await StopService( cancellationToken);
               

                _logService.Log($"主服务发生错误：{ex.Message}", LogCategory.ServiceElection, LogLevel.Error);
                // 如果发生错误，退出主服务逻辑
            }
            return result;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        private async Task StartService(CancellationToken cancellationToken)
        {
            if (!_signalCollector.IsInit)
            {
                await _signalCollector.Init(getAllPLcLinks, cancellationToken);
                await _signalCollector.Start(cancellationToken);
            }
            else
            {
                if (!_signalCollector.IsRunning && !_signalCollector.IsUserStop)
                {
                    await _signalCollector.ReStart(cancellationToken);
                }
            }


            if (!_signalWriter.IsInit)
            {
                _signalWriter.Init(_serverOptions.WriteBatchInterval, _serverOptions.WriteBatchSize, cancellationToken);
                await _signalWriter.Start(cancellationToken);
            }
            else
            {
                if (!_signalWriter.IsRunning && !_signalWriter.IsUserStop)
                {
                    await _signalWriter.ReStart(cancellationToken);
                }
            }
        }
        private async Task StopService(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (_signalCollector.IsInit && _signalCollector.IsRunning)
                {
                    await _signalCollector.Stop();
                }

                if (_signalWriter.IsInit && _signalWriter.IsRunning)
                {
                    await _signalWriter.Stop();
                }
            }
            else
            {
                await _signalCollector.Stop();
                await _signalWriter.Stop();
            }
           
        }
        /// <summary>
        /// 组装链路信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<PLCLink>> getAllPLcLinks()
        {
            List<PLCLink> result = new List<PLCLink>();
            #region 加载链路信息
            var plclinksIds = await _collectorRedisService._redisHelper.StringGetAsync<List<string>>(_redisKeyOptions._PLCLinksIDs);
            if (plclinksIds.Any())
            {
                var task = plclinksIds.Select(async plclinkId =>
                {
                    // 这里调用你的 HashGetAsync 方法获取链路信息
                    var plclinkData = await _collectorRedisService._redisHelper.StringGetAsync<PLCLinkInfo>($"{_redisKeyOptions._PLCLinkInfo(plclinkId)}");
                    return plclinkData;
                });
                var plclinkInfos = await Task.WhenAll(task);
                if (plclinkInfos.Any())
                {
                    List<PLCLink> plclinks = new List<PLCLink>();
                    foreach (var d in plclinkInfos)
                    {
                        PLCLink plclink = _pLCLinkFactory.CreatePLCLink<UniversalPLCLink>(door =>
                        {
                            door.plclinkInfo = d;
                        });
                        //仅Modbus链路
                        //if (d.ProtocolType_ == ProtocolType.Modbus)
                        //{
                            plclinks.Add(plclink);
                       // }
                    }
                    result.AddRange(plclinks);
                }

            }
            #endregion
            #region  加载链路信号信息
            if (result.Any())
            {
                foreach (var plclink in result)
                {
                    #region 获取链路信号
                    var d_signals_dic = await _collectorRedisService._redisHelper.HashGetAsync(_redisKeyOptions._PLCLinksSignals(plclink.plclinkInfo.PLCLinkId));

                    if (d_signals_dic.Any())
                    {
                        List<IPlcSignal> plcSignals = new List<IPlcSignal>();
                        foreach (var signal in d_signals_dic.Values)
                        {
                            if (!string.IsNullOrEmpty(signal))
                            {
                                IPlcSignal plcSignal = null;
                                if (plclink.plclinkInfo.ProtocolType_ == Core.Enums.ProtocolType.S7)
                                {
                                    plcSignal = JsonConvert.DeserializeObject<S7Signal>(signal);
                                }
                                else if (plclink.plclinkInfo.ProtocolType_ == Core.Enums.ProtocolType.Modbus)
                                {
                                    plcSignal = JsonConvert.DeserializeObject<ModbusSignal>(signal);
                                }
                                if (plcSignal != null)
                                {
                                    
                                    plcSignal.PLCLink = plclink;
                                    plcSignals.Add(plcSignal);
                                }

                            }


                        }
                        plclink.SubscribeToSignals(plcSignals);
                    }

                    #endregion


                }

            }

            return result;

            #endregion
        }
        #endregion
    }
}
