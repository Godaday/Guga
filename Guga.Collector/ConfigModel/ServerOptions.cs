using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.ConfigModel
{
    public class ServerOptions
    {

        /// <summary>
        /// 服务名称
        /// </summary>
        public string? ServerName { get; set; }

        [Required(ErrorMessage = "请配置服务编码")]
        public string? ServerCode { get; set; }

        [Required(ErrorMessage = "请配置服务运行锁定时间")]
        public int LockSeconds { get; set; }

        [Required(ErrorMessage = "请配置服务续锁时间")]
        public int RenewalLockSeconds { get; set; }

        [Required(ErrorMessage = "请配置信号写入批次间隔")]
        public int WriteBatchInterval { get; set; }

        [Required(ErrorMessage = "请配置信号写入批次大小")]
        public int WriteBatchSize { get; set; }
    }
}
