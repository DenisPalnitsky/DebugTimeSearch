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

        internal static DebugPropertyViewModel From(Model.IDebugPropertyInfo debugPropertyInfo)
        {
            DebugPropertyViewModel vm = new DebugPropertyViewModel()
            {
                Name = debugPropertyInfo.Name,
                Value = debugPropertyInfo.Value,
                ValueType = debugPropertyInfo.ValueType
            };

            return vm;
        }
    }
}
