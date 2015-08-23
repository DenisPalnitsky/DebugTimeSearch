using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class ValuePropertyInfo : IValuePropertyInfo
    {
        string _value;

        public ValuePropertyInfo(string fullName, string name, string valueType, string value)
            : this(fullName, name, valueType, value, true)
        {            
        }

        public ValuePropertyInfo(string fullName, string name, string valueType)
            : this(fullName, name, valueType, null, false)
        {                        
        }

        private ValuePropertyInfo(string fullName, string name, string valueType, string value, bool isEvaluated)
        {
            if (String.IsNullOrEmpty(fullName) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(valueType))
                throw new InvalidOperationException("Name and ValueType should not be null");

            Name = name;
            ValueType = valueType;
            FullName = fullName;
            IsEvaluated = isEvaluated;
            _value = value;
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
                if (!IsEvaluated)
                    return "[Not Evaluated]";

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


        public bool IsEvaluated         
        {            
            get;            
            private set;     
        }

    }
}
