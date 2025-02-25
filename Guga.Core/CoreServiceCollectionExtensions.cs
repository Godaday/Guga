using Guga.Core.Interfaces;
using Guga.Core.PLCLinks;
using Microsoft.Extensions.DependencyInjection;

namespace Guga.Core
{
    public static class CoreServiceCollectionExtensions
    {
        /// <summary>
        /// 注册核心服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddGugaCoreServices(this IServiceCollection services)
        {
          

            services.AddTransient<IPLCLinkFactory, PLCLinkFactory>();
            // 注册链路管理器为单例
            services.AddSingleton<IPLCLinkManager, PLCLinkManager>();
            return services;
        }
    }
}
