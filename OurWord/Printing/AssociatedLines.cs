#region ***** AssociatedLines.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    AssociatedLines.cs
 * Author:  John Wimbish
 * Created: 20 Dec 2009
 * Purpose: Groups of body lines and their corresponding footnotes
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Collections.Generic;
using System.Drawing;
using OurWord.Edit;
using OurWordData.DataModel;
#endregion

namespace OurWord.Printing
{
    public class AssociatedLines
    {
        private readonly List<ELine> BodyLines;
        public readonly List<ELine> FootnoteLines;
        #region VAttr{g}: bool HasFootnotes
        public bool HasFootnotes
        {
            get
            {
                return FootnoteLines.Count > 0;
            }
        }
        #endregion

        // Picture ---------------------------------------------------------------------------
        public EPicture Picture;
        #region Method: void DrawPicture(IDraw draw)
        void DrawPicture(IDraw draw)
        {
            if (null == Picture)
                return;
            Picture.Draw(draw);
        }
        #endregion
        #region Attr{g}: bool HasPicture
        bool HasPicture
        {
            get
            {
                return Picture != null;
            }
        }
        #endregion

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
        public float TopY { get; set; }
        public float BottomY { private get; set; }

        public float SpaceBefore { get; set; }
        public float SpaceAfter { get; set; }

        #region VAttr{g}: float TotalHeight
        public float TotalHeight
        {
            get
            {
                // Start with the height of the main body
                var fHeight = BottomY - TopY;

                fHeight += FootnotesHeight;

                return fHeight;
            }
        }
        #endregion
        #region VAttr{g}: float FootnotesHeight
        public float FootnotesHeight
        {
            get
            {
                var fHeight = 0F;

                foreach (var line in FootnoteLines)
                    fHeight += line.LargestItemHeight;

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
        #region Attr{g}: bool KeepWithNex
        public bool KeepWithNext
        {
            get
            {
                if (BodyLines.Count == 0)
                    return false;

                var lastLine = BodyLines[BodyLines.Count - 1];
                var item = (lastLine.SubItems.Length > 0) ? lastLine.SubItems[0] : null;
                if (null == item)
                    return false;

                var owp = item.Owner as OWPara;
                if (null == owp)
                    return false;

                return owp.PStyle.KeepWithNext;
            }
        }
        #endregion
        #region Method: bool Append(group)
        public bool Append(AssociatedLines group)
        {
            if (group.HasPicture)
                return false;

            BodyLines.AddRange(group.BodyLines);
            FootnoteLines.AddRange(group.FootnoteLines);
            SpaceAfter = group.SpaceAfter;
            BottomY = group.BottomY;
            return true;
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
        #region Method: void MoveYs(float fBodyAdjustment)
        public void MoveYs(float fBodyAdjustment)
        {
            if (fBodyAdjustment == 0F)
                return;

            TopY += fBodyAdjustment;
            BottomY += fBodyAdjustment;

            foreach (var line in BodyLines)
                line.MoveYs(fBodyAdjustment);
    
            if (null != Picture)
                Picture.MoveYs(fBodyAdjustment);
        }
        #endregion
        #region Method: void HandleLineSpacing(float fMultiplier)
        public void HandleLineSpacing(float fMultiplier)
        {
            if (fMultiplier == 1F)
                return;

            var y = TopY;

            // Add the SpaceBefore
            SpaceBefore *= fMultiplier;
            y += SpaceBefore;

            // Add the picture
            if (HasPicture)
            {
                Picture.SetY(y);
                y += Picture.Height;
            }

            // Handle the lines of text
            for (var i = 0; i < BodyLines.Count; i++)
            {
                var thisLine = BodyLines[i];
                var nextLine = (i < BodyLines.Count - 1) ? BodyLines[i + 1] : null;

                var ySpaceBetweenLines = (null != nextLine) ?
                    nextLine.Position.Y - thisLine.Position.Y :
                    thisLine.LargestItemHeight;

                thisLine.SetYs(y);

                y += ySpaceBetweenLines * fMultiplier;
            }

            // Add the SpaceAfter
            SpaceAfter *= fMultiplier;
            y += SpaceAfter;

            // Set the bottom
            BottomY = y;
        }
        #endregion
    }
}
