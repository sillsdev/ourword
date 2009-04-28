/**********************************************************************************************
 * Project: Our Word!
 * File:    DTeamSettings.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: The settings that apply across multiple projects; e.g., the settings for the
 *            entire Timor Team. 
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

namespace JWdb.DataModel
{
	#region CLASS DTeamSettings
	public class DTeamSettings : JObjectOnDemand
	{
        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: int SettingsVersion
        public int SettingsVersion
        {
            get
            {
                return m_nSettingsVersion;
            }
            set
            {
                m_nSettingsVersion = value;
            }
        }
        int m_nSettingsVersion = 0;
        #endregion

        #region BAttr{g/s}: string FileNameLanguage
        public string FileNameLanguage
        {
            get
            {
                // We need to be able to read in the old way of doing things, which was
                // an integer. Thus we don't mess up the Timor data.
                if (-1 != m_nOldFileNameLanguage)
                {
                    if (0 == m_nOldFileNameLanguage)
                        return "English";
                    if (1 == m_nOldFileNameLanguage)
                        return "Bahasa Indonesia";
                }

                return m_sFileNameLanguage;
            }
            set
            {
                SetValue(ref m_nOldFileNameLanguage, -1);
                SetValue(ref m_sFileNameLanguage, value);
            }
        }
        int m_nOldFileNameLanguage = -1;
        string m_sFileNameLanguage = "English";
        #endregion

        #region BAttr{g/s}: string CopyrightNotice
        public string CopyrightNotice
		{
			get
			{
				return m_sCopyrightNotice;
			}
			set
			{
                SetValue(ref m_sCopyrightNotice, value);
			}
		}
		string m_sCopyrightNotice = "";
		#endregion

        #region DOC: To Add Another Type of Footer Part
        /* To Add Another Type of Footer Part
		 * 
		 * - TeamSettings
		 *     - Add it to the FooterParts enumeration
		 *     - If it is to be a default value, add to the appropriate FooterPart BAttr
		 * - Add the Localization resource to DlgProps, PrintOptionsPage section
		 * - SetupPrintOptionsPage
		 *     - Add to GetTextFromFooterPart()
		 *     - Add to GetFooterPartFromString()
		 *     - Add to Loalization()
		 * - Class PPage, add to _GetFooterPartString()
		 * - Helps: Add it to the Print Options Page
		 * 
		 */
		#endregion
		#region Enum: FooterParts
		public enum FooterParts
		{
			kBlank, 
			kCopyrightNotice, 
			kPageNumber, 
			kScriptureReference, 
			kStageAndDate,
			kLanguageStageAndDate
		};
		#endregion
		#region BAttr{g/s}: FooterParts OddLeft
		public FooterParts OddLeft
		{
			get
			{
				return (FooterParts)m_OddLeft;
			}
			set
			{
                SetValue(ref m_OddLeft, (int)value);
            }
		}
		int m_OddLeft = (int)FooterParts.kCopyrightNotice;
		#endregion
		#region BAttr{g/s}: FooterParts OddMiddle
		public FooterParts OddMiddle
		{
			get
			{
				return (FooterParts)m_OddMiddle;
			}
			set
			{
                SetValue(ref m_OddMiddle, (int)value);
            }
		}
		int m_OddMiddle = (int)FooterParts.kPageNumber;
		#endregion
		#region BAttr{g/s}: FooterParts OddRight
		public FooterParts OddRight
		{
			get
			{
				return (FooterParts)m_OddRight;
			}
			set
			{
                SetValue(ref m_OddRight, (int)value);
			}
		}
		int m_OddRight = (int)FooterParts.kScriptureReference;
		#endregion
		#region BAttr{g/s}: FooterParts EvenLeft
		public FooterParts EvenLeft
		{
			get
			{
				return (FooterParts)m_EvenLeft;
			}
			set
			{
                SetValue(ref m_EvenLeft, (int)value);
			}
		}
		int m_EvenLeft = (int)FooterParts.kScriptureReference;
		#endregion
		#region BAttr{g/s}: FooterParts EvenMiddle
		public FooterParts EvenMiddle
		{
			get
			{
				return (FooterParts)m_EvenMiddle;
			}
			set
			{
                SetValue(ref m_EvenMiddle, (int)value);
			}
		}
		int m_EvenMiddle = (int)FooterParts.kPageNumber;
		#endregion
		#region BAttr{g/s}: FooterParts EvenRight
		public FooterParts EvenRight
		{
			get
			{
				return (FooterParts)m_EvenRight;
			}
			set
			{
                SetValue(ref m_EvenRight, (int)value);
			}
		}
		int m_EvenRight = (int)FooterParts.kLanguageStageAndDate;
		#endregion

        #region BAttr{g}: BStringArray NotesCategories
        public BStringArray NotesCategories
        {
            get
            {
                return m_bsaNotesCategories;
            }
        }
        public BStringArray m_bsaNotesCategories = null;
        #endregion
        #region BAttr{g}: BStringArray NotesFrontCategories
        public BStringArray NotesFrontCategories
        {
            get
            {
                return m_bsaNotesFrontCategories;
            }
        }
        public BStringArray m_bsaNotesFrontCategories = null;
        #endregion

        /***
        ***/
        #region Attr{g/s}: bool RepositoryIsActive - T if source control is turned on
        public bool RepositoryIsActive
        {
            get
            {
                return m_bRepositoryIsActive;
            }
            set
            {
                SetValue(ref m_bRepositoryIsActive, value);
            }
        }
        bool m_bRepositoryIsActive;
        #endregion

		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();

            DefineAttr("SettingsVersion", ref m_nSettingsVersion);

            DefineAttr("FileNameLanguage", ref m_sFileNameLanguage);
            DefineAttr("FileNameLang", ref m_nOldFileNameLanguage);  // Deprecated
			DefineAttr("Copyright",    ref m_sCopyrightNotice);

			DefineAttr("OddLeft",      ref m_OddLeft);
			DefineAttr("OddMiddle",    ref m_OddMiddle);
			DefineAttr("OddRight",     ref m_OddRight);
			DefineAttr("EvenLeft",     ref m_EvenLeft);
			DefineAttr("EvenMiddle",   ref m_EvenMiddle);
			DefineAttr("EvenRight",    ref m_EvenRight);

            // Translator Notes
            DefineAttr("NotesCategories", ref m_bsaNotesCategories);
            DefineAttr("NotesFrontCategories", ref m_bsaNotesFrontCategories);

            DefineAttr("Repository", ref m_bRepositoryIsActive);
        }
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g}: DStyleSheet StyleSheet
		public DStyleSheet StyleSheet
		{
			get 
			{ 
				return m_StyleSheet.Value; 
			}
		}
		private JOwn<DStyleSheet> m_StyleSheet = null;  
		#endregion
		#region JAttr{g}: DSFMapping SFMapping
		public DSFMapping SFMapping
		{
			get
			{
				if (null == j_SFMapping.Value)
					j_SFMapping.Value = new DSFMapping();
				return j_SFMapping.Value;
			}
		}
		private JOwn<DSFMapping> j_SFMapping = null;
		#endregion
		#region JAttr{g}: BookStages TranslationStages
		public BookStages TranslationStages
			// See the note in ConstructAttrs as to why we initialize this here.
		{
			get 
			{ 
				if (null == m_TranslationStages)
                    m_TranslationStages = new JOwn<BookStages>("Stages", this);

				if (null == m_TranslationStages.Value)
					m_TranslationStages.Value = new BookStages();

				return m_TranslationStages.Value; 
			}
		}
		private JOwn<BookStages> m_TranslationStages = null;  
		#endregion
        #region JAttr{g}: JOwnSeq BookGroupings
        public JOwnSeq<DBookGrouping> BookGroupings
        {
            get
            {
                if(null == m_BookGroupings)
                    m_BookGroupings = new JOwnSeq<DBookGrouping>("BookGroupings", this);
                if (m_BookGroupings.Count == 0)
                    DBookGrouping.InitializeGroupings(m_BookGroupings);
                return m_BookGroupings;
            }
        }
        private JOwnSeq<DBookGrouping> m_BookGroupings = null;
        #endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DTeamSettings()
			: base()
		{
			// Our Default Display Name is simply "Our Word." Since this is the namem of the
			// cluster, users will likely change it to something more meaningful, like "Timor"
            DisplayName = LanguageResources.AppTitle;

			// Style Sheet
			m_StyleSheet = new JOwn<DStyleSheet>("StyleSheet", this);

			// Translation Stages. Note that we initialize this at runtime (i.e., 
			// we call "new BookStages" as part of the TranslationStages
			// get accessor, rather than here, because:
			// 1. Doing it here puts us in an infinite loop, because the
			//    BookStages constructor needs to know about TeamSettings,
			//    which may not exist yet, and
			// 2. BookStages was a later addition and earlier data (e.g.,
			//    Timor) does not have it, so to construct it here means
			//    that the Read operation would overwrite it.
			m_TranslationStages = new JOwn<BookStages>("Stages", this);

            // Book Groupings
            m_BookGroupings = new JOwnSeq<DBookGrouping>("BookGroupings", this);
            DBookGrouping.InitializeGroupings(m_BookGroupings);

			// Standard Format conversion mapping
			j_SFMapping = new JOwn<DSFMapping>("SFMapping", this);
			j_SFMapping.Value = new DSFMapping();

			// Default copyright notice
			if (CopyrightNotice.Length == 0)
			{
				CopyrightNotice = "Copyright © " + 
					DateTime.Today.Year.ToString() + ".";
			}

            // Translator Notes Categories
            m_bsaNotesCategories = new BStringArray();
            m_bsaNotesFrontCategories = new BStringArray();

            // Local Repository
            m_Repository = new Repository(this, RepositoryIsActive);
        }
		#endregion
        #region Constructor(sDisplayName)
        public DTeamSettings(string sDisplayName)
            : this()
        {
            DisplayName = sDisplayName;
        }
        #endregion

        // Repository ------------------------------------------------------------------------
        #region Attr{g}: Repository Repository
        public Repository Repository
        {
            get
            {
                return m_Repository;
            }
        }
        Repository m_Repository;
        #endregion

        // Initializations -------------------------------------------------------------------
        #region Method: void New() - resets the Team Settings to the factory defaults
        public void New()
		{
			// Clear out everything
			Clear();

			// Set to default values
			EnsureInitialized();
		}
		#endregion
        #region void EnsureInitialized()
        public void EnsureInitialized()
            // This is re-entrant; we want to be able to scan an existing TeamSettings
            // to make sure it reflects our current needs, as OurWord may change 
        {
            // Style Sheet
            if (null == m_StyleSheet.Value)
                m_StyleSheet.Value = new DStyleSheet();
            StyleSheet.Initialize(false);

			// Make sure we have writing systems for the current project. Just create
			// blank ones if we need to.
			if (null != DB.Project)
			{
				foreach (DTranslation t in DB.Project.AllTranslations)
				{
					StyleSheet.FindOrAddWritingSystem(t.WritingSystemVernacular.Name);
					StyleSheet.FindOrAddWritingSystem(t.WritingSystemConsultant.Name);
				}
			}

            // TODO: Other TeamSettings initializations
        }
		#endregion
		#region Method: void TemporaryFixes()
		public void TemporaryFixes()
		{
			int nCurrentSettingsVersion = 1;

			if (SettingsVersion < nCurrentSettingsVersion)
			{
				// Fix Hebrews being associated with the wrong grouping
				DBookGrouping.InitializeGroupings(DB.TeamSettings.BookGroupings);
			}

			SettingsVersion = nCurrentSettingsVersion;
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region SAttr{g}: string FileExtension
		static public string FileExtension
		{
			get
			{
				return ".owt";
			}
		}
		#endregion
        #region OAttr{g}: override string DefaultFileExtension
        public override string DefaultFileExtension
        {
            get
            {
				return FileExtension;
            }
        }
        #endregion
		#region SAttr{g}: string SettingsFolderName
		static public string SettingsFolderName
		{
			get
			{
				return ".Settings";
			}
		}
		#endregion
		#region SAttr{g}: string BackupFolderName
		static public string BackupFolderName
		{
			get
			{
				return ".Backup";
			}
		}
		#endregion
		#region Attr{g}: string ClusterFolder - Top-level, e.g., "MyDocuments\OurWord"
		public string ClusterFolder
			/* We place the Cluster's root onto the user's MyDocuments, thus, e.g.,
			 *    MyDocuments\OurWord\
			 *    MyDocuments\Timor\
			 *    
			 * I originally considered using AllUsers\MyDocuments, so that multiple users
			 * (log ins) can share; but then decided that instead, it would make more
			 * sense for each user to have his own repostory; and then we can use
			 * Hg to synch across users on the same computer.
			*/
		{
			get
			{
				// The JWU method ensures that the folder exists
				return JWU.GetMyDocumentsFolder(DisplayName);
			}
		}
		#endregion
		#region Attr{g}: string SettingsFolder - Top-level, e.g., "MyDocuments\OurWord\Settings"
		public string SettingsFolder
		{
			get
			{
				// Make sure the folder exists
				string sFolder = ClusterFolder + SettingsFolderName + Path.DirectorySeparatorChar;
				if (!Directory.Exists(sFolder))
					Directory.CreateDirectory(sFolder);

				return sFolder;
			}
		}
		#endregion
		#region Attr{g}: string BackupFolder - Top-level, e.g., "MyDocuments\OurWord\Backup"
		public string BackupFolder
		{
			get
			{
				// Make sure the folder exists
				string sFolder = ClusterFolder + BackupFolderName + Path.DirectorySeparatorChar;
				if (!Directory.Exists(sFolder))
					Directory.CreateDirectory(sFolder);

				return sFolder;
			}
		}
		#endregion
		#region OAttr{g}: string StoragePath
		public override string StoragePath
		{
			get
			{
				return SettingsFolder + StorageFileName;
			}
		}
		#endregion
        #region OAttr{g}: string StorageFileName
        public override string StorageFileName
        {
            get
            {
                // Make sure we have a display name. Old Version1 files will not have this 
                // set, so reading an old file will mess up.
                if (string.IsNullOrEmpty(DisplayName))
                    DisplayName = LanguageResources.AppTitle;

                return base.StorageFileName;
            }
        }
        #endregion
        #region OMethod: bool OnLoad(TextReader)
        protected override bool OnLoad(TextReader tr, string sPath, IProgressIndicator progress)
        {
            bool bResult = base.OnLoad(tr, sPath, progress);

            Repository.Active = RepositoryIsActive;

            return bResult;
        }
        #endregion

        // Disk Directory Operations ---------------------------------------------------------
		#region SMethod: List<s> GetLanguageListFromDisk(sClusterName)
		static public List<string> GetLanguageListFromDisk(string sClusterName)
		{
			List<string> v = new List<string>();

			// Get the settings folder
			string sSettingsFolder = JWU.GetMyDocumentsFolder(sClusterName);
			sSettingsFolder += DTeamSettings.SettingsFolderName +
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
	#endregion

	#region CLASS: DStyleSheet : JStyleSheet
	public class DStyleSheet : JStyleSheet
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DStyleSheet()
			: base()
		{
		}
		#endregion

 		// Finding Styles --------------------------------------------------------------------
		#region Method: JParagraphStyle FindParagraphStyleOrNormal(string sStyleAbbrev)
		public JParagraphStyle FindParagraphStyleOrNormal(string sStyleAbbrev)
		{
			JParagraphStyle pStyle = FindParagraphStyle(sStyleAbbrev);
			if (null == pStyle)
				pStyle = FindParagraphStyle(c_sfmParagraph);
			Debug.Assert(null != pStyle);
			return pStyle;
		}
		#endregion
		#region Method: JCharacterStyle FindCharacterStyleOrNormal(string sAbbrev)
		public JCharacterStyle FindCharacterStyleOrNormal(string sAbbrev)
		{
			// Look amongst the character styles
			JCharacterStyle cStyle = FindCharacterStyle(sAbbrev);

			// If we dont find it there, look amongst the paragraph styles
			if (null == cStyle)
			{
				JParagraphStyle pStyle = FindParagraphStyleOrNormal(sAbbrev);
				cStyle = pStyle.CharacterStyle;
			}

			Debug.Assert(null != cStyle);
			return cStyle;
		}
		#endregion

		// Character Style Abbreviations -----------------------------------------------------
		public const string c_StyleAbbrevVerse            = "v";
		public const string c_StyleAbbrevChapter          = "c";
		public const string c_StyleAbbrevFootLetter       = "fn";
		public const string c_StyleAbbrevItalic           = "i";
		public const string c_StyleAbbrevBold             = "b";
		public const string c_StyleAbbrevUnderline        = "u";
		public const string c_StyleAbbrevDashed           = "d";
        public const string c_StyleAbbrevBigHeader        = "bh";
		public const string c_StyleAbbrevLabel            = "L";
        public const string c_StyleAbbrevPictureCaption   = "cap";
        public const string c_StyleFootnote               = "ft";

        // User-Interface Only
        public const string c_CStyleRevisionDeletion = "del";
        public const string c_CStyleRevisionAddition = "add";

        // SFM Markers -----------------------------------------------------------------------
        // Book Title / Header
        public const string c_sfmRunningHeader          = "h";
        public const string c_sfmBookTitle              = "mt";
        public const string c_sfmBookSubTitle           = "st";
        // Vernacular Paragraphs
        public const string c_sfmMajorSection           = "ms";
        public const string c_sfmMajorSectionCrossRef   = "mr";
        public const string c_sfmSectionHead            = "s";
        public const string c_sfmSectionHeadMinor       = "s2";
        public const string c_sfmCrossReference         = "r";
		public const string c_sfmParagraph              = "p";
        public const string c_sfmParagraphContinuation  = "m";
		public const string c_sfmLine1                  = "q";
		public const string c_sfmLine2                  = "q2";
		public const string c_sfmLine3                  = "q3";

        // Other
        public const string c_sfmTranslatorNote         = "tn";

		// Paragraph Style Abbreviations -----------------------------------------------------
        // Scripture Text

        public const string c_StyleQuoteCentered          = "qc";
        public const string c_StyleReferenceTranslation   = "RefTrans";

        // Translator Notes
        public const string c_StyleNoteHeader             = "NoteHeader";
        public const string c_StyleNoteDate               = "NoteDate";
        public const string c_StyleNoteDiscussion         = "NoteDiscussion";
        public const string c_StyleNote                   = "nt";   // Note Discussion Paragraph

        // User-Interface Only
        public const string c_PStyleMergeHeader = "MergeHeader";
        public const string c_PStyleMergeParagraph = "MergeParagraph";

		#region Method: bool IsQuoteStyle(string sAbbrev)
		static public bool IsQuoteStyle(string sAbbrev)
		{
			if (sAbbrev == c_sfmLine1)
				return true;
			if (sAbbrev == c_sfmLine2)
				return true;
			if (sAbbrev == c_sfmLine3)
				return true;
			if (sAbbrev == c_StyleQuoteCentered)
				return true;
			return false;
		}
		#endregion

        // Classifications -------------------------------------------------------------------


		// Initialization --------------------------------------------------------------------
        public const string c_Latin = "Latin";
        public const string c_DefaultWritingSystem = c_Latin;
		#region Method: void _InitializeWritingSystems()
		private void _InitializeWritingSystems()
		{
			// A general-purpose writing system that will handle most situations
            if (null == FindWritingSystem(c_Latin))
            {
                JWritingSystem wsLatin = FindOrAddWritingSystem(c_Latin);
            }

			/***
			 * I HAD THESE in OurWord 1.0, but think it better to not do them
			 * any longer; users try to delete them but they keep coming back
			 * if I leave them here....and they are meaningless for most people.
			 * 
			// Chinese
            if (null == FindWritingSystem("Chinese"))
            {
                JWritingSystem wsChinese = FindOrAddWritingSystem("Chinese");
                wsChinese.IsIdeaGraph = true;
            }

			// Aboriginal languages
            if (null == FindWritingSystem("Aboriginal"))
            {
                JWritingSystem wsAboriginal = FindOrAddWritingSystem("Aboriginal");
            }

			// Northern Nigeria
            if (null == FindWritingSystem("Nigeria"))
            {
                JWritingSystem wsDoulos = FindOrAddWritingSystem("Nigeria");
            }
			***/
		}
		#endregion
		#region Method: void _InitializeParagraphStyles()
		private void _InitializeParagraphStyles()
		{
			JParagraphStyle style;

			// Running Header (h)
			if (null == FindParagraphStyle(c_sfmRunningHeader))
			{
                style = AddParagraphStyle(c_sfmRunningHeader, "Running Header");
				style.SetFonts(true, 10, true);
				style.IsJustified = true;
			}

			// UI Title
			if (null == FindParagraphStyle("uiTitle"))
			{
				style = AddParagraphStyle("uiTitle", "UI Title");
				style.SetFonts(false, 18, true);
				style.IsCentered = true;
			}

			// Main Book Title (mt)
            if (null == FindParagraphStyle(c_sfmBookTitle))
			{
                style = AddParagraphStyle(c_sfmBookTitle, "Book Title");
				style.SetFonts(false, 16, true);
				style.IsCentered = true;
				style.KeepWithNext = true;
                style.SpaceAfter = 9;
			}

			// Book SubTitle (st)
            if (null == FindParagraphStyle(c_sfmBookSubTitle))
			{
                style = AddParagraphStyle(c_sfmBookSubTitle, "Book SubTitle");
				style.SetFonts(false, 14, true);
				style.IsCentered = true;
				style.KeepWithNext = true;
                style.SpaceAfter = 9;
			}

            // Major Section header (ms)
            if (null == FindParagraphStyle(c_sfmMajorSection))
            {
                style = AddParagraphStyle(c_sfmMajorSection, "Major Section Title");
                style.SetFonts(false, 14, true);
                style.SpaceBefore = 12;
                style.IsCentered = true;
                style.KeepWithNext = true;
                style.SpaceAfter = 3;
            }

			// Section Header (s)
            if (null == FindParagraphStyle(c_sfmSectionHead))
			{
                style = AddParagraphStyle(c_sfmSectionHead, "Section Title");
				style.SetFonts(false, 12, true);
				style.SpaceBefore = 12;
				style.IsCentered = true;
				style.KeepWithNext = true;
                style.SpaceAfter = 3;
			}

			// Section Header Level 2 (s2)
            if (null == FindParagraphStyle(c_sfmSectionHeadMinor))
			{
                style = AddParagraphStyle(c_sfmSectionHeadMinor, "Section Title 2");
				style.SetFonts(false, 12, true);
				style.SpaceBefore = 9;
				style.IsCentered = true;
				style.KeepWithNext = true;
                style.SpaceAfter = 3;
			}

            // Major Section Cross Reference (mr)
            if (null == FindParagraphStyle(c_sfmMajorSectionCrossRef))
            {
                style = AddParagraphStyle(c_sfmMajorSectionCrossRef, "Major Cross References");
                style.SetFonts(true, 10, false, true, false, false, Color.Black);
                style.IsCentered = true;
                style.KeepWithNext = true;
                style.SpaceAfter = 3;
            }

            // Cross Reference (r)
            if (null == FindParagraphStyle(c_sfmCrossReference))
			{
                style = AddParagraphStyle(c_sfmCrossReference, "Cross References");
				style.SetFonts(true, 10, false, true, false, false, Color.Black);
				style.IsCentered = true;
				style.KeepWithNext = true;
                style.SpaceAfter = 3;
			}

			// Normal paragraph (p)
			if (null == FindParagraphStyle(c_sfmParagraph))
			{
				style = AddParagraphStyle(c_sfmParagraph, "Paragraph");
				style.SetFonts(true, 10, false);
				style.IsJustified = true;
			}

			// Paragraph continuation (m)
            if (null == FindParagraphStyle(c_sfmParagraphContinuation))
			{
                style = AddParagraphStyle(c_sfmParagraphContinuation, "Paragraph Continuation");
				style.SetFonts(true, 10, false);
				style.IsJustified = true;
			}

			// Quote Level 1 (q)
			if (null == FindParagraphStyle( c_sfmLine1 ))
			{
				style = AddParagraphStyle(c_sfmLine1, "Quote Level 1");
				style.SetFonts(true, 10, false);
				style.LeftMargin = 0.2;
				style.IsJustified = true;
			}

			// Quote Level 2 (q2)
			if (null == FindParagraphStyle(c_sfmLine2))
			{
				style = AddParagraphStyle(c_sfmLine2, "Quote Level 2");
				style.SetFonts(true, 10, false);
				style.LeftMargin = 0.4;
				style.IsJustified = true;
			}

			// Quote Level 3 (q3)
			if (null == FindParagraphStyle(c_sfmLine3))
			{
				style = AddParagraphStyle(c_sfmLine3, "Quote Level 3");
				style.SetFonts(true, 10, false);
				style.LeftMargin = 0.6;
				style.IsJustified = true;
			}

			// Centered quote (qc)
			if (null == FindParagraphStyle(c_StyleQuoteCentered))
			{
				style = AddParagraphStyle(c_StyleQuoteCentered, "Centered Quote");
				style.SetFonts(true, 10, false);
				style.LeftMargin = 0.2;
				style.RightMargin = 0.2;
			}

			// Footnote paragraph (ft)
            if (null == FindParagraphStyle(c_StyleFootnote))
			{
                style = AddParagraphStyle(c_StyleFootnote, "Footnote Paragraph");
				style.SetFonts(true, 10, false);
				style.IsJustified = true;
                style.SpaceAfter = 3;
            }

			// SeeAlso Footnote paragraph (cft)
			if (null == FindParagraphStyle("cft"))
			{
				style = AddParagraphStyle("cft", "SeeAlso Footnote Paragraph");
				style.SetFonts(true, 10, false);
				style.IsJustified = true;
                style.SpaceAfter = 3;
            }

            // Notes
            // Note Header
            if (null == FindParagraphStyle(c_StyleNoteHeader))
            {
                style = AddParagraphStyle(c_StyleNoteHeader, "Note Header");
                style.IsLeft = true;
                style.SpaceBefore = 0;
                style.SpaceAfter = 0;
            }
            if (null == FindParagraphStyle(c_StyleNoteDate))
            {
                style = AddParagraphStyle(c_StyleNoteDate, "Note Date");
                style.IsRight = true;
                style.SpaceBefore = 0;
                style.SpaceAfter = 0;
                style.SetFonts(true, 9, false);
            }
            // Note Discussion
            if (null == FindParagraphStyle(c_StyleNoteDiscussion))
            {
                style = AddParagraphStyle(c_StyleNoteDiscussion, "Note Discussion");
                style.IsJustified = true;
                style.SpaceBefore = 3;
                style.SpaceAfter = 3;
            }
            // Note Paragraph (soon to be deprecated)
            if (null == FindParagraphStyle(c_StyleNote))
			{
                style = AddParagraphStyle(c_StyleNote, "Note");
				style.IsJustified = true;
				style.FirstLineIndent = -0.20;
				style.LeftMargin = 0.20;
                style.SpaceAfter = 3;
            }

			// Consultant Note Paragraph (ntck)
			if (null == FindParagraphStyle("ntck"))
			{
				style = AddParagraphStyle("ntck", "Note for Consultant");
				style.IsJustified = true;
                style.SpaceAfter = 3;
            }

			// Picture caption (cap)
            if (null == FindParagraphStyle(c_StyleAbbrevPictureCaption))
			{
                style = AddParagraphStyle(c_StyleAbbrevPictureCaption, "Picture Caption");
				style.SetFonts(true, 10, false, true, false, false, Color.Black);
				style.IsCentered = true;
			}

            // Reference Translations
            if (null == FindParagraphStyle(c_StyleReferenceTranslation))
            {
                style = AddParagraphStyle(c_StyleReferenceTranslation, "Reference Translation");
                style.SetFonts(true, 10, false);
                style.IsJustified = true;
                style.FirstLineIndent = -0.20;
                style.LeftMargin = 0.20;
            }
		}
		#endregion

        #region Method: void _InitializeCharacterStyles()
        private void _InitializeCharacterStyles()
		{
			JCharacterStyle charStyle;

			// Chapter Number (c)
			if (null == FindCharacterStyle(c_StyleAbbrevChapter))
			{
				charStyle = AddCharacterStyle(c_StyleAbbrevChapter, "Chapter Number");
				charStyle.SetFonts(false, 20, true);
			}

			// Verse Number (v)
			if (null == FindCharacterStyle(c_StyleAbbrevVerse))
			{
				charStyle = AddCharacterStyle( c_StyleAbbrevVerse, "Verse Number");
				charStyle.SetFonts(false, 10, false, false, false, false, Color.Red);
				charStyle.IsSuperScript = true;
			}

			// Footnote Character (fn)
			if (null == FindCharacterStyle(c_StyleAbbrevFootLetter))
			{
				charStyle = AddCharacterStyle( 	c_StyleAbbrevFootLetter, "Footnote Character");
                charStyle.SetFonts(false, 10, false, false, false, false, Color.Navy);
                charStyle.IsSuperScript = true;
			}

            // Footnote Character (cf)
            if (null == FindCharacterStyle(c_StyleAbbrevBigHeader))
            {
                charStyle = AddCharacterStyle(c_StyleAbbrevBigHeader,
                    "Header in Window Panes");
                charStyle.SetFonts(false, 12, true);
            }

            // Label (L) - e.g., the verse reference at the beginning of a footnote
			if (null == FindCharacterStyle(c_StyleAbbrevLabel))
			{
				charStyle = AddCharacterStyle( c_StyleAbbrevLabel, "Label");
				charStyle.SetFonts(false, 10, false, false, false, false, Color.Crimson);
			}

			// Italic
			if (null == FindCharacterStyle(DStyleSheet.c_StyleAbbrevItalic))
			{
				charStyle = AddCharacterStyle( c_StyleAbbrevItalic, "Italic");
				charStyle.SetFonts(true, 10, false, true, false, false, Color.Black);
			}

			// Bold
			if (null == FindCharacterStyle(c_StyleAbbrevBold))
			{
				charStyle = AddCharacterStyle( c_StyleAbbrevBold, "Bold");
				charStyle.SetFonts(true, 10, true, false, false, false, Color.Black);
			}

			// Underlined
			if (null == FindCharacterStyle(c_StyleAbbrevUnderline))
			{
				charStyle = AddCharacterStyle( c_StyleAbbrevUnderline, "Underline");
				charStyle.SetFonts(true, 10, false, false, false, true, Color.Black);
			}

			// Dashed
			if (null == FindCharacterStyle(c_StyleAbbrevDashed))
			{
				charStyle = AddCharacterStyle( c_StyleAbbrevDashed, "Dashed");
				charStyle.SetFonts(true, 10, false, false, false, true, Color.Navy);
			}

			// Note Character (ntc)
			if (null == FindCharacterStyle("ntc"))
			{
				charStyle = AddCharacterStyle("ntc", "Note Character");
				charStyle.SetFonts(false, 10, false, false, false, false, Color.Navy);
				charStyle.IsSuperScript = true;
				charStyle.IsEditable = false;
			}

		}
		#endregion

        #region Method: void _InitializeUIParagraphStyles()
        private void _InitializeUIParagraphStyles()
            // TODO: Move appropriate styles to this section
        {
            JParagraphStyle style;

            // Merge Window: Header
            if (null == FindParagraphStyle(c_PStyleMergeHeader))
            {
                style = AddParagraphStyle(c_PStyleMergeHeader, "Merge Window Header");
                style.SetFonts(false, 12, true);
                style.SpaceAfter = 0;
            }
            // Merge Window: Paragraph
            if (null == FindParagraphStyle(c_PStyleMergeParagraph))
            {
                style = AddParagraphStyle(c_PStyleMergeParagraph, "Merge Window Paragraph");
                style.SetFonts(true, 10, false);
                style.Alignment = JParagraphStyle.AlignType.kJustified;
            }
        }
        #endregion
        #region Method: void _InitializeUICharacterStyles()
        private void _InitializeUICharacterStyles()
        {
            JCharacterStyle charStyle;

            // Revision Deletions
            if (null == FindCharacterStyle(c_CStyleRevisionDeletion))
            {
                charStyle = AddCharacterStyle(c_CStyleRevisionDeletion, "Revision Deletion");
                charStyle.SetFonts(false, 10, false);
            }

            // Revision Additions
            if (null == FindCharacterStyle(c_CStyleRevisionAddition))
            {
                charStyle = AddCharacterStyle(c_CStyleRevisionAddition, "Revision Addition");
                charStyle.SetFonts(false, 10, false);
            }

            // Int BT Glosses
            if (null == FindCharacterStyle("ibtGloss"))
            {
                charStyle = AddCharacterStyle("ibtGloss", "BT - Glosses");
                charStyle.SetFonts(false, 11, true);
            }

            // Int BT Analysis
            if (null == FindCharacterStyle("ibtAn"))
            {
                charStyle = AddCharacterStyle("ibtAn", "BT - Analysis Line");
                charStyle.SetFonts(false, 8, true);
            }
        }
        #endregion

		#region Method: void Initialize(bool bClearOutPrevious)
		public void Initialize(bool bClearOutPrevious)
		{
			// Remove the previous data if asked. (Leave the Writing Systems alone for now,
			// as we have the CurrentWS stuff going on.....and I think I'll just want to
			// get rid of WS's anyway before too long.)
			if (bClearOutPrevious)
			{
				ParagraphStyles.Clear();
				CharacterStyles.Clear();
			}

			// Initialize the various parts of the stylesheet
			_InitializeWritingSystems();
			_InitializeParagraphStyles();
            _InitializeUIParagraphStyles();
			_InitializeCharacterStyles();
            _InitializeUICharacterStyles();
		}
		#endregion
	}
	#endregion

}
