using Guga.Collector.Interfaces;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Core.PLCLinks;
using Guga.Core.PlcSignals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Services
{
    public class SignalWriter : ISignalWriter
    {
        private readonly IPlcConnectionManager _connectionManager;
        private static SemaphoreSlim Semaphore; // 限制最N个并发操作
        private static Func<Dictionary<IPlcSignal, Object>> _getSignalWriteFunc; // 获取信号的 Func

        public SignalWriter(IPlcConnectionManager plcConnectionManager)
        {
            _connectionManager= plcConnectionManager;
        }
        /// <summary>
        /// 初始化信号写入服务
        /// </summary>
        /// <param name="plclinks">链路信息</param>
        /// <param name="getSignalWrites">获取待写入信号数据</param>
        /// <param name="requestInterval">获取频率 默认200毫秒</param>
        /// <param name="semaphoreSlim_MaxCount">最大并发数</param>
        /// <returns></returns>
        public SignalWriter Init(List<PLCLink> plclinks,
            Func<Dictionary<IPlcSignal, Object>> getSignalWrites,
            int requestInterval = 200,
            int semaphoreSlim_MaxCount = 10
            )
        {
            _connectionManager.Init(plclinks);
            Semaphore = new SemaphoreSlim(semaphoreSlim_MaxCount);
            _getSignalWriteFunc = getSignalWrites;
            return this;
        }
        /// <summary>
        /// 启动写入
        /// </summary>
        public void Start()
        {
            Task.Run(async () =>
            {

            });
        }
        /// <summary>
        /// 停止写入
        /// </summary>
        public void Stop()
        {
            _connectionManager.Dispose();//释放连接
        }
    }
}
     