#region ***** ELine.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    ELine.cs
 * Author:  John Wimbish
 * Created: 16 Dec 2009
 * Purpose: An individual line as laid out for a paragraph
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
#endregion

namespace OurWord.Edit
{
    public class ELine : EContainer
    {
        // Content attrs ---------------------------------------------------------------------
        #region Attr{g/s}: EChapter Chapter - the EChapter prior to this line (or null if none there)
        public OWPara.EChapter Chapter
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
        OWPara.EChapter m_Chapter;
        #endregion
        #region Attr{g/s}: float LeftIndent - how much to indent the line for the chapter
        public float LeftIndent { private get; set; }
        #endregion

        // Virtual attrs ---------------------------------------------------------------------
        #region OAttr{g}: float Width
        override public float Width
        {
            get
            {
                float f = 0;
                foreach (var item in SubItems)
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
                return (Count == 0) ? 0 : SubItems[0].Height;
            }
            set
            {                
            }
        }
        #endregion
        #region Attr{g}: float LargestItemHeight
        public float LargestItemHeight
        {
            get
            {
                var fLargestItemHeight = 0F;
                foreach (var item in SubItems)
                    fLargestItemHeight = Math.Max(fLargestItemHeight, item.Height);
                return fLargestItemHeight;
            }
        }
        #endregion

        // Line numbers ----------------------------------------------------------------------
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
            var s = LineNo + " ";

            // Calculate the width of this number
            var fWidth = para.Context.Measure(s, window.LineNumberAttrs.Font);

            // The X coordinate is the x of the window (root) left, 
            var x = para.Root.Position.X;
            // plus the width allocated to columns
            x += window.LineNumberAttrs.ColumnWidth;
            // Less the space needed to draw this number
            x -= fWidth;

            // The Y coordinate is the y of the first block
            var y = SubItems[0].Position.Y;

            // Draw the line number
            window.Draw.DrawString(s, window.LineNumberAttrs.Font,
                window.LineNumberAttrs.Brush, new PointF(x, y));
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region OMethod: void Append(EItem item)
        public override void Append(EItem item)
            // Override in order to make sure the ELine doesn't become the owner, as until we
            // do a rewrite, the ELine isn't directly in the ownership hierarchy.
        {
            var container = item.Owner;
            base.Append(item);
            item.Owner = container;
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
            var fRawWidthOfMaterial = LeftIndent + Width;
            var cPixelsToJustify = (int)(xWidthToFill - fRawWidthOfMaterial);

            // Calculate how many positions we can add these pixels to
            var cJustificationPositions = Count - 1;
            foreach (EBlock block in SubItems)
            {
                if (block.GlueToNext)
                    cJustificationPositions--;
            }

            // Calculate how many pixels to add to each position
            var cPadding = 0;
            var cRemainder = 0;
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
                    var nPad = cPadding;

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
            // TranslatorNote. Thus,
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
            var block = GetBlockAt(pt);
            if (null != block as EWord)
            {
                var word = block as EWord;
                var iBlock = word.PositionWithinPara;
                var iChar = word.GetCharacterIndex(pt);
                block.Window.Selection = new OWWindow.Sel(word.Para,
                    new OWWindow.Sel.SelPoint(iBlock, iChar));
                return true;
            }

            // Not possible, so perhaps the line is too short, or perhaps the Block there
            // is a verse number or some other uneditable. So......
            // Examine all of the blocks in the line for the one which is closest to x
            float fDistance = 10000;
            EWord ClosestWord = null;
            var bBeginningIsClosest = true;
            foreach (EItem b in SubItems)
            {
                // Retrieve each EWord in the line
                var w = b as EWord;
                if (null == w)
                    continue;

                // Calculate its distance
                var d1 = Math.Abs(pt.X - w.Position.X);
                var d2 = Math.Abs(pt.X - (w.Position.X + w.Width));
                var d = Math.Min(d1, d2);

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
                var iBlock = ClosestWord.PositionWithinPara;
                var iChar = (bBeginningIsClosest) ? 0 : ClosestWord.Text.Length;
                ClosestWord.Window.Selection = new OWWindow.Sel(
                    ClosestWord.Para,
                    new OWWindow.Sel.SelPoint(iBlock, iChar));
                return true;
            }

            return false;
        }
        #endregion

        #region Method: void Draw(IDraw)
        public void Draw(IDraw draw)
        {
            if (null != Chapter)
                Chapter.Draw(draw);

            foreach (EBlock block in SubItems)
                block.Draw(draw);
        }
        #endregion
    }
}
