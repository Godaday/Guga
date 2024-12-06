using Guga.Core.Devices;
using Guga.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.MediatR
{
    /// <summary>
    /// 信号变化事件
    /// </summary>
    public class SignalsChangedEvent: INotification
    {
       public IEnumerable<IPlcSignal> PlcSignals_ { get; set; }
        public Device Device_ { get; set; }
        public SignalsChangedEvent(IEnumerable<IPlcSignal> plcSignals, Device device)
        {
            PlcSignals_ = plcSignals;
            Device_ = device;
        }
    }
}
