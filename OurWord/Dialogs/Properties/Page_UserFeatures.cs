using System;
using JWTools;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Properties
{
    /* Adding new features must be in the following places:
     * - Load: set to the Current user's values
     * - SetAllFeatures method
     * - HarvestChanges
     */
    public partial class Page_UserFeatures : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_UserFeatures(DialogProperties dlgParent)
            : base(dlgParent)
        {
            InitializeComponent();
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region OAttr{g}: string ID
        public override string ID
        {
            get
            {
                return "idUserFeatures";
            }
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: string Title
        public override string Title
        {
            get
            {
                return "Features On/Off";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            Users.Current.CanEditStructure = m_checkStructuralEditing.Checked;
            Users.Current.CanUndoRedo = m_checkUndoRedo.Checked;

            Users.Current.CanNavigateChapter = m_checkChapterButton.Checked;
            Users.Current.CanNavigateFirstLast = m_checkFirstLast.Checked;

            Users.Current.CanDoBackTranslation = m_checkBackTranslation.Checked;
            Users.Current.CanDoNaturalnessCheck = m_checkNaturalnessCheck.Checked;
            Users.Current.CanDoConsultantPreparation = m_checkConsultantPreparation.Checked;
            Users.Current.CanZoom = m_checkCanZoom.Checked;

            Users.Current.CanCreateProject = m_checkNewProject.Checked;
            Users.Current.CanOpenProject = m_checkOpenProject.Checked;
            Users.Current.CanExportProject = m_checkExport.Checked;

            Users.Current.CanPrint = m_checkPrinting.Checked;
            Users.Current.CanFilter = m_checkFilters.Checked;
            Users.Current.CanLocalize = m_checkLocalization.Checked;
            Users.Current.CanRestoreBackups = m_checkRestoreBackups.Checked;

            return true;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            m_checkStructuralEditing.Checked = Users.Current.CanEditStructure;
            m_checkUndoRedo.Checked = Users.Current.CanUndoRedo;

            m_checkChapterButton.Checked = Users.Current.CanNavigateChapter;
            m_checkFirstLast.Checked = Users.Current.CanNavigateFirstLast;

            m_checkBackTranslation.Checked = Users.Current.CanDoBackTranslation;
            m_checkNaturalnessCheck.Checked = Users.Current.CanDoNaturalnessCheck;
            m_checkConsultantPreparation.Checked = Users.Current.CanDoConsultantPreparation;
            m_checkCanZoom.Checked = Users.Current.CanZoom;

            m_checkNewProject.Checked = Users.Current.CanCreateProject;
            m_checkOpenProject.Checked = Users.Current.CanOpenProject;
            m_checkExport.Checked = Users.Current.CanExportProject;

            m_checkPrinting.Checked = Users.Current.CanPrint;
            m_checkFilters.Checked = Users.Current.CanFilter;
            m_checkLocalization.Checked = Users.Current.CanLocalize;
            m_checkRestoreBackups.Checked = Users.Current.CanRestoreBackups;
        }
        #endregion

        #region Cmd: cmdTurnAllFeaturesOn
        private void cmdTurnAllFeaturesOn(object sender, EventArgs e)
        {
            SetAllFeatures(true);
        }
        #endregion
        #region Cmd: cmdTurnAllFeaturesOff
        private void cmdTurnAllFeaturesOff(object sender, EventArgs e)
        {
            SetAllFeatures(false);
        }
        #endregion
        #region Method: SetAllFeatures(bool bOn)
        void SetAllFeatures(bool bOn)
        {
            // Editing
            m_checkStructuralEditing.Checked = bOn;
            m_checkUndoRedo.Checked = bOn;

            // Navigation
            m_checkChapterButton.Checked = bOn;
            m_checkFirstLast.Checked = bOn;

            // Windows / Tasks
            m_checkNaturalnessCheck.Checked = bOn;
            m_checkBackTranslation.Checked = bOn;
            m_checkConsultantPreparation.Checked = bOn;
            m_checkCanZoom.Checked = bOn;

            // Tools
            m_checkPrinting.Checked = bOn;
            m_checkNewProject.Checked = bOn;
            m_checkOpenProject.Checked = bOn;
            m_checkExport.Checked = bOn;
            m_checkLocalization.Checked = bOn;
            m_checkFilters.Checked = bOn;
        }
        #endregion
    }
}
