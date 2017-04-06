using SearchLocals.Model;
using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.Model.Services;
using SearchLocals.Model.VSPropertyModel;
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
        IDebugProperty _property;
        string _filterText;
        DebugPropertyViewModelCollection _visibleProperties = new DebugPropertyViewModelCollection();
        object _visiblePropertiesLock = new object();

        ILog _logger = IocContainer.Resolve<ILog>();
        ISearchStatus _searchStatus = IocContainer.Resolve<ISearchStatus>();

        private string _errorMessage;
        private string _statusBarText;
        private string _searchText;
        string _searchingReportText;

        public ExpressionEvaluatorViewModel()
        {            
            _visibleProperties.CollectionChanged += visibleProperties_CollectionChanged;
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_visibleProperties, _visiblePropertiesLock);
            _searchStatus.StatusUpdated = (s) => SearchingReportText = s;
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
                        var expressionEvaluatorProvider = IocContainer.Resolve<IExpressionEvaluatorProvider>();
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
