#region ***** Styles.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Styles.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Top-level class for the Stylesheet. Meant to be accessed as a Global (thus
 *          everything is implemented as a static
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class StyleSheet
    {
        #region SMethod: void DeclareDirty()
        static public void DeclareDirty()
        {
            s_bIsDirty = true;
        }
        #endregion
        static private bool s_bIsDirty;

        // Writing Systems
        static public WritingSystem DefaultWritingSystem;

        // Character Styles
        static public CharacterStyle VerseNumber;
        static public CharacterStyle ChapterNumber;
        static public CharacterStyle FootnoteLetter;

        // Paragraph Styles
        static public ParagraphStyle Normal;
        static public ParagraphStyle Line1;
        static public ParagraphStyle Line2;

        // List of Styles
        #region SAttr{g}: List<CharacterStyle> StyleList
        static List<CharacterStyle> StyleList
        {
            get
            {
                if (null == s_StyleList)
                    s_StyleList = new List<CharacterStyle>();
                Debug.Assert(null != s_StyleList);
                return s_StyleList;
            }
        }
        private static List<CharacterStyle> s_StyleList;
        #endregion
        #region Method: CharacterStyle Find(sStyleName)
        static CharacterStyle Find(string sStyleName)
        {
            foreach (var style in StyleList)
            {
                if (style.StyleName == sStyleName)
                    return style;
            }
            return null;
        }
        #endregion
        #region Method: CharacterStyle InitializeStyle(CharacterStyle)
        static CharacterStyle InitializeStyle(CharacterStyle style)
        {
            var existing = Find(style.StyleName);

            if (null == existing)
            {
                StyleList.Add(style);
                return style;
            }

            existing.SetDefaults(style);
            return existing;
        }
        #endregion

        // Initialize 
        #region SMethod: void Clear()
        static void Clear()
        {
            s_StyleList = null;
            s_WritingSystems = null;
        }
        #endregion
        #region SMethod: void EnsureFactoryInitialized()
        private static void EnsureFactoryInitialized()
            // Defaults in the style system; anything different must be explicitly 
            // declared here
            //
            // CharacterStyle
            //   FontSize = 10.0
            //   FontName = Arial
            //   FontStyle = Regular
            //   ForeColor = Black
            //
            // ParagraphStyle
            //   Alignment = Left
            //   Left & Right Margins = 0
        {
            EnsureFactoryWritingSystemsInitialized();
            EnsureFactoryCharacterStylesInitialized();
            EnsureFactoryParagraphStylesInitialized();

            // The initialize process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }
        #endregion
        #region SMethod: void EnsureFactoryCharacterStylesInitialized()
        static void EnsureFactoryCharacterStylesInitialized()
        {
            VerseNumber = InitializeStyle(new CharacterStyle("Verse Number") 
            {
                FontColor = Color.Red
            });

            ChapterNumber = InitializeStyle(new CharacterStyle("Chapter Number") 
            { 
                DefaultFontSize = 20,
                DefaultFontStyle = FontStyle.Bold
            });

            FootnoteLetter = InitializeStyle(new CharacterStyle("Footnote Letter")
            {
                FontColor = Color.Navy
            });
        }
        #endregion
        #region SMethod: void EnsureFactoryParagraphStylesInitialized()
        static void EnsureFactoryParagraphStylesInitialized()
        {
            Normal = InitializeStyle(new ParagraphStyle("Normal")
            {
                Alignment = ParagraphStyle.Align.Justified
            }) as ParagraphStyle;

            Line1 = InitializeStyle(new ParagraphStyle("Line 1")
            {
                Alignment = ParagraphStyle.Align.Justified,
                LeftMarginInches = 0.2
            }) as ParagraphStyle;

            Line2 = InitializeStyle(new ParagraphStyle("Line 2")
            {
                Alignment = ParagraphStyle.Align.Justified,
                LeftMarginInches = 0.4
            }) as ParagraphStyle;
        }
        #endregion
        #region SMethod: void EnsureFactoryWritingSystemsInitialized()
        static void EnsureFactoryWritingSystemsInitialized()
        {
            DefaultWritingSystem = FindOrCreate(WritingSystem.DefaultWritingSystemName);
        }
        #endregion

        // WritingSystems --------------------------------------------------------------------
        #region SAttr{g}: List<WritingSystem> WritingSystems
        static List<WritingSystem> WritingSystems
        {
            get
            {
                if (null == s_WritingSystems)
                    s_WritingSystems = new List<WritingSystem>();
                Debug.Assert(null != s_WritingSystems);
                return s_WritingSystems;
            }
        }
        static private List<WritingSystem> s_WritingSystems;
        #endregion
        #region SMethod: WritingSystem FindWritingSystem(sWritingSystemName)
        static WritingSystem FindWritingSystem(string sWritingSystemName)
        {
            foreach (var ws in WritingSystems)
            {
                if (ws.Name == sWritingSystemName)
                    return ws;
            }
            return null;
        }
        #endregion
        #region SMethod: WritingSystem FindOrCreate(string sWritingSystemName)
        static WritingSystem FindOrCreate(string sWritingSystemName)
        {
            var ws = FindWritingSystem(sWritingSystemName);

            if (null == ws)
            {
                ws = new WritingSystem {Name = sWritingSystemName};
                WritingSystems.Add(ws);
            }

            return ws;
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        #region IO Constants
        private const string c_sTag = "StyleSheet";
        private const string c_sTagStyles = "Styles";
        private const string c_sTagWritingSystems = "WritingSystems";

        private const string c_sAttrVersion = "Version";
        private const int c_nStyleSheetVersionNo = 1;
        #endregion
        #region SMethod: void Save(sPath)
        static public void Save(string sPath)
        {
            // Force a save, even if not dirty, if the file does noto already exist
            if (!string.IsNullOrEmpty(sPath) && !File.Exists(sPath))
                DeclareDirty();

            // Don't write unless changes have been made
            if (!s_bIsDirty)
                return;

            var doc = new XmlDoc();
            doc.AddXmlDeclaration();
            var nodeStyleSheet = doc.AddNode(null, c_sTag);
            doc.AddAttr(nodeStyleSheet, c_sAttrVersion, c_nStyleSheetVersionNo);

            // Styles
            var nodeStyles = doc.AddNode(nodeStyleSheet, c_sTagStyles);
            foreach (var style in StyleList)
                style.Save(doc, nodeStyles);

            // Writing Systems
            var nodeWritingSystems = doc.AddNode(nodeStyleSheet, c_sTagWritingSystems);
            foreach (var ws in WritingSystems)
                ws.Save(doc, nodeWritingSystems);

            doc.Write(sPath);

            s_bIsDirty = false;
        }
        #endregion
        #region SMethod: void Initialize(sPath)
        #region SMethod: void ReadStyles(XmlNode nodeStyleSheet)
        static void ReadStyles(XmlNode nodeStyleSheet)
        {
            var nodeStyles = XmlDoc.FindNode(nodeStyleSheet, c_sTagStyles);
            if (null == nodeStyles) 
                return;

            foreach (XmlNode node in nodeStyles.ChildNodes)
            {
                var style = CharacterStyle.Create(node) ??
                            ParagraphStyle.Create(node);

                if (null != style)
                    StyleList.Add(style);
            }
        }
        #endregion
        #region SMethod: void ReadWritingSystems(XmlNode nodeStyleSheet)
        static void ReadWritingSystems(XmlNode nodeStyleSheet)
        {
            var nodeWritingSystems = XmlDoc.FindNode(nodeStyleSheet, c_sTagWritingSystems);
            if (null == nodeWritingSystems) 
                return;

            foreach (XmlNode node in nodeWritingSystems.ChildNodes)
            {
                var ws = WritingSystem.Create(node);
                if (null != ws)
                    WritingSystems.Add(ws);
            }
        }
        #endregion
        #region SMethod: void ReadStyleSheet(string sPath)
        static void ReadStyleSheet(string sPath)
        {
            if (string.IsNullOrEmpty(sPath))
                return;

            if (!File.Exists(sPath))
                return;

            try
            {
                var doc = new XmlDoc();
                doc.Load(sPath);

                var nodeStyleSheet = XmlDoc.FindNode(doc, c_sTag);
                var nVersion = XmlDoc.GetAttrValue(nodeStyleSheet, c_sAttrVersion, 0);
                if (nVersion > c_nStyleSheetVersionNo)
                {
                    throw new Exception("StyleSheet is a later version, requires an OurWord upgrade.");
                }

                if (null != nodeStyleSheet)
                {
                    ReadStyles(nodeStyleSheet);
                    ReadWritingSystems(nodeStyleSheet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on reading stylesheet: " + e.Message);
            }
        }
        #endregion
        static public void Initialize(string sPath)
        {
            // Start with an empty stylesheet
            Clear();

            // Attenpt to load and read the Xml stylesheet (an empty path is ignored)
            ReadStyleSheet(sPath);

            // Make sure the styles we expect are indeed in this stylesheet
            EnsureFactoryInitialized();

            // The reading process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }
        #endregion
    }
}
