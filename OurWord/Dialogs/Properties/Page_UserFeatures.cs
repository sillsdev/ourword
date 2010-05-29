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
            return true;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            bag_Setup();
        }
        #endregion

        // Property Bag ----------------------------------------------------------------------
        private PropertyBag m_bag;
        #region Constants
        private const string c_sGroupEditing = "Editing";
        private const string c_sStructuralChanges = "propStructuralChanges";
        private const string c_sUndoRedo = "propUndoRedo";

        private const string c_sGroupNavigation = "Navigation";
        private const string c_sChapter = "propChapter";
        private const string c_sFirstLast = "propFirstLast";
        
        private const string c_sGroupWindows = "Available Windows / Tasks";
        private const string c_sNaturalnessCheck = "propNaturalness";
        private const string c_sBackTranslation = "propBackTranslation";
        private const string c_sConsultantPreparation = "propConsultantPreparation";
        private const string c_sCanZoomText = "propZoom";

        private const string c_sGroupProject = "Project";
        private const string c_sCreateNew = "propCreateNewProject";
        private const string c_sOpenExisting = "propOpenExistingProject";
        private const string c_sExport = "propExportProject";

        private const string c_sGroupTools = "Tools";
        private const string c_sPrinting = "propPrinting";
        private const string c_sFilters = "propFilters";
        private const string c_sLocalization = "propLocalization";
        private const string c_sRestoreBackups = "propRestoreBackups";

        #endregion
        #region Cmd: bag_Get
        static void bag_Get(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sStructuralChanges:
                    YesNoPropertySpec.Put(e, Users.Current.CanEditStructure);
                    break;
                case c_sUndoRedo:
                    YesNoPropertySpec.Put(e, Users.Current.CanUndoRedo);
                    break;

                case c_sChapter:
                    YesNoPropertySpec.Put(e, Users.Current.CanNavigateChapter);
                    break;
                case c_sFirstLast:
                    YesNoPropertySpec.Put(e, Users.Current.CanNavigateFirstLast);
                    break;

                case c_sNaturalnessCheck:
                    YesNoPropertySpec.Put(e, Users.Current.CanDoNaturalnessCheck);
                    break;
                case c_sBackTranslation:
                    YesNoPropertySpec.Put(e, Users.Current.CanDoBackTranslation);
                    break;
                case c_sConsultantPreparation:
                    YesNoPropertySpec.Put(e, Users.Current.CanDoConsultantPreparation);
                    break;
                case c_sCanZoomText:
                    YesNoPropertySpec.Put(e, Users.Current.CanZoom);
                    break;

                case c_sCreateNew:
                    YesNoPropertySpec.Put(e, Users.Current.CanCreateProject);
                    break;
                case c_sOpenExisting:
                    YesNoPropertySpec.Put(e, Users.Current.CanOpenProject);
                    break;
                case c_sExport:
                    YesNoPropertySpec.Put(e, Users.Current.CanExportProject);
                    break;

                case c_sPrinting:
                    YesNoPropertySpec.Put(e, Users.Current.CanPrint);
                    break;
                case c_sFilters:
                    YesNoPropertySpec.Put(e, Users.Current.CanFilter);
                    break;
                case c_sLocalization:
                    YesNoPropertySpec.Put(e, Users.Current.CanLocalize);
                    break;
                case c_sRestoreBackups:
                    YesNoPropertySpec.Put(e, Users.Current.CanRestoreBackups);
                    break;

            }
        }
        #endregion
        #region Cmd: bag_Set
        static void bag_Set(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sStructuralChanges:
                    Users.Current.CanEditStructure = YesNoPropertySpec.Pull(e);
                    break;
                case c_sUndoRedo:
                    Users.Current.CanUndoRedo = YesNoPropertySpec.Pull(e);
                    break;

                case c_sChapter:
                    Users.Current.CanNavigateChapter = YesNoPropertySpec.Pull(e);
                    break;
                case c_sFirstLast:
                    Users.Current.CanNavigateFirstLast = YesNoPropertySpec.Pull(e);
                    break;

                case c_sNaturalnessCheck:
                    Users.Current.CanDoNaturalnessCheck = YesNoPropertySpec.Pull(e);
                    break;
                case c_sBackTranslation:
                    Users.Current.CanDoBackTranslation = YesNoPropertySpec.Pull(e);
                    break;
                case c_sConsultantPreparation:
                    Users.Current.CanDoConsultantPreparation = YesNoPropertySpec.Pull(e);
                    break;
                case c_sCanZoomText:
                    Users.Current.CanZoom = YesNoPropertySpec.Pull(e);
                    break;

                case c_sCreateNew:
                    Users.Current.CanCreateProject = YesNoPropertySpec.Pull(e);
                    break;
                case c_sOpenExisting:
                    Users.Current.CanOpenProject = YesNoPropertySpec.Pull(e);
                    break;
                case c_sExport:
                    Users.Current.CanExportProject = YesNoPropertySpec.Pull(e);
                    break;

                case c_sPrinting:
                    Users.Current.CanPrint = YesNoPropertySpec.Pull(e);
                    break;
                case c_sFilters:
                    Users.Current.CanFilter = YesNoPropertySpec.Pull(e);
                    break;
                case c_sLocalization:
                    Users.Current.CanLocalize = YesNoPropertySpec.Pull(e);
                    break;
                case c_sRestoreBackups:
                    Users.Current.CanRestoreBackups = YesNoPropertySpec.Pull(e);
                    break;
            }
        }
        #endregion
        #region Method: void bag_Setup()
        void bag_Setup()
        {
            m_bag = new PropertyBag();
            m_bag.GetValue += bag_Get;
            m_bag.SetValue += bag_Set;

            #region Structural Changes
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sStructuralChanges,
                "Structural Changes",
                c_sGroupEditing,
                "Enables the translator to split and join paragraphs, or to assign different " +
                        "styles to a paragraph. By doing this the translation will depart from " +
                        "the paragraph structure of the front translation.",
                true
                ));
            #endregion
            #region Undo / Redo
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sUndoRedo,
                "Undo and Redo",
                c_sGroupEditing,
                "Enables the Undo and Redo menus, by which you can undo actions such as " +
                        "typing, deleting, splitting and joining paragraphs, etc.",
                true
                ));
            #endregion

            #region Go To Chapter
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sChapter,
                "Show Chapter button in toolbar",
                c_sGroupNavigation,
                "Makes the Chapter button visible, by which you can navigate directly to " +
                        "the first section in the desired chapter. Experienced users may want " +
                        "to have this button visible for easier movement around the book.",
                true
                ));
            #endregion
            #region Go To First/Last
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sFirstLast,
                "Show First & Last buttons in toolbar",
                c_sGroupNavigation,
                "Makes the First and Last Buttons visible, by which you can navigate to the " +
                        "first and last sections in the book. Experienced users may want to have " +
                        "these buttons visible for easier movement around the book.",
                true
                ));
            #endregion

            #region Naturalness
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sNaturalnessCheck,
                "Naturalness Check",
                c_sGroupWindows,
                "A layout where only the translation is visible, so that you can read through " +
                        "for naturalness, without being influenced by the front translation.",
                true
                ));
            #endregion
            #region Back Translation
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sBackTranslation,
                "Back Translation",
                c_sGroupWindows,
                "A layout where you can do a Back Translation; that is, where you provide " +
                        "a translation  in the language the consultant understands.",
                true
                ));
            #endregion
            #region Consultant Preparation
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sConsultantPreparation,
                "Consultant Preparation",
                c_sGroupWindows,
                "Displays the Front/Model translation and the Target translation, with both " +
                        "the vernacular and back translations. Use this to prepare for the " +
                        "consultant (e.g., exegetical notes.)",
                true
                ));
            #endregion
            #region Zoom Text
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sCanZoomText,
                "Can Zoom Text?",
                c_sGroupWindows,
                "Allows the user to adjust the text display size, e.g., to magnify it to 150% " +
                    "if needed for eyesight issues.",
                true
                ));
            #endregion

            #region Create New Project
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sCreateNew,
                "Can create a new project",
                c_sGroupProject,
                "Can create a new project within this cluster, or create a project within " +
                    "an entirely new cluster",
                true
                ));
            #endregion
            #region Open Existing Project
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sOpenExisting,
                "Can open an existing project",
                c_sGroupProject,
                "Can open other projects within the cluster for which this user is a member.",
                true
                ));
            #endregion
            #region Export Project
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sExport,
                "Export",
                c_sGroupProject,
                "Provides export to Toolbox, USFM, Microsft Word 2007, GoBible, and " +
                    "potentially other file formats.",
                true
                ));
            #endregion

            #region Print
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sPrinting,
                "Print",
                c_sGroupTools,
                "The book can be printed. ",
                true
                ));
            #endregion
            #region Filters
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sFilters,
                "Filters (Only show matching sections)",
                c_sGroupTools,
                "The \"Go To\" Menu (e.g., Next, Previous commands) will only go to those " +
                        "sections which match certain criteria. E.g., they must have a certain word " +
                        "defined, or must have mismatched quotes. This can be a good way to locate " +
                        "errors, or To Do Notes.",
                true
                ));
            #endregion
            #region Localization
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sLocalization,
                "Localization Tool",
                c_sGroupTools,
                "This dialog, appearing in the Tools menu, allows you to translate the " +
                        "user interface of OurWord into another language.",
                true
                ));
            #endregion
            #region Restore Backups
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sRestoreBackups,
                "Restore Backups",
                c_sGroupTools,
                "Files can be automatically backed up " +
                        "to a flash card or other storage device. If you turn on this Restore " +
                        "feature, the Tools menu will provide access to a dialog by which the " +
                        "current file can be replaced by a previously stored backup.",
                true
                ));
            #endregion

            LocDB.Localize(this, m_bag);
            m_grid.SelectedObject = m_bag;
        }
        #endregion
    }
}
