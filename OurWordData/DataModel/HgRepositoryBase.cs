#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using JWTools;
using Chorus.Utilities;
using Chorus.VcsDrivers.Mercurial;
#endregion

namespace OurWordData.DataModel
{
    #region Class: HgRepositoryBase
    public class HgRepositoryBase
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: int SecondsBeforeTimeout
        protected virtual int SecondsBeforeTimeout
        {
            get
            {
                // ToDo: What, if anything, do we want to do with this?
                // 3 minutes
                return 30*60; 
            }
        }
        #endregion
        #region VAttr{g}: string FullPathToRepositoryRoot
        public virtual string FullPathToRepositoryRoot
        {
            get
            {
                Debug.Assert(false, "Subclass must implement");
                return null;
            }
        }
        #endregion
        #region VAttr{g}: bool IsRemoteOnInternet
        public bool IsRemoteOnInternet
        {
            get
            {
                return FullPathToRepositoryRoot.Contains("http:");
            }
        }
        #endregion
        #region SVAttr{g}: string PathToChorusMerge

        static public string PathToChorusMerge
        {
            get
            {
                return SurroundWithQuotes(
                    Path.Combine(Other.DirectoryOfExecutingAssembly, "ChorusMerge.exe"));
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected HgRepositoryBase()
        {
        }
        #endregion
        #region SMethod: string SurroundWithQuotes(string s)
        protected static string SurroundWithQuotes(string sIn)
        {
            // We need to remove any existing quotes we might have, thus this can be called
            // on paths that already are surrounded by quotes.
            var sOut = "";
            foreach (var ch in sIn)
            {
                if (ch != '\"')
                    sOut += ch;
            }

            // Now place quotes at beginning and end
            return ("\"" + sOut + "\"");
        }
        #endregion
        #region static string StripTrailingPathSeparator(sPath)
        static public string StripTrailingPathSeparator(string sPath)
        // Some (all?) Mercurial commands fail if there is a trailing path separator
        {
            if (string.IsNullOrEmpty(sPath))
                return sPath;

            // Check not only for the generic DirectorySeparatorChar, but also for stuff
            // the user might have hardcoded irregardless of which OS was running.
            var ch = sPath[sPath.Length - 1];
            if (ch == Path.DirectorySeparatorChar || ch == '\\' || ch == '/')
                sPath = sPath.Substring(0, sPath.Length - 1);

            return sPath;
        }
        #endregion

        // Heads ----------------------------------------------------------------------------
        #region CLASS: Head
        public class Head
        {
            public readonly string ChangeSet;
            public readonly string Tag;
            public readonly string User;
            public readonly string Date;
            public readonly string Summary;

            #region Constructor(sChangeSet, sTag, sUser, sDate, sSummary)
            public Head(string sChangeSet, string sTag, string sUser, string sDate, string sSummary)
            {
                ChangeSet = sChangeSet;
                Tag = sTag;
                User = sUser;
                Date = sDate;
                Summary = sSummary;
            }
            #endregion
        }
        #endregion
        #region Method: List<Head> GetHeads()
        public List<Head> GetHeads()
        // 21sep09 - I was originally doing the v.Add as part of encountering a
        // "summary", but on a brand-spanking-new repository, there is no summary;
        // so instead moved it to the "changeset"; thus called there, and also at the
        // end of the loop.
        {
            var v = new List<Head>();

            // Request the information from Mercurial
            var result = DoCommand("heads");
            if (null == result)
                return null;

            var vsLines = result.StandardOutput.Split('\n');

            var sChangeSet = "";
            var sTag = "";
            var sUser = "";
            var sDate = "";
            var sSummary = "";

            const int iPosData = 14;

            foreach (var s in vsLines)
            {
                if (s.StartsWith("changeset:") && s.Length > iPosData)
                {
                    if (!string.IsNullOrEmpty(sChangeSet))
                        v.Add(new Head(sChangeSet, sTag, sUser, sDate, sSummary));

                    sChangeSet = s.Substring(iPosData);
                    sTag = "";
                    sUser = "";
                    sDate = "";
                    sSummary = "";
                }

                if (s.StartsWith("tag:") && s.Length > iPosData)
                    sTag = s.Substring(iPosData);
                if (s.StartsWith("user:") && s.Length > iPosData)
                    sUser = s.Substring(iPosData);
                if (s.StartsWith("date:") && s.Length > iPosData)
                    sDate = s.Substring(iPosData);
                if (s.StartsWith("summary:") && s.Length > iPosData)
                    sSummary = s.Substring(iPosData);
            }

            if (!string.IsNullOrEmpty(sChangeSet))
                v.Add(new Head(sChangeSet, sTag, sUser, sDate, sSummary));

            return v;
        }
        #endregion

        // Operations ------------------------------------------------------------------------
        #region Method: ExecutionResult DoCommand(commandLine)
        public ExecutionResult DoCommand(string commandLine)
        {
            var result = HgRunner.Run(commandLine,
                FullPathToRepositoryRoot,
                SecondsBeforeTimeout, 
                new Chorus.Utilities.NullProgress());
            return result;
        }
        #endregion
        #region Method: ExecutionResult CloneTo(destinationPath)
        public ExecutionResult CloneTo(string destinationPath)
        {
            if (string.IsNullOrEmpty(destinationPath)) 
                throw new ArgumentException("destinationPath");

            if (Directory.Exists(destinationPath))
                Directory.Delete(destinationPath, true);

            var source = SurroundWithQuotes(FullPathToRepositoryRoot);
            var destination = SurroundWithQuotes(StripTrailingPathSeparator(destinationPath));

            var cloneCommand = string.Format("clone {0} {1}", source, destination);
            return DoCommand(cloneCommand);
        }
        #endregion
        #region Method: ExecutionResult CommitChangedFiles(user, message)
        public ExecutionResult CommitChangedFiles(string user, string message)
        {
            if (string.IsNullOrEmpty(user)) throw new ArgumentException("user");
            if (string.IsNullOrEmpty(message)) throw new ArgumentException("message");

            if (GetChangedFiles().Count == 0)
                return new ExecutionResult {ExitCode = 0, StandardError = "", StandardOutput = ""};

            var commitCommand = string.Format("commit -A -u \"{0}\" -m \"{1}\"", user, message);
            return DoCommand(commitCommand);
        }
        #endregion
        #region Method: ExecutionResult CommitRepairOfUnresolvedMerge(username)
        ExecutionResult CommitRepairOfUnresolvedMerge(string username)
        {
            if (string.IsNullOrEmpty(username)) 
                throw new ArgumentException("user");

            var commitCommand = string.Format("commit -u \"{0}\" -m \"Repaired an unresolved merge.\"", 
                username);
            return DoCommand(commitCommand);
        }
        #endregion
        #region Method: ExecutionResult CommitResultsOfMerge(username)
        public ExecutionResult CommitResultsOfMerge(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("user");

            var now = DateTime.UtcNow.ToString("u", DateTimeFormatInfo.InvariantInfo);
            var message = string.Format("Merged by {0} {1}", username, now);
            var commitCommand = string.Format("commit -u \"{0}\" -m \"{1}\"",
                username, message);
            return DoCommand(commitCommand);
        }
        #endregion
        #region Method: ExecutionResult Recover()
        public ExecutionResult Recover()
        {
            return DoCommand("Recover");
        }
        #endregion
        #region Method: bool CheckHasUnresolvedFiles()
        bool CheckHasUnresolvedFiles()
            // Any line in standard output that starts as "U " signals an unresolved
            // file, for which merge is needed.
            //
            // Returns true if there are unresolved files, false if none; thus false
            // indicates the repository is in good shape.
        {
            var result = DoCommand("resolve -l");
            var vsLines = result.StandardOutput.Split('\n');

            foreach (var sLine in vsLines)
            {
                if (sLine.Length > 2 && sLine[0] == 'U' && sLine[1] == ' ')
                    return true;
            }

            return false;
        }
        #endregion
        #region Method: bool ResolveFilesIfNeeded(string username)
        public bool ResolveFilesIfNeeded(string username)
            // Returns false if there are still unresolved files (that is, if the attempt
            // to merge any unresolved files failed.
        {
            if (!CheckHasUnresolvedFiles())
                return true;

            // Attempt to resolve them. Most likely this will fail, because unresolved 
            // files are generally due to a problem in the merge code; but if the user 
            // has installed a later version of OW, then perhaps the problem will have 
            // been fixed.
            using (new ShortTermEnvironmentalVariable("ChorusPathToRepository", FullPathToRepositoryRoot))
            using (new ShortTermEnvironmentalVariable("HGMERGE", PathToChorusMerge))
            {
                var result = DoCommand("resolve -a");
                if (result.ExitCode == 0) // Successful
                    CommitRepairOfUnresolvedMerge(username);
            }

            return !CheckHasUnresolvedFiles();
        }
        #endregion
        #region Method: List<string> GetChangedFiles()
        public List<string> GetChangedFiles()
        {
            var vsFiles = new List<string>();

            // Returns a list of the files that have changed
            var result = DoCommand("status");

            var vsLines = result.StandardOutput.Split('\n');

            foreach (var sLine in vsLines)
            {
                if (sLine.Trim().Length > 2)
                    vsFiles.Add(sLine.Substring(2)); //! data.txt
            }

            return vsFiles;
        }
        #endregion
        #region Method: ExecutionResult PushTo(sFullPathToTargetRepository)
        public ExecutionResult PushTo(string sFullPathToTargetRepository)
        {
            var path = SurroundWithQuotes(sFullPathToTargetRepository);
            var pushCommand = string.Format("push {0}", path);
            return DoCommand(pushCommand);
        }
        #endregion
    }
    #endregion

    #region Class: HgLocalRepository
    public class HgLocalRepository : HgRepositoryBase
    {
        // Attrs -----------------------------------------------------------------------------
        #region override string FullPathToRepositoryRoot
        public override string FullPathToRepositoryRoot
        {
            get
            {
                return m_RepositoryRootPath;
            }
        }
        #endregion
        readonly string m_RepositoryRootPath;
        #region VAttr{g}: string FullPathToHgFolder
        string FullPathToHgFolder
        {
            get
            {
                return FullPathToRepositoryRoot + Path.DirectorySeparatorChar + ".hg";
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(repositoryRootPath)
        public HgLocalRepository(string repositoryRootPath)
        {
            m_RepositoryRootPath = StripTrailingPathSeparator(repositoryRootPath);
        }
        #endregion

        // Operations ------------------------------------------------------------------------
        #region Attr{g}: bool Exists
        public bool Exists
        {
            get
            {
                var pathToRepositoryStorage = FullPathToRepositoryRoot + 
                    Path.DirectorySeparatorChar + ".hg";

                return Directory.Exists(pathToRepositoryStorage);
            }
        }
        #endregion
        #region Method: ExecutionResult CreateIfDoesntExist()
        public ExecutionResult CreateIfDoesntExist()
        {
            if (Exists)
                return null;

            // Make sure the root folder exists; in the case of some unit tests we may
            // not have created the folder yet.
            if (!Directory.Exists(FullPathToRepositoryRoot))
                Directory.CreateDirectory(FullPathToRepositoryRoot);

            const string createRepositoryCommand = "init";
            return DoCommand(createRepositoryCommand);
        }
        #endregion
        #region Method: bool CheckUnlockedRepository()
        public bool CheckUnlockedRepository()
            // Attempts to unlock the file if one is there.
            // Returns true if there is no locked file
        {
            var sLockFile = FullPathToHgFolder + Path.DirectorySeparatorChar + "wlock";

            if (File.Exists(sLockFile))
            {
                Thread.Sleep(5000);
                if (File.Exists(sLockFile))
                    File.Delete(sLockFile);
                if (File.Exists(sLockFile))
                    return false;
            }

            return true;
        }
        #endregion
        #region Method: bool CheckUnrecognizedAddFiles()
        public bool CheckUnrecognizedAddFiles()
        {
            var recognizedExtensions = new List<string> {".otrans", ".owp", ".owt", ".oxes"};

            // Get a list of the files that have changed
            var vsChangedFiles = GetChangedFiles();
            if (vsChangedFiles.Count == 0)
                return true;

            // Remove any that we don't recognize, so that they don't get added to the repo.
            // (If they were already in the repo, this will resort in their being removed.)
            foreach (var sPartialFilePath in vsChangedFiles)
            {
                var sPath = FullPathToRepositoryRoot + Path.DirectorySeparatorChar + sPartialFilePath;

                var sExtension = Path.GetExtension(sPath).ToLower();
                var bExtensionFound = recognizedExtensions.Contains(sExtension);
                if (bExtensionFound)
                    continue;

                File.Delete(sPath);
                if (File.Exists(sPath))
                    return false;
            }

            return true;
        }
        #endregion
        #region Method: ExecutionResult PullFrom(sFullPathToRepositoryRoot)
        public ExecutionResult PullFrom(string sFullPathToRepositoryRoot)
        {
            if (string.IsNullOrEmpty(sFullPathToRepositoryRoot)) 
                throw new ArgumentException("sFullPathToRepositoryRoot");

            var path = SurroundWithQuotes(sFullPathToRepositoryRoot);
            var pullCommand = string.Format("pull {0}", path);
            return DoCommand(pullCommand);
        }
        #endregion
    }
    #endregion

    #region Class: HgInternetRepository
    public class HgInternetRepository : HgRepositoryBase
    {
        // Attrs
        #region OAttr{g}: string FullPathToRepositoryRoot
        public override string FullPathToRepositoryRoot
        {
            get
            {
                var path = string.Format("http://{0}:{1}@{2}/{3}",
                    UserName, Password, Server, s_ClusterName.ToLower());
                return path;
            }
        }
        #endregion
        #region OAttr{g}: int SecondsBeforeTimeout
        protected override int SecondsBeforeTimeout
        {
            get
            {
                // 30 minutes
                return 30*60;
            }
        }
        #endregion

        // Username/password are stored in the Registry --------------------------------------
        #region SVAttr{g}: string RegistryClusterSubKey
        static string RegistryClusterSubKey
        {
            get
            {
                return string.Format("Collaboration\\{0}", s_ClusterName);
            }
        }
        #endregion
        private const string c_registryUserName = "RemoteUserName";
        private const string c_registryPassword = "RemotePassword";
        private const string c_registryServer = "RemoteServer";
        #region SAttr{g/s}: string UserName
        static public string UserName
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, c_registryUserName, "");
            }
            set
            {
                JW_Registry.SetValue(RegistryClusterSubKey, c_registryUserName, value);
            }
        }
        #endregion
        #region SAttr{g/s}: string Password
        static public string Password
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, c_registryPassword, "");
            }
            set
            {
                JW_Registry.SetValue(RegistryClusterSubKey, c_registryPassword, value);
            }
        }
        #endregion
        #region SAttr{g/s}: string Server
        static public string Server
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, c_registryServer, 
                    "hg-public.languagedepot.org");
            }
            set
            {
                var sServerNoHttp = StripLeadingHttp(value);
                var sServerNoSlash = StripTrailingSlash(sServerNoHttp);

                JW_Registry.SetValue(RegistryClusterSubKey, c_registryServer, sServerNoSlash);
            }
        }
        #endregion
        #region SMethod: string StripLeadingHttp(sUrl)
        static private string StripLeadingHttp(string sUrl)
        // In case user typed this, we remove it
        {
            if (string.IsNullOrEmpty(sUrl))
                return sUrl;

            return sUrl.Replace("http://", "");
        }
        #endregion
        #region SMethod: string StripTrailingSlash(string sUrl)
        static private string StripTrailingSlash(string sUrl)
        {
            if (string.IsNullOrEmpty(sUrl))
                return sUrl;

            var lastChar = sUrl[sUrl.Length - 1];
            if (lastChar != '\\' && lastChar != '/')
                return sUrl;

            return sUrl.Substring(0, sUrl.Length - 1);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        static private string s_ClusterName;
        #region Constructor(clusterDisplayName)
        public HgInternetRepository(string clusterDisplayName)
        {
            if (string.IsNullOrEmpty(clusterDisplayName)) 
                throw new ArgumentNullException("clusterDisplayName");

            s_ClusterName = clusterDisplayName;
        }
        #endregion
    }
    #endregion

    #region Class: Synchronize
    public class Synchronize
    {
        private readonly HgLocalRepository m_LocalRepository;
        private readonly HgRepositoryBase m_OtherRepository;
        private readonly string m_UserNameForCommits;

        // Helper Methods --------------------------------------------------------------------
        #region Method: bool CanPingGoogle()
        static bool CanPingGoogle()
            // We'll just ping on Google, since they're up most of the time.
        {
            try
            {
                var ping = new Ping();
                var reply = ping.Send("www.google.com");
                if (null == reply)
                    return false;
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
        #endregion
        #region Class: SynchException : Exception
        class SynchException : Exception
        {
            public readonly string LocalizedMessage;

            public SynchException(string sLocId, string sEnglishMessage)
                : base(sEnglishMessage)
            {
                LocalizedMessage = LocDB.GetValue(
                    new[] { "Strings", "SynchMessages" },
                    sLocId,
                    sEnglishMessage,
                    null,
                    null);
            }
        }
        #endregion

        // SynchSteps ------------------------------------------------------------------------
        #region Class: SynchStep
        class SynchStep
        {
            public readonly string StepName;

            public delegate void Action();
            public readonly Action SynchAction;

            public SynchStep(string sStepName, Action synchAction)
            {
                StepName = sStepName;
                SynchAction = synchAction;
            }
        }
        #endregion
        private readonly List<SynchStep> m_SynchSteps;
        #region Method: void AddSynchStep(sStepName, Action synchAction)
        void AddSynchStep(string sStepName, SynchStep.Action synchAction)
        {
            Debug.Assert(null != m_SynchSteps);
            Debug.Assert(!string.IsNullOrEmpty(sStepName));
            Debug.Assert(null != synchAction);

            var step = new SynchStep(sStepName, synchAction);
            m_SynchSteps.Add(step);
        }
        #endregion
        #region Method: string[] GetSynchStepNames()
        string[] GetSynchStepNames()
        {
            var v = new List<string>();
            foreach(var step in m_SynchSteps)
                v.Add(step.StepName);
            return v.ToArray();
        }
        #endregion

        // Sync Actions / Steps --------------------------------------------------------------
        #region CheckInternetAccess
        static void CheckInternetAccess()
        {
            if (CanPingGoogle())
                return;

            throw new SynchException("msgNoInternetConnection",
                "OurWord is unable to connect to the Internet.\n\n" +
                "Please check that you have an active Internet connection, then try again.");
        }
        #endregion
        #region CheckLocalIntegrity
        void CheckLocalIntegrity()
        {
            // Is Mercurial installed? GetHeads will return non-zero.
            if (m_LocalRepository.GetHeads().Count == 0)
            {
                throw new SynchException("msgNoMercurial",
                    "The external program Mercurial did not respond.\n\n" +
                    "Either it is not installed (most likely), or else you have a corrupt repository.");
            }

            // Attempt to unlock the repository if it is locked
            if (!m_LocalRepository.CheckUnlockedRepository())
            {
                throw new SynchException("msgRepositoryLocked",
                    "The Repository is in use by another process. Please wait, then try again.\n\n" +
                    "If this message continues to appear, try restarting your computer.");
            }

            // If we had an interrupted transaction, then we need to recover from it. This
            // command has  no effect if the repositiory is in good shape; but it repairs the
            // repositiory if a transaction was interrupted.
            m_LocalRepository.Recover();

            // Do we have an failed merge, e.g., leftover from a previous version of OurWord?
            if (!m_LocalRepository.ResolveFilesIfNeeded(m_UserNameForCommits))
            {
                throw new SynchException("msgRepositoryProblem",
                    "We're sorry, but there is apparently a bug in OurWord related to merging files.\n\n" +
                    "If you have upgraded to the latest version, then please contact us at " +
                    "http://ourword.TheSeedCompany.org for information on how to solve " +
                    "this problem.");
            }
        }
        #endregion
        #region StoreChangedFiles
        void StoreChangedFiles()
        {
            if (!m_LocalRepository.CheckUnrecognizedAddFiles())
            {
                throw new SynchException("msgCantDeleteFileBeforeCommit",
                    "A file with an unrecognized extension is in your data folder{n}{n}" +
                    "OurWord was uable to delete it. Perhaps you ave some other software " +
                    "running which is using that file. Please delete the file, then try " +
                    "again. OurWord only recognizes files of type \".owp\", \".owt\", " +
                    "\".otrans\" and \".oxes\".");
            }

            var now = DateTime.UtcNow.ToString("u", DateTimeFormatInfo.InvariantInfo);
            var message = string.Format("Committing changes from {0} {1}", m_UserNameForCommits, now);
            var result = m_LocalRepository.CommitChangedFiles(m_UserNameForCommits, message);
            if (result.ExitCode != 0)
            {
                throw new SynchException("msgCantCommitRecentChanges",
                     "OurWord was unable to place your most recent changes into the local Repository.\n\n" +
                     "Please try again. If the problem continues, please contact us at " +
                     "http://ourword.TheSeedCompany.org so that we can determine how to " +
                     "solve this problem.");
            }
                    }
        #endregion
        #region PullNewerFiles
        void PullNewerFiles()
        {
            var result = m_LocalRepository.PullFrom(m_OtherRepository.FullPathToRepositoryRoot);
            if (0 != result.ExitCode)
            {
                throw new SynchException("msgUnableToPull",
                    "OurWord is unable to retrieve changes from the Internet. \n\n" +
                    "The remote computer may not be working, or you may have the username " +
                    "or password incorrectly entered. Please try again later.");
            }
        }
        #endregion
        #region Merge
        void Merge()
            // Hg's Merge gives strange error messages. E.g., its an error if there was
            // nothing to merge. So we're ignoring the error message returned from 
            // the Execute command.
            // 
            // TODO: Figure out if the Merge was unsuccessful and display an error message
        {
            using (new ShortTermEnvironmentalVariable("ChorusPathToRepository", m_LocalRepository.FullPathToRepositoryRoot))
            using (new ShortTermEnvironmentalVariable("HGMERGE", HgRepositoryBase.PathToChorusMerge))
            {
                m_LocalRepository.DoCommand("merge");
            }
        }
        #endregion
        #region StoreMergeResults
        void StoreMergeResults()
        {
            var result = m_LocalRepository.CommitResultsOfMerge(m_UserNameForCommits);
            if (0 != result.ExitCode)
            {
                throw new SynchException("msgUnableToCommitMerge",
                    "OurWord was unable to store the results of the merge. \n\n" +
                    "This is an odd thing to happen. If it continues, please contact us at http://ourword.TheSeedCompany.org so that we can work with you to solve the problem.");
            }
        }
        #endregion
        #region PushToOther
        void PushToOther()
        {
            var result = m_LocalRepository.PushTo(m_OtherRepository.FullPathToRepositoryRoot);
            if (0 != result.ExitCode)
            {
                if (result.StandardOutput.Contains("abort: push creates new remote heads"))
                    throw new SynchException("msgNewRemoteHeads",
                        "Unable to save changes, because merge did not suceed (with the " +
                        "ccmplaint of having created new \"heads\") If this continues, " +
                        "please report it as a bug.");

                throw new SynchException("msgPushFailed",
                   "Sending changes back to the Internet failed, perhaps due to a bad " +
                   "Internet connection. Please try again.");
            }
        }
        #endregion
        #region UpdateLocalFiles
        void UpdateLocalFiles()
        {
            m_LocalRepository.DoCommand("update");
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Constructor(local, other, sUserNameForCommits)
        public Synchronize(HgLocalRepository local, HgRepositoryBase other, string sUserNameForCommits)
        {
            Debug.Assert(null != local);
            Debug.Assert(null != other);
            Debug.Assert(!string.IsNullOrEmpty(sUserNameForCommits));

            m_LocalRepository = local;
            m_OtherRepository = other;
            m_UserNameForCommits = sUserNameForCommits;

            // Setup the steps/actions that the synch will require
            m_SynchSteps = new List<SynchStep>();
            var sOtherName = (other.IsRemoteOnInternet) ? "Internet" : "other repository";
//            if (other.IsRemoteOnInternet)
//                AddSynchStep("Checking Internet access", CheckInternetAccess);
            AddSynchStep("Checking data integrity", CheckLocalIntegrity);
            AddSynchStep("Storing any files you've changed", StoreChangedFiles);
            AddSynchStep("Retrieving any newer files from the " + sOtherName, PullNewerFiles);
            AddSynchStep("Merging your changes with theirs", Merge);
            AddSynchStep("Storing the results of the merge", StoreMergeResults);
            AddSynchStep("Sending all changes back to the " + sOtherName, PushToOther);
            AddSynchStep("Updating your data with the changed", UpdateLocalFiles);
        }
        #endregion
        #region Method: bool Do()
        public bool Do()
            // Returns true if successful
        {
            EnumeratedStepsProgressDlg.Start("Performing Send/Receive...", GetSynchStepNames());

            try
            {
                foreach (var step in m_SynchSteps)
                {
                    EnumeratedStepsProgressDlg.IncrementStep();
                    step.SynchAction();
                }

                // The progress dialog is closed if we're successful; but stays open
                // on an error in order to be explicitly dismissed by the user.
                EnumeratedStepsProgressDlg.Stop();
                return true;
            }
            catch (SynchException e)
            {
                EnumeratedStepsProgressDlg.Fail(e.LocalizedMessage);
                return false;
            }
        }
        #endregion
    }
    #endregion
}
