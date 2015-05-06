using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.EditorHelper
{
    class CodeUnderCursor
    {
        public static string GetExpression(IVsTextView vsTextView)
        {
            int line;
            int column;
            vsTextView.GetCaretPos(out line, out column );
            
            TextSpan[] textSpan = new TextSpan[10];

            vsTextView.GetWordExtent(
                line,
                column,
                (uint)WORDEXTFLAGS.WORDEXT_FINDEXPRESSION,
                textSpan);

            if (textSpan.Length > 0)
            {
                IVsTextLines textLines;
                vsTextView.GetBuffer(out textLines);
                string text;

                textLines.GetLineText(textSpan[0].iStartLine, textSpan[0].iStartIndex, textSpan[0].iEndLine, textSpan[0].iEndIndex,
                    out text);

                return text;
            }
            else return "Nothing :(";
        }

                         
    }
}
