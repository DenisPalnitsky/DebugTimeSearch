using System;
namespace MyCompany.VariableExplorer.Model
{
    interface IValuePropertyInfo : IPropertyInfo
    {                
        string Value { get; }        
    }
}
