using Guga.Collector.Interfaces;
using Guga.Collector.Models;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using S7.Net;
using S7.Net.Types;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace Guga.Collector
{

    public class S7Client : IDeviceClient
    {
        private Plc _plc;


        public S7Client(string ipAddress, CpuType cpuType, short rack, short slot)
        {
            _plc = new Plc(cpuType, ipAddress, rack, slot);

        }

        public async Task<Result> ConnectAsync()
        {
            if (IsConnected())
            {
                return Result.Failure("设备已经连接。");
            }

            try
            {
                await Task.Run(() => _plc.Open());

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"连接设备失败: {ex.Message}");
            }
        }

        public bool IsConnected()
        {
            return _plc != null && _plc.IsConnected;
        }

        public async Task<Result> DisconnectAsync()
        {
            if (!IsConnected())
            {
                return Result.Failure("设备未连接。");
            }

            try
            {
                await Task.Run(() => _plc.Close());

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"断开连接失败: {ex.Message}");
            }
        }

        public async Task<OperationResult<object>> ReadDataAsync(IPlcSignal plcSignal)
        {

            if (plcSignal is S7Signal s7Signal)
            {
                if (!IsConnected())
                {
                    return OperationResult<object>.Failure("设备未连接，无法读取数据。");
                }
                try
                {
                    List<DataItem> dataItems = new List<DataItem>{
                        new DataItem
    {
        DataType = s7Signal.S7DataType,
        VarType = s7Signal.S7VarType,
        DB = s7Signal.DB,
        BitAdr = s7Signal.BitAdr,
        Count = s7Signal.Count,
        StartByteAdr = s7Signal.StartByteAdr,
        Value =new object() // 或者初始化为适合的数据类型
    }};

                    await _plc.ReadMultipleVarsAsync(dataItems);
                    plcSignal.SetValue(dataItems.FirstOrDefault()!.Value);
                    return OperationResult<object>.Success(plcSignal);
                }
                catch (Exception ex)
                {
                    return OperationResult<object>.Failure($"读取信号 '{plcSignal.SignalName}' 数据失败: {ex.Message}");
                }
            }
            else
            {
                return OperationResult<object>.Failure("不支持的信号类型。");
            }


        }

        public async Task<OperationResult<IEnumerable<IPlcSignal>>> ReadDataAsync(IEnumerable<IPlcSignal> signals)
        {
            if (!IsConnected())
            {
                return OperationResult<IEnumerable<IPlcSignal>>.Failure("设备未连接，无法读取数据。");
            }

            try
            {
                var signalList = signals.OfType<S7Signal>().ToList();
                List<DataItem> dataItems = signalList // 筛选出 S7Signal 类型的信号
     .Select(s7Signal => new DataItem
     {
         DataType = s7Signal.S7DataType,
         VarType = s7Signal.S7VarType,
         DB = s7Signal.DB,
         BitAdr = s7Signal.BitAdr,
         Count = s7Signal.Count,
         StartByteAdr = s7Signal.StartByteAdr,
         Value = null // 或者初始化为适合的数据类型
     })
     .ToList();

                var result = new List<IPlcSignal>();
                await _plc.ReadMultipleVarsAsync(dataItems);
                for (int i = 0; i < dataItems.Count; i++)
                {
                    signalList[i].Value = dataItems[i].Value;
                }
                return OperationResult<IEnumerable<IPlcSignal>>.Success(signalList);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<IPlcSignal>>.Failure($"读取数据失败: {ex.Message}");
            }
        }

        public async Task<Result> WriteDataAsync(IPlcSignal signal, object value)
        {
            if (!IsConnected())
            {
                return Result.Failure("设备未连接，无法写入数据。");
            }
            if (signal is S7Signal s7Signal)
            {
                try
                {

                    List<DataItem> dataItems = new List<DataItem>{
                        new DataItem
    {
        DataType = s7Signal.S7DataType,
        VarType = s7Signal.S7VarType,
        DB = s7Signal.DB,
        BitAdr = s7Signal.BitAdr,
        Count = s7Signal.Count,
        StartByteAdr = s7Signal.StartByteAdr,
        Value =ConvertValue(s7Signal.S7VarType,value) // 或者初始化为适合的数据类型
    }};


                    await _plc.WriteAsync(dataItems.ToArray());
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Result.Failure($"写入信号 '{signal}' 数据失败: {ex.Message}");
                }
            }
            else
            {
                return Result.Failure("不支持的信号类型。");
            }
        }

        public async Task<Result> WriteDataAsync(Dictionary<IPlcSignal, object> data)
        {
            if (!IsConnected())
            {
                return Result.Failure("设备未连接，无法写入数据。");
            }

            try
            {
                // 构建 DataItem 列表
                List<DataItem> dataItems = data.Keys
                    .OfType<S7Signal>() // 筛选 S7Signal 类型的信号
                    .Select(s7Signal =>
                    {
                        if (!data.TryGetValue(s7Signal, out var value))
                        {
                            throw new ArgumentException($"No value provided for signal: {s7Signal.SignalName}");
                        }

                        return new DataItem
                        {
                            DataType = s7Signal.S7DataType,
                            VarType = s7Signal.S7VarType,
                            DB = s7Signal.DB,
                            BitAdr = s7Signal.BitAdr,
                            Count = s7Signal.Count,
                            StartByteAdr = s7Signal.StartByteAdr,
                            Value = ConvertValue(s7Signal.S7VarType, value)
                        };
                    })
                    .ToList();
                await _plc.WriteAsync(dataItems.ToArray());
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"写入数据失败: {ex.Message}");
            }
        }
        private static object ConvertValue(VarType varType, object value)
        {
//         Please refer to the source code of the class to see its methods.With them you can insert/extract:
//         BITS
//         WORD(unsigned 16 bit integer)
//         INT(signed 16 bit integer)
//         DWORD(unsigned 32 bit integer)
//         DINT(signed 32 bit integer)
//         REAL(32 bit floating point number)
//         S7 Strings
//         S7 Array of char
//         The class is able also to read/write a S7 DATE_AND_TIME variable mapping it into the native language format and vice-versa:
//         S7 DT<-> .NET DateTime
//         S7 DT <-> Pascal TDateTime
//         S7 DT <-> C++ tm struct


            return varType switch
            {
                VarType.Byte => Convert.ToByte(value),
                VarType.Word => Convert.ToUInt16(value),
                VarType.DWord => Convert.ToUInt32(value),
                VarType.Int => Convert.ToInt16(value),
                VarType.DInt => Convert.ToInt32(value),
                VarType.Real => Convert.ToSingle(value),
                VarType.Bit => Convert.ToBoolean(value),
                VarType.S7WString => Convert.ToString(value),
                _ => throw new NotSupportedException($"Unsupported VarType: {varType}")
            };
        }



    }
}

