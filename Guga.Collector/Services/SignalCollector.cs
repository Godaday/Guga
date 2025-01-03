using Guga.Collector.Interfaces;
using Guga.Core.Devices;
using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Guga.Collector.Services
{
    public class SignalCollector: ISignalCollector
    {
        private readonly object _lock_ = new();
        private readonly Dictionary<int,Timer> _timers = new (); // 定时器，触发信号采集
        public readonly Dictionary<int,List<Device>> groups_= new (); // 刷新频率设备列表
        public readonly PlcConnectionManager _connectionManager;
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(10); // 限制最多10个并发操作


        public SignalCollector(List<Device> devices,TimeSpan timerSpan)
        {
            
           
            _connectionManager = new PlcConnectionManager(devices);
            //初始化频率-设备分组
            devices.ForEach(d => AddDevice(d));
            //初始化定时器
            foreach (var item in groups_)
            {
                AddTimer(item);
            }



        }

        private void AddTimer(KeyValuePair<int,List<Device>> keyValuePair)
        {
            var readCycle = keyValuePair.Key;
            TimeSpan timeSpan =TimeSpan.FromMilliseconds(readCycle);
            if (!_timers.ContainsKey(readCycle))
            {
                _timers.Add(readCycle,
                    new Timer(async _ => 
                    await CollectSignalsWithConcurrencyLimit(keyValuePair.Value),
                    null, 
                    TimeSpan.Zero,
                    timeSpan
                    ));
            }
        }

        private void AddDevice(Device device)
        {
            lock (_lock_)
            {
                //没有所属分组则创建个新分组
                if (!groups_.ContainsKey(device.ReadCycle))
                {
                    groups_[device.ReadCycle] = new List<Device>() { device };
                }
                else
                {
                    groups_[device.ReadCycle].Add(device);
                }
            }
        }
        private void RemoveDevice(Device device)
        {
            lock (_lock_)
            {
               
                if (groups_.ContainsKey(device.ReadCycle))
                {
                    groups_[device.ReadCycle].Remove(device); ;
                }
                
            }


        }
        /// <summary>
        /// 异步并发采集多个设备的信号
        /// </summary>
        /// <param name="devices">设备列表</param>
        /// <param name="signals">信号列表</param>
        /// <returns></returns>
        public async Task CollectSignalsWithConcurrencyLimit(List<Device> _devices)
        {
            var collectTasks = _devices.Select(async device =>
            {
                await Semaphore.WaitAsync();  // 等待可用的并发资源
                try
                {
                    await CollectDeviceSignalsAsync(device);
                    device.UpdateSignals(device.GetSubscribedSignals());  // 更新设备的信号
                }
                finally
                {
                    Semaphore.Release();  // 释放资源
                }
            });
            await Task.WhenAll(collectTasks);
        }

        /// <summary>
        /// 异步采集单个信号
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <param name="signal">信号对象</param>
        /// <returns></returns>
        public async Task CollectSignalAsync(Device device, IPlcSignal signal)
        {
            var connection = _connectionManager.GetConnection(device);
            var signalResult = await connection.ReadDataAsync(signal);
            if (signalResult.IsSuccess)
            {
                signal.SetValue(signalResult.Data);  // 更新信号值
            }
            
           // device.UpdateSignals(new List{ signal });  // 更新设备的信号
        }
        /// <summary>
        /// 异步采集单个设备的信号
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <param name="signals">信号列表</param>
        /// <returns></returns>
        private async Task CollectDeviceSignalsAsync(Device device)
        {
            // 获取设备的连接
            var connection = _connectionManager.GetConnection(device);
            if (!connection.IsConnected())
            {
                await connection.ConnectAsync();
            }
            // 使用PLC连接采集信号
            foreach (var signal in device.GetSubscribedSignals())
            {
                var signalResult = await connection.ReadDataAsync(signal);
                if (signalResult.IsSuccess)
                {
                    signal.SetValue(signalResult.Data);
                }
                
            }
            device.UpdateSignals(device.GetSubscribedSignals());
        }
    }
}
