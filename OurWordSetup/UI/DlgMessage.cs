using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OurWordSetup.UI
{
    public partial class DlgMessage : Form
    {
        #region Attr{g/s}: string Message
        public string Message
        {
            get
            {
                return m_Message.Text;
            }
            set
            {
                m_Message.Text = value;
            }
        }
        #endregion

        public DlgMessage()
        {
            InitializeComponent();
        }

        private void cmdLoad(object sender, EventArgs e)
        {
            m_pErrorIcon.Image = SystemIcons.Information.ToBitmap();
        }
    }
}
