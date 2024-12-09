using Guga.Collector.Interfaces;
using Guga.Collector.Models;
using Guga.Core.Devices;
using Guga.Core.Interfaces;
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
    public class PlcConnectionManager
    {
        /// <summary>
        /// PLC连接池
        /// </summary>
        private readonly Dictionary<string, PlcConnection> _connectionPool = new();
        
        /// <summary>
        /// 初始化连接管理器
        /// </summary>
        /// <param name="devices"></param>
        public PlcConnectionManager(List<Device> devices)
        {
            foreach (var device in devices)
            {
                var connectionKey = $"{device.Ip}:{device.Port}";
                if (!_connectionPool.ContainsKey(connectionKey))
                {
                    _connectionPool[connectionKey] = CreateConnection(device.Ip, device.Port);
                }
            }
        }
        /// <summary>
        /// 根据IP和端口获取连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public PlcConnection GetConnection(Device device)
        {
            var connectionKey = $"{device.Ip}:{device.Port}";
           
          
            return _connectionPool.ContainsKey(connectionKey) ? _connectionPool[connectionKey] : null!;
        }

        private PlcConnection CreateConnection(string ip, int? port)
        {
            return new PlcConnection(ip, port.GetValueOrDefault());
        }
    }
}
