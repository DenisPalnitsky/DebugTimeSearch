using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model.Services
{
    class OutputWindowLogger : ILog
    {
        public void Info(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(message, parameters);
        }


        public void Error(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(message, parameters);
        }
    }
}
