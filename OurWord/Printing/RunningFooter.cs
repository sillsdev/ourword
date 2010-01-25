#region ***** RunningFooter *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    RunningFooter.cs
 * Author:  John Wimbish
 * Created: 20 Dec 2009
 * Purpose: The three-column footer at the bottom of each page
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using OurWord.Edit;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.Styles;

#endregion

namespace OurWord.Printing
{
    public class RunningFooter
    {
        private readonly string m_sLeftColumn;
        private readonly string m_sMiddleColumn;
        private readonly string m_sRightColumn;

        private readonly Font m_font;

        private readonly int m_nPageNumber;
        private readonly DReference m_refChapterAndVerse;
        private readonly IDrawingContext m_Context;

        private readonly float m_yFooter;
        private readonly float m_xFooter;
        private readonly float m_FooterWidth;

        private readonly string m_sRunningFooterText;

        #region Attr{g}: float Height
        public float Height
        {
            get
            {
                return m_font.Height;
            }
        }
        #endregion
        #region VAttr{g}: bool IsEvenPage
        public bool IsEvenPage
        {
            get
            {
                // Divide the page number in half, any remainder (e.g., as will be true
                // with an odd number) will be dropped
                var nHalf = m_nPageNumber/2;

                return nHalf*2 == m_nPageNumber;
            }
        }
        #endregion

        #region Method: string GetFooterPartString(kFooterPart)
        string GetFooterPartString(DTeamSettings.FooterParts kFooterPart)
        {
            switch (kFooterPart)
            {
                case DTeamSettings.FooterParts.kPageNumber:
                    return string.Format("- {0} -", m_nPageNumber); ;

                case DTeamSettings.FooterParts.kCopyrightNotice:
                    return DB.TargetBook.HasCopyrightNotice ?
                        DB.TargetBook.Copyright :
                        DB.TeamSettings.CopyrightNotice;

                case DTeamSettings.FooterParts.kScriptureReference:
                    return string.Format("{0} {1}:{2}",
                        m_sRunningFooterText,
                        m_refChapterAndVerse.Chapter,
                        m_refChapterAndVerse.Verse);

                case DTeamSettings.FooterParts.kStageAndDate:
                    return string.Format("{0} - {1}",
                        DB.TargetBook.Stage.LocalizedName, 
                        DateTime.Today.ToShortDateString());

                case DTeamSettings.FooterParts.kLanguageStageAndDate:
                    return string.Format("{0} {1} - {2}",
                        DB.TargetBook.Translation.DisplayName,
                        DB.TargetBook.Stage.LocalizedName,
                        DateTime.Today.ToShortDateString()); 

                default:
                    throw new NotImplementedException("Unknown RunningFooter Part.");
            }
        }
        #endregion

        #region Method: void Draw(IDraw)
        public void Draw(IDraw draw)
        {
            // Left side.
            draw.DrawString(m_sLeftColumn, m_font, Brushes.Black, 
                new PointF(m_xFooter, m_yFooter));

            // Middle
            var widthMid = m_Context.Measure(m_sMiddleColumn, m_font);
            var x = m_xFooter + m_FooterWidth/2 - (widthMid / 2);
            draw.DrawString(m_sMiddleColumn, m_font, Brushes.Black, 
                new PointF(x, m_yFooter));

            // Right side
            var fDateWidth = m_Context.Measure(m_sRightColumn, m_font);
            x = m_xFooter + m_FooterWidth - fDateWidth;
            draw.DrawString(m_sRightColumn, m_font, Brushes.Black, 
                new PointF(x, m_yFooter));
        }
        #endregion

        #region Constructor(nPageNumber, DReference, pdoc)
        public RunningFooter(int nPageNumber, DReference chapterAndVerse, 
            PrintDocument pdoc, string sRunningFooterText)
        {
            m_nPageNumber = nPageNumber;
            m_refChapterAndVerse = chapterAndVerse;
            m_Context = new PrintContext(pdoc);
            m_sRunningFooterText = sRunningFooterText;

            // Retrieve the font from the stylesheet (needed for Height calculation below)
            var ps = DB.StyleSheet.FindParagraphStyleOrNormal(DStyleSheet.c_sfmRunningHeader);
            m_font = ps.CharacterStyle.FindOrAddFontForWritingSystem(
                DB.TargetTranslation.WritingSystemVernacular).DefaultFont;

            // Layout
            m_FooterWidth = pdoc.DefaultPageSettings.Bounds.Width -
                            pdoc.DefaultPageSettings.Margins.Left -
                            pdoc.DefaultPageSettings.Margins.Right;
            m_xFooter = pdoc.DefaultPageSettings.Margins.Left;
            m_yFooter = pdoc.DefaultPageSettings.Bounds.Height -
                        pdoc.DefaultPageSettings.Margins.Bottom -
                        Height;

            // Text in each column depends on even/odd page
            m_sLeftColumn = GetFooterPartString( 
                IsEvenPage ? DB.TeamSettings.EvenLeft : DB.TeamSettings.OddLeft);
            m_sMiddleColumn = GetFooterPartString(
                IsEvenPage ? DB.TeamSettings.EvenMiddle : DB.TeamSettings.OddMiddle);
            m_sRightColumn = GetFooterPartString(
                IsEvenPage ? DB.TeamSettings.EvenRight : DB.TeamSettings.OddRight);
        }
        #endregion
    }

}
