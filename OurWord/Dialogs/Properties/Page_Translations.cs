using System;
using System.Windows.Forms;
using JWTools;
using OurWordData;
using OurWordData.DataModel;

namespace OurWord.Dialogs.Properties
{
    public partial class Page_Translations : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(parent)
        public Page_Translations(DialogProperties parent)
            : base(parent)
        {
            InitializeComponent();
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        public const string c_sID = "idProjectTranslations";
        #region OAttr{g}: string ID
        public override string ID
        {
            get
            {
                return c_sID;
            }
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kReferenceTranslations);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Translations";
            }
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            Control[] vExclude = {m_listTranslations, m_bTarget, m_bSource, m_bReference};
            LocDB.Localize(this, vExclude);

            var sBase = G.GetLoc_String("kEditProperties", "Edit {0} Properties...");
            m_bTarget.Text = LocDB.Insert(sBase, new[] { DB.TargetTranslation.DisplayName });
            m_bSource.Text = LocDB.Insert(sBase, new[] { DB.FrontTranslation.DisplayName });

            PopulateReferenceLanguagesList(null);
        }
        #endregion

        // Reference Languages ---------------------------------------------------------------
        #region Method: void PopulateReferenceLanguagesList()
        void PopulateReferenceLanguagesList(string sNameToSelect)
        {
            // Get the list from the current folder contents
            var ci = ClusterList.FindClusterInfo(DB.TeamSettings.DisplayName);
            var vAvailableLanguages = ci.GetClusterLanguageList(false);

            // Remove the Front and Target translations
            if (null != DB.FrontTranslation)
                vAvailableLanguages.Remove(DB.FrontTranslation.DisplayName);
            if (null != DB.TargetTranslation)
                vAvailableLanguages.Remove(DB.TargetTranslation.DisplayName);

            // Populate the list
            m_listTranslations.Items.Clear();
            foreach(var sLanguage in vAvailableLanguages)
            {
                var bChecked = false;
                foreach(DTranslation t in DB.Project.OtherTranslations)
                {
                    if (t.DisplayName == sLanguage)
                        bChecked = true;
                }
                m_listTranslations.Items.Add(sLanguage, bChecked);
            }

            // Select the requested item, or the first one if unspecified
            if (m_listTranslations.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(sNameToSelect))
                    m_listTranslations.SelectedItem = sNameToSelect;

                if (-1 == m_listTranslations.SelectedIndex)
                    m_listTranslations.SelectedIndex = 0;
            }
        }
        #endregion
        #region Cmd: cmdReferenceLanguageChecked - turn on/off display of a translation
        private void cmdReferenceLanguageChecked(object sender, ItemCheckEventArgs e)
        {
            // Retrieve the item
            var sItem = (string)m_listTranslations.Items[e.Index];
            var bChecked = (e.NewValue == CheckState.Checked);

            // Locate the translation in the list, if it is there
            var iPos = DB.Project.OtherTranslations.Find(sItem);

            // If it is being unchecked, then remove it from the list.
            if (!bChecked && -1 != iPos)
            {
                var tRemove = DB.Project.OtherTranslations[iPos];
                if (null != tRemove)
                    DB.Project.OtherTranslations.Remove(tRemove);
            }

            // Else it is being checked; add it to the list if it isn't there
            // (It may already be there if we're initializing the dialog)
            if (bChecked && -1 == iPos)
            {
                var tInsert = new DTranslation(sItem);
                DB.Project.OtherTranslations.Append(tInsert);
                tInsert.LoadFromFile();
            }
        }
        #endregion
        #region Cmd: cmdReferenceSelectionChanged
        private void cmdReferenceSelectionChanged(object sender, EventArgs e)
        {
            var i = m_listTranslations.SelectedIndex;
            if (-1 == i)
                return;
            var sItem = (string)m_listTranslations.Items[i];

            var sBase = G.GetLoc_String("kEditProperties", "Edit {0} Properties...");
            m_bReference.Text = LocDB.Insert(sBase, new[] { sItem });
        }
        #endregion
        #region Cmd: cmdCreateNewReferenceTranslation
        private void cmdCreateNewReferenceTranslation(object sender, EventArgs e)
        {
            // Ask the user to supply a valid name for this new translation
            var dlg = new DlgCreateTranslation();
            if (dlg.ShowDialog(Parent) != DialogResult.OK)
                return;

            // Create and store the new translation
            var t = new DTranslation(dlg.TranslationName);
            DB.Project.OtherTranslations.Append(t);
            t.WriteToFile(new NullProgress());
            PopulateReferenceLanguagesList(dlg.TranslationName);
        }
        #endregion
        #region Attr{g}: DTranslation SelectedReferenceTranslation
        DTranslation SelectedReferenceTranslation
        {
            get
            {
                var i = m_listTranslations.SelectedIndex;
                if (-1 == i)
                    return null;

                var sItem = (string)m_listTranslations.Items[i];
                int iTranslation = DB.Project.OtherTranslations.Find(sItem);
                if (-1 == iTranslation)
                    return null;

                return DB.Project.OtherTranslations[iTranslation];
            }
        }
        #endregion

        // Edit Properties -------------------------------------------------------------------
        #region Cmd: cmdEditReferenceProperties
        private void cmdEditReferenceProperties(object sender, EventArgs e)
        {
            var translation = SelectedReferenceTranslation;
            if (null == translation)
                return;

            var dlg = new DlgTranslationProperties(translation);
            dlg.ShowDialog(Parent);
        }
        #endregion
        #region Cmd:cmdEditTargetProperties
        private void cmdEditTargetProperties(object sender, EventArgs e)
        {
            var dlg = new DlgTranslationProperties(DB.TargetTranslation);
            dlg.ShowDialog(Parent);
        }
        #endregion
        #region Cmd:cmdEditSourceProperties
        private void cmdEditSourceProperties(object sender, EventArgs e)
        {
            var dlg = new DlgTranslationProperties(DB.FrontTranslation) 
            {
                SuppressCreateBook = true
            };
            dlg.ShowDialog(Parent);
        }
        #endregion

    }
}
