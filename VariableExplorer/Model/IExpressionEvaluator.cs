using System;
namespace MyCompany.VariableExplorer.Model
{
    /// <summary>
    /// Evaluates expressions
    /// Dotn't use or pass it directly, use IExpressionEvaluatorProvider instead. It may not be available at certain points.
    /// </summary>
    interface IExpressionEvaluator 
    {
        IDebugProperty EvaluateExpression(string expression);        
    }
}
