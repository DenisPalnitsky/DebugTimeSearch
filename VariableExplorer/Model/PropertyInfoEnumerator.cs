﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    /// <summary>
    /// Get the flat list of value properties expanding expandable properties
    /// </summary>
    class PropertyInfoEnumerator 
    {        

        internal static IEnumerable<IValuePropertyInfo> Enumerate(IEnumerable<IPropertyInfo> propertyInfo, 
                                                               IExpressionEvaluatorProvider exparessionEvaluatorProvider)
        {
            foreach (var property in propertyInfo)
            {
                var valueProperty  = property as IValuePropertyInfo;
                if (valueProperty != null)
                    yield return valueProperty;
                else if (property is IExpandablePropertyInfo)
                {
                    // property name in [] means that it's parent property and should not be evaluated
                    if ((!property.Name.StartsWith("[") && !property.Name.EndsWith("]") && exparessionEvaluatorProvider.IsEvaluatorAvailable ))
                    {
                        foreach (var child in Enumerate(exparessionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(property.FullName).Children,
                                                    exparessionEvaluatorProvider))
                        {
                            yield return child;
                        }
                    }
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }

        }
                 
    }
}