#region ***** CopyBTfromFront.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    CopyBTfromFront.cs
 * Author:  John Wimbish
 * Created: 16 Mar 2006
 * Purpose: Dialog and method to copy the back translations from the front to the daughter
 * Legal:   Copyright (c) 2004-11, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;
#endregion

namespace OurWord.Dialogs
{
	public partial class CopyBTfromFront : Form
	{
		// Scaffolding -----------------------------------------------------------------------
        readonly bool m_bEntireBook; // If F, then we're just doing the Current Section
		#region Constructor(bEntireBook)
		public CopyBTfromFront(bool bEntireBook)
		{
			m_bEntireBook = bEntireBook;

			// Required for Windows Form Designer support
			InitializeComponent();
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, EventArgs e)
		{
			// Display the icon
			m_WarningIcon.Image = SystemIcons.Warning.ToBitmap();

			// Scope Message
            m_labelScope.Text = m_bEntireBook ?
				"The Entire Book will be copied." :
				"Only the Current Section will be copied.";

			// Craft the message to display the names of the languages involved
			var sSummary = "This process will copy the back translation from " +
				DB.Project.FrontTranslation.DisplayName + " " +
				DB.Project.SFront.Book.DisplayName + " to " +
				DB.Project.TargetTranslation.DisplayName + " " +
				DB.Project.STarget.Book.DisplayName + ".";
			m_labelSynopsis.Text = sSummary;
		}
		#endregion
	}

    class CopyBtException : Exception
    {
        public readonly bool ProceedAnyway;
        #region Constructor(sMessage, bProceedAnyway)
        public CopyBtException(string sMessage, bool bProceedAnyway)
            : base(sMessage)
        {
            ProceedAnyway = bProceedAnyway;
        }
        #endregion
    }

    public class CopyBtFromFrontMethod
    {
        private readonly DSection m_FrontSection;
        private readonly DSection m_TargetSection;
        private bool m_bCheckForErrorsOnly;
        private bool m_bBtAlreadyHasMaterial;

        // Messages --------------------------------------------------------------------------
        #region method: bool HasError_ParagraphCount(vpFront, vpTarget)
        bool HasError_ParagraphCount(IList<DParagraph> vpFront, IList<DParagraph> vpTarget)
        {
            if (vpFront.Count == vpTarget.Count)
                return false;

            if (m_bCheckForErrorsOnly)
            {
                var sRef = (vpFront.Count > 0) ?
                    vpFront[0].ReferenceSpan.Start.FullName :
                    vpTarget[0].ReferenceSpan.Start.FullName;
                var bProceed = LocDB.Message("msgCopyBt_ParagraphMismatch",
                   "There is a mismatch between the number of paragraphs in '{0}' and '{1}'; " +
                   "The problem is near {2}.\n\n" +
                   "OurWord will probably not be able to completely copy between the two.\n\n" + 
                   "Do you wish to ignore this and any other errors and attempt the copy anyway?",
                   new[] { m_FrontSection.Translation.DisplayName, 
                       m_TargetSection.Translation.DisplayName, sRef },
                   LocDB.MessageTypes.WarningYN);
                throw new CopyBtException("Paragraph Mismatch", bProceed);
            }

            return true;
        }
        #endregion
        #region method: bool HasError_RunCount(vtFront, vtTarget)
        bool HasError_RunCount(IList<DBasicText> vtFront, IList<DBasicText> vtTarget)
        {
            if (vtFront.Count == vtTarget.Count)
                return false;

            if (m_bCheckForErrorsOnly)
            {
                var sRef = (vtFront.Count > 0) ?
                   vtFront[0].Paragraph.ReferenceSpan.Start.FullName :
                   vtTarget[0].Paragraph.ReferenceSpan.Start.FullName;
                var bProceed = LocDB.Message("msgCopyBt_TextMismatch",
                   "There is a mismatch between the number of paragraph elements in '{0}'" +
                   "and '{1}'; The problem is in the paragraphs near {2}.\n\n" +
                   "OurWord will not be able to completely copy between the two.\n\n" +
                   "Do you wish to ignore this and any other errors and attempt the copy anyway?",
                   new[] { m_FrontSection.Translation.DisplayName, 
                       m_TargetSection.Translation.DisplayName, sRef },
                   LocDB.MessageTypes.WarningYN);
                throw new CopyBtException("Text Mismatch", bProceed);
            }

            return true;
        }
        #endregion
        #region method: void IssueWarning_IfMaterialAlreadyInBt(sProseBtAsString)
        void IssueWarning_IfMaterialAlreadyInBt(string sProseBtAsString)
        {
            if (string.IsNullOrEmpty(sProseBtAsString))
                return;

            m_bBtAlreadyHasMaterial = true;
        }
        #endregion
        #region method: bool HasError_FootnoteCount(vfnFront, vfnTarget)
        bool HasError_FootnoteCount(ICollection<DFootnote> vfnFront, ICollection<DFootnote> vfnTarget)
        {
            if (vfnFront.Count == vfnTarget.Count)
                return false;

            if (m_bCheckForErrorsOnly)
            {
                var bProceed = LocDB.Message("msgCopyBt_FootnoteMismatch",
                   "There is a mismatch between the number of footnotes in '{0}' and '{1}';\n\n" +
                   "OurWord will not be able to completely copy between the two.\n\n" +
                   "Do you wish to ignore this and any other errors and attempt the copy anyway?",
                   new[] { m_FrontSection.Translation.DisplayName, 
                       m_TargetSection.Translation.DisplayName },
                   LocDB.MessageTypes.WarningYN);
                throw new CopyBtException("Footnote Mismatch", bProceed);
            }

            return true;
        }
        #endregion

        // Copy Methods ----------------------------------------------------------------------
        #region method: void CopyParagraph(DParagraph PFront, DParagraph PTarget)
        void CopyParagraph(DParagraph pFront, DParagraph pTarget)
        {
            // We want the same count of editable runs in the paragraph
            var vFrontTexts = GetEditableRuns(pFront);
            var vTargetTexts = GetEditableRuns(pTarget);
            if (HasError_RunCount(vFrontTexts, vTargetTexts))
                return;

            for (var i = 0; i < vFrontTexts.Count; i++)
            {
                var textFront = vFrontTexts[i];
                var textTarget = vTargetTexts[i];

                // Is there something to copy?
                if (string.IsNullOrEmpty(textFront.ProseBTAsString))
                    continue;

                // Is there material already there?
                IssueWarning_IfMaterialAlreadyInBt(textTarget.ProseBTAsString);

                // Do the copy
                if (!m_bCheckForErrorsOnly)
                {
                    const bool bReplaceTarget = false;
                    textTarget.CopyBackTranslationsFromFront(textFront, bReplaceTarget);
                }
            }
        }
        #endregion
        #region method: void Copy()
        void Copy()
        {
            // Should be the same; else the sections would not have loaded
            Debug.Assert(m_FrontSection.SectionStructure == m_TargetSection.SectionStructure);
            var cSectionStructureParts = m_TargetSection.SectionStructure.Length;

            // Flag that will tell us if the destination BT already has material in it
            m_bBtAlreadyHasMaterial = false;

            // Go through the major parts, dividing into chunks
            for (var n = 0; n < cSectionStructureParts; n++)
            {
                var vFrontParas = GetParagraphsFromPart(m_FrontSection, n);
                var vTargetParas = GetParagraphsFromPart(m_TargetSection, n);
                if (HasError_ParagraphCount(vFrontParas, vTargetParas))
                    continue;
                for (var i = 0; i < vFrontParas.Count; i++)
                    CopyParagraph(vFrontParas[i], vTargetParas[i]);
            }

            // Footnotes
            var vFrontFootnotes = GetEditableFootnotes(m_FrontSection);
            var vTargetFootnotes = GetEditableFootnotes(m_TargetSection);
            if (HasError_FootnoteCount(vFrontFootnotes, vTargetFootnotes))
                return;
            for (var i = 0; i < vFrontFootnotes.Count; i++)
                CopyParagraph(vFrontFootnotes[i], vTargetFootnotes[i]);
        }
        #endregion

        // Work Methods ----------------------------------------------------------------------
        #region sattr{g/s}: string SternWarningSeen
        private static bool SternWarningSeen
        {
            get
            {
                return JW_Registry.GetValue(c_sSternWarningSeen, false);
            }
            set
            {
                JW_Registry.SetValue(c_sSternWarningSeen, value);
            }
        }
        const string c_sSternWarningSeen = "CopyBtSternWarningSeen";
        #endregion
        #region smethod: bool DisplaySternWarning()
        static bool DisplaySternWarning()
        // returns false if the user wants to abort
        {
            if (!SternWarningSeen)
            {
                var dlg = new CopyBTfromFront(false);
                if (DialogResult.OK != dlg.ShowDialog())
                    return false;
                SternWarningSeen = true;
            }

            return true;
        }
        #endregion
        #region smethod: char GetParagraphType(DParagraph)
        static char GetParagraphType(DParagraph p)
        {
            var ch = DSection.c_Text;
            if (p as DPicture != null)
                ch = DSection.c_Pict;
            return ch;
        }
        #endregion
        #region smethod: List<DParagraph> GetParagraphsFromPart(DSection, nPart)
        static List<DParagraph> GetParagraphsFromPart(DSection section, int nPart)
        {
            var iPara = 0;

            var sSectionStructure = section.SectionStructure;

            // Move past the preceeding parts of the section
            for (var n = 0; n < nPart; n++)
            {
                var chPartType = sSectionStructure[n];
                while (iPara < section.Paragraphs.Count)
                {
                    if (chPartType != GetParagraphType(section.Paragraphs[iPara]))
                        break;
                    ++iPara;
                }
            }

            // Now add the desired paragraphs to the list
            var v = new List<DParagraph>();
            var chDesiredPartType = sSectionStructure[nPart];
            while (iPara < section.Paragraphs.Count)
            {
                var p = section.Paragraphs[iPara];
                if (chDesiredPartType != GetParagraphType(p))
                    break;
                v.Add(p);
                ++iPara;
            }

            return v;
        }
        #endregion
        #region smethod: List<DBasicText> GetEditableRuns(DParagraph p)
        static List<DBasicText> GetEditableRuns(DParagraph p)
        {
            var v = new List<DBasicText>();

            foreach (DRun run in p.Runs)
            {
                if (null != run as DBasicText)
                    v.Add(run as DBasicText);
            }

            return v;
        }
        #endregion
        #region smethod: List<DFootnote> GetEditableFootnotes(DSection)
        static List<DFootnote> GetEditableFootnotes(DSection section)
        {
            var v = new List<DFootnote>();

            var vAll = section.AllFootnotes;

            foreach (var foot in vAll)
            {
                if (foot.IsExplanatory)
                    v.Add(foot);
            }

            return v;
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Constructor(FrontSection, TargetSection)
        public CopyBtFromFrontMethod(DSection front, DSection target)
        {
            m_FrontSection = front;
            m_TargetSection = target;
        }
        #endregion
        #region Method: void Run()
        public void Run()
        {
            // Display a very intense warning to the user.
            if (!DisplaySternWarning())
                return;

            // First pass: Check for errors. Structural Errors throw an exception and halt
            // further checking; whereas material already in the BT sets m_bBtAlreadyHasMaterial.
            m_bCheckForErrorsOnly = true;
            var bProceed = true;
            try
            {
                Copy();

                // This is not a structural mismatch, so we treat it as more of a warning
                // here rather than as a major warning in the exception section.
                if (m_bBtAlreadyHasMaterial)
                {
                    bProceed = LocDB.Message("msgCopyBt_MaterialAlreadyPresent",
                        "The '{0}' back translation already has material in it. If you continue, " +
                        "the '{1}' back translation will be appended to the existing material.\n\n" +                        
                        "Do you still wish to proceed?",
                        new[] {m_TargetSection.Translation.DisplayName,
                            m_FrontSection.Translation.DisplayName},
                        LocDB.MessageTypes.WarningYN);
                }
            }
            catch (CopyBtException e)
                // If here, there was a structural error, and copying will be incomplete
            {
                bProceed = e.ProceedAnyway;
            }
            if (!bProceed)
                return;

            // Second pass: Do it
            m_bCheckForErrorsOnly = false;
            Copy();

        }
        #endregion
    }

}
