#region ***** SetupManager.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    SetupManager.cs
 * Author:  John Wimbish
 * Created: 30 Jan 2010
 * Purpose: Handles setup and automatic updates operations
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using OurWordSetup.UI;
#endregion

namespace OurWordSetup.Data
{
    public class SetupManager
    {
        // Locations and Files ---------------------------------------------------------------
        public const string c_sRemoteWebSite = "http://ourwordsoftware.org/Download/Program/";
        public const string c_sOurWordSetupFileName = "SetupOurWord.exe";
        public const string c_sOurWordApplication = "OurWord.exe";
        #region SVattr{g}: string ApplicationsFolder
        public static string ApplicationsFolder
        {
            get
            {
                var sBaseFolder = Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

                var sApplicationsFolder = Path.Combine(sBaseFolder, "OurWord");
                if (!Directory.Exists(sApplicationsFolder))
                    Directory.CreateDirectory(sApplicationsFolder);

                return sApplicationsFolder;
            }
        }
        #endregion
        #region SVattr{g}: string DownloadFolder
        public static string DownloadFolder
        {
            get
            {
                var sDownloadFolder = Path.Combine(ApplicationsFolder, "Download");
                if (!Directory.Exists(sDownloadFolder))
                    Directory.CreateDirectory(sDownloadFolder);

                return sDownloadFolder;
            }
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        private bool QuietMode { get; set; }
        #region Attr{g}: Form ParentWindow
        Form ParentWindow
        {
            get
            {
                Debug.Assert(null != m_ParentWindow);
                return m_ParentWindow;
            }
        }
        private readonly Form m_ParentWindow;
        #endregion

        // Manifests for both current and remote ---------------------------------------------
        #region Attr{g}: Manifest RemoteManifest
        Manifest RemoteManifest
        {
            get
            {
                Debug.Assert(null != m_RemoteManifest);
                return m_RemoteManifest;
            }
        }
        private readonly Manifest m_RemoteManifest;
        #endregion
        #region Attr{g}: Manifest LocalManifest
        Manifest LocalManifest
        {
            get
            {
                Debug.Assert(null != m_LocalManifest);
                return m_LocalManifest;
            }
        }
        private readonly Manifest m_LocalManifest;
        #endregion

        // Public Interface ------------------------------------------------------------------
        private const int c_nUserAborted = -2;
        private const int c_nError = -1;
        private const int c_nNoUpdatedNeeded = 0;
        private const int c_nUpdateLaunched = 1;

        #region Constructor(parentWindow)
        public SetupManager(Form parentWindow)
        {
            m_ParentWindow = parentWindow;

            m_LocalManifest = new Manifest(Path.Combine(ApplicationsFolder, 
                Manifest.ManifestFileName));
            m_RemoteManifest = new Manifest(Path.Combine(DownloadFolder,
                Manifest.ManifestFileName));
        }
        #endregion
        #region SMethod: int CheckForUpdates(parentForm, bInformUserIfThereWereNoUpdates)
        static public int CheckForUpdates(Form parentForm, bool bQuietMode)
            // Entry point called from OurWord
        {
            var manager = new SetupManager(parentForm) 
            {
                QuietMode = bQuietMode 
            };
            return manager.DoCheckForUpdates();
        }
        #endregion
        #region Method: int DoCheckForUpdates()
        private int DoCheckForUpdates()
            // Returns true if we're launching the external updater (and thus are shutting down),
            // as the caller may need to abort further processing. (See Synchronize for example)
            //
            // Returns:
            //   0 - No update required
            //  -1 - User canceled form an update
            //   1 - Update is being done, caller needs to shut down
        {
            #region UI Strings
            var ui = new CommonUserInterfaceStrings
            {
                PleaseWaitWhileChecking = "OurWord is checking the Internet for updates...",
                CheckingInternetAccessStatus = "Checking Internet access...",
                NoInternetConnectionErrorMessage =
                    "OurWord was unable to check for updates,\n" +
                    "There is no connection to the Internet.",
                DownloadingManifestStatus = "Checking if an update is available...",
                CantDownloadManifestErrorMessage =
                    "OurWord was unable to check for updates,\n" +
                    "The OurWord Internet site did not respond.\n" +
                    "Please try again later.",
                CantReadManifestFileErrorMessage =
                    "OurWord was unable to check for updates,\n" +
                    "The version returned from the Internet was incomplete.\n" +
                    "Please try again later.",
                PleaseWaitWhileDownloading = 
                    "Please wait while OurWord downloads and installs the update."
            };
            #endregion

            // We need the Manifest in order to know if an update is available
            if(!CheckInternetAccessAndDownloadManifest(ui))
                return c_nError;

            // For, e.g., synch, we will be silent here and not tell the user there
            // is no need; but if they explicitly ask to check for updates, then a
            // message needs to be presented. 
            if (!IsAnUpdateIndicated())
            {
                if (QuietMode)
                    DisplayMessage(ParentWindow, "Your version of OurWord is up-to-date.");
                return c_nNoUpdatedNeeded;
            }

            // Give the user opportunity to bail
            if (!DoesTheUserWantToUpdateNow())
                return c_nUserAborted;

            // Download the files
            var vItemsToDownload = DetermineItemsToDownload();
            var downloader = new DlgDownloader(vItemsToDownload) 
                { PleaseWaitMessage = ui.PleaseWaitWhileDownloading };
            downloader.ShowDialog(ParentWindow);
            if (downloader.DownloadCanceledByUser)
                return c_nUserAborted;

            // At this point, files are downloaded. We now need to shut down and 
            // launch another app to copy the files into the application folder
            LaunchExternalSetup();
            return c_nUpdateLaunched;
        }
        #endregion
        #region Method: void FinishUpdate()
        public void FinishUpdate()
        {
            // We can't copy files into the App folder if OurWord is still running
            WaitForOurWordToClose();

            // Removes files from previous installs that we no longer use
            LocalManifest.ReadFile();
            RemoteManifest.ReadFile();
            RemoveStaleFilesFromPreviousInstall();

            // Move the new files from the Download folder to where they belong
            InstallDownloadedFiles();

            // Restart OurWord and exit
            LaunchOurWord();
            Application.Exit();
        }
        #endregion
        #region Method: bool DoFullSetup()
        public void DoFullSetup()
        {
            #region UI Strings
            var ui = new CommonUserInterfaceStrings
            {
                PleaseWaitWhileChecking = "OurWord is preparing for setup...",
                CheckingInternetAccessStatus = "Checking Internet access...",
                NoInternetConnectionErrorMessage = 
                    "OurWord was unable to download files needed for installation.\n" +
                    "There is no connection to the Internet.",
                DownloadingManifestStatus = "Determining what is needed for setup...",
                CantDownloadManifestErrorMessage = 
                    "OurWord was unable to download the necessary installation files,\n" +
                    "The OurWord Internet site did not respond.\n" +
                    "Please try again later.",
                CantReadManifestFileErrorMessage = 
                    "OurWord was unable to determine what to install,\n" +
                    "The version returned from the Internet was incomplete.\n" +
                    "Please try again later.",
                PleaseWaitWhileDownloading = 
                    "Please wait while OurWord downloads the necessary files."
            };
            #endregion

            // Clear out the App folder completely
            InitializeEmptyApplicationsFolder();

            // If we have an appropriately named zip file available, then we can bypass
            // downloading from the Internet
            if (ExpandZipIntoDownloadFolder())
            {
                RemoteManifest.ReadFile();
            }
            else
            {
                // We need the Manifest in order to know what to download
                if (!CheckInternetAccessAndDownloadManifest(ui))
                    return;

                // Download all of the files
                var downloader = new DlgDownloader(RemoteManifest) 
                    { PleaseWaitMessage = ui.PleaseWaitWhileDownloading };
                if (DialogResult.Abort == downloader.ShowDialog(ParentWindow))
                    return;
            }

            // Place them in the App folder
            InstallDownloadedFiles();

            // Launch OurWord and Exit
            LaunchOurWord();
            Application.Exit();
        }
        #endregion

        // CheckForUpdates Work Methods ------------------------------------------------------
        #region SMethod: bool IsAnUpdateIndicated()
        bool IsAnUpdateIndicated()
        {
            var ourVersion = CurrentlyRunningVersion;

            if (RemoteManifest.Major > ourVersion.Major)
                return true;
            if (RemoteManifest.Minor > ourVersion.Minor)
                return true;
            if (RemoteManifest.Build > ourVersion.Build)
                return true;

            return false;
        }
        #endregion
        #region SMethod: bool DoesTheUserWantToUpdateNow()
        bool DoesTheUserWantToUpdateNow()
        {
            var ourVersion = CurrentlyRunningVersion;
            var remoteVersion = RemoteManifest.Version;

            var dlg = new DlgDoYouWishToUpdate
            {
                OurVersion = BuildFriendlyVersion(ourVersion),
                RemoteVersion = BuildFriendlyVersion(remoteVersion)
            };

            return (dlg.ShowDialog(ParentWindow) == DialogResult.OK);
        }
        #endregion
        #region Method: List<ManifestItem> DetermineItemsToDownload()
        List<ManifestItem> DetermineItemsToDownload()
        {
            // If the local manifest is empty, then we must download everything
            LocalManifest.ReadFile();
            if (LocalManifest.Count == 0)
                return RemoteManifest;

            // Otherwise, we can compare the two manifests
            var v = new List<ManifestItem>();
            foreach(var itemRemote in RemoteManifest)
            {
                var itemLocal = LocalManifest.Find(itemRemote.Filename);

                // If item isn't in the local manifest, then we do want to download it
                if (itemLocal == null)
                {
                    v.Add(itemRemote);
                    continue;
                }

                // If the two have differing lengths or hashes, then we declare that they
                // are different files and need to be downloaded.
                if (itemLocal.Length != itemRemote.Length ||
                    itemLocal.Hash != itemRemote.Hash)
                {
                    v.Add(itemRemote);
                }

                // If it is the Setup app, and it is missing from the Download folder,
                // then we must download it or the update will not be able to finish.
                if (itemRemote.Filename == c_sOurWordSetupFileName && 
                    !File.Exists(Path.Combine(DownloadFolder, c_sOurWordSetupFileName)))
                {
                    v.Add(itemRemote);
                }
            }

            return v;
        }
        #endregion
        #region SMethod: void LaunchExternalSetup()
        static void LaunchExternalSetup()
        {
            var sSetupAppPath = Path.Combine(DownloadFolder, c_sOurWordSetupFileName);

            var info = new ProcessStartInfo(sSetupAppPath) 
            {
                Arguments = Program.c_sActionFinishUpdate
            };

            Process.Start(info);

            // We must exit OurWord, as Setup needs to be able to overwrite files
            // in the App folder
            Application.Exit();
        }
        #endregion

        // FinishUpdate Work Methods ---------------------------------------------------------
        #region SMethod: void WaitForOurWordToClose()
        private static void WaitForOurWordToClose()
        {
            const string ourWordNormalProcess = "ourword";
            const string ourWordVisualStudioProcess = "ourword.vshost";

            // Find the process
            var vProcesses = Process.GetProcesses();
            Process ourWordProcess = null;
            foreach (var process in vProcesses)
            {
                var s = process.ProcessName.ToLower();
                if (s == ourWordVisualStudioProcess || s == ourWordNormalProcess)
                    ourWordProcess = process;
            }

            // Already ended
            if (null == ourWordProcess)
                return;

            // Wait for the process to end
            var cHalfSecondsWaited = 0;
            var cHalfSecondsToWait = (ourWordProcess.ProcessName.ToLower() == ourWordVisualStudioProcess) ? 1 : 20;
            while (!ourWordProcess.HasExited)
            {
                Thread.Sleep(500);

                cHalfSecondsWaited++;
                if (cHalfSecondsWaited <= cHalfSecondsToWait)
                    continue;

                ourWordProcess.Kill();
                ourWordProcess.WaitForExit();
            }
        }
        #endregion
        #region Method: void InstallDownloadedFiles()
        void InstallDownloadedFiles()
        {
            foreach(var item in RemoteManifest)
            {
                // Source and Destination paths
                var sDownloadPath = Path.Combine(DownloadFolder, item.Filename);
                var sInstallPath = Path.Combine(ApplicationsFolder, item.Filename);

                // Leave the Setup program where it is (we're running it, anyway, so
                // it would generate an error if we tried to move it!)...but make a
                // copy to the App folder, too, as OW needs it when it does an update
                if (item.Filename == c_sOurWordSetupFileName)
                {
                    File.Copy(sDownloadPath, sInstallPath, true);
                    continue;
                }

                // If we don't have the file in the Download folder, then it didn't need
                // to be updated
                if (!File.Exists(sDownloadPath))
                    continue;

                // Delete the file in the app folder
                if (File.Exists(sInstallPath))
                    File.Delete(sInstallPath);

                // Move the downloaded file to the app folder
                File.Move(sDownloadPath, sInstallPath);

                // Extract any zipped contents
                if (Zip.IsZipFile(sInstallPath))
                    (new Zip(sInstallPath)).Extract();
            }

            // Copy the Remote Manifest over, it is now the Local Manifest
            var sDownloadedManifest = Path.Combine(DownloadFolder, Manifest.ManifestFileName);
            var sLocalManifest = Path.Combine(ApplicationsFolder, Manifest.ManifestFileName);
            if (File.Exists(sLocalManifest))
                File.Delete(sLocalManifest);
            File.Move(sDownloadedManifest, sLocalManifest);
        }
        #endregion
        #region SMethod: void LaunchOurWord()
        static private void LaunchOurWord()
        {
            var ourWordAppPath = Path.Combine(ApplicationsFolder, c_sOurWordApplication);
            if (!File.Exists(ourWordAppPath))
                return;

            var info = new ProcessStartInfo(ourWordAppPath);
            Process.Start(info);
        }
        #endregion

        // FullSetup Work Methods ------------------------------------------------------------
        #region Method: void InitializeEmptyApplicationsFolder()
        static void InitializeEmptyApplicationsFolder()
        {
            if (Directory.Exists(ApplicationsFolder))
                Directory.Delete(ApplicationsFolder, true);

            Directory.CreateDirectory(ApplicationsFolder);

            if (!Directory.Exists(DownloadFolder))
                Directory.CreateDirectory(DownloadFolder);

            return;
        }
        #endregion
        #region SMethod: bool ExpandZipIntoDownloadFolder()
        static bool ExpandZipIntoDownloadFolder()
        {
            // Check for an appropriately named zip file in this SetupManager's folder
            var sSetupAssemblyPath = Assembly.GetAssembly(typeof(SetupManager)).Location;
            var sSetupAssemblyFolder = Path.GetDirectoryName(sSetupAssemblyPath);
            var sZipPath = Path.Combine(sSetupAssemblyFolder, Manifest.AllFilesZipFileName);
            if (!File.Exists(sZipPath))
                return false;

            // Extract to the Downloads folder
            var zip = new Zip(sZipPath);
            zip.Extract(DownloadFolder);

            return true;
        }
        #endregion

        // Common Helper Methods -------------------------------------------------------------
        #region Method: void DisplayMessage(parentWnd, sErrorText)
        static public void DisplayMessage(Form parent, string sErrorText)
        {
            MessageBox.Show(parent, sErrorText, "OurWord Setup",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        #region SMethod: string BuildFriendlyVersion(Version)
        static string BuildFriendlyVersion(Version v)
        {
            var chBuild = (char)('a' + v.Build);
            var sBuild = (v.Build == 0) ? "" : chBuild.ToString();

            return string.Format("{0}.{1}{2}", v.Major, v.Minor, sBuild);
        }
        #endregion
        #region Attr{g}: Version CurrentlyRunningVersion
        static Version CurrentlyRunningVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version;
            }
        }
        #endregion
        #region Method: void ClearOutApplicationFolder()
        void RemoveStaleFilesFromPreviousInstall()
            // We want to clear out stale files from old versions. That is, anything that
            // is not in the new (remote) manifest
        {
            var vsFilesToClearOut = RemoteManifest.GetStaleFiles(LocalManifest);
            foreach(var sPath in vsFilesToClearOut)
            {
                if (File.Exists(sPath))
                    File.Delete(sPath);
            }
        }
        #endregion
        #region struct CommonUserInterfaceStrings
        struct CommonUserInterfaceStrings
        {
            public string PleaseWaitWhileChecking;
            public string CheckingInternetAccessStatus;
            public string NoInternetConnectionErrorMessage;
            public string DownloadingManifestStatus;
            public string CantDownloadManifestErrorMessage;
            public string CantReadManifestFileErrorMessage;
            public string PleaseWaitWhileDownloading;
        }
        #endregion

        // CheckInternetAccessAndDownloadManifest --------------------------------------------
        #region Method: bool CheckInternetAccessAndDownloadManifest(ui)
        bool CheckInternetAccessAndDownloadManifest(CommonUserInterfaceStrings ui)
        {
            // Display a separate-threaded dialog telling the user what we're doing
            DlgCheckingForUpdates.QuietMode = QuietMode;
            DlgCheckingForUpdates.InformationMessage = ui.PleaseWaitWhileChecking;
            DlgCheckingForUpdates.Start(ParentWindow);

            // Abort if there's no Internet
            DlgCheckingForUpdates.SetStatusText(ui.CheckingInternetAccessStatus);
            if (!PingForLiveInternetConnection())
            {
                DlgCheckingForUpdates.Stop();
                DisplayMessage(ParentWindow, ui.NoInternetConnectionErrorMessage);
                return false;
            }

            // Abort if we can't download the Manifest file
            DlgCheckingForUpdates.SetStatusText(ui.DownloadingManifestStatus);
            if (!DownloadRemoteManifestFile())
            {
                DlgCheckingForUpdates.Stop();
                DisplayMessage(ParentWindow, ui.CantDownloadManifestErrorMessage);
                return false;
            }

            // Abort if the Remote Manifest file cannot parsed
            if (!ReadRemoteManifestFile())
            {
                DlgCheckingForUpdates.Stop();
                DisplayMessage(ParentWindow, ui.CantReadManifestFileErrorMessage);
                return false;
            }

            // Done with the process dialog
            DlgCheckingForUpdates.Stop();

            return true;
        }
        #endregion
        #region SMethod: bool PingForLiveInternetConnection()
        static bool PingForLiveInternetConnection()
        {
            try
            {
                var ping = new Ping();
                var reply = ping.Send("www.google.com");
                if (null == reply)
                    return false;
                return (reply.Status == IPStatus.Success);
            }
            catch {}
            return false;
        }
        #endregion
        #region Method: bool DownloadRemoteManifestFile()
        bool DownloadRemoteManifestFile()
        {
            try
            {
                const string sRemoteUrl = c_sRemoteWebSite + Manifest.ManifestFileName;

                // Make sure we actually download a file, rather than mistakenly
                // using a stale one.
                if (File.Exists(RemoteManifest.FilePath))
                    File.Delete(RemoteManifest.FilePath);

                var web = new WebClient();
                web.DownloadFile(sRemoteUrl, RemoteManifest.FilePath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
        #endregion
        #region Method: bool ReadRemoteManifestFile()
        bool ReadRemoteManifestFile()
        {
            // Read
            try
            {
                RemoteManifest.ReadFile();
                return true;
            }
            catch {}

            return false;
        }
        #endregion
    }
}
