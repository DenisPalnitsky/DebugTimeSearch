using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class PropertyInfoFactory
    {
        public static IPropertyInfo Create(DEBUG_PROPERTY_INFO propertyInfo ) 
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
                    var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();
                    if (expressionEvaluatorProvider.IsEvaluatorAvailable)
                    {
                        IDebugProperty property = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(propertyInfo.bstrFullName);
                        return property.PropertyInfo;                                                
                    }
                    else
                    {
                        return new ValuePropertyInfo(propertyInfo.bstrFullName,
                                                    propertyInfo.bstrName,
                                                    propertyInfo.bstrType,
                                                    "Expression evaluator is not available");
                    }               
                }
                else
                    return new ValuePropertyInfo(propertyInfo.bstrFullName,
                        propertyInfo.bstrName,
                        propertyInfo.bstrType,
                        propertyInfo.bstrValue);
            }
        }
    }
}
