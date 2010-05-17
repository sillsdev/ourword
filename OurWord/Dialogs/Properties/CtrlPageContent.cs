#region ***** CtrlPageContent.cs *****
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace OurWord.Dialogs.Properties
{
    public partial class CtrlPageContent : UserControl
    {
        #region Method: void SetTitle(sText)
        public void SetTitle(string sText)
        {
            m_NavTitle.Text = sText;
        }
        #endregion
        #region Method: void SetContent(UserControl ctrl)
        public void SetContent(UserControl ctrl)
        {
            SuspendLayout();

            // Turn off and remove any current page
            while(m_panelContent.Controls.Count > 0)
            {
                var old = m_panelContent.Controls[0];
                old.Visible = false;
                m_panelContent.Controls.Remove(old);
            }

            // Add and display the new one
            ctrl.Visible = true;
            m_panelContent.Controls.Add(ctrl);
            ctrl.Location = new Point(0,0);
            ctrl.Size = m_panelContent.Size;

            ResumeLayout();
        }
        #endregion

        #region Constructor()
        public CtrlPageContent()
        {
            InitializeComponent();
        }
        #endregion
    }
}
