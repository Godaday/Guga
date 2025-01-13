using Guga.Core.Interfaces;
using Guga.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Devices
{
    public class DeviceManager : IDeviceManager
    {
        public List<Device> Devices { get; set; } = new List<Device>();
        public List<S7RackSlotConfig> s7RackSlotDefaultConfigs { get; set; }

       
    }

}
