using Guga.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
  public interface IPlcSignal
    {
      
 
        IPLCLink PLCLink { get; set; }



        /// <summary>
        /// 读取周期，以毫秒为单位，例如：1000ms
        /// </summary>
         int ReadCycle { get; set; }
        /// <summary>
        /// 信号名称。
        /// </summary>
        string SignalName { get; set; }

        /// <summary>
        /// 信号的地址，例如 "DB83.DBD146" 或 "40001"。
        /// </summary>
        string Address { get; set; }

         object GetValue();
        /// <summary>
        /// 信号状态
        /// </summary>
        SignalStatus SignalStatus_ { get; set; }

        void SetValue(object value,bool updateCollectTime =true);
        /// <summary>
        /// 采集时间
        /// </summary>
        DateTime CollectTime { get; set; }
        /// <summary>
        /// 配置信号的属性。
        /// </summary>
        /// <typeparam name="TConfig">配置的具体类型。</typeparam>
        /// <param name="config">配置信息。</param>
        void Configure<TConfig>(TConfig config);
        /// <summary>
        /// 返回信号存储JSON字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetSignalStoreValue();
    }

    
}
