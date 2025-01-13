using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Transformer.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Guga.Core.Devices
{
    /// <summary>
    /// 设备基础
    /// </summary>
    public abstract class Device: IDevice, IDeviceRules
    {
        /// <summary>
        /// Mediator
        /// </summary>
        public IMediator _mediator { get; set; }
      
        /// <summary>
        /// 设备基础信息
        /// </summary>
         public DeviceInfo deviceInfo { get; set; }= new DeviceInfo();

        private  HashSet<IPlcSignal> _subscribedSignals = new HashSet<IPlcSignal>();

        private readonly object _lockObject = new object();
    
      
        /// <summary>
        /// 设备订阅信号
        /// </summary>
        /// <param name="signals"></param>
        public virtual void SubscribeToSignals(IEnumerable<IPlcSignal> signals)
        {
            if (signals.Any())
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
                    _subscribedSignals.Remove(signal); 
                }
            }
        }

        /// <summary>
        /// 更新信号值，触发信号改变事件
        /// </summary>
        /// <param name="updatedSignals"></param>
        public virtual void UpdateSignals(IEnumerable<IPlcSignal>? updatedSignals)
        {
            IEnumerable < IPlcSignal > signals = updatedSignals ?? this.GetSubscribedSignals();
            lock (_lockObject)
            {
                foreach (var updatedSignal in signals)
                {
                    var signal = _subscribedSignals.FirstOrDefault(s => 
                    s.Address == updatedSignal.Address
                    && s.Device.deviceInfo.DeviceId==updatedSignal.Device.deviceInfo.DeviceId
                    );
                    if (signal != null)
                    {
                        // 更新信号的值
                        signal.SetValue(updatedSignal.GetValue());
                    }
                }
               
            }
            //设备信号转换业务状态
            SignalChangeEvent();
        }
        /// <summary>
        /// 获取设备的订阅信号
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IPlcSignal> GetSubscribedSignals()
        {
            return _subscribedSignals;
        }
       

        public override string ToString()
        {
            return $"DeviceId:{deviceInfo.DeviceId}, DeviceName:{deviceInfo.DeviceName}, DeviceCode:{deviceInfo.DeviceCode}, DeviceType:{deviceInfo.DeviceType_}";
        }
        /// <summary>
        /// 信号改变事件
        /// </summary>
        /// <returns></returns>
        public abstract void SignalChangeEvent();

        /// <summary>
        /// 获取信号转换业务规则
        /// </summary>
        /// <returns></returns>
        public abstract List<IRule> GetSignalToBusinessRules();


        /// <summary>
        /// 获取业务对象转信号的规则
        /// </summary>
        /// <returns></returns>
        public abstract List<IRule> GetBusinessToSignalRules();
        
    }

}
