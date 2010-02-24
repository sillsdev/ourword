#region ***** EditableNoteTip.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    EditableNoteTip.cs
 * Author:  John Wimbish
 * Created: 11 Feb 2010
 * Purpose: ToolTip window for notes where the user can respond
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;
using OurWordData.Styles;
#endregion

namespace OurWord.ToolTips
{
    public partial class EditableNoteTip : ShadowedBalloon
    {
        #region Attr{g}: ENote NoteBlock
        ENote NoteBlock
        {
            get
            {
                Debug.Assert(null != m_NoteBlock);
                return m_NoteBlock;
            }
        }
        private readonly ENote m_NoteBlock;
        #endregion
        #region VAttr{g}: TranslatorNote Note
        TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != NoteBlock.Note);
                return NoteBlock.Note;
            }
        }
        #endregion

        #region Attr{g}: ToolTipContents ContentWindow
        OWWindow ContentWindow
        {
            get
            {
                return m_ContentWindow;
            }
        }
        readonly OWWindow m_ContentWindow;
        #endregion
        #region Attr{g}: OWBookmark Bookmark
        OWBookmark Bookmark
        {
            get
            {
                Debug.Assert(null != m_Bookmark);
                return m_Bookmark;
            }
        }
        private readonly OWBookmark m_Bookmark;
        #endregion

        private bool m_bMustRegenerateUnderlyingWindow;

        #region Constructor()
        public EditableNoteTip(ENote noteBlock)
        {
            // References the TranslatorNote that this tip is about
            m_NoteBlock = noteBlock;

            // Need to have created our controls prior to InitComponent
            m_ContentWindow = new OWWindow(OWWindow.WindowClass.Tooltip);
            InitializeComponent();

            // Bookmark the current position. That way, if on cleaning up we remove the 
            // annotation (e.g., because nothing was added), the underlying can be restored
            // to where it was (including scroll position)
            m_Bookmark = new OWBookmark(UnderlyingWindow);

            // Content window
            ContentWindow.Dock = DockStyle.Fill;
            m_panelClientArea.Controls.Add(ContentWindow);

            m_toolStrip.Renderer = new TrulyTransparentToolStripRenderer();

            // Shortcut key localizations. Do it here once, rather than as part
            // of ProcessCmdKey every time a key is pressed.
            m_keyCut = LocDB.GetShortcutKey(new[] { "m_ToolStrip" }, "m_menuCut",
                "Cu&t", "Ctrl+X");
            m_keyCopy = LocDB.GetShortcutKey(new[] { "m_ToolStrip" }, "m_menuCopy",
                "&Copy", "Ctrl+C");
            m_keyPaste = LocDB.GetShortcutKey(new[] { "m_ToolStrip" }, "m_menuPaste",
                "&Paste", "Ctrl+V");
        }
        #endregion

        // ToolTip Overrides -----------------------------------------------------------------
        #region OMethod: void OnLayoutControls()
        protected override void OnLayoutControls()
        {
            var rect = ContentAreaInsideMargins;

            const int xSpaceBetweenIconAndReference = 2;
            const int ySpaceBetweenTitleRowAndToolbar = 3;
            const int ySpaceBetweenToolbarAndContent = 3;

            var x = rect.X;
            var y = rect.Y;

            // TITLE ROW
            // Icon
            m_NoteIcon.Location = new Point(x, y);

            // Reference
            var xReference = m_NoteIcon.Right + xSpaceBetweenIconAndReference;
            var yReference = m_NoteIcon.Bottom - m_Reference.Height;
            m_Reference.Location = new Point(xReference, yReference);
            m_Reference.Size = new Size(MeasureReferenceWidth(), m_Reference.Height);

            // Close Button
            m_btnClose.Location = new Point(rect.Right - m_btnClose.Width, y);

            // Expand Buttom
            m_btnExpandWindow.Location = new Point(m_btnClose.Left - m_btnExpandWindow.Width, y);

            // Title
            var xTitleRight = (!PaintAsBalloon) ?
                rect.Right : m_btnExpandWindow.Left;
            var xTitle = m_Reference.Right;
            var yTitle = m_NoteIcon.Bottom - m_Title.Height;
            m_Title.Location = new Point(xTitle, yTitle);
            m_Title.Width = (xTitleRight - m_Title.Left);

            // TOOLBAR
            y = m_NoteIcon.Bottom + ySpaceBetweenTitleRowAndToolbar;
            m_toolStrip.Location = new Point(x,y);

            // EDITABLE CONTENT AREA
            y = m_toolStrip.Bottom + ySpaceBetweenToolbarAndContent;
            m_panelClientArea.Location = new Point(x, y);
            m_panelClientArea.Size = new Size(
                rect.Width,
                rect.Bottom - y);
            ContentWindow.SetSize(m_panelClientArea.Width, m_panelClientArea.Height);
            ContentWindow.Invalidate();
        }
        #endregion
        #region OMethod: void OnPopulateControls()
        protected override void OnPopulateControls()
        {
            m_Title.Text = Note.Title;

            PopulateAssignedTo();
            PopulateCategory();

            BuildDeleteControl();

            BuildContentWindow();

            SetControlToolTips();
        }
        #endregion
        #region OMethod: OnMouseLeave
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!TranslatorNote.DismissWhenMouseLeaves)
                return;

            base.OnMouseLeave(e);
        }
        #endregion

        // Reference -------------------------------------------------------------------------
        #region Method: int MeasureReferenceWidth()
        int MeasureReferenceWidth()
            // Determine the width of the reference, so we can shorten the control, so that
            // we don't have extra space prior to the Title
        {
            // Make sure we have the correct text
            m_Reference.Text = string.Format("{0}:", Note.GetDisplayableReference());

            // Perform the measurement
            var format = new StringFormat();
            var rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, m_Reference.Text.Length) };

            format.SetMeasurableCharacterRanges(ranges);

            var graphics = m_Reference.CreateGraphics();

            var regions = graphics.MeasureCharacterRanges(m_Reference.Text, 
                m_Reference.Font, rect, format);
            rect = regions[0].GetBounds(graphics);

            graphics.Dispose();

            var width = (int)(rect.Right + 1.0f);

            // Add a bit of extra padding to be on the safe side
            width += 2;

            return width;
        }
        #endregion

        // Title -----------------------------------------------------------------------------
        #region Cmd: cmdTitleTextChanged
        private void cmdTitleTextChanged(object sender, EventArgs e)
        {
            Note.Title = m_Title.Text;
            m_bMustRegenerateUnderlyingWindow = true;
        }
        #endregion

        // AssignedTo ------------------------------------------------------------------------
        #region Method: void SetAssignedToText()
        void SetAssignedToText()
        {
            // Button Text
            m_AssignedTo.Text = Note.Status.LocalizedName;

            // ToolTip
            SetAssignedToToolTip();

            // Checked / Unchecked
            SetAssignedToItemsChecks();

            // Icon in this tooltip
            m_NoteIcon.Image = ENote.BuildBitmap(BackgroundColor, Note.Status.IconColor, false);

            // Icon in main window
            var wndMain = NoteBlock.Window;
            NoteBlock.InitializeBitmap(wndMain.BackColor);
            m_bMustRegenerateUnderlyingWindow = true;
        }
        #region Method: void SetAssignedToToolTip()
        void SetAssignedToToolTip()
        {
            m_AssignedTo.ToolTipText = Note.Status.LocalizedToolTipText;
        }
        #endregion
        #region Method: void SetAssignedToItemsChecks()
        void SetAssignedToItemsChecks()
        {
            foreach (var item in m_AssignedTo.DropDownItems)
            {
                var menuItem = item as ToolStripMenuItem;
                if (null == menuItem)
                    continue;

                menuItem.Checked = (menuItem.Text == Note.Status.LocalizedName);
            }
        }
        #endregion
        #endregion
        #region Method: void PopulateAssignedTo()
        void PopulateAssignedTo()
        {
            m_AssignedTo.DropDownItems.Clear();

            foreach(var role in Role.AllRoles)
            {
                if (role.ThisUserCanAccess)
                {
                    var item = BuildAssignedToItem(role);
                    m_AssignedTo.DropDownItems.Add(item);
                }
            }

            // The text on the button is the TranslatorNote's status; also its tooltip
            SetAssignedToText();
        }
        #endregion
        #region Method: ToolStripMenuItem BuildAssignedToItem(Role)
        ToolStripMenuItem BuildAssignedToItem(Role role)
        {
            var item = new ToolStripMenuItem(role.LocalizedName);
            item.Click += OnAssignTo;

            var bIsCurrentRole = (role == Note.Status);

            item.Image = ENote.BuildBitmap(item.BackColor, role.IconColor, bIsCurrentRole);

            if (bIsCurrentRole)
                item.ForeColor = Color.Navy;

            return item;
        }
        #endregion
        #region Cmd: OnAssignTo
        private void OnAssignTo(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;
            var sNewStatus = item.Text;

            // Make sure the mouse goes back into the tooltip window so that it will not get
            // dismissed once the dropdown closes.
            MoveMouseIntoWindow();

            Note.Status = Role.FindFromLocalizedName(sNewStatus);

            if (Note.LastMessage.IsCompletelyEmpty)
            {
                var sBase = Loc.GetNotes("kChangedStatus", "Changed status to {0}.");
                Note.LastMessage.SimpleText = string.Format(sBase, sNewStatus);
                BuildContentWindow();
            }

            PopulateAssignedTo();
            SetAssignedToText();
        }
        #endregion

        // Categories ------------------------------------------------------------------------
        #region Method: void PopulateCategory()
        void PopulateCategory()
        {
            if (DB.TeamSettings.NotesCategories.Length == 0)
            {
                m_Category.Visible = false;
                return;
            }

            m_Category.DropDownItems.Clear();

            foreach(string category in DB.TeamSettings.NotesCategories)
            {
                var item = BuildCategoryItem(category);
                m_Category.DropDownItems.Add(item);
            }

            m_Category.DropDownItems.Add(BuildCategoryItem(TranslatorNote.NoCategory));

            m_Category.Text = (string.IsNullOrEmpty(Note.Category)) ? 
                Loc.GetNotes("category", "(category)") : 
                Note.Category;
        }
        #endregion
        #region Method: ToolStripMenuItem BuildCategoryItem(sCategoryName)
        ToolStripMenuItem BuildCategoryItem(string sCategoryName)
        {
            var item = new ToolStripMenuItem(sCategoryName);
            item.Click += OnCategory;

            var bIsCurrentRole = (sCategoryName == Note.Category);

            if (bIsCurrentRole)
            {
                item.ForeColor = Color.Navy;
                item.Checked = true;
            }

            return item;
        }
        #endregion
        #region Cmd: OnCategory
        private void OnCategory(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;
            Note.Category = item.Text;

            MoveMouseIntoWindow();

            if (Note.LastMessage.IsCompletelyEmpty)
            {
                var sBase = Loc.GetNotes("kChangedCategory", "Changed category to {0}.");
                Note.LastMessage.SimpleText = string.Format(sBase, Note.Category);
                BuildContentWindow();
            }

            PopulateCategory();
        }
        #endregion

        // Delete ----------------------------------------------------------------------------
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
            Close();

            // Since Close in some cases deletes notes, we have to test to see if it has
            // been deleted by whether or not it has an owning text. Removing the
            // TranslatorNote is an un-doable action. 
            if (null != note.OwningTextOrNull)
                (new DeleteNoteAction(ContentWindow, note)).Do();
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
            MoveMouseIntoWindow();

            // Removing the only discussion is the same as deleting the entire TranslatorNote;
            // the BuildDeleteControl method shouldn't let us arrive here in such condition.
            if (Note.Messages.Count < 2)
                return;

            // Remove it from the TranslatorNote
            Note.Messages.Remove(message);
            Note.Debug_VerifyIntegrity();

            // Recalc the window
            BuildContentWindow();
        }
        #endregion
        #region void BuildDeleteSimpleButton(JObject objToDelete)
        void BuildDeleteSimpleButton(JObject objToDelete)
        {
            var sButtonText = Loc.GetNotes("Delete", "Delete...");
            var btn = new ToolStripButton(sButtonText) {Image = JWU.GetBitmap("Delete.ico")};

            // Event handler depends on the object passed in
            if (null != objToDelete as DMessage)
                btn.Click += OnDeleteMessage;
            else if (null != objToDelete as TranslatorNote)
                btn.Click += OnDeleteNote;

            // Set the Tag to the object we want to delete; if null, then disable the button
            btn.Tag = objToDelete;
            if (null == objToDelete)
                btn.Enabled = false;

            // Tooltip
            btn.ToolTipText = Loc.GetNotes("DeleteNote_tip",
                "Delete this note or message.\n" +
                "(Disabled if there are messages in this\nnote that you did not author.)");

            m_toolStrip.Items.Add(new ToolStripSeparator());
            m_toolStrip.Items.Add(btn);
        }
        #endregion
        #region void BuildDeleteDropDownButton()
        void BuildDeleteDropDownButton()
        {
            var sButtonText = Loc.GetNotes("DeleteNote", "Delete");
            var btn = new ToolStripDropDownButton(sButtonText)
            {
                Image = JWU.GetBitmap("Delete.ico"),
                ToolTipText = Loc.GetNotes("DeleteNoteWithPriviledges_tip",
                    "Delete this note or any of its messages.")
            };

            var bDeleteNote = new ToolStripMenuItem(
                Loc.GetNotes("DeleteEntireNote", "Entire Note..."),
                null,
                new EventHandler(OnDeleteNote)) 
                {Tag = Note};
            btn.DropDownItems.Add(bDeleteNote);

            btn.DropDownItems.Add(new ToolStripSeparator());

            foreach (DMessage message in Note.Messages)
            {
                var s = message.Author + ", " + message.LocalTimeCreated.ToShortDateString();

                if (message.SimpleText.Length > 20)
                    s += "    (" + message.SimpleText.Substring(0, 20) + "...)";
                else
                    s += "    (" + message.SimpleText + ")";


                var b = new ToolStripMenuItem(s, null, new EventHandler(OnDeleteMessage)) 
                {
                    Tag = message
                };
                btn.DropDownItems.Add(b);
            }

            m_toolStrip.Items.Add(new ToolStripSeparator());
            m_toolStrip.Items.Add(btn);
        }
        #endregion
        #region Method: void BuildDeleteControl()
        void BuildDeleteControl()
        {
            // If we do not have global delete priviledges, our options are limited.
            if (!TranslatorNote.CanDeleteAnything)
            {
                // If there is one message and we authored it, we can delete the annotation
                if (Note.Messages.Count == 1 && DB.UserName == Note.LastMessage.Author)
                {
                    BuildDeleteSimpleButton(Note);
                    return;
                }

                // If there are more than one message, but we authored the last one, then
                // we can delete that message
                if (Note.Messages.Count > 1 && DB.UserName == Note.LastMessage.Author)
                {
                    BuildDeleteSimpleButton(Note.LastMessage);
                    return;
                }

                // Otherwise, the button is disabled.
                BuildDeleteSimpleButton(null);
                return;
            }

            // If we do have global priviledges, then we can delete anything we please
            if (Note.Messages.Count == 1)
            {
                BuildDeleteSimpleButton(Note);
                return;
            }

            // If we are here, we have multiple messages, so we go to a dropdown button listing
            // all of the options
            BuildDeleteDropDownButton();
        }
        #endregion

        // Content Window --------------------------------------------------------------------
        #region Method: void BuildContentWindow()
        void BuildContentWindow()
        {
            // We automatically insert a DMessage (response) with today's date and the current
            // user as author, so there is no Response button they have to click on; its just
            // automatically there and ready for typing.
            EnsureHasEditableResponseToday();

            ContentWindow.Clear();

            foreach(DMessage message in Note.Messages)
            {
                var p = IsEditableResponse(message) ?
                    BuildEditableMessage(message) :
                    BuildMessage(message);
                ContentWindow.Contents.Append(p);
            }

            ContentWindow.LoadData();
        }
        #endregion
        #region Method: OWPara BuildMessage(DMessage)
        OWPara BuildMessage(DMessage message)
        {
            var owp = new OWPara(
                Note.Behavior.GetWritingSystem(Note),
                StyleSheet.TipMessage,
                message,
                BackgroundColor,
                OWPara.Flags.None);

            // Add the author's name and the date as uneditable labels
            PrependAuthorAndDate(owp, message);

            return owp;
        }
        #endregion
        #region Method: void PrependAuthorAndDate(EContainer pDestination, DMessage message)
        void PrependAuthorAndDate(EContainer pDestination, DMessage message)
        {
            var writingSystem = Note.Behavior.GetWritingSystem(Note).Name;

            var fontLabel = StyleSheet.TipMessage.GetFont(writingSystem, FontStyle.Bold, G.ZoomPercent);
            var sAuthor = message.Author + ",";
            var author = new OWPara.ELabel(fontLabel, new DLabel(sAuthor));

            var fontDate = StyleSheet.TipMessage.GetFont(writingSystem, FontStyle.Italic, G.ZoomPercent);
            var sDate = message.LocalTimeCreated.ToShortDateString() + ":\u00A0";
            var date = new OWPara.ELabel(fontDate, new DLabel(sDate));

            pDestination.InsertAt(0, author);
            pDestination.InsertAt(1, date);
        }
        #endregion
        #region Method: EItem BuildEditableMessage(DMessage message)
        EItem BuildEditableMessage(DMessage message)
        {
            Debug.Assert(null != Note);
            var writingSystem = Note.Behavior.GetWritingSystem(Note);

            // Create the paragraph
            var owp = new OWPara(
                writingSystem, StyleSheet.TipMessage, message, Color.White,
                (OWPara.Flags.IsEditable | OWPara.Flags.CanItalic));

            // Add the author's name and the date as uneditable labels
            PrependAuthorAndDate(owp, message);

            // Make it stand out by placing it inside a container that shows the color better.
            var eEdit = new EColumn();
            eEdit.Border = new EContainer.SquareBorder(eEdit)
            {
                BorderPlacement = EContainer.BorderBase.BorderSides.Top,
                Padding = {Top = 2},
                Margin = {Top = 2}
            };
            eEdit.Append(owp);

            return eEdit;
        }
        #endregion
        #region Method: bool IsEditableResponse(DMessage message)
        bool IsEditableResponse(DMessage message)
        {
            // The message must have been entered by the current user
            if (message.Author != DB.UserName)
                return false;

            // The message should have been entered today
            if (message.UtcCreated.Date == DateTime.Today)
                return true;

            // If not today, then it is editable only if all following messages are also
            // editable. (Thus recursive)
            var iMessage = Note.Messages.FindObj(message);
            Debug.Assert(-1 != iMessage);
            if (iMessage == Note.Messages.Count - 1)
                return true;
            if (IsEditableResponse(Note.Messages[iMessage+1]))
                return true;

            return false;
        }
        #endregion
        #region Method: void EnsureHasEditableResponseToday()
        void EnsureHasEditableResponseToday()
        {
            // Scan: if we have an editable message with today's date then we're done
            foreach(DMessage message in Note.Messages)
            {
                if (IsEditableResponse(message) && message.UtcCreated.Date == DateTime.Today)
                    return;
            }

            // If here, we don't have one, so we create it
            var newMessage = new DMessage();
            Note.Messages.Append(newMessage);
            Note.Debug_VerifyIntegrity();
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Cmd: cmdFirstShown
        private void cmdFirstShown(object sender, EventArgs e)
        {
            if (false == ContentWindow.Contents.Select_LastWord_End())
                ContentWindow.Selection = null;

            ContentWindow.Focus();
        }
        #endregion
        #region Cmd: cmdClose
        private void cmdClose(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
        #region Method: void SetControlToolTips()
        void SetControlToolTips()
        {
            var tipReference = new ToolTip();
            tipReference.SetToolTip(m_Reference, Note.GetFullReference());

            var tipTitle = new ToolTip();
            var sTitle = Loc.GetNotes("tipClickOnTitle", "Click on the Title to edit it");
            tipTitle.SetToolTip(m_Title, sTitle);

            var tipClose = new ToolTip();
            var sClose = Loc.GetNotes("tipCloseWindow", "Close this window");
            tipClose.SetToolTip(m_btnClose, sClose);

            var tipExpand = new ToolTip();
            var sExpand = Loc.GetNotes("tipExpand", "Change this balloon popup into a dialog that you can resize");
            tipExpand.SetToolTip(m_btnExpandWindow, sExpand);
        }
        #endregion
        #region Cmd: cmdClosing
        private void cmdClosing(object sender, FormClosingEventArgs e)
        {
            Note.RemoveEmptyMessages();

            // If this results in an empty TranslatorNote, we need to completely delete it
            if (!Note.HasMessages)
            {
                var OwningText = Note.OwningTextOrNull;
                Debug.Assert(null != OwningText);
                OwningText.TranslatorNotes.Remove(Note);
                m_bMustRegenerateUnderlyingWindow = true;
            }

            // Regenerate the underlying window display
            if (m_bMustRegenerateUnderlyingWindow)
            {
                UnderlyingWindow.LoadData();
                Bookmark.RestoreWindowSelectionAndScrollPosition();
//                UnderlyingWindow.Focus();
            }
        }
        #endregion
        #region Cmd: cmdExpandToNormalDialogWindow
        private void cmdExpandToNormalDialogWindow(object sender, EventArgs e)
            // The window starts out at a balloon tooltip; this turns it into a resizeable
            // dialog, so the user can see more, move the window, etc.
        {
            PaintAsBalloon = false;
            Text = Loc.GetNotes("kNoteWindowTitle", "Note");

            m_btnExpandWindow.Hide();
            m_btnClose.Hide();
        }
        #endregion

        // Editing commands ------------------------------------------------------------------
        #region Cmd: cmdItalics
        private void cmdItalics(object sender, EventArgs e)
        {
            ContentWindow.cmdToggleItalics();
        }
        #endregion
        #region Cmd: cmdCut
        private void cmdCut(object sender, EventArgs e)
        {
            ContentWindow.cmdCut();
        }
        #endregion
        #region Cmd: cmdCopy
        private void cmdCopy(object sender, EventArgs e)
        {
            ContentWindow.cmdCopy();
        }
        #endregion
        #region Cmd: cmdPaste
        private void cmdPaste(object sender, EventArgs e)
        {
            ContentWindow.cmdPaste();
        }
        #endregion
        #region OMethod: ProcessCmdKey
        private readonly Keys m_keyCut;
        private readonly Keys m_keyCopy;
        private readonly Keys m_keyPaste;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            // Enables (localized) keyboard shortcuts of toolbar items
        {
            if (keyData == m_keyCut)
                ContentWindow.cmdCut();
            else if (keyData == m_keyCopy)
                ContentWindow.cmdCopy();
            else if (keyData == m_keyPaste)
                ContentWindow.cmdPaste();

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion


    }
}
