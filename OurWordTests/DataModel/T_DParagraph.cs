/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DParagraph.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DParagraph class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DParagraph
    {
        DSection m_section;

        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();

            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DB.Project.TargetTranslation = new DTranslation("Test Translation", "Latin", "Latin");
            DBook book = new DBook("MRK");
            DB.Project.TargetTranslation.AddBook(book);
            m_section = new DSection();
            book.Sections.Append(m_section);
        }
        #endregion
        #region TearDown
        [TearDown]
        public void TearDown()
        {
            DB.Project = null;
        }
        #endregion

        // Splitting & Joining paragraphs ----------------------------------------------------
        #region Method: DParagraph SplitParagraphSetup()
        private DParagraph SplitParagraphSetup()
        {
            // Create a paragraph
            DParagraph p = new DParagraph();

            // Add various runs
            p.AddRun(DChapter.Create("3"));
            p.AddRun(DVerse.Create("16"));

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph, "For God so loved the "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, "world "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph, "that he gave his one and only son"));
            p.AddRun(text);

            p.AddRun(DVerse.Create("17"));

            text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph, "that whosoever believes in him "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, "shall not perish, "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph, "but have everlasting life."));
            p.AddRun(text);

            m_section.Paragraphs.Append(p);

            return p;
        }
        #endregion
        #region Test: SplitAndJoinParas_BetweenWords
        [Test]
        public void SplitAndJoinParas_BetweenWords()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Split it at "For |God"
            p.Split(p.Runs[2] as DBasicText, 4);
            Assert.AreEqual(2, m_section.Paragraphs.Count);
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("{c 3}{v 16}For ",
                p.DebugString,
                "Split Left");
            Assert.AreEqual("God so loved the |iworld |rthat he gave his one and only son{v 17}that whosoever " +
                "believes in him |ishall not perish, |rbut have everlasting life.",
                pRight.DebugString,
                "Split Right");
            Assert.AreEqual("316", p.ProseBTAsString);
            Assert.AreEqual("17", pRight.ProseBTAsString);

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count);
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: SplitAndJoinParas_WithinWord
        [Test] public void SplitAndJoinParas_WithinWord()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Split at "wo|rld"
            p.Split(p.Runs[2] as DBasicText, 23);
            Assert.AreEqual(2, m_section.Paragraphs.Count);
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iwo|r",
                p.DebugString,
                "Split Left");
            Assert.AreEqual("|irld |rthat he gave his one and only son{v 17}that whosoever " +
                "believes in him |ishall not perish, |rbut have everlasting life.",
                pRight.DebugString,
                "Split Right");
            Assert.AreEqual("316", p.ProseBTAsString);
            Assert.AreEqual("17", pRight.ProseBTAsString);

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count);
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: SplitAndJoinParas_BoundaryBeginning
        [Test] public void SplitAndJoinParas_BoundaryBeginning()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Split at "|For God"
            p.Split(p.Runs[2] as DBasicText, 0);
            Assert.AreEqual(2, m_section.Paragraphs.Count);
            DParagraph pLeft = m_section.Paragraphs[0] as DParagraph;
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("{c 3}{v 16}",
                pLeft.DebugString,
                "Split Left");
            Assert.AreEqual("For God so loved the |iworld |rthat he gave his one and only son{v 17}that whosoever " +
                "believes in him |ishall not perish, |rbut have everlasting life.",
                pRight.DebugString,
                "Split Right");
            // Note: we should have a DPhrase in the left paragraph here
            Assert.AreEqual(3, pLeft.Runs.Count);
            Assert.IsNotNull(pLeft.Runs[2] as DBasicText);
            Assert.AreEqual("316", p.ProseBTAsString);
            Assert.AreEqual("17", pRight.ProseBTAsString);

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count);
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: SplitAndJoinParas_BoundaryEnding
        [Test] public void SplitAndJoinParas_BoundaryEnding()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Split at "life.|"
            p.Split(p.Runs[4] as DBasicText, 75);
            Assert.AreEqual(2, m_section.Paragraphs.Count);
            DParagraph pLeft = m_section.Paragraphs[0] as DParagraph;
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one and only son{v 17}that " +
                "whosoever believes in him |ishall not perish, |rbut have everlasting life.",
                pLeft.DebugString,
                "Split Left");
            Assert.AreEqual("",
                pRight.DebugString,
                "Split Right");
            Assert.AreEqual(1, pRight.Runs.Count);
            Assert.IsNotNull(pRight.Runs[0] as DText);
            Assert.AreEqual("31617", pLeft.ProseBTAsString);
            Assert.AreEqual("", pRight.ProseBTAsString);

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count);
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: SplitAndJoinParas_AfterVerse
        [Test] public void SplitAndJoinParas_AfterVerse()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Split after verse 17 (the 17 should go to the new paragraph)
            p.Split(p.Runs[4] as DBasicText, 0);
            Assert.AreEqual(2, m_section.Paragraphs.Count, "Split Count");
            DParagraph pLeft = m_section.Paragraphs[0] as DParagraph;
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one and only son",
                pLeft.DebugString,
                "Split Left");
            Assert.AreEqual("{v 17}that whosoever believes in him |ishall not perish, |rbut have everlasting life.",
                pRight.DebugString,
                "Split Right");

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count, "Join Count");
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: SplitAndJoinParas_BeforeVerse
        [Test] public void SplitAndJoinParas_BeforeVerse()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Split before verse 17 (the 17 should go with the newly-formed paragraph)
            p.Split(p.Runs[2] as DBasicText, 60);
            Assert.AreEqual(2, m_section.Paragraphs.Count, "Split Count");
            DParagraph pLeft = m_section.Paragraphs[0] as DParagraph;
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one and only son",
                pLeft.DebugString,
                "Split Left");
            Assert.AreEqual("{v 17}that whosoever believes in him |ishall not perish, |rbut have everlasting life.",
                pRight.DebugString,
                "Split Right");

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count, "Join Count");
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: SplitAndJoinParas_ParaBeginning
        [Test] public void SplitAndJoinParas_ParaBeginning()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Remove the non-DBT's from the paragraph beginning
            while (p.Runs.Count > 0 && (p.Runs[0] as DBasicText) == null)
                p.Runs.RemoveAt(0);

            // Split at "|For God"
            p.Split(p.Runs[0] as DBasicText, 0);
            Assert.AreEqual(2, m_section.Paragraphs.Count);
            DParagraph pLeft = m_section.Paragraphs[0] as DParagraph;
            DParagraph pRight = m_section.Paragraphs[1] as DParagraph;
            Assert.AreEqual("",
                pLeft.DebugString,
                "Split Left");
            Assert.AreEqual("For God so loved the |iworld |rthat he gave his one and only son{v 17}that whosoever " +
                "believes in him |ishall not perish, |rbut have everlasting life.",
                pRight.DebugString,
                "Split Right");
            // Note: we should have a DPhrase in the left paragraph here
            Assert.AreEqual(1, pLeft.Runs.Count);
            Assert.IsNotNull(pLeft.Runs[0] as DBasicText);
            Assert.AreEqual("", p.ProseBTAsString);
            Assert.AreEqual("17", pRight.ProseBTAsString);

            // Join it back up
            p.JoinToNext();
            Assert.AreEqual(1, m_section.Paragraphs.Count);
            Assert.AreEqual("For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Join");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: PreventSplitForSomeParagraphStyles
        [Test] public void PreventSplitForSomeParagraphStyles()
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // The styles we don't allow
            string[] vs = new string[] { 
                DStyleSheet.c_StyleAbbrevPictureCaption,
                DStyleSheet.c_StyleNote,
                DStyleSheet.c_StyleFootnote
            };

            // Try out each one
            foreach (string sAbbrev in vs)
            {
                p.StyleAbbrev = sAbbrev;
                p.Split(p.Runs[2] as DBasicText, 23);
                Assert.AreEqual(1, m_section.Paragraphs.Count);
            }
        }
        #endregion

        // Inserting and Removing Footnotes --------------------------------------------------
        #region Method: void TestInsertFootnote(iRun, iPosWithinRun, s, sBT)
        DFoot TestInsertFootnote(int iRun, int iPosWithinRun, 
            string sExpectedAfterInsert, string sExpectedAfterInsertBT)
        {
            // Create a test paragraph
            DParagraph p = SplitParagraphSetup();

            // Locate the run (DBasicText) in question
            DBasicText dbt = p.Runs[iRun] as DBasicText;

            // Insert the footnote
            DFoot foot = p.InsertFootnote(dbt, iPosWithinRun);

            // Debugging optional
            //Console.WriteLine("Expected    = <" + sExpectedAfterInsert + ">");
            //Console.WriteLine("Actual      = <" + p.DebugString + ">");
            //Console.WriteLine("Expected BT = <" + sExpectedAfterInsertBT + ">");
            //Console.WriteLine("Actual   BT = <" + p.ProseBTAsString + ">");

            // Test
            Assert.AreEqual(sExpectedAfterInsert, p.DebugString, "Insert Footnote");
            Assert.AreEqual(sExpectedAfterInsertBT, p.ProseBTAsString, "Insert Footnote - BT");

            return foot;
        }
        #endregion
        #region Method: void TestRemoveFootnote(DFoot)
        void TestRemoveFootnote(DFoot foot)
        {
            // Remove the footnote
            DParagraph p = foot.Owner as DParagraph;
            p.RemoveFootnote(foot);
            Assert.AreEqual("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one " +
                "and only son{v 17}that whosoever believes in him |ishall not perish, |rbut " +
                "have everlasting life.",
                p.DebugString,
                "Remove Footnote");

            // Clear what we've done
            m_section.Paragraphs.Clear();
        }
        #endregion
        #region Test: InsertFootnote_BetweenWords
        [Test] public void InsertFootnote_BetweenWords()
        {
            // Insert at "For |God"
            DFoot foot = TestInsertFootnote(2, 4,
                "{c 3}{v 16}For {fn a}God so loved the |iworld |rthat he gave his " +
                    "one and only son{v 17}that whosoever believes in him |ishall " +
                    "not perish, |rbut have everlasting life.",
                "316a17");
            TestRemoveFootnote(foot);
        }
        #endregion
        #region Test: InsertFootnote_WithinWord
        [Test] public void InsertFootnote_WithinWord()
        {
            // Insert at "wo|rld"
            DFoot foot = TestInsertFootnote(2, 23,
                "{c 3}{v 16}For God so loved the |iwo|r{fn a}|irld |rthat he gave his " +
                    "one and only son{v 17}that whosoever believes in him |ishall " +
                    "not perish, |rbut have everlasting life.",
                "316a17");
            TestRemoveFootnote(foot);
        }
        #endregion
        #region Test: InsertFootnote_BoundaryBeginning
        [Test] public void InsertFootnote_BoundaryBeginning()
        {
            // Insert at "|For God": we don't permit the footnote to be inserted
            // at the beginning of a text, as there would be no text before it.
            // So we expect nothing to happen.
            DFoot foot = TestInsertFootnote(2, 0,
                "{c 3}{v 16}For God so loved the |iworld |rthat he gave his " +
                    "one and only son{v 17}that whosoever believes in him |ishall " +
                    "not perish, |rbut have everlasting life.",
                "31617");
            Assert.IsNull(foot);
        }
        #endregion
        #region Test: InsertFootnote_BoundaryEnding
        [Test] public void InsertFootnote_BoundaryEnding()
        {
            // Insert at "life.|"
            DFoot foot = TestInsertFootnote(4, 75,
                "{c 3}{v 16}For God so loved the |iworld |rthat he gave his " +
                    "one and only son{v 17}that whosoever believes in him |ishall " +
                    "not perish, |rbut have everlasting life.{fn a}",
                "31617a");
            TestRemoveFootnote(foot);
        }
        #endregion
        #region Test: InsertFootnote_AfterVerseNumber
        [Test] public void InsertFootnote_AfterVerseNumber()
        {
            // Insert after verse 17: nothing should happen because we're at the beginning
            // of text.
            DFoot foot = TestInsertFootnote(4, 0,
                "{c 3}{v 16}For God so loved the |iworld |rthat he gave his " +
                    "one and only son{v 17}that whosoever believes in him |ishall " +
                    "not perish, |rbut have everlasting life.",
                "31617");
            Assert.IsNull(foot);
        }
        #endregion
        #region Test: InsertFootnote_BeforeVerseNumber
        [Test] public void InsertFootnote_BeforeVerseNumber()
        {
            // Insert before verse 17: 
            DFoot foot = TestInsertFootnote(2, 60,
                "{c 3}{v 16}For God so loved the |iworld |rthat he gave his " +
                    "one and only son{fn a}{v 17}that whosoever believes in him |ishall " +
                    "not perish, |rbut have everlasting life.",
                "316a17");
            TestRemoveFootnote(foot);
        }
        #endregion

        // Verse numbers ---------------------------------------------------------------------
        #region Test: FirstActualVerseNumber_VerseAtBeginning
        [Test] public void FirstActualVerseNumber_VerseAtBeginning()
        {
            // Build the paragraph
            DParagraph p = new DParagraph();
            p.AddRun(DVerse.Create("16"));

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph, 
                "For God so loved the world that he gave his one and only son "));
            p.AddRun(text);

            p.AddRun(DVerse.Create("17"));

            text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph,
                "that whosoever believes in him shall not perish."));
            p.AddRun(text);

            m_section.Paragraphs.Append(p);

            // Run the test
            Assert.AreEqual(16, p.FirstActualVerseNumber);
        }
        #endregion
        #region Test: FirstActualVerseNumber_VerseAtMiddle
        [Test] public void FirstActualVerseNumber_VerseAtMiddle()
        {
            // Build the paragraph
            DParagraph p = new DParagraph();

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph,
                "For God so loved the world that he gave his one and only son "));
            p.AddRun(text);

            p.AddRun(DVerse.Create("17"));

            text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph,
                "that whosoever believes in him shall not perish."));
            p.AddRun(text);

            m_section.Paragraphs.Append(p);

            // Run the test
            Assert.AreEqual(17, p.FirstActualVerseNumber);
        }
        #endregion
        #region Test: FirstActualVerseNumber_NoVerse
        [Test] public void FirstActualVerseNumber_NoVerse()
        {
            // Build the paragraph
            DParagraph p = new DParagraph();

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_sfmParagraph,
                "For God so loved the world that he gave his one and only son "));
            p.AddRun(text);

            m_section.Paragraphs.Append(p);

            // Run the test
            Assert.AreEqual(0, p.FirstActualVerseNumber);
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Test: EditableTextLength
        [Test] public void EditableTextLength()
        {
            DParagraph p = SplitParagraphSetup();

            Assert.AreEqual(135, p.EditableTextLength);
        }
        #endregion
        #region Test: AsString
        [Test] public void AsString()
        {
            // Set up a paragraph
            DParagraph p = new DParagraph();
            p.AddRun(DChapter.Create("3"));
            p.AddRun(DVerse.Create("1"));
            p.AddRun(DText.CreateSimple("In the beginning was the word."));
            p.AddRun(DVerse.Create("2"));
            p.AddRun(DText.CreateSimple("And the Word was with God, and the Word was God."));
            p.AddRun(DVerse.Create("3"));
            p.AddRun(DText.CreateSimple("He was with God in the beginning."));
            p.Cleanup();  // Determines where leading spaces are needed

            // Did we get what we expect?
            string sExpected = "31In the beginning was the word.2And the Word " +
                "was with God, and the Word was God.3He was with God in the " +
                "beginning.";
            Assert.AreEqual(sExpected, p.AsString);
            //Console.WriteLine("AsString = \"" + p.AsString + "\"");
        }
        #endregion
        #region Test: CombineDTexts
        [Test] public void CombineDTexts()
        {
            // Create a paragraph
            DParagraph p = new DParagraph();

            // Place an initial phrase into the paragraph
            p.SimpleText = "This is a phrase.";
            p.SimpleTextBT = "This is the BT of a phrase.";

            // Add a second phrase
            DText text = DText.CreateSimple();
            text.Phrases[0].Text = "Appended Phrase.";
            text.PhrasesBT[0].Text = "Appended BT of a phrase.";
            p.AddRun(text);

            // This call should now combine the DTexts, and it should insert a space
            // between them.
            p.Cleanup();

            // Test the result
            Assert.AreEqual("This is a phrase. Appended Phrase.", p.SimpleText);
            Assert.AreEqual("This is the BT of a phrase. Appended BT of a phrase.", p.SimpleTextBT);
        }
        #endregion
        #region Test: BestGuessAtInsertingTextPositions
        [Test] public void BestGuessAtInsertingTextPositions()
        {
            // An empty paragraph should be given a DText
            DParagraph p = new DParagraph();
            p.BestGuessAtInsertingTextPositions();
            Assert.AreEqual(1, p.Runs.Count);
            Assert.IsNotNull(p.Runs[0] as DText);

            // There should be DText after verses
            p = new DParagraph();
            p.Runs.Append(DVerse.Create("3"));
            p.Runs.Append(DVerse.Create("4"));
            p.Runs.Append( new DFoot( new DFootnote(2, 4, DFootnote.Types.kExplanatory)));
            p.BestGuessAtInsertingTextPositions();
            Assert.AreEqual(5, p.Runs.Count);
            Assert.IsNotNull(p.Runs[0] as DVerse);
            Assert.IsNotNull(p.Runs[1] as DText);
            Assert.IsNotNull(p.Runs[2] as DVerse);
            Assert.IsNotNull(p.Runs[3] as DText);
            Assert.IsNotNull(p.Runs[4] as DFoot);

            // There should be a DText before a paragraph-initial footnote
            p = new DParagraph();
            p.Runs.Append(new DFoot( new DFootnote(2, 4, DFootnote.Types.kExplanatory)));
            p.BestGuessAtInsertingTextPositions();
            Assert.AreEqual(2, p.Runs.Count);
            Assert.IsNotNull(p.Runs[0] as DText);
            Assert.IsNotNull(p.Runs[1] as DFoot);
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Test: IoAbbreviated
        [Test] public void IoAbbreviated()
        {
            // Create a paragraph with a single DText
            DParagraph p = new DParagraph();
            m_section.Paragraphs.Append(p);
            DText text = new DText();
            p.Runs.Append(text);
            text.Phrases.Append(new DPhrase("p", "This is some text."));
            text.PhrasesBT.Append(new DPhrase("p", ""));

            // Create the Xml Element from it
            XElement x = p.ToXml(true);

            // Create a new recipient paragraph and interpret the xml
            DParagraph pNew = new DParagraph();
            pNew.FromXml(x);

            // Should be the same
            Assert.IsTrue(p.ContentEquals(pNew), "Paragraphs should be equal.");
        }
        #endregion

        #region Test: oxesIO
        [Test] public void oxesIO()
        {
            // Set up a hierarchy all the way down to our paragraph
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;
            var Book = new DBook("MRK");
            Translation.Books.Append(Book);
            var Section = new DSection();
            Book.Sections.Append(Section);

            // Create the xml doc
            var oxes = new XmlDoc();
            var nodeBook = oxes.AddNode(null, "book");

            // Attribute data
            string sText = "Ini adalah sesuatu paragraph";
            string sBT = "This is a paragraph.";
            string sStyle = "q2";

            // Create a paragraph. The ID will be automatically set to 0.
            var paragraphIn = new DParagraph();
            Section.Paragraphs.Append(paragraphIn);
            paragraphIn.SimpleText = sText;
            paragraphIn.SimpleTextBT = sBT;
            paragraphIn.StyleAbbrev = sStyle;

            // Save it to an xml node
            var nodeParagraph = paragraphIn.SaveToOxesBook(oxes, nodeBook);

            // Create a new paragraph from that node
            var paragraphOut = DParagraph.CreateParagraph(nodeParagraph);

            // Should be identical
            Assert.AreEqual(sText, paragraphOut.SimpleText);
            Assert.AreEqual(sBT, paragraphOut.SimpleTextBT);
            Assert.AreEqual(sStyle, paragraphOut.StyleAbbrev);
            Assert.IsTrue(paragraphOut.ContentEquals(paragraphIn), "Paras are the same");
        }
        #endregion
    }
}
