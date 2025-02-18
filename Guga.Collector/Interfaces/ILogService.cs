using Guga.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Interfaces
{
    public interface ILogService
    {
        IObservable<LogEntry> Logs { get; }
        void Log(string message, LogCategory category, LogLevel level = LogLevel.Info);
        void Complete();
    }

}
