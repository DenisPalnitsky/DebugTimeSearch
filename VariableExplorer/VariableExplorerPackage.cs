﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.ComponentModelHost;
//using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Debugger.Interop;
using EnvDTE;
using MyCompany.VariableExplorer.Model;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using MyCompany.VariableExplorer.EditorHelper;

namespace MyCompany.VariableExplorer
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(SearchLocalsToolWindow))]
    [Guid(GuidList.guidVariableExplorerPkgString)]
    [ProvideAutoLoad("{ADFC4E64-0397-11D1-9F4E-00A0C911004F}")]
    public sealed class VariableExplorerPackage : Package
    {

        public static readonly VsCommandIdentifier CmdQuickWatch = new VsCommandIdentifier("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 254);
        

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VariableExplorerPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            ShowToolWindow();
        }

        private void ShowToolWindow()
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            SearchLocalsToolWindow window = (SearchLocalsToolWindow)this.FindToolWindow(typeof(SearchLocalsToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        ExpressionEvaluatorDispatcher _dispatcher;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            VisualStudioServices.Initialize(this);
                        
            IVsDebugger _debugger = VisualStudioServices.VsDebugger;
             _dispatcher =  ExpressionEvaluatorDispatcher.Create(VisualStudioServices.VsDebugger);            


            SomeMenuCode();
        }

        protected override void Dispose(bool disposing)
        {
            _dispatcher.Dispose();
            base.Dispose(disposing);
        }
                    
        private void SomeMenuCode()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVariableExplorerCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);

                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidVariableExplorerCmdSet, (int)PkgCmdIDList.cmdidMyTool);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand(menuToolWin);

                // Create the command for the context menu
                //CommandID contextMenuCommandID = new CommandID(GuidList.guidVariableExplorerCmdSet, (int)PkgCmdIDList.cmdidBrowseVariable);
                //MenuCommand contextMenuToolWin = new MenuCommand(ShowToolWindow, contextMenuCommandID);
                //mcs.AddCommand(contextMenuToolWin);


            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;

            IVsTextManager txtMgr = VisualStudioServices.GetService<SVsTextManager,IVsTextManager>();
            int mustHaveFocus = 1;
            IVsTextView vTextView = null;            
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
                
            string textUnderCursor =  CodeUnderCursor.GetExpression(vTextView);
            var expressionEvaluatorViewModel = MyCompany.VariableExplorer.Model.Services.IocContainer.Resolve<MyCompany.VariableExplorer.UI.IExpressionEvaluatorViewModel>();
            expressionEvaluatorViewModel.ExpressionText = textUnderCursor;
            ShowToolWindow();

                

            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
            //           0,
            //           ref clsid,
            //           "VariableExplorer",
            //           string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback(). Text under cursor: '{1}'", this.ToString(), CodeUnderCursor.GetExpression(vTextView)),
            //           //objectDump,
            //           string.Empty,
            //           0,
            //           OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
            //           OLEMSGICON.OLEMSGICON_INFO,
            //           0,        // false
            //           out result));
        }


        private IWpfTextView GetActiveTextView()
        {
            IWpfTextView view = null;
            IVsTextView vTextView = null;

            IVsTextManager txtMgr =
                (IVsTextManager)GetService(typeof(SVsTextManager));
            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);

            IVsUserData userData = vTextView as IVsUserData;
            if (null != userData)
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                view = viewHost.TextView;
            }

            return view;
        }
    }
}
