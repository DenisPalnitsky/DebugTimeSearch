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
using System.Diagnostics;

namespace MyCompany.VariableExplorer.UI
{

    class ExpressionEvaluatorViewModel : ObservableObject, MyCompany.VariableExplorer.UI.IExpressionEvaluatorViewModel
    {
        IDebugProperty _property;
        string _expressionText;
        DebugPropertyViewModelCollection _visibleProperties = new DebugPropertyViewModelCollection();
        object _visiblePropertiesLock = new object();

        ILog _logger;
        private string _errorMessage;
        private string _statusBarText;
        private string _searchText;

        public ExpressionEvaluatorViewModel(ILog logger)
        {
            _logger = logger;
            _visibleProperties.CollectionChanged += visibleProperties_CollectionChanged;
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_visibleProperties, _visiblePropertiesLock);
        }

        public IEnumerable<DebugPropertyViewModel> Properties
        {
            get
            {
                return _visibleProperties;
            }
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

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
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

        public string StatusBarText
        {
            get { return _statusBarText; }
            private set 
            {
                _statusBarText = value;
                OnPropertyChanged(() => StatusBarText);
            }
        }

        
                
        public ICommand EvaluateExpressionCommand
        {
            get 
            {                
                return new DelegateCommand( EvaluateExpression ); 
            }
        }
        
        public ICommand  SearchLocalsCommand
        {
            get { return new DelegateCommand(SearchLocals); }
        }

        private void EvaluateExpression()
        {            
            var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();

            if (expressionEvaluatorProvider.IsEvaluatorAvailable)
            {                
                _property = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(ExpressionText);                
                IterateThrueProperty(expressionEvaluatorProvider);
            }            
        }

        private void SearchLocals()
        {
            var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();
            if (expressionEvaluatorProvider.IsEvaluatorAvailable)
            {
                _property = expressionEvaluatorProvider.ExpressionEvaluator.GetLocals();
                IterateThrueProperty(expressionEvaluatorProvider);
            }
        }


        private void IterateThrueProperty(IExpressionEvaluatorProvider expressionEvaluatorProvider)
        {
            ErrorMessage = null;
            _visibleProperties.Clear();
            StatusBarText = "Searching...";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (_property != null)
            {
                IPropertyVisitor eventSink = PropertyIterator.CreateThreadSafeActionBasedVisitor(
                            expndableProperty =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _visibleProperties.AddRange(expndableProperty.Select(item => DebugPropertyViewModel.From(item)));
                                });
                            },
                            valueProperty =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _visibleProperties.AddRange(valueProperty.Select(item => DebugPropertyViewModel.From(item)));
                                });
                            });

                PropertyIterator propertyIterator = new PropertyIterator(
                    expressionEvaluatorProvider,
                    eventSink);

                Task.Run(
                    () => propertyIterator.TraversalOfPropertyTreeDeepFirst(_property))
                    .ContinueWith(t =>
                    {
                        stopwatch.Stop();
                        PostSearchCompleteMessage(stopwatch.Elapsed);
                    },   
                    IocContainer.Resolve<ITaskSchedulerProvider>().GetCurrentScheduler());
            }
            else
            {
                _logger.Info("ExpressionEvaluator is not initialized");
                ErrorMessage = "ExpressionEvaluator is not initialized";
            }
        }

 

        void visibleProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(() => Properties);
        }
      
        private void  PostSearchCompleteMessage(TimeSpan timeSpent)
        {
            string time = String.Format(@"{0:.000} seconds", timeSpent.TotalSeconds);
            if (timeSpent.Days > 0)
                time = "That took a while " + timeSpent.ToString("g");
            
            if (timeSpent.Hours > 0)
                time = "Patience is a virtue. " + timeSpent.ToString("g");

            if (timeSpent.Minutes > 0)
                time = String.Format(@"{0:.00} minutes ", timeSpent.TotalMinutes);

 	        StatusBarText = String.Format("Search Complete. {0} item(s) found. Search time: {1}", 
                _visibleProperties.Count, time);
            System.Media.SystemSounds.Beep.Play();
        } 
        
      
      
    }
}
