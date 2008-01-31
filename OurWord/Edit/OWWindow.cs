/**********************************************************************************************
 * Project: OurWord!
 * File:    OWPara.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2007
 * Purpose: A window, with rows and piles
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using NUnit.Framework;
#endregion

namespace OurWord.Edit
{
    public class OWWindow : Panel
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: bool DrawLineBetweenColumns - if T, draw the vertical line btwn columns
        public bool DrawLineBetweenColumns
        {
            get
            {
                return m_bDrawLineBetweenColumns;
            }
            set
            {
                m_bDrawLineBetweenColumns = value;
            }
        }
        bool m_bDrawLineBetweenColumns = true;
        #endregion
        #region Attr{g}: SizeF ColumnMargins - the pixels on each side of the columns (top & bottom)
        public SizeF ColumnMargins
        {
            get
            {
                Debug.Assert(null != m_szColumnMargins);
                return m_szColumnMargins;
            }
            set
            {
                m_szColumnMargins = value;
            }
        }
        SizeF m_szColumnMargins;
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
        #region Attr{g/s}: bool LockedFromEditing - If T, can select, but can't insert/delete
        public bool LockedFromEditing
        {
            get
            {
                return m_bLockedFromEditing;
            }
            set
            {
                m_bLockedFromEditing = value;
            }
        }
        bool m_bLockedFromEditing = false;
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
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara paragraph in pile.Paragraphs)
                    {
                        foreach (OWPara.EBlock block in paragraph.Blocks)
                        {
                            block.MeasureWidth(Draw.Graphics);
                        }
                    }
                }
            }

            // Signal that we have a Draw object and that the blocks are all measured.
            // Otherwise, DoLayout would choke.
            m_bLoaded = true;

            // Perform a layout so that everything is located correctly
            DoLayout();

            // Tell Windows to paint the entire window
            Invalidate();

            // Select the first possible item
            Select_FirstWord();

            // Load, layout amd paint any secondary windows
            foreach (OWWindow w in SecondaryWindows)
                w.LoadData();
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
        #region Method: virtual void OnSelectAndScrollFromNote(DNote note)
        public virtual void OnSelectAndScrollFromNote(DNote note)
        {
            // Find the paragraph containing the icon which references this note
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara para in pile.Paragraphs)
                    {
                        if (!para.Editable)
                            continue;

                        // We want to keep track of the most recent editable place, if any
                        OWPara.EWord word = null;

                        foreach (OWPara.EBlock block in para.Blocks)
                        {
                            // Keep updating this, so that it points to the most close word 
                            // preceeding the note icon
                            if (block as OWPara.EWord != null)
                                word = block as OWPara.EWord;

                            // Look for a Note icon
                            OWPara.ENote n = block as OWPara.ENote;
                            if (n != null && n.Note == note)
                            {
                                // If we found a place we can select, then do so
                                if (null != word)
                                {
                                    Selection = new Sel(word.Para, 
                                        word.PositionWithinPara, 
                                        word.Text.Length);

                                    Focus();
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Method: virtual void OnSelectAndScrollFromFootnote(DFootnote footnote)
        public virtual void OnSelectAndScrollFromFootnote(DFootnote footnote)
        {
            // Find the paragraph containing the icon which references this note
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara para in pile.Paragraphs)
                    {
                        if (!para.Editable)
                            continue;

                        // We want to keep track of the most recent editable place, if any
                        OWPara.EWord word = null;

                        foreach (OWPara.EBlock block in para.Blocks)
                        {
                            // Keep updating this, so that it points to the most close word 
                            // preceeding the note icon
                            if (block as OWPara.EWord != null)
                                word = block as OWPara.EWord;

                            // Look for a FootLetter or a SeeAlso
                            OWPara.ESeeAlso also = block as OWPara.ESeeAlso;
                            OWPara.EFootLetter letter = block as OWPara.EFootLetter;
                            if ( (also != null && also.Footnote == footnote) ||
                                 (letter != null && letter.Footnote == footnote) )
                            {
                                // If we found a place we can select, then do so
                                if (null != word)
                                {
                                    Selection = new Sel(word.Para,
                                        word.PositionWithinPara,
                                        word.Text.Length);

                                    Focus();
                                }
                                return;
                            }
                        }
                    }
                }
            }
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
        #region Secondary Window Message: AddNote
        protected virtual void OnAddNote(DNote note, bool bIsEditable)
        {
        }
        public void Secondary_AddNote(DNote note, bool bIsEditable)
        {
            foreach (OWWindow w in SecondaryWindows)
                w.OnAddNote(note, bIsEditable);
        }
        #endregion
        #region Secondary Window Message: OnSelectAndScrollToNote
        public virtual void OnSelectAndScrollToNote(DNote note)
        {
        }
        public void Secondary_SelectAndScrollToNote(DNote note)
        {
            foreach (OWWindow w in SecondaryWindows)
                w.OnSelectAndScrollToNote(note);
        }
        #endregion
        #region Secondary Window Message: OnselectionChanged
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public OWWindow(string _sName, int _cColumnCount)
            : base()
        {
            // Number of columns in this window
            m_cColumnCount = _cColumnCount;

            // Name for the window
            Name = _sName;

            // Registry key for settings
            m_sRegistrySettingsSubKey = _sName;

            // Initialize ScrollBar
            AutoScroll = false;             // Don't use Panel's built-in scrollbar
            m_ScrollBar = new VScrollBar();
            m_ScrollBar.Dock = DockStyle.Right;
            Controls.Add(m_ScrollBar);
            ScrollBarPosition = 0;
            ScrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);

            // Used for Up/Down arrow behavior
            m_LineUpDownX = new LineUpDownX();

            // Initialize to empty rows
            m_vRows = new Row[0];

            // Default margins
            ColumnMargins = new SizeF(5, 5);

            // Set up a double buffer for flicker-free painting (it must be re-created
            // upon any resize.)
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Give us a sunken border
            BorderStyle = BorderStyle.Fixed3D;

            // Vector of secondary windows
            m_vSecondaryWindows = new OWWindow[0];
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
        #region Method: override void OnCreateControl()
        protected override void OnCreateControl()
        {
            //Console.WriteLine("ON CREATE CONTROL");
            base.OnCreateControl();

            LoadData();
        }
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
        #region Attr{g}: virtual string PassageName
        public virtual string PassageName
        {
            get
            {
                return "Passage";
            }
        }
        #endregion

        // Rows ------------------------------------------------------------------------------
        #region CLASS: Row
        public class Row
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: OWWindow Window - the owning window
            OWWindow Window
            {
                get
                {
                    Debug.Assert(null != m_Window);
                    return m_Window;
                }
            }
            OWWindow m_Window = null;
            #endregion
            #region Attr{g}: Bitmap Bmp - the picture's bitmap
            Bitmap Bmp
            {
                get
                {
                    return m_bmp;
                }
            }
            Bitmap m_bmp = null;
            #endregion
            const int c_BitmapMargin = 5;         // vert marg above/below the bitmap
            const int c_BitmapPadAtBottom = 2;    // pixels at bottom to ensure line gets drawn

            // Piles -------------------------------------------------------------------------
            #region CLASS: Pile
            public class Pile
            {
                // Screen Region -------------------------------------------------------------
                #region Attr{g/s}: PointF Position
                public PointF Position
                {
                    get
                    {
                        return m_ptPosition;
                    }
                    set
                    {
                        m_ptPosition = value;
                    }
                }
                private PointF m_ptPosition;
                #endregion
                #region Attr{g/s}: float MeasuredWidth - Calc'd via the MeasureWidth method during layout
                public float MeasuredWidth
                {
                    get
                    {
                        return m_fMeasuredWidth;
                    }
                    set
                    {
                        m_fMeasuredWidth = value;
                    }
                }
                protected float m_fMeasuredWidth = 0;
                #endregion
                #region Attr{g/s}: float Height - calc'd from LineCount and Para.LineHeight
                public float Height
                {
                    get
                    {
                        return m_fHeight;
                    }
                    set
                    {
                        m_fHeight = value;
                    }
                }
                float m_fHeight = 0;
                #endregion
                #region VAttr{g}: RectangleF Rectangle
                public RectangleF Rectangle
                {
                    get
                    {
                        return new RectangleF(Position, new SizeF(MeasuredWidth, Height));
                    }
                }
                #endregion
                #region Method: virtual bool ContainsPoint(PointF pt)
                public bool ContainsPoint(PointF pt)
                {
                    return Rectangle.Contains(pt);
                }
                #endregion

                // Attrs ---------------------------------------------------------------------
                #region Attr{g}: OWPara[] Paragraphs - the paragraphs in this pile
                public OWPara[] Paragraphs
                {
                    get
                    {
                        Debug.Assert(null != m_vParagraphs);
                        return m_vParagraphs;
                    }
                }
                OWPara[] m_vParagraphs;
                #endregion
                #region Attr{g}: Row Row - the owning row
                public Row Row 
                {
                    get
                    {
                        Debug.Assert(null != m_Row);
                        return m_Row;
                    }
                }
                Row m_Row = null;
                #endregion
                #region VAttr{g}: OWWindow Window - the owning window
                public OWWindow Window
                {
                    get
                    {
                        return Row.Window;
                    }
                }
                #endregion
                #region Attr{g}: bool DisplayFootnoteSeparator - if T, shows line btwn para's and footnotes
                bool DisplayFootnoteSeparator
                {
                    get
                    {
                        return m_bDisplayFootnoteSeparator;
                    }
                }
                bool m_bDisplayFootnoteSeparator = false;
                #endregion

                // Scaffolding ---------------------------------------------------------------
                #region Constructor(Row)
                public Pile(Row row, bool _bDisplayFootnoteSeparator)
                {
                    m_Row = row;
                    m_vParagraphs = new OWPara[0];
                    m_bDisplayFootnoteSeparator = _bDisplayFootnoteSeparator;
                }
                #endregion

                // Placing Content into the window -------------------------------------------
                #region Method: void AddParagraph(OWPara p)
                public void AddParagraph(OWPara p)
                {
                    // Create a new vector that is one longer
                    OWPara[] v = new OWPara[Paragraphs.Length + 1];

                    // Transfer the existing contents to it
                    for (int i = 0; i < Paragraphs.Length; i++)
                        v[i] = Paragraphs[i];

                    // The final item in the new vector should be the appended paragraph
                    v[Paragraphs.Length] = p;

                    // Set the old to point ot the new
                    m_vParagraphs = v;
                }
                #endregion
                #region Method: void RemoveParagraph(OWPara p)
                public void RemoveParagraph(OWPara p)
                {
                    // Create a new vector that is one shorted
                    OWPara[] v = new OWPara[Paragraphs.Length - 1];

                    // Transfer the keeper paragraphs to it
                    int k = 0;
                    for (int i = 0; i < Paragraphs.Length; i++)
                    {
                        if (Paragraphs[i] != p)
                        {
                            v[k] = Paragraphs[i];
                            k++;
                        }
                    }

                    // Set the old to point ot the new
                    m_vParagraphs = v;
                }
                #endregion

                // Layout & Paint ------------------------------------------------------------
                #region Method: void DoLayout(PointF ptPos, int nColumnWidth)
                public void DoLayout(PointF ptPos, int nColumnWidth)
                {
                    // Remember the top-left position and width
                    Position = ptPos;
                    MeasuredWidth = nColumnWidth;
                    Height = 0;

                    // If we are displaying the footnote separator, then add a pixel to the
                    // height to make room for it.
                    if (DisplayFootnoteSeparator)
                        Height += 1;

                    // Layout the paragraphs, one below the other
                    foreach (OWPara p in Paragraphs)
                    {
                        p.DoLayout(new PointF(Position.X, Position.Y + Height), nColumnWidth);
                        Height += p.Height;
                    }
                }
                #endregion
                #region Method: void Paint(ClipRectangle)
                public void Paint(Rectangle ClipRectangle)
                {
                    // Display the footnote separator if appropriate
                    if (DisplayFootnoteSeparator)
                    {
                        float xSeparatorWidth = Rectangle.Width / 3.0F;
                        Pen pen = new Pen(Color.Black);
                        Window.Draw.Line(pen, Position, 
                            new PointF(Position.X + xSeparatorWidth, Position.Y));
                    }

                    // Display each of the paragraphs
                    foreach (OWPara para in Paragraphs)
                        para.Paint(ClipRectangle);
                }
                #endregion
                #region Method: void RePosition(float y) - change the Y coord of this pile and its children
                public void RePosition(float y)
                {
                    // Set the Pile's position
                    Position = new PointF(Position.X, y);

                    // We'll calculate the height of the pile here
                    float fHeight = 0;

                    // If we are displaying the footnote separator, then add a pixel to the
                    // height to make room for it.
                    if (DisplayFootnoteSeparator)
                    {
                        fHeight += 1;
                        y += 1;
                    }

                    // Reset the "y" of each paragraph in the pile
                    foreach (OWPara p in Paragraphs)
                    {
                        p.RePosition(y);

                        y += p.Height;

                        fHeight += p.Height;
                    }

                    Height = fHeight;
                }
                #endregion
                #region Method: void AssignLineNumbers(ref int nLineNo)
                public void AssignLineNumbers(ref int nLineNo)
                {
                    foreach(OWPara p in Paragraphs)
                        p.AssignLineNumbers(ref nLineNo);
                }
                #endregion

                // Selection -----------------------------------------------------------------
                #region Method: bool Select_FirstWord()
                public bool Select_FirstWord()
                {
                    foreach (OWPara p in Paragraphs)
                    {
                        if (p.Select_BeginningOfFirstWord())
                            return true;
                    }
                    return false;
                }
                #endregion
                #region Method: bool Select_NextWord(int iParaCandidate, int iWordCandidate)
                public bool Select_NextWord(int iParaCandidate, int iWordCandidate)
                {
                    // Attempt within the candidate paragraph first
                    if (Paragraphs[iParaCandidate].Select_NextWord(iWordCandidate))
                        return true;

                    // Now attempt with all subsequent paragraphs
                    for (int i = iParaCandidate + 1; i < Paragraphs.Length; i++)
                    {
                        if (Paragraphs[i].Select_BeginningOfFirstWord())
                            return true;
                    }

                    // Unable to find something we could select
                    return false;
                }
                #endregion
                #region Method: bool Select_LastWord()
                public bool Select_LastWord()
                {
                    for(int i = Paragraphs.Length - 1; i >=0 ; i--)
                    {
                        if (Paragraphs[i].Select_EndOfLastWord())
                            return true;
                    }
                    return false;
                }
                #endregion
                #region Method: bool Select_PreviousWord(int iParaCandidate, int iWordCandidate)
                public bool Select_PreviousWord(int iParaCandidate, int iWordCandidate)
                {
                    // Attempt within the candidate paragraph first
                    if (Paragraphs[iParaCandidate].Select_PreviousWord(iWordCandidate))
                        return true;

                    // Now attempt with all subsequent paragraphs
                    for (int i = iParaCandidate - 1; i >= 0; i--)
                    {
                        if (Paragraphs[i].Select_EndOfLastWord())
                            return true;
                    }

                    // Unable to find something we could select
                    return false;
                }
                #endregion

                #region Method: EBlock GetBlockAt(PointF pt)
                public OWPara.EBlock GetBlockAt(PointF pt)
                {
                    foreach (OWPara paragraph in Paragraphs)
                    {
                        OWPara.EBlock block = paragraph.GetBlockAt(pt);
                        if (null != block)
                            return block;
                    }
                    return null;
                }
                #endregion
                #region Method: int GetParagraphIndex(OWPara p)
                public int GetParagraphIndex(OWPara p)
                {
                    for (int i = 0; i < Paragraphs.Length; i++)
                    {
                        if (Paragraphs[i] == p)
                            return i;
                    }
                    return -1;
                }
                #endregion

            }
            #endregion
            #region Attr{g}: Pile[] Piles - the piles in this row
            public Pile[] Piles
            {
                get
                {
                    Debug.Assert(null != m_vPiles);
                    return m_vPiles;
                }
            }
            Pile[] m_vPiles;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(OWWindow, cColumns, bDisplayFootnoteSeparator, Bitmap)
            public Row(OWWindow _Window, int cColumns, bool _bDisplayFootnoteSeparator, Bitmap _bmp)
            {
                // We want to remember the parent window
                m_Window = _Window;

                // Is there a bitmap requested?
                m_bmp = _bmp;

                // Create the vector that has room for each column
                m_vPiles = new Pile[cColumns];

                // Create a Pile for each column
                for (int i = 0; i < cColumns; i++)
                    m_vPiles[i] = new Pile(this, _bDisplayFootnoteSeparator);
            }
            #endregion

            // Placing Content into the window -----------------------------------------------
            #region Method: void AddParagraph(int iColumn, DParagraph paragraph, Flags)
            public void AddParagraph(int iColumn, DParagraph paragraph, OWPara.Flags options)
            {
                Debug.Assert(iColumn < Window.ColumnCount);
                Debug.Assert(iColumn >= 0 && iColumn < Piles.Length);

                // Retrieve the requested pile
                Pile pile = Piles[iColumn];
                Debug.Assert(null != pile);

                // Determine the paragraph's writing system depending upon whether or not
                // the BT is being displayed.
                JWritingSystem ws = 
                    ((options & OWPara.Flags.ShowBackTranslation) == 
                        OWPara.Flags.ShowBackTranslation) ?
                    paragraph.Translation.WritingSystemConsultant :
                    paragraph.Translation.WritingSystemVernacular;

                // Create and initialize the new paragraph
                OWPara p = new OWPara(pile, paragraph, ws, options);

                // Append the paragraph to the pile
                pile.AddParagraph(p);
            }
            #endregion
            #region Method: void AddNote(DNote note, Flags)
            public void AddNote(DNote note, OWPara.Flags options)
            {
                Debug.Assert(Window.ColumnCount == 1);

                // Retrieve the first pile
                Debug.Assert(Piles.Length == 1);
                Pile pile = Piles[0];

                // Determine the note's writing system
                JWritingSystem ws = note.Paragraph.Translation.WritingSystemConsultant;

                // Create and initialize the new note
                OWPara p = new OWPara(pile, note, ws, options);

                // Append the note to the pile
                pile.AddParagraph(p);
            }
            #endregion
            #region Method: void AddLabeledText(DTranslation, DRun[], sLabel)
            public void AddLabeledText(DTranslation translation, DRun[] vRuns, string sLabel)
            {
                Debug.Assert(Window.ColumnCount == 1);

                // Retrieve the first pile
                Debug.Assert(Piles.Length == 1);
                Pile pile = Piles[0];

                // Determine the writing system from the Translation
                Debug.Assert(vRuns.Length > 0);
                JWritingSystem ws = translation.WritingSystemVernacular;

                // Create and initialize the new text
                OWPara p = new OWPara(pile, vRuns, ws, sLabel, OWPara.Flags.None);

                // Append it to the pile
                pile.AddParagraph(p);
            }
            #endregion

            // Screen Region -----------------------------------------------------------------
            #region Attr{g/s}: PointF Position
            public PointF Position
            {
                get
                {
                    return m_ptPosition;
                }
                set
                {
                    m_ptPosition = value;
                }
            }
            private PointF m_ptPosition;
            #endregion
            #region Attr{g/s}: float MeasuredWidth - Calc'd via the MeasureWidth method during layout
            public float MeasuredWidth
            {
                get
                {
                    return m_fMeasuredWidth;
                }
                set
                {
                    m_fMeasuredWidth = value;
                }
            }
            protected float m_fMeasuredWidth = 0;
            #endregion
            #region Attr{g/s}: float Height - calc'd from LineCount and Para.LineHeight
            public float Height
            {
                get
                {
                    return m_fHeight;
                }
                set
                {
                    m_fHeight = value;
                }
            }
            float m_fHeight = 0;
            #endregion
            #region VAttr{g}: RectangleF Rectangle
            public RectangleF Rectangle
            {
                get
                {
                    return new RectangleF(Position, new SizeF(MeasuredWidth, Height));
                }
            }
            #endregion
            #region VAttr{g}: Rectangle IntRectangle
            public Rectangle IntRectangle
            {
                get
                {
                    int x = (int)Position.X;
                    int y = (int)Position.Y;
                    int w = (int)MeasuredWidth;
                    int h = (int)Height;
                    return new Rectangle(x, y, w, h);
                }
            }
            #endregion
            #region Method: virtual bool ContainsPoint(PointF pt)
            public bool ContainsPoint(PointF pt)
            {
                return Rectangle.Contains(pt);
            }
            #endregion

            // Layout & Paint ----------------------------------------------------------------
            #region Method: void DoLayout(...)
            public void DoLayout(float[] xColumns, float y, int nColumnWidth)
            {
                MeasuredWidth = Window.Width;
                _LayoutEngine(xColumns[0], y, xColumns, nColumnWidth);
            }
            #endregion
            #region Method: void RePosition(float yNew) - Change the y coord of this Row and its children
            public void RePosition(float yNew)
            {
                _LayoutEngine(Position.X, yNew, null, 0);

                #region OBSOLETE ON 3 OCT 2007 - KEEP A WHILE TO MAKE SURE
                /*** (Replaced by _LayoutEngine)
                float fHeight = 0;

                Position = new PointF(Position.X, y);

                // Adjust y for the bitmap, if present
                if (null != Bmp)
                {
                    // Adjust for the upper line
                    y += 1; 
                    // Adjust for the margin
                    y += c_BitmapMargin;
                    // Adjust for the bitmap itself
                    y += Bmp.Height;
                }

                // Reposition the piles
                foreach (Pile pile in Piles)
                {
                    pile.RePosition(y);
                    fHeight = Math.Max(fHeight, pile.Height);
                }

                // Adjust the height for the bitmap
                if (null != Bmp)
                {
                    // Adjust for the upper and lower lines
                    fHeight += 2;
                    // Adjust for padding at the bottom
                    fHeight += c_BitmapPadAtBottom;
                    // Adjust for the margin
                    fHeight += c_BitmapMargin;
                    // Adjust for the bitmap itself
                    fHeight += Bmp.Height;
                }

                Height = fHeight;
                ***/
                #endregion
            }
            #endregion
            #region Method: void _LayoutEngine(xLeft, yTop, float[] xColumns, nColumnWidth)
            void _LayoutEngine(float xLeft, float yTop, float[] xColumns, int nColumnWidth)
                // The goals are to layout the children, and to calclate the Height
                //
                // Parameters
                //   xLeft - the left pixel for the Row
                //   yTop - the top pixel for the Row
                //   xColumns - the left x pixel for each column. If null, then
                //       we want to just do a reposition, not a layout.
                //   nColumnWidth - the desired width for each column in the row.
                //       If nColumnWidth is nonzero, then we want to do a re-layout; 
                //       otherwise it is just a reposition.
            {
                Position = new PointF(xLeft, yTop);

                // Allow for the top part of the bitmap in "y" when laying out the piles
                float fHeightBmp = 0;
                if (null != Bmp)
                {
                    // Allow one pixel room for the line above the drawing
                    fHeightBmp += 1;
                    // Allow room for the margin above the bitmap
                    fHeightBmp += c_BitmapMargin;
                    // Allow room for the bitmap itself
                    fHeightBmp += Bmp.Height;
                }

                // Calculate for the tallest pile
                float fHeightPiles = 0;
                for (int i = 0; i < Piles.Length; i++)
                {

                    if (nColumnWidth != 0 && null != xColumns)
                    {
                        PointF ptPilePosition = new PointF(
                            xColumns[i],
                            yTop + fHeightBmp);

                        Piles[i].DoLayout(ptPilePosition, nColumnWidth);
                    }
                    else
                    {
                        Piles[i].RePosition(yTop + fHeightBmp);
                    }

                    fHeightPiles = Math.Max(Piles[i].Height, fHeightPiles);
                }

                // Adjust for the bottom part of the bitmap borders (if there is one)
                if (null != Bmp)
                {
                    // Allow room for the margin below the bitmap
                    fHeightBmp += c_BitmapMargin;
                    // Allow one pixel for the line below the drawing
                    fHeightBmp += 1;
                    // Allow a touch of extra padding so the line draws correctly
                    fHeightBmp += c_BitmapPadAtBottom;
                }

                // The Row's Height is the sum of PileHeight and BmpHeight
                Height = fHeightBmp + fHeightPiles;
            }
            #endregion
            #region Method: void Paint(ClipRectangle)
            public void Paint(Rectangle ClipRectangle)
            {
                // Check to see that this is something we truly need to be painting; 
                // simply return if not.
                if (!ClipRectangle.IntersectsWith(IntRectangle))
                    return;

                // Handle the bitmap, if present
                if (null != Bmp)
                {
                    // Line above and below
                    Pen pen = new Pen(Color.Black);
                    Window.Draw.Line(pen, 
                        Position,
                        new PointF(Position.X + Rectangle.Width, Position.Y));
                    Window.Draw.Line(pen, 
                        new PointF(Position.X, Position.Y + Rectangle.Height - 2),
                        new PointF(Position.X + Rectangle.Width, Position.Y + Rectangle.Height - 2));

                    // Draw the bitmap
                    float xBmp = Position.X + (Rectangle.Width - Bmp.Width) / 2;
                    float yBmp = Position.Y + 1 + c_BitmapMargin;
                    Window.Draw.Image(Bmp, new PointF(xBmp, yBmp));
                }

                // Paint each pile
                foreach (Pile pile in Piles)
                    pile.Paint(ClipRectangle);
            }
            #endregion
            #region Method: void AssignLineNumbers(ref int[] vnLineNo)
            public void AssignLineNumbers(ref int[] vnLineNo)
            {
                for (int i = 0; i < Piles.Length; i++)
                    Piles[i].AssignLineNumbers(ref vnLineNo[i]);
            }
            #endregion

            // Selection ---------------------------------------------------------------------
            #region Method: void Select_FirstWord()
            public bool Select_FirstWord()
            {
                foreach (Pile pile in Piles)
                {
                    if (pile.Select_FirstWord())
                        return true;
                }
                return false;
            }
            #endregion
            #region Method: bool Select_NextWord(...)
            public bool Select_NextWord(int iPileCandidate, int iParagraphCandidate, int iWordCandidate)
            {
                // Attempt within the candidate pile first
                if (Piles[iPileCandidate].Select_NextWord(iParagraphCandidate, iWordCandidate))
                    return true;

                // Now attempt with all subsequent piles
                for (int i = iPileCandidate + 1; i < Piles.Length; i++)
                {
                    if (Piles[i].Select_FirstWord())
                        return true;
                }

                // Unable to find something we could select
                return false;
            }
            #endregion
            #region Method: bool Select_LastWord()
            public bool Select_LastWord()
            {
                for (int i = Piles.Length - 1; i >= 0; i--)
                {
                    if (Piles[i].Select_LastWord())
                        return true;
                }
                return false;
            }
            #endregion
            #region Method: bool Select_PreviousWord(...)
            public bool Select_PreviousWord(int iPileCandidate, int iParagraphCandidate, int iWordCandidate)
            {
                // Attempt within the candidate pile first
                if (Piles[iPileCandidate].Select_PreviousWord(iParagraphCandidate, iWordCandidate))
                    return true;

                // Now attempt with all subsequent piles
                for (int i = iPileCandidate - 1; i >= 0; i--)
                {
                    if (Piles[i].Select_LastWord())
                        return true;
                }

                // Unable to find something we could select
                return false;
            }
            #endregion

            #region Method: EBlock GetBlockAt(PointF pt)
            public OWPara.EBlock GetBlockAt(PointF pt)
            {
                // Since we are sequentially working through the rows, from top of window to
                // bottom, all we need to do here is test whether the point comes lower than
                // this row.
                if (pt.Y > Position.Y + Height)
                    return null;

                foreach (Pile pile in Piles)
                {
                    OWPara.EBlock block = pile.GetBlockAt(pt);
                    if (null != block)
                        return block;
                }
                return null;
            }
            #endregion
            #region Method: int GetPileIndex(Pile pile)
            public int GetPileIndex(Pile pile)
            {
                for (int i = 0; i < Piles.Length; i++)
                {
                    if (Piles[i] == pile)
                        return i;
                }
                return -1;
            }
            #endregion
        }
        #endregion
        #region Attr{g}: Row[] Rows
        public Row[] Rows
        {
            get
            {
                Debug.Assert(null != m_vRows);
                return m_vRows;
            }
        }
        Row[] m_vRows = null;
        #endregion

        // Placing Data Content into the window ----------------------------------------------
        #region Method: void Clear()
        public void Clear()
        {
            // Reset the Rows
            m_vRows = new Row[0];

            // No longer is there a selection
            Selection = null;

            // Have the secondary windows do the same
            foreach (OWWindow w in SecondaryWindows)
                w.Clear();
        }
        #endregion
        #region Method: void StartNewRow() - this version does not have the footnote separator
        public void StartNewRow()
        {
            StartNewRow(false, null);
        }
        #endregion
        #region Method: void StartNewRow(bDisplayFootnoteSeparator, Bitmap)
        public void StartNewRow(bool _bDisplayFootnoteSeparator, Bitmap _bmp)
        {
            // Create the new Row
            Row row = new Row(this, ColumnCount, _bDisplayFootnoteSeparator, _bmp);

            // Create a new vector that is one longer
            Row[] v = new Row[Rows.Length + 1];

            // Transfer the existing contents to it
            for (int i = 0; i < Rows.Length; i++)
                v[i] = Rows[i];

            // The final item in the new vector should be the new, appended row
            v[Rows.Length] = row;

            // Set the old vector to now point to this new, longer one
            m_vRows = v;
        }
        #endregion
        #region Method: void AddParagraph(int iColumn, DParagraph, Flags)
        public void AddParagraph(int iColumn, DParagraph paragraph, OWPara.Flags options)
        {
            // Make sure iColumn is within range
            Debug.Assert(ColumnCount > 0);
            Debug.Assert(iColumn >= 0 && iColumn < ColumnCount);

            // If we do not have any rows, then add one
            if (Rows.Length == 0)
                StartNewRow();

            // Retrieve the last row in our vector
            Row row = Rows[Rows.Length - 1];
            Debug.Assert(null != row);

            // Add the paragraph to the desired column
            row.AddParagraph(iColumn, paragraph, options);
        }
        #endregion
        #region Method: void AddNote(DNote note)
        public void AddNote(DNote note, OWPara.Flags options)
        {
            // At this time, we are only supporting a NotesWIndow with a single column
            Debug.Assert(ColumnCount == 1);

            // If we do not have any rows, then add one
            if (Rows.Length == 0)
                StartNewRow();

            // Retrieve the last row in our vector
            Row row = Rows[Rows.Length - 1];
            Debug.Assert(null != row);

            // Add the note
            row.AddNote(note, options);
        }
        #endregion
        #region Method: void AddLabeledText(DTranslation, DRun[], sLabel)
        public void AddLabeledText(DTranslation translation, DRun[] vRuns, string sLabel)
        {
            // At this time, we are only supporting a NotesWIndow with a single column
            Debug.Assert(ColumnCount == 1);

            // If we do not have any rows, then add one
            if (Rows.Length == 0)
                StartNewRow();

            // Retrieve the last row in our vector
            Row row = Rows[Rows.Length - 1];
            Debug.Assert(null != row);

            // Add the Labeled Text
            row.AddLabeledText(translation, vRuns, sLabel);
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
            delegate void InvalidateBlockCallback(OWPara.EBlock block);
            public void InvalidateBlock(OWPara.EBlock block)
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
                        block.MeasuredWidth + block.JustificationPaddingAdded,
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
                    para.MeasuredWidth,
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
        #region Attr{g}: float[] ColumnSeparatorPositions - the x's for the lines between columns
        float[] ColumnSeparatorPositions
        {
            get
            {
                Debug.Assert(null != m_vfColumnSeparatorPositions);
                Debug.Assert(m_vfColumnSeparatorPositions.Length == (ColumnCount - 1));
                return m_vfColumnSeparatorPositions;
            }
        }
        float[] m_vfColumnSeparatorPositions = null;
        #endregion
        #region Attr{g}: float[] ColumnContentPositions - the x's for the column text
        float[] ColumnContentPositions
        {
            get
            {
                Debug.Assert(null != m_vfColumnContentPositions);
                Debug.Assert(m_vfColumnContentPositions.Length == (ColumnCount));
                return m_vfColumnContentPositions;
            }
        }
        float[] m_vfColumnContentPositions = null;
        #endregion
        #region Attr{g/s}: The background color for words which can be edited. Default is White
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
        public void DoLayout()
        {
            // Make certain OnLoad() was called already (and thus, that we have a
            // Draw object, and that the EBlocks were all measured.
            if (!m_bLoaded)
                return;

            if (ColumnCount == 0)
                return;

            // Calculate the width available for each column, and the spacing between columns
            float fClientWidth = Width - m_ScrollBar.Width;
            float fWidthBetweenColumns = ColumnMargins.Width * 2 + 
                ((DrawLineBetweenColumns) ? 1 : 0);
            float fTotalContentWidth = fClientWidth - ColumnMargins.Width * 2 - 
                ((ColumnCount - 1) * fWidthBetweenColumns);
            int nColumnWidth = (int)(fTotalContentWidth / (float)ColumnCount);

            // We'll start from the top of the window, taking into account the margin
            float y = ColumnMargins.Height;

            // Calculate the location of each column and the separator lines
            m_vfColumnSeparatorPositions = new float[ColumnCount - 1];
            m_vfColumnContentPositions = new float[ColumnCount];
            float x = ColumnMargins.Width;
            for (int i = 0; i < ColumnCount; i++)
            {
                // On entering the loop, we are positioned for the column's content
                m_vfColumnContentPositions[i] = x;

                // Add the width of the column
                x += nColumnWidth;

                // Add the right column margin
                x += ColumnMargins.Width;

                // Add the line, if we are drawing it
                if (DrawLineBetweenColumns)
                {
                    x += 1;
                    if (i < ColumnCount - 1)
                        m_vfColumnSeparatorPositions[i] = x;
                }

                // Add the margin into the next column
                x += ColumnMargins.Width;
            }

            // Lay out the rows
            foreach (Row row in Rows)
            {
                row.DoLayout(ColumnContentPositions, y, nColumnWidth);
                y += row.Height;
            }

            // Now that the lines have been defined in the low-level OWPara's,
            // give each line a line number
            AssignLineNumbers();

            // Set the ScrollBar, adding some (30 pixels) padding at the bottom
            Layout_SetupScrollBar((int)y);
        }
        #endregion
        #region Method: void PaintNoDataMessage(PaintEventArgs e)
        void PaintNoDataMessage(PaintEventArgs e)
        {
            if (!IsMainWindow)
                return;

            string sText = G.GetLoc_GeneralUI("NoDataToDisplay", 
                "(There is no data to display. Use the Project-Properties menu item to " +
                "set up both a Front and a Target translation.)"); 

            Brush brush = new SolidBrush(Color.Black);
            Font font = SystemFonts.MenuFont;

            Draw.MultilineText(sText, font, brush, ClientRectangle);

            /*** TODO OBS - 18dec07 - Replaced with the line above; I think using the 
             * ClipRectangle was the reason it was displaying strangly on Mono. Need to 
             * verify before deleting this code. 
            Rectangle rect = new Rectangle(e.ClipRectangle.X,
                e.ClipRectangle.Y + (int)ScrollBarPosition,
                e.ClipRectangle.Width, e.ClipRectangle.Height);

            Draw.MultilineText(sText, font, brush, rect);
            ***/

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
            if (Rows.Length == 0)
            {
                PaintNoDataMessage(e);
                e.Graphics.DrawImageUnscaled(Draw.DoubleBuffer, 0, 0);
                return;
            }

            // Draw the lines between the columns
            if (DrawLineBetweenColumns && ColumnCount > 1)
            {
                Pen pen = new Pen(Color.Black);
                foreach (float x in ColumnSeparatorPositions)
                    Draw.Line(pen, x, 0, x, Height);
            }

            // Paint the rows
            foreach (Row row in Rows)
                row.Paint(r);

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
        #region Method: int GetRowIndex(Row row)
        public int GetRowIndex(Row row)
        {
            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] == row)
                    return i;
            }
            return -1;
        }
        #endregion
        #region Cmd: OnParagraphHeightChanged - recalculate positions to accomdate the new-sized paragraph
        public void OnParagraphHeightChanged(Row rowContainingChangedParagraph)
            // Each paragraph needs to recalculate its position and be redrawn
        {
            // Start at the top margin for the window
            float y = ColumnMargins.Height;

            // We'll not redraw until we encounter the row that has changed; thus we'll use
            // bFound to indicate when we've located that row
            bool bFound = false;

            // Process through each row
            foreach (Row row in Rows)
            {
                // Set the bFound flag once we encounter the target row
                if (row == rowContainingChangedParagraph)
                    bFound = true;

                // RePosition each row from the target to the end of the screen
                if (bFound)
                    row.RePosition(y);

                y += row.Height;
            }

            // Recalculate the lines numbers, as they may have changed
            AssignLineNumbers();

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
                    return Brushes.DarkGray;
                }
            }
            #endregion

            #region Constructor(Graphics)
            public CLineNumberAttrs(Graphics g)
            {
                // We'll use a fixed-space font so that the numbers line up
                float fSize = 10 * G.ZoomFactor;
                m_fLineNumberFont = new Font("Courier New", fSize);

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
        #region Method: void AssignLineNumbers()
        public void AssignLineNumbers()
        {
            // What is the maximum number of columns we have in any row?
            int cColumns = 0;
            foreach (Row r in Rows)
                cColumns = Math.Max(cColumns, r.Piles.Length);

            // Initialize a count for the number of columns we are supporting
            int[] vnLineNo = new int[cColumns];
            for (int i = 0; i < cColumns; i++)
                vnLineNo[i] = 1;

            // Go through each row, numbering the columns
            foreach (Row r in Rows)
                r.AssignLineNumbers(ref vnLineNo);
        }
        #endregion

        // Scroll Bar ------------------------------------------------------------------------
        #region Scroll Bar
        #region Attr{g}: VScrollBar ScrollBar
        VScrollBar ScrollBar
        {
            get
            {
                Debug.Assert(null != m_ScrollBar);
                return m_ScrollBar;
            }
        }
        VScrollBar m_ScrollBar;
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
                return (float)ScrollBar.Value;
            }
            set
            {
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
            // We'll set the range to include a little padding, to make sure we
            // can always scroll everything into view.
            ScrollBarRange = yTotalHeight + 30;

            // A small change will be enough to move a normal line at a time. We're guessing
            // a value based on the Target Translation's normal (zoomed) line height.
            JParagraphStyle PStyle = G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleAbbrevNormal);

            JWritingSystem ws = (null == G.TTranslation) ?
                G.StyleSheet.FindWritingSystem(DStyleSheet.c_Latin) :
                G.TTranslation.WritingSystemVernacular;

            float fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                ws).FontNormalZoom.Height;
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
                public OWPara.EWord Word
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
                OWPara.EWord m_Word = null;
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
            #region VAttr{g}: int iRow - the index of the Row containing this selection
            public int iRow
            {
                get
                {
                    OWWindow.Row row = Paragraph.Row;

                    int i = Window.GetRowIndex(row);
                    Debug.Assert(-1 != i);

                    return i;
                }
            }
            #endregion
            #region VAttr{g}: int iPile - the index of the Pile containing this selection
            public int iPile
            {
                get
                {
                    OWWindow.Row.Pile pile = Paragraph.Pile;
                    OWWindow.Row row = pile.Row;

                    int i = row.GetPileIndex(pile);
                    Debug.Assert(-1 != i);

                    return i;
                }
            }
            #endregion
            #region VAttr{g}: int iParagraph - the index of the OWPara within the Pile, containing this selection
            public int iParagraph
            {
                get
                {
                    OWWindow.Row.Pile pile = Paragraph.Pile;

                    int i = pile.GetParagraphIndex(Paragraph);
                    Debug.Assert(-1 != i);

                    return i;
                }
            }
            #endregion
            #region VAttr{g}: Row Row - the row containing this selection
            public OWWindow.Row Row
            {
                get
                {
                    return Paragraph.Row;
                }
            }
            #endregion
            #region VAttr{g}: Pile Pile - the pile containing this selection
            public OWWindow.Row.Pile Pile
            {
                get
                {
                    return Paragraph.Pile;
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
                            s += Paragraph.Blocks[i].Text;
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
                    for (int ib = 0; ib < Paragraph.Blocks.Length; ib++)
                    {
                        if (Paragraph.Blocks[ib] as OWPara.EWord != null)
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
                    for (int ib = Paragraph.Blocks.Length - 1; ib >= 0; ib--)
                    {
                        if (Paragraph.Blocks[ib] as OWPara.EWord != null)
                        {
                            if (ib != Anchor.iBlock)
                                return false;
                            break;
                        }
                    }

                    // Must be the last position in the block
                    OWPara.EWord word = Paragraph.Blocks[Anchor.iBlock] as OWPara.EWord;
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
                        OWPara.EWord word = Paragraph.Blocks[i] as OWPara.EWord;
                        
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
                    for (int i = Last.iBlock; i < Paragraph.Blocks.Length; i++)
                    {
                        // Attempt to cast to an EWord. 
                        OWPara.EWord word = Paragraph.Blocks[i] as OWPara.EWord;

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
                    OWPara.EWord word = Paragraph.Blocks[i] as OWPara.EWord;
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
                // We''ll keep track of the EWord we're working on here
                int iBlock = 0;
                OWPara.EWord word = null;

                // Scan for the first EWord for the DBT
                for (; iBlock < paragraph.Blocks.Length; iBlock++)
                {
                    word = paragraph.Blocks[iBlock] as OWPara.EWord;
                    if (word != null && word.Phrase.BasicText == DBT)
                        break;
                }

                // Proceed through the words, working our way through the iPos
                for (; iBlock < paragraph.Blocks.Length; iBlock++)
                {
                    word = paragraph.Blocks[iBlock] as OWPara.EWord;
                    if (null == word || word.Phrase.BasicText != DBT)
                    {
                        if (iBlock > 0 && (paragraph.Blocks[iBlock - 1] as OWPara.EWord) != null)
                        {
                            return new Sel(paragraph, iBlock - 1,
                                (paragraph.Blocks[iBlock - 1] as OWPara.EWord).Text.Length);
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
                OWPara.EWord word = null;

                // Scan for the first EWord for the DBT
                for (; iBlock < paragraph.Blocks.Length; iBlock++)
                {
                    word = paragraph.Blocks[iBlock] as OWPara.EWord;
                    if (word != null && word.Phrase.BasicText == DBT)
                        break;
                }

                SelPoint sp1 = null;
                SelPoint sp2 = null;

                // Proceed through the words, working our way through the iPos's
                for (; iBlock < paragraph.Blocks.Length; iBlock++)
                {
                    // Retrieve the EWord
                    word = paragraph.Blocks[iBlock] as OWPara.EWord;
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

            }
            #endregion
            #region Method: void SetupTimer()
            void SetupTimer()
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

                    OWPara.EWord word = Anchor.Word;

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
                        OWPara.EWord word = Paragraph.Blocks[i] as OWPara.EWord;
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
                OWPara.EBlock block = Window.GetBlockAt(pt);
                if (null == block)
                    return false;
                OWPara.EWord word = block as OWPara.EWord;
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
                    if (word == Paragraph.Blocks[i] as OWPara.EWord)
                        return true;
                }

                return false;
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(OWPara, iBlock, iChar)
            public Sel(OWPara paragraph, int iBlock, int iChar)
                : this(paragraph, new SelPoint(iBlock, iChar), null)
            {
            }
            #endregion
            #region Constructor(OWPara, SelPoint Anchor)
            public Sel(OWPara para, SelPoint Anchor)
                : this(para, Anchor, null)
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
                Anchor.Word = Paragraph.Blocks[Anchor.iBlock] as OWPara.EWord;

                // Set the End and the End.Word attribute
                m_End = _End;
                if (null != End)
                    End.Word = Paragraph.Blocks[End.iBlock] as OWPara.EWord;

                // If Anchor == End, then we have a Insertion Point.
                if (null != End && Anchor.Word == End.Word && Anchor.iChar == End.iChar)
                    m_End = null;
                if (Anchor == End)
                    m_End = null;

                // Check for a potential InsertionIcon, and modify the selection to
                // reflect it if so
                if (Anchor.Word.Text == OWPara.EWord.c_chInsertionSpace.ToString())
                {
                    m_Anchor = new SelPoint(Anchor.iBlock, 0);
                    Anchor.Word = Paragraph.Blocks[Anchor.iBlock] as OWPara.EWord;
                    m_End = new SelPoint(Anchor.iBlock, 1);
                    End.Word = Paragraph.Blocks[End.iBlock] as OWPara.EWord;
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
            }
        }
        Sel m_Selection = null;
        #endregion
        #region Method: void Select_FirstWord()
        public void Select_FirstWord()
        {
            Focus();

            foreach (Row row in Rows)
            {
                row.Select_FirstWord();
                if (null != Selection)
                    break;
            }
        }
        #endregion
        #region Method: bool Select_NextWord(...)
        public bool Select_NextWord(int iRowCandidate, int iPileCandidate, int iParagraphCandidate, int iWordCandidate)
        {
            // Attempt within the candidate row first
            if (Rows[iRowCandidate].Select_NextWord(iPileCandidate, iParagraphCandidate, iWordCandidate))
                return true;

            // Now attempt with all subsequent rows
            for (int i = iRowCandidate + 1; i < Rows.Length; i++)
            {
                if (Rows[i].Select_FirstWord())
                    return true;
            }

            // Unable to find something we could select
            return false;
        }
        #endregion
        #region Method: bool Select_LastWord()
        public bool Select_LastWord()
        {
            for (int i = Rows.Length - 1; i >= 0; i--)
            {
                if (Rows[i].Select_LastWord())
                    return true;
            }
            return false;
        }
        #endregion
        #region Method: bool Select_PreviousWord(...)
        public bool Select_PreviousWord(int iRowCandidate, int iPileCandidate, int iParagraphCandidate, int iWordCandidate)
        {
            // Attempt within the candidate row first
            if (Rows[iRowCandidate].Select_PreviousWord(iPileCandidate, iParagraphCandidate, iWordCandidate))
                return true;

            // Now attempt with all subsequent rows
            for (int i = iRowCandidate - 1; i >= 0; i--)
            {
                if (Rows[i].Select_LastWord())
                    return true;
            }

            // Unable to find something we could select
            return false;
        }
        #endregion
        #region Method: bool Select_FirstPositionInParagraph(DParagraph p)
        public bool Select_FirstPositionInParagraph(DParagraph p)
        {
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara para in pile.Paragraphs)
                    {
                        if (para.DataSource == p)
                            return para.Select_BeginningOfFirstWord();
                    }
                }
            }
            return false;
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
        #region Cmd: OnKeyPress - most data entry
        protected override void OnKeyPress(KeyPressEventArgs e)
        {

            if (!Char.IsControl(e.KeyChar))
            {
                if (!AutoReplace(e.KeyChar))
                    cmdInsertChar(e.KeyChar);
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }
        #endregion

        // Text Changes ----------------------------------------------------------------------
        #region Cmd: cmdInsertChar(char)
        void cmdInsertChar(char ch)
        {
            if (HandleLockedFromEditing())
                return;

            Selection.Paragraph.Insert(ch.ToString());
        }
        #endregion
        #region Cmd: cmdDelete
        void cmdDelete()
        {
            if (HandleLockedFromEditing())
                return;

            Selection.Paragraph.Delete(OWPara.DeleteMode.kDelete);
        }
        #endregion
        #region Cmd: cmdBackspace
        void cmdBackspace()
        {
            if (HandleLockedFromEditing())
                return;

            Selection.Paragraph.Delete(OWPara.DeleteMode.kBackSpace);
        }
        #endregion
        #region Cmd: cmdCut
        public void cmdCut()
        {
            if (HandleLockedFromEditing())
                return;

            Selection.Paragraph.Delete(OWPara.DeleteMode.kCut);
        }
        #endregion
        #region Cmd: cmdCopy
        public void cmdCopy()
        {
            Selection.Paragraph.Delete(OWPara.DeleteMode.kCopy);
        }
        #endregion
        #region Cmd: cmdPaste
        public void cmdPaste()
        {
            if (HandleLockedFromEditing())
                return;

            if (!Clipboard.ContainsData(DataFormats.UnicodeText))
                return;

            string sClipboard = Clipboard.GetData(DataFormats.UnicodeText) as string;
            if (null == sClipboard || sClipboard.Length == 0)
                return;

            // Don't insert if too big; we figure the user has the wrong stuff on the clipboard!
            if (sClipboard.Length > 500)
                return;

            // Remove any characters we don't like
            string sInsert = "";
            char chPrev = '\0';
            foreach (char ch in sClipboard)
            {
                // Avoid inserting, e.g., line feeds, carriage returns
                if (char.IsControl(ch))
                    continue;

                // Try to avoid inserting multiple white spaces
                if (char.IsWhiteSpace(ch))
                {
                    if (char.IsWhiteSpace(chPrev))
                        continue;
                }

                // If we're here, we've approved the character. 
                sInsert += ch;

                // Want to rememebr the previous one for the Whitespace test
                chPrev = ch;
            }

            // Do the insertion
            Selection.Paragraph.Insert(sInsert);
        }
        #endregion
        #region Method: bool HandleLockedFromEditing()
        bool HandleLockedFromEditing()
        {
            // If not locked from editing, nothing further is needed
            if (!LockedFromEditing)
                return false;

            // If this is not the main window, then we do nothing. If 
            // we don't have a pointer in the MainWindow attr, then we
            // can infer that This is the main window.
            if (null != MainWindow)
                return false;

            // Retrieve the book we're working on
            DBook book = G.STarget.Book;

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
        #region Method: bool AutoReplace(char chKey)
        bool AutoReplace(char chKey)
        {
            // Check for conditions where we do nothing
            if (null == Selection)
                return false;
            if (HandleLockedFromEditing())
                return false;

            // Get the Writing System that has the autoreplace strings
            JWritingSystem jws = Selection.Paragraph.WritingSystem;

            // An Insertion Icon is simple, we just check for the chKey, and insert it
            // if a match
            if (Selection.IsInsertionIcon)
            {
                int c = 0;
                string sInsert = jws.SearchAutoReplace(chKey.ToString(), ref c);
                if (null == sInsert || sInsert.Length == 0)
                    return false;
                Selection.Paragraph.Insert(sInsert);
                return true;
            }

            // We are considering a content selection to be meaningless; thus from
            // here we are only interested in an Insertion Point.
            if (!Selection.IsInsertionPoint)
                return false;

            // Retrieve the string in this DBasicText up to the selection point
            string sDBT = (Selection.Paragraph.DisplayBT) ?
                Selection.DBT.ProseBTAsString :
                Selection.DBT.ContentsAsString;
            // Begin temporary crash code----The crash we've been struggling with-----
            if (Selection.DBT_iCharFirst > sDBT.Length)
            {
                string sPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                    Path.DirectorySeparatorChar + "OurWordErrorLog.txt";
                StreamWriter w = new StreamWriter(sPath, false);
                w.WriteLine("The Dreaded Edit Problem....");
                w.WriteLine("Version = " + G.Version);
                w.WriteLine(DateTime.Today.ToShortDateString());
                w.WriteLine("");
                w.WriteLine("In OWWindow.AutoReplace(chKey = \"" + chKey.ToString() + "\") ...");
                w.WriteLine("- sDBT = \"" + sDBT + "\"");
                w.WriteLine("- sDBT.Length = \"" + sDBT.Length.ToString() + "\"");
                w.WriteLine("- DBT_iCharFirst = \"" + Selection.DBT_iCharFirst.ToString() + "\"");
                w.WriteLine("");
                w.WriteLine("Selection Info...");
                w.WriteLine("- iRow = " + Selection.iRow.ToString());
                w.WriteLine("- iPile = " + Selection.iPile.ToString());
                w.WriteLine("- iPara = " + Selection.iParagraph.ToString());
                w.WriteLine("- SelectionString = \"" + Selection.SelectionString + "\" (should be empty)");
                w.WriteLine("- Anchor.iBlock = " + Selection.Anchor.iBlock.ToString());
                w.WriteLine("- Anchor.iChar = " + Selection.Anchor.iChar.ToString());
                w.WriteLine("- Anchor.EWord = \"" + Selection.Anchor.Word.Text + "\"");
                w.WriteLine("");
                w.WriteLine("Other Info...");
                w.WriteLine("- Project = " + G.Project.DisplayName);
                w.WriteLine("- Translation = " + G.TTranslation.DisplayName);
                w.WriteLine("- Book = " + G.TBook.DisplayName);
                w.WriteLine("- SectionNo = " + G.Project.Nav.SectionNo.ToString());
                w.Close();
                MessageBox.Show("OurWord is about to crash due to that pesky editing problem.\n\n" +
                    "The file OurWordErrorLog.txt has been created on your desktop. \n" +
                    "Please email it to John_Wimbish@tsco.org. \n\n" +
                    "A screen shot would also be very helpful. After closing this error\n" +
                    "message, press Alt-PrtScn, then paste the result into your email.\n\n" +
                    "Finally, please email the books:\n" + 
                    "  - " + G.TTranslation.DisplayName + ": " + G.TBook.DisplayName + "\n" +
                    "  - " + G.FTranslation.DisplayName + ": " + G.FBook.DisplayName + "\n\n" +
                    "Sorry for the inconvenience;...and thanks for your help, -John",
                    "OurWord", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // End temporary crash code-------------------------------------------------
            string sSource = sDBT.Substring(0, Selection.DBT_iCharFirst);

            // Append what was just typied
            sSource += chKey;

            // Check for a match
            int cSelectionCount = 0;
            string sReplace = jws.SearchAutoReplace(sSource, ref cSelectionCount);
            if (null == sReplace || sReplace.Length == 0)
                return false;
            Debug.Assert(cSelectionCount > 0);

            // The number of chars to selection is 1 less than what SearchAutoReplace
            // returns, because the return value included the key that was just typed.
            cSelectionCount--;

            // Make a selection so that we can delete the appropriate number of characters
            if (cSelectionCount > 0)
            {
                int i2 = Selection.DBT_iCharFirst;
                int i1 = i2 - cSelectionCount;
                Sel sel = Sel.CreateSel(Selection.Paragraph, Selection.DBT, i1, i2);
                if (null == sel)
                    return false;
                Selection = sel;
            }

            // Do the insertion
            Selection.Paragraph.Insert(sReplace);

            // Tell the caller that no further processing should be done
            return true;
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

            // Otherwise, we want to make a paragraph break
            cmdSplitParagraph();
        }
        #endregion
        #region Cmd: cmdSplitParagraph
        void cmdSplitParagraph()
        {
            if (HandleLockedFromEditing())
                return;

            // If we have a selection, we don't want to erase it. While deleting it is typical 
            // Microsoft Word behavior, I fear it would be unsettling to the MTT. So instead,
            // we move to the end of the selection.
            if (Selection.IsContentSelection)
                Selection = new Sel(Selection.Paragraph, Selection.Last);

            // Remember the scroll bar position so that we can attempt to scroll back to it
            float fScrollBarPosition = ScrollBarPosition;

            // Get the position where the split will happen
            DBasicText text = Selection.DBT;
            int iPos = Selection.DBT_iChar(Selection.Anchor);

            // Retrieve the underlying paragraph
            DParagraph para = text.Paragraph;
            if (null == para)
                return;

            // Split the underlying paragraph
            DParagraph paraNew = para.Split(text, iPos);

            // Re-Load the window's data. This is time-consuming, but it is the only way to make paragraphs line
            // correctly side-by-side.
            this.LoadData();

            // Select the first thing possible in the new paragraph
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara owp in pile.Paragraphs)
                    {
                        if (owp.DataSource as DParagraph == paraNew)
                        {
                            owp.Select_BeginningOfFirstWord();
                            ScrollBarPosition = fScrollBarPosition;
                            return;
                        }
                    }
                }
            }
            Debug.Assert(false);
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
        #region Method: EBlock GetBlockAt(PointF pt)
        public OWPara.EBlock GetBlockAt(PointF pt)
        {
            foreach (Row row in Rows)
            {
                OWPara.EBlock block = row.GetBlockAt(pt);
                if (null != block)
                    return block;
            }
            return null;
        }
        #endregion
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
            OWPara.EBlock block = GetBlockAt(pt);
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
                OWPara.EBlock block = GetBlockAt(pt);
                if (null != block)
                    Cursor = block.MouseOverCursor;
                else
                    Cursor = Cursors.Default;
            }

            if (m_MouseState == MouseStates.kSelectingText)
            {
                OWPara.EBlock block = GetBlockAt(pt);
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
            OWPara.EBlock block = GetBlockAt(pt);

            // Let the block handle it
            block.cmdLeftMouseDoubleClick(pt);

            m_MouseState = MouseStates.kNone;
        }
        #endregion
        #region Cmd: OnMouseWheel
        protected override void OnMouseWheel(MouseEventArgs e)
        {
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

        // Movement --------------------------------------------------------------------------
        #region Selection Movement & Extend Commands
        #region Cmd: cmdMoveTop - move cursor to first EWord in window
        void cmdMoveTop()
        {
            Select_FirstWord();
        }
        #endregion
        #region Cmd: cmdMoveBottom - move cursor to first EWord in window
        void cmdMoveBottom()
        {
            Select_LastWord();
        }
        #endregion
        #region Cmd: cmdMoveCharLeft - move cursor one char to the left
        void cmdMoveCharLeft()
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
            bool bSuccessful = Select_PreviousWord(Selection.iRow, Selection.iPile, 
                Selection.iParagraph, Selection.Anchor.iBlock - 1);

            // If this puts us at the end of a word, and if there was a EWord to our
            // right, then we need to decrement one more.
            if (bSuccessful && Selection.Anchor.Word.IsBesideEWord(true))
                cmdMoveCharLeft();
        }
        #endregion
        #region Cmd: cmdMoveCharRight - move cursor one char to the right
        void cmdMoveCharRight()
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
                    Selection.Anchor.iBlock < Selection.Paragraph.Blocks.Length - 1 &&
                    null != Selection.Paragraph.Blocks[Selection.Anchor.iBlock + 1] as OWPara.EWord)
                {
                    Select_NextWord(Selection.iRow, Selection.iPile, Selection.iParagraph,
                       Selection.Anchor.iBlock + 1);
                }

                return;
            }

            // If we've made it here, then we want to move to the beginning of the next
            // EWord (which may mean going to another paragraph/pile/row)
            Select_NextWord(Selection.iRow, Selection.iPile, Selection.iParagraph,
                Selection.Anchor.iBlock + 1);
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
                Select_PreviousWord(Selection.iRow, Selection.iPile, Selection.iParagraph,
                    Selection.Anchor.iBlock - 1);
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
            if (null != Selection)
            {
                Select_NextWord(Selection.iRow, Selection.iPile, Selection.iParagraph,
                    Selection.Last.iBlock + 1);
            }
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
            if (null == Selection)
                return;

            // Get the current position we're in
            int iPile = Selection.iPile;
            int iPara = Selection.iParagraph;

            // We ignore paragraphs until we find the one that is currently
            // selected.
            bool bParaFound = false;

            for (int iRow = Selection.iRow; iRow < Rows.Length; iRow++)
            {
                // Retrieve the Row and Pile. (We want the same column
                // as the current selection; so iPile does not change.)
                Row row = Rows[iRow];
                Row.Pile pile = row.Piles[iPile];

                // Loop through the pile's paragraphs
                foreach (OWPara p in pile.Paragraphs)
                {
                    // If we found the current paragraph at some point
                    // previously, then we're ready to attempt a selection.
                    // Once we succeed at getting a valid selection, then
                    // we're done and we exit the method.
                    if (bParaFound)
                    {
                        if (p.Select_BeginningOfFirstWord())
                            return;
                    }

                    // Signal that the current paragraph has been found. From
                    // here on out, any OWPara is a candidate.
                    if (p == Selection.Paragraph)
                        bParaFound = true;
                } // endforeach p
            } //endfor iRow
        }
        #endregion
        #region Cmd: cmdMoveNextBasicText() - e.g., in response to the Tab key
        void cmdMoveNextBasicText()
        {
            if (null == Selection)
                return;

            // Move to the end of the current DBasicText
            int iBlock = Selection.DBT_iBlockLast;
            OWPara.EWord word = Selection.Paragraph.Blocks[iBlock] as OWPara.EWord;
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
            OWPara.EWord word = Selection.Paragraph.Blocks[iBlock] as OWPara.EWord;
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

            // Figure out the starting points, which we obtain from the current selection.
            // We want to start the scan on the preceeding line.
            int iParaStart = Selection.iParagraph;
            OWPara.Line lnStart = Selection.Paragraph.LineContainingBlock(
                Selection.Last.Word);
            int iLineStart = Selection.Paragraph.IndexOfLine(lnStart) - 1;

            // Work through the rows, starting with the one that contains the selection
            bool bFirstTime = true;
            for (int iRow = Selection.iRow; iRow >=0; iRow--)
            {
                // We want to stay in the same column (pile), so retrieve that row's
                // appropriate pile if it exists.
                if (Rows[iRow].Piles.Length <= Selection.iPile)
                    continue;
                Row.Pile pile = Rows[iRow].Piles[Selection.iPile];

                // Work through the paragraphs in that pile. The first time around, we
                // start at the paragraph that contains the current selection; otherwise
                // we start at the last paragraph in the pile.
                if (!bFirstTime)
                    iParaStart = pile.Paragraphs.Length - 1;

                // Loop through the pile's paragraphs
                for (int iPara = iParaStart; iPara >= 0; iPara--)
                {
                    OWPara paragraph = pile.Paragraphs[iPara];

                    // Skip this paragraph if it does not allow editing
                    if (!paragraph.Editable)
                        continue;

                    if (!bFirstTime)
                        iLineStart = paragraph.Lines.Length - 1;

                    for (int iLine = iLineStart; iLine >= 0; iLine--)
                    {
                        OWPara.Line line = paragraph.Lines[iLine];
                        PointF pt = new PointF(x, line.Position.Y);

                        if (line.MakeSelectionClosestTo(pt))
                            return;
                    }

                    bFirstTime = false;
                }
                // We reset FirstTime here in case we never executted the inner loop.
                bFirstTime = false;
            }
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

            // Figure out the starting points, which we obtain from the current selection.
            // We want to start the scan on the following line.
            int iParaStart = Selection.iParagraph;
            OWPara.Line lnStart = Selection.Paragraph.LineContainingBlock(
                Selection.Last.Word);
            int iLineStart = Selection.Paragraph.IndexOfLine(lnStart) + 1;

            // Work through the rows, starting with the one that contains the selection
            for (int iRow = Selection.iRow; iRow < Rows.Length; iRow++)
            {
                // We want to stay in the same column (pile), so retrieve that row's
                // appropriate pile if it exists.
                if (Rows[iRow].Piles.Length <= Selection.iPile)
                    continue;
                Row.Pile pile = Rows[iRow].Piles[Selection.iPile];

                // Work through the paragraphs in that pile
                for (int iPara = iParaStart; iPara < pile.Paragraphs.Length; iPara++)
                {
                    OWPara paragraph = pile.Paragraphs[iPara];

                    // Skip this paragraph if it does not allow editing
                    if (!paragraph.Editable)
                        continue;

                    for (int iLine = iLineStart; iLine < paragraph.Lines.Length; iLine++)
                    {
                        OWPara.Line line = paragraph.Lines[iLine];
                        PointF pt = new PointF(x, line.Position.Y);

                        if (line.MakeSelectionClosestTo(pt))
                            return;
                    }
                    iLineStart = 0;
                }
                iParaStart = 0;
            }
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
            Selection.Paragraph.ExtendSelection_CharRight();
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
            Selection.Paragraph.ExtendSelection_CharLeft();
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
            Selection.Paragraph.ExtendSelection_WordRight();
        }
        #endregion
        #region Cmd: cmdExtendWordLeft
        void cmdExtendWordLeft()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;
            Selection.Paragraph.ExtendSelection_WordLeft();
        }
        #endregion
        #region Cmd: cmdExtendFarRight
        void cmdExtendFarRight()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;
            Selection.Paragraph.ExtendSelection_FarRight();
        }
        #endregion
        #region Cmd: cmdExtendFarLeft
        void cmdExtendFarLeft()
        {
            if (null == Selection || Selection.IsInsertionIcon)
                return;
            Selection.Paragraph.ExtendSelection_FarLeft();
        }
        #endregion
        #endregion

    }
}
