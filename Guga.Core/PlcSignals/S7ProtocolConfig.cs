using Guga.Core.Interfaces;
using Guga.Core.Models;

namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// S7协议的配置信息管理。
    /// </summary>
    public class S7ProtocolConfig : IProtocolConfig
    {
        private readonly S7Config _config;

        public S7ProtocolConfig(S7Config config)
        {
            _config = config;
        }

        public void ApplyConfiguration(IPlcSignal signal)
        {
            if (signal is S7Signal s7Signal)
            {
                s7Signal.Configure(_config);
            }
            else
            {
                throw new ArgumentException("Invalid signal type for S7 protocol.");
            }
        }
    }
}
