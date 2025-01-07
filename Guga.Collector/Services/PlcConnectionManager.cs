using Guga.Collector.Interfaces;
using Guga.Collector.Models;
using Guga.Core.Devices;
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
    public class PlcConnectionManager
    {
        /// <summary>
        /// PLC连接池
        /// </summary>
        private readonly Dictionary<string, IDeviceClient> _connectionPool = new();
        
        /// <summary>
        /// 初始化连接管理器
        /// </summary>
        /// <param name="devices"></param>
        public PlcConnectionManager(List<Device> devices)
        {
            // 按照协议类型分组
            var protocolTypeGroup =devices.GroupBy(x => x.ProtocolType_);
            foreach(var group in protocolTypeGroup)
            {
                var protocolType = group.Key;
                var protocolDevices = group.ToList();
                switch (protocolType)
                {
                    case ProtocolType.S7:
                        foreach (var device in protocolDevices)
                        {
                            var connectionKey = $"{device.Ip}:{device.Port}:{device.ProtocolType_}";
                            if (!_connectionPool.ContainsKey(connectionKey))
                            {
                                _connectionPool[connectionKey] = CreateClient(device);
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
        public IDeviceClient GetConnection(Device device)
        {
            var connectionKey = $"{device.Ip}:{device.Port}:{device.ProtocolType_}";


            return _connectionPool.ContainsKey(connectionKey) ? _connectionPool[connectionKey] : null!;
        }

        /// <summary>
        /// 创建PLC连接客户端
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IDeviceClient CreateClient(Device device)=>
            device.ProtocolType_ switch
            {
                ProtocolType.S7 => new S7Client(device.Ip,(CpuType)device.S7CPUType_!,device.rack,device.slot),
                //ProtocolType.modbus => new ModbusClient(device),
                _ => throw new ArgumentOutOfRangeException()
            };



    }
}
