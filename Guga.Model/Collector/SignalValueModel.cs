﻿using Guga.Models.Enums;

namespace Guga.Models.Collector
{
    public class SignalValueModel
    {
        /// <summary>
        /// 链路名称
        /// </summary>
        public string LinkCode { get; set; }
        /// <summary>
        /// 信号地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 写入信号的地址
        /// </summary>
        public object Value { get; set; }
        public DateTime? CollectorTime  { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public SignalStatus Status { get; set; }= SignalStatus.Normal;

        public string? ErrorMessage { get; set; }
        /// <summary>
        /// 采集周期（用于计算失效）
        /// </summary>
        public int? ReadCycle { get; set; }

    }
}
