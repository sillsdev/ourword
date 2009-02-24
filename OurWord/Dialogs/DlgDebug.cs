using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OurWord.Dialogs
{
    public partial class DlgDebug : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: string DebugText
        string DebugText
        {
            get
            {
                return m_DebugText.Text;
            }
            set
            {
                m_DebugText.Text = value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        private DlgDebug()
        {
            InitializeComponent();
        }
        #endregion
        #region Cmd: cmdClosed
        private void cmdClosed(object sender, FormClosedEventArgs e)
        {
            s_dlg = null;
        }
        #endregion
        static DlgDebug s_dlg = null;
        #region Method: void Init()
        static void Init()
        {
            if (null == s_dlg)
            {
                s_dlg = new DlgDebug();
                s_dlg.Show();
            }
        }
        #endregion

        // Write Info ------------------------------------------------------------------------
        static public void AddLine(string sLine)
        {
            Init();
            s_dlg.DebugText += (sLine + "\r\n");
        }

    }
}