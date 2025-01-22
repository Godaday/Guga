using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.ConfigModel
{
    public class LinkConectionOptions
    {
        [Required(ErrorMessage = "请配置PLC连接重试上限次数")]
        public int retryCount { get; set; }

        [Required(ErrorMessage = "请配置PLC连接重试间隔时间")]
        public int retryInterval { get; set; }

       
    }
}
