/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_Summary.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: A final opportunity to look over how the New Project will be created.
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
    public partial class WizNew_Summary : UserControl, IJW_WizPage
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
        #region VAttr{g}: bool LaunchPropertiesDialogWhenDone
        public bool LaunchPropertiesDialogWhenDone
        {
            get
            {
                return m_checkLaunchProperties.Checked;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_Summary()
        {
            InitializeComponent();

            m_checkLaunchProperties.Checked = true;
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            m_textLanguageName.Text = Wizard.ProjectName;

            m_textAbbreviation.Text = Wizard.TargetAbbreviation;

            m_textSettingsFolder.Text = Wizard.TargetSettingsFolder;

            m_textSourceTranslation.Text =
                Wizard.FrontName + ", " +
                Wizard.FrontAbbreviation + ", " +
                Wizard.FrontSettingsFolder;

            m_textTeamSettings.Text =
                Wizard.TargetSettingsFolder;
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
            return LocDB.GetValue(this, "strSummaryh", "Summary", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

    }
}
