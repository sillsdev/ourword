using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace OurWord.Edit
{
    public interface IDraw
    {
        void FillRectangle(Color clrBackground, RectangleF rect);
        void DrawString(string s, Font f, Brush brush, PointF position);
        void DrawString(string s, Font f, Brush brush, RectangleF rect);
        void DrawRectangle(Pen pen, RectangleF rect);
        void DrawRoundedRectangle(Pen borderPen, Brush fillBrush, RectangleF rect, float fRadius);
        void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
        void DrawLine(Pen pen, PointF pt1, PointF pt2);
        void DrawVertLine(Pen pen, float x, float y1, float y2);
        void DrawBullet(Color color, PointF pt, float fRadius);
        void DrawImage(Image image, PointF pt);
    }

    public class Draw
    {
        #region SMethod: void DrawString(Graphics g, string s, Font font, Brush brush, PointF pt)
        static protected void DrawString(Graphics g, string s, Font font, Brush brush, PointF pt)
        {
            g.DrawString(s, font, brush, pt.X, pt.Y, StringFormat.GenericTypographic);
        }
        #endregion
        #region SMethod: void DrawBullet(Color, PointF, fRadius)
        protected static void DrawBullet(Graphics g, Color color, PointF pt, float fRadius)
        {
            Brush brush = new SolidBrush(color);

            var xLeft = pt.X - fRadius;
            var yTop = pt.Y - fRadius;
            var diameter = fRadius * 2;

            g.FillEllipse(brush, xLeft, yTop, diameter, diameter);
        }
        #endregion
        #region SMethod: void DrawRoundedRectangle(Pen, FillBrush, Rect, Radius)
        protected static void DrawRoundedRectangle(Graphics g, Pen BorderPen, Brush FillBrush, RectangleF Rect, float fRadius)
        {
            var xLeft = Rect.Left;
            var xRight = Rect.Right;
            var yTop = Rect.Top;
            var yBottom = Rect.Bottom;

            var diameter = fRadius * 2;

            var gp = new GraphicsPath();

            gp.StartFigure();

            // Top Horz line
            gp.AddLine(xLeft + fRadius, yTop, xRight - fRadius, yTop);

            // Top-right arc
            gp.AddArc(xRight - diameter, yTop, diameter, diameter, 270, 90);

            // Right Vert line
            gp.AddLine(xRight, yTop + fRadius, xRight, yBottom - fRadius);

            // Bottom-right arc
            gp.AddArc(xRight - diameter, yBottom - diameter, diameter, diameter, 0, 90);

            // Bottom Horz line
            gp.AddLine(xRight - fRadius, yBottom, xLeft + fRadius, yBottom);

            // Bottom-left arc
            gp.AddArc(xLeft, yBottom - diameter, diameter, diameter, 90, 90);

            // Left Vert line
            gp.AddLine(xLeft, yBottom - fRadius, xLeft, yTop + fRadius);

            // Top-Left arc
            gp.AddArc(xLeft, yTop, diameter, diameter, 180, 90);

            gp.CloseFigure();

            // Fill the interior if requested
            if (null != FillBrush)
                g.FillPath(FillBrush, gp);

            // Draw the border if requested
            if (null != BorderPen)
                g.DrawPath(BorderPen, gp);
        }
        #endregion
    }

    public class ScreenDraw : Draw, IDraw
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow)
        public ScreenDraw(OWWindow window)
        {
            m_bmpDoubleBuffer = new Bitmap(window.Width, window.Height);
            Graphics = Graphics.FromImage(m_bmpDoubleBuffer);

            // Turn off Hinting. This means that, yes, the text will not be as pretty to
            // read, but it makes the cursor appear in the correct place; and eliminates
            // the moving around that was happening when selecting text.
            Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            m_ScrollBar = window.ScrollBar;
        }
        #endregion
        private Bitmap m_bmpDoubleBuffer;
        private readonly VScrollBar m_ScrollBar;
        #region Attr{g}: float ScrollBarPosition
        private float ScrollBarPosition
        {
            get
            {
                return (null == m_ScrollBar) ? 0 : m_ScrollBar.Value;
            }
        }
        #endregion
        public Graphics Graphics { get; private set; }

        // IDraw Interface -------------------------------------------------------------------
        #region Method: void FillRectangle(clrBackground, rect)
        public void FillRectangle(Color clrBackground, RectangleF rect)
        {
            var r = new RectangleF(rect.X, rect.Y - ScrollBarPosition,
                rect.Width, rect.Height);

            Graphics.FillRectangle(new SolidBrush(clrBackground), r);
        }
        #endregion
        #region Method: void DrawString(s, font, Brush, pt)
        public void DrawString(string s, Font font, Brush brush, PointF pt)
        {
            pt = new PointF(pt.X, pt.Y - ScrollBarPosition);
            DrawString(Graphics, s, font, brush, pt);
        }
        #endregion
        #region Method: void DrawString(s, font, Brush, pt)
        public void DrawString(string s, Font font, Brush brush, RectangleF rect)
        {
            var r = new RectangleF(rect.X, rect.Y - ScrollBarPosition,
                rect.Width, rect.Height);

            Graphics.DrawString(s, font, brush, r);
        }
        #endregion
        #region Method: void DrawRectangle(Pen, RectangleF)
        public void DrawRectangle(Pen pen, RectangleF rect)
        {
            Graphics.DrawRectangle(pen,
                rect.X, rect.Y - ScrollBarPosition,
                rect.Width, rect.Height);
        }
        #endregion
        #region Method: void DrawRoundedRectangle(borderPen, fillBrush, rect, fRadius)
        public void DrawRoundedRectangle(Pen borderPen, Brush fillBrush, RectangleF rect, float fRadius)
        {
            var rectAdjusted = new RectangleF(rect.X - ScrollBarPosition, rect.Y, 
                rect.Width, rect.Height);

            DrawRoundedRectangle(Graphics, borderPen, fillBrush, rectAdjusted, fRadius);
        }
        #endregion
        #region Method: void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            Graphics.DrawLine(pen,
                x1, y1 - ScrollBarPosition,
                x2, y2 - ScrollBarPosition);
        }
        #endregion
        #region Method: void DrawLine(Pen pen, PointF pt1, PointF pt2)
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            Graphics.DrawLine(pen,
                pt1.X, pt1.Y - ScrollBarPosition,
                pt2.X, pt2.Y - ScrollBarPosition);
        }
        #endregion
        #region Method: void DrawVertLine(Pen pen, float x, float y1, float y2)
        public void DrawVertLine(Pen pen, float x, float y1, float y2)
        {
            Graphics.DrawLine(pen,
                x, y1 - ScrollBarPosition,
                x, y2 - ScrollBarPosition);
        }
        #endregion
        #region Method: void DrawBullet(Color, PointF, fRadius)
        public void DrawBullet(Color color, PointF pt, float fRadius)
        {
            DrawBullet(Graphics, color, 
                new PointF(pt.X, pt.Y - ScrollBarPosition), 
                fRadius);
        }
        #endregion
        #region Method: void DrawImage(Image image, PointF pt)
        public void DrawImage(Image image, PointF pt)
        {
            var point = new Point(
                (int)pt.X,
                (int)(pt.Y - ScrollBarPosition));

            Graphics.DrawImage(image, point);
        }
        #endregion

        // Unique to ScreenDraw --------------------------------------------------------------
        #region Method: void Dispose()
        public void Dispose()
        {
            if (null != m_bmpDoubleBuffer)
                m_bmpDoubleBuffer.Dispose();
            m_bmpDoubleBuffer = null;

            if (null != Graphics)
                Graphics.Dispose();
            Graphics = null;
        }
        #endregion
        #region Method: void TransferToScreen(Graphics destinationGraphics)
        public void TransferToScreen(Graphics destinationGraphics)
        {
            destinationGraphics.DrawImageUnscaled(m_bmpDoubleBuffer, 0, 0);
        }
        #endregion
    }

    public class PrinterDraw : Draw, IDraw
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(graphics)
        public PrinterDraw(Graphics graphics)
        {
            m_Graphics = graphics;
        }
        #endregion
        private readonly Graphics m_Graphics;

        // IDraw Interface -------------------------------------------------------------------
        #region Method: void FillRectangle(clrBackground, rect)
        public void FillRectangle(Color clrBackground, RectangleF rect)
        {
            m_Graphics.FillRectangle(new SolidBrush(clrBackground), rect);
        }
        #endregion
        #region Method: void DrawString(s, font, Brush, pt)
        public void DrawString(string s, Font font, Brush brush, PointF pt)
        {
            DrawString(m_Graphics, s, font, brush, pt);
        }
        #endregion
        #region Method: void DrawString(s, font, Brush, rect)
        public void DrawString(string s, Font font, Brush brush, RectangleF rect)
        {
            m_Graphics.DrawString(s, font, brush, rect);
        }
        #endregion
        #region Method: void DrawRectangle(Pen, RectangleF)
        public void DrawRectangle(Pen pen, RectangleF rect)
        {
            m_Graphics.DrawRectangle(pen,
                rect.X, rect.Y,
                rect.Width, rect.Height);
        }
        #endregion
        #region Method: void DrawRoundedRectangle(borderPen, fillBrush, rect, fRadius)
        public void DrawRoundedRectangle(Pen borderPen, Brush fillBrush, RectangleF rect, float fRadius)
        {
            DrawRoundedRectangle(m_Graphics, borderPen, fillBrush, rect, fRadius);
        }
        #endregion
        #region Method: void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            m_Graphics.DrawLine(pen, x1, y1, x2, y2);
        }
        #endregion
        #region Method: void DrawLine(Pen pen, PointF pt1, PointF pt2)
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            m_Graphics.DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        #endregion
        #region Method: void DrawVertLine(Pen pen, float x, float y1, float y2)
        public void DrawVertLine(Pen pen, float x, float y1, float y2)
        {
            m_Graphics.DrawLine(pen, x, y1, x, y2);
        }
        #endregion
        #region Method: void DrawBullet(Color, PointF, fRadius)
        public void DrawBullet(Color color, PointF pt, float fRadius)
        {
            DrawBullet(m_Graphics, color, pt, fRadius);
        }
        #endregion
        #region Method: void DrawImage(Image image, PointF pt)
        public void DrawImage(Image image, PointF pt)
        {
            var point = new Point( (int)pt.X, (int)(pt.Y) );

            m_Graphics.DrawImage(image, point);
        }
        #endregion
    }
}
