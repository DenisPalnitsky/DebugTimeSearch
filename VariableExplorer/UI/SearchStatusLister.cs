using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.UI
{
    internal class SearchStatusLister : ISearchStatus
    {
        ILog _logger = Logger.GetLogger();
        
        public void Report(string text)
        {
            _logger.Info("Search status update. " + text);
            Task.Run( () => StatusUpdated(text) );
        }
        
        public Action<string> StatusUpdated { get; set; }
    }
}
