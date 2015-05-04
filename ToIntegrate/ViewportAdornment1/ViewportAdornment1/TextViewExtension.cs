//using Microsoft.VisualStudio.Debugger.DebuggerToolWindows.DisassemblyWindow;
//using Microsoft.VisualStudio.Debugger.Utilities;
//using Microsoft.VisualStudio.Debugger.Utilities.WPF;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Windows;
using System.Windows.Media;


namespace Microsoft.VisualStudio.Debugger.Utilities.Text
{
    internal class TextViewExtensions
    {
        IVsEditorAdaptersFactoryService _adapterService;
        //IServiceProvider _sp;

        public TextViewExtensions(IVsEditorAdaptersFactoryService adapterService)
        {
            _adapterService = adapterService;
        }


        public TextExtent GetSelectedExpression(ITextView view)
        {
            if (view.Selection.IsEmpty)
                return GetWordAtPosition(view, view.Caret.Position.VirtualBufferPosition.Position);
            else
                return new TextExtent(new SnapshotSpan(view.Selection.Start.Position, view.Selection.End.Position), true);
        }

        public TextExtent GetWordAtPosition(ITextView view, SnapshotPoint position)
        {
            ITextSnapshotLine containingLine = position.GetContainingLine();
            TextSpan[] pSpan = new TextSpan[1] { new TextSpan() };
            pSpan[0].iStartLine = pSpan[0].iEndLine = containingLine.LineNumber;
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            TextSpan local1 = pSpan[0];
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            TextSpan local2 = pSpan[0];
            int position1 = position.Position;
            int position2 = containingLine.Start.Position;
            int num1;
            int num2 = num1 = position1 - position2;
            // ISSUE: explicit reference operation
            local2.iEndIndex = num1;
            int num3 = num2;
            // ISSUE: explicit reference operation
            local1.iStartIndex = num3;
            if (GetVsTextView(view).GetWordExtent(pSpan[0].iStartLine, pSpan[0].iStartIndex, 0U, pSpan) != 0)
                return new TextExtent();
            int start = (int)(position.Snapshot.GetLineFromLineNumber(pSpan[0].iStartLine).Start + pSpan[0].iStartIndex);
            int length = (int)(position.Snapshot.GetLineFromLineNumber(pSpan[0].iEndLine).Start + pSpan[0].iEndIndex) - start;
            return new TextExtent(new SnapshotSpan(position.Snapshot, start, length), true);
        }

        public IVsTextView GetVsTextView(ITextView view)
        {
            return _adapterService.GetViewAdapter(view);
        }

        //public static Point AdjustedPointFromScreen(this IWpfTextView view, Point screenCoordinates)
        //{
        //    Point point = view.VisualElement.PointFromScreen(screenCoordinates);
        //    point.Y += view.ViewportTop;
        //    point.X += view.ViewportLeft;
        //    return point;
        //}

        //public static Point AdjustedPointFromDeviceIndependent(this IWpfTextView view, Point diCoordinates)
        //{
        //    Point point = ExtensionMethods.PointFromDeviceIndependent((Visual)view.VisualElement, diCoordinates);
        //    point.Y += view.ViewportTop;
        //    point.X += view.ViewportLeft;
        //    return point;
        //}

        //public static Point LinePointToDeviceIndependent(this IWpfTextView view, ITextViewLine line, Point lineCoords)
        //{
        //    Point local = new Point(lineCoords.X - view.ViewportLeft, lineCoords.Y - view.ViewportTop);
        //    return ExtensionMethods.PointToDeviceIndependent((Visual)view.VisualElement, local);
        //}

        //public static void EnsurePositionVisible(this ITextView view, SnapshotPoint position)
        //{
        //    if (view == null || view.TextViewLines == null || !(view.TextViewLines.FirstVisibleLine.Start > position) && ((int)position < (int)view.TextViewLines.LastVisibleLine.Start || view.TextViewLines.LastVisibleLine.VisibilityState != VisibilityState.PartiallyVisible) && !(position > view.TextViewLines.LastVisibleLine.End))
        //        return;
        //    view.DisplayTextLineContainingBufferPosition(position, view.ViewportHeight * 0.5, ViewRelativePosition.Top);
        //}

        //public static IWpfTextViewHost GetTextViewHost(this IWpfTextView textView)
        //{
        //    return ExtensionMethods.FindParentOfType<IWpfTextViewHost>((DependencyObject)textView.VisualElement);
        //}

        //public static void CenterAtPosition(this ITextView view, SnapshotPoint position, int numberVisibleLines)
        //{
        //    if (view == null || view.TextViewLines == null || !(view.TextViewLines.FirstVisibleLine.Start > position) && !(position > view.TextViewLines.LastVisibleLine.End))
        //        return;
        //    int lineNumber = Math.Max(0, view.TextSnapshot.GetLineNumberFromPosition((int)position) - numberVisibleLines / 2);
        //    SnapshotPoint start = view.TextSnapshot.GetLineFromLineNumber(lineNumber).Start;
        //    TextViewExtensions.ScrollToPosition(view, start);
        //}

        //public static void ScrollToPosition(this ITextView view, SnapshotPoint position)
        //{
        //    view.DisplayTextLineContainingBufferPosition(position, 0.0, ViewRelativePosition.Top);
        //}

       

        //public static SnapshotPoint? ConvertToPosition(this ITextView view, Point mousePos)
        //{
        //    SnapshotPoint? nullable = new SnapshotPoint?();
        //    if (view != null && view.TextViewLines != null && (mousePos.X >= 0.0 && mousePos.X < view.ViewportWidth) && (mousePos.Y >= 0.0 && mousePos.Y < view.ViewportHeight))
        //    {
        //        ITextViewLine containingYcoordinate = view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePos.Y + view.ViewportTop);
        //        if (containingYcoordinate != null)
        //        {
        //            double xCoordinate = mousePos.X + view.ViewportLeft;
        //            nullable = containingYcoordinate.GetBufferPositionFromXCoordinate(xCoordinate);
        //            if (!nullable.HasValue && containingYcoordinate.LineBreakLength == 0 && ((int)containingYcoordinate.EndIncludingLineBreak == view.TextSnapshot.Length && containingYcoordinate.Left <= xCoordinate) && xCoordinate < containingYcoordinate.TextRight + containingYcoordinate.EndOfLineWidth)
        //                nullable = new SnapshotPoint?(containingYcoordinate.End);
        //        }
        //    }
        //    return nullable;
        //}

        //public static SnapshotSpan? ConvertPositionToLineSpan(this ITextView view, Point position)
        //{
        //    SnapshotSpan? nullable = new SnapshotSpan?();
        //    ITextViewLine nearestLine = TextViewExtensions.FindNearestLine(view, position);
        //    if (nearestLine != null)
        //        nullable = new SnapshotSpan?(new SnapshotSpan(view.TextSnapshot, new Span(nearestLine.Start.Position, nearestLine.Length)));
        //    return nullable;
        //}

        //public static ITextViewLine FindNearestLine(this ITextView view, Point position)
        //{
        //    ITextViewLine textViewLine = (ITextViewLine)null;
        //    if (view != null && view.TextViewLines != null && (position.Y >= view.TextViewLines[0].Top && position.Y < view.TextViewLines[view.TextViewLines.Count - 1].Bottom))
        //        textViewLine = view.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y);
        //    if (textViewLine == null && view.TextViewLines != null)
        //    {
        //        if (position.Y >= view.TextViewLines[view.TextViewLines.Count - 1].Bottom)
        //            textViewLine = view.TextViewLines[view.TextViewLines.Count - 1];
        //        else if (position.Y < view.ViewportTop)
        //            textViewLine = view.TextViewLines[0];
        //    }
        //    return textViewLine;
        //}

    

        //public static IVsWindowFrame GetWindowFrame(this ITextView view)
        //{
        //    if (view.TextBuffer.ContentType.IsOfType("Disassembly"))
        //        return (ServiceProvider.Instance.DisassemblyWindow as Mediator).ToolWindowFrame;
        //    IVsTextView vsTextView = TextViewExtensions.GetVsTextView(view);
        //    if (vsTextView == null)
        //        return (IVsWindowFrame)null;
        //    else
        //        return TextViewExtensions.GetWindowFrame(vsTextView);
        //}

        //public static IVsWindowFrame GetWindowFrame(this IVsTextView vsTextView)
        //{
        //    IServiceProvider serviceProvider = vsTextView as IServiceProvider;
        //    if (serviceProvider != null)
        //        return serviceProvider.GetService(typeof(SVsWindowFrame)) as IVsWindowFrame;
        //    else
        //        return (IVsWindowFrame)null;
        //}

        //public static bool IsViewOnScreen(this ITextView view)
        //{
        //    IVsWindowFrame windowFrame = TextViewExtensions.GetWindowFrame(view);
        //    int pfOnScreen;
        //    if (windowFrame != null && windowFrame.IsOnScreen(out pfOnScreen) == 0)
        //        return pfOnScreen != 0;
        //    else
        //        return false;
        //}

        //public static void Focus(this ITextView view)
        //{
        //    ((UIElement)view).Focus();
        //}

        //public static ITextDocument GetTextDocument(this ITextView view)
        //{
        //    ITextDocumentFactoryService service = ComponentManager.GetService<ITextDocumentFactoryService>();
        //    ITextDocument textDocument = (ITextDocument)null;
        //    service.TryGetTextDocument(view.TextBuffer, out textDocument);
        //    return textDocument;
        //}
    }
}
