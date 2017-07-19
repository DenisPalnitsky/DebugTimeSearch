using System;

namespace SearchLocals.Model.VSEnvironmentEvents
{
    class VSEnvironmentEventsPublisher : IVSEnvironmentEventsPublisher, IVsEnvironmentEvents
    {
        public event EventHandler EvaluatorBecomeAvailable;
        public event EventHandler EvaluatorBecomeUnAvailable;

        public void ExpressionEvaluatorBecomeAvaialable()
        {
            EvaluatorBecomeAvailable?.Invoke(this, null);
        }

        public void ExpressionEvaluatorBecomeUnAvaialable()
        {
            EvaluatorBecomeUnAvailable?.Invoke(this, null);
        }
    }
}
