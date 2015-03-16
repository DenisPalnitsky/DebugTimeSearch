using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model.Services
{
    class OutputWindowLogger : ILogger
    {
        public void Info(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(message, parameters);
        }
    }
}
