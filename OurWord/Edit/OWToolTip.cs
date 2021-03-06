﻿#region ***** OWToolTip.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    OWToolTip.cs
 * Author:  John Wimbish
 * Created: 02 Sep 2009
 * Purpose: Tooltip window for EBlocks within an OWWindow
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OurWord.Edit.Blocks;
using OurWord.ToolTips;
using OurWord.Utilities;
using OurWordData;
using OurWordData.DataModel;
using JWTools;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Membership;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWord.Edit
{
    #region CODE FROM ToolTip ContentWindow, when we automatically adjusted Height
    /* CODE FROM ToolTip ContentWindow, when we automatically adjusted Height
         * 
        // Layout and Dynamic Window Height --------------------------------------------------
        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // Have the block load the boxes. We test for a null Block, because a mouse move
            // over the main window can cause an asynchronus call to ClearBlock, before this
            // method here is entered, e.g., if the user changed the Status and then moved the
            // mouse before the ChangeStatus.SetStatus method can call us here. (Took a while
            // to track this one down!) 
            var block = m_Tip.Block;
            if (null != block)
                block.LoadToolTip(this);

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
        #region OMethod: void DoLayout()
        public override void DoLayout()
            // Once a layout is done, we want to adjust the height of our window so that
            // it holds all of the contents (and thus we don't need a scrollbar. 
        {
            // Let the window calculate its height first
            base.DoLayout();

            // Then adjust our height to match it
            m_Tip.Height = (int)Contents.Height +
                (int)WindowMargins.Height * 2;

            // Create a new Drawbuffer to reflect the changed height of the window. Normally
            // we would call SetSize, but this requires another call to DoLayout; thus this
            // situation where the layout drives the window size causes us to do things
            // like this.
            Draw = new ScreenDraw(this);
        }
        #endregion
        #region Cmd: OnSizeChanged
        protected override void OnSizeChanged(EventArgs e)
            // Override OWWindow, where a change in the window size triggers a call to DoLayout.
            // Here, we want the reverse, where a call to DoLayout triggers a size change. So
            // if we didn't override with a do-nothing method, we'd be in an endless loop.
        {
        }
        #endregion
        */
    #endregion

    #region CLASS: AnnotationTipBuilder
    public class AnnotationTipBuilder
    {
        protected readonly OWWindow Tip;
        protected readonly TranslatorNote Note;

        #region Constructor(tip, note)
        public AnnotationTipBuilder(OWWindow tip, TranslatorNote note)
        {
            Tip = tip;
            Debug.Assert(null != Tip);

            Note = note;
            Debug.Assert(null != Note);

            Tip.Contents.Clear();
        }
        #endregion

        // View building ---------------------------------------------------------------------
        #region void LoadNoteTitle(sIconResource)
        public void LoadNoteTitle(string sIconResource)
        {
            Debug.Assert(null != Note);

            // The E classes require a text object for this to go into
            var dbt = new DBasicText();

            // Title if supplied (General annotations don't add to the title)
            if (Note.Behavior != TranslatorNote.General && 
                !string.IsNullOrEmpty(Note.Behavior.Title))
            {
                var pNoteTitle = new DPhrase(Note.Behavior.Title) { FontToggles = FontStyle.Bold };
                dbt.Phrases.Append(pNoteTitle);
            }

            // Add the reference, in italics
            var sBookRef = Note.GetDisplayableReference();
            if (!string.IsNullOrEmpty(sBookRef))
            {
                if (dbt.Phrases.Count > 0)
                {
                    var sText = DPhrase.c_chInsertionSpace + "-" + DPhrase.c_chInsertionSpace;
                    var phrase = new DPhrase(sText) { FontToggles = FontStyle.Bold };
                    dbt.Phrases.Append(phrase);
                }

                var pRef = new DPhrase(sBookRef + ":" + DPhrase.c_chInsertionSpace) 
                    { FontToggles = FontStyle.Italic };
                dbt.Phrases.Append(pRef);
            }

            // Add a truncated and quote-surrounded version of the selected text
            var sSelectedText = Note.SelectedText;
            if (!string.IsNullOrEmpty(sSelectedText))
            {
                if (sSelectedText.Length > 40)
                    sSelectedText = sSelectedText.Substring(0, 40) + "...";
                sSelectedText = "\"" + sSelectedText + "\"";
                var pSelectedText = new DPhrase(sSelectedText);
                dbt.Phrases.Append(pSelectedText);
            }

            // Create the paragraph
            var pTitle = new OWPara(
                Note.Behavior.GetWritingSystem(Note),
                StyleSheet.TipHeader,
                dbt.Phrases.AsVector);

            // Pre-pend the icon
            if (!string.IsNullOrEmpty(sIconResource))
                pTitle.InsertAt(0, new EIcon(sIconResource));

            // Load into the window
            Tip.Contents.Append(pTitle);
        }
        #endregion

        #region virtual void LoadInteractiveMessages()
        public virtual void LoadInteractiveMessages()
        {
            // We'll place the meessages in their own container, so we can have a 
            // bottom border separating them from the next message.
            var messagesBox = new EColumn();
            messagesBox.Border = new EContainer.SquareBorder(messagesBox)
            {
                BorderPlacement = EContainer.BorderBase.BorderSides.Bottom,
                Padding = {Bottom = 5},
                Margin = {Top = 5}
            };
            Tip.Contents.Append(messagesBox);

            // Add each message to the container
            foreach (DMessage message in Note.Messages)
            {
                // Author and Date
                var messageTitle = BuildMessageTitle(message);
                messagesBox.Append(messageTitle);

                var messageContents = BuildMessageContents(message);
                messagesBox.Append(messageContents);
            }
        }
        #endregion
        #region EItem BuildMessageContents(message)
        protected EItem BuildMessageContents(DMessage message)
        {
            Debug.Assert(null != Note);
            var writingSystem = Note.Behavior.GetWritingSystem(Note);

            // Background color depends on editibility
            var clrBackground = (message.IsEditable) ? Color.White : Color.Cornsilk;

            // Flags depend on editibility
            var flags = (message.IsEditable) ?
                (OWPara.Flags.IsEditable | OWPara.Flags.CanItalic) :
                (OWPara.Flags.None);

            // Create the paragraph
            var style = (message.IsEditable) ? StyleSheet.TipContent : StyleSheet.TipText;
            var p = new OWPara(writingSystem, style, message, clrBackground, flags);

            // If the message is editable, we want to make it stand out by placing it inside
            // a container that shows the color better.
            if (!message.IsEditable)
                return p;

            var eEdit = new EColumn();
            eEdit.Border = new EContainer.RoundedBorder(eEdit, 8);
            eEdit.Border.Padding.Top = 3;
            eEdit.Border.Padding.Bottom = 3;
            eEdit.Border.BorderColor = Color.Navy;
            eEdit.Border.FillColor = Color.White;
            eEdit.Border.Margin.Top = 4; // get out of the way of the toolbar
            eEdit.Append(p);
            return eEdit;

        }
        #endregion

        #region void LoadSingleMessageContents()
        public void LoadSingleMessageContents()
        {
            // Only interested in the first message
            var message = Note.FirstMessage;

            // Background color depends on editibility
            var clrBackground = (message.IsEditable) ? Color.White : Color.Cornsilk;

            // Flags depend on editibility
            var flags = (message.IsEditable) ?
                (OWPara.Flags.IsEditable | OWPara.Flags.CanItalic) :
                (OWPara.Flags.None);

            // Create the paragraph
            var writingSystem = Note.Behavior.GetWritingSystem(Note);
            Debug.Assert(null != writingSystem);
            var p = new OWPara(writingSystem, StyleSheet.TipText, message, clrBackground,
                flags);

            // If the message is editable, we want to make it stand out by placing it inside
            // a container that shows the color better.
            if (!message.IsEditable)
            {
                Tip.Contents.Append(p);
                return;
            }
                
            var eEdit = new EColumn();
            eEdit.Border = new EContainer.RoundedBorder(eEdit, 8);
            eEdit.Border.Padding.Top = 3;
            eEdit.Border.Padding.Bottom = 3;
            eEdit.Border.BorderColor = Color.Navy;
            eEdit.Border.FillColor = Color.White;
            eEdit.Append(p);
            Tip.Contents.Append(eEdit);
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region void MoveCursorIntoTipArea()
        void MoveCursorIntoTipArea()
        {
            Debug.Assert(null != Tip);

            var yWndBottom = Tip.PointToScreen(new Point(Tip.Left, Tip.Bottom)).Y;
            if (Cursor.Position.Y > yWndBottom)
                Cursor.Position = new Point(Cursor.Position.X, yWndBottom - 3);
        }
        #endregion

        // Annotation Toolstrip and Handlers -------------------------------------------------
        #region virtual void LoadToolStrip()
        public virtual void LoadToolStrip()
        {
            // Create the EToolStrip
            var boxToolstrip = new EToolStrip(Tip);
            var ctrlToolstrip = boxToolstrip.ToolStrip; // Shorthand
            Tip.Contents.Append(boxToolstrip);

            // Respond button
            var buttonRespond = BuildRespondControl();
            if (null != buttonRespond)
            {
                // Add the button
                ctrlToolstrip.Items.Add(buttonRespond);

                // Add space between the next control
                ctrlToolstrip.Items.Add(new ToolStripLabel("  "));
            }

            // Add the Assigned Status Control
            ctrlToolstrip.Items.Add(BuildAssignStatusControl());

            // Add space between the next control
            ctrlToolstrip.Items.Add(new ToolStripLabel("  "));

            // Add the Delete Annotation control
            ctrlToolstrip.Items.Add(BuildDeleteNoteControl());
        }
        #endregion

        // Response control
        #region OnRespond
        private void OnRespond(object sender, EventArgs e)
        {
            var button = sender as ToolStripButton;
            if (null == button)
                return;

            var note = button.Tag as TranslatorNote;
            if (null == note)
                return;

//            (new AddMessageAction(Tip, note)).Do();
        }
        #endregion
        #region ToolStripButton BuildRespondControl()
        ToolStripButton BuildRespondControl()
        {
            // Create the button
            var sButtonText = Loc.GetNotes("AddResponse", "Respond");
            var btnAddResponse = new ToolStripButton(sButtonText)
            {
                Tag = Note,
                Image = JWU.GetBitmap("Note_OldVersions.ico")
            };

            // Command handler
            btnAddResponse.Click += new EventHandler(OnRespond);

            // Normal tooltip
            btnAddResponse.ToolTipText = Loc.GetNotes("AddResponse_tip",
                "Add your response to this note.");

            // If a user has already entered a message today, they just edit it directly,
            // rather than adding a new one. So we disable the button, but leave it there
            // so that the user isn't confused by a changing toolstrip.
            if (Note.LastMessage.Author == Users.Current.NoteAuthorsName &&
                Note.LastMessage.UtcCreated.Date == DateTime.Today)
            {
                // Disable the button
                btnAddResponse.Enabled = false;

                // Tooltip tells the user why
                btnAddResponse.ToolTipText = Loc.GetNotes("AddResponse_tipDisabled",
                    "Add your response to this note.\n" +
                    "(Disabled if yours is already the most recent response\n" +
                    "today; just add your additional thoughts to it.)");
            }

            return btnAddResponse;
        }
        #endregion

        // Status control
        #region OnAssignStatus
        private void OnAssignStatus(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            // Move the mouse back into the tooltip window so the window will
            // not get dismissed when the mouse next moves.
            var yWndBottom = Tip.PointToScreen(new Point(Tip.Left, Tip.Bottom)).Y;
            if (Cursor.Position.Y > yWndBottom)
                Cursor.Position = new Point(Cursor.Position.X, yWndBottom - 3);

            // Make the change
//            (new ChangeStatus(Tip, Note, item)).Do();
        }
        #endregion
        #region static void SetStatusToolTip(TranslatorNote, ToolStripDropDownButton)
        static public void SetStatusToolTip(TranslatorNote note, ToolStripDropDownButton item)
        {
            item.ToolTipText = note.Status.LocalizedToolTipText;
        }
        #endregion
        #region var BuildAssignStatusItem(Role)
        ToolStripMenuItem BuildAssignStatusItem(Role role)
        {
            var item = new ToolStripMenuItem(role.LocalizedName);
            item.Click += new EventHandler(OnAssignStatus);

            if (role == Note.Status)
                item.Checked = true;

            return item;
        }
        #endregion
        #region var BuildAssignStatusControl()
        ToolStripDropDownButton BuildAssignStatusControl()
        {
            // Create the dropdown button
            var btnStatus = new ToolStripDropDownButton();
            btnStatus.Name = "Status";
            SetStatusToolTip(Note, btnStatus);

            foreach(var role in Role.AllRoles)
            {
                var item = BuildAssignStatusItem(role);
                btnStatus.DropDownItems.Add(item);
            }

            // The text on the button is the Annotation's status
            btnStatus.Text = Note.Status.LocalizedName;

            return btnStatus;
        }
        #endregion

        // Delete control
        #region OnDeleteNote
        void OnDeleteNote(object sender, EventArgs e)
        {
            // Get the user interface item
            var button = sender as ToolStripItem;
            Debug.Assert(null != button);

            // Get the annotation
            var note = button.Tag as TranslatorNote;
            Debug.Assert(null != note);

            // Give the user the opportunity to change his/her mind
            var sText = note.SelectedText;
            if (sText.Length > 40)
                sText = sText.Substring(0, 40) + "...";
            var sMsgAddition = "\n\n\"" + sText + "\"";
            if (false == Messages.ConfirmNoteDeletion(sMsgAddition))
                return;

            // Close the ToolTip window
//            OWToolTip.ToolTip.CloseWindow();

            // Remove the note
            (new DeleteNoteAction(Tip, note)).Do();
        }
        #endregion
        #region OnDeleteMessage
        void OnDeleteMessage(object sender, EventArgs e)
        {
            // Get the user interface item
            var button = sender as ToolStripItem;
            Debug.Assert(null != button);

            // Get the target message
            var message = button.Tag as DMessage;
            Debug.Assert(null != message);

            // Give the user the opportunity to change his/her mind
            var sText = message.Author + ", " + message.LocalTimeCreated.ToShortDateString();
            var bProceed = LocDB.Message(
                "msgConfirmMessageDeletion",
                "Are you sure you want to delete the message:\n  {0}?",
                new[] { sText },
                LocDB.MessageTypes.YN);
            if (!bProceed)
                return;

            // Remove the message
 //           (new RemoveMessageAction(Tip, message)).Do();
        }
        #endregion
        #region ToolStripButton BuildDeleteSimpleButton(JObject objWhatToDelete)
        ToolStripButton BuildDeleteSimpleButton(JObject objWhatToDelete)
        {
            var sButtonText = Loc.GetNotes("DeleteNote", "Delete...");
            var btn = new ToolStripButton(sButtonText);
            btn.Image = JWU.GetBitmap("Delete.ico");

            // Event handler depends on the object passed in
            if (null != objWhatToDelete as DMessage)
                btn.Click += new EventHandler(OnDeleteMessage);
            else if (null != objWhatToDelete as TranslatorNote)
                btn.Click += new EventHandler(OnDeleteNote);

            // Set the Tag to the object we want to delete; if null, then disable the button
            btn.Tag = objWhatToDelete;
            if (null == objWhatToDelete)
                btn.Enabled = false;

            // Tooltip
            btn.ToolTipText = Loc.GetNotes("DeleteNote_tip",
                "Delete this note or message.\n" +
                "(Disabled if there are messages in this note that you did not author.)");

            return btn;
        }
        #endregion
        #region ToolStripDropDownButton BuildDeleteDropDownButton()
        ToolStripDropDownButton BuildDeleteDropDownButton()
        {
            var sButtonText = Loc.GetNotes("DeleteNote", "Delete...");
            var btn = new ToolStripDropDownButton(sButtonText);
            btn.Image = JWU.GetBitmap("Delete.ico");
            btn.ToolTipText = Loc.GetNotes("DeleteNoteWithPriviledges_tip",
                "Delete this note or any of its messages.");

            var bDeleteNote = new ToolStripMenuItem(
                Loc.GetNotes("DeleteEntireNote", "Entire Note..."),
                null,
                new EventHandler(OnDeleteNote));
            bDeleteNote.Tag = Note;
            btn.DropDownItems.Add(bDeleteNote);

            btn.DropDownItems.Add(new ToolStripSeparator());

            foreach (DMessage message in Note.Messages)
            {
                string s = message.Author + ", " + message.LocalTimeCreated.ToShortDateString();

                if (message.SimpleText.Length > 20)
                    s += "    (" + message.SimpleText.Substring(0, 20) + "...)";
                else
                    s += "    (" + message.SimpleText + ")";


                var b = new ToolStripMenuItem(s, null, new EventHandler(OnDeleteMessage));
                b.Tag = message;
                btn.DropDownItems.Add(b);
            }

            return btn;
        }
        #endregion
        #region Method: ToolStripButton BuildDeleteNoteControl()
        protected ToolStripItem BuildDeleteNoteControl()
        {
            // If we do not have global delete priveledges, our options are limited.
            if (!Users.Current.CanDeleteNotesAuthoredByOthers)
            {
                // If there is one message and we authored it, we can delete the annotation
                if (Note.Messages.Count == 1 && Users.Current.NoteAuthorsName == Note.LastMessage.Author)
                    return BuildDeleteSimpleButton(Note);

                // If there are more than one message, but we authored the last one, then
                // we can delete that message
                if (Note.Messages.Count > 1 && Users.Current.NoteAuthorsName == Note.LastMessage.Author)
                    return BuildDeleteSimpleButton(Note.LastMessage);

                // Otherwise, the button is disabled.
                return BuildDeleteSimpleButton(null);
            }

            // If we do have global priviledges, then we can delete anything we please
            if (Note.Messages.Count == 1)
                return BuildDeleteSimpleButton(Note);

            // If we are here, we have multiple messages, so we go to a dropdown button listing
            // all of the options
            return BuildDeleteDropDownButton();
        }
        #endregion

        // Message Title and Handlers --------------------------------------------------------
        #region EItem BuildMessageTitle(message)
        EItem BuildMessageTitle(DMessage message)
        {
            var writingSystem = Note.Behavior.GetWritingSystem(Note);
            Debug.Assert(null != writingSystem);

            // Header style and font
            var fontHeader = StyleSheet.TipHeader.GetFont(writingSystem.Name,
                Users.Current.ZoomPercent);

            // Uneditable messages are just a line of text
            if (!message.IsEditable)
            {
                var p = new OWPara(writingSystem,
                    StyleSheet.TipHeader,
                    new[] { 
                        new DPhrase(message.Author),
                        new DPhrase(", "),
                        new DPhrase(message.LocalTimeCreated.ToShortDateString())
                    });
                return p;
            }

            // For an editable message, we want to permit the user to change the author, thus
            // we need a toolstrip
            var eToolstrip = new EToolStrip(Tip);
            var ts = eToolstrip.ToolStrip;

            // The first item is a combo box, whose dropdown items are the team members, and 
            // which allows names to be typed into the text area
            var combo = new ToolStripComboBox("author");
            ts.Items.Add(combo);
            combo.Text = message.Author;
            combo.Tag = Note;
            foreach (string sPerson in DB.Project.People)
                combo.Items.Add(sPerson);
            if (!DB.Project.People.Contains(Users.Current.NoteAuthorsName))
                combo.Items.Add(Users.Current.NoteAuthorsName);
            combo.DropDownClosed += new EventHandler(OnAuthorDropDownClosed);
            combo.TextChanged += new EventHandler(OnAuthorTextChanged);

            // Add space between the next control
            ts.Items.Add(new ToolStripLabel("  "));

            // Add the date
            var label = new ToolStripLabel(message.LocalTimeCreated.ToShortDateString()) 
                {Font = fontHeader};
            ts.Items.Add(label);

            return eToolstrip;
        }
        #endregion
        #region OnAuthorDropDownClosed
        private void OnAuthorDropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ToolStripComboBox;
            if (null == comboBox)
                return;

            var note = comboBox.Tag as TranslatorNote;
            if (null == note)
                return;

            // Move the mouse back into the tooltip window so the window will
            // not get dismissed when the mouse next moves.
            MoveCursorIntoTipArea();

            // The original author is the combo's current text
            string sOriginalAuthor = comboBox.Text;

            // The new author is the value in the dropdown
            var sNewAuthor = (string)comboBox.SelectedItem;

            // Make the change
            (new ChangeAuthor(G.App.CurrentLayout, note, comboBox, sNewAuthor, sOriginalAuthor)).Do();
        }
        #endregion
        #region OnAuthorTextChanged
        private static void OnAuthorTextChanged(object sender, EventArgs e)
        {
            var combo = sender as ToolStripComboBox;
            if (null == combo)
                return;

            var note = combo.Tag as TranslatorNote;
            if (null == note)
                return;

            // The Original Author is who we've stored as the Annotation's Author
            var sOriginalAuthor = Users.Current.NoteAuthorsName;

            // The New Author is now in the combo text
            var sNewAuthor = combo.Text;

            // If the new author would be empty, we don't make the change
            if (string.IsNullOrEmpty(sNewAuthor))
                return;

            // Make the change
            (new ChangeAuthor(G.App.CurrentLayout, note, combo, sNewAuthor, sOriginalAuthor)).Do();
        }
        #endregion
    }
    #endregion

    #region CLASS: HistoryBuilder
    public class HistoryBuilder : AnnotationTipBuilder
        // TODO: Can we inherit the Delete control?
    {
        #region Constructor(tip, note)
        public HistoryBuilder(OWWindow tip, TranslatorNote history)
            : base(tip, history)
        {
        }
        #endregion

        // View building ---------------------------------------------------------------------
        #region override void LoadInteractiveMessages()
        public override void LoadInteractiveMessages()
        {
            foreach (DEventMessage message in Note.Messages)
            {
                // Container for the message
                var boxMessage = BuildMessageContainer(message);
                Tip.Contents.Append(boxMessage);

                // Title
                boxMessage.Append(BuildMessageTitle(message));

                // Contents
                boxMessage.Append(BuildMessageContents(message));
            }
        }
        #endregion

        #region EContainer BuildMessageContainer(DMessage)
        EContainer BuildMessageContainer(DMessage message)
        {
            var boxMessage = new EColumn();
            boxMessage.Border = new EContainer.SquareBorder(boxMessage);
            if (message == Note.FirstMessage)
            {
                boxMessage.Border.BorderPlacement = EContainer.BorderBase.BorderSides.TopAndBottom;
                boxMessage.Border.Margin.Top = 5;
            }
            else
            {
                boxMessage.Border.BorderPlacement = EContainer.BorderBase.BorderSides.Bottom;
            }
            return boxMessage;
        }
        #endregion

        // Annotation Toolstrip and Handlers -------------------------------------------------
        #region override void LoadToolStrip()
        public override void LoadToolStrip()
        {
            // Create the EToolStrip
            var boxToolstrip = new EToolStrip(Tip);
            var ctrlToolstrip = boxToolstrip.ToolStrip; // Shorthand
            Tip.Contents.Append(boxToolstrip);

            // Add control
            ctrlToolstrip.Items.Add(BuildAppendEventControl());
            ctrlToolstrip.Items.Add(new ToolStripLabel("  "));

            // Delete control
            ctrlToolstrip.Items.Add(BuildDeleteEventButton());
        }
        #endregion

        // AddEvent control
        #region Attr{g}: ToolStripButton AddEventButton
        public ToolStripButton AddEventButton
        {
            get
            {
                Debug.Assert(null != m_btnAddEvent);
                return m_btnAddEvent;
            }
        }
        private ToolStripButton m_btnAddEvent;
        #endregion
        #region OnAppendEvent
        private void OnAppendEvent(object sender, EventArgs e)
        // Since implemented here, not undoable
        {
            var button = sender as ToolStripButton;
            if (null == button)
                return;

            var history = button.Tag as TranslatorNote;
            if (null == history || history.Behavior != TranslatorNote.History)
                return;

            // The stage of the new event will be what we used last time
            Stage stage = (history.HasMessages) ?
                (history.LastMessage as DEventMessage).Stage :
                DB.TeamSettings.Stages.Draft;

            // Create the Event, and thus remember it here, so that Undo/Redo will work.
            history.AddMessage(DateTime.UtcNow, stage, "");
            var bookmark = Tip.CreateBookmark();
            // Reset the bookmark's flags to none, because the former last message will no
            // no longer be editable; and restoring the bookmark will not otherwise work
            // because it seeks a paragraph with the same flags as when the bookmark
            // was originally set.
            bookmark.ParagraphFlags = OWPara.Flags.None;
            Tip.LoadData();
            bookmark.RestoreWindowSelectionAndScrollPosition();
            Tip.Contents.Select_LastWord_End();
        }
        #endregion
        #region var BuildAppendEventControl()
        private ToolStripButton BuildAppendEventControl()
        {
            // Create the button
            var sButtonText = Loc.GetNotes("AddEvent", "Add New");
            m_btnAddEvent = new ToolStripButton(sButtonText) 
            {
                Tag = Note, 
                Image = JWU.GetBitmap("Note_OldVersions.ico")
            };

            // Command handler
            m_btnAddEvent.Click += OnAppendEvent;

            // Normal tooltip
            m_btnAddEvent.ToolTipText = Loc.GetNotes("AddEvent_tip",
                "Add another event to this history.");

            // Disable is last message is blank
            if (Note.HasMessages && string.IsNullOrEmpty(Note.LastMessage.SimpleText))
                m_btnAddEvent.Enabled = false;

            return m_btnAddEvent;
        }
        #endregion

        // Delete control
        #region Cmd: OnDeleteEvent
        private void OnDeleteEvent(object sender, EventArgs e)
            // Since implemented here, not undoable
        {
            var button = sender as ToolStripButton;
            if (null == button)
                return;

            var history = button.Tag as TranslatorNote;
            if (null == history || history.Behavior != TranslatorNote.History)
                return;

            // Can't delete the one remaining event
            if (history.Messages.Count < 2)
                return;
            var message = history.LastMessage as DEventMessage;
            Debug.Assert(null != message);

            // Confirm
            var sContents = message.EventDate.ToShortDateString() + " - " +
                message.Stage + " - " + message.SimpleText.Trim();
            if (sContents.Length > 60)
                sContents = sContents.Substring(0, 60) + "...";
            if (!LocDB.Message("kDeleteEvent",
                "Are you sure you want to delete:\n\n\"{0}\"?",
                new[] { sContents },
                LocDB.MessageTypes.YN))
            {
                return;
            }

            // Remove it
            history.RemoveMessage(history.LastMessage);
            Tip.LoadData();
        }
        #endregion
        #region ToolStripButton BuildDeleteEventButton()
        private ToolStripButton BuildDeleteEventButton()
        {
            var sButtonText = Loc.GetNotes("DeleteEvent", "Delete...");

            var button = new ToolStripButton(sButtonText)
            {
                Image = JWU.GetBitmap("Delete.ico"), 
                Tag = Note
            };
            button.Click += OnDeleteEvent;

            // Disable if only one message
            if (Note.Messages.Count < 2)
                button.Enabled = false;

            // Tooltip
            button.ToolTipText = Loc.GetNotes("DeleteEvent_tip",
                "Delete the most recent event.");

            return button;
        }
        #endregion

        // Message Title and Handlers --------------------------------------------------------
        #region Method: EItem BuildMessageTitle(DEventMessage)
        EItem BuildMessageTitle(DEventMessage message)
        {
            if (message.IsEditable)
            {
                var boxToolstrip = new EToolStrip(Tip);
                boxToolstrip.ToolStrip.Items.Add(BuildDatePicker(message));
                boxToolstrip.ToolStrip.Items.Add(new ToolStripLabel("   "));
                boxToolstrip.ToolStrip.Items.Add(BuildStageDropdown(message));
                return boxToolstrip;
            }

            var sStage = (null == message.Stage) ? "" : message.Stage.LocalizedAbbrev;

            var pTitle = new OWPara(
                Note.Behavior.GetWritingSystem(Note),
                StyleSheet.TipHeader,
                new[] { 
                    new DPhrase( message.EventDate.ToShortDateString()),
                    new DPhrase( ", "),
                    new DPhrase( sStage)
                });
            return pTitle;
        }
        #endregion

        #region ToolStripItem BuildDatePicker(Event)
        static ToolStripItem BuildDatePicker(DEventMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            // Create a date-time picker
            var ctrl = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = message.EventDate.ToLocalTime(),
                Width = 100,
                Tag = message
            };
            ctrl.ValueChanged += OnDateChanged;

            // Place it in a control  host
            return new ToolStripControlHost(ctrl);
        }
        #endregion
        #region OnDateChanged
        private static void OnDateChanged(Object sender, EventArgs e)
        // Not undoable since implemented here
        {
            var datePicker = (sender as DateTimePicker);
            if (null == datePicker)
                return;

            var message = datePicker.Tag as DEventMessage;
            if (null == message)
                return;

            // Update the event's date
            message.EventDate = datePicker.Value.ToUniversalTime();
        }
        #endregion

        #region ToolStripItem BuildStageDropdown(message)
        static ToolStripItem BuildStageDropdown(DEventMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var menuStage = new ToolStripDropDownButton(message.Stage.LocalizedAbbrev);

            var bCurrentStageFound = false;
            foreach (Stage stage in DB.TeamSettings.Stages)
            {
                AddStageMenuItem(menuStage,
                    stage.LocalizedAbbrev,
                    message,
                    (message.Stage == stage));

                if (message.Stage == stage)
                    bCurrentStageFound = true;
            }

            if (!bCurrentStageFound)
                AddStageMenuItem(menuStage, message.Stage.LocalizedAbbrev, message, true);

            return menuStage;
        }
        #endregion
        #region void AddStageMenuItem(...)
        static void AddStageMenuItem(ToolStripDropDownItem menu,
            string sMenuText,
            DEventMessage message,
            bool bChecked)
        {
            if (menu == null) throw new ArgumentNullException("menu");
            if (string.IsNullOrEmpty(sMenuText)) throw new ArgumentNullException("sMenuText");
            if (message == null) throw new ArgumentNullException("message");

            var item = new ToolStripMenuItem(sMenuText)
            {
                Tag = message, 
                Checked = bChecked
            };
            item.Click += OnChangeStage;
            menu.DropDownItems.Add(item);
        }
        #endregion
        #region OnChangeStage
        private static void OnChangeStage(object sender, EventArgs e)
        {
            // Get the various entities of interest
            var menuItem = sender as ToolStripMenuItem;
            if (null == menuItem)
                return;

            var message = menuItem.Tag as DEventMessage;
            if (null == message)
                return;

            var menu = menuItem.OwnerItem as ToolStripDropDownButton;
            if (null == menu)
                return;

            // The menu item's text is the localized form of the stage's abbreviation
            var stage = DB.TeamSettings.Stages.Find(
                StageList.FindBy.LocalizedAbbrev,
                menuItem.Text);
            if (null == stage)
                return;

            // Set the event's new stage
            message.Stage = stage;

            // Update the menu
            foreach (ToolStripMenuItem item in menu.DropDownItems)
                item.Checked = (item.Text == stage.LocalizedAbbrev);
            menu.Text = stage.LocalizedAbbrev;
        }
        #endregion

    }
    #endregion
}
