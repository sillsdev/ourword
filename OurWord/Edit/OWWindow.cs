#region ***** OWWindow.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    OWWindow.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2007
 * Purpose: A window, with rows and piles
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
using Palaso.UI.WindowsForms.Keyboarding;
#endregion
#endregion

namespace OurWord.Edit
{
    public class OWWindow : Panel
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: float WidthBetweenColumns
        public float WidthBetweenColumns
        {
            get
            {
                return m_fWidthBetweenColumns;
            }
            set
            {
                m_fWidthBetweenColumns = value;
            }
        }
        float m_fWidthBetweenColumns;
        #endregion
        #region Attr{g}: SizeF WindowMargins - the pixels between window edge and content (top & bottom)
        public SizeF WindowMargins
        {
            get
            {
                Debug.Assert(null != m_szWindowMargins);
                return m_szWindowMargins;
            }
            set
            {
                m_szWindowMargins = value;
            }
        }
        SizeF m_szWindowMargins;
        #endregion
        #region Attr{g}: int ColumnCount - the number of columns in each row, must be >= 1.
        public int ColumnCount
        {
            get
            {
                // Should be set in the constuctor
                Debug.Assert(m_cColumnCount >= 1);

                return m_cColumnCount;
            }
        }
        int m_cColumnCount = 0;
        #endregion
        #region Attr{g/s}: float ZoomFactor
        public float ZoomFactor
        {
            get
            {
                // We're supporting 60% through 250% in the dialog box currently
                Debug.Assert(m_fZoomFactor > 0.5F && m_fZoomFactor < 2.6F);
                return m_fZoomFactor;
            }
            set
            {
                m_fZoomFactor = value;
            }
        }
        float m_fZoomFactor = 1.0F;
        #endregion

        // Registry-Stored Settings ----------------------------------------------------------
        #region Attr{g}: string RegistrySettingsSubKey
        string RegistrySettingsSubKey
        {
            get
            {
                Debug.Assert(null != m_sRegistrySettingsSubKey && 
                    m_sRegistrySettingsSubKey.Length > 0);

                return m_sRegistrySettingsSubKey;
            }
        }
        string m_sRegistrySettingsSubKey = null;
        #endregion
        #region REGISTRY: Background Color
        const string c_NameBackColor = "BackColor";
        static public void SetRegistryBackgroundColor(string sSubKey, string sColor)
        {
            JW_Registry.SetValue(sSubKey, c_NameBackColor, sColor);
        }
        static public string GetRegistryBackgroundColor(string sSubKey, string sColorDefault)
        {
            return JW_Registry.GetValue(sSubKey, c_NameBackColor, sColorDefault);
        }
        #endregion

        // Interaction with OurWord Main -----------------------------------------------------
        bool m_bLoaded = false;
        #region Method: virtual void LoadData() - the subclass should populate the window
        public virtual void LoadData()
            // Called by the Client (OurWordMain) whenever it is time to clear and then
            // repopulate the contents of the window
            //
            // The client MUST call this method at the very end, after it has placed
            // content into the windows.
        {
            // Get the window's background color (it may have changed since the window was 
            // first created.
            string sBackColor = GetRegistryBackgroundColor(RegistrySettingsSubKey, "Wheat");
            BackColor = Color.FromName(sBackColor);

            // Instantiate a draw object (Assumption is that if we need to do a Resize,
            // then the Draw object's double buffer needs to be recreated so it will be
            // of the correct size.
            Draw = new DrawBuffer(this);

            // Start with a fresh calculation of the font size, window width, etc., for
            // the LineNumbers column, should it be turned on by the user.
            m_LineNumberAttrs = new CLineNumberAttrs(Draw.Graphics);

            // Measure all of the EBlocks. This is a one-time thing; we only do it again when
            // individual EWords are edited/added/etc.
            Contents.CalculateBlockWidths(Draw.Graphics);

            // Signal that we have a Draw object and that the blocks are all measured.
            // Otherwise, DoLayout would choke.
            m_bLoaded = true;

            // Perform a layout so that everything is located correctly
            DoLayout();

            // Tell Windows to paint the entire window
            Invalidate();

            // Select the first possible item
            Contents.Select_FirstWord();
            Focus();

            // Load, layout amd paint any secondary windows
            foreach (OWWindow w in SecondaryWindows)
                w.LoadData();
        }
        #endregion
        #region Method: void SetSize(Size)
        public void SetSize(Size sz)
        {
            SetSize(sz.Width, sz.Height);
        }
        #endregion
        #region Method: void SetSize(nWidth, nHeight) - respond to resize of owner
        public void SetSize(int nWidth, int nHeight)
        {
            Width = nWidth;
            Height = nHeight;

            Draw = new DrawBuffer(this);
            DoLayout();
        }
        #endregion

        // Messages to Main Window -----------------------------------------------------------
        #region Attr{g}: OWWindow MainWindow
        public OWWindow MainWindow
        {
            get
            {
                return m_wndMain;
            }
        }
        OWWindow m_wndMain = null;
        #endregion
        #region VAttr{g}: bool IsMainWindow - F if a secondary window
        bool IsMainWindow
        {
            get
            {
                return (null == MainWindow);
            }
        }
        #endregion
        #region VMethod: void OnCursorTimerTick()
        protected virtual void OnCursorTimerTick()
        {
        }
        #endregion

        // Messages To Secondary Windows -----------------------------------------------------
        #region Attr{g}: OWWindow[] SecondaryWindows
        OWWindow[] SecondaryWindows
        {
            get
            {
                Debug.Assert(null != m_vSecondaryWindows);
                return m_vSecondaryWindows;
            }
        }
        OWWindow[] m_vSecondaryWindows;
        #endregion
        #region Method: void ResetSecondaryWindows
        public void ResetSecondaryWindows()
        {
            foreach (OWWindow w in SecondaryWindows)
                w.m_wndMain = null;

            m_vSecondaryWindows = new OWWindow[0];
        }
        #endregion
        #region Method: void RegisterSecondaryWindow(OWWindow wnd)
        public void RegisterSecondaryWindow(OWWindow wnd)
        {
            // Reciprocal: Make sure the secondary knows who its main is.
            wnd.m_wndMain = this;

            // Add the new window to our vector of windows
            OWWindow[] v = new OWWindow[SecondaryWindows.Length + 1];
            for (int i = 0; i < SecondaryWindows.Length; i++)
                v[i] = SecondaryWindows[i];
            v[SecondaryWindows.Length] = wnd;
            m_vSecondaryWindows = v;
        }
        #endregion
        #region Secondary Window Message: OnSelectAndScrollToNote
        public virtual void OnSelectAndScrollToNote(TranslatorNote note)
        {
        }
        public void Secondary_SelectAndScrollToNote(TranslatorNote note)
        {
            foreach (OWWindow w in SecondaryWindows)
                w.OnSelectAndScrollToNote(note);
        }
        #endregion
        #region Secondary Window Message: OnSelectionChanged
        public virtual void OnSelectionChanged(DBasicText dbt)
        {
        }
        public void Secondary_OnSelectionChanged(DBasicText dbt)
        {
            foreach (OWWindow w in SecondaryWindows)
                w.OnSelectionChanged(dbt);

            // Don't let a secondary window capture focus
            if (!Focused)
                Focus();
        }
        #endregion
        #region VirtMethod: ENote.Flags GetNoteContext(note, ParagraphFlags)
        public virtual ENote.Flags GetNoteContext(TranslatorNote note, OWPara.Flags ParagraphFlags)
        {
            Debug.Assert(false, "Views must override GetNoteContext so that notes will display");
            return ENote.Flags.None;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public enum WindowClass { Tooltip };
        #region Constructor(WindowClass)
        public OWWindow(WindowClass wc)
        {
            if (wc != WindowClass.Tooltip)
            {
                Debug.Assert(false, "This constructor is only for tooltips.");
                return;
            }

            // General setup
            m_cColumnCount = 1;
            Name = "ToolTip";
            m_sRegistrySettingsSubKey = Name;
            m_Contents = new ERoot(this);
            WindowMargins = new SizeF(7, 5);
            m_vSecondaryWindows = new OWWindow[0];
            m_LineUpDownX = new LineUpDownX();

            // Typical tooltip color
            SetRegistryBackgroundColor(Name, "Cornsilk");

            // Double buffer for flicker-free painting 
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Single line for the border
            BorderStyle = BorderStyle.FixedSingle;
        }
        #endregion
        #region Constructor(sName, cColumnCount)
        public OWWindow(string _sName, int _cColumnCount)
            : base()
        {
            // Number of columns in this window
            m_cColumnCount = _cColumnCount;

            // Name for the window
            Name = _sName;

            // Registry key for settings
            m_sRegistrySettingsSubKey = _sName;
            if (_sName.StartsWith("LS-"))
                m_sRegistrySettingsSubKey = LiterateSettingsWnd.c_sRegSubKey;

            // Initialize ScrollBar
            AutoScroll = false;             // Don't use Panel's built-in scrollbar
            m_ScrollBar = new VScrollBar();
            m_ScrollBar.Dock = DockStyle.Right;
            Controls.Add(m_ScrollBar);
            ScrollBarPosition = 0;
            ScrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);

            // Used for Up/Down arrow behavior
            m_LineUpDownX = new LineUpDownX();

            // Initialize the EItems root container (to empty subitems)
            m_Contents = new ERoot(this);

            // Default margins
            WindowMargins = new SizeF(7, 5);
            WidthBetweenColumns = 10;

            // Set up a double buffer for flicker-free painting (it must be re-created
            // upon any resize.)
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Give us a sunken border
            BorderStyle = BorderStyle.Fixed3D;

            // Vector of secondary windows
            m_vSecondaryWindows = new OWWindow[0];

            // Initialize the tooltips
            m_ToolTip = OWToolTip.ToolTip;
        }
        #endregion
        #region Method: void TypingErrorBeep()
        static public void TypingErrorBeep()
        {
            int frequency = 37;  // Minimum possible
            int duration = 100;  // 1/10 of a second
            Console.Beep(frequency, duration);
        }
        #endregion
        #region OBSOLETE 5sep09 - override void OnCreateControl()
        /**
         * Removed on 5 Sep 09, on working to create the ToolTip window, because it
         * was causing the data to be loaded twice. I have no memory of why this is
         * here, and so am not removing it entirely in case the reason shows up soon.
        protected override void OnCreateControl()
        {
            Console.WriteLine("OnCreateControl LOADDATA");
            base.OnCreateControl();

            LoadData();
        }
        **/
        #endregion
        #region Attr{g}: virtual string WindowName - override, e.g., "Drafting"
        public virtual string WindowName
        {
            get
            {
                return "OWWindow";
            }
        }
        #endregion
        #region Attr{g}: string LanguageInfo - e.g., "Kupang to AMARASI"
        public virtual string LanguageInfo
        {
            get
            {
                return "";
            }
        }
        #endregion
        #region Attr{g}: virtual string PassageName
        public virtual string PassageName
        {
            get
            {
                if (!DB.IsValidProject)
                    return "";
                if (null == DB.TargetTranslation)
                    return "";
                if (null == DB.FrontTranslation)
                    return "";
                if (null == DB.TargetSection)
                    return "";

                return DB.TargetSection.ReferenceName;
            }
        }
        #endregion

        // Placing Data Content into the window ----------------------------------------------
        #region Attr{g}: ERoot Contents
        public ERoot Contents
        {
            get
            {
                Debug.Assert(null != m_Contents);
                return m_Contents;
            }
        }
        ERoot m_Contents;
        #endregion
        #region Method: void Clear()
        public void Clear()
        {
            // Reset the Rows
            Contents.Clear();

            // No longer is there a selection
            Selection = null;

            // Have the secondary windows do the same
            foreach (OWWindow w in SecondaryWindows)
                w.Clear();
        }
        #endregion

        // Layout & Paint --------------------------------------------------------------------
        #region CLASS: DrawBuffer
        public class DrawBuffer
        {
            // Attrs (Mostly only used internally) -------------------------------------------
            #region Attr{g}: Bitmap DoubleBuffer
            public Bitmap DoubleBuffer
            {
                get
                {
                    Debug.Assert(null != m_bmpDoubleBuffer);
                    return m_bmpDoubleBuffer;
                }
            }
            Bitmap m_bmpDoubleBuffer;
            #endregion
            #region Attr{g}: Graphics Graphics
            public Graphics Graphics
            {
                get
                {
                    Debug.Assert(null != m_graphics);
                    return m_graphics;
                }
            }
            Graphics m_graphics;
            #endregion
            #region Attr{g}: OWWindow Wnd
            OWWindow Wnd
            {
                get
                {
                    Debug.Assert(null != m_wnd);
                    return m_wnd;
                }
            }
            OWWindow m_wnd;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(OWWindow)
            public DrawBuffer(OWWindow wnd)
            {
                // Save a pointer to the window
                m_wnd = wnd;

                // Create space for the double buffer
                m_bmpDoubleBuffer = new Bitmap(wnd.Width, wnd.Height);

                // Set a graphics object to it
                m_graphics = Graphics.FromImage(DoubleBuffer);

                // Turn off Hinting. This means that, yes, the text will not be as pretty to
                // read, but it makes the cursor appear in the correct place; and eliminates
                // the moving around that was happening when selecting text.
                m_graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            #endregion
            #region Method: void Dispose() -good to call this manually to free memory
            public void Dispose()
            {
                if (null != DoubleBuffer)
                    DoubleBuffer.Dispose();
                m_bmpDoubleBuffer = null;

                if (null != Graphics)
                    Graphics.Dispose();
                m_graphics = null;
            }
            #endregion

            // Drawing Methods ---------------------------------------------------------------
            #region Method: void DrawRoundedRectangle(Pen, FillBrush, Rect, Radius)
            public void DrawRoundedRectangle(Pen BorderPen, Brush FillBrush, RectangleF Rect, float Radius)
            {
                float xLeft = Rect.Left;
                float xRight = Rect.Right;
                float yTop = Rect.Top - Wnd.ScrollBarPosition;
                float yBottom = Rect.Bottom - Wnd.ScrollBarPosition;

                float diameter = Radius * 2;

                GraphicsPath gp = new GraphicsPath();

                gp.StartFigure();

                // Top Horz line
                gp.AddLine(xLeft + Radius, yTop, xRight - Radius, yTop);

                // Top-right arc
                gp.AddArc(xRight - diameter, yTop, diameter, diameter, 270, 90);

                // Right Vert line
                gp.AddLine(xRight, yTop + Radius, xRight, yBottom - Radius);

                // Bottom-right arc
                gp.AddArc(xRight - diameter, yBottom - diameter, diameter, diameter, 0, 90);

                // Bottom Horz line
                gp.AddLine(xRight - Radius, yBottom, xLeft + Radius, yBottom);

                // Bottom-left arc
                gp.AddArc(xLeft, yBottom - diameter, diameter, diameter, 90, 90);

                // Left Vert line
                gp.AddLine(xLeft, yBottom - Radius, xLeft, yTop + Radius);

                // Top-Left arc
                gp.AddArc(xLeft, yTop, diameter, diameter, 180, 90);

                gp.CloseFigure();

                // Fill the interior if requested
                if (null != FillBrush)
                    Graphics.FillPath(FillBrush, gp);

                // Draw the border if requested
                if (null != BorderPen)
                    Graphics.DrawPath(BorderPen, gp);
            }
            #endregion
            #region Method: void DrawRectangle(Pen, RectangleF)
            public void DrawRectangle(Pen pen, RectangleF rect)
            {
                Graphics.DrawRectangle(pen, 
                    rect.X, rect.Y - Wnd.ScrollBarPosition,
                    rect.Width, rect.Height);
            }
            #endregion
			#region Method: void DrawBullet(Color, PointF, fRadius)
			public void DrawBullet(Color color, PointF pt, float Radius)
			{
				Brush brush = new SolidBrush(color);

				float xLeft = pt.X - Radius;
				float yTop = pt.Y - Radius - Wnd.ScrollBarPosition;
				float diameter = Radius * 2;

				Graphics.FillEllipse(brush, xLeft, yTop, diameter, diameter);
			}
			#endregion
			#region Method: void FillRectangle(Color clrBackground, RectangleF rect)
			public void FillRectangle(Color clrBackground, RectangleF rect)
            {
                Brush brush = new SolidBrush(clrBackground);

                RectangleF r = new RectangleF(rect.X, rect.Y - Wnd.ScrollBarPosition,
                    rect.Width, rect.Height);

                Graphics.FillRectangle(brush, r);
            }
            #endregion
            #region Method: void String(string s, Font font, Brush brush, PointF pt)
            public void String(string s, Font font, Brush brush, PointF pt)
            {
                Graphics.DrawString(s, font, brush, pt.X, pt.Y - Wnd.ScrollBarPosition,
                    StringFormat.GenericTypographic);
            }
            #endregion
            #region Method: void Line(Pen pen, float x1, float y1, float x2, float y2)
            public void Line(Pen pen, float x1, float y1, float x2, float y2)
            {
                Graphics.DrawLine(pen,
                    x1, y1 - Wnd.ScrollBarPosition,
                    x2, y2 - Wnd.ScrollBarPosition);
            }
            #endregion
            #region Method: void Line(Pen pen, PointF pt1, PointF pt2)
            public void Line(Pen pen, PointF pt1, PointF pt2)
            {
                Graphics.DrawLine(pen,
                    pt1.X, pt1.Y - Wnd.ScrollBarPosition,
                    pt2.X, pt2.Y - Wnd.ScrollBarPosition);
            }
            #endregion
            #region Method: void VertLine(Pen pen, float x, float y1, float y2)
            public void VertLine(Pen pen, float x, float y1, float y2)
            {
                Graphics.DrawLine(pen,
                    x, y1 - Wnd.ScrollBarPosition,
                    x, y2 - Wnd.ScrollBarPosition);
            }
            #endregion
            #region Method: void Image(Image image, PointF pt)
            public void Image(Image image, PointF pt)
            {
                Point point = new Point(
                    (int)pt.X, 
                    (int)(pt.Y - Wnd.ScrollBarPosition));

                Graphics.DrawImage(image, point);
            }
            #endregion

            #region Method: void Invalidate()
            public void Invalidate()
            {
                Wnd.Invalidate();
            }
            #endregion
            #region Method: void Invalidate(RectangleF rect)
            public void Invalidate(RectangleF rect)
            {
                RectangleF r = new RectangleF(
                    rect.X, rect.Y - Wnd.ScrollBarPosition,
                    rect.Width, rect.Height);
                Wnd.Invalidate(new Region(r), false);
            }
            #endregion
            #region Method: void InvalidateBlock(EBlock)
            delegate void InvalidateBlockCallback(EBlock block);
            public void InvalidateBlock(EBlock block)
                // This can be called with the Sel Timer wants to redraw the flashing
                // cursor; thus an asynchronic call from a different thread.
            {
                if (Wnd.InvokeRequired)
                {
                    InvalidateBlockCallback cb = new InvalidateBlockCallback(InvalidateBlock);
                    Wnd.Invoke(cb, new object[] { block });
                }
                else
                {
                    if (!Wnd.Focused)
                        return;

                    RectangleF r = new RectangleF(
                        block.Position.X,
                        block.Position.Y - Wnd.ScrollBarPosition,
                        block.Width + block.JustificationPaddingAdded,
                        block.Height);

                    Wnd.Invalidate(new Region(r), false);
                }
            }
            #endregion
            #region Method: void InvalidateParagraph(OWPara)
            public void InvalidateParagraph(OWPara para)
            {
                RectangleF r = new RectangleF(
                    para.Position.X,
                    para.Position.Y - Wnd.ScrollBarPosition,
                    para.Width,
                    para.Height);

                Wnd.Invalidate(new Region(r), false);
            }
            #endregion

            #region Method: float Measure(string sText, Font font)
            public float Measure(string sText, Font font)
            {
                StringFormat fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                return Graphics.MeasureString(sText, font, 1000, fmt).Width;
            }
            #endregion
            #region Method: void MultilineText(string s, Font font, Brush brush, RectangleF rect)
            public void MultilineText(string s, Font font, Brush brush, RectangleF rect)
            {
                RectangleF r = new RectangleF(rect.X, rect.Y - Wnd.ScrollBarPosition,
                    rect.Width, rect.Height);

                Graphics.DrawString(s, font, brush, r);
            }
            #endregion
        }
        #endregion
        #region Attr{g/s}: DrawBuffer Draw
        public DrawBuffer Draw
        {
            get
            {
                Debug.Assert(null != m_Draw);
                return m_Draw;
            }
            set
            {
                if (null != m_Draw)
                    Draw.Dispose();
                m_Draw = value;
            }
        }
        DrawBuffer m_Draw = null;
        #endregion
        #region Attr{g/s}: EditableBackgroundColor - The background color for words which can be edited. Default is White
        public Color EditableBackgroundColor
        {
            get
            {
                return m_EditableBackgroundColor;
            }
            set
            {
                m_EditableBackgroundColor = value;
            }
        }
        Color m_EditableBackgroundColor = Color.White;
        #endregion

        #region Method: DoLayout()
        public virtual void DoLayout()
        {
            // Make certain OnLoad() was called already (and thus, that we have a
            // Draw object, and that the EBlocks were all measured.
            if (!m_bLoaded)
                return;

            // Calculate the Lefts and Widths for the EContainer hierarchy
            Contents.CalculateContainerHorizontals();

			// Calculate the vertical layout
			float yTop = WindowMargins.Height;   // Top of the window, taking margin into account
			Contents.CalculateVerticals(yTop, false);

            // Now that the lines have been defined in the low-level OWPara's,
            // give each line a line number
            Contents.CalculateLineNumbers();

            // Set the ScrollBar, adding some (30 pixels) padding at the bottom
            Layout_SetupScrollBar((int)(yTop + Contents.Height));
        }
        #endregion
        #region Method: void PaintNoDataMessage(PaintEventArgs e)
        void PaintNoDataMessage(PaintEventArgs e)
        {
            if (!IsMainWindow)
                return;

            string sText = G.GetLoc_GeneralUI("NoDataToDisplayMsg", 
                "(There is no data to display. Use the Project menu to create a new Project or " +
				"open an existing project; or use Tools-Configure to set up both a Front and " +
				"a Target translation.)"); 

            Brush brush = new SolidBrush(Color.Black);
			Font font = new Font(SystemFonts.MenuFont.FontFamily, 
				SystemFonts.MenuFont.Size * 1.5F);

			Rectangle rect = new Rectangle(
				ClientRectangle.X + 20,
				ClientRectangle.Y + 20,
				ClientRectangle.Width - 40,
				ClientRectangle.Height - 40);

			Draw.MultilineText(sText, font, brush, rect);
        }
        #endregion
        #region Cmd: OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            // Background
            // The rectangle passed in will be in Client coordinates, not taking
            // into account the ScrollBar's position. So we must convert it here.
            Rectangle r = new Rectangle(e.ClipRectangle.X,
                e.ClipRectangle.Y + (int)ScrollBarPosition,
                e.ClipRectangle.Width, e.ClipRectangle.Height);
            // We do the background fill here, because we've set the
            // AllPaintingInWmPaint style in the constructor, so that all 
            // painting (including the background) goes through the double
            // buffer.
            Draw.FillRectangle(BackColor, r);

            // If there is no data, then display a help message
            if (Contents.Count == 0)
            {
                PaintNoDataMessage(e);
                e.Graphics.DrawImageUnscaled(Draw.DoubleBuffer, 0, 0);
                return;
            }

            // Paint the contents
            Contents.OnPaint(r);

			// Paint any controls (otherwise, the ones we don't paint due to the
			// clip rectangle, tend to stay around on the screen.)
			Contents.PaintControls();

            // Text Selection
            if (null != Selection)
                Selection.Paint();

            // Transfer from the DoubleBuffer to our actual window
            e.Graphics.DrawImageUnscaled(Draw.DoubleBuffer, 0, 0);
        }
        #endregion
        #region Cmd: OnPaintBackground - do nothing!
        protected override void OnPaintBackground(PaintEventArgs pevent)
            // We do the background painting as part of OnPaint; we don't want to 
            // process the base.OnPaintBackground event because it would cause
            // flicker.
        {
        }
        #endregion
        #region Cmd: OnSizeChanged
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            DoLayout();
        }
        #endregion
        #region Cmd: OnParagraphHeightChanged - recalculate positions to accomdate the new-sized paragraph
        public void OnParagraphHeightChanged(EContainer TopLevelContainer)
            // Each paragraph needs to recalculate its position and be redrawn
        {
            // Start at the top margin for the window
            float y = WindowMargins.Height;

            // We'll not redraw until we encounter the row that has changed; thus we'll use
            // bFound to indicate when we've located that row
            bool bFound = false;

            // Process through each row
            foreach (EContainer container in Contents.SubItems)
            {
                // Set the bFound flag once we encounter the target row
                if (container == TopLevelContainer)
                    bFound = true;

                // RePosition each row from the target to the end of the screen
                if (bFound)
                    container.CalculateVerticals(y, true);

                y += container.Height;
            }

            // Recalculate the lines numbers, as they may have changed
            Contents.CalculateLineNumbers();

            // Change the scrollbar to reflect the new height
            Layout_SetupScrollBar((int)y);

            // Tell the window to do some painting
            Invalidate();
        }
        #endregion

        // Line Numbers ----------------------------------------------------------------------
        #region CLASS: CLineNumberAttrs
        public class CLineNumberAttrs
        {
            #region Attr{g}: float ColumnWidth
            public float ColumnWidth
            {
                get
                {
                    Debug.Assert(-1 != m_fLineNumberColumnWidth);
                    return m_fLineNumberColumnWidth;
                }
            }
            float m_fLineNumberColumnWidth = -1;
            #endregion
            #region Attr{g}: Font Font
            public Font Font
            {
                get
                {
                    Debug.Assert(null != m_fLineNumberFont);
                    return m_fLineNumberFont;
                }
            }
            Font m_fLineNumberFont = null;
            #endregion
            #region VAttr{g}: Brush Brush
            public Brush Brush
            {
                get
                {
                    return m_Brush;
                }
                set
                {
                    m_Brush = value;
                }
            }
            Brush m_Brush;
            #endregion

            #region Constructor(Graphics)
            public CLineNumberAttrs(Graphics g)
            {
                // We'll use a fixed-space font so that the numbers line up
                float fSize = 10 * G.ZoomFactor;
                m_fLineNumberFont = new Font("Courier New", fSize);

                // Get the default Brush color; the window can overridei this
                m_Brush = Brushes.DarkGray;

                // Calculate the width required for the line number column
                // We'll measure a fat, 3-digit string plus a trailing space
                string s = "000 ";
                StringFormat fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                m_fLineNumberColumnWidth = g.MeasureString(s, Font, 1000, fmt).Width;
            }
            #endregion
        }
        #endregion
        #region Attr{g}: LineNumberAttrs LineNumberAttrs - init'd by LoadData
        public CLineNumberAttrs LineNumberAttrs
        {
            get
            {
                Debug.Assert(null != m_LineNumberAttrs);
                return m_LineNumberAttrs;
            }
        }
        public CLineNumberAttrs m_LineNumberAttrs = null;
        #endregion

        // Scroll Bar ------------------------------------------------------------------------
        #region Scroll Bar
        #region Attr{g}: VScrollBar ScrollBar
        VScrollBar ScrollBar
        {
            get
            {
                return m_ScrollBar;
            }
        }
        public VScrollBar m_ScrollBar;
        #endregion
        #region Attr{g/s}: float ScrollBarRange
        float ScrollBarRange
        {
            get
            {
                return (float)ScrollBar.Maximum;
            }
            set
            {
                // We fudge a bit and add a few pixels, just to be absolutely sure we 
                // can see everything if the user scrolls to the bottom.
                ScrollBar.Maximum = (int)(value + 10);
            }
        }
        #endregion
        #region Attr{g/s}: int ScrollBarPosition
        public float ScrollBarPosition
        {
            get
            {
                if (null == ScrollBar)
                    return 0;

                return (float)ScrollBar.Value;
            }
            set
            {
                if (null == ScrollBar)
                    return;

                int nRequested = (int)value;

                // Don't let it go beyond the possible range
                int nMax = (int)ScrollBarRange - (int)((float)Height * 0.7F);
                int n = Math.Min(nRequested, nMax);

                // Don't let it go below zero
                n = Math.Max(n, 0);

                ScrollBar.Value = n;
            }
        }
        #endregion
        #region Method: void Layout_SetupScrollBar(int yTotalHeight)
        void Layout_SetupScrollBar(int yTotalHeight)
        {
            if (null == ScrollBar)
                return;

            // We'll set the range to include a little padding, to make sure we
            // can always scroll everything into view.
            ScrollBarRange = yTotalHeight + 30;

            // A small change will be enough to move a normal line at a time. We're guessing
            // a value based on the Target Translation's normal (zoomed) line height.
            JParagraphStyle PStyle = DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_sfmParagraph);

            JWritingSystem ws = (null == DB.TargetTranslation) ?
                DB.StyleSheet.FindWritingSystem(DStyleSheet.c_Latin) :
                DB.TargetTranslation.WritingSystemVernacular;

            float fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                ws).LineHeightZoomed;
            ScrollBar.SmallChange = (int)fLineHeight;

            // A large change will scroll 4/5 of the window's height
            ScrollBar.LargeChange = (int)((float)Height * 0.80F);
        }
        #endregion
        #region Cmd: OnScrollBarValueChanged
        private void OnScrollBarValueChanged(Object sender, EventArgs e)
        {
            Invalidate();
        }
        #endregion
        #region Attr{g/s}: int ScrollPositionBufferMargin - maintain some space above/below in window
        public int ScrollPositionBufferMargin
        {
            get
            {
                return m_nScrollPositionBufferMargin;
            }
            set
            {
                m_nScrollPositionBufferMargin = value;
            }
        }
        int m_nScrollPositionBufferMargin = 0;
        #endregion
        #region Method: void ScrollSelectionIntoView()
        void ScrollSelectionIntoView()
        {
            if (null == ScrollBar)
                return;

            // Determine which point we need to be able to see
            if (null == Selection)
                return;
            Sel.SelPoint sp = Selection.Anchor;
            if (null != Selection.End)
                sp = Selection.End;

            // Make sure it is not above the top of the window
            int yTop = (int)Math.Max(sp.Word.Position.Y - ScrollPositionBufferMargin, 0.0F);
            if (ScrollBarPosition > yTop)
                ScrollBarPosition = yTop;

            // Make sure it is not below the bottom of the window
            int yBottom = (int)Math.Min(
                ScrollBarRange,
                sp.Word.Position.Y - Height + sp.Word.Height + ScrollPositionBufferMargin);
            if (ScrollBarPosition < yBottom)
                ScrollBarPosition = yBottom;
        }
        #endregion
        #endregion

        // Selection -------------------------------------------------------------------------
        #region CLASS: Sel
        public class Sel
        {
            // Attrs: Define the Selection ---------------------------------------------------
            #region CLASS SelPoint
            public class SelPoint
            {
                // Content Attrs -------------------------------------------------------------
                #region Attr{g}: int iBlock - the index into the paragraph's Blocks vector
                public int iBlock
                {
                    get
                    {
                        return m_iBlock;
                    }
                }
                int m_iBlock;
                #endregion
                #region Attr{g}: int iChar - the index into the EWord's text
                public int iChar
                {
                    get
                    {
                        return m_iChar;
                    }
                }
                int m_iChar;
                #endregion
                #region Attr{g/s}: EWord Word
                public EWord Word
                {
                    get
                    {
                        Debug.Assert(null != m_Word);
                        return m_Word;
                    }
                    set
                    {
                        m_Word = value;
                    }
                }
                EWord m_Word = null;
                #endregion

                // Secondary Attrs -----------------------------------------------------------
                #region VAttr{g}: float xFromWordLeft
                public float xFromWordLeft
                {
                    get
                    {
                        if (iChar == 0)
                            return 0;

                        return Word.GetXat(iChar);
                    }
                }
                #endregion
                #region VAttr{g}: float X - position of the selection
                public float X
                {
                    get
                    {
                        return Word.Position.X + xFromWordLeft;
                    }
                }
                #endregion
                #region VAttr{g}: DBasicText BasicText - the DBasicText which owns this word
                public DBasicText BasicText
                {
                    get
                    {
                        Debug.Assert(null != Word.Phrase);
                        Debug.Assert(null != Word.Phrase.BasicText);
                        return Word.Phrase.BasicText;
                    }
                }
                #endregion

                // Scaffolding ---------------------------------------------------------------
                #region Constructor(iBlock, iChar)
                public SelPoint(int iBlock, int iChar)
                {
                    Debug.Assert(iChar >= 0);

                    m_iBlock = iBlock;
                    m_iChar = iChar;
                }
                #endregion
            }
            #endregion
            #region Attr{g}: SelPoint Anchor
            public SelPoint Anchor
            {
                get
                {
                    Debug.Assert(null != m_Anchor);
                    return m_Anchor;
                }
            }
            SelPoint m_Anchor = null;
            #endregion
            #region Attr{g}: SelPoint End
            public SelPoint End
            {
                get
                {
                    return m_End;
                }
            }
            SelPoint m_End = null;
            #endregion
            #region Attr{g}: OWPara Paragraph
            public OWPara Paragraph
            {
                get
                {
                    Debug.Assert(null != m_Paragraph);
                    return m_Paragraph;
                }
            }
            OWPara m_Paragraph = null;
            #endregion
            #region VAttr{g}: OWWindow Window
            OWWindow Window
            {
                get
                {
                    Debug.Assert(null != Paragraph.Window);
                    return Paragraph.Window;

                }
            }
            #endregion
            #region VAttr{g}: bool IsInsertionPoint - T if End==null
            public bool IsInsertionPoint
            {
                get
                {
                    if (null == End)
                        return true;
                    return false;
                }
            }
            #endregion            
            #region VAttr{g}: bool IsContentSelection - T if selection has a span
            public bool IsContentSelection
            {
                get
                {
                    if (null == End)
                        return false;

                    if (Anchor.iBlock != End.iBlock)
                        return true;
                    if (Anchor.iChar != End.iChar)
                        return true;

                    return false;
                }
            }
            #endregion
            #region VAttr{g}: bool IsInsertionIcon
            public bool IsInsertionIcon
            {
                get
                {
                    return Anchor.Word.IsInsertionIcon;
                }
            }
            #endregion
            #region VAttr{g}: bool SelectionIsInForwardDirection
            public bool SelectionIsInForwardDirection
            {
                get
                {
                    // Shouldn't call this if we don't have a selection
                    Debug.Assert(null != End);

                    // Check if they are different blocks
                    if (End.iBlock > Anchor.iBlock)
                        return true;
                    if (End.iBlock < Anchor.iBlock)
                        return false;

                    // Otherwise, then it depends on the direction within the block
                    if (End.iChar > Anchor.iChar)
                        return true;
                    return false;
                }
            }
            #endregion
            #region VAttr{g}: SelPoint First - Returns Anchor or End, whichever occurs First
            public SelPoint First
            {
                get
                {
                    if (IsInsertionPoint)
                        return Anchor;

                    if (SelectionIsInForwardDirection)
                        return Anchor;
                    return End;
                }
            }
            #endregion
            #region VAttr{g}: SelPoint Last - Returns Anchor or End, whichever occurs Last
            public SelPoint Last
            {
                get
                {
                    if (IsInsertionPoint)
                        return Anchor;

                    if (SelectionIsInForwardDirection)
                        return End;
                    return Anchor;
                }
            }
            #endregion
            #region VAttr{g}: List<EContainer> ContainerStack
            public List<EContainer> ContainerStack
            {
                get
                {
                    List<EContainer> v = new List<EContainer>();

                    // Start with the owner of the selection word
                    EContainer container = Anchor.Word.Owner;

                    while (null != container)
                    {
                        // We add it to the top of the list, so that it is working
                        // top-to-bottom fashion.
                        v.Insert(0, container);

                        // Move to the next one up the hierarhcy
                        container = container.Owner;
                    }

                    return v;
                }
            }
            #endregion
            #region VAttr{g}: ArrayList ContainerIndicesStack
            public ArrayList ContainerIndicesStack
            {
                get
                {
                    ArrayList aiStack = new ArrayList();

                    // Start with this word
                    EItem item = Anchor.Word;

                    // Work up until we get the Root
                    do
                    {
                        // Iterate: Move to the owner of the item we just processed
                        item = item.Owner;

                        // Find this item within its own owner
                        if (null == item.Owner)
                            break;
                        int i = item.Owner.Find(item);
                        Debug.Assert(-1 != i);

                        // Add it to the array, at the top, so that top-to-bottom works
                        // downward through the hierarchy.
                        aiStack.Insert(0, i);

                    } while (true);

                    return aiStack;
                }
            }
            #endregion

            #region VAttr{g}: string SelectionString
            public string SelectionString
            {
                get
                {
                    if (IsInsertionPoint)
                        return "";

                    if (IsInsertionIcon)
                        return "";

                    if (Anchor.Word == End.Word)
                    {
                        int iPos = First.iChar;
                        int c = Last.iChar - First.iChar;
                        return Anchor.Word.Text.Substring(iPos, c);
                    }

                    string s = "";
                    for (int i = First.iBlock; i <= Last.iBlock; i++)
                    {
                        if (i == First.iBlock)
                            s += First.Word.Text.Substring(First.iChar);
                        else if (i == Last.iBlock)
                            s += Last.Word.Text.Substring(0, Last.iChar);
                        else
                            s += (Paragraph.SubItems[i] as EBlock).Text;
                    }
                    return s;
                }
            }
            #endregion
            #region VAttr{g}: bool IsInsertionPoint_AtParagraphBeginning
            public bool IsInsertionPoint_AtParagraphBeginning
            {
                get
                {
                    // Must be an insertion point
                    if (!IsInsertionPoint)
                        return false;

                    // Must be the first editable block
                    for (int ib = 0; ib < Paragraph.SubItems.Length; ib++)
                    {
                        if (Paragraph.SubItems[ib] as EWord != null)
                        {
                            if (ib != Anchor.iBlock)
                                return false;
                            break;
                        }
                    }

                    // Must be the first position in the block
                    if (Anchor.iChar != 0)
                        return false;

                    // All tests passed; we're at the beginning
                    return true;
                }
            }
            #endregion
            #region VAttr{g}: bool IsInsertionPoint_AtParagraphEnding
            public bool IsInsertionPoint_AtParagraphEnding
            {
                get
                {
                    // Must be an insertion point
                    if (!IsInsertionPoint)
                        return false;

                    // Must be the last editable block
                    for (int ib = Paragraph.SubItems.Length - 1; ib >= 0; ib--)
                    {
                        if (Paragraph.SubItems[ib] as EWord != null)
                        {
                            if (ib != Anchor.iBlock)
                                return false;
                            break;
                        }
                    }

                    // Must be the last position in the block
                    EWord word = Paragraph.SubItems[Anchor.iBlock] as EWord;
                    if (Anchor.iChar != word.Text.Length)
                        return false;

                    // All tests passed; we're at the beginning
                    return true;
                }
            }
            #endregion

            // Attrs: DBasicText (DBT) related -----------------------------------------------
            #region VAttr{g}: DBasicText DBT - The DBasicText that is referenced by this Sel
            public DBasicText DBT
            {
                get
                {
                    return Anchor.Word.Phrase.BasicText;
                }
            }
            #endregion
            #region VAttr{g}: int DBT_iBlockFirst - The first iBlock from this DBasicText
            public int DBT_iBlockFirst
            {
                get
                {
                    // Because a Selection is immutable, we only have to calculate
                    // this the first time; thereafter we just return the value.
                    // Thus there is no performance hit for calling this multiple times.
                    if (m_iBlockFirst != -1)
                        return m_iBlockFirst;

                    // Start at the first block of the selection, and scan backwards
                    // towards the beginning of the paragraph.
                    for (int i = First.iBlock; i >= 0; i--)
                    {
                        // Attempt to cast to an EWord. 
                        EWord word = Paragraph.SubItems[i] as EWord;
                        
                        // As long as we keep finding EWords, we are still in the same 
                        // DBT. Otherwise, we've found the boundary, so return the
                        // most recent successful cast.
                        if (null == word)
                            return m_iBlockFirst;

                        // Remember this position as a successful cast.
                        m_iBlockFirst = i;
                    }

                    // If we exited the loop, then we are at the first block
                    // in the paragraph (iFirst should == 0)
                    return m_iBlockFirst;
                }
            }
            int m_iBlockFirst = -1;
            #endregion
            #region VAttr{g}: int DBT_iBlockLast - The last iBlock from this DBasicText
            public int DBT_iBlockLast
            {
                get
                {
                    // Because a Selection is immutable, we only have to calculate
                    // this the first time; thereafter we just return the value.
                    // Thus there is no performance hit for calling this multiple times.
                    if (m_iBlockLast != -1)
                        return m_iBlockLast;

                    // Start at the first block of the selection, and scan forwards
                    // towards the end of the paragraph.
                    for (int i = Last.iBlock; i < Paragraph.SubItems.Length; i++)
                    {
                        // Attempt to cast to an EWord. 
                        EWord word = Paragraph.SubItems[i] as EWord;

                        // As long as we keep finding EWords, we are still in the same 
                        // DBT. Otherwise, we've found the boundary, so return the
                        // most recent successful cast.
                        if (null == word)
                            return m_iBlockLast;

                        // Remember this position as a successful cast.
                        m_iBlockLast = i;
                    }

                    // If we exited loop, then we are at the final block in the paragraph.
                    return m_iBlockLast;
                }
            }
            int m_iBlockLast = -1;
            #endregion
            #region Method: int DBT_iChar(SelPoint sp)
            public int DBT_iChar(SelPoint sp)
            {
                int iPos = 0;

                for (int i = DBT_iBlockFirst; i < sp.iBlock; i++)
                {
                    EWord word = Paragraph.SubItems[i] as EWord;
                    Debug.Assert(null != word);
                    iPos += word.Text.Length;
                }

                iPos += sp.iChar;

                return iPos;
            }
            #endregion
            #region VAttr{g}: int DBT_iCharFirst
            public int DBT_iCharFirst
            {
                get
                {
                    return DBT_iChar(First);
                }
            }
            #endregion
            #region VAttr{g}: int DBT_iCharLast
            public int DBT_iCharLast
            {
                get
                {
                    return DBT_iChar(Last);
                }
            }
            #endregion
            #region VAttr{g}: int DBT_iCharCount
            public int DBT_iCharCount
            {
                get
                {
                    return DBT_iCharLast - DBT_iCharFirst;
                }
            }
            #endregion
            #region VAttr{g}: int DBT_BlockCount
            public int DBT_BlockCount
            {
                get
                {
                    return 1 + DBT_iBlockLast - DBT_iBlockFirst;
                }
            }
            #endregion
            #region SMethod: static Sel CreateSel(OWPara paragraph, DBasicText DBT, int iPos)
            static public Sel CreateSel(OWPara paragraph, DBasicText DBT, int iPos)
                // Create a SelectionPoint given the DBasicText and a position in it. We assume
                // that the EWords are already in the paragraph.
            {
                Debug.Assert(iPos >= 0);

                // We''ll keep track of the EWord we're working on here
                int iBlock = 0;
                EWord word = null;

                // Scan for the first EWord for the DBT
                for (; iBlock < paragraph.SubItems.Length; iBlock++)
                {
                    word = paragraph.SubItems[iBlock] as EWord;
                    if (word != null && word.Phrase.BasicText == DBT)
                        break;
                }

                // Proceed through the words, working our way through the iPos
                for (; iBlock < paragraph.SubItems.Length; iBlock++)
                {
                    word = paragraph.SubItems[iBlock] as EWord;
                    if (null == word || word.Phrase.BasicText != DBT)
                    {
                        if (iBlock > 0 && (paragraph.SubItems[iBlock - 1] as EWord) != null)
                        {
                            return new Sel(paragraph, iBlock - 1,
                                (paragraph.SubItems[iBlock - 1] as EWord).Text.Length);
                        }
                        return null;
                    }

                    if (iPos > word.Text.Length)
                    {
                        iPos -= word.Text.Length;
                    }
                    else
                    {
                        return new Sel(paragraph, iBlock, iPos);
                    }
                }

                return null;
            }
            #endregion
            #region SMethod: static Sel CreateSel(OWPara paragraph, DBasicText DBT, int iPos1, int iPos2)
            static public Sel CreateSel(OWPara paragraph, DBasicText DBT, int iPos1, int iPos2)
            // Create a SelectionPoint given the DBasicText and a position in it. We assume
            // that the EWords are already in the paragraph.
            {
                // We''ll keep track of the EWord we're working on here
                int iBlock = 0;
                EWord word = null;

                // Scan for the first EWord for the DBT
                for (; iBlock < paragraph.SubItems.Length; iBlock++)
                {
                    word = paragraph.SubItems[iBlock] as EWord;
                    if (word != null && word.Phrase.BasicText == DBT)
                        break;
                }

                SelPoint sp1 = null;
                SelPoint sp2 = null;

                // Proceed through the words, working our way through the iPos's
                for (; iBlock < paragraph.SubItems.Length; iBlock++)
                {
                    // Retrieve the EWord
                    word = paragraph.SubItems[iBlock] as EWord;
                    if (null == word || word.Phrase.BasicText != DBT)
                        return null;

                    // Set the first SelPoint if possible
                    if (iPos1 > word.Text.Length)
                        iPos1 -= word.Text.Length;
                    else if (null == sp1)
                        sp1 = new SelPoint(iBlock, iPos1);

                    // Set the second SelPoint if possible
                    if (iPos2 > word.Text.Length)
                        iPos2 -= word.Text.Length;
                    else if (null == sp2)
                        sp2 = new SelPoint(iBlock, iPos2);

                    // Once both SelPoints are set, we can create the selection
                    if (null != sp1 && null != sp2)
                        return new Sel(paragraph, sp1, sp2);
                }

                return null;
            }
            #endregion

            // Timer -------------------------------------------------------------------------
            // TODO: MSDN says Windows.Form.Timer is in the same thread as the Form, and thus
            // the better option to use.
            System.Windows.Forms.Timer m_Timer;
            const int c_nTimerInterval = 600;
            bool m_bFlashOn = false;
            #region Method: OnTimerTick(...)
            void OnTimerTick(object sender, EventArgs e) // System.Timers.ElapsedEventArgs e)
            {
                if (!Window.Visible)
                    return;
                if (!IsInsertionPoint && !IsInsertionIcon)
                    return;

                // Toggle the insertion point between Off and On
                m_bFlashOn = !m_bFlashOn;

                // Invalidate the word that contains the insertion point
                Window.Draw.InvalidateBlock(Anchor.Word);

                // Let the window do anything it wants
                Window.OnCursorTimerTick();
            }
            #endregion
            #region Method: void SetupTimer()
            public void SetupTimer()
            {
                m_Timer = new System.Windows.Forms.Timer();
                m_Timer.Tick += new EventHandler(OnTimerTick);

                m_Timer.Interval = c_nTimerInterval;
                m_Timer.Start();
            }
            #endregion

            // Misc Methods ------------------------------------------------------------------
            #region Method: void Paint()
            public void Paint()
                // Here, we only paint the verticle insertion point. If the IP is supposed
                // to be in the "off" state, then it is turned off because we re-draw the
                // underlying Word.
            {
                if (IsInsertionIcon & Window.Focused)
                {
                    if (m_bFlashOn)
                        Anchor.Word.PaintSelection(-1, -1);
                    return;
                }

                if (m_bFlashOn && IsInsertionPoint && Window.Focused)
                {
                    Pen pen = new Pen(System.Drawing.Color.Black, 2);

                    EWord word = Anchor.Word;

                    float x = word.Position.X + Anchor.xFromWordLeft;

                    // Adjust off of the boundary, so we can make certain it is in the
                    // word's drawing area (rounding areas on the Screen can affect this.)
                    if (Anchor.iChar == 0)
                        x++;
                    if (Anchor.iChar == Anchor.Word.Text.Length)
                        x--;

                    Window.Draw.VertLine(pen, x, word.Position.Y,
                        word.Position.Y + word.Height);

                    return;
                }

                // If a selection, the flash is irrelevant; we just want to overlay
                // the selection on top of the drawing
                if (!IsInsertionPoint)
                {
                    // Remember these for a tiny performance boost
                    SelPoint SelFirst = First;
                    SelPoint SelLast = Last;

                    // Selection is within a single Word: simple and we're done
                    if (SelFirst.Word == SelLast.Word)
                    {
                        First.Word.PaintSelection(SelFirst.iChar, SelLast.iChar);
                        return;
                    }

                    // Paint the first partial word
                    First.Word.PaintSelection(SelFirst.iChar, -1);

                    // Paint the whole words in-between
                    for (int i = SelFirst.iBlock + 1; i < SelLast.iBlock; i++)
                    {
                        EWord word = Paragraph.SubItems[i] as EWord;
                        Debug.Assert(null != word);
                        if (null != word)
                            word.PaintSelection(-1, -1);
                    }

                    // Paint the final partial word
                    Last.Word.PaintSelection(-1, SelLast.iChar);
                }
            }
            #endregion
            #region Method: bool IsWithinContentSelection(PointF)
            public bool IsWithinContentSelection(PointF pt)
            {
                // Must have a content selection
                if (!IsContentSelection)
                    return false;

                // Get the EWord the mouse if over
                EBlock block = Window.Contents.GetBlockAt(pt);
                if (null == block)
                    return false;
                EWord word = block as EWord;
                if (null == word)
                    return false;

                // Get the position within this word
                int iChar = word.GetCharacterIndex(pt);

                // Check for the index before or after within the boundary words
                if (word == First.Word && iChar < First.iChar)
                    return false;
                if (word == Last.Word && iChar > Last.iChar)
                    return false;

                // Now, having dealt with the boundaries; if we get a match with any
                // of the selected blocks, we know we have a Drag starting
                for (int i = First.iBlock; i <= Last.iBlock; i++)
                {
                    if (word == Paragraph.SubItems[i] as EWord)
                        return true;
                }

                return false;
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            static string s_KeyboardName = "";
            #region Constructor(OWPara, iBlock, iChar)
            public Sel(OWPara paragraph, int iBlock, int iChar)
                : this(paragraph, new SelPoint(iBlock, iChar), null)
            {
            }
            #endregion
            #region Constructor(OWPara, SelPoint Anchor)
            public Sel(OWPara paragraph, SelPoint Anchor)
                : this(paragraph, Anchor, null)
            {
            }
            #endregion
            #region Constructor(OWPara, SelPoint Anchor, SelPoint End)
            public Sel(OWPara _Paragraph, SelPoint _Anchor, SelPoint _End)
            {
                // Set the Paragraph attribute
                m_Paragraph = _Paragraph;
                Debug.Assert(null != Paragraph);

                // Set the Anchor and the Anchor.Word attribute
                m_Anchor = _Anchor;
                Anchor.Word = Paragraph.SubItems[Anchor.iBlock] as EWord;

                // Set the End and the End.Word attribute
                m_End = _End;
                if (null != End)
                    End.Word = Paragraph.SubItems[End.iBlock] as EWord;

                // If Anchor == End, then we have a Insertion Point.
                if (null != End && Anchor.Word == End.Word && Anchor.iChar == End.iChar)
                    m_End = null;
                if (Anchor == End)
                    m_End = null;

                // Check for a potential InsertionIcon, and modify the selection to
                // reflect it if so
                if (Anchor.Word.Text == EWord.c_chInsertionSpace.ToString())
                {
                    m_Anchor = new SelPoint(Anchor.iBlock, 0);
                    Anchor.Word = Paragraph.SubItems[Anchor.iBlock] as EWord;
                    m_End = new SelPoint(Anchor.iBlock, 1);
                    End.Word = Paragraph.SubItems[End.iBlock] as EWord;
                }

                // Get the selection drawn as fast as possible. The Timer will then make
                // the flashing happen. (That is, we draw here, because if we wait on
                // the timer we might have a 0.5 second delay, which is disconcerting
                // to the user if he is moving around doing a mouse click.
                m_bFlashOn = true;
                if (IsInsertionPoint || IsInsertionIcon)
                    Window.Draw.InvalidateBlock(Anchor.Word);
                else
                    Window.Draw.InvalidateParagraph(Paragraph);

                // Set the keyboard if necessary (we check the writing system of this
                // new selection, and switch the keyboard if its name has changed.)
                if (m_Paragraph.WritingSystem.KeyboardName != s_KeyboardName)
                {
                    s_KeyboardName = m_Paragraph.WritingSystem.KeyboardName;
                    if (string.IsNullOrEmpty(s_KeyboardName))
                        KeyboardController.DeactivateKeyboard();
                    else
                    {
                        // The following code is for debugging/diagnostics; to see what keyboards
                        // are defined on the system.
                        // 
                        // List<KeyboardController.KeyboardDescriptor> v =
                        //    KeyboardController.GetAvailableKeyboards(KeyboardController.Engines.All);
                        // end diagnostic code

                        KeyboardController.ActivateKeyboard(s_KeyboardName);
                    }
                }

                // Create a timer and start the on/off flashing
                SetupTimer();
            }
            #endregion
            #region Method: void Dispose()
            public void Dispose()
                // Hide the previous selection, since it is no longer valid
            {
                m_Timer.Stop();
                m_Timer.Dispose();

                // Insertion Point
                if (IsInsertionPoint)
                {
                    Window.Draw.InvalidateBlock(Anchor.Word);
                    return;
                }

                // Selection
                Window.Draw.InvalidateParagraph(Paragraph);
            }
            #endregion
        }
        #endregion
        #region Attr{g/s}: Sel Selection - "set" disposes (and thus erases) the previous one
        public Sel Selection
        {
            get
            {
                return m_Selection;
            }
            set
            {
                if (null != m_Selection)
                    m_Selection.Dispose();

                m_Selection = value;

                ScrollSelectionIntoView();

                if (null != m_Selection)
                    Secondary_OnSelectionChanged( m_Selection.DBT );

                G.App.EnableItalicsButton();
            }
        }
        Sel m_Selection = null;
        #endregion
        #region Attr{g}: bool HasSelection
        public bool HasSelection
        {
            get
            {
                return (null != Selection);
            }
        }
        #endregion
        #region VMethod: OWBookmark CreateBookmark()
        public virtual OWBookmark CreateBookmark()
        {
            return new OWBookmark(this);
        }
        #endregion

        // Command Routing -------------------------------------------------------------------
        #region Method: override bool IsInputKey(Keys keyData) - T if EPanel processes this key
        protected override bool IsInputKey(Keys keyData)
        // It is necessary to tell the system which keys the EPanel will process,
        // otherwise these will be processed by the owning Form, and EPanel will
        // not see them at all.
        {
            Keys[] vKeysWeProcess = 
				{
                    Keys.Tab,
					Keys.Right,
					Keys.Left,
					Keys.Up,
					Keys.Down,
                    Keys.Shift | Keys.Tab,
					Keys.Shift | Keys.Right,
					Keys.Shift | Keys.Left,
					Keys.Shift | Keys.Up,
					Keys.Shift | Keys.Down
				};

            foreach (Keys k in vKeysWeProcess)
            {
                if (k == keyData)
                    return true;
            }

            return base.IsInputKey(keyData);
        }
        #endregion
        #region Cmd: OnKeyDown
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Clear the X value for UP and DOWN arrows if appropriate
            m_LineUpDownX.CheckKeyDown(e);

            // SHIFT + CTRL + n
            if (e.Modifiers == (Keys.Shift | Keys.Control))
            {
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.Home:  cmdExtendFarLeft();   return;
                    case Keys.End:   cmdExtendFarRight();  return;
                    case Keys.Right: cmdExtendWordRight(); return;
                    case Keys.Left:  cmdExtendWordLeft();  return;
                    case Keys.Tab: G.App.CycleFocusToPreviousWindow(); return;
                }
                e.Handled = false;
            }

            // CTRL + n
            if (e.Modifiers == Keys.Control)
                // Note that Ctrl-C, Ctrl-X, Ctrl-V are passed via DotNetBar in the
                // UserCommandHandler method in OurWordMain, and thus we don't do
                // them here. (If we do them here, then they will be called twice.)
            {
                e.Handled = true;

                switch (e.KeyCode)
                {
                    case Keys.Home:   cmdMoveTop();       return;
                    case Keys.End:    cmdMoveBottom();    return;
                    case Keys.Left:   cmdMoveWordLeft();  return;
                    case Keys.Right:  cmdMoveWordRight(); return;
                    case Keys.Insert: cmdCopy();          return;
                    case Keys.Delete: cmdDelete();        return;
                    case Keys.Tab: G.App.CycleFocusToNextWindow(); return;
                }

 				e.Handled = false;
           }

            // SHIFT + n
            if (e.Modifiers == Keys.Shift)
            {
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.Left:   cmdExtendCharLeft();  return;
                    case Keys.Right:  cmdExtendCharRight(); return;
                    case Keys.Home:   cmdExtendLineBegin(); return;
                    case Keys.End:    cmdExtendLineEnd();   return;
                    case Keys.Up:     cmdExtendLineUp();    return;
                    case Keys.Down:   cmdExtendLineDown();  return;
                    case Keys.Insert: cmdPaste();           return;
                    case Keys.Delete: cmdCut();             return;
                    case Keys.Tab: cmdMovePreviousBasicText(); return;
                }
                e.Handled = false;
            }

            // n
            if (e.Modifiers == Keys.None)
            {
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.Left:   cmdMoveCharLeft();      return;
                    case Keys.Right:  cmdMoveCharRight();     return;
                    case Keys.Home:   cmdMoveLineBegin();     return;
                    case Keys.End:    cmdMoveLineEnd();       return;
                    case Keys.Up:     cmdMoveLineUp();        return;
                    case Keys.Down:   cmdMoveLineDown();      return;
                    case Keys.Delete: cmdDelete();            return;
                    case Keys.Back:   cmdBackspace();         return;
                    case Keys.Enter:  cmdEnter();             return;
                    case Keys.Tab:    cmdMoveNextBasicText(); return;
                }
                e.Handled = false;
            }

            base.OnKeyDown(e);
        }
        #endregion

        // Text Changes ----------------------------------------------------------------------
        #region Method: bool HandleLockedFromEditing()
        virtual public bool HandleLockedFromEditing()
        {
            // If not locked from editing, nothing further is needed
            if (null == Selection)
                return false;
            OWPara SelectedParagraph = Selection.Paragraph;
            if (null == SelectedParagraph)
                return false;
            if (!SelectedParagraph.IsLocked)
                return false;

            // If this is not the main window, then we do nothing. If 
            // we don't have a pointer in the MainWindow attr, then we
            // can infer that This is the main window.
            if (null != MainWindow)
                return false;

            // Retrieve the book we're working on
            DBook book = DB.TargetBook;

            // Have we displayed the "Book is locked" message yet? Do so if not.
            if (!book.UserHasSeenLockedMessage)
            {
                Messages.BookIsLocked( book.DisplayName );
                book.UserHasSeenLockedMessage = true;
            }
            else
                TypingErrorBeep();

            // Tell the caller (e.g., cmdInsertChar, cmdDelete) to not do anything
            return true;
        }
        #endregion
        #region Method: string GetCurrentParagraphStyle()
        public string GetCurrentParagraphStyle()
        {
            if (null == Selection)
                return null;

            OWPara p = Selection.Paragraph;

            DParagraph paragraph = p.DataSource as DParagraph;
            if (null != paragraph)
                return paragraph.StyleAbbrev;

            return null;
        }
        #endregion
        #region Cmd: cmdEnter - react to the Enter key, either split a paragraph, or move to the next paragraph
        void cmdEnter()
        {
            if (null == Selection)
                return;

            // If paragraph restructuring is turned off, then we just move to the next paragraph
            if (!Selection.Paragraph.CanRestructureParagraphs)
            {
                cmdMoveNextParagraph();
                return;
            }

            // Otherwise, we want to make a paragraph break (split the paragraph)
            cmdSplitParagraph();
        }
        #endregion

        // Undo- / Redo-able actions ---------------------------------------------------------
        // Paragraph Manipulations
        #region URCmd: cmdSplitParagraph
        public void cmdSplitParagraph()
        {
            (new SplitParagraphAction(this)).Do();
        }
        #endregion
        #region URCmd: cmdChangeParagraphTo(sStyleAbbrev)
        public void cmdChangeParagraphTo(string sStyleAbbrev)
        {
            (new ChangeParagraphStyleAction(this, sStyleAbbrev)).Do();
        }
        #endregion

        // Footnotes
        #region URCmd: cmdInsertFootnote
        public void cmdInsertFootnote()
        {
            (new InsertFootnoteAction(this)).Do();
        }
        #endregion
        #region URCmd: cmdDeleteFootnote
        public void cmdDeleteFootnote()
        {
            (new DeleteFootnoteAction(this)).Do();
        }
        #endregion
        #region Can: canInsertFootnote
        public bool canInsertFootnote
        {
            get
            {
                OWWindow.Sel selection = Selection;
                if (null == selection)
                    return false;

                return selection.Paragraph.CanInsertFootnote;
            }
        }
        #endregion
        #region Can: canDeleteFootnote
        public bool canDeleteFootnote
        {
            get
            {
                OWWindow.Sel selection = Selection;
                if (null == selection)
                    return false;

                return selection.Paragraph.CanDeleteFootnote;
            }
        }
        #endregion

        // Deletions
        #region URCmd: cmdDelete
        public void cmdDelete()
        {
            if (HandleLockedFromEditing())
                return;

            if (!HasSelection)
                return;

            if (Selection.Paragraph.JoinParagraph(OWPara.DeleteMode.kDelete))
                return;

            (new DeleteAction(this, DeleteAction.DeleteMode.kDelete)).Do();
        }
        #endregion
        #region URCmd: cmdBackspace
        public void cmdBackspace()
        {
            if (HandleLockedFromEditing())
                return;

            if (!HasSelection)
                return;

            if (Selection.Paragraph.JoinParagraph(OWPara.DeleteMode.kBackSpace))
                return;

            (new DeleteAction(this, DeleteAction.DeleteMode.kBackSpace)).Do();
        }
        #endregion
        #region URCmd: cmdCut
        public void cmdCut()
        {
            if (HandleLockedFromEditing())
                return;

            if (!HasSelection)
                return;

            (new DeleteAction(this, DeleteAction.DeleteMode.kCut)).Do();
        }
        #endregion
        #region URCmd: cmdCopy
        public void cmdCopy()
        {
            if (!HasSelection)
                return;

            (new DeleteAction(this, DeleteAction.DeleteMode.kCopy)).Do();
        }
        #endregion

        // Insertions
        #region URCmd: OnKeyPress - most data entry
        protected override void OnKeyPress(KeyPressEventArgs e)
        {

            if (!Char.IsControl(e.KeyChar) && HasSelection)
            {
                (new TypingAction(this, e.KeyChar)).Do();
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }
        #endregion
        #region URCmd: cmdPaste
        public void cmdPaste()
        {
            if (!HasSelection)
                return;

            (new PasteAction(this)).Do();
        }
        #endregion

        // Other Edits
        #region URCmd: cmdToggleItalics
        public void cmdToggleItalics()
        {
            if (canItalic)
                (new ItalicsAction(this)).Do();
        }
        #endregion
        #region Can: canItalic
        public bool canItalic
        {
            get
            {
                OWWindow.Sel selection = Selection;
                if (null == selection)
                    return false;

                return selection.Paragraph.CanItalic;
            }
        }
        #endregion

        // Mousing ---------------------------------------------------------------------------
        #region DOC: Mouse Behavior
        /* Mouse Behavior: Selecting vs Drag/Drop
		 * 
		 * We work with the following mutually exclusive states:
		 * 1. SelectingText
		 * 2. PotentialDragDrop
		 * 3. ConfirmedDragDrop
		 *  
		 * OnMouseDown
		 *   If (1) there is already a selection, and (2) we are clicking inside it,
		 *      State = PotentialDragDrop
		 *   Else
		 *      State = SelectingText
		 *      Create a TIP Anchor at the click coordinates
		 * 
		 * OnMouseMove
		 *   If (State == SelectingText)
		 *      Selection EndPoint is extended to the coordinates
		 *   Else If (State == PotentialDragDrop)
		 *      State = ConfirmedDragDrop
		 * 
		 * OnMouseUp
		 *   If (State == SelectingText)
		 *      Finish making the selection
		 *   Else if (State == PotentialDragDrop)
		 *      Create a TIP where the click was released
		 *   Else (State == ConfirmedDragDrop)
		 *      Process the Drop action.
		 * 
		 **/
        #endregion
        enum MouseStates { kNone, kSelectingText, kPotentialDragDrop, kConfirmedDragDrop };
        MouseStates m_MouseState = MouseStates.kNone;
        bool m_bMouseDown = false;
        #region Cmd: OnMouseDown
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            // We need to remember that it is down for future MouseMove events
            m_bMouseDown = true;

            // The point of the mouse click, accounting for the scroll bar
            PointF pt = new PointF(e.X, e.Y + ScrollBarPosition);

            // Is this a possible DragAndDrop operation? It is if (1) there is already
            // a selection, and (2) the mouse click is within it.
 //           if (null != Selection && Selection.IsWithinContentSelection(pt))
 //           {
 //               m_MouseState = MouseStates.kPotentialDragDrop;
 //               // TODO: Chamge the cursor to a DragDrop thing
 //           }

            // Get the EBlock we're over, and perform that block's action
            EBlock block = Contents.GetBlockAt(pt);
            if (null == block)
                return;
            block.cmdLeftMouseClick(pt);
            m_MouseState = MouseStates.kSelectingText;
        }
        #endregion
        #region Cmd: OnMouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // The point of the mouse, accounting for the scroll bar
            PointF pt = new PointF(e.X, e.Y + ScrollBarPosition);

            // Get the EBlock we are currently over
            if (m_MouseState == MouseStates.kNone)
            {
                EBlock block = Contents.GetBlockAt(pt);
                if (null != block)
                {
                    Cursor = block.MouseOverCursor;
                    if (null != m_ToolTip)
                        m_ToolTip.SetBlock(block);
                }
                else
                {
                    Cursor = Cursors.Default;
                    if (null != m_ToolTip)
                        m_ToolTip.ClearBlock();
                }
            }

            if (m_MouseState == MouseStates.kSelectingText)
            {
                EBlock block = Contents.GetBlockAt(pt);
                if (null != block)
                    block.cmdMouseMove(pt);
            }
        }
        #endregion
        #region Cmd: OnMouseUp
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // If the mouse button isn't being held down, then we're not interested
            // in further processing. (But one would think that this method would not
            // fire unless the button was indeed down, right?)
            if (!m_bMouseDown)
                return;
            m_bMouseDown = false;

            // If we are selecting text, then we need to end doing the selection
            if (m_MouseState == MouseStates.kSelectingText)
            {
                UpDownX.Clear();
            }

            m_MouseState = MouseStates.kNone;
        }
        #endregion
        #region Cmd: OnMouseLeave
        protected override void OnMouseLeave(EventArgs e)
        {
            // Make sure we've set the mouse back to an arrow
            Cursor = Cursors.Default;
            m_MouseState = MouseStates.kNone;

            base.OnMouseLeave(e);
        }
        #endregion
        #region Cmd: OnMouseDoubleClick(e)
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            // The point of the mouse, accounting for the scroll bar
            PointF pt = new PointF(e.X, e.Y + ScrollBarPosition);

            // Get the EBlock we are currently over
            EBlock block = Contents.GetBlockAt(pt);

            // Let the block handle it
            if (null != block)
                block.cmdLeftMouseDoubleClick(pt);

            m_MouseState = MouseStates.kNone;
        }
        #endregion
        #region Cmd: OnMouseWheel
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (null == m_ScrollBar)
                return;

            // We'll take a Wheel Delta as being the equivalent of one line of text multipled
            // by this figure
            int nWheelMultiplier = 5;

            // Determine whether we are going up or down by the sign
            if (e.Delta > 0)
                ScrollBarPosition -= (ScrollBar.SmallChange * nWheelMultiplier);
            else
                ScrollBarPosition += (ScrollBar.SmallChange * nWheelMultiplier);

            base.OnMouseWheel(e);
        }
        #endregion
        OWToolTip m_ToolTip;

        // Movement --------------------------------------------------------------------------
        #region Selection Movement & Extend Commands
        #region Cmd: cmdMoveTop - move cursor to first EWord in window
        void cmdMoveTop()
        {
            Contents.Select_FirstWord();
            Focus();
        }
        #endregion
        #region Cmd: cmdMoveBottom - move cursor to final EWord in window
        void cmdMoveBottom()
        {
            Contents.Select_LastWord_End();
        }
        #endregion
        #region Cmd: cmdMoveCharLeft - move cursor one char to the left
        public void cmdMoveCharLeft()
        {
            // If there is no Selection object, then we can't do anything (we are in a
            // wierd state.
            if (null == Selection)
                return;

            // If we have a multi-character selection, then we simply want to end it, 
            // placing the cursor on the Left side.
            if (!Selection.IsInsertionPoint && !Selection.IsInsertionIcon)
            {
                Selection = new Sel(Selection.Paragraph, Selection.First);
                return;
            }

            // If we are not yet to the beginning of the EWord, then simply decrement
            // the iChar by one.
            if (Selection.Anchor.iChar > 0)
            {
                // Create the selection
                Sel.SelPoint Anchor = new Sel.SelPoint(
                    Selection.Anchor.iBlock,
                    Selection.Anchor.iChar - 1);
                Selection = new Sel(Selection.Paragraph, Anchor);
                return;
            }

            // If we've made it here, then we want to move to the end of the previous
            // EWord (which may mean going to another paragraph/pile/row)
            bool bSuccessful = Contents.Select_PrevWord_End(Selection);

            // If this puts us at the end of a word, and if there was a EWord to our
            // right, then we need to decrement one more.
            if (bSuccessful && Selection.Anchor.Word.IsBesideEWord(true))
                cmdMoveCharLeft();
        }
        #endregion
        #region Cmd: cmdMoveCharRight - move cursor one char to the right
        public void cmdMoveCharRight()
        {
            // If there is no Selection object, then we can't' do anything (we are in a
            // wierd state.
            if (null == Selection)
                return;

            // If we are at an insertion icon, then we want to move to the next word
            if (Selection.IsInsertionIcon)
            {
                cmdMoveWordRight();
                return;
            }

            // If we have a multi-character selection, then we simply want to end it, 
            // placing the cursor on the Right side.
            if (!Selection.IsInsertionPoint)
            {
                Selection = new Sel(Selection.Paragraph, Selection.Last);
                return;
            }

            // If we are not yet to the end of the EWord, then we simply increment
            // the iChar by one.
            if (Selection.Anchor.iChar < Selection.Anchor.Word.Text.Length)
            {
                // Create the selection
                Sel.SelPoint Anchor = new Sel.SelPoint(
                    Selection.Anchor.iBlock,
                    Selection.Anchor.iChar + 1);
                Selection = new Sel(Selection.Paragraph, Anchor);

                // If this puts us at the end of a Word, and if there is an EWord right 
                // next to us, then we'll want to actually be at the first position in
                // that EWord. 
                if (Selection.Anchor.iChar == Selection.Anchor.Word.Text.Length &&
                    Selection.Anchor.iBlock < Selection.Paragraph.SubItems.Length - 1 &&
                    null != Selection.Paragraph.SubItems[Selection.Anchor.iBlock + 1] as EWord)
                {
                    Contents.Select_NextWord_Begin(Selection);
                }

                return;
            }

            // If we've made it here, then we want to move to the beginning of the next
            // EWord (which may mean going to another paragraph/pile/row)
            Contents.Select_NextWord_Begin(Selection);
        }
        #endregion
        #region Cmd: cmdMoveWordLeft
        void cmdMoveWordLeft()
        {
            if (null == Selection)
                return;

            // If the Selection is currently at the beginning of a word, then move
            // to the previous word
            if (Selection.First.iChar == 0)
            {
                Contents.Select_PrevWord_Begin(Selection);
            }

            // Now move to the beginning of that word
            if (Selection.First.iChar > 0)
            {
                Selection = new Sel(Selection.Paragraph, 
                    new Sel.SelPoint( Selection.First.iBlock, 0) );
            }
        }
        #endregion
        #region Cmd: cmdMoveWordRight
        void cmdMoveWordRight()
            // We want to move to the beginning of the next EWord we can find
        {
            Contents.Select_NextWord_Begin(Selection);
        }
        #endregion
        #region Cmd: cmdMoveLineBegin
        void cmdMoveLineBegin()
        {
            if (null != Selection)
            {
                Selection.Paragraph.Select_LineBegin();
            }
        }
        #endregion
        #region Cmd: cmdMoveLineEnd
        void cmdMoveLineEnd()
        {
            if (null != Selection)
            {
                Selection.Paragraph.Select_LineEnd();
            }
        }
        #endregion

        #region Cmd: cmdMoveNextParagraph - e.g., in response to the Enter key
        void cmdMoveNextParagraph()
        {
            Contents.Select_NextPara_Begin(Selection);
        }
        #endregion
        #region Cmd: cmdMoveNextBasicText() - e.g., in response to the Tab key
        void cmdMoveNextBasicText()
        {
            if (null == Selection)
                return;

            // Move to the end of the current DBasicText
            int iBlock = Selection.DBT_iBlockLast;
            EWord word = Selection.Paragraph.SubItems[iBlock] as EWord;
            Debug.Assert(null != word);
            Selection = new OWWindow.Sel(Selection.Paragraph,
                iBlock, word.Text.Length);

            // Move to the next DBasicText by moving right one character
            cmdMoveCharRight();
        }
        #endregion
        #region Cmd: cmdMovePreviousBasicText() - e.g., in response to the Shift-Tab key
        void cmdMovePreviousBasicText()
        {
            if (null == Selection)
                return;

            // Move to the beginning of the current DBasicText
            int iBlock = Selection.DBT_iBlockFirst;
            EWord word = Selection.Paragraph.SubItems[iBlock] as EWord;
            Debug.Assert(null != word);
            Selection = new OWWindow.Sel(Selection.Paragraph, iBlock, 0);

            // Move to the previous DBasicText by moving left one character
            cmdMoveCharLeft();
        }
        #endregion

        #region Attr{g}: LineUpDownX UpDownX - keeps the x coord constant during multiple UPs/DOWNs
        #region DOCUMENTATION
        /* If the cursor is towards the end of the line, and the user presses DOWN, then the
         * cursor will move to the left if the next line is shorted. But if the following line
         * is longer and user presses DOWN again, we want to move back over to the original x.
         * 
         * So the LineUpDownX is a way to remember this X value. We set it whenever the
         * UP or DOWN arrow are pressed (if there is no value already there), and then use
         * it for subsequent UP/DOWN requests. Most other keypresses clear out this X value.
         * 
         * Thus we have to check for these keypresses as part of OnKeyDown, and as part of mousing.
         */
        #endregion
        #region CLASS LineUpDownX
        public class LineUpDownX
        {
            #region Attr{g}: float X - the coordinate for Up/Down actions
            public float X
            {
                get
                {
                    return m_fX;
                }
            }
            float m_fX = 0;
            #endregion
            #region Attr{g}: bool IsActive - if T, the value should be used by Up/Down
            public bool IsActive
            {
                get
                {
                    return (X != 0);
                }
            }
            #endregion
            #region Method: void Clear() - reset so that there is no Up/Down expectation
            public void Clear()
            {
                m_fX = 0;
            }
            #endregion
            #region Method: void CheckKeyDown(KeyEventArgs e)
            public void CheckKeyDown(KeyEventArgs e)
            {
                bool bDontClear = false;

                if (e.Modifiers == Keys.None)
                {
                    if (e.KeyCode == Keys.Up)
                        bDontClear = true;
                    if (e.KeyCode == Keys.Down)
                        bDontClear = true;
                }

                else if (e.Modifiers == Keys.Shift)
                {
                    if (e.KeyCode == Keys.Up)
                        bDontClear = true;
                    if (e.KeyCode == Keys.Down)
                        bDontClear = true;
                }

                if (!bDontClear)
                    Clear();
            }
            #endregion
            #region Method: void Set(x) - set X to the value
            public void Set(float x)
            {
                if (m_fX != 0)
                    return;

                m_fX = x;
            }
            #endregion
        }
        #endregion
        #region Attr{g}: LineUpDownX UpDownX
        public LineUpDownX UpDownX
        {
            get
            {
                Debug.Assert(null != m_LineUpDownX);
                return m_LineUpDownX;
            }
        }
        LineUpDownX m_LineUpDownX;
        #endregion
        #endregion
        #region Cmd: cmdMoveLineUp
        void cmdMoveLineUp()
        {
            if (null == Selection)
                return;

            // Get the desired horizontal position
            float x = (UpDownX.IsActive) ? UpDownX.X : Selection.Last.X;
            UpDownX.Set(x);

            // Retrieve our current position in the container heirarchy
            ArrayList aiStack = Selection.ContainerIndicesStack;

            // Select at the same position on the next line
            Contents.MoveLineUp(aiStack, new PointF(x, Selection.Last.Word.Position.Y));
        }
        #endregion
        #region Cmd: cmdMoveLineDown
        void cmdMoveLineDown()
        {
            if (null == Selection)
                return;

            // Get the desired horizontal position
            float x = (UpDownX.IsActive) ? UpDownX.X : Selection.Last.X;
            UpDownX.Set(x);

            // Retrieve our current position in the container heirarchy
            ArrayList aiStack = Selection.ContainerIndicesStack;

            // Select at the same position on the next line
            Contents.MoveLineDown(aiStack, new PointF(x, Selection.Last.Word.Position.Y));
        }
        #endregion
        #region Cmd: cmdExtendLineUp
        void cmdExtendLineUp()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            // Get the desired horizontal position
            float x = (UpDownX.IsActive) ?
                UpDownX.X :
                (Selection.IsInsertionPoint ? Selection.Anchor.X : Selection.End.X);
            UpDownX.Set(x);

            Selection.Paragraph.ExtendSelection_LineUpDown(false, x);
        }
        #endregion
        #region Cmd: cmdExtendLineDown
        void cmdExtendLineDown()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            // Get the desired horizontal position
            float x = (UpDownX.IsActive) ? 
                UpDownX.X : 
                (Selection.IsInsertionPoint ? Selection.Anchor.X : Selection.End.X);
            UpDownX.Set(x);

            Selection.Paragraph.ExtendSelection_LineUpDown(true, x);
        }
        #endregion

        #region Cmd: cmdExtendCharRight
        void cmdExtendCharRight()
        {
            // If there is no Selection object, then we can't' do anything (we are in a
            // wierd state.
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            // The paragraph handles this action, because we do not permit a selection to
            // extend beyond conjoining EWords.
            Selection = Selection.Paragraph.ExtendSelection_CharRight(Selection);
        }
        #endregion
        #region Cmd: cmdExtendCharLeft
        void cmdExtendCharLeft()
        {
            // If there is no Selection object, then we can't' do anything (we are in a
            // wierd state.
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            // The paragraph handles this action, because we do not permit a selection to
            // extend beyond conjoining EWords.
            Selection = Selection.Paragraph.ExtendSelection_CharLeft(Selection);
        }
        #endregion
        #region Cmd: cmdExtendLineBegin
        void cmdExtendLineBegin()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;
            Selection.Paragraph.ExtendSelection_Line(false);
        }
        #endregion
        #region Cmd: cmdExtendLineEnd
        void cmdExtendLineEnd()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;
            Selection.Paragraph.ExtendSelection_Line(true);
        }
        #endregion
        #region Cmd: cmdExtendWordRight
        void cmdExtendWordRight()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            OWPara para = Selection.Paragraph;

            Selection = para.ExtendSelection_WordRight(Selection);
        }
        #endregion
        #region Cmd: cmdExtendWordLeft
        void cmdExtendWordLeft()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            OWPara para = Selection.Paragraph;

            Selection = para.ExtendSelection_WordLeft(Selection);
        }
        #endregion
        #region Cmd: cmdExtendFarRight
        void cmdExtendFarRight()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;

            Selection = Selection.Paragraph.ExtendSelection_FarRight(Selection);
        }
        #endregion
        #region Cmd: cmdExtendFarLeft
        void cmdExtendFarLeft()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;
            
            Selection = Selection.Paragraph.ExtendSelection_FarLeft(Selection);
        }
        #endregion
        #endregion
    }


}
