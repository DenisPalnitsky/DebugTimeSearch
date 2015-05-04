using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ViewportAdornment1
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class MyCommandFilterProvider : IVsTextViewCreationListener
    {
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; set; }

        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

        [Import(typeof(SVsServiceProvider))]
        internal System.IServiceProvider ServiceProvider { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = EditorAdaptersFactory.GetWpfTextView(textViewAdapter);
            if (textView == null) return;
            ITextStructureNavigator navigator = TextStructureNavigatorSelector.GetTextStructureNavigator(textView.TextBuffer);
            AddCommandFilter(textViewAdapter, new MyCommandFilter(textView, navigator, ServiceProvider));
        }

        void AddCommandFilter(IVsTextView viewAdapter, MyCommandFilter commandFilter)
        {
            IOleCommandTarget next;
            if (VSConstants.S_OK == viewAdapter.AddCommandFilter(commandFilter, out next))
                if (next != null)
                    commandFilter.nextCmdTarget = next;
        }

    }

    internal class MyCommandFilter : IOleCommandTarget
    {
        private IWpfTextView wpfTextView;
        private ITextStructureNavigator navigator;
        private System.IServiceProvider serviceProvider;
        internal IOleCommandTarget nextCmdTarget;

        public MyCommandFilter(IWpfTextView textView, ITextStructureNavigator navigator, System.IServiceProvider sp)
        {
            this.wpfTextView = textView;
            this.navigator = navigator;
            this.serviceProvider = sp;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            // don't handle keystrokes when called via automation
            if (!VsShellUtilities.IsInAutomationFunction(serviceProvider))
            {
                if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                {
                    char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    if (typedChar.Equals(' '))
                    {
                        SnapshotPoint? point = wpfTextView.Caret.Position.Point.GetPoint(wpfTextView.TextBuffer, PositionAffinity.Predecessor);
                        if (point.HasValue)
                        {
                            // how to retrieve (and possibly store) this
                            TextExtent wordExtent = navigator.GetExtentOfWord(point.Value - 1);
                            string wordText = wpfTextView.TextSnapshot.GetText(wordExtent.Span);
                            System.Diagnostics.Debug.WriteLine(wordText);
                        }
                    }
                }
            }
            return nextCmdTarget.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextCmdTarget.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
