/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DParagraph.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DParagraph class
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DParagraph
    {
        DSection m_section;

        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.InitializeFactoryStyleSheet();
            G.Project.DisplayName = "Project";
            G.Project.TargetTranslation = new DTranslation("Test Translation", "Latin", "Latin");
            DBook book = new DBook("MRK", "");
            G.Project.TargetTranslation.AddBook(book);
            m_section = new DSection(1);
            book.Sections.Append(m_section);
        }
        #endregion
        #region TearDown
        [TearDown]
        public void TearDown()
        {
            OurWordMain.Project = null;
        }
        #endregion

        // Splitting & Joinging paragraphs ---------------------------------------------------
        #region Method: DParagraph SplitParagraphSetup()
        private DParagraph SplitParagraphSetup()
        {
            // Create a paragraph
            DParagraph p = new DParagraph(G.Project.TargetTranslation);

            // Add various runs
            p.AddRun(DChapter.Create("3"));
            p.AddRun(DVerse.Create("16"));

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, "For God so loved the "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, "world "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, "that he gave his one and only son"));
            p.AddRun(text);

            p.AddRun(DVerse.Create("17"));

            text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, "that whosoever believes in him "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, "shall not perish, "));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, "but have everlasting life."));
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
        [Test]
        public void SplitAndJoinParas_BoundaryEnding()
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
        [Test]
        public void SplitAndJoinParas_AfterVerse()
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
        [Test]
        public void SplitAndJoinParas_BeforeVerse()
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
                DStyleSheet.c_StyleSectionTitle,
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

        // Verse numbers ---------------------------------------------------------------------
        #region Test: FirstActualVerseNumber_VerseAtBeginning
        [Test] public void FirstActualVerseNumber_VerseAtBeginning()
        {
            // Build the paragraph
            DParagraph p = new DParagraph(G.Project.TargetTranslation);
            p.AddRun(DVerse.Create("16"));

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, 
                "For God so loved the world that he gave his one and only son "));
            p.AddRun(text);

            p.AddRun(DVerse.Create("17"));

            text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal,
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
            DParagraph p = new DParagraph(G.Project.TargetTranslation);

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal,
                "For God so loved the world that he gave his one and only son "));
            p.AddRun(text);

            p.AddRun(DVerse.Create("17"));

            text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal,
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
            DParagraph p = new DParagraph(G.Project.TargetTranslation);

            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal,
                "For God so loved the world that he gave his one and only son "));
            p.AddRun(text);

            m_section.Paragraphs.Append(p);

            // Run the test
            Assert.AreEqual(0, p.FirstActualVerseNumber);
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Test: EditableTextLength
        [Test]
        public void EditableTextLength()
        {
            DParagraph p = SplitParagraphSetup();

            Assert.AreEqual(135, p.EditableTextLength);
        }
        #endregion
    }
}
