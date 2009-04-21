/**********************************************************************************************
 * Project: OurWord!
 * File:    OWBookmark.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2007
 * Purpose: A bookmark for the current Selection, that is not dependent on pointers
 *            to transient objects such as OWParas, DParagraphs, etc.
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
using Palaso.UI.WindowsForms.Keyboarding;
#endregion

namespace OurWord.Edit
{
    public class OWBookmark
    {
        // Attrs -----------------------------------------------------------------------------
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
        #region Attr{g}: bool HasSelection
        bool HasSelection
        {
            get
            {
                return m_bHasSelection;
            }
        }
        bool m_bHasSelection;
        #endregion
        #region Attr{g}: float ScrollBarPosition
        public float ScrollBarPosition
        {
            get
            {
                return m_fScrollBarPosition;
            }
        }
        float m_fScrollBarPosition;
        #endregion
        #region Attr{g}: int AnchorPositionInParagraph
        int AnchorPositionInParagraph
        {
            get
            {
                Debug.Assert(-1 != m_iAnchorPositionInParagraph);
                return m_iAnchorPositionInParagraph;
            }
        }
        int m_iAnchorPositionInParagraph;
        #endregion
        #region Attr{g}: int EndPositionInParagraph
        int EndPositionInParagraph
        {
            get
            {
                Debug.Assert(-1 != m_iEndPositionInParagraph);
                return m_iEndPositionInParagraph;
            }
        }
        int m_iEndPositionInParagraph;
        #endregion
        #region Attr{g}: bool IsInsertionPoint
        public bool IsInsertionPoint
        {
            get
            {
                return m_bIsInsertionPoint;
            }
        }
        bool m_bIsInsertionPoint = true;
        #endregion
        #region Attr{g}: Flags ParagraphFlags
        public OWPara.Flags ParagraphFlags
        {
            get
            {
                return m_ParagraphFlags;
            }
        }
        OWPara.Flags m_ParagraphFlags = OWPara.Flags.None;
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(selection)
        public OWBookmark(OWWindow _Wnd)
        {
            // The Window
            m_Window = _Wnd;

            // Scroll bar position
            m_fScrollBarPosition = m_Window.ScrollBarPosition;

            // The Selection
            m_bHasSelection = Window.HasSelection;
            if (!HasSelection)
                return;
            OWWindow.Sel selection = Window.Selection;

            // Paragraph editing flags
            m_ParagraphFlags = selection.Paragraph.Options;

            // Get the path to the run
            m_Root = selection.DBT.RootOwner;
            m_sPathToDBTFromRoot = selection.DBT.GetPathFromRoot();

            // Selection Start Position
            m_iAnchorPositionInParagraph = selection.DBT_iChar(selection.Anchor);

            // Selection End Position
            if (null != selection.End)
            {
                m_iEndPositionInParagraph = selection.DBT_iChar(selection.End);
                m_bIsInsertionPoint = false;
            }
        }
        #endregion
        #region VMethod: bool ContentEquals(OWBookmark)
        public virtual bool ContentEquals( OWBookmark bm )
        {
            if (Window != bm.Window)
                return false;

            if (AnchorPositionInParagraph != bm.AnchorPositionInParagraph)
                return false;

            if (EndPositionInParagraph != bm.EndPositionInParagraph)
                return false;

            if (IsInsertionPoint != bm.IsInsertionPoint)
                return false;

            if (ParagraphFlags != bm.ParagraphFlags)
                return false;

            if (PathToDBTFromRoot != bm.PathToDBTFromRoot)
                return false;

            if (Root != bm.Root)
                return false;

            return true;
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Method: Sel CreateSelection()
        public OWWindow.Sel CreateSelection()
        {
            if (!HasSelection)
                return null;

            // Retrieve the DBT
            DBasicText DBT = Root.GetObjectFromPath(PathToDBTFromRoot) as DBasicText;
            Debug.Assert(null != DBT);

            // It's owner is the paragraph's data source
            DParagraph pDataSource = DBT.Owner as DParagraph;
            Debug.Assert(null != pDataSource);

            // Locate the OWPara which has this data source
            OWPara op = Window.Contents.FindParagraph(pDataSource, ParagraphFlags);
            Debug.Assert(null != op);

            // Create and return the normalized selection
            OWWindow.Sel selection = (IsInsertionPoint) ?
                OWWindow.Sel.CreateSel(op, DBT, AnchorPositionInParagraph) :
                OWWindow.Sel.CreateSel(op, DBT, AnchorPositionInParagraph, EndPositionInParagraph);
            return op.NormalizeSelection(selection);
        }
        #endregion
        #region Method: void RestoreWindowSelectionAndScrollPosition()
        public virtual void RestoreWindowSelectionAndScrollPosition()
        {
            Window.Selection = CreateSelection();
            Window.ScrollBarPosition = ScrollBarPosition;
        }
        #endregion
        #region Method: bool IsAdjacentTo(OWBookmark bm)
        public bool IsAdjacentTo(OWBookmark bm)
        {
            // Check for equality on the big picture. I thought about ignoring ScrollBarPosition,
            // but abandoned the idea because it would be difficult to then know how to combine
            // the two bookmarks.
            if (Window != bm.Window)
                return false;
            if (Root != bm.Root)
                return false;
            if (PathToDBTFromRoot != bm.PathToDBTFromRoot)
                return false;
            if (ParagraphFlags != bm.ParagraphFlags)
                return false;

            // Shorthand for Left and Right positions
            int iRight = (IsInsertionPoint) ? AnchorPositionInParagraph : EndPositionInParagraph;
            int iLeft = AnchorPositionInParagraph;

            int ibmRight = (bm.IsInsertionPoint) ? bm.AnchorPositionInParagraph : bm.EndPositionInParagraph;
            int ibmLeft = bm.AnchorPositionInParagraph;

            // Is bm to the right?
            if (iRight == ibmLeft)
                return true;

            // is bm to the left?
            if (iLeft == ibmRight)
                return true;

            return false;
        }
        #endregion
    }
}
