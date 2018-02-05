using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals
{
     public class VisualStudioServices
  {
    private static readonly Dictionary<Type, object> _serviceCache = new Dictionary<Type, object>();
    private static System.IServiceProvider _serviceProvider;
    //public static DummyWindow DummyInvokeWindow;
    private static Version _vsVersion;
    //private static IVsEditorAdaptersFactoryService _vsEditorAdaptersFactoryService;

    public static string VisualStudioDevEnvPath
    {
      get
      {
        return VisualStudioServices.Dte.FullName;
      }
    }

    //public static string VisualStudioCommon7Path
    //{
    //  get
    //  {
    //    string directoryName = Path.GetDirectoryName(VisualStudioServices.VisualStudioDevEnvPath);
    //    Release.Assert((directoryName != null ? 1 : 0) != 0, \u0023\u003DqMaeYhOXm5Tqyu4jbjzlvbWw7m24bW5Ft0fwI3Qd1nKY\u003D.\u0023\u003DqLDDNAQJ3_fgdUeR0zNvonQ\u003D\u003D(-1156219147), (object) VisualStudioServices.VisualStudioDevEnvPath);
    //    return Directory.GetParent(directoryName).FullName;
    //  }
    //}

    public static DTE Dte
    {
      get
      {
        return VisualStudioServices.GetService<SDTE, DTE>();
      }
    }

    //public static Debugger4 DteDebugger
    //{
    //  get
    //  {
    //    return (Debugger4) VisualStudioServices.Dte.Debugger;
    //  }
    //}

    public static IVsDebugger VsDebugger
    {
      get
      {
        return VisualStudioServices.GetService<IVsDebugger, IVsDebugger>();
      }
    }

    public static IVsDebugger2 VsDebugger2
    {
      get
      {
        return (IVsDebugger2) VisualStudioServices.GetService<IVsDebugger, IVsDebugger>();
      }
    }

    public static IVsShell VsShell
    {
      get
      {
        return VisualStudioServices.GetService<SVsShell, IVsShell>();
      }
    }

    public static IVsUIShell VsUiShell
    {
      get
      {
        return VisualStudioServices.GetService<SVsUIShell, IVsUIShell>();
      }
    }

    //public static IVsTextManager2 VsTextManager
    //{
    //  get
    //  {
    //    return VisualStudioServices.GetService<SVsTextManager, IVsTextManager2>();
    //  }
    //}

    public static IVsMonitorSelection MonitorSelection
    {
      get
      {
        return VisualStudioServices.GetService<SVsShellMonitorSelection, IVsMonitorSelection>();
      }
    }

    public static IVsRunningDocumentTable RunningDocumentTable
    {
      get
      {
        return VisualStudioServices.GetService<SVsRunningDocumentTable, IVsRunningDocumentTable>();
      }
    }

    public static IOleComponentManager OleComponentManager
    {
      get
      {
        return VisualStudioServices.GetService<SOleComponentManager, IOleComponentManager>();
      }
    }

    //public static IComponentModel ComponentModel
    //{
    //  get
    //  {
    //    return VisualStudioServices.GetService<SComponentModel, IComponentModel>();
    //  }
    //}

    public static IVsSolution VsSolution
    {
      get
      {
        return VisualStudioServices.GetService<SVsSolution, IVsSolution>();
      }
    }

    public static IVsSolutionBuildManager2 VsSolutionBuildManager
    {
      get
      {
        return (IVsSolutionBuildManager2) VisualStudioServices._serviceProvider.GetService(typeof (IVsSolutionBuildManager));
      }
    }

    //public static IDebuggerInternalAdapter DebuggerInternal
    //{
    //  get
    //  {
    //    IVsDebugger service = VisualStudioServices.GetService<SVsShellDebugger, IVsDebugger>();
    //    Version visualStudioVersion = VisualStudioServices.GetVisualStudioVersion();
    //    switch (visualStudioVersion.Major)
    //    {
    //      case 10:
    //        return (IDebuggerInternalAdapter) new DebuggerInternalAdapter100(service);
    //      case 11:
    //      case 12:
    //        return (IDebuggerInternalAdapter) new DebuggerInternalAdapter110(service);
    //      default:
    //        throw new InvalidOperationException();
    //    }
    //  }
    //}



    //public static IPackage Package
    //{
    //  get
    //  {
    //    if (VisualStudioServices._serviceProvider == null)
    //      throw new InvalidOperationException(\u0023\u003DqMaeYhOXm5Tqyu4jbjzlvbWw7m24bW5Ft0fwI3Qd1nKY\u003D.\u0023\u003DqLDDNAQJ3_fgdUeR0zNvonQ\u003D\u003D(-1156219831));
    //    else
    //      return (IPackage) VisualStudioServices._serviceProvider;
    //  }
    //}

    //public static IVsEditorAdaptersFactoryService VsEditorAdaptersFactoryService
    //{
    //  get
    //  {
    //    return VisualStudioServices._vsEditorAdaptersFactoryService ?? (VisualStudioServices._vsEditorAdaptersFactoryService = VisualStudioServices.ComponentModel.GetService<IVsEditorAdaptersFactoryService>());
    //  }
    //}

    //public static IVsLanguageDebugInfo CSharpLanguageDebugInfo
    //{
    //  get
    //  {
    //    return ServiceProvider.GlobalProvider.GetService(new Guid(\u0023\u003DqMaeYhOXm5Tqyu4jbjzlvbWw7m24bW5Ft0fwI3Qd1nKY\u003D.\u0023\u003DqLDDNAQJ3_fgdUeR0zNvonQ\u003D\u003D(-1156219861))) as IVsLanguageDebugInfo;
    //  }
    //}

    public static void Initialize(System.IServiceProvider package)
    {
      if (package == null)
        throw new ArgumentNullException("");
      VisualStudioServices._serviceProvider = package;
      //VisualStudioServices.DummyInvokeWindow = new DummyWindow();
    }

    public static TReturnType GetService<TRequestType, TReturnType>() where TReturnType : class
    {
      if (VisualStudioServices._serviceProvider == null)
        throw new InvalidOperationException();
      Type index = typeof (TRequestType);
      if (!VisualStudioServices._serviceCache.ContainsKey(index))
      {
        object service = VisualStudioServices._serviceProvider.GetService(index);
        if (service == null)
            throw new ExternalException("Can't get service " + index);
        VisualStudioServices._serviceCache[index] = service;
      }
      TReturnType returnType = VisualStudioServices._serviceCache[index] as TReturnType;
      if ((object) returnType == null)
        throw new InvalidOperationException("");
      else
        return returnType;
    }

    //public static Version GetVisualStudioVersion()
    //{
    //  return VisualStudioServices._vsVersion ?? (VisualStudioServices._vsVersion = new VsProperties(VisualStudioServices._serviceProvider).Version);
    //}

    public static void AddWatch(string watchText)
    {
      watchText = watchText.Replace(Environment.NewLine, "\n");
      // ISSUE: reference to a compiler-generated method
      VisualStudioServices.Dte.ExecuteCommand("AddWatch", watchText);
    }

    public static IVsHierarchy GetHierarchy(Project project)
    {
      IVsHierarchy ppHierarchy;
      ErrorHandler.ThrowOnFailure(VisualStudioServices.GetService<SVsSolution, IVsSolution>().GetProjectOfUniqueName(project.UniqueName, out ppHierarchy));
      return ppHierarchy;
    }

    public static ProjectItem ProjectItemFromWindowSafely(Window window)
    {
      if (window == null)
        throw new ArgumentNullException();
      try
      {
        return window.ProjectItem;
      }
      catch (InvalidCastException ex)
      {
        return (ProjectItem) null;
      }
    }

    public static void ExecuteCommand(VsCommandIdentifier cmd)
    {
      object obj = (object) null;
      VisualStudioServices.Dte.Commands.Raise(cmd.GUID, cmd.Index, ref obj, ref obj);
    }
  }
}
