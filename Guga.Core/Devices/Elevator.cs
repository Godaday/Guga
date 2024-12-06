using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guga.Core.Enums;
using Guga.Transformer.Interfaces;
using MediatR;

namespace Guga.Core.Devices
{
    /// <summary>
    /// 电梯
    /// </summary>
    public class Elevator : Device
    {
        public int CurrentFloor { get; private set; } // 当前楼层
        public bool IsMoving { get; private set; } // 电梯是否在运动
        public bool IsOpenDoor { get; private set; } //是否开门

       

        public override void SignalChangeEvent()
        {
            throw new NotImplementedException();
        }

        public override List<IRule> GetSignalToBusinessRules()
        {
            throw new NotImplementedException();
        }

        public override List<IRule> GetBusinessToSignalRules()
        {
            throw new NotImplementedException();
        }
    }
}
