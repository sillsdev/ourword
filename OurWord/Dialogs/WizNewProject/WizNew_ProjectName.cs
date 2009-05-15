/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_ProjectName.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Obtains the name of the project
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
    public partial class WizNew_ProjectName : UserControl, IJW_WizPage
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
        #region Attr{g}: string ProjectName
        public string ProjectName
        {
            get
            {
                return m_textProjectName.Text;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_ProjectName()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
			// For performance, get this list as we enter the page, so that we don't
			// have to access the disk every time the user types (the cmdProjectNameChanged
			// command handler.
            if (null != Wizard.ChosenCluster)
                m_vLanguages = Wizard.ChosenCluster.GetClusterLanguageList();

			// Default to the error not being visible.
			m_lblError.Visible = false;
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            if (string.IsNullOrEmpty(ProjectName))
                return false;
			if (NameAlreadyExists)
				return false;
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strProjectName", "Project Name", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

		// Implementation -------------------------------------------------------------------
		#region Attr{g}: List<string> Languages
		List<string> Languages
		{
			get
			{
				Debug.Assert(null != m_vLanguages);
				return m_vLanguages;
			}
		}
		List<string> m_vLanguages;
		#endregion
		#region VAttr{g}: bool NameAlreadyExists
		bool NameAlreadyExists
		{
			get
			{
				string sProposedLanguage = m_textProjectName.Text;

				if (Languages.IndexOf(sProposedLanguage) != -1)
					return true;

				return false;
			}
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdProjectNameChanged
        private void cmdProjectNameChanged(object sender, EventArgs e)
            // If we have something valid in the Project Name, then we can go ahead and
            // enable the Next button.
        {
			m_lblError.Visible = NameAlreadyExists;
            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
        }
        #endregion
    }
}
