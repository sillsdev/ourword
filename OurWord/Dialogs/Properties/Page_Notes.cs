/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Notes.cs
 * Author:  John Wimbish
 * Created: 12 Jan 2008
 * Purpose: Sets up the notes display.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using OurWordData;
using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.SideWnd;
using OurWord.Layouts;
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

			// Create a OWWindow as the one-and-only child
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region BAG CONSTANTS
        const string c_sGroupMisc = "Misc";
        const string c_sDefaultAuthor = "propDefaultAuthor";
        const string c_sCanDeleteAnything = "propCanDeleteAnything";

        const string c_sGroupClassifications = "People";
        const string c_sPeople = "propPeople";

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
                case c_sDefaultAuthor:
                    e.Value = DB.UserName;
                    break;
                case c_sCanDeleteAnything:
                    YesNoPropertySpec CanDeleteAnythingPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != CanDeleteAnythingPS);
                    e.Value = CanDeleteAnythingPS.GetBoolString(TranslatorNote.CanDeleteAnything);
                    break;

                case c_sPeople:
                    e.Value = DB.Project.People.ToCommaDelimitedString();
                    break;
            }
        }
        #endregion
        #region Method: void bag_SetValue(...)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sDefaultAuthor:
                    DB.UserName = (string)e.Value;
                    break;
                case c_sCanDeleteAnything:
                    YesNoPropertySpec CanDeleteAnythingPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != CanDeleteAnythingPS);
                    TranslatorNote.CanDeleteAnything = CanDeleteAnythingPS.IsTrue(e.Value);
                    break;

                case c_sPeople:
                    DB.Project.People.FromCommaDelimitedString((string)e.Value);
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

            // Categories & People
            #region People
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
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanDeleteAnything,
                "Can Delete Other's Notes & Messages?",
                c_sGroupMisc,
                "If Yes, you will have the ability to delete notes and messages written by " +
                    "other people; e.g., to clean up exegetical notes after all others have " +
                    "finished commenting on them.",
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
}
