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
using JWdb;
#endregion

namespace JWdb.DataModel
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
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("NoteType", ref m_nNoteType);
		}
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region Attr{g}: DReference Reference
		public DReference Reference
		{
			get
			{
				return j_reference.Value;
			}
			set
			{
				Debug.Assert(null != value);
				j_reference.Value.Copy(value);
			}
		}
		private JOwn<DReference> j_reference = null;
		#endregion

        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g}: DFoot Foot - Owner
        public DFoot Foot
        {
            get
            {
                Debug.Assert(null != m_Foot);
                return m_Foot;
            }
            set
            {
                m_Foot = value;
            }
        }
        DFoot m_Foot;
        #endregion

        // Static Attrs ----------------------------------------------------------------------
		#region SAttr{g/s}: enum RefLabelTypes RefLabelType - Label (kNone, kStandard (def))
		public enum RefLabelTypes { kNone, kStandard };
		static public RefLabelTypes RefLabelType
		{
			get
			{
				return (RefLabelTypes)m_nRefLabelType;
			}
			set
			{
				m_nRefLabelType = (int)value;
			}
		}
		static private int m_nRefLabelType = (int)RefLabelTypes.kStandard;
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

        // Scaffolding -----------------------------------------------------------------------
        #region private Constructor()
        private DFootnote()
            : base()
        {
            j_reference = new JOwn<DReference>("Reference", this);
            j_reference.Value = new DReference();
        }
        #endregion
        #region Constructor(DFootnote FnFront)
        public DFootnote(DFootnote FnFront)
			: this()
		{
			Reference  = FnFront.Reference;
			NoteType = FnFront.NoteType;
		}
		#endregion
		#region Constructor(nChapter, nVerse, DTranslation)
		public DFootnote(int nChapter, int nVerse, Types nNoteType)
			: this()
		{
			Reference = new DReference(nChapter, nVerse);

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

			// Clear out the Target's data. We'll be rebuilding from scratch
			Runs.Clear();

			// Convert the source
			string sDest = Translation.ConvertCrossReferences( fnFront ); 

			// This becomes the footnote's contents
			SimpleText = sDest;
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

			// If we don't want a label, then there is nothing here to do
			if (RefLabelType == RefLabelTypes.kNone)
				return s;

			// The kStandard option is for a verse reference. We expect a somewhat
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

			return s.Substring(i).Trim();
		}
		#endregion


	}

}