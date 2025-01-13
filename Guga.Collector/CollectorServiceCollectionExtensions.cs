using Guga.Collector.Interfaces;
using Guga.Collector.Services;

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
           services.AddSingleton<ISignalCollector, SignalCollector>();
            return services;
        }
    }
}
