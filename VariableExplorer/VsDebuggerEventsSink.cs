using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer
{
    class VsDebuggerEventsSink : IVsDebuggerEvents
    {
        public event EventHandler OnEnterRunMode;
        public event EventHandler OnEnterDesignMode;
        public event EventHandler OnEnterBreakMode;

        int IVsDebuggerEvents.OnModeChange(DBGMODE dbgmodeNew)
        {
            switch (dbgmodeNew)
            {
                case DBGMODE.DBGMODE_Run:
                    if (OnEnterRunMode != null)
                        OnEnterRunMode(this, null);                    
                    break;

                case DBGMODE.DBGMODE_Break:
                    if (OnEnterBreakMode != null)
                        OnEnterBreakMode(this, null);
                    break;

                case DBGMODE.DBGMODE_Design:
                    if (OnEnterDesignMode != null)
                        OnEnterDesignMode(this, null);
                    break;
            }

            return VSConstants.S_OK;
        }
    }
}
