using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    /// <summary>
    ///  Used by Property enumerator to add parallelist 
    /// </summary>
    public class TaskFactory 
    {
     

        private class SequentialTaskFactory : ITaskFactory
        {            
            public void StartNew(Action action)
            {
                action();
            }
        }


        public static ITaskFactory CreateParalleTaskFactory() 
        {
            return new ParallelTaskFactory();
        }

        public static ITaskFactory CreateSequentialTaskFactory()
        {
            return new ParallelTaskFactory();
        }
    }
}
