using System.Collections.Generic;

namespace SearchLocals.Model.VSPropertyModel
{
    interface IDebugProperty
    {
        IEnumerable<IPropertyInfo> Children { get; }
        IPropertyInfo PropertyInfo { get; }
    }
}
