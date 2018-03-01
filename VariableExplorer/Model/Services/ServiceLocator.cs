using Microsoft.Practices.Unity;
using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.UI;

namespace SearchLocals.Model.Services
{
    class ServiceLocator
    {
        static IUnityContainer _container = new UnityContainer();
        
        static ServiceLocator()
        {            
            DefaultRegistration();
        }

        public static void Register<TInterface, TImplementer>()
            where TInterface : class
            where TImplementer : TInterface 
        {
            _container.RegisterType<TInterface, TImplementer>();
        }
      

        public static void DefaultRegistration ()
        {
            Register<IConfiguration, Configuration>();
            Register<ILog, OutputWindowLogger>();

            var ev = new VSEnvironmentEvents.VSEnvironmentEventsPublisher();
            RegisterInstance<IVSEnvironmentEventsPublisher>(ev);
            RegisterInstance<IVsEnvironmentEvents>(ev);

            _container.RegisterInstance<ExpressioEvaluation.ISearchStatus>(new SearchStatusLister(_container.Resolve<ILog>()));            
            RegisterInstance<IExpressionEvaluatorViewModel>(new  ExpressionEvaluatorViewModel());
            RegisterInstance<ITaskSchedulerProvider>(new TaskSchedulerProvider());

#if DEBUG
            _container.RegisterType<IExpressionsCache, DisabledExpressionsCache>();
#else
            _container.RegisterType<IExpressionsCache, ExpressionsCache>();
#endif
            
        }


        public static T Resolve<T> ()
        {
            return _container.Resolve<T>();
        }


        internal static void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance<T>(instance);            
        }
    }
}
