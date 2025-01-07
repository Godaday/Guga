using Guga.Core.Devices;


namespace Guga.Collector.Interfaces
{
    public interface ISignalCollector
    {
        public void AddDevice(Device device);
        public void RemoveDevice(Device device);
        public void StartTimers();
        public void StopAllTimers();
    }
}
