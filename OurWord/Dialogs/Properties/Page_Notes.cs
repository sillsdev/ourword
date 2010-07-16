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
using OurWordData.DataModel.Membership;

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
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region BAG CONSTANTS
        private const string c_sGroupPeople = "People";
        private const string c_sPeople = "propPeople";

        /*
        */

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

                    /*
                    */
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

                    /*
                    */
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

            /*
            */

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
            var user = Users.Current;

            user.CanMakeNotes = m_checkTurnOnNotes.Checked;
            user.NoteAuthorsName = m_textNoteAuthorName.Text;
            user.CloseNotesWindowWhenMouseLeaves = m_checkCloseWindowWhemMouseLeaves.Checked;
            user.ShowExpandedNotesIcon = m_checkShowTitleBesideIcon.Checked;
            user.CanDeleteNotesAuthoredByOthers = m_checkCanDeleteNotes.Checked;
            user.CanAuthorHintForDaughterNotes = m_checkCanCreateHintForDaughterNotes.Checked;
            user.CanAuthorInformationNotes = m_checkCanCreateInformationNotes.Checked;
            user.CanAssignNoteToConsultant = m_checkCanAssignToConsultant.Checked;
            user.CanCreateFrontTranslationNotes = m_checkCreateFrontNotes.Checked;

            return true;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Property Grid
            SetupPropertyGrid();

            var user = Users.Current;
            m_checkTurnOnNotes.Checked = user.CanMakeNotes;
            m_textNoteAuthorName.Text = Users.Current.NoteAuthorsName;
            m_textNoteAuthorName.Text = user.NoteAuthorsName;
            m_checkCloseWindowWhemMouseLeaves.Checked = user.CloseNotesWindowWhenMouseLeaves;
            m_checkShowTitleBesideIcon.Checked = user.ShowExpandedNotesIcon;
            m_checkCanDeleteNotes.Checked = user.CanDeleteNotesAuthoredByOthers;
            m_checkCanCreateHintForDaughterNotes.Checked = user.CanAuthorHintForDaughterNotes;
            m_checkCanCreateInformationNotes.Checked = user.CanAuthorInformationNotes;
            m_checkCanAssignToConsultant.Checked = user.CanAssignNoteToConsultant;
            m_checkCreateFrontNotes.Checked = user.CanCreateFrontTranslationNotes;

            SetEnabling(m_checkTurnOnNotes.Checked);
        }
        #endregion

        #region Cmd: cmdTurnOnNotesCheckChanged
        private void cmdTurnOnNotesCheckChanged(object sender, EventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (null == checkbox)
                return;

            SetEnabling(checkbox.Checked);
        }
        #endregion
        #region Method: void SetEnabling(bEnabled)
        private void SetEnabling(bool bEnabled)
        {
            m_labelAuthorName.Enabled = bEnabled;
            m_textNoteAuthorName.Enabled = bEnabled;
            m_checkCloseWindowWhemMouseLeaves.Enabled = bEnabled;
            m_checkShowTitleBesideIcon.Enabled = bEnabled;
            m_checkCanDeleteNotes.Enabled = bEnabled;
            m_checkCanCreateHintForDaughterNotes.Enabled = bEnabled;
            m_checkCanCreateInformationNotes.Enabled = bEnabled;
            m_checkCanAssignToConsultant.Enabled = bEnabled;
            m_checkCreateFrontNotes.Enabled = bEnabled;
        }
        #endregion

    }


}
