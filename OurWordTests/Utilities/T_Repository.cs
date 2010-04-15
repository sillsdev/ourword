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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chorus.merge;
using Chorus.Utilities;
using Chorus.VcsDrivers.Mercurial;
using NUnit.Framework;

using JWTools;
using OurWordData.DataModel;
using OurWordData.Synchronize;

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
            file1.CreateAndWrite("");

            // Create another file a folder lower
            const string sFile2 = "hello.owt";
            const string sFolder2 = ".Settings";
            var sFolderFile2 = sFolder2 + Path.DirectorySeparatorChar + sFile2;
            var file2 = new TestFile(sFolder2, sFile2);
            file2.CreateAndWrite("");

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
            file1.CreateAndWrite("");
            var file2 = new TestFile(".Settings", "hello.owt");
            file2.CreateAndWrite("");
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
            var clonedRepository = new LocalRepository(clonedRepositoryPath);
            clonedRepository.CloneFrom(originRepository);

            // Add a new file to the original repo
            var file = new TestFile("origin", filename);
            file.CreateAndWrite("");
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
    public class TestLocalRepository : LocalRepository
    {
        #region Constructor()
        public TestLocalRepository() : 
            base(TestFolder.RootFolderPath)
        {
            // As a result, "this" can be a LocalRepository should we want
            // to test in that manner.
        }
        #endregion
        #region SMethod: void CreateSimpleFile(sPath, sOneLinerContent)
        static void CreateSimpleFile(string sPath, string sOneLinerContent)
        {
            var w = new StreamWriter(sPath, false, Encoding.UTF8);
            var tw = TextWriter.Synchronized(w);
            tw.WriteLine(sOneLinerContent);
            tw.Close();
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
        #region Test: TCloneFrom
        [Test] public void TCloneFrom()
        {
            TestFolder.CreateEmpty();

            var originRepositoryFolder = TestFolder.CreateEmptySubFolder("origin");
            var originRepository = new LocalRepository(originRepositoryFolder);
            originRepository.CreateIfDoesntExist();

            var clonedRepositoryFolder = TestFolder.CreateEmptySubFolder("cloned");
            var clonedRepository = new LocalRepository(clonedRepositoryFolder);
            clonedRepository.CloneFrom(originRepository);

            Assert.IsTrue(clonedRepository.Exists);

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TVersionTagIsCreated
        [Test] public void TVersionTagIsCreated()
        {
            TestFolder.CreateEmpty();
            CreateIfDoesntExist();

            var version = GetOurWordVersion();

            Assert.AreNotEqual(0, version, "Should be nonzero on a create");

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TUpdateVersionTag
        [Test] public void TUpdateVersionTag()
        {
            var folder = TestFolder.CreateEmpty();
            var repo = new LocalRepository(folder);
            repo.CreateIfDoesntExist();

            // Remove the tag that we just inserted
            var removeCommand = string.Format("tag --remove {0}", 
                SurroundWithQuotes(TagContents));
            DoCommand(removeCommand);

            var version = repo.GetOurWordVersion();
            Assert.AreEqual(0, version, "Should be zero following removal");

            repo.UpdateVersionTag();

            Assert.AreEqual(c_nCurrentVersionNo, repo.GetOurWordVersion(),
                "Version should be " + c_nCurrentVersionNo);
        }
        #endregion
        #region Test: TBackupFileNotOverwritten
        [Test] public void TBackupFileNotOverwritten()
        {
            try
            {
                TestFolder.CreateEmpty();

                var originRepositoryFolder = TestFolder.CreateEmptySubFolder("origin");
                var originRepository = new LocalRepository(originRepositoryFolder);
                originRepository.CreateIfDoesntExist();

                // Add a file, put it in our repository
                var newFilePath = Path.Combine(originRepository.FullPathToRepositoryRoot, "hello.txt");
                CreateSimpleFile(newFilePath, "Hello");
                originRepository.CommitChangedFiles("jsw", "committing hello.txt");

                // Create the backup
                var backupRepository = new BackupRepository(originRepository);
                backupRepository.SafeDelete();
                backupRepository.MakeOrUpdateBackup();

                // Add another file, but don't put it in our repository
                newFilePath = Path.Combine(originRepository.FullPathToRepositoryRoot, "world.txt");
                CreateSimpleFile(newFilePath, "World");

                // Corrupt our repository!
                var disasterFolder = Path.Combine(originRepository.FullPathToHgFolder, "store");
                var disasterPath = Path.Combine(disasterFolder, "fncache");
                Assert.IsTrue(File.Exists(disasterPath));
                File.Delete(disasterPath);
                Assert.IsTrue(originRepository.CheckIsCorruptRepository());

                // This should restore things
                backupRepository.CheckSourceForErrors();

                // Test 1: Does out Hello file still exist?
                Assert.IsTrue(File.Exists(newFilePath), "The uncommitted file world.txt should still exist.");

                // Test 2: Is our repository no longer corrupt?
                Assert.IsFalse(originRepository.CheckIsCorruptRepository(), "The new repo should not be corrupt.");

                // Cleanup
                backupRepository.SafeDelete();
                TestFolder.DeleteIfExists();

            }
            catch (Exception e)
            {
                Assert.Fail("An error was thrown: ", e.Message);
            }
        }
        #endregion
        #region Test: TCorruptRepository
        [Test] public void TCorruptRepository()
        {
            TestFolder.CreateEmpty();

            // Create a repository
            var repositoryFolder = TestFolder.CreateEmptySubFolder("origin");
            var repository = new LocalRepository(repositoryFolder);
            repository.CreateIfDoesntExist();

            // Should be in good shape
            Assert.IsFalse(repository.CheckIsCorruptRepository());

            // Corrupt it
            var disasterFolder = Path.Combine(repository.FullPathToHgFolder, "store");
            var disasterPath = Path.Combine(disasterFolder, "fncache");
            Assert.IsTrue(File.Exists(disasterPath));
            File.Delete(disasterPath);

            // Should be in bad shape
            Assert.IsTrue(repository.CheckIsCorruptRepository());

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
        [SetUp] public void Setup()
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
            (new BackupRepository(originRepository)).SafeDelete();
            WriteAndCommitFile(originRepository, sFileBaseName, parentDoc, "Parent created");

            // Clone it to another repository; thus we have "parent" in two places
            var clonedRepositoryPath = GetFreshRepositoryRootPath("cloned");
            var clonedRepository = new LocalRepository(clonedRepositoryPath);
            (new BackupRepository(clonedRepository)).SafeDelete();
            clonedRepository.CloneFrom(originRepository);

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
            (new BackupRepository(originRepository)).SafeDelete();
            (new BackupRepository(clonedRepository)).SafeDelete();
        }
        #endregion
        #region Test: DetectAndRecoverFromLaterVersion
        [Test] public void DetectAndRecoverFromLaterVersion()
        {
            TestFolder.CreateEmpty();

            // Create a repositiory with data version "0"
            var folderLocal = TestFolder.CreateEmptySubFolder("local");
            var repoLocal = new LocalRepository(folderLocal);
            repoLocal.CreateIfDoesntExist();
            var removeCommand = string.Format("tag --remove \"{0}\"", 
                LocalRepository.TagContents);
            repoLocal.DoCommand(removeCommand);

            // Clone it to "other"
            var folderOther = TestFolder.CreateEmptySubFolder("other");
            var repoOther = new LocalRepository(folderOther);
            repoOther.CloneFrom(repoLocal);

            // In the "Other", update the version
            repoOther.UpdateVersionTag();
            Assert.IsTrue(0 == repoLocal.GetOurWordVersion());
            Assert.IsTrue(0 < repoOther.GetOurWordVersion());

            // Attempt the pull; local's version should be less than other's.
            try
            {
                var synch = new Synchronize(repoLocal, repoOther, "jsw");
                synch.PullNewerFiles();

                Assert.IsTrue(false, "Exception in PullNewerFiles shouldl tell the user to upgrade");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("upgrade"));
            }

            // Recover should remove the pull
            repoLocal.Rollback();
            Assert.IsTrue(0 == repoLocal.GetOurWordVersion());

            TestFolder.DeleteIfExists();
        }
        #endregion
        #region Test: TestRollbackUponMergeFailure
        [Test] public void TestRollbackUponMergeFailure()
        {
            TestFolder.CreateEmpty();
            const string filename = "01 GEN - Pura.MustFail.oxes";

            // Create a original repository
            var folderLocal = TestFolder.CreateEmptySubFolder("original");
            var repoLocal = new LocalRepository(folderLocal);
            (new BackupRepository(repoLocal)).SafeDelete();
            repoLocal.CreateIfDoesntExist();

            // Add a test file to it
            var fileLocal = new TestFile(folderLocal, filename);
            fileLocal.CreateAndWrite("");
            repoLocal.CommitChangedFiles("jsw", "Created PuraGEN");

            // Clone it to "other"
            var folderOther = TestFolder.CreateEmptySubFolder("other");
            var repoOther = new LocalRepository(folderOther);
            (new BackupRepository(repoOther)).SafeDelete();
            repoOther.CloneFrom(repoLocal);

            // Change the original to "one"
            fileLocal.CreateAndWrite("one");

            // Change the "other" to "two" and commit it so that it will pull
            var fileOther = new TestFile(folderOther, filename);
            fileOther.CreateAndWrite("two");
            repoOther.CommitChangedFiles("jsw", "Changed PuraGEN");

            // Attempt the merge. We should get an Exception saying the merge
            // could not be done. (As our files are not really oxes files)
            EnumeratedStepsProgressDlg.DisableForTesting = true;
            var synch = new Synchronize(repoLocal, repoOther, "jsw");
            var success = synch.SynchLocalToOther();
            Assert.IsFalse(success, "Synch #1 should have failed.");

            // The file in our working directory should still be "one"
            var vsContents = fileLocal.Read();
            Assert.AreEqual(1, vsContents.Count);
            Assert.AreEqual("one", vsContents[0]);

            // There should be exactly one head in the repository
            var heads = (new HgRepository(folderLocal, new NullProgress()).GetHeads());
            Assert.AreEqual(1, heads.Count);

            // Now change our local file to "one++"
            fileLocal.CreateAndWrite("one++");

            // Do another failed synch
            success = synch.SynchLocalToOther();
            Assert.IsFalse(success, "Synch #2 should have failed.");

            // Our local file should still be "one++"
            vsContents = fileLocal.Read();
            Assert.AreEqual(1, vsContents.Count);
            Assert.AreEqual("one++", vsContents[0]);

            // There should be exactly one head in the repository
            heads = (new HgRepository(folderLocal, new NullProgress()).GetHeads());
            Assert.AreEqual(1, heads.Count);

            // Cleannup
            (new BackupRepository(repoLocal)).SafeDelete();
            (new BackupRepository(repoOther)).SafeDelete();
        }
        #endregion

    }

}
