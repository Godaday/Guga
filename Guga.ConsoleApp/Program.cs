// See https://aka.ms/new-console-template for more information

using Guga.Collector;
using Guga.Collector.Services;
using Guga.Core.Devices;
using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Core.PlcSignals;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using S7.Net;
using System.Reflection;

var serviceCollection = new ServiceCollection();
serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
serviceCollection.AddTransient<IDeviceFactory, DeviceFactory>();
var collectionProvider = serviceCollection.BuildServiceProvider();
var mediator = collectionProvider.GetRequiredService<IMediator>();
var deviceFactory = collectionProvider.GetRequiredService<IDeviceFactory>();
Console.WriteLine($"测试验证开始：{DateTime.Now}");
List<Device> devices = new List<Device>();
//创建设备
AutomaticDoor automaticDoor = deviceFactory.CreateDevice<AutomaticDoor>(door => {
    door.DeviceId = "1";
    door.DeviceName = "自动门";
    door.DeviceCode = "WR001";
    door.DeviceType_ = DeviceType.AutomaticDoor;
    door.Ip = "127.0.0.1";
    door.ProtocolType_ = ProtocolType.S7;
    door.S7CPUType_ = S7CPUType.S71200;
    door.rack = 0;
    door.slot = 1;
   
    
});
//创建信号
var s7Signals = CreateS7TestSignals();

//设备订阅信号
automaticDoor.SubscribeToSignals(s7Signals);
devices.Add(automaticDoor);


var s7Client = new S7Client("127.0.0.1", CpuType.S71200, 0, 0);
s7Client.ConnectAsync().Wait();
Console.WriteLine($"连接状态：{s7Client.IsConnected()}");

 var continuousWriter = new ContinuousWriter(s7Client);
continuousWriter.StartWriting(s7Signals);

//初始化采集器
SignalCollector signalCollector = new SignalCollector(devices);
signalCollector.StartTimers();

//模拟PLC信号读取数据及变化
while (true)
{
    Console.WriteLine($"Signal Value Loop  Print: {DateTime.Now}");
    PrintSignalValue(s7Signals);
    Thread.Sleep(7000);
}
 static  void PrintSignalValue(IEnumerable<IPlcSignal> signals)
{
    foreach (var signal in signals)
    {
        Console.WriteLine($"Signal: {signal.SignalName}, Value: {signal.GetValue()}");
    }
}


static IEnumerable<IPlcSignal> CreateS7TestSignals()
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

                // 使用工厂创建信号
                var signal = PlcSignalFactory.CreateSignal(
                    ProtocolType.S7,
                    $"{dataType}_{varType}_Signal",
                    $"DB{config.DB}.{config.StartByteAdr}",
                    config
                );

                signals.Add(signal);
                startByte += config.Count; // 更新地址
            }
        }

        return signals;
    }

    






