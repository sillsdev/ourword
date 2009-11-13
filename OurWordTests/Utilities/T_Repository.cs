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
using NUnit.Framework;

using JWTools;
using OurWordData;
using OurWordData.DataModel;
#endregion
#endregion

namespace OurWordTests.Utilities
{
    [TestFixture]
    public class TestHgRepositoryBase
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: StripTrailingPathSeparator
        [Test] public void StripTrailingPathSeparator()
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
                HgRepositoryBase.StripTrailingPathSeparator(pathWithTrailingSeparator));

            // Do we do nothing if there's no trailing separator to begin with?
            Assert.AreEqual(pathWithoutTrailingSeparator,
                HgRepositoryBase.StripTrailingPathSeparator(pathWithoutTrailingSeparator));
        }
        #endregion
        #region Test: CloneTo
        [Test] public void CloneTo()
        {
            var originRepositoryFolder = JWU.NUnit_GetFreshTempSubFolder("origin");
            var originRepository = new HgLocalRepository(originRepositoryFolder);
            originRepository.CreateIfDoesntExist();

            var clonedRepositoryFolder = JWU.NUnit_GetFreshTempSubFolder("cloned");
            originRepository.CloneTo(clonedRepositoryFolder);
            var clonedRepository = new HgLocalRepository(clonedRepositoryFolder);

            Assert.IsTrue(clonedRepository.Exists);
        }
        #endregion
    }

    [TestFixture]
    public class TestHgLocalRepository
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: CreateIfDoesntExist
        [Test] public void CreateIfDoesntExist()
        {
            var repositoryRootFolder = JWU.NUnit_GetFreshTempSubFolder("repository");

            Directory.CreateDirectory(repositoryRootFolder);
            var repository = new HgLocalRepository(repositoryRootFolder);
            repository.CreateIfDoesntExist();

            var hgFolder = repositoryRootFolder + Path.DirectorySeparatorChar + ".hg";
            Assert.IsTrue(Directory.Exists(hgFolder), "Should have a .hg folder.");
        }
        #endregion
    }

    [TestFixture]
    public class TestHgInternetRepository
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: BuildUrlToInternetRepository
        [Test] public void BuildUrlToInternetRepository()
        {
            var repository = new HgInternetRepository("Cherokee");
            HgInternetRepository.UserName = "Harry";
            HgInternetRepository.Password = "NoClue";
            HgInternetRepository.Server = "hg-public.languagedepot.org";

            var path = repository.FullPathToRepositoryRoot;

            // "cherokee" should be normalized to lower case to make Linux servers happy
            Assert.AreEqual("http://Harry:NoClue@hg-public.languagedepot.org/cherokee", path);
        }
        #endregion
        #region Test: StripLeadingHttpOnSettingServerValue
        [Test] public void StripLeadingHttpOnSettingServerValue()
        {
            var repository = new HgInternetRepository("Cherokee");
            HgInternetRepository.Server = "http://bitbucket.org";

            Assert.AreEqual("bitbucket.org", HgInternetRepository.Server);
        }
        #endregion
        #region Test: StripTrailingSlashOnSettingServerValue
        [Test] public void StripTrailingSlashOnSettingServerValue()
        {
            var repository = new HgInternetRepository("Cherokee");

            HgInternetRepository.Server = "bitbucket.org/";
            Assert.AreEqual("bitbucket.org", HgInternetRepository.Server);

            HgInternetRepository.Server = "bitbucket.org\\";
            Assert.AreEqual("bitbucket.org", HgInternetRepository.Server);
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
            JWU.NUnit_Setup();
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
        static void WriteAndCommitFile(HgRepositoryBase repository, string sFileBaseName, XmlDoc doc, string message)
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
            var originRepository = new HgLocalRepository(originRepositoryPath);
            originRepository.CreateIfDoesntExist();
            WriteAndCommitFile(originRepository, sFileBaseName, parentDoc, "Parent created");

            // Clone it to another repository; thus we have "parent" in two places
            var clonedRepositoryPath = GetFreshRepositoryRootPath("cloned");
            originRepository.CloneTo(clonedRepositoryPath);
            var clonedRepository = new HgLocalRepository(clonedRepositoryPath);

            // Update the repositories with "our" and "their" versions
            WriteAndCommitFile(originRepository, sFileBaseName, ourDoc, "Origin updated with ours");
            WriteAndCommitFile(clonedRepository, sFileBaseName, theirDoc, "Clone updated with theirs");

            // Do the synch; it will fail if ChorusMerge is not correctly hooked up
            EnumeratedStepsProgressDlg.DisableForTesting = true;
            var success = (new Synchronize(originRepository, clonedRepository, "jsw")).Do();

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


    [TestFixture] 
    public class T_Repository
    {
        // Setup/Teardown --------------------------------------------------------------------
        #region Attr{g}: Repository Repository
        Repository Repository
        {
            get
            {
                return DB.TeamSettings.Repository;
            }
        }
        #endregion
        #region VAttr{g}: string ClonePath - Another repo on my hard drive
        string ClonePath
        {
            get
            {
                return JWU.GetMyDocumentsFolder(null) + "Clone";

            }
        }
        #endregion

        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();

            // Get rid of any remnants of our Test Cluster, e.g., from debugging.
            JWU.NUnit_TeardownClusterFolder();

            // Create the empty cluster folder
            JWU.NUnit_SetupClusterFolder();

            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();
            DB.TeamSettings.Repository.Active = true;

            DB.Project.DisplayName = "RepositoryTestProject";

            DB.Project.TargetTranslation = new DTranslation("Test Translation", "Latin", "Latin");
            DBook book = new DBook("MRK");
            DB.Project.TargetTranslation.AddBook(book);
            DSection section = new DSection();
            book.Sections.Append(section);
            DB.Project.WriteToFile(new NullProgress());

        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            // Get rid of our Test Cluster
            JWU.NUnit_TeardownClusterFolder();

            // Remove the Clone/PushTo Cluster
            if (Directory.Exists(ClonePath))
                Directory.Delete(ClonePath, true);

            DB.Project = null;
        }
        #endregion

        // Remote test repository ------------------------------------------------------------
        #region VAttr{g}: string RemoteUrl - http://hg.mx.languagedepot.org/Test
        string RemoteUrl
        {
            get
            {
                return "http://hg.mx.languagedepot.org/Test";

            }
        }
        #endregion
        #region VAttr{g}: string RemoteUserName - "tester"
        string RemoteUserName
        {
            get
            {
                return "tester";

            }
        }
        #endregion
        #region VAttr{g}: string RemotePassword = "auto"
        string RemotePassword
        {
            get
            {
                return "auto";

            }
        }
        #endregion

        // Helper Methods --------------------------------------------------------------------
        #region Method: void CreateFile(sFullPath, svContents)
        void CreateFile(string sPath, string[] svContents)
        {
            TextWriter w = JW_Util.GetTextWriter(sPath);

            if (null != svContents)
            {
                foreach (string s in svContents)
                    w.WriteLine(s);
            }

            w.Close();
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: HgIsInstalled
        [Test] 
        public void HgIsInstalled()
            // Not much of a test, if Hg is installed. A true test would try it
            // out both if installed and not installed.
        {
            Assert.IsTrue( Repository.HgIsInstalled, "Hg Should Be Installed");
        }
        #endregion
        #region Test: GetChangedFiles
        [Test] public void GetChangedFiles()
        {
            // Create the Repository
            Repository.Create();

            // Create a couple of files
            string sFileName1 = ".Settings" + Path.DirectorySeparatorChar + "hello.owt";
            CreateFile(DB.TeamSettings.ClusterFolder + sFileName1, null);

            string sFileName2 = "AtRoot.db";
            CreateFile(DB.TeamSettings.ClusterFolder + sFileName2, null);

            // Our two files should be returned
            List<string> vsFiles = Repository.GetChangedFiles();

            // Debugging
            /*
            Console.WriteLine("Count = " + vsFiles.Count.ToString());
            foreach (string s in vsFiles)
                Console.WriteLine(s);
            */

            Assert.IsTrue(vsFiles.Contains(sFileName1), "Doesn't contain " + sFileName1);
            Assert.IsTrue(vsFiles.Contains(sFileName2), "Doesn't contain " + sFileName2);
        }
        #endregion
        #region Test: Commit
        [Test] public void Commit()
        {
            // Create the Repository
            Repository.Create();

            // Create a couple of files
            string sFileName1 = ".Settings" + Path.DirectorySeparatorChar + "hello.owt";
            CreateFile(DB.TeamSettings.ClusterFolder + sFileName1, null);
            string sFileName2 = "AtRoot.db";
            CreateFile(DB.TeamSettings.ClusterFolder + sFileName2, null);

            // Get the count of files that would be committed
            List<string> vsFiles = Repository.GetChangedFiles();
            Assert.IsTrue(vsFiles.Count >= 2, "Should have at least the two we created.");

            // Do the commit
            string sMessage = "Created two files for test purposes";
            Repository.Commit(sMessage, true);

            // We should now have zero files
            vsFiles = Repository.GetChangedFiles();
            Assert.IsTrue(vsFiles.Count == 0, "should have no uncommitted files now.");
        }
        #endregion
        #region Test: Clone
        [Test] public void Clone()
        {
            // Create the Repository
            Repository.Create();
            Repository.Commit("Clone Test", true);

            // Destination for the repository (make sure it is clear); we
            // fail otherwise.
            if (Directory.Exists(ClonePath))
                Directory.Delete(ClonePath, true);

            // Clone it
            Repository.CloneTo(ClonePath);

            // Does the directory exist?
            string sFolder = ClonePath + Path.DirectorySeparatorChar + ".Hg";
            Assert.IsTrue(Directory.Exists(sFolder));
        }
        #endregion
        #region Test: ParseChangeSetString
        [Test] public void ParseChangeSetString()
        {
            string[] vs = 
            {
                "changeset:   1:bbe11909d08a",
                "tag:         tip",
                "user:        JWimbish",
                "date:        Wed Mar 04 19:37:07 2009 -0500",
                "summary:     New File Added"
            };

            var v = Repository.ChangeSetDescription.Create(vs);

            Assert.AreEqual(1, v.Count, "Should have exactly one change set");

            Repository.ChangeSetDescription csd = v[0];

            Assert.AreEqual(csd.ID, "1:bbe11909d08a");
            Assert.AreEqual(csd.Tag, "tip");
            Assert.AreEqual(csd.User, "JWimbish");
            Assert.AreEqual(csd.Date, "Wed Mar 04 19:37:07 2009 -0500");
            Assert.AreEqual(csd.Summary, "New File Added");
        }
        #endregion
        #region Test: PushTo
        [Test] public void PushTo()
        {
            // Create the Repository
            Repository.Create();
            Repository.Commit("Push Test", true);

            // Clone it
            if (Directory.Exists(ClonePath))
                Directory.Delete(ClonePath, true);
            Repository.CloneTo(ClonePath);

            // Add a new file to our repository
            string sNewFile = "NewFile.db";
            CreateFile(DB.TeamSettings.ClusterFolder + sNewFile, null);
            Repository.Commit("New File Added", true);

            // Verify that we have a change to make
            var v = Repository.OutGoing(ClonePath);
            Assert.AreEqual(1, v.Count, "Should have exactly one change set");

            // Push it
            bool bResult = Repository.PushTo(ClonePath);
            Assert.IsTrue(bResult, "Push should have returned 'true'");

            // Should not have any changes remaining
            v = Repository.OutGoing(ClonePath);
            Assert.AreEqual(0, v.Count, "Should have zero change sets");

        }
        #endregion
    }
}
