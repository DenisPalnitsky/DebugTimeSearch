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
    }
}
