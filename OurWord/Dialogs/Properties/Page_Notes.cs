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
        const string c_sShow = "Show_";
        const string c_sColor = "Color_";
        const string c_sShowNotesFromFront = "ShowNotesFromFront";
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
            // ShowHintsFromFront requires additional treatment
            if (e.Property.ID == c_sShowNotesFromFront)
            {
                YesNoPropertySpec ps = e.Property as YesNoPropertySpec;
                Debug.Assert(null != ps);
                e.Value = ps.GetBoolString(DNote.ShowHintsFromFront);
                return;
            }

            // All other notes
            foreach (DNoteDef notedef in DNote.NoteDefs)
            {
                if (e.Property.ID == (c_sShow + notedef.NoteType.ToString()))
                {
                    YesNoPropertySpec ps = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ps);
                    e.Value = ps.GetBoolString(notedef.Show);
                }

                if (e.Property.ID == (c_sColor + notedef.NoteType.ToString()))
                {
                    e.Value = notedef.BackgroundColorName;
                }
            }
        }
        #endregion
        #region Method: void bag_SetValue(...)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // ShowHintsFromFront requires additional treatment
            if (e.Property.ID == c_sShowNotesFromFront)
            {
                YesNoPropertySpec ps = e.Property as YesNoPropertySpec;
                Debug.Assert(null != ps);
                DNote.ShowHintsFromFront = ps.IsTrue(e.Value);
                return;
            }

            // All other notes
            foreach (DNoteDef notedef in DNote.NoteDefs)
            {
                if (e.Property.ID == (c_sShow + notedef.NoteType.ToString()))
                {
                    YesNoPropertySpec ps = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ps);
                    notedef.Show = ps.IsTrue(e.Value);
                }

                if (e.Property.ID == (c_sColor + notedef.NoteType.ToString()))
                {
                    notedef.BackgroundColorName = (string)e.Value;
                }
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

            // Each note will be in its own group (Category)
            foreach (DNoteDef notedef in DNote.NoteDefs)
            {
                // Whether or not to show the note
                if (!notedef.IsCombined)
                {
                    // Whether or not the note is visible
                    Bag.Properties.Add( new YesNoPropertySpec(
                        c_sShow + notedef.NoteType.ToString(),
                        "Show?",
                        notedef.NoteType.ToString(),
                        "If yes, the " + notedef.EnglishName + " Note will be visible when " +
                            "the Notes Pane is active; it will show up both in the Notes " +
                            "Pane and as icons in the text.",
                        notedef.ShowDefault
                        ));

                    // The background color for the note
                    Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                        c_sColor + notedef.NoteType.ToString(),
                        "Background Color",
                        notedef.NoteType.ToString(),
                        "The color for the background of the " + notedef.EnglishName + 
                            " Note in the Notes Pane.",
                        notedef.BackgroundColorName));

                    // ShowHintsFromFront requires additional treatment
                    if (notedef.NoteType == DNote.Types.kHintForDaughter)
                    {
                        Bag.Properties.Add(new YesNoPropertySpec(
                            c_sShowNotesFromFront,
                            "Show Hints from the Front Translation?",
                            notedef.NoteType.ToString(),
                            "If yes, any Hint For Daughter notes that were defined in the Front " +
                                "Translation will be visible.",
                            false
                            ));
                    }
                }
            }

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
            //HelpSystem.ShowPage_Translation();
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
