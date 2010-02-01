using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OurWordSetup.Data;

namespace OurWordSetup.UI
{
    public partial class DlgFullSetup : Form
    {
        public DlgFullSetup()
        {
            InitializeComponent();
        }

        private void cmdInstall(object sender, EventArgs e)
        {
            Hide();
            var setup = new SetupManager(this);
            setup.DoFullSetup();
            Close();
        }

        private void cmdCancel(object sender, EventArgs e)
        {
            Close();
        }
    }
}
