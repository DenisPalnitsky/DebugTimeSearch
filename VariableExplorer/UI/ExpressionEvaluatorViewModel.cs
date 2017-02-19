using MyCompany.VariableExplorer.Model;
using MyCompany.VariableExplorer.Model.ExpressioEvaluation;
using MyCompany.VariableExplorer.Model.Services;
using MyCompany.VariableExplorer.Model.VSPropertyModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyCompany.VariableExplorer.UI
{

    class ExpressionEvaluatorViewModel : ObservableObject, MyCompany.VariableExplorer.UI.IExpressionEvaluatorViewModel
    {
        IDebugProperty _property;
        string _filterText;
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

        public string FilterText
        {
            get { return _filterText; }
            set 
            { 
                _filterText = value;                
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
            
        
        public ICommand  SearchLocalsCommand
        {
            get { return new DelegateCommand(Search); }
        }
     

        private void Search()
        {
            var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();
            if (expressionEvaluatorProvider.IsEvaluatorAvailable)
            {
                if (String.IsNullOrEmpty(FilterText)) // search all locals
                {
                    _property = expressionEvaluatorProvider.ExpressionEvaluator.GetLocals();
                    IterateThrueProperty(expressionEvaluatorProvider);
                }
                else
                {
                    _property = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(FilterText);
                    IterateThrueProperty(expressionEvaluatorProvider);
                }
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
                IPropertyVisitor propertyVisitor = new ActionBasedPropertyVisitor(
                    expandableProperty =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _visibleProperties.AddRange(expandableProperty.Select(item => DebugPropertyViewModel.From(item)));
                                });
                                _logger.Info(String.Join("\n", expandableProperty));
                            },
                            valueProperty =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _visibleProperties.AddRange(valueProperty.Select(item => DebugPropertyViewModel.From(item)));
                                });
                                _logger.Info(String.Join("\n", valueProperty));
                            });

                PropertyIterator propertyIterator = new PropertyIterator(
                    expressionEvaluatorProvider,
                    propertyVisitor);

                Task.Run(
                    () => propertyIterator.TraversPropertyTree(_property, _searchText))
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

        private void ExpandableProperyVisited (IExpandablePropertyInfo prop)
        {

        }

        private void visibleProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
