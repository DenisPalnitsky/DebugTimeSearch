using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    public class ParallelTaskFactory : ITaskFactory
    {
        System.Threading.Tasks.TaskFactory _taskFactory = new System.Threading.Tasks.TaskFactory(
            TaskCreationOptions.AttachedToParent,
            TaskContinuationOptions.AttachedToParent);

        Task _parentTask;
        object syncRoot = new object();

        public Task ParentTask { get { return _parentTask; } }


        public void StartNew(Action action)
        {
            lock (syncRoot)
            {
                var task = _taskFactory.StartNew(action);
                if (_parentTask == null)
                    _parentTask = task;
            }
        }


    }
}
