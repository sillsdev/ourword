#region ***** EBigHeader.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EBigHeader.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: A large-font header for a paragraph
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Drawing;
using OurWordData.Styles;
#endregion

namespace OurWord.Edit.Blocks
{
    class EBigHeader : EBlock
    {
        #region Constructor(font, sText)
        public EBigHeader(Font font, string sText)
            : base(font, sText + " ")
        {
        }
        #endregion

        #region oattr{g}: Color TextColor
        protected override Color TextColor
        {
            get
            {
                return StyleSheet.BigHeader.FontColor;
            }
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
}
