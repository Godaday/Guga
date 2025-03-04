﻿using Guga.Collector.Interfaces;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using Guga.Models.Collector;
using Guga.Models.Enums;
using S7.Net;

namespace Guga.Collector.Services
{
    public interface ISimulatedSignalWriter
    {
        public bool SimulatedSignalWriteState { get; set; }//模拟信号状态
        public int SimulatedSignalWriteInterval { get; set; }//模拟产生信号间隔
        void Start();
        void Stop();
    }

    public class SimulatedSignalWriter : ISimulatedSignalWriter
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly ICollectorRedisService _collectorRedisService;
        private readonly IPLCLinkManager _plcLinkManager;
        public bool SimulatedSignalWriteState { get; set; }//模拟信号状态
        public int SimulatedSignalWriteInterval { get; set; } = 1000;//模拟产生信号间隔
        private static readonly Random RandomGenerator = new Random();
        private ILogService _logService;
        public SimulatedSignalWriter(
            ICollectorRedisService collectorRedisService,
            IPLCLinkManager plcLinkManager,
            ILogService logService)
        {
            _collectorRedisService = collectorRedisService;
            _plcLinkManager = plcLinkManager;
            _logService = logService;
        }

        public void Start()
        {
            if (SimulatedSignalWriteState)
            {
                Console.WriteLine("Simulated signal writer is already running.");
                return;
            }

            SimulatedSignalWriteState = true;
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () => await SimulateWriteAsync(_cancellationTokenSource.Token));

            _logService.Log($"信号模拟器写入启动完成", LogCategory.WriteSimulator, LogLevel.Info);
        }

        public void Stop()
        {
            if (!SimulatedSignalWriteState)
            {
                
                    _logService.Log($"信号模拟器写入当前没有运行", LogCategory.WriteSimulator, LogLevel.Warning);
                    return;
            }

            SimulatedSignalWriteState = false;
            _cancellationTokenSource?.Cancel();

            _logService.Log($"信号模拟写入停止", LogCategory.WriteSimulator, LogLevel.Warning);
        }

        private async Task SimulateWriteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Simulated signal writer started.");

            try
            {
                while (SimulatedSignalWriteState && !cancellationToken.IsCancellationRequested)
                {
                    foreach (var plcLink in _plcLinkManager.PLCLinks)
                    {
                        var signals = plcLink.GetSubscribedSignals();

                        foreach (var signal in signals)
                        {
                            object value = GetDefaultValueForSignal(signal);

                            await _collectorRedisService.EnqueueAsyncSignalWriteDataAsync(
                                new SignalWriteModel(
                                    plcLink.plclinkInfo.PLCLinkCode,
                                    signal.Address,
                                    value
                                ));
                            string key = $"{plcLink.plclinkInfo.PLCLinkCode}:{signal.Address}";
                            _logService.Log($"key:{key} value:{value}", LogCategory.WriteSimulator, LogLevel.Info);

                            if (!SimulatedSignalWriteState || cancellationToken.IsCancellationRequested)
                                break;
                        }

                        if (!SimulatedSignalWriteState || cancellationToken.IsCancellationRequested)
                            break;

                        await Task.Delay(SimulatedSignalWriteInterval, cancellationToken);
                    }
                }

              
                _logService.Log($"信号模拟写入停止", LogCategory.WriteSimulator, LogLevel.Warning);
            }
            catch (TaskCanceledException)
            {
                _logService.Log($"Simulated signal writer task canceled.", LogCategory.WriteSimulator, LogLevel.Warning);
               
            }
            catch (Exception ex)
            {
               
                _logService.Log($"An error occurred in simulated signal writer: {ex.Message}", LogCategory.WriteSimulator, LogLevel.Error);
            }
            finally
            {
                SimulatedSignalWriteState = false;
            }
        }

        public object GetDefaultValueForSignal(IPlcSignal signal)
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

            if (signal is ModbusSignal modbusSignal)
            {
                return modbusSignal.VarType switch
                {
                    ModbusDataType.BOOL => Convert.ToBoolean(RandomGenerator.Next(0, 2) == 1), // 随机布尔值
                    ModbusDataType.INT16 => Convert.ToInt16(RandomGenerator.Next(short.MinValue, short.MaxValue + 1)),
                    ModbusDataType.UINT16 => (ushort)RandomGenerator.Next(ushort.MinValue, ushort.MaxValue + 1),
                    ModbusDataType.INT32 => RandomGenerator.Next(int.MinValue, int.MaxValue),
                    ModbusDataType.UINT32 => (uint)RandomGenerator.NextInt64(0, uint.MaxValue),
                    ModbusDataType.FLOAT => (float)(RandomGenerator.NextDouble() * 1000), // 随机浮点数
                    ModbusDataType.DOUBLE => RandomGenerator.NextDouble() * 1000, // 随机双精度浮点数
                    ModbusDataType.STRING => GenerateRandomString(6), // 长度为10的随机字符串
                    _ => throw new NotSupportedException($"Unsupported VarType: {modbusSignal.VarType}")
                };
            }

            throw new InvalidOperationException("Signal is not an S7Signal.");
        }
        static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789一二三四五六七八九十";
            var randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomGenerator.Next(s.Length)]).ToArray());

            return randomString;
        }


    }

}
