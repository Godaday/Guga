using Guga.Core.Devices;
using Guga.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    public interface IDeviceManager
    {


        List<Device> Devices { get; set; }

        /// <summary>
        /// 西门子设备CPU Rack Slot号默认配置
        /// </summary>
        List<S7RackSlotConfig> s7RackSlotDefaultConfigs { get; set; }


     

    }
}
