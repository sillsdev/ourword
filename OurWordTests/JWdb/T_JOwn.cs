/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JOwn.cs
 * Author:  John Wimbish
 * Created: 14 July 2008
 * Purpose: Tests the JOwn implementation
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
    [TestFixture] public class T_JOwn
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

        // Correct signature of owned object -------------------------------------------------
        #region SignatureControl
        [Test]
        [ExpectedException(typeof(eBadSignature))]
        public void SignatureControl()
        // We declare a JOwn with a signature of one type, and then attempt to
        // add an object to it of a different type. We expect an exception. If we check
        // anytime an object is being added, then we prevent the sequence from ever
        // having bad objects.
        {
            // Create an owning object
            TObjA objOwner = new TObjA("a");

            // Attempt to add the wrong kind of object
            objOwner.m_own1.Value = new TObjD("d");
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Attr: ReadWritePathName - pathname for test file
        private string ReadWritePathName
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
            TObjA objOwner1 = new TObjA("a1");
            TObjA objOwner2 = new TObjA("a2");

            // Set up an owning attr and write it out
            objOwner1.m_own1.Value = new TObjB("VerseNo");
            TextWriter tw = JUtil.GetTextWriter(ReadWritePathName);
            objOwner1.m_own1.Write(tw, 0);
            tw.Close();

            // Read it into anouther owning attr
            TextReader tr = JUtil.GetTextReader(ReadWritePathName);
            string sLine;
            while ((sLine = tr.ReadLine()) != null)
            {
                objOwner2.m_own1.Read(sLine, tr);
            }
            tr.Close();

            // Compare the two
            TObjB b1 = objOwner1.m_own1.Value as TObjB;
            TObjB b2 = objOwner2.m_own1.Value as TObjB;
            Assert.AreEqual(b1.Name, b2.Name);

            // Cleanup
            File.Delete(ReadWritePathName);
        }
        #endregion
    }
}
