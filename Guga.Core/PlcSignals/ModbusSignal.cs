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
        public string SignalName { get; set; }
        public string Address { get; set; }
        public object? Value { get; set; }
        public byte FunctionCode { get; set; }
        public ushort StartRegister { get; set; }
        public ushort RegisterCount { get; set; }
        public IDevice Device { get; set; }

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

        public void SetValue(object value)
        {
            value = Value;
        }
    }
}
