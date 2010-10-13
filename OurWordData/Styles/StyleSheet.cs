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
using System.Linq;
using System.Text;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class StyleSheet
    {
        // Style Declarations ----------------------------------------------------------------
        #region DOC - Default settings
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
        #endregion

        // Character Styles
        #region CharacterStyle VerseNumber
        public static readonly CharacterStyle VerseNumber =
            new CharacterStyle("Verse Number")
            {
                OriginalStyle = new CharacterStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 8,
                    FontColor = Color.Red,
                    VerticalPosition = CharacterStyle.Position.Superscript
                }
            };
        #endregion
        #region CharacterStyle ChapterNumber
        public static readonly CharacterStyle ChapterNumber =
            new CharacterStyle("Chapter Number")
            {
                OriginalStyle = new CharacterStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 20,
                    DefaultFontStyle = FontStyle.Bold
                }
            };
        #endregion
        #region CharacterStyle FootnoteLetter
        public static readonly CharacterStyle FootnoteLetter =
            new CharacterStyle("Footnote Letter")
            {
                OriginalStyle = new CharacterStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 8,
                    FontColor = Color.Navy,
                    VerticalPosition = CharacterStyle.Position.Superscript
                }
            };
        #endregion
        #region CharacterStyle BigHeader
        public static readonly CharacterStyle BigHeader =
            new CharacterStyle("Header in Window Panes")
            {
                OriginalStyle = new CharacterStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 12,
                    DefaultFontStyle = FontStyle.Bold
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region CharacterStyle Label
        public static readonly CharacterStyle Label =
            new CharacterStyle("Label")
            {
                OriginalStyle = new CharacterStyle(CharacterStyle.c_sOriginalStyle)
                {
                    FontColor = Color.Crimson
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion

        // Paragraph Styles: Scripture Organization
        #region ParagraphStyle RunningHeader
        public static readonly ParagraphStyle RunningHeader =
            new ParagraphStyle("Header")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Bold,
                    Alignment = ParagraphStyle.Align.Justified
                },
                UsfmMarker = "h"
            };
        #endregion
        #region ParagraphStyle BookTitle
        public static readonly ParagraphStyle BookTitle =
            new ParagraphStyle("Title Main")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Bold,
                    DefaultFontSize = 16,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsAfter = 9,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "mt"
            };
        #endregion
        #region ParagraphStyle BookSubTitle
        public static readonly ParagraphStyle BookSubTitle =
            new ParagraphStyle("Title Secondary")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Bold,
                    DefaultFontSize = 14,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsAfter = 9,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "mt2",
                ToolboxMarker = "st"
            };
        #endregion
        #region ParagraphStyle MajorSection
        public static readonly ParagraphStyle MajorSection =
            new ParagraphStyle("Major Section Head")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Bold,
                    DefaultFontSize = 14,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsBefore = 12,
                    PointsAfter = 3,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "ms",
            };
        #endregion
        #region ParagraphStyle MajorSectionCrossReference
        public static readonly ParagraphStyle MajorSectionCrossReference =
            new ParagraphStyle("Major Section Range")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Italic,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsAfter = 3,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "mr",
            };
        #endregion
        #region ParagraphStyle Section
        public static readonly ParagraphStyle Section =
            new ParagraphStyle("Section Head")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Bold,
                    DefaultFontSize = 12,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsBefore = 12,
                    PointsAfter = 3,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "s1",
                ToolboxMarker = "s"
            };
        #endregion
        #region ParagraphStyle SectionCrossReference
        public static readonly ParagraphStyle SectionCrossReference =
            new ParagraphStyle("Parallel Passage Reference")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Italic,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsAfter = 3,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "r"
            };
        #endregion
        #region ParagraphStyle MinorSection
        public static readonly ParagraphStyle MinorSection =
            new ParagraphStyle("Section Head 2")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Bold,
                    DefaultFontSize = 12,
                    Alignment = ParagraphStyle.Align.Centered,
                    PointsBefore = 9,
                    PointsAfter = 3,
                    KeepWithNextParagraph = true
                },
                UsfmMarker = "s2"
            };
        #endregion

        // Paragraph Styles: Scripture Content
        #region ParagraphStyle Paragraph
        public static readonly ParagraphStyle Paragraph = 
            new ParagraphStyle("Paragraph")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle) 
                {
                    Alignment = ParagraphStyle.Align.Justified 
                },
                Uses = CharacterStyle.Usage.IsCannonicalScriptureText,
                UsfmMarker = "p"
           };
        #endregion
        #region ParagraphStyle ParagraphContinuation
        public static readonly ParagraphStyle ParagraphContinuation = 
            new ParagraphStyle("Paragraph Continuation")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle) 
                {
                    Alignment = ParagraphStyle.Align.Justified 
                },
                Uses = CharacterStyle.Usage.IsCannonicalScriptureText,
                UsfmMarker = "m"
            };
        #endregion
        #region ParagraphStyle Line1
        public static readonly ParagraphStyle Line1 =
            new ParagraphStyle("Line 1")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.2
                },
                Uses = CharacterStyle.Usage.IsScripturePoetry | 
                       CharacterStyle.Usage.IsCannonicalScriptureText,
                UsfmMarker = "q1",
                ToolboxMarker = "q"
            };
        #endregion
        #region ParagraphStyle Line2
        public static readonly ParagraphStyle Line2 =
            new ParagraphStyle("Line 2")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.4
                },
                Uses = CharacterStyle.Usage.IsScripturePoetry |
                       CharacterStyle.Usage.IsCannonicalScriptureText,
                UsfmMarker = "q2",
            };
        #endregion
        #region ParagraphStyle Line3
        public static readonly ParagraphStyle Line3 =
            new ParagraphStyle("Line 3")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.6
                },
                Uses = CharacterStyle.Usage.IsScripturePoetry |
                       CharacterStyle.Usage.IsCannonicalScriptureText,
                UsfmMarker = "q3",
            };
        #endregion
        #region ParagraphStyle LineCentered
        public static readonly ParagraphStyle LineCentered =
            new ParagraphStyle("Line Centered")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.2,
                    RightMarginInches = 0.2
                },
                Uses = CharacterStyle.Usage.IsScripturePoetry |
                       CharacterStyle.Usage.IsCannonicalScriptureText,
                UsfmMarker = "qc",
            };
        #endregion
        #region ParagraphStyle PictureCaption
        public static readonly ParagraphStyle PictureCaption =
            new ParagraphStyle("Caption")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontStyle = FontStyle.Italic,
                    Alignment = ParagraphStyle.Align.Centered
                },
                UsfmMarker = "fig",
                ToolboxMarker = "cap"
            };
        #endregion
        #region ParagraphStyle Footnote
        public static readonly ParagraphStyle Footnote =
            new ParagraphStyle("Footnote")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsAfter = 3
                },
                UsfmMarker = "f",
                ToolboxMarker = "fn"
            };
        #endregion

        // Paragraph Styles: Literate Help Window
        #region ParagraphStyle LiterateParagraph
        public static readonly ParagraphStyle LiterateParagraph =
            new ParagraphStyle("Literate Paragraph")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 8,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 3,
                    PointsAfter = 3
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle LiterateHeading
        public static readonly ParagraphStyle LiterateHeading =
            new ParagraphStyle("Literate Heading")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 9,
                    DefaultFontStyle = FontStyle.Bold,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 6,
                    PointsAfter = 3
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle LiterateAttention
        public static readonly ParagraphStyle LiterateAttention =
            new ParagraphStyle("Literate Attention")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 8,
                    DefaultFontStyle = FontStyle.Bold,
                    FontColor = Color.DarkRed,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 3,
                    PointsAfter = 3
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle LiterateList
        public static readonly ParagraphStyle LiterateList =
            new ParagraphStyle("Literate List")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 8,
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.2,
                    PointsBefore = 0,
                    PointsAfter = 0,
                    Bulleted = true
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion

        // Paragraph Styles: Other User Interface
        #region ParagraphStyle ReferenceTranslation
        public static readonly ParagraphStyle ReferenceTranslation =
            new ParagraphStyle("Reference Translation")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 10,
                    Alignment = ParagraphStyle.Align.Justified,
                    FirstLineIndentInches = -0.20,
                    LeftMarginInches = 0.20
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle ReferenceTranslationContinued
        public static readonly ParagraphStyle ReferenceTranslationContinued =
            new ParagraphStyle("Reference Translation Continued")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 10,
                    Alignment = ParagraphStyle.Align.Justified,
                    LeftMarginInches = 0.20
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion

        #region ParagraphStyle TipHeader
        static public readonly ParagraphStyle TipHeader =
            new ParagraphStyle("Tip Header")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    Alignment = ParagraphStyle.Align.Left,
                    PointsBefore = 0,
                    PointsAfter = 0
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle TipContent
        static public readonly ParagraphStyle TipContent =
            new ParagraphStyle("Tip Content")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 9,
                    DefaultFontStyle = FontStyle.Bold,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 0,
                    PointsAfter = 0,
                    LeftMarginInches = 0.1
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle TipText
        static public readonly ParagraphStyle TipText =
            new ParagraphStyle("Tip Text")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 9,
                    DefaultFontStyle = FontStyle.Bold,
                    Alignment = ParagraphStyle.Align.Justified,
                    PointsBefore = 5,
                    PointsAfter = 0
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle TipMessageHanging
        // Used for a hanging-indent paragraph for displaying multiple messages within a
        // TranslationNote. Hanging-indent helps to distinquish individual messages from
        // each other.
        static public readonly ParagraphStyle TipMessageHanging =
            new ParagraphStyle("Tip: Note Discussion")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 10,
                    DefaultFontName = "Gentium",
                    Alignment = ParagraphStyle.Align.Left,
                    PointsBefore = 0,
                    PointsAfter = 3,
                    LeftMarginInches = 0.15,
                    FirstLineIndentInches = -0.15
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion
        #region ParagraphStyle TipBlockParagraph
        // Used for displaying a single-paragraph tooltip; presents a block paragraph
        static public readonly ParagraphStyle TipBlockParagraph =
            new ParagraphStyle("Tip: Block Paragraph")
            {
                OriginalStyle = new ParagraphStyle(CharacterStyle.c_sOriginalStyle)
                {
                    DefaultFontSize = 10,
                    DefaultFontName = "Gentium",
                    Alignment = ParagraphStyle.Align.Left,
                    PointsBefore = 0,
                    PointsAfter = 0
                },
                Uses = CharacterStyle.Usage.OnlyInUserInterface
            };
        #endregion

        // List of Styles --------------------------------------------------------------------
        #region SAttr{g}: List<CharacterStyle> StyleList
        static public List<CharacterStyle> StyleList
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
        #region SMethod: ParagraphStyle FindFromToolboxMarker(sMarker)
        static public ParagraphStyle FindFromToolboxMarker(string sMarker)
        {
            foreach(var style in StyleList)
            {
                var paragraphStyle = style as ParagraphStyle;
                if (null == paragraphStyle)
                    continue;
                if (paragraphStyle.ToolboxMarker == sMarker)
                    return paragraphStyle;
            }
            return null;
        }
        #endregion
        #region SMethod: void ResetStylesToOriginal()
        public static void ResetStylesToOriginal()
        {
            foreach(var style in StyleList)
                style.ResetToOriginal();
        }
        #endregion

        // List of Writing Systems -----------------------------------------------------------
        static private WritingSystem DefaultWritingSystem;
        #region SAttr{g}: List<WritingSystem> WritingSystems
        static public List<WritingSystem> WritingSystems
        {
            get
            {
                if (null == s_WritingSystems)
                {
                    s_WritingSystems = new List<WritingSystem>();
                    DefaultWritingSystem = FindOrCreate(WritingSystem.DefaultWritingSystemName);
                    s_bIsDirty = false;
                }
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
        #region SMethod: WritingSystem FindOrCreate(sWritingSystemName)
        static public WritingSystem FindOrCreate(string sWritingSystemName)
        {
            // Return it if it is already in the list
            var writingSystem = FindWritingSystem(sWritingSystemName);
            if (null != writingSystem)
                return writingSystem;

            // Otherwise create the new one and add it
            writingSystem = new WritingSystem {Name = sWritingSystemName};
            WritingSystems.Add(writingSystem);
            SortWritingSystems();
            DeclareDirty();
            return writingSystem;
        }
        #endregion
        #region SMethod: void SortWritingSystems()
        static public void SortWritingSystems()
        {
            WritingSystems.Sort(WritingSystem.SortCompare);
        }
        #endregion
        #region SMethod: void RemoveWritingSystem(WritingSystem)
        static public void RemoveWritingSystem(WritingSystem writingSystem)
        {
            if (-1 == WritingSystems.IndexOf(writingSystem))
                return;

            WritingSystems.Remove(writingSystem);
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        #region SMethod: void DeclareDirty()
        static public void DeclareDirty()
        {
            s_bIsDirty = true;
        }
        #endregion
        static private bool s_bIsDirty;
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
                var sStyleName = CharacterStyle.GetStyleNameFromXml(node);

                var style = Find(sStyleName);

                if (null != style)
                    style.ReadContent(node);
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
                if (null == ws)
                    continue;

                // Prevent duplicates (e.g., read-in Latin replaces the default Latin)
                var stale = FindWritingSystem(ws.Name);
                if (null != stale)
                    RemoveWritingSystem(stale);

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
            // Start with no writing systems; otherwise we might be keeping around some
            // from a previously loaded project.
            s_WritingSystems = null;

            // This gets rid of any Fonts that might have been created, e.g., if we previously
            // had a different cluster loaded.
            ResetStylesToOriginal();

            // Attenpt to load and read the Xml stylesheet (an empty path is ignored)
            ReadStyleSheet(sPath);

            // The reading process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region SAttr{g}: Font LargeDialogFont
        public static Font LargeDialogFont
        // This font is used for examining raw oxes files. I use a slightly larger
        // font due to the possible presence of diacritics which can otherwise be
        // difficult to read.
        {
            get
            {
                if (null == s_LargeDialogFont)
                {
                    s_LargeDialogFont = new Font(SystemFonts.DialogFont.FontFamily,
                        SystemFonts.DialogFont.Size * 1.2F,
                        FontStyle.Regular);
                }
                return s_LargeDialogFont;
            }
        }
        private static Font s_LargeDialogFont;
        #endregion

    }
}
