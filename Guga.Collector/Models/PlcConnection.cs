using Guga.Collector.Interfaces;
using Guga.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Models
{
    public class PlcConnection
    {
        public string IpAddress { get; }
        public int Port { get; }

        public PlcConnection(string ip, int port)
        {
            IpAddress = ip;
            Port = port;
        }
        public Task<object> ReadSignalAsync(IPlcSignal signal)
        {
            return Task.FromResult<object>(new Random().Next(0,2));
        }

        public Task WriteSignalAsync(IPlcSignal signal)
        {
            throw new NotImplementedException();
        }
    }
}
