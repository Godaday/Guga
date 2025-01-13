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
UniversalDevice automaticDoor = deviceFactory.CreateDevice<UniversalDevice>(door => {
    door.deviceInfo.DeviceId = "1";
    door.deviceInfo.DeviceName = "西门子设备";
    door.deviceInfo.DeviceCode = "WR001";
    door.deviceInfo.DeviceType_ = DeviceType.Universal;
    door.deviceInfo.Ip = "127.0.0.1";
    door.deviceInfo.Port = 102;
    door.deviceInfo.ProtocolType_ = ProtocolType.S7;
    door.deviceInfo.S7CPUType_ = S7CPUType.S71200;
    door.deviceInfo.rack = 0;
    door.deviceInfo.slot = 1;
   
    
});
//创建信号
var s7Signals = ContinuousWriter.CreateS7TestSignals();

//设备订阅信号
automaticDoor.SubscribeToSignals(s7Signals);
devices.Add(automaticDoor);


var s7Client = new S7Client("127.0.0.1", CpuType.S71200, 0, 0);
s7Client.ConnectAsync().Wait();
Console.WriteLine($"连接状态：{s7Client.IsConnected()}");

 var continuousWriter = new ContinuousWriter(s7Client);
continuousWriter.StartWriting(s7Signals);

//初始化采集器
SignalCollector signalCollector = new SignalCollector();
signalCollector.Init(devices)
    .StartTimers();


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




    






