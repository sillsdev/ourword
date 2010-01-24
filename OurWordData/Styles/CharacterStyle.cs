#region ***** CharacterStyle.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    CharacterStyle.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Character styles are used for special in-paragraph runs such as chapter numbers
 *          or footnote letters.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class CharacterStyle
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: string StyleName - serves as the style's unique ID
        public string StyleName
        {
            get
            {
                return m_sStyleName;
            }
        }
        private readonly string m_sStyleName;
        #endregion
        #region Attr{g/s}: Color FontColor - color for text; default is black
        public Color FontColor
        {
            get
            {
                return m_Color;
            }
            set
            {
                if (m_Color == value)
                    return;
                m_Color = value;
                StyleSheet.DeclareDirty();
            }
        }
        private Color m_Color = Color.Black;
        #endregion

        // FontFactories ---------------------------------------------------------------------
        #region Attr{g}: List<FontFactory> FontFactories
        public List<FontFactory> FontFactories
        {
            get
            {
                return m_FontFactories;
            }
        }
        private readonly List<FontFactory> m_FontFactories;
        #endregion
        #region Method: void ResetFonts()
        public void ResetFonts()
        {
            foreach (var factory in FontFactories)
                factory.ResetFonts();
        }
        #endregion
        #region Method: FontFactory FindFontFactory(sWritingSystemName)
        public FontFactory FindFontFactory(string sWritingSystemName)
        {
            foreach (var factory in FontFactories)
            {
                if (factory.WritingSystemName == sWritingSystemName)
                    return factory;
            }
            return null;
        }
        #endregion
        #region Method: FontFactory FindOrAddFontFactory(sWritingSystemName)
        public FontFactory FindOrAddFontFactory(string sWritingSystemName)
        {
            var factory = FindFontFactory(sWritingSystemName);

            if (null == factory)
            {
                factory = new FontFactory 
                {
                    WritingSystemName = sWritingSystemName,
                    FontName = m_DefaultFont.FontName,
                    FontSize = m_DefaultFont.FontSize,
                    FontStyle = m_DefaultFont.FontStyle
                };
                FontFactories.Add(factory);
            }

            Debug.Assert(null != factory);
            return factory;
        }
        #endregion
        #region Method: Font GetFont(sWritingSystemName, fZoomPercent)
        public Font GetFont(string sWritingSystemName, float fZoomPercent)
        {
            var factory = FindOrAddFontFactory(sWritingSystemName);
            return factory.GetFont(fZoomPercent);
        }
        #endregion

        // Default (factory) font settings ---------------------------------------------------
        #region VAttr{s}: string DefaultFontName
        public string DefaultFontName
        {
            set
            {
                m_DefaultFont.FontName = value;
            }
        }
        #endregion
        #region VAttr{s}: float DefaultFontSize
        public float DefaultFontSize
        {
            set
            {
                m_DefaultFont.FontSize = value;
            }
        }
        #endregion
        #region VAttr{s}: FontStyle DefaultFontStyle
        public FontStyle DefaultFontStyle
        {
            set
            {
                m_DefaultFont.FontStyle = value;
            }
        }
        #endregion
        protected readonly FontFactory m_DefaultFont;

        // Scaffolding -----------------------------------------------------------------------
        private const string c_sDefaultFontName = "Arial";
        private const float c_fDefaultFontSize = 10.0F;
        #region Constructor(sStyleName)
        public CharacterStyle(string sStyleName)
        {
            m_sStyleName = sStyleName;

            m_FontFactories = new List<FontFactory>();

            m_DefaultFont = new FontFactory 
            {
                WritingSystemName = "Default",
                FontName = c_sDefaultFontName,
                FontSize = c_fDefaultFontSize,
                FontStyle = FontStyle.Regular
            };
        }
        #endregion
        #region SMethod: int SortCompare(CharacterStyle x, CharacterStyle y)
        public static int SortCompare(CharacterStyle x, CharacterStyle y)
        {
            // This will sort first by char vs para style, then by the style name
            var xName = ((null != x as ParagraphStyle) ? "P-" : "C-") + x.StyleName;
            var yName = ((null != y as ParagraphStyle) ? "P-" : "C-") + y.StyleName;

            return string.Compare(xName, yName);
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        #region VirtAttr{g}: Tag
        private const string c_sTag = "CharacterStyle";
        protected virtual string Tag
        {
            get
            {
                return c_sTag;
            }
        }
        #endregion
        #region I/O Constants
        private const string c_sAttrStyleName = "Name";
        private const string c_sAttrColor = "Color";
        private const string c_sAttrDefaultFontName = "FontName";
        private const string c_sAttrDefaultFontSize = "FontSize";
        private const string c_sAttrDefaultFontStyle = "FontStyle";
        #endregion
        #region SMethod: string GetStyleNameFromXml(XmlNode node)
        static protected string GetStyleNameFromXml(XmlNode node)
        {
            return XmlDoc.GetAttrValue(node, c_sAttrStyleName, "");
        }
        #endregion
        #region Method: XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        public XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        {
            var node = doc.AddNode(nodeParent, Tag);
            SaveContent(doc, node);
            return node;
        }
        #endregion
        #region VirtMethod: void SaveContent(XmlDoc, nodeStyle)
        protected virtual void SaveContent(XmlDoc doc, XmlNode node)
        {
            doc.AddAttr(node, c_sAttrStyleName, StyleName);
            doc.AddAttr(node, c_sAttrColor, FontColor.Name);
            doc.AddAttr(node, c_sAttrDefaultFontName, m_DefaultFont.FontName);
            doc.AddAttr(node, c_sAttrDefaultFontSize, m_DefaultFont.FontSize);
            doc.AddAttr(node, c_sAttrDefaultFontStyle, 
                FontFactory.GetFontStyleAsString(m_DefaultFont.FontStyle));

            foreach (var factory in FontFactories)
                factory.Save(doc, node);
        }
        #endregion
        #region VirtMethod: void ReadContent(XmlNode node)
        virtual protected void ReadContent(XmlNode node)
        {
            // Content Attributes
            m_Color = Color.FromName(XmlDoc.GetAttrValue(node, c_sAttrColor,
                Color.Black.ToString()));
            DefaultFontName = XmlDoc.GetAttrValue(node, c_sAttrDefaultFontName, c_sDefaultFontName);
            DefaultFontSize = XmlDoc.GetAttrValue(node, c_sAttrDefaultFontSize, c_fDefaultFontSize);
            DefaultFontStyle = FontFactory.GetFontStyleFromString(
                XmlDoc.GetAttrValue(node, c_sAttrDefaultFontStyle, ""));

            // FontsForWritingSystem
            foreach (XmlNode child in node.ChildNodes)
            {
                var factory = FontFactory.Create(child);
                if (null == factory)
                    continue;

                FontFactories.Add(factory);
            }
        }
        #endregion
        #region SMethod: CharacterStyle Create(node)
        static public CharacterStyle Create(XmlNode node)
        {
            if (node.Name != c_sTag)
                return null;

            // Abort if we have an invalid style name
            var sStyleName = GetStyleNameFromXml(node);
            if (string.IsNullOrEmpty(sStyleName))
                return null;

            // Create the style
            var style = new CharacterStyle(sStyleName);
            style.ReadContent(node);

            return style;
        }
        #endregion
        #region Method: void Merge(parent, theirs)
        public void Merge(CharacterStyle parent, CharacterStyle theirs)
        {
            Debug.Assert(parent != null);
            Debug.Assert(theirs != null);

            // We require them to all be the same StyleName (that's their Unique ID)
            Debug.Assert(StyleName == parent.StyleName);
            Debug.Assert(StyleName == theirs.StyleName);

            // Attributes. Algorithm: We keep theirs iff they differ from ours and ours is
            // unchanged from the parent. Otherwise we always keep ours.
            if (FontColor != theirs.FontColor && FontColor == parent.FontColor)
                FontColor = theirs.FontColor;

            // Merge those fonts which exist in all three
            foreach (var ourFont in FontFactories)
            {
                var parentFont = parent.FindFontFactory(ourFont.WritingSystemName);
                var theirFont = theirs.FindFontFactory(ourFont.WritingSystemName);
                if (null != parentFont && null != theirFont)
                    ourFont.Merge(parentFont, theirFont);
            }

            // Add any new fonts that only exist in theirs
            foreach (var theirFactory in theirs.FontFactories)
            {
                var ourFactory = FindFontFactory(theirFactory.WritingSystemName);
                var parentFactory = parent.FindFontFactory(theirFactory.WritingSystemName);
                if (null == parentFactory && null == ourFactory)
                    FontFactories.Add(theirFactory.Clone());
            }
        }
        #endregion


    }
}
