using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IExpressionEvaluator 
    {
        IDebugProperty EvaluateExpression(string expression);        
    }
}
