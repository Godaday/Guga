using Guga.Core.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    public interface IDeviceFactory
    {
        T CreateDevice<T>(Action<T>? configure = null) where T : Device, new();
    }
}
