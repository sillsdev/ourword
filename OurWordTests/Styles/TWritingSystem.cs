#region ***** TWritingSystem.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TWritingSystem.cs
 * Author:  John Wimbish
 * Created: 12 Dec 2009
 * Purpose: Tests class WritingSystem
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using JWTools;
using NUnit.Framework;
using OurWordData;
using OurWordData.Styles;
#endregion


namespace OurWordTests.Styles
{
    [TestFixture]
    public class TWritingSystem : WritingSystem
    {
        #region SMethod: WritingSystem CreateFromXml(string sXml)
        static WritingSystem CreateFromXml(string sXml)
        {
            var doc = new XmlDoc(sXml);
            var node = XmlDoc.FindNode(doc, c_sTag);
            var ws = Create(node);
            return ws;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        private const string c_sXmlForIoTest =
            "<WritingSystem Name=\"Amharic\" IdeaGraph=\"true\" Punct=\"&lt;&gt;?!()[]\" " +
            "EndPunct=\"()\" Keyboard=\"Ethiopic\" Consonants=\"dfghjk\" " +
            "UseAutoHyph=\"true\" AutoHyphCV=\"V-VC\" AutoHyphMinSplit=\"5\" " +
            "AutoReplaceSource=\"2 {^a} {^A}\" " +
            "AutoReplaceResult=\"2 {á} {Á}\" />";
        #region Test: TSave
        [Test] public void TSave()
        {
            var ws = new WritingSystem
            {
                Name="Amharic",
                IsIdeaGraph=true,
                PunctuationChars = "<>?!()[]",
                EndPunctuationChars = "()",
                KeyboardName = "Ethiopic",
                Consonants = "dfghjk",
                UseAutomatedHyphenation = true,
                HyphenationCVPattern = "V-VC",
                MinHyphenSplit = 5
            };

            ws.AutoReplaceSource.Clear();
            ws.AutoReplaceResult.Clear();

            ws.AutoReplaceSource.Append("^a");
            ws.AutoReplaceResult.Append("á");

            ws.AutoReplaceSource.Append("^A");
            ws.AutoReplaceResult.Append("Á");

            var doc = new XmlDoc();
            var node = ws.Save(doc, null);

            Assert.AreEqual(c_sXmlForIoTest, node.OuterXml);
        }
        #endregion
        #region Test: TCreate
        [Test] public void TCreate()
        {
            var doc = new XmlDoc(c_sXmlForIoTest);
            var node = XmlDoc.FindNode(doc, c_sTag);
            var ws = Create(node);

            Assert.AreEqual("Amharic", ws.Name);
            Assert.AreEqual(true, ws.IsIdeaGraph);
            Assert.AreEqual("<>?!()[]", ws.PunctuationChars);
            Assert.AreEqual("()", ws.EndPunctuationChars);
            Assert.AreEqual("Ethiopic", ws.KeyboardName);

            Assert.AreEqual("dfghjk", ws.Consonants);
            Assert.AreEqual(true, ws.UseAutomatedHyphenation);
            Assert.AreEqual("V-VC", ws.HyphenationCVPattern);
            Assert.AreEqual(5, ws.MinHyphenSplit);

            Assert.AreEqual("2 {^a} {^A}", ws.AutoReplaceSource.SaveLine);
            Assert.AreEqual("2 {á} {Á}", ws.AutoReplaceResult.SaveLine);
        }
        #endregion
        #region Test: TMerge
        [Test] public void TMerge()
        {
            const string sParent = c_sXmlForIoTest;

            var sOurs = sParent;
            sOurs = sOurs.Replace("Ethiopic", "International");
            sOurs = sOurs.Replace("2 {á} {Á}", "2 {áá} {Áá}");
            sOurs = sOurs.Replace("dfghjk", "DFGHJKdfghjk");

            var sTheirs = sParent;
            sTheirs = sTheirs.Replace("IdeaGraph=\"true\"", "");
            sTheirs = sTheirs.Replace("5", "12");

            const string sExpected = "<WritingSystem Name=\"Amharic\" Punct=\"&lt;&gt;?!()[]\" " +
                "EndPunct=\"()\" Keyboard=\"International\" Consonants=\"DFGHJKdfghjk\" " +
                "UseAutoHyph=\"true\" AutoHyphCV=\"V-VC\" AutoHyphMinSplit=\"12\" " +
                "AutoReplaceSource=\"2 {^a} {^A}\" " +
                "AutoReplaceResult=\"2 {áá} {Áá}\" />";

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
