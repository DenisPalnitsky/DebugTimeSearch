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


        class TestableScheduler : TaskScheduler
        {
            private Queue<Task> m_taskQueue = new Queue<Task>();

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return m_taskQueue;
            }

            protected override void QueueTask(Task task)
            {
                m_taskQueue.Enqueue(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                task.RunSynchronously();
                return true;
            }

            public void RunAll()
            {
                while (m_taskQueue.Count > 0)
                {
                    m_taskQueue.Dequeue().RunSynchronously();
                }
            }
        }

        [Test]
        [Ignore]
        public void EvaluateExpressionCommand_sets_property()
        {

            var taskSchedulerProviderMock = new Moq.Mock<ITaskSchedulerProvider>();
            taskSchedulerProviderMock.Setup(s => s.GetCurrentScheduler()).Returns(new TestableScheduler());
            IocContainer.RegisterInstance<ITaskSchedulerProvider>(taskSchedulerProviderMock.Object);


            // Arrange

            var valueProperty = new Mock<IValuePropertyInfo>();
            valueProperty.Setup(s => s.FullName).Returns("FullName");
            valueProperty.Setup(s => s.Name).Returns("Name");

            var expressionEvaluatorMock = new Mock<IExpressionEvaluator>();            
            var debugPropertMock = new Mock<IDebugProperty>();            
            debugPropertMock.Setup(s=>s.Children).Returns(
                () => new[] { valueProperty.Object });


            debugPropertMock.Setup(s => s.PropertyInfo).Returns(valueProperty.Object);
            
            expressionEvaluatorMock.Setup(s=>s.EvaluateExpression("a")).Returns( debugPropertMock.Object);            
            var expressionEvaluatorProviderMock  = new Mock<IExpressionEvaluatorProvider>();
            expressionEvaluatorProviderMock.Setup(e => e.ExpressionEvaluator).Returns(expressionEvaluatorMock.Object);
            expressionEvaluatorProviderMock.Setup(e => e.IsEvaluatorAvailable).Returns(true);

            IocContainer.RegisterInstance<IExpressionEvaluatorProvider>(expressionEvaluatorProviderMock.Object);

            ExpressionEvaluatorViewModel objectUnderTest = new ExpressionEvaluatorViewModel(Mock.Of<ILog>() );
            objectUnderTest.ExpressionText = "a";
            // Act
            objectUnderTest.EvaluateExpressionCommand.Execute(null);

            // Assert
            Assert.AreEqual(2, objectUnderTest.Properties.Count());
         
            //objectUnderTest.Properties

        }
    }
}
