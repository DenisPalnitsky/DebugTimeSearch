using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class IocContainer
    {
        static WindsorContainer _container = new WindsorContainer();

        static IocContainer()
        {
            _container = new WindsorContainer();
            DefaultRegistration();
        }

        public static void Register<TInterface, TImplementer>()
            where TInterface : class
            where TImplementer : TInterface 
        {
            _container.Register(Component.For<TInterface>().ImplementedBy<TImplementer>().IsDefault());
        }

        public static void RegisterInstance<TInterface>(TInterface implementation)
            where TInterface : class            
        {
            _container.Register(Component.For<TInterface>().Instance(implementation).IsDefault());
        }

        public static void UnRegisterInstance<TInterface>()
            where TInterface : class
        {
            var implementer = _container.Resolve<TInterface>();            
            if (implementer is IDisposable)
                ((IDisposable)implementer).Dispose();
            
            _container.Release(implementer);
        }


        public static void DefaultRegistration ()
        {
            Register<IConfiguration, Configuration>();
        }

        
        public static T Resolve<T> ()
        {
            return _container.Resolve<T>();
        }        

    }
}
