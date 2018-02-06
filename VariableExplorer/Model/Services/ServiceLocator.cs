using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLocals.Model.Services;
using SearchLocals.UI;
using SearchLocals.Model.ExpressioEvaluation;

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

            RegisterInstance<ExpressioEvaluation.ISearchStatus>(new SearchStatusLister());
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
