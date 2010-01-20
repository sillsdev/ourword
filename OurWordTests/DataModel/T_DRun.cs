/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DRun.cs
 * Author:  John Wimbish
 * Created: 11 Jul 2008
 * Purpose: Tests the DRun class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;
using OurWordData.DataModel;

using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

namespace OurWordTests.DataModel
{
    #region CLASS: T_DRun
    [TestFixture] public class T_DRun
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();

            OurWordMain.App = new OurWordMain();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
        }
        #endregion

        // Test GetPhrase---------------------------------------------------------------------
        #region Method: void _Evaluate(Phrase p, sStyleAbbrev, sText)
        void _Evaluate(DSection.IO.Phrase p, FontStyle modification, string sText)
        {
            Assert.AreEqual(p.Modification, modification);
            Assert.AreEqual(p.Text, sText);
        }
        #endregion
        #region Test: Test_GetPhrase
        [Test] public void Test_GetPhrase()
        {
            // Test: a footnote in the midst of text
            var s = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            var iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "Ije lais alekot. Ije Uis-neno In Anmone");

            Assert.IsTrue(DSection.IO.Phrase.GetPhrase(s, ref iPos).IsFootLetter);

            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "in na' monin.");

            // Test: Looks like a footnote, but isn't.
            s = "Ije lais alekot. Ije ||Uis-neno In Anmone||fn in na' monin.";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "Ije lais alekot. Ije |Uis-neno In Anmone|fn in na' monin.");

            // Test: An italic phrase and a literal look-alike
            s = "This is |iitalic|r and this is ||inot.||r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "This is ");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Italic,
                "italic");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                " and this is |inot.|r");

            // Test: Bold at the beginning and a literal look-alike.
            s = "|bThis is bold|r and this is ||bnot||r bold.";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Bold,
                "This is bold");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                " and this is |bnot|r bold.");

            // Test: Underline at the end and a literal look-alike
            s = "||uThis is not underlined||r and this is |uunderlined.|r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "|uThis is not underlined|r and this is ");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Underline,
                "underlined.");

            // Test: Lots of literals
            s = "||||This i||s a str||ing with a b||unch of li||terals in it.||||";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "||This i|s a str|ing with a b|unch of li|terals in it.||");

            // Test: Footnote at the end
            s = "There is a footnote at the end.|fn";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                FontStyle.Regular,
                "There is a footnote at the end.");

            Assert.IsTrue(DSection.IO.Phrase.GetPhrase(s, ref iPos).IsFootLetter);

            // Test: nothing but a footnote
            s = "|fn";
            iPos = 0;
            Assert.IsTrue(DSection.IO.Phrase.GetPhrase(s, ref iPos).IsFootLetter);

            // Test: nothing but two footnotes
            s = "|fn|fn";
            iPos = 0;
            Assert.IsTrue(DSection.IO.Phrase.GetPhrase(s, ref iPos).IsFootLetter);
            Assert.IsTrue(DSection.IO.Phrase.GetPhrase(s, ref iPos).IsFootLetter);
        }
        #endregion

        // Test FieldToRuns-------------------------------------------------------------------
        #region Method: void CheckFoot(List<DRun>, iRun)
        void CheckFoot(List<DRun> vRuns, int iRun)
        {
            DFoot foot = vRuns[iRun] as DFoot;
            Assert.IsNotNull(foot);
        }
        #endregion
        #region Method: void CheckText(List<DRun>, iRun, iPhrase, sTextExpected)
        void CheckText(List<DRun> vRuns, int iRun, int iPhrase, string sTextExpected)
        {
            DText txt = vRuns[iRun] as DText;
            Assert.IsNotNull(txt);

            DPhrase phrase = txt.Phrases[iPhrase] as DPhrase;
            Assert.IsNotNull(phrase);

            Assert.AreEqual(sTextExpected, phrase.Text);
        }
        #endregion
        #region Method: void CheckBT(List<DRun>, iRun, iPhrase, sTextExpected)
        void CheckBT(List<DRun> vRuns, int iRun, int iPhrase, string sTextExpected)
        {
            DText txt = vRuns[iRun] as DText;
            Assert.IsNotNull(txt);

            DPhrase phrase = txt.PhrasesBT[iPhrase] as DPhrase;
            Assert.IsNotNull(phrase);

            Assert.AreEqual(sTextExpected, phrase.Text);
        }
        #endregion
        #region Test: FieldToRuns
        [Test] public void FieldToRuns()
        {
            var field = new SfField("vt", "", 1);
            List<DRun> vRuns;

            // Test: data with footnote in the middle
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good story/matter. This is *God's Son's|fn life.";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This is a good story/matter. This is *God's Son's");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "life.");

            // Back translation does not have a footnote
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good story/matter. This is *God's Son's life.";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This is a good story/matter. This is *God's Son's life.");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "");

            // Back translation has too many footnotes
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good|fn story/matter. This is *God's Son's|fn life.";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This is a good");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "story/matter. This is *God's Son's life.");

            // Back translation has WAY too many footnotes
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This|fn is a good|fn story/matter.|fn This is *God's Son's|fn life.|fn";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "is a good story/matter. This is *God's Son's life.");

            // Missing Back Translation
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "");

            // Test: Missing vernacular
            field.Data = "";
            field.BT = "This is a good story/matter. This is *God's Son's|fn life.";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(0, vRuns.Count);

            // Test: data with italic phrase in the middle
            field.Data = "Ije lais alekot. Ije |iUis-neno In Anmone|r in na' monin.";
            field.BT = "This is a good story/matter. This is |i*God's Son's|r life.";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(1, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije ");
            CheckBT(vRuns, 0, 0, "This is a good story/matter. This is ");
            CheckText(vRuns, 0, 1, "Uis-neno In Anmone");
            CheckBT(vRuns, 0, 1, "*God's Son's");
            CheckText(vRuns, 0, 2, " in na' monin.");
            CheckBT(vRuns, 0, 2, " life.");

            // Test: data with heaps of phrases
            field.Data = "|bIje|r lais |ualekot|r. Ije |iUis-neno In Anmone|r in na' monin.";
            field.BT = "|bThis|r is a good |ustory/matter|r. This is |i*God's Son's|r life.";
            vRuns = DSection.IO.FieldToRuns(field);
            Assert.AreEqual(1, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije");
            CheckBT(vRuns, 0, 0, "This");
            CheckText(vRuns, 0, 1, " lais ");
            CheckBT(vRuns, 0, 1, " is a good ");
            CheckText(vRuns, 0, 2, "alekot");
            CheckBT(vRuns, 0, 2, "story/matter");
            CheckText(vRuns, 0, 3, ". Ije ");
            CheckBT(vRuns, 0, 3, ". This is ");
            CheckText(vRuns, 0, 4, "Uis-neno In Anmone");
            CheckBT(vRuns, 0, 4, "*God's Son's");
            CheckText(vRuns, 0, 5, " in na' monin.");
            CheckBT(vRuns, 0, 5, " life.");

//			CheckText(runs, 0, 0, "");
//			CheckBT(  runs, 0, 0, "");
        }
        #endregion

        // Blank Spaces handling -------------------------------------------------------------
        #region Test: EliminateSpaces
        [Test] public void EliminateSpaces()
        {
            // For an individual phrase, it should remove all doubles (but leave a
            // single leading and trailing space.)
            var sIn = "  This  has     too    many spaces.   ";
            var phrase = new DPhrase(sIn);
            phrase.Text = DPhrase.EliminateSpuriousSpaces(phrase.Text);
            Assert.AreEqual(phrase.Text, " This has too many spaces. ");

            // For a paragraph, we get rid of leading and trailing as well.
            var text = DText.CreateSimple(sIn);
            text.Phrases.InsertAt(0, new DPhrase(" Leading    Italic Phrase. ") 
                {FontModification = FontStyle.Italic});
            text.Phrases.Append(new DPhrase(" Trailing Italic    Phrase.  ") 
                {FontModification = FontStyle.Italic});

            var phrase1 = text.Phrases[0];
            var phrase2 = text.Phrases[1];
            var phrase3 = text.Phrases[2];

            phrase1.Text = DPhrase.EliminateSpuriousSpaces(phrase1.Text);
            phrase2.Text = DPhrase.EliminateSpuriousSpaces(phrase2.Text);
            phrase3.Text = DPhrase.EliminateSpuriousSpaces(phrase3.Text);

            text.EliminateSpuriousSpaces();

            var sComposite = phrase1.Text + phrase2.Text + phrase3.Text;
            Assert.AreEqual("Leading Italic Phrase. This has too many spaces. Trailing Italic Phrase.",
                sComposite);
        }
        #endregion

        // WordList --------------------------------------------------------------------------
        #region Test: TextToWordList
        void _Evaluate(int i, int iPosExpected, string sWordExpected, ArrayList aWords, ArrayList aPositions)
        {
            Assert.AreEqual(sWordExpected, aWords[i] as string);
            Assert.AreEqual(iPosExpected, (int)aPositions[i]);
        }
        [Test] public void TextToWordList()
        {
            var aWords = new ArrayList();
            var aPositions = new ArrayList();

            // Normal example of text that we would expect
            var text = DText.CreateSimple("A hard worker has plenty ");
            text.Phrases.Append(new DPhrase("of food, ") { FontModification = FontStyle.Italic });
            text.Phrases.Append(new DPhrase("but ") { FontModification = FontStyle.Bold });
            text.Phrases.Append(new DPhrase("a person who chases fantasies "));
            text.Phrases.Append(new DPhrase("ends up ") { FontModification = FontStyle.Italic });
            text.Phrases.Append(new DPhrase("in poverty."));

            text.GetWordOffsetPairs(ref aWords, ref aPositions);

            _Evaluate( 0,  0, "A",         aWords, aPositions);
            _Evaluate( 1,  2, "hard",      aWords, aPositions);
            _Evaluate( 2,  7, "worker",    aWords, aPositions);
            _Evaluate( 3, 14, "has",       aWords, aPositions);
            _Evaluate( 4, 18, "plenty",    aWords, aPositions);
            _Evaluate( 5, 25, "of",        aWords, aPositions);
            _Evaluate( 6, 28, "food,",     aWords, aPositions);
            _Evaluate( 7, 34, "but",       aWords, aPositions);
            _Evaluate( 8, 38, "a",         aWords, aPositions);
            _Evaluate( 9, 40, "person",    aWords, aPositions);
            _Evaluate(10, 47, "who",       aWords, aPositions);
            _Evaluate(11, 51, "chases",    aWords, aPositions);
            _Evaluate(12, 58, "fantasies", aWords, aPositions);
            _Evaluate(13, 68, "ends",      aWords, aPositions);
            _Evaluate(14, 73, "up",        aWords, aPositions);
            _Evaluate(15, 76, "in",        aWords, aPositions);
            _Evaluate(16, 79, "poverty.",  aWords, aPositions);

            // Pathalogical example
            text = DText.CreateSimple("This . is a pathalogical ,!: case. ");
            text.GetWordOffsetPairs(ref aWords, ref aPositions);
            _Evaluate(0,  0, "This",         aWords, aPositions);
            _Evaluate(1,  7, "is",           aWords, aPositions);
            _Evaluate(2, 10, "a",            aWords, aPositions);
            _Evaluate(3, 12, "pathalogical", aWords, aPositions);
            _Evaluate(4, 29, "case.",        aWords, aPositions);
        }
        #endregion
    }
    #endregion

    #region CLASS: T_DFoot
    [TestFixture] public class T_DFoot
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: FootnoteLetter_abc
        [Test] public void FootnoteLetter_abc()
        {
            Assert.AreEqual("a", DFoot.GetFootnoteLetter_abc(0));
            Assert.AreEqual("c", DFoot.GetFootnoteLetter_abc(2));
            Assert.AreEqual("z", DFoot.GetFootnoteLetter_abc(25));

            Assert.AreEqual("a", DFoot.GetFootnoteLetter_abc(26));
            Assert.AreEqual("b", DFoot.GetFootnoteLetter_abc(27));
            Assert.AreEqual("c", DFoot.GetFootnoteLetter_abc(28 + 26));
        }
        #endregion
        #region Test: FootnoteLetter_iv
        [Test] public void FootnoteLetter_iv()
        {
            Assert.AreEqual("i", DFoot.GetFootnoteLetter_iv(0));
            Assert.AreEqual("iii", DFoot.GetFootnoteLetter_iv(2));
            Assert.AreEqual("xxvi", DFoot.GetFootnoteLetter_iv(25));

            Assert.AreEqual("xxx", DFoot.GetFootnoteLetter_iv(29));
            Assert.AreEqual("i", DFoot.GetFootnoteLetter_iv(30));
            Assert.AreEqual("iii", DFoot.GetFootnoteLetter_iv(32 + 30));
        }
        #endregion
        #region Test: FootnoteLetter_bsa
        [Test] public void FootnoteLetter_bsa()
        {
            BStringArray bsa = new BStringArray();
            bsa.Append("*");
            bsa.Append("+");
            bsa.Append("#");
            bsa.Append("@");

            Assert.AreEqual("*", DFoot.GetFootnoteLetter_bsa(0, bsa));
            Assert.AreEqual("#", DFoot.GetFootnoteLetter_bsa(2, bsa));
            Assert.AreEqual("@", DFoot.GetFootnoteLetter_bsa(3, bsa));

            Assert.AreEqual("*", DFoot.GetFootnoteLetter_bsa(4, bsa));
            Assert.AreEqual("#", DFoot.GetFootnoteLetter_bsa(6, bsa));
            Assert.AreEqual("@", DFoot.GetFootnoteLetter_bsa(7, bsa));

            Assert.AreEqual("*", DFoot.GetFootnoteLetter_bsa(8, bsa));
        }
        #endregion

        #region Test: OxesIO_Simple
        [Test] public void OxesIO_Simple()
        {
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();

            var oxes = new XmlDoc();
            var node = oxes.AddNode(null, "para");

            // Create a footnote
            string sFootnoteText = "One of the key verses in the Bible.";
            var footnoteIn = new DFootnote("3:16b", DFootnote.Types.kExplanatory);
            footnoteIn.SimpleText = sFootnoteText;
            var footIn = new DFoot(footnoteIn);

            // Output it to an xml node
            var nodeFoot = footIn.SaveToOxesBook(oxes, node);

            // Create a new footnote from that xml node
            var footOut = DFoot.Create(nodeFoot);

            // Should be identical
            Assert.IsNotNull(footOut);
            Assert.AreEqual("3:16b", footOut.Footnote.VerseReference);
            Assert.AreEqual(DFootnote.Types.kExplanatory, footOut.Footnote.NoteType);
            Assert.AreEqual(sFootnoteText, footOut.Footnote.SimpleText);
        }
        #endregion
        #region Test: OxesIO_MultiplePhrases
        [Test] public void OxesIO_MultiplePhrases()
        {
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();

            var oxes = new XmlDoc();
            var node = oxes.AddNode(null, "para");

            // Create a footnote
            var footnoteIn = new DFootnote("1:2", DFootnote.Types.kExplanatory);
            var text = new DText();
            text.Phrases.Append(new DPhrase("This is "));
            text.Phrases.Append(new DPhrase("very, very ") {FontModification = FontStyle.Italic});
            text.Phrases.Append(new DPhrase("cool ") {FontModification = FontStyle.Bold});
            text.Phrases.Append(new DPhrase("indeed!"));
            footnoteIn.Runs.Append(text);
            var footIn = new DFoot(footnoteIn);

            // Output it to an xml node
            var nodeFoot = footIn.SaveToOxesBook(oxes, node);

            // Create a new footnote from that xml node
            var footOut = DFoot.Create(nodeFoot);

            // Should be identical
            Assert.IsNotNull(footOut);
            Assert.AreEqual("1:2", footOut.Footnote.VerseReference);
            Assert.AreEqual(DFootnote.Types.kExplanatory, footOut.Footnote.NoteType);
            Assert.AreEqual("This is |ivery, very |r|bcool |rindeed!", footOut.Footnote.DebugString);
        }
        #endregion
    }
    #endregion

    #region CLASS: T_DVerse
    [TestFixture] public class T_DVerse
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: OxesIO
        [Test] public void OxesIO()
        {
            var oxes = new XmlDoc();
            var node = oxes.AddNode(null, "para");

            // Create a verse
            var verseIn = new DVerse("13b");

            // Output it to an xml node
            var nodeVerse = verseIn.SaveToOxesBook(oxes, node);

            // Create a new verse from that xml node
            var verseOut = DVerse.Create(nodeVerse);

            // Should have a verse whose text is the same as the original one
            Assert.IsNotNull(verseOut);
            Assert.AreEqual("13b", verseOut.Text);
        }
        #endregion
    }
    #endregion

    #region CLASS: T_DChapter
    [TestFixture] public class T_DChapter
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: OxesIO
        [Test] public void OxesIO()
        {
            var oxes = new XmlDoc();
            var node = oxes.AddNode(null, "para");

            // Create a chapter
            var chapterIn = DChapter.Create("5");

            // Output it to an xml node
            var nodeChapter = chapterIn.SaveToOxesBook(oxes, node);

            // Create a new chapter from that xml node
            var chapterOut = DChapter.Create(nodeChapter);

            // Should have a chapter whose nmber is the same as the original one
            Assert.IsNotNull(chapterOut);
            Assert.AreEqual("5", chapterOut.Text);
        }
        #endregion
    }
    #endregion
}
