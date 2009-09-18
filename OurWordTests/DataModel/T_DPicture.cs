#region ***** T_DPicture.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DPicture.cs
 * Author:  John Wimbish
 * Created: 15 Aug 2009
 * Purpose: Tests the DPicture  class
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

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class T_DPicture : TestCommon
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: OxesIO
        [Test] public void OxesIO()
        {
            // Set up a hierarchy all the way down to our picture
            var section = CreateHierarchyThroughTargetSection("MRK");

            // Create the xml doc
            var oxes = new XmlDoc();
            var nodeBook = oxes.AddNode(null, "book");

            // Attribute data
            string sPath = "C:\\graphics\\wineskin.jpg";
            string sRtf = "width:11.0cm;verticalPosition:top;horizontalPosition:center";
            string sCap = "Ini adalah sesuatu wineskin.";
            string sCapBT = "This is a wineskin.";

            // Create a picture. 
            var pictureIn = new DPicture();
            section.Paragraphs.Append(pictureIn);
            pictureIn.PathName = sPath;
            pictureIn.WordRtfInfo = sRtf;
            pictureIn.SimpleText = sCap;
            pictureIn.SimpleTextBT = sCapBT;

            // Save it to an xml node
            var nodePicture = pictureIn.SaveToOxesBook(oxes, nodeBook);

            // Create a new picture from that node
            var pictureOut = DPicture.CreatePicture(nodePicture);

            // Should be identical
            Assert.AreEqual(sPath, pictureOut.PathName);
            Assert.AreEqual(sRtf, pictureOut.WordRtfInfo);
            Assert.AreEqual(sCap, pictureOut.SimpleText);
            Assert.AreEqual(sCapBT, pictureOut.SimpleTextBT);
            Assert.IsTrue(pictureOut.ContentEquals(pictureIn), "Pictures are the same.");
        }
        #endregion
    }
}
