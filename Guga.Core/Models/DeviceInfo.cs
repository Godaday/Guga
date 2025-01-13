using Guga.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Models
{
    public class DeviceInfo
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public  string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 设备名称
        /// </summary>
        public  string DeviceName { get; set; } = string.Empty;
        /// <summary>
        /// 设备编号
        /// </summary>
        public  string DeviceCode { get; set; } = string.Empty;

        /// <summary>
        /// 设备状态（默认启用）
        /// </summary>
        public  DeviceState DeviceState_ { get; set; } = DeviceState.Enabled;
        /// <summary>
        /// 设备类型
        /// </summary>
        public  DeviceType DeviceType_ { get; set; } = DeviceType.unknown;


        /// <summary>
        /// 设备型号 如西门子 S7-300，不需要的可为空
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
        /// 读取周期，以毫秒为单位，例如：1000ms
        /// </summary>
        public  int ReadCycle { get; set; } = 2000;

        /// <summary>
        /// IP地址
        /// </summary>
        public  string Ip { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        public  int? Port { get; set; } = null;
    }
}
