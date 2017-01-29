using MyCompany.VariableExplorer.Model.VSPropertyModel;
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
        public string FullName { get; private set; }
        public string ValueType { get; private set; }

        private DebugPropertyViewModel() { }

        public static DebugPropertyViewModel From(IPropertyInfo debugPropertyInfo)
        {
            if (debugPropertyInfo is IExpandablePropertyInfo)
                return From((IExpandablePropertyInfo)debugPropertyInfo);

            if (debugPropertyInfo is IValuePropertyInfo)
                return From((IValuePropertyInfo)debugPropertyInfo);

            throw new NotSupportedException();
        }

        static DebugPropertyViewModel From(IValuePropertyInfo debugPropertyInfo)
        {
            DebugPropertyViewModel vm = new DebugPropertyViewModel()
            {
                Name = debugPropertyInfo.Name,
                Value = debugPropertyInfo.Value,
                ValueType = debugPropertyInfo.ValueType,
                FullName = debugPropertyInfo.FullName

            };

            return vm;
        }

        static DebugPropertyViewModel From(IExpandablePropertyInfo debugPropertyInfo)
        {
            DebugPropertyViewModel vm = new DebugPropertyViewModel()
            {
                Name = debugPropertyInfo.Name,
                Value = "Expandable",
                ValueType = debugPropertyInfo.ValueType,
                FullName = debugPropertyInfo.FullName
            };

            return vm;
        }
    }
}
