#region ***** EFootnoteLabel.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EFootnoteLabel.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: The reference text beside a footnote
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWordData.DataModel;
using OurWordData.Styles;
#endregion

namespace OurWord.Edit.Blocks
{
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
        readonly DFootnote m_Footnote;
        #endregion

        #region Constructor(font, DFootLetter)
        public EFootnoteLabel(Font font, DFootnote footnote)
            : base(font, footnote.Letter + " ")
        {
            m_Footnote = footnote;
        }
        #endregion
        #region oattr{g}: Color TextColor
        protected override Color TextColor
        {
            get
            {
                return StyleSheet.FootnoteLetter.FontColor;
            }
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
                var para = Window.Contents.FindParagraph(Footnote);
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

}
