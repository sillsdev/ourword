#region ***** DlgExportProgress.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgExportProgress.cs
 * Author:  John Wimbish
 * Created: 26 July 2007
 * Purpose: Progress dialog, allowing cancelation, for the Export process
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.View;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class DlgExportProgress : Form
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgExportProgress()
        {
            InitializeComponent();
        }
        #endregion
        static DlgExportProgress s_Dlg;

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: bool UserSaysCancel
        static public bool UserSaysCancel
        {
            get
            {
                return m_bUserSaysCancel;
            }
        }
        static bool m_bUserSaysCancel = false;
        #endregion

        // Current Book ----------------------------------------------------------------------
        delegate void cbSetCurrentBook(string sText);
        #region Method: void SetCurrentBook(string sText)
        static public void SetCurrentBook(string sText)
        {
            if (null == s_Dlg)
                return;

            if (s_Dlg.InvokeRequired)
            {
                var d = new cbSetCurrentBook(SetCurrentBook);
                s_Dlg.Invoke(d, new object[] { sText });
            }
            else
            {
                s_Dlg.m_labelBookName.Text = sText;
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
            Thread t = new Thread(new ThreadStart(DlgExportProgress.StartDialog));
            t.IsBackground = true;
            t.Name = "Export Progress";
            //t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        static private void StartDialog()
        {
            s_Dlg = new DlgExportProgress();
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

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdCancel
        private void cmdCancel(object sender, EventArgs e)
        {
            m_bUserSaysCancel = true;
        }
        #endregion
    }
}
