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
using OurWordData.DataModel.Membership;
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
        int m_nSettingsVersion;
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

        #region bool CheckForUpdatesBeforeSynchronize
	    public bool CheckForUpdatesBeforeSynchronize
	    {
	        get
	        {
	            return m_bCheckForUpdatesBeforeSynchronize;
	        }
            set
            {
                m_bCheckForUpdatesBeforeSynchronize = value;
            }
	    }
        private bool m_bCheckForUpdatesBeforeSynchronize;
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

            DefineAttr("Updates", ref m_bCheckForUpdatesBeforeSynchronize);
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

        // Repository Factories --------------------------------------------------------------
        #region Method: InternetRepository GetInternetRepository()
        public InternetRepository GetInternetRepository()
        {
            return new InternetRepository(DisplayName, Users.Current.CollaborationUserName,
                Users.Current.CollaborationPassword);
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
        public static void EnsureInitialized()
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
            // Version Values
            // 1 - Initial
            // 2 - Got rid of BookGroupings as a user-modifyable property
			const int nCurrentSettingsVersion = 2;

			if (SettingsVersion < nCurrentSettingsVersion)
			{
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
                var sClusterFolder = ClusterFolder;
                if (string.IsNullOrEmpty(sClusterFolder))
                    return null;

				// Make sure the folder exists
				var sFolder = ClusterFolder + SettingsFolderName + Path.DirectorySeparatorChar;
                if (!Directory.Exists(sFolder))
                {
                    if (sFolder.Contains("OurWordData\\.Settings"))
                        Debug.Fail("Attempt to create OurWordData\\.Settings; need a call stack to debug (DTeamSettings.SettingsFolder)");
                    Directory.CreateDirectory(sFolder);
                }

			    return sFolder;
			}
		}
		#endregion
		#region Attr{g}: string BackupFolder - Top-level, e.g., "MyDocuments\OurWord\.Backup"
		public string BackupFolder
		{
			get
			{
				// Make sure the folder exists
				var sFolder = ClusterFolder + BackupFolderName + Path.DirectorySeparatorChar;
				if (!Directory.Exists(sFolder))
					Directory.CreateDirectory(sFolder);

				return sFolder;
			}
		}
		#endregion
        #region Attr{g}: string PicturesFolder - Top-level, e.g., "MyDocuments\OurWord\.Pictures"
        public string PicturesFolder
        {
            get
            {
                var sFolder = ClusterFolder + ".Pictures" + Path.DirectorySeparatorChar;
                if (!Directory.Exists(sFolder))
                    Directory.CreateDirectory(sFolder);
                return sFolder;
            }
        }
        #endregion
        #region Attr{g}: string UsersFolder - Top-level, e.g., "MyDocuments\OurWord\.Users"
        public string UsersFolder
        {
            get
            {
                var sFolder = ClusterFolder + ".Users" + Path.DirectorySeparatorChar;
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

}
