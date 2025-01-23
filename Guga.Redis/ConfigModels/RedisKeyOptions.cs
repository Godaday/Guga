using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Redis.ConfigModels
{
    public class RedisKeyOptions
    {
        
        [Required(ErrorMessage = "请配置RedisKey前缀")]
        public string KeyPrefix { get; set; }

        [Required(ErrorMessage = "请配置西门子链路默认CPU机架号、槽号信息存储的RedisKey")]
        public string S7RackSlotTemple_key { get; set; }

        [Required(ErrorMessage = "请配置存储链路ID Set集合的RedisKey")]
        public string PLCLinks_IDs_key { get; set; }

        [Required(ErrorMessage = "请配置存储链路信息的RedisKey")]
        public string PLCLink_key { get; set; }

        

        [Required(ErrorMessage = "请配置存储链路信号信息的RedisKey")]
        public string PLCLinks_Signals_key { get; set; }

        [Required(ErrorMessage = "请配置信号写入队列的RedisKey")]
        public string Signals_Write_Queue_key { get; set; }

        [Required(ErrorMessage = "请配置存储信号值的RedisKey")]
        public string Signal_Values_key { get; set; }
        [Required(ErrorMessage = "请配置服务注册RedisKey")]
        public string Server_Register_key { get; set; }

        /// <summary>
        /// s7链路CPU机架号、槽号信息存储的RedisKey
        /// </summary>
        public string _S7RackSlot => $"{KeyPrefix}:{S7RackSlotTemple_key}";
        /// <summary>
        /// 链路ID存储Key
        /// </summary>
        public string _PLCLinksIDs => $"{KeyPrefix}:{PLCLinks_IDs_key}";
        /// <summary>
        /// 获取链路存储Key
        /// </summary>
        public string _PLCLink(string plclinkId) => $"{KeyPrefix}:{PLCLink_key}:{plclinkId}";
        public string _PLCLinkInfo(string plclinkId) => $"{_PLCLink(plclinkId)}:info";
        /// <summary>
        /// 获取存储链路信号信息的Key
        /// </summary>
        public string _PLCLinksSignals (string plclinkId)=> $"{_PLCLink(plclinkId)}:{PLCLinks_Signals_key}";

        /// <summary>
        /// 信号写入队列的RedisKey
        /// </summary>
        public string _Signals_Write=> $"{KeyPrefix}:{Signals_Write_Queue_key}";

        public string _Signal_Values => $"{KeyPrefix}:{Signal_Values_key}";
        public string _Server_Register => $"{KeyPrefix}:{Server_Register_key}";



    }
}
