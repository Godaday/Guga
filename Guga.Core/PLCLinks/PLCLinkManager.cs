using Guga.Core.Interfaces;
using Guga.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.PLCLinks
{
    public class PLCLinkManager : IPLCLinkManager
    {
        public List<PLCLink> PLCLinks { get; set; } = new List<PLCLink>();
        public List<S7RackSlotConfig> s7RackSlotDefaultConfigs { get; set; }

       
    }

}
