#region ***** EBlock.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EBlock.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: An individual word in a paragraph
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace OurWord.Edit.Blocks
{
    public class EBlock : EItem
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
        #region Attr{g}: bool GlueToNext
        //  T if this block must be beside the next one
        public virtual bool GlueToNext { get; set; }
        #endregion

        // Screen Region -----------------------------------------------------------------
        #region OAttr{g}: float Height
        override public float Height
        {
            get
            {
                // we use the paragraph's line height, as some individual elements could
                // be using different fonts.
                return Para.LineHeight;
            }
            protected set
            {
                Debug.Assert(false, "Can't set the line height of an EBlock");
            }
        }
        #endregion
        #region OMethod: bool ContainsPoint(PointF pt)
        public override bool ContainsPoint(PointF pt)
        {
            var r = new RectangleF(Position,
                new SizeF(Width + JustificationPaddingAdded, Height));
            return r.Contains(pt);
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(PointF)
        public override EBlock GetBlockAt(PointF pt)
        {
            return this;
        }
        #endregion
        #region Attr{g/s}: int JustificationPaddingAdded
        public int JustificationPaddingAdded { get; set; }
        #endregion

        // Scaffolding -------------------------------------------------------------------
        #region VAttr{g}: OWPara Para
        public OWPara Para
        // Keep a pointer back to the owner so we can get, e.g., the PStyle
        {
            get
            {
                var para = Owner as OWPara;
                Debug.Assert(null != para);
                return para;
            }
        }
        #endregion
        #region VAttr{g}: int PositionWithinPara
        public int PositionWithinPara
        {
            get
            {
                for (int i = 0; i < Para.SubItems.Length; i++)
                {
                    if (Para.SubItems[i] as EBlock == this)
                        return i;
                }
                return -1;
            }
        }
        #endregion
        #region Constructor(Font, sText)
        protected EBlock(Font font, string sText)
        {
            GlueToNext = false;
            JustificationPaddingAdded = 0;
            m_sText = sText;
            m_Font = font;
        }
        #endregion

        // Painting ----------------------------------------------------------------------
        protected readonly Font m_Font;
        #region Attr{g/s}: Color TextColor
        protected Color TextColor
        {
            get
            {
                return m_TextColor;
            }
            set
            {
                m_TextColor = value;
            }
        }
        private Color m_TextColor = Color.Black;
        #endregion
        #region Method: Brush GetBrush()
        protected Brush GetBrush()
        {
            return new SolidBrush(TextColor);
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

        // Tooltips
        #region VirtMethod: void LaunchToolTip()
        public virtual void LaunchToolTip()
        {
            // Those subclasses which suppport tooltips will launch here
        }
        #endregion
        #region Attr{g}: HasTooltip
        virtual public bool HasToolTip()
        {
            // Return false if no tooltip is desired
            return false;
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region VirtMethod: void CalculateWidth()
        virtual public void CalculateWidth()
        // Those subclasses which override will not need to call this base method.
        {
            Width = Context.Measure(Text, m_Font);
        }
        #endregion
    }
}
