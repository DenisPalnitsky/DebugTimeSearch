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
            _container.Register(Component.For<IConfiguration>().ImplementedBy<Configuration>());            
        }
        
        public static T Resolve<T> ()
        {
            return _container.Resolve<T>();
        }        

    }
}
