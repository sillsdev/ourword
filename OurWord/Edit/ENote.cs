#region ***** ENote.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    ENote.cs
 * Author:  John Wimbish
 * Created: 02 Sep 2009
 * Purpose: An note icon, pointing to a not
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
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion
#endregion

namespace OurWord.Edit
{
    public class ENote : EBlock
    {
        #region Attr{g}: TranslatorNote Note
        public TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }
        TranslatorNote m_Note = null;
        #endregion
        #region Attr{g}: Bitmap Bmp - the note's bitmap
        Bitmap Bmp
        {
            get
            {
                // Get this when we first need it. We previously had this in
                // the constructor; but the problem was that we do not
                // have access to the Window at the time of construction.
                if (null == m_bmp)
                    m_bmp = Note.GetBitmap(Window.BackColor);

                Debug.Assert(null != m_bmp);
                return m_bmp;
            }
        }
        Bitmap m_bmp = null;
        #endregion
        #region OAttr{g}: float Width
        public override float Width
        {
            get
            {
                return Bmp.Width;
            }
            set
            {
                // Can't be set; its the nature of the bitmap
            }
        }
        #endregion

        #region Constructor(TranslatorNote)
        public ENote(TranslatorNote _Note)
            : base(null, "")
        {
            m_Note = _Note;
        }
        #endregion
        #region OMethod: void CalculateWidth(Graphics g)
        public override void CalculateWidth(Graphics g)
        {
            // Do-nothing override
        }
        #endregion
        #region Method: void CauseBmpReset()
        public void CauseBmpReset()
        {
            m_bmp = null;
        }
        #endregion

        #region Method: override void Paint()
        public override void Paint()
        {
            Draw.Image(Bmp, Position);
        }
        #endregion

        #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
        public override Cursor MouseOverCursor
        {
            get
            {
                return Cursors.Hand;
            }
        }
        #endregion
        #region Method: override void cmdLeftMouseClick(PointF pt)
        public override void cmdLeftMouseClick(PointF pt)
        {
            OWToolTip.ToolTip.LaunchToolTipWindow();
        }
        #endregion

        // Tooltip ---------------------------------------------------------------------------
        // AssignStatus within ToolStrip
        #region Cmd: OnAssignStatus - user has responded to the Status dropdown
        public void OnAssignStatus(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var en = item.Tag as ENote;
            if (null == en)
                return;

            // Move the mouse back into the tooltip window so the window will
            // not get dismissed when the mouse next moves.
            var wnd = OWToolTip.ToolTip.ContentWindow;
            int yWndBottom = wnd.PointToScreen(new Point(wnd.Left, wnd.Bottom)).Y;
            if (Cursor.Position.Y > yWndBottom)
                Cursor.Position = new Point(Cursor.Position.X, yWndBottom - 3);

            // Make the change
            (new ChangeStatus(Window, en, item)).Do();
        }
        #endregion
        #region Method: void SetStatusToolTip(ToolStripDropDownButton item)
        public void SetStatusToolTip(ToolStripDropDownButton item)
        {
            string sTip = "";
            if (Note.Status == DMessage.Closed)
            {
                sTip = Loc.GetNotes("btnStatusClosed",
                    "This note has been closed out (considered finished).\n" +
                    "Click here to re-open it, by assigning it to someone.");
            }
            else
            {
                string sBase = Loc.GetNotes("btnStatusOpen", 
                    "This note has been assigned to {0}.");
                sTip = LocDB.Insert(sBase, new string[] { Note.Status });
            }
            item.ToolTipText = sTip;
        }
        #endregion
        #region Method: var BuildAssignStatusItem(sDisplayValue)
        ToolStripMenuItem BuildAssignStatusItem(string sDisplayValue)
        {
            var item = new ToolStripMenuItem(sDisplayValue);
            item.Tag = this;
            item.Click += new EventHandler(OnAssignStatus);

            if (sDisplayValue == Note.Status)
                item.Checked = true;

            return item;
        }
        #endregion
        #region Method: var BuildAssignStatusControl()
        ToolStripDropDownButton BuildAssignStatusControl()
        {
            // Create the dropdown button
            var btnStatus = new ToolStripDropDownButton();
            btnStatus.Name = "Status";
            SetStatusToolTip(btnStatus);

            // At a minimum we have "Anyone" and "Closed"
            btnStatus.DropDownItems.Add(BuildAssignStatusItem(DMessage.Anyone));
            btnStatus.DropDownItems.Add(BuildAssignStatusItem(DMessage.Closed));

            // If we have additional people, then we add a separator line
            if (DB.Project.People.Length > 0)
                btnStatus.DropDownItems.Add(new ToolStripSeparator());

            // Add in the people
            foreach (string sPerson in DB.Project.People)
                btnStatus.DropDownItems.Add(BuildAssignStatusItem(sPerson));

            // The text on the button is the Note's status
            btnStatus.Text = Note.Status;

            return btnStatus;
        }
        #endregion

        // Response within ToolStrip
        #region Cmd: OnRespond
        public void OnRespond(object sender, EventArgs e)
        {
            var item = sender as ToolStripButton;
            if (null == item)
                return;

            var en = item.Tag as ENote;
            if (null == en)
                return;

            (new AddMessageAction(Window, en)).Do();
        }
        #endregion
        #region Method: var BuildRespondControl()
        ToolStripButton BuildRespondControl()
        {
            // Create the button
            string sButtonText = Loc.GetNotes("AddResponse", "Respond");
            var btnAddResponse = new ToolStripButton(sButtonText);
            btnAddResponse.Tag = this;
            btnAddResponse.Image = JWU.GetBitmap("Note_OldVersions.ico");

            // Command handler
            btnAddResponse.Click += new EventHandler(OnRespond);

            // Normal tooltip
            btnAddResponse.ToolTipText = Loc.GetNotes("AddResponse_tip", 
                "Add your response to this note.");

            // If a user has already entered a message today, they just edit it directly,
            // rather than adding a new one. So we disable the button, but leave it there
            // so that the user isn't confused by a changing toolstrip.
            if (Note.LastMessage.Author == DB.UserName &&
                Note.LastMessage.Created.Date == DateTime.Today)
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

        // Delete within ToolStrip
        #region Cmd: OnDeleteNote
        void OnDeleteNote(object sender, EventArgs e)
        {
            // Get the user interface item
            var btn = sender as ToolStripItem;
            Debug.Assert(null != btn);
            if (null == btn)
                return;

            // Get the note
            var note = btn.Tag as TranslatorNote;
            Debug.Assert(null != note);
            if (null == note)
                return;

            // Give the user the opportunity to change his/her mind
            string sText = note.SelectedText;
            if (sText.Length > 40)
                sText = sText.Substring(0, 40) + "...";
            string sMsgAddition = "\n\n\"" + sText + "\"";
            if (false == Messages.ConfirmNoteDeletion(sMsgAddition))
                return;

            // Close the ToolTip window
            OWToolTip.ToolTip.CloseWindow();

            // Remove the note
            (new DeleteNoteAction(Window, note)).Do();
        }
        #endregion
        #region Cmd: OnDeleteMessage
        void OnDeleteMessage(object sender, EventArgs e)
        {
            // Get the user interface item
            var btn = sender as ToolStripItem;
            Debug.Assert(null != btn);
            if (null == btn)
                return;

            // Get the target message
            var message = btn.Tag as DMessage;
            Debug.Assert(null != message);
            if (null == message)
                return;

            // Give the user the opportunity to change his/her mind
            string sText = message.Author + ", " + message.Created.ToShortDateString();
            bool bProceed =  LocDB.Message(
                "msgConfirmMessageDeletion",
                "Are you sure you want to delete the message:\n  {0}?",
                new string[] { sText },
                LocDB.MessageTypes.YN);
            if (!bProceed)
                return;

            // Remove the message
            (new RemoveMessageAction(Window, message)).Do();
        }
        #endregion
        #region Method: ToolStripButton BuildDeleteSimpleButton(JObject objWhatToDelete)
        ToolStripButton BuildDeleteSimpleButton(JObject objWhatToDelete)
        {
            string sButtonText = Loc.GetNotes("DeleteNote", "Delete...");
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
        #region Method: ToolStripDropDownButton BuildDeleteDropDownButton()
        ToolStripDropDownButton BuildDeleteDropDownButton()
        {
            string sButtonText = Loc.GetNotes("DeleteNote", "Delete...");
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
                string s = message.Author + ", " + message.Created.ToShortDateString();

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
        ToolStripItem BuildDeleteNoteControl()
        {
            // If we do not have global delete priveledges, our options are limited.
            if (!TranslatorNote.CanDeleteAnything)
            {
                // If there is one message and we authored it, we can delete the note
                if (Note.Messages.Count == 1 && DB.UserName == Note.LastMessage.Author)
                    return BuildDeleteSimpleButton(Note);

                // If there are more than one message, but we authored the last one, then
                // we can delete that message
                if (Note.Messages.Count > 1 && DB.UserName == Note.LastMessage.Author)
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

        // Text Contents
        #region Method: var BuildNoteTitle(ws, sNoteTitle, sIconResource)
        OWPara BuildNoteTitle(JWritingSystem ws, 
            string sNoteTitle,
            string sIconResource)
        {
            var wnd = OWToolTip.ToolTip.ContentWindow;

            // Basic Text this will go into
            var dbt = new DBasicText();

            // Note Title if supplied
            if (!string.IsNullOrEmpty(sNoteTitle))
            {
                var pNoteTitle = new DPhrase( DStyleSheet.c_StyleAbbrevBold,
                    sNoteTitle + DPhrase.c_chInsertionSpace + "-" + DPhrase.c_chInsertionSpace);
                dbt.Phrases.Append(pNoteTitle);
            }

            // Start with the reference, in italics
            string sBookRef = Note.GetDisplayableReference() + ":" + DPhrase.c_chInsertionSpace;
            var pRef = new DPhrase(DStyleSheet.c_StyleAbbrevItalic, sBookRef);
            dbt.Phrases.Append(pRef);

            // Create a truncated and quote-surrounded version of the selected text
            string sSelectedText = Note.SelectedText;
            if (sSelectedText.Length > 40)
                sSelectedText = sSelectedText.Substring(0, 40) + "...";
            sSelectedText = "\"" + sSelectedText + "\"";
            var pSelectedText = new DPhrase(DStyleSheet.c_sfmParagraph, sSelectedText);
            dbt.Phrases.Append(pSelectedText);


            // Create the paragraph
            OWPara pTitle = new OWPara(
                ws,
                DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleToolTipHeader),
                dbt.Phrases.AsVector);

            // Pre-pend the icon
            pTitle.InsertAt(0, new EIcon(sIconResource));

            return pTitle;
        }
        #endregion
        #region Method: var BuildSingleMessageContents(...)
        EContainer BuildSingleMessageContents(JWritingSystem ws, DMessage message, string sStyle)
        {
            bool bIsEditable = message.IsEditable;

            // Background color depends on editibility
            var clrBackground = (bIsEditable) ? Color.White : Color.Cornsilk;

            // Flags depend on editibility
            var flags = (bIsEditable) ?
                (OWPara.Flags.IsEditable | OWPara.Flags.CanItalic) : 
                (OWPara.Flags.None);

            // Create the paragraph
            var p = new OWPara(
                ws,
                DB.StyleSheet.FindParagraphStyle(sStyle),
                message,
                clrBackground,
                flags);

            // If the message is editable, we want to make it stand out by placing it inside
            // a container that shows the color better.
            if (bIsEditable)
            {
                var eEdit = new EColumn();
                eEdit.Border = new EContainer.RoundedBorder(eEdit, 8);
                eEdit.Border.Padding.Top = 3;
                eEdit.Border.Padding.Bottom = 3;
                eEdit.Border.BorderColor = Color.Navy;
                eEdit.Border.FillColor = Color.White;
                eEdit.Append(p);
                return eEdit;
            }
            else
            {
                return p;
            }
        }
        #endregion
        #region Method: var BuildInteractiveMessageContents(...)
        EContainer BuildInteractiveMessageContents(JWritingSystem ws)
        {
            // We'll place the meessages in their own container, so we can have a top and
            // bottom border
            var eMessages = new EColumn();
            eMessages.Border = new EContainer.SquareBorder(eMessages);
            eMessages.Border.BorderPlacement = EContainer.BorderBase.BorderSides.TopAndBottom;
            eMessages.Border.Padding.Bottom = 5;     // So not too tight with bottom horz line
            eMessages.Border.Margin.Top = 5;         // So not too tight with the title

            // Add each message
            foreach (DMessage message in Note.Messages)
            {
                // Author and Date
                var pMessageHeader = new OWPara(ws,
                    DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleMessageHeader),
                    new DPhrase[] { 
                        new DPhrase( DStyleSheet.c_StyleToolTipHeader, message.Author),
                        new DPhrase( DStyleSheet.c_StyleToolTipText, ", "),
                        new DPhrase( DStyleSheet.c_StyleToolTipText, message.Created.ToShortDateString())
                    });
                eMessages.Append(pMessageHeader);

                // Contents
                var pMessage = BuildSingleMessageContents(ws, message,
                    DStyleSheet.c_StyleMessageContent);
                eMessages.Append(pMessage);
            }

            return eMessages;
        }
        #endregion

        // Load, based on Class and Context
        #region Method: bool LoadToolTip_HintFromFront(ToolTipContents)
        bool LoadToolTip_HintFromFront(ToolTipContents wnd)
        {
            // Only for Hints from the Front Translation
            if (Note.IsTargetTranslationNote)
                return false;
            if (!Note.IsHintForDraftingNote)
                return false;

            // The writing system will be the vernacular of rhe front
            var ws = DB.FrontTranslation.WritingSystemVernacular;

            // The title is be "Drafting Hint."
            string sTitle = Loc.GetString("kDraftingHint", "Drafting Hint");
            var pTitle = BuildNoteTitle(ws, sTitle, "NoteHint_Me.ico");
            wnd.Contents.Append(pTitle);

            // We are only interested in the first paragraph(message), not any subsequent
            // discussion which the translators in the Front translation might have
            // conducted.
            var pMessage = BuildSingleMessageContents(ws, Note.FirstMessage, 
                DStyleSheet.c_StyleToolTipText);
            wnd.Contents.Append(pMessage);

            return true;
        }
        #endregion
        #region Method: bool LoadToolTip_HintForDaughter(ToolTipContents)
        bool LoadToolTip_HintForDaughter(ToolTipContents wnd)
        {
            // We only want Hints that are in the Target Translation
            if (!Note.IsTargetTranslationNote || !Note.IsHintForDraftingNote)
                return false;

            // The writing system will be the vernacular of rhe target translation
            var ws = DB.TargetTranslation.WritingSystemVernacular;

            // The title is be "Drafting Hint." 
            string sTitle = Loc.GetString("kDraftingHint", "Drafting Hint");
            var pTitle = BuildNoteTitle(ws, sTitle, "NoteHint_Me.ico");
            wnd.Contents.Append(pTitle);

            // Messages section
            wnd.Contents.Append(BuildInteractiveMessageContents(ws));

            // Add the toolbar for user actions
            wnd.Contents.Append(BuildToolStrip(wnd));

            return true;
        }
        #endregion
        #region Method: bool LoadToolTip_Exegesis(ToolTipContents)
        bool LoadToolTip_Exegesis(ToolTipContents wnd)
        {
            // Only exegesis notes
            if (!Note.IsExegeticalNote)
                return false;

            // The writing system is that of the consultant
            var ws = DB.TargetTranslation.WritingSystemConsultant;

            // The title is "Exegetical Note"
            string sTitle = Loc.GetString("kExegeticalNote", "Exegetical Note");
            var pTitle = BuildNoteTitle(ws, sTitle, "NoteExegesis_Me.ico");
            wnd.Contents.Append(pTitle);

            // Messages section
            wnd.Contents.Append(BuildInteractiveMessageContents(ws));

            // Add the toolbar for user actions
            wnd.Contents.Append(BuildToolStrip(wnd));
            return true;
        }
        #endregion
        #region Method: bool LoadToolTip_Consultant(ToolTipContents)
        bool LoadToolTip_Consultant(ToolTipContents wnd)
        {
            // Only exegesis notes
            if (!Note.IsConsultantNote)
                return false;

            // The writing system is that of the consultant
            var ws = DB.TargetTranslation.WritingSystemConsultant;

            // The title is "Consultant Note"
            string sTitle = Loc.GetString("kConsultantNote", "Consultant Note");
            var pTitle = BuildNoteTitle(ws, sTitle, "NoteConsultant_Me.ico");
            wnd.Contents.Append(pTitle);

            // Messages section
            wnd.Contents.Append(BuildInteractiveMessageContents(ws));

            // Add the toolbar for user actions
            wnd.Contents.Append(BuildToolStrip(wnd));
            return true;
        }
        #endregion
        #region Method: bool LoadToolTip_General(ToolTipContents)
        bool LoadToolTip_General(ToolTipContents wnd)
        {
            if (!Note.IsTargetTranslationNote)
                return false;
            if (!Note.IsGeneralNote)
                return false;

            // The writing system will be the vernacular of rhe target
            var ws = DB.TargetTranslation.WritingSystemVernacular;

            // Note Title
            var pTitle = BuildNoteTitle(ws, 
                null, 
                Note.IconResourceForClass);
            wnd.Contents.Append(pTitle);

            // Messages section
            wnd.Contents.Append(BuildInteractiveMessageContents(ws));

            // Add the toolbar for user actions
            wnd.Contents.Append(BuildToolStrip(wnd));

            return true;
        }
        #endregion
        #region Method: void LoadToolTip(ToolTipContents)
        override public void LoadToolTip(ToolTipContents wnd)
        {
            wnd.Contents.Clear();

            if (LoadToolTip_General(wnd))
                return;
            if (LoadToolTip_HintFromFront(wnd))
                return;
            if (LoadToolTip_HintForDaughter(wnd))
                return;
            if (LoadToolTip_Exegesis(wnd))
                return;
            if (LoadToolTip_Consultant(wnd))
                return;
        }
        #endregion

        // Misc
        #region Method: EToolStrip BuildToolStrip(OWWindow)
        EToolStrip BuildToolStrip(OWWindow wnd)
        {
            // Create the EToolStrip
            var toolstrip = new EToolStrip(wnd);
            ToolStrip ts = toolstrip.ToolStrip; // Shorthand

            // Respond button
            var btn = BuildRespondControl();
            if (null != btn)
            {
                // Add the button
                ts.Items.Add(btn);

                // Add space between the next control
                ts.Items.Add(new ToolStripLabel("  "));
            }

            // Add the Assigned Status Control
            ts.Items.Add(BuildAssignStatusControl());

            // Add space between the next control
            ts.Items.Add(new ToolStripLabel("  "));

            // Add the Delete Note control
            ts.Items.Add(BuildDeleteNoteControl());

            return toolstrip;
        }
        #endregion
        #region Attr{g}: bool HasToolTip()
        public override bool HasToolTip()
        {
            return true;
        }
        #endregion
    }
}
