using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using JWTools;

namespace OurWordData.Styles
{
    public class Styles
    {
        #region SMethod: void DeclareDirty()
        static public void DeclareDirty()
        {
            s_bIsDirty = true;
        }
        #endregion
        static private bool s_bIsDirty;

        // Character Styles
        static public CharacterStyle VerseNumber;
        static public CharacterStyle ChapterNumber;
        static public CharacterStyle FootnoteLetter;

        // Paragraph Styles
        static public ParagraphStyle Normal;
        static public ParagraphStyle Line1;
        static public ParagraphStyle Line2;

        // List of Styles
        private static List<CharacterStyle> s_StyleList;
        #region Method: CharacterStyle Find(string sStyleName)
        static CharacterStyle Find(string sStyleName)
        {
            foreach (var style in s_StyleList)
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
            var existing = Find(style.StyleName) as CharacterStyle;

            if (null == existing)
            {
                s_StyleList.Add(style);
                return style;
            }

            existing.SetDefaults(style);
            return existing;
        }
        #endregion

        static public void Initialize()
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
            if (null == s_StyleList)
                s_StyleList = new List<CharacterStyle>();

            // Character Styles
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

            // Paragraph Styles
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

            // The initialize process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }

        // I/O & Merge -----------------------------------------------------------------------
        private const string c_sTag = "StyleSheet";
        #region SMethod: void Save(string sPath)
        static public void Save(string sPath)
        {
            if (!s_bIsDirty)
                return;

            var doc = new XmlDoc();
            doc.AddXmlDeclaration();

            var nodeStyleSheet = doc.AddNode(null, c_sTag);

            foreach (var style in s_StyleList)
                style.Save(doc, nodeStyleSheet);

            doc.Write(sPath);

            s_bIsDirty = false;
        }
        #endregion
        #region SMethod: void Read(string sPath)
        static public void Read(string sPath)
        {
            s_StyleList = new List<CharacterStyle>();

            var doc = new XmlDoc();
            doc.Load(sPath);

            var nodeStyleSheet = XmlDoc.FindNode(doc, c_sTag);
            if (null != nodeStyleSheet)
            {
                foreach(XmlNode nodeStyle in nodeStyleSheet.ChildNodes)
                {
                    var style = CharacterStyle.Create(nodeStyle) ?? 
                        ParagraphStyle.Create(nodeStyle);

                    if (null != style)
                        s_StyleList.Add(style);
                }
            }

            Initialize();

            // The reading process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }
        #endregion
    }
}
