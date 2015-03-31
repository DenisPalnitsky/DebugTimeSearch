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
                    var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();
                    if (expressionEvaluatorProvider.IsEvaluatorAvailable)
                    {
                        if (!_evaluatedProperties.ContainsKey(propertyInfo.bstrFullName))
                        {
                            IDebugProperty property = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(propertyInfo.bstrFullName);
                            return property.PropertyInfo;
                        }
                        else
                            return _evaluatedProperties[propertyInfo.bstrFullName];
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
