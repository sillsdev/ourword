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

        // I/O -------------------------------------------------------------------------------
        private const string c_sXmlForIoTest = 
            "<CharacterStyle Name=\"Verse\" Color=\"Red\">" +
                "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"10\" />" +
            "</CharacterStyle>";

        #region Test: TSave
        [Test] public void TSave()
        {
            var style = new CharacterStyle("Verse")
            {
                FontColor = Color.Red
            };
            style.FontsForWritingSystem.Add(new FontForWritingSystem 
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
            var doc = new XmlDoc(c_sXmlForIoTest);
            var node = XmlDoc.FindNode(doc, "CharacterStyle");
            var style = Create(node);

            Assert.AreEqual("Verse", style.StyleName);
            Assert.AreEqual(Color.Red, style.FontColor);

            Assert.AreEqual(1, style.FontsForWritingSystem.Count);
            Assert.AreEqual("Arial", style.FontsForWritingSystem[0].FontName);
        }
        #endregion

        #region Test: TMerge
        [Test] public void TMerge()
        {
            const string sParent = 
                "<CharacterStyle Name=\"Verse\" Color=\"Red\">" +
                    "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"11\" />" +
                    "<Font WritingSystem=\"Timor\" Name=\"Gentium\" Size=\"10\" Style=\"Bold\" />" +
               "</CharacterStyle>";

            var sOurs = sParent.Replace("Gentium", "Playbill");
            sOurs = sOurs.Replace("Red", "Yellow");

            var sTheirs = sParent.Replace("11", "12");
            sTheirs = sTheirs.Replace("</C", "<Font WritingSystem=\"Tomohon\" Name=\"Courier\" Size=\"12\" /></C");

            const string sExpected =
                "<CharacterStyle Name=\"Verse\" Color=\"Yellow\">" +
                    "<Font WritingSystem=\"Latin\" Name=\"Arial\" Size=\"12\" />" +
                    "<Font WritingSystem=\"Timor\" Name=\"Playbill\" Size=\"10\" Style=\"Bold\" />" +
                    "<Font WritingSystem=\"Tomohon\" Name=\"Courier\" Size=\"12\" />" +
                "</CharacterStyle>";

            var nodeParent = XmlDoc.FindNode((new XmlDoc(sParent)), "CharacterStyle");
            var nodeOurs = XmlDoc.FindNode((new XmlDoc(sOurs)), "CharacterStyle");
            var nodeTheirs = XmlDoc.FindNode((new XmlDoc(sTheirs)), "CharacterStyle");

            Merge(nodeOurs, nodeTheirs, nodeParent);

            Assert.AreEqual(sExpected, nodeOurs.OuterXml);
        }
        #endregion
    }
}
