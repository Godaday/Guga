using Guga.Collector.Services;
using Guga.Core.Interfaces;
using Guga.Core.PLCLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Interfaces
{
    public interface ISignalWriter
    {
        /// <summary>
        /// 初始化信号写入服务
        /// </summary>
        /// <param name="plclinks">链路信息</param>
        /// <param name="getSignalWrites">获取待写入信号数据</param>
        /// <param name="requestInterval">获取频率</param>
        /// <param name="semaphoreSlim_MaxCount">最大并发数</param>
        public SignalWriter Init(List<PLCLink> plclinks, Func<Dictionary<IPlcSignal, Object>> getSignalWrites, int requestInterval=200, int semaphoreSlim_MaxCount = 10);
        public void Start() { }
        public void Stop() { }
    }
}
