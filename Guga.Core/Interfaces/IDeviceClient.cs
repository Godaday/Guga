using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    public interface IDeviceClient
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task<T> ReadDataAsync<T>(string signal);
        Task WriteDataAsync<T>(string signal, T value);
    }
}
