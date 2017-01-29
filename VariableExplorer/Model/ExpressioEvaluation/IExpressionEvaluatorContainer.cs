namespace MyCompany.VariableExplorer.Model.ExpressioEvaluation
{
    interface IExpressionEvaluatorContainer
    {
        void Register(IExpressionEvaluator evaluator);
        void UnRegister();
    }
}
