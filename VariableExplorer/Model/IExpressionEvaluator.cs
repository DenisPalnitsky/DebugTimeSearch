﻿using System;
namespace MyCompany.VariableExplorer.Model
{
    /// <summary>
    /// Evaluates code expressions.
    /// Do not use or pass it directly, use IExpressionEvaluatorProvider instead. It may not be available at certain points.
    /// </summary>
    interface IExpressionEvaluator 
    {
        IDebugProperty EvaluateExpression(string expression);
        IDebugProperty GetLocals();        
    }
}
