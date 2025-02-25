using Guga.Options.Collector;
using Microsoft.Extensions.Options;
using PLCCollect.Collector.Interfaces;

namespace PLCCollect.Collector.Services
{
    public class MasterServeStatus:IMasterServeStatus
    {
        public  ServerOptions serverOptions { get; set; }

        public MasterServeStatus(IOptions<ServerOptions> serverOptions_) {
            serverOptions= serverOptions_.Value;
           
        }
        
        /// <summary>
        /// 当前是否是主服务
        /// </summary>
        public bool IsMaster { get; set; } = false;
       
    }
}
