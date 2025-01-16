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
        public Task<IEnumerable<T>> GetPLCLinkIdsAsync<T>(RedisKeyOptions redisKeyOptions);
        public Task<IEnumerable<T>> SetPLCLinkIdsAsync<T>(RedisKeyOptions redisKeyOptions);
        public Task<T> GetAllPLCLinkAsync<T>(RedisKeyOptions redisKeyOptions);
        public Task<T> GetPLCLinkAsync<T>(string plclinkId);

    }
}
