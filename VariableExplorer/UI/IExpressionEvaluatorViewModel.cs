using System;
namespace MyCompany.VariableExplorer.UI
{
    interface IExpressionEvaluatorViewModel
    {        
        string ExpressionText { get; set; }        
        System.Collections.Generic.IEnumerable<DebugPropertyViewModel> Properties { get; }
    }
}
