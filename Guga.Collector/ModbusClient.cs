using Guga.Collector.Interfaces;
using Guga.Collector.Models;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using Guga.Models.Enums;
using NModbus;
using System.Net.Sockets;
using System.Text;

namespace Guga.Collector
{
    public class ModbusClient : IPLCLinkClient
    {
        private IModbusMaster? _modbusMaster;
        private TcpClient? _tcpClient;
        private string _ipAddress = string.Empty;
        private int _port;

        public ModbusClient(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public async Task<Result> ConnectAsync(int retryCount, int delayMilliseconds, CancellationToken cancellationToken)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    // 如果之前有 TCP 连接，先关闭它
                    _tcpClient?.Close();
                    _tcpClient?.Dispose();
                    _tcpClient = new TcpClient();

                    await _tcpClient.ConnectAsync(_ipAddress, _port, cancellationToken);

                    var factory = new ModbusFactory();
                    _modbusMaster = factory.CreateMaster(_tcpClient);

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    // 如果发生异常，确保连接被释放
                    _tcpClient?.Close();
                    _tcpClient?.Dispose();
                    _tcpClient = null; // 避免使用已处于异常状态的对象

                    await Task.Delay(delayMilliseconds, cancellationToken);

                    if (i == retryCount - 1)
                    {
                        return Result.Failure($"Failed to connect to Modbus device. Error: {ex.Message}");
                    }
                }
            }
            return Result.Failure("Failed to connect to Modbus device after retrying.");
        }


        public async Task<Result> DisconnectAsync()
        {
            try
            {
                if (_tcpClient?.Connected == true)
                {
                    _tcpClient.Close();
                    _modbusMaster = null;
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error disconnecting: {ex.Message}");
            }
        }

        public bool IsConnected()
        {
            return _tcpClient?.Connected ?? false;
        }

        public async Task<OperationResult<object>> ReadDataAsync(IPlcSignal plcSignal)
        {
            if (plcSignal is ModbusSignal signal)
            {
                try
                {
                    var values = await ReadRegistersAsync(signal.SlaveId, signal.RegisterType, signal.StartAddress, signal.Length);
                    var data = ModbusDataConverter.ConvertToDataType(values, signal.VarType);
                    return OperationResult<object>.Success(data);
                }
                catch (Exception ex)
                {
                    return OperationResult<object>.Failure($"Error reading data: {ex.Message}");
                }
            }
            else
            {
                return OperationResult<object>.Failure($"signal is  not modbusSignal");
            }
        }
        /// <summary>
        /// 读取多个信号
        /// </summary>
        /// <param name="plcSignals"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<IPlcSignal>>> ReadDataAsync(IEnumerable<IPlcSignal> plcSignals)
        {
            //按从站ID 和 寄存器类型分组
            var groupedSignals = plcSignals
                .OfType<ModbusSignal>()
                .GroupBy(s => new { s.SlaveId, s.RegisterType })
                .ToList();

            var results = new List<IPlcSignal>();

            foreach (var group in groupedSignals)
            {
                //起始地址排序
                var signals = group.OrderBy(s => s.StartAddress).ToList();
                //获取类型的最大请求限制（保持寄存器、输入寄存器 限制125个，线圈、离散限制2000）
                ushort maxBatchSize = group.Key.RegisterType == ModbusRegisterType.HoldingRegister || group.Key.RegisterType == ModbusRegisterType.InputRegister ? (ushort)125 : (ushort)2000;

                int currentIndex = 0;

                while (currentIndex < signals.Count)
                {
                    var batchSignals = new List<ModbusSignal>();
                    ushort batchStartAddress = signals[currentIndex].StartAddress;
                    ushort batchEndAddress = batchStartAddress;
                    //分批请求
                    while (currentIndex < signals.Count && (batchEndAddress - batchStartAddress) + signals[currentIndex].Length <= maxBatchSize)
                    {
                        batchSignals.Add(signals[currentIndex]);
                        batchEndAddress = (ushort)(signals[currentIndex].StartAddress + signals[currentIndex].Length);
                        currentIndex++;
                    }
                    //计算当前批长度
                    ushort length = (ushort)(batchEndAddress - batchStartAddress);

                    try
                    {
                        var values = await ReadRegistersAsync(group.Key.SlaveId, group.Key.RegisterType, batchStartAddress, length);

                        foreach (var signal in batchSignals)
                        {
                            int offset = signal.StartAddress - batchStartAddress;
                            var signalValues = values.Skip(offset).Take(signal.Length).ToArray();
                            var data = ModbusDataConverter.ConvertToDataType(signalValues, signal.VarType);
                            signal.SetValue(data);
                            results.Add(signal);
                        }
                    }
                    catch (Exception ex)
                    {
                        return OperationResult<IEnumerable<IPlcSignal>>.Failure($"Error reading data: {ex.Message}");
                    }
                }
            }
            return OperationResult<IEnumerable<IPlcSignal>>.Success(results);
        }


        public async Task<Result> WriteDataAsync(IPlcSignal plcSignal, object data)
        {
            if (plcSignal is ModbusSignal signal) {

                try
                {
                    if (signal.RegisterType == ModbusRegisterType.InputRegister
                        || signal.RegisterType == ModbusRegisterType.DiscreteInput)
                    {
                        //寄存器和离散输入过滤
                        return Result.Success();
                    }
                        var rawData = ModbusDataConverter.ConvertFromDataType(data, signal.VarType);
                   
                    await WriteRegistersAsync(signal.SlaveId, signal.RegisterType, signal.StartAddress, rawData);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Result.Failure($"Error writing data: {ex.Message}");
                }
            }
            else
            {
                return Result.Failure($"signal is  not modbusSignal");
            }



        }

        public async Task<Result> WriteDataAsync(Dictionary<IPlcSignal, object> data)
        {
            foreach (var kvp in data)
            {
                var result = await WriteDataAsync(kvp.Key, kvp.Value);
                if (!result.IsSuccess) return result;
            }
            return Result.Success();
        }

        private async Task<ushort[]> ReadRegistersAsync(byte slaveId, ModbusRegisterType registerType, ushort startAddress, ushort length)
        {
            try
            {
                return registerType switch
                {
                   
                    ModbusRegisterType.HoldingRegister => await _modbusMaster!.ReadHoldingRegistersAsync(slaveId, startAddress, length),
                    ModbusRegisterType.InputRegister => await _modbusMaster!.ReadInputRegistersAsync(slaveId, startAddress, length),
                    ModbusRegisterType.Coil =>
                        (await _modbusMaster!.ReadCoilsAsync(slaveId, startAddress, length))
                            .Select(b => (ushort)(b ? 1 : 0))
                            .ToArray(),
                    ModbusRegisterType.DiscreteInput =>
                        (await _modbusMaster!.ReadInputsAsync(slaveId, startAddress, length))
                            .Select(b => (ushort)(b ? 1 : 0))
                            .ToArray(),
                    _ => throw new NotSupportedException("Unsupported register type for reading.")
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading Modbus registers: {ex.Message}", ex);
            }
        }

        private async Task WriteRegistersAsync(byte slaveId, ModbusRegisterType registerType, ushort startAddress, ushort[] data)
        {
            try
            {
                switch (registerType)
                {
                    case ModbusRegisterType.HoldingRegister:
                        await _modbusMaster!.WriteMultipleRegistersAsync(slaveId, startAddress, data);
                        break;

                    case ModbusRegisterType.Coil:
                        // 将 ushort[] 转换成 bool[]，0 为 false，其余为 true
                        var coilData = data.Select(d => d > 0).ToArray();
                        await _modbusMaster!.WriteMultipleCoilsAsync(slaveId, startAddress, coilData);
                        break;

                    case ModbusRegisterType.InputRegister:
                    case ModbusRegisterType.DiscreteInput:
                        throw new NotSupportedException($"Cannot write to {registerType}. Input registers and discrete inputs are read-only.");

                    default:
                        throw new NotSupportedException($"Unsupported register type for writing: {registerType}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error writing Modbus registers: {ex.Message}", ex);
            }
        }

    }

    public static class ModbusDataConverter
    {
        public static object ConvertToDataType(ushort[] values, ModbusDataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case ModbusDataType.BOOL:
                        return values[0] > 0;
                    case ModbusDataType.INT16:
                        return (short)values[0];
                    case ModbusDataType.UINT16:
                        return values[0];
                    case ModbusDataType.INT32:
                        return (int)((values[0] << 16) | values[1]);
                    case ModbusDataType.UINT32:
                        return (uint)((values[0] << 16) | values[1]);
                    case ModbusDataType.FLOAT:
                        return BitConverter.ToSingle(BitConverter.GetBytes((values[0] << 16) | values[1]), 0);
                    case ModbusDataType.DOUBLE:
                        return BitConverter.Int64BitsToDouble(((long)values[0] << 48) | ((long)values[1] << 32) | ((long)values[2] << 16) | values[3]);

                    case ModbusDataType.STRING:
                        {
                            // 假设 values 是 ushort[]，每个值代表一个16位寄存器（两个字符）
                            StringBuilder sb = new StringBuilder();
                            foreach (var val in values)
                            {
                                // 高字节和低字节分别是两个 ASCII 字符
                                char highByte = (char)((val >> 8) & 0xFF);  // 高8位
                                char lowByte = (char)(val & 0xFF);          // 低8位
                                sb.Append(highByte).Append(lowByte);
                            }
                            // 去掉字符串末尾的空字符（如果有）
                            return sb.ToString().TrimEnd('\0');
                        }
                    default:
                        throw new NotSupportedException($"Unsupported Modbus data type: {dataType}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting data: {ex.Message}", ex);
            }
        }

        public static ushort[] ConvertFromDataType(object value, ModbusDataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case ModbusDataType.BOOL:
                        return new ushort[] { (bool)value ? (ushort)1 : (ushort)0 };
                    case ModbusDataType.INT16:
                       
                            return new ushort[] { (ushort)(short)Convert.ToInt16(value) };
                    case ModbusDataType.UINT16:
                        return new ushort[] { (ushort)Convert.ToUInt16(value) };
                    case ModbusDataType.INT32:
                        return new ushort[] { (ushort)(Convert.ToInt32(value) >> 16), (ushort)Convert.ToInt32(value) };
                    case ModbusDataType.UINT32:
                        return new ushort[] { (ushort)((uint)Convert.ToUInt32(value) >> 16), (ushort)(uint)Convert.ToUInt32(value) };
                    case ModbusDataType.FLOAT:
                        var floatBytes = BitConverter.GetBytes(Convert.ToSingle(value));
                        return new ushort[] { BitConverter.ToUInt16(floatBytes, 2), BitConverter.ToUInt16(floatBytes, 0) };
                    case ModbusDataType.DOUBLE:
                        var doubleBytes = BitConverter.GetBytes(Convert.ToDouble(value));
                        return new ushort[] {
                    BitConverter.ToUInt16(doubleBytes, 6),
                    BitConverter.ToUInt16(doubleBytes, 4),
                    BitConverter.ToUInt16(doubleBytes, 2),
                    BitConverter.ToUInt16(doubleBytes, 0)
                };
                    case ModbusDataType.STRING:
                        var str = value.ToString();
                        var byteArray = System.Text.Encoding.ASCII.GetBytes(str);
                        var ushortArray = new ushort[(byteArray.Length + 1) / 2];
                        Buffer.BlockCopy(byteArray, 0, ushortArray, 0, byteArray.Length);
                        return ushortArray;
                    default:
                        throw new NotSupportedException($"Unsupported Modbus data type: {dataType}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting data: {ex.Message}", ex);
            }
        }
    }


}
