using Microsoft.VisualStudio.Debugger.Interop;
using SearchLocals.Model.Services;
using SearchLocals.Model.VSPropertyModel;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class ExpressionEvaluator : IExpressionEvaluator
    {       
        ILog _log = ServiceLocator.Resolve<ILog>();
        
        IDebugStackFrame2 _stackFrame;

        IExpressionsCache _cache =  ServiceLocator.Resolve<IExpressionsCache>();

        public ExpressionEvaluator(IDebugStackFrame2 stackFrame )
        {
            _stackFrame = stackFrame;
        }
        
        public IVSDebugPropertyProxy EvaluateExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression is empty");
            }

            _log.Info("Evaluating expression {0}. CurrentTime {1:H:mm:ss.ffff}", expression, DateTime.Now);
            
            IVSDebugPropertyProxy resultDebugProperty = _cache.TryGetFromCache(expression);
            if (resultDebugProperty != null)
                return resultDebugProperty;

            IDebugProperty2 debugProperty = GetVsDebugProperty(expression);
            _log.Info($"Done evaluating expression {expression} ", DateTime.Now);

            resultDebugProperty = VSDebugPropertyProxy.Create(debugProperty);
            _log.Info($"Property retreived { resultDebugProperty }");

            _cache.Cache(expression, resultDebugProperty);
            
            return resultDebugProperty;
        }

        public IVSDebugPropertyProxy GetLocals()
        {
            IDebugProperty2 d;
            _stackFrame.GetDebugProperty(out d).ThrowOnFailure();

            if (d == null)
                throw new InvalidOperationException("Error receiving locals");
                
            return VSDebugPropertyProxy.Create(d);
        }

        private IDebugProperty2 GetVsDebugProperty(string expression)
        {
            StringBuilder sb = new StringBuilder();

            IDebugExpressionContext2 debugContext;
            _stackFrame.GetExpressionContext(out debugContext).ThrowOnFailure();
            IDebugExpression2 debugExpression;
            string errorString;
            uint pichError;

             
            debugContext.ParseText(expression,
                                    enum_PARSEFLAGS.PARSE_EXPRESSION | enum_PARSEFLAGS.PARSE_DESIGN_TIME_EXPR_EVAL,
                                    10,
                                    out debugExpression,
                                    out errorString,
                                    out pichError).ThrowOnFailure();


            IDebugProperty2 debugProperty;
            
            debugExpression.EvaluateSync(
                enum_EVALFLAGS.EVAL_NOSIDEEFFECTS |
                enum_EVALFLAGS.EVAL_ALLOW_IMPLICIT_VARS |
                enum_EVALFLAGS.EVAL_ALLOWERRORREPORT,
                    ServiceLocator.Resolve<IConfiguration>().DefaultTimeoutForVSCalls,
                    null,
                    out debugProperty).ThrowOnFailure();
            return debugProperty;
        }
     
    }
}
