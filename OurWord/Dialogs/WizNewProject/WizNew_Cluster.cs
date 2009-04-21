#region ***** WizNew_Cluster.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_Cluster.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Obtains the team settings file
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
using OurWord.Utilities;
#endregion
#endregion

namespace OurWord.Dialogs.WizNewProject
{
    public partial class WizNew_Cluster : UserControl, IJW_WizPage
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
        #region VAttr{g}: List<string> ClusterList
        public List<string> ClusterList
        {
            get
            {
                return m_ClusterListView.ClusterList;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_Cluster()
        {
            InitializeComponent();

            m_ClusterListView.OnSelectedClusterChanged =
                new ClusterListView.SelectedClusterChanged(cmdSelectedClusterChanged);
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            // In case the user has done something with Explorer, re-scan MyDocuments
            // for the up-to-date list of clusters
            m_ClusterListView.Populate();

            // Select what we've chosen in the wizard (either in this session, or previously)
            m_ClusterListView.SelectedCluster = Wizard.ChosenCluster;
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
            return LocDB.GetValue(this, "strCluster", "Cluster", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdSelectedClusterChanged
        private void cmdSelectedClusterChanged(string sNewCluster)
        {
            if (!string.IsNullOrEmpty(sNewCluster))
                Wizard.ChosenCluster = sNewCluster;
        }
		#endregion

	}
}
