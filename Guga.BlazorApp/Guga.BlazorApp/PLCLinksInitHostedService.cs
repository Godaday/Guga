﻿using ColinChang.RedisHelper;
using Guga.Collector;
using Guga.Collector.Interfaces;
using Guga.Collector.Services;
using Guga.Core.Interfaces;
using Guga.Core.PLCLinks;
using Guga.Core.PlcSignals;
using Guga.Models.Collector;
using Guga.Models.Enums;
using Guga.Options.Collector;
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


        private readonly ServerOptions _serverOptions;
        private readonly IRedisHelper _redisHelper;
        private readonly IServiceProvider _serviceProvider;
        private readonly RedisKeyOptions _redisKeyOptions;
        private readonly IPLCLinkFactory _pLCLinkFactory;
     
        private readonly ICollectorRedisService _collectorRedisService;
        private readonly ISimulatedSignalWriter _simulatedSignalWriter;
        private readonly IMasterElectionService _masterElectionService;
        private CancellationTokenSource? _cts;
        public PLCLinksInitHostedService( 
            IServiceProvider serviceProvider, IOptions<RedisKeyOptions> redisKeyOptions,
            IPLCLinkFactory pLCLinkFactory,ICollectorRedisService collectorRedisService
            , ISimulatedSignalWriter simulatedSignalWriter,
            IMasterElectionService masterElectionService,
             IRedisHelper redisHelper,
            IOptions<ServerOptions> serverOptions
            )
        {


            _serverOptions = serverOptions.Value;
            _redisHelper = redisHelper;
            _serviceProvider = serviceProvider;
            _redisKeyOptions = redisKeyOptions.Value;
            _pLCLinkFactory = pLCLinkFactory;
        
            _collectorRedisService = collectorRedisService;
            _simulatedSignalWriter= simulatedSignalWriter;
            _masterElectionService = masterElectionService;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();


            //    var plclinkFactory = _serviceProvider.GetRequiredService<IPLCLinkFactory>();
            //    //创建链路
            //    PLCLinkInfo plclinkInfo = new  PLCLinkInfo{
            //        PLCLinkId = "DV-2",
            //        PLCLinkName = "西门子链路",
            //        PLCLinkCode = "WR001",
            //        PLCLinkType_ = PLCLinkType.Universal,
            //        Ip = "127.0.0.1",
            //        ProtocolType_ = ProtocolType.S7,
            //        S7CPUType_ = S7CPUType.S71200,
            //        rack = 0,
            //        slot = 1,
            //    };
            //    await _collectorRedisService._redisHelper.StringSetAsync(_redisKeyOptions._PLCLinksIDs, new List<string>() { "DV-2" });
            //    await _collectorRedisService._redisHelper.StringSetAsync(_redisKeyOptions._PLCLinkInfo(plclinkInfo.PLCLinkId), plclinkInfo);
            //    //创建信号
            //    var s7Signals = CreateS7TestSignals();

            //   var  cc = s7Signals.ToDictionary(
            //    signal => signal.SignalName,
            //    signal => JsonConvert.SerializeObject(signal)
            //);
            //    ConcurrentDictionary<string, string> entries = new ConcurrentDictionary<string, string>();
            //    foreach (var signal in s7Signals)
            //    {
            //        entries.TryAdd(signal.SignalName, JsonConvert.SerializeObject(signal));

            //    } 
            //      await _collectorRedisService._redisHelper.HashSetAsync(_redisKeyOptions._PLCLinksSignals(plclinkInfo.PLCLinkId), entries);



            //    //Check 默认配置是否存在
            //    #region 从 Redis 加载西门子链路不同型号机架号、插槽数据
            //    var s7RackSlotredisKey = _redisKeyOptions._S7RackSlot;
            //    var redis_S7RackSlot = await _collectorRedisService._redisHelper.StringGetAsync<List<S7RackSlotConfig>>(s7RackSlotredisKey);
            //    if (redis_S7RackSlot == null || redis_S7RackSlot.Count == 0)
            //    {
            //        // 从配置文件加载

            //        var appsetting_S7RackSlot = configuration.GetSection(_redisKeyOptions.S7RackSlotTemple_key).Get<List<S7RackSlotConfig>>();
            //        await _collectorRedisService._redisHelper.StringSetAsync(s7RackSlotredisKey, appsetting_S7RackSlot);
            //        // await _redisHelper.HashSetAsync(s7RackSlotredisKey, appsetting_S7RackSlot);
            //    }
            //    #endregion
            #region 启动服务竞选
            _masterElectionService.StartMasterElection(_cts.Token);
            #endregion
            #region 检查信号状态
            CheckSignalStatus(_cts.Token);
            #endregion

        }


        /// <summary>
        /// 检查信号采集数据的状态
        /// </summary>
        /// <param name="cancellationToken"></param>
        private void CheckSignalStatus(CancellationToken cancellationToken)
        {
            var weightBase = _serverOptions.SignalInvalidWeight;//基础权重 (60000ms/1min)
            var delayMilliseconds = _serverOptions.SignalInvalidCheckDelay;//延迟时间
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 1. 从 Redis 加载所有已采集的信号
                        var allSiganlValues = await _redisHelper.HashGetAsync(_redisKeyOptions._Signal_Values);
                        if (allSiganlValues != null)
                        {

                            foreach (var s in allSiganlValues)
                            {
                                var signal = JsonConvert.DeserializeObject<SignalValueModel>(s.Value);
                                if (signal.Status == SignalStatus.Normal
                                && signal.ReadCycle.HasValue)
                                {
                                    // 2. 检查信号状态
                                    var differenceMilliseconds = (System.DateTime.Now - signal.CollectorTime.Value).TotalMilliseconds;
                                    //循环周期小于等于基础权重，权重不变，大于基础权重，权重翻倍
                                    var readCycleWeight = signal.ReadCycle <= weightBase ?
                                    signal.ReadCycle : signal.ReadCycle * 2;
                                    if (differenceMilliseconds > readCycleWeight)
                                    {
                                        // 3. 信号失效 更新信号状态 
                                        signal.Status = SignalStatus.Invalid;
                                        signal.ErrorMessage = string.IsNullOrWhiteSpace(signal.ErrorMessage)
                                        ?"信号失效": signal.ErrorMessage;

                                        ConcurrentDictionary<string, string> entries = new ConcurrentDictionary<string, string>();

                                        entries.TryAdd($"{s.Key}", JsonConvert.SerializeObject(signal));

                                        await _redisHelper.HashSetAsync(_redisKeyOptions._Signal_Values, entries);

                                    }

                                }
                            }


                        }
                        // 异步休眠 1 秒，支持取消
                        await Task.Delay(delayMilliseconds, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // 异步休眠被取消，正常退出循环
                        break;
                    }
                }
                Console.WriteLine("信号状态检查任务已退出");
            });
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
                    var d_signals_dic = await _collectorRedisService._redisHelper.HashGetAsync(_redisKeyOptions._PLCLinksSignals(plclink.plclinkInfo.PLCLinkId));

                    if (d_signals_dic.Any())
                    {
                        List<IPlcSignal> plcSignals = new List<IPlcSignal>();
                        foreach (var signal in d_signals_dic.Values)
                        {
                            if (!string.IsNullOrEmpty(signal))
                            {
                                IPlcSignal plcSignal = null;
                                if (plclink.plclinkInfo.ProtocolType_ == ProtocolType.S7)
                                {
                                    plcSignal = JsonConvert.DeserializeObject<S7Signal>(signal);
                                }
                                else if (plclink.plclinkInfo.ProtocolType_ == ProtocolType.Modbus)
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
            Console.WriteLine("停止主服务竞选");
            _cts?.Cancel(); // 通知停止

            // 可选：处理停止逻辑
            return Task.CompletedTask;
        }
       
    }
}
