using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model.Services
{
    interface ILog: IDisposable
    {
        void Info(string message, params object[] parameters);
        void Error(string message, params object[] parameters);        
    }
}
