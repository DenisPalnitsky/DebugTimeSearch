using SearchLocals.Model.Services;
using System;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class ExpressionEvaluatorProvider : IExpressionEvaluatorProvider, IExpressionEvaluatorContainer
    {
        private IExpressionEvaluator _expressionEvaluator;

        public bool IsEvaluatorAvailable
        {
            get { return _expressionEvaluator != null; }
        }

        public IExpressionEvaluator ExpressionEvaluator
        {
            get { return _expressionEvaluator; }
        }

        public void Register(IExpressionEvaluator evaluator)
        {
            _expressionEvaluator = evaluator;
            ServiceLocator.Resolve<IVSEnvironmentEventsPublisher>().ExpressionEvaluatorBecomeAvaialable();
        }

        public void UnRegister()
        {                        
            _expressionEvaluator = null;
            ServiceLocator.Resolve<IVSEnvironmentEventsPublisher>().ExpressionEvaluatorBecomeUnAvaialable();
        }
    }
}
