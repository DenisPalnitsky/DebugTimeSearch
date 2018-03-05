using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.Shell.Interop;
using SearchLocals.Model.ExpressioEvaluation;
using SearchLocals.UI;

namespace SearchLocals.Model.Services
{
    static class UnityContainerExtension
    {
            
        public static void RegisteDefaultTypes (this IUnityContainer container)
        {     
            container.RegisterType<ILog, OutputWindowLogger>();

            var ev = new VSEnvironmentEvents.VSEnvironmentEventsPublisher();
            container.RegisterInstance<IVSEnvironmentEventsPublisher>(ev);
            container.RegisterInstance<IVsEnvironmentEvents>(ev);

            container.RegisterType<ExpressioEvaluation.ISearchStatus,SearchStatusLister>();                        
            container.RegisterInstance<ITaskSchedulerProvider>(new TaskSchedulerProvider());

            var provider = new ExpressionEvaluatorProvider(ev);
            container.RegisterInstance<IExpressionEvaluatorProvider>(provider);
            container.RegisterInstance<IExpressionEvaluatorContainer>(provider);

            // Use this to disable cache while debugging
            //container.RegisterType<IExpressionsCache, DisabledExpressionsCache>();
            container.RegisterType<IExpressionsCache, ExpressionsCache>();


            IVsDebugger _debugger = VisualStudioServices.VsDebugger;
            ExpressionEvaluatorDispatcher _dispatcher;
            _dispatcher = ExpressionEvaluatorDispatcher.Create(VisualStudioServices.VsDebugger,
                container.Resolve<IExpressionEvaluatorContainer>(),
                container.Resolve<IExpressionsCache>());
            container.RegisterInstance<ExpressionEvaluatorDispatcher>(_dispatcher, new ContainerControlledLifetimeManager());

            container.RegisterType<IExpressionEvaluatorViewModel, ExpressionEvaluatorViewModel>();

        }
    }
}
