#region ***** Printer.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Printer.cs
 * Author:  John Wimbish
 * Created: 20 Dec 2009
 * Purpose: Printing (direct to printer)
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
using OurWord.Edit.Blocks;
using OurWord.Layouts;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.Styles;

#endregion

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
        #region Attr{g}: DialogPrint UserSettings
        protected DialogPrint UserSettings
        {
            get
            {
                Debug.Assert(null != m_dlgUserSettings);
                return m_dlgUserSettings;
            }
        }
        private readonly DialogPrint m_dlgUserSettings;
        #endregion
        #region Attr{g}: PrintDocument PDoc
        PrintDocument PDoc
        {
            get
            {
                return UserSettings.PDoc;
            }
        }
        #endregion

        private string m_sRunningFooterText;

        // Public Interface ------------------------------------------------------------------
        #region Constructor(DSection)
        public Printer(DSection currentSectionOfBookToPrint)
            : this()
        {
            m_CurrentSection = currentSectionOfBookToPrint;
            m_BookToPrint = CurrentSection.Book;
        }
        #endregion
        #region Constructor() - for testing
        protected Printer()
        {
            var pdoc = new PrintDocument();
            m_dlgUserSettings = new DialogPrint(pdoc);

            m_Pages = new List<Page>();
        }
        #endregion
        #region Method: void Do()
        public void Do()
        {
            if (null == BookToPrint)
                return;

            // Determine how the user wishes to print
            if (UserSettings.ShowDialog() != DialogResult.OK)
                return;

            // Print Document Settings
            PDoc.DocumentName = BookToPrint.DisplayName;
            PDoc.PrinterSettings.PrinterName = UserSettings.PrinterName;
            // Disable the default "Print Progress" dialog that would otherwise appear
            PDoc.PrintController = new StandardPrintController();

            // Initialize the progress dialog
            var vsProgressSteps = new[] 
            { 
                "Measuring the text",
                "Laying out the pages",
                "Sending to the Printer"
            };
            EnumeratedStepsProgressDlg.Start("Printing", vsProgressSteps);
            
            // Layout & send to printer
            Layout();
            DoPrint();

            EnumeratedStepsProgressDlg.Stop();
        }
        #endregion

        // Substitutions ---------------------------------------------------------------------
        private TreeRoot m_ReplaceTree;
        #region Method: string MakeQuoteReplacements(s)
        protected string MakeQuoteReplacements(string s)
        {
            if (!UserSettings.MakeQuoteSubstitutions)
                return s;

            // Make sure the tree has been built
            if (null == m_ReplaceTree)
            {
                m_ReplaceTree = new TreeRoot();

                m_ReplaceTree.Add("<<<", "“‘");
                m_ReplaceTree.Add("<<", "“");
                m_ReplaceTree.Add("<", "‘");

                m_ReplaceTree.Add(">>>", "’”");
                m_ReplaceTree.Add(">>", "”");
                m_ReplaceTree.Add(">", "’");                
            }

            // Do the replacements
            return m_ReplaceTree.MakeReplacements(s);
        }
        #endregion
        #region Method: void MakeQuoteReplacements(EContainer owp)
        private void MakeQuoteReplacements(EContainer owp)
        {
            if (!UserSettings.MakeQuoteSubstitutions)
                return;

            foreach(var item in owp.SubItems)
            {
                var word = item as EWord;
                if (null != word)
                    word.Text = MakeQuoteReplacements(word.Text);
            }
        }
        #endregion

        // Layout ----------------------------------------------------------------------------
        #region Method: void Layout()
        void Layout()
        {
            // Begin step: "Measuring the text"
            EnumeratedStepsProgressDlg.IncrementStep();

            // Create OWParas for the body and footnotes we intend to print
            var vDataParagraphs = GetParagraphsToPrint();
            m_sRunningFooterText = ExtractRunningFooter(vDataParagraphs);
            var vDisplayParagraphs = GetDisplayParagraphsToPrint(vDataParagraphs);
            var vDisplayFootnotes = CollectAndNumberFootnotes(vDisplayParagraphs);

            // Get them properly measured and laid out according to their styles
            LayoutDisplayLines(vDisplayParagraphs);
            LayoutDisplayLines(vDisplayFootnotes);

            // A body line will appear on a page with all of its associated footnotes
            var vLineGroups = AssociateBodyWithFootnotes(vDisplayParagraphs, vDisplayFootnotes);
            SetScriptureReferences(vLineGroups);
            HandleKeepWithNext(vLineGroups);
            HandleLineSpacing(vLineGroups);

            // Create the pages based on the heights that will fit
            EnumeratedStepsProgressDlg.IncrementStep();
            while (vLineGroups.Count > 0)
            {
                var page = new Page(PDoc, Pages.Count, vLineGroups, 
                    m_sRunningFooterText, UserSettings.AllowPicturesToFloatOnPage)
                    {
                        WaterMarkText = (UserSettings.PrintWaterMark) ?
                            UserSettings.WaterMarkText : ""
                    };

                Pages.Add(page);
                EnumeratedStepsProgressDlg.AppendToLabelText("..." + Pages.Count);
            }
            EnumeratedStepsProgressDlg.ClearAppend();
        }
        #endregion
        #region Method: List<DParagraph> GetParagraphsToPrint()
        ICollection<DParagraph> GetParagraphsToPrint()
        {
            var vParagraphs = new List<DParagraph>();

            var vSectionsToPrint = DetermineSectionsToPrint();
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
        #region SMethod: string ExtractRunningFooter(vParagraphs)
        static string ExtractRunningFooter(ICollection<DParagraph> vParagraphs)
        {
            foreach(var p in vParagraphs)
            {
                if (p.Style != StyleSheet.RunningHeader) 
                    continue;

                vParagraphs.Remove(p);
                return p.AsString;
            }

            return DB.TargetBook.DisplayName;
        }
        #endregion
        #region Method: List<OWPara> GetDisplayParagraphsToPrint(vDParagraphs)
        List<OWPara> GetDisplayParagraphsToPrint(IEnumerable<DParagraph> vParagraphs)
        {
            var vDisplayParagraphs = new List<OWPara>();

            var writingSystem = (UserSettings.BackTranslation) ?
                BookToPrint.Translation.WritingSystemConsultant :
                BookToPrint.Translation.WritingSystemVernacular;

            var flags = (UserSettings.BackTranslation) ?
                OWPara.Flags.ShowBackTranslation :
                OWPara.Flags.None;

            foreach (var paragraph in vParagraphs)
            {
                // Skip over pictures if the user wants them ommited
                var bIsPicture = (paragraph as DPicture != null);
                if (bIsPicture && !UserSettings.PrintPictures)
                    continue;

                var owp = new OWPara(writingSystem, paragraph.Style,
                    paragraph, Color.White,flags);
                vDisplayParagraphs.Add(owp);

                MakeQuoteReplacements(owp);

                owp.SetPicture(WLayout.GetPicture(paragraph), false);
            }

            return vDisplayParagraphs;
        }
        #endregion
        #region Method: List<OWPara> CollectAndNumberFootnotes(IEnumerable<OWPara> vDisplayParagraphs)
        List<OWPara> CollectAndNumberFootnotes(IEnumerable<OWPara> vDisplayParagraphs)
        {
            var n = 0;

            var vFootnotes = new List<OWPara>();

            foreach (var paragraph in vDisplayParagraphs)
            {
                foreach (var item in paragraph.SubItems)
                {
                    var footLetter = item as EFoot;
                    if (null == footLetter)
                        continue;

                    var footnote = footLetter.Footnote;
                    var sLetter = footnote.Foot.GetFootnoteLetterFor(n++);
                    footLetter.Text = sLetter;

                    var owfn = new OWFootnotePara(footnote, Color.White, OWPara.Flags.None);
                    vFootnotes.Add(owfn);
                    MakeQuoteReplacements(owfn);

                    var label = owfn.SubItems[0] as EFootnoteLabel;
                    if (null != label)
                        label.Text = sLetter;
                }
            }

            return vFootnotes;
        }
        #endregion
        #region Method: void LayoutDisplayLines(vDisplayParagraphs)
        void LayoutDisplayLines(List<OWPara> vDisplayParagraphs)
        {
            // Place the paragraphs into a root container
            var root = new ERoot(null, new PrintContext(PDoc));
            root.Append(vDisplayParagraphs.ToArray());

            // Lay them out
            root.CalculateBlockWidths();
            root.DoLayout();
        }
        #endregion
        #region SMethod: List<AssociatedLines> AssociateBodyWithFootnotes(vDisplayParagraphs, vDisplayFootnotes)
        static List<AssociatedLines> AssociateBodyWithFootnotes(
            IEnumerable<OWPara> vDisplayParagraphs, 
            IEnumerable<OWPara> vDisplayFootnotes)
            // Handles widow/orphan control, by grouping Body Lines together that would otherwise
            // potentially form an orphan.
        {
            var vGroups = new List<AssociatedLines>();

            foreach(var owp in vDisplayParagraphs)
            {
                // Shorthand
                var fSpaceBefore = owp.Context.PointsToDeviceY(owp.Style.PointsBefore);
                var fSpaceAfter = owp.Context.PointsToDeviceY(owp.Style.PointsAfter);
                var count = owp.Lines.Count;

                // Widow/orphan control: small paragraphs always go together
                if (count <= 3)
                {
                    vGroups.Add( new AssociatedLines(owp.Lines, vDisplayFootnotes)
                        {
                            TopY = owp.Rectangle.Top,
                            BottomY = owp.Rectangle.Bottom,
                            SpaceBefore = fSpaceBefore,
                            SpaceAfter = fSpaceAfter,
                            Picture = owp.Picture
                        });
                    continue;
                }

                // The first two lines are a group
                vGroups.Add( new AssociatedLines( owp.Lines.GetRange(0, 2), vDisplayFootnotes)
                    {
                        TopY = owp.Rectangle.Top,
                        BottomY = owp.Lines[1].Rectangle.Bottom,
                        SpaceBefore = fSpaceBefore,
                        Picture = owp.Picture
                    });

                // The middle lines are individually separate
                for (var i = 2; i < count - 2; i++)
                    vGroups.Add(new AssociatedLines(owp.Lines[i], vDisplayFootnotes) 
                        {
                            TopY = owp.Lines[i].Rectangle.Top,
                            BottomY = owp.Lines[i].Rectangle.Bottom
                        });

                // The final two lines are a group
                vGroups.Add( new AssociatedLines(owp.Lines.GetRange(count - 2, 2), vDisplayFootnotes)
                    {
                        TopY = owp.Lines[count - 2].Rectangle.Top,
                        BottomY = owp.Rectangle.Bottom,
                        SpaceAfter = fSpaceAfter
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
        #region Method:  void HandleLineSpacing(vGroups)
        void HandleLineSpacing(IEnumerable<AssociatedLines> vGroups)
        {
            if (UserSettings.LineSpacing == 1F)
                return;

            var fTop = 0F;

            foreach (var group in vGroups)
            {
                group.TopY = fTop;
                group.HandleLineSpacing(UserSettings.LineSpacing);
                fTop += group.TotalHeight;
            }
        }
        #endregion
        #region SMethod: void HandleKeepWithNext(vGroups)
        static void HandleKeepWithNext(IList<AssociatedLines> vGroups)
        {
            for (var i = 0; i < vGroups.Count - 1;)
            {
                var thisGroup = vGroups[i];
                if (!thisGroup.KeepWithNext)
                {
                    ++i;
                    continue;
                }

                var nextGroup = vGroups[i + 1];
                if (!thisGroup.Append(nextGroup))
                {
                    ++i;
                    continue;
                }

                vGroups.Remove(nextGroup);
            }
        }
        #endregion

        // Implementation --------------------------------------------------------------------
        #region Method: List<DSection> DetermineSectionsToPrint()
        List<DSection> DetermineSectionsToPrint()
        {
            if (UserSettings.CurrentSection)
                return new List<DSection> {CurrentSection};

            if (UserSettings.EntireBook)
            {
                var v = new List<DSection>();
                foreach(DSection section in BookToPrint.Sections)
                    v.Add(section);
                return v;
            }

            if (UserSettings.Chapters)
            {
                var nStartAtChapter = UserSettings.StartChapter;
                var nEndAtChapter = Math.Max(UserSettings.EndChapter, nStartAtChapter);

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
        private int m_nCurrentPrintPage;
        #region Method: void DoPrint()
        void DoPrint()
        {
            try
            {
                EnumeratedStepsProgressDlg.IncrementStep();
                m_nCurrentPrintPage = 1;

                PDoc.PrintPage += PrintPage;
                PDoc.Print();

                EnumeratedStepsProgressDlg.ClearAppend();
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
            EnumeratedStepsProgressDlg.AppendToLabelText("..." + m_nCurrentPrintPage++);
            var page = Pages[0];
            page.Draw(new PrinterDraw(ev.Graphics));

            // Still more to do?
		    Pages.RemoveAt(0);
            ev.HasMorePages = (Pages.Count > 0);
        }
        #endregion
    }
}
