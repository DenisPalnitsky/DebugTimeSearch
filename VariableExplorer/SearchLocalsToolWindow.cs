﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.Shell;
using SearchLocals.Model.Services;
using SearchLocals.UI;

namespace SearchLocals
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("569ac44f-8998-443a-ba4d-40c5f2ad9077")]
    public class SearchLocalsToolWindow : ToolWindowPane
    {

        IExpressionEvaluatorViewModel _expressionEvaluatorViewModel;
        UI.SearchLocalsControl _searchLocalsControl;
        IUnityContainer _unityContainer = new UnityContainer();
       

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public SearchLocalsToolWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            _unityContainer.RegisteDefaultTypes();
               

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            _searchLocalsControl = new UI.SearchLocalsControl();

            _expressionEvaluatorViewModel = _unityContainer.Resolve<IExpressionEvaluatorViewModel>();
            _searchLocalsControl.DataContext = _expressionEvaluatorViewModel;

            base.Content = _searchLocalsControl; 
        }             
        
        internal void SetFilterText(string filterText)
        {            
            _expressionEvaluatorViewModel.FilterText = filterText;            
        }

        protected override void Dispose(bool disposing)
        {
            _unityContainer.Dispose();
            base.Dispose(disposing);
        }
    }
}
