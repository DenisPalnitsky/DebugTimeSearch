using System.Collections.Generic;

namespace MyCompany.VariableExplorer.Model.VSPropertyModel
{
    interface IDebugProperty
    {
        IEnumerable<IPropertyInfo> Children { get; }
        IPropertyInfo PropertyInfo { get; }
    }
}
