using ColinChang.RedisHelper;
using Guga.Collector.Interfaces;
using Guga.Core.Devices;
using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Core.PlcSignals;
using Guga.Redis;
using Guga.Redis.ConfigModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Guga.BlazorApp
{
    /// <summary>
    /// 设备初始化服务。
    /// </summary>
    public class DevicesInitHostedService : IHostedService
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IRedisHelper _redisHelper;
        private readonly ISignalCollector _signalCollector;
        private readonly IServiceProvider _serviceProvider;
        private readonly RedisKeyOptions _redisKeyOptions;
        public DevicesInitHostedService(IDeviceManager deviceManager, IRedisHelper redisHelper,
            ISignalCollector signalCollector, IServiceProvider serviceProvider, IOptions<RedisKeyOptions> redisKeyOptions
            )
        {
            _deviceManager = deviceManager;
            _redisHelper = redisHelper;
            _signalCollector = signalCollector;
            _serviceProvider = serviceProvider;
            _redisKeyOptions = redisKeyOptions.Value;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();

#if DEBUG
            var deviceFactory = _serviceProvider.GetRequiredService<IDeviceFactory>();
            //创建设备
            DeviceInfo deviceInfo = new  DeviceInfo{
                DeviceId = "DV-2",
                DeviceName = "西门子设备",
                DeviceCode = "WR001",
                DeviceType_ = DeviceType.Universal,
                Ip = "127.0.0.1",
                ProtocolType_ = ProtocolType.S7,
                S7CPUType_ = S7CPUType.S71200,
                rack = 0,
                slot = 1,


            };
            await _redisHelper.StringSetAsync(_redisKeyOptions._DevicesIDs, new List<string>() { "DV-2" });
            await _redisHelper.StringSetAsync(_redisKeyOptions._DeviceInfo(deviceInfo.DeviceId), deviceInfo);
            //创建信号
            var s7Signals = ContinuousWriter.CreateS7TestSignals();
            
           var  cc = s7Signals.ToDictionary(
            signal => signal.SignalName,
            signal => JsonConvert.SerializeObject(signal)
        );
            ConcurrentDictionary<string, string> entries = new ConcurrentDictionary<string, string>();
            foreach (var signal in s7Signals)
            {
                entries.TryAdd(signal.SignalName, JsonConvert.SerializeObject(signal));

            } 
              await _redisHelper.HashSetAsync(_redisKeyOptions._DevicesSignals(deviceInfo.DeviceId), entries);

#endif

            //Check 默认配置是否存在
            #region 从 Redis 加载西门子设备不同型号机架号、插槽数据
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
            #region 加载设备及信号
            #region 加载设备信息
            var devicesIds = await _redisHelper.StringGetAsync<List<string>>(_redisKeyOptions._DevicesIDs);
            if (devicesIds.Any())
            {
                var task = devicesIds.Select(async deviceId =>
                {
                    // 这里调用你的 HashGetAsync 方法获取设备信息
                    var deviceData = await _redisHelper.StringGetAsync<DeviceInfo>($"{_redisKeyOptions._DeviceInfo(deviceId)}");
                    return deviceData;
                });
                var deviceInfos = await Task.WhenAll(task);
                if (deviceInfos.Any())
                {
                    List<Device> devices = new List<Device>();
                    foreach (var d in deviceInfos)
                    {
                        Device device = deviceFactory.CreateDevice<UniversalDevice>(door =>
                        {
                            door.deviceInfo = d;
                        });
                        devices.Add(device);
                    }
                    _deviceManager.Devices.AddRange(devices);
                }

            }
            #endregion
            #region  加载设备信号信息
            if (_deviceManager.Devices.Any())
            {
                _deviceManager.Devices.ForEach(async device =>
                {
                    #region 获取设备信号
                    var  d_signals_dic = await _redisHelper.HashGetAsync(_redisKeyOptions._DevicesSignals(device.deviceInfo.DeviceId));

                    if (d_signals_dic.Any())
                    {
                        List<IPlcSignal> plcSignals = new List<IPlcSignal>();
                        foreach (var signal in d_signals_dic.Values)
                        {
                            if (string.IsNullOrEmpty(signal))
                            {
                                IPlcSignal plcSignal = null;
                                if (device.deviceInfo.ProtocolType_ == Core.Enums.ProtocolType.S7)
                                {
                                    plcSignal = JsonConvert.DeserializeObject<S7Signal>(signal);
                                }
                                else if (device.deviceInfo.ProtocolType_ == Core.Enums.ProtocolType.Modbus)
                                {
                                    plcSignal = JsonConvert.DeserializeObject<ModbusSignal>(signal);
                                }
                                if (plcSignal != null) {
                                    plcSignal.Device = device;
                                    plcSignals.Add(plcSignal);
                                }
                               
                            }
                            
                           
                        }
                        device.SubscribeToSignals(plcSignals);
                    }
                 
                    #endregion
                   

                });

            }

            #endregion
            #endregion


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _signalCollector.StopAllTimers();
            // 可选：处理停止逻辑
            return Task.CompletedTask;
        }
    }
}
