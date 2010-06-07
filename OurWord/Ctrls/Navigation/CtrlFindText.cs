using System;
using System.Drawing;
using System.Windows.Forms;

namespace OurWord.Ctrls.Navigation
{
    public delegate void FindTextChangedHandler(string sNewText);

    public class CtrlFindText : ToolStripControlHost
    {
        #region Attr{g}: string SearchText
        public string SearchText
        {
            get
            {
                return m_TextBox.Text;
            }
        }
        #endregion

        private readonly ToolStripLabel m_Label;
        private readonly ToolStripTextBox m_TextBox;

        #region Constructor()
        public CtrlFindText()
            : base(new ToolStrip())
        {
            // Find Label
            m_Label = new ToolStripLabel 
            { 
                Text = "Find:", //"G.GetLoc_String("kFind", "Find:"),
                BackColor = Color.Transparent
            };
            ToolStripControl.Items.Add(m_Label);

            // TextBox for the search text
            m_TextBox = new ToolStripTextBox 
            {
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.Black,
            };
            m_TextBox.TextChanged += cmdFindTextChanged;
            m_TextBox.KeyDown += cmdTextBoxKeyDown;
            ToolStripControl.Items.Add(m_TextBox);

            // Tool strip properties
            ToolStripControl.GripStyle = ToolStripGripStyle.Hidden;
            ToolStripControl.BackColor = Color.Transparent;
        }
        #endregion

        #region cmd: cmdTextBoxKeyDown
        void cmdTextBoxKeyDown(object sender, KeyEventArgs e)
            // When <Enter> is pressed in the textbox, close the menu item and 
            // start the Find operation
        {
            if (e.Modifiers == Keys.None && Keys.Enter == e.KeyCode)
            {
                e.Handled = true;
                PerformClick();
            }
        }
        #endregion

        public FindTextChangedHandler OnFindTextChanged;
        #region cmd: cmdFindTextChanged
        void cmdFindTextChanged(object sender, EventArgs e)
        {
            if (null != OnFindTextChanged)
                OnFindTextChanged(SearchText);
        }
        #endregion

        #region attr{g}: ToolStrip ToolStripControl
        private ToolStrip ToolStripControl
        {
            get
            {
                return Control as ToolStrip;
            }
        }
        #endregion

    }
}
