using Guga.Core.Interfaces;
using Guga.Models.Collector;

namespace Guga.Core.PLCLinks
{
    public class PLCLinkManager : IPLCLinkManager
    {
        public List<PLCLink> PLCLinks { get; set; } = new List<PLCLink>();
        public List<S7RackSlotConfig> s7RackSlotDefaultConfigs { get; set; }

       
    }

}
