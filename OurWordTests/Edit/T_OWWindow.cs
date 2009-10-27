/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_OWWindow.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the OWWindow class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordTests.DataModel;

using OurWordTests.Cellar;
#endregion

namespace OurWordTests.Edit
{

    [TestFixture] public class T_OWWindow
    {
        // Setup / Teardown ------------------------------------------------------------------
        #region Setup - using T_Section: TestData #4
        [SetUp] public void Setup()
        {
            EditTest.Setup(SectionTestData.BaikenoMark430_ImportVariant);
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            EditTest.TearDown();
        }
        #endregion

        // Undo-able Split Paragraph ---------------------------------------------------------
        // Paragraph 6 is the first content one, and I inserted an Italic phrase, thus: 
        const string c_sBenchmark =
            "{v 30}Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi " +
            "nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, " +
            "tal antee sin namfau nok.{BT Then Jesus spoke again, saying, <<I give another " +
            "example like this: You(pl) can compare God's *social group. From just a few " +
            "people, it nevertheless increases (in number), to the point that they are very many.}";
        const string c_sParagraphContents = // AsString does not return the italics
            "30Oke te, Jesus namolok antein, mnak, <<Au uleta' 'tein on ii: Hi " +
		    "nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes " +
		    "nabaab-took, tal antee sin namfau nok.";

        // So the following tests will split/join at various places in this paragraph
        // to check on expected behavior
        #region Helper: void _EvaluteSplitResults(int cParasPreSplit, int iPara, int iSplitPos, sRight)
        void _EvaluteSplitResults(int cParasPreSplit, int iPara, int iSplitPos, string sRight)
        {
            Assert.AreEqual(cParasPreSplit + 1, EditTest.Section.Paragraphs.Count, "Split paragraph count");

            DParagraph pLeft = EditTest.Section.Paragraphs[iPara] as DParagraph;
            DParagraph pRight = EditTest.Section.Paragraphs[iPara + 1] as DParagraph;

            int iSplitPos_PlusAllowForVerseNumber = iSplitPos + 2;

            Assert.AreEqual(
                c_sParagraphContents.Substring(0, iSplitPos_PlusAllowForVerseNumber),
                pLeft.AsString, 
                "Left");

            Assert.AreEqual(
                c_sParagraphContents.Substring(iSplitPos_PlusAllowForVerseNumber),
                pRight.AsString, 
                "Right");

            if (!string.IsNullOrEmpty(sRight))
            {
                string s = pRight.AsString.Substring(0, sRight.Length);
                Assert.AreEqual(sRight, s);
            }
        }
        #endregion
        #region Helper: void _EvaluateJoinResults(int cParasPreSplit, int iPara, iSplitPos)
        void _EvaluateJoinResults(int cParasPreSplit, int iPara, int iSplitPos)
        {
            Assert.AreEqual(cParasPreSplit, EditTest.Section.Paragraphs.Count, "Join paragraph count");

            DParagraph pUndone = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, pUndone.DebugString, "Joined");

            Assert.AreEqual(iSplitPos, EditTest.Wnd.Selection.DBT_iCharFirst);
        }
        #endregion
        #region Helper: void _SplitParagraphTest(iSplitPos, sRight)
        void _SplitParagraphTest(int iSplitPos, string sRight)
        {
            // Benchmark: Make sure we're starting where we think we are
            int cParagraphs = EditTest.Section.Paragraphs.Count;
            Assert.AreEqual(8, cParagraphs, "Benchmark: Initial paragraph count");
            int iPara = 5;
            DParagraph p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            DBasicText DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");

            // We'll use the OWBookmark code to locate its OWPara
            OWPara op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);

            // Set the cursor position
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, iSplitPos);

            // Split the paragraph at the requested position
            EditTest.Wnd.cmdSplitParagraph();

            // Look at the results: two paragraphs
            _EvaluteSplitResults(cParagraphs, iPara, iSplitPos, sRight);

            // Undo and wee if we have what we thought.
            G.URStack.Undo();
            _EvaluateJoinResults(cParagraphs, iPara, iSplitPos);

            // Redo, Undo again
            G.URStack.Redo();
            _EvaluteSplitResults(cParagraphs, iPara, iSplitPos, sRight);
            G.URStack.Undo();
            _EvaluateJoinResults(cParagraphs, iPara, iSplitPos);
        }
        #endregion

        #region Test: SplitParagraph_AfterSpaceBeforeWord
        [Test] public void SplitParagraph_AfterSpaceBeforeWord()
        {
            // Split at "Oke |te";
            _SplitParagraphTest(4, "te");
        }
        #endregion
        #region Test: SplitParagraph_BeforeSpaceAfterWord
        [Test] public void SplitParagraph_BeforeSpaceAfterWord()
        {
            // Split at "Oke| te";
            _SplitParagraphTest(3, " te");
        }
        #endregion
        #region Test: SplitParagraph_MidWord
        [Test] public void SplitParagraph_MidWord()
        {
            // Split at "Oke te, Jesus nam|olok antein";
            _SplitParagraphTest(17, "olok");
        }
        #endregion
        #region Test: SplitParagraph_PhraseBegin
        [Test] public void SplitParagraph_PhraseBegin()
        {
            // Split at "Oke te, |Jesus namolok antein";
            _SplitParagraphTest(8, "Jesus");
        }
        #endregion
        #region Test: SplitParagraph_PhraseEnd
        [Test] public void SplitParagraph_PhraseEnd()
        {
            // Split at "Oke te, Jesus namolok| antein";
            _SplitParagraphTest(21, " antein");
        }
        #endregion
        #region Test: SplitParagraph_ParaBegin
        [Test] public void SplitParagraph_ParaBegin()
        {
            // Split at "Oke te, Jesus namolok| antein";
            _SplitParagraphTest(0, "Oke");
        }
        #endregion
        #region Test: SplitParagraph_ParaEnd
        [Test] public void SplitParagraph_ParaEnd()
        {
            // Split at "namfau nok.|";
            int c = c_sParagraphContents.Length;
            c -= 2; // subtract the verse length ("30")
            _SplitParagraphTest(c, "");
        }
        #endregion

        #region Test: MultiLevel_SplitUndoRedo
        [Test] public void MultiLevel_SplitUndoRedo()
        {
            // We have an issue when we create split twice, then Undo twice, then Redo twice,
            // that the second Redo fails. So we want to simulate that here to keep the 
            // Fix from ever going wrong again.

            // Benchmark: Make sure we're starting where we think we are
            int cParagraphs = EditTest.Section.Paragraphs.Count;
            Assert.AreEqual(8, cParagraphs, "Benchmark: Initial paragraph count");
            int iPara = 5;
            DParagraph p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            DBasicText DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");

            // First Split: at "nam|olok"
            int iSplitPos1 = 17;
            OWPara op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, iSplitPos1);
            EditTest.Wnd.cmdSplitParagraph();
            _EvaluteSplitResults(cParagraphs, iPara, iSplitPos1, "olok");

            // Second Split, at "t|e "
            int iSplitPos2 = 5;
            p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);
            DBT = p.Runs[1] as DBasicText;
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, iSplitPos2);
            EditTest.Wnd.cmdSplitParagraph();
            p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual("30Oke t", p.AsString);

            // Undo them both
            G.URStack.Undo();
            G.URStack.Undo();
            _EvaluateJoinResults(cParagraphs, iPara, iSplitPos1);

            // Redo them both
            G.URStack.Redo();
            G.URStack.Redo();
            Assert.AreEqual("30Oke t", p.AsString);
        }
        #endregion

        #region Test: Join_FirstParaEndsWithSpace
        [Test] public void Join_FirstParaEndsWithSpace()
            // Bug0281
        {
            // Split a paragraph so that we get one that does not start with a verse
            // First, figure out where we are
            int iPara = 5;
            DParagraph p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            DBasicText DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");

            // We'll use the OWBookmark code to locate its OWPara
            OWPara op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);

            // Set the cursor position  "Oke |te," and do the split
            // P: "Oke "
            // P: "|te, ..."
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 4);
            EditTest.Wnd.cmdSplitParagraph();
            // Console.WriteLine("1 = <" + EditTest.Wnd.Selection.DBT.AsString.Substring(0,15) + ">, expecting <te, ...>"); 

            // Now, do a split again, so we have
            // P: "Oke "
            // P: ""
            // P: "|te, ..."
            EditTest.Wnd.cmdSplitParagraph();
            // Console.WriteLine("2 = <" + EditTest.Wnd.Selection.DBT.AsString.Substring(0, 15) + ">, expecting <te, ...>"); 

            // Move to the previous paragraph
            // P: "Oke "
            // P: "|"
            // P: "te, ..."
            EditTest.Wnd.cmdMoveCharLeft();
            // Console.WriteLine("3 = <" + EditTest.Wnd.Selection.DBT.AsString + ">, expecting <>"); 

            // Type a blank space
            // P: "Oke "
            // P: " |"
            // P: "te, ..."
            (new TypingAction(EditTest.Wnd, ' ')).Do();
            // Console.WriteLine("4 = <" + EditTest.Wnd.Selection.DBT.AsString + ">, expecting < >"); 

            // Finally, do a Delete to join the paragraphs
            // P: "Oke "
            // P: " |te, ..."
            EditTest.Wnd.cmdDelete();
            // Console.WriteLine("5 = <" + EditTest.Wnd.Selection.DBT.AsString + ">, expecting < te, ...>"); 

            // Verify the resulting paragraph/selection is " |te, ..."
            Assert.IsTrue(EditTest.Wnd.Selection.IsInsertionPoint, "Should be a point, not a content selection");
            Assert.AreEqual(1, EditTest.Wnd.Selection.DBT_iCharFirst);
            Assert.AreEqual(" te, ", EditTest.Wnd.Selection.DBT.AsString.Substring(0, 5));
        }
        #endregion
    }
}
