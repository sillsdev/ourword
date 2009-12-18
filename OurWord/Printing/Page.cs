using System;
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
        #region Attr{g}: List<ELine> BodyLines
        List<ELine> BodyLines
        {
            get
            {
                Debug.Assert(null != m_BodyLines);
                return m_BodyLines;
            }
        }
        private readonly List<ELine> m_BodyLines;
        #endregion
        #region Attr{g}: List<ELine> FootnoteLines
        List<ELine> FootnoteLines
        {
            get
            {
                Debug.Assert(null != m_FootnoteLines);
                return m_FootnoteLines;
            }
        }
        private readonly List<ELine> m_FootnoteLines;
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

        #region Constructor(pDoc, nPageNumber, vGroups)
        public Page(PrintDocument pdoc, int nPageNumber, IList<AssociatedLines> vLineGroups)
        {
            m_BodyLines = new List<ELine>();
            m_FootnoteLines = new List<ELine>();

            m_PageSettings = pdoc.PrinterSettings.DefaultPageSettings;

            m_fTotalAvailableContentHeight = m_PageSettings.Bounds.Height
                - m_PageSettings.Margins.Top - m_PageSettings.Margins.Bottom;

            // Setup and measure the running footer
            m_RunningFooter = new RunningFooter(nPageNumber, vLineGroups[0].Reference);
            RunningFooter.Layout(pdoc);

            // Extract from the source list the groups which will fit on this page
            var vGroupsThisPage = GetGroupsThatWillFit(vLineGroups);

            // Place them on the page
            LayoutBody(vGroupsThisPage);
            LayoutFootnotes(vGroupsThisPage);
        }
        #endregion

        // Layout (done during construction) -------------------------------------------------
        #region Method: List<AssociatedLines> GetGroupsThatWillFit(vSourceGroups)
        List<AssociatedLines> GetGroupsThatWillFit(IList<AssociatedLines> vSourceGroups)
        {
            var vGroupsThisPage = new List<AssociatedLines>();

            var fHeightRemaining = m_fTotalAvailableContentHeight;

            // Make sure the page has at least one group, even if it is too large
            var firstGroup = vSourceGroups[0];
            vGroupsThisPage.Add(firstGroup);
            vSourceGroups.Remove(firstGroup);
            fHeightRemaining -= firstGroup.TotalHeight;

            // Transfer as many other groups over as will fit
            while (vSourceGroups.Count > 0)
            {
                var group = vSourceGroups[0];

                if (group.TotalHeight > fHeightRemaining)
                    break;

                vSourceGroups.Remove(group);
                vGroupsThisPage.Add(group);
                fHeightRemaining -= group.TotalHeight;
            }

            return vGroupsThisPage;
        }
        #endregion
        #region Method: void LayoutBody(IList<AssociatedLines> vGroups)
        void LayoutBody(IList<AssociatedLines> vGroups)
        {
            float fTop = m_PageSettings.Bounds.Top + m_PageSettings.Margins.Top;

            if (vGroups.Count == 0 || vGroups[0].BodyLines.Count == 0)
                return;
            var firstLine = vGroups[0].BodyLines[0];
            var fAdjust = firstLine.Position.Y - fTop;

            foreach(var group in vGroups)
            {
                foreach(var line in group.BodyLines)
                {
                    foreach(var item in line.SubItems)
                        item.Position = new PointF(item.Position.X, item.Position.Y - fAdjust);

                    BodyLines.Add(line);
                }
            }
        }
        #endregion
        #region Method: void LayoutFootnotes(IEnumerable<AssociatedLines> vGroups)
        void LayoutFootnotes(IEnumerable<AssociatedLines> vGroups)
        {
            float fBottom = m_PageSettings.Bounds.Bottom + m_PageSettings.Margins.Bottom;

            foreach(var group in vGroups)
            {
                foreach(var line in group.FootnoteLines)
                {
                    var fLineHeight = 0F;
                    foreach (var item in line.SubItems)
                        fLineHeight = Math.Max(fLineHeight, item.Height);

                    fBottom -= fLineHeight;

                    foreach (var item in line.SubItems)
                        item.Position = new PointF(item.Position.X, fBottom);
                    
                    FootnoteLines.Add(line);
                }
            }
        }
        #endregion

        public void Draw(Graphics g)
        {
            var allLines = new List<ELine>();
            allLines.AddRange(BodyLines);
            allLines.AddRange(FootnoteLines);

            foreach (var line in allLines)
            {
                foreach (EBlock block in line.SubItems)
                    block.Print(g);
            }
        }
    }
}
