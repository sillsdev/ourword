/**********************************************************************************************
 * Project: Our Word!
 * File:    DParagraph.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2004
 * Purpose: Handles a paragraph and its related back translation, interlinear BT, etc.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
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

	public class DParagraph : JParagraph
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region JAttr{g}: JOwnSeq Runs - seq of DRun (verse no, chapter no, TextElement)
		public JOwnSeq Runs
		{
			get
			{
				return j_osRuns;
			}
		}
		private JOwnSeq j_osRuns;
		#endregion
		#region Declare BAttrs
		enum BAttrs { bContents = BAttrBase };

		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
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
				Debug.Assert(null != Owner);
				return (DSection)Owner;
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
		public string DebugString
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
				if (G.Map.StyleCrossRef == StyleAbbrev)
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
				return G.StyleSheet.FindParagraphStyle(StyleAbbrev);
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
					if (run.NeedsLeadingSpace)
						s += " ";
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
					if (run.NeedsLeadingSpace)
						s += " ";
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

        // REVISION ===

		#region Method: void AddRun(DRun)
		public void AddRun(DRun run)
		{
			if (null != run)
				Runs.Append(run);
		}
		#endregion

		#region Method: void ConnectFootnote(ref char chLetter, DFootnote footnote)
		public void ConnectFootnote(ref char chLetter, DFootnote footnote)
		{
			foreach(DRun run in Runs)
			{
				DFootLetter letter = run as DFootLetter;
				if (null != letter && null == letter.Footnote)
				{
					letter.Footnote = footnote;
					return;
				}

				DSeeAlso SeeAlso = run as DSeeAlso;
				if (null != SeeAlso && null == SeeAlso.Footnote)
				{
					SeeAlso.Footnote = footnote;
					return;
				}
			}

			// If we get to here, then it means we do not have a marker in the paragraph
			// for this footnote. So we need to append a marker to the end of the
			// paragraph.
			DRun add = null;
			if (footnote.NoteType == DFootnote.Types.kExplanatory)
				add = DFootLetter.Create(chLetter, footnote);
			else
				add = DSeeAlso.Create(chLetter, footnote);

            if (null != add)
            {
                Runs.Append(add);

                // Increment the letter; wrap  back to 'a' if it is going too far
                if (chLetter == 'z')
                    chLetter = 'a';
                else
                    chLetter++;
            }
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
					Runs.InsertAt( i+1, DText.CreateSimple() );
				}
			}

			// Case 2: A paragraph-initial footnote or cross reference, then there should
			// be a place to type prior to it.
			if (Runs.Count > 0)
			{
				if (Runs[0] as DFootLetter != null ||
					Runs[0] as DSeeAlso != null)
				{
					Runs.InsertAt(0, DText.CreateSimple() );
				}
			}

			// Case 3: An empty paragraph should have a place to type
			if (Runs.Count == 0)
			{
				Runs.Append( DText.CreateSimple() );
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
					Runs.InsertAt(iTarget, text);
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
				DFootLetter foot = Runs[i] as DFootLetter;
				DSeeAlso    also = Runs[i] as DSeeAlso;

				bool bDelete = false;

				if (null != foot && null == foot.Footnote)
					bDelete = true;
				if (null != also && null == also.Footnote)
					bDelete = true;

				if (bDelete)
				{
					Runs.RemoveAt(i);
				}
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

			// Set defaults to no leading spaces
			foreach(DRun run in Runs)
			{
				run.NeedsLeadingSpace = false;
			}

			// If we have a DText or a footnote/seealso, and if it is followed
			// by a chapter, verse, or DText, then we insert a space; which we
			// do as a leading space in the chapter, verse or DText.
			for(int i=0; i < Runs.Count - 1; i++)
			{
				// Are we at a text or a footnote?
				if (null == Runs[i] as DText && 
					null == Runs[i] as DFootLetter &&
					null == Runs[i] as DSeeAlso)
				{
					continue;
				}

				// Is the next one a Chapter, Verse or Text?
				DRun run = Runs[i+1] as DRun;
				if (null != run as DChapter)
					run.NeedsLeadingSpace = true;
				else if (null != run as DVerse)
					run.NeedsLeadingSpace = true;
			}

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

		#region Method: void UpdateExegesisNotes( DParagraph pSource)
		public void UpdateExegesisNotes( DParagraph pSource)
		{
			if (pSource.StructureCodes == StructureCodes)
			{
				SynchRunsToModelParagraph(pSource);
				for(int i=0; i < Runs.Count; i++)
				{
					DText t   = Runs[i] as DText;
					DText src = pSource.Runs[i] as DText;

					if (null != t && null != src)
						t.UpdateExegesisNotes(src);
				}
			}
		}
		#endregion

        // Split / Join Paragraphs -----------------------------------------------------------
        #region Method: DParagraph Split(DBasicText textToSplit, int iTextSplitPos)
        #region Implementation Class: SplitParagraphMethod
        class SplitParagraphMethod
        {
            // Helper methods ----------------------------------------------------------------
            #region Method: DParagraph CreateDestinationParagraph(paraToSplit)
            DParagraph CreateDestinationParagraph(DParagraph paraToSplit)
            {
                DSection section = paraToSplit.Section;

                DParagraph paraNew = new DParagraph(paraToSplit.Translation);
                paraNew.StyleAbbrev = paraToSplit.StyleAbbrev;

                int iParaNew = section.Paragraphs.FindObj(paraToSplit) + 1;
                section.Paragraphs.InsertAt(iParaNew, paraNew);

                return paraNew;
            }
            #endregion
            #region Method: DBasicText GetNextBasicText(DBasicText)
            DBasicText GetNextBasicText(DBasicText text)
            {
                bool bFound = false;

                DParagraph paraToSplit = text.Paragraph;

                foreach (DRun run in paraToSplit.Runs)
                {
                    if (run as DBasicText != null && bFound)
                        return run as DBasicText;

                    if (run as DBasicText == text)
                        bFound = true;
                }
                return null;
            }
            #endregion
            #region Method: DPhrase DetermineTargetPhrase(dbtToSplit, int iParaSplitPos)
            DPhrase DetermineTargetPhrase(DBasicText dbtToSplit, int iParaSplitPos)
                // We want to split the phrase in two, and thus we'll point to the beginning of a phrase
                // to be moved. If at a phrase boundary, no action is necessary here.
            {
                // Find the phrase and position within, where the split will occur
                int iPhraseSplitPos = iParaSplitPos;
                DPhrase phraseToMove = null;
                foreach (DPhrase p in dbtToSplit.Phrases)
                {
                    if (p.Text.Length > iPhraseSplitPos)
                    {
                        phraseToMove = p;
                        break;
                    }
                    iPhraseSplitPos -= p.Text.Length;
                }

                // If we are in the midst of a phrase, then split it
                if (null != phraseToMove && iPhraseSplitPos > 0 && iPhraseSplitPos < phraseToMove.Text.Length)
                {
                    phraseToMove = dbtToSplit.Phrases.Split(phraseToMove, iPhraseSplitPos);
                }

                // If we aren't pointing to a phrase, then we must be at the end of one; so 
                // we need to advance to the next DBT
                if (null == phraseToMove)
                {
                    DBasicText dbtNext = GetNextBasicText(dbtToSplit);

                    if (null == dbtNext)
                    {
                        DParagraph para = dbtToSplit.Paragraph;
                        dbtNext = DText.CreateSimple();
                        para.Runs.Append(dbtNext);
                    }

                    phraseToMove = dbtNext.Phrases[0] as DPhrase;
                }

                Debug.Assert(null != phraseToMove);
                return phraseToMove;
            }
            #endregion
            #region Method: DBasicText DetermineTargetText(DPhrase phraseToMove)
            DBasicText DetermineTargetText(DPhrase phraseToMove)
            {
                // Retrieve the text that is the parent of the phrase we're moving
                DBasicText textToSplit = phraseToMove.BasicText;
                Debug.Assert(null != textToSplit);

                // Retrieve its position within the paragraph
                DParagraph paraToSplit = textToSplit.Paragraph;
                int iTextPos = paraToSplit.Runs.FindObj(textToSplit);
                Debug.Assert(-1 != iTextPos);

                // Retrieve the position of the phrase within the parent text
                int iPhrasePos = textToSplit.Phrases.FindObj(phraseToMove);
                Debug.Assert( -1 != iPhrasePos);

                // If it isn't the first position, then we need to split the DText into two
                if (iPhrasePos != 0)
                {
                    DText textRight = new DText();
                    paraToSplit.Runs.InsertAt(iTextPos + 1, textRight);

                    while (textToSplit.Phrases.Count > iPhrasePos)
                    {
                        DPhrase phrase = textToSplit.Phrases[iPhrasePos] as DPhrase;
                        textToSplit.Phrases.Remove(phrase);
                        textRight.Phrases.Append(phrase);
                    }

                    textRight.PhrasesBT.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, ""));

                    textToSplit = textRight;
                }

                return textToSplit;
            }
            #endregion
            #region Method: bool ParagraphHasText(DParagraph)
            bool ParagraphHasText(DParagraph para)
            {
                foreach (DRun run in para.Runs)
                {
                    if (run as DBasicText != null)
                        return true;
                }
                return false;
            }
            #endregion
            #region Method: bool SplitIsLegal(DParagraph p)
            bool SplitIsLegal(DParagraph p)
            {
                string[] vs = new string[] { 
                    DStyleSheet.c_StyleSectionTitle,
                    DStyleSheet.c_StyleAbbrevPictureCaption,
                    DStyleSheet.c_StyleNote,
                    DStyleSheet.c_StyleFootnote
                };

                foreach(string sAbbrev in vs)
                {
                    if (p.StyleAbbrev == sAbbrev)
                        return false;
                }

                return true;
            }
            #endregion

            // Public Interface --------------------------------------------------------------
            #region Constructor()
            public SplitParagraphMethod()
            {
            }
            #endregion
            #region Method: DParagraph Run(DBasicText dbtToSplit, int iParaSplitPos)
            public DParagraph Run(DBasicText dbtToSplit, int iParaSplitPos)
            {
                // Point to the paragraph we're about to split
                DParagraph paraToSplit = dbtToSplit.Paragraph;

                // Make sure the split is allowed
                if (!SplitIsLegal(paraToSplit))
                    return null;

                // Create and insert  a new, empty paragraph to receive the right-side of the split
                DParagraph paraNew = CreateDestinationParagraph(paraToSplit);

                // Determine the target phrase, including splitting it if we're in the middle of it
                DPhrase phraseToMove = DetermineTargetPhrase(dbtToSplit, iParaSplitPos);

                // Determine the target DBasicText, including splitting it if we're in the middle of it
                DBasicText dbtToMove = DetermineTargetText(phraseToMove);

                // Move the phrases to the new paragraph
                int iMove = paraToSplit.Runs.FindObj(dbtToMove);
                Debug.Assert(iMove != -1); // Shouldn't be -1 (not found)
                while (paraToSplit.Runs.Count > iMove)
                {
                    DRun run = paraToSplit.Runs.RemoveAt(iMove) as DRun;
                    Debug.Assert(null != run);
                    paraNew.Runs.Append(run);
                }

                // Boundary condition: if we just moved all of the editable stuff out of the paragraph,
                // then we need to insert something blank and editable
                if (!ParagraphHasText(paraToSplit))
                    paraToSplit.Runs.Append( DText.CreateSimple() );

                // Boundary condition: the new paragraph must have a text as well
                if (!ParagraphHasText(paraNew))
                    paraNew.Runs.InsertAt(0, DText.CreateSimple() );

                // Boundary condition: don't let paraToSplit end in Verse or Chapter
                while (paraToSplit.Runs.Count > 0)
                {
                    int i = paraToSplit.Runs.Count - 1;
                    DRun run = paraToSplit.Runs[i] as DRun;
                    if (run as DVerse == null && run as DChapter == null)
                        break;

                    paraToSplit.Runs.Remove(run);
                    paraNew.Runs.InsertAt(0, run);
                }

                // Return our new paragraph
                return paraNew;
            }
            #endregion
        }
        #endregion
        #region Method: DParagraph Split(DBasicText textToSplit, int iTextSplitPos)
        /// <summary>
        /// Splits the paragraph into two.
        /// </summary>
        /// <param name="textToSplit">The DBasicText within the paragraph that will be split.</param>
        /// <param name="iTextSplitPos">The position within the "textToSplit" where the split will happen.</param>
        public DParagraph Split(DBasicText textToSplit, int iTextSplitPos)
        {
            SplitParagraphMethod m = new SplitParagraphMethod();
            return m.Run(textToSplit, iTextSplitPos);

            #region OBSOLETE SOON - 20feb08 - What I originally wrote in Tepic; replaced by SplitParagraphMethod class
            /***
            // Create a new paragraph and insert it in the DSection to follow the old one
            DParagraph paraNew = new DParagraph(Translation);
            paraNew.StyleAbbrev = StyleAbbrev;
            int iParaNew = Section.Paragraphs.FindObj(this) + 1;
            Section.Paragraphs.InsertAt(iParaNew, paraNew);

            // Find the phrase and position within, where the split will occur
            int iPhraseSplitPos = iTextSplitPos;
            DPhrase phrase = null;
            foreach (DPhrase p in textToSplit.Phrases)
            {
                if (p.Text.Length > iPhraseSplitPos)
                {
                    phrase = p;
                    break;
                }
                iPhraseSplitPos -= p.Text.Length;
            }

            // If we don't have a phrase here, then it means we are asking to split at the
            // end of a phrase. 
            // - If we are at the last editable phrase in the paragraph, then we'll want to
            //     append another DText/DPhrase to the end of the paragraph, so to have 
            //     something to move to the next paragraph.
            // - If we are mid-paragraph, then we want to point to the next available
            //     DPhrase and conduct the split there.
            if (null == phrase)
            {
                DBasicText NextText = paraNew.NextBasicText(textToSplit);

                if (null == NextText)
                {
                    Debug.Assert(textToSplit.Phrases.Count > 0);
                    DPhrase lastPhrase = textToSplit.Phrases[textToSplit.Phrases.Count - 1] as DPhrase;
                    Debug.Assert(null != lastPhrase);
                    phrase = new DPhrase(lastPhrase.CharacterStyleAbbrev, "");
                    textToSplit.Phrases.Append(phrase);
                }
                else
                {
                    textToSplit = NextText;
                    phrase = textToSplit.Phrases[0];
                    iPhraseSplitPos = 0;
                }
            }

            // Set indices to our target text and phrase
            int iText = Runs.FindObj(textToSplit);       // The Run containing this phrase to be moved
            int iPhrase = textToSplit.Phrases.FindObj(phrase);  // The phrase we will move

            // If we are in the middle of a phrase, then it needs to be split into two phrases.
            if (iPhraseSplitPos > 0 && iPhraseSplitPos < phrase.Text.Length)
            {
                textToSplit.Split(phrase, iPhraseSplitPos);
                iPhrase++;
            }

            // If we are in the midst of a DText, then we create a new one
            // to hold the right-phrase and all phrases after it; otherwise
            // the entire DText will be moved together
            if (iPhrase > 0)
            {
                DText textRight = new DText();
                paraNew.Runs.Append(textRight);
                while (textToSplit.Phrases.Count > iPhrase)
                {
                    phrase = textToSplit.Phrases[iPhrase] as DPhrase;
                    textToSplit.Phrases.Remove(phrase);
                    textRight.Phrases.Append(phrase);
                }
                textRight.PhrasesBT.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, ""));
                iText++;
            }

            // Move all of the remaining DRuns to the new paragraph
            while (Runs.Count > iText)
            {
                DRun run = Runs[iText] as DRun;
                Runs.Remove(run);
                paraNew.Runs.Append(run);
            }

            // Boundary condition: both paragraphs must have a DText
            bool bFound = false;
            foreach (DRun r in Runs)
            {
                if (r as DBasicText != null)
                    bFound = true;
            }
            if (!bFound)
                Runs.Append(DText.CreateSimple(""));

            bFound = false;
            foreach (DRun r in paraNew.Runs)
            {
                if (r as DBasicText != null)
                    bFound = true;
            }
            if (!bFound)
                paraNew.Runs.Append(DText.CreateSimple(""));

            // Boundary Condition: Don't leave a chapter or a verse orphaned at the end of a paragraph
            while (Runs.Count > 0)
            {
                int i = Runs.Count - 1;
                DRun run = Runs[i] as DRun;
                if (run as DVerse == null && run as DChapter == null)
                    break;

                Runs.Remove(run);
                paraNew.Runs.InsertAt(0, run);
            }

            return paraNew;
            ***/
            #endregion
        }
        #endregion
        #endregion
        #region Method: void JoinToNext()
        public void JoinToNext()
        {
            // Retrieve the following paragraph
            int iNext = Section.Paragraphs.FindObj(this) + 1;
            if (iNext >= Section.Paragraphs.Count)
                return;
            DParagraph pNext = Section.Paragraphs[iNext] as DParagraph;
            Debug.Assert(null != pNext);

            // Move its runs into this one
            while (pNext.Runs.Count > 0)
            {
                DRun run = pNext.Runs[0] as DRun;
                pNext.Runs.Remove(run);
                this.Runs.Append(run);
            }

            // Remove it from the owner
            Section.Paragraphs.Remove(pNext);

            // Get rid of any spurious spaces, etc.
            //
            // (Note: I wonder if I really need this? I definiteily don't want to call
            // Cleanup, because that can get rid of leading spaces, etc., which then
            // mess up the selection when OW attempts to restore it following
            // the join action. See Bug0281.)
            CombineAdjacentDTexts(false);  // Need to call this first with "false"
        }
        #endregion

        // Copy BT From Front ----------------------------------------------------------------
		#region Method: void CopyBackTranslationsFromFront(DParagraph PFront)
		public bool CopyBackTranslationsFromFront(DParagraph PFront)
            // Returns false if copying is canceled; true otherwise.
		{
			if (Runs.Count != PFront.Runs.Count)
				return true;
			if (TypeCodes != PFront.TypeCodes)
				return true;

            // If the Front's BT and the Target's BT have identical text, then we do nothing
            bool bIsIdentical = true;
            for (int i = 0; i < Runs.Count; i++)
            {
                DRun RTarget = Runs[i] as DRun;
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
            if (HasBackTranslationText && !DialogCopyBTConflict.ApplyToAll)
            {
                DialogCopyBTConflict dlg = new DialogCopyBTConflict(PFront, this);
                if (DialogResult.OK != dlg.ShowDialog())
                    return false;
            }

            // If the user said "Do Nothing", then we're done here
            if (HasBackTranslationText &&
                DialogCopyBTConflict.CopyBTAction == DialogCopyBTConflict.Actions.kKeepExisting)
            {
                return true;
            }

            // Perform the copy (or append, depending)
			for(int i = 0; i < Runs.Count; i++)
			{
				DRun RTarget = Runs[i] as DRun;
				DRun RFront  = PFront.Runs[i] as DRun;

				RTarget.CopyBackTranslationsFromFront(RFront);
			}

			Cleanup();

            return true;
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
		#region Method: DNote GetNoteOfType(DNote.Types type)
		public DNote GetNoteOfType(DNote.Types type)
		{
			DText text = GetOrAddLastDText();

			return text.GetNoteOfType(type);
		}
		#endregion
		#region Attr{g}: DNote[] AllNotes
		public DNote[] AllNotes
		{
			get
			{
				ArrayList list = new ArrayList();

				foreach(DRun run in Runs)
				{
					if (null != run as DText)
					{
						foreach(DNote note in (run as DText).Notes)
							list.Add(note);
					}
				}

				DNote[] v = new DNote[list.Count];

				for(int i=0; i<list.Count; i++)
					v[i] = list[i] as DNote;

				return v;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Method: void CopyFromFront(DParagraph pFront)
		public virtual void CopyFromFront(DParagraph pFront)
		{
			// Same reference
			ChapterI = pFront.ChapterI;
			VerseI   = pFront.VerseI;

			// Same style
			StyleAbbrev = pFront.StyleAbbrev;

			// If a cross reference, then convert it
			if (G.TeamSettings.SFMapping.IsCrossRef( StyleAbbrev ) )
			{
				SimpleText = Translation.ConvertCrossReferences(pFront);
				return;
			}

			// Create the content (copying chapters and verses, blanks for phrases, etc.)
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
				DFootLetter foot = run as DFootLetter;
				if (null != foot)
				{
					DFootnote fnFront = foot.Footnote;
					DFootnote fnTarget = new DFootnote( Translation, fnFront);
					Section.Footnotes.Append(fnTarget);
					Runs.Append( DFootLetter.Copy( foot, fnTarget ) );
					continue;
				}

				// See Also
				DSeeAlso also = run as DSeeAlso;
				if (null != also)
				{
					DFootnote fnFront = also.Footnote;
					DFootnote fnTarget = new DFootnote( Translation, fnFront);
					Section.Footnotes.Append(fnTarget);
					fnTarget.ConvertCrossReferences(fnFront);
					Runs.Append( DSeeAlso.Copy( also, fnTarget ) );
					continue;
				}

				DText textFront = run as DText;
				if (null != textFront)
				{
					Runs.Append( DText.CreateSimple() );
					continue;
				}

				Debug.Assert(false, "A subclass of DRun we don't handle!");
			}
		}
		#endregion

		#region Constructor(DTranslation Trans) 
		public DParagraph(DTranslation Translation)
			: base(Translation.WritingSystemVernacular)
		{
			_ConstructAttrs();
			StyleAbbrev = "";
			_Initialize(null);
		}
		#endregion
		#region Method: void _ConstructAttrs()
		private void _ConstructAttrs()
		{
			j_osRuns = new JOwnSeq("Runs", this, typeof(DRun), false, false);
		}
		#endregion
		#region Method: private void _Initialize() - sets up the attributes, creating objs, etc.
		private void _Initialize(string sStyleAbbrev)
		{
			if (null != sStyleAbbrev)
				StyleAbbrev = sStyleAbbrev;
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
	}

    // Tests ---------------------------------------------------------------------------------
    #region Old-Style TESTs
	public class Test_DParagraph : Test
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attrs (TeamSettings, Project, Translation, Book)
		DTeamSettings TeamSettings = null;
		DProject Project = null;
		DTranslation Translation = null;
		DBook Book = null;
		#endregion
		#region Attr{g}: DSection Section
		DSection Section
		{
			get
			{
				return Book.Sections[0] as DSection;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public Test_DParagraph()
			: base("DParagraph")
		{
			AddTest( new IndividualTest( CombineDTexts ),  
				"Combine DTexts" );

			AddTest( new IndividualTest( BestGuessAtInsertingTextPositions ),
				"Best Guess At Inserting Text Positions" );

			AddTest( new IndividualTest( AsString ),  
				"AsString" );
		}
		#endregion
		#region Method: override void Setup()
		public override void Setup()
		{
			// Team Settings (uses program defaults)
			TeamSettings = new DTeamSettings();
			TeamSettings.InitializeFactoryStyleSheet();

			// Initialize Project1
			Project = new DProject();
			Project.DisplayName = "Test Project";
			Translation = new DTranslation("Test Translation", "Latin", "Latin");
			Project.TargetTranslation = Translation;
			Book = new DBook("MRK", "");
			Translation.AddBook(Book);
		}
		#endregion
		#region Method: override void TearDown()
		public override void TearDown()
		{
			Book = null;
			Translation = null;
			Project = null;
			TeamSettings = null;
		}
		#endregion

		// Tests -----------------------------------------------------------------------------
		#region Test: CombineDTexts
		public void CombineDTexts()
		{
			// Create a paragraph
			DParagraph p = new DParagraph(Translation);

			// Place an initial phrase into the paragraph
			p.SimpleText = "This is a phrase.";
			p.SimpleTextBT = "This is the BT of a phrase.";

			// Add a second phrase
			DText text = DText.CreateSimple();
			text.Phrases[0].Text   = "Appended Phrase.";
			text.PhrasesBT[0].Text = "Appended BT of a phrase.";
			p.AddRun(text);

			// This call should now combine the DTexts, and it should insert a space
			// between them.
			p.Cleanup();

			// Test the result
			AreSame("This is a phrase. Appended Phrase.", p.SimpleText);
			AreSame("This is the BT of a phrase. Appended BT of a phrase.", p.SimpleTextBT);
		}
		#endregion
		#region Test: BestGuessAtInsertingTextPositions
		void BestGuessAtInsertingTextPositions()
		{
			// An empty paragraph should be given a DText
			DParagraph p = new DParagraph(Translation);
			p.BestGuessAtInsertingTextPositions();
			AreSame(1, p.Runs.Count);
			IsNotNull( p.Runs[0] as DText);

			// There should be DText after verses
			p = new DParagraph(Translation);
			p.Runs.Append( DVerse.Create("3") );
			p.Runs.Append( DVerse.Create("4") );
			p.Runs.Append( DSeeAlso.Create('a', new DFootnote(2, 4, Translation)) );
			p.BestGuessAtInsertingTextPositions();
			AreSame(5, p.Runs.Count);
			IsNotNull( p.Runs[0] as DVerse);
			IsNotNull( p.Runs[1] as DText);
			IsNotNull( p.Runs[2] as DVerse);
			IsNotNull( p.Runs[3] as DText);
			IsNotNull( p.Runs[4] as DSeeAlso);

			// There should be a DText before a paragraph-initial footnote
			p = new DParagraph(Translation);
			p.Runs.Append( DSeeAlso.Create('a', new DFootnote(2, 4, Translation)) );
			p.BestGuessAtInsertingTextPositions();
			AreSame(2, p.Runs.Count);
			IsNotNull( p.Runs[0] as DText);
			IsNotNull( p.Runs[1] as DSeeAlso);
		}
		#endregion
		#region Test: AsString
		void AsString()
		{
			// Set up a paragraph
			DParagraph p = new DParagraph(Translation);
			p.AddRun( DChapter.Create("3") );
			p.AddRun( DVerse.Create("1") );
			p.AddRun( DText.CreateSimple("In the beginning was the word.") );
			p.AddRun( DVerse.Create("2") );
			p.AddRun( DText.CreateSimple("And the Word was with God, and the Word was God.") );
			p.AddRun( DVerse.Create("3") );
			p.AddRun( DText.CreateSimple("He was with God in the beginning.") );
			p.Cleanup();  // Determines where leading spaces are needed

			// Did we get what we expect?
			string sExpected = "31In the beginning was the word. 2And the Word " +
				"was with God, and the Word was God. 3He was with God in the " +
				"beginning.";
			AreSame(sExpected, p.AsString);
			//Console.WriteLine("AsString = \"" + p.AsString + "\"");
		}
		#endregion
	}
	#endregion

}
