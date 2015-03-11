using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IDebugProperty
    {
        System.Collections.Generic.IEnumerable<IPropertyInfo> Children { get; }
        IPropertyInfo PropertyInfo { get; }
    }
}
