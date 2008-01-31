/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_TargetInfo.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Obtains the abbreviation and settings file for the target translation
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
    public partial class WizNew_TargetInfo : UserControl, IJW_WizPage
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
        #region Attr{g}: string Abbreviation
        public string Abbreviation
        {
            get
            {
                return m_textAbbrev.Text;
            }
        }
        #endregion
        #region Attr{g}: string SettingsFolder
        public string SettingsFolder
        {
            get
            {
                return m_textSettingsFolder.Text;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_TargetInfo()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
            // If there is nothing in the abbreviation, the populate it with the first
            // several letters of the language name
        {
            if (string.IsNullOrEmpty(Abbreviation))
            {
                int nLength = Math.Min(3, Wizard.ProjectName.Length);
                m_textAbbrev.Text = Wizard.ProjectName.Substring(0, nLength);
            }
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            if (string.IsNullOrEmpty(Abbreviation))
                return false;
            if (string.IsNullOrEmpty(SettingsFolder))
                return false;
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strTargetInfo", "Target Information", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdAbbreviationChanged
        private void cmdAbbreviationChanged(object sender, EventArgs e)
        {
            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
        }
        #endregion
        #region Cmd: cmdBrowse - choose the Settings Folder
        private void cmdBrowse(object sender, EventArgs e)
        {
            // Set up the Browse For Folder dialog
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = LocDB.GetValue(this, "strBrowseTargetFolderDialogTitle",
                "Choose the folder in which to store settings for this project", null);
            dlg.ShowNewFolderButton = true;
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            dlg.SelectedPath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );

            // Run the dialog
            if (DialogResult.OK == dlg.ShowDialog(Wizard))
            {
                m_textSettingsFolder.Text = dlg.SelectedPath;
                Wizard.AdvanceButtonEnabled = CanGoToNextPage();
            }
        }
        #endregion
    }
}
