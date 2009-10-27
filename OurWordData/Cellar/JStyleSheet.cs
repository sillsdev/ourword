/**********************************************************************************************
 * App:     Josiah
 * File:    JStyleSheet.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles paragraph and character styles, and the containing stylesheet
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using JWTools;
#endregion
#region Documentation
/* Documentation
 * 
 * A Font's Height refers to line spacing; Size is the size of the letters. All specified
 * in Points. I typically use a Size of 10, which results in a Height of 16.
 */
#endregion

namespace OurWordData
{
	#region CLASS JStyleSheet
	public class JStyleSheet : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string DisplayName - the style's name as it appears in the UI
		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
                SetValue(ref m_sDisplayName, value);
			}
		}
		private string m_sDisplayName = "Default Style Sheet";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("DisplayName", ref m_sDisplayName);
		}
		#endregion

		// JAttributes: ----------------------------------------------------------------------
		#region Attr{g}: JOwnSeq ParagraphStyles - the list of paragraph styles
		public JOwnSeq<JParagraphStyle> ParagraphStyles
		{
			get 
			{ 
				return j_osParagraphStyles; 
			}
		}
		private JOwnSeq<JParagraphStyle> j_osParagraphStyles = null;
		#endregion
		#region Attr{g}: JOwnSeq CharacterStyles - the list of character styles
		public JOwnSeq<JCharacterStyle> CharacterStyles
		{
			get 
			{ 
				return j_osCharacterStyles; 
			}
		}
		private JOwnSeq<JCharacterStyle> j_osCharacterStyles = null;
		#endregion
		#region Attr{g}: JOwnSeq WritingSystems - the list of writing systems
		public JOwnSeq<JWritingSystem> WritingSystems
		{
			get 
			{ 
				return j_osWritingSystems; 
			}
		}
		private JOwnSeq<JWritingSystem> j_osWritingSystems = null;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JStyleSheet()
			: base()
		{
            j_osParagraphStyles = new JOwnSeq<JParagraphStyle>("ParaStyles", this, true, true);
            j_osCharacterStyles = new JOwnSeq<JCharacterStyle>("CharStyles", this, true, true);
            j_osWritingSystems = new JOwnSeq<JWritingSystem>("WritingSystems", this, true, true);
		}
		#endregion
		#region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return DisplayName; 
			}
		}
		#endregion

		// WritingSystems Access -------------------------------------------------------------
		#region Method: JWritingSystem AddWritingSystem(sName) - Adds a new WS
		public JWritingSystem AddWritingSystem(string sName)
		{
			JWritingSystem ws = new JWritingSystem(sName);
			WritingSystems.Append(ws);
			return ws;
		}
		#endregion
		#region Method: JWritingSystem FindWritingSystem(sName) - returns the named WS
		public JWritingSystem FindWritingSystem(string sName)
		{
			int i = WritingSystems.Find(sName);
			if (-1 == i)
				return null;
			return (JWritingSystem)WritingSystems[i];
		}
		#endregion
		#region Method: JWritingSystem FindOrAddWritingSystem(sName) - creates the WS if necessary
		public JWritingSystem FindOrAddWritingSystem(string sName)
		{
			JWritingSystem ws = FindWritingSystem(sName);
			if (null != ws)
				return ws;

			// Add the writing system
			ws = AddWritingSystem(sName);

			// Make sure we have the JFontsForWritingSystems objects
			foreach (JCharacterStyle cs in CharacterStyles)
				cs.EnsureFontsForWritingSystems();
			foreach (JParagraphStyle ps in ParagraphStyles)
				ps.CharacterStyle.EnsureFontsForWritingSystems();

			return ws;
		}
		#endregion
		#region Method: JWritingSystem GetWritingSystem(index) - returns the WS at the given index
		public JWritingSystem FindWritingSystem(int index)
		{
			if (index < 0 || index >= WritingSystems.Count)
				return null;
			return (JWritingSystem)WritingSystems[index];
		}
		#endregion
		#region Method: void RemoveWritingSystem(JWritingSystem)
		public void RemoveWritingSystem(JWritingSystem ws)
		{
			if (-1 == WritingSystems.FindObj(ws))
				return;

			// Remove the corresponding FontsForWritingSystem
			foreach (JCharacterStyle cs in CharacterStyles)
			{
				JFontForWritingSystem fws = cs.FindOrAddFontForWritingSystem(ws);
				cs.FontsForWritingSystems.Remove(fws);
			}
			foreach (JParagraphStyle ps in ParagraphStyles)
			{
				JCharacterStyle cs = ps.CharacterStyle;
				JFontForWritingSystem fws = cs.FindOrAddFontForWritingSystem(ws);
				cs.FontsForWritingSystems.Remove(fws);
			}

			// Remove the Writing System
			WritingSystems.Remove(ws);
		}
		#endregion

		// Paragraph Styles Access -----------------------------------------------------------
		#region Method: JParagraphStyle AddParagraphStyle(sAbbrev, sDisplayName)
		public JParagraphStyle AddParagraphStyle(string sAbbrev, string sDisplayName)
		{
			JParagraphStyle style = new JParagraphStyle(sAbbrev, sDisplayName);
			ParagraphStyles.Append(style);
			return style;
		}
		#endregion
		#region Method: JParagraphStyle FindParagraphStyle(sAbbrev)
		public JParagraphStyle FindParagraphStyle(string sAbbrev)
		{
			int i = ParagraphStyles.Find(sAbbrev);
			if (-1 == i)
				return null;
			return (JParagraphStyle)ParagraphStyles[i];
		}
		#endregion
		#region Method: JParagraphStyle FindOrAddParagraphStyle(sAbbrev, sDisplayName)
		public JParagraphStyle FindOrAddParagraphStyle(string sAbbrev, string sDisplayName)
		{
			JParagraphStyle style = FindParagraphStyle(sAbbrev);
			if (null == style)
				style = AddParagraphStyle(sAbbrev, sDisplayName);
			return style;
		}
		#endregion
		#region Method: JParagraphStyle FindParagraphStyleByDisplayName(sDisplayName)
		public JParagraphStyle FindParagraphStyleByDisplayName(string sDisplayName)
		{
			foreach(JParagraphStyle style in ParagraphStyles)
			{
				if (style.DisplayName == sDisplayName)
					return style;
			}
			return null;
		}
		#endregion

		// Character Styles Access -----------------------------------------------------------
		#region Method: JCharacterStyle AddCharacterStyle(sAbbrev, sDisplayName)
		public JCharacterStyle AddCharacterStyle(string sAbbrev, string sDisplayName)
		{
			JCharacterStyle style = new JCharacterStyle(sAbbrev, sDisplayName);
			CharacterStyles.Append(style);
			return style;
		}
		#endregion
		#region Method: JCharacterStyle FindCharacterStyle(sAbbrev)
		public JCharacterStyle FindCharacterStyle(string sAbbrev)
		{
			if (null == sAbbrev || sAbbrev.Length == 0)
				return null;
			int i = CharacterStyles.Find(sAbbrev);
			if (-1 == i)
				return null;
			return (JCharacterStyle)CharacterStyles[i];
		}
		#endregion
		#region Method: JCharacterStyle FindOrAddCharacterStyle(sAbbrev, sDisplayName)
		public JCharacterStyle FindOrAddCharacterStyle(string sAbbrev, string sDisplayName)
		{
			JCharacterStyle style = FindCharacterStyle(sAbbrev);
			if (null == style)
				style = AddCharacterStyle(sAbbrev, sDisplayName);
			return style;
		}
		#endregion
		#region Method: bool IsCharacterStyle(string sAbbrev)
		public bool IsCharacterStyle(string sAbbrev)
		{
			if (null == FindCharacterStyle(sAbbrev))
				return false;
			return true;
		}
		#endregion
		#region Method: JCharacterStyle FindCharacterStyleByDisplayName(sDisplayName)
		public JCharacterStyle FindCharacterStyleByDisplayName(string sDisplayName)
		{
			foreach(JCharacterStyle style in CharacterStyles)
			{
				if (style.DisplayName == sDisplayName)
					return style;
			}
			return null;
		}
		#endregion

        #region Attr{g/s}: float ZoomFactor
        public float ZoomFactor
        {
            get
            {
                return m_fZoomFactor;
            }
            set
            {
                m_fZoomFactor = value;
                _ResetFonts();
            }
        }
        float m_fZoomFactor = 1.0F;
        #endregion
        #region Method: void _ResetFonts()
        private void _ResetFonts()
        {
            foreach (JCharacterStyle cs in CharacterStyles)
                cs.ResetFonts();

            foreach (JParagraphStyle ps in ParagraphStyles)
                ps.CharacterStyle.ResetFonts();
        }
        #endregion
    }
	#endregion

	#region CLASS JParagraphStyle
	public class JParagraphStyle : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: int SpaceBefore - points of space before a paragraph
		public int SpaceBefore
		{
			get
			{
				return m_ptSpaceBefore;
			}
			set
			{
                SetValue(ref m_ptSpaceBefore, value);
			}
		}
		private int m_ptSpaceBefore = 0;
		#endregion
		#region BAttr{g/s}: int SpaceAfter - points of space after a paragraph
		public int SpaceAfter
		{
			get
			{
				return m_ptSpaceAfter;
			}
			set
			{
                SetValue(ref m_ptSpaceAfter, value);
			}
		}
		private int m_ptSpaceAfter = 6;
		#endregion
		#region BAttr{g/s}: (private) AlignType Alignment (kLeft, kRight, kCentered, kJustified)
		public enum AlignType { kLeft = 0, kRight, kCentered, kJustified };
		public AlignType Alignment
		{
			get
			{
				return (AlignType)m_nAlignment;
			}
			set
			{
                SetValue(ref m_nAlignment, (int)value);
			}
		}
		private int m_nAlignment = (int)AlignType.kLeft;
		#endregion
		#region BAttr{g/s}: double FirstLineIndent - Inches for first line (e.g., .125 = 1/8")
		public double FirstLineIndent
		{
			get
			{
				return m_dblFirstLineIndent;
			}
			set
			{
                SetValue(ref m_dblFirstLineIndent, value);
			}
		}
		private double m_dblFirstLineIndent = 0.0;
		#endregion
		#region BAttr{g/s}: double LeftMargin - Inches for left margin
		public double LeftMargin
		{
			get
			{
				return m_dblLeftMargin;
			}
			set
			{
                SetValue(ref m_dblLeftMargin, value);
			}
		}
		private double m_dblLeftMargin = 0.0;
		#endregion
		#region BAttr{g/s}: double RightMargin - Inches for right margin
		public double RightMargin
		{
			get
			{
				return m_dblRightMargin;
			}
			set
			{
                SetValue(ref m_dblRightMargin, value);
			}
		}
		private double m_dblRightMargin = 0.0;
		#endregion
		#region BAttr{g/s}: string SFM - the SFM marker for the style
		public string SFM
		{
			get
			{
				return m_sSFM;
			}
			set
			{
                SetValue(ref m_sSFM, value);
			}
		}
		private string m_sSFM = "";
		#endregion
		#region BAttr{g/s}: string DisplayName - the style's name as it appears in the UI
		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
                SetValue(ref m_sDisplayName, value);
			}
		}
		private string m_sDisplayName = "";
		#endregion
		#region BAttr{g/s}: string Description - UI-displayable description for the style
		public string Description
		{
			get
			{
				return m_sDescription;
			}
			set
			{
                SetValue(ref m_sDescription, value);
			}
		}
		private string m_sDescription = "";
		#endregion
		#region BAttr{g/s}: bool KeepWithNext - if T, must be on same page as next para
		public bool KeepWithNext
		{
			get
			{
				return m_bKeepWithNext;
			}
			set
			{
                SetValue(ref m_bKeepWithNext, value);
			}
		}
		private bool m_bKeepWithNext = false;
		#endregion
		#region BAttr{g/s}: bool Bulleted - if T, has a bullet to the "left"
		public bool Bulleted
		{
			get
			{
				return m_bBulleted;
			}
			set
			{
				SetValue(ref m_bBulleted, value);
			}
		}
		private bool m_bBulleted = false;
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("SpaceBefore",     ref m_ptSpaceBefore);
			DefineAttr("SpaceAfter",      ref m_ptSpaceAfter);
			DefineAttr("Alignment",       ref m_nAlignment);
			DefineAttr("FirstLineIndent", ref m_dblFirstLineIndent);
			DefineAttr("LeftMargin",      ref m_dblLeftMargin);
			DefineAttr("RightMargin",     ref m_dblRightMargin);
			DefineAttr("Abbrev",          ref m_sSFM);
			DefineAttr("DisplayName",     ref m_sDisplayName);
			DefineAttr("Description",     ref m_sDescription);
			DefineAttr("Keep",            ref m_bKeepWithNext);
			DefineAttr("Bullet",          ref m_bBulleted);
		}
		#endregion

		// JAttributes: General --------------------------------------------------------------
		#region JAttr{g}: DCharacterStyle CharacterStyle - the char style for the pragraph
		public JCharacterStyle CharacterStyle
		{
			get 
			{ 
				return j_CharacterStyle.Value; 
			}
		}
		private JOwn<JCharacterStyle> j_CharacterStyle = null; 
		#endregion

		// Derived Attributes ----------------------------------------------------------------
		#region VAttr{g}: JStyleSheet StyleSheet - returns the StyleSheet that owns this style
		public JStyleSheet StyleSheet
		{
			get
			{
				JStyleSheet stylesheet = (JStyleSheet)Owner;
				Debug.Assert(null != stylesheet);
				return stylesheet;
			}
		}
		#endregion
		#region VAttr{g/s}: book IsLeft - paragraph is left-aligned
		public bool IsLeft
		{
			get 
			{ 
				return Alignment == AlignType.kLeft; 
			}
			set 
			{ 
				Alignment = AlignType.kLeft; 
			}
		}
		#endregion
		#region VAttr{g/s}: book IsRight - paragraph is right-aligned
		public bool IsRight
		{
			get 
			{ 
				return Alignment == AlignType.kRight; 
			}
			set 
			{
				Alignment = AlignType.kRight; 
			}
		}
		#endregion
		#region VAttr{g/s}: book IsCentered - paragraph is centered
		public bool IsCentered
		{
			get 
			{ 
				return Alignment == AlignType.kCentered; 
			}
			set 
			{ 
				Alignment = AlignType.kCentered; 
			}
		}
		#endregion
		#region VAttr{g/s}: book IsJustified - paragraph is justified
		public bool IsJustified
		{
			get 
			{ 
				return Alignment == AlignType.kJustified; 
			}
			set 
			{
				Alignment = AlignType.kJustified; 
			}
		}
		#endregion

		// Character Style Attributes --------------------------------------------------------
		#region Method: void SetFonts(bool bSerif, int nSize, bool bIsBold)
		public void SetFonts(bool bSerif, int nSize, bool bIsBold)
		{
			CharacterStyle.SetFonts(bSerif, nSize, bIsBold);
		}
		#endregion
		#region Method: void SetFonts(bSerif, nSize, bBold, bItalic, bStrike, bUnder, clrText)
		public void SetFonts(bool bSerif, int nSize, bool bIsBold, bool bIsItalic, 
			bool bIsStrikeout, bool bIsUnderline, Color colorText)
		{
			CharacterStyle.SetFonts(bSerif, nSize, bIsBold, bIsItalic, bIsStrikeout,
				bIsUnderline, colorText);
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor() - no parameters, used only for reading
		public JParagraphStyle()
			: base()
		{
            j_CharacterStyle = new JOwn<JCharacterStyle>("CharStyle", this);
            j_CharacterStyle.Value = new JCharacterStyle();
		}
		#endregion
		#region Constructor(sAbbrev, sDisplayName)
		public JParagraphStyle(string sAbbrev, string sDisplayName)
			: this()
		{
			SFM = sAbbrev;
			DisplayName = sDisplayName;
		}
		#endregion

		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			JParagraphStyle style = (JParagraphStyle)obj;
			return style.SFM == this.SFM;
		}
		#endregion
		#region Attribute(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return SFM; 
			}
		}
		#endregion
	}
	#endregion

    #region CLASS JFontForWritingSystem
    public class JFontForWritingSystem : JObject
    {
        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string FontName - the name of the Font
        public string FontName
        {
            get
            {
                return m_sFontName;
            }
            set
            {
                SetValue(ref m_sFontName, value);
            }
        }
        private string m_sFontName = "Arial";
        #endregion
        #region BAttr{g/s}: int Size - The height of the font, default is 10
        public int Size
        {
            get
            {
                return m_nSize;
            }
            set
            {
                SetValue(ref m_nSize, value);
            }
        }
        private int m_nSize = 10;
        #endregion
        #region BAttr{g/s}: bool IsBold - T if the font is bold
        public bool IsBold
        {
            get
            {
                return m_bIsBold;
            }
            set
            {
                SetValue(ref m_bIsBold, value);
            }
        }
        private bool m_bIsBold = false;
        #endregion
        #region BAttr{g/s}: bool IsItalic - T if the font is italic
        public bool IsItalic
        {
            get
            {
                return m_bIsItalic;
            }
            set
            {
                SetValue(ref m_bIsItalic, value);
            }
        }
        private bool m_bIsItalic = false;
        #endregion
        #region BAttr{g/s}: bool IsStrikeout - T if the font is strike out
        public bool IsStrikeout
        {
            get
            {
                return m_bStrikeout;
            }
            set
            {
                SetValue(ref m_bStrikeout, value);
            }
        }
        private bool m_bStrikeout = false;
        #endregion
        #region Method: void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Name", ref m_sFontName);
            DefineAttr("Size", ref m_nSize);
            DefineAttr("Bold", ref m_bIsBold);
            DefineAttr("Italic", ref m_bIsItalic);
            DefineAttr("Strike", ref m_bStrikeout);
        }
        #endregion

        // JAttrs ----------------------------------------------------------------------------
        #region JAttr{g/s}: JWritingSystem WritingSystem - reference to a JWritingSystem
        public JWritingSystem WritingSystem
        {
            get
            {
                Debug.Assert(null != j_refWritingSystem);
                Debug.Assert(null != j_refWritingSystem.Value);
                return j_refWritingSystem.Value;
            }
            set
            {
                j_refWritingSystem.Value = value;
            }
        }
        JRef<JWritingSystem> j_refWritingSystem;
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: JCharacterStyle CharacterStyle
        public JCharacterStyle CharacterStyle
        {
            get
            {
                Debug.Assert(null != Owner && null != Owner as JCharacterStyle);
                return Owner as JCharacterStyle;
            }
        }
        #endregion
        #region VAttr{g}: Color FontColor - color for text; default is black
        public Color FontColor
        {
            get
            {
                return CharacterStyle.FontColor;
            }
        }
        #endregion

        // Fonts (created On-Demand) ---------------------------------------------------------
        #region CLASS: FontZoomed - Adds the Zoomed attribute
        class FontZoomed
        {
            #region Attr{g}: bool Zoomed
            public bool Zoomed
            {
                get
                {
                    return m_bZoomed;
                }
            }
            bool m_bZoomed = false;
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
            Font m_Font;
            #endregion

            #region Constroctor(bZoomed, sFontName, fSize, mods, fZoomFactor)
            public FontZoomed(bool bZoomed, 
                string sFontName, 
                float fSize, 
                FontStyle mods,
                float fZoomFactor)
            {
                m_bZoomed = bZoomed;

                if (bZoomed)
                    fSize *= fZoomFactor;

                m_Font = new Font(sFontName, fSize, mods);

            }
            #endregion
        }
        #endregion
        #region Attr{g}: FontZoomed[] Fonts
        FontZoomed[] Fonts
        {
            get
            {
                Debug.Assert(null != m_vFonts);
                return m_vFonts;
            }
        }
        FontZoomed[] m_vFonts;
        #endregion 
        #region Method: void ClearFonts()
        public void ClearFonts()
        {
            // Clear the array
            m_vFonts = new FontZoomed[0];

            // Clear the optimizations
            m_DefaultFont = null;
            m_DefaultFontZoomed = null;
        }
        #endregion  
        #region Method: Font FindOrAddFont(bool bZoom, FontStyle mods)
        public Font FindOrAddFont(bool bZoom, FontStyle mods)
        {
            // Determine the desired mods, as an additive merge between the
            // font's settings, and the passed-in mods. Thus mods are always
            // additive; they don't turn off a feature once it has been turned
            // on in the Style.
            if (IsBold)
                mods |= FontStyle.Bold;
            if (IsItalic)
                mods |= FontStyle.Italic;
            if (IsStrikeout)
                mods |= FontStyle.Strikeout;

            // Return the existing one if it is there
            foreach (FontZoomed f in Fonts)
            {
                if (f.Zoomed == bZoom && f.Font.Style == mods)
                    return f.Font;
            }

            // Otherwise create it
            FontZoomed fz = new FontZoomed(bZoom, FontName, (float)Size, mods,
                CharacterStyle.StyleSheet.ZoomFactor);

            // Add it to the vector
            FontZoomed[] v = new FontZoomed[Fonts.Length + 1];
            for (int i = 0; i < Fonts.Length; i++)
                v[i] = Fonts[i];
            v[Fonts.Length] = fz;
            m_vFonts = v;

            return fz.Font;
        }
        #endregion

        #region VAttr{g}: float LineHeight
        public float LineHeight
        {
            get
            {
                return (float)FindOrAddFont(false, FontStyle.Regular).Height;
            }
        }
        #endregion
        #region VAttr{g}: float LineHeightZoomed
        public float LineHeightZoomed
        {
            get
            {
                int height = FindOrAddFont(true, FontStyle.Regular).Height;
                if (height == 0)
                    MessageBox.Show("Height = 0");
                return (float)height;
            }
        }
        #endregion

        #region VAttr{g}: Font DefaultFont - the styled font, without any forced mods
        public Font DefaultFont
        {
            get
            {
                if (null == m_DefaultFont)
                    m_DefaultFont= FindOrAddFont(false, FontStyle.Regular);
                return m_DefaultFont;
            }
        }
        Font m_DefaultFont = null;
        #endregion
        #region VAttr[g}: Font DefaultFontZoomed - the styled font, without any forced mods
        public Font DefaultFontZoomed
        {
            get
            {
                if (null == m_DefaultFontZoomed)
                    m_DefaultFontZoomed = FindOrAddFont(true, FontStyle.Regular);
                return m_DefaultFontZoomed;
            }
        }
        Font m_DefaultFontZoomed = null;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor() - uses default values
        public JFontForWritingSystem()
			: base()
		{
            ClearFonts();
            j_refWritingSystem = new JRef<JWritingSystem>("ws", this);
		}
		#endregion
        #region Constructor(JWritingSystem) 
        public JFontForWritingSystem(JWritingSystem ws)
            : this()
        {
            WritingSystem = ws;
        }
        #endregion
        #region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
        public override string SortKey
        // In order to support sorting, the subclass must implement a SortKey attribute,
        // and this SortKey must return something other than an empty string. 
        {
            get
            {
                return WritingSystem.Name + "-" + 
                    FontName + "-" + 
                    Size.ToString() + "-" + 
                    IsBold.ToString() + "-" + 
                    IsItalic.ToString() + "-" +
                    IsStrikeout.ToString();
            }
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

			JFontForWritingSystem ws = obj as JFontForWritingSystem;
			if (ws == null)
				return false;

			if (ws.WritingSystem.Name != WritingSystem.Name)
				return false;
			if (ws.FontName != FontName)
				return false;
			if (ws.Size != Size)
				return false;
			if (ws.IsBold != IsBold)
				return false;
			if (ws.IsItalic != IsItalic)
				return false;
			if (ws.IsStrikeout != IsStrikeout)
				return false;

            return true;
        }
        #endregion
    }
    #endregion

    #region CLASS JCharacterStyle
    public class JCharacterStyle : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: bool IsSuperScript - T if the characters are superscripted
		public bool IsSuperScript
		{
			get
			{
				return m_bIsSuperScript;
			}
			set
			{
                SetValue(ref m_bIsSuperScript, value);
            }
		}
		private bool m_bIsSuperScript = false;
		#endregion
		#region BAttr{g/s}: bool IsSubScript   - T if the characters are subscripted
		public bool IsSubScript
		{
			get
			{
				return m_bIsSubScript;
			}
			set
			{
                SetValue(ref m_bIsSubScript, value);
			}
		}
		private bool m_bIsSubScript = false;
		#endregion
		#region BAttr{g/s}: bool IsEditable    - T if the characters are allowed to be edited
		public bool IsEditable
		{
			get
			{
				return m_bIsEditable;
			}
			set
			{
                SetValue(ref m_bIsEditable, value);
			}
		}
		private bool m_bIsEditable = true;
		#endregion
		#region BAttr{g/s}: string Abbrev - an abbreviation for the style, used in inline text
		public string Abbrev
		{
			get
			{
				return m_sAbbrev;
			}
			set
			{
                SetValue(ref m_sAbbrev, value);
            }
		}
		private string m_sAbbrev = "";
		#endregion
		#region BAttr{g/s}: string DisplayName - the style's name as it appears in the UI
		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
                SetValue(ref m_sDisplayName, value);
			}
		}
		private string m_sDisplayName = "";
		#endregion
		#region BAttr{g/s}: string Description - UI-displayable description for the style
		public string Description
		{
			get
			{
				return m_sDescription;
			}
			set
			{
                SetValue(ref m_sDescription, value);
			}
		}
		private string m_sDescription = "";
		#endregion

        #region BAttr{g/s}: int SizeByDefault - The height of the font, default is 10
        public int SizeByDefault
        {
            get
            {
                return m_nSizeByDefault;
            }
            set
            {
                SetValue(ref m_nSizeByDefault, value);
            }
        }
        private int m_nSizeByDefault = 10;
        #endregion
        #region BAttr{g/s}: bool IsBoldByDefault - T if the font is bold
        public bool IsBoldByDefault
        {
            get
            {
                return m_bIsBoldByDefault;
            }
            set
            {
                SetValue(ref m_bIsBoldByDefault, value);
            }
        }
        private bool m_bIsBoldByDefault = false;
        #endregion
        #region BAttr{g/s}: bool IsItalicByDefault - T if the font is italic
        public bool IsItalicByDefault
        {
            get
            {
                return m_bIsItalicByDefault;
            }
            set
            {
                SetValue(ref m_bIsItalicByDefault, value);
            }
        }
        private bool m_bIsItalicByDefault = false;
        #endregion
        #region BAttr{g/s}: bool IsStrikeoutByDefault - T if the font is italic
        public bool IsStrikeoutByDefault
        {
            get
            {
                return m_bStrikeoutByDefault;
            }
            set
            {
                SetValue(ref m_bStrikeoutByDefault, value);
            }
        }
        private bool m_bStrikeoutByDefault = false;
        #endregion

        #region BAttr{g/s}: Color FontColor - color for text; default is black
        public Color FontColor
        {
            get
            {
                Color color = Color.FromName(m_sColorName);
                return color;
            }
            set
            {
                SetValue(ref m_sColorName, value.Name);
            }
        }
        private string m_sColorName = Color.Black.Name;
        #endregion

        #region BAttr{g/s}: bool IsUnderline - T if the font is strikeout
        public bool IsUnderline
        {
            get
            {
                return m_bIsUnderline;
            }
            set
            {
                SetValue(ref m_bIsUnderline, value);
            }
        }
        private bool m_bIsUnderline = false;
        #endregion

		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("Super",       ref m_bIsSuperScript);
			DefineAttr("Sub",         ref m_bIsSubScript);
			DefineAttr("Editable",    ref m_bIsEditable);
			DefineAttr("Abbrev",      ref m_sAbbrev);
			DefineAttr("DisplayName", ref m_sDisplayName);
			DefineAttr("Description", ref m_sDescription);

            DefineAttr("Height",    ref m_nSizeByDefault);
            DefineAttr("Bold",      ref m_bIsBoldByDefault);
            DefineAttr("Italic",    ref m_bIsItalicByDefault);
            DefineAttr("Strikeout", ref m_bStrikeoutByDefault);
            DefineAttr("Underline", ref m_bIsUnderline);
            DefineAttr("FontColor", ref m_sColorName);
        }
		#endregion

        // FontsForWritingSystems ------------------------------------------------------------
        #region JAttr{g}: JOwnSeq FontsForWritingSystems - the list of FontsForWritingSystems
        public JOwnSeq<JFontForWritingSystem> FontsForWritingSystems
        {
            get
            {
                return j_osFontsForWritingSystems;
            }
        }
        private JOwnSeq<JFontForWritingSystem> j_osFontsForWritingSystems = null;
        #endregion
        #region Method: JFontForWritingSystem FindOrAddFontForWritingSystem(ws)
        public JFontForWritingSystem FindOrAddFontForWritingSystem(JWritingSystem ws)
        {
            foreach (JFontForWritingSystem fws in FontsForWritingSystems)
            {
                if (fws.WritingSystem == ws)
                    return fws;
            }

            JFontForWritingSystem newFWS = new JFontForWritingSystem(ws);
            newFWS.IsBold = IsBoldByDefault;
            newFWS.IsItalic = IsItalicByDefault;
            newFWS.IsStrikeout = IsStrikeoutByDefault;
            newFWS.Size = SizeByDefault;
            FontsForWritingSystems.Append(newFWS);

            return newFWS;
        }
        #endregion
        #region Method: void EnsureFontsForWritingSystems()
        public void EnsureFontsForWritingSystems()
        {
            foreach (JWritingSystem ws in StyleSheet.WritingSystems)
            {
                FindOrAddFontForWritingSystem(ws);
            }
        }
        #endregion
        #region Method: void ResetFonts()
        public void ResetFonts()
        {
            foreach (JFontForWritingSystem fws in FontsForWritingSystems)
                fws.ClearFonts();
        }
        #endregion

		// Derived Attributes: General--------------------------------------------------------
		#region VAttr{g}: JStyleSheet StyleSheet - returns the StyleSheet that owns this style
		public JStyleSheet StyleSheet
		{
			get
			{
				if (Owner.GetType() == typeof(JParagraphStyle))
				{
					JParagraphStyle paraStyle = (JParagraphStyle)Owner;
					Debug.Assert(null != paraStyle);
					Debug.Assert(null != paraStyle.StyleSheet);
					return paraStyle.StyleSheet;
				}
				else
				{
					JStyleSheet stylesheet = (JStyleSheet)Owner;
					Debug.Assert(null != stylesheet);
					return stylesheet;
				}
			}
		}
		#endregion

		#region Method: void SetFonts(bool bSerif, int nSize, bool bIsBold)
		public void SetFonts(bool bSerif, int nSize, bool bIsBold)
		{
			SetFonts(bSerif, nSize, bIsBold, false, false, false, Color.Black);
		}
		#endregion
		#region Method: void SetFonts(bSerif, nSize, bBold, bItalic, bStrike, bUnder, clrText)
		public void SetFonts(bool bSerif, int nSize, bool bIsBold, bool bIsItalic, 
			bool bIsStrikeout, bool bIsUnderline, Color colorText)
		{
            // These will seed the FWS objects as they are created
            m_nSizeByDefault = nSize;
            m_bIsBoldByDefault = bIsBold;
            m_bIsItalicByDefault = bIsItalic;
            m_bStrikeoutByDefault = bIsStrikeout;

            m_bIsUnderline = bIsUnderline;

            FontColor = colorText;
		}
		#endregion

        #region VAttr{g}: JParagraphStyle ParagraphStyle - The owning PStyle, or null
        public JParagraphStyle ParagraphStyle
        {
            get
            {
                foreach (JParagraphStyle ps in StyleSheet.ParagraphStyles)
                {
                    if (ps.CharacterStyle == this)
                        return ps;
                }
                return null;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor() - uses default values
		public JCharacterStyle()
			: base()
		{
            j_osFontsForWritingSystems = new JOwnSeq<JFontForWritingSystem>("fws", this);
        }
		#endregion
		#region Constructor(sAbbrev, sDisplayName) 
		public JCharacterStyle(string sAbbrev, string sDisplayName)
			: this()
		{
			Abbrev = sAbbrev;
			DisplayName = sDisplayName;
		}
		#endregion

		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			JCharacterStyle style = (JCharacterStyle)obj;
			return style.Abbrev == this.Abbrev;
		}
		#endregion
		#region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return Abbrev; 
			}
		}
		#endregion
	}
	#endregion

	#region CLASS: JWritingSystem
	public class JWritingSystem : JObject
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: bool IsIdeaGraph - a glyph equals a word, e.g., Chinese script
		public bool IsIdeaGraph
		{
			get
			{
				return m_bIsIdeaGraph;
			}
			set
			{
                SetValue(ref m_bIsIdeaGraph, value);
			}
		}
		private bool m_bIsIdeaGraph = false;
		#endregion
		#region BAttr{g/s}: string Name - the name of the writing system
		public string Name
		{
			get 
			{ 
				return m_sName;
			}
			set 
			{
                SetValue(ref m_sName, value);
			}
		}
		private string m_sName = "";
		#endregion
        #region BAttr{g/s}: string Abbrev - the name of the writing system
        public string Abbrev
        {
            get
            {
                return m_sAbbrev;
            }
            set
            {
                SetValue(ref m_sAbbrev, value);
            }
        }
        private string m_sAbbrev = "";
        #endregion
        #region BAttr{g/s}: string PunctuationChars - a list of punctuation characters for this WS
		public string PunctuationChars
		{
			get 
			{
				return m_sPunctuationChars;
			}
			set 
			{
                SetValue(ref m_sPunctuationChars, value);
			}
		}
        private string m_sPunctuationChars = c_sDefaultPunctuationChars;
        public const string c_sDefaultPunctuationChars = "“‘.,;:<>?!()[]’”";
		#endregion
		#region BAttr{g/s}: string EndPunctuationChars - punctuation that occurs sentence-final.
		public string EndPunctuationChars
		{
			get 
			{
				return m_sEndPunctuationChars;
			}
			set 
			{
                SetValue(ref m_sEndPunctuationChars, value);
			}
		}
        private string m_sEndPunctuationChars = c_sDefaultEndPunctuationChars;
        public const string c_sDefaultEndPunctuationChars = ".>?!)],:;’”";
		#endregion
		#region BAttr{g/s}: BStringArray AutoReplaceSource
		public BStringArray AutoReplaceSource
		{
			get
			{
				return m_bsaAutoReplaceSource;
			}
			set
			{
                SetValue(ref m_bsaAutoReplaceSource, value);
			}
		}
		private BStringArray m_bsaAutoReplaceSource = null;
		#endregion
		#region BAttr{g/s}: BStringArray AutoReplaceResult
		public BStringArray AutoReplaceResult
		{
			get
			{
				return m_bsaAutoReplaceResult;
			}
			set
			{
                SetValue(ref m_bsaAutoReplaceResult, value);
			}
		}
		private BStringArray m_bsaAutoReplaceResult = null;
		#endregion
        #region BAttr{g/s}: string KeyboardName - the name of the keyboard to use with this WS
        public string KeyboardName
        {
            get
            {
                return m_sKeyboardName;
            }
            set
            {
                SetValue(ref m_sKeyboardName, value);
            }
        }
        private string m_sKeyboardName = "";
        #endregion
        // Auto Hyphen Attrs
        #region BAttr{g/s}: string Consonants - a list of Consonants for this WS
        public string Consonants
        {
            get
            {
                return m_sConsonants;
            }
            set
            {
                SetValue(ref m_sConsonants, value);
            }
        }
        private string m_sConsonants = c_sDefaultConsonants;
        public const string c_sDefaultConsonants = "bcdfghjklmnpqrstvwxyz";
        #endregion
        #region BAttr{g/s}: bool UseAutomatedHyphenation
        public bool UseAutomatedHyphenation
        {
            get
            {
                return m_bUseAutomatedHyphenation;
            }
            set
            {
                SetValue(ref m_bUseAutomatedHyphenation, value);
            }
        }
        private bool m_bUseAutomatedHyphenation = false;
        #endregion
        #region BAttr{g/s}: string HyphenationCVPattern - E.g., "V-C", or "VC-CV"
        public string HyphenationCVPattern
        {
            get
            {
                return m_sHyphenationCVPattern;
            }
            set
            {
                SetValue(ref m_sHyphenationCVPattern, value);
            }
        }
        private string m_sHyphenationCVPattern = c_sHyphenationCVPattern;
        public const string c_sHyphenationCVPattern = "V-C";
        #endregion
        #region BAttr{g/s}: int MinHyphenSplit - points of space before a paragraph
        public int MinHyphenSplit
        {
            get
            {
                return m_nMinHyphenSplit;
            }
            set
            {
                SetValue(ref m_nMinHyphenSplit, value);
            }
        }
        private int m_nMinHyphenSplit = 3;
        #endregion
        // DeclareAttrs
        #region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("IdeaGraph",  ref m_bIsIdeaGraph);
			DefineAttr("Name",       ref m_sName);
            DefineAttr("Abbrev",     ref m_sAbbrev);
            DefineAttr("Punct",      ref m_sPunctuationChars);
			DefineAttr("EndPunct",   ref m_sEndPunctuationChars);
            DefineAttr("Consonants", ref m_sConsonants);
            DefineAttr("AutoHyph",   ref m_bUseAutomatedHyphenation);
            DefineAttr("AutoHyphCV", ref m_sHyphenationCVPattern);
            DefineAttr("AutoHyphMinSplit", ref m_nMinHyphenSplit);
            DefineAttr("ARSource",   ref m_bsaAutoReplaceSource);
			DefineAttr("ARResult",   ref m_bsaAutoReplaceResult);
            DefineAttr("Keyboard",   ref m_sKeyboardName);
		}
		#endregion

		// Derived Attributes ----------------------------------------------------------------
		#region VAttr(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return Name; 
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor() - no parameters, used only for reading
		public JWritingSystem()
			: base()
		{
			ConstructAttrs();
		}
		#endregion
		#region Constructor(sName)
		public JWritingSystem(string sName)
			: base()
		{
			ConstructAttrs();
			Name = sName;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			JWritingSystem ws = (JWritingSystem)obj;
			return ws.Name == this.Name;
		}
		#endregion
		#region Method: void ConstructAttrs()
		private void ConstructAttrs()
		{

			string[] rgSource = 
			{ 
				// `V
				"`a",     "`A",     "`e",     "`E",     "`i",     "`I",   
				"`o",     "`O",     "`u",     "`U" ,
				// ^V
				"^a",     "^A",     "^e",     "^E",     "^i",     "^I",   
				"^o",     "^O",     "^u",     "^U"   
			};
			string[] rgResult = 
			{ 
				// `V
				"\u00E0", "\u00C0", "\u00E8", "\u00C8", "\u00EC", "\u00CC", 
				"\u00F2", "\u00D2", "\u00F9", "\u00D9",
				// ^V
				"\u00E1", "\u00C1", "\u00E9", "\u00C9", "\u00ED", "\u00CD", 
				"\u00F3", "\u00D3", "\u00FA", "\u00DA" 
			};

			m_bsaAutoReplaceSource = new BStringArray(rgSource);
			m_bsaAutoReplaceResult = new BStringArray(rgResult);

			BuildAutoReplace();
		}
		#endregion

        // AutoReplace -----------------------------------------------------------------------
        #region Attr{g}: TreeRoot AutoReplace
        public TreeRoot AutoReplace
        {
            get
            {
                Debug.Assert(null != m_AutoReplace);
                return m_AutoReplace;
            }
        }
		TreeRoot m_AutoReplace = null;
        #endregion
        #region Method: void BuildAutoReplace()
        public void BuildAutoReplace()
		{
			m_AutoReplace = new TreeRoot();
			for(int i=0; i<AutoReplaceSource.Length; i++)
			{
				m_AutoReplace.Add( AutoReplaceSource[i], AutoReplaceResult[i] );
			}
		}
		#endregion
		#region Method: string SearchAutoReplace(string sSource, ref int cSourceLen)
		public string SearchAutoReplace(string sSource, ref int cSourceLen)
		{
			if (null == m_AutoReplace)
				BuildAutoReplace();
			return m_AutoReplace.Search(sSource, ref cSourceLen);
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: bool IsPunctuation(char) - T if char is a punct character
		public bool IsPunctuation(char cTest)
		{
			foreach( char c in PunctuationChars)
			{
				if (c == cTest)
					return true;
			}
			return false;
		}
		#endregion
        #region Method: bool IsEndPunctuation(char) - T if char is a punct character
        public bool IsEndPunctuation(char cTest)
        {
            foreach (char c in EndPunctuationChars)
            {
                if (c == cTest)
                    return true;
            }
            return false;
        }
        #endregion
        #region Method: bool IsWhiteSpace(char) - T if char is a whitespace character
        public bool IsWhiteSpace(char ch)
        {
            if (ch == ' ')
                return true;
            return false;
        }
        #endregion
        #region Method: bool IsWordBreak(s, iPos)
        public bool IsWordBreak(string s, int iPos)
            // At this point, I define a word break as occuring at either the beginning of the
            // string, or following a whitespace. This method is more of a place-holder, as I
            // anticipate that there will be writing systems where other types of word break
            // rules will be necessary.
        {
            // Make sure we have valid input
            Debug.Assert(iPos >= 0);
            Debug.Assert(iPos < s.Length);
            Debug.Assert(null != s);

            // If at beginning of string, this is a word break.
            if (iPos == 0)
                return true;

            // Get the two characters of interest
            char chWhitespace = s[iPos - 1];
            char chNonWhitespace = s[iPos];

            // Do the test
            if (char.IsWhiteSpace(chWhitespace) && !char.IsWhiteSpace(chNonWhitespace))
                return true;

            return false;
        }
        #endregion
        #region Method: bool IsHyphenBreak(sWord, iPosWithinWord)
        public bool IsHyphenBreak(string s, int iPos)
            /* OK, this is quick-and-dirty, to see if I can get a simple hyphenation
             * support working. I will need to do more than this. But this implementation
             * will be that I can hyphen if:
             * - I'm four letters from either end of a word
             * - I'm located at a consonant
             * - The preveeding letter is not a consonant
             * 
             * E.g., this is a V-C type of rule.
             * 
             * I'm going to need to install that ICU stuff (sigh) to do this right.
             * Pity the poor user that must download it.
             * 
             * Or maybe I can do something where users can enter rules for simple
             * stuff. Joe tells me that Huichol hyphens after CV's. So saying CV-
             * plus enumerating the consonants is sufficient for Huichol.
             */
        {
            // Don't bother if automated hyphenation is not turned on
            if (UseAutomatedHyphenation == false)
                return false;

            // 1 - Don't be too close to an end of a word
            if (iPos < MinHyphenSplit)
                return false;
            if (iPos > s.Length - MinHyphenSplit)
                return false;

            // Find the position of the hyphen in our string
            int iHyphenPos = HyphenationCVPattern.IndexOf('-');
            if (-1 == iHyphenPos)
                return false;

            // Get the pattern string, minus the hyphen
            string sPattern = HyphenationCVPattern.Remove(iHyphenPos, 1);

            // Determine where to start, and if we have room for the test
            int iStart = iPos - iHyphenPos;
            if (iStart < 0)
                return false;
            int cPositions = sPattern.Length;
            if (iStart + cPositions > s.Length)
                return false;

            // Check against our pattern
            for(int k=0; k<cPositions; k++)
            {
                char chPattern = sPattern[k];
                char chTest = s[iStart + k];

                // Sitting on a consonant?
                if (char.ToUpper(chPattern) == 'C')
                {
                    if (Consonants.IndexOf( char.ToLower(chTest)) == -1)
                        return false;
                }

                // Sitting on a vowel?
                if (char.ToUpper(chPattern) == 'V')
                {
                    if (Consonants.IndexOf( char.ToLower(chTest)) != -1)
                        return false;
                }
            }

            return true;
        }
        #endregion

		// I/O -------------------------------------------------------------------------------
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            base.FromXml(x);

            BuildAutoReplace();
        }
        #endregion

    }
	#endregion
}
