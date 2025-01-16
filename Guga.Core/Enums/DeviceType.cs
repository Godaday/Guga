using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Enums
{
    /// <summary>
    /// 链路类型
    /// </summary>
   public enum PLCLinkType
    {

        [Description("未知")]
        unknown,
        [Description("自动门")]
        AutomaticDoor,
        [Description("电梯")]
        Elevator,
        [Description("AGV")]
        AGV,
        [Description("RGV")]
        RGV,
        [Description("通用链路")]
        Universal
    }
}
