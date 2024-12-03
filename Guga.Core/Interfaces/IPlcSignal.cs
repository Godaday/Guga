using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
  public interface IPlcSignal
    {
        /// <summary>
        /// 信号名称
        /// </summary>
        string SignalName { get; set; }

        /// <summary>
        /// 所属设备
        /// </summary>
        IDevice Device { get; set; }

        /// <summary>
        /// 获取当前信号所关联的设备
        /// </summary>
        /// <returns></returns>
        IDevice GetDevice();

        /// <summary>
        /// 信号值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        object GetValue();

        /// <summary>
        /// 设置信号值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
       void  SetValue(object obj);
    }

    public interface IPlcSignal<T> : IPlcSignal
    {
        /// <summary>
        /// 信号值
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// 获取信号的值，返回具体的类型 T
        /// </summary>
        new  T GetValue();
    }
}
