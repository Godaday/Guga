using Guga.Core.Interfaces;
using Guga.Models.Collector;

namespace Guga.Core.PLCLinks
{
    /// <summary>
    /// 链路基础
    /// </summary>
    public abstract class PLCLink: IPLCLink
    {
     
      
        /// <summary>
        /// 链路基础信息
        /// </summary>
         public PLCLinkInfo plclinkInfo { get; set; }= new PLCLinkInfo();

        private  HashSet<IPlcSignal> _subscribedSignals = new HashSet<IPlcSignal>();

        private readonly object _lockObject = new object();
    
      
        /// <summary>
        /// 链路订阅信号
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
                        signal.PLCLink = this;
                    }
                }
                UpdateSignals(signals);
            }
            
        }
        /// <summary>
        /// 取消链路订阅的信号
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
                    && s.PLCLink.plclinkInfo.PLCLinkId==updatedSignal.PLCLink.plclinkInfo.PLCLinkId
                    );
                    if (signal != null)
                    {
                        // 更新信号的值
                        signal.SetValue(updatedSignal.GetValue());
                    }
                }
               
            }
            //链路信号转换业务状态
            SignalChangeEvent();
        }
        /// <summary>
        /// 获取链路的订阅信号
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IPlcSignal> GetSubscribedSignals()
        {
            return _subscribedSignals;
        }
       

        public override string ToString()
        {
            return $"PLCLinkId:{plclinkInfo.PLCLinkId}, PLCLinkName:{plclinkInfo.PLCLinkName}, PLCLinkCode:{plclinkInfo.PLCLinkCode}, PLCLinkType:{plclinkInfo.PLCLinkType_}";
        }
        /// <summary>
        /// 信号改变事件
        /// </summary>
        /// <returns></returns>
        public abstract void SignalChangeEvent();


        
    }

}
