using EnvDTE;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer
{

    class ObjectDump
    {
        public static string  GetLocalVariable(string expression)
        {
            try
            {
                return GetCurrentObjectDump(expression);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.ToString());
                return "!!! Error: " + e.ToString();
            }                
        }

        public static string GetCurrentText()
        { 
            var selection = (TextSelection)VisualStudioServices.Dte.ActiveDocument.Selection;
            var activePoint = selection.ActivePoint;
            return selection.Text;
        }

        private static string GetCurrentObjectDump(string expression)
        {
            StringBuilder sb = new StringBuilder();
            
            var context = VisualStudioServices.GetService<IDebugExpressionContext2, IDebugExpressionContext2>();

            var stackFrame = (IDebugStackFrame2)VisualStudioServices.Dte.Debugger.CurrentStackFrame;
            
            IDebugExpressionContext2 debugContext;
            stackFrame.GetExpressionContext(out debugContext);
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
            


           
            //sb.AppendLine("Locals:");
            //foreach (Expression local in stackFrame.Locals)
            //{
            //    sb.AppendFormat("{0} = {1}\n", local.Name, local.Value);
            //}
           
            SimpleExpressionEvaluation(expression, sb);

            return sb.ToString();
        }

        private static void SimpleExpressionEvaluation(string expression, StringBuilder sb)
        {
            Expression selectedExpr = VisualStudioServices.Dte.Debugger.GetExpression(expression);

            sb.AppendLine("Expression:");
            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(selectedExpr));

            sb.AppendLine("Value:");
            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(selectedExpr.Value));

            sb.AppendLine("");
            

            System.Diagnostics.Debug.WriteLine(sb.ToString());
        }

        private static void CodeElementsByScopes(StringBuilder sb, VirtualPoint activePoint)
        {
            sb.AppendLine("Code elements");

            string elems = "";
            vsCMElement scopes = 0;

            foreach (vsCMElement scope in Enum.GetValues(scopes.GetType()))
            {
                CodeElement elem = activePoint.get_CodeElement(scope);

                if (elem != null)
                {
                    elems += elem.Name +
                        " (" + scope.ToString() + ")\n";
                }
            }

            sb.Append(elems);
        }
    }
}
