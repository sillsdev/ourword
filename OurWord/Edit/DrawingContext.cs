using System.Drawing;
using System.Drawing.Printing;

namespace OurWord.Edit
{
    public interface IDrawingContext
    {
        // Attributes ------------------------------------------------------------------------
        Graphics Graphics { get; }
        float AvailableWidthForContent { get; }
        float LeftMargin { get; }
        float TopMargin { get; }
        Color BackgroundColor { get; }
        float WidthBetweenColumns { get; }

        // Operations ------------------------------------------------------------------------
        float Measure(string sText, Font font);
        float PointsToDeviceY(float fPoints);
        float InchesToDeviceX(float fInches);
    }

    public class DrawingContext
    {
        #region Method: float Measure(Graphics, sText, font)
        protected static float Measure(Graphics g, string sText, Font font)
        {
            var fmt = StringFormat.GenericTypographic;
            fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            return g.MeasureString(sText, font, 1000, fmt).Width;
        }
        #endregion        
    }

    public class PrintContext : DrawingContext, IDrawingContext
    {
        // IDrawingContext Implementation ----------------------------------------------------
        #region Attr{g}: Graphics Graphics
        public Graphics Graphics
        {
            get
            {
                return m_Graphics;
            }
        }
        private readonly Graphics m_Graphics;
        #endregion
        #region Attr{g}: float AvailableWidthForContent
        public float AvailableWidthForContent
        {
            get
            {
                return m_PageSettings.Bounds.Width - 
                    m_PageSettings.Margins.Left -
                    m_PageSettings.Margins.Right;
            }
        }
        #endregion
        #region Attr{g}: float LeftMargin
        public float LeftMargin
        {
            get
            {
                return (m_PageSettings.Bounds.Left + m_PageSettings.Margins.Left);
            }
        }
        #endregion
        #region Attr{g}: float TopMargin
        public float TopMargin
        {
            get
            {
                return (m_PageSettings.Bounds.Top + m_PageSettings.Margins.Top);
            }
        }
        #endregion
        #region Attr{g}: Color BackgroundColor
        public Color BackgroundColor
        {
            get
            {
                return Color.White;
            }
        }
        #endregion
        #region Attr{g}: float WidthBetweenColumns
        public float WidthBetweenColumns
        {
            get
            {
                return 15F;
            }
        }
        #endregion

        #region Method: float Measure(sText, font)
        public float Measure(string sText, Font font)
        {
            return Measure(Graphics, sText, font);
        }
        #endregion
        #region Method: float PointsToPixelsY(fPixels)
        public float PointsToDeviceY(float fPixels)
        {
            return fPixels;
        }
        #endregion
        #region Method: float InchesToDeviceX(fInches)
        public float InchesToDeviceX(float fInches)
        {
            // For reasons I don't understand, I need to divide in order to have the
            // appearance on the printed page look correct. This was developed udner
            // a printer with 600dpi, will not surprise me if a different resolution 
            // requires different treatment. Frustrating.
            const float fKludge = 6F;

            return fInches * Graphics.DpiX / fKludge;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        private readonly PrintDocument m_PrintDoc;
        private readonly PageSettings m_PageSettings;
        #region Constructor(PrintDocument)
        public PrintContext(PrintDocument printDoc)
        {
            m_PrintDoc = printDoc;
            m_PageSettings = printDoc.PrinterSettings.DefaultPageSettings;

            m_Graphics = m_PrintDoc.PrinterSettings.CreateMeasurementGraphics(m_PageSettings);
        }
        #endregion
    }

    public class WindowContext : DrawingContext, IDrawingContext
    {
        // IDrawingContext Implementation ----------------------------------------------------
        #region Attr{g}: Graphics Graphics
        public Graphics Graphics
        {
            get
            {
                return m_Window.Draw.Graphics;
            }
        }
        #endregion
        #region Attr{g}: float AvailableWidthForContent
        public float AvailableWidthForContent
        {
            get
            {
                var fAvailableWidth = m_Window.Width - (m_Window.WindowMargins.Width * 2);
                if (m_Window.HasScrollbar)
                    fAvailableWidth -= m_Window.ScrollBar.Width;

                return fAvailableWidth;
            }
        }
        #endregion
        #region Attr{g}: float LeftMargin
        public float LeftMargin
        {
            get
            {
                return m_Window.WindowMargins.Width;
            }
        }
        #endregion
        #region Attr{g}: float TopMargin
        public float TopMargin
        {
            get
            {
                return m_Window.WindowMargins.Height;
            }
        }
        #endregion
        #region Attr{g}: Color BackgroundColor
        public Color BackgroundColor
        {
            get
            {
                return m_Window.BackColor;
            }
        }
        #endregion
        #region Attr{g}: float WidthBetweenColumns
        public float WidthBetweenColumns
        {
            get
            {
                return 10F;
            }
        }
        #endregion

        #region Method: float Measure(sText, font)
        public float Measure(string sText, Font font)
        {
            return Measure(Graphics, sText, font);
        }
        #endregion
        #region Method: float PointsToPixelsY(fPixels)
        public float PointsToDeviceY(float fPixels)
        {
            return (fPixels * Graphics.DpiY / 72F);
        }
        #endregion
        #region Method: float InchesToDeviceX(fInches)
        public float InchesToDeviceX(float fInches)
        {
            return fInches * Graphics.DpiX;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        private readonly OWWindow m_Window;
        #region Constructor(window)
        public WindowContext(OWWindow window)
        {
            m_Window = window;
        }
        #endregion
    }
}
