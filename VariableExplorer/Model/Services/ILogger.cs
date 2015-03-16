using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model.Services
{
    interface ILogger
    {
        void Info(string message, params object[] parameters);
        
    }
}
