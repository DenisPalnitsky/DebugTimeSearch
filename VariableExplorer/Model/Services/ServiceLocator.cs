using Microsoft.Practices.Unity;
using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.UI;

namespace SearchLocals.Model.Services
{
    static class UnityContainerExtension
    {
            
        public static void RegisteTypes (this IUnityContainer container)
        {            
            container.RegisterType<ILog, OutputWindowLogger>();

            var ev = new VSEnvironmentEvents.VSEnvironmentEventsPublisher();
            container.RegisterInstance<IVSEnvironmentEventsPublisher>(ev);
            container.RegisterInstance<IVsEnvironmentEvents>(ev);

            container.RegisterType<ExpressioEvaluation.ISearchStatus,SearchStatusLister>();                        
            container.RegisterInstance<ITaskSchedulerProvider>(new TaskSchedulerProvider());

            container.RegisterType<IExpressionEvaluatorProvider, ExpressionEvaluatorProvider>();
            container.RegisterType<IExpressionEvaluatorContainer, ExpressionEvaluatorProvider>();

#if DEBUG
            container.RegisterType<IExpressionsCache, DisabledExpressionsCache>();
#else
            container.RegisterType<IExpressionsCache, ExpressionsCache>();
#endif
            container.RegisterType<IExpressionEvaluatorViewModel, ExpressionEvaluatorViewModel>();

        }
    }
}
