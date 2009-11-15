#region ***** T_DTranslation.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DTranslation.cs
 * Author:  John Wimbish
 * Created: 08 July 2008
 * Purpose: Tests the DTranslation class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;
using Chorus.merge;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DTranslation
    {
        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();

            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings("Test");
            DB.TeamSettings.EnsureInitialized();
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: ConvertsCrossReferences
        [Test] public void ConvertsCrossReferences()
        // Test the conversion from a Source cross-reference to a Target cross-reference.
        // We want to see that 
        // - only those things that are in the Source get changed,
        // - only whole words in the string get changed (not partial matches)
        {
            DB.Project.FrontTranslation = new DTranslation("Front", "Latin", "Latin");
            DB.Project.TargetTranslation = new DTranslation("Target", "Latin", "Latin");

            DBook bookFront = new DBook("MRK");
            DB.Project.FrontTranslation.AddBook(bookFront);
            DSection sectionFront = new DSection();
            bookFront.Sections.Append(sectionFront);
            DParagraph paraFront = new DParagraph();
            sectionFront.Paragraphs.Append(paraFront);

            DBook bookTarget = new DBook("MRK");
            DB.Project.TargetTranslation.AddBook(bookTarget);
            DSection sectionTarget = new DSection();
            bookTarget.Sections.Append(sectionTarget);
            DParagraph paraTarget = new DParagraph();
            sectionTarget.Paragraphs.Append(paraTarget);

            DB.Project.FrontTranslation.BookNamesTable.Clear();
            DB.Project.FrontTranslation.BookNamesTable.Append("Genesis");
            DB.Project.FrontTranslation.BookNamesTable.Append("Exodus");
            DB.Project.FrontTranslation.BookNamesTable.Append("Ge");
            DB.Project.FrontTranslation.BookNamesTable.Append("2 Kings");
            DB.Project.FrontTranslation.BookNamesTable.Append("Song of Songs");
            DB.Project.FrontTranslation.BookNamesTable.Append("2 John");
            DB.Project.FrontTranslation.BookNamesTable.Append("Carita (Mula-Mula)");

            DB.Project.TargetTranslation.BookNamesTable.Clear();
            DB.Project.TargetTranslation.BookNamesTable.Append("Kejadian");
            DB.Project.TargetTranslation.BookNamesTable.Append("Keluaran");
            DB.Project.TargetTranslation.BookNamesTable.Append("Imamat");
            DB.Project.TargetTranslation.BookNamesTable.Append("2 Raja-Raja");
            DB.Project.TargetTranslation.BookNamesTable.Append("Kidung Agung");
            DB.Project.TargetTranslation.BookNamesTable.Append("2 Yohanes");
            DB.Project.TargetTranslation.BookNamesTable.Append("Kejadian");

            string sSource = "(Genesis 3:4; Exodus 12:4, 3; Ex 3:4, " +
                "Genesissy 23:4; 2 Kings 13:3; Carita (Mula-Mula) 5:5, 23";
            string sExpected = "(Kejadian 3:4; Keluaran 12:4, 3; Ex 3:4, " +
                "Genesissy 23:4; 2 Raja-Raja 13:3; Kejadian 5:5, 23";
            paraFront.SimpleText = sSource;

            DB.Project.TargetTranslation.ConvertCrossReferences(paraFront, paraTarget);

            Assert.AreEqual(sExpected, paraTarget.SimpleText);
        }
        #endregion

        #region Test: PopulateBookListFromFolder
        [Test] public void PopulateBookListFromFolder()
        {
            // Create a translation
            var translation = new DTranslation("Kupang");
            DB.Project.TargetTranslation = translation;

            // Create a folder
            ClusterList.CreateNewCluster(true, DB.TeamSettings.DisplayName);
            ClusterList.ScanForClusters();
            if (!Directory.Exists(translation.BookStorageFolder))
                Directory.CreateDirectory(translation.BookStorageFolder);

            // Add some oxes files
            string[] vs = new string[] {
                "42 LUK - Kupang",
                "41 MRK - Kupang",
                "44 ACT - Kupang"
            };
            foreach (string s in vs)
            {
                var f = File.Create(translation.BookStorageFolder + s + ".oxes");
                f.Close();
            }

            // Perform the method
            translation.PopulateBookListFromFolder();

            // Did we get the BookList we expected?
            Assert.AreEqual(3, translation.BookList.Count);
            Assert.AreEqual("MRK", translation.BookList[0].BookAbbrev);
            Assert.AreEqual("LUK", translation.BookList[1].BookAbbrev);
            Assert.AreEqual("ACT", translation.BookList[2].BookAbbrev);

            // Remove the folder
            Directory.Delete(DB.TeamSettings.ClusterFolder, true);
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Test: IO
        [Test] public void IO()
        {
            // Individual attributes
            string sDisplayName = "Amarasi";
            string sVernacularWS = "Malay";
            string sConsultantWS = "English";
            string sBookNames = "66 {Tutui Hata Mana Dadik Sososan} {Kalua numa Masir mai} " +
                "{Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} " +
                "{Mana Maketu Dede'a kara} {Rut} {1 Samuel} {2 Samuel} {1 Mane-mane kara} " +
                "{2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} " +
                "{Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio kara} {Dede'a Lasik} " +
                "{Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} " +
                "{Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} {Yunus} {Mika} {Nahum} " +
                "{Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} " +
                "{Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} {2 Korentus} {Galati} " +
                "{Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} " +
                "{2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} {2 Petrus} " +
                "{1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}";
            string sComment = "Drafted using OurWord's Marla strategy.";
            var nFootnoteType = DFoot.FootnoteSequenceTypes.iv;
            string sCustomFootnotes = "5 {a} {b} {x} {y} {z}";

            // Expected xml string
            string sExpected = "<DTranslation " +
                "DisplayName=\"Amarasi\" " +
                "VernacularWS=\"Malay\" " +
                "ConsultantWS=\"English\" " +
                "Comment=\"Drafted using OurWord's Marla strategy.\" " +
                "BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} {Kalua numa Masir mai} " +
                    "{Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} " +
                    "{Mana Maketu Dede'a kara} {Rut} {1 Samuel} {2 Samuel} {1 Mane-mane kara} " +
                    "{2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} " +
                    "{Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio kara} {Dede'a Lasik} " +
                    "{Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} " +
                    "{Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} {Yunus} {Mika} {Nahum} " +
                    "{Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} " +
                    "{Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} {2 Korentus} {Galati} " +
                    "{Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} " +
                    "{2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} {2 Petrus} " +
                    "{1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\" " +
                "FootnoteSeqType=\"1\" " +
                "FootnoteCustomSeq=\"5 {a} {b} {x} {y} {z}\"" +
                " />";

            // Create and populate a translation
            var t = new DTranslation();
            t.DisplayName = sDisplayName;
            t.VernacularWritingSystemName = sVernacularWS;
            t.ConsultantWritingSystemName = sConsultantWS;
            t.BookNamesTable.Read(sBookNames);
            t.Comment = sComment;
            t.FootnoteSequenceType = nFootnoteType;
            t.FootnoteCustomSeq.Read(sCustomFootnotes);

            // A. TEST the WRITE implementation
            // Write it to a temporary file
            t.WriteToFile(JWU.NUnit_TestFilePathName, new NullProgress());

            // Read it in and see if we got what we expected
            var vs = JWU.ReadFile(JWU.NUnit_TestFilePathName);
            string sActual = "";
            foreach (string s in vs)
                sActual += s;
            Assert.AreEqual(sExpected, sActual);

            // B. TEST the READ implementation
            var tIn = new DTranslation();
            tIn.LoadFromFile(JWU.NUnit_TestFilePathName);
            Assert.AreEqual(sDisplayName, tIn.DisplayName);
            Assert.AreEqual(sVernacularWS, tIn.VernacularWritingSystemName);
            Assert.AreEqual(sConsultantWS, tIn.ConsultantWritingSystemName);
            Assert.AreEqual(sBookNames, tIn.BookNamesTable.SaveLine);
            Assert.AreEqual(sComment, tIn.Comment);
            Assert.AreEqual(nFootnoteType, tIn.FootnoteSequenceType);
            Assert.AreEqual(sCustomFootnotes, tIn.FootnoteCustomSeq.SaveLine);
        }
        #endregion
        #region Test: Merge
        [Test] public void Merge()
        {
            #region Data
            // Parent
            string sParent = "<DTranslation " +
                "DisplayName=\"Amarasi\" " +
                "VernacularWS=\"Malay\" " +
                "ConsultantWS=\"English\" " +
                "Comment=\"Drafted using OurWord's Marla strategy.\" " +
                "BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} {Kalua numa Masir mai} " +
                    "{Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} " +
                    "{Mana Maketu Dede'a kara} {Rut} {1 Samuel} {2 Samuel} {1 Mane-mane kara} " +
                    "{2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} " +
                    "{Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio kara} {Dede'a Lasik} " +
                    "{Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} " +
                    "{Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} {Yunus} {Mika} {Nahum} " +
                    "{Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} " +
                    "{Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} {2 Korentus} {Galati} " +
                    "{Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} " +
                    "{2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} {2 Petrus} " +
                    "{1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\" " +
                "FootnoteSeqType=\"1\" " +
                "FootnoteCustomSeq=\"5 {a} {b} {x} {y} {z}\"" +
                " />";

            // Ours Changes: 
            //   DisplayName = "Helong"
            //   Vernacular = "Latin"
            //   FirstBook = "Kejadian"
            string sOurs = "<DTranslation " +
                "DisplayName=\"Helong\" " +
                "VernacularWS=\"Latin\" " +
                "ConsultantWS=\"English\" " +
                "Comment=\"Drafted using OurWord's Marla strategy.\" " +
                "BookNamesTable=\"66 {Kejadian} {Kalua numa Masir mai} " +
                    "{Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} " +
                    "{Mana Maketu Dede'a kara} {Rut} {1 Samuel} {2 Samuel} {1 Mane-mane kara} " +
                    "{2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} " +
                    "{Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio kara} {Dede'a Lasik} " +
                    "{Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} " +
                    "{Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} {Yunus} {Mika} {Nahum} " +
                    "{Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} " +
                    "{Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} {2 Korentus} {Galati} " +
                    "{Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} " +
                    "{2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} {2 Petrus} " +
                    "{1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\" " +
                "FootnoteSeqType=\"1\" " +
                "FootnoteCustomSeq=\"5 {a} {b} {x} {y} {z}\"" +
                " />";


            // Theirs Changes:
            //   ConsultantWS = "Spanish"
            //   SecondBook = "Keluaran"
            //   Comment = "Revised"
            //   CustomSeq = "6 {1} {2} {3} {4} {5} {6}"
            //   FootnoteSeq = "0"
            string sTheirs = "<DTranslation " +
                "DisplayName=\"Amarasi\" " +
                "VernacularWS=\"Malay\" " +
                "ConsultantWS=\"Spanish\" " +
                "Comment=\"Revised\" " +
                "BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} {Keluaran} " +
                    "{Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} " +
                    "{Mana Maketu Dede'a kara} {Rut} {1 Samuel} {2 Samuel} {1 Mane-mane kara} " +
                    "{2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} " +
                    "{Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio kara} {Dede'a Lasik} " +
                    "{Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} " +
                    "{Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} {Yunus} {Mika} {Nahum} " +
                    "{Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} " +
                    "{Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} {2 Korentus} {Galati} " +
                    "{Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} " +
                    "{2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} {2 Petrus} " +
                    "{1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\" " +
                "FootnoteSeqType=\"0\" " +
                "FootnoteCustomSeq=\"6 {1} {2} {3} {4} {5} {6}\"" +
                " />";

            // Expected Result
            //   DisplayName = "Helong"
            //   Vernacular = "Latin"
            //   ConsultantWS = "Spanish"
            //   FirstBook = "Kejadian"
            //   SecondBook = "Keluaran"
            //   Comment = "Revised"
            //   CustomSeq = "6 {1} {2} {3} {4} {5} {6}"
            //   FootnoteSeq = "0"
            string sExpected = "<DTranslation " +
                "DisplayName=\"Helong\" " +
                "VernacularWS=\"Latin\" " +
                "ConsultantWS=\"Spanish\" " +
                "Comment=\"Revised\" " +
                "BookNamesTable=\"66 {Kejadian} {Keluaran} " +
                    "{Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} " +
                    "{Mana Maketu Dede'a kara} {Rut} {1 Samuel} {2 Samuel} {1 Mane-mane kara} " +
                    "{2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} " +
                    "{Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio kara} {Dede'a Lasik} " +
                    "{Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} " +
                    "{Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} {Yunus} {Mika} {Nahum} " +
                    "{Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} " +
                    "{Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} {2 Korentus} {Galati} " +
                    "{Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} " +
                    "{2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} {2 Petrus} " +
                    "{1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\" " +
                "FootnoteSeqType=\"0\" " +
                "FootnoteCustomSeq=\"6 {1} {2} {3} {4} {5} {6}\"" +
                " />";
            #endregion

            // Write the three files
            string sFolder = JWU.NUnit_TestFileFolder + Path.DirectorySeparatorChar;

            var sFileParent = sFolder + "parent.otrans";
            JWU.WriteFile(sFileParent, sParent);

            var sFileOurs = sFolder + "ours.otrans";
            JWU.WriteFile(sFileOurs, sOurs);

            var sFileTheirs = sFolder + "theirs.otrans";
            JWU.WriteFile(sFileTheirs, sTheirs);

            var MergeInfo = new MergeOrder(
                sFileOurs, sFileParent, sFileTheirs,
                new NullMergeSituation());


//            var MergeInfo = new MergeOrder(MergeOrder.ConflictHandlingMode.WeWin,
//                sFileOurs, sFileParent, sFileTheirs);

            // Do the merge
            DTranslation.Merge(MergeInfo);

            // Read the result and compare vs expected
            var t = new DTranslation();
            t.LoadFromFile(sFileOurs);
            t.IsDirty = true;
            t.WriteToFile(JWU.NUnit_TestFilePathName, new NullProgress());
            var vs = JWU.ReadFile(JWU.NUnit_TestFilePathName);
            string sActual = "";
            foreach (string s in vs)
                sActual += s;
            Assert.AreEqual(sExpected, sActual);
        }
        #endregion

    }
}
