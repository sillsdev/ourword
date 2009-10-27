/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JRef.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the T_JRef implementation
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
using OurWordData;
#endregion

namespace OurWordTests.Cellar
{
    [TestFixture] public class T_JRef
    {
        // Supporting classes (defines a hierarchy) ------------------------------------------
        #region TObjA
        public class TObjA : JObject
        {
            public JOwnSeq<TObjB> m_osA1 = null;
            public JOwnSeq<TObjB> m_osA2 = null;
            public JOwnSeq<TObjB> m_osA3 = null;
            public JOwn<TObjB> m_own1 = null;
            public JOwn<TObjB> m_own2 = null;
            public JRef<TObjE> m_ref1 = null;
            public JRef<TObjB> m_ref2 = null;

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
            JOwnSeq<TObjC> m_osB = null;
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
                m_osB = new JOwnSeq<TObjC>("B", this);
            }
            public TObjC FirstC
            {
                get
                {
                    return m_osB[0] as TObjC;
                }
            }
            public TObjC ObjC
            {
                get
                {
                    return m_osB[0] as TObjC;
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
            public JOwnSeq<TObjD> m_osC = null;
            public JOwn<TObjE> m_own = null;
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
                m_osC = new JOwnSeq<TObjD>("C", this);
                m_own = new JOwn<TObjE>("COwn", this);
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
            public JRef<TObjE> m_ref1;
            public JRef<TObjA> m_ref2;
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
                m_ref1 = new JRef<TObjE>("ref1", this);
                m_ref2 = new JRef<TObjA>("ref2", this);
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
