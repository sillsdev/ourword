#region ***** DProject.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DProject.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: The entire group of translations currently being viewed / edited.
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
using System.IO;
using JWTools;
using JWdb;
#endregion
#endregion

namespace JWdb.DataModel
{
    public class DProject : JObjectOnDemand
	{
		// ZAttrs ----------------------------------------------------------------------------
		const string c_sDefaultProjectName = "My Project";
		#region BAttr{g/s}: string Comment - a miscellaneous note about the project in general
		public string Comment
		{
			get
			{
				return m_sComment;
			}
			set
			{
                SetValue(ref m_sComment, value);
			}
		}
		private string m_sComment = "";
		#endregion
        #region BAttr{g/s}: bool VD_ShowTranslationsPane
        static public bool VD_ShowTranslationsPane
		{
			get
			{
				return m_bVD_ShowTranslationsPane;
			}
			set
			{
                m_bVD_ShowTranslationsPane = value;
			}
		}
        static private bool m_bVD_ShowTranslationsPane = false;
		#endregion
        #region BAttr{g/s}: bool ShowDictionaryPane
#if FEATURE_WESAY
        static public bool ShowDictionaryPane
        {
            get
            {
                return m_bShowDictionaryPane;
            }
            set
            {
                m_bShowDictionaryPane = value;
            }
        }
        static private bool m_bShowDictionaryPane = false;
#endif
        #endregion
        #region BAttr{g/s}: string PathToDictionaryApp - e.g., where to find WeSay
        public string PathToDictionaryApp
        {
            get
            {
                return m_sPathToDictionaryApp;
            }
            set
            {
                SetValue(ref m_sPathToDictionaryApp, value);
            }
        }
        private string m_sPathToDictionaryApp = "";
        #endregion
        #region BAttr{g/s}: string PathToDictionaryData - e.g., where to find WeSay
        public string PathToDictionaryData
        {
            get
            {
                return m_sPathToDictionaryData;
            }
            set
            {
                SetValue(ref m_sPathToDictionaryData, value);
            }
        }
        private string m_sPathToDictionaryData = "";
        #endregion
        #region BAttr{g/s}: List<string>PlannedBooks
        public List<string> PlannedBooks
        {
            get
            {
                var v = new List<string>();

                if (!string.IsNullOrEmpty(m_sPlannedBooks))
                {
                    var vs = m_sPlannedBooks.Split(' ');
                    foreach (string s in vs)
                        v.Add(s);
                }

                return v;
            }
            set
            {
                string sPlanned = "";
                foreach (string s in value)
                    sPlanned += s + " ";
                sPlanned = sPlanned.Trim();
                SetValue(ref m_sPlannedBooks, sPlanned);
            }
        }
        string m_sPlannedBooks = "";
        #endregion

        #region BAttr{g}:   BStringArray People
        public BStringArray People
        {
            get
            {
                return m_bsaPeople;
            }
        }
        public BStringArray m_bsaPeople = null;
        #endregion
		#region BAttr{g/s}: int Version
		public int Version
			// Version of the settings; used for upgrades
		{
			get
			{
				return m_nVersion;
			}
			set
			{
				m_nVersion = value;
			}
		}
		int m_nVersion = 0;
		#endregion

		#region Method void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();

			DefineAttr("Comment", ref m_sComment);
            DefineAttr("Planned", ref m_sPlannedBooks);

            DefineAttr("DictApp", ref m_sPathToDictionaryApp);
            DefineAttr("DictData", ref m_sPathToDictionaryData);

            DefineAttr("vdShowRelLangs", ref m_bVD_ShowTranslationsPane);

            DefineAttr("Persons", ref m_bsaPeople);
			DefineAttr("Version", ref m_nVersion);

#if FEAT_WESAY
            DefineAttr("ShowDictionary", ref m_bShowDictionaryPane);
#endif
        }
		#endregion

        // JAttrs: ---------------------------------------------------------------------------
        #region JAttr{g}: DTeamSettings TeamSettings
        public DTeamSettings TeamSettings
        {
            get
            {
                return j_oTeamSettings.Value;
            }
            set
            {
                j_oTeamSettings.Value = value;
            }
        }
        private JOwn<DTeamSettings> j_oTeamSettings = null;
        #endregion
		#region JAttr{g}: DTranslation FrontTranslation - e.g., the Kupang Translation
		public DTranslation FrontTranslation
		{
			get 
			{ 
				return m_FrontTranslation.Value; 
			}
			set
			{
				m_FrontTranslation.Value = value;
			}
		}
		private JOwn<DTranslation> m_FrontTranslation = null; 
		#endregion
		#region JAttr{g}: DTranslation TargetTranslation - e.g., the target Translation
		public DTranslation TargetTranslation
		{
			get 
			{ 
				return m_TargetTranslation.Value; 
			}
			set
			{
				m_TargetTranslation.Value = value;
			}
		}
		private JOwn<DTranslation> m_TargetTranslation = null; 
		#endregion
		#region JAttr{g}: JOwnSeq OtherTranslations - related and reference translations
		public JOwnSeq<DTranslation> OtherTranslations
		{
			get { return m_osOtherTranslations; }
		}
		private JOwnSeq<DTranslation> m_osOtherTranslations; 
		#endregion

		// Derived Attrs: Global settings ----------------------------------------------------
		#region VAttr{g}:   DStyleSheet StyleSheet
		public DStyleSheet StyleSheet
		{
			get 
			{ 
				Debug.Assert(null != TeamSettings);
				return TeamSettings.StyleSheet;
			}
		}
		#endregion
		#region VAttr{g}:   DSFMapping SFMapping - the mapping from SF to styles
		public DSFMapping SFMapping
		{
			get
			{
				if (null == TeamSettings)
					return null;
				return TeamSettings.SFMapping;
			}
		}
		#endregion

		// Derived Attrs: Current Section in the various translations ------------------------
		#region Attr{g}: DSection SFront - returns the current front section, or null
		public DSection SFront
		{
			get
			{
				return Nav.SFront;
			}
		}
		#endregion
		#region Attr{g}: DSection STarget - returns the current target section, or null
		public DSection STarget
		{
			get
			{
				return Nav.STarget;
			}
		}
		#endregion
		#region Attr{g}: int HasDataToDisplay - T if we have both a SFront and a STarget
		public bool HasDataToDisplay
		{
			get
			{
				if (null == FrontTranslation || null == SFront)
					return false;
				if (null == TargetTranslation || null == STarget)
					return false;
				return true;
			}
		}
		#endregion

        // Derived Attrs: Misc ---------------------------------------------------------------
        #region Attr{g}: bool ShowTranslationsPane - T if pane should be visible
        public bool ShowTranslationsPane
		{
			get
			{
                if (!m_bVD_ShowTranslationsPane)
					return false;
				if (OtherTranslations.Count == 0)
					return false;
				return true;
			}
		}
		#endregion
		#region Attr{g}: bool HasContent - if F, we don't bother saving the project
		public bool HasContent
		{
			get
			{
				if (c_sDefaultProjectName != DisplayName)
					return true;
				if (Comment.Length > 0)
					return true;
				if (null != FrontTranslation)
					return true;
				if (null != TargetTranslation)
					return true;
				if (OtherTranslations.Count != 0)
					return true;
				return false;
			}
		}
		#endregion
        #region Attr{g}: DTranslation[] AllTranslations
        public DTranslation[] AllTranslations
        {
            get
            {
                int c = 0;
                if (FrontTranslation != null)
                    c++;
                if (TargetTranslation != null)
                    c++;
                c += OtherTranslations.Count;

                DTranslation[] v = new DTranslation[c];

                c = 0;

                if (FrontTranslation != null)
                    v[c++] = FrontTranslation;
                if (TargetTranslation != null)
                    v[c++] = TargetTranslation;

                foreach (DTranslation t in OtherTranslations)
                    v[c++] = t;

                return v;
            }
        }
        #endregion

		// Dictionary ------------------------------------------------------------------------
		#region WESAY STUFF
		public Dictionary Dictionary
        {
            get
            {
                if (null == m_Dictionary)
                    m_Dictionary = new Dictionary(this);
                return m_Dictionary;
            }
        }
        Dictionary m_Dictionary = null;
		#endregion

		// Navigation ------------------------------------------------------------------------
		#region EMBEDDED CLASS: Navigation
		public class Navigation
		{
			// The Two Items we set to change Navigation -------------------------------------
			#region Attr{g}: string BookAbbrev
			public string BookAbbrev
			{
				get
				{
					return m_sBookAbbrev;
				}
			}
			string m_sBookAbbrev = "";
			#endregion
			#region Attr{g/s}: int SectionNo
			public int SectionNo
			{
				get
				{
					return m_iSectionNo;
				}
				set
				{
					if (value < 0 || value >= SectionCount)
						m_iSectionNo = -1;
					else
						m_iSectionNo = value;
				}
			}
			int  m_iSectionNo  = -1;
			#endregion
            #region Method: void GoToAvailableBook(sNewBook, IProgressIndicator)
            public void GoToAvailableBook(string sNewBook, IProgressIndicator progress)
            {
                // Is the proposed book one that we consider available? (Meaning it
                // exists in both Front and Target, and has been loaded?
                if (IsAvailableBook(sNewBook, progress))
                {
                    m_sBookAbbrev = sNewBook;
                    SectionNo = 0;
                    return;
                }

                // Otherwise, attempt to keep the book we already had. (We have to
                // test, because things may have changed.)
                if (IsAvailableBook(m_sBookAbbrev, progress))
                {
                    if (-1 == SectionNo)
                        SectionNo = 0;
                    return;
                }

                // Otherwise, go to the first available book we can find
                foreach (string sBookAbbrev in PotentialBookAbbrevs)
                {
                    if (IsAvailableBook(sBookAbbrev, progress))
                    {
                        m_sBookAbbrev = sBookAbbrev;
                        SectionNo = 0;
                        return;
                    }
                }

                // Otherwise, give up and leave it empty.
                m_sBookAbbrev = "";
                SectionNo = -1;
            }
            #endregion

            // Available Books ---------------------------------------------------------------
			#region Attr{g}: string[] PotentialBookAbbrevs - Books defined in both Front & Target
			string[] PotentialBookAbbrevs
			{
				get
				{
					if (null == TFront || null == TTarget)
						return new string[0];

					// We'll compile a list of books that exist in both target and front
					ArrayList list = new ArrayList();

					// Loop through all Target books; add those which are also in the Front
					foreach(DBook bt in TTarget.BookList)
					{
						foreach(DBook bf in TFront.BookList)
						{
							if (bt.BookAbbrev == bf.BookAbbrev)
							{
								list.Add(bt);
							}
						}
					}

					// Compile a string array of BookAbbrev's from this list of books
					string[] rg = new string[ list.Count ];
					for(int i=0; i<rg.Length; i++)
						rg[i] = (list[i] as DBook).BookAbbrev;

					return rg;
				}
			}
			#endregion
			#region Method: bool IsAvailableBook(string sBookAbbrev)
			bool IsAvailableBook(string sBookAbbrev, IProgressIndicator progress)
			{
				foreach(string s in PotentialBookAbbrevs)
				{
					if (s == sBookAbbrev)
					{
						// Retrieve the books. They will either come back as loaded, or as
						// null (in which case, GetLoadedBook will have removed them from
						// the translation.
                        DBook bFront = GetLoadedBook(TFront, sBookAbbrev, progress);
                        DBook bTarget = GetLoadedBook(TTarget, sBookAbbrev, progress);

						// Make sure we got loaded books
						if (null == bFront || null == bTarget)
							return false;

						// Both books have been found and sucessfully loaded.
						return true;
					}
				}
				return false;
			}
			#endregion
			#region Attr{g}: DBook[] PotentialTargetBooks - Books defined in both Front & Target
			public DBook[] PotentialTargetBooks
			{
				get
				{
					if (null == TFront || null == TTarget)
						return new DBook[0];

					// We'll compile a list of books that exist in both target and front
					ArrayList list = new ArrayList();

					// Loop through all Target books; add those which are also in the Front
					foreach(DBook bt in TTarget.BookList)
					{
						foreach(DBook bf in TFront.BookList)
						{
							if (bt.BookAbbrev == bf.BookAbbrev)
							{
								list.Add(bt);
							}
						}
					}

					// Compile a string array of BookAbbrev's from this list of books
					DBook[] rg = new DBook[ list.Count ];
					for(int i=0; i<rg.Length; i++)
						rg[i] = (list[i] as DBook);

					return rg;
				}
			}
			#endregion

			// Access to other translations -------------------------------------------------
			#region Method: DBook GetLoadedBook(...) - gets the book, loading it if necessary
			public DBook GetLoadedBook(DTranslation translation, 
                string sBookAbbrev, IProgressIndicator progress)
			{
				if (null == translation)
					return null;

				DBook book = translation.FindBook(sBookAbbrev);
				if (null == book)
					return null;

				// Attempt to load the book
                book.LoadBook(progress);

				// If we were unable to load the book, then don't return it; the caller will
                // then know to find a different book.
				if (false == book.Loaded)
					return null;

				return book;
			}
			#endregion
			#region Method: DSection GetSection(DTranslation)
			public DSection GetSection(DTranslation t)
			{
				DBook b = GetLoadedBook(t, BookAbbrev, new NullProgress());
				if (null == b)
					return null;
				if (b.Sections.Count != BFront.Sections.Count)
					return null;
				return b.Sections[SectionNo] as DSection;
			}
			#endregion

			// GoTo --------------------------------------------------------------------------
			#region Attr{g}: int SectionCount
			public int SectionCount
			{
				get
				{
					if (null == BFront)
						return 0;
					return BFront.Sections.Count;
				}
			}
			#endregion
			#region Method: void GoToFirstSection()
			public void GoToFirstSection()
			{
				// If a filter is active, then the first section is defined as the first one
				// which matches the current filter criteria.
				if (DSection.FilterIsActive)
				{
					for(int i=0; i<SectionCount; i++)
					{
						if ((BTarget.Sections[i] as DSection).MatchesFilter)
						{
							SectionNo = i;
							return;
						}
					}
					Debug.Assert(false, "No sections match the filter.");
				}

				// Otherwise we don't have filters, so we simply return the first section.
				SectionNo = 0;
			}
			#endregion
			#region Method: void GoToLastSection()
			public void GoToLastSection()
			{
				// If a filter is active, then the Last section is defined as the final one
				// which matches the current filter criteria.
				if (DSection.FilterIsActive)
				{
					for(int i = SectionCount - 1; i>=0; i--)
					{
						DSection section = DB.TargetBook.Sections[i] as DSection;
						if (section.MatchesFilter)
						{
							SectionNo = i;
							return;
						}
					}
					Debug.Assert(false, "No sections match the filter.");
				}

				// Otherwise we don't have filters, so we simply return the last section.
				SectionNo = SectionCount - 1;
			}
			#endregion
			#region Method: void GoToNextSection()
			public void GoToNextSection()
			{
				// If a filter is active, then we look for the next section following this
				// one which matches. If there is no match, then we'll leave the position
				// where it currently is.
				if (DSection.FilterIsActive)
				{
					for(int i = SectionNo + 1; i<SectionCount; i++)
					{
						if ((BTarget.Sections[i] as DSection).MatchesFilter)
						{
							SectionNo = i;
							return;
						}
					}
					return;
				}
				// Otherwise there are no filters, just go to the next section, if any.
				if (SectionNo < SectionCount - 1)
					SectionNo++;
			}
			#endregion
			#region Method: void GoToPreviousSection()
			public void GoToPreviousSection()
			{
				// If a filter is active, then we look for the section preceeding this
				// one which matches. If there is no match, then we'll leave the position
				// where it currently is.
				if (DSection.FilterIsActive)
				{
					for(int i=SectionNo - 1; i>=0; i--)
					{
						if ((BTarget.Sections[i] as DSection).MatchesFilter)
						{
							SectionNo = i;
							return;
						}
					}
					return;
				}

				// Otherwise there are no filters, just go to the previous section, if any.
				if (SectionNo > 0)
					SectionNo--;
			}
			#endregion
			#region Method: void GoToSection(int iSection)
			public void GoToSection(int iSection)
			{
				if (iSection >= 0 && iSection < SectionCount)
					SectionNo = iSection;
			}
			#endregion
            #region Method: void GoToBook(sBookAbbrev, IProgressIndicator) - move the position to another book
            public void GoToBook(string sBookAbbrev, IProgressIndicator progress)
			{
                // Do nothing if we're already there
                if (sBookAbbrev == BookAbbrev)
                    return;

				// Get the target book that has this abbreviation
				foreach( DBook b in TTarget.BookList)
				{
                    if (b.BookAbbrev == sBookAbbrev)
					{
                        BTarget.RemoveFilter();
                        GoToAvailableBook(sBookAbbrev, progress);
						return;
					}
				}
			}
			#endregion
			#region Method: void GoToFirstAvailableBook()
			public void GoToFirstAvailableBook(IProgressIndicator progress)
			{
				// The BookAbbrev mechanism will find a reasonable alternative if the
				// first book in this array does not qualify for some reason.
				if (PotentialBookAbbrevs.Length > 0)
				{
					GoToAvailableBook( PotentialBookAbbrevs[0], progress);
				}
				else
					m_sBookAbbrev = "";
			}
			#endregion
            #region Method: void GoToReasonableBook(IProgressIndicator) - If we aren't at a book, try to find one
            public void GoToReasonableBook(IProgressIndicator progress)
			{
                if (BookAbbrev.Length > 0 && IsAvailableBook(BookAbbrev, progress))
				{
					if (-1 == SectionNo)
						SectionNo = 0;
					if (-1 != SectionNo)
						return;
				}
                GoToFirstAvailableBook(progress);
			}
			#endregion

            #region Method: bool IsAtLastSection
            public bool IsAtLastSection
            {
                get
                {
                    if (-1 == SectionNo)
                        return true;
                    if (DSection.FilterIsActive && BTarget.IndexOfLastFilterMatch == SectionNo)
                        return true;
                    if (BTarget.Sections.Count - 1 == SectionNo)
                        return true;
                    return false;
                }
            }
            #endregion
            #region Method: bool IsAtFirstSection
            public bool IsAtFirstSection
            {
                get
                {
                    if (-1 == SectionNo)
                        return true;
                    if (DSection.FilterIsActive && BTarget.IndexOfFirstFilterMatch == SectionNo)
                        return true;
                    if (0 == SectionNo)
                        return true;
                    return false;
                }
            }
            #endregion

            // Current Translations, Books, and Sections -------------------------------------
			#region Attr{g}: DTranslation TFront
			public DTranslation TFront
			{
				get
				{
					if (null != m_project)
						return m_project.FrontTranslation;
					return null;
				}
			}
			#endregion
			#region Attr{g}: DTranslation TTarget
			public DTranslation TTarget
			{
				get
				{
					if (null != m_project)
						return m_project.TargetTranslation;
					return null;
				}
			}
			#endregion
			#region Attr{g}: DBook BFront
			public DBook BFront
			{
				get
				{
					if (null != TFront)
						return TFront.FindBook(BookAbbrev);
					return null;
				}
			}
			#endregion
			#region Attr{g}: DBook BTarget
			public DBook BTarget
			{
				get
				{
					if (null != TTarget)
						return TTarget.FindBook(BookAbbrev);
					return null;
				}
			}
			#endregion
			#region Attr{g}: DSection SFront
			public DSection SFront
			{
				get
				{
					if (null != BFront)
						return BFront.Sections[SectionNo] as DSection;
					return null;
				}
			}
			#endregion
			#region Attr{g}: DSection STarget
			public DSection STarget
			{
				get
				{
					if (null != BTarget)
						return BTarget.Sections[SectionNo] as DSection;
					return null;
				}
			}
			#endregion

			// Registry ----------------------------------------------------------------------
			const string c_SubKey  = "Position";
			const string c_Book    = "Book";
			const string c_Section = "Section";
			#region Method: void SavePositionToRegistry() - for restoring on program load
			public void SavePositionToRegistry()
			{
				JW_Registry.SetValue(c_SubKey, c_Book,    BookAbbrev);
				JW_Registry.SetValue(c_SubKey, c_Section, SectionNo);
			}
			#endregion
            #region Method: void RetrievePositionFromRegistry(IProgressIndicator)
            public void RetrievePositionFromRegistry(IProgressIndicator progress)
			{
                GoToAvailableBook(JW_Registry.GetValue(c_SubKey, c_Book, BookAbbrev), progress);
				SectionNo  = JW_Registry.GetValue(c_SubKey, c_Section, SectionNo);
			}
			#endregion

			// Scaffolding -------------------------------------------------------------------
			DProject m_project = null;
			#region Constructor(DProject)
			public Navigation(DProject project)
			{
				m_project = project;
			}
			#endregion
		};
		#endregion
		#region Attr{g}: Navigation Nav
		public Navigation Nav
		{
			get
			{
				Debug.Assert(null != m_Nav);
				return m_Nav;
			}
		}
		Navigation m_Nav = null;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DProject()
			: base()
		{
			m_Nav = new Navigation(this);

            // Team Settings
            j_oTeamSettings = new JOwn<DTeamSettings>("Team", this);
            j_oTeamSettings.Value = new DTeamSettings();
            TeamSettings.New();

			// Owning Attrs
            m_FrontTranslation = new JOwn<DTranslation>("Front", this);
			m_TargetTranslation = new JOwn<DTranslation>("Target", this);

			// Other Translations
			m_osOtherTranslations = new JOwnSeq<DTranslation>("OtherTrans", this,
                true, false);

            m_bsaPeople = new BStringArray();

            // Default for a display name
            DisplayName = c_sDefaultProjectName;
		}
		#endregion
        #region Constructor(sDisplayName)
        public DProject(string sDisplayName)
            : this()
        {
            DisplayName = sDisplayName;
        }
        #endregion
        #region Destructor()
        ~DProject()
        {
            Dispose();
        }
        #endregion
        #region Method: void Dispose()
        public void Dispose()
        {
            if (null != m_Dictionary)
                m_Dictionary.Dispose();
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void CheckTeamSettingsName(DTeamSettings, sPath)
        void CheckTeamSettingsName(DTeamSettings ts, string sPath)
             // Does the name equal what's in the path? (or has the folder been moved?)
       {
            string[] vItemsInPath = sPath.Split(new char[] { Path.DirectorySeparatorChar });

            int c = vItemsInPath.Length;

            if (c > 3)
            {
                string sClusterName = vItemsInPath[c - 3];
                //Console.WriteLine("Cluster = " + sClusterName);
                //Console.WriteLine("Path    = " + sPath);
                //Console.WriteLine("Name    = " + DisplayName);
                //Console.WriteLine("---------");

                ts.DisplayName = sClusterName;
            }
       }
        #endregion
        #region OMethod: bool OnLoad(TextReader, IProgressIndicator)
        protected override bool OnLoad(TextReader tr, string sPath, IProgressIndicator progress)
        {
            // Read this DProject as per normal
			if (!base.OnLoad(tr, sPath, progress))
                return false;

            // Initialize the Team Settings file
            CheckTeamSettingsName(TeamSettings, sPath);
			if (File.Exists(TeamSettings.StoragePath))
                TeamSettings.LoadFromFile(progress);
			else
                TeamSettings.New();
			TeamSettings.TemporaryFixes();
            TeamSettings.EnsureInitialized();

            // Read the LoadOnDemand translation objects
            if (null != FrontTranslation)
            {
                FrontTranslation.LoadFromFile(progress);
                if (!FrontTranslation.Loaded)
                    FrontTranslation = null;
            }

            if (null != TargetTranslation)
            {
                TargetTranslation.LoadFromFile(progress);
                if (!TargetTranslation.Loaded)
                    TargetTranslation = null;
            }

            // Load the other translations; remove them from the list if unsuccessful
            for (int i = 0; i < OtherTranslations.Count; )
            {
                DTranslation t = OtherTranslations[i] as DTranslation;

                t.LoadFromFile(progress);

                if (!t.Loaded)
                    OtherTranslations.Remove(t);
                else
                    i++;
            }

            IsDirty = false;
            return true;
        }
        #endregion
		#region Method: DTranslation FindTranslation(string sDisplayName)
		public DTranslation FindTranslation(string sDisplayName)
		{
			if (null != FrontTranslation && FrontTranslation.DisplayName == sDisplayName)
				return FrontTranslation;

			if (null != TargetTranslation && TargetTranslation.DisplayName == sDisplayName)
				return TargetTranslation;

            foreach (DTranslation t in OtherTranslations)
			{
				if (t.DisplayName == sDisplayName)
					return t;
			}
			return null;
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
        #region SAttr{g}: string FileExtension
        static public string FileExtension
        {
            get
            {
                return ".owp";
            }
        }
        #endregion
        #region VAttr{g}: override string DefaultFileExtension
        public override string DefaultFileExtension
        {
            get
            {
                return FileExtension;
            }
        }
        #endregion
		#region OAttr{g}: string StoragePath
		public override string StoragePath
			// We'll store the Project settings (owp) file in the Cluster's settings folder
		{
			get
			{
				return TeamSettings.SettingsFolder + StorageFileName;
			}
		}
		#endregion
        #region Method: void Save(IProgressIndicator)
        public void Save(IProgressIndicator progress)
        {
            // Nothing to do if we don't have anything to save
            if (!HasContent)
                return;

            // Version 1.x projects did not have a DisplayName in the TeamSettings;
            // we don't save those, because they are obsolete remnants
            if (string.IsNullOrEmpty(TeamSettings.DisplayName))
                return;

            // The JObjectOnDemand does the writing
            WriteToFile(progress);
        }
        #endregion
	}

}
