#region ***** TranslationProgress.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    TranslationProgress.cs
 * Author:  John Wimbish
 * Created: 07 Oct 2009
 * Purpose: For displaying the progress of a book within the History dialog
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using JWTools;
using OurWordData.DataModel;
#endregion

namespace OurWord.Dialogs
{
    public partial class TranslationProgress : UserControl
    {
        // Attrs -----------------------------------------------------------------------------
        #region DTranslation Translation
        static public DTranslation Translation
        {
            private get
            {
                return s_Translation;
            }
            set
            {
                s_Translation = value;
            }
        }
        static DTranslation s_Translation;
        #endregion
        #region Attr[g}: Panel Chart
        Panel Chart
        {
            get
            {
                Debug.Assert(null != m_panelTranslationProgress);
                return m_panelTranslationProgress;
            }
        }
        #endregion
        #region Attr[g}: Panel Header
        Panel Header
        {
            get
            {
                Debug.Assert(null != m_panelHeader);
                return m_panelHeader;
            }
        }
        #endregion

        // Scrollbar -------------------------------------------------------------------------
        #region Attr{g}: int ScrollPos
        int ScrollPos
        {
            get
            {
                return m_ScrollBar.Value;
            }
        }
        #endregion
        #region Method: void SetScrollParams(int yContentHeight)
        void SetScrollParams(int yContentHeight)
        {
            m_ScrollBar.Maximum = yContentHeight; 

            m_ScrollBar.LargeChange = (int)(Chart.ClientRectangle.Height * 0.90);
            m_ScrollBar.SmallChange = (int)(Chart.ClientRectangle.Height * 0.05);
        }
        #endregion

        // Layout Params ---------------------------------------------------------------------
        #region SVAttr[g}: int VertMargin
        private static int VertMargin
        {
            get
            {
                return 5;
            }
        }
        #endregion
        #region SVAttr[g}: int HorzMargin
        public static int HorzMargin
        {
            get
            {
                return 5;
            }
        }
        #endregion
        #region VAttr{g}: int RowWidth - room for rows, less margins
        int RowWidth
        {
            get
            {
                return Chart.ClientRectangle.Width - 2 * HorzMargin;
            }
        }
        #endregion
        #region VAttr{g}: int BookNameColumnWidth
        public int BookNameColumnWidth
        {
            get
            {
                return (int)(RowWidth * 0.20F);
            }
        }
        #endregion
        #region VAttr{g}: int StageColumnWidth
        public int StageColumnWidth
        {
            get
            {
                return (RowWidth - BookNameColumnWidth) / Stages.Count;
            }
        }
        #endregion
        #region Method: Color GetStageColor(i)
        public static Color GetStageColor(int i)
        {
            var vc = new[] { 
                    Color.Red, Color.Navy, Color.Orange, Color.Green,
                    Color.Gray, Color.Blue, Color.Brown, Color.Teal};

            while(i >= vc.Length)
                i -= vc.Length;

                return vc[i];
        }
        #endregion
        #region SVAttr{g}: int Between
        public static int Between
        {
            get
            {
                return 3;
            }
        }
        #endregion
        #region Attr{g}: Font BookNameFont
        public Font BookNameFont
        {
            get
            {
                if (null == m_BookNameFont)
                    m_BookNameFont = SystemFonts.DialogFont;
                return m_BookNameFont;
            }
        }
        Font m_BookNameFont;
        #endregion
        #region Attr{g}: List<Stage> Stages
        public List<Stage> Stages
            // The FinalRevision stage isn't really part of the translatio process; it is just
            // used to represent revisions that happen after the book has been printed (or
            // approved for printing) but before the NT/Bible is published. So we don't really
            // want to have a column for it in the display.
        {
            get
            {
                if (null == m_vStages)
                {
                    m_vStages = new List<Stage>();

                    foreach (Stage stage in DB.TeamSettings.Stages)
                    {
                        if (stage.ID != Stage.c_idFinalRevision)
                            m_vStages.Add(stage);
                    }
                }

                return m_vStages;
            }
        }
        List<Stage> m_vStages;
        #endregion

        // Book Boxes (info on drawing individual books) -------------------------------------
        #region Attr{g}: List<BookBox> Boxes
        List<BookBox> Boxes
        {
            get
            {
                if (null == m_Boxes)
                    m_Boxes = new List<BookBox>();
                return m_Boxes;
            }
        }
        List<BookBox> m_Boxes;
        #endregion
        #region Method: void InitializeBookBoxes()
        void InitializeBookBoxes()
        {
            Boxes.Clear();

            if (null == Translation)
                return;

            // Loop through all possible books
            var vPlanned = DB.Project.PlannedBooks;
            var vBookInfos = G.BookGroups.AllBookInfos;
            for (var i = 0; i < vBookInfos.Count; i++)
            {
                var info = vBookInfos[i];

                // If the book is one we've started, add it
                var book = Translation.FindBook(info.Abbrev);
                if (null != book)
                    Boxes.Add(new BookBox(book));

                // If the book is in our PlannedBooks, add it
                else if (vPlanned.Contains(info.Abbrev))
                    Boxes.Add(new BookBox(Translation.BookNamesTable[i], info));
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public TranslationProgress()
        {
            InitializeComponent();
        }
        #endregion

        // Header ----------------------------------------------------------------------------
        #region Cmd: cmdPaintHeader
        private void cmdPaintHeader(object sender, PaintEventArgs e)
        {
            // We'll paint to a double buffer to avoid flicker
            var bmpBuffer = new Bitmap(Header.Width, Header.Height);
            var g = Graphics.FromImage(bmpBuffer);

            // Background
            g.FillRectangle(Brushes.BlanchedAlmond, Header.ClientRectangle);

            // Title. If there's no Translation defined (e.g., we're in Visual Studio), 
            // we're done.
            var y = DrawTitle(g, 0);
            if (null == Translation)
                return;

            // Stage names
            DrawStages(g, y);

            // Copy the double buffer to the control
            e.Graphics.DrawImageUnscaled(bmpBuffer, 0, 0);
            bmpBuffer.Dispose();
        }
        #endregion
        #region SAttr{g}: Font TitleFont
        static Font TitleFont
        {
            get
            {
                return new Font("Arial", 16);
            }
        }
        #endregion
        #region SAttr{g}: Font StageHeaderFont
        static Font StageHeaderFont
        {
            get
            {
                return new Font("Arial", 11, FontStyle.Bold);
            }
        }
        #endregion
        private const int c_LineWidthUnderneathTitle = 1;
        private const int c_MarginBetweenTitleAndStageNames = 5;
        #region SAttr{g}: int HeaderHeight
        static int HeaderHeight
        {
            get
            {
                var y = TitleFont.Height;
                y += c_LineWidthUnderneathTitle;
                y += c_MarginBetweenTitleAndStageNames;
                y += StageHeaderFont.Height;
                return y;
            }
        }
        #endregion
        #region Method: void DrawTitle(g)
        int DrawTitle(Graphics g, int y)
            // Returns the new y coordinate following the draw
        {
            // Colors, fonts, etc
            var brushTitle = Brushes.Navy;
            var font = TitleFont;

            // Text
            var sTitle = "Translation Progress";
            if (null != Translation)
            {
                var sBase = Loc.GetString("hpTitle", "{0} Translation Progress");
                sTitle = LocDB.Insert(sBase, new[] { Translation.DisplayName });
            }

            // Determine where to draw it
            var fWidth = JWU.CalculateDisplayWidth(g, sTitle, font);
            var fLeft = (Chart.Width - fWidth) / 2.0F;
            var pt = new PointF(fLeft, y);

            // Draw it
            g.DrawString(sTitle, font, brushTitle, pt, StringFormat.GenericTypographic);
            y += font.Height;

            // An underline to set it off
            g.DrawLine(Pens.Navy, fLeft, y, fLeft + fWidth, y);
            y += c_LineWidthUnderneathTitle;

            // Some margin before the table
            y += c_MarginBetweenTitleAndStageNames;

            return y;
        }
        #endregion
        #region Method: int DrawStages(Graphics g, int y)
        void DrawStages(Graphics g, int y)
        {
            // Colors, fonts, etc
            var font = StageHeaderFont;

            var x = HorzMargin + BookNameColumnWidth;
            var iStage = 0;
            foreach (var stage in Stages)
            {
                // Get text position so we can center in the column
                var fTextWidth = JWU.CalculateDisplayWidth(g, stage.LocalizedAbbrev, font);
                var xText = x + (int)(StageColumnWidth - fTextWidth)/2;

                var brushStage = new SolidBrush(GetStageColor(iStage++) );

                g.DrawString(stage.LocalizedAbbrev, font, brushStage, new PointF(xText, y));
                x += StageColumnWidth;
            }
        }
        #endregion

        // Chart -----------------------------------------------------------------------------
        #region Cmd: cmdPaintProgress
        private void cmdPaintProgress(object sender, PaintEventArgs e)
        {
            int yTop = -ScrollPos;
            int y = -ScrollPos + VertMargin;

            // We'll paint to a double buffer to avoid flicker
            var bmpBuffer = new Bitmap(Chart.Width, Chart.Height);
            var g = Graphics.FromImage(bmpBuffer);

            // Background
            var clrBackgroud = Brushes.BlanchedAlmond;
            g.FillRectangle(clrBackgroud, Chart.ClientRectangle);

            // Layout, then draw the boxes
            InitializeBookBoxes();
            foreach (var box in Boxes)
                y = box.LayoutAndPaint(g, this, y);

            // Update the scrollbar max, thumbs, etc with the new height
            SetScrollParams(y - yTop);

            // Copy the double buffer to the control
            e.Graphics.DrawImageUnscaled(bmpBuffer, 0, 0);
            bmpBuffer.Dispose();
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Cmd: OnPaintBackground
        protected override void OnPaintBackground(PaintEventArgs e)
            // We do the background painting as part of OnPaint; we don't want to 
            // call the base.OnPaintBackground event because it would cause flicker.
        {
        }
        #endregion
        #region Cmd: cmdScrolled
        private void cmdScrolled(object sender, ScrollEventArgs e)
            // Recalc the layout and redraw, since the scroll position has changed
        {
            Chart.Invalidate();
        }
        #endregion
        #region Cmd: cmdResize
        private void cmdResize(object sender, EventArgs e)
        {
            // Top Header is docked, so all we worry about is the height
            Header.Height = HeaderHeight;

            // Scroll bar
            m_ScrollBar.Location = new Point(Width - m_ScrollBar.Width, Header.Height);
            m_ScrollBar.Height = Height - Header.Height;

            // Chart
            Chart.Location = new Point(0, Header.Height);
            Chart.Size = new Size(Width - m_ScrollBar.Width, Height - Header.Height);
        }
        #endregion
    }

    class BookBox
    {
        // Attrs ----------------------------------------------------------------------------
        #region Attr{g}: string BookDisplayName
        string BookDisplayName
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sBookDisplayName));
                return m_sBookDisplayName;
            }
        }
        readonly string m_sBookDisplayName;
        #endregion
        #region Attr{g}: BookInfo Info
        BookInfo Info
        {
            get
            {
                Debug.Assert(null != m_BookInfo);
                return m_BookInfo;
            }
        }
        readonly BookInfo m_BookInfo;
        #endregion
        #region Attr{g}: DBook Book
        DBook Book
        {
            get
            {
                return m_Book;
            }
        }
        readonly DBook m_Book;
        #endregion

        // Layout and Paint ------------------------------------------------------------------
        #region VAttr{g}: double HeightRatio
        double HeightRatio
            // Returns a number between 0 and 1 representing this book's number of
            // verses, compared with those books having the min and the max.
        {
            get
            {
                double cBookVerses = Info.VerseCount;

                double cMaxVerses = G.BookGroups.MaxVerses;
                double cMinVerses = G.BookGroups.MinVerses;

                var factor = (cBookVerses - cMinVerses) /
                    (cMaxVerses - cMinVerses);

                return factor;
            }
        }
        #endregion
        #region Method: int LayoutAndPaint(Graphics g, TranslationProgress tp, int y)
        public int LayoutAndPaint(Graphics g, TranslationProgress tp, int y)
        {
            // Row Height: the largest book will be shown on the screen as three
            // times taller than the small ones, to show it was more work. 
            var nRowHeight = tp.BookNameFont.Height;
            nRowHeight += (int)(2 * nRowHeight * HeightRatio);

            // Book Name
            var yBookName = y + (int)((nRowHeight - (double)tp.BookNameFont.Height)/2.0);
            var sBookName = BookDisplayName;
            var xBookName = JWU.CalculateDisplayWidth(g, sBookName, tp.BookNameFont);
            var cTruncate = 0;
            while (xBookName > tp.BookNameColumnWidth - TranslationProgress.Between)
            {
                cTruncate++;
                sBookName = BookDisplayName.Substring(0, BookDisplayName.Length - cTruncate) + "...";
                xBookName = JWU.CalculateDisplayWidth(g, sBookName, tp.BookNameFont);
            }

            g.DrawString(sBookName, tp.BookNameFont, Brushes.Navy, 
                new PointF(TranslationProgress.HorzMargin, yBookName));

            // Planned books will have null stage, so we'll just do an outline to show it as a
            // placeholder
            if (null == Book)
            {
                var pen = new Pen(TranslationProgress.GetStageColor(0));
                int x = TranslationProgress.HorzMargin + tp.BookNameColumnWidth;
                int width = tp.StageColumnWidth - TranslationProgress.Between;
                var rect = new Rectangle(x, y, width, nRowHeight);
                g.DrawRectangle(pen, rect);
            }

            // Otherwise, we have a book with stages
            else
            {
                // Which stage are we in this book
                var iStage = 0;
                while (iStage < tp.Stages.Count && tp.Stages[iStage] != Book.Stage)
                    iStage++;

                // Fill in the colors of what's been done
                for (int i = 0; i <= iStage && i < tp.Stages.Count; i++)
                    PaintStage(g, tp, i, y, nRowHeight);
            }

            return y + nRowHeight + TranslationProgress.Between;
        }
        #endregion
        #region SMethod void PaintStage(...)
        static void PaintStage(Graphics g, TranslationProgress tp, int iStage, int y, int nRowHeight)
        {
            var brush = new SolidBrush(TranslationProgress.GetStageColor(iStage));
            var x = TranslationProgress.HorzMargin + tp.BookNameColumnWidth + (iStage * tp.StageColumnWidth);
            var width = tp.StageColumnWidth - TranslationProgress.Between;
            var rect = new Rectangle(x, y, width, nRowHeight);
            g.FillRectangle(brush, rect);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DBook)
        public BookBox(DBook book)
        {
            m_Book = book;
            m_sBookDisplayName = book.DisplayName;
            m_BookInfo = G.BookGroups.FindBook(book.BookAbbrev);
        }
        #endregion
        #region Constructor(sBookDisplayName, BookInfo)
        public BookBox(string sBookDisplayName, BookInfo bi)
        {
            m_sBookDisplayName = sBookDisplayName;
            m_BookInfo = bi;
        }
        #endregion
    }


}
