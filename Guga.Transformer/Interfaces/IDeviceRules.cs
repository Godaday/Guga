using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Transformer.Interfaces
{
    /// <summary>
    /// 设备规则接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public interface IDeviceRules
    {
        /// <summary>
        /// 信号转业务规则
        /// </summary>
        /// <param name="t"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public List<IRule>  GetSignalToBusinessRules();
        /// <summary>
        /// 业务转信号规则
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<IRule> GetBusinessToSignalRules();
    }
}
