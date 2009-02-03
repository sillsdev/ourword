/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Notes.cs
 * Author:  John Wimbish
 * Created: 12 Jan 2008
 * Purpose: Sets up the notes display.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public partial class Page_Notes : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_Notes(DialogProperties _ParentDlg)
            : base(_ParentDlg)
        {
            InitializeComponent();
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region BAG CONSTANTS
        const string c_sGroupColors = "Colors";
        const string c_sNotesWndBkgColor = "propNotesWndBkgColor";
        const string c_sNotesBorderColor = "propBorderColor";
        const string c_sNotesHeaderColor = "propHeaderColor";
        const string c_sNotesUneditableColor = "propUneditableolor";

        const string c_sGroupMisc = "Misc";
        const string c_sDefaultAuthor = "propDefaultAuthor";

        const string c_sGroupClassifications = "Categories & People";
        const string c_sCategories = "propCategories";
        const string c_sShowCategories = "propShowCategories";
        const string c_sPeople = "propPeople";
        const string c_sShowAssignedTo = "propShowAssignedTo";

        const string c_sYes = "Yes";
        const string c_sNo = "No";
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
        #region Method: void bag_GetValue(...)
        void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sNotesWndBkgColor:
                    e.Value = NotesWnd.RegistryBackgroundColor;
                    break;
                case c_sNotesBorderColor:
                    e.Value = TranslatorNote.BorderColor.Name;
                    break;
                case c_sNotesHeaderColor:
                    e.Value = TranslatorNote.DiscussionHeaderColor.Name;
                    break;
                case c_sNotesUneditableColor:
                    e.Value = TranslatorNote.UneditableColor.Name;
                    break;

                case c_sDefaultAuthor:
                    e.Value = Discussion.DefaultAuthor;
                    break;

                case c_sCategories:
                    e.Value = TranslatorNote.Categories.CommaDelimitedString;
                    break;
                case c_sShowCategories:
                    YesNoPropertySpec ShowCategoriesPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ShowCategoriesPS);
                    e.Value = ShowCategoriesPS.GetBoolString(TranslatorNote.ShowCategories);
                    break;
                case c_sPeople:
                    e.Value = TranslatorNote.People.CommaDelimitedString;
                    break;
                case c_sShowAssignedTo:
                    YesNoPropertySpec ShowAssignedToPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ShowAssignedToPS);
                    e.Value = ShowAssignedToPS.GetBoolString(TranslatorNote.ShowAssignedTo);
                    break;
            }
        }
        #endregion
        #region Method: void bag_SetValue(...)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sNotesWndBkgColor:
                    NotesWnd.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sNotesBorderColor:
                    TranslatorNote.BorderColor = Color.FromName( (string)e.Value );
                    break;
                case c_sNotesHeaderColor:
                    TranslatorNote.DiscussionHeaderColor = Color.FromName((string)e.Value);
                    break;
                case c_sNotesUneditableColor:
                    TranslatorNote.UneditableColor = Color.FromName((string)e.Value);
                    break;

                case c_sDefaultAuthor:
                    Discussion.DefaultAuthor = (string)e.Value;
                    break;

                case c_sCategories:
                    TranslatorNote.Categories.CommaDelimitedString = (string)e.Value;
                    break;
                case c_sShowCategories:
                    YesNoPropertySpec ShowCategoriesPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ShowCategoriesPS);
                    TranslatorNote.ShowCategories = ShowCategoriesPS.IsTrue(e.Value);
                    break;
                case c_sPeople:
                    TranslatorNote.People.CommaDelimitedString = (string)e.Value;
                    break;
                case c_sShowAssignedTo:
                    YesNoPropertySpec ShowAssignedToPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ShowAssignedToPS);
                    TranslatorNote.ShowAssignedTo = ShowAssignedToPS.IsTrue(e.Value);
                    break;
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this page
            m_bag = new PropertyBag();
            Bag.GetValue += new PropertySpecEventHandler(bag_GetValue);
            Bag.SetValue += new PropertySpecEventHandler(bag_SetValue);

            // Colors
            #region Colors
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sNotesWndBkgColor,
                "Window Background",
                c_sGroupColors,
                "The color of the Notes window background.",
                "Light Gray"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sNotesBorderColor,
                "Outer Border",
                c_sGroupColors,
                "The color of the thin borders of an individual chat/discussion.",
                "Dark Gray"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sNotesHeaderColor,
                "Header Area",
                c_sGroupColors,
                "The color of inner area of an individual chat/discussion.",
                "Light Green"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sNotesUneditableColor,
                "Non-Editable Discussion",
                c_sGroupColors,
                "The color of those discussion/chats which are not available for edit.",
                "Light Yellow"));
            #endregion

            // Categories & People
            #region Categories & People
            Bag.Properties.Add(new PropertySpec(
                c_sCategories,
                "Categories of Notes",
                typeof(string),
                c_sGroupClassifications,
                "Provide a list of the categories of notes, separated by commas. Note that any " +
                    "existing categories in the current book will be automatically added back " +
                    "into this list.",
                "",
                "",
                null
                ));
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sShowCategories,
                "Show Categories combobox?",
                c_sGroupClassifications,
                "If Yes, you will have the ability to classify a note according to its " +
                    "category, and to then turn the display of categorized notes on and off.",
                false
                ));

            Bag.Properties.Add(new PropertySpec(
                c_sPeople,
                "People we can Assign To",
                typeof(string),
                c_sGroupClassifications,
                "Provide a list of the people that a note can be assigned to, separated by " +
                    "commas. Note that any existing people in the current book will be " +
                    "automatically added back into this list.",
                "",
                "",
                null
                ));
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sShowAssignedTo,
                "Show Assigned To combobox?",
                c_sGroupClassifications,
                "If Yes, you will have the ability to assign a note to a person's attention; " +
                    "and to then turn off the display of any notes that are not assigned to you.",
                false
                ));
            #endregion

            // Misc
            #region Misc
            Bag.Properties.Add(new PropertySpec(
                c_sDefaultAuthor,
                "New Note Author's Name",
                typeof(string),
                c_sGroupMisc,
                "This defaults to your computer's name; you'll probably want to " + 
                    "your real name here, so that others will know that it was " +
                    "who wrote the note.",
                "",
                "",
                null
                ));
            #endregion

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kTranslationNotes);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
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
