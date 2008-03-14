/**********************************************************************************************
 * Project: OurWord!
 * File:    NotesPane.cs
 * Author:  John Wimbish
 * Created: 26 Feb 2008
 * Purpose: Internals of the Tab Page for displaying Notes
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
        #region VAttr{g}: NotesWnd WndNotes
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
            string sNoteType = (string)((sender as ToolStripItem).Tag);

            for (DNote.Types k = 0; k != DNote.Types.kUnknown; k++)
            {
                if (k.ToString() == sNoteType)
                {
                    _InsertNote(k);
                    return;
                }
            }
        }
        private void _InsertNote(DNote.Types type)
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
            if (null == text)
                return;

            // Remember the location
            OWBookmark bookmark = new OWBookmark(G.App.MainWindow.Selection);

            // Insert the note
            DNote note = text.InsertNote(type, G.App.MainWindow.Selection.SelectionString);
            if (null != note)
            {
                // Update the display
                G.App.ResetWindowContents();

                // Return the Main Window to where it was
                bookmark.RestoreWindowSelectionAndScrollPosition();

                // Select the new note and bring the focus to it
                WndNotes.SelectEndOfNote(note);
                WndNotes.Focus();
            }
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

            // Request the targeted DNote from the NotesWnd
            Debug.Assert(null != WndNotes);
            DNote note = WndNotes.GetSelectedNote();
            if (null == note)
                return;

            // Give the user the opportunity to change his/her mind
            string sText = note.NoteText.ContentsAsString;
            if (sText.Length > 40)
                sText = sText.Substring(0, 40) + "...";
            string sMsgAddition = "\n\n\"" + sText + "\"";
            if (false == Messages.ConfirmNoteDeletion(sMsgAddition))
                return;

            // Remove the note from the paragraph (we delete it from the
            // owning DText)
            DText text = note.Text;
            Debug.Assert(null != text);
            text.Notes.Remove(note);

            // Regenerate the windows
            G.App.ResetWindowContents();
        }
        #endregion
        #region Cmd: cmdLoad - Localize the toolstrip
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(m_toolstripNotes);
        }
        #endregion

        // Visibility and Enabling -----------------------------------------------------------
        #region Method: void SetControlsVisibility()
        public void SetControlsVisibility()
            // Visibility is based mostly on user settings, as stored in DNote; but o
            // occasionally on context (e.g., back translation)
        {
            m_menuInsertGeneral.Visible = DNote.ShowGeneral;
            m_menuInsertToDo.Visible = DNote.ShowToDo;
            m_menuInsertAskUNS.Visible = DNote.ShowAskUns;
            m_menuInsertDefinition.Visible = DNote.ShowDefinition;
            m_menuInsertOldVersion.Visible = DNote.ShowOldVersion;
            m_menuInsertReason.Visible = DNote.ShowReason;
            m_menuInsertFrontIssue.Visible = DNote.ShowFront;
            m_menuInsertHintForDaughter.Visible = DNote.ShowHintForDaughter;

            m_menuInsertBT.Visible = DNote.ShowBT &&
                OurWordMain.App.MainWindowIsBackTranslation;
        }
        #endregion
        #region Method: void SetControlsEnabling()
        public void SetControlsEnabling()
            // The idea is that we insert notes when we are in the main window, but
            // not when we are in the notes pane. And we can only delete notes if
            // we are in the notes pane.
        {
            // Insert Note menu items
            bool bCanInsertNote = canInsertNote;
            m_btnInsert.Enabled = bCanInsertNote;
            m_menuInsertGeneral.Enabled = bCanInsertNote;
            m_menuInsertToDo.Enabled = bCanInsertNote;
            m_menuInsertAskUNS.Enabled = bCanInsertNote;
            m_menuInsertDefinition.Enabled = bCanInsertNote;
            m_menuInsertOldVersion.Enabled = bCanInsertNote;
            m_menuInsertReason.Enabled = bCanInsertNote;
            m_menuInsertFrontIssue.Enabled = bCanInsertNote;
            m_menuInsertHintForDaughter.Enabled = bCanInsertNote;
            m_menuInsertBT.Enabled = bCanInsertNote;

            // Delete Note
            m_btnDeleteNote.Enabled = canDeleteNote;
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

        // Secondary Window Messaging --------------------------------------------------------
        #region Method: override void OnAddNote(DNote note, bool bIsEditable)
        protected override void OnAddNote(DNote note, bool bIsEditable)
        {
            // Determine the note's writing system
            JWritingSystem ws = note.Paragraph.Translation.WritingSystemConsultant;

            // Determine the note's background
            Color clrEditableBackground = note.NoteDef.BackgroundColor;

            // Determine the note's editability
            OWPara.Flags options = (bIsEditable) ? OWPara.Flags.IsEditable : OWPara.Flags.None;

            // Determine the note's style
            JParagraphStyle PStyle = G.StyleSheet.FindParagraphStyle(DNote.StyleAbbrev);

            // Create a OWParagraph for the note
            OWPara p = new OWPara(this, ws, PStyle, note, clrEditableBackground, options);

            // Add it to the view
            StartNewRow();
            AddParagraph(0, p);
        }
        #endregion
        #region Method: override void OnSelectAndScrollToNote(DNote note)
        public override void OnSelectAndScrollToNote(DNote note)
        {
            foreach (Row r in Rows)
            {
                // Retrieve the one and only pile in this row
                if (r.Piles.Length == 0)
                    continue;
                Row.Pile pile = r.Piles[0];

                // Examine all of the OWPara's in this pile (should just be one)
                foreach (OWPara para in pile.Paragraphs)
                {
                    // If we find the paragraph which represents the note, attempt
                    // to select with it (if we can, then focus this window); at
                    // any rate, we're done.
                    if (para.DataSource == note)
                    {
                        OWWindow.Sel selection = para.Select_BeginningOfFirstWord();
                        if (null != selection)
                        {
                            Selection = selection;
                            Focus();
                        }
                        return;
                    }
                }
            }
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: DNote GetSelectedNote()
        public DNote GetSelectedNote()
        {
            if (Selection == null)
                return null;

            DNote note = Selection.Paragraph.DataSource as DNote;
            return note;
        }
        #endregion
        #region Method: void SelectEndOfNote(DNote note)
        public void SelectEndOfNote(DNote note)
        {
            // Drill down to find the OWPara that corresponds to the note
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara para in pile.Paragraphs)
                    {
                        if (para.DataSource == note)
                        {
                            // Attempt to make a selection at the end of this note
                            OWWindow.Sel selection = para.Select_EndOfLastWord();

                            // If selection made, set the Window's selection to it
                            if (null != selection)
                                Selection = selection;

                            // Either way, we're done here
                            return;
                        }
                    }
                }
            }
        }
        #endregion
    }

}
