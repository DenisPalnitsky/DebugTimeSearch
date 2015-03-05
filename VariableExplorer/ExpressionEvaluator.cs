using Microsoft.VisualStudio.Debugger.Interop;
using MyCompany.VariableExplorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer
{
    class ExpressionEvaluator : IExpressionEvaluator
    {        
        public ExpressionEvaluator(IDebugStackFrame2 stackFrame )
        {
            _stackFrame = stackFrame;
        }

        public IDebugStackFrame2 _stackFrame { get; private set; }

        public IDebugProperty EvaluateExpression(string expression)
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
                        
            return Model.DebugProperty.Create (debugProperty);                                             
        }
        
    }
}
