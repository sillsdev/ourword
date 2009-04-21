#region ***** SynchProgressDlg.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    SynchProgressDlg.cs
 * Author:  John Wimbish
 * Created: 25 March 2009
 * Purpose: Show progress to the user during a synch
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using JWTools;
#endregion
#endregion

namespace JWdb.DataModel
{
    public partial class SynchProgressDlg : Form
    {
        #region Constructor()
        public SynchProgressDlg()
        {
            InitializeComponent();
        }
        #endregion

        static SynchProgressDlg s_Dlg;

        // Status ----------------------------------------------------------------------------
        public enum states { Pending, InProcess, Complete, Failure };

        #region Method: void SetPicture(PictureBox, states NewState)
        delegate void cbSetPicture(PictureBox pict, states state);
        void SetPicture(PictureBox pict, states NewState)
        {
            if (InvokeRequired)
            {
                var d = new cbSetPicture(SetPicture);
                s_Dlg.Invoke(d, new object[] { pict, NewState });
            }
            else
            {
                if (NewState == states.Pending)
                    pict.Image = JWU.GetBitmap("BlueDot.ico");
                else if (NewState == states.InProcess)
                    pict.Image = JWU.GetBitmap("Project.ico");
                else if (NewState == states.Complete)
                    pict.Image = JWU.GetBitmap("Check.ico");
                else if (NewState == states.Failure)
                    pict.Image = JWU.GetBitmap("Failure.ico");
            }
        }
        #endregion

        #region SAttr{s}: states InternetAccess
        static public states InternetAccess
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictInternetAccess, value);
            }
        }
        #endregion
        #region SAttr{s}: states Integrity
        static public states Integrity
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictIntegrity, value);
            }
        }
        #endregion
        #region SAttr{s}: states StoringRecentChanges
        static public states StoringRecentChanges
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictStoringRecentChanges, value);
            }
        }
        #endregion
        #region SAttr{s}: states Pulling
        static public states Pulling
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictPulling, value);
            }
        }
        #endregion
        #region SAttr{s}: states Merging
        static public states Merging
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictMerging, value);
            }
        }
        #endregion
        #region SAttr{s}: states StoringMergeResults
        static public states StoringMergeResults
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictStoringMergeResults, value);
            }
        }
        #endregion
        #region SAttr{s}: states Pushing
        static public states Pushing
        {
            set
            {
                if (null != s_Dlg)
                    s_Dlg.SetPicture(s_Dlg.m_pictPush, value);
            }
        }
        #endregion

        #region SMethod: states GetStartState()
        static public states GetStartState()
        {
            return states.InProcess;
        }
        #endregion
        #region SMethod: states GetFinishState(bool bResult)
        static public states GetFinishState(bool bResult)
        {
            // If true, then the caller was successful
            if (bResult)
                return states.Complete;

            // Otherwise, a problem was encountered
            s_bHadProblem = true;
            return states.Failure;
        }
        #endregion

        // Error Messages: do it here so it is on top of the Synch dialog --------------------
        #region SAttr{g}: bool HadProblem
        static public bool HadProblem
        {
            get
            {
                return s_bHadProblem;
            }
        }
        static bool s_bHadProblem = false;
        #endregion
        #region SMethod: void ShowError(string sID, string sDefaultText)
        delegate void cbShowError(string sID, string sDefaultText);
        static public void ShowError(string sID, string sDefaultText)
        {
            // Shouldn't happen
            if (null == s_Dlg)
                return;

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbShowError(ShowError);
                s_Dlg.Invoke(d, new object[] { sID, sDefaultText });
            }
            else
            {
                LocDB.Message(sID, sDefaultText, null, LocDB.MessageTypes.Error);
            }
        }
        #endregion

        // Start/Stop the dialog -------------------------------------------------------------
        #region SAttr{g}: bool IsCreated
        static public bool IsCreated
        {
            get
            {
                return (null != s_Dlg);
            }
        }
        static public bool IsShowing = false;
        #endregion
        #region SMethod: public void Start()
        static public void Start()
        {
            s_bHadProblem = false;

            Thread t = new Thread(new ThreadStart(SynchProgressDlg.StartDialog));
            t.IsBackground = true;
            t.Name = "Synchronize Progress";
            //t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        static private void StartDialog()
        {
            s_Dlg = new SynchProgressDlg();
            Application.Run(s_Dlg);
        }
        #endregion
        #region SMethod: void Stop()
        delegate void cbStop(); 
        static public void Stop()
        {
            if (s_Dlg.InvokeRequired)
            {
                var d = new cbStop(Stop);
                s_Dlg.Invoke(d);
            }
            else
            {
                s_Dlg.Close();
            }
        }
        #endregion

        #region SMethod: void EnableOkButton()
        delegate void cbEnableOkButton();
        static public void EnableOkButton()
        {
            if (s_Dlg.InvokeRequired)
            {
                var d = new cbEnableOkButton(EnableOkButton);
                s_Dlg.Invoke(d);
            }
            else
            {
                s_Dlg.m_btnCancel.Text = "OK";
                s_Dlg.m_btnCancel.Enabled = true;
            }
        }
        #endregion

        #region Cmd: cmdDone
        private void cmdDone(object sender, EventArgs e)
        {
            SynchProgressDlg.Stop();
        }
        #endregion
    }

}
