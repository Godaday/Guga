using Guga.Redis.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Redis
{
    public class RedisStorageService: IRedisStorageService
    {
        public Task<IEnumerable<T>> GetPLCLinkIdsAsync<T>(RedisKeyOptions redisKeyOptions)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> SetPLCLinkIdsAsync<T>(RedisKeyOptions redisKeyOptions)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllPLCLinkAsync<T>(RedisKeyOptions redisKeyOptions)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetPLCLinkAsync<T>(string plclinkId)
        {
            throw new NotImplementedException();
        }
    }
}
