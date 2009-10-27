#region ***** Page_Cluster.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Cluster.cs
 * Author:  John Wimbish
 * Created: 18 Sep 2007
 * Purpose: Sets up clusters under MyDocuments
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using OurWordData;
using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class Page_Cluster : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_Cluster(DialogProperties _ParentDlg)
            : base(_ParentDlg)
        {
            InitializeComponent();
        }
        #endregion

		// DlgPropertySheet implementation ---------------------------------------------------
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return "idClusters";
			}
		}
		#endregion
		#region Method: void ShowHelp()
		public override void ShowHelp()
        {
            HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Clusters";
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
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Localize
            Control[] vExclude = new Control[] { m_ClusterListView };
            LocDB.Localize(this, vExclude);
        }
        #endregion

        #region Cmd: cmdCreateNewCluster
        private void cmdCreateNewCluster(object sender, EventArgs e)
        {
            m_ClusterListView.CreateNewCluster();
        }
        #endregion
        #region Cmd: cmdRenameCluster
        private void cmdRenameCluster(object sender, EventArgs e)
        {
            m_ClusterListView.InitiateRename();
        }
        #endregion
        #region Cmd: cmdDeleteCluster
        private void cmdDeleteCluster(object sender, EventArgs e)
        {
            m_ClusterListView.DeleteCluster();
        }
        #endregion
        #region Cmd: cmdMoveCluster
        private void cmdMoveCluster(object sender, EventArgs e)
        {
            m_ClusterListView.MoveCluster();
        }
        #endregion
    }
}
