using Guga.Core.PLCLinks;
using System.Threading;

namespace Guga.Collector.Interfaces
{
    public interface IPlcConnectionManager
    {
        public void Init(List<PLCLink> plclinks, CancellationToken cancellationToken);
        IPLCLinkClient CreateClient(PLCLink link);
        void Dispose();
        IPLCLinkClient GetConnection(PLCLink link);

         Task  ConnectionAllAsync(CancellationToken cancellationToken);
    }
}