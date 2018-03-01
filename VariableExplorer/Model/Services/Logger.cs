namespace SearchLocals.Model.Services
{
    internal class Logger
    {
        static ILog _log = new OutputWindowLogger();       

        public static ILog GetLogger()
        {
            return _log;
        }
    }
}
