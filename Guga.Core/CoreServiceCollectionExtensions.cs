using Guga.Core.Devices;
using Guga.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddTransient<IDeviceFactory, DeviceFactory>();
            // 注册设备管理器为单例
            services.AddSingleton<IDeviceManager, DeviceManager>();
            return services;
        }
    }
}
