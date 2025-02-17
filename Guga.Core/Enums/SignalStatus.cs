using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Enums
{
    public enum SignalStatus
    {
        /// <summary>
        /// 信号正常
        /// </summary>
        Normal,
        /// <summary>
        /// 信号异常
        /// </summary>
        Abnormal,

        /// <summary>
        /// 信号失效
        /// </summary>
        Invalid,
        /// <summary>
        /// 信号未知
        /// </summary>
        Unknown

    }
}
