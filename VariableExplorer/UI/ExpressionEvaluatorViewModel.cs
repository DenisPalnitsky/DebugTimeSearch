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
        private string _errorMessage;

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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            
            private set 
            { 
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(()=>IsErrorMessageVisible);
            }
        }

        public bool IsErrorMessageVisible
        {
            get {  return !String.IsNullOrEmpty(ErrorMessage); }
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
            ErrorMessage = null;

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
                {
                    _logger.Info("ExpressionEvaluator is not initialized");
                    ErrorMessage = "ExpressionEvaluator is not initialized";
                }
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("Exception during EvaluateExpression. {0} ", e.ToString());
                _logger.Info(errorMessage);
                ErrorMessage = errorMessage;
                
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
