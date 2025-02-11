﻿using Guga.Collector.Interfaces;
using Guga.Core.Interfaces;

namespace Guga.Collector.Services
{


    public class SignalWriter : ISignalWriter
    {
        private readonly IPlcConnectionManager _connectionManager;//连接管理
        private readonly IPLCLinkManager _plclinkManager;//链路管理
        private readonly ICollectorRedisService _collectorRedisService; //redis相关操作

        public int _MaxProcessCount { get;  set; }= 0; //单次写入读取量
        public  int _WriteInterval { get;  set; } = 500;//写入读取间隔
        private Timer? _timer;
        public bool IsRunning { get; private set; } = false;
        public bool IsInit { get; private set; } = false;
        public bool IsUserStop { get; set; } = false;
        public SignalWriter(IPlcConnectionManager plcConnectionManager, IPLCLinkManager plclinkManager,
            ICollectorRedisService collectorRedisService)
        {
            _connectionManager = plcConnectionManager;
            _plclinkManager = plclinkManager;
            _collectorRedisService = collectorRedisService;
        }

        public SignalWriter Init(int writeInterval, int maxProcessCount,CancellationToken cancellationToken)
        {
            _MaxProcessCount = maxProcessCount;
            _WriteInterval = writeInterval;
            _timer = new Timer(async _ => await WriteSiganlForTimerAsync(cancellationToken), null, Timeout.Infinite, Timeout.Infinite);
#if DEBUG
            Console.WriteLine("写入信号服务 初始化完成");
#endif
            IsInit = true;
            return this;
        }

        public async Task WriteSiganlForTimerAsync(CancellationToken cancellationToken) { 
        
               bool linkConnected = await WriteSiganl_Async();
            if (!linkConnected)
            {
              await   _connectionManager.ConnectionAllAsync(cancellationToken);
            }
        }
        /// <summary>
        /// 读取数据写入信号
        /// </summary>
        /// <returns></returns>
        private async Task<bool> WriteSiganl_Async()
        {
            bool linkConnected = true;
            //获取指定数量的待写入信号数据
            var pendingSignals = await _collectorRedisService.PeekSignalWriteDataAsync(stop: _MaxProcessCount-1);
            if (pendingSignals!=null&&pendingSignals.Any() && _plclinkManager.PLCLinks.Any())
            {
                var linkCodes = pendingSignals.Select(x => x.LinkCode).Distinct().ToList();
                var links = _plclinkManager.PLCLinks.Where(x => linkCodes.Contains(x.plclinkInfo.PLCLinkCode)).ToList();
                if (linkCodes.Count == links.Count)
                {
                    foreach (var s in pendingSignals)
                    {
                        //信号所属链路
                        var link = links.FirstOrDefault(x => x.plclinkInfo.PLCLinkCode == s.LinkCode);
                        var signal = link.GetSubscribedSignals().FirstOrDefault(x => x.Address == s.Address);
                        if (signal != null)
                        {
                            //获取链路的连接
                            var linkConn = _connectionManager.GetConnection(link);
                            if (!linkConn.IsConnected())
                            {
                                linkConnected = false;
                                return linkConnected;
                            }

                            try
                            {
                                var writeResult = await linkConn.WriteDataAsync(signal, s.Value);

                                if (writeResult.IsSuccess)
                                {
                                    await _collectorRedisService.DequeueSignalWriteDataAsync();//出队

                                }
                                else
                                {
                                    await _collectorRedisService.DequeueSignalWriteDataAsync();//出队

                                    await _collectorRedisService.EnqueueAsyncSignalWriteDataAsync(s);//不成功 重新入队
                                }

                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }

                    }
                   
                }
            }
           
            return linkConnected;
        }
      

        /// <summary>
        /// 启动写入
        /// </summary>
        public async Task Start( CancellationToken cancellation )
        {
            await Task.Run(() =>
            {
                _timer?.Change(0, _WriteInterval);
                IsRunning=true;
            });

        }
        /// <summary>
        /// 停止写入
        /// </summary>
        public async Task Stop(bool isUserStop = false)
        {
            await Task.Run(() =>
            {
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);//启动定时器
                IsRunning = false;
            });
            IsUserStop = isUserStop;
        }
        /// <summary>
        /// 重启
        /// </summary>
        public async Task ReStart(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                Init(_WriteInterval, _MaxProcessCount, cancellationToken);
                await Start(cancellationToken);
            });
        }
    }
}
     