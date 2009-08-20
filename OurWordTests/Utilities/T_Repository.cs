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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;
using JWdb.DataModel;

using OurWord;
using OurWord.Utilities;
#endregion


namespace OurWordTests.Utilities
{
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
            DB.Project.Write(new NullProgress());

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

        // Merging ---------------------------------------------------------------------------
        #region Method: void WriteFile(string sPath, string[] vs)
        void WriteFile(string sPath, string[] vs)
        {
            StreamWriter sw = new StreamWriter(sPath, false, Encoding.UTF8);
            TextWriter W = TextWriter.Synchronized(sw);
            foreach (string s in vs)
                W.WriteLine(s);
            W.Close();
        }
        #endregion
        #region Method: List<string> ReadFile(sPath)
        List<string> ReadFile(string sPath)
        {
            List<string> v = new List<string>();

            StreamReader sr = new StreamReader(sPath, Encoding.UTF8);
            TextReader R = TextReader.Synchronized(sr);

            string s;
            while ((s = R.ReadLine()) != null)
            {
                s = s.Trim();

                if (s.Length > 0 && s[0] == '\\')
                    v.Add(s);
                else if (v.Count > 0)
                    v[v.Count - 1] = v[v.Count - 1] + " " + s;
            }

            R.Close();

            return v;
        }
        #endregion

        /* [Test] */ public void Synchronize()
            // The purpose of this test is to check that the broad ChorusMerge 
            // is working. Individual classes will have their own merge tests for
            // specific situations.
        {
            // Discussion Parts
            string sJohn1 = "<Discussion Author=\"John\" Created=\"2009-02-28 09:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"'Arono' is used twice in this verse, isn't that stylistically bad?\"/></ownseq></Discussion>";
            string sLarry1 = "<Discussion Author=\"Larry\" Created=\"2009-03-01 09:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"Its a discourse feature. You obviously haven't read my paper on Yawa Discourse!\"/></ownseq></Discussion>";
            string sLinda = "<Discussion Author=\"Linda\" Created=\"2009-03-01 10:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"I don't know, Larry, you're ok as an administrator, but why do you think I'm the one doing this final revision?\"/></ownseq></Discussion>";
            string sLarry2 = "<Discussion Author=\"Larry\" Created=\"2009-03-01 12:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"Well, you've got a point.\"/></ownseq></Discussion>";
            string sOwen = "<Discussion Author=\"Owen\" Created=\"2009-03-02 10:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"Hey! Who's the MTT here, anyway? I say, Wimbish is right!\"/></ownseq></Discussion>";
            string sJohn2 = "<Discussion Author=\"John\" Created=\"2009-03-02 11:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"Looks like my work here is done.\"/></ownseq></Discussion>";
            string sSue = "<Discussion Author=\"Sue\" Created=\"2009-03-05 11:14:18Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"But I'm the consultant, and I say, rubbish!\"/></ownseq></Discussion>";

            // Book contents
            #region string[] vsParent - John1 Larry1 Linda Owen John2
            string[] vsParent = new string[]
            {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
			    "\\rcrd MRK 1",
			    "\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
			    "\\bts The Lord Jesus give an example of a seed that is extremely tiny",
			    "\\nt sain = jenis biji yg kici ana",
			    "\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
			    "\\p",
			    "\\v 30",
			    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, tal antee sin namfau nok.",
			    "\\btvt Then Jesus spoke again, saying, <<I give another example like this: You(pl) can compare God's *social group. From just a few people, it nevertheless increases (in number), to the point that they are very many.",
			    "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"Owen\" Context=\"Arono\" Reference=\"001:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                    sJohn1 + sLarry1 + sLinda + sOwen + sJohn2 +
                    "</ownseq></TranslatorNote>",
                "\\p",
			    "\\v 31",
			    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			    "\\btvt That is like a seed that is very tiny.",
			    "\\v 32",
			    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
			    "\\btvt If we(inc) plant it (with dibble stick) it will grow to become a large tree. And birds will come looking for shade, and make nests in it.>>",
			    "\\p",
			    "\\v 33",
			    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			    "\\btvt Jesus' way of teaching was according to their understanding.",
			    "\\v 34",
			    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' sin, In natoon lais leta' nane in na' naon ok-oke'.",
			    "\\btvt He taught people using only examples. But with His *disciples, He told the meaning of all the examples.",
            };
            #endregion
            #region string[] vsOurs -   John1 Larry1 Linda Owen John2 Sue
            string[] vsOurs = new string[]
            {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
			    "\\rcrd MRK 1",
			    "\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
			    "\\bts The Lord Jesus give an example of a seed that is extremely tiny",
			    "\\nt sain = jenis biji yg kici ana",
			    "\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
			    "\\p",
			    "\\v 30",
			    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, tal antee sin namfau nok.",
			    "\\btvt Then Jesus spoke again, saying, <<I give another example like this: You(pl) can compare God's *social group. From just a few people, it nevertheless increases (in number), to the point that they are very many.",
			    "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"Owen\" Context=\"Arono\" Reference=\"001:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                    sJohn1 + sLarry1 + sLinda + sOwen + sJohn2 + sSue +
                    "</ownseq></TranslatorNote>",
                "\\p",
			    "\\v 31",
			    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			    "\\btvt That is like a seed that is very tiny.",
			    "\\v 32",
			    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
			    "\\btvt If we(inc) plant it (with dibble stick) it will grow to become a large tree. And birds will come looking for shade, and make nests in it.>>",
			    "\\p",
			    "\\v 33",
			    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			    "\\btvt Jesus' way of teaching was according to their understanding.",
			    "\\v 34",
			    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' sin, In natoon lais leta' nane in na' naon ok-oke'.",
			    "\\btvt He taught people using only examples. But with His *disciples, He told the meaning of all the examples.",
            };
            #endregion
            #region string[] vsTheirs - John1 Larry1 Linda Larry2 Owen John2
            string[] vsTheirs = new string[]
            {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
			    "\\rcrd MRK 1",
			    "\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
			    "\\bts The Lord Jesus give an example of a seed that is extremely tiny",
			    "\\nt sain = jenis biji yg kici ana",
			    "\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
			    "\\p",
			    "\\v 30",
			    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, tal antee sin namfau nok.",
			    "\\btvt Then Jesus spoke again, saying, <<I give another example like this: You(pl) can compare God's *social group. From just a few people, it nevertheless increases (in number), to the point that they are very many.",
			    "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"Owen\" Context=\"Arono\" Reference=\"001:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                    sJohn1 + sLarry1 + sLinda + sLarry2 + sOwen + sJohn2 +
                    "</ownseq></TranslatorNote>",
                "\\p",
			    "\\v 31",
			    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			    "\\btvt That is like a seed that is very tiny.",
			    "\\v 32",
			    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
			    "\\btvt If we(inc) plant it (with dibble stick) it will grow to become a large tree. And birds will come looking for shade, and make nests in it.>>",
			    "\\p",
			    "\\v 33",
			    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			    "\\btvt Jesus' way of teaching was according to their understanding.",
			    "\\v 34",
			    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' sin, In natoon lais leta' nane in na' naon ok-oke'.",
			    "\\btvt He taught people using only examples. But with His *disciples, He told the meaning of all the examples.",
            };
            #endregion
            #region string[] vsSynchedResult - John1 Larry1 Linda Larry2 Owen John2 Sue
            string[] vsSynchedResult = new string[]
            {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
			    "\\rcrd MRK 1",
			    "\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
			    "\\bts The Lord Jesus give an example of a seed that is extremely tiny",
			    "\\nt sain = jenis biji yg kici ana",
			    "\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
			    "\\p",
			    "\\v 30",
			    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, tal antee sin namfau nok.",
			    "\\btvt Then Jesus spoke again, saying, <<I give another example like this: You(pl) can compare God's *social group. From just a few people, it nevertheless increases (in number), to the point that they are very many.",
			    "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"Owen\" Context=\"Arono\" Reference=\"001:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                    sJohn1 + sLarry1 + sLinda + sLarry2 + sOwen + sJohn2 + sSue +
                    "</ownseq></TranslatorNote>",
                "\\p",
			    "\\v 31",
			    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			    "\\btvt That is like a seed that is very tiny.",
			    "\\v 32",
			    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
			    "\\btvt If we(inc) plant it (with dibble stick) it will grow to become a large tree. And birds will come looking for shade, and make nests in it.>>",
			    "\\p",
			    "\\v 33",
			    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			    "\\btvt Jesus' way of teaching was according to their understanding.",
			    "\\v 34",
			    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' sin, In natoon lais leta' nane in na' naon ok-oke'.",
			    "\\btvt He taught people using only examples. But with His *disciples, He told the meaning of all the examples.",
            };
            #endregion

            // Create our local Repository and write the file that will serve as our parrent
            Repository.Create();
            string sOurPath = Repository.HgRepositoryRoot + Path.DirectorySeparatorChar + "05 MRK.db";
            WriteFile(sOurPath, vsParent);
            Repository.Commit("Parent Created.", true);

            // Clone it. So now, both Repositories should be identical, both with the
            // parent book
            if (Directory.Exists(ClonePath))
                Directory.Delete(ClonePath, true);
            Repository.CloneTo(ClonePath);
            var v = Repository.OutGoing(ClonePath);
            Assert.AreEqual(0, v.Count, "Should have zero change sets");

            // Change the book in our local repository
            WriteFile(sOurPath, vsOurs);
            Repository.Commit("Ours Changed.", true);

            // Change it in the cloned repository
            string sTheirPath = ClonePath + Path.DirectorySeparatorChar + "05 MRK.db";
            WriteFile(sTheirPath, vsTheirs);
            Process p = new Process();
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = ClonePath;
            p.StartInfo.FileName = "hg";
            p.StartInfo.Arguments = "commit -u \"Them\" -m \"Theirs Changed.\" \"" + ClonePath + "\"";
            p.Start();

            // Do the synch
            Repository.SynchronizeWith(ClonePath);

            // Did we get what we expected?
            List<string> vActual = ReadFile(
                Repository.HgRepositoryRoot + Path.DirectorySeparatorChar + "05 MRK.db");
            foreach (string s in vActual)
                Console.WriteLine("<" + s + ">");
            Assert.AreEqual(vsSynchedResult.Length, vActual.Count, "Counts should be equal");

            Console.WriteLine("");
            Console.WriteLine("-----------------------------------------------");
            for (int i = 0; i < vActual.Count; i++)
            {
                Console.WriteLine("i=" + i.ToString());
                Console.WriteLine("Exp=" + vsSynchedResult[i]);
                Console.WriteLine("Act=" + vActual[i]);
                Console.WriteLine("");

                Assert.AreEqual(vsSynchedResult[i].Trim(), vActual[i].Trim(), "Contents should be equal");
            }
        }




    }
}
