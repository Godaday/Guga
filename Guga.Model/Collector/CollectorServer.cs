using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Models.Collector
{
    public class CollectorServer
    {
        //      "ServerName": "default Server", //服务名称
        //"ServerCode": "server-01",
        //"LockSeconds": 20, //锁定周期
        //"RenewalLockSeconds": 15 //续锁周期
        /// <summary>
        /// 服务名称
        /// </summary>
        [Required(ErrorMessage = "请配置采集器服务名称")]
        public string ServerName { get; set; }
        /// <summary>
        /// 服务Code
        /// </summary>
        [Required(ErrorMessage = "请配置采集器服务Code")]
        public string ServerCode { get; set; }

        public int LockSeconds { get; set; }

        public int RenewalLockSeconds { get; set; }


    }
}
