#region ***** T_BookNames.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_BookNames.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the BookNames class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWordTests.Dialogs
{
    [TestFixture] public class T_BookNames
    {
        #region Method: void Setup()
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: BookNameRetrieval
        [Test]
        public void BookNameRetrieval()
        {
			// U+00F1 = small n with tilde
            string[] vSpanish = BookNames.GetTable("Espa\u00F1ol");

			// U+00c9 = capital E with acute
            Assert.AreEqual("\u00C9xodo", vSpanish[1]);
        }
        #endregion
    }
}
