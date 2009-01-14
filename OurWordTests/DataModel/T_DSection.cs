/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DSection.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DSection class
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
    [TestFixture] public class T_DSection
    {
        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp]
        public void Setup()
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

        // Test Smarts -----------------------------------------------------------------------
        #region Method: void _WriteToFile(string sPathname, string[] vs)
        void _WriteToFile(string sPathname, string[] vs)
        {
            TextWriter W = JW_Util.GetTextWriter(sPathname);
            foreach (string s in vs)
                W.WriteLine(s);
            W.Close();
        }
        #endregion
        #region Method: string[] _ReadFromFile(string sPathname)
        string[] _ReadFromFile(string sPathname)
        {
            SfDb DB = new SfDb();
            DB.Read(ref sPathname);
            string[] vs = DB.ExtractData();
            return vs;
        }
        #endregion
        #region Method: void _Compare(string sTitle, string[] vsActual, string[] vsExpected)
        void _Compare(string sTitle, string[] vsActual, string[] vsExpected)
        {
            // Do a quick comparison to see if there are any differences
            bool bSame = true;
            if (vsExpected.Length != vsActual.Length)
                bSame = false;
            else
            {
                for (int k = 0; k < vsActual.Length; k++)
                {
                    if (vsExpected[k] != vsActual[k])
                        bSame = false;
                }
            }

            // Optionally output to console for debugging
            bool bConsoleOut = false;
            if (bConsoleOut)
            {

                Console.WriteLine("Test_DSectionIO ----- [" + sTitle + "] -----");

                if (bSame)
                    Console.Write("Sections are identical!");
                else
                {
                    Console.WriteLine("Expected=" + vsExpected.Length.ToString() +
                        "    Actual=" + vsActual.Length.ToString());
                    for (int n = 0; n < vsActual.Length && n < vsExpected.Length; n++)
                    {
                        Console.WriteLine("");
                        if (vsActual[n] != vsExpected[n])
                            Console.WriteLine("--DIFFERENT--");
                        Console.WriteLine("vExpected = {" + vsExpected[n] + "}");
                        Console.WriteLine("vActual   = {" + vsActual[n] + "}");
                    }
                }

                if (vsActual.Length > vsExpected.Length)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Actual was longer. Next line was:");
                    Console.WriteLine(vsActual[ vsExpected.Length ]);
                }
                if (vsActual.Length < vsExpected.Length)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Expected was longer. Next line was:");
                    Console.WriteLine(vsExpected[vsActual.Length]);
                }

                Console.WriteLine("");
            }

            // Compare
            Assert.AreEqual(vsExpected.Length, vsActual.Length);
            for (int k = 0; k < vsActual.Length; k++)
                Assert.AreEqual(vsExpected[k], vsActual[k]);
        }
        #endregion
        #region Method: void IO_TestEngine(string[] vsRaw, string[] vsSav)
        public void IO_TestEngine(string[] vsRaw, string[] vsSav)
        // vsRaw - typically data which we are importing, could have all 
        //            kinds of inconsistencies or problems.
        // vsSav - the cannonical/consistent format that we expect 
        //            OurWord to save to.
        // 
        {
            // Preliminary: Create the superstructure we need for a DBook
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.InitializeFactoryStyleSheet();
            G.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            G.Project.TargetTranslation = Translation;

            //////////////////////////////////////////////////////////////////////////////////
            // PART 1: IMPORT vsRaw AND SEE IF WE SAVE IT TO MATCH vsSAV. Thus we are testing
            // the import mechanism's ability to handle various stuff.
            //////////////////////////////////////////////////////////////////////////////////

            // Write out vsRaw to disk file
            string sPathname = GetTestPathName("TestDBIO1");
            _WriteToFile(sPathname, vsRaw);

            // Now read it into the book. This will test TransformIn and DBook/DSection Import
            DBook Book = new DBook("MRK", "");
            Translation.AddBook(Book);
            Book.AbsolutePathName = sPathname;
            Book.Load();

            // Now write it, to test TransformOut, & etc.
            Book.DeclareDirty();                 // It'll not write without this
            Book.Write();

            // Read the result from disk
            string[] vsResult = _ReadFromFile(Book.AbsolutePathName);

            // Compare what was written (vsResult) with what we expect (vsSav)
            _Compare("PART ONE", vsResult, vsSav);


            //////////////////////////////////////////////////////////////////////////////////
            // PART 2: GIVEN vsSav, READ IT, THEN WRITE IT, SEE IF THE READ/WRITE PRESERVES 
            // vsSav. Thus we are testing TransformIn, TransformOut, and the various IO 
            // routines in DBook and DSection
            //////////////////////////////////////////////////////////////////////////////////

            // First, write the "cannonical" data out to disk file
            sPathname = Test.GetPathName("TestDBIO2");
            _WriteToFile(sPathname, vsSav);

            // Now, read it into a book. This will do DB.TransformIn, as well as the
            // various read methods in DBook
            Translation.Books.Remove(Book);
            Book = new DBook("MRK", "");
            Translation.AddBook(Book);
            Book.AbsolutePathName = sPathname;
            Book.Load();

            // Now, write it back out. This will do DB.TransformOut, as well as the
            // various write methods in DBook.
            Book.DeclareDirty();                 // It'll not write without this
            Book.Write();

            // Finaly, read the result from disk
            vsResult = _ReadFromFile(Book.AbsolutePathName);

            // Compare what was written (vsResult) with what we expect (vsSav)
            _Compare("PART TWO", vsResult, vsSav);
        }
        #endregion

        // Individual Tests (each has a different data set) ----------------------------------
        #region TEST: IO_DataSet4 - Baikeno Mark 4:30-34
        #region TestData #4: s_vsRaw4
	    static public string[] s_vsRaw4 = new string[] 
	    {
		    "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
            "",
		    "\\rcrd MRK",
		    "\\h Mark",
		    "\\st The Gospel Of",
		    "\\mt Mark",
		    "\\id Mark",
            "",
		    "\\rcrd MRK 1",
		    "\\s Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
		    "\\bts The Lord Jesus give an example of a seed that is extremely tiny",
		    "\\nt sain = jenis biji yg kici ana",
		    "\\r (Mateus 13:31-32, 34; Lukas 13:18-19)",
		    "\\p",
		    "\\v 30",
		    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi " +
		    "nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes " +
		    "nabaab-took, tal antee sin namfau nok.",
		    "\\btvt Then Jesus spoke again, saying, <<I give another example like " +
		    "this: You(pl) can compare God's *social group. From just a few people, " +
		    "it nevertheless increases (in number), to the point that they are very " +
		    "many.",
		    "\\nt bonak = Setaria italica, Rote = botok ; an-ana' = very small ; " +
		    "fini = seed for planting ; minoon = banding, ; kle'o = few ; nabaab-took " +
		    "= tamba banyak",
            "\\p",
		    "\\v 31",
		    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
		    "\\btvt That is like a seed that is very tiny.",
		    "\\v 32",
		    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof " +
		    "kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
		    "\\btvt If we(inc) plant it (with dibble stick) it will grow to become " +
		    "a large tree. And birds will come looking for shade, and make nests " +
		    "in it.>>",
		    "\\nt tseen ~ [ceen] = plant with dibble stick ; hau tlaef = ranting " +
		    "pohon ; mafo' = sombar ; kuna' = sarang",
		    "\\p",
		    "\\v 33",
		    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
		    "\\btvt Jesus' way of teaching was according to their understanding.",
		    "\\v 34",
		    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' " +
		    "sin, In natoon lais leta' nane in na' naon ok-oke'.",
		    "\\btvt He taught people using only examples. But with His *disciples, He " +
		    "told the meaning of all the examples.",
		    "\\ud 17/Jun/2003"		
	    };
        #endregion
        [Test] public void IO_DataSet4()
        {
            #region TestData #4: vsSav4
            string[] vsSav4 = new string[] 
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
			    "\\vt Oke te, |iJesus namolok|r antein, mnak, <<Au uleta' 'tein on ii: Hi " +
			    "nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes " +
			    "nabaab-took, tal antee sin namfau nok.",
			    "\\btvt Then Jesus spoke again, saying, <<I give another example like " +
			    "this: You(pl) can compare God's *social group. From just a few people, " +
			    "it nevertheless increases (in number), to the point that they are very " +
			    "many.",
			    "\\nt bonak = Setaria italica, Rote = botok ; an-ana' = very small ; " +
			    "fini = seed for planting ; minoon = banding, ; kle'o = few ; nabaab-took " +
			    "= tamba banyak",
                "\\p",
			    "\\v 31",
			    "\\vt Nane namnees onle' fini le' in nesan an-ana' neis.",
			    "\\btvt That is like a seed that is very tiny.",
			    "\\v 32",
			    "\\vt Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof " +
			    "kolo neem namin mafo', ma nmo'en kuna' neu ne.>>",
			    "\\btvt If we(inc) plant it (with dibble stick) it will grow to become " +
			    "a large tree. And birds will come looking for shade, and make nests " +
			    "in it.>>",
			    "\\nt tseen ~ [ceen] = plant with dibble stick ; hau tlaef = ranting " +
			    "pohon ; mafo' = sombar ; kuna' = sarang",
			    "\\p",
			    "\\v 33",
			    "\\vt Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
			    "\\btvt Jesus' way of teaching was according to their understanding.",
			    "\\v 34",
			    "\\vt In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' " +
			    "sin, In natoon lais leta' nane in na' naon ok-oke'.",
			    "\\btvt He taught people using only examples. But with His *disciples, He " +
			    "told the meaning of all the examples.",
			    "\\ud 17/Jun/2003"		
            };
            #endregion

            IO_TestEngine(s_vsRaw4, vsSav4);
        }
        #endregion
        #region TEST: IO_DataSet5 - Pura Mark 14:03-14:09
        [Test] public void IO_DataSet5()
        {
            // Pura Mark 14:03-14:09
            // Verses redone, start at 1, not 3. Things in this data:
            // - Verse 7 has a unicode character.
            // - Verse 1 has ||'s, including 
            //     + ||fn - which should not be interpreted as a footnote
            //     + ||i - which should not be interpreted as +Italic.
            //     + ||r - which should not be interpted as -Italic
            // - Verse 2 has {{'s and }}'s. Occurring in a doublet, these should
            //     be presented to the user as literal singlets.
            #region TestData #5: vsRaw5
            string[] vsRaw5 = new string[] 
		    {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
                "",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
                "",
			    "\\rcrd Mark 1",
			    "\\c 1",
			    "\\s Ne he jangu ba mina menema e vili ele boal, ma Tuhang Yesus enang",
			    "\\r (Matius 26:6-13; Yohanis 12:1-8)",
			    "\\p",
			    "\\v 1",
			    "\\vt Abang Betania mi, ne nu ue ene Simon. Turang mi, ne ava aing veng " + 
			    "ororing, tagal dirang hapeburang aing veng. Ba sakarang, ana aung ila.",
			    "\\btvt This is some back translation.",
			    "\\p",
			    "\\vt Seng angu mu, o||ras angu ve||d-ved aung hoa jedung, Yesus ini ue " +
			    "ila Simon e hava mi nana. ||Oras ||ini nana, nehe jangu nu Yesus evele " +
			    "hoa Aing dapa. Ana ue botol nu pina ba ini var ma ening. ||Mina " +
			    "nemema asli ba mi evili talalu ele.|fn Seng, nebe jangu angu botol " +
			    "ememng angu||fn ma vil bue ening kivita. Mu ana boal ening to tu tahang-tahang " +
			    "mina angu ma Yesus ong tang||, e jadi tanda Yesus Aing ta janing",
			    "\\btvt This is some more back translation.",
			    "\\ft This is a footnote.",
			    "\\p",
			    "\\v 2",
			    "\\vt Ba ne }}ebeung iva di {{ue umurung}} nana. Oras ini eteing nehe jangu " +
			    "angu ening ula{{{{ng angu, mi ini ili il, e ini i ta tu}}tuk sombong " +
			    "hula, <<Hmm! Ne he {{jangu na ba {{ba anga, e ana ila mina}} menema " +
			    "e vili ele angu viat parsuma.",
			    "\\btvt Some back translation for verse two.",
			    "\\v 3",
			    "\\vt Lebe aung mina angu ana avali ba! E ila e hoang toang angu paul, " +
			    "ma ne malarat anaung ing enang! Se mina menema angu e vili veng, " +
			    "ma nenu e gaji tung nu veng hama.>>",
			    "\\btvt Forescore and seven years ago,",
			    "\\p ",
			    "\\v 4",
			    "\\vt Ba Yesus balas hula, <<Ini ake nehe jangu anga ening susa! Mang " +
			    "aing kilang ba! Na sanang, tagal mina menema anga ana ma Noboa " +
			    "veng obokong ila. ",
			    "\\btvt  our forefathers brought forth on this continant",
			    "\\v 5",
			    "\\vt Ne kasiang anaung salalu ae ing veng. Jadi, ini bisa taveding " +
			    "di ini ing tulung. Ba kalo Naing,|fn lung niang ila, se Na ing veng " +
			    "hama-hama niang ila. ",
			    "\\btvt Some back translating for verse 5",
                "\\ft A footnote for verse 5",
			    "\\cf 14:7: Ulangan 15:11",
			    "\\v 6",
			    "\\vt Ne e vetang lung niang ila. Mang nehe jangu anga vede mina obokong " +
			    "anga, ana sidiat ila ma Neboa veng etura, emang hula ana Ne baring " +
			    "veng bunga me at ila.|fn",
			    "\\btvt And verse six reads as so.",
                "\\nt And this is a translator note",
                "\\ft Verse six footnote",
			    "\\v 7",
			    "\\vt Benganit aung-aung, o`! Ta ang mi ba Tuhang Lahatala E Sirinta " +
			    "Aung ila alamula anga goleng, indi pasti nehe jangu e aung anga " +
			    "veng sirinta! E biar ne emampi aing vengani.>>",
			    "\\btvt A final backtranslation to end it all."
		    };
            #endregion
            #region TestData #5: vsSav5
            string[] vsSav5 = new string[] 
		    {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
			    "\\rcrd Mark 1",
			    "\\c 1",
			    "\\s Ne he jangu ba mina menema e vili ele boal, ma Tuhang Yesus enang",
			    "\\r (Matius 26:6-13; Yohanis 12:1-8)",
			    "\\p",
			    "\\v 1",
			    "\\vt Abang Betania mi, ne nu ue ene Simon. Turang mi, ne ava aing veng " + 
			    "ororing, tagal dirang hapeburang aing veng. Ba sakarang, ana aung ila.",
			    "\\btvt This is some back translation.",
			    "\\p",
			    "\\vt Seng angu mu, o||ras angu ve||d-ved aung hoa jedung, Yesus ini ue " +
			    "ila Simon e hava mi nana. ||Oras ||ini nana, nehe jangu nu Yesus evele " +
			    "hoa Aing dapa. Ana ue botol nu pina ba ini var ma ening. ||Mina " +
			    "nemema asli ba mi evili talalu ele.|fn",
			    "\\btvt This is some more back translation.",
			    "\\ft This is a footnote.",
                "\\vt Seng, nebe jangu angu botol ememng angu||fn ma vil bue ening kivita. " +
                "Mu ana boal ening to tu tahang-tahang mina angu ma Yesus ong tang||, e jadi " +
                "tanda Yesus Aing ta janing",
			    "\\p",
			    "\\v 2",
			    "\\vt Ba ne }}ebeung iva di {{ue umurung}} nana. Oras ini eteing nehe jangu " +
			    "angu ening ula{{{{ng angu, mi ini ili il, e ini i ta tu}}tuk sombong " +
			    "hula, <<Hmm! Ne he {{jangu na ba {{ba anga, e ana ila mina}} menema " +
			    "e vili ele angu viat parsuma.",
			    "\\btvt Some back translation for verse two.",
			    "\\v 3",
			    "\\vt Lebe aung mina angu ana avali ba! E ila e hoang toang angu paul, " +
			    "ma ne malarat anaung ing enang! Se mina menema angu e vili veng, " +
			    "ma nenu e gaji tung nu veng hama.>>",
			    "\\btvt Forescore and seven years ago,",
			    "\\p",
			    "\\v 4",
			    "\\vt Ba Yesus balas hula, <<Ini ake nehe jangu anga ening susa! Mang " +
			    "aing kilang ba! Na sanang, tagal mina menema anga ana ma Noboa " +
			    "veng obokong ila.",
			    "\\btvt our forefathers brought forth on this continant",
			    "\\v 5",
			    "\\vt Ne kasiang anaung salalu ae ing veng. Jadi, ini bisa taveding " +
			    "di ini ing tulung. Ba kalo Naing,|fn",
			    "\\btvt Some back translating for verse 5",
                "\\ft A footnote for verse 5",
                "\\vt lung niang ila, se Na ing veng hama-hama niang ila.",
			    "\\cf 14:7: Ulangan 15:11",
			    "\\v 6",
			    "\\vt Ne e vetang lung niang ila. Mang nehe jangu anga vede mina obokong " +
			    "anga, ana sidiat ila ma Neboa veng etura, emang hula ana Ne baring " +
			    "veng bunga me at ila.|fn",
			    "\\btvt And verse six reads as so.",
                "\\nt And this is a translator note",
                "\\ft Verse six footnote",
			    "\\v 7",
			    "\\vt Benganit aung-aung, o`! Ta ang mi ba Tuhang Lahatala E Sirinta " +
			    "Aung ila alamula anga goleng, indi pasti nehe jangu e aung anga " +
			    "veng sirinta! E biar ne emampi aing vengani.>>",
			    "\\btvt A final backtranslation to end it all."
		    };
            #endregion

            IO_TestEngine(vsRaw5, vsSav5);
        }
        #endregion
        #region TEST: IO_DataSet6 - Helong Acts 4:01-4:04
        [Test] public void IO_DataSet6()
        {
            #region TestData #6: vsRaw6
            string[] vsRaw6 = new string[] 
		    {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
                "",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
                "",
			    "\\rcrd act4.1-4.22",
			    "\\c 4",
			    "\\s Tulu-tulu agama las haman Petrus nol Yohanis maas tala",
			    "\\bts The heads of religion summon Petrus and Yohanis to come appear.before [them]",
			    "\\p",
			    "\\v 1",
			    "\\vt Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, " +
				    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				    "agama Saduki. Oen maas komali le ahan Petrus nol Yohanis.",
			    "\\btvt At that time, Petrus and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Petrus and Yohanis.",
			    "\\nt doh =doha;  tala=mangada",
			    "\\v 2",
			    "\\vt Oen komali lole Petrus nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Petrus and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\"|fn " +
				    "(check two kon)",
			    "\\ft 4:1-2: Atuil deng partai agama Saduki, oen sium in tui na lo " +
				    "man noen atuil mate haup in nuli pait.",
			    "\\btft 4:1-2: People from the religious party Saduki, they did not accept that " +
				    "teaching that says dead people can live again.",
			    "\\v 3",
			    "\\vt Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
				    "deng lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
				    "Le ola ka halas-sam, oen nehan dais na.",
			    "\\btvt And so they ordered their people to go capture the two of them. But " +
				    "because the sun already wanted to set, then they pushed [&] entered " +
				    "the two of them in jail. So.that the next day, they could take.care.of " +
				    "that affair/litigation/problem.",
			    "\\nt nehan = urus, mengatur",
			    "\\v 4",
			    "\\vt Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their people increased a lot  " +
				    "approximately to five thousand people.",
			    "\\cat c:\\graphics\\cook\\cnt\\cn01901b.tif",
			    "\\ref width:7.0cm;textWrapping:around;horizontalPosition:right",
			    "\\cap Atuil-atuil tene kas klaa Petrus nol tapang ngas",
			    "\\btcap The leaders accuse/criticise Petrus and his friends"
		    };
            #endregion
            #region TestData #6: vsSav6
            string[] vsSav6 = new string[] 
		    {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\st The Gospel Of",
			    "\\mt Mark",
			    "\\id Mark",
			    "\\rcrd act4.1-4.22",
			    "\\c 4",
			    "\\s Tulu-tulu agama las haman Petrus nol Yohanis maas tala",
			    "\\bts The heads of religion summon Petrus and Yohanis to come appear.before [them]",
			    "\\p",
			    "\\v 1",
			    "\\vt Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, " +
				    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				    "agama Saduki. Oen maas komali le ahan Petrus nol Yohanis.",
			    "\\btvt At that time, Petrus and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Petrus and Yohanis.",
			    "\\nt doh =doha; tala=mangada",
			    "\\v 2",
			    "\\vt Oen komali lole Petrus nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Petrus and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)",
			    "\\ft 4:1-2: Atuil deng partai agama Saduki, oen sium in tui na lo " +
				    "man noen atuil mate haup in nuli pait.",
			    "\\btft 4:1-2: People from the religious party Saduki, they did not accept that " +
				    "teaching that says dead people can live again.",
			    "\\v 3",
			    "\\vt Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
				    "deng lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
				    "Le ola ka halas-sam, oen nehan dais na.",
			    "\\btvt And so they ordered their people to go capture the two of them. But " +
				    "because the sun already wanted to set, then they pushed [&] entered " +
				    "the two of them in jail. So.that the next day, they could take.care.of " +
				    "that affair/litigation/problem.",
			    "\\nt nehan = urus, mengatur",
			    "\\v 4",
			    "\\vt Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their people increased a lot " +
				    "approximately to five thousand people.",
			    "\\cat c:\\graphics\\cook\\cnt\\cn01901b.tif",
			    "\\ref width:7.0cm;textWrapping:around;horizontalPosition:right",
			    "\\cap Atuil-atuil tene kas klaa Petrus nol tapang ngas",
			    "\\btcap The leaders accuse/criticise Petrus and his friends"
		    };
            #endregion

            IO_TestEngine(vsRaw6, vsSav6);
        }
        #endregion
        #region TEST: DontReorderMarkEndings
        [Test] public void DontReorderMarkEndings()
        {
            #region TestData - RAW
            string[] vsRaw = new string[] 
		    {
			    "\\_sh v3.0 7 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
                "",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\mt Mark",
			    "\\id Mark",
                "",
                "\\rcrd MRK 16.1-16.8",
                "\\c 16",
                "\\s Tuhan Yesus idop kambali!",
                "\\p",
                "\\v 1",
                "\\vt Dia pung beso,",
                "",
                "\\rcrd MRK 16.9-16.11",
                "\\s [Tuhan Yesus katumu deng Maria dari Magdala",
                "\\p",
                "\\v 9",
                "\\vt Hari Minggu papagi tu,",
                "",
                "\\rcrd MRK 16.12-16.13",
                "\\s Tuhan Yesus kasi tunju muka sang dua orang lai",
                "\\p",
                "\\v 12",
                "\\vt Abis itu,",
                "",
                "\\rcrd MRK 16.14-16.18",
                "\\s Tuhan Yesus kasi tunju muka sang ana bua dong samua",
                "\\p",
                "\\v 14",
                "\\vt Abis ju",
                "",
                "\\rcrd MRK 16.19-16.20",
                "\\s Tuhan Yesus ta'angka nae pi sorga",
                "\\p",
                "\\v 19",
                "\\vt Abis ba'omong deng Dia",
                "",
                "\\rcrd MRK 17.9-17.10",
                "\\s [MARKUS PUNG TUTU CARITA, IKO TULISAN DOLU-DOLU YANG LAEN",
                "\\p",
                "\\v 9",
                "\\vt Waktu parampuan tiga orang tu,"
            };
            #endregion
            #region TestData = SAV
            string[] vsSav = new string[] 
		    {
			    "\\_sh v3.0 7 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\h Mark",
			    "\\mt Mark",
			    "\\id Mark",
                "\\rcrd MRK 16.1-16.8",
                "\\c 16",
                "\\s Tuhan Yesus idop kambali!",
                "\\p",
                "\\v 1",
                "\\vt Dia pung beso,",
                "\\rcrd MRK 16.9-16.11",
                "\\s [Tuhan Yesus katumu deng Maria dari Magdala",
                "\\p",
                "\\v 9",
                "\\vt Hari Minggu papagi tu,",
                "\\rcrd MRK 16.12-16.13",
                "\\s Tuhan Yesus kasi tunju muka sang dua orang lai",
                "\\p",
                "\\v 12",
                "\\vt Abis itu,",
                "\\rcrd MRK 16.14-16.18",
                "\\s Tuhan Yesus kasi tunju muka sang ana bua dong samua",
                "\\p",
                "\\v 14",
                "\\vt Abis ju",
                "\\rcrd MRK 16.19-16.20",
                "\\s Tuhan Yesus ta'angka nae pi sorga",
                "\\p",
                "\\v 19",
                "\\vt Abis ba'omong deng Dia",
                "\\rcrd MRK 17.9-17.10",
                "\\s [MARKUS PUNG TUTU CARITA, IKO TULISAN DOLU-DOLU YANG LAEN",
                "\\p",
                "\\v 9",
                "\\vt Waktu parampuan tiga orang tu,"
            };
            #endregion

            IO_TestEngine(vsRaw, vsSav);
        }
        #endregion

        // Other Tests -----------------------------------------------------------------------
        #region ParseVerseStrings
        [Test] public void ParseVerseStrings()
        {
            string sVersePart = "";
            string sTextPart = "";

            // Test: Just a plain old verse number
            string sInput = "3 Ije lais alekot.";
            DSection.ParseVerseString(sInput, ref sVersePart, ref sTextPart);
            Assert.AreEqual(sVersePart, "3");
            Assert.AreEqual(sTextPart, "Ije lais alekot.");

            // Test: A verse bridge with extra blank spaces
            sInput = "24 - 26 Ije lais alekot.";
            DSection.ParseVerseString(sInput, ref sVersePart, ref sTextPart);
            Assert.AreEqual(sVersePart, "24-26");
            Assert.AreEqual(sTextPart, "Ije lais alekot.");

            // Test: A verse bridge with letters
            sInput = "24b - 26a Ije lais alekot.";
            DSection.ParseVerseString(sInput, ref sVersePart, ref sTextPart);
            Assert.AreEqual(sVersePart, "24b-26a");
            Assert.AreEqual(sTextPart, "Ije lais alekot.");

            // Test: A verse bridge without spaces
            sInput = "24-26a Ije lais alekot.";
            DSection.ParseVerseString(sInput, ref sVersePart, ref sTextPart);
            Assert.AreEqual(sVersePart, "24-26a");
            Assert.AreEqual(sTextPart, "Ije lais alekot.");

            //Console.WriteLine("Input = >" + sInput + "<");
            //Console.WriteLine("Verse = >" + sVersePart + "<");
            //Console.WriteLine("Text  = >" + sTextPart + "<");
        }
        #endregion




    }
}
