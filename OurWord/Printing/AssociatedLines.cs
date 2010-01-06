using System.Collections.Generic;
using System.Drawing;
using OurWord.Edit;
using OurWordData.DataModel;

namespace OurWord.Printing
{
    public class AssociatedLines
    {
        public readonly List<ELine> BodyLines;
        public readonly List<ELine> FootnoteLines;
        public Bitmap Picture { private get; set; }

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
        public float SpaceBefore { private get; set; }
        public float SpaceAfter { private get; set; }
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

                fHeight += SpaceBefore;
                fHeight += SpaceAfter;

                fHeight += Picture.Height;

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

        // Drawing ---------------------------------------------------------------------------
        #region Method: void Draw(IDraw)
        public void Draw(IDraw draw)
        {
            var vLines = AllLines;
            foreach (var line in vLines)
                line.Draw(draw);

            DrawPicture(draw);
        }
        #endregion
        #region VAttr{g}: List<ELine> AllLines
        public List<ELine> AllLines
        {
            get
            {
                var v = new List<ELine>();
                v.AddRange(BodyLines);
                v.AddRange(FootnoteLines);
                return v;
            }
        }
        #endregion
        void DrawPicture(IDraw draw)
        {
            if (null == Picture)
                return;

            var xBmp = 100F;
            var yBmp = 100F;
            draw.DrawImage(Picture, new PointF(xBmp, yBmp));
        }

        public void Layout(ref float yBodyTop, ref float yFootnoteTop)
        {

 //           float fTop = m_PageSettings.Bounds.Top + m_PageSettings.Margins.Top;
 //           var firstLine = Groups[0].BodyLines[0];
 //           var fAdjust = firstLine.Position.Y - yPrintAreaTop;

            /*
            foreach (var line in BodyLines)
            {
                foreach (var item in line.SubItems)
                    item.Position = new PointF(item.Position.X, item.Position.Y - fAdjust);
            }
            */
        }

    }
}
