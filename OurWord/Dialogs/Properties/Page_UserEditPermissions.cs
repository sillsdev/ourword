using System.Diagnostics;
using JWTools;

namespace OurWord.Dialogs.Properties
{
    public partial class Page_UserEditPermissions : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DialogProperties)
        public Page_UserEditPermissions(DialogProperties parentDlg)
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
                return "idUserEditPermissions";
            }
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kTableOfContents);
        }
        #endregion
        #region Attr{g}: string Title
        public override string Title
        {
            get
            {
                return "Editing Permissions";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
 //           var user = Users.Current;

            return true;
        }
        #endregion

        private void cmdMakeAllFullEditing(object sender, System.EventArgs e)
        {

        }

        private void cmdMakeAllNotesOnly(object sender, System.EventArgs e)
        {

        }

        private void cmdMakeAllReadOnly(object sender, System.EventArgs e)
        {

        }

        #region Cmd: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
        {
            SetupPropertyGrid();
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region Attr{g}: PropertyBag Bag
        PropertyBag Bag
        {
            get
            {
                Debug.Assert(null != m_bag);
                return m_bag;
            }
        }
        PropertyBag m_bag;
        #endregion

        #region SMethod: void bag_GetValue(...)
        static void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
        }
        #endregion
        #region SMethod: void bag_SetValue(...)
        static void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
        }
        #endregion
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this page
            m_bag = new PropertyBag();
            Bag.GetValue += bag_GetValue;
            Bag.SetValue += bag_SetValue;



            // Set the Property Grid to this PropertyBag
            m_gridBookByBook.SelectedObject = Bag;
        }


    }
}
