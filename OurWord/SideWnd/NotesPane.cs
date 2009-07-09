#region ***** NotesPane.cs *****
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
using JWdb.DataModel;
using OurWord.View;
using OurWord.Edit;
#endregion
#endregion

namespace OurWord.SideWnd
{
    public partial class NotesPane : UserControl, ISideWnd
    {
        // ISideWnd --------------------------------------------------------------------------
        #region Method: void SetSize(Size sz)
        public void SetSize(Size sz)
        {
            this.Size = sz;

            // Extend the toolstrip across the entire size
            m_toolstripNotes.Location = new Point(0, 0);
            m_toolstripNotes.Width = sz.Width;

            // Position the Notes Window
            Window.Location = new Point(0, m_toolstripNotes.Height);
            Window.SetSize(sz.Width, sz.Height - m_toolstripNotes.Height);
        }
        #endregion
        #region Attr{g}: OWWindow Window
        public OWWindow Window
        {
            get
            {
                return m_Window;
            }
        }
        NotesWnd m_Window;
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public NotesPane()
        {
            InitializeComponent();

            // Create and add the Notes OWWindow
            m_Window = new NotesWnd(this);
            Controls.Add(Window);
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

            (new InsertNoteAction(Window as NotesWnd)).Do();
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
                if (!Window.Focused)
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
            NotesWnd wnd = Window as NotesWnd;
            Debug.Assert(null != wnd);
            TranslatorNote tn = wnd.GetSelectedNote();
            if (null == tn)
                return;

            // If we have more than one discussion, then we just want to remove that
            // last discussion, not the entire note
            if (tn.Discussions.Count > 1)
            {
                // Give the user the opportunity to change his/her mind
                string sRemoveDiscussionMsg = tn.LastDiscussion.Paragraphs[0].AsString;
                if (sRemoveDiscussionMsg.Length > 40)
                    sRemoveDiscussionMsg = sRemoveDiscussionMsg.Substring(0, 40) + "...";
                if (false == Messages.ConfirmDiscussionDeletion("\n\n\"" + sRemoveDiscussionMsg + "\""))
                    return;

                // Remove it
                (new RemoveDiscussionAction(wnd, tn)).Do();
                return;
            }

            // Give the user the opportunity to change his/her mind
            string sText = tn.Context;
            if (sText.Length > 40)
                sText = sText.Substring(0, 40) + "...";
            string sMsgAddition = "\n\n\"" + sText + "\"";
            if (false == Messages.ConfirmNoteDeletion(sMsgAddition))
                return;

            // Remove it
            (new DeleteNoteAction(wnd, tn)).Do();
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
                string sShowAllCategories = Loc.GetNotes("ShowAllCategories", "Show All Categories");
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
                    string sFrontCategories = Loc.GetNoteDefs("FrontCategories", "Notes From Front Translation");
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
            string sShowJustMe = "Show Notes Assigned to '" + DB.UserName + "'";
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
            if (!DB.Project.HasDataToDisplay)
                return;

            // Retrieve the notes that we will be showing, from both Front and Target
            List<TranslatorNote> vNotes = DB.TargetSection.GetAllShownTranslatorNotes();
            vNotes.AddRange(DB.FrontSection.GetAllShownTranslatorNotes());

            // Sort (the IComparer will do this by reference)
            vNotes.Sort();
            
            // Place them into the window
            foreach (TranslatorNote note in vNotes)
                Contents.Append( BuildView(note) );

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();

            // Make sure we're positioned at the top. For a longer set of data, we might
            // not be, because the very top of a note is not editable.
            ScrollBarPosition = 0;
        }
        #endregion

        // Save/Restore Editing State overrides ----------------------------------------------
        #region CLASS: NotesBookmark : OWBookmark
        public class NotesBookmark : OWBookmark
        {
            #region CLASS: NoteCollapseState
            class NoteCollapseState
            {
                #region Attr{g}: TranslatorNote Note
                public TranslatorNote Note
                {
                    get
                    {
                        return m_Note;
                    }
                }
                TranslatorNote m_Note;
                #endregion
                #region Attr{g}: bool IsCollapsed
                public bool IsCollapsed
                {
                    get
                    {
                        return m_bIsCollapsed;
                    }
                }
                bool m_bIsCollapsed;
                #endregion

                #region Contructor(TranslatorNote, bIsCollapsed)
                public NoteCollapseState(TranslatorNote note, bool bIsCollapsed)
                {
                    m_Note = note;
                    m_bIsCollapsed = bIsCollapsed;
                }
                #endregion
            }
            #endregion
            #region Attr{g}: List<NoteCollapseState> CollapsedStates
            List<NoteCollapseState> CollapsedStates
            {
                get
                {
                    Debug.Assert(null != m_vCollapsedStates);
                    return m_vCollapsedStates;
                }
            }
            List<NoteCollapseState> m_vCollapsedStates;
            #endregion

            #region Method: void RestoreCollapseStates()
            public void RestoreCollapseStates()
                // For restoring the collapsed states, without restoring the superclass
                // bookmark
            {
                foreach (EItem item in Window.Contents)
                {
                    // Get the collapsable container
                    ECollapsableHeaderColumn collapsable = item as ECollapsableHeaderColumn;
                    if (null == collapsable)
                        continue;

                    // Get its note
                    TranslatorNote note = NotesWnd.GetNoteFromContainer(collapsable);
                    if (null == note)
                        continue;

                    // Get the matching NoteCollapseState, and use it to set the
                    // collapse state of our display container
                    foreach (NoteCollapseState ncs in CollapsedStates)
                    {
                        if (ncs.Note == note)
                        {
                            collapsable.IsCollapsed = ncs.IsCollapsed;
                            break;
                        }
                    }
                }

                // Re-do the layout
                Window.DoLayout();
                Window.Invalidate();
            }
            #endregion

            #region OMethod: void RestoreWindowSelectionAndScrollPosition()
            public override void RestoreWindowSelectionAndScrollPosition()
            {
                // Restore the header collapse states
                RestoreCollapseStates();

                // Restore the rest of the Bookmark
                base.RestoreWindowSelectionAndScrollPosition();
            }
            #endregion

            #region Constructor(OWWindow)
            public NotesBookmark(OWWindow wnd)
                : base(wnd)
            {
                // Save the header collapse states
                m_vCollapsedStates = new List<NoteCollapseState>();
                foreach (EItem item in wnd.Contents)
                {
                    ECollapsableHeaderColumn collapsable = item as ECollapsableHeaderColumn;
                    if (null == collapsable)
                        continue;

                    TranslatorNote note = GetNoteFromContainer(collapsable);
                    if (null == note)
                        continue;

                    CollapsedStates.Add(new NoteCollapseState(note, collapsable.IsCollapsed));
                }
            }
            #endregion
        }
        #endregion
        #region OMethod: OWBookmark CreateBookmark()
        public override OWBookmark CreateBookmark()
        {
            return new NotesBookmark(this);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: OnChangeCategory - user has responded to the Category dropdown
        public void OnChangeCategory(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            TranslatorNote note = item.Tag as TranslatorNote;
            if (null == note)
                return;

            (new ChangeClassification(this,
               note, sender as ToolStripMenuItem, "Change Category to")).Do();
        }
        #endregion
        #region Cmd: OnChangeAssignedTo - user has responded to the AssignedTo dropdown
        public void OnChangeAssignedTo(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            TranslatorNote note = item.Tag as TranslatorNote;
            if (null == note)
                return;

            (new ChangeClassification(this,
                note, sender as ToolStripMenuItem, "Assign to")).Do();
        }
        #endregion
        #region Cmd: OnAddResponse
        public void OnAddResponse(object sender, EventArgs e)
        {
            ToolStripButton item = sender as ToolStripButton;
            if (null == item)
                return;

            TranslatorNote note = item.Tag as TranslatorNote;
            if (null == note)
                return;

            (new AddDiscussionAction(this, note)).Do();
        }
        #endregion

        // View Building ---------------------------------------------------------------------
        public const string c_sAssignedTo = "AssignedTo";
        public const string c_sCategory = "Category";

        #region SMethod: TranslatorNote GetNoteFromContainer(EContainer)
        static TranslatorNote GetNoteFromContainer(EContainer container)
            // The container can be any subcontainer in the view hierarchy
        {
            OWPara para = container as OWPara;
            if (null != para)
            {
                DParagraph pSource = para.DataSource as DParagraph;
                if (null != pSource)
                {
                    Discussion discussion = pSource.Owner as Discussion;
                    if (null != discussion)
                        return discussion.Note;
                }
            }

            foreach (EItem item in container.SubItems)
            {
                EContainer SubContainer = item as EContainer;
                if (null != SubContainer)
                {
                    TranslatorNote n = GetNoteFromContainer(SubContainer);
                    if (null != n)
                        return n;
                }
            }

            return null;
        }
        #endregion
        #region Method: ECollapsableHeaderColumn GetCollapsableFromNote(note)
        public ECollapsableHeaderColumn GetCollapsableFromNote(TranslatorNote note)
        {
            foreach (EContainer container in Contents)
            {
                var collapsable = container as ECollapsableHeaderColumn;
                if (null == collapsable)
                    continue;

                if (note == collapsable.Tag as TranslatorNote)
                    return collapsable;
            }
            return null;
        }
        #endregion

        public ToolStripDropDownButton GetDropDownButton(TranslatorNote note, string sWhich)
        {
            // Get the major container for this note
            var eContainer = GetCollapsableFromNote(note);
            if (null == eContainer)
                return null;

            // Loop through its subitems for the EToolStrip; then scan its items for the
            // target one.
            foreach (EItem item in eContainer.SubItems)
            {
                EToolStrip ts = item as EToolStrip;
                if (null == ts)
                    continue;

                foreach (ToolStripItem tsi in ts.ToolStrip.Items)
                {
                    if (tsi as ToolStripDropDownButton != null && tsi.Name == sWhich)
                        return tsi as ToolStripDropDownButton;
                }
            }

            return null;
        }

        #region Method: void BuildAddButton(TranslatorNote, ToolStrip)
        void BuildAddButton(TranslatorNote note, ToolStrip ts)
        {
            // Is this note editable?
            if (!note.IsEditable)
                return;

            // Are we allowed to add a discussion? No, if we have the same
            // author and the same date.
            if (note.LastDiscussion.Author == DB.UserName &&
                note.LastDiscussion.Created.Date == DateTime.Today)
                return;

            // If here, we can go ahead and create the Respond button
            string sButtonText = Loc.GetNotes("AddResponse", "Respond");
            ToolStripButton btnAddResponse = new ToolStripButton(sButtonText);
            btnAddResponse.Image = JWU.GetBitmap("Note_OldVersions.ico");
            btnAddResponse.Tag = note;
            btnAddResponse.Click += new EventHandler(OnAddResponse);
            ts.Items.Add(btnAddResponse);
        }
        #endregion
        #region Method: void BuildCategoryControl(note, ToolStrip)
        void BuildCategoryControl(TranslatorNote note, ToolStrip ts)
        {
            // If this is turned off, don't show anything
            if (!TranslatorNote.ShowCategories)
                return;

            // Determine if the category is changeable by this user
            // Default to "yes"
            bool bCategoryIsChangeable = true;
            // Can't change it if it is the front translation
            if (!note.IsOwnedInTargetTranslation)
                bCategoryIsChangeable = false;

            // If editable, then we buld a dropdown control
            if (bCategoryIsChangeable)
            {
                // We need a touch of space between controls
                ts.Items.Add(new ToolStripLabel("  "));

                ToolStripDropDownButton menuCategory = new ToolStripDropDownButton(note.Category);
                menuCategory.Name = c_sCategory;
                foreach (TranslatorNote.Classifications.Classification cat in TranslatorNote.Categories)
                {
                    if (cat.IsChecked)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(cat.Name);
                        item.Tag = note;
                        item.Click += new EventHandler(OnChangeCategory);
                        if (cat.Name == note.Category)
                        {
                            item.Checked = true;
                            menuCategory.Text = cat.Name;
                        }
                        menuCategory.DropDownItems.Add(item);
                    }
                }
                ts.Items.Add(menuCategory);
                return;
            }

            // Otherwise, we just display it
            ToolStripLabel labelCategory = new ToolStripLabel(note.Category);
            ts.Items.Add(labelCategory);
        }
        #endregion
        #region Method: void BuildAssignedToControl(note, ToolStrip)
        void BuildAssignedToControl(TranslatorNote note, ToolStrip ts)
        {
            // If this is turned off, don't show anything
            if (!TranslatorNote.ShowAssignedTo)
                return;

            // If this is a Front Translation note, we don't want to show anything.
            if (!note.IsOwnedInTargetTranslation)
                return;

            // We need a touch of space between controls
            ts.Items.Add(new ToolStripLabel("  "));

            // Place the AssignedTo as a drop-down button
            string sMenuName = Loc.GetNotes("kAssignTo", "Assign To");
            ToolStripDropDownButton menuAssignedTo = new ToolStripDropDownButton(sMenuName);
            menuAssignedTo.Name = c_sAssignedTo;
            foreach (TranslatorNote.Classifications.Classification cl in TranslatorNote.People)
            {
                // Don't let it be assigned to "Unknown Author"
                if (cl.Name == TranslatorNote.UnknownAuthor)
                    continue;

                // Create the menu item
                ToolStripMenuItem item = new ToolStripMenuItem(cl.Name);
                item.Tag = note;
                item.Click += new EventHandler(OnChangeAssignedTo);
                if (cl.Name == note.AssignedTo)
                {
                    item.Checked = true;
                    menuAssignedTo.Text = cl.Name;
                }
                menuAssignedTo.DropDownItems.Add(item);
            }
            ts.Items.Add(menuAssignedTo);
        }
        #endregion

        #region Method: EToolStrip BuildToolStrip(note)
        EToolStrip BuildToolStrip(TranslatorNote note)
        {
            // Create the EToolStrip
            EToolStrip toolstrip = new EToolStrip(this);
            ToolStrip ts = toolstrip.ToolStrip; // Shorthand

            // Add the "Add" button
            BuildAddButton(note, ts);

            // Add the Category Control
            BuildCategoryControl(note, ts);

            // Add the AssignedTo Control
            BuildAssignedToControl(note, ts);

            // If we didn't add anything, then dispose of it
            if (ts.Items.Count == 0)
            {
                ts.Dispose();
                return null;
            }

            return toolstrip;
        }
        #endregion
        #region Method: EContainer BuildView(note)
        public EContainer BuildView(TranslatorNote note)
            // We place the note in a collapsable container, so the user can hit the 
            // plus/minus icon to see the entire note or not.
        {
            // Create a header paragraph, to show when the note is collapsed
            DBasicText textHeader = note.GetCollapsableHeaderText("");
            OWPara pHeader = new OWPara(
                DB.TargetTranslation.WritingSystemVernacular,
                DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteHeader),
                textHeader.Phrases.AsVector);

            // Create the Collapsable Header Column to house the note
            ECollapsableHeaderColumn eHeader = new ECollapsableHeaderColumn(pHeader);
            eHeader.IsCollapsed = true;
            eHeader.ShowHeaderWhenExpanded = true;
            eHeader.Tag = note;

            // We want a horizontal line underneath it, to separate notes visually from 
            // each other
            eHeader.Border = new EContainer.SquareBorder(eHeader);
            eHeader.Border.BorderPlacement = EContainer.BorderBase.BorderSides.Bottom;
            eHeader.Border.BorderColor = Color.DarkGray;
            eHeader.Border.BorderWidth = 2;
            eHeader.Border.Margin.Bottom = 5;
            eHeader.Border.Padding.Bottom = 5;

            // Add the Discussions
            foreach (Discussion disc in note.Discussions)
                eHeader.Append(BuildView(disc));

            // Add the Category and AssignedTo, if turned on
            if (note.IsOwnedInTargetTranslation)
            {
                EToolStrip ts = BuildToolStrip(note);

                // We only add this if it turned out that there is something actually in it
                if (null != ts)
                    eHeader.Append(ts);
            }

            return eHeader;
        }
        #endregion

        #region Method: EContainer BuildView(Discussion)
        public EContainer BuildView(Discussion discussion)
        // We'll put this Discussion into a HeaderColumn. The header will be the
        // author and date, the body will be the paragraphs
        {
            discussion.Debug_VerifyIntegrity();

            int nRoundedCornerInset = 8;

            // We don't bother with showing the date for Notes from the Front Translation,
            // figuring that this is less relevant (and therefore clutter)
            bool bIsFromTargetTranslation = discussion.Note.IsOwnedInTargetTranslation;
            int cHeaderColumns = (bIsFromTargetTranslation) ? 2 : 1;

            // Define the Header. Insert the left/right margins so that they do not overlap
            // the rounded corners.
            ERowOfColumns eHeader = new ERowOfColumns(cHeaderColumns);
            eHeader.Border.Padding.Left = nRoundedCornerInset;
            eHeader.Border.Padding.Right = nRoundedCornerInset;

            OWPara pAuthor = new OWPara(
                DB.TargetTranslation.WritingSystemVernacular,
                DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteHeader),
                discussion.Author);
            eHeader.Append(pAuthor);

            // Define the author
            if (bIsFromTargetTranslation)
            {
                OWPara pDate = new OWPara(
                    DB.TargetTranslation.WritingSystemVernacular,
                    DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteDate),
                    discussion.Created.ToShortDateString());
                eHeader.Append(pDate);
            }

            // Create the main container and define its border
            EHeaderColumn eMainContainer = new EHeaderColumn(eHeader);
            eMainContainer.Border = new EContainer.RoundedBorder(eMainContainer, 12);
            eMainContainer.Border.BorderColor = TranslatorNote.BorderColor;
            eMainContainer.Border.FillColor = TranslatorNote.DiscussionHeaderColor;
            // The contents of the HeaderColumn are inset from the edges
            eMainContainer.Border.Padding.Left = 3;
            eMainContainer.Border.Padding.Right = 2;
            eMainContainer.Border.Padding.Bottom = 2;

            // Color depends on editability
            Color clrBackground = (discussion.IsEditable) ? Color.White : TranslatorNote.UneditableColor;

            // Create a Column to hold the discussion paragraphs. Insert the left/right
            // margins so nothing overlaps the rounded corners.
            EColumn eDiscussionHolder = new EColumn();
            eDiscussionHolder.Border = new EContainer.RoundedBorder(eDiscussionHolder, 12);
            eDiscussionHolder.Border.BorderColor = TranslatorNote.BorderColor;
            eDiscussionHolder.Border.FillColor = clrBackground;
            eDiscussionHolder.Border.Padding.Left = nRoundedCornerInset;
            eDiscussionHolder.Border.Padding.Right = nRoundedCornerInset;
            eMainContainer.Append(eDiscussionHolder);

            // Paragraph editing options
            OWPara.Flags options = OWPara.Flags.None;
            if (discussion.IsEditable)
                options |= (OWPara.Flags.IsEditable | OWPara.Flags.CanItalic);
            if (OurWordMain.Features.F_StructuralEditing)
                options |= OWPara.Flags.CanRestructureParagraphs;

            // Add the paragraphs to the Discussion Holder
            foreach (DParagraph para in discussion.Paragraphs)
            {
                // Create the OWPara and add it to its container
                OWPara pDiscussion = new OWPara(
                    DB.TargetTranslation.WritingSystemVernacular,
                    DB.StyleSheet.FindParagraphStyle(para.StyleAbbrev),
                    para,
                    clrBackground,
                    options
                    );
                if (!discussion.IsEditable)
                    pDiscussion.NonEditableBackgroundColor = clrBackground;
                eDiscussionHolder.Append(pDiscussion);
            }

            return eMainContainer;
        }
        #endregion
    }

}
