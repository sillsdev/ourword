/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Xml.cs
 * Author:  John Wimbish
 * Created: 14 May 2008
 * Purpose: Tests the xml classes
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

namespace OurWordTests.JWTools
{
    [TestFixture] public class T_Xml
    {
        #region TagRetrieval
        [Test] public void TagRetrieval()
        {
            string sLine = "   <person name=\"John\" gender=\"male\" age=\"44\">";
            Assert.IsTrue("person" == JW_Xml.GetTag(sLine));
            Assert.IsTrue("person" == JW_Xml.GetTag("<person>"));

            Assert.IsTrue(JW_Xml.IsTag("person", sLine));
            Assert.IsTrue(JW_Xml.IsTag("person", "<person>"));

            Assert.IsFalse(JW_Xml.IsTag("people", sLine));

            Assert.IsTrue(JW_Xml.IsClosingTag("person", "</person>"));
            Assert.IsTrue(JW_Xml.IsClosingTag("person", "     </person>"));
            Assert.IsTrue(JW_Xml.IsClosingTag("/person", "</person>"));
            Assert.IsTrue(JW_Xml.IsClosingTag("/person", "     </person>"));
        }
        #endregion
        #region ValueRetrieval
        [Test] public void ValueRetrieval()
        {
            string sLine = "   <person name=\"John\" gender=\"male\" age=\"44\">";

            Assert.IsTrue("John" == JW_Xml.GetValue("name", sLine));
            Assert.IsTrue("male" == JW_Xml.GetValue("gender", sLine));
            Assert.IsTrue("44" == JW_Xml.GetValue("age", sLine));
        }
        #endregion
    }
}
