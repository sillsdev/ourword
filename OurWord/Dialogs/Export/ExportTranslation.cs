using System;
using System.Diagnostics;
using System.Windows.Forms;
using OurWordData.DataModel;

namespace OurWord.Dialogs.Export
{
    public class ExportTranslation
    {
        private readonly DTranslation m_Translation;

        // Work Methods ----------------------------------------------------------------------
        #region Method: void UpdateProgress(sBookName)
        void UpdateProgress(string sBookName)
        {
            var sBase = Loc.GetString("kExportStatus", "Exporting {0} - {1}...");
            var sStatus = string.Format(sBase, m_Translation.DisplayName, sBookName);
            DlgProgress.SetCurrentActivity(sStatus);
        }
        #endregion

        #region smethod: int GetChapterCount(DBook book)
        static int GetChapterCount(DBook book)
        {
            var bookInfo = G.BookGroups.FindBook(book.BookAbbrev);
            Debug.Assert(null != bookInfo);
            return bookInfo.ChapterCount;
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Constructor(DTranslation)
        public ExportTranslation(DTranslation translation)
        {
            if (null == translation)
                throw new ArgumentException("\"translation\" cannot be null..");

            m_Translation = translation;
        }
        #endregion
        #region Attr{g}: bool CanExportTranslation
        public bool CanExportTranslation
        {
            get
            {
                if (!DB.IsValidProject)
                    return false;
                if (m_Translation.BookList.Count == 0)
                    return false;
                return true;
            }
        }
        #endregion
        #region Attr{g}: int TotalChapterCount
        public int TotalChapterCount
        {
            get
            {
                var c = 0;
                foreach (var book in m_Translation.BookList)
                {
                    c += GetChapterCount(book);
                }
                return c;
            }
        }
        #endregion

        #region Method: void Do(Form parentWnd)
        public void Do(Form parentWnd)
        {
            // Get the user's desires (or cancel)
            var dlgDesires = new DialogExport(DB.TargetTranslation);
            if (DialogResult.OK != dlgDesires.ShowDialog(parentWnd))
                return;
           
            // Create and display the progress dialog
            DlgProgress.Start();
            DlgProgress.SetCurrentActivity(Loc.GetString("kExportSettingUp", "Setting up..."));
            DlgProgress.SetProgressMax(TotalChapterCount);

            if (!dlgDesires.CurrentExportMethod.Setup())
            {
                DlgProgress.Stop();
                return;
            }

            // Loop through all of the translation's books
            foreach(var book in m_Translation.BookList)
            {
                if (DlgProgress.UserSaysCancel)
                    break;
                UpdateProgress(book.DisplayName);

                if (dlgDesires.ExportCurrentBookOnly && book.DisplayName != DB.TargetBook.DisplayName)
                    continue;

                using (new LoadedBook(book))
                {
                    dlgDesires.CurrentExportMethod.DoExport(book);
                    DlgProgress.IncrementProgressValue(GetChapterCount(book));
                }
            }

            // Done with the progress dialog
            DlgProgress.Stop();
        }
        #endregion
    }


    /*
    // BEGIN HUICHOL FIX *******************************************************
    if (book.BookAbbrev == "EXO")
    {
        book.OneOffForHuichol_StripOutOldTranslatorNotes();
        book.DeclareDirty();
        book.WriteBook(G.CreateProgressIndicator());
    }
    // END HUICHOL FIX *********************************************************
    */

}
