using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IValuePropertyInfo : IPropertyInfo
    {        
        bool IsValueEvaluated { get; }                
        string Value { get; }        
    }
}
