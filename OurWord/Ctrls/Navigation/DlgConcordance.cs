using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWord.Dialogs;
using OurWordData.DataModel;

namespace OurWord.Ctrls.Navigation
{
    public partial class DlgConcordance : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region attr{g/s}: bool IgnoreCase
        bool IgnoreCase
        {
            get
            {
                return m_checkIgnoreCase.Checked;
            }
            set
            {
                m_checkIgnoreCase.Checked = value;
            }
        }
        #endregion
        #region attr{g/s}: bool CurrentBookOnly
        bool CurrentBookOnly
        {
            get
            {
                return m_checkCurrentBookOnly.Checked;
            }
            set
            {
                m_checkCurrentBookOnly.Checked = value;
            }
        }
        #endregion
        #region attr{g}: string ConcordOnText
        string ConcordOnText
        {
            get
            {
                return m_tConcordOn.Text;
            }
        }
        #endregion
        public GoToLookupItem OnGoToLookupItem;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgConcordance()
        {
            InitializeComponent();
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Our first launch, no concordance exists, so disable the control
            m_List.Enabled = false;

            // Default options
            IgnoreCase = true;
            CurrentBookOnly = false;
            ResetOccurrences();

            // Unless there is text to concord on, disable the button
            m_btnBuildConcordance.Enabled = false;

            // Resize the final column to fill up all available width
            cmdListViewSizeChanged(null, null);
        }
        #endregion
        #region Cmd: cmdListViewSizeChanged - resize final column
        private void cmdListViewSizeChanged(object sender, EventArgs e)
            // Make the final column take up all available room
        {
            // Get the width of the preceeding columns
            var nWidth = 0;
            for (var i = 0; i < m_List.Columns.Count - 1; i++)
                nWidth += m_List.Columns[i].Width;

            // The desired final column width is the client width less what
            // the other column(s) have taken up.
            var nFinalColumnWidth = m_List.ClientRectangle.Width - nWidth;
            var colFinal = m_List.Columns[m_List.Columns.Count - 1];
            colFinal.Width = nFinalColumnWidth;
        }
        #endregion
        #region Cmd: cmdConcordOnTextChanged - Enable 'Build' button
        private void cmdConcordOnTextChanged(object sender, EventArgs e)
        {
            // Enable the Build button if there is text to concord upon
            m_btnBuildConcordance.Enabled = !string.IsNullOrEmpty(m_tConcordOn.Text);

            AcceptButton = (m_btnBuildConcordance.Enabled) ? m_btnBuildConcordance : null;
        }
        #endregion
        #region Cmd: cmdClose
        private void cmdClose(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion
        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
            // Don't close, just hide.
        {
            e.Cancel = true;
            Hide();
        }
        #endregion

        // Building the concordance ----------------------------------------------------------
        #region CLASS: ConcordanceGroup
        class ConcordanceGroup : List<LookupInfo>
        {
            readonly string BookAbbev;
            readonly string BookDisplayName;
            #region Constructor(DBook)
            public ConcordanceGroup(DBook book)
            {
                BookAbbev = book.BookAbbrev;
                BookDisplayName = book.DisplayName;
            }
            #endregion
            #region Method: void AddToListView(ListView)
            public void AddToListView(ListView lv)
            {
                // Create and add the group
                var sGroupHeader = string.Format("{0} - {1}", BookAbbev, BookDisplayName);
                var group = new ListViewGroup(sGroupHeader);
                lv.Groups.Add(group);

                // Create the individual items
                var v = new ListViewItem[Count];
                for (var i = 0; i < Count; i++)
                    v[i] = this[i].ToListViewItem(group);

                // Add the items to the listview
                lv.Items.AddRange(v);
            }
            #endregion
        }
        #endregion

        #region smethod: void StartProgressDialog()
        static void StartProgressDialog()
        {
            var sTitle = Loc.GetString("kBuildConcordanceProgress_DlgTitle", 
                "Building Concordance");
            var sExplanation = Loc.GetString("kBuildConcordanceProgress_Explanation",
                "Please wait while OurWord scans the books in your translation.");
            DlgProgress.Start();
            DlgProgress.SetTitle(sTitle);
            DlgProgress.SetExplanation(sExplanation);
            DlgProgress.SetProgressMax(Scanner.GetChapterCount());
        }
        #endregion
        #region smethod: void UpdateProgressMessage(sDisplayName)
        static void UpdateProgressMessage(string sDisplayName)
        {
            var sBase = Loc.GetString("kBuildConcordanceProgress_Status", 
                "Scanning {0}...");
            var sStatus = string.Format(sBase, sDisplayName);
            DlgProgress.SetCurrentActivity(sStatus);
        }
        #endregion

        #region method: void cmdBuildConcordance(object sender, EventArgs e)
        private void cmdBuildConcordance(object sender, EventArgs e)
        {
            StartProgressDialog();

            // Start with a clear list
            m_List.Items.Clear();
            m_List.Groups.Clear();
            m_List.Enabled = false;
            ResetOccurrences();

            var context = new Scanner.SearchContext(ConcordOnText, null) {
                IgnoreCase = IgnoreCase
            };

            // Count how many books had hits
            var cBooks = 0;

            // Scan each book
            foreach(var book in DB.TargetTranslation.BookList)
            {
                // Abort if user grows weary of waiting
                if (DlgProgress.UserSaysCancel)
                    break;
                UpdateProgressMessage(book.DisplayName);

                // Skip other books if user only wants the current book
                if (CurrentBookOnly && book.DisplayName != DB.TargetBook.DisplayName)
                    continue;

                // Scan the book
                using (new LoadedBook(book))
                {
                    var vHits = Scanner.ScanBook(context, book);

                    if (vHits.Count > 0)
                    {
                        cBooks++;
                        var group = new ConcordanceGroup(book);
                        group.AddRange(vHits);
                        group.AddToListView(m_List);
                    }

                    DlgProgress.IncrementProgressValue(Scanner.GetChapterCount(book));
                }
            }

            // Done
            if (m_List.Items.Count > 0)
                m_List.Enabled = true;
            SetOccurrences(cBooks, m_List.Items.Count);
            DlgProgress.Stop();
        }
        #endregion

        // Displaying the concordance --------------------------------------------------------
        #region Cmd: cmdDrawSubItem
        private void cmdDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // Only interested in the Scripture subitem
            if (e.ColumnIndex != 1)
            {
                e.DrawDefault = true;
                return;
            }

            // Colors
            var colorText = (e.Item.Selected) ? SystemColors.HighlightText : SystemColors.MenuText;
            var colorConcordOn = (e.Item.Selected) ? Color.Pink : Color.Red;
            var colorBackground = (e.Item.Selected) ? SystemColors.Highlight : SystemColors.Window;

            // Selection
            if (e.Item.Selected)
                e.Graphics.FillRectangle(new SolidBrush(colorBackground), e.Bounds);

            // Get the Info for what we want to draw
            var info = e.Item.Tag as LookupInfo;
            Debug.Assert(null != info);

            // Get the before and after
            var sLeft = (info.IndexIntoText > 0) ?
                info.Text.Substring(0, info.IndexIntoText) : "";
            var sMiddle = info.Text.Substring(info.IndexIntoText, ConcordOnText.Length);
            var sRight = (info.IndexIntoText + ConcordOnText.Length < info.Text.Length) ?
                info.Text.Substring(info.IndexIntoText + ConcordOnText.Length) : "";

            // Measure the target string
            var nMiddleWidth = JWU.MeasureTextDisplayWidth(ConcordOnText, e.Graphics, Font);

            // How much width for Left and Right?
            var nTotalWidth = e.Bounds.Width;
            var nWidthSide = (nTotalWidth - nMiddleWidth) / 2;

            // Chop off Left until it fits
            while (JWU.MeasureTextDisplayWidth(sLeft, e.Graphics, Font) > nWidthSide)
                sLeft = sLeft.Substring(1);

            // Draw
            var x = e.Bounds.X + nWidthSide - JWU.MeasureTextDisplayWidth(sLeft, e.Graphics, Font);
            var y = e.Bounds.Y;
            e.Graphics.DrawString(sLeft, Font, new SolidBrush(colorText), x, y);
            x += JWU.MeasureTextDisplayWidth(sLeft, e.Graphics, Font);
            e.Graphics.DrawString(sMiddle, Font, new SolidBrush(colorConcordOn), x, y);
            x += JWU.MeasureTextDisplayWidth(sMiddle, e.Graphics, Font);
            e.Graphics.DrawString(sRight, Font, new SolidBrush(colorText), x, y);

            e.DrawDefault = false;
        }
        #endregion
        #region Cmd: cmdDrawColumnHeader
        private void cmdDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }
        #endregion
        #region method: void SetOccurrences(cBooks, cTotalHits)
        void SetOccurrences(int cBooks, int cTotalHits)
        {
            var sBase = Loc.GetString("kBuildConcordanceProgress_Occurrences",
                "Found {0} occurrences of {1} in {2} book(s).");
            m_labelOccurences.Text = string.Format(sBase, cTotalHits, ConcordOnText, cBooks);
        }
        #endregion
        #region method: void ResetOccurrences()
        void ResetOccurrences()
        {
            m_labelOccurences.Text = "";
        }
        #endregion

        // Interactions with the concordance -------------------------------------------------
        #region Cmd: cmdListDoubleClicked
        private void cmdListDoubleClicked(object sender, EventArgs e)
        {
            if (m_List.SelectedItems.Count != 1)
                return;
            var item = m_List.SelectedItems[0];

            var info = item.Tag as LookupInfo;
            if (null == info)
                return;

            if (null == OnGoToLookupItem) 
                return;

            var bSelectionSucessfullyMade = OnGoToLookupItem(info);
            if (!bSelectionSucessfullyMade)
                DisplayNotFoundMessage(info);
        }
        #endregion
        #region smethod:  void DisplayNotFoundMessage(LookupInfo)
        static void DisplayNotFoundMessage(LookupInfo ci)
        {
            LocDB.Message("kBuildConcordanceProgress_NotFoundMessage", 
                "OurWord could not make the selection at {0}:{1},\n" +
                "it is likely the underlying text has been edited.\n\n" +                
                "Perhaps you need to rebuild the Concordance?", 
                new[] {ci.Chapter.ToString(), ci.Verse.ToString() }, 
                LocDB.MessageTypes.Info);
        }
        #endregion
    }

}
