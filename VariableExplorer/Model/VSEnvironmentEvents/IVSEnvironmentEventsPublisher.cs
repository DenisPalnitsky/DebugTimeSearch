using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.Model
{
    interface IVSEnvironmentEventsPublisher
    {
        void ExpressionEvaluatorBecomeAvaialable();
        void ExpressionEvaluatorBecomeUnAvaialable();
    }
}
