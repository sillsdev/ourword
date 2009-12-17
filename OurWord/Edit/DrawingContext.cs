using System.Drawing;
using System.Drawing.Printing;

namespace OurWord.Edit
{
    public class DrawingContext
    {
        // Attrs -----------------------------------------------------------------------------
        public Graphics Graphics { get; private set; }
        public float AvailableWidthForContent { get; private set; }
        public float LeftMargin { get; private set; }
        public float TopMargin { get; private set; }

        // Creation --------------------------------------------------------------------------
        static public DrawingContext CreateFromWindow(OWWindow window)
        {
            // Width of window less margins and scrollbar
            var fAvailableWidth = window.Width - (window.WindowMargins.Width*2);
            if (window.HasSelection)
                fAvailableWidth -= window.ScrollBar.Width;

            return new DrawingContext
            {
                Graphics = window.Draw.Graphics,
                AvailableWidthForContent = fAvailableWidth,
                LeftMargin = window.WindowMargins.Width,
                TopMargin = window.WindowMargins.Height
            };
        }

        static public DrawingContext CreateFromPrintDocument(PrintDocument pdoc)
        {
            // Shorthand
            var pageSettings = pdoc.PrinterSettings.DefaultPageSettings;

            // Width of paper less margins
            var fAvailableWidth = pageSettings.Bounds.Width - pageSettings.Margins.Left -
                pageSettings.Margins.Right;

            return new DrawingContext
            {
                Graphics = pdoc.PrinterSettings.CreateMeasurementGraphics(),
                AvailableWidthForContent = fAvailableWidth,
                LeftMargin = pageSettings.Bounds.Left + pageSettings.Margins.Left,
                TopMargin = pageSettings.Bounds.Top + pageSettings.Margins.Top
            };
        }

    }




}
