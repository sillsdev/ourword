/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Localization.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the Localization classes
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
using OurWord.Layouts;
#endregion

namespace OurWordTests.JWTools
{
    [TestFixture] public class T_Localization
    {
        #region Method: void Setup()
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: IO_LocAlternate
        [Test]
        public void IO_LocAlternate()
        {
            // Create a LocAlternate
            string sValue = "Baru";
            string sKey = "Ctrl+B";
            string sTip = "Membuku file baru";
            LocAlternate altOut = new LocAlternate(sValue, sKey, sTip);

            // File Internals
            string sTag = "Test";
            string sFileName = "LocAlt.xml";

            // Write it to file
            TextWriter w = JWU.NUnit_OpenTextWriter(sFileName);
            XmlField xml = new XmlField(w, sTag);
            xml.Begin();
            altOut.WriteXML(xml, "inz");
            xml.End();
            w.Close();

            // Read it back in from file
            TextReader r = JWU.NUnit_OpenTextReader(sFileName);
            XmlRead xr = new XmlRead(r);
            LocAlternate altIn = null;
            while (xr.ReadNextLineUntilEndTag(sTag))
            {
                if (xr.IsTag(LocAlternate.c_sTag))
                    altIn = LocAlternate.ReadXML(xr);
            }
            r.Close();

            // Are they the same?
            Assert.IsNotNull(altIn);
            Assert.AreEqual(sValue, altIn.Value, "bad Value");
            Assert.AreEqual(sKey, altIn.ShortcutKey, "bad ShortcutKey");
            Assert.AreEqual(sTip, altIn.ToolTip, "bad ToolTip");
        }
        #endregion
        #region Test: IO_LocItem
        [Test]
        public void IO_LocItem()
        {
            // Create a LocAlternate
            string sID = "idNew";
            string sEnglish = "&New";
            string sKey = "Ctrl+N";
            string sTip = "Open a new file";
            string sInfo = "This is the menu command to open a new file.";
            LocItem itemOut = new LocItem(sID);
            itemOut.English = sEnglish;
            itemOut.ShortcutKey = sKey;
            itemOut.ToolTip = sTip;
            itemOut.Information = sInfo;
            itemOut.CanHaveShortcutKey = true;
            itemOut.CanHaveToolTip = true;

            // File Internals
            string sTag = "Test";
            string sFileName = "LocItem.xml";

            // Write it to file
            TextWriter w = JWU.NUnit_OpenTextWriter(sFileName);
            XmlField xml = new XmlField(w, sTag);
            xml.Begin();
            itemOut.WriteXML(xml);
            xml.End();
            w.Close();

            // Read it back in from file
            TextReader r = JWU.NUnit_OpenTextReader(sFileName);
            XmlRead xr = new XmlRead(r);
            LocItem itemIn = null;
            while (xr.ReadNextLineUntilEndTag(sTag))
            {
                if (xr.IsTag(LocItem.c_sTag))
                    itemIn = LocItem.ReadXML(xr);
            }
            r.Close();

            // Are they the same?
            Assert.IsNotNull(itemIn);
            Assert.AreEqual(sID, itemIn.ID, "bad ID");
            Assert.AreEqual(sEnglish, itemIn.English, "bad English");
            Assert.AreEqual(sKey, itemIn.ShortcutKey, "bad ShortcutKey");
            Assert.AreEqual(sTip, itemIn.ToolTip, "bad ToolTip");
            Assert.AreEqual(sInfo, itemIn.Information, "bad Information");
            Assert.AreEqual(true, itemIn.CanHaveShortcutKey, "bad CanHaveShortcutKey");
            Assert.AreEqual(true, itemIn.CanHaveToolTip, "bad CanHaveToolTip");
        }
        #endregion
        #region Test: IO_LocLanguage
        [Test]
        public void IO_LocLanguage()
        {
            // Create a LocLanguage
            string sID = "inz";
            string sName = "Bahasa Indonesia";
            string sFontName = "Times New Roman";
            int nFontSize = 20;
            LocLanguage langOut = new LocLanguage(sID, sName, 0);
            langOut.FontName = sFontName;
            langOut.FontSize = nFontSize;

            // File Internals
            string sTag = "Test";
            string sFileName = "LocAlt.xml";

            // Write it to file
            TextWriter w = JWU.NUnit_OpenTextWriter(sFileName);
            XmlField xml = new XmlField(w, sTag);
            xml.Begin();
            langOut.WriteXML(xml);
            xml.End();
            w.Close();

            // Read it back in from file
            TextReader r = JWU.NUnit_OpenTextReader(sFileName);
            XmlRead xr = new XmlRead(r);
            LocLanguage langIn = null;
            while (xr.ReadNextLineUntilEndTag(sTag))
            {
                if (xr.IsTag(LocLanguage.c_sTag))
                    langIn = LocLanguage.ReadXML(0, xr);
            }
            r.Close();

            // Are they the same?
            Assert.IsNotNull(langIn);
            Assert.AreEqual(sID, langIn.ID, "bad ID");
            Assert.AreEqual(sName, langIn.Name, "bad Name");
            Assert.AreEqual(sFontName, langIn.FontName, "bad FontName");
            Assert.AreEqual(nFontSize, langIn.FontSize, "bad FontSize");
        }
        #endregion
    }
}
