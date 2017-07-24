using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace SearchLocals
{

    public class DebuggerEvents : IVsDebuggerEvents, IDebugEventCallback2
    {
        private readonly IVsDebugger _debugger;
        private uint _cookie;

        public event EventHandler OnEnterRunMode;
        public event EventHandler OnEnterDesignMode;
        public event EventHandler<IDebugThread2> OnEnterBreakMode;


        public DebuggerEvents(IVsDebugger debugger) 
        { 
            _debugger = debugger;
            _debugger.AdviseDebuggerEvents(this, out _cookie);
            _debugger.AdviseDebugEventCallback(this);
        }

        public int Event(IDebugEngine2 engine, IDebugProcess2 process, IDebugProgram2 program,
                         IDebugThread2 thread, IDebugEvent2 debugEvent, ref Guid riidEvent, uint attributes)
        {
            // TODO: Consider swithch to Microsoft.VisualStudio.Shell.KnownUIContexts.DebuggingContext.UIContextChanged
            
            System.Diagnostics.Debug.WriteLine(debugEvent.GetType());
        
            // TODO: Select appropriate event 
            if (debugEvent is IDebugBreakEvent2)
            {
                OnEnterBreakMode(this, thread);            
            }
            else if (debugEvent is IDebugBreakpointEvent2)
            {
                OnEnterBreakMode(this, thread);
            }
            else if (debugEvent is IDebugExceptionEvent2)
            {
                OnEnterBreakMode(this, thread);
            }     
         
            return VSConstants.S_OK;
        }

        public int OnModeChange(DBGMODE mode)
        {
            switch (mode)
            {
                case DBGMODE.DBGMODE_Run:
                    OnEnterRunMode?.Invoke(this, null);
                    break;

                case DBGMODE.DBGMODE_Break:
                    break;

                case DBGMODE.DBGMODE_Design:
                    OnEnterDesignMode?.Invoke(this, null);
                    break;
            }

            return VSConstants.S_OK;
        }
    }
}
