using Moq;
using MyCompany.VariableExplorer.Model;
using MyCompany.VariableExplorer.Model.Services;
using MyCompany.VariableExplorer.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    public class ExpressionEvaluatorViewModelTest
    {
        [Test]
        public void EvaluateExpressionCommand_sets_property()
        {            
            // Arrange
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>();            
            var debugPropertMock = new Mock<IDebugProperty>();            
            debugPropertMock.Setup(s=>s.Children).Returns(
                () => new [] { Mock.Of<IPropertyInfo>() } );

            
            debugPropertMock.Setup(s => s.PropertyInfo).Returns(Mock.Of<IPropertyInfo>());
            
            expressionEvaluatorMock.Setup(s=>s.EvaluateExpression("a")).Returns( debugPropertMock.Object);            
            var expressionEvaluatorProviderMock  = new Mock<IExpressionEvaluatorProvider>();
            expressionEvaluatorProviderMock.Setup(e => e.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object);
            expressionEvaluatorProviderMock.Setup(e => e.IsEvaluatorAvailable).Returns(true);

            IocContainer.RegisterInstance<IExpressionEvaluatorProvider>(expressionEvaluatorProviderMock.Object);

            ExpressionEvaluatorViewModel objectUnderTest = new ExpressionEvaluatorViewModel();
            objectUnderTest.ExpressionText = "a";
            // Act
            objectUnderTest.EvaluateExpressionCommand.Execute(null);

            // Assert
            Assert.AreEqual(2, objectUnderTest.Properties.Count());
         
            //objectUnderTest.Properties

        }
    }
}
