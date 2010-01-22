#region ***** TParagraphStyle.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TParagraphStyle.cs
 * Author:  John Wimbish
 * Created: 11 Dec 2009
 * Purpose: Tests class ParagraphStyle
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
    public class TParagraphStyle : ParagraphStyle
    {
        #region Constructor()
        public TParagraphStyle()
            : base("Dummy Paragraph Style Name")
        {
        }
        #endregion
        #region SMethod: ParagraphStyle CreateFromXml(string sXml)
        static ParagraphStyle CreateFromXml(string sXml)
        {
            var doc = new XmlDoc(sXml);
            var node = XmlDoc.FindNode(doc, "ParagraphStyle");
            var style = Create(node);
            return style;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        private const string c_sXmlForIoTest =
            "<ParagraphStyle Name=\"SillyParagraph\" Color=\"Red\" Before=\"3\" After=\"6\" " +
                "FirstLineIndent=\"-0.25\" LeftMargin=\"0.5\" RightMargin=\"0.25\" " +
                "KeepWithNext=\"true\" Bulleted=\"true\" Alignment=\"Justified\">" +
                    "<Font WritingSystem=\"Latin\" Name=\"Playbill\" Size=\"11\" />" +
            "</ParagraphStyle>";
        #region Test: TSave
        [Test] public void TSave()
        {
            var style = new ParagraphStyle("SillyParagraph")
            {
                FontColor = Color.Red,
                PointsBefore = 3,
                PointsAfter = 6,
                FirstLineIndentInches = -0.25,
                LeftMarginInches = 0.5,
                RightMarginInches = 0.25,
                KeepWithNextParagraph = true,
                Bulleted = true,
                Alignment = Align.Justified
            };
            style.FontFactories.Add(new FontFactory
            {
                WritingSystemName = "Latin",
                FontName = "Playbill",
                FontSize = 11.0F
            });

            var node = style.Save(new XmlDoc(), null);

            Assert.AreEqual(c_sXmlForIoTest, node.OuterXml);
        }
        #endregion
        #region Test: TCreate
        [Test] public void TCreate()
        {
            var doc = new XmlDoc(c_sXmlForIoTest);
            var node = XmlDoc.FindNode(doc, "ParagraphStyle");
            var style = Create(node);

            Assert.AreEqual("SillyParagraph", style.StyleName);
            Assert.AreEqual(Color.Red, style.FontColor);

            Assert.AreEqual(1, style.FontFactories.Count);
            Assert.AreEqual("Playbill", style.FontFactories[0].FontName);

            Assert.AreEqual(3, style.PointsBefore);
            Assert.AreEqual(6, style.PointsAfter);
            Assert.AreEqual(-0.25, style.FirstLineIndentInches);
            Assert.AreEqual(0.5, style.LeftMarginInches);
            Assert.AreEqual(0.25, style.RightMarginInches);
            Assert.AreEqual(true, style.KeepWithNextParagraph);
            Assert.AreEqual(true, style.Bulleted);
            Assert.AreEqual(Align.Justified, style.Alignment);
        }
        #endregion
        #region Test: TMerge
        [Test] public void TMerge()
        {
            const string sParent = c_sXmlForIoTest;

            var sOurs = sParent;
            sOurs = sOurs.Replace("FirstLineIndent=\"-0.25\"", "FirstLineIndent=\"-0.15\"");
            sOurs = sOurs.Replace("LeftMargin=\"0.5\"", "LeftMargin=\"0.3\"");
            sOurs = sOurs.Replace("RightMargin=\"0.25\"", "RightMargin=\"0.15\"");

            var sTheirs = sParent;
            sTheirs = sTheirs.Replace("Alignment=\"Justified\"", "Alignment=\"Left\"");

            var sExpected = sParent;
            sExpected = sExpected.Replace("FirstLineIndent=\"-0.25\"", "FirstLineIndent=\"-0.15\"");
            sExpected = sExpected.Replace("LeftMargin=\"0.5\"", "LeftMargin=\"0.3\"");
            sExpected = sExpected.Replace("RightMargin=\"0.25\"", "RightMargin=\"0.15\"");
            sExpected = sExpected.Replace("Alignment=\"Justified\"", "Alignment=\"Left\"");

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
