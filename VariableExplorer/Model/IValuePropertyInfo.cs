using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IValuePropertyInfo : IPropertyInfo
    {
        bool IsEvaluated { get; }
        string Value { get; }        
    }
}
