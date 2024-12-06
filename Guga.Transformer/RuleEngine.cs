using Guga.Transformer.Interfaces;

namespace Guga.Transformer
{
    /// <summary>
    /// 规则引擎
    /// </summary>
    public class RuleEngine : IRuleEngine
    {
        private readonly List<IRule> _rules = new List<IRule>();  // 存储所有规则

       /// <summary>
       /// 添加单个规则
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <typeparam name="U"></typeparam>
       /// <param name="rule"></param>
        public void AddRule<T, U>(Rule<T, U> rule)
        {
            _rules.Add(rule);  
        }

        /// <summary>
        /// 添加多规则
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="rules"></param>
        public void AddRules<T, U>(List<Rule<T, U>> rules)
        {
            _rules.AddRange(rules); 
        }

       /// <summary>
       /// 执行规则
       /// </summary>
        public void ExecuteRules()
        {
            if (_rules == null)
            {
                return;
            }
           
            foreach (var rule in _rules)
            {
             if (rule.EvaluateCondition())
             {
              rule.ExecuteAction();
             }
                
            }
        }
    }

}
