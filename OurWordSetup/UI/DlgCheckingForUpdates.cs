﻿#region ***** DlgCheckingForUpdates.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgCheckingForUpdates.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Progress dialog for initial part of Check for Updates process
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace OurWordSetup.UI
{
    public partial class DlgCheckingForUpdates : Form
    {
        static public bool QuietMode { get; set; }

        #region Attr{s}: string StatusMessage
        private string StatusMessage
        {
            set
            {
                m_labelStatus.Text = value;
            }
        }
        #endregion
        #region SMethod: void SetStatusText(string sEnglishStatus)
        delegate void SetStatusTextDelegate(string sEnglishStatus);
        static public void SetStatusText(string sStatus)
        {
            if (QuietMode)
                return;

            if (s_Dialog.InvokeRequired)
            {
                var d = new SetStatusTextDelegate(SetStatusText);
                s_Dialog.Invoke(d, new object[] { sStatus });
            }
            else
            {
                s_Dialog.StatusMessage = sStatus;
            }
        }
        #endregion

        #region Constructor()
        public DlgCheckingForUpdates()
        {
            InitializeComponent();
        }
        #endregion

        // Startup / Shutdown ----------------------------------------------------------------
        static private DlgCheckingForUpdates s_Dialog;
        private static Form s_ParentWindow;
        static public string InformationMessage;
        #region SMethod: public void Start()
        static public void Start(Form parent)
        {
            if (QuietMode)
                return;

            s_Dialog = null;
            s_ParentWindow = parent;

            var t = new Thread(new ThreadStart(DlgCheckingForUpdates.StartDialog))
                        {
                            IsBackground = true,
                            Name = "CheckForUpdates Window"
                        };
            t.Start();

            while (null == s_Dialog)
                Thread.Sleep(500);
            Thread.Sleep(2000);
        }
        #region SMethod: void StartDialog()
        static private void StartDialog()
        {
            s_Dialog = new DlgCheckingForUpdates();

            // Override the default message of "OurWord is checking the Internet for updates..."
            if (!string.IsNullOrEmpty(InformationMessage))
                s_Dialog.m_labelIsChecking.Text = InformationMessage;

            if (null != s_ParentWindow)
            {
                var xLeft = s_ParentWindow.Left + s_ParentWindow.Width/2 - s_Dialog.Width/2;
                var yTop = s_ParentWindow.Top + s_ParentWindow.Height/2 - s_Dialog.Height/2;
                s_Dialog.Location = new Point(xLeft, yTop);
            }

            Application.Run(s_Dialog);
        }
        #endregion
        #endregion
        #region SMethod: void Stop()
        delegate void StopDelegate();
        static public void Stop()
        {
            if (QuietMode)
                return;

            if (s_Dialog.InvokeRequired)
            {
                var d = new StopDelegate(Stop);
                s_Dialog.Invoke(d);
            }
            else
            {
                s_Dialog.Close();
            }
        }
        #endregion
    }
}