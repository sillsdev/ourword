/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Drafting.cs
 * Author:  John Wimbish
 * Created: 15 May 2008
 * Purpose: Tests the Drafting layout class
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

namespace OurWordTests.Edit
{
    [TestFixture] public class T_Drafting
    {
        DBook FrontBook;
        DBook TargetBook;

        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.EnsureInitialized();
            G.Project.DisplayName = "Drafting Test Project";

            G.Project.FrontTranslation = new DTranslation("Front Translation", "Latin", "Latin");
            FrontBook = new DBook("GEN", "");
            G.Project.FrontTranslation.AddBook(FrontBook);

            G.Project.TargetTranslation = new DTranslation("Target Translation", "Latin", "Latin");
            TargetBook = new DBook("GEN", "");
            G.Project.TargetTranslation.AddBook(TargetBook);
        }
        #endregion
        #region TearDown
        [TearDown]
        public void TearDown()
        {
            OurWordMain.Project = null;
        }
        #endregion
        #region Method: void ReadIntoBook(DBook book, string[] vs)
        void ReadIntoBook(DBook book, string[] vs)
        {
            // Get a file name for doing the io
            string sPathname = JWU.NUnit_TestFileFolder +
                Path.DirectorySeparatorChar + "DraftLayoutTest.x";

            // Write it out raw to a disk file
            TextWriter W = JW_Util.GetTextWriter(sPathname);
            foreach (string s in vs)
                W.WriteLine(s);
            W.Close();

            // Now read it into the book
            book.AbsolutePathName = sPathname;
            book.Load();
        }
        #endregion

        // Mismatched Paragraph Alignment Tests ----------------------------------------------
        #region vsTarget - Huichol
        static string[] vsTarget = new string[]
        {
		    "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd GEN",
		    "\\h Sutüa",
		    "\\mt Sutüarica",
		    "\\id GEN - Huichol (hch)",
            "\\rcrd GEN 01.01-02.04",
            "\\c 1",
            "\\s Cacaüyari Naime Tiniutavevieni",
            "\\p ",
            "\\q ",
            "\\v 1",
            "\\vt Sutüapai Cacaüyari muyuavi niutavevieni,",
            "\\q2 ",
            "\\vt cuieta niutavevieni.",
            "\\q ",
            "\\v 2",
            "\\vt 'Ana puyüvicai xeicüa naitü cuiepa, cuie havaicü pücahecumacai, naisarie puyüvicai hapa.",
            "\\q2 ",
            "\\vt Cacaüyari 'iyarieya ha heima",
            "\\q2 ",
            "\\vt püyeyuanenecai.",
            "\\q ",
            "\\v 3",
            "\\vt Hicü Cacaüyari müpaü niutayuni: «Hecüariya que'uneni.»",
            "\\q2 ",
            "\\vt Hicü hecüariya niuneni.",
            "\\q ",
            "\\v 4",
            "\\vt Nixeiya Cacaüyari hecüariya, «'Aixüa pü'ane,» niutayüni,",
            "\\q2 ",
            "\\vt 'Ana nipata yüriya Cacaüyari.",
            "\\q ",
            "\\v 5",
            "\\vt «Hecüariya tucari pütiteva,» niutayüni,",
            "\\q2 ",
            "\\vt «Yüriya tücari pütiteva,» niutayüni.",
            "\\q ",
            "\\vt 'Ana caninuani yüriya, ximerita canayani,",
            "\\q2 ",
            "\\vt mexüacame tucari nayani, ximeri taicai.",
            "\\q ",
            "\\v 6",
            "\\vt Cacaüyari müpaü niutayüni: «Muyuavi que'uneni,",
            "\\q2 ",
            "\\vt que'ipata ha taheima mieme cuiepa mieme.»",
            "\\q ",
            "\\v 7",
            "\\vt Müpaü tiniuyurieni Cacaüyari. Nitavevieni sutümeyari,",
            "\\q2 ",
            "\\vt nipata ha, haivitüritüa mütamacacü,",
            "\\q ",
            "\\vt hixüata 'atü hetüana caniyuhayeva ha,",
            "\\q2 ",
            "\\vt hapa caniutaveviyani taheima muyema.",
            "\\q ",
            "\\v 8",
            "\\vt Sutümeyari, Cacaüyari «muyuavi» tiniuterüva.",
            "\\q2 ",
            "\\vt Mericüsü taicai nayani, ximerita nayani,",
            "\\q2 ",
            "\\vt Hutarieca mieme tucari canayani.",
            "\\q ",
            "\\v 9",
            "\\vt Hicü Cacaüyari müpaü niutayüni: «Ha muyuavitüa mütama",
            "\\q2 ",
            "\\vt que'uyucuxeürieni 'axeicüa,",
            "\\q2 ",
            "\\vt 'uvaquitü que'ayani.»",
            "\\q ",
            "\\vt Quemutayü müpaü pütiüyü.",
            "\\v 10",
            "\\vt Muvaqui Cacaüyari tiniterüva «cuie,»",
            "\\q2 ",
            "\\vt ha muyucuxeüri tiniterüva «haramara.»",
            "\\q ",
            "\\vt 'Ixeiyatü Cacaüyari «'Aixüa pü'ane,» niutayüni.",
            "\\q ",
            "\\v 11",
            "\\vt 'Arique cacaüyari müpaü niutayüni: «Cuiepa que'utineni tupiriya,",
            "\\q2 ",
            "\\vt yutacari yuhasiyari hexeiyatü,",
            "\\q ",
            "\\vt cüyexi meta yutacari hexeiyatü yuhasiyari.",
            "\\q2 ",
            "\\vt naitü xexuitü quemütiupitüarie.»",
            "\\q ",
            "\\vt Müpaü tiniuyüni.",
            "\\v 12",
            "\\vt 'Ana catiniutineni naitü,",
            "\\q2 ",
            "\\vt tupiriyate, yutacari yuhasiyari hexeiyatü,",
            "\\q ",
            "\\vt cüyexi 'iteüri yutacari yuhasiyari hexeiyatü,",
            "\\q2 ",
            "\\vt naitü quemütiupitüarie.",
            "\\q ",
            "\\vt Mericüsü Cacaüyari «'Aixüa pü'ane,» niutayüni.",
            "\\q2 ",
            "\\v 13",
            "\\vt Hicü taicai nayani, hicütari ximeri nayani,",
            "\\q2 ",
            "\\vt hairieca tucari nayani.",
            "\\q ",
            "\\v 14",
            "\\vt Müpaüta niutayüni Cacaüyari: «Meque'uneni muyuavisie hecüarivivamete,",
            "\\q2 ",
            "\\vt meque'ipata tucari tücari,",
            "\\q2 ",
            "\\vt mücü cüxemete 'inüari meque'acüne vitari taxari 'inünüatamete,",
            "\\q2 ",
            "\\vt tücarisie mieme viyarisie mieme.",
            "\\q ",
            "\\v 15",
            "\\vt Hecüariya quehecüarivieca muyuavisie,",
            "\\q2 ",
            "\\vt naime quehecüariyani cuiepa.»",
            "\\q ",
            "\\vt Müpaü tiniuyüni.",
            "\\v 16",
            "\\vt Cacaüyari canitivevieni hutame mamarivaveme cüxeme,",
            "\\q2 ",
            "\\vt tau niutavevieni mariveme mütita'aitacü tucaricü,",
            "\\q ",
            "\\vt mesata tiutavevi mütita'aitacü tücacüta,",
            "\\q2 ",
            "\\vt xuravesixi meta tücaricü miemete.",
            "\\q ",
            "\\v 17",
            "\\vt Cacaüyari cüxeme niuyevieni muyuavisie",
            "\\q2 ",
            "\\vt xuravesixi mühecüariviecacü cuiepa.",
            "\\q ",
            "\\v 18",
            "\\vt Püvarutivevi memüteta'aitacü tucaricü tücaricü,",
            "\\q2 ",
            "\\vt hecüariya mipatacü yüriya.",
            "\\q ",
            "\\vt Hicü Cacaüyari «'Aixüa pü'ane,» niutayüni.",
            "\\q2 ",
            "\\v 19",
            "\\vt Hicü pucuyürixü, hicütari caniucuhecüare,",
            "\\q2 ",
            "\\vt ximeri canayani naurieca tucarisie.",
            "\\q ",
            "\\v 20",
            "\\vt Müpaü niutayüni Cacaüyari: «Mequehüneni quesüte hapa,",
            "\\q2 ",
            "\\vt mequeheüvütücani viquixi cuiepa meutitevasiepai,",
            "\\q2 ",
            "\\vt muyuavisiepai mequeheuvütücani.»",
            "\\q ",
            "\\v 21",
            "\\vt Mericüsü catinivarutivevieni yunaime 'amemütemamarivave haramarasie memu'uva,",
            "\\q2 ",
            "\\vt yunaime memüte'ayeneniere 'esimepepetü memücuyuatüca.",
            "\\q ",
            "\\vt yunaime viquixi,",
            "\\q2 ",
            "\\vt müpaü quemütivarupitüa muyuavisie miemete.",
            "\\q ",
            "\\vt Mericüsü vaxeiyatü Cacaüyari «'Aixüa pü'ane» niutayüni.",
            "\\q2 ",
            "\\v 22",
            "\\vt 'Aixüa nivaruyurieni müpaü 'utaitü,",
            "\\q ",
            "\\vt «Xequeneyuxiütüaca xequeneyumüiriyani,",
            "\\q2 ",
            "\\vt xequenehüneni hapa haramarasie.",
            "\\q2 ",
            "\\vt Viquixi meta xequeneyümüiriyani cuiepa.»",
            "\\q ",
            "\\v 23",
            "\\vt Hicü pucuyürixü, hicütari caniucuhecüare,",
            "\\q2 ",
            "\\vt ximeri canayani 'auxüvirieca tucarisie.",
            "\\q ",
            "\\v 24",
            "\\vt Müpaü niutayüni Cacaüyari: «Xequeneyümüiriyani cuiepa yunaitü xemayeyuyurini,",
            "\\q2 ",
            "\\vt yunaitü memüteyutevasinüa, yeutari meta,",
            "\\q2 ",
            "\\vt yuhucama memucu'uva cuiepa, yunaime quemütivarupitüa.»",
            "\\q ",
            "\\vt Müpaü pütiuyü.",
            "\\v 25",
            "\\vt Cacaüyari yunaime catinivarunetüani memüteyutevasinüa,",
            "\\q2 ",
            "\\vt yeutari, yunaime memüca'ucünivave yuhucama memucu'uva,",
            "\\q2 ",
            "\\vt yunaime que mütivarupitüa.",
            "\\q ",
            "\\vt Cacaüyari müpaü niutayüni, «'Aixüa pü'ane».",
            "\\q2 ",
            "\\v 26",
            "\\vt Müpaüta niutayüni: «Tevi tepütavevieni",
            "\\q2 ",
            "\\vt tahepaü 'aneme tahepaü tiyücühüaveme.",
            "\\q ",
            "\\vt Pütivata'aitüani quesüte haramarasie miemete,",
            "\\q2 ",
            "\\vt viquixi meta muyuavisie miemete,",
            "\\q ",
            "\\vt yunaime memüteyutevasinüa,",
            "\\q2 ",
            "\\vt yeutari,",
            "\\q ",
            "\\vt yunaime memüca'ucünivave",
            "\\q2 ",
            "\\vt yuhucama memucu'uva cuiepa.»",
            "\\q ",
            "\\v 27",
            "\\vt Cacaüyari tevi niutavevieni yuhepaü 'aneme,",
            "\\q2 ",
            "\\vt pitavevi 'inüarieya mayanicü yuhepaü raca'eriecame.",
            "\\q ",
            "\\vt 'Uquitüme putavevi, 'ucata putavevi.",
            "\\q2 ",
            "\\v 28",
            "\\vt 'Aixüa nivaruyurieni müpaü tivarutahüaveca,",
            "\\q ",
            "\\vt «Xequenexiüca xequeneyümüiriyani,",
            "\\q2 ",
            "\\vt cuiepa xequenehüneni xequetene'aitani.",
            "\\q ",
            "\\vt Xepütevata'aitüani quesüte haramarasie miemete, viquixi yunaime muyuavisie miemete,",
            "\\q2 ",
            "\\vt yunaime memucünivave yuhucama memucu'uva cuiepa.»",
            "\\q ",
            "\\v 29",
            "\\vt Müpaüta tinivarutahüave: «Ne nepüxemini cuiepa mieme 'iteüri,",
            "\\q2 ",
            "\\vt naime 'iteüri yuhasiyari hexeiyame,",
            "\\q ",
            "\\vt naime cüyexi yutacari yuhasiyari hexeiyame.",
            "\\q2 ",
            "\\vt Mücü naitü xe'icuai pürayani.",
            "\\q ",
            "\\v 30",
            "\\vt Tupiriyata nepüvamini müsisiüravi, 'icuai payani,",
            "\\q2 ",
            "\\vt yunaitü yeutari cuiepa memütexuave,",
            "\\q2 ",
            "\\vt yunaitü viquixi muyuavisie miemete",
            "\\q2 ",
            "\\vt yunaitü memayeyuyurini",
            "\\q2 ",
            "\\vt memeumaquetüca cuiepa.»",
            "\\q ",
            "\\vt Quemutayü müpaü pütiüyü.",
            "\\v 31",
            "\\vt Cacaüyari naime tiniuxeiya tita mütiutivevi.",
            "\\q2 ",
            "\\vt «'Aixüa pü'ane,» niutayuni.",
            "\\q ",
            "\\vt Hicü niucüyure, hicütari niucahecüare,",
            "\\q2 ",
            "\\vt 'ataxevirieca tucari nayani.",
            "\\c 2",
            "\\q2 ",
            "\\v 1",
            "\\vt Müpaü 'anetü niuyuhayeva nürietü muyuavi cuie meta naitü tita hesiena mütixuave.",
            "\\q ",
            "\\v 2",
            "\\vt 'Atahutarieca tucarisie, Cacaüyari niuca'uxipieni,",
            "\\q2 ",
            "\\vt naime mütiunücü yu'uximayasica quemütiuyühüavixü.",
            "\\q2 ",
            "\\v 3",
            "\\vt Cacaüyari 'aixüa catiniuyurieni 'atahutarieca tucari hepaüsita. Nipata yuhesie mieme",
            "\\q2 ",
            "\\vt 'ana müca'uxipicü, naime yu'uximayasica 'unüca que mütitavevi muyuavi cuie meta.",
            "\\q2 ",
            "\\v 4",
            "\\vt Quepaucua Cacaüyaritütü Yave mitavevi muyuavi cuie meta,",
            "\\q2 ",
            "\\vt 'icü mücü pü'üxasiyari."
        };
        #endregion
        #region Method: void MismatchedParaAlignment(int cExpectedPairsCount, string[] vsFront)
        void MismatchedParaAlignment(int cExpectedPairsCount, string[] vsFront)
        {
            // Initialize the books
            ReadIntoBook(FrontBook, vsFront);
            ReadIntoBook(TargetBook, vsTarget);

            // Set the sections
            G.Project.Nav.GoToFirstAvailableBook();
            G.Project.Nav.GoToFirstSection();

            // Conduct the alignment routine
            WndDrafting.ParagraphAlignmentPair[] v = WndDrafting.ScanForAlignmentPairs(
                0, G.SFront.Paragraphs.Count,
                0, G.STarget.Paragraphs.Count);
            WndDrafting.AddAlignmentCounts(v, 
                G.SFront.Paragraphs.Count, 
                G.STarget.Paragraphs.Count);

            // Temp code to write out the results to a desktop file
            //TextWriter W = JW_Util.GetTextWriter("C:\\Users\\JWimbish\\Desktop\\Output.txt");
            //foreach (WndDrafting.ParagraphAlignmentPair pair in v)
            //    W.WriteLine(pair.DebugString);
            //W.Close();

            // Temp code to write out to the console
            //Console.WriteLine("Total Count of Pairs = " + v.Length.ToString());
            //foreach (WndDrafting.ParagraphAlignmentPair pair in v)
            //    Console.WriteLine(pair.DebugString);

            // Compare against what we would expect. Any change in the overall count (the first
            // test here) would indicate that something has changed; as the counts represent
            // what we should get if the routine operates correctly.
            Assert.AreEqual(cExpectedPairsCount, v.Length);
            int cFront = 0;
            int cTarget = 0;
            foreach (WndDrafting.ParagraphAlignmentPair pair in v)
            {
                DParagraph pF = G.SFront.Paragraphs[pair.iFront] as DParagraph;
                DParagraph pT = G.STarget.Paragraphs[pair.iTarget] as DParagraph;

                Assert.AreEqual(pF.FirstActualVerseNumber, pair.VerseNo);
                Assert.AreEqual(pT.FirstActualVerseNumber, pair.VerseNo);
                Assert.AreNotEqual(-1, pair.VerseNo);

                cFront += pair.cFront;
                cTarget += pair.cTarget;
            }
            Assert.AreEqual(G.SFront.Paragraphs.Count, cFront);
            Assert.AreEqual(G.STarget.Paragraphs.Count, cTarget);
        }
        #endregion

        #region Test: MismatchedParaAlignment_Many
        [Test] public void MismatchedParaAlignment_Many()
            // We're using poetry from Huichol Genesis 1
        {
            #region vsFront - Spanish with lots of paragraphs
            string[] vsFront = new string[]
            {
                "\\_sh v3.0 72 SHW-Scripture",
                "\\_DateStampHasFourDigitYear ",
                "\\rcrd GEN",
                "\\h Gn",
                "\\mt Génesis",
                "\\id GEN",
                "\\rcrd GEN",
                "\\c 1",
                "\\s La creación",
                "\\p ",
                "\\q ",
                "\\v 1",
                "\\vt Dios, en el principio,",
                "\\q2 ",
                "\\vt creó los cielos y la tierra.",
                "\\q ",
                "\\v 2",
                "\\vt La tierra era un caos total,",
                "\\q2 ",
                "\\vt las tinieblas cubrían el abismo,",
                "\\q ",
                "\\vt y el Espíritu de Dios iba y venía",
                "\\q2 ",
                "\\vt sobre la superficie de las aguas.",
                "\\q ",
                "\\v 3",
                "\\vt Y dijo Dios: «¡Que exista la luz!»",
                "\\q2 ",
                "\\vt Y la luz llegó a existir.",
                "\\q ",
                "\\v 4",
                "\\vt Dios consideró que la luz era buena",
                "\\q2 ",
                "\\vt y la separó de las tinieblas.",
                "\\q ",
                "\\v 5",
                "\\vt A la luz la llamó «día»,",
                "\\q2 ",
                "\\vt y a las tinieblas, «noche».",
                "\\q ",
                "\\vt Y vino la noche, y llegó la mañana:",
                "\\q2 ",
                "\\vt ése fue el primer día.",
                "\\q ",
                "\\v 6",
                "\\vt Y dijo Dios: «¡Que exista el firmamento",
                "\\q2 ",
                "\\vt en medio de las aguas, y que las separe!»",
                "\\q ",
                "\\v 7",
                "\\vt Y así sucedió: Dios hizo el firmamento",
                "\\q2 ",
                "\\vt y separó las aguas que están abajo,",
                "\\q2 ",
                "\\vt de las aguas que están arriba.",
                "\\q ",
                "\\v 8",
                "\\vt Al firmamento Dios lo llamó «cielo».",
                "\\q2 ",
                "\\vt Y vino la noche, y llegó la mañana:",
                "\\q2 ",
                "\\vt ése fue el segundo día.",
                "\\q ",
                "\\v 9",
                "\\vt Y dijo Dios: «¡Que las aguas debajo del cielo",
                "\\q2 ",
                "\\vt se reúnan en un solo lugar,",
                "\\q2 ",
                "\\vt y que aparezca lo seco!»",
                "\\q ",
                "\\vt Y así sucedió.",
                "\\v 10",
                "\\vt A lo seco Dios lo llamó «tierra»,",
                "\\q2 ",
                "\\vt y al conjunto de aguas lo llamó «mar».",
                "\\q ",
                "\\vt Y Dios consideró que esto era bueno.",
                "\\q ",
                "\\v 11",
                "\\vt Y dijo Dios: «¡Que haya vegetación sobre la tierra;",
                "\\q2 ",
                "\\vt que ésta produzca hierbas que den semilla,",
                "\\q ",
                "\\vt y árboles que den su fruto con semilla,",
                "\\q2 ",
                "\\vt todos según su especie!»",
                "\\q ",
                "\\vt Y así sucedió.",
                "\\v 12",
                "\\vt Comenzó a brotar la vegetación:",
                "\\q2 ",
                "\\vt hierbas que dan semilla,",
                "\\q ",
                "\\vt y árboles que dan su fruto con semilla,",
                "\\q2 ",
                "\\vt todos según su especie.",
                "\\q ",
                "\\vt Y Dios consideró que esto era bueno.",
                "\\q2 ",
                "\\v 13",
                "\\vt Y vino la noche, y llegó la mañana:",
                "\\q2 ",
                "\\vt ése fue el tercer día.",
                "\\q ",
                "\\v 14",
                "\\vt Y dijo Dios: «¡Que haya luces en el firmamento",
                "\\q2 ",
                "\\vt que separen el día de la noche;",
                "\\q ",
                "\\vt que sirvan como señales de las estaciones,",
                "\\q2 ",
                "\\vt de los días y de los años,",
                "\\q ",
                "\\v 15",
                "\\vt y que brillen en el firmamento",
                "\\q2 ",
                "\\vt para iluminar la tierra!»",
                "\\q ",
                "\\vt Y sucedió así.",
                "\\v 16",
                "\\vt Dios hizo los dos grandes astros:",
                "\\q2 ",
                "\\vt el astro mayor para gobernar el día,",
                "\\q ",
                "\\vt y el menor para gobernar la noche.",
                "\\q2 ",
                "\\vt También hizo las estrellas.",
                "\\q ",
                "\\v 17",
                "\\vt Dios colocó en el firmamento",
                "\\q2 ",
                "\\vt los astros para alumbrar la tierra.",
                "\\q ",
                "\\v 18",
                "\\vt Los hizo para gobernar el día y la noche,",
                "\\q2 ",
                "\\vt y para separar la luz de las tinieblas.",
                "\\q ",
                "\\vt Y Dios consideró que esto era bueno.",
                "\\q2 ",
                "\\v 19",
                "\\vt Y vino la noche, y llegó la mañana:",
                "\\q2 ",
                "\\vt ése fue el cuarto día.",
                "\\q ",
                "\\v 20",
                "\\vt Y dijo Dios: «¡Que rebosen de seres vivientes las aguas,",
                "\\q2 ",
                "\\vt y que vuelen las aves sobre la tierra",
                "\\q2 ",
                "\\vt a lo largo del firmamento!»",
                "\\q ",
                "\\v 21",
                "\\vt Y creó Dios los grandes animales marinos,",
                "\\q2 ",
                "\\vt y todos los seres vivientes",
                "\\q2 ",
                "\\vt que se mueven y pululan en las aguas",
                "\\q ",
                "\\vt y todas las aves,",
                "\\q2 ",
                "\\vt según su especie.",
                "\\q ",
                "\\vt Y Dios consideró que esto era bueno,",
                "\\q2 ",
                "\\v 22",
                "\\vt y los bendijo con estas palabras:",
                "\\q ",
                "\\vt «Sean fructíferos y multiplíquense;",
                "\\q2 ",
                "\\vt llenen las aguas de los mares.",
                "\\q2 ",
                "\\vt ¡Que las aves se multipliquen sobre la tierra!»",
                "\\q ",
                "\\v 23",
                "\\vt Y vino la noche, y llegó la mañana:",
                "\\q2 ",
                "\\vt ése fue el quinto día.",
                "\\q ",
                "\\v 24",
                "\\vt Y dijo Dios: «¡Que produzca la tierra seres vivientes:",
                "\\q2 ",
                "\\vt animales domésticos, animales salvajes,",
                "\\q2 ",
                "\\vt y reptiles, según su especie!»",
                "\\q ",
                "\\vt Y sucedió así.",
                "\\v 25",
                "\\vt Dios hizo los animales domésticos,",
                "\\q2 ",
                "\\vt los animales salvajes, y todos los reptiles,",
                "\\q2 ",
                "\\vt según su especie.",
                "\\q ",
                "\\vt Y Dios consideró que esto era bueno,",
                "\\q2 ",
                "\\v 26",
                "\\vt y dijo: «Hagamos al **ser humano",
                "\\q2 ",
                "\\vt a nuestra imagen y semejanza.",
                "\\q ",
                "\\vt Que tenga dominio sobre los peces del mar,",
                "\\q2 ",
                "\\vt y sobre las aves del cielo;",
                "\\q ",
                "\\vt sobre los animales domésticos,",
                "\\q2 ",
                "\\vt sobre los animales salvajes,",
                "\\q ",
                "\\vt y sobre todos los reptiles",
                "\\q2 ",
                "\\vt que se arrastran por el suelo.»",
                "\\q ",
                "\\v 27",
                "\\vt Y Dios creó al ser humano a su imagen;",
                "\\q2 ",
                "\\vt lo creó a imagen de Dios.",
                "\\q ",
                "\\vt **Hombre y mujer los creó,",
                "\\q2 ",
                "\\v 28",
                "\\vt y los bendijo con estas palabras:",
                "\\q ",
                "\\vt «Sean fructíferos y multiplíquense;",
                "\\q2 ",
                "\\vt llenen la tierra y sométanla;",
                "\\q ",
                "\\vt dominen a los peces del mar y a las aves del cielo,",
                "\\q2 ",
                "\\vt y a todos los reptiles que se arrastran por el suelo.»",
                "\\q ",
                "\\v 29",
                "\\vt También les dijo: «Yo les doy de la tierra",
                "\\q2 ",
                "\\vt todas las plantas que producen semilla",
                "\\q ",
                "\\vt y todos los árboles que dan fruto con semilla;",
                "\\q2 ",
                "\\vt todo esto les servirá de alimento.",
                "\\q ",
                "\\v 30",
                "\\vt Y doy la hierba verde como alimento",
                "\\q2 ",
                "\\vt a todas las fieras de la tierra,",
                "\\q ",
                "\\vt a todas las aves del cielo",
                "\\q2 ",
                "\\vt y a todos los seres vivientes",
                "\\q2 ",
                "\\vt que se arrastran por la tierra.»",
                "\\q ",
                "\\vt Y así sucedió.",
                "\\v 31",
                "\\vt Dios miró todo lo que había hecho,",
                "\\q2 ",
                "\\vt y consideró que era muy bueno.",
                "\\q ",
                "\\vt Y vino la noche, y llegó la mañana:",
                "\\q2 ",
                "\\vt ése fue el sexto día.",
                "\\c 2",
                "\\q ",
                "\\v 1",
                "\\vt Así quedaron terminados los cielos y la tierra,",
                "\\q2 ",
                "\\vt y todo lo que hay en ellos.",
                "\\q ",
                "\\v 2",
                "\\vt Al llegar el séptimo día, Dios descansó",
                "\\q2 ",
                "\\vt porque había terminado la obra que había emprendido.",
                "\\q ",
                "\\v 3",
                "\\vt Dios bendijo el séptimo día, y lo **santificó,",
                "\\q2 ",
                "\\vt porque en ese día descansó de toda su obra creadora.",
                "\\q ",
                "\\v 4",
                "\\vt Ésta es la historia de la creación",
                "\\q2 ",
                "\\vt de los cielos y la tierra."
            };
            #endregion

            // Do the test
            MismatchedParaAlignment(36, vsFront);
        }
        #endregion
        #region Method: MismatchedParaAlignment_Few
        [Test] public void MismatchedParaAlignment_Few()
        {
            #region vsFront - Spanish with relatively few paragraphs
            string[] vsFront = new string[]
            {
                "\\_sh v3.0 72 SHW-Scripture",
                "\\_DateStampHasFourDigitYear ",
                "\\rcrd GEN",
                "\\h Gn",
                "\\mt Génesis",
                "\\id GEN",
                "\\rcrd GEN",
                "\\c 1",
                "\\s La creación",

                "\\p",
                "\\v 1",
                "\\vt Dios, en el principio, creó los cielos y la tierra.",
                "\\v 2",
                "\\vt La tierra era un caos total, las tinieblas cubrían el abismo, y el " +
                    "Espíritu de Dios iba y venía sobre la superficie de las aguas.",

                "\\p",
                "\\v 3",
                "\\vt Y dijo Dios: «¡Que exista la luz!» Y la luz llegó a existir.",
                "\\v 4",
                "\\vt Dios consideró que la luz era buena y la separó de las tinieblas.",
                "\\v 5",
                "\\vt A la luz la llamó «día», y a las tinieblas, «noche». Y vino la noche, " +
                    "y llegó la mañana: ése fue el primer día.",

                "\\p",
                "\\v 6",
                "\\vt Y dijo Dios: «¡Que exista el firmamento en medio de las aguas, y " +
                    "que las separe!»",
                "\\v 7",
                "\\vt Y así sucedió: Dios hizo el firmamento y separó las aguas que están " +
                    "abajo, de las aguas que están arriba.",
                "\\v 8",
                "\\vt Al firmamento Dios lo llamó «cielo». Y vino la noche, y llegó la " +
                    "mañana: ése fue el segundo día.",

                "\\p",
                "\\v 9",
                "\\vt Y dijo Dios: «¡Que las aguas debajo del cielo se reúnan en un solo " +
                    "lugar, y que aparezca lo seco!» Y así sucedió.",
                "\\v 10",
                "\\vt A lo seco Dios lo llamó «tierra», y al conjunto de aguas lo llamó «mar». " +
                    "Y Dios consideró que esto era bueno.",
                "\\v 11",
                "\\vt Y dijo Dios: «¡Que haya vegetación sobre la tierra; que ésta produzca " +
                    "hierbas que den semilla, y árboles que den su fruto con semilla, todos " +
                    "según su especie!» Y así sucedió.",
                "\\v 12",
                "\\vt Comenzó a brotar la vegetación: hierbas que dan semilla, y árboles que " +
                    "dan su fruto con semilla, todos según su especie. Y Dios consideró que " +
                    "esto era bueno.",
                "\\v 13",
                "\\vt Y vino la noche, y llegó la mañana: ése fue el tercer día.",

                "\\p",
                "\\v 14",
                "\\vt Y dijo Dios: «¡Que haya luces en el firmamento que separen el día de la " +
                    "noche; que sirvan como señales de las estaciones, de los días y de los años,",
                "\\v 15",
                "\\vt y que brillen en el firmamento para iluminar la tierra!» Y sucedió así.",
                "\\v 16",
                "\\vt Dios hizo los dos grandes astros: el astro mayor para gobernar el día, y " +
                    "el menor para gobernar la noche. También hizo las estrellas.",
                "\\v 17",
                "\\vt Dios colocó en el firmamento los astros para alumbrar la tierra.",
                "\\v 18",
                "\\vt Los hizo para gobernar el día y la noche, y para separar la luz de las " +
                    "tinieblas. Y Dios consideró que esto era bueno.",
                "\\v 19",
                "\\vt Y vino la noche, y llegó la mañana: ése fue el cuarto día.",

                "\\p",
                "\\v 20",
                "\\vt Y dijo Dios: «¡Que rebosen de seres vivientes las aguas, y que vuelen las " +
                    "aves sobre la tierra a lo largo del firmamento!»",
                "\\v 21",
                "\\vt Y creó Dios los grandes animales marinos, y todos los seres vivientes que " +
                    "se mueven y pululan en las aguas y todas las aves, según su especie. Y Dios " +
                    "consideró que esto era bueno,",
                "\\v 22",
                "\\vt y los bendijo con estas palabras: «Sean fructíferos y multiplíquense; llenen " +
                    "las aguas de los mares. ¡Que las aves se multipliquen sobre la tierra!»",
                "\\v 23",
                "\\vt Y vino la noche, y llegó la mañana: ése fue el quinto día.",

                "\\p",
                "\\v 24",
                "\\vt Y dijo Dios: «¡Que produzca la tierra seres vivientes: animales domésticos, " +
                    "animales salvajes, y reptiles, según su especie!» Y sucedió así.",
                "\\v 25",
                "\\vt Dios hizo los animales domésticos, los animales salvajes, y todos los " +
                    "reptiles, según su especie. Y Dios consideró que esto era bueno,",
                "\\v 26",
                "\\vt y dijo: «Hagamos al **ser humano a nuestra imagen y semejanza. Que tenga " +
                    "dominio sobre los peces del mar, y sobre las aves del cielo; sobre los animales " +
                    "domésticos, sobre los animales salvajes, y sobre todos los reptiles que se " +
                    "arrastran por el suelo.»",
                "\\v 27",
                "\\vt Y Dios creó al ser humano a su imagen; lo creó a imagen de Dios. **Hombre " +
                    "y mujer los creó,",
                "\\v 28",
                "\\vt y los bendijo con estas palabras: «Sean fructíferos y multiplíquense; llenen " +
                    "la tierra y sométanla; dominen a los peces del mar y a las aves del cielo, y " +
                    "a todos los reptiles que se arrastran por el suelo.»",
                "\\v 29",
                "\\vt También les dijo: «Yo les doy de la tierra todas las plantas que producen " +
                    "semilla y todos los árboles que dan fruto con semilla; todo esto les servirá " +
                    "de alimento.",
                "\\v 30",
                "\\vt Y doy la hierba verde como alimento a todas las fieras de la tierra, a todas " +
                    "las aves del cielo y a todos los seres vivientes que se arrastran por la " +
                    "tierra.» Y así sucedió.",
                "\\v 31",
                "\\vt Dios miró todo lo que había hecho, y consideró que era muy bueno. Y vino la " +
                    "noche, y llegó la mañana: ése fue el sexto día.",

                "\\c 2",
                "\\p",
                "\\v 1",
                "\\vt Así quedaron terminados los cielos y la tierra, y todo lo que hay en ellos.",
                "\\v 2",
                "\\vt Al llegar el séptimo día, Dios descansó porque había terminado la obra que " +
                    "había emprendido.",
                "\\v 3",
                "\\vt Dios bendijo el séptimo día, y lo **santificó, porque en ese día descansó " +
                    "de toda su obra creadora.",

                "\\p",
                "\\v 4",
                "\\vt Ésta es la historia de la creación de los cielos y la tierra."
            };
            #endregion

            // Do the test
            MismatchedParaAlignment(10, vsFront);
        }
        #endregion


    }
}
