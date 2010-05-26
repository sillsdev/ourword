using System;
using System.Diagnostics;
using System.Windows.Forms;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Membership
{
    public partial class DlgAdministratorLogin : Form
    {
        #region Constructor()
        public DlgAdministratorLogin()
        {
            InitializeComponent();
        }
        #endregion

        #region Method: void PopulateAdministratorComboItems()
        void PopulateAdministratorComboItems()
        {
            foreach(var user in Users.Members)
            {
                if (user.IsAdministrator)
                    m_comboAdministrators.Items.Add(user.UserName);
            }
        }
        #endregion

        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            PopulateAdministratorComboItems();
            if (m_comboAdministrators.Items.Count > 0)
                m_comboAdministrators.Text = (string)m_comboAdministrators.Items[0];
        }
        #endregion
        #region Cmd: cmdAdministratorTextChanged
        private void cmdAdministratorTextChanged(object sender, EventArgs e)
        {
            m_textPassword.Text = "";
        }
        #endregion

        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
            // Verify that we have a valid administrator/password combination
        {
            if (DialogResult.OK != DialogResult)
                return;

            var user = Users.Find(m_comboAdministrators.Text);
            Debug.Assert(null != user);

            if (user.Password != m_textPassword.Text)
            {
                if (!Messages.TryPasswordAgain())
                    e.Cancel = true;
            }
        }
        #endregion

    }
}
