// See https://aka.ms/new-console-template for more information

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
//设备集合
HashSet<Device> devices = new HashSet<Device>();
;
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
Console.WriteLine(devices.FirstOrDefault()?.ToString());
Type type = devices.FirstOrDefault()?.GetType();


Console.WriteLine($"Device Type:{type}");


foreach (var item in devices.FirstOrDefault()?.GetSubscribedSignals())
{
   
    Console.WriteLine($"{item.ToString()} 数据类型：{item.GetType().GetGenericArguments().FirstOrDefault()}");
    
};
while (true)
{
    state.SetValue(new Random().Next(0,2));
    Console.WriteLine($"信号值：{state.GetValue()}");
    automaticDoor.UpdateSignals(new List<IPlcSignal> { state });
    Console.WriteLine(automaticDoor.ToString());
    Thread.Sleep(2000);
}

