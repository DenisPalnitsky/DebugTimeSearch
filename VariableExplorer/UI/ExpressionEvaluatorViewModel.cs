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
    class ExpressionEvaluatorViewModel : ObservableObject
    {
        IDebugProperty _property;
        string _expressionText;
        string _logText;
        IEnumerable<DebugPropertyViewModel> _visibleProperties;

        public ExpressionEvaluatorViewModel()
        {
            IocContainer.RegisterInstance<ILogger>(new  RedirectLogger( m => { LogText += m; } ));
        }

        public string ExpressionText
        {
            get { return _expressionText; }
            set 
            { 
                _expressionText = value;
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

        public string LogText
        {
            get { return _logText; }
         
            set
            {
                _logText = value;
                OnPropertyChanged();
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
                        //foreach (var childProperty in PropertyInfoEnumerator.Enumerate(_property.Children, expressionEvaluatorProvider))
                        //    result.Add(DebugPropertyViewModel.From(childProperty));
                        foreach (var childProperty in _property.Children)
                            result.Add(DebugPropertyViewModel.From(childProperty));
                    }
                    Properties = result;
                }
                else
                    LogText = "ExpressionEvaluator is not initialized";
            }
            catch (Exception e)
            {                
                LogText = e.ToString();
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
