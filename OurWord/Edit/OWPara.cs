/**********************************************************************************************
 * Project: OurWord!
 * File:    OWPara.cs
 * Author:  John Wimbish
 * Created: 15 Mar 2007
 * Purpose: An individual paragraph
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
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using NUnit.Framework;
#endregion

namespace OurWord.Edit
{
    public class OWPara
    {
        // Ownership Hierarchy ---------------------------------------------------------------
        #region Attr{g}: OWWindow.Row.Pile Pile - the owning pile that this paragraph is in
        public OWWindow.Row.Pile Pile
        {
            get
            {
                Debug.Assert(null != m_Pile);
                return m_Pile;
            }
        }
        OWWindow.Row.Pile m_Pile = null;
        #endregion
        #region VAttr{g}: OWWindow.Row Row - the owning row
        public OWWindow.Row Row
        {
            get
            {
                return Pile.Row;
            }
        }
        #endregion
        #region VAttr{g}: OWWindow Window - the owning window
        public OWWindow Window
        {
            get
            {
                return Pile.Window;
            }
        }
        #endregion
        #region Attr{g}: JObject DataSource - the DParagraph or DNote behind this OWPara
        public JObject DataSource
        {
            get
            {
                Debug.Assert(null != m_objDataSource);
                return m_objDataSource;
            }
        }
        JObject m_objDataSource = null;
        #endregion

        // Blocks ----------------------------------------------------------------------------
        #region Block Operations (Add, Remove, etc)
        #region Attr{g}: EBlock[] Blocks - the list of displayable elements in this paragraph
        public EBlock[] Blocks
        {
            get
            {
                Debug.Assert(null != m_vBlocks);
                return m_vBlocks;
            }
        }
        EBlock[] m_vBlocks;
        #endregion
        #region Method: void AddBlock(EBlock block)
        void AddBlock(EBlock block)
        {
            EBlock[] v = new EBlock[Blocks.Length + 1];

            for (int i = 0; i < Blocks.Length; i++)
                v[i] = Blocks[i];

            v[Blocks.Length] = block;

            m_vBlocks = v;
        }
        #endregion
        #region Method: void RemoveBlockAt(int iPos)
        void RemoveBlockAt(int iPos)
        {
            Debug.Assert(Blocks.Length > 0);

            EBlock[] v = new EBlock[Blocks.Length - 1];

            int i = 0;
            for (; i < iPos; i++)
                v[i] = Blocks[i];
            for (; i < v.Length; i++)
                v[i] = Blocks[i + 1];

            m_vBlocks = v;
        }
        #endregion
        #region Method: void RemoveBlocksAt(int iPos, int cBlocksToRemove)
        public void RemoveBlocksAt(int iPos, int cBlocksToRemove)
        {
            Debug.Assert(Blocks.Length >= cBlocksToRemove);

            // Create a new array to hold our answer
            EBlock[] v = new EBlock[Blocks.Length - cBlocksToRemove];

            // Copy over the blocks prior to the position iPos
            int i = 0;
            for (; i < iPos; i++)
                v[i] = Blocks[i];

            // Copy over the final blocks following cBlocksToremove
            for (; i < v.Length; i++)
                v[i] = Blocks[i + cBlocksToRemove];

            // Replace the original vector with our new one
            m_vBlocks = v;
        }
        #endregion
        #region Method: void InsertBlocks(int iPos, EBlock[] vInsert)
        void InsertBlocks(int iPos, EBlock[] vInsert)
        {
            // Create a new vector that will hold the entirety
            EBlock[] v = new EBlock[Blocks.Length + vInsert.Length];

            // Copy over all of the EBlocks prior to position iPos
            int i = 0;
            for (; i < iPos; i++)
                v[i] = Blocks[i];

            // Copy in the new EBlocks we are inserting
            for (int k = 0; k < vInsert.Length; k++)
                v[i + k] = vInsert[k];

            // Copy the remaining EBlocks from the original vector
            for (; i < Blocks.Length; i++)
                v[i + vInsert.Length] = Blocks[i];

            // Replace the original vector with our new one
            m_vBlocks = v;
        }
        #endregion
        #region Method: void AppendBlocks(EBlock[] vAppend)
        void AppendBlocks(EBlock[] vAppend)
        {
            InsertBlocks(Blocks.Length, vAppend);
        }
        #endregion
        #region Method: void ClearBlocks()
        void ClearBlocks()
        {
            m_vBlocks = new EBlock[0];
        }
        #endregion
        #region Method: int IndexOfBlock(EBlock block)
        int IndexOfBlock(EBlock block)
        {
            for (int i = 0; i < Blocks.Length; i++)
            {
                if (Blocks[i] == block)
                    return i;
            }
            return -1;
        }
        #endregion
        #endregion
        #region CLASS: EBlock
        public class EBlock
        {
            // Main Content Attrs ------------------------------------------------------------
            #region Attr{g/s}: string Text
            public string Text
            {
                get
                {
                    return m_sText;
                }
                set
                {
                    m_sText = value;
                }
            }
            protected string m_sText = "";
            #endregion
            #region Attr{g}: bool GlueToNext - T if this block must be beside the next one
            public virtual bool GlueToNext
            {
                get
                {
                    return m_bGlueToNext;
                }
                set
                {
                    m_bGlueToNext = value;
                }
            }
            bool m_bGlueToNext = false;
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
            public virtual float MeasuredWidth
            {
                get
                {
                    return m_fMeasuredWidth;
                }
            }
            protected float m_fMeasuredWidth = 0;
            #endregion
            #region VAttr{g}: int LineCount - number of lines taken up by this block
            virtual protected int LineCount
            {
                get
                {
                    return 1;
                }
            }
            #endregion 
            #region VAttr{g}: float Height - calc'd from LineCount and Para.LineHeight
            public float Height
            {
                get
                {
                    return Para.LineHeight * LineCount;
                }
            }
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
            public virtual bool ContainsPoint(PointF pt)
            {
                RectangleF r = new RectangleF(Position, 
                    new SizeF(MeasuredWidth + JustificationPaddingAdded, Height));
                return r.Contains(pt);
            }
            #endregion
            #region Attr{g}: int JustificationPaddingAdded
            public int JustificationPaddingAdded
            {
                get
                {
                    return m_nJustificationPaddingAdded;
                }
                set
                {
                    m_nJustificationPaddingAdded = value;
                }
            }
            int m_nJustificationPaddingAdded = 0;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Attr{g}: OWPara Para - Keep a pointer back to the owner so we can get, e.g., the PStyle
            public OWPara Para
            {
                get
                {
                    Debug.Assert(null != m_para);
                    return m_para;
                }
            }
            OWPara m_para = null;
            #endregion
            #region VAttr{g}: OWWindow Window
            public OWWindow Window
            {
                get
                {
                    return Para.Window;
                }
            }
            #endregion
            #region VAttr{g}: OWWindow.DrawBuffer Draw
            public OWWindow.DrawBuffer Draw
            {
                get
                {
                    return Para.Window.Draw;
                }
            }
            #endregion
            #region VAttr{g}: int PositionWithinPara
            public int PositionWithinPara
            {
                get
                {
                    for (int i = 0; i < Para.Blocks.Length; i++)
                    {
                        if (Para.Blocks[i] as EBlock == this)
                            return i;
                    }
                    return -1;
                }
            }
            #endregion
            #region Constructor(OWPara, sText)
            public EBlock(OWPara _para, string _sText)
            {
                m_para = _para;
                m_sText = _sText;

                // Default for the Character Style is the DParagraph's style
                m_FontForWS = Para.PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                    Para.WritingSystem);
            }
            #endregion

            // Painting ----------------------------------------------------------------------
            #region Attr{g}: JFontForWritingSystem FontForWS - remember it here for performance
            protected JFontForWritingSystem FontForWS
            {
                get
                {
                    Debug.Assert(null != m_FontForWS);
                    return m_FontForWS;
                }
            }
            protected JFontForWritingSystem m_FontForWS = null;
            #endregion
            #region Method: Font GetSuperscriptFont()
            protected Font GetSuperscriptFont()
            {
                return new Font(FontForWS.FontNormalZoom.FontFamily,
                    FontForWS.FontNormalZoom.Size * 0.8F,
                    FontForWS.FontNormalZoom.Style);
            }
            #endregion
            #region Method: Brush GetBrush()
            protected Brush GetBrush()
            {
                return new SolidBrush(FontForWS.FontColor);
            }
            #endregion
            #region Method: Pen GetPen()
            protected Pen GetPen()
            {
                return new Pen(FontForWS.FontColor);
            }
            #endregion

            #region Method: virtual void MeasureWidth(Graphics g)
            public virtual void MeasureWidth(Graphics g)
                // Those subclasses which override will not need to call this base method.
            {
                // For the most cases, we'll measure the width of Text according to the
                // font stored in the CharacterStyle.
                StringFormat fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                m_fMeasuredWidth = g.MeasureString(Text, FontForWS.FontNormalZoom, 1000, fmt).Width;
            }
            #endregion

            #region Method: virtual void Paint()
            virtual public void Paint()
            {
//                Debug.Assert(false);
            }
            #endregion

            // Mousing
            #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
            public virtual Cursor MouseOverCursor
            {
                get
                {
                    return Cursors.Arrow;
                }
            }
            #endregion
            #region Cmd: cmdLeftMouseClick
            public virtual void cmdLeftMouseClick(PointF pt)
            {
            }
            #endregion
            #region Cmd: cmdLeftMouseDoubleClick
            public virtual void cmdLeftMouseDoubleClick(PointF pt)
            {
            }
            #endregion

            #region Cmd: cmdMouseMove
            public virtual void cmdMouseMove(PointF pt)
            {
            }
            #endregion
        }
        #endregion
        #region CLASS: EChapter
        public class EChapter : EBlock
        {
            // Screen Region -----------------------------------------------------------------
            #region Attr{g}: override int LineCount - A Chapter by definition takes up two lines.
            protected override int LineCount
            {
                get
                {
                    return 2;
                }
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(OWPara, DChapter)
            public EChapter(OWPara para, DChapter chapter)
                : base(para, chapter.Text)
            {
                // Add a little space to the end so that it appears a bit nicer in the 
                // display. It is uneditable, so this only affects the display.
                m_sText = Text + "\u00A0";

                // We want to point to the appropriate font and store it for performance reasons
                JCharacterStyle cs = G.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevChapter);
                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);
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
                float y = Position.Y + (Height / 2) - (FontForWS.FontNormalZoom.Height / 2);

                // Draw the string
                Draw.String(Text, FontForWS.FontNormalZoom, GetBrush(), new PointF(x, y));
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

            #region Method: override void MeasureWidth(g)
            public override void MeasureWidth(Graphics g)
            {
                string s = Text;

                if (NeedsExtraLeadingSpace)
                    s = c_sLeadingSpace + Text;

                StringFormat fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                m_fMeasuredWidth = g.MeasureString(s, FontForWS.FontNormalZoom, 1000, fmt).Width;
            }
            #endregion

            #region Constructor(OWPara, DVerse)
            public EVerse(OWPara para, DVerse verse)
                : base(para, verse.Text)
            {
                JCharacterStyle cs = G.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevVerse);
                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);
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
                string s = Text;
                if (NeedsExtraLeadingSpace)
                    s = c_sLeadingSpace + Text;
                Draw.String(s, GetSuperscriptFont(), GetBrush(), Position);
            }
            #endregion
        }
        #endregion
        #region CLASS: EFootLetter
        public class EFootLetter : EBlock
        {
            #region Attr{g}: DFootnote Footnote
            public DFootnote Footnote
            {
                get
                {
                    Debug.Assert(null != m_Footnote);
                    return m_Footnote;
                }
            }
            DFootnote m_Footnote = null;
            #endregion

            #region Constructor(OWPara, DFootLetter)
            public EFootLetter(OWPara para, DFootLetter footLetter)
                : base(para, footLetter.Text)
            {
                JCharacterStyle cs = G.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevFootLetter);
                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);

                m_Footnote = footLetter.Footnote;
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
                    return Cursors.Hand;
                }
            }
            #endregion
            #region Method: override void cmdLeftMouseClick(PointF pt)
            public override void cmdLeftMouseClick(PointF pt)
            {
                Window.Select_FirstPositionInParagraph(Footnote);
            }
            #endregion
        }
        #endregion
        #region CLASS: ESeeAlso
        public class ESeeAlso : EBlock
        {
            #region Attr{g}: DFootnote Footnote
            public DFootnote Footnote
            {
                get
                {
                    Debug.Assert(null != m_Footnote);
                    return m_Footnote;
                }
            }
            DFootnote m_Footnote = null;
            #endregion
            
            #region Constructor(OWPara, DSeeAlso)
            public ESeeAlso(OWPara para, DSeeAlso seeAlso)
                : base(para, seeAlso.Text)
            {
                JCharacterStyle cs = G.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevSeeAlsoLetter);
                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);

                m_Footnote = seeAlso.Footnote;
            }
            #endregion

            #region Method: override void Paint()
            public override void Paint()
            {
                Draw.String(Text, GetSuperscriptFont(), GetBrush(), Position);
            }
            #endregion
        }
        #endregion
        #region CLASS: ELabel
        class ELabel : EBlock
        {
            public const string c_Spaces = "\u00A0\u00A0";

            #region Constructor(OWPara, DLabel)
            public ELabel(OWPara para, DLabel label)
                : base(para, label.Text + c_Spaces)
            {
                JCharacterStyle cs = G.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevLabel);
                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);
            }
            #endregion

            #region Attr{g}: bool GlueToNext - Always T for a Label
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
                Draw.String(Text, FontForWS.FontNormalZoom, GetBrush(), Position);
            }
            #endregion
        }
        #endregion
        #region CLASS: ENote
        public class ENote : EBlock
        {
            #region Attr{g}: DNote Note
            public DNote Note
            {
                get
                {
                    Debug.Assert(null != m_Note);
                    return m_Note;
                }
            }
            DNote m_Note = null;
            #endregion
            #region Attr{g}: Bitmap Bmp - the note's bitmap
            Bitmap Bmp
            {
                get
                {
                    Debug.Assert(null != m_bmp);
                    return m_bmp;
                }
            }
            Bitmap m_bmp = null;
            #endregion

            #region Attr{g}: float MeasuredWidth
            public override float MeasuredWidth
                // The width here is the width of the bitmap
            {
                get
                {
                    return Bmp.Width;
                }
            }
            #endregion

            #region Constructor(OWPara, DNote)
            public ENote(OWPara para, DNote _Note)
                : base(para, "")
            {
                m_Note = _Note;

                // Retrieve the bitmap, with the correct background
                Color clrBackground = Para.Window.BackColor;
                m_bmp = Note.NoteDef.GetTransparentBitmap(clrBackground);
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
                    Window.MainWindow.OnSelectAndScrollFromNote(Note);
            }
            #endregion
        }
        #endregion
        #region CLASS: EWord
        public class EWord : EBlock
        {
            #region VAttr{g}: Font Font - returns the font for the WS, including Italic, Bold, etc.
            Font Font
            {
                get
                {
                    switch (m_mods)
                    {
                        case JFontForWritingSystem.Mods.Bold:
                            return FontForWS.FontBoldZoom;
                        case JFontForWritingSystem.Mods.Italic:
                            return FontForWS.FontItalicZoom;
                        case JFontForWritingSystem.Mods.BoldItalic:
                            return FontForWS.FontBoldItalicZoom;
                        default:
                            return FontForWS.FontNormalZoom;
                    }
                }
            }
            #endregion
            JFontForWritingSystem.Mods m_mods;

            // Scaffolding -------------------------------------------------------------------
            #region DPhrase Phrase - the phrase from which this EWord was generated
            public DPhrase Phrase
            {
                get
                {
                    Debug.Assert(null != m_Phrase);
                    return m_Phrase;
                }
            }
            DPhrase m_Phrase = null;
            #endregion
            #region Constructor(OWPara, DPhrase, sText)
            public EWord(OWPara para, JCharacterStyle style, DPhrase phrase, string sText, JFontForWritingSystem.Mods _mods)
                : base(para, sText)
            {
                // Remember the phrase from which this EWord was generated
                m_Phrase = phrase;

                // Retrieve the JFont as passed in.
                JCharacterStyle cs = style;
                m_mods = _mods;

                // Switch to italics (or whatever) if that is what is desired
                if (phrase.CharacterStyleAbbrev != DStyleSheet.c_StyleAbbrevNormal &&
                    m_mods == JFontForWritingSystem.Mods.None)
                {
                    cs = G.StyleSheet.FindCharacterStyleOrNormal(phrase.CharacterStyleAbbrev);
                }

                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);
            }
            #endregion
            #region static EWord CreateAsInsertionIcon(OWPara, JCharacterStyle, DPhrase)
            static public EWord CreateAsInsertionIcon(
                OWPara para, JCharacterStyle style, DPhrase phrase)
            {
                EWord word = new EWord(para, style, phrase, c_chInsertionSpace.ToString(), 
                    JFontForWritingSystem.Mods.None);
                return word;
            }
            #endregion

            // Painting ----------------------------------------------------------------------
            #region Method: void PaintBackgroundRectangle(Color color)
            void PaintBackgroundRectangle(Color color)
            {
                RectangleF r = new RectangleF(Position,
                    new SizeF(MeasuredWidth + JustificationPaddingAdded, Height));
                Draw.FillRectangle(color, r);
            }
            #endregion
            #region Method: override void Paint()
            public override void Paint()
            {
                // The white background
                PaintBackgroundRectangle(
                    (Para.Editable && !Window.LockedFromEditing) ? 
                        Para.EditableBackgroundColor : 
                        Window.BackColor );

                // The text
                if (!IsInsertionIcon)
                    Draw.String(Text, Font, GetBrush(), Position);
            }
            #endregion
            #region Method: void PaintSelection(int iCharLeft, int iCharRight)
            public void PaintSelection(int iCharLeft, int iCharRight)
            {
                // Create the colors and brushes we'll need
                Color clrSelectedBackground = SystemColors.Highlight;
                Color clrSelectedText = SystemColors.HighlightText;
                Brush brushSelectedText = new SolidBrush(clrSelectedText);
                Brush brushNormalText = new SolidBrush(FontForWS.FontColor);

                // Insertion Icon
                if (IsInsertionIcon)
                {
                    PaintBackgroundRectangle(clrSelectedBackground);
                    Draw.String(StrRes.TypeHere, Font, brushSelectedText, Position);
                    return;
                }

                // Interpret the default values
                if (iCharLeft == -1)
                    iCharLeft = 0;
                if (iCharRight == -1)
                    iCharRight = Text.Length;

                // Optimization/Shortcut: Paint the entire word as selected if apropriate
                if (iCharLeft == 0 && iCharRight == Text.Length)
                {
                    PaintBackgroundRectangle(clrSelectedBackground);
                    Draw.String(Text, Font, brushSelectedText, Position);
                    return;
                }

                // If the two parameters are still the same, then we have no selection to paint
                if (iCharLeft == iCharRight)
                    return;

                // Figure out the selection texts
                string sLeft = (iCharLeft == 0) ? "" : Text.Substring(0, iCharLeft);
                string sTemp = (iCharLeft == 0) ? Text : Text.Substring(iCharLeft);
                string sSelected = (iCharRight == Text.Length) ? sTemp : 
                    sTemp.Substring(0, iCharRight - iCharLeft);
                string sRight = (iCharRight == Text.Length) ? "" : 
                    sTemp.Substring(iCharRight - iCharLeft);

                // Figure out the boundaries
                float fTotalWidth = MeasuredWidth + JustificationPaddingAdded;
                float xSelLeft = Position.X +
                    ((iCharLeft == 0) ? 0 : Draw.Measure(sLeft, Font));
                float xSelRight = xSelLeft + ((iCharRight == Text.Length) ?
                    fTotalWidth - Draw.Measure(sLeft, Font) :
                    Draw.Measure(sSelected, Font));

                // Paint the white background, for those portions that are not selected
                PaintBackgroundRectangle(
                    Window.LockedFromEditing ? Window.BackColor : Para.EditableBackgroundColor);

                // Paint the selected background
                RectangleF rectSelected = new RectangleF(xSelLeft, Position.Y, xSelRight - xSelLeft, Height);
                Draw.FillRectangle(clrSelectedBackground, rectSelected);

                // Paint the text
                if (sLeft.Length > 0)
                    Draw.String(sLeft, Font, brushNormalText, Position);

                Draw.String(sSelected, Font, brushSelectedText, 
                    new PointF(xSelLeft, Position.Y));

                if (sRight.Length > 0)
                {
                    Draw.String(sRight, Font, brushNormalText, 
                        new PointF(xSelRight, Position.Y));
                }
            }
            #endregion
            #region Method: float GetXat(int i)
            public float GetXat(int i)
            {
                if (i == 0)
                    return 0;

                return Draw.Measure(Text.Substring(0, i), Font);
            }
            #endregion

            // Insertion Point ---------------------------------------------------------------
            public const char c_chInsertionSpace = '\u2004';   // Unicode's "Four-Per-EM space"
            public const float c_xInsertionSize = 50;          // Pixels for insertion display
            #region Attr{g}: bool IsInsertionIcon
            public bool IsInsertionIcon
            {
                get
                {
                    if (Text.Length == 1 && Text[0] == c_chInsertionSpace)
                        return true;
                    return false;
                }
            }
            #endregion
            #region Method: override void MeasureWidth(Graphics g)
            public override void MeasureWidth(Graphics g)
            {
                if (IsInsertionIcon)
                    m_fMeasuredWidth = Draw.Measure(StrRes.TypeHere, Font);
                else
                    base.MeasureWidth(g);
            }
            #endregion

            // Queries -----------------------------------------------------------------------
            #region Method: int GetCharacterIndex(PointF pt)
            public int GetCharacterIndex(PointF pt)
            // Find out the index of the character we've just clicked over
            {
                int iChar = 0;
                float x1 = Position.X;
                for (; iChar < Text.Length; iChar++)
                {
                    // Get the substring up to iChar +1 letters (this will max out at the
                    // total possible length of Text.
                    string sPortion = Text.Substring(0, iChar + 1);

                    // Get the position right after this character
                    float x2 = Position.X + Draw.Measure(sPortion, Font);

                    // The average should thus be right in the midst of this character,
                    // since x1 represents the position to the char's left.
                    float xAvg = (x1 + x2) / 2;

                    // So if we are less than the midpoint, then we'll be returning the
                    // value of this character; otherwise we loop to the next one.
                    if (pt.X < xAvg)
                        break;

                    x1 = x2;
                }
                return iChar;
            }
            #endregion
            #region Method: bool IsBesideEWord(bool bOnRight)
            public bool IsBesideEWord(bool bOnRight)
            {
                int iBlock = Para.IndexOfBlock(this);

                EBlock blockBeside = null;

                if (bOnRight)
                {
                    if (iBlock == Para.Blocks.Length - 1)
                        return false;
                    blockBeside = Para.Blocks[iBlock + 1];
                }
                else
                {
                    if (iBlock == 0)
                        return false;
                    blockBeside = Para.Blocks[iBlock - 1];
                }

                if (null != blockBeside as EWord)
                    return true;

                return false;
            }
            #endregion
            #region Attr{g}: bool EndsWithWhiteSpace
            public bool EndsWithWhiteSpace
            {
                get
                {
                    if (Text.Length == 0)
                        return false;
                    if (char.IsWhiteSpace(Text[Text.Length - 1]))
                        return true;
                    return false;
                }
            }
            #endregion

            // Command Processing ------------------------------------------------------------
            #region Attr{g}: Cursor MouseOverCursor - Shape of cursor indicates what a Left-Click will do
            public override Cursor MouseOverCursor
            {
                get
                {
                    if (!Para.Editable)
                        return Cursors.Default;
                    return Cursors.IBeam;
                }
            }
            #endregion
            #region Cmd: cmdLeftMouseClick - Make a selection at this coordinate
            public override void cmdLeftMouseClick(PointF pt)
                // Create a selection Anchor at this point. Given a click in the middle of
                // a letter, we decide whether to select at the beginning or end of that
                // letter (by finding the average of the begin/end measurements of that
                // letter.)
            {
                // If the paragraph is not editable, then it is a moot point; no sel is possible
                if (!Para.Editable)
                    return;

                // Find out the index of the character we've just clicked over
                int iChar = GetCharacterIndex(pt);

                // Get the number of this block within the paragraph
                int iBlock = PositionWithinPara;

                // For an insertion icon, just select the entire word
                if (IsInsertionIcon)
                {
                    Para.Window.Selection = new OWWindow.Sel(Para, 
                        new OWWindow.Sel.SelPoint(iBlock, 0),
                        new OWWindow.Sel.SelPoint(iBlock, Text.Length));
                    return;
                }

                // Create the Selection
                OWWindow.Sel.SelPoint Anchor = new OWWindow.Sel.SelPoint(iBlock, iChar);
                Para.Window.Selection = new OWWindow.Sel(Para, Anchor);

                // Deal with those at the end of a word
                Para.NormalizeSelection();
            }
            #endregion
            #region Cmd: cmdMouseMove
            public override void cmdMouseMove(PointF pt)
            {
                // Do we already have a selection?
                if (Window.Selection == null)
                    return;

                // Is the selection within this paragraph?
                if (Window.Selection.Paragraph != Para)
                    return;

                // If this paragraph is not editable, then we can't do anything (probably
                // should never get to this code anyway.)
                if (!Para.Editable)
                    return;

                // Is there anything between Here and the Anchor that is NOT an EWord?
                int iEnd = PositionWithinPara;
                int iAnchor = Window.Selection.Anchor.iBlock;
                int iFirst = (iAnchor < iEnd) ? iAnchor : iEnd;
                int iLast = (iEnd > iAnchor) ? iEnd : iAnchor;
                for (int i = iFirst; i < iLast; i++)
                {
                    if (Para.Blocks[i] as EWord == null)
                        return;
                }

                // Passed the test; so extend the selection to this new End point
                int iChar = GetCharacterIndex(pt);
                OWWindow.Sel.SelPoint end = new OWWindow.Sel.SelPoint(iEnd, iChar);
                Window.Selection = new OWWindow.Sel(Para, Window.Selection.Anchor, end);
            }
            #endregion
            #region Cmd: cmdLeftMouseDoubleClick
            public override void cmdLeftMouseDoubleClick(PointF pt)
            {
                // If the paragraph is not editable, then it is a moot point; no sel is possible
                if (!Para.Editable)
                    return;

                // Get the number of this block within the paragraph
                int iBlock = PositionWithinPara;

                // Select the entire word
                Para.Window.Selection = new OWWindow.Sel(Para,
                    new OWWindow.Sel.SelPoint(iBlock, 0),
                    new OWWindow.Sel.SelPoint(iBlock, Text.Length));
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

            #region Constructor(OWPara, DFootLetter)
            public EFootnoteLabel(OWPara para, DFootnote footnote)
                : base(para, footnote.Letter + " ")
            {
                JCharacterStyle cs = G.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevFootLetter);
                m_FontForWS = cs.FindOrAddFontForWritingSystem(Para.WritingSystem);
              
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
                    return Cursors.Hand;
                }
            }
            #endregion
            #region Method: override void cmdLeftMouseClick(PointF pt)
            public override void cmdLeftMouseClick(PointF pt)
            {
                Window.OnSelectAndScrollFromFootnote(Footnote);
            }
            #endregion
        }
        #endregion
        #region CLASS: EBigHeader
        class EBigHeader : EBlock
        {
            #region Constructor(OWPara, string sText)
            public EBigHeader(OWPara para, JWritingSystem ws, string sText)
                : base(para, sText + " ")
            {
                JCharacterStyle cs = G.StyleSheet.FindParagraphStyle(
                    DStyleSheet.c_StyleSectionTitle).CharacterStyle;
                m_FontForWS = cs.FindOrAddFontForWritingSystem(ws);
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
                Draw.String(Text, FontForWS.FontNormalZoom, GetBrush(), Position);
            }
            #endregion
        }
        #endregion

        // Initialize to a paragraph's contents ----------------------------------------------
        #region Method: EWord[] ParseBasicTextIntoWords(DBasicText t)
        EWord[] ParseBasicTextIntoWords(DBasicText t, JCharacterStyle CStyleOverride)
        {
            // Select between the Vernacular vs the Back Translation text
            DBasicText.DPhrases phrases = (DisplayBT) ? t.PhrasesBT : t.Phrases;
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
                JFontForWritingSystem.Mods mods = JFontForWritingSystem.Mods.None;
                JCharacterStyle cs = CStyleOverride;
                if (null == cs)
                {
                    cs = t.CharacterStyle;
                    if (phrase.CharacterStyleAbbrev != DStyleSheet.c_StyleAbbrevNormal)
                    {
                        if (phrase.CharacterStyle.Abbrev == DStyleSheet.c_StyleAbbrevBold)
                            mods = JFontForWritingSystem.Mods.Bold;
                        else if (phrase.CharacterStyle.Abbrev == DStyleSheet.c_StyleAbbrevItalic)
                            mods = JFontForWritingSystem.Mods.Italic;
                        else
                            cs = phrase.CharacterStyle;
                    }
                }

                // Process through the phrase's text string
                for (int i = 0; i < phrase.Text.Length; i++)
                {
                    // If we are sitting at a word break, then add the word and reset
                    // in order to build the next one.
                    if (WritingSystem.IsWordBreak(phrase.Text, i) && sWord.Length > 0)
                    {
                        a.Add(new EWord(this, cs, phrase, sWord, mods));
                        sWord = "";
                    }

                    // Add the character to the word we are building
                    sWord += phrase.Text[i];
                }

                // Pick up the final word in the string, IsWordBreak will not have 
                // caught it.
                if (sWord.Length > 0)
                    a.Add(new EWord(this, cs, phrase, sWord, mods));
            }

            // If we did not find any words, then we want to create an InsertionIcon
            if (a.Count == 0)
            {
                JCharacterStyle cStyle = (null != t.Paragraph) ?
                    t.Paragraph.Style.CharacterStyle :
                    t.Note.Style.CharacterStyle;

                a.Add(EWord.CreateAsInsertionIcon(this, cStyle, phrases[0]));
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
            AppendBlocks(vWords);
        }
        #endregion
        #region Method: void _InitializeTextNotes(DText t)
        void _InitializeTextNotes(DText t)
            // Whether or not a particular note is actually shown depends both on Global
            // settings and on the particular context. We currently have all of that
            // logic in the Show attr in DNote.
        {
            // The logic is that if the paragraph is not Editable, then we don't display
            // any notes for it. This prevents the notes icons from showing up in the
            // Back Translation view, on both the Left and the Right Sides, as we only
            // want them on the Right side, not both places.
            if (HideNotesIcons)
                return;

            Debug.Assert(null != t);
            foreach (DNote note in t.Notes)
            {
                if (note.Show)
                {
                    AddBlock(new ENote(this, note));

                    Window.Secondary_AddNote(note, true);
                }
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
            Debug.Assert(iRight < Blocks.Length);

            // Retrieve their EBlock objects
            EBlock blockLeft = Blocks[iLeft] as EBlock;
            EBlock blockRight = Blocks[iRight] as EBlock;
            Debug.Assert(null != blockLeft && null != blockRight);

            // The default is not to glue
            blockLeft.GlueToNext = false;

            // But we do glue in the case, e.g., where we are followed by certain objects
            if (blockRight as EFootLetter != null)
                blockLeft.GlueToNext = true;
            if (blockRight as ESeeAlso != null)
                blockLeft.GlueToNext = true;
            if (blockRight as ENote != null)
                blockLeft.GlueToNext = true;
            if (blockLeft as EFootnoteLabel != null)
                blockLeft.GlueToNext = true;
        }
        #endregion
        #region Method: void _Initialize(DParagraph)
        void _Initialize(DParagraph p)
        {
            // Clear out any previous list contents
            ClearBlocks();

            // If this is a footnote, we need to add its letter
            if (null != p as DFootnote)
                AddBlock(new EFootnoteLabel(this, p as DFootnote)); 

            // Loop through the paragraph's runs
            foreach (DRun r in p.Runs)
            {
                switch (r.GetType().Name)
                {
                    case "DVerse":
                        AddBlock(new EVerse(this, r as DVerse));
                        break;
                    case "DChapter":
                        AddBlock(new EChapter(this, r as DChapter));
                        break;
                    case "DFootLetter":
                        AddBlock(new EFootLetter(this, r as DFootLetter));
                        break;
                    case "DSeeAlso":
                        AddBlock(new ESeeAlso(this, r as DSeeAlso));
                        break;
                    case "DLabel":
                        AddBlock(new ELabel(this, r as DLabel));
                        break;
                    case "DBasicText":
                        _InitializeBasicTextWords(r as DBasicText, null);
                        break;
                    case "DText":
                        _InitializeBasicTextWords(r as DBasicText, null);
                        _InitializeTextNotes(r as DText);
                        break;
                    default:
                        Console.WriteLine("Unknown type in OWPara.Initialize...Name=" + 
                            r.GetType().Name);
                        Debug.Assert(false);
                        break;
                }
            }

            // Loop through to set the GlueToNext values
            for (int i = 0; i < Blocks.Length - 1; i++)
                _InitializeGlueToNext(i);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Attr{g}: bool Editable - true if the paragraph can be edited
        public bool Editable
        {
            get
            {
                return m_bEditable;
            }
        }
        bool m_bEditable = true;
        #endregion
        #region Attr{g}: bool DisplayBT
        public bool DisplayBT
        {
            get
            {
                return m_bDisplayBT;
            }
        }
        bool m_bDisplayBT = false;
        #endregion
        #region VAttr{g}: bool HideNotesIcons - Surpress Notes, even if they'd otherwise appear
        public bool HideNotesIcons
            // In the Back Translation Job, we need to make sure that we see the Notes icons
            // only on the right-hand side; the side that is editable. By basing the 
            // decision on "Editable", we get the job done.
            //
            // Otherwise, we could always pass in a parameter to "AddParagraph" and deal
            // with it as we create the paragraphs. But I think that this strategy should
            // be adequate.
        {
            get
            {
                if (!Editable)
                    return true;
                return false;
            }
        }
        #endregion

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
        JParagraphStyle PStyle
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

        #region private Constructor(...) - the stuff that is in common
        private OWPara(
            OWWindow.Row.Pile _Pile, 
            JObject _objDataSource,
            JWritingSystem _ws,
            bool _bDisplayBT,
            bool _bEditable)
        {
            // Keep track of the attributes passed in
            m_Pile = _Pile;
            m_objDataSource = _objDataSource;
            m_WritingSystem = _ws;
            m_bDisplayBT = _bDisplayBT;
            m_bEditable = _bEditable;

            // Initialize the vector of Blocks
            ClearBlocks();

            // Initialize the list of Lines
            m_vLines = new Line[0];
        }
        #endregion
        #region Constructor(Pile, DParagraph, JWritingSystem, bDisplayBT, bool bEditable) - for DParagraph
        public OWPara(OWWindow.Row.Pile _Pile, DParagraph p, JWritingSystem _ws, 
            bool _bDisplayBT, bool _bEditable)
            : this(_Pile, p as JObject, _ws, _bDisplayBT, _bEditable)
        {
            // The paragraph itself may override to make itself uneditable, 
            // even though we received "true" from the _bEditable parameter.
            if (!p.IsUserEditable)
                m_bEditable = false;

            // Store the paragraph's style here, so we don't have to keep
            // looking it up (p.Style does an Find into the stylesheet.)
            m_pStyle = p.Style;

            // Line height
            m_fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                WritingSystem).FontNormalZoom.Height;

            // Interpret the paragraph's contents into our internal data structure
            _Initialize(p);

            // Retrieve the background color for editable parts of the paragraph
            m_EditableBackgroundColor = Window.EditableBackgroundColor;
        }
        #endregion
        #region Constructor(Pile, DNote, JWritingSystem) - for DNote
        public OWPara(OWWindow.Row.Pile _Pile, DNote note, JWritingSystem _ws, bool _bEditable)
            : this(_Pile, note as JObject, _ws, false, _bEditable)
        {
            // The note itself may override <_bEditable> to make itself uneditable, 
            // even though we received "true" from the <_bEditable> parameter.
            if (!note.IsUserEditable)
                m_bEditable = false;

            // Store the paragraph's style here, so we don't have to keep
            // looking it up (p.Style does an Find into the stylesheet.)
            m_pStyle = G.StyleSheet.FindParagraphStyle(DNote.StyleAbbrev);

            // Line height
            m_fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                WritingSystem).FontNormalZoom.Height;

            // Add the note's bitmap
            ENote nNote = new ENote(this, note);
            nNote.GlueToNext = true;
            AddBlock(nNote);

            // Add the verse reference
            EVerse nVerse = new EVerse(this, new DVerse(" " + note.Reference + " "));
            nVerse.GlueToNext = true;
            AddBlock(nVerse);

            // Add the note text
            _InitializeBasicTextWords(note.NoteText, null);

            // Retrieve the background color for editable parts of the note
            m_EditableBackgroundColor = note.NoteDef.BackgroundColor;
        }
        #endregion
        #region Constructor(Pile, DRun[], JWritingSystem, sLabel)
        public OWPara(OWWindow.Row.Pile _Pile, DRun[] vRuns, JWritingSystem _ws, string sLabel)
            : this(_Pile, vRuns[0].Owner, _ws, false, false)
            // For the Related Languages window
        {
            // For the style, we'll just use normal
            m_pStyle = G.StyleSheet.FindParagraphStyleOrNormal(DStyleSheet.c_StyleReferenceTranslation);

            // Line height
            m_fLineHeight = PStyle.CharacterStyle.FindOrAddFontForWritingSystem(
                WritingSystem).FontNormalZoom.Height;

            // We'll add the language name as a BigHeader
            AddBlock(new EBigHeader(this, WritingSystem, sLabel));

            // Add the text (we only care about verses and text)
            foreach (DRun run in vRuns)
            {
                switch (run.GetType().Name)
                {
                    case "DVerse":
                        AddBlock(new EVerse(this, run as DVerse));
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
            for (int i = 0; i < Blocks.Length - 1; i++)
            {
                EWord word = Blocks[i] as EWord;
                if (null == word || !word.IsBesideEWord(true))
                    continue;
                if (!word.EndsWithWhiteSpace)
                    word.Text += ' ';
            }
        }
        #endregion

        // Screen Region ---------------------------------------------------------------------
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
                return new Rectangle(x,y,w,h);
            }
        }
        #endregion
        #region Method: virtual bool ContainsPoint(PointF pt)
        public bool ContainsPoint(PointF pt)
        {
            return Rectangle.Contains(pt);
        }
        #endregion

        // Layout Dependant ------------------------------------------------------------------
        #region CLASS: Line
        public class Line
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
            #region Attr{g}: Block[] Blocks
            public EBlock[] Blocks
            {
                get
                {
                    Debug.Assert(null != m_vBlocks);
                    return m_vBlocks;
                }
            }
            EBlock[] m_vBlocks = null;
            #endregion
            #region Method: void AddBlock(Block block)
            public void AddBlock(EBlock block)
            {
                EBlock[] v = new EBlock[Blocks.Length + 1];

                for (int i = 0; i < Blocks.Length; i++)
                    v[i] = Blocks[i];

                v[Blocks.Length] = block;

                m_vBlocks = v;
            }
            #endregion

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
            #region Attr{g}: float MeasuredWidth
            public float MeasuredWidth
            {
                get
                {
                    float f = 0;
                    foreach (EBlock b in Blocks)
                        f += b.MeasuredWidth;
                    return f;
                }
            }
            #endregion

            #region Constructor()
            public Line()
            {
                m_vBlocks = new EBlock[0];
            }
            #endregion

            #region Method: void SetPositions(float x, float y, float xMaxWidth, bool bJustify)
            public void SetPositions(float x, float y, float xWidthToFill, bool bJustify)
            {
                // Remember the position of this line
                m_ptPosition = new PointF(x, y);

                // Set the Chapter, if we have one
                if (null != Chapter)
                    Chapter.Position = new PointF(x, y);

                // Indent for the chapter if necessary
                x += LeftIndent;

                // Calculate how many pixels we have to justify
                float fRawWidthOfMaterial = LeftIndent + MeasuredWidth;
                int cPixelsToJustify = (int)(xWidthToFill - fRawWidthOfMaterial);

                // Calculate how many positions we can add these pixels to
                int cJustificationPositions = Blocks.Length - 1;
                foreach (EBlock block in Blocks)
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

                foreach (EBlock block in Blocks)
                {
                    block.Position = new PointF(x, y);
                    x += block.MeasuredWidth;

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
                        if (block != Blocks[Blocks.Length-1])
                            block.JustificationPaddingAdded = nPad;
                    }
                }
            }
            #endregion

            #region Method: EBlock GetBlockAt(PointF pt)
            public EBlock GetBlockAt(PointF pt)
            {
                // Is it in the EChapter?
                if (null != Chapter)
                {
                    if (Chapter.ContainsPoint(pt))
                        return Chapter;
                }

                // Quickly rule out points that are above or below the line
                if (Blocks.Length == 0)
                    return null;
                if (pt.Y < Position.Y)
                    return null;
                if (pt.Y > Position.Y + Blocks[0].Height)
                    return null;

                // Scan through all of the blocks
                foreach (EBlock block in Blocks)
                {
                    if (block.ContainsPoint(pt))
                        return block;
                }

                return null;
            }
            #endregion
            #region Method: bool Contains(EBlock block)
            public bool Contains(EBlock block)
            {
                if (block == Chapter)
                    return true;

                foreach (EBlock b in Blocks)
                {
                    if (b == block)
                        return true;
                }
                return false;
            }
            #endregion
            #region Method: int IndexOfBlock(EBlock block)
            public int IndexOfBlock(EBlock block)
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i] == block)
                        return i;
                }
                return -1;
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
                foreach(EBlock b in Blocks)
                {
                    // Retrieve each EWord in the line
                    EWord w = b as EWord;
                    if (null == w)
                        continue;

                    // Calculate its distance
                    float d1 = Math.Abs(pt.X - w.Position.X);
                    float d2 = Math.Abs(pt.X - (w.Position.X + w.MeasuredWidth));
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
        #region Method: float Layout_CalcNextChunkWidth(...)
        float Layout_CalcNextChunkWidth(Graphics g, int i, out int cChunkSize)
            // Calculate the width of the next "chunk" that we would add to the line, if
            // it will fit.
        {
            // Retrieve the first element (the one at "i")
            EBlock block = Blocks[i] as EBlock;

            // Measure the proposed Element
            float fWidth = block.MeasuredWidth;

            // We'll define the chunk as being One element in size
            cChunkSize = 1;

            // If it is glued to the next one, then add the next one's measurements
            for (int k = i; k < Blocks.Length - 1; k++)
            {
                // Is this one glued to the next one? 
                if (!block.GlueToNext)
                    break;

                // We got glue, so Measure and add
                EBlock blockNext = Blocks[k + 1] as EBlock;
                float fWidthNext = blockNext.MeasuredWidth;
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

                // The X position depends upon the paragraph alignment. Note that
                // the first line has to also allow for the paragraph's FirstLineIndent
                float x = ptPos.X;           // Default: For Left and Justified
                if (ln == Lines[0])
                    x += (float)PStyle.FirstLineIndent * g.DpiX;
                if (PStyle.IsRight)
                    x += (xMaxWidth - ln.MeasuredWidth);
                if (PStyle.IsCentered)
                    x += (xMaxWidth - ln.MeasuredWidth) / 2;

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
        #region Method: void DoLayout(PointF ptPos, float xMaxWidth)
        public void DoLayout(PointF ptPos, float xMaxWidth)
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
            Graphics g = Window.Draw.Graphics;

            // Remember the top-left position and paragraph width
            Position = ptPos;
            MeasuredWidth = xMaxWidth;

            // Decrease the width by the paragraph margins
            xMaxWidth -= (float)PStyle.LeftMargin * g.DpiX;
            xMaxWidth -= (float)PStyle.RightMargin * g.DpiX;

            // We'll use "y" to work through the height of the paragraph. The initial
            // value is what was passed in.
            float y = ptPos.Y;

            // Add any SpaceBefore. This comes to us in Points, so we divide by 72 to
            // get Inches, then multiply by the screen's DPI to get pixels.
            float ySpaceBefore = (((float)PStyle.SpaceBefore) * g.DpiY / 72.0F);
            ySpaceBefore *= G.ZoomFactor;
            y += ySpaceBefore;

            // A "chunk" is the word or words that we'll add incrementally to the line.
            // We can't just add one word at a time because some items will be glued
            // to others. Thus the final word in a phrase might be glued to a footnote
            // letter, which might be glued to another footnote letter. We define all of
            // that as a "chunk".
            int cChunkSize = 1;

            // We'll add to x until we get to xMaxWidth, to know how much can fit on a line.
            // For this first x, we need to allow for the paragraph's FirstLineIndent
            float x = (float)PStyle.FirstLineIndent * g.DpiX;
 
            // We'll build the lines here
            ClearLines();
            Line line = new Line();
            AddLine(line);

            // Loop through all the blocks, adding them into lines
            for (int i = 0; i < Blocks.Length; i += cChunkSize)
            {
                // If we have a chapter, we treat if separately, since it takes up two lines.
                EChapter chapter = Blocks[i] as EChapter;
                if (null != chapter)
                {
                    // If we have contents in the current line, then we need to start a new
                    // line, so the chapter occurs at the left margin.
                    if (line.Blocks.Length > 0)
                    {
                        line = new Line();
                        AddLine(line);
                    }

                    line.Chapter = chapter;
                    x = chapter.MeasuredWidth;
                    line.LeftIndent = chapter.MeasuredWidth;
                    continue;
                }

                // If we have a Verse, we first determine if it needs extra leading space.
                // Refer to the DOC above.
                if (i > 0 && null != Blocks[i] as EVerse)
                {
                    (Blocks[i] as EVerse).NeedsExtraLeadingSpace = true;
                    (Blocks[i] as EVerse).MeasureWidth(g);
                }

                // Measure the next "chunk" we want to add (this may be more than one EBlock
                // due to glue.)
                float fWidth = Layout_CalcNextChunkWidth(g, i, out cChunkSize);
                Debug.Assert(cChunkSize >= 1);

                // Will the Chunk fit on the line? If not, start a new line
                if (x + fWidth > xMaxWidth)
                {
                    // If we're working on a chapter, then both this line and the next line will 
                    // need to reflect the indentation
                    float fIndentLine = (null == line.Chapter) ? 0 : line.Chapter.MeasuredWidth;

                    // Create the new line, and put this appropriate indentation to it
                    line = new Line();
                    AddLine(line);
                    line.LeftIndent = fIndentLine;

                    // Reset x so we can work through this line
                    x = fIndentLine;

                    // Since we're at the beginning of a line, extra leading space is not needed
                    // for a verse number. Refer to the DOC above.
                    if (null != Blocks[i] as EVerse)
                    {
                        (Blocks[i] as EVerse).NeedsExtraLeadingSpace = false;
                        (Blocks[i] as EVerse).MeasureWidth(g);
                        fWidth = Layout_CalcNextChunkWidth(g, i, out cChunkSize);
                    }
                }

                // Add the chunk(s) to the line
                for(int k=0; k<cChunkSize; k++)
                    line.AddBlock( Blocks[i + k] as EBlock );
                x += fWidth;
            }

            // Finally, we need to loop and actually assign Screen Coordinates to these objects,
            // now that we've broken them down into lines.
            float xLeft = ptPos.X + (float)PStyle.LeftMargin * g.DpiX;
            Layout_SetCoordinates(g, new PointF(xLeft, y), xMaxWidth);

            // Add any SpaceBefore and Space-After to the Height. 
            // Convert from Points (See SpaceBefore above.)
            Height += ySpaceBefore;
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
            foreach (EBlock block in Blocks)
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

            // Rework the paragraph: words on each line, justification, etc., etc.
            DoLayout(Position, MeasuredWidth);

            // If the height did not change, then all we need to do is redraw the paragraph.
            if (Height == fHeightGoingIn)
            {
                Window.Draw.Invalidate(Rectangle);
                return;
            }

            // Otherwise, we need to shift things up/down. So we call the Window and tell
            // it refigure everything starting with the owning Row
            Window.OnParagraphHeightChanged(Row);
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region Attr{g}: Color EditableBackgroundColor - shows the user where typing is permitted
        Color EditableBackgroundColor
        {
            get
            {
                return m_EditableBackgroundColor;
            }
        }
        Color m_EditableBackgroundColor = Color.White;
        #endregion
        #region void Paint(Rectangle ClipRectangle)
        public void Paint(Rectangle ClipRectangle)
        {
            // See if this paragraph needs to be painted
            if (!ClipRectangle.IntersectsWith(IntRectangle))
                return;

            // Paint the contents
            foreach (EBlock block in Blocks)
                block.Paint();
        }
        #endregion

        // Selection -------------------------------------------------------------------------
        #region Select_ and ExtendSelection_
        #region Method: EBlock GetBlockAt(PointF pt)
        public EBlock GetBlockAt(PointF pt)
        {
            // Don't waste time if we're not in this paragraph
            if (!ContainsPoint(pt))
                return null;

            // For Efficiency, let the Lines handle it
            foreach (Line ln in Lines)
            {
                EBlock block = ln.GetBlockAt(pt);
                if (null != block)
                    return block;
            }

            return null;
        }
        #endregion

        #region Method: bool Select_BeginningOfFirstWord()
        public bool Select_BeginningOfFirstWord()
        {
            if (!Editable)
                return false;

            for (int i = 0; i < Blocks.Length; i++)
            {
                if (null != Blocks[i] as EWord)
                {
                    Window.Selection = new OWWindow.Sel(this, i, 0);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Method: bool Select_NextWord(int iFirstCandidate)
        public bool Select_NextWord(int iFirstCandidate)
        {
            if (!Editable)
                return false;

            for (int i = iFirstCandidate; i < Blocks.Length; i++)
            {
                if (null != Blocks[i] as EWord)
                {
                    Window.Selection = new OWWindow.Sel(this, i, 0);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Method: bool Select_EndOfLastWord()
        public bool Select_EndOfLastWord()
        {
            if (!Editable)
                return false;

            for (int i = Blocks.Length - 1; i >= 0; i--)
            {
                if (null != Blocks[i] as EWord)
                {
                    Window.Selection = new OWWindow.Sel(this, i, Blocks[i].Text.Length);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Method: bool Select_PreviousWord(int iFirstCandidate)
        public bool Select_PreviousWord(int iFirstCandidate)
        {
            if (!Editable)
                return false;

            for (int i = iFirstCandidate; i >= 0; i--)
            {
                if (null != Blocks[i] as EWord)
                {
                    Window.Selection = new OWWindow.Sel(this, i, Blocks[i].Text.Length);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Method: void Select_LineBegin()
        public void Select_LineBegin()
        {
            if (!Editable)
                return;

            foreach (Line ln in Lines)
            {
                if (ln.Contains(Window.Selection.Anchor.Word))
                {
                    foreach (EBlock block in ln.Blocks)
                    {
                        if (null != block as EWord)
                        {
                            Window.Selection = new OWWindow.Sel(this, 
                                new OWWindow.Sel.SelPoint(IndexOfBlock(block), 0));
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
            if (!Editable)
                return;

            foreach (Line ln in Lines)
            {
                if (ln.Contains(Window.Selection.Anchor.Word))
                {
                    for(int iBlock = ln.Blocks.Length - 1; iBlock >= 0; iBlock--)
                    {
                        EWord word = ln.Blocks[iBlock] as EWord;
                        if (null != word)
                        {
                            int iChar = word.Text.Length;
                            Window.Selection = new OWWindow.Sel(this, 
                                new OWWindow.Sel.SelPoint(IndexOfBlock(word), iChar));
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region Method: void ExtendSelection_CharRight()
        public void ExtendSelection_CharRight()
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Move one to the right if possible
            if (iChar < pt.Word.Text.Length)
                iChar++;

            // Move over into the next word if necessary (and if possible)
            if (iChar == pt.Word.Text.Length)
            {
                if (iBlock < Blocks.Length - 1 && null != Blocks[iBlock+1] as EWord)
                {
                    iBlock++;
                    iChar = 0;
                }
            }

            // Create the new, updated selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_CharLeft()
        public void ExtendSelection_CharLeft()
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // If we are at a word boundary, then move to the end of the preceeding word
            if (iChar == 0)
            {
                if (iBlock > 0 && null != Blocks[iBlock - 1] as EWord)
                {
                    iBlock--;
                    iChar = Blocks[iBlock].Text.Length;
                }
            }

            // Move one to the left if possible
            if (iChar > 0)
                iChar--;

            // Create the new, updated selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_Line(bool bEnd)
        public void ExtendSelection_Line(bool bEnd)
            // bEnd - T if extending to the End of the line, 
            //        F if extending to the beginning
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;

            // Locate the Line this block is on
            Line line = LineContainingBlock(pt.Word);
            Debug.Assert(null != line);
            int iBlockInLine = line.IndexOfBlock(pt.Word);
            Debug.Assert(-1 != iBlockInLine);

            // Move to the furthest left/right block possible
            if (bEnd)
            {
                while (iBlockInLine < line.Blocks.Length - 1 &&
                    line.Blocks[iBlockInLine + 1] as EWord != null)
                {
                    ++iBlockInLine;
                }
            }
            else
            {
                while (iBlockInLine > 0 &&
                    line.Blocks[iBlockInLine - 1] as EWord != null)
                {
                    --iBlockInLine;
                }
            }

            // Get the index of this block in the Paragraph's Blocks attr
            // (as opposed to the Line's Blocks attr
            int iBlock = IndexOfBlock(line.Blocks[iBlockInLine]);

            // The position within the text is the first/last possible position
            int iChar = (bEnd) ? line.Blocks[iBlockInLine].Text.Length : 0;

            // Create the selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_LineUpDown(bool bDown, float x)
        public void ExtendSelection_LineUpDown(bool bDown, float x)
        {
            if (!Editable)
                return;

            // Determine which SelPoint to work from
            OWWindow.Sel.SelPoint sp = (Window.Selection.IsInsertionPoint) ?
                Window.Selection.Anchor :
                Window.Selection.End;

            // Get the next line down; abort if no-can-do
            Line lineCurrent = LineContainingBlock(sp.Word);
            int iLineCurrent = IndexOfLine(lineCurrent);
            int iLineTarget = (bDown) ? iLineCurrent + 1 : iLineCurrent - 1;
            if (iLineTarget == Lines.Length || iLineTarget < 0)
                return;
            Line lineTarget = Lines[iLineTarget];
            PointF pt = new PointF(x, lineTarget.Position.Y);

            // Loop through the blocks on this line, 
            //    bDown == T: from left-to-right, 
            //    bDown == F: from right-to-left
            // to extend  the selection as far as is possible before 
            // encountering a non-EWord.
            int iStart = (bDown) ? 0 : lineTarget.Blocks.Length - 1;
            int iEnd = (bDown) ? lineTarget.Blocks.Length : -1;
            int i = iStart;
            while (i != iEnd)
            {
                // If we run into a block that is not an EWord, then we can go no further.
                EWord word = lineTarget.Blocks[i] as EWord;
                if (null == word)
                    return;
                int iBlock = word.PositionWithinPara;

                // If the word contains "x", then we are happily sucessfully done.
                if (word.ContainsPoint(pt))
                {
                    int iChar = word.GetCharacterIndex(pt);
                    Window.Selection = new OWWindow.Sel(this, 
                        Window.Selection.Anchor,
                        new OWWindow.Sel.SelPoint(iBlock, iChar));
                    return;
                }

                // Otherwise, set the selection to the end of this word and loop to
                // see if the next one works for us.
                Window.Selection = new OWWindow.Sel(this, 
                    Window.Selection.Anchor,
                    new OWWindow.Sel.SelPoint(iBlock, ((bDown) ? word.Text.Length : 0)));

                // Increment to the next block
                i += (bDown) ? 1 : -1;
            }
        }
        #endregion
        #region Method: void ExtendSelection_WordRight()
        public void ExtendSelection_WordRight()
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;
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
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_WordLeft()
        public void ExtendSelection_WordLeft()
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // If at the beginning of a word, then move to the previous word
            if (iChar == 0 && pt.Word.IsBesideEWord(false))
            {
                iBlock--;
            }
            iChar = 0;

            // Create the new, updated selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_FarRight()
        public void ExtendSelection_FarRight()
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Go as far to the right as possible
            while (iBlock < Blocks.Length - 1 && Blocks[iBlock + 1] as EWord != null)
                ++iBlock;
            iChar = Blocks[iBlock].Text.Length;

            // Create the new, updated selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion
        #region Method: void ExtendSelection_FarLeft()
        public void ExtendSelection_FarLeft()
        {
            if (!Editable)
                return;

            // Get the point we are working from
            OWWindow.Sel.SelPoint pt = Window.Selection.Anchor;
            if (!Window.Selection.IsInsertionPoint)
                pt = Window.Selection.End;
            int iBlock = pt.iBlock;
            int iChar = pt.iChar;

            // Go as far to the left as possible
            while (iBlock > 0 && Blocks[iBlock - 1] as EWord != null)
                --iBlock;
            iChar = 0;

            // Create the new, updated selection
            Window.Selection = new OWWindow.Sel(this, 
                Window.Selection.Anchor,
                new OWWindow.Sel.SelPoint(iBlock, iChar));
        }
        #endregion

        #region Method: void NormalizeSelection()
        public void NormalizeSelection()
        {
            OWWindow.Sel sel = Window.Selection;
            if (null == sel)
                return;
            if (sel.IsInsertionIcon)
                return;

            // If we are on an Insertion Point at the end of the word,
            // then move to the beginning of the next word if possible
            if (sel.IsInsertionPoint)
            {
                if (sel.Anchor.iChar == sel.Anchor.Word.Text.Length &&
                    sel.Anchor.Word.IsBesideEWord(true))
                {
                    Select_NextWord(sel.Anchor.iBlock + 1);
                }
                return;
            }

            // If a Selection, then move the End accordingly
            if (null != sel.End && sel.End.iChar == sel.End.Word.Text.Length)
                ExtendSelection_WordRight();
        }
        #endregion
        #endregion

        // Edit Operations -------------------------------------------------------------------
        public enum DeleteMode { kDelete, kCut, kCopy, kBackSpace };
        #region Method: void Delete(DeleteMode) - All-purpose deletion
        public void Delete(DeleteMode mode)
        {
            // Do nothing if the paragraph is uneditable (we shouldn't be able
            // to get a selection here in the first place!)
            if (!Editable)
                return;

            // Setup based on the DeleteMode
            switch (mode)
            {
                case DeleteMode.kDelete:
                    // Delete: create a "content" selection forward
                    if (Window.Selection.IsInsertionPoint)
                        ExtendSelection_CharRight();
                    // Do nothing if an insertion icon
                    if (Window.Selection.IsInsertionIcon)
                        return;
                    break;

                case DeleteMode.kCut:
                    // Cut: Must have a "content" selection
                    if (Window.Selection.IsInsertionPoint || Window.Selection.IsInsertionIcon)
                        return;
                    break;

                case DeleteMode.kCopy:
                    // Copy: Must have a "content" selection
                    if (Window.Selection.IsInsertionPoint || Window.Selection.IsInsertionIcon)
                        return;
                    break;

                case DeleteMode.kBackSpace:
                    // Delete: create a "content" selection backward
                    if (Window.Selection.IsInsertionPoint)
                        ExtendSelection_CharLeft();
                    // Do nothing if an insertion icon
                    if (Window.Selection.IsInsertionIcon)
                        return;
                    break;
            }

            // Shorthand
            OWWindow.Sel sel = Window.Selection;
            if (null == sel)
                return;

            // Can't delete unless we have a valid selection that spans at least one character
            if (!sel.IsContentSelection)
            {
                OWWindow.TypingErrorBeep();
                return;
            }

            // Retrieve which phrase we'll be working on (Vernacular or Back Translation)
            DBasicText DBT = sel.DBT; 
            DBasicText.DPhrases phrases = (DisplayBT) ? DBT.PhrasesBT : DBT.Phrases;

            // Get the iBlocks that correspond to the DBasicText that contains the selection
            int iBlockFirst = sel.DBT_iBlockFirst;

            // Get the offsets into the DBasicText of the selection
            int n1 = sel.DBT_iCharFirst;
            int n2 = sel.DBT_iCharLast;
            int cCharsToDelete = n2 - n1;
            int nRestorePosition = n1;

            // If the deletion would result in two whitespaces together, then we
            // will need to delete an additional character. (We don't do this with 
            // the Copy command, because we want the Clipboard to reflect exactly
            // what was copied.
            string sDBT = (DisplayBT) ? DBT.ProseBTAsString : DBT.AsString;
            if (mode != DeleteMode.kCopy && 
                n1 > 0 && WritingSystem.IsWhiteSpace(sDBT[n1-1]) &&
                n2 < sDBT.Length && WritingSystem.IsWhiteSpace(sDBT[n2]))
            {
                cCharsToDelete++;
                n1--;
                if (mode == DeleteMode.kBackSpace)   // Backspace moves left, Delete, cut, etc move right
                    nRestorePosition--;
            }

            // We'll keep track of which DPhrase we're currently processing here
            int iPhrase = 0;

            // Increment past any DPhrases that are prior to the deletion point
            DPhrase phr = null;
            while (true)
            {
                phr = phrases[iPhrase];

                if (phr.Text.Length > n1)     // Originally '>=', changed 17oct07 for bug when Backspace to right of Italics
                    break;

                n1 -= phr.Text.Length;
                iPhrase++;
            }

            // We'll build a string of the characters that are getting deleted; so we can
            // place them on the clipboard if requested.
            string sClipboard = "";

            // Delete the requested number of characters, moving into following DPhrases
            // as needed.
            while (cCharsToDelete > 0)
            {
                phr = phrases[iPhrase];

                int cCanDelete = Math.Min(phr.Text.Length - n1, cCharsToDelete);

                if (cCanDelete > 0)
                {
                    sClipboard += phr.Text.Substring(n1, cCanDelete);

                    if (mode != DeleteMode.kCopy)
                        phr.Text = phr.Text.Remove(n1, cCanDelete);

                    cCharsToDelete -= cCanDelete;
                }
                else
                    iPhrase++;
            }

            // Update the clipboard if appropriate. If we were doing a Copy, then
            // we are finished.
            if (mode == DeleteMode.kCut || mode == DeleteMode.kCopy)
                Clipboard.SetText(sClipboard, TextDataFormat.UnicodeText);
            if (mode == DeleteMode.kCopy)
                return;

            // Remove the old blocks and replace with new ones
            RemoveBlocksAt(sel.DBT_iBlockFirst, sel.DBT_BlockCount);
            EWord[] vWords = ParseBasicTextIntoWords(DBT, null);
            foreach (EWord w in vWords)
                w.MeasureWidth(Window.Draw.Graphics);
            InsertBlocks(iBlockFirst, vWords);
            ReLayout();

            // Restore a selection at the deletion point
            Window.Selection = OWWindow.Sel.CreateSel(this, DBT, nRestorePosition); 
            NormalizeSelection();
        }
        #endregion
        #region Method: void Insert(sInsert) - All-purpose insertion
        public void Insert(string sInsert)
        {
            // Do nothing if the paragraph is uneditable (we shouldn't be able
            // to get a selection here in the first place!)
            if (!Editable)
                return;

            // If we have an InsertionIcon, replace it with a space, so that the normal
            // mechanism can deal with it (deleting the space, then doing the insertion.)
            if (Window.Selection.IsInsertionIcon)
            {
                Window.Selection.Anchor.Word.Text = " ";
                Window.Selection = new OWWindow.Sel(Window.Selection.Paragraph,
                    Window.Selection.Anchor);
            }

            // If we have a selection, we must delete the data that is there
            if (Window.Selection.IsContentSelection)
                Delete(DeleteMode.kDelete);

            // Get the iBlocks that correspond to the DBasicText
            OWWindow.Sel sel = Window.Selection;
            int iBlockFirst = sel.DBT_iBlockFirst;

            // Get the offset into the DBasicText
            int n = Window.Selection.DBT_iCharFirst;

            // Retrieve which phrase we'll be working on (Vernacular or Back Translation)
            DBasicText DBT = sel.DBT;
            DBasicText.DPhrases phrases = (DisplayBT) ? DBT.PhrasesBT : DBT.Phrases;

            // We'll keep track of which DPhrase we're currently processing here
            int iPhrase = 0;

            // Increment past any DPhrases that are prior to the insertion point
            DPhrase phr = null;
            while (true)
            {
                phr = phrases[iPhrase];

                if (phr.Text.Length >= n)
                    break;

                n -= phr.Text.Length;
                iPhrase++;
            }

            // We now have the DPhrase insertion point
            DPhrase phrase = phrases[iPhrase];
            int iPos = n;

            // Insert the text. Note that the phrase.Text removes spurious spaces, so if the result
            // of the insertion is that we have what we started with, then we need proceed no further.
            string sBeforeInsert = phrase.Text;
            phrase.Text = phrase.Text.Insert(n, sInsert);
            if (sBeforeInsert == phrase.Text)
            {
                // The test is for a cursor right before a space, e.g., "Hello| World." In this case,
                // we want the cursor to advance, as if the user hit right arrow, thus "Hello |World."
                // So for all other cases, we return here and do nothing, but in this one special
                // case, we want to continue through the method so that the cursor advances.
                if (!(n < phrase.Text.Length && phrase.Text[n] == ' ' && sInsert == " "))
                    return;
            }

            // Remove the old blocks and replace with new ones
            RemoveBlocksAt(sel.DBT_iBlockFirst, sel.DBT_BlockCount);
            EWord[] vWords = ParseBasicTextIntoWords(DBT, null);
            foreach (EWord w in vWords)
                w.MeasureWidth(Window.Draw.Graphics);
            InsertBlocks(iBlockFirst, vWords);
            ReLayout();

            // Restore a selection at the deletion point plus the amount inserted. 
            int iInsertPos = sel.DBT_iCharFirst + sInsert.Length;
            sel = OWWindow.Sel.CreateSel(this, sel.DBT, iInsertPos);
            Debug.Assert(null != sel);
            Window.Selection = sel;
            NormalizeSelection();
        }
        #endregion
    }
}
