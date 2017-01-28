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
            IPropertyVisitor propertyVisitor )
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;
            _propertyVisitor = propertyVisitor;            
        }            

        public static IPropertyVisitor CreateThreadSafeActionBasedVisitor(
                    Action<IEnumerable<IExpandablePropertyInfo>> expandablePropertyAttended,
                Action<IEnumerable<IValuePropertyInfo>> valuePropertyAttended)
        {
            return new ThreadSafeActionBasedPropertyVisitor(expandablePropertyAttended, valuePropertyAttended);
        }

        public void TraversalOfPropertyTreeDeepFirst (
            IDebugProperty debugProperty,
            string searchCriteria)
        {
             StringFilter stringFilter = new StringFilter(searchCriteria);

             RecursiveTraversalOfPropertyTreeDeepFirst(debugProperty, stringFilter);
            
            _propertyVisitor.Dispose();
        }

        private void RecursiveTraversalOfPropertyTreeDeepFirst (
            IDebugProperty debugProperty,
            StringFilter stringFilter)
        {
            // visit root            
            RiseAppropriateAction(debugProperty.PropertyInfo, stringFilter);
            
            // travers all children
            foreach (IPropertyInfo childProperty in debugProperty.Children)
            {
                var valueProperty = childProperty as IValuePropertyInfo;
                if (valueProperty != null)
                {
                    RiseAppropriateAction(childProperty, stringFilter);
                }
                else if (childProperty is IExpandablePropertyInfo)
                {
                    IDebugProperty evaluated = null;
                    evaluated = EvaluateExpression(childProperty);                         

                    if (evaluated != null)
                        RecursiveTraversalOfPropertyTreeDeepFirst(evaluated, stringFilter);
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

        private void RiseAppropriateAction(IPropertyInfo propertyInfo, StringFilter stringFilter)
        {
            if (propertyInfo == null)
                return;
            
            bool nameMatches = (stringFilter.IsMatching(propertyInfo.Name) || stringFilter.IsMatching(propertyInfo.FullName));

            if (propertyInfo is IExpandablePropertyInfo)
            {
                if (nameMatches)
                    _propertyVisitor.ParentPropertyAttended((IExpandablePropertyInfo)propertyInfo);
            }
            else if (propertyInfo is IValuePropertyInfo)
            {
                var valuePropertyInfo = (IValuePropertyInfo)propertyInfo;

                if (!valuePropertyInfo.IsEvaluated)
                {
                    IDebugProperty eveluatedProperty = EvaluateExpression(valuePropertyInfo);
                    if (eveluatedProperty != null && eveluatedProperty.PropertyInfo is IValuePropertyInfo)
                        valuePropertyInfo = (IValuePropertyInfo)eveluatedProperty.PropertyInfo;
                }

                if (nameMatches || stringFilter.IsMatching(valuePropertyInfo.Value))
                    _propertyVisitor.ValuePropertyAttended(valuePropertyInfo);

            }
            else
                throw new NotSupportedException("This property info type is not supported. Contact developer.");
        }
        
      
       
    }
}
