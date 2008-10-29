/**********************************************************************************************
 * Project: OurWord!
 * File:    MergePane.cs
 * Author:  John Wimbish
 * Created: 02 Feb 2008
 * Purpose: Internals of the Tab Page for displaying the merge implementation
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.View;
using OurWord.Edit;
#endregion

namespace OurWord.Edit
{
    public partial class MergePane : UserControl
    {
        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: MergeWindow WndMerge
        public MergeWindow WndMerge
        {
            get
            {
                return m_wndMerge;
            }
        }
        MergeWindow m_wndMerge;
        #endregion
        #region VAttr{g}: bool SettingsMode
        bool SettingsMode
        {
            get
            {
                return m_btnSetupMerge.Checked;
            }
        }
        #endregion
        #region VAttr{g}: DBook MasterBook
        public DBook MasterBook
        {
            get
            {
                return G.TBook;
            }
        }
        #endregion
        #region VAttr{g}: bool ShowDiffs
        public bool ShowDiffs
        {
            get
            {
                return m_btnDiffs.Checked;
            }
        }
        #endregion

        // Controls Bundle (MergeFileInfo) ---------------------------------------------------
        #region CLASS: MergeFileInfo
        class MergeFileInfo
        {
            // Data --------------------------------------------------------------------------
            #region Attr{g}: DMergeBook MergeBook
            public DMergeBook MergeBook
            {
                get
                {
                    Debug.Assert(null != m_MergeBook);
                    return m_MergeBook;
                }
            }
            DMergeBook m_MergeBook;
            #endregion

            #region VAttr{g}: string PathName
            public string PathName
            {
                get
                {
                    return MergeBook.AbsolutePathName;
                }
            }
            #endregion
            #region VAttr{g}: string NickName
            public string NickName
            {
                get
                {
                    return MergeBook.NickName;
                }
            }
            #endregion

            // Controls ----------------------------------------------------------------------
            #region Attr{g}: Label ctrlFileName
            Label ctrlFileName
            {
                get
                {
                    Debug.Assert(null != m_labelFileName);
                    return m_labelFileName;
                }
            }
            Label m_labelFileName;
            #endregion
            #region Attr{g}: ToolTip ctrlFileNameTip
            ToolTip ctrlFileNameTip
            {
                get
                {
                    Debug.Assert(null != m_tipFileName);
                    return m_tipFileName;
                }
            }
            ToolTip m_tipFileName;
            #endregion
            #region Attr{g}: Label ctrlNickNameLabel
            Label ctrlNickNameLabel
            {
                get
                {
                    Debug.Assert(null != m_labelNickName);
                    return m_labelNickName;
                }
            }
            Label m_labelNickName;
            #endregion
            #region Attr{g}: TextBox ctrlNickNameText
            TextBox ctrlNickNameText
            {
                get
                {
                    Debug.Assert(null != m_textNickName);
                    return m_textNickName;
                }
            }
            TextBox m_textNickName;
            #endregion
            #region Attr{g}: Button ctrlRemove
            Button ctrlRemove
            {
                get
                {
                    Debug.Assert(null != m_btnRemove);
                    return m_btnRemove;
                }
            }
            Button m_btnRemove;
            #endregion

            // Attrs -------------------------------------------------------------------------
            #region Attr{g/s}: bool Visible
            public bool Visible
            {
                get
                {
                    return ctrlFileName.Visible;
                }
                set
                {
                    ctrlFileName.Visible = value;
                    ctrlNickNameLabel.Visible = value;
                    ctrlNickNameText.Visible = value;
                    ctrlRemove.Visible = value;
                }
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(parent, DMergeBook, nCtrlHeight)
            public MergeFileInfo(MergePane parent, DMergeBook book, int nCtrlHeight)
            {
                // Remember the DMergeBook
                m_MergeBook = book;

                // File Name (short)
                m_labelFileName = new Label();
                ctrlFileName.Height = nCtrlHeight;
                ctrlFileName.TextAlign = ContentAlignment.BottomLeft;
                ctrlFileName.Text = Path.GetFileName(MergeBook.AbsolutePathName);
                parent.Controls.Add(ctrlFileName);

                // File Name (full) is in the tooltip
                m_tipFileName = new ToolTip();
                ctrlFileNameTip.SetToolTip(ctrlFileName, MergeBook.AbsolutePathName);

                // Nick Name Label
                m_labelNickName = new Label();
                ctrlNickNameLabel.Height = nCtrlHeight;
                ctrlNickNameLabel.Text = "Nick Name:";
                parent.Controls.Add(ctrlNickNameLabel);

                // Nick Name Text
                m_textNickName = new TextBox();
                ctrlNickNameText.Height = nCtrlHeight;
                ctrlNickNameText.Text = MergeBook.NickName;
                m_textNickName.TextChanged += new EventHandler(cmdNickNameChanged);
                parent.Controls.Add(ctrlNickNameText);

                // ctrlRemove Button
                m_btnRemove = new Button();
                ctrlRemove.Image = JWU.GetBitmap("DeleteMergeFile.ico");
                ctrlRemove.AutoSize = true;
                ctrlRemove.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                ctrlRemove.FlatAppearance.BorderSize = 0;
                ctrlRemove.FlatStyle = FlatStyle.Flat;
                ctrlRemove.Height = nCtrlHeight;
                ctrlRemove.Tag = this;
                ctrlRemove.Click += new EventHandler(parent.cmdRemoveMergeFile);
                ToolTip tipRemove = new ToolTip();
                tipRemove.SetToolTip(ctrlRemove, "Remove this file from the Merge Window");
                parent.Controls.Add(ctrlRemove);
            }
            #endregion
            #region Method: void RemoveControls(parent)
            public void RemoveControls(MergePane parent)
            {
                parent.Controls.Remove(ctrlFileName);
                parent.Controls.Remove(ctrlNickNameLabel);
                parent.Controls.Remove(ctrlNickNameText);
                parent.Controls.Remove(ctrlRemove);
            }
            #endregion
            #region Cmd: cmdNickNameChanged
            private void cmdNickNameChanged(object sender, EventArgs e)
            {
                MergeBook.NickName = ctrlNickNameText.Text;
            }
            #endregion

            // Layout ------------------------------------------------------------------------
            const int c_xNickNameWidth = 80;
            const int c_yBetweenControls = 3;
            #region Method: void SetWidths(int nAvailableWidth)
            public void SetWidths(int nAvailableWidth)
            {
                ctrlFileName.Width = nAvailableWidth - ctrlRemove.Width;
                ctrlNickNameLabel.Width = c_xNickNameWidth;
                ctrlNickNameText.Width = nAvailableWidth - c_xNickNameWidth;

                ctrlRemove.Location = new Point(
                    ctrlFileName.Location.X + ctrlFileName.Width,
                    ctrlFileName.Location.Y);
            }
            #endregion
            #region Method: int Layout(int x, int y)
            public int Layout(int x, int y)
            {
                // First row: file name and Remove Button
                ctrlFileName.Location = new Point(x, y);
                ctrlRemove.Location = new Point(x, y);
                y += ctrlFileName.Height;
                y += c_yBetweenControls;

                // Second row: Nickname
                ctrlNickNameLabel.Location = new Point(x, y);
                ctrlNickNameText.Location = new Point(x + c_xNickNameWidth, y);
                y += Math.Max(ctrlNickNameLabel.Height, ctrlNickNameText.Height);

                return y;
            }
            #endregion

        }
        #endregion
        #region Attr{g}: MergeFileInfo MergeFileInfos
        MergeFileInfo[] MergeFileInfos
        {
            get
            {
                Debug.Assert(null != m_vMergeFileInfos);
                return m_vMergeFileInfos;
            }
        }
        MergeFileInfo[] m_vMergeFileInfos;
        #endregion
        #region Method: void AddMFI(MergeFileInfo mfi)
        void AddMFI(MergeFileInfo mfi)
        {
            MergeFileInfo[] v = new MergeFileInfo[MergeFileInfos.Length + 1];
            for (int i = 0; i < MergeFileInfos.Length; i++)
                v[i] = MergeFileInfos[i];
            v[MergeFileInfos.Length] = mfi;
            m_vMergeFileInfos = v;
        }
        #endregion
        #region Method: void RemoveMFI(MergeFileInfo mfiToRemove)
        void RemoveMFI(MergeFileInfo mfiToRemove)
        {
            mfiToRemove.RemoveControls(this);

            MergeFileInfo[] v = new MergeFileInfo[MergeFileInfos.Length - 1];
            int k = 0;
            foreach (MergeFileInfo mfi in MergeFileInfos)
            {
                if (mfi != mfiToRemove)
                    v[k++] = mfi;
            }
            m_vMergeFileInfos = v;
        }
        #endregion
        #region Method: void PopulateFromMasterBook()
        public void PopulateFromMasterBook()
        {
            // If we are already the same, then there is nothing to do (and thus we can avoid
            // reloading all of these books. Thus we want this to be "re-entrant", in that we
            // can call it many times, but it only takes action if needed.
            bool bIsSame = true;
            if (MasterBook.MergeBooks.Length != MergeFileInfos.Length)
                bIsSame = false;
            if (bIsSame)
            {
                for (int i = 0; i < MergeFileInfos.Length; i++)
                {
                    if (MergeFileInfos[i].MergeBook != MasterBook.MergeBooks[i])
                    {
                        bIsSame = false;
                        break;
                    }
                }
            }
            if (bIsSame)
                return;

            // Clear out anything previous
            foreach (MergeFileInfo mfi in MergeFileInfos)
                mfi.RemoveControls(this);
            m_vMergeFileInfos = new MergeFileInfo[0];
            LayoutControls();

            // Loop through each DMergeBook to add them back in
            foreach (DMergeBook book in MasterBook.MergeBooks)
            {
                // If we can't load the book, then we can't display it
                book.Load();
                if (!book.Loaded)
                    continue;

                // Create and add the MFI
                MergeFileInfo mfi = new MergeFileInfo(this, book, m_btnBrowse.Height);
                AddMFI(mfi);
            }

            // Build the Merge Diffs List
            MasterBook.BuildMergeDiffs();

            // Update display
            WndMerge.BuildContents();
            SetProgressStatus(-1);
            LayoutControls();
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public MergePane()
        {
            InitializeComponent();

            m_wndMerge = new MergeWindow(this);
            Controls.Add(WndMerge);

            m_btnBrowse.Width = c_xButtonWidth;
            m_btnFinished.Width = c_xButtonWidth;

            m_vMergeFileInfos = new MergeFileInfo[0];

            LayoutControls();

            SetControlsVisibility();
        }
        #endregion

        // Layout ----------------------------------------------------------------------------
        public const int c_xButtonWidth = 150;
        const int c_xControlMargin = 10;
        const int c_yBetweenGroupsOfControls = 20;
        #region Method: void SetSize(Size sz)
        public void SetSize(Size sz)
        {
            this.Size = sz;

            // Extend the toolstrip across the entire size
            m_toolstripMerge.Location = new Point(0, 0);
            m_toolstripMerge.Width = sz.Width;

            // Position the Merge Window
            WndMerge.Location = new Point(0, m_toolstripMerge.Height);
            WndMerge.SetSize(sz.Width, sz.Height - m_toolstripMerge.Height);

            // The filename controls extend across the window
            foreach (MergeFileInfo mfi in MergeFileInfos)
                mfi.SetWidths( sz.Width - (2 * c_xControlMargin) );
        }
        #endregion
        #region Method: void SetControlsVisibility()
        void SetControlsVisibility()
        {
            WndMerge.Visible = !SettingsMode;

            m_btnNextIssue.Visible = !SettingsMode;
            m_btnDiffs.Visible = !SettingsMode;
            m_Position.Visible = !SettingsMode;

            m_btnBrowse.Visible = SettingsMode;
            m_btnFinished.Visible = SettingsMode;

            for (int i = 0; i < MergeFileInfos.Length; i++)
                MergeFileInfos[i].Visible = SettingsMode;
        }
        #endregion
        #region Method: void LayoutControls()
        void LayoutControls()
        {
            // Starting position y
            int y = m_toolstripMerge.Height + c_yBetweenGroupsOfControls;

            // Loop through all of the file controls
            foreach (MergeFileInfo mfi in MergeFileInfos)
            {
                y = mfi.Layout(c_xControlMargin, y);
                y += c_yBetweenGroupsOfControls;               
            }

            // Locate the button for adding a new file
            m_btnBrowse.Location = new Point(c_xControlMargin, y);
            y += m_btnBrowse.Height;
            y += c_yBetweenGroupsOfControls;

            // Locate the button for ending the settings mode
            m_btnFinished.Location = new Point(c_xControlMargin, y);

            // Set the Widths of the controls
            SetSize(new Size(Width, Height));
        }
        #endregion
        #region Method: void SetProgressStatus(iMergePosition)
        public void SetProgressStatus(int iMergePosition)
        {
            if (-1 == iMergePosition)
            {
                m_Position.Text = "";
                return;
            }

            m_Position.Text = "Issue " + (iMergePosition + 1).ToString() + " of " +
                MasterBook.MergeDiffs.Length.ToString();
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Cmd: cmdToggleSetupMode
        private void cmdToggleSetupMode(object sender, EventArgs e)
            // The Mode depends on the checked state of the SetupMerge button
        {
            WndMerge.BuildContents();
            SetControlsVisibility();
        }
        #endregion
        #region Cmd: cmdLeaveSetupMode
        private void cmdLeaveSetupMode(object sender, EventArgs e)
        {
            m_btnSetupMerge.Checked = false;
            cmdToggleSetupMode(sender, e);
        }
        #endregion
        #region Cmd: cmdAddMergeFile
        private void cmdAddMergeFile(object sender, EventArgs e)
        {
            // Create an OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Only permit a single file to be selected
            dlg.Multiselect = false;

            // The title reinforces the instructions ("Choose the file you wish
            // to merge")
            dlg.Title = LocDB.GetValue(this, "strMergeDialogTitle",
                "Choose the file you wish to merge", null);

            // The Filter lists the possible types of files:
            // - Shoebox/Toolbox (db)
            // - Standard Format (sf)
            // - Paratext (ptx)
            // - All files (which we default to)
            dlg.Filter = DlgBookPropsRes.FindFileDlgFilter;
            dlg.FilterIndex = 4;

            // Default to the Data Root folder; if we've browsed before, go to that
            // folder we last browsed to.
            dlg.InitialDirectory = G.BrowseDirectory;

            // We cannot have a FileName because we have not browsed for anything yet
            dlg.FileName = "";

            // Do the dialog; if the user cancels, we leave things as they are
            if (DialogResult.OK != dlg.ShowDialog(this))
                return;

            // Update the Browse Directory, so that the next time we browse we'll
            // start at this point where the user navigated.
            G.BrowseDirectory = Path.GetDirectoryName(dlg.FileName);

            // Create and load the book
            DMergeBook book = new DMergeBook(MasterBook, dlg.FileName,
                Path.GetFileName(dlg.FileName));
            book.Load();
            if (!book.Loaded)
                return;

            // Add it to the MasterBook
            MasterBook.AddMergeBook(book);

            // Create and add the new MergeFileInfo object
            MergeFileInfo mfi = new MergeFileInfo(this, book, m_btnBrowse.Height);
            AddMFI(mfi);

            // Build the diff's list
            MasterBook.BuildMergeDiffs();

            // Update display
            SetProgressStatus(-1);
            LayoutControls();
        }
        #endregion
        #region Cmd: cmdRemoveMergeFile
        private void cmdRemoveMergeFile(object sender, EventArgs e)
        {
            // Retrieve the MFI to be removed, from the Button's tag field
            Button btn = sender as Button;
            Debug.Assert(null != btn);
            MergeFileInfo mfiToRemove = btn.Tag as MergeFileInfo;
            Debug.Assert(null != mfiToRemove);

            // Confirm that the user wants to remove it
            if (!LocDB.Message("msgRemoveMergeFile",
                "Remove \"{0}\" from the Merge Window?",
                new string[] { Path.GetFileName(mfiToRemove.MergeBook.AbsolutePathName) },
                LocDB.MessageTypes.YN))
            {
                return;
            }

            // Remove it from the Master Book
            mfiToRemove.MergeBook.Unload();
            MasterBook.RemoveMergeBook( mfiToRemove.MergeBook );

            // Remove it from the vector
            RemoveMFI(mfiToRemove);

            // Update display
            SetProgressStatus(-1);
            LayoutControls();
        }
        #endregion
        #region Cmd: cmdToggleDiffs
        private void cmdToggleDiffs(object sender, EventArgs e)
        {
            WndMerge.BuildContents();
        }
        #endregion
    }


    public class MergeWindow : OWWindow
    {
        #region Attr{g}: MergePane MergePane
        MergePane MergePane
        {
            get
            {
                Debug.Assert(null != m_MergePane);
                return m_MergePane;
            }
        }
        MergePane m_MergePane;
        #endregion

        #region SAttr{g/s}: string RegistryBackgroundColor - background color setting for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "Khaki");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "Merge";
        const int c_cColumnCount = 1;
        #region Constructor(MergePane)
        public MergeWindow(MergePane _MergePane)
            : base(c_sName, c_cColumnCount)
        {
            m_MergePane = _MergePane;
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("MergeWindowName", "Merge");
            }
        }
        #endregion

        // Secondary Window Messaging --------------------------------------------------------
        #region Attr{g}: DBasicText BasicText - vernacular's DBasicText on which to build wnd contents
        DBasicText BasicText
        {
            get
            {
                return m_BasicText;
            }
        }
        DBasicText m_BasicText = null;
        #endregion
        #region Method: void OnSelectionChanged(DBasicText)
        public override void OnSelectionChanged(DBasicText dbt)
        {
            // Do nothing if we're in the same DBasicText (an optimization)
            if (dbt == BasicText)
                return;

            // Make sure the pane is properly set to the correct book's merge files
            MergePane.PopulateFromMasterBook();

            // Set to the new basic text
            m_BasicText = dbt;

            // Refresh the window contents
            BuildContents();
        }
        #endregion
        #region OMethod: bool HandleLockedFromEditing()
        public override bool HandleLockedFromEditing()
        {
            TypingErrorBeep();
            return true;
        }
        #endregion

        // Shorthand -------------------------------------------------------------------------
        #region VAttr{g}: DParagraph SelectedParagraph
        DParagraph SelectedParagraph
        {
            get
            {
                if (null == BasicText)
                    return null;
                return BasicText.Paragraph;
            }
        }
        #endregion
        #region VAttr{g}: DSection SelectedSection
        DSection SelectedSection
        {
            get
            {
                DParagraph p = SelectedParagraph;
                if (null == p)
                    return null;
                return p.Section;
            }
        }
        #endregion
        #region VAttr{g}: DBook SelectedBook
        DBook SelectedBook
        {
            get
            {
                DSection section = SelectedSection;
                if (section == null)
                    return null;
                return section.Book;
            }
        }
        #endregion
        #region VAttr{g}: DTranslation SelectedTranslation
        DTranslation SelectedTranslation
        {
            get
            {
                DBook book = SelectedBook;
                if (null == book)
                    return null;
                return book.Translation;
            }
        }
        #endregion

        #region VAttr{g}: int SectionNo
        int SectionNo
        {
            get
            {
                DBook book = SelectedBook;
                if (null == book)
                    return -1;
                return book.Sections.FindObj(SelectedSection);
            }
        }
        #endregion
        #region VAttr{g}: int ParagraphNo
        int ParagraphNo
        {
            get
            {
                DSection section = SelectedSection;
                if (null == section)
                    return -1;
                return section.Paragraphs.FindObj(SelectedParagraph);
            }
        }
        #endregion
        #region VAttr{g}: int DBTNo
        int DBTNo
        {
            get
            {
                DParagraph paragraph = SelectedParagraph;
                if (null == paragraph)
                    return -1;
                return paragraph.Runs.FindObj(BasicText);
            }
        }
        #endregion

        // Build Contents --------------------------------------------------------------------
        #region Method: void BuildContents()
        public void BuildContents()
        {
            // Start with a clean slate
            Clear();

            // Do we have a valid selection?
            if (null == BasicText)
            {
                LoadData();
                return;
            }
           
            // Does this position correspond to anything in the MergeDiff's list?
            // If not, display a message and we're done.
            int iMergePosition = SelectedBook.IsInMergeDiffs(SectionNo, ParagraphNo, DBTNo);
            MergePane.SetProgressStatus(iMergePosition);
            if (-1 == iMergePosition)
            {
                StartNewRow();
                OWPara p = new OWPara(
                    this,
                    LastRow.SubItems[0] as EContainer,
                    SelectedTranslation.WritingSystemConsultant,
                    G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleAbbrevNormal),
                    "The current selection does not correspond to a merge issue.");
                AddParagraph(0, p);
                LoadData();
                return;
            }

            // Show the Scripture reference
            BuildDisplayReference();

            // An array showing whether or not the particular version has been displayed
            bool[] m_vbDisplayed = new bool[SelectedBook.MergeBooks.Length];
            for (int i = 0; i < m_vbDisplayed.Length; i++)
                m_vbDisplayed[i] = false;

            // Loop through the books
            for (int i = 0; i < SelectedBook.MergeBooks.Length; i++)
            {
                // Skip this one if we've already accounted for it
                if (m_vbDisplayed[i])
                    continue;

                // Retrieve the DBT in question
                DBasicText dbtMerge = GetDBT(SelectedBook.MergeBooks[i], 
                    SectionNo,
                    ParagraphNo, 
                    DBTNo);

                // See if there is a difference with what is in the main window
                if (dbtMerge.AsString == BasicText.AsString)
                    continue;

                // Display the titles of all files that exhibit this string
                for (int k = 0; k < SelectedBook.MergeBooks.Length; k++)
                {
                    if (m_vbDisplayed[k])
                        continue;

                    DBasicText dbtSibling = GetDBT(SelectedBook.MergeBooks[k], 
                        SectionNo,
                        ParagraphNo, 
                        DBTNo);

                    if (dbtSibling.AsString == dbtMerge.AsString)
                    {
                        StartNewRow();
                        OWPara para = new OWPara(
                            this,
                            LastRow.SubItems[0] as EContainer,
                            SelectedTranslation.WritingSystemConsultant,
                            G.StyleSheet.FindParagraphStyle(DStyleSheet.c_PStyleMergeHeader),
                            SelectedBook.MergeBooks[k].NickName);
                        AddParagraph(0, para);
                        m_vbDisplayed[k] = true;
                    }
                }

                // Display the text paragraph
                BuildTextParagraph(dbtMerge);
            }

            LoadData();
        }
        #endregion
        #region Method: BuildDisplayReference()
        void BuildDisplayReference()
        {
            DParagraph p = SelectedParagraph;

            // Collect the Chapter and Verse at the start of the paragraph
            int nChapter = BasicText.Paragraph.ReferenceSpan.Start.Chapter;
            int nVerse = BasicText.Paragraph.ReferenceSpan.Start.Verse;

            // Move through the paragraph to see if the verse increments
            foreach (DRun run in p.Runs)
            {
                if (run as DBasicText == BasicText)
                    break;

                DVerse verse = run as DVerse;
                if (null != verse)
                    nVerse = verse.VerseNo;

                DChapter chapter = run as DChapter;
                if (null != chapter)
                    nChapter = chapter.ChapterNo;
            }

            // Create the reference string
            string sBase = G.GetLoc_Merge("Paragraph {0}, Verse {1}:{2}");
            string sRef = LocDB.Insert(sBase, new string[] {
                (ParagraphNo + 1).ToString(),
                nChapter.ToString(),
                nVerse.ToString()
                });

            // Display it in the window
            StartNewRow();
            OWPara para = new OWPara(
                this,
                LastRow.SubItems[0] as EContainer,
                SelectedTranslation.WritingSystemConsultant,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_PStyleMergeParagraph),
                sRef);
            AddParagraph(0, para);
        }
        #endregion
        #region Method: void BuildTextParagraph(DBasicText dbtMerge)
        void BuildTextParagraph(DBasicText dbtMerge)
        {
            // We'll just display everything as a normal paragraph, to avoid indendation, etc.
            JParagraphStyle PStyle = G.StyleSheet.FindParagraphStyle(
                DStyleSheet.c_PStyleMergeParagraph);

            // The paragraph will be in a new row
            StartNewRow();

            // If the ShowDiffs button is checked, then we do the diff analysis
            if (MergePane.ShowDiffs)
            {
                DPhrase[] vPhrases = AnalyzeDiff(BasicText.AsString, dbtMerge.AsString);
                OWPara p = new OWPara(this,
                    LastRow.SubItems[0] as EContainer,
                    SelectedTranslation.WritingSystemVernacular, 
                    PStyle, 
                    vPhrases);
                AddParagraph(0, p);
            }

            // Otherwise, show the paragraph in a form by which text can be selected and copied,
            // but not edited
            else
            {
                OWPara p = new OWPara(
                    this,
                    LastRow.SubItems[0] as EContainer,
                    SelectedTranslation.WritingSystemVernacular,
                    PStyle,
                    new DRun[] { dbtMerge }, 
                    "", 
                    OWPara.Flags.IsEditable | OWPara.Flags.IsLocked);
                AddParagraph(0, p);
            }
        }
        #endregion
        #region Method: string AnalyzeDiff(string sMaster, string sMerge)
        DPhrase[] AnalyzeDiff(string sMaster, string sMerge)
            // from Matthias Hertel, http://www.mathertel.de, derived from his example pm
            // interpreting the results, http://www.mathertel.de/Diff/DiffTextLines.aspx
        {
            // Run the Diff routine to get an array of differences
            Diff diff = new Diff();
            int[] vMaster = Diff.DiffCharCodes(sMaster);
            int[] vMerge = Diff.DiffCharCodes(sMerge);
            Diff.Item[] vItems = Diff.DiffInt(vMaster, vMerge);

            // We'll build the array of phrases here
            ArrayList a = new ArrayList();

            int pos = 0;
            for (int n = 0; n < vItems.Length; n++)
            {
                Diff.Item it = vItems[n];

                // write unchanged chars
                if ((pos < it.StartB) && (pos < sMerge.Length))
                {
                    string sUnchanged = "";
                    while ((pos < it.StartB) && (pos < sMerge.Length))
                        sUnchanged += sMerge[pos++];
                    DPhrase pUnchanged = new DPhrase(null, sUnchanged);
                    a.Add(pUnchanged);
                }

                // write deleted chars
                if (it.deletedA > 0)
                {
                    string sDeleted = "";
                    for (int m = 0; m < it.deletedA; m++)
                        sDeleted += sMaster[it.StartA + m];
                    DPhrase pDeleted = new DPhrase(DStyleSheet.c_CStyleRevisionDeletion, sDeleted);
                    a.Add(pDeleted);
                }

                // write inserted chars
                if (pos < it.StartB + it.insertedB)
                {
                    string sAdded = "";
                    while (pos < it.StartB + it.insertedB)
                        sAdded += sMerge[pos++];
                    DPhrase pAdded = new DPhrase(DStyleSheet.c_CStyleRevisionAddition, sAdded);
                    a.Add(pAdded);
                }
            }

            // write rest of unchanged chars
            string sEnd = "";
            while (pos < sMerge.Length)
                sEnd += sMerge[pos++];
            DPhrase pEnd = new DPhrase(null, sEnd);
            a.Add(pEnd);

            // Return as a vector
            DPhrase[] vp = new DPhrase[a.Count];
            for (int i = 0; i < a.Count; i++)
                vp[i] = a[i] as DPhrase;
            return vp;
        }
        #endregion
        #region Method: DBasicText GetDBT(DBook MergeBook, nSection, nPara, nDBT)
        DBasicText GetDBT(DBook book, int nSection, int nPara, int nDBT)
        {
            DSection section = book.Sections[nSection] as DSection;
            DParagraph para = section.Paragraphs[nPara] as DParagraph;
            DBasicText DBT = para.Runs[nDBT] as DBasicText;
            return DBT;
        }
        #endregion
    }

}
