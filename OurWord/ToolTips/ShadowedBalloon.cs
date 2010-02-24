#region ***** ShadowedBalloon.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ShadowedBalloon.cs
 * Author:  John Wimbish
 * Created: 11 Feb 2010
 * Purpose: A shadowed ballon window as the superclass for tooltips
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OurWord.Edit;

#endregion

namespace OurWord.ToolTips
{
    public partial class ShadowedBalloon : Form
    {
        // User-Definable Geometry -----------------------------------------------------------
        #region Attr{g}: float CornerRadius
        public float CornerRadius
        {
            get
            {
                return m_fCornerRadius;
            }
            set
            {
                Debug.Assert(value >= 7, "Small radius does not work for some reason");
                m_fCornerRadius = value;
            }
        }
        private float m_fCornerRadius = 10F;
        #endregion
        #region Attr{g}: float TipWidthAtBase
        public float TipWidthAtBase = 20F;
        #endregion
        #region Attr{g}: float TipHeight
        public float TipHeight = 15F;
        #endregion
        #region Attr{g}: int ShadowSize
        public int ShadowSize = 3;
        #endregion
        #region Attr{g}: int InteriorMargin
        public int InteriorMargin = 10;
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Attr{g}: OWWindow UnderlyingWindow
        protected OWWindow UnderlyingWindow
        {
            get
            {
                return G.App.CurrentLayout;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected ShadowedBalloon()
        {
            InitializeComponent();
        }
        #endregion
        #region Method: void Launch(Point ptTipInScreenCoords)
        public void Launch(Point ptTipInScreenCoords)
        {
            SetPosition(ptTipInScreenCoords);
            OnLayoutControls();
            OnPopulateControls();
            MoveMouseIntoWindow();
            ShowDialog(UnderlyingWindow);
        }
        #endregion
        #region VirtMethod: void OnLayoutControls()
        protected virtual void OnLayoutControls()
        {
            // Subclasses should position their controls here
        }
        #endregion
        #region VirtMethod: void OnPopulateControls()
        protected virtual void OnPopulateControls()
        {
            // Subclasses should put data into their controls here
        }
        #endregion
        #region Cmd: cmdFirstShown
        private void cmdFirstShown(object sender, EventArgs e)
            // Subclasses should, e.g., select the control that should have initial focus
        {
            Focus();
        }
        #endregion
        #region VirtMethod: OnMouseLeave
        private void OnMouseLeave(object sender, EventArgs e)
            // This closes the window if the mouse leaves it. 
            // This is only active if we're displaying as a balloon. For a normal dialog,
            // we expect the user to close it as in normal Windows clicking on an 'x'.
        {
            if (!PaintAsBalloon)
                return;

            var location = PointToClient(MousePosition);

            if (location.X <= 0 || location.X >= Width ||
                location.Y <= 0 || location.Y >= Height)
            {
                Close();
            }
        }
        #endregion

        // Window Positioning ----------------------------------------------------------------
        private const int c_nMarginFromScreenEdge = 4;
        #region Method: Screen GetScreenContainingPoint(Point ptTipInScreenCoords)
        static Screen GetScreenContainingPoint(Point ptTipInScreenCoords)
        {
            var screen = Screen.PrimaryScreen;
            foreach (var sc in Screen.AllScreens)
            {
                if (sc.Bounds.Contains(ptTipInScreenCoords))
                    screen = sc;
            }
            return screen;
        }
        #endregion
        #region Method: void SetPosition(Point ptTipInScreenCoords)
        void SetPosition(Point ptTipInScreenCoords)
        {
            var screen = GetScreenContainingPoint(ptTipInScreenCoords);

            // Horizontal
            // We want the Tip to be at least this far from the Left side of the tooltip
            const int xOffset = 30;
            // We want to position the Tooltip horizontally so that it is left-aligned with 
            // the block; but moving it to the left if it will not fit on the current screen
            var x = ptTipInScreenCoords.X - xOffset;
            if (x + Width > screen.Bounds.Right - c_nMarginFromScreenEdge)
                x = screen.Bounds.Right - Width - c_nMarginFromScreenEdge;

            // Vertical:
            // 1. Our top desire is to position so we're just under the block
            var y = ptTipInScreenCoords.Y;
            // 2. This could leave the block partially below the screen, in which case we attempt
            // to position it above the block
            if (y + Height > screen.Bounds.Bottom - c_nMarginFromScreenEdge)
                y -= Height;
            // 3. Patheological case: For a really small screen, this could leave us above the 
            // top of the screen. So now we must potentially move it down.
            y = Math.Max(y, 0);

            // Position the window
            Location = new Point(x, y);

            // Set our "Tip" attribute to client coordinates, as expected by the Path methods
            Tip = PointToClient(ptTipInScreenCoords);
        }
        #endregion
        #region Method: void MoveMouseIntoWindow()
        protected void MoveMouseIntoWindow()
            // We want to move the Mouse to be within the window, so that it is easier on
            // the user; otherwise a faulty mouse movement would easily dismiss the window.
            // So we want to move it vertically so that it is within the window.
        {
            // We'll work with client coordinates
            var ptCursorClientCoords = PointToClient(Cursor.Position);

            // Current x,y; we'll change these if needed
            var x = ptCursorClientCoords.X;
            var y = ptCursorClientCoords.Y;

            var rect = ContentAreaInsideMargins;

            x = Math.Max(x, rect.Left);
            x = Math.Min(x, rect.Right);

            y = Math.Max(y, rect.Top);
            y = Math.Min(y, rect.Bottom);

            // COnvert back to screen coordinates and set the cursor
            ptCursorClientCoords = new Point(x,y);
            Cursor.Position = PointToScreen(ptCursorClientCoords);
        }
        #endregion

        // Tip and related queries -----------------------------------------------------------
        #region Attr{g}: Point Tip
        private Point Tip
        {
            get
            {
                return m_ptTip;
            }
            set
            {
                m_ptTip = value;
            }
        }
        private Point m_ptTip = new Point(30, 0);
        #endregion
        private enum Position { Top, Bottom };
        #region VAttr{g}: Position TipPosition
        Position TipPosition
        {
            get
            {
                return (Tip.Y == 0) ? Position.Top : Position.Bottom;
            }
        }
        #endregion
        #region VAttr{g}: RectangleF ContentWindowRectangle
        private RectangleF ContentWindowRectangle
        {
            get
            {
                // If not showing the balloon, we just want a normal dialog
                if (!PaintAsBalloon)
                    return ClientRectangle;

                // Determine the endpoints of the inside rectangle; the rectangle with the border
                // around it that excludes the tip.
                var xLeft = ClientRectangle.X;

                var xRight = ClientRectangle.Right - ShadowSize;

                var yTop = (TipPosition == Position.Top) ?
                    ClientRectangle.Y + TipHeight : ClientRectangle.Y;

                var yBottom = ClientRectangle.Bottom - ShadowSize;
                if (TipPosition == Position.Bottom) 
                    yBottom -= (int)TipHeight;

                return new RectangleF(xLeft, yTop, xRight - xLeft, yBottom - yTop);
            }
        }
        #endregion
        #region VAttr{g}: Rectangle ContentAreaInsideMargins
        protected Rectangle ContentAreaInsideMargins
        {
            get
            {
                var rect = ContentWindowRectangle;

                return new Rectangle(
                    (int)rect.Location.X + InteriorMargin, 
                    (int)rect.Location.Y + InteriorMargin,
                    (int)rect.Size.Width - InteriorMargin*2,
                    (int)rect.Size.Height - InteriorMargin*2);
            }
        }
        #endregion

        // GraphicsPath ----------------------------------------------------------------------
        #region Method: void AddLineToPath(GraphicsPath, xLeft, xRight, yLine)
        void AddLineToPath(GraphicsPath path, float xLeft, float xRight, float yLine, Point tip)
        {
            // Calculate the x at the base, left side; make sure it doesn't go too far left
            var xPointerLeft = tip.X - TipWidthAtBase / 2;
            xPointerLeft = Math.Max(xLeft, xPointerLeft);

            // Calculate the x at the base, right side; make sure it doesn't go too far right
            var xPointerRight = xPointerLeft + TipWidthAtBase;
            xPointerRight = Math.Min(xRight, xPointerRight);

            // With both Left and Right adjusted, make sure we still have the correct width
            // at the base
            xPointerLeft = xPointerRight - TipWidthAtBase;

            // We'll collect the set of endpoints, moving left-to-right
            var v = new List<PointF> { new PointF(xLeft, yLine) };

            // Line to the left of the base, if any
            if (xLeft != xPointerLeft)
                v.Add(new PointF(xPointerLeft, yLine));

            // Line defining the tip
            v.Add(tip);
            v.Add(new PointF(xPointerRight, yLine));

            // Line to the right of the base, if any
            if (xRight != xPointerRight)
                v.Add(new PointF(xRight, yLine));

            // We must add the points in the direction of the overall path. So if the tip is on
            // the top line, then we go left-to-right; but if on the bottom, we must reverse it.
            // This is a requirement of how the GraphicsPath expects its entities to be input.
            if (TipPosition == Position.Top)
            {
                for (var i = 0; i < v.Count - 1; i++)
                    path.AddLine(v[i], v[i + 1]);
            }
            else
            {
                for (var i = v.Count - 1; i > 0; i--)
                    path.AddLine(v[i], v[i - 1]);
            }
        }
        #endregion
        #region Method: GraphicsPath CalculateGraphicsPath(RectangleF rectContent, Point tip)
        GraphicsPath CalculateGraphicsPath(RectangleF rect, Point tip)
        {
            var path = new GraphicsPath();

            var xLeft = rect.Left;
            var xRight = rect.Right;
            var yTop = rect.Top;
            var yBottom = rect.Bottom;

            var diameter = CornerRadius * 2;

            path.StartFigure();

            // Top Line
            if (TipPosition == Position.Top)
                AddLineToPath(path, xLeft + CornerRadius, xRight - CornerRadius, yTop, tip);
            else
                path.AddLine(xLeft + CornerRadius, yTop, xRight - CornerRadius, yTop);

            // Top-right arc
            path.AddArc(xRight - diameter, yTop, diameter, diameter, 270, 90);

            // Right Vert line
            path.AddLine(xRight, yTop + CornerRadius, xRight, yBottom - CornerRadius);

            // Bottom-right arc
            path.AddArc(xRight - diameter, yBottom - diameter, diameter, diameter, 0, 90);

            // Bottom Line
            if (TipPosition == Position.Bottom)
                AddLineToPath(path, xLeft + CornerRadius, xRight - CornerRadius, yBottom, tip);
            else
                path.AddLine(xRight - CornerRadius, yBottom, xLeft + CornerRadius, yBottom);

            // Bottom-left arc
            path.AddArc(xLeft, yBottom - diameter, diameter, diameter, 90, 90);

            // Left Vert line
            path.AddLine(xLeft, yBottom - CornerRadius, xLeft, yTop + CornerRadius);

            // Top-Left arc
            path.AddArc(xLeft, yTop, diameter, diameter, 180, 90);

            path.CloseFigure();

            return path;
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region Attr{g/s}: bool PaintAsBalloon
        protected bool PaintAsBalloon
            // Toggles between the balloon owner draw, vs the standard Windows sizeable dialog
        {
            get
            {
                return m_bPaintAsBalloon;
            }
            set
            {
                if (m_bPaintAsBalloon == value)
                    return;

                m_bPaintAsBalloon = value;

                // Normal dialog
                if (!m_bPaintAsBalloon)
                {
                    FormBorderStyle = FormBorderStyle.Sizable;
                    ControlBox = true;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.None;
                    ControlBox = false;
                }

                Invalidate();
            }
        }
        private bool m_bPaintAsBalloon = true;
        #endregion
        #region Attr{g}: Color BackgroundColor
        protected Color BackgroundColor
        {
            get
            {
                return Color.Pink;
            }
        }
        #endregion
        #region Cmd: cmdPaint
        private void cmdPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            var brushBackground = new SolidBrush(BackgroundColor);

            if (!PaintAsBalloon)
            {
                g.FillRectangle(brushBackground, ClientRectangle);
                return;
            }

            // Difficult to get the border to draw correctly; by trial and error this seems
            // to work, but I imagine there is a better way.
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Shadow border
            var shadowBrush = new SolidBrush(Color.Gray);
            var shadowRect = ContentWindowRectangle;
            shadowRect.Offset(ShadowSize, ShadowSize);
            var shadowTip = Tip;
            shadowTip.Offset(ShadowSize, ShadowSize);
            var shadowPath = CalculateGraphicsPath(shadowRect, shadowTip);
            g.FillPath(shadowBrush, shadowPath);
            g.DrawPath(new Pen(Color.Gray, ShadowSize), shadowPath);

            // Main Border
            var path = CalculateGraphicsPath(ContentWindowRectangle, Tip);
            g.FillPath(brushBackground, path);
            g.DrawPath(new Pen(Color.Navy, 1), path);
        }
        #endregion
        #region Cmd: OnPaintBackground
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Surpress normal Windows painting of the background
            // by doing nothing
            if (!PaintAsBalloon)
                base.OnPaintBackground(e);
        }
        #endregion
        #region Cmd: cmdResize
        private void cmdResize(object sender, EventArgs e)
        {
            OnLayoutControls();
        }
        #endregion
    }

    #region CLASS: TrulyTransparentToolStripRenderer
    public class TrulyTransparentToolStripRenderer : ToolStripProfessionalRenderer 
        // Inherit from ToolStripProfessionalRenderer to get reasonable behavior. This is far
        // better than inheriting from ToolStripRenderer, which gives flakey drawing results.
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Do nothing. By default, ToolStrip attempts to draw a border, even though
            // it is set to Transparent mode. So by not drawing the border, we get
            // the effect of transparency.
        }
    }
    #endregion
}


