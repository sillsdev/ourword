#region ***** Stage.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Stage.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Represents the translation stage of the book
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using System.Text;
using System.Threading;
using System.IO;
using JWTools;
using OurWordData;
#endregion
#endregion

namespace OurWordData.DataModel
{
    public class Stage : IComparable<Stage>
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g}: int ID - historical data id's, don't change
        public int ID
        {
            get
            {
                return m_ID;
            }
        }
        int m_ID;
        #endregion
        #region Attr{g}: string LocID - ID for the localization database
        public string LocID
        {
            get
            {
                return m_sLocID;
            }
        }
        private string m_sLocID = "";
        #endregion
        #region Attr{g}: string EnglishAbbrev - A short name for the stage
        public string EnglishAbbrev
        {
            get
            {
                return m_sEnglishAbbrev;
            }
        }
        private string m_sEnglishAbbrev = "";
        #endregion
        #region Attr{g}: string EnglishName - The full name for the stage
        public string EnglishName
        {
            get
            {
                return m_sEnglishName;
            }

        }
        private string m_sEnglishName = "";
        #endregion

        // Project tweakable -----------------------------------------------------------------
        #region Attr{g/s}: int SortIndex
        public int SortIndex
        {
            get
            {
                return m_iSortIndex;
            }
            set
            {
                m_iSortIndex = value;
            }
        }
        int m_iSortIndex;
        #endregion
        #region Attr{g/s}: bool UsedInThisProject
        public bool UsedInThisProject
        {
            get
            {
                return m_bUsedInThisProject;
            }
            set
            {
                m_bUsedInThisProject = value;
            }
        }
        bool m_bUsedInThisProject;
        #endregion

        // Virtuals --------------------------------------------------------------------------
        #region Vattr{g}: string LocalizedAbbrev
        public string LocalizedAbbrev
        {
            get
            {
                return Loc.GetTranslationStage("abbrev" + m_sLocID, EnglishAbbrev);
            }
        }
        #endregion
        #region Vattr{g}: string LocalizedName
        public string LocalizedName
        {
            get
            {
                return Loc.GetTranslationStage(m_sLocID, EnglishName);
            }
        }
        #endregion

        // ID's ------------------------------------------------------------------------------
        #region IDs
        // IMPORTANT: The IDs must not be changed, as they are the ones I set up
        // originally in 2004, and thus are in Timor and Tomohon books. They must
        // stay the same in order to maintain backward compatability.
        public const int c_idDraft = 0;
        public const int c_idTeamCheck = 1;
        public const int c_idAdvisorCheck = 2;
        public const int c_idCommunityCheck = 3;
        public const int c_idBackTranslation = 6;
        public const int c_idConsultantCheck = 4;
        public const int c_idFinalForPrinting = 5;
        public const int c_idFinalRevision = 7;

        public const string c_sDraft = "Draft";
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(nID, iSortIndex, bUsedInThisProject, sLocID, sEnglishAbbrev, sEnglishName)
        public Stage(int id, int iSortIndex, bool bUsedInThisProject, string sLocID, string sEnglishAbbrev, string sEnglishName)
        {
            m_ID = id;
            m_iSortIndex = iSortIndex;
            m_bUsedInThisProject = bUsedInThisProject;
            m_sLocID = sLocID;
            m_sEnglishAbbrev = sEnglishAbbrev;
            m_sEnglishName = sEnglishName;
        }
        #endregion
        #region int CompareTo(other)
        public int CompareTo(Stage other)
        {
            if (other == this)
                return 0;

            return (SortIndex < other.SortIndex) ? -1 : 1;
        }
        #endregion
        #region OMethod: string ToString()
        public override string ToString()
        {
            return LocalizedName;
        }
        #endregion
    }

    public class StageList : List<Stage>
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public StageList()
        {
            InitializeToFactoryDefaults();
        }
        #endregion
        #region Method: void InitializeToFactoryDefaults()
        public void InitializeToFactoryDefaults()
        {
            Clear();

            Add(new Stage(Stage.c_idDraft, 0, true,
                Stage.c_sDraft, "Draft", "Draft"));

            Add(new Stage(Stage.c_idTeamCheck, 1, true, 
                "TeamCheck", "Team", "Team Check"));

            Add(new Stage(Stage.c_idAdvisorCheck, 2, true, 
                "AdvisorCheck", "Adv", "Advisor Check"));

            Add(new Stage(Stage.c_idCommunityCheck, 3, true, 
                "CommunityCheck", "Comm", "Community Check"));

            Add(new Stage(Stage.c_idBackTranslation, 4, true, 
                "BackTranslation", "BT", "Back Translation"));

            Add(new Stage(Stage.c_idConsultantCheck, 5, true, 
                "ConsultantCheck", "Consult", "Consultant Check"));

            Add(new Stage(Stage.c_idFinalForPrinting, 6, true, 
                "FinalForPrinting", "Final", "Final For Printing"));

            Add(new Stage(Stage.c_idFinalRevision, 7, true, 
                "FinalRevisions", "Rev", "Final Revisions"));

            Sort();
        }
        #endregion

        // List Access -----------------------------------------------------------------------
        #region Method: Stage Find(nID)
        public Stage Find(int nID)
        {
            foreach (Stage stage in this)
            {
                if (stage.ID == nID)
                    return stage;
            }
            return null;
        }
        #endregion
        #region Method: Stage Find(sEnglishAbbrev)
        public Stage Find(string sEnglishAbbrev)
        {
            return Find(FindBy.EnglishAbbrev, sEnglishAbbrev);
        }
        #endregion
        #region Method: Stage Find(FindBy, s)
        public enum FindBy { EnglishName, EnglishAbbrev, LocalizedName, LocalizedAbbrev };
        public Stage Find(FindBy criteria, string s)
        {
            foreach (Stage stage in this)
            {
                switch (criteria)
                {
                    case FindBy.EnglishAbbrev:
                        if (stage.EnglishAbbrev == s)
                            return stage;
                        break;

                    case FindBy.EnglishName:
                        if (stage.EnglishName == s)
                            return stage;
                        break;

                    case FindBy.LocalizedAbbrev:
                        if (stage.LocalizedAbbrev == s)
                            return stage;
                        break;

                    case FindBy.LocalizedName:
                        if (stage.LocalizedName == s)
                            return stage;
                        break;
                }
            }
            return null;
        }
        #endregion

        #region Method: void MoveUp(Stage)
        public void MoveUp(Stage stage)
        {
            for (int i = 0; i < Count - 1; i++)
            {
                var StageBefore = this[i];
                if (stage == this[i + 1])
                {
                    int n = StageBefore.SortIndex;
                    StageBefore.SortIndex = stage.SortIndex;
                    stage.SortIndex = n;
                }
            }
            Sort();
        }
        #endregion
        #region Method: void MoveDown(Stage)
        public void MoveDown(Stage stage)
        {
            for (int i = 1; i < Count; i++)
            {
                var StageAfter = this[i];
                if (stage == this[i - 1])
                {
                    int n = StageAfter.SortIndex;
                    StageAfter.SortIndex = stage.SortIndex;
                    stage.SortIndex = n;
                }
            }
            Sort();
        }
        #endregion
        #region Attr[g}: Stage Draft - generally the default stage if no other is specified
        public Stage Draft
        {
            get
            {
                var draft = Find("Draft");
                Debug.Assert(null != draft);
                return draft;
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Method: string ToSaveString()
        public string ToSaveString()
        {
            string s = "";

            foreach (Stage stage in this)
            {
                s += (stage.UsedInThisProject) ? "+" : "-";
                s += stage.EnglishAbbrev;
                s += " ";
            }

            return s.Trim();
        }
        #endregion
        #region Method: void FromSaveString(string s)
        public void FromSaveString(string s)
            // We expect a string along the lines of
            // "+Draft -Adv +Team +Comm +BT +Final +Consult -Rev"
        {
            // Make sure we have the set of stages in the list
            InitializeToFactoryDefaults();

            // Re-do the sort order by end-user settings
            try
            {
                // Set all of the existing sort indexes to -1
                foreach (Stage stage in this)
                    stage.SortIndex = -1;

                // Break the string down into its parts. 
                var vs = s.Split(new char[] { ' ' });

                // Loop through each one
                for (int i = 0; i < vs.Length; i++)
                {
                    // Retrieve the next part
                    string sStageInfo = vs[i];
                    if (string.IsNullOrEmpty(sStageInfo) || sStageInfo.Length < 2)
                        throw new Exception("Bad data in Stage attribute.");

                    // Parse whether this stage is used or not in this project
                    bool bUsedInThisProject = true;
                    if (sStageInfo.Length > 0 && sStageInfo[0] == '-')
                        bUsedInThisProject = false;

                    // Retrieve the stage's name, and find the stage
                    string sStageName = sStageInfo.Substring(1);
                    var stage = Find(sStageName);
                    if (null == stage)
                        throw new Exception("Bad data in Stage attribute.");

                    // The stage's position in the StagesList is its order in the
                    // string we are parsing.
                    stage.SortIndex = i;
                    stage.UsedInThisProject = bUsedInThisProject;
                }

                // Any stages that were missing from the string now need a sort index;
                // we'll not touch their UsedInThisProject, though, leaving them at their
                // factory default.
                int iSort = vs.Length;
                foreach (Stage stage in this)
                {
                    if (stage.SortIndex == -1)
                        stage.SortIndex = iSort++;
                }

                // Make the sort a reality
                Sort();
            }
            catch (Exception)
            {
                // Undo anything that might have been accomplished above
                InitializeToFactoryDefaults();
            }
        }
        #endregion
        #region Method: Stage FromOxesAttr(string sAttrValue)
        public Stage FromOxesAttr(string sAttrValue)
            // We need this in order to interpret old files, where we stored the ID rather
            // than the EnglishAbbrev.
        {
            if (string.IsNullOrEmpty(sAttrValue))
                return null;

            // See if we have a single digit; this would be old-style based on the ID
            if (sAttrValue.Length == 1 && char.IsDigit(sAttrValue[0]))
            {
                try
                {
                    int nID = Convert.ToInt16(sAttrValue);
                    return Find(nID);
                }
                catch(Exception) {}
            }

            // Otherwise, we have the EnglishAbbrev as a string
            return Find(sAttrValue);
        }
        #endregion
    }

}
