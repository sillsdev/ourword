/**********************************************************************************************
 * Project: OurWord!
 * File:    NotesPane.cs
 * Author:  John Wimbish
 * Created: 26 Feb 2008
 * Purpose: Internals of the Tab Page for displaying Notes
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.View;
using OurWord.Edit;
#endregion

// TODO: Decise on the note's editability when creating the view. E.g., Hints For Daughters
//   are not editable. Neither are paragraphs earlier written, as per my design document.

namespace OurWord.Edit
{
    public partial class NotesPane : UserControl
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: NotesWnd WndNotes
        public NotesWnd WndNotes
        {
            get
            {
                return m_wndNotes;
            }
        }
        NotesWnd m_wndNotes;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public NotesPane()
        {
            InitializeComponent();

            // Create and add the Notes OWWindow
            m_wndNotes = new NotesWnd(this);
            Controls.Add(WndNotes);
        }
        #endregion

        // Layout ----------------------------------------------------------------------------
        #region Method: void SetSize(Size sz)
        public void SetSize(Size sz)
        {
            this.Size = sz;

            // Extend the toolstrip across the entire size
            m_toolstripNotes.Location = new Point(0, 0);
            m_toolstripNotes.Width = sz.Width;

            // Position the Notes Window
            WndNotes.Location = new Point(0, m_toolstripNotes.Height);
            WndNotes.SetSize(sz.Width, sz.Height - m_toolstripNotes.Height);
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Can: canInsertNote
        public bool canInsertNote
        {
            get
            {
                // If the Main Window is not focused, then we don't have a
                // context for inserting notes.
                Debug.Assert(null != G.App.MainWindow);
                if (!G.App.MainWindow.Focused)
                    return false;

                return true;
            }
        }
        #endregion
        #region Cmd: cmdInsertNote
        private void cmdInsertNote(object sender, EventArgs e)
        {
            // Double-check that we think we can insert. In theory the menu
            // item would have been disabled and so we would not get here if
            // canInsertNote returns false.
            if (!canInsertNote)
                return;

            // Retrieve the DText to which this note will be attached
            if (G.App.MainWindow.Selection == null)
                return;
            DText text = G.App.MainWindow.Selection.Anchor.BasicText as DText;

            // Remember the location, as we have to reset the scroll position 
            // after we regenerate window contents
            OWBookmark bookmark = new OWBookmark(G.App.MainWindow.Selection);

            // Create a blank note and insert it
            TranslatorNote note = new TranslatorNote(
                text.Section.GetReferenceAt(text).ParseableName,
                G.App.MainWindow.Selection.SelectionString
                );
            note.Discussions.Append(new Discussion());
            text.TranslatorNotes.Append(note);

            // Recalculate the display
            G.App.ResetWindowContents();

            // Return the Main Window to where it was
            bookmark.RestoreWindowSelectionAndScrollPosition();

            // Select the new note and bring the focus to it
            EContainer container = WndNotes.Contents.FindContainerOfDataSource(
                note.LastParagraph);
            container.Select_LastWord_End();
            WndNotes.Focus();
        }
        #endregion

        #region Can: bool canDeleteNote
        public bool canDeleteNote
        {
            get
            {
                // If the notes window does not have focus, then we can't
                // delete notes, because the editing context is not there.
                //
                // By implication, if there is focus here, then it means
                // we have an InsertionPoint, which thus means we have
                // a Note that can conceivably be deleted.
                if (!WndNotes.Focused)
                    return false;

                // Otherwise, we return a tentative "true", recognizing that the
                // Delete method will have to do further error condition checking.
                return true;
            }
        }
        #endregion
        #region Cmd: cmdDeleteNote
        private void cmdDeleteNote(object sender, EventArgs e)
        {
            // Double-check that we think we can delete. 
            if (!canDeleteNote)
                return;

            // Request the targeted TranslatorNote from the NotesWnd
            Debug.Assert(null != WndNotes);
            TranslatorNote tn = WndNotes.GetSelectedNote();
            if (null == tn)
                return;

            // Give the user the opportunity to change his/her mind
            string sText = tn.Context;
            if (sText.Length > 40)
                sText = sText.Substring(0, 40) + "...";
            string sMsgAddition = "\n\n\"" + sText + "\"";
            if (false == Messages.ConfirmNoteDeletion(sMsgAddition))
                return;

            // Remove the note from the paragraph (we delete it from the
            // owning DText)
            DText text = tn.Text;
            Debug.Assert(null != text);
            text.TranslatorNotes.Remove(tn);

            // Regenerate the windows
            G.App.ResetWindowContents();
        }
        #endregion
        #region Cmd: cmdLoad - Localize the toolstrip
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(m_toolstripNotes);

            // Set this up after calling Localize, as we don't want to change these
            // from what the advisor will typically have set up. Those few items
            // in the menu that need to be localized are done by the method.
            SetupShowDropdown();
        }
        #endregion

        // Visibility and Enabling -----------------------------------------------------------
        #region Method: void SetControlsVisibility()
        public void SetControlsVisibility()
        {
            m_btnDeleteNote.Visible = OurWordMain.Features.F_CanDeleteNote;
        }
        #endregion
        #region Method: void SetControlsEnabling()
        public void SetControlsEnabling()
            // The idea is that we insert notes when we are in the main window, but
            // not when we are in the notes pane. And we can only delete notes if
            // we are in the notes pane.
        {
            m_btnInsert.Enabled = canInsertNote;
            m_btnDeleteNote.Enabled = canDeleteNote;
        }
        #endregion
        #region Method: void SetupShowDropdown()
        public void SetupShowDropdown()
        {
            // Start with an empty list
            m_Show.DropDownItems.Clear();

            // Add the categories
            foreach (string s in TranslatorNote.Categories)
            {
                // Create a new menu item
                ToolStripMenuItem item = new ToolStripMenuItem(s);

                // Retrieve the checked state from the registry
                item.CheckState = TranslatorNote.GetCategoryIsChecked(s) ?
                    CheckState.Checked : CheckState.Unchecked;

                // Set the event handler
                item.Click += new System.EventHandler(cmdToggleCategoryChecked);

                // We'll use this for the Show All command
                item.Tag = "category";

                // Add it to the menu list
                m_Show.DropDownItems.Add(item);
            }

            // Add a menu item to show all of the categories
            string sShowAllCategories = G.GetLoc_Notes("ShowAllCategories", "Show All Categories");
            ToolStripMenuItem itemAll = new ToolStripMenuItem(sShowAllCategories);
            itemAll.Click += new System.EventHandler(cmdTurnOnAllCategories);
            m_Show.DropDownItems.Add(itemAll);

            // Show Notes from the Front Translation
            if (TranslatorNote.FrontCategories.Count > 0)
            {
                // Add a menu item for the Front Translation notes we want to see
                string sFrontCategories = G.GetLoc_NoteDefs("FrontCategories", "Notes From Front Translation");
                ToolStripMenuItem itemFromFront = new ToolStripMenuItem(sFrontCategories);
                m_Show.DropDownItems.Add(itemFromFront);

                // Add the Front Translation Categories to this submenu
                foreach (string s in TranslatorNote.FrontCategories)
                {
                    // Create a new menu item
                    ToolStripMenuItem item = new ToolStripMenuItem(s);

                    // Retrieve the checked state
                    item.CheckState = TranslatorNote.GetFrontCategoryIsChecked(s) ?
                        CheckState.Checked : CheckState.Unchecked;

                    // Set the event handler
                    item.Click += new System.EventHandler(cmdToggleFrontCategoryChecked);

                    // Add it to the menu list
                    itemFromFront.DropDownItems.Add(item);
                }
            }
        }
        #endregion
        #region Cmd: cmdTurnOnAllCategories
        private void cmdTurnOnAllCategories(object sender, EventArgs e)
        {
            // Make sure all Category menu items are checked
            foreach (ToolStripMenuItem item in m_Show.DropDownItems)
            {
                if ((string)item.Tag == "category")
                {
                    item.Checked = true;
                    string sCategory = item.Text;
                    TranslatorNote.SetCategoryIsChecked(sCategory, true);
                }
            }

            // Regenerate the window display
            G.App.ResetWindowContents();
        }
        #endregion
        #region Cmd: cmdToggleCategoryChecked
        private void cmdToggleCategoryChecked(object sender, EventArgs e)
        {
            // Determine which category has changed
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;
            string sCategory = item.Text;

            // Toggle the checked state and reset the menu item
            bool bDisplayThisCategory = !item.Checked;
            item.Checked = bDisplayThisCategory;

            // Place the setting into the registry
            TranslatorNote.SetCategoryIsChecked(sCategory, bDisplayThisCategory);

            // Regenerate the window display
            G.App.ResetWindowContents();
        }
        #endregion
        #region Cmd: cmdToggleFrontCategoryChecked
        private void cmdToggleFrontCategoryChecked(object sender, EventArgs e)
        {
            // Determine which category has changed
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;
            string sCategory = item.Text;

            // Toggle the checked state and reset the menu item
            bool bDisplayThisCategory = !item.Checked;
            item.Checked = bDisplayThisCategory;

            // Place the setting into the registry
            TranslatorNote.SetFrontCategoryIsChecked(sCategory, bDisplayThisCategory);

            // Regenerate the window display
            G.App.ResetWindowContents();
        }
        #endregion

    }

    public class NotesWnd : OWWindow
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: NotesPane NotesPane
        NotesPane NotesPane
        {
            get
            {
                Debug.Assert(null != m_NotesPane);
                return m_NotesPane;
            }
        }
        NotesPane m_NotesPane;
        #endregion
        #region SAttr{g/s}: string RegistryBackgroundColor - background color for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "LightGray");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "Notes";
        const int c_cColumnCount = 1;
        #region Constructor(NotesPane)
        public NotesWnd(NotesPane _NotesPane)
            : base(c_sName, c_cColumnCount)
        {
            m_NotesPane = _NotesPane;
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("NotesWindowName", "Notes");
            }
        }
        #endregion
        #region Cmd: OnGotFocus - make sure commands are properly enabled
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            NotesPane.SetControlsEnabling();
        }
        #endregion
        #region Cmd: OnLostFocus - make sure commands are proeprly enabled
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            NotesPane.SetControlsEnabling();
        }
        #endregion

        #region Method: override void OnSelectAndScrollToNote(TranslatorNote)
        public override void OnSelectAndScrollToNote(TranslatorNote note)
        {
            EContainer container = Contents.FindContainerOfDataSource(note.FirstParagraph);
            if (null != container)
            {
                if (container.Select_FirstWord())
                    Focus();
            }
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: void AddNote(TranslatorNote)
        public void AddNote(TranslatorNote note)
        {
            Contents.Append(note.BuildNotesPaneView());
        }
        #endregion

        #region Method: TranslatorNote GetSelectedNote()
        public TranslatorNote GetSelectedNote()
        {
            if (Selection == null)
                return null;

            DParagraph p = Selection.Paragraph.DataSource as DParagraph;
            Debug.Assert(null != p);

            Discussion d = p.Owner as Discussion;
            if (null == d)
                return null;

            TranslatorNote tn = d.Owner as TranslatorNote;
            return tn;
        }
        #endregion

    }

}
