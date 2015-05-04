using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Debugger.Utilities.Text;

namespace ViewportAdornment1
{
    #region Adornment Factory
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class PurpleBoxAdornmentFactory : IWpfTextViewCreationListener
    {
        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered 
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("ViewportAdornment1")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;

        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService = null;

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

         //<summary>
         //Instantiates a ViewportAdornment1 manager when a textView is created.
         //</summary>
         //<param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            IComponentModel componentModel = ServiceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            IVsEditorAdaptersFactoryService editorFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            IServiceProvider sp = Package.GetGlobalService(
                            typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider))
                        as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;



            IVsTextView vsTextView = editorFactory.CreateVsTextViewAdapter(sp);
            var tve = new TextViewExtensions(editorFactory);


            new ViewportAdornment1(textView, tve);
            
        }

      
    }
    #endregion //Adornment Factory
}
