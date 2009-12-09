#region ***** TFontForWritingSystem.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TFontForWritingSystem.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Tests class FontForWritingSystem
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
    public class TFontForWritingSystem : FontForWritingSystem
    {
        #region SMethod: FontForWritingSystem CreateFromXml(string sXml)
        static FontForWritingSystem CreateFromXml(string sXml)
        {
            var doc = new XmlDoc(sXml);
            var node = XmlDoc.FindNode(doc, c_sTag);
            var style = Create(node);
            return style;
        }
        #endregion

        // FontStyle to FontStyleAsString ----------------------------------------------------
        #region Test: TFontStyleAsString_Get
        [Test] public void TFontStyleAsString_Get()
        {
            var obj = new TFontForWritingSystem();

            Assert.AreEqual("", obj.FontStyleAsString);

            obj.FontStyle = FontStyle.Bold;
            Assert.AreEqual("Bold", obj.FontStyleAsString);

            obj.FontStyle = FontStyle.Italic;
            Assert.AreEqual("Italic", obj.FontStyleAsString);

            obj.FontStyle = FontStyle.Underline;
            Assert.AreEqual("Underline", obj.FontStyleAsString);

            obj.FontStyle = FontStyle.Strikeout;
            Assert.AreEqual("Strikeout", obj.FontStyleAsString);

            obj.FontStyle = FontStyle.Bold | FontStyle.Italic;
            Assert.AreEqual("BoldItalic", obj.FontStyleAsString);

            obj.FontStyle = FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout;
            Assert.AreEqual("BoldUnderlineStrikeout", obj.FontStyleAsString);
        }
        #endregion
        #region Test: TFontStyleAsString_Set
        [Test] public void TFontStyleAsString_Set()
        {
            var obj = new TFontForWritingSystem();

            Assert.AreEqual("", obj.FontStyleAsString);

            obj.FontStyleAsString = "Bold";
            Assert.AreEqual(FontStyle.Bold, obj.FontStyle);

            obj.FontStyleAsString = "Italic";
            Assert.AreEqual(FontStyle.Italic, obj.FontStyle);

            obj.FontStyleAsString = "Underline";
            Assert.AreEqual(FontStyle.Underline, obj.FontStyle);

            obj.FontStyleAsString = "Strikeout";
            Assert.AreEqual(FontStyle.Strikeout, obj.FontStyle);

            obj.FontStyleAsString = "BoldItalic";
            Assert.AreEqual(FontStyle.Bold, obj.FontStyle & FontStyle.Bold);
            Assert.AreEqual(FontStyle.Italic, obj.FontStyle & FontStyle.Italic);
            Assert.AreNotEqual(FontStyle.Underline, obj.FontStyle & FontStyle.Underline);
            Assert.AreNotEqual(FontStyle.Strikeout, obj.FontStyle & FontStyle.Strikeout);

            obj.FontStyleAsString = "BoldUnderlineStrikeout";
            Assert.AreEqual(FontStyle.Bold, obj.FontStyle & FontStyle.Bold);
            Assert.AreNotEqual(FontStyle.Italic, obj.FontStyle & FontStyle.Italic);
            Assert.AreEqual(FontStyle.Underline, obj.FontStyle & FontStyle.Underline);
            Assert.AreEqual(FontStyle.Strikeout, obj.FontStyle & FontStyle.Strikeout);
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Test: TClone
        [Test] public void TClone()
        {
            var original = new FontForWritingSystem
            {
                WritingSystemName = "Latin",
                FontName = "PLaybill",
                FontSize = 13.0F,
                FontStyle = FontStyle.Italic
            };

            var cloned = original.Clone();

            Assert.AreEqual(original.WritingSystemName, cloned.WritingSystemName);
            Assert.AreEqual(original.FontName, cloned.FontName);
            Assert.AreEqual(original.FontSize, cloned.FontSize);
            Assert.AreEqual(original.FontStyle, cloned.FontStyle);
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        private const string c_sXmlForIoTest = "<Font WritingSystem=\"Cherokee\" " +
            "Name=\"Gentium\" Size=\"10.3\" Style=\"BoldStrikeout\" />";
        #region Test: TSave
        [Test] public void TSave()
        {
            var obj = new TFontForWritingSystem
            {
                FontName = "Gentium",
                FontSize = 10.3F,
                FontStyle = FontStyle.Bold | FontStyle.Strikeout,
                WritingSystemName = "Cherokee"
            };

            var doc = new XmlDoc();
            var node = obj.Save(doc, null);

            Assert.AreEqual(c_sXmlForIoTest, node.OuterXml);
        }
        #endregion
        #region Test: TCreate
        [Test] public void TCreate()
        {
            var doc = new XmlDoc(c_sXmlForIoTest);
            var node = XmlDoc.FindNode(doc, "Font");
            var obj = Create(node);

            Assert.AreEqual("Gentium", obj.FontName);
            Assert.AreEqual(10.3F, obj.FontSize);

            Assert.AreEqual(FontStyle.Bold, obj.FontStyle & FontStyle.Bold);
            Assert.AreNotEqual(FontStyle.Italic, obj.FontStyle & FontStyle.Italic);
            Assert.AreEqual(FontStyle.Strikeout, obj.FontStyle & FontStyle.Strikeout);
            Assert.AreNotEqual(FontStyle.Underline, obj.FontStyle & FontStyle.Underline);

            Assert.AreEqual("Cherokee", obj.WritingSystemName);

        }
        #endregion
        #region Test: TMerge
        [Test] public void TMerge()
            // Ours changes Name:  Ours should be kept
            // Theirs changes Size: Theirs should be kept
            // Both change Style: Ours should be kept
        {
            const string sParent = "<Font WritingSystem=\"Cherokee\" Name=\"Gentium\" Size=\"10.3\" Style=\"BoldStrikeout\" />";
            const string sOurs = "<Font WritingSystem=\"Cherokee\" Name=\"Arial\" Size=\"10.3\" Style=\"Italic\" />";
            const string sTheirs = "<Font WritingSystem=\"Cherokee\" Name=\"Gentium\" Size=\"11\" Style=\"Bold\" />";
            const string sExpected = "<Font WritingSystem=\"Cherokee\" Name=\"Arial\" Size=\"11\" Style=\"Italic\" />";

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
