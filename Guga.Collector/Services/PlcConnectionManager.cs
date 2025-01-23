using Guga.Collector.Interfaces;
using Guga.Collector.Models;
using Guga.Core.PLCLinks;
using Guga.Core.Enums;
using Guga.Core.Interfaces;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Services
{
    /// <summary>
    /// PLC连接管理
    /// </summary>
    public class PlcConnectionManager : IPlcConnectionManager
    {
        /// <summary>
        /// PLC连接池
        /// </summary>
        private readonly Dictionary<string, IPLCLinkClient> _connectionPool = new();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private int _ReconnectInterval;
        private int _ReconnectCount;
        public PlcConnectionManager(int reconnectInterval,int reconnectCount)
        {
            _ReconnectInterval = reconnectInterval;
            _ReconnectCount = reconnectCount;
        }

        /// <summary>
        /// 初始化连接管理器
        /// </summary>
        /// <param name="plclinks"></param>
        public void Init(List<PLCLink> plclinks,CancellationToken cancellationToken)
        {

            _connectionPool.Clear();
               // 按照协议类型分组
               var protocolTypeGroup = plclinks.GroupBy(x => x.plclinkInfo.ProtocolType_);
            foreach (var group in protocolTypeGroup)
            {
                var protocolType = group.Key;
                var protocolPLCLinks = group.ToList();
                switch (protocolType)
                {
                    case ProtocolType.S7:
                        foreach (var plclink in protocolPLCLinks)
                        {
                            var connectionKey = plclink.plclinkInfo.GetKey();
                            if (!_connectionPool.ContainsKey(connectionKey))
                            {
                                _connectionPool[connectionKey] = CreateClient(plclink);
                            }
                        }
                        break;
                    case ProtocolType.Modbus:
                        //创建modbus连接
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }
        /// <summary>
        /// 根据IP和端口获取连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public IPLCLinkClient GetConnection(PLCLink link)
        {
            var connectionKey = link.plclinkInfo.GetKey();


            return _connectionPool.ContainsKey(connectionKey) ? _connectionPool[connectionKey] : null!;
        }

        /// <summary>
        /// 创建PLC连接客户端
        /// </summary>
        /// <param name="plclink"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IPLCLinkClient CreateClient(PLCLink link) =>
            link.plclinkInfo.ProtocolType_ switch
            {
                ProtocolType.S7 => new S7Client(link.plclinkInfo.Ip, (CpuType)link.plclinkInfo.S7CPUType_!, link.plclinkInfo.rack, link.plclinkInfo.slot),
                //ProtocolType.modbus => new ModbusClient(plclink),
                _ => throw new ArgumentOutOfRangeException()
            };
        /// <summary>
        /// 释放资源 断开连接池中的所有连接
        /// </summary>
        public void Dispose()
        {
            foreach (var connection in _connectionPool)
            {
                connection.Value.DisconnectAsync();
            }
        }

      public async  Task ConnectionAllAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync();

            try
            {
                foreach (var connection in _connectionPool)
                {
                    var result = await connection.Value.ConnectAsync(_ReconnectCount, _ReconnectInterval, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        throw new TimeoutException($"启动失败，建立链路达到重试上限,{connection.Key}");
                    }
                }

            }
            finally {
                _semaphore.Release();
            }
           
            
           
        }
    }
}
