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
    public class CharacterStyle : Style
    {
        // Content Attrs ---------------------------------------------------------------------
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
        private readonly FontForWritingSystem m_DefaultFont;
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
            : base(sStyleName)
        {
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
        private const string c_sTag = "CharacterStyle";
        private const string c_sAttrColor = "Color";
        #region OMethod: XmlNode Save(XmlDoc, nodeParent)
        public override XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        {
            var node = doc.AddNode(nodeParent, c_sTag);

            // Superclass attributes
            SaveStyleAttrs(doc, node);

            // This class's data
            doc.AddAttr(node, c_sAttrColor, FontColor.Name );
            foreach (var fws in FontsForWritingSystem)
                fws.Save(doc, node);

            return node;
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

            // Create the style with its attributes
            var style = new CharacterStyle(sStyleName)
            {
                m_Color = Color.FromName(XmlDoc.GetAttrValue(node, c_sAttrColor, 
                    Color.Black.ToString()))
            };

            // Read in the FontsForWritingSystem
            foreach(XmlNode child in node.ChildNodes)
            {
                var fws = FontForWritingSystem.Create(child);
                if (null == fws) 
                    continue;

                style.FontsForWritingSystem.Add(fws);
            }

            return style;
        }
        #endregion
        #region SMethod: void Merge(nodeOurs, nodeTheirs, nodeParent)
        #region Method: XmlNode GetCorrespondingFontNode(targetFontNode, parent)
        static XmlNode GetCorrespondingFontNode(XmlNode targetFontNode, XmlNode parent)
        {
            var sWritingSystemName = FontForWritingSystem.GetWritingSystemNameFromXml(targetFontNode);

             foreach(XmlNode child in parent.ChildNodes)
             {
                 if (FontForWritingSystem.GetWritingSystemNameFromXml(child) == sWritingSystemName)
                     return child;
             }
             return null;
        }
        #endregion
        static public void Merge(XmlNode ours, XmlNode theirs, XmlNode parent)
        {
            if (!CanMerge(ours, theirs, parent))
                return;

            Debug.Assert(ours.Name == c_sTag);
            Debug.Assert(theirs.Name == c_sTag);
            Debug.Assert(parent.Name == c_sTag);

            // Merge the Attributes
            XmlDoc.MergeAttr(ours, theirs, parent, c_sAttrColor);

            // Merge the fonts which exist in all three
            foreach(XmlNode ourFontNode in ours.ChildNodes)
            {
                var theirFontNode = GetCorrespondingFontNode(ourFontNode, theirs);
                var parentFontNode = GetCorrespondingFontNode(ourFontNode, parent);

                if (null != theirFontNode && null != parentFontNode)
                    FontForWritingSystem.Merge(ourFontNode, theirFontNode, parentFontNode);
            }

            // Add any new fonts that only exist in theirs
            foreach (XmlNode theirFontNode in theirs.ChildNodes)
            {
                var ourFontNode = GetCorrespondingFontNode(theirFontNode, ours);
                var parentFontNode = GetCorrespondingFontNode(theirFontNode, parent);

                if (null == parentFontNode && null == ourFontNode)
                    XmlDoc.CopyNode(ours, theirFontNode);
            }
        }
        #endregion
    }
}
