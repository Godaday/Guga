using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
  public interface IPlcSignal
    {
      
 
        IDevice Device { get; set; }
      



        /// <summary>
        /// 信号名称。
        /// </summary>
        string SignalName { get; set; }

        /// <summary>
        /// 信号的地址，例如 "DB83.DBD146" 或 "40001"。
        /// </summary>
        string Address { get; set; }

         object GetValue();

        void SetValue(object value);

        /// <summary>
        /// 配置信号的属性。
        /// </summary>
        /// <typeparam name="TConfig">配置的具体类型。</typeparam>
        /// <param name="config">配置信息。</param>
        void Configure<TConfig>(TConfig config);
    }

    
}
