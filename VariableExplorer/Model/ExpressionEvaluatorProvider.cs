using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class ExpressionEvaluatorProvider : IExpressionEvaluatorProvider, IExpressionEvaluatorContainer
    {
        private IExpressionEvaluator expressionEvaluator;

        public bool IsEvaluatorAvailable
        {
            get { return expressionEvaluator != null; }
        }

        public IExpressionEvaluator ExpressionEvaluator
        {
            get { return expressionEvaluator; }
        }

        public void Register(IExpressionEvaluator evaluator)
        {
            expressionEvaluator = evaluator;
        }

        public void UnRegister()
        {                        
            expressionEvaluator = null;
        }
    }
}
