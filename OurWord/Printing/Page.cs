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

            // Lay them out on the page
            LayoutBody();
            LayoutFootnotes();
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

        float HeightRequiredForFootnotes
        {
            get
            {
                var fHeight = 0.0F;

                foreach (var group in Groups)
                    foreach (var line in group.FootnoteLines)
                        fHeight += line.LargestItemHeight;

                return fHeight;
            }
        }

        void Layout()
        {
            float yPrintAreaTop = m_PageSettings.Bounds.Top + m_PageSettings.Margins.Top;
            var yBodyTop = yPrintAreaTop;

            float yPrintAreaBottom = m_PageSettings.Bounds.Bottom - m_PageSettings.Margins.Bottom;
            var yFootnoteTop = yPrintAreaBottom - HeightRequiredForFootnotes;
          
            foreach(var group in Groups)
            {
                group.Layout(ref yBodyTop, ref yFootnoteTop);
            }
        }

        #region Method: void LayoutBody()
        void LayoutBody()
        {
            if (Groups.Count == 0 || Groups[0].BodyLines.Count == 0)
                return;

            float fTop = m_PageSettings.Bounds.Top + m_PageSettings.Margins.Top;
            var firstLine = Groups[0].BodyLines[0];
            var fAdjust = firstLine.Position.Y - fTop;

            foreach(var group in Groups)
                foreach(var line in group.BodyLines)
                    foreach(var item in line.SubItems)
                        item.Position = new PointF(item.Position.X, item.Position.Y - fAdjust);
        }
        #endregion
        #region Method: void LayoutFootnotes()
        void LayoutFootnotes()
        {
            // Calculate the top of the footnotes area
            float fBottom = m_PageSettings.Bounds.Bottom - m_PageSettings.Margins.Bottom;

            foreach (var group in Groups)
            {
                foreach (var line in group.FootnoteLines)
                    fBottom -= line.LargestItemHeight;
            }

            // Layout the footnotes
            foreach(var group in Groups)
            {
                foreach(var line in group.FootnoteLines)
                {
                    foreach (var item in line.SubItems)
                        item.Position = new PointF(item.Position.X, fBottom);

                    fBottom += line.LargestItemHeight;               
                }
            }
        }
        #endregion
        #region Method: void Draw(IDraw draw)
        public void Draw(IDraw draw)
        {
            foreach(var group in Groups)
                group.Draw(draw);
        }
        #endregion
    }
}
