/**********************************************************************************************
 * Project: Our Word!
 * File:    DParagraph.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2004
 * Purpose: Handles a paragraph and its related back translation, interlinear BT, etc.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using System.Xml;

using JWTools;
using JWdb;
#endregion

namespace JWdb.DataModel
{

	public class DParagraph : JObject 
	{
		// ZAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string StyleAbbrev - e.g., "p", "q", "q2".
        public string StyleAbbrev
        {
            get
            {
                return m_sStyleAbbrev;
            }
            set
            {
                SetValue(ref m_sStyleAbbrev, value);
            }
        }
        private string m_sStyleAbbrev = "p";
        #endregion
        #region JAttr{g}: JOwnSeq Runs - seq of DRun (verse no, chapter no, TextElement)
		public JOwnSeq<DRun> Runs
		{
			get
			{
				return j_osRuns;
			}
		}
		private JOwnSeq<DRun> j_osRuns;
		#endregion
        #region OMethod: void DeclareAttrs()
        protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
            DefineAttr("Abbrev", ref m_sStyleAbbrev);
        }
		#endregion

		// Run-Time Only Attrs ---------------------------------------------------------------
		#region Temp Attr{g/s}: bool AddedByCluster - so we don't save temp para's
		public bool AddedByCluster
		{
			get
			{
				if (SimpleText.Length > 0)
					return false;
				return m_AddedByCluster;
			}
			set
			{
				m_AddedByCluster = value;
			}
		}
		private bool m_AddedByCluster = false;
		#endregion

		// Run-Time Only Attrs: Chapter / Verse ----------------------------------------------
		#region Attr{g}: int ChapterI - the initial chapter number of this paragraph
		public int ChapterI
		{
			get
			{
				foreach(DRun run in Runs)
				{
					if (null != run as DText)
						break;
					if (null != run as DChapter)
						return (run as DChapter).ChapterNo;
				}
				return m_nChapterI;
			}
			set
			{
				m_nChapterI = value;
			}
		}
		private int m_nChapterI = 0;
		#endregion
		#region Attr{g}: int VerseI - the initial verse number of this paragraph
		public int VerseI
		{
			get
			{
				// Search through the paragraph, and return the first verse found.
				// If we have seen text prior to the verse number , then we return the
				// previous verse number, 
				bool bTextFound = false;
				foreach(DRun run in Runs)
				{
					if (null != run as DText)
						bTextFound = true;

					DVerse verse = run as DVerse;
					if (null != verse)
					{
						int nVerse = verse.VerseNo;
						if (bTextFound)
							nVerse--;
						return Math.Max(nVerse, 1);
					}
				}

				// If we got here, then we have to rely on whatever was (hopefully) 
				// previously stored.
				return m_nVerseI;
			}
			set
			{
				m_nVerseI = value;
			}
		}
		private int m_nVerseI = 0;
		#endregion
		#region Attr{g}: int ChapterF - the final chapter number of this paragraph
		public int ChapterF
		{
			get
			{
				int n = ChapterI;

				foreach(DRun run in Runs)
				{
					DChapter chapter = run as DChapter;

					if (null != chapter)
					{
						n = chapter.ChapterNo;
					}
				}

				return n;
			}
		}
		#endregion
		#region Attr{g}: int VerseF - the final verse number of this paragraph
		public int VerseF
		{
			get
			{
                // Default to the Initial Verse; if there are no DVerses in the
                // paragraph, then Start and End verse numbers will be the same.
				int n = VerseI;

				foreach(DRun run in Runs)
				{
                    // If we find a Chapter, then reset the verse (n) to 1
					DChapter chapter = run as DChapter;
					if (null != chapter)
						n = 1;

                    // Any DVerses we find will increment which verse we have
					DVerse verse = run as DVerse;
					if (null != verse)
					{
						int nVerse = verse.VerseNoFinal;
						n = Math.Max(n, nVerse);
					}
				}

				return n;
			}
		}
		#endregion
		#region Attr{g}: bool IsSameReferenceAs(DParagraph p)
		public bool IsSameReferenceAs(DParagraph p)
		{
			if (ChapterI != p.ChapterI)
				return false;
			if (ChapterF != p.ChapterF)
				return false;
			if (VerseI != p.VerseI)
				return false;
			if (VerseF != p.VerseF)
				return false;
			return true;
		}
		#endregion
        #region Attr{g}: int FirstActualVerseNumber
        /// <summary>
        /// Searches the paragraph for its verse DVerse, and returns its number. If there
        /// is no DVerse in the paragraph, then returns 0.
        /// </summary>
        public int FirstActualVerseNumber
        {
            get
            {
                foreach (DRun r in Runs)
                {
                    DVerse v = r as DVerse;
                    if (null != v)
                        return v.VerseNo;
                }
                return 0;
            }
        }
        #endregion

		// Derived Attrs: Ownership Hierarchy ------------------------------------------------
		#region Attr[g}: DSection Section - the owning section
		public DSection Section
		{
			get
			{
                // Somewhere up the ownership hierarchy, we are owned by a section. Most
                // paragraphs are immediately owned by the section, but those in Translator
                // Notes are several levels down.
                JObject obj = Owner;
                while (null != obj && obj as DSection == null)
                    obj = obj.Owner;

                DSection section = obj as DSection;
                Debug.Assert(null != section, "Paragraph is not (eventually) owned by a DSection");

                return section;
			}
		}
		#endregion
		#region Attr[g}: DBook Book - the owning book
		public DBook Book
		{
			get
			{
				return Section.Book;
			}
		}
		#endregion
		#region Attr[g}: DTranslation Translation - the owning translation
		public DTranslation Translation
		{
			get
			{
				return Section.Translation;
			}
		}
		#endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region Attr{g}: DPhrase[] Phrases - flattens the list to just return phrases
		DPhrase[] Phrases
		{
			get
			{
				ArrayList list = new ArrayList();

				foreach(DRun run in Runs)
				{
					DText text = run as DText;
					if (null != text)
					{
						foreach( DPhrase phrase in text.Phrases)
							list.Add(phrase);
					}
				}

				DPhrase[] phrases = new DPhrase[ list.Count ];
				for(int i=0; i < list.Count; i++)
					phrases[i] = list[i] as DPhrase;
				return phrases;
			}
		}
		#endregion
		#region Attr{g}: string TypeCodes - A string representing the types of the DRun subclasses
		public string TypeCodes
		{
			get
			{
				string s = "";

				foreach(DRun run in Runs)
				{
					s += run.TypeCode;
				}

				return s;
			}
		}
		#endregion
		#region Attr{g}: string StructureCodes -Like TypeCodes, except DTexts ignored
		public string StructureCodes
		{
			get
			{
				string s = "";

				foreach(DRun run in Runs)
				{
					if (DRun.c_codeNormal != run.TypeCode)
						s += run.TypeCode;
				}

				return s;
			}
		}
		#endregion
		#region Attr{g}: string DebugString
		public virtual string DebugString
		{
			get
			{
				string s = "";
				foreach(DRun run in Runs)
				{
					s += run.DebugString;
				}
				return s;
			}
		}
		#endregion
		#region Attr{g}: bool IsCompletelyEmpty - T if nothing in the paragraph
		public bool IsCompletelyEmpty
		{
			get
			{
				// If we have no runs, then we are empty
				if (Runs.Count == 0)
					return true;

				// If we have exactly one run, and if it is a DText, and if its
				// contents are a zero-length string, then we are considered to be 
				// empty.
				if (Runs.Count == 1 && 
					null != Runs[0] as DText && 
					SimpleText.Length == 0 &&
					SimpleTextBT.Length == 0)
				{
					return true;
				}

				// All other cases are considered to have contents
				return false;
			}
		}
		#endregion
        #region Attr{g}: bool HasDBT - true if the paragraph has a DBasicText in its Runs
        public bool HasDBT
        {
            get
            {
                foreach (DRun run in Runs)
                {
                    if (null != run as DBasicText)
                        return true;
                }
                return false;
            }
        }
        #endregion
        #region Attr{g}: bool HasDText - true if the paragraph has a DText in its Runs
		public bool HasDText
		{
			get
			{
				foreach(DRun run in Runs)
				{
					if (null != run as DText)
						return true;
				}
				return false;
			}
		}
		#endregion
		#region Attr{g}: bool HasStyleAssigned - T if StyleAbbrev is not ""
		public bool HasStyleAssigned
		{
			get 
			{ 
				return "" != StyleAbbrev; 
			}
		}
		#endregion
		#region Attr{g}: bool HasItalics - T if the italics style is within the paragraph
		public bool HasItalics
		{
			get
			{
				foreach ( DPhrase phrase in Phrases )
				{
					if (phrase.CharacterStyleAbbrev == DStyleSheet.c_StyleAbbrevItalic)
						return true;
				}
				return false;
			}
		}
		#endregion
		#region Attr{g}: virtual bool IsUserEditable
		public virtual bool IsUserEditable
		{
			get
			{
				if (DB.Map.StyleCrossRef == StyleAbbrev)
					return false;
				return true;
			}
		}
		#endregion
		#region Attr{g/s}: JParagraphStyle Style - the paragraph style for this paragraph
		public JParagraphStyle Style
		{
			get 
			{ 
				return DB.StyleSheet.FindParagraphStyle(StyleAbbrev);
			}
		}
		#endregion
		#region Attr{g}: string AsString  - the text contents of the paragraph
		public string AsString
		{
			get
			{
				string s = "";

				foreach(DRun run in this.Runs)
				{
					s += run.AsString;
				}

				return s;
			}
		}
		#endregion
		#region Attr{g}: string ProseBTAsString  - the BT text contents of the paragraph
		public string ProseBTAsString
		{
			get
			{
				string s = "";

				foreach(DRun run in this.Runs)
				{
					s += run.ProseBTAsString;
				}

				return s;
			}
		}
		#endregion
        #region Attr{g}: DReferenceSpan ReferenceSpan
        public DReferenceSpan ReferenceSpan
        {
            get
            {
                DReferenceSpan span = new DReferenceSpan();

                span.Start.Chapter = ChapterI;
                span.Start.Verse = VerseI;
                span.End.Chapter = ChapterF;
                span.End.Verse = VerseF;

                return span;
            }
        }
        #endregion
        #region VAttr{g}: bool HasBackTranslationText
        public bool HasBackTranslationText
        {
            get
            {
                foreach (DRun run in Runs)
                {
                    DBasicText text = run as DBasicText;
                    if (text != null && !string.IsNullOrEmpty(text.ProseBTAsString))
                        return true;
                }
                return false;
            }
        }
        #endregion
        #region VAttr{g}: int EditableTextLength
        /// <summary>
        /// Returns the length of the editable parts of the paragraph.
        /// </summary>
        public int EditableTextLength
        {
            get
            {
                int c = 0;

                foreach (DRun run in Runs)
                {
                    DBasicText text = run as DBasicText;
                    if (null != text)
                    {
                        c += text.PhrasesLength;
                    }
                }

                return c;
            }
        }
        #endregion
        #region VAttr{g}: List<string> CanChangeParagraphStyleTo
        public List<string> CanChangeParagraphStyleTo
        {
            get
            {
                // Can't change the style for Annotation Messages
                if (null != this as DMessage)
                    return null;

                // We'll compile the possible styles here
                var vPossibilities = new List<string>();

                // Some styles are always possibilities
                vPossibilities.Add(DStyleSheet.c_sfmParagraph);
                vPossibilities.Add(DStyleSheet.c_sfmLine1);
                vPossibilities.Add(DStyleSheet.c_sfmLine2);
                vPossibilities.Add(DStyleSheet.c_sfmLine3);

                // Is Scripture (rather than, e.g., a Translator Note)
                bool bIsScripture = (Owner == Section);
                if (bIsScripture)
                {
                    vPossibilities.Add(DStyleSheet.c_sfmMajorSection);
                    vPossibilities.Add(DStyleSheet.c_sfmSectionHead);
                    vPossibilities.Add(DStyleSheet.c_sfmSectionHeadMinor);
                }

                // Scripture in the First Section of the book
                bool bIsFirstSection = (Book.Sections.FindObj(Section) == 0);
                if (bIsFirstSection && bIsScripture)
                {
                    vPossibilities.Add(DStyleSheet.c_sfmBookTitle);
                    vPossibilities.Add(DStyleSheet.c_sfmBookSubTitle);
                }

                // Make sure our current style is present.
                if (!vPossibilities.Contains(StyleAbbrev))
                    vPossibilities.Add(StyleAbbrev);

                return vPossibilities;
            }
        }
        #endregion
        #region VAttr{g}: List<DFootnote> AllFootnotes
        public List<DFootnote> AllFootnotes
        {
            get
            {
                var v = new List<DFootnote>();

                foreach (DRun run in Runs)
                {
                    if (run as DFoot != null)
                        v.Add((run as DFoot).Footnote);
                }

                return v;
            }
        }
        #endregion

        // REVISION =============

		#region Method: void AddRun(DRun)
		public void AddRun(DRun run)
		{
			if (null != run)
				Runs.Append(run);
		}
		#endregion

		#region Method: void ConnectFootnote(DFootnote footnote)
		public void ConnectFootnote(DFootnote footnote)
		{
            // Look for a DFoot whose Footnote attribute is still null
			foreach(DRun run in Runs)
			{
                DFoot foot = run as DFoot;
                if (null != foot && null == foot.Footnote)
                {
                    foot.Footnote = footnote;
                    return;
                }
			}

			// If we get to here, then it means we do not have a marker in the paragraph
			// for this footnote. So we need to append a marker to the end of the
			// paragraph.
            Runs.Append(new DFoot(footnote));
		}
		#endregion

		#region Attr{g/s}: string SimpleText - For those paragraphs that have a single Run and Phrase
		public string SimpleText
		{
			get
				// The Get method returns just the raw text; multiple runs and phrases are 
				// permitted and left unchanged; but any chapters, verses, footletters, etc.,
				// are ignored; and any formatting (e.g., italic, bold) is lost. What is
				// left is just the text in a single string.
			{
				string s = "";

				foreach(DRun run in Runs)
				{
					DText text = run as DText;
					if (null != text)
					{
						foreach(DPhrase phrase in text.Phrases)
							s += phrase.Text;
					}
				}

				return s;
			}
			set
				// The Set method removes any former contents of the paragraph, and replaces
				// them with a single run containing a single, Normal phrase. 
			{
				if (Runs.Count > 1)
					Runs.Clear();
				if (Runs.Count == 0)
					AddRun(new DText());
				DText text = Runs[0] as DText;

				text.Phrases.Clear();

				DPhrase phrase = new DPhrase( StyleAbbrev, value);
				text.Phrases.Append( phrase );

			}
		}
		#endregion
		#region Attr{g/s}: string SimpleTextBT - For those paragraphs that have a single Run and Phrase
		public string SimpleTextBT
		{
			get
				// The Get method returns just the raw text; multiple runs and phrases are 
				// permitted and left unchanged; but any chapters, verses, footletters, etc.,
				// are ignored; and any formatting (e.g., italic, bold) is lost. What is
				// left is just the text in a single string.
			{
				string s = "";

				foreach(DRun run in Runs)
				{
					DText text = run as DText;
					if (null != text)
					{
						foreach(DPhrase phrase in text.PhrasesBT)
							s += phrase.Text;
					}
				}

				return s;
			}
			set
				// The Set method removes any former contents of the paragraph, and replaces
				// them with a single run containing a single, Normal phrase. 
			{
				if (Runs.Count > 1)
					Runs.Clear();
				if (Runs.Count == 0)
					AddRun(new DText());
				DText text = Runs[0] as DText;

				text.PhrasesBT.Clear();

				DPhrase phrase = new DPhrase( StyleAbbrev, value);
				text.PhrasesBT.Append( phrase );

			}
		}
		#endregion

		#region Method: void BestGuessAtInsertingTextPositions()
		public void BestGuessAtInsertingTextPositions()
			// We figure there should minimally be a DText:
			// 1. following a verse
			// 2. paragraph initial if the first thing would otherwise be a footnote
			// 3. if the paragraph is otherwise empty
            //
            // We use the SuppressDeclareDirty option when entering these, because 
            // the changes are not saved when the book goes out to standard format;
            // therefore we don't wish to create a save that has no changes on the
            // disk. 
		{
			// Case 1: If we have a DVerse, then we should have a place to type following it.
			for(int i=0; i<Runs.Count; i++)
			{
				DVerse verse = Runs[i]   as DVerse;

				DText text = null;
				if ( (i + 1 ) < Runs.Count )
					text = Runs[ i + 1 ] as DText;

				if (null != verse && null == text)
				{
					Runs.InsertAt( i+1, DText.CreateSimple(), true );
				}
			}

			// Case 2: A paragraph-initial footnote or cross reference, then there should
			// be a place to type prior to it.
			if (Runs.Count > 0)
			{
                if (Runs[0] as DFoot != null)
					Runs.InsertAt(0, DText.CreateSimple(), true );
			}

			// Case 3: An empty paragraph should have a place to type
			if (Runs.Count == 0)
			{
				Runs.Append( DText.CreateSimple(), true );
			}
		}
		#endregion
		#region Method: void SynchRunsToModelParagraph((DParagraph pModel)
		public void SynchRunsToModelParagraph(DParagraph pModel)
			// The Target paragraph should have the same sequence of runs as the Model
			// paragraph. 
		{
			// If the structures are different, then we have incompatable paragraphs, and
			// there is nothing further to do here.
			if (StructureCodes != pModel.StructureCodes)
			{
				BestGuessAtInsertingTextPositions();
				return;
			}

			// Add DTexts to the target as needed
			int iTarget = 0;
			for(int i = 0; i < pModel.Runs.Count; i++, iTarget++)
			{
				// Retrieve the Model's run
				DRun runModel = pModel.Runs[i] as DRun;

				// Retrieve the target's run (if any)
				DRun runTarget = null;
				if (iTarget < Runs.Count)
					runTarget = Runs[iTarget] as DRun;

				// If the model is a DText, but if the Target isn't, then we need to
				// insert a DText into the Target.
				bool bModelIsDText = (null != runModel as DText);
				bool bTargetIsNot  = (null == runTarget || null == runTarget as DText);
				// The previous run should not be a DText, else we are merely inserting
				// one adjacent to another, which is not necessary.
				bool bTargetPrevIsDText = true;
				if (iTarget == 0 || 
					null == Runs[iTarget-1] || 
					null == Runs[iTarget-1] as DText)
				{
					bTargetPrevIsDText = false;
				}
				if (bModelIsDText && bTargetIsNot && !bTargetPrevIsDText)
				{
					DText text = DText.CreateSimple();
					Runs.InsertAt(iTarget, text, true);
				}
			}
		}
		#endregion

		#region Method: void CombineAdjacentDTexts()
		public void CombineAdjacentDTexts(bool bInsertSpacesBetweenPhrases)
			// Combine any adjacent DTexts. These can arise, e.g., when there are 
			// multiple \vt fields following a single \v field (which can easily happen
			// in Toolbox when the user combines verses into a verse bridge.
		{
			for(int i=0; i<Runs.Count - 1; )
			{
				DText text1 = Runs[i] as DText;
				DText text2 = Runs[i+1] as DText;

				if (null != text1 && null != text2)
				{
                    text1.Append(text2, bInsertSpacesBetweenPhrases);
					Runs.Remove(text2);
				}
				else
					i++;
			}
		}
		#endregion
		#region Method: void Cleanup()
		public void Cleanup()
		{
			// If we have any DFootLetter's or DSeeAlso's that do not have a DFootnote
			// attached to them, then we remove them from the paragraph.
			for(int i=0; i < Runs.Count; )
			{
				DFoot foot = Runs[i] as DFoot;

                if (null != foot && null == foot.Footnote)
                    Runs.RemoveAt(i);
                else
                    i++;
			}

			CombineAdjacentDTexts(true);
             
			// Clean up any spaces within individual runs:
			// - No double (or more) spaces
			// - No leading or trailing spaces
			// - No "insertion" spaces
			foreach(DRun run in Runs)
				run.EliminateSpuriousSpaces();
			CombineAdjacentDTexts(true);

			// Make sure we have at a minimum a DText with a DPhrase; that is, we
			// do not want a paragraph that has no runs
			if (0 == Runs.Count)
			{
				SimpleText = "";
				SimpleTextBT = "";
			}

            // Make sure that all DTexts have phrases
            foreach (DRun run in Runs)
            {
                DBasicText text = run as DBasicText;
                if (null != text)
                    text.Cleanup();
            }
		}
		#endregion

        // Methods involving splitting -------------------------------------------------------
        #region Method: DBasicText SplitText(DBasicText dbtToSplit, int iTextSplitPos)
        #region Implementation Class: SplitTextMethod
        class SplitTextMethod
        {
            // Attrs: the DBT and position within that we're working with --------------------
            #region Attr{g}: DBasicText DBT - the DBT to be split
            protected DBasicText DBT
            {
                get
                {
                    Debug.Assert(null != m_dbt);
                    return m_dbt;
                }
            }
            DBasicText m_dbt;
            #endregion
            #region Attr{g}: int iPos - 0-based split position within the DBT
            protected int iPos
            {
                get
                {
                    return m_iPos;
                }
            }
            int m_iPos;
            #endregion
            #region Attr{g}: DParagraph Paragraph - the (original) owner of DBT
            protected DParagraph Paragraph
            {
                get
                {
                    Debug.Assert(null != m_Paragraph);
                    return m_Paragraph;
                }
            }
            DParagraph m_Paragraph;

            #endregion

            // Helper methods ----------------------------------------------------------------
            #region VAttr{g}: bool SplitIsLegal
            protected bool SplitIsLegal
            {
                get
                {
                    // Styles for which splitting text is not permitted
                    string[] vs = new string[] 
                        { 
                        DStyleSheet.c_StyleAbbrevPictureCaption,
                        DStyleSheet.c_StyleNote,
                        DStyleSheet.c_StyleFootnote
                        };

                    // See if this style is one of the forbidden ones
                    foreach (string sAbbrev in vs)
                    {
                        if (Paragraph.StyleAbbrev == sAbbrev)
                            return false;
                    }

                    return true;
                }
            }
            #endregion
            #region VAttt{g}: DBasicText NextDBT
            DBasicText NextDBT
            {
                get
                {
                    bool bFound = false;

                    foreach (DRun run in Paragraph.Runs)
                    {
                        if (run as DBasicText != null && bFound)
                            return run as DBasicText;

                        if (run as DBasicText == DBT)
                            bFound = true;
                    }

                    return null;
                }
            }
            #endregion

            #region Method: DPhrase SplitPhrase()
            protected DPhrase SplitPhrase()
                // We expect to have a Left and a Right phrase, with the split happening
                // in between them. Thus we need to split the phrase in two, unless we
                // are at a phrase boundary (in which case no action is necessary.)
                //
                // Returns the right-hand phrase.
            {
                // Find the phrase and position within, where the split will occur. 
                DPhrase phrase = null;
                int iPosWithinPhrase = iPos;
                foreach (DPhrase p in DBT.Phrases)
                {
                    if (p.Text.Length > iPosWithinPhrase)
                    {
                        phrase = p;
                        break;
                    }
                    iPosWithinPhrase -= p.Text.Length;
                }

                // If we are in the midst of a phrase, then split it
                if (null != phrase && 
                    iPosWithinPhrase > 0 &&
                    iPosWithinPhrase < phrase.Text.Length)
                {
                    phrase = DBT.Phrases.Split(phrase, iPosWithinPhrase);
                }

                // If we aren't pointing to a phrase, then we must be at the end of one; so 
                // we need to advance to the next DBT
                if (null == phrase)
                {
                    DBasicText dbtNext = NextDBT;

                    if (null == dbtNext)
                    {
                        dbtNext = DText.CreateSimple();
                        Paragraph.Runs.Append(dbtNext);
                    }

                    phrase = dbtNext.Phrases[0] as DPhrase;
                }

                Debug.Assert(null != phrase);
                return phrase;
            }
            #endregion
            #region Method: DBasicText SplitText(DPhrase phraseRight)
            protected DBasicText SplitText(DPhrase phraseRight)
            {
                // Retrieve the text that is the parent of the phrase we're moving
                DBasicText textToSplit = phraseRight.BasicText;
                Debug.Assert(null != textToSplit);

                // Retrieve its position within the paragraph
                int iTextPos = Paragraph.Runs.FindObj(textToSplit);
                Debug.Assert(-1 != iTextPos);

                // Retrieve the position of the phrase within the parent text
                int iPhrasePos = textToSplit.Phrases.FindObj(phraseRight);
                Debug.Assert(-1 != iPhrasePos);

                // If it isn't the first position, then we need to split the DText into two
                if (iPhrasePos != 0)
                {
                    DText textRight = new DText();
                    Paragraph.Runs.InsertAt(iTextPos + 1, textRight);

                    while (textToSplit.Phrases.Count > iPhrasePos)
                    {
                        DPhrase phrase = textToSplit.Phrases[iPhrasePos] as DPhrase;
                        textToSplit.Phrases.Remove(phrase);
                        textRight.Phrases.Append(phrase);
                    }

                    textRight.PhrasesBT.Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));

                    return textRight;
                }

                return textToSplit;
            }
            #endregion

            // Public Interface --------------------------------------------------------------
            #region Constructor(DBT, iPos)
            public SplitTextMethod(DBasicText _dbtToSplit, int _iPos)
            {
                // The owning paragraph
                m_Paragraph = _dbtToSplit.Paragraph;

                // The DBT and position where the split is anticipated
                m_dbt = _dbtToSplit;
                m_iPos = _iPos;
            }
            #endregion
            #region Method: DBasicText Do()
            public DBasicText Do()
            {
                // Make sure the split is allowed
                if (!SplitIsLegal)
                    return null;

                // Determine the target (right-hand side) phrase, including splitting 
                // it if we're in the middle of it
                DPhrase phraseRight = SplitPhrase();

                // Determine the target DBasicText, including splitting it if we're in the middle of it
                DBasicText dbtRight = SplitText(phraseRight);
                return dbtRight;
            }
            #endregion
        }
        #endregion
        DBasicText SplitText(DBasicText dbtToSplit, int iTextSplitPos)
        {
            SplitTextMethod m = new SplitTextMethod(dbtToSplit, iTextSplitPos);
            return m.Do();
        }
        #endregion
        // Split / Join Paragraphs -----------------------------------------------------------
        #region Method: DParagraph Split(DBasicText textToSplit, int iTextSplitPos)
        /// <summary>
        /// Splits the paragraph into two.
        /// </summary>
        /// <param name="textToSplit">The DBasicText within the paragraph that will be split.</param>
        /// <param name="iTextSplitPos">The position within the "textToSplit" where the split will happen.</param>
        public DParagraph Split(DBasicText dbtToSplit, int iTextSplitPos)
        {
            // Shorthand
            DParagraph Paragraph = dbtToSplit.Paragraph;

            // Split the text (and internal phrase) into two halves. We'll move the right
            // half into another paragraph
            DBasicText dbtRight = SplitText(dbtToSplit, iTextSplitPos);
            if (null == dbtRight)
                return null;

            // Create and insert  a new, empty paragraph to receive the right-side of the split
            JOwnSeq<DParagraph> seq = Paragraph.GetMyOwningAttr() as JOwnSeq<DParagraph>;
            DParagraph paraNew = new DParagraph();
            paraNew.StyleAbbrev = Paragraph.StyleAbbrev;
            int iParaNew = seq.FindObj(Paragraph) + 1;
            seq.InsertAt(iParaNew, paraNew);

            // Move the runs to the new paragraph
            int iMove = Paragraph.Runs.FindObj(dbtRight);
            Debug.Assert(iMove != -1); // Shouldn't be -1 (not found)
            while (Paragraph.Runs.Count > iMove)
            {
                DRun run = Paragraph.Runs.RemoveAt(iMove) as DRun;
                Debug.Assert(null != run);
                paraNew.Runs.Append(run);
            }

            // Boundary condition: if we just moved all of the editable stuff out of the paragraph,
            // then we need to insert something blank and editable
            if (!Paragraph.HasDBT)
                Paragraph.Runs.Append(DText.CreateSimple());

            // Boundary condition: the new paragraph must have a text as well
            if (!paraNew.HasDBT)
                paraNew.Runs.InsertAt(0, DText.CreateSimple());

            // Boundary condition: don't let original Paragraph end in Verse or Chapter
            while (Paragraph.Runs.Count > 0)
            {
                int i = Paragraph.Runs.Count - 1;
                DRun run = Paragraph.Runs[i] as DRun;
                if (run as DVerse == null && run as DChapter == null)
                    break;

                Paragraph.Runs.Remove(run);
                paraNew.Runs.InsertAt(0, run);
            }

            // Return our new paragraph
            return paraNew;
        }
        #endregion
        #region Method: void JoinToNext()
        public void JoinToNext()
        {
            // Retrieve the sequence that owns this paragraph
            JOwnSeq<DParagraph> seq = GetMyOwningAttr() as JOwnSeq<DParagraph>;
            Debug.Assert(null != seq);

            // Retrieve the paragraph that follows this one
            int iNext = seq.FindObj(this) + 1;
            if (iNext >= seq.Count)
                return;
            DParagraph pNext = seq[iNext] as DParagraph;
            Debug.Assert(null != pNext);

            // Move its runs into this one
            while (pNext.Runs.Count > 0)
            {
                DRun run = pNext.Runs[0] as DRun;
                pNext.Runs.Remove(run);
                this.Runs.Append(run);
            }

            // Remove it from the owner
            seq.Remove(pNext);

            // Get rid of any spurious spaces, etc.
            //
            // (Note: I wonder if I really need this? I definiteily don't want to call
            // Cleanup, because that can get rid of leading spaces, etc., which then
            // mess up the selection when OW attempts to restore it following
            // the join action. See Bug0281.)
            CombineAdjacentDTexts(false);  // Need to call this first with "false"
        }
        #endregion
        // Insert / Remove Footnotes ---------------------------------------------------------
        #region Method: DFootLetter InsertFootnote(DBasicText, iPos)
        public DFoot InsertFootnote(DBasicText dbt, int iPos)
        {
            // Illegal to insert a footnote within a footnote
            if (this as DFootnote != null)
                return null;

            // If at the beginning of a dbt, then we do nothing
            if (0 == iPos)
                return null;

            // Split the text (and internal phrase) into two halves if we're not
            // at a boundary. 
            DBasicText dbtRight = SplitText(dbt, iPos);
            if (null == dbtRight)
                return null;
            int iInsertPosition = Runs.FindObj(dbtRight);
            Debug.Assert(-1 != iInsertPosition);

            // We need to back up the insertion position in certain cases:
            // - we would be inserting to the right of a verse or a chapter
            while (iInsertPosition > 0)
            {
                if (Runs[iInsertPosition - 1] as DVerse != null)
                    --iInsertPosition;
                else if (Runs[iInsertPosition - 1] as DChapter != null)
                    --iInsertPosition;
                else
                    break;
            }
            // Note that we don't expect at this point to be taken down to zero,
            // as that would imply that we were at a boundary beginning. which
            // would have meant a null dbtRight up above.
            Debug.Assert(0 != iInsertPosition);

            // Deterermine the chapter and verse
            int nChapter = ChapterI;
            int nVerse = VerseI;
            for (int i = 0; i < iInsertPosition; i++)
            {
                DChapter chapter = Runs[i] as DChapter;
                if (null != chapter)
                {
                    nChapter = chapter.ChapterNo;
                    nVerse = 1;
                }

                DVerse verse = Runs[i] as DVerse;
                if (null != verse)
                {
                    nVerse = verse.VerseNo;
                }
            }

            // Create an empty footnote, insert it into the section
            DFootnote footnote = new DFootnote(nChapter, nVerse, 
                DFootnote.Types.kExplanatory);
            footnote.SimpleText = "";         // Insert some text so we have a place to type

            // Create the footletter for it to go into, and insert it
            DFoot foot = new DFoot(footnote);
            Runs.InsertAt(iInsertPosition, foot);

            return foot;
        }
        #endregion
        #region Method: void RemoveFootnote(DFoot)
        public void RemoveFootnote(DFoot foot)
        {
            // Remove the footnote
            Runs.Remove(foot);
            CombineAdjacentDTexts(false);
        }
        #endregion

		#region Method: PWord[] GetPWords()
		public PWord[] GetPWords()
		{
			ArrayList al = new ArrayList();
			foreach(DRun r in Runs)
			{
				al.Add( r.GetPWords() );
			}

			int c = 0;
			foreach( PWord[] vpw in al )
				c += vpw.Length;

			PWord[] v = new PWord[ c ];

			int i = 0;
			foreach( PWord[] vpw in al )
			{
				foreach( PWord pw in vpw )
				{
					v[i++] = pw;
				}
			}
			return v;
		}
		#endregion

		// Translator Notes ------------------------------------------------------------------
		#region DText GetOrAddLastDText() - Used during the SF Read operation
		public DText GetOrAddLastDText()
		{
			// Look for a DText within the paragraph
			DText text = null;
			foreach(DRun run in Runs)
			{
				if (null != run as DText)
					text = run as DText;
			}

			// If one is not found, then add one
			if (null == text)
			{
				text = new DText();
				AddRun( text );
			}

			return text;
		}
		#endregion
        #region Method: List<TranslatorNote> GetAllTranslatorNotes()
        public List<TranslatorNote> GetAllTranslatorNotes()
        {
            var v = new List<TranslatorNote>();

            foreach (DRun run in Runs)
            {
                // Notes that are attached to this text
                var text = run as DText;
                if (null != text)
                {
                    foreach (TranslatorNote note in text.TranslatorNotes)
                        v.Add(note);
                }

                // Notes that are attached to this footnote
                var foot = run as DFoot;
                if (null != foot)
                {
                    v.AddRange(foot.Footnote.GetAllTranslatorNotes());
                }
            }

            return v;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DParagraph()
            : base()
        {
            j_osRuns = new JOwnSeq<DRun>("Runs", this, false, false);

            StyleAbbrev = "";
        }
        #endregion
		#region Method: void CopyFrom(DParagraph pFront)
		public virtual void CopyFrom(DParagraph pFront, bool bTruncateText)
		{
			// Same reference
			ChapterI = pFront.ChapterI;
			VerseI   = pFront.VerseI;

			// Same style
			StyleAbbrev = pFront.StyleAbbrev;

			// If a cross reference, then convert it
			if (DB.TeamSettings.SFMapping.IsCrossRef( StyleAbbrev ) )
			{
				Translation.ConvertCrossReferences(pFront, this);
				return;
			}

			// Create the content (copying chapters and verses, blanks for phrases, etc.)
            Runs.Clear();
			foreach( DRun run in pFront.Runs)
			{
				// Verse
				DVerse verse = run as DVerse;
				if (null != verse)
				{
					Runs.Append( DVerse.Copy(verse) );
					continue;
				}

				// Chapter
				DChapter chapter = run as DChapter;
				if (null != chapter)
				{
					Runs.Append( DChapter.Copy(chapter) );
					continue;
				}

				// Footnote
                DFoot foot = run as DFoot;
                if (null != foot)
                {
                    DFootnote fnFront = foot.Footnote;
                    DFootnote fnTarget = new DFootnote(fnFront.VerseReference, fnFront.NoteType);

                    Runs.Append(new DFoot(fnTarget));

                    if (foot.IsSeeAlso)
                        fnTarget.ConvertCrossReferences(fnFront);

                    continue;
                }

                // DLabel
                DLabel label = run as DLabel;
                if (null != label)
                {
                    Runs.Append(new DLabel(label.Text));
                    continue;
                }

				DText textFront = run as DText;
				if (null != textFront)
				{
                    if (bTruncateText)
                        Runs.Append(DText.CreateSimple());
                    else
                    {
                        DText t = new DText();
                        textFront.CopyDataTo(t);
                        Runs.Append(t);
                    }
					continue;
				}

				Debug.Assert(false, "A subclass of DRun we don't handle!");
			}
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DParagraph p = obj as DParagraph;

			if (this.StyleAbbrev != p.StyleAbbrev)
				return false;

			if (this.Runs.Count != p.Runs.Count)
				return false;

			for(int i = 0; i < Runs.Count; i++)
			{
				DRun run1 = Runs[i] as DRun;
				DRun run2 = p.Runs[i] as DRun;

				if (false == run1.ContentEquals(run2))
					return false;
			}

			return true;
		}
		#endregion

        // I/O -------------------------------------------------------------------------------
        const string c_sAttrContents = "Contents";
        const string c_sAttrBT = "BT";
        #region OMethod: XElement ToXml(bool bIncludeNonBasicAttrs)
        public override XElement ToXml(bool bIncludeNonBasicAttrs)
        {
            // Afraid of future changes to the class definition. This first assertion means
            // that the only JAttr we have is Runs.
            Debug.Assert(AllAttrs.Count == 1);
            // This next assertion means we have exactly two BAttrs (OID and StyleAbbrev)
            Debug.Assert(BAttrCount == 1);
            // If any of this changes, then ToXml and FromXml will need to be updated.

            // If we have exactly one DText, and no other runs, then we do a shorter
            // output, in order to make the disk files a bit more human readable
            // and concise.
            // So first we test to see if the condition applies. If it doesn't, then
            // we let the normal mechanism do the work.
            bool bCanShorthand = (Runs.Count == 1 && Runs[0] as DText != null);
            if (!bCanShorthand)
                return base.ToXml(bIncludeNonBasicAttrs);

            // Create the XElement for this object
            XElement x = new XElement(GetType().Name);

            // Do the basic attributes
            x.AddAttr("Abbrev", m_sStyleAbbrev);

            // Do our one-and-only DText
            DText text = Runs[0] as DText;
            string sContents = text.Phrases.ToSaveString;
            string sBT = text.PhrasesBT.ToSaveString;
            x.AddAttr(c_sAttrContents, sContents);
            if (!string.IsNullOrEmpty(sBT))
                x.AddAttr(c_sAttrBT, sBT);

            // Done
            return x;
        }
        #endregion
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            // If we have no subitems, ite means we used our special override of ToXml
            // Thus if we have subitems, then we let the base method take care of it.
            if (x.Items.Count > 0)
            {
                base.FromXml(x);
                return;
            }

            /***
            // If we have a Contents attribute, then it means we used our special override
            // of ToXml.
            if (null == x.FindAttr("Contents"))
            {
                base.FromXml(x);
                return;
            }
            ***/

            // We want exactly one DText
            Clear();
            DText text = new DText();
            Runs.Append(text);

            // So if we are here, we do the reverse of ToXml
            // First, get the abbrev
            XElement.XAttr attr = x.FindAttr("Abbrev");
            if (null != attr)
                m_sStyleAbbrev = attr.Value;

            // Retrieve the contents
            attr = x.FindAttr(c_sAttrContents);
            if (null != attr)
                text.Phrases.FromSaveString(attr.Value);
            else
                text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));

            // Retrieve the back translation
            attr = x.FindAttr(c_sAttrBT);
            if (null != attr)
                text.PhrasesBT.FromSaveString(attr.Value);
            else
                text.PhrasesBT.Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));
        }
        #endregion

        /*
        public int ID
        {
            get
            {
                EnsureHasID();
                return m_nID;
            }
        }
        int m_nID = -1;
        public void EnsureHasID()
        {
            if (m_nID == -1)
                m_nID = Book.GetID();
            Debug.Assert(m_nID > -1);
        }
        */

        // Oxes ------------------------------------------------------------------------------
        const string c_sTagParagraph = "p";
        const string c_sAttrStyle = "class";
        const string c_sAttrUsfm = "usfm";
        #region Method: void ReadOxesPhrase(XmlNode)
        public void  ReadOxesPhrase(XmlNode node)
        {
            // We want to add the node's data (which will be a phrase) to a DText. If the 
            // last Run isn't a DText, then append an empty one.
            if (0 == Runs.Count || null == Runs[Runs.Count - 1] as DText)
                Runs.Append(new DText());
            var text = Runs[Runs.Count - 1] as DText;
            Debug.Assert(null != text);

            // Let the DText place it where it belongs (e.g., back translation, italic phrase,
            // varnacular text, whatever.
            text.ReadOxesPhrase(node);
        }
        #endregion
        #region Method:  void ReadOxes(nodeParagraph)
        protected void ReadOxes(XmlNode nodeParagraph)
            // Note that DPicture.CreatePicture calls this, too.
        {
            // Style attribute
            string sStyleName = XmlDoc.GetAttrValue(nodeParagraph, c_sAttrStyle, "Paragraph");
            var map = DB.Map.FindMappingFromName(sStyleName);
            if (null == map)
                throw (new XmlDocException("Missing or unknown paragraph style name."));
            StyleAbbrev = map.OurWord;

            // Populate the runs from the child nodes
            foreach (XmlNode child in nodeParagraph.ChildNodes)
            {
                switch (child.Name)
                {
                    case DVerse.c_sNodeTag:
                        Runs.Append(DVerse.Create(child));
                        break;

                    case DChapter.c_sNodeTag:
                        Runs.Append(DChapter.Create(child));
                        break;

                    case DFoot.c_sNodeTag:
                        Runs.Append(DFoot.Create(child));
                        break;

                    case TranslatorNote.c_sTagTranslatorNote:
                    case "TranslatorNote":
                        {
                            var note = TranslatorNote.Create(child);
                            if (null != note)
                            {
                                var dbt = GetOrAddLastDText();
                                dbt.TranslatorNotes.Append(note);
                            }
                        }
                        break;

                    default:
                        ReadOxesPhrase(child);
                        break;
                }
            }

            // Make sure the paragraph is well-formed (e.g., all DTexts should have
            // a BT phrase, even if the input data didn't.
            Cleanup();
        }
        #endregion
        #region SMethod: DParagraph CreateParagraph(nodeParagraph)
        static public DParagraph CreateParagraph(XmlNode nodeParagraph)
        {
            if (null == nodeParagraph || nodeParagraph.Name != c_sTagParagraph)
                return null;

            var p = new DParagraph();
            p.ReadOxes(nodeParagraph);
            return p;
        }
        #endregion

        #region VMethod: XmlNode SaveToOxesBook(oxes, nodeBook, bIncludeID)
        public virtual XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeBook)
            // Saves the paragraph to the oxes document.
        {
            var map = DB.Map.FindMappingFromOurWord(StyleAbbrev);
            Debug.Assert(null != map, "No map for style: " + StyleAbbrev);

            var nodeParagraph = oxes.AddNode(nodeBook, c_sTagParagraph);

            oxes.AddAttr(nodeParagraph, c_sAttrStyle, map.Name);

            if (!string.IsNullOrEmpty(map.Usfm))
                oxes.AddAttr(nodeParagraph, c_sAttrUsfm, map.Usfm);

            foreach (DRun run in Runs)
                run.SaveToOxesBook(oxes, nodeParagraph);

            return nodeParagraph;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region VirtMethod: void Merge(DParagraph Parent, DParagraph Theirs)
        public virtual void Merge(DParagraph Parent, DParagraph Theirs)
        {
            // TODO: For now, we assume no changes in structure. If there are changes, it
            // means we are completely throwing away Theirs. 
            if (StructureCodes != Parent.StructureCodes || Runs.Count != Parent.Runs.Count)
                return;
            if (Theirs.StructureCodes != Parent.StructureCodes || Theirs.Runs.Count != Parent.Runs.Count)
                return;

            for (int i = 0; i < Runs.Count; i++)
            {
                DText tOurs = Runs[i] as DText;
                DText tParent = Parent.Runs[i] as DText;
                DText tTheirs = Theirs.Runs[i] as DText;

                if (null != tOurs && null != tParent && null != tTheirs)
                    tOurs.Merge(tParent, tTheirs);
            }
        }
        #endregion

    }

}
