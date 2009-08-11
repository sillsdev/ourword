/**********************************************************************************************
 * Project: OurWord!
 * File:    OWPara.cs
 * Author:  John Wimbish
 * Created: 15 Mar 2007
 * Purpose: An individual paragraph
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion

namespace OurWord.Edit
{
    public class OWPara : EContainer
    {
        // Paragraph options -----------------------------------------------------------------
        #region Flags enum - SuppressVerseNumbers, ShowLineNumbers, IsEditable, ShowBackTranslation, etc.
        [Flags] public enum Flags { 
            None = 0,
            SuppressVerseNumbers = 1,
            ShowLineNumbers = 2,
            IsEditable = 4,                  // Can Type, Delete, Select, Copy, Cut, Paste
            IsLocked = (8 | IsEditable),     // Can only Select & Copy; no changes (requires IsEditable to enable Select/Copy)
            ShowBackTranslation = 16,
            CanRestructureParagraphs = 32,
            CanItalic = 64,                  // Italics are permitted in thie paragraph
            ShowIBT = 128                    // Show the interlinear back translation
        };
        #endregion
        #region Attr{g}: Flags Options
        public Flags Options
        {
            get
            {
                return m_Options;
            }
        }
        Flags m_Options = Flags.None;
        #endregion
        #region VAttr{g/s}: bool IsEditable - true if the paragraph can be edited
        override public bool IsEditable
        {
            get
            {
                return (Options & Flags.IsEditable) == Flags.IsEditable;
            }
            set
            {
                if (value)
                {
                    m_Options |= Flags.IsEditable;
                }
                else
                {
                    // Suppose we start with..........110101
                    // Reverse it.....................001010
                    m_Options = ~m_Options;

                    // Add in the IsEditable value....___1__
                    // yields.........................001110
                    m_Options |= Flags.IsEditable;

                    // Reverse it again...............110001
                    m_Options = ~m_Options;
                }
            }
        }
        #endregion
        #region VAttr{g}: bool DisplayBT
        public bool DisplayBT
        {
            get
            {
                return (Options & Flags.ShowBackTranslation) == Flags.ShowBackTranslation;
            }
        }
        #endregion
        #region VAttr{g}: bool SuppressVerseNumbers
        public bool SuppressVerseNumbers
        {
            get
            {
                return (Options & Flags.SuppressVerseNumbers) == Flags.SuppressVerseNumbers;
            }
        }
        #endregion
        #region VAttr{g}: bool ShowLineNumbers
        public bool ShowLineNumbers
        {
            get
            {
                return (Options & Flags.ShowLineNumbers) == Flags.ShowLineNumbers;
            }
        }
        #endregion
        #region VAttr{g}: bool CanRestructureParagraphs
        public bool CanRestructureParagraphs
        {
            get
            {
                return (Options & Flags.CanRestructureParagraphs) == Flags.CanRestructureParagraphs;
            }
        }
        #endregion
        #region VAttr{g}: bool IsLocked
        public bool IsLocked
        {
            get
            {
                return (Options & Flags.IsLocked) == Flags.IsLocked;
            }
        }
        #endregion
        #region VAttr{g}: bool CanItalic - true if italics are permitted in this paragraph
        public bool CanItalic
        {
            get
            {
                return (Options & Flags.CanItalic) == Flags.CanItalic;
            }
        }
        #endregion
        #region VAttr{g}: bool ShowIBT - true if the Interlinear BT should be shown
        public bool ShowIBT
        {
            get
            {
                return (Options & Flags.ShowIBT) == Flags.ShowIBT;
            }
        }
        #endregion

        #region VAttr{g}: bool CanInsertFootnote
        public bool CanInsertFootnote
        {
            get
            {
                if (!IsEditable)
                    return false;
                if (IsLocked)
                    return false;
                if (!CanRestructureParagraphs)
                    return false;

                if (!DB.Map.IsVernacularParagraph(PStyle.SFM))
                    return false;

                return true;
            }
        }
        #endregion
        #region VAttr{g}: bool CanDeleteFootnote
        public bool CanDeleteFootnote
        {
            get
            {
                if (!IsEditable)
                    return false;
                if (IsLocked)
                    return false;
                if (!OurWordMain.s_Features.F_StructuralEditing)
                    return false;

                if (DataSource as DFootnote == null)
                    return false;

                return true;

            }
        }
        #endregion

        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g}: JObject DataSource - the DParagraph or DNote behind this OWPara
        public JObject DataSource
        {
            get
            {
                // Because I allowo literals, sometimes paragraphs to not have data sources.
                // EContainer.FindContainerOfDataSource, e.g., needs DataSource to return
                // null, rather than fire an assertion.
                //    Once I get all OWPara's to have a data source, then I should
                // put this assertion back in.
                // Debug.Assert(null != m_objDataSource);

                return m_objDataSource;
            }
        }
        JObject m_objDataSource = null;
        #endregion

        // Blocks ----------------------------------------------------------------------------
        #region CLASS: EChapter
        public class EChapter : EBlock
        {
            // Screen Region -----------------------------------------------------------------
            #region OAttr{g}: float Height
            override public float Height
            {
                get
                {
                    // A chapter number takes up two lines
                    return Para.LineHeight * 2;
                }
                set
                {
                    Debug.Assert(false, "Can't set the line height of an EBlock");
                }
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(DChapter)
            public EChapter(JFontForWritingSystem f, DChapter chapter)
                : base(f, chapter.Text)
            {
                // Add a little space to the end so that it appears a bit nicer in the 
                // display. It is uneditable, so this only affects the display.
                m_sText = Text + "\u00A0";
            }
            #endregion

            #region Attr{g}: bool GlueToNext - Always T for a Chapter
            public override bool GlueToNext
            {
                get
                {
                    return true;
                }
                set
                {
                }
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
                // We want to center the chapter within a nice, pretty rectangular border
            {
                // Position "x" at the left margin
                float x = Position.X; 

                // Calculate "y" to be centered horizontally
                float y = Position.Y + (Height / 2) - (FontForWS.LineHeightZoomed / 2);

                // Draw the string
                Draw.String(Text, FontForWS.DefaultFontZoomed, GetBrush(), new PointF(x, y));
            }
            #endregion
        }
        #endregion
        #region CLASS: EVerse
        class EVerse : EBlock
        {
            const string c_sLeadingSpace = "  ";
            #region Attr{g}: bool NeedsExtraLeadingSpace - T if some extra leading padding is required.
            public bool NeedsExtraLeadingSpace
            {
                get
                {
                    return m_bNeedsExtraLeadingSpace;
                }
                set
                {
                    m_bNeedsExtraLeadingSpace = value;
                }
            }
            bool m_bNeedsExtraLeadingSpace = false;
            #endregion

            #region Constructor(DVerse)
            public EVerse(JFontForWritingSystem f, DVerse verse)
                : base(f, verse.Text)
            {
            }
            #endregion

            #region Attr{g}: bool GlueToNext - Always T for a Verse
            public override bool GlueToNext
            {
                get
                {
                    return true;
                }
                set
                {
                }
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
                // The verse size in the stylesheet reflects a normal style; we need to
                // decrease it for the superscript.
            {
                // If verse numbers are turned off, we potentially need to paint a white 
                // background.
                if (Para.SuppressVerseNumbers)
                {
                    if (Para.IsEditable && !Para.IsLocked)
                    {
                        RectangleF r = new RectangleF(Position, new SizeF(Width, Height));
                        Draw.FillRectangle(Para.EditableBackgroundColor, r);
                    }
                    return;
                }
               
                string s = Text;
                if (NeedsExtraLeadingSpace)
                    s = c_sLeadingSpace + Text;
                Draw.String(s, GetSuperscriptFont(), GetBrush(), Position);
            }
            #endregion

            // Layout Calculations ---------------------------------------------------------------
            #region OMethod: override void CalculateWidth(g)
            public override void CalculateWidth(Graphics g)
            {
                // The text we will measure
                string s = Text;

                // If we are not doing verse numbers, then we have nothing to measure
                if (Para.SuppressVerseNumbers)
                    s = "";

                // Leading space is needed if the verseno is not paragraph initial
                if (NeedsExtraLeadingSpace)
                    s = c_sLeadingSpace + s;

                // Don't bother measuring if nothing to measure
                if (string.IsNullOrEmpty(s))
                {
                    Width = 0;
                    return;
                }

                // Do the measurement
                StringFormat fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                Width = g.MeasureString(s, FontForWS.DefaultFontZoomed, 1000, fmt).Width;
            }
            #endregion

        }
        #endregion
        #region CLASS: EFoot
        public class EFoot : EBlock
        {
            #region Attr{g}: DFoot Foot
            DFoot Foot
            {
                get
                {
                    Debug.Assert(null != m_Foot);
                    return m_Foot;
                }
            }
            DFoot m_Foot;
            #endregion
            #region VAttr{g}: DFootnote Footnote
            public DFootnote Footnote
            {
                get
                {
                    Debug.Assert(null != Foot.Footnote);
                    return Foot.Footnote;
                }
            }
            #endregion

            #region Constructor(JFontForWritingSystem, DFoot)
            public EFoot(JFontForWritingSystem f, DFoot foot)
                : base(f, foot.Text)
            {
                m_Foot = foot;
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
            {
                Draw.String(Text, GetSuperscriptFont(), GetBrush(), Position);
            }
            #endregion

            // Explanatory Footnote
            #region VAttr{g}: bool FootnoteIsEditable - if T, we can jump to it by clicking on the letter
            bool FootnoteIsEditable
            // Test to see if the footnote is editable. If it is, then we
            // 1. Show the Hand cursor when we hover over it,
            // 2. Can jump to it
            //
            // This test will not work until the entire window has been laid out. 
            // If we don't have a selection in the window, then it is safe to
            // assume that the window is not ready.
            {
                get
                {
                    if (!m_bFootnoteIsEditableComputed)
                    {
                        if (!Foot.IsExplanatory)
                            return false;

                        // Can't do this if we don't have a selection
                        if (!Window.HasSelection)
                            return false;

                        // Remember our current selection
                        OWBookmark bm = Window.CreateBookmark();

                        // Attempt to select the footnote
                        EContainer container = Window.Contents.FindContainerOfDataSource(Footnote);
                        m_bFootnoteIsEditable = container.Select_FirstWord();

                        // Restore the original selection
                        bm.RestoreWindowSelectionAndScrollPosition();

                        // We did the analysis
                        m_bFootnoteIsEditableComputed = true;
                    }

                    return m_bFootnoteIsEditable;
                }
            }
            bool m_bFootnoteIsEditable;
            bool m_bFootnoteIsEditableComputed = false;
            #endregion

            #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
            public override Cursor MouseOverCursor
            {
                get
                {
                    if (FootnoteIsEditable)
                        return Cursors.Hand;
                    return Cursors.Arrow;
                }
            }
            #endregion
            #region Method: override void cmdLeftMouseClick(PointF pt)
            public override void cmdLeftMouseClick(PointF pt)
            {
                if (Foot.IsExplanatory)
                {
                    EContainer container = Window.Contents.FindContainerOfDataSource(Footnote);
                    container.Select_FirstWord();
                }
            }
            #endregion
        }
        #endregion
        #region CLASS: ELabel
        public class ELabel : EBlock
        {
            public const string c_Spaces = "\u00A0\u00A0";

            #region Constructor(JFontForWritingSystem, DLabel)
            public ELabel(JFontForWritingSystem f, DLabel label)
                : base(f, label.Text + c_Spaces)
            {
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
            {
                Draw.String(Text, FontForWS.DefaultFontZoomed, GetBrush(), Position);
            }
            #endregion
        }
        #endregion
        #region CLASS: ELiteral
        class ELiteral : EWord 
            // As a literal of EWord, hyphenation is possible.
        {
            #region Constructor(sText)
            public ELiteral(JFontForWritingSystem f, string sText)
                : base(f, null, sText, FontStyle.Regular)
            {
            }
            #endregion
            #region OMethod: EWord Clone()
            public override EWord Clone()
            {
               return new ELiteral(FontForWS, Text);
            }
            #endregion
            #region OMethod: Cursor MouseOverCursor
            public override Cursor MouseOverCursor
            {
                get
                {
                    return Cursors.Default;
                }
            }
            #endregion
            #region OMethod: void Paint()
            public override void Paint()
            {
                Draw.String(Text, FontForWS.DefaultFontZoomed, GetBrush(), Position);
            }
            #endregion
            #region OMethod: void cmdLeftMouseClick(PointF pt)
            public override void cmdLeftMouseClick(PointF pt)
                // No selection allowed
            {
                return;
            }
            #endregion
            #region OMethod: void cmdMouseMove(PointF pt)
            public override void cmdMouseMove(PointF pt)
            {
                return;
            }
            #endregion
            #region OMethod: void cmdLeftMouseDoubleClick(PointF pt)
            public override void cmdLeftMouseDoubleClick(PointF pt)
            {
                return;
            }
            #endregion
        }
        #endregion
        #region CLASS: ENote
        public class ENote : EBlock
        {
            #region Attr{g}: TranslatorNote Note
            public TranslatorNote Note
            {
                get
                {
                    Debug.Assert(null != m_Note);
                    return m_Note;
                }
            }
            TranslatorNote m_Note = null;
            #endregion
            #region Attr{g}: Bitmap Bmp - the note's bitmap
            Bitmap Bmp
            {
                get
                {
                    // Get this when we first need it. We previously had this in
                    // the constructor; but the problem was that we do not
                    // have access to the Window at the time of construction.
                    if (null == m_bmp)
                        m_bmp = TranslatorNote.GetBitmap(Window.BackColor);

                    Debug.Assert(null != m_bmp);
                    return m_bmp;
                }
            }
            Bitmap m_bmp = null;
            #endregion
            #region OAttr{g}: float Width
            public override float Width
            {
                get
                {
                    return Bmp.Width;
                }
                set
                {
                    // Can't be set; its the nature of the bitmap
                }
            }
            #endregion

            #region Constructor(TranslatorNote)
            public ENote(TranslatorNote _Note)
                : base(null, "")
            {
                m_Note = _Note;
            }
            #endregion
            #region OMethod: void CalculateWidth(Graphics g)
            public override void CalculateWidth(Graphics g)
            {
                // Do-nothing override
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
            {
                Draw.Image(Bmp, Position);
            }
            #endregion

            #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
            public override Cursor MouseOverCursor
            {
                get
                {
                    return Cursors.Hand;
                }
            }
            #endregion

            #region Method: override void cmdLeftMouseClick(PointF pt)
            public override void cmdLeftMouseClick(PointF pt)
            {
                // Update the secondary windows, if any
                Window.Secondary_SelectAndScrollToNote(Note);

                // Update the main window (if this isn't the main)
                if (null != Window.MainWindow)
                    Window.MainWindow.Contents.OnSelectAndScrollFrom(Note);
            }
            #endregion
        }
        #endregion
        #region CLASS: EFootnoteLabel
        class EFootnoteLabel : EBlock
        {
            #region Attr{g}: DFootnote Footnote
            DFootnote Footnote
            {
                get
                {
                    Debug.Assert(null != m_Footnote);
                    return m_Footnote;
                }
            }
            DFootnote m_Footnote = null;
            #endregion

            #region Constructor(DFootLetter)
            public EFootnoteLabel(JFontForWritingSystem f, DFootnote footnote)
                : base(f, footnote.Letter + " ")
            {             
                m_Footnote = footnote;
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
            {
                Draw.String(Text, GetSuperscriptFont(), GetBrush(), Position);
            }
            #endregion

            #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
            public override Cursor MouseOverCursor
            {
                get
                {
                    OWPara para = Window.Contents.FindParagraph(Footnote);
                    if (para != null)
                    {
                        if (para.IsEditable)
                            return Cursors.Hand;
                    }
                    return Cursors.Arrow;
                }
            }
            #endregion
            #region Method: override void cmdLeftMouseClick(PointF pt)
            public override void cmdLeftMouseClick(PointF pt)
            {
                Window.Contents.OnSelectAndScrollFrom(Footnote);
            }
            #endregion
        }
        #endregion
        #region CLASS: EBigHeader
        class EBigHeader : EBlock
        {
            #region Constructor(string sText)
            public EBigHeader(JFontForWritingSystem f, string sText)
                : base(f, sText + " ")
            {
            }
            #endregion

            #region Attr{g}: bool GlueToNext - Always T for a BigHeader
            public override bool GlueToNext
            {
                get
                {
                    return true;
                }
                set
                {
                }
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
            {
                Draw.String(Text, FontForWS.DefaultFontZoomed, GetBrush(), Position);
            }
            #endregion
        }
        #endregion

        // Initialize to a paragraph's contents ----------------------------------------------
        #region Method: EWord[] ParseBasicTextIntoWords(DBasicText t)
        EWord[] ParseBasicTextIntoWords(DBasicText t, JCharacterStyle CStyleOverride)
        {
            // Select between the Vernacular vs the Back Translation text
            DBasicText.DPhrases<DPhrase> phrases = (DisplayBT) ? t.PhrasesBT : t.Phrases;
            Debug.Assert(phrases.Count > 0);

            // We'll temporarily collect the EWords here
            ArrayList a = new ArrayList();

            // Loop through all of the phrases in this DBasicText
            foreach (DPhrase phrase in phrases)
            {
                // We'll collect individual words here
                string sWord = "";

                // Determine which character style goes with the word. We default to
                // the "Override" which is passed in, but if this is null, then we
                // get it from the normal source (the paragraph's style)
                FontStyle mods = FontStyle.Regular;
                JCharacterStyle cs = CStyleOverride;
                if (null == cs)
                {
                    cs = t.Paragraph.Style.CharacterStyle;
                    if (phrase.CharacterStyleAbbrev != DStyleSheet.c_sfmParagraph)
                    {
                        if (phrase.CharacterStyle.Abbrev == DStyleSheet.c_StyleAbbrevBold)
                            mods = FontStyle.Bold;
                        else if (phrase.CharacterStyle.Abbrev == DStyleSheet.c_StyleAbbrevItalic)
                            mods = FontStyle.Italic;
                        else
                            cs = phrase.CharacterStyle;
                    }
                }

                // Get the font for the style
                JFontForWritingSystem fontForWS = cs.FindOrAddFontForWritingSystem(WritingSystem);

                // Process through the phrase's text string
                for (int i = 0; i < phrase.Text.Length; i++)
                {
                    // If we are sitting at a word break, then add the word and reset
                    // in order to build the next one.
                    if (WritingSystem.IsWordBreak(phrase.Text, i) && sWord.Length > 0)
                    {
                        a.Add(new EWord(fontForWS, phrase, sWord, mods));
                        sWord = "";
                    }

                    // Add the character to the word we are building
                    sWord += phrase.Text[i];
                }

                // Pick up the final word in the string, IsWordBreak will not have 
                // caught it.
                if (sWord.Length > 0)
                    a.Add(new EWord(fontForWS, phrase, sWord, mods));
            }

            // If we did not find any words, then we want to create an InsertionIcon
            if (a.Count == 0)
            {
                JCharacterStyle cStyle = t.Paragraph.Style.CharacterStyle;
                JFontForWritingSystem fontForWS = cStyle.FindOrAddFontForWritingSystem(
                    WritingSystem);

                a.Add(EWord.CreateAsInsertionIcon(fontForWS, phrases[0]));
            }

            // Convert to an array of EWords
            EWord[] v = new EWord[a.Count];
            for (int k = 0; k < a.Count; k++)
                v[k] = a[k] as EWord;
            return v;
        }
        #endregion
        #region Method: void _InitializeBasicTextWords(DBasicText, CStyleOverride)
        void _InitializeBasicTextWords(DBasicText t, JCharacterStyle CStyleOverride)
        {
            EWord[] vWords = ParseBasicTextIntoWords(t, CStyleOverride);
            Append(vWords);
        }
        #endregion
        #region Method: void InitializeNoteIcons(DText)
        void InitializeNoteIcons(DText text)
        {
            foreach (TranslatorNote tn in text.TranslatorNotes)
            {
                if (tn.IsShown())
                    Append( new ENote(tn) );
            }
        }
        #endregion
        #region Method: void _InitializeGlueToNext(int iLeft)
        void _InitializeGlueToNext(int iLeft)
        {
            // We are interested both in the Block passed in (iLeft) and the word immediately
            // to its right.
            int iRight = iLeft + 1;

            // Make sure we've passed in words that exist in the list
            Debug.Assert(iLeft >= 0);
            Debug.Assert(iRight < SubItems.Length);

            // Retrieve their EBlock objects
            EBlock blockLeft = SubItems[iLeft] as EBlock;
            EBlock blockRight = SubItems[iRight] as EBlock;
            Debug.Assert(null != blockLeft && null != blockRight);

            // The default is not to glue
            blockLeft.GlueToNext = false;

            // But we do glue in the case, e.g., where we are followed by certain objects
            if (blockRight as EFoot != null)
                blockLeft.GlueToNext = true;
            if (blockRight as ENote != null)
                blockLeft.GlueToNext = true;
            if (blockLeft as EFootnoteLabel != null)
                blockLeft.GlueToNext = true;
        }
        #endregion
        #region Method: JFontForWritingSystem RetrieveFont(sCharStyleAbbrev)
        JFontForWritingSystem RetrieveFont(string sCharStyleAbbrev)
        {
            JCharacterStyle cs = DB.StyleSheet.FindCharacterStyle(sCharStyleAbbrev);
            return cs.FindOrAddFontForWritingSystem(WritingSystem);
        }
        #endregion
        #region Method: JFontForWritingSystem RetrieveFont()
        JFontForWritingSystem RetrieveFont()
        {
            return PStyle.CharacterStyle.FindOrAddFontForWritingSystem(WritingSystem);
        }
        #endregion
        #region Method: void _Initialize(DParagraph)
        void _Initialize(DParagraph p)
        {
            // Clear out any previous list contents
            Clear();

            // Compute the fonts
            JFontForWritingSystem fChapter = RetrieveFont(DStyleSheet.c_StyleAbbrevChapter);
            JFontForWritingSystem fVerse = RetrieveFont(DStyleSheet.c_StyleAbbrevVerse);
            JFontForWritingSystem fFootLetter = RetrieveFont(DStyleSheet.c_StyleAbbrevFootLetter);
            JFontForWritingSystem fLabel = RetrieveFont(DStyleSheet.c_StyleAbbrevLabel);
            JFontForWritingSystem fFootnoteLabel = fFootLetter;

            // If this is a footnote, we need to add its letter
            if (null != p as DFootnote)
                Append(new EFootnoteLabel(fFootnoteLabel, p as DFootnote)); 

            // Loop through the paragraph's runs
            foreach (DRun r in p.Runs)
            {
                switch (r.GetType().Name)
                {
                    case "DChapter":
                        Append(new EChapter(fChapter, r as DChapter));
                        break;
                    case "DVerse":
                        Append(new EVerse(fVerse, r as DVerse));
                        break;
                    case "DFoot":
                        Append(new EFoot(fFootLetter, r as DFoot));
                        break;
                    case "DLabel":
                        Append(new ELabel(fLabel, r as DLabel));
                        break;
                    case "DBasicText":
                        _InitializeBasicTextWords(r as DBasicText, null);
                        break;
                    case "DText":
                        _InitializeBasicTextWords(r as DBasicText, null);
                        if (G.App.HasSideWindows && G.App.SideWindows.HasNotesWindow)
                            InitializeNoteIcons(r as DText);
                        break;
                    default:
                        Console.WriteLine("Unknown type in OWPara.Initialize...Name=" + 
                            r.GetType().Name);
                        Debug.Assert(false);
                        break;
                }
            }

            // Loop through to set the GlueToNext values
            for (int i = 0; i < SubItems.Length - 1; i++)
                _InitializeGlueToNext(i);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Attr{g}: JWritingSystem WritingSystem
        public JWritingSystem WritingSystem
        {
            get
            {
                Debug.Assert(null != m_WritingSystem);
                return m_WritingSystem;
            }
        }
        JWritingSystem m_WritingSystem = null;
        #endregion
        #region Attr{g}: JParagraphStyle PStyle - pointer stored here for performance
        public JParagraphStyle PStyle
        {
            get
            {
                Debug.Assert(null != m_pStyle);
                return m_pStyle;
            }
        }
        JParagraphStyle m_pStyle = null;
        #endregion
        #region Attr{g}: float LineHeight
        public float LineHeight
        {
            get
            {
                Debug.Assert(0.0F != m_fLineHeight);

                // Round up to nearest int. The problem is that if line spacing is rounded down,
                // occasionally on some machines, it results in lines overlapping, and thus, e.g.,
                // in some fonts, a "g" has the bottom removed and looks like a "q".
                return (float)((int)m_fLineHeight);
            }
        }
        float m_fLineHeight = 0;
        #endregion      

        #region private Constructor(JWS, PStyle, JObject, flags) - the stuff that is in common
        private OWPara(
            JWritingSystem _ws,
            JParagraphStyle _PStyle,
            JObject _objDataSource,
            Flags _Options)
            : base()
        {
            // Keep track of the attributes passed in
            m_WritingSystem = _ws;
            m_pStyle = _PStyle;
            m_objDataSource = _objDataSource;
            m_Options = _Options;

            // Initialize the vector of Blocks
            Clear();

            // Initialize the list of Lines
            m_vLines = new Line[0];
        }
        #endregion
        #region Constructor(JWS, PStyle, DParagraph, clrEditableBackground, Flags) - for DParagraph
        public OWPara(
            JWritingSystem _ws, 
            JParagraphStyle PStyle,
            DParagraph p,
            Color clrEditableBackground, 
            Flags _Options)
            : this(_ws, PStyle, p as JObject, _Options)
        {
            // The paragraph itself may override to make itself uneditable, 
            // even though we received "true" from the _bEditable parameter.
            if (!p.IsUserEditable)
                IsEditable = false;

            // Line height
            m_fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                WritingSystem).LineHeightZoomed;

            // Interpret the paragraph's contents into our internal data structure
            _Initialize(p);

            // Retrieve the background color for editable parts of the paragraph
            m_EditableBackgroundColor = clrEditableBackground;
        }
        #endregion
        #region Constructor(JWS, PStyle, DRun[], sLabel, Flags)
        public OWPara(
            JWritingSystem _ws,
            JParagraphStyle PStyle,
            DRun[] vRuns, 
            string sLabel, 
            Flags _Options)
            : this(_ws, PStyle, vRuns[0].Owner, _Options)
            // For the Related Languages window
        {
            // Line height
            m_fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                WritingSystem).LineHeightZoomed;

            // We'll add the language name as a BigHeader
            if (!string.IsNullOrEmpty(sLabel))
            {
                JFontForWritingSystem f = RetrieveFont(DStyleSheet.c_StyleAbbrevBigHeader);
                Append(new EBigHeader(f, /*WritingSystem,*/ sLabel));
            }

            // Add the text (we only care about verses and text)
            foreach (DRun run in vRuns)
            {
                switch (run.GetType().Name)
                {
                    case "DVerse":
                        Append(new EVerse(RetrieveFont(DStyleSheet.c_StyleAbbrevVerse), 
                            run as DVerse));
                        break;
                    case "DText":
                    case "DBasicText":
                        _InitializeBasicTextWords(run as DBasicText, PStyle.CharacterStyle);
                        break;
                }
            }

            // Special situation: When adding Related Languages, we may be placing data from multiple
            // DParagraphs into a single OWPara. The problem is that DParagraphs never end in spaces,
            // and so the end of a DParagraph that joins up to the beginning of another DParagraph
            // will have words that "runtogether" thusly. So we have this kludge here to append
            // a space to such EWords.
            for (int i = 0; i < SubItems.Length - 1; i++)
            {
                EWord word = SubItems[i] as EWord;
                if (null == word || !word.IsBesideEWord(true))
                    continue;
                if (!word.EndsWithWhiteSpace)
                    word.Text += ' ';
            }
        }
        #endregion
        #region Constructor(JWS, PStyle, sLiteralString)
        public OWPara(
            JWritingSystem _ws, 
            JParagraphStyle pStyle, 
            string sLiteralText)
            : this(_ws, pStyle, new DPhrase[] { new DPhrase(null, sLiteralText) })
        {
        }
        #endregion
        #region Constructor(JWS, PStyle, DPhrase[] vLiteralPhrases)
        public OWPara(
            JWritingSystem _ws, 
            JParagraphStyle pStyle, 
            DPhrase[] vLiteralPhrases)
            : this(_ws, pStyle, (JObject)null, Flags.None)
        {
            // Line height
            m_fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                WritingSystem).LineHeightZoomed;

            // Each Literal String will potentially have its own character style
            foreach (DPhrase p in vLiteralPhrases)
            {
                // The Split method we're about to call will remove spaces, including
                // a trailing one. We need to know if we had a trailing one, so we
                // can add it back in.
                bool bEndsWithSpace = false;
                if (p.Text.Length > 0 && p.Text[p.Text.Length - 1] == ' ')
                    bEndsWithSpace = true;

                // Parse the Literal Test into its parts
                string[] vs = p.Text.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                // Create the literal strings
                for (int i = 0; i < vs.Length; i++)
                {
                    if (i < vs.Length - 1 || bEndsWithSpace)
                        vs[i] = vs[i] + " ";

                    // Figure out the font for this literal string
                    // Start with the paragraph's character style
                    JCharacterStyle cs = PStyle.CharacterStyle;
                    // Override with the phrases's character style if it is different
                    if (!string.IsNullOrEmpty(p.CharacterStyleAbbrev) &&
                        p.CharacterStyleAbbrev != DStyleSheet.c_sfmParagraph)
                    {
						JStyleSheet ss = PStyle.StyleSheet;
						cs = ss.FindCharacterStyle(p.CharacterStyleAbbrev);
                        //cs = G.StyleSheet.FindCharacterStyle(p.CharacterStyleAbbrev);
                    }
                    // Get the font from whatever character style we wound up with
                    JFontForWritingSystem f = cs.FindOrAddFontForWritingSystem(WritingSystem);

                    // Add the literal
                    Append(new ELiteral(f, vs[i]));
                }
            }
        }
        #endregion

        // Layout Dependant ------------------------------------------------------------------
        #region CLASS: Line
        public class Line : EContainer
        {
            // Content Attrs
            #region Attr{g/s}: EChapter Chapter - the EChapter prior to this line (or null if none there)
            public EChapter Chapter
            {
                get
                {
                    return m_Chapter;
                }
                set
                {
                    Debug.Assert(null != value);
                    m_Chapter = value;
                }
            }
            EChapter m_Chapter = null;
            #endregion

            #region Attr{g/s}: float LeftIndent - how much to indent the line for the chapter
            public float LeftIndent
            {
                get
                {
                    return m_fLeftIndent;
                }
                set
                {
                    m_fLeftIndent = value;
                }
            }
            float m_fLeftIndent = 0;
            #endregion
            #region OAttr{g}: float Width
            override public float Width
            {
                get
                {
                    float f = 0;
                    foreach (EItem item in SubItems)
                        f += item.Width;
                    return f;
                }
                set
                {
                }
            }
            #endregion
            #region OAttr{g} float Height
            public override float Height
            {
                get
                {
                    if (Count == 0)
                        return 0;
                    return SubItems[0].Height;
                }
                set
                {
                }
            }
            #endregion

            #region OMethod: void AddParagraph(EItem item)
            public override void Append(EItem item)
                // Override in order to make sure the Line doesn't become the owner, as until we
                // do a rewrite, the Line isn't directly in the ownership hierarchy.
            {
                EContainer container = item.Owner;
                base.Append(item);
                item.Owner = container;
            }
            #endregion

            // Line numbers ------------------------------------------------------------------
            #region Attr{g}: int LineNo
            public int LineNo
            {
                get
                {
                    return m_nLineNo;
                }
                set
                {
                    m_nLineNo = value;
                }
            }
            int m_nLineNo = -1;
            #endregion
            #region Method: void PaintLineNumber(OWWindow window, OWPara para)
            public void PaintLineNumber(OWWindow window, OWPara para)
            {
                // No lines to paint
                if (Count == 0)
                    return;

                // Lines in, e.g., uneditable paragraphs, where we don't show their line numbers
                if (LineNo == -1)
                    return;

                // Get the string we'll draw, including trailing space for a margin
                string s = LineNo.ToString() + " ";

                // Calculate the width of this number
                float fWidth = window.Draw.Measure(s, window.LineNumberAttrs.Font);

                // The X coordinate is the x of the window (root) left, 
                float x = para.Root.Position.X;
                // plus the width allocated to columns
                x += window.LineNumberAttrs.ColumnWidth;
                // Less the space needed to draw this number
                x -= fWidth;

                // The Y coordinate is the y of the first block
                float y = SubItems[0].Position.Y;

                // Draw the line number
                window.Draw.String(s, window.LineNumberAttrs.Font,
                    window.LineNumberAttrs.Brush, new PointF(x, y));
            }
            #endregion

            #region Constructor()
            public Line()
                : base()
            {
            }
            #endregion

            #region Method: void SetPositions(float x, float y, float xMaxWidth, bool bJustify)
            public void SetPositions(float x, float y, float xWidthToFill, bool bJustify)
            {
                // Remember the position of this line
                Position = new PointF(x, y);

                // Indent for the chapter if necessary
                x += LeftIndent;

                // Calculate how many pixels we have to justify
                float fRawWidthOfMaterial = LeftIndent + Width;
                int cPixelsToJustify = (int)(xWidthToFill - fRawWidthOfMaterial);

                // Calculate how many positions we can add these pixels to
                int cJustificationPositions = Count - 1;
                foreach (EBlock block in SubItems)
                {
                    if (block.GlueToNext)
                        cJustificationPositions--;
                }

                // Calculate how many pixels to add to each position
                int cPadding = 0;
                int cRemainder = 0;
                if (cJustificationPositions > 0)
                {
                    cPadding = (int)((float)cPixelsToJustify / (float)cJustificationPositions);
                    cRemainder = cPixelsToJustify - (cPadding * cJustificationPositions);
                }

                foreach (EBlock block in SubItems)
                {
                    block.Position = new PointF(x, y);
                    x += block.Width;

                    // Add the justification for padding
                    block.JustificationPaddingAdded = 0;
                    if (bJustify && !block.GlueToNext)
                    {
                        // We'll certainly add the standard padding amount
                        int nPad = cPadding;

                        // We'll also work our way through any remainder
                        if (cRemainder > 0)
                        {
                            nPad++;
                            cRemainder--;
                        }

                        // Add the final answer to X (and to the block for future reference)
                        x += nPad;
                        if (block != SubItems[Count - 1])
                            block.JustificationPaddingAdded = nPad;
                    }
                }
            }
            #endregion

            #region OMethod: EBlock GetBlockAt(PointF pt)
            public override EBlock GetBlockAt(PointF pt)
            {
                // Is it in the EChapter?
                if (null != Chapter)
                {
                    if (Chapter.ContainsPoint(pt))
                        return Chapter;
                }

                // Don't call the base class; it gets confused, as witnessed by the notes
                // window, where the Hand cursor shows up while moving over the text of a
                // note. Thus,
                //    return base.GetBlockAt(pt);
                // didn't work.
                foreach (EBlock block in SubItems)
                {
                    if (block.ContainsPoint(pt))
                        return block;
                }
                return null;

            }
            #endregion
            #region Method: bool Contains(EItem)
            public override bool Contains(EItem item)
            {
                if (item == Chapter)
                    return true;

                return base.Contains(item);
            }
            #endregion

            #region Method: bool MakeSelectionClosestTo(PointF pt)
            public bool MakeSelectionClosestTo(PointF pt)
            // In response to the LineUp/LineDown keyboarding (up & down arrows), we need to find
            // the spot on a line closest so the requested pt.
            {
                // Attempt to find a block exactly where we want it, that is, at the indicated "x".
                EBlock block = GetBlockAt(pt);
                if (null != block as EWord)
                {
                    EWord word = block as EWord;
                    int iBlock = word.PositionWithinPara;
                    int iChar = word.GetCharacterIndex(pt);
                    block.Window.Selection = new OWWindow.Sel(word.Para,
                        new OWWindow.Sel.SelPoint(iBlock, iChar));
                    return true;
                }

                // Not possible, so perhaps the line is too short, or perhaps the Block there
                // is a verse number or some other uneditable. So......
                // Examine all of the blocks in the line for the one which is closest to x
                float fDistance = 10000;
                EWord ClosestWord = null;
                bool bBeginningIsClosest = true;
                foreach (EItem b in SubItems)
                {
                    // Retrieve each EWord in the line
                    EWord w = b as EWord;
                    if (null == w)
                        continue;

                    // Calculate its distance
                    float d1 = Math.Abs(pt.X - w.Position.X);
                    float d2 = Math.Abs(pt.X - (w.Position.X + w.Width));
                    float d = Math.Min(d1, d2);

                    // Do we have a new shortest one?
                    if (d < fDistance)
                    {
                        ClosestWord = w;
                        fDistance = d;
                        bBeginningIsClosest = (d1 < d2);
                    }
                }

                // If we found a block, then put the selection into it
                if (null != ClosestWord)
                {
                    int iBlock = ClosestWord.PositionWithinPara;
                    int iChar = (bBeginningIsClosest) ? 0 : ClosestWord.Text.Length;
                    ClosestWord.Window.Selection = new OWWindow.Sel(
                        ClosestWord.Para,
                        new OWWindow.Sel.SelPoint(iBlock, iChar));
                    return true;
                }

                return false;
            }
            #endregion
        }
        #endregion
        #region Attr{g}: Line[] Lines - the list of Lines in the currently calculated layout
        public Line[] Lines
        {
            get
            {
                Debug.Assert(null != m_vLines);
                return m_vLines;
            }
        }
        Line[] m_vLines = null;
        #endregion
        #region Method: void AddLine(Line line)
        void AddLine(Line line)
        {
            Line[] v = new Line[Lines.Length + 1];

            for (int i = 0; i < Lines.Length; i++)
                v[i] = Lines[i];

            v[Lines.Length] = line;

            m_vLines = v;
        }
        #endregion
        #region Method: void ClearLines()
        void ClearLines()
        {
            m_vLines = new Line[0];
        }
        #endregion
        #region Method: Line LineContainingBlock(EBlock block)
        public Line LineContainingBlock(EBlock block)
        {
            foreach (Line ln in Lines)
            {
                if (ln.Contains(block))
                    return ln;
            }
            return null;
        }
        #endregion
        #region Method: int IndexOfLine(Line ln)
        public int IndexOfLine(Line ln)
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] == ln)
                    return i;
            }
            return -1;
        }
        #endregion

        #region CLASS/Method: ProposeNextLayoutChunk
        public class ProposeNextLayoutChunk
        {
            // Attrs: Input params
            #region Attr{g}: int iStartItem
            int iStartItem
            {
                get
                {
                    return c_iStartItem;
                }
            }
            int c_iStartItem;
            #endregion
            #region Attr{g}: OWPara Para
            OWPara Para
            {
                get
                {
                    Debug.Assert(null != m_Para);
                    return m_Para;
                }
            }
            OWPara m_Para;
            #endregion
            #region Attr{g/s}: float AvailableWidth
            float AvailableWidth
            {
                get
                {
                    Debug.Assert(-1 != m_fAvailableWidth);
                    return m_fAvailableWidth;
                }
            }
            float m_fAvailableWidth = -1;
            #endregion
            #region Attr{g}: Graphics G
            Graphics G
            {
                get
                {
                    Debug.Assert(null != m_g);
                    return m_g;
                }
            }
            Graphics m_g;
            #endregion

            // Attrs: The Answer
            #region Attr{g/s}: int ChunkSize
            public int ChunkSize
            {
                get
                {
                    Debug.Assert(-1 != m_cChunkSize);
                    return m_cChunkSize;
                }
                set
                {
                    m_cChunkSize = value;
                }
            }
            int m_cChunkSize = -1;
            #endregion
            #region Attr{g/s}: float ChunkWidth
            public float ChunkWidth
            {
                get
                {
                    Debug.Assert(-1 != m_fChunkWidth);
                    return m_fChunkWidth;
                }
                set
                {
                    m_fChunkWidth = value;
                }
            }
            float m_fChunkWidth = -1;
            #endregion
            #region Attr{g}: bool TooLarge
            public bool TooLarge
            {
                get
                {
                    return m_bTooLarge;
                }
            }
            bool m_bTooLarge;
            #endregion
            #region VAttr{g}: string DebugHelper
            public string DebugHelper
            {
                get
                {
                    string s = "Size=" + ChunkSize.ToString();
                    s += "  Large=" + (TooLarge?"T ":"F ");
                    for(int i=0; i<ChunkSize; i++)
                        s += (Para.SubItems[iStartItem + i] as EBlock).Text;
                    return s;

                }
            }
            #endregion
            #region Attr{g}: bool ForcedHyphen - If T, we hyphened regardless of writing system
            public bool ForcedHyphen
            {
                get
                {
                    return m_bForcedHyphen;
                }
            }
            bool m_bForcedHyphen;
            #endregion

            // Helper Methods
            #region Method: void CalculateChunkContents()
            void CalculateChunkContents()
            {
                ChunkSize = 0;
                ChunkWidth = 0;

                for (int i = iStartItem; i < Para.SubItems.Length; i++)
                {
                    // Get the next block: could be a verse, footnote letter, etc.
                    EBlock block = Para.SubItems[i] as EBlock;

                    // Keep track of our chunk width and size thus far
                    ChunkWidth += block.Width;
                    ChunkSize++;

                    // If we aren't glued to next, then we've identified our chunk size
                    if (!block.GlueToNext)
                        break;
                }
            }
            #endregion
            #region VAttr{g}: bool ChunkFitsWithinWidth
            bool ChunkFitsWithinWidth
            {
                get
                {
                    if (ChunkWidth <= AvailableWidth)
                        return true;
                    return false;
                }
            }
            #endregion
            #region VAttr{g}: EWord HyphenedWord
            EWord HyphenedWord
            {
                get
                {
                    for (int i = iStartItem; i < iStartItem + ChunkSize; i++)
                    {
                        if (Para.SubItems[i] as EWord != null)
                            return Para.SubItems[i] as EWord;
                    }
                    return null;
                }
            }
            #endregion
            #region VAttr{g}: EWord OverflowWord
            EWord OverflowWord
            {
                get
                {
                    EWord wordHyphen = HyphenedWord;
                    if (null == wordHyphen)
                        return null;

                    int iWordOverflow = Para.Find(wordHyphen) + 1;
                    if( iWordOverflow >= Para.SubItems.Length)
                        return null;
                    EWord wordOverflow = Para.SubItems[iWordOverflow] as EWord;
                    return wordOverflow;
                }
            }
            #endregion

            // Hyphenation
            #region Attr{g/s}: int iHyphenPos
            int iHyphenPos
            {
                get
                {
                    Debug.Assert(-1 != m_iHyphenPos);
                    return m_iHyphenPos;
                }
                set
                {
                    m_iHyphenPos = value;
                }
            }
            int m_iHyphenPos = -1;
            #endregion
            #region Attr{g}: string OriginalTextToHyphen
            string OriginalTextToHyphen
            {
                get
                {
                    return m_sOriginalWordToHyphen;
                }
            }
            string m_sOriginalWordToHyphen = "";
            #endregion
            #region Method: bool CalcNextHyphenPos()
            bool CalcNextHyphenPos()
                // Returns false if there are no hyphenation positions
            {
                EWord word = HyphenedWord;
                JWritingSystem ws = word.FontForWS.WritingSystem;
                string s = word.Text;

                for (iHyphenPos = s.Length - 1; iHyphenPos > 0; iHyphenPos--)
                {
                    // This is triggered if a hyphen is required, because there
                    // is nothing on the line yet, and we've already tried
                    // the normal way of asking the writing system. I.e.,
                    // last resort.
                    if (ForcedHyphen)
                        return true;

                    // This is the preferred way of doing a hyphen: asking the
                    // writing system if we can.
                    if (ws.IsHyphenBreak(OriginalTextToHyphen, iHyphenPos))
                        return true;
                }

                return false;
            }
            #endregion
            #region Method: void CreateHyphenedWordPair()
            void CreateHyphenedWordPair()
                // Creates the hyphened word / overflow word pair, if they do
                // not already exist
            {
                EWord word = HyphenedWord;

                // Already done
                if (word.Hyphenated == true)
                    return;

                // Create a new, empty word
                EWord wordNew = word.Clone();
                wordNew.Text = "";

                // Insert it after our word
                int iPos = Para.Find(word);
                Para.InsertAt(iPos + 1, wordNew);

                // We now have a hyphenated word
                word.Hyphenated = true;
            }
            #endregion
            #region Method: void MoveTextIntoOverflowWord()
            void MoveTextIntoOverflowWord()
            {
                EWord wordHyphen = HyphenedWord;
                Debug.Assert(null != wordHyphen);
                EWord wordOverflow = OverflowWord;
                Debug.Assert(null != wordOverflow);

                wordHyphen.Text = OriginalTextToHyphen.Substring(0, iHyphenPos);
                wordOverflow.Text = OriginalTextToHyphen.Substring(iHyphenPos);

                // The words now have new lengths
                wordHyphen.CalculateWidth(G);
                wordOverflow.CalculateWidth(G);
            }
            #endregion
            #region Method: void RemoveAnyHyphenation()
            void RemoveAnyHyphenation()
                // If we just can't fit, we need to remove any attempt at hyphenation we
                // may have done.
            {
                m_bTooLarge = true;

                EWord word = HyphenedWord;
                if (null == word)
                    return;

                if (!word.Hyphenated)
                    return;

                if (word.Text == OriginalTextToHyphen)
                    return;

                EWord wordOverflow = OverflowWord;
                Debug.Assert(null != wordOverflow);

                word.Text += wordOverflow.Text;
                word.Hyphenated = wordOverflow.Hyphenated;
                word.CalculateWidth(G);

                Para.Remove(wordOverflow);
            }
            #endregion

            // Public Interface
            #region Constructor(g, para, fAvailableWidth, iStartItem)
            public ProposeNextLayoutChunk(Graphics g, 
                OWPara para, 
                float fAvailableWidth, 
                int iStartItem,
                bool bMustForceHyphen)
            {
                m_g = g;
                m_Para = para;
                m_fAvailableWidth = fAvailableWidth;
                c_iStartItem = iStartItem;

                while (true)
                {
                    CalculateChunkContents();

                    // This chunk fits; it qualifies to place into the line
                    if (ChunkFitsWithinWidth)
                    {
                        m_bTooLarge = false;
                        return;
                    }

                    // If there is no hyphenable entity, then we are by definition 
                    // too large.
                    if (null == HyphenedWord)
                    {
                        m_bTooLarge = true;
                        return;
                    }

                    // If our first time through, make a copy of what we'll be hyphenating,
                    // so we'll have access to its full context
                    if (string.IsNullOrEmpty(OriginalTextToHyphen))
                        m_sOriginalWordToHyphen = HyphenedWord.Text;

                    // Calculate the next hyphenation break, if there is one. If there
                    // isn't, then this chunk will not fit into the line.
                    if (CalcNextHyphenPos() == false)
                    {

                        // This means we've yet to put anything on the current line. So
                        // even though CalcNextHyphenPos turned out negative, we now
                        // have to reset and try again, this time not asking the 
                        // writing system, but instead, just carving off letters until
                        // the resultant word fits on the line.
                        if (bMustForceHyphen)
                        {
                            RemoveAnyHyphenation();
                            m_bForcedHyphen = true;
                            iHyphenPos = HyphenedWord.Text.Length - 1;
                            continue;
                        }

                        RemoveAnyHyphenation();
                        m_bTooLarge = true;
                        return;
                    }

                    // Create a hyphenated word-pair (hyphened word and overflow word) if
                    // such do not already exist.
                    CreateHyphenedWordPair();

                    // Move the hyphenated text from left to right
                    MoveTextIntoOverflowWord();
                }
            }
            #endregion
        }
        #endregion
        #region Method: void RemoveHyphenation()
        public void RemoveHyphenation()
        {
            for (int i = 0; i < SubItems.Length - 1; )
            {
                // Is this a hyphenated word?
                EWord word = SubItems[i] as EWord;
                if (null == word)
                    goto loop;
                if (!word.Hyphenated)
                    goto loop;

                // Is the next item a word?
                EWord wordNext = SubItems[i + 1] as EWord;
                if (null == wordNext)
                {
                    word.Hyphenated = false;
                    goto loop;
                }

                // If yes, combine the two (but don't loop, because we may
                // be continuing a hyphenation into even the next word.)
                word.Text = word.Text + wordNext.Text;
                word.Hyphenated = wordNext.Hyphenated;
                word.CalculateWidth(Window.Draw.Graphics);
                RemoveAt(i+1);
                continue;

            loop:
                i++;
            }
        }
        #endregion

        #region Method: float Layout_CalcNextChunkWidth(...)
        float Layout_CalcNextChunkWidth(Graphics g, int i, out int cChunkSize)
            // Calculate the width of the next "chunk" that we would add to the line, if
            // it will fit.
        {
            // Retrieve the first element (the one at "i")
            EBlock block = SubItems[i] as EBlock;

            // Measure the proposed first element
            float fWidth = block.Width;

            // We'll define the chunk as being One element in size
            cChunkSize = 1;

            // If it is glued to the next one, then add the next one's measurements
            for (int k = i; k < SubItems.Length - 1; k++)
            {
                // Is this one glued to the next one? 
                if (!block.GlueToNext)
                    break;

                // We got glue, so Measure and add
                EBlock blockNext = SubItems[k + 1] as EBlock;
                float fWidthNext = blockNext.Width;
                fWidth += fWidthNext;

                // The chunk must be big enough to include this glue'd word
                cChunkSize++;

                // Ready for the next one
                block = blockNext;
            }

            return fWidth;
        }
        #endregion
        #region Method: void Layout_SetCoordinates(...)
        void Layout_SetCoordinates(Graphics g, PointF ptPos, float xMaxWidth)
        {
            // We'll use "y" to indicate the height of each line
            float y = ptPos.Y;

            foreach (Line ln in Lines)
            {
                // Whether or not the line is justified depends first on the
                // paragraph style; but the final line is not justified in
                // any case.
                bool bJustify = PStyle.IsJustified;
                if (ln == Lines[Lines.Length - 1])
                    bJustify = false;

                // For Left and Justified paragraphs, the line starts at the ptPos's x value
                float x = ptPos.X;            // Start of line for Left and Justified

                // The X position must be shifted if the LineNumbers column is turned on.
                if (ShowLineNumbers)
                    x += Window.LineNumberAttrs.ColumnWidth;

                // If the line has a chapter, we set its position here, now that we know
                // what the X position of the line is.
                if (null != ln.Chapter)
                    ln.Chapter.Position = new PointF(x, y);

                // The X position depends upon the paragraph alignment. Note that
                // the first line has to also allow for the paragraph's FirstLineIndent
                // (assuming there is no chapter number; as we ignore first-line indentation
                // where we have chapter numbers)
                if (ln == Lines[0] && null == ln.Chapter)
                    x += (float)PStyle.FirstLineIndent * g.DpiX;
                if (PStyle.IsRight)
                    x += (xMaxWidth - ln.Width);
                if (PStyle.IsCentered)
                    x += (xMaxWidth - ln.Width) / 2;

                // Calculate the width to be filled. If we are working on the first line, we need
                // to adjust that width. Thus, e.g., for a negative line indent, we want to increase
                // the fill-width. Hence we subtract.
                float xWidthToFill = xMaxWidth;
                if (ln == Lines[0])
                    xWidthToFill -= (float)PStyle.FirstLineIndent * g.DpiX;

                ln.SetPositions(x, y, xWidthToFill, bJustify);
                y += LineHeight;
            }

            // The paragraph's "raw" or content height is what we wind up with after
            // the final line. THis represents the height of the lines, but does not include
            // the space before and after.
            Height = y - ptPos.Y;
        }
        #endregion

        #region OMethod: void CalculateVerticals(y, bRepositionOnly)
        public override void CalculateVerticals(float y, bool bRepositionOnly)
        // y - The top coordinate of the paragraph. We'll use "y" to work through the 
        //     height of the paragraph, setting the individula paragraph parts.
        #region DOC - Leading Space before Verses
        /* Leading Space before Verses - Whether or not to have leading space before
             *   a verse is a Layout issue. If the verse number is at the beginning of a
             *   line, no leading space is required. But if it is in the middle of a line,
             *   then we need the space.
             *      What I do is to set any Verse I come across as needing the space. 
             *   Then, if we create a new line, and the verse is the first item, we declare
             *   that it does not need the leading space. After each declaration, we must
             *   call MeasureWidth(g) to recalculate the width.
             *      I needed to do this to make it look correct, but I sure do dislike
             *   adding little things like this, due to the Bug potential.
             */
        #endregion
        {
            if (bRepositionOnly)
                RePosition(y);

            Graphics g = Window.Draw.Graphics;

            // Combine all hyphenated words, we'll figure out shortly if we must re-hyphenate
            RemoveHyphenation();

            // Remember the top coordinate
            Position = new PointF(Position.X, y);

            // We'll adjust the "internal" width based on the paragraph style
            float xMaxWidth = Width;

            // Decrease the width by the paragraph margins
            xMaxWidth -= (float)PStyle.LeftMargin * g.DpiX;
            xMaxWidth -= (float)PStyle.RightMargin * g.DpiX;

            // Decrease the width if line numbers are requested
            if (ShowLineNumbers)
                xMaxWidth -= Window.LineNumberAttrs.ColumnWidth;

            // Add any SpaceBefore. This comes to us in Points, so we divide by 72 to
            // get Inches, then multiply by the screen's DPI to get pixels.
            float ySpaceBefore = (((float)PStyle.SpaceBefore) * g.DpiY / 72.0F);
            ySpaceBefore *= G.ZoomFactor;
            y += ySpaceBefore;
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);
            y += CalculateBitmapHeightRequirement();

            // We'll add to x until we get to xMaxWidth, to know how much can fit on a line.
            // For this first x, we need to allow for the paragraph's FirstLineIndent
            float x = (float)PStyle.FirstLineIndent * g.DpiX;

            // We'll build the lines here
            ClearLines();
            Line line = new Line();
            AddLine(line);

            // Loop through all the blocks, adding them into lines
            for (int i = 0; i < SubItems.Length; )
            {
                // If we have a chapter, we treat if separately, since it takes up two lines.
                EChapter chapter = SubItems[i] as EChapter;
                if (null != chapter)
                {
                    // If we have contents in the current line, then we need to start a new
                    // line, so the chapter occurs at the left margin.
                    if (line.Count > 0)
                    {
                        line = new Line();
                        AddLine(line);
                    }

                    line.Chapter = chapter;
                    x = chapter.Width;
                    line.LeftIndent = chapter.Width;
                    i++;
                    continue;
                }

                // If we have a Verse, we first determine if it needs extra leading space.
                // Refer to the DOC above.
                EVerse verse = (i > 0) ? SubItems[i] as EVerse : null;
                if (null != verse)
                {
                    verse.NeedsExtraLeadingSpace = true;
                    verse.CalculateWidth(g);
                }

                if (this.SubItems.Length == 1 && (this.SubItems[0] as EBlock != null
                    && (this.SubItems[0] as EBlock).Text == "JohnWimbish"))
                {
                    Console.WriteLine("Ouch");
                }

                // Measure the next "chunk" we want to add (this may be more than one EBlock
                // due to glue, but it will have at most only one DText). If the chunk is too
                // long, then we break it apart using hyphenation rules.
                float fAvailWidth = xMaxWidth - x;
                bool bMustForceHyphen = (line.SubItems.Length == 0);
                ProposeNextLayoutChunk chunk = new ProposeNextLayoutChunk(
                    g, this, fAvailWidth, i, bMustForceHyphen);

                Debug.Assert( !(chunk.TooLarge && line.SubItems.Length == 0),
                    "Word too long for the line, and wasn't hyphenated. Shouldn't happen.");

                // Will the Chunk fit on the line? If not, start a new line
                if (chunk.TooLarge)
                {
                    // If we're working on a chapter, then both this line and the next line will 
                    // need to reflect the indentation
                    float fIndentLine = (null == line.Chapter) ? 0 : line.Chapter.Width;

                    // Create the new line, and put this appropriate indentation to it
                    line = new Line();
                    AddLine(line);
                    line.LeftIndent = fIndentLine;

                    // Reset x so we can work through this line
                    x = fIndentLine;

                    // Since we're at the beginning of a line, extra leading space is not needed
                    // for a verse number. Refer to the DOC above.
                    if (null != verse)
                    {
                        verse.NeedsExtraLeadingSpace = false;
                        verse.CalculateWidth(g);
                    }

                    // Loop back, because now that we have a longer line, we can try hyphenation
                    // proposals that might have previously failed.
                    continue;
                }

                // Add the approved chunk(s) to the line
                for (int k = 0; k < chunk.ChunkSize; k++)
                {
                    EBlock block = SubItems[i] as EBlock;
                    line.Append(block);
                    x += block.Width;   // Can't use ChunkWidth because of verse width recalcs
                    i++;
                }
            }

            // Finally, we need to loop and actually assign Screen Coordinates to these objects,
            // now that we've broken them down into lines.
            float xLeft = Position.X + (float)PStyle.LeftMargin * g.DpiX;
            Layout_SetCoordinates(g, new PointF(xLeft, y), xMaxWidth);

            // Add any SpaceBefore and Space-After to the Height. 
            // Convert from Points (See SpaceBefore above.)
            Height += ySpaceBefore;
            Height += Border.GetTotalWidth(BorderBase.BorderSides.Top);
            Height += Border.GetTotalWidth(BorderBase.BorderSides.Bottom);
            Height += CalculateBitmapHeightRequirement();
            float ySpaceAfter = (((float)PStyle.SpaceAfter) * g.DpiY / 72.0F);
            ySpaceAfter *= G.ZoomFactor;
            Height += ySpaceAfter;
        }
        #endregion

        #region Method: void RePosition(float yNew) - change the Y coord of the paragraph and its parts
        public void RePosition(float yNew)
            // Moves all of the "y" coordinates in the paragraph (paragraph, EBlocks, Lines)
            // to the new yNew. This is called when the paragraph is being moved to reflect
            // editing somewhere on the screen.
        {
            // Get the difference between the current and the new Y values
            float yDiff = yNew - Position.Y;

            // Set the paragraph to its new value
            Position = new PointF(Position.X, yNew);

            // Set the individual blocks to their new values
            foreach (EBlock block in SubItems)
            {
                block.Position = new PointF(block.Position.X,
                    block.Position.Y + yDiff);
            }

            // Set the individual lines to their new values
            foreach (Line ln in Lines)
            {
                ln.Position = new PointF(ln.Position.X,
                    ln.Position.Y + yDiff);
            }
        }
        #endregion
        #region Method: void ReLayout() - recalculate the layout; decide if the rest of screen needs to be updated
        void ReLayout()
            // Call this when the paragraph's contents have changed (e.g., due to a
            // Delete or Insert; so that higher-level containers can be appropriately
            // shifted / redrawn in the window.
            //
            // Probably lots of optimizations will be due: for now, we're just routinely
            // calling DoLayout, and then seeing if the paragraph height has changed.
        {
            // Get the height before the layout
            float fHeightGoingIn = Height;

            // Get the line number of the first line (if any) before the layout
            int nLineNo = -1;
            if (Lines.Length > 0)
                nLineNo = Lines[0].LineNo;

            // Rework the paragraph: words on each line, justification, etc., etc.
            CalculateVerticals(Position.Y, false);

            // If the height did not change, then all we need to do is re-do the
            // line numbers (because we've created new Lines) and redraw the paragraph.
            if (Height == fHeightGoingIn)
            {
                CalculateLineNumbers(ref nLineNo);
                Window.Draw.Invalidate(Rectangle);
                return;
            }

            // Otherwise, we need to shift things up/down. So we call the Window and tell
            // it refigure everything starting with the owning Row
// TEMPORARY TESTING HILARIO SCREEN PROBLEM: Trying the full DoLayout/Invalidate to
// see if the culprit is the ReLayout thing.
            Window.DoLayout();
            Window.Invalidate();
            //Window.OnParagraphHeightChanged(TopContainer);
// END TEMPORARY
        }
        #endregion
        #region OMethod: void CalculateLineNumbers(ref nLineNo)
        protected override void CalculateLineNumbers(ref int nLineNo)
        {
            if (!IsEditable)
                return;

            foreach (Line line in Lines)
                line.LineNo = nLineNo++;
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region Attr{g}: Color EditableBackgroundColor - shows the user where typing is permitted
        public Color EditableBackgroundColor
        {
            get
            {
                return m_EditableBackgroundColor;
            }
        }
        Color m_EditableBackgroundColor = Color.White;
        #endregion
        #region Attr{g/s}: Color NonEditableBackgroundColor - shows where typing is not permitted
        public Color NonEditableBackgroundColor
        {
            get
            {
                if (m_NonEditableBackgroundColor == Color.Empty)
                    m_NonEditableBackgroundColor = Window.BackColor;
                return m_NonEditableBackgroundColor;
            }
            set
            {
                Debug.Assert(Color.Empty != value);
                m_NonEditableBackgroundColor = value;
            }
        }
        Color m_NonEditableBackgroundColor = Color.Empty;
        #endregion
        #region OMethod: void OnPaint(ClipRectangle)
        public override void OnPaint(Rectangle ClipRectangle)
        {
            // See if this paragraph needs to be painted
            if (!ClipRectangle.IntersectsWith(IntRectangle))
                return;

			// Bullet if indicated
			PaintBullet();

            // Borders if indicated
            Border.Paint();

            // Bitmap if indicated
            PaintBitmap();

            // Paint the contents
            foreach (EBlock block in SubItems)
                block.Paint();

            // Paint the line numbers, if turned on
            if (ShowLineNumbers)
            {
                foreach (Line line in Lines)
                    line.PaintLineNumber(Window, this);
            }
        }
        #endregion
		#region Method: void PaintBullet()
		void PaintBullet()
		{
			if (!PStyle.Bulleted)
				return;

			Graphics g = Window.Draw.Graphics;

			// The radius is 1/5 of the line height
			float fRadius = LineHeight / 5;

			// We'll place it to the left of our first line by three times the radius
			float xLeft = Position.X + (float)PStyle.LeftMargin * g.DpiX - fRadius * 3;

			// We'll place it vertically in the middle of the first line
			float yTop = Position.Y + LineHeight / 2;

			// Draw the bullet
			Window.Draw.DrawBullet( Color.Black, new PointF(xLeft, yTop), fRadius);
		}
		#endregion

		// Selection -------------------------------------------------------------------------
        #region Select_ and ExtendSelection_

        #region OMethod: EBlock GetBlockAt(PointF pt)
        public override EBlock GetBlockAt(PointF pt)
        {
            // Don't waste time if we're not in this paragraph
            if (!ContainsPoint(pt))
                return null;

            // For Efficiency, let the Lines handle it; this saves time for larger paragraphs
            // TODO: Refactoring Lines could help here.
            foreach (Line ln in Lines)
            {
                EBlock block = ln.GetBlockAt(pt);
                if (null != block)
                    return block;
            }

            return null;
        }
        #endregion

        #region Method: void Select_LineBegin()
        public void Select_LineBegin()
        {
            if (!IsEditable)
                return;

            foreach (Line ln in Lines)
            {
                if (ln.Contains(Window.Selection.Anchor.Word))
                {
                    foreach (EBlock block in ln.SubItems)
                    {
                        if (null != block as EWord)
                        {
                            Window.Selection = new OWWindow.Sel(this, 
                                new OWWindow.Sel.SelPoint(Find(block), 0));
                            return;
                        }
                    }
                }
            }
        }
        #endregion
        #region Method: void Select_LineEnd()
        public void Select_LineEnd()
        {
            if (!IsEditable)
                return;

            foreach (Line ln in Lines)
            {
                if (ln.Contains(Window.Selection.Anchor.Word))
                {
                    for(int iBlock = ln.Count - 1; iBlock >= 0; iBlock--)
                    {
                        EWord word = ln.SubItems[iBlock] as EWord;
                        if (null != word)
                        {
                            int iChar = word.Text.Length;
                            Window.Selection = new OWWindow.Sel(this,
                                new OWWindow.Sel.SelPoint(Find(word), iChar));
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region Method: void ExtendSelection_Line(bool bEnd)
        public void ExtendSelection_Line(bool bEnd)
            // bEnd - T if extending to the End of the line, 
            //        F if extending to the beginning
        {
            if (!IsEditable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;

            // Locate the Line this block is on
            Line line = LineContainingBlock(pt.Word);
            Debug.Assert(null != line);
            int iBlockInLine = line.Find(pt.Word);
            Debug.Assert(-1 != iBlockInLine);

            // Move to the furthest left/right block possible
            if (bEnd)
            {
                while (iBlockInLine < line.Count - 1 &&
                    line.SubItems[iBlockInLine + 1] as EWord != null)
                {
                    ++iBlockInLine;
                }
            }
            else
            {
                while (iBlockInLine > 0 &&
                    line.SubItems[iBlockInLine - 1] as EWord != null)
                {
                    --iBlockInLine;
                }
            }

            // Get the index of this block in the Paragraph's Blocks attr
            // (as opposed to the Line's Blocks attr
            int iBlock = Find( line.SubItems[iBlockInLine] );

            // The position within the text is the first/last possible position
            int iChar = (bEnd) ? (line.SubItems[iBlockInLine] as EBlock).Text.Length : 0;

            // Create the selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_LineUpDown(bool bDown, float x)
        public void ExtendSelection_LineUpDown(bool bDown, float x)
        {
            if (!IsEditable)
                return;

            // Determine which SelPoint to work from
            OWWindow.Sel.SelPoint sp = (Window.Selection.IsInsertionPoint) ?
                Window.Selection.Anchor :
                Window.Selection.End;

            // Retrieve the current line
            Line lineCurrent = LineContainingBlock(sp.Word);
            int iLineCurrent = IndexOfLine(lineCurrent);

            // Get the next line down; abort if no-can-do. This gives us coordinates to look for.
            int iLineTarget = (bDown) ? iLineCurrent + 1 : iLineCurrent - 1;
            if (iLineTarget == Lines.Length || iLineTarget < 0)
                return;
            Line lineTarget = Lines[iLineTarget];
            PointF pt = new PointF(x, lineTarget.Position.Y);

            // Loop through the blocks, looking for the pt, but don't go beyond the line in question
            EBlock blockEnd = (bDown) ?
                lineTarget.SubItems[lineTarget.Count - 1] as EBlock:
                lineTarget.SubItems[0] as EBlock;
            int i = Find(sp.Word);
            int iEnd = Find(blockEnd);
            do
            {
                // Iterate to the next block; if it isn't an EWord, then we can go no further.
                i += (bDown) ? 1 : -1;
                EWord word = SubItems[i] as EWord;
                if (null == word)
                    break;

                // If the word contains "x", then we are happily sucessfully done.
                if (word.ContainsPoint(pt))
                {
                    int iChar = word.GetCharacterIndex(pt);
                    Window.Selection = new OWWindow.Sel(this, 
                        Window.Selection.Anchor,
                        new OWWindow.Sel.SelPoint(i, iChar));
                    return;
                }

                // Otherwise, set the selection to the end of this word; the loop will then
                // see if the next one will work for us.
                Window.Selection = new OWWindow.Sel(this,
                    Window.Selection.Anchor,
                    new OWWindow.Sel.SelPoint(i, ((bDown) ? word.Text.Length : 0)));

            } while (i != iEnd);
        }
        #endregion

        // Extending Selections
        #region Method: Sel ExtendSelection_WordRight(Sel) - Creates a selection that includes the next word
        public OWWindow.Sel ExtendSelection_WordRight(OWWindow.Sel selection)
        {
            if (!IsEditable)
                return selection;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = selection.Anchor;
            if (!selection.IsInsertionPoint)
                pt = selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Move to the beginning of the next word; or to the end of this
            // one if there is no word on the right
            if (pt.Word.IsBesideEWord(true))
            {
                iBlock++;
                iChar = 0;
            }
            else
                iChar = pt.Word.Text.Length;

            // Create the new, updated selection
            OWWindow.Sel selectionNew = new OWWindow.Sel(this,
                selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
            return selectionNew;
        }
        #endregion
        #region Method: Sel ExtendSelection_WordLeft(Sel) - Creates a selection that includes the previous word
        public OWWindow.Sel ExtendSelection_WordLeft(OWWindow.Sel selection)
        {
            if (!IsEditable)
                return selection;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = selection.Anchor;
            if (!selection.IsInsertionPoint)
                pt = selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // If at the beginning of a word, then move to the previous word
            if (iChar == 0 && pt.Word.IsBesideEWord(false))
            {
                iBlock--;
            }
            iChar = 0;

            // Create the new, updated selection
            OWWindow.Sel selectionNew = new OWWindow.Sel(this, 
                selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
            return selectionNew;
        }
        #endregion
        #region Method: Sel ExtendSelection_FarRight(Sel)- Creates a selection that extends to the end of the DBT
        public OWWindow.Sel ExtendSelection_FarRight(OWWindow.Sel selection)
        {
            if (!IsEditable)
                return selection;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = selection.Anchor;
            if (!selection.IsInsertionPoint)
                pt = selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Go as far to the right as possible
            while (iBlock < SubItems.Length - 1 && SubItems[iBlock + 1] as EWord != null)
                ++iBlock;
            iChar = (SubItems[iBlock] as EBlock).Text.Length;

            // Create the new, updated selection
            return new OWWindow.Sel(this, 
                selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: Sel ExtendSelection_FarLeft(Sel) - Creates a selection that extends to the DBT's beginning
        public OWWindow.Sel ExtendSelection_FarLeft(OWWindow.Sel selection)
        {
            if (!IsEditable)
                return selection;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = selection.Anchor;
            if (!selection.IsInsertionPoint)
                pt = selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Go as far to the left as possible
            while (iBlock > 0 && SubItems[iBlock - 1] as EWord != null)
                --iBlock;
            iChar = 0;

            // Create the new, updated selection
            return new OWWindow.Sel(this, 
                selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: Sel ExtendSelection_CharRight(Sel) - Creates a selection that extends a char to the right
        public OWWindow.Sel ExtendSelection_CharRight(OWWindow.Sel selection)
        {
            if (!IsEditable)
                return selection;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = selection.Anchor;
            if (!selection.IsInsertionPoint)
                pt = selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Move one to the right if possible
            if (iChar < pt.Word.Text.Length)
                iChar++;

            // Move over into the next word if necessary (and if possible)
            if (iChar == pt.Word.Text.Length)
            {
                if (iBlock < SubItems.Length - 1 && null != SubItems[iBlock + 1] as EWord)
                {
                    iBlock++;
                    iChar = 0;
                }
            }

            // Create the new, updated selection
            return new OWWindow.Sel(this, 
                selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: Sel ExtendSelection_CharLeft(Sel) - Creates a selection that extends a char to the left
        public OWWindow.Sel ExtendSelection_CharLeft(OWWindow.Sel selection)
        {
            if (!IsEditable)
                return selection;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = selection.Anchor;
            if (!selection.IsInsertionPoint)
                pt = selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // If we are at a word boundary, then move to the end of the preceeding word
            if (iChar == 0)
            {
                if (iBlock > 0 && null != SubItems[iBlock - 1] as EWord)
                {
                    iBlock--;
                    iChar = (SubItems[iBlock] as EBlock).Text.Length;
                }
            }

            // Move one to the left if possible
            if (iChar > 0)
                iChar--;

            // Create the new, updated selection
            return new OWWindow.Sel(this, 
                selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion

        // Misc selection support
        #region Method: Sel NormalizeSelection(sel) - Moves from end of word to beginning of next word
        public OWWindow.Sel NormalizeSelection(OWWindow.Sel selection)
        {
            // Do we have something normalizable?
            if (null == selection || selection.IsInsertionIcon)
                return selection;

            // If we are on an Insertion Point at the end of the word,
            // then move to the beginning of the next word if possible
            if (selection.IsInsertionPoint)
            {
                if (selection.Anchor.iChar == selection.Anchor.Word.Text.Length &&
                    selection.Anchor.Word.IsBesideEWord(true))
                {
                    Root.Select_NextWord_Begin(selection);
                    return Window.Selection;
                }

                return selection;
            }

            // If a Selection, then move the End accordingly
            if (null != selection.End &&
                selection.End.iChar == selection.End.Word.Text.Length)
            {
                return ExtendSelection_WordRight(selection);
            }

            return selection;
        }
        #endregion
        #endregion
        #region OMethod: bool MoveLineDown(aiStack, ptCurrentLocation)
        public override bool MoveLineDown(ArrayList aiStack, PointF ptCurrentLocation)
        {
            // Loop through the subitems
            for (int i = PopSelectionStack(aiStack, true); i < Count; i++)
            {
                EItem item = SubItems[i] as EItem;
                EWord word = item as EWord;

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If not a word, skip it
                if (null == word)
                    continue;

                // If the word is not beyond the "y" value, then skip it
                if (word.Position.Y <= ptCurrentLocation.Y)
                    continue;

                // Get the line this word is in
                Line line = LineContainingBlock(word);
                if( line.MakeSelectionClosestTo(new PointF(ptCurrentLocation.X, word.Position.Y)))
                    return true;
            }

            return false;
        }
        #endregion
        #region OMethod: bool MoveLineUp(aiStack, ptCurrentLocation)
        public override bool MoveLineUp(ArrayList aiStack, PointF ptCurrentLocation)
        {
            // Loop through the subitems
            for (int i = PopSelectionStack(aiStack, false); i >= 0; i--)
            {
                EItem item = SubItems[i] as EItem;
                EWord word = item as EWord;

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If not a word, skip it
                if (null == word)
                    continue;

                // If the word is not below the "y" value, then skip it
                if (word.Position.Y >= ptCurrentLocation.Y)
                    continue;

                // Get the line this word is in
                Line line = LineContainingBlock(word);
                if (line.MakeSelectionClosestTo(new PointF(ptCurrentLocation.X, word.Position.Y)))
                    return true;
            }

            return false;
        }
        #endregion

        // Edit Operations -------------------------------------------------------------------
        public enum DeleteMode { kDelete, kCut, kCopy, kBackSpace };
        #region Method: bool JoinParagraph(DeleteMode mode)
        public bool JoinParagraph(DeleteMode mode)
        {
            // Do nothing if the paragraph is uneditable
            if (!IsEditable)
                return false;

            // Must be an insertion point
            if (Window.Selection.IsInsertionPoint)
            {
                if (CanRestructureParagraphs)
                {
                    // If a delete at the end of a paragraph, then join with the next paragraph
                    if (Window.Selection.IsInsertionPoint_AtParagraphEnding && 
                        mode == DeleteMode.kDelete)
                    {
                        Window.cmdMoveCharRight();
                        mode = DeleteMode.kBackSpace;
                    }

                    if (Window.Selection.IsInsertionPoint_AtParagraphBeginning && 
                        mode == DeleteMode.kBackSpace)
                    {
                        if ((new JoinParagraphAction(this.Window)).Do())
                            return true;
                    }
                }
            }

            // Did not join paragraphs
            return false;
        }
        #endregion

        #region Method: void ReplaceBlocksWithNewDBT(OWWindow.Sel selection, DBasicText DBT)
        public void ReplaceBlocksWithNewDBT(OWWindow.Sel selection, DBasicText DBT)
        {
            int iBlockFirst = selection.DBT_iBlockFirst;
            RemoveAt(selection.DBT_iBlockFirst, selection.DBT_BlockCount);
            EWord[] vWords = ParseBasicTextIntoWords(DBT, null);
            InsertAt(iBlockFirst, vWords);
            foreach (EWord w in vWords)
                w.CalculateWidth(Window.Draw.Graphics);
            ReLayout();
        }
        #endregion
    }

}
