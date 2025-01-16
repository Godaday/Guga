using ColinChang.RedisHelper;
using Guga.Collector.Interfaces;
using Guga.Core.delegates;
using Guga.Core.Interfaces;
using Guga.Core.Models;
using Guga.Core.PLCLinks;
using Guga.Core.PlcSignals;
using Guga.Redis.ConfigModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Guga.Collector.Services
{
    public class SignalCollector : ISignalCollector
    {
        private readonly object _lock_ = new();
        private static Dictionary<string, Timer> _timers = new(); // 定时器，触发信号采集

        private IPlcConnectionManager _connectionManager;
        private static SemaphoreSlim Semaphore; // 限制最N个并发操作
        private  int _maxConcurrency;
        private bool Running = false;
        public bool IsRunning {
        get{ return Running; }
        }

        private readonly RedisKeyOptions _redisKeyOptions;
        private readonly IRedisHelper _redisHelper;
        private readonly IPLCLinkFactory _pLCLinkFactory;
        private readonly IPLCLinkManager _plclinkManager;

        private  GetPlcLinksDelegate _getPlcLinks;//获取链路委托
        public SignalCollector(IPlcConnectionManager connectionManager, IOptions<RedisKeyOptions> redisKeyOptions,
            IRedisHelper redisHelper, IPLCLinkFactory pLCLinkFactory, IPLCLinkManager plclinkManager)
        {
            _connectionManager = connectionManager;
            _redisKeyOptions = redisKeyOptions.Value;
            _redisHelper = redisHelper;
            _pLCLinkFactory = pLCLinkFactory;
            _plclinkManager = plclinkManager;
        }
        public async Task ReLoadLinkAndSignal()
        {
            _plclinkManager.PLCLinks.Clear();
        
            if (_getPlcLinks != null)
            {
                _plclinkManager.PLCLinks= await _getPlcLinks.Invoke() ?? throw new ArgumentNullException(nameof(_getPlcLinks));
            }
        }

        /// <summary>
        /// 初始化采集器
        /// </summary>
        /// <param name="plclinks"></param>
        /// <param name="semaphoreSlim_MaxCount"></param>
        /// <returns></returns>
        public async Task<ISignalCollector> Init(GetPlcLinksDelegate getPlcLinksDelegate, int semaphoreSlim_MaxCount=10)
        {
            _getPlcLinks = getPlcLinksDelegate;
            List<PLCLink> plclinks= await getPlcLinksDelegate.Invoke() ?? throw new ArgumentNullException(nameof(getPlcLinksDelegate));
            _plclinkManager.PLCLinks = plclinks;
           _maxConcurrency = semaphoreSlim_MaxCount;
               Semaphore = new SemaphoreSlim(semaphoreSlim_MaxCount);
           
            _connectionManager.Init(plclinks);
            //初始化定时器
            foreach (var item in plclinks)
            {
                AddTimer(item);
            }
            return this;


        }
        /// <summary>
        /// 启动所有定时器
        /// </summary>
        public async Task Start()
        {
           await  _connectionManager.ConnectionAllAsync();
            foreach (var item in _timers)
            {
               
                item.Value.Change(0, GetTimerKey(item.Key) );//启动定时器
            }
            Running =true;
#if DEBUG
            Console.WriteLine("定时器已启动");
#endif

        }
        /// <summary>
        /// 获取定时器Key中的周期
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetTimerKey(string key)
        {
            string lastPart = key.Split(':').Last(); 
            if (int.TryParse(lastPart, out int result)) // 尝试将最后一个部分转换为整数
            {
                return result;
            }
            return 200;
        }
        // 停止所有定时器
        public  async Task Stop()
        {
            foreach (var timer in _timers)
            {
                // 停止每个定时器
                timer.Value.Change(Timeout.Infinite, Timeout.Infinite);
               
            }
            Running = false;
#if DEBUG
            Console.WriteLine("定时器已关闭");
#endif
        }

        public async Task ReStart() {
           await  Stop();
            _timers.Clear();
          await  ReLoadLinkAndSignal();
           await  Init(this._getPlcLinks, _maxConcurrency);

           await Start();
          
        }
        private void AddTimer(PLCLink pLCLink)
        {
            var linkSignals=pLCLink.GetSubscribedSignals();
             var signalGropByCycle= linkSignals
        .GroupBy(signal => signal.ReadCycle) // 按周期分组
        .ToDictionary(group => group.Key, group => group.ToList());
            foreach (var c in signalGropByCycle)
            {
                var timerKey = $"{pLCLink.plclinkInfo.GetKey()}:{c.Key}";//链路Key+周期 作为定时器Key
                if (!_timers.ContainsKey(timerKey))
                {
                    _timers.Add(timerKey,
                        new Timer(async _ =>
                        await CollectSignalsWithConcurrencyLimit( pLCLink,c.Value),
                        null,
                        Timeout.Infinite, Timeout.Infinite
                        //TimeSpan.Zero,
                        //timeSpan
                        ));
                }
            }
        
            
        }

        public void AddPLCLink(PLCLink  link)
        {
            //lock (_lock_)
            //{
            //    //没有所属分组则创建个新分组
            //    if (!groups_.ContainsKey(link.plclinkInfo.ReadCycle))
            //    {
            //        groups_[link.plclinkInfo.ReadCycle] = new List<PLCLink>() { link };
            //    }
            //    else
            //    {
            //        groups_[link.plclinkInfo.ReadCycle].Add(link);
            //    }

            //    Console.WriteLine($"采集器添加链路:{link.plclinkInfo.PLCLinkName}");

            //}
        }
        public void RemovePLCLink(PLCLink  link)
        {
            //lock (_lock_)
            //{
               
            //    if (groups_.ContainsKey(link.plclinkInfo.ReadCycle))
            //    {
            //        groups_[link.plclinkInfo.ReadCycle].Remove(link); ;
            //    }

            //    Console.WriteLine($"采集器移除链路:{link.plclinkInfo.PLCLinkName}");

            //}


        }
        /// <summary>
        /// 异步并发采集多个链路的信号
        /// </summary>
        /// <param name="plclinks">链路列表</param>
        /// <param name="signals">信号列表</param>
        /// <returns></returns>
        public async Task CollectSignalsWithConcurrencyLimit(PLCLink pLCLink,List<IPlcSignal> _plcSignals)
        {
            // 获取链路的连接
            var connection = _connectionManager.GetConnection(pLCLink);
            if (connection.IsConnected())
            {
                // 使用PLC连接采集信号

                var signals = _plcSignals;
                var signalResult = await connection.ReadDataAsync(signals);
                if (signalResult.IsSuccess)
                {
                    //plclink.UpdateSignals(signalResult.Data);
                }
            }
            



           
        }

        /// <summary>
        /// 异步采集单个信号
        /// </summary>
        /// <param name="plclink">链路对象</param>
        /// <param name="signal">信号对象</param>
        /// <returns></returns>
        public async Task CollectSignalAsync(PLCLink plclink, IPlcSignal signal)
        {
            var connection = _connectionManager.GetConnection(plclink);
            var signalResult = await connection.ReadDataAsync(signal);
            if (signalResult.IsSuccess)
            {
                signal.SetValue(signalResult.Data);  // 更新信号值
            }
            
           // plclink.UpdateSignals(new List{ signal });  // 更新链路的信号
        }
     
    }
}
