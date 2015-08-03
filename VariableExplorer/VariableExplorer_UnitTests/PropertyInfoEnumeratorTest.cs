using Moq;
using MyCompany.VariableExplorer.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    class PropertyInfoVisitorTest
    {
        IUnityContainer _container = new Microsoft.Practices.Unity.UnityContainer();


        private class SequentialTaskFactory : ITaskFactory
        {
            public void StartNew(Action action)
            {
                action();
            }
        }     

        [SetUp]
        public void SetupContainer()
        {
            _container.RegisterInstance<ITaskFactory>( new SequentialTaskFactory());
            _container.RegisterInstance<IPropertyVisitor>(Mock.Of<IPropertyVisitor>());

            _container.RegisterInstance<IExpressionEvaluator>(Mock.Of<IExpressionEvaluator>());

            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>();
            exparessionEvaluatorProviderMock.Setup(p => p.IsEvaluatorAvailable).Returns(true).Verifiable();
            exparessionEvaluatorProviderMock.Setup(p => p.ExpressionEvaluator).Returns( _container.Resolve<IExpressionEvaluator>()).Verifiable();
            _container.RegisterInstance<IExpressionEvaluatorProvider>(exparessionEvaluatorProviderMock.Object);

            _container.RegisterType<PropertyIterator>();
            
        }

        [Test]
        public void Enumerate_when_called_goes_through_all_child_properties_and_return_value_properties()
        {
            // Arrange

            // we setup here this object
            //   -Parent 
            //   -- ExpandableProperty 
            //   -- ValueProperty1
                        
            var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();                                    
            string expandablePropertyFullName = "ExpandableProperty";            
            expandablePropertyMock.Setup(p => p.FullName).Returns(expandablePropertyFullName).Verifiable();
            expandablePropertyMock.Setup(p => p.Name).Returns(expandablePropertyFullName).Verifiable();

            var parentPropertyMock = new Mock<IExpandablePropertyInfo>();            
            parentPropertyMock.Setup(p => p.FullName).Returns("Parent").Verifiable();
            parentPropertyMock.Setup(p => p.Name).Returns("Parent").Verifiable();
            
            var valuePropertyInfoFromExpandable = new Moq.Mock<IValuePropertyInfo>();
            valuePropertyInfoFromExpandable.Setup(p => p.Name).Returns("ValueProp");                      

            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>();
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>();
            var parentDebugPropertyMock = new Mock<IDebugProperty>();
            parentDebugPropertyMock.Setup(d => d.Children).Returns(new List<IPropertyInfo>
                    { 
                        expandablePropertyMock.Object,
                        valuePropertyInfoFromExpandable.Object 
                    }
                ).Verifiable();
            parentDebugPropertyMock.Setup(d => d.PropertyInfo).Returns(parentPropertyMock.Object).Verifiable();


            var expandableDebugPropertyMock = new Mock<IDebugProperty>();
            expandableDebugPropertyMock.Setup(d => d.Children).Returns(new List<IPropertyInfo>
                    {  }
                ).Verifiable();
            expandableDebugPropertyMock.Setup(d => d.PropertyInfo).Returns(expandablePropertyMock.Object).Verifiable();

            expressionEvaluatorMock.Setup(e => e.EvaluateExpression(expandablePropertyFullName)).Returns(expandableDebugPropertyMock.Object).Verifiable();
            
            exparessionEvaluatorProviderMock.Setup(p => p.IsEvaluatorAvailable).Returns(true).Verifiable();
            exparessionEvaluatorProviderMock.Setup(p => p.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object).Verifiable();

            Mock<IPropertyVisitor> propertyVisitorMock = new Mock<IPropertyVisitor>(  MockBehavior.Strict);
            propertyVisitorMock.Setup(v => v.ParentPropertyAttended(It.Is<IExpandablePropertyInfo>(e => e.Name == "Parent"))).Verifiable();
            propertyVisitorMock.Setup(v=>v.ParentPropertyAttended(It.Is<IExpandablePropertyInfo>(e=>e.Name == expandablePropertyFullName )) ).Verifiable();
            propertyVisitorMock.Setup(v=>v.ValuePropertyAttended(It.Is<IValuePropertyInfo>(e => e.Name == "ValueProp"))).Verifiable();            


            // Act
            List<IValuePropertyInfo> results = new List<IValuePropertyInfo>();
            PropertyIterator propertIterator = new PropertyIterator(exparessionEvaluatorProviderMock.Object,
                propertyVisitorMock.Object, new SequentialTaskFactory()   );

            propertIterator.TraversalOfPropertyTreeDeepFirst(parentDebugPropertyMock.Object);
         
            // Assert
            exparessionEvaluatorProviderMock.VerifyAll();
            expressionEvaluatorMock.VerifyAll();
            parentDebugPropertyMock.VerifyAll();
            expandablePropertyMock.VerifyAll();  
        }

        [Test]
        public void Enumerate_when_called_for_three_level_return_value_properties()
        {
            // Arrange

            // we setup here this object
            //   -Parent 
            //   -- ExpandableProperty 
            //      --- ValueProperty1

            var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();
            string expandablePropertyFullName = "ExpandableProperty";
            expandablePropertyMock.Setup(p => p.FullName).Returns(expandablePropertyFullName).Verifiable();
            expandablePropertyMock.Setup(p => p.Name).Returns(expandablePropertyFullName).Verifiable();

            var parentPropertyMock = new Mock<IExpandablePropertyInfo>();
            parentPropertyMock.Setup(p => p.FullName).Returns("Parent").Verifiable();
            parentPropertyMock.Setup(p => p.Name).Returns("Parent").Verifiable();

            var valuePropertyInfoFromExpandable = new Moq.Mock<IValuePropertyInfo>();
            valuePropertyInfoFromExpandable.Setup(p => p.Name).Returns("ValueProp");

            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>();
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>();
            var parentDebugPropertyMock = new Mock<IDebugProperty>();
            parentDebugPropertyMock.Setup(d => d.Children).Returns(new List<IPropertyInfo>
                    { 
                        expandablePropertyMock.Object,             
                    }
                ).Verifiable();
            parentDebugPropertyMock.Setup(d => d.PropertyInfo).Returns(parentPropertyMock.Object).Verifiable();


            var expandableDebugPropertyMock = new Mock<IDebugProperty>();
            expandableDebugPropertyMock.Setup(d => d.Children).Returns(new List<IPropertyInfo> {
                       valuePropertyInfoFromExpandable.Object 
            }
                ).Verifiable();
            expandableDebugPropertyMock.Setup(d => d.PropertyInfo).Returns(expandablePropertyMock.Object).Verifiable();

            expressionEvaluatorMock.Setup(e => e.EvaluateExpression(expandablePropertyFullName)).Returns(expandableDebugPropertyMock.Object).Verifiable();

            exparessionEvaluatorProviderMock.Setup(p => p.IsEvaluatorAvailable).Returns(true).Verifiable();
            exparessionEvaluatorProviderMock.Setup(p => p.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object).Verifiable();

            Mock<IPropertyVisitor> propertyVisitorMock = new Mock<IPropertyVisitor>(MockBehavior.Strict);
            propertyVisitorMock.Setup(v => v.ParentPropertyAttended(It.Is<IExpandablePropertyInfo>(e => e.Name == "Parent"))).Verifiable();
            propertyVisitorMock.Setup(v => v.ParentPropertyAttended(It.Is<IExpandablePropertyInfo>(e => e.Name == expandablePropertyFullName))).Verifiable();
            propertyVisitorMock.Setup(v => v.ValuePropertyAttended(It.Is<IValuePropertyInfo>(e => e.Name == "ValueProp"))).Verifiable();


            // Act
            List<IValuePropertyInfo> results = new List<IValuePropertyInfo>();
            PropertyIterator propertIterator = new PropertyIterator(exparessionEvaluatorProviderMock.Object,
                propertyVisitorMock.Object,
                 new SequentialTaskFactory());

            propertIterator.TraversalOfPropertyTreeDeepFirst(parentDebugPropertyMock.Object);

            // Assert
            exparessionEvaluatorProviderMock.VerifyAll();
            expressionEvaluatorMock.VerifyAll();
            parentDebugPropertyMock.VerifyAll();
            expandablePropertyMock.VerifyAll();
        }

        [Test]
        public void Enumerate_when_called_returns_property_value()
        {
            var parentValuePropertyInfo = new Mock<IValuePropertyInfo>();
            parentValuePropertyInfo.Setup(p => p.Value).Returns("1").Verifiable();
            
            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>( MockBehavior.Strict);
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>(MockBehavior.Strict);
            var debugPropertyMock = new Mock<IDebugProperty>();
            debugPropertyMock.Setup(d => d.PropertyInfo).Returns(parentValuePropertyInfo.Object).Verifiable();

            List<IPropertyInfo> results = new List<IPropertyInfo>();
            PropertyIterator propertyIterator = new PropertyIterator( 
                exparessionEvaluatorProviderMock.Object, 
                PropertyIterator.CreateActionBasedVisitor(
                 p=>results.Add(p), 
                 v=>results.Add(v)),
                 new SequentialTaskFactory() );

           propertyIterator.TraversalOfPropertyTreeDeepFirst( debugPropertyMock.Object);            

            exparessionEvaluatorProviderMock.Verify();
            expressionEvaluatorMock.VerifyAll();
            debugPropertyMock.VerifyAll();

            Assert.AreEqual(1, results.Count);
            Assert.That(results.Any(p => (p as IValuePropertyInfo) != null && (p as IValuePropertyInfo).Value == "1"));            
        }


        [Test]
        public void Enumerate_when_called_do_not_evaluates_properties_in_brackets()
        {
            // Arrange
            var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();

            string expandablePropertyFullName = "ExpandableProperty";            
            expandablePropertyMock.Setup(p => p.Name).Returns("[" + expandablePropertyFullName + "]" ).Verifiable();            
            
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>(MockBehavior.Strict);
            expressionEvaluatorMock.Setup(s=>s.EvaluateExpression(It.IsAny<string>()));

            var debugPropertyMock = new Mock<IDebugProperty>();
            debugPropertyMock.Setup(d => d.Children).Returns(new[] { expandablePropertyMock.Object});

            ObservableCollection<IPropertyInfo> result = new ObservableCollection<IPropertyInfo>();
          
            List<IPropertyInfo> results = new List<IPropertyInfo>();

            PropertyIterator propertyIterator = _container.Resolve<PropertyIterator>(
                new DependencyOverride<IExpressionEvaluator>(expressionEvaluatorMock.Object));

            // Act 
            propertyIterator.TraversalOfPropertyTreeDeepFirst(debugPropertyMock.Object);            
                        
            // Assert
            expressionEvaluatorMock.Verify(s=>s.EvaluateExpression(It.IsAny<string>()), Times.Never());
        }

        //[Test]
        //public void Enumerate_when_called_do_not_evaluates_with_same_names()
        //{
        //    var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();

        //    string expandablePropertyFullName = "ExpandableProperty";
        //    expandablePropertyMock.Setup(p => p.FullName).Returns(expandablePropertyFullName ).Verifiable();
        //    expandablePropertyMock.Setup(p => p.Name).Returns(expandablePropertyFullName).Verifiable();                   
            
        //    var debugPropertyMock = new Mock<IDebugProperty>();
        //    debugPropertyMock.Setup(d => d.Children).Returns(new[] { expandablePropertyMock.Object });

        //    // circular reference
        //    var expressionEvaluatorMock = new Mock<IExpressionEvaluator>(MockBehavior.Strict);                                    
        //    expressionEvaluatorMock.Setup(s => s.EvaluateExpression(expandablePropertyFullName)).Returns(debugPropertyMock.Object);
            

        //    ObservableCollection<IPropertyInfo> result = new ObservableCollection<IPropertyInfo>();

        //    List<IPropertyInfo> results = new List<IPropertyInfo>();

        //    PropertyIterator propertyIterator = _container.Resolve<PropertyIterator>(new DependencyOverride<IExpressionEvaluator>(expressionEvaluatorMock.Object));

        //    propertyIterator.TraversalOfPropertyTreeDeepFirst(debugPropertyMock.Object);
            
        //    expressionEvaluatorMock.Verify(s => s.EvaluateExpression(expandablePropertyFullName), Times.Once);                        
        //}
 
         
    }
}
