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
        static public CharacterStyle BigHeader;
        static public CharacterStyle Label;

        // Paragraph Styles: Book Organization
        static public ParagraphStyle RunningHeader;
        static public ParagraphStyle BookTitle;
        static public ParagraphStyle BookSubTitle;

        static public ParagraphStyle MajorSection;
        static public ParagraphStyle MajorSectionCrossReference;
        static public ParagraphStyle Section;
        static public ParagraphStyle SectionCrossReference;
        static public ParagraphStyle MinorSection;

        // Paragraph Styles: Scripture Content
        static public ParagraphStyle Paragraph;
        static public ParagraphStyle ParagraphContinuation;
        static public ParagraphStyle Line1;
        static public ParagraphStyle Line2;
        static public ParagraphStyle Line3;
        static public ParagraphStyle LineCentered;
        static public ParagraphStyle PictureCaption;
        static public ParagraphStyle Footnote;

        // Paragraph Styles: Literate Settings
        static public ParagraphStyle LiterateParagraph;
        static public ParagraphStyle LiterateHeading;
        static public ParagraphStyle LiterateAttention;
        static public ParagraphStyle LiterateList;

        // Paragraph Styles: User Interface
        static public ParagraphStyle ReferenceTranslation;
        public static ParagraphStyle TipHeader;
        public static ParagraphStyle TipContent;
        public static ParagraphStyle TipText;
        public static ParagraphStyle TipMessage;

        // List of Styles --------------------------------------------------------------------
        #region SAttr{g}: List<CharacterStyle> StyleList
        static List<CharacterStyle> StyleList
        {
            get
            {
                if (null == s_vStyleList)
                    s_vStyleList = new List<CharacterStyle>();
                Debug.Assert(null != s_vStyleList);
                return s_vStyleList;
            }
        }
        private static List<CharacterStyle> s_vStyleList;
        #endregion
        #region SMethod: CharacterStyle Find(sStyleName)
        static public CharacterStyle Find(string sStyleName)
        {
            foreach (var style in StyleList)
            {
                if (style.StyleName == sStyleName)
                    return style;
            }
            return null;
        }
        #endregion
        #region SMethod: CharacterStyle FindOrAdd(CharacterStyle)
        static CharacterStyle FindOrAdd(CharacterStyle styleToAddIfNotAlreadyPresent)
        {
            var style = Find(styleToAddIfNotAlreadyPresent.StyleName);

            if (null == style)
            {
                StyleList.Add(styleToAddIfNotAlreadyPresent);
                StyleList.Sort(CharacterStyle.SortCompare);
                style = styleToAddIfNotAlreadyPresent;
                DeclareDirty();
            }

            Debug.Assert(null != style);
            return style;
        }
        #endregion
        static public ParagraphStyle FindFromToolboxMarker(string sMarker)
        {
            foreach(var style in StyleList)
            {
                var paragraphStyle = style as ParagraphStyle;
                if (null == paragraphStyle)
                    continue;
                if (paragraphStyle.Map.ToolboxMarker == sMarker)
                    return paragraphStyle;
            }
            return null;
        }

        // Initialization -------------------------------------------------------------------- 
        #region SMethod: void Clear()
        static void Clear()
        {
            s_vStyleList = null;
            s_WritingSystems = null;
        }
        #endregion
        #region SMethod: void EnsureFactoryInitialized()
        public static void EnsureFactoryInitialized()
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
            EnsureLiterateSettingsStyleInitialized();
            EnsureUserInterfaceParagraphsInitialized();

            // The initialize process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }
        #endregion
        #region SMethod: void EnsureFactoryCharacterStylesInitialized()
        static void EnsureFactoryCharacterStylesInitialized()
        {
            VerseNumber = FindOrAdd(new CharacterStyle("Verse Number") 
                { DefaultFontSize = 8, FontColor = Color.Red });

            ChapterNumber = FindOrAdd(new CharacterStyle("Chapter Number")
                { DefaultFontSize = 20, DefaultFontStyle = FontStyle.Bold });

            FootnoteLetter = FindOrAdd(new CharacterStyle("Footnote Letter") 
                { DefaultFontSize = 8, FontColor = Color.Navy });

            BigHeader = FindOrAdd(new CharacterStyle("Header in Window Panes")
                { DefaultFontSize = 12, DefaultFontStyle = FontStyle.Bold });

            Label = FindOrAdd(new CharacterStyle("Label")
                { FontColor = Color.Crimson });
        }
        #endregion
        #region SMethod: void EnsureFactoryParagraphStylesInitialized()
        static void EnsureFactoryParagraphStylesInitialized()
        {
            // Book Organization Paragraphs
            RunningHeader = EnsureInitialized(
                new ParagraphStyle("Header") { 
                        DefaultFontStyle = FontStyle.Bold, 
                        Alignment = ParagraphStyle.Align.Justified },
                new ParagraphStyle.Mapping("h"));

            BookTitle = EnsureInitialized(
                new ParagraphStyle("Title Main") {
                        DefaultFontStyle = FontStyle.Bold,
                        DefaultFontSize = 16,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsAfter = 9,
                        KeepWithNextParagraph = true },
                new ParagraphStyle.Mapping("mt"));

            BookSubTitle = EnsureInitialized(
                new ParagraphStyle("Title Secondary") {
                        DefaultFontStyle = FontStyle.Bold,
                        DefaultFontSize = 14,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsAfter = 9,
                        KeepWithNextParagraph = true },
                new ParagraphStyle.Mapping("mt2") {ToolboxMarker = "st"});

            MajorSection = EnsureInitialized(
                new ParagraphStyle("Major Section Head") {
                        DefaultFontStyle = FontStyle.Bold,
                        DefaultFontSize = 14,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsBefore = 12,
                        PointsAfter = 3,
                        KeepWithNextParagraph = true },
                new ParagraphStyle.Mapping("ms"));

            MajorSectionCrossReference = EnsureInitialized(
                new ParagraphStyle("Major Section Range") {
                        DefaultFontStyle = FontStyle.Italic,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsAfter = 3,
                        KeepWithNextParagraph = true}, 
                new ParagraphStyle.Mapping("mr"));

            Section = EnsureInitialized(
                new ParagraphStyle("Section Head") {
                        DefaultFontStyle = FontStyle.Bold,
                        DefaultFontSize = 12,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsBefore = 12,
                        PointsAfter = 3,
                        KeepWithNextParagraph = true }, 
                new ParagraphStyle.Mapping("s1") {ToolboxMarker = "s"});

            SectionCrossReference = EnsureInitialized(
                new ParagraphStyle("Parallel Passage Reference") {
                        DefaultFontStyle = FontStyle.Italic,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsAfter = 3,
                        KeepWithNextParagraph = true },
                new ParagraphStyle.Mapping("r"));

            MinorSection = EnsureInitialized(
                new ParagraphStyle("Section Head 2") {
                        DefaultFontStyle = FontStyle.Bold,
                        DefaultFontSize = 12,
                        Alignment = ParagraphStyle.Align.Centered,
                        PointsBefore = 9,
                        PointsAfter = 3,
                        KeepWithNextParagraph = true}, 
                new ParagraphStyle.Mapping("s2"));

            // Scripture Content Paragraphs
            Paragraph = EnsureInitialized(
                new ParagraphStyle("Paragraph") {
                        Alignment = ParagraphStyle.Align.Justified }, 
                new ParagraphStyle.Mapping("p") {
                        IsScripture = true });

            ParagraphContinuation = EnsureInitialized(
                new ParagraphStyle("Paragraph Continuation") {
                        Alignment = ParagraphStyle.Align.Justified },
                new ParagraphStyle.Mapping("m") {
                    IsScripture = true });

            Line1 = EnsureInitialized(
                new ParagraphStyle("Line 1") {
                        Alignment = ParagraphStyle.Align.Justified,
                        LeftMarginInches = 0.2 },
                new ParagraphStyle.Mapping("q1") {
                    ToolboxMarker = "q", IsScripture = true, IsPoetry = true});

            Line2 = EnsureInitialized(
                new ParagraphStyle("Line 2") {
                        Alignment = ParagraphStyle.Align.Justified,
                        LeftMarginInches = 0.4 },
                new ParagraphStyle.Mapping("q2") {
                    IsScripture = true, IsPoetry = true});

            Line3 = EnsureInitialized(
                new ParagraphStyle("Line 3") {
                        Alignment = ParagraphStyle.Align.Justified,
                        LeftMarginInches = 0.6 },
                new ParagraphStyle.Mapping("q3") {
                    IsScripture = true, IsPoetry = true});

            LineCentered = EnsureInitialized(
                new ParagraphStyle("Line Centered") {
                        Alignment = ParagraphStyle.Align.Justified,
                        LeftMarginInches = 0.2,
                        RightMarginInches = 0.2 },
                new ParagraphStyle.Mapping("qc") {
                    IsScripture = true, IsPoetry = true});

            PictureCaption = EnsureInitialized(
                new ParagraphStyle("Caption") {
                        DefaultFontStyle = FontStyle.Italic,
                        Alignment = ParagraphStyle.Align.Centered },
                new ParagraphStyle.Mapping("fig") {ToolboxMarker = "cap"});

            Footnote = EnsureInitialized(
                new ParagraphStyle("Footnote") {
                        Alignment = ParagraphStyle.Align.Justified,
                        PointsAfter = 3 },
                new ParagraphStyle.Mapping("f") { ToolboxMarker = "fn" });

        }
        #endregion
        #region SMethod: void EnsureLiterateSettingsStyleInitialized()
        static void EnsureLiterateSettingsStyleInitialized()
        {
            LiterateParagraph = EnsureInitialized(
                new ParagraphStyle("Literate Paragraph") {
                        DefaultFontSize = 8,
                        Alignment = ParagraphStyle.Align.Justified,
                        PointsBefore = 3,
                        PointsAfter = 3 },
                new ParagraphStyle.Mapping("LitParagraph") {
                        IsUserInterface = true });

            LiterateHeading = EnsureInitialized(
                new ParagraphStyle("Literate Heading") {
                    DefaultFontSize = 9,
                    DefaultFontStyle = FontStyle.Bold,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 6,
                    PointsAfter = 3 },
                new ParagraphStyle.Mapping("LitHeading") {
                    IsUserInterface = true });

            LiterateAttention = EnsureInitialized(
                new ParagraphStyle("Literate Attention") {
                    DefaultFontSize = 8,
                    DefaultFontStyle = FontStyle.Bold,
                    FontColor = Color.DarkRed,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 3,
                    PointsAfter = 3 },
                new ParagraphStyle.Mapping("LitAttention") {
                    IsUserInterface = true });

            LiterateList = EnsureInitialized(
                new ParagraphStyle("Literate List") {
                    DefaultFontSize = 8,
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.2,
                    PointsBefore = 0,
                    PointsAfter = 0,
                    Bulleted = true },
                new ParagraphStyle.Mapping("LitList") {
                    IsUserInterface = true });
        }
        #endregion
        #region SMethod: void EnsureUserInterfaceParagraphsInitialized()
        static void EnsureUserInterfaceParagraphsInitialized()
        {
            ReferenceTranslation = EnsureInitialized(
               new ParagraphStyle("Reference Translation") {
                   DefaultFontSize = 10,
                   Alignment = ParagraphStyle.Align.Justified,
                   FirstLineIndentInches = -0.20,
                   LeftMarginInches = 0.20 },
               new ParagraphStyle.Mapping("ReferenceTranslation") {
                   IsUserInterface = true });

            TipHeader = EnsureInitialized(
               new ParagraphStyle("Tip Header") {
                   Alignment = ParagraphStyle.Align.Left,
                   PointsBefore = 0,
                   PointsAfter = 0 },
               new ParagraphStyle.Mapping("TipHeader") {
                   IsUserInterface = true });

            TipContent = EnsureInitialized(
               new ParagraphStyle("Tip Content") {
                   DefaultFontSize = 9,
                   DefaultFontStyle = FontStyle.Bold,
                   Alignment = ParagraphStyle.Align.Justified,
                   PointsBefore = 0,
                   PointsAfter = 0,
                   LeftMarginInches = 0.1 },
               new ParagraphStyle.Mapping("TipContent") {
                   IsUserInterface = true });

            TipText = EnsureInitialized(
               new ParagraphStyle("Tip Text") {
                   DefaultFontSize = 9,
                   DefaultFontStyle = FontStyle.Bold,
                   Alignment = ParagraphStyle.Align.Justified,
                   PointsBefore = 5,
                   PointsAfter = 0 },
               new ParagraphStyle.Mapping("TipText") {
                   IsUserInterface = true });

            TipMessage = EnsureInitialized(
               new ParagraphStyle("Tip Note Discussion")
               {
                   Alignment = ParagraphStyle.Align.Justified,
                   PointsBefore = 3,
                   PointsAfter = 3
               },
               new ParagraphStyle.Mapping("TipText")
               {
                   IsUserInterface = true
               });
        }
        #endregion

        #region SMethod: ParagraphStyle EnsureInitialized(defaultStyle, map)
        static ParagraphStyle EnsureInitialized(CharacterStyle defaultStyle, ParagraphStyle.Mapping map)
        {
            var style = FindOrAdd(defaultStyle) as ParagraphStyle;
            Debug.Assert(null != style);
            style.Map = map;
            return style;
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

            // The reading process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;

            // Make sure the styles we expect are indeed in this stylesheet; anything added
            // here will DeclareDirty and result in a save.
            EnsureFactoryInitialized();
        }
        #endregion
    }
}
