namespace SearchLocals.Model.VSPropertyModel
{
    class UnevaluatedPropertyInfo : IUnevaluatedPropertyInfo
    {
        public UnevaluatedPropertyInfo(string fullName, string name, string valueType, IExpandablePropertyInfo parent)
        {
            FullName = fullName;
            Name = name;
            ValueType = valueType;
            Parent = parent;
        }

        public string FullName { get; private set; } 

        public string Name { get; private set; }

        public string ValueType { get; private set; }

        public IExpandablePropertyInfo Parent { get; private set; }
    }
}
