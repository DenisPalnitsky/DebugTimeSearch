using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;

namespace SearchLocals.Model.ExpressioEvaluation
{
    class ExpressionEvaluatorDispatcher : IDisposable
    {        
        DebuggerEvents _debuggerEvents;
        IExpressionEvaluatorContainer _expressionEvaluatorContainer;
        IExpressionsCache _expressionCache;

        private ExpressionEvaluatorDispatcher(IExpressionEvaluatorContainer expressionEvaluatorContainer, IExpressionsCache expressionCache)
        {            
            _expressionEvaluatorContainer = expressionEvaluatorContainer;
            _expressionCache = expressionCache;
        }

        internal static ExpressionEvaluatorDispatcher Create(Microsoft.VisualStudio.Shell.Interop.IVsDebugger vsDebugger,
            IExpressionEvaluatorContainer container,
            IExpressionsCache expressionCache)
        {
            ExpressionEvaluatorDispatcher result = new ExpressionEvaluatorDispatcher(container, expressionCache);
            uint debugEventsCookie = VSConstants.VSCOOKIE_NIL;
            result._debuggerEvents = new DebuggerEvents(vsDebugger);
            result._debuggerEvents.OnEnterBreakMode += result.OnEnterBreakMode;
            result._debuggerEvents.OnEnterDesignMode += result.debuggerSink_OnEnterDesignMode;
            vsDebugger.AdviseDebuggerEvents(result._debuggerEvents, out debugEventsCookie).ThrowOnFailure();
            return result;
        }

        private  IExpressionEvaluator GetExpressionEvaluator(IDebugThread2 debugThread, IExpressionsCache expressionCache )
        {
            var stackFrame = GetCurrentStackFrame(debugThread);            
            return new ExpressionEvaluator(stackFrame, expressionCache);
        }        

        private IDebugStackFrame2 GetCurrentStackFrame(IDebugThread2 thread)
        {
            IEnumDebugFrameInfo2 enumDebugFrameInfo2;
            thread.EnumFrameInfo(enum_FRAMEINFO_FLAGS.FIF_FRAME, 10, out enumDebugFrameInfo2);

            uint pCeltFetched = 0;

            IDebugStackFrame2 stackFrame = null;

            uint count;
            enumDebugFrameInfo2.GetCount(out count);
            FRAMEINFO[] frameinfo = new FRAMEINFO[1];
            if (count > 0 && enumDebugFrameInfo2.Next(1, frameinfo, ref pCeltFetched) == VSConstants.S_OK)
            {
                stackFrame = frameinfo[0].m_pFrame;
            }
            else
            {
                throw new InvalidOperationException("IDebugStackFrame2 is not available");
            }

            return stackFrame;
        }

        private void debuggerSink_OnEnterDesignMode(object sender, EventArgs e)
        {
            _expressionEvaluatorContainer.UnRegister();
        }

        private void OnEnterBreakMode(object sender, IDebugThread2 debugThread)
        {
            _expressionEvaluatorContainer.Register(GetExpressionEvaluator(debugThread, _expressionCache));
        }


        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Disposing ExpressionEvaluatorDispatcher");
            _expressionEvaluatorContainer.UnRegister();
            _debuggerEvents.OnEnterBreakMode -= OnEnterBreakMode;
            _debuggerEvents.OnEnterDesignMode += debuggerSink_OnEnterDesignMode;
        }
    }
}
