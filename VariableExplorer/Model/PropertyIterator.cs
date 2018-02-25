using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.Model.Services;
using SearchLocals.Model.VSPropertyModel;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SearchLocals.Model
{
    class PropertyIterator
    {        
        IExpressionEvaluatorProvider _exparessionEvaluatorProvider;
        IPropertyVisitor _propertyVisitor;
        HashSet<string> _processedExpressions = new HashSet<string>();
        ILog _logger = ServiceLocator.Resolve<ILog>();
        ISearchStatus _searchStatus = ServiceLocator.Resolve<ISearchStatus>();

        bool _isCanceling = false;


        public int MaxDepth { get; private set; }

        public void Cancel()
        {
            _isCanceling = true;
        }

        public PropertyIterator (
            IExpressionEvaluatorProvider exparessionEvaluatorProvider,
            IPropertyVisitor propertyVisitor ):this(exparessionEvaluatorProvider, propertyVisitor, 50)
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;               
        }

        public PropertyIterator(
            IExpressionEvaluatorProvider exparessionEvaluatorProvider,
            IPropertyVisitor propertyVisitor,
            int maxDepth)
        {
            this._exparessionEvaluatorProvider = exparessionEvaluatorProvider;
            _propertyVisitor = propertyVisitor;
            MaxDepth = maxDepth;
        }

        public void TraversPropertyTree (
            IVSDebugPropertyProxy debugProperty,
            string searchCriteria)
        {
             StringFilter stringFilter = new StringFilter(searchCriteria);
            _isCanceling = false;
             TraversPropertyTreeInternal(debugProperty, stringFilter, 0);            
            _propertyVisitor.Dispose();
        }

        /// <summary>
        /// Traversal of Properties Tree (deep-fir, recursive)
        /// </summary>         
        private void TraversPropertyTreeInternal(
            IVSDebugPropertyProxy debugProperty,
            StringFilter stringFilter,
            int depth)
        {
            depth++;
            if (depth > MaxDepth)
            {               
                _searchStatus.StatusUpdated($"Skipping property: {debugProperty.PropertyInfo.FullName}. Max depth reached");
                return;
            }

            if (SelfReferenceDetection.DoesItLookLikeSelfReference(debugProperty.PropertyInfo.Name))
            {
                _searchStatus.StatusUpdated($"Skip traversing property {debugProperty.PropertyInfo.FullName}. Property looks like referece to self object");
                return;
            }

            ThrowIfCancelRequested();

            _logger.Info("Traversing property {0}", debugProperty.PropertyInfo.FullName);
            // visit root            
            RiseAppropriateAction(debugProperty.PropertyInfo, stringFilter);

            // travers all children
            foreach (IPropertyInfo unevalProperty in debugProperty.Children)
            {
                ThrowIfCancelRequested();

                IVSDebugPropertyProxy evaluated = EvaluateExpression(unevalProperty);

                // null in case we can skip that property
                if (evaluated == null)
                    continue;

                var valueProperty = evaluated.PropertyInfo as IValuePropertyInfo;                

                if (valueProperty != null)
                {
                    RiseAppropriateAction(valueProperty, stringFilter);
                }
                else if (evaluated.PropertyInfo is IExpandablePropertyInfo)
                {                                                       
                    TraversPropertyTreeInternal(evaluated, stringFilter, depth);
                }
                else
                    throw new NotSupportedException($"Property info type { evaluated.PropertyInfo.GetType().Name } is not supported. Contact developer.");
            }
        }

        private void ThrowIfCancelRequested()
        {            
            if (_isCanceling)
            {
                _searchStatus.StatusUpdated("Search canceled");
                _logger.Info("Canceling traversing");
                throw new System.Threading.Tasks.TaskCanceledException("Search canceled");
            }
        }


        private IVSDebugPropertyProxy EvaluateExpression(IPropertyInfo propertyToEvaluate)
        {
            // property name in [] means that it's parent property and should not be evaluated
            // property namy with ( ) means that we cas type which is not required
            if (
                _exparessionEvaluatorProvider.IsEvaluatorAvailable &&
                !_processedExpressions.Contains(propertyToEvaluate.FullName)
                // (!propertyToEvaluate.Name.StartsWith("[") && !propertyToEvaluate.Name.EndsWith("]")) &&
                //!propertyToEvaluate.Name.Any(c=> c == ',' || c == ' '  || c == '(')  &&                
                //!propertyToEvaluate.FullName.StartsWith("(")  
                )                 
            {
                _searchStatus.Report($"Evaluating { propertyToEvaluate.FullName } ");
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
                    IVSDebugPropertyProxy eveluatedProperty = EvaluateExpression(valuePropertyInfo);
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
