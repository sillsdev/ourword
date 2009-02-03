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
            if (TranslatorNote.ShowCategories)
            {
                foreach (TranslatorNote.Classifications.Classification cat in TranslatorNote.Categories)
                {
                    ToolStripMenuItem item = cat.CreateMenuItem(
                        new EventHandler(cmdToggleClassificationChecked));

                    m_Show.DropDownItems.Add(item);
                }

                // Add a menu item to show all of the categories
                string sShowAllCategories = G.GetLoc_Notes("ShowAllCategories", "Show All Categories");
                ToolStripMenuItem itemAll = new ToolStripMenuItem(sShowAllCategories);
                itemAll.Click += new System.EventHandler(cmdTurnOnAllClassifications);
                m_Show.DropDownItems.Add(itemAll);

                // Categories from the Front Translation
                if (TranslatorNote.FrontCategories.Count > 0)
                {
                    // Add a separator
                    if (m_Show.DropDownItems.Count > 0)
                        m_Show.DropDownItems.Add(new ToolStripSeparator());

                    // Add a menu item for the Front Translation notes we want to see
                    string sFrontCategories = G.GetLoc_NoteDefs("FrontCategories", "Notes From Front Translation");
                    ToolStripMenuItem itemFromFront = new ToolStripMenuItem(sFrontCategories);
                    m_Show.DropDownItems.Add(itemFromFront);

                    // Add the Front Translation Categories to this submenu
                    foreach (TranslatorNote.Classifications.Classification cat in TranslatorNote.FrontCategories)
                    {
                        ToolStripMenuItem item = cat.CreateMenuItem(
                            new EventHandler(cmdToggleClassificationChecked));

                        itemFromFront.DropDownItems.Add(item);
                    }
                }
            }

            // "Assigned To" choice
            AddAssignedTo();

            // If nothing was added, then we don't show this in the notes pane
            m_Show.Visible = (m_Show.DropDownItems.Count > 0);
        }
        #endregion
        #region Cmd: cmdTurnOnAllClassifications
        private void cmdTurnOnAllClassifications(object sender, EventArgs e)
        {
            // Make sure all Category menu items are checked
            foreach (ToolStripItem item in m_Show.DropDownItems)
            {
                // Cast to a menu item, if possible
                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (null == menuItem)
                    continue;

                // Get the Classification from the tag
                TranslatorNote.Classifications.Classification classification =
                    menuItem.Tag as TranslatorNote.Classifications.Classification;
                if (null == classification)
                    continue;

                // Check both the menu and the Classification
                menuItem.Checked = true;
                classification.IsChecked = true;
            }

            // Regenerate the window display
            G.App.ResetWindowContents();
        }
        #endregion
        #region Cmd: cmdToggleClassificationChecked
        private void cmdToggleClassificationChecked(object sender, EventArgs e)
        {
            // Retrieve the menu item
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            // Retrieve the Category from the tag
            TranslatorNote.Classifications.Classification classification =
                item.Tag as TranslatorNote.Classifications.Classification;
            if (null == classification)
                return;

            // Toggle the checked state and reset the menu item
            bool bDisplayThisCategory = !item.Checked;
            item.Checked = bDisplayThisCategory;
            classification.IsChecked = bDisplayThisCategory;

            // Regenerate the window display
            G.App.ResetWindowContents();
        }
        #endregion

        // AssignedTo Menu Items -------------------------------------------------------------
        ToolStripMenuItem m_menuShowAllPeople;
        ToolStripMenuItem m_menuShowJustMe;
        #region Cmd: cmdAssignedToClicked
        private void cmdAssignedToClicked(object sender, EventArgs e)
        {
            // Check the menu items as requested
            m_menuShowAllPeople.Checked = (sender == m_menuShowAllPeople);
            m_menuShowJustMe.Checked = (sender == m_menuShowJustMe);

            // Update the setting this affects
            TranslatorNote.ShowAllPeople = m_menuShowAllPeople.Checked;

            // Regenerate the window display
            G.App.ResetWindowContents();
        }
        #endregion
        #region Method: void AddAssignedTo()
        void AddAssignedTo()
        {
            if (!TranslatorNote.ShowAssignedTo)
                return;

            // Add a separator
            if (m_Show.DropDownItems.Count > 0)
                m_Show.DropDownItems.Add(new ToolStripSeparator());

            // Menu item to Show All
            m_menuShowAllPeople = new ToolStripMenuItem("Show Notes Assigned to Anyone");
            m_menuShowAllPeople.Checked = TranslatorNote.ShowAllPeople;
            m_menuShowAllPeople.Click += new EventHandler(cmdAssignedToClicked);
            m_Show.DropDownItems.Add(m_menuShowAllPeople);

            // Menu item to only show current user AssignedTo's
            string sShowJustMe = "Show Notes Assigned to '" + Discussion.DefaultAuthor + "'";
            m_menuShowJustMe = new ToolStripMenuItem(sShowJustMe);
            m_menuShowJustMe.Checked = !TranslatorNote.ShowAllPeople;
            m_menuShowJustMe.Click += new EventHandler(cmdAssignedToClicked);
            m_Show.DropDownItems.Add(m_menuShowJustMe);
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

        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!G.Project.HasDataToDisplay)
                return;

            // Retrieve the notes that we will be showing, from both Front and Target
            List<TranslatorNote> vNotes = G.STarget.GetAllTranslatorNotes(true);
            vNotes.AddRange(G.SFront.GetAllTranslatorNotes(true));

            // Sort (the IComparer will do this by reference)
            vNotes.Sort();
            
            // Place them into the window
            foreach (TranslatorNote note in vNotes)
                Contents.Append( note.BuildNotesPaneView(this) );

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();

            // Make sure we're positioned at the top. For a longer set of data, we might
            // not be, because the very top of a note is not editable.
            ScrollBarPosition = 0;
        }
        #endregion

        // Save/Restore Editing State overrides ----------------------------------------------
        #region CLASS: NotesEditState : EditState
        public class NotesEditState : EditState
        {
            #region Attr{g}: List<bool> HeaderCollapseState
            List<bool> HeaderCollapseState
            {
                get
                {
                    Debug.Assert(null != m_vHeaderCollapseState);
                    return m_vHeaderCollapseState;
                }
            }
            List<bool> m_vHeaderCollapseState;
            #endregion

            #region OMethod: void Restore()
            public override void Restore()
            {
                // Bookmark
                base.Restore();

                // Restore the Header Collapse states

                // Get a list of header containers currently in the window
                var vChc = new List<ECollapsableHeaderColumn>();
                foreach (EContainer container in Wnd.Contents)
                {
                    ECollapsableHeaderColumn chc = container as ECollapsableHeaderColumn;
                    if (null != chc)
                        vChc.Add(chc);
                }

                // Are they the same length?
                if (vChc.Count != HeaderCollapseState.Count)
                    return;

                // Restore the values
                for (int i = 0; i < vChc.Count; i++)
                    vChc[i].IsCollapsed = HeaderCollapseState[i];

                // Re-do the layout
                Wnd.DoLayout();
                Wnd.Invalidate();
            }
            #endregion

            #region Constructor(OWWindow)
            public NotesEditState(OWWindow wnd)
                : base(wnd)
            {
                // Save the header collapse states
                m_vHeaderCollapseState = new List<bool>();
                foreach (EContainer container in wnd.Contents)
                {
                    ECollapsableHeaderColumn chc = container as ECollapsableHeaderColumn;
                    if (null != chc)
                        m_vHeaderCollapseState.Add(chc.IsCollapsed);
                }
            }
            #endregion
        }
        #endregion
        #region OMethod: EditState PushEditState()
        public override EditState PushEditState()
        {
            NotesEditState es = new NotesEditState(this);
            EditStateStack.Add(es);
            return es;
        }
        #endregion
    }

}
