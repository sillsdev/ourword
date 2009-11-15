/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Collaboration.cs
 * Author:  John Wimbish
 * Created: 5 March 2009
 * Purpose: Sets up the collaboration (chorus/mercurial) options
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using OurWordData.DataModel;
using OurWord.Edit;
#endregion

namespace OurWord.Dialogs
{
    public partial class Page_Collaboration : DlgPropertySheet
    {
        // Literate Settings -----------------------------------------------------------------
        #region Attr{g}: LiterateSettingsWnd LS
        LiterateSettingsWnd LS
        {
            get
            {
                return Collaboration;
            }
        }
        #endregion
        EditTextSetting m_RemoteServerUrl;
        EditTextSetting m_RemoteServerUserName;
        EditTextSetting m_RemoteServerPassword;
        #region Method: BuildLiterateSettings()
        void BuildLiterateSettings()
        {
            var internetRepository = DB.TeamSettings.GetInternetRepository();

            // Make sure the styles have been defined
            Information.InitStyleSheet();

            // Introduction
            LS.AddInformation("co100", Information.PStyleHeading1,
                "A. Introduction");
            LS.AddInformation("co110", Information.PStyleNormal,
                "A typical Bible Translation project has several different people, each with " +
                "his own computer, working on it. Most projects have as a minimum a translator, " +
                "and an advisor or consultant. Some projects have multiple translators and " +
                "multiple consultants. It becomes a tremendous challenge to keep the data on " +
                "all of these computers in synch, especially when these computers are not in " +
                "the same geographic location.");
            LS.AddInformation("co120", Information.PStyleNormal,
                "OurWord provides a Collaboration Feature as a means of solving this problem. " +
                "Internally we make use of software called \"Mercurial,\" which was specifically " +
                "designed for collaboration across multiple computers. You do not need to know " +
                "how to use Mercurial, we do that for you.");
            LS.AddInformation("co130", Information.PStyleNormal,
                "The Helps provide an much more in-depth discussion of Collaboration. There " +
                "you can learn how files are stored on the disk, how merging is done when " +
                "users modify the same paragraph, how the system keeps track of which changes " +
                "are current, and how the system remembers the history of every change anyone " +
                "has ever made. Some of these concepts can be challenging to comprehend, but " +
                "if you are part of a more complex cluster, then this is information you'll " +
                "probably want to know.");

            // Need to install mercurial
            if (!Repository.CheckMercialIsInstalled())
            {
                LS.AddInformation("co200", Information.PStyleAttention,
                    "Note: OurWord has detected that Mercurial is not installed on this computer. " +
                    "Unfortunately, you will need to do this yourself. You should exit OurWord, " +
                    "perform the install, and then come back to this page. This paragraph should " +
                    "then no longer be visible. You can find Mercurial at:");
                LS.AddInformation("co210", Information.PStyleList1,
                    "Linux - http://www.selenic.com/mercurial/wiki/index.cgi/BinaryPackages");
                LS.AddInformation("co220", Information.PStyleList1,
                    "Windows - http://sourceforge.net/project/showfiles.php?group_id=199155");
            }

            // Central Repository
            LS.AddInformation("co400", Information.PStyleHeading1,
                "C. Central Repository");
            LS.AddInformation("co410", Information.PStyleNormal,
                "Once Collaboration is turned on, a Repository will be created on your computer. " +
                "This is a special folder that contains a copy of the data, plus a history of " +
                "changes. Your working files still exist; the Repository is something in addition " +
                "to them.");
            LS.AddInformation("co420", Information.PStyleNormal,
                "In this initial Collaboration implementation, OurWord requires there to be a " +
                "central Repository that all project computers can interact with. Typically " +
                "this resides on a computer that project computers can access across the " +
                "Internet at any time. Any time a computer is logged into the Internet, it can " +
                "send its changes, and receive changes other project members have made. ");
            LS.AddInformation("co430", Information.PStyleNormal,
                "If you are involved in a Seed Company or SIL related project, use the contact " +
                "page if you want to request that a central repository be set up. Otherwise " +
                "Google on \"Mercurial hosting\" to find a host.");
            m_RemoteServerUrl = LS.AddEditText("coUrl",
                "Central Repository URL:",
                "The Url for accessing the remote, central respository.)",
                internetRepository.Server);
            m_RemoteServerUserName = LS.AddEditText("coUserName",
                "User Name:",
                "The central repositiory administrator will provide you with your user name.",
                internetRepository.UserName);
            m_RemoteServerPassword = LS.AddEditText("coPassword",
                "Password",
                "The central repositiory administrator will provide you with your password.",
                internetRepository.Password);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_Collaboration(DialogProperties parentDlg)
            : base(parentDlg)
        {
            InitializeComponent();
            BuildLiterateSettings();
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region OAttr{g}: string ID
        public override string ID
        {
            get
            {
                return "idCollaboration";
            }
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            //HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Collaboration";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            var internetRepository = DB.TeamSettings.GetInternetRepository();

            internetRepository.Server = m_RemoteServerUrl.Value;
            internetRepository.UserName = m_RemoteServerUserName.Value;
            internetRepository.Password = m_RemoteServerPassword.Value;
            return true;
        }
        #endregion
    }
}
