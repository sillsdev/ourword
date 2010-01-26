/**********************************************************************************************
 * Project: JWdb
 * File:    SfMapping.cs
 * Author:  John Wimbish
 * Created: 24 Sept 2005 (from 25 Nov 2004 original)
 * Purpose: Mapping of standard format into Scripture.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using JWTools;
using OurWordData;
#endregion

namespace OurWordData.DataModel
{
    public class StyleMapping
    {
        public string ToolboxCode { get; private set; }
        public string UsfmCode { get; private set; }
        public string StyleName { get; private set; }
        public bool IsVernacularParagraphStyle { get; private set; }

        #region Constructor(sToolboxCode, sUsfmCode, sStyleName, bIsVernacularParagraph)
        public StyleMapping(string sToolboxCode, string sUsfmCode, 
            string sStyleName, bool bIsVernacularParagraphStyle)
        {
            ToolboxCode = sToolboxCode;
            UsfmCode = sUsfmCode;
            StyleName = sStyleName;
            IsVernacularParagraphStyle = bIsVernacularParagraphStyle;
        }
        #endregion
    }


	public class DSFMapping : JObject
    {
        #region Attr{g}: List<StyleMapping> StyleMappings
        public List<StyleMapping> StyleMappings
        {
            get
            {
                Debug.Assert(null != m_vStyleMappings);
                return m_vStyleMappings;
            }
        }
        List<StyleMapping> m_vStyleMappings;
        #endregion
        #region Method: StyleMapping FindMappingFromOurWord(sOurWordStyle)
        public StyleMapping FindMappingFromOurWord(string sToolboxCode)
        {
            foreach (StyleMapping sm in StyleMappings)
            {
                if (sm.ToolboxCode == sToolboxCode)
                    return sm;
            }
            return null;
        }
        #endregion
        #region Method: StyleMapping FindMappingFromUsfm(sUsfm)
        public StyleMapping FindMappingFromUsfm(string sUsfm)
        {
            foreach (StyleMapping sm in StyleMappings)
            {
                if (sm.UsfmCode == sUsfm)
                    return sm;
            }
            return null;
        }
        #endregion
        #region Method: StyleMapping FindMappingFromName(sName)
        public StyleMapping FindMappingFromName(string sName)
        {
            foreach (StyleMapping sm in StyleMappings)
            {
                if (sm.StyleName == sName)
                    return sm;
            }
            return null;
        }
        #endregion

        #region Attr{g}: bool IsVernacularParagraph(string sMarker)
        public bool IsVernacularParagraph(string sMarker)
        {
            var mapping = FindMappingFromOurWord(sMarker);
            if (null == mapping)
                return false;
            return mapping.IsVernacularParagraphStyle;
        }
        #endregion
        #region VAttr{g}: List<string> VernacularParagraphMarkers
        public List<string> VernacularParagraphMarkers
        {
            get
            {
                var v = new List<string>();

                foreach (StyleMapping sm in StyleMappings)
                {
                    if (sm.IsVernacularParagraphStyle)
                        v.Add(sm.ToolboxCode);
                }

                return v;
            }
        }
        #endregion

        // Constants -------------------------------------------------------------------------
		public const string c_sMkrID = "id";
        public const string c_sMkrPictureCaption = "cap";  // Vernacular caption
        public const string c_sMkrPicturePath = "cat";  // Typically the full pathname
        public const string c_sMkrPictureWordRtf = "ref";  // Info to Word on how to display the picture
        public const string c_sMkrTranslatorNote = "tn";

        // Back Translation Fields -----------------------------------------------------------
        #region Method: string MkrBT(string sMkrBase)
        static string MkrBT(string sMkrBase)
        {
            return "bt" + sMkrBase;
        }
        #endregion
        #region SVAttr{g/s}: string MkrPictureCaptionBT - picture's caption's BT, default: "btcap"
        static public string MkrPictureCaptionBT
		{
			get
			{
                return MkrBT(c_sMkrPictureCaption);
			}
		}
		#endregion

		// ZAttrs: Markers -------------------------------------------------------------------
        #region BAttr{g/s}: string MkrMainTitle        - main title,                   default: "mt"
        public string MkrMainTitle
        {
            get
            {
                return m_sMkrMainTitle;
            }
            set
            {
                SetValue(ref m_sMkrMainTitle, value);
            }
        }
        private string m_sMkrMainTitle = "mt";
        #endregion
        #region BAttr{g/s}: string MkrSubTitle         - Book subtitle,                default: "st"
        public string MkrSubTitle
        {
            get
            {
                return m_sMkrSubTitle;
            }
            set
            {
                SetValue(ref m_sMkrSubTitle, value);
            }
        }
        private string m_sMkrSubTitle = "st";
        #endregion
        #region BAttr{g/s}: string MkrMainTitleBT      - back trans for main title,    default: "btmt"
		public string MkrMainTitleBT
		{
			get
			{
				return m_sMkrMainTitleBT;
			}
			set
			{
                SetValue(ref m_sMkrMainTitleBT, value);
            }
		}
		private string m_sMkrMainTitleBT = "btmt";
		#endregion
		#region BAttr{g/s}: string MkrSubTitleBT       - back trans for book subtitle, default: "btst"
		public string MkrSubTitleBT
		{
			get
			{
				return m_sMkrSubTitleBT;
			}
			set
			{
                SetValue(ref m_sMkrSubTitleBT, value);
            }
		}
		private string m_sMkrSubTitleBT = "btst";
		#endregion
		#region BAttr{g/s}: string MkrFileId           - First line in file            default: "id"
		public string MkrFileId
		{
			get
			{
				return m_sMkrFileId;
			}
			set
			{
                SetValue(ref m_sMkrFileId, value);
            }
		}
		private string m_sMkrFileId = c_sMkrID;
		#endregion
		#region BAttr{g/s}: string MkrHeader           - the running header            default: "h"
		public string MkrHeader
		{
			get
			{
				return m_sMkrHeader;
			}
			set
			{
                SetValue(ref m_sMkrHeader, value);
            }
		}
		private string m_sMkrHeader = "h";
		#endregion
		#region BAttr{g/s}: string MkrHeaderBT         - back trans for run header     default: "bth"
		public string MkrHeaderBT
		{
			get
			{
				return m_sMkrHeaderBT;
			}
			set
			{
                SetValue(ref m_sMkrHeaderBT, value);
            }
		}
		private string m_sMkrHeaderBT = "h";
		#endregion
		#region BAttr{g/s}: string MkrBookHistory      - translation progress notes    default: "hist"
		public string MkrBookHistory
		{
			get
			{
				return m_sMkrBookHistory;
			}
			set
			{
                SetValue(ref m_sMkrBookHistory, value);
            }
		}
		private string m_sMkrBookHistory = "hist";
		#endregion
		#region BAttr{g/s}: string MkrBookNotes        - e.g., info about writing sys  default: "nt"
		public string MkrBookNotes
		{
			get
			{
				return m_sMkrBookNotes;
			}
			set
			{
                SetValue(ref m_sMkrBookNotes, value);
            }
		}
		private string m_sMkrBookNotes = "nt";
		#endregion

        #region BAttr{g/s}: string MkrMajorSection     - Major Section Header,         default: "ms"
        public string MkrMajorSection
        {
            get
            {
                return m_sMkrMajorSection;
            }
            set
            {
                SetValue(ref m_sMkrMajorSection, value);
            }
        }
        private string m_sMkrMajorSection = "ms";
        #endregion
        #region BAttr{g/s}: string MkrMajorSectionCrossRef - Maj Sect Cross Ref        default: "mr"
        public string MkrMajorSectionCrossRef
        {
            get
            {
                return m_sMkrMajorSectionCrossRef;
            }
            set
            {
                SetValue(ref m_sMkrMajorSectionCrossRef, value);
            }
        }
        private string m_sMkrMajorSectionCrossRef = "mr";
        #endregion

		#region BAttr{g/s}: string MkrShoeboxRecord    - Shoebox Record Marker         default: "rcrd"
		public string MkrShoeboxRecord
		{
			get
			{
				return m_sMkrShoeboxRecord;
			}
			set
			{
                SetValue(ref m_sMkrShoeboxRecord, value);
            }
		}
		private string m_sMkrShoeboxRecord = "rcrd";
		#endregion
		#region BAttr{g/s}: string MkrSection          - section title,                default: "s"
		public string MkrSection
		{
			get
			{
				return m_sMkrSection;
			}
			set
			{
                SetValue(ref m_sMkrSection, value);
            }
		}
		private string m_sMkrSection = "s";
		#endregion
		#region BAttr{g/s}: string MkrSection2         - section title level 2,        default: "s2"
		public string MkrSection2
		{
			get
			{
				return m_sMkrSection2;
			}
			set
			{
                SetValue(ref m_sMkrSection2, value);
            }
		}
		private string m_sMkrSection2 = "s2";
		#endregion
		#region BAttr{g/s}: string MkrSectionBT        - back trans for section title, default: "bts"
		public string MkrSectionBT
		{
			get
			{
				return m_sMkrSectionBT;
			}
			set
			{
                SetValue(ref m_sMkrSectionBT, value);
            }
		}
		private string m_sMkrSectionBT = "bts";
		#endregion
		#region BAttr{g/s}: string MkrChapter          - chapter number,               default: "c"
		public string MkrChapter
		{
			get
			{
				return m_sMkrChapter;
			}
			set
			{
                SetValue(ref m_sMkrChapter, value);
            }
		}
		private string m_sMkrChapter = "c";
		#endregion
		#region BAttr{g/s}: string MkrVerse            - verse number,                 default: "v"
		public string MkrVerse
		{
			get
			{
				return m_sMkrVerse;
			}
			set
			{
                SetValue(ref m_sMkrVerse, value);
            }
		}
		private string m_sMkrVerse = "v";
		#endregion
		#region BAttr{g/s}: string MkrCrossRef         - cross reference paragraph,    default: "r"
		public string MkrCrossRef
		{
			get
			{
				return m_sMkrCrossRef;
			}
			set
			{
                SetValue(ref m_sMkrCrossRef, value);
            }
		}
		private string m_sMkrCrossRef = "r";
		#endregion
		#region BAttr{g/s}: string MkrVerseText        - verse text,                   default: "vt"
		public string MkrVerseText
		{
			get
			{
				return m_sMkrVerseText;
			}
			set
			{
                SetValue(ref m_sMkrVerseText, value);
            }
		}
		private string m_sMkrVerseText = "vt";
		#endregion
		#region BAttr{g/s}: string MkrVerseTextBT      - verse text prose back trans,  default: "btvt"
		public string MkrVerseTextBT
		{
			get
			{
				return m_sMkrVerseTextBT;
			}
			set
			{
                SetValue(ref m_sMkrVerseTextBT, value);
            }
		}
		private string m_sMkrVerseTextBT = "btvt";
		#endregion
		#region BAttr{g/s}: string MkrSeeAlso          - See Also footnote,            default: "cf"
		public string MkrSeeAlso
		{
			get
			{
				return m_sMkrSeeAlso;
			}
			set
			{
                SetValue(ref m_sMkrSeeAlso, value);
            }
		}
		private string m_sMkrSeeAlso = "cf";
		#endregion
		#region BAttr{g/s}: string MkrFootnote         - footnote paragraph,           default: "ft"
		public string MkrFootnote
		{
			get
			{
				return m_sMkrFootnote;
			}
			set
			{
                SetValue(ref m_sMkrFootnote, value);
            }
		}
		private string m_sMkrFootnote = "ft";
		#endregion
		#region BAttr{g/s}: string MkrFootnoteBT       - footnote para's back trans,   default: "btft"
		public string MkrFootnoteBT
		{
			get
			{
				return m_sMkrFootnoteBT;
			}
			set
			{
                SetValue(ref m_sMkrFootnoteBT, value);
            }
		}
		private string m_sMkrFootnoteBT = "btft";
		#endregion

		#region BAttr{g/s}: string InlineFootnote      - Footnote letter in text,      default: "fn"
		public string InlineFootnote
		{
			get
			{
				return m_sInlineFootnote;
			}
			set
			{
                SetValue(ref m_sInlineFootnote, value);
            }
		}
		private string m_sInlineFootnote = "fn";
		#endregion
		#region BAttr{g/s}: string MkrComment          - remark, comment               default: "rem"
		public string MkrComment
		{
			get
			{
				return m_sMkrComment;
			}
			set
			{
                SetValue(ref m_sMkrComment, value);
            }
		}
		private string m_sMkrComment = "rem";
		#endregion
		#region BAttr{g/s}: string MkrCopyright        - Copyright information         default: "cy"
		public string MkrCopyright
		{
			get
			{
				return m_sMkrCopyright;
			}
			set
			{
                SetValue(ref m_sMkrCopyright, value);
            }
		}
		private string m_sMkrCopyright = "cy";
		#endregion
		#region BAttr{g/s}: string MkrStatusComment    - comment on checking done      default: "chk"
		public string MkrStatusComment
		{
			get
			{
				return m_sMkrStatusComment;
			}
			set
			{
                SetValue(ref m_sMkrStatusComment, value);
            }
		}
		private string m_sMkrStatusComment = "chk";
		#endregion
		#region BAttr{g/s}: string MkrDateStamp        - Date last edited              default: "ud"
		public string MkrDateStamp
		{
			get
			{
				return m_sMkrDateStamp;
			}
			set
			{
                SetValue(ref m_sMkrDateStamp, value);
            }
		}
		private string m_sMkrDateStamp = "ud";
		#endregion

		// ZAttrs: Style Abbreviations -------------------------------------------------------
		#region BAttr{g/s}: string StyleMainTitle      - book main title,              default: "mt"
		public string StyleMainTitle
		{
			get
			{
				return m_sStyleMainTitle;
			}
			set
			{
                SetValue(ref m_sStyleMainTitle, value);
            }
		}
		private string m_sStyleMainTitle = "mt";
		#endregion
		#region BAttr{g/s}: string StyleSubTitle       - book sub title,               default: "st"
		public string StyleSubTitle
		{
			get
			{
				return m_sStyleSubTitle;
			}
			set
			{
                SetValue(ref m_sStyleSubTitle, value);
            }
		}
		private string m_sStyleSubTitle = "st";
		#endregion
		#region BAttr{g/s}: string StyleHeader         - book running header,          default: "h"
		public string StyleHeader
		{
			get
			{
				return m_sStyleHeader;
			}
			set
			{
                SetValue(ref m_sStyleHeader, value);
            }
		}
		private string m_sStyleHeader = "h";
		#endregion
		#region BAttr{g/s}: string StyleSection        - section title,                default: "s"
		public string StyleSection
		{
			get
			{
				return m_sStyleSection;
			}
			set
			{
                SetValue(ref m_sStyleSection, value);
            }
		}
		private string m_sStyleSection = "s";
		#endregion
		#region BAttr{g/s}: string StyleSection2       - section title level 2,        default: "s2"
		public string StyleSection2
		{
			get
			{
				return m_sStyleSection2;
			}
			set
			{
                SetValue(ref m_sStyleSection2, value);
            }
		}
		private string m_sStyleSection2 = "s2";
		#endregion
		#region BAttr{g/s}: string StyleCrossRef       - cross reference,              default: "r"
		public string StyleCrossRef
		{
			get
			{
				return m_sStyleCrossRef;
			}
			set
			{
                SetValue(ref m_sStyleCrossRef, value);
            }
		}
		private string m_sStyleCrossRef = "r";
		#endregion
		#region BAttr{g/s}: string StyleChapter        - chapter number,               default: "c"
		public string StyleChapter
		{
			get
			{
				return m_sStyleChapter;
			}
			set
			{
                SetValue(ref m_sStyleChapter, value);
            }
		}
		private string m_sStyleChapter = "c";
		#endregion
		#region BAttr{g/s}: string StyleVerse          - verse number,                 default: "v"
		public string StyleVerse
		{
			get
			{
				return m_sStyleVerse;
			}
			set
			{
                SetValue(ref m_sStyleVerse, value);
            }
		}
		private string m_sStyleVerse = "v";
		#endregion
		#region BAttr{g/s}: string StyleSeeAlso        - See Also,                     default: "cf"
		public string StyleSeeAlso
		{
			get
			{
				return m_sStyleSeeAlso;
			}
			set
			{
                SetValue(ref m_sStyleSeeAlso, value);
            }
		}
		private string m_sStyleSeeAlso = "cf";
		#endregion
		#region BAttr{g/s}: string StyleFootLetter     - Foot Letter,                  default: "fn"
		public string StyleFootLetter
		{
			get
			{
				return m_sStyleFootLetter;
			}
			set
			{
                SetValue(ref m_sStyleFootLetter, value);
            }
		}
		private string m_sStyleFootLetter = "fn";
		#endregion
		#region BAttr{g/s}: string StyleFootnotePara   - Foot Paragagraph,             default: "ft"
		public string StyleFootnotePara
		{
			get
			{
				return m_sStyleFootnotePara;
			}
			set
			{
                SetValue(ref m_sStyleFootnotePara, value);
            }
		}
		private string m_sStyleFootnotePara = "ft";
		#endregion
		#region BAttr{g/s}: string StyleSeeAlsoPara   - SeeAlso Paragagraph,           default: "cft"
		public string StyleSeeAlsoPara
		{
			get
			{
				return m_sStyleSeeAlsoPara;
			}
			set
			{
                SetValue(ref m_sStyleSeeAlsoPara, value);
            }
		}
		private string m_sStyleSeeAlsoPara = "cft";
		#endregion

		#region BAttr{g/s}: string StylePicCaption     - picture caption,              default: "cap"
		public string StylePicCaption
		{
			get
			{
				return m_sStylePicCaption;
			}
			set
			{
                SetValue(ref m_sStylePicCaption, value);
            }
		}
		private string m_sStylePicCaption = "cap";
		#endregion
		#region BAttr{g/s}: string StyleNoteLetter     - Note Letter,                  default: "ntc"
		public string StyleNoteLetter
		{
			get
			{
				return m_sStyleNoteLetter;
			}
			set
			{
                SetValue(ref m_sStyleNoteLetter, value);
            }
		}
		private string m_sStyleNoteLetter = "ntc";
		#endregion
		#region BAttr{g/s}: string StyleNoteParagraph  - Note Paragraph,               default: "nt"
		public string StyleNoteParagraph
		{
			get
			{
				return m_sStyleNoteParagraph;
			}
			set
			{
                SetValue(ref m_sStyleNoteParagraph, value);
			}
		}
		private string m_sStyleNoteParagraph = "nt";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("MainTitleBT",     ref m_sMkrMainTitleBT);
			DefineAttr("SubTitleBT",      ref m_sMkrSubTitleBT);
			DefineAttr("ID",              ref m_sMkrFileId);
			DefineAttr("Header",          ref m_sMkrHeader);
			DefineAttr("HeaderBT",        ref m_sMkrHeaderBT);
			DefineAttr("History",         ref m_sMkrBookHistory);
			DefineAttr("ShoeboxRcd",      ref m_sMkrShoeboxRecord);
            DefineAttr("MajSect",         ref m_sMkrMajorSection);
            DefineAttr("MajSectCrossRef", ref m_sMkrMajorSectionCrossRef);
			DefineAttr("Section",         ref m_sMkrSection);
			DefineAttr("SectionBT",       ref m_sMkrSectionBT);
			DefineAttr("Chapter",         ref m_sMkrChapter);
			DefineAttr("Verse",           ref m_sMkrVerse);
			DefineAttr("CrossRef",        ref m_sMkrCrossRef);
			DefineAttr("VerseText",       ref m_sMkrVerseText);
			DefineAttr("VerseTextBT",     ref m_sMkrVerseTextBT);
			DefineAttr("SeeAlso",         ref m_sMkrSeeAlso);
			DefineAttr("Footnote",        ref m_sMkrFootnote);
			DefineAttr("FootnoteBT",      ref m_sMkrFootnoteBT);
			DefineAttr("FootLetter",      ref m_sInlineFootnote);
			DefineAttr("Comment",         ref m_sMkrComment);
			DefineAttr("StatusComment",   ref m_sMkrStatusComment);
			DefineAttr("Copyright",       ref m_sMkrCopyright);

			DefineAttr("StyleMainTitle",  ref m_sStyleMainTitle);
			DefineAttr("StyleSubTitle",   ref m_sStyleSubTitle);
			DefineAttr("StyleHeader",     ref m_sStyleHeader);
			DefineAttr("StyleSection",    ref m_sStyleSection);
			DefineAttr("StyleCrossRef",   ref m_sStyleCrossRef);
			DefineAttr("StyleChapter",    ref m_sStyleChapter);
			DefineAttr("StyleVerse",      ref m_sStyleVerse);
			DefineAttr("StyleSeeAlso",    ref m_sStyleSeeAlso);
			DefineAttr("StyleFootLetter", ref m_sStyleFootLetter);
			DefineAttr("StylePicCaption", ref m_sStylePicCaption);
			DefineAttr("StyleNoteLetter", ref m_sStyleNoteLetter);
			DefineAttr("StyleNotePara",   ref m_sStyleNoteParagraph);
		}
		#endregion

		// Unconverted attributes ------------------------------------------------------------
		private ArrayList m_rgDiscardMrks = null;

		// Mappings Tests --------------------------------------------------------------------
		#region Attr{g}: bool IsFileIdMarker(sMarker)
		public bool IsFileIdMarker(string sMarker)
		{
			return ( sMarker == MkrFileId );
		}
		#endregion
		#region Attr{g}: bool IsBookHistoryMarker(sMarker)
		public bool IsBookHistoryMarker(string sMarker)
		{
			return ( sMarker == MkrBookHistory );
		}
		#endregion
		#region Attr{g}: bool IsBookNotesMarker(sMarker)
		public bool IsBookNotesMarker(string sMarker)
		{
			return ( sMarker == MkrBookNotes );
		}
		#endregion
		#region Attr{g}: bool IsHeaderMarker(sMarker)
		public bool IsHeaderMarker(string sMarker)
		{
			return ( sMarker == MkrHeader );
		}
		#endregion
		#region Attr{g}: bool IsShoeboxRecordMarker(sMarker)
		public bool IsShoeboxRecordMarker(string sMarker)
		{
			return ( sMarker == MkrShoeboxRecord );
		}
		#endregion
		#region Attr{g}: bool IsSection(sMarker)
		public bool IsSection(string sMarker)
		{
			return ( sMarker == MkrSection );
		}
		#endregion
		#region Attr{g}: bool IsSection2(sMarker)
		public bool IsSection2(string sMarker)
		{
			return ( sMarker == MkrSection2 );
		}
		#endregion
		#region Attr{g}: bool IsSectionBT(sMarker)
		public bool IsSectionBT(string sMarker)
		{
			return ( sMarker == MkrSectionBT );
		}
		#endregion
		#region Attr{g}: bool IsCrossRef(sMarker)
		public bool IsCrossRef(string sMarker)
		{
			return ( sMarker == MkrCrossRef );
		}
		#endregion
		#region Attr{g}: bool IsChapter(sMarker)
		public bool IsChapter(string sMarker)
		{
			return ( sMarker == MkrChapter );
		}
		#endregion
		#region Attr{g}: bool IsVerse(sMarker)
		public bool IsVerse(string sMarker)
		{
			return ( sMarker == MkrVerse );
		}
		#endregion
		#region Attr{g}: bool IsVerseText(sMarker)
		public bool IsVerseText(string sMarker)
		{
			return ( sMarker == MkrVerseText );
		}
		#endregion
		#region Attr{g}: bool IsVerseTextBT(sMarker)
		public bool IsVerseTextBT(string sMarker)
		{
			return ( sMarker == MkrVerseTextBT );
		}
		#endregion

        #region Attr{g}: bool IsDiscardedField(string sMarker)
        public bool IsDiscardedField(string sMarker)
		{
			foreach ( string s in m_rgDiscardMrks )
			{
				if ( s == sMarker )
					return true;
			}
			return false;
		}
		#endregion
		#region SMethod: bool IsPicture(sMarker)
		static public bool IsPicture(string sMarker)
		{

            if (sMarker == c_sMkrPicturePath) 
                return true;
            if (sMarker == c_sMkrPictureWordRtf) 
                return true;
            if (sMarker == c_sMkrPictureCaption) 
                return true;

			if (sMarker == MkrPictureCaptionBT)
				return true;

			return false;
		}
		#endregion

		#region Attr{g}: bool IsPictureCaptionBT(sMarker)
		public bool IsPictureCaptionBT(string sMarker)
		{
			return (sMarker == MkrPictureCaptionBT);
		}
		#endregion
		#region Attr{g}: bool IsMainTitleField(string sMarker)
		public bool IsMainTitleField(string sMarker)
		{
            return (sMarker == MkrMainTitle);
		}
		#endregion
		#region Attr{g}: bool IsMainTitleBT(sMarker)
		public bool IsMainTitleBT(string sMarker)
		{
			return ( sMarker == MkrMainTitleBT );
		}
		#endregion
		#region Attr{g}: bool IsSubTitleField(string sMarker)
		public bool IsSubTitleField(string sMarker)
		{
            return (sMarker == MkrSubTitle);
		}
		#endregion
		#region Attr{g}: bool IsSubTitleBT(sMarker)
		public bool IsSubTitleBT(string sMarker)
		{
			return ( sMarker == MkrSubTitleBT );
		}
		#endregion
		#region Attr{g}: bool IsHeaderBT(sMarker)
		public bool IsHeaderBT(string sMarker)
		{
			return ( sMarker == MkrHeaderBT );
		}
		#endregion
        #region Attr{g}: bool IsMajorSectionMarker(string sMarker)
        public bool IsMajorSectionMarker(string sMarker)
        {
            if (sMarker == "ms")
                return true;
            if (sMarker == "mr")
                return true;
            return false;
        }
        #endregion
        #region Attr{g}: bool IsSeeAlso(sMarker)
        public bool IsSeeAlso(string sMarker)
		{
			return ( sMarker == MkrSeeAlso );
		}
		#endregion
		#region Attr{g}: bool IsFootnote(sMarker)
		public bool IsFootnote(string sMarker)
		{
			return ( sMarker == MkrFootnote );
		}
		#endregion
		#region Attr{g}: bool IsFootnoteBT(sMarker)
		public bool IsFootnoteBT(string sMarker)
		{
			return ( sMarker == MkrFootnoteBT );
		}
		#endregion
		#region Attr{g}: int InlineFootnoteLength - e.g., "|fn" returns 3
		public int InlineFootnoteLength
		{
			get
			{
				return 1 + InlineFootnote.Length;
			}
		}
		#endregion
		#region Method: bool IsInlineFootnote(string s, int iPos) - T if s[iPos] is "|fn"
		public bool IsInlineFootnote(string s, int iPos)
		{
			// Make sure the string is long enough to qualify.
			// E.g., given "hello|fn" (Length = 8), if iPos = 6 we bail.
			if (iPos + InlineFootnoteLength > s.Length)
				return false;

			// Should start with the Standard Format inline marker
			if (s[iPos] != '|')
				return false;

			// Next should be the inline string we are expecting
			if ( InlineFootnote == s.Substring(iPos + 1, InlineFootnote.Length))
			{
				// If this is the end of the string, then we've suceeded
				if (s.Length == iPos + InlineFootnoteLength)
					return true;

				// Otherwise, the next char should be a whitespace or punctuation
				char c = s[iPos + InlineFootnoteLength];
				if (c == ' ' || char.IsPunctuation(c))
					return true;
			}

			return false;
		}
		#endregion
		#region Attr{g}: bool IsCommentMarker(sMarker)
		public bool IsCommentMarker(string sMarker)
		{
			return ( sMarker == MkrComment );
		}
		#endregion
		#region Attr{g}: bool IsStatusCommentMarker(sMarker)
		public bool IsStatusCommentMarker(string sMarker)
		{
			return ( sMarker == MkrStatusComment );
		}
		#endregion
		#region Attr{g}: bool IsDateStampMarker(sMarker)
		public bool IsDateStampMarker(string sMarker)
		{
			return ( sMarker == MkrDateStamp );
		}
		#endregion
		#region Attr{g}: bool IsCopyrightMarker(sMarker)
		public bool IsCopyrightMarker(string sMarker)
		{
			return ( sMarker == MkrCopyright );
		}
		#endregion

        // Categories of markers -------------------------------------------------------------
        #region Method: bool IsSectionContentsMarker(string sMarker)
        public bool IsSectionContentsMarker(string sMarker)
        {
            // Compare it against everything we currently recognize
            if (IsMajorSectionMarker(sMarker)) return true;
            if (IsSection(sMarker)) return true;
            if (IsSection2(sMarker)) return true;
            if (IsCrossRef(sMarker)) return true;
            if (IsChapter(sMarker)) return true;
            if (IsVerse(sMarker)) return true;
            if (IsVerseText(sMarker)) return true;
            if (IsVernacularParagraph(sMarker)) return true;
            if (IsPicture(sMarker)) return true;
            if (IsSeeAlso(sMarker)) return true;
            if (IsFootnote(sMarker)) return true;
            if (IsCommentMarker(sMarker)) return true;
            if (IsStatusCommentMarker(sMarker)) return true;
            if (IsDateStampMarker(sMarker)) return true;
            if (IsCopyrightMarker(sMarker)) return true;
            if (TranslatorNote.IsOldStyleMarker(sMarker))
                return true;
            if (c_sMkrTranslatorNote == sMarker) return true;

            // Didn't find it in our list
            return false;
        }
        #endregion
        #region Method: bool IsBookOverviewMarker(string sMarker)
        public bool IsBookOverviewMarker(string sMarker)
        {
            // Now compare it against everything we currently recognize
            if (IsFileIdMarker(sMarker)) return true;
            if (IsBookHistoryMarker(sMarker)) return true;
            if (IsBookNotesMarker(sMarker)) return true;
            if (IsHeaderMarker(sMarker)) return true;
            if (IsShoeboxRecordMarker(sMarker)) return true;
            if (IsDiscardedField(sMarker)) return true;
            if (IsMainTitleField(sMarker)) return true;
            if (IsSubTitleField(sMarker)) return true;
            if (IsCommentMarker(sMarker)) return true;
            if (IsDateStampMarker(sMarker)) return true;

            // Didn't find it in our list
            return false;
        }
        #endregion
        #region Method: bool IsRecognizedMarker(string sMarker)
        public bool IsRecognizedMarker(string sMarker)
        {
            // Check the Section Contents Markers
            if (IsSectionContentsMarker(sMarker))
                return true;

            // IF not there, then the Book Overview set of markers
            if (IsBookOverviewMarker(sMarker))
                return true;

            // History can be in either Book or Section
            if (sMarker == "History")
                return true;

            // Didn't find it in either list
            return false;
        }
        #endregion
        #region Method: bool IsUSFMExportMarker(string sMarker)
        public bool IsUSFMExportMarker(string sMarker)
        {
            // Now compare it against everything we currently export to USFM
            if (IsFileIdMarker(sMarker)) return true;
            if (IsHeaderMarker(sMarker)) return true;
            if (IsMainTitleField(sMarker)) return true;
            if (IsSubTitleField(sMarker)) return true;
            if (IsSection(sMarker)) return true;
            if (IsSection2(sMarker)) return true;
            if (IsCrossRef(sMarker)) return true;
            if (IsChapter(sMarker)) return true;
            if (IsVerse(sMarker)) return true;
            if (IsVerseText(sMarker)) return true;
            if (IsVernacularParagraph(sMarker)) return true;
            if (IsPicture(sMarker)) return true;
            if (IsSeeAlso(sMarker)) return true;
            if (IsFootnote(sMarker)) return true;

            // Didn't find it in our list
            return false;
        }
        #endregion
        #region Method: bool IsGoBibleExportMarker(string sMarker)
        public bool IsGoBibleExportMarker(string sMarker)
            // GoBibleCreator does not support the full set of USFM. So we just
            // check against the markers that it does support.
        {
            string[] vsGoBibleMarkers = { 
                "id", "h", "mt", "p", "q", "q2", "q3", "q4", "qc", "s", "c", "v", "vt" };

            foreach (string s in vsGoBibleMarkers)
            {
                if (s == sMarker)
                    return true;
            }

            // Didn't find it in our list
            return false;
        }
        #endregion

        // Character Style Mappings ----------------------------------------------------------
		#region Method: bool IsItalicBegin(string s, int iPos)
		public bool IsItalicBegin(string s, int iPos)
		{
			if (iPos < s.Length - 1 && s.Substring(iPos,2) == "|i")
				return true;
			return false;
		}
		#endregion
		#region Method: bool IsItalicEnd(string s, int iPos)
		public bool IsItalicEnd(string s, int iPos)
		{
			if (iPos < s.Length - 1 && s.Substring(iPos,2) == "|r")
				return true;
			return false;
		}
		#endregion
		#region Method: bool IsBoldBegin(string s, int iPos)
		public bool IsBoldBegin(string s, int iPos)
		{
			if (iPos < s.Length - 1 && s.Substring(iPos,2) == "|b")
				return true;
			return false;
		}
		#endregion
		#region Method: public bool IsBoldEnd(string s, int iPos)
		public bool IsBoldEnd(string s, int iPos)
		{
			if (iPos < s.Length - 1 && s.Substring(iPos,2) == "|r")
				return true;
			return false;
		}
		#endregion

		// Style tests -----------------------------------------------------------------------
		#region Attr{g}: bool IsRaisedCharStyle(sAbbrev) - T if v, c, foot letter, note letter.
		public bool IsRaisedCharStyle(string sStyleAbbrev)
		{
			if (sStyleAbbrev == StyleFootLetter)
				return true;
			if (sStyleAbbrev == StyleChapter)
				return true;
			if (sStyleAbbrev == StyleVerse)
				return true;
			if (sStyleAbbrev == StyleNoteLetter)
				return true;
			if (sStyleAbbrev == StyleSeeAlso)
				return true;
			return false;
		}
		#endregion
		#region Attr{g}: bool IsTitleStyle(string sStyleAbbrev)
		public bool IsTitleStyle(string sStyleAbbrev)
		{
			if (sStyleAbbrev == StyleMainTitle)
				return true;
			if (sStyleAbbrev == StyleSubTitle)
				return true;
			return false;
		}
		#endregion
		#region Attr{g}: bool IsHeaderStyle(string sStyleAbbrev)
		public bool IsHeaderStyle(string sStyleAbbrev)
		{
			if (sStyleAbbrev == StyleHeader)
				return true;
			return false;
		}
		#endregion
		#region Attr{g}: bool IsFootnoteParaStyle(string sStyleAbbrev)
		public bool IsFootnoteParaStyle(string sStyleAbbrev)
		{
			if (sStyleAbbrev == StyleFootnotePara)
				return true;
			return false;
		}
		#endregion
		#region Attr{g}: bool IsSeeAlsoFootnoteParaStyle(string sStyleAbbrev)
		public bool IsSeeAlsoFootnoteParaStyle(string sStyleAbbrev)
		{
			if (sStyleAbbrev == StyleSeeAlsoPara)
				return true;
			return false;
		}
		#endregion
		#region Attr{g}: bool IsSectionEmptyReferenceStyle(string sStyleAbbrev)
		public bool IsSectionEmptyReferenceStyle(string sStyleAbbrev)
		{
			if (sStyleAbbrev == StyleSection)
				return true;
			if (sStyleAbbrev == StyleSection2)
				return true;
			if (sStyleAbbrev == this.StyleCrossRef)
				return true;
			return false;
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DSFMapping()
			: base()
		{
            m_vStyleMappings = new List<StyleMapping>();
            StyleMappings.Add(new StyleMapping("h", "h", "Header", false));
            StyleMappings.Add(new StyleMapping("mt", "mt", "Title Main", false));
            StyleMappings.Add(new StyleMapping("st", "mt2", "Title Secondary", false));
            StyleMappings.Add(new StyleMapping("p", "p", "Paragraph", true));
            StyleMappings.Add(new StyleMapping("m", "m", "Paragraph Continuation", true));
            StyleMappings.Add(new StyleMapping("s", "s1", "Section Head", false));
            StyleMappings.Add(new StyleMapping("s2", "s2", "Section Head 2", false));
            StyleMappings.Add(new StyleMapping("r", "r", "Parallel Passage Reference", false));
            StyleMappings.Add(new StyleMapping("q", "q1", "Line 1", true));
            StyleMappings.Add(new StyleMapping("q2", "q2", "Line 2", true));
            StyleMappings.Add(new StyleMapping("q3", "q3", "Line 3", true));
            StyleMappings.Add(new StyleMapping("qc", "qc", "Line Centered", true));
            StyleMappings.Add(new StyleMapping("cap", "fig", "Caption", false));
            StyleMappings.Add(new StyleMapping("ms", "ms", "Major Section Head", false));
            StyleMappings.Add(new StyleMapping("mr", "mr", "Major Section Range", false));
            StyleMappings.Add(new StyleMapping("cf", "x", "Note Cross Reference Paragraph", false));
            StyleMappings.Add(new StyleMapping("fn", "f", "Note General Paragraph", false));
            StyleMappings.Add(new StyleMapping("NoteMessage", "", "Annotation Message", false));

			// TODO: Need to persist these array values

			// Markers that signal fields that we'll discard
			m_rgDiscardMrks = new ArrayList();
			m_rgDiscardMrks.Add("e");   // End of file
			m_rgDiscardMrks.Add("bk");  // Tomohon's "Bahasa Kupang" (Kupang translation)
			m_rgDiscardMrks.Add("tb");  // Tomohon's "Terjemahan Baru" (Indonesian translation)
			m_rgDiscardMrks.Add("bis"); // Tomohon's Indonesian translation
			m_rgDiscardMrks.Add("bm");  // Tomohon's Bahasa Manado field
		}
		#endregion
	}
}
