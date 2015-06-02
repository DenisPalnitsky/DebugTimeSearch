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
        ObservableCollection<DebugPropertyViewModel> _visibleProperties = new ObservableCollection<DebugPropertyViewModel>();
        ILog _logger;
        private string _errorMessage;

        public ExpressionEvaluatorViewModel(ILog logger)
        {
            _logger = logger;
            _visibleProperties.CollectionChanged += _visibleProperties_CollectionChanged;
        }

        void _visibleProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(() => Properties);
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

                    _visibleProperties.Clear();

                    if (_property != null)
                    {
                        PropertyIterator propertyIterator = new PropertyIterator(
                            expressionEvaluatorProvider,
                            PropertyIterator.CreateActionBasedVisitor(
                                    p => _visibleProperties.Add(DebugPropertyViewModel.From(p)) , 
                                    v=> _visibleProperties.Add(DebugPropertyViewModel.From(v))));
                        propertyIterator.TraversalOfPropertyTree(_property);                        
                    }                    
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
        }

        public string Json
        {
            get
            {
                StringBuilder text = new StringBuilder();
                foreach (DebugPropertyViewModel property in Properties)
                {
                    text.AppendFormat("{0} = {1}", property.Name, property.Value);
                }

                return text.ToString();
            }            
        }

    }
}
