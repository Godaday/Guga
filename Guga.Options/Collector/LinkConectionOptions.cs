using System.ComponentModel.DataAnnotations;

namespace Guga.Options.Collector
{
    public class LinkConectionOptions
    {
        [Required(ErrorMessage = "请配置PLC连接重试上限次数")]
        public int retryCount { get; set; }

        [Required(ErrorMessage = "请配置PLC连接重试间隔时间")]
        public int retryInterval { get; set; }

       
    }
}
