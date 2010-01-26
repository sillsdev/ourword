#region ***** TCharacterStyle.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TCharacterStyle.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Tests class CharacterStyle
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Drawing;
using JWTools;
using NUnit.Framework;
using OurWordData.Styles;
#endregion

namespace OurWordTests.Styles
{
    [TestFixture]
    public class TCharacterStyle : CharacterStyle
    {
        #region Constructor()
        public TCharacterStyle() 
            : base("Dummy Style Name")
        {
        }
        #endregion
        #region SMethod: CharacterStyle CreateFromXml(string sXml)
        static CharacterStyle CreateFromXml(string sXml)
        {
            var doc = new XmlDoc(sXml);
            var node = XmlDoc.FindNode(doc, "CharacterStyle");
            var sStyleName = CharacterStyle.GetStyleNameFromXml(node);

            StyleSheet.StyleList.Clear();
            var style = new CharacterStyle(sStyleName);

            style.ReadContent(node);
            return style;
        }
        #endregion

        // Fonts for Writing System ----------------------------------------------------------
        #region Test: TFindFont
        [Test] public void TFindFont()
        {
            const string sStyle =
               "<CharacterStyle Name=\"Verse\" Color=\"Red\">" +
                   "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"11\" />" +
                   "<Font WritingSystem=\"Timor\" Name=\"Gentium\" Size=\"10\" Style=\"Bold\" />" +
              "</CharacterStyle>";

            var style = CreateFromXml(sStyle);

            Assert.IsNotNull(style.FindFontFactory("Latin"));
            Assert.IsNotNull(style.FindFontFactory("Timor"));
        }
        #endregion
        #region Test: TFindFontReturnsNullIfNotPresent
        [Test] public void TFindFontReturnsNullIfNotPresent()
        // This is important when merging, that we return null rather than create
        // a font for the given writing system; so that merge only deals with the
        // one explicitly created by, e.g., "theirs"
        {
            const string sStyle =
               "<CharacterStyle Name=\"Verse\" Color=\"Red\">" +
                   "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"11\" />" +
                   "<Font WritingSystem=\"Timor\" Name=\"Gentium\" Size=\"10\" Style=\"Bold\" />" +
              "</CharacterStyle>";

            var style = CreateFromXml(sStyle);

            Assert.IsNull(style.FindFontFactory("Tomohon"));
        }
        #endregion
        #region Test: TFindOrAddFont
        [Test] public void TFindOrAddFont()
        {
            Assert.AreEqual(0, FontFactories.Count);

            // Add a font
            var font = FindOrAddFontFactory("Tomohon");

            // The defaults should be what we're set to
            Assert.AreEqual(1, FontFactories.Count);
            Assert.AreEqual(m_DefaultFont.FontName, font.FontName);
            Assert.AreEqual(m_DefaultFont.FontSize, font.FontSize);
            Assert.AreEqual(m_DefaultFont.FontStyle, font.FontStyle);

            // Searching for it again should not add it again
            font = FindOrAddFontFactory("Tomohon");
            Assert.IsNotNull(font);
            Assert.AreEqual(1, FontFactories.Count);
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        private const string c_sXmlForIoTest =
            "<CharacterStyle Name=\"Verse\" Color=\"Red\" FontName=\"Gentium\" FontSize=\"8\" FontStyle=\"Italic\">" +
                "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"10\" />" +
            "</CharacterStyle>";
        #region Test: TSave
        [Test] public void TSave()
        {
            var style = new CharacterStyle("Verse")
            {
                FontColor = Color.Red,
                DefaultFontName = "Gentium",
                DefaultFontSize = 8,
                DefaultFontStyle = FontStyle.Italic
            };
            style.FontFactories.Add(new FontFactory 
                {
                    WritingSystemName = "Latin", 
                    FontName = "Arial", 
                    FontSize = 10.0F
                });

            var doc = new XmlDoc();
            var node = style.Save(doc, null);

            Assert.AreEqual(c_sXmlForIoTest, node.OuterXml);
        }
        #endregion
        #region Test: TCreate
        [Test] public void TCreate()
        {
            var style = CreateFromXml(c_sXmlForIoTest);

            Assert.AreEqual("Verse", style.StyleName);
            Assert.AreEqual(Color.Red, style.FontColor);

            Assert.AreEqual(1, style.FontFactories.Count);
            Assert.AreEqual("Arial", style.FontFactories[0].FontName);
        }
        #endregion
        #region Test: TMerge
        [Test] public void TMerge()
        {
            const string sParent =
               "<CharacterStyle Name=\"Verse\" Color=\"Red\" FontName=\"Roman\" FontSize=\"8\">" +
                   "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"11\" />" +
                   "<Font WritingSystem=\"Timor\" Name=\"Gentium\" Size=\"10\" Style=\"Bold\" />" +
              "</CharacterStyle>";

            var sOurs = sParent.Replace("Gentium", "Playbill");
            sOurs = sOurs.Replace("Red", "Yellow");

            var sTheirs = sParent.Replace("11", "12");
            sTheirs = sTheirs.Replace("</C", "<Font WritingSystem=\"Tomohon\" Name=\"Courier\" Size=\"12\" /></C");

            const string sExpected =
                "<CharacterStyle Name=\"Verse\" Color=\"Yellow\" FontName=\"Roman\" FontSize=\"8\">" +
                    "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"12\" />" +
                    "<Font WritingSystem=\"Timor\" Name=\"Playbill\" Size=\"10\" Style=\"Bold\" />" +
                    "<Font WritingSystem=\"Tomohon\" Name=\"Courier\" Size=\"12\" />" +
                "</CharacterStyle>";

            var parent = CreateFromXml(sParent);
            var ours = CreateFromXml(sOurs);
            var theirs = CreateFromXml(sTheirs);

            ours.Merge(parent, theirs);

            var sActual = ours.Save(new XmlDoc(), null).OuterXml;
            Assert.AreEqual(sExpected, sActual);
        }
        #endregion

    }
}
