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
        const int dwTimeout = 10000;

        public ExpressionEvaluator(IDebugStackFrame2 stackFrame )
        {
            _stackFrame = stackFrame;
        }
        

        public IDebugStackFrame2 _stackFrame { get; private set; }

        public string EvaluateExpression(string expression)
        {
           
            StringBuilder sb = new StringBuilder();                        

            IDebugExpressionContext2 debugContext;
            _stackFrame.GetExpressionContext(out debugContext).ThrowOnFailure();
            IDebugExpression2 debugExpression;
            string errorString;
            uint pichError;
            
            
            debugContext.ParseText(expression, 
                                    enum_PARSEFLAGS.PARSE_EXPRESSION,
                                    10,
                                    out debugExpression, 
                                    out errorString,
                                    out pichError).ThrowOnFailure();
            
            
            IDebugProperty2 debugProperty;
            
            debugExpression.EvaluateSync(enum_EVALFLAGS.EVAL_RETURNVALUE,
                    dwTimeout,
                    null,
                    out debugProperty).ThrowOnFailure();

            sb.AppendLine("");
            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(debugProperty));
                       
            ShowPropertyValues(sb, debugProperty);            
            
            // enum children            
            EnumChildren(sb, debugProperty);
            
            return sb.ToString();
        }

        private static void EnumChildren(StringBuilder sb,  IDebugProperty2 debugProperty)
        {
            IEnumDebugPropertyInfo2 debugPropertyEnum;

            debugProperty.EnumChildren(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL,
                10,
                dbgGuids.guidFilterLocals,
                enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_ALL,
                "", 
                dwTimeout, 
                out debugPropertyEnum).ThrowOnFailure();

            uint count;
            debugPropertyEnum.GetCount(out count).ThrowOnFailure();
            DEBUG_PROPERTY_INFO [] debugPropInfos = new DEBUG_PROPERTY_INFO[count];
            uint fetched;
            debugPropertyEnum.Next(count, debugPropInfos, out fetched).ThrowOnFailure();
            sb.AppendLine("------ Enum children");
            OutputDEBUG_PROPERTY_INFO(sb, debugPropInfos);
        }
        
        private static void ShowPropertyValues(StringBuilder sb, IDebugProperty2 debugProperty)
        {
            IDebugReference2[] reference = new IDebugReference2[1];
            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];

            debugProperty.GetPropertyInfo(
                    enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE,
                    10,
                    dwTimeout,
                    reference,
                    0,
                    propertyInfo).ThrowOnFailure();

            OutputDEBUG_PROPERTY_INFO(sb, propertyInfo);
        }

        private static void OutputDEBUG_PROPERTY_INFO(StringBuilder sb, DEBUG_PROPERTY_INFO[] propertyInfo)
        {
            foreach (var prop in propertyInfo)
            {
                sb.AppendFormat("Type {0}, {1} = {2}", prop.bstrType, prop.bstrName, prop.bstrValue);
                sb.AppendLine();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
