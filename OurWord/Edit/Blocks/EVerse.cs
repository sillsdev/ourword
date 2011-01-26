#region ***** EVerse.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EVerse.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: A verse number in a paragraph
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWord.ToolTips;
using OurWordData.DataModel;
using OurWordData.Styles;
#endregion

namespace OurWord.Edit.Blocks
{
    public class EVerse : EBlock
    {
        // Attrs -----------------------------------------------------------------------------
        const string c_sLeadingSpace = "  ";
        #region Attr{g/s}: bool NeedsExtraLeadingSpace
        // T if some extra leading padding is required.
        public bool NeedsExtraLeadingSpace { private get; set; }
        #endregion
        #region VAttr{g}: int Number
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
        #region Attr{g}: DVerse Verse
        public DVerse Verse
        {
            get
            {
                Debug.Assert(null != m_Verse);
                return m_Verse;
            }
        }
        readonly DVerse m_Verse;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(font, DVerse)
        public EVerse(Font font, DVerse verse)
            : base(font, verse.Text)
        {
            m_Verse = verse;
            NeedsExtraLeadingSpace = false;
        }
        #endregion
        #region oattr{g}: Color TextColor
        protected override Color TextColor
        {
            get
            {
                return StyleSheet.VerseNumber.FontColor;
            }
        }
        #endregion

        // Layout and Drawing ----------------------------------------------------------------
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

        // Tooltip ---------------------------------------------------------------------------
        #region OAttr{g}: Cursor MouseOverCursor
        public override Cursor MouseOverCursor
        {
            get
            {
                return (HasToolTip() ? Cursors.Hand : base.MouseOverCursor);
            }
        }
        #endregion
        #region Attr{g}: bool HasToolTip()
        public override bool HasToolTip()
            // Whether or not we have a tooltip depends on if there are Reference translations
            // for this translation.
        {
            // We only do tooltips for the target translation
            var book = Verse.RootOwner as DBook;
            if (null == book || !book.IsTargetBook)
                return false;

            // There must be reference translations defined for this project
            var project = book.Project;
            return (project.OtherTranslations.Count > 0);
        }
        #endregion 
        #region OMethod: void LaunchToolTip()
        public override void LaunchToolTip()
        {
            var y = (int)(Middle.Y - Window.ScrollBarPosition);
            var ptScreenLocation = Window.PointToScreen(new Point(Middle.X, y));

            var owp = Owner as OWPara;
            if (null == owp)
                return;

            var tip = new ReferenceTranslationsTip(Verse, owp.DisplayBT);
            tip.Launch(ptScreenLocation);
        }
        #endregion
        #region OMethod: void cmdLeftMouseClick(PointF)
        public override void cmdLeftMouseClick(PointF pt)
        {
            ToolTipLauncher.LaunchNow(this);
        }
        #endregion
    }
}
