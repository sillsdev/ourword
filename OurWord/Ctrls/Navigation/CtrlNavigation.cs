using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWord.Dialogs;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Ctrls.Navigation
{
    // TODO: Turn off Filters on GoToBook / GoToChapter

    public delegate void GoToBookHandler(string sBookAbbrev);
    public delegate void GoToChapterHandler(int nChapterNumber);
    public delegate void GoToSectionHandler(DSection section);
    public delegate void FindText(string sSearchText, bool bFindNext);

    public partial class CtrlNavigation : UserControl
    {
        // Scaffolding -----------------------------------------------------------------------
        private CtrlFindText m_FindTextMenuItem;
        #region Constructor()
        public CtrlNavigation()
        {
            InitializeComponent();
            m_vSections = new List<DSection>();
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            Height = TopTools.Height + BottomTools.Height;

            m_FindTextMenuItem = new CtrlFindText();
            m_FindTextMenuItem.Click += cmdFindText;
            m_FindTextMenuItem.OnFindTextChanged += cmdFindTextChanged;
            m_Find.DropDownItems.Insert(0, m_FindTextMenuItem);

            SetMenuEnabling(false);
        }
        #endregion
        #region method: void SetMenuEnabling(bEnable)
        void SetMenuEnabling(bool bEnable)
        {
            m_FindNext.Enabled = bEnable;
            m_Filter.Enabled = bEnable;
        }
        #endregion
        #region cmd: cmdFindTextChanged(sNewText)
        void cmdFindTextChanged(string sNewText)
        {
            var bEnableMenuItems = !string.IsNullOrEmpty(sNewText);
            SetMenuEnabling(bEnableMenuItems);
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Method: void Setup(DSection)
        public void Setup(DSection currentSection)
        {
            if (null == currentSection)
                return;

            Debug.Assert(currentSection.IsTargetSection);

            m_CurrentSection = currentSection;

            // Temporary: Disable the Find button until we implement it
            m_Find.Available = false;
            m_Separator.Available = false;

            // Localize prior to setting up the dropdowns
            Localize();

            SetupBookCtrl();
            SetupChapterCtrl();
            SetupVerse();
            SetupSections();
            SetupEnabling();
            SetupIcons();
            SetupLocked();
        }
        #endregion

        // Setup -----------------------------------------------------------------------------
        #region method: void Localize()
        void Localize()
        {
            // Clear out any dropdown section titles so that we don't attempt to localize them
            m_Previous.DropDownItems.Clear();
            m_Next.DropDownItems.Clear();

            LocDB.Localize(BottomTools);
        }
        #endregion
        #region method: void SetupBookCtrl()
        void SetupBookCtrl()
        {
            // Title
            const int c_nMaxLength = 23;
            var sTitle = CurrentSection.Book.DisplayName;
            if (sTitle.Length > c_nMaxLength)
                sTitle = sTitle.Substring(0, c_nMaxLength - 2) + "...";
            m_Book.Text = sTitle;

            // Tool Tip Text
            m_Book.ToolTipText = CurrentSection.Book.DisplayName;

            // Populate Dropdown
            var vBooks = DB.Project.Nav.PotentialTargetBooks;
            SetupGotoBookDropDown(vBooks);

            // Enabling
            m_Book.Enabled = DB.IsValidProject;
        }
        #endregion
        #region method: void SetupChapterCtrl()
        void SetupChapterCtrl()
        {
            var sChapter = CurrentSection.ReferenceSpan.Start.Chapter.ToString();
            m_Chapter.Text = sChapter + @":";

            m_Chapter.Enabled = DB.IsValidProject;
        }
        #endregion
        #region method: void SetupVerse()
        void SetupVerse()
        {
            var span = CurrentSection.ReferenceSpan;
            var sFirst = span.Start.Verse.ToString();
            var sEnd = span.End.Verse.ToString();
            m_Verse.Text = string.Format("{0}-{1}", sFirst, sEnd);
        }
        #endregion
        #region method: void SetupEnabling()
        void SetupEnabling()
        {
            var i = IndexOfCurrentSection;

            var bIsAtFirst = (i == 0);
            m_First.Enabled = !bIsAtFirst;
            m_Previous.Enabled = !bIsAtFirst;

            var bIsAtLast = (i == Sections.Count - 1);
            m_Next.Enabled = !bIsAtLast;
            m_Last.Enabled = !bIsAtLast;
        }
        #endregion
        #region method: void SetupIcons()
        void SetupIcons()
        {
            const int c_iIconFirst = 0;
            const int c_iIconPrevious = 1;
            const int c_iIconNext = 2;
            const int c_iIconLast = 3;
            var imageList = (m_bFilterIsActive) ? FilteredNavigation : UnfilteredNavigation;

            m_First.Image = imageList.Images[c_iIconFirst];
            m_Previous.Image = imageList.Images[c_iIconPrevious];
            m_Next.Image = imageList.Images[c_iIconNext];
            m_Last.Image = imageList.Images[c_iIconLast];
        }
        #endregion
        #region method: void SetupLocked()
        void SetupLocked()
        {
            var bIsDisplayableProject = (null != DB.Project && DB.Project.HasDataToDisplay);

            if (!bIsDisplayableProject)
            {
                m_Locked.Available = false;
                return;
            }

            m_Locked.Available = Users.Current.GetBookEditability(DB.TargetBook) != 
                User.TranslationSettings.Editability.Full;
        }
        #endregion
        #region cmd: cmdFindDropDownOpening
        private void cmdFindDropDownOpening(object sender, EventArgs e)
        {

        }
        #endregion

        // Book Dropdown ---------------------------------------------------------------------
        #region method: ToolStripMenuItem CreateBookMenuItem(DBook)
        ToolStripMenuItem CreateBookMenuItem(DBook book)
        {
            // Create the menu item, showing the book's DisplayName
            var item = new ToolStripMenuItem(book.DisplayName, null, cmdGoToBook)
            {
                Name = "menu" + book.BookAbbrev,
                Tag = book.BookAbbrev,
                Font = SystemFonts.DialogFont
            };

            // For a restricted book, write its text as a different color
            var editability = Users.Current.GetBookEditability(book);
            item.ForeColor = User.TranslationSettings.GetUiColor(editability);

            return item;
        }
        #endregion
        #region method: void SetupGotoBookDropDown_Flat(vBooks)
        void SetupGotoBookDropDown_Flat(IEnumerable<DBook> vBooks)
        {
            m_Book.DropDownItems.Clear();

            foreach (var book in vBooks)
            {
                var item = CreateBookMenuItem(book);
                m_Book.DropDownItems.Add(item);
            }
        }
        #endregion
        #region smethod: int CountTargetBooksInThisGrouping(vBooks, BookGroup group)
        static int CountTargetBooksInThisGrouping(IEnumerable<DBook> vBooks, BookGroup group)
        {
            var c = 0;
            foreach (var book in vBooks)
            {
                var bookInfo = G.BookGroups.FindBook(book.BookAbbrev);
                if (bookInfo.Group == group)
                    c++;
            }
            return c;
        }
        #endregion
        #region method: ToolStripMenuItem FindOrAddBookGroupNode(BookGroup)
        ToolStripMenuItem FindOrAddBookGroupNode(BookGroup group)
        {
            foreach(ToolStripMenuItem item in m_Book.DropDownItems)
            {
                if (group.LocalizedName == item.Text)
                    return item;
            }

            // If here, we  must create it
            var itemGrouping = new ToolStripMenuItem(group.LocalizedName)
            {
                Name = "menu" + group.EnglishName,
                Font = SystemFonts.DialogFont
            };
            m_Book.DropDownItems.Add(itemGrouping);
            return itemGrouping;
        }
        #endregion
        #region method: void SetupGotoBookDropDown_Hierarchical(vBooks)
        private void SetupGotoBookDropDown_Hierarchical(IEnumerable<DBook> vBooks)
        {
            m_Book.DropDownItems.Clear();

            foreach (var book in vBooks)
            {
                // Create the menu item
                var item = CreateBookMenuItem(book);

                // Retrieve the hierchical information about the book
                var bookInfo = G.BookGroups.FindBook(book.BookAbbrev);

                // If the book is in a group of one (e.g., Acts), or
                // If there are two or less target books in the group,
                // then add it to the top level
                if (bookInfo.Group.Count == 1 || 
                    CountTargetBooksInThisGrouping(vBooks, bookInfo.Group) < 3)
                {
                    m_Book.DropDownItems.Add(item);
                    continue;
                }

                // Find or Create a node for this group
                var parentItem = FindOrAddBookGroupNode(bookInfo.Group);
                parentItem.DropDownItems.Add(item);
            }
        }
        #endregion
        #region method: void SetupGotoBookDropDown(vBooks)
        public void SetupGotoBookDropDown(ICollection<DBook> vBooks)
            // This is Public so we can test it.
            //
            // How many books do we have? (If too few, we will not want to nest with subitems)
        {
            const int cMinBooksRequiredToActivitateGroupings = 12;

            if (vBooks.Count < cMinBooksRequiredToActivitateGroupings)
                SetupGotoBookDropDown_Flat(vBooks);
            else
                SetupGotoBookDropDown_Hierarchical(vBooks);
        }
        #endregion

        // Sections --------------------------------------------------------------------------
        #region attr{g}: DSection CurrentSection
        private DSection CurrentSection
        {
            get
            {
                Debug.Assert(null != m_CurrentSection || !Visible, 
                    "Caller must initialize to a section or else hide the control");
                return m_CurrentSection;
            }
        }
        private DSection m_CurrentSection;
        #endregion
        #region attr{g}: List<DSection> Sections
        List<DSection> Sections
        {
            get
            {
                Debug.Assert(null != m_vSections, "Should be initialized in constructor");
                return m_vSections;
            }
        }
        private readonly List<DSection> m_vSections;
        #endregion
        #region method: void SetupSections()
        void SetupSections()
        {
            Sections.Clear();

            var vAllSectionsInBook = CurrentSection.Book.Sections;
            foreach (DSection section in vAllSectionsInBook)
            {
                if (m_bFilterIsActive)
                {
                    var bPassed = G.App.FilterTest(m_dlgFilter, section);
                    if (!bPassed)
                        continue;
                }
                
                Sections.Add(section);
            }

            SetupPreviousDropDown();
            SetupNextDropDown();
        }
        #endregion

        #region method: void SetupPreviousDropDown()
        void SetupPreviousDropDown()
        {
            m_Previous.DropDownItems.Clear();

            if (IndexOfCurrentSection <= 0) 
                return;

            var nBefore = IndexOfCurrentSection;
            var vSections = Sections.GetRange(0, nBefore);
            var vItems = CreateDropDownItems(vSections);
            m_Previous.DropDownItems.AddRange(vItems.ToArray());
        }
        #endregion
        #region method: void SetupNextDropDown()
        void SetupNextDropDown()
        {
            m_Next.DropDownItems.Clear();

            if (IndexOfCurrentSection >= Sections.Count - 1) 
                return;

            var nAfter = Sections.Count - IndexOfCurrentSection - 1;
            var vSections = Sections.GetRange(IndexOfCurrentSection + 1, nAfter);
            var vItems = CreateDropDownItems(vSections);
            m_Next.DropDownItems.AddRange(vItems.ToArray());
        }
        #endregion
        #region method: List<ToolStripMenuItem> CreateDropDownItems(vSections)
        List<ToolStripMenuItem> CreateDropDownItems(IEnumerable<DSection> vSections)
            // Build a list of menu items; we do this separately for performance reasons. AddRange is much
            // faster for a dropdown button than just adding one at a time.
        {
            var vItems = new List<ToolStripMenuItem>();
            foreach (var section in vSections)
            {
                var sMenuText = ComputeSectionMenuName(section);
                var item = new ToolStripMenuItem(sMenuText, null, cmdGoToSection, sMenuText)
                {
                    Tag = section
                };
                vItems.Add(item);
            }
            return vItems;
        }
        #endregion
        #region SMethod: string ComputeSectionMenuName(section)
        static string ComputeSectionMenuName(DSection section)
        {
            // Keep sub-menus from getting too long
            const int c_cMaxMenuLength = 60;

            // Get the Chapter:Verse reference
            var frontSection = section.CorrespondingFrontSection;
            var sReference = frontSection.ReferenceSpan.Start.FullName;

            // Get a section Title; if there's nothing in the target, then use the front's title
            var sTitle = section.Title.Trim();
            if (string.IsNullOrEmpty(sTitle))
                sTitle = frontSection.Title;

            // Keep its length from being too long (otherwise the menu gets too cluttered.
            if (sTitle.Length > c_cMaxMenuLength)
                sTitle = (sTitle.Substring(0, c_cMaxMenuLength) + "...");

            // Build the menu name
            var sMenuText = sReference;
            if (!string.IsNullOrEmpty(sTitle))
                sMenuText += (" - " + sTitle);

            return sMenuText;
        }
        #endregion

        #region attr{g}: bool HasSections
        bool HasSections
        {
            get
            {
                return (Sections.Count > 0);
            }
        }
        #endregion
        #region attr{g}: int IndexOfCurrentSection
        int IndexOfCurrentSection
        {
            get
            {
                if (!HasSections)
                    return -1;

                return Sections.IndexOf(CurrentSection);
            }
        }
        #endregion

        // Filters ---------------------------------------------------------------------------
        private bool m_bFilterIsActive;

        //The dialog is a member variable so its settings persist throughout the OW session. 
        static DialogFilter m_dlgFilter;

        private void cmdFilter(object sender, EventArgs e)
        {
            // Get the user's pleasure
            if (null == m_dlgFilter)
                m_dlgFilter = new DialogFilter();
            var result = m_dlgFilter.ShowDialog(this);

            // If the user Canceled out, then we remove any existing filter
			if (result == DialogResult.Cancel || m_dlgFilter.NothingIsChecked)
			{
			    m_bFilterIsActive = false;
			}
			else
			{
			    m_bFilterIsActive = true;
			}

            // Calc Filtered Sections
            SetupSections();
            SetupIcons();



            // OnGoToSection if current doesn't match the filter


        }


        // Handlers --------------------------------------------------------------------------
        public GoToBookHandler OnGoToBook;
        public GoToChapterHandler OnGoToChapter;
        public GoToSectionHandler OnGoToSection;
        public FindText OnFindText;

        #region Cmd: cmdGoToBook
        private void cmdGoToBook(object sender, EventArgs e)
        {
            // Retrieve the target book from the menu item's text
            var item = (sender as ToolStripMenuItem);
            if (null == item)
                return;
            var sBookAbbrev = (string)item.Tag;
            Debug.Assert(!string.IsNullOrEmpty(sBookAbbrev));

            // Caller should load the book and then call Setup()
            // Otherwise this control will be as if nothing happened.
            if (null != OnGoToBook)
                OnGoToBook(sBookAbbrev);
        }
        #endregion
        #region Cmd: cmdGoToChapter
        private void cmdGoToChapter(object sender, EventArgs e)
        {
            // Retrieve the Chapter button
            var btn = sender as ToolStripDropDownButton;
            if (null == btn)
                return;

            // Determine the location for the popup
            var pt = new Point(btn.Bounds.X, btn.Bounds.Y + btn.Size.Height);
            pt = PointToScreen(pt);

            // What is the current chapter?
            var nCurrentChapterNo = CurrentSection.ReferenceSpan.Start.Chapter;

            // Create and display the popup
            var cm = new ChapterMenu(CurrentSection.Book, pt, nCurrentChapterNo);
            var result = cm.ShowDialog(this);
            if (DialogResult.OK != result)
                return;

            // Nothing to do if the same chapter is requested
            if (cm.Chapter == nCurrentChapterNo)
                return;

            // Caller should call Setup() to the first section containing the desired 
            // chapter number. Otherwise this control will be as if nothing happened.
            if (null != OnGoToChapter)
                OnGoToChapter(cm.Chapter);
        }
        #endregion

        #region cmd: cmdGoToFirstSection
        private void cmdGoToFirstSection(object sender, EventArgs e)
        {
            if (!HasSections)
                return;

            if (CurrentSection == Sections[0])
                return;

            if (null != OnGoToSection)
                OnGoToSection(Sections[0]);
        }
        #endregion
        #region cmd: cmdGoToLastSection
        private void cmdGoToLastSection(object sender, EventArgs e)
        {
            if (!HasSections)
                return;

            var iLast = Sections.Count - 1;
            if (CurrentSection == Sections[iLast])
                return;

            if (null != OnGoToSection)
                OnGoToSection(Sections[iLast]);
        }
        #endregion
        #region cmd: cmdGoToNextSection
        private void cmdGoToNextSection(object sender, EventArgs e)
        {
            if (!HasSections)
                return;

            var iNext = IndexOfCurrentSection + 1;
            if (iNext == Sections.Count)
                return;

            if (null != OnGoToSection)
                OnGoToSection(Sections[iNext]);
        }
        #endregion
        #region cmd: cmdGoToPreviousSection
        private void cmdGoToPreviousSection(object sender, EventArgs e)
        {
            if (!HasSections)
                return;

            var iPrevious = IndexOfCurrentSection - 1;
            if (iPrevious < 0)
                return;

            if (null != OnGoToSection)
                OnGoToSection(Sections[iPrevious]);
        }
        #endregion
        #region cmd: cmdGoToSection
        private void cmdGoToSection(object sender, EventArgs e)
        {
            var item = (sender as ToolStripMenuItem);
            Debug.Assert(null != item);

            var section = item.Tag as DSection;
            Debug.Assert(null != section);

            if (null != OnGoToSection)
                OnGoToSection(section);
        }
        #endregion

        #region cmd: cmdFindText
        void cmdFindText(object sender, EventArgs e)
        {
            if (null == OnFindText) 
                return;

            var sSearchText = m_FindTextMenuItem.SearchText;
            OnFindText(sSearchText, false);
        }
        #endregion
        #region cmd: cmdFindNext
        private void cmdFindNext(object sender, EventArgs e)
        {
              if (null == OnFindText) 
                return;

            var sSearchText = m_FindTextMenuItem.SearchText;
            OnFindText(sSearchText, true);
       
        }
        #endregion

    }
}
