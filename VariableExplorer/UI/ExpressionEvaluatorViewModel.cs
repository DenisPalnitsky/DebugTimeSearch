using SearchLocals.Model;
using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.Model.Services;
using SearchLocals.Model.VSPropertyModel;
using SearchLocals.UI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SearchLocals.UI
{

    class ExpressionEvaluatorViewModel : ObservableObject, SearchLocals.UI.IExpressionEvaluatorViewModel, IDataErrorInfo
    {
        IVSDebugPropertyProxy _property;
        string _filterText;
        DebugPropertyViewModelCollection _visibleProperties = new DebugPropertyViewModelCollection();
        object _visiblePropertiesLock = new object();
        ILog _logger = Logger.GetLogger();
        ISearchStatus _searchStatus;
        MutableDelegateCommand _cancelSearch = new MutableDelegateCommand();
        IExpressionEvaluatorProvider _expressionEvaluatorProvider;
        ITaskSchedulerProvider _taskSchedulerProvider;
        private string _errorMessage;
        private string _statusBarText;
        private string _searchText;
        private string _searchingReportText;
        private bool _isSearchInProgress = false;
        private bool _isEnabled;

        public ExpressionEvaluatorViewModel(IVsEnvironmentEvents vsEvents, 
            ISearchStatus searchStatus, 
            IExpressionEvaluatorProvider expressionEvaluatorProvider,
            ITaskSchedulerProvider taskSchedulerProvider)
        {            
            _visibleProperties.CollectionChanged += visibleProperties_CollectionChanged;
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_visibleProperties, _visiblePropertiesLock);
            _searchStatus = searchStatus;
            _searchStatus.StatusUpdated = (s) => SearchingReportText = s;
            _expressionEvaluatorProvider = expressionEvaluatorProvider;
            _taskSchedulerProvider = taskSchedulerProvider;

            vsEvents.EvaluatorBecomeAvailable += (a, b) => { IsEnabled = true; };
            vsEvents.EvaluatorBecomeUnAvailable += VsEvents_EvaluatorBecomeUnAvailable;
        }

        private void VsEvents_EvaluatorBecomeUnAvailable(object sender, EventArgs e)
        {
            IsEnabled = false;
            _searchStatus.StatusUpdated(string.Empty);
            _visibleProperties.Clear();
            OnPropertyChanged(() => Properties);
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
        
        public string SearchingReportText
        {
            get { return _searchingReportText; }
            private set
            {
                _searchingReportText = value;
                OnPropertyChanged(() => SearchingReportText);
            }
        }


        public ICommand  SearchLocalsCommand
        {
            get { return new DelegateCommand(Search); }
        }        

        public ICommand CancelSearch
        {
            get { return _cancelSearch; }
        }      

        public bool IsSearchInProgress
        {
            get { return _isSearchInProgress; }
            private set
            {
                _isSearchInProgress = value;
                OnPropertyChanged(nameof(IsSearchInProgress));
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            private set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }                
            }
        }
        

        #region IDataErrorInfo 

        public string Error { get; private set; }
        

        public string this[string columnName]
        {
            get
            {
                string validationMessage = string.Empty;
                switch (columnName)
                {
                    case nameof(FilterText):
                        var expressionEvaluatorProvider = _expressionEvaluatorProvider;
                        if (string.IsNullOrEmpty(FilterText))
                            break;  

                        if (expressionEvaluatorProvider.IsEvaluatorAvailable)
                        {
                            var propertyInfo = expressionEvaluatorProvider.ExpressionEvaluator.EvaluateExpression(FilterText).PropertyInfo;
                            

                            if( !(propertyInfo is ExpandablePropertyInfo))
                                validationMessage = "Object does not exist or it's not expandable";
                        }
                        else
                        {
                            validationMessage = "Visual Studio is not in debug mode";
                        }
                        break;
                }

                return validationMessage;
            }
        }
        #endregion



        private void Search()
        {
            var expressionEvaluatorProvider = _expressionEvaluatorProvider;
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
                    propertyVisitor,
                    _searchStatus);

                _cancelSearch.Action = propertyIterator.Cancel;

                var searchTask = Task.Run(
                    () =>
                    {
                        IsSearchInProgress = true;
                        propertyIterator.TraversPropertyTree(_property, _searchText);
                    })                    
                    .ContinueWith(t =>
                    {
                        IsSearchInProgress = false;
                        stopwatch.Stop();
                        if (t.Exception != null)
                        {
                            if (t.Exception.InnerExceptions.First() is TaskCanceledException)
                            {
                                _logger.Info("Search canceled");
                                PostSearchCompleteMessage(stopwatch.Elapsed, true);
                            }
                            else // Error 
                            {
                                _logger.Error(t.Exception.ToString());
                                StatusBarText = "Error during evaluation. " + t.Exception.ToString();                                
                                throw t.Exception;
                            }
                        }
                        else
                        {
                            _logger.Info("Search finished");
                            PostSearchCompleteMessage(stopwatch.Elapsed, false);
                        }

                        _cancelSearch.Action = null;                        
                    },
                    _taskSchedulerProvider.GetCurrentScheduler());
            }
            else
            {
                _logger.Info("ExpressionEvaluator is not initialized");
                ErrorMessage = "ExpressionEvaluator is not initialized";
            }
        }

        private void ExpandableProperyVisited (IExpandablePropertyInfo prop)
        {
            // nothing to do 
        }

        private void visibleProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(() => Properties);
        }
      
        private void  PostSearchCompleteMessage(TimeSpan timeSpent, bool isCanceled)
        {
            string time = String.Format(@"{0:.000} seconds", timeSpent.TotalSeconds);
            if (timeSpent.Days > 0)
                time = "That took a while " + timeSpent.ToString("g");
            
            if (timeSpent.Hours > 0)
                time = "Patience is a virtue. " + timeSpent.ToString("g");

            if (timeSpent.Minutes > 0)
                time = String.Format(@"{0:.00} minutes ", timeSpent.TotalMinutes);

            string start = "Search completed.";
            if (isCanceled)
                start = "Search canceled.";

 	        StatusBarText = String.Format(start+ " {0} item(s) found. Search time: {1}", 
                _visibleProperties.Count, time);

            System.Media.SystemSounds.Beep.Play();
        } 
        
      
      
    }
}
