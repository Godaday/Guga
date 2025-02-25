using Guga.Collector.Interfaces;
using Guga.Models.Enums;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Guga.Collector.Services
{
    public class LogService : ILogService
    {
        private readonly ReplaySubject<LogEntry> _logSubject = new(100);

        public IObservable<LogEntry> Logs => _logSubject.AsObservable();

        public void Log(string message, LogCategory category, LogLevel level = LogLevel.Info)
        {
            _logSubject.OnNext(new LogEntry
            {
                Message = message,
                Category = category,
                Level = level
            });
        }

            public void Complete()
        {
            _logSubject.OnCompleted();
        }
    }

}
