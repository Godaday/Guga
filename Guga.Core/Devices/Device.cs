using Guga.Core.Enums;
using Guga.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Devices
{
    /// <summary>
    /// 设备基础
    /// </summary>
    public abstract class Device : IDevice
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public virtual string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 设备名称
        /// </summary>
        public virtual string DeviceName { get; set; } = string.Empty;
        /// <summary>
        /// 设备编号
        /// </summary>
        public virtual string DeviceCode { get; set; } = string.Empty;
        /// <summary>
        /// 设备类型
        /// </summary>
        public virtual DeviceType DeviceType { get; set; } = DeviceType.unknown;
        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string IpAddress { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        public virtual int? port { get; set; } = null;

        private  HashSet<IPlcSignal> _subscribedSignals;

        private readonly object _lockObject = new object();

        protected Device(string deviceId, string deviceName, string deviceCode, DeviceType deviceType)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceCode = deviceCode;
            DeviceType = deviceType;
            _subscribedSignals = new HashSet<IPlcSignal>(); 
        }
        /// <summary>
        /// 设备订阅信号
        /// </summary>
        /// <param name="signals"></param>
        public virtual void SubscribeToSignals(IEnumerable<IPlcSignal> signals)
        {
            lock (_lockObject)
            {
                foreach (var signal in signals)
                {
                   
                    _subscribedSignals.Add(signal);
                    signal.Device = this;
                }
            }
            UpdateSignals(signals);
        }
        /// <summary>
        /// 取消设备订阅的信号
        /// </summary>
        /// <param name="signals"></param>
        public virtual void UnsubscribeFromSignals(IEnumerable<IPlcSignal> signals)
        {
            lock (_lockObject)
            {
                foreach (var signal in signals)
                {
                    _subscribedSignals.Remove(signal); // 更高效的删除操作
                }
            }
        }

        /// <summary>
        /// 更新信号值
        /// </summary>
        /// <param name="updatedSignals"></param>
        public virtual void UpdateSignals(IEnumerable<IPlcSignal> updatedSignals)
        {
            lock (_lockObject)
            {
                foreach (var updatedSignal in updatedSignals)
                {
                    var signal = _subscribedSignals.FirstOrDefault(s => 
                    s.SignalCode == updatedSignal.SignalCode
                    &&s.Device.DeviceId==updatedSignal.Device.DeviceId
                    );
                    if (signal != null)
                    {
                        // 更新信号的值
                        signal.SetValue(updatedSignal.GetValue());
                    }
                }
               
            }
            //设备信号转换业务状态
            TransformerSignals(_subscribedSignals);
        }
        /// <summary>
        /// 获取设备的订阅信号
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IPlcSignal> GetSubscribedSignals()
        {
            return _subscribedSignals;
        }
        /// <summary>
        /// 根据信号转换设备状态
        /// </summary>
        /// <param name="plcSignals"></param>
        public abstract void TransformerSignals(IEnumerable<IPlcSignal> plcSignals);

        public override string ToString()
        {
            return $"DeviceId:{DeviceId}, DeviceName:{DeviceName}, DeviceCode:{DeviceCode}, DeviceType:{DeviceType}";
        }
    }

}
