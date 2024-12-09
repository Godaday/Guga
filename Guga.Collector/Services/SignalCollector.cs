using Guga.Collector.Interfaces;
using Guga.Core.Devices;
using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Services
{
    public class SignalCollector
    {
        private readonly Timer _timer; // 定时器，触发信号采集
        public readonly List<Device> _devices; // 所有设备列表
        public readonly PlcConnectionManager _connectionManager;
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(10); // 限制最多10个并发操作


        public SignalCollector(List<Device> devices,TimeSpan timerSpan)
        {
            _devices = devices;
            _connectionManager = new PlcConnectionManager(devices);
            _timer = new Timer(async _ => await CollectSignalsWithConcurrencyLimit(),null, TimeSpan.Zero, timerSpan);
        }
        /// <summary>
        /// 异步并发采集多个设备的信号
        /// </summary>
        /// <param name="devices">设备列表</param>
        /// <param name="signals">信号列表</param>
        /// <returns></returns>
        public async Task CollectSignalsWithConcurrencyLimit()
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
            var signalValue = await connection.ReadSignalAsync(signal);
            signal.SetValue(signalValue);  // 更新信号值
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

            // 使用PLC连接采集信号
            foreach (var signal in device.GetSubscribedSignals())
            {
                var signalValue = await connection.ReadSignalAsync(signal); 
                signal.SetValue(signalValue); 
            }
            device.UpdateSignals(device.GetSubscribedSignals());
        }
    }
}
