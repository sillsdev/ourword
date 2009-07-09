/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_StdFmtTransformt.cs
 * Author:  John Wimbish
 * Created: 2 Jul 2009
 * Purpose: Tests the standard format transforms (most testing is accomplsiehd via T_DSection)
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

using OurWord;
using JWdb.DataModel;
using JWTools;
using JWdb;
#endregion

namespace OurWordTests.JWdb
{
    [TestFixture]
    public class T_StdFmrTransform
    {
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
        }
        #endregion

        #region Method: CompareStringArrays(vsExpected, vsActual)
        void CompareStringArrays(string[] vsExpected, string[] vsActual)
        {
            Assert.AreEqual(vsExpected.Length, vsActual.Length);
            for (int i = 0; i < vsExpected.Length; i++)
                Assert.AreEqual(vsExpected[i], vsActual[i]);
        }
        #endregion

        #region Test: XForm_HandleEmbeddedParatextParagraphs
        [Test] public void XForm_HandleEmbeddedParatextParagraphs()
        {
            var vsIn = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };
            var vsExpected = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, ",
                "\\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. ",
                "\\q Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kParatext;

            // Run the transform
            var transform = new XForm_HandleEmbeddedParatextParagraphs(DB);
            transform.OnImport();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: XForm_HandleLineMedialNormalizations
        [Test] public void XForm_HandleLineMedialNormalizations()
        {
            var vsIn = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q1 <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q1 Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };
            var vsExpected = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kParatext;

            // Run the transform
            var transform = new XForm_NormalizeMarkers(DB);
            transform.OnImport();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
    }
}
