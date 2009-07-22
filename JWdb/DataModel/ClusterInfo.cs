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
using JWdb;
#endregion
#endregion

namespace JWdb.DataModel
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

        #region Method: List<string> GetClusterLanguageList()
        public List<string> GetClusterLanguageList()
        {
            List<string> v = new List<string>();

            // Get the settings folder
            string sSettingsFolder = ParentFolder +
                Name +
                Path.DirectorySeparatorChar +
                DTeamSettings.SettingsFolderName +
                Path.DirectorySeparatorChar;

            // Get the otrans files in the settings folder
            string[] sFiles = Directory.GetFiles(sSettingsFolder,
                "*" + DTranslation.FileExtension,
                SearchOption.TopDirectoryOnly);

            // The base name of these are the languages
            foreach (string s in sFiles)
                v.Add(Path.GetFileNameWithoutExtension(s));

            return v;
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

        #region SMethod: void ScanForClusters()
        static public void ScanForClusters()
        {
            s_vClusters = new List<ClusterInfo>();

            // Places where we'll search for clusters
            var vsPossibleClusterLocations = new List<string>();
            vsPossibleClusterLocations.Add(JWU.GetMyDocumentsFolder(null));
            vsPossibleClusterLocations.Add(JWU.GetLocalApplicationDataFolder(
                ClusterInfo.c_sLanguageDataFolder));

            // Loop to search each location
            foreach (string sPossibleLocation in vsPossibleClusterLocations)
            {
                string[] sPotentialClusterFolders = Directory.GetDirectories(sPossibleLocation);

                foreach (string sFolder in sPotentialClusterFolders)
                {
                    string sSettingsFolder = sFolder + Path.DirectorySeparatorChar +
                        DTeamSettings.SettingsFolderName + Path.DirectorySeparatorChar;

                    if (Directory.Exists(sSettingsFolder))
                    {
                        string sClusterName = JWU.ExtractRightmostSubFolder(sFolder);
                        s_vClusters.Add(new ClusterInfo(sClusterName, sPossibleLocation));
                    }
                }
            }
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
    }


}
