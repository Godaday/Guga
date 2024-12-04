using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guga.Core.Enums;

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

        public Elevator(string deviceId, string deviceName, string deviceCode)
            : base(deviceId, deviceName, deviceCode, DeviceType.Elevator)
        {
        }

        public override void TransformerSignals(IEnumerable<IPlcSignal> plcSignals)
        {
            throw new NotImplementedException();
        }
    }
}
