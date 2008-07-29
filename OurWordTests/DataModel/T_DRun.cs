/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DRun.cs
 * Author:  John Wimbish
 * Created: 11 Jul 2008
 * Purpose: Tests the DRun class
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DRun
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();

            OurWordMain.App = new OurWordMain();
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.InitializeFactoryStyleSheet();
        }
        #endregion

        // Test GetPhrase---------------------------------------------------------------------
        #region Method: void _Evaluate(Phrase p, sStyleAbbrev, sText)
        void _Evaluate(DSection.IO.Phrase p, string sStyleAbbrev, string sText)
        {
            Assert.AreEqual(p.StyleAbbrev, sStyleAbbrev);
            Assert.AreEqual(p.Text, sText);
        }
        #endregion
        #region Test: Test_GetPhrase
        [Test] public void Test_GetPhrase()
        {
            // Test: a footnote in the midst of text
            string s = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            int iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "Ije lais alekot. Ije Uis-neno In Anmone");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevFootLetter,
                "");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "in na' monin.");

            // Test: Looks like a footnote, but isn't.
            s = "Ije lais alekot. Ije ||Uis-neno In Anmone||fn in na' monin.";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "Ije lais alekot. Ije |Uis-neno In Anmone|fn in na' monin.");

            // Test: An italic phrase and a literal look-alike
            s = "This is |iitalic|r and this is ||inot.||r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "This is ");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevItalic,
                "italic");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                " and this is |inot.|r");

            // Test: Bold at the beginning and a literal look-alike.
            s = "|bThis is bold|r and this is ||bnot||r bold.";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevBold,
                "This is bold");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                " and this is |bnot|r bold.");

            // Test: Underline at the end and a literal look-alike
            s = "||uThis is not underlined||r and this is |uunderlined.|r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "|uThis is not underlined|r and this is ");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevUnderline,
                "underlined.");

            // Test: dashed
            s = "|dThis is dashed text.|r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevDashed,
                "This is dashed text.");

            // Test: Lots of literals
            s = "||||This i||s a str||ing with a b||unch of li||terals in it.||||";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "||This i|s a str|ing with a b|unch of li|terals in it.||");

            // Test: Footnote at the end
            s = "There is a footnote at the end.|fn";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevNormal,
                "There is a footnote at the end.");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevFootLetter,
                "");

            // Test: nothing but a footnote
            s = "|fn";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevFootLetter,
                "");

            // Test: nothing but two footnotes
            s = "|fn|fn";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevFootLetter,
                "");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevFootLetter,
                "");

        }
        #endregion

        // Test FieldToRuns-------------------------------------------------------------------
        #region Method: void CheckFoot(DRun[], iRun)
        void CheckFoot(DRun[] runs, int iRun)
        {
            DFootLetter foot = runs[iRun] as DFootLetter;
            Assert.IsNotNull(foot);

            Assert.AreEqual('a', foot.Letter);
        }
        #endregion
        #region Method: void CheckText(DRun[], iRun, iPhrase, sTextExpected)
        void CheckText(DRun[] runs, int iRun, int iPhrase, string sTextExpected)
        {
            DText txt = runs[iRun] as DText;
            Assert.IsNotNull(txt);

            DPhrase phrase = txt.Phrases[iPhrase] as DPhrase;
            Assert.IsNotNull(phrase);

            Assert.AreEqual(sTextExpected, phrase.Text);
        }
        #endregion
        #region Method: void CheckBT(DRun[], iRun, iPhrase, sTextExpected)
        void CheckBT(DRun[] runs, int iRun, int iPhrase, string sTextExpected)
        {
            DText txt = runs[iRun] as DText;
            Assert.IsNotNull(txt);

            DPhrase phrase = txt.PhrasesBT[iPhrase] as DPhrase;
            Assert.IsNotNull(phrase);

            Assert.AreEqual(sTextExpected, phrase.Text);
        }
        #endregion
        #region Test: FieldToRuns
        [Test] public void FieldToRuns()
        {
            char chNextFootLetter = 'a';
            SfField field = new SfField("vt", "", 1);
            DRun[] runs = null;

            // Test: data with footnote in the middle
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good story/matter. This is *God's Son's|fn life.";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, runs.Length);
            CheckText(runs, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(runs, 0, 0, "This is a good story/matter. This is *God's Son's");
            CheckFoot(runs, 1);
            CheckText(runs, 2, 0, "in na' monin.");
            CheckBT(runs, 2, 0, "life.");

            // Back translation does not have a footnote
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good story/matter. This is *God's Son's life.";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, runs.Length);
            CheckText(runs, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(runs, 0, 0, "This is a good story/matter. This is *God's Son's life.");
            CheckFoot(runs, 1);
            CheckText(runs, 2, 0, "in na' monin.");
            CheckBT(runs, 2, 0, "");

            // Back translation has too many footnotes
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good|fn story/matter. This is *God's Son's|fn life.";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, runs.Length);
            CheckText(runs, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(runs, 0, 0, "This is a good");
            CheckFoot(runs, 1);
            CheckText(runs, 2, 0, "in na' monin.");
            CheckBT(runs, 2, 0, "story/matter. This is *God's Son's life.");

            // Back translation has WAY too many footnotes
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This|fn is a good|fn story/matter.|fn This is *God's Son's|fn life.|fn";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, runs.Length);
            CheckText(runs, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(runs, 0, 0, "This");
            CheckFoot(runs, 1);
            CheckText(runs, 2, 0, "in na' monin.");
            CheckBT(runs, 2, 0, "is a good story/matter. This is *God's Son's life.");

            // Missing Back Translation
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, runs.Length);
            CheckText(runs, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(runs, 0, 0, "");
            CheckFoot(runs, 1);
            CheckText(runs, 2, 0, "in na' monin.");
            CheckBT(runs, 2, 0, "");

            // Test: Missing vernacular
            chNextFootLetter = 'a';
            field.Data = "";
            field.BT = "This is a good story/matter. This is *God's Son's|fn life.";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(0, runs.Length);

            // Test: data with italic phrase in the middle
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije |iUis-neno In Anmone|r in na' monin.";
            field.BT = "This is a good story/matter. This is |i*God's Son's|r life.";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(1, runs.Length);
            CheckText(runs, 0, 0, "Ije lais alekot. Ije ");
            CheckBT(runs, 0, 0, "This is a good story/matter. This is ");
            CheckText(runs, 0, 1, "Uis-neno In Anmone");
            CheckBT(runs, 0, 1, "*God's Son's");
            CheckText(runs, 0, 2, " in na' monin.");
            CheckBT(runs, 0, 2, " life.");

            // Test: data with heaps of phrases
            chNextFootLetter = 'a';
            field.Data = "|bIje|r lais |ualekot|r. Ije |iUis-neno In Anmone|r in na' |dmonin.|r";
            field.BT = "|bThis|r is a good |ustory/matter|r. This is |i*God's Son's|r |dlife.|r";
            runs = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(1, runs.Length);
            CheckText(runs, 0, 0, "Ije");
            CheckBT(runs, 0, 0, "This");
            CheckText(runs, 0, 1, " lais ");
            CheckBT(runs, 0, 1, " is a good ");
            CheckText(runs, 0, 2, "alekot");
            CheckBT(runs, 0, 2, "story/matter");
            CheckText(runs, 0, 3, ". Ije ");
            CheckBT(runs, 0, 3, ". This is ");
            CheckText(runs, 0, 4, "Uis-neno In Anmone");
            CheckBT(runs, 0, 4, "*God's Son's");
            CheckText(runs, 0, 5, " in na' ");
            CheckBT(runs, 0, 5, " ");
            CheckText(runs, 0, 6, "monin.");
            CheckBT(runs, 0, 6, "life.");

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
            string sIn = "  This  has     too    many spaces.   ";
            DPhrase phrase = new DPhrase("p", sIn);
            phrase.Text = DPhrase.EliminateSpuriousSpaces(phrase.Text);
            Assert.AreEqual(phrase.Text, " This has too many spaces. ");

            // For a paragraph, we get rid of leading and trailing as well.
            DText text = DText.CreateSimple(sIn);
            text.Phrases.InsertAt(0, new DPhrase("i", " Leading    Italic Phrase. "));
            text.Phrases.Append(new DPhrase("i", " Trailing Italic    Phrase.  "));

            DPhrase phrase1 = text.Phrases[0] as DPhrase;
            DPhrase phrase2 = text.Phrases[1] as DPhrase;
            DPhrase phrase3 = text.Phrases[2] as DPhrase;

            phrase1.Text = DPhrase.EliminateSpuriousSpaces(phrase1.Text);
            phrase2.Text = DPhrase.EliminateSpuriousSpaces(phrase2.Text);
            phrase3.Text = DPhrase.EliminateSpuriousSpaces(phrase3.Text);

            text.EliminateSpuriousSpaces();

            string sComposite = phrase1.Text + phrase2.Text + phrase3.Text;
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
            ArrayList aWords = new ArrayList();
            ArrayList aPositions = new ArrayList();

            // Normal example of text that we would expect
            DText text = DText.CreateSimple("A hard worker has plenty ");
            text.Phrases.Append(new DPhrase("i", "of food, "));
            text.Phrases.Append(new DPhrase("b", "but "));
            text.Phrases.Append(new DPhrase("p", "a person who chases fantasies "));
            text.Phrases.Append(new DPhrase("i", "ends up "));
            text.Phrases.Append(new DPhrase("p", "in poverty."));

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
}
