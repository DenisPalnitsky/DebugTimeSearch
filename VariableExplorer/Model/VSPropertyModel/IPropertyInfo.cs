using System;

namespace SearchLocals.Model.VSPropertyModel
{
    interface IPropertyInfo
    {
        string FullName { get; }

        string Name { get; }

        string ValueType { get; }

        IExpandablePropertyInfo Parent { get; }
    }
}
