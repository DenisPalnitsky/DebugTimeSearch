using System;
using System.Collections.Generic;
namespace MyCompany.VariableExplorer.Model
{
    interface IDebugProperty
    {
        IEnumerable<IPropertyInfo> Children { get; }
        IPropertyInfo PropertyInfo { get; }
    }
}
