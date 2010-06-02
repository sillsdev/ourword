using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWord.Dialogs;
using OurWordData.DataModel;

namespace OurWord.Ctrls.Navigation
{
    // TODO: Turn off Filters on GoToBook / GoToChapter

    public delegate void GoToBookHandler(string sBookAbbrev);
    public delegate void GoToChapterHandler(int nChapterNumber);
    public delegate void GoToSectionHandler(DSection section);

    public partial class CtrlNavigation : UserControl
    {
        // Scaffolding -----------------------------------------------------------------------
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
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Method: void Setup(DSection)
        public void Setup(DSection currentSection)
        {
            m_CurrentSection = currentSection;

            SetupBookCtrl();
            SetupChapterCtrl();
            SetupVerse();
            SetupSections();
            SetupEnabling();
            SetupIcons();
        }
        #endregion

        // Setup -----------------------------------------------------------------------------
        #region method: void SetupBookCtrl()
        void SetupBookCtrl()
        {
            // Title
            const int c_nMaxLength = 14;
            var sTitle = CurrentSection.Book.DisplayName;
            if (sTitle.Length > c_nMaxLength)
                sTitle = sTitle.Substring(0, c_nMaxLength - 2) + "...";
            m_Book.Text = sTitle;

            // Dropdown
            DBookGrouping.PopulateGotoBook(m_Book, cmdGoToBook);
        }
        #endregion
        #region method: void SetupChapterCtrl()
        void SetupChapterCtrl()
        {
            var sChapter = CurrentSection.ReferenceSpan.Start.Chapter.ToString();
            m_Chapter.Text = sChapter + @":";
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
            const int iIconFirst = 0;
            const int iIconPrevious = 1;
            const int iIconNext = 2;
            const int iIconLast = 3;
            var imageList = (m_bFilterIsActive) ? FilteredNavigation : UnfilteredNavigation;

            m_First.Image = imageList.Images[iIconFirst];
            m_Previous.Image = imageList.Images[iIconPrevious];
            m_Next.Image = imageList.Images[iIconNext];
            m_Last.Image = imageList.Images[iIconLast];
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
                // ToDo: If doesn't match filter, continue

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

            if (IndexOfCurrentSection > 0)
            {
                var nBefore = IndexOfCurrentSection;
                var vSections = Sections.GetRange(0, nBefore);
                var vItems = CreateDropDownItems(vSections);
                m_Previous.DropDownItems.AddRange(vItems.ToArray());
            }
        }
        #endregion
        #region method: void SetupNextDropDown()
        void SetupNextDropDown()
        {
            m_Next.DropDownItems.Clear(); 

            if (IndexOfCurrentSection < Sections.Count - 1)
            {
                var nAfter = Sections.Count - IndexOfCurrentSection - 1;
                var vSections = Sections.GetRange(IndexOfCurrentSection + 1, nAfter);
                var vItems = CreateDropDownItems(vSections);
                m_Next.DropDownItems.AddRange(vItems.ToArray());
            }
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
        private void cmdFilter(object sender, EventArgs e)
        {
            // Display the Filter dialog


            // Calc Sections



            // OnGoToSection if current doesn't match the filter


        }


        // Handlers --------------------------------------------------------------------------
        public GoToBookHandler OnGoToBook;
        public GoToChapterHandler OnGoToChapter;
        public GoToSectionHandler OnGoToSection;

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

    }
}
