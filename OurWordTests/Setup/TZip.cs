#region ***** TZip.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Zip.cs
 * Author:  John Wimbish
 * Created: 2 Feb 2010
 * Purpose: Tests the Zip class
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using OurWordSetup.Data;
#endregion

namespace OurWordTests.Setup
{
    [TestFixture]
    public class TZip
    {
        // Setup -----------------------------------------------------------------------------
        // Temporary folder for testing
        #region SAttr{g}: string TemporaryFolder
        static string TemporaryFolder
        {
            get
            {
                const string c_sFolder = "ziptest";
                var sMyDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var sTempFolder = Path.Combine(sMyDocs, c_sFolder);
                return sTempFolder;
            }
        }
        #endregion
        #region SMethod: CreateEmptyTemporaryFolder()
        static void CreateEmptyTemporaryFolder()
        {
            DeleteTemporaryFolder();
            Directory.CreateDirectory(TemporaryFolder);
        }
        #endregion
        #region SMethod: void DeleteTemporaryFolder()
        static void DeleteTemporaryFolder()
        {
            if (Directory.Exists(TemporaryFolder))
                Directory.Delete(TemporaryFolder, true);
        }
        #endregion
        #region TesrDown() - remove the temp folder
        [TearDown]
        public void TearDown()
        {
            DeleteTemporaryFolder();
        }
        #endregion

        // Temporary archive for testing
        #region SAttr{g}: List<string> s_vsZipShortPaths

        private const string s_ShortPathMark = "Mark.oxes";

        private static readonly string s_ShortPathLakhota = 
            "Sioux" + Path.DirectorySeparatorChar +
            "Lakhota" + Path.DirectorySeparatorChar + "Luke 42.oxes";

        static private readonly List<string> s_vsZipShortPaths = new List<string> 
        { 
            ".Settings" + Path.DirectorySeparatorChar + "English.otrans",
            ".Pictures" + Path.DirectorySeparatorChar + "Wineskin.jpg",
            s_ShortPathMark,
            s_ShortPathLakhota,
            "Cheeze.xml"
        };
        #endregion
        #region SMethod: string CreateRootedArchivePath(sZipShortPath)
        static string CreateRootedArchivePath(string sZipShortPath)
        {
            return Path.Combine(TemporaryFolder, sZipShortPath);
        }
        #endregion
        #region SMethod: List<string> CreatedRootedArchivePaths(vsZipShortPaths)
        static List<string> CreatedRootedArchivePaths(IEnumerable<string> vsZipShortPaths)
        {
            var v = new List<string>();
            foreach (var s in vsZipShortPaths)
            {
                var sFullPath = CreateRootedArchivePath(s);
                v.Add(sFullPath);
            }
            return v;
        }
        #endregion
        #region SMethod: string CreateArchiveableFileContent(sFullPath)
        static string CreateArchiveableFileContent(string sFullPath)
        {
            return string.Format("<file>{0}</file>", sFullPath);
        }
        #endregion
        #region SMethod: void CreateArchivableFiles(vsFullPaths)
        static void CreateArchivableFiles(IEnumerable<string> vsFullPaths)
        {
            foreach (var sPath in vsFullPaths)
            {
                // Make sure the folder exists
                var sFolder = Path.GetDirectoryName(sPath);
                if (!Directory.Exists(sFolder))
                    Directory.CreateDirectory(sFolder);
               
                // Create and write out the file with some minimal text content
                var sContents = CreateArchiveableFileContent(sPath);
                File.WriteAllText(sPath, sContents);
            }
        }
        #endregion
        #region SMethod: void DeleteArchiveableFiles(vsFullPaths)
        static void DeleteArchiveableFiles(IEnumerable<string> vsFullPaths)
        {
            foreach (var sFile in vsFullPaths)
            {
                var sPath = Path.Combine(TemporaryFolder, sFile);
                if (File.Exists(sPath))
                    File.Delete(sPath);
            }
        }
        #endregion
        #region SMethod: bool CompareContents(string sFullPath)
        static bool CompareContents(string sFullPath)
        {
            var sContentsActual = File.ReadAllText(sFullPath);
            return (CreateArchiveableFileContent(sFullPath) == sContentsActual);
        }
        #endregion
        #region SMethod: void CreateZipArchive(vsFullPaths)
        static Zip CreateZipArchive(string sZipPath, List<string> vsFullPaths)
        {
            var zip = new Zip(sZipPath);
            CreateArchivableFiles(vsFullPaths);
            zip.Create(TemporaryFolder, vsFullPaths);
            DeleteArchiveableFiles(vsFullPaths);
            return zip;
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: TMakeUnixRelativePath
        [Test] public void TMakeUnixRelativePath()
        {
            var sRel = Zip.MakeUnixRelativePath("C:\\Users\\Me\\Temp", 
                "C:\\Users\\Me\\Temp\\Sioux\\Lakhota\\file.zip");
            Assert.AreEqual("Sioux/Lakhota/file.zip", sRel);
        }
        #endregion
        #region Test: TOutputFolder
        [Test] public void TOutputFolder()
        {
            var sApplicationFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            var sOurWordInstallFolder = Path.Combine(sApplicationFolder, "OurWord");

            // We expect ...\Local\OurWord\Loc\
            var zip = new Zip("app.Loc.Localizations.zip");
            var sExpectedFolder = Path.Combine(sOurWordInstallFolder, "Loc");
            Assert.AreEqual(sExpectedFolder, zip.OutputFolder);

            // We expect ...\Local\OurWord
            zip = new Zip("app.Palaso.zip");
            Assert.AreEqual(sOurWordInstallFolder, zip.OutputFolder);

            // We expect ...\Documents\SampleData\Navajo\settings.zip
            zip = new Zip("mydocs.SampleData.Navajo.settings.zip");
            var sMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sExpectedFolder = Path.Combine(sMyDocumentsFolder, 
                "SampleData" + Path.DirectorySeparatorChar +
                "Navajo");
            Assert.AreEqual(sExpectedFolder, zip.OutputFolder);

            // We expect ...\Local\Language Data\OurWordSample
            zip = new Zip("langdata.OurWordSample.Sample.zip");
            sExpectedFolder = Path.Combine(sApplicationFolder,
               "Language Data" + Path.DirectorySeparatorChar +
               "OurWordSample");
            Assert.AreEqual(sExpectedFolder, zip.OutputFolder);
        }
        #endregion
        #region Test: TCreateZipThenGetFileNames
        [Test] public void TCreateZipThenGetFileNames()
            // The zip library follows a unix convention of forward slashes, e.g.,
            //   ".Settings/English.otrans"
            // Even though the zip file was created on a Windows machine.
            // So what we want is for the / or \ to reflect the OS.
            //
            // For this test to succeed, zip.Create must also succeed; so this
            // is a test of the Create method as well.
        {
            // Create the zip file in our temporary folder
            CreateEmptyTemporaryFolder();
            var sZipPath = Path.Combine(TemporaryFolder, "ziptest.Test.zip");
            CreateZipArchive(sZipPath, CreatedRootedArchivePaths(s_vsZipShortPaths));

            // Open a new zip object on our archive, and retrieve the filenames
            var zip = new Zip(sZipPath);
            var vFiles = zip.GetFileNames();

            // See if we have our list of filenames
            Assert.AreEqual(5, vFiles.Count);
            foreach (var s in s_vsZipShortPaths)
                Assert.IsTrue(vFiles.Contains(s));

            DeleteTemporaryFolder();
        }
        #endregion
        #region Test: TExtract
        [Test] public void TExtract()
            // This test proves that round-tripping of actual data that is zipped works.
            // The test will fail if both Create and Extract do not work.
        {
            // Create the zip file in our temporary folder
            CreateEmptyTemporaryFolder();
            var sZipPath = Path.Combine(TemporaryFolder, "ziptest.Test.zip");
            CreateZipArchive(sZipPath, CreatedRootedArchivePaths(s_vsZipShortPaths));
            // The zip should be the only file there
            Assert.AreEqual(1, Directory.GetFiles(TemporaryFolder).Length);

            // Open a new zip object on our archive, and extract the files
            var zip = new Zip(sZipPath);
            zip.Extract();
            var vFiles = zip.GetFileNames();

            // We should have five files whose contents are unchanged
            Assert.AreEqual(5, vFiles.Count);
            foreach (var sPath in CreatedRootedArchivePaths(s_vsZipShortPaths))
                Assert.IsTrue(CompareContents(sPath));

            DeleteTemporaryFolder();
        }
        #endregion
        #region Test: public void TGetFullPathNames()
        [Test] public void TGetFullPathNames()
        {
            // Create the zip file in our temporary folder
            CreateEmptyTemporaryFolder();
            var sZipPath = Path.Combine(TemporaryFolder, "ziptest.Test.zip");
            CreateZipArchive(sZipPath, CreatedRootedArchivePaths(s_vsZipShortPaths));

            // Open a new zip object on our archive, and retrieve the pathnames
            var zip = new Zip(sZipPath);
            var vFiles = zip.GetFullPathNames();

            // See if we have our list of pathnames
            Assert.AreEqual(5, vFiles.Count);
            foreach (var s in s_vsZipShortPaths)
            {
                var sPath = Path.Combine(TemporaryFolder, s);
                Assert.IsTrue(vFiles.Contains(sPath));
            }

            DeleteTemporaryFolder();
        }
        #endregion
        #region Test: TGetFullPathNamesOfAdditionalFiles
        [Test] public void TGetFullPathNamesOfAdditionalFiles()
        {
            // Create the zip file in our temporary folder
            CreateEmptyTemporaryFolder();
            var sSupersetZipPath = Path.Combine(TemporaryFolder, "ziptest.Superset.zip");
            var zipSuperset = CreateZipArchive(sSupersetZipPath, 
                CreatedRootedArchivePaths(s_vsZipShortPaths));

            // Create another zipfile that has fewer files
            var vsSubset = new List<string>();
            foreach(var s in s_vsZipShortPaths)
                vsSubset.Add(s);
            vsSubset.Remove(s_ShortPathMark);
            vsSubset.Remove(s_ShortPathLakhota);
            var sSubsetZipPath = Path.Combine(TemporaryFolder, "ziptest.Subset.zip");
            var zipSubset = CreateZipArchive(sSubsetZipPath,
                CreatedRootedArchivePaths(vsSubset));

            // Perforn the function
            var vsExtraFiles = zipSubset.GetFullPathNamesOfAdditionalFiles(zipSuperset);

            // Evaluate: Should have exactly two files; our removed ones
            Assert.AreEqual(2, vsExtraFiles.Count);
            Assert.IsTrue(vsExtraFiles.Contains(CreateRootedArchivePath(s_ShortPathMark)));
            Assert.IsTrue(vsExtraFiles.Contains(CreateRootedArchivePath(s_ShortPathLakhota)));

            DeleteTemporaryFolder();
        }
        #endregion
    }
}
