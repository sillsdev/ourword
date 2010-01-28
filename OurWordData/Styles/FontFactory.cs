#region ***** FontFactory.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    FontFactory.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Each character style stores a font for each of the writing systems. This permits,
 *          e.g., a larger size for writing systems that need it.
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class FontFactory
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: string FontName
        public string FontName
        {
            get
            {
                return m_sFontName;
            }
            set
            {
                if (m_sFontName == value || string.IsNullOrEmpty(value))
                    return;
                m_sFontName = value;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sFontName = "Arial";
        #endregion
        #region Attr{g/s}: float FontSize
        public float FontSize
        {
            get
            {
                return m_fFontSize;
            }
            set
            {
                if (m_fFontSize == value)
                    return;
                m_fFontSize = value;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        private float m_fFontSize = 10;
        #endregion
        #region Attr{g/s}: FontStyle FontStyle
        public FontStyle FontStyle
        {
            get
            {
                return m_FontStyle;
            }
            set
            {
                if (m_FontStyle == value)
                    return;
                m_FontStyle = value;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        private FontStyle m_FontStyle;
        #endregion
        #region Attr{g/s}: string WritingSystemName
        public string WritingSystemName
        {
            get
            {
                return m_sWritingSystemName;
            }
            set
            {
                if (m_sWritingSystemName == value || string.IsNullOrEmpty(value))
                    return;
                m_sWritingSystemName = value;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sWritingSystemName = "Latin";
        #endregion

        // Convient shorthand ----------------------------------------------------------------
        #region Attr{g/s}: bool IsBold
        public bool IsBold
        {
            get
            {
                return ((FontStyle & FontStyle.Bold) == FontStyle.Bold);
            }
            set
            {
                if (value)
                    FontStyle |= FontStyle.Bold;
                else if (IsBold)
                    FontStyle ^= FontStyle.Bold;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        #endregion
        #region Attr{g/s}: bool IsItalic
        public bool IsItalic
        {
            get
            {
                return ((FontStyle & FontStyle.Italic) == FontStyle.Italic);
            }
            set
            {
                if (value)
                    FontStyle |= FontStyle.Italic;
                else if (IsBold)
                    FontStyle ^= FontStyle.Italic;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        #endregion
        #region Attr{g/s}: bool IsUnderline
        public bool IsUnderline
        {
            get
            {
                return ((FontStyle & FontStyle.Underline) == FontStyle.Underline);
            }
            set
            {
                if (value)
                    FontStyle |= FontStyle.Underline;
                else if (IsBold)
                    FontStyle ^= FontStyle.Underline;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        #endregion
        #region Attr{g/s}: bool IsStrikeout
        public bool IsStrikeout
        {
            get
            {
                return ((FontStyle & FontStyle.Strikeout) == FontStyle.Strikeout);
            }
            set
            {
                if (value)
                    FontStyle |= FontStyle.Strikeout;
                else if (IsBold)
                    FontStyle ^= FontStyle.Strikeout;
                ResetFonts();
                StyleSheet.DeclareDirty();
            }
        }
        #endregion

        // Fonts Dictionary ------------------------------------------------------------------
        private readonly Dictionary<string, Font> m_FontsDictionary;
        #region SMethod: string MakeKey(FontStyle toggles, float zoomPercent)
        static protected string MakeKey(FontStyle toggles, float zoomPercent)
        {
            var sKey = zoomPercent + "-";

            if (toggles == FontStyle.Regular)
                sKey += "Regular";
            else
                sKey += GetFontStyleAsString(toggles);

            return sKey;
        }
        #endregion
        #region Method: FontStyle GetToggledFontStyle(FontStyle toggles)
        protected FontStyle GetToggledFontStyle(FontStyle toggles)
            // To determine the FontStyle we want, we wish to toggle anything that
            // is set in the "toggles" parameter.
            //
            // 1010 ^ 0010 = 1000
        {
            return FontStyle ^ toggles;
        }
        #endregion
        #region Method: Font GetFont(FontStyle toggles, float zoomPercent)
        public Font GetFont(FontStyle toggles, float zoomPercent)
        {
            var actualFontStyle = GetToggledFontStyle(toggles);

            var sKey = MakeKey(actualFontStyle, zoomPercent);

            Font font;
            if (!m_FontsDictionary.TryGetValue(sKey, out font))
            {
                var actualSize = FontSize * zoomPercent / 100F;

                font = new Font(FontName, actualSize, actualFontStyle);
                m_FontsDictionary.Add(sKey, font);
            }

            return font;
        }
        #endregion
        #region Method: Font GetFont(float zoomPercent)
        public Font GetFont(float zoomPercent)
        {
            return GetFont(FontStyle.Regular, zoomPercent);
        }
        #endregion
        #region Method: void ResetFonts()
        public void ResetFonts()
        {
            m_FontsDictionary.Clear();
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public FontFactory()
        {
            m_FontsDictionary = new Dictionary<string, Font>();
        }
        #endregion
        #region Method: FontFactory Clone()
        public FontFactory Clone()
        {
            return new FontFactory
            {
                WritingSystemName = WritingSystemName,
                FontName = FontName,
                FontSize = FontSize,
                FontStyle = FontStyle
            };
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        #region I/O Constants
        protected const string c_sTag = "Font";
        private const string c_sAttrWritingSystem = "WritingSystem";
        private const string c_sAttrFontName = "Name";
        private const string c_sAttrSize = "Size";
        private const string c_sAttrStyle = "Style";
        #endregion
        #region SMethod: string GetFontStyleAsString(FontStyle style)
        static public string GetFontStyleAsString(FontStyle style)
        {
            var s = "";

            if ((style & FontStyle.Bold) == FontStyle.Bold)
                s += c_sBold;
            if ((style & FontStyle.Italic) == FontStyle.Italic)
                s += c_sItalic;
            if ((style & FontStyle.Underline) == FontStyle.Underline)
                s += c_sUnderline;
            if ((style & FontStyle.Strikeout) == FontStyle.Strikeout)
                s += c_sStrikeout;

            return s;
        }
        #endregion
        #region SMethod: FontStyle GetFontStyleFromString(string s)
        static public FontStyle GetFontStyleFromString(string s)
        {
            var fs = new FontStyle();

            if (s.Contains(c_sBold))
                fs |= FontStyle.Bold;
            if (s.Contains(c_sItalic))
                fs |= FontStyle.Italic;
            if (s.Contains(c_sUnderline))
                fs |= FontStyle.Underline;
            if (s.Contains(c_sStrikeout))
                fs |= FontStyle.Strikeout;

            return fs;
        }
        #endregion
        #region Attr{g/s}: string FontStyleAsString
        protected string FontStyleAsString
        {
            get
            {
                return GetFontStyleAsString(FontStyle);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                FontStyle = GetFontStyleFromString(value);
            }
        }
        private const string c_sBold = "Bold";
        private const string c_sItalic = "Italic";
        private const string c_sUnderline = "Underline";
        private const string c_sStrikeout = "Strikeout";
        #endregion
        #region Method: XmlNode Save(XmlDoc, nodeParent)
        public XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        {
            var nodeFont = doc.AddNode(nodeParent, c_sTag);

            doc.AddAttr(nodeFont, c_sAttrWritingSystem, WritingSystemName);
            doc.AddAttr(nodeFont, c_sAttrFontName, FontName);
            doc.AddAttr(nodeFont, c_sAttrSize, FontSize);
            doc.AddAttr(nodeFont, c_sAttrStyle, FontStyleAsString);

            return nodeFont;
        }
        #endregion
        #region SMethod: FontFactory Create(nodeFont)
        static public FontFactory Create(XmlNode nodeFont)
        {
            if (nodeFont.Name != c_sTag)
                return null;

            var factory = new FontFactory
            {
                WritingSystemName = XmlDoc.GetAttrValue(nodeFont, c_sAttrWritingSystem, 
                    WritingSystem.DefaultWritingSystemName),
                FontName = XmlDoc.GetAttrValue(nodeFont, c_sAttrFontName, "Arial"),
                FontSize = XmlDoc.GetAttrValue(nodeFont, c_sAttrSize, 10.0F),
                FontStyleAsString = XmlDoc.GetAttrValue(nodeFont, c_sAttrStyle, "")
            };

            return factory;
        }
        #endregion
        #region Method: void Merge(parent, theirs)
        public void Merge(FontFactory parent, FontFactory theirs)
        {
            Debug.Assert(parent != null);
            Debug.Assert(theirs != null);

            // We require them to all be the same writing system (that's their Unique ID)
            Debug.Assert(WritingSystemName == parent.WritingSystemName);
            Debug.Assert(WritingSystemName == theirs.WritingSystemName);

            // Algorithm: We keep theirs iff they differ from ours and ours is
            // unchanged from the parent. Otherwise we always keep ours.
            if (FontName != theirs.FontName && FontName == parent.FontName)
                FontName = theirs.FontName;
            if (FontSize != theirs.FontSize && FontSize == parent.FontSize)
                FontSize = theirs.FontSize;
            if (FontStyle != theirs.FontStyle && FontStyle == parent.FontStyle)
                FontStyle = theirs.FontStyle;
        }
        #endregion
    }
}
