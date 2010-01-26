#region ***** DTeamSettings.cs *****
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
using System.IO;
using JWTools;
using OurWordData.Styles;
using OurWordData.Synchronize;

#endregion
#endregion

namespace OurWordData.DataModel
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

        #region BAttr{g/s}: string StagesSetup
        public string StagesSetup
        {
            get
            {
                if (string.IsNullOrEmpty(m_sStagesSetup) && null != m_Stages)
                    m_sStagesSetup = Stages.ToSaveString();

                return m_sStagesSetup;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && null != m_Stages)
                    Stages.FromSaveString(value);

                SetValue(ref m_sStagesSetup, value);
            }
        }
        string m_sStagesSetup = "";
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

		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();

            DefineAttr("SettingsVersion", ref m_nSettingsVersion);

			DefineAttr("Copyright",    ref m_sCopyrightNotice);
            DefineAttr("Stages", ref m_sStagesSetup);

			DefineAttr("OddLeft",      ref m_OddLeft);
			DefineAttr("OddMiddle",    ref m_OddMiddle);
			DefineAttr("OddRight",     ref m_OddRight);
			DefineAttr("EvenLeft",     ref m_EvenLeft);
			DefineAttr("EvenMiddle",   ref m_EvenMiddle);
			DefineAttr("EvenRight",    ref m_EvenRight);
        }
		#endregion

		// JAttrs ----------------------------------------------------------------------------
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

        // Stages ----------------------------------------------------------------------------
        #region Attr{g}: StageList Stages
        public StageList Stages
        {
            get
            {
                Debug.Assert(null != m_Stages);
                return m_Stages;
            }
        }
        StageList m_Stages;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DTeamSettings()
			: base()
		{
			// Our Default Display Name is simply "Our Word." Since this is the name of the
			// cluster, users will likely change it to something more meaningful, like "Timor"
            DisplayName = LanguageResources.AppTitle;

            // Stages
            m_Stages = new StageList();

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
        }
		#endregion
        #region Constructor(sDisplayName)
        public DTeamSettings(string sDisplayName)
            : this()
        {
            DisplayName = sDisplayName;
        }
        #endregion

        // Repositor Factories ---------------------------------------------------------------
        #region Method: InternetRepository GetInternetRepository()
        public InternetRepository GetInternetRepository()
        {
            return new InternetRepository(DisplayName);
        }
        #endregion
        #region Method: LocalRepository GetLocalRepository()
        public LocalRepository GetLocalRepository()
        {
            return new LocalRepository(ClusterFolder);
        }
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
			// Make sure we have writing systems for the current project. Just create
			// blank ones if we need to.
			if (null != DB.Project)
			{
				foreach (var t in DB.Project.AllTranslations)
				{
					StyleSheet.FindOrCreate(t.WritingSystemVernacular.Name);
                    StyleSheet.FindOrCreate(t.WritingSystemConsultant.Name);
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
                var ci = ClusterList.FindClusterInfo(DisplayName);
                if (null == ci)
                    return null;

                if (!Directory.Exists(ci.ClusterFolder))
                    Directory.CreateDirectory(ci.ClusterFolder);

                return ci.ClusterFolder;
			}
		}
		#endregion
		#region Attr{g}: string SettingsFolder - Top-level, e.g., "MyDocuments\OurWord\.Settings"
		public string SettingsFolder
		{
			get
			{
                string sClusterFolder = ClusterFolder;
                if (string.IsNullOrEmpty(sClusterFolder))
                    return null;

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
        #region Attr{g}: string StyleSheetStoragePath
        public string StyleSheetStoragePath
	    {
	        get
	        {
	            var sFileName = DisplayName + ".StyleSheet";
                return Path.Combine(SettingsFolder, sFileName);
	        }
        }
        #endregion
        #region OMethod: bool OnLoad(TextReader)
        protected override bool OnLoad(TextReader tr, string sPath, IProgressIndicator progress)
        {
            var bResult = base.OnLoad(tr, sPath, progress);
            return bResult;
        }
        #endregion
	}
	#endregion

	#region CLASS: DStyleSheet : JStyleSheet - GONE
    /*
	public class DStyleSheet : JStyleSheet
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DStyleSheet()
			: base()
		{
		}
		#endregion

		// Character Style Abbreviations -----------------------------------------------------
        public const string c_StyleAbbrevPictureCaption   = "cap";
        public const string c_StyleFootnote               = "ft";

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
        public const string c_StyleAnnotationMessage      = "NoteMessage";
        public const string c_StyleNote                   = "nt";

        public const string c_StyleToolTipText            = "ToolTipText";
        public const string c_StyleToolTipHeader          = "ToolTipHeader";

        public const string c_StyleMessageContent         = "MessageContent";
        public const string c_StyleMessageHeader          = "MessageHeader";

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


		}
		#endregion

		#region Method: void Initialize(bool bClearOutPrevious)
		public void Initialize(bool bClearOutPrevious)
		{


			// Initialize the various parts of the stylesheet
			_InitializeWritingSystems();

		}
		#endregion


        #region SAttr{g}: Font LargeDialogFont
        public static Font LargeDialogFont
        // This font is used for examining raw oxes files. I use a slightly larger
        // font due to the possible presence of diacritics which can otherwise be
        // difficult to read.
        {
            get
            {
                if (null == s_LargeDialogFont)
                {
                    s_LargeDialogFont = new Font(SystemFonts.DialogFont.FontFamily,
                        SystemFonts.DialogFont.Size * 1.2F,
                        FontStyle.Regular);
                }
                return s_LargeDialogFont;
            }
        }
        private static Font s_LargeDialogFont;
        #endregion

	}
    */
	#endregion

}
