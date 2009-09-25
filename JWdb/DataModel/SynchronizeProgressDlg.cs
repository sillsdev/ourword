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
        public enum steps
        {
            InternetAccess,
            Integrity,
            StoringChanges,
            Pulling,
            Merging,
            StoringMerge,
            Pushing
        };
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

        #region SMethod: void SetStepStart(steps step)
        static public void SetStepStart(steps step)
        {
            SetStep(step, states.InProcess);
        }
        #endregion
        #region SMethod: void SetStepFailed(steps step)
        static public void SetStepFailed(steps step)
        {
            SetStep(step, states.Failure);
            s_bHadProblem = true;
        }
        #endregion
        #region SMethod: void SetStepSuccess(steps step)
        static public void SetStepSuccess(steps step)
        {
            SetStep(step, states.Complete);
        }
        #endregion
        #region SMethod: void SetStep(steps step, states state)
        static void SetStep(steps step, states state)
        {
            if (null == s_Dlg)
                return;

            switch (step)
            {
                case steps.InternetAccess:
                    s_Dlg.SetPicture(s_Dlg.m_pictInternetAccess, state);
                    break;

                case steps.Integrity:
                    s_Dlg.SetPicture(s_Dlg.m_pictIntegrity, state);
                    break;

                case steps.StoringChanges:
                    s_Dlg.SetPicture(s_Dlg.m_pictStoringRecentChanges, state);
                    break;

                case steps.Pulling:
                    s_Dlg.SetPicture(s_Dlg.m_pictPulling, state);
                    break;

                case steps.Merging:
                    s_Dlg.SetPicture(s_Dlg.m_pictMerging, state);
                    break;

                case steps.StoringMerge:
                    s_Dlg.SetPicture(s_Dlg.m_pictStoringMergeResults, state);
                    break;

                case steps.Pushing:
                    s_Dlg.SetPicture(s_Dlg.m_pictPush, state);
                    break;
            }
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
        #region SMethod: void SetMode(bool bClone)
        static public void SetMode(bool bClone)
        {
            if (null == s_Dlg)
                return;

            // If cloning, then we hide a bunch of controls
            bool bShow = (bClone) ? false : true;

            // Show/hide the controls
            s_Dlg.m_labelIntegrity.Visible = bShow;
            s_Dlg.m_pictIntegrity.Visible = bShow;

            s_Dlg.m_labelStoringRecentChanges.Visible = bShow;
            s_Dlg.m_pictStoringRecentChanges.Visible = bShow;

            s_Dlg.m_labelMerge.Visible = bShow;
            s_Dlg.m_pictMerging.Visible = bShow;

            s_Dlg.m_labelStoringMergeResults.Visible = bShow;
            s_Dlg.m_pictStoringMergeResults.Visible = bShow;

            s_Dlg.m_labelPush.Visible = bShow;
            s_Dlg.m_pictPush.Visible = bShow;

            // Pulling position depends on valuel
            int yDiff = (s_Dlg.m_pictPush.Location.Y - s_Dlg.m_pictStoringMergeResults.Location.Y);
            if (bClone)
            {
                s_Dlg.m_labelPulling.Location = new Point(
                    s_Dlg.m_labelPulling.Location.X,
                    s_Dlg.m_labelInternetAccess.Location.Y + yDiff);
                s_Dlg.m_pictPulling.Location = new Point(
                    s_Dlg.m_pictPulling.Location.X,
                    s_Dlg.m_pictInternetAccess.Location.Y + yDiff);
            }
            else
            {
                s_Dlg.m_labelPulling.Location = new Point(
                    s_Dlg.m_labelPulling.Location.X,
                    s_Dlg.m_labelStoringRecentChanges.Location.Y + yDiff);
                s_Dlg.m_pictPulling.Location = new Point(
                    s_Dlg.m_pictPulling.Location.X,
                    s_Dlg.m_pictStoringRecentChanges.Location.Y + yDiff);
            }

            // Overall label depends on operation
            s_Dlg.m_labelHeader.Text = (bClone) ?
                "Please wait while OurWord downloads your data from the Internet" :
                "Please wait while OurWord synchronizes your data with the Internet"; 
        }
        #endregion

        static bool s_bCloneMode = false;

        #region SMethod: public void Start(bCloneMode)
        static public void Start(bool bCloneMode)
        {
            s_bHadProblem = false;
            s_bCloneMode = bCloneMode;

            Thread t = new Thread(new ThreadStart(SynchProgressDlg.StartDialog));
            t.IsBackground = true;
            t.Name = "Synchronize Progress";
            //t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        static private void StartDialog()
        {
            s_Dlg = new SynchProgressDlg();
            SetMode(s_bCloneMode);
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
