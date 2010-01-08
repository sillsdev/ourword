using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData.DataModel;

namespace OurWord.Printing
{
    public class Printer
    {
        // Private attrs ---------------------------------------------------------------------
        #region Attr{g}: List<Page> Pages
        List<Page> Pages
        {
            get
            {
                Debug.Assert(null != m_Pages);
                return m_Pages;
            }
        }
        private readonly List<Page> m_Pages;
        #endregion
        #region Attr{g}: DSection CurrentSection
        DSection CurrentSection
        {
            get
            {
                return m_CurrentSection;
            }
        }
        private readonly DSection m_CurrentSection;
        #endregion
        #region Attr{g}: DBook BookToPrint
        DBook BookToPrint
        {
            get
            {
                return m_BookToPrint;
            }
        }
        private readonly DBook m_BookToPrint;
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Constructor(DSection)
        public Printer(DSection currentSectionOfBookToPrint)
        {
            m_Pages = new List<Page>();

            m_CurrentSection = currentSectionOfBookToPrint;
            m_BookToPrint = CurrentSection.Book;
        }
        #endregion
        #region Method: void Do()
        public void Do()
        {
            if (null == BookToPrint)
                return;

            // Determine how the user wishes to print
            var pdoc = new PrintDocument();
            var userSettings = new DialogPrint(pdoc);
            if (userSettings.ShowDialog() != DialogResult.OK)
                return;

            // Print Document Settings
            pdoc.DocumentName = BookToPrint.DisplayName;
            pdoc.PrinterSettings.PrinterName = userSettings.PrinterName;
            // Disable the default "Print Progress" dialog that would otherwise appear
            pdoc.PrintController = new StandardPrintController();
            
            // Layout & send to printer
            Layout(userSettings, pdoc);
            DoPrint(pdoc);
        }
        #endregion

        // Layout ----------------------------------------------------------------------------
        #region Method: void Layout(DialogPrint userSettings, PrintDocument pdoc)
        void Layout(DialogPrint userSettings, PrintDocument pdoc)
        {
            // Create OWParas for the body and footnotes we intend to print
            var vDataParagraphs = GetParagraphsToPrint(userSettings);
            var vDisplayParagraphs = GetDisplayParagraphsToPrint(vDataParagraphs);
            var vDisplayFootnotes = CollectAndNumberFootnotes(vDisplayParagraphs);

            // Get them properly measured and laid out according to their styles
            LayoutDisplayLines(vDisplayParagraphs, pdoc);
            LayoutDisplayLines(vDisplayFootnotes, pdoc);

            // A body line will appear on a page with all of its associated footnotes
            var vLineGroups = AssociateBodyWithFootnotes(pdoc, 
                vDisplayParagraphs, vDisplayFootnotes);
            SetScriptureReferences(vLineGroups);

            // Create the pages based on the heights that will fit
            while(vLineGroups.Count > 0)
                Pages.Add(new Page(pdoc, Pages.Count, vLineGroups));
        }
        #endregion
        #region Method: List<DParagraph> GetParagraphsToPrint(userSettings)
        List<DParagraph> GetParagraphsToPrint(DialogPrint userSettings)
        {
            var vParagraphs = new List<DParagraph>();

            var vSectionsToPrint = DetermineSectionsToPrint(userSettings);
            if (null == vSectionsToPrint || vSectionsToPrint.Count == 0)
                return vParagraphs;

            foreach (var section in vSectionsToPrint)
            {
                foreach (DParagraph paragraph in section.Paragraphs)
                    vParagraphs.Add(paragraph);
            }

            return vParagraphs;
        }
        #endregion
        #region Method: List<OWPara> GetDisplayParagraphsToPrint(vDParagraphs)
        List<OWPara> GetDisplayParagraphsToPrint(IEnumerable<DParagraph> vParagraphs)
        {
            var vDisplayParagraphs = new List<OWPara>();

            var writingSystem = BookToPrint.Translation.WritingSystemVernacular;

            foreach (var paragraph in vParagraphs)
            {
                var owp = new OWPara(writingSystem, paragraph.Style,
                    paragraph, Color.White, OWPara.Flags.None);
                vDisplayParagraphs.Add(owp);
                owp.SetPicture(WLayout.GetPicture(paragraph), false);
            }

            return vDisplayParagraphs;
        }
        #endregion
        #region SMethod: List<OWPara> CollectAndNumberFootnotes(IEnumerable<OWPara> vDisplayParagraphs)
        static List<OWPara> CollectAndNumberFootnotes(IEnumerable<OWPara> vDisplayParagraphs)
        {
            var n = 0;

            var vFootnotes = new List<OWPara>();

            foreach (var paragraph in vDisplayParagraphs)
            {
                foreach (var item in paragraph.SubItems)
                {
                    var footLetter = item as OWPara.EFoot;
                    if (null == footLetter)
                        continue;

                    var footnote = footLetter.Footnote;
                    var sLetter = footnote.Foot.GetFootnoteLetterFor(n++);
                    footLetter.Text = sLetter;

                    var owfn = new OWFootnotePara(footnote, Color.White, OWPara.Flags.None);
                    vFootnotes.Add(owfn);

                    var label = owfn.SubItems[0] as OWPara.EFootnoteLabel;
                    if (null != label)
                        label.Text = sLetter;
                }
            }

            return vFootnotes;
        }
        #endregion
        #region SMethod: void LayoutDisplayLines(vDisplayParagraphs, pdoc)
        static void LayoutDisplayLines(List<OWPara> vDisplayParagraphs, PrintDocument pdoc)
        {
            // Place the paragraphs into a root container
            var root = new ERoot(null, new PrintContext(pdoc));
            root.Append(vDisplayParagraphs.ToArray());

            // Lay them out
            root.CalculateBlockWidths();
            root.DoLayout();
        }
        #endregion
        #region SMethod: List<AssociatedLines> AssociateBodyWithFootnotes(vDisplayParagraphs, vDisplayFootnotes)
        static List<AssociatedLines> AssociateBodyWithFootnotes(
            PrintDocument pdoc,
            IEnumerable<OWPara> vDisplayParagraphs, 
            IEnumerable<OWPara> vDisplayFootnotes)
            // Handles widow/orphan control, by grouping Body Lines together that would otherwise
            // potentially form an orphan.
        {
            var vGroups = new List<AssociatedLines>();

            foreach(var owp in vDisplayParagraphs)
            {
                // Shorthand
//                var fSpaceBefore = owp.Context.PointsToDeviceY(owp.PStyle.SpaceBefore);
//                var fSpaceAfter = owp.Context.PointsToDeviceY(owp.PStyle.SpaceAfter);
                var count = owp.Lines.Count;

                // Widow/orphan control: small paragraphs always go together
                if (count <= 3)
                {
                    vGroups.Add( new AssociatedLines(owp.Lines, vDisplayFootnotes)
                        {
                            TopY = owp.Rectangle.Top,
                            BottomY = owp.Rectangle.Bottom,
                            IsParagraphContinuation = false,
                            Picture = owp.Picture
                        });
                    continue;
                }

                // The first two lines are a group
                vGroups.Add( new AssociatedLines( owp.Lines.GetRange(0, 2), vDisplayFootnotes)
                    {
                        TopY = owp.Rectangle.Top,
                        BottomY = owp.Lines[2].Rectangle.Bottom,
                        IsParagraphContinuation = false,
                        Picture = owp.Picture
                    });

                // The middle lines are individually separate
                for (var i = 2; i < count - 2; i++)
                    vGroups.Add(new AssociatedLines(owp.Lines[i], vDisplayFootnotes) 
                        {
                            TopY = owp.Lines[i].Rectangle.Top,
                            BottomY = owp.Lines[i].Rectangle.Bottom,
                            IsParagraphContinuation = true,
                        });

                // The final two lines are a group
                vGroups.Add( new AssociatedLines(owp.Lines.GetRange(count - 2, 2), vDisplayFootnotes)
                    {
                        TopY = owp.Lines[count - 2].Rectangle.Top,
                        BottomY = owp.Rectangle.Bottom,
                        IsParagraphContinuation = true,
                    });
            }

            return vGroups;
        }
        #endregion

        #region SMethod: void SetScriptureReferences(vGroups)
        static void SetScriptureReferences(IEnumerable<AssociatedLines> vGroups)
        {
            var nChapter = 1;
            var nVerse = 1;

            foreach(var group in vGroups)
                group.SetScriptureReferences(ref nChapter, ref nVerse);
        }
        #endregion

        // Implementation --------------------------------------------------------------------
        #region Method: List<DSection> DetermineSectionsToPrint(DialogPrint userSettings)
        List<DSection> DetermineSectionsToPrint(DialogPrint userSettings)
        {
            if (userSettings.CurrentSection)
                return new List<DSection> {CurrentSection};

            if (userSettings.EntireBook)
            {
                var v = new List<DSection>();
                foreach(DSection section in BookToPrint.Sections)
                    v.Add(section);
                return v;
            }

            if (userSettings.Chapters)
            {
                var nStartAtChapter = userSettings.StartChapter;
                var nEndAtChapter = Math.Max(userSettings.EndChapter, nStartAtChapter);

                var v = new List<DSection>();
                foreach(DSection section in BookToPrint.Sections)
                {
                    if (section.ReferenceSpan.End.Chapter >= nStartAtChapter)
                        v.Add(section);
                    if (section.ReferenceSpan.Start.Chapter > nEndAtChapter)
                        break;
                }
                return v;
            }

            Debug.Assert(false);
            return null;
        }
        #endregion
        #region Method: void DoPrint(PrintDocument pdoc)
        void DoPrint(PrintDocument pdoc)
        {
            try
            {
                pdoc.PrintPage += PrintPage;
                pdoc.Print();
            }
            catch (Exception e)
            {
                LocDB.Message("msgPrintFailed",
                    "Printing failed with operating system message:\n\n{0}.",
                    new[] { e.Message },
                    LocDB.MessageTypes.Error);
            }
        }
        #endregion
        #region Handler: void PrintPage(sender, PrintPageEventArgs)
        private void PrintPage(object sender, PrintPageEventArgs ev)
		{
            // Take the page off the beginning of the list and draw it
		    var page = Pages[0];
            page.Draw(new PrinterDraw(ev.Graphics));

            // Still more to do?
		    Pages.RemoveAt(0);
            ev.HasMorePages = (Pages.Count > 0);
        }
        #endregion
    }
}
