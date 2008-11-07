/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JRef.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the T_JRef implementation
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
    [TestFixture] public class T_JRef
    {
        // Supporting classes (defines a hierarchy) ------------------------------------------
        #region TObjA
        public class TObjA : JObject
        {
            public JOwnSeq m_osA1 = null;
            public JOwnSeq m_osA2 = null;
            public JOwnSeq m_osA3 = null;
            public JOwn m_own1 = null;
            public JOwn m_own2 = null;
            public JRef m_ref1 = null;
            public JRef m_ref2 = null;

            public TObjA()
                : base()
            {
                // One complete deep ownership hierarhcy
                m_osA1 = new JOwnSeq("A1", this, typeof(TObjB));
                m_osA1.Append(new TObjB("B1"));

                // Some more owning sequences for AllOwningAttrs testing
                m_osA2 = new JOwnSeq("A2", this, typeof(TObjB));
                m_osA2.Append(new TObjB("B2"));
                m_osA3 = new JOwnSeq("A3", this, typeof(TObjB));
                m_osA3.Append(new TObjB("B3"));

                // Some atomic owners
                m_own1 = new JOwn("own1", this, typeof(TObjB));
                m_own2 = new JOwn("own2", this, typeof(TObjB));

                // Some atomic references
                m_ref1 = new JRef("ref1", this, typeof(TObjE));
                m_ref1.Value = ObjE;
                m_ref2 = new JRef("ref2", this, typeof(TObjB));

                ObjD.m_ref2.Value = this;
            }
            public TObjB FirstB
            {
                get
                {
                    return (TObjB)m_osA1[0];
                }
            }
            public TObjC ObjC
            {
                get
                {
                    return FirstB.ObjC;
                }
            }
            public TObjD ObjD
            {
                get
                {
                    return ObjC.ObjD;
                }
            }
            public TObjE ObjE
            {
                get
                {
                    return ObjC.ObjE;
                }
            }
        }
        #endregion
        #region TObjB
        public class TObjB : TObject
        {
            JOwnSeq m_osB = null;
            public TObjB(string s)
                : base(s)
            {
                _ConstructAttrs();
                m_osB.Append(new TObjC("C"));
            }
            public TObjB()
                : base("") // Read constructor
            {
                _ConstructAttrs();
            }
            private void _ConstructAttrs()
            {
                m_osB = new JOwnSeq("B", this, typeof(TObjC));
            }
            public TObjC FirstC
            {
                get
                {
                    return (TObjC)m_osB[0];
                }
            }
            public TObjC ObjC
            {
                get
                {
                    return (TObjC)m_osB[0];
                }
            }
            public TObjD ObjD
            {
                get
                {
                    return ObjC.ObjD;
                }
            }
            public TObjE ObjE
            {
                get
                {
                    return ObjC.ObjE;
                }
            }
        }
        #endregion
        #region TObjC
        public class TObjC : TObject
        {
            public JOwnSeq m_osC = null;
            public JOwn m_own = null;
            public TObjC()
                : base("")
            {
                _ConstructAttrs();
            }
            public TObjC(string s)
                : base(s)
            {
                _ConstructAttrs();
                m_osC.Append(new TObjD("D"));
                m_own.Value = new TObjE("E");
                ObjD.m_ref1.Value = m_own.Value;
            }
            private void _ConstructAttrs()
            {
                m_osC = new JOwnSeq("C", this, typeof(TObjD));
                m_own = new JOwn("COwn", this, typeof(TObjE));
            }
            public TObjD FirstD
            {
                get
                {
                    return (TObjD)m_osC[0];
                }
            }
            public TObjD ObjD
            {
                get
                {
                    return FirstD;
                }
            }
            public TObjE ObjE
            {
                get
                {
                    return (TObjE)m_own.Value;
                }
            }
        }
        #endregion
        #region TobjD
        public class TObjD : TObject
        {
            public JRef m_ref1;
            public JRef m_ref2;
            public TObjD()
                : base("")
            {
                _ConstructAttrs();
            }
            public TObjD(string s)
                : base(s)
            {
                _ConstructAttrs();
            }
            private void _ConstructAttrs()
            {
                m_ref1 = new JRef("ref1", this, typeof(TObjE));
                m_ref2 = new JRef("ref2", this, typeof(TObjA));
            }
        }
        #endregion
        #region TobjE
        public class TObjE : TObject
        {
            public TObjE(string s)
                : base(s)
            {
            }
            public TObjE()
                : base("")
            {
            }
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: PathToReferencedObject
        [Test] public void PathToReferencedObject()
        {
            TObjA objA = new TObjA();
            TObjC objC = objA.ObjC;
            TObjD objD = objA.ObjD;
            TObjE objE = objA.ObjE;

            string sPathFromAtoD = objA.m_ref1.PathToReferencedObject;
            //Console.WriteLine("PathFromAtoD = " + objA.m_ref1.PathToReferencedObject);
            Assert.AreEqual("0-A1-0-B-0-COwn", sPathFromAtoD);

            string sPathFromDtoE = objD.m_ref1.PathToReferencedObject;
            //Console.WriteLine("PathFromDtoE = " + objD.m_ref1.PathToReferencedObject);
            Assert.AreEqual("1-COwn", sPathFromDtoE);

            string sPathFromDtoA = objD.m_ref2.PathToReferencedObject;
            //Console.WriteLine("PathFromDtoA = " + objD.m_ref2.PathToReferencedObject);
            Assert.AreEqual("3", sPathFromDtoA);
        }
        #endregion

        /*** TODO: Get this test working again
        #region RefObjReadWrite
        [Test] public void RefObjReadWrite()
        // Create a style sheet and write it out; then read into a different stylesheet;
        // then check to see if the references were set correctly in the new stylesheet.
        // If this works, there is a lot that is working correctly to get to this point.
        {
            string sPath = JWU.NUnit_TestFilePathName;

            // Create a style sheet and write it out
            JStyleSheet SS_out = new JStyleSheet();
            JWritingSystem WS_out = SS_out.AddWritingSystem("Greek");
            JCharacterStyle CS_out = SS_out.AddCharacterStyle("Verse", "");
            SS_out.Write(sPath, 0);

            // Read it into a blank style sheet
            JStyleSheet SS_in = new JStyleSheet();
            string sReadWritePathName = sPath;
            SS_in.Read(ref sReadWritePathName, "");

            // Compare the reference attributes
            JCharacterStyle CS_in = SS_in.FindCharacterStyle("Verse");
            Assert.IsNotNull(CS_in);
            JWritingSystem WS_in = SS_in.FindWritingSystem("Greek");
            Assert.IsNotNull(WS_in);
        }
        #endregion
        ***/

    }
}
