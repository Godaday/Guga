using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guga.Core.Enums;
namespace Guga.Core.Devices
{
    public class WarehouseRobot: Device
    {
        public string status { get; set; } = string.Empty;
        public WarehouseRobot(string deviceId, string deviceName, string deviceCode, string deviceType)
            : base(deviceId, deviceName, deviceCode, DeviceType.AGV)
        {
        }

        public override void TransformerSignals(IEnumerable<IPlcSignal> plcSignals)
        {
            if (plcSignals.FirstOrDefault(x => x.SignalName == "状态")?.GetValue().ToString() == "1")
            {
                status = "正常";
            }
        }

        public override void UpdateSignals(IEnumerable<IPlcSignal> updatedSignals)
        {
            base.UpdateSignals(updatedSignals);
        }
        public override string ToString()
        {
            return $"DeviceId:{DeviceId}, DeviceName:{DeviceName}, DeviceCode:{DeviceCode}, DeviceType:{DeviceType} status:{status}";
        }
    }
  
}
