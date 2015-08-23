using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{   
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
  

        internal void TraversalOfPropertyTreeDeepFirst (
            IDebugProperty debugProperty)
        {
            RecursiveTraversalOfPropertyTreeDeepFirst(debugProperty);
            _propertyVisitor.Dispose();
        }

        private void RecursiveTraversalOfPropertyTreeDeepFirst (
            IDebugProperty debugProperty)
        {
            // visit root            
            RiseAppropriateAction(debugProperty.PropertyInfo);
            
            // travers all children
            foreach (IPropertyInfo childProperty in debugProperty.Children)
            {
                var valueProperty = childProperty as IValuePropertyInfo;
                if (valueProperty != null)
                {
                    RiseAppropriateAction(childProperty);
                }
                else if (childProperty is IExpandablePropertyInfo)
                {
                    IDebugProperty evaluated = null;
                    evaluated = EvaluateExpression(childProperty);                         

                    if (evaluated != null)
                        RecursiveTraversalOfPropertyTreeDeepFirst(evaluated);
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }
        }

        HashSet<string> _processedExpressions = new HashSet<string>();
        private IDebugProperty EvaluateExpression(IPropertyInfo propertyToEvaluate)
        {
            // property name in [] means that it's parent property and should not be evaluated
            if (!_processedExpressions.Contains(propertyToEvaluate.FullName) &&
                (!propertyToEvaluate.Name.StartsWith("[") && !propertyToEvaluate.Name.EndsWith("]")
                && _exparessionEvaluatorProvider.IsEvaluatorAvailable))
            {

                _processedExpressions.Add(propertyToEvaluate.FullName);
                return _exparessionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(propertyToEvaluate.FullName);
            }
            return null;
        }

        private void RiseAppropriateAction(IPropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return;

            if (propertyInfo is IExpandablePropertyInfo)
                _propertyVisitor.ParentPropertyAttended((IExpandablePropertyInfo)propertyInfo);
            else if (propertyInfo is IValuePropertyInfo)
            {
                var valuePropertyInfo =  (IValuePropertyInfo)propertyInfo;
                
                if (valuePropertyInfo.IsEvaluated)
                    _propertyVisitor.ValuePropertyAttended(valuePropertyInfo);
                else
                {
                    IDebugProperty eveluatedProperty = EvaluateExpression(valuePropertyInfo);
                    if (eveluatedProperty != null && eveluatedProperty.PropertyInfo is IValuePropertyInfo)
                        _propertyVisitor.ValuePropertyAttended((IValuePropertyInfo)eveluatedProperty.PropertyInfo);
                }
                

            }
            else
                throw new NotSupportedException("This property info type is not supported. Contact developer.");
        }

        public static IPropertyVisitor CreateActionBasedVisitor(
                Action<IExpandablePropertyInfo> expandablePropertyAttended,
                Action<IValuePropertyInfo> valuePropertyAttended)
        {
            return new ActionBasedPropertyVisitor(expandablePropertyAttended, valuePropertyAttended);
        }

        public static IPropertyVisitor CreateThreadSafeActionBasedVisitor(
                    Action<IEnumerable<IExpandablePropertyInfo>> expandablePropertyAttended,
                Action<IEnumerable<IValuePropertyInfo>> valuePropertyAttended)
        {
            return new ThreadSafeActionBasedPropertyVisitor(expandablePropertyAttended, valuePropertyAttended);
        }
       
    }
}
