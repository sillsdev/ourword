/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgCopyBTConflict.cs
 * Author:  John Wimbish
 * Created: 02 Oct 2007
 * Purpose: Dialog to copy the back translations from the front to the daughter
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
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion

namespace OurWord.Dialogs
{
    public partial class DialogCopyBTConflict : Form
    {
        // Resolution UndoStack ----------------------------------------------------------------
        public enum Actions { kKeepExisting, kReplaceTarget, kAppendToTarget, kCancel };
        #region SAttr{g/s}: UndoStack CopyBTAction
        static public Actions CopyBTAction
        {
            get
            {
                return s_CopyBTAction;
            }
            set
            {
                s_CopyBTAction = value;
            }
        }
        static public Actions s_CopyBTAction = Actions.kAppendToTarget;
        #endregion
        #region SAttr{g/s}: bool ApplyToAll
        static public bool ApplyToAll
        {
            get
            {
                return s_bApplyToAll;
            }
            set
            {
                s_bApplyToAll = value;
            }
        }
        static bool s_bApplyToAll = false;
        #endregion

        // Attrs -----------------------------------------------------------------------------
        DParagraph m_pFront;
        DParagraph m_pTarget;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(PFront, PTarget)
        public DialogCopyBTConflict(DParagraph _pFront, DParagraph _pTarget)
        {
            m_pFront = _pFront;
            m_pTarget = _pTarget;

            InitializeComponent();
        }
        #endregion
        #region Constructor() - only for Visual Studio
        public DialogCopyBTConflict()
        {
            InitializeComponent();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Localization
            Control[] vExclude = { m_groupFront, m_groupTarget };
            LocDB.Localize(this, vExclude);

            // Group Text-Label Values
            m_groupFront.Text = m_pFront.Translation.DisplayName;
            m_groupTarget.Text = m_pTarget.Translation.DisplayName;

            // Load up the paragraphs
            m_textFrontVernacular.Text = m_pFront.AsString;
            m_textFrontBT.Text = m_pFront.ProseBTAsString;
            m_textTargetVernacular.Text = m_pTarget.AsString;
            m_textTargetBT.Text = m_pTarget.ProseBTAsString;
        }
        #endregion
        #region Cmd: cmdKeepExisting
        private void cmdKeepExisting(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kKeepExisting;

            DialogResult = DialogResult.OK;

            Close();
        }
        #endregion
        #region Cmd: cmdReplaceTargetWithFront
        private void cmdReplaceTargetWithFront(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kReplaceTarget;

            DialogResult = DialogResult.OK;

            Close();
        }
        #endregion
        #region Cmd: cmdAppendFrontToTarget
        private void cmdAppendFrontToTarget(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kAppendToTarget;

            DialogResult = DialogResult.OK;

            Close();
        }
        #endregion
        #region Cmd: cmdCheckChanged
        private void cmdCheckChanged(object sender, EventArgs e)
        {
            s_bApplyToAll = m_checkDoAll.Checked;
        }
        #endregion
        #region Cmd: cmdCancel
        private void cmdCancel(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kCancel;
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion
    }


    public class CopyBtMethod
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: bool EntireBook
        public bool EntireBook
        {
            get
            {
                return m_bEntireBook;
            }
        }
        readonly bool m_bEntireBook;
        #endregion

        // Work Methods ----------------------------------------------------------------------
        #region Method: void DisplaySternWarning()
        static bool m_bSternWarningSeen = false;
        void DisplaySternWarning()
        {
            if (!m_bSternWarningSeen)
            {
                CopyBTfromFront dlg = new CopyBTfromFront(EntireBook);
                if (DialogResult.OK != dlg.ShowDialog())
                    return;
                m_bSternWarningSeen = true;
            }
        }
        #endregion
        #region Method: void DisplayMismatchedSectionsWarning()
        bool m_bMismatchErrorShown = false;
        void DisplayMismatchedSectionsWarning()
        {
            if (false == m_bMismatchErrorShown)
            {
                MessageBox.Show(Form.ActiveForm,
                    "Unable to completely copy the BT's in one (or more) sections because\n" +
                    "the structures do not line up exactly. You'll have to do these manually.",
                    "Our Word!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_bMismatchErrorShown = true;
            }
        }
        #endregion

        #region Method: bool DoFootnotesMatch(...)
        private bool DoFootnotesMatch(DSection SFront, DSection STarget)
        {
            if (SFront.Footnotes.Count != STarget.Footnotes.Count)
                return false;

            for (int i = 0; i < STarget.Footnotes.Count; i++)
            {
                DParagraph PFront = SFront.Footnotes[i] as DParagraph;
                DParagraph PTarget = STarget.Footnotes[i] as DParagraph;
                if (PFront.StyleAbbrev != PTarget.StyleAbbrev)
                    return false;
            }

            return true;
        }
        #endregion
        #region Method: DParagraph[] GetParagraphsFromPart(DSection, int nPart)
        DParagraph[] GetParagraphsFromPart(DSection section, int nPart)
        {
            int iPara = 0;

            // Move past the preceeding parts of the section
            string sSectionStructure = section.SectionStructure;
            for (int n = 0; n < nPart; n++)
            {
                char chPartType = sSectionStructure[n];

                while (iPara < section.Paragraphs.Count)
                {
                    DParagraph p = section.Paragraphs[iPara] as DParagraph;

                    // Get the type (either a "Text" or a "Picture")
                    char ch = DSection.c_Text;
                    if (p as DPicture != null)
                        ch = DSection.c_Pict;

                    if (ch != chPartType)
                        break;

                    ++iPara;
                }
            }

            // Now add the desired paragraphs to the array list
            ArrayList a = new ArrayList();
            char chDesiredPartType = sSectionStructure[nPart];
            while (iPara < section.Paragraphs.Count)
            {
                DParagraph p = section.Paragraphs[iPara] as DParagraph;

                // Get the type (either a "Text" or a "Picture")
                char ch = DSection.c_Text;
                if (p as DPicture != null)
                    ch = DSection.c_Pict;

                if (ch != chDesiredPartType)
                    break;
                a.Add(p);
                ++iPara;
            }

            // Convert to an array
            DParagraph[] v = new DParagraph[a.Count];
            for (int k = 0; k < a.Count; k++)
                v[k] = a[k] as DParagraph;
            return v;
        }
        #endregion

        #region Method: bool CopyParagraph(DParagraph PFront, DParagraph PTarget)
        bool CopyParagraph(DParagraph PFront, DParagraph PTarget)
        {
            if (PTarget.Runs.Count != PFront.Runs.Count)
                return true;
            if (PTarget.TypeCodes != PFront.TypeCodes)
                return true;

            // If the Front's BT and the Target's BT have identical text, then we do nothing
            bool bIsIdentical = true;
            for (int i = 0; i < PTarget.Runs.Count; i++)
            {
                DRun RTarget = PTarget.Runs[i] as DRun;
                DRun RFront = PFront.Runs[i] as DRun;

                if (RTarget.ProseBTAsString != RFront.ProseBTAsString)
                {
                    bIsIdentical = false;
                    break;
                }
            }
            if (bIsIdentical)
                return true;

            // Does the Back Translation already exist? We need to find out what the
            // user desires, if so.
            if (PTarget.HasBackTranslationText && !DialogCopyBTConflict.ApplyToAll)
            {
                DialogCopyBTConflict dlg = new DialogCopyBTConflict(PFront, PTarget);
                if (DialogResult.OK != dlg.ShowDialog())
                    return false;
            }

            // If the user said "Do Nothing", then we're done here
            if (PTarget.HasBackTranslationText &&
                DialogCopyBTConflict.CopyBTAction == DialogCopyBTConflict.Actions.kKeepExisting)
            {
                return true;
            }

            // Perform the copy (or append, depending)
            bool bReplaceTarget = (DialogCopyBTConflict.CopyBTAction == 
                DialogCopyBTConflict.Actions.kReplaceTarget);
            for (int i = 0; i < PTarget.Runs.Count; i++)
            {
                DRun RTarget = PTarget.Runs[i] as DRun;
                DRun RFront = PFront.Runs[i] as DRun;

                RTarget.CopyBackTranslationsFromFront(RFront, bReplaceTarget);
            }

            PTarget.Cleanup();

            return true;
        }
        #endregion
        #region Method: bool CopySection(DSection SFront, DSection STarget)
        bool CopySection(DSection SFront, DSection STarget)
        {
            bool bCompletelySuccessful = true;

            // Get the section's structure; (they should be identical)
            Debug.Assert(STarget.SectionStructure == SFront.SectionStructure);
            int cParts = STarget.SectionStructure.Length;

            // Go through the paragraphs, dividing into chunks based on the SectionStructure.
            // (This strategy allows some items to be copied when paragraphs do not match,
            // as some chunks may match while others might not.
            for (int n = 0; n < cParts; n++)
            {
                // Get the paragraphs in this chunk
                DParagraph[] vpFront = GetParagraphsFromPart(SFront, n);
                DParagraph[] vpTarget = GetParagraphsFromPart(STarget, n);

                // We need to have the same count of paragraphs, and the same styles
                // in order to copy this chunk
                if (vpFront.Length != vpTarget.Length)
                {
                    bCompletelySuccessful = false;
                    continue;
                }
                bool bChunkIsIdentical = true;
                for (int i = 0; i < vpFront.Length; i++)
                {
                    if (vpFront[i].StyleAbbrev != vpTarget[i].StyleAbbrev)
                    {
                        bChunkIsIdentical = false;
                        continue;
                    }
                }
                if (bChunkIsIdentical)
                {
                    for (int i = 0; i < vpFront.Length; i++)
                    {
                        if (DB.Map.StyleCrossRef != vpFront[i].StyleAbbrev)
                            CopyParagraph(vpFront[i], vpTarget[i]);
                    }
                }
                else
                    bCompletelySuccessful = false;
            }

            // Footnotes: copy provided we have the same count and styles
            if (DoFootnotesMatch(SFront, STarget))
            {
                for (int i = 0; i < STarget.Footnotes.Count; i++)
                {
                    DFootnote FFront = SFront.Footnotes[i] as DFootnote;
                    DFootnote FTarget = STarget.Footnotes[i] as DFootnote;

                    if (FFront.NoteType == DFootnote.Types.kSeeAlso)
                        continue;

                    CopyParagraph(FFront, FTarget);

                    if (DialogCopyBTConflict.Actions.kCancel == DialogCopyBTConflict.CopyBTAction)
                        return false;
                }
            }
            else
                bCompletelySuccessful = false;

            return bCompletelySuccessful;
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Constructor(bEntireBook)
        public CopyBtMethod(bool bEntireBook)
        {
            m_bEntireBook = bEntireBook;
        }
        #endregion
        #region Method: void Run()
        public void Run()
        {
            // Display a very intense warning to the user.
            DisplaySternWarning();

            // Set the default actions to take
            DialogCopyBTConflict.ApplyToAll = false;
            DialogCopyBTConflict.CopyBTAction = DialogCopyBTConflict.Actions.kAppendToTarget;

            // Do the copy for each section in the book
            for (int i = 0; i < DB.FrontBook.Sections.Count; i++)
            {
                DSection SFront = DB.FrontBook.Sections[i];
                DSection STarget = DB.TargetBook.Sections[i];

                // If we're only doing the current section, then skip all other sections
                if (!EntireBook && STarget != DB.TargetSection)
                    continue;

                // Do the copy (displaying the Conflicts Dialog if necessary)
                bool bOK = CopySection(SFront, STarget);

                // Did the user abort during the Conflicts Dialog?
                if (DialogCopyBTConflict.Actions.kCancel == DialogCopyBTConflict.CopyBTAction)
                    return;

                // Error if we had mismatched sections
                if (!bOK)
                    DisplayMismatchedSectionsWarning();
            }
        }
        #endregion
    }



}