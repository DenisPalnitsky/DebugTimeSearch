using System;
namespace SearchLocals.UI
{
    interface IExpressionEvaluatorViewModel
    {        
        string FilterText { get; set; }        
        System.Collections.Generic.IEnumerable<DebugPropertyViewModel> Properties { get; }
    }
}
