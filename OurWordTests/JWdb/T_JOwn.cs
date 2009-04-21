/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JOwn.cs
 * Author:  John Wimbish
 * Created: 14 July 2008
 * Purpose: Tests the JOwn implementation
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
    [TestFixture] public class T_JOwn
    {
        // Supporting test classes -----------------------------------------------------------
	    #region TObjA
	    public class TObjA : JObjectOnDemand
	    { 
		    public JOwnSeq<TObjB> m_osA1 = null;
		    public JOwnSeq<TObjB> m_osA2 = null;
		    public JOwnSeq<TObjB> m_osA3 = null;
		    public JOwn<TObjB> m_own1 = null;
		    public JOwn<TObjB> m_own2 = null;
		    public JRef<TObjE> m_ref1 = null;
		    public JRef<TObjB> m_ref2 = null;

		    // Constructor that attempts to create two attrs of the same name; should fail
		    public TObjA(string s, bool b) : base(s)
		    {
			    m_own1 = new JOwn<TObjB>("own", this);
			    m_own2 = new JOwn<TObjB>("own", this);
		    }

            public TObjA()
                : base()
            {
			    // One complete deep ownership hierarhcy
			    m_osA1 = new JOwnSeq<TObjB>("A1", this);
			    m_osA1.Append(new TObjB("B1"));

			    // Some more owning sequences for AllOwningAttrs testing
			    m_osA2 = new JOwnSeq<TObjB>("A2", this);
			    m_osA2.Append(new TObjB("B2"));
			    m_osA3 = new JOwnSeq<TObjB>("A3", this);
			    m_osA3.Append(new TObjB("B3"));

			    // Some atomic owners
			    m_own1 = new JOwn<TObjB>("own1", this);
			    m_own2 = new JOwn<TObjB>("own2", this);

			    // Some atomic references
			    m_ref1 = new JRef<TObjE>("ref1", this);
			    m_ref1.Value = ObjE;
			    m_ref2 = new JRef<TObjB>("ref2", this);

			    ObjD.m_ref2.Value = this;
            }

		    public TObjA(string s) : this()
		    {
                base.DisplayName = s;
		    }
		    public TObjB FirstB { get { return (TObjB)m_osA1[0]; } }
		    public TObjC ObjC { get { return FirstB.ObjC; } }
		    public TObjD ObjD { get { return ObjC.ObjD; } }
		    public TObjE ObjE { get { return ObjC.ObjE; } }

			#region OAttr{g}: string StoragePath
			public override string StoragePath
			{
				get
				{
					return T_JOwn.ReadWritePathName;
				}
			}
			#endregion
		}
	    #endregion
	    #region TObjB
	    public class TObjB : TObject 
	    { 
		    JOwnSeq<TObjC> m_osB = null;
		    public TObjB(string s) : base(s) 
		    {
			    _ConstructAttrs();
			    m_osB.Append(new TObjC("C"));
		    }
		    public TObjB() : base("") // Read constructor
		    {
			    _ConstructAttrs();
		    }
		    private void _ConstructAttrs()
		    {
			    m_osB = new JOwnSeq<TObjC>("B", this);
		    }
		    public TObjC FirstC { get { return (TObjC)m_osB[0]; } }
		    public TObjC ObjC { get { return (TObjC)m_osB[0]; } }
		    public TObjD ObjD { get { return ObjC.ObjD; } }
		    public TObjE ObjE { get { return ObjC.ObjE; } }
	    }
	    #endregion
	    #region TObjC
	    public class TObjC : TObject 
	    { 
		    public JOwnSeq<TObjD> m_osC = null;
		    public JOwn<TObjE> m_own = null;
		    public TObjC() : base("") 
		    {
			    _ConstructAttrs();
		    }
		    public TObjC(string s) : base(s) 
		    {
			    _ConstructAttrs();
			    m_osC.Append(new TObjD("D"));
			    m_own.Value = new TObjE("E");
                ObjD.m_ref1.Value = m_own.Value;
		    }
		    private void _ConstructAttrs()
		    {
			    m_osC = new JOwnSeq<TObjD>("C", this);
			    m_own = new JOwn<TObjE>("COwn", this);
		    }
		    public TObjD FirstD { get { return (TObjD)m_osC[0]; } }
		    public TObjD ObjD { get { return FirstD; } }
		    public TObjE ObjE { get { return m_own.Value; } }
	    }
	    #endregion
	    #region TobjD
	    public class TObjD : TObject 
	    { 
		    public JRef<TObjE> m_ref1;
		    public JRef<TObjA> m_ref2;
		    public TObjD() : base("") 
		    {
			    _ConstructAttrs();
		    }
		    public TObjD(string s) : base(s) 
		    {
			    _ConstructAttrs();
		    }
		    private void _ConstructAttrs()
		    {
			    m_ref1 = new JRef<TObjE>("ref1", this);
			    m_ref2 = new JRef<TObjA>("ref2", this);
		    }
	    }
	    #endregion
	    #region TobjE
	    public class TObjE : TObject
	    {
		    public TObjE(string s): base(s)
		    {
		    }
		    public TObjE(): base("")
		    {
		    }
	    }
	    #endregion

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
        #region Ownership
        [Test] public void Ownership()
        {
            // Create a couple of objects. They should have no owner
            TObjB obj1 = new TObjB("b1");
            Assert.IsTrue(obj1.Owner == null);
            TObjB obj2 = new TObjB("b2");
            Assert.IsTrue(obj2.Owner == null);

            // Set the first obj into the attr, and see if it has an owner
            TObjA objOwner = new TObjA("a");
            objOwner.m_own1.Value = obj1;
            Assert.IsTrue(obj1.Owner == objOwner);

            // Put the other obj into the attr, and see that the owner is the second obj.
            objOwner.m_own1.Value = obj2;
            Assert.IsTrue(obj2.Owner == objOwner);
            Assert.IsTrue(obj1.Owner == null);
        }
        #endregion
        #region OwnershipUniqueness
        [Test]
        [ExpectedException(typeof(eAlreadyOwned))]
        public void OwnershipUniqueness()
        {
            // Create an owning object
            TObjA objOwner = new TObjA("a");

            // Place an object into the first attribute
            TObjB objB = new TObjB("b1");
            objOwner.m_own1.Value = objB;

            // Now attempt to place it into the second attr. Should fail because
            // object is already owned.
            objOwner.m_own2.Value = objB;
        }
        #endregion
        #region OwnerStoresSelf
        [Test] public void OwnerStoresSelf()
        {
            TObjA objOwner = new TObjA("a");
            Assert.IsTrue(objOwner._test_ContainsAttribute(objOwner.m_own1));
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Attr: ReadWritePathName - pathname for test file
        public static string ReadWritePathName
        {
            get
            {
                return JWU.NUnit_TestFileFolder + Path.DirectorySeparatorChar + "Test.x";
            }
        }
        #endregion
        #region ReadWrite
        [Test] public void ReadWrite()
        {
            // Create owning objects

            // Set up an owning attr and write it out
            TObjA objOwner1 = new TObjA("a1");
            objOwner1.m_own1.Value = new TObjB("VerseNo");
            objOwner1.Write(new NullProgress());

            // Read it into anouther owning attr
            TObjA objOwner2 = new TObjA("a2");
            objOwner2.Load(new NullProgress());

            // Compare the two
            TObjB b1 = objOwner1.m_own1.Value;
            TObjB b2 = objOwner2.m_own1.Value;
            Assert.AreEqual(b1.Name, b2.Name);

            // Cleanup
            File.Delete(ReadWritePathName);
        }
        #endregion
    }


}
