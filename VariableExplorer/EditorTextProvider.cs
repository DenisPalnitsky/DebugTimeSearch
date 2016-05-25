using EnvDTE;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer
{

    class EditorTextProvider
    {

        public static string GetCurrentText()
        {
            var selection = (TextSelection)VisualStudioServices.Dte.ActiveDocument.Selection;            
            var activePoint = selection.ActivePoint;            
            return selection.Text;
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
