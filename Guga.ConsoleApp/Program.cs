// See https://aka.ms/new-console-template for more information

//var serviceCollection = new ServiceCollection();
//serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
//serviceCollection.AddTransient<IPLCLinkFactory, PLCLinkFactory>();

Console.WriteLine($"-------------");
//var collectionProvider = serviceCollection.BuildServiceProvider();
//var mediator = collectionProvider.GetRequiredService<IMediator>();
//var plclinkFactory = collectionProvider.GetRequiredService<IPLCLinkFactory>();
//Console.WriteLine($"测试验证开始：{DateTime.Now}");
//List<PLCLink> plclinks = new List<PLCLink>();
////创建链路
//UniversalPLCLink automaticDoor = plclinkFactory.CreatePLCLink<UniversalPLCLink>(door => {
//    door.plclinkInfo.PLCLinkId = "1";
//    door.plclinkInfo.PLCLinkName = "西门子链路";
//    door.plclinkInfo.PLCLinkCode = "WR001";
//    door.plclinkInfo.PLCLinkType_ = PLCLinkType.Universal;
//    door.plclinkInfo.Ip = "127.0.0.1";
//    door.plclinkInfo.Port = 102;
//    door.plclinkInfo.ProtocolType_ = ProtocolType.S7;
//    door.plclinkInfo.S7CPUType_ = S7CPUType.S71200;
//    door.plclinkInfo.rack = 0;
//    door.plclinkInfo.slot = 1;


//});
////创建信号
//var s7Signals = ContinuousWriter.CreateS7TestSignals();

////链路订阅信号
//automaticDoor.SubscribeToSignals(s7Signals);
//plclinks.Add(automaticDoor);


//var s7Client = new S7Client("127.0.0.1", CpuType.S71200, 0, 0);
//s7Client.ConnectAsync().Wait();
//Console.WriteLine($"连接状态：{s7Client.IsConnected()}");

// var continuousWriter = new ContinuousWriter(s7Client);
//continuousWriter.StartWriting(s7Signals);

//IPlcConnectionManager  plcConnectionManager = new PlcConnectionManager(3000,5);
////初始化采集器
//SignalCollector signalCollector = new SignalCollector(plcConnectionManager,);
//signalCollector.Init(plclinks)
//    .Start();


////模拟PLC信号读取数据及变化
//while (true)
//{
//    Console.WriteLine($"Signal Value Loop  Print: {DateTime.Now}");
//    PrintSignalValue(s7Signals);
//    Thread.Sleep(7000);
//}
// static  void PrintSignalValue(IEnumerable<IPlcSignal> signals)
//{
//    foreach (var signal in signals)
//    {
//        Console.WriteLine($"Signal: {signal.SignalName}, Value: {signal.GetValue()}");
//    }
//}











