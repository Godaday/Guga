using Guga.Core.Interfaces;
using Guga.Transformer.Interfaces;

namespace Guga.Transformer
{
    /// <summary>
    /// 规则转换器
    /// </summary>
    /// <typeparam name="T">条件对象</typeparam>
    /// <typeparam name="U">转换对象</typeparam>
    public class Rule<T, U>: IRule
    {
        public T OriginObject { get; set; } 
        public U BusinessObject { get; set; } 
        public Func<T, bool> Condition { get; set; } 
        public Action<U> Action { get; set; }

        public bool EvaluateCondition()
        {
            return Condition(OriginObject); ;
        }

        public void ExecuteAction()
        {
            Action(BusinessObject); ;
        }
    }

}