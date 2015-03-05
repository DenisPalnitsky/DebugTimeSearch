using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IDebugProperty
    {
        System.Collections.Generic.IEnumerable<IDebugPropertyInfo> Children { get; }
        IDebugPropertyInfo PropertyInfo { get; }
    }
}
