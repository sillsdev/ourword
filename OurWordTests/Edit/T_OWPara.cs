/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_OWPara.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the OWPara class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;
using OurWordData.DataModel;

using OurWord;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

#endregion

namespace OurWordTests.Edit
{
    [TestFixture] public class T_OWPara
    {
        // Attrs from Setup ------------------------------------------------------------------
        #region Attr{g}: OWWindow Wnd
        OWWindow Wnd
        {
            get
            {
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        Form m_Form;
        DSection m_section;
        #region VAttr{g}: JWritingSystem WSVernacular
        JWritingSystem WSVernacular
        {
            get
            {
                return DB.Project.TargetTranslation.WritingSystemVernacular;
            }
        }
        #endregion
        #region Attr{b}: OWPara OP
        OWPara OP
        {
            get
            {
                Assert.IsNotNull(m_op, "Attr: OP is not null");
                return m_op;
            }
        }
        OWPara m_op;
        #endregion
        #region Attr{g}: DParagraph P
        DParagraph P
        {
            get
            {
                Assert.IsNotNull(m_p, "Attr: Paragraph initialized");
                return m_p;
            }
        }
        DParagraph m_p;
        #endregion
        #region Attr{g}: DBasicText DBT1
        DBasicText DBT1
        {
            get
            {
                Assert.IsNotNull(m_DBT1, "Attr: m_DBT1 is not null");
                return m_DBT1;
            }
        }
        DBasicText m_DBT1;
        #endregion
        #region Attr{g}: DBasicText DBT2
        DBasicText DBT2
        {
            get
            {
                Assert.IsNotNull(m_DBT2, "Attr: m_DBT2 is not null");
                return m_DBT2;
            }
        }
        DBasicText m_DBT2;
        #endregion

        // Data ------------------------------------------------------------------------------
        const string c_AsString = "316For God so loved the world that he gave his one and " +
            "only son17that whosoever believes in him shall not perish, but have everlasting " +
            "life.";
        const string c_DebugString = "{c 3}{v 16}For God so loved the |iworld |rthat he gave his one and " +
            "only son{v 17}that whosoever believes in him |ishall not perish, |rbut have everlasting " +
            "life.";
        #region Method: DParagraph CreateParagraph_John_3_16()
        private DParagraph CreateParagraph_John_3_16()
        {
            // Create a paragraph
            var p = new DParagraph(StyleSheet.Paragraph);

            // Add various runs
            p.AddRun(DChapter.Create("3"));
            p.AddRun(DVerse.Create("16"));

            m_DBT1 = new DText();
            m_DBT1.Phrases.Append(new DPhrase("For God so loved the "));
            m_DBT1.Phrases.Append(new DPhrase("world ") { FontToggles = FontStyle.Italic });
            m_DBT1.Phrases.Append(new DPhrase("that he gave his one and only son"));
            p.AddRun(m_DBT1);

            p.AddRun(DVerse.Create("17"));

            m_DBT2 = new DText();
            m_DBT2.Phrases.Append(new DPhrase("that whosoever believes in him "));
            m_DBT2.Phrases.Append(new DPhrase("shall not perish, ") { FontToggles = FontStyle.Italic });
            m_DBT2.Phrases.Append(new DPhrase("but have everlasting life."));
            p.AddRun(m_DBT2);

            m_section.Paragraphs.Append(p);

            return p;
        }
        #endregion

        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();

            // Application and Project initialization
            OurWordMain.App = new OurWordMain();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DB.Project.TargetTranslation = new DTranslation("Test Translation", "Latin", "Latin");
            DBook book = new DBook("MRK");
            DB.Project.TargetTranslation.AddBook(book);
            G.URStack.Clear();

            m_section = new DSection();
            book.Sections.Append(m_section);

            m_Window = new OWWindow("TestWindow", 1);

            m_Form = new Form();
            m_Form.Name = "TestForm";
            m_Form.Controls.Add(m_Window);

            WSVernacular.UseAutomatedHyphenation = false;

            // Set up John 3:16
            m_p = CreateParagraph_John_3_16();
            m_op = new OWPara( 
                WSVernacular, 
                m_p.Style, 
                m_p, 
                Color.Wheat, 
                OWPara.Flags.IsEditable);
            Wnd.Contents.Append(m_op);
            Wnd.LoadData();

            // Tests will mess up if this gets changed.
            Assert.AreEqual(c_AsString, m_p.AsString, "Attr: Paragraph AsString correct");
            Assert.AreEqual(c_DebugString, m_p.DebugString, "Attr: Paragraph DebugString correct");
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            DB.Project = null;
            m_Form.Dispose();
            m_Form = null;
        }
        #endregion

        // Helpers ---------------------------------------------------------------------------
        #region Method: void DoUndoRedoTest(string sPhraseExpected, string sWordExpected)
        void DoUndoRedoTest(string sPhraseExpected, string sWordExpected)
            // if sWordExpected is null, we ignore it as not being relevant to the test
        {
            // Do
            Assert.AreEqual(sPhraseExpected, P.DebugString.Substring(0, sPhraseExpected.Length), "Do");
            if (null != sWordExpected)
                Assert.AreEqual(sWordExpected, Wnd.Selection.Anchor.Word.Text, "Do");

            // Undo
            G.URStack.Undo();
            Assert.AreEqual(c_DebugString, P.DebugString, "Undo");

            // Redo
            G.URStack.Redo();
            Assert.AreEqual(sPhraseExpected, P.DebugString.Substring(0, sPhraseExpected.Length), "Redo");
            if (null != sWordExpected)
                Assert.AreEqual(sWordExpected, Wnd.Selection.Anchor.Word.Text, "Redo");
        }
        #endregion

        // Italics ---------------------------------------------------------------------------
        // In Word, if any portion of the selected text is not italic, then the toggle turns
        // everything to italics. Otherwise, the selection is homogenous and is thus toggled
        // to its opposite value.
        #region Test: Italics_HomogenousSelectionOn
        [Test] public void Italics_HomogenousSelectionOn()
        {
            // Italicize "God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 4, 7);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();
            DoUndoRedoTest("{c 3}{v 16}For |iGod|r so loved", null);
        }
        #endregion
        #region Test: Italics_HomogenousSelectionOff
        [Test] public void Italics_HomogenousSelectionOff()
        {
            // Un-italicize "world "
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 21, 27);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();
            DoUndoRedoTest("{c 3}{v 16}For God so loved the world that he", null);
        }
        #endregion
        #region Test: Italics_MixedSelectionOn
        [Test] public void Italics_MixedSelectionOn()
        {
            // Italicize "the world"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 17, 27);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();
            DoUndoRedoTest("{c 3}{v 16}For God so loved |ithe world |rthat he", null);
        }
        #endregion
        #region Test: Italics_FirstWord
        [Test] public void Italics_FirstWord()
        {
            // Italicize "For"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 0, 3);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();
            DoUndoRedoTest("{c 3}{v 16}|iFor|r God so loved", null);
        }
        #endregion
        #region Test: Italics_LastWord
        [Test] public void Italics_LastWord()
        {
            // Italicize "For"
            int nEnd = DBT2.AsString.Length;
            int nStart = nEnd - 5;
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT2, nStart, nEnd);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();
            DoUndoRedoTest("{c 3}{v 16}For God so loved the |iworld |rthat he gave his one and " +
                "only son{v 17}that whosoever believes in him |ishall not perish, |rbut have everlasting " +
                "|ilife.|r", null);
        }
        #endregion
        #region Test: Italics_Twice
        [Test] public void Italics_Twice()
        {
            // Italicize "|For|"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 0, 3);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();

            // Italicize "Fo|r Go|d so loved"
            selection = OWWindow.Sel.CreateSel(OP, DBT1, 2, 6);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new ItalicsAction(Wnd)).Do();

            string sPhraseExpected = "{c 3}{v 16}|iFor Go|rd so loved";
            Assert.AreEqual(sPhraseExpected, P.DebugString.Substring(0, sPhraseExpected.Length), "Do");
        }
        #endregion

        // Making Selections -----------------------------------------------------------------
        #region Test: Select_NextWord
        [Test] public void Select_NextWord()
        {
            // Make a selection in the middle of "lo|ved"
            int iBlock = 5;
            OWWindow.Sel selection = new OWWindow.Sel(m_op, iBlock, 2);
            m_Window.Contents.Select_NextWord_Begin(selection);
            Assert.AreEqual(6, m_Window.Selection.Anchor.iBlock);
            Assert.AreEqual(0, m_Window.Selection.Anchor.iChar);
            Assert.IsTrue(m_Window.Selection.IsInsertionPoint, "IsInsertionPoint");

            // Select at the beginning of "son". Should skip "17" and go to "that"
            iBlock = 15;
            selection = new OWWindow.Sel(m_op, iBlock, 0);
            m_Window.Contents.Select_NextWord_Begin(selection);
            Assert.AreEqual(17, m_Window.Selection.Anchor.iBlock);
            Assert.AreEqual(0, m_Window.Selection.Anchor.iChar);

            // Select in the final word: Should return null
            iBlock = 28;
            selection = new OWWindow.Sel(m_op, iBlock, 0);
            bool bSelectionMade = m_Window.Contents.Select_NextWord_Begin(selection);
            Assert.IsFalse(bSelectionMade, "No selection made.");
        }
        #endregion
        #region Test: Select_PrevWord
        [Test] public void Select_PreviousWord()
        {
            // Request to select the previous word from "loved": Should be the beginning of the current block
            int iBlock = 5;
            int iChar = 3;
            OWWindow.Sel selection = new OWWindow.Sel(m_op, iBlock, iChar);
            m_op.Window.Contents.Select_PrevWord_End(selection);
            Assert.AreEqual(4, m_Window.Selection.Anchor.iBlock);
            Assert.AreEqual(3, m_Window.Selection.Anchor.iChar);
            Assert.IsTrue(selection.IsInsertionPoint, "IsInsertionPoint");

            selection = new OWWindow.Sel(m_op, iBlock, iChar);
            m_op.Window.Contents.Select_PrevWord_Begin(selection);
            Assert.AreEqual(4, m_Window.Selection.Anchor.iBlock);
            Assert.AreEqual(0, m_Window.Selection.Anchor.iChar);
            Assert.IsTrue(selection.IsInsertionPoint, "IsInsertionPoint");

            // Select at the beginning of "that". Should skip "17" and go to "son"
            iBlock = 17;
            selection = new OWWindow.Sel(m_op, iBlock, 0);
            m_op.Window.Contents.Select_PrevWord_End(selection);
            Assert.AreEqual(15, m_Window.Selection.Anchor.iBlock);
            Assert.AreEqual(3, m_Window.Selection.Anchor.iChar);

            selection = new OWWindow.Sel(m_op, iBlock, 0);
            m_op.Window.Contents.Select_PrevWord_Begin(selection);
            Assert.AreEqual(15, m_Window.Selection.Anchor.iBlock);
            Assert.AreEqual(0, m_Window.Selection.Anchor.iChar);

            // Select in the first word: Should return null
            iBlock = 2;
            selection = new OWWindow.Sel(m_op, iBlock, 0);
            bool bSuccessful = m_op.Window.Contents.Select_PrevWord_Begin(selection);
            Assert.IsFalse(bSuccessful, "No selection made.");
            bSuccessful = m_op.Window.Contents.Select_PrevWord_End(selection);
            Assert.IsFalse(bSuccessful, "No selection made.");
        }
        #endregion
        #region Test: Select_BeginningOfFirstWord
        [Test] public void Select_BeginningOfFirstWord()
        {
            bool bSuccessful = m_op.Select_FirstWord();
            Assert.IsTrue(bSuccessful, "Selection was made");
            Assert.AreEqual("For ", Wnd.Selection.Anchor.Word.Text);
        }
        #endregion
        #region Test: Select_EndOfLastWord
        [Test] public void Select_EndOfLastWord()
        {
            bool bSuccessful = m_op.Select_LastWord_End();
            Assert.IsTrue(bSuccessful, "Selection was made");
            Assert.AreEqual("life.", m_Window.Selection.Anchor.Word.Text);
        }
        #endregion

        // Extending Selections --------------------------------------------------------------
        #region Test: ExtendSelection_WordRight
        [Test]
        public void ExtendSelection_WordRight()
        {
            // Make an Insertion Point in the middle of "lo|ved"
            int iBlock = 5;
            int iChar = 2;
            OWWindow.Sel selection = new OWWindow.Sel(m_op, new OWWindow.Sel.SelPoint(iBlock, iChar));
            Assert.AreEqual(iBlock, selection.Anchor.iBlock);
            Assert.AreEqual(iChar, selection.Anchor.iChar);
            Assert.IsTrue(selection.IsInsertionPoint, "IsInsertionPoint");

            // Extend to the right...#1  > "ved"
            selection = m_op.ExtendSelection_WordRight(selection);
            Assert.AreEqual("ved ", selection.SelectionString);

            // Extend to the right...#2  > "ved the "
            selection = m_op.ExtendSelection_WordRight(selection);
            Assert.AreEqual("ved the ", selection.SelectionString);

            // Extend to the right...#3  > "ved the world " (moving over italice)
            selection = m_op.ExtendSelection_WordRight(selection);
            Assert.AreEqual("ved the world ", selection.SelectionString);

            // Extend to the right...#4  > "ved the world that " (moving past italics)
            selection = m_op.ExtendSelection_WordRight(selection);
            Assert.AreEqual("ved the world that ", selection.SelectionString);

            // Boundary: Extend lots and lots of times; should stay at end of the DBT
            for (int i = 0; i < 100; i++)
                selection = m_op.ExtendSelection_WordRight(selection);
            Assert.AreEqual("ved the world that he gave his one and only son", selection.SelectionString);

            // Select at the end of a DBT: Extend should do nothing
            selection = new OWWindow.Sel(m_op, selection.End);
            selection = m_op.ExtendSelection_WordRight(selection);
            Assert.IsTrue(selection.IsInsertionPoint);

            // Select at the beginning of a word: Extend should get the entire word
            selection = new OWWindow.Sel(m_op, 5, 0);
            selection = m_op.ExtendSelection_WordRight(selection);
            Assert.AreEqual("loved ", selection.SelectionString);
        }
        #endregion
        #region Test: ExtendSelection_WordLeft
        [Test]
        public void ExtendSelection_WordLeft()
        {
            // Make an Insertion Point in the middle of "lo|ved"
            int iBlock = 5;
            int iChar = 2;
            OWWindow.Sel selection = new OWWindow.Sel(m_op, new OWWindow.Sel.SelPoint(iBlock, iChar));
            Assert.AreEqual(iBlock, selection.Anchor.iBlock);
            Assert.AreEqual(iChar, selection.Anchor.iChar);
            Assert.IsTrue(selection.IsInsertionPoint, "IsInsertionPoint");

            // Extend to the left...#1  > "lo"
            selection = m_op.ExtendSelection_WordLeft(selection);
            Assert.AreEqual("lo", selection.SelectionString);

            // Extend to the left...#2  > "so lo"
            selection = m_op.ExtendSelection_WordLeft(selection);
            Assert.AreEqual("so lo", selection.SelectionString);

            // Boundary: Extend lots and lots of times; should stay at beginning of the DBT
            for (int i = 0; i < 100; i++)
                selection = m_op.ExtendSelection_WordLeft(selection);
            Assert.AreEqual("For God so lo", selection.SelectionString);

            // Select at the beginning of a DBT: Extend should do nothing
            selection = new OWWindow.Sel(m_op, new OWWindow.Sel.SelPoint(2, 0));
            selection = m_op.ExtendSelection_WordLeft(selection);
            Assert.IsTrue(selection.IsInsertionPoint, "IsInsertionPoint at Beginning of DBT");
        }
        #endregion
        #region Test: ExtendSelection_FarRight
        [Test]
        public void ExtendSelection_FarRight()
        {
            // Make a selection at "lo|ved"
            OWWindow.Sel selection = new OWWindow.Sel(m_op, 5, 2);

            // Extend it to the right
            selection = m_op.ExtendSelection_FarRight(selection);
            Assert.AreEqual("ved the world that he gave his one and only son",
                selection.SelectionString);

            // Boundary condition: make the selection at the end of a DBT
            selection = new OWWindow.Sel(m_op, 15, 3);
            selection = m_op.ExtendSelection_FarRight(selection);
            Assert.IsTrue(selection.IsInsertionPoint, "Should be an insertion point.");
        }
        #endregion
        #region Test: ExtendSelection_FarLeft
        [Test]
        public void ExtendSelection_FarLeft()
        {
            // Make a selection at "lo|ved"
            OWWindow.Sel selection = new OWWindow.Sel(m_op, 5, 2);

            // Extend it to the left
            selection = m_op.ExtendSelection_FarLeft(selection);
            Assert.AreEqual("For God so lo", selection.SelectionString);

            // Boundary condition: make the selection at the beginning of a DBT
            selection = new OWWindow.Sel(m_op, 2, 0);
            selection = m_op.ExtendSelection_FarLeft(selection);
            Assert.IsTrue(selection.IsInsertionPoint, "Should be an insertion point.");
        }
        #endregion
        #region Test: ExtendSelection_CharRight
        [Test]
        public void ExtendSelection_CharRight()
        {
            // Make a selection at "lo|ved"
            OWWindow.Sel selection = new OWWindow.Sel(m_op, 5, 2);

            // Extend it to the right several times
            selection = m_op.ExtendSelection_CharRight(selection);
            Assert.AreEqual("v", selection.SelectionString);
            selection = m_op.ExtendSelection_CharRight(selection);
            Assert.AreEqual("ve", selection.SelectionString);
            selection = m_op.ExtendSelection_CharRight(selection);
            Assert.AreEqual("ved", selection.SelectionString);
            selection = m_op.ExtendSelection_CharRight(selection);
            Assert.AreEqual("ved ", selection.SelectionString);
            selection = m_op.ExtendSelection_CharRight(selection);
            Assert.AreEqual("ved t", selection.SelectionString);

            // Boundary: should be able to extend at the end of a DBT
            // Boundary condition: make the selection at the end of a DBT
            selection = new OWWindow.Sel(m_op, 15, 3);
            selection = m_op.ExtendSelection_CharRight(selection);
            Assert.IsTrue(selection.IsInsertionPoint, "Should be an insertion point.");
        }
        #endregion
        #region Test: ExtendSelection_CharLeft
        [Test]
        public void ExtendSelection_CharLeft()
        {
            // Make a selection at "lo|ved"
            OWWindow.Sel selection = new OWWindow.Sel(m_op, 5, 2);

            // Extend it to the right several times
            selection = m_op.ExtendSelection_CharLeft(selection);
            Assert.AreEqual("o", selection.SelectionString);
            selection = m_op.ExtendSelection_CharLeft(selection);
            Assert.AreEqual("lo", selection.SelectionString);
            selection = m_op.ExtendSelection_CharLeft(selection);
            Assert.AreEqual(" lo", selection.SelectionString);
            selection = m_op.ExtendSelection_CharLeft(selection);
            Assert.AreEqual("o lo", selection.SelectionString);
            selection = m_op.ExtendSelection_CharLeft(selection);
            Assert.AreEqual("so lo", selection.SelectionString);

            // Boundary condition: make the selection at the beginning of a DBT
            selection = new OWWindow.Sel(m_op, 2, 0);
            selection = m_op.ExtendSelection_CharLeft(selection);
            Assert.IsTrue(selection.IsInsertionPoint, "Should be an insertion point.");
        }
        #endregion

        // Insertions ------------------------------------------------------------------------
        #region Test: Insert_MidWord
        [Test] public void Insert_MidWord()
        {
            // At "F|or God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 1);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, "Hi")).Do();
            DoUndoRedoTest("{c 3}{v 16}FHior God", "FHior ");
        }
        #endregion
        #region Test: Insert_ParaBegin
        [Test] public void Insert_ParaBegin()
        {
            // At "|For God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 0);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, "Hi")).Do();
            DoUndoRedoTest("{c 3}{v 16}HiFor God", "HiFor ");
        }
        #endregion
        #region Test: Insert_ParaEnd
        [Test] public void Insert_ParaEnd()
        {
            // At "...everlasting life.|"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT2, DBT2.Phrases.AsString.Length);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, "Hi")).Do();
            string sPhraseExpected = c_DebugString + "Hi";
            DoUndoRedoTest(sPhraseExpected, "life.Hi");
        }
        #endregion
        #region Test: Insert_PhraseBegin
        [Test] public void Insert_PhraseBegin()
        {
            // At "For God so loved the |world"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 21);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, "Hi")).Do();
            DoUndoRedoTest("{c 3}{v 16}For God so loved the |iHiworld", "Hiworld ");
        }
        #endregion
        #region Test: Insert_PhraseEnd
        [Test] public void Insert_PhraseEnd()
        {
            // At "{c 3}{v 16}For God so loved the world |that he "
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 27);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, "Hi")).Do();
            string sPhraseExpected = "{c 3}{v 16}For God so loved the |iworld |rHithat he ";
            DoUndoRedoTest(sPhraseExpected, "Hithat ");
        }
        #endregion
        #region Test: Insert_SpaceBeforeSpace - make sure we don't insert double spaces
        [Test] public void Insert_SpaceBeforeSpace()
        {
            // At "For| God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 3);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, " ")).Do();
            DoUndoRedoTest("{c 3}{v 16}For God", "God ");
        }
        #endregion
        #region Test: Insert_OverWord
        [Test] public void Insert_OverWord()
        {
            // We want to select a word, but not any of the before/after spaces
            // At "For |God| so"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 4, 7);
            Wnd.Selection = OP.NormalizeSelection(selection);
            (new InsertAction("Insert", Wnd, "Tuhan Allah")).Do();
            DoUndoRedoTest("{c 3}{v 16}For Tuhan Allah so", "Allah ");
        }
        #endregion

        #region Test: AutoReplace
        [Test] public void AutoReplace()
        {
            // Build some AutoReplace sequences
            WSVernacular.AutoReplace.Add("GodQ", "Tuhan Allah");
            WSVernacular.AutoReplace.Add("sov", "very much");

            // Select at "For God| so loved"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 7);
            Wnd.Selection = OP.NormalizeSelection(selection);

            // Simulate typing "Q"
            (new TypingAction(Wnd, 'Q')).Do();

            // Test the result
            DoUndoRedoTest("{c 3}{v 16}For Tuhan Allah so loved", "Allah ");
        }
        #endregion
        #region Test: AutoReplace_DisableOverSelection
        [Test] public void AutoReplace_DisableOverSelection()
        {
            // Build some AutoReplace sequences
            WSVernacular.AutoReplace.Add("GQ", "Tuhan Allah");
            WSVernacular.AutoReplace.Add("sov", "very much");

            // Select at "For G|od| so loved"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 5, 7);
            Wnd.Selection = OP.NormalizeSelection(selection);

            // Simulate typing "Q"
            (new TypingAction(Wnd, 'Q')).Do();

            // If AutoReplace were active, we'd get "{c 3}{v 16}For Tuhan Allah so loved"
            // But I believe we don't want to do this, as we are in effect combining
            // letters from before the selection with those after. So instead, we just
            // want what is typed to happen, thus "GQ" stays in the text, not 
            // "Tuhan Allah"

            // Test the result
            DoUndoRedoTest("{c 3}{v 16}For GQ so loved", "GQ ");
        }
        #endregion

        #region Test: AdjacentTypingIsSingleUndo_TwoChars
        [Test] public void AdjacentTypingIsSingleUndo_TwoChars()
        {
            // At "F|or God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 1);
            Wnd.Selection = OP.NormalizeSelection(selection);

            // Type several letters
            (new TypingAction(Wnd, 'H')).Do();
            (new TypingAction(Wnd, 'E')).Do();

            // This test assumes a single Undo call and a single Redo call
            DoUndoRedoTest("{c 3}{v 16}FHEor God", "FHEor ");
        }
        #endregion
        #region Test: AdjacentTypingIsSingleUndo_ManyChars
        [Test] public void AdjacentTypingIsSingleUndo_ManyChars()
        {
            // At "F|or God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 1);
            Wnd.Selection = OP.NormalizeSelection(selection);

            // Type several letters
            (new TypingAction(Wnd, 'H')).Do();
            (new TypingAction(Wnd, 'E')).Do();
            (new TypingAction(Wnd, 'L')).Do();
            (new TypingAction(Wnd, 'L')).Do();
            (new TypingAction(Wnd, 'O')).Do();
            (new TypingAction(Wnd, ' ')).Do();
            (new TypingAction(Wnd, 'T')).Do();
            (new TypingAction(Wnd, 'H')).Do();
            (new TypingAction(Wnd, 'E')).Do();
            (new TypingAction(Wnd, 'R')).Do();
            (new TypingAction(Wnd, 'E')).Do();

            // This test assumes a single Undo call and a single Redo call
            DoUndoRedoTest("{c 3}{v 16}FHELLO THEREor God", "THEREor ");
        }
        #endregion
        #region Test: AdjacentTypingIsSingleUndo_WithAutoReplace
        [Test] public void AdjacentTypingIsSingleUndo_WithAutoReplace()
        {
            // Build some AutoReplace sequences
            WSVernacular.AutoReplace.Add("GodQQ", "Tuhan");
            WSVernacular.AutoReplace.Add("sov", "very much");

            // At "For God| so"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 7);
            Wnd.Selection = OP.NormalizeSelection(selection);

            // Type several letters
            (new TypingAction(Wnd, 'Q')).Do();
            (new TypingAction(Wnd, 'Q')).Do();
            (new TypingAction(Wnd, ' ')).Do();
            (new TypingAction(Wnd, 'A')).Do();
            (new TypingAction(Wnd, 'l')).Do();
            (new TypingAction(Wnd, 'l')).Do();
            (new TypingAction(Wnd, 'a')).Do();
            (new TypingAction(Wnd, 'h')).Do();
            (new TypingAction(Wnd, ' ')).Do();

            // This test assumes a single Undo call and a single Redo call
            DoUndoRedoTest("{c 3}{v 16}For Tuhan Allah so", "so ");
        }
        #endregion

        // Delete (& Undo Delete) ------------------------------------------------------------
        #region Test: DeletePoint_MidWord
        [Test] public void DeletePoint_MidWord()
        {
            // Delete the first letter, "F|or God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 1);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete(); 

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}Fr God", "Fr ");
        }
        #endregion
        #region Test: DeletePoint_ParaBegin
        [Test] public void DeletePoint_ParaBegin()
        {
            // Delete the first letter, "|For God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 0);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}or God", "or ");
        }
        #endregion
        #region Test: DeletePoint_ParaLastChar
        [Test] public void DeletePoint_ParaLastChar()
        {
            // Delete at "...everlasting life|."
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT2, 74);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            string sPhraseExpected = c_DebugString.Substring(0, c_DebugString.Length - 1);
            DoUndoRedoTest(sPhraseExpected, "life");
        }
        #endregion
        #region Test: DeletePoint_PhraseBegin
        [Test] public void DeletePoint_PhraseBegin()
        {
            // Delete at "For God so loved the |world"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 21);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For God so loved the |iorld", "orld ");
        }
        #endregion
        #region Test: DeletePoint_PhraseEnd
        [Test] public void DeletePoint_PhraseEnd()
        {
            // Delete at "{c 3}{v 16}For God so loved the world |that he "
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 27);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Undo/Redo Test
            string sPhraseExpected = "{c 3}{v 16}For God so loved the |iworld |rhat he ";
            DoUndoRedoTest(sPhraseExpected, "hat ");
        }
        #endregion

        #region Test: DeleteSel_MidWord
        [Test] public void DeleteSel_MidWord()
        {
            // Delete the first letter, "For God so l|ove|d the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 12, 15);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For God so ld the", "ld ");
        }
        #endregion
        #region Test: DeleteSel_WholeWord
        [Test] public void DeleteSel_WholeWord()
        {
            // Delete the first letter, "For God so |loved |the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 11, 17);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For God so the", "the ");
        }
        #endregion
        #region Test: DeleteSel_MultipleWords
        [Test] public void DeleteSel_MultipleWords()
        {
            // Delete the first letter, "For |God so loved |the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 4, 17);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For the", "the ");
        }
        #endregion
        #region Test: DeleteSel_PartialMultipleWords
        [Test] public void DeleteSel_PartialMultipleWords()
        {
            // Delete the first letter, "For G|od so loved t|he"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 5, 18);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For Ghe", "Ghe ");
        }
        #endregion
        #region Test: DeleteSel_AcrossMidPhrases
        [Test] public void DeleteSel_AcrossMidPhrases()
        {
            // Delete the first letter, "For God so lo|ved the |iwo|rld |rthat he "
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 13, 23);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For God so lo|irld |rthat he ", "rld ");
        }
        #endregion
        #region Test: DeleteSel_MiddlePhrase
        [Test] public void DeleteSel_MiddlePhrase()
        {
            // Delete the first letter, "For God so loved the ||iworld |r|that he "
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 21, 27);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            string sPhrase = "{c 3}{v 16}For God so loved the that ";
            string sWord = "that ";
            Assert.AreEqual(1, DBT1.Phrases.Count, "Two remaining phrases should be combined.");
            DoUndoRedoTest(sPhrase, sWord);
        }
        #endregion
        #region Test: DeleteSel_EntireDBT
        [Test] public void DeleteSel_EntireDBT()
        {
            // Delete the first letter, "For God so l|ove|d the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 0, DBT1.Phrases.AsString.Length);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdDelete();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}{v 17}", DPhrase.c_chInsertionSpace.ToString());
        }
        #endregion

        // Backspace (& Undo Backspace) ------------------------------------------------------
        #region Test: Backspace_MidWord
        [Test] public void Backspace_MidWord()
        {
            // Backspace the first letter, "Fo|r God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 2);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdBackspace();

            // Check results
            string sPhraseExpected = "{c 3}{v 16}Fr God";
            string sWordExpected = "Fr ";

            // Do/Undo/Redo Test
            DoUndoRedoTest(sPhraseExpected, sWordExpected);
        }
        #endregion
        #region Test: Backspace_ParaFirstChar
        [Test] public void Backspace_ParaFirstChar()
        {
            // Backspace the first letter, "F|or God"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 1);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdBackspace();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}or God", "or ");
        }
        #endregion
        #region Test: Backspace_ParaEnd
        [Test] public void Backspace_ParaEnd()
        {
            // Backspace at "...everlasting life.|"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT2, 75);
            // Console.WriteLine(selection.Anchor.Word.Text);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdBackspace();

            // Do/Undo/Redo Test
            string sPhraseExpected = c_DebugString.Substring(0, c_DebugString.Length - 1);
            DoUndoRedoTest(sPhraseExpected, "life");
        }
        #endregion
        #region Test: Backspace_PhraseBegin
        [Test] public void Backspace_PhraseBegin()
        {
            // Backspace at "For God so loved the |world"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 21);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdBackspace();

            // Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For God so loved the|iworld", "world ");
        }
        #endregion
        #region Test: Backspace_PhraseEnd
        [Test] public void Backspace_PhraseEnd()
        {
            // Backspace at "{c 3}{v 16}For God so loved the world |that he "
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 27);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdBackspace();

            // Undo/Redo Test
            string sPhraseExpected = "{c 3}{v 16}For God so loved the |iworld|rthat he ";
            DoUndoRedoTest(sPhraseExpected, "that ");
        }
        #endregion
        #region Test: BackspaceSel_MultipleWords (since Bksp same as Del, we'll not test other conditions)
        [Test] public void BackspaceSel_MultipleWords()
        {
            // Delete the first letter, "For |God so loved |the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 4, 17);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdBackspace();

            // Do/Undo/Redo Test
            DoUndoRedoTest("{c 3}{v 16}For the", "the ");
        }
        #endregion

        // Cut,Copy & Paste(& Undo) ----------------------------------------------------------
        #region Test: Cut
        [Test] public void Cut()
            // No need to re-do all of the deletion tests; we just want to check clipboard behavior
        {
            
            // Place something on the clipboard
            string sOriginalClipboard = "Fruit flies like a banana.";
            Clipboard.SetText(sOriginalClipboard);

            // Delete the first word, "For God so |loved |the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 11, 17);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdCut();
            Assert.AreEqual("loved ", Clipboard.GetText());

            // Delete another word, "God"
            selection = OWWindow.Sel.CreateSel(OP, DBT1, 4, 8);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdCut();
            Assert.AreEqual("God ", Clipboard.GetText());

            // Undo #1
            G.URStack.Undo();
            Assert.AreEqual("loved ", Clipboard.GetText());

            // Undo #2
            G.URStack.Undo();
            Assert.AreEqual(sOriginalClipboard, Clipboard.GetText());

            // Redo
            G.URStack.Redo();
            Assert.AreEqual("loved ", Clipboard.GetText());

            // Redo
            G.URStack.Redo();
            Assert.AreEqual("God ", Clipboard.GetText());
        }
        #endregion
        #region Test: Copy
        [Test] public void Copy()
            // Check clipboard behavior
        {
            
            // Place something on the clipboard
            string sOriginalClipboard = "Fruit flies like a banana.";
            Clipboard.SetText(sOriginalClipboard);

            // Delete the first word, "For God so |loved |the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 11, 17);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdCopy();
            Assert.AreEqual("loved ", Clipboard.GetText());
            Assert.AreEqual(c_DebugString, P.DebugString);

            // Delete another word, "God"
            selection = OWWindow.Sel.CreateSel(OP, DBT1, 4, 8);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdCopy();
            Assert.AreEqual("God ", Clipboard.GetText());
            Assert.AreEqual(c_DebugString, P.DebugString);

            // Undo #1
            G.URStack.Undo();
            Assert.AreEqual("loved ", Clipboard.GetText());
            Assert.AreEqual(c_DebugString, P.DebugString);

            // Undo #2
            G.URStack.Undo();
            Assert.AreEqual(sOriginalClipboard, Clipboard.GetText());
            Assert.AreEqual(c_DebugString, P.DebugString);

            // Redo
            G.URStack.Redo();
            Assert.AreEqual("loved ", Clipboard.GetText());
            Assert.AreEqual(c_DebugString, P.DebugString);

            // Redo
            G.URStack.Redo();
            Assert.AreEqual("God ", Clipboard.GetText());
            Assert.AreEqual(c_DebugString, P.DebugString);
        }
        #endregion
        #region Test: Paste
        [Test] public void Paste()
        {
            // Place something on the clipboard
            string sClipboard = "Hello";
            Clipboard.SetText(sClipboard);

            // Paste it at "For God so lo|ved the"
            OWWindow.Sel selection = OWWindow.Sel.CreateSel(OP, DBT1, 13);
            Wnd.Selection = OP.NormalizeSelection(selection);
            Wnd.cmdPaste();
            DoUndoRedoTest("{c 3}{v 16}For God so loHelloved the", "loHelloved ");
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Test: NormalizeSelection
        [Test] public void NormalizeSelection()
        {
            // Part 1: Insertion Points

            // Make a selection at the end of "loved"
            OWWindow.Sel selection = new OWWindow.Sel(m_op, new OWWindow.Sel.SelPoint(5, 6));

            // Normlizing should move it to the next one
            selection = m_op.NormalizeSelection(selection);
            Assert.AreEqual(6, selection.Anchor.iBlock);
            Assert.AreEqual(0, selection.Anchor.iChar);

            // Normlizing again should have no effect
            selection = m_op.NormalizeSelection(selection);
            Assert.AreEqual(6, selection.Anchor.iBlock);
            Assert.AreEqual(0, selection.Anchor.iChar);

            // Place at the end of a DBT: normalizing should not move it
            selection = new OWWindow.Sel(m_op, new OWWindow.Sel.SelPoint(15, 3));
            selection = m_op.NormalizeSelection(selection);
            Assert.AreEqual(15, selection.Anchor.iBlock);
            Assert.AreEqual(3, selection.Anchor.iChar);

            // Part 2: Extend Selections
            // Make a selection the latter half of "loved"
            selection = new OWWindow.Sel(m_op,
                new OWWindow.Sel.SelPoint(5, 3),
                new OWWindow.Sel.SelPoint(5, 6));
            selection = m_op.NormalizeSelection(selection);
            Assert.AreEqual(5, selection.Anchor.iBlock);
            Assert.AreEqual(3, selection.Anchor.iChar);
            Assert.AreEqual(6, selection.End.iBlock);
            Assert.AreEqual(0, selection.End.iChar);

            // Normlizing again should have no effect
            selection = m_op.NormalizeSelection(selection);
            Assert.AreEqual(5, selection.Anchor.iBlock);
            Assert.AreEqual(3, selection.Anchor.iChar);
            Assert.AreEqual(6, selection.End.iBlock);
            Assert.AreEqual(0, selection.End.iChar);
        }
        #endregion

        // Hyphenation -----------------------------------------------------------------------
        #region Method: DParagraph CreateParagraph_LongHuicholWords()
        private DParagraph CreateParagraph_LongHuicholWords()
        {
            // Create a paragraph
            var p = new DParagraph(StyleSheet.Paragraph);

            // Add various runs
            p.AddRun(DVerse.Create("16"));

            m_DBT1 = new DText();
            m_DBT1.Phrases.Append(new DPhrase("Mepücatemaicai memü memüteyurieniquecai. "));
            m_DBT1.Phrases.Append(new DPhrase("yuxexuitü ") { FontToggles = FontStyle.Italic });
            m_DBT1.Phrases.Append(new DPhrase("ivaviyacaitüni tineunaque quetatineutaxatüa."));
            p.AddRun(m_DBT1);

            p.AddRun(DVerse.Create("17"));

            m_DBT2 = new DText();
            m_DBT2.Phrases.Append(new DPhrase("Haqueva pepeyetüa? "));
            m_DBT2.Phrases.Append(new DPhrase("Quenanucuqueca! 'Acacaüyari queneutahivi! ") { FontToggles = FontStyle.Italic });
            m_DBT2.Phrases.Append(new DPhrase("Mücü canicacaüyaritüni."));
            p.AddRun(m_DBT2);

            m_section.Paragraphs.Append(p);

            return p;
        }
        #endregion
        #region Test: ProposeNextLayoutChunk
        [Test] public void ProposeNextLayoutChunk()
        {
            // Zero out stuff, we're starting over
            Wnd.Clear();
            m_section.Paragraphs.Clear();

            // Set up the hyphenation
            WSVernacular.UseAutomatedHyphenation = true;

            // Create a paragraph with big Huchol words.
            m_p = CreateParagraph_LongHuicholWords();
            m_op = new OWPara(
                WSVernacular,
                m_p.Style,
                m_p,
                Color.Wheat,
                OWPara.Flags.IsEditable);
            Wnd.Contents.Append(m_op);

            // By nature of LoadData, all the EBlocks will have been measured
            Wnd.LoadData();

            // Add the widths of the first two EBlocks
            float fAvailWidth = 0;
            for (int i = 0; i < 2; i++)
                fAvailWidth += m_op.SubItems[i].Width;

            // TEST 1 - SHOULD JUST FIT
            // Request chunking from the paragraph
            OWPara.ProposeNextLayoutChunk chunk = new OWPara.ProposeNextLayoutChunk(
                Wnd.Draw.Graphics, m_op, fAvailWidth, 0, false);

            // Check the two and three words
            EWord wL = m_op.SubItems[1] as EWord;
            EWord wR = m_op.SubItems[2] as EWord;

            Assert.IsFalse(wL.Hyphenated, "1-First word should not be hyphenated");
            Assert.IsFalse(wR.Hyphenated, "1-Second word should not be hyphenated");

            Assert.AreEqual("Mepücatemaicai ", wL.Text);
            Assert.AreEqual("memü ", wR.Text);


            // TEST 2 - DOESNT' FIT
            // Subtract a bit to make it too small
            fAvailWidth -= 20;

            // Request chunking from the paragraph
            chunk = new OWPara.ProposeNextLayoutChunk(
                Wnd.Draw.Graphics, m_op, fAvailWidth, 0, false);

            // Check the two and three words
            wL = m_op.SubItems[1] as EWord;
            wR = m_op.SubItems[2] as EWord;

            Assert.IsTrue(wL.Hyphenated, "2-First word should be hyphenated");
            Assert.IsFalse(wR.Hyphenated, "2-Second word should not be hyphenated");

            Assert.AreEqual("Mepücate", wL.Text);
            Assert.AreEqual("maicai ", wR.Text);
        }
        #endregion
        #region Test: void RemoveHyphenation()
        [Test] public void RemoveHyphenation()
        {
            // Zero out stuff, we're starting over
            Wnd.Clear();
            m_section.Paragraphs.Clear();

            WSVernacular.UseAutomatedHyphenation = true;

            // Create a paragraph with big Huchol words.
            m_p = CreateParagraph_LongHuicholWords();
            m_op = new OWPara(
                WSVernacular,
                m_p.Style,
                m_p,
                Color.Wheat,
                OWPara.Flags.IsEditable);
            Wnd.Contents.Append(m_op);

            // How many blocks do we have currently
            int cBlocks = m_op.SubItems.Length;

            // By nature of LoadData, all the EBlocks will have been measured
            Wnd.LoadData();

            // Add the widths of the first two EBlocks
            float fAvailWidth = 0;
            for (int i = 0; i < 2; i++)
                fAvailWidth += m_op.SubItems[i].Width;

            // Subtract a bit to make it too small
            fAvailWidth -= 20;

            // Request chunking from the paragraph
            OWPara.ProposeNextLayoutChunk chunk = new OWPara.ProposeNextLayoutChunk(
                Wnd.Draw.Graphics, m_op, fAvailWidth, 0, false);

            // Because we hyphenated, we should have an extra block
            Assert.IsTrue(m_op.SubItems.Length > cBlocks, "Hyphenation created new blocks");
            Assert.AreEqual("Mepücate", (m_op.SubItems[1] as EWord).Text);

            // Now remove them
            m_op.RemoveHyphenation();

            // Should be back to the same number of blocks
            Assert.IsTrue(m_op.SubItems.Length == cBlocks , "New block has been removed");
            Assert.AreEqual("Mepücatemaicai ", (m_op.SubItems[1] as EWord).Text);
        }
        #endregion
    }
}
