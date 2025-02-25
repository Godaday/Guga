using Guga.Models.Collector;

namespace Guga.Core.Interfaces
{
    /// <summary>
    /// 链路接口
    /// </summary>
    public interface IPLCLink
    {

        /// <summary>
        /// 链路基础信息
        /// </summary>
        public PLCLinkInfo plclinkInfo { get; set; }

        /// <summary>
        /// 订阅多个信号
        /// </summary>
        /// <param name="signals"></param>
        public  void SubscribeToSignals(IEnumerable<IPlcSignal> signals);

        /// <summary>
        /// 取消订阅多个信号
        /// </summary>
        /// <param name="signals"></param>
        void UnsubscribeFromSignals(IEnumerable<IPlcSignal> signals);

        /// <summary>
        /// 更新多个信号
        /// </summary>
        /// <param name="updatedSignals"></param>
        void UpdateSignals(IEnumerable<IPlcSignal> updatedSignals);

        /// <summary>
        /// 获取当前链路订阅的所有信号
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlcSignal> GetSubscribedSignals();



    }
}
