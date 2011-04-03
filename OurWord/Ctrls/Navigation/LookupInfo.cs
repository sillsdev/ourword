using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;

namespace OurWord.Ctrls.Navigation
{
    public class LookupInfo
    {
        // Attrs -----------------------------------------------------------------------------
        // Attrs: Hierarchy
        public string BookAbbrev;
        public int SectionNo;
        public int ParagraphNo;
        public int RunNo;
        public int FootnoteRunNo = -1;  // By default, not a footnote
        #region VAttr{g}: bool IsFootnote
        public bool IsFootnote
        {
            get
            {
                return FootnoteRunNo != -1;
            }
        }
        #endregion

        // Attrs: Reference
        public int Chapter;
        public int Verse;

        // Attrs: Selection
        public string Text;
        public int IndexIntoText;
        public int SelectionLength;
        public bool IsBackTranslation;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DText, iIndexIntoText, cSelectionLength)
        public LookupInfo(DPhraseList<DPhrase> phrases, int iIndexIntoText, int cSelectionLength)
        {
            // Get Hierarchy: Retrieve the owning text and its paragraph
            var text = phrases.Text;
            var paragraph = text.Paragraph;
            RunNo = paragraph.GetNonEmptyRunNo(text);

            // Get Hierarchy: If a footnote, we must bump up a level to the owning paragraph
            var footnote = text.Paragraph as DFootnote;
            if (null != footnote)
            {
                FootnoteRunNo = RunNo;
                paragraph = footnote.OwningParagraph;
                RunNo = paragraph.GetNonEmptyRunNo(footnote.Foot);
            }

            // Get Hierarchy: Section and Book
            var section = paragraph.Section;
            ParagraphNo = section.Paragraphs.FindObj(paragraph);

            var book = section.Book;
            SectionNo = book.Sections.FindObj(section);
            BookAbbrev = book.BookAbbrev;

            // Store info about the selection
            IsBackTranslation = phrases.IsBackTranslation;
            Text = phrases.AsString;
            IndexIntoText = iIndexIntoText;
            SelectionLength = cSelectionLength;

            // Store Chapter/Verse so as to display correct reference
            Chapter = paragraph.ReferenceSpan.Start.Chapter;
            Verse = paragraph.ReferenceSpan.Start.Verse;
            foreach (var run in paragraph.Runs)
            {
                // Stop when we find our text, either in the main paragraph...
                if (run == text)
                    break;
                // ...or in a footnote
                var foot = run as DFoot;
                if (null != foot && foot.Footnote.Runs.FindObj(text) != -1)
                    break;

                var verse = run as DVerse;
                if (null != verse)
                    Verse = verse.VerseNo;

                var chapter = run as DChapter;
                if (null != chapter)
                {
                    Chapter = chapter.ChapterNo;
                    Verse = 1;
                }
            }
        }
        #endregion
        #region constructor()
        protected LookupInfo()
        {
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: ListViewItem ToListViewItem(ListViewGroup group)
        public ListViewItem ToListViewItem(ListViewGroup group)
        {
            var sReference = string.Format("{0} {1}:{2}", BookAbbrev, Chapter, Verse);
            return new ListViewItem(new[] { sReference, Text }, group) { Tag = this };
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        private const string c_sBookAbbrev = "book";
        private const string c_sSectionNo = "section";
        private const string c_sParagraphNo = "para";
        private const string c_sRunNo = "run";
        private const string c_sFootnoteRunNo = "foot";

        private const string c_sChapter = "c";
        private const string c_sVerse = "v";

        private const string c_sText = "text";
        private const string c_sIndex = "index";
        private const string c_sLength = "length";
        private const string c_sBT = "bt";
        #endregion
        #region method: void Save(doc, node)
        protected void Save(XmlDoc doc, XmlNode node)
        {
            doc.AddAttr(node, c_sBookAbbrev, BookAbbrev);
            doc.AddAttr(node, c_sSectionNo, SectionNo);
            doc.AddAttr(node, c_sParagraphNo, ParagraphNo);
            doc.AddAttr(node, c_sRunNo, RunNo);
            doc.AddAttr(node, c_sFootnoteRunNo, FootnoteRunNo);

            doc.AddAttr(node, c_sChapter, Chapter);
            doc.AddAttr(node, c_sVerse, Verse);

            doc.AddAttr(node, c_sText, Text);
            doc.AddAttr(node, c_sIndex, IndexIntoText);
            doc.AddAttr(node, c_sLength, SelectionLength);
            doc.AddAttr(node, c_sBT, IsBackTranslation);
        }
        #endregion
        #region method: void Read(node)
        protected void Read(XmlNode node)
        {
            BookAbbrev = XmlDoc.GetAttrValue(node, c_sBookAbbrev);
            SectionNo = XmlDoc.GetAttrValue(node, c_sSectionNo, 0);
            ParagraphNo = XmlDoc.GetAttrValue(node, c_sParagraphNo, 0);
            RunNo = XmlDoc.GetAttrValue(node, c_sRunNo, 0);
            FootnoteRunNo = XmlDoc.GetAttrValue(node, c_sFootnoteRunNo, -1);

            Chapter = XmlDoc.GetAttrValue(node, c_sChapter, 0);
            Verse = XmlDoc.GetAttrValue(node, c_sVerse, 0);

            Text = XmlDoc.GetAttrValue(node, c_sText);
            IndexIntoText = XmlDoc.GetAttrValue(node, c_sIndex, 0);
            SelectionLength = XmlDoc.GetAttrValue(node, c_sLength, 0);
            IsBackTranslation = XmlDoc.GetAttrValue(node, c_sBT, false);
        }
        #endregion
    }
}
