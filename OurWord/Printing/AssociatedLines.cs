using System.Collections.Generic;
using OurWord.Edit;
using OurWordData.DataModel;

namespace OurWord.Printing
{
    public class AssociatedLines
    {
        public readonly List<ELine> BodyLines;
        public readonly List<ELine> FootnoteLines;

        // Chapter/Verse Scripture References ------------------------------------------------
        private int ChapterNumber { get; set; }
        private int VerseNumber { get; set; }
        #region Attr{g}: DReference ScriptureReference
        public DReference ScriptureReference
        {
            get
            {
                return new DReference(ChapterNumber, VerseNumber);
            }
        }
        #endregion
        #region Method: void SetScriptureReferences(ref nChapterNumber, ref nVerseNumber)
        public void SetScriptureReferences(ref int nChapterNumber, ref int nVerseNumber)
        {
            // By default, these will be what is passed in
            ChapterNumber = nChapterNumber;
            VerseNumber = nVerseNumber;

            // Update if we encounter a number within our body lines
            foreach(var line in BodyLines)
            {
                foreach (var item in line.SubItems) 
                {
                    var chapter = item as OWPara.EChapter;
                    if (null != chapter)
                    {
                        ChapterNumber = chapter.Number;
                        VerseNumber = 1;
                    }

                    var verse = item as OWPara.EVerse;
                    if (null != verse)
                    {
                        VerseNumber = verse.Number;
                    }
                }
            }

            // The caller needs the updated values to pass to the next one
            nChapterNumber = ChapterNumber;
            nVerseNumber = VerseNumber;
        }
        #endregion

        // Line height -----------------------------------------------------------------------
        public float SpaceAbove { private get; set; }
        public float SpaceBelow { private get; set; }
        #region VAttr{g}: float TotalHeight
        public float TotalHeight
        {
            get
            {
                var fHeight = 0.0F;

                foreach (var line in BodyLines)
                    fHeight += line.Height;

                foreach (var line in FootnoteLines)
                    fHeight += line.LargestItemHeight;

                fHeight += SpaceAbove;
                fHeight += SpaceBelow;

                return fHeight;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(ELine, vDisplayFootnotes)
        public AssociatedLines(ELine line, IEnumerable<OWPara> vDisplayFootnotes)
            : this(new List<ELine> {line}, vDisplayFootnotes )
        {
        }
        #endregion
        #region Constructor(List<ELine>, vDisplayFootnotes)
        public AssociatedLines(IEnumerable<ELine> vLines, IEnumerable<OWPara> vDisplayFootnotes)
        {
            ChapterNumber = 1;
            VerseNumber = 1;

            BodyLines = new List<ELine>();
            FootnoteLines = new List<ELine>();

            BodyLines.AddRange(vLines);

            foreach (var line in vLines)
            {
                foreach (var item in line.SubItems)
                {
                    var foot = item as OWPara.EFoot;
                    if (null == foot)
                        continue;

                    foreach (var owfn in vDisplayFootnotes)
                    {
                        if (owfn.DataSource == foot.Footnote)
                            FootnoteLines.AddRange(owfn.Lines);
                    }
                }               
            }
        }
        #endregion

    }
}
