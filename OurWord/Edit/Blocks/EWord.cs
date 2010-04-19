#region ***** EWord.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EWord.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008
 * Purpose: An individual word in a paragraph
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWordData.DataModel.Runs;
#endregion

namespace OurWord.Edit.Blocks
{
    #region CLASS: EWord : EBlock
    public class EWord : EBlock
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DPhrase Phrase - the phrase from which this EWord was generated
        public DPhrase Phrase
        {
            get
            {
                Debug.Assert(null != m_Phrase);
                return m_Phrase;
            }
        }
        readonly DPhrase m_Phrase;
        #endregion

        // Hyphenation
        #region Attr{g/s}: bool Hyphenated
        public bool Hyphenated { get; set; }
        #endregion
        #region Attr{g/s}: float HyphenWidth
        private float HyphenWidth { get; set; }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(font, DPhrase, sText)
        public EWord(Font font, DPhrase phrase, string sText)
            : base(font, sText)
        {
            Debug.Assert(null != phrase);
            m_Phrase = phrase;
        }
        #endregion
        #region Method: EWord Clone()
        public virtual EWord Clone()
        {
            var word = new EWord(m_Font, Phrase, Text);
            return word;
        }
        #endregion
        #region static EWord CreateAsInsertionIcon(font, DPhrase)
        static public EWord CreateAsInsertionIcon(Font font, DPhrase phrase)
        {
            var word = new EWord(font, phrase, c_chInsertionSpace.ToString());
            return word;
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region Method: void PaintBackgroundRectangle(IDraw, Color color)
        void PaintBackgroundRectangle(IDraw draw, Color color)
        {
            var fWidthWithPadding = Width + JustificationPaddingAdded - HyphenWidth;
            var r = new RectangleF(Position, new SizeF(fWidthWithPadding, Height));
            draw.DrawBackground(color, r);
        }
        #endregion
        #region Method: override void Draw(IDraw)
        public override void Draw(IDraw draw)
        {
            // The white background
            if (!draw.IsSendingToPrinter)
            {
                var backgroundColor = (Para.IsEditable && !Para.IsLocked) ?
                    Para.EditableBackgroundColor : Para.NonEditableBackgroundColor;
                PaintBackgroundRectangle(draw, backgroundColor);
            }

            // The text
            if (IsInsertionIcon) 
                return;

            draw.DrawString(Text, m_Font, GetBrush(), Position);

            if (Hyphenated)
            {
                var ptHyphenPosition = new PointF(Position.X + Width - HyphenWidth, Position.Y);
                draw.DrawString("-", m_Font, GetBrush(), ptHyphenPosition);
            }
        }
        #endregion
        #region Method: void PaintSelection(IDraw, int iCharLeft, int iCharRight)
        public void PaintSelection(IDraw draw, int iCharLeft, int iCharRight)
        {
            // Create the colors and brushes we'll need
            var clrSelectedBackground = SystemColors.Highlight;
            var clrSelectedText = SystemColors.HighlightText;
            Brush brushSelectedText = new SolidBrush(clrSelectedText);
            Brush brushNormalText = new SolidBrush(TextColor);

            // Insertion Icon
            if (IsInsertionIcon)
            {
                PaintBackgroundRectangle(draw, clrSelectedBackground);
                draw.DrawString(G.GetLoc_String("TypeHere", "[Type Here]"),
                    m_Font, brushSelectedText, Position);
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
                PaintBackgroundRectangle(draw, clrSelectedBackground);
                draw.DrawString(Text, m_Font, brushSelectedText, Position);
                return;
            }

            // If the two parameters are still the same, then we have no selection to paint
            if (iCharLeft == iCharRight)
                return;

            // Figure out the selection texts
            var sLeft = (iCharLeft == 0) ? "" : Text.Substring(0, iCharLeft);
            var sTemp = (iCharLeft == 0) ? Text : Text.Substring(iCharLeft);
            var sSelected = (iCharRight == Text.Length) ? sTemp :
                sTemp.Substring(0, iCharRight - iCharLeft);
            var sRight = (iCharRight == Text.Length) ? "" :
                sTemp.Substring(iCharRight - iCharLeft);

            // Figure out the boundaries
            var fTotalWidth = Width + JustificationPaddingAdded;
            var xSelLeft = Position.X +
                ((iCharLeft == 0) ? 0 : Context.Measure(sLeft, m_Font));
            var xSelRight = xSelLeft + ((iCharRight == Text.Length) ?
                fTotalWidth - Context.Measure(sLeft, m_Font) :
                Context.Measure(sSelected, m_Font));

            // Paint the white background, for those portions that are not selected
            PaintBackgroundRectangle(draw,
                Para.IsLocked ? Para.NonEditableBackgroundColor : Para.EditableBackgroundColor);

            // Paint the selected background
            var rectSelected = new RectangleF(xSelLeft, Position.Y, xSelRight - xSelLeft, Height);
            draw.FillRectangle(clrSelectedBackground, rectSelected);

            // Paint the text
            if (sLeft.Length > 0)
                draw.DrawString(sLeft, m_Font, brushNormalText, Position);

            draw.DrawString(sSelected, m_Font, brushSelectedText,
                new PointF(xSelLeft, Position.Y));

            if (sRight.Length > 0)
            {
                draw.DrawString(sRight, m_Font, brushNormalText,
                    new PointF(xSelRight, Position.Y));
            }
        }
        #endregion
        #region Method: float GetXat(int i)
        public float GetXat(int i)
        {
            if (i == 0)
                return 0;

            return Context.Measure(Text.Substring(0, i), m_Font);
        }
        #endregion

        // Insertion Point ---------------------------------------------------------------
        public const char c_chInsertionSpace = '\u2004';   // Unicode's "Four-Per-EM space"
        #region Attr{g}: bool IsInsertionIcon
        public bool IsInsertionIcon
        {
            get
            {
                return (Text.Length == 1 && Text[0] == c_chInsertionSpace);
            }
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
                float x2 = Position.X + Context.Measure(sPortion, m_Font);

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
            var iBlock = Para.Find(this);

            EBlock blockBeside;

            if (bOnRight)
            {
                if (iBlock == Para.SubItems.Length - 1)
                    return false;
                blockBeside = Para.SubItems[iBlock + 1] as EBlock;
            }
            else
            {
                if (iBlock == 0)
                    return false;
                blockBeside = Para.SubItems[iBlock - 1] as EBlock;
            }

            return (null != blockBeside as EWord);
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

        // Command Processing ----------------------------------------------------------------
        #region Attr{g}: Cursor MouseOverCursor - Shape of cursor indicates what a Left-Click will do
        public override Cursor MouseOverCursor
        {
            get
            {
                if (!Para.IsEditable)
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
            if (!Para.IsEditable)
                return;

            // Find out the index of the character we've just clicked over
            var iChar = GetCharacterIndex(pt);

            // Get the number of this block within the paragraph
            var iBlock = PositionWithinPara;

            // For an insertion icon, just select the entire word
            if (IsInsertionIcon)
            {
                Para.Window.Selection = new OWWindow.Sel(Para,
                    new OWWindow.Sel.SelPoint(iBlock, 0),
                    new OWWindow.Sel.SelPoint(iBlock, Text.Length));
                return;
            }

            // Create the Selection
            var Anchor = new OWWindow.Sel.SelPoint(iBlock, iChar);
            var selection = new OWWindow.Sel(Para, Anchor);
            Para.Window.Selection = Para.NormalizeSelection(selection);
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
            if (!Para.IsEditable)
                return;

            // Is there anything between Here and the Anchor that is NOT an EWord?
            var iEnd = PositionWithinPara;
            var iAnchor = Window.Selection.Anchor.iBlock;
            var iFirst = (iAnchor < iEnd) ? iAnchor : iEnd;
            var iLast = (iEnd > iAnchor) ? iEnd : iAnchor;
            for (var i = iFirst; i < iLast; i++)
            {
                if (Para.SubItems[i] as EWord == null)
                    return;
            }
            var iChar = GetCharacterIndex(pt);

            // If this is the same position as our previous mouse move, then
            // do nothing. Seems that Windows keeps feeding these messages even when the
            // mouse doesn't move, and this prevents the screen from being repainted,
            // and thus the selection never shows up.
            if (null != Window.Selection.End && Window.Selection.End.iChar == iChar)
                return;

            // Passed the test; so extend the selection to this new End point
            var end = new OWWindow.Sel.SelPoint(iEnd, iChar);
            var selection = new OWWindow.Sel(Para, Window.Selection.Anchor, end);
            Window.Selection = Para.NormalizeSelection(selection);
        }
        #endregion
        #region Cmd: cmdLeftMouseDoubleClick
        public override void cmdLeftMouseDoubleClick(PointF pt)
        {
            // If the paragraph is not editable, then it is a moot point; no sel is possible
            if (!Para.IsEditable)
                return;

            // Get the number of this block within the paragraph
            int iBlock = PositionWithinPara;

            // Select the entire word
            Para.Window.Selection = new OWWindow.Sel(Para,
                new OWWindow.Sel.SelPoint(iBlock, 0),
                new OWWindow.Sel.SelPoint(iBlock, Text.Length));
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateWidth()
        override public void CalculateWidth()
        {
			HyphenWidth = 0;

            if (IsInsertionIcon)
            {
                Width = Context.Measure(G.GetLoc_String("TypeHere", "[Type Here]"), m_Font);
            }
            else if (Hyphenated)
            {
                Width = Context.Measure(Text, m_Font);

                HyphenWidth = Context.Measure("-", m_Font);
                Width += HyphenWidth;
            }
            else
            {
                Width = Context.Measure(Text, m_Font);
            }
        }
        #endregion
    }
    #endregion

}
