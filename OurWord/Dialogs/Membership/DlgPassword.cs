using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JWTools;

namespace OurWord.Dialogs.Membership
{
    public partial class DlgPassword : Form
    {
        #region Attr{g}: string Password
        public string Password
        {
            get
            {
                return m_textPassword.Text;
            }
        }
        #endregion
        #region Attr{s}: string UserName
        public string UserName { private get; set; }
        #endregion

        #region Constructor()
        public DlgPassword()
        {
            InitializeComponent();
        }
        #endregion

        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(this, null);

            var sBase = m_labelEnterPassword.Text;
            m_labelEnterPassword.Text = LocDB.Insert(sBase, new string[] { UserName });

            m_textPassword.Focus();
        }
        #endregion
        #region Cmd: cmdKeyDown
        private void cmdKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None && 
                e.KeyCode == Keys.Enter &&
                Password.Length > 0)
            {
                e.Handled = true;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        #endregion
    }
}
