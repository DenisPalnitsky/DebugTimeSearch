using SearchLocals.Model.VSPropertyModel;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class DisabledExpressionsCache : IExpressionsCache
    {
        public void Cache(string expression, IVSDebugPropertyProxy resultDebugProperty)
        {
            // do nothing           
        }

        public void Clear()
        {
            // do nothing            
        }

        public IVSDebugPropertyProxy TryGetFromCache(string expression)
        {
            return null;
        }
    }
}
