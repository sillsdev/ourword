/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JObject.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the JObject implementation
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
#endregion

namespace OurWordTests.JWdb
{
    [TestFixture] public class T_JObject
    {
        #region Embedded Class for Testing: TestObject
        class TestObject : JObject
        {
            // ZAttrs ------------------------------------------------------------------------
            #region BAttr{g/s}: string Name
            public string Name
            {
                get
                {
                    return m_sName;
                }
                set
                {
                    m_sName = value;
                }
            }
            private string m_sName;
            #endregion
            #region Method void DeclareAttrs()
            protected override void DeclareAttrs()
            {
                base.DeclareAttrs();
                DefineAttr("Name", ref m_sName);
            }
            #endregion

            #region Attr{g}: JOwnSeq OwnSeq1
            public JOwnSeq<TestObject> OwnSeq1
            {
                get
                {
                    return m_osOwnSeq1;
                }
            }
            JOwnSeq<TestObject> m_osOwnSeq1;
            #endregion
            #region Attr{g}: JOwnSeq OwnSeq2
            public JOwnSeq<TestObject> OwnSeq2
            {
                get
                {
                    return m_osOwnSeq2;
                }
            }
            JOwnSeq<TestObject> m_osOwnSeq2;
            #endregion
            #region Attr{g}: JOwnSeq OwnSeq3
            public JOwnSeq<TestObject> OwnSeq3
            {
                get
                {
                    return m_osOwnSeq3;
                }
            }
            JOwnSeq<TestObject> m_osOwnSeq3;
            #endregion

            #region Attr{g}: JOwn Own1
            public JOwn<TestObject> Own1
            {
                get
                {
                    return m_own1;
                }
            }
            JOwn<TestObject> m_own1;
            #endregion
            #region Attr{g}: JOwn Own2
            public JOwn<TestObject> Own2
            {
                get
                {
                    return m_own2;
                }
            }
            JOwn<TestObject> m_own2;
            #endregion

            #region Attr{g}: JRef Ref1
            public JRef<TestObject> Ref1
            {
                get
                {
                    return m_ref1;
                }
            }
            JRef<TestObject> m_ref1;
            #endregion
            #region Attr{g}: JRef Ref2
            public JRef<TestObject> Ref2
            {
                get
                {
                    return m_ref2;
                }
            }
            JRef<TestObject> m_ref2;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(sName)
            public TestObject(string sName)
                : base()
            {
                m_sName = sName;

                m_osOwnSeq1 = new JOwnSeq<TestObject>("os1", this);
                m_osOwnSeq2 = new JOwnSeq<TestObject>("os2", this);
                m_osOwnSeq3 = new JOwnSeq<TestObject>("os3", this);

                m_own1 = new JOwn<TestObject>("own1", this);
                m_own2 = new JOwn<TestObject>("own2", this);

                m_ref1 = new JRef<TestObject>("ref1", this);
                m_ref2 = new JRef<TestObject>("ref2", this);
            }
            #endregion
            #region OAttr{g}: string SortKey
            public override string SortKey
            {
                get
                {
                    return Name;
                }
            }
            #endregion
        }
        #endregion

        // Setup/TearDown --------------------------------------------------------------------
        TestObject objRoot;
        TestObject objLevel2;
        TestObject objLevel3;
        TestObject objLevel4;
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();

            // Create objects in a hierarchy
            objRoot = new TestObject("Level1");
            objLevel2 = new TestObject("Level2");
            objLevel3 = new TestObject("Level3");
            objLevel4 = new TestObject("Level4");
            objRoot.OwnSeq1.Append(objLevel2);
            objLevel2.OwnSeq1.Append(objLevel3);
            objLevel3.OwnSeq1.Append(objLevel4);
        }
        #endregion

        #region Test: AllOwningObjects
        [Test] public void AllOwningObjects()
        {
            // objRoot should not have an owner
            ArrayList list = objRoot.AllOwners;
            Assert.AreEqual(0, list.Count);

            // objLevel2 should have one owner, objRoot.
            list = objLevel2.AllOwners;
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] == objRoot);

            // objLevel4 should have three owners, in order, Root-1-2.
            list = objLevel4.AllOwners;
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list[0] == objRoot);
            Assert.IsTrue(list[1] == objLevel2);
            Assert.IsTrue(list[2] == objLevel3);
        }
        #endregion
        #region Test: RootOwner
        [Test] public void RootOwner()
            // Test that the ownership hierarchy correctly identifies the root owner
        {
            Assert.IsTrue(objRoot == objLevel2.RootOwner);
            Assert.IsTrue(objRoot == objLevel3.RootOwner);
            Assert.IsTrue(objRoot == objLevel4.RootOwner);

            Assert.IsTrue(objRoot.IsRoot);
            Assert.IsFalse(objLevel2.IsRoot);
            Assert.IsFalse(objLevel3.IsRoot);
            Assert.IsFalse(objLevel4.IsRoot);
        }
        #endregion
        #region Test: PathFromOwningObject
        [Test] public void PathFromOwningObject()
        {
            // Ownership path via JOwnSeq's
            string s = objLevel4.GetPathFromRoot();
            Assert.AreEqual("-os1-0-os1-0-os1-0", s);

            // Partial ownership path
            s = objLevel4.GetPathFromOwningObject(objLevel2);
            Assert.AreEqual("-os1-0-os1-0", s);

            // Via JOwn own1
            TestObject objOwned = new TestObject("owned");
            objLevel3.Own1.Value = objOwned;
            s = objOwned.GetPathFromOwningObject(objRoot);
            Assert.AreEqual("-os1-0-os1-0-own1", s);
        }
        #endregion
        #region Test: ObjectFromPath
        [Test] public void ObjectFromPath()
        {
            // We'll use an object that has both JOwn and JOwnSeq's in its path.
            string sPath = objLevel4.GetPathFromRoot();

            // Get the object; should be objLevel4
            JObject obj = objRoot.GetObjectFromPath(sPath);
            Assert.IsNotNull(obj);
            Assert.IsTrue(objLevel4 == obj);
        }
        #endregion
        #region Test: NoDuplicateAttrNames

        #region Embedded test class: TestObjectDups
        class TestObjectDups : TestObject
        {
            #region Attr{g}: JOwnSeq OwnSeqDup
            public JOwnSeq<TestObject> OwnSeqDup
            {
                get
                {
                    return m_osOwnSeqDup;
                }
            }
            JOwnSeq<TestObject> m_osOwnSeqDup;
            #endregion
            #region Constructor()
            public TestObjectDups()
                : base("dup")
            {
                m_osOwnSeqDup = new JOwnSeq<TestObject>("os2", this);
            }
            #endregion
        }
        #endregion

        [Test]
        [ExpectedException(typeof(eDuplicateAttrName))]
        public void NoDuplicateAttrNames()
        {
            TestObjectDups notPermitted = new TestObjectDups();
        }
        #endregion

        #region Test: SimpleBAttrIO
        #region Embedded Class for Testing: BAttrTestObject
        class BAttrTestObject : JObjectOnDemand
        {
            #region BAttr{g/s}: string Name
            public string Name
            {
                get
                {
                    return m_sName;
                }
                set
                {
                    m_sName = value;
                }
            }
            private string m_sName;
            #endregion
            #region BAttr{g/s}: bool IsYoung
            public bool IsYoung
            {
                get
                {
                    return m_bIsYoung;
                }
                set
                {
                    SetValue(ref m_bIsYoung, value);
                }
            }
            private bool m_bIsYoung = false;
            #endregion
            #region BAttr{g/s}: int Age
            public int Age
            {
                get
                {
                    return m_nAge;
                }
                set
                {
                    SetValue(ref m_nAge, value);
                }
            }
            private int m_nAge;
            #endregion
            #region BAttr{g/s}: double Weight
            public double Weight
            {
                get
                {
                    return m_dWeight;
                }
                set
                {
                    SetValue(ref m_dWeight, value);
                }
            }
            private double m_dWeight;
            #endregion
            #region BAttr{g/s}: DateTime DOB
            public DateTime DOB
            {
                get
                {
                    return m_dtDOB;
                }
                set
                {
                    SetValue(ref m_dtDOB, value);
                }
            }
            private DateTime m_dtDOB;
            #endregion
            #region BAttr{g/s}: char Gender
            public char Gender
            {
                get
                {
                    return m_chGender;
                }
                set
                {
                    SetValue(ref m_chGender, value);
                }
            }
            private char m_chGender;
            #endregion

            #region Method void DeclareAttrs()
            protected override void DeclareAttrs()
            {
                base.DeclareAttrs();
                DefineAttr("Name", ref m_sName);
                DefineAttr("Young", ref m_bIsYoung);
                DefineAttr("Age", ref m_nAge);
                DefineAttr("Weight", ref m_dWeight);
                DefineAttr("DOB", ref m_dtDOB);
                DefineAttr("Gender", ref m_chGender);
            }
            #endregion

            #region Constructor()
            public BAttrTestObject()
                : base()
            {
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

        [Test] public void SimpleBAttrIO()
        {
            // Create an object and populate attributes for each of our data types
            BAttrTestObject objStart = new BAttrTestObject();
            objStart.Name = "John";
            objStart.IsYoung = true;
            objStart.Age = 48;
            objStart.Weight = 174.2;
            objStart.DOB = new DateTime(1959, 11, 23, 23, 59, 00);
            objStart.Gender = 'M';

            // Verify the attributes were set as I expected them to be
            Assert.AreEqual("John", objStart.Name, "Set");
            Assert.AreEqual(true, objStart.IsYoung, "Set");
            Assert.AreEqual(48, objStart.Age, "Set");
            Assert.AreEqual(174.2, objStart.Weight, "Set");
            Assert.AreEqual(new DateTime(1959, 11, 23, 23, 59, 00), objStart.DOB, "Set");
            Assert.AreEqual('M', objStart.Gender, "Set");

            // Write out the object
            objStart.Write(new NullProgress());

            // Create a read-in object and populate it from the file we just wrote out
            BAttrTestObject objEnd = new BAttrTestObject();
            objEnd.Load(new NullProgress());

            // Test that the attributes are all the same
            Assert.AreEqual(objStart.Name, objEnd.Name, "Read-Name");
            Assert.AreEqual(objStart.IsYoung, objEnd.IsYoung, "Read-IsYoung");
            Assert.AreEqual(objStart.Age, objEnd.Age, "Read-Age");
            Assert.AreEqual(objStart.Weight, objEnd.Weight, "Read-Weight");
            Assert.AreEqual(objStart.DOB, objEnd.DOB, "Read-DOB");
            Assert.AreEqual(objStart.Gender, objEnd.Gender, "Read-Gender");
        }
        #endregion

        // Merging
        #region Test: MergeBasicAttrs
        #region Class TMerge - test class for MergeBasicAttrs
        class TMerge : JObject
        {
            public string m_sName;
            public int m_nAge;
            public DateTime m_dtBirthDay;
            public char m_chGender;
            #region OMethod: void DeclareAttrs()
            protected override void DeclareAttrs()
            {
                base.DeclareAttrs();
                DefineAttr("name", ref m_sName);
                DefineAttr("age", ref m_nAge);
                DefineAttr("birthday", ref m_dtBirthDay);
                DefineAttr("gender", ref m_chGender);
            }
            #endregion

            #region Constructor(sName, nAge, dtBirthDay, chGender)
            public TMerge(string sName, int nAge, DateTime dtBirthDay, char chGender)
            {
                m_sName = sName;
                m_nAge = nAge;
                m_dtBirthDay = dtBirthDay;
                m_chGender = chGender;
            }
            #endregion

            #region Method: bool IsSame(TMerge obj)
            public bool IsSame(TMerge obj)
            {
                if (m_sName != obj.m_sName)
                    return false;
                if (m_nAge != obj.m_nAge)
                    return false;
                if (m_dtBirthDay != obj.m_dtBirthDay)
                    return false;
                if (m_chGender != obj.m_chGender)
                    return false;
                return true;
            }
            #endregion
            #region Method: TMerge Clone()
            public TMerge Clone()
            {
                return new TMerge(m_sName, m_nAge, m_dtBirthDay, m_chGender);
            }
            #endregion
        }
        #endregion

        [Test] public void MergeBasicAttrs()
        {
            TMerge John = new TMerge("John", 49, new DateTime(1959, 11, 23), 'M');
            TMerge Sandra = new TMerge("Sandra", 47, new DateTime(1961, 8, 8), 'F');
            TMerge Emily = new TMerge("Emily", 20, new DateTime(1988, 11, 3), 'F');

            // Theirs changes, ours stays the same: expect result to be Theirs
            TMerge Parent = John.Clone();
            TMerge Mine = John.Clone();
            TMerge Theirs = Sandra.Clone();
            Mine.MergeBasicAttrs(Parent, Theirs, false);
            Assert.IsTrue(Mine.IsSame(Theirs), "A");

            // Ours changes, theirs stays the same; expect result to be our original value
            Mine = Sandra.Clone();
            Theirs = John.Clone();
            Mine.MergeBasicAttrs(Parent, Theirs, false);
            Assert.IsTrue(Mine.IsSame(Sandra.Clone()), "B");

            // Both change, We're supposed to win
            Mine = Sandra.Clone();
            Theirs = Emily.Clone();
            Mine.MergeBasicAttrs(Parent, Theirs, true);
            Assert.IsTrue(Mine.IsSame(Sandra.Clone()), "C");          

            // Both change, They're supposed to win
            Mine = Sandra.Clone();
            Theirs = Emily.Clone();
            Mine.MergeBasicAttrs(Parent, Theirs, false);
            Assert.IsTrue(Mine.IsSame(Emily.Clone()), "D");
        }
        #endregion
    }
}
