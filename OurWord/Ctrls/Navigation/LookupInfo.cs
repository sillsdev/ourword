using System.Diagnostics;
using System.Windows.Forms;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;

namespace OurWord.Ctrls.Navigation
{
    public class LookupInfo
    {
        // Attrs -----------------------------------------------------------------------------
        // Attrs: Hierarchy
        public readonly string BookAbbrev;
        public readonly int SectionNo;
        public readonly int ParagraphNo;
        public readonly int RunNo;
        public readonly int FootnoteRunNo = -1;  // By default, not a footnote
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
        public readonly int Chapter;
        public readonly int Verse;

        // Attrs: Selection
        public readonly string Text;
        public readonly int IndexIntoText;
        public readonly int SelectionLength;
        public readonly bool IsBackTranslation;

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

        // Methods ---------------------------------------------------------------------------
        #region Method: ListViewItem ToListViewItem(ListViewGroup group)
        public ListViewItem ToListViewItem(ListViewGroup group)
        {
            var sReference = string.Format("{0} {1}:{2}", BookAbbrev, Chapter, Verse);

            return new ListViewItem(new[] { sReference, Text }, group) { Tag = this };
        }
        #endregion

    }
}
