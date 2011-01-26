#region ***** ELiteral.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    ELiteral.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: An individual word in a paragraph, but uneditable
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Drawing;
using System.Windows.Forms;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

#endregion

namespace OurWord.Edit.Blocks
{
    public class ELiteral : EWord
    // As a literal of EWord, hyphenation is possible.
    {
        #region Constructor(font, DPhrase, sText)
        public ELiteral(Font font, DPhrase phrase, string sText, Color textColor)
            : base(font, phrase, sText, textColor)
        {
        }
        #endregion
        #region OMethod: EWord Clone()
        public override EWord Clone()
        {
            return new ELiteral(m_Font, Phrase, Text, TextColor);
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
}
