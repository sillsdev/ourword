/**********************************************************************************************
 * Project: Our Word!
 * File:    DSection.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles a section (passage) of Scripture
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using JWTools;
using JWdb;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.DataModel
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
		#region JAttr{g}: JOwnSeq Footnotes - Includes footnotes and SeeAlso's
        public JOwnSeq<DParagraph> Footnotes
		{
			get
			{
				return m_osFootnotes;
			}
		}
        private JOwnSeq<DParagraph> m_osFootnotes;
		#endregion
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
		#region Attr{g}: JStyleSheet StyleSheet - returns the Project-owned stylesheet
		private JStyleSheet StyleSheet
		{
			get
			{
				return Project.StyleSheet;
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
				return Project.TeamSettings.SFMapping;
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
					if (p.StyleAbbrev == Map.StyleSection)
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
					if (pg.StyleAbbrev == Map.StyleCrossRef)
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
				return CheckAllParagraphsMatch(G.Project.SFront);
			}
		}
		#endregion
		#region Method: bool CheckAllParagraphsMatch(DSection SFront)
		public bool CheckAllParagraphsMatch(DSection SFront)
		{
			// Are the paragraph and footnote counts the same?
			if (SFront.Paragraphs.Count != Paragraphs.Count)
				return false;
			if (SFront.Footnotes.Count != Footnotes.Count)
				return false;

			// Do we have the same styles in the paragraphs & footnotes?
			for(int i=0; i < Paragraphs.Count; i++)
			{
				DParagraph PFront = SFront.Paragraphs[i] as DParagraph;
				if (PFront.StyleAbbrev != (Paragraphs[i] as DParagraph).StyleAbbrev)
					return false;
			}
			for(int i=0; i < Footnotes.Count; i++)
			{
				DParagraph PFront = SFront.Footnotes[i] as DParagraph;
				if (PFront.StyleAbbrev != (Footnotes[i] as DParagraph).StyleAbbrev)
					return false;
			}

			// If we made it here, then they are the same
			return true;
		}
		#endregion
		#region Attr{g}: DRun[] AllParagraphRuns
		public DRun[] AllParagraphRuns
		{
			get
			{
				ArrayList a = new ArrayList();

				foreach(DParagraph p in Paragraphs)
				{
					foreach(DRun r in p.Runs)
						a.Add(r);
				}

				// Convert into an array of runs
				DRun[] vRuns = new DRun[ a.Count ];
				for(int i=0; i<a.Count; i++)
					vRuns[i] = a[i] as DRun;

				return vRuns;
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

		// Scaffolding -----------------------------------------------------------------------
		public int m_nSectionNo = -1;
		#region Constructor()
		public DSection(int nSectionNo)
			: base()
		{
			m_nSectionNo = nSectionNo;

            // Paragraphs and Footnotes: flags are
            // - Don't check for duplicates
            // - Don't sort
            m_osParagraphs = new JOwnSeq<DParagraph>("Paras", this, false, false);
            m_osFootnotes = new JOwnSeq<DParagraph>("Footnotes", this, false, false);

            // Scripture Reference Span
            j_ownReferenceSpan = new JOwn<DReferenceSpan>("RefSpan", this);
            ReferenceSpan = new DReferenceSpan();

		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DSection objSection = (DSection)obj;
			return m_nSectionNo == objSection.m_nSectionNo;
		}
		#endregion
		#region Method: void InitializeFromFrontSection(DSection SFront)
		public void InitializeFromFrontSection(DSection SFront)
		{
			m_nSectionNo = SFront.m_nSectionNo;

			// Duplicate the Front's reference
			Debug.Assert(SFront.ReferenceSpan.Start.Chapter > 0);
			ReferenceSpan.CopyFrom(SFront.ReferenceSpan);

			// Clear out the footnotes; they will be created as the paragraphs are created
			Footnotes.Clear();

			// Duplicate the Front's paragraphs
			Paragraphs.Clear();
			foreach(DParagraph pFront in SFront.Paragraphs)
			{
				DParagraph p = null;

				if (null != pFront as DPicture)
					p = new DPicture();
				else
					p = new DParagraph();

				Paragraphs.Append(p);
				p.CopyFrom(pFront, true);
			}

			// Exegetical notes
			UpdateExegesisNotes(SFront);
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

			// Exegetical notes
			UpdateExegesisNotes(SFront);
		}
		#region Helper Method: ArrayList _GetCrossRefParagraphs(DSection)
		ArrayList _GetCrossRefParagraphs(DSection section)
		{
			ArrayList v = new ArrayList();
			foreach(DParagraph p in section.Paragraphs)
			{
				if (Map.StyleCrossRef == p.StyleAbbrev)
					v.Add(p);
			}
			return v;
		}
		#endregion
		#region Helper Method: ArrayList _GetCrossRefFootnotes(DSection, DFootnote.Types)
		ArrayList _GetCrossRefFootnotes(DSection section, DFootnote.Types fnType)
		{
			ArrayList v = new ArrayList();
			foreach(DFootnote fn in section.Footnotes)
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

				pTarget.SimpleText = pTarget.Translation.ConvertCrossReferences(pFront);
			}
		}
		#endregion
		#region Helper Method: void _UpdateExplanatoryLabels(vFront, vTarget)
		void _UpdateExplanatoryLabels(ArrayList vFront, ArrayList vTarget)
		{
			if (vFront.Count != vTarget.Count)
				return;

			if (DFootnote.RefLabelTypes.kNone == DFootnote.RefLabelType)
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
		#region Method: void UpdateExegesisNotes( DSection SFront)
		public void UpdateExegesisNotes( DSection SFront)
		{
			if (SectionMatchesFront(SFront))
			{
				for(int i=0; i<Paragraphs.Count; i++)
				{
					DParagraph pTarget = Paragraphs[i] as DParagraph;
					DParagraph pSource = SFront.Paragraphs[i] as DParagraph;

					if (null != pTarget && null != pSource)
						pTarget.UpdateExegesisNotes(pSource);
				}
			}
		}
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
				if (DStyleSheet.IsQuoteStyle( p.StyleAbbrev ))
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool FilterTest_HasNoteOfType( DNote.Types kType)
		public bool FilterTest_HasNoteOfType( DNote.Types kType)
		{
			foreach(DParagraph p in Paragraphs)
			{
				foreach( DRun r in p.Runs)
				{
					DText text = r as DText;
					if (null != text)
					{
						DNote note = text.GetNoteOfType(kType);
						if (null != note)
							return true;
					}
				}
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
			string sAbbrev = p.StyleAbbrev;
			if (G.Map.IsTitleStyle(sAbbrev))
				return false;
			if (G.Map.StyleSection == sAbbrev)
				return false;
			if (G.Map.StyleSection2 == sAbbrev)
				return false;
			if (G.Map.StyleSubTitle == sAbbrev)
				return false;
			if (G.Map.StylePicCaption == sAbbrev)
				return false;
			if (G.Map.StyleCrossRef == sAbbrev)
				return false;
			if (G.Map.StyleHeader == sAbbrev)
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
				DFootLetter foot = p.Runs[i] as DFootLetter;
				DSeeAlso  also = p.Runs[i] as DSeeAlso;
				if (null == foot && null == also)
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
			JWritingSystem ws = G.Project.TargetTranslation.WritingSystemVernacular;
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
				DPicture pict = p as DPicture;
				if (null == pict)
					continue;

				// If there is no directory, we'll consider this a problem (e.g., we
				// can't locate the picture becquse no picture has been defined!)
				if (pict.PathName.Length == 0)
					return true;

				// See if the picture exists
				bool bFileExists = File.Exists(pict.PathName);
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
			bool bNoteToDo,
			bool bNoteAskUNS,
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

			// Note Type Tests
			if (bNoteToDo)
				a.Add( FilterTest_HasNoteOfType( DNote.Types.kToDo ) );
			if (bNoteAskUNS)
				a.Add( FilterTest_HasNoteOfType( DNote.Types.kAskUns ) );

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
			#region Attr{g}: ScriptureDB DB
			ScriptureDB DB
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
					return G.TeamSettings.SFMapping;
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

			char m_chLetter = 'a';

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
			static public int s_nChapter = 0;
			static public int s_nVerse   = 0;

			// Converting SfField > DRun -----------------------------------------------------
			#region Class: Phrase - Helper for parsing input string into Scripture Text phrases
			public class Phrase
			{
				// Constants ---------------------------------------------------------------------
				const char c_chVerticalBar = '|';

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
				#region Attr{g/s}: string StyleAbbrev
				public string StyleAbbrev
				{
					get
					{
						return m_sStyleAbbrev;
					}
					set
					{
						Debug.Assert(null != value && value.Length > 0);
						m_sStyleAbbrev = value;
					}
				}
				string m_sStyleAbbrev = "p";
				#endregion

				// Scaffolding -------------------------------------------------------------------
				#region Constructor(sText, Type)
				private Phrase(string sText, string sStyleAbbrev)
				{
					Text = sText;
					StyleAbbrev = sStyleAbbrev;
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
					string sText = "";

					// Are we setting at a footnote marker? This is defined as a |fn. 
					// If so, then declare the type and we are done.
					if (IsFootnote(sIn, iPos))
					{
						iPos += 3;

						// Eat any white space that follows the footnote
						while (iPos < sIn.Length-1 && sIn[iPos] == ' ')
							++iPos;

						return new Phrase("", DStyleSheet.c_StyleAbbrevFootLetter);
					}

					// Default to Normal; if we encounter a style we will change it.
					string sStyleAbbrev = DStyleSheet.c_StyleAbbrevNormal;

					// Are we sitting at a style? This is defined as a opening bar followed
					// by a recognized style character. If so, then retrieve the
					// styleabbrev (which is that character) and increment beyond the style 
					// declaration.segment
					if (IsStyleBegin(sIn, iPos))
					{
						sStyleAbbrev = (sIn[iPos + 1]).ToString();
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

					return (sText.Length > 0) ? new Phrase(sText, sStyleAbbrev) : null ;
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
			#region Method: static void CombineLikePhrases( ArrayList vRuns )
			static private void CombineLikePhrases( ArrayList vRuns)
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
				for(int i=0; i<os.Count - 1; )
				{
					DPhrase p1 = os[i] as DPhrase;
					DPhrase p2 = os[i+1] as DPhrase;

					if (p1.CharacterStyleAbbrev == p2.CharacterStyleAbbrev)
					{
						p1.Text = CombineStrings(p1.Text, p2.Text);
						os.Remove(p2);
					}
					else
						i++;
				}
			}
			#endregion

			#region Method: static ArrayList GetPhrases(string sSource)
			static ArrayList GetPhrases(string sSource)
			{
				ArrayList v = new ArrayList();

				Phrase p = null;
				int iPos = 0;

				while ( (p = Phrase.GetPhrase( sSource, ref iPos)) != null)
					v.Add(p);

				return v;
			}
			#endregion

			#region Method: static int _AdvancePastFootnote(int i, ArrayList vRaw)
			static int _AdvancePastFootnote(int i, ArrayList vRaw)
			{
				if (i < vRaw.Count)
				{
					Phrase raw = vRaw[i] as Phrase;

					if (raw.StyleAbbrev == DStyleSheet.c_StyleAbbrevFootLetter)
						i++;
				}

				return i;
			}
			#endregion
			#region Method: static ArrayList _CreateDRuns(SfField, ref char chNextFootLetter)
			static ArrayList _CreateDRuns(SfField field, ref char chNextFootLetter)
			{
				// Retrieve the raw phrases
				ArrayList vTextRawPhrases = GetPhrases( field.Data );

				// Loop to create the DRun's and populate the vRuns list
				ArrayList vRuns = new ArrayList();
				for( int i=0; i<vTextRawPhrases.Count; i++ )
				{
					Phrase raw = vTextRawPhrases[i] as Phrase;

					// Footnote: add the new run into the array, then loop to the next one
					if (raw.StyleAbbrev == DStyleSheet.c_StyleAbbrevFootLetter)
					{
                        DFootLetter foot = DFootLetter.Create(chNextFootLetter++, null);
                        if (null != foot)  // TODO: Fails if we go beyond 'z'
						    vRuns.Add( foot );
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
					DPhrase phrase = new DPhrase( raw.StyleAbbrev, raw.Text);
					txt.Phrases.Append(phrase);
				}

				return vRuns;
			}
			#endregion
			#region Method: static ArrayList _GetNextDPhrases(ref int i, ArrayList vRaw)
			static ArrayList _GetNextDPhrases(ref int i, ArrayList vRaw)
			{
				// We'll put the DPhrases we create into this arraylist
				ArrayList vPhrases = new ArrayList();

				// Loop from the curent position until the end
				while (i < vRaw.Count)
				{
					// Retrieve the next phrase
					Phrase raw = vRaw[i] as Phrase;

					// If the Phrases's style is a foot letter, then we are done, as
					// we are not dealing with a DPhrase.
					if (raw.StyleAbbrev == DStyleSheet.c_StyleAbbrevFootLetter)
						break;

					// Create the phrase and add it to the destination array
					DPhrase phrase = new DPhrase( raw.StyleAbbrev, raw.Text);
					vPhrases.Add(phrase);

					// Move on to process the next item in vRaw
					i++;
				}

				return vPhrases;
			}
			#endregion
			#region Method: static DRun[] FieldToRuns(SfField field, ref char chNextFootLetter)
			static public DRun[] FieldToRuns(SfField field, ref char chNextFootLetter)
			{
				// Loop to create the DRun's and populate the vRuns list
				ArrayList vRuns = _CreateDRuns(field, ref chNextFootLetter);

				// Retrieve the raw phrases (Vernacular and Back Translation)
				ArrayList vBTRawPhrases   = GetPhrases( field.BT );
				ArrayList vIBTRawPhrases  = GetPhrases( field.IBT );

				// Reconcile the BT Phrases into the Runs
				int iBT = 0;
				int iIBT = 0;
				foreach( DRun run in vRuns)
				{
					// Deal with a footnote if that is what we currently have
					DFootLetter foot = run as DFootLetter;
					if (null != foot) 
					{
						iBT  = _AdvancePastFootnote(iBT,  vBTRawPhrases);
						iIBT = _AdvancePastFootnote(iIBT, vIBTRawPhrases);
						continue;
					}

					// If we are here, then we are dealing with Scripture Text. So
					// get the DText we'll be working with.
					DText txt = run as DText;

					// Retrieve the Prose BT's DPhrases and add them into the DText's
					// BT attribute.
					ArrayList vDPhrasesBT = _GetNextDPhrases(ref iBT, vBTRawPhrases);
					foreach(DPhrase p in vDPhrasesBT)
						txt.PhrasesBT.Append(p);

					// Make sure there is at least an empty phrase for the run's Prose BT
					if (txt.PhrasesBT.Count == 0)
					{
						DPhrase phrase = new DPhrase(DStyleSheet.c_StyleAbbrevNormal, "");
						txt.PhrasesBT.Append(phrase);
					}

				}

				// Did we have any Back Translations left over? This could happen if
				// the footnotes or phrases did not line up. 
				if (iBT < vBTRawPhrases.Count)
				{
					// Append all the remaining phrases into a single string
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
							DPhrase phrase = new DPhrase(DStyleSheet.c_StyleAbbrevNormal, sBT);
							txt.PhrasesBT.Append(phrase);
							break;
						}

						// Otherwise, increment down to try the preceding one
						iText--;
					}
				}

				// Combine like phrases
				CombineLikePhrases(vRuns);

				// Convert to an array
				DRun[] aRuns = new DRun[ vRuns.Count ];
				for(int i=0; i<vRuns.Count; i++)
					aRuns[i] = vRuns[i] as DRun;

				return aRuns;
			}
			#endregion
			#region Method: void AddParagraphText(DParagraph, sfField)
			public void AddParagraphText(DParagraph p, SfField field)
			{
				DRun[] runs = FieldToRuns( field, ref m_chLetter );
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
				if (p.StyleAbbrev ==  Map.StyleHeader )    
					return Map.MkrHeader;
				if (p.StyleAbbrev ==  Map.StyleMainTitle )  
					return "mt";
				if (p.StyleAbbrev ==  Map.StyleSubTitle )  
					return "st";
				if (p.StyleAbbrev ==  Map.StyleSection )   
					return Map.MkrSection;
				if (p.StyleAbbrev ==  Map.StyleSection2 )   
					return Map.MkrSection2;
				if (p.StyleAbbrev ==  Map.StyleCrossRef )  
					return Map.MkrCrossRef;
                if (p.StyleAbbrev == DStyleSheet.c_StyleMajorSection)
                    return DStyleSheet.c_StyleMajorSection;
                if (p.StyleAbbrev == DStyleSheet.c_StyleMajorSectionCrossRef)
                    return DStyleSheet.c_StyleMajorSectionCrossRef;

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
					DB.Append( new SfField( GetMarkerFromStyle(p) ) );
					return;
				}

				// If we are here, we do not expect to have a paragraph with a DVerse,
				// DChapter, or DSeeAlso.
				foreach( DRun run in p.Runs)
				{
					if (null != run as DVerse || 
						null != run as DChapter || 
						null != run as DSeeAlso)
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
					if (null != p.Runs[i] as DFootLetter)
						++cFootnotes;
				}

				// Each pass through this loop corresponds to a \vt field being produced.
				while (iPosFirst <= iPosLast)
				{
					// Build a list of the Translator Notes that we'll output
                    List<TranslatorNote> listTranslatorNotes = new List<TranslatorNote>();
					ArrayList listNotes = new ArrayList();

					// We will build the \vt data here
					string sContents = "";
					string sProseBT  = "";
					string sIntBT    = "";

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

							foreach(DNote note in text.Notes)
								listNotes.Add(note);
						}

						DFootLetter foot = run as DFootLetter;
						if (null != foot)
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
					DB.Append(field);

					// If we have a footnote, add it to the database
					if (null != footnote)
						Footnote_out(footnote);

					// Add the Translator Notes
					foreach(DNote note in listNotes)
						note.ToDB(DB);
                    foreach (TranslatorNote tn in listTranslatorNotes)
                        tn.AddToSfmDB(DB);
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
					pict.PathName = field.Data;
				if (DSFMapping.c_sMkrPictureWordRtf == field.Mkr)
					pict.WordRtfInfo = field.Data;
				if (DSFMapping.c_sMkrPictureCaption == field.Mkr )
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

				DB.Append( new SfField(DSFMapping.c_sMkrPicturePath, pict.PathName));
				DB.Append( new SfField(DSFMapping.c_sMkrPictureWordRtf,pict.WordRtfInfo));

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
					DParagraph p = new DParagraph();
					Section.Paragraphs.Append(p);

					p.StyleAbbrev = sStyle;
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
					DFootnote fn = new DFootnote(s_nChapter, s_nVerse,
                        DFootnote.Types.kExplanatory);
					Section.Footnotes.Append(fn);

					// Connect up the DRun in the most recent paragraph to this footnote
					// (or alternatively, a |fn will be added.)
					LastParagraph.ConnectFootnote(ref m_chLetter, fn);

					// Footnote label
					string sLabel;
					field.BT = DFootnote.ParseLabel(field.BT, out sLabel);
					field.Data = DFootnote.ParseLabel(field.Data, out sLabel);
					if (sLabel.Length > 0)
					{
						fn.Runs.Append( new DLabel(sLabel) );
					}

					// Populate the Contents and ProseBT of the footnote
					AddParagraphText(fn, field);

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

			// Status ------------------------------------------------------------------------
			#region Method: bool CheckingStatus_in(SfField field)
			private bool CheckingStatus_in(SfField field)
			{
				if (!Map.IsStatusCommentMarker( field.Mkr ) )
					return false;

				if (Section.StatusComment.Length > 0)
					Section.StatusComment += " ";

				Section.StatusComment += field.Data;

				return true;
			}
			#endregion
			#region Method: void CheckingStatus_out()
			private void CheckingStatus_out()
			{
				if (Section.StatusComment.Length > 0)
				{
					DB.Append( new SfField(Map.MkrStatusComment, Section.StatusComment));
				}
			}
			#endregion

            // Translator Notes --------------------------------------------------------------
            bool m_bConvertNotes = true;
            bool TranslatorNote_in(SfField field)
            {
                TranslatorNote tn = null;

                // Is this field a TranslatorNote?
                if (DSFMapping.c_sMkrTranslatorNote == field.Mkr)
                {
                    XElement[] x = XElement.CreateFrom(field.Data);
                    if (x.Length == 1)
                    {
                        tn = new TranslatorNote();
                        tn.FromXml(x[0]);
                    }
                }

                // Otherwise, is it an old-style (pre version 1.1) note?
                else
                {
                    if (m_bConvertNotes)
                    {
                        tn = TranslatorNote.ImportFromOldStyle(
                            s_nChapter, s_nVerse, field);
                    }
                }

                // If we don't have a note, then we're done processing
                if (null == tn)
                    return false;

                // Make sure we have a paragraph we can put it into
                if (null == LastParagraph)
                {
                    throw new eBookReadException(
                        G.GetLoc_Messages("msgMissingParagraphMarkerForNote",
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

			// Notes -------------------------------------------------------------------------
			#region Method: bool Note_in(SfField)
			private bool Note_in(SfField field)
			{
				// Determine if we are dealing with a note; if not, return false.
				DNote.Types NoteType = DNote.GetTypeFromMarker(field.Mkr);
				if (DNote.Types.kUnknown == NoteType)
					return false;

                if (m_bConvertNotes)
                {
                    // Should not get here now that TranslatorNote_in is implemented
                    Debug.Assert(false);
                }

				// Now that I've reorganized notes from paragraphs to DTexts, I need to
				// get rid of the old {v 2} sequences.
				field.Data = DNote.ConvertOldVerseReferences(field.Data);

				// Convert the note text to its component runs. There should only be one,
				// and it should be a DText. (We still return "true", because even though
				// we aren't adding a note; we are still indeed finished with the field.)
				char chLetter = 'a';
				DRun[] runs = FieldToRuns(field, ref chLetter);
				if (chLetter != 'a' || runs.Length != 1)
					return true;
				DText noteText = runs[0] as DText;
				if (null == noteText)
					return true;

				// For a note, we really want a Basic Text, so we must do a conversion
				DBasicText bt = new DBasicText(noteText);

				// Make sure we have a paragraph we can put it into
				if (null == LastParagraph)
				{
					throw new eBookReadException(
                        G.GetLoc_Messages("msgMissingParagraphMarkerForNote",
                            "A translator note was encountered but there was no paragraph marker " +
                            "for it to go into."),
						HelpSystem.Topic.kErrMissingParagraphMarkerForNote,
						field.LineNo);
				}

				// Locate the note if it already exists in the paragraph
				DNote note = LastParagraph.GetNoteOfType(NoteType);

				// If it doesn't exist (the normal case, go ahead and add the new one);
				// then return.
				if (null == note)
				{
					string sReference = s_nVerse.ToString();

                    if (DSection.ParagraphHasNoReference(LastParagraph))
                    {
                        string sNonVersePara = G.GetLoc_StyleName(LastParagraph.Style.DisplayName);
                        if (0 != sNonVersePara.Length)
                            sReference = sNonVersePara;
                    }

					note = new DNote(sReference, bt, NoteType);
					DText text = LastParagraph.GetOrAddLastDText();
					text.Notes.Append(note);
					return true;
				}

				// If we have gotten here, then we are dealing with data in which multiple notes
				// are attached to \vt's, rather than to the entire paragraph. So we need to
				// append some extra information to help the user track where the note came from.
				// We simply add the verse (for now, in italic font)
				string sVerseString = "(" + s_nVerse.ToString() + ") ";
				DPhrase phrase = new DPhrase("i", sVerseString);
				bt.Phrases.InsertAt(0, phrase);
				note.NoteText.Append(bt, true);
				return true;
			}
			#endregion
            /***
			#region Method: void Note_out(DNote note)
			void Note_out(DNote note)
			{
				note.ToDB(DB);
			}
			#endregion
            ***/

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
					DB.Append( new SfField(Map.MkrDateStamp, Section.DateStamp));
			}
			#endregion

			#region Method: bool AddVernPara(SfField)
			bool AddVernPara(SfField field)
			{
				// Test to see if we should process this marker
				if ( !Map.IsVernacularParagraph( field.Mkr ) && 
					!Map.IsCrossRef( field.Mkr) &&
					!Map.IsSection( field.Mkr ) &&
					!Map.IsSection2(field.Mkr) )
				{
					return false;
				}

				// Create the new paragraph. The style is the same as the field marker
				DParagraph p = new DParagraph();
				Section.Paragraphs.Append(p);

				p.StyleAbbrev = field.Mkr;
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
                        G.GetLoc_Messages("msgChapterNotInParagraph",
                            "A \\c was encountered but there was no paragraph marker before it."),
						HelpSystem.Topic.kErrChapterNotInParagraph,
						field.LineNo);

				// Add it to the paragraph
				DChapter chapter = DChapter.Create( field.Data );
				if (null == chapter)
				{
					throw new eBookReadException(
                        G.GetLoc_Messages("msgBadChapterNo",
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
				if ( ! Map.IsVernacularParagraph( LastParagraph.StyleAbbrev ) )
					throw new eBookReadException(
                        G.GetLoc_Messages("msgMissingParagraphMarker",
                            "A verse field was encountered but there was no paragraph marker for the verse to go into."),
						HelpSystem.Topic.kErrMissingParagraphMarker,
						field.LineNo);

				// Create the DRun subclass and add it to the paragraph
				DVerse verse = DVerse.Create( field.Data );
				if (null == verse)
				{
					throw new eBookReadException(
                        G.GetLoc_Messages( "msgMissingVerseNumber",
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
				if ( ! Map.IsVernacularParagraph( LastParagraph.StyleAbbrev ) )
					throw new eBookReadException(
                        G.GetLoc_Messages( "msgMissingParagraphMarkerForCF",
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
				DFootnote fn = new DFootnote(s_nChapter, s_nVerse,
                    DFootnote.Types.kSeeAlso);
				Section.Footnotes.Append(fn);
				AddParagraphText(fn, field);

				// Add the See Also text to the paragraph
				LastParagraph.ConnectFootnote(ref m_chLetter, fn);

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
				SfField fSection = DB.GetCurrentField();
                Section.RecordKey = fSection.Data.Trim();

				// As we come across |fn's in the text, we will first increment this following,
				// and then use it to create the in-line marker, e.g., "{fn c}".
				m_chLetter = 'a';

				// Read Loop: reads in each SF field, and deals with it appropriately
				do
				{
					// Retrieve the next field
					SfField field = DB.GetNextField();
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
                        DStyleSheet.c_StyleMajorSection))
                        continue;
                    if (AddSimple(field, Map.MkrMajorSectionCrossRef, 
                        DStyleSheet.c_StyleMajorSectionCrossRef))
                        continue;

					// Checking Status, DateStamp
					if ( CheckingStatus_in(field) )
						continue;
					if ( DateStamp_in(field))
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
                    if (TranslatorNote_in(field))
                        continue;
					if ( Note_in(field))
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
					if (!DB.CurrentFieldIsRecordMarker)
					{
						Console.WriteLine("Unhandled Marker at " + field.LineNo + 
							" ---->" + field.Mkr + " - " + field.Data);

						if (field.Data.Length > 0 && null != LastParagraph)
						{
							field.Data = "(" + field.Mkr + ") " + field.Data;
							field.Mkr  = "nt";
							Note_in(field);
						}
					}

                } while (!DB.CurrentFieldIsRecordMarker);

				// Was there a verse in the section?
				if (!VerseNumberFound)
					throw new eBookReadException(
                        G.GetLoc_Messages( "msgNoVerseInSection",
                            "A section (\\s) was encountered that did not have a verse (\\v) field in it."),
						HelpSystem.Topic.kErrNoVerseInSection,
						fSection.LineNo);

				// Deal with spaces
				foreach(DParagraph p in Section.Paragraphs)
					p.Cleanup();

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
                DB.Append(new SfField(DB.RecordMkr, Section.RecordKey));

				// Checking status (written only if it has content)
				CheckingStatus_out();

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
					if (Map.IsVernacularParagraph(p.StyleAbbrev))
					{
                        // Create a field containing just the marker, e.g., \p
                        DB.Append(new SfField(p.StyleAbbrev));

                        // Add fields for the paragraph's individual runs
                        if (!p.IsCompletelyEmpty)
                        {
                            foreach (DRun run in p.Runs)
                                run.ToSfm(DB);
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

				if ( ! Map.IsSectionEmptyReferenceStyle(p.StyleAbbrev) )
					continue;

				for(int k = i + 1; k < Paragraphs.Count; k++)
				{
					DParagraph pNext = Paragraphs[k] as DParagraph;

					if ( ! Map.IsSectionEmptyReferenceStyle(pNext.StyleAbbrev) )
					{
						p.ChapterI = pNext.ChapterI;
						p.VerseI   = pNext.VerseI;
						break;
					}
				}
			}
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

		#region Method: bool SectionMatchesFront(DSection SFront) - T if a true daughter matching section
		public bool SectionMatchesFront(DSection SFront)
		{
			if (!SFront.ReferenceSpan.ContentEquals(this.ReferenceSpan))
				return false;
			if (SFront.Paragraphs.Count != this.Paragraphs.Count)
				return false;
			return true;
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
            if (p.StyleAbbrev == G.Map.StyleSection)
                return true;
            if (p.StyleAbbrev == G.Map.StyleSection2)
                return true;
            if (p.StyleAbbrev == G.Map.StyleCrossRef)
                return true;
            if (p.StyleAbbrev == G.Map.StyleMainTitle)
                return true;
            if (p.StyleAbbrev == G.Map.StyleSubTitle)
                return true;
            if (p.StyleAbbrev == G.Map.StyleHeader)
                return true;
            if (p.StyleAbbrev == G.Map.StylePicCaption)
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
                    {
                        nChapter = chapter.ChapterNo;
                        continue;
                    }

                    DVerse verse = run as DVerse;
                    if (null != verse)
                    {
                        nVerse = verse.VerseNo;
                        continue;
                    }

                    if (run == runTarget)
                    {
                        return new DReference(nChapter, nVerse);
                    }
                }
            }

            Debug.Assert(false, "GetReferenceAt was called for a section that does not have the runTarget");
            return null;
        }
        #endregion

        #region Method: string GetNoteReference(DNote note)
        public string GetNoteReference(DNote note)
            // Called when inserting a new note, where we want to get the text to display
            // to the left of the note in the Notes Pane.
        {
            int nChapter = ReferenceSpan.Start.Chapter;
            int nVerse = ReferenceSpan.Start.Verse;

            foreach (DParagraph p in Paragraphs)
            {
                foreach (DRun run in p.Runs)
                {
                    DChapter chapter = run as DChapter;
                    if (null != chapter)
                    {
                        nChapter = chapter.ChapterNo;
                        continue;
                    }

                    DVerse verse = run as DVerse;
                    if (null != verse)
                    {
                        nVerse = verse.VerseNo;
                        continue;
                    }

                    DText text = run as DText;
                    if (null != text)
                    {
                        foreach (DNote n in text.Notes)
                        {
                            if (n == note)
                            {
                                // Some paragraphs (e.g., Section Title) just return the 
                                // stylename rather than the reference
                                if (ParagraphHasNoReference(p))
                                {
                                    string sNonVersePara = G.GetLoc_StyleName(p.Style.DisplayName);
                                    if (0 != sNonVersePara.Length)
                                        return sNonVersePara;
                                }

                                // Compute and return the reference
                                return nVerse.ToString();
                            }
                        }
                    }
                }
            }

            // Should not get here
            Debug.Assert(false);
            return "";
        }
        #endregion
        #region Method: int CountParagraphsWithStyle(string sStyleAbbrev)
        public int CountParagraphsWithStyle(string sStyleAbbrev)
        {
            int c = 0;

            foreach (DParagraph p in Paragraphs)
            {
                if (p.StyleAbbrev == sStyleAbbrev)
                    ++c;
            }

            return c;
        }
        #endregion
        #region Method: void UpdateFootnoteLetters()
        public void UpdateFootnoteLetters()
        {
            char ch = 'a';
            int i = 0;

            foreach (DParagraph p in Paragraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    // Retrieve the run; skip if not of the right type
                    DFootLetter letter = r as DFootLetter;
                    DSeeAlso also = r as DSeeAlso;
                    if (null == letter && null == also)
                        continue;

                    DFootnote footnote = null;

                    if (null != letter)
                    {
                        letter.Letter = ch;
                        footnote = letter.Footnote;
                    }

                    if (null != also)
                    {
                        also.Letter = ch;
                        footnote = also.Footnote;
                    }

                    // Make sure the footnotes are in the correct order
                    int iCurrentFootnotePos = Footnotes.FindObj(footnote);
                    Footnotes.MoveTo(iCurrentFootnotePos, i);
                    i++;

                    // Update the letter for the next one
                    if (ch == 'z')
                        ch = 'a';
                    else
                        ch++;
                }
            }
        }
        #endregion

        // Method: CopyBackTranslationsFromFront ---------------------------------------------
		#region Method: bool _CopyBT_DoFootnotesMatch(Section SFront)
		private bool _CopyBT_DoFootnotesMatch(DSection SFront)
		{
			if (SFront.Footnotes.Count != Footnotes.Count)
				return false;

			for(int i=0; i < Footnotes.Count; i++)
			{
				DParagraph PFront  = SFront.Footnotes[i] as DParagraph;
				DParagraph PTarget = this.Footnotes[i] as DParagraph;
				if (PFront.StyleAbbrev != PTarget.StyleAbbrev)
					return false;
			}

			return true;
		}
		#endregion
		#region Method: DParagraph[] _CopyBT_GetParagraphsFromPart(DSection, int nPart)
		DParagraph[] _CopyBT_GetParagraphsFromPart(DSection section, int nPart)
		{
			int iPara = 0;

			// Move past the preceeding parts of the section
			string sSectionStructure = section.SectionStructure;
			for(int n=0; n<nPart; n++)
			{
				char chPartType = sSectionStructure[n];

				while (iPara < section.Paragraphs.Count)
				{
					DParagraph p = section.Paragraphs[iPara] as DParagraph;

                    // Get the type (either a "Text" or a "Picture")
                    char ch = c_Text;
                    if (p as DPicture != null)
                        ch = c_Pict;

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
                char ch = c_Text;
                if (p as DPicture != null)
                    ch = c_Pict;

				if (ch != chDesiredPartType)
					break;
				a.Add(p);
				++iPara;
			} 

			// Convert to an array
			DParagraph[] v = new DParagraph[ a.Count ];
			for(int k=0; k<a.Count; k++)
				v[k] = a[k] as DParagraph;
			return v;
		}
		#endregion
		#region Method: void CopyBackTranslationsFromFront(SFront)
		public bool CopyBackTranslationsFromFront(DSection SFront)
		{
			bool bCompletelySuccessful = true;

			// Get the section's structure; (they should be identical)
			string sSectionStructure = SectionStructure;
			Debug.Assert(sSectionStructure == SFront.SectionStructure);
			int cParts = sSectionStructure.Length;

			// Go through the paragraphs, dividing into chunks based on the SectionStructure.
			// (This strategy allows some items to be copied when paragraphs do not match,
			// as some chunks may match while others might not.
			for(int n=0; n<cParts; n++)
			{
				// Get the paragraphs in this chunk
				DParagraph[] vpFront  = _CopyBT_GetParagraphsFromPart(SFront, n);
				DParagraph[] vpTarget = _CopyBT_GetParagraphsFromPart(this, n);

				// We need to have the same count of paragraphs, and the same styles
				// in order to copy this chunk
				if (vpFront.Length != vpTarget.Length)
				{
					bCompletelySuccessful = false;
					continue;
				}
				bool bChunkIsIdentical = true;
				for(int i=0; i < vpFront.Length; i++)
				{
					if (vpFront[i].StyleAbbrev != vpTarget[i].StyleAbbrev)
					{
						bChunkIsIdentical = false;
						continue;
					}
				}
				if (bChunkIsIdentical)
				{
					for(int i=0; i < vpFront.Length; i++)
					{
						if (Map.StyleCrossRef != vpFront[i].StyleAbbrev)
							vpTarget[i].CopyBackTranslationsFromFront(vpFront[i]);
					}
				}
				else
					bCompletelySuccessful = false;
			}

			// Footnotes: copy provided we have the same count and styles
			if (_CopyBT_DoFootnotesMatch(SFront))
			{
				for(int i=0; i < Footnotes.Count; i++)
				{
					DFootnote FFront  = SFront.Footnotes[i] as DFootnote;
					DFootnote FTarget = this.Footnotes[i] as DFootnote;

					if (FFront.NoteType == DFootnote.Types.kSeeAlso)
						continue;

					FTarget.CopyBackTranslationsFromFront(FFront);
                    if (DialogCopyBTConflict.Actions.kCancel == DialogCopyBTConflict.CopyBTAction)
                        return false;
				}
			}
			else
				bCompletelySuccessful = false;

			return bCompletelySuccessful;
		}
		#endregion
	}

    // TODO: MOVE ALL OF THE Test_DSection TestIO TESTS DOWN HERE TO NUnit
	#region TEST
	public class Test_DSection : Test
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attrs (TeamSettings, Project1, Translation1, Book1)
		DTeamSettings TeamSettings = null;
		DProject Project1 = null;
		DProject Project2 = null;
		DTranslation Translation1 = null;
		DTranslation Translation2 = null;
		DBook Book1 = null;
		DBook Book2 = null;
		#endregion
		#region Attr{g}: DSection Section1
		DSection Section1
		{
			get
			{
				return Book1.Sections[0] as DSection;
			}
		}
		#endregion
		#region Attr{g}: DSection Section2
		DSection Section2
		{
			get
			{
				return Book2.Sections[0] as DSection;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public Test_DSection()
			: base("DSection")
		{
			AddTest( new IndividualTest( TestIO_1 ),   "IO Section #1" );
			AddTest( new IndividualTest( TestIO_2 ),   "IO Section #2" );
			AddTest( new IndividualTest( TestIO_3 ),   "IO Section #3" );
			AddTest( new IndividualTest( TestIO_7 ),   "IO Section #7" );
			AddTest( new IndividualTest( TestIO_8 ),   "IO Section #8" );
			AddTest( new IndividualTest( TestIO_9 ),   "IO Section #9" );
			AddTest( new IndividualTest( TestIO_10 ),  "IO Section #10" );
            AddTest( new IndividualTest( TestIO_11 ),  "IO Section #11 (\\ms, \\mr)");

			AddTest( new IndividualTest( CreateTargetSection ), "CreateTargetSection" );
		}
		#endregion
		#region Method: override void Setup()
		public override void Setup()
		{
			// Team Settings (uses program defaults)
			TeamSettings = new DTeamSettings();
			TeamSettings.EnsureInitialized();

			// Initialize Project1
			Project1 = new DProject();
			Project1.DisplayName = "Test Project 1";
			Translation1 = new DTranslation("Test Translation 1", "Latin", "Latin");
			Project1.TargetTranslation = Translation1;
			Book1 = new DBook("MRK", "");
			Translation1.AddBook(Book1);

			// Initialize Project2
			Project2 = new DProject();
			Project2.DisplayName = "Test Project 2";
			Translation2 = new DTranslation("Test Translation 2", "Latin", "Latin");
			Project2.TargetTranslation = Translation2;
			Book2 = new DBook("MRK", "");
			Translation2.AddBook(Book2);

		}
		#endregion
		#region Method: override void TearDown()
		public override void TearDown()
		{
			Book1 = null;
			Book2 = null;
			Translation1 = null;
			Translation2 = null;
			Project1 = null;
			Project2 = null;
			TeamSettings = null;
		}
		#endregion

		// Test Sections Data & Worker Methods -----------------------------------------------
		#region TestData - Header section for a book
		static public string[] s_vsBookRecord = new string[]
		{
			"\\_sh v3.0 2 SHW-Scripture", 
			"\\_DateStampHasFourDigitYear",
			"\\rcrd MRK",
			"\\h Mark",
			"\\st The Gospel Of",
			"\\mt Mark"
		};
		#endregion
		#region TestData #1 - Baikeno Mark 1:1
		static public string[] m_SectionTest1 = new string[] 
		{
			"\\rcrd MRK 01.01-01.03",
			"\\c 1",
			"\\p",
			"\\v 1",
			"\\vt Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin. In " +
			"kana, Jesus Kristus, es Uis-neno nleek nani na'ko un-unu'. " +
			"In lasi nane, nahuun nak on ii:",
			"\\btvt This is a good story/matter. This is *God's Son's|fn life. " +
			"His name is Jesus Kristus, who God designated beforehand from " +
			"long ago. His story/issue begins like this:",
			"\\ft 1:1: Lasi <Uis-neno In Anmone> ka nmui' fa matu'i mane'o bian.",
			"\\btft 1:1: The words <God's Son> is not there in some of the old writings.",
			"\\nt tonas = cerita, berita, riwayat ; una = pohon, pertama, " + 
			"sumber, awal base ; nleek = tunju ; Abalbalat = yg kekal ; " + 
			"mane'o = betul, asli ; amna' = tua ; in una = its beginning " +
			"## na' mo'on = life story, riwayat (not known by all) ; " +
			"ma'mo'en = perbuatan",
			"\\s Nai' Joao Aslain Atoni, naleko' lalan neu Usif Jesus",
			"\\bts Sir Joao the *Baptiser of People, fix/prepares the way/path for " +
			"the *Lord Jesus",
			"\\r (Mateus 3:1-12; Lukas 3:1-18; Joao 1:19-28)",
			"\\p",
			"\\v 2",
			"\\vt Jesus fe' ka nanaob In mepu, mes Uis-neno nsonu' nahuun In " +
			"atoni mese', in kanan nai' Joao. Nai' Joao musti nao naleko' " +
			"lalan neu Jesus In amneman. Fun natuin na'ko un-unu', " +
			"Uis-neno anpaek nalail In mafeef' es. Mafefa' nane, in kanan " +
			"Na'i Yesaya. In ntui nani, nak on ii:",
			"\\btvt Jesus had not yet begun His work, but God sent beforehand one " +
			"of His men, whose name as sir Joao. Sir Joao must fix/prepare the " +
			"path/way for Jesus' coming. Because from long ago, God had used one " +
			"of His mouth (=spokesperson). That spokesperson was named " +
			"Grandfather/ancestor Yesaya. He had written like this:",
			"\\nt nasoitan = buka ; nseef = buka, (tali) ; naloitan = perbaiki ; " +
			"nani = memang, fore-",
			"\\q",
			"\\vt <<Mneen nai, he! Au 'leul Au 'haef ma 'nimaf, henati nao naleko' " +
			"lalan neu Ko",
			"\\btvt <<Listen up, he! I send My foot and hand (=trusty servant) to go " +
			"fix/prepare the way/path for You.",
			"\\cf 1:2: Maleakhi 3:1",
			"\\q",
			"\\v 3",
			"\\vt Le atoni nane lof in anao mbi bael sona' es, he in nkoa', mnak:",
			"\\btvt That man will go to an uninhabited place, to shout.words, saying:"
		};
		#endregion
		#region TestData #2 - Baikeno Mark 1:9
		static public string[] m_SectionTest2 = new string[] 
		{
			"\\rcrd MRK 01.09-01.11",
			"\\s Nai' Joao naslain Usif Jesus",
			"\\bts Sirr Joao baptises the Lord Jesus",
			"\\r (Mateus 3:13-17; Lukas 3:21-22)",
			"\\p",
			"\\v 9",
			"\\vt Mbi neno nane, Jesus neem na'ko kuan Najaret, mbi profinsia Galilea, " +
			"ma he na'euk nok nai' Joao. Ma nai' Joao naslani Ee mbi noel Jordan.",
			"\\btvt At that time, Jesus came from the village of Najaret, in the profinsia " +
			"of Galilea, and he met with sir Joao. And sir Joao *baptised Him in the " +
			"Jordan river.",
			"\\nt na'eku < na'euk ; /j/ is halfway between [z] and [j]",
			"\\v 10",
			"\\vt Olas Jesus mpoi na'ko oel, suk naskeek, napeen niit neno-tunan natfei'. " +
			"Nalali te, Uis-neno In Smana Knino' nsaon neem neu Ne, namnees onle' kol-pani.",
			"\\btvt When Jesus came out from the water, suddenly, was seen the heaven/sky " +
			"opened (=no Actor). Then God's *Spirit descended coming to Him, like a dove.",
			"\\nt natfei = opened, tabela  ; natfaika = like a ship parting the waters ; " +
			"habu = sky, clouds ; nipu = clouds ; Asmanan ; kolo = burung ; kol-pani = " +
			"yg biasa orang piara, putih, abu-abu, coklat muda",
			"\\v 11",
			"\\vt Ma on nane te, neen Uis-neno In hanan na'ko neno-tunan, nak,",
			"\\btvt And then was heard (=no Actor) God's voice from the sky, saying,",
			"\\q",
			"\\vt <<Ho le' ii, Au An-Honi'.",
			"\\btvt <<You here, are My 1) very own Child, 2) beloved Child [ambiguous].",
			"\\nt An-Honi",
			"\\q2",
			"\\vt Ho es meki mhaliin Kau, Au nekak.>>",
			"\\btvt You are the one who pleases my liver.>>",
			"\\cf 1:11: *Kejadian 22:2, Mazmur 2:7, Yesaya 42:1, Mateus 3:17, 12:18, Markus " +
			"9:7, Lukas 3:22*",
			"\\nt neno-tunan ~ pah-pinan ; meki ; mhaliin = senang",
			"\\cat c:\\graphics\\cook\\cnt\\cn01656b.tif",
			"\\ref width:9.0cm",
			"\\cap Joao naslain nalail Usif Jesus",
			"\\btcap Joao has finished baptising the Lord Yesus",
			"\\p"
		};
		#endregion
		#region TestData #3 - Baikeno Mark 16:19
		static public string[] m_SectionTest3 = new string[] 
		{
			"\\rcrd MRK 1",
			"\\s Usif Jesus ansae on neno-tunan",
			"\\bts The Lord Jesus ascends to heaven",
			"\\r (Lukas 24:50-53; Haefin 1:9-11)",
			"\\p",
			"\\v 19",
			"\\vt Namolok nalail nok In atopu'-noina' sin, Uis-neno na'aiti' " +
			"nasaeb Usif Jesus on sonaf neno-tunan. Mbi nane, In ntook mbi " +
			"Uis-neno In banapan a'ne'u, ma sin nhuuk plenat nabuan.",
			"\\btvt After having spoken with His disciples, God took up the " +
			"Lord Jesus to the palace/kingdom in heaven. There, He sits at " +
			"God's right side, and they hold rule together.",
			"\\cf 16:19: *Kisah Para Rasul 1:9-11*",
			"\\p",
			"\\v 20",
			"\\vt Nalali te, In atopu'-noina' sin nanaoba In aplenat. Sin naon " +
			"neu pah-pah, ma natonan Usif Jesus In Lais Alekot neu atoni " +
			"ok-oke'. Ma Uis-neno nfee sin kuasat, henati sin anmo'en lasi " +
			"mkakat ok-oke' le' Usif Jesus natoon nalail neu sin. Ma nalail, " +
			"nmui' atoni namfau le' npalsai neu Usif Jesus, fun sin nahinen " +
			"nak, Lais Alekot nane, namneo.",
			"\\btvt After that, His disciples carried out His commands. They " +
			"went to various lands and told the Lord Jesus' Good News to all " +
			"people. And God gave them power so that they did all the miracles " +
			"that the Lord Jesus had foretold to them. And then, there were " +
			"many people who believed in the Lord Jesus, cause they knew that " +
			"the Good News was true.",
			"\\p",
			"\\p",
			"\\s NAI' MARKUS NAHEUB IN MOLOK, NATUIN LULAT UN-UNU' BIAN",
			"\\bts Sir Markus ends his story, according to other old writings",
			"\\ft 16:9-10: Tuis uab Yunani le' ahun-hunut ma le' naleko neis, " +
			"na'tu'bon es ela' 8. Nai' Markus in Tonas namsoup be neik lasi " +
			"nono' nua. Es amnanu (Markus 16:9-20), ma esa na'paal (Markus " +
			"16:9-10). Natuin atoin ahinet sin, le' nahiin mane'o-mane'o Nai' " +
			"Markus in Tonas ii, sin nak lasi nono' nua in ii, le' sin nluul " +
			"namunib. Lasi nono' nua in nane, naleta' neu Usif Jesus nmoni " +
			"nfain na'ko In a'maten, ma mepu plenat neu atoni le' anpalsai " +
			"neu Jesus.",
			"\\btft 16:9-10: The oldest writings in the Yunani language that " +
			"are better, finish at verse 8. Sir Markus' Story ends with two " +
			"story versions. One is long (Markus 16:9-20), and one is short " +
			"(Markus 16:9-10). According to knowledgable people, who really " +
			"understand this Story of Sir Markus, they say that both of " +
			"these versions were written later. Both of those versions tell " +
			"about the Lord Jesus living again from His death, and the work " +
			"orders to people who believe in Jesus.",
			"\\p",
			"\\v 9",
			"\\vt Olas bifeel teun in nane, naon ntenuk sin Pedru, sin natonan " +
			"ok-oke' le' alelo sin niit li'an munif nane, le' namolok nok " +
			"sin mbi bol fatu.",
			"\\btvt When those three women went arriving at Pedru, they told " +
			"everything that they had just seen of that young man, which he " +
			"had told them at the rock hole.",
			"\\v 10",
			"\\vt Oke te, Usif Jesus naplenat kun In atopu'-noina' sin, he " +
			"naon natonan In Lais Alekot ii neu pah-pah ok-oke', tal antee " +
			"pah-pinan fun am nateef. Lais Alekot ije nalekan lalan henati " +
			"Uis-neno nsaok atoni amfau tin na'ko sin sanat ma penu, ma he " +
			"nmoin piut nok Uis-neno.",
			"\\btvt Then the Lord Jesus himself commanded His disciples to go " +
			"tell this Good News of His in all lands/countries, until the " +
			"far corners of the earth. This Good News shows the way so that " +
			"God can wipe away the sins1 and wrongs1 of many people, and so " +
			"they can live continually with God.",
			"\\p",
			"\\vt Lais Alekot ije, namneo on naan. Es nane te, Lais Alekot ii " +
			"nhaek piut, tal antee nabal-baal. Amen.",
			"\\btvt This Good News is really true. That is why this Good News " +
			"continues to stand, forever. Amen.",
			"\\nt apenut = orang pakane'o ; nhaek = berdiri, tegak, teguh",
			"\\p",
			"\\cat c:\\graphics\\maps\\bible\\palestinTP.jpg" ,
			"\\ref width:10.5cm" ,
			"\\e"	
		};
		#endregion
		#region TestData #4 - Baikeno Mark 4:30-34
		static public string[] m_SectionTest4 = new string[] 
		{
			"\\rcrd MRK 1",
			"\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
			"\\bts The Lord Jesus give an example of a seed that is extremely tiny",
			"\\nt sain = jenis biji yg kici ana",
			"\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
			"\\p",
			"\\v 30",
			"\\vt Oke te, Jesus namolok antein, mnak, <<Au uleta' 'tein on ii: Hi " +
			"nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes " +
			"nabaab-took, tal antee sin namfau nok.",
			"\\btvt Then Jesus spoke again, saying, <<I give another example like " +
			"this: You(pl) can compare God's *social group. From just a few people, " +
			"it nevertheless increases (in number), to the point that they are very " +
			"many.",
			"\\nt bonak = Setaria italica, Rote = botok ; an-ana' = very small ; " +
			"fini = seed for planting ; minoon = banding, ; kle'o = few ; nabaab-took " +
			"= tamba banyak",
			"\\v 31",
			"\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			"\\btvt That is like a seed that is very tiny.",
			"\\v 32",
			"\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof " +
			"kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
			"\\btvt If we(inc) plant it (with dibble stick) it will grow to become " +
			"a large tree. And birds will come looking for shade, and make nests " +
			"in it.>>",
			"\\nt tseen ~ [ceen] = plant with dibble stick ; hau tlaef = ranting " +
			"pohon ; mafo' = sombar ; kuna' = sarang",
			"\\p",
			"\\v 33",
			"\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			"\\btvt Jesus' way of teaching was according to their understanding.",
			"\\v 34",
			"\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' " +
			"sin, In natoon lais leta' nane in na' naon ok-oke'.",
			"\\btvt He taught people using only examples. But with His *disciples, He " +
			"told the meaning of all the examples.",
			"\\ud 17/Jun/2003"		
		};
		#endregion
		#region TestData #5 - Pura Mark 14:03-14:09
		static public string[] m_SectionTest5 = new string[] 
			// Verses redone, start at 1, not 3. Things in this data:
			// - Verse 7 has a unicode character.
			// - Verse 1 has ||'s, including 
			//     + ||fn - which should not be interpreted as a footnote
			//     + ||i - which should not be interpreted as +Italic.
			//     + ||r - which should not be interpted as -Italic
			// - Verse 2 has {{'s and }}'s. Occurring in a doublet, these should
			//     be presented to the user as literal singlets.
		{
			"\\rcrd ARK 1",
			"\\c 1",
			"\\s Ne he jangu ba mina menema e vili ele boal, ma Tuhang Yesus enang",
			"\\r (Matius 26:6-13; Yohanis 12:1-8)",
			"\\p",
			"\\v 1",
			"\\vt Abang Betania mi, ne nu ue ene Simon. Turang mi, ne ava aing veng " + 
			"ororing, tagal dirang hapeburang aing veng. Ba sakarang, ana aung ila.",
			"\\btvt",
			"\\p",
			"\\vt Seng angu mu, o||ras angu ve||d-ved aung hoa jedung, Yesus ini ue " +
			"ila Simon e hava mi nana. ||Oras ||ini nana, nehe jangu nu Yesus evele " +
			"hoa Aing dapa. Ana ue botol nu pina ba ini var ma ening. ||Mina " +
			"nemema asli ba mi evili talalu ele.|fn Seng, nebe jangu angu botol " +
			"ememng angu||fn ma vil bue ening kivita. Mu ana boal ening to tu tahang-tahang " +
			"mina angu ma Yesus ong tang||, e jadi tanda Yesus Aing ta janing",
			"\\btvt",
			"\\ft",
			"\\p",
			"\\v 2",
			"\\vt Ba ne }}ebeung iva di {{ue umurung}} nana. Oras ini eteing nehe jangu " +
			"angu ening ula{{{{ng angu, mi ini ili il, e ini i ta tu}}tuk sombong " +
			"hula, <<Hmm! Ne he {{jangu na ba {{ba anga, e ana ila mina}} menema " +
			"e vili ele angu viat parsuma.",
			"\\btvt",
			"\\v 3",
			"\\vt Lebe aung mina angu ana avali ba! E ila e hoang toang angu paul, " +
			"ma ne malarat anaung ing enang! Se mina menema angu e vili veng, " +
			"ma nenu e gaji tung nu veng hama.>>",
			"\\btvt ",
			"\\p ",
			"\\v 4",
			"\\vt Ba Yesus balas hula, <<Ini ake nehe jangu anga ening susa! Mang " +
			"aing kilang ba! Na sanang, tagal mina menema anga ana ma Noboa " +
			"veng obokong ila. ",
			"\\btvt  ",
			"\\v 5",
			"\\vt Ne kasiang anaung salalu ae ing veng. Jadi, ini bisa taveding " +
			"di ini ing tulung. Ba kalo Naing, lung niang ila, se Na ing veng " +
			"hama-hama niang ila. ",
			"\\btvt",
			"\\cf 14:7: Ulangan 15:11",
			"\\v 6",
			"\\vt Ne e vetang lung niang ila. Mang nehe jangu anga vede mina obokong " +
			"anga, ana sidiat ila ma Neboa veng etura, emang hula ana Ne baring " +
			"veng bunga me at ila. ",
			"\\btvt",
			"\\v 7",
			"\\vt Benganit aung-aung, o`! Ta ang mi ba Tuhang Lahatala E Sirinta " +
			"Aung ila alamula anga goleng, indi pasti nehe jangu e aung anga " +
			"veng sirinta! E biar ne emampi aing vengani.>>",
			"\\btvt"
		};
		#endregion
		#region TestData #6 - Helong Acts 4:01-4:04
		static public string[] m_SectionTest6 = new string[] 
		{
			"\\rcrd act4.1-4.22",
			"\\c 4",
			"\\s Tulu-tulu agama las haman Petrus nol Yohanis maas tala",
			"\\bts The heads of religion summon Petrus and Yohanis to come appear.before [them]",
			"\\p",
			"\\v 1",
			"\\vt Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, " +
				"atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				"tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				"agama Saduki. Oen maas komali le ahan Petrus nol Yohanis.",
			"\\btvt At that time, Petrus and Yohanis still were talking with those people, " +
				"several big/important people came. Those them(=They in focus), " +
				"[were] heads of the Yahudi religion, and the head of guarding the " +
				"*Temple, and people from the religious party Saduki. They came " +
				"angry to scream at Petrus and Yohanis.",
			"\\nt doh =doha;  tala=mangada",
			"\\v 2",
			"\\vt Oen komali lole Petrus nol Yohanis na mo, kom isi le tek " +
				"atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				"mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				"haup in nuli pait kon.>>|fn",
			"\\btvt They were angry because that Petrus and Yohanis, like to tell all " +
				"people, saying, \"Yesus has lived again from His death! With that, He " +
				"opened the path for dead people so that they also could live again.\"|fn " +
				"(check two kon)",
			"\\ft 4:1-2: Atuil deng partai agama Saduki, oen sium in tui na lo " +
				"man noen atuil mate haup in nuli pait.",
			"\\btft 4:1-2: People from the religious party Saduki, they did not accept that " +
				"teaching that says dead people can live again.",
			"\\v 3",
			"\\vt Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
				"deng lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
				"Le ola ka halas-sam, oen nehan dais na.",
			"\\btvt And so they ordered their people to go capture the two of them. But " +
				"because the sun already wanted to set, then they pushed [&] entered " +
				"the two of them in jail. So.that the next day, they could take.care.of " +
				"that affair/litigation/problem.",
			"\\nt nehan = urus, mengatur",
			"\\v 4",
			"\\vt Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
				"atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
				"tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			"\\btvt But from the people who liked to hear from those apostles, therefore " +
				"many people had already acknowledged that that which they taught, " +
				"[was] spot.on correct. That's.why their people increased a lot  " +
				"approximately to five thousand people.",
			"\\cat c:\\graphics\\cook\\cnt\\cn01901b.tif",
			"\\ref width:7.0cm;textWrapping:around;horizontalPosition:right",
			"\\cap Atuil-atuil tene kas klaa Petrus nol tapang ngas",
			"\\btcap The leaders accuse/criticise Petrus and his friends"
		};
		#endregion
		#region TestData #7 - Helong Acts 7:54-8:01a
		static public string[] m_SectionTest7 = new string[] 
		{
			"\\rcrd ACT 07.54-08.01",
			"\\s Oen pasang tele Stefanus",
			"\\bts They throw killing Stefanus",
			"\\p",
			"\\v 54",
			"\\vt Atuil man dad le nehan dasi la, ming Stefanus in dehet ta, oen tan meman " +
				"noan un soleng bel oen kula ka. Hidim oen dalen ili le duu-duu siin nol " +
				"Stefanus.",
			"\\btvt The person (pl?) who were sitting to take.care of the litigation heard " +
				"that speaking of Stefanus', they knew that he was throwing_out giving their " +
				"wrongs. So they were very angry [lit. sick insides] to ate/ground their " +
				"teeth for Stefanus.",
			"\\nt duu-duu = makan gigi grind teeth ; agak barhenti hujan lebat =   ulan na " +
				"siin son (hujan sudah mau berhenti sedikit) duu = kunya   in mouth ; duta " +
				"= grind/ulik",
			"\\p",
			"\\v 55",
			"\\vt Mo Stefanus man hapu Ama Lamtua Ko Niu' ka, botas ngat laok el apan nua. " +
				"Se la, un ngat net Ama Lamtua Allah dui to-toang nol Ama Lamtua Yesus dil " +
				"se Ama Lamtua Allah halin kanan na, se man in todan dui ka.",
			"\\btvt But Stefanus who had obtained that Holy Spirit of the Lord's, lifted " +
				"his eyes to look at the sky. There, he saw all the Lord God's greaterness, " +
				"and the Lord Yesus standing at the Lord God's right side in that place " +
				"which is more honorable.",
			"\\v 56",
			"\\vt Kon Stefanus dehet noan, <<Elia! Ama-amas to-toang. Auk ngat net apan nu " +
				"hol sai, nol An Atuli la dil ne man in todan lahing isi ka se Ama Lamtua " +
				"Allah halin kanan na.>>",
			"\\btvt And Stefanus spoke saying, <<Like this!, All fathers. I am seeing the sky " +
				"open, and Humanity's Child standing at that place which is most honorable at " +
				"the Lord God's right side.>>",
			"\\nt botas ngat = angkat kepala to see",
			"\\p",
			"\\v 57",
			"\\vt Ming ela kon, atuil in nehan dasi la ka oen kuim hngilans. Hidim oen kidu " +
				"ahan le tek Stefanus noan boel lobo lo. Kon oen tukin haung pul leo-leo le " +
				"pisu sisin Stefanus.",
			"\\btvt Hearing that then, the people who take care of that litigation shut? " +
				"their ears. Then they yelled screaming to order Stefanus to shut (his) mouth. " +
				"And they all jumped up quickly with.alot.of.excitement in.order.to tear apart " +
				"Stefanus.",
			"\\v 58",
			"\\vt Hidi kon oen pela lakang un puti lako likun deng kota la. Ela kon saksi-" +
				"saksi las oen kolong oen kaod likun nas, le bel tana muda mesa le kilas. Un " +
				"ngala ka Saulus. Hidi kon, oen lakos pasang tele Stefanus nini batu.",
			"\\btvt Then they forced him out going outside the city. Next the witnesses took " +
				"off their outside clothes, to give them to a young person to hold. His name " +
				"Saulus, he was their inciter (lit. fanner). And then, they went to throw " +
				"kill Stefanus using stones.",
			"\\nt tukin haung = bangun tiba-tiba ; tukin = bangun/naik ; pisu =   tear ; pisu " +
				"sisin cabut-cabut (cek) ; kuim = tutup; 58: ratulin   in iha-iha = " +
				"provacator; iha = kipas ; kolong = buka ; pela lakang   = paksa; lakang = " +
				"mendesak/dorong",
			"\\cat c:\\graphics\\cook\\cnt\\cn02154b.tif",
			"\\ref width:11.0cm;verticalPosition:top;horizontalPosition:center",
			"\\cap Oen pasang tele Stefanus",
			"\\btcap They throw killing Stefanus",
			"\\p",
			"\\v 59",
			"\\vt Oen pasang Stefanus nabael ela kon, un haman mu-muun le tek noan, <<Ama " +
				"Lamtua Yesus! Auk oras sa da-dani son. Sium auk tia!>>",
			"\\btvt They were throwing at Stefanus, then he yelled with a loud voice saying, " +
				"<<Lord Yesus! My time is very close. Receive Me (imperative)!>>",
			"\\v 60",
			"\\vt Hidi kon un lea holimit hai buku ka, le un ahan pait nol fala mu-muun tek " +
				"noan, <<Ama, beles oen lepa hal kula-sala nia deken!>> Hidi na, un hngasa " +
				"ka nutus, kon mate.",
			"\\btvt Then he fell folding his knees? in.order.to yell again with a loud voice, " +
				"saying, <<Father, don't shoulder.carry this sin!>> And then his breath was " +
				"cut.off/stopped and [he]died.",
			"\\c 8",
			"\\v 1a",
			"\\vt Nol Saulus kon bab se man na, un sium banan nol in pasang tele Stefanus " +
				"son na.",
			"\\btvt And Saulus was also at that place, he accepted well [the fact that] they " +
				"threw murdered that Stefanus.",
			"\\nt holimit = terlipat?",
			"\\ud 24/Jan/2005"
		};
		#endregion
		#region TextData #8 - Helong Rev 1:4-8 (empty shell)
		static public string[] m_SectionTest8 = new string[] 
		{
			"\\rcrd REV 01.04-01.08",
			"\\s ",
			"\\p ",
			"\\v 4",
			"\\vt ",
			"\\p ",
			"\\vt |fn ",
			"\\btvt |fn ",
			"\\ft ",
			"\\cf 1:4: La'o sai hosi Mesir 3:14, Rai-klaran Foun 4:5",
			"\\v 5",
			"\\p ",
			"\\cf 1:5: Yesaya 55:4, Kananuk sia 89:27",
			"\\v 6",
			"\\cf 1:6: La'o sai hosi Mesir 19:6, Rai-klaran Foun 5:10",
			"\\p ",
			"\\v 7",
			"\\cf 1:7: Daniel 7:13, Santo Mateus 24:30, Santo Markus 13:26, Santo Lukas 21:27, 1 Tesalonika 4:17, Sakarias 12:10, Santo Yohanis 19:34, 37",
			"\\p ",
			"\\v 8",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\vt ",
			"\\cf 1:8: Rai-klaran Foun 22:13, La'o sai hosi Mesir 3:14"
		};
		#endregion
		#region TextData #9 - Manado Mark - two footnotes
		static public string[] m_SectionTest9 = new string[] 
		{
			"\\rcrd mrk13.14-23",
			"\\s Yesus kase tau samua tu mo jadi di hari-hari siksa",
			"\\r (Matius 24:15-28; Lukas 21:20-24)",
			"\\p ",
			"\\v 14",
			"\\vt Yesus bilang, \"Satu orang yang paling jaha|fn mo datang. ",
			"\\ft 13:14a: Tu orang ini Allah pe musu [lia Daniel 9:27; 11:31; 12:11].",
			"\\vt Orang ini mo badiri di tampa yang nyanda cocok for dia.|fn (Sapa " +
			"yang baca ini, taru kira akang bae-bae). Kong kalu ngoni lia tu orang " +
			"itu so badiri di tampa itu, lebe bae orang-orang yang ada di Yudea " +
			"manyingkir jo ka gunung.",
			"\\ft 13:14b Tu tampa itu Ruma Ibada Pusat [lia Matius 24:15].",
			"\\nt Usulan dari pak Michael: Yesus bilang, \"Satu orang yang paling " +
			"jaha mo datang. ",
			"\\cf 13:14: *Daniel 9:27, 11:31, 12:11*",
			"\\p ",
			"\\v 15",
			"\\vt Tu orang yang ada di atas ruma, jang pi ambe tu barang di dalam " +
			"ruma, mar turung kong lari jo."
		};
		#endregion
		#region TextData #10 - Tombulu Acts - empty section
		static public string[] m_SectionTest10 = new string[] 
		{
			"\\rcrd act 09.32-35",
			"\\s Si Enéas é liné'os ni Petrus",
			"\\p ",
			"\\v 32",
			"\\vt",
			"\\v 33",
			"\\vt",
			"\\p ",
			"\\v 34",
			"\\vt",
			"\\v 35",
			"\\vt",
			"\\ud 12/Apr/2006"
		};
		#endregion
        #region TextData #11 - \mr, \ms
        static public string[] m_SectionTest11 = new string[] 
        {
            "\\rcrd JER 001",
            "\\ms Judah in Trouble",
            "\\mr (Psalm 123:5)",
            "\\s Jeremiah's Prayer",
            "\\p",
            "\\v 1",
            "\\vt I know, Lord, that our lives are not our own.",
		    "\\p",
		    "\\v 2",
		    "\\vt The Lord gave another message to Jeremiah."
       };
        #endregion

        #region Method: void CompareSections() - compares Section with Section2
        static public void CompareSections(Test t, DSection s1, DSection s2)
		{
			t.AreSame( s1.Title, s2.Title);
			t.AreSame( s1.Footnotes.Count, s2.Footnotes.Count);
			t.AreSame( s1.Paragraphs.Count, s2.Paragraphs.Count);
			t.IsTrue( s1.ReferenceSpan.End.Chapter != 0);

			t.WriteLine("Compare Sections: Paragraphs Count = " + s1.Paragraphs.Count.ToString());
			for(int i=0; i < s1.Paragraphs.Count; i++)
			{
				DParagraph pg1 = (DParagraph)s1.Paragraphs[i];
				DParagraph pg2 = (DParagraph)s2.Paragraphs[i];

				t.IsTrue( pg1.ContentEquals(pg2));

				if (pg1.GetType() == typeof(DPicture))
				{
					t.IsTrue(pg2.GetType() == typeof(DPicture));
				}
				if (pg2.GetType() == typeof(DPicture))
				{
					t.IsTrue(pg1.GetType() == typeof(DPicture));
					DPicture pict1 = pg1 as DPicture;
					DPicture pict2 = pg2 as DPicture;
					t.IsNotNull(pict1);
					t.IsNotNull(pict2);
					t.AreSame( pict1.PathName,    pict2.PathName);
					t.AreSame( pict1.WordRtfInfo, pict2.WordRtfInfo);
					t.AreSame( pict1.DebugString, pict2.DebugString);
				}
				if (G.TeamSettings.SFMapping.IsVernacularParagraph(pg1.StyleAbbrev))
				{
					t.IsTrue(pg1.ChapterI > 0);
					t.IsTrue(pg1.VerseI > 0);
					t.IsTrue(pg1.ChapterF > 0);
					t.IsTrue(pg1.VerseF > 0);
					t.IsTrue(pg1.ChapterI == pg2.ChapterI);
					t.IsTrue(pg1.VerseI   == pg2.VerseI);
					t.IsTrue(pg1.ChapterF == pg2.ChapterF);
					t.IsTrue(pg1.VerseF   == pg2.VerseF);
				}
				t.AreSame( pg1.AllNotes.Length, pg2.AllNotes.Length );
				for(int iN=0; iN < pg1.AllNotes.Length; iN++)
				{
					DNote n1 = pg1.AllNotes[iN] as DNote;
					DNote n2 = pg2.AllNotes[iN] as DNote;
					t.IsTrue( n1.NoteText.ContentEquals( n2.NoteText ) );
				}
			}

			for(int i=0; i < s1.Footnotes.Count; i++)
			{
				DFootnote f1 = (DFootnote)s1.Footnotes[i];
				DFootnote f2 = (DFootnote)s2.Footnotes[i];
				t.AreSame( f1.DebugString,   f2.DebugString);
			}
		}
		#endregion
		#region Method: void CheckPictureCount(DSection, int cExpected)
		void CheckPictureCount(DSection section, int cExpected)
		{
			int cPictures = 0;
			foreach(DParagraph p in section.Paragraphs)
			{
				if (p.GetType() == typeof(DPicture))
				{
					++cPictures;
				}
			}
			this.AreSame(cExpected, cPictures);
		}
		#endregion
		#region Method: DPicture GetPicture(DSection sect, int i)
		DPicture GetPicture(DSection sect, int i)
		{
			int cPictures = 0;
			foreach(DParagraph p in sect.Paragraphs)
			{
				if (p.GetType() == typeof(DPicture))
				{
					if (cPictures == i)
						return p as DPicture;
					++cPictures;
				}
			}
			IsTrue(false); // picture not found
			return null;
		}
		#endregion

		#region Method: ScriptureDB InitializeBook(DBook book, string[] vsSection)
		static public ScriptureDB InitializeBook(DBook book, string[] vsSection)
		{
			// The book needs a pathname, so we can write the strings out and then
			// load them back in.
            book.AbsolutePathName = GetPathName("InitBook");

			// Create one array of strings that has the book record and the section record
			int nCount = s_vsBookRecord.Length + vsSection.Length;
			string[] vs = new string[nCount];
			int i = 0;
			foreach(string s in s_vsBookRecord)
				vs[i++] = s;
			foreach(string s in vsSection)
				vs[i++] = s;

			// Create the DB object and write it out to file
			ScriptureDB DB = new ScriptureDB();
            DB.Format = ScriptureDB.Formats.kToolbox;
			DB.Initialize(vs);
            DB.TransformIn();
            DB.Write(book.AbsolutePathName);

			// Read it into the book
            book.Load();

			return DB;
		}
		#endregion
		#region TestIO_Engine
		void TestIO_Engine(string[] vsSection, bool bPerformDBTest)
			// The DB test checks to see if the Sf fields written from the original array
			// match up exactly with what is written when the book is saved. We don't do
			// this test everytime, because some of the test data has transforms that we
			// expect to happen, and thus the DB will be different from the string array.
			// The main purpose of the test is to see that, e.g., things like Back Translations
			// are not disappearing.
		{
			//EnableTracing = true;

			ScriptureDB DBInitialData = InitializeBook( Book1, vsSection );

			// Write it out to file (Book1's pathname gets changed, but the folder is the same).
            Book1.Write();

			// Read it into Book2
            Book2.AbsolutePathName = Book1.AbsolutePathName;
            Book2.Load();

			// Are the two sections equal?
			CompareSections(this, Section1, Section2);

			// Cleanup
			File.Delete(PathNameA);
            File.Delete(Book1.AbsolutePathName);
		}
		#endregion
		#region TestIO_1
		public void TestIO_1()
		{
//			EnableTracing = true;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest1, true);

			// Did we input what we expected into Section2?
			AreSame( 1, Section2.ReferenceSpan.Start.Chapter );
			AreSame( 3, Section2.ReferenceSpan.End.Verse );
			DParagraph p = ((DParagraph)Section2.Paragraphs[7]);
			AreSame( p.DebugString,
				"<<Mneen nai, he! Au 'leul Au 'haef ma 'nimaf, henati nao naleko' " +
				"lalan neu Ko{BT <<Listen up, he! I send My foot and hand (=trusty " +
				"servant) to go fix/prepare the way/path for You.}{cf b}");
			p = ((DParagraph)Section2.Paragraphs[8]);
			AreSame( p.DebugString,
				"{v 3}Le atoni nane lof in anao mbi bael sona' es, he in nkoa', " +
				"mnak:{BT That man will go to an uninhabited place, to shout.words, " +
				"saying:}");
			AreSame( p.StyleAbbrev, "q");
		}
		#endregion
		#region TestIO_2
		public void TestIO_2()
		{
//			EnableTracing = true;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest2, false);

			// Did we input what we expected into Section2?
			AreSame( 11, Section2.ReferenceSpan.End.Verse);
			DParagraph p = ((DParagraph)Section2.Paragraphs[7]);
			AreSame( "Ho es meki mhaliin Kau, Au nekak.>>{BT You are the one who " +
				"pleases my liver.>>}{cf a}", p.DebugString);
			AreSame( "q2", p.StyleAbbrev);
			p = ((DParagraph)Section2.Paragraphs[3]);
			AreSame( "Nai' Joao naslain Usif Jesus{BT Sirr Joao baptises the Lord Jesus}", 
				p.DebugString);
			AreSame( "s", p.StyleAbbrev);

			// Picture
			CheckPictureCount(Section2, 1);
			DPicture pict = GetPicture(Section2, 0);
			AreSame( pict.PathName,    "c:\\graphics\\cook\\cnt\\cn01656b.tif");
			AreSame( pict.WordRtfInfo, "width:9.0cm");
			AreSame( "Joao naslain nalail Usif Jesus{BT Joao has finished baptising " +
				"the Lord Yesus}", pict.DebugString);

			// Notes
			int cNotes = 0;
			foreach(DParagraph para in Section2.Paragraphs)
				cNotes += para.AllNotes.Length;
			AreSame( cNotes, 4);
		}
		#endregion
		#region TestIO_3
		public void TestIO_3()
			// Addresses bugs observed in the Baikeno Mark 16:19 passage:
			// 1. Not writing out the picture info where all we had was a pathname
			//      and rtf information Import.
			// 2. Not writing out a footnote (\ft) where we don't have a {fn} in 
			//      the preceeding verse.
		{
//			EnableTracing = true;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest3, false);

			// Was the picture information read in? (It's in the 2nd section)
			DSection sectionA = Book2.Sections[0] as DSection;
			DSection sectionB = Book2.Sections[1] as DSection;
			CheckPictureCount(sectionB, 1);
			DPicture pict = GetPicture(sectionB, 0);
			AreSame( pict.PathName, "c:\\graphics\\maps\\bible\\palestinTP.jpg" );
			AreSame( pict.WordRtfInfo, "width:10.5cm");

			// We should have one footnote read in in both sections
			AreSame( 1, sectionA.Footnotes.Count );
			AreSame( 1, sectionB.Footnotes.Count );
		}
		#endregion
		#region TestIO_7
		public void TestIO_7()
		{
			//EnableTracing = true;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest7, false);

			// The chapter number should be at thte end of the last paragraph
			DParagraph pLast = Section2.Paragraphs[ Section2.Paragraphs.Count - 1] as DParagraph;
			int iChapter = 4;
			IsTrue(pLast.Runs[iChapter] as DChapter != null);
		}
		#endregion
		#region TestIO_8
		public void TestIO_8()
		{
			EnableTracing = false;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest8, false);

			// Check the contents of selected paragraphs (Recall that we add 3 paragraphs
			// to the front, so, e.g., the \s in the data is really paragraph 4 (or [3].
			//foreach(DParagraph p in Section2.Paragraphs)
			//	Console.WriteLine("p = " + p.DebugString);
			DParagraph p3 = Section2.Paragraphs[5] as DParagraph;
			AreSame("{fn a}{cf b}{v 5}", p3.DebugString);
			DParagraph p4 = Section2.Paragraphs[6] as DParagraph;
			AreSame("{cf c}{v 6}{cf d}", p4.DebugString);
		}
		#endregion
		#region TestIO_9
		public void TestIO_9()
		{
			EnableTracing = false;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest9, false);
		}
		#endregion
		#region TestIO_10
		public void TestIO_10()
		{
			EnableTracing = false;

			// Do the IO and section comparision
			TestIO_Engine(m_SectionTest10, false);
		}
		#endregion
        #region TestIO_11
        public void TestIO_11()
        {
            EnableTracing = false;

            // Do the IO and section comparision
            TestIO_Engine(m_SectionTest11, false);
        }
        #endregion

		// Other Tests -----------------------------------------------------------------------
		#region CreateTargetSection()
		public void CreateTargetSection()
			// Tests the routine which creates a blank, ready-for-translating, section
			// based on the front translation.
		{
//			EnableTracing = true;

			// Write out the string array, then read it into Book1's first section
			InitializeBook(Book1, m_SectionTest2);

			// Find which book is Luke, which is Isaiah, etc.
			int iLuke = -1;
			int iIsaiah = -1;
			int iMatthew = -1;
			for(int i=0; i<66; i++)
			{
				if (DBook.BookAbbrevs[i] == "MAT")
					iMatthew = i;
				if (DBook.BookAbbrevs[i] == "LUK")
					iLuke = i;
				if (DBook.BookAbbrevs[i] == "ISA")
					iIsaiah = i;
			}

			// Set up some matching book names from the "front" translation
			Translation1.BookNamesTable[iLuke]    = "Lukas";
			Translation1.BookNamesTable[iIsaiah]  = "Yesaya";
			Translation1.BookNamesTable[iMatthew] = "Mateus";

			// Create a target translation, with some different book names
			DTranslation TTarget = new DTranslation("Target", "Latin", "Latin");
			TTarget.BookNamesTable[iLuke]    = "Luke";
			TTarget.BookNamesTable[iIsaiah]  = "Isaiah";
			TTarget.BookNamesTable[iMatthew] = "Matthew";

			// Create the new empty section
			DSection SNew = new DSection(1);
			DBook    BNew = new DBook();
			TTarget.Books.Append(BNew);
			BNew.Sections.Append(SNew);
			SNew.InitializeFromFrontSection(Section1);

			// Check for the same styles in all the paragraph groups
			AreSame(10, SNew.Paragraphs.Count);
			int iCrossRefPara = -1;
			for(int i=0; i<SNew.Paragraphs.Count; i++)
			{
				string sExp = (Section1.Paragraphs[i] as DParagraph).StyleAbbrev;
				string sAct = (SNew.Paragraphs[i] as DParagraph).StyleAbbrev;
				AreSame(sExp, sAct);
				if (sAct == G.TeamSettings.SFMapping.StyleCrossRef)
					iCrossRefPara = i;
			}

			// Check for \r paragraph to be appropriately converted
			string sCrossRefActual = (SNew.Paragraphs[iCrossRefPara] as 
				DParagraph).DebugString;
			AreSame("(Matthew 3:13-17; Luke 3:21-22)", sCrossRefActual);

			// Check for xref footnote (the first one) to be appropriately converted
			AreSame(1, SNew.Footnotes.Count);
			string sFootnoteAct = (SNew.Footnotes[0] as DFootnote).SimpleText;
			string sFootnoteExp = "1:11: Kejadian 22:2, Mazmur 2:7, Isaiah 42:1, " +
				"Matthew 3:17, 12:18, Markus 9:7, Luke 3:22";
			AreSame(sFootnoteExp, sFootnoteAct);

			// Check for expected paragraph content, e.g., empty, or with verse/chapter/fn no's
			string[] sParas = new string[SNew.Paragraphs.Count];
			for(int i=0; i<SNew.Paragraphs.Count; i++)
				sParas[i] = (SNew.Paragraphs[i] as DParagraph).DebugString;
			AreSame("",                                sParas[0]);
			AreSame("",                                sParas[1]);
			AreSame("",                                sParas[2]);
			AreSame("",                                sParas[3]);
			AreSame("(Matthew 3:13-17; Luke 3:21-22)", sParas[4]);
			AreSame("{v 9}{v 10}{v 11}",               sParas[5]);
			AreSame("",                                sParas[6]);
			AreSame("{cf a}",                          sParas[7]);
			AreSame("",                                sParas[8]);
			AreSame("",                                sParas[9]);

			// Check for picture replication
			CheckPictureCount(Section1, 1);
			DPicture pict = GetPicture(SNew, 0);
			AreSame( pict.PathName, "c:\\graphics\\cook\\cnt\\cn01656b.tif");
			AreSame( pict.WordRtfInfo, "width:9.0cm");
			AreSame( pict.DebugString, "");

			// Check the reference
			IsTrue(SNew.ReferenceSpan.ContentEquals(Section1.ReferenceSpan));
		}
		#endregion
	}
	#endregion
}
