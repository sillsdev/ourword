/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_SfDB.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the SfDB class
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
    [TestFixture] public class T_SfDB
    {
        #region TEST: Construction - all contructors propertly initialize all attrs
        [Test] public void Construction()
        // Test that all of the attributes are initialized properly, no matter which
        // version of the constructor is called
        {
            SfField f = new SfField("v");
            ConstructionValidation(f, "v", "", 0, "", "");

            f = new SfField("vt", "This is some verse text.");
            ConstructionValidation(f, "vt", "This is some verse text.", 0, "", "");

            f = new SfField("c", "1", 233);
            ConstructionValidation(f, "c", "1", 233, "", "");

            f = new SfField("vt", "pigi", "pergi", "ibt");
            ConstructionValidation(f, "vt", "pigi", 0, "pergi", "ibt");
        }
        private void ConstructionValidation(SfField f, string sMkr, string sData,
            int nLineNo, string sBT, string sIBT)
        {
            Assert.AreEqual(f.Mkr, sMkr);
            Assert.AreEqual(f.Data, sData);
            Assert.AreEqual(f.LineNo, nLineNo);
            Assert.AreEqual(f.BT, sBT);
            Assert.AreEqual(f.IBT, sIBT);
        }
        #endregion
        #region TEST: Comparison - The ContentEquals method works correctly
        [Test] public void Comparison()
        {
            SfField f1 = new SfField("vt", "pigi", 27);
            SfField f2 = new SfField("vt", "pigi", 27);
            Assert.IsTrue(f1.ContentEquals(f2));

            f2 = new SfField("v");
            Assert.IsFalse(f1.ContentEquals(f2));

            f1 = new SfField("vt", "pigi", "pergi", "ibt");
            f2 = new SfField("vt", "pigi", "pergi", "ibt");
            Assert.IsTrue(f1.ContentEquals(f2));
        }
        #endregion
    }
}
