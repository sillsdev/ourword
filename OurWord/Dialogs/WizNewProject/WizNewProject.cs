/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNewProject.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Wizard that manages creating a new project in OurWord
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
    class WizNewProject : JW_Wizard
	{
        // Attrs We Elicit from the User -----------------------------------------------------
        #region VAttr{g}: string ProjectName
        public string ProjectName
        {
            get
            {
                if (null == m_pageProjectName)
                    return null;
                return m_pageProjectName.ProjectName;
            }
        }
        #endregion
        #region VAttr{g}: string FrontName
		public const string c_sRegKeyLastFrontUsed = "LastFrontUsed";
		public string FrontName
        {
            get
            {
				// If we have no front name, init it from the registry. Don't worry
				// about it being stale, because the Page will not display it if it
				// is no longer a possiblity; and thus when Finishing the wizard,
				// it will have been changed to something valid.
				if (string.IsNullOrEmpty(m_sFrontName))
					m_sFrontName = JW_Registry.GetValue(c_sRegKeyLastFrontUsed, "");

				return m_sFrontName;
            }
			set
			{
				m_sFrontName = value;
				JW_Registry.SetValue(c_sRegKeyLastFrontUsed, m_sFrontName);
			}
        }
		string m_sFrontName = "";
        #endregion
		#region VAttr{g}: string ChosenCluster
		public const string c_sRegKeyLastCluster = "LastClusterUsed";
		public string ChosenCluster
		{
			get
			{
				// See what we had in the registry, if anything
				if (string.IsNullOrEmpty(m_sChosenCluster))
				{
					Debug.Assert(Clusters.Count > 0);
					string sLastTime = JW_Registry.GetValue(c_sRegKeyLastCluster, "");
					if (Clusters.IndexOf(sLastTime) != -1)
						m_sChosenCluster = sLastTime;
					else
						m_sChosenCluster = Clusters[0];
				}

				Debug.Assert(!string.IsNullOrEmpty(m_sChosenCluster));
				return m_sChosenCluster;
			}
			set
			{
				Debug.Assert(Clusters.IndexOf(value) != -1);
				m_sChosenCluster = value;
				JW_Registry.SetValue(c_sRegKeyLastCluster, m_sChosenCluster);
			}
		}
		string m_sChosenCluster;
		#endregion
        #region VAttr{g}: bool LaunchPropertiesDialogWhenDone
        public bool LaunchPropertiesDialogWhenDone
        {
            get
            {
                if (null == m_pageSummary)
                    return false;
                return m_pageSummary.LaunchPropertiesDialogWhenDone;
            }
        }
        #endregion
		#region VAttr{g}: bool CreatingNewFront
		public bool CreatingNewFront
		{
			get
			{
				return m_pageFrontInfo.CreatingNewFront;
			}
		}
		#endregion

		// Pages (in order of appearance) ----------------------------------------------------
        WizNew_Introduction m_pageIntroduction;
        WizNew_Cluster m_pageCluster;
        WizNew_ProjectName m_pageProjectName;
        WizNew_FrontInfo m_pageFrontInfo;
        WizNew_Summary m_pageSummary;

		// Other Attrs we use ----------------------------------------------------------------
		#region VAttr{g}: List<s> Clusters
		public List<string> Clusters
		{
			get
			{
                Debug.Assert(null != m_pageCluster);
                return m_pageCluster.ClusterList;
			}
		}
		#endregion
		#region Attr{g}: bool ShowClusterChoicePage
		public bool ShowClusterChoicePage
		{
			get
			{
				return m_bShowClusterChoicePage;
			}
		}
		bool m_bShowClusterChoicePage = false;
		#endregion
		#region VAttr{g}: List<string> Languages
		public List<string> Languages
		{
			get
			{
				return DTeamSettings.GetLanguageListFromDisk(ChosenCluster);
			}
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string c_sEnglishTitle = "Create a New Project";
        #region Constructor()
        public WizNewProject()
            : base("WizNewProject", c_sEnglishTitle, JWU.GetBitmap("WizImportFile.gif"))
        {
            // Go ahead and create the Clusters page, even if we don't use it, so that
            // we'll have the list of clusters for use elsewhere in the wizard. The
            // page has a ClusterListView, the internals of which will automatically
            // create one cluster ("OurWord") if there are otherwise none on this computer.
		    m_pageCluster = new WizNew_Cluster();

            // Misc Wizard Dialog Settings
            NavigationColor = Color.Wheat;

            // Introduction Page: tell the user what we're going to do
            m_pageIntroduction = new WizNew_Introduction();
            AddPage(m_pageIntroduction);

			// Clusters: If we have more than one cluster, then we'll need to show the page 
			// for choosing which one.
			if (Clusters.Count > 1)
			{
				m_bShowClusterChoicePage = true;
				AddPage(m_pageCluster);
			}

			// Project Name: the user supplies the name of this language/translation/project
			// (they're all the same)
            m_pageProjectName = new WizNew_ProjectName();
            AddPage(m_pageProjectName);

			// Front Translation: the user supplies the name of the source
            m_pageFrontInfo = new WizNew_FrontInfo();
            AddPage(m_pageFrontInfo);

			// Summary: Confirm to the user what we're about to do
            m_pageSummary = new WizNew_Summary();
            AddPage(m_pageSummary);
        }
        #endregion
        #region OMethod: void Localization()
        protected override void Localization()
        {
            Control[] vExclude = new Control[0];

            LocDB.Localize(this, vExclude);
        }
        #endregion
    }
}
