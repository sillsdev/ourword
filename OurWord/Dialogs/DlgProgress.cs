#region ***** DlgProgress.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgProgress.cs
 * Author:  John Wimbish
 * Created: 26 July 2007
 * Purpose: Progress dialog, allowing cancelation, for the Export process
 * Legal:   Copyright (c) 2005-11, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Windows.Forms;
using System.Threading;
#endregion

namespace OurWord.Dialogs
{
    public partial class DlgProgress : Form
    {
        // Scaffolding -----------------------------------------------------------------------
        #region constructor()
        private DlgProgress()
        {
            InitializeComponent();
        }
        #endregion
        static DlgProgress s_Dlg;

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: bool UserSaysCancel
        static public bool UserSaysCancel
        {
            get
            {
                return m_bUserSaysCancel;
            }
        }
        static bool m_bUserSaysCancel;
        #endregion

        // Dialog Attrs / Text ---------------------------------------------------------------
        #region smethod: void SetTitle(sTitle)
        delegate void cbSetTitle(string sTitle);
        static public void SetTitle(string sTitle)
        {
            if (null == s_Dlg)
                return;

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbSetTitle(SetTitle);
                s_Dlg.Invoke(d, new object[] { sTitle });
            }
            else
            {
                s_Dlg.Text = sTitle;
            }
        }
        #endregion
        #region smethod: void SetExplanation(sExplanation)
        delegate void cbSetExplanation(string sExplanation);
        static public void SetExplanation(string sExplanation)
        {
            if (null == s_Dlg)
                return;

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbSetExplanation(SetExplanation);
                s_Dlg.Invoke(d, new object[] { sExplanation });
            }
            else
            {
                s_Dlg.m_labelHeader.Text = sExplanation;
            }
        }
        #endregion

        // Current Activity ------------------------------------------------------------------
        delegate void cbSetCurrentBook(string sText);
        #region Method: void SetCurrentBook(string sText)
        static public void SetCurrentActivity(string sText)
        {
            if (null == s_Dlg)
                return;

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbSetCurrentBook(SetCurrentActivity);
                s_Dlg.Invoke(d, new object[] { sText });
            }
            else
            {
                s_Dlg.m_labelCurrentActivity.Text = sText;
            }
        }
        #endregion

        // Progress Bar ----------------------------------------------------------------------
        delegate void cbSetProgressValue(int n);
        #region SMethod: void SetProgressMax(cMaxValue)
        static public void SetProgressMax(int cMaxValue)
        {
            // Give the Starter thread time to get this dialog lauched
            while(null == s_Dlg)
                Thread.Sleep(100);

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbSetProgressValue(SetProgressMax);
                s_Dlg.Invoke(d, new object[] { cMaxValue });
            }
            else
            {
                s_Dlg.m_ProgressBar.Minimum = 0;
                s_Dlg.m_ProgressBar.Maximum = cMaxValue;
                s_Dlg.m_ProgressBar.Value = 0;
            }
        }
        #endregion
        #region SMethod: void IncrementProgressValue(int nAmout)
        static public void IncrementProgressValue(int nAmout)
        {
            if (null == s_Dlg)
                return;

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbSetProgressValue(IncrementProgressValue);
                s_Dlg.Invoke(d, new object[] { nAmout });
            }
            else
            {
                s_Dlg.m_ProgressBar.Value += nAmout;
            }
        }
        #endregion

        // Start/Stop the dialog -------------------------------------------------------------
        #region SMethod: public void Start()
        static public void Start()
        {
            m_bUserSaysCancel = false;
            var t = new Thread(new ThreadStart(DlgProgress.StartDialog))
                {
                    IsBackground = true,
                    Name = "Progress"
                };
            t.Start();

            // Give the Starter thread time to get this dialog lauched
            while (null == s_Dlg)
                Thread.Sleep(100);
        }
        static private void StartDialog()
        {
            s_Dlg = new DlgProgress();
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
                s_Dlg = null;
            }
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdCancel
        private void cmdCancel(object sender, EventArgs e)
        {
            m_bUserSaysCancel = true;
        }
        #endregion
    }


}
