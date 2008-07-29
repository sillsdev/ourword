/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JOwnSeq.cs
 * Author:  John Wimbish
 * Created: 14 July 2008
 * Purpose: Tests the JOwSeqn implementation
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

using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.JWdb
{
    [TestFixture] public class T_JOwnSeq
    {
        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
        }
        #endregion

        // Ownership -------------------------------------------------------------------------
        #region Test_Ownership
        class TestOwnershipMethod : JObject
        {
            JOwnSeq ownseq;
            public TestOwnershipMethod()
            {
                ownseq = new JOwnSeq("Ownership", this, typeof(TObject1));

                // Test the Append method
                TObject1 objA = new TObject1("orange");
                ownseq.Append(objA);
                Assert.IsTrue(objA.Owner == this);

                // Test the InsertAt method
                TObject1 objB = new TObject1("yello");
                ownseq.InsertAt(0, objB);
                Assert.IsTrue(objB.Owner == this);

                // Test the indexer
                TObject1 objC = new TObject1("red");
                ownseq[0] = objC;
                Assert.IsTrue(objC.Owner == this);

                // Test that RemoveAt sets owner back to null. 
                ownseq.RemoveAt(0);
                Assert.IsTrue(objC.Owner == null);

                // When we replaced objB with objC via the indexer, objB should now have a null owner
                Assert.IsTrue(objB.Owner == null);

                // When we clear out the list, ObjA should have no owner
                ownseq.Clear();
                Assert.IsTrue(objA.Owner == null);

                // To test Remove, add the objects all back in, them remove one of them.
                ownseq.Append(objA);
                ownseq.Append(objB);
                ownseq.Append(objC);
                Assert.IsTrue(objB.Owner == this);
                ownseq.Remove(objB);
                Assert.IsTrue(objB.Owner == null);
            }
        }
        [Test] public void Ownership()
        {
            new TestOwnershipMethod();
        }
        #endregion
        #region OwnershipUniqueness
        class OwnershipUniquenessMethod : JObject
        {
            JOwnSeq ownseq1;
            JOwnSeq ownseq2;
            public OwnershipUniquenessMethod()
            {
                ownseq1 = new JOwnSeq("unique1", this, typeof(TObject1));
                ownseq2 = new JOwnSeq("unique2", this, typeof(TObject1));
                TObject1 obj = new TObject1("hello");
                ownseq1.Append(obj);
                ownseq2.Append(obj); // should fail, because obj is already owned by ownseq1.
            }
        }
        [Test]
        [ExpectedException(typeof(eAlreadyOwned))]
        public void OwnershipUniqueness()
        {
            new OwnershipUniquenessMethod();
        }
        #endregion
        #region ObjectUniqueness
        class ObjectUniquenessMethod : JObject
        {
            JOwnSeq ownseq;
            public ObjectUniquenessMethod()
            {
                ownseq = new JOwnSeq("Unique3", this, typeof(TObject1));

                // We're testing for referential duplicates, not content duplicates, so we
                // must turn off this code.
                ownseq.AvoidDuplicates = false;

                // Attempt to append the same object twice.
                TObject1 obj = new TObject1("hello");
                ownseq.Append(obj);
                ownseq.Append(obj); // should fail, because obj is already owned by ownseq1.
            }
        }
        [Test]
        [ExpectedException(typeof(eAlreadyOwned))]
        public void ObjectUniqueness()
        // The code that protects against ownership uniquess also prevents the same
        // object from appearing more than once in the sequence. So here we just
        // have a test that attempts to place it twice in the sequence, and we
        // test to make sure the exception from OwnershipUniqueness is fired.
        {
            new ObjectUniquenessMethod();
        }
        #endregion
        #region OwnerStoresSelf
        class OwnerStoresSelfMethod : JObject
        {
            JOwnSeq ownseq;
            public OwnerStoresSelfMethod()
            {
                ownseq = new JOwnSeq("StoresSelf", this, typeof(TObject1));
                Assert.IsTrue(_test_ContainsAttribute(ownseq));
            }
        }
        [Test] public void OwnerStoresSelf()
        {
            new OwnerStoresSelfMethod();
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region ReadWrite
        class ReadWriteMethod : JObject
        {
            JOwnSeq ownseqW;
            JOwnSeq ownseqR;
            JOwnSeq ownseqRS;
            public ReadWriteMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence
                m_bSurpressDuplicateAttrTest = true;
                ownseqW = new JOwnSeq("test", this, typeof(TObject));
                ownseqW.Append(new TObject("Verse"));
                ownseqW.Append(new TObject("Chapter"));
                ownseqW.Append(new TObject("Inscription"));
                ownseqW.Append(new TObject("Lord"));

                // Write it out
                TextWriter tw = JUtil.GetTextWriter(sPathName);
                ownseqW.Write(tw, 0);
                tw.Close();

                // Read in to another unsorted sequence
                ownseqR = new JOwnSeq("test", this, typeof(TObject));
                TextReader tr = JUtil.GetTextReader(sPathName);
                string sLine;
                while ((sLine = tr.ReadLine()) != null)
                {
                    ownseqR.Read(sLine, tr);
                }
                tr.Close();

                // Compare the two
                Assert.AreEqual(4, ownseqR.Count);
                Assert.AreEqual(ownseqW.Count, ownseqR.Count);
                for (int i = 0; i < 4; ++i)
                {
                    TObject objW = ownseqW[i] as TObject;
                    TObject objR = ownseqR[i] as TObject;
                    Assert.AreEqual(objW.Name, objR.Name);
                }

                // Create an empty sorted sequence
                ownseqRS = new JOwnSeq("test", this, typeof(TObject), false, true);
                tr = JUtil.GetTextReader(sPathName);
                while ((sLine = tr.ReadLine()) != null)
                {
                    ownseqRS.Read(sLine, tr);
                }
                tr.Close();

                // Verify that the sorted sequence is indeed sorted
                for (int i = 0; i < ownseqRS.Count - 2; i++)
                {
                    TObject obj1 = ownseqRS[i] as TObject;
                    TObject obj2 = ownseqRS[i + 1] as TObject;
                    Assert.IsTrue(obj1.SortKey.CompareTo(obj2.SortKey) < 0);
                }

                // Cleanup
                File.Delete(sPathName);
            }
        }
        [Test] public void ReadWrite()
        {
            new ReadWriteMethod();
        }
        #endregion
        #region SetUpAnOwningSequence
        static public void SetUpAnOwningSequence(JOwnSeq os)
        // If anything is changed here, I need to also change the FindUnsorted test.
        {
            os.Clear();
            TObject objA = new TObject("Emily");
            TObject objB = new TObject("David");
            TObject objC = new TObject("Robert");
            TObject objD = new TObject("Christiane");
            os.Append(objA);
            os.Append(objB);
            os.Append(objC);
            os.Append(objD);
        }
        #endregion
        #region MergeNone
        class MergeNoneMethod : JObject
        {
            JOwnSeq os;
            JOwnSeq osM;
            public MergeNoneMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence  that we can merge into
                m_bSurpressDuplicateAttrTest = true;
                os = new JOwnSeq("test", this, typeof(TObject), true, false);
                T_JOwnSeq.SetUpAnOwningSequence(os);
                string sPath = sPathName + "mn";

                // Set up a merge OS: Test kNone.
                osM = new JOwnSeq("test", this, typeof(TObject), true, false);
                os.MergeOption = Merge.kNone;
                osM.Append(new TObject("Fred"));
                osM.Append(new TObject("Emily"));
                TextWriter tw = JUtil.GetTextWriter(sPath);
                osM.Write(tw, 0);
                tw.Close();

                // Do the merge.
                TextReader tr = JUtil.GetTextReader(sPath);
                string sLine;
                while ((sLine = tr.ReadLine()) != null)
                {
                    os.Read(sLine, tr);
                }
                tr.Close();
                File.Delete(sPath);
                Assert.AreEqual(2, os.Count);
            }
        }
        [Test] public void MergeNone()
        // Creates a sequence with several members, writes it,then reads
        // in a different sequence. Tests with MergeOption = kNone,
        // which should result in the first sequence being discarded
        // and the second one read replacing it.
        {
            new MergeNoneMethod();
        }
        #endregion
        #region MergeKeepOld
        class MergeKeepOldMethod : JObject
        {
            JOwnSeq os;
            JOwnSeq osM;
            public MergeKeepOldMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence  that we can merge into
                os = new JOwnSeq("Old", this, typeof(TObject), true, false);
                T_JOwnSeq.SetUpAnOwningSequence(os);
                foreach (TObject o in os)  // So we can tell the old from the new
                    o.Description = "1";
                string sPath = sPathName + "mko";

                // Set up a merge OS: Test kKeepOld.
                osM = new JOwnSeq("OldM", this, typeof(TObject), true, false);
                osM.Append(new TObject("David"));
                osM.Append(new TObject("Emily"));
                TextWriter tw = JUtil.GetTextWriter(sPathName);
                osM.Write(tw, 0);
                tw.Close();

                // Do the merge.
                os.MergeOption = Merge.kKeepOld;
                TextReader tr = JUtil.GetTextReader(sPathName);
                string sLine;
                while ((sLine = tr.ReadLine()) != null)
                {
                    os.Read(sLine, tr);
                }
                tr.Close();
                File.Delete(sPath);
                Assert.AreEqual(4, os.Count);
                foreach (TObject o in os)
                {
                    Assert.AreEqual("1", o.Description);
                }
            }
        }
        [Test] public void MergeKeepOld()
        // Creates a sequence with several members, writes it,then reads
        // in a different sequence. Tests with MergeOption = kKeepOld,
        // which should result in the first sequence being nleft intact,
        // which is evidenced by m_n == 1 rather than the default of 0.
        {
            new MergeKeepOldMethod();
        }
        #endregion
        #region MergeKeepNew
        class MergeKeepNewMethod : JObject
        {
            JOwnSeq os;
            JOwnSeq osM;
            public MergeKeepNewMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence  that we can merge into
                m_bSurpressDuplicateAttrTest = true;
                os = new JOwnSeq("test", this, typeof(TObject), true, false);
                T_JOwnSeq.SetUpAnOwningSequence(os);
                foreach (TObject o in os)  // So we can tell the old from the new
                    o.Description = "1";
                string sPath = sPathName + "mkn";

                // Set up a merge OS: Test kKeepNew.
                osM = new JOwnSeq("test", this, typeof(TObject), true, false);
                osM.Append(new TObject("David"));
                osM.Append(new TObject("Emily"));
                TextWriter tw = JUtil.GetTextWriter(sPath);
                osM.Write(tw, 0);
                tw.Close();

                // Do the merge.
                os.MergeOption = Merge.kKeepNew;
                TextReader tr = JUtil.GetTextReader(sPath);
                string sLine;
                while ((sLine = tr.ReadLine()) != null)
                {
                    os.Read(sLine, tr);
                }
                tr.Close();
                File.Delete(sPath);
                Assert.AreEqual(4, os.Count);
                Assert.AreEqual("", ((TObject)os[os.Find("David")]).Description);
                Assert.AreEqual("", ((TObject)os[os.Find("Emily")]).Description);
            }
        }
        [Test] public void MergeKeepNew()
        // Creates a sequence with several members, writes it,then reads
        // in a different sequence. Tests with MergeOption = kKeepOld,
        // which should result in the first sequence being nleft intact,
        // which is evidenced by m_n == 1 rather than the default of 0.
        {
            new MergeKeepNewMethod();
        }
        #endregion
    }
}
