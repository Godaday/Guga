using Guga.Core.Interfaces;
using Guga.Models.Collector;
using Guga.Models.Enums;
using Newtonsoft.Json;

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

        /// <summary>
        /// 从站ID
        /// </summary>
        public byte SlaveId { get; set; }
        /// <summary>
        /// 寄存器类型
        /// </summary>
        public ModbusRegisterType RegisterType { get; set; }
        /// <summary>
        /// 起始地址
        /// </summary>
        public ushort StartAddress { get; set; }
        /// <summary>
        /// 读取长度（以寄存器数量计算）
        /// </summary>
        public ushort Length { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public ModbusDataType VarType { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string ErrorMessage { get; set; }
        public IPLCLink PLCLink { get; set; }
        public DateTime CollectTime { get; set; }
        public ModbusSignal(string signalName, string address)
        {
            SignalName = signalName;
            Address = address;
        }


        /// <summary>
        /// 点位地址转换（地址字符串格式：{从站ID}.{寄存器类型}.{起始地址}.{读取长度} ）
        /// </summary>
        public static (bool Success, byte SlaveId, ModbusRegisterType RegisterType, ushort StartAddress, ushort Length) TryParse(string modbusAddress)
        {
            if (string.IsNullOrWhiteSpace(modbusAddress))
                return (false, 0, default, 0, 0);

            try
            {
                var parts = modbusAddress.Split('.');
                if (parts.Length != 5)
                    return (false, 0, default, 0, 0);

                byte slaveId = byte.Parse(parts[0]);
                ModbusRegisterType registerType = Enum.Parse<ModbusRegisterType>(parts[1], true);
                ushort startAddress = ushort.Parse(parts[2]);
                ushort length = ushort.Parse(parts[3]);

                return (true, slaveId, registerType, startAddress, length);
            }
            catch
            {
                return (false, 0, default, 0, 0);
            }
        }

        public void Configure<TConfig>(TConfig config)
        {
            //if (config is ModbusConfig modbusConfig)
            //{
            //    FunctionCode = modbusConfig.FunctionCode;
            //    StartAddress = modbusConfig.StartRegister;
            //    Length = modbusConfig.RegisterCount;
            //}
            //else
            //{
            //    throw new ArgumentException("Invalid config type for ModbusSignal.");
            //}
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object value, bool updateCollectTime = true)
        {
          Value = value;
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
            var t = new SignalValueModel()
            {
                LinkCode = this.PLCLink.plclinkInfo.PLCLinkCode,
                Address = this.Address,
                Value = this.Value,
                CollectorTime = CollectTime,
                Status = SignalStatus_,
                ErrorMessage = ErrorMessage,
                ReadCycle = ReadCycle// 采集周期（用于计算失效）

            };
            return JsonConvert.SerializeObject(t);
        }
    }
}
