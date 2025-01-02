using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    /// <summary>
    /// 信号解析器接口，用于解析信号字符串。
    /// </summary>
    public interface ISignalParser
    {
        (string Signal, int Length, string DataType) Parse(string signal);
    }
}
