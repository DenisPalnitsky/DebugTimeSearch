using Moq;
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
    class PropertyInfoEnumeratorTest
    {
        [Test]
        public void Enumerate_when_called_goes_through_all_child_properties_and_return_value_properties()
        {
            var valuePropertyInfo1 = new Mock<IValuePropertyInfo>();
            valuePropertyInfo1.Setup(p => p.Value).Returns("1").Verifiable();

            var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();
            
            string expandablePropertyFullName = "ExpandableProperty";
            expandablePropertyMock.Setup(p => p.FullName).Returns(expandablePropertyFullName).Verifiable();
            
            var valuePropertyInfo2 = new Moq.Mock<IValuePropertyInfo>();
            valuePropertyInfo2.Setup(p => p.Value).Returns("2");

            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>();
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>();
            var debugPropertyMock = new Mock<IDebugProperty>();
            debugPropertyMock.Setup(d => d.Children).Returns(new[] { valuePropertyInfo2.Object }).Verifiable();
            expressionEvaluatorMock.Setup(e => e.EvaluateExpression(expandablePropertyFullName)).Returns(debugPropertyMock.Object).Verifiable();
            exparessionEvaluatorProviderMock.Setup(p => p.IsEvaluatorAvailable).Returns(true).Verifiable();
            exparessionEvaluatorProviderMock.Setup(p => p.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object).Verifiable();


            List<IValuePropertyInfo> results = new List<IValuePropertyInfo>();
            foreach (var property in PropertyInfoEnumerator.Enumerate(
                                                 new IPropertyInfo[] { valuePropertyInfo1.Object, expandablePropertyMock.Object },
                                                 exparessionEvaluatorProviderMock.Object))
            {
                results.Add(property);
            }
            
            exparessionEvaluatorProviderMock.VerifyAll();
            expressionEvaluatorMock.VerifyAll();
            debugPropertyMock.VerifyAll();
            expandablePropertyMock.VerifyAll();

            Assert.AreEqual(2, results.Count);
            Assert.That(results.Any(p => p.Value == "1"));
            Assert.That(results.Any(p => p.Value == "2"));
        }
    }
}
