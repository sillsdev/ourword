#region ***** Repository.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Repository.cs
 * Author:  John Wimbish
 * Created: 03 March 2009
 * Purpose: Mercurial connectivity
 * 
 * NOTE: My intention is to use Chorus. But given that I have just over two weeks before
 * introducing this in a language project, and that Chorus isn't ready, I'm going to
 * implement code here and shamelessly steal from Chorus, with the idea that Hatton
 * and I can later reconcile anything new I invent, and obsolete most of this code.
 *    I figure I'll always need a class that OurWord interacts with (which this will be),
 * but the internals of the class will likely eventually just become calls to Chorus.
 * 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using JWTools;
using JWdb;

using Chorus.VcsDrivers.Mercurial;
using Chorus.sync;
#endregion
#endregion

/* Design Notes
 *    A local OurWord Repository exists at the Cluster level; we'll use a single repository 
 * for the entire cluster.
 */

namespace JWdb.DataModel
{
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
        #region VAttr{g}: string PathToChorusMerge
        string PathToChorusMerge
        {
            get
            {
                return SurroundWithQuotes(
                    Path.Combine(Other.DirectoryOfExecutingAssembly, "ChorusMerge.exe"));
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
        static string SurroundWithQuotes(string sIn)
        {
            // We need to remove any existing quotes we might have
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
        const string c_sMerge = "merge";
        const string c_sUpdate = "update";
        const string c_sHeads = "heads";
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
        static ExecutionResult Execute(string sHgCommand, string sWorkingDir)
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
            catch (Exception)
            {
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
        #region SMethod: bool CloneTo(string sDestinationPath, string sSourcePath)
        static public bool CloneTo(string sDestinationPath, string sSourcePath)
        {
            string sCommand = c_sClone;

            // Source Repository
            sCommand += (" " + SurroundWithQuotes(sSourcePath));

            // Destination Path
            sCommand += (" " + SurroundWithQuotes(sDestinationPath));

            // Do it
            ExecutionResult result = Execute(sCommand, sSourcePath);
            return result.Successful;
        }
        #endregion
        #region SMethod: bool Pull(string sSourcePath)
        static public bool Pull(string sSourcePath)
        {
            // Pull
            string sCommand = c_sPull;

            // Source Repository
            sCommand += (" " + SurroundWithQuotes(sSourcePath));

            // Do it
            ExecutionResult result = Execute(sCommand, sSourcePath);
            return result.Successful;
        }
        #endregion

        // Heads ----------------------------------------------------------------------------
        #region CLASS: Head
        class Head
        {
            #region Attr{g/s}: string ChangeSet
            public string ChangeSet
            {
                get
                {
                    return m_sChangeSet;
                }
                set
                {
                    m_sChangeSet = value;
                }
            }
            string m_sChangeSet;
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

            #region Constructor(sChangeSet, sTag, sUser, sDate, sSummary)
            public Head(string _sChangeSet, string _sTag, string _sUser, string _sDate, string _sSummary)
            {
                m_sChangeSet = _sChangeSet;
                m_sTag = _sTag;
                m_sUser = _sUser;
                m_sDate = _sDate;
                m_sSummary = _sSummary;
            }
            #endregion
        }
        #endregion
        #region Method: List<Head> GetHeads()
        List<Head> GetHeads()
        {
            var v = new List<Head>();

            if (!Active)
                return v;

            // Request the information from Mercurial
            var result = Execute(c_sHeads);
            if (null == result)
                return null;

            string[] vsLines = result.StandardOutput.Split('\n');

            string sChangeSet = "";
            string sTag = "";
            string sUser = "";
            string sDate = "";
            string sSummary = "";

            int iPosData = 14;

            foreach (string s in vsLines)
            {
                if (s.StartsWith("changeset:") && s.Length > iPosData)
                {
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
                {
                    sSummary = s.Substring(iPosData);
                    v.Add( new Head(sChangeSet, sTag, sUser, sDate, sSummary));
                }
            }

            return v;
        }
        #endregion
        #region Method: int GetHeadsCount()
        int GetHeadsCount()
        {
            if (!Active)
                return 0;

            return GetHeads().Count;
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
            ExecutionResult result = Execute(c_sInit);
            bool bSuccess = ((result.ExitCode == 0) ? true : false );

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
        public void CreateHgIgnore()
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
        public List<ChangeSetDescription> OutGoing(string sDestinationPath)
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
            if (!Active)
                return new List<ChangeSetDescription>();

            // Outgoing
            string sCommand = c_sOutGoing;

            // Destination Repository
            sCommand += (" " + SurroundWithQuotes(sDestinationPath));

            // Do it
            ExecutionResult result = Execute(sCommand);

            // Interpret it
            if (result.Successful)
            {
                string[] vsLines = result.StandardOutput.Split('\n');
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
        #region Method: bool PullFrom(sSourcePath)
        public bool PullFrom(string sSourcePath)
            #region Mercurial Documentation
            /* Mercurial Documentation
             * ------------------------
             * pull[-u] [-f] [-r REV]… [-e CMD] [—remotecmd CMD] [SOURCE]
             * 
             * Pull changes from a remote repository to a local one.
             * 
             * This finds all changes from the repository at the specified path
             * or URL and adds them to the local repository. By default, this
             * does not update the copy of the project in the working directory.
             * 
             * If SOURCE is omitted, the 'default' path will be used.
             * See 'hg help urls' for more information.
             * 
             * options:
             * -u, --update  update to new tip if changesets were pulled
             * -f, --force   run even when remote repository is unrelated
             * -r, --rev     a specific revision up to which you would like to
             *                   pull
             * -e, --ssh     specify ssh command to use
             * --remotecmd   specify hg command to run on the remote side
             */
            #endregion
        {
            if (!Active)
                return false;

            // Pull
            string sCommand = c_sPull;

            // Source Repository
            sCommand += (" " + SurroundWithQuotes(sSourcePath));

            // Do it
            ExecutionResult result = Execute(sCommand);
            /***
            if (!result.Successful)
            {
                Console.WriteLine("PullFrom failed with ExitCode=" + result.ExitCode.ToString() +
                    "\nE=" + result.StandardError +
                    "\nO=" + result.StandardOutput);
                Console.WriteLine("\nCommand=" + sCommand);
            }
            ***/
            return result.Successful;
        }
        #endregion
        #region Method: bool Update()
        public bool Update()
            #region Mercurial Documentation
            /* Mercurial Documentation
             * ------------------------
             * update[-C] [-d DATE] [[-r] REV] 
             * 
             * Update the repository's working directory to the specified revision, or 
             * the tip of the current branch if none is specified. Use null as the revision 
             * to remove the working copy (like hg clone -U).

             * When the working dir contains no uncommitted changes, it will be
             * replaced by the state of the requested revision from the repo.  When
             * the requested revision is on a different branch, the working dir
             * will additionally be switched to that branch.
             * 
             * When there are uncommitted changes, use option -C to discard them,
             * forcibly replacing the state of the working dir with the requested
             * revision.
             * 
             * When there are uncommitted changes and option -C is not used, and
             * the parent revision and requested revision are on the same branch,
             * and one of them is an ancestor of the other, then the new working
             * directory will contain the requested revision merged with the
             * uncommitted changes.  Otherwise, the update will fail with a
             *  suggestion to use 'merge' or 'update -C' instead.
             * 
             * If you want to update just one file to an older revision, use revert.
             * 
             * See 'hg help dates' for a list of formats valid for --date.
             * 
             * options:
             * -C, --clean  overwrite locally modified files (no backup)
             * -d, --date   tipmost revision matching date
             * -r, --rev    revision
             * 
             *  aliases: up checkout co
             */
            #endregion
        {
            if (!Active)
                return false;

            // Update
            string sCommand = c_sUpdate;

            // Do it
            ExecutionResult result = Execute(sCommand);
            return result.Successful;
        }
        #endregion
        #region Method: bool SynchronizeWithRemote()
        public bool SynchronizeWithRemote()
        {
            string sRemotePath = "http://" +
                RemoteUserName + ":" + 
                RemotePassword + "@" +
                RemoteUrl;
            return SynchronizeWith(sRemotePath);
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
        #region Attr{g}: bool HasUnresolvedFiles
        bool HasUnresolvedFiles
        {
            get
            {
                var result = Execute("resolve -l");
                string[] vsLines = result.StandardOutput.Split('\n');

                foreach (string s in vsLines)
                {
                    if (s.Length > 2 && s[0] == 'U' && s[1] == ' ')
                        return true;
                }

                return false;
            }
        }
        #endregion

        // Main Synchronize Helper Methods ---------------------------------------------------
        // TODO: Error messages on everything; and incorporate the Hg's error text into our error dialog.
        #region Method: void LaunchSynchProgressDialog()
        void LaunchSynchProgressDialog()
            // Launch the Progress dialog in a separate thread so that it will update.
            // We loop until we know it is created, and then waiti an addition couple
            // of seconds to make sure it is showing.
        {
            SynchProgressDlg.Start();
            while (!SynchProgressDlg.IsCreated)
                Thread.Sleep(500);
            Thread.Sleep(2000);
        }
        #endregion
        #region Method: bool CheckInternetAccess()
        bool CheckInternetAccess()
        {
            SynchProgressDlg.InternetAccess = SynchProgressDlg.GetStartState();

            if (!CanAccessInternet())
            {
                SynchProgressDlg.InternetAccess = SynchProgressDlg.GetFinishState(false);

                SynchProgressDlg.ShowError("kNoInternetConnection",
                    "OurWord is unable to connect to the Internet. \n\n" +
                    "Please check that you have an active Internet connection, then try again.");

                return false;
            }

            SynchProgressDlg.InternetAccess = SynchProgressDlg.GetFinishState(true);
            return true;
        }
        #endregion
        #region Method: bool CheckMercurialPresent()
        bool CheckMercurialPresent()
        {
            // Issue the Mercurial command to get the number of heads in the repo
            int cHeads = GetHeadsCount();

            // If it returns zero, we interpret this to mean Mercurial is not properly installed.
            if (cHeads == 0)
            {
                SynchProgressDlg.ShowError("msgNoMercurial",
                    "The external program Mercurial did not respond.\n\n" +
                    "Either it is not installed, or you have a corrupt repository. Refer to the Help file.");
                return false;
            }

            return true;
        }
        #endregion
        #region Method: bool CheckLocalIntegrity()
        bool CheckLocalIntegrity()
        {
            SynchProgressDlg.Integrity = SynchProgressDlg.GetStartState();

            // Issue a command just to make certain that Mercurial is installed
            if (!CheckMercurialPresent())
            {
                SynchProgressDlg.Integrity = SynchProgressDlg.GetFinishState(false);
                return false;
            }

            // If we had an interrupted transaction, then we need to recover from it. This
            // command has  no effect if the repositiory is in good shape; but it repairs the
            // repositiory if a transaction was interrupted.
            Execute("Recover");

            // Do we have unresolved files?
            if (HasUnresolvedFiles)
            {
                // Attempt to resolve them. Most likely this will fail, because unresolved files are 
                // generally due to a bug in the merge code; but if the user has installed a later
                // version of OW, then perhaps the bug will have been fixed.
                using (new ShortTermEnvironmentalVariable("HGMERGE", PathToChorusMerge))
                {
                    var result = Execute("resolve -a");
                    if (result.Successful)
                        Commit("Repaired an unresolved merge.", false);
                }

                // If we still have unresolved files, then report the problem to the user
                // TODO: Offer the user the opportunity to choose Repo vs Local versions for the problem files
                if (HasUnresolvedFiles)
                {
                    SynchProgressDlg.ShowError("msgRepositoryProblem",
                        "We're sorry, but there is apparently a bug in OurWord related to synchronization.\n\n" +
                        "Please contact us at http://ourword.TheSeedCompany.org for information " +
                        "on how to solve this problem.");
                    SynchProgressDlg.Integrity = SynchProgressDlg.GetFinishState(false);
                    return false;
                }
            }

            SynchProgressDlg.Integrity = SynchProgressDlg.GetFinishState(true);
            return true;
        }
        #endregion
        #region Method: bool CommitAnyChangedFiles()
        bool CommitAnyChangedFiles()
        {
            string[] vsExtensions = new string[] { ".owp", ".otrans", ".owt", ".db" };

            SynchProgressDlg.StoringRecentChanges = SynchProgressDlg.GetStartState();

            // Get a list of the files that have changed
            var vsChangedFiles = GetChangedFiles();
            if (vsChangedFiles.Count == 0)
                goto Done;

            // Remove any that we don't recognize, so that they don't get added to the repo.
            // (If they were already in the repo, then this will resort in their being
            // removed.)
            foreach (string s in vsChangedFiles)
            {
                string sPath = HgRepositoryRoot + Path.DirectorySeparatorChar + s;

                string sExtension = Path.GetExtension(sPath).ToLower();
                bool bExtensionFound = false;
                foreach (string sExt in vsExtensions)
                {
                    if (sExtension == sExt)
                        bExtensionFound = true;
                }

                if (!bExtensionFound)
                {
                    File.Delete(sPath);
                    if (File.Exists(sPath))
                    {
                        LocDB.Message("msgCantDeleteFileBeforeCommit",
                            "An unrecognized file, \"{0}\", is in your data folder.{n}{n} " +
                                "OurWord was unable to delete it. Perhaps you have some other " +
                                "software running which is using that file. Please delete the " +
                                "file, then try again.",
                            new string[] { s },
                            LocDB.MessageTypes.Error);
                        SynchProgressDlg.StoringRecentChanges = SynchProgressDlg.GetFinishState(false);
                        return false;
                    }
                }
            }

            // Rebuild our list, to make sure we still have something to commit
            vsChangedFiles = GetChangedFiles();
            if (vsChangedFiles.Count == 0)
                goto Done;

            // Commit the changed files
            bool bResult = Commit("Synching from " + DB.UserName + " " +
                DateTime.UtcNow.ToString("u", DateTimeFormatInfo.InvariantInfo), true);

            // Report failure
            if (!bResult)
            {
                 SynchProgressDlg.ShowError("msgCantCommitRecentChanges",
                     "OurWord was unable to place your most recent changes into the local Repository.\n\n" +
                    "Please try again. If the problem continues, please contact us at " +
                     "http://ourword.TheSeedCompany.org so that we can determine how to " +
                     "solve this problem.");
                 SynchProgressDlg.StoringRecentChanges = SynchProgressDlg.GetFinishState(false);
                 return false;
            }

        Done:
            SynchProgressDlg.StoringRecentChanges = SynchProgressDlg.GetFinishState(true);
            return true;
        }
        #endregion
        #region Method: bool PullRemoteChanges(string sTheirPath)
        bool PullRemoteChanges(string sTheirPath)
        {
            SynchProgressDlg.Pulling = SynchProgressDlg.GetStartState();

            if (!PullFrom(sTheirPath))
            {
                SynchProgressDlg.ShowError("msgUnableToPull",
                    "OurWord is unable to retrieve changes from the Internet. \n\n" +
                    "The remote computer may not be working. Please try again sometime later.");

                SynchProgressDlg.Pulling = SynchProgressDlg.GetFinishState(false);
                return false;
            }

            SynchProgressDlg.Pulling = SynchProgressDlg.GetFinishState(true);
            return true;
        }
        #endregion
        #region Method: bool MergeOurChanges()
        bool MergeOurChanges()
            // Hg's Merge gives strange error messages. E.g., its an error if there was
            // nothing to merge. So we're ignoring the error message returned from 
            // the Execute command.
            // 
            // TODO: Figure out if the Merge was unsuccessful and display an error message
        {
            SynchProgressDlg.Merging = SynchProgressDlg.GetStartState();

            using (new ShortTermEnvironmentalVariable("HGMERGE", PathToChorusMerge))
            {
                ExecutionResult result = Execute(c_sMerge);
            }

            SynchProgressDlg.Merging = SynchProgressDlg.GetFinishState(true);
            return true;
        }
        #endregion
        #region Method: bool CommitTheMerge()
        bool CommitTheMerge()
            // TODO: Cam Commit return a false error? E.g., saying it didn't commit when
            // in actuality it was because there was nothing to commit?
        {
            SynchProgressDlg.StoringMergeResults = SynchProgressDlg.GetStartState();

            bool bOK = Commit("Merged by " + DB.UserName + " " +
                DateTime.UtcNow.ToString("u", DateTimeFormatInfo.InvariantInfo),
                false);

            if (!bOK)
            {
                SynchProgressDlg.ShowError("msgUnableToCommitMerge",
                    "OurWord was unable to store the results of the merge. \n\n" +
                    "This is an odd thing to happen. If it continues, please contact us at http://ourword.TheSeedCompany.org so that we can work with you to solve the problem.");

                SynchProgressDlg.StoringMergeResults = SynchProgressDlg.GetFinishState(false);
                return false;
            }

            SynchProgressDlg.StoringMergeResults = SynchProgressDlg.GetFinishState(true);
            return true;
        }
        #endregion
        #region Method: bool PushToRemote(sTheirPath)
        bool PushToRemote(string sTheirPath)
        {
            SynchProgressDlg.Pushing = SynchProgressDlg.GetStartState();

            bool bResult = PushTo(sTheirPath);

            SynchProgressDlg.Pushing = SynchProgressDlg.GetFinishState(bResult);

            return bResult;
        }
        #endregion
        #region Method: bool SynchronizeWith(string sTheirPath)
        public bool SynchronizeWith(string sTheirPath)
            // Returns true if succeeded, false on failure
        {
            // Only proceed if Source Control has been turned on
            if (!Active)
                return false;

            // Launch the Progress dialog (in another thread)
            LaunchSynchProgressDialog();
            bool bOK = true;

            // Do we have an Internet connection? Display a message if not and give up
            if (!CheckInternetAccess())
                bOK = false;

            // Check for any problems in the local setup (e.g., unresolved merges), and fix them.
            if (bOK && !CheckLocalIntegrity())
                bOK = false;

            // Commit any files we've changed
            if (bOK && !CommitAnyChangedFiles())
                bOK = false;

            // Pull any changes from the remote server
            if (bOK && !PullRemoteChanges(sTheirPath))
                bOK = false;

            // Merge our changes with what we've just pulled
            if (bOK && !MergeOurChanges())
                bOK = false;

            // Commit the merge we've just done
            if (bOK && !CommitTheMerge())
                bOK = false;

            // Push the merge to the destination repository
            if (bOK && !PushToRemote(sTheirPath))
                bOK = false;

            // Update our current working directory
            if (bOK && !Update())
                bOK = false;

            // Done with the Progress dialog
            SynchProgressDlg.Stop();
            return bOK;
        }
        #endregion

        #region REWORKED 22jul09 - Delete when confident rework is ok
        /****
        #region Method: bool SynchronizeWith(string sTheirPath)
        public bool SynchronizeWith(string sTheirPath)
            // We Pull, Merge, then Push
        {
            return SynchWith(sTheirPath);

            bool bResult = true;

            if (!Active)
                return false;

            // Launch the Progress dialog in a separate thread so that it will update.
            SynchProgressDlg.Start();
            while (!SynchProgressDlg.IsCreated)  // Wait for it to get created
                Thread.Sleep(500);
            Thread.Sleep(2000);                  // Give it time to show

            // Do we have an Internet connection? Display a message if not.
            SynchProgressDlg.InternetAccess = SynchProgressDlg.GetStartState();
            if (!CanAccessInternet())
            {
                // TODO: TEMPORARY FOR YAWA INTALLATION
                SynchProgressDlg.ShowError("kNoInternetConnection",
                    "Sabda Kita tidak dapat terhubung dengan Internet.\n\n" +
                    "Periksalah apakah anda terhubung secara aktif dengan Internet, lalu coba lagi.");

                //SynchProgressDlg.ShowError("kNoInternetConnection",
                //    "OurWord is unable to connect to the Internet. \n\n" +
                //    "Please check that you have an active Internet connection, then try again.");

                SynchProgressDlg.Stop();
                return false; // No point in sticking around.
            }
            SynchProgressDlg.InternetAccess = SynchProgressDlg.GetFinishState(true);

            // If we are in a state where a previous merge was not committed, take care of 
            // that first. (It happened in Sentani, due to network timeouts and such.)
            SynchProgressDlg.Integrity = SynchProgressDlg.GetStartState();
            bool bResult = true;
            int cHeads = GetHeadsCount();
            if (cHeads == 0)
            {
                SynchProgressDlg.ShowError("kNoMercurial",
                    "The external program Mercurial did not respond.\n\n" +
                    "Either it is not installed, or you have a corrupt repository. Refer to the Help file.");
                SynchProgressDlg.Stop();
                return false;
            }
            if (cHeads > 1)
                bResult = Commit("Committing unresolved merge", false);
            SynchProgressDlg.Integrity = SynchProgressDlg.GetFinishState(bResult);
                
            // Commit if we have any changed files
            SynchProgressDlg.StoringRecentChanges = SynchProgressDlg.GetStartState();
            if (GetChangedFiles().Count > 0)
            {
                bResult = Commit("Synching from " + DB.UserName + " " +
                    DateTime.UtcNow.ToString("u", DateTimeFormatInfo.InvariantInfo), true);
            }
            SynchProgressDlg.StoringRecentChanges = SynchProgressDlg.GetFinishState(bResult);

            // Pull the changes from Theirs
            SynchProgressDlg.Pulling = SynchProgressDlg.GetStartState();
            if (!PullFrom(sTheirPath))
            {
                // TODO: TEMPORARY FOR YAWA INTALLATION
                SynchProgressDlg.ShowError("kUnableToPull",
                    "Sabda Kita tidak bisa menirimah dari Internet.\n\n" +
                    "Computer yang ada di America mungkin tidak jalan. Tungguh dulu, lalu coba lagi.");

                //SynchProgressDlg.ShowError("kUnableToPull",
                //    "OurWord is unable to retrieve changes from the Internet. \n\n" +
                //    "The remote computer may not be working. Please try again sometime later.");
                SynchProgressDlg.Stop();
                return false;
            }
            SynchProgressDlg.Pulling = SynchProgressDlg.GetFinishState(true);

            // Merge anything that needs it. Setting HGMERGE causes Chorus to be called.
            SynchProgressDlg.Merging = SynchProgressDlg.GetStartState();
            string sPathToChorusMerge = SurroundWithQuotes(
                Path.Combine(Other.DirectoryOfExecutingAssembly, "ChorusMerge.exe"));
            using (new ShortTermEnvironmentalVariable("HGMERGE", sPathToChorusMerge))
            {
                ExecutionResult result = Execute(c_sMerge);
                bResult = result.Successful;
            }
            // TODO: Merge gives strange error messages, e.g., its an error if there was
            // nothing to merge. So for Yawa, we'll just use true until we can figure 
            // out what bResult we really want to place here.
            SynchProgressDlg.Merging = SynchProgressDlg.GetFinishState(true);

            // Commit the result of the merge
            SynchProgressDlg.StoringMergeResults = SynchProgressDlg.GetStartState();
            bResult = Commit("Merged by " + DB.UserName + " " +
                DateTime.UtcNow.ToString("u", DateTimeFormatInfo.InvariantInfo), 
                false);
            SynchProgressDlg.StoringMergeResults = SynchProgressDlg.GetFinishState(bResult);

            // Push the merge to the destination repository
            SynchProgressDlg.Pushing = SynchProgressDlg.GetStartState();
            bResult = PushTo(sTheirPath);
            SynchProgressDlg.Pushing = SynchProgressDlg.GetFinishState(bResult);

            // Update our current working directory
            Update();

            // Remove the dialog if all was well; keep it around if there was even one
            // problem so he can see it.
            if (!SynchProgressDlg.HadProblem)
                SynchProgressDlg.Stop();
            else
                SynchProgressDlg.EnableOkButton();
            return true;
        }
        #endregion
        ****/
        #endregion
    }


    #region CLASS: ShortTermEnvironmentalVariable
    public class ShortTermEnvironmentalVariable : IDisposable
    {
        private readonly string _name;
        private string oldValue;

        public ShortTermEnvironmentalVariable(string name, string value)
        {
            _name = name;
            oldValue = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(_name, oldValue);
        }
    }
    #endregion
    #region CLASS: Other
    public class Other
    {
        public static string DirectoryOfExecutingAssembly
        {
            get
            {
                string path;
                bool unitTesting = Assembly.GetEntryAssembly() == null;
                if (unitTesting)
                {
                    path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
                    path = Uri.UnescapeDataString(path);
                }
                else
                {
                    path = Assembly.GetExecutingAssembly().Location;
                }
                return Directory.GetParent(path).FullName;
            }
        }

        protected static string GetTopAppDirectory()
        {
            string path;

            path = DirectoryOfExecutingAssembly;

            if (path.ToLower().IndexOf("output") > -1)
            {
                //go up to output
                path = Directory.GetParent(path).FullName;
                //go up to directory containing output
                path = Directory.GetParent(path).FullName;
            }
            return path;
        }
    }
    #endregion
}
