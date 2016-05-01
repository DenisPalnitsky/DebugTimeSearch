using Microsoft.VisualStudio.Debugger.Interop;
using MyCompany.VariableExplorer.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class PropertyInfoFactory
    {
        Dictionary<string, IPropertyInfo> _evaluatedProperties = new Dictionary<string, IPropertyInfo>();

        public IPropertyInfo Create(DEBUG_PROPERTY_INFO propertyInfo ) 
        {
            if (propertyInfo.dwAttrib.HasFlag(enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE))
            {
                return new ExpandablePropertyInfo(propertyInfo);
            }
            else
            {
                if (propertyInfo.dwAttrib.HasFlag(enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_ERROR))
                {
                    // evaluate fetched properties                 
                    return new BrokenValuePropertyInfo(propertyInfo.bstrFullName,
                                                propertyInfo.bstrName,                                                
                                                propertyInfo.bstrValue);
                 
                }
                else if ( propertyInfo.bstrName == null &&
                            propertyInfo.bstrType == null &&
                            propertyInfo.bstrValue == null)
                {
                    return new BrokenValuePropertyInfo("<Name is null>",
                                              "<Name is null>",
                                              "<Value is null>");
                }
                else
                {
                    return new ValuePropertyInfo(propertyInfo.bstrFullName,
                                                propertyInfo.bstrName,
                                                propertyInfo.bstrType,
                                                propertyInfo.bstrValue);
                }
                
            }
        }
    }
}
