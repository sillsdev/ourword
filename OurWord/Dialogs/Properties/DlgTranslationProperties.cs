using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;
using OurWordData.Styles;

namespace OurWord.Dialogs.Properties
{
    public partial class DlgTranslationProperties : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DTranslation Translation
        DTranslation Translation
        {
            get
            {
                Debug.Assert(null != m_Translation);
                return m_Translation;
            }
        }
        readonly DTranslation m_Translation;
        #endregion
        #region Attr{g/s}: bool SuppressCreateBook
        public bool SuppressCreateBook { private get; set; }
        #endregion
        #region Attr{g/s}: string LanguageName
        string LanguageName
        {
            get
            {
                return m_editLanguageName.Text;
            }
            set
            {
                m_editLanguageName.Text = value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DTranslation)
        public DlgTranslationProperties(DTranslation translation)
        {
            InitializeComponent();

            m_Translation = translation;

            // Set up the Literate Settings contents
            BuildLiterateSettings();
        }
        #endregion

        // Literate Settings: Other ----------------------------------------------------------
        #region Attr{g}: LiterateSettingsWnd LS
        LiterateSettingsWnd LS
        {
            get
            {
                return m_LiterateSettings;
            }
        }
        #endregion
        StringChoiceSetting m_WSAdvisor;
        StringChoiceSetting m_WSVernacular;
        StringChoiceSetting m_FootnoteLetterType;
        EditTextSetting m_FootnoteCustomSequence;
        #region Method: void ParseFootnoteCustomSequence()
        void ParseFootnoteCustomSequence()
        {
            var sIn = m_FootnoteCustomSequence.Value;
            var vs = sIn.Split(new[] { ' ', ',' });
            if (vs.Length == 0)
                return;

            Translation.FootnoteCustomSeq.Clear();
            foreach (var s in vs)
            {
                var sTrimmed = s.Trim();
                if (!string.IsNullOrEmpty(sTrimmed))
                    Translation.FootnoteCustomSeq.Append(sTrimmed);
            }
        }
        #endregion
        #region Method: void BuildLiterateSettings()
        void BuildLiterateSettings()
        {
            // Get the list of currently-defined writing systems
            var vsWritingSystems = new string[StyleSheet.WritingSystems.Count];
            for (var i = 0; i < StyleSheet.WritingSystems.Count; i++)
                vsWritingSystems[i] = StyleSheet.WritingSystems[i].Name;

            // Writing Systems
            var sGroupWritingSystems = G.GetLoc_String("kWritingSystems", "Writing Systems");
            LS.AddInformation("tr100", StyleSheet.LiterateHeading,
                "Writing Systems");
            LS.AddInformation("tr110", StyleSheet.LiterateParagraph,
                "Writing Systems are defined in another section of this Configuration Dialog. " +
                "They have information such as which keyboard to use when typing, autor-replace, " +
                "which letters constitute punctuation, and how to do hyphenation. Once these " +
                "have been defined, you need to tell OurWord which writing system to use for this " +
                "translation.");
            LS.AddInformation("tr120", StyleSheet.LiterateParagraph,
                "The Vernacular Writing System is the one in which the translation is done. Thus " +
                "it will be used in the right-hand column of the drafting view.");
            LS.AddInformation("tr130", StyleSheet.LiterateParagraph,
                "The Advisor Writing System is the one in which materials are typically created " +
                "for the advisor or consultant. Thus it is the one used in the right-hand column " +
                "of the back translation view.");

            m_WSVernacular = LS.AddAtringChoice(
                "trVernacular",
                "Vernacular",
                "The writing system that this translation is being drafted in.",
                Translation.VernacularWritingSystemName,
                vsWritingSystems);
            m_WSVernacular.Group = sGroupWritingSystems;

            m_WSAdvisor = LS.AddAtringChoice(
                "trAdvisor",
                "Advisor",
                "The writing system for the back translation and other materials the advisor or consultant might use.",
                Translation.ConsultantWritingSystemName,
                vsWritingSystems);
            m_WSAdvisor.Group = sGroupWritingSystems;

            // Footnotes
            var sGroupFootnotes = G.GetLoc_String("kFootnotes", "Footnotes");
            LS.AddInformation("tr200", StyleSheet.LiterateHeading,
                "Footnote Letter Sequence");
            LS.AddInformation("tr210", StyleSheet.LiterateParagraph,
                "When a footnote is encountered within the Scriptures, it typically is signaled " +
                "by a letter (such as 'a', 'b', 'c', etc.). By looking for that letter at the " +
                "bottom of the page one can quickly see the corresponding footnote paragraph.");
            LS.AddInformation("tr220", StyleSheet.LiterateParagraph,
                "OurWord defaults to the \"a, b, c, ...\" sequence, which is suitable for many " +
                "alphabets. But a great many languages will need to define a different sequence. " +
                "Thus if you cannot use one of the built-in sequences, you should enter your own.");
            LS.AddInformation("tr230", StyleSheet.LiterateParagraph,
                "First, choose either one of the built-in sequences, or indicate that you are " +
                "using a \"custom\" sequence that you will define.");

            m_FootnoteLetterType = LS.AddAtringChoice(
                "trFootnoteLetterType",
                "Letter Sequence",
                "Define the sequence of footnote callout letters. Use one of the OurWord built-in " +
                    " sequences, or set to 'custom' to define your own sequence.",
                DFoot.TypeToString(Translation.FootnoteSequenceType),
                DFoot.FootnoteSequenceChoices);
            m_FootnoteLetterType.Group = sGroupFootnotes;

            LS.AddInformation("tr240", StyleSheet.LiterateParagraph,
                "To define a custom sequence, enter its letters, with spaces in-between. This " +
                "allows a means for you to specify multi-letter combinations, e.g., the 'll' " +
                "and 'ng' in \"...j k l ll m n ng o p...\"");

            var sEditText = "";
            foreach (string s in Translation.FootnoteCustomSeq)
                sEditText += (s + ' ');
            m_FootnoteCustomSequence = LS.AddEditText(
                "trFootnoteCustom",
                "Custom Sequence",
                "If you want to use a custom sequence, set the Letter Sequence to 'custom', then " +
                    "define you sequence here, using spaces as separators.",
                sEditText);
            m_FootnoteCustomSequence.Group = sGroupFootnotes;

            LS.AddInformation("tr250", StyleSheet.LiterateParagraph,
                "If you have more footnotes than you have letters, then OurWord will just roll " +
                "over back to the beginning. E.g., after 'z' comes 'a'.");
        }
        #endregion

        // Show Filter -----------------------------------------------------------------------
        enum FilterOn { All, NewTestament, OldTestament, StartedBooks };
        FilterOn m_FilterOn = FilterOn.NewTestament;
        #region Cmd: cmdFilterOnAll
        private void cmdFilterOnAll(object sender, EventArgs e)
        {
            if (!m_radioAll.Checked) 
                return;

            m_FilterOn = FilterOn.All;
            UpdateFilter(SelectedBookAbbrev);
        }
        #endregion
        #region Cmd: cmdFilterOnNT
        private void cmdFilterOnNT(object sender, EventArgs e)
        {
            if (!m_radioNewTestament.Checked)
                return;

            m_FilterOn = FilterOn.NewTestament;

            var sSelectBook = SelectedBookAbbrev;
            if (!DBook.GetIsNewTestamentBook(SelectedBookAbbrev))
                sSelectBook = "MAT";

            UpdateFilter(sSelectBook);
        }
        #endregion
        #region Cmd: cmdFilterOnOT
        private void cmdFilterOnOT(object sender, EventArgs e)
        {
            if (!m_radioOldTestament.Checked)
                return;

            m_FilterOn = FilterOn.OldTestament;

            var sSelectBook = SelectedBookAbbrev;
            if (!DBook.GetIsOldTestamentBook(SelectedBookAbbrev))
                sSelectBook = "GEN";

            UpdateFilter(sSelectBook);
        }
        #endregion
        #region Cmd: cmdFilterOnStartedBooks
        private void cmdFilterOnStartedBooks(object sender, EventArgs e)
        {
            if (!m_radioStartedBooks.Checked) 
                return;

            m_FilterOn = FilterOn.StartedBooks;
            UpdateFilter(SelectedBookAbbrev);
        }
        #endregion
        #region Method: void UpdateFilter(sBookToSelect)
        void UpdateFilter(string sBookToSelect)
        {
            m_radioAll.Checked = (m_FilterOn == FilterOn.All);
            m_radioNewTestament.Checked = (m_FilterOn == FilterOn.NewTestament);
            m_radioOldTestament.Checked = (m_FilterOn == FilterOn.OldTestament);
            m_radioStartedBooks.Checked = (m_FilterOn == FilterOn.StartedBooks);

            // The grid will apply the filter
            PopulateGrid(sBookToSelect, true);
        }
        #endregion

        // Books Data Grid -------------------------------------------------------------------
        #region VAttr{g}: string Planned
        static string Planned
        {
            get
            {
                return Loc.GetString("Planned", "Planned");
            }
        }
        #endregion
        #region Method: void SetupStageCombo(DataGridViewComboBoxCell, sBookAbbrev)
        void SetupStageCombo(DataGridViewComboBoxCell combo, string sBookAbbrev)
        {
            Debug.Assert(null != combo);
            var book = Translation.FindBook(sBookAbbrev);

            // If we have a book, then the combo's possibilities are the translation stages
            if (null != book)
            {
                foreach (var stage in DB.TeamSettings.Stages)
                    combo.Items.Add(stage.LocalizedName);
                combo.Value = book.Stage.LocalizedName;
            }

            // Otherwise, the possibilities are Planned vs nothing
            if (null != book || Translation != DB.TargetTranslation) 
                return;
            combo.Items.Add("");
            combo.Items.Add(Planned);
            if (DB.Project.PlannedBooks.Contains(sBookAbbrev))
                combo.Value = Planned;
        }
        #endregion
        #region Method: DataGridViewRow PopulateRow(int iBook)
        DataGridViewRow PopulateRow(int iBook)
        {
            // Cannonical 3-letter abbreviation
            var sAbbrev = DBook.BookAbbrevs[iBook];

            // Book name in the target language
            var sBookName = Translation.BookNamesTable[iBook];

            // Create the row
            var row = new DataGridViewRow();
            row.CreateCells(m_gridBooks, new object[] { sAbbrev, sBookName, "" });
            row.Tag = sAbbrev;

            // Stage
            var cellCombo = row.Cells[2] as DataGridViewComboBoxCell;
            SetupStageCombo(cellCombo, sAbbrev);

            // Locked books, set to red text
            var book = Translation.FindBook(sAbbrev);
            if (null != book)
            {
                var editability = Users.Current.GetBookEditability(book);
                var color = User.TranslationSettings.GetUiColor(editability);
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.ForeColor = color;
            }

            return row;
        }
        #endregion
        #region Method: void PopulateGrid(string sBookAbbrevToSelect)
        void PopulateGrid(string sBookAbbrevToSelect, bool bHarvestExistingBookNames)
        {
            // In case book names have been changed, we want to harvest them before we
            // regenerate this. The exception comes from our CopyBookNamesFrom method,
            // where we have new names we want to place into the grid; otherwise
            // HarvestGridData would overwrite our Newly Copied Names with the 
            // previous old names.
            if (bHarvestExistingBookNames)
                HarvestGridData();

            // Clear out whatever was there
            m_gridBooks.Rows.Clear();

            // One row per book
            for (var i = 0; i < DBook.BookAbbrevsCount; i++)
            {
                var sBookAbbrev = DBook.BookAbbrevs[i];

                // If the book exists in the translation, get its stage
                var book = Translation.FindBook(sBookAbbrev);

                // Apply the Show filter
                switch (m_FilterOn)
                {
                    case FilterOn.NewTestament:
                        if (i < DBook.c_iMatthew)
                            continue;
                        break;
                    case FilterOn.OldTestament:
                        if (i >= DBook.c_iMatthew)
                            continue;
                        break;
                    case FilterOn.StartedBooks:
                        if (null == book)
                            continue;
                        break;
                }

                var row = PopulateRow(i);
                m_gridBooks.Rows.Add(row);

                // If the book doesn't exist, gray out the stage cell
                if (null == book)
                    row.Cells[2].Style.BackColor = BackColor;

                // Select this row?
                if (sBookAbbrev == sBookAbbrevToSelect)
                    row.Selected = true;
            }

            // Make sure something is selected
            if (string.IsNullOrEmpty(SelectedBookAbbrev) && m_gridBooks.Rows.Count > 0)
                m_gridBooks.Rows[0].Selected = true;
        }
        #endregion
        #region Attr{g}: string SelectedBookAbbrev
        string SelectedBookAbbrev
        {
            get
            {
                if (m_gridBooks.SelectedRows.Count != 1)
                    return null;
                return (string)m_gridBooks.SelectedRows[0].Tag;
            }
        }
        #endregion
        #region VAttr{g}: DBook SelectedBook
        DBook SelectedBook
        {
            get
            {
                var sAbbrev = SelectedBookAbbrev;
                if (!string.IsNullOrEmpty(sAbbrev))
                    return Translation.FindBook(sAbbrev);
                return null;
            }
        }
        #endregion
        #region Method: void HarvestGridData()
        void HarvestGridData()
        {
            // The DataGrid does not necessarily commit changes into its cache right away.
            // In particular, the Stage combo is not commited upon something being selected.
            // So this command forces any remaining commits to be done, so that we'll see 
            // them as we harvest.
            m_gridBooks.CommitEdit(DataGridViewDataErrorContexts.Commit);

            foreach (DataGridViewRow row in m_gridBooks.Rows)
            {
                var sAbbrev = (string)row.Cells[0].Value;
                var sName = (string)row.Cells[1].Value;
                var sStage = (string)row.Cells[2].Value;

                // Find the book in question
                var iBook = DBook.FindBookAbbrevIndex(sAbbrev);
                if (-1 == iBook)
                    continue;
                var book = Translation.FindBook(sAbbrev);

                // Update the name if not empty; if it is empty, assume user mistake and
                // don't update it.
                if (!string.IsNullOrEmpty(sName))
                {
                    if (Translation.BookNamesTable[iBook] != sName)
                    {
                        Translation.BookNamesTable[iBook] = sName;
                        Translation.DeclareDirty();
                    }
                    if (null != book && book.DisplayName != sName)
                    {
                        book.LoadBook(new NullProgress());
                        book.DisplayName = sName;
                        book.DeclareDirty();
                    }
                }

                // Update the stage
                if (null != book && !string.IsNullOrEmpty(sStage) && book.Stage.LocalizedName != sStage)
                {
                    book.LoadBook(new NullProgress());
                    book.Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.LocalizedName, sStage);
                    book.DeclareDirty();
                }

                // Update the Planned Books
                if (null == book && Translation == DB.TargetTranslation)
                {
                    var vPlanned = DB.Project.PlannedBooks;
                    if (sStage == Planned && !vPlanned.Contains(sAbbrev))
                    {
                        vPlanned.Add(sAbbrev);
                        DB.Project.PlannedBooks = vPlanned;
                    }
                    if (sStage != Planned && vPlanned.Contains(sAbbrev))
                    {
                        vPlanned.Remove(sAbbrev);
                        DB.Project.PlannedBooks = vPlanned;
                    }

                }
            }
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region cmd: cmdCopyBookNames
        private void cmdCopyBookNames(object sender, EventArgs e)
        {
            var dlg = new DlgCopyBookNames(Translation);
            if (DialogResult.Yes != dlg.ShowDialog(this))
                return;

            // We will put the source table here
            string[] vsBookNamesSource = null;

            // DTranslation as source?
            var sourceTranslation = dlg.SourceTranslation;
            if (null != sourceTranslation)
                vsBookNamesSource = sourceTranslation.BookNamesTable.GetCopy();

            // UI Language as source?
            if (null == vsBookNamesSource)
            {
                foreach (var language in LocDB.DB.Languages)
                {
                    if (dlg.SourceName != language.Name)
                        continue;
                    vsBookNamesSource = BookNames.GetTable(dlg.SourceName);
                }
            }

            // English?
            if (null == vsBookNamesSource)
            {
                if (dlg.SourceName == "English")
                    vsBookNamesSource = BookNames.English;
            }

            // Give up if still not found
            if (null == vsBookNamesSource)
                return;


            // Populate the translation
            Translation.BookNamesTable.ReplaceAll(vsBookNamesSource);

            // Update the cross references in the loaded book
            if (null != DB.Project.FrontTranslation &&
                null != DB.Project.TargetTranslation &&
                DB.Project.FrontTranslation.DisplayName == dlg.SourceName &&
                Translation == DB.Project.TargetTranslation)
            {
                DB.Project.TargetTranslation.UpdateFromFront();
            }

            // Recalculate the grid
            PopulateGrid(SelectedBookAbbrev, false);
        }
        #endregion
        #region Cmd: cmdLoad - Populate the controls
        private void cmdLoad(object sender, EventArgs e)
        {
            // Localization
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

            // Language Name
            LanguageName = Translation.DisplayName;

            // Count books present in each Testament
            var cOldTestamentBooksCount = 0;
            var cNewTestamentBooksCount = 0;
            foreach (var book in Translation.BookList)
            {
                if (book.IsOldTestamentBook)
                    cOldTestamentBooksCount++;
                else
                    cNewTestamentBooksCount++;
            }

            // If this is predonimately a NT project, then that's how we want to filter;
            // otherwise we just show all books
            if (cNewTestamentBooksCount > cOldTestamentBooksCount && cOldTestamentBooksCount < 5)
                m_radioNewTestament.Checked = true;
            else
                m_radioAll.Checked = true;

            // Populate the list of books; select the first item in the list
            PopulateGrid("GEN", false);

            // Hide the CreateBook button if requested (Front books cannot be created,
            // they can only be imported. Move the other buttons up.
            m_btnCreate.Visible = !SuppressCreateBook; 
        }
        #endregion
        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
        {
            HarvestGridData();

            // The translation's name should be a valid, non-zero-lengthed name
            if (LanguageName.Length == 0)
            {
                m_tabctrlTranslation.SelectedTab = m_tabOther;
                Messages.TranslationNeedsName();
                m_editLanguageName.Focus();
                m_editLanguageName.Select();
                e.Cancel = true;
            }
            Translation.DisplayName = LanguageName;

            // Writing Systems
            Translation.ConsultantWritingSystemName = m_WSAdvisor.Value;
            Translation.VernacularWritingSystemName = m_WSVernacular.Value;

            // Footnotes
            Translation.FootnoteSequenceType = DFoot.TypeFromString(
                m_FootnoteLetterType.Value);
            ParseFootnoteCustomSequence();
        }
        #endregion

        #region Cmd: cmdDataGridResized - adjust internal column width so we don't horizontal scroll
        private void cmdDataGridResized(object sender, EventArgs e)
        {
            const int iBookNameColumn = 1;

            // Start with our available width
            var nAvailableWidth = m_gridBooks.ClientRectangle.Width;

            // Subtract a little kludge for the borders
            nAvailableWidth -= 4;

            // Subtract the scroll bar's width
            nAvailableWidth -= SystemInformation.VerticalScrollBarWidth;

            // Subtract the widths of the columns, except for the BookNames
            // column (column[1]), which is the one we'll be setting
            for (int i = 0; i < m_gridBooks.ColumnCount; i++)
            {
                if (i != iBookNameColumn)
                    nAvailableWidth -= m_gridBooks.Columns[i].Width;
            }

            // Stick with a minimum size in case the grid gets too small
            // Of course, this will cause the horizontal scroll bar to appear
            nAvailableWidth = Math.Max(nAvailableWidth, 100);

            // Set the book-name column
            m_gridBooks.Columns[iBookNameColumn].Width = nAvailableWidth;
        }
        #endregion
        #region Cmd: cmdGridSelectionChanged
        private void cmdGridSelectionChanged(object sender, EventArgs e)
        {
            // From the abbreviation, we can see if the translation defines this book
            var book = Translation.FindBook(SelectedBookAbbrev);

            // Disable/Enable buttons accordingly
            m_btnCreate.Enabled = (book == null);
            m_btnRemove.Enabled = (book != null);
            m_btnProperties.Enabled = (book != null);
        }
        #endregion

        // Book Command Handlers -------------------------------------------------------------
        #region Cmd: cmdRemoveBook - Remove Book button clicked
        private void cmdRemoveBook(object sender, EventArgs e)
        {
            // Get the selection
            var book = SelectedBook;
            if (null == book)
                return;

            // Make sure the user wants to remove the book.
            var bRemove = LocDB.Message(
                "msgVerifyDeleteBook",
                "Do you want to delete this book from the translation?\n\n" +
                "(This will delete the file from the disk; and if you synchronize with others\n" +
                "it will remove it from their computers as well.)",
                null,
                LocDB.MessageTypes.WarningYN);
            if (!bRemove)
                return;

            // Delete the book
            try
            {
                File.Delete(book.StoragePath);
                Translation.BookList.Remove(book);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to delete book: " + ex.Message);
            }

            // Update the property page
            PopulateGrid(book.BookAbbrev, true);
        }
        #endregion
        #region Cmd: cmdBookProperties - either via Button or Double-Click
        private void cmdBookProperties(object sender, EventArgs e)
        {
            // Get the current selection
            var book = SelectedBook;
            if (null == book)
                return;

            // Make sure the book is loaded, as we can neither see nor edit the 
            // properties otherwise
            book.LoadBook(G.CreateProgressIndicator());

            // Launch the dialog
            Debug.Assert(null != Translation);
            Debug.Assert(null != Translation.Project);
            var dlg = new DBookProperties(Translation.Project.FrontTranslation, Translation, book);
            dlg.ShowDialog(this);

            // Update the property page
            PopulateGrid(book.BookAbbrev, true);
        }
        #endregion
        #region Cmd: cmdCreateBook
        private void cmdCreateBook(object sender, EventArgs e)
        {
            // Get the book we wish to create from the selected row
            var sAbbrev = SelectedBookAbbrev;
            if (string.IsNullOrEmpty(sAbbrev))
                return;
            var sBookName = DBook.GetBookName(sAbbrev, Translation);

            // Make sure the book exists in the Front
            var bFront = DB.FrontTranslation.FindBook(sAbbrev);
            if (null == bFront)
            {
                LocDB.Message("msgNoCorrespondingBookInFront",
                    "OurWord uses the Source translation as a template when it creates a book\n" +
                    "in the Target. Therefore, you need to first import {0} into\n" +
                    "{1}, before you will be able to create it here in {2}.",
                    new[] { sBookName, DB.FrontTranslation.DisplayName, Translation.DisplayName },
                    LocDB.MessageTypes.Error);
                return;
            }

            // Get confirmation as a courtesy to the user
            var bProceed = LocDB.Message("msgConfirmCreateBook",
                "Do you want OurWord to create a blank book for drafting {0} into {1}?",
                new[] { sBookName, Translation.DisplayName },
                LocDB.MessageTypes.YN);
            if (!bProceed)
                return;

            // Create the book, with "Drafting" defaults
            var book = new DBook(sAbbrev);
            var iBook = DBook.FindBookAbbrevIndex(sAbbrev);
            if (-1 == iBook)
                return;
            book.DisplayName = Translation.BookNamesTable[iBook];

            Translation.AddBook(book);
            Cursor.Current = Cursors.WaitCursor;
            if (false == book.InitializeFromFrontTranslation(G.CreateProgressIndicator()))
            {
                Translation.BookList.Remove(book);
                Cursor.Current = Cursors.Default;
                return;
            }
            book.WriteBook(G.CreateProgressIndicator());
            Cursor.Current = Cursors.Default;
            PopulateGrid(sAbbrev, true);
        }
        #endregion
        #region Cmd: cmdEditRawFile
        private void cmdEditRawFile(object sender, EventArgs e)
        {
            if (null == SelectedBook)
                return;

            // Put up the dialog and allow the user to edit
            var dlg = new DlgRawFileEdit(SelectedBook);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            // If they did edit, we must unload the book so that it will be re-loaded
            SelectedBook.Unload(new NullProgress());
        }
        #endregion

        // Import Book -----------------------------------------------------------------------
        #region Method: bool DeleteExistingBook(sBookAbbrev, sImportPath)
        private bool DeleteExistingBook(string sBookAbbrev, string sImportPath)
            // Returns false if the user Cancels, not wanting to erase an existing book
        {
            // Does the book already exist in the translation?
            var existingBook = Translation.FindBook(sBookAbbrev);
            if (null == existingBook)
                return true;

            // If the user says Cancel, then return False to abort the Import operation
            if (!Messages.VerifyOverwriteBook())
                return false;

            // Unload the book if currently loaded
            existingBook.IsDirty = false;
            existingBook.Unload(G.CreateProgressIndicator());

            // Delete the book
            if (existingBook.StoragePath != sImportPath && File.Exists(existingBook.StoragePath))
                File.Delete(existingBook.StoragePath);

            // Remove it from the translation's list
            Translation.BookList.Remove(existingBook);
            return true;
        }
        #endregion
        #region Method: bool ConvertFromStandardFormat(DBook book, ref string sPath)
        private bool ConvertFromStandardFormat(DBook book, ref string sPath)
        {
            // We currently recognize either Oxes or Standard Format. We trust the file's
            // extension to tell us what the file is. (Probably dumb idea, but seems to work.)
            var sExtension = Path.GetExtension(sPath).ToLowerInvariant();
            var bIsOxes = (sExtension == ".oxes");
            if (bIsOxes)
                return true;

            // Do the import. If the book is not considered loaded, then the user ran into
            // format problems and aborted.
            Debug.Assert(!book.Loaded);
            book.LoadFromStandardFormat(sPath);
            if (!book.Loaded)
            {
                Translation.BookList.Remove(book);
                return false;
            }

            // If here, the Import was successful. We need to write it out to Oxes.
            book.DisplayName = book.BookName;
            book.DeclareDirty();  // Make certain this will be written to file
            book.WriteBook(G.CreateProgressIndicator());
            sPath = book.StoragePath;

            return true;
        }
        #endregion
        #region SMethod: void UnloadBooks(DBook book)
        static void UnloadBooks(DBook book)
        {
            // Unload the book, so that normal Oxes read process can take care of it;
            // this makes certain error checking happen
            if (book.Loaded)
                book.Unload(G.CreateProgressIndicator());

            // If the book is a Front book, then we must unload its corresponding target
            // book as well, so that structural checking happens when next loaded
            if (book.IsFrontBook)
            {
                var targetBook = DB.TargetTranslation.FindBook(book.BookAbbrev);
                if (null != targetBook && targetBook.Loaded)
                    targetBook.Unload(G.CreateProgressIndicator());
            }
        }
        #endregion
        #region Cmd: cmdImportBook
        private void cmdImportBook(object sender, EventArgs e)
        {
            // Show the wizard; the user will input the needed information and
            // indicate whether or not to proceed.
            var wizard = new WizImportBook.WizImportBook(Translation);
            if (DialogResult.OK != wizard.ShowDialog(this))
                return;
            var sImportPath = wizard.ImportFileName;

            // Delete the book/file if it already exists; abort if user cancels
            if (!DeleteExistingBook(wizard.BookAbbrev, sImportPath))
                return;

            // Create a new book object and add it to the translation
            var book = new DBook(wizard.BookAbbrev);
            Translation.AddBook(book);

            // Convert from Standard Format if necessary
            if (!ConvertFromStandardFormat(book, ref sImportPath))
                return;

            // For an Oxes file, we just copy the file to where it belongs if
            // it is not already there
            if (sImportPath != book.StoragePath)
                File.Copy(sImportPath, book.StoragePath);

            // Unload so that error checking can happen on proper Load operation
            UnloadBooks(book);

            // Update the property page
            PopulateGrid(book.BookAbbrev, true);
        }
        #endregion
    }
}
