#region ***** UndoRedo.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    UndoRedo.cs
 * Author:  John Wimbish
 * Created: 01 Mar 2008
 * Purpose: Handles Undo / Redo actions
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
using System.Threading;
using System.Windows.Forms;

using JWTools;
using OurWordData;
using OurWordData.DataModel;
using OurWord.SideWnd;
#endregion
#endregion

namespace OurWord.Edit
{
    // General Undo/Redo Mechanism -----------------------------------------------------------
    #region CLASS: Action - Superclass for Do, Undo, Redo commands
    public class Action
    {
        #region Attr{g}: OWWindow Window
        protected OWWindow Window
        {
            get
            {
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
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

        #region Constructor(OWWindow, sDisplayName)
        public Action(OWWindow window, string sDisplayName)
        {
            Debug.Assert(null != window);
            m_Window = window;

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

            // If there's no action, then we just use the basic menu item
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

            // AddParagraph the new one
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
            // Add the new action to the Undo stack
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

        // Bookmark the stack, so we can remove just a portion of the Actions ----------------
        int m_nBookmarkPosition = -1;
        #region Method: void BookmarkStack()
        public void BookmarkStack()
        {
            // This causes ReDo stack to clear. Thus when we Bookmark the stack, we
            // should do so purposefully.
            _Clear(ref m_vRedoStack);

            m_nBookmarkPosition = UndoStack.Length;
        }
        #endregion
        #region Method: oid RestoreBookmarkedStack()
        public void RestoreBookmarkedStack()
        {
            Debug.Assert(m_nBookmarkPosition != -1);
            Debug.Assert(m_nBookmarkPosition <= m_vUndoStack.Length);

            _Clear(ref m_vRedoStack);

            while (m_vUndoStack.Length > m_nBookmarkPosition)
                _PopAction(ref m_vUndoStack);

            m_nBookmarkPosition = -1;

            SetMenuText();
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

            // AddParagraph it to the redo stack
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

            // AddParagraph it to the Undo stack
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

    // ToDo: If we can make BookmarkedAction the superclass for all, then we can
    // combine it with Action. Makes sense, to just have a Before and After
    // set of bookmarks. And simpler logic, to just have the two methods to override
    #region CLASS: BookmarkedAction
    public class BookmarkedAction : Action
    {
        #region Attr{g}: OWBookmark BmBefore
        protected OWBookmark BmBefore
        {
            get
            {
                Debug.Assert(null != m_BmBefore);
                return m_BmBefore;
            }
        }
        OWBookmark m_BmBefore;
        #endregion
        #region Attr{g}: OWBookmark BmAfter
        protected OWBookmark BmAfter
        {
            get
            {
                Debug.Assert(null != m_BmAfter);
                return m_BmAfter;
            }
        }
        OWBookmark m_BmAfter;
        #endregion

        #region Constructor(sDisplayName, OWWindow, TranslatorNote)
        public BookmarkedAction(OWWindow window, string sDisplayName)
            : base(window, sDisplayName)
        {
        }
        #endregion

        // Suclasses should override these two
        #region VMethod: bool PerformAction() 
        virtual protected bool PerformAction()
        {
            return false;
        }
        #endregion
        #region VMethod: void ReverseAction()
        virtual protected void ReverseAction()
        {
        }
        #endregion
        #region VMethod: void UndoFinishing()
        protected virtual void UndoFinishing()
        {
        }
        #endregion

        // Do, Undo, Redo
        #region OMethod: bool Do()
        public override bool Do()
        {
            // Bookmark the Before state so we can Undo back to it
            m_BmBefore = Window.CreateBookmark();

            // Do the action
            if (false == PerformAction())
                return false;

            // Remember where we are for undo purposes
            m_BmAfter = Window.CreateBookmark();

            // Place this action on the stack
            Push();
            return true;
        }
        #endregion
        #region OMethod: void Undo()
        public override void Undo()
        {
            // Don't assume that we're in the correct place
            BmAfter.RestoreWindowSelectionAndScrollPosition();

            // Undo the action
            ReverseAction();

            // Restore to the original pre-Undo bookmark
            BmBefore.RestoreWindowSelectionAndScrollPosition();

            // Hook for, e.g., changing focus to a different window
            UndoFinishing();
        }
        #endregion
        #region OMethod: void Redo()
        public override void Redo()
        {
            // Restore to the original place before we insernted
            BmBefore.RestoreWindowSelectionAndScrollPosition();

            // Perform the action
            PerformAction();

            // Restore to the After place
            BmAfter.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
    }
    #endregion

    // Join / Split Paragraphs ---------------------------------------------------------------
    #region CLASS: SplitJoinParagraphAction : Action
    public class SplitJoinParagraphAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
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
        #region Attr{g/s}: string FollowingParagraphStyle
        // If we Join two paragraphs with different styles, we need to rememver the style
        // of the second one, in case the user does an Undo. Otherwise, the default 
        // behavior is that the second paragraph will have the same style as the first.
        string FollowingParagraphStyle
        {
            get
            {
                return m_sFollowingParagraphStyle;
            }
            set
            {
                m_sFollowingParagraphStyle = value;
            }
        }
        string m_sFollowingParagraphStyle;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected SplitJoinParagraphAction(string sDisplayName, OWWindow window)
            : base(window, sDisplayName)
        {
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
            m_bookmark_BeforeSplit = Window.CreateBookmark();

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

            // If we're at the beginning of a DText, but there is another DText to the left,
            // with intervening non-edtiable stuff, we need to move to the end of the
            // preceeding DText, in order for the bookmark restore to perform properly.
            if (selection.Anchor.iChar == 0 && !selection.IsInsertionPoint_AtParagraphBeginning)
            {
                Window.Contents.Select_PrevWord_End(selection);
                selection = Window.Selection;
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
            OWBookmark bm = Window.CreateBookmark();

            // Split the underlying paragraph
            DParagraph paraNew = para.Split(text, iPos);
            if (null == paraNew)
                return false;
            if (!string.IsNullOrEmpty(FollowingParagraphStyle))
                paraNew.StyleAbbrev = FollowingParagraphStyle;

            // Reload the window's data. This is time-consuming, but it is the only way to make 
            // paragraphs line up correctly side-by-side.
            Window.LoadData();

            // Restore the selection insertion point to the Window.Selection
            bm.RestoreWindowSelectionAndScrollPosition();

            // This has us at the end of the first paragraph, we need to move to the
            // beginning of the next one
            Window.cmdMoveCharRight();

            // Remember this position in case we do a future Undo
            m_bookmark_AfterSplit = Window.CreateBookmark();

            return true;
        }
        #endregion
        #region Method: bool JoinToPrevious()
        protected bool JoinToPrevious()
        {
            // Bookmark the "Before" selection so we can Undo back to it
            OWWindow.Sel selection = Window.Selection;
            m_bookmark_BeforeJoin = Window.CreateBookmark();

            // Retrieve the underlying paragraph
            DParagraph p = selection.Paragraph.DataSource as DParagraph;
            if (null == p)
                return false;
            FollowingParagraphStyle = p.StyleAbbrev;

            // Retrieve the paragraph previous to it
            JOwnSeq<DParagraph> seq = p.GetMyOwningAttr() as JOwnSeq<DParagraph>;
            Debug.Assert(null != seq);
            int i = seq.FindObj(p) - 1;
            if (i < 0)
                return false;
            p = seq[i] as DParagraph;

            // We need to move the Window selection left, so that we have something valid to bookmark
            Window.cmdMoveCharLeft();
            OWBookmark bm = Window.CreateBookmark();

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
            : base(window, "Change Paragraph Style")
        {
            m_sRequestedStyleAbbrev = sRequestedStyleAbbrev;
        }
        #endregion

        // Helper methods --------------------------------------------------------------------
        #region Helper: bool _IsValidRequest(DParagraph p)
        bool _IsValidRequest(DParagraph p)
        {
            // If we're requesting a Section Title.... 
            bool bIsScripture = (p.Owner == p.Section);
            if (bIsScripture && RequestedStyleAbbrev == DStyleSheet.c_sfmSectionHead)
            {
                // ...there must not already be a section title
                if (p.Section.CountParagraphsWithStyle(DStyleSheet.c_sfmSectionHead) > 0)
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
            OWBookmark bm = Window.CreateBookmark();

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
                return DB.StyleSheet.FindParagraphStyle(OriginalStyleAbbrev).DisplayName;
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
            : base(window,
                (mode == DeleteMode.kDelete) ? "Delete" : 
                (mode == DeleteMode.kCut) ? "Cut" :
                (mode == DeleteMode.kCopy) ? "Copy" :
                (mode == DeleteMode.kBackSpace) ? "Backspace" :
                "Other")
        {
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
            m_bookmark_BeforeDelete = Window.CreateBookmark();

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
                    m_bookmark_AfterDelete = Window.CreateBookmark();
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
            m_bookmark_AfterDelete = Window.CreateBookmark();

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
            : base(window, sDisplayName)
        {
            m_sTextToInsert = sTextToInsert;
        }
        #endregion
        #region Constructor(OWWindow, chCharToInsert)
        public InsertAction(OWWindow window, char chCharToInsert)
            : base(window, "Typing")
        {
            m_sTextToInsert = chCharToInsert.ToString();
        }
        #endregion

        // Insert ----------------------------------------------------------------------------
        #region Method:bool Insert(string sTextToInsert)
        protected bool Insert(string sInsert)
        {
            // Retrieve the selected paragraph
            var op = Window.Selection.Paragraph;
            if (null == op)
                return false;

            // Do nothing if the paragraph is uneditable
            if (!op.IsEditable)
                return false;

            //Do nothing if there's nothing to insert
            if (string.IsNullOrEmpty(sInsert))
                return false;

            // Save a bookmark for where we are prior to the insertion
            m_bookmark_BeforeInsert = Window.CreateBookmark();

            // If we have an InsertionIcon, replace it with a space, so that the normal
            // mechanism can deal with it (deleting the space, then doing the insertion.)
            if (Window.Selection.IsInsertionIcon)
            {
                Window.Selection.Anchor.Word.Text = " ";
                Window.Selection = new OWWindow.Sel(Window.Selection.Paragraph,
                    Window.Selection.Anchor);
            }

            // We'll make a copy of the data for Undo; this is only a copy of the DBasicText,
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
            var sel = Window.Selection;
            var iBlockFirst = sel.DBT_iBlockFirst;

            // Get the offset into the DBasicText
            var n = Window.Selection.DBT_iCharFirst;

            // Retrieve which phrase we'll be working on (Vernacular or Back Translation)
            var DBT = sel.DBT;
            var phrases = (op.DisplayBT) ? DBT.PhrasesBT : DBT.Phrases;

            // We'll keep track of which DPhrase we're currently processing here
            var iPhrase = 0;

            // Increment past any DPhrases that are prior to the insertion point
            DPhrase phr = null;
            while (true)
            {
                phr = phrases[iPhrase];

                // We will do the insert in this phrase. (We use '>' rather than
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
            var phrase = phrases[iPhrase];
            var iPos = n;

            // Insert the text. We remove spurious spaces, so if the result of the insertion 
            // is that we have what we started with, then we need proceed no further.
            var sBeforeInsert = phrase.Text;
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
            var iInsertPos = sel.DBT_iCharFirst + sInsert.Length;
            sel = OWWindow.Sel.CreateSel(op, sel.DBT, iInsertPos);
            Window.Selection = op.NormalizeSelection(sel);

            // Save a bookmark for the result of the deletion
        done:
            m_bookmark_AfterInsert = Window.CreateBookmark();
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

            // AddParagraph what was just typed
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
            OWBookmark bm = Window.CreateBookmark();

            // Get the AutoReplace text
            TextToInsert = ProcessAutoReplace();
            m_bookmark_AutoReplace = Window.CreateBookmark();

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
            if (m_sClipboardText.Length > 1000)
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
            : base(window, "Italics")
        {
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
            m_bookmark_Italics = Window.CreateBookmark();

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

    // Toggle Expand/Collapsed Header --------------------------------------------------------
    #region CLASS: ToggleCollapsedHeader
    public class ToggleCollapsedHeader : BookmarkedAction
    {
        #region Attr{g}: CollapsableHeaderColumn CHC
        ECollapsableHeaderColumn CHC
        {
            get
            {
                Debug.Assert(null != m_chc);
                return m_chc;
            }
        }
        ECollapsableHeaderColumn m_chc;
        #endregion

        #region Constructor(OWWindow, ECollapsableHeaderColumn)
        public ToggleCollapsedHeader(OWWindow window, ECollapsableHeaderColumn chc)
            : base(window, (chc.IsCollapsed ? "Expand" : "Collapse"))
        {
            m_chc = chc;
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            // Toggle the setting
            CHC.IsCollapsed = !CHC.IsCollapsed;

            // If the selection is in this container, we'll need to move it
            bool bMustMoveSelection = CHC.ContainsSelection;

            // TODO: Recalculate our vertical spacing: CalculateContainerVerticals
            // Call Window.OnParagraphHeightChanged, rather than what we're doing
            // here of calling DoLayout. (Actually, unless there is a performance
            // issue, maybe it doesn't matter so much?)
            Window.DoLayout();
            Window.Invalidate();

            // So if we needed to, then select the first word of the window
            // TODO: Select into the preceeding or following EContainer
            //     so as to not mess up scrolling.
            // TODO: What if there is no place to move the selection?
            if (bMustMoveSelection)
            {
                if (!Window.Contents.Select_FirstWord())
                    Window.Selection = null;
            }

            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            PerformAction();
        }
        #endregion
    }
    #endregion

    // Footnotes -----------------------------------------------------------------------------
    #region CLASS: InsertDeleteFootnoteAction : Action
    public class InsertDeleteFootnoteAction : Action
    {
        // Protected Attrs -------------------------------------------------------------------
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
        #region Constructor(sDisplayName, OWWindow)
        protected InsertDeleteFootnoteAction(string sDisplayName, OWWindow window)
            : base(window, sDisplayName)
        {
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
            m_bookmark_BeforeInsert = Window.CreateBookmark();

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
            DFoot foot = para.InsertFootnote(text, iPos);
            m_InsertedFootnote = foot.Footnote;

            // Reload the window's data. This is time-consuming, but it is the only way to make 
            // paragraphs line up correctly side-by-side.
            Window.LoadData();

            // Move editing to the new footnote
            DFootnote footnote = foot.Footnote;
            EContainer container = Window.Contents.FindContainerOfDataSource(footnote);
            container.Select_FirstWord();

            // Remember where we are so we can undo
            m_bookmark_AfterInsert = Window.CreateBookmark();

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
            m_bookmark_BeforeDelete = Window.CreateBookmark();

            // Retrieve the current footnote
            DFootnote footnote = selection.DBT.Paragraph as DFootnote;
            if (null == footnote)
                return false;

            // Prepare for possible future Undo
            // Move the cursor to its place in the text; make a note of the selection
            Window.Contents.OnSelectAndScrollFrom(footnote);
            m_bookmark_PositionOfFootnoteLetter = Window.CreateBookmark();
            // Make a copy of the footnote
            m_CopyOfDeletedFootnote = new DFootnote(footnote.VerseReference, footnote.NoteType);
            m_CopyOfDeletedFootnote.CopyFrom(footnote, false);

            // Retrieve the paragraph and run that refers to it; and remove
            // the footnote
            bool bRemoved = false;
            foreach (DParagraph p in footnote.Section.Paragraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    DFoot foot = r as DFoot;
                    if (foot == null)
                        continue;

                    if (foot.Footnote != footnote)
                        continue;

                    p.RemoveFootnote(foot);
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
            m_bookmark_AfterDelete = Window.CreateBookmark();

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

    // Translator Notes ----------------------------------------------------------------------
    #region CLASS: AddMessageAction
    public class AddMessageAction : BookmarkedAction
    {
        #region VAttr{g}: TranslatorNote Note
        TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }
        private readonly TranslatorNote m_Note;
        #endregion

        #region Constructor(OWWindow, ENote)
        public AddMessageAction(OWWindow window, TranslatorNote note)
            : base(window, "Add Response to Note")
        {
            m_Note = note;
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
            // Create and add a new Message item
        {
            // Create a new Message object and add it to the annotation
            var message = new DMessage();
            Note.Messages.Append(message);
            Note.Status = DMessage.Anyone;
            Note.Debug_VerifyIntegrity();

            // Reload the Window, and recalculate its display
            var wnd = OWToolTip.ToolTip.ContentWindow;
            wnd.LoadData();

            // Update the underlying window's icon in case the status is now different
            G.App.ResetWindowContents();

            // Select the new discussion
            var container = wnd.Contents.FindContainerOfDataSource(message);
            container.Select_LastWord_End();
            wnd.Focus();

            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            // Retrieve the last discussion
            var message = Note.LastMessage;

            // Remove it from the note
            Note.Messages.Remove(message);
            Note.Debug_VerifyIntegrity();

            // Reload the Window, and recalculate its display
            var wnd = OWToolTip.ToolTip.ContentWindow;
            wnd.LoadData();

            // Update the underlying window's icon
            G.App.ResetWindowContents();
        }
        #endregion
    }
    #endregion
    #region CLASS: RemoveMessageAction
    public class RemoveMessageAction : BookmarkedAction
    {
        #region VAttr[g}: TranslatorNote Note
        protected TranslatorNote Note
        {
            get
            {
                return m_nNote;
            }
        }
        TranslatorNote m_nNote;
        #endregion
        #region Attr{g}: DMessage RemovedMessage
        DMessage RemovedMessage
        {
            get
            {
                Debug.Assert(null != m_RemovedMessage);
                return m_RemovedMessage;
            }
            set
            {
                Debug.Assert(null != value);
                m_RemovedMessage = value;
            }
        }
        DMessage m_RemovedMessage;
        #endregion

        #region Constructor(OWWindow, DMessage)
        public RemoveMessageAction(OWWindow window, DMessage message)
            : base(window, "Remove Response to Note")
        {
            m_RemovedMessage = message;
            m_nNote = message.Note;
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            // Removing the only discussion is the same as deleting the note
            if (Note.Messages.Count < 2)
                return false;

            // Remove it from the note
            Note.Messages.Remove(RemovedMessage);
            Note.Debug_VerifyIntegrity();

            // Recalc the window
            OWToolTip.ToolTip.ContentWindow.LoadData();

            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            // AddParagraph the removed discussion
            Note.Messages.Append(RemovedMessage);
            Note.Debug_VerifyIntegrity();

            // Recalc the window
            OWToolTip.ToolTip.ContentWindow.LoadData();
        }
        #endregion
    }
    #endregion
    #region CLASS: InsertNoteAction
    public class InsertNoteAction : BookmarkedAction
        #region Doc
        /* The user will have a selection (and focus) in the main window, and 
         * when we insert the note, focus goes to the notes window. So on Undo,
         * we have to restore things in the main window, including moving
         * focus there. 
         */
        #endregion
    {
        #region Attr{g/s}: OWBookmark BmMainWnd - keep track of the main window's selection
        protected OWBookmark BmMainWnd
        {
            get
            {
                Debug.Assert(null != m_BmMainWnd);
                return m_BmMainWnd;
            }
            set
            {
                m_BmMainWnd = value;
            }
        }
        OWBookmark m_BmMainWnd;
        #endregion
        #region Attr[g}: TranslatorNote Note - so we know which to delete on an Undo on
        public TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }
        TranslatorNote m_Note;
        #endregion
        #region Attr{g}: TranslatorNote.Properties Behavior
        TranslatorNote.Properties Behavior
        {
            get
            {
                return m_Behavior;
            }
        }
        TranslatorNote.Properties m_Behavior;
        #endregion

        #region Constructor(OWWindow, Properties)
        public InsertNoteAction(OWWindow window, TranslatorNote.Properties _Behavior)
            : base(window, "Insert Translator Note")
        {
            m_Behavior = _Behavior;
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            // Remember the main window location, as we have to reset the 
            // scroll position after we regenerate window contents
            if (G.App.CurrentLayout.Selection == null)
                return false;
            BmMainWnd = G.App.CurrentLayout.CreateBookmark();

            // Retrieve the DText to which this note will be attached
            DText text = G.App.CurrentLayout.Selection.Anchor.BasicText as DText;

            // Create a blank note and insert it
            m_Note = new TranslatorNote(G.App.CurrentLayout.Selection.SelectionString);
            Note.Behavior = Behavior;
            Note.Messages.Append(new DMessage());
            Note.Status = DMessage.Anyone;
            text.TranslatorNotes.Append(Note);

            // Recalculate the entire display
            G.App.ResetWindowContents();

            // Return the Main Window to where it was
            BmMainWnd.RestoreWindowSelectionAndScrollPosition();

            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            // Remove the note from the owning DText
            DText text = Note.Text;
            Debug.Assert(null != text);
            text.TranslatorNotes.Remove(Note);

            // Recalculate the entire display
            G.App.ResetWindowContents();

            // Focus back to the main window
            BmMainWnd.RestoreWindowSelectionAndScrollPosition();
        }
        #endregion
    }
    #endregion
    #region CLASS: DeleteNoteAction
    public class DeleteNoteAction : BookmarkedAction
        #region Doc
        /* Focus and Selection will be in the Notes window. When we delete the 
         * note, we need a new selection in the notes window. Thus our behavior
         * it not like InsertNoteAction, where focus changes windows.
         */
        #endregion
    {
        // The Note we'll Undo
        #region Attr[g}: TranslatorNote Note - We keep it so we can restore on Undo
        protected TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }
        TranslatorNote m_Note;
        #endregion

        // Where the re-inserted note will be placed
        #region Attr{g}: JObject Root
        JObject Root
        {
            get
            {
                Debug.Assert(null != m_Root);
                return m_Root;
            }
        }
        JObject m_Root;
        #endregion
        #region Attr{g}: string PathToDBTFromRoot
        string PathToDBTFromRoot
        {
            get
            {
                return m_sPathToDBTFromRoot;
            }
        }
        string m_sPathToDBTFromRoot;
        #endregion
        #region Attr{g/s}: int NotePos - of Note within the DText's attr
        int NotePos
        {
            get
            {
                return m_iNotePos;
            }
            set
            {
                m_iNotePos = value;
            }
        }
        int m_iNotePos;
        #endregion

        #region Constructor(OWWindow, TranslatorNote)
        public DeleteNoteAction(OWWindow window, TranslatorNote note)
            : base(window, "Delete Translator Note")
        {
            Debug.Assert(null != note);
            m_Note = note;
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            // Get the owning DText
            DText OwningText = Note.Text;
            Debug.Assert(null != OwningText);

            // Save the path to it, so we can recover it on an undo. (We can't just
            // store the DText, because subsequent actions could have destroyed it.)
            m_Root = Note.RootOwner;
            m_sPathToDBTFromRoot = OwningText.GetPathFromRoot();
            NotePos = OwningText.TranslatorNotes.FindObj(Note);
            Debug.Assert(-1 != NotePos);

            // Remove the note from its owning DText
            OwningText.TranslatorNotes.Remove(Note);

            // Regenerate all of the windows
            G.App.ResetWindowContents();

            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            // Get the DText we want to insert to
            DText text = Root.GetObjectFromPath(PathToDBTFromRoot) as DText;
            Debug.Assert(null != text);

            // Re-insert the note
            text.TranslatorNotes.InsertAt(NotePos, Note);

            // Recalculate the entire display
            G.App.ResetWindowContents();
        }
        #endregion
    }
    #endregion
	#region CLASS: ChangeStatus
	class ChangeStatus : BookmarkedAction
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: ENote ENote
        ENote ENote
        {
            get
            {
                var vParagraphs = G.App.CurrentLayout.Contents.AllParagraphs;
                foreach (var paragraph in vParagraphs)
                {
                    foreach(var item in paragraph.SubItems)
                    {
                        var enote = item as ENote;
                        if (null == enote)
                            continue;

                        if (enote.Note == Note)
                            return enote;
                    }
                }

                Debug.Assert(false, "View doesn't contain the enote");
                return null;
            }
        }
        #endregion
        #region VAttr{g}: TranslatorNote Note
        TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }

	    readonly private TranslatorNote m_Note;
        #endregion
        #region Attr{g}: ToolStripDropDownButton DropDownButton
        public ToolStripDropDownButton DropDownButton
        {
            get
            {
                return m_DropDownButton;
            }
        }
        ToolStripDropDownButton m_DropDownButton;
        #endregion
        #region Attr{g}: string NewStatus
        string NewStatus
        {
            get
            {
                return m_sNewStatus;
            }
        }
        string m_sNewStatus;
        #endregion
        #region Attr{g}: string OriginalStatus
        string OriginalStatus
        {
            get
            {
                return m_sOriginalStatus;
            }
        }
        string m_sOriginalStatus;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow, ENote, ToolStripMenuItem newPerson)
        public ChangeStatus(OWWindow window, 
            TranslatorNote note,
            ToolStripMenuItem itemNewStatus)
            : base(window, "Assign to")
        {
            m_Note = note;

            // Save a pointer to the owning DropDown button which owns this item
            m_DropDownButton = itemNewStatus.OwnerItem as ToolStripDropDownButton;
            Debug.Assert(null != m_DropDownButton);

            // Save what the new classification text will be
            m_sNewStatus = itemNewStatus.Text;

            // Remember what the current (original) status is
            m_sOriginalStatus = DropDownButton.Text;
        }
        #endregion
		#region OAttr{g}: string Contents - Places the name we assigned to, into the Undo menu
		public override string Contents
		{
			get
			{
				return NewStatus;
			}
		}
		#endregion

        // Action ----------------------------------------------------------------------------
        #region Method: void SetStatus(string sStatus)
        void SetStatus(string sStatus)
        {
            // Save the new status
            Note.Status = sStatus;

            // Update the dropdown button text
            DropDownButton.Text = sStatus;

            // Check the correct item within
            foreach (ToolStripItem item in DropDownButton.DropDownItems)
            {
                var menuItem = item as ToolStripMenuItem;
                if (null != menuItem)
                    menuItem.Checked = (menuItem.Text == sStatus);
            }

            // Handle the item's tooltip
            AnnotationTipBuilder.SetStatusToolTip(Note, DropDownButton);

            // Reload the Window, and recalculate its display, in order to update the icon
            var wnd = OWToolTip.ToolTip.ContentWindow;
            wnd.LoadData();

            // Update the underlying window's icon
            ENote.InitializeBitmap(wnd.BackColor);
            G.App.CurrentLayout.Invalidate();
        }
        #endregion
        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            SetStatus(NewStatus);
            return true;
        }
        #endregion
        #region Omethod: void ReverseAction()
        protected override void ReverseAction()
        {
            SetStatus(OriginalStatus);
        }
        #endregion
	}
	#endregion
    #region CLASS: ChangeAuthor
    class ChangeAuthor : BookmarkedAction
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: TranslatorNote Note
        TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }
        TranslatorNote m_Note;
        #endregion
        #region Attr{g}: ToolStripComboBox Combo
        public ToolStripComboBox Combo
        {
            get
            {
                return m_Combo;
            }
        }
        ToolStripComboBox m_Combo;
        #endregion
        #region Attr{g}: string NewAuthor
        string NewAuthor
        {
            get
            {
                return m_sNewAuthor;
            }
        }
        string m_sNewAuthor;
        #endregion
        #region Attr{g}: string OriginalAuthor
        string OriginalAuthor
        {
            get
            {
                return m_sOriginalAuthor;
            }
        }
        string m_sOriginalAuthor;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow, Note, ToolStripComboBox combo)
        public ChangeAuthor(OWWindow window,
            TranslatorNote _note,
            ToolStripComboBox comboAuthor,
            string sNewAuthor,
            string sOriginalAuthor)
            : base(window, "Author")
        {
            m_Note = _note;
            m_sNewAuthor = sNewAuthor;
            m_sOriginalAuthor = sOriginalAuthor;
            m_Combo = comboAuthor;
        }
        #endregion
        #region OAttr{g}: string Contents - Places the name we assigned to, into the Undo menu
        public override string Contents
        {
            get
            {
                return NewAuthor;
            }
        }
        #endregion

        // Action ----------------------------------------------------------------------------
        #region Method: void SetAuthor(string sAuthor)
        void SetAuthor(string sAuthor)
        {
            // Save the new status
            Note.LastMessage.Author = sAuthor;
            DB.UserName = sAuthor;

            // Update the dropdown button text
            Combo.Text = sAuthor;
        }
        #endregion
        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            SetAuthor(NewAuthor);
            return true;
        }
        #endregion
        #region Omethod: void ReverseAction()
        protected override void ReverseAction()
        {
            SetAuthor(OriginalAuthor);
        }
        #endregion
    }
    #endregion

    // History -------------------------------------------------------------------------------
    #region CLASS: ChangeStage
    public class ChangeStage : BookmarkedAction
    {
        #region Attr{g}: DEventMessage Event
        DEventMessage Event
        {
            get
            {
                Debug.Assert(null != m_Event);
                return m_Event;
            }
        }
        DEventMessage m_Event;
        #endregion
        #region Attr{g}: Stage NewStage
        Stage NewStage
        {
            get
            {
                return m_NewStage;
            }
        }
        Stage m_NewStage;
        #endregion
        #region Attr{g}: Stage OriginalStage
        Stage OriginalStage
        {
            get
            {
                return m_OriginalStage;
            }
        }
        Stage m_OriginalStage;
        #endregion
        #region Attr{g}: ToolStripDropDownButton StagesDropDown
        public ToolStripDropDownButton StagesDropDown
        {
            get
            {
                Debug.Assert(null != m_StagesDropDownButton);
                return m_StagesDropDownButton;
            }
        }
        ToolStripDropDownButton m_StagesDropDownButton;
        #endregion

        #region Constructor(OWWindow, Event, itemNewStage)
        public ChangeStage(OWWindow window, DEventMessage Event, ToolStripMenuItem itemNewStage)
            : base(window, "Change Translation Stage to")
        {
            m_Event = Event;

            // Get the stage we want to get set to
            string sNewStageLocAbbrev = itemNewStage.Text;
            m_NewStage = DB.TeamSettings.Stages.Find(
                StageList.FindBy.LocalizedAbbrev,
                sNewStageLocAbbrev);

            // Get the stage we are currently
            m_OriginalStage = Event.Stage;

            // Save a pointer to the owning DropDown button which owns this item
            m_StagesDropDownButton = itemNewStage.OwnerItem as ToolStripDropDownButton;
            Debug.Assert(null != m_StagesDropDownButton);
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            // Set the event's stage
            Event.Stage = NewStage;

            // Update the menu
            foreach (ToolStripMenuItem item in StagesDropDown.DropDownItems)
                item.Checked = (item.Text == Event.Stage.LocalizedAbbrev);

            StagesDropDown.Text = Event.Stage.LocalizedAbbrev;

            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            // Re-set the event's stage
            Event.Stage = OriginalStage;

            // Update the menu
            foreach (ToolStripMenuItem item in StagesDropDown.DropDownItems)
                item.Checked = (item.Text == NewStage.LocalizedAbbrev);

            StagesDropDown.Text = Event.Stage.LocalizedAbbrev;
        }
        #endregion

        #region OAttr{g}: string Contents - places the new Stage into the Undo menu
        public override string Contents
        {
            get
            {
                return NewStage.LocalizedAbbrev;
            }
        }
        #endregion
    }
    #endregion
    #region CLASS: ChangeEventDate
    public class ChangeEventDate : BookmarkedAction
    {
        #region Attr{g}: DEventMessage Event
        DEventMessage Event
        {
            get
            {
                Debug.Assert(null != m_Event);
                return m_Event;
            }
        }
        DEventMessage m_Event;
        #endregion
        #region Attr{g}: DateTime NewDate
        DateTime NewDate
        {
            get
            {
                Debug.Assert(null != m_NewDate);
                return m_NewDate;
            }
        }
        DateTime m_NewDate;
        #endregion
        #region Attr{g}: DateTime OriginalDate
        DateTime OriginalDate
        {
            get
            {
                Debug.Assert(null != m_OriginalDate);
                return m_OriginalDate;
            }
        }
        DateTime m_OriginalDate;
        #endregion

        #region Constructor(OWWindow, Event, DateTimePicker)
        public ChangeEventDate(OWWindow window, DEventMessage Event, DateTimePicker dtp)
            : base(window, "Change Date to")
        {
            m_Event = Event;
            m_NewDate = dtp.Value;
            m_OriginalDate = Event.EventDate;
        }
        #endregion

        #region OMethod: bool PerformAction()
        protected override bool PerformAction()
        {
            Event.EventDate = NewDate;
            return true;
        }
        #endregion
        #region OMethod: void ReverseAction()
        protected override void ReverseAction()
        {
            Event.EventDate = OriginalDate;
        }
        #endregion

        #region OAttr{g}: string Contents - places the new date into the Undo menu
        public override string Contents
        {
            get
            {
                return NewDate.ToString("yyyy-MM-dd");
            }
        }
        #endregion
    }
    #endregion   
}
