#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Chorus.sync;
using Chorus.VcsDrivers;
using JWTools;
using Chorus.Utilities;
using Chorus.VcsDrivers.Mercurial;
#endregion

namespace OurWordData.Synchronize
{
    #region Class: Repository
    public class Repository
    {
        // Attrs -----------------------------------------------------------------------------
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected Repository()
        {
        }
        #endregion
        #region SMethod: string SurroundWithQuotes(string s)
        public static string SurroundWithQuotes(string sIn)
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
        protected static string StripTrailingPathSeparator(string sPath)
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

        // Operations ------------------------------------------------------------------------
        #region Method: ExecutionResult DoCommand(commandLine)
        public ExecutionResult DoCommand(string commandLine)
        {
            const int cSecondsBeforeTimeout = 60*60;

            var result = HgRunner.Run(commandLine,
                FullPathToRepositoryRoot,
                cSecondsBeforeTimeout, 
                new Chorus.Utilities.NullProgress());

            return result;
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
        public void Recover()
            // Has no effect if the repositiory is in good shape; but it repairs the
            // repositiory if a transaction was interrupted.
        {
            DoCommand("Recover");
        }
        #endregion
        #region Method: bool CheckHasUnresolvedFiles()
        public bool CheckHasUnresolvedFiles()
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
        #region SMethod: bool CheckMercialIsInstalled()
        static public bool CheckMercialIsInstalled()
        {
            // The Chorus routine calls "Hg version" and returns null if successful;
            // returns a string with a message otherwise.
            var notInstalledMessage = HgRepository.GetEnvironmentReadinessMessage("en");
            return (string.IsNullOrEmpty(notInstalledMessage));
        }
        #endregion
        #region Method: bool GetFileExistsInRepo(string pathFromRoot)
        public bool GetFileExistsInRepo(string pathFromRoot)
        {
            var repo = new HgRepository(FullPathToRepositoryRoot, 
                new Chorus.Utilities.NullProgress());
            return repo.GetFileExistsInRepo(pathFromRoot);
        }
        #endregion
    }
    #endregion

    #region Class: LocalRepository
    public class LocalRepository : Repository
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
        public LocalRepository(string repositoryRootPath)
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
        public void CreateIfDoesntExist()
        {
            if (Exists)
                return;

            // Make sure the root folder exists; in the case of some unit tests we may
            // not have created the folder yet.
            if (!Directory.Exists(FullPathToRepositoryRoot))
                Directory.CreateDirectory(FullPathToRepositoryRoot);

            // Initialize a repo here
            const string createRepositoryCommand = "init";
            DoCommand(createRepositoryCommand);

            // Files for Mercurial to ignore
            CreateHgIgnoreFile();

            // Commit the ignore file
            CommitChangedFiles("OurWord", "Initial setup (Added .hgignore)");

            // Version Number for this repository. Do this after the hgignore commit,
            // otherwise it will not show up in the tags command.
            AddOurWordVersionTag();
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
        #region void UpdateToCurrentBranchTip()
        public void UpdateToCurrentBranchTip()
        {
            DoCommand("update");
        }
        #endregion
        #region Method: ExecutionResult CloneFrom(Repository other)
        public ExecutionResult CloneFrom(Repository other)
        {
            // Get rid of anything that might have been there; then creat an empty
            // folder so that Hg will not complain of an invalid working directory
            if (Directory.Exists(FullPathToRepositoryRoot))
                JWU.SafeFolderDelete(FullPathToRepositoryRoot);
            Directory.CreateDirectory(FullPathToRepositoryRoot);

            var destination = SurroundWithQuotes(FullPathToRepositoryRoot);

            var source = SurroundWithQuotes(StripTrailingPathSeparator(
                other.FullPathToRepositoryRoot));

            var cloneCommand = string.Format("clone {0} {1}", source, destination);
            return DoCommand(cloneCommand);
        }
        #endregion
        #region Method: void Rollback()
        public void Rollback()
        {
            DoCommand("rollback");
        }
        #endregion

        // Repository Version ----------------------------------------------------------------
        private const string c_VersionTag = "OurWordVersion";
        public const int c_CurrentVersionNo = 2;
        #region SAttr{g}: string TagContents
        public static string TagContents
        {
            get
            {
                return c_VersionTag + "=" + c_CurrentVersionNo;
            }
        }
        #endregion
        #region Method: int GetOurWordVersion()
        public int GetOurWordVersion()
            // Tags are of the form:
            // tip                                1:a545ad8f51c7
            // OurWordVersion=14                  0:f214fd2969a7
        {
            var result = DoCommand("tags");

            var vsTags = result.StandardOutput.Split('\n');
            var iStart = c_VersionTag.Length + 1;

            var vsVersionStrings = new List<string>();
            foreach(var s in vsTags)
            {
                if (!s.Contains(c_VersionTag + "=")) 
                    continue;
                var sTag = s.Substring(iStart, s.IndexOf(' ') - iStart);
                vsVersionStrings.Add(sTag);
            }

            // We want that one with the highest number; a repository may have incremented
            // versions (formats) along the way; the highest one should also be the most
            // recent one.
            var nVersion = 0;
            foreach (var s in vsVersionStrings)
            {
                try
                {
                    var n = Convert.ToInt16(s);
                    nVersion = Math.Max(nVersion, n);
                }
                catch (Exception e)
                {
                    var message = string.Format("GetOurWordVersion ToInt({0}) failed: {1}",
                        s, e.Message);
                    Console.WriteLine(message);
                }
            }

            return nVersion;
        }
        #endregion
        #region Method: void UpdateVersionTag()
        public void UpdateVersionTag()
        {
            // Do nothing if we're already at the desired version
            var currentVersion = GetOurWordVersion();
            if (currentVersion >= c_CurrentVersionNo)
                return;

            AddOurWordVersionTag();
        }
        #endregion
        #region Method: void AddOurWordVersionTag()
        void AddOurWordVersionTag()
        {
            var tagCommand = "tag " + SurroundWithQuotes(TagContents);
            DoCommand(tagCommand);
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void CreateHgIgnoreFile()
        private void CreateHgIgnoreFile()
        {
            // HgIgnore Path
            var sPath = FullPathToRepositoryRoot + Path.DirectorySeparatorChar + ".hgignore";

            // Delete it if it was there already, we're starting over
            if (File.Exists(sPath))
                File.Delete(sPath);

            // Create the new one
            var w = JW_Util.GetTextWriter(sPath);
            w.WriteLine("# List of files that will not be added to the repository");
            w.WriteLine("# WARNING: DO NOT EDIT THIS FILE!");
            w.WriteLine("");

            // Allows us to use expressions like "*.c"
            w.WriteLine("syntax: glob");

            // Ignore anything in the backup folder
            w.WriteLine(".Backup");

            // Ignore any of our bak files that got left hanging around for some reason
            w.WriteLine("*.bak");
            w.WriteLine("*.owtbak");
            w.WriteLine("*.oTransbak");
            w.WriteLine("*.owpbak");

            // Ignore Mercurial files
            w.WriteLine("*.orig");
            w.WriteLine("*.conflict");
            w.WriteLine("*.resolve");

            // Done
            w.Close();
        }
        #endregion        
    }
    #endregion

    #region Class: InternetRepository
    public class InternetRepository : Repository
    {
        // Attrs -----------------------------------------------------------------------------
        #region OAttr{g}: string FullPathToRepositoryRoot
        public override string FullPathToRepositoryRoot
        {
            get
            {
                var path = string.Format("http://{0}:{1}@{2}/{3}",
                    UserName, Password, Server, m_ClusterName.ToLower());
                return path;
            }
        }
        #endregion

        // Username/password are stored in the Registry --------------------------------------
        #region VAttr{g}: string RegistryClusterSubKey
        string RegistryClusterSubKey
        {
            get
            {
                return string.Format("Collaboration\\{0}", m_ClusterName);
            }
        }
        #endregion
        private const string c_registryUserName = "RemoteUserName";
        private const string c_registryPassword = "RemotePassword";
        private const string c_registryServer = "RemoteServer";
        #region SAttr{g/s}: string UserName
        public string UserName
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
        public string Password
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
        public string Server
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
            return string.IsNullOrEmpty(sUrl) ? sUrl : sUrl.Replace("http://", "");
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
        private readonly string m_ClusterName;
        #region Constructor(clusterDisplayName)
        public InternetRepository(string clusterDisplayName)
        {
            if (string.IsNullOrEmpty(clusterDisplayName)) 
                throw new ArgumentNullException("clusterDisplayName");

            m_ClusterName = clusterDisplayName;
        }
        #endregion
    }
    #endregion

    #region Class: Synchronize
    public class Synchronize
    {
        private readonly LocalRepository m_LocalRepository;
        private readonly Repository m_OtherRepository;
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
        #region SMethod: void ThrowIfNoMercurial()
        static void ThrowIfNoMercurial()
        {
            if (!Repository.CheckMercialIsInstalled())
            {
                throw new SynchException("msgNoMercurial",
                    "The external program Mercurial did not respond.\n\n" +
                    "Either it is not installed (most likely), or else you have a corrupt repository.");
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
        #region Method: bool Do(sActionName)
        private bool Do(string sActionName)
            // Returns true if successful
        {
            EnumeratedStepsProgressDlg.Start(sActionName, GetSynchStepNames());

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

        // Sync Actions / Steps --------------------------------------------------------------
        // Common
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

        // SynchLocalToOther
        #region CheckLocalIntegrity
        void CheckLocalIntegrity()
        {
            // Is Mercurial installed? 
            ThrowIfNoMercurial();

            // If this is the first time to synch, then we'll not have created the
            // repo yet. If on the other hand the project was initated from the Internet,
            // then a repo already exists.
            m_LocalRepository.CreateIfDoesntExist();

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

            // If we have problem from a bad merge (from an old version), we need to report 
            // it to the user and get them to fix it (with help our help, I'm sure.)
            if (m_LocalRepository.CheckHasUnresolvedFiles())
            {
                throw new SynchException("msgRepositoryProblem", 
                    "Your local repository is in an error state. This is probably due to a " +
                    "version of OurWord previous to 1.5. Please contact us for help.");
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
                    "OurWord was uable to delete it. Perhaps you have some other software " +
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
        public void PullNewerFiles()
        {
            var result = m_LocalRepository.PullFrom(m_OtherRepository.FullPathToRepositoryRoot);
            if (0 != result.ExitCode)
            {
                throw new SynchException("msgUnableToPull",
                    "OurWord is unable to retrieve changes from the Internet. \n\n" +
                    "The remote computer may not be working, or you may have the username " +
                    "or password incorrectly entered. Please try again later.");
            }

            var nPulledVersion = m_LocalRepository.GetOurWordVersion();
            if (nPulledVersion > LocalRepository.c_CurrentVersionNo && 
                LocalRepository.c_CurrentVersionNo > 0)
            {
                m_LocalRepository.Rollback();
                throw new SynchException("msgYouNeedToUpgrade", 
                    "The data on the Internet was created with a newer version of OurWord. " +
                    "You need to upgrade to the latest version before you will be able to " +
                    "send/receive.");
            }
        }
        #endregion

        #region Merge
        void Merge()
            // Hg's Merge gives strange error messages. E.g., its an error if there was
            // nothing to merge. So we're ignoring the error message returned from 
            // the Execute command.
        {
            var pathToChorusMerge = Repository.SurroundWithQuotes(
                Path.Combine(ExecutionEnvironment.DirectoryOfExecutingAssembly, "ChorusMerge.exe"));

            using (new ShortTermEnvironmentalVariable("OurWordDataVersion", LocalRepository.c_CurrentVersionNo.ToString()))
            using (new ShortTermEnvironmentalVariable("ChorusPathToRepository", m_LocalRepository.FullPathToRepositoryRoot))
            using (new ShortTermEnvironmentalVariable("HGMERGE", pathToChorusMerge))
            {
                m_LocalRepository.DoCommand("merge");
            }

            // If there were unresolved files, it means the merge waas unsuccessful. 
            // We don't want to leave the repository in this state, so we roll it back
            if (m_LocalRepository.CheckHasUnresolvedFiles())
            {
                m_LocalRepository.Rollback();
                throw new SynchException("msgUnableToMerge",
                    "OurWord was unable to merge. This is most likely a bug with OurWord. " +
                    "If you are runing the latest version, then  please contact us " +
                    "at http://ourword.TheSeedCompany.org so that we can work with you to " +
                    "solve the problem.");
            }

            // Store the results of the merge
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
            // Make sure the repository reflects our current version, now that we're 
            // sending changes up. Thus on other computers, older versions of OurWord
            // will rollback if they pull a repo with a higher version than they can
            // handle.
            m_LocalRepository.UpdateVersionTag();

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
            m_LocalRepository.UpdateToCurrentBranchTip();
        }
        #endregion

        // CloneFromOther
        #region Method: void PreliminaryCheckingBeforeClone()
        void PreliminaryCheckingBeforeClone()
        {
            ThrowIfNoMercurial();

            if (Directory.Exists(m_LocalRepository.FullPathToRepositoryRoot))
            {
                throw new SynchException("msgClusterAlreadyExists",
                    "We cannot download this cluster because it already exists. Downloading " +
                    "would overwrite everything you already have. Please backup anything " +
                    "you wish to save, then delete the cluster, then try again.");
            }
        }
        #endregion
        #region Method: void DoClone()
        void DoClone()
        {
            var result = m_LocalRepository.CloneFrom(m_OtherRepository);

            if (0 != result.ExitCode)
            {
                throw new SynchException("msgCloneFailed", 
                    "OurWord was unable to retrieve the data from the Internet.");
            }
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Constructor(local, other, sUserNameForCommits)
        public Synchronize(LocalRepository local, Repository other, string sUserNameForCommits)
        {
            Debug.Assert(null != local);
            Debug.Assert(null != other);
            Debug.Assert(!string.IsNullOrEmpty(sUserNameForCommits));

            m_LocalRepository = local;
            m_OtherRepository = other;
            m_UserNameForCommits = sUserNameForCommits;

            m_SynchSteps = new List<SynchStep>();
        }
        #endregion
        #region Method: bool SynchLocalToOther()
        public bool SynchLocalToOther()
        // Returns true if successful
        {
            var sOtherName = (m_OtherRepository.IsRemoteOnInternet) ? 
                "Internet" : "other repository";

            if (m_OtherRepository.IsRemoteOnInternet)
                AddSynchStep("Checking Internet access", CheckInternetAccess);
            AddSynchStep("Checking data integrity", CheckLocalIntegrity);
            AddSynchStep("Storing any files you've changed", StoreChangedFiles);
            AddSynchStep("Retrieving any newer files from the " + sOtherName, PullNewerFiles);
            AddSynchStep("Merging your changes with theirs", Merge);
            AddSynchStep("Sending all changes back to the " + sOtherName, PushToOther);
            AddSynchStep("Updating your data with the changed", UpdateLocalFiles);

            return Do("Performing Send / Receive...");
        }
        #endregion
        #region Method: bool CloneFromOther()
        public bool CloneFromOther()
        // Returns true if successful
        {
            if (m_OtherRepository.IsRemoteOnInternet)
                AddSynchStep("Checking Internet access", CheckInternetAccess);
            AddSynchStep("Checking your local disk", PreliminaryCheckingBeforeClone);
            AddSynchStep("Downloading from the Internet", DoClone);

            return Do("Creating a cluster from the Internet...");
        }
        #endregion
    }
    #endregion

    #region Class: HowChorusMightWork
    public class HowChorusMightWork
    {
        static public SyncResults LocalWithRemote(string clusterName, string fullPathToLocal, string fullPathToRemote)
        {
            var configuration = BuildConfiguration(fullPathToLocal);

            var synchronizer = Synchronizer.FromProjectConfiguration(configuration,
                new ConsoleProgress());

            var remoteAddress = RepositoryAddress.Create(clusterName, fullPathToRemote);
            remoteAddress.ReadOnly = false;
            remoteAddress.Enabled = true;

            var options = BuildSyncOptions(remoteAddress, "message");

            return synchronizer.SyncNow(options);
        }

        static void LocalWithLocal(string repositoryName, string fullPathToLocal, string fullPathToOther)
        {
            var configuration = BuildConfiguration(fullPathToLocal);

            var synchronizer = Synchronizer.FromProjectConfiguration(configuration,
                new ConsoleProgress());

            var otherAddress = RepositoryAddress.Create(repositoryName, fullPathToOther);
            otherAddress.ReadOnly = false;
            otherAddress.Enabled = true;

            var options = BuildSyncOptions(otherAddress, "message");

            synchronizer.SyncNow(options);
        }

        // Major helper methods
        #region ProjectFolderConfiguration BuildConfiguration(fullPathToLocal)
        static ProjectFolderConfiguration BuildConfiguration(string fullPathToLocal)
        {
            var configuration = new ProjectFolderConfiguration(
                StripTrailingPathSeparator(fullPathToLocal));

            // Files we will most definitely exclude (name them to be double certain)
            configuration.ExcludePatterns.Add("/.backup");
            configuration.ExcludePatterns.Add("**.bak");

            // Files to include
            configuration.IncludePatterns.Add("**.oxes");
            configuration.IncludePatterns.Add(".Settings/*.otrans");
            configuration.IncludePatterns.Add(".Settings/*.owp");
            configuration.IncludePatterns.Add(".Settings/*.owt");

            return configuration;
        }
        #endregion
        #region SyncOptions BuildSyncOptions(address, checkInDescription)
        static SyncOptions BuildSyncOptions(RepositoryAddress address, string checkInDescription)
        {
            var options = new SyncOptions
            {
                DoSendToOthers = true,
                DoPullFromOthers = true,
                DoMergeWithOthers = true,
                CheckinDescription = checkInDescription
            };

            options.RepositorySourcesToTry.Clear();
            options.RepositorySourcesToTry.Add(address);

            return options;
        }
        #endregion

        // Minor helper methods
        #region static string StripTrailingPathSeparator(sPath)
        private static string StripTrailingPathSeparator(string sPath)
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
    }
    #endregion
}
