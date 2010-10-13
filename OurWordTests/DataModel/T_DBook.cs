#region ***** T_DBook.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DBook.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DBook class
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
using OurWordData.DataModel;

using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWordTests.DataModel
{
	#region CLASS: DTestBook : DBook - let's us override the StoragePath for test purposes
	public class DTestBook : DBook
	{
		#region Constructor(sStoragePath)
		public DTestBook(string sStoragePath)
			: base("MRK")
		{
			m_sStoragePath = sStoragePath;
		}
		#endregion
		#region OAttr{g}: string StoragePath
		public override string StoragePath
		{
			get
			{
				return m_sStoragePath;
			}
		}
		string m_sStoragePath;
		#endregion
	}
	#endregion

	[TestFixture] public class T_DBook : TestCommon
    {
        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
            JWU.NUnit_SetupClusterFolder();
        }
        #endregion
        #region Method: void TearDown()
        [TearDown] public void TearDown()
        {
            JWU.NUnit_TeardownClusterFolder();
        }
        #endregion

        #region Method: string GetTestPathName(string sBaseName)
        string GetTestPathName(string sBaseName)
        {
            string sPath = JWU.NUnit_TestFileFolder +
                Path.DirectorySeparatorChar + sBaseName + ".x";

            return sPath;
        }
        #endregion
        #region Method: void _WriteToFile(string sPathname, string[] vs)
        void _WriteToFile(string sPathname, string[] vs)
        {
            TextWriter W = JW_Util.GetTextWriter(sPathname);
            foreach (string s in vs)
                W.WriteLine(s);
            W.Close();
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: BookSortKeys
        [Test]  public void BookSortKeys()
        // Make certain we have a two-digit sort key, so that, e.g.,
        // Leviticus follows Exodus rather than Proverbs.
        {
            var book = CreateHierarchyThroughTargetBook("LEV");
            Assert.AreEqual("02", book.SortKey);
        }
        #endregion
        #region Test: CalculateVersification_NonContiguous
        [Test] public void CalculateVersification_NonContiguous()
        {
            #region TestData
            string[] vsRaw = new string[] 
		    {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\mt Mark",
                "",
			    "\\rcrd MRK 1",
			    "\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
			    "\\bts The Lord Jesus give an example of a seed that is extremely tiny",
			    "\\nt sain = jenis biji yg kici ana",
			    "\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
                "\\c 1",
			    "\\p",
			    "\\v 30",
			    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi " +
			    "nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes " +
			    "nabaab-took, tal antee sin namfau nok.",
                "",
                "\\rcrd MRK 2",
                "\\c 13",
                "\\p",
			    "\\v 12",
			    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			    "\\v 32",
			    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof " +
			    "kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
                "",
                "\\rcrd MRK 3",
                "\\c 4",
			    "\\p",
			    "\\v 33",
			    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			    "\\btvt Jesus' way of teaching was according to their understanding.",
                "",
                "\\rcrd MRK 4",
                "\\c 15",
                "\\q",
			    "\\v 34",
			    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' " +
			    "sin, In natoon lais leta' nane in na' naon ok-oke'.",
            };
            #endregion

            // Setup
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;

            // Write out vsRaw to disk file
            string sPathname = GetTestPathName("TestDBIO1");
            _WriteToFile(sPathname, vsRaw);

            // Now read it into the book. 
			DBook Book = new DBook("MRK");
            Translation.AddBook(Book);
            Book.LoadFromStandardFormat(sPathname);
            Book.DisplayName = "Mark";

            // Check for the the proper book references for each section
            Assert.AreEqual(4, Book.Sections.Count);
            Assert.AreEqual("Mark 1:30-30", (Book.Sections[0] as DSection).ReferenceName);
            Assert.AreEqual("Mark 13:12-32", (Book.Sections[1] as DSection).ReferenceName);
            Assert.AreEqual("Mark 4:33-33", (Book.Sections[2] as DSection).ReferenceName);
            Assert.AreEqual("Mark 15:34-34", (Book.Sections[3] as DSection).ReferenceName);
        }
        #endregion
        #region Test: ParseFileName
        [Test] public void ParseFileName()
            // The book is calculated as:
            //    "01 GEN - Amarasi - Draft-A 2008-11-23"
            // Here, we request the book to calculate something, so we can test both
            // that the generate code hasn't changed, but also that we parse correctly.
        {
            // Set up a book
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Amarasi", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;
            DBook book = new DBook("MRK");
            Translation.AddBook(book);

            // Set up stage, version info
            book.Stage = DB.TeamSettings.Stages.Find(Stage.c_idCommunityCheck);
            book.Version = "B";

            // Compute the filename
            string sBaseName = book.BaseNameForBackup;

            Assert.AreEqual("41 MRK - Amarasi - Comm-B " + DB.Today, sBaseName);

            // Now parse the filename
            int nBookNumber = 0;
            string sBookAbbrev = "";
            string sLanguageName = "";
            string sStageAbbrev = "";
            string sVersion = "";
            DBook.ParseFileName(sBaseName, ref nBookNumber, ref sBookAbbrev,
                ref sLanguageName, ref sStageAbbrev, ref sVersion);

            Assert.AreEqual(41, nBookNumber);
            Assert.AreEqual("MRK", sBookAbbrev);
            Assert.AreEqual("Amarasi", sLanguageName);
            Assert.AreEqual("Comm", sStageAbbrev);
            Assert.AreEqual("B", sVersion);
        }
        #endregion
        #region Test: GetInfoFromPath
        [Test] public void GetInfoFromPath()
        {
            // Normal path, should succeed
            string sPath = "C:\\Users\\JWimbish\\Documents\\Timor2\\North Huidao\\42 LUK - North Huidao";
            string[] vs = DBook.GetInfoFromPath(sPath);
            Assert.AreEqual("42", vs[0]);
            Assert.AreEqual("LUK", vs[1]);
            Assert.AreEqual("North Huidao", vs[2]);

            // Number doesn't match abbrev, should fail
            sPath = "C:\\Users\\JWimbish\\Documents\\Timor2\\North Huidao\\02 LUK - North Huidao";
            vs = DBook.GetInfoFromPath(sPath);
            Assert.IsNull(vs, "Number doesn't match abbrev");
        }
        #endregion

        #region Test: RestoreFromBackup
        #region Method: string GetSimpleParagraphText(DParagraph p)
        string GetSimpleParagraphText(DParagraph p)
        {
            if (p.Runs.Count == 0)
                p.AddRun(new DText());
            var text = p.Runs[0] as DText;

            if (text.Phrases.Count == 0)
                text.Phrases.Append(new DPhrase(""));
            var phrase = text.Phrases[0] as DPhrase;

            return phrase.Text;
        }
        #endregion
        #region Method: void SetSimpleParagraphText(DParagraph p, string s)
        void SetSimpleParagraphText(DParagraph p, string s)
        {
            if (p.Runs.Count == 0)
                p.AddRun(new DText());
            var text = p.Runs[0] as DText;

            if (text.Phrases.Count == 0)
                text.Phrases.Append(new DPhrase(""));
            var phrase = text.Phrases[0];

            phrase.Text = s;
        }
        #endregion
        [Test] public void RestoreFromBackup()
        {
            // We don't want message dialogs to show up during the tests
            LocDB.Reset();
            LocDB.SuppressMessages = true;

            // Original settings
            const string sTextO = "Original";

            // Later, changed settings (that backup will erase)
            const string sTextC = "Changed";

            // Setup a project/translation/book
            var book = CreateHierarchyThroughTargetBook("MRK");

            var writer = JWU.NUnit_OpenTextWriter("book.db");
            foreach (var s in SectionTestData.BaikenoMark0101_ImportVariant)
                writer.WriteLine(s);
            writer.Close();
            book.LoadFromStandardFormat(Path.Combine(JWU.NUnit_TestFileFolder, "book.db"));
            book.Stage = DB.TeamSettings.Stages.Find(Stage.c_idDraft);
            book.Version = "A";

            // Add an extra paragraph
            var p = new DParagraph(StyleSheet.Line1);
            var section = book.Sections[0];
            section.Paragraphs.Append(p);
            SetSimpleParagraphText(p, sTextO);

            // Save the book. Then save it again, so that "Original" gets placed
            // into the ".backup" folder
            book.DeclareDirty();
            book.WriteBook(new NullProgress());

            // Compute the backup path we've just saved to
            var sBackupPathName = book.Translation.Project.TeamSettings.BackupFolder +
                    book.BaseNameForBackup + ".bak";

            // Change the book and the pathname and save it as a current version
            SetSimpleParagraphText(p, sTextC);
            book.Stage = DB.TeamSettings.Stages.Find(Stage.c_idConsultantCheck);
            book.Version = "C";
            book.DeclareDirty();
            book.WriteBook(new NullProgress());

            // Restore the original
            DBook.RestoreFromBackup(book, sBackupPathName, new NullProgress());

            // Check for original data, stage, version,.
            section = book.Sections[0];
            var iParaLast = section.Paragraphs.Count - 1;
            var pLast = section.Paragraphs[iParaLast];
            var sActual = GetSimpleParagraphText(pLast);
            Assert.AreEqual(sTextO, sActual);
            Assert.AreEqual(Stage.c_idDraft, book.Stage.ID);
            Assert.AreEqual("A", book.Version);

            // Reset the LocDB
            LocDB.Reset();
        }
        #endregion

        // Book Stats ------------------------------------------------------------------------
        #region Test: StatsIO
        [Test] public void StatsIO()
        {
            // Load some data into a book
            var book = new DBook("MRK");
            book.BookStats.SetValue(DB.TeamSettings.Stages.Find(Stage.c_idDraft), 25.2);
            book.BookStats.SetValue(DB.TeamSettings.Stages.Find(Stage.c_idCommunityCheck), 12);
            book.BookStats.SetValue(DB.TeamSettings.Stages.Find(Stage.c_idBackTranslation), 5.3);

            // Do we get the expected save string?
            string sSave = book.BookStats.ToSaveString();
            Assert.AreEqual("Draft=25.20+Comm=12.00+BT=5.30", sSave);

            // Clear the book, then populate the stats from our save string
            book.BookStats.DictStatistics.Clear();
            Assert.AreEqual(0, book.BookStats.DictStatistics.Count);
            book.BookStats.FromSaveString(sSave);

            // Do we get the stats we expect?
            Assert.AreEqual("25.20", 
                book.BookStats.GetPercentComplete(DB.TeamSettings.Stages.Find(Stage.c_idDraft)).ToString("0.00"));
            Assert.AreEqual("12.00", 
                book.BookStats.GetPercentComplete(DB.TeamSettings.Stages.Find(Stage.c_idCommunityCheck)).ToString("0.00"));
            Assert.AreEqual("5.30", 
                book.BookStats.GetPercentComplete(DB.TeamSettings.Stages.Find(Stage.c_idBackTranslation)).ToString("0.00"));
        }
        #endregion



    }

}
