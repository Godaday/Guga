using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Models
{
    public class ModbusConfig
    {
        public byte FunctionCode { get; set; }
        public ushort StartRegister { get; set; }
        public ushort RegisterCount { get; set; }
    }
}
