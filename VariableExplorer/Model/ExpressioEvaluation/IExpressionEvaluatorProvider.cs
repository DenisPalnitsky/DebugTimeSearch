namespace SearchLocals.Model.ExpressioEvaluation
{
    /// <summary>
    /// Main mechanism to retreive IExpressionEvaluator
    /// </summary>
    interface IExpressionEvaluatorProvider
    {
        bool IsEvaluatorAvailable { get;  }
        IExpressionEvaluator ExpressionEvaluator { get; }
    }
}
