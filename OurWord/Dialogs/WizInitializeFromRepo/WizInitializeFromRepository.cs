#region ***** WizInitializeFromRepository.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WizInitializeFromRepository.cs
 * Author:  John Wimbish
 * Created: 16 May 2009
 * Purpose: Wizard that collects the settings for setting up a cluster from a remote
 *          (Internet) repository.
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
#endregion
#endregion

namespace OurWord.Dialogs
{
    public class WizInitializeFromRepository : JW_Wizard
    {
        // Individual Pages in the Wizard, in order of appearance ----------------------------
        WizRepo_Introduction m_pageIntroduction;
        WizRepo_GetRepoInfo m_pageGetRepoInfo;
        WizRepo_GetClusterName m_pageGetClusterName;
        WizRepo_GetLocation m_pageGetLocation;
        WizRepo_Summary m_pageSummary;

        // User-Entered Information ----------------------------------------------------------
        #region VAttr{g/s}: string Url
        public string Url
        {
            get
            {
                return m_pageGetRepoInfo.Url;
            }
            set
            {
                m_pageGetRepoInfo.Url = value;
            }
        }
        #endregion
        #region VAttr{g/s}: string UserName
        public string UserName
        {
            get
            {
                return m_pageGetRepoInfo.UserName;
            }
            set
            {
                m_pageGetRepoInfo.UserName = value;
            }
        }
        #endregion
        #region VAttr{g/s}: string Password
        public string Password
        {
            get
            {
                return m_pageGetRepoInfo.Password;
            }
            set
            {
                m_pageGetRepoInfo.Password = value;
            }
        }
        #endregion
        #region VAttr{g/s}: string ClusterName
        public string ClusterName
        {
            get
            {
                return m_pageGetClusterName.ClusterName;
            }
            set
            {
                m_pageGetClusterName.ClusterName = value;
            }
        }
        #endregion
        #region VAttr{g/s}: bool IsInMyDocuments
        public bool IsInMyDocuments
        {
            get
            {
                return m_pageGetLocation.IsInMyDocuments;
            }
            set
            {
                m_pageGetLocation.IsInMyDocuments = value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string c_sEnglishTitle = "Initialize a Cluster by downloading from the Internet";
        #region Constructor()
        public WizInitializeFromRepository()
            : base("WizInitializeFromRepository", c_sEnglishTitle, JWU.GetBitmap("WizImportFile.gif"))
        {
            // Misc Wizard Dialog Settings
            NavigationColor = Color.Wheat;

            // Add the wizard's pages
            m_pageIntroduction = new WizRepo_Introduction();
            AddPage(m_pageIntroduction);

            m_pageGetRepoInfo = new WizRepo_GetRepoInfo();
            AddPage(m_pageGetRepoInfo);

            m_pageGetClusterName = new WizRepo_GetClusterName();
            AddPage(m_pageGetClusterName);

            m_pageGetLocation = new WizRepo_GetLocation();
            AddPage(m_pageGetLocation);

            m_pageSummary = new WizRepo_Summary();
            AddPage(m_pageSummary);
        }
        #endregion
        #region OMethod: void Localization()
        protected override void Localization()
        {
            LocDB.Localize(this, null);
        }
        #endregion
    }
}
