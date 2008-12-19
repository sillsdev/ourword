/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JObject.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the JObject implementation
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
            public JOwnSeq OwnSeq1
            {
                get
                {
                    return m_osOwnSeq1;
                }
            }
            JOwnSeq m_osOwnSeq1;
            #endregion
            #region Attr{g}: JOwnSeq OwnSeq2
            public JOwnSeq OwnSeq2
            {
                get
                {
                    return m_osOwnSeq2;
                }
            }
            JOwnSeq m_osOwnSeq2;
            #endregion
            #region Attr{g}: JOwnSeq OwnSeq3
            public JOwnSeq OwnSeq3
            {
                get
                {
                    return m_osOwnSeq3;
                }
            }
            JOwnSeq m_osOwnSeq3;
            #endregion

            #region Attr{g}: JOwn Own1
            public JOwn Own1
            {
                get
                {
                    return m_own1;
                }
            }
            JOwn m_own1;
            #endregion
            #region Attr{g}: JOwn Own2
            public JOwn Own2
            {
                get
                {
                    return m_own2;
                }
            }
            JOwn m_own2;
            #endregion

            #region Attr{g}: JRef Ref1
            public JRef Ref1
            {
                get
                {
                    return m_ref1;
                }
            }
            JRef m_ref1;
            #endregion
            #region Attr{g}: JRef Ref2
            public JRef Ref2
            {
                get
                {
                    return m_ref2;
                }
            }
            JRef m_ref2;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(sName)
            public TestObject(string sName)
                : base()
            {
                m_sName = sName;

                m_osOwnSeq1 = new JOwnSeq("os1", this, typeof(TestObject));
                m_osOwnSeq2 = new JOwnSeq("os2", this, typeof(TestObject));
                m_osOwnSeq3 = new JOwnSeq("os3", this, typeof(TestObject));

                m_own1 = new JOwn("own1", this, typeof(TestObject));
                m_own2 = new JOwn("own2", this, typeof(TestObject));

                m_ref1 = new JRef("ref1", this, typeof(TestObject));
                m_ref2 = new JRef("ref2", this, typeof(TestObject));
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
        #region Test: All_X_Attrs
        [Test] public void All_X_Attrs()
        {
            // All owning attrs
            ArrayList list = objRoot.AllOwningAttrs;
            Assert.AreEqual(5, list.Count);
            foreach (JAttr attr in list)
            {
                Assert.IsTrue(attr.GetType() != typeof(JRef));
                Assert.IsTrue(attr.GetType() == typeof(JOwnSeq) || 
                    attr.GetType() == typeof(JOwn));
            }

            // All atomic own attrs
            list = objRoot.AllJOwnAttrs;
            Assert.AreEqual(2, list.Count);
            foreach (JAttr attr in list)
            {
                Assert.IsTrue(attr.GetType() == typeof(JOwn));
            }

            // All seq own attrs
            list = objRoot.AllJOwnSeqAttrs;
            Assert.AreEqual(3, list.Count);
            foreach (JAttr attr in list)
            {
                Assert.IsTrue(attr.GetType() == typeof(JOwnSeq));
            }

            // All atomic ref attrs
            list = objRoot.AllJRefAttrs;
            Assert.AreEqual(2, list.Count);
            foreach (JAttr attr in list)
            {
                Assert.IsTrue(attr.GetType() == typeof(JRef));
            }

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
            public JOwnSeq OwnSeqDup
            {
                get
                {
                    return m_osOwnSeqDup;
                }
            }
            JOwnSeq m_osOwnSeqDup;
            #endregion
            #region Constructor()
            public TestObjectDups()
                : base("dup")
            {
                m_osOwnSeqDup = new JOwnSeq("os2", this, typeof(TestObject));
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
        class BAttrTestObject : JObject
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
            string sFileName = "text.x";
            TextWriter tw = JWU.NUnit_OpenTextWriter(sFileName);
            objStart.Write(tw, 0);
            tw.Close();

            // Create a read-in object and populate it from the file we just wrote out
            BAttrTestObject objEnd = new BAttrTestObject();
            TextReader tr = JWU.NUnit_OpenTextReader(sFileName);
            string sLine = tr.ReadLine();
            objEnd.Read(sLine, tr);
            tr.Close();

            // Test that the attributes are all the same
            Assert.AreEqual(objStart.Name, objEnd.Name, "Read");
            Assert.AreEqual(objStart.IsYoung, objEnd.IsYoung, "Read");
            Assert.AreEqual(objStart.Age, objEnd.Age, "Read");
            Assert.AreEqual(objStart.Weight, objEnd.Weight, "Read");
            Assert.AreEqual(objStart.DOB, objEnd.DOB, "Read");
            Assert.AreEqual(objStart.Gender, objEnd.Gender, "Read");
        }
        #endregion
    }
}
