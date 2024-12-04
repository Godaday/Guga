using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Transformer.Interfaces
{
    public interface IRule
    {
        // 用于执行规则的条件判断
        bool EvaluateCondition();

        // 用于执行规则的动作
        void ExecuteAction();
    }
}
