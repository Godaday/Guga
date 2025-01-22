using Guga.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Interfaces
{
    public interface ICollectorRedisService
    {
        public long _CurrentWriteQueueLength { get;  }//队列长度
        Task<T> DequeueAsync<T>(string key) where T : class;
        Task<SignalWriteModel> DequeueSignalWriteDataAsync();
        Task<long> EnqueueAsync<T>(string key, T value) where T : class;
        Task<long> EnqueueAsyncSignalWriteDataAsync(SignalWriteModel signalWriteModel);
        Task<IEnumerable<T>> PeekRangeAsync<T>(string key, long start = 0L, long stop = 0L) where T : class;
        Task<IEnumerable<SignalWriteModel>> PeekSignalWriteDataAsync(long start = 0L, long stop = 0L);
        /// <summary>
        /// 信号值读取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="signalWriteModel"></param>
        /// <returns>入队后队列长度</returns>
         Task<ConcurrentDictionary<string, SignalValueModel>> SearchSignalValueAsync(List<string> keys);
        Task<List<S7RackSlotConfig>> S7RackSlotConfigs();
        }

}
