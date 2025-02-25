using Guga.Collector.Interfaces;
using Guga.Collector.Services;
using Guga.Options.Collector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PLCCollect.Collector.Interfaces;
using PLCCollect.Collector.Services;

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
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IPlcConnectionManager>(provider => {

                var options = provider.GetRequiredService<IOptions<LinkConectionOptions>>().Value;
                var logService = provider.GetRequiredService<ILogService>();
                return  new PlcConnectionManager(options.retryInterval, options.retryCount, logService);
                
            });//采集器连接失败重试次数，及重试间隔
            services.AddSingleton<ISignalCollector, SignalCollector>();
            services.AddSingleton<ICollectorRedisService, CollectorRedisService>();
            services.AddSingleton<ISignalWriter, SignalWriter>();
            //模拟信号写入产生服务
            services.AddSingleton<ISimulatedSignalWriter, SimulatedSignalWriter>();
            services.AddSingleton<IMasterElectionService, MasterElectionService>();
            services.AddSingleton<IMasterServeStatus, MasterServeStatus>();
            return services;
        }
    }
}
