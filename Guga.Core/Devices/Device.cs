using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Transformer.Interfaces;
using MediatR;
using System.Collections.Generic;

namespace Guga.Core.Devices
{
    /// <summary>
    /// 设备基础
    /// </summary>
    public abstract class Device: IDevice, IDeviceRules
    {
        public IMediator _mediator { get; set; } = null!;
      
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
        public virtual DeviceType DeviceType_ { get; set; } = DeviceType.unknown;


        /// <summary>
        /// 设备型号 如西门子 S7-300，不需要的可为空
        /// </summary>
        public virtual S7CPUType? S7CPUType_ { get; set; } = null;
        /// <summary>
        /// 通信协议
        /// </summary>
        public virtual ProtocolType ProtocolType_ { get; set; }

        /// <summary>
        /// 机架号（s7）
        /// </summary>
        public virtual short rack { get; set; }
        /// <summary>
        /// 槽号(s7)
        /// </summary>
        public virtual short slot { get; set; }
        /// <summary>
        /// 读取周期，以毫秒为单位，例如：1000ms
        /// </summary>
        public virtual int ReadCycle { get; set; } = 2000;

        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string Ip { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        public virtual int? Port { get; set; } = null;

        private  HashSet<IPlcSignal> _subscribedSignals = new HashSet<IPlcSignal>();

        private readonly object _lockObject = new object();
        /// <summary>
        /// Mediator
        /// </summary>
       
      
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
                    && s.Device.DeviceId==updatedSignal.Device.DeviceId
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
            return $"DeviceId:{DeviceId}, DeviceName:{DeviceName}, DeviceCode:{DeviceCode}, DeviceType:{DeviceType_}";
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
