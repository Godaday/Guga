using Guga.Models.Enums;

namespace Guga.Core.PLCLinks
{
    /// <summary>
    /// 通用链路类
    /// </summary>
    public class UniversalPLCLink : PLCLink
    {
        public UniversalPLCLink()
        {
            base.plclinkInfo.PLCLinkType_ = PLCLinkType.Universal; //通用链路
        }
       
        /// <summary>
        /// 信号改变事件
        /// </summary>
        public override void SignalChangeEvent()
        {
            //throw new NotImplementedException();
        }
       
    }
}
