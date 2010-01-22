#region ***** Drafting.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\Drafting.cs
 * Author:  John Wimbish
 * Created: 17 Jan 2004
 * Purpose: Manages the Drafting view.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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

using OurWord.Edit;
using OurWord.Layouts;
using OurWordData;
using OurWordData.DataModel;
using JWTools;
#endregion
#endregion

namespace OurWord.Layouts
{
    public class WndDrafting : WLayout
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
        #region OAttr{g}: string LayoutName
        public override string LayoutName
        {
            get
            {
                return c_sName;
            }
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
                if (!DB.IsValidProject)
                    return "";
                if (null == DB.Project.TargetTranslation)
                    return "";

                string sBase = G.GetLoc_GeneralUI("DraftingReference", "{0} to {1}");

                string sFrontName = (null == DB.FrontTranslation) ?
                    G.GetLoc_GeneralUI("NoFrontDefined", "(no front defined)") :
                    DB.FrontTranslation.DisplayName;

                string sTargetName = (null == DB.TargetTranslation) ?
                    G.GetLoc_GeneralUI("NoTargetDefined", "(no target defined)") :
                    DB.TargetTranslation.DisplayName.ToUpper();

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
                DParagraph pF = DB.FrontSection.Paragraphs[iF] as DParagraph;

                // Get its initial verse number. If it doesn't have a DVerse in it, then
                // we look to the next front paragraph.
                int nVerseFront = pF.FirstActualVerseNumber;
                if (0 == nVerseFront)
                    continue;

                // Now loop through the Target paragraphs that we haven't handled yet
                for (int iT = pairLast.iTarget + 1; iT < iTarget + cTarget; iT++)
                {
                    // Get the target paragraph
                    DParagraph pT = DB.TargetSection.Paragraphs[iT] as DParagraph;

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
                EColumn colFront;
                EColumn colTarget;
                CreateRow(Contents, out colFront, out colTarget, false);

                for (int kF = 0; kF < pair.cFront; kF++)
                {
                    DParagraph pFront = DB.FrontSection.Paragraphs[ pair.iFront + kF ];
                    colFront.Append( CreateFrontPara(pFront) );
                }
                for (int kT = 0; kT < pair.cTarget; kT++)
                {
                    DParagraph pTarget = DB.TargetSection.Paragraphs[ pair.iTarget + kT ];
                    pTarget.BestGuessAtInsertingTextPositions();
                    colTarget.Append( CreateTargetPara(pTarget, true) );
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
                DParagraph pFront = DB.FrontSection.Paragraphs[iFront + i] as DParagraph;
                DParagraph pTarget = DB.TargetSection.Paragraphs[iTarget + i] as DParagraph;

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
            var FrontFootnotes = DB.FrontSection.AllFootnotes;
            var TargetFootnotes = DB.TargetSection.AllFootnotes;

            // If the two sections do not have the same number of footnotes, then
            // they cannot appear in the side-by-side mode.
            if (FrontFootnotes.Count != TargetFootnotes.Count)
                return false;

            // If the individual footnotes do not have the same types, then 
            // they cannot appear in the side-by-side mode.
            for (int i = 0; i < TargetFootnotes.Count; i++)
            {
                if (FrontFootnotes[i].NoteType != TargetFootnotes[i].NoteType)
                    return false;
            }

            // If we got here, then side-by-side is OK.
            return true;
        }
        #endregion

        #region Method: OWPara CreateFrontPara(DParagraph)
        OWPara CreateFrontPara(DParagraph p)
        {
            if (null != p as DFootnote)
            {
                return CreateFootnotePara(p as DFootnote,
                    p.Translation.WritingSystemVernacular,
                    BackColor, 
                    OWPara.Flags.None);
            }
            else
            {
                return new OWPara(
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    BackColor,
                    OWPara.Flags.None);
            }
        }
        #endregion
        #region Method: OWPara CreateTargetPara(DParagrap, bAllowItalics)
        OWPara CreateTargetPara(DParagraph p, bool bAllowItalics)
        {
            // Options for paragraphs that will be on the right-hand, editable side
            var DraftingOptions = OWPara.Flags.None;
            if (p.IsUserEditable)
            {
                DraftingOptions = OWPara.Flags.IsEditable;
                if (OurWordMain.s_Features.F_StructuralEditing && null == p as DFootnote)
                    DraftingOptions |= OWPara.Flags.CanRestructureParagraphs;
                if (OurWordMain.TargetIsLocked)
                    DraftingOptions |= OWPara.Flags.IsLocked;
                if (bAllowItalics)
                    DraftingOptions |= OWPara.Flags.CanItalic;
            }

            // Background Color
            Color color = EditableBackgroundColor;
            if (OurWordMain.TargetIsLocked || !p.IsUserEditable)
                color = BackColor;

            // Add the paragraph
            if (null != p as DFootnote)
            {
                return CreateFootnotePara(p as DFootnote, 
                    p.Translation.WritingSystemVernacular,
                    color, 
                    DraftingOptions);
            }
            else
            {
                return new OWPara(
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    color,
                    DraftingOptions);
            }
        }
        #endregion
        #region Method: ERowOfColumns CreateRow(Container, out colLeft, out colRight, bIncludeSeparator)
        static public ERowOfColumns CreateRow(EContainer Container, out EColumn colLeft, out EColumn colRight, bool bIncludeSeparator)
        {
            // A single row with two columns
            ERowOfColumns row = new ERowOfColumns(2);
            Container.Append(row);
            colLeft = new EColumn();
            colRight = new EColumn();
            row.Append(colLeft);
            row.Append(colRight);

            // Footnote separator horizontal lines
            if (bIncludeSeparator)
            {
                colLeft.Border = new EContainer.FootnoteSeparatorBorder(colLeft, 
                    c_nFootnoteSeparatorWidth);
                colRight.Border = new EContainer.FootnoteSeparatorBorder(colRight, 
                    c_nFootnoteSeparatorWidth);
            }

            return row;
        }
        #endregion

        #region Method: void LoadFootnotes()
        void LoadFootnotes()
        {
            var FrontFootnotes = DB.FrontSection.AllFootnotes;
            var TargetFootnotes = DB.TargetSection.AllFootnotes;

            // Anything to load?
            if (FrontFootnotes.Count == 0 && TargetFootnotes.Count == 0)
                return;

            // Load them all in a single row if we must
            if (!_CanSideBySideFootnotes())
            {
                // A single row with two columns
                EColumn colFront;
                EColumn colTarget;
                CreateRow(Contents, out colFront, out colTarget, true);

                // All Front Footnotes
                foreach (var f in FrontFootnotes)
                    colFront.Append(CreateFrontPara(f));

                // All Target Footnotes
                foreach (var f in TargetFootnotes)
                    colTarget.Append(CreateTargetPara(f, true));

                return;
            }

            // Otherwise, they are on individual parallel rows
            for (var k = 0; k < FrontFootnotes.Count; k++)
            {
                // A single row with two columns
                EColumn colFront;
                EColumn colTarget;
                CreateRow(Contents, out colFront, out colTarget, k == 0);

                // Ensure a place to type
                TargetFootnotes[k].SynchRunsToModelParagraph(FrontFootnotes[k]);

                // Append the two
                colFront.Append( CreateFrontPara( FrontFootnotes[k] ));
                colTarget.Append(CreateTargetPara(TargetFootnotes[k], FrontFootnotes[k].HasItalicsToggled));
            }
        }
        #endregion

        #region Method: override void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!DB.Project.HasDataToDisplay)
                return;

            // Load the paragraphs, then the footnotes
            LoadParagraphs();
            LoadFootnotes();

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
        #region Method: void LoadParagraphs()
        void LoadParagraphs()
        {
            // We'll work our way through both the Front and the Target paragraphs
            var iFront = 0;
            var iTarget = 0;

            // Loop until all paragraphs have been accounted for
            while (iFront < DB.FrontSection.Paragraphs.Count ||
                   iTarget < DB.TargetSection.Paragraphs.Count)
            {
                // If we are missing either a Front or a Target, we need to
                // add one so that something will get displayed.
                if (iFront == DB.FrontSection.Paragraphs.Count)
                {
                    var pNew = new DParagraph {AddedByCluster = true};
                    DB.FrontSection.Paragraphs.Append(pNew);
                }
                if (iTarget == DB.TargetSection.Paragraphs.Count)
                {
                    var pNew = new DParagraph {AddedByCluster = true};
                    DB.TargetSection.Paragraphs.Append(pNew);
                }

                // We use Pictures to help us re-allign paragraphs; thus, we count 
                // the number of paragraphs in both Front and Target that are the
                // same type.
                var cFront = _CountMatchingParagraphTypes(iFront, DB.FrontSection.Paragraphs);
                var cTarget = _CountMatchingParagraphTypes(iTarget, DB.TargetSection.Paragraphs);

                if (!_CanSideBySideParagraphs(iFront, cFront, iTarget, cTarget))
                {
                    LoadMismatchedParagraphs(iFront, cFront, iTarget, cTarget);
                }
                else
                {
                    for (var k = 0; k < cFront; k++)
                    {                       
                        // Start the row
                        EColumn colFront;
                        EColumn colTarget;
                        var row = CreateRow(Contents, out colFront, out colTarget, false);
                        row.SetPicture(
                            GetPicture(DB.TargetSection.Paragraphs[iTarget + k]), 
                            true);

                        // Synchronize the Vernacular to the Target
                        var pFront = DB.FrontSection.Paragraphs[iFront + k];
                        var pTarget = DB.TargetSection.Paragraphs[iTarget + k];
                        pTarget.SynchRunsToModelParagraph(pFront);

                        // If we have no content in the Front and Target, then we don't add the paragraphs.
                        // (E.g., a picture with no caption, or a spurious paragraph marker in the Front)
                        if (pFront.SimpleText.Length == 0 && pTarget.SimpleText.Length == 0)
                            continue;

                        // Add the left and right paragraphs
                        colFront.Append(CreateFrontPara(pFront));
                        colTarget.Append(CreateTargetPara(pTarget, pFront.HasItalicsToggled));
                    }
                }

                // Increment for the next bunch of paragraphs
                iFront += cFront;
                iTarget += cTarget;
            }
        }
        #endregion
        #region OMethod: ENote.Flags GetNoteContext(note, OWPara.Flags)
        public override ENote.Flags GetNoteContext(TranslatorNote note, OWPara.Flags ParagraphFlags)
        {
            // Front Translation (which will be the Vernacular by definition) we are only 
            // interested in HintForDrafting notes. 
            // - Not editable (info for the MTT)
            // + Bright icon to draw user's attention
            if (note.IsFrontTranslationNote && note.Behavior == TranslatorNote.HintForDrafting)
                return ENote.Flags.DisplayMeIcon | ENote.Flags.FirstMessageOnly;

            // In the Target Translation, just show the General notes (e.g., what's for use by
            // the MTT, as opposed to stuff for the consultant.)
            // + Editable (user action desired)
            if (note.IsTargetTranslationNote && note.Behavior == TranslatorNote.General)
                return ENote.Flags.UserEditable;

            return ENote.Flags.None;
        }
        #endregion
    }

}
