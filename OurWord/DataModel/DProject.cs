/**********************************************************************************************
 * Project: Our Word!
 * File:    DProject.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: The entire group of translations currently being viewed / edited.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using JWTools;
using JWdb;
using OurWord.Dialogs;
#endregion

namespace OurWord.DataModel
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
		#region BAttr{g/s}: bool VD_ShowNotesPane - View:Drafting, F is pane is hidden
		static public bool VD_ShowNotesPane
		{
			get
			{
				return m_bVD_ShowNotesPane;
			}
			set
			{
				m_bVD_ShowNotesPane = value;
			}
		}
		static private bool m_bVD_ShowNotesPane = false;
		#endregion
        #region BAttr{g/s}: bool VD_ShowTranslationsPane - View:Drafting, F is pane is hidden
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
		#region Method void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();

			DefineAttr("Comment",        ref m_sComment);

			DefineAttr("vdShowNotes",    ref m_bVD_ShowNotesPane);
            DefineAttr("vdShowRelLangs", ref m_bVD_ShowTranslationsPane);
		}
		#endregion

        // JAttrs: ---------------------------------------------------------------------------
        #region JAttr{g}: DTeamSettings TeamSettings
        public DTeamSettings TeamSettings
        {
            get
            {
                return (DTeamSettings)j_oTeamSettings.Value;
            }
            set
            {
                j_oTeamSettings.Value = value;
            }
        }
        private JOwn j_oTeamSettings = null;
        #endregion
		#region JAttr{g}: DTranslation FrontTranslation - e.g., the Kupang Translation
		public DTranslation FrontTranslation
		{
			get 
			{ 
				return (DTranslation)m_FrontTranslation.Value; 
			}
			set
			{
				m_FrontTranslation.Value = value;
			}
		}
		private JOwn m_FrontTranslation = null; 
		#endregion
		#region JAttr{g}: DTranslation TargetTranslation - e.g., the target Translation
		public DTranslation TargetTranslation
		{
			get 
			{ 
				return (DTranslation)m_TargetTranslation.Value; 
			}
			set
			{
				m_TargetTranslation.Value = value;
			}
		}
		private JOwn m_TargetTranslation = null; 
		#endregion
		#region JAttr{g}: JOwnSeq OtherTranslations - related and reference translations
		public JOwnSeq OtherTranslations
		{
			get { return m_osOtherTranslations; }
		}
		private JOwnSeq m_osOtherTranslations; 
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

        // Navigation ------------------------------------------------------------------------
		#region EMBEDDED CLASS: Navigation
		public class Navigation
		{
			// The Two Items we set to change Navigation -------------------------------------
			#region Attr{g/s}: string BookAbbrev
			public string BookAbbrev
			{
				get
				{
					return m_sBookAbbrev;
				}
				set
				{
					// Is the proposed book one that we consider available? (Meaning it
					// exists in both Front and Target, and has been loaded?
					if (IsAvailableBook(value))
					{
						m_sBookAbbrev = value;
						SectionNo = 0;
						return;
					}

					// Otherwise, attempt to keep the book we already had. (We have to
					// test, because things may have changed.)
					if (IsAvailableBook(m_sBookAbbrev))
					{
						if (-1 == SectionNo)
							SectionNo = 0;
						return;
					}

					// Otherwise, go to the first available book we can find
					foreach(string sBookAbbrev in PotentialBookAbbrevs)
					{
						if (IsAvailableBook(sBookAbbrev))
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
					foreach(DBook bt in TTarget.Books)
					{
						foreach(DBook bf in TFront.Books)
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
			bool IsAvailableBook(string sBookAbbrev)
			{
				foreach(string s in PotentialBookAbbrevs)
				{
					if (s == sBookAbbrev)
					{
						// Retrieve the books. They will either come back as loaded, or as
						// null (in which case, GetLoadedBook will have removed them from
						// the translation.
						DBook bFront  = GetLoadedBook(TFront,  sBookAbbrev);
						DBook bTarget = GetLoadedBook(TTarget, sBookAbbrev);

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
					foreach(DBook bt in TTarget.Books)
					{
						foreach(DBook bf in TFront.Books)
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
			#region Method: DBook GetLoadedBook(DTranslation) - gets the book, loading it if necessary
			public DBook GetLoadedBook(DTranslation translation, string sBookAbbrev)
			{
				if (null == translation)
					return null;

				DBook book = translation.FindBook(sBookAbbrev);
				if (null == book)
					return null;

				// Attempt to load the book
                book.Load();

				// If we were unable to load the book, then we must remove it from the 
				// translation, because we are unable to make use of it. The user will 
				// have to go to Properties and get it back in correctly.
				if (false == book.Loaded)
				{
					translation.Books.Remove(book);
					return null;
				}

				return book;
			}
			#endregion
			#region Method: DSection GetSection(DTranslation)
			public DSection GetSection(DTranslation t)
			{
				DBook b = GetLoadedBook(t, BookAbbrev);
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
						DSection section = G.TBook.Sections[i] as DSection;
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
			#region Method: void GoToBook(sBookDisplayName) - move the position to another book
			public void GoToBook(string sBookDisplayName)
			{
				// Get the target book that has this display name
				DBook book = null;
				foreach( DBook b in TTarget.Books)
				{
					if (b.DisplayName == sBookDisplayName)
					{
						book = b;
						break;
					}
				}
				if (null == book)
					return;

				// Get the book's abbreviation; do nothing if we are already there
				string sBookAbbrev = book.BookAbbrev;
				if (sBookAbbrev == BookAbbrev)
					return;

				// Turn off any filter before going to a different book
				BTarget.RemoveFilter();

				// Go there (this also adjusts the SectionNo to 0
				BookAbbrev = sBookAbbrev;
			}
			#endregion
			#region Method: void GoToFirstAvailableBook()
			public void GoToFirstAvailableBook()
			{
				// The BookAbbrev mechanism will find a reasonable alternative if the
				// first book in this array does not qualify for some reason.
				if (PotentialBookAbbrevs.Length > 0)
				{
					BookAbbrev = PotentialBookAbbrevs[0];
				}
				else
					m_sBookAbbrev = "";
			}
			#endregion
			#region Method: void GoToReasonableBook() - If we aren't at a book, try to find one
			public void GoToReasonableBook()
			{
				if (BookAbbrev.Length > 0 && IsAvailableBook(BookAbbrev))
				{
					if (-1 == SectionNo)
						SectionNo = 0;
					if (-1 != SectionNo)
						return;
				}
				GoToFirstAvailableBook();
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
			#region Method: void RetrievePositionFromRegistry()
			public void RetrievePositionFromRegistry()
			{
				BookAbbrev = JW_Registry.GetValue(c_SubKey, c_Book,    BookAbbrev);
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
		#region Constructor(DTeamSettings)
		public DProject()
			: base()
		{
			m_Nav = new Navigation(this);

			_ConstructAttrs();

            // Default for a display name
            DisplayName = c_sDefaultProjectName;
		}
		#endregion
		#region Method: void _ConstructAttrs()
		private void _ConstructAttrs()
		{
            // Team Settings
            j_oTeamSettings = new JOwn("Team", this, typeof(DTeamSettings));
            j_oTeamSettings.Value = new DTeamSettings();
            TeamSettings.New();

			// Owning Attrs
            m_FrontTranslation = new JOwn("Front", this, typeof(DTranslation));
			m_TargetTranslation = new JOwn("Target", this, typeof(DTranslation));

			// Other Translations
			m_osOtherTranslations = new JOwnSeq("OtherTrans", this, typeof(DTranslation),
                false, false);
		}
		#endregion
        #region VAttr{g}: override string DefaultFileExtension
        public override string DefaultFileExtension
        {
            get
            {
                return ".owp";
            }
        }
        #endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void Write(tw, nIndent) - writes the project, and writes its translations
		public override void Write(TextWriter tw, int nIndent)
		{
			// Write this DProject as per normal
			base.Write(tw, nIndent);

			// Write the LoadOnDemand translation objects
			if (null != FrontTranslation)
				FrontTranslation.Write();

			if (null != TargetTranslation)
				TargetTranslation.Write();

			foreach (DTranslation t in OtherTranslations)
				t.Write();
		}
		#endregion

		#region Method: void Read(sLine, tr) - reads the proj, and reads its translations
		public override void Read(string sLine, TextReader tr)
		{
			// Read this DProject as per normal
            string sAbsolutePathName = this.AbsolutePathName;
			base.Read(sLine, tr);
            this.AbsolutePathName = sAbsolutePathName;

            // Initialize the Team Settings file
            if (string.IsNullOrEmpty(TeamSettings.AbsolutePathName))
                TeamSettings.New();
            else
                TeamSettings.Load();

			// Read the LoadOnDemand translation objects
			if (null != FrontTranslation)
			{
                FrontTranslation.Load();
                if (!FrontTranslation.Loaded)
					FrontTranslation = null;
			}

			if (null != TargetTranslation)
			{
                TargetTranslation.Load();
                if (!TargetTranslation.Loaded)
					TargetTranslation = null;
			}

            // Load the other translations; remove them from the list if unsuccessful
            for (int i = 0; i < OtherTranslations.Count; )
            {
                DTranslation t = OtherTranslations[i] as DTranslation;

                t.Load();

                if (!t.Loaded)
                    OtherTranslations.Remove(t);
                else
                    i++;
            }
		}
		#endregion
		#region Method: void SaveCurrentBook()
		public void SaveCurrentBook()
		{
			if (null == STarget)
				return;

			DBook book = STarget.Book;
			if (null == book)
				return;

			// Save the file
            if (!book.Loaded)
                book.Write();
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
	}

	#region TEST
	public class Test_DProject : Test
	{
		// Test Data -------------------------------------------------------------------------
		#region DATA: string[] m_RawKupangMark
		static public string[] m_RawKupangMark = new string[] 
		{
			"\\_sh v3.0  540  SHW-Scripture",
			"\\_DateStampHasFourDigitYear",
			"",
			"\\rcrd mrk",
			"\\h Markus",
			"\\st Tuhan Yesus pung Carita Bae,",
			"\\btst Lord Yesus' *Gospel1 (Good Story)",
			"\\st iko",
			"\\btst according to",
			"\\mt Markus",
			"\\btmt Markus",
			"",
			"\\rcrd mrk16.12-16.13",
			"\\s Tuhan Yesus kasi tunju muka sang dua orang lai",
			"\\bts The Lord Yesus reveals himself to two more people",
			"\\r (Lukas 24:13-35)",
			"\\p",
			"\\v 12",
			"\\vt Abis itu, Yesus pung ana bua dua orang bajalan pi satu kampong. Di " +
				"tenga jalan ju, Yesus pi kasi tunju muka sang dong, ma Dia su jadi " +
				"laen. Ma sadiki lai, dong ju kanál sang Dia.",
			"\\btvt After that, two of Yesus' disciples walked to a village. On the " +
				"road then, Yesus went to reveal himself to them, and He was different. " +
				"But after a little (while), they recognised Him.",
			"\\nt rupa = form, model (KM)",
			"\\v 13",
			"\\vt Ais dong dua pulang pi kasi tau dong pung tamán, bilang, <<We! Bosong " +
				"dengar dolo! Tadi ini, baru botong katumu deng Yesus di jalan!>>",
			"\\btvt They they two went.home to tell their friends saying, <<Hey! You-pl " +
				"listen! Just now, we-e just met with Yesus on the road!>>",
			"\\p",
			"\\vt Ma dong pung tamán dong samua manyao, bilang, <<We! Bosong jang omong " +
				"kosong bagitu!>>",
			"\\btvt But their friends all answered saying, <<Hey! You-pl don't lie like." +
				"that!>>",
			"\\ud 20/Dec/2004",
			"",
			"\\rcrd mrk16.14-16.18",
			"\\s Tuhan Yesus kasi tunju muka sang ana bua dong samua",
			"\\bts The Lord Yesus reveals himself to all his disciples",
			"\\r (Mateos 28:16-20; Lukas 24:36-49; Yohanis 20:19-23; Utusan dong pung " +
				"Carita 1:6-8)",
			"\\p",
			"\\v 14",
			"\\vt Abis ju, Yesus katumu deng Dia pung ana bua sablas orang, waktu dong " +
				"ada dudu makan. Dia togor sang dong, bilang, <<We! Bosong pung kapala " +
				"batu lai! Orang su kasi tau sang bosong bilang, dong su lia sang Beta " +
				"deng dong pung mata biji sandiri, ma bosong sonde mau parcaya, ó! " +
				"Bosong bilang, dong samua omong kosong! Naa! Sakarang ini, Beta sandiri " +
				"su datang. Beta memang su mati, ma sakarang ini, Beta ini su idop kambali! " +
				"Naa, bosong lia sandiri sa.",
			"\\btvt And then, Yesus met with his eleven disciples when they were eating. " +
				"He scolded them saying, <<Hey! You-pl are very stubborn! People have told " +
				"you that, they have seen Me with their very own eyeballs, but you don't " +
				"believe! You say, they're all lying/talking.nonsense! So! Now, I myself " +
				"have come. I truly died, but now, I live again! So, you just see for " +
				"yourselves.",
			"\\nt mata biji = KM very own eyeballs (vs. BI biji mata 'pupils')",
			"\\p",
			"\\v 15",
			"\\vt Jadi, sakarang bosong musti pi kuliling di ini dunya, ko pi kasi tau " +
				"samua orang, Beta pung Carita Bae ni.",
			"\\btvt Thus, now you must go all around this earth, to tell all people, " +
				"this Good Story of Mine.",
			"\\nt dropped prep 'about' to zero",
			"\\cf 16:15: Utusan dong pung Carita 1:8",
			"\\v 16",
			"\\vt Sapa yang parcaya sang Beta, deng dapa sarani, nanti Tuhan Allah kasi " +
				"salamat sang dia, ko dia dapa tenga tarús deng Tuhan Allah di sorga. " +
				"Ma sapa yang sonde parcaya, nanti dia kaná hukum, deng Tuhan tola buang " +
				"sang dia ko tenga tarús di luar.",
			"\\btvt Whoever believes in Me, and gets *baptised, God will save him, so that " +
				"he can live continually with God in heaven. But whoever doesn't believe, " +
				"he will get sentenced/punished, and the Lord with push throw.away him so." +
				"that he stay continually outside (heaven).",
			"\\v 17",
			"\\vt Deng orang yang parcaya sang Beta, nanti dong bekin tanda heran " +
				"macam-macam, ko samua orang tau bilang dong pung kuasa tu, memang datang " +
				"dari Tuhan. Andia ko, nanti dong bisa usir setan-setan pake Beta pung " +
				"nama. Nanti Tuhan Allah bekin dong bisa ba'omong pake bahasa laen yang " +
				"dong sonde tau.",
			"\\btvt And the people who believe in Me, they will do *miracles of all sorts, " +
				"so.that all people know that that power of theirs, truly comes from the " +
				"Lord. That's.why they will be able to expel  *evil spirits using My name. " +
				"Later God will make them able to speak using other languages that they " +
				"don't know.",
			"\\v 18",
			"\\vt Ais, kalo dong pegang ular baracon ko, dong minum kaná racon, dong sonde " +
				"kaná calaka. Ju kalo dong taro tangan di atas orang saki pung kapala, " +
				"nanti itu orang jadi bae.>>",
			"\\btvt Then, if they hold poisonous snake or, they drink being.affected.by " +
				"poison, they will not experience misfortune. And if they put hands on " +
				"top of sick people's heads, those people will become well/healed.>>",
			"\\ud 20/Dec/2004",
			"",
			"\\rcrd mrk16.19-16.10",
			"\\s Tuhan Yesus ta'angka nae pi surga",
			"\\bts Lord Yesus is taken ascending to heaven",
			"\\r (Lukas 24:50-53; Utusan dong pung Carita 1:9-11)",
			"\\p",
			"\\v 19",
			"\\vt Abis ba'omong deng Dia pung ana bua tu, ju Tuhan Allah angka kasi nae " +
				"sang Yesus pi sorga. Di situ, Dia jadi Tuhan Allah pung tangan kanan, " +
				"ko dong dua dudu parenta sama-sama.",
			"\\btvt After speaking with those disciples of his, then God lift raised Yesus " +
				"to heaven. There He is God's right hand, so the two of them rule together.",
			"\\ov Bagitu Yesus abis omong deng dong ju Dia taangka naek pi di sorga. ",
			"\\cf 16:19: Utusan dong pung Carita 1:9-11",
			"\\p",
			"\\v 20",
			"\\vt Ais ju, Dia pung ana bua dong, iko Dia pung parenta. Dong pi di mana-mana " +
				"ko kasi tau samua orang, Yesus pung Carita Bae. Ju Tuhan Allah kasi kuasa " +
				"sang dong, ko dong bekin samua tanda heran yang Yesus su kasi tau tu. Ais " +
				"banya orang parcaya sang Yesus, te dong su tau bilang Carita Bae tu, " +
				"memang batúl.",
			"\\btvt And then, His disciples, followed His command. The went all.over.the." +
				"placee to tell all people, Yesus' Good Story. And God gave them power so." +
				"that they could do all those miracles that Yesus had told them. Then many " +
				"people believed in Yesus, cuz they knew that that Good Story, is really " +
				"true.",
			"\\p",
			"\\s MARKUS PUNG TUTUP CARITA, IKO TULISAN DOLU-DOLU YANG LAEN",
			"\\bts THE END OF MARKUS' STORY, FOLLOWING ANOTHER ANCIENT WRITING",
			"\\ft 16:9-10: Tulisan bahasa Yunani yang paling tua deng yang paling bae abis " +
				"di ayat 8. Markus pung Carita tutup deng dua cara: satu yang panjang " +
				"(Markus 16:9-20), deng satu laen yang pendek (Markus 16:9-10). Iko " +
				"orang-orang yang pintar dong, yang mangarti bae-bae Markus pung Carita " +
				"ni, dua tutup ni dong tulis dari balakang. Dua-dua carita Yesus bangkit " +
				"dari Dia pung mati, deng tugas orang yang parcaya sang Yesus.",
			"\\btft 16:9-10: The oldest writings in the Yunani language, and those that " +
				"are best, end at verse (loan) 8. Markus' Story closes in two ways: one " +
				"which is long (Markus 16:9-20), and another which is short (Markus " +
				"16:9-10). According to the opinion of knowledgable people who know " +
				"Markus' story very well (=expert), these two closings were written " +
				"afterward. Both of them tell of the resurrection of Yesus from His death, " +
				"and the work/task of people who believe in Yesus.",
			"\\p",
			"\\v 9 ",
			"\\vt Waktu parampuan tiga orang tu, su sampe di Petrus dong, ju dong kasi " +
				"tau di samua-samua tentang itu orang muda yang ba'omong di kubur tadi.",
			"\\btvt When those three women, had arrived at Petrus them, then they told " +
				"everyone about that young person who had talked in the grave earlier.",
			"\\v 10",
			"\\vt Abis ju, Yesus sandiri parenta Dia pung ana bua, ko dong pi kasi tau " +
				"Dia pung Carita Bae ni di samua tampa, sampe di dunya pung ujung-ujung. " +
				"Carita Bae ini kasi tunju jalan ko Tuhan Allah kasi salamat orang dong " +
				"dari dong pung sala-sala, ko dong idop tarús deng Dia.",
			"\\btvt And then, Yesus himself commanded His disciples, that they go tell " +
				"this Good Story of His to all places, to the ends  of the world. This " +
				"Good Story shows the way for God to save people from thier sins1, so.that " +
				"they can live continually with Him.",
			"\\p",
			"\\vt Carita Bae ini, memang batúl sakali. Andia ko dia batahan tarús, sampe " +
				"sonde tau abis-abis.",
			"\\btvt This Good Story, is truly  very true. That.is.why it endures " +
				"continually, to-point-that it never ends.",
			"\\p",
			"\\cat c:\\graphics\\maps\\bible\\palestin.bmp",
			"\\ref width:10.5cm",
			"\\e",
			"\\ud 20/Dec/2004"
		};
		#endregion
		#region DATA: string[] m_RawHelongMark
		static public string[] m_RawHelongMark = new string[] 
		{
			"\\_sh v3.0  96  SHW-Scripture",
			"\\_DateStampHasFourDigitYear ",
			"",
			"\\rcrd mrk",
			"\\h Markus",
			"\\st Dehet Dais Banan deng Yesus Kristus",
			"\\btst The Good News Story about Yesus Kristus",
			"\\st muding",
			"\\btst according to",
			"\\mt Markus",
			"\\btmt Mark",
			"",
			"\\rcrd mrk16.12-16.13",
			"\\s Lamtua Yesus tutnaal Maria deng Magdala",
			"\\bts Lord Jesus meets with Maria from Mgdala",
			"\\r (Lukas 24:13-35)",
			"\\p ",
			"\\v 9",
			"\\vt Leol minggu oskaong nga, Yesus lako tulu sila ka muna bel Maria deng " +
				"Magdala, man hmunan nu Yesus nulut uik dale itu deng un apa ka.",
			"\\btvt Easrly Sunday Morning, Jesus went showed [his] face first to Maria " +
				"from Magdala, who earlier Jesus had cast out 7 evil spirits from her body.",
			"\\v 10",
			"\\vt Un ngaat hidi Yesus, kon un lako pait le tek atulis totoang man in " +
				"muid Yesus lolo hmunan nua. Oen nakbua nabael nol dael ili, nol oen " +
				"lilu tu-tungus, undeng oen nangan Yesus in mate ka.",
			"\\btvt She finished seeing Jesus, then she went home to tell all the People " +
				"there who folowed Jesus from long ago. They were still gathered to " +
				"remember that death of Jesus', and they were also crying.",
			"\\v 11",
			"\\vt Lako lius se naa kon, Maria tekas noan, <<Sataon le mi sus nabale lia? " +
				"Sus den tia, ta Yesus nuli pait son! Apin ni, halas-sam auk tutnaal " +
				"Una lam!>>",
			"\\btvt Arrving there, Maria told them saying, <<Why then you-pl still in a " +
				"state of difficulty? Don't be in a state of difficulty, cuz Jesus " +
				"alread y lives again! Just now, I really met with Him !>>",
			"\\nt dael ili = sakit hati, sedih",
			"\\p ",
			"\\vt Mo oen totoang situn noan, <<He! Ku in dehet ta nole'!>>",
			"\\btvt But they all replied saying, <<Hey! That story of yours is nonsense!>>",
			"",
			"\\rcrd mrk16.14-16.18",
			"\\s Lamtua Yesus tulu sila ka bel atuli at dua pait",
			"\\bts Lord Jesus shows face to two men as.well",
			"\\r (Mateos 28:16-20; Lukas 24:36-49; Yuhanis 20:19-23; Lamtua Yesus Anan " +
				"in Nutus sas Dehet tas 1:6-8)",
			"\\p ",
			"\\v 12",
			"\\vt Hidi na, Yesus ana in muid Una ngas at dua lakos se ingu mesa. Se " +
				"lalan hlala ka, kon Yesus tulu sila bel one, mo Un daid kisa son. Mo " +
				"nesang lo kam, oen tan Una.",
			"\\btvt After that, two of Jesus's disciples were going to a village. In " +
				"the middle of the raod then, Jesus showed face [revealed himself] to " +
				"them, and He was different. But not longer, they recognised Him.",
			"\\v 13",
			"\\vt Hidim oen duas pait lakos tek oen tapans sas noan, <<Hei! Mi ming le! " +
				"Apin ni, halas-sam kaim tutnaal Yesus se lalan nua!>>",
			"\\btvt Then the two went to their friends saying, <<Hey! You-pl listen up! " +
				"Earlier, we just meet with Jesus on the road over there!>>",
			"\\p ",
			"\\vt Mo oen tapana sas totoang situs noan, <<He! Mi dehet tam, nole ela " +
				"deken!>>",
			"\\btvt But all their firends repleid saying, <<Hey! Don't You-pl speak " +
				"rubbish like that!>>",
			"\\nt batan = sejajar, companion ; nole = omong kosong, putar-balek, " +
				"bohong, dusta",
			"",
			"\\rcrd mrk16.19-16.10",
			"\\s Lamtua Yesus tulu sila ka bel ana in muid Una ngas totoang",
			"\\bts Lord Jesus shows face to all the disciples",
			"\\r (Lukas 24:50-53; Lamtua Yesus Anan in Nutus sas Dehet tas 1:9-11)",
			"\\p ",
			"\\v 14",
			"\\vt Hidi kon, Yesus tutnaal nol Un ana in muid Una at hngul-esa kas, oras " +
				"oen daad le kaa. Un kaing oen noan, <<We! Mi tuluns sas baut isi'! Atuli " +
				"tek mi son noan, oen net Auk son nol oen matan beas esa, mo mi parsai " +
				"lo kam! Mi noan, oen totoang dehet in nole kam! Halas ni, Auk esang " +
				"maang. Auk meman mateng, mo halas ni, Auk nuling pait son! Mi ngaat esan.",
			"\\btvt And then, Jesus meet with His 12 disciples, while they were sitting " +
				"eating. He scolded them saying, <<Wah! You-pl are stubborn! People had " +
				"told you-pl that, they saw Me already with thir own eyes, but you-pl " +
				"did not beleive them! You-pl said, all of them were spreaking nonsense! " +
				"So now, I myself have come. I truly was dead, but now, I live already " +
				"again! You-pl see for yourselves.",
			"\\p ",
			"\\v 15",
			"\\vt Tiata, halas ni mi musti laok pap-mees se apan-kloma kia, le tek atulis " +
				"totoang Auk Dehet Banan nia.",
			"\\btvt Therefore, now you-pl must go around this world, to go tell all people " +
				"this My Good News.",
			"\\cf 16:19: Lamtua Yesus Anan in Nutus sas Dehet tas 1:9-11",
			"\\v 16",
			"\\vt Sii man parsai Au, nol haup in sarani, bet Lamtua Allah bel un slamat, " +
				"le un haup in daad napiut nol Lamtua Allah se sorga. Mo asii man parsai " +
				"lo', bet un haup in hukung, nol Lamtua hutun soleng una, le nang se " +
				"likun na tuun.",
			"\\btvt Whoever beleives Me, and obtains baptism, then God will give him " +
				"salvation, so.that. he obtains continual life with God in heaven. But " +
				"whoever doesn't beleive, then he obtains a sentence, with the Lord " +
				"pushing away throwing out him leaving him outside (heaven).",
			"\\v 17",
			"\\vt Nol atuli in parsai Au, bet oen tao tad heran bili-ngala', le atulis " +
				"totoang tanan noan oen kuasa ka, meman maa deng Ama Lamtua. Tiata, " +
				"oen bisa nulut uik dale kas totoang nini Auk ngalang. Hidim Ama Lamtua " +
				"Allah tao oen le aa nol dais didang man oen tanan lo ka.",
			"\\btvt And the peopel who beleive Me, then they will do various miracles " +
				"(signs), so.that all people know that their power, actually come from " +
				"the Lord. So.that then they can cast out all evil spirits suing My " +
				"name. Then God will make them speak using different languages that " +
				"they don't know.",
			"\\v 18",
			"\\vt Hidim, eta oen hep ula lasong tam (sila el ul-hmolo), ta oen niun tom " +
				"laso nam, oen tom calaka lo'. Eta oen tao iman se atuil ili bon nas, " +
				"bet atuli las banan.>>",
			"\\btvt Then, if they hold a poisonious snake (like the green snake), or " +
				"they drink or experience poison, they will not experience misfortune. " +
				"And if they put their hands onto sick people's heads, then those people " +
				"wil becoem well.>>",
			"\\nt kisa-kisa = macam2; |b18:|r laso = racun ; calaka = is there a better " +
				"word?",
			"",
			"\\rcrd mrk16.19-16.10 2",
			"\\s Lamtua Yesus sake lako sorga",
			"\\bts Lord Jeuss ascends into heaven",
			"\\r (Lukas 24:50-53; Lamtua Yesus Anan in Nutus sas Dehet tas 1:9-11)",
			"\\p ",
			"\\v 19",
			"\\vt Aa hidi nol Un ana in muid Una ngas, kon Lamtua Allah nikit sakeng " +
				"Yesus lako sorga. Se naa, Un daid Lamtua Allah ima kanan, le oen duas " +
				"daad kil bandu leo-leo.",
			"\\btvt Finishing speaking with His disciples, then God lifted up took on " +
				"Jesus into Heaven. There He sat at God's right hand so the two of them " +
				"rule together.",
			"\\cf 16:19: Lamtua Yesus Anan in Nutus sas Dehet tas 1:9-11",
			"\\p ",
			"\\v 20",
			"\\vt Hidi kon, Un ana in muid Una ngas, muid Un in leka-tadu. Oen lakos " +
				"se ola-ola le tek atuli las totoang, Dehet Banan deng Yesus sa. Kon " +
				"Ama Lamtua Allah beles kuas le oen tao naal tad heran mamo el man " +
				"Yesus tek oen son nas. Atuli mamo parsai Yesus, ta oen tanan son noan " +
				"Dehet Banan na, meman tom bak tetebes.",
			"\\btvt And then, His discipes, followed His command. They went to " +
				"everywhere to tell all people, the Good News from Jesus. And God gave " +
				"power to their many miracles like Jesus told them already.",		};
		#endregion


		#region Constructor()
		public Test_DProject()
			: base("DProject")
		{
			AddTest( new IndividualTest( Test1 ),  "Test1" );
		}
		#endregion

		public void Test1()
		{
		}


	}
	#endregion

}
