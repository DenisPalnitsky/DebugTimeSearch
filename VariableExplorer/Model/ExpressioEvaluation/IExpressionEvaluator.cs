using SearchLocals.Model.VSPropertyModel;

namespace SearchLocals.Model.ExpressioEvaluation
{
    /// <summary>
    /// Evaluates code expressions.
    /// Do not use or pass it directly, use IExpressionEvaluatorProvider instead. It may not be available at certain points.
    /// </summary>
    interface IExpressionEvaluator 
    {
        IVSDebugPropertyProxy EvaluateExpression(string expression);
        IVSDebugPropertyProxy GetLocals();        
    }
}
