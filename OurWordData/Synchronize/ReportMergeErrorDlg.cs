#region ***** ReportMergeErrorDlg.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ReportMergeErrorDlg.cs
 * Author:  John Wimbish
 * Created: 15 Dec 2009
 * Purpose: Request permissio to send an email upon a merge error
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Windows.Forms;
#endregion

namespace OurWordData.Synchronize
{
    public partial class ReportMergeErrorDlg : Form
    {
        #region Attr{g}: bool CanSendEmail
        public bool CanSendEmail
        {
            get
            {
                return m_checkSendEmail.Checked;
            }
        }
        #endregion

        #region Constructor()
        public ReportMergeErrorDlg()
        {
            InitializeComponent();
        }
        #endregion

        #region Cmd: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
        {
            m_checkSendEmail.Checked = true;
        }
        #endregion
    }
}
