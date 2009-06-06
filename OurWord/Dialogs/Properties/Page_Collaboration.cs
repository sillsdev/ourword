/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Collaboration.cs
 * Author:  John Wimbish
 * Created: 5 March 2009
 * Purpose: Sets up the collaboration (chorus/mercurial) options
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb;
using JWdb.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
using OurWord.Utilities;
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
        YesNoSetting m_Activate;
        EditTextSetting m_RemoteServerUrl;
        EditTextSetting m_RemoteServerUserName;
        EditTextSetting m_RemoteServerPassword;
        YesNoSetting m_MineAlwaysWins;
        #region Method: BuildLiterateSettings()
        void BuildLiterateSettings()
        {
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
            if (!Repository.HgIsInstalled)
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

            // Activate
            LS.AddInformation("co300", Information.PStyleHeading1,
                "B. Activation");
            LS.AddInformation("co310", Information.PStyleNormal,
                "By default Collaboration is not turned on. You need to do that here. This " +
                "setting will apply to every language and every user in the entire cluster.");
            m_Activate = LS.AddYesNo(
                "coActivate",
                "Activate Collaboration?",
                "Turn on the Mercurial Collaboration feature.",
                DB.TeamSettings.RepositoryIsActive);

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
                Repository.RemoteUrl);
            m_RemoteServerUserName = LS.AddEditText("coUserName",
                "User Name:",
                "The central repositiory administrator will provide you with your user name.",
                Repository.RemoteUserName);
            m_RemoteServerPassword = LS.AddEditText("coPassword",
                "Password",
                "The central repositiory administrator will provide you with your password.",
                Repository.RemotePassword);

            // Merging
            LS.AddInformation("co500", Information.PStyleHeading1,
                "D. Merging");
            LS.AddInformation("co510", Information.PStyleNormal,
                "When two (or more) people modify the same text on their individual computers, " +
                "a \"conflict\" results. When we merge their files, we must determine which " +
                "person's change to keep.");
            LS.AddInformation("co520", Information.PStyleNormal,
                "For the translation itself, we first assume that if a change has been made, " +
                "it has the right to be there. That is, the person making the change had the " +
                "authority to do so. If you want to prevent a user from making changes, you " +
                "need to lock that book from editing through its Properties dialog.");
            LS.AddInformation("co530", Information.PStyleNormal,
                "Thus Person A and Person B both are assumed to have permission to make " +
                "changes. Now if Person A and Person B make changes to different parts of " +
                "the draft, there is no conflict and both changes are kept. The problem " +
                "comes with A and B both change the exact same text. When that happens, " +
                "we have a \"conflict.\"");
            LS.AddInformation("co540", Information.PStyleNormal,
                "What we do in this case is not change the draft on the person's computer " +
                "who is doing the merge. Rather, we take the incoming conflicting text, " +
                "and make a Translator Note out of it. It thus appears in the Side Window, " +
                "where the user can study it and deal with it at his convenience.");
            LS.AddInformation("co550", Information.PStyleNormal,
                "Unfortunately we cannot make Translator Notes when dealing with settings. " +
                "If two users open this Configuration dialog and make conflicting changes, " +
                "OurWord is forced to keep one and toss the other. ");
            LS.AddInformation("co560", Information.PStyleNormal,
                "We assume that a typical project will have one or more Advisors who will " +
                "be the ones who generally make changes to settings; and then one or more " +
                "translators who generally will not be touching the settings. Therefore upon " +
                "encountering a merge conflict in a settings file, we want to keep those " +
                "from the Advisor, and toss those from the translator. To facilitate this, " +
                "you can tell OurWord that merges performed on this computer should keep " +
                "any conflicting settings. For translator computers you should keep the " +
                "default value of \"no.\"");
            m_MineAlwaysWins = LS.AddYesNo(
                "coMySettingsWin",
                "On Conflict, Keep My Settings?",
                "When others have edited the same settings, keep mine? Typically Yes for " +
                    "and advisor, No for a translator.",
                Repository.MineAlwaysWins);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_Collaboration(DialogProperties _ParentDlg)
            : base(_ParentDlg)
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
            // Strip any leading http from the value, as the Repository class 
            // needs to add this when it prepends the username and password
            string sRemoteUrl = m_RemoteServerUrl.Value;
            string sHttp = "http://";
            if (sRemoteUrl.Length > sHttp.Length &&
                sRemoteUrl.Substring(0, sHttp.Length) == sHttp)
            {
                sRemoteUrl = sRemoteUrl.Substring(sHttp.Length);
            }
            Repository.RemoteUrl = sRemoteUrl;

            // Store the other settings
            DB.TeamSettings.RepositoryIsActive = m_Activate.Value;
            Repository.RemoteUserName = m_RemoteServerUserName.Value;
            Repository.RemotePassword = m_RemoteServerPassword.Value;
            return true;
        }
        #endregion
    }
}
