using Guga.Core.PLCLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    public interface IPLCLinkFactory
    {
        T CreatePLCLink<T>(Action<T>? configure = null) where T : PLCLink, new();
    }
}
