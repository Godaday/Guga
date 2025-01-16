using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Enums
{
    /// <summary>
    /// 链路型号
    /// </summary>
    public  enum S7CPUType
    {
        /// <summary>
        /// S7 200 cpu type
        /// </summary>
        S7200 = 0,

        /// <summary>
        /// Siemens Logo 0BA8
        /// </summary>
        Logo0BA8 = 1,

        /// <summary>
        /// S7 200 Smart
        /// </summary>
        S7200Smart = 2,

        /// <summary>
        /// S7 300 cpu type
        /// </summary>
        S7300 = 10,

        /// <summary>
        /// S7 400 cpu type
        /// </summary>
        S7400 = 20,

        /// <summary>
        /// S7 1200 cpu type
        /// </summary>
        S71200 = 30,

        /// <summary>
        /// S7 1500 cpu type
        /// </summary>
        S71500 = 40,
    }
}
