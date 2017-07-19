using SearchLocals.Model.VSPropertyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchLocals.UI
{
    // TODO: Disable when debugger inactive
    class DebugPropertyViewModel
    {
        
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string FullName { get; private set; }
        public string ValueType { get; private set; }

        public string ToolTip
        {
            get
            {
                return String.Format("Full Name:\t{0}\nFull path:\t{1}", FullName, String.Join("\\",Parents));
            }
        }

        public IEnumerable<string> Parents { get; private set; }

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
                FullName = debugPropertyInfo.FullName,
                Parents = ListParents(debugPropertyInfo)
            };

            return vm;
        }

        private static IEnumerable<string> ListParents(IPropertyInfo debugPropertyInfo)
        {
            List<string> parents = new List<string>();
            IExpandablePropertyInfo parent = debugPropertyInfo.Parent;
            while (parent != null)
            {
                parents.Insert(0, parent.Name);
                parent = parent.Parent;
            }

            return parents;
        }

        static DebugPropertyViewModel From(IExpandablePropertyInfo debugPropertyInfo)
        {
            DebugPropertyViewModel vm = new DebugPropertyViewModel()
            {
                Name = debugPropertyInfo.Name,
                Value = "Expandable",
                ValueType = debugPropertyInfo.ValueType,
                FullName = debugPropertyInfo.FullName,
                Parents = ListParents(debugPropertyInfo)
            };

            return vm;
        }
    }
}
