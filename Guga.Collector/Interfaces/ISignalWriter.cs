using Guga.Collector.Services;
using Guga.Core.Interfaces;
using Guga.Core.PLCLinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Interfaces
{
    public interface ISignalWriter
    {
        public bool IsRunning { get; }

        public int _MaxProcessCount { get; set; }
        public int _WriteInterval { get; set; }
        SignalWriter Init(int writeInterval, int maxProcessCount);
        Task ReStart();
        Task Start();
        Task Stop();
    }
}
