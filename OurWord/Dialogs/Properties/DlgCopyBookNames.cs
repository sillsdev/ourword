using System;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;

namespace OurWord.Dialogs.Properties
{
    public partial class DlgCopyBookNames : Form
    {
        private readonly DTranslation DestinationTranslation;
        #region Constructor()
        public DlgCopyBookNames(DTranslation destinationTranslation)
        {
            InitializeComponent();
            DestinationTranslation = destinationTranslation;
        }
        #endregion
        #region event: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(this, new[] {m_comboChoices});

            PopulateCopyNamesPossibilities();

            m_comboChoices.Text = @"English";
        }
        #endregion

        #region Method: void PopulateCopyNamesPossibilities()
        void PopulateCopyNamesPossibilities()
        {
            // Start with an empty dropdown so we don't double things
            m_comboChoices.Items.Clear();

            // Put in English, as a language we always have
            AddDropDownItem("English");

            // UI Languages
            if (null != LocDB.DB.PrimaryLanguage)
                AddDropDownItem(LocDB.DB.PrimaryLanguage.Name);
            if (null != LocDB.DB.SecondaryLanguage)
                AddDropDownItem(LocDB.DB.SecondaryLanguage.Name);

            // The project's translations
            var vTranslations = DB.Project.AllTranslations;
            foreach(var t in vTranslations)
            {
                if (t != DestinationTranslation)
                    AddDropDownItem(t.DisplayName);
            }
        }
        #endregion
        #region Method: void AddDropDownItem(sLanguageName)
        void AddDropDownItem(string sLanguageName)
        {
            // Valid item?
            if (string.IsNullOrEmpty(sLanguageName))
                return;

            // If it is already there, don't add it
            foreach (string item in m_comboChoices.Items)
            {
                if (item.CompareTo(sLanguageName) == 0)
                    return;
            }

            // Add the item
            m_comboChoices.Items.Add(sLanguageName);
        }
        #endregion

        #region Attr{g}: string SourceName
        public string SourceName
        {
            get
            {
                return m_comboChoices.Text;
            }
        }
        #endregion
        #region Attr{g}: DTranslation SourceTranslation
        public DTranslation SourceTranslation
        {
            get
            {
                var sLanguageName = m_comboChoices.Text;
                if (string.IsNullOrEmpty(sLanguageName))
                    return null;

                var vTranslations = DB.Project.AllTranslations;
                foreach(var t in vTranslations)
                {
                    if (t.DisplayName == sLanguageName)
                        return t;
                }

                // Not found
                return null;
            }
        }
        #endregion
    }
}
