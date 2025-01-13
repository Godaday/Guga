using Guga.Core.Devices;


namespace Guga.Collector.Interfaces
{
    public interface ISignalCollector
    {
        public ISignalCollector Init(List<Device> devices, int semaphoreSlim_MaxCount = 10);

        public void AddDevice(Device device);
        public void RemoveDevice(Device device);
        public void StartTimers();
        public void StopAllTimers();
    }
}
