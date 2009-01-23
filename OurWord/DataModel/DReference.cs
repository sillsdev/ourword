/**********************************************************************************************
 * Project: Our Word!
 * File:    DReference.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles a Bible reference, or a reference span
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
#endregion

namespace OurWord.DataModel
{
	public class DReference : JObject
		// Encapsulates a Bible chapter-verse reference, e.g., 3:16.
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: int Chapter - the number of the chapter (1-based)
		public int Chapter
		{
			get
			{
				return m_nChapter;
			}
			set
			{
                SetValue(ref m_nChapter, value);
			}
		}
		private int m_nChapter = 0;
		#endregion
		#region BAttr{g/s}: int Verse - the number of the verse (1-based)
		public int Verse
		{
			get
			{
				return m_nVerse;
			}
			set
			{
                SetValue(ref m_nVerse, value);
			}
		}
		private int m_nVerse = 0;
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("Chapter", ref m_nChapter);
			DefineAttr("Verse",   ref m_nVerse);
		}
		#endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region Attr{g}: bool IsNotInitialized - T if the Chapter & Verse have not been set
		public bool IsNotInitialized
		{
			get
			{
				return ( 0 == Chapter || 0 == Verse );
			}
		}
		#endregion
		#region Attr{g}: string FullName - returns, e.g., "3:16"
		public string FullName
		{
			get
			{
				return Chapter.ToString() + ":" + Verse.ToString();
			}
		}
		#endregion
		#region Attr{g}: string ParseableName
		public string ParseableName
		{
			get
			{
				return Chapter.ToString("000") + "." + Verse.ToString("000");
			}
		}
		#endregion

		// Setup Methods ---------------------------------------------------------------------
		#region public Constructor() - sets up the object, but Chapter:Verse not yet initialized
		public DReference()
			: base()
		{
		}
		#endregion
		#region public Constructor(nChapter, nVerse) - sets up the object, Chapter:Verse are initialized
		public DReference(int nChapter, int nVerse)
			: base()
		{
			Chapter = nChapter;
			Verse   = nVerse;
		}
		#endregion
		#region Method: void Copy(DReference) - copies a reference's values into this
		public void Copy(DReference reference)
		{
			Chapter = reference.Chapter;
			Verse   = reference.Verse;
		}
		#endregion

		// Parsing Methods -------------------------------------------------------------------
		#region Method: int _ParseFirstNumber(string) - given "2" or "2-4", returns "2"
		private int _ParseFirstNumber(string s)
		{
			string sVerse = "";
			int i = 0;

			while ( i < s.Length && Char.IsDigit(s[i]))
			{
				sVerse += s[i];
				i++;
			}

			Debug.Assert(sVerse.Length > 0);

			return Convert.ToInt16(sVerse);  
		}
		#endregion
		#region Method: int _ParseFinalNumber(string) - given "4" or "2-4", returns "4"
		public int _ParseFinalNumber(string s)
		{
			int nVerse = _ParseFirstNumber(s);
			int nHyphen = s.IndexOf("-");
			if (nHyphen > 0)
			{
				string sLastVerse = s.Substring(nHyphen + 1);
				nVerse = _ParseFirstNumber(sLastVerse);
			}
			return nVerse;
		}
		#endregion
		#region Method: void UpdateVerse(string sVerseNo, bool bFirstInBridge)
		public void UpdateVerse(string sVerseNo, bool bFirst)
		{
			Verse = (bFirst) ? _ParseFirstNumber(sVerseNo) : _ParseFinalNumber(sVerseNo); 
		}
		#endregion
		#region Method: void UpdateChapter(sChapterNo) - sets Chapter to the number in the string
		public void UpdateChapter(string sChapterNo)
		{
			UpdateChapter( Convert.ToInt16( sChapterNo ) );
		}
		#endregion
		#region Method: void UpdateChapter(nChapterNo) - sets Chapter to the number
		public void UpdateChapter(int nChapterNo)
		{
			Chapter = nChapterNo;
			if (0 == Verse)
				Verse = 1;
		}
		#endregion
        #region SMethod: DReference CreateFromParsing(s)
        static public DReference CreateFromParsing(string s)
        {
            s = s.Trim();
            int i = 0;

            // Extract the chapter part
            string sChapter = "";
            while (i < s.Length && s[i] != ':')
                sChapter += s[i++];

            // Skip over the medial puctuation
            if (i < s.Length && s[i] == ':')
                i++;

            // Extract the verse part
            string sVerse = "";
            while (i < s.Length)
                sVerse += s[i++];

            // Convert to integers and create the reference, if possible
            try
            {
                int nChapter = Convert.ToInt16(sChapter);
                int nVerse = Convert.ToInt16(sVerse);
                return new DReference(nChapter, nVerse);
            }
            catch (Exception) {}

            // Not successful, return null
            return null;
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
		#region Method: bool ContentEquals(DReference) - T if the contents are the same
		public bool ContentEquals(DReference rs)
		{
			return Chapter == rs.Chapter && Verse == rs.Verse ;
		}
		#endregion
		#region Method: override operator >=
		public static bool operator >= (DReference rLeft, DReference rRight) 
		{
			if (rLeft.Chapter > rRight.Chapter)
				return true;
			if (rLeft.Chapter == rRight.Chapter && rLeft.Verse >= rRight.Verse)
				return true;
			return false;
		}
		#endregion
		#region Method: override operator <=
		public static bool operator <= (DReference rLeft, DReference rRight) 
		{
			if (rLeft.Chapter < rRight.Chapter)
				return true;
			if (rLeft.Chapter == rRight.Chapter && rLeft.Verse <= rRight.Verse)
				return true;
			return false;
		}
		#endregion
		#region Method: override operator >
		public static bool operator > (DReference rLeft, DReference rRight) 
		{
			if (rLeft.Chapter > rRight.Chapter)
				return true;
			if (rLeft.Chapter == rRight.Chapter && rLeft.Verse > rRight.Verse)
				return true;
			return false;
		}
		#endregion
		#region Method: override operator <
		public static bool operator < (DReference rLeft, DReference rRight) 
		{
			// If the Left Chapter is less, then the entire reference is less.
			if (rLeft.Chapter < rRight.Chapter)
				return true;

			// If the chapters are equal, then the left verse must be less.
			if (rLeft.Chapter == rRight.Chapter && rLeft.Verse < rRight.Verse)
				return true;

			// Everything else means it ain't less.
			return false;
		}
		#endregion
	}

	public class DReferenceSpan : JObject
		// Encapsulates a chapter-verse span, e.g., 3:16-17, or "4:5-5:12"
	{
		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g/s}: DReference Start - the first reference of the span
		public DReference Start
		{
			get 
			{ 
				return j_Start.Value; 
			}
			set
			{
                j_Start.Value.Copy(value);
			}
		}
		private JOwn<DReference> j_Start = null;
		#endregion
		#region JAttr{g/s}: DReference End - the last reference of the span
		public DReference End
		{
			get 
			{ 
				return j_End.Value; 
			}
			set
			{
                j_End.Value.Copy(value);
			}
		}
		private JOwn<DReference> j_End = null;
		#endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region VAttr{g} string DisplayName - returns "3:5-6" or "3:5-6:12"
		public string DisplayName
		{
			get
			{
				string sText = Start.FullName + "-";
				if (Start.Chapter != End.Chapter)
					sText += (End.Chapter.ToString() + ":");
				sText += End.Verse.ToString();
				return sText;
			}
		}
		#endregion
		#region VAttr{g}: string ParseableName
		public string ParseableName
		{
			get
			{
				return Start.ParseableName + "-" + End.ParseableName;
			}
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DReferenceSpan()
			: base()
		{
            j_Start = new JOwn<DReference>("Start", this);
            j_Start.Value = new DReference();

            j_End = new JOwn<DReference>("End", this);
            j_End.Value = new DReference();
		}
		#endregion
		#region Method: bool ContentEquals(DReferenceSpan) - T if the contents are the same
		public bool ContentEquals(DReferenceSpan rs)
		{
			return Start.ContentEquals(rs.Start) && End.ContentEquals(rs.End);
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void UpdateFromLinearRead(DReference) - initializes Start if needed, updates End
		public void UpdateFromLinearRead( DReference reference )
			// The idea is that we are feeding this span one verse at a time, starting at the
			// earliest reference. Thus the first time we call it, we want to set the Start
			// reference (e.g., we just read verse 3:5). Then this time and each subsequent
			// time, we also update End. The eventual result is something like 3:5-3:17.
		{
			if ( Start.IsNotInitialized )
				Start.Copy(reference);
			End.Copy(reference);
		}
		#endregion
		#region Method: bool ContainsReference(DReference) - T if the reference is within this span
		public bool ContainsReference(DReference reference)
		{
			if (Start <= reference && reference <= End)
				return true;
			return false;
		}
		#endregion
		#region Method: void CopyFrom(DReferenceSpan refSpanSource)
		public void CopyFrom(DReferenceSpan refSpanSource)
		{
			Start.Copy(refSpanSource.Start);
			End.Copy(refSpanSource.End);
		}
		#endregion
	}
}
