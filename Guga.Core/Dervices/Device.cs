using Guga.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Dervices
{
    public abstract class Device : IDevice
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceCode { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;

        private HashSet<IPlcSignal> _subscribedSignals;

        private readonly object _lockObject = new object();

        protected Device(string deviceId, string deviceName, string deviceCode, string deviceType)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceCode = deviceCode;
            DeviceType = deviceType;
            _subscribedSignals = new HashSet<IPlcSignal>(); 
        }

        public virtual void SubscribeToSignals(IEnumerable<IPlcSignal> signals)
        {
            lock (_lockObject)
            {
                foreach (var signal in signals)
                {
                    _subscribedSignals.Add(signal); 
                }
            }
        }

        public virtual void UnsubscribeFromSignals(IEnumerable<IPlcSignal> signals)
        {
            lock (_lockObject)
            {
                foreach (var signal in signals)
                {
                    _subscribedSignals.Remove(signal); // 更高效的删除操作
                }
            }
        }

        public virtual void UpdateSignals(IEnumerable<IPlcSignal> updatedSignals)
        {
            lock (_lockObject)
            {
                foreach (var updatedSignal in updatedSignals)
                {
                    var signal = _subscribedSignals.FirstOrDefault(s => s.SignalName == updatedSignal.SignalName);
                    if (signal != null)
                    {
                        // 实际的更新逻辑，可以直接更新信号的值
                        signal.SetValue(updatedSignal.GetValue());
                    }
                }
            }
        }

        public virtual IEnumerable<IPlcSignal> GetSubscribedSignals()
        {
            return _subscribedSignals;
        }
    }

}
