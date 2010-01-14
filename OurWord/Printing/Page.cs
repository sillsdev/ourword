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
        #region Attr{g}: RunningFooter RunningFooter
        RunningFooter RunningFooter
        {
            get
            {
                Debug.Assert(null != m_RunningFooter);
                return m_RunningFooter;
            }
        }
        private readonly RunningFooter m_RunningFooter;
        #endregion

        private readonly PageSettings m_PageSettings;
        private readonly float m_fTotalAvailableContentHeight;
        public string WaterMarkText { private get; set; }

        private const int c_nMarginBetweenBodyAndFootnotes = 10;

        #region Constructor(pDoc, nPageNumber, vGroups)
        public Page(PrintDocument pdoc, int nPageNumber, IList<AssociatedLines> vSourceGroups)
        {
            m_PageSettings = pdoc.PrinterSettings.DefaultPageSettings;

            m_fTotalAvailableContentHeight = m_PageSettings.Bounds.Height
                - m_PageSettings.Margins.Top - m_PageSettings.Margins.Bottom;

            // Setup and measure the running footer
            m_RunningFooter = new RunningFooter(nPageNumber, vSourceGroups[0].ScriptureReference,
                new PrintContext(pdoc));
            RunningFooter.Layout(pdoc);

            // Extract from the source list the groups which will fit on this page
            m_vGroups = new List<AssociatedLines>();
            CalculateGroupsThatWillFit(vSourceGroups);
            Layout();
        }
        #endregion

        // Layout (done during construction) -------------------------------------------------
        #region Method: void CalculateGroupsThatWillFit(vSourceGroups)
        void CalculateGroupsThatWillFit(IList<AssociatedLines> vSourceGroups)
        {
            var fHeightRemaining = m_fTotalAvailableContentHeight;
            var bFootnoteFound = false;

            do
            {
                // Add the next group: by doing it first in the do loop we ensure that we 
                // always add at least one group, even if too high; thus avoiding an endless loop
                var group = vSourceGroups[0];
                Groups.Add(group);

                // If we see footnotes, we need to allow room between body and footnote
                if (group.HasFootnotes && !bFootnoteFound)
                {
                    fHeightRemaining -= c_nMarginBetweenBodyAndFootnotes;
                    bFootnoteFound = true;
                }

                // Subtract this group's height from the height available on the page.6
                // For the initial group on the page, we want to eat the SpaceBefore, as we don't
                // need for the paragraph to appear lower when its at the top of the page.
                var fGroupHeight = (Groups.Count == 1) ? 
                    (group.TotalHeight - group.SpaceBefore) : 
                    group.TotalHeight;
                fHeightRemaining -= fGroupHeight;

                // Remove the group we've just added from the queue
                vSourceGroups.Remove(group);
                if (vSourceGroups.Count == 0)
                    break;

                // If the next group will not fit, then the next page will have to pick it up.
                var nextGroup = vSourceGroups[0];
                var nextGroupHeightIfAtPageBottom = nextGroup.TotalHeight - nextGroup.SpaceAfter;
                if (fHeightRemaining < nextGroupHeightIfAtPageBottom)
                    break;

            } while (true);

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
            float yPrintAreaTop = m_PageSettings.Bounds.Top + m_PageSettings.Margins.Top;
            var yFirstGroupTopOfText = Groups[0].TopY + Groups[0].SpaceBefore;
            var fBodyAdjustment = yPrintAreaTop - yFirstGroupTopOfText;

            float yPrintAreaBottom = m_PageSettings.Bounds.Bottom - m_PageSettings.Margins.Bottom;
            var yFootnoteTop = yPrintAreaBottom - HeightRequiredForFootnotes;
          
            foreach(var group in Groups)
            {
                group.MoveYs(fBodyAdjustment);

                foreach (var line in group.FootnoteLines)
                {
                    foreach (var item in line.SubItems)
                        item.Position = new PointF(item.Position.X, yFootnoteTop);
                    yFootnoteTop += line.LargestItemHeight;
                }
            }
        }
        #endregion

        // Drawing ---------------------------------------------------------------------------
        #region Method: void DrawWatermark(IDraw)
        void DrawWatermark(IDraw draw)
        {
            // Don't proceed if the water mark isn't wanted
            if (string.IsNullOrEmpty(WaterMarkText))
                return;

            // Calculate the maximum length this text should appear (non-rotated). We'll
            // assume a 30-degree angle. 
            const float fAngle = 30;
            const double fRadians = fAngle * Math.PI / 180.0;
            var nPrintAreaWidth = m_PageSettings.PaperSize.Width - m_PageSettings.Margins.Left -
                               m_PageSettings.Margins.Right;
            var fTextWidthMax = nPrintAreaWidth / (float)Math.Cos(fRadians);
            fTextWidthMax *= 0.90F; // Fudge down so we're sure to fit the margins.

            // Find the largest font size that does not exceed this width
            float fSize = 20;
            var font = new Font("Arial", fSize, FontStyle.Bold);
            do
            {
                var fWidth = draw.Graphics.MeasureString(WaterMarkText, font).Width;
                if (fWidth > fTextWidthMax)
                    break;

                fSize += 1;
                font = new Font("Arial", fSize, FontStyle.Bold);
            } while (true);

            // We want a very light gray
            const int nGray = 225;
            Brush brush = new SolidBrush(Color.FromArgb(nGray, nGray, nGray));

            // Figure out where to put the text
            float x = m_PageSettings.Margins.Left;
            var szText = draw.Graphics.MeasureString(WaterMarkText, font);

            var yPageMiddle = (m_PageSettings.Bounds.Height -
                m_PageSettings.Margins.Top - 
                m_PageSettings.Margins.Bottom) / 2F;

            var fHeightDueToFont = szText.Height * (float) Math.Cos(fRadians) / 2F;
            var fHeightDueToRise = szText.Width * (float) Math.Sin(fRadians) / 2F;

            var y = yPageMiddle + fHeightDueToRise + fHeightDueToFont;

            // Draw the text
            draw.Graphics.TranslateTransform(x, y);
            draw.Graphics.RotateTransform(-fAngle);
            draw.Graphics.DrawString(WaterMarkText, font, brush, 0, 0);
            draw.Graphics.ResetTransform();
        }
        #endregion
        #region Method: void DrawFootnoteSeparator(IDraw draw)
        void DrawFootnoteSeparator(IDraw draw)
        {
            if(HeightRequiredForFootnotes == 0)
                return;

            const int yPixelsAboveFootnotes = 2;
            var x = m_PageSettings.Margins.Left;
            var y = m_PageSettings.Bounds.Bottom - 
                m_PageSettings.Margins.Bottom -
                HeightRequiredForFootnotes - 
                yPixelsAboveFootnotes;

            const int xSeparatorLength = 100;

            draw.DrawLine(Pens.Black, x, y, x + xSeparatorLength, y);
        }
        #endregion
        #region Method: void Draw(IDraw draw)
        public void Draw(IDraw draw)
        {
            DrawWatermark(draw);
            DrawFootnoteSeparator(draw);

            foreach (var group in Groups)
                group.Draw(draw);
        }
        #endregion
    }
}
