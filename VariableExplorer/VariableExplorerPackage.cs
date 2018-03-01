﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using SearchLocals.EditorHelper;
using SearchLocals.Model.ExpressioEvaluation;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SearchLocals
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
    [ProvideAutoLoad(UIContextGuids.Debugging)]
    public sealed class VariableExplorerPackage : Package
    {

        public static readonly VsCommandIdentifier CmdQuickWatch = new VsCommandIdentifier("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 254);

        public const string UIContextGuid = "{ADFC4E64-0397-11D1-9F4E-00A0C911004F}";

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

            //KnownUIContexts.DebuggingContext.UIContextChanged += DebuggingContext_UIContextChanged;

            CreateMenuCommands();
            AddMenuCommands();
        }

        

        //private void DebuggingContext_UIContextChanged(object sender, UIContextChangedEventArgs e)
        //{
        //    if (e.Activated)
        //    {
        //        AddMenuCommands();
        //    }
        //    else
        //    {
        //        RemoveMenuCommands();
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            _dispatcher.Dispose();
            //KnownUIContexts.DebuggingContext.UIContextChanged -= DebuggingContext_UIContextChanged;
            base.Dispose(disposing);
        }

        MenuCommand menuItem;
        MenuCommand menuToolWin;




        private void AddMenuCommands()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                
                // Create the command for the menu item.

                mcs.AddCommand(menuItem);

                // Create the command for the tool window

                mcs.AddCommand(menuToolWin);

                // Create the command for the context menu
                //CommandID contextMenuCommandID = new CommandID(GuidList.guidVariableExplorerCmdSet, (int)PkgCmdIDList.cmdidBrowseVariable);
                //MenuCommand contextMenuToolWin = new MenuCommand(ShowToolWindow, contextMenuCommandID);
                //mcs.AddCommand(contextMenuToolWin);
            }
        }

        private void RemoveMenuCommands()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                mcs.RemoveCommand(menuItem);
                mcs.RemoveCommand(menuToolWin);
            }
        }

        private void CreateMenuCommands()
        {
            CommandID menuCommandID = new CommandID(GuidList.guidVariableExplorerCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
            menuItem = new MenuCommand(MenuItemCallback, menuCommandID);

            CommandID toolwndCommandID = new CommandID(GuidList.guidVariableExplorerCmdSet, (int)PkgCmdIDList.cmdidMyTool);
            menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
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

            IVsTextManager txtMgr = VisualStudioServices.GetService<SVsTextManager,IVsTextManager>();
            int mustHaveFocus = 1;
            IVsTextView vTextView = null;            
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
                
            string textUnderCursor =  CodeUnderCursor.GetExpression(vTextView);
            var expressionEvaluatorViewModel = SearchLocals.Model.Services.ServiceLocator.Resolve<SearchLocals.UI.IExpressionEvaluatorViewModel>();
            expressionEvaluatorViewModel.FilterText = textUnderCursor;
            ShowToolWindow();               
        }

    }
}
