using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JWTools;

namespace OurWordData.DataModel.Membership
{
    static public class Users
    {
        // List of users ---------------------------------------------------------------------
        #region SAttr{g}: List<User> Members
        static public List<User> Members
        {
            get 
            { 
                return s_vMembers ?? (s_vMembers = new List<User>()); 
            }
        }
        static private List<User> s_vMembers;
        #endregion
        #region Attr{g}: bool HasAdministrator
        static public bool HasAdministrator
        {
            get
            {
                foreach(var user in Members)
                {
                    if (user.IsAdministrator)
                        return true;
                }
                return false;
            }
        }
        #endregion
        #region SMethod: User Find(sUserName)
        public static User Find(string sUserName)
        {
            foreach(var user in Members)
            {
                if (user.UserName.ToLowerInvariant() == sUserName.ToLowerInvariant())
                    return user;
            }
            return null;
        }
        #endregion
        #region SMethod: void Add(User)
        public static void Add(User user)
        {
            if (null != Find(user.UserName))
                return;

            Members.Add(user);
        }
        #endregion
        #region SMethod: void Delete(User user)
        public static void Delete(User user)
        {
            Members.Remove(user);

            // The Save command automatically removes files for users we've deleted
            Save();
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        private static DTeamSettings s_TeamSettings;
        #region Method: void Save()
        static public void Save()
        {
            var sFolder = s_TeamSettings.UsersFolder;

            if (!Directory.Exists(sFolder))
                Directory.CreateDirectory(sFolder);

            // Clear out any existing files for which we no longer has corresponding users
            var vsUserFiles = Directory.GetFiles(sFolder, User.c_sFileMask);
            foreach(var sPath in vsUserFiles)
            {
                var sName = Path.GetFileNameWithoutExtension(sPath);

                var user = Find(sName);
                if (null == user)
                    File.Delete(sPath);
            }

            // Save all users
            foreach(var user in Members)
                user.Save(sFolder);
        }
        #endregion
        #region Method: void Read(DTeamSettings)
        static public void Read(DTeamSettings team)
        {
            s_TeamSettings = team;

            Members.Clear();

            var vsUserPaths = Directory.GetFiles(s_TeamSettings.UsersFolder, User.c_sFileMask);
            foreach(var sUserPath in vsUserPaths)
            {
                var user = User.Create(sUserPath);
                if (null != user)
                    Members.Add(user);              
            }

            SetInitialValueOfCurrent();
        }
        #endregion

        // Built-In --------------------------------------------------------------------------
        #region Observer
        public static readonly User Observer = new User
        {
            UserName = "Observer",
            Type = User.UserType.Observer,

            MaximizeWindowOnStartup = false,

            CanNavigateFirstLast = true,
            CanDoBackTranslation = true,
            CanDoNaturalnessCheck = true,
            CanZoom = true
        };
        #endregion
        #region Translator
        public static readonly User Translator = new User()
        {
            UserName = "Translator",
            Type = User.UserType.Translator,
            MaximizeWindowOnStartup = true,           
            CanDoNaturalnessCheck = true,
            CanZoom = true,
            CanPrint = true,
            CanMakeNotes = true
        };
        #endregion
        #region Consultant
        public static readonly User Consultant = new User()
        {
            UserName = "Consultant",
            Type = User.UserType.Consultant,

            MaximizeWindowOnStartup = false,           

            CanNavigateFirstLast = true,
            CanDoNaturalnessCheck = true,
            CanDoConsultantPreparation = true,
            CanZoom = true,
            CanExportProject = true,
            CanPrint = true,
            CanFilter = true,
            ShowExpandedNotesIcon = true,
            CanMakeNotes = true,
            CanAssignNoteToConsultant = true
        };
        #endregion
        #region Administrator
        public static readonly User Administrator = new User()
        {
            UserName = "Administrator",
            Type = User.UserType.Administrator,

            MaximizeWindowOnStartup = false,           

            CanEditStructure = true,
            CanUndoRedo = true,
            CanNavigateFirstLast = true,
            CanDoBackTranslation = true,
            CanDoNaturalnessCheck = true,
            CanDoConsultantPreparation = true,
            CanZoom = true,
            CanCreateProject = true,
            CanOpenProject = true,
            CanExportProject = true,
            CanPrint = true,
            CanFilter = true,
            CanLocalize = true,
            CanRestoreBackups = true,

            CanMakeNotes = true,
            CloseNotesWindowWhenMouseLeaves = false,
            ShowExpandedNotesIcon = true,
            CanDeleteNotesAuthoredByOthers = true,
            CanAuthorHintForDaughterNotes = true,
            CanAuthorInformationNotes = true,
            CanAssignNoteToConsultant = true,
            CanCreateFrontTranslationNotes = true
        };
        #endregion

        // Current User ----------------------------------------------------------------------
        private const string c_sRegKey = "User";
        #region SAttr{g}: User Current
        public static User Current
        {
            get
            {
                return s_Current;
            }
            set
            {
                s_Current = value;
                JW_Registry.SetValue(c_sRegKey, s_TeamSettings.DisplayName, s_Current.UserName);
            }
        }
        private static User s_Current = Observer;
        #endregion
        #region Method: void SetInitialValueOfCurrent()
        static private void SetInitialValueOfCurrent()
        {
            // Attempt to set up to whoever was last (which is in the registry)
            var sUserName = JW_Registry.GetValue(c_sRegKey, s_TeamSettings.DisplayName, "");
            if (!string.IsNullOrEmpty(sUserName))
            {
                var user = Find(sUserName);
                if (null != user)
                {
                    Current = user;
                    return;
                }
            }

            // Last ditch: Set to least-accessible possibility
            Current = Observer;
        }
        #endregion
    }
}
