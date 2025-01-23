using Guga.Core.delegates;
using Guga.Core.PLCLinks;


namespace Guga.Collector.Interfaces
{
    public interface ISignalCollector
    {
        public bool IsRunning { get; }
        public bool IsInit { get; }
        public bool IsUserStop { get; set; }

        public Task ReLoadLinkAndSignal();
        public Task<ISignalCollector> Init(GetPlcLinksDelegate getPlcLinksDelegate,CancellationToken cancellationToken, int semaphoreSlim_MaxCount = 10);
        public Task Start(CancellationToken cancellationToken);
        public Task Stop(bool isUserStop = false);
        public Task ReStart(CancellationToken cancellationToken);
    }
}
