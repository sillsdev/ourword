using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OurWord.Edit;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;

namespace OurWord.Ctrls.Navigation
{
    static public class Scanner
    {
        #region smethod: List<int> GetIndexesOf(sSource, sSearchFor, bIgnoreCase)
        static public List<int> GetIndexesOf(string sSource, string sSearchFor, bool bIgnoreCase)
        {
            var rules = (bIgnoreCase) ? StringComparison.InvariantCultureIgnoreCase :
                StringComparison.InvariantCulture;

            var v = new List<int>();

            var iOffset = 0;
            int iPos;
            while ((iPos = sSource.IndexOf(sSearchFor, rules)) != -1)
            {
                v.Add(iOffset + iPos);
                sSource = sSource.Substring(iPos + sSearchFor.Length);
                iOffset += (iPos + sSearchFor.Length);
            }

            return v;
        }
        #endregion
        #region SMethod: List<LookupInfo> ScanBook(SearchContext, DBook)
        public static List<LookupInfo> ScanBook(SearchContext context, DBook book)
        {
            var vTexts = GetTextsToScan(book, ScanOption.All, null);

            // Scan the DTexts for the search string
            var vLookupInfo = new List<LookupInfo>();
            foreach (var text in vTexts)
            {
                var phrases = (context.IsBackTranslation) ?
                    text.PhrasesBT : text.Phrases;
                var sText = phrases.AsString;
                var vIndexes = GetIndexesOf(sText, context.SearchFor, context.IgnoreCase);
                foreach (var iPosition in vIndexes)
                    vLookupInfo.Add(new LookupInfo(phrases, iPosition, context.SearchFor.Length));
            }

            return vLookupInfo;
        }
        #endregion
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

        // Scan Book for Search String -------------------------------------------------------
        private static bool s_bContinueToNextBook;
        private static bool s_bDontAskAgain;
        #region Class: SearchContext
        public class SearchContext
        {
            public readonly string SearchFor;
            private readonly OWWindow.Sel OriginalSelection;
            public bool IgnoreCase;

            #region Constructor(sSearchFod, originalSelection)
            public SearchContext(string sSearchFor, OWWindow.Sel selection)
            {
                SearchFor = sSearchFor;
                OriginalSelection = selection;
            }
            #endregion

            #region VAttr{g}: bool IsBackTranslation
            public bool IsBackTranslation
            {
                get
                {
                    if (null == OriginalSelection)
                        return false;
                    return OriginalSelection.Paragraph.DisplayBT; 
                }
            }
            #endregion
            #region VAttr{g}: DBook OriginalBook
            public DBook OriginalBook
            {
                get
                {
                    return null == OriginalSelection ? null : OriginalSelection.Book;
                }
            }
            #endregion
        }
        #endregion
        public enum ScanOption { PriorTo, After, All };
        #region smethod: List<DBasicText> GetTextsAndFootnotes(DParagraph paragraph)
        static public List<DBasicText> GetTextsAndFootnotes(DParagraph paragraph)
        {
            var v = new List<DBasicText>();

            foreach(var run in paragraph.Runs)
            {
                // Paragraph text
                var text = run as DBasicText;
                if (null != text)
                    v.Add(text);

                // Footnotes
                var foot = run as DFoot;
                if (null != foot)
                {
                    foreach(var footRun in foot.Footnote.Runs)
                    {
                        if (null != footRun as DBasicText)
                            v.Add(footRun as DBasicText);
                    }
                }
            }

            return v;
        }
        #endregion
        #region smethod: List<DBasicText> GetTextsToScan(DBook, ScanOption, DBasicText target)
        static public List<DBasicText> GetTextsToScan(DBook book, ScanOption option, DBasicText target)
            // Collect all of the DTexts within a book that we want to scan for the search
            // string, with options for All, or for either before or after a target text.
        {
            var bFound = false;

            var v = new List<DBasicText>();

            var vParagraphs = book.AllParagraphs;
            foreach(var paragraph in vParagraphs)
            {
                if (!paragraph.IsUserEditable)
                    continue;
                var vTexts = GetTextsAndFootnotes(paragraph);

                foreach (var text in vTexts)
                {
                    switch (option)
                    {
                        case ScanOption.PriorTo:
                            v.Add(text);
                            if (text == target)
                                return v;
                            break;

                        case ScanOption.All:
                            v.Add(text);
                            break;

                        case ScanOption.After:
                            if (text == target)
                                bFound = true;
                            if (bFound)
                                v.Add(text);
                            break;
                    }
                }
            }

            return v;
        }
        #endregion
        #region smethod: LookupInfo ScanBook(SearchContext, DBook, anOption, selection)
        static public LookupInfo ScanBook(SearchContext context, DBook book, ScanOption option, 
            OWWindow.Sel selection)
            // selection - starting/ending point for the search, depending on the
            //   ScanOption. Ignored if the ScanOption is 'All'.
        {
            // If we have text selected, move to its end
            if (null != selection && selection.IsContentSelection)
            {
                selection = (ScanOption.PriorTo == option) ? 
                    new OWWindow.Sel(selection.Paragraph, selection.First) : 
                    new OWWindow.Sel(selection.Paragraph, selection.Last);
            }

            // Get the candidate texts
            var vTexts = GetTextsToScan(book, option, 
                (null == selection ? null : selection.DBT));

            // The first text, we use the selection to determine where we start at, so that
            // we scan for more hits in the current text; afterwards we can reset to zero
            // as we'll be moving to later texts.
            var iStartAt = (null == selection || ScanOption.After != option) ? 
                0 : 
                selection.DBT_iCharFirst;

            // The final text, we again use the selection to determine where to stop at,
            // so that we don't bother the user again with something we've already given him.
            var iEndAt = (null == selection || ScanOption.PriorTo != option) ?
                -1 :
                selection.DBT_iCharFirst;

            var comparisonOption = (context.IgnoreCase) ?
                StringComparison.InvariantCultureIgnoreCase :
                StringComparison.InvariantCulture;

            // Scan the texts
            foreach(var text in vTexts)
            {
                var phrases = (context.IsBackTranslation) ?
                    text.PhrasesBT : text.Phrases;

                var sText = phrases.AsString;

                // If the final text, don't search past iEndAt
                if (text == vTexts[vTexts.Count - 1] && -1 != iEndAt)
                    sText = sText.Substring(0, iEndAt);

                var i = sText.IndexOf(context.SearchFor, iStartAt, comparisonOption);
                if (-1 != i)
                    return new LookupInfo(phrases, i, context.SearchFor.Length);

                iStartAt = 0;
            }

            return null;
        }
        #endregion
        #region SMethod: LookupInfo ScanForNext(sSearchFor, OWWindow.Sel selectionStopAt)
        static public LookupInfo ScanForNext(string sSearchFor, OWWindow.Sel originalSelection)
        {
            // Get our current context
            var currentSelection = G.App.CurrentLayout.Selection;
            if (null == currentSelection)
                return null;
            var currentBook = currentSelection.Book;

            var context = new SearchContext(sSearchFor, originalSelection);

            // First scan the remainder of this current book
            var lookupInfo = ScanBook(context, currentBook, ScanOption.After, currentSelection);
            if (null != lookupInfo)
                return lookupInfo;

            // Build the list of books in the order we'll scan them
            var bBookFound = false;
            var iInsertPoint = 0;
            var vBooks = new List<DBook>();
            foreach(var book in DB.TargetTranslation.BookList)
            {
                if (book == currentBook)
                {
                    bBookFound = true;
                    continue;
                }

                if (!bBookFound)
                    vBooks.Add(book);
                else
                    vBooks.Insert(iInsertPoint++, book);
            }

            // Ask if the user wishes to continue to other books
            if (vBooks.Count > 0 && !s_bDontAskAgain)
            {
                var dlg = new DlgContinueToNextBook();
                var result = dlg.ShowDialog(G.App);
                s_bContinueToNextBook = (result == DialogResult.Yes);
                s_bDontAskAgain = dlg.DontAskAgain;
            }
            if (!s_bContinueToNextBook)
                return null;

            // If not found, scan the remaining books
            foreach(var book in vBooks)
            {
                if (book == context.OriginalBook)
                    break;
                using (new LoadedBook(book))
                {
                    lookupInfo = ScanBook(context, book, ScanOption.All, null);                    
                }
                if (null != lookupInfo)
                    return lookupInfo;
            }

            // If not found, scan the end book prior to the end selection
            return ScanBook(context, context.OriginalBook, ScanOption.PriorTo, originalSelection);
        }
        #endregion
    }
}
