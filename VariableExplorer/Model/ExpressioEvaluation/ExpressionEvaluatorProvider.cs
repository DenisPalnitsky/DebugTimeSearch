namespace SearchLocals.Model.ExpressioEvaluation
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
