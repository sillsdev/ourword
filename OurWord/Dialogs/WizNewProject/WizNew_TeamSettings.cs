/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_TeamSettings.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Obtains the team settings file
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
    public partial class WizNew_TeamSettings : UserControl, IJW_WizPage
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

        public enum Options { kDefault, kUseExisting, kSaveAs };
        #region Attr{g}: Options Option
        public Options Option
        {
            get
            {
                return m_Option;
            }
        }
        Options m_Option = Options.kDefault;
        #endregion

        #region Attr{g}: string PathName
        public string PathName
        {
            get
            {
                return m_sPathName;
            }
        }
        string m_sPathName;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_TeamSettings()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            if (string.IsNullOrEmpty(PathName))
            {
                SetDefaultPathName();
                m_Option = Options.kDefault;
            }
            SetCurrentAction();
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strTeamSettings", "Team Settings", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

        // Supporting Methods ----------------------------------------------------------------
        #region Method: void SetDefaultPathName() - the default Team Settings action
        void SetDefaultPathName()
        {
            m_sPathName = Wizard.TargetSettingsFolder + 
                Path.DirectorySeparatorChar +  
                "Team Settings.owt";
        }
        #endregion
        #region Method: void SetCurrentAction()
        void SetCurrentAction()
        {
            switch (Option)
            {
                case Options.kDefault:
                    m_textCurrentAction.Text = "Default: Settings will be stored in:\n" + 
                        PathName;
                    break;
                case Options.kUseExisting:
                    m_textCurrentAction.Text = "Use Existing: Settings will be obtained from:\n" +
                        PathName;
                    break;
                case Options.kSaveAs:
                    break;
            }
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdUseExisting
        private void cmdUseExisting(object sender, EventArgs e)
            // TODO: Can we combine this with what's in the Page_TeamSettings of the Properties dialog?
        {
            // Set up the dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = G.GetLoc_Files("TeamSettingsFileFilter",
                "Our Word Team Settings File (*.owt)|*.owt");
            dlg.FilterIndex = 0;
            dlg.Title = G.GetLoc_Files("OpenTeamSettingsDlgTitle",
                "Use an Existing Team Settings File");
            dlg.InitialDirectory = Wizard.TargetSettingsFolder;

            // Run the Dialog
            if (DialogResult.OK == dlg.ShowDialog(Wizard))
            {
                m_Option = Options.kUseExisting;
                m_sPathName = dlg.FileName;
                SetCurrentAction();
            }
        }
        #endregion
        #region Cmd: cmdSaveAs
        private void cmdSaveAs(object sender, EventArgs e)
            // TODO: Combine this with Prop's Dialog Page_TeamSettings?
        {
            // Choose a filename for these settings
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = G.GetLoc_Files("TeamSettingsFileFilter",
                "Our Word Team Settings File (*.owt)|*.owt");
            dlg.DefaultExt = "owt";
            dlg.InitialDirectory = Wizard.TargetSettingsFolder;
            dlg.Title = G.GetLoc_Files("SaveTeamSettingsDlgTitle",
                "Save the Team Settings file as");
            if (DialogResult.OK == dlg.ShowDialog())
            {
                m_Option = Options.kSaveAs;
                m_sPathName = dlg.FileName;
                SetCurrentAction();
            }
        }
        #endregion
        #region Cmd: cmdDefault
        private void cmdDefault(object sender, EventArgs e)
        {
            m_Option = Options.kDefault;
            SetDefaultPathName();
            SetCurrentAction();
        }
        #endregion
    }
}
