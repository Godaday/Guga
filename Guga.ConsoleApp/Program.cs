// See https://aka.ms/new-console-template for more information

using Guga.Core.Devices;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;

Console.WriteLine($"测试验证开始：{DateTime.Now}");
//设备集合
HashSet<Device> devices = new HashSet<Device>();

//创建设备
WarehouseRobot  device= new WarehouseRobot("1", "仓库机器人", "WR001", "WarehouseRobot");
//创建信号
IPlcSignal state = new PlcSignal<int>("状态","states",1);
IPlcSignal local = new PlcSignal<double>("位置","local", 52.23);
IPlcSignal IP  = new PlcSignal<string>("Ip","IP", "172.0.0.1");
//设备订阅信号
device.SubscribeToSignals(new List<IPlcSignal> { state, local, IP });

devices.Add(device);
Console.WriteLine(devices.FirstOrDefault()?.ToString());
Type type = devices.FirstOrDefault()?.GetType();


Console.WriteLine($"Device Type:{type}");


foreach (var item in devices.FirstOrDefault()?.GetSubscribedSignals())
{
   
    Console.WriteLine($"{item.ToString()} 数据类型：{item.GetType().GetGenericArguments().FirstOrDefault()}");
    
};
Console.ReadLine();
