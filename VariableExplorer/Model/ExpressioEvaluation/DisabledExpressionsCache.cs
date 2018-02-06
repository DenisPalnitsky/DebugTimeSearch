using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLocals.Model.VSPropertyModel;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class DisabledExpressionsCache : IExpressionsCache
    {
        public void Cache(string expression, IVSDebugPropertyProxy resultDebugProperty)
        {
            // do nothing           
        }

        public IVSDebugPropertyProxy TryGetFromCache(string expression)
        {
            return null;
        }
    }
}
