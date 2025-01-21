using ColinChang.RedisHelper;
using Guga.Collector;
using Guga.Collector.Guga.Collector;
using Guga.Collector.Interfaces;
using Guga.Collector.Services;
using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Core.PLCLinks;
using Guga.Core.PlcSignals;
using Guga.Redis.ConfigModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using S7.Net;
using S7.Net.Types;
using System.Collections.Concurrent;

namespace Guga.BlazorApp
{
    /// <summary>
    /// 链路初始化服务。
    /// </summary>
    public class PLCLinksInitHostedService : IHostedService
    {
        private readonly IPLCLinkManager _plclinkManager;
        private readonly IRedisHelper _redisHelper;
        private readonly ISignalCollector _signalCollector;
        private readonly IServiceProvider _serviceProvider;
        private readonly RedisKeyOptions _redisKeyOptions;
        private readonly IPLCLinkFactory _pLCLinkFactory;
        private readonly ISignalWriter _signalWriterService;
        private readonly ICollectorRedisService _collectorRedisService;
        private readonly ISimulatedSignalWriter _simulatedSignalWriter;
        public PLCLinksInitHostedService(IPLCLinkManager plclinkManager, IRedisHelper redisHelper,
            ISignalCollector signalCollector, IServiceProvider serviceProvider, IOptions<RedisKeyOptions> redisKeyOptions,
            IPLCLinkFactory pLCLinkFactory, ISignalWriter signalWriterService, ICollectorRedisService collectorRedisService
            , ISimulatedSignalWriter simulatedSignalWriter
            )
        {
            _plclinkManager = plclinkManager;
            _redisHelper = redisHelper;
            _signalCollector = signalCollector;
            _serviceProvider = serviceProvider;
            _redisKeyOptions = redisKeyOptions.Value;
            _pLCLinkFactory = pLCLinkFactory;
            _signalWriterService = signalWriterService;
            _collectorRedisService = collectorRedisService;
            _simulatedSignalWriter= simulatedSignalWriter;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();


            var plclinkFactory = _serviceProvider.GetRequiredService<IPLCLinkFactory>();
            //创建链路
            PLCLinkInfo plclinkInfo = new  PLCLinkInfo{
                PLCLinkId = "DV-2",
                PLCLinkName = "西门子链路",
                PLCLinkCode = "WR001",
                PLCLinkType_ = PLCLinkType.Universal,
                Ip = "127.0.0.1",
                ProtocolType_ = ProtocolType.S7,
                S7CPUType_ = S7CPUType.S71200,
                rack = 0,
                slot = 1,
            };
            await _redisHelper.StringSetAsync(_redisKeyOptions._PLCLinksIDs, new List<string>() { "DV-2" });
            await _redisHelper.StringSetAsync(_redisKeyOptions._PLCLinkInfo(plclinkInfo.PLCLinkId), plclinkInfo);
            //创建信号
            var s7Signals = CreateS7TestSignals();
            
           var  cc = s7Signals.ToDictionary(
            signal => signal.SignalName,
            signal => JsonConvert.SerializeObject(signal)
        );
            ConcurrentDictionary<string, string> entries = new ConcurrentDictionary<string, string>();
            foreach (var signal in s7Signals)
            {
                entries.TryAdd(signal.SignalName, JsonConvert.SerializeObject(signal));

            } 
              await _redisHelper.HashSetAsync(_redisKeyOptions._PLCLinksSignals(plclinkInfo.PLCLinkId), entries);



            //Check 默认配置是否存在
            #region 从 Redis 加载西门子链路不同型号机架号、插槽数据
            var s7RackSlotredisKey = _redisKeyOptions._S7RackSlot;
            var redis_S7RackSlot = await _redisHelper.StringGetAsync<List<S7RackSlotConfig>>(s7RackSlotredisKey);
            if (redis_S7RackSlot == null || redis_S7RackSlot.Count == 0)
            {
                // 从配置文件加载

                var appsetting_S7RackSlot = configuration.GetSection(_redisKeyOptions.S7RackSlotTemple_key).Get<List<S7RackSlotConfig>>();
                await _redisHelper.StringSetAsync(s7RackSlotredisKey, appsetting_S7RackSlot);
                // await _redisHelper.HashSetAsync(s7RackSlotredisKey, appsetting_S7RackSlot);
            }
            #endregion
            #region 启动采集
            await _signalCollector.Init(getAllPLcLinks);
            await _signalCollector.Start();
            #endregion
            #region 启动信号写入监测服务
            _signalWriterService.Init(2000, 50);
            await _signalWriterService.Start();

            //var s7Client = new S7Client("127.0.0.1", CpuType.S71200, 0, 0);
            //s7Client.ConnectAsync().Wait();
            //var continuousWriter = new ContinuousWriter(s7Client);
            //continuousWriter.StartWriting(s7Signals);

            //模拟写入
            //Task.Run(async () =>
            //{
            //    while (_signalWriterService.SimulatedSignalWriteState)
            //    {
            //        foreach (var d in _plclinkManager.PLCLinks)
            //        {
            //            var kk = d.GetSubscribedSignals();
            //            foreach (var k in kk)
            //            {
            //                object value = ContinuousWriter.GetDefaultValueForSignal(k);
            //                 _collectorRedisService.EnqueueAsyncSignalWriteDataAsync(
            //                       new SignalWriteModel(
            //                           d.plclinkInfo.PLCLinkCode,
            //                           k.Address, value
            //                           ));
            //            }
            //             Task.Delay(_signalWriterService.SimulatedSignalWriteInterval);
            //        }

            //    }

            //});

            _simulatedSignalWriter.Start();
            #endregion

        }
        public static IEnumerable<IPlcSignal> CreateS7TestSignals()
        {
            var signals = new List<IPlcSignal>();

            // 定义所有可能的 VarType 和 DataType 的组合
            var varTypes = new[]
            {
            VarType.Byte,
            VarType.Word,
            VarType.DWord,
            VarType.Int,
            VarType.DInt,
            VarType.Real,
             VarType.Bit,
            VarType.S7WString
        };

            var dataTypes = new[]
            {
            DataType.DataBlock,
            //DataType.Input,
            //DataType.Output,
            //DataType.Memory
        };

            // 遍历所有组合，生成测试信号
            int startByte = 0;
            foreach (var dataType in dataTypes)
            {
                foreach (var varType in varTypes)
                {
                    // 配置参数
                    var config = new S7Config
                    {
                        DataType = dataType,
                        VarType = varType,
                        DB = 1, // 默认 DB1
                        StartByteAdr = startByte,
                        Count = varType == VarType.S7WString ? 10 : 1,
                        BitAdr = 0

                    };
                    var dataItem = new DataItem
                    {
                        DataType = dataType,
                        VarType = varType,
                        DB = 1, // 默认 DB1
                        StartByteAdr = startByte,
                        Count = varType == VarType.S7WString ? 10 : 1,
                        BitAdr = 0

                    };
                    var addres = dataItem.ToAddressString();


                    // 使用工厂创建信号
                    var signal = PlcSignalFactory.CreateSignal(
                        ProtocolType.S7,
                        $"{dataType}_{varType}_Signal",
                        $"{ProtocolType.S7}.{addres}",
                        config
                    );
                    signal.ReadCycle = new Random().Next(200, 1000);
                    signals.Add(signal);
                    startByte += config.Count; // 更新地址
                }
            }

            return signals;
        }
            /// <summary>
            /// 组装链路信息
            /// </summary>
            /// <returns></returns>
            public async Task< List<PLCLink>> getAllPLcLinks()
        {
            List<PLCLink> result = new List<PLCLink>();
            #region 加载链路信息
           var plclinksIds = await _redisHelper.StringGetAsync<List<string>>(_redisKeyOptions._PLCLinksIDs);
            if (plclinksIds.Any())
            {
                var task = plclinksIds.Select(async plclinkId =>
                {
                    // 这里调用你的 HashGetAsync 方法获取链路信息
                    var plclinkData = await _redisHelper.StringGetAsync<PLCLinkInfo>($"{_redisKeyOptions._PLCLinkInfo(plclinkId)}");
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
                        plclinks.Add(plclink);
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
                    var d_signals_dic = await _redisHelper.HashGetAsync(_redisKeyOptions._PLCLinksSignals(plclink.plclinkInfo.PLCLinkId));

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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _signalCollector.Stop();
            // 可选：处理停止逻辑
            return Task.CompletedTask;
        }
    }
}
