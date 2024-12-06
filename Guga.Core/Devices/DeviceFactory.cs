using Guga.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Devices
{
    /// <summary>
    /// 创建设备对象工厂实现
    /// IMediator 对象统一注入，方便设备对象内部调用 MediatR 发送消息
    /// </summary>
    public class DeviceFactory : IDeviceFactory
    {
        private readonly IMediator _mediator;

        public DeviceFactory(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// 创建具体的设备对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configure"></param>
        /// <returns></returns>
        public T CreateDevice<T>(Action<T>? configure = null) where T : Device, new()
        {
            var device = new T {
                _mediator = _mediator
            };
            
            configure?.Invoke(device);
            return device;
        }
    }
}
