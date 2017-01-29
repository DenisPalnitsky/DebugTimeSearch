using MyCompany.VariableExplorer.Model.ExpressioEvaluation;
using MyCompany.VariableExplorer.Model.Services;
using MyCompany.VariableExplorer.Model.VSPropertyModel;
using System;
using System.Collections.Generic;

namespace MyCompany.VariableExplorer.Model
{
    class PropertyIterator
    {        
        IExpressionEvaluatorProvider _exparessionEvaluatorProvider;
        IPropertyVisitor _propertyVisitor;
        HashSet<string> _processedExpressions = new HashSet<string>();
        ILog _logger = IocContainer.Resolve<ILog>();
        

        public PropertyIterator (
            IExpressionEvaluatorProvider exparessionEvaluatorProvider,
            IPropertyVisitor propertyVisitor )
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;
            _propertyVisitor = propertyVisitor;            
        }            


        public void TraversPropertyTree (
            IDebugProperty debugProperty,
            string searchCriteria)
        {
             StringFilter stringFilter = new StringFilter(searchCriteria);
             TraversPropertyTreeInternal(debugProperty, stringFilter);            
            _propertyVisitor.Dispose();
        }

        /// <summary>
        /// Traversal of Properties Tree (deep-fir, recursive)
        /// </summary>         
        private void TraversPropertyTreeInternal(
            IDebugProperty debugProperty,
            StringFilter stringFilter)
        {
            _logger.Info("TRaversing property {0}",  debugProperty.PropertyInfo.FullName);
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
                        TraversPropertyTreeInternal(evaluated, stringFilter);
                }
                else
                    throw new NotSupportedException("This property info type is not supported. Contact developer.");
            }
        }
        


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
