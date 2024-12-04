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
    /// 自动门
    /// </summary>
    public class AutomaticDoor : Device
    {
        public string DoorStatus { get; private set; } // 门的状态（开/关）

        public AutomaticDoor(string deviceId, string deviceName, string deviceCode)
            : base(deviceId, deviceName, deviceCode, DeviceType.AutomaticDoor)
        {
        }

        public override void TransformerSignals(IEnumerable<IPlcSignal> plcSignals)
        {
            throw new NotImplementedException();
           
        }
    }
}
