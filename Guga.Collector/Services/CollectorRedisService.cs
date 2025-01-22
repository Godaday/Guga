using ColinChang.RedisHelper;
using Guga.Collector.Interfaces;
using Guga.Core.Models;
using Guga.Redis.ConfigModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Services
{
   

    public class CollectorRedisService : ICollectorRedisService
    {
        private readonly RedisKeyOptions _redisKeyOptions;//Redis key
        private readonly IRedisHelper _redisHelper;//redis 操作
        public  long _CurrentWriteQueueLength { get; private set; } = 0;
        /// <summary>
        /// cotr
        /// </summary>
        /// <param name="redisKeyOptions"></param>
        /// <param name="redisHelper"></param>
        public CollectorRedisService(IOptions<RedisKeyOptions> redisKeyOptions,
            IRedisHelper redisHelper)
        {

            _redisKeyOptions = redisKeyOptions.Value;
            _redisHelper = redisHelper;
        }
        #region  信号写入相关
        /// <summary>
        /// 信号写入数据入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="signalWriteModel"></param>
        /// <returns>入队后队列长度</returns>
        public async Task<long> EnqueueAsyncSignalWriteDataAsync(SignalWriteModel signalWriteModel)
        {
            return await EnqueueAsync<SignalWriteModel>(_redisKeyOptions._Signals_Write, signalWriteModel);

        }
        /// <summary>
        /// 信号值读取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="signalWriteModel"></param>
        /// <returns>入队后队列长度</returns>
        public async Task<ConcurrentDictionary<string, SignalValueModel>> SearchSignalValueAsync(List<string> keys)
        {
            ConcurrentDictionary<string, string> result
                = await _redisHelper.HashGetFieldsAsync(_redisKeyOptions._Signal_Values, keys);

            ConcurrentDictionary<string, SignalValueModel> targetDict = new ConcurrentDictionary<string, SignalValueModel>();

            foreach (var kvp in result)
            {

                if (!string.IsNullOrWhiteSpace(kvp.Value))
                {
                    // 添加到目标字典
                    targetDict.TryAdd(kvp.Key, JsonConvert.DeserializeObject<SignalValueModel>(kvp.Value));
                }

            }
            return targetDict;

        }
        /// <summary>
        /// 信号写入数据出队（移除数据）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<SignalWriteModel> DequeueSignalWriteDataAsync()
        {
            return await DequeueAsync<SignalWriteModel>(_redisKeyOptions._Signals_Write);

        }
        /// <summary>
        /// 信号写入数据出队，但不会移除
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SignalWriteModel>> PeekSignalWriteDataAsync(long start = 0L, long stop = 0L)
        {
            return await PeekRangeAsync<SignalWriteModel>(_redisKeyOptions._Signals_Write, start, stop);
           
        }
        #endregion
        #region 基础函数
        /// <summary>
        /// 入队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> EnqueueAsync<T>(string key, T value) where T : class
        { 

            var length = await _redisHelper.EnqueueAsync<T>(key, value);
            _CurrentWriteQueueLength = length;
            return length;


        }
        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<T> DequeueAsync<T>(string key) where T : class
        {
            if (_CurrentWriteQueueLength > 0)
            {
                _CurrentWriteQueueLength--;
            }
            return await _redisHelper.DequeueAsync<T>(key);
          

            
            
        }
        /// <summary>
        /// 从队列中读取指定范围的数据，但不会移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">为队列名称</param>
        /// <param name="start">start 和 stop 用于指定读取范围</param>
        /// <param name="stop">start 和 stop 用于指定读取范围</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> PeekRangeAsync<T>(string key, long start = 0L, long stop = -1L) where T : class
        {
            return await _redisHelper.PeekRangeAsync<T>(key, start, stop);
        }
        #endregion
        #region 其他
        public async Task<List<S7RackSlotConfig>> S7RackSlotConfigs()
        {
            var s7RackSlotredisKey = _redisKeyOptions._S7RackSlot;
            var redis_S7RackSlot = await _redisHelper.StringGetAsync<List<S7RackSlotConfig>>(s7RackSlotredisKey);
            return redis_S7RackSlot;
        }
       
        #endregion

    }
}
