using Microsoft.VisualStudio.Debugger.Interop;
using MyCompany.VariableExplorer.Model;
using MyCompany.VariableExplorer.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class ExpressionEvaluator : IExpressionEvaluator
    {       
        ILog _log = IocContainer.Resolve<ILog>();

        public ExpressionEvaluator(IDebugStackFrame2 stackFrame )
        {
            _stackFrame = stackFrame;
        }

        IDebugStackFrame2 _stackFrame;

        public IDebugProperty EvaluateExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression is empty");
            }

            _log.Info("Evaluating expression {0}. CurrentTime {1:H:mm:ss.ffff}", expression, DateTime.Now);
            IDebugProperty2 debugProperty = GetVsDebugProperty(expression);
            _log.Info("Done evaluating expression {0}. CurrentTime {1:H:mm:ss.ffff}", expression, DateTime.Now);
            
            return Model.DebugProperty.Create (debugProperty);            
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
