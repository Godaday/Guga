// See https://aka.ms/new-console-template for more information

using Guga.Collector.Services;
using Guga.Core.Devices;
using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
AutomaticDoor automaticDoor = deviceFactory.CreateDevice<AutomaticDoor>(door=>  {
    door.DeviceId = "1";
    door.DeviceName = "自动门";
    door.DeviceCode = "WR001";
});
//创建信号
IPlcSignal state = new PlcSignal<int>("状态", "0032", 1);

//设备订阅信号
automaticDoor.SubscribeToSignals(new List<IPlcSignal> { state});
devices.Add(automaticDoor);

SignalCollector signalCollector = new SignalCollector(devices, TimeSpan.FromSeconds(2));
//模拟PLC信号读取数据及变化
while (true)
{
   
    Console.WriteLine($"信号值：{state.GetValue()}");
   
    Console.WriteLine(automaticDoor.ToString());
    Thread.Sleep(2000);
}

