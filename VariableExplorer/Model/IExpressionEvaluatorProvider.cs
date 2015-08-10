using MyCompany.VariableExplorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model
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
