using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Models
{
    /// <summary>
    /// 信号写入值的Model
    /// {
    /// "SignalAddress": "LinkCode:DB1.DBW5",
    ///   "Value": 122
    ///   }
    /// </summary>
    public class SignalWriteModel
    {
   

       public string SignalAddress { get; set; }
        public object Value { get; set; }

        public SignalWriteModel(string signalAddress, object value)
        {
            SignalAddress = signalAddress;
            Value = value;
        }
    }
}
