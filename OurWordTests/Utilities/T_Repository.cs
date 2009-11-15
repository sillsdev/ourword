#region ***** T_Repository.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Repository.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the Repository class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System.Collections.Generic;
using System.IO;
using Chorus.merge;
using Chorus.Utilities;
using NUnit.Framework;

using JWTools;
using OurWordData.DataModel;
#endregion
#endregion

namespace OurWordTests.Utilities
{
    [TestFixture]
    public class TestRepository : Repository
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: TStripTrailingPathSeparator
        [Test] public void TStripTrailingPathSeparator()
        {
            // Build a path in a OS-independant manner
            var vFolders = new[] { "Users", "Albert", "My Documents", "Inuit" };
            var pathWithoutTrailingSeparator = "C:";
            foreach (var sFolder in vFolders)
                pathWithoutTrailingSeparator = Path.Combine(pathWithoutTrailingSeparator, sFolder);

            var pathWithTrailingSeparator = pathWithoutTrailingSeparator +
                Path.DirectorySeparatorChar;

            // Make sure that our two paths do indeed differ by a trailing character
            Assert.AreEqual(pathWithoutTrailingSeparator.Length + 1,
                pathWithTrailingSeparator.Length);
            Assert.AreEqual(pathWithTrailingSeparator[pathWithTrailingSeparator.Length - 1],
                Path.DirectorySeparatorChar);

            // Do we strip out the trailing separator if its there?
            Assert.AreEqual(pathWithoutTrailingSeparator,
                StripTrailingPathSeparator(pathWithTrailingSeparator));

            // Do we do nothing if there's no trailing separator to begin with?
            Assert.AreEqual(pathWithoutTrailingSeparator,
                StripTrailingPathSeparator(pathWithoutTrailingSeparator));
        }
        #endregion
        #region Test: TCloneTo
        [Test] public void TCloneTo()
        {
            TestFolder.CreateEmpty();

            var originRepositoryFolder = TestFolder.CreateEmptySubFolder("origin");
            var originRepository = new LocalRepository(originRepositoryFolder);
            originRepository.CreateIfDoesntExist();

            var clonedRepositoryFolder = TestFolder.CreateEmptySubFolder("cloned");
            originRepository.CloneTo(clonedRepositoryFolder);
            var clonedRepository = new LocalRepository(clonedRepositoryFolder);

            Assert.IsTrue(clonedRepository.Exists);

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TCheckMercialIsInstalled
        [Test] public void TCheckMercialIsInstalled()
            // Not much of a test, if Hg is installed. A true test would try it
            // out both if installed and not installed.
            //   And yet, if the test fails, it means the underlying Chorus that
            // the method is calling has changed in how it operates.
        {
            // As a developer's machine, I expect Mercurial to be isntalled
            Assert.IsTrue( CheckMercialIsInstalled(), 
                "Hg should be installed");

            // If we remove the Path environment variable, the ability to run Hg
            // should fail, thus simulating a not-installed situation.
            using (new ShortTermEnvironmentalVariable("Path", ""))
            {
                Assert.IsFalse( CheckMercialIsInstalled(),
                    "Hg should seem to not be installed");
            }
        }
        #endregion
        #region Test: TGetChangedFiles
        [Test] public void TGetChangedFiles()
        {
            var path = TestFolder.CreateEmpty();
            var repository = new LocalRepository(path);
            repository.CreateIfDoesntExist();

            // Create a file at the repository root
            const string sFile1 = "AtRoot.oxes";
            var file1 = new TestFile(null, sFile1);
            file1.CreateAndWrite(null);

            // Create another file a folder lower
            const string sFile2 = "hello.owt";
            const string sFolder2 = ".Settings";
            var sFolderFile2 = sFolder2 + Path.DirectorySeparatorChar + sFile2;
            var file2 = new TestFile(sFolder2, sFile2);
            file2.CreateAndWrite(null);

            var vsFiles = repository.GetChangedFiles();

            // We should be able to find our two files
            Assert.AreEqual(2, vsFiles.Count);
            Assert.IsTrue(vsFiles.Contains(sFile1), "Missing " + sFile1);
            Assert.IsTrue(vsFiles.Contains(sFolderFile2), "Missing " + sFolderFile2);

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TCommitChangedFiles
        [Test] public void TCommitChangedFiles()
        {
            // Create the Repository
            TestFolder.CreateEmpty();
            var repository = new LocalRepository(TestFolder.RootFolderPath);
            repository.CreateIfDoesntExist();

            // Create a couple of files
            var file1 = new TestFile(null, "AtRoot.oxes");
            file1.CreateAndWrite(null);
            var file2 = new TestFile(".Settings", "hello.owt");
            file2.CreateAndWrite(null);
            Assert.AreEqual(2, repository.GetChangedFiles().Count,
                "should have two uncommitted files.");

            // Do the commit
            repository.CommitChangedFiles("jsw", "Created two files for test purposes");

            // Should have no files to commit
            Assert.AreEqual(0, repository.GetChangedFiles().Count,
                "should have no uncommitted files now.");
        }
        #endregion
        #region Test: TPushTo
        [Test] public void TPushTo()
        {
            TestFolder.CreateEmpty();

            const string filename = "hello.oxes";

            // Create an origin Repository
            var originRepositoryPath = TestFolder.CreateEmptySubFolder("origin");
            var originRepository = new LocalRepository(originRepositoryPath);
            originRepository.CreateIfDoesntExist();

            // Clone it
            var clonedRepositoryPath = TestFolder.CreateEmptySubFolder("cloned");
            originRepository.CloneTo(clonedRepositoryPath);
            var clonedRepository = new LocalRepository(clonedRepositoryPath);

            // Add a new file to the original repo
            var file = new TestFile("origin", filename);
            file.CreateAndWrite(null);
            originRepository.CommitChangedFiles("jsw", "Commiting new file to original.");

            // Verify the file exists in only the origin repo
            Assert.IsTrue(originRepository.GetFileExistsInRepo(filename), 
                "File should exist in origin");
            Assert.IsFalse(clonedRepository.GetFileExistsInRepo(filename), 
                "File should not exist in cloned");

            // Push it. The Update is required because GetFileExistsInRepo tests
            // for actual files, not stuff hidden in the .Hg folder.
            originRepository.PushTo(clonedRepositoryPath);
            clonedRepository.UpdateToCurrentBranchTip();

            // Verify file now exists in cloned
            Assert.IsTrue(clonedRepository.GetFileExistsInRepo(filename), 
                "File should exist in cloned");

            TestFolder.DeleteIfExists();
        }
        #endregion
    }

    [TestFixture]
    public class TestLocalRepository 
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: TCreateIfDoesntExist
        [Test] public void TCreateIfDoesntExist()
        {
            var repositoryRootFolder = TestFolder.CreateEmpty();
            var repository = new LocalRepository(repositoryRootFolder);
            repository.CreateIfDoesntExist();

            var hgFolder = Path.Combine(repositoryRootFolder, ".hg");
            Assert.IsTrue(Directory.Exists(hgFolder), "Should have a .hg folder.");

            TestFolder.DeleteIfExists();
        }
        #endregion
    }

    [TestFixture]
    public class TestInternetRepository
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: BuildUrlToInternetRepository
        [Test] public void BuildUrlToInternetRepository()
        {
            var repository = new InternetRepository("Cherokee")
            {
                UserName = "Harry",
                Password = "NoClue",
                Server = "hg-public.languagedepot.org"
            };

            var path = repository.FullPathToRepositoryRoot;

            // "cherokee" must be normalized to lower case to make Linux servers happy
            Assert.AreEqual("http://Harry:NoClue@hg-public.languagedepot.org/cherokee", path);
        }
        #endregion
        #region Test: StripLeadingHttpOnSettingServerValue
        [Test] public void StripLeadingHttpOnSettingServerValue()
        {
            var repository = new InternetRepository("Cherokee") 
            {
                Server = "http://bitbucket.org"
            };

            Assert.AreEqual("bitbucket.org", repository.Server);
        }
        #endregion
        #region Test: StripTrailingSlashOnSettingServerValue
        [Test] public void StripTrailingSlashOnSettingServerValue()
        {
            var repository = new InternetRepository("Cherokee") 
            {
                Server = "bitbucket.org/"
            };
            Assert.AreEqual("bitbucket.org", repository.Server);

            repository.Server = "bitbucket.org\\";
            Assert.AreEqual("bitbucket.org", repository.Server);
        }
        #endregion
    }

    [TestFixture]
    public class TestSynchronize
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        // Helper methods: Setup Xml Test Data
        #region static string CreateMessageStorageString(sAuthor, sDateTime, sContents)
        static string CreateMessageStorageString(string sAuthor, string sDateTime, string sContents)
        {
            return string.Format("<Message author=\"{0}\" created=\"{1}\">{2}</Message>", 
                sAuthor, sDateTime, sContents);
        }
        #endregion
        #region static XmlDoc CreateXmlDoc( IEnumerable<int> indicesOfMessagesToInclude )
        static XmlDoc CreateXmlDoc( IEnumerable<int> indicesOfMessagesToInclude )
        {
            var allXmlMessages = new[]
            {
                CreateMessageStorageString("John", "2009-02-28 09:14:18Z", "'Arono' is used twice in this verse, isn't that stylistically bad?"),
                CreateMessageStorageString("Larry", "2009-03-01 09:14:18Z", "Its a discourse feature. You obviously haven't read my paper on Yawa Discourse!"),
                CreateMessageStorageString("Linda", "2009-03-01 10:14:18Z", "I don't know, Larry, you're ok as an administrator, but why do you think I'm the one doing this final revision?"),
                CreateMessageStorageString("Larry", "2009-03-01 12:14:18Z", "Well, you've got a point"),
                CreateMessageStorageString("Owen", "2009-03-02 10:14:18Z", "Hey! Who's the MTT here, anyway? I say, Wimbish is right!"),
                CreateMessageStorageString("John", "2009-03-02 11:14:18Z", "Looks like my work here is done."),
                CreateMessageStorageString("Kathy", "2009-03-05 11:14:18Z", "But I'm the consultant, and I say, rubbish!")
            };

            var includedXmlMessages = "";
            foreach (var i in indicesOfMessagesToInclude)
                includedXmlMessages += allXmlMessages[i];

            var oxesScriptureParagraphs = new[]
            {
		        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>", 
		        "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
                "  <book id=\"MRK\" stage=\"Final\" version=\"A\">",
                "    <p class=\"Header\" usfm=\"h\">Mark</p>",
                "    <p class=\"Title Secondary\" usfm=\"mt2\">The Gospel Of</p>",
                "    <p class=\"Title Main\" usfm=\"mt\">Mark</p>",
                "    <p class=\"Section Head\" usfm=\"s1\">Usif Jesus naleta' neu fini le' in nesan an-ana' neis<bt>The Lord Jesus give an example of a seed that is extremely tiny</bt></p>",
                "    <p class=\"Parallel Passage Reference\" usfm=\"r\">(Mateus 13:31-32, 34; Lukas 13:18-19)</p>",
                "    <p class=\"Paragraph\" usfm=\"p\">" +
                        "<v n=\"30\" />" +
                        "Oke te, Jesus namolok antein, mnak, \"Au uleta' 'tein on ii: Hi nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, tal antee sin namfau nok." +
                        "<bt>Then Jesus spoke again, saying, \"I give another example like this: You(pl) can compare God's *social group. From just a few people, it nevertheless increases (in number), to the point that they are very many.</bt>" +
                        "<Annotation class=\"General\" selectedText=\"Arono\">" +
                            includedXmlMessages +
                        "</Annotation>" +
                        "</p>",
                "    <p class=\"Paragraph\" usfm=\"p\">" +
                        "<v n=\"31\" />" +
                        "Nane namnees onle' fini le' in nesan an-ana' neis." +
                        "<bt>That is like a seed that is very tiny.</bt>" +
                        "<v n=\"32\" />" +
                        "Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof kolo neem namin mafo', ma nmo'en kuna' neu ne.\"" +
                        "<bt>If we(inc) plant it (with dibble stick) it will grow to become a large tree. And birds will come looking for shade, and make nests in it.\"</bt>" +
                        "</p>",
                "    <p class=\"Paragraph\" usfm=\"p\">" +
                        "<v n=\"33\" />" +
                        "Jesus In na'noina' in ma'mo'en natuin sin hiin kini." +
                        "<bt>Jesus' way of teaching was according to their understanding.</bt>" +
                        "<v n=\"34\" />" +
                        "In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' sin, In natoon lais leta' nane in na' naon ok-oke'." +
                        "<bt>He taught people using only examples. But with His *disciples, He told the meaning of all the examples.</bt>" +
                        "</p>",
                "  </book>",
                "</bible>"
            };

            return new XmlDoc(oxesScriptureParagraphs);
        }
        #endregion

        // Helper methods: Files
        #region static string GetFreshRepositoryRootPath(repositoryName)
        static string GetFreshRepositoryRootPath(string repositoryName)
        {
            var sFolder = JWU.GetMyDocumentsFolder(null) + repositoryName;

            CleanupByEnsuringIsDeleted(sFolder);

            Directory.CreateDirectory(sFolder);

            return sFolder;
        }
        #endregion
        #region static void WriteAndCommitFile(repository, sFileBaseName, XmlDoc, message)
        static void WriteAndCommitFile(Repository repository, string sFileBaseName, XmlDoc doc, string message)
        {
            var path = repository.FullPathToRepositoryRoot + Path.DirectorySeparatorChar +
                       sFileBaseName;
            doc.Write(path);

            repository.CommitChangedFiles("jsw", message);
        }
        #endregion
        #region static void CleanupByEnsuringIsDeleted(string sFolder)
        static void CleanupByEnsuringIsDeleted(string sFolder)
        {
            if (Directory.Exists(sFolder))
                JWU.SafeFolderDelete(sFolder);
        }
        #endregion

        // Tests
        #region Test: TestVanilaMerge
        [Test] public void TestVanilaMerge()
            // Just tests the DBook.Merge method, not synch nor chorus
        {
            var parentDoc = CreateXmlDoc(new[] { 0, 1, 4 });
            var ourDoc = CreateXmlDoc(new[] { 0, 1, 2, 4, 5, 6 });
            var theirDoc = CreateXmlDoc(new[] { 0, 1, 2, 3, 4, 5 });
            var mergedDocExpected = CreateXmlDoc(new[] { 0, 1, 2, 3, 4, 5, 6 });

            var tempFolder = GetFreshRepositoryRootPath("MergeTest");

            var parentPath = tempFolder + Path.DirectorySeparatorChar + "41 MRK - parent.oxes";
            var ourPath = tempFolder + Path.DirectorySeparatorChar + "41 MRK - our.oxes";
            var theirPath = tempFolder + Path.DirectorySeparatorChar + "41 MRK - their.oxes";

            parentDoc.Write(parentPath);
            ourDoc.Write(ourPath);
            theirDoc.Write(theirPath);

            var mergeOrder = new MergeOrder(ourPath, parentPath, theirPath, null);
            DBook.Merge(mergeOrder);

            var mergeDocActual = new XmlDoc();
            mergeDocActual.Load(ourPath);
            Assert.IsTrue(XmlDoc.Compare(mergedDocExpected, mergeDocActual),
                "Merge should have given the expected result");

            CleanupByEnsuringIsDeleted(tempFolder);
        }
        #endregion
        #region Test: TestCompleteSynchMerge
        [Test] public void TestCompleteSynchMerge()
        {
            const string sFileBaseName = "41 MRK - Test.oxes";

            // We create minimal oxes books and parse them in. They only differ by the
            // content of one of the annotations, in terms of which messages the
            // annotation has. We're looking to test that the overall merge mechanism
            // works, as opposed to testing every possible merge permutation.
            //    Thus "ours" and "theirs" contain everything in "parent", plus additional
            // messages; and "mergedDoc" should contain the entire total of messages.
            var parentDoc = CreateXmlDoc(new[] { 0, 1,       4});
            var ourDoc    = CreateXmlDoc(new[] { 0, 1, 2,    4, 5, 6});
            var theirDoc  = CreateXmlDoc(new[] { 0, 1, 2, 3, 4, 5 });
            var mergedDocExpected = CreateXmlDoc(new[] { 0, 1, 2, 3, 4, 5, 6 });

            // Create an origin repository
            var originRepositoryPath = GetFreshRepositoryRootPath("origin");
            var originRepository = new LocalRepository(originRepositoryPath);
            originRepository.CreateIfDoesntExist();
            WriteAndCommitFile(originRepository, sFileBaseName, parentDoc, "Parent created");

            // Clone it to another repository; thus we have "parent" in two places
            var clonedRepositoryPath = GetFreshRepositoryRootPath("cloned");
            originRepository.CloneTo(clonedRepositoryPath);
            var clonedRepository = new LocalRepository(clonedRepositoryPath);

            // Update the repositories with "our" and "their" versions
            WriteAndCommitFile(originRepository, sFileBaseName, ourDoc, "Origin updated with ours");
            WriteAndCommitFile(clonedRepository, sFileBaseName, theirDoc, "Clone updated with theirs");

            // Do the synch; it will fail if ChorusMerge is not correctly hooked up
            EnumeratedStepsProgressDlg.DisableForTesting = true;
            var success = (new Synchronize(originRepository, clonedRepository, "jsw")).SynchLocalToOther();

            // Did we get what we expected?
            var mergeDocActual = new XmlDoc();
            mergeDocActual.Load(originRepositoryPath + Path.DirectorySeparatorChar + sFileBaseName);
            Assert.IsTrue(XmlDoc.Compare(mergedDocExpected, mergeDocActual),
                "Merge should have given the expected result");
            Assert.IsTrue(success, "Merge result was ok, but Do method didn't complete all steps successfully.");

            // Remove the repositories we created
            CleanupByEnsuringIsDeleted(originRepositoryPath);
            CleanupByEnsuringIsDeleted(clonedRepositoryPath);
        }
        #endregion
    }

}
