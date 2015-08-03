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
using System.Windows;
using System.Windows.Threading;

namespace MyCompany.VariableExplorer.UI
{

    class ExpressionEvaluatorViewModel : ObservableObject, MyCompany.VariableExplorer.UI.IExpressionEvaluatorViewModel
    {
        IDebugProperty _property;
        string _expressionText;
        ObservableCollection<DebugPropertyViewModel> _visibleProperties = new ObservableCollection<DebugPropertyViewModel>();
        object _visiblePropertiesLock = new object();

        ILog _logger;
        private string _errorMessage;

        public ExpressionEvaluatorViewModel(ILog logger)
        {
            _logger = logger;
            _visibleProperties.CollectionChanged += _visibleProperties_CollectionChanged;
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_visibleProperties, _visiblePropertiesLock);
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
                        var eventSink = PropertyIterator.CreateThreadSafeActionBasedVisitor(
                                    p => 
                                        {
                                            Application.Current.Dispatcher.Invoke(()=>_visibleProperties.Add(DebugPropertyViewModel.From(p)));
                                        }, 
                                    v=> {
                                            Application.Current.Dispatcher.Invoke(() => _visibleProperties.Add(DebugPropertyViewModel.From(v)));
                                        });

                        PropertyIterator propertyIterator = new PropertyIterator(
                            expressionEvaluatorProvider,
                            eventSink, 
                            new ParallelTaskFactory());

                        propertyIterator.TraversalOfPropertyTreeDeepFirst(_property);
                    
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
      
    }
}
