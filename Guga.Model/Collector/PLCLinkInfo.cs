using Guga.Models.Enums;

namespace Guga.Models.Collector
{
    public class PLCLinkInfo
    {
        /// <summary>
        /// 链路ID
        /// </summary>
        public  string PLCLinkId { get; set; } = string.Empty;
        /// <summary>
        /// 链路名称
        /// </summary>
        public  string PLCLinkName { get; set; } = string.Empty;
        /// <summary>
        /// 链路编号
        /// </summary>
        public  string PLCLinkCode { get; set; } = string.Empty;

        /// <summary>
        /// 链路状态（默认启用）
        /// </summary>
        public  PLCLinkState PLCLinkState_ { get; set; } = PLCLinkState.Enabled;
        /// <summary>
        /// 链路类型
        /// </summary>
        public  PLCLinkType PLCLinkType_ { get; set; } = PLCLinkType.unknown;


        /// <summary>
        /// 链路型号 如西门子 S7-300，不需要的可为空
        /// </summary>
        public  S7CPUType? S7CPUType_ { get; set; } = null;
        /// <summary>
        /// 通信协议
        /// </summary>
        public  ProtocolType ProtocolType_ { get; set; }

        /// <summary>
        /// 机架号（s7）
        /// </summary>
        public  short rack { get; set; }
        /// <summary>
        /// 槽号(s7)
        /// </summary>
        public  short slot { get; set; }


        /// <summary>
        /// IP地址
        /// </summary>
        public  string Ip { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        public  int? Port { get; set; } = null;

       public string GetKey()
        {
            return  $"{Ip}:{Port}:{ProtocolType_}";
        }
    }
}
