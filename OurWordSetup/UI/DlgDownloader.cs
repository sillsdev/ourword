#region ***** DlgDownloader.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgDownloader.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Downloads the files to the Download folder
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using OurWordSetup.Data;
#endregion

namespace OurWordSetup.UI
{
    public partial class DlgDownloader : Form
    {
        // Download Items --------------------------------------------------------------------
        #region Attr{g}: List<ManifestItem> ItemsToDownload
        List<ManifestItem> ItemsToDownload
        {
            get
            {
                Debug.Assert(null != m_vItemsToDownload);
                return m_vItemsToDownload;
            }
        }
        private readonly List<ManifestItem> m_vItemsToDownload;
        #endregion
        private int m_iCurrentItem;
        #region Attr{g}: ManifestItem CurrentItem
        ManifestItem CurrentItem
        {
            get
            {
                return m_iCurrentItem == ItemsToDownload.Count ? 
                    null : ItemsToDownload[m_iCurrentItem];
            }
        }
        #endregion

        // Locations -------------------------------------------------------------------------
        #region SVattr{g}: string DownloadFolder
        static string DownloadFolder
        {
            get
            {
                return SetupManager.DownloadFolder;
            }
        }
        #endregion
        #region SMethod: Uri MakeRemoteUri(ManifestItem)
        static Uri MakeRemoteUri(ManifestItem item)
        {
            return new Uri(SetupManager.c_sRemoteWebSite + item.Filename);
        }
        #endregion
        #region SMethod: string MakeDestinationPath(ManifestItem)
        static string MakeDestinationPath(ManifestItem item)
        {
            return Path.Combine(DownloadFolder, item.Filename);
        }
        #endregion

        // Progress Bar ----------------------------------------------------------------------
        #region Attr{g}: int TotalKToDownload
        private int TotalKToDownload
        {
            get
            {
                var c = 0L;
                foreach (var item in ItemsToDownload)
                    c += item.Length;
                return (int)(c / 1000);
            }
        }
        #endregion
        private int m_nKReceivedThusFar;

        // Scaffolding -----------------------------------------------------------------------
        #region Constuctor(vItemsToDownload)
        public DlgDownloader(List<ManifestItem> vItemsToDownload)
        {
            InitializeComponent();

            m_vItemsToDownload = vItemsToDownload;

            DialogResult = DialogResult.OK;
        }
        #endregion
        public bool DownloadCanceledByUser { get; private set; }
        #region Attr{g}: string PleaseWaitMessage
        public string PleaseWaitMessage
        {
            set
            {
                m_labelPleaseWait.Text = value;
            }
        }
        #endregion

        // Events ----------------------------------------------------------------------------
        #region Cmd: cmdFormFirstShown
        private void cmdFormFirstShown(object sender, EventArgs e)
        {
            ClearOutDownloadFolder();

            // Setup the progress bar
            m_ProgressBar.Maximum = TotalKToDownload;
            m_ProgressBar.Minimum = 0;

            // Now that the form is visible, start the downloads. These are 
            // asynchronus, so as each finishes it starts the next one, with
            // the status bar being updated at each one.
            m_iCurrentItem = 0;
            DownloadNextFile();
        }
        #endregion
        #region Cmd: cmdCancel
        private void cmdCancel(object sender, EventArgs e)
        {
            DownloadCanceledByUser = true;
        }
        #endregion

        // Work Methods ----------------------------------------------------------------------
        #region Method: void ClearOutDownloadFolder()
        static void ClearOutDownloadFolder()
            // Remove anything currently in the downloads folder; we could have old
            // files from a previously canceled download taking up space.
            // 
            // We don't want to remove the newly-downloaded manifest file, though.
            // Nor the Setup program, because we might not necessarily be downloading
            // a new setup program.
        {
            var vsFilePaths = Directory.GetFiles(DownloadFolder);
            foreach(var sFilePath in vsFilePaths)
            {
                var sFilename = Path.GetFileName(sFilePath);
                if (sFilename != Manifest.ManifestFileName &&
                    sFilename != SetupManager.c_sOurWordSetupFileName)
                {
                    File.Delete(sFilePath);
                }
            }

            // Also clear out any subfolders
            var vsSubfolders = Directory.GetDirectories(DownloadFolder);
            foreach(var sSubfolder in vsSubfolders)
                Directory.Delete(sSubfolder);
        }
        #endregion
        #region Method: void SetStatus(string sText)
        void SetStatus(string sText)
        {
            m_labelCurrentStatus.Text = sText;
        }
        #endregion

        // Async File Download Sequence ------------------------------------------------------
        #region Method: void DownloadNextFile()
        void DownloadNextFile()
        {
            if (null == CurrentItem)
            {
                Close();
                return;
            }

            var web = new WebClient();
            web.DownloadProgressChanged += FileDownloadProgressChanged;
            web.DownloadFileCompleted += FileDownloadCompleted;
            web.DownloadFileAsync(
                MakeRemoteUri(CurrentItem),
                MakeDestinationPath(CurrentItem));

            SetFileDownloadStatusMessage(0, 0);
        }
        #endregion
        #region Handler: FileDownloadProgressChanged
        void FileDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var nTotalK = e.TotalBytesToReceive/1000;

            m_ProgressBar.Value = m_nKReceivedThusFar + (int)e.BytesReceived/1000;

            SetFileDownloadStatusMessage(e.ProgressPercentage, nTotalK);

            if (!DownloadCanceledByUser) 
                return;

            var web = sender as WebClient;
            if (null != web)
                web.CancelAsync();
        }
        #endregion
        #region Handler: FileDownloadCompleted
        void FileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                SetupManager.DisplayMessage(this, 
                    "An error occurred while downloading:\n" +
                    "(" + e.Error.Message + ")\n" +
                    "Please try again.");
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            /* 
             * GOOD IDEA, But doesn't want to work. Apparently the file gets changed
             * somehow as a result of the download. Need to research it. I'd likek to
             * have some sort of checksum working for this.
             * 
            // If the computed hash doesn't match, then it was corrupted during the
            // download
            var hash = Manifest.ComputeHash(MakeDestinationPath(CurrentItem));
            if (hash != CurrentItem.Hash)
            {
                const string sMessage = 
                    "File {0} appears to have been corrupted during the download.\n" + 
                    "Please try again. If the problem continues, please notify us as a \n" + 
                    "corrupt file may have been stored on the Internet.";
                SetupManager.DisplayMessage(this, string.Format(sMessage, CurrentItem.Filename));
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }
            */

            if (DownloadCanceledByUser)
            {
                SetupManager.DisplayMessage(this, 
                    "The download was canceled. Installation did not finish.");
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            m_nKReceivedThusFar += (int)CurrentItem.Length/1000;

            // Move on to the next one
            m_iCurrentItem++;
            DownloadNextFile();
        }
        #endregion
        #region Method: void SetFileDownloadStatusMessage(nProgressPercent, nTotalK)
        private void SetFileDownloadStatusMessage(int nProgressPercent, long nTotalK)
        {
            var sStatus = string.Format("Status: Downloading file {0}/{1}:  {2}...",
                m_iCurrentItem + 1, ItemsToDownload.Count, CurrentItem.Filename);

            if (nProgressPercent > 0)
            {
                sStatus = string.Format("{0}  {1}% of {2}K",
                    sStatus, nProgressPercent, nTotalK);
            }

            SetStatus(sStatus);
        }
        #endregion
    }
}