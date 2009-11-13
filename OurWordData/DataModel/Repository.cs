#region ***** Repository.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Repository.cs
 * Author:  John Wimbish
 * Created: 03 March 2009
 * Purpose: Mercurial connectivity
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Chorus.VcsDrivers;
using JWTools;
using Chorus.sync;
#endregion
#endregion

/* Design Notes
 *    A local OurWord Repository exists at the Cluster level; we'll use a single repository 
 * for the entire cluster.
 */

namespace OurWordData.DataModel
{
    public class SynchronizeIdea
    {
        static public void LocalWithRemote(string clusterName, string fullPathToLocal, string fullPathToRemote)
        {
            var configuration = BuildConfiguration(fullPathToLocal);

            var synchronizer = Synchronizer.FromProjectConfiguration(configuration,
                new Chorus.Utilities.ConsoleProgress());

            var remoteAddress = RepositoryAddress.Create(clusterName, fullPathToRemote);
            remoteAddress.ReadOnly = false;
            remoteAddress.Enabled = true;

            var options = BuildSyncOptions(remoteAddress, "message");

            var results = synchronizer.SyncNow(options);
        }

        static void LocalWithLocal(string repositoryName, string fullPathToLocal, string fullPathToOther)
        {
            var configuration = BuildConfiguration(fullPathToLocal);

            var synchronizer = Synchronizer.FromProjectConfiguration(configuration,
                new Chorus.Utilities.ConsoleProgress());

            var otherAddress = RepositoryAddress.Create(repositoryName, fullPathToOther);
            otherAddress.ReadOnly = false;
            otherAddress.Enabled = true;

            var options = BuildSyncOptions(otherAddress, "message");

            var results = synchronizer.SyncNow(options);
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
        #region string BuildRemoteRepositoryPath(sBaseUrl, sUserName, sPassword)
        static public string BuildRemoteRepositoryPath(string baseUrl, string userName, string password)
        {
            baseUrl = StripLeadingHttp(baseUrl);
            baseUrl = StripTrailingPathSeparator(baseUrl);
            return string.Format("http://{0}:{1}@{2}", userName, password, baseUrl);
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
        static public string StripTrailingPathSeparator(string sPath)
            // Some (all?) Mercurial commands fail if there is a trailing path separator
        {
            if (string.IsNullOrEmpty(sPath))
                return sPath;

            // Check not only for the generic DirectorySeparatorChar, but also for stuff
            // the user might have hardcoded irregardless of which OS was running.
            char ch = sPath[sPath.Length - 1];
            if (ch == Path.DirectorySeparatorChar || ch == '\\' || ch == '/')
                sPath = sPath.Substring(0, sPath.Length - 1);

            return sPath;
        }
        #endregion
        #region static public string StripLeadingHttp(sUrl)
        static public string StripLeadingHttp(string sUrl)
        // In case user typed this, we remove it
        {
            if (string.IsNullOrEmpty(sUrl))
                return sUrl;

            return sUrl.Replace("http://", "");
        }
        #endregion

    }


    public class Repository
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DTeamSettings Cluster
        DTeamSettings Cluster
        {
            get
            {
                Debug.Assert(null != m_Cluster);
                return m_Cluster;
            }
        }
        readonly DTeamSettings m_Cluster;
        #endregion
        #region Attr{g/s}: bool Active - T if source control is turned on
        public bool Active
        {
            get
            {
                return m_bActive;
            }
            set
            {
                // Don't need to do the initialization stuff if we're already there.
                if (m_bActive == value)
                    return;

                // Set to the new value
                m_bActive = value;

                // Make sure we have a repository
                if (true == value)
                    Create();
            }
        }
        bool m_bActive;
        #endregion

        // Settings --------------------------------------------------------------------------
        const string c_sRegistrySubkey = "Collaboration";
        const string c_sRegistryRemoteUserName = "RemoteUserName";
        const string c_sRegistryRemotePassword = "RemotePassword";
        const string c_sRegistryMineAlwaysWins = "MineAlwaysWins";
        #region SAttr{g}: string RegistryClusterSubKey
        static string RegistryClusterSubKey
        {
            get
            {
                return c_sRegistrySubkey + "\\" + DB.TeamSettings.DisplayName;
            }
        }
        #endregion
        #region SAttr{g/s}: string RemoteUrl - Url of the remote central repository
        static public string RemoteUrl
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, DB.TeamSettings.DisplayName, "");
            }
            set
            {
                JW_Registry.SetValue(RegistryClusterSubKey, DB.TeamSettings.DisplayName, value);
            }
        }
        #endregion
        #region SAttr{g/s}: string RemoteUserName - User Name for the central repository
        static public string RemoteUserName
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, c_sRegistryRemoteUserName, "");
            }
            set
            {
                JW_Registry.SetValue(RegistryClusterSubKey, c_sRegistryRemoteUserName, value);
            }
        }
        #endregion
        #region SAttr{g/s}: string RemotePassword - Password for the central repository
        static public string RemotePassword
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, c_sRegistryRemotePassword, "");
            }
            set
            {
                JW_Registry.SetValue(RegistryClusterSubKey, c_sRegistryRemotePassword, value);
            }
        }
        #endregion
        #region SAttr{g/s}: bool MineAlwaysWins
        static public bool MineAlwaysWins
        {
            get
            {
                return JW_Registry.GetValue(RegistryClusterSubKey, 
                    c_sRegistryMineAlwaysWins, false);
            }
            set
            {
                JW_Registry.SetValue(RegistryClusterSubKey, 
                    c_sRegistryMineAlwaysWins, value);
            }
        }
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: string HgRepositoryRoot - e.g., "MyDocuments\Timor"
        public string HgRepositoryRoot
            // Cannot end with a \, else Hg commands like "init" do not work
        {
            get
            {
                string sFolder = Cluster.ClusterFolder;
                Debug.Assert(sFolder.Length > 1);
                sFolder = sFolder.Substring(0, sFolder.Length - 1);
                return sFolder;
            }
        }
        #endregion
        #region VAttr{g}: string RepositoryStore - e.g., "MyDocuments\Timor\.hg\"
        string RepositoryStore
        {
            get
            {
                return Cluster.ClusterFolder + ".hg" + Path.DirectorySeparatorChar;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(Cluster)
        public Repository(DTeamSettings _Cluster, bool bActive)
        {
            Debug.Assert(null != _Cluster);
            m_Cluster = _Cluster;

            m_bActive = bActive;
        }
        #endregion

        // Supporting Methods ----------------------------------------------------------------
        #region SMethod: string SurroundWithQuotes(string s)
        static public string SurroundWithQuotes(string sIn)
        {
            // We need to remove any existing quotes we might have, thus this can be called
            // on paths that already are surrounded by quotes.
            string sOut = "";
            foreach (char ch in sIn)
            {
                if (ch != '\"')
                    sOut += ch;
            }

            // Now place quotes at beginning and end
            return ("\"" + sOut + "\"");
        }
        #endregion
        #region SMethod: string RemoveSpacesAndStuff(string s)
        static string RemoveSpacesAndStuff(string s)
        {
            string sOut = "";
            foreach (char ch in s)
            {
                if (!char.IsWhiteSpace(ch) && !char.IsPunctuation(ch))
                    sOut += ch;
            }
            return sOut;
        }
        #endregion

        // Mercurial Direct Commands ---------------------------------------------------------
        // Commands
        const string c_sInit = "init";  // Create the repository
        const string c_sStatus = "status"; // Get a list of files that have changed
        const string c_sCommit = "commit"; // Commit the changed files to the repo
        const string c_sClone = "clone";
        const string c_sPush = "push";
        const string c_sPull = "pull";
        const string c_sOutGoing = "outgoing"; // Show changes that will be pushed
       // Options
        const string c_sAddRemove = "-A"; // mark new/missing files as added/removed before committing
        const string c_sMessage = "-m";   // The following text is the Message for a commit
        const string c_suser = "-u";

        #region CLASS: ProcessStream (from Chorus)
        // from SeemabK in a comment here: http://www.hanselman.com/blog/CommentView.aspx?guid=362
        public class ProcessStream
        {
            private Thread StandardOutputReader;
            private Thread StandardErrorReader;
            private static Process RunProcess;

            private string _StandardOutput = "";
            public string StandardOutput
            {
                get { return _StandardOutput; }
            }
            private string _StandardError = "";
            public string StandardError
            {
                get { return _StandardError; }
            }

            public ProcessStream()
            {
                Init();
            }

            public int Read(ref Process process)
            {
                try
                {
                    Init();
                    RunProcess = process;

                    if (RunProcess.StartInfo != null)
                    {
                        if (RunProcess.StartInfo.RedirectStandardOutput)
                        {
                            StandardOutputReader = new Thread(new ThreadStart(ReadStandardOutput));
                            StandardOutputReader.Start();
                        }
                        if (RunProcess.StartInfo.RedirectStandardError)
                        {
                            StandardErrorReader = new Thread(new ThreadStart(ReadStandardError));
                            StandardErrorReader.Start();
                        }
                    }

                    //RunProcess.WaitForExit();
                    if (StandardOutputReader != null)
                        StandardOutputReader.Join();
                    if (StandardErrorReader != null)
                        StandardErrorReader.Join();
                }
                catch
                { }

                return 1;
            }

            private void ReadStandardOutput()
            {
                if (RunProcess != null)
                    _StandardOutput = RunProcess.StandardOutput.ReadToEnd();
            }

            private void ReadStandardError()
            {
                if (RunProcess != null)
                    _StandardError = RunProcess.StandardError.ReadToEnd();
            }

            private int Init()
            {
                _StandardError = "";
                _StandardOutput = "";
                RunProcess = null;
                Stop();
                return 1;
            }

            public int Stop()
            {
                try
                {
                    StandardOutputReader.Abort();
                }
                catch { }
                try
                {
                    StandardErrorReader.Abort();
                }
                catch { }
                StandardOutputReader = null;
                StandardErrorReader = null;
                return 1;
            }
        }
        #endregion
        #region CLASS: ExecutionResult (from Chorus)
        public class ExecutionResult
        {
            public int ExitCode;
            public string StandardError;
            public string StandardOutput;

            public bool Successful
            {
                get
                {
                    return (ExitCode == 0);
                }
            }

            public ExecutionResult(Process proc)
            {
                ProcessStream ps = new ProcessStream();
                ps.Read(ref proc);
                StandardOutput = ps.StandardOutput;
                StandardError = ps.StandardError;
                ExitCode = proc.ExitCode;
            }
        }
        #endregion
        #region SMethod: ExecutionResult Execute(string sHgCommand, string sWorkingDir)
        static public ExecutionResult Execute(string sHgCommand, string sWorkingDir)
        {
            // Initialize the process
            Process p = new Process();
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = sWorkingDir;
            p.StartInfo.FileName = "hg";
            p.StartInfo.Arguments = sHgCommand;

            // Execute the process
            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Execution failed with message: " + e.Message);
                return null;
            }

            // Success, but possibly with messages as stored in the Result
            return new ExecutionResult(p);
        }
        #endregion
        #region Method: ExecutionResult Execute(sHgCommand)
        ExecutionResult Execute(string sHgCommand)
        {
            return Execute(sHgCommand, HgRepositoryRoot);
        }
        #endregion

        #region CLASS: ChangeSetDescription
        public class ChangeSetDescription
        {
            #region Attr{g/s}: string ID
            public string ID
            {
                get
                {
                    return m_sID;
                }
                set
                {
                    m_sID = value;
                }
            }
            string m_sID;
            #endregion
            #region Attr{g/s}: string Tag
            public string Tag
            {
                get
                {
                    return m_sTag;
                }
                set
                {
                    m_sTag = value;
                }
            }
            string m_sTag;
            #endregion
            #region Attr{g/s}: string User
            public string User
            {
                get
                {
                    return m_sUser;
                }
                set
                {
                    m_sUser = value;
                }
            }
            string m_sUser;
            #endregion
            #region Attr{g/s}: string Date
            public string Date
            {
                get
                {
                    return m_sDate;
                }
                set
                {
                    m_sDate = value;
                }
            }
            string m_sDate;
            #endregion
            #region Attr{g/s}: string Summary
            public string Summary
            {
                get
                {
                    return m_sSummary;
                }
                set
                {
                    m_sSummary = value;
                }
            }
            string m_sSummary;
            #endregion

            #region Constructor(sID, sTag, sUser, sDate, sSummary)
            public ChangeSetDescription(string sID, string sTag, string sUser, 
                string sDate, string sSummary)
            {
                m_sID = sID;
                m_sTag = sTag;
                m_sUser = sUser;
                m_sDate = sDate;
                m_sSummary = sSummary;
            }
            #endregion
            #region Constructor()
            public ChangeSetDescription()
            {
            }
            #endregion

            #region List<> Create(vs[])
            static public List<ChangeSetDescription> Create(string[] vs)
            /* Changesets arrive in the form of:
             *    changeset:   1:bbe11909d08a
             *    tag:         tip
             *    user:        JWimbish
             *    date:        Wed Mar 04 19:37:07 2009 -0500
             *    summary:     New File Added
             */
            {
                var v = new List<ChangeSetDescription>();

                string sID = null;
                string sTag = null;
                string sUser = null;
                string sDate = null;
                string sSummary = null;

                foreach (string s in vs)
                {
                    int i = 0;

                    // Collect the field
                    string sField = "";
                    while (i < s.Length && s[i] != ':')
                        sField += s[i++];

                    // Move to the next one
                    if (i < s.Length && s[i] == ':')
                        i++;
                    while (i < s.Length && char.IsWhiteSpace(s[i]))
                        i++;

                    // Collect the data
                    string sData = "";
                    while (i < s.Length)
                        sData += s[i++];

                    if (sField == "changeset")
                        sID = sData;
                    if (sField == "tag")
                        sTag = sData;
                    if (sField == "user")
                        sUser = sData;
                    if (sField == "date")
                        sDate = sData;
                    if (sField == "summary")
                    {
                        sSummary = sData;

                        if (!string.IsNullOrEmpty(sID) &&
                            !string.IsNullOrEmpty(sTag) &&
                            !string.IsNullOrEmpty(sUser) &&
                            !string.IsNullOrEmpty(sDate) &&
                            !string.IsNullOrEmpty(sSummary))
                        {
                            ChangeSetDescription set = new ChangeSetDescription(
                                sID, sTag, sUser, sDate, sSummary);
                            v.Add(set);
                        }

                        sID = null;
                        sTag = null;
                        sUser = null;
                        sDate = null;
                        sSummary = null;

                    }
                }
                return v;
            }
            #endregion
        }
        #endregion

        // Static version of Operations ------------------------------------------------------
        #region SMethod: string BuildRemoteRepositoryString(sUrl, sUserName, sPassword)
        static public string BuildRemoteRepositoryString(string sUrl, string sUserName, string sPassword)
        {
            string sRemotePath = "http://" +
                sUserName + ":" +
                sPassword + "@" +
                sUrl;
            return sRemotePath;
        }
        #endregion
        #region SMethod: bool CloneTo(string sDestinationPath, string sRepository)
        static public bool CloneTo(string sDestinationPath, string sRepository)
        {
            string sCommand = c_sClone;

            // Source Repository
            sCommand += (" " + SurroundWithQuotes(sRepository));

            // Destination Path
            sDestinationPath = sDestinationPath.Trim(new char[] { Path.DirectorySeparatorChar });
            sCommand += (" " + SurroundWithQuotes(sDestinationPath));

            // Do it
            ExecutionResult result = Execute(sCommand, "");
            return result.Successful;
        }
        #endregion


        // Operations ------------------------------------------------------------------------
        #region SAttr{g}: bool HgIsInstalled
        static public bool HgIsInstalled
        {
            get
            {
                // Attempt to get mercurial's version. If it fails, then Mercurial
                // was not found.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "hg";
                startInfo.Arguments = "version";
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion
        #region VAttr{g}: bool Exists
        /// <summary>
        /// Returns T if the repository exists, as evidenced by having a .Hg directory
        /// </summary>
        public bool Exists
        {
            get
            {
                if (Directory.Exists(RepositoryStore))
                    return true;
                return false;
            }
        }
        #endregion
        #region Method: bool Create()
        public bool Create()
        {
            if (!Active)
                return false;

            // If it exists already, we don't want to create over it.
            if (Exists)
                return false;

            // Create the directory if it does not exist
            if (!Directory.Exists(HgRepositoryRoot))
                Directory.CreateDirectory(HgRepositoryRoot);

            // This will create an ".Hg" directory under the Cluster
            var result = Execute(c_sInit);
            var bSuccess = ((result.ExitCode == 0) ? true : false );

            // Create the list of files to ignore; make that our first commit
            if (bSuccess)
            {
                CreateHgIgnore();
                Commit("Initial set up of Repository.", true);
            }

            return bSuccess;
        }
        #endregion
        #region Method: void CreateHgIgnore()
        /// <summary>
        /// Create (if missing) and make sure the HgIgnore file is up to date
        /// </summary>
        private void CreateHgIgnore()
        {
            if (!Active)
                return;

            // HgIgnore Path
            string sPath = HgRepositoryRoot + Path.DirectorySeparatorChar + ".hgignore";

            // Delete it if it was there already, we're starting over
            if (File.Exists(sPath))
                File.Delete(sPath);

            // Create the new one
            TextWriter w = JW_Util.GetTextWriter(sPath);
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

            // Done
            w.Close();
        }
        #endregion
        #region Method: List<string> GetChangedFiles()
        public List<string> GetChangedFiles()
        {
            List<string> vsFiles = new List<string>();

            if (!Active)
                return vsFiles;

            ExecutionResult result = Execute(c_sStatus);

            string[] vsLines = result.StandardOutput.Split('\n');

            foreach (string line in vsLines)
            {
                if(line.Trim()!="")
                    vsFiles.Add(line.Substring(2)); //! data.txt
            }

            return vsFiles;
        }
        #endregion
        #region Method: bool Commit(sMessage, bAddFiles)
        public bool Commit(string sMessage, bool bAddFiles)
            #region Mercurial Documentation
            /* Mercurial Documentation
             * ------------------------
             *  Hg commit[OPTION] [FILE]
             * 
             *  Commit changes to the given files into the repository.
             *  
             *  If a list of files is omitted, all changes reported by "hg status" will be committed.
             *  
             *  If you are committing the result of a merge, do not provide any file names or
             *  -I/-X filters.
             *  
             *  If no commit message is specified, the configured editor is started to enter a message.
             *  
             *  See 'hg help dates' for a list of formats valid for -d/--date.
             *  
             *  options:
             *  -A, --addremove  mark new/missing files as added/removed before committing
             *  --close-branch   mark a branch as closed, hiding it from the branch list
             *  -I, --include    include names matching the given patterns
             *  -X, --exclude    exclude names matching the given patterns
             *  -m, --message    use <text> as commit message
             *  -l, --logfile    read commit message from <file>
             *  -d, --date       record datecode as commit date
             *  -u, --user       record user as committer
             */
            #endregion
        {
            if (!Active)
                return false;

            // Commit
            string sCommand = c_sCommit;

            // Any files not known about (new or removed) will be added
            // to tracking automatically.
            if (bAddFiles)
                sCommand += (" " + c_sAddRemove);

            // User name
            sCommand += (" " + c_suser + " " +
                SurroundWithQuotes(RemoveSpacesAndStuff(DB.UserName)));

            // Commit message
            sCommand += (" " + c_sMessage + " " + SurroundWithQuotes(sMessage));

            // Do it
            ExecutionResult result = Execute(sCommand);
            return result.Successful;
        }
        #endregion

        #region Method: bool CloneTo(sDestinationPath)
        public bool CloneTo(string sDestinationPath)
            // If cloning to a path on a computer, the destination path cannot
            // exist, the Clone process creates it, and fails if it already
            // exists. I don't delete it here, though, because I don't yet
            // know what happens if you are cloning across the Internet.
        {
            if (!Active)
                return false;

            string sCommand = c_sClone;

            // Source Repository
            sCommand += (" " + SurroundWithQuotes(HgRepositoryRoot));

            // Destination Repository
            sCommand += (" " + SurroundWithQuotes(sDestinationPath));

            // Do it
            ExecutionResult result = Execute(sCommand);
            return result.Successful;
        }
        #endregion
        #region Method: List<csd> OutGoing(sDestinationPath)
        static public List<ChangeSetDescription> OutGoing(string sDestinationPath)
        /* Mercurial Documentation
         * ------------------------
         * outgoing [-M] [-p] [-n] [-f] [-r REV]… [DEST]
         * 
         * Show changesets not found in the specified destination repository or the default 
         * push location. These are the changesets that would be pushed if a push was requested.
         * 
         * See pull for valid destination format details.
         * 
         * options:
         * -f, --force         run even when remote repository is unrelated
         * -r, --rev           a specific revision up to which you would like to push
         * -n, --newest-first  show newest record first
         * -p, --patch         show patch
         * -g, --git           use git extended diff format
         * -l, --limit         limit number of changes displayed
         * -M, --no-merges     do not show merges
         * --style             display using template map file
         * --template          display with template
         * -e, --ssh           specify ssh command to use
         * --remotecmd         specify hg command to run on the remote side
         */
        {
//            if (!Active)
//                return new List<ChangeSetDescription>();

            // Outgoing
            var sCommand = c_sOutGoing;

            // Destination Repository
            sCommand += (" " + SurroundWithQuotes(sDestinationPath));

            // Do it
            var result = Execute(sCommand, "");

            // Interpret it
            if (result.Successful)
            {
                var vsLines = result.StandardOutput.Split('\n');
                var v = ChangeSetDescription.Create(vsLines);
                return v;
            }

            // Unsuccessful: return an empty list
            return new List<ChangeSetDescription>();
        }
        #endregion

        #region Method: bool PushTo(sDestinationPath)
        public bool PushTo(string sDestinationPath)
            #region Mercurial Documentation
            /* Mercurial Documentation
             * ------------------------
             * Hg push [-f] [-r REV] [-e CMD] [—remotecmd CMD] [DEST] 
             * 
             * Push changes from the local repository to the given destination.
             * 
             * This is the symmetrical operation for pull. It helps to move changes from
             * the current repository to a different one. If the destination is local this
             * is identical to a pull in that directory from the current one.
             * 
             * By default, push will refuse to run if it detects the result would increase
             * the number of remote heads. This generally indicates the the client has
             * forgotten to pull and merge before pushing.
             * 
             * If -r is used, the named changeset and all its ancestors will be pushed
             * to the remote repository.
             * 
             * Look at the help text for urls for important details about ssh:// URLs. If
             * DESTINATION is omitted, a default path will be used. See 'hg help urls'
             * for more information.
             * 
             * options:
             * -f, --force  force push
             * -r, --rev    a specific revision up to which you would like to push
             * -e, --ssh    specify ssh command to use
             * --remotecmd  specify hg command to run on the remote side
             */
            #endregion
            // Note: We don't include the Local repository in the command line, but Hg 
            // knows about it anyway because the Execute command sets the working directory 
            // to it.
        {
            if (!Active)
                return false;

            // Push
            string sCommand = c_sPush;

            // Destination Repository
            sCommand += (" " + SurroundWithQuotes(sDestinationPath));

            // Do it
            ExecutionResult result = Execute(sCommand);
            return result.Successful;
        }
        #endregion
        #region Method: bool CanAccessInternet()
        static public bool CanAccessInternet()
            // We'll just ping on Google, since they're up most of the time.
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send("www.google.com");
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
        #endregion

    }

}
