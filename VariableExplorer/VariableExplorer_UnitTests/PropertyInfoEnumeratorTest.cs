using Moq;
using SearchLocals.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using SearchLocals.Model.VSPropertyModel;
using SearchLocals.Model.ExpressioEvaluation;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    public class PropertyInfoVisitorTest
    {
        IUnityContainer _container = new Microsoft.Practices.Unity.UnityContainer();

        #region Mocks 

        internal class PropertyVisitorMock : IPropertyVisitor
        {
            private Action<IExpandablePropertyInfo> _expandablePropertyAttended;
            private Action<IValuePropertyInfo> _valuePropertyAttended;

            public PropertyVisitorMock(Action<IExpandablePropertyInfo> expandablePropertyAttended,
                Action<IValuePropertyInfo> valuePropertyAttended)
            {
                _expandablePropertyAttended = expandablePropertyAttended;
                _valuePropertyAttended = valuePropertyAttended;
            }

            public virtual void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
            {
                _expandablePropertyAttended(expandablePropertyInfo);
            }

            public virtual void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
            {
                _valuePropertyAttended(valuePropertyInfo);
            }

            public void Dispose()
            {
                // no need to do anything
            }
        }

        #endregion

        [SetUp]
        public void SetupContainer()
        {            
            _container.RegisterInstance<IPropertyVisitor>(Mock.Of<IPropertyVisitor>());

            var iExpressionEvaluatorMock = new Mock<IExpressionEvaluator>( );
            iExpressionEvaluatorMock.Name = "Default IExpressionEvaluator";
            _container.RegisterInstance<IExpressionEvaluator>(iExpressionEvaluatorMock.Object);

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
            var exparessionEvaluator = CreateEvaluatorWithThreeLevelProperty();

            Mock<IPropertyVisitor> propertyVisitorMock = new Mock<IPropertyVisitor>(MockBehavior.Strict);
            propertyVisitorMock.Setup(v => v.ParentPropertyAttended(It.Is<IExpandablePropertyInfo>(e => e.FullName == "Parent" && e.Name == "Parent"))).Verifiable();
            propertyVisitorMock.Setup(v => v.ParentPropertyAttended(It.Is<IExpandablePropertyInfo>(e => e.FullName == "ExpandableProperty" && e.Name == "ExpandableProperty"))).Verifiable();
            propertyVisitorMock.Setup(v => v.ValuePropertyAttended(It.Is<IValuePropertyInfo>(e => e.FullName == "ValueProp" && e.Name == "ValueProp"))).Verifiable();
            propertyVisitorMock.Setup(v => v.Dispose());

            // Act            
            PropertyIterator propertIterator = new PropertyIterator(exparessionEvaluator,
                propertyVisitorMock.Object);

            propertIterator.TraversPropertyTree(exparessionEvaluator.ExpressionEvaluator.EvaluateExpression("Parent"), String.Empty);

            // Assert
            propertyVisitorMock.VerifyAll();
            
        }

        private static IExpressionEvaluatorProvider CreateEvaluatorWithThreeLevelProperty()
        {
            // we setup here object with structure below
            //   -Parent 
            //   -- ExpandableProperty 
            //      --- ValueProperty1

            var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();
            string expandablePropertyFullName = "ExpandableProperty";
            expandablePropertyMock.Setup(p => p.FullName).Returns(expandablePropertyFullName);
            expandablePropertyMock.Setup(p => p.Name).Returns(expandablePropertyFullName);

            var parentPropertyMock = new Mock<IExpandablePropertyInfo>();
            parentPropertyMock.Setup(p => p.FullName).Returns("Parent");
            parentPropertyMock.Setup(p => p.Name).Returns("Parent");

            var valuePropertyInfoFromExpandable = new Moq.Mock<IValuePropertyInfo>();
            valuePropertyInfoFromExpandable.Setup(p => p.Name).Returns("ValueProp");
            valuePropertyInfoFromExpandable.Setup(p => p.FullName).Returns("ValueProp");

            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>();
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>();
            var parentDebugPropertyMock = new Mock<IVSDebugPropertyProxy>();
            parentDebugPropertyMock.Setup(d => d.Children).Returns(new List<IPropertyInfo>
                    {
                        expandablePropertyMock.Object,
                    }
                );
            parentDebugPropertyMock.Setup(d => d.PropertyInfo).Returns(parentPropertyMock.Object);


            var expandableDebugPropertyMock = new Mock<IVSDebugPropertyProxy>();
            expandableDebugPropertyMock.Setup(d => d.Children).Returns(new List<IPropertyInfo> {
                       valuePropertyInfoFromExpandable.Object
            });
            expandableDebugPropertyMock.Setup(d => d.PropertyInfo).Returns(expandablePropertyMock.Object);

            expressionEvaluatorMock.Setup(e => e.EvaluateExpression(expandablePropertyFullName)).Returns(expandableDebugPropertyMock.Object);
            expressionEvaluatorMock.Setup(e => e.EvaluateExpression("Parent")).Returns(parentDebugPropertyMock.Object);


            exparessionEvaluatorProviderMock.Setup(p => p.IsEvaluatorAvailable).Returns(true);
            exparessionEvaluatorProviderMock.Setup(p => p.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object);
            var expressionEvaluatorProvider = exparessionEvaluatorProviderMock.Object;
            return expressionEvaluatorProvider;
        }

        [Test]
        public void Enumerate_when_called_returns_property_value()
        {
            var parentValuePropertyInfo = new Mock<IValuePropertyInfo>();
            parentValuePropertyInfo.Setup(p => p.Value).Returns("1").Verifiable();
            parentValuePropertyInfo.Setup(p => p.Name).Returns("Name");
            parentValuePropertyInfo.Setup(p => p.FullName).Returns("FullName");
            
            var exparessionEvaluatorProviderMock = new Mock<IExpressionEvaluatorProvider>( MockBehavior.Strict);
            exparessionEvaluatorProviderMock.Setup(e => e.IsEvaluatorAvailable).Returns(true);
            
            
            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>(MockBehavior.Strict);
            

            var debugPropertyMock = new Mock<IVSDebugPropertyProxy>();
            debugPropertyMock.Setup(d => d.PropertyInfo).Returns(parentValuePropertyInfo.Object).Verifiable();



            exparessionEvaluatorProviderMock.Setup(e => e.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object);
            expressionEvaluatorMock.Setup(e => e.EvaluateExpression("FullName")).Returns(debugPropertyMock.Object);
            
            List<IPropertyInfo> results = new List<IPropertyInfo>();
            PropertyIterator propertyIterator = new PropertyIterator( 
                exparessionEvaluatorProviderMock.Object, 
                new PropertyVisitorMock(                
                 p=>results.Add(p), 
                 v=>results.Add(v)));

            propertyIterator.TraversPropertyTree(debugPropertyMock.Object, String.Empty);            

            exparessionEvaluatorProviderMock.Verify();
            expressionEvaluatorMock.VerifyAll();
            debugPropertyMock.VerifyAll();

            Assert.AreEqual(1, results.Count);
            Assert.That(results.Any(p => (p as IValuePropertyInfo) != null && (p as IValuePropertyInfo).Value == "1"));            
        }




        //[Test]
        //public void Enumerate_when_called_do_not_evaluates_with_same_names()
        //{
        //    var expandablePropertyMock = new Mock<IExpandablePropertyInfo>();

        //    string expandablePropertyFullName = "ExpandableProperty";
        //    expandablePropertyMock.Setup(p => p.FullName).Returns(expandablePropertyFullName ).Verifiable();
        //    expandablePropertyMock.Setup(p => p.Name).Returns(expandablePropertyFullName).Verifiable();                   

        //    var debugPropertyMock = new Mock<IVSDebugPropertyWrapper>();
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
