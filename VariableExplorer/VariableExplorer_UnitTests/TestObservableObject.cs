
using SearchLocals.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    class TestObservableObject
    {
        class DummyImplementation : ObservableObject
        {
            public object Property
            {
                get { throw new NotImplementedException(); }
                set { OnPropertyChanged(); }
            }
        }

        [Test]
        public void TestPropertyChanged()
        {
            bool wasDelegatecalled = false;
            DummyImplementation objectUnderTest = new DummyImplementation();
            objectUnderTest.PropertyChanged += delegate(object o, PropertyChangedEventArgs e)
            {
                Assert.AreEqual("Property", e.PropertyName);
                wasDelegatecalled = true;
            };
            objectUnderTest.Property = new object();

            Assert.IsTrue(wasDelegatecalled);
        }
    
    }
}
