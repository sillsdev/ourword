using System.Windows.Forms;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;

namespace OurWord.Ctrls.Navigation
{
    public class ConcordanceInfo
    {
        // Attrs: Hierarchy
        public readonly string BookAbbrev;
        public readonly int SectionNo;
        public readonly int ParagraphNo;
        public readonly int RunNo;

        // Attrs: Reference
        public readonly int Chapter;
        public readonly int Verse;

        // Attrs: Selection
        public readonly string Text;
        public readonly int IndexIntoText;
        public readonly int IndexIntoParagraph;
        public readonly int SelectionLength;

        #region Constructor(DText, iIndexIntoText, cSelectionLength)
        public ConcordanceInfo(DBasicText text, int iIndexIntoText, int cSelectionLength)
        {
            var paragraph = text.Paragraph;
            var section = paragraph.Section;
            var book = section.Book;

            // Store hierarchy so as to be able to drill down
            BookAbbrev = book.BookAbbrev;
            SectionNo = book.Sections.FindObj(section);
            ParagraphNo = section.Paragraphs.FindObj(paragraph);
            RunNo = paragraph.Runs.FindObj(text);

            // Store Chapter/Verse so as to display correct reference
            Chapter = paragraph.ReferenceSpan.Start.Chapter;
            Verse = paragraph.ReferenceSpan.Start.Verse;
            foreach (var run in paragraph.Runs)
            {
                if (run == text)
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

                var priorText = run as DText;
                if (null != priorText)
                    IndexIntoParagraph += priorText.ContentsAsString.Length;
            }

            // Store info about the selection
            Text = text.ContentsAsString;
            IndexIntoText = iIndexIntoText;
            SelectionLength = cSelectionLength;
            IndexIntoParagraph += IndexIntoText;
        }
        #endregion

        #region Method: ListViewItem ToListViewItem(ListViewGroup group)
        public ListViewItem ToListViewItem(ListViewGroup group)
        {
            var sReference = string.Format("{0} {1}:{2}", BookAbbrev, Chapter, Verse);

            return new ListViewItem(new[] { sReference, Text }, group) { Tag = this };
        }
        #endregion
    }
}
