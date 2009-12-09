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
                Styles.DeclareDirty();
            }
        }
        private Color m_Color = Color.Black;
        #endregion

        // Fonts for Writing System ----------------------------------------------------------
        #region Attr{g}: List<FontForWritingSystem> FontsForWritingSystem
        public List<FontForWritingSystem> FontsForWritingSystem
        {
            get
            {
                return m_FontsForWritingSystem;
            }
        }
        private readonly List<FontForWritingSystem> m_FontsForWritingSystem;
        #endregion
        #region Method: void ResetFonts()
        public void ResetFonts()
        {
            foreach(var fws in FontsForWritingSystem)
                fws.ResetFonts();
        }
        #endregion
        #region Method: FontForWritingSystem FindFont(sWritingSystemName)
        public FontForWritingSystem FindFont(string sWritingSystemName)
        {
            foreach(var fws in FontsForWritingSystem)
            {
                if (fws.WritingSystemName == sWritingSystemName)
                    return fws;
            }
            return null;
        }
        #endregion
        #region Method: FontForWritingSystem FindOrAddFont(sWritingSystemName)
        public FontForWritingSystem FindOrAddFont(string sWritingSystemName)
        {
            var fws = FindFont(sWritingSystemName);

            if (null == fws)
            {
                fws = new FontForWritingSystem 
                {
                    WritingSystemName = sWritingSystemName,
                    FontName = m_DefaultFont.FontName,
                    FontSize = m_DefaultFont.FontSize,
                    FontStyle = m_DefaultFont.FontStyle
                };
                FontsForWritingSystem.Add(fws);
            }

            Debug.Assert(null != fws);
            return fws;
        }
        #endregion

        // Default (factory) font settings --------------------------------------------------
        #region Attr{s}: string DefaultFontName
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
        protected readonly FontForWritingSystem m_DefaultFont;
        #region Method: void SetDefaults(pattern)
        public void SetDefaults(CharacterStyle pattern)
        {
            DefaultFontName = pattern.m_DefaultFont.FontName;
            DefaultFontSize = pattern.m_DefaultFont.FontSize;
            DefaultFontStyle = pattern.m_DefaultFont.FontStyle;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sStyleName)
        public CharacterStyle(string sStyleName)
        {
            m_sStyleName = sStyleName;

            m_FontsForWritingSystem = new List<FontForWritingSystem>();

            m_DefaultFont = new FontForWritingSystem 
            {
                WritingSystemName = "Factory",
                FontName = "Arial",
                FontSize = 10.0F,
                FontStyle = FontStyle.Regular
            };
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

            foreach (var fws in FontsForWritingSystem)
                fws.Save(doc, node);
        }
        #endregion
        #region VirtMethod: void ReadContent(XmlNode node)
        virtual protected void ReadContent(XmlNode node)
        {
            // Content Attributes
            m_Color = Color.FromName(XmlDoc.GetAttrValue(node, c_sAttrColor,
                Color.Black.ToString()));

            // FontsForWritingSystem
            foreach (XmlNode child in node.ChildNodes)
            {
                var fws = FontForWritingSystem.Create(child);
                if (null == fws)
                    continue;

                FontsForWritingSystem.Add(fws);
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
            foreach(var ourFont in FontsForWritingSystem)
            {
                var parentFont = parent.FindFont(ourFont.WritingSystemName);
                var theirFont = theirs.FindFont(ourFont.WritingSystemName);
                if (null != parentFont && null != theirFont)
                    ourFont.Merge(parentFont, theirFont);
            }

            // Add any new fonts that only exist in theirs
            foreach(var theirFont in theirs.FontsForWritingSystem)
            {
                var ourFont = FindFont(theirFont.WritingSystemName);
                var parentFont = parent.FindFont(theirFont.WritingSystemName);
                if (null == parentFont && null == ourFont)
                    FontsForWritingSystem.Add(theirFont.Clone());
            }
        }
        #endregion

    }
}
