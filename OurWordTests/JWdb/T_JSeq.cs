/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JOSeq.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the JSeq implementation
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using JWdb;
#endregion

namespace OurWordTests.JWdb
{
    [TestFixture] public class T_JSeq
    {
        #region Supporting Class: TestObj_Name
        class TestObj_Name : JObject
        {
            public string Name;
            public TestObj_Name(string sName)
                : base()
            {
                Name = sName;
            }
            public override bool ContentEquals(JObject obj)
            {
                return (obj as TestObj_Name).Name == Name;
            }
            public override string SortKey
            {
                get
                {
                    return Name;
                }
            }
        }
        #endregion
        #region Supporting Class: TestObj_Container
        class TestObj_Container : JObject
        {
            public JOwnSeq<TestObj_Name> seq;
            public TestObj_Container()
                : base()
            {
                seq = new JOwnSeq<TestObj_Name>("test", this);
                seq.Append(new TestObj_Name("Emily"));
                seq.Append(new TestObj_Name("Robert"));
                seq.Append(new TestObj_Name("David"));
                seq.Append(new TestObj_Name("Christiane"));
            }
        }
        #endregion
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
        }
        #endregion

        // Correct signature of owned objects ------------------------------------------------
        // (These tests will all go away as I make the change to T)
        #region Supporting Classes for signature of owned objects
        class ObjTypeA : JObject
        {
            public JOwnSeq<ObjTypeA> seq;
            public ObjTypeA()
                : base()
            {
                seq = new JOwnSeq<ObjTypeA>("test", this);
            }
        }
        class ObjTypeB : JObject
        {
        }
        #endregion
        #region Test: SignatureControl_Append
        [Test]
        [ExpectedException(typeof(eBadSignature))]
        public void SignatureControl_Append()
        // We declare a JOwnSeq with a signature of one type, and then attempt to
        // add an object to it of a different type. We expect an exception. If we check
        // anytime an object is being added, then we prevent the sequence from ever
        // having bad objects.
        {
            ObjTypeA objA = new ObjTypeA();
            objA.seq.Append( new ObjTypeB());
        }
        #endregion
        #region Test: SignatureControl_InsertAt
        [Test]
        [ExpectedException(typeof(eBadSignature))]
        public void SignatureControl_InsertAt()
        {
            ObjTypeA objA = new ObjTypeA();
            objA.seq.InsertAt(0, new ObjTypeB());
        }
        #endregion

        // Enumerator ------------------------------------------------------------------------
        #region Test: EnumeratorBasics
        [Test] public void EnumeratorBasics()
            // Purpose: See that we can enumerate sequentially through a set of objects.
            //
            // Test:
            // 1. Create our standard ordered sequence containing four objects
            // 2. Using the foreach command, see that the four objects appear
            //     in the enumeration in the order expected.
            // 3. Run through the foreach command again, to make sure Reset works correctly.
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Make sure that the foreach statement returns the elements in the order we expect
            int i = 0;
            foreach (TestObj_Name obj in TestObj.seq)
            {
                if (i == 0)
                    Assert.IsTrue(obj.Name == "Emily");
                if (i == 1)
                    Assert.IsTrue(obj.Name == "Robert");
                if (i == 2)
                    Assert.IsTrue(obj.Name == "David");
                if (i == 3)
                    Assert.IsTrue(obj.Name == "Christiane");
                ++i;
            }

            // Make sure the enumerator resets correctly for a second pass
            i = 0;
            foreach (TestObj_Name obj in TestObj.seq)
            {
                if (i == 2)
                    Assert.IsTrue(obj.Name == "David");
            }
        }
        #endregion
        #region Test: EnumeratorIllegalAppend
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumeratorIllegalAppend()
            // Purpose: One an object has been appended to the list, it is illegal to
            // continue with the enumeration. 
            //
            // Test:
            // 1. Create our standard ordered sequence containing four objects
            // 2. While in the midst of the foreach, add a fifth object
            // 3. The next call in the foreach should thrown an exception.
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Adding an object in the midst of a foreach should be illegal
            int i = 0;
            foreach (TestObj_Name o in TestObj.seq)
            {
                if (i++ == 2)
                {
                    TestObj.seq.Append(new TestObj_Name("New"));
                }
            }
        }
        #endregion
        #region Test: EnumeratorIllegalInsertAt
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumeratorIllegalInsertAt()
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Adding an object in the midst of a foreach should be illegal
            int i = 0;
            foreach (TestObj_Name o in TestObj.seq)
            {
                if (i++ == 2)
                {
                    TestObj.seq.InsertAt(1, new TestObj_Name("New"));
                }
            }
        }
        #endregion
        #region Test: EnumeratorIllegalIndexerSet
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumeratorIllegalIndexerSet()
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Inserting an object via the indexer in the midst of a foreach should be illegal
            int i = 0;
            foreach (TestObj_Name o in TestObj.seq)
            {
                if (i++ == 2)
                {
                    TestObj.seq[0] = new TestObj_Name("New");
                }
            }
        }
        #endregion
        #region Test: EnumeratorIllegalRemove
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumeratorIllegalRemove()
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Removing an object via the indexer in the midst of a foreach should be illegal
            int i = 0;
            foreach (TestObj_Name o in TestObj.seq)
            {
                if (i++ == 2)
                {
                    TestObj.seq.Remove(o);
                }
            }
        }
        #endregion
        #region Test: EnumeratorIllegalRemoveAt
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumeratorIllegalRemoveAt()
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Removing an object via the indexer in the midst of a foreach should be illegal
            int i = 0;
            foreach (TestObj_Name o in TestObj.seq)
            {
                if (i++ == 2)
                {
                    TestObj.seq.RemoveAt(2);
                }
            }
        }
        #endregion
        #region Test: EnumeratorIllegalClear
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumeratorIllegalClear()
        {
            TestObj_Container TestObj = new TestObj_Container();

            // Clearing the list in the midst of a foreach should be illegal
            int i = 0;
            foreach (TestObj_Name o in TestObj.seq)
            {
                if (i++ == 2)
                {
                    TestObj.seq.Clear();
                }
            }
        }
        #endregion

        // Duplicates and sorting ------------------------------------------------------------
        #region Test: AvoidDupplicatesAppend
        [Test]
        [ExpectedException(typeof(eContentDuplication))]
        public void AvoidDupplicatesAppend()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.AvoidDuplicates = true;
            container.seq.ComplainIfDuplicateAttempted = true;
            container.seq.Append(new TestObj_Name("David"));
        }
        #endregion
        #region Test: AvoidDupplicatesInsertAt
        [Test]
        [ExpectedException(typeof(eContentDuplication))]
        public void AvoidDupplicatesInsertAt()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.AvoidDuplicates = true;
            container.seq.ComplainIfDuplicateAttempted = true;
            container.seq.InsertAt(2, new TestObj_Name("David"));
        }
        #endregion
        #region Test: AvoidDupplicatesIndexerSet
        [Test]
        [ExpectedException(typeof(eContentDuplication))]
        public void AvoidDupplicatesIndexerSet()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.AvoidDuplicates = true;
            container.seq.ComplainIfDuplicateAttempted = true;
            container.seq[2] = new TestObj_Name("David");
        }
        #endregion
        #region Test: FindSorted
        [Test] public void FindSorted()
            // Purpose: Test that the a sorted list, even with randomly-ordered additions,
            // results in objects being in their correct sort order.
        {
            TestObj_Container container = new TestObj_Container();

            // Setup unsorted sequence
            container.seq.Clear();
            container.seq.Append(new TestObj_Name("Emily"));
            container.seq.Append(new TestObj_Name("Robert"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("Christiane"));

            // Switch it to be sorted
            container.seq.IsSorted = true;

            // Can we find everything?
            Assert.IsTrue(container.seq.Find("Christiane") == 0);
            Assert.IsTrue(container.seq.Find("David") == 1);
            Assert.IsTrue(container.seq.Find("Emily") == 2);
            Assert.IsTrue(container.seq.Find("Robert") == 3);

            // Do we fail on things not there?
            Assert.IsTrue(container.seq.Find("A") == -1);
            Assert.IsTrue(container.seq.Find("Da") == -1);
            Assert.IsTrue(container.seq.Find("Emil") == -1);
            Assert.IsTrue(container.seq.Find("Emily Who") == -1);
            Assert.IsTrue(container.seq.Find("Fred") == -1);
        }
        #endregion
        #region Test: FindUnsorted
        [Test]
        public void FindUnsorted()
        {
            TestObj_Container container = new TestObj_Container();

            // Setup unsorted sequence
            container.seq.Clear();
            container.seq.Append(new TestObj_Name("Emily"));
            container.seq.Append(new TestObj_Name("Robert"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("Christiane"));

            // Can we find everything?
            Assert.IsTrue(container.seq.Find("Emily") == 0);
            Assert.IsTrue(container.seq.Find("Robert") == 1);
            Assert.IsTrue(container.seq.Find("David") == 2);
            Assert.IsTrue(container.seq.Find("Christiane") == 3);

            // Do we fail on things not there?
            Assert.IsTrue(container.seq.Find("A") == -1);
            Assert.IsTrue(container.seq.Find("Da") == -1);
            Assert.IsTrue(container.seq.Find("Emil") == -1);
            Assert.IsTrue(container.seq.Find("Emily Who") == -1);
            Assert.IsTrue(container.seq.Find("Fred") == -1);
            Assert.IsTrue(container.seq.Find("Z") == -1);
        }
        #endregion
        #region Test: FindInsertPosition
        [Test]
        public void FindInsertPosition()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.IsSorted = true;

            // Can we find everything?
            Assert.IsTrue(container.seq._FindInsertionPosition("Christiane") == 1);
            Assert.IsTrue(container.seq._FindInsertionPosition("David") == 2);
            Assert.IsTrue(container.seq._FindInsertionPosition("Emily") == 3);
            Assert.IsTrue(container.seq._FindInsertionPosition("Robert") == 4);

            // Do we fail on things not there?
            Assert.IsTrue(container.seq._FindInsertionPosition("A") == 0);
            Assert.IsTrue(container.seq._FindInsertionPosition("Da") == 1);
            Assert.IsTrue(container.seq._FindInsertionPosition("Emil") == 2);
            Assert.IsTrue(container.seq._FindInsertionPosition("Emily Who") == 3);
            Assert.IsTrue(container.seq._FindInsertionPosition("Fred") == 3);
        }
        #endregion
        #region Test: SortedInsert
        [Test]
        public void SortedInsert()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.IsSorted = true;

            // Check the Append method
            TestObj_Name objJohn = new TestObj_Name("John");
            container.seq.Append(objJohn);
            Assert.IsTrue(container.seq[3] == objJohn);

            // Check the InsertAt method
            TestObj_Name objSandra = new TestObj_Name("Sandra");
            container.seq.InsertAt(1, objSandra);
            Assert.IsTrue(container.seq[5] == objSandra);	
        }
        #endregion
        #region Test: SortedList_CantIndexer
        [Test]
        [ExpectedException(typeof(eSortedSequence))]
        public void SortedList_CantIndexer()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.IsSorted = true;

            container.seq[2] = new TestObj_Name("hi");
        }
        #endregion
        #region Test: SwitchSorting
        [Test] public void SwitchSorting()
        {
            TestObj_Container container = new TestObj_Container();
            container.seq.IsSorted = true;

			// Turn sorting off and insert a record
			container.seq.IsSorted = false;
			TestObj_Name obj = new TestObj_Name("John");
			container.seq.InsertAt(1, obj);
			Assert.IsTrue( container.seq[1] == obj);

			// Now turn on sorting and verify that the object has moved to its proper position
			container.seq.IsSorted = true;
			Assert.IsTrue( container.seq[3] == obj);
        }
        #endregion
        #region Test: DuplicateFinds
        [Test] public void DuplicateFinds()
        {
            // Add a bunch of duplicates
            TestObj_Container container = new TestObj_Container();
            container.seq.IsSorted = true;
            container.seq.AvoidDuplicates = false;
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            Assert.AreEqual(1, container.seq.Find("David"));
            Assert.AreEqual(8, container.seq.FindAll("David").Count);

            // Repeat, this time unsorted
            container = new TestObj_Container();
            container.seq.IsSorted = false;
            container.seq.AvoidDuplicates = false;
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            container.seq.Append(new TestObj_Name("David"));
            Assert.AreEqual(2, container.seq.Find("David"));
            Assert.AreEqual(6, container.seq.FindAll("David").Count);
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Test: ContentEquals
        [Test] public void ContentEquals()
        {
            TestObj_Container container1 = new TestObj_Container();
            TestObj_Container container2 = new TestObj_Container();

            // Sequences should be the same
            Assert.IsTrue(container1.seq.Count > 2, "make sure we have a non-zero length sequence");
            Assert.IsTrue(container1.seq.ContentEquals(container2.seq), "should be same");

            // Add something to container 2; they should now be unequal
            container2.seq.Append(new TestObj_Name("John"));
            Assert.IsFalse(container1.seq.ContentEquals(container2.seq), "should be different");
        }
        #endregion
    }
}
