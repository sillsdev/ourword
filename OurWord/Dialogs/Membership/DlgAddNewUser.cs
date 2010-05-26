using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Membership
{
    public partial class DlgAddNewUser : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string FullName
        public string FullName
        {
            get
            {
                return m_textFullName.Text;
            }
        }
        #endregion
        #region Attr{g}: string Password
        public string Password
        {
            get
            {
                return m_textPassword.Text;
            }
            set
            {
                m_textPassword.Text = value;
            }
        }
        #endregion
        #region Attr{g}: User InitializeAs
        public User InitializeAs
        {
            get
            {
                if (m_radioAdministrator.Checked)
                    return Users.Administrator;
                if (m_radioConsultant.Checked)
                    return Users.Consultant;
                if (m_radioTranslator.Checked)
                    return Users.Translator;
                return Users.Observer;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgAddNewUser()
        {
            InitializeComponent();
        }
        #endregion

        // Handlers --------------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(this, null);
            m_radioTranslator.Checked = true;

            Password = GenerateRandomPassword();

            m_textFullName.Focus();
        }
        #endregion
        #region Cmd: cmdClosing
        private void cmdClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            // Must have a non-null user name
            if (string.IsNullOrEmpty(FullName))
            {
                Messages.NeedUserName();
                e.Cancel = true;
                return;
            }

            // Username must be unique to the entire cluster
            if (null != Users.Find(FullName))
            {
                Messages.NeedUniqueUserName();
                e.Cancel = true;
                return;
            }

            // Must have a decent password
            if (string.IsNullOrEmpty(Password) || Password.Length < 8)
            {
                Messages.NeedPassword();
                e.Cancel = true;
            }
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region SMethod: string GenerateRandomPassword()
        static string GenerateRandomPassword()
        {
            const string c_sPasswordLetters = 
                "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
            var random = new Random();

            var sPassword = "";
            while (sPassword.Length < 8)
                sPassword += c_sPasswordLetters[random.Next(c_sPasswordLetters.Length-1)];

            return sPassword;
        }
        #endregion
    }
}
