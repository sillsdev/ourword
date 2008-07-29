/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DBook.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DBook class
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DBook
    {
        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
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
            DBook book = new DBook("LEV", "");
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
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.InitializeFactoryStyleSheet();
            G.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            G.Project.TargetTranslation = Translation;

            // Write out vsRaw to disk file
            string sPathname = GetTestPathName("TestDBIO1");
            _WriteToFile(sPathname, vsRaw);

            // Now read it into the book. 
            DBook Book = new DBook("MRK", "");
            Translation.AddBook(Book);
            Book.AbsolutePathName = sPathname;
            Book.Load();
            Book.DisplayName = "Mark";

            // Check for the the proper book references for each section
            Assert.AreEqual(4, Book.Sections.Count);
            Assert.AreEqual("Mark 1:30-30", (Book.Sections[0] as DSection).ReferenceName);
            Assert.AreEqual("Mark 13:12-32", (Book.Sections[1] as DSection).ReferenceName);
            Assert.AreEqual("Mark 4:33-33", (Book.Sections[2] as DSection).ReferenceName);
            Assert.AreEqual("Mark 15:34-34", (Book.Sections[3] as DSection).ReferenceName);
        }
        #endregion
    }

}
