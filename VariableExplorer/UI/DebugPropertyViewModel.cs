using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.UI
{
    class DebugPropertyViewModel
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string ValueType { get; private set; }

        private DebugPropertyViewModel() { }

        public static DebugPropertyViewModel From(Model.IPropertyInfo debugPropertyInfo)
        {
            if (debugPropertyInfo is Model.IExpandablePropertyInfo)
                return From((Model.IExpandablePropertyInfo)debugPropertyInfo);

            if (debugPropertyInfo is Model.IValuePropertyInfo)
                return From((Model.IValuePropertyInfo)debugPropertyInfo);

            throw new NotSupportedException();
        }

        static DebugPropertyViewModel From(Model.IValuePropertyInfo debugPropertyInfo)
        {
            DebugPropertyViewModel vm = new DebugPropertyViewModel()
            {
                Name = debugPropertyInfo.FullName,
                Value = debugPropertyInfo.Value,
                ValueType = debugPropertyInfo.ValueType
            };

            return vm;
        }

        static DebugPropertyViewModel From(Model.IExpandablePropertyInfo debugPropertyInfo)
        {
            DebugPropertyViewModel vm = new DebugPropertyViewModel()
            {
                Name = debugPropertyInfo.FullName,
                Value = "Expandable",
                ValueType = debugPropertyInfo.ValueType
            };

            return vm;
        }
    }
}
