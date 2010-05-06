#region ***** TManifest.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TManifest.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Unit tests for Manifest and ManifestItem
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.IO;
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
            "length=\"2534\" hash=\"42\" />";
        #region Test: TSave
        [Test] public void TSave()
        {
            var item = new ManifestItem
            {
                Filename = "OurWordData.dll",
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
            Assert.AreEqual(2534L, item.Length);
            Assert.AreEqual("42", item.Hash);
        }
        #endregion
    }

    [TestFixture]
    public class TManifest : Manifest
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion
        #region Constructor()
        public TManifest()
            : base((new TestFile("manifest", "OurWordManifest.xml")).FullPath)
        {           
        }
        #endregion

        #region Test: IO
        [Test] public void IO()
        {
            const string c_sXml = 
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<Manifest major=\"1\" minor=\"6\" build=\"3\">\r\n" +
                "  <Item filename=\"OurWord.exe\" hash=\"43\" />\r\n" +
                "  <Item filename=\"OurWordData.dll\" hash=\"44\" />\r\n" +
                "  <Item filename=\"JWTools.dll\" hash=\"45\" />\r\n" +
                "</Manifest>";

            // Set up a manifest
            Major = 1;
            Minor = 6;
            Build = 3;
            Add(new ManifestItem
            { 
                Filename = "OurWord.exe", 
                Hash = "43"
            });
            Add(new ManifestItem
            {
                Filename = "OurWordData.dll",
                Hash = "44"
            });
            Add(new ManifestItem
            {
                Filename = "JWTools.dll",
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
        #region Test: TFindAndContainsFile
        [Test] public void TFindAndContainsFile()
        {
            Add(new ManifestItem { Filename = "OurWord.exe" });
            Add(new ManifestItem { Filename = "OurWordData.dll" });
            Add(new ManifestItem { Filename = "JWTools.dll" });

            Assert.IsTrue(ContainsFile("JWTools.dll"));
            Assert.IsFalse(ContainsFile("Cheese.dll"));

            // The method just looks for the filename, so prepending a folder
            // shouldn't make a difference
            var sPath = Path.Combine("C:", "users");
            sPath = Path.Combine(sPath, "appdata");
            sPath = Path.Combine(sPath, "JWTools.dll");
            Assert.IsTrue(ContainsFile(sPath));
        }
        #endregion
        #region Test: TComputeHashForFileContents
        [Test] public void TComputeHashForFileContents()
            
        {
            TestFolder.CreateEmpty();

            // Create a test file and compute its hash
            var file = new TestFile(null, "HashTest.txt");
            file.CreateAndWrite("Hello, World");
            var sHash = ComputeHashForFileContents(file.FullPath);
            const string c_sHashExpected = "00AC000A001900D800EB00EE008300520020005E00F2008E007D00BB00F600FB009E00C1006A00AF";

            // Should be equal to what we expect
            Assert.AreEqual(c_sHashExpected, sHash);

            // Create a slightly different test file and compute its hash
            file.CreateAndWrite("Hello, world");
            sHash = ComputeHashForFileContents(file.FullPath);

            // Should be different
            Assert.AreNotEqual(c_sHashExpected, sHash);

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TBuildFromFolderContents
        [Test] public void TBuildFromFolderContents()
            // Tests that files are placed into the manifest
        {
            TestFolder.CreateEmpty();

            var file1 = new TestFile(null, "HelloWorld.txt");
            file1.CreateAndWrite("Hello, World");

            var file2 = new TestFile(null, "Shakespeare.txt");
            file2.CreateAndWrite("To be or not to be");

            BuildFromFolderContents(TestFolder.RootFolderPath);

            var manifsetFile = Path.Combine(TestFolder.RootFolderPath, ManifestFileName);
            var manifest = new Manifest(manifsetFile);
            manifest.ReadFile();

            Assert.AreEqual(2, manifest.Count);
            Assert.IsTrue(manifest.ContainsFile("HelloWorld.txt"));
            Assert.IsTrue(manifest.ContainsFile("Shakespeare.txt"));

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TGetStaleFiles
        [Test] public void TGetStaleFiles()
        {
            var manifestRemote = new Manifest("remote")
            {
                new ManifestItem {Filename = "peperroni.xml", Length = 12},
                new ManifestItem {Filename = "sausage.xml", Length = 13},
                new ManifestItem {Filename = "pineapple.xml", Length = 42}
            };

            var manifestLocal = new Manifest("local")
            {
                new ManifestItem {Filename = "peperroni.xml", Length = 12},
                new ManifestItem {Filename = "onion.xml", Length = 13},
                new ManifestItem {Filename = "sausage.xml", Length = 13},
                new ManifestItem {Filename = "pineapple.xml", Length = 42},
                new ManifestItem {Filename = "bacon.xml", Length=14}
            };

            var vsObsoleteFileNames = manifestRemote.GetStaleFiles(manifestLocal);

            Assert.AreEqual(2, vsObsoleteFileNames.Count);
            Assert.AreEqual( Path.Combine("local", "onion.xml"), vsObsoleteFileNames[0]);
            Assert.AreEqual( Path.Combine("local", "bacon.xml"), vsObsoleteFileNames[1]);
        }
        #endregion
    }
}
