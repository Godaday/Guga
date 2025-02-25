using Guga.Core.PLCLinks;
using Guga.Models.Collector;

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
