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
			var s = "";

			foreach(var ch in Text)
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

}
