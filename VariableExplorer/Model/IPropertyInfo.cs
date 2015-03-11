using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IPropertyInfo
    {
        string FullName { get; }
        string Name { get; }
        string ValueType { get; }
    }
}
