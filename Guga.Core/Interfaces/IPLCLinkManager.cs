using Guga.Core.PLCLinks;
using Guga.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    public interface IPLCLinkManager
    {


        List<PLCLink> PLCLinks { get; set; }

      
        /// <summary>
        /// 西门子链路CPU Rack Slot号默认配置
        /// </summary>
        List<S7RackSlotConfig> s7RackSlotDefaultConfigs { get; set; }


     

    }
}
