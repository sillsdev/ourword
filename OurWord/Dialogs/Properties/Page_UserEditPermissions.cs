using System.Diagnostics;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Properties
{
    public partial class Page_UserEditPermissions : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DialogProperties)
        public Page_UserEditPermissions(DialogProperties parentDlg)
            : base(parentDlg)
        {
            InitializeComponent();        }
        #endregion
        #region method: void SetEnabling(bEnabled)
        void SetEnabling(bool bEnabled)
        {
            m_gridBookByBook.Enabled = bEnabled;
            m_btnFullEditing.Enabled = bEnabled;
            m_btnNotesOnly.Enabled = bEnabled;
            m_btnReadOnly.Enabled = bEnabled;
            m_labelMakeAll.Enabled = bEnabled;
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
            return true;
        }
        #endregion

        // List of Translations Combo --------------------------------------------------------
        private string m_CurrentTranslationName;
        #region method: void SetupPermissionsForCombo()
        void SetupPermissionsForCombo()
        {
            m_cPermissionsFor.Items.Clear();

            var ci = ClusterList.FindClusterInfo(DB.TeamSettings.DisplayName);
            if (null == ci)
                return;

            var vProjectNames = ci.GetClusterLanguageList(true);
            foreach (var sProjectName in vProjectNames)
                m_cPermissionsFor.Items.Add(sProjectName);

            m_cPermissionsFor.Text = m_CurrentTranslationName;
        }
        #endregion
        #region cmd: void cmdPermissionsForChanged(object sender, System.EventArgs e)
        private void cmdPermissionsForChanged(object sender, System.EventArgs e)
        {
            var sProjectName = m_cPermissionsFor.Text;

            m_CurrentTranslationName = sProjectName;

            SetMembershipComboValue();
            SetupPropertyGrid();

            var sBase = Loc.GetString("kPermissionsFor", "{0}'s Permissions for {1}");
            m_group.Text = LocDB.Insert(sBase, 
                new[] {Users.Current.UserName, m_CurrentTranslationName});

        }
        #endregion

        // Buttons to change editability for all books ---------------------------------------
        #region method: void MakeAll(Editability)
        private void MakeAll(User.TranslationSettings.Editability editability)
        {
            foreach(var sBookAbbrev in DBook.BookAbbrevs)
            {
                Users.Current.SetBookEditability(m_CurrentTranslationName, sBookAbbrev, editability);
            }
            SetupPropertyGrid();
        }
        #endregion
        #region Cmd: cmdMakeAllFullEditing
        private void cmdMakeAllFullEditing(object sender, System.EventArgs e)
        {
            MakeAll(User.TranslationSettings.Editability.Full);
        }
        #endregion
        #region Cmd: cmdMakeAllNotesOnly
        private void cmdMakeAllNotesOnly(object sender, System.EventArgs e)
        {
            MakeAll(User.TranslationSettings.Editability.Notes);

        }
        #endregion
        #region Cmd: cmdMakeAllReadOnly
        private void cmdMakeAllReadOnly(object sender, System.EventArgs e)
        {
            MakeAll(User.TranslationSettings.Editability.ReadOnly);
        }
        #endregion

        // Membership Combo ------------------------------------------------------------------
        #region sattr{g}: string FullEditing
        static string FullEditing
        {
            get
            {
                return Loc.GetString("kUserFullEditing", "Full Editing Priviledges");
            }
        }
        #endregion
        #region sattr{g}: string CanMakeNotes
        static string CanMakeNotes
        {
            get
            {
                return Loc.GetString("kUserCanMakeNotes", "Can Only Make Notes");
            }
        }
        #endregion
        #region sattr{g}: string ReadOnly
        static string ReadOnly
        {
            get
            {
                return Loc.GetString("kUserReadOnly", "Read Only");
            }
        }
        #endregion
        #region sattr{g}: string CustomBookByBook
        static string CustomBookByBook
        {
            get
            {
                return Loc.GetString("kUserBookByBook", "Customize Book By Book");
            }
        }
        #endregion
        #region cmd: cmdGlobalEditingChanged
        private void cmdGlobalEditingChanged(object sender, System.EventArgs e)
        {
            // Full Editing
            if (FullEditing == m_comboMembership.Text)
            {
                Users.Current.FindOrAdd(m_CurrentTranslationName).GlobalEditability = 
                    User.TranslationSettings.Editability.Full;
                SetEnabling(false);
            }

            // Read Only
            else if (ReadOnly == m_comboMembership.Text)
            {
                Users.Current.FindOrAdd(m_CurrentTranslationName).GlobalEditability =
                   User.TranslationSettings.Editability.ReadOnly;
                SetEnabling(false);
            }

            // Can Make Notes
            else if (CanMakeNotes == m_comboMembership.Text)
            {
                Users.Current.FindOrAdd(m_CurrentTranslationName).GlobalEditability =
                   User.TranslationSettings.Editability.Notes;
                SetEnabling(false);
            }

            // Custom
            else if (CustomBookByBook == m_comboMembership.Text)
            {
                Users.Current.FindOrAdd(m_CurrentTranslationName).GlobalEditability =
                   User.TranslationSettings.Editability.Custom;
                SetEnabling(true);
            }

            SetupPropertyGrid();
        }
        #endregion
        #region method: void SetupMembershipComboOptions()
        void SetupMembershipComboOptions()
        {
            m_comboMembership.Items.Clear();
            m_comboMembership.Items.Add(FullEditing);
            m_comboMembership.Items.Add(CanMakeNotes);
            m_comboMembership.Items.Add(ReadOnly);
            m_comboMembership.Items.Add(CustomBookByBook);
        }
        #endregion
        #region method: void SetMembershipComboValue()
        void SetMembershipComboValue()
        {
            switch (Users.Current.FindOrAdd(m_CurrentTranslationName).GlobalEditability)
            {
                case User.TranslationSettings.Editability.Full:
                    m_comboMembership.Text = FullEditing;
                    break;

                case User.TranslationSettings.Editability.Notes:
                    m_comboMembership.Text = CanMakeNotes;
                    break;

                case User.TranslationSettings.Editability.ReadOnly:
                    m_comboMembership.Text = ReadOnly;
                    break;

                case User.TranslationSettings.Editability.Custom:
                    m_comboMembership.Text = CustomBookByBook;
                    SetEnabling(true);
                    break;

                default:
                    Debug.Assert(false, "Unknown editability");
                    break;
            }
        }
        #endregion

        // Other Command Handlers ------------------------------------------------------------
        #region cmd: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
        {
            SetEnabling(false);

            // Combo: Which project we're doing the permissions for
            m_CurrentTranslationName = DB.TargetTranslation.DisplayName;
            SetupPermissionsForCombo();

            // Combo: Global permissions options
            SetupMembershipComboOptions();
            SetMembershipComboValue();

            // Property Grid: for setting permissions book-by-book
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
        #region method: void bag_GetValue(...)
        void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
            // Look up the book's editability for the current user
            var sBookAbbrev = e.Property.ID;
            var editability = Users.Current.GetBookEditability(m_CurrentTranslationName, sBookAbbrev);

            // Set the combo box to display it
            var ps = e.Property as EnumPropertySpec;
            Debug.Assert(null != ps);
            e.Value = ps.GetEnumValueFor((int)editability);
        }
        #endregion
        #region method: void bag_SetValue(...)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // Retrieve the value in the property grid
            var ps = e.Property as EnumPropertySpec;
            Debug.Assert(null != ps);
            var editability = (User.TranslationSettings.Editability) 
                ps.GetEnumNumberFor((string) e.Value);

            // Assign it to the User's setting
            var sBookAbbrev = e.Property.ID;
            Users.Current.SetBookEditability(m_CurrentTranslationName, sBookAbbrev, editability);
        }
        #endregion
        #region method: void SetupPropertyGrid()
        void SetupPropertyGrid()
            // HACK: The PropertyGrid control does not permit me to specify my own
            // sort order for the categories. So to keep these categories from being
            // sorted, we append tabs \t in front of them. The one to sort first has
            // the most number of tabs, and then they decrease with each one. The
            // PropertyGrid uses the tabs for sorting, but does not display them.
            // Of course, a future .Net could mess this up at any time, so it is 
            // indeed risky and undesirable; yet I sure didn't want to do 01-Pentateuch,
            // 02-Historical, etc.
        {
            // Create the PropertyBag for this page
            m_bag = new PropertyBag();
            Bag.GetValue += bag_GetValue;
            Bag.SetValue += bag_SetValue;

            // HACK, see above
            var iTabs = G.BookGroups.Count;

            // Populate an item for each group/book
            foreach (var group in G.BookGroups)
            {
                // HACK, see above
                var sGroupName = new string('\t', iTabs--) + group.LocalizedName;

                foreach (var bookInfo in group)
                {
                    var sBookName = DBook.GetBookName(bookInfo.Abbrev, DB.TargetTranslation);

                    Bag.Properties.Add(new  EnumPropertySpec(
                        bookInfo.Abbrev,
                        sBookName,
                        sGroupName,
                        "Choose an edit permission for this book.",
                        typeof(User.TranslationSettings.Editability),
                        new[] { 
                            (int)User.TranslationSettings.Editability.Full, 
                            (int)User.TranslationSettings.Editability.Notes,
                            (int)User.TranslationSettings.Editability.ReadOnly
                        },
                        new[] { 
                            "Full Editing", 
                            "Can only make Notes", 
                            "Read Only" 
                        },
                        "Full Editing"
                        ) {DontLocalizeName = true, DontLocalizeCategory = true});
                }
            }

            // Set the Property Grid to this PropertyBag
            m_gridBookByBook.SelectedObject = Bag;
        }
        #endregion
    }
}
