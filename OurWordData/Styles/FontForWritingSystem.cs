#region ***** FontForWritingSystem.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    FontForWritingSystem.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Each character style stores a font for each of the writing systems. This permits,
 *          e.g., a larger size for writing systems that need it.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class FontForWritingSystem
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
                Styles.DeclareDirty();
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
                Styles.DeclareDirty();
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
                Styles.DeclareDirty();
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
                Styles.DeclareDirty();
            }
        }
        private string m_sWritingSystemName = "Latin";
        #endregion

        // Fonts -----------------------------------------------------------------------------
        #region Attr{g}: Font UnzoomedFont
        public Font UnzoomedFont
        {
            get
            {
                if (null == m_UnzoomedFont)
                    m_UnzoomedFont = new Font(FontName, FontSize, FontStyle);
                return m_UnzoomedFont;
            }
        }
        private Font m_UnzoomedFont;
        #endregion
        #region Attr{g}: Font ZoomedFont
        public Font ZoomedFont
        {
            get
            {
                if (null == m_ZoomedFont)
                {
                    var zoomedSize = FontSize * 1.5F;
                    m_ZoomedFont = new Font(FontName, zoomedSize, FontStyle);
                }
                return m_ZoomedFont;
            }
        }
        private Font m_ZoomedFont;
        #endregion
        #region Method: void ResetFonts()
        public void ResetFonts()
        {
            m_UnzoomedFont = null;
            m_ZoomedFont = null;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Method: FontForWritingSystem Clone()
        public FontForWritingSystem Clone()
        {
            return new FontForWritingSystem
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
        private const string c_sDefaultWritingSystem = "Latin";
        #endregion
        #region Attr{g/s}: string FontStyleAsString
        protected string FontStyleAsString
        {
            get
            {
                var s = "";

                if ((FontStyle & FontStyle.Bold) == FontStyle.Bold)
                    s += c_sBold;
                if ((FontStyle & FontStyle.Italic) == FontStyle.Italic)
                    s += c_sItalic;
                if ((FontStyle & FontStyle.Underline) == FontStyle.Underline)
                    s += c_sUnderline;
                if ((FontStyle & FontStyle.Strikeout) == FontStyle.Strikeout)
                    s += c_sStrikeout;

                return s;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                var fs = new FontStyle();

                if (value.Contains(c_sBold))
                    fs |= FontStyle.Bold;
                if (value.Contains(c_sItalic))
                    fs |= FontStyle.Italic;
                if (value.Contains(c_sUnderline))
                    fs |= FontStyle.Underline;
                if (value.Contains(c_sStrikeout))
                    fs |= FontStyle.Strikeout;

                FontStyle = fs;
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
        #region SMethod: FontForWritingSystem Create(nodeFont)
        static public FontForWritingSystem Create(XmlNode nodeFont)
        {
            if (nodeFont.Name != c_sTag)
                return null;

            var obj = new FontForWritingSystem
            {
                WritingSystemName = XmlDoc.GetAttrValue(nodeFont, c_sAttrWritingSystem, c_sDefaultWritingSystem),
                FontName = XmlDoc.GetAttrValue(nodeFont, c_sAttrFontName, "Arial"),
                FontSize = XmlDoc.GetAttrValue(nodeFont, c_sAttrSize, 10.0F),
                FontStyleAsString = XmlDoc.GetAttrValue(nodeFont, c_sAttrStyle, "")
            };

            return obj;
        }
        #endregion
        #region Method: void Merge(parent, theirs)
        public void Merge(FontForWritingSystem parent, FontForWritingSystem theirs)
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
