using System;
using System.Windows.Forms;
using JWTools;

namespace OurWord.Ctrls.Navigation
{
    public partial class DlgContinueToNextBook : Form
    {
        #region VAttr{g}: bool DontAskAgain
        public bool DontAskAgain
        {
            get
            {
                return m_checkDontAskAgain.Checked;
            }
        }
        #endregion

        #region Constructor()
        public DlgContinueToNextBook()
        {
            InitializeComponent();
        }
        #endregion

        #region cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(this, null);
        }
        #endregion
    }
}
