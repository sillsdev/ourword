#region ***** T_JAttr.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JAttr.cs
 * Author:  John Wimbish
 * Created: 31 Aug 2009
 * Purpose: Tests the classes in the JAttr file
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using NUnit.Framework;

using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion
#endregion


namespace OurWordTests.JWdb
{
    [TestFixture] public class T_BStringArray
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: CommaDelimitedString
        [Test] public void CommaDelimitedString()
        {
            var bsa = new BStringArray();

            bsa.Append("Genesis");
            bsa.Append("Exodus");
            bsa.Append("Leviticus");
            bsa.Append("Numbers");
            bsa.Append("Deuteronomy");

            string sDelimited = bsa.ToCommaDelimitedString();

            Assert.AreEqual("Genesis, Exodus, Leviticus, Numbers, Deuteronomy", 
                bsa.ToCommaDelimitedString(), 
                "Convert to comma delimited string");

            bsa.Clear();
            Assert.AreEqual(0, bsa.Length);
            bsa.FromCommaDelimitedString(sDelimited);

            // They are read back in sorted
            Assert.AreEqual(5, bsa.Length);
            Assert.AreEqual("Deuteronomy", bsa[0], "Expected Deuteronomy");
            Assert.AreEqual("Exodus", bsa[1], "Expected Exodus");
            Assert.AreEqual("Genesis", bsa[2], "Expected Genesis");
            Assert.AreEqual("Leviticus", bsa[3], "Expected Leviticus");
            Assert.AreEqual("Numbers", bsa[4], "Expected Numbers");
        }
        #endregion
    }
}
