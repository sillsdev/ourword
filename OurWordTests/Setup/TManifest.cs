#region ***** TManifest.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TManifest.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Unit tests for Manifest and ManifestItem
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using JWTools;
using NUnit.Framework;
using OurWordSetup.Data;

#endregion

namespace OurWordTests.Setup
{
    [TestFixture]
    public class TManifestItem : ManifestItem
    {
        // I/O -------------------------------------------------------------------------------
        private const string c_sXmlForIoTest = "<Item filename=\"OurWordData.dll\" " +
            "destination=\"{app}\" length=\"2534\" hash=\"42\" />";
        #region Test: TSave
        [Test] public void TSave()
        {
            var item = new ManifestItem()
            {
                Filename = "OurWordData.dll",
                Destination = "{app}",
                Length = 2534L,
                Hash = "42"
            };

            var tf = new TestFile("test", "XmlDocWriterTest.xml");
            var doc = new XDoc(tf.FullPath);
            var node = item.Save(doc, null);

            Assert.AreEqual(c_sXmlForIoTest, node.OuterXml);
        }
        #endregion
        #region Test: TCreate
        [Test] public void TCreate()
        {
            var doc = new XmlDoc(c_sXmlForIoTest);
            var node = XmlDoc.FindNode(doc, "Item");
            var item = Create(node);

            Assert.AreEqual("OurWordData.dll", item.Filename);
            Assert.AreEqual("{app}", item.Destination);
            Assert.AreEqual(2534L, item.Length);
            Assert.AreEqual("42", item.Hash);
        }
        #endregion
    }


    [TestFixture]
    public class TManifest : Manifest
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Constructor()
        public TManifest()
            : base((new TestFile("manifest", "OurWordManifest.xml")).FullPath)
        { }
        #endregion

        #region Test: IO
        [Test] public void IO()
        {
            const string c_sXml = 
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<Manifest major=\"1\" minor=\"6\" build=\"3\">\r\n" +
                "  <Item filename=\"OurWord.exe\" destination=\"{app}\" hash=\"43\" />\r\n" +
                "  <Item filename=\"OurWordData.dll\" destination=\"{app}\" hash=\"44\" />\r\n" +
                "  <Item filename=\"JWTools.dll\" destination=\"{app}\" hash=\"45\" />\r\n" +
                "</Manifest>";

            // Set up a manifest
            Major = 1;
            Minor = 6;
            Build = 3;
            Add(new ManifestItem() 
            { 
                Filename = "OurWord.exe", 
                Destination="{app}", 
                Hash = "43"
            });
            Add(new ManifestItem()
            {
                Filename = "OurWordData.dll",
                Destination = "{app}",
                Hash = "44"
            });
            Add(new ManifestItem()
            {
                Filename = "JWTools.dll",
                Destination = "{app}",
                Hash = "45"
            });
            Assert.AreEqual(3, Count);

            // Save it to disk
            Save();

            // Zero out all the data
            Clear();
            Assert.AreEqual(0, Count);

            // Read the file back in
            var sXmlActual = ReadFile();

            // Get what we expected: xml string?
            Assert.AreEqual(c_sXml, sXmlActual);

            // Get what we expected: values?
            Assert.AreEqual(3, Count);
            Assert.AreEqual("OurWord.exe", this[0].Filename);
            Assert.AreEqual("JWTools.dll", this[2].Filename);
            Assert.AreEqual(1, Major);
            Assert.AreEqual(6, Minor);
            Assert.AreEqual(3, Build);
        }
        #endregion        

        #region Test: TContainsFile
        [Test] public void TContainsFile()
        {
            Add(new ManifestItem() { Filename = "OurWord.exe", Destination = "{app}" });
            Add(new ManifestItem() { Filename = "OurWordData.dll", Destination = "{app}" });
            Add(new ManifestItem() { Filename = "JWTools.dll", Destination = "{app}" });

            Assert.IsTrue(ContainsFile("JWTools.dll"));
            Assert.IsFalse(ContainsFile("Cheese.dll"));

            Assert.IsTrue(ContainsFile("c:\\users\\appdata\\JWTools.dll"));
        }
        #endregion
    }
}
