#region ***** DlgProperties.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgProperties.cs
 * Author:  John Wimbish
 * Created: 17 Sep 2007
 * Purpose: Edit the settings that an advisor will typically handle.
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using JWTools;
using OurWord.Dialogs.Properties;
using OurWordData.DataModel;
using OurWord.Utilities;
using OurWordData.DataModel.Membership;
#endregion

// TODO: Check over Localization of pages, especially, e.g., "Front: Kupang"

namespace OurWord.Dialogs
{
    public partial class DialogProperties : Form
    {
		// Controls --------------------------------------------------------------------------
        #region VAttr{g}: GroupedTasksList NavList
        GroupedTasksList NavList
        {
            get
            {
                Debug.Assert(null != m_NavTasks);
                return m_NavTasks;
            }
        }
        #endregion
        #region Attr{g}: CtrlPageContent PageContent
        private CtrlPageContent PageContent;
        #endregion

        // Pages -----------------------------------------------------------------------------
        #region Attr{g}: List<DlgPropertySheet> Pages
        List<DlgPropertySheet> Pages
        {
            get
            {
                Debug.Assert(null != m_Pages);
                return m_Pages;
            }
        }
        List<DlgPropertySheet> m_Pages;
        #endregion
		#region Attr{g/s}: DlgPropertySheet CurrentPage
		DlgPropertySheet CurrentPage
		{
			get
			{
				return m_CurrentPage;
			}
			set
			{
				m_CurrentPage = value;
			}
		}
		DlgPropertySheet m_CurrentPage;
		#endregion
        #region Method: bool HarvestChangesFromCurrentPage()
        public bool HarvestChangesFromCurrentPage()
        {
            if (null != CurrentPage)
                return CurrentPage.HarvestChanges();
            return true;
        }
        #endregion
        #region Method: void UpdateNavigationControls()
        public void UpdateNavigationControls()
        {
            string sGroupTitle = "";
            GroupedTasks gt = NavList.LastSelectedGroup;
            if (null != gt)
                sGroupTitle += (gt.GroupName + " - ");

            PageContent.SetTitle(sGroupTitle + CurrentPage.Title);

            if (null != NavList.LastSelectedButton)
                NavList.LastSelectedButton.Text = "  " + CurrentPage.Title;
        }
        #endregion
        #region Method: void SetActivePage(string sID)
        public void SetActivePage(string sID)
		{
            // Harvest changes from the old page; do this now, in case
            // harvesting involves regeinerating new pages
            if (null != CurrentPage)
                HarvestChangesFromCurrentPage();

            // Locate the page we want to make current
            DlgPropertySheet pageNew = null;
            foreach (var page in Pages)
            {
                if (page.ID == sID)
                    pageNew = page;
            }
            if (null == pageNew)
                return;

            PageContent.SetContent(pageNew);
            CurrentPage = pageNew;
            CurrentPage.Focus();

            // Update the Title Text
            UpdateNavigationControls();
            NavList.SelectButton(CurrentPage.ID);
		}
		#endregion
        #region Method: void AddPage(GroupedTasks gt, page, iImage)
        void AddPage(GroupedTasks gt, DlgPropertySheet page, int iImage)
        {
            Pages.Add(page);
            gt.AddTask(page.Title, page.ID, iImage);
        }
        #endregion
        #region Method: void InitNavigation(string sIdActivePage)
        public void InitNavigation(string sIdActivePage)
        {
            // Clear out anything that is already there (makes this re-entrant); since setting
            // up features, etc., can change what options are available
            m_Pages = new List<DlgPropertySheet>();
            NavList.ClearGroups();

            // Project Settings
            var groupProject = NavList.AddGroup("Project");
            AddPage(groupProject, new Page_Translations(this), c_iImageDefault);

            // Current User Settings
            var groupUser = NavList.AddGroup(Users.Current.UserName);
            AddPage(groupUser, new Page_UserOptions(this), c_iImageOptions);
            AddPage(groupUser, new Page_UserFeatures(this), c_iImageFeaturesOnOff);
            AddPage(groupUser, new Page_Notes(this), c_iImageNotes);
            AddPage(groupUser, new Page_UserEditPermissions(this), c_iImageUserEditPermissions);

            // Entire Cluster Settings
            var groupCluster = NavList.AddGroup("Cluster");
            AddPage(groupCluster, new Page_Options(this), c_iImageOptions);
            AddPage(groupCluster, new Page_StyleSheet(this), c_iImageStyleSheet);
            AddPage(groupCluster, new Page_AdvancedPrintOptions(this), c_iImageAdvancedPrint);
            AddPage(groupCluster, new Page_TranslationStages(this), c_iImageDefault);
            AddPage(groupCluster, new Page_WritingSystems(this), c_iImageWritingSystem);

            // This Computer
            var groupThisComputer = NavList.AddGroup("This Computer");
            AddPage(groupThisComputer, new Page_Cluster(this), c_iImageClusters);

            // Go to the requested page)
            if (!string.IsNullOrEmpty(sIdActivePage))
                SetActivePage(sIdActivePage);

            // Make the nav list show up correctly
            NavList.cmdLayout(null, null);
        }
        #endregion

        // Images ----------------------------------------------------------------------------
        private const int c_iImageDefault = 0;
        private const int c_iImageAdvancedPrint = 1;
        private const int c_iImageNotes = 2;
        private const int c_iImageOptions = 3;
        private const int c_iImageWritingSystem = 4;
        private const int c_iImageStyleSheet = 6;
        private const int c_iImageClusters = 7;
        private const int c_iImageFeaturesOnOff = 8;
        private const int c_iImageUserEditPermissions = 9;

		// Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DialogProperties()
        {
            InitializeComponent();

            // Set up the Tasks list
            NavList.OnItemSelected = new GroupedTasksList.ItemSelectedDel(SetActivePage);
        }
        #endregion
        #region Method: void SetTitleBarText()
        public void SetTitleBarText()
        {
            string sLanguageName = (null != DB.Project.TargetTranslation) ?
                DB.Project.TargetTranslation.DisplayName : "";

            if (!string.IsNullOrEmpty(sLanguageName))
            {
                Text = LocDB.Insert(
                    LocDB.GetValue(this, "strTitleWithLanguageName", "{0} Project Properties"),
                    new string[] { sLanguageName });
            }
            else
            {
                Text = LocDB.GetValue(this, "Title", "Project Properties");
            }
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Localization
            Control[] vExclude = { m_NavTasks };
            LocDB.Localize(this, vExclude);

            // Set the TitleBar after the localization, as we override it
            SetTitleBarText();

            // Set up the pages
            NavList.Images = m_Images;
            InitNavigation(Page_Translations.c_sID);
        }
        #endregion
        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
        {
            // We're only interested in further processing if the user has hit the OK
            // button, signallying he is done and wishes to save his results.
            if (DialogResult != DialogResult.OK)
                return;

            // Save the active pages's data
            if (!HarvestChangesFromCurrentPage())
            {
                e.Cancel = true;
                return;
            }

            // Make sure the project has a reasonable name
            if (null != DB.Project.TargetTranslation &&
                DB.Project.TargetTranslation.DisplayName.Length > 0)
            {
                DB.Project.DisplayName = DB.Project.TargetTranslation.DisplayName;
            }
            else
                DB.Project.DisplayName = "My Project";

            // Go ahead and close
            return;
        }
        #endregion
        #region Cmd: cmdHelp
        private void cmdHelp(object sender, EventArgs e)
        {
            CurrentPage.ShowHelp();
        }
        #endregion

	}

    #region CLASS: DlgPropertySheet - Provides stubs for page behavior
    public class DlgPropertySheet : UserControl
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DlgProperties ParentDlg
        protected DialogProperties ParentDlg
        {
            get
            {
                Debug.Assert(null != m_ParentDlg);
                return m_ParentDlg;
            }
        }
        readonly DialogProperties m_ParentDlg;
        #endregion

		// Scaffolding -----------------------------------------------------------------------
        #region Constructor(parent)
        public DlgPropertySheet(DialogProperties _ParentDlg)
			: this()
        {
            m_ParentDlg = _ParentDlg;
            Dock = DockStyle.Fill;
        }
        #endregion
		#region Constructor() - for VS 2008
		public DlgPropertySheet() 
        { 
        }
		#endregion

		// Stubs: Subclass should override
		#region Attr{g}: string ID - a unique ID for identifying this page
		public virtual string ID
		{
			get
			{
				return "";
			}
		}
		#endregion
        #region Method: virtual bool HarvestChanges() - return T if everything is OK
        public virtual bool HarvestChanges()
        {
            return true;
        }
        #endregion
        #region Method: virtual void ShowHelp()
        public virtual void ShowHelp()
        {
            HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: virtual string Title
        public virtual string Title
        {
            get
            {
                //Debug.Assert(false);
                return "Override in Subclass!";
            }
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region method: string GetLocalizedTitle(string sTitleEnglish)
        protected string GetLocalizedTitle(string sTitleEnglish)
        {
            return LocDB.GetValue(new[] { ParentDlg.Name, ParentDlg.Name + "_" + Name },
                "pageTitle",
                sTitleEnglish,
                null, null);
        }
        #endregion
    }
    #endregion

}