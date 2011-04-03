using System;
using System.Windows.Forms;
using JWTools;

namespace OurWord.Ctrls.Navigation
{
    public delegate void OptionsChangedHandler();

    public partial class CtrlFindOptions : UserControl
    {
        #region Attr{g/s}: bool IgnoreCase
        public bool IgnoreCase
        {
            get
            {
                return m_checkIgnoreCase.Checked;
            }
            set
            {
                m_checkIgnoreCase.Checked = value;
            }
        }
        #endregion
        #region Attr{g/s}: bool OnlyScanCurrentBook
        public bool OnlyScanCurrentBook
        {
            get
            {
                return m_checkCurrentBookOnly.Checked;
            }
            set
            {
                m_checkCurrentBookOnly.Checked = value;
            }
        }
        #endregion
        #region Attr{g}: Scanner.SearchType Type
        public Scanner.SearchType Type
        {
            get
            {
                if (m_checkMustBe.Checked == false)
                    return Scanner.SearchType.Anywhere;

                if (m_comboSearchType.Text.CompareTo(m_sEntireWord) == 0)
                    return Scanner.SearchType.Whole;
                if (m_comboSearchType.Text.CompareTo(m_sWordBegin) == 0)
                    return Scanner.SearchType.Beginning;
                if (m_comboSearchType.Text.CompareTo(m_sWordEnd) == 0)
                    return Scanner.SearchType.End;

                throw new Exception("Unknown SearchType");
            }
        }
        #endregion
        public OptionsChangedHandler OnOptionsChanged;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public CtrlFindOptions()
        {
            InitializeComponent();
        }
        #endregion
        #region Method: void LocalizeAndInitialize()
        public void LocalizeAndInitialize()
        {
            LocDB.Localize(this, new[] {m_comboSearchType});
            GetComboLocalizations();

            // Set the combo box to default to Entire Word
            m_comboSearchType.Items.Clear();
            m_comboSearchType.Items.Add(m_sEntireWord);
            m_comboSearchType.Items.Add(m_sWordBegin);
            m_comboSearchType.Items.Add(m_sWordEnd);
            m_comboSearchType.Text = m_sEntireWord;

            // Setting the combo will will have checked this; so we must uncheck it
            m_checkMustBe.Checked = false;
        }
        #endregion

        // Combo Values ----------------------------------------------------------------------
        private string m_sEntireWord;
        private string m_sWordBegin;
        private string m_sWordEnd;
        #region method: void GetComboLocalizations()
        private void GetComboLocalizations()
        {
            m_sEntireWord = G.GetLoc_String("kWholeWord", "Entire Word");
            m_sWordBegin = G.GetLoc_String("kWordBegin", "At Beginning of Word");
            m_sWordEnd = G.GetLoc_String("kWordEnd", "At End of Word");
        }
        #endregion

        // Events ----------------------------------------------------------------------------
        #region event: cmdOptionChanged
        private void cmdOptionChanged(object sender, EventArgs e)
        {
            if (null != OnOptionsChanged)
                OnOptionsChanged();
        }
        #endregion
        #region event: cmdComboChanged
        private void cmdComboChanged(object sender, EventArgs e)
        {
            m_checkMustBe.Checked = true;
            cmdOptionChanged(sender, e);
        }
        #endregion
    }
}
