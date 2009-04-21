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
        #region Attr{g/s}: string SelectedCluster
        public string SelectedCluster
        {
            get
            {
                if (Clusters.SelectedItems.Count == 1)
                    return Clusters.SelectedItems[0].Text;
                return null;
            }
            set
            {
                foreach (ListViewItem item in Clusters.Items)
                {
                    item.Selected = (item.Text == value);
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
        #region VAttr{g}: List<s> ClusterList
        public List<string> ClusterList
        {
            get
            {
                // We calculate this from what is in the ListView control
                var v = new List<string>();
                foreach (ListViewItem item in Clusters.Items)
                    v.Add(item.Text);
                return v;
            }
        }
        #endregion

        // Initialize List Contents ----------------------------------------------------------
        #region Method: void Populate()
        public void Populate()
        {
            Clusters.Items.Clear();

            var v = GetClusterListFromDisk();
            foreach (string sCluster in v)
            {
                ListViewItem item = new ListViewItem(sCluster);
                Clusters.Items.Add(item);
            }
        }
        #endregion      
        #region SMethod: List<s> GetClusterListFromDisk()
        static public List<string> GetClusterListFromDisk()
        {
            List<string> v = new List<string>();

            // Get a collection of all of the folders in My Documents
            string sMyDocumentsFolder = JWU.GetMyDocumentsFolder(null);
            string[] sPotentialClusterFolders = Directory.GetDirectories(sMyDocumentsFolder);

            // Work through them, looking for those with a Settings Folder with an OWT file.
            foreach (string sFolder in sPotentialClusterFolders)
            {
                // Get the files in the settings folder
                string sSettingsFolder = sFolder + Path.DirectorySeparatorChar +
                    DTeamSettings.SettingsFolderName + Path.DirectorySeparatorChar;
                if (!Directory.Exists(sSettingsFolder))
                    continue;
                string[] sFiles = Directory.GetFiles(sSettingsFolder);

                // If we have an OWT extension, then we have a cluster folder
                foreach (string sFile in sFiles)
                {
                    if (Path.GetExtension(sFile) == DTeamSettings.FileExtension)
                    {
                        // Retrieve the right-most folder; its the name of the cluster
                        string sClusterName = JWU.ExtractRightmostSubFolder(sFolder);

                        // Add it to our list; and we're done with this one.
                        v.Add(sClusterName);
                        break;
                    }
                }
            }

            // If we didn't get one, we create one so that we always have a default
            // cluster to work from.
            if (v.Count == 0)
            {
                DTeamSettings ts = new DTeamSettings();
                ts.EnsureInitialized();
                ts.Write(new NullProgress());
                v.Add(ts.DisplayName);
            }

            return v;
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

        // Localizations ---------------------------------------------------------------------
        #region Attr{g}: string NewClusterName - returns localized "New Cluster"
        string NewClusterName
        {
            get
            {
                return Loc.GetString("kNewCluster", "New Cluster");
            }
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

        // Events we go ahead and respond to here --------------------------------------------
        private void cmdAfterLabelEdit(object sender, LabelEditEventArgs e)
            // Rename the cluster. This happens in response to a request to rename the cluster,
            // via InitiateRename, which puts the label into edit mode.
        {
            // Retrieve the Old and the proposed New names
            string sOldClusterName = SelectedCluster;
            string sNewClusterName = e.Label;

            // Nothing to do if they're the same, or if New is empty
            if (string.IsNullOrEmpty(sNewClusterName) ||  sNewClusterName == sOldClusterName)
                return;

            // If it is "New Cluster", then they didn't rename after CreateNewCluster,
            // so we'll remove it from the list, and assume the user's intent was to
            // cancel.
            if (sNewClusterName == NewClusterName)
            {
                ListViewItem item = FindItem(NewClusterName);
                if (null != item)
                    Clusters.Items.Remove(item);
                // Select the first item so that the list continues to have something selected
                if (Clusters.Items.Count > 0)
                    Clusters.Items[0].Selected = true;
                return;
            }

            // If neither the Old nor the New physically exist on the disk, then we're
            // creating a new Cluster. 
            var v = GetClusterListFromDisk();
            if (!v.Contains(sNewClusterName) && !v.Contains(sOldClusterName))
            {
                DTeamSettings ts = new DTeamSettings(sNewClusterName);
                ts.Write(new NullProgress());
                return;
            }

            // If we're here, then we're renaming an existing cluster
            // TODO: Implementation
            MessageBox.Show("Apologies! Renaming an existing cluster isn't implemented yet.",
                "OurWord", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }



        // Cluster-Related Operations from Outside -------------------------------------------
        #region Method: void InitiateRename()
        public void InitiateRename()
        {
            ListViewItem item = m_lvClusters.FocusedItem;
            if (null == item)
                return;

            item.BeginEdit();
        }
        #endregion
        #region Method: void CreateNewCluster()
        public void CreateNewCluster()
        {
            // If we already have a cluster of this name, then we will just edit it.
            ListViewItem itemNew = FindItem(NewClusterName);

            // Otherwise, we need to create and add it
            itemNew = new ListViewItem(NewClusterName);
            Clusters.Items.Add(itemNew);

            // Select the item, then invokve edit on it. Then, when the user names it 
            // to something meaningful, we'll create it (via cmdAfterLabelEdit)
            itemNew.Selected = true;
            itemNew.BeginEdit();
        }
        #endregion

        public void DeleteCluster()
        {
            // TODO: Implementation
            MessageBox.Show("Apologies! Deleting an existing cluster isn't implemented yet.",
                "OurWord", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
