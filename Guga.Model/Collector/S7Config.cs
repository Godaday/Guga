using S7.Net;

namespace Guga.Models.Collector
{
    /// <summary>
    /// S7协议信号的配置信息。
    /// </summary>
    public class S7Config
    {
        public DataType DataType { get; set; }
        public VarType VarType { get; set; }
        public int DB { get; set; }
        public int StartByteAdr { get; set; }
        public byte BitAdr { get; set; }
        public int Count { get; set; }
    }
}
