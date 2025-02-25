namespace Guga.Models.Collector
{

    /// <summary>
    /// 信号写入队列
    /// </summary>
    public class SignalWriteModel
    {
   
       /// <summary>
       /// 链路名称
       /// </summary>
      public string LinkCode { get; set; }
       /// <summary>
       /// 信号地址
       /// </summary>
       public string  Address { get; set; }
       /// <summary>
       /// 写入信号的地址
       /// </summary>
        public object Value { get; set; }
        public DateTime? CreateTime { get; set; }

        public SignalWriteModel(string linkCode,string signalAddress, object value)
        {
            Address = signalAddress;
            Value = value;
            LinkCode = linkCode;
            CreateTime=DateTime.Now;
        }
    }
}
