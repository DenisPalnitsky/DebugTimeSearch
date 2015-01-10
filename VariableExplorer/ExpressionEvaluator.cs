using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer
{
    class ExpressionEvaluator : IDisposable
    {
        public ExpressionEvaluator(IDebugStackFrame2 stackFrame )
        {
            _stackFrame = stackFrame;
        }
        

        public IDebugStackFrame2 _stackFrame { get; private set; }

        public string EvaluateExpression(string expression)
        {
           
            StringBuilder sb = new StringBuilder();                        

            IDebugExpressionContext2 debugContext;
            _stackFrame.GetExpressionContext(out debugContext);
            IDebugExpression2 debugExpression;
            string errorString;
            uint pichError;
            debugContext.ParseText(expression, enum_PARSEFLAGS.PARSE_EXPRESSION, 10, out debugExpression, out errorString, out pichError);
            
            
            IDebugProperty2 debugProperty;
            debugExpression.EvaluateSync(enum_EVALFLAGS.EVAL_RETURNVALUE,
                6000,
                null,
                out debugProperty);

            sb.AppendLine("");
            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(debugProperty));
                       
            
            IDebugReference2[] reference = new IDebugReference2[1];
            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];
            
            debugProperty.GetPropertyInfo(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL,
                10, 
                100000,
                reference, 
                0, 
                propertyInfo);

             
            foreach (var prop in propertyInfo)
            {
                sb.AppendFormat("{2} {1} = {0}", prop.bstrName, prop.bstrValue, prop.bstrType);
            }
            
            return sb.ToString();
        }

        public void Dispose()
        {
            
        }
    }
}
