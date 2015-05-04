using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Debugger.Utilities.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;

namespace ViewportAdornment1
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    class ViewportAdornment1
    {
        private Image _image;
        private IWpfTextView _wpfTextView;
        private IAdornmentLayer _adornmentLayer;
        private ITextView _textView;
        TextViewExtensions _tve;

        /// <summary>
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public ViewportAdornment1(IWpfTextView view, TextViewExtensions tve)
        {
            _wpfTextView = view;
            _textView = view;
            _tve = tve;

            Brush brush = new SolidColorBrush(Colors.Red);
            brush.Freeze();
            Brush penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            Pen pen = new Pen(penBrush, 0.5);
            pen.Freeze();

            //draw a square with the created brush and pen
            System.Windows.Rect r = new System.Windows.Rect(0, 0, 30, 30);
            Geometry g = new RectangleGeometry(r);
            GeometryDrawing drawing = new GeometryDrawing(brush, pen, g);
            drawing.Freeze();

            DrawingImage drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();

            _image = new Image();
            _image.Source = drawingImage;

            //Grab a reference to the adornment layer that this adornment should be added to
            _adornmentLayer = view.GetAdornmentLayer("ViewportAdornment1");

            _wpfTextView.ViewportHeightChanged += delegate { this.onSizeChange(); };
            _wpfTextView.ViewportWidthChanged += delegate { this.onSizeChange(); };
            _wpfTextView.MouseHover += _view_MouseHover;
        }

        void _view_MouseHover(object sender, MouseHoverEventArgs e)
        {            
            System.Diagnostics.Debug.WriteLine(
                    KeywordDetect.DetectKeyword(e.TextPosition.AnchorBuffer.CurrentSnapshot.GetText(),
                    e.Position) );

            System.Diagnostics.Debug.WriteLine("GetWordExtent: " + GetExpression(e));
        
        }

        private string GetExpression(MouseHoverEventArgs e)
        {
            ITextSnapshotLine line = e.TextPosition.AnchorBuffer.CurrentSnapshot.GetLineFromPosition(e.Position);
            TextSpan[] textSpan = new TextSpan[10];

            _tve.GetVsTextView(e.View).GetWordExtent(
                line.LineNumber,
                e.Position - line.Start.Position,
                (uint)WORDEXTFLAGS.WORDEXT_FINDEXPRESSION,
                textSpan);

            if (textSpan.Length > 0)
            {
                string lineText = e.TextPosition.AnchorBuffer.CurrentSnapshot.GetLineFromLineNumber(textSpan[0].iStartLine).GetText();
                return lineText.Substring(textSpan[0].iStartIndex, textSpan[0].iEndIndex - textSpan[0].iStartIndex);
            }
            else return "Nothing :(";
        }


        public void onSizeChange()
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_image, _wpfTextView.ViewportRight - 60);
            Canvas.SetTop(_image, _wpfTextView.ViewportTop + 30);

            //add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _image, null);
        }
    }
}
