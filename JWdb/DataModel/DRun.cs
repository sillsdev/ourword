/**********************************************************************************************
 * Project: Our Word!
 * File:    DRun.cs
 * Author:  John Wimbish
 * Created: 31 Jan 2005
 * Purpose: A run of Scripture text
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using JWTools;
using JWdb;
#endregion

namespace JWdb.DataModel
{
	// Scripture Text Classes ----------------------------------------------------------------
	#region Class: DRun - Superclass for all of the following subclasses
	public class DRun : JObject
	{
		// Attrs -----------------------------------------------------------------------------
		public const char c_codeVerse      = 'V';
		public const char c_codeChapter    = 'C';
		public const char c_codeFootNote   = 'F';
		public const char c_codeSeeAlso    = 'S';
		public const char c_codeNormal     = '-';
		public const char c_codeInvalid    = 'E';
		public const char c_codeLabel      = 'L';

		public const string c_NoBreakSpace = "\u00A0";
		public const string c_BreakSpace   = "\u2004";

		#region Attr{g}: bool RunIsEditable(char ch)
		static public bool RunIsEditable(char ch)
		{
			if (ch == c_codeNormal)
				return true;
			return false;
		}
		#endregion

        #region VAttr{g}: DSection Section
        public DSection Section
        {
            get
            {
                DParagraph p = Owner as DParagraph;
                if (p == null)
                    return null;
                return p.Section;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DRun()
			: base()
		{
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			//			DRun run = obj as DRun;
			//			if (run.Contents == Contents)
			//				return true;

			return false;
		}
		#endregion

		// Subclasses must override ----------------------------------------------------------
		#region Attr{g}: char TypeCode - a single character representing the subclass type
		public virtual char TypeCode
		{
			get
			{
				return c_codeInvalid;
			}
		}
		#endregion
		#region Attr{g}: virtual string DebugString
		public virtual string DebugString
		{
			get
			{
				return "";
			}
		}
		#endregion
		#region Attr{g}: string ContentsSfmSaveString - should be overridden in the subclasses
		public virtual string ContentsSfmSaveString
		{
			get
			{
				return "subclass has no override of ContentsSfmSaveString.";
			}
		}
		#endregion
		#region Attr{g}: string ProseBTSfmSaveString - should be overridden in the subclasses
		public virtual string ProseBTSfmSaveString
		{
			get
				// By default, we just return the same thing as the Contents string. E.g.,
				// for footnote letters.
			{
				return ContentsSfmSaveString;
			}
		}
		#endregion
		#region Attr{g}: string IntBTSfmSaveString - should be overridden in the subclasses
		public virtual string IntBTSfmSaveString
		{
			get
			{
				return "";
			}
		}
		#endregion
		#region Attr{g}: virtual string AsString
		public virtual string AsString
		{
			get
			{
				return "";
			}
		}
		#endregion
		#region Attr{g}: virtual string ProseBTAsString
		public virtual string ProseBTAsString
		{
			get
			{
				return AsString;
			}
		}
		#endregion

		#region Method: virtual void EliminateSpuriousSpaces()
		public virtual void EliminateSpuriousSpaces()
		{
		}
		#endregion

		#region Method: virtual void ToSfm(ScriptureDB DB)
		public virtual void ToSfm(ScriptureDB DB)
		{
		}
		#endregion

		#region method: virtual PWord[] GetPWords()
		public virtual PWord[] GetPWords()
		{
			Debug.Assert(false);
			return null;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: virtual void CopyBackTranslationsFromFront(DRun RFront)
		public virtual void CopyBackTranslationsFromFront(DRun RFront, bool bReplaceTarget)
		{
		}
		#endregion
	}
	#endregion
	#region Class: DVerse
	public class DVerse : DRun
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g/s}: string Text
		public string Text
		{
			get
			{
				return m_sText;
			}
			set
			{
				m_sText = value;
			}
		}
		string m_sText = "";
		#endregion
		#region Attr{g}: int VerseNo - the numerical portion of the verse Text
		public int VerseNo
		{
			get
			{
				string sDigits = "";
				foreach( char ch in Text)
				{
					if (char.IsDigit(ch))
						sDigits += ch;
					else
						break;
				}
				try
				{
					return Convert.ToInt32(sDigits);
				}
				catch (Exception)
				{
					return -1;
				}
			}
		}
		#endregion
		#region Attr{g}: int VerseNoFinal
		public int VerseNoFinal
			// If we have a verse text of the form 
			//   3f-6a
			// this returns
			//   6
		{
			get
			{
				int i = Text.IndexOf('-');
				if  (-1 == i )
					return VerseNo;

				string sEnd = Text.Substring(i);
				if (sEnd.Length < 2)
					return VerseNo;
				sEnd = sEnd.Substring(1);

				string sDigits = "";
				foreach( char ch in sEnd)
				{
					if (char.IsDigit(ch))
						sDigits += ch;
					else
						break;
				}
				try
				{
					return Convert.ToInt32(sDigits);
				}
				catch (Exception)
				{
					return VerseNo;
				}
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(string sVerseText)
		public DVerse(string sVerseText)
			: base()
		{
			Text = sVerseText;
		}
		#endregion
		#region Method: static DVerse Create(sVerseText)
		static public DVerse Create(string sVerseText)
		{
			// If we are passed an empty Chapter string, we can't do anything
			if (null == sVerseText || 0 == sVerseText.Length)
				return null;

			return new DVerse(sVerseText);

		}
		#endregion
		#region Method: DVerse Copy( DVerse )
		static public DVerse Copy( DVerse verse)
		{
			return new DVerse( verse.Text );
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DVerse verse = obj as DVerse;

			if (Text != verse.Text)
				return false;

			return true;
		}
		#endregion

		// DRun Override Scaffolding ---------------------------------------------------------
		#region Attr{g}: char TypeCode - a single character representing the subclass type
		public override char TypeCode
		{
			get
			{
				return c_codeVerse;
			}
		}
		#endregion
		#region Attr{g}: string DebugString
		public override string DebugString
		{
			get
			{
				return "{v " + Text + "}";
			}
		}
		#endregion
		#region Method: override void EliminateSpuriousSpaces()
		public override void EliminateSpuriousSpaces()
			// There should be no spaces at all within a verse string
		{
			string s = "";

			foreach(char ch in Text)
			{
				if (ch != ' ')
					s += ch;
			}

			Text = s;
		}
		#endregion
		#region method: override PWord[] GetPWords()
		public override PWord[] GetPWords()
		{
			PWord[] v = new PWord[1];
			v[0] = new PWord(Text, 
                DB.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevVerse),
                null);
			return v;
		}
		#endregion
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: override void ToSfm(ScriptureDB DBS)
		public override void ToSfm(ScriptureDB DBS)
		{
			DBS.Append( new SfField( DBS.Map.MkrVerse, Text ) );
		}
		#endregion
	}
	#endregion
	#region Class: DChapter
	public class DChapter : DRun
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g/s}: int ChapterNo
		public int ChapterNo
		{
			get
			{
				return m_nChapterNo;
			}
			set
			{
				m_nChapterNo = value;
			}
		}
		int m_nChapterNo;
		#endregion
		#region Attr{g}: string Text - returns ChapterNo as a string
		public string Text
		{
			get
			{
				return ChapterNo.ToString();
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region protected Constructor(nChapterNo)
		protected DChapter(int nChapterNo)
			: base()
		{
			ChapterNo = nChapterNo;
		}
		#endregion
		#region Method: static DChapter Create(string sContents)
		static public DChapter Create(string sContents)
		{
			// If we are passed an empty Chapter string, we can't do anything
			if (null == sContents || 0 == sContents.Length)
				return null;

			// If the Chapter string cannot be parsed into an integer, or if the number
			// is not valid (e.g., 0), we can't do anything
			try
			{
				// Attempt the conversion
				int nChapter = Convert.ToInt16(sContents);

				// Make sure the chapter number if within range of what we expect in
				// Bible books.
				if (nChapter <= 0)
					return null;
				if (nChapter > 150)
					return null;

				// If we're here, we have passed all tests
				return new DChapter(nChapter);
			}
			catch (Exception)
			{
				return null;
			}
		}
		#endregion
		#region Method: DChapter Copy( DChapter )
		static public DChapter Copy( DChapter chapter )
		{
			return new DChapter( chapter.ChapterNo );
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DChapter chapter = obj as DChapter;

			if (ChapterNo != chapter.ChapterNo)
				return false;

			return true;
		}
		#endregion

		// DRun Override Scaffolding ---------------------------------------------------------
		#region Attr{g}: char TypeCode - a single character representing the subclass type
		public override char TypeCode
		{
			get
			{
				return c_codeChapter;
			}
		}
		#endregion
		#region Attr{g}: string DebugString
		public override string DebugString
		{
			get
			{
				return "{c " + ChapterNo.ToString() + "}";
			}
		}
		#endregion
		#region method: override PWord[] GetPWords()
		public override PWord[] GetPWords()
		{
			PWord[] v = new PWord[1];
            v[0] = new PWord(Text,
                DB.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevChapter),
                null);
			return v;
		}
		#endregion
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: override void ToSfm(ScriptureDB DBS)
		public override void ToSfm(ScriptureDB DBS)
		{
			DBS.Append( new SfField( DBS.Map.MkrChapter, ChapterNo.ToString() ) );
		}
		#endregion

	}
	#endregion

    #region CLass: DFoot
    public class DFoot : DRun
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: char Letter
        public char Letter
        {
            get
            {
                return m_chLetter;
            }
            set
            {
                m_chLetter = value;
            }
        }
        char m_chLetter = 'a';
        #endregion
        #region Attr{g/s}: DFootnote Footnote
        public DFootnote Footnote
        {
            get
            {
                return m_footnote;
            }
            set
            {
                Debug.Assert(null != value);
                m_footnote = value;
            }
        }
        DFootnote m_footnote = null;
        #endregion

        // VAttrs ----------------------------------------------------------------------------
		#region VAttr{g}: string Text - returns Letter as a string
		public string Text
		{
			get
			{
				return Letter.ToString();
			}
		}
		#endregion
        #region VAttr{g}: bool IsExplanatory
        public bool IsExplanatory
        {
            get
            {
                return (TypeCode == c_codeFootNote);
            }
        }
        #endregion
        #region VAttr{g}: bool IsSeeAlso
        public bool IsSeeAlso
        {
            get
            {
                return (TypeCode == c_codeSeeAlso);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(chLetter, DFootnote)
        protected DFoot(char chLetter, DFootnote footnote)
            : base()
        {
			// Make sure we are passed a valid letter
            Debug.Assert(chLetter >= 'a' && chLetter <= 'z');
            m_chLetter = chLetter;

            m_footnote = footnote;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            DFoot foot = obj as DFoot;

            if (Letter != foot.Letter)
                return false;

            return true;
        }
        #endregion

        // DRun Override Scaffolding ---------------------------------------------------------
		#region OAttr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion
        #region OAttr{g}: char TypeCode
        public override char TypeCode
        {
            get
            {
                if (Footnote.NoteType == DFootnote.Types.kExplanatory)
                    return c_codeFootNote;

                if (Footnote.NoteType == DFootnote.Types.kSeeAlso)
                    return c_codeSeeAlso;

                Debug.Assert(false, "Unknown type code.");
                return c_codeFootNote;
            }
        }
        #endregion
        #region Attr{g}: string ContentsSfmSaveString
        public override string ContentsSfmSaveString
        {
            get
            {
                // For an Explanatory footnote, return |fn
                if (IsExplanatory)
                    return "|fn ";

                // For SeeAlso, return nothing
                return "";
            }
        }
        #endregion
        #region Attr{g}: string DebugString
        public override string DebugString
        {
            get
            {
                string sStart = (IsSeeAlso) ? "{cf " : "{fn " ;
                return sStart + Letter.ToString() + "}";
            }
        }
        #endregion
        #region method: override PWord[] GetPWords()
        public override PWord[] GetPWords()
        {
            string sStyle = ((IsSeeAlso) ?
                DStyleSheet.c_StyleAbbrevSeeAlsoLetter :
                DStyleSheet.c_StyleAbbrevFootLetter );

            PWord[] v = new PWord[1];
            v[0] = new PWord(Text,
                DB.StyleSheet.FindCharacterStyle(sStyle),
                null,
                this);
            return v;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region OMethod: void ToSfm(ScriptureDB DBS)
        public override void ToSfm(ScriptureDB DBS)
        {
            // SeeAlso's are simple
            if (IsSeeAlso)
            {
                DBS.Append(new SfField(DBS.Map.MkrSeeAlso, Footnote.SimpleText));
                return;
            }

            // For Explanatory footnotes, append the "|fn" marker to the previous verse text
            SfField LastField = DBS.LastField(DBS.Map.MkrVerseText);
            if (null != LastField)
            {
                LastField.Data += "|fn";
                LastField.BT += "|fn";
            }

            // Build the footnote text from its runs and append it to the DB
            string sContents = "";
            string sProseBT = "";
            foreach (DRun run in Footnote.Runs)
            {
                sContents += run.ContentsSfmSaveString;
                sProseBT += run.ProseBTSfmSaveString;
            }
            DBS.Append(new SfField(DBS.Map.MkrFootnote, sContents,
                sProseBT, ""));
        }
        #endregion
    }
    #endregion
    #region Class: DFootLetter
    public class DFootLetter : DFoot
	{
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(char chLetter, DFootnote footnote)
        protected DFootLetter(char chLetter, DFootnote footnote)
            : base(chLetter, footnote)
		{
		}
		#endregion
		#region Method: static DFootLetter Create(chLetter, DFootnote)
		static public DFootLetter Create(char chLetter, DFootnote footnote)
		{
			return new DFootLetter( chLetter, footnote );
		}
		#endregion
		#region Method: DFootLetter Copy( DFootLetter )
		static public DFootLetter Copy( DFootLetter footletter, DFootnote fn )
		{
			return new DFootLetter( footletter.Letter, fn );
		}
		#endregion

	}
	#endregion
	#region Class: DSeeAlso
    public class DSeeAlso : DFoot
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(char chLetter, DFootnote footnote)
		protected DSeeAlso(char chLetter, DFootnote footnote)
            : base(chLetter, footnote)
		{
		}
		#endregion
		#region Method: static DSeeAlso Create(chLetter, DFootnote)
		static public DSeeAlso Create(char chLetter, DFootnote footnote)
		{
			return new DSeeAlso( chLetter, footnote );
		}
		#endregion
		#region Method: DSeeAlso Copy( DSeeAlso )
		static public DSeeAlso Copy( DSeeAlso also, DFootnote fn )
		{
			return new DSeeAlso( also.Letter, fn );
		}
		#endregion
	}
	#endregion
    /***
    ***/

	#region Class: DLabel
	public class DLabel : DRun
	{
		// Content Attrs ---------------------------------------------------------------------
		#region Attr{g/s}: string Text
		public string Text
		{
			get
			{
				return m_sText;
			}
			set
			{
				m_sText = value;
			}
		}
		string m_sText = "";
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sText)
		public DLabel(string sText)
			: base()
		{
			Text = sText;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DLabel label = obj as DLabel;

			if (Text != label.Text)
				return false;

			return true;
		}
		#endregion

		// DRun Override Scaffolding ---------------------------------------------------------
		#region Attr{g}: char TypeCode - a single character representing the subclass type
		public override char TypeCode
		{
			get
			{
				return c_codeLabel;
			}
		}
		#endregion
		#region Attr{g}: string DebugString
		public override string DebugString
		{
			get
			{
				return "{L" + Text + "}";
			}
		}
		#endregion
		#region Attr{g}: string ContentsSfmSaveString 
		public override string ContentsSfmSaveString
		{
			get
			{
				return Text + " ";
			}
		}
		#endregion
		#region Method: override void EliminateSpuriousSpaces()
		public override void EliminateSpuriousSpaces()
			// There should be no spaces at all within a label
		{
			string s = "";

			foreach(char ch in Text)
			{
				if (ch != ' ')
					s += ch;
			}

			Text = s;
		}
		#endregion

		#region method: override PWord[] GetPWords()
		public override PWord[] GetPWords()
		{
			PWord[] v = new PWord[1];
            v[0] = new PWord(Text,
                DB.StyleSheet.FindCharacterStyle(DStyleSheet.c_StyleAbbrevLabel),
                null);
			return v;
		}
		#endregion
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion

		// Methods ---------------------------------------------------------------------------

	}
	#endregion

	#region Class: DBasicText
	public class DBasicText : DRun
	{
		// Attrs -----------------------------------------------------------------------------
		#region CLASS DPhrases : JOwnSeq - handles typecasting
		public class DPhrases<T> : JOwnSeq<T> where T:DPhrase
		{
			#region Constructor(sName, objOwner)
			public DPhrases(string sName, JObject objOwner)
				: base(sName, objOwner, false, false)
			{
			}
			#endregion
			#region DPhrase Indexer[]
			public new T this [int index]
			{
				get
				{
					return base[index];
				}
				set
				{
					base[index] = value;
				}
			}
			#endregion

            #region VAttr{g}: string AsString
            public string AsString
            {
                get
                {
                    string s = "";
                    foreach (DPhrase p in this)
                        s += p.Text;
                    return s;
                }
            }
            #endregion
            #region VAttr{g}: DPhrase[] AsVector
            public DPhrase[] AsVector
            {
                get
                {
                    DPhrase[] v = new DPhrase[Count];
                    for (int i = 0; i < Count; i++)
                        v[i] = this[i];
                    return v;
                }
            }
            #endregion

            #region Method: char GetCharAt(iPos)
            public char GetCharAt(int iPos)
            {
                return AsString[iPos];
            }
            #endregion
            #region Method: int GetPosInPhrase(iPosInPhrases)
            public int GetPosInPhrase(int iPosInPhrases)
            {
                int iPos = iPosInPhrases;
                foreach (DPhrase phrase in this)
                {
                    if (phrase.Text.Length > iPos)
                        return iPos;
                    iPos -= phrase.Text.Length;
                }
                return iPos;
            }
            #endregion
            #region Method: DPhrase GetPhraseAt(iPos)
            public DPhrase GetPhraseAt(int iPos)
            {
                foreach (DPhrase phrase in this)
                {
                    if (phrase.Text.Length > iPos)
                        return phrase;
                    iPos -= phrase.Text.Length;
                }

                if (iPos == 0 && Count > 0)
                    return this[Count - 1];

                return null;
            }
            #endregion
            #region Method: void CombineSameStyledPhrases()
            public void CombineSameStyledPhrases()
            {
                for (int i = 0; i < Count - 1; )
                {
                    DPhrase left = this[i] as DPhrase;
                    DPhrase right = this[i + 1] as DPhrase;

                    if (left.CharacterStyleAbbrev == right.CharacterStyleAbbrev)
                    {
                        left.Text += right.Text;
                        Remove(right);
                    }
                    else
                        i++;
                }
            }
            #endregion

            #region Method: DPhrase Split(DPhrase phraseToSplit, int iPos)
            /// <summary>
            /// Splits the phrase, iff the position requested is not at a phrase boundary. 
            /// Otherwise, nothing is done.
            /// </summary>
            /// <param name="phraseToSplit">The phrase to be split into two</param>
            /// <param name="iPos">The position within the phrase. It must be greater than
            /// zero, and less than the length of the phrase; otherwise no action is taken.</param>
            public DPhrase Split(DPhrase phraseToSplit, int iPos)
            {
                if (iPos == 0 || iPos == phraseToSplit.Text.Length)
                    return phraseToSplit;

                DPhrase phraseLeft = new DPhrase(phraseToSplit.CharacterStyleAbbrev,
                    phraseToSplit.Text.Substring(0, iPos));

                DPhrase phraseRight = new DPhrase(phraseToSplit.CharacterStyleAbbrev,
                    phraseToSplit.Text.Substring(iPos));

                int iPhrasePosition = FindObj(phraseToSplit);

                InsertAt(iPhrasePosition, phraseRight);
                InsertAt(iPhrasePosition, phraseLeft);
                Remove(phraseToSplit);
                return phraseRight;
            }
            #endregion

            #region Method: void Delete(iStart, iCount)
            #region Helper: void _DeleteChar(iStart)
            void _DeleteChar(int iStart)
            // WARNING: We can't make this public, because it does not join up phrases; so the
            // caller should instead use the general-purpose Delete() method below.
            {
                if (iStart < 0)
                    throw new ArgumentOutOfRangeException("iStart", "In DBasicText.DeleteChar");

                // Locate the phrase and the position with it where the deletion is to start
                int iPhrase = 0;
                DPhrase phrase = this[iPhrase];
                while (iStart >= phrase.Text.Length)
                {
                    iStart -= phrase.Text.Length;
                    iPhrase++;
                    if (iPhrase == Count)
                        throw new ArgumentOutOfRangeException("iStart", "Too long in DBasicText.DeleteChar");
                    phrase = this[iPhrase];
                }

                // Delete the character 
                phrase.Text = phrase.Text.Remove(iStart, 1);

                // If this just created an empty phrase, then we must remove it
                if (phrase.Text.Length == 0)
                    Remove(phrase);

                // If we now have two phrases of the same type, then we must combine them
                for (int i = 0; i < Count - 1; )
                {
                    DPhrase phraseLeft = this[i];
                    DPhrase phraseRight = this[i + 1];

                    if (phraseLeft.CharacterStyleAbbrev == phraseRight.CharacterStyleAbbrev)
                    {
                        phraseLeft.Text += phraseRight.Text;
                        Remove(phraseRight);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            #endregion

            public void Delete(int iStart, int iCount, bool bCleanUpDoubleSpacing)
            {
                // Do the requested deletion of iCount characters
                while (iCount > 0)
                {
                    _DeleteChar(iStart);
                    iCount--;
                }

                // This has the potential to place two spaces together, in which case we
                // must delete the forward one. Thus if we were at "_|e_" and hit delete,
                // we'll now be at "_|_". We do this through a recursive call.
                if (bCleanUpDoubleSpacing)
                {
                    string s = "";
                    foreach (DPhrase p in this)
                        s += p.Text;
                    int iDouble = s.IndexOf("  ");
                    if (iDouble != -1 && iDouble == iStart - 1)
                        _DeleteChar(iStart);
                }

                // If everything is deleted, we should at a minimum have a single, empty DPhrase
                if (Count == 0)
                    Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));
            }

            public void Delete(int iStart, int iCount)
            {
                Delete(iStart, iCount, true);
            }
            #endregion

            #region Method: ToggleItalics(iStart, iCount)
            public void ToggleItalics(int iStart, int iCount)
            {
                // Determine which direction we're taking it. If anything in the selection is
                // not Italic, then we want to make the entire selection Italic
                bool bMakeItalic = false;
                for (int i = iStart; i < iStart + iCount; i++)
                {
                    DPhrase phrase = GetPhraseAt(i);
                    if (!phrase.IsItalic)
                        bMakeItalic = true;
                }

                // Make a new phrase to house the Italics
                string s = AsString.Substring(iStart, iCount);
                string sStyleAbbrev = (bMakeItalic) ?
                    DStyleSheet.c_StyleAbbrevItalic :
                    DStyleSheet.c_sfmParagraph;
                DPhrase phraseItalic = new DPhrase(sStyleAbbrev, s);

                // Remove the corresponding text from the existing DPhrases
                Delete(iStart, iCount, false);

                // If we're at the end, just append the phrase
                if (iStart == AsString.Length)
                {
                    Append(phraseItalic);
                }
                // Otherwise, split and insert
                else
                {
                    // Split the phrase into two so we can insert between them.
                    DPhrase phraseToSplit = GetPhraseAt(iStart);
                    int iPhraseToSplit = FindObj(phraseToSplit);
                    int iPosInPhrase = GetPosInPhrase(iStart);
                    DPhrase phraseRight = Split(phraseToSplit, iPosInPhrase);

                    // If at the beginning of the paragraph, the insert position is the 0th position;
                    // otherwise it is the one after the phrase we just split. (I.e., The split didn't
                    //  happenat the 0th position.)
                    int iInsertPos = FindObj(phraseRight);

                    // Insert our new phrase
                    InsertAt(iInsertPos, phraseItalic);
                }

                CombineSameStyledPhrases();
            }
            #endregion

            // I/O ---------------------------------------------------------------------------
            #region Attr{g}: string ToSaveString
            public string ToSaveString
            {
                get
                {
                    // Start with an empty string
                    string s = "";

                    // Add the phrases
                    foreach (DPhrase phrase in this)
                        s += phrase.SfmSaveString;

                    // Return the result
                    return s;
                }
            }
            #endregion
            #region Method: void FromSaveString(string s)
            public void FromSaveString(string s)
            {
                // We'll build a list of transitional phrase objects here
                List<DSection.IO.Phrase> v = new List<DSection.IO.Phrase>();

                // Parse the string into phrase objects
                int iPos = 0;
                DSection.IO.Phrase p;
                while ((p = DSection.IO.Phrase.GetPhrase(s, ref iPos)) != null)
                    v.Add(p);

                // Create DPhrases from these
                Clear();
                foreach (DSection.IO.Phrase phrase in v)
                {
                    Append(new DPhrase(phrase.StyleAbbrev, phrase.Text));
                }

                // At a minimum we want at least one empty phrase. If "s" was empty
                // when passed in, then we wioll not have this.
                if (this.Count == 0)
                    Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));
            }
            #endregion
        }
        #endregion
        #region JAttr{g}: DPhrases Phrases
        public DPhrases<DPhrase> Phrases
		{
			get { return m_osPhrases; }
		}
		private DPhrases<DPhrase> m_osPhrases;
		#endregion
		#region JAttr{g}: DPhrases PhrasesBT
		public DPhrases<DPhrase> PhrasesBT
		{
			get { return m_osPhrasesBT; }
		}
        private DPhrases<DPhrase> m_osPhrasesBT;
		#endregion

		// Derived / Const attrs -------------------------------------------------------------
		#region Attr{g}: DParagraph Paragraph
		public DParagraph Paragraph
		{
			get
			{
                Debug.Assert(null != Owner as DParagraph);
				return Owner as DParagraph;
			}
		}
		#endregion
		#region Attr{g}: string ContentsAsString
		public string ContentsAsString
		{
			get
			{
				string s = "";

				foreach(DPhrase phrase in Phrases)
				{
					s += phrase.Text;
				}

				return s;
			}
		}
		#endregion
        #region VAttr{g}: int PhrasesLength
        public int PhrasesLength
        {
            get
            {
                int c = 0;

                foreach (DPhrase phrase in Phrases)
                    c += phrase.Text.Length;

                return c;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor() - Creates the attributes
		public DBasicText()
			: base()
		{
			// Construct the two types of phrases
            m_osPhrases = new DPhrases<DPhrase>("Phrase", this);
            m_osPhrasesBT = new DPhrases<DPhrase>("PhraseBT", this);
		}
		#endregion
		#region Constructor(string sText)
		public  DBasicText(string sText)
			: this()
		{
			Phrases.Append(   new DPhrase(null,sText) );
			PhrasesBT.Append( new DPhrase(null,"") );
		}
		#endregion
		#region Constructor(DBasicText)
		public DBasicText( DBasicText source )
			: this()
		{
			foreach(DPhrase p in source.Phrases)
				Phrases.Append( new DPhrase(p) );

			foreach(DPhrase p in source.PhrasesBT)
				PhrasesBT.Append( new DPhrase(p) );
		}
		#endregion
        #region Method: bool PhrasalContentsEquals(DBasicText)
        public bool PhrasalContentsEquals(DBasicText text)
            // Using this method in Merge, in anticipation of when I combine DText and
            // DBasicText into a single class; because at that time, ContentEquals
            // will need to compare owned translator notes, which would mess with the
            // merge logic.
        {
            if (Phrases.Count != text.Phrases.Count)
                return false;

            if (PhrasesBT.Count != text.PhrasesBT.Count)
                return false;

            for (int i = 0; i < Phrases.Count; i++)
            {
                DPhrase phrase1 = Phrases[i] as DPhrase;
                DPhrase phrase2 = text.Phrases[i] as DPhrase;

                if (false == phrase1.ContentEquals(phrase2))
                    return false;
            }

            for (int i = 0; i < PhrasesBT.Count; i++)
            {
                DPhrase phrase1 = PhrasesBT[i] as DPhrase;
                DPhrase phrase2 = text.PhrasesBT[i] as DPhrase;

                if (false == phrase1.ContentEquals(phrase2))
                    return false;
            }

            return true;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DBasicText text = obj as DBasicText;
			if (null == text)
				return false;

			if (Phrases.Count != text.Phrases.Count)
				return false;

			if (PhrasesBT.Count != text.PhrasesBT.Count)
				return false;

			for(int i=0; i < Phrases.Count; i++)
			{
				DPhrase phrase1 = Phrases[i] as DPhrase;
				DPhrase phrase2 = text.Phrases[i] as DPhrase;

				if (false == phrase1.ContentEquals(phrase2))
					return false;
			}

			for(int i=0; i < PhrasesBT.Count; i++)
			{
				DPhrase phrase1 = PhrasesBT[i] as DPhrase;
				DPhrase phrase2 = text.PhrasesBT[i] as DPhrase;

				if (false == phrase1.ContentEquals(phrase2))
					return false;
			}

			return true;
		}
		#endregion
        #region Method: void Cleanup()
        public void Cleanup()
            // Make sure we have at least one phrase
        {
            if (Phrases.Count == 0)
                Phrases.Append(new DPhrase(null, ""));
            if (PhrasesBT.Count == 0)
                PhrasesBT.Append(new DPhrase(null, ""));
        }
        #endregion
        #region Method: void CopyDataTo(DBasicText DBT)
        public void CopyDataTo(DBasicText DBT)
        {
            DBT.Phrases.Clear();
            foreach (DPhrase p in Phrases)
                DBT.Phrases.Append(new DPhrase(p));

            DBT.PhrasesBT.Clear();
            foreach (DPhrase p in PhrasesBT)
                DBT.PhrasesBT.Append(new DPhrase(p));
        }
        #endregion

        // DRun Override Scaffolding ---------------------------------------------------------
		#region Attr{g}: char TypeCode - a single character representing the subclass type
		public override char TypeCode
		{
			get
			{
				return c_codeNormal;
			}
		}
		#endregion
		#region Attr{g}: string DebugString
		public override string DebugString
		{
			get
			{
				string s = "";

				foreach( DPhrase p in Phrases )
					s += p.SfmSaveString;

				string sBT = "";
				foreach( DPhrase p in PhrasesBT )
					sBT += p.SfmSaveString;
				if (sBT.Length > 0)
					s += "{BT " + sBT + "}";

				return s;
			}
		}
		#endregion
		#region Attr{g}: string ContentsSfmSaveString 
		public override string ContentsSfmSaveString
		{
			get
			{
                return Phrases.ToSaveString;
			}
		}
		#endregion
		#region Attr{g}: string ProseBTSfmSaveString
		public override string ProseBTSfmSaveString
		{
			get
			{
                return PhrasesBT.ToSaveString;
			}
		}
		#endregion
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				string s = "";

				foreach(DPhrase p in Phrases)
					s += p.Text;

				return s;
			}
		}
		#endregion
		#region Attr{g}: override string ProseBTAsString
		public override string ProseBTAsString
		{
			get
			{
				string s = "";

				foreach(DPhrase p in PhrasesBT)
					s += p.Text;

				return s;
			}
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: override void EliminateSpuriousSpaces()
		public override void EliminateSpuriousSpaces()
		{
			EliminateSpuriousSpaces(Phrases);
			EliminateSpuriousSpaces(PhrasesBT);
		}
        /// <summary>
        /// Performs various cleanup activities on the sequence of runs: (1) eliminating
        /// double spaces, eliminating leading or trailing spaces for the DText, and
        /// (3) eliminating empty DPhrases.
        /// </summary>
        /// <param name="osPhrases">The sequence of phrases to clean up.</param>
        /// <returns>The number of spaces removed. We need this because this method is
        /// called during the delete command, and the number of spaces helps to determine
        /// where to place the cursor following the deletion.</returns>
		public int EliminateSpuriousSpaces(DPhrases<DPhrase> osPhrases)
		{
            int cSpacesRemoved = 0;

            _RemoveEmptyPhrases(osPhrases);
            _CombinePhrases(osPhrases);

			// Eliminate where one phrase ends with a space and the next begins with one
			for(int i = 0; i < osPhrases.Count - 1; i++)
			{
				DPhrase phrase1 = osPhrases[i]   as DPhrase;
				DPhrase phrase2 = osPhrases[i+1] as DPhrase;

				if (phrase1.Text.Length == 0 || phrase2.Text.Length == 0 )
					continue;

				while (_HasTrailingSpace(phrase1.Text) && _HasLeadingSpace(phrase2.Text))
				{
                    phrase2.Text = _RemoveLeadingSpace(phrase2.Text);
                    cSpacesRemoved++;
				}
			}

			// Make sure the first phrase does not begin with a leading space
			if (osPhrases.Count > 0)
			{
				DPhrase phrase = osPhrases[0] as DPhrase;
                while (_HasLeadingSpace(phrase.Text))
                {
                    phrase.Text = _RemoveLeadingSpace(phrase.Text);
                    cSpacesRemoved++;
                }
			}

			// Make sure the final phrase does not end with a space
			if (osPhrases.Count > 0)
			{
				DPhrase phrase = osPhrases[ osPhrases.Count - 1] as DPhrase;
                while (_HasTrailingSpace(phrase.Text))
                {
                    phrase.Text = _RemoveTrailingSpace(phrase.Text);
                    cSpacesRemoved++;
                }
			}

            // Make sure there is at least one phrase present
            if (osPhrases.Count == 0)
                osPhrases.Append(new DPhrase(null, ""));

            return cSpacesRemoved;
        }
        bool _HasLeadingSpace(string s)
        {
            if (s.Length > 0 && s[0] == ' ')
                return true;
            return false;
        }
        bool _HasTrailingSpace(string s)
        {
            if (s.Length > 0 && s[s.Length - 1] == ' ')
                return true;
            return false;
        }
        string _RemoveLeadingSpace(string s)
        {
            if (_HasLeadingSpace(s))
                return s.Substring(1);
            return s;
        }
        string _RemoveTrailingSpace(string s)
        {
            if (_HasTrailingSpace(s))
                return s.Substring(0, s.Length - 1);
            return s;
        }

        #region Method: void _RemoveEmptyPhrases(DPhrases osPhrases)
        void _RemoveEmptyPhrases(DPhrases<DPhrase> osPhrases)
        {
            for (int i = 0; i < osPhrases.Count; )
            {
                DPhrase phrase = osPhrases[i] as DPhrase;
                if (string.IsNullOrEmpty(phrase.Text))
                    osPhrases.Remove(phrase);
                else
                    i++;
            }
        }
        #endregion
        #region Method: void _CombinePhrases(DPhrases osPhrases)
        void _CombinePhrases(DPhrases<DPhrase> osPhrases)
        {
            for (int i = 0; i < osPhrases.Count - 1; )
            {
                DPhrase phrase1 = osPhrases[i] as DPhrase;
                DPhrase phrase2 = osPhrases[i + 1] as DPhrase;

                if (phrase1.CharacterStyleAbbrev == phrase2.CharacterStyleAbbrev)
                {
                    phrase1.Text += phrase2.Text;
                    osPhrases.Remove(phrase2);
                }
                else
                {
                    i++;
                }
            }
        }
        #endregion
        #endregion
        #region Method: void AppendPhrases(DPhrases osPhrases, DPhrases phrasesToAppend)
        private void AppendPhrases(DPhrases<DPhrase> osPhrases, DPhrases<DPhrase> phrasesToAppend, bool bInsertSpacesBetweenPhrases)
		{
			DPhrase LastPhrase = ( osPhrases.Count > 0 ) ? osPhrases[ osPhrases.Count - 1] : null;

			// Add a space to our last phrase if needed
            if (bInsertSpacesBetweenPhrases)
            {
                if (null != LastPhrase &&
                    !LastPhrase.EndsWithSpace &&
                    phrasesToAppend.Count > 0 &&
                    !phrasesToAppend[0].BeginsWithSpace)
                {
                    osPhrases[osPhrases.Count - 1].Text += " ";
                }
            }

			// Add in the phrases
			while (phrasesToAppend.Count > 0)
			{
				DPhrase phrase = phrasesToAppend[0];
				phrasesToAppend.RemoveAt(0);

                LastPhrase = (osPhrases.Count > 0) ? osPhrases[osPhrases.Count - 1] : null;

				if (null != LastPhrase && phrase.CharacterStyleAbbrev == LastPhrase.CharacterStyleAbbrev)
				{
					LastPhrase.Text += phrase.Text;
				}
				else
				{
					osPhrases.Append(phrase);
				}
			}
		}
		#endregion
		#region Method: void AddParagraph(DBasicText text)
        public virtual void Append(DBasicText text, bool bInsertSpacesBetweenPhrases)
		{
			// AddParagraph the vernacular and BT phrases
            AppendPhrases(Phrases, text.Phrases, bInsertSpacesBetweenPhrases);
            AppendPhrases(PhrasesBT, text.PhrasesBT, bInsertSpacesBetweenPhrases);
		}
		#endregion
		#region Method: void GetWordOffsetPairs(ref aWords, ref aPositions)
		public void GetWordOffsetPairs(ref ArrayList aWords, ref ArrayList aPositions)
			//
			// Given:
			//     0123456789 123456789 123456789 1234567
			//    "The brown fox jumped over the lazy dog."
			//
			// Returns
			//     0  "The"
			//     4  "brown"
			//    10  "fox"
			//    14  "jumped"
			//    21  "over"
			//    26  "the"
			//    30  "lazy"
			//    35  "dog."
		{
			// Make sure the arrays are empty
			aWords.Clear();
			aPositions.Clear();

			// Collect a single string that is the sum of all of the phrases
			string sText = "";
			foreach(DPhrase p in Phrases)
				sText += p.Text;
			sText = sText.Trim();
			int iPos = 0;

			// TODO: Eventually, we should get these from the Language information
			char[] aWordBoundary = new char[1];
			aWordBoundary[0] = ' ';

			while (sText.Length > 0)
			{
				// Determine the end of the next word
				int iPosEnd = sText.IndexOfAny(aWordBoundary);
				if (-1 == iPosEnd)
					iPosEnd = sText.Length;

				// Retrieve that next word, then remove any punctuation and convert it
				// to lower case. If, when we are done, we have a non-zero-length string,
				// then add it to our array.
				string sAdd = sText.Substring(0, iPosEnd).Trim();
				bool bHasNonPunct = false;
				foreach(char ch in sAdd)
				{
					if (!Char.IsPunctuation(ch))
						bHasNonPunct = true;
				}
				if (sAdd.Length > 0 && bHasNonPunct)
				{
					aWords.Add( sAdd );
					aPositions.Add(iPos);
				}

				// Remove the word from our source string, so the loop will look at the
				// next one.
				sText = sText.Substring(iPosEnd);
				iPos += iPosEnd;
				while(sText.Length > 0 && 0 == sText.IndexOfAny(aWordBoundary))
				{
					iPos ++;
					sText = sText.Substring(1);
				}

			}
		}
		#endregion
		#region Method: override PWord[] GetPWords()
		public override PWord[] GetPWords()
		{
			ArrayList al = new ArrayList();
			foreach(DPhrase p in Phrases)
			{
				PWord[] vpw = p.GetPWords();
				if (null != vpw && vpw.Length > 0)
					al.Add( vpw );
			}

			int c = 0;
			foreach( PWord[] vpw in al )
				c += vpw.Length;

			PWord[] v = new PWord[ c ];

			int i = 0;
			foreach( PWord[] vpw in al )
			{
				foreach( PWord pw in vpw )
					v[i++] = pw;
			}

			return v;
		}
		#endregion
		#region Method: override void CopyBackTranslationsFromFront(DRun RFront)
		public override void CopyBackTranslationsFromFront(DRun RFront, bool bReplaceTarget)
		{
            DBasicText FrontText = RFront as DBasicText;

            // Replace Mode means we get rid of existing BT phrases
            if (bReplaceTarget)
                PhrasesBT.Clear();

            // Clear everything if it is blank anyway, and if we're sure we have something to copy
            // (otherwise, we can get stuck with a empty DPhrase at the beginning)
            // Reference Bug0256.
// QUITE POSSIBILY OBSOLETE due to changes made to EliminateSpuriousSpaces
 //           if (string.IsNullOrEmpty(ProseBTAsString) && !string.IsNullOrEmpty(FrontText.ProseBTAsString))
 //               PhrasesBT.Clear();

            // If we have a phrase still in the target (which means we're in kAppendToTarget mode),
            // then add a space to it.
            if (PhrasesBT.Count > 0)
            {
                DPhrase phr = PhrasesBT[PhrasesBT.Count - 1];
                phr.Text += " ";
            }

            // Add the Front phrases
            foreach (DPhrase phraseFront in FrontText.PhrasesBT)
            {
                DPhrase phraseTarget = new DPhrase(phraseFront);
                PhrasesBT.Append(phraseTarget);
            }

            // Eliminate extra spaces this may have created
            EliminateSpuriousSpaces(PhrasesBT);
        }
        #endregion

        #region Method: void Join(int iPhraseLeft)
        /// <summary>
        /// Joins the phrase as iPhraseLeft with the phrase to its right. The resultant
        /// phrase keeps the type of the iPhraseLeft
        /// </summary>
        /// <param name="iPhraseLeft">The index of the left phrase; it will be joined
        /// to the one on its right.</param>
        public void Join(int iPhraseLeft)
        {
            // Make certain a Join operation is supported at this position
            Debug.Assert(iPhraseLeft >= 0);
            Debug.Assert(iPhraseLeft < Phrases.Count - 1);

            // Point to the two phrases in question
            DPhrase phraseLeft = Phrases[iPhraseLeft] as DPhrase;
            DPhrase phraseRight = Phrases[iPhraseLeft + 1] as DPhrase;
            Debug.Assert(null != phraseLeft);
            Debug.Assert(null != phraseRight);

            // Move the contents of the Right phrase into the Left
            phraseLeft.Text += phraseRight.Text;

            // Remove the Right phrase
            Phrases.Remove(phraseRight);
        }
        #endregion
    }
	#endregion
	#region Class: DText
	public class DText : DBasicText
	{
        // Translator Notes ------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq<TranslatorNote> TranslatorNotes
        public JOwnSeq<TranslatorNote> TranslatorNotes
        {
            get
            {
                Debug.Assert(null != m_osTranslatorNotes);
                return m_osTranslatorNotes;
            }
        }
        JOwnSeq<TranslatorNote> m_osTranslatorNotes;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor() - Creates the attributes
		public DText()
			: base()
		{
            // Translator Notes
            m_osTranslatorNotes = new JOwnSeq<TranslatorNote>("tn", this, false, true);
		}
		#endregion
		#region Method: static public DText CreateSimple()
		static public DText CreateSimple()
		{
			return DText.CreateSimple("", "");
		}
		#endregion
		#region Method: static public DText CreateSimple(string sPhraseText)
		static public DText CreateSimple(string sPhraseText)
		{
			return DText.CreateSimple(sPhraseText, "");
		}
		#endregion
		#region Method: static public DText CreateSimple(string sPhraseText, string sPhraseTextBT)
		static public DText CreateSimple(string sPhraseText, string sPhraseTextBT)
		{
			DText text = new DText();
			text.Phrases.Append( new DPhrase( DStyleSheet.c_sfmParagraph, sPhraseText ) );
			text.PhrasesBT.Append( new DPhrase( DStyleSheet.c_sfmParagraph, sPhraseTextBT ) );
			return text;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DText text = obj as DText;
			if (null == text)
				return false;

			return base.ContentEquals(obj);
		}
		#endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void Append(DText text)
        public override void Append(DBasicText basicText, bool bInsertSpacesBetweenPhrases)
		{
			DText text = basicText as DText;
			Debug.Assert(null != text);

			// add the vernacular and BT phrases
            base.Append(text, bInsertSpacesBetweenPhrases);

            // Move the Translator Notes
            // We do this tediously, one-at-a-time, as we must remove from the source
            // (which null's the owner), then add to the destination (which gives it an
            // owner.) 
            while (text.TranslatorNotes.Count > 0)
            {
                TranslatorNote tn = text.TranslatorNotes[0];
                text.TranslatorNotes.Remove(tn);
                TranslatorNotes.Append(tn);
            }
		}
		#endregion
		#region Method: override void ToSfm(ScriptureDB DB)
		public override void ToSfm(ScriptureDB DB)
		{
			DB.Append( new SfField( DB.Map.MkrVerseText, ContentsSfmSaveString, 
				ProseBTSfmSaveString, IntBTSfmSaveString ) );

            // Translator Notes
            foreach (TranslatorNote tn in TranslatorNotes)
            {
                // We're doing this temporary ToSfm thing for now. Later, the new SfField
                // line is all we'll need.
                tn.AddToSfmDB(DB);
            }

		}
		#endregion

        // Merging ---------------------------------------------------------------------------
        #region SMethod: string GetNoteContext(sOurs, sTheirs)
        const int c_nMaxContextLength = 10;
        static public string GetNoteContext(string sOurs, string sTheirs)
        {
            // Scan forward until we get a difference
            int iStart = 0;
            while (iStart < sOurs.Length && 
                iStart < sTheirs.Length &&
                sOurs[iStart] == sTheirs[iStart])
            {
                iStart++;
            }

            // Scan backward from the end until we get a difference
            int iEnd = sOurs.Length - 1;
            int iTheirEnd = sTheirs.Length - 1;
            while( iEnd >= 0 && 
                iTheirEnd >= 0 && 
                sOurs[iEnd] == sTheirs[iTheirEnd])
            {
                iEnd--;
                iTheirEnd--;
            }

            // Everything in-between Start and End is where the differences were found
            string sContext = "";
            for (int i = iStart; i <= iEnd && sContext.Length < c_nMaxContextLength; i++)
                sContext += sOurs[i];

            // We're not wanting to fool with whitespace
            sContext = sContext.Trim();

            // If we have no differences, it is because the changes were appended either
            // to the front or to the end
            if (string.IsNullOrEmpty(sContext))
            {
                // If iStart is zero, it means Theirs was different immediately, indicating
                // a prepend
                if (iStart == 0)
                {
                    for (int k = 0; k < sOurs.Length && k < c_nMaxContextLength; k++)
                        sContext += sOurs[k];
                    if (sContext.Length == c_nMaxContextLength)
                        sContext = sContext.Trim() + "...";
                }

                // If we still have nothing, then extract the end of the string. In some
                // cases this will be correct because iEnd will not have moved, signalling
                // something was post-pended to Theirs.
                if (string.IsNullOrEmpty(sContext))
                {
                    for (int k = sOurs.Length - 1; 
                        k >= 0 && sContext.Length < c_nMaxContextLength; 
                        k--)
                    {
                        sContext = sOurs[k] + sContext;
                    }
                    if (sContext.Length == c_nMaxContextLength)
                        sContext = "..." + sContext.Trim();
                }
            }

            return sContext.Trim();
        }
        #endregion
        #region Method:  void Merge(DBasicText Parent, DBasicText Theirs)
        public void Merge(DBasicText Parent, DBasicText Theirs)
        {
            // If theirs didn't change (which we define as meaning that Theirs is identical
            // to the Parent),  then regardless of whether ours changed or not, we just keep
            // ours; as there is nothing to merge.
            if (Theirs.PhrasalContentsEquals(Parent))
                return;

            // So if we're here, Theirs changed. If ours didn't change, then we just want to
            // overwrite ours with a copy of theirs.
            if (this.PhrasalContentsEquals(Parent))
            {
                Theirs.CopyDataTo(this);
                return;
            }

            // If we're here, then both changed, and we have a Conflict. So we want to keep
            // Ours (as this is less confusing for the user), and generate a TranslatorNote
            // indicating the nature of the conflict.
            TranslatorNote note = new TranslatorNote();
            note.Reference = Section.GetReferenceAt(this).ParseableName;
            note.Context = GetNoteContext(ContentsAsString, Theirs.ContentsAsString);
            // TODO: Back Translation Context as well
            note.Category = TranslatorNote.ToDo;

            // TODO: Assign it to this user? Or, perhaps this should be a merge setting.
            // I guess for now it just stays unassigned.

            Discussion disc = new Discussion(TranslatorNote.MergeAuthor, 
                DateTime.Now, Theirs.ContentsAsString);
            note.Discussions.Append(disc);

            TranslatorNotes.Append(note);
        }
        #endregion
    }
	#endregion

	#region Class: DPhrase
	public class DPhrase : JObject
	{
		// Constants -------------------------------------------------------------------------
		public const char c_chInsertionSpace = '\u2004';   // Unicode's "Four-Per-EM space"
		public const int  c_cInsertedSpaces = 7;           // Number spaces for InsertionIcon
		public const char c_chVerticalBar = '|';           // Sfm for char styles

		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: public string Text - The well-formed contents of the phrase
		public string Text
		{
			get
			{
                return m_sText;
			}
			set
			{
                SetValue(ref m_sText, value);
			}
		}
		string m_sText;
		#endregion
		#region BAttr{g/s}: public string CharacterStyleAbbrev
		public string CharacterStyleAbbrev
		{
			get
			{
				return m_sCharacterStyleAbbrev;
			}
			set
			{
				Debug.Assert(null != value && value.Length > 0);
                SetValue(ref m_sCharacterStyleAbbrev, value);
			}
		}
		string m_sCharacterStyleAbbrev = DStyleSheet.c_sfmParagraph;
		#endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Text", ref m_sText);
            DefineAttr("Style", ref m_sCharacterStyleAbbrev);
        }
        #endregion

        // Derived Attrs ---------------------------------------------------------------------
        #region VAttr{g}: bool IsItalic
        public bool IsItalic
        {
            get
            {
                return (CharacterStyleAbbrev == DStyleSheet.c_StyleAbbrevItalic);
            }
        }
        #endregion
		#region Attr{g}: public string SfmSaveString -  For saving to an SFM file
		public string SfmSaveString
		{
			get
			{
				// Start with an empty string
				string s = "";

				if (CharacterStyleAbbrev != DStyleSheet.c_sfmParagraph)
				{
					s += c_chVerticalBar;
					s += CharacterStyleAbbrev;
				}

				// Output the text, doubling any literals
				foreach(char ch in Text)
				{
					s += ch;

					if (ch == c_chVerticalBar)
						s += ch;
				}

				// Style End
				if (CharacterStyleAbbrev != DStyleSheet.c_sfmParagraph)
				{
					s += c_chVerticalBar;
					s += 'r';
				}

				// Return the result
				return s;
			}
		}
		#endregion
		#region Attr{g}: public bool EndsWithSpace
		public bool EndsWithSpace
		{
			get
			{
				if (Text.Length > 0 && Text[ Text.Length-1 ] == ' ')
					return true;
				return false;
			}
		}
		#endregion
		#region Attr{g}: public bool BeginsWithSpace
		public bool BeginsWithSpace
		{
			get
			{
				if (Text.Length > 0 && Text[0] == ' ')
					return true;
				return false;
			}
		}
		#endregion
        #region VAttr{g}: JCharacterStyle CharacterStyle
        public JCharacterStyle CharacterStyle
        {
            get
            {
                JCharacterStyle cs = DB.StyleSheet.FindCharacterStyleOrNormal(CharacterStyleAbbrev);
                Debug.Assert(null != cs);
                return cs;
            }
        }
        #endregion
        #region VAttr{g}: DBasicText BasicText
        public DBasicText BasicText
        {
            get
            {
                Debug.Assert(null != Owner as DBasicText);
                return Owner as DBasicText;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor() - for xml read
        public DPhrase()
            : base()
        {
        }
        #endregion
        #region Constructor(sStyleAbbrev, sText)
        public DPhrase(string sCharacterStyleAbbrev, string sText)
			: this()
		{
			// Make certain we have a valid Style; use the normal style if the StyleAbbrev
			// passed in is empty, or if it represents a paragraph style.
			if (string.IsNullOrEmpty(sCharacterStyleAbbrev) || 
				false == DB.StyleSheet.IsCharacterStyle(sCharacterStyleAbbrev))
			{
				sCharacterStyleAbbrev = DStyleSheet.c_sfmParagraph;
			}

			// Attributes are now ready to store
			CharacterStyleAbbrev = sCharacterStyleAbbrev;
			Text = sText;
		}
		#endregion
		#region Constructor(DPhrase source)
		public DPhrase( DPhrase source )
            : this()
		{
			CharacterStyleAbbrev = source.CharacterStyleAbbrev;
			Text                 = source.Text;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DPhrase phrase = obj as DPhrase;
			if (phrase.Text == Text && phrase.CharacterStyleAbbrev == CharacterStyleAbbrev)
				return true;

			return false;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: static string CreateInsertionString() - creates the standard blank IP
		static public string CreateInsertionString()
		{
			return new string(c_chInsertionSpace, c_cInsertedSpaces);
		}
		#endregion
		#region Method: static bool IsInsertionString(string s)
		static public bool IsInsertionString(string s)
		{
			if (s.Length != c_cInsertedSpaces)
				return false;

			for(int i=0; i < c_cInsertedSpaces; i++)
			{
				if (s[i] != c_chInsertionSpace )
					return false;
			}

			return true;
		}
		#endregion
		#region SMethod: string EliminateSpuriousSpaces(sSource)
		static public string EliminateSpuriousSpaces(string sSource)
		{
			// Eliminate the special InsertionSpace
			string sOut = "";
			foreach(char ch in sSource)
			{
				if (ch != c_chInsertionSpace)
					sOut += ch;
			}

			// Eliminate double spaces
			int n;
			while( -1 != (n = sOut.IndexOf("  ")) )
			{
				sOut = sOut.Remove(n + 1, 1);
			}

			return sOut;
		}
		#endregion

		#region method: override PWord[] GetPWords()
		public PWord[] GetPWords()
		{
            // Get the true character style. 
            JCharacterStyle cs = CharacterStyle;
            if (cs.Abbrev == DStyleSheet.c_sfmParagraph)
                cs = BasicText.Paragraph.Style.CharacterStyle;

			// Get a working string we can play with
			string s = EliminateSpuriousSpaces(Text);

			// Eliminate any leading or trailing spaces
			s = s.Trim();
			if (s.Length == 0)
				return null;

			// Count the number of spaces.
			int cSpaces = 0;
			foreach(char ch in s)
			{
				if (ch == ' ')
					++cSpaces;
			}

			// The number of words is one greater than the number of spaces
			int cWords = cSpaces + 1;

			// Create the string array to hold the results
			PWord[] v = new PWord[cWords];

			// Go through the string and collect the words
			string sWord = "";
			int i = 0;
			foreach(char ch in s)
			{
				if (ch == ' ')
				{
                    sWord += ch;
                    v[i] = new PWord(sWord, cs, BasicText.Paragraph.Style); 
					i++;
					sWord = "";
				}
				else
					sWord += ch;
			}
            v[i] = new PWord(sWord, cs, BasicText.Paragraph.Style);


			// Return the result
			return v;
		}
		#endregion

	}
	#endregion

    // For printing: hope to move printing to OWWindow and make this obsolete
    #region CLASS: PWord
    public class PWord
    // "PrintWord" At the top level, single word to be printed. But it also
    // includes PWords which might be glued to it, such as footnote letters.
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string Text - the string representing the word
        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }
        string m_Text;
        #endregion
        #region Attr{g}: DRun Footnote - the DRun containing the footnote, or null
        DRun Footnote
        {
            get
            {
                return m_Footnote;
            }
        }
        DRun m_Footnote;
        #endregion

        #region Attr{g}: Font Font
        public Font Font
        {
            get
            {
                Debug.Assert(null != m_Font);
                return m_Font;
            }
        }
        Font m_Font = null;
        #endregion
        #region Attr{g}: Brush TextBrush - retrieves the brush for painting text
        public Brush TextBrush
        {
            get
            {
                return new SolidBrush(m_CStyle.FontColor);
            }
        }
        #endregion
        #region Attr{g/s}: PWord GlueTo
        public PWord GlueTo
        {
            get
            {
                return m_GlueTo;
            }
            set
            {
                m_GlueTo = value;
            }
        }
        PWord m_GlueTo = null;
        #endregion
        #region Attr{g}: string CStyleAbbrev
        public string CStyleAbbrev
        {
            get
            {
                return m_CStyle.Abbrev;
            }
        }
        #endregion
        JCharacterStyle m_CStyle;
        #region Attr{g}: DRun[] FootnoteRuns
        public DRun[] FootnoteRuns
        {
            get
            {
                ArrayList a = new ArrayList();

                if (null != Footnote)
                    a.Add(Footnote);

                if (null != GlueTo)
                {
                    foreach (DRun r in GlueTo.FootnoteRuns)
                        a.Add(r);
                }

                DRun[] v = new DRun[a.Count];

                for (int i = 0; i < a.Count; i++)
                    v[i] = a[i] as DRun;

                return v;
            }
        }
        #endregion
        #region Attr{g}: bool HasFootnotes
        public bool HasFootnotes
        {
            get
            {
                return ((FootnoteRuns.Length > 0) ? true : false);
            }
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Attr{g}: float WidthShrinkage
        float WidthShrinkage
        // The words are too far apart, and I can't figure out why, so
        // I'll arbitrarily subtract a tad. I'll probably need something more
        // complicated than a raw number (e.g., a function of the font size),
        // but for now, I'll try this.
        {
            get
            {
                return 2;
            }
        }
        #endregion
        #region Method: SizeF Measure(Graphics g)
        public SizeF Measure(Graphics g)
        {
            // Measure this word
            StringFormat fmt = StringFormat.GenericTypographic;
            fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            SizeF sz = g.MeasureString(Text, Font, 1000, fmt);
            // Old Way ---> SizeF sz = g.MeasureString(Text, Font);

            // Add the measurements of words that we are glue'd to. This will recurse
            // if the next word(s) are also glued to further words.
            if (null != GlueTo)
            {
                SizeF szGlueTo = GlueTo.Measure(g);
                sz.Width += szGlueTo.Width;
                sz.Height = Math.Max(sz.Height, szGlueTo.Height);
            }

            // Return the result
            return sz;
        }
        #endregion
        #region Method: SizeF MeasureSpace(Graphics g)
        public SizeF MeasureSpace(Graphics g)
        {
            // Get the measurement for a space in this font
            SizeF sz = g.MeasureString(" ", Font);

            // If we are glued to the next word, then we want the measurements of whichever
            // space is in the largest font. (This recurses should other words be glued
            // downstream.
            if (null != GlueTo)
            {
                SizeF szGlueTo = GlueTo.MeasureSpace(g);

                sz.Width = Math.Max(sz.Width, szGlueTo.Width);
                sz.Height = Math.Max(sz.Height, szGlueTo.Height);
            }

            // Return the result
            return sz;
        }
        #endregion
        #region Method: void Draw(Graphics g, float x, float y)
        public void Draw(Graphics g, float x, float y)
        {
            g.DrawString(Text, Font, TextBrush, x, y);

            if (null != GlueTo)
            {
                x += g.MeasureString(Text, Font).Width - WidthShrinkage;
                GlueTo.Draw(g, x, y);
            }
        }
        #endregion
        #region Method: void SetFootnoteLetter(ref char chFootnoteLetter)
        public void SetFootnoteLetter(ref char chFootnoteLetter)
        {
            if (CStyleAbbrev == DStyleSheet.c_StyleAbbrevFootLetter ||
                CStyleAbbrev == DStyleSheet.c_StyleAbbrevSeeAlsoLetter)
            {
                Text = chFootnoteLetter.ToString();
                chFootnoteLetter++;
            }

            if (null != GlueTo)
                GlueTo.SetFootnoteLetter(ref chFootnoteLetter);
        }
        #endregion
        #region Attr{g}: char FirstFootnoteLetter
        public char FirstFootnoteLetter
        {
            get
            {
                if (CStyleAbbrev == DStyleSheet.c_StyleAbbrevFootLetter ||
                    CStyleAbbrev == DStyleSheet.c_StyleAbbrevSeeAlsoLetter)
                {
                    return Text[0];
                }

                if (null != GlueTo)
                    return GlueTo.FirstFootnoteLetter;

                return ' ';
            }
        }
        #endregion

        // Text Replacements -----------------------------------------------------------------
        static public bool ShouldMakeReplacements = false;
        #region SAttr{g}: TreeRoot ReplaceTree
        static public TreeRoot ReplaceTree
        {
            get
            {
                return s_ReplaceTree;
            }
        }
        static TreeRoot s_ReplaceTree = null;
        #endregion
        #region Method: static void BuildReplaceTree()
        static public void BuildReplaceTree()
        {
            if (null != s_ReplaceTree)
                return;

            s_ReplaceTree = new TreeRoot();

            s_ReplaceTree.Add("<<<", "“‘");
            s_ReplaceTree.Add("<<", "“");
            s_ReplaceTree.Add("<", "‘");

            s_ReplaceTree.Add(">>>", "’”");
            s_ReplaceTree.Add(">>", "”");
            s_ReplaceTree.Add(">", "’");
        }
        #endregion
        #region Method: static string MakeReplacements(string s)
        static public string MakeReplacements(string s)
        {
            // Just return the source string if replacements are not desired
            if (!ShouldMakeReplacements)
                return s;

            // Make sure the tree has been built
            if (null == ReplaceTree)
                BuildReplaceTree();
            Debug.Assert(null != ReplaceTree);

            // Do the replacements
            return ReplaceTree.MakeReplacements(s);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Method: _InitializeFont(JCharacterStyle, JParagraphStyle)
        void _InitializeFont(JCharacterStyle _CStyle, JParagraphStyle _PStyle)
        // Set up the Font for the word
        {
            Debug.Assert(null != _CStyle);

            // Determine what, if any, mods are requested
            FontStyle mods = FontStyle.Regular;
            if (string.IsNullOrEmpty(_CStyle.Abbrev))
            {
                _CStyle = _PStyle.CharacterStyle;
            }
            else if (_CStyle.Abbrev == DStyleSheet.c_StyleAbbrevItalic)
            {
                mods = FontStyle.Italic;
                Debug.Assert(null != _PStyle);
                _CStyle = _PStyle.CharacterStyle;
            }
            else if (_CStyle.Abbrev == DStyleSheet.c_StyleAbbrevBold)
            {
                mods = FontStyle.Bold;
                Debug.Assert(null != _PStyle);
                _CStyle = _PStyle.CharacterStyle;
            }

            // Get the font container for the writing system
            JFontForWritingSystem fws = _CStyle.FindOrAddFontForWritingSystem(
                DB.TargetTranslation.WritingSystemVernacular);

            // Adjust for any modifications
            m_Font = fws.FindOrAddFont(false, mods);

            // Adjust for superscript
            if (_CStyle.IsSuperScript)
            {
                float fSize = (float)m_Font.Size * 0.8f;
                m_Font = new Font(m_Font.FontFamily, fSize, m_Font.Style);
            }

            m_CStyle = _CStyle;
        }
        #endregion

        #region Constructor(string _Text, JCharacterStyle, JParagraphStyle)
        public PWord(string _Text, JCharacterStyle _CStyle, JParagraphStyle _PStyle)
        {
            m_Text = _Text;
            Debug.Assert(null != m_Text && m_Text.Length > 0);

            _InitializeFont(_CStyle, _PStyle);

            // Make any replacements
            m_Text = MakeReplacements(m_Text);
        }
        #endregion
        #region Constructor(string _Text, JCharacterStyle, JParagraphStyle, DRun _Footnote)
        public PWord(
            string _Text,
            JCharacterStyle _CStyle,
            JParagraphStyle _PStyle,
            DRun _Footnote)
            : this(_Text, _CStyle, _PStyle) // _CharacterStyleAbbrev)
        {
            Debug.Assert(
                _Footnote as DFootLetter != null ||
                _Footnote as DSeeAlso != null);

            m_Footnote = _Footnote;
        }
        #endregion
    }
    #endregion

}






