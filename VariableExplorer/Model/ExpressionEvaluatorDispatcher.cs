using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class ExpressionEvaluatorDispatcher : IDisposable
    {        
        DebuggerEvents debuggerEvents;
        
        public IExpressionEvaluator GetExpressionEvaluator(IDebugThread2 debugThread)
        {
            var stackFrame = GetCurrentStackFrame(debugThread);
            //stackFrame.GetExpressionContext(out expressionContext);
            return new ExpressionEvaluator(stackFrame);
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

        internal static ExpressionEvaluatorDispatcher Create(Microsoft.VisualStudio.Shell.Interop.IVsDebugger vsDebugger)
        {
            ExpressionEvaluatorDispatcher result = new ExpressionEvaluatorDispatcher();
            uint debugEventsCookie = VSConstants.VSCOOKIE_NIL;
            result.debuggerEvents = new DebuggerEvents(vsDebugger);
            result.debuggerEvents.OnEnterBreakMode += result.debuggerSink_OnEnterBreakMode;
            result.debuggerEvents.OnEnterDesignMode += result.debuggerSink_OnEnterDesignMode;
            vsDebugger.AdviseDebuggerEvents(result.debuggerEvents, out debugEventsCookie).ThrowOnFailure();            
            return result;
        }

        void debuggerSink_OnEnterDesignMode(object sender, EventArgs e)
        {
            UnregisterExpressionEvaluator();
        }

        private static void UnregisterExpressionEvaluator()
        {
            if (IocContainer.Resolve<IExpressionEvaluator>() != null)
            {
                IocContainer.UnRegisterInstance<IExpressionEvaluator>();
            }
        }

        void debuggerSink_OnEnterBreakMode(object sender, IDebugThread2 debugThread)
        {
            ExpressionEvaluatorDispatcher provider = new ExpressionEvaluatorDispatcher();
            IocContainer.RegisterInstance<IExpressionEvaluator>(provider.GetExpressionEvaluator(debugThread));
        }


        public void Dispose()
        {
            UnregisterExpressionEvaluator();
            debuggerEvents.OnEnterBreakMode -= debuggerSink_OnEnterBreakMode;
            debuggerEvents.OnEnterDesignMode += debuggerSink_OnEnterDesignMode;
        }
    }
}
