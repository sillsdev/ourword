using System;
using System.Collections.Generic;
using System.Diagnostics;
using OurWord.Edit;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

namespace OurWord.Ctrls.Navigation
{
    static public class Scanner
    {
        // Chapter count ---------------------------------------------------------------------
        #region SMethod: int GetChapterCount(DBook book)
        static public int GetChapterCount(DBook book)
        {
            var bookInfo = G.BookGroups.FindBook(book.BookAbbrev);
            Debug.Assert(null != bookInfo);
            return bookInfo.ChapterCount;
        }
        #endregion
        #region SMethod: int GetChapterCount()
        static public int GetChapterCount()
        {
            var cTotalChapters = 0;
            foreach (var book in DB.TargetTranslation.BookList)
                cTotalChapters += GetChapterCount(book);
            return cTotalChapters;
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region Class: SearchContext
        public class SearchContext
        {
            // Directly supplied by caller
            public readonly string SearchFor;
            public bool IgnoreCase;
            public readonly bool IsBackTranslation;
            public bool CurrentBookOnly;
            public readonly DBook OriginalBook;
            private readonly string PathToOriginalDbt;
            public readonly int IndexIntoText;
            public SearchType Type = SearchType.Anywhere;

            #region vattr{g}: DBasicText OriginalDbt
            public DBasicText OriginalDbt
            {
                get
                {
                    Debug.Assert(OriginalBook.Loaded);
                    return OriginalBook.GetObjectFromPath(PathToOriginalDbt) as DBasicText;
                }
            }
            #endregion
            #region vattr{g}: StringComparison ComparisonOption
            public StringComparison ComparisonOption
            {
                get
                {
                    return (IgnoreCase) ?
                        StringComparison.InvariantCultureIgnoreCase :
                        StringComparison.InvariantCulture;
                }
            }
            #endregion
            #region vattr{g}: ritingSystem WritingSystem
            public WritingSystem WritingSystem
            {
                get
                {
                    return (IsBackTranslation) ? 
                        DB.TargetTranslation.WritingSystemConsultant :
                        DB.TargetTranslation.WritingSystemVernacular;
                }
            }
            #endregion

            #region Constructor(sSearchFor, selection)
            public SearchContext(string sSearchFor, OWWindow.Sel selection)
                : this(sSearchFor, selection.Book, selection.DBT.GetPathFromRoot(), 
                    selection.DBT_iCharFirst)
            {
                IsBackTranslation = selection.Paragraph.DisplayBT;
            }
            #endregion
            #region Constructor(sSearchFor, book, sPathToDbt, IndexIntoText)
            public SearchContext(string sSearchFor, DBook book, string sPathToDbt, int iIndexIntoText)
            {
                SearchFor = sSearchFor;
                OriginalBook = book;
                PathToOriginalDbt = sPathToDbt;
                IndexIntoText = iIndexIntoText;
            }
            #endregion
        }
        #endregion

        // String Scan -----------------------------------------------------------------------
        public enum SearchType { Whole, Beginning, End, Anywhere };
        #region smethod: bool IsAtWordBeginning(context, sSource, iPos)
        static public bool IsAtWordBeginning(SearchContext context, string sSource, int iPos)
        {
            // If at beginning of string
            if (iPos == 0)
                return true;

            // If preceeded by whitespace or punctuation
            var chBefore = sSource[iPos - 1];
            if (char.IsWhiteSpace(chBefore))
                return true;
            return context.WritingSystem.IsPunctuation(chBefore);
        }
        #endregion
        #region smethod:  bool IsAtWordEnding(context, sSource, iPos)
        static public bool IsAtWordEnding(SearchContext context, string sSource, int iPos)
        {
            // If at end of the string
            var iPosEnd = iPos + context.SearchFor.Length;
            if (sSource.Length == iPosEnd)
                return true;

            // If followed by whitespace or punctuation
            var chAfter = sSource[iPosEnd];
            if (char.IsWhiteSpace(chAfter))
                return true;
            return context.WritingSystem.IsEndPunctuation(chAfter);
        }
        #endregion
        #region smethod: bool MatchesSearchType(context, sSource, iPos)
        static public bool MatchesSearchType(SearchContext context, string sSource, int iPos)
        {
            switch (context.Type)
            {
                case SearchType.Anywhere:
                    return true;
                case SearchType.Beginning:
                    return IsAtWordBeginning(context, sSource, iPos);
                case SearchType.End:
                    return IsAtWordEnding(context, sSource, iPos);
                case SearchType.Whole:
                    {
                        var bBegin = IsAtWordBeginning(context, sSource, iPos);
                        var bEnd = IsAtWordEnding(context, sSource, iPos);
                        return bBegin && bEnd;
                    }
                default:
                    throw new Exception("Unrecognized SearthType");
            }
        }
        #endregion
        #region smethod: List<int> ScanString(context, sSource)
        static public List<int> ScanString(SearchContext context, string sSource)
        {
            var v = new List<int>();

            var i = 0;
            while ((i = sSource.IndexOf(context.SearchFor, i, context.ComparisonOption)) != -1)
            {
                if (MatchesSearchType(context, sSource, i))
                    v.Add(i);
                i += context.SearchFor.Length;
            }

            return v;
        }
        #endregion

        // Scan and Search -------------------------------------------------------------------
        #region smethod: List<DBasicText> GetTexts(book)
        static public List<DBasicText> GetTexts(DBook book)
        {
            Debug.Assert(book.Loaded);

            var v = new List<DBasicText>();

            foreach (DSection section in book.Sections)
            {
                var vMain = new List<DBasicText>();
                var vFootnotes = new List<DBasicText>();

                foreach (DParagraph paragraph in section.Paragraphs)
                {
                    foreach (DRun run in paragraph.Runs)
                    {
                        if (null != run as DBasicText)
                            vMain.Add(run as DBasicText);

                        if (null != run as DFoot)
                            vFootnotes.AddRange((run as DFoot).Footnote.Texts);
                    }
                }

                v.AddRange(vMain);
                v.AddRange(vFootnotes);
            }

            return v;
        }
        #endregion
        #region smethod: List<DBasicText> GetTextsAfter(dbtTarget)
        static public List<DBasicText> GetTextsAfter(DBasicText dbtTarget)
        {
            var book = dbtTarget.Paragraph.Book;

            var vAllTexts = GetTexts(book);

            for (var i = 0; i < vAllTexts.Count; i++)
            {
                if (vAllTexts[i] == dbtTarget)
                {
                    const int iFirstOneToRemove = 0;    // Remove from the beginning
                    var cAmountToRemove = i + 1;
                    vAllTexts.RemoveRange(iFirstOneToRemove, cAmountToRemove);
                    break;
                }
            }

            return vAllTexts;
        }
        #endregion
        #region smethod: List<DBasicText> GetTextsPrior(dbtTarget)
        static public List<DBasicText> GetTextsPrior(DBasicText dbtTarget)
        {
            var book = dbtTarget.Paragraph.Book;

            var vAllTexts = GetTexts(book);

            for (var i = 0; i < vAllTexts.Count; i++)
            {
                if (vAllTexts[i] == dbtTarget)
                {
                    var iFirstOneToRemove = i;    // Remove the target (and all after)
                    var cAmountToRemove = vAllTexts.Count - iFirstOneToRemove;
                    vAllTexts.RemoveRange(iFirstOneToRemove, cAmountToRemove);
                    break;
                }
            }

            return vAllTexts;
        }
        #endregion
        #region smethod: List<LookupInfo> ScanTexts(context, vTexts)
        static public List<LookupInfo> ScanTexts(SearchContext context, IEnumerable<DBasicText> vTexts)
        {
            var vAllTexts = new List<LookupInfo>();

            foreach (var dbt in vTexts)
            {
                var vThisDbt = ScanText(context, dbt);
                foreach (var li in vThisDbt)
                    vAllTexts.Add(li);
            }

            return vAllTexts;
        }
        #endregion
        #region smethod: List<LookupInfo> ScanText(SearchContext context, DBasicText dbt)
        static IEnumerable<LookupInfo> ScanText(SearchContext context, DBasicText dbt)
        {
            var phrases = (context.IsBackTranslation) ? dbt.PhrasesBT : dbt.Phrases;
            var sSource = phrases.AsString;

            var v = new List<LookupInfo>();

            var vi = ScanString(context, sSource);
            foreach(var i in vi)
                v.Add(new LookupInfo(phrases, i, context.SearchFor.Length));

            return v;
        }
        #endregion
        #region smethod: List<DBook> BuildBookScanList(DBook bookExclude)
        static List<DBook> BuildBookScanList(DBook bookExclude)
        {
            var bBookFound = false;
            var iInsertPoint = 0;
            var vBooks = new List<DBook>();

            foreach (var book in DB.TargetTranslation.BookList)
            {
                if (book == bookExclude)
                {
                    bBookFound = true;
                    continue;
                }

                if (!bBookFound)
                    vBooks.Add(book);
                else
                    vBooks.Insert(iInsertPoint++, book);
            }

            return vBooks;
        }
        #endregion
        #region smethod: LookupInfo ScanForNext(SearchContext context)
        static public LookupInfo ScanForNext(SearchContext context)
        {
            // Get the current selection (move to the end if a content selection)
            var selection = G.App.CurrentLayout.Selection;
            if (null == selection)
                return null;
            if (selection.IsContentSelection)
                selection = new OWWindow.Sel(selection.Paragraph, selection.Last);
            var dbtTarget = selection.DBT;
            var bookTarget = dbtTarget.Paragraph.Book;
            var indexTarget = selection.DBT_iCharFirst;

            // Scan the remainder of the current text
            var vLookupCurrentText = ScanText(context, dbtTarget);
            foreach (var lookupInfo in vLookupCurrentText)
            {
                if (lookupInfo.IndexIntoText > indexTarget)
                    return lookupInfo;
            }

            // Scan the remainder of the current book
            var vAfter = GetTextsAfter(selection.DBT);
            var vLookup = ScanTexts(context, vAfter);
            if (vLookup.Count > 0)
                return vLookup[0];

            // Scan the remaining books
            if(!context.CurrentBookOnly)
            {
                var vBooks = BuildBookScanList(bookTarget);               
                foreach (var book in vBooks)
                {
                    using (new LoadedBook(book))
                    {
                        var vTexts = (book == context.OriginalBook) ?
                            GetTextsPrior(context.OriginalDbt) :
                            GetTexts(book);
                        vLookup = ScanTexts(context, vTexts);
                        if (vLookup.Count > 0)
                            return vLookup[0];
                    }
                }
            }

            // Scan the beginning of this book
            var vBefore = GetTextsPrior(dbtTarget);
            vLookup = ScanTexts(context, vBefore);
            if (vLookup.Count > 0)
                return vLookup[0];

            // Scan the beginning of this text
            foreach (var lookupInfo in vLookupCurrentText)
            {
                if (lookupInfo.IndexIntoText < context.IndexIntoText)
                    return lookupInfo;
            }
            return null;
        }
        #endregion
    }
}
