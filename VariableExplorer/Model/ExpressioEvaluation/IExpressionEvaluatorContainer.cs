namespace SearchLocals.Model.ExpressioEvaluation
{
    interface IExpressionEvaluatorContainer
    {
        void Register(IExpressionEvaluator evaluator);
        void UnRegister();
    }
}
