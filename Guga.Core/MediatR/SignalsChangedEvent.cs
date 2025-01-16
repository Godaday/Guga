using Guga.Core.PLCLinks;
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
        public PLCLink PLCLink_ { get; set; }
        public SignalsChangedEvent(IEnumerable<IPlcSignal> plcSignals, PLCLink plclink)
        {
            PlcSignals_ = plcSignals;
            PLCLink_ = plclink;
        }
    }
}
