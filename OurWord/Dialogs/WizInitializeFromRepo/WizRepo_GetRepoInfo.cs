#region ***** WizRepo_GetRepoInfo.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WizRepo_GetRepoInfo.cs
 * Author:  John Wimbish
 * Created: 16 May 2009
 * Purpose: Gets the URL, Username, and Password
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
using OurWordData;
using OurWordData.DataModel;
using OurWordData.Synchronize;

#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class WizRepo_GetRepoInfo : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: WizInitializeFromRepository Wizard - the owning wizard
        WizInitializeFromRepository Wizard
        {
            get
            {
                Debug.Assert(null != Parent as WizInitializeFromRepository);
                return Parent as WizInitializeFromRepository;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizRepo_GetRepoInfo()
        {
            InitializeComponent();

        }
        #endregion

        // User-Entered Information ----------------------------------------------------------
        #region Attr{g/s}: string Url
        public string Url
        {
            get
            {
                return m_textURL.Text;
            }
            set
            {
                m_textURL.Text = value;
            }
        }
        #endregion
        #region Attr{g/s}: string UserName
        public string UserName
        {
            get
            {
                return m_textUserName.Text;
            }
            set
            {
                m_textUserName.Text = value;
            }
        }
        #endregion
        #region Attr{g/s}: string Password
        public string Password
        {
            get
            {
                return m_textPassword.Text;
            }
            set
            {
                m_textPassword.Text = value;
            }
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            // Default value
            if (string.IsNullOrEmpty(Url))
                Url = InternetRepository.c_sDefaultServer;

            if (string.IsNullOrEmpty(UserName))
                UserName = Wizard.InitialUserName;

            if (string.IsNullOrEmpty(Password))
                Password = Wizard.InitialPassword;
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            // Make sure we have a non-empty Url
            if (string.IsNullOrEmpty(Url))
            {
                m_labelErrorMsg.Text = Loc.GetString("kEnterUrl", "Please enter the repository's URL");
                return false;
            }

            // Non-empty Username
            if (string.IsNullOrEmpty(UserName))
            {
                m_labelErrorMsg.Text = Loc.GetString("kEnterUsername", 
                    "Please enter your User Name for accessing the repository.");
                return false;
            }

            // Non-empty Password
            if (string.IsNullOrEmpty(Password))
            {
                m_labelErrorMsg.Text = Loc.GetString("kEnterPassword",
                    "Please enter your User Name for accessing the repository.");
                return false;
            }

            m_labelErrorMsg.Text = "";

            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strRepositoryInfo", "Repository Info", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

        // Handlers --------------------------------------------------------------------------
        #region Cmd: cmdTextChanged
        private void cmdTextChanged(object sender, EventArgs e)
        {
            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
        }
        #endregion
    }
}
