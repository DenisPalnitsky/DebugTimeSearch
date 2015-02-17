using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IDebugPropertyInfo
    {
        string FullName { get; }
        bool IsValueEvaluated { get; }
        string Name { get; }
        string Value { get; }
        string ValueType { get; }
    }
}
