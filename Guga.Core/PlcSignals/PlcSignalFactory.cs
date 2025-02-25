using Guga.Core.Interfaces;
using Guga.Models.Collector;
using Guga.Models.Enums;


namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// 创建信号实例的工厂类。
    /// </summary>
    public static class PlcSignalFactory
    {
        public static IPlcSignal CreateSignal<TConfig>(ProtocolType protocolType, string name, string address, TConfig config,object value =null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "Config cannot be null.");
            }


            IProtocolConfig protocolConfig = protocolType switch
            {
                ProtocolType.S7 => new S7ProtocolConfig(config as S7Config),
                ProtocolType.Modbus => new ModbusProtocolConfig(config as ModbusConfig),
                _ => throw new NotSupportedException($"Unsupported protocol: {protocolType}")
            };

            IPlcSignal signal = protocolType switch
            {
                ProtocolType.S7 => new S7Signal(name, address, value),
                ProtocolType.Modbus => new ModbusSignal(name, address),
                _ => throw new NotSupportedException($"Unsupported protocol: {protocolType}")
            };

            protocolConfig.ApplyConfiguration(signal);
            return signal;
        }
    }
}
