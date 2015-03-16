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
            get { return new DelegateCommand( EvaluateExpression ); }
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
            var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();

            if (expressionEvaluatorProvider.IsEvaluatorAvailable )
            {
                _property = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(ExpressionText);                                
                OnPropertyChanged(()=> Properties);                
            }
            else
                LogText = "ExpressionEvaluator is not initialized";
        }

        public IEnumerable<DebugPropertyViewModel> Properties  
        { 
            get 
            {
                var result = new List<DebugPropertyViewModel>();
                if (_property != null)
                {
                    result.Add(DebugPropertyViewModel.From(_property.PropertyInfo ));                    
                    result.AddRange(_property.Children.Select( c=> DebugPropertyViewModel.From(c)));
                    
                }
                return result;
            }
        }

    }
}
