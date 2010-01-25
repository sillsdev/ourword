#region ***** OWPara.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    OWPara.cs
 * Author:  John Wimbish
 * Created: 15 Mar 2007
 * Purpose: An individual paragraph
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

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
            CanItalic = 64                   // Italics are permitted in thie paragraph
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

                return Style.Map.IsScripture;
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
        #region Attr{g}: JObject DataSource - the DParagraph behind this OWPara
        public JObject DataSource
        {
            get
            {
                // Because I allow literals, sometimes paragraphs do not have data sources.
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
            #region Constructor(font, DChapter)
            public EChapter(Font font, DChapter chapter)
                : base(font, chapter.Text)
            {
                // Add a little space to the end so that it appears a bit nicer in the 
                // display. It is uneditable, so this only affects the display.
                m_sText = Text + "\u00A0";

                TextColor = StyleSheet.ChapterNumber.FontColor;
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

            // Drawing -----------------------------------------------------------------------
            #region Method: override void Draw(IDraw)
            public override void Draw(IDraw draw)
            {
                // Position "x" at the left margin
                var x = Position.X; 

                // Calculate "y" to be centered horizontally
                var y = Position.Y + (Height / 2) - (m_Font.Height / 2F);

                // Draw the string
                draw.DrawString(Text, m_Font, GetBrush(), new PointF(x, y));
            }
            #endregion
            #region Attr{g}: int Number
            public int Number
            {
                get
                {
                    try
                    {
                        var sNumber = "";
                        foreach(var ch in Text)
                        {
                            if (char.IsDigit(ch))
                                sNumber += ch;
                            else
                                break;
                        }
                        return Convert.ToInt16(sNumber);
                    }
                    catch (Exception)
                    {
                    }
                    return 1;
                }
            }
            #endregion
        }
        #endregion
        #region CLASS: EVerse
        public class EVerse : EBlock
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

            #region Constructor(font, DVerse)
            public EVerse(Font font, DVerse verse)
                : base(font, verse.Text)
            {
                TextColor = StyleSheet.VerseNumber.FontColor;
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

            #region Method: override void Draw(IDraw)
            public override void Draw(IDraw draw)
                // The verse size in the stylesheet reflects a normal style; we need to
                // decrease it for the superscript.
            {
                // If verse numbers are turned off, we potentially need to paint a white 
                // background.
                if (Para.SuppressVerseNumbers)
                {
                    if (Para.IsEditable && !Para.IsLocked)
                    {
                        var r = new RectangleF(Position, new SizeF(Width, Height));
                        draw.DrawBackground(Para.EditableBackgroundColor, r);
                    }
                    return;
                }
               
                var s = Text;
                if (NeedsExtraLeadingSpace)
                    s = c_sLeadingSpace + Text;
                draw.DrawString(s, m_Font, GetBrush(), Position);
            }
            #endregion

            // Layout Calculations ---------------------------------------------------------------
            #region OMethod: override void CalculateWidth()
            public override void CalculateWidth()
            {
                // The text we will measure
                var s = Text;

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
                var fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                Width = Context.Graphics.MeasureString(s, m_Font, 
                    1000, fmt).Width;
            }
            #endregion

            #region Attr{g}: int Number
            public int Number
            {
                get
                {
                    try
                    {
                        var sNumber = "";
                        foreach (var ch in Text)
                        {
                            if (char.IsDigit(ch))
                                sNumber += ch;
                            else
                                break;
                        }
                        return Convert.ToInt16(sNumber);
                    }
                    catch (Exception)
                    {
                    }
                    return 1;
                }
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

            #region Constructor(font, DFoot)
            public EFoot(Font font, DFoot foot)
                : base(font, foot.Text)
            {
                m_Foot = foot;
                TextColor = StyleSheet.FootnoteLetter.FontColor;
            }
            #endregion

            #region Method: override void Draw(IDraw)
            public override void Draw(IDraw draw)
            {
                draw.DrawString(Text, m_Font, GetBrush(), Position);
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

            #region Constructor(font, DLabel)
            public ELabel(Font font, DLabel label)
                : base(font, label.Text + c_Spaces)
            {
            }
            #endregion

            #region Method: override void Draw(IDraw)
            public override void Draw(IDraw draw)
            {
                draw.DrawString(Text, m_Font, GetBrush(), Position);
            }
            #endregion
        }
        #endregion
        #region CLASS: ELiteral
        class ELiteral : EWord 
            // As a literal of EWord, hyphenation is possible.
        {
            #region Constructor(font, DPhrase, sText)
            public ELiteral(Font font, DPhrase phrase, string sText)
                : base(font, phrase, sText)
            {
            }
            #endregion
            #region OMethod: EWord Clone()
            public override EWord Clone()
            {
               return new ELiteral(m_Font, Phrase, Text);
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
            #region OMethod: void Draw(IDraw)
            public override void Draw(IDraw draw)
            {
                draw.DrawString(Text, m_Font, GetBrush(), Position);
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
        #region CLASS: EFootnoteLabel
        public class EFootnoteLabel : EBlock
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
            readonly DFootnote m_Footnote = null;
            #endregion

            #region Constructor(font, DFootLetter)
            public EFootnoteLabel(Font font, DFootnote footnote)
                : base(font, footnote.Letter + " ")
            {             
                m_Footnote = footnote;
                TextColor = StyleSheet.FootnoteLetter.FontColor;
            }
            #endregion

            #region Method: override void Draw(IDraw)
            public override void Draw(IDraw draw)
            {
                draw.DrawString(Text, m_Font, GetBrush(), Position);
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
            #region Constructor(font, sText)
            public EBigHeader(Font font, string sText)
                : base(font, sText + " ")
            {
                TextColor = StyleSheet.BigHeader.FontColor;

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

            #region Method: override void Draw(IDraw)
            public override void Draw(IDraw draw)
            {
                draw.DrawString(Text, m_Font, GetBrush(), Position);
            }
            #endregion
        }
        #endregion

        // Initialize to a paragraph's contents ----------------------------------------------
        #region Method: List<EWord> ParseBasicTextIntoWords(DBasicText t)
        List<EWord> ParseBasicTextIntoWords(DBasicText t)
        {
            // Select between the Vernacular vs the Back Translation text
            var phrases = (DisplayBT) ? t.PhrasesBT : t.Phrases;
            Debug.Assert(phrases.Count > 0);

            // We'll collect the EWords here
            var vWords = new List<EWord>();

            // Get the font factory for everything in this DBT
            var style = t.Paragraph.Style;
            var fontFactory = style.FindFontFactory(WritingSystem.Name);

            // Loop through all of the phrases in this DBasicText
            foreach (DPhrase phrase in phrases)
            {
                // We'll collect individual words here
                var sWord = "";

                // Process through the phrase's text string
                for (var i = 0; i < phrase.Text.Length; i++)
                {
                    // If we are sitting at a word break, then add the word and reset
                    // in order to build the next one.
                    if (WritingSystem.IsWordBreak(phrase.Text, i) && sWord.Length > 0)
                    {
                        var fontZoomed = fontFactory.GetFont(phrase.FontToggles, G.ZoomPercent);
                        vWords.Add(new EWord(fontZoomed, phrase, sWord));
                        sWord = "";
                    }

                    // Add the character to the word we are building
                    sWord += phrase.Text[i];
                }

                // Pick up the final word in the string, IsWordBreak will not have 
                // caught it.
                if (sWord.Length > 0)
                {
                    var fontZoomed = fontFactory.GetFont(phrase.FontToggles, G.ZoomPercent);
                    vWords.Add(new EWord(fontZoomed, phrase, sWord));
                }
            }

            // If we did not find any words, then we want to create an InsertionIcon
            if (vWords.Count == 0)
            {
                var fontZoomed = fontFactory.GetFont(G.ZoomPercent);
                vWords.Add(EWord.CreateAsInsertionIcon(fontZoomed, phrases[0]));
            }

            return vWords;
        }
        #endregion
        #region Method: void _InitializeBasicTextWords(DBasicText)
        void _InitializeBasicTextWords(DBasicText t)
        {
            var vWords = ParseBasicTextIntoWords(t);
            Append(vWords.ToArray());
        }
        #endregion
        #region Method: void InitializeNoteIcons(DText)
        void InitializeNoteIcons(DText text, bool bShowingBT)
        {
            if (!OurWordMain.s_Features.TranslatorNotes)
                return;

            foreach (TranslatorNote note in text.TranslatorNotes)
            {
                OWWindow wnd = OurWordMain.App.CurrentLayout;
                if (null != wnd)
                {
                    var context = wnd.GetNoteContext(note, Options);
                    if (ENote.Flags.None != context)
                        Append(new ENote(note, context));
                }
            }
        }
        #endregion
        #region Method: void _InitializeGlueToNext(int iLeft)
        void _InitializeGlueToNext(int iLeft)
        {
            // We are interested both in the Block passed in (iLeft) and the word immediately
            // to its right.
            var iRight = iLeft + 1;

            // Make sure we've passed in words that exist in the list
            Debug.Assert(iLeft >= 0);
            Debug.Assert(iRight < SubItems.Length);

            // Retrieve their EBlock objects
            var blockLeft = SubItems[iLeft] as EBlock;
            var blockRight = SubItems[iRight] as EBlock;
            Debug.Assert(null != blockLeft && null != blockRight);

            // The default is not to glue
            blockLeft.GlueToNext = false;

            // Glue to following footnotes letters
            if (blockRight as EFoot != null)
                blockLeft.GlueToNext = true;

            // Glue to following TranslatorNotes (but not subsequent ones, as the Manado
            // data exhibits so many that by themselve they can take up an entire line.
            if (blockLeft as ENote == null && blockRight as ENote != null)
                blockLeft.GlueToNext = true;

            // For the footnote paragraphs themselves, we want the preceeding labels
            // (which house the Scripture reference) to be glued, so that any justification
            // will not give them a ragged-looking alignment.
            if (blockLeft as EFootnoteLabel != null)
                blockLeft.GlueToNext = true;
        }
        #endregion
        #region Method: void _Initialize(DParagraph)
        void _Initialize(DParagraph p)
        {
            // Clear out any previous list contents
            Clear();

            // Get the fonts
            var fontChapter = StyleSheet.ChapterNumber.GetFont(WritingSystem.Name, G.ZoomPercent);
            var fontVerse = StyleSheet.VerseNumber.GetFont(WritingSystem.Name, G.ZoomPercent);
            var fontFootLetter = StyleSheet.FootnoteLetter.GetFont(WritingSystem.Name, G.ZoomPercent);
            var fontLabel = StyleSheet.Label.GetFont(WritingSystem.Name, G.ZoomPercent);              

            // Loop through the paragraph's runs
            foreach (DRun r in p.Runs)
            {
                switch (r.GetType().Name)
                {
                    case "DChapter":
                        Append(new EChapter(fontChapter, r as DChapter));
                        break;
                    case "DVerse":
                        Append(new EVerse(fontVerse, r as DVerse));
                        break;
                    case "DFoot":
                        Append(new EFoot(fontFootLetter, r as DFoot));
                        break;
                    case "DLabel":
                        Append(new ELabel(fontLabel, r as DLabel));
                        break;
                    case "DBasicText":
                        _InitializeBasicTextWords(r as DBasicText);
                        break;
                    case "DText":
                        _InitializeBasicTextWords(r as DBasicText);
                        InitializeNoteIcons(r as DText, DisplayBT);
                        break;
                    default:
                        Console.WriteLine("Unknown type in OWPara.Initialize...Name=" + 
                            r.GetType().Name);
                        Debug.Assert(false);
                        break;
                }
            }

            // Loop through to set the GlueToNext values
            for (var i = 0; i < SubItems.Length - 1; i++)
                _InitializeGlueToNext(i);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Attr{g}: WritingSystem WritingSystem
        public WritingSystem WritingSystem
        {
            get
            {
                Debug.Assert(null != m_WritingSystem);
                return m_WritingSystem;
            }
        }
        readonly WritingSystem m_WritingSystem;
        #endregion
        #region Attr{g}: ParagraphStyle Style
        public ParagraphStyle Style
        {
            get
            {
                Debug.Assert(null != m_style);
                return m_style;
            }
        }
        readonly ParagraphStyle m_style;
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
        readonly float m_fLineHeight;
        #endregion      

        #region private Constructor(WS, style, JObject, flags) - the stuff that is in common
        private OWPara(
            WritingSystem writingSystem,
            ParagraphStyle style,
            JObject objDataSource,
            Flags options)
            : base()
        {
            // Keep track of the attributes passed in
            m_WritingSystem = writingSystem;
            m_style = style;
            m_objDataSource = objDataSource;
            m_Options = options;

            // Calculate the line height for the paragraph
            m_fLineHeight = style.GetFont(writingSystem.Name, G.ZoomPercent).Height;

            // Initialize the vector of Blocks
            Clear();

            // Initialize the list of Lines
            m_vLines = new List<ELine>();
        }
        #endregion
        #region Constructor(WS, style, DParagraph, clrEditableBackground, Flags) - for DParagraph
        public OWPara(
            WritingSystem ws, 
            ParagraphStyle style,
            DParagraph p,
            Color clrEditableBackground, 
            Flags options)
            : this(ws, style, p, options)
        {
            // The paragraph itself may override to make itself uneditable, 
            // even though we received "true" from the _bEditable parameter.
            if (!p.IsUserEditable)
                IsEditable = false;

            // Interpret the paragraph's contents into our internal data structure
            _Initialize(p);

            // Retrieve the background color for editable parts of the paragraph
            m_EditableBackgroundColor = clrEditableBackground;
        }
        #endregion
        #region Constructor(JWS, style, DRun[], sLabel, Flags)
        public OWPara(
            JWritingSystem writingSystem,
            ParagraphStyle style,
            DRun[] vRuns, 
            string sLabel, 
            Flags options)
            : this(writingSystem, style, vRuns[0].Owner, options)
            // For the Related Languages window
        {
            // We'll add the language name as a BigHeader
            if (!string.IsNullOrEmpty(sLabel))
            {
                var fontBigHeader = StyleSheet.BigHeader.GetFont(writingSystem.Name, G.ZoomPercent);
                Append(new EBigHeader(fontBigHeader, sLabel));
            }

            // Add the text (we only care about verses and text)
            foreach (DRun run in vRuns)
            {
                switch (run.GetType().Name)
                {
                    case "DVerse":
                        {
                            var fontVerse = StyleSheet.VerseNumber.GetFont(writingSystem.Name, G.ZoomPercent);
                            Append(new EVerse(fontVerse, run as DVerse));
                        }
                        break;
                    case "DText":
                    case "DBasicText":
                        _InitializeBasicTextWords(run as DBasicText);
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
                var word = SubItems[i] as EWord;
                if (null == word || !word.IsBesideEWord(true))
                    continue;
                if (!word.EndsWithWhiteSpace)
                    word.Text += ' ';
            }
        }
        #endregion
        #region Constructor(JWS, style, sLiteralString)
        public OWPara(
            JWritingSystem _ws, 
            ParagraphStyle style, 
            string sLiteralText)
            : this(_ws, style, new DPhrase[] { new DPhrase(sLiteralText) })
        {
        }
        #endregion
        #region Constructor(JWS, style, DPhrase[] vLiteralPhrases)
        public OWPara(
            JWritingSystem writingSystem, 
            ParagraphStyle style, 
            DPhrase[] vLiteralPhrases)
            : this(writingSystem, style, (JObject)null, Flags.None)
        {

            // Each Literal String will potentially have its own character style
            foreach (var phrase in vLiteralPhrases)
            {
                // The Split method we're about to call will remove spaces, including
                // a trailing one. We need to know if we had a trailing one, so we
                // can add it back in.
                var bEndsWithSpace = false;
                if (phrase.Text.Length > 0 && phrase.Text[phrase.Text.Length - 1] == ' ')
                    bEndsWithSpace = true;

                // Parse the Literal Test into its parts
                var vs = phrase.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Determine the font for this phrase
                var font = style.GetFont(writingSystem.Name, phrase.FontToggles, G.ZoomPercent);

                // Create the literal strings
                for (var i = 0; i < vs.Length; i++)
                {
                    if (i < vs.Length - 1 || bEndsWithSpace)
                        vs[i] = vs[i] + " ";

                    // Add the literal
                    Append(new ELiteral(font, phrase, vs[i]));
                }
            }
        }
        #endregion

        // Layout Dependant ------------------------------------------------------------------
        #region Attr{g}: List<ELine> Lines - the list of Lines in the currently calculated layout
        public List<ELine> Lines
        {
            get
            {
                Debug.Assert(null != m_vLines);
                return m_vLines;
            }
        }
        readonly List<ELine> m_vLines = null;
        #endregion
        #region Method: Line LineContainingBlock(EBlock block)
        ELine LineContainingBlock(EBlock block)
        {
            foreach (var ln in Lines)
            {
                if (ln.Contains(block))
                    return ln;
            }
            return null;
        }
        #endregion
        #region Method: int IndexOfLine(EDisplayLine ln)
        int IndexOfLine(ELine ln)
        {
            for (int i = 0; i < Lines.Count; i++)
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
                var word = HyphenedWord;
                var writingSystem = Para.WritingSystem;

                for (iHyphenPos = word.Text.Length - 1; iHyphenPos > 0; iHyphenPos--)
                {
                    // This is triggered if a hyphen is required, because there
                    // is nothing on the line yet, and we've already tried
                    // the normal way of asking the writing system. I.e.,
                    // last resort.
                    if (ForcedHyphen)
                        return true;

                    // This is the preferred way of doing a hyphen: asking the
                    // writing system if we can.
                    if (writingSystem.IsHyphenBreak(OriginalTextToHyphen, iHyphenPos))
                        return true;
                }

                return false;
            }
            #endregion
            #region Method: void CreateHyphenedWordPair()
            void CreateHyphenedWordPair()
            // Creates the hyphened word / overflow word pair, if they do not already exist
            {
                var word = HyphenedWord;

                // Already done
                if (word.Hyphenated)
                    return;

                // Create a new, empty word
                var wordNew = word.Clone();
                wordNew.Text = "";

                // Insert it after our word
                var iPos = Para.Find(word);
                Para.InsertAt(iPos + 1, wordNew);

                // We now have a hyphenated word
                word.Hyphenated = true;

                // The next word now has the glue setting; the current word is no longer glued.
                wordNew.GlueToNext = word.GlueToNext;
                word.GlueToNext = false;
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
                wordHyphen.CalculateWidth();
                wordOverflow.CalculateWidth();
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
                word.CalculateWidth();

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
                word.CalculateWidth();
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

            foreach (var ln in Lines)
            {
                // Whether or not the line is justified depends first on the
                // paragraph style; but the final line is not justified in
                // any case.
                var bJustify = Style.IsJustified;
                if (ln == Lines[Lines.Count - 1])
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
                    x += Context.InchesToDeviceX((float)Style.FirstLineIndentInches);
                if (Style.IsRight)
                    x += (xMaxWidth - ln.Width);
                if (Style.IsCentered)
                    x += (xMaxWidth - ln.Width) / 2;

                // Calculate the width to be filled. If we are working on the first line, we need
                // to adjust that width. Thus, e.g., for a negative line indent, we want to increase
                // the fill-width. Hence we subtract.
                var xWidthToFill = xMaxWidth;
                if (ln == Lines[0])
                    xWidthToFill -= Context.InchesToDeviceX((float)Style.FirstLineIndentInches);

                ln.SetPositions(x, y, xWidthToFill, bJustify);
                y += LineHeight;
            }

            // The paragraph's "raw" or content height is what we wind up with after
            // the final line. THis represents the height of the lines, but does not include
            // the space before and after.
            Height = y - ptPos.Y;
        }
        #endregion

        #region OMethod: void CalculateVerticals(y)
        public override void CalculateVerticals(float y)
        // y - The top coordinate of the paragraph. We'll use "y" to work through the 
        //     height of the paragraph, setting the individual paragraph parts.
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
            // Shorthand
            var g = Context.Graphics;

            // Combine all hyphenated words, we'll figure out shortly if we must re-hyphenate
            RemoveHyphenation();

            // Remember the top coordinate
            Position = new PointF(Position.X, y);

            // We'll adjust the "internal" width based on the paragraph style
            var xMaxWidth = Width;

            // Decrease the width by the paragraph margins
            xMaxWidth -= Context.InchesToDeviceX((float)Style.LeftMarginInches);
            xMaxWidth -= Context.InchesToDeviceX((float)Style.RightMarginInches);

            // Decrease the width if line numbers are requested
            if (ShowLineNumbers)
                xMaxWidth -= Window.LineNumberAttrs.ColumnWidth;

            // Add any SpaceBefore. This comes to us in Points, so we divide by 72 to
            // get Inches, then multiply by the screen's DPI to get pixels.
            var ySpaceBefore = Context.PointsToDeviceY(Style.PointsBefore);
            ySpaceBefore *= G.ZoomFactor;
            y += ySpaceBefore;
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);
            y += CalculateBitmapHeightRequirement();

            // We'll add to x until we get to xMaxWidth, to know how much can fit on a line.
            // For this first x, we need to allow for the paragraph's FirstLineIndent
            var x = Context.InchesToDeviceX((float)Style.FirstLineIndentInches);

            // We'll build the lines here
            Lines.Clear();
            var line = new ELine();
            Lines.Add(line);

            // Loop through all the blocks, adding them into lines
            for (var i = 0; i < SubItems.Length; )
            {
                // If we have a chapter, we treat if separately, since it takes up two lines.
                var chapter = SubItems[i] as EChapter;
                if (null != chapter)
                {
                    // If we have contents in the current line, then we need to start a new
                    // line, so the chapter occurs at the left margin.
                    if (line.Count > 0)
                    {
                        line = new ELine();
                        Lines.Add(line);
                    }

                    line.Chapter = chapter;
                    x = chapter.Width;
                    line.LeftIndent = chapter.Width;
                    i++;
                    continue;
                }

                // If we have a Verse, we first determine if it needs extra leading space.
                // Refer to the DOC above.
                var verse = (i > 0) ? SubItems[i] as EVerse : null;
                if (null != verse)
                {
                    verse.NeedsExtraLeadingSpace = true;
                    verse.CalculateWidth();
                }

                if (this.SubItems.Length == 1 && (this.SubItems[0] as EBlock != null
                    && (this.SubItems[0] as EBlock).Text == "JohnWimbish"))
                {
                    Console.WriteLine("Ouch");
                }

                // Measure the next "chunk" we want to add (this may be more than one EBlock
                // due to glue, but it will have at most only one DText). If the chunk is too
                // long, then we break it apart using hyphenation rules.
                var fAvailWidth = xMaxWidth - x;
                var bMustForceHyphen = (line.SubItems.Length == 0);
                var chunk = new ProposeNextLayoutChunk(
                    g, this, fAvailWidth, i, bMustForceHyphen);

                Debug.Assert( !(chunk.TooLarge && line.SubItems.Length == 0),
                    "Word too long for the line, and wasn't hyphenated. Shouldn't happen.");

                // Will the Chunk fit on the line? If not, start a new line
                if (chunk.TooLarge)
                {
                    // If we're working on a chapter, then both this line and the next line will 
                    // need to reflect the indentation
                    var fIndentLine = (null == line.Chapter) ? 0 : line.Chapter.Width;

                    // Create the new line, and put this appropriate indentation to it
                    line = new ELine();
                    Lines.Add(line);
                    line.LeftIndent = fIndentLine;

                    // Reset x so we can work through this line
                    x = fIndentLine;

                    // Since we're at the beginning of a line, extra leading space is not needed
                    // for a verse number. Refer to the DOC above.
                    if (null != verse)
                    {
                        verse.NeedsExtraLeadingSpace = false;
                        verse.CalculateWidth();
                    }

                    // Loop back, because now that we have a longer line, we can try hyphenation
                    // proposals that might have previously failed.
                    continue;
                }

                // Add the approved chunk(s) to the line
                for (var k = 0; k < chunk.ChunkSize; k++)
                {
                    var block = SubItems[i] as EBlock;
                    line.Append(block);
                    x += block.Width;   // Can't use ChunkWidth because of verse width recalcs
                    i++;
                }
            }

            // Finally, we need to loop and actually assign Screen Coordinates to these objects,
            // now that we've broken them down into lines.
            var xLeft = Position.X + Context.InchesToDeviceX((float)Style.LeftMarginInches);
            Layout_SetCoordinates(g, new PointF(xLeft, y), xMaxWidth);

            // Add any PointsBefore and PointsAfter to the Height. 
            // Convert from Points (See comment on PointsBefore above.)
            Height += ySpaceBefore;
            Height += Border.GetTotalWidth(BorderBase.BorderSides.Top);
            Height += Border.GetTotalWidth(BorderBase.BorderSides.Bottom);
            Height += CalculateBitmapHeightRequirement();
            var ySpaceAfter = Context.PointsToDeviceY(Style.PointsAfter);
            ySpaceAfter *= G.ZoomFactor;
            Height += ySpaceAfter;
        }
        #endregion

        #region Method: void ReLayout(IDrawingContext) - recalculate the layout; decide if the rest of screen needs to be updated
        void ReLayout(IDrawingContext context)
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
            if (Lines.Count > 0)
                nLineNo = Lines[0].LineNo;

            // Rework the paragraph: words on each line, justification, etc., etc.
            CalculateVerticals(Position.Y);

            // If the height did not change, then all we need to do is re-do the
            // line numbers (because we've created new Lines) and redraw the paragraph.
            if (Height == fHeightGoingIn)
            {
                CalculateLineNumbers(ref nLineNo);
                Window.Invalidate(Rectangle);
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

            foreach (var line in Lines)
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
        #region OMethod: void OnPaint(IDraw, ClipRectangle)
        public override void OnPaint(IDraw draw, Rectangle clipRectangle)
        {
            // See if this paragraph needs to be painted
            if (!clipRectangle.IntersectsWith(IntRectangle))
                return;

			// Bullet if indicated
			PaintBullet(draw);

            // Borders if indicated
            Border.Paint(draw);

            // Bitmap if indicated
            PaintBitmap(draw);

            // Paint the contents
            foreach (EBlock block in SubItems)
                block.Draw(draw);

            // Paint the line numbers, if turned on
            if (!ShowLineNumbers) 
                return;
            foreach (var line in Lines)
                line.PaintLineNumber(Window, this);
        }
        #endregion
		#region Method: void PaintBullet(IDraw)
		void PaintBullet(IDraw draw)
		{
			if (!Style.Bulleted)
				return;

			// The radius is 1/5 of the line height
			var fRadius = LineHeight / 5;

			// We'll place it to the left of our first line by three times the radius
		    var xLeft = SubItems[0].Position.X - fRadius * 3;
// OLD: Verify a bullet displays correctly, then we can remove this. The line above
// Should do the trick without the need for the DpiX value.
//			var xLeft = Position.X + (float)PStyle.LeftMargin * g.DpiX - fRadius * 3;

			// We'll place it vertically in the middle of the first line
			var yTop = Position.Y + LineHeight / 2;

			// Draw the bullet
			draw.DrawBullet( Color.Black, new PointF(xLeft, yTop), fRadius);
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
            foreach (var ln in Lines)
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

            foreach (var ln in Lines)
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

            foreach (var ln in Lines)
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
            var line = LineContainingBlock(pt.Word);
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
            var lineCurrent = LineContainingBlock(sp.Word);
            int iLineCurrent = IndexOfLine(lineCurrent);

            // Get the next line down; abort if no-can-do. This gives us coordinates to look for.
            int iLineTarget = (bDown) ? iLineCurrent + 1 : iLineCurrent - 1;
            if (iLineTarget == Lines.Count || iLineTarget < 0)
                return;
            var lineTarget = Lines[iLineTarget];
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
                var line = LineContainingBlock(word);
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
                var line = LineContainingBlock(word);
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
            var vWords = ParseBasicTextIntoWords(DBT);
            InsertAt(iBlockFirst, vWords.ToArray());
            foreach (var w in vWords)
                w.CalculateWidth();

            var context = new WindowContext(Window);
            ReLayout(context);
        }
        #endregion
    }



    public class OWFootnotePara : OWPara
    {
        // Construction ----------------------------------------------------------------------
        #region Constructor(DFootnote, colorBackground, options)
        public OWFootnotePara(DFootnote footnote, Color clrEditableBackground, Flags options)
            : base(GetWritingSystem(footnote, options),            
                StyleSheet.Footnote, footnote, clrEditableBackground, options)
        {
            ConstructFootnoteReference(footnote, options);
            ConstructFootnoteLetter(footnote);
        }
        #endregion

        #region Method: void ConstructFootnoteReference(DFootnote, Flags)
        void ConstructFootnoteReference(DFootnote footnote, Flags options)
        {
            if (string.IsNullOrEmpty(footnote.VerseReference))
                return;

            var writngSystem = GetWritingSystem(footnote, options);
            var font = StyleSheet.Footnote.GetFont(writngSystem.Name, G.ZoomPercent);

            var label = new DLabel(footnote.VerseReference + ": ");

            InsertAt(0, new ELabel(font, label));
        }
        #endregion

        #region Method: void ConstructFootnoteLetter(DFootnote)
        void ConstructFootnoteLetter(DFootnote footnote)
        {
            var fontFootnoteLabel = StyleSheet.FootnoteLetter.GetFont(WritingSystem.Name, G.ZoomPercent);
            var label = new EFootnoteLabel(fontFootnoteLabel, footnote);
            InsertAt(0, label);
        }
        #endregion

        #region SMethod: JWritingSystem GetWritingSystem(DParagraph p, Flags options)
        static JWritingSystem GetWritingSystem(DParagraph p, Flags options)
        {
            return ((options & Flags.ShowBackTranslation) == Flags.ShowBackTranslation) ?
                p.Translation.WritingSystemVernacular :
                p.Translation.WritingSystemConsultant;
        }
        #endregion
    }

}
