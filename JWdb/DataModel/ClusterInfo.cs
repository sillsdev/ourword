#region ***** ClusterInfo.cs *****
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
}
