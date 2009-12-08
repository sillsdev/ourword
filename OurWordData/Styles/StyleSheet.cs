using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
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

        // List of Styles
        private static List<Style> s_StyleList;
        #region Method: Style Find(string sStyleName)
        static Style Find(string sStyleName)
        {
            foreach (var style in s_StyleList)
            {
                if (style.StyleName == sStyleName)
                    return style;
            }
            return null;
        }
        #endregion

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


        static public void Initialize()
        {
            if (null == s_StyleList)
                s_StyleList = new List<Style>();

            VerseNumber = InitializeStyle(new CharacterStyle("Verse Number") 
            {
                FontColor = Color.Red,
                DefaultFontSize = 10
            });

            ChapterNumber = InitializeStyle(new CharacterStyle("Chapter Number") 
            { 
                DefaultFontSize = 20,
                DefaultFontStyle = FontStyle.Bold
            });

            FootnoteLetter = InitializeStyle(new CharacterStyle("Footnote Letter")
            {
                FontColor = Color.Navy,
                DefaultFontSize = 10
            });

            // The initialize process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }

        // I/O -------------------------------------------------------------------------------
        private const string c_sTag = "StyleSheet";

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

        static public void Read(string sPath)
        {
            s_StyleList = new List<Style>();

            var doc = new XmlDoc();
            doc.Load(sPath);

            var nodeStyleSheet = XmlDoc.FindNode(doc, c_sTag);

            foreach(XmlNode nodeStyle in nodeStyleSheet.ChildNodes)
            {
                var style = CharacterStyle.Create(nodeStyle);
                if (null != style)
                    s_StyleList.Add(style);
            }

            Initialize();

            // The reading process sets attributes, which in turn sets the dirty
            // flag; so we need to clear it now that we're all done
            s_bIsDirty = false;
        }

    }
}
