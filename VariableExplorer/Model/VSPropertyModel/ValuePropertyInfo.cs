using System;

namespace MyCompany.VariableExplorer.Model.VSPropertyModel
{
    class ValuePropertyInfo : IValuePropertyInfo
    {
        string _value;

        public ValuePropertyInfo(string fullName, string name, string valueType, string value, IExpandablePropertyInfo parent)
            : this(fullName, name, valueType, value, true, parent)
        {            
        }

        private ValuePropertyInfo(string fullName, 
            string name, 
            string valueType, 
            string value, 
            bool isEvaluated, 
            IExpandablePropertyInfo parent)
        {
            if (String.IsNullOrEmpty(fullName) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(valueType))
                throw new InvalidOperationException("Name and ValueType should not be null");

            Name = name;
            ValueType = valueType;
            FullName = fullName;
            IsEvaluated = isEvaluated;
            _value = value;
            Parent = parent;
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

        public IExpandablePropertyInfo Parent
        {
            get; private set;
        }
    }
}
