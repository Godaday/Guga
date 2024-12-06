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
    public class WarehouseRobot: Device
    {
        public string status { get; set; } = string.Empty;
      

      

        public override void UpdateSignals(IEnumerable<IPlcSignal> updatedSignals)
        {
            base.UpdateSignals(updatedSignals);
        }
        public override string ToString()
        {
            return $"DeviceId:{DeviceId}, DeviceName:{DeviceName}, DeviceCode:{DeviceCode}, DeviceType:{DeviceType_} status:{status}";
        }

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
