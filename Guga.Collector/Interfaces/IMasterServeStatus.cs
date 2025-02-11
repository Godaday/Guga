using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCCollect.Collector.Interfaces
{
   public interface  IMasterServeStatus
    {
        /// <summary>
        /// 当前是否是主服务
        /// </summary>
        public bool IsMaster { get; set; }
    }
}
