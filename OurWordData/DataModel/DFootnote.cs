/**********************************************************************************************
 * Project: Our Word!
 * File:    DFootnote.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles a footnote in Scripture, both the \ft and the \cf styles. 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using OurWordData;
#endregion

namespace OurWordData.DataModel
{
	public class DFootnote : DParagraph
	{		
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: enum Types NoteType - The type of note (kSeeAlso, kExplanatory)
		public enum Types { kSeeAlso, kExplanatory };
		public Types NoteType
		{
			get
			{
				return (Types)m_nNoteType;
			}
			set
			{
				SetValue(ref m_nNoteType, (int)value);

				if ( value == Types.kSeeAlso)
					StyleAbbrev = DB.Map.StyleSeeAlsoPara;
				else
					StyleAbbrev = DB.Map.StyleFootnotePara;
			}
		}
		private int m_nNoteType = (int)Types.kSeeAlso;
		#endregion
        #region BAttr{g/s}: string VerseReference 
        public string VerseReference
        {
            get
            {
                return m_sVerseReference;
            }
            set
            {
                // By default, we'll have a reference; we don't want this to be set
                // to null. (See the constructors, which always set a reference.)
                if (!string.IsNullOrEmpty(value))
                    SetValue(ref m_sVerseReference, value);
            }
        }
        private string m_sVerseReference = "";
        #endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("NoteType", ref m_nNoteType);
            DefineAttr("VerseRef", ref m_sVerseReference);
        }
		#endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region VAttr{g}: override bool IsUserEditable
		public override bool IsUserEditable
		{
			get
			{
				if (NoteType == Types.kSeeAlso)
					return false;
				return true;
			}
		}
		#endregion
        #region VAttr{g}: string Letter - 'a', 'b', ..., 
        public string Letter
        {
            get
            {
                return Foot.Text;
            }
        }
        #endregion
        #region VAttr{g}: bool IsExplanatory
        public bool IsExplanatory
        {
            get
            {
                return (NoteType == Types.kExplanatory);
            }
        }
        #endregion
        #region VAttr{g}: bool IsSeeAlso
        public bool IsSeeAlso
        {
            get
            {
                return (NoteType == Types.kSeeAlso);
            }
        }
        #endregion
        #region VAttr{g}: DFoot Foot - the owner
        DFoot Foot
        {
            get
            {
                return Owner as DFoot;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region private Constructor()
        private DFootnote()
            : base()
        {
        }
        #endregion
        #region Constructor(sVerseReference, kNoteType)
        public DFootnote(string sVerseReference, Types kNoteType)
			: this()
		{
            VerseReference = sVerseReference;
			NoteType = kNoteType;
		}
		#endregion
		#region Constructor(nChapter, nVerse, DTranslation)
		public DFootnote(int nChapter, int nVerse, Types nNoteType)
			: this()
		{
            if (nChapter>0 && nVerse>0)
                VerseReference = nChapter.ToString() + ":" + nVerse.ToString();
            NoteType = nNoteType;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;
			// We're just going to assume we did it correctly in the calling method for now.
			return false;
		}
		#endregion

		#region Method: void ConvertCrossReferences(DFootnote fnFront)
		public void ConvertCrossReferences(DFootnote fnFront)
		{
			Debug.Assert(null != fnFront);

			// Make sure it is the right kind of footnote
			if (NoteType != Types.kSeeAlso)
				return;
			if (fnFront.NoteType != Types.kSeeAlso)
				return;

			// Convert the source
			DTranslation.ConvertCrossReferences( fnFront, this ); 
		}
		#endregion

        #region Method: static string ParseLabel(string s, out string sLabel)
        #region Method: bool _Acts_27_28(string s, int iPos)
        static bool _Acts_27_28(string s, int iPos)
            // From Kupang Malay example in Acts 27:28:
            //
            // Given "\ft 27:28: 20 fathoms is 37 meters", 
            //                  ^
            //                  iPos
            //
            // The "27:28:" will be interpreted as the Label, and
            // the "20 fathoms is 37 meters"" will be the note.
        {
            if (iPos < 0)
                return false;

            if (s[iPos] != ':')
                return false;

            for (int i = iPos - 1; i >= 0; i--)
            {
                if (s[i] == ':')
                    return true;

                if (s[i] == ';')
                    return false;
            }

            return false;
        }
        #endregion
        static public string ParseLabel(string s, out string sLabel)
		{
			sLabel = "";

			// We want to extract the verse reference. We expect a somewhat
			// unpredictable sequence of numbers and certain types of punctuation. 
			int i = 0;
			string sPunctChars = ":;.-";
			for(; i < s.Length; i++)
			{
                // Shorthand: categorize the current/previous characters
				bool bIsDigit = char.IsDigit(s, i);
				bool bIsPunct = sPunctChars.IndexOf(s[i]) != -1;
				bool bIsSpace = char.IsWhiteSpace(s, i);
				bool bIsLower = char.IsLower(s, i);
				bool bPrevIsDigit = ( i>0 && char.IsDigit(s, i-1) );

                // A boundary beside-which a letter (e.g., '9a') is part of the reference
				bool bNextIsBoundary = false;
				if (i == s.Length-1)
					bNextIsBoundary = true;
				if ( i < s.Length-1 && i>0 && 
					(char.IsPunctuation(s, i+1) || char.IsWhiteSpace(s, i+1)) )
				{
					bNextIsBoundary = true;
				}

                // Is it a letter which is part of a reference (e.g., '9a')
				bool bIsLetter = false;
				if (bPrevIsDigit && bIsLower && bNextIsBoundary )
					bIsLetter = true;

                // Handle Kupang Acts: "27:28: 20 fathoms is 37 meters"; we want
                // to exit the loop prior to adding to the sLabel if we've reached
                // this condition.
                if (_Acts_27_28(s, i - 1))
                    break;

                // If is a reference part, then add it and we'll increment
				if (bIsDigit || bIsPunct || bIsLetter)
					sLabel += s[i];

                // If not a reference part, then we're done
				if (!bIsDigit && !bIsPunct && !bIsLetter & !bIsSpace)
					break;
			}

            // Remove the trailing colon; we'll add that programatically
            if (!string.IsNullOrEmpty(sLabel))
            {
                int k = sLabel.Length - 1;
                if(sLabel[k] == ':' || sLabel[k] == ';')
                    sLabel = sLabel.Substring(0, sLabel.Length - 1);
            }

			return s.Substring(i).Trim();
		}
		#endregion
	}

}
