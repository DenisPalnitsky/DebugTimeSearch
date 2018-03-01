using SearchLocals.Model.Services;
using System;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class ExpressionEvaluatorProvider : IExpressionEvaluatorProvider, IExpressionEvaluatorContainer
    {
        private readonly IVSEnvironmentEventsPublisher _expressionEvaluatorProvider;
        private IExpressionEvaluator _expressionEvaluator;

        public ExpressionEvaluatorProvider (IVSEnvironmentEventsPublisher expressionEvaluatorProvider)
        {
            _expressionEvaluatorProvider = expressionEvaluatorProvider;
        }

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
            _expressionEvaluatorProvider.ExpressionEvaluatorBecomeAvaialable();
        }

        public void UnRegister()
        {                        
            _expressionEvaluator = null;
            _expressionEvaluatorProvider.ExpressionEvaluatorBecomeUnAvaialable();
        }
    }
}
