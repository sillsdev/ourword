/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Bookmark.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the OWBookmark class
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

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData.DataModel.Runs;
using OurWordTests.DataModel;

using OurWordTests.Cellar;
#endregion

namespace OurWordTests.Edit
{
    [TestFixture] public class T_Bookmark
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

        #region Test: BasicInsertionPointBookmarks
        [Test] public void BasicInsertionPointBookmarks()
        {
            // We'll use the 6th paragraph
            int iPara = 5;
            DParagraph p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            DBasicText DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");

            // We'll use the OWBookmark code to locate its OWPara
            OWPara op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);

            // Select "Oke |te" and bookmark it
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 4);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            string s1 = "te, ";
            Assert.AreEqual(s1, EditTest.Wnd.Selection.Anchor.Word.Text, "Set Bookmark 1");
            Assert.AreEqual(2, EditTest.Wnd.Selection.Anchor.iBlock);
            Assert.AreEqual(0, EditTest.Wnd.Selection.Anchor.iChar);
            OWBookmark bm1 = EditTest.Wnd.CreateBookmark();

            // select "iJe|sus" and bookmark it
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 10);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            string s2 = "Jesus ";
            Assert.AreEqual(s2, EditTest.Wnd.Selection.Anchor.Word.Text, "Set Bookmark 2");
            Assert.AreEqual(3, EditTest.Wnd.Selection.Anchor.iBlock);
            Assert.AreEqual(2, EditTest.Wnd.Selection.Anchor.iChar);
            OWBookmark bm2 = EditTest.Wnd.CreateBookmark();

            // Return to the first bookmark
            bm1.RestoreWindowSelectionAndScrollPosition();
            Assert.AreEqual(s1, EditTest.Wnd.Selection.Anchor.Word.Text, "Return to " + s1);
            Assert.AreEqual(2, EditTest.Wnd.Selection.Anchor.iBlock, "1 - iBlock");
            Assert.AreEqual(0, EditTest.Wnd.Selection.Anchor.iChar, "1 - iChar");

            // Return to the second bookmark
            bm2.RestoreWindowSelectionAndScrollPosition();
            Assert.AreEqual(s2, EditTest.Wnd.Selection.Anchor.Word.Text, "Return to " + s2);
            Assert.AreEqual(3, EditTest.Wnd.Selection.Anchor.iBlock, "2 - iBlock");
            Assert.AreEqual(2, EditTest.Wnd.Selection.Anchor.iChar, "2 - iChar");
        }
        #endregion
        #region Test: BasicContentBookmarks
        [Test] public void BasicContentBookmarks()
        {
            // We'll use the 6th paragraph
            const int iPara = 5;
            var p = EditTest.Section.Paragraphs[iPara];
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            var DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");

            // We'll use the OWBookmark code to locate its OWPara
            var op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);

            // Select "Oke |te|" and bookmark it
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 4, 6);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            const string s1 = "te";
            Assert.AreEqual(s1, EditTest.Wnd.Selection.SelectionString);
            var bm1 = EditTest.Wnd.CreateBookmark();

            // Select "a|ntei|n" and bookmark it
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 23, 27);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            const string s2 = "ntei";
            Assert.AreEqual(s2, EditTest.Wnd.Selection.SelectionString);
            var bm2 = EditTest.Wnd.CreateBookmark();

            // Return to the first bookmark
            bm1.RestoreWindowSelectionAndScrollPosition();
            Assert.AreEqual(s1, EditTest.Wnd.Selection.SelectionString, "Return to " + s1);

            // Return to the second bookmark
            bm2.RestoreWindowSelectionAndScrollPosition();
            Assert.AreEqual(s2, EditTest.Wnd.Selection.SelectionString, "Return to " + s2);
        }
        #endregion
        #region Test: BookmarkEquality
        [Test] public void BookmarkEquality()
        {
            // We'll use the 6th paragraph
            int iPara = 5;
            DParagraph p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            DBasicText DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");
            OWPara op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);

            // Select "Oke |te" and bookmark it
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 4);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            OWBookmark bm1 = EditTest.Wnd.CreateBookmark();

            // Create another one: should be equal
            OWBookmark bm2 = EditTest.Wnd.CreateBookmark();
            Assert.IsTrue(bm1.ContentEquals(bm2), "bookmark equals");

            // Different selection should not be equal
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 5);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            bm2 = EditTest.Wnd.CreateBookmark();
            Assert.IsFalse(bm1.ContentEquals(bm2), "bookmark not equal");
        }
        #endregion
        #region Test: BookmarkIsAdjacentTo
        [Test] public void BookmarkIsAdjacentTo()
        {
            // We'll use the 6th paragraph
            int iPara = 5;
            DParagraph p = EditTest.Section.Paragraphs[iPara] as DParagraph;
            Assert.AreEqual(c_sBenchmark, p.DebugString, "Benchmark: Paragraph contents");
            DBasicText DBT = p.Runs[1] as DBasicText;
            Assert.IsNotNull(DBT, "DBT Found");
            OWPara op = EditTest.Wnd.Contents.FindParagraph(p, OWPara.Flags.None);

            // Select "Oke |te" and bookmark it
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 4);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            OWBookmark bm1 = EditTest.Wnd.CreateBookmark();

            // Create another one: because they are both insertion points, 
            // they should be adjacent
            OWBookmark bm2 = EditTest.Wnd.CreateBookmark();
            Assert.IsTrue(bm1.IsAdjacentTo(bm2), "insertion points");

            // Create one that is a content bookmark: "Oke |te|, |iJesus"
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 4, 6);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            bm2 = EditTest.Wnd.CreateBookmark();
            Assert.IsTrue(bm1.IsAdjacentTo(bm2), "insertion point; selection");

            // Create one that is not adjacent
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 5, 6);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            bm2 = EditTest.Wnd.CreateBookmark();
            Assert.IsFalse(bm1.IsAdjacentTo(bm2), "insertion point; selection; not adjacent");

            // Two adjacent selections
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 3, 5);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            bm1 = EditTest.Wnd.CreateBookmark();
            Assert.IsTrue(bm1.IsAdjacentTo(bm2), "two selections");
            Assert.IsTrue(bm2.IsAdjacentTo(bm1), "two selections");

            // two non-adjacent selections
            EditTest.Wnd.Selection = OWWindow.Sel.CreateSel(op, DBT, 3, 4);
            EditTest.Wnd.Selection = op.NormalizeSelection(EditTest.Wnd.Selection);
            bm1 = EditTest.Wnd.CreateBookmark();
            Assert.IsFalse(bm1.IsAdjacentTo(bm2), "two selections; not adjacent");
            Assert.IsFalse(bm2.IsAdjacentTo(bm1), "two selections; not adjacent");
        }
        #endregion
    }
}
