using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class DebugPropertyInfo : MyCompany.VariableExplorer.Model.IDebugPropertyInfo
    {
        DEBUG_PROPERTY_INFO _propertyInfo;

        public DebugPropertyInfo(DEBUG_PROPERTY_INFO propertyInfo, bool isValueEvaluated) 
        {
            _propertyInfo = propertyInfo;
            IsValueEvaluated = isValueEvaluated;
        }

        public string ValueType
        {
            get
            {
                return _propertyInfo.bstrType;
            }
        }

        public string Value
        {
            get
            {
                return _propertyInfo.bstrValue;
            }
        }

        public string Name
        {
            get { return _propertyInfo.bstrName; }
        }

        public string FullName
        {
            get { return _propertyInfo.bstrFullName; }
        }

        public bool IsValueEvaluated { get; private set; }
    }
}
