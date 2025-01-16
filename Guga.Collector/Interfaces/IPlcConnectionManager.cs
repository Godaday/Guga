using Guga.Core.PLCLinks;

namespace Guga.Collector.Interfaces
{
    public interface IPlcConnectionManager
    {
        public void Init(List<PLCLink> plclinks);
        IPLCLinkClient CreateClient(PLCLink link);
        void Dispose();
        IPLCLinkClient GetConnection(PLCLink link);

         Task  ConnectionAllAsync();
    }
}