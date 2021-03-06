﻿#region ***** ClusterInfo.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ClusterInfo.cs
 * Author:  John Wimbish
 * Created: 3 June 2009
 * Purpose: Information about a cluster on the disk file 
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using JWTools;
using OurWordData;
#endregion
#endregion

namespace OurWordData.DataModel
{
    public class ClusterInfo
    {
        public const string c_sLanguageDataFolder = "Language Data";

        #region Attr{g/s}: string Name
        public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                m_sName = value;
            }
        }
        string m_sName;
        #endregion
        #region Attr{g/s}: string ParentFolder
        public string ParentFolder
        {
            get
            {
                return m_sParentFolder;
            }
            set
            {
                m_sParentFolder = value;
            }
        }
        string m_sParentFolder;
        #endregion

        #region Attr{g}: bool IsInMyDocuments
        public bool IsInMyDocuments
        {
            get
            {
                if (ParentFolder == JWU.GetMyDocumentsFolder(null))
                    return true;
                return false;
            }
        }
        #endregion
        #region VAttr{g}: string Location
        public string Location
        {
            get
            {
                if (null == DB.Project)
                    return "Location";

                // My Documents
                if (ParentFolder == JWU.GetMyDocumentsFolder(null))
                    return Loc.GetString("kMyDocuments", "My Documents");

                // My Application Data (roaming)
                // Note: We don't localize c_sLanguageDataFolder, because if the user
                // changed the UI language, they'd loose track of where their data is.
                if (ParentFolder == JWU.GetLocalApplicationDataFolder(c_sLanguageDataFolder))
                    return Loc.GetString("kMyApplicationData", "My Application Data");

                return ParentFolder;
            }
        }
        #endregion
        #region VAttr{g}: string ClusterFolder
        public string ClusterFolder
        {
            get
            {
                string s = ParentFolder;
                if (s[s.Length - 1] != Path.DirectorySeparatorChar)
                    s += Path.DirectorySeparatorChar;
                s += Name;
                s += Path.DirectorySeparatorChar;
                return s;
            }
        }
        #endregion

        #region Constructor(sName, sParentFolder)
        public ClusterInfo(string sName, string sParentFolder)
        {
            m_sName = sName;
            m_sParentFolder = sParentFolder;
        }
        #endregion

        #region Method: List<string> GetClusterLanguageList(bProjectsOnly)
        public List<string> GetClusterLanguageList(bool bProjectsOnly)
        {
            var v = new List<string>();

            var sExtension = (bProjectsOnly) ? 
                DProject.FileExtension : DTranslation.FileExtension;

            // Get the settings folder
            var sSettingsFolder = ParentFolder +
                Name +
                Path.DirectorySeparatorChar +
                DTeamSettings.SettingsFolderName +
                Path.DirectorySeparatorChar;

            // Get the owp files in the settings folder
            var sFiles = Directory.GetFiles(sSettingsFolder,
                "*" + sExtension,
                SearchOption.TopDirectoryOnly);

            // The base name of these are the languages
            foreach (var s in sFiles)
                v.Add(Path.GetFileNameWithoutExtension(s));

            return v;
        }
        #endregion

        #region Method: string GetProjectPath(sProjectName)
        public string GetProjectPath(string sProjectName)
        {
            var sPath = ClusterFolder +
                ".Settings" + Path.DirectorySeparatorChar +
                sProjectName + ".owp";
            return sPath;
        }
        #endregion

        // User access of each projct, stored in the registry
        const string c_sSubKeyAccess = "ProjectAccess";
        #region Method: bool GetUserCanAccess(sProjectName)
        public bool GetUserCanAccess(string sProjectName)
        {
            return JW_Registry.GetValue(c_sSubKeyAccess + "\\" + Name, sProjectName, true);
        }
        #endregion
        #region Method: void SetUserCanAccess(sProjectName, bCanAccess)
        public void SetUserCanAccess(string sProjectName, bool bCanAccess)
        {
            JW_Registry.SetValue(c_sSubKeyAccess + "\\" + Name, sProjectName, bCanAccess);
        }
        #endregion
    }

    static public class ClusterList
    {
        #region SAttr{g}: List<ClusterInfo> Clusters
        static public List<ClusterInfo> Clusters
        {
            get
            {
                if (null == s_vClusters)
                    ScanForClusters();
                return s_vClusters;
            }
        }
        static List<ClusterInfo> s_vClusters;
        #endregion

        #region SMethod:  void CreateNewCluster(bool bStoreInMyDocuments, string sNewClusterName)
        static public void CreateNewCluster(bool bStoreInMyDocuments, string sNewClusterName)
            // A Cluster is defined as (1) a Cluster Folder, which (2) owns a
            // Settings folder. 
        {
            // The owning folder is either in the publically visible MyDocuments, or
            // in the relatively hidden local data folder.
            string sParentFolder = (bStoreInMyDocuments) ?
                JWU.GetMyDocumentsFolder(null) :
                JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder);

            // Build the path to the cluster folder
            string sSettingsPath =
                sParentFolder + Path.DirectorySeparatorChar +
                sNewClusterName + Path.DirectorySeparatorChar +
                DTeamSettings.SettingsFolderName + Path.DirectorySeparatorChar;

            // Create the folder
            if (!Directory.Exists(sSettingsPath))
                Directory.CreateDirectory(sSettingsPath);
        }
        #endregion

        #region SMethod: void ScanForClusters()
        static public void ScanForClusters()
        {
            s_vClusters = new List<ClusterInfo>();

            // Locations (base folders) where we'll search for clusters
            var vsPossibleClusterLocations = new List<string>
            {
                JWU.GetMyDocumentsFolder(null),
                JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder)
            };

            // Loop to search each location
            foreach (var sPossibleLocation in vsPossibleClusterLocations)
            {
                var sPotentialClusterFolders = Directory.GetDirectories(sPossibleLocation);

                foreach (var sFolder in sPotentialClusterFolders)
                {
                    var sSettingsFolder = sFolder + Path.DirectorySeparatorChar +
                        DTeamSettings.SettingsFolderName + Path.DirectorySeparatorChar;

                    if (!Directory.Exists(sSettingsFolder)) 
                        continue;

                    var sClusterName = JWU.ExtractRightmostSubFolder(sFolder);
                    s_vClusters.Add(new ClusterInfo(sClusterName, sPossibleLocation));
                }
            }

            // If we came up empty, we need to create a cluster, so that we always
            // have one to put things into. We'll place it into MyDocuments
            if (s_vClusters.Count != 0) 
                return;
            CreateNewCluster(true, "OurWordData");
            ScanForClusters();
        }
        #endregion

        #region SMethod: ClusterInfo FindClusterInfo(string sName)
        static public ClusterInfo FindClusterInfo(string sName)
        {
            // It is always safest, albeit time consuming, to re-scan, because
            // we never know where changes might have happened. (Without the re-scan,
            // our Unit tests all fail, because they create and delete the test cluster
            // folders.)
            ScanForClusters();

            foreach (ClusterInfo ci in Clusters)
            {
                if (ci.Name == sName)
                    return ci;
            }
            return null;
        }
        #endregion

        #region SMethod: bool GetUserCanAccessProject(sClusterName, sProjectName)
        static public bool GetUserCanAccessProject(string sClusterName, string sProjectName)
        {
            var ci = FindClusterInfo(sClusterName);
            if (null == ci)
                return false;
            return ci.GetUserCanAccess(sProjectName);
        }
        #endregion
        #region SMethod: bool UserCanAccessAllProjects
        static public bool UserCanAccessAllProjects
        {
            get
            {
                bool bHasUnchecked = false;
                bool bHasChecked = false;

                foreach (ClusterInfo ci in Clusters)
                {
                    foreach (string sProject in ci.GetClusterLanguageList(true))
                    {
                        var bChecked = GetUserCanAccessProject(ci.Name, sProject);
                        if (bChecked)
                            bHasChecked = true;
                        else
                            bHasUnchecked = true;

                    }
                }

                // The answer is True if either all are checked, or all are unchecked
                if (bHasUnchecked && !bHasChecked)
                    return true;
                if (!bHasUnchecked && bHasChecked)
                    return true;

                return false;
            }
        }
        #endregion
        #region SAttr{g}: string UserCanAccessAllProjectsFriendly
        static public string UserCanAccessAllProjectsFriendly
        {
            get
            {
                if (UserCanAccessAllProjects)
                    return Loc.GetString("CanAccessAllProjects", "All");
                return Loc.GetString("CanAccessLimitedProjects", "Limited");
            }
        }
        #endregion
    }

}
