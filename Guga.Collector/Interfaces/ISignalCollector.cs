using Guga.Core.delegates;
using Guga.Core.PLCLinks;


namespace Guga.Collector.Interfaces
{
    public interface ISignalCollector
    {
        public bool IsRunning { get; }
        public  Task ReLoadLinkAndSignal();
        public Task<ISignalCollector> Init( GetPlcLinksDelegate getPlcLinksDelegate, int semaphoreSlim_MaxCount = 10);

        public void AddPLCLink(PLCLink plclink);
        public void RemovePLCLink(PLCLink plclink);
        public Task Start();
        public Task Stop();
        public Task ReStart();
    }
}
