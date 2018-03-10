using SearchLocals.Model.VSPropertyModel;

namespace SearchLocals.Model.ExpressioEvaluation
{
    interface IExpressionsCache
    {
        void Cache(string expression, IVSDebugPropertyProxy resultDebugProperty);
        IVSDebugPropertyProxy TryGetFromCache(string expression);
        void Clear();
    }
}