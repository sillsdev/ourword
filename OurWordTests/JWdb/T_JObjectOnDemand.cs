/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JObjectOnDemand.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the T_JObjectOnDemand implementation
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
    [TestFixture] public class T_JObjectOnDemand
    {
        #region Supporting Class: TObjLOD
        public class TObjLOD : JObjectOnDemand
        {
            public JOwnSeq<JCharacterStyle> m_os = null;
            public JOwn<JCharacterStyle> m_own = null;
            public JRef<JCharacterStyle> m_ref = null;

            public TObjLOD(string sDisplayName)
                : base()
            {
                // We are setting up the attrs, so we don't want it try to Read
                m_bIsLoaded = true;

                // Display Name 
                DisplayName = sDisplayName;

                // Owning sequence
                m_os = new JOwnSeq<JCharacterStyle>("os", this);
                m_os.Append(new JCharacterStyle("v", "Verse Number"));
                m_os.Append(new JCharacterStyle("c", "Chapter Number"));

                // Owning atomic
                m_own = new JOwn<JCharacterStyle>("own", this);
                m_own.Value = new JCharacterStyle("h", "Header");

                // Reference Atomic
                m_ref = new JRef<JCharacterStyle>("ref", this);
                m_ref.Value = m_own.Value as JCharacterStyle;
			}

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

        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: LoadOnDemandMechanism
        [Test] public void LoadOnDemandMechanism()
        // Checks that the LoadOnDemand mechanism works for the Value attribute
        {
            // Create a LOD attribute and populate it
            TObjLOD obj = new TObjLOD("OwnAtomicTest");
            Assert.IsNotNull(obj.m_own.Value);

            // Write it and Release it; check that it is indeed released
            Assert.IsTrue(obj.Loaded);
            Assert.IsNotNull(obj.m_own.Value);
            obj.Unload(new NullProgress());
            Assert.IsNull(obj.m_own.Value);
            Assert.IsFalse(obj.Loaded);

            // Load the object
            obj.Load(new NullProgress());

            // Check that it is loaded
            Assert.IsNotNull(obj.m_own.Value);
            Assert.IsTrue(obj.Loaded);
            JCharacterStyle style = (JCharacterStyle)obj.m_own.Value;
            Assert.AreEqual("h", style.Abbrev);
        }
        #endregion
        #region Test: DirtyMechanism
        [Test] public void DirtyMechanism()
        {
            // Create a LOD attribute and populate it
            TObjLOD obj = new TObjLOD("OwnAtomicTest");
            Assert.IsNotNull(obj.m_own.Value);
            JCharacterStyle o;

            // JRef: Clear --> Dirty is set to true
            obj.IsDirty = false;
            Assert.IsFalse(obj.IsDirty);
            obj.m_ref.Clear();
            Assert.IsTrue(obj.IsDirty);

            // JRef: Set Value --> Dirty is set to true
            obj.IsDirty = false;
            obj.m_ref.Value = obj.m_own.Value as JCharacterStyle;
            Assert.IsTrue(obj.IsDirty);

            // JRef: Get Value --> Dirty is unaffected
            obj.IsDirty = false;
            o = obj.m_ref.Value;
            Assert.IsFalse(obj.IsDirty);
            obj.IsDirty = true;
            o = obj.m_ref.Value;
            Assert.IsTrue(obj.IsDirty);

            // JOwn: Clear --> Dirty is set to true
            obj.IsDirty = false;
            obj.m_own.Clear();
            Assert.IsTrue(obj.IsDirty);

            // JOwn: Set Value --> Dirty is set to true
            obj.IsDirty = false;
            obj.m_own.Value = new JCharacterStyle("h", "Header");
            Assert.IsTrue(obj.IsDirty);

            // JOwn: Get Value --> Dirty is unaffected
            obj.IsDirty = false;
            o = obj.m_own.Value;
            Assert.IsFalse(obj.IsDirty);
            obj.IsDirty = true;
            o = obj.m_own.Value;
            Assert.IsTrue(obj.IsDirty);

            // JObject: Clear -> Dirty is set to true
            obj.IsDirty = false;
            obj.Clear();
            Assert.IsTrue(obj.IsDirty);

            // JSeq: Clear --> Dirty is set to true
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            obj.m_os.Clear();
            Assert.IsTrue(obj.IsDirty);

            // JSeq: AddParagraph --> Dirty is set to true
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            obj.m_os.Append(new JCharacterStyle("fn", "Footnote"));
            Assert.IsTrue(obj.IsDirty);

            // JSeq: InsertAt --> Dirty is set to true
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            o = new JCharacterStyle("fn", "Footnote");
            obj.m_os.InsertAt(1, o);
            Assert.IsTrue(obj.IsDirty);

            // JSeq: Remove --> Dirty is set to true
            obj.IsDirty = false;
            obj.m_os.Remove(o);
            Assert.IsTrue(obj.IsDirty);

            // JSeq: Remove --> Dirty is set to true
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            obj.m_os.RemoveAt(0);
            Assert.IsTrue(obj.IsDirty);

            // JSeq: ForceSort --> Dirty is set to true
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            obj.m_os.ForceSort();
            Assert.IsTrue(obj.IsDirty);

            // JSeq: Find --> Dirty is unaffected
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            obj.m_os.Find("v");
            Assert.IsFalse(obj.IsDirty);
            obj.IsDirty = true;
            obj.m_os.Find("v");
            Assert.IsTrue(obj.IsDirty);

            // JSeq: Iterator --> Dirty is unaffected
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            foreach (JCharacterStyle cs in obj.m_os)
            {
                o = cs;
            }
            Assert.IsFalse(obj.IsDirty);
            obj.IsDirty = true;
            foreach (JCharacterStyle cs in obj.m_os)
            {
                o = cs;
            }
            Assert.IsTrue(obj.IsDirty);

            // JSeq: Indexor --> Dirty is set to true
            obj = new TObjLOD("OwnAtomicTest");
            obj.IsDirty = false;
            o = new JCharacterStyle("fn", "Footnote");
            obj.m_os[0] = o;
            Assert.IsTrue(obj.IsDirty);
        }
        #endregion
    }
}
