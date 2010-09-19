using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Properties
{
    public partial class Page_UserOptions : DlgPropertySheet
    {
        #region Constructor(DlgProperties)
        public Page_UserOptions(DialogProperties parentDlg)
            : base(parentDlg)
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
                return "idUserOptions";
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
                return "User Options";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            return true;
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            bag_Setup();
        }
        #endregion

        // Property Bag ----------------------------------------------------------------------
        private PropertyBag m_bag;
        #region Constants
        private const string c_sGroupBehavior = "Behavior";
        private const string c_sMaximizeOnStartup = "propMaximizeOnStartup";
        private const string c_sZoomPercent = "propZoomPercent";
        private const string c_sUserPassword = "propUserPassword";

        private const string c_sGroupUILanguage = "Language of the User Interface";
        private const string c_sPrimaryLanguage = "propPrimaryLanguage";
        private const string c_sSecondaryLanguage = "propSecondaryLanguage";

        private const string c_sGroupSendReceive = "Send / Receive";
        private const string c_sUserName = "propUserName";
        private const string c_sPaassword = "propPassword";

        private const string c_sGroupBackgroundColors = "Window Background Colors";
        private const string c_sColorDrafting = "propDrafting";
        private const string c_sColorBackTranslation = "propBackTranslation";
        private const string c_sColorNaturalnessCheck = "propNaturalness";
        private const string c_sColorConsultantPreparation = "propConsultant";
        #endregion
        #region Cmd: bag_Get
        static void bag_Get(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                // Behavior
                case c_sMaximizeOnStartup:
                    YesNoPropertySpec.Put(e, Users.Current.MaximizeWindowOnStartup);
                    break;
                case c_sZoomPercent:
                    ZoomPropertySpec.Put(e, Users.Current.ZoomPercent);
                    break;
                case c_sUserPassword:
                    e.Value = Users.Current.Password;
                    break;

                // UI Languages
                case c_sPrimaryLanguage:
                    e.Value = Users.Current.PrimaryUiLanguage;
                    break;
                case c_sSecondaryLanguage:
                   e.Value = Users.Current.SecondaryUiLanguage;
                   break;

                // Send / Receive
                case c_sUserName:
                    e.Value = Users.Current.CollaborationUserName;
                   break;
                case c_sPaassword:
                    e.Value = Users.Current.CollaborationPassword;
                   break;

                // Window background colors
                case c_sColorDrafting:
                   e.Value = Users.Current.DraftingWindowBackground;
                   break;
                case c_sColorBackTranslation:
                   e.Value = Users.Current.BackTranslationWindowBackground;
                   break;
                case c_sColorConsultantPreparation:
                   e.Value = Users.Current.ConsultantWindowBackground;
                   break;
                case c_sColorNaturalnessCheck:
                   e.Value = Users.Current.NaturalnessWindowBackground;
                   break;
            }
        }
        #endregion
        #region Cmd: bag_Set
        static void bag_Set(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                // Behavior
                case c_sMaximizeOnStartup:
                    Users.Current.MaximizeWindowOnStartup = YesNoPropertySpec.Pull(e);
                    break;
                case c_sZoomPercent:
                    Users.Current.ZoomPercent = ZoomPropertySpec.Pull(e);
                    break;
                case c_sUserPassword:
                    Users.Current.Password = (string) e.Value;
                    break;

                // UI Languages
                case c_sPrimaryLanguage:
                    Users.Current.PrimaryUiLanguage = (string) e.Value;
                    break;
                case c_sSecondaryLanguage:
                    Users.Current.SecondaryUiLanguage = (string)e.Value;
                    break;

                // Send / Receive
                case c_sUserName:
                    Users.Current.CollaborationUserName = (string) e.Value;
                    break;
                case c_sPaassword:
                    Users.Current.CollaborationPassword = (string) e.Value;
                    break;

                // Window background colors
                case c_sColorDrafting:
                    Users.Current.DraftingWindowBackground = (string)e.Value;
                    break;
                case c_sColorBackTranslation:
                    Users.Current.BackTranslationWindowBackground = (string)e.Value;
                    break;
                case c_sColorConsultantPreparation:
                    Users.Current.ConsultantWindowBackground = (string)e.Value;
                    break;
                case c_sColorNaturalnessCheck:
                    Users.Current.NaturalnessWindowBackground = (string)e.Value;
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

            #region Maximize on Startup
            m_bag.Properties.Add(new YesNoPropertySpec(
                c_sMaximizeOnStartup,
                "Maxmimize Window on Startup?",
                c_sGroupBehavior,
                "If Yes, OurWord starts up maximized. This may help newer users to have sufficient " +
                    "size on the screen for doing work.",
                true
                ));
            #endregion
            #region ZoomPercent
            m_bag.Properties.Add(new ZoomPropertySpec(
                c_sZoomPercent,
                "Zoom Percent",
                c_sGroupBehavior,
                "Text in the windows can be larger (or smaller) by the chosen percentage. (You can " +
                    "also set this in the Window dropdown.)",
                new[] { 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 175, 200, 225, 250 },
                100
                ) {DontLocalizeEnums = true});
            #endregion
            #region OurWord User Password
            m_bag.Properties.Add(new PropertySpec(
                c_sUserPassword,
                "OurWord User Password",
                typeof(string),
                c_sGroupBehavior,
                "The password used to get into this Configuration Dialog.",
                ""));
            #endregion

            #region Primary UI Language
            m_bag.Properties.Add(new PropertySpec(
                c_sPrimaryLanguage,
                "Preferred (primary)",
                c_sGroupUILanguage,
                "Use this language for the User Interface.",
                LocDB.DB.LanguageChoices,
                LocItem.c_sEnglish) {DontLocalizeEnums = true});
            #endregion
            #region Secondary UI Language
            m_bag.Properties.Add(new PropertySpec(
                c_sSecondaryLanguage,
                "Fallback (secondary)",
                c_sGroupUILanguage,
                "Use this language for the User Interface if the preferred (primary) language is unavailable.",
                LocDB.DB.LanguageChoices,
                LocItem.c_sEnglish) {DontLocalizeEnums = true});
            #endregion

            #region Send/Receive UserName
            m_bag.Properties.Add(new PropertySpec(
                c_sUserName,
                "User Name",
                typeof(string),
                c_sGroupSendReceive,
                "The username used to access LanguageDepot.org data; supplied by the LanguageDepot administrator.",
                ""));
            #endregion
            #region Send/Receive Password
            m_bag.Properties.Add(new PropertySpec(
                c_sPaassword,
                "Send/Receive Password",
                typeof(string),
                c_sGroupSendReceive,
                "The password used to access LanguageDepot.org data; supplied by the LanguageDepot administrator.",
                ""));
            #endregion

            #region Drafting Back Color
            m_bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorDrafting,
                "Drafting",
                c_sGroupBackgroundColors,
                "The color of the Drafting window background.",
                "Wheat"));
            #endregion
            #region Back Translation Back Color
            m_bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorBackTranslation,
                "Back Translation",
                c_sGroupBackgroundColors,
                "The color of the Back Translation window background.",
                "Wheat"));
            #endregion
            #region Consultant Prepartion Back Color
            m_bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorConsultantPreparation,
                "Consultant Preparation",
                c_sGroupBackgroundColors,
                "The color of the Consultant Preparation window background.",
                "LilghtYellow"));
            #endregion
            #region Naturalness Back Color
            m_bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorNaturalnessCheck,
                "Naturalness Check",
                c_sGroupBackgroundColors,
                "The color of the Naturalness Check window background.",
                "Wheat"));
            #endregion

            LocDB.Localize(this, m_bag);
            m_grid.SelectedObject = m_bag;
        }
        #endregion
    }


    // DlgCheckTree
    public class ProjectAccessEditor : UITypeEditor
    {
        #region smethod: void CreateCheckTreeItems(DlgCheckTree dlg)
        static void CreateCheckTreeItems(DlgCheckTree dlg)
        {
            foreach (var ci in ClusterList.Clusters)
            {
                var ciItem = new CheckTreeItem(ci.Name, false, ci);
                dlg.Items.Add(ciItem);

                foreach (var sProject in ci.GetClusterLanguageList(true))
                {
                    var bChecked = Users.Current.IsMemberOf(sProject);
                    var item = new CheckTreeItem(sProject, bChecked, sProject);
                    ciItem.SubItems.Add(item);
                }
            }
        }
        #endregion
        #region smethod: void HarvestCheckTreeItems(DlgCheckTree dlg)
        static void HarvestCheckTreeItems(DlgCheckTree dlg)
        {
            foreach (var ctiCluster in dlg.Items)
            {
                var ci = ClusterList.FindClusterInfo(ctiCluster.Name);
                if (null == ci)
                    continue;

                foreach (var ctiProject in ctiCluster.SubItems)
                {
                    if (ctiProject.Checked)
                        Users.Current.AddMembershipTo(ctiProject.Name);
                    else
                        Users.Current.RemoveMembershipFrom(ctiProject.Name);
                }
            }
        }
        #endregion

        #region OMethod: UITypeEditorEditStyle GetEditStyle(context)
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        #endregion
        #region OMethod: object EditValue(...)
        public override object EditValue(ITypeDescriptorContext context,
            IServiceProvider provider, object value)
        {
            // Set up the dialog
            var dlg = new DlgCheckTree
            {
                Label_Instruction = "Place a check beside the projects this user can access; " +
                    "or uncheck them all if they can access any project."
            };
            CreateCheckTreeItems(dlg);

            // Perform the dialog
            if (dlg.ShowDialog() == DialogResult.OK)
                HarvestCheckTreeItems(dlg);
            return Users.Current.MemberProjects;
        }
        #endregion
    }


}
