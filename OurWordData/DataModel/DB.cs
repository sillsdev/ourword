/**********************************************************************************************
 * Project: Our Word!
 * File:    DB.cs
 * Author:  John Wimbish
 * Created: 10 Mar 2009
 * Purpose: A top level Database object. 
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
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using JWTools;
using OurWordData;
#endregion


namespace OurWordData.DataModel
{
    public class DB
    {
        // Parts of a Project ----------------------------------------------------------------
        #region SAttr{g/s}: DProject Project
        static public DProject Project
        {
            get
            {
                return s_Project;
            }
            set
            {
                s_Project = value;
            }
        }
        static DProject s_Project;
        #endregion
        #region SAttr{g}: DTeamSettings TeamSettings
        static public DTeamSettings TeamSettings
        {
            get
            {
                return Project.TeamSettings;
            }
        }
        #endregion
        #region SAttr{g}: static public DSFMapping Map - Styles <> Sf Markers
        public static DSFMapping Map
        {
            get
            {
                return TeamSettings.SFMapping;
            }
        }
        #endregion
        #region SAttr{g}: static public DStyleSheet StyleSheet
        static public DStyleSheet StyleSheet
        {
            get
            {
                return TeamSettings.OldStyleSheet;
            }
        }
        #endregion

        #region SAttr{g}: DTranslation FrontTranslation
        static public DTranslation FrontTranslation
        {
            get
            {
                if (null == Project)
                    return null;
                return Project.FrontTranslation;
            }
        }
        #endregion
        #region SAttr{g}: DBook FrontBook
        static public DBook FrontBook
        {
            get
            {
                if (null == Project || null == Project.SFront)
                    return null;
                return Project.SFront.Book;

            }
        }
        #endregion
        #region SAttr{g}: DSection FrontSection
        static public DSection FrontSection
        {
            get
            {
                if (null == Project)
                    return null;
                return Project.SFront;

            }
        }
        #endregion

        #region SAttr{g}: DTranslation TargetTranslation
        static public DTranslation TargetTranslation
        {
            get
            {
                if (null == Project)
                    return null;
                return Project.TargetTranslation;
            }
        }
        #endregion
        #region SAttr{g}: DBook TargetBook
        static public DBook TargetBook
        {
            get
            {
                if (null == Project || null == Project.STarget)
                    return null;
                return Project.STarget.Book;

            }
        }
        #endregion
        #region SAttr{g}: DSection TargetSection
        static public DSection TargetSection
        {
            get
            {
                if (null == Project)
                    return null;
                return Project.STarget;

            }
        }
        #endregion

        #region SAttr{g}: bool IsValidProject
        static public bool IsValidProject
        {
            get
            {
                if (null == Project)
                    return false;
                if (null == FrontBook)
                    return false;
                if (null == TargetBook)
                    return false;
                return true;
            }
        }
        #endregion
        #region SAttr{g}: bool TargetBookIsLocked
        static public bool TargetBookIsLocked
        {
            get
            {
                if (null == TargetBook)
                    return false;
                return TargetBook.Locked;
            }
        }
        #endregion

        // Misc Shorthand --------------------------------------------------------------------
        #region SAttr{g}: string Today - returns today's date as "2005-08-21" format.
        static public string Today
        {
            get
            {
                DateTime dt = DateTime.Today;
                return dt.ToString("yyyy-MM-dd");
            }
        }
        #endregion

        // Options stored in the registry ----------------------------------------------------
        const string c_sSubKey = "Options";
        const string c_keyPictureSearchPath = "PictureSearchPath";
        #region SAttr{g/s}: string PictureSearchPath
        static public string PictureSearchPath
        {
            get
            {
                return JW_Registry.GetValue(c_sSubKey, c_keyPictureSearchPath, "");
            }
            set
            {
                JW_Registry.SetValue(c_sSubKey, c_keyPictureSearchPath, value);
            }
        }
        #endregion

        const string c_sRegistryName = "UserName";
        #region SAttr{g/s}: string UserName
        static public string UserName
        {
            get
            {
                // Start with the name of the logged-in user
                string sDefaultName = Environment.UserName;
                if (string.IsNullOrEmpty(sDefaultName))
                    sDefaultName = Environment.MachineName;

                // Override it from the registry
                string sAuthor = JW_Registry.GetValue(c_sSubKey, c_sRegistryName, sDefaultName);

                // If we came up empty, then force it back to the machine name
                if (string.IsNullOrEmpty(sAuthor))
                {
                    sAuthor = sDefaultName;
                    JW_Registry.SetValue(c_sSubKey, c_sRegistryName, sDefaultName);
                }

                return sAuthor;
            }
            set
            {
                // Do nothing if we aren't using a reasonable name
                if (string.IsNullOrEmpty(value))
                    return;

                // Place it in the registry
                JW_Registry.SetValue(c_sSubKey, c_sRegistryName, value);
            }
        }
        #endregion

        #region SMethod: int GetYearThisAssemblyWasCompiled()
        static public int GetYearThisAssemblyWasCompiled()
        {
            var assembly = Assembly.GetAssembly(typeof(DB));
            Debug.Assert(null != assembly);
            var version = assembly.GetName().Version;
            var daysSince01Jan2000 = version.Build;
            var buildDate = new DateTime(2000, 1, 1).AddDays(daysSince01Jan2000);
            return buildDate.Year;
        }
        #endregion
    }

    public class Loc
        // Shorthand for various localization sections used at the DB level
    {
        #region SMethod: string GetTranslationStage(sItemID, sEnglish) - "TranslationStages"
        static public string GetTranslationStage(string sItemID, string sEnglish)
        {
            return LocDB.GetValue(
                new string[] { "TranslationStages" },
                sItemID,
                sEnglish,
                null, 
                null);
        }
        #endregion

        #region SMethod: string GetBookGroupings(sItemID, sEnglish) - "BookGroupings"
        static public string GetBookGroupings(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "BookGroupings" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetString(sItemID, sEnglish) -        "Strings"
        static public string GetString(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetNoteDefs(sItemID, sEnglish) -      "Strings\NoteDefs"
        static public string GetNoteDefs(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "NoteDefs" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetNotes(sItemID, sEnglish) -         "Strings\Notes"
        static public string GetNotes(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "Notes" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetStructureMessages(sItemID, sEnglish) -      "Messages\FileStructureMessages"
        static public string GetStructureMessages(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Messages", "FileStructureMessages" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetMessages(sItemID, sEnglish) -      "Messages"
        static public string GetMessages(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Messages" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion

    }


}
