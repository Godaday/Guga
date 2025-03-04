﻿using Guga.Core.Interfaces;
using Guga.Models.Collector;

namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// Modbus协议的配置信息管理。
    /// </summary>
    public class ModbusProtocolConfig : IProtocolConfig
    {
        private readonly ModbusConfig _config;

        public ModbusProtocolConfig(ModbusConfig config)
        {
            _config = config;
        }

        public void ApplyConfiguration(IPlcSignal signal)
        {
            if (signal is ModbusSignal modbusSignal)
            {
                modbusSignal.Configure(_config);
            }
            else
            {
                throw new ArgumentException("Invalid signal type for Modbus protocol.");
            }
        }
    }

}
