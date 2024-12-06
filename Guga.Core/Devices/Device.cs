using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Transformer.Interfaces;
using MediatR;

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
        /// IP地址
        /// </summary>
        public virtual string IpAddress { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        public virtual int? port { get; set; } = null;

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
