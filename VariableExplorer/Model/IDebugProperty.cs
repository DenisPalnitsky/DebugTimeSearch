using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IDebugProperty
    {
        System.Collections.Generic.IEnumerable<DebugPropertyInfo> Children { get; }
        DebugPropertyInfo PropertyInfo { get; }
    }
}
