using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    /// <summary>
    /// This shitty class doesn' work
    /// </summary>
    class PropertyIterator 
    {        
        IExpressionEvaluatorProvider _exparessionEvaluatorProvider;
        IPropertyVisitor _propertyVisitor;
        

        public PropertyIterator (IExpressionEvaluatorProvider exparessionEvaluatorProvider,
            IPropertyVisitor propertyVisitor)
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;
            _propertyVisitor = propertyVisitor;
        }
  

        internal void TraversalOfPropertyTree (
            IDebugProperty debugProperty)
        {
            // visit root            
            RiseAppropriateAction(debugProperty.PropertyInfo);
            
            // travers all children
            foreach (var childProperty in debugProperty.Children)
            {
                var valueProperty = childProperty as IValuePropertyInfo;
                if (valueProperty != null)
                    RiseAppropriateAction(childProperty);
                else if (childProperty is IExpandablePropertyInfo)
                {
                    // property name in [] means that it's parent property and should not be evaluated
                    if ((!childProperty.Name.StartsWith("[") && !childProperty.Name.EndsWith("]") && _exparessionEvaluatorProvider.IsEvaluatorAvailable))
                    {
                        TraversalOfPropertyTree(_exparessionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(childProperty.FullName));                        
                    }
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }
        }

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

        public static IPropertyVisitor  CreateActionBasedVisitor(Action<IExpandablePropertyInfo> expandablePropertyAttended,
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
    }
}
