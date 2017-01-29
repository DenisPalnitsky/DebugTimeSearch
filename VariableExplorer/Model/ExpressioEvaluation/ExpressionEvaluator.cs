using Microsoft.VisualStudio.Debugger.Interop;
using MyCompany.VariableExplorer.Model.Services;
using MyCompany.VariableExplorer.Model.VSPropertyModel;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace MyCompany.VariableExplorer.Model.ExpressioEvaluation
{
    class ExpressionEvaluator : IExpressionEvaluator
    {       
        ILog _log = IocContainer.Resolve<ILog>();
        ConcurrentDictionary<string, IDebugProperty> _cache = new ConcurrentDictionary<string, IDebugProperty>();
        IDebugStackFrame2 _stackFrame;

        public ExpressionEvaluator(IDebugStackFrame2 stackFrame )
        {
            _stackFrame = stackFrame;
        }
        
        public IDebugProperty EvaluateExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression is empty");
            }

            _log.Info("Evaluating expression {0}. CurrentTime {1:H:mm:ss.ffff}", expression, DateTime.Now);
            if (_cache.ContainsKey(expression))
            {
                _log.Info("Expression '{0}' taken from cache", expression);                
                return _cache[expression];
            }
            
            IDebugProperty2 debugProperty = GetVsDebugProperty(expression);
            _log.Info("Done evaluating expression {0}. CurrentTime {1:H:mm:ss.ffff}", expression, DateTime.Now);
            
            IDebugProperty resultDebugProperty = DebugProperty.Create(debugProperty);            
            _cache[expression] = resultDebugProperty;
            return resultDebugProperty;
        }

        public IDebugProperty GetLocals()
        {
            IDebugProperty2 d;
            _stackFrame.GetDebugProperty(out d);

            return DebugProperty.Create(d);
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
                    IocContainer.Resolve<IConfiguration>().DefaultTimeoutForVSCalls,
                    null,
                    out debugProperty).ThrowOnFailure();
            return debugProperty;
        }
     
    }
}
