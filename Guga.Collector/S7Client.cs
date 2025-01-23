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
using System.Text.RegularExpressions;

namespace Guga.Collector
{

    public class S7Client : IPLCLinkClient
    {
        private object _lock = new object();
        private Plc _plc;


        public S7Client(string ipAddress, CpuType cpuType, short rack, short slot)
        {
            _plc = new Plc(cpuType, ipAddress, rack, slot);

        }

        public async Task<Result> ConnectAsync(int retryCount, int delayMilliseconds, CancellationToken cancellationToken)
        {
            if (IsConnected())
            {
                 return Result.Success();
            }
            var _retryCount= retryCount;
            //-1 表示一直重试
            if (retryCount == -1)
            {
                _retryCount=int.MaxValue;
            }
            for (int attempt = 1; attempt <= _retryCount; attempt++)
            {
                try
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Run(() => _plc.Open());
                    }
                    else {

                        return Result.Failure($"重连服务取消");
                    }
                   

                    if (IsConnected())
                    {
                        return Result.Success();
                    }
                }
                catch (Exception ex)
                {
                    // 打印或记录每次失败的信息
                    Console.WriteLine($"连接链路失败（第 {attempt} 次尝试）：{ex.Message}");

                    if (attempt == retryCount)
                    {
                        return Result.Failure($"连接链路失败: {ex.Message}");
                    }
                }

                // 等待一段时间再尝试
                await Task.Delay(delayMilliseconds);
            }

            return Result.Failure("连接链路失败：达到最大重试次数。");
        }


        public bool IsConnected()
        {
            return _plc != null && _plc.IsConnected;
        }

        public async Task<Result> DisconnectAsync()
        {
            if (!IsConnected())
            {
                return Result.Failure("链路未连接。");
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
                    return OperationResult<object>.Failure("链路未连接，无法读取数据。");
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
                return OperationResult<IEnumerable<IPlcSignal>>.Failure("链路未连接，无法读取数据。");
            }

            try
            {
               var signalList= await   Read(signals);
                return OperationResult<IEnumerable<IPlcSignal>>.Success(signalList);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<IPlcSignal>>.Failure($"读取数据失败: {ex.Message}");
            }
        }
        private async Task<List<S7Signal>> Read(IEnumerable<IPlcSignal> signals)
        {
            var signalList = signals.OfType<S7Signal>().ToList();

            // 将信号转换为 DataItem
            List<DataItem> dataItems = new List<DataItem>();

            try
            {
                foreach (var s in signalList)
                {
                    if (!string.IsNullOrEmpty(s.Address))
                    {
                        //var ss = s.PLCLink.plclinkInfo.ProtocolType_.ToString().ToUpper();
                        //string[] prefixes = { ss }; // 协议前缀
                        //string pattern = $"^({string.Join("|", prefixes)}\\.)";
                        //var addressUpper = s.Address.ToUpper();
                        //string realAddress = Regex.Replace(addressUpper, pattern, string.Empty);

                        //var dataItem = DataItem.FromAddress(realAddress);
                        var dataItem = new DataItem
                        {
                            DataType = s.S7DataType,
                            VarType = s.S7VarType,
                            DB = s.DB,
                            BitAdr = s.BitAdr,
                            Count = s.Count,
                            StartByteAdr = s.StartByteAdr,
                            Value = new object() 
                        };
                        if (dataItem != null)
                        {
                            if (s.Count > 1)
                            {
                                dataItem.Count = s.Count;
                            }
                            dataItems.Add(dataItem);
                        }

                    }
                }
            }
            catch (Exception ex )
            {

                throw ex;
            }
           

            var result = new List<S7Signal>();
            const int maxRequestSize = 240; // 请求的最大字节长度限制
            const int baseSize = 19; // 固定字节数
            const int itemSize = 12; // 每个 DataItem 的字节数

            // 按批次处理数据
            for (int i = 0; i < dataItems.Count;)
            {
                int batchSize = Math.Min((maxRequestSize - baseSize) / itemSize, dataItems.Count - i);
                var batch = dataItems.Skip(i).Take(batchSize).ToList();
                i += batchSize;

                // 读取当前批次数据
                await _plc.ReadMultipleVarsAsync(batch);

                // 更新 S7Signal 的值
                for (int j = 0; j < batch.Count; j++)
                {
                    if (signalList[i - batchSize + j].S7VarType == VarType.S7WString)
                    {
                        signalList[i - batchSize + j].SetValue(batch[j].Value);
                        //if (batch[j].Value is ushort[] ushortArray)
                        //{
                        //   byte[] cc=  ushortArray.SelectMany(BitConverter.GetBytes).ToArray();
                        //   string ddd = S7WString.FromByteArray(cc);
                        //    signalList[i - batchSize + j].SetValue(ddd);
                        //}
                        
                    }
                    else
                    {
                        signalList[i - batchSize + j].SetValue(batch[j].Value);
                    }
                  
                   
                }
            }

            return signalList;
        }

        public async Task<Result> WriteDataAsync(IPlcSignal signal, object value)
        {
            if (!IsConnected())
            {
                return Result.Failure("链路未连接，无法写入数据。");
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
                return Result.Failure("链路未连接，无法写入数据。");
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

