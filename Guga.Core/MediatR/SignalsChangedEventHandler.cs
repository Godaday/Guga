﻿using Guga.Core.Devices;
using Guga.Transformer.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.MediatR
{
    /// <summary>
    /// 信号改变事件处理程序
    /// </summary>
    public class SignalsChangedEventHandler : INotificationHandler<SignalsChangedEvent>
    {
        public async Task Handle(SignalsChangedEvent notification, CancellationToken cancellationToken)
        {
            var device = notification.Device_;
           var rules = device.GetSignalToBusinessRules();
            foreach (var rule in rules)
            {
                rule.ExecuteAction();
            }
              

            await Task.CompletedTask;
        }
    }
}
