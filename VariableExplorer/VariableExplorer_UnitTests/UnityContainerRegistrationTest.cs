using Microsoft.Practices.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLocals.Model.Services;
using SearchLocals.UI;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    class UnityContainerRegistrationTest
    {
        [Test]
        public void Registration()
        {
            UnityContainer container = new UnityContainer();
            container.RegisteDefaultTypes();

            var vm = container.Resolve<IExpressionEvaluatorViewModel>();
        }
    }
}
