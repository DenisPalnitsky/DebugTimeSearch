using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals
{
    public static class ErrorCheckForVsCalls
    {
        public static void ThrowOnFailure(this int returnCode)
        {
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(returnCode);
        }
    }
}
