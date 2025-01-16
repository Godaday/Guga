using Guga.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.PLCLinks
{
    /// <summary>
    /// 创建链路对象工厂实现
    /// IMediator 对象统一注入，方便链路对象内部调用 MediatR 发送消息
    /// </summary>
    public class PLCLinkFactory : IPLCLinkFactory
    {
        private readonly IMediator _mediator;

        public PLCLinkFactory(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// 创建具体的链路对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configure"></param>
        /// <returns></returns>
        public T CreatePLCLink<T>(Action<T>? configure = null) where T : PLCLink, new()
        {
            var plclink = new T
            {
                _mediator = _mediator
            };
            //var plclink = new T();
            configure?.Invoke(plclink);
            return plclink;
        }
    }
}
