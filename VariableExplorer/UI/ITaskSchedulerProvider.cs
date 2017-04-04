using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.UI
{
    interface ITaskSchedulerProvider
    {
        TaskScheduler GetCurrentScheduler();
    }

    class TaskSchedulerProvider :ITaskSchedulerProvider
    {

        public TaskScheduler GetCurrentScheduler()
        {
            return TaskScheduler.FromCurrentSynchronizationContext();
        }
    }

    
}
