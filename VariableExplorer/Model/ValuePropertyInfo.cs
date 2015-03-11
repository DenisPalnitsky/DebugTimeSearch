using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class ValuePropertyInfo : MyCompany.VariableExplorer.Model.IValuePropertyInfo
    {
        string _value;

        public ValuePropertyInfo(string fullName, string name, string valueType, string value)
            : this(fullName, name, valueType)
        {
            _value = value;
            IsValueEvaluated = true;
        }

        public ValuePropertyInfo(string fullName, string name, string valueType) 
        {
            Name = name;            
            ValueType = valueType;
            FullName = fullName;            
            IsValueEvaluated = false;
        }

        public string ValueType
        {
            get;
            private set;
        }

        public string Value
        {
            get
            {          
                return _value;
            }
        }

        public string Name
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            private set;
        }

        public bool IsValueEvaluated { get; private set; }
    }
}
