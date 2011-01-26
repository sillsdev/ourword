#region ***** EChapter.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EChapter.cs
 * Author:  John Wimbish
 * Created: 02 Sep 2009 (separate file 19 Apr 2010)
 * Purpose: A chapter number as displayed in a paragraph
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using OurWordData.DataModel;
using OurWordData.Styles;
#endregion

namespace OurWord.Edit.Blocks
{
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
            protected set
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
        }
        #endregion
        #region oattr{g}: Color TextColor
        protected override Color TextColor
        {
            get
            {
                return StyleSheet.ChapterNumber.FontColor;
            }
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
}
