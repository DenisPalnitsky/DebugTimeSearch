using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.Model
{
    interface IVsEnvironmentEvents
    {
        event EventHandler EvaluatorBecomeAvailable;
        event EventHandler EvaluatorBecomeUnAvailable;
    }
}
