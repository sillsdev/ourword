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
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DTranslation
    {
        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();

            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
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

            DBook bookFront = new DBook();
            DB.Project.FrontTranslation.Books.Append(bookFront);
            DSection sectionFront = new DSection(1);
            bookFront.Sections.Append(sectionFront);
            DParagraph paraFront = new DParagraph();
            sectionFront.Paragraphs.Append(paraFront);

            DBook bookTarget = new DBook();
            DB.Project.TargetTranslation.Books.Append(bookTarget);
            DSection sectionTarget = new DSection(1);
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

        // Merging ---------------------------------------------------------------------------
        #region SAttr{g}: string[] vsParent
        static string[] vsParent = 
        {
            "<DTranslation FileFormat=\"0\" DisplayName=\"Tii\" VernacularWS=\"Latin\" ConsultantWS=\"Latin\" BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} " +
            "{Kalua numa Masir mai} {Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} {Mana Maketu Dede'a kara} {Rut} {1 Samuel} " +
            "{2 Samuel} {1 Mane-mane kara} {2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} {Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio " +
            "kara} {Dede'a Lasik} {Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} {Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} " +
            "{Yunus} {Mika} {Nahum} {Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} {Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} " +
            "{2 Korentus} {Galati} {Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} {2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} " +
            "{2 Petrus} {1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\">",
            "  <ownseq Name=\"Books\">",
            "    <DBook FileFormat=\"0\" DisplayName=\"Tutui Hata Mana Dadik Sososan\" Abbrev=\"GEN\" ID=\"GEN\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Mateos\" Abbrev=\"MAT\" ID=\"MAT\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Markus\" Abbrev=\"MRK\" ID=\"MRK\" Stage=\"5\" Version=\"H\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Nadenu nara Tutuin\" Abbrev=\"ACT\" ID=\"ACT\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Galati\" Abbrev=\"GAL\" ID=\"GAL\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Efesus\" Abbrev=\"EPH\" ID=\"EPH\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Felipi\" Abbrev=\"PHP\" ID=\"PHP\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Kolose\" Abbrev=\"COL\" ID=\"COL\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Tesalonika\" Abbrev=\"1TH\" ID=\"1TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Tesalonika\" Abbrev=\"2TH\" ID=\"2TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Timotius\" Abbrev=\"1TI\" ID=\"1TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Timotius\" Abbrev=\"2TI\" ID=\"2TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Filmon\" Abbrev=\"PHM\" ID=\"PHM\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Ibrani\" Abbrev=\"HEB\" ID=\"HEB\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Yakobis\" Abbrev=\"JAS\" ID=\"JAS\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"Dae-bafo Mata Beuk\" Abbrev=\"REV\" ID=\"REV\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "  </ownseq>",
            "</DTranslation>"
        };
        #endregion
        #region SAttr{g}: string[] vsOurs
        static string[] vsOurs = 
        {
            "<DTranslation FileFormat=\"0\" DisplayName=\"Tii\" VernacularWS=\"Latin\" ConsultantWS=\"Latin\" BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} " +
            "{Kalua numa Masir mai} {Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} {Mana Maketu Dede'a kara} {Rut} {1 Samuel} " +
            "{2 Samuel} {1 Mane-mane kara} {2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} {Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio " +
            "kara} {Dede'a Lasik} {Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} {Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} " +
            "{Yunus} {Mika} {Nahum} {Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} {Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} " +
            "{2 Korentus} {Galati} {Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} {2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} " +
            "{2 Petrus} {1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\">",
            "  <ownseq Name=\"Books\">",
            "    <DBook FileFormat=\"0\" DisplayName=\"Tutui Hata Mana Dadik Sososan\" Abbrev=\"GEN\" ID=\"GEN\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Mateos\" Abbrev=\"MAT\" ID=\"MAT\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Markus\" Abbrev=\"MRK\" ID=\"MRK\" Stage=\"5\" Version=\"H\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Lukas\" Abbrev=\"LUK\" ID=\"LUK\" Stage=\"1\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Nadenu nara Tutuin\" Abbrev=\"ACT\" ID=\"ACT\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Galati\" Abbrev=\"GAL\" ID=\"GAL\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Efesus\" Abbrev=\"EPH\" ID=\"EPH\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Tesalonika\" Abbrev=\"1TH\" ID=\"1TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Tesalonika\" Abbrev=\"2TH\" ID=\"2TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Timotius\" Abbrev=\"1TI\" ID=\"1TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Timotius\" Abbrev=\"2TI\" ID=\"2TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Titus\" Abbrev=\"TIT\" ID=\"TIT\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Filmon\" Abbrev=\"PHM\" ID=\"PHM\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Ibrani\" Abbrev=\"HEB\" ID=\"HEB\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Yakobis\" Abbrev=\"JAS\" ID=\"JAS\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"Dae-bafo Mata Beuk\" Abbrev=\"REV\" ID=\"REV\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "  </ownseq>",
            "</DTranslation>"
        };
        #endregion
        #region SAttr{g}: string[] vsTheirs
        static string[] vsTheirs = 
        {
            "<DTranslation FileFormat=\"0\" DisplayName=\"Tii\" VernacularWS=\"Latin\" ConsultantWS=\"Latin\" BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} " +
            "{Kalua numa Masir mai} {Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} {Mana Maketu Dede'a kara} {Rut} {1 Samuel} " +
            "{2 Samuel} {1 Mane-mane kara} {2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} {Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio " +
            "kara} {Dede'a Lasik} {Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} {Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} " +
            "{Yunus} {Mika} {Nahum} {Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} {Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} " +
            "{2 Korentus} {Galati} {Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} {2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} " +
            "{2 Petrus} {1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\">",
            "  <ownseq Name=\"Books\">",
            "    <DBook FileFormat=\"0\" DisplayName=\"Tutui Hata Mana Dadik Sososan\" Abbrev=\"GEN\" ID=\"GEN\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Mateos\" Abbrev=\"MAT\" ID=\"MAT\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Markus\" Abbrev=\"MRK\" ID=\"MRK\" Stage=\"5\" Version=\"H\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Nadenu nara Tutuin\" Abbrev=\"ACT\" ID=\"ACT\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Roma\" Abbrev=\"ROM\" ID=\"ROM\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Galati\" Abbrev=\"GAL\" ID=\"GAL\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Efesus\" Abbrev=\"EPH\" ID=\"EPH\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Felipi\" Abbrev=\"PHP\" ID=\"PHP\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Tesalonika\" Abbrev=\"1TH\" ID=\"1TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Tesalonika\" Abbrev=\"2TH\" ID=\"2TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Timotius\" Abbrev=\"1TI\" ID=\"1TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Timotius\" Abbrev=\"2TI\" ID=\"2TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Filmon\" Abbrev=\"PHM\" ID=\"PHM\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Ibrani\" Abbrev=\"HEB\" ID=\"HEB\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Yakobis\" Abbrev=\"JAS\" ID=\"JAS\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"Yudas\" Abbrev=\"JUD\" ID=\"JUD\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Dae-bafo Mata Beuk\" Abbrev=\"REV\" ID=\"REV\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "  </ownseq>",
            "</DTranslation>"
        };
        #endregion
        #region SAttr{g}: string[] vsExpected
        static string[] vsExpected = 
        {
            "<DTranslation FileFormat=\"0\" DisplayName=\"Tii\" VernacularWS=\"Latin\" ConsultantWS=\"Latin\" BookNamesTable=\"66 {Tutui Hata Mana Dadik Sososan} " +
            "{Kalua numa Masir mai} {Malangga Anggamar Dala Hadan} {Susura Rerekek} {Tui Seluk Dala Sodak} {Yosua} {Mana Maketu Dede'a kara} {Rut} {1 Samuel} " +
            "{2 Samuel} {1 Mane-mane kara} {2 Mane-mane kara} {1 Isra'el no Yahuda Tutuin} {2 Isra'el no Yahuda Tutuin} {Esra} {Nehemia} {Ester} {Ayub} {Sosoda Kokoa-Kikio " +
            "kara} {Dede'a Lasik} {Nenori la'e neu Sodak} {Soleman Dede'a Binin} {Yesaya} {Yermia} {Bu'i Nekerereu} {Yeskial} {Danial} {Hosea} {Yoel} {Amos} {Obaja} " +
            "{Yunus} {Mika} {Nahum} {Habakuk} {Sefanya} {Hanggai} {Sakarias} {Maleaki} {Mateos} {Markus} {Lukas} {Yohanis} {Nadenu nara Tutuin} {Roma} {1 Korentus} " +
            "{2 Korentus} {Galati} {Efesus} {Felipi} {Kolose} {1 Tesalonika} {2 Tesalonika} {1 Timotius} {2 Timotius} {Titus} {Filmon} {Ibrani} {Yakobis} {1 Petrus} " +
            "{2 Petrus} {1 Yohanis} {2 Yohanis} {3 Yohanis} {Yudas} {Dae-bafo Mata Beuk}\">",
            "  <ownseq Name=\"Books\">",
            "    <DBook FileFormat=\"0\" DisplayName=\"Tutui Hata Mana Dadik Sososan\" Abbrev=\"GEN\" ID=\"GEN\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Mateos\" Abbrev=\"MAT\" ID=\"MAT\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Markus\" Abbrev=\"MRK\" ID=\"MRK\" Stage=\"5\" Version=\"H\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Lukas\" Abbrev=\"LUK\" ID=\"LUK\" Stage=\"1\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Nadenu nara Tutuin\" Abbrev=\"ACT\" ID=\"ACT\" Stage=\"5\" Version=\"F\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Roma\" Abbrev=\"ROM\" ID=\"ROM\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Galati\" Abbrev=\"GAL\" ID=\"GAL\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Efesus\" Abbrev=\"EPH\" ID=\"EPH\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Tesalonika\" Abbrev=\"1TH\" ID=\"1TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Tesalonika\" Abbrev=\"2TH\" ID=\"2TH\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"1 Timotius\" Abbrev=\"1TI\" ID=\"1TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"2 Timotius\" Abbrev=\"2TI\" ID=\"2TI\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Titus\" Abbrev=\"TIT\" ID=\"TIT\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Filmon\" Abbrev=\"PHM\" ID=\"PHM\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Ibrani\" Abbrev=\"HEB\" ID=\"HEB\" Stage=\"0\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Yakobis\" Abbrev=\"JAS\" ID=\"JAS\" Stage=\"5\" Version=\"C\" Locked=\"true\" \"/>",
            "    <DBook FileFormat=\"0\" DisplayName=\"Yudas\" Abbrev=\"JUD\" ID=\"JUD\" Stage=\"3\" Version=\"A\" Locked=\"false\" />",
            "    <DBook FileFormat=\"0\" DisplayName=\"Dae-bafo Mata Beuk\" Abbrev=\"REV\" ID=\"REV\" Stage=\"5\" Version=\"C\" Locked=\"true\" />",
            "  </ownseq>",
            "</DTranslation>"
        };
        #endregion

        #region Method: DTranslation CreateTranslation(sDisplayName, string[] vsData)
        DTranslation CreateTranslation(string sDisplayName, string[] vsData)
        {
            DTranslation t = new DTranslation(sDisplayName);

            // Writei out the data so we can read it in
            TextWriter W = JWU.NUnit_OpenTextWriter("test.x");
            foreach (string s in vsData)
                W.WriteLine(s);
            W.Close();

            // Read it in
            string sPath = JWU.NUnit_TestFilePathName;
            t.Load(ref sPath, new NullProgress());
            t.DisplayName = sDisplayName;

            // So ownership hierarchy will work in debugger
            DB.Project.OtherTranslations.Append(t);

            return t;
        }
        #endregion

        #region Test: MergeBooksSequence
        [Test] public void MergeBooksSequence()
        {
            // Differences
            // Ours
            // - Adds Luke and Titus  (thus Parent, Theirs lacks these; Expected has them)
            // - Deletes Philippians  (thus Parent, Theirs have it, Expected doesn't)
            //
            // Theirs
            // - Adds Romans, Yudas (thus Parent,  Mine lacks these, Expected has them)
            //
            // Both
            // - Delete Colosians (thus Parent has these, all others don't)

            DTranslation tParent = CreateTranslation("parent", vsParent);
            DTranslation tOurs = CreateTranslation("ours", vsOurs);
            DTranslation tTheirs = CreateTranslation("theirs", vsTheirs);
            DTranslation tExpected = CreateTranslation("expected", vsExpected);

            // Do the merge
            tOurs.Merge(tParent, tTheirs, true);

            // Compare
            Assert.AreEqual(tExpected.Books.Count, tOurs.Books.Count, "Same Count");
            Assert.IsTrue(tExpected.Books.ContentEquals(tOurs.Books), "Same Books");
        }
        #endregion
    }
}
