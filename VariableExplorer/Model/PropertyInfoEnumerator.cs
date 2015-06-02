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
        IExpressionEvaluatorProvider _exparessionEvaluatorProvider;
        IPropertyVisitor _propertyVisitor;


        public PropertyInfoEnumerator(IExpressionEvaluatorProvider exparessionEvaluatorProvider,
            IPropertyVisitor propertyVisitor)
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;
            _propertyVisitor = propertyVisitor;
        }

        internal IEnumerable<IValuePropertyInfo> Enumerate(IEnumerable<IPropertyInfo> propertyInfo)
        {
            foreach (var property in propertyInfo)
            {
                var valueProperty  = property as IValuePropertyInfo;
                if (valueProperty != null)
                {
                    RiseAppropriateAction(valueProperty);
                    yield return valueProperty;
                }
                else if (property is IExpandablePropertyInfo)
                {
                    // property name in [] means that it's parent property and should not be evaluated
                    if ((!property.Name.StartsWith("[") && !property.Name.EndsWith("]") && _exparessionEvaluatorProvider.IsEvaluatorAvailable))
                    {
                        foreach (var child in Enumerate(_exparessionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(property.FullName).Children))
                        {
                            RiseAppropriateAction(child);
                            yield return child;
                        }
                    }
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }

        }

        #region Visitor stuff
        private void RiseAppropriateAction(IPropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return;

            if (propertyInfo is IExpandablePropertyInfo)
                _propertyVisitor.ParentPropertyAttended((IExpandablePropertyInfo)propertyInfo);
            else if (propertyInfo is IValuePropertyInfo)
                _propertyVisitor.ValuePropertyAttended((IValuePropertyInfo)propertyInfo);
            else
                throw new NotSupportedException("This property info type is not supported. Contact developer.");
        }

        public static IPropertyVisitor CreateActionBasedVisitor(Action<IExpandablePropertyInfo> expandablePropertyAttended,
                Action<IValuePropertyInfo> valuePropertyAttended)
        {
            return new ActionBasedPropertyVisitor(expandablePropertyAttended, valuePropertyAttended);
        }

        private class ActionBasedPropertyVisitor : IPropertyVisitor
        {
            private Action<IExpandablePropertyInfo> _expandablePropertyAttended;
            private Action<IValuePropertyInfo> _valuePropertyAttended;
            public ActionBasedPropertyVisitor(Action<IExpandablePropertyInfo> expandablePropertyAttended,
                Action<IValuePropertyInfo> valuePropertyAttended)
            {
                _expandablePropertyAttended = expandablePropertyAttended;
                _valuePropertyAttended = valuePropertyAttended;
            }

            public void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
            {
                _expandablePropertyAttended(expandablePropertyInfo);
            }

            public void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
            {
                _valuePropertyAttended(valuePropertyInfo);
            }
        }    
        #endregion
    }
}
