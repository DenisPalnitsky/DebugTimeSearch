using MyCompany.VariableExplorer.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.UI
{
    public class RedirectLogger : ILogger
    {
        Action<string> _logAction;
        public RedirectLogger(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void Info(string message, params object[] parameters)
        {
            _logAction(String.Format(message, parameters) + Environment.NewLine);
        }
    }
    
}
