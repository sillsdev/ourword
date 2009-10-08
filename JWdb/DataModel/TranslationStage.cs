/**********************************************************************************************
 * Project: Our Word!
 * File:    TranslationStage.cs
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
using JWdb;
#endregion

namespace JWdb.DataModel
{
    public class TranslationStage : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: int ID - an ID number for referring to the stage in disk storage
        public int ID
        {
            get
            {
                return m_nID;
            }
            set
            {
                m_nID = value;
            }
        }
        private int m_nID = 0;
        #endregion
        #region BAttr{g/s}: string Abbrev - A short name for the stage
        public string Abbrev
        {
            get
            {
                return m_sAbbrev;
            }
            set
            {
                m_sAbbrev = value;
            }
        }
        private string m_sAbbrev = "";
        #endregion
        #region BAttr{g/s}: string Name - The full name for the stage
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
        private string m_sName = "";
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("ID", ref m_nID);
            DefineAttr("Abbrev", ref m_sAbbrev);
            DefineAttr("Name", ref m_sName);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor() - Used only by the Read operation
        public TranslationStage()
            : base()
        {
        }
        #endregion
        #region Constructor( nID, sAbbrev, sName)
        public TranslationStage(int _nID, string _sAbbrev, string _sName)
            : base()
        {
            ID = _nID;
            Abbrev = _sAbbrev;
            Name = _sName;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            TranslationStage stage = (TranslationStage)obj;

            if (stage.Abbrev != this.Abbrev)
                return false;
            if (stage.Name != this.Name)
                return false;
            return true;
        }
        #endregion
        #region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
        public override string SortKey
        // In order to support sorting, the subclass must implement a SortKey attribute,
        // and this SortKey must return something other than an empty string. 
        {
            get
            {
                return Abbrev;
            }
        }
        #endregion
    }

    public class BookStages : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq TranslationStages - The translation stages defined for this Team
        public JOwnSeq<TranslationStage> TranslationStages
        {
            get
            {
                return m_osTranslationStages;
            }
        }
        private JOwnSeq<TranslationStage> m_osTranslationStages;
        #endregion
        #region BAttr{g/s}: int NextID - The next available ID for creating a new TranslationStage
        private int NextID
        {
            get
            {
                return m_nNextID;
            }
            set
            {
                m_nNextID = value;
            }
        }
        // Start with a fairly large number, so we can easily add cannonical ones
        // in the future without conflicting with the user-added ones.
        private int m_nNextID = 1000;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("NextID", ref m_nNextID);
        }
        #endregion

        // Virtual Attrs ---------------------------------------------------------------------
        #region Attr{g}: LocLanguage FileNameLanguage
        public LocLanguage FileNameLanguage
        {
            get
            {
                return LocDB.DB.FindLanguageByName(DB.TeamSettings.FileNameLanguage);
            }
        }
        #endregion
        #region Attr{g}: TranslationStage GetFirstStage
        public TranslationStage GetFirstStage
        {
            get
            {
                Debug.Assert(TranslationStages.Count > 0);
                Debug.Assert(TranslationStages[0] as TranslationStage != null);

                return TranslationStages[0] as TranslationStage;
            }
        }
        #endregion
        #region Attr{g}: int Count - the number of TranslationStages in the ownseq
        public int Count
        {
            get
            {
                return TranslationStages.Count;
            }
        }
        #endregion
        #region VAttr{g}: string[] AllNames
        public string[] AllNames
        {
            get
            {
                string[] v = new string[Count];
                for (int i = 0; i < Count; i++)
                {
                    v[i] = TranslationStages[i].Name;
                }
                return v;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public BookStages()
            : base()
        {
            // Initialize the owning sequence object
            m_osTranslationStages = new JOwnSeq<TranslationStage>("Stages", this,
                false, false);

            // Initialize the default Stages values; a Read of the TeamSettings will override.
            SetToFactoryDefault();
        }
        #endregion
        #region Indexer[] - Adds ownership support to the base indexer
        public TranslationStage this[int index]
        {
            get
            {
                return TranslationStages[index] as TranslationStage;
            }
            set
            {
                TranslationStages[index] = value;
            }
        }
        #endregion

        // IDs for Factory Defaults ----------------------------------------------------------
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
        #endregion
        #region Embedded Class IDGroup
        protected class IDGroup
        {
            #region Attr{g}: int ID - the group id (NOT the LocDB ID!)
            public int ID
            {
                get
                {
                    return m_nID;
                }
            }
            int m_nID;
            #endregion

            string m_sLocID;
            string m_sEnglishAbbrevDefault;
            string m_sEnglishDefault;

            #region Constructor(nID, sLocID, sEnglishAbbrevDefault, sEnglishDefault)
            public IDGroup(int nID, string sLocID, string sEnglishAbbrevDefault, string sEnglishDefault)
            {
                m_nID = nID;
                m_sLocID = sLocID;
                m_sEnglishAbbrevDefault = sEnglishAbbrevDefault;
                m_sEnglishDefault = sEnglishDefault;
            }
            #endregion

            #region Attr{g}: string Abbrev - returns the localized Abbreviation
            public string Abbrev
            {
                get
                {
                    return Loc.GetTranslationStage("abbrev" + m_sLocID, m_sEnglishAbbrevDefault);
                }
            }
            #endregion
            #region Attr{g}: string Name = returns the localized Name
            public string Name
            {
                get
                {
                    return Loc.GetTranslationStage(m_sLocID, m_sEnglishDefault);
                }
            }
            #endregion
        };
        #endregion
        #region IDGroup[] s_IDGroups - Mappings from IDs to localizations
        static IDGroup[] s_IDGroups =
		{
			new IDGroup(c_idDraft,            "Draft",            "Draft", "Draft"),
			new IDGroup(c_idTeamCheck,        "TeamCheck",        "Team",  "Team Check"),
			new IDGroup(c_idAdvisorCheck,     "AdvisorCheck",     "Adv",   "Advisor Check"),
			new IDGroup(c_idCommunityCheck,   "CommunityCheck",   "Comm",  "Community Check"),
			new IDGroup(c_idBackTranslation,  "BackTranslation",  "BT",    "Back Translation"),
			new IDGroup(c_idConsultantCheck,  "ConsultantCheck",  "Consult", "Consultant Check"),
			new IDGroup(c_idFinalForPrinting, "FinalForPrinting", "Final",  "Final For Printing"),
			new IDGroup(c_idFinalRevision,    "FinalRevisions",   "Rev",    "Final Revisions")
		};
        #endregion
        #region Method: void SetToFactoryDefault()
        public void SetToFactoryDefault()
        {
            // Zero out anything that was previously there
            TranslationStages.Clear();

            foreach (IDGroup group in s_IDGroups)
            {
                TranslationStage stage = new TranslationStage(
                    group.ID, group.Abbrev, group.Name);

                TranslationStages.Append(stage);
            }
        }
        #endregion
        #region Method: void UpdateFactoryLanguage()
        public void UpdateFactoryLanguage()
        // Called potentially after the TeamSettings.FileNameLanguage has been
        // changed, this method updates the values in each TranslationStage
        // to the new language, as stored in the LocDB.
        {
            foreach (IDGroup group in s_IDGroups)
            {
                // Get the stage for this ID (test for it, as the user may have
                // deleted it.)
                TranslationStage stage = GetFromID(group.ID);
                if (null == stage)
                    continue;

                // Update its values to whatever is in the resources
                stage.Abbrev = group.Abbrev;
                stage.Name = group.Name;
            }
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: TranslationStage GetFromID(nID)
        public TranslationStage GetFromID(int nID)
        {
            foreach (TranslationStage stage in TranslationStages)
            {
                if (stage.ID == nID)
                    return stage;
            }
            return null;
        }
        #endregion
        #region Method: TranslationStage GetFromIndex(i)
        public TranslationStage GetFromIndex(int i)
        {
            Debug.Assert(i >= 0 && i < TranslationStages.Count);
            TranslationStage stage = TranslationStages[i] as TranslationStage;
            Debug.Assert(null != stage);
            return stage;
        }
        #endregion
        #region Method: TranslationStage GetFromName(sName)
        public TranslationStage GetFromName(string sName)
        {
            foreach (TranslationStage stage in TranslationStages)
            {
                if (stage.Name == sName)
                    return stage;
            }
            return null;
        }
        #endregion
        #region Method: TranslationStage GetFromAbbrev(sAbbrev)
        public TranslationStage GetFromAbbrev(string sAbbrev)
        {
            foreach (TranslationStage stage in TranslationStages)
            {
                if (stage.Abbrev == sAbbrev)
                    return stage;
            }
            return null;
        }
        #endregion
        #region Method: int GetIndexOf(TranslationStage stage)
        public int GetIndexOf(TranslationStage stage)
        {
            return TranslationStages.FindObj(stage);
        }
        #endregion
        #region Method: void PopulateCombo(ComboBox combo)
        public void PopulateCombo(ComboBox combo)
        {
            combo.Items.Clear();

            foreach (TranslationStage stage in TranslationStages)
            {
                combo.Items.Add(stage.Abbrev);
            }
        }
        #endregion
        #region Method: int GetAndIncrementNextID()
        public int GetAndIncrementNextID()
        {
            int n = NextID;
            NextID++;
            return n;
        }
        #endregion
        #region Method: bool Append( TranslationStage stage )
        public bool Append(TranslationStage stage)
        {
            // Make sure there isn't already an identical one already there; abort if so
            // as we don't want to have duplicates.
            if (GetFromAbbrev(stage.Abbrev) != null)
                return false;
            if (GetFromName(stage.Name) != null)
                return false;

            // Add the new one to the list
            TranslationStages.Append(stage);
            return true;
        }
        #endregion
        #region Method: void Remove( TranslationStage stage )
        public void Remove(TranslationStage stage)
        {
            TranslationStages.Remove(stage);
        }
        #endregion
        #region Method: void MoveTo( int iStage, int iNewPos)
        public void MoveTo(int iStage, int iNewPos)
        {
            Debug.Assert(iStage >= 0 && iStage < TranslationStages.Count);
            Debug.Assert(iNewPos >= 0 && iNewPos < TranslationStages.Count);
            TranslationStages.MoveTo(iStage, iNewPos);
        }
        #endregion
    }

}
