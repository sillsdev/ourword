/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\Drafting.cs
 * Author:  John Wimbish
 * Created: 17 Jan 2004
 * Purpose: Manages the Drafting view.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using OurWord.DataModel;
using OurWord.Edit;
using OurWord.View;
using JWdb;
using JWTools;
#endregion

namespace OurWord.View
{
    public class WndDrafting : OWWindow
    {
        // Registry-Stored Settings ----------------------------------------------------------
        public const string c_sName = "Draft";
        #region SAttr{g/s}: string RegistryBackgroundColor - background color for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "Wheat");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const int c_cColumnCount = 2;
        #region Constructor()
        public WndDrafting()
            : base(c_sName, c_cColumnCount)
        {
            // We want to maintain text below the cursor so the user does not think
            // there is nothing below when there actually is.
            ScrollPositionBufferMargin = 50;

            // Background color for the window; default is "Linen"
            BackColor = Color.Wheat;

            // Background color for those parts that are editable
            EditableBackgroundColor = Color.White;
        }
        #endregion
        #region Cmd: OnGotFocus - make sure commands are properly enabled
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            G.App.EnableMenusAndToolbars();
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("DraftingWindowName", "Drafting");
            }
        }
        #endregion
        #region Attr{g}: override string LanguageInfo
        public override string LanguageInfo
        {
            get
            {
                if (!G.IsValidProject)
                    return "";
                if (null == OurWordMain.Project.TargetTranslation)
                    return "";

                string sBase = G.GetLoc_GeneralUI("DraftingReference", "{0} to {1}");

                string sFrontName = (null == G.FTranslation) ?
                    G.GetLoc_GeneralUI("NoFrontDefined", "(no front defined)") :
                    G.FTranslation.DisplayName;

                string sTargetName = (null == G.TTranslation) ?
                    G.GetLoc_GeneralUI("NoTargetDefined", "(no target defined)") :
                    G.TTranslation.DisplayName.ToUpper();

                return LocDB.Insert(sBase, new string[] { sFrontName, sTargetName });
            }
        }
        #endregion

        // Deal with mismatched paragraphs (e.g., Front doesn't match Target)-----------------
        #region CLASS: ParagraphAlignmentPair
        public class ParagraphAlignmentPair
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: int VerseNo - the first verse in the para; same in both front and target
            public int VerseNo
            {
                get
                {
                    return m_nVerseNo;
                }
            }
            int m_nVerseNo;
            #endregion
            #region Attr{g}: int iFront - index of first Front paragraph for this row
            public int iFront
            {
                get
                {
                    return m_iFront;
                }
            }
            int m_iFront;
            #endregion
            #region Attr{g}: int iTarget - index of first Target paragraph for this row
            public int iTarget
            {
                get
                {
                    return m_iTarget;
                }
            }
            int m_iTarget;
            #endregion

            #region Attr{g}: int cFront - count of how many front paragraphs go in this row
            public int cFront
            {
                get
                {
                    return m_cFront;
                }
                set
                {
                    m_cFront = value;
                }
            }
            int m_cFront;
            #endregion
            #region Attr{g}: int cTarget - count of how many target paragraphs go in this row
            public int cTarget
            {
                get
                {
                    return m_cTarget;
                }
                set
                {
                    m_cTarget = value;
                }
            }
            int m_cTarget;
            #endregion

            // Scaffolding ------------------------------------------------------------------
            #region Constructor(nVerseNo, iFront, iTarget)
            public ParagraphAlignmentPair(int _nVerseNo, int _iFfront, int _iTarget)
            {
                m_nVerseNo = _nVerseNo;

                m_iFront = _iFfront;
                m_iTarget = _iTarget;

                m_cFront = 1;
                m_cTarget = 1;
            }
            #endregion
            #region VAttr{g}: void DebugString
            public string DebugString
            {
                get
                {
                    return "VerseNo=" + VerseNo.ToString() +
                        "      iFront=" + iFront.ToString() + "  c=" + cFront.ToString() +
                        "      iTarget=" + iTarget.ToString() + "  c=" + cTarget.ToString();
                }
            }
            #endregion
        }
        #endregion
        #region SMethod: ParagraphAlignmentPair[] ScanForAlignmentPairs(...)
        static public ParagraphAlignmentPair[] ScanForAlignmentPairs(
            int iFront, int cFront, int iTarget, int cTarget)
        {
            ArrayList a = new ArrayList();

            // By definition, the first paragraphs align, even if no verse is present
            ParagraphAlignmentPair pairLast = new ParagraphAlignmentPair(0, iFront, iTarget);
            a.Add(pairLast);

            // Loop to create the list of alignment pairs
            // Begin with the next Front paragraph, and loop through the total count
            for(int iF = iFront + 1; iF < iFront + cFront; iF++)
            {
                // Get the next front paragraph
                DParagraph pF = G.SFront.Paragraphs[iF] as DParagraph;

                // Get its initial verse number. If it doesn't have a DVerse in it, then
                // we look to the next front paragraph.
                int nVerseFront = pF.FirstActualVerseNumber;
                if (0 == nVerseFront)
                    continue;

                // Now loop through the Target paragraphs that we haven't handled yet
                for (int iT = pairLast.iTarget + 1; iT < iTarget + cTarget; iT++)
                {
                    // Get the target paragraph
                    DParagraph pT = G.STarget.Paragraphs[iT] as DParagraph;

                    // Get its initial verse number, if any
                    int nVerseTarget = pT.FirstActualVerseNumber;

                    // If the two paragraph's verses are the same, then we have an alignment pair
                    if (nVerseFront == nVerseTarget)
                    {
                        pairLast = new ParagraphAlignmentPair(nVerseTarget, iF, iT);
                        a.Add(pairLast);
                        break;
                    }
                }
            }

            // Convert to a vector
            ParagraphAlignmentPair[] v = new ParagraphAlignmentPair[a.Count];
            for (int k = 0; k < a.Count; k++)
                v[k] = a[k] as ParagraphAlignmentPair;
            return v;
        }
        #endregion
        #region SMethod: void AddAlignmentCounts(...)
        static public void AddAlignmentCounts(ParagraphAlignmentPair[] vPairs, int cFront, int cTarget)
        {
            for (int i = 0; i < vPairs.Length - 1; i++)
            {
                ParagraphAlignmentPair pair = vPairs[i];
                ParagraphAlignmentPair next = vPairs[i + 1];

                pair.cFront = next.iFront - pair.iFront;
                pair.cTarget = next.iTarget - pair.iTarget;

                cFront -= pair.cFront;
                cTarget -= pair.cTarget;
            }

            ParagraphAlignmentPair last = vPairs[vPairs.Length - 1];
            last.cFront = cFront;
            last.cTarget = cTarget;
        }
        #endregion
        #region Method: void LoadMismatchedParagraphs(int iFront, int cFront, int iTarget, int cTarget)
        void LoadMismatchedParagraphs(int iFront, int cFront, int iTarget, int cTarget)
        {
            // Get the raw alignment pairs
            ParagraphAlignmentPair[] vPairs = ScanForAlignmentPairs(iFront, cFront, iTarget, cTarget);

            // Each pair needs the count of paragraphs that go with it
            AddAlignmentCounts(vPairs, cFront, cTarget);

            //  Create the rows according to the alignment pairs
            foreach (ParagraphAlignmentPair pair in vPairs)
            {
                StartNewRow();
                for (int kF = 0; kF < pair.cFront; kF++)
                {
                    DParagraph pFront = G.SFront.Paragraphs[ pair.iFront + kF ] as DParagraph;
                    _LoadHintsFromFront(pFront);
                    AddFrontParagraph(pFront);
                }
                for (int kT = 0; kT < pair.cTarget; kT++)
                {
                    DParagraph pTarget = G.STarget.Paragraphs[ pair.iTarget + kT ] as DParagraph;
                    pTarget.BestGuessAtInsertingTextPositions();
                    AddTargetParagraph(pTarget, true);
                }
            }
        }
        #endregion

        // Create the Window Contents from the data ------------------------------------------
        const int c_xMaxPictureWidth = 300;
        #region Method: int _CountMatchingParagraphTypes(int iStart, JOwnSeq vParagraphs)
        int _CountMatchingParagraphTypes(int iStart, JOwnSeq<DParagraph> vParagraphs)
        {
            if (iStart >= vParagraphs.Count)
                return 0;

            // Determine what the first paragraph's type is
            DParagraph p = vParagraphs[iStart] as DParagraph;
            bool bIsFootnote = ((p as DFootnote) != null);
            bool bIsPicture  = ((p as DPicture) != null);
            bool bIsParagraph = (!bIsFootnote && !bIsPicture);

            // We'll count how many subsequent paragraphs match it; starting
            // with "1" as that is the initial paragraph
            int c = 1;

            // Loop until we run into a non-match
            while (iStart + c < vParagraphs.Count)
            {
                // Determine the type of the text paragraph
                DParagraph pTest = vParagraphs[iStart + c] as DParagraph;
                bool bTestIsFootnote = ((pTest as DFootnote) != null);
                bool bTestIsPicture = ((pTest as DPicture) != null);
                bool bTestIsParagraph = (!bTestIsFootnote && !bTestIsPicture);

                // Break if "p" is a picture but "pTest" is not.
                if (bIsPicture && !bTestIsPicture)
                    break;

                // Break if "p" is a footnote but "pTest" is not.
                if (bIsFootnote && !bTestIsFootnote)
                    break;

                // Break if "p" is a paragraph but "pTest" is not.
                if (bIsParagraph && !bTestIsParagraph)
                    break;

                c++;
            }

            return c;
        }
        #endregion
        #region Method: bool _CanSideBySideParagraphs(...) - Front & Target paragraphs correspond
        bool _CanSideBySideParagraphs(int iFront, int cFront, int iTarget, int cTarget)
        {
            // If the two "blocks" do not have the same count of paragraphs, then
            // they cannot appear in the side-by-side mode
            if (cFront != cTarget)
                return false;

            // If the individual paragraphs to not have the same 
            // 1. style, 
            // 2. verse reference
            // then they cannot appear in side-by-side mode.
            for (int i = 0; i < cFront; i++)
            {
                DParagraph pFront = G.SFront.Paragraphs[iFront + i] as DParagraph;
                DParagraph pTarget = G.STarget.Paragraphs[iTarget + i] as DParagraph;

                if (pFront.StyleAbbrev != pTarget.StyleAbbrev)
                    return false;
                if (!pFront.IsSameReferenceAs(pTarget))
                    return false;
            }

            // If we got here, then side-by-side is OK
            return true;
        }
        #endregion
        #region Method: bool _CanSideBySideFootnotes()
        bool _CanSideBySideFootnotes()
            // Determines if the footnotes can each have their own row, or if they all are
            // lumped together on a single row.
        {
            // If the two sections do not have the same number of footnotes, then
            // they cannot appear in the side-by-side mode.
            if (G.SFront.Footnotes.Count != G.STarget.Footnotes.Count)
                return false;

            // If the individual footnotes do not have the same types, then 
            // they cannot appear in the side-by-side mode.
            for (int i = 0; i < G.SFront.Footnotes.Count; i++)
            {
                DFootnote fnFront = G.SFront.Footnotes[i] as DFootnote;
                DFootnote fnTarget = G.STarget.Footnotes[i] as DFootnote;

                if (fnFront.NoteType != fnTarget.NoteType)
                    return false;
            }

            // If we got here, then side-by-side is OK.
            return true;
        }
        #endregion
        #region Method: _LoadHintsFromFront(DParagraph pFront)
        void _LoadHintsFromFront(DParagraph pFront)
        {
            foreach(DRun r in pFront.Runs)
            {
                DText text = r as DText;
                if (null == text)
                    continue;

                foreach (TranslatorNote tn in text.TranslatorNotes)
                {
                    if (tn.ShowInDaughterTranslations && tn.Show)
                        G.App.SideWindows.AddNote(tn);
                }
            }
        }
        #endregion
        #region Method: void _LoadFootnotes()
        void _LoadFootnotes()
        {
            // Anything to load?
            if (G.SFront.Footnotes.Count == 0 && G.STarget.Footnotes.Count == 0)
                return;

            // Load them all in a single row if we must
            if (!_CanSideBySideFootnotes())
            {
                StartNewRow(true);

                for (int kF = 0; kF < G.SFront.Footnotes.Count; kF++)
                    AddFrontFootnote(G.SFront.Footnotes[kF] as DFootnote);
                for (int kT = 0; kT < G.STarget.Footnotes.Count; kT++)
                    AddTargetFootnote(G.STarget.Footnotes[kT] as DFootnote, true);
                return;
            }

            // Otherwise, they are on individual parallel rows
            for (int k = 0; k < G.SFront.Footnotes.Count; k++)
            {
                StartNewRow( ((k == 0) ? true : false) );

                DFootnote fFront = G.SFront.Footnotes[k] as DFootnote;
                DFootnote fTarget = G.STarget.Footnotes[k] as DFootnote;
                fTarget.SynchRunsToModelParagraph(fFront);

                AddFrontFootnote(fFront);
                AddTargetFootnote(fTarget, fFront.HasItalics);
            }

        }
        #endregion
        #region Method: override void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!G.Project.HasDataToDisplay)
                return;

            // Load the paragraphs, then the footnotes
            _LoadParagraphs();
            _LoadFootnotes();

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
        #region Method: void _LoadParagraphs()
        void _LoadParagraphs()
        {
            // We'll work our way through both the Front and the Target paragraphs
            int iFront = 0;
            int iTarget = 0;

            // Loop until all paragraphs have been accounted for
            while (iFront < G.SFront.Paragraphs.Count ||
                   iTarget < G.STarget.Paragraphs.Count)
            {
                // If we are missing either a Front or a Target, we need to
                // add one so that something will get displayed.
                if (iFront == G.SFront.Paragraphs.Count)
                {
                    DParagraph pNew = new DParagraph();
                    pNew.AddedByCluster = true;
                    G.SFront.Paragraphs.Append(pNew);
                }
                if (iTarget == G.STarget.Paragraphs.Count)
                {
                    DParagraph pNew = new DParagraph();
                    pNew.AddedByCluster = true;
                    G.STarget.Paragraphs.Append(pNew);
                }

                // We use Pictures to help us re-allign paragraphs; thus, we count 
                // the number of paragraphs in both Front and Target that are the
                // same type.
                int cFront = _CountMatchingParagraphTypes(iFront, G.SFront.Paragraphs);
                int cTarget = _CountMatchingParagraphTypes(iTarget, G.STarget.Paragraphs);

                if (!_CanSideBySideParagraphs(iFront, cFront, iTarget, cTarget))
                {
                    LoadMismatchedParagraphs(iFront, cFront, iTarget, cTarget);
                }
                else
                {
                    for (int k = 0; k < cFront; k++)
                    {
                        // Retrieve the bitmap, if a picture is involved
                        Bitmap bmp = null;
                        DPicture pict = G.SFront.Paragraphs[iFront + k] as DPicture;
                        if (null != pict)
                            bmp = pict.GetBitmap(c_xMaxPictureWidth);
                        
                        // Start the row
                        EContainer container = StartNewRow(false);
                        container.Bmp = bmp;

                        // Synchronize the Vernacular to the Target
                        DParagraph pFront = G.SFront.Paragraphs[iFront + k] as DParagraph;
                        DParagraph pTarget = G.STarget.Paragraphs[iTarget + k] as DParagraph;
                        pTarget.SynchRunsToModelParagraph(pFront);

                        // If we have no content in the Front and Target, then we don't add the paragraphs.
                        // (E.g., a picture with no caption, or a spurious paragraph marker in the Front)
                        if (pFront.SimpleText.Length == 0 && pTarget.SimpleText.Length == 0)
                            continue;

                        // Add the left and right paragraphs
                        _LoadHintsFromFront(pFront);
                        AddFrontParagraph(pFront);
                        AddTargetParagraph(pTarget, pFront.HasItalics);
                    }
                }

                // Increment for the next bunch of paragraphs
                iFront += cFront;
                iTarget += cTarget;
            }
        }
        #endregion

        // Methods to create and add the display paragraphs ----------------------------------
        const int c_iColFront = 0;    // The Front translation goes in the first (left) column
        const int c_iColTarget = 1;   // The Target translation goes in the second (right) column
        #region Method: void AddFrontParagraph(DParagraph)
        void AddFrontParagraph(DParagraph pFront)
        {
            OWPara para = new OWPara(
                pFront.Translation.WritingSystemVernacular,
                pFront.Style,
                pFront,
                BackColor,
                OWPara.Flags.None);

            AddParagraph(c_iColFront, para);
        }
        #endregion
        #region Method: void AddFrontFootnote(DFootnote)
        void AddFrontFootnote(DFootnote fnFront)
        {
            AddParagraph(c_iColFront, new OWPara(
                fnFront.Translation.WritingSystemVernacular,
                fnFront.Style,
                fnFront,
                BackColor,
                OWPara.Flags.None));
        }
        #endregion
        #region Method: void AddTargetParagraph(DParagraph, bool AllowItalics)
        void AddTargetParagraph(DParagraph pTarget, bool AllowItalics)
        {
            // Options for paragraphs that will be on the right-hand, editable side
            OWPara.Flags DraftingOptions = OWPara.Flags.None;
            if (pTarget.IsUserEditable)
            {
                DraftingOptions = OWPara.Flags.IsEditable;
                if (OurWordMain.s_Features.F_StructuralEditing)
                    DraftingOptions |= OWPara.Flags.CanRestructureParagraphs;
                if (OurWordMain.TargetIsLocked)
                    DraftingOptions |= OWPara.Flags.IsLocked;
                if (AllowItalics)
                    DraftingOptions |= OWPara.Flags.CanItalic;
            }

            // Background Color
            Color color = EditableBackgroundColor;
            if (OurWordMain.TargetIsLocked || !pTarget.IsUserEditable)
                color = BackColor;

            // Add the paragraph
            AddParagraph(c_iColTarget, new OWPara(
                pTarget.Translation.WritingSystemVernacular,
                pTarget.Style,
                pTarget,
                color,
                DraftingOptions));
        }
        #endregion
        #region Method: void AddTargetFootnote(DFootnote, bAllowItalics)
        void AddTargetFootnote(DFootnote fnTarget, bool AllowItalics)
        {
            // Editing options
            OWPara.Flags DraftingOptions = OWPara.Flags.None;
            if (fnTarget.IsUserEditable)
            {
                DraftingOptions = OWPara.Flags.IsEditable;
                if (OurWordMain.TargetIsLocked)
                    DraftingOptions |= OWPara.Flags.IsLocked;
                if (AllowItalics)
                    DraftingOptions |= OWPara.Flags.CanItalic;
            }

            // Background Color
            Color color = EditableBackgroundColor;
            if (OurWordMain.TargetIsLocked || !fnTarget.IsUserEditable)
                color = BackColor;

            // Add the displayable paragraph
            AddParagraph(c_iColTarget, new OWPara(
                fnTarget.Translation.WritingSystemVernacular,
                fnTarget.Style,
                fnTarget,
                color,
                DraftingOptions));
        }
        #endregion


    }

}
