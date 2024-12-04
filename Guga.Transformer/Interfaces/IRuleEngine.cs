using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Transformer.Interfaces
{
    public interface IRuleEngine
    {
       /// <summary>
       /// 添加单个规则
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <typeparam name="U"></typeparam>
       /// <param name="rule"></param>
        void AddRule<T, U>(Rule<T, U> rule);

       /// <summary>
       /// 添加多规则
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <typeparam name="U"></typeparam>
       /// <param name="rules"></param>
        void AddRules<T, U>(List<Rule<T, U>> rules);

       /// <summary>
       /// 执行规则
       /// </summary>
        void ExecuteRules();
    }
}
