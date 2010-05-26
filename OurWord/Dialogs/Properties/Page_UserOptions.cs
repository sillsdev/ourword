using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Properties
{
    public partial class Page_UserOptions : DlgPropertySheet
    {
        #region Constructor(DlgProperties)
        public Page_UserOptions(DialogProperties parentDlg)
            : base(parentDlg)
        {
            InitializeComponent();
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region OAttr{g}: string ID
        public override string ID
        {
            get
            {
                return "idUserOptions";
            }
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: string Title
        public override string Title
        {
            get
            {
                return "User Options";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            var user = Users.Current;

            user.MaximizeWindowOnStartup = m_cMaximizeWindowOnStartup.Checked;
            user.ZoomPercent = ReadZoomPercentCombo();

            user.CollaborationUserName = m_tUserName.Text;
            user.CollaborationPassword = m_tPassword.Text;

            return true;
        }
        #endregion

        #region Method: void PopulateZoomPercentCombo()
        private void PopulateZoomPercentCombo()
        {
            m_cZoomPercent.Items.Clear();

            foreach(var n in User.PossibleZoomPercents)
            {
                var sPercent = n + @"%";
                m_cZoomPercent.Items.Add(sPercent);
            }

            m_cZoomPercent.Text = Users.Current.ZoomPercent + @"%";
        }
        #endregion
        #region Method: int ReadZoomPercentCombo()
        int ReadZoomPercentCombo()
        {
            try
            {
                var s = "";
                foreach(var ch in m_cZoomPercent.Text)
                {
                    if (char.IsDigit(ch))
                        s += ch;
                }

                return Convert.ToInt16(s);
            }
            catch (Exception) { }

            return Users.Current.ZoomPercent;
        }
        #endregion

        private void cmdLoad(object sender, EventArgs e)
        {
            PopulateZoomPercentCombo();
            
            var user = Users.Current;
            m_cMaximizeWindowOnStartup.Checked = user.MaximizeWindowOnStartup;

            m_tUserName.Text = user.CollaborationUserName;
            m_tPassword.Text = user.CollaborationPassword;
        }

    }
}
