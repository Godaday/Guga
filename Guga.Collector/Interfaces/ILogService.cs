using Guga.Models.Enums;

namespace Guga.Collector.Interfaces
{
    public interface ILogService
    {
        IObservable<LogEntry> Logs { get; }
        void Log(string message, LogCategory category, LogLevel level = LogLevel.Info);
        void Complete();
    }

}
