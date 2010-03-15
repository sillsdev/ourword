/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_UndoRedo.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the UndoRedo class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using NUnit.Framework;
using OurWord.Edit;
using OurWordTests.DataModel;

#endregion

namespace OurWordTests.Edit
{
    [TestFixture] public class T_UndoRedo
    {
        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            EditTest.Setup(SectionTestData.BaikenoMark430_ImportVariant);

            TestCommon.GlobalTestSetup();
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            EditTest.TearDown();
        }
        #endregion

        // Stack Operations ------------------------------------------------------------------
        #region Test: TestStackOperations
        [Test] public void TestStackOperations()
        {
            string sTyping = "Typing 'abc'";
            string sBackspace = "Backspace";
            string sJoin = "Join Paragraphs";
            string sDelete = "Delete";

            UndoRedoStack stack = new UndoRedoStack(0, null, null);

            Assert.IsTrue(!stack.HasUndo, "No Undo in stack - a");
            Assert.IsTrue(!stack.HasRedo, "No Redo in stack - a");

            // Add a few items to the stack
            stack.Push(new Action(EditTest.Wnd, sTyping));
            stack.Push(new Action(EditTest.Wnd, sBackspace));
            stack.Push(new Action(EditTest.Wnd, sJoin));
            Assert.IsTrue(stack.HasUndo, "Has Undo in stack - b");
            Assert.IsTrue(!stack.HasRedo, "No Redo in stack - b");

            // Pop an item: should be the last one entered
            Action action = stack.Undo();
            Assert.AreEqual(sJoin, action.DisplayName);
            Assert.IsTrue(stack.HasUndo, "Has Undo in stack - c");
            Assert.IsTrue(stack.HasRedo, "Has Redo in stack - c");

            // Peek should retrieve, but not change, the stack
            action = stack.PeekUndo;
            Assert.AreEqual(sBackspace, action.DisplayName);
            action = stack.PeekRedo;
            Assert.AreEqual(sJoin, action.DisplayName);

            // Pop another item from Undo
            action = stack.Undo();
            Assert.AreEqual(sBackspace, action.DisplayName);
            Assert.IsTrue(stack.HasUndo, "Has Undo in stack - d");
            Assert.IsTrue(stack.HasRedo, "Has Redo in stack - d");

            // Is Peek what we expect?
            action = stack.PeekUndo;
            Assert.AreEqual(sTyping, action.DisplayName);
            action = stack.PeekRedo;
            Assert.AreEqual(sBackspace, action.DisplayName);

            // Pop item from Redo: should be the Backspace action again
            action = stack.Redo();
            Assert.AreEqual(sBackspace, action.DisplayName);
            Assert.IsTrue(stack.HasUndo, "Has Undo in stack - e");
            Assert.IsTrue(stack.HasRedo, "Has Redo in stack - e");

            // Add a new item; should clear ReDo 
            stack.Push(new Action(EditTest.Wnd, sDelete));
            Assert.IsTrue(stack.HasUndo, "Has Undo in stack - f");
            Assert.IsTrue(!stack.HasRedo, "No Redo in stack - f");
        }
        #endregion
        #region Test: TestStackMaxDepth
        [Test] public void TestStackMaxDepth()
        {
            // Create a stack of infinite depth
            UndoRedoStack stack = new UndoRedoStack(0, null, null);
            Assert.IsTrue(stack.AllowsInfiniteDepth, "Allows Infinite Depth");

            // Create a stack of liminted depth
            stack = new UndoRedoStack(3, null, null);
            Assert.IsTrue(!stack.AllowsInfiniteDepth, "Doesn't allow Infinite Depth");

            // Add four entries
            string sTyping = "Typing 'abc'";
            string sSplit = "Split Paragraph";
            string sBackspace = "Backspace";
            string sJoin = "Join Paragraphs";
            string sDelete = "Delete";
            stack.Push(new Action(EditTest.Wnd, sTyping));
            stack.Push(new Action(EditTest.Wnd, sSplit));
            stack.Push(new Action(EditTest.Wnd, sBackspace));
            stack.Push(new Action(EditTest.Wnd, sJoin));
            stack.Push(new Action(EditTest.Wnd, sDelete));

            // We should have only the last three entries
            Assert.AreEqual(sDelete, stack.Undo().DisplayName);
            Assert.AreEqual(sJoin, stack.Undo().DisplayName);
            Assert.AreEqual(sBackspace, stack.Undo().DisplayName);
            Assert.IsTrue(!stack.HasUndo, "Undo should now be clear");
        }
        #endregion
    }
}
