#region ***** ClusterListView.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ClusterListView.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2009
 * Purpose: Encapsulates the various manipulations on clusters.
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
using OurWord.Dialogs;
#endregion
#endregion

namespace OurWord.Utilities
{
    public partial class ClusterListView : UserControl
    {
        // List of Clusters ------------------------------------------------------------------
        #region Ctrl: ListView Clusters
        ListView Clusters
        {
            get
            {
                return m_lvClusters;
            }
        }
        #endregion
        #region Attr{g/s}: ClusterInfo SelectedCluster
        public ClusterInfo SelectedCluster
        {
            get
            {
                if (Clusters.SelectedItems.Count == 1)
                    return Clusters.SelectedItems[0].Tag as ClusterInfo;
                return null;
            }
            set
            {
                foreach (ListViewItem item in Clusters.Items)
                {
                    item.Selected = (item.Tag as ClusterInfo == value);
                }
            }
        }
        #endregion
        #region Attr{g}: bool HasSelectedCluster
        public bool HasSelectedCluster
        {
            get
            {
                if (m_lvClusters.SelectedItems.Count != 1)
                    return false;
                return true;
            }
        }
        #endregion
        #region Method: bool ClusterExists(string sClusterName)
        public bool ClusterExists(string sClusterName)
        {
            if (null != FindItem(sClusterName))
                return true;
            return false;
        }
        #endregion
        #region Method: ListViewItem FindItem(string sClusterName)
        ListViewItem FindItem(string sClusterName)
        {
            foreach (ListViewItem item in Clusters.Items)
            {
                if (item.Text == sClusterName)
                    return item;
            }
            return null;
        }
        #endregion

        // Initialize List Contents ----------------------------------------------------------
        #region Method: void Populate()
        public void Populate()
        {
            Clusters.Items.Clear();

            foreach (ClusterInfo ci in ClusterList.Clusters)
            {
                ListViewItem item = new ListViewItem(ci.Name);
                item.SubItems.Add(ci.Location);
                item.Tag = ci;
                Clusters.Items.Add(item);
            }
        }
        #endregion      

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ClusterListView()
        {
            InitializeComponent();

            // Retrieve the list of clusters currently on the user's computer, and
            // populate the listview with it
            Populate();
        }
        #endregion

        // SelectedClusterChanged event ------------------------------------------------------
        public delegate void SelectedClusterChanged(string sSelectedCluster);
        #region Handler{s}: SelectedClusterChanged OnSelectedClusterChanged
        public SelectedClusterChanged OnSelectedClusterChanged
        {
            set
            {
                m_OnSelectedClusterChanged = value;
            }
        }
        public SelectedClusterChanged m_OnSelectedClusterChanged;
        #endregion
        #region Cmd: cmdSelectedIndexChanged
        private void cmdSelectedIndexChanged(object sender, EventArgs e)
        {
            // Determine which item is currently selected
            if (m_lvClusters.SelectedItems.Count != 1)
                return;
            ListViewItem item = m_lvClusters.SelectedItems[0];

            // Invoke the delegate
            if (null != m_OnSelectedClusterChanged)
                m_OnSelectedClusterChanged(item.Text);
        }
        #endregion

        // Command Handlers for creating, renaming, deleting clusters ------------------------
        #region Method: void InitiateRename()
        public void InitiateRename()
        {
            ListViewItem item = m_lvClusters.FocusedItem;
            if (null == item)
                return;

            item.BeginEdit();
        }
        #endregion
        #region Cmd: cmdAfterLabelEdit
        private void cmdAfterLabelEdit(object sender, LabelEditEventArgs e)
            // Rename the cluster. This happens in response to a request to rename the cluster,
            // via InitiateRename, which puts the label into edit mode.
        {
            // Retrieve the Old and the proposed New names
            if (Clusters.SelectedItems.Count != 1)
                return;
            string sOldClusterName = Clusters.SelectedItems[0].Text;
            string sNewClusterName = e.Label;

            // Nothing to do if they're the same, or if New is empty
            if (string.IsNullOrEmpty(sNewClusterName) ||  sNewClusterName == sOldClusterName)
                return;

            try
            {
                // If we're here, then we're renaming an existing cluster
                ClusterInfo ciOld = SelectedCluster;
                ClusterInfo ciNew = new ClusterInfo(sNewClusterName, ciOld.ParentFolder);
                Directory.Move(ciOld.ClusterFolder, ciNew.ClusterFolder);
                ciOld.Name = sNewClusterName;

                // Update everything
                ClusterList.ScanForClusters();
                Populate();
                SelectedCluster = ClusterList.FindClusterInfo(sNewClusterName);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Method: void CreateNewCluster()
        public void CreateNewCluster()
        {
            // Invoke the dialog. The dialog ensure a valid cluster would be created
            var dlg = new DlgNewCluster();
            if (DialogResult.OK != dlg.ShowDialog(this))
                return;

            // A Cluster is defined as (1) a Cluster Folder, which (2) owns a
            // Settings folder. 
            string sParentFolder = (dlg.StoreInMyDocuments) ?
                JWU.GetMyDocumentsFolder(null) :
                JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder);
            string sSettingsPath =
                sParentFolder + Path.DirectorySeparatorChar +
                dlg.NewClusterName + Path.DirectorySeparatorChar +
                DTeamSettings.SettingsFolderName + Path.DirectorySeparatorChar;
            if (!Directory.Exists(sSettingsPath))
                Directory.CreateDirectory(sSettingsPath);

            // Recalculate the list of clusters
            ClusterList.ScanForClusters();

            // Recalculate the listview and select the new item
            Populate();
            SelectedCluster = ClusterList.FindClusterInfo(dlg.NewClusterName);
        }
        #endregion
        #region Method: DeleteCluster
        public void DeleteCluster()
        {
            // Retrieve the cluster the user wishes to delete
            ClusterInfo ciDelete = SelectedCluster;
            if (null == ciDelete)
                return;

            // Display a major warning message
            bool bProceed = LocDB.Message("kConfirmDeleteCluster",
                "Are you certain you want to delete cluster \"{0}\"?\n\n" +
                "This will remove all of the cluster's language data from your disk permanently; " +
                "the action cannot be undone.",
                new string[] { ciDelete.Name },
                LocDB.MessageTypes.WarningYN);
            if (!bProceed)
                return;

            // Display a second warning message
            bProceed = LocDB.Message("kConfirmDeleteCluster2",
                "Excuse this second query, but we want to make absolutely sure!\n\n" +
                "Are you certain you want to delete cluster \"{0}\" and " +
                "all data that goes with it?",
                new string[] { ciDelete.Name },
                LocDB.MessageTypes.WarningYN);
            if (!bProceed)
                return;

            // If we're here, then they're serious. Delete the folder
            string sClusterFolder = ciDelete.ParentFolder;
            if (sClusterFolder[sClusterFolder.Length - 1] != Path.DirectorySeparatorChar)
                sClusterFolder += Path.DirectorySeparatorChar;
            sClusterFolder += (ciDelete.Name + Path.DirectorySeparatorChar);

            JWU.SafeFolderDelete(sClusterFolder);

            // Recalculate our list of clusters
            ClusterList.ScanForClusters();

            // Recalculate the listview and select the first item
            Populate();
            if (Clusters.Items.Count > 0)
                SelectedCluster = (Clusters.Items[0].Tag as ClusterInfo);
        }
        #endregion
        #region Method: void MoveCluster()
        public void MoveCluster()
        {
            // Get the currently selected cluster
            ClusterInfo ci = SelectedCluster;
            if (null == ci)
                return;

            // Find out where to move it to
            var dlg = new DlgMoveCluster();
            dlg.StoreInMyDocuments = ci.IsInMyDocuments;
            if (DialogResult.OK != dlg.ShowDialog(this.Parent))
                return;

            // Don't bother if the destination didn't change
            if (ci.IsInMyDocuments == dlg.StoreInMyDocuments)
                return;

            // Get the new destination (Parent Folder)
            string sParentFolder = (dlg.StoreInMyDocuments) ?
                JWU.GetMyDocumentsFolder(null) :
                JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder);
                ClusterInfo ciNew = new ClusterInfo(ci.Name, sParentFolder);

            // If the new destination already exists, then we can't do the move
            if (Directory.Exists(ciNew.ClusterFolder))
            {
                LocDB.Message("msgFolderAlreadyExists",
                    "Unable to move the Cluster because a folder of that name already exists.",
                    null, LocDB.MessageTypes.Error);
                return;
            }

            // Do the move
            Cursor.Current = Cursors.WaitCursor;
            JWU.Move(ci.ClusterFolder, ciNew.ClusterFolder);
            Cursor.Current = Cursors.Default; ;

            // Update everything
            ClusterList.ScanForClusters();
            Populate();
            SelectedCluster = ClusterList.FindClusterInfo(ci.Name);
        }
        #endregion
    }


}
