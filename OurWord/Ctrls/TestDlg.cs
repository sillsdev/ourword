using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OurWordData.DataModel;

namespace OurWord.Ctrls
{
    public partial class TestDlg : Form
    {
        public TestDlg()
        {
            InitializeComponent();

            nav.Setup(DB.TargetSection);
        }

        private void cmdLoad(object sender, EventArgs e)
        {
            contentPanel.Top = nav.Bottom;
            contentPanel.Left = 0;
            contentPanel.Size = new Size(Right, Height - nav.Height);
        }
    }
}
