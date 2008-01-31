/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_FrontInfo.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Obtains the name, abbreviation and settings file for the front translation
 * Legal:   Copyright (c) 2003-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Dialogs.WizNewProject
{
    public partial class WizNew_FrontInfo : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: WizNewProject Wizard - the owning wizard
        WizNewProject Wizard
        {
            get
            {
                Debug.Assert(null != Parent as WizNewProject);
                return Parent as WizNewProject;
            }
        }
        #endregion
        #region VAttr{g}: bool UseExisting
        public bool UseExisting
        {
            get
            {
                if (string.IsNullOrEmpty(ExistingFrontSettingsFilePath))
                    return false;
                return true;
            }
        }
        #endregion
        #region VAttr{g}: string ExistingFrontSettingsFilePath
        public string ExistingFrontSettingsFilePath
        {
            get
            {
                return m_sExistingFrontSettingsFilePath;
            }
        }
        string m_sExistingFrontSettingsFilePath;
        #endregion
        #region VAttr{g}: string FrontName
        public string FrontName
        {
            get
            {
                return m_textFrontName.Text;
            }
        }
        #endregion
        #region VAttr{g}: string FrontAbbreviation
        public string FrontAbbreviation
        {
            get
            {
                return m_textFrontAbbreviation.Text;
            }
        }
        #endregion
        #region VAttr{g}: string FrontSettingsFolder
        public string FrontSettingsFolder
        {
            get
            {
                return m_textFrontSettingsFolder.Text;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_FrontInfo()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            if (string.IsNullOrEmpty(FrontName))
                return false;
            if (string.IsNullOrEmpty(FrontAbbreviation))
                return false;
            if (string.IsNullOrEmpty(FrontSettingsFolder))
                return false;
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strFrontInfo", "Front Information", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdUseExisting
        private void cmdUseExisting(object sender, EventArgs e)
        {
            // Launch the dialog to prompt for the Settings Filename. The user has the 
            // option to abort from this dlg.
            DialogOpenTranslation dlg = new DialogOpenTranslation();
            if (DialogResult.OK != dlg.ShowDialog())
                return;

            // Read in the translation object
            DTranslation Translation = new DTranslation();
            Translation.AbsolutePathName = dlg.SettingsPath;  // First so we know what to load
            Translation.Load();
            Translation.AbsolutePathName = dlg.SettingsPath;  // Second because Load overwrote it

            // Retrieve all of the settings we're interested in
            m_sExistingFrontSettingsFilePath = Translation.AbsolutePathName;
            m_textFrontName.Text = Translation.DisplayName;
            m_textFrontAbbreviation.Text = Translation.LanguageAbbrev;
            m_textFrontSettingsFolder.Text = Path.GetDirectoryName(Translation.AbsolutePathName);

            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
        }
        #endregion
        #region Cmd: cmdBrowse
        private void cmdBrowse(object sender, EventArgs e)
        {
            // Set up the Browse For Folder dialog
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = LocDB.GetValue(this, "strBrowseFrontFolderDialogTitle",
                "Choose the folder in which to store settings for the Front translation.", null);
            dlg.ShowNewFolderButton = true;
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Run the dialog
            if (DialogResult.OK == dlg.ShowDialog(Wizard))
            {
                m_textFrontSettingsFolder.Text = dlg.SelectedPath;
                Wizard.AdvanceButtonEnabled = CanGoToNextPage();
            }
        }
        #endregion
        #region Cmd: cmdLanguageNameChanged
        private void cmdLanguageNameChanged(object sender, EventArgs e)
            // Same algorithm as in the CreateTranslationDlg; probably should
            // combine them at some point.
        {
            int cAbbrevLength = 3;

            if (FrontName.Length == 0)
                return;

            // Get the default abbreviation as the first three letters of the name
            string sDefaultAbbrev = "";
            if (FrontName.Length > 0)
            {
                foreach (char ch in FrontName)
                {
                    if (ch != ' ')
                    {
                        sDefaultAbbrev += ch;
                        cAbbrevLength--;
                    }
                    if (0 == cAbbrevLength)
                        break;
                }
            }

            // If the abbreviation currently entered matches this default (for as
            // many letters as it has), then add any remaining letters
            int i = 0;
            for (; i < FrontAbbreviation.Length && i < sDefaultAbbrev.Length; i++)
            {
                if (FrontAbbreviation[i] != sDefaultAbbrev[i])
                    break;
            }
            if (i == FrontAbbreviation.Length && i < sDefaultAbbrev.Length)
                m_textFrontAbbreviation.Text = sDefaultAbbrev;
        }
        #endregion

    }
}
