using Guga.Core.Interfaces;
using Guga.Core.Models;
using Newtonsoft.Json;
using S7.Net;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Guga.Core.PlcSignals
{
    /// <summary>
    /// 基础信号
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <summary>
    /// 表示一个使用S7协议的PLC信号。
    /// </summary>
    public class S7Signal : IPlcSignal
    {
        public string SignalName { get; set; }
        public string Address { get; set; }
        public object? Value { get; set; }
        public S7.Net.DataType S7DataType { get; set; }
        public VarType S7VarType { get; set; }
        public int DB { get; set; }
        public int StartByteAdr { get; set; }
        public byte BitAdr { get; set; }
        public int Count { get; set; }
        public IPLCLink PLCLink { get ; set; }

        public int ReadCycle { get; set; } = 200;
        public DateTime CollectTime { get; set; }

        public S7Signal(string signalName, string address,object value=null)
        {
            SignalName = signalName;
            Address = address;
            Value= value;
        }

        public void Configure<TConfig>(TConfig config)
        {
            if (config is S7Config s7Config)
            {
                S7DataType = s7Config.DataType;
                S7VarType = s7Config.VarType;
                DB = s7Config.DB;
                StartByteAdr = s7Config.StartByteAdr;
                BitAdr = s7Config.BitAdr;
                Count = s7Config.Count;
            }
            else
            {
                throw new ArgumentException("Invalid config type for S7Signal.");
            }
        }

        public object GetValue()
        {
           return Value;
        }

        public void SetValue(object value , bool updateCollectTime = true)
        {
            Value = value;
            if (updateCollectTime)
            {
                CollectTime = DateTime.Now;
            }
            
        }
        /// <summary>
        /// 返回信号存储JSON字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetSignalStoreValue(DateTime? dateTime ) {
            if (dateTime == null)
            {
                dateTime=DateTime.Now;
            }
            var t = new SignalValueModel() {
                LinkCode =this.PLCLink.plclinkInfo.PLCLinkCode,
                Address =this.Address,
                Value = this.Value,
                CollectorTime = dateTime
                
            };
          return  JsonConvert.SerializeObject(t);
        }
    }

}
