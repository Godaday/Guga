using Guga.Collector;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using Guga.Models.Collector;
using Guga.Models.Enums;
using S7.Net;
using S7.Net.Types;

public class ContinuousWriter
{
    private readonly S7Client _s7Client;
    private CancellationTokenSource _cancellationTokenSource;
    private static  readonly Random RandomGenerator = new Random();
    public ContinuousWriter(S7Client s7Client)
    {
        _s7Client = s7Client ?? throw new ArgumentNullException(nameof(s7Client));
    }

    public void StartWriting(IEnumerable<IPlcSignal> plcSignals)
    {
       
      
        if (_cancellationTokenSource != null)
        {
            throw new InvalidOperationException("Writing is already in progress.");
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        Task.Run(async () =>
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //信号默认值
                    Dictionary<IPlcSignal, object> signals = new Dictionary<IPlcSignal, object>();
                   
                    foreach (var s in plcSignals)
                    {
                        // Perform the write operation
                        signals.Add(s, GetDefaultValueForSignal(s));
                    }
                    var result = await _s7Client.WriteDataAsync(signals);
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine($"Write failed: {result.Error}");
                    }
                    else
                    {
                        Console.WriteLine("Write successful.");
                    }

                   
                    
                   
                   
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Writing operation was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }, cancellationToken);
    }

    public void StopWriting()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }
    }


  public static  object GetDefaultValueForSignal(IPlcSignal signal)
    {
        

        if (signal is S7Signal s7Signal)
        {
            return s7Signal.S7VarType switch
            {
                VarType.Byte => (byte)RandomGenerator.Next(byte.MinValue, byte.MaxValue + 1), // 0~255
                VarType.Word => (ushort)RandomGenerator.Next(int.MinValue, ushort.MaxValue + 1), // 0~65535
                VarType.DWord => (uint)RandomGenerator.NextInt64(0, uint.MaxValue), // 0~4294967295
                VarType.Int => (short)RandomGenerator.Next(short.MinValue, short.MaxValue + 1), // -32768~32767
                VarType.DInt => RandomGenerator.Next(int.MinValue, int.MaxValue), // -2147483648~2147483647
                VarType.Real => (float)(RandomGenerator.NextDouble() * 1000), // 0~1000 的浮点数
                VarType.Bit => RandomGenerator.Next(0, 2) == 1, // 随机布尔值
                VarType.S7WString => GenerateRandomString(10), // 长度为10的随机字符串
                _ => throw new NotSupportedException($"Unsupported VarType: {s7Signal.S7VarType}")
            };
        }
        throw new InvalidOperationException("Signal is not an S7Signal.");
    }
  static  string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789一二三四五六七八九十";
        var randomString = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[RandomGenerator.Next(s.Length)]).ToArray());

        return randomString;
    }

    public static IEnumerable<IPlcSignal> CreateS7TestSignals()
    {
        var signals = new List<IPlcSignal>();

        // 定义所有可能的 VarType 和 DataType 的组合
        var varTypes = new[]
        {
            VarType.Byte,
            VarType.Word,
            VarType.DWord,
            VarType.Int,
            VarType.DInt,
            VarType.Real,
             VarType.Bit,
            VarType.S7WString
        };

        var dataTypes = new[]
        {
            DataType.DataBlock,
            //DataType.Input,
            //DataType.Output,
            //DataType.Memory
        };

        // 遍历所有组合，生成测试信号
        int startByte = 0;
        foreach (var dataType in dataTypes)
        {
            foreach (var varType in varTypes)
            {
                // 配置参数
                var config = new S7Config
                {
                    DataType = dataType,
                    VarType = varType,
                    DB = 1, // 默认 DB1
                    StartByteAdr = startByte,
                    Count = varType == VarType.S7WString ? 10 : 1,
                    BitAdr = 0

                };
                var dataItem = new DataItem
                {
                    DataType = dataType,
                    VarType = varType,
                    DB = 1, // 默认 DB1
                    StartByteAdr = startByte,
                    Count = varType == VarType.S7WString ? 10 : 1,
                    BitAdr = 0

                };
                var addres =dataItem.ToAddressString();


                // 使用工厂创建信号
                var signal = PlcSignalFactory.CreateSignal(
                    ProtocolType.S7,
                    $"{dataType}_{varType}_Signal",
                    $"{ProtocolType.S7}.{addres}",
                    config
                );
                signal.ReadCycle = new Random().Next(200, 1000);
                signals.Add(signal);
                startByte += config.Count; // 更新地址
            }
        }

        return signals;
    }
}
