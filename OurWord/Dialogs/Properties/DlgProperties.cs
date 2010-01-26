#region ***** DlgProperties.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgProperties.cs
 * Author:  John Wimbish
 * Created: 17 Sep 2007
 * Purpose: Edit the settings that an advisor will typically handle.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using JWTools;
using OurWordData;
using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWord.Utilities;
using OurWordData.Styles;

#endregion
#endregion

// TODO: Check over Localization of pages, especially, e.g., "Front: Kupang"

namespace OurWord.Dialogs
{
    public partial class DialogProperties : Form
    {
		// Controls --------------------------------------------------------------------------
		#region VAttr{g}: Label NavTitle
		Label NavTitle
		{
			get
			{
				return m_NavTitle;
			}
		}
		#endregion
		#region VAttr{s}: string NavTitleText
		string NavTitleText
        {
            set
            {
                m_NavTitle.Text = value;
            }
        }
        #endregion
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

            NavTitleText = sGroupTitle + CurrentPage.Title;

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
            foreach (DlgPropertySheet page in Pages)
            {
                if (page.ID == sID)
                    pageNew = page;
            }
            if (null == pageNew)
                return;

            // Turn off the old page
            if (null != CurrentPage)
            {
                CurrentPage.Visible = false;
                Controls.Remove(CurrentPage);
            }

            // Display the new page
            int nMargin = NavTitle.Left - NavList.Right;
            CurrentPage = pageNew;
            CurrentPage.Left = NavTitle.Left;
            CurrentPage.Top = NavTitle.Bottom + nMargin;
            CurrentPage.Width = NavTitle.Width;
            CurrentPage.Height = m_btnOK.Top - NavTitle.Bottom - nMargin * 2;
            CurrentPage.Visible = true;
            CurrentPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Top |
                AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(CurrentPage);
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

            // Translations
            GroupedTasks gtTranslations = NavList.AddGroup("Translations");

            if (null == DB.TargetTranslation)
                AddPage(gtTranslations, new Page_SetupTarget(this), c_iImageDefault);
            else
                AddPage(gtTranslations, new Page_Translation(this, DB.TargetTranslation, false), c_iImageDefault);

            if (null == DB.FrontTranslation)
                AddPage(gtTranslations, new Page_SetupFront(this), c_iImageDefault);
            else
                AddPage(gtTranslations, new Page_Translation(this, DB.FrontTranslation, true), c_iImageDefault);

            // Reference Translations
            GroupedTasks gtReferenceTranslations = NavList.AddGroup("Reference");
            AddPage(gtReferenceTranslations, new Page_OtherTranslations(this), c_iImageDefault);
            foreach (DTranslation t in DB.Project.OtherTranslations)
                AddPage(gtReferenceTranslations, new Page_Translation(this, t, true), c_iImageDefault);

            // Options
            GroupedTasks gtOptions = NavList.AddGroup("Options");
            AddPage(gtOptions, new Page_Options(this), c_iImageOptions);
            AddPage(gtOptions, new Page_Notes(this), c_iImageNotes);
            AddPage(gtOptions, new Page_Collaboration(this), c_iImageCollaboration);
            AddPage(gtOptions, new Page_Cluster(this), c_iImageClusters);
            AddPage(gtOptions, new Page_StyleSheet(this), c_iImageStyleSheet);
            AddPage(gtOptions, new Page_AdvancedPrintOptions(this), c_iImageAdvancedPrint);
            AddPage(gtOptions, new Page_TranslationStages(this), c_iImageDefault);

            // Writing Systems (start collapsed, as this is a less-frequent group)
            GroupedTasks gtWritingSystems = NavList.AddGroup(Strings.PropDlgTab_WritingSystems);
            gtWritingSystems.Expanded = false;
            AddPage(gtWritingSystems, new Page_AddWritingSystem(this), c_iImageAddWritingSystem);
            foreach (var ws in StyleSheet.WritingSystems)
                AddPage(gtWritingSystems, new Page_WritingSystems(this, ws), c_iImageWritingSystem);

            // Go to the requested page
            if (!string.IsNullOrEmpty(sIdActivePage))
                SetActivePage(sIdActivePage);

            // Make the nav list show up correctly
            NavList.cmdLayout(null, null);
        }
        #endregion

        // Images ----------------------------------------------------------------------------
        const int c_iImageDefault = 0;
        const int c_iImageAdvancedPrint = 1;
        const int c_iImageNotes = 2;
        const int c_iImageOptions = 3;
        const int c_iImageWritingSystem = 4;
        const int c_iImageAddWritingSystem = 5;
        const int c_iImageStyleSheet = 6;
        const int c_iImageCollaboration = 7;
        const int c_iImageClusters = 8;

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
            Control[] vExclude = { m_NavTitle, m_NavTasks };
            LocDB.Localize(this, vExclude);

            // Set the TitleBar after the localization, as we override it
            SetTitleBarText();

			// The first page we'll go to
			string sInitialPageID = Page_SetupFront.c_sID;
			if (DB.FrontTranslation == null)
				sInitialPageID = Page_SetupFront.c_sID;
			else if (DB.TargetTranslation == null)
				sInitialPageID = Page_SetupTarget.c_sID;
			else
				sInitialPageID = Page_Translation.ComputeID(DB.TargetTranslation.DisplayName);

            // Set up the pages
            NavList.Images = m_Images;
            InitNavigation(sInitialPageID);
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

            // The following warnings still permit the dialog to be closed; but we don't want
            // to drive the user nuts with warnings; so we just show the first one we come to.
            bool bCanCloseAnywayWarningIssued = false;

            // The project should have a Front Translation. If not, we'll allow the user to
            // exit, but we will at least have warned him.
            if (null == DB.Project.FrontTranslation)
            {
                bCanCloseAnywayWarningIssued = true;
                if (Messages.ProjectHasNoFront())
                {
					SetActivePage(Page_SetupFront.c_sID);
                    e.Cancel = true;
                    return;
                }
            }

            // The project should have a Target Translation. If not, we'll allow the user to
            // exit, but we will at least have warned him.
            if (!bCanCloseAnywayWarningIssued && null == DB.Project.TargetTranslation)
            {
                if (Messages.ProjectHasNoTarget())
                {
					SetActivePage(Page_SetupTarget.c_sID);
                    e.Cancel = true;
                    return;
                }
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
    public class DlgPropertySheet : System.Windows.Forms.UserControl
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
        DialogProperties m_ParentDlg;
        #endregion

		// Scaffolding -----------------------------------------------------------------------
        #region Constructor(parent)
        public DlgPropertySheet(DialogProperties _ParentDlg)
			: this()
        {
            m_ParentDlg = _ParentDlg;
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
    }
    #endregion

}