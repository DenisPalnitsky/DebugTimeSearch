using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompany.VariableExplorer.Model;
using System.Windows.Input;

namespace MyCompany.VariableExplorer.UI
{
    class ExpressionEvaluatorViewModel : ObservableObject
    {
        DebugProperty _property;
        string _expressionText;
        string _logText;

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
            var expressionEvaluator = IocContainer.Resolve<IExpressionEvaluator>();

            if (expressionEvaluator != null)
            {
                _property = expressionEvaluator.EvaluateExpression(ExpressionText);
                LogText = Newtonsoft.Json.JsonConvert.SerializeObject(_property);
            }
            else
                LogText = "ExpressionEvaluator is not initialized";
        }
        
    }
}
