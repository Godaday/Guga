namespace Guga.Models.Collector
{
    public class S7RackSlotConfig
    {
        /// <summary>
        /// S7 链路类型名称，例如 S7-200, S7-1200 等。
        /// </summary>
        public string S7TypeName { get; set; }

        /// <summary>
        /// 对应的 CPU 类型标识，例如 S7200, S71500 等。
        /// </summary>
        public string S7CPUType { get; set; }

        /// <summary>
        /// 链路的 Rack 配置，可能为 null。
        /// </summary>
        public int? Rack { get; set; }

        /// <summary>
        /// 链路的 Slot 配置，可能为 null。
        /// </summary>
        public int? Slot { get; set; }

        /// <summary>
        /// 备注信息，说明链路的通信方式或特殊配置。
        /// </summary>
        public string Remark { get; set; }
    }

}
