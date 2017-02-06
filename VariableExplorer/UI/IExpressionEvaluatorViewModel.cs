using System;
namespace MyCompany.VariableExplorer.UI
{
    interface IExpressionEvaluatorViewModel
    {        
        string FilterText { get; set; }        
        System.Collections.Generic.IEnumerable<DebugPropertyViewModel> Properties { get; }
    }
}
