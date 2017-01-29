namespace MyCompany.VariableExplorer.Model.VSPropertyModel
{
    class BrokenValuePropertyInfo : IValuePropertyInfo
    {        
        public BrokenValuePropertyInfo(string fullName, string name, string errorMessage)
        {
            // TODO: Complete member initialization
            FullName = fullName;
            Name = name;
            Value = errorMessage;
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
    }
}
