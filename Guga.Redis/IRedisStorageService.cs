using Guga.Redis.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Redis
{

    public interface IRedisStorageService
    {
        public Task<IEnumerable<T>> GetDeviceIdsAsync<T>(RedisKeyOptions redisKeyOptions);
        public Task<IEnumerable<T>> SetDeviceIdsAsync<T>(RedisKeyOptions redisKeyOptions);
        public Task<T> GetAllDeviceAsync<T>(RedisKeyOptions redisKeyOptions);
        public Task<T> GetDeviceAsync<T>(string deviceId);

    }
}
