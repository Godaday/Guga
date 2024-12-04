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
        /// 信号编号
        /// </summary>
        string SignalCode { get; set; }

        /// <summary>
        /// 所属设备
        /// </summary>
        IDevice Device { get; set; }

        /// <summary>
        /// 获取当前信号所关联的设备
        /// </summary>
        /// <returns></returns>
        IDevice? GetDevice();

        /// <summary>
        /// 信号值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        object GetValue();
        /// <summary>
        /// 信号值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void SetValue(object t);

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

        void SetValue(T obj);
    }
}
