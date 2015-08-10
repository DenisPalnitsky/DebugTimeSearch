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
            foreach (var childProperty in debugProperty.Children)
            {
                var valueProperty = childProperty as IValuePropertyInfo;
                if (valueProperty != null)
                {
                    RiseAppropriateAction(childProperty);
                }
                else if (childProperty is IExpandablePropertyInfo)
                {
                    // property name in [] means that it's parent property and should not be evaluated
                    if ( !_processedExpressions.Contains(childProperty.FullName) &&
                        (!childProperty.Name.StartsWith("[") && !childProperty.Name.EndsWith("]")
                        && _exparessionEvaluatorProvider.IsEvaluatorAvailable))
                    {
                          RecursiveTraversalOfPropertyTreeDeepFirst(EvaluateExpression(childProperty.FullName));                        
                    }
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }
        }

        HashSet<string> _processedExpressions = new HashSet<string>();
        private IDebugProperty EvaluateExpression(string expression)
        {
            _processedExpressions.Add(expression);
            return _exparessionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(expression);
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

        #region InternalCalsses

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

            public virtual  void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
            {
                _expandablePropertyAttended(expandablePropertyInfo);
            }

            public virtual void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
            {
                _valuePropertyAttended(valuePropertyInfo);
            }

            public void Dispose()
            {
                // no need to do anything
            }
        }

        private class ThreadSafeActionBasedPropertyVisitor : IPropertyVisitor
        {
            List<IPropertyInfo> _propertyInfos = new List<IPropertyInfo>();
            const int ITEMS_TO_RELEASE_PER_EVENT = 100;

            readonly Action<IEnumerable<IExpandablePropertyInfo>> _expandablePropertyAttended;
            readonly Action<IEnumerable<IValuePropertyInfo>> _valuePropertyAttended;

            public ThreadSafeActionBasedPropertyVisitor(Action<IEnumerable<IExpandablePropertyInfo>> expandablePropertyAttended,
                Action<IEnumerable<IValuePropertyInfo>> valuePropertyAttended)
                
            {
                _expandablePropertyAttended = expandablePropertyAttended;
                _valuePropertyAttended = valuePropertyAttended;
            }

            public void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
            {
                CheckAndReleaseProperiesInfoList();

                _propertyInfos.Add(expandablePropertyInfo);
            }

            private void CheckAndReleaseProperiesInfoList()
            {
                if (_propertyInfos.Count == ITEMS_TO_RELEASE_PER_EVENT)
                    ReleaseEventList();
            }

            public void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
            {
                CheckAndReleaseProperiesInfoList();
                _propertyInfos.Add(valuePropertyInfo);
            }

            public void Dispose()
            {
                if (_propertyInfos.Count > 0)
                    ReleaseEventList();
            }

            private void ReleaseEventList()
            {
                _valuePropertyAttended(_propertyInfos.OfType<IValuePropertyInfo>());
                _expandablePropertyAttended( _propertyInfos.OfType<IExpandablePropertyInfo>());
                _propertyInfos = new List<IPropertyInfo>();
            }
        }
        
        #endregion
    }
}
