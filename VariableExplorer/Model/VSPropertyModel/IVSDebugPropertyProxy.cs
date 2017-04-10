using System.Collections.Generic;

namespace SearchLocals.Model.VSPropertyModel
{
    interface IVSDebugPropertyProxy
    {
        IEnumerable<IPropertyInfo> Children { get; }
        IPropertyInfo PropertyInfo { get; }
    }
}
