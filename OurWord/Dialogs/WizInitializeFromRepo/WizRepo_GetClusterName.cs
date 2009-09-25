#region ***** WizRepo_GetClusterName.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WizRepo_GetClusterName.cs
 * Author:  John Wimbish
 * Created: 16 May 2009
 * Purpose: Get the cluster name, e.g., "Timor".
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
#endregion


namespace OurWord.Dialogs
{
    public partial class WizRepo_GetClusterName : UserControl, IJW_WizPage
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
        public WizRepo_GetClusterName()
        {
            InitializeComponent();
        }
        #endregion

        // User-Entered Information ----------------------------------------------------------
        #region Attr{g/s}: string ClusterName
        public string ClusterName
        {
            get
            {
                return m_textClusterName.Text;
            }
            set
            {
                m_textClusterName.Text = value;
            }
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
            // Make sure we have a non-empty Url
            if (string.IsNullOrEmpty(ClusterName))
            {
                m_labelErrorMsg.Text = Loc.GetString("kEnterClusterName", 
                    "Please enter a name for the cluster.");
                return false;
            }

            m_labelErrorMsg.Text = "";

            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strClusterName", "Cluster Name", null);
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
