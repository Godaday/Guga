using Guga.Collector;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class ContinuousWriter
{
    private readonly S7Client _s7Client;
    private CancellationTokenSource _cancellationTokenSource;
    private  readonly Random RandomGenerator = new Random();
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
                    Dictionary<IPlcSignal, object> signals = plcSignals.ToDictionary(
                        signal => signal,
                        signal => GetDefaultValueForSignal(signal)
                    );
                    // Perform the write operation
                    var result = await _s7Client.WriteDataAsync(signals);
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine($"Write failed: {result.Error}");
                    }
                    else
                    {
                        Console.WriteLine("Write successful.");
                    }

                    // Delay before the next write (adjust interval as needed)
                    await Task.Delay(8000, cancellationToken);
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


    object GetDefaultValueForSignal(IPlcSignal signal)
    {
        

        if (signal is S7Signal s7Signal)
        {
            return s7Signal.S7VarType switch
            {
                VarType.Byte => (byte)RandomGenerator.Next(byte.MinValue, byte.MaxValue + 1), // 0~255
                VarType.Word => (ushort)RandomGenerator.Next(ushort.MinValue, ushort.MaxValue + 1), // 0~65535
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
     string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[RandomGenerator.Next(s.Length)]).ToArray());
    }
}
