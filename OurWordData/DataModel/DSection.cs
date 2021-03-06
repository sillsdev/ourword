#region ***** DSection.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DSection.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles a section (passage) of Scripture
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using JWTools;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;
#endregion

namespace OurWordData.DataModel
{
	#region EMBEDDED CLASS: ReserveCharIO
	class ReserveCharIO
		// A double bar, "||", is how a verticle bar is represented in the file
		// as a literal "|". A single "|" is reserved to indicate a style or
		// a footnote. This class handles the conversion.
	{
		public const char c_VerticalBar = '|';
		public const char c_StyleBegin  = '{';
		public const char c_StyleEnd    = '}';

		#region Method: bool ToCluster(string sIn, ref int iPos, ref string sToCluster)
		static public bool ToCluster(string sIn, ref int iPos, ref string sToCluster)
		{
			// Possible braces
			if ( sIn[iPos] == c_StyleBegin || sIn[iPos] == c_StyleEnd)
			{
				if (iPos < sIn.Length - 1 && sIn[iPos] == sIn[iPos+1] )
				{
					// Add just one to the destination string
					sToCluster += sIn[iPos];

					// Increment the position past the doublet
					iPos += 2;

					// Tell the caller we found it
					return true;
				}
			}
			return false;
		}
		#endregion
		#region Method: bool FromCluster(string sFromCluster, ref int iPos, ref string sOut)
		static public bool FromCluster(string sFromCluster, ref int iPos, ref string sOut)
		{
			char ch = sFromCluster[iPos];
			if ( ch == c_StyleBegin || ch == c_StyleEnd )
			{
				sOut += ch;
				sOut += ch;

				iPos++;

				return true;
			}

			return false;
		}
		#endregion

		#region Method: bool ToMemory(string sFromDisk, ref int iPos, ref string sToProgram)
		static public bool ToMemory(string sFromDisk, ref int iPos, ref string sToProgram)
		{
			// Possible verticle bar
			if (sFromDisk[iPos] == c_VerticalBar )
			{
				if (iPos < sFromDisk.Length-1 && sFromDisk[iPos+1] == c_VerticalBar)
				{
					// Add just one to the destination string
					sToProgram += c_VerticalBar;

					// Increment the position past the doublet
					iPos += 2;

					// Tell the caller we found it
					return true;
				}
			}

			// Literal braces; we convert it to a doublet; the cluster code will
			// convert it back to a singlet for user typing. Braces are only used
			// internally, and so we don't want the db file to need to do anything
			// special.
			if (sFromDisk[iPos] == c_StyleBegin || sFromDisk[iPos] == c_StyleEnd)
			{
				// Add two to the destination string
				sToProgram += sFromDisk[iPos];
				sToProgram += sFromDisk[iPos];

				// Increment the position past the singlet
				iPos++;

				// Tell the caller we found it
				return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FromMemory(string sFromProgram, ref int iPos, ref string sToDisk)
		static public bool FromMemory(string sFromProgram, ref int iPos, ref string sToDisk)
		{
			// Verticle bar: double it
			if ( sFromProgram[iPos] == c_VerticalBar)
			{
				sToDisk += "||";
				iPos++;
				return true;
			}

			// Literal braces; we convert the doublet into a singlet for storage in the
			// db file.
			if (sFromProgram[iPos] == c_StyleBegin || sFromProgram[iPos] == c_StyleEnd)
			{
				if (iPos < sFromProgram.Length-1 && sFromProgram[iPos+1] == sFromProgram[iPos])
				{
					// Add only one to the destination string
					sToDisk += sFromProgram[iPos];

					// Increment the position past the doublet
					iPos += 2;

					// Tell the caller we found it
					return true;
				}
			}

			return false;
		}
		#endregion
	}
	#endregion

	public class DSection : JObject
	{
        // BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string StatusComment - Prose comment about the status of this section
		public string StatusComment
		{
			get
			{
				return m_sStatusComment;
			}
			set
			{
                SetValue(ref m_sStatusComment, value);
			}
		}
		private string m_sStatusComment = "";
		#endregion
		#region BAttr{g/s}: string DateStamp - Date the section was last edited
		public string DateStamp
		{
			get
			{
				return m_sDateStamp;
			}
			set
			{
                SetValue(ref m_sDateStamp, value);
			}
		}
		private string m_sDateStamp = "";
		#endregion
        #region BAttr{g/s}: string RecordKey - The Toolbox record key
        public string RecordKey
            // We preserve whatever was in the import file
        {
            get
            {
                return m_sRecordKey;
            }
            set
            {
                SetValue(ref m_sRecordKey, value);
            }
        }
        private string m_sRecordKey = "";
        #endregion
        #region Method void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("StatusComment", ref m_sStatusComment);
			DefineAttr("DateStamp",     ref m_sDateStamp);
            DefineAttr("Key",           ref m_sRecordKey);
        }
		#endregion

        // JAttrs ----------------------------------------------------------------------------
		#region JAttr{g}: JOwnSeq Paragraphs - a paragraph, its BT, IBT, etc.
		public JOwnSeq<DParagraph> Paragraphs
		{
			get { return m_osParagraphs; }
		}
		private JOwnSeq<DParagraph> m_osParagraphs;
		#endregion
		#region JAttr{g/s}: DReferenceSpan ReferenceSpan - the verses covered by this section
		public DReferenceSpan ReferenceSpan
		{
			get
			{
				return j_ownReferenceSpan.Value;
			}
			set
			{
				j_ownReferenceSpan.Value = value;
			}
		}
		private JOwn<DReferenceSpan> j_ownReferenceSpan = null;
		#endregion
        #region JAttr{g/s}: TranslatorNote History
        public TranslatorNote History
        {
            get
            {
                // An unload calls Clear; but we want to always make sure we have
                // an object here for any future load.
                if (null == j_ownHistory.Value)
                    j_ownHistory.Value = new TranslatorNote(TranslatorNote.History);

                return j_ownHistory.Value;
            }
            set
            {
                j_ownHistory.Value = value;
            }
        }
        private JOwn<TranslatorNote> j_ownHistory = null;
        #endregion

		// Derived Attributes: Ownership Hiererachy ------------------------------------------
		#region Attr{g}: DBook Book - returns the book that owns this section
		public DBook Book 
		{
			get
			{
				DBook book = (DBook)Owner;
				Debug.Assert(null != book);
				return book;
			}
		}
		#endregion
		#region Attr{g}: DTranslation Translation - returns the Translation that owns this section
		public DTranslation Translation
		{
			get { return Book.Translation; }
		}
		#endregion
		#region Attr{g}: DProject Project - returns the Project that owns this section
		public DProject Project
		{
			get { return Book.Project; }
		}
		#endregion
		#region Attr{g}: public DSFMapping Map - used only during SF Read operation
		// This object allows us to map from the read.Marker to the way to handle each
		// field, e.g., which one is a Section Title, which one is a back translation,
		// etc.
		public DSFMapping Map
		{
			get
			{
                return DB.Map;
			}
		}
		#endregion

		// Derived Attributes: Other ---------------------------------------------------------
		#region Attr{g}: string Title - the Title for the section
		public string Title
		{
			get 
			{ 
				foreach( DParagraph p in Paragraphs)
				{
					if (p.Style == StyleSheet.Section)
						return p.SimpleText;
				}
				return ""; 
			}
		}
		#endregion
		#region Attr{g}: DParagraph CrossReferences - the cross-references (if any) for the section
		public DParagraph CrossReferences
		{
			get 
			{ 
				foreach( DParagraph pg in Paragraphs)
				{
                    if (pg.Style == StyleSheet.SectionCrossReference)
						return pg;
				}
				return null; 
			}
		}
		#endregion
		#region Attr{g}: string DisplayName - book abbrev, reference and section title
		public string DisplayName
		{
			get
			{
				string s = Book.BookAbbrev + " " + ReferenceSpan.DisplayName;
				if (Title.Length > 0)
					s += ( " - " + 	Title);
				return s;
			}
		}
		#endregion
		#region Attr{g}: string ReferenceName - includes book and reference
		public string ReferenceName
		{
			get
			{
				string s = Book.DisplayName + " " + ReferenceSpan.DisplayName;
				return s;
			}
		}
		#endregion
		#region Attr{g}: DParagraph LastParagraphGroup - returns the last one in the sequence
		private DParagraph LastParagraphGroup
		{
			get 
			{
				if ( 0 == Paragraphs.Count)
					return null;
				return (DParagraph)Paragraphs[ Paragraphs.Count - 1 ];
			}
		}
		#endregion
		#region Attr{g}: bool IsTargetSection - T if part of the target translation
		public bool IsTargetSection
		{
			get
			{
				if (Project.TargetTranslation == Translation)
					return true;
				return false;
			}
		}
		#endregion
		#region Attr{g}: bool IsFrontSection - T if part of the front translation
		public bool IsFrontSection
		{
			get
			{
				if (Project.FrontTranslation == Translation)
					return true;
				return false;
			}
		}
		#endregion
		#region Attr{g}: DSection CorrespondingFrontSection
		public DSection CorrespondingFrontSection
		{
			get
			{
				if (IsFrontSection)
					return this;
				int iSection = Book.Sections.FindObj(this);
				Debug.Assert(iSection >=0 && iSection < Book.Sections.Count);

				DBook FrontBook = Book.FrontBook;
				if (null == FrontBook)
					return null;

				DSection SFront = FrontBook.Sections[iSection] as DSection;
				if (null == SFront)
					return null;
				return SFront;
			}
		}
		#endregion
		#region Attr{g}: string SectionStructure - seq of c_Text, c_Pict
		public const char c_Text = 'T';
		public const char c_Pict = 'P';
		public string SectionStructure
		{
			get
			{
				string s = "";
				char chLast = '\0';

				foreach(DParagraph p in Paragraphs)
				{
                    // Get the type (either a "Text" or a "Picture")
                    char ch = c_Text;
                    if (p as DPicture != null)
                        ch = c_Pict;

					if (ch != chLast || ch == c_Pict)
					{
						chLast = ch;
						s += ch;
					}
				}

				return s;
			}
		}
		#endregion
		#region Attr{g}: bool AllParagraphsMatchFront
		public bool AllParagraphsMatchFront
		{
			get
			{
				return CheckAllParagraphsMatch(DB.Project.SFront);
			}
		}
		#endregion
		#region Method: bool CheckAllParagraphsMatch(DSection SFront)
		public bool CheckAllParagraphsMatch(DSection SFront)
		{
            // Should havw the same number of paragraphs
            if (SFront.Paragraphs.Count != Paragraphs.Count)
                return false;

            // Examine the internals
            for (int i = 0; i < Paragraphs.Count; i++)
            {
                var PFront = SFront.Paragraphs[i];
                var PThis = this.Paragraphs[i];

                // Are the styles the same?
                if (PFront.Style != PThis.Style)
                    return false;

                // Collect the footnotes
                var FrontFoots = PFront.AllFootnotes;
                var ThisFoots = PThis.AllFootnotes;

                // Should have same count and styles
                if (FrontFoots.Count != ThisFoots.Count)
                    return false;
                for (int k = 0; k < ThisFoots.Count; k++)
                {
                    if (FrontFoots[k].Style != ThisFoots[k].Style)
                        return false;
                }
            }

			// If we made it here, then they are the same
			return true;
		}
		#endregion
        #region Attr{g}: List<DRun> AllRuns
        public List<DRun> AllRuns
		{
			get
			{
			    var v = new List<DRun>();

                foreach(DParagraph paragraph in Paragraphs)
                    foreach(DRun run in paragraph.Runs)
                        v.Add(run);

			    return v;
			}
		}
		#endregion
		#region Attr{g/s}: int LineNoInFile - the SFM's line no for the \s marker
		public int LineNoInFile
		{
			get
			{
				return m_nLineNoInFile;
			}
			set
			{
				m_nLineNoInFile = value;
			}
		}
		int m_nLineNoInFile;
		#endregion
        #region Method: List<TranslatorNote> GetAllTranslatorNotes()
        public List<TranslatorNote> GetAllTranslatorNotes()
        {
            var v = new List<TranslatorNote>();

            foreach (DParagraph p in Paragraphs)
                v.AddRange(p.GetAllTranslatorNotes());

            return v;
        }
        #endregion
        #region VAttr{g}: List<DFootnote> AllFootnotes
        public List<DFootnote> AllFootnotes
        {
            get
            {
                var v = new List<DFootnote>();

                foreach (DParagraph p in Paragraphs)
                    v.AddRange(p.AllFootnotes);

                return v;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DSection()
			: base()
		{
            // Paragraphs: flags are
            // - Don't check for duplicates
            // - Don't sort
            m_osParagraphs = new JOwnSeq<DParagraph>("Paras", this, false, false);

            // Scripture Reference Span
            j_ownReferenceSpan = new JOwn<DReferenceSpan>("RefSpan", this);
            ReferenceSpan = new DReferenceSpan();

            // Section History
            j_ownHistory = new JOwn<TranslatorNote>("History", this);
            History = new TranslatorNote(TranslatorNote.History);
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DSection objSection = (DSection)obj;

            return (objSection == this);
		}
		#endregion
		#region Method: void InitializeFromFrontSection(DSection SFront)
		public void InitializeFromFrontSection(DSection SFront)
            // Create a blank template for editing
		{
			// Duplicate the Front's reference
			Debug.Assert(SFront.ReferenceSpan.Start.Chapter > 0);
			ReferenceSpan.CopyFrom(SFront.ReferenceSpan);

			// Duplicate the Front's paragraphs
			Paragraphs.Clear();
			foreach(DParagraph pFront in SFront.Paragraphs)
			{
				DParagraph p = null;

				if (null != pFront as DPicture)
					p = new DPicture();
				else
					p = new DParagraph(pFront.Style);

				Paragraphs.Append(p);
				p.CopyFrom(pFront, true);
			}
		}
		#endregion
		#region Method: void UpdateFromFront(DSection SFront)
		public void UpdateFromFront(DSection SFront)
		{
			// Collect and update the cross reference paragraphs
			ArrayList vTarget = _GetCrossRefParagraphs(this);
			ArrayList vFront  = _GetCrossRefParagraphs(SFront);
			_UpdateCrossRefs(vFront, vTarget);

			// Collect and update the See Alsos
			vTarget = _GetCrossRefFootnotes(this,   DFootnote.Types.kSeeAlso);
			vFront  = _GetCrossRefFootnotes(SFront, DFootnote.Types.kSeeAlso);
			_UpdateCrossRefs(vFront, vTarget);

			// Collect and update the footnote labels
			vTarget = _GetCrossRefFootnotes(this,   DFootnote.Types.kExplanatory);
			vFront  = _GetCrossRefFootnotes(SFront, DFootnote.Types.kExplanatory);
			_UpdateExplanatoryLabels(vFront, vTarget);
        }
		#region Helper Method: ArrayList _GetCrossRefParagraphs(DSection)
		ArrayList _GetCrossRefParagraphs(DSection section)
		{
			ArrayList v = new ArrayList();
			foreach(DParagraph p in section.Paragraphs)
			{
				if (StyleSheet.SectionCrossReference == p.Style)
					v.Add(p);
			}
			return v;
		}
		#endregion
		#region Helper Method: ArrayList _GetCrossRefFootnotes(DSection, DFootnote.Types)
		ArrayList _GetCrossRefFootnotes(DSection section, DFootnote.Types fnType)
		{
			ArrayList v = new ArrayList();
			foreach(DFootnote fn in section.AllFootnotes)
			{
				if (fn.NoteType == fnType)
					v.Add(fn);
			}
			return v;
		}
		#endregion
		#region Helper Method: void _UpdateCrossRefs(vFront, vTarget)
		void _UpdateCrossRefs(ArrayList vFront, ArrayList vTarget)
		{
			if (vFront.Count != vTarget.Count)
				return;

			for(int i=0; i<vFront.Count; i++)
			{
				DParagraph pTarget = vTarget[i] as DParagraph;
				DParagraph pFront  = vFront[i]  as DParagraph;

				DTranslation.ConvertCrossReferences(pFront, pTarget);
			}
		}
		#endregion
		#region Helper Method: void _UpdateExplanatoryLabels(vFront, vTarget)
		void _UpdateExplanatoryLabels(ArrayList vFront, ArrayList vTarget)
		{
			if (vFront.Count != vTarget.Count)
				return;

			for(int i=0; i<vFront.Count; i++)
			{
				DFootnote fnTarget = vTarget[i] as DFootnote;
				DFootnote fnFront  = vFront[i]  as DFootnote;

				if (null == fnTarget || null == fnFront)
					return;

				DLabel labelTarget = fnTarget.Runs[0] as DLabel;
				DLabel labelFront  = fnFront.Runs[0] as DLabel;

				if (null == labelTarget || null == labelFront)
					return;

				labelTarget.Text = labelFront.Text;
			}
		}
		#endregion
        #endregion

        // Filters ---------------------------------------------------------------------------
		#region Attr{g/s}: bool MatchesFilter - T if DSection has passed current Filter test
		public bool MatchesFilter
		{
			get
			{
				return m_bMatchesFilter;
			}
			set
			{
				m_bMatchesFilter = value;

			}
		}
		bool m_bMatchesFilter = false;
		#endregion
		#region SAttr{g/s}: bool FilterIsActive - T if navigation to pay attention to filters
		static public bool FilterIsActive
		{
			get
			{
				return s_bFilterIsActive;
			}
			set
			{
				s_bFilterIsActive = value;
			}
		}
		static bool s_bFilterIsActive = false;
		#endregion
		#region Method: bool FilterTest_HasPictureWithCaption()
		public bool FilterTest_HasPictureWithCaption()
		{
			foreach(DParagraph p in Paragraphs)
			{
				// Are we dealing with a picture?
				DPicture picture = p as DPicture;
				if (null == picture)
					continue;

				// Get the corresponding front picture
				DPicture pictFront = GetCorrespondingFrontPicture(p as DPicture);
				if (null == pictFront)
					continue;

				// If the Front's length is greater than zero, then we have
				// a picture that expects a caption.
				if (pictFront.AsString.Length > 0)
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest_HasQuoteParagraph()
		public bool FilterTest_HasQuoteParagraph()
		{
			foreach(DParagraph p in Paragraphs)
			{
                if (p.Style.IsScripturePoetry)
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest_HasVernacularString(string s)
		public bool FilterTest_HasVernacularString(string s)
		{
			foreach(DParagraph p in Paragraphs)
			{
				string sText = p.AsString;

				if (sText.IndexOf(s) >= 0)
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest_VernacularStringInFront(string s)
		public bool FilterTest_VernacularStringInFront(string s)
		{
			DSection sFront = CorrespondingFrontSection;
			if (null == sFront)
				return false;

			return sFront.FilterTest_HasVernacularString(s);
		}
		#endregion
		#region Method: bool FilterTest_HasBTString(string s)
		public bool FilterTest_HasBTString(string s)
		{
			foreach(DParagraph p in Paragraphs)
			{
				string sText = p.ProseBTAsString;

				if (sText.IndexOf(s) >= 0)
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest_HasMismatchedQuotes()
		public bool FilterTest_HasMismatchedQuotes()
		{
			// Create a string that holds the entire section
			string s = "";
			foreach(DParagraph p in Paragraphs)
				s += p.AsString;

			// The Depth will be 0 if everything matches. 
			int nDepth = 0;

			// Process through the string
			foreach(char ch in s)
			{
				if (ch == '<')
					nDepth++;

				if (ch == '>')
					nDepth--;

				// We have a closing mark without an opening one
				if (nDepth < 0)
					return true;
			}

			// The two did not balance out
			if (nDepth != 0)
				return true;

			return false;
		}
		#endregion
		#region Method: bool FilterTest_HasUntranslatedText()
		public bool FilterTest_HasUntranslatedText()
		{
			DSection sFront = CorrespondingFrontSection;

			foreach(DParagraph p in Paragraphs)
			{
				// If a picture that is not supposed to have a caption, then
				// skip it.
				if (p as DPicture != null)
				{
					DPicture pFront = GetCorrespondingFrontPicture(p as DPicture);
					if (pFront != null && pFront.AsString.Length == 0)
						continue;
				}

				// Check for an empty DText
				foreach(DRun run in p.Runs)
				{
					DText text = run as DText;
					if (null != text)
					{
						if (text.AsString.Length == 0)
							return true;
					}
				}
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest_PunctuationProblem()

		#region Helper Method: bool _MissingParagraphFinalPunctuation(...)
		private bool _MissingParagraphFinalPunctuation(DParagraph p, string sEndPunctuation)
		{
			// We don't care with certain types of paragraphs
		    var vExcludedStyles = new List<ParagraphStyle>()
            {
                StyleSheet.BookTitle,
                StyleSheet.BookSubTitle,
                StyleSheet.Section,
                StyleSheet.MinorSection,
                StyleSheet.PictureCaption,
                StyleSheet.SectionCrossReference,
                StyleSheet.RunningHeader
            };
            if (vExcludedStyles.Contains(p.Style))
                return false;

			// Get the last DText in the paragraph
			for(int i = p.Runs.Count - 1; i >=0; i--)
			{
				// Loop until we get to the final DText
				DText text = p.Runs[i] as DText;
				if (null == text) 
					continue;

				// Get the final DPhrase
				if (text.Phrases.Count == 0)
					return false;
				DPhrase phrase = text.Phrases[ text.Phrases.Count - 1 ];

				// Retrieve the phrase's text, with no trailing space
				string s = phrase.Text.TrimEnd();
				if (s.Length == 0)
					return false;
				char chEnd = s[ s.Length - 1 ];

				// If we don't find this character in the EndPunctuation, then the
				// test fires (true).
				if (sEndPunctuation.IndexOf(chEnd) == -1)
				{
					//Console.WriteLine("MISSING PARAGRAPH FINAL: (" + p.ChapterI.ToString() + ":" +
					//	p.VerseI.ToString() + ") - \"" + p.AsString + "\"");
					return true;
				}
				break;
			}

			// Paragraph is OK
			return false;
		}
		#endregion
		#region Helper Method: bool _OrphanedPunctuation(...)
		private bool _OrphanedPunctuation(DParagraph p, string sPunctuation)
			// "End of sentence ."
			// "< Beginning of quote"
			// "Punctuation with spaces on both sides : like this colon"
			// "Two or more orphaned punctuation together !>"
			//
			// The basic idea is to look for punctuation surrouded by spaces, or which
			// is at a boundary.
		{
			string s = p.AsString;

			for(int i = 0; i < s.Length; i++)
			{
				// Examine the character, if it is not punctuation, then keep scanning
				char ch  = s[i];
				if (sPunctuation.IndexOf(ch) == -1)
					continue;

				// Do we have space (or a boundary) before this one?
				bool bSpaceBefore = false;
				if (i == 0)
					bSpaceBefore = true;
				if (i > 0 && char.IsWhiteSpace( s[i-1] ) )
					bSpaceBefore = true;

				// We need to see if we have a string of punctuation, and move to the final
				// one if so.
				int k = i;
				while (k < s.Length - 1 && sPunctuation.IndexOf( s[k + 1] ) != -1 )
					k++;

				// Do we have space (or a boundary following?
				bool bSpaceAfter = false;
				if (k == s.Length - 1)
					bSpaceAfter = true;
				if (k < s.Length - 1 && char.IsWhiteSpace( s[k+1] ) )
					bSpaceAfter = true;

				// If both before and after, then the test fires "true"
				if (bSpaceBefore && bSpaceAfter)
				{
					//Console.WriteLine("ORPHANED: (" + p.ChapterI.ToString() + ":" +
					//	p.VerseI.ToString() + ") - \"" + p.AsString + "\"");
					return true;
				}
			}

			// No orphaned punctuation encountered
			return false;
		}
		#endregion
		#region Helper Method: _EndPunctuationFollowsFootLetter(...)
		private bool _EndPunctuationFollowsFootLetter(DParagraph p, string sEndPunctuation)
		{
			for(int i=0; i<p.Runs.Count - 1; i++)
			{
				// Are we sitting at a foot letter?
				DFoot foot = p.Runs[i] as DFoot;
				if (null == foot)
					continue;

				// Is the next item a DText
				DText text = p.Runs[i+1] as DText;
				if (null == text)
					continue;

				// If the next item is End Punctuation, the test fires "true"
				string s = text.AsString;
				if (s.Length > 0 && sEndPunctuation.IndexOf(s[0]) != -1)
				{
					//Console.WriteLine("FOLLOWS FOOT LETTER: (" + p.ChapterI.ToString() + ":" +
					//	p.VerseI.ToString() + ") - \"" + p.AsString + "\"");
					return true;
				}
			}

			// No problem found
			return false;
		}
		#endregion

		#region Main Method: FilterTest_PunctuationProblem()
		public bool FilterTest_PunctuationProblem()
		{
			// Retrieve the writing system's punctuation
			WritingSystem ws = DB.Project.TargetTranslation.WritingSystemVernacular;
			string sPunctuation = ws.PunctuationChars;
			string sEndPunctuation = ws.EndPunctuationChars;

			// Loop through all the paragraphs
			foreach(DParagraph p in Paragraphs)
			{
				// Paragraph does not end with punctuation
				if (_MissingParagraphFinalPunctuation(p, sEndPunctuation))
					return true;

				// Paragraph has orphaned punctuation (punctuation with spaces on each side)
				if (_OrphanedPunctuation(p, sPunctuation))
					return true;

				// Paragraph has End Punctuation immediately following a footnote letter
				if (_EndPunctuationFollowsFootLetter(p, sEndPunctuation))
					return true;
			}

			// No punctuation problem encountered in this section
			return false;
		}
		#endregion

		#endregion
		#region Method: bool FilterTest_PictureCannotBeLocatedOnDisk()
		public bool FilterTest_PictureCannotBeLocatedOnDisk()
		{
			// Loop to find each picture
			foreach(DParagraph p in this.Paragraphs)
			{
				// Filter out everything that isn't a picture
				var pict = p as DPicture;
				if (null == pict)
					continue;

				// See if the picture exists
				var bFileExists = File.Exists(pict.FullPathName);
				if (!bFileExists)
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest(...) - Calls all the other tests
		public bool FilterTest(
			bool bOneOnly, 
			bool bVernacularText, string sVernacularSearchString,
			bool bFrontText,      string sFrontSearchString,
			bool bVernacularBT,   string sVernacularBTSearchString,
			bool bUntranslatedText,
			bool bMismatchedQuotes,
			bool bPictureWithCaption, 
			bool bHasQuoteParagraph,
			bool bPunctuationProblem,
			bool bPictureCannotBeLocatedOnDisk)
		{
			// Do the requested tests
			ArrayList a = new ArrayList();

			// Make sure the paragraphs are in acceptable condition
			foreach(DParagraph p in Paragraphs)
				p.Cleanup();

			// Search String Tests
			if (bVernacularText)
				a.Add( FilterTest_HasVernacularString(sVernacularSearchString) );
			if (bFrontText)
				a.Add( FilterTest_VernacularStringInFront(sFrontSearchString) );
			if (bVernacularBT)
				a.Add( FilterTest_HasBTString(sVernacularBTSearchString) );

			// Possible Problems
			if (bUntranslatedText)
				a.Add( FilterTest_HasUntranslatedText() );
			if (bMismatchedQuotes)
				a.Add( FilterTest_HasMismatchedQuotes() );
			if (bPunctuationProblem)
				a.Add( FilterTest_PunctuationProblem() );
			if (bPictureCannotBeLocatedOnDisk)
				a.Add( FilterTest_PictureCannotBeLocatedOnDisk() );

			// Structure Tests
			if (bPictureWithCaption)
				a.Add( FilterTest_HasPictureWithCaption() );
			if (bHasQuoteParagraph)
				a.Add( FilterTest_HasQuoteParagraph() );

			// If only one needs to be true, then scan the results to see if we had
			// one of the tests that passed.
			if (bOneOnly)
			{
				foreach( bool b in a)
				{
					if (b)
						return true;
				}
				return false;
			}

			// Otherwise, they must all be true. So scan all the results, and fail if
			// even one of the tests did not pass.
			foreach(bool b in a)
			{
				if (!b)
					return false;
			}
			return true;

		}
		#endregion

		// Methods: Reading Standard Format --------------------------------------------------
		#region Method: bool IsDigit(char ch)
		static bool IsDigit(char ch)
		{
			if (Char.IsDigit(ch))
				return true;
			if (ch == 'l' || ch == 'O')
				return true;
			return false;
		}
		#endregion
		#region Method: void ParseVerseStrings(...)
		static public void ParseVerseString(string s, ref string sVerseNo, ref string sVerseText)
		{
			int i = 0;
			sVerseNo = "";
			sVerseText = "";

            // Remove any leading spaces
            s = s.Trim();

            // Could have some leading punctuation (saw this is 43LukTch.ptx). We'll just
            // move this to appear after the verse number.
            while (s.Length > i && (s[i] == '(' || s[i] == '['))
            {
                sVerseText += s[i];
                i++;
            }

            // Loop through the string
            bool bMostRecentWasDigit = false;
            for (; i < s.Length; i++)
            {
                char ch = s[i];
                char chNext = (i < s.Length - 1) ? s[i + 1] : '\0';

                // Verses will normally consist of digits
                if (IsDigit(ch))
                {
                    // If the data is an 'l' (el), we assume it was a typo and change it to 
                    // a '1' (one). Somewhat kuldgy, but I'm finding real data this way.
                    if (ch == 'l')
                        ch = '1';
                    // Simularly O (oh) and zero
                    if (ch == 'O')
                        ch = '0';

                    sVerseNo += ch;
                    bMostRecentWasDigit = true;
                    continue;
                }

                // A single letter is acceptable, if it immediately follows a digit (thus, '10b'),
                // and if the following item is not a letter.
                if (char.IsLetter(ch) && bMostRecentWasDigit)
                {
                    if (char.IsLetter(chNext))
                        break;
                    sVerseNo += ch;
                    bMostRecentWasDigit = false;
                    continue;
                }

                // Spaces are permitted only if the next character is not a letter
                if (char.IsWhiteSpace(ch) && !char.IsLetter(chNext))
                {
                    bMostRecentWasDigit = false;
                    continue;
                }

                // We permit a comma to be interpretted as a verse bridge
                if (ch == ',')
                {
                    sVerseNo += '-';
                    bMostRecentWasDigit = false;
                    continue;
                }

                // A single hyphen is used for verse bridges
                if (ch == '-')
                {
                    sVerseNo += ch;
                    bMostRecentWasDigit = false;
                    continue;
                }

                // If we're here, then we are no longer working on a verse number
                break;
            }

            // Move past any blanks
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;

            // Anything else is the verse text
            if (s.Length > i)
                sVerseText += s.Substring(i);

            // If we wound up with multiple hyphens, then get rid of the interior. Thus "3,4,5" would
            // have become "3-4-5", which we turn into "3-5"
            int k1 = sVerseNo.IndexOf('-');
            int k2 = sVerseNo.LastIndexOf('-');
            if (k1 != -1 && k2 != -1 && k1 != k2)
                sVerseNo = sVerseNo.Remove(k1, k2 - k1);

            #region OBSOLETE - Replaced with the above on 10may08
            /////////////////////
            /***
			// Could have some leading punctuation (saw this is 43LukTch.ptx). We'll just
			// move this to appear after the verse number.
			while (s.Length > i && (s[i] == '(' || s[i] == '[' ))
			{
				sVerseText += s[i];
				++i;
			}

			// This part is tricky: We do a loop where we test for numbers, because sometimes
			// people have put in a spurious blank in the midst of their verse number, e.g.,
			// we want to catch "\v 3b - 5a".
			while (i < s.Length && ( IsDigit(s[i]) || s[i]=='-' ))
			{
				// The verse number is generally the next bunch of text prior to a whitespace.
				// It can be a non-digit, e.g., "\v 3b-5a"
				while (i < s.Length  && s[i] != ' ')
				{
					char ch = s[i];

					// If the data is an 'l' (el), we assume it was a typo and change it to 
					// a '1' (one). Somewhat kuldgy, but I'm finding real data this way.
					if (ch == 'l')
						ch = '1';
					// Simularly O (oh) and zero
					if (ch == 'O')
						ch = '0';

					sVerseNo += ch;
					++i;
				}

				// Move past any blanks
				while (s.Length > i && s[i] == ' ')
					++i;
			}

			// Anything else is the verse text
			if (s.Length > i)
				sVerseText = s.Substring(i);
            ***/
            #endregion
        }
		#endregion

		// I/O (Standard Format) -------------------------------------------------------------
		#region CLASS IO
		public class IO
		{
			// Attrs -------------------------------------------------------------------------
			#region Attr{g}: ScriptureDB SDB
			ScriptureDB SDB
			{
				get
				{
					Debug.Assert(null != m_db);
					return m_db;
				}
			}
			ScriptureDB m_db = null;
			#endregion
			#region Attr{g}: DSection Section
			DSection Section
			{
				get
				{
					Debug.Assert(null != m_section);
					return m_section;
				}
			}
			DSection m_section = null;
			#endregion
			#region Attr{g}; DParagraph LastParagraph
			DParagraph LastParagraph
			{
				get
				{
					if (Section.Paragraphs.Count == 0)
						return null;
					return Section.Paragraphs[ Section.Paragraphs.Count - 1 ] as DParagraph;
				}
			}
			#endregion
			#region Attr{g}: public DSFMapping Map - used only during SF Read operation
			// This object allows us to map from the read.Marker to the way to handle each
			// field, e.g., which one is a Section Title, which one is a back translation,
			// etc.
			public DSFMapping Map
			{
				get
				{
					return DB.TeamSettings.SFMapping;
				}
			}
			#endregion
			#region Attr{g/s}: bool VerseNumberFound - T if a \v exists in the section
			// A valid section will have at least one verse number in it; otherwise
			// we will have problems with navigation. We initialize this to false, meaning
			// that none have been found; and then rely on the AddVerse method
			// to tell us that a verse has indeed been found.
			bool VerseNumberFound
			{
				get
				{
					return m_VerseNumberFound;
				}
				set
				{
					m_VerseNumberFound = value;
				}
			}
			bool m_VerseNumberFound = false;
			#endregion

			// Chapter/Verse -----------------------------------------------------------------
			#region Method: static void ResetChapterVerse()
			static public void ResetChapterVerse()
			{
				s_nChapter = 0;
				s_nVerse = 0;
			}
			#endregion
			#region Method: static string CurrentChapterVerse
			static string CurrentChapterVerse
			{
				get
				{
					return s_nChapter.ToString() + ":" + s_nVerse.ToString();
				}
			}
			#endregion
			static public int s_nChapter;
			static public int s_nVerse;

			// Converting SfField > DRun -----------------------------------------------------
			#region Class: Phrase - Helper for parsing input string into Scripture Text phrases
			public class Phrase
			{
				// Constants ---------------------------------------------------------------------
				const char c_chVerticalBar = '|';
			    private const char c_chItalic = 'i';
			    private const char c_chBold = 'b';
			    private const char c_chUnderline = 'u';

				// Attrs -------------------------------------------------------------------------
				#region Attr{g/s}: string Text
				public string Text
				{
					get
					{
						return m_text;
					}
					set
					{
						m_text = value;
					}
				}
				string m_text;
				#endregion
			    public FontStyle FontToggles = FontStyle.Regular;
			    public bool IsFootLetter;

				// Scaffolding -------------------------------------------------------------------
				#region Constructor(sText)
				private Phrase(string sText)
				{
					Text = sText;
				}
				#endregion

				// Extract a phrase from the input string ----------------------------------------
				#region Method: bool IsStyleBegin(string sIn, int iPos) - "|b", "|i", etc.
				static bool IsStyleBegin(string sIn, int iPos)
				{
					// Are there enough letters following this on in the string that a
					// style can be declared?
					if (iPos >= sIn.Length - 3)
						return false;

					// Are we sitting at a character style marker? (Verticle Bar)
					if (sIn[iPos] != c_chVerticalBar)
						return false;
					if (sIn[iPos] == sIn[iPos+1])
						return false;

					// Do we have one of the recognized styles? Currently:
					// i = italic
					// b = bold
					// u = underlined
					// d = dashed underlined
					char ch = sIn[iPos + 1];
					if (ch != 'i' && ch != 'u' && ch != 'd' && ch != 'b')
						return false;

					// If we got here, then we have a inline style
					return true;
				}
				#endregion
				#region Method: bool IsStyleEnd(string sIn, int iPos) - "|r"
				static bool IsStyleEnd(string sIn, int iPos)
				{
					// Are there enough letters following this on in the string that a
					// style can be ended?
					if (iPos >= sIn.Length - 1)
						return false;

					// Shorthand
					char ch1 = sIn[iPos];
					char ch2 = sIn[iPos + 1];

					// Are we sitting at a character style marker? (Verticle Bar)
					if (ch1 != c_chVerticalBar)
						return false;
					if (ch1 == ch2)
						return false;

					// Is the next character the one that turns off the styles?
					if (ch2 != 'r')
						return false;

					// If we got here, then we have a inline style
					return true;
				}
				#endregion
				#region Method: bool IsDoublet(string sIn, int iPos)
				static bool IsDoublet(string sIn, int iPos)
				{
					// Is there a character following this one in the input string?
					if (iPos >= sIn.Length - 1)
						return false;

					// Shorthand: get the current character and the next one
					char ch1 = sIn[iPos];
					char ch2 = sIn[iPos + 1];

					// Is the current character one of the recognized reserve characters?
					if (ch1 != c_chVerticalBar)
						return false;

					// Are the current character and the next one the same value?
					if (ch1 != ch2)
						return false;

					// If we have gotten to here, then we have a literal doublet.
					return true;
				}
				#endregion
				#region Method: bool IsFootnote(string sIn, int iPos)
				static bool IsFootnote(string sIn, int iPos)
				{
					// Is the input string long enough for there to be a footnote here?
					if (iPos > sIn.Length - 3)
						return false;
				
					// Do we have a match with the expected footnote marker?
					if (sIn.Substring(iPos, 3) != "|fn")
						return false;

					// If we get here, then we have a footnote.
					return true;
				}
				#endregion
				#region Method: Phrase GetPhrase(string sIn, ref int iPos)
				static public Phrase GetPhrase(string sIn, ref int iPos)
				{
					// We'll collect the phrase here
					var sText = "";

					// Are we setting at a footnote marker? This is defined as a |fn. 
					// If so, then declare the type and we are done.
					if (IsFootnote(sIn, iPos))
					{
						iPos += 3;

						// Eat any white space that follows the footnote
						while (iPos < sIn.Length-1 && sIn[iPos] == ' ')
							++iPos;

						return new Phrase("") {IsFootLetter = true};
					}

					// Default to Normal; if we encounter a style we will change it.
				    var toggles = FontStyle.Regular;

					// Are we sitting at a font modification? This is defined as a opening 
                    // bar followed by a recognized style character. If so, then retrieve the
					// character, interpret it and increment beyond it.
					if (IsStyleBegin(sIn, iPos))
					{
						var chMod = (sIn[iPos + 1]);
                        if (chMod == c_chBold)
                            toggles = FontStyle.Bold;
                        if (chMod == c_chItalic)
                            toggles = FontStyle.Italic;
                        if (chMod == c_chUnderline)
                            toggles = FontStyle.Underline;
                        iPos += 2;
					}

					// Loop through the input string to collect the text
					while( iPos < sIn.Length )
					{
						// A double vertical bar or brace should be interpreted as a literal 
						// single. So we add the first occurrence into the text, and then
						// increment 2 so as to pass by the doublet.
						if (IsDoublet(sIn,iPos))
						{
							sText += sIn[iPos];
							iPos += 2;
							continue;
						}

						// Footnote reference
						if (IsFootnote(sIn, iPos))
							break;

						// Style begin
						if (IsStyleBegin(sIn, iPos))
							break;

						// Style end
						if (IsStyleEnd(sIn, iPos))
						{
							iPos += 2;
							break;
						}

						// If we are here, then no special situations remain; add the character
						sText += sIn[iPos++];
					}

					return (sText.Length > 0) ?
                        new Phrase(sText) { FontToggles = toggles } : 
                        null ;
				}
				#endregion
			}
			#endregion
			#region Method: static string CombineStrings(string s1, string s2)
			static string CombineStrings(string s1, string s2)
			{
				string s = s1;

				if (s1.Length > 0 && s1[ s1.Length-1 ] != ' ' &&
					s2.Length > 0 && s2[0] != ' ')
				{
					s += " ";
				}

				s += s2;

				return s;
			}
			#endregion
            #region Method: static void CombineLikePhrases( List<DRun> vRuns )
            static private void CombineLikePhrases( List<DRun> vRuns)
			{
				foreach( DRun run in vRuns)
				{
					DText txt = run as DText;
					if (null != txt)
					{
						CombineLikePhrases( txt.Phrases );
						CombineLikePhrases( txt.PhrasesBT );
					}
				}
			}
			#endregion

			#region Method: static void CombineLikePhrases( JOwnSeq os )
			static private void CombineLikePhrases(JOwnSeq<DPhrase> os)
			{
				for(var i=0; i<os.Count - 1; )
				{
					var p1 = os[i] as DPhrase;
					var p2 = os[i+1] as DPhrase;

                    if (p1.FontToggles == p2.FontToggles)
					{
						p1.Text = CombineStrings(p1.Text, p2.Text);
						os.Remove(p2);
					}
					else
						i++;
				}
			}
			#endregion

            #region Method: static List<Phrase> GetPhrases(string sSource)
            static List<Phrase> GetPhrases(string sSource)
			{
                var v = new List<Phrase>();

				Phrase p = null;
				int iPos = 0;

				while ( (p = Phrase.GetPhrase( sSource, ref iPos)) != null)
					v.Add(p);

				return v;
			}
			#endregion

            #region SMethod: string EatSpuriousVerticleBars(string sSource)
            static public bool s_VerticleBarsEncountered = false;
            static public string EatSpuriousVerticleBars(string sSource)
                // Unfortuantely I've got these ||||i and |||r sequences, due to bugs that
                // caused them to double on each save, as if they were literals.
                // So going to eat them for a while to clean up files.
            {
                string[] v = new string[] { "||i", "||r" };

                for (int i = 0; i < v.Length; i++)
                {
                    do
                    {
                        int n = sSource.IndexOf(v[i]);
                        if (n == -1)
                            break;

                        sSource = sSource.Remove(n, 1);
                        s_VerticleBarsEncountered = true;
                    } while (true);
                }

                return sSource;
            }
            #endregion

            #region Method: static int _AdvancePastFootnote(int i, List<Phrase> vRaw)
            static int _AdvancePastFootnote(int i, List<Phrase> vRaw)
			{
				if (i < vRaw.Count)
				{
					var raw = vRaw[i];

					if (raw.IsFootLetter)
						i++;
				}

				return i;
			}
			#endregion
            #region Method: static List<DRun> CreateDRunsFromInputText(SfField)
            static public List<DRun> CreateDRunsFromInputText(string sSource)
			{
				// Retrieve the raw phrases
                var vTextRawPhrases = GetPhrases(sSource);

				// Loop to create the DRun's and populate the vRuns list
				var vRuns = new List<DRun>();
				for( var i=0; i<vTextRawPhrases.Count; i++ )
				{
					var raw = vTextRawPhrases[i];

					// Footnote: add the new run into the array, then loop to the next one
					if (raw.IsFootLetter)
					{
                        vRuns.Add(new DFoot(null));
						continue;
					}

					// Otherwise, we have Scripture text
					// Get a DScriptureText to add this phrase into. If we are currently
					// sitting at a DText, then add to it; otherwise create a new DText
					DText txt = null;
					if (vRuns.Count > 0)
						txt = vRuns[ vRuns.Count - 1] as DText;
					if (null == txt)
					{
						txt = new DText();
						vRuns.Add(txt);
					}

					// Create the phrase and add it to the DScriptureText's sequence
                    var phrase = new DPhrase(raw.Text) { FontToggles = raw.FontToggles };
					txt.Phrases.Append(phrase);
				}

				return vRuns;
			}
			#endregion
            #region Method: static List<DPhrase> _GetNextDPhrases(ref int i, ArrayList vRaw)
            static List<DPhrase> _GetNextDPhrases(ref int i, List<Phrase> vRaw)
			{
				// We'll put the DPhrases we create into this arraylist
				var vPhrases = new List<DPhrase>();

				// Loop from the curent position until the end
				while (i < vRaw.Count)
				{
					// Retrieve the next phrase
					var raw = vRaw[i] as Phrase;

					// If the Phrases's style is a foot letter, then we are done, as
					// we are not dealing with a DPhrase.
					if (raw.IsFootLetter)
						break;

					// Create the phrase and add it to the destination array
					var phrase = new DPhrase( raw.Text) {FontToggles = raw.FontToggles};
					vPhrases.Add(phrase);

					// Move on to process the next item in vRaw
					i++;
				}

				return vPhrases;
			}
			#endregion
            #region Method: static List<DRun> FieldToRuns(SfField field)
            static public List<DRun> FieldToRuns(SfField field)
			{
				// Loop to create the DRun's and populate the vRuns list
				var vRuns = CreateDRunsFromInputText(field.Data);

				// Retrieve the raw phrases (Vernacular and Back Translation)
				var vBTRawPhrases = GetPhrases( field.BT );

				// Reconcile the BT Phrases into the Runs
				var iBT = 0;
				foreach( var run in vRuns)
				{
					// Deal with a footnote if that is what we currently have
					var foot = run as DFoot;
					if (null != foot) 
					{
						iBT  = _AdvancePastFootnote(iBT,  vBTRawPhrases);
						continue;
					}

					// If we are here, then we are dealing with Scripture Text. So
					// get the DText we'll be working with.
					var txt = run as DText;

					// Retrieve the Prose BT's DPhrases and add them into the DText's
					// BT attribute.
					var vDPhrasesBT = _GetNextDPhrases(ref iBT, vBTRawPhrases);
					foreach(DPhrase p in vDPhrasesBT)
						txt.PhrasesBT.Append(p);

					// Make sure there is at least an empty phrase for the run's Prose BT
					if (txt.PhrasesBT.Count == 0)
					{
						var phrase = new DPhrase("");
						txt.PhrasesBT.Append(phrase);
					}

				}

				// Did we have any Back Translations left over? This could happen if
				// the footnotes or phrases did not line up. 
				if (iBT < vBTRawPhrases.Count)
				{
					// AddParagraph all the remaining phrases into a single string
					string sBT = "";
					while (iBT < vBTRawPhrases.Count)
					{
						Phrase phBT = vBTRawPhrases[iBT] as Phrase;

						sBT = CombineStrings(sBT, phBT.Text);

						iBT++;
					}

					// Place it into the last non-footnote DRun
					int iText = vRuns.Count - 1;
					while (iText >= 0)
					{
						// Shorthand to the DRun
						DText txt = vRuns[iText] as DText;

						// If this run is anything other than a Footnote Letter, then append
						// the back translation into it.
						if ( null != txt )
						{
							var phrase = new DPhrase(sBT);
							txt.PhrasesBT.Append(phrase);
							break;
						}

						// Otherwise, increment down to try the preceding one
						iText--;
					}
				}

				// Combine like phrases
				CombineLikePhrases(vRuns);

                return vRuns;
			}
			#endregion
			#region Method: void AddParagraphText(DParagraph, sfField)
			public void AddParagraphText(DParagraph p, SfField field)
			{
				List<DRun> runs = FieldToRuns( field );
				foreach(DRun run in runs)
					p.Runs.Append(run);
			}
			#endregion

			// Converting DRun > SfField -----------------------------------------------------
			#region Method: string GetMarkerFromStyle(DParagraph p)
			string GetMarkerFromStyle(DParagraph p)
			{
				// Pictures
                if (null != p as DPicture)
                    return DSFMapping.c_sMkrPictureCaption;

				// Footnotes
				DFootnote fn = p as DFootnote;
				if (null != fn)
				{
					if (fn.NoteType == DFootnote.Types.kSeeAlso)
						return Map.MkrSeeAlso;
					else
						return Map.MkrFootnote;
				}
				// Simple Paragraphs (no verses)
			    var vSimpleParagraphs = new List<ParagraphStyle>()
			    {
                    StyleSheet.RunningHeader,
                    StyleSheet.BookTitle,
                    StyleSheet.BookSubTitle,
                    StyleSheet.Section,
                    StyleSheet.MinorSection,
                    StyleSheet.SectionCrossReference,
                    StyleSheet.MajorSection,
                    StyleSheet.MajorSectionCrossReference
			    };
                if (vSimpleParagraphs.Contains(p.Style))
                    return p.Style.ToolboxMarker;

				// Otherwise, we want a \vt for verse text
				return Map.MkrVerseText;
			}
			#endregion
			#region Method: void AddTextField(DParagraph p) - over all runs, e.g., a Simple para
			void AddTextField(DParagraph p)
			{
				// Handle paragraphs that have no data in them
				if (p.Runs.Count == 0)
				{
                    SfField f = new SfField(GetMarkerFromStyle(p));
                    SDB.Append(f);
					return;
				}

				// If we are here, we do not expect to have a paragraph with a DVerse,
				// DChapter, or DSeeAlso.
				foreach( DRun run in p.Runs)
				{
					if (null != run as DVerse || 
						null != run as DChapter || 
						(null != run as DFoot && (run as DFoot).IsSeeAlso))
					{
						Debug.Assert(false, "Disallowed data in a simple paragraph.");
						return;
					}
				}

				int iPosFirst = 0;
				int iPosLast  = p.Runs.Count - 1;
				AddTextField(p, ref iPosFirst, ref iPosLast);
			}
			#endregion
			#region Method: void AddTextField(DParagraph, ref iPosFirst, ref iPosLast)
			void AddTextField(DParagraph p, ref int iPosFirst, ref int iPosLast)
			{
				// Do we have anything to do?
				if ( -1 == iPosFirst || -1 == iPosLast)
				{
					iPosFirst = -1;
					iPosLast  = -1;
					return;
				}

				// How many footnotes are we dealing with in this verse? The utility that
				// outputs to Word requres that there not be more than one |fn within a
				// single \vt line. (It doesn't have problems with \cf's.)
				int cFootnotes = 0;
				for(int i = iPosFirst; i <= iPosLast; i++)
				{
                    DFoot foot = p.Runs[i] as DFoot;
                    if (null != foot && foot.IsExplanatory)
						++cFootnotes;
				}

				// Each pass through this loop corresponds to a \vt field being produced.
				while (iPosFirst <= iPosLast)
				{
					// Build a list of the Translator Notes that we'll output
                    List<TranslatorNote> listTranslatorNotes = new List<TranslatorNote>();

					// We will build the \vt data here
					string sContents = "";
					string sProseBT  = "";
					string sIntBT    = "";
                  
                    // If what we're working on is itself a footnote, then we need its verse)
                    if (p as DFootnote != null && 
                        !string.IsNullOrEmpty((p as DFootnote).VerseReference))
                    {
                        sContents += (p as DFootnote).VerseReference + ": ";
                        sProseBT += (p as DFootnote).VerseReference + ": ";
                    }

					// If we encounter a footnote, we'll remember it here, so that we can
					// output it after the \vt
					DFootnote footnote = null;

					// Loop through the runs to build the Save Strings
					for(; iPosFirst <= iPosLast; iPosFirst++)
					{
						DRun run = p.Runs[iPosFirst] as DRun;

						sContents += run.ContentsSfmSaveString;
						sProseBT  += run.ProseBTSfmSaveString;
						sIntBT    += run.IntBTSfmSaveString;

						// Collect any Translator Notes
						DText text = run as DText;
						if (null != text)
						{
                            foreach (TranslatorNote tn in text.TranslatorNotes)
                                listTranslatorNotes.Add(tn);
						}

						DFoot foot = run as DFoot;
						if (null != foot && foot.IsExplanatory)
						{
							// Store the footnote
							footnote = foot.Footnote;

							// If we still have footnotes remaining, then break from
							// this inner for loop. We'll need to make another pass through
							// the outer while loop.
							--cFootnotes;
							if (cFootnotes > 0)
								break;
						}
					}

					// Create the field and add it to the list
					string sMkr = GetMarkerFromStyle(p);
					SfField field = new SfField(sMkr, sContents, sProseBT, sIntBT);
					SDB.Append(field);

					// If we have a footnote, add it to the database
					if (null != footnote)
						Footnote_out(footnote);
                    footnote = null;

					// Add the Translator Notes
                    foreach (TranslatorNote tn in listTranslatorNotes)
                        tn.AddToSfmDB(SDB);
				}

				// Reset the counters, since we have now dealt with their data
				iPosFirst = -1;
				iPosLast  = -1;
			}
			#endregion

			// Pictures ----------------------------------------------------------------------
			#region Method: bool Picture_in(SfField)
			private bool Picture_in(SfField field)
			{
				// Test to see if we should process this marker
				if ( !DSFMapping.IsPicture( field.Mkr ) )
					return false;

				// Examine the last paragraph; if it is not a picture, then create one
				DPicture pict = null;
				if (null != LastParagraph as DPicture)
					pict = LastParagraph as DPicture;
				if (null == pict)
				{
					pict = new DPicture();
					Section.Paragraphs.Append(pict);
				}

				// Add the data depending on the marker
                if (DSFMapping.c_sMkrPicturePath == field.Mkr)
                    pict.RelativePathName = field.Data;
				if (DSFMapping.c_sMkrPictureWordRtf == field.Mkr)
					pict.WordRtfInfo = field.Data;
                if (DSFMapping.c_sMkrPictureCaption == field.Mkr)
                    AddParagraphText(pict, field);
				return true;
			}
			#endregion
			#region Method: bool Picture_out(DParagraph)
			private bool Picture_out(DParagraph p)
			{
				if (p.GetType() != typeof(DPicture))
					return false;

				DPicture pict = p as DPicture;

                SfField fPath = new SfField(DSFMapping.c_sMkrPicturePath, pict.FullPathName);
				SDB.Append(fPath);

				SDB.Append( new SfField(DSFMapping.c_sMkrPictureWordRtf,pict.WordRtfInfo));

				AddTextField(pict);

				return true;
			}
			#endregion

			// Simple paragraphs (e.g., \s, \s2, \r, \mt) ------------------------------------
			#region Method: bool AddSimple(SfField, sMkr, sStyle)
			bool AddSimple(SfField field, string sMkr, string sStyle)
			{
				// If the current field's marker is a match, then create a new paragraph
				if (field.Mkr == sMkr)
				{

				    var style = StyleSheet.FindFromToolboxMarker(sStyle) ??
				                StyleSheet.Paragraph;
                    var p = new DParagraph(style);

					Section.Paragraphs.Append(p);

					AddParagraphText(p, field);

					return true;
				}

				return false;
			}
			#endregion

			// Footnotes ---------------------------------------------------------------------
			#region Method: bool Footnote_in(SfField)
			private bool Footnote_in(SfField field)
			{
				// Vernacular Footnote
				if (Map.IsFootnote( field.Mkr ))
				{
					// Create and append a footnote into the Section's sequence
					DFootnote footnote = new DFootnote(s_nChapter, s_nVerse,
                        DFootnote.Types.kExplanatory);

					// Connect up the DRun in the most recent paragraph to this footnote
					// (or alternatively, a |fn will be added.)
                    LastParagraph.ConnectFootnote(footnote);

					// Verse Reference
					string sVerseReference;
                    field.BT = DFootnote.ParseLabel(field.BT, out sVerseReference);
                    field.Data = DFootnote.ParseLabel(field.Data, out sVerseReference);
                    footnote.VerseReference = sVerseReference;

					// Populate the Contents and ProseBT of the footnote
                    AddParagraphText(footnote, field);

					return true;
				}

				return false;
			}
			#endregion
			#region Method: void Footnote_out(DFootnote)
			public void Footnote_out(DFootnote footnote)
			{
				AddTextField(footnote);
			}
			#endregion

            // Translator Notes --------------------------------------------------------------
            #region Method: bool TranslatorNote_in(SfField)
            bool TranslatorNote_in(SfField field)
            {
                TranslatorNote tn = null;

                // Is this field a TranslatorNote?
                if (DSFMapping.c_sMkrTranslatorNote == field.Mkr)
                {
                    var oxes = new XmlDoc(field.Data);
                    var node = XmlDoc.FindNode(oxes, TranslatorNote.c_sTagTranslatorNote);
                    if (null == node)
                        node = XmlDoc.FindNode(oxes, "TranslatorNote");
                    tn = TranslatorNote.Create(node);
                }

                // Otherwise, is it an old-style (pre version 1.1) note?
                else
                {
                    tn = TranslatorNote.ImportFromOldStyle(s_nChapter, s_nVerse, field);
                }

                // If we don't have a note, then we're done processing
                if (null == tn)
                    return false;
                tn.Debug_VerifyIntegrity();

                // Make sure we have a paragraph we can put it into
                if (null == LastParagraph)
                {
                    throw new eBookReadException(
                        Loc.GetMessages("msgMissingParagraphMarkerForNote",
                            "A translator note was encountered but there was no paragraph " +
                            "marker for it to go into."),
                        HelpSystem.Topic.kErrMissingParagraphMarkerForNote,
                        field.LineNo);
                }

                // Get the last DText, as that's where the note belongs
                DText text = LastParagraph.GetOrAddLastDText();

                // Insert the note
                text.TranslatorNotes.Append(tn);

                return true;
            }
            #endregion

            // History -----------------------------------------------------------------------
            #region Method: bool History_In(SfField)
            private bool History_In(SfField field)
            {
                // Old-style status comment: Import it
                if (Map.IsStatusCommentMarker(field.Mkr))
                {
                    if (!string.IsNullOrEmpty(field.Data.Trim()))
                    {
                        Section.History.AddMessage(
                            DEventMessage.DefaultDate,
                            DB.TeamSettings.Stages.Draft,
                            field.Data);
                    }
                    return true;
                }

                return Section.History.ReadOldHistory(field);
            }
            #endregion
            #region Method: void History_Out()
            private void History_Out()
            {
                Section.History.AddToSfmDB(SDB);
            }
            #endregion

            // DateStamp ---------------------------------------------------------------------
			#region Method: bool DateStamp_in(SfField field)
			private bool DateStamp_in(SfField field)
			{
				// Test to see if we should process this marker
				if (!Map.IsDateStampMarker(field.Mkr))
					return false;

				// Just copy in the field contents; don't attempt to parse
				Section.DateStamp = field.Data;
				return true;
			}
			#endregion
			#region Method: void DateStamp_out()
			private void DateStamp_out()
			{
				if (Section.DateStamp.Length > 0)
					SDB.Append( new SfField(Map.MkrDateStamp, Section.DateStamp));
			}
			#endregion

			#region Method: bool AddVernPara(SfField)
			bool AddVernPara(SfField field)
			{
                bool bIsVernacularPara = Map.IsVernacularParagraph(field.Mkr);

				// Test to see if we should process this marker
                if (!bIsVernacularPara && 
					!Map.IsCrossRef( field.Mkr) &&
					!Map.IsSection( field.Mkr ) &&
					!Map.IsSection2(field.Mkr) )
				{
					return false;
				}

				// Create the new paragraph. The style is the same as the field marker
			    var style = StyleSheet.FindFromToolboxMarker(field.Mkr) ??
			                StyleSheet.Paragraph;
                DParagraph p = new DParagraph(style);
                Section.Paragraphs.Append(p);
   				AddParagraphText(p, field);

				return true;
			}
			#endregion
			#region Method: bool AddChapter(SfField)
			bool AddChapter(SfField field)
			{
				// Test to see if we should process this marker
				if ( !Map.IsChapter( field.Mkr ) )
					return false;

				// Do we have a valid paragraph to add it to?
				if (null == LastParagraph)
					throw new eBookReadException(
                        Loc.GetMessages("msgChapterNotInParagraph",
                            "A \\c was encountered but there was no paragraph marker before it."),
						HelpSystem.Topic.kErrChapterNotInParagraph,
						field.LineNo);

				// Add it to the paragraph
				DChapter chapter = DChapter.Create( field.Data );
				if (null == chapter)
				{
					throw new eBookReadException(
                        Loc.GetMessages("msgBadChapterNo",
                            "Unable to open the file due to a bad Chapter Number field."),
						HelpSystem.Topic.kErrBadChapterNo,
						field.LineNo);
				}
				LastParagraph.AddRun(chapter);
				s_nChapter = chapter.ChapterNo;
				s_nVerse   = 1;

				return true;
			}
			#endregion
			#region Method: bool AddVerse(SfField)
			private bool AddVerse(SfField field)
			{
				// Test to see if we should process this marker
				if ( !Map.IsVerse( field.Mkr ) )
					return false;

				// Make sure the paragraph is a valid place for a verse. 
				// Nov05: I attempted just adding a \p, but the problem is that
				// by the time we get here, the calculation of ReferenceSpan has
				// already happened, so we just get another error (about section
				// mismatches) down the road, with no good way to correct it.
                DParagraph last = LastParagraph;
                if (null == last || !Map.IsVernacularParagraph(last.Style.ToolboxMarker))
                {
                    throw new eBookReadException(
                        Loc.GetMessages("msgMissingParagraphMarker",
                            "A verse field was encountered but there was no paragraph marker for the verse to go into."),
                        HelpSystem.Topic.kErrMissingParagraphMarker,
                        field.LineNo);
                }

				// Create the DRun subclass and add it to the paragraph
				DVerse verse = DVerse.Create( field.Data );
				if (null == verse)
				{
					throw new eBookReadException(
                        Loc.GetMessages( "msgMissingVerseNumber",
                            "A \\v was encountered but there was no verse number beside it."),
						HelpSystem.Topic.kErrMissingVerseNumber,
						field.LineNo);
				}
				LastParagraph.AddRun(verse);
				s_nVerse = verse.VerseNo;

				// The section can now be considered valid; it has a verse number
				VerseNumberFound = true;

				return true;
			}
			#endregion
			#region Method: bool AddVerseText(SfField)
			private bool AddVerseText(SfField field)
			{
				// Test to see if we should process this marker
				if ( !Map.IsVerseText( field.Mkr ) )
					return false;

				// Parse the text and add it to the paragraph
				AddParagraphText( LastParagraph, field );

				return true;
			}
			#endregion

			#region Method: bool AddSeeAlso(SfField)
			private bool AddSeeAlso(SfField field)
				// Assumes that any \cf's come after any \ft's that might be in the
				// verse. The SeeAlso is a letter that is appended to the end of
				// the current verse. (A verse-internal SeeAlso should be done as a
				// normal footnote.)
			{
				// Test to see if we should process this marker
				if ( !Map.IsSeeAlso( field.Mkr ) )
					return false;

				// Make sure the paragraph is a valid place for a See Also.
				if ( ! Map.IsVernacularParagraph( LastParagraph.Style.ToolboxMarker ) )
					throw new eBookReadException(
                        Loc.GetMessages( "msgMissingParagraphMarkerForCF",
                            "A cross-reference field (\\cf) was encountered but there was no " +
                            "paragraph marker for the reference to go into."),
						HelpSystem.Topic.kErrMissingParagraphMarkerForCF,
						field.LineNo);

				// Remove any asterix's (*) (an anomaly in the Timor data)
				// TODO: Do we need to parameterize whether or not this action
				// is executed?
				string s = "";
				foreach( char c in field.Data)
				{
					if (c != '*')
						s += c;
				}
				field.Data = s;

				// Create a footnote containing the text
                DFootnote footnote = new DFootnote(s_nChapter, s_nVerse,
                    DFootnote.Types.kSeeAlso);

                // Verse Reference
                string sVerseReference;
                field.Data = DFootnote.ParseLabel(field.Data, out sVerseReference);
                footnote.VerseReference = sVerseReference;

                // Populate the Contents of the footnote
                AddParagraphText(footnote, field);

				// Add the See Also text to the paragraph
                LastParagraph.ConnectFootnote(footnote);

				return true;
			}
			#endregion
			#region Method: bool SkipDiscardFields(SfField)
			private bool SkipDiscardFields(SfField field)
			{
				// Test to see if we should process this marker
				if ( !Map.IsDiscardedField( field.Mkr ) )
					return false;

				// Don't Do Anything

				return true;
			}
			#endregion

			// public access -----------------------------------------------------------------
			#region Constructor(ScriptureDB db, DSection section)
			public IO(ScriptureDB db, DSection section)
			{
				m_db = db;
				m_section = section;
			}
			#endregion
			#region Method: void ReadSF()
			public void ReadSF()
			{
				// Make a note of the section's marker. We want to preserve it. OurWord
                // keeps sections in the order as received, but Toolbox relies on the
                // record marker for sorting; so it is important that we not change this.
				SfField fSection = SDB.GetCurrentField();
                Section.RecordKey = fSection.Data.Trim();

				// Read Loop: reads in each SF field, and deals with it appropriately
				do
				{
					// Retrieve the next field
					SfField field = SDB.GetNextField();
					if (null == field)
						break;

					// Header, Main Title, SubTitle, Major Sections
					if ( AddSimple( field, Map.MkrHeader, Map.StyleHeader))
						continue;
					if ( AddSimple( field, Map.MkrMainTitle, Map.StyleMainTitle))
						continue;
					if ( AddSimple( field, Map.MkrSubTitle, Map.StyleSubTitle))
						continue;
                    if ( AddSimple(field, Map.MkrMajorSection, 
                        StyleSheet.MajorSection.ToolboxMarker))
                        continue;
                    if ( AddSimple(field, Map.MkrMajorSectionCrossRef, 
                        StyleSheet.MajorSectionCrossReference.ToolboxMarker))
                        continue;

					// Checking Status, DateStamp
					if ( DateStamp_in(field))
						continue;
                    if ( History_In(field))
                        continue;

					// Pictures
					if ( Picture_in(field) )
						continue;

					// Vernacular Paragraphs, ChapterNos, Verse Nos, VerseText
					if ( AddVernPara(field))
						continue;
					if ( AddChapter(field))
						continue;
					if ( AddVerse(field))
						continue;
					if ( AddVerseText(field ))
						continue;

					// Notes, Footnotes, See Also's
                    if ( TranslatorNote_in(field))
                        continue;
					if ( Footnote_in(field))
						continue;
					if ( AddSeeAlso(field))
						continue;

					// Fields we are set up to ignore
					if ( SkipDiscardFields(field))
						continue;

					// All other fields shall be handled as general notes, so that the
					// data is not lost. 
					if (!SDB.CurrentFieldIsRecordMarker)
					{
						Console.WriteLine("Unhandled Marker at " + field.LineNo + 
							" ---->" + field.Mkr + " - " + field.Data);

						if (field.Data.Length > 0 && null != LastParagraph)
						{
							field.Data = "(" + field.Mkr + ") " + field.Data;
							field.Mkr  = "nt";
                            TranslatorNote_in(field);
						}
					}

                } while (!SDB.CurrentFieldIsRecordMarker);

				// Was there a verse in the section?
				if (!VerseNumberFound)
					throw new eBookReadException(
                        Loc.GetMessages( "msgNoVerseInSection",
                            "A section (\\s) was encountered that did not have a verse (\\v) field in it."),
						HelpSystem.Topic.kErrNoVerseInSection,
						fSection.LineNo);

                // Deal with spaces
				foreach(DParagraph p in Section.Paragraphs)
					p.Cleanup();

                /////// SEARCHING for a bug, can delete once found
                foreach (TranslatorNote tn in Section.GetAllTranslatorNotes())
                    tn.Debug_VerifyIntegrity();
                /////// END BUG

            }
			#endregion
			#region Method: void WriteSF()
			public void WriteSF()
                // 10jan08 - Formerly, I began this method with the code:
                //
				//     Deal with spurious spaces the user might have input
                //     foreach(DParagraph p in Section.Paragraphs)
                //         p.Cleanup();
                //
                // This was causing a crash during autosave, because it removed a space
                // that the screen thought was needed. So now, I'm content to call
                // Cleanup during load, and assume that OurWord keeps things in reasonable
                // shape during its execution, so that a cleanup is not necessary.
			{
				// The record begins with a record marker (See the ReadSF comment for
                // reason we preserve what was in the import file.)
                if (string.IsNullOrEmpty(Section.RecordKey))
                {
                    Section.RecordKey = Section.Book.BookAbbrev + " " + 
                        Section.ReferenceSpan.ParseableName;
                }
                SDB.Append(new SfField(SDB.RecordMkr, Section.RecordKey));

                // History
                History_Out();

				// Which verse we've just written out
				DReference CurrentReference = new DReference();
				CurrentReference.Chapter = Section.ReferenceSpan.Start.Chapter; 
			
				// Loop through the paragraphs
				foreach( DParagraph p in Section.Paragraphs)
				{
					// Ignore the ones we inserted to make the parallel rows come out
					if (p.AddedByCluster)
						continue;

					// Pictures
					if (Picture_out(p))
						continue;

					// Vernacular (verse containing) and simple paragraphs
					if (Map.IsVernacularParagraph(p.Style.ToolboxMarker))
					{
                        // Create a field containing just the marker, e.g., \p
                        SfField f = new SfField(p.Style.ToolboxMarker);
                        SDB.Append(f);

                        // Add fields for the paragraph's individual runs
                        if (!p.IsCompletelyEmpty)
                        {
                            foreach (DRun run in p.Runs)
                                run.ToSfm(SDB);
                        }
					}
					else
					{
						AddTextField(p);
					}
				}

				// DateStamp (written only if it has content)
				DateStamp_out();
			}
			#endregion
		}
		#endregion
		#region Method: void ReadStandardFormat(ScriptureDB DB)
		public void ReadStandardFormat(ScriptureDB DB)
		{
			IO io = new IO(DB, this);
			io.ReadSF();
		}
		#endregion
		#region Method: void WriteStandardFormat(ScriptureDB DB)
		public void WriteStandardFormat(ScriptureDB DB)
		{
			IO io = new IO(DB, this);
			io.WriteSF();
		}
		#endregion

		#region Method: void CalculateVersification(ref int nChapter, ref int nVerse)
		public void CalculateVersification(ref int nChapter, ref int nVerse)
		{
            // Scan through the section for the first paragraph that returns a
            // non-zero verse; this will be our first chapter/verse of the section,
            // and we'll use it for any early paragraphs (e.g., section heads)
            foreach (DParagraph p in Paragraphs)
            {
                if (p.VerseI != 0)
                {
                    nVerse = p.VerseI;
                    if (0 != p.ChapterI)
                        nChapter = p.ChapterI;
                    break;
                }
            }
            if (0 == nChapter)
                nChapter = 1;

			// The initial reference for the section defaults to the first thing passed in
			ReferenceSpan.Start.Copy( new DReference(nChapter, nVerse ) );

			// Set the reference for each of the paragraphs
			foreach(DParagraph p in Paragraphs)
			{
				p.ChapterI = nChapter;
				p.VerseI   = nVerse;

				nChapter = p.ChapterF;
				nVerse   = p.VerseF;
			}

			// The final reference for the section
			ReferenceSpan.End.Copy( new DReference(nChapter, nVerse ) );

			// Section Titles, Cross Ref's, etc use the reference of the following paragraph
			for(int i = 0; i < Paragraphs.Count; i++)
			{
				DParagraph p = Paragraphs[i] as DParagraph;

				if ( ! Map.IsSectionEmptyReferenceStyle(p.Style.ToolboxMarker) )
					continue;

				for(int k = i + 1; k < Paragraphs.Count; k++)
				{
					DParagraph pNext = Paragraphs[k] as DParagraph;

					if ( ! Map.IsSectionEmptyReferenceStyle(pNext.Style.ToolboxMarker) )
					{
						p.ChapterI = pNext.ChapterI;
						p.VerseI   = pNext.VerseI;
						break;
					}
				}
			}
		}
		#endregion

        // Oxes ------------------------------------------------------------------------------
        #region Method: void SaveToOxesBook(oxes, nodeBook)
        public void SaveToOxesBook(XmlDoc oxes, XmlNode nodeBook)
        {
            // Save the section's paragraphs
            foreach (DParagraph p in Paragraphs)
                p.SaveToOxesBook(oxes, nodeBook);

            // Save the History note, if any. It will be at the same level as the paragraphs,
            // but on Read we will place it into the section we are currently reading in.
            History.Save(oxes, nodeBook);
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
		#region Method: DParagraph[] GetParagraphs(DReferenceSpan span)
		public DParagraph[] GetParagraphs(DReferenceSpan span)
		{
			// We'll store the paragraphs we find here.
			ArrayList a = new ArrayList();

			// Loop through the paragraphs, finding those which are within the span
			foreach( DParagraph p in Paragraphs)
			{
				// The paragraph is prior to our target span, so keep searching.
				if (p.ReferenceSpan.End < span.Start)
					continue;

				// The paragraph is after our target span, so we're done.
                if (p.ReferenceSpan.Start > span.End)
					break;

				// If we're here, then the paragraph is within the span, so
				// we want to keep it.
				a.Add(p);

				// If, within this paragraph, our EndVerse is part of a verse
				// bridge in the paragraph we are scanning, then we need to
				// set the EndVerse to the final verse of the bridge, because
				// we cannot know how many paragraphs lower that EndVerse might
				// actually appear.
				foreach(DRun run in p.Runs)
				{
					DVerse verse = run as DVerse;
					if (null == verse)
						continue;

					if (verse.VerseNo == span.End.Verse)
						span.End.Verse = verse.VerseNoFinal;
				}
			}

			// Convert to a DParagraph[]
			DParagraph[] v = new DParagraph[ a.Count ];
			for(int i=0; i<v.Length; i++)
				v[i] = a[i] as DParagraph;
			return v;
		}
		#endregion

		#region Method: DPicture GetCorrespondingFrontPicture(DPicture picture)
		DPicture GetCorrespondingFrontPicture(DPicture picture)
		{
			if (picture as DPicture == null)
				return null;

			int cPicture = 0;

			// Get which picture this is in our list
			foreach(DParagraph p in Paragraphs)
			{
				if (p == picture)
					break;
				if (p as DPicture != null)
					cPicture ++;
			}

			// Find it in the front section
			DSection SFront = CorrespondingFrontSection;
			if (SFront == null)
				return null;
			int cFront = 0;
			foreach(DParagraph pf in SFront.Paragraphs)
			{
				if (pf as DPicture != null)
				{
					if (cFront == cPicture)
						return pf as DPicture;
					cFront++;
				}
			}

			return null;
		}
		#endregion
        #region SMethod: bool ParagraphHasNoReference(DParagraph p)
        public static bool ParagraphHasNoReference(DParagraph p)
        {
            var v = new List<ParagraphStyle>()
            { 
                StyleSheet.Section,
                StyleSheet.MinorSection,
                StyleSheet.SectionCrossReference,
                StyleSheet.BookTitle,
                StyleSheet.BookSubTitle,
                StyleSheet.RunningHeader,
                StyleSheet.PictureCaption
            };
            if (v.Contains(p.Style))
                return true;

            return false;
        }
        #endregion
        #region Method: DReference GetReferenceAt(DRun runTarget)
        public DReference GetReferenceAt(DRun runTarget)
        {
            int nChapter = ReferenceSpan.Start.Chapter;
            int nVerse = ReferenceSpan.Start.Verse;

            foreach (DParagraph p in Paragraphs)
            {
                foreach (DRun run in p.Runs)
                {
                    DChapter chapter = run as DChapter;
                    if (null != chapter)
                        nChapter = chapter.ChapterNo;

                    DVerse verse = run as DVerse;
                    if (null != verse)
                        nVerse = verse.VerseNo;

                    if (run == runTarget)
                        return new DReference(nChapter, nVerse);
                }
            }

            // Happens if we have a footnote
            return null;
        }
        #endregion

        #region Method: int CountParagraphsWithStyle(style)
        public int CountParagraphsWithStyle(ParagraphStyle style)
        {
            var c = 0;

            foreach (DParagraph p in Paragraphs)
            {
                if (p.Style == style)
                    ++c;
            }

            return c;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region CLASS: MergeMethod
        public class MergeMethod
        {
            // The Three Sections ------------------------------------------------------------
            #region Attr{g}: DSection Parent
            DSection Parent
            {
                get
                {
                    return m_Parent;
                }
            }
            DSection m_Parent;
            #endregion
            #region Attr{g}: DSection Mine
            DSection Mine
            {
                get
                {
                    return m_Mine;
                }
            }
            DSection m_Mine;
            #endregion
            #region Attr{g}: DSection Theirs
            DSection Theirs
            {
                get
                {
                    return m_Theirs;
                }
            }
            DSection m_Theirs;
            #endregion

            // Notes -------------------------------------------------------------------------
            #region Method: TranslatorNote GetCorrespondingNote(vNotes, note)
            TranslatorNote GetCorrespondingNote(List<TranslatorNote> v, TranslatorNote note)
            {
                foreach (var n in v)
                {
                    if (note.IsSameOriginAs(n))
                        return n;
                }
                return null;
            }
            #endregion

            #region method: void MergeCorrespondingNotes(vParents, vOurs, vTheirs)
            void MergeCorrespondingNotes(
                List<TranslatorNote> vParents, 
                List<TranslatorNote> vOurs, 
                List<TranslatorNote> vTheirs)
                // For those where all three exist, do a merge; and then remove them from the
                // Theirs and Parents lists as we no longer need to work with them.
            {
                foreach (var ourNote in vOurs)
                {
                    var theirNote = GetCorrespondingNote(vTheirs, ourNote);
                    var parentNote = GetCorrespondingNote(vParents, ourNote);
                    if (null == theirNote || null == parentNote)
                        continue;

                    ourNote.Merge(parentNote, theirNote);

                    vTheirs.Remove(theirNote);
                    vParents.Remove(parentNote);
                }
            }
            #endregion
            #region method: void RemoveDeletedNotes(vParents, vOurs, vTheirs)
            void RemoveDeletedNotes(
                List<TranslatorNote> vParents,
                List<TranslatorNote> vOurs,
                List<TranslatorNote> vTheirs)
                // If a TranslatorNote exists in the Parent and in Ours (and is identical),
                // but has been deleted from Theirs, then the intention is to delete it, and
                // we remove it from Parent and Ours
            {
                foreach (var parentNote in vParents)
                {
                    var ourNote = GetCorrespondingNote(vOurs, parentNote);
                    var theirNote = GetCorrespondingNote(vTheirs, parentNote);

                    // Must exist in Ours
                    if (null == ourNote)
                        continue;

                    // Must be identical to the Parent
                    if (!ourNote.ContentEquals(parentNote))
                        continue;

                    // Must not exist in Theirs
                    if (null != theirNote)
                        continue;

                    // Remove it from Ours
                    vOurs.Remove(ourNote);
                }
            }
            #endregion
            #region method: void AddTheirNewNotes(vTheirs)
            void AddTheirNewNotes(IEnumerable<TranslatorNote> vTheirs)
            {
                foreach (var theirNote in vTheirs)
                {
                    // Get their context (DText, paragraph, etc.)
                    var theirDText = theirNote.Owner as DText;
                    Debug.Assert(null != theirDText);
                    var theirParagraph = theirDText.Paragraph;
                    Debug.Assert(null != theirParagraph);
                    var iText = theirParagraph.Runs.FindObj(theirDText);
                    var iParagraph = Theirs.Paragraphs.FindObj(theirParagraph);
                    Debug.Assert(-1 != iText);
                    Debug.Assert(-1 != iParagraph);

                    // Make a copy of theirs that we can add
                    var note = theirNote.Clone();

                    // If the sections match in structure, then we can place the it in the same place
                    if (StructuresAreSame(new[] { Mine, Theirs }))
                    {
                        var ourParagraph = Mine.Paragraphs[iParagraph];
                        Debug.Assert(null != ourParagraph);
                        var ourDText = ourParagraph.Runs[iText] as DText;
                        Debug.Assert(null != ourDText);
                        ourDText.TranslatorNotes.Append(note);
                        continue;
                    }

                    // If here, the sections don't match in structure. (Once we go to a form
                    // of OXES with unique id's for paragraphs, this problem should get easier
                    if (iParagraph > Mine.Paragraphs.Count - 1)
                    {
                        iParagraph = Mine.Paragraphs.Count - 1;
                        iText = Mine.Paragraphs[iParagraph].Runs.Count - 1;
                    }
                    while (null == Mine.Paragraphs[iParagraph].Runs[iText] as DText)
                    {
                        --iText;
                        if (iText < 0)
                        {
                            --iParagraph;
                            if (iParagraph < 0)
                                break;
                            iText = Mine.Paragraphs[iParagraph].Runs.Count - 1;
                        }
                    }
                    if (iParagraph >= 0 && iText >= 0)
                    {
                        var ourDText = Mine.Paragraphs[iParagraph].Runs[iText] as DText;
                        if (null != ourDText)
                            ourDText.TranslatorNotes.Append(note);
                    }
                }
            }
            #endregion

            #region Method: void MergeNotes()
            void MergeNotes()
            {
                // Collect the relevant notes in lists
                var vOurNotes = Mine.GetAllTranslatorNotes();
                var vTheirNotes = Theirs.GetAllTranslatorNotes();
                var vParentNotes = Parent.GetAllTranslatorNotes();

                // Merge those that correspond
                MergeCorrespondingNotes(vParentNotes, vOurNotes, vTheirNotes);

                // Merge those that have been removed from either child
                RemoveDeletedNotes(vParentNotes, vOurNotes, vTheirNotes);
                RemoveDeletedNotes(vParentNotes, vTheirNotes, vOurNotes);

                // Add whatever remains in "theirs" as new notes that they added.
                AddTheirNewNotes(vTheirNotes);
            }
            #endregion

            // Evaluate Structures -----------------------------------------------------------
            #region SMethod: bool StructuresAreSame(DSection[] sections)
            static bool StructuresAreSame(DSection[] sections)
            {
                for(var i=0; i<sections.Length - 1; i++)
                {
                    var one = sections[i];
                    var two = sections[i + 1];

                    if (one.Paragraphs.Count != two.Paragraphs.Count)
                        return false;
                    if (one.AllFootnotes.Count != two.AllFootnotes.Count)
                        return false;

                    for(var k=0; k<one.Paragraphs.Count; k++)
                    {
                        var p1 = one.Paragraphs[k];
                        var p2 = two.Paragraphs[k];

                        if (p1.Style != p2.Style)
                            return false;
                        if (p1.StructureCodes != p2.StructureCodes)
                            return false;
                    }
                }
                return true;
            }
            #endregion
            #region Method: bool SameStructureMerge()
            bool SameStructureMerge()
            {
                if (!StructuresAreSame(new[] {Parent, Mine, Theirs}))
                    return false;

                // Take care of the translator notes
                MergeNotes();

                // Take care of the paragraphs & Footnotes 
                for (int i = 0; i < Mine.Paragraphs.Count; i++)
                    Mine.Paragraphs[i].Merge(Parent.Paragraphs[i], Theirs.Paragraphs[i]);
                for (int i = 0; i < Mine.AllFootnotes.Count; i++)
                    Mine.AllFootnotes[i].Merge(Parent.AllFootnotes[i], Theirs.AllFootnotes[i]);

                return true;
            }
            #endregion

            #region Attr{g}: DParagraph FirstAvailableNoteParagraph
            DParagraph FirstAvailableNoteParagraph
            {
                get
                {
                    foreach (DParagraph p in Mine.Paragraphs)
                    {
                        if (p.HasDText)
                            return p;
                    }
                    return null;
                }
            }
            #endregion
            #region Attr{g}: DText FirstAvailableNoteText
            DText FirstAvailableNoteText
            {
                get
                {
                    DParagraph p = FirstAvailableNoteParagraph;
                    foreach (DRun run in p.Runs)
                    {
                        if (run as DText != null)
                            return run as DText;
                    }
                    return null;
                }
            }
            #endregion
            #region Method: void DiffStructureMerge()
            void DiffStructureMerge()
            {
                bool bMineHasChanged = !StructuresAreSame( new[] {Mine, Parent});
                bool bTheirsHasChanged = !StructuresAreSame(new[] {Theirs, Parent});

                // If their's hasn't changed, then we're done. We just keep ours.
                if (!bTheirsHasChanged)
                    return;

                // If we've not changed, but they have, then we simply want to use theirs.
                // We just swap the two, so that we can keep the other data around for the
                // text merge.
                if (bTheirsHasChanged && !bMineHasChanged)
                {
                    // Point to the two objects
                    DSection mine = Mine;
                    DBook MyBook = Mine.Book;
                    DSection theirs = Theirs;
                    DBook TheirBook = Theirs.Book;

                    // Get their positions within their owners
                    int iMine = MyBook.Sections.FindObj(Mine);
                    int iTheirs = TheirBook.Sections.FindObj(Theirs);

                    // Remove them from their owners
                    MyBook.Sections.Remove(mine);
                    TheirBook.Sections.Remove(theirs);

                    // Insert them into their new owners
                    MyBook.Sections.InsertAt(iMine, Theirs);
                    TheirBook.Sections.InsertAt(iTheirs, Mine);

                    return;
                }

                // If both have changed, we're out of our league. We keep ours, and just
                // add a note that they'll have to go back through Mercurial's history
                // to reconstruct what happened.
                if (bTheirsHasChanged && bMineHasChanged)
                {
                    // Get the first DParagraph and DText we can write a note onto
                    var paragraph = FirstAvailableNoteParagraph;
                    var text = FirstAvailableNoteText;
                    if (null == paragraph || null == text)
                        return;

                    // Create our note
                    var note = new TranslatorNote
                    {
                        SelectedText = TranslatorNote.MergeAuthor,
                        Behavior = TranslatorNote.General
                    };
                    var sMessage = Loc.GetString("kStructureConflictInMerge",
                        "This section's paragraphing was changed by more than one user. " +
                        "We kept one; you'll need to look at Mercurial's history to see " +
                        "what the other user did; we were not able to keep their changes.");
                    var message = new DMessage(TranslatorNote.MergeAuthor,
                        DateTime.Now, Role.Anyone, sMessage);
                    note.Messages.Append(message);
                    text.TranslatorNotes.Append(note);
                }
            }
            #endregion

            // Reconcile Textual Changes -----------------------------------------------------
            #region Method: string GetParagraphContentsAsFlatString(DParagraph)
            string GetParagraphContentsAsFlatString(DParagraph p)
            {
                string s = "";

                if (null != p as DFootnote && !string.IsNullOrEmpty((p as DFootnote).VerseReference))
                    s += (p as DFootnote).VerseReference + ":";

                foreach (DRun run in p.Runs)
                {
                    s += run.AsString;
                    if (run as DText != null)
                        s += run.ProseBTAsString;
                }
                return s;
            }
            #endregion
            #region Method: string GetSectionContentsAsFlatString(DSection)
            public string GetSectionContentsAsFlatString(DSection section)
            {
                string s = "";

                foreach (DParagraph p in section.Paragraphs)
                    s += GetParagraphContentsAsFlatString(p);

                foreach (DFootnote fn in section.AllFootnotes)
                    s += GetParagraphContentsAsFlatString(fn);

                return s;
            }
            #endregion
            #region Method: DRun PositionToRun(DParagraph, ref iPos)
            DRun PositionToRun(DParagraph p, ref int iPos)
            {
                foreach (DRun run in p.Runs)
                {
                    int len = run.AsString.Length;
                    if (null != run as DText)
                        len += run.ProseBTAsString.Length;

                    if (iPos < len)
                        return run;
                    iPos -= len;
                }
                return null;
            }
            #endregion
            #region Method: DRun PositionToRun(DSection, iPos)
            public DRun PositionToRun(DSection section, int iPos)
            {
                foreach (DParagraph p in section.Paragraphs)
                {
                    DRun run = PositionToRun(p, ref iPos);
                    if (null != run)
                        return run;
                }

                foreach (DFootnote fn in section.AllFootnotes)
                {
                    DRun run = PositionToRun(fn, ref iPos);
                    if (null != run)
                        return run;
                }

                return null;
            }
            #endregion
            #region Method: int[] GetDiffCodes(s)
            int[] GetDiffCodes(string s)
            {
                int[] vnCodes;
                vnCodes = new int[s.Length];
                for (int n = 0; n < s.Length; n++)
                    vnCodes[n] = (int)s[n];
                return vnCodes;
            }
            #endregion

            #region CLASS: DiffItemEx
            class DiffItemEx
            {
                // Attrs ---------------------------------------------------------------------
                #region Attr{g}: Diff.Item Item
                Diff.Item Item
                {
                    get
                    {
                        return m_Item;
                    }
                }
                Diff.Item m_Item;
                #endregion
                #region Attr{g}: DRun MyRun
                public DRun MyRun
                {
                    get
                    {
                        return m_MyRun;
                    }
                }
                DRun m_MyRun;
                #endregion
                #region Attr{g}: DRun TheirRun
                public DRun TheirRun
                {
                    get
                    {
                        return m_TheirRun;
                    }
                }
                DRun m_TheirRun;
                #endregion
                #region Attr{g}: int PosInMyRun
                int PosInMyRun
                {
                    get
                    {
                        Debug.Assert(m_iPosInMyRun >= 0);
                        return m_iPosInMyRun;
                    }
                }
                int m_iPosInMyRun;
                #endregion
                #region Attr{g}: int PosInTheirRun
                int PosInTheirRun
                {
                    get
                    {
                        Debug.Assert(m_iPosInTheirRun >= 0);
                        return m_iPosInTheirRun;
                    }
                }
                int m_iPosInTheirRun;
                #endregion
                #region Attr{g}: string Context
                public string Context
                {
                    get
                    {
                        return m_sContext;
                    }
                }
                string m_sContext;
                #endregion
                #region Attr{g}: string NoteText
                public string NoteText
                {
                    get
                    {
                        return m_sNoteText;
                    }
                }
                string m_sNoteText;
                #endregion

                // Helper Methods ------------------------------------------------------------
                #region Method: string GetContextString()
                const int c_cContextCount = 7;
                string GetContextString()
                {
                    if (null == MyRun)
                        return "";

                    if (!IsDTextChange)
                        return MyRun.AsString;

                    // Fetch the DText
                    var MyText = MyRun as DText;
                    Debug.Assert(null != MyText);

                    // Determine which string: Vernacular or Backtranslation
                    var iPos = PosInMyRun;
                    var sFull = MyText.AsString;
                    if (sFull.Length <= iPos)
                    {
                        iPos -= sFull.Length;
                        sFull = MyText.ProseBTAsString;
                    }

                    // Get the Preceeding Context
                    string sContext = "";
                    for (int c = 0; (c < c_cContextCount) && (iPos - c > 0); c++)
                        sContext = sFull[iPos - c] + sContext;

                    // Add an insertion symbol
                    sContext += "*";

                    // Get the Following Context
                    for (int c = 0; (c < c_cContextCount) && iPos + c < sFull.Length; c++)
                        sContext += sFull[iPos + c];

                    return sContext;
                }
                #endregion
                #region Method: DRun PositionToRun(DSection, iPos)
                public DRun PositionToRun(DSection section, ref int iPos)
                {
                    // Collect the paragraphs and footnotes
                    var v = new List<DParagraph>();
                    foreach (DParagraph p in section.Paragraphs)
                        v.Add(p);
                    foreach (DFootnote fn in section.AllFootnotes)
                        v.Add(fn);

                    // Scan each paragraph/footnote
                    foreach (DParagraph p in v)
                    {
                        foreach(DRun run in p.Runs)
                        {
                            int len = run.AsString.Length;
                            if (null != run as DText)
                                len += run.ProseBTAsString.Length;

                            if (iPos < len)
                                return run;

                            iPos -= len;
                        }
                    }

                    // If here, it means we were appending to the end, and there is
                    // no corresponding run to attach to.
                    return null;
                }
                #endregion
                #region Attr{g}: bool IsDTextChange
                public bool IsDTextChange
                {
                    get
                    {
                        if (null == MyRun as DText)
                            return false;
                        if (null == TheirRun as DText)
                            return false;
                        return true;
                    }
                }
                #endregion
                #region Attt{g}: string GetNoteText()
                string GetNoteText()
                {
                    var sTheirValue = (null == TheirRun) ? "" : TheirRun.AsString;
                    return LocDB.GetValue(
                        new string[] { "Strings " },
                        "mergeNote",
                        "The other version had \"{0}\".",
                        null,
                        new string[] { sTheirValue });
                }
                #endregion
                #region Method: void ConsoleOut()
                public void ConsoleOut()
                {
                    Console.WriteLine("");
                    Console.WriteLine("Item: " +
                       "  StartA=" + Item.StartA.ToString() +
                       "  StartB=" + Item.StartB.ToString() +
                       "  DeleteA=" + Item.deletedA.ToString() +
                       "  InsertB=" + Item.insertedB.ToString());
                    Console.WriteLine("MyRun   =" + MyRun.DebugString);
                    Console.WriteLine("TheirRun=" + TheirRun.DebugString);
                    Console.WriteLine("NoteText=" + GetNoteText());
                }
                #endregion

                // Scaffolding ---------------------------------------------------------------
                #region Constructor(SMine, STheirs, Diff.Item)
                public DiffItemEx(DSection SMine, DSection STheirs, Diff.Item item )
                {
                    m_Item = item;

                    m_iPosInMyRun = Item.StartA;
                    m_MyRun = PositionToRun(SMine, ref m_iPosInMyRun);

                    m_iPosInTheirRun = Item.StartB;
                    m_TheirRun = PositionToRun(STheirs, ref m_iPosInTheirRun);

                    m_sContext = GetContextString();
                    m_sNoteText = GetNoteText();
                }
                #endregion
            }
            #endregion

            #region Method: void MakeNotesOfTextualChanges()
            void MakeNotesOfTextualChanges()
            {
                // Collect our two versions as flat strings
                string sMine = GetSectionContentsAsFlatString(Mine);
                string sTheirs = GetSectionContentsAsFlatString(Theirs);

                // Apply the Diff process
                int[] vnMine = GetDiffCodes(sMine);
                int[] vnTheirs = GetDiffCodes(sTheirs);
                Diff.Item[] vDiffItems = Diff.DiffInt(vnMine, vnTheirs);

                // Adjust to our context (create the Extended versions of Item)
                var vItemsEx = new List<DiffItemEx>();
                foreach (Diff.Item item in vDiffItems)
                    vItemsEx.Add(new DiffItemEx(Mine, Theirs, item));

                // Loop through the changes, creating notes
                DRun LastNoteAttachedTo = null;
                foreach (DiffItemEx itemx in vItemsEx)
                {
                    // For Debugging
                    // itemx.ConsoleOut();

                    // Get the closest DText to append a note to. Normally this will
                    // be done by casting MyRun to a DText. That fails, though, in 
                    // the case of a footnote, in which case we just ignore the
                    // change. (TODO: handles it when MyRun is a DFoot). 
                    var AttachTo = itemx.MyRun as DText;
                    if (null == AttachTo)
                        continue;
                    
                    var note = new TranslatorNote();
                    var reference = Mine.GetReferenceAt(itemx.MyRun);
                    if (null == reference)
                        continue;
                    note.SelectedText = itemx.Context;
                    var message = new DMessage(
                        TranslatorNote.MergeAuthor,
                        DateTime.UtcNow,
                        Role.Anyone,
                        itemx.NoteText);
                    note.Messages.Append(message);

                    // We are just doing a generic note for now, rather than showing
                    // diffs, so we only do this once for a given MyRun.
                    if (AttachTo == LastNoteAttachedTo)
                        continue;
                    LastNoteAttachedTo = AttachTo;

                    // Attach the note
                    AttachTo.TranslatorNotes.Append(note);
                }

            }
            #endregion

            // Public
            #region Constructor(Parent, Mine, Theirs)
            public MergeMethod(DSection _Parent, DSection _Mine, DSection _Theirs)
            {
                m_Parent = _Parent;
                m_Mine = _Mine;
                m_Theirs = _Theirs;
            }
            #endregion
            #region Method: void Run()
            public void Run()
            {
                // Where the structure doesn't change, this is elegant and nice and
                // preferred. So we do the simple merge and we're done. Yeah!
                if (SameStructureMerge())
                    return;

                // Keep whichever DSection has changed structure. If both have changed,
                // keep ours and make a TranslatorNote telling them we couldn't preserve
                // theirs.
                DiffStructureMerge();

                // Now that we've accomplished the structure (and potentially chosen their
                // section over ours), we can merge the notes.
                MergeNotes();

                // Merge the histories
                Mine.History.Merge(Parent.History, Theirs.History);

                // Use Diff to reconcile any textual changes
                MakeNotesOfTextualChanges();
            }
            #endregion
        }
        #endregion
        #region Method: void Merge(Parent, Theirs)
        public void Merge(DSection Parent, DSection Theirs)
        {
            MergeMethod m = new MergeMethod(Parent, this, Theirs);
            m.Run();
        }
        #endregion
    }
}
