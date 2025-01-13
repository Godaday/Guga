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

        [Required(ErrorMessage = "请配置西门子设备默认CPU机架号、槽号信息存储的RedisKey")]
        public string S7RackSlotTemple_key { get; set; }

        [Required(ErrorMessage = "请配置存储设备ID Set集合的RedisKey")]
        public string Devices_IDs_key { get; set; }

        [Required(ErrorMessage = "请配置存储设备信息的RedisKey")]
        public string Device_key { get; set; }

        

        [Required(ErrorMessage = "请配置存储设备信号信息的RedisKey")]
        public string Devices_Signals_key { get; set; }

        /// <summary>
        /// s7设备CPU机架号、槽号信息存储的RedisKey
        /// </summary>
        public string _S7RackSlot => $"{KeyPrefix}:{S7RackSlotTemple_key}";
        /// <summary>
        /// 设备ID存储Key
        /// </summary>
        public string _DevicesIDs => $"{KeyPrefix}:{Devices_IDs_key}";
        /// <summary>
        /// 获取设备存储Key
        /// </summary>
        public string _Device(string deviceId) => $"{KeyPrefix}:{Device_key}:{deviceId}";
        public string _DeviceInfo(string deviceId) => $"{_Device(deviceId)}:info";
        /// <summary>
        /// 获取存储设备信号信息的Key
        /// </summary>
        public string _DevicesSignals (string deviceId)=> $"{_Device(deviceId)}:{Devices_Signals_key}";



    }
}
