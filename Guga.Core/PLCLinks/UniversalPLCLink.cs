using Guga.Core.Enums;
using Guga.Transformer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 对象状态转信号规则
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<IRule> GetBusinessToSignalRules()
        {
           return  new List<IRule>();
        }
        /// <summary>
        /// 信号转对象状态规则
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<IRule> GetSignalToBusinessRules()
        {
            return new List<IRule>();
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
