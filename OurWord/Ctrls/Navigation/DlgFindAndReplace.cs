#region *** DlgFindAndReplace ***
using System;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWordData.DataModel;
#endregion

/* Implementation Notes
 *  Replace/ReplaceAll enabling: These are initially disabled. They become enabled upon a
 *     successful 'FindNext', where the selection is now resting on a match. If either
 *     the user moves the selection, or changes the FindWhat text, then the match has
 *     become invalidated, and the buttons are consequently disabled. A FindNext is required
 *     to enable them again.

*/

namespace OurWord.Ctrls.Navigation
{
    public partial class DlgFindAndReplace : Form
    {
        // Dialog Attrs ----------------------------------------------------------------------
        #region attr{g}: string FindWhat
        public string FindWhat
        {
            private get
            {
                return m_textFindWhat.Text;
            }
            set
            {
                m_textFindWhat.Text = value;
            }
        }
        #endregion
        #region attr{g}: CtrlFindOptions Options
        CtrlFindOptions Options
        {
            get
            {
                return m_ctrlFindOptions;
            }
        }
        #endregion

        #region attr{g}: string ReplaceWith
        string ReplaceWith
        {
            get
            {
                return m_textReplaceWith.Text;
            }
        }
        #endregion
        public GoToLookupItem OnGoToLookupItem;

        // Scaffolding -----------------------------------------------------------------------
        private readonly JW_WindowState m_WindowState;
        #region Constructor()
        public DlgFindAndReplace()
        {
            InitializeComponent();
            m_ctrlFindOptions.OnOptionsChanged += CreateSearchContext;

            m_WindowState = new JW_WindowState(this, false, "FindAndReplace");
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        private Scanner.SearchContext m_Context;
        #region method: CreateSearchContext()
        void CreateSearchContext()
        {
            m_Context = new Scanner.SearchContext(FindWhat, CurrentSelection) {
                IgnoreCase = Options.IgnoreCase,
                CurrentBookOnly = Options.OnlyScanCurrentBook,
                Type = Options.Type
            };
        }
        #endregion
        private bool m_bWasFound;
        #region attr{g}: OWWindow.Sel CurrentSelection
        static OWWindow.Sel CurrentSelection
        {
            get
            {
                return G.App.CurrentLayout.Selection;
            }
        }
        #endregion
        #region method: void SetNextButtonEnabling()
        void SetNextButtonEnabling()
        {
            // The Next button is not available for an empty string
            m_btnFindNext.Enabled = !string.IsNullOrEmpty(m_textFindWhat.Text);
        }
        #endregion

        // Events ----------------------------------------------------------------------------
        #region event: onLoad
        private void onLoad(object sender, EventArgs e)
        {
            // Restore the window to its location; but don't let the WindowState place its size
            // to some stale value in the registry.
            var sz = new Size(Width, Height);
            m_WindowState.RestoreWindowState();
            Width = sz.Width;
            Height = sz.Height;

            var font = CtrlNavigation.GetFont();
            m_textFindWhat.Font = font;
            m_textReplaceWith.Font = font;

            LocDB.Localize(this, null);
            m_ctrlFindOptions.LocalizeAndInitialize();
        }
        #endregion
        #region event: onFormClosing
        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            m_WindowState.SaveWindowState();

            G.App.CurrentLayout.OnSelectionChanged -= OnMainWindowSelectionChanged;

            // If we're closing because the parent (OurWord main window) is closing, then
            // go ahead and do a normal close.
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            // Otherwise, hide the form, don't close it (which results in a Dispose) so we can
            // reuse it later.
            Hide();
            e.Cancel = true;
        }
        #endregion
        #region event: onActivated
        private void onActivated(object sender, EventArgs e)
        {
            G.App.CurrentLayout.OnSelectionChanged += OnMainWindowSelectionChanged;
        }
        #endregion

        private bool m_bPreventRecursion;

        #region event: onFindWhatChanged
        private void onFindWhatChanged(object sender, EventArgs e)
        {
            if (m_bPreventRecursion)
                return;

            // Reset the starting point to our current position, with no items found or replaced
            CreateSearchContext();
            m_bWasFound = false;

            // Process autoreplace
            m_bPreventRecursion = true;
            m_Context.WritingSystem.ProcessAutoReplace(m_textFindWhat);
            m_bPreventRecursion = false;

            // Replace Buttons are now disabled, as we have to rebuild the list before
            // it would be valid to Replace again.
            m_btnReplace.Enabled = false;
            m_btnReplaceAll.Enabled = false;

            SetNextButtonEnabling();
        }
        #endregion
        #region event: onReplaceWithChanged

        private string sPreviousReplaceWith;
        private int iPreviousSelectionStart;
        private int cPreviousSelectionLength;

        private void onReplaceWithChanged(object sender, EventArgs e)
        {
            if (m_bPreventRecursion)
                return;

            // Process autoreplace
            m_bPreventRecursion = true;
            m_Context.WritingSystem.ProcessAutoReplace(m_textReplaceWith);
            m_bPreventRecursion = false;

            // Don't allow double spaces anywhere
            if (m_textReplaceWith.Text.IndexOf("  ") != -1)
            {
                m_bPreventRecursion = true;
                m_textReplaceWith.Text = sPreviousReplaceWith;
                m_textReplaceWith.SelectionStart = iPreviousSelectionStart;
                m_textReplaceWith.SelectionLength = cPreviousSelectionLength;
                m_bPreventRecursion = false;
            }
            sPreviousReplaceWith = m_textReplaceWith.Text;
            iPreviousSelectionStart = m_textReplaceWith.SelectionStart;
            cPreviousSelectionLength = m_textReplaceWith.SelectionLength;

            // The replace buttons remain unchanged; we'll allow the user to
            // change the ReplaceWith value without rebuilding the scan list.
            SetNextButtonEnabling();
        }
        #endregion

        private bool m_bProcessSelectionChanged = true;
        #region event: OnMainWindowSelectionChanged
        private void OnMainWindowSelectionChanged(OWWindow.Sel selection)
        {
            if (!m_bProcessSelectionChanged)
                return;

            // If the selection changed, we cannot do a Replace because the text is 
            // no longer selected
            m_btnReplace.Enabled = false;
            m_btnReplaceAll.Enabled = false;
        }
        #endregion

        // Button Clicks ---------------------------------------------------------------------
        #region cmd: cmdClose
        private void cmdClose(object sender, EventArgs e)
        {
            // By calling Close, we get onFormClosing, which hides the form correctly
            Close();
        }
        #endregion
        #region cmd: cmdFindNext
        private void cmdFindNext(object sender, EventArgs e)
            // Move to the next match
            // If at end of book, ask if we should continue scanning through remaining books
            //   if there are any.
        {
            // Find the next match
            var lookupInfo = Scanner.ScanForNext(m_Context);

            // No match, tell the user and we're done
            if (null == lookupInfo)
            {
                if (m_bWasFound)
                {
                    LocDB.Message("kFindAndReplace_NoMoreMatches",
                        "There are no more occurrences of '{0}.'",
                        new[] { FindWhat },
                        LocDB.MessageTypes.Info);
                }
                else
                {
                    LocDB.Message("kFindAndReplace_NoneFound",
                        "There are no occurrences of '{0}' in the translation.",
                        new[] { FindWhat },
                        LocDB.MessageTypes.Info);                    
                }
                // Nothing to replace
                m_btnReplace.Enabled = false;
                m_btnReplaceAll.Enabled = false;
                // So that the next search will start from the current selection, which
                // might have a different result.
                CreateSearchContext();
                return;
            }

            // Go to the selection
            m_bProcessSelectionChanged = false;
            m_bWasFound = true;
            OnGoToLookupItem(lookupInfo);
            m_bProcessSelectionChanged = true;

            // The Replace buttons are ok to be used
            m_btnReplace.Enabled = true;
            m_btnReplaceAll.Enabled = true;
        }
        #endregion
        #region cmd: cmdReplace
        private void cmdReplace(object sender, EventArgs e)
        {
            // Make the replacement as an undoable action
            m_bProcessSelectionChanged = false;
            (new InsertAction("Replace", G.App.CurrentLayout, ReplaceWith)).Do();
            m_bProcessSelectionChanged = true;

            // Go to the next one
            cmdFindNext(sender, e);
        }
        #endregion
        #region cmd: cmdReplaceAll
        private void cmdReplaceAll(object sender, EventArgs e)
        {
            // Get the user's confirmation
            var bProceed = LocDB.Message("kFindAndReplace_ConfirmReplaceAll",
                "Warning: This CANNOT be undone!\n\n" +
                "You are about to replace ALL occurrences of '{0}' with '{1}'\n" +
                    "throughout your entire translation.\n\n" +
                "Are you absolutely certain you wish to proceed?",
                new[] { FindWhat, ReplaceWith },
                LocDB.MessageTypes.YN);
            if (false == bProceed)
                return;

            // Setup
            DlgProgress.Start();
            DlgProgress.SetTitle(Loc.GetString("kReplaceAllProgress_DlgTitle", "Replace All"));
            DlgProgress.SetExplanation(Loc.GetString("kReplaceAllProgress_Explanation",
                "Please wait while OurWord replaces the text throughout your translation."));
            DlgProgress.SetProgressMax(Scanner.GetChapterCount());
            var sProgressMessageBase = Loc.GetString("kReplaceAllProgress_Status", "Processing {0}...");

            var bIsBackTranslation = CurrentSelection.Paragraph.DisplayBT;

            // Transfer all changes from the editor to the data
            G.App.OnLeaveSection();

            // Scan through each book
            var countReplacements = 0;
            var countBooks = 0;
            foreach (var book in DB.TargetTranslation.BookList)
            {
                // Abort if user grows weary of waiting
                if (DlgProgress.UserSaysCancel)
                    break;
                DlgProgress.SetCurrentActivity(string.Format(sProgressMessageBase, book.DisplayName));

                using (new LoadedBook(book))
                {
                    var countReplacementsThisBook = 0;
                    var vTexts = Scanner.GetTexts(book);
                    foreach(var text in vTexts)
                    {
                        var phrases = (bIsBackTranslation) ? text.PhrasesBT : text.Phrases;
                        countReplacementsThisBook += phrases.ReplaceAll(FindWhat, ReplaceWith);
                    }
                    if (countReplacementsThisBook > 0)
                    {
                        countReplacements += countReplacementsThisBook;
                        countBooks++;
                    }
                }

                DlgProgress.IncrementProgressValue(Scanner.GetChapterCount(book));
            }

            // Rebuld the editor
            G.App.OnEnterSection();

            DlgProgress.Stop();

            m_btnReplace.Enabled = false;
            m_btnReplaceAll.Enabled = false;

            // Display summary to the user
            LocDB.Message("kFindAndReplace_Summary",
                "{0} occurrences of '{1}' were replaced with '{2}' over {3} books.",
                new[] { countReplacements.ToString(), FindWhat, ReplaceWith, countBooks.ToString() },
                LocDB.MessageTypes.Info);
        }
        #endregion
    }
}

