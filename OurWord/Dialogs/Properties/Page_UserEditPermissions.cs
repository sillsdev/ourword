using System;
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
            InitializeComponent();

            m_UserIsNotProjectMember.OnGrantMembership = cmdGrantMembership;
            m_UserIsNotProjectMember.Hide();
        }
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

        // Buttons to change editability for all books ---------------------------------------
        #region method: void MakeAll(Editability)
        private void MakeAll(User.TranslationSettings.Editability editability)
        {
            foreach(var sBookAbbrev in DBook.BookAbbrevs)
            {
                Users.Current.SetEditability(DB.TargetTranslation.DisplayName, sBookAbbrev, editability);
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

        // Other Command Handlers ------------------------------------------------------------
        #region cmd: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
        {
            if (Users.Current.IsMemberOf(DB.TargetTranslation.DisplayName))
            {
                m_UserIsNotProjectMember.Hide();
            }
            else
            {
                m_UserIsNotProjectMember.Show();
                SetEnabling(false);
            }

            SetupPropertyGrid();
        }
        #endregion
        #region cmd: cmdGrantMembership
        private void cmdGrantMembership()
        {
            Users.Current.AddMembershipTo(DB.TargetTranslation.DisplayName);
            m_UserIsNotProjectMember.Hide();
            SetEnabling(true);
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
            // Look up the book's editability for the current user
            var sBookAbbrev = e.Property.ID;
            var editability = Users.Current.GetEditability(DB.TargetTranslation.DisplayName, 
                sBookAbbrev);

            // Set the combo box to display it
            var ps = e.Property as EnumPropertySpec;
            Debug.Assert(null != ps);
            e.Value = ps.GetEnumValueFor((int)editability);
        }
        #endregion
        #region SMethod: void bag_SetValue(...)
        static void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // Retrieve the value in the property grid
            var ps = e.Property as EnumPropertySpec;
            Debug.Assert(null != ps);
            var editability = (User.TranslationSettings.Editability) 
                ps.GetEnumNumberFor((string) e.Value);

            // Assign it to the User's setting
            var sBookAbbrev = e.Property.ID;
            Users.Current.SetEditability(DB.TargetTranslation.DisplayName, sBookAbbrev,
                editability);
        }
        #endregion
        #region method: void SetupPropertyGrid()
        void SetupPropertyGrid()
            // HACK: The PropertyGrid control does not permit me to specify my own
            // sort order for the categories. So to keep these categories froom being
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
