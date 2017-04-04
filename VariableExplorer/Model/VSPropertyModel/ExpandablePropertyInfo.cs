using System;

namespace SearchLocals.Model.VSPropertyModel
{
    class ExpandablePropertyInfo : IExpandablePropertyInfo
    {
        private Microsoft.VisualStudio.Debugger.Interop.DEBUG_PROPERTY_INFO propertyInfo;

        public ExpandablePropertyInfo(Microsoft.VisualStudio.Debugger.Interop.DEBUG_PROPERTY_INFO propertyInfo, IExpandablePropertyInfo parent)
        {
            // TODO: Complete member initialization
            this.propertyInfo = propertyInfo;
            Parent = parent;
        }

        public string FullName
        {
            get { return propertyInfo.bstrFullName; }
        }

        public string Name
        {
            get { return propertyInfo.bstrName; }
        }

        public IExpandablePropertyInfo Parent
        {
            get; private set;
        }

        public string ValueType
        {
            get { return propertyInfo.bstrType; }
        }
    }
}
