using SearchLocals.Model.Services;
using SearchLocals.Model.VSPropertyModel;
using System.Collections.Concurrent;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class ExpressionsCache : IExpressionsCache
    {
        ILog _log = ServiceLocator.Resolve<ILog>();

        ConcurrentDictionary<string, IVSDebugPropertyProxy> _cache = new ConcurrentDictionary<string, IVSDebugPropertyProxy>();
       

        public IVSDebugPropertyProxy TryGetFromCache(string expression)
        {
            if (_cache.ContainsKey(expression))
            {
                _log.Info("Expression '{0}' taken from cache", expression);
                return _cache[expression];
            }

            return null;
        }

        public void Cache(string expression, IVSDebugPropertyProxy resultDebugProperty)
        {            
            _cache.TryAdd(expression, resultDebugProperty);
        }
    }
}
