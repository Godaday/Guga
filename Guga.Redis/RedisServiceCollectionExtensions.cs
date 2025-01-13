using ColinChang.RedisHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Guga.Redis
{


    public static class RedisServiceCollectionExtensions
    {
        /// <summary>
        /// 注册 Redis 服务，包括连接和工具类
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="connectionString">Redis 连接字符串</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddGugaRedisServices(this IServiceCollection services,IConfiguration redisconfiguration)
        {
            // 注册 Redis 连接
          
            services.AddRedisHelper(redisconfiguration);
            
            return services;
        }
    }

}
