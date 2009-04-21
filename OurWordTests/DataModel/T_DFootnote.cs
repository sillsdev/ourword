/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DFootnote.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DFootnote class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DFootnote
    {
        #region Test: ParseLabel
        [Test]
        public void ParseLabel()
        {
            string sOut;
            string sLabel;

            // Remember the current setting so we can restore it
            DFootnote.RefLabelTypes sav = DFootnote.RefLabelType;

            // Test the kNone setting
            DFootnote.RefLabelType = DFootnote.RefLabelTypes.kNone;
            sOut = DFootnote.ParseLabel("5:4: Text1", out sLabel);
            Assert.AreEqual("", sLabel);
            Assert.AreEqual("5:4: Text1", sOut);

            // Test the kStandard settings
            DFootnote.RefLabelType = DFootnote.RefLabelTypes.kStandard;
            sOut = DFootnote.ParseLabel("5:4: Text2", out sLabel);
            Assert.AreEqual("5:4:", sLabel);
            Assert.AreEqual("Text2", sOut);

            sOut = DFootnote.ParseLabel("5:4a: Text3", out sLabel);
            Assert.AreEqual("5:4a:", sLabel);
            Assert.AreEqual("Text3", sOut);

            sOut = DFootnote.ParseLabel("5.4-3; Text4", out sLabel);
            Assert.AreEqual("5.4-3;", sLabel);
            Assert.AreEqual("Text4", sOut);

            sOut = DFootnote.ParseLabel("5.4-3; <Text5", out sLabel);
            Assert.AreEqual("5.4-3;", sLabel);
            Assert.AreEqual("<Text5", sOut);

            sOut = DFootnote.ParseLabel("5.4-3; \"Text6", out sLabel);
            Assert.AreEqual("5.4-3;", sLabel);
            Assert.AreEqual("\"Text6", sOut);

            // Chuck's Acts 27:28 example, "27:28: 20 fathoms is 37 meters"
            sOut = DFootnote.ParseLabel("27:28: 20 fathoms1", out sLabel);
            Assert.AreEqual("27:28:", sLabel);
            Assert.AreEqual("20 fathoms1", sOut);
            sOut = DFootnote.ParseLabel("27:28:20 fathoms2", out sLabel);
            Assert.AreEqual("27:28:", sLabel);
            Assert.AreEqual("20 fathoms2", sOut);

            // Restore the setting
            DFootnote.RefLabelType = sav;
        }
        #endregion
    }
}
