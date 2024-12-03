﻿using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// 基础信号
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PlcSignal<T> : IPlcSignal<T>
    {
        /// <summary>
        /// 信号名称
        /// </summary>
        public string SignalName { get; set; }
        /// <summary>
        /// 所属设备
        /// </summary>
        public IDevice Device { get; set; }
        /// <summary>
        /// 信号值
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// 获取信号的值，返回具体的类型 T
        /// </summary>
        /// <param name="signalName"></param>
        /// <param name="value"></param>
        /// <param name="device"></param>
        public PlcSignal(string signalName, T value, IDevice device)
        {
            SignalName = signalName;
            Value = value;
            Device = device;
        }

        /// <summary>
        /// 获取信号值
        /// </summary>
        /// <returns></returns>
        public T GetValue() => Value;
        /// <summary>
        /// 设置信号值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value) => Value = value;

        /// <summary>
        /// 获取信号的值，返回 object 类型，统一访问接口
        /// </summary>
        /// <returns></returns>
        object IPlcSignal.GetValue() => Value;

        /// <summary>
        /// 获取信号所属设备
        /// </summary>
        /// <returns></returns>
        public IDevice GetDevice() => Device;
        /// <summary>
        /// 设置信号
        /// </summary>
        /// <param name="obj"></param>
        public void SetValue(object obj)
        {
            Value=(T)obj;
        }
    }

}