using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// 用于应用配置信息到信号的接口。
    /// </summary>
    public interface IProtocolConfig
    {
        void ApplyConfiguration(IPlcSignal signal);
    }
}
