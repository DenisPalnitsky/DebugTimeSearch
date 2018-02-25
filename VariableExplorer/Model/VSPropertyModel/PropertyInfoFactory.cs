using Microsoft.VisualStudio.Debugger.Interop;

namespace SearchLocals.Model.VSPropertyModel
{
    class PropertyInfoFactory
    {       
        public IPropertyInfo Create(DEBUG_PROPERTY_INFO propertyInfo, IExpandablePropertyInfo parent) 
        {         

            if (propertyInfo.dwAttrib.HasFlag(enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE))
            {
                return new ExpandablePropertyInfo(propertyInfo, parent);
            }
            else
            {
                if (propertyInfo.dwAttrib.HasFlag(enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_ERROR))
                {
                    // evaluate fetched properties                 
                    return new BrokenValuePropertyInfo(propertyInfo.bstrFullName,
                                                propertyInfo.bstrName,                                                
                                                propertyInfo.bstrValue, 
                                                parent);
                 
                }
                else if ( propertyInfo.bstrName == null &&
                            propertyInfo.bstrType == null &&
                            propertyInfo.bstrValue == null)
                {
                    return new BrokenValuePropertyInfo("<Name is null>",
                                              "<Name is null>",
                                              "<Value is null>", 
                                              parent);
                }
                else
                {
                    return new ValuePropertyInfo(propertyInfo.bstrFullName,
                                                propertyInfo.bstrName,
                                                propertyInfo.bstrType,
                                                propertyInfo.bstrValue, 
                                                parent);
                }
                
            }
        }

        internal IUnevaluatedPropertyInfo CreateUnevaluated(DEBUG_PROPERTY_INFO propertyInfo, IExpandablePropertyInfo parent)
        {            
            return new UnevaluatedPropertyInfo(propertyInfo.bstrFullName,
                                                propertyInfo.bstrName,
                                                propertyInfo.bstrType,                                                
                                                parent);
        }
    }
}
