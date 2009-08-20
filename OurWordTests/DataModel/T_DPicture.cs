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
    public class T_DPicture
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
            // Set up a hierarchy all the way down to our paragraph
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;
            var Book = new DBook("MRK");
            Translation.Books.Append(Book);
            var Section = new DSection();
            Book.Sections.Append(Section);

            // Create the xml doc
            var oxes = new XmlDoc();
            var nodeBook = oxes.AddNode(null, "book");

            // Attribute data
            string sPath = "C:\\graphics\\wineskin.jpg";
            string sRtf = "width:11.0cm;verticalPosition:top;horizontalPosition:center";
            string sCap = "Ini adalah sesuatu wineskin.";
            string sCapBT = "This is a wineskin.";

            // Create a picture. The ID will be automatically set to 0.
            var pictureIn = new DPicture();
            pictureIn.PathName = sPath;
            pictureIn.WordRtfInfo = sRtf;
            pictureIn.SimpleText = sCap;
            pictureIn.SimpleTextBT = sCapBT;
            Section.Paragraphs.Append(pictureIn);

            // Save it to an xml node
            var nodePicture = pictureIn.SaveToOxesBook(oxes, nodeBook);

            // Create a new picture from that node
            var pictureOut = DPicture.CreatePicture(nodePicture);

            // Should be identical
            Assert.AreEqual(sPath, pictureOut.PathName);
            Assert.AreEqual(sRtf, pictureOut.WordRtfInfo);
            Assert.AreEqual(sCap, pictureOut.SimpleText);
            Assert.AreEqual(sCapBT, pictureOut.SimpleTextBT);
            Assert.AreEqual(0, pictureOut.ID);
            Assert.IsTrue(pictureOut.ContentEquals(pictureIn), "Pictures are the same.");
        }
        #endregion
    }
}
