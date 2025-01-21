using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Models
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
        public int Status { get; set; } = 0;

      
    }
}
