#region *** DlgAdvancedFind ***
using System;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
#endregion

namespace OurWord.Ctrls.Navigation
{
    public partial class DlgAdvancedFind : Form
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

        public GoToLookupItem OnGoToLookupItem;

        // Scaffolding -----------------------------------------------------------------------
        private readonly JW_WindowState m_WindowState;
        #region Constructor()
        public DlgAdvancedFind()
        {
            InitializeComponent();
            m_ctrlFindOptions.OnOptionsChanged += CreateSearchContext;

            m_WindowState = new JW_WindowState(this, false, "AdvancedFind");
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

            if (LocDB.IsInitialized)
            {
                LocDB.Localize(this, new Control[] {});
                m_ctrlFindOptions.LocalizeAndInitialize();
            }
        }
        #endregion
        #region event: onFormClosing
        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            m_WindowState.SaveWindowState();

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

            SetNextButtonEnabling();
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
                // So that the next search will start from the current selection, which
                // might have a different result.
                CreateSearchContext();
                return;
            }

            // Go to the selection
            m_bWasFound = true;
            OnGoToLookupItem(lookupInfo);
        }
        #endregion
    }
}

