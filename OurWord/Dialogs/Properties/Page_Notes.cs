#region ***** Page_Notes.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Notes.cs
 * Author:  John Wimbish
 * Created: 12 Jan 2008
 * Purpose: Sets up the notes display.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;

#endregion

namespace OurWord.Dialogs
{
    public partial class Page_Notes : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_Notes(DialogProperties parentDlg)
            : base(parentDlg)
        {
            InitializeComponent();

            // Create a OWWindow as the one-and-only child
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region BAG CONSTANTS
        const string c_sGroupSettings = "Settings";
        const string c_sDefaultAuthor = "propDefaultAuthor";
        const string c_sCanDeleteAnything = "propCanDeleteAnything";
        const string c_sDismissWhenMouseLeaves = "propDismissWhenMouseLeaves";
        const string c_sShowTitleWithNoteIcon = "propShowTitleWithNoteIcon";

        private const string c_sGroupCategories = "Categories";
        private const string c_sCategories = "propCategories";
        private const string c_sCanSetCategories = "propCanSetCategories";

        const string c_sGroupPeople = "People";
        private const string c_sVisibleRoles = "propVisibleRoles";
        private const string c_sPeople = "propPeople";
        #endregion
        #region Attr{g}: PropertyBag Bag - Defines the properties to display (including localizations)
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
            switch (e.Property.ID)
            {
                case c_sDefaultAuthor:
                    {
                        e.Value = DB.UserName;
                        break;
                    }
                case c_sCanDeleteAnything:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        e.Value = ps.GetBoolString(TranslatorNote.CanDeleteAnything);
                        break;
                    }
                case c_sDismissWhenMouseLeaves:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        e.Value = ps.GetBoolString(TranslatorNote.DismissWhenMouseLeaves);
                        break;
                    }
                case c_sShowTitleWithNoteIcon:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        e.Value = ps.GetBoolString(TranslatorNote.ShowTitleWithNoteIcon);
                        break;
                    }
                case c_sPeople:
                    {
                        e.Value = DB.Project.People.ToCommaDelimitedString();
                        break;
                    }
                case c_sVisibleRoles:
                    e.Value = Role.RolesTurnedOnForThisUser;
                    break;

                case c_sCategories:
                    {
                        e.Value = DB.TeamSettings.NotesCategories.ToCommaDelimitedString();
                        break;
                    }
                case c_sCanSetCategories:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        e.Value = ps.GetBoolString(TranslatorNote.CanSetCategories);
                        break;
                    }

            }
        }
        #endregion
        #region SMethod: void bag_SetValue(...)
        static void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sDefaultAuthor:
                    {
                        DB.UserName = (string)e.Value;
                        break;
                    }
                case c_sCanDeleteAnything:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        TranslatorNote.CanDeleteAnything = ps.IsTrue(e.Value);
                        break;
                    }
                case c_sDismissWhenMouseLeaves:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        TranslatorNote.DismissWhenMouseLeaves = ps.IsTrue(e.Value);
                        break;
                    }
                case c_sShowTitleWithNoteIcon:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        TranslatorNote.ShowTitleWithNoteIcon = ps.IsTrue(e.Value);
                        break;
                    }
                case c_sPeople:
                    {
                        DB.Project.People.FromCommaDelimitedString((string)e.Value);
                        break;
                    }
                case c_sVisibleRoles:
                    break;

                case c_sCategories:
                    {
                        DB.TeamSettings.NotesCategories.FromCommaDelimitedString((string)e.Value);
                        break;
                    }
                case c_sCanSetCategories:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        TranslatorNote.CanSetCategories = ps.IsTrue(e.Value);
                        break;
                    }
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this page
            m_bag = new PropertyBag();
            Bag.GetValue += bag_GetValue;
            Bag.SetValue += bag_SetValue;

            // Categories
            #region Categories
            Bag.Properties.Add(new PropertySpec(
                c_sCategories,
                "Notes categories",
                typeof(string),
                c_sGroupCategories,
                "You can optionally provide a list of categories for classifying notes, such as " +
                    "Exegetical, Study Bible, etc.",
                "",
                "",
                null
                ));
            #endregion
            #region Can Set Notes Categories
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanSetCategories,
                "Can Set Notes Categories?",
                c_sGroupCategories,
                "If Yes, you will have the ability to change the category of a note. If No, " + 
                    "the Categories dropdown will not even be visible.",
                false
                ));
            #endregion

            // People
            #region People
            Bag.Properties.Add(new PropertySpec(
                c_sPeople,
                "People for History",
                typeof(string),
                c_sGroupPeople,
                "Provide a list of the people in this project whom you wish to reference when " +
                    "filling out the History.",
                "",
                "",
                null
                ));
            #endregion
            #region This user can assign notes to
            Bag.Properties.Add(new PropertySpec(
                c_sVisibleRoles,
                "This user can assign notes to",
                typeof(string),
                c_sGroupPeople,
                "By default the user can assign to Translator, Advisor, Anyone and Closed." +
                    "Use this to assign to addition people in the project.",
                Role.NoExtraRoles,
                typeof(RolesAccessEditor),
                null));
            #endregion

            // Settings
            #region Author
            Bag.Properties.Add(new PropertySpec(
                c_sDefaultAuthor,
                "New Note Author's Name",
                typeof(string),
                c_sGroupSettings,
                "This defaults to your computer's name; you'll probably want " +
                    "your real name here, so that others will know that " +
                    "who wrote the note.",
                "",
                "",
                null
                ));
            #endregion
            #region Can Delete Other's Messages
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanDeleteAnything,
                "Can Delete Other's Notes & Messages?",
                c_sGroupSettings,
                "If Yes, you will have the ability to delete notes and messages written by " +
                    "other people; e.g., to clean up exegetical notes after all others have " +
                    "finished commenting on them.",
                false
                ));
            #endregion
            #region Dismiss when Mouse Leaves
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sDismissWhenMouseLeaves,
                "Close Notes Window when mouse leaves it?",
                c_sGroupSettings,
                "If Yes, when you move your mouse outside of the Notes window, it will " +
                    "disappear. If No, you must click on the Close button to dismiss " +
                    "the window.",
                false
                ));
            #endregion
            #region Show Title With Note Icon
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sShowTitleWithNoteIcon,
                "Show title beside note icon?",
                c_sGroupSettings,
                "If Yes, then the title of the note will appear beside the note icon so " +
                    "that you can see what the note is about without having to launch " +
                    "the notes window.",
                false
                ));
            #endregion

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region OAttr{g}: string ID
        public override string ID
        {
            get
            {
                return "idNotes";
            }
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kTranslationNotes);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Notes";
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
            // Property Grid
            SetupPropertyGrid();
        }
        #endregion

    }

    public class RolesAccessEditor : UITypeEditor
    {
        #region SMethod: void HarvestAnswers(DlgCheckTree dlg)
        static void HarvestAnswers(DlgCheckTree dlg)
        {
            foreach(var item in dlg.Items)
            {
                var role = item.Tag as Role;
                Debug.Assert(null != role);
                role.ThisUserCanAccess = item.Checked;
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
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // Set up the dialog
            var dlg = new DlgCheckTree
            {
                Label_Instruction = "Place a check beside the people to whom this person " +
                    "can assign a note.",
                Height = 250
            };

            foreach (var role in Role.AllRoles)
            {
                if (!role.AlwaysAvailable)
                {
                    var item = new CheckTreeItem(role.LocalizedName, role.ThisUserCanAccess, role);
                    dlg.Items.Add(item);
                }
            }

            // Perform the dialog
            if (dlg.ShowDialog() == DialogResult.OK)
                HarvestAnswers(dlg);
            dlg.Dispose();
            return ClusterList.UserCanAccessAllProjectsFriendly; 
        }
        #endregion
    }


}
