/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgProperties.cs
 * Author:  John Wimbish
 * Created: 17 Sep 2007
 * Purpose: Edit the settings that an advisor will typically handle.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
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
using JWdb;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public partial class DialogProperties : Form
    {
        // Controls --------------------------------------------------------------------------
        #region Attr{s}: string NavTitleText
        string NavTitleText
        {
            set
            {
                m_NavTitle.Text = value;
            }
        }
        #endregion
        #region Attr{g}: TabControl TC
        TabControl TC
        {
            get
            {
                return m_TabControl;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DialogProperties()
        {
            InitializeComponent();
        }
        #endregion
        #region Method: void SetTitleBarText()
        public void SetTitleBarText()
        {
            string sLanguageName = (null != G.Project.TargetTranslation) ?
                G.Project.TargetTranslation.DisplayName : "";

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

        // Navigation Buttons (left-hand side) -----------------------------------------------
        #region Navigation Constants
        public const string c_navEssentials = "Essentials";
        public const string c_navOptions = "Options";
        public const string c_navTranslations = "Translations";
        public const string c_navTeamSettings = "TeamSettings";
        #endregion
        #region Cmd: cmdNavEssentials
        private void cmdNavEssentials(object sender, EventArgs e)
        {
            string sActiveTag = (null == G.Project.TargetTranslation) ?
                c_tagEssentialsFront : c_tagEssentialsTarget;

            SetupTabControl(c_navEssentials);
            ActivatePage(sActiveTag);

            NavTitleText = m_btnNavEssentials.Text;
        }
        #endregion
        #region Cmd: cmdNavOptions
        private void cmdNavOptions(object sender, EventArgs e)
        {
            NavTitleText = m_btnNavOptions.Text;
            SetupTabControl(c_navOptions);
            ActivatePage(c_tagOptions);
        }
        #endregion
        #region Cmd: cmdNavOtherTranslations
        private void cmdNavOtherTranslations(object sender, EventArgs e)
        {
            NavTitleText = m_btnNavTranslations.Text;
            SetupTabControl(c_navTranslations);
            ActivatePage(c_tagOtherTranslations);
        }
        #endregion
        #region Cmd: cmdNavTeamSettings
        private void cmdNavTeamSettings(object sender, EventArgs e)
        {
            NavTitleText = m_btnNavTeamSettings.Text;
            SetupTabControl(c_navTeamSettings);
            ActivatePage(c_tagTeamSettings);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            cmdNavEssentials(null, null);

            // Localization
            Control[] vExclude = { m_NavTitle };
            LocDB.Localize(this, vExclude);

            // Set the TitleBar after the localization, as we override it
            SetTitleBarText();
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
            if (!HarvestChangesFromCurrentSheet())
            {
                e.Cancel = true;
                return;
            }

            // The following warnings still permit the dialog to be closed; but we don't want
            // to drive the user nuts with warnings; so we just show the first one we come to.
            bool bCanCloseAnywayWarningIssued = false;

            // The project should have a Front Translation. If not, we'll allow the user to
            // exit, but we will at least have warned him.
            if (null == G.Project.FrontTranslation)
            {
                bCanCloseAnywayWarningIssued = true;
                if (Messages.ProjectHasNoFront())
                {
                    ActivatePage(c_tagEssentialsFront);
                    e.Cancel = true;
                    return;
                }
            }

            // The project should have a Target Translation. If not, we'll allow the user to
            // exit, but we will at least have warned him.
            if (!bCanCloseAnywayWarningIssued && null == G.Project.TargetTranslation)
            {
                if (Messages.ProjectHasNoTarget())
                {
                    ActivatePage(c_tagEssentialsTarget);
                    e.Cancel = true;
                    return;
                }
            }

            // Make sure the project has a reasonable name
            if (null != G.Project.TargetTranslation &&
                G.Project.TargetTranslation.DisplayName.Length > 0)
            {
                G.Project.DisplayName = G.Project.TargetTranslation.DisplayName;
            }
            else
                G.Project.DisplayName = "My Project";

            // Go ahead and close
            return;
        }
        #endregion
        #region Cmd: cmdHelp
        private void cmdHelp(object sender, EventArgs e)
        {
            CurrentSheet.ShowHelp();
        }
        #endregion

        // Tab Pages -------------------------------------------------------------------------
        #region Page Constants
        public const string c_tagEssentialsFront   = "Essentials-Front";
        public const string c_tagEssentialsTarget  = "Essentials-Target";
        const string c_tagOptions           = "Options";
        const string c_tagNotes             = "Notes";
        const string c_tagOtherTranslations = "OtherTranslations";
        const string c_tagTeamSettings      = "TS";
        const string c_tagWritingSystems    = "TS-WritingSystems";
        const string c_tagStyleSheet        = "TS-StyleSheet";
        const string c_tagAdvancedPrintOptions = "TS-AdvancedPrintOptions";
        const string c_tagTranslationStages = "TS-TranslationStages";
        #endregion
        #region Method: string GetNewTagForOtherTranslation()
        public string GetNewTagForOtherTranslation()
        {
            s_OtherTranslationTag++;
            return c_tagOtherTranslations + s_OtherTranslationTag.ToString();
        }
        static long s_OtherTranslationTag = 0;
        #endregion
        #region Method: void AddPage(string sTag, DlgPropertySheet)
        void AddPage(string sTag, DlgPropertySheet sheet)
        {
            // Create the TabPage and add it to the TabControl
            TabPage page = new TabPage();
            page.Tag = sTag;
            TC.TabPages.Add(page);

            // Retrieve the name of the tab from the sheet
            page.Text = sheet.TabText; 

            // Place the Sheet into the TabPage
            page.Controls.Add(sheet);

            // TODO: Localization
        }
        #endregion
        #region Method: void ActivatePage(string sTag)
        public void ActivatePage(string sTag)
        {
            foreach (TabPage page in TC.TabPages)
            {
                if ((string)page.Tag == sTag)
                {
                    TC.SelectTab(page);
                    break;
                }
            }
        }
        #endregion
        #region Method: void ActivatePage(DTranslation t)
        public void ActivatePage(DTranslation t)
        {
            if (t == G.Project.FrontTranslation)
            {
                SetupTabControl(c_navEssentials);
                ActivatePage(c_tagEssentialsFront);
            }
            else if (t == G.Project.TargetTranslation)
            {
                SetupTabControl(c_navEssentials);
                ActivatePage(c_tagEssentialsTarget);
            }
            else if (-1 != G.Project.OtherTranslations.FindObj(t))
            {
                SetupTabControl(c_navTranslations);
                foreach (TabPage page in TC.TabPages)
                {
                    if ((string)page.Text == t.DisplayName)
                        TC.SelectTab(page);
                }
            }
        }
                #endregion
        #region Attr{g}: DlgPropertySheet CurrentSheet
        DlgPropertySheet CurrentSheet
        {
            get
            {
                TabPage pageCurrent = TC.SelectedTab;
                if (null == pageCurrent)
                    return null;

                foreach (Control ctrl in pageCurrent.Controls)
                {
                    DlgPropertySheet sheet = ctrl as DlgPropertySheet;
                    if (null != sheet)
                        return sheet;
                }

                return null;
            }
        }
        #endregion
        #region Method: bool HarvestChangesFromCurrentSheet()
        public bool HarvestChangesFromCurrentSheet()
        {
            if (null != CurrentSheet)
                return CurrentSheet.HarvestChanges();
            return true;
        }
        #endregion
        #region Method: void SetupTabControl(sActiveNav)
        public void SetupTabControl(string sActiveNav)
        {
            // Harvest Changes from the current page (if any). Abort if we fail;
            // it means the user needs to fix something
            if (!HarvestChangesFromCurrentSheet())
                return;

            // Clear out pages so we can start over
            TC.TabPages.Clear();

            // Essentials
            if (sActiveNav == c_navEssentials)
            {
                // Front Translation
                AddPage( c_tagEssentialsFront, (null == G.FTranslation) ?
                    new Page_SetupFront(this) as DlgPropertySheet :
                    new Page_Translation(this, G.FTranslation, true) as DlgPropertySheet);

                // Target Translation
                AddPage(c_tagEssentialsTarget, (null == G.TTranslation) ?
                    new Page_SetupTarget(this) as DlgPropertySheet :
                    new Page_Translation(this, G.TTranslation, false) as DlgPropertySheet);
            }

            // Other Translations
            if (sActiveNav == c_navTranslations)
            {
                // Other Translations (General Page)
                AddPage(c_tagOtherTranslations,
                    new Page_OtherTranslations(this));

                foreach (DTranslation t in G.Project.OtherTranslations)
                {
                    AddPage(GetNewTagForOtherTranslation(),
                        new Page_Translation(this, t, true));
                }
            }

            // Options
            if (sActiveNav == c_navOptions)
            {
                // General Options
                AddPage(c_tagOptions,
                    new Page_Options(this));

                // Notes
                AddPage(c_tagNotes,
                    new Page_Notes(this));
            }

            // TeamSettings
            if (sActiveNav == c_navTeamSettings)
            {
                // General Page
                AddPage(c_tagTeamSettings,
                    new Page_TeamSettings(this));

                // StyleSheet Page
                AddPage(c_tagStyleSheet,
                    new Page_StyleSheet(this));

                // Writing Systems Page
                AddPage(c_tagWritingSystems,
                    new Page_WritingSystems(this));

                // Advanced Print Options Page
                AddPage(c_tagAdvancedPrintOptions,
                    new Page_AdvancedPrintOptions(this));

                // Translation Stages
                AddPage(c_tagTranslationStages, 
                    new Page_TranslationStages(this));
            }

            // By Default, we activate the first page; we can call ActivatePage
            // in order to activate a different one.
            TC.SelectTab(0);
        }
        #endregion
        #region Method: void UpdateActiveTabText()
        public void UpdateActiveTabText()
        {
            TabPage pageCurrent = TC.SelectedTab;
            if (null == pageCurrent)
                return;

            DlgPropertySheet sheet = CurrentSheet;
            if (null == sheet)
                return;

            pageCurrent.Text = sheet.TabText;
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
        #region Constructor()
        public DlgPropertySheet(DialogProperties _ParentDlg)
        {
            m_ParentDlg = _ParentDlg;
        }
        #endregion
        public DlgPropertySheet() { }

        // Stubs: Subclass should override
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
        #region Attr{g}: virtual string TabText
        public virtual string TabText
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