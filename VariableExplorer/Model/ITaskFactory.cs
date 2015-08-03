using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model
{
    public interface ITaskFactory
    {
        void StartNew(Action action);
    }
}
