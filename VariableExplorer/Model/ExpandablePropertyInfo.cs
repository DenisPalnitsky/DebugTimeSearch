﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model
{
    class ExpandablePropertyInfo : IExpandablePropertyInfo
    {
        private Microsoft.VisualStudio.Debugger.Interop.DEBUG_PROPERTY_INFO propertyInfo;

        public ExpandablePropertyInfo(Microsoft.VisualStudio.Debugger.Interop.DEBUG_PROPERTY_INFO propertyInfo)
        {
            // TODO: Complete member initialization
            this.propertyInfo = propertyInfo;
        }

        public string FullName
        {
            get { return propertyInfo.bstrFullName; }
        }

        public string Name
        {
            get { return propertyInfo.bstrName; }
        }

        public string ValueType
        {
            get { return propertyInfo.bstrType; }
        }
    }
}
