using Guga.Core.PLCLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.delegates
{
    /// <summary>
    /// 获取所有的PLC链路
    /// </summary>
    /// <returns></returns>
    public delegate Task<List<PLCLink>> GetPlcLinksDelegate();

}
