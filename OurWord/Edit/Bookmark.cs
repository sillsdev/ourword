/**********************************************************************************************
 * Project: OurWord!
 * File:    OWBookmark.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2007
 * Purpose: A bookmark for the current Selection, that is not dependent on pointers
 *            to transient objects such as OWParas, DParagraphs, etc.
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using Palaso.UI.WindowsForms.Keyboarding;
#endregion

namespace OurWord.Edit
{
    public class OWBookmark
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: OWWindow Window
        OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_Window);
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        #region Attr{g}: JOwnSeq SectionSeq - either Section.Paragraphs or Section.Footnotes
        JOwnSeq SectionSeq
        {
            get
            {
                Debug.Assert(null != m_SectionSeq);
                return m_SectionSeq;
            }
        }
        JOwnSeq m_SectionSeq;
        #endregion
        #region Attr{g}: int iParaPosWithinSectionSeq - the position with the Secton's paragraph/Footnote sequence
        int iParaPosWithinSectionSeq
        {
            get
            {
                return m_iParaPosWithinSectionSeq;
            }
        }
        int m_iParaPosWithinSectionSeq;
        #endregion
        #region Attr[g}: int iRunPosWithinParagraph
        int iRunPosWithinParagraph
        {
            get
            {
                return m_iRunPosWithinParagraph;
            }
        }
        int m_iRunPosWithinParagraph;
        #endregion
        #region Attr[g}: int iNotePosWithinRun
        int iNotePosWithinRun
        {
            get
            {
                return m_iNotePosWithinRun;
            }
        }
        int m_iNotePosWithinRun = -1;
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(selection)
        public OWBookmark(OWWindow.Sel selection)
        {
            // The Window
            m_Window = selection.Paragraph.Window;

            // And editing flags
            m_ParagraphFlags = selection.Paragraph.Options;

            // Get the owning paragraph. The selection is owned by either a DNote or a
            // DParagraph, in case of the former we just go up a level.
            DParagraph p = selection.Paragraph.DataSource as DParagraph;
            DNote note = selection.Paragraph.DataSource as DNote;
            if (null != note)
                p = note.Paragraph;
            Debug.Assert(null != p);

            // Get the paragraph's position within its owning sequence
            m_iParaPosWithinSectionSeq = p.Section.Paragraphs.FindObj(p);
            if (-1 != m_iParaPosWithinSectionSeq)
            {
                m_SectionSeq = p.Section.Paragraphs;
            }
            else
            {
                m_iParaPosWithinSectionSeq = p.Section.Footnotes.FindObj(p);
                m_SectionSeq = p.Section.Footnotes;
            }
            Debug.Assert(-1 != m_iParaPosWithinSectionSeq);

            // Get the run's position within its paragraph
            DBasicText DBT = selection.DBT;
            if (null != note)
                DBT = note.Text;
            m_iRunPosWithinParagraph = p.Runs.FindObj(DBT);

            // In the case of notes, find the note's position within the run
            if (null != note)
                m_iNotePosWithinRun = (DBT as DText).Notes.FindObj(note);

            // Scroll bar position
            m_fScrollBarPosition = m_Window.ScrollBarPosition;

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

        // Public Interface ------------------------------------------------------------------
        #region Method: Sel CreateSelection()
        public OWWindow.Sel CreateSelection()
        {
            // Retrieve the paragraph in question
            DParagraph p = SectionSeq[iParaPosWithinSectionSeq] as DParagraph;
            Debug.Assert(null != p);

            // Retrieve the DBT in question
            DBasicText DBT = p.Runs[iRunPosWithinParagraph] as DBasicText;

            // Locate the Note in question, if any
            DNote note = null;
            if (DBT as DText != null && iNotePosWithinRun != -1)
            {
                note = (DBT as DText).Notes[iNotePosWithinRun] as DNote;
                DBT = note.NoteText;
            }

            // Determine the data source
            JObject objDataSource = p;
            if (null != note)
                objDataSource = note;
            Debug.Assert(null != objDataSource);

            // Locate the OWPara which has this as its data source
            OWPara op = FindPara(Window, objDataSource, ParagraphFlags);
            Debug.Assert(null != op);

            // Create and return the normalized selection
            OWWindow.Sel selection = (IsInsertionPoint) ?
                OWWindow.Sel.CreateSel(op, DBT, AnchorPositionInParagraph) :
                OWWindow.Sel.CreateSel(op, DBT, AnchorPositionInParagraph, EndPositionInParagraph);
            return op.NormalizeSelection(selection);
        }
        #endregion
        #region Method: void RestoreWindowSelectionAndScrollPosition()
        public void RestoreWindowSelectionAndScrollPosition()
        {
            Window.Selection = CreateSelection();
            Window.ScrollBarPosition = ScrollBarPosition;
        }
        #endregion
        #region SMethod: OWPara FindPara(OWWindow wnd, JObject objDataSource, OWPara.Flags)
        static public OWPara FindPara(OWWindow wnd, JObject objDataSource, OWPara.Flags Flags)
            // The paragraph may appear multiple times in a view (e.g., the back
            // translation view does this; to distinquish, for paragraphs, we
            // check for the same editing flags.
        {
            // We want most of the flags to be the same; though we don't care if
            // CanItalics is different.
            Flags |= OWPara.Flags.CanItalic;

            // Loop through the entire display searching for the data source
            foreach (OWWindow.Row row in wnd.Rows)
            {
                foreach (OWWindow.Row.Pile pile in row.Piles)
                {
                    foreach (OWPara op in pile.Paragraphs)
                    {
                        if (op.DataSource == objDataSource)
                        {
                            if ( Flags == (op.Options | OWPara.Flags.CanItalic))
                                return op;
                            else if (Flags == OWPara.Flags.CanItalic) // E.g., for where None was passed in
                                return op;
                        }
                    }
                }
            }
            return null;
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
            if (SectionSeq != bm.SectionSeq)
                return false;
            if (iParaPosWithinSectionSeq != bm.iParaPosWithinSectionSeq)
                return false;
            if (iRunPosWithinParagraph != bm.iRunPosWithinParagraph)
                return false;
            if (iNotePosWithinRun != bm.iNotePosWithinRun)
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
