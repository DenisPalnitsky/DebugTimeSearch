using System;

namespace SearchLocals.Model.VSPropertyModel
{
    class BrokenValuePropertyInfo : IValuePropertyInfo
    {        
        public BrokenValuePropertyInfo(string fullName, string name, string errorMessage, IExpandablePropertyInfo parent)
        {
            // TODO: Complete member initialization
            FullName = fullName;
            Name = name;
            Value = errorMessage;
            Parent = parent;
        }




        public bool IsEvaluated
        {
            get { return false; }
        }

        public string Value
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }


        public string ValueType
        {
            get { return "[Evaluation error]"; }
        }

        public IExpandablePropertyInfo Parent
        {
            get; private set;
        }
    }
}
