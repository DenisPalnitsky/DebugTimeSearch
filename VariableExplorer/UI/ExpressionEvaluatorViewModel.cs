using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompany.VariableExplorer.Model;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MyCompany.VariableExplorer.Model.Services;

namespace MyCompany.VariableExplorer.UI
{
    class ExpressionEvaluatorViewModel : ObservableObject, MyCompany.VariableExplorer.UI.IExpressionEvaluatorViewModel
    {
        IDebugProperty _property;
        string _expressionText;
        IEnumerable<DebugPropertyViewModel> _visibleProperties;
        ILog _logger;

        public ExpressionEvaluatorViewModel(ILog logger)
        {
            _logger = logger;
        }

        public string ExpressionText
        {
            get { return _expressionText; }
            set 
            { 
                _expressionText = value;
                EvaluateExpression();
                OnPropertyChanged();
            }
        }
        
        public ICommand EvaluateExpressionCommand
        {
            get 
            {                
                return new DelegateCommand( EvaluateExpression ); 
            }
        }
        

        private void EvaluateExpression()
        {
            try
            {
                var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();

                if (expressionEvaluatorProvider.IsEvaluatorAvailable)
                {
                    _property = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(ExpressionText);

                    var result = new List<DebugPropertyViewModel>();

                    if (_property != null)
                    {
                        if (_property.PropertyInfo is ValuePropertyInfo)
                        {
                            result.Add(DebugPropertyViewModel.From(_property.PropertyInfo));
                        }

                        foreach (var childProperty in PropertyInfoEnumerator.Enumerate(_property.Children, expressionEvaluatorProvider))
                            result.Add(DebugPropertyViewModel.From(childProperty));
                        //foreach (var childProperty in _property.Children)
                        //    result.Add(DebugPropertyViewModel.From(childProperty));
                    }
                    Properties = result;
                }
                else
                    _logger.Info("ExpressionEvaluator is not initialized");
            }
            catch (Exception e)
            {
                _logger.Info("Exception during EvaluateExpression. {0} ",e.ToString());
            }
        }

        public IEnumerable<DebugPropertyViewModel> Properties  
        { 
            get 
            {
                return _visibleProperties;              
            }

            set
            {
                _visibleProperties = value;
                OnPropertyChanged(() => Properties);
            }
        }

    }
}
