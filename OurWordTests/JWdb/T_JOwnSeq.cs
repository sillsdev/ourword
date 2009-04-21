/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JOwnSeq.cs
 * Author:  John Wimbish
 * Created: 14 July 2008
 * Purpose: Tests the JOwSeqn implementation
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
using JWdb;

using OurWord;
using JWdb.DataModel;
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
            JOwnSeq<TObject1> ownseq;
            public TestOwnershipMethod()
            {
                ownseq = new JOwnSeq<TObject1>("Ownership", this);

                // Test the AddParagraph method
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
            JOwnSeq<TObject1> ownseq1;
            JOwnSeq<TObject1> ownseq2;
            public OwnershipUniquenessMethod()
            {
                ownseq1 = new JOwnSeq<TObject1>("unique1", this);
                ownseq2 = new JOwnSeq<TObject1>("unique2", this);
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
            JOwnSeq<TObject1> ownseq;
            public ObjectUniquenessMethod()
            {
                ownseq = new JOwnSeq<TObject1>("Unique3", this);

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
            JOwnSeq<TObject1> ownseq;
            public OwnerStoresSelfMethod()
            {
                ownseq = new JOwnSeq<TObject1>("StoresSelf", this);
                Assert.IsTrue(_test_ContainsAttribute(ownseq));
            }
        }
        [Test] public void OwnerStoresSelf()
        {
            new OwnerStoresSelfMethod();
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Test: ReadWrite
        #region Class: OwnSeqTestUnsorted : JObjectOnDemand
        class OwnSeqTestUnsorted : JObjectOnDemand
        {
            public JOwnSeq<TObject> m_os;

			#region Constructor()
			public OwnSeqTestUnsorted()
                : base()
            {
                m_os = new JOwnSeq<TObject>("os", this, true, false);
			}
			#endregion

			#region OAttr{g}: string StoragePath
			public override string StoragePath
			{
				get
				{
					return JWU.NUnit_TestFilePathName;
				}
			}
			#endregion
		}
        #endregion
        #region Class: OwnSeqTestSorted : JObjectOnDemand
        class OwnSeqTestSorted : JObjectOnDemand
        {
            public JOwnSeq<TObject> m_os;

			#region Constructor()
			public OwnSeqTestSorted()
                : base()
            {
                m_os = new JOwnSeq<TObject>("os", this, true, true);
			}
			#endregion

			#region OAttr{g}: string StoragePath
			public override string StoragePath
			{
				get
				{
					return JWU.NUnit_TestFilePathName;
				}
			}
			#endregion
        }
        #endregion
        [Test] public void ReadWrite()
        {
            // Create the object (it's an unsorted sequence)
            OwnSeqTestUnsorted ost1 = new OwnSeqTestUnsorted();
            ost1.m_os.Append(new TObject("Verse"));
            ost1.m_os.Append(new TObject("Chapter"));
            ost1.m_os.Append(new TObject("Inscription"));
            ost1.m_os.Append(new TObject("Lord"));
            ost1.Write(new NullProgress());

            // Read into another object
            OwnSeqTestUnsorted ost2 = new OwnSeqTestUnsorted();
            ost2.Load(new NullProgress());

            // Compare the two
            Assert.AreEqual(4, ost2.m_os.Count);
            Assert.AreEqual(ost1.m_os.Count, ost2.m_os.Count);
            for (int i = 0; i < 4; ++i)
            {
                TObject objW = ost1.m_os[i] as TObject;
                TObject objR = ost2.m_os[i] as TObject;
                Assert.AreEqual(objW.Name, objR.Name);
            }

            // Read it into the Sorted sequence object
            OwnSeqTestSorted oSorted = new OwnSeqTestSorted();
            oSorted.Load(new NullProgress());
            Assert.AreEqual(4, oSorted.m_os.Count);
            // Verify that the sorted sequence is indeed sorted
            for (int i = 0; i < oSorted.m_os.Count - 2; i++)
            {
                TObject obj1 = oSorted.m_os[i] as TObject;
                TObject obj2 = oSorted.m_os[i + 1] as TObject;
                Assert.IsTrue(obj1.SortKey.CompareTo(obj2.SortKey) < 0);
            }

            // Cleanup
            File.Delete(JWU.NUnit_TestFilePathName);
        }
        #endregion

        #region SetUpAnOwningSequence
        static public void SetUpAnOwningSequence(JOwnSeq<TObject> os)
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

        #region OLD MERGE TESTS - keep around in case we re-implement this
        /***
        #region MergeNone
        class MergeNoneMethod : JObject
        {
            JOwnSeq<TObject> os;
            JOwnSeq<TObject> osM;
            public MergeNoneMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence  that we can merge into
                m_bSurpressDuplicateAttrTest = true;
                os = new JOwnSeq<TObject>("test", this, true, false);
                T_JOwnSeq.SetUpAnOwningSequence(os);
                string sPath = sPathName + "mn";

                // Set up a merge OS: Test kNone.
                osM = new JOwnSeq<TObject>("test", this, true, false);
                os.MergeOption = Merge.kNone;
                osM.AddParagraph(new TObject("Fred"));
                osM.AddParagraph(new TObject("Emily"));
                TextWriter tw = JUtil.GetTextWriter(sPath);

                XElement x = new XElement("os");
                osM.ToXml(x);
                x.Out(tw, 0);
//                osM.Write(tw, 0);

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
            JOwnSeq<TObject> os;
            JOwnSeq<TObject> osM;
            public MergeKeepOldMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence  that we can merge into
                os = new JOwnSeq<TObject>("Old", this, true, false);
                T_JOwnSeq.SetUpAnOwningSequence(os);
                foreach (TObject o in os)  // So we can tell the old from the new
                    o.Description = "1";
                string sPath = sPathName + "mko";

                // Set up a merge OS: Test kKeepOld.
                osM = new JOwnSeq<TObject>("OldM", this, true, false);
                osM.AddParagraph(new TObject("David"));
                osM.AddParagraph(new TObject("Emily"));
                TextWriter tw = JUtil.GetTextWriter(sPathName);

                XElement x = new XElement("os");
                osM.ToXml(x);
                x.Out(tw, 0);
//                osM.Write(tw, 0);

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
            JOwnSeq<TObject> os;
            JOwnSeq<TObject> osM;
            public MergeKeepNewMethod()
            {
                string sPathName = JWU.NUnit_TestFilePathName;

                // Set up a unsorted owning sequence  that we can merge into
                m_bSurpressDuplicateAttrTest = true;
                os = new JOwnSeq<TObject>("test", this, true, false);
                T_JOwnSeq.SetUpAnOwningSequence(os);
                foreach (TObject o in os)  // So we can tell the old from the new
                    o.Description = "1";
                string sPath = sPathName + "mkn";

                // Set up a merge OS: Test kKeepNew.
                osM = new JOwnSeq<TObject>("test", this, true, false);
                osM.AddParagraph(new TObject("David"));
                osM.AddParagraph(new TObject("Emily"));
                TextWriter tw = JUtil.GetTextWriter(sPath);

                XElement x = new XElement("os");
                osM.ToXml(x);
                x.Out(tw, 0);
//                osM.Write(tw, 0);

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
        ***/
        #endregion
    }
}
