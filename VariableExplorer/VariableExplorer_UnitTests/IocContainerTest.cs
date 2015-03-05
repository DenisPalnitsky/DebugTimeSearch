using Castle.MicroKernel;
using Castle.Windsor;
using Microsoft.Practices.Unity;
using MyCompany.VariableExplorer.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    public class IocContainerTest
    {
        interface IFoo
        { }

        class Foo : IFoo { }

        class EmptyFoo : IFoo { }
      
    

        [Test]
        public void Resolve_after_UnRegisterInstance_returns_null2()
        {
            var f = new Foo();
            WindsorContainer c = new WindsorContainer();
            
            c.Register(Castle.MicroKernel.Registration.Component.For<IFoo>().Instance(f));

            Assert.AreEqual(c.Resolve<IFoo>(), c.Resolve<IFoo>());

            //c.Register(Castle.MicroKernel.Registration.Component.For<IFoo>());

            Assert.AreNotEqual(f, c.Resolve<IFoo>());
        }

        [Test]
        public void TestUnity()
        {
            var f = new Foo();
            
              
            UnityContainer c = new UnityContainer();
            var lifetimeManager = new TransientLifetimeManager();
            c.RegisterInstance<IFoo>(f, lifetimeManager);
                        
            Assert.AreEqual(c.Resolve<IFoo>(), c.Resolve<IFoo>());

            foreach (var registration in c.Registrations.Where(p => p.RegisteredType == typeof(object) && p.LifetimeManagerType == typeof(ContainerControlledLifetimeManager)))
            {
                registration.LifetimeManager.RemoveValue();
            }

            Assert.AreNotEqual(f, c.Resolve<IFoo>());
            Assert.IsNull (c.Resolve<IFoo>());
        }
    }
}
