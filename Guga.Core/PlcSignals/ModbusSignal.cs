using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// 表示一个使用Modbus协议的PLC信号。
    /// </summary>
    public class ModbusSignal : IPlcSignal
    {
        /// <summary>
        /// 信号状态
        /// </summary>
       public SignalStatus SignalStatus_ { get; set; }
        /// <summary>
        /// 读取周期，以毫秒为单位，例如：1000ms
        /// </summary>
        public int ReadCycle { get; set; }= 200;
        public string SignalName { get; set; }
        public string Address { get; set; }
        public object? Value { get; set; }
        public byte FunctionCode { get; set; }
        public ushort StartRegister { get; set; }
        public ushort RegisterCount { get; set; }
        public IPLCLink PLCLink { get; set; }
        public DateTime CollectTime { get; set; }
        public ModbusSignal(string signalName, string address)
        {
            SignalName = signalName;
            Address = address;
        }

        public void Configure<TConfig>(TConfig config)
        {
            if (config is ModbusConfig modbusConfig)
            {
                FunctionCode = modbusConfig.FunctionCode;
                StartRegister = modbusConfig.StartRegister;
                RegisterCount = modbusConfig.RegisterCount;
            }
            else
            {
                throw new ArgumentException("Invalid config type for ModbusSignal.");
            }
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object value, bool updateCollectTime = true)
        {
            value = Value;
            if (updateCollectTime)
            {
                CollectTime = DateTime.Now;
            }

        }
        /// <summary>
        /// 返回信号存储JSON字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetSignalStoreValue() {
            return null;
        }
    }
}
