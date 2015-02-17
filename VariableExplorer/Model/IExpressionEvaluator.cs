using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IExpressionEvaluator 
    {
        DebugProperty EvaluateExpression(string expression);
    }
}
