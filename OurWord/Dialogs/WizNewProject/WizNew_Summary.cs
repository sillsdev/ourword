/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_Summary.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: A final opportunity to look over how the New Project will be created.
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
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
using JWdb.DataModel;
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
			// New Language Name
            m_textLanguageName.Text = Wizard.ProjectName;

			// Source Language Name
            m_textSourceTranslation.Text = Wizard.FrontName;
			if (Wizard.CreatingNewFront)
			{
				m_textSourceTranslation.Text += 
					(" " + G.GetLoc_String("CreatingNewFront", "(creating)"));
			}

			// The Cluster its going into, if there are more than one in the system
			if (!Wizard.ShowClusterChoicePage)
			{
				m_lblCluster.Visible = false;
				m_textCluster.Visible = false;
			}
			else
			{
                if (null != Wizard.ChosenCluster)
				    m_textCluster.Text = Wizard.ChosenCluster.Name;
			}
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
            return LocDB.GetValue(this, "strSummary", "Summary", null);
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
