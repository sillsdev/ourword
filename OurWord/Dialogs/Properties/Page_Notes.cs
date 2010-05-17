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
        private const string c_sGroupPeople = "People";
        private const string c_sPeople = "propPeople";

        private const string c_sGroupSettings = "Settings";
        private const string c_sDefaultAuthor = "propDefaultAuthor";
        private const string c_sDismissWhenMouseLeaves = "propDismissWhenMouseLeaves";
        private const string c_sShowTitleWithNoteIcon = "propShowTitleWithNoteIcon";

        private const string c_sGroupPermissions = "Permissions";
        private const string c_sCanCreateHintForDaughter = "propCanCreateHintForDaughter";
        private const string c_sCanCreateInformationNotes = "propCanCreateInformationNote";
        private const string c_sCanCreateNotesToConsultants = "propCanCreateConsultantNote";
        private const string c_sCanCreateNotesInFront = "propCanCreateNotesInFront";
        private const string c_sCanDeleteAnything = "propCanDeleteAnything";

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
                // People Group
                case c_sPeople:
                    e.Value = DB.Project.People.ToCommaDelimitedString();
                    break;

                // Settings Group
                case c_sDefaultAuthor:
                    e.Value = DB.UserName;
                    break;
                case c_sDismissWhenMouseLeaves:
                    e.YesNoValue = TranslatorNote.DismissWhenMouseLeaves;
                    break;
                case c_sShowTitleWithNoteIcon:
                    e.YesNoValue = TranslatorNote.ShowTitleWithNoteIcon;
                    break;

                // Permissions Group
                case c_sCanDeleteAnything:
                    e.YesNoValue = TranslatorNote.CanDeleteAnything;
                    break;
                case c_sCanCreateHintForDaughter:
                    e.YesNoValue = TranslatorNote.CanCreateHintForDaughter;
                    break;
                case c_sCanCreateInformationNotes:
                    e.YesNoValue = TranslatorNote.CanCreateInformationNotes;
                    break;
                case c_sCanCreateNotesToConsultants:
                    e.YesNoValue = TranslatorNote.CanCreateConsultantNotes;
                    break;
                case c_sCanCreateNotesInFront:
                    e.YesNoValue = TranslatorNote.CanCreateNotesInFront;
                    break;
            }
        }
        #endregion
        #region SMethod: void bag_SetValue(...)
        static void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                // People Group
                case c_sPeople:
                    DB.Project.People.FromCommaDelimitedString((string)e.Value);
                    break;

                // Settings Group
                case c_sDefaultAuthor:
                    DB.UserName = (string)e.Value;
                    break;
                case c_sDismissWhenMouseLeaves:
                    TranslatorNote.DismissWhenMouseLeaves = e.YesNoValue;
                    break;
                case c_sShowTitleWithNoteIcon:
                    TranslatorNote.ShowTitleWithNoteIcon = e.YesNoValue;
                    break;

                // Permissions Group
                case c_sCanDeleteAnything:
                    TranslatorNote.CanDeleteAnything = e.YesNoValue;
                    break;
                case c_sCanCreateHintForDaughter:
                    TranslatorNote.CanCreateHintForDaughter = e.YesNoValue;
                    break;
                case c_sCanCreateInformationNotes:
                    TranslatorNote.CanCreateInformationNotes = e.YesNoValue;
                    break;
                case c_sCanCreateNotesToConsultants:
                    TranslatorNote.CanCreateConsultantNotes = e.YesNoValue;
                    break;
                case c_sCanCreateNotesInFront:
                    TranslatorNote.CanCreateNotesInFront = e.YesNoValue;
                    break;
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

            // Permissions
            #region Can Delete Other's Messages
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanDeleteAnything,
                "Can Delete Other's Notes & Messages?",
                c_sGroupPermissions,
                "If Yes, you will have the ability to delete notes and messages written by " +
                    "other people; e.g., to clean up exegetical notes after all others have " +
                    "finished commenting on them.",
                false
                ));
            #endregion
            #region Can Create Hint-For-Daughter Notes
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanCreateHintForDaughter,
                "Can create \"Hint For Daughter\" notes?",
                c_sGroupPermissions,
                "If Yes, you will have the ability to set a note's Assign To to a " +
                    "\"Hint For Daughter\" note, which will then show up on in the Source " + 
                    "translation when someone is drafting a daughter translation.",
                false
                ));
            #endregion
            #region Can Create Information Notes
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanCreateInformationNotes,
                "Can create \"Information\" notes?",
                c_sGroupPermissions,
                "If Yes, you will have the ability to set a note's Assign To to a " +
                    "\"Information\" note, which you might use for exegetical notes " +
                    "or other notes the consultant might wish to see (but not comment on).",
                false
                ));
            #endregion
            #region Can Create Notes to Consultants
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanCreateNotesToConsultants,
                "Can assign notes to the Consultant?",
                c_sGroupPermissions,
                "If Yes, you will have the ability to set a note's Assign To to " +
                    "\"Consultant,\" which means that the consultant can participate " +
                    "in the notes conversation.",
                false
                ));
            #endregion
            #region Can Create Front Notes
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanCreateNotesInFront,
                "Can create notes in the Front translation?",
                c_sGroupPermissions,
                "If Yes, you will have the ability to create a note in the front / " +
                    "source translation. Use this to notify the Front team of any " +
                    "issues.",
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
        #region Attr{g}: string Title
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


}
