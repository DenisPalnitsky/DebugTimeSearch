using System;

namespace SearchLocals.Model.ExpressioEvaluation
{
    internal interface ISearchStatus
    {
        void Report(string text);

        Action<string> StatusUpdated { get; set; }
    }
    
}