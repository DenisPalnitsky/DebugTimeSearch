using System;
using System.IO;

namespace MyCompany.VariableExplorer.Model.Services
{
    class OutputWindowLogger : ILog
    {        
        public OutputWindowLogger()
        {            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error("Exception: " + e.ExceptionObject.ToString());
        }

        public void Info(string message, params object[] parameters)
        {
            Error(message, parameters);
        }

        public void Error(string message, params object[] parameters)
        {
            var str = string.Format(DateTime.Now.ToString("hh:mm:ss.fff") + " " + message, parameters);
            System.Diagnostics.Debug.WriteLine(str);            
        }

        public void Dispose()
        {
        }
    }
}
