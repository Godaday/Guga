using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Enums
{
    /// <summary>
    /// 设备类型
    /// </summary>
   public enum DeviceType
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
        [Description("通用设备")]
        Universal
    }
}
