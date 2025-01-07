using S7.Net;

namespace Guga.Core.Models
{
    /// <summary>
    /// S7协议信号的配置信息。
    /// </summary>
    public class S7Config
    {
        public S7.Net.DataType DataType { get; set; }
        public S7.Net.VarType VarType { get; set; }
        public int DB { get; set; }
        public int StartByteAdr { get; set; }
        public byte BitAdr { get; set; }
        public int Count { get; set; }
    }
}
