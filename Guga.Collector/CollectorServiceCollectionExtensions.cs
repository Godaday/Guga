
using Guga.Collector.Interfaces;
using Guga.Collector.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Guga.Collector
{
    public static class CollectorServiceCollectionExtensions
    {
        /// <summary>
        /// 添加采集器服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGugaCollectorServices(this IServiceCollection services)
        {
            services.AddSingleton<IPlcConnectionManager>(provider =>
    new PlcConnectionManager(3000, 10));//采集器连接失败重试次数，及重试间隔
            services.AddSingleton<ISignalCollector, SignalCollector>();
          
            return services;
        }
    }
}
