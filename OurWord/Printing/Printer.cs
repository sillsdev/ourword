using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
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
        #region Constructor(DBook)
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
            Layout(userSettings);
            DoPrint(pdoc);
        }
        #endregion

        // Layout ----------------------------------------------------------------------------
        void Layout(DialogPrint userSettings)
        {
            var vDataParagraphs = GetParagraphsToPrint(userSettings);
            var vDisplayParagraphs = GetDisplayParagraphsToPrint(vDataParagraphs);
            var vDisplayLines = LayoutDisplayLines(vDisplayParagraphs);
            
        }
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
            }

            return vDisplayParagraphs;
        }
        #endregion

        List<ELine> LayoutDisplayLines(List<OWPara> vDisplayParagraphs)
        {
            // Place the paragraphs into a root container
 //           var root = new ERoot();



            var vLines = new List<ELine>();

            return vLines;
        }




        void LayoutThePages(PrintDocument pdoc, DialogPrint userSettings)
        {
            var vSectionsToPrint = DetermineSectionsToPrint(userSettings);
            if (null == vSectionsToPrint || vSectionsToPrint.Count == 0)
                return;

            // We'll create containers for measuring the paragraphs and the footnotes
            var drawingEnvironment = new DrawEnvironment()
            {
                Graphics = pdoc.PrinterSettings.CreateMeasurementGraphics()
            };
            var rootParagraphs = new ERoot(null) {Drawing = drawingEnvironment};
            var rootFootnotes = new ERoot(null) {Drawing = drawingEnvironment};

            // Create OWPara's for everything we want to measure
            var writingSystem = BookToPrint.Translation.WritingSystemVernacular;
            foreach (var section in vSectionsToPrint)
            {
                foreach (DParagraph paragraph in section.Paragraphs)
                {
                    var owp = new OWPara(writingSystem, paragraph.Style,
                        paragraph, Color.White, OWPara.Flags.None);
                    rootParagraphs.Append(owp);
                }
                foreach (var footnote in section.AllFootnotes)
                {
                    var owp = new OWPara(writingSystem, footnote.Style,
                        footnote, Color.White, OWPara.Flags.None);
                    rootFootnotes.Append(owp);
                }
            }

            // Have each paragraph do its layout. This takes care of space before/after,
            // margins, etc, as well as splitting the paragraph into lines.



            // Create the initial page
  //          var page = AddPage(vSectionsToPrint[0].ReferenceSpan.Start);

            // Add lines to the page

        }

        // Implementation --------------------------------------------------------------------
        #region Method: Page AddPage(DReference chapterAndVerse)
        Page AddPage(DReference chapterAndVerse)
        {
            // Compute the page number for the new page. The first one is page "1".
            var nPageNumber = Pages.Count + 1;

            var page = new Page(nPageNumber, chapterAndVerse);
            Pages.Add(page);
            return page;
        }
        #endregion
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
                pdoc.PrintPage += new PrintPageEventHandler(PrintPage);
                pdoc.Print();
            }
            catch (Exception e)
            {
                LocDB.Message("msgPrintFailed",
                    "Printing failed with Windows message:\n\n{0}.",
                    new string[] { e.Message },
                    LocDB.MessageTypes.Error);
            }
        }
        #endregion
        #region Handler: void PrintPage(sender, PrintPageEventArgs)
        private void PrintPage(object sender, PrintPageEventArgs ev)
		{
            // Take the page off the beginning of the list and draw it
		    var page = Pages[0];
		    page.Draw(ev.Graphics);

            // Still more to do?
		    Pages.RemoveAt(0);
            ev.HasMorePages = (Pages.Count > 0);
        }
        #endregion
    }
}
