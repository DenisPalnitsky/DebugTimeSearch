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
        TaskFactory _taskFactory = new TaskFactory();

        public PropertyIterator (IExpressionEvaluatorProvider exparessionEvaluatorProvider,
            IPropertyVisitor propertyVisitor)
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;
            _propertyVisitor = propertyVisitor;
        }
  

        internal void TraversalOfPropertyTreeDeepFirst (
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
                    if ((!childProperty.Name.StartsWith("[") && !childProperty.Name.EndsWith("]") && _exparessionEvaluatorProvider.IsEvaluatorAvailable))
                    {
                        ParrallelTraversalOfPropertyTreeDeepFirst(_exparessionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(childProperty.FullName));
                    }
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }
        }

       internal Task ParrallelTraversalOfPropertyTreeDeepFirst(
            IDebugProperty debugProperty)
        {
            return _taskFactory.StartNew(() => TraversalOfPropertyTreeDeepFirst(debugProperty));
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

        public static IPropertyVisitor CreateThreadSafeActionBasedVisitor(Action<IExpandablePropertyInfo> expandablePropertyAttended,
               Action<IValuePropertyInfo> valuePropertyAttended)
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
        }

        private class ThreadSafeActionBasedPropertyVisitor : ActionBasedPropertyVisitor
        {
            object lockObject = new object();

            public ThreadSafeActionBasedPropertyVisitor(Action<IExpandablePropertyInfo> expandablePropertyAttended,
                Action<IValuePropertyInfo> valuePropertyAttended)
                : base(expandablePropertyAttended, valuePropertyAttended)
            {                
            }

            public void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
            {
                lock (lockObject)
                {
                    base.ParentPropertyAttended(expandablePropertyInfo);
                }
            }

            public void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
            {
                lock (lockObject)
                {
                    base.ValuePropertyAttended(valuePropertyInfo);
                }
            }
        }
        
        #endregion
    }
}
