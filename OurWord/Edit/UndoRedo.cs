/**********************************************************************************************
 * Project: OurWord!
 * File:    UndoRedo.cs
 * Author:  John Wimbish
 * Created: 01 Mar 2008
 * Purpose: Handles Undo / Redo actions
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
using System.Threading;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Edit
{
    // General Undo/Redo Mechanism -----------------------------------------------------------
    #region CLASS: Action - Superclass for Do, Undo, Redo commands
    public class Action
    {
        #region Attr{g}: string DisplayName
        public string DisplayName
        {
            get
            {
                return m_sDisplayName;
            }
        }
        protected string m_sDisplayName;
        #endregion
        #region Attr{g}: virtual string Contents
        virtual public string Contents
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region Constructor(sDisplayName)
        public Action(string sDisplayName)
        {
            m_sDisplayName = sDisplayName;
        }
        #endregion

        #region Method: void Push() - Place this action on the UR Stack
        protected void Push()
        {
            G.URStack.Push(this);
        }
        #endregion

        #region VMethod: bool Do() - Perform the action the initial time; T if successful
        virtual public bool Do()
        {
            return false;
        }
        #endregion
        #region VMethod: void Redo() - Perform the action following an Undo
        virtual public void Redo()
        {
        }
        #endregion
        #region VMethod: void Undo() - Undo what was done by Do or Redo
        virtual public void Undo()
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: UndoRedoStack
    public class UndoRedoStack
    {
        #region Attr{g}: int MaxDepth - the maximum number of Undo actions in the stack
        public int MaxDepth
        {
            get
            {
                return m_nMaxDepth;
            }
        }
        int m_nMaxDepth = 0; // zero is interpreted as infinite
        #endregion
        #region VAttr{g}: bool AllowsInfiniteDepth
        public bool AllowsInfiniteDepth
        {
            get
            {
                return (MaxDepth == 0);
            }
        }
        #endregion

        // Menus -----------------------------------------------------------------------------
        ToolStripMenuItem m_menuUndo;
        ToolStripMenuItem m_menuRedo;
        #region Method: void SetMenuText(ToolStripMenuItem menuItem, Action action, string sEnglishMenuText)
        void SetMenuText(ToolStripMenuItem menuItem, Action action, string sEnglishMenuText)
        {
            if (menuItem == null)
                return;

            // If there's not action, then we just use the basic menu item
            if (null == action)
            {
                LocDB.Localize(menuItem);
                menuItem.Enabled = false;
                return;
            }

            menuItem.Enabled = true;

            string sMenuText = sEnglishMenuText + " " + action.DisplayName;

            // Compute an ID from the English
            string sID = "";
            foreach (char ch in sMenuText)
            {
                if (!char.IsWhiteSpace(ch) && ch != '&')
                    sID += ch;
            }

            // If there are no contents, then we just display the "Undo Typing" sort of thing
            if (string.IsNullOrEmpty(action.Contents))
            {
                menuItem.Text = G.GetLoc_UndoRedo(sID, sMenuText);
                return;
            }

            // Truncate the contents if needed
            int nMaxLen = 20;
            string sContents = action.Contents;
            if (sContents.Length > nMaxLen)
                sContents = (sContents.Substring(0, nMaxLen) + "...");

            // Insert the contents of the Undo action
            string sBase = G.GetLoc_UndoRedo(sID + "_what", sMenuText + " '{0}'");
            menuItem.Text = LocDB.Insert(sBase, new string[] { sContents });
        }
        #endregion
        #region Method: void SetMenuText()
        public void SetMenuText()
        {
            SetMenuText(m_menuUndo, PeekUndo, "&Undo");
            SetMenuText(m_menuRedo, PeekRedo, "&Redo");
        }
        #endregion

        // Attrs: Undo & Redo Stacks ---------------------------------------------------------
        #region Attr{g}: Action[] UndoStack
        Action[] UndoStack
        {
            get
            {
                Debug.Assert(null != m_vUndoStack);
                return m_vUndoStack;
            }
        }
        Action[] m_vUndoStack;
        #endregion
        #region Attr{g}: Action[] RedoStack
        Action[] RedoStack
        {
            get
            {
                Debug.Assert(null != m_vRedoStack);
                return m_vRedoStack;
            }
        }
        Action[] m_vRedoStack;
        #endregion

        // Helper methods: Generic Stack Operations ------------------------------------------
        #region Method: void _Clear(ref Action[])
        void _Clear(ref Action[] vAction)
        {
            vAction = new Action[0];
        }
        #endregion
        #region Method: void _PushAction(ref Action[], newAction)
        void _PushAction(ref Action[] vAction, Action newAction)
        {
            // Make certain we have a valid action to input
            if (null == newAction)
                return;

            // Create a new, longer vector to hold the larger set of actions
            Action[] v = new Action[vAction.Length + 1];

            // Transfer the existig actions over
            for (int i = 0; i < vAction.Length; i++)
                v[i] = vAction[i];

            // Append the new one
            v[vAction.Length] = newAction;

            // Point to our new array
            vAction = v;

            // Make sure we don't exceed the Max Depth requirement
            _CheckMaxDepth(ref vAction);
        }
        #endregion
        #region Method: Action _PopAction(ref Action[])
        Action _PopAction(ref Action[] vAction)
        {
            if (vAction.Length == 0)
                return null;

            // Remember the desired action so we can return it at the end
            Action action = vAction[vAction.Length - 1];

            // Create a new, shorter vector to hold the resulting list of actions
            Action[] v = new Action[vAction.Length - 1];
            for (int i = 0; i < vAction.Length - 1; i++)
                v[i] = vAction[i];
            vAction = v;

            // Return the action that we popped
            return action;
        }
        #endregion
        #region Method: void _CheckMaxDepth(ref Action[])
        void _CheckMaxDepth(ref Action[] vAction)
        {
            // Nothing to do if we're permitting infinite depth
            if (AllowsInfiniteDepth)
                return;

            // Nothing to do if we haven't reached the Max Depth
            if (vAction.Length <= MaxDepth)
                return;

            // Transfer into a smaller vector
            Action[] v = new Action[MaxDepth];
            for (int i = 0; i < MaxDepth; i++)
                v[i] = vAction[i + 1];
            vAction = v;
        }
        #endregion

        // Public access to the Stacks -------------------------------------------------------
        #region Method: void Push(Action) - Insert a new edit action onto the stack
        public void Push(Action action)
        {
            // Append the new action to the Undo stack
            _PushAction(ref m_vUndoStack, action);

            // Clear out the Redo Stack
            _Clear(ref m_vRedoStack);

            // Update the menus
            SetMenuText();
        }
        #endregion
        #region Method: void Clear() - Clear out the Undo and Redo stacks
        public void Clear()
            // Clears out the Undo and Redo stacks
        {
            _Clear(ref m_vUndoStack);
            _Clear(ref m_vRedoStack);
            SetMenuText();
        }
        #endregion
        #region VAttr{g}: bool HasUndo - T if there is an action available to Undo
        public bool HasUndo
        {
            get
            {
                return (UndoStack.Length > 0);
            }
        }
        #endregion
        #region VAttr{g}: bool HasRedo - T if there is an action available to Redo
        public bool HasRedo
        {
            get
            {
                return (RedoStack.Length > 0);
            }
        }
        #endregion
        #region VAttr{g}: Action PeekUndo - Examine the topmost Undo on the stack
        public Action PeekUndo
        {
            get
            {
                if (HasUndo)
                    return UndoStack[UndoStack.Length - 1];
                return null;
            }
        }
        #endregion
        #region VAttr{g}: Action PeekRedo - Examine the topmost Redo on the stack
        public Action PeekRedo
        {
            get
            {
                if (HasRedo)
                    return RedoStack[RedoStack.Length - 1];
                return null;
            }
        }
        #endregion

        // Undo / Redo -----------------------------------------------------------------------
        #region Method: Action Undo() - Pops the Undo action, performs it, adds it to the Redo stack
        public Action Undo()
        {
            // Nothing to do if there's nothing in the stack
            if (!HasUndo)
                return null;

            // Remove the action from the Undo stack
            Action action = _PopAction(ref m_vUndoStack);

            // Append it to the redo stack
            _PushAction(ref m_vRedoStack, action);

            // Do the Undo action
            action.Undo();

            // Update the menus
            SetMenuText();

            // Return it to the caller
            return action;
        }
        #endregion
        #region Method: Action Redo() - Pops the Redo action, performs it, adds it to the Uno stack
        public Action Redo()
        {
            // Nothing to do if there's nothing in the stack
            if (!HasRedo)
                return null;

            // Remove the action from the Redo stack
            Action action = _PopAction(ref m_vRedoStack);

            // Append it to the Undo stack
            _PushAction(ref m_vUndoStack, action);

            // Do the Redo action
            action.Redo();

            // Update the menus
            SetMenuText();

            // Return it to the caller
            return action;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public UndoRedoStack(int nMaxDepth, ToolStripMenuItem mUndo, ToolStripMenuItem mRedo)
        {
            Clear();
            m_nMaxDepth = nMaxDepth;

            m_menuUndo = mUndo;
            m_menuRedo = mRedo;
        }
        #endregion
    }
    #endregion

    // Join / Split Paragraphs ---------------------------------------------------------------
    #region CLASS: SplitJoinParagraphAction : Action
    public class SplitJoinParagraphAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_BeforeSplit
        protected OWBookmark Bookmark_BeforeSplit
        {
            get
            {
                Debug.Assert(null != m_bookmark_BeforeSplit);
                return m_bookmark_BeforeSplit;
            }
        }
        OWBookmark m_bookmark_BeforeSplit;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterSplit
        protected OWBookmark Bookmark_AfterSplit
        {
            get
            {
                Debug.Assert(null != m_bookmark_AfterSplit);
                return m_bookmark_AfterSplit;
            }
        }
        OWBookmark m_bookmark_AfterSplit;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_BeforeJoin
        protected OWBookmark Bookmark_BeforeJoin
        {
            get
            {
                Debug.Assert(null != m_bookmark_BeforeJoin);
                return m_bookmark_BeforeJoin;
            }
        }
        OWBookmark m_bookmark_BeforeJoin;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterJoin
        protected OWBookmark Bookmark_AfterJoin
        {
            get
            {
                Debug.Assert(null != m_bookmark_AfterJoin);
                return m_bookmark_AfterJoin;
            }
        }
        OWBookmark m_bookmark_AfterJoin;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected SplitJoinParagraphAction(string sDisplayName, OWWindow window)
            : base(sDisplayName)
        {
            m_Window = window;
        }
        #endregion

        // Split and Join commands -----------------------------------------------------------
        #region Method: bool Split()
        protected bool Split()
        {
            if (Window.HandleLockedFromEditing())
                return false;

            // Bookmark the "Before" selection so we can Undo back to it
            OWWindow.Sel selection = Window.Selection;
            m_bookmark_BeforeSplit = new OWBookmark(selection);

            // If we have a selection, we don't want to delete the text. While deleting it  
            // is the typical Microsoft Word behavior, I fear it would be unsettling to the
            //  MTT. So instead, we move to the end of the selection.
            if (selection.IsContentSelection)
            {
                selection = OWWindow.Sel.CreateSel(
                    selection.Paragraph,
                    selection.DBT,
                    selection.DBT_iCharLast);
            }

            // Get the position where the split will happen
            DBasicText text = selection.DBT;
            int iPos = selection.DBT_iChar(selection.Anchor);

            // Retrieve the underlying paragraph
            DParagraph para = text.Paragraph;
            if (null == para)
                return false;

            // Remember the cursor position so that we can restore back to it....Split()
            // may remove some of the underlying data, so we need the bookmark to restore
            // a valid selection.
            Window.PushEditState();

            // Split the underlying paragraph
            DParagraph paraNew = para.Split(text, iPos);
            if (null == paraNew)
                return false;

            // Reload the window's data. This is time-consuming, but it is the only way to make 
            // paragraphs line up correctly side-by-side.
            Window.LoadData();

            // Restore the selection insertion point to the Window.Selection
            Window.PopEditState();

            // This has us at the end of the first paragraph, we need to move to the
            // beginning of the next one
            Window.cmdMoveCharRight();

            // Remember this position in case we do a future Undo
            m_bookmark_AfterSplit = new OWBookmark(Window.Selection);

            return true;
        }
        #endregion
        #region Method: bool JoinToPrevious()
        protected bool JoinToPrevious()
        {
            // Bookmark the "Before" selection so we can Undo back to it
            OWWindow.Sel selection = Window.Selection;
            m_bookmark_BeforeJoin = new OWBookmark(selection);

            // Retrieve the underlying paragraph
            DParagraph p = selection.Paragraph.DataSource as DParagraph;
            if (null == p)
                return false;

            // Retrieve the paragraph previous to it
            int i = p.Section.Paragraphs.FindObj(p) - 1;
            if (i < 0)
                return false;
            p = p.Section.Paragraphs[i] as DParagraph;

            // We need to move the Window selection left, so that we have something valid to bookmark
            Window.cmdMoveCharLeft();
            OWBookmark bm = new OWBookmark(Window.Selection);

            // Join the paragraphs in the underlying data
            p.JoinToNext();

            // Re-Load the window's data. This is time-consuming, but it is the only way to
            // make the paragraphs line correctly side-by-side.
            Window.LoadData();

            // Restore the bookmark now that the paragraphs have changed
            bm.RestoreWindowSelectionAndScrollPosition();

            // Remember this position in case we do a future Undo
            m_bookmark_AfterJoin = bm;

            return true;
        }
        #endregion
    }
    #endregion
    #region CLASS: SplitParagraphAction : SplitJoinParagraphAction
    public class SplitParagraphAction : SplitJoinParagraphAction
    {
        #region Constructor(OWWindow)
        public SplitParagraphAction(OWWindow window)
            : base("Split Paragraph", window)
        {
        }
        #endregion
        #region OMethod: bool Do() - Perform the Split action
        public override bool Do()
        {
            if (Split())
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Don't assume that we're in the correct place
            Bookmark_AfterSplit.RestoreWindowSelectionAndScrollPosition();

            // Do the Join
            JoinToPrevious();

            // Restore to the original pre-Undo bookmark
            Bookmark_BeforeSplit.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Don't assume that we're in the correct place
            Bookmark_BeforeSplit.RestoreWindowSelectionAndScrollPosition();

            // Split the paragraph
            Split();
        }
        #endregion
    }
    #endregion
    #region CLASS: JoinParagraphAction : SplitJoinParagraphAction
    public class JoinParagraphAction : SplitJoinParagraphAction
    {
        #region Constructor(OWWindow)
        public JoinParagraphAction(OWWindow window)
            : base("Join Paragraph", window)
        {
        }
        #endregion
        #region OMethod: bool Do() - Perform the Join action
        public override bool Do()
        {
            if (JoinToPrevious())
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Don't assume that we're in the correct place
            Bookmark_AfterJoin.RestoreWindowSelectionAndScrollPosition();

            // Split the paragraph
            Split();

            // Restore to the original pre-Undo bookmark
            Bookmark_BeforeJoin.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Don't assume that we're in the correct place
            Bookmark_BeforeJoin.RestoreWindowSelectionAndScrollPosition();

            // Re-Joining the paragraph
            JoinToPrevious();
        }
        #endregion
    }
    #endregion

    // Paragraph Style -----------------------------------------------------------------------
    #region CLASS: ChangeParagraphStyleAction : Action
    public class ChangeParagraphStyleAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        #region Attr{g}: string RequestedStyleAbbrev
        string RequestedStyleAbbrev
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sRequestedStyleAbbrev));
                return m_sRequestedStyleAbbrev;
            }
        }
        string m_sRequestedStyleAbbrev;
        #endregion
        #region Attr{g}: string OriginalStyleAbbrev
        string OriginalStyleAbbrev
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sOriginalStyleAbbrev));
                return m_sOriginalStyleAbbrev;
            }
        }
        string m_sOriginalStyleAbbrev;
        #endregion
        #region Attr{g}: OWBookmark Bookmark
        protected OWBookmark Bookmark
        {
            get
            {
                Debug.Assert(null != m_Bookmark);
                return m_Bookmark;
            }
        }
        OWBookmark m_Bookmark;
        #endregion

        // Scaffolding ----------------------------------------------------------------------
        #region Constructor(OWWindow)
        public ChangeParagraphStyleAction(OWWindow window, string sRequestedStyleAbbrev)
            : base("Change Paragraph Style")
        {
            m_Window = window;
            m_sRequestedStyleAbbrev = sRequestedStyleAbbrev;
        }
        #endregion

        // Helper methods --------------------------------------------------------------------
        #region Helper: bool _IsValidRequest(DParagraph p)
        bool _IsValidRequest(DParagraph p)
        {
            // If we're requesting a Section Title.... 
            if (RequestedStyleAbbrev == DStyleSheet.c_StyleSectionTitle)
            {
                // ...there must not already be a section title
                if (p.Section.CountParagraphsWithStyle(DStyleSheet.c_StyleSectionTitle) > 0)
                {
                    LocDB.Message("msgSectionTitleAlreadyExists",
                        "You cannot change this paragraph to a Section Title, because a Section Title " +
                            "already exists in this section.",
                        null,
                        LocDB.MessageTypes.Info);
                    return false;
                }
                // ...there must not be any Verses, Chapters or Footnotes in it
                if (p.StructureCodes.Length > 0)
                {
                    LocDB.Message("msgSectionTitleCannotHaveVerses",
                        "You cannot change this paragraph to a Section Title, because a Section Title " +
                            "cannot have verses, chapters or footnotes in it.",
                        null,
                        LocDB.MessageTypes.Info);
                    return false;
                }
            }

            return true;
        }
        #endregion
        #region Helper: void _ChangeStyle(DParagraph p, string sNewStyleAbbrev)
        OWBookmark _ChangeStyle(DParagraph p, string sNewStyleAbbrev)
        {
            // Remember the cursor position so that we can restore back to it after the re-LoadData.
            OWBookmark bm = new OWBookmark(Window.Selection);

            // Change the underlying paragraph's style
            p.StyleAbbrev = sNewStyleAbbrev;

            // Re-Load the window's data. This is time-consuming, but it is the only way to make
            // sure the paragraphs line correctly side-by-side.
            Window.LoadData();

            // Restore the selection insertion point
            bm.RestoreWindowSelectionAndScrollPosition();

            return bm;
        }
        #endregion

        // Action Overrides ------------------------------------------------------------------
        #region OMethod: bool Do()
        public override bool Do()
        {
            // Retrieve the underlying paragraph
            DParagraph p = Window.Selection.Paragraph.DataSource as DParagraph;
            if (null == p)
                return false;

            // Nothing to do if this is already the requested style
            if (p.StyleAbbrev == RequestedStyleAbbrev)
                return false;

            // Make sure it is valid to assign this style to this paragraph
            if (!_IsValidRequest(p))
                return false;

            // Remember the former style so we can undo it
            m_sOriginalStyleAbbrev = p.StyleAbbrev;

            // Perform the change
            m_Bookmark = _ChangeStyle(p, RequestedStyleAbbrev);

            // We performed the action, so add it to the stack
            Push();
            return true;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Restore the selection to the correct paragraph
            Bookmark.RestoreWindowSelectionAndScrollPosition();

            // Retrieve the paragraph
            DParagraph p = Window.Selection.Paragraph.DataSource as DParagraph;

            // Change back to the original style
            _ChangeStyle(p, OriginalStyleAbbrev);

        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Restore the selection to the correct paragraph
            Bookmark.RestoreWindowSelectionAndScrollPosition();

            // Retrieve the paragraph
            DParagraph p = Window.Selection.Paragraph.DataSource as DParagraph;

            // Change back to the new style
            _ChangeStyle(p, RequestedStyleAbbrev);
        }
        #endregion
        #region OMethod: string Contents
        public override string Contents
        {
            get
            {
                return G.StyleSheet.FindParagraphStyle(OriginalStyleAbbrev).DisplayName;
            }
        }
        #endregion

    }
    #endregion

    // Deletion ------------------------------------------------------------------------------
    #region CLASS: DeleteAction : Action
    public class DeleteAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        public enum DeleteMode { kDelete, kCut, kCopy, kBackSpace, kNone };
        #region Attr{g}: DeleteMode Mode
        protected DeleteMode Mode
        {
            get
            {
                Debug.Assert(m_Mode != DeleteMode.kNone);
                return m_Mode;
            }
        }
        protected DeleteMode m_Mode = DeleteMode.kNone;
        #endregion
        #region Attr{g}: DBasicText DBT_CopyOfOriginal
        DBasicText DBT_CopyOfOriginal
        {
            get
            {
                Debug.Assert(null != m_dbtCopyOfOriginal);
                return m_dbtCopyOfOriginal;
            }
        }
        DBasicText m_dbtCopyOfOriginal;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_BeforeDelete
        protected OWBookmark Bookmark_BeforeDelete
        {
            get
            {
                Debug.Assert(null != m_bookmark_BeforeDelete);
                return m_bookmark_BeforeDelete;
            }
        }
        OWBookmark m_bookmark_BeforeDelete;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterDelete
        protected OWBookmark Bookmark_AfterDelete
        {
            get
            {
                Debug.Assert(null != m_bookmark_AfterDelete);
                return m_bookmark_AfterDelete;
            }
        }
        OWBookmark m_bookmark_AfterDelete;
        #endregion
        #region Attr{g}: string DeletedText
        string DeletedText
        {
            get
            {
                return m_sDeletedText;
            }
        }
        string m_sDeletedText;
        #endregion

        // Clipboard -------------------------------------------------------------------------
        string m_sClipboardText = null;
        #region Method: void PushClipboard() - saves the clipboard test so it can be restored
        void PushClipboard()
        {
            m_sClipboardText = null;
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                m_sClipboardText = Clipboard.GetText(TextDataFormat.UnicodeText);
        }
        #endregion
        #region Method: void PopClipboard() - restores formerly pushed clipboard text
        void PopClipboard()
        {
            if (null != m_sClipboardText)
                Clipboard.SetText(m_sClipboardText, TextDataFormat.UnicodeText);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow, DeleteMode)
        public DeleteAction(OWWindow window, DeleteMode mode)
            : base(
                (mode == DeleteMode.kDelete) ? "Delete" : 
                (mode == DeleteMode.kCut) ? "Cut" :
                (mode == DeleteMode.kCopy) ? "Copy" :
                (mode == DeleteMode.kBackSpace) ? "Backspace" :
                "Other")
        {
            m_Window = window;
            m_Mode = mode;
        }
        #endregion

        // Delete Command --------------------------------------------------------------------
        #region Method: bool Delete()
        public bool Delete(bool bCleanUpDoubleSpaces)
        {
            // Shorthand
            OWWindow.Sel selection = Window.Selection;
            if (null == selection)
                return false;

            // Retrieve the selected paragraph
            OWPara op = Window.Selection.Paragraph;
            if (null == op)
                return false;

            // Do nothing if the paragraph is uneditable
            if (!op.IsEditable)
                return false;

            // Do nothing if an Insertion Icon
            if (selection.IsInsertionIcon)
                return false;

            // Save a bookmark for where we are prior to the deletion
            m_bookmark_BeforeDelete = new OWBookmark(selection);

            // If an InsertionPoint, extend Right/Left if kDelete/kBackspace. We need a selection
            // with a beginning and ending, so that we have something we can actually delete.
            if (selection.IsInsertionPoint)
            {
                if (Mode == DeleteMode.kDelete)
                    selection = op.ExtendSelection_CharRight(selection);
                if (Mode == DeleteMode.kBackSpace)
                    selection = op.ExtendSelection_CharLeft(selection);
            }

            // If still an InsertionPoint, we can't delete; we're at the end of a paragraph or
            // some other illegal spot; so we teach the user with a Beep.
            if (selection.IsInsertionPoint)
            {
                OWWindow.TypingErrorBeep();
                return false;
            }

            // Retrieve the DBasicText and phrases we'll delete
            DBasicText DBT = selection.DBT;
            DBasicText.DPhrases<DPhrase> phrases = (op.DisplayBT) ? DBT.PhrasesBT : DBT.Phrases;

            // We'll make a copy of the data for Undo; note this is only a copy of the DBasicText,
            // not the DText, so when we do the Undo, we must copy the data back to the
            // original object, not replace it.
            m_dbtCopyOfOriginal = new DBasicText(DBT);

            // Determine the position to delete, and the number of characters
            int iStart = selection.DBT_iCharFirst;
            int iCount = selection.DBT_iCharLast - iStart;

            // Save what we're about to delete, so we can put it in the Undo/Redo menus
             m_sDeletedText = phrases.AsString.Substring(iStart, iCount);

            // Clipboard operations. With kCopy, we don't proceed to the actual deletion code
            if (Mode == DeleteMode.kCut || Mode == DeleteMode.kCopy)
            {
                PushClipboard();
                Clipboard.SetText(DeletedText, TextDataFormat.UnicodeText);
                if (Mode == DeleteMode.kCopy)
                {
                    m_bookmark_AfterDelete = new OWBookmark(Window.Selection);
                    return true;
                }
            }

            // Do the deletion to the underlying DParagraph data
            phrases.Delete(iStart, iCount, bCleanUpDoubleSpaces);

            // Update the OWPara
            op.ReplaceBlocksWithNewDBT(selection, DBT);

            // Restore a selection at the deletion point
            selection = OWWindow.Sel.CreateSel(op, DBT, iStart);
            Window.Selection = op.NormalizeSelection(selection);

            // Save a bookmark for the result of the deletion
            m_bookmark_AfterDelete = new OWBookmark(Window.Selection);

            // Successful
            return true;
        }
        #endregion

        // Action Overrides ------------------------------------------------------------------
        #region OMethod: bool Do()
        public override bool Do()
        {
            if (Delete(true))
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Restore to the selection following the deletion, so we can get the right data
            OWWindow.Sel selection = Bookmark_AfterDelete.CreateSelection();

            // If a Cut or Copy command, then restore the clipboard
            if (Mode == DeleteMode.kCut || Mode == DeleteMode.kCopy)
            {
                PopClipboard();
                if (Mode == DeleteMode.kCopy)
                {
                    Bookmark_BeforeDelete.RestoreWindowSelectionAndScrollPosition();
                    return;
                }
            }

            // Retrieve the DBasicText we worked with previously
            DBasicText DBT = selection.DBT;

            // Restore its former data
            DBT_CopyOfOriginal.CopyDataTo(DBT);

            // Update the OWPara
            selection.Paragraph.ReplaceBlocksWithNewDBT(selection, DBT);

            // Restore to the selection prior to the deletion
            Bookmark_BeforeDelete.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Restore to the selection prior to the deletion
            Bookmark_BeforeDelete.RestoreWindowSelectionAndScrollPosition();

            // Do the deletion
            Delete(true);
        }
        #endregion
        #region OAttr{g}: string Contents
        public override string Contents
        {
            get
            {
                return DeletedText;
            }
        }
        #endregion
    }
    #endregion

    // Insertions ----------------------------------------------------------------------------
    #region CLASS: InsertAction : Action
    public class InsertAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        #region Attr{g/s}: OWBookmark Bookmark_BeforeInsert
        protected OWBookmark Bookmark_BeforeInsert
        {
            get
            {
                Debug.Assert(null != m_bookmark_BeforeInsert);
                return m_bookmark_BeforeInsert;
            }
            set
            {
                m_bookmark_BeforeInsert = value;
            }
        }
        OWBookmark m_bookmark_BeforeInsert;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterInsert
        protected OWBookmark Bookmark_AfterInsert
        {
            get
            {
                Debug.Assert(null != m_bookmark_AfterInsert);
                return m_bookmark_AfterInsert;
            }
        }
        OWBookmark m_bookmark_AfterInsert;
        #endregion
        #region Attr{g/s}: string TextToInsert
        protected string TextToInsert
        {
            get
            {
                return m_sTextToInsert;
            }
            set
            {
                m_sTextToInsert = value;
            }
        }
        string m_sTextToInsert;
        #endregion
        #region Attr{g}: DBasicText DBT_CopyOfOriginal
        protected DBasicText DBT_CopyOfOriginal
        {
            get
            {
                Debug.Assert(null != m_dbtCopyOfOriginal);
                return m_dbtCopyOfOriginal;
            }
        }
        DBasicText m_dbtCopyOfOriginal;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(string sDisplayName, OWWindow, sTextToInsert)
        public InsertAction(string sDisplayName, OWWindow window, string sTextToInsert)
            : base(sDisplayName)
        {
            m_Window = window;
            m_sTextToInsert = sTextToInsert;
        }
        #endregion
        #region Constructor(OWWindow, chCharToInsert)
        public InsertAction(OWWindow window, char chCharToInsert)
            : base("Typing")
        {
            m_Window = window;
            m_sTextToInsert = chCharToInsert.ToString();
        }
        #endregion

        // Insert ----------------------------------------------------------------------------
        #region Method:bool Insert(string sTextToInsert)
        protected bool Insert(string sInsert)
        {
            // Retrieve the selected paragraph
            OWPara op = Window.Selection.Paragraph;
            if (null == op)
                return false;

            // Do nothing if the paragraph is uneditable
            if (!op.IsEditable)
                return false;

            // Save a bookmark for where we are prior to the insertion
            m_bookmark_BeforeInsert = new OWBookmark(Window.Selection);

            // If we have an InsertionIcon, replace it with a space, so that the normal
            // mechanism can deal with it (deleting the space, then doing the insertion.)
            if (Window.Selection.IsInsertionIcon)
            {
                Window.Selection.Anchor.Word.Text = " ";
                Window.Selection = new OWWindow.Sel(Window.Selection.Paragraph,
                    Window.Selection.Anchor);
            }

            // We'll make a copy of the data for Undo; note this is only a copy of the DBasicText,
            // not the DText, so when we do the Undo, we must copy the data back to the
            // original object, not replace it.
            m_dbtCopyOfOriginal = new DBasicText(Window.Selection.DBT);

            // If we have a selection, we must delete the data that is there. This is
            // not a deletion that we want to push onto the Undo stack, so we call
            // Delete rather than Do.
            if (Window.Selection.IsContentSelection)
            {
                (new DeleteAction(Window, DeleteAction.DeleteMode.kDelete)).Delete(false);
            }

            // Get the iBlocks that correspond to the DBasicText
            OWWindow.Sel sel = Window.Selection;
            int iBlockFirst = sel.DBT_iBlockFirst;

            // Get the offset into the DBasicText
            int n = Window.Selection.DBT_iCharFirst;

            // Retrieve which phrase we'll be working on (Vernacular or Back Translation)
            DBasicText DBT = sel.DBT;
            DBasicText.DPhrases<DPhrase> phrases = (op.DisplayBT) ? DBT.PhrasesBT : DBT.Phrases;

            // We'll keep track of which DPhrase we're currently processing here
            int iPhrase = 0;

            // Increment past any DPhrases that are prior to the insertion point
            DPhrase phr = null;
            while (true)
            {
                phr = phrases[iPhrase];

                // We will do the insert in this phrase. (Note that we use '>' rather than
                // '>=', because at the phrase boundary, this moves us to the next phrase,
                // rather than attempting to append at the end of a phrase; which gives
                // unwanted (i.e., wierd) typing behavior.
                if (phr.Text.Length > n)
                    break;

                // If we're at the last phrase, then we're done
                if (iPhrase == phrases.Count - 1)
                    break;


                n -= phr.Text.Length;
                iPhrase++;
            }

            // We now have the DPhrase insertion point
            DPhrase phrase = phrases[iPhrase];
            int iPos = n;

            // Insert the text. Note that we remove spurious spaces, so if the result of the
            // insertion is that we have what we started with, then we need proceed no further.
            string sBeforeInsert = phrase.Text;
            phrase.Text = DPhrase.EliminateSpuriousSpaces(phrase.Text.Insert(n, sInsert));
            if (sBeforeInsert == phrase.Text)
            {
                // The test is for a cursor right before a space, e.g., "Hello| World." In this case,
                // we want the cursor to advance, as if the user hit right arrow, thus "Hello |World."
                // So for all other cases, we return here and do nothing, but in this one special
                // case, we want to continue through the method so that the cursor advances.
                if (!(n < phrase.Text.Length && phrase.Text[n] == ' ' && sInsert == " "))
                    goto done;
            }

            // Remove the old blocks and replace with new ones
            op.ReplaceBlocksWithNewDBT(Window.Selection, DBT);

            // Restore a selection at the deletion point plus the amount inserted. 
            int iInsertPos = sel.DBT_iCharFirst + sInsert.Length;
            sel = OWWindow.Sel.CreateSel(op, sel.DBT, iInsertPos);
            Window.Selection = op.NormalizeSelection(sel);

            // Save a bookmark for the result of the deletion
        done:
            m_bookmark_AfterInsert = new OWBookmark(Window.Selection);
            return true;
        }
        #endregion

        // Action Overrides ------------------------------------------------------------------
        #region OMethod: bool Do()
        public override bool Do()
        {
            if (Insert(TextToInsert))
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Restore to the selection following the deletion, so we can get the right data
            OWWindow.Sel selection = Bookmark_AfterInsert.CreateSelection();

            // Retrieve the DBasicText we worked with previously
            DBasicText DBT = selection.DBT;

            // Restore its former data
            DBT_CopyOfOriginal.CopyDataTo(DBT);

            // Update the OWPara
            selection.Paragraph.ReplaceBlocksWithNewDBT(selection, DBT);

            // Restore to the selection prior to the deletion
            Bookmark_BeforeInsert.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Restore to the selection prior to the deletion
            Bookmark_BeforeInsert.RestoreWindowSelectionAndScrollPosition();

            // Do the insertion
            Insert(TextToInsert);
        }
        #endregion
    }
    #endregion
    #region CLASS: TypingAction : InsertAction
    public class TypingAction : InsertAction
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: char chKey - the key that was typed
        char chKey
        {
            get
            {
                return m_chKey;
            }
        }
        char m_chKey;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterAutoReplace
        protected OWBookmark Bookmark_AfterAutoReplace
        {
            get
            {
                Debug.Assert(null != m_bookmark_AutoReplace);
                return m_bookmark_AutoReplace;
            }
        }
        OWBookmark m_bookmark_AutoReplace;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow, chKey)
        public TypingAction(OWWindow window, char _chKey)
            : base("Typing", window, "")
        {
            m_chKey = _chKey;
            m_vAdjacentTyping = new TypingAction[0];
            m_Owner = this;
        }
        #endregion

        // AutoReplace -----------------------------------------------------------------------
        #region Method: string ProcessAutoReplace()
        string ProcessAutoReplace()
        {
            // Shorthand
            OWWindow.Sel selection = Window.Selection;
            if (null == selection)
                return null;
            OWPara op = selection.Paragraph;
            JWritingSystem jws = op.WritingSystem;

            // Edits not permitted here
            if (Window.HandleLockedFromEditing())
                return null;

            // An Insertion Icon is simple, we just check for the chKey, and insert it
            // if a match
            if (selection.IsInsertionIcon)
            {
                int c = 0;
                string sInsert = jws.SearchAutoReplace(chKey.ToString(), ref c);
                if (null == sInsert || sInsert.Length == 0)
                    return null;
                return sInsert;
            }

            // We are considering a content selection to be meaningless; thus from
            // here we are only interested in an Insertion Point.
            if (!selection.IsInsertionPoint)
                return null;

            // Retrieve the string in this DBasicText up to the selection point
            string sDBT = (selection.Paragraph.DisplayBT) ?
                selection.DBT.ProseBTAsString :
                selection.DBT.ContentsAsString;
            string sSource = sDBT.Substring(0, selection.DBT_iCharFirst);

            // Append what was just typed
            sSource += chKey;

            // Check for a match
            int cSelectionCount = 0;
            string sReplace = jws.SearchAutoReplace(sSource, ref cSelectionCount);
            if (null == sReplace || sReplace.Length == 0)
                return null;
            Debug.Assert(cSelectionCount > 0);

            // The number of chars to select is 1 less than what SearchAutoReplace
            // returns, because the return value included the key that was just typed.
            cSelectionCount--;

            // Make a selection so that we can delete the appropriate number of characters
            if (cSelectionCount > 0)
            {
                int i2 = selection.DBT_iCharFirst;
                int i1 = i2 - cSelectionCount;
                OWWindow.Sel sel = OWWindow.Sel.CreateSel(op, selection.DBT, i1, i2);
                if (null == sel)
                    return null;
                Window.Selection = sel;
            }

            return sReplace;
        }
        #endregion

        // Combine Actions -------------------------------------------------------------------
        //    We have a sequence of daughter actions, and combining means placing actions into
        // this sequence. Thus the Contents attr will be a sum through all of these actions.
        // Undo/Redo likewise work their way through the entire sequence, one-at-a-time.
        #region Attr{g}: TypingAction[] AdjacentTyping
        TypingAction[] AdjacentTyping
        {
            get
            {
                Debug.Assert(null != m_vAdjacentTyping);
                return m_vAdjacentTyping;
            }
        }
        TypingAction[] m_vAdjacentTyping;
        #endregion
        #region Attr{g}: TypingAction Owner
        TypingAction Owner
        {
            get
            {
                Debug.Assert(null != m_Owner);
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
        TypingAction m_Owner;
        #endregion
        #region Attr{g}: bool IsOwner
        bool IsOwner
        {
            get
            {
                return (Owner == this);
            }
        }
        #endregion
        #region Attr{g}: void AddAdjacentTypingAction(TypingAction)
        void AddAdjacentTypingAction(TypingAction action)
        {
            // Set the owning action attr so the action know who owns it
            action.Owner = this;

            // Add it to the array
            TypingAction[] v = new TypingAction[AdjacentTyping.Length + 1];
            for (int i = 0; i < AdjacentTyping.Length; i++)
                v[i] = AdjacentTyping[i];
            v[AdjacentTyping.Length] = action;
            m_vAdjacentTyping = v;

            // Update the stack's menus
            G.URStack.SetMenuText();
        }
        #endregion
        #region Method: bool CombineActions()
        bool CombineActions()
        {
            // Get the previous action, if any
            Action action = G.URStack.PeekUndo;
            if (null == action)
                return false;

            // See if it was a typing action
            TypingAction PreviousTypingAction = action as TypingAction;
            if (null == PreviousTypingAction)
                return false;

            // See if it has a daughter
            int len = PreviousTypingAction.AdjacentTyping.Length;
            if (len > 0)
                PreviousTypingAction = PreviousTypingAction.AdjacentTyping[len - 1];

            // Both actions, going in, needed to have been done on an InsertionPoint, not
            // on a content selection (which would have involved deletion.) A content
            // deletion would be confusing to the user if it were combined.
            if (!PreviousTypingAction.Bookmark_BeforeInsert.IsInsertionPoint)
                return false;
            if (!Bookmark_BeforeInsert.IsInsertionPoint)
                return false;

            // If the end of the last action is the same bookmark as the beginning of this
            // action, then we want to combine.
            OWBookmark bmPrevious = PreviousTypingAction.Bookmark_AfterInsert;
            OWBookmark bmCurrent = Bookmark_BeforeInsert;
            if (!bmPrevious.IsAdjacentTo(bmCurrent))
                return false;

            // If we are here, then we're qualified to do the combine
            PreviousTypingAction.Owner.AddAdjacentTypingAction(this);
            return true;
        }
        #endregion

        // Action Overrides ------------------------------------------------------------------
        // The superclass Undo works because it returns to the state prior to the first 
        // action (thus subsequent Adjacent Typing actions are undone automatically.)
        #region OMethod: bool Do()
        public override bool Do()
        {
            // Do nothing for Control keys
            if (Char.IsControl(chKey))
                return false;

            // We Need to copy the "Before" state, because the Insert process will
            // do something mid-process (after the AutoReplace); and we really need the
            // start Going In.
            OWBookmark bm = new OWBookmark(Window.Selection);

            // Get the AutoReplace text
            TextToInsert = ProcessAutoReplace();
            m_bookmark_AutoReplace = new OWBookmark(Window.Selection);

            // If it returned nothing, then we just want to insert the key that was pressed
            if (string.IsNullOrEmpty(TextToInsert))
                TextToInsert = chKey.ToString();

            // Attempt the insertion
            bool bSuccessful = Insert(TextToInsert);

            // Reset the state that we copied so that Undo/Redo works. We want the selected
            // state prior to the AutoReplace, rather than after it as would happen if we
            // didn't do this.
            Bookmark_BeforeInsert = bm;

            // If successful, push this on the stack
            if (bSuccessful)
            {
                if (!CombineActions())
                    Push();
                return true;
            }

            return false;
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Restore the selection to the AutoReplace result, because this selects the
            // text that needs to be deleted.
            Bookmark_AfterAutoReplace.RestoreWindowSelectionAndScrollPosition();

            // Do the insertion
            Insert(TextToInsert);

            // Do the adjacent actions
            foreach (TypingAction action in AdjacentTyping)
                action.Redo();
        }
        #endregion
        #region OAttr{g}: string Contents
        public override string Contents
        {
            get
            {
                string s = chKey.ToString();

                if (!IsOwner)
                    return s;

                foreach (TypingAction a in AdjacentTyping)
                    s += a.Contents;

                return s;
            }
        }
        #endregion
    }
    #endregion
    #region CLASS: PasteAction : InsertAction
    public class PasteAction : InsertAction
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow)
        public PasteAction(OWWindow window)
            : base("Paste", window, "")
        {
        }
        #endregion

        // Clipboard -------------------------------------------------------------------------
        #region Attr{g}: string ClipboardText
        string ClipboardText 
        {
            get
            {
                return m_sClipboardText;
            }
        }
        string m_sClipboardText = null;
        #endregion
        #region Method: string ProcessTextFromClipboard()
        string ProcessTextFromClipboard()
        {
            if (Window.HandleLockedFromEditing())
                return null;

            // Retrieve the clipboard's text
            if (!Clipboard.ContainsData(DataFormats.UnicodeText))
                return null;
            m_sClipboardText = Clipboard.GetData(DataFormats.UnicodeText) as string;
            if (string.IsNullOrEmpty(m_sClipboardText))
                return null;

            // Don't insert if too big; we figure the user has the wrong stuff on the clipboard!
            if (m_sClipboardText.Length > 500)
                return null;

            // Remove the characters we don't like
            string sInsert = "";
            char chPrev = '\0';
            foreach (char ch in m_sClipboardText)
            {
                // Avoid inserting, e.g., line feeds, carriage returns
                if (char.IsControl(ch))
                    continue;

                // Try to avoid inserting multiple white spaces
                if (char.IsWhiteSpace(ch))
                {
                    if (char.IsWhiteSpace(chPrev))
                        continue;
                }

                // If we're here, we've approved the character. 
                sInsert += ch;

                // Want to rememebr the previous one for the Whitespace test
                chPrev = ch;
            }

            return sInsert;
        }
        #endregion

        // Action Overrides ------------------------------------------------------------------
        #region OMethod: bool Do()
        public override bool Do()
        {
            TextToInsert = ProcessTextFromClipboard();
            if (Insert(TextToInsert))
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OAttr{g}: string Contents
        public override string Contents
        {
            get
            {
                return ClipboardText;
            }
        }
        #endregion
    }
    #endregion

    // Italics -------------------------------------------------------------------------------
    #region CLASS: ItalicsAction
    public class ItalicsAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        #region Attr{g}: DBasicText DBT_CopyOfOriginal
        DBasicText DBT_CopyOfOriginal
        {
            get
            {
                Debug.Assert(null != m_dbtCopyOfOriginal);
                return m_dbtCopyOfOriginal;
            }
        }
        DBasicText m_dbtCopyOfOriginal;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_Italics
        protected OWBookmark Bookmark_Italics
        {
            get
            {
                Debug.Assert(null != m_bookmark_Italics);
                return m_bookmark_Italics;
            }
        }
        OWBookmark m_bookmark_Italics;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sDisplayName, OWWindow)
        public ItalicsAction(OWWindow window)
            : base("Italics")
        {
            m_Window = window;
        }
        #endregion

        // Italics ---------------------------------------------------------------------------
        #region Method: bool Italics()
        bool Italics()
        {
            // Retrieve the selected paragraph
            OWPara op = Window.Selection.Paragraph;
            if (null == op)
                return false;

            // Do nothing if the paragraph is uneditable
            if (!op.IsEditable)
                return false;

            // There's nothing to do if we don't have a content selection
            if (!Window.Selection.IsContentSelection)
                return false;

            // Save a bookmark for where we are prior to the italics
            m_bookmark_Italics = new OWBookmark(Window.Selection);

            // Retrieve which phrase we'll be working on (Vernacular or Back Translation)
            DBasicText DBT = Window.Selection.DBT;
            DBasicText.DPhrases<DPhrase> phrases = (op.DisplayBT) ? DBT.PhrasesBT : DBT.Phrases;

            // We'll make a copy of the data for Undo; note this is only a copy of the DBasicText,
            // not the DText, so when we do the Undo, we must copy the data back to the
            // original object, not replace it.
            m_dbtCopyOfOriginal = new DBasicText(Window.Selection.DBT);

            // Have the DBT do the work
            phrases.ToggleItalics(
                Window.Selection.DBT_iCharFirst,
                Window.Selection.DBT_iCharCount);

            // Remove the old blocks and replace with new ones
            op.ReplaceBlocksWithNewDBT(Window.Selection, DBT);

            // Restore the selection, as it doesn't change
            Bookmark_Italics.RestoreWindowSelectionAndScrollPosition();
            return true;
        }
        #endregion

        // Action Overrides ------------------------------------------------------------------
        #region OMethod: bool Do()
        public override bool Do()
        {
            if (Italics())
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Restore to the selection following the italics, so we can get the right data
            OWWindow.Sel selection = Bookmark_Italics.CreateSelection();

            // Retrieve the DBasicText we worked with previously
            DBasicText DBT = selection.DBT;

            // Restore its former data
            DBT_CopyOfOriginal.CopyDataTo(DBT);

            // Update the OWPara
            selection.Paragraph.ReplaceBlocksWithNewDBT(selection, DBT);

            // Restore to the selection prior to the italics
            Bookmark_Italics.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Restore to the selection prior to the italics
            Bookmark_Italics.RestoreWindowSelectionAndScrollPosition();

            // Do the italics
            Italics();
        }
        #endregion
    }
    #endregion

    // Footnotes -----------------------------------------------------------------------------
    #region CLASS: InsertDeleteFootnoteAction : Action
    public class InsertDeleteFootnoteAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_BeforeInsert
        protected OWBookmark Bookmark_BeforeInsert
        {
            get
            {
                Debug.Assert(null != m_bookmark_BeforeInsert);
                return m_bookmark_BeforeInsert;
            }
        }
        OWBookmark m_bookmark_BeforeInsert;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterInsert
        protected OWBookmark Bookmark_AfterInsert
        {
            get
            {
                Debug.Assert(null != m_bookmark_AfterInsert);
                return m_bookmark_AfterInsert;
            }
        }
        OWBookmark m_bookmark_AfterInsert;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_BeforeDelete
        protected OWBookmark Bookmark_BeforeDelete
        {
            get
            {
                Debug.Assert(null != m_bookmark_BeforeDelete);
                return m_bookmark_BeforeDelete;
            }
        }
        OWBookmark m_bookmark_BeforeDelete;
        #endregion
        #region Attr{g}: OWBookmark Bookmark_AfterDelete
        protected OWBookmark Bookmark_AfterDelete
        {
            get
            {
                Debug.Assert(null != m_bookmark_AfterDelete);
                return m_bookmark_AfterDelete;
            }
        }
        OWBookmark m_bookmark_AfterDelete;
        #endregion

        // Attrs needed for undoing a footnote deletion --------------------------------------
        protected DFootnote m_CopyOfDeletedFootnote;
        protected OWBookmark m_bookmark_PositionOfFootnoteLetter;
        protected DFootnote m_InsertedFootnote;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected InsertDeleteFootnoteAction(string sDisplayName, OWWindow window)
            : base(sDisplayName)
        {
            m_Window = window;
        }
        #endregion

        // Insert and Delete Commands --------------------------------------------------------
        #region Method: bool InsertFootnote()
        public bool InsertFootnote()
        {
            if (Window.HandleLockedFromEditing())
                return false;

            // Bookmark the "Before" selection so we can Undo back to it
            OWWindow.Sel selection = Window.Selection;
            m_bookmark_BeforeInsert = new OWBookmark(selection);

            // If we have a selection, then move to the end of it. We'll be placing the new
            // footnote at the end of this selection
            if (selection.IsContentSelection)
            {
                selection = OWWindow.Sel.CreateSel(
                    selection.Paragraph,
                    selection.DBT,
                    selection.DBT_iCharLast);
            }

            // Get the position where the footnote will happen
            DBasicText text = selection.DBT;
            int iPos = selection.DBT_iChar(selection.Anchor);

            // Retrieve the underlying paragraph
            DParagraph para = text.Paragraph;
            if (null == para)
                return false;

            // Insert a DFootLetter into the paragraph
            DFootLetter footLetter = para.InsertFootnote(text, iPos);
            m_InsertedFootnote = footLetter.Footnote;

            // Reload the window's data. This is time-consuming, but it is the only way to make 
            // paragraphs line up correctly side-by-side.
            Window.LoadData();

            // Move editing to the new footnote
            DFootnote footnote = footLetter.Footnote;
            EContainer container = Window.Contents.FindContainerOfDataSource(footnote);
            container.Select_FirstWord();

            // Remember where we are so we can undo
            m_bookmark_AfterInsert = new OWBookmark(Window.Selection);

            return true;
        }
        #endregion
        #region Method: bool RemoveFootnote()
        public bool RemoveFootnote()
        {
            if (Window.HandleLockedFromEditing())
                return false;

            // Bookmark the "Before" selection so we can Undo back to it
            OWWindow.Sel selection = Window.Selection;
            m_bookmark_BeforeDelete = new OWBookmark(selection);

            // Retrieve the current footnote
            DFootnote footnote = selection.DBT.Paragraph as DFootnote;
            if (null == footnote)
                return false;

            // Prepare for possible future Undo
            // Move the cursor to its place in the text; make a note of the selection
            Window.Contents.OnSelectAndScrollFrom(footnote);
            m_bookmark_PositionOfFootnoteLetter = new OWBookmark(Window.Selection);
            // Make a copy of the footnote
            m_CopyOfDeletedFootnote = new DFootnote(footnote);
            m_CopyOfDeletedFootnote.CopyFrom(footnote, false);

            // Retrieve the paragraph and run that refers to it; and remove
            // the footnote
            bool bRemoved = false;
            foreach (DParagraph p in footnote.Section.Paragraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    DFootLetter letter = r as DFootLetter;
                    if (letter == null)
                        continue;

                    if (letter.Footnote != footnote)
                        continue;

                    footnote.Section.Footnotes.Remove(footnote);
                    p.RemoveFootnote(r as DFootLetter);
                    bRemoved = true;
                    break;
                }
                if (bRemoved)
                    break;
            }
            if (!bRemoved)
                return false;

            // Reload the window's data. This is time-consuming, but it is the only way to make 
            // paragraphs line up correctly side-by-side.
            Window.LoadData();

            // Select at the beginning of the window
            Window.Contents.Select_FirstWord();
            Window.Focus();

            // Remember where we are so we can undo
            m_bookmark_AfterDelete = new OWBookmark(Window.Selection);

            return true;
        }
        #endregion
    }
    #endregion
    #region CLASS: InsertFootnoteAction : InsertDeleteFootnoteAction
    public class InsertFootnoteAction : InsertDeleteFootnoteAction
    {
        #region Constructor(OWWindow)
        public InsertFootnoteAction(OWWindow window)
            : base("Insert Footnote", window)
        {
        }
        #endregion

        #region OMethod: bool Do() - Perform the Insert Footnote action
        public override bool Do()
        {
            if (InsertFootnote())
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Don't assume that we're in the correct place
            Bookmark_AfterInsert.RestoreWindowSelectionAndScrollPosition();

            // Delete the footnote
            RemoveFootnote();

            // Restore to the original pre-Undo bookmark
            Bookmark_BeforeInsert.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Don't assume that we're in the correct place
            Bookmark_BeforeInsert.RestoreWindowSelectionAndScrollPosition();

            // Insert the new footnote
            InsertFootnote();
        }
        #endregion
    }
    #endregion
    #region CLASS: DeleteFootnoteAction : InsertDeleteFootnoteAction
    public class DeleteFootnoteAction : InsertDeleteFootnoteAction
    {
        #region Constructor(OWWindow)
        public DeleteFootnoteAction(OWWindow window)
            : base("Delete Footnote", window)
        {
        }
        #endregion

        #region OMethod: bool Do() - Perform the Delete Footnote action
        public override bool Do()
        {
            if (RemoveFootnote())
            {
                Push();
                return true;
            }
            return false;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Place the selection to where the footnote should go
            m_bookmark_PositionOfFootnoteLetter.RestoreWindowSelectionAndScrollPosition();

            // Insert the footnote
            InsertFootnote();

            // Copy its original contents back in; requires that we re-load the data
            m_InsertedFootnote.CopyFrom(m_CopyOfDeletedFootnote, false);
            Window.LoadData();

            // Restore to the original pre-Undo bookmark
            Bookmark_BeforeDelete.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Don't assume that we're in the correct place
            Bookmark_BeforeDelete.RestoreWindowSelectionAndScrollPosition();

            // Delete the footnote
            RemoveFootnote();
        }
        #endregion
    }
    #endregion
}
