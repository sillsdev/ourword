#region ***** ELabel.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    ELabel.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: An individual phrase in a paragraph, uneditable
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Drawing;
using OurWordData.DataModel;
#endregion

namespace OurWord.Edit.Blocks
{
    public class ELabel : EBlock
    {
        private const string c_spaces = "\u00A0";

        #region Constructor(font, DLabel)
        public ELabel(Font font, DLabel label)
            : base(font, label.Text + c_spaces)
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
}
