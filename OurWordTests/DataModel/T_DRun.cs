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
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;
using JWdb.DataModel;

using OurWord;
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
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
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
                DStyleSheet.c_sfmParagraph,
                "Ije lais alekot. Ije Uis-neno In Anmone");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevFootLetter,
                "");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
                "in na' monin.");

            // Test: Looks like a footnote, but isn't.
            s = "Ije lais alekot. Ije ||Uis-neno In Anmone||fn in na' monin.";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
                "Ije lais alekot. Ije |Uis-neno In Anmone|fn in na' monin.");

            // Test: An italic phrase and a literal look-alike
            s = "This is |iitalic|r and this is ||inot.||r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
                "This is ");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevItalic,
                "italic");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
                " and this is |inot.|r");

            // Test: Bold at the beginning and a literal look-alike.
            s = "|bThis is bold|r and this is ||bnot||r bold.";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_StyleAbbrevBold,
                "This is bold");
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
                " and this is |bnot|r bold.");

            // Test: Underline at the end and a literal look-alike
            s = "||uThis is not underlined||r and this is |uunderlined.|r";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
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
                DStyleSheet.c_sfmParagraph,
                "||This i|s a str|ing with a b|unch of li|terals in it.||");

            // Test: Footnote at the end
            s = "There is a footnote at the end.|fn";
            iPos = 0;
            _Evaluate(DSection.IO.Phrase.GetPhrase(s, ref iPos),
                DStyleSheet.c_sfmParagraph,
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
        #region Method: void CheckFoot(List<DRun>, iRun)
        void CheckFoot(List<DRun> vRuns, int iRun)
        {
            DFoot foot = vRuns[iRun] as DFoot;
            Assert.IsNotNull(foot);

            Assert.AreEqual('a', foot.Letter);
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
            char chNextFootLetter = 'a';
            SfField field = new SfField("vt", "", 1);
            List<DRun> vRuns = null;

            // Test: data with footnote in the middle
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good story/matter. This is *God's Son's|fn life.";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This is a good story/matter. This is *God's Son's");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "life.");

            // Back translation does not have a footnote
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good story/matter. This is *God's Son's life.";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This is a good story/matter. This is *God's Son's life.");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "");

            // Back translation has too many footnotes
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This is a good|fn story/matter. This is *God's Son's|fn life.";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This is a good");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "story/matter. This is *God's Son's life.");

            // Back translation has WAY too many footnotes
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "This|fn is a good|fn story/matter.|fn This is *God's Son's|fn life.|fn";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "This");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "is a good story/matter. This is *God's Son's life.");

            // Missing Back Translation
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin.";
            field.BT = "";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(3, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije Uis-neno In Anmone");
            CheckBT(vRuns, 0, 0, "");
            CheckFoot(vRuns, 1);
            CheckText(vRuns, 2, 0, "in na' monin.");
            CheckBT(vRuns, 2, 0, "");

            // Test: Missing vernacular
            chNextFootLetter = 'a';
            field.Data = "";
            field.BT = "This is a good story/matter. This is *God's Son's|fn life.";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(0, vRuns.Count);

            // Test: data with italic phrase in the middle
            chNextFootLetter = 'a';
            field.Data = "Ije lais alekot. Ije |iUis-neno In Anmone|r in na' monin.";
            field.BT = "This is a good story/matter. This is |i*God's Son's|r life.";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
            Assert.AreEqual(1, vRuns.Count);
            CheckText(vRuns, 0, 0, "Ije lais alekot. Ije ");
            CheckBT(vRuns, 0, 0, "This is a good story/matter. This is ");
            CheckText(vRuns, 0, 1, "Uis-neno In Anmone");
            CheckBT(vRuns, 0, 1, "*God's Son's");
            CheckText(vRuns, 0, 2, " in na' monin.");
            CheckBT(vRuns, 0, 2, " life.");

            // Test: data with heaps of phrases
            chNextFootLetter = 'a';
            field.Data = "|bIje|r lais |ualekot|r. Ije |iUis-neno In Anmone|r in na' |dmonin.|r";
            field.BT = "|bThis|r is a good |ustory/matter|r. This is |i*God's Son's|r |dlife.|r";
            vRuns = DSection.IO.FieldToRuns(field, ref chNextFootLetter);
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
            CheckText(vRuns, 0, 5, " in na' ");
            CheckBT(vRuns, 0, 5, " ");
            CheckText(vRuns, 0, 6, "monin.");
            CheckBT(vRuns, 0, 6, "life.");

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

    [TestFixture] public class T_DText
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();

            OurWordMain.App = new OurWordMain();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
        }
        #endregion

        #region Test: ToFromString
        [Test] public void ToFromString()
        {
            // Create a DText that has several phrases
            DText text = new DText();
            text.Phrases.Append(new DPhrase("p", "These are the "));
            text.Phrases.Append(new DPhrase("i", "times "));
            text.Phrases.Append(new DPhrase("p", "that "));
            text.Phrases.Append(new DPhrase("u", "try "));
            text.Phrases.Append(new DPhrase("b", "men's souls."));

            // Is the output what we expect?
            string sSave = text.Phrases.ToSaveString;
            Assert.AreEqual("These are the |itimes |rthat |utry |r|bmen's souls.|r", 
                sSave);

            // Create a recipient DText
            DText text2 = new DText();
            text2.Phrases.FromSaveString(sSave);

            // Are the two texts equal?
            Assert.IsTrue(text.ContentEquals(text2), "DTexts are not the same");
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: DText _CreateText(s)
        DText _CreateText(string s)
        {
            DText t = new DText();
            t.Phrases.FromSaveString(s);
            return t;
        }
        #endregion
        #region Test: Merge_OursChanged
        [Test] public void Merge_OursChanged()
        {
            // Create the three versions
            string sParent = "These are the |itimes |rthat try men's souls.";
            DText Parent = _CreateText(sParent);

            DText Theirs = _CreateText(sParent);

            string sOurs = "These are not the |itimes |rthat try men's souls.";
            DText Ours = _CreateText(sOurs);

            // Do the Merge
            Ours.Merge(Parent, Theirs);

            // Ours should not have changed
            Assert.AreEqual(sOurs, Ours.Phrases.ToSaveString);
        }
        #endregion
        #region Test: Merge_TheirsChanged
        [Test] public void Merge_TheirsChanged()
        {
            // Create the three versions
            string sParent = "These are the |itimes |rthat try men's souls.";
            DText Parent = _CreateText(sParent);

            string sTheirs = "These are definitely the |itimes |rthat try men's souls.";
            DText Theirs = _CreateText(sTheirs);

            DText Ours = _CreateText(sParent);

            // Do the Merge
            Ours.Merge(Parent, Theirs);

            // Ours should have changed to Theirs
            Assert.AreEqual(sTheirs, Ours.Phrases.ToSaveString);
        }
        #endregion
        #region Test: Merge_BothChanged
        [Test] public void Merge_BothChanged()
        {
            // Create the three versions
            string sParent = "These are the |itimes |rthat try men's souls.";
            DText Parent = _CreateText(sParent);

            string sTheirs = "These are definitely the |itimes |rthat try men's souls.";
            DText Theirs = _CreateText(sTheirs);

            string sOurs = "These are not the |itimes |rthat try men's souls.";
            DText Ours = _CreateText(sOurs);

            // Because we're creating a Note, we need for Ours to be owned so that a
            // Reference can be calculated.
            DSection section = new DSection(1);
            section.ReferenceSpan = new DReferenceSpan();
            section.ReferenceSpan.Start = new DReference(3, 8);
            section.ReferenceSpan.End = new DReference(3, 15);
            DParagraph p = new DParagraph();
            section.Paragraphs.Append(p);
            p.Runs.Append(Ours);

            // Do the Merge
            Ours.Merge(Parent, Theirs);

            // Ours should not be changed
            Assert.AreEqual(sOurs, Ours.Phrases.ToSaveString);

            // But we should have a Translator Note giving their version
            Assert.AreEqual(1, Ours.TranslatorNotes.Count);
            TranslatorNote note = Ours.TranslatorNotes[0];
            Assert.AreEqual("003:008", note.Reference);
            Assert.AreEqual("not", note.Context);
        }
        #endregion
        #region Test: GetNoteContext
        [Test] public void GetNoteContext()
        {
            // Middle
            string sOurs = "For God so loved the world";
            string sTheirs = "For God really cared for the world";
            Assert.AreEqual("so loved", DText.GetNoteContext(sOurs, sTheirs), "Middle");

            // Beginning
            sOurs = "For God so loved the world";
            sTheirs = "Truly God so loved the world";
            Assert.AreEqual("For", DText.GetNoteContext(sOurs, sTheirs), "Beginning");

            // End
            sOurs = "For God so loved the world";
            sTheirs = "For God so loved the planet";
            Assert.AreEqual("world", DText.GetNoteContext(sOurs, sTheirs), "End");

            // Append at end
            sOurs = "For God so loved the world";
            sTheirs = "For God so loved the world he gave his son.";
            Assert.AreEqual("...the world", DText.GetNoteContext(sOurs, sTheirs), "Append at end");

            // Append at beginning
            sOurs = "For God so loved the world";
            sTheirs = "Since For God so loved the world";
            Assert.AreEqual("For God so...", DText.GetNoteContext(sOurs, sTheirs), "Append at beginning");

            // Append at end; yet looks the same
            sOurs = "For God so loved the world";
            sTheirs = "For God so loved the world he gave the world";
            Assert.AreEqual("...the world", DText.GetNoteContext(sOurs, sTheirs), "Pathelogic");
        }
        #endregion
    }
}
