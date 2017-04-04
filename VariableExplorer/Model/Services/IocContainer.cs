using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLocals.Model.Services;
using SearchLocals.UI;

namespace SearchLocals.Model.Services
{
    class IocContainer
    {
        static IUnityContainer _container = new UnityContainer();
        
        static IocContainer()
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
            RegisterInstance<IExpressionEvaluatorViewModel>(new  ExpressionEvaluatorViewModel(Resolve<ILog>()));
            RegisterInstance<ITaskSchedulerProvider>(new TaskSchedulerProvider());            
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
