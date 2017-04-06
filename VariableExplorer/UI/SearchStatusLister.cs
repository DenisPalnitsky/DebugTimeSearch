using SearchLocals.Model.ExpressioEvaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.UI
{
    internal class SearchStatusLister : ISearchStatus
    {
        public void Report(string text)
        {
            Task.Run( ()=> StatusUpdated(text) );
        }
        
        public Action<string> StatusUpdated { get; set; }
    }
}
