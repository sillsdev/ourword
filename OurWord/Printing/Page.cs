#region ***** Page.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Page.cs
 * Author:  John Wimbish
 * Created: 20 Dec 2009
 * Purpose: A single printed page
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using OurWord.Edit;
#endregion

namespace OurWord.Printing
{
    public class Page
    {
        // Major Page Components -------------------------------------------------------------
        #region Attr{g}: List<AssociatedLines> Groups
        List<AssociatedLines> Groups
        {
            get
            {
                Debug.Assert(null != m_vGroups);
                return m_vGroups;
            }
        }
        private readonly List<AssociatedLines> m_vGroups;
        #endregion

        #region SAttr{g}: PageSettings PageSettings
        static public PageSettings PageSettings
        {
            get
            {
                return DialogPrint.s_PageSettings;
            }
        }
        #endregion

        private readonly RunningFooter m_runningFooter;

        private readonly float m_fTotalAvailableContentHeight;
        public string WaterMarkText { private get; set; }
        private readonly bool m_bAllowPicturesToFloatOnPage;

        private const int c_nMarginBetweenBodyAndFootnotes = 10;
        private const int c_nMarginAboveRunningFooter = 10;

        #region Constructor(pDoc, nPageNumber, vGroups)
        public Page(PrintDocument pdoc, int nPageNumber, 
            IList<AssociatedLines> vSourceGroups,
            string sRunningFooterText,
            bool bAllowPicturesToFloatOnPage)
        {
            m_bAllowPicturesToFloatOnPage = bAllowPicturesToFloatOnPage;

            m_fTotalAvailableContentHeight = PageSettings.Bounds.Height
                - PageSettings.Margins.Top - PageSettings.Margins.Bottom;

            // Setup and measure the running footer
            m_runningFooter = new RunningFooter(nPageNumber, 
                vSourceGroups[0].ScriptureReference,  
                pdoc,
                sRunningFooterText);
            m_fTotalAvailableContentHeight -= m_runningFooter.Height;
            m_fTotalAvailableContentHeight -= c_nMarginAboveRunningFooter;

            // Extract from the source list the groups which will fit on this page
            m_vGroups = new List<AssociatedLines>();
            CalculateGroupsThatWillFit(vSourceGroups);
            Layout();
        }
        #endregion

        // Layout (done during construction) -------------------------------------------------
        #region Method: void CalculateGroupsThatWillFit(vSourceGroups)
        #region Method: AssociatedLines GetNextGroup(fHeightRemaining, vSourceGroups)
        AssociatedLines GetNextGroup(float fHeightRemaining, IList<AssociatedLines> vSourceGroups)
        {
            if (vSourceGroups.Count == 0)
                return null;

            // Return the next group if it will fit
            var nextGroup = vSourceGroups[0];
            var printableHeight = nextGroup.TotalHeight - nextGroup.SpaceAfter;
            if (printableHeight <= fHeightRemaining)
                return nextGroup;

            // If the next group is a picture, see if we can slide the following group
            // ahead of it.
            if (nextGroup.HasPicture && vSourceGroups.Count > 1 && m_bAllowPicturesToFloatOnPage)
            {
                var followingGroup = vSourceGroups[1];
                printableHeight = followingGroup.TotalHeight - followingGroup.SpaceAfter;
                if (printableHeight <= fHeightRemaining && !followingGroup.HasPicture)
                    return followingGroup;
            }

            return null;
        }
        #endregion

        void CalculateGroupsThatWillFit(IList<AssociatedLines> vSourceGroups)
        {
            var fHeightRemaining = m_fTotalAvailableContentHeight;
            var bFootnoteFound = false;

            // We want to always do one, whether it fits or not; otherwise we might have
            // an infinite loop.
            var group = vSourceGroups[0];

            do
            {
                Groups.Add(group);

                // If we see footnotes, we need to allow room between body and footnote
                if (group.HasFootnotes && !bFootnoteFound)
                {
                    fHeightRemaining -= c_nMarginBetweenBodyAndFootnotes;
                    bFootnoteFound = true;
                }

                // Subtract this group's height from the height available on the page.
                // For the initial group on the page, we want to eat the SpaceBefore, as we don't
                // need for the paragraph to appear lower when its at the top of the page.
                var fGroupHeight = (Groups.Count == 1) ? 
                    group.TotalHeight - group.SpaceBefore : 
                    group.TotalHeight;
                fHeightRemaining -= fGroupHeight;

                // Remove the group we've just added from the queue
                vSourceGroups.Remove(group);

                // If the next group will not fit, then the next page will have to pick it up.
                group = GetNextGroup(fHeightRemaining, vSourceGroups);

            } while (null != group);

        }
        #endregion
        #region Attr{g}: float HeightRequiredForFootnotes
        float HeightRequiredForFootnotes
        {
            get
            {
                var fHeight = 0.0F;

                foreach (var group in Groups)
                    fHeight += group.FootnotesHeight;

                return fHeight;
            }
        }
        #endregion
        #region Method: void Layout()
        void Layout()
        {
            // 1. BODY LINES
            float yTopBody = PageSettings.Bounds.Top + PageSettings.Margins.Top;

            // Since we're at the top of the page, we eat the SpaceBefore
            var firstGroup = Groups[0];
            yTopBody -= firstGroup.SpaceBefore;

            foreach(var group in Groups)
            {
                group.SetYs(yTopBody);
                yTopBody += group.BodyHeight;
            }

            // 2. FOOTNOTE LINES
            var yPrintAreaBottom = PageSettings.Bounds.Bottom - PageSettings.Margins.Bottom;
            var yTopFootnotes = yPrintAreaBottom - 
                m_runningFooter.Height -
                c_nMarginAboveRunningFooter -
                HeightRequiredForFootnotes;

            foreach(var group in Groups)
            {
                foreach(var line in group.FootnoteLines)
                {
                    line.SetYs(yTopFootnotes);
                    yTopFootnotes += line.LargestItemHeight;
                }
            }
        }
        #endregion

        // Drawing ---------------------------------------------------------------------------
        #region method: int GetWaterMarkFontSize(IDraw, fTextWidthMax)
        int GetWaterMarkFontSize(IDraw draw, float fTextWidthMax)
        {
            // Start small, and work up until we're too big
            var nSize = 19;

            do
            {
                var font = new Font("Arial", nSize + 1, FontStyle.Bold);
                var fWidth = draw.Graphics.MeasureString(WaterMarkText, font).Width;
                if (fWidth > fTextWidthMax)
                    return nSize;
                nSize++;
            } while (true);
        }
        #endregion
        #region method: void DrawWatermark(IDraw)
        void DrawWatermark(IDraw draw)
        {
            // Don't proceed if the water mark isn't wanted
            if (string.IsNullOrEmpty(WaterMarkText))
                return;

            // Text Angle
            const float c_fAngle = 30;
            const double c_fRadians = c_fAngle * Math.PI / 180.0;

            // Print Areas
            var nPrintAreaWidth = PageSettings.Bounds.Width -
                PageSettings.Margins.Left - PageSettings.Margins.Right;
            var nPrintAreaHeight = PageSettings.Bounds.Height -
                PageSettings.Margins.Top - PageSettings.Margins.Bottom;

            // Center
            var xCenter = PageSettings.Margins.Left + nPrintAreaWidth/2;
            var yCenter = PageSettings.Margins.Top + nPrintAreaHeight/2;

            // Calculate the maximum length this text should appear (non-rotated).
            var fTextWidthMax = nPrintAreaWidth / (float)Math.Cos(c_fRadians);
            fTextWidthMax *= 0.90F; // Fudge down so we're sure to fit the margins.

            // Get the Font
            var nFontSize = GetWaterMarkFontSize(draw, fTextWidthMax);
            var font = new Font("Arial", nFontSize, FontStyle.Bold);
            var szWaterMarkText = draw.Graphics.MeasureString(WaterMarkText, font);

            // Brush: We want a very light gray
            const int c_nGray = 225;
            Brush brush = new SolidBrush(Color.FromArgb(c_nGray, c_nGray, c_nGray));

            // Draw the text
            draw.Graphics.TranslateTransform(xCenter, yCenter);
            draw.Graphics.RotateTransform(-c_fAngle);
            draw.Graphics.DrawString(WaterMarkText, font, brush, 
                -szWaterMarkText.Width/2, -szWaterMarkText.Height/2);
            draw.Graphics.ResetTransform();
        }
        #endregion
        #region Method: void DrawFootnoteSeparator(IDraw)
        void DrawFootnoteSeparator(IDraw draw)
        {
            if(HeightRequiredForFootnotes == 0)
                return;

            const int c_yPixelsAboveFootnotes = 2;
            var x = PageSettings.Margins.Left;
            var y = PageSettings.Bounds.Top + 
                PageSettings.Margins.Top +
                m_fTotalAvailableContentHeight -
                HeightRequiredForFootnotes - 
                c_yPixelsAboveFootnotes;

            const int c_xSeparatorLength = 100;

            draw.DrawLine(Pens.Black, x, y, x + c_xSeparatorLength, y);
        }
        #endregion
        #region Method: void Draw(IDraw)
        public void Draw(IDraw draw)
        {
            DrawWatermark(draw);
            DrawFootnoteSeparator(draw);

            foreach (var group in Groups)
                group.Draw(draw);

            m_runningFooter.Draw(draw);
        }
        #endregion
    }
}
