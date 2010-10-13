#region ***** DlgFind.cs *****
using System;
using System.Diagnostics;
using System.Windows.Forms;
using JWTools;
using OurWord.Ctrls.Commands;

#endregion

namespace OurWord.Dialogs
{
    public delegate void FindNextHandler(DlgFind dlg);

    public partial class DlgFind : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: string SearchText
        public string SearchText
        {
            get
            {
                return m_tFind.Text;
            }
        }
        #endregion
        public FindNextHandler OnFindNext;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgFind()
        {
            InitializeComponent();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region cmd: cmdFindNext
        private void cmdFindNext(object sender, EventArgs e)
        {
            Debug.Assert(null != OnFindNext);
            OnFindNext(this);
        }
        #endregion

        private const string WindowStateRegistryKey = "FindDialog";
        #region cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
        {
            var state = new JW_WindowState(this, false) {
                WindowStateRegistrySubKey = WindowStateRegistryKey,
                StartMaximized = false,
            };
            state.SaveWindowState();
        }
        #endregion
        #region cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            var state = new JW_WindowState(this, false) {
                WindowStateRegistrySubKey = WindowStateRegistryKey,
                StartMaximized = false,
            };
            state.RestoreWindowState();

            // If we're at 0,0, assume we are first-time and move to somewhere a little
            // more reasonable for the user to find. We'll aim for somewhere in the vicinity
            // of below the toolbar.
            if (Left == 0 && Top == 0)
            {
                Left = G.App.Left + 15;
                Top = G.App.Top + 100;
            }
        }
        #endregion
        #region cmd: cmdClose
        private void cmdClose(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
