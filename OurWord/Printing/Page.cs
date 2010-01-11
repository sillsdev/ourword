﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using OurWord.Edit;

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

            // Make sure the page has at least one group, even if it is too large
            var firstGroup = vSourceGroups[0];
            Groups.Add(firstGroup);
            vSourceGroups.Remove(firstGroup);
            fHeightRemaining -= firstGroup.TotalHeight;

            // Transfer as many other groups over as will fit
            while (vSourceGroups.Count > 0)
            {
                var group = vSourceGroups[0];

                if (group.TotalHeight > fHeightRemaining)
                    break;

                fHeightRemaining -= group.TotalHeight;

                vSourceGroups.Remove(group);
                Groups.Add(group);
            }
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
            var yFirstGroupTop = Groups[0].TopY;
            var fBodyAdjustment = yPrintAreaTop - yFirstGroupTop;

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
            var fAngledHeight = szText.Width * (float)Math.Tan(fRadians);
            var fTextHeight = szText.Height * (float)Math.Cos(fRadians);
            var y = m_PageSettings.Margins.Top +
                (m_PageSettings.PaperSize.Height / 2) +
                (fAngledHeight / 2) -
                (fTextHeight / 2);

            // Draw the text
            draw.Graphics.TranslateTransform(x, y);
            draw.Graphics.RotateTransform(-fAngle);
            draw.Graphics.DrawString(WaterMarkText, font, brush, 0, 0);
            draw.Graphics.ResetTransform();
        }
        #endregion

        #region Method: void Draw(IDraw draw)
        public void Draw(IDraw draw)
        {
            DrawWatermark(draw);

            foreach(var group in Groups)
                group.Draw(draw);
        }
        #endregion
    }
}
