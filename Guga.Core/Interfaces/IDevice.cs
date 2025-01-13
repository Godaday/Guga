using Guga.Core.Enums;
using Guga.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    /// <summary>
    /// 设备接口
    /// </summary>
   public interface IDevice
    {

        /// <summary>
        /// 设备基础信息
        /// </summary>
        public DeviceInfo deviceInfo { get; set; }

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
        /// 获取当前设备订阅的所有信号
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlcSignal> GetSubscribedSignals();



    }
}
