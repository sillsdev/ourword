using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace OurWordData
{
    public interface IProgressIndicator
    {
        void Start(string sMessage, int nCount);
        void Step();
        void End();
    }

    #region Class: NullProgress : IProgressIndicator
    public class NullProgress : IProgressIndicator
    {
        public void Start(string sMessage, int nCount)
        {
        }
        public void Step()
        {
        }
        public void End()
        {
        }
    }
    #endregion

    public class ToolStripProgress : IProgressIndicator
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: Form MainForm
        Form MainForm
        {
            get
            {
                Debug.Assert(null != m_MainForm);
                return m_MainForm;
            }
        }
        readonly Form m_MainForm;
        #endregion
        #region Attr{g}: ToolStripProgressBar Bar
        ToolStripProgressBar Bar
        {
            get
            {
                Debug.Assert(null != m_Bar);
                return m_Bar;
            }
        }
        readonly ToolStripProgressBar m_Bar;
        #endregion
        #region Attr{g}: string MessageText
        string MessageText
        {
            get
            {
                if (null != m_Label)
                    return m_Label.Text;
                return "";
            }
            set
            {
                if (null != m_Label)
                    m_Label.Text = value;
            }
        }
        readonly ToolStripStatusLabel m_Label;
        #endregion

        // IProgressIndicator ----------------------------------------------------------------
        #region Method: void Start(sMessage, nCount)
        public void Start(string sMessage, int nCount)
        {
            MessageText = sMessage;
            Bar.Invalidate();
            Bar.Minimum = 0;
            Bar.Maximum = nCount;
            Bar.Value = 0;
            Bar.Step = 1;
            Bar.Visible = true;
            MainForm.Cursor = Cursors.WaitCursor;
        }
        #endregion
        #region Method: void Step()
        public void Step()
        {
            Bar.PerformStep();
        }
        #endregion
        #region Method: void End()
        public void End()
        {
            Bar.Visible = false;
            MessageText = "";
            MainForm.Cursor = Cursors.Default;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(Form, ToolStripProgressBar, ToolStripStatusLabel)
        public ToolStripProgress(Form _form, ToolStripProgressBar _bar, ToolStripStatusLabel _label)
        {
            m_MainForm = _form;
            m_Bar = _bar;
            m_Label = _label;
        }
        #endregion
    }


}
