using Moq;
using MyCompany.VariableExplorer.Model;
using MyCompany.VariableExplorer.Model.ExpressioEvaluation;
using MyCompany.VariableExplorer.Model.VSPropertyModel;
using MyCompany.VariableExplorer.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    public class LoadTestPropertyInfoEnumerator
    {
        internal class MockDebugPropertyInfo : IDebugProperty
        {
            public int valuePropertiesCount = 1000000;
            public int expandablePropertiesCount = 1000;

            public IEnumerable<IPropertyInfo> Children
            {
                get 
                {
                    var valueProperties = new List<ValuePropertyInfo>();

                    for (int i = 0; i < valuePropertiesCount; i++)
                        valueProperties.Add(new ValuePropertyInfo("Prop " + i, "Name" + i, "Type" + i));

                    var expandableProperties = new List<IExpandablePropertyInfo>();
                    for (int i = 0; i < expandablePropertiesCount; i++)
                        expandableProperties.Add(
                            Mock.Of<IExpandablePropertyInfo>( ));

                    return valueProperties;
                }
            }

            public IPropertyInfo PropertyInfo
            {
                get { return new ValuePropertyInfo("Root", "Root name", "string"); }
            }
        }

        [Test]
        [Ignore]
        public void Test()
        {
            var sw = Stopwatch.StartNew();
            var propInfo = new MockDebugPropertyInfo();
            Mock<IExpressionEvaluatorProvider> evaluatorProvider = new Mock<IExpressionEvaluatorProvider>();
            Mock<IExpressionEvaluator> evaluator = new Mock<IExpressionEvaluator>();
            evaluator.Setup(e => e.EvaluateExpression(It.IsAny<string>())).Returns(Mock.Of < IDebugProperty>());

            evaluatorProvider.Setup(e => e.IsEvaluatorAvailable).Returns(true);
            evaluatorProvider.Setup(e => e.ExpressionEvaluator).Returns(evaluator.Object);

            var eventSink = new ActionBasedPropertyVisitor(e => {

                System.Diagnostics.Debug.WriteLine( "ExpandableProps released:" + e.Count() ); 

            }, v => {
                System.Diagnostics.Debug.WriteLine("ValueProps released:" + v.Count()); 
            });

            PropertyIterator propertyIterator = new PropertyIterator(evaluatorProvider.Object, eventSink);

            propertyIterator.TraversPropertyTree(propInfo, String.Empty);
                        
            sw.Stop();
            var filename = "LoadTestResults.txt";
            File.AppendAllText(filename,
                String.Format("{3}: Values:{0}, Types:{1}, Time {2} " + Environment.NewLine,
                propInfo.valuePropertiesCount,
                propInfo.expandablePropertiesCount,
                sw.ElapsedMilliseconds,
                DateTime.Now.ToString("yyyy-MM-dd hh:mm")));

            System.Diagnostics.Process.Start(filename);
        }


    }
}
