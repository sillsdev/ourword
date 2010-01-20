#region ***** DRun.cs *****
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
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

using JWTools;
using OurWordData;
using OurWordData.Styles;
using OurWordData.Tools;
using OurWordData.DataModel.Runs;
#endregion
#endregion

namespace OurWordData.DataModel
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
        #region VMethod: DRun ConvertCrossRefs(bsaSourceSubs, bsaDestSubs)
        public virtual DRun ConvertCrossRefs(
            BStringArray bsaSourceSubstitutions, 
            BStringArray bsaDestSubstitutions)
        {
            return null;
        }
        #endregion

        #region virtual XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParent)
        public virtual XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParent)
        {
            return null;
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
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion

        // Oxes ------------------------------------------------------------------------------
        public const string c_sNodeTag = "v";
        const string c_sAttrText = "n";
        #region SMethod: DVerse Create(XmlNode node)
        static public DVerse Create(XmlNode node)
        {
            if (node.Name == c_sNodeTag)
            {
                string sText = XmlDoc.GetAttrValue(node, c_sAttrText, "");
                if (string.IsNullOrEmpty(sText))
                    throw new XmlDocException(node, "Missing verse number");
                return new DVerse(sText);
            }
            return null;
        }
        #endregion
        #region OMethod: XmlNode SaveToOxesBook(oxes, nodeParent)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParent)
        {
            var node = oxes.AddNode(nodeParent, c_sNodeTag);
            oxes.AddAttr(node, c_sAttrText, Text);
            return node;
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
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion

        // Oxes ------------------------------------------------------------------------------
        public const string c_sNodeTag = "c";
        const string c_sAttrText = "n";
        #region SMethod: DChapter Create(XmlNode node)
        static public DChapter Create(XmlNode node)
        {
            if (node.Name == c_sNodeTag)
            {
                int nChapterNo = XmlDoc.GetAttrValue(node, c_sAttrText, 0);
                if (0 == nChapterNo)
                    throw new XmlDocException(node, "Bad or missing chapter number in oxes data.");

                return new DChapter(nChapterNo);
            }
            return null;
        }
        #endregion
        #region OMethod: XmlNode SaveToOxesBook(oxes, nodeParent)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParent)
        {
            var node = oxes.AddNode(nodeParent, c_sNodeTag);
            oxes.AddAttr(node, c_sAttrText, Text);
            return node;
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
        // ZAttrs ----------------------------------------------------------------------------
        #region JAttr{g/s}: DFootnote Footnote
        public DFootnote Footnote
        {
            get
            {
                return j_Footnote.Value;
            }
            set
            {
                j_Footnote.Value = value;
            }
        }
        JOwn<DFootnote> j_Footnote = null;
        #endregion

        // Footnote Letter -------------------------------------------------------------------
        public enum FootnoteSequenceTypes { abc=0, iv, custom };
        #region SAttr{g}: string[] FootnoteSequenceChoices
        static public string[] FootnoteSequenceChoices
        {
            get
            {
                string[] vs = new string[3];
                vs[0] = TypeToString(FootnoteSequenceTypes.abc);
                vs[1] = TypeToString(FootnoteSequenceTypes.iv);
                vs[2] = TypeToString(FootnoteSequenceTypes.custom);
                return vs;
            }
        }
        #endregion
        const string c_abc = "a, b, c, ..., z";
        const string c_iv = "i, ii, iii, iv, ...";
        const string c_Custom = "Custom";
        #region SMethod: string TypeToString(FootnoteSequenceTypes)
        static public string TypeToString(FootnoteSequenceTypes nType)
        {
            switch (nType)
            {
                case FootnoteSequenceTypes.abc:
                    return c_abc;
                case FootnoteSequenceTypes.iv:
                    return c_iv;
                case FootnoteSequenceTypes.custom:
                    return Loc.GetString("kFootnoteCustom", c_Custom);
                default:
                    Debug.Assert(false, "Unknown FootnoteSequenceType");
                    return "*";
            }
        }
        #endregion
        #region SMethod: FootnoteSequenceTypes TypeFromString(s)
        static public FootnoteSequenceTypes TypeFromString(string s)
        {
            if (s == c_abc)
                return FootnoteSequenceTypes.abc;
            if (s == c_iv)
                return FootnoteSequenceTypes.iv;
            return FootnoteSequenceTypes.custom;
        }
        #endregion

        #region SMethod: string GetFootnoteLetter_abc(n)
        static public string GetFootnoteLetter_abc(int n)
        {
            Debug.Assert(n >= 0);

            while (n >= 26)
                n -= 26;

            return ((char)((int)'a' + n)).ToString();
        }
        #endregion
        #region SMethod: string GetFootnoteLetter_iv(n)
        static public string GetFootnoteLetter_iv(int n)
            // For now, we just cycle through 30 over and over
        {
            string[] vRoman = 
            { 
                "i",   "ii",   "iii",   "iv",   "v",   "vi",   "vii",   "viii",   "ix",   "x",
                "xi",  "xii",  "xiii",  "xiv",  "xv",  "xvi",  "xvii",  "xviii",  "xix",  "xx",
                "xxi", "xxii", "xxiii", "xxiv", "xxv", "xxvi", "xxvii", "xxviii", "xxix", "xxx"
            };

            while (n >= vRoman.Length)
                n -= vRoman.Length;

            return vRoman[n];
        }
        #endregion
        #region SMethod: string GetFootnoteLetter_bsa(n, bsa)
        public static string GetFootnoteLetter_bsa(int n, BStringArray bsa)
        {
            // Debug version: make sure we have a valid bsa; Runtime version, return something
            // useful.
            Debug.Assert(null != bsa);
            Debug.Assert(bsa.Length > 0);
            if (null == bsa || bsa.Length == 0)
                return "*";

            // If all they want is, e.g., an asterix everywhere, then we just give it to them
            if (bsa.Length == 1)
            {
                if (string.IsNullOrEmpty(bsa[0].Trim()))
                    return "*";
                return bsa[0];
            }

            // By reducing n, we in effect just have it roll over, e..g, from 'z' to 'a'.
            while (n >= bsa.Length)
                n -= bsa.Length;

            return bsa[n];
        }
        #endregion
        #region Method: string GetFootnoteLetterFor(int n)
        public string GetFootnoteLetterFor(int n)
        {
            var translation = Section.Translation;

            switch (translation.FootnoteSequenceType)
            {
                case FootnoteSequenceTypes.abc:
                    return GetFootnoteLetter_abc(n);

                case FootnoteSequenceTypes.iv:
                    return GetFootnoteLetter_iv(n);

                case FootnoteSequenceTypes.custom:
                    return GetFootnoteLetter_bsa(n, translation.FootnoteCustomSeq);

                default:
                    Debug.Assert(false, "Unsupported FootnoteSequenceType");
                    break;
            }

            return "*";
        }
        #endregion

        // VAttrs ----------------------------------------------------------------------------
		#region VAttr{g}: string Text - returns the callout letter for the footnote
		public string Text
		{
			get
			{
                // Get the owning section
                DSection section = Section;
                Debug.Assert(null != Section);

                // Get this footnote's position within the owning section
                int iPos = -1;
                foreach (DParagraph p in Section.Paragraphs)
                {
                    foreach (DRun r in p.Runs)
                    {
                        DFoot foot = r as DFoot;

                        if (null != foot)
                            iPos++;

                        if (foot == this)
                            goto found;
                    }
                }
            found:
                Debug.Assert(-1 != iPos, "Footnote wasn't found in its owning section!");

                // Return the proper letter, depending on the type in the DTranslation
                return GetFootnoteLetterFor(iPos);
            }
		}
		#endregion
        #region VAttr{g}: bool IsExplanatory
        public bool IsExplanatory
        {
            get
            {
                return Footnote.IsExplanatory;
            }
        }
        #endregion
        #region VAttr{g}: bool IsSeeAlso
        public bool IsSeeAlso
        {
            get
            {
                return Footnote.IsSeeAlso;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DFootnote)
        public DFoot(DFootnote footnote)
            : base()
        {
            j_Footnote = new JOwn<DFootnote>("Footnote", this);
            j_Footnote.Value = footnote;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            DFoot foot = obj as DFoot;

            if (Text != foot.Text)
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
                if (Footnote.IsExplanatory)
                    return c_codeFootNote;

                if (Footnote.IsSeeAlso)
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
                return sStart + Text + "}";
            }
        }
        #endregion

        // Oxes ------------------------------------------------------------------------------
        public const string c_sNodeTag = "note";
        const string c_sAttrVerseRef = "reference";
        const string c_sAttrStyle = "class";
        const string c_sAttrUsfm = "usfm";
        #region SMethod: DFoot Create(XmlNode node)
        static public DFoot Create(XmlNode node)
        {
            if (node.Name != c_sNodeTag)
                return null;

            // Retrieve the attributes; complain if missing or empty if appropriate
            string sVerseReference = XmlDoc.GetAttrValue(node, c_sAttrVerseRef, "");

            string sStyle = XmlDoc.GetAttrValue(node, c_sAttrStyle, "");
            if (string.IsNullOrEmpty(sStyle))
                throw new XmlDocException(node, "Missing Style attribute in oxes read.");

            string sUsfm = XmlDoc.GetAttrValue(node, c_sAttrUsfm, "");
            if (string.IsNullOrEmpty(sUsfm))
                throw new XmlDocException(node, "Missing UsfmCode attribute in oxes read.");

            // The UsfmCode and the Style should match up; if they don't, we hardly know which
            // one the user wanted.
            var map = DB.Map.FindMappingFromUsfm(sUsfm);
            if (null == map || map.StyleName != sStyle)
                throw new XmlDocException(node, "Style and UsfmCode data do not match.");

            // Create the empty footnote, and place it into a DFoot object
            var footnote = new DFootnote(
                sVerseReference, 
                ((sUsfm == "x") ? DFootnote.Types.kSeeAlso : DFootnote.Types.kExplanatory));
            var foot = new DFoot(footnote);

            // Buld the footnote content
            foreach (XmlNode child in node.ChildNodes)
                footnote.ReadOxesPhrase(child);

            // Make sure we are properly formed, e.g., at least one phrase in the BT
            footnote.Cleanup();

            return foot;
        }
        #endregion
        #region OMethod: XmlNode SaveToOxesBook(oxes, nodeParent)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParent)
        {
            var node = oxes.AddNode(nodeParent, c_sNodeTag);

            if (!string.IsNullOrEmpty(Footnote.VerseReference))
                oxes.AddAttr(node, c_sAttrVerseRef, Footnote.VerseReference);

            oxes.AddAttr(node, c_sAttrStyle,
                IsSeeAlso ? "Note Cross Reference Paragraph" : "Note General Paragraph");

            oxes.AddAttr(node, c_sAttrUsfm,
                IsSeeAlso ? "x" : "f");

            foreach (DRun run in Footnote.Runs)
                run.SaveToOxesBook(oxes, node);

            return node;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region OMethod: void ToSfm(ScriptureDB DBS)
        public override void ToSfm(ScriptureDB DBS)
        {
            // SeeAlso's are simple
            if (IsSeeAlso)
            {
                string sText = "";
                if (!string.IsNullOrEmpty(Footnote.VerseReference))
                    sText = Footnote.VerseReference + ": ";
                sText += Footnote.SimpleText;

                DBS.Append(new SfField(DBS.Map.MkrSeeAlso, sText));
                return;
            }

            // Find the \vt field to append to
            SfField VTField = null;
            for (int i = DBS.Count - 1; i >= 0; i--)
            {
                SfField f = DBS.Fields[i] as SfField;

                if (f.Mkr == DBS.Map.MkrVerseText)
                {
                    VTField = f;
                    break;
                }

                if (DBS.Map.IsVernacularParagraph(f.Mkr) || f.Mkr == DBS.Map.MkrVerse)
                {
                    VTField = DBS.InsertAt(i+1, new SfField(DBS.Map.MkrVerseText));
                    break;
                }
            }
            if (null == VTField)
                VTField = DBS.Append(new SfField(DBS.Map.MkrVerseText));
            Debug.Assert(null != VTField && DBS.Map.MkrVerseText == VTField.Mkr);

            // Add the markers
            VTField.Data += "|fn";
            VTField.BT += "|fn";

            // Build the footnote text from its runs and append it to the DB
            string sContents = "";
            string sProseBT = "";
            if (!string.IsNullOrEmpty(Footnote.VerseReference))
            {
                sContents += Footnote.VerseReference + ": ";
                sProseBT += Footnote.VerseReference + ": ";
            }
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
		#region OMethod: void EliminateSpuriousSpaces()
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
		#region Attr{g}: override string AsString
		public override string AsString
		{
			get
			{
				return Text;
			}
		}
		#endregion

	}
	#endregion

	#region Class: DBasicText
	public class DBasicText : DRun
	{
		// Attrs -----------------------------------------------------------------------------
        #region JAttr{g}: DPhrases Phrases
        public DPhraseList<DPhrase> Phrases
		{
			get { return m_osPhrases; }
		}
		private readonly DPhraseList<DPhrase> m_osPhrases;
		#endregion
		#region JAttr{g}: DPhrases PhrasesBT
        public DPhraseList<DPhrase> PhrasesBT
		{
			get { return m_osPhrasesBT; }
		}
        private readonly DPhraseList<DPhrase> m_osPhrasesBT;
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
            m_osPhrases = new DPhraseList<DPhrase>("Phrase", this);
            m_osPhrasesBT = new DPhraseList<DPhrase>("PhraseBT", this);
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
                Phrases.Append(new DPhrase(""));
            if (PhrasesBT.Count == 0)
                PhrasesBT.Append(new DPhrase(""));
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
			    return Phrases.AsString;
			}
		}
		#endregion
		#region Attr{g}: override string ProseBTAsString
		public override string ProseBTAsString
		{
			get
			{
			    return PhrasesBT.AsString;
			}
		}
		#endregion
        #region OMethod: DRun ConvertCrossRefs(bsaSourceSubs, bsaDestSubs)
        public override DRun ConvertCrossRefs(
            BStringArray bsaSourceSubstitutions,
            BStringArray bsaDestSubstitutions)
        {
            // We assume a simple, one-phrase text
            string sSource = "";
            foreach (DPhrase phr in Phrases)
                sSource += phr.Text;

            // Create the converted string
            int i = 0;
            string sDest = "";

            // Loop through the source string, comparing for matches
            while (i < sSource.Length)
            {
                // Look for a match from amongst the book names
                int iBookName = bsaSourceSubstitutions.FindSubstringMatch(sSource, i, true);

                // If not found, add the current character and move on to the next one
                if (-1 == iBookName)
                {
                    sDest += sSource[i++];
                    continue;
                }

                // Else it was found; so make the substitution.
                sDest += bsaDestSubstitutions[iBookName];
                i += bsaSourceSubstitutions[iBookName].Length;
            }

            return DText.CreateSimple(sDest);
        }
        #endregion

        // Oxes ------------------------------------------------------------------------------
        const string c_sAttrBackTranslation = "bt";
        #region Method: void ReadOxesPhrase(XmlNode node)
        public void ReadOxesPhrase(XmlNode node)
            // With well-formed data, we always expect to be adding a phrase here, as opposed
            // to appending data to an existing phrase.
        {
            // Back translation
            if (node.Name == c_sAttrBackTranslation)
            {
                foreach (XmlNode child in node.ChildNodes)
                    PhrasesBT.ReadOxesPhrase(child);
                return;
            }

            // Vernacular text
            Phrases.ReadOxesPhrase(node);
        }
        #endregion
        #region OMethod: XmlNode SaveToOxesBook(oxes, nodeParagraph)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParagraph)
        {
            Phrases.SaveToOxesBook(oxes, nodeParagraph);

            if (PhrasesBT.Count > 0 && !string.IsNullOrEmpty(ProseBTAsString))
            {
                var nodeBT = oxes.AddNode(nodeParagraph, c_sAttrBackTranslation);
                PhrasesBT.SaveToOxesBook(oxes, nodeBT);
            }

            return nodeParagraph;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
		#region Method: override void EliminateSpuriousSpaces()
		public override void EliminateSpuriousSpaces()
		{
			Phrases.EliminateSpuriousSpaces();
			PhrasesBT.EliminateSpuriousSpaces();
		}
        #endregion
        #region Method: void Append(DBasicText text)
        public virtual void Append(DBasicText text, bool bInsertSpacesBetweenPhrases)
		{
			// AddParagraph the vernacular and BT phrases
            Phrases.AppendPhrases(text.Phrases, bInsertSpacesBetweenPhrases);
            PhrasesBT.AppendPhrases(text.PhrasesBT, bInsertSpacesBetweenPhrases);
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
            PhrasesBT.EliminateSpuriousSpaces();
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
	    readonly JOwnSeq<TranslatorNote> m_osTranslatorNotes;
        #endregion

        // Oxes ------------------------------------------------------------------------------
        #region OMethod: XmlNode SaveToOxesBook(XmlDoc, nodeParagraph)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParagraph)
        {
            // Save the phrase data
            base.SaveToOxesBook(oxes, nodeParagraph);

            // Save the notes
            foreach (TranslatorNote tn in TranslatorNotes)
                tn.Save(oxes, nodeParagraph);

            return nodeParagraph;
        }
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
			var text = new DText();
			text.Phrases.Append( new DPhrase(sPhraseText ) );
			text.PhrasesBT.Append( new DPhrase(sPhraseTextBT ) );
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
            var iStart = 0;
            while (iStart < sOurs.Length && 
                iStart < sTheirs.Length &&
                sOurs[iStart] == sTheirs[iStart])
            {
                iStart++;
            }

            // Scan backward from the end until we get a difference
            var iEnd = sOurs.Length - 1;
            var iTheirEnd = sTheirs.Length - 1;
            while( iEnd >= 0 && 
                iTheirEnd >= 0 && 
                sOurs[iEnd] == sTheirs[iTheirEnd])
            {
                iEnd--;
                iTheirEnd--;
            }

            // Everything in-between Start and End is where the differences were found
            var sContext = "";
            for (var i = iStart; i <= iEnd && sContext.Length < c_nMaxContextLength; i++)
                sContext += sOurs[i];

            // We're not wanting to fool with whitespace
            sContext = sContext.Trim();

            // If Theirs is empty, then all of Ours is different. We want an elipses if
            // it is longer than what we just extracted
            if (string.IsNullOrEmpty(sTheirs))
            {
                if (sOurs.Length > c_nMaxContextLength)
                    sContext += "...";
                return sContext;
            }

            // If we have no differences, it is because the changes were appended either
            // to the front or to the end
            if (string.IsNullOrEmpty(sContext))
            {
                // If iStart is zero, it means Theirs was different immediately, indicating
                // a prepend
                if (iStart == 0)
                {
                    for (var k = 0; k < sOurs.Length && k < c_nMaxContextLength; k++)
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
        #region SMethod: string GetConflictMergeNoteContents(sParent, sOurs, sTheirs)
        static public string GetConflictMergeNoteContents(string sParent, string sOurs, string sTheirs)
        {
            var sParentChanged = sParent;
            var sOursChanged = sOurs;
            var sTheirsChanged = sTheirs;

            const int minimumLength = 12;

            // Remove what they have in common at their beginning
            while (!string.IsNullOrEmpty(sParentChanged) && 
                !string.IsNullOrEmpty(sOursChanged) &&
                !string.IsNullOrEmpty(sTheirsChanged))
            {
                if (sParentChanged.Length < minimumLength ||
                    sOursChanged.Length < minimumLength ||
                    sTheirsChanged.Length < minimumLength)
                    break;

                if (sParentChanged[0] != sOursChanged[0])
                    break;
                if (sParentChanged[0] != sTheirsChanged[0])
                    break;

                sParentChanged = sParentChanged.Substring(1);
                sOursChanged = sOursChanged.Substring(1);
                sTheirsChanged = sTheirsChanged.Substring(1);
            }

            // Remove what they have in common at their ending
            while (!string.IsNullOrEmpty(sParentChanged) &&
                !string.IsNullOrEmpty(sOursChanged) &&
                !string.IsNullOrEmpty(sTheirsChanged))
            {
                if (sParentChanged.Length < minimumLength ||
                    sOursChanged.Length < minimumLength ||
                    sTheirsChanged.Length < minimumLength)
                    break;

                var iParent = sParentChanged.Length - 1;
                var iOurs = sOursChanged.Length - 1;
                var iTheirs = sTheirsChanged.Length - 1;

                if (sParentChanged[iParent] != sOursChanged[iOurs])
                    break;
                if (sParentChanged[iParent] != sTheirsChanged[iTheirs])
                    break;

                sParentChanged = sParentChanged.Substring(0, iParent);
                sOursChanged = sOursChanged.Substring(0, iOurs);
                sTheirsChanged = sTheirsChanged.Substring(0, iTheirs);
            }

            // Build the string
            var sContents = string.Format("Merge Conflict: Original was \"{0}\"; Ours was \"{1}\"; Theirs was \"{2}\"",
                sParentChanged, sOursChanged, sTheirsChanged);

            return sContents;
        }
        #endregion
        #region Method:  void Merge(DBasicText Parent, DBasicText Theirs)
        public void Merge(DBasicText Parent, DBasicText Theirs)
        {
            Phrases.Do3WayMerge(Parent.Phrases, Theirs.Phrases);
            PhrasesBT.Do3WayMerge(Parent.PhrasesBT, Theirs.PhrasesBT);
        }
        #endregion
    }
	#endregion


}
