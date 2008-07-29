/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_TeamSettings.cs
 * Author:  John Wimbish
 * Created: 18 Sep 2007
 * Purpose: Sets up general information about the Team Settings
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
using System.Text;
using System.Reflection;
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
    public partial class Page_TeamSettings : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_TeamSettings(DialogProperties _ParentDlg)
            : base(_ParentDlg)
        {
            InitializeComponent();
        }
        #endregion

        // Controls --------------------------------------------------------------------------
        #region Attr{s}: string TeamSettingsFileText
        string TeamSettingsFileText
        {
            set
            {
                m_textTeamSettingsFile.Text = JWU.PathEllipses(value, 50);
            }
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
        {
            get
            {
                return "General";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            return true;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdCreateNew
        private void cmdCreateNew(object sender, EventArgs e)
        {
            // Choose a filename for these settings
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = G.GetLoc_Files("TeamSettingsFileFilter",
                "Our Word Team Settings File (*.owt)|*.owt");
            dlg.DefaultExt = "owt";
            dlg.InitialDirectory = Path.GetDirectoryName( G.Project.TeamSettings.AbsolutePathName);
            dlg.FileName = Path.GetFileName(G.Project.TeamSettings.AbsolutePathName);
            dlg.Title = G.GetLoc_Files("SaveTeamSettingsDlgTitle",
                "Save the Team Settings file as");
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            // Put the new filename into the Team Settings object
            G.Project.TeamSettings.AbsolutePathName = dlg.FileName;

            // Put the new filename into the dialog control
            TeamSettingsFileText = dlg.FileName;
        }
        #endregion
        #region Cmd: cmdOpenExisting
        private void cmdOpenExisting(object sender, EventArgs e)
        {
            // Navigate to the desired settings file
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Our Word Team Settings File (*.owt)|*.owt";
            dlg.FilterIndex = 0;
            dlg.Title = "Open Team Settings File";
            dlg.InitialDirectory = G.BrowseDirectory;
            if (DialogResult.OK != dlg.ShowDialog(this))
                return;
            G.BrowseDirectory = Path.GetDirectoryName(dlg.FileName);

            // Save the old Team Settings, in case any changes had been made
            G.Project.TeamSettings.Write();

            // Clear out the existing Team Settings
            G.Project.TeamSettings = new DTeamSettings();

            // Load the requested team settings
            G.Project.TeamSettings.AbsolutePathName = dlg.FileName;
            G.Project.TeamSettings.Load();

            // Set the path again, as Load will have overwritten it
            G.Project.TeamSettings.AbsolutePathName = dlg.FileName;

            // Put the new filename into the dialog control
            TeamSettingsFileText = dlg.FileName;

            // Re-do the property pages
            ParentDlg.SetupTabControl(DialogProperties.c_navTeamSettings);
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Current values
            TeamSettingsFileText = G.Project.TeamSettings.AbsolutePathName;

            // Localize
            Control[] vExclude = new Control[] { m_textTeamSettingsFile };
            LocDB.Localize(this, vExclude);
        }
        #endregion
    }
}
