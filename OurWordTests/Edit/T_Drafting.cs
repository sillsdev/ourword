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
		    "\\h Sut�a",
		    "\\mt Sut�arica",
		    "\\id GEN - Huichol (hch)",
            "\\rcrd GEN 01.01-02.04",
            "\\c 1",
            "\\s Caca�yari Naime Tiniutavevieni",
            "\\p ",
            "\\q ",
            "\\v 1",
            "\\vt Sut�apai Caca�yari muyuavi niutavevieni,",
            "\\q2 ",
            "\\vt cuieta niutavevieni.",
            "\\q ",
            "\\v 2",
            "\\vt 'Ana puy�vicai xeic�a nait� cuiepa, cuie havaic� p�cahecumacai, naisarie puy�vicai hapa.",
            "\\q2 ",
            "\\vt Caca�yari 'iyarieya ha heima",
            "\\q2 ",
            "\\vt p�yeyuanenecai.",
            "\\q ",
            "\\v 3",
            "\\vt Hic� Caca�yari m�pa� niutayuni: �Hec�ariya que'uneni.�",
            "\\q2 ",
            "\\vt Hic� hec�ariya niuneni.",
            "\\q ",
            "\\v 4",
            "\\vt Nixeiya Caca�yari hec�ariya, �'Aix�a p�'ane,� niutay�ni,",
            "\\q2 ",
            "\\vt 'Ana nipata y�riya Caca�yari.",
            "\\q ",
            "\\v 5",
            "\\vt �Hec�ariya tucari p�titeva,� niutay�ni,",
            "\\q2 ",
            "\\vt �Y�riya t�cari p�titeva,� niutay�ni.",
            "\\q ",
            "\\vt 'Ana caninuani y�riya, ximerita canayani,",
            "\\q2 ",
            "\\vt mex�acame tucari nayani, ximeri taicai.",
            "\\q ",
            "\\v 6",
            "\\vt Caca�yari m�pa� niutay�ni: �Muyuavi que'uneni,",
            "\\q2 ",
            "\\vt que'ipata ha taheima mieme cuiepa mieme.�",
            "\\q ",
            "\\v 7",
            "\\vt M�pa� tiniuyurieni Caca�yari. Nitavevieni sut�meyari,",
            "\\q2 ",
            "\\vt nipata ha, haivit�rit�a m�tamacac�,",
            "\\q ",
            "\\vt hix�ata 'at� het�ana caniyuhayeva ha,",
            "\\q2 ",
            "\\vt hapa caniutaveviyani taheima muyema.",
            "\\q ",
            "\\v 8",
            "\\vt Sut�meyari, Caca�yari �muyuavi� tiniuter�va.",
            "\\q2 ",
            "\\vt Meric�s� taicai nayani, ximerita nayani,",
            "\\q2 ",
            "\\vt Hutarieca mieme tucari canayani.",
            "\\q ",
            "\\v 9",
            "\\vt Hic� Caca�yari m�pa� niutay�ni: �Ha muyuavit�a m�tama",
            "\\q2 ",
            "\\vt que'uyucuxe�rieni 'axeic�a,",
            "\\q2 ",
            "\\vt 'uvaquit� que'ayani.�",
            "\\q ",
            "\\vt Quemutay� m�pa� p�ti�y�.",
            "\\v 10",
            "\\vt Muvaqui Caca�yari tiniter�va �cuie,�",
            "\\q2 ",
            "\\vt ha muyucuxe�ri tiniter�va �haramara.�",
            "\\q ",
            "\\vt 'Ixeiyat� Caca�yari �'Aix�a p�'ane,� niutay�ni.",
            "\\q ",
            "\\v 11",
            "\\vt 'Arique caca�yari m�pa� niutay�ni: �Cuiepa que'utineni tupiriya,",
            "\\q2 ",
            "\\vt yutacari yuhasiyari hexeiyat�,",
            "\\q ",
            "\\vt c�yexi meta yutacari hexeiyat� yuhasiyari.",
            "\\q2 ",
            "\\vt nait� xexuit� quem�tiupit�arie.�",
            "\\q ",
            "\\vt M�pa� tiniuy�ni.",
            "\\v 12",
            "\\vt 'Ana catiniutineni nait�,",
            "\\q2 ",
            "\\vt tupiriyate, yutacari yuhasiyari hexeiyat�,",
            "\\q ",
            "\\vt c�yexi 'ite�ri yutacari yuhasiyari hexeiyat�,",
            "\\q2 ",
            "\\vt nait� quem�tiupit�arie.",
            "\\q ",
            "\\vt Meric�s� Caca�yari �'Aix�a p�'ane,� niutay�ni.",
            "\\q2 ",
            "\\v 13",
            "\\vt Hic� taicai nayani, hic�tari ximeri nayani,",
            "\\q2 ",
            "\\vt hairieca tucari nayani.",
            "\\q ",
            "\\v 14",
            "\\vt M�pa�ta niutay�ni Caca�yari: �Meque'uneni muyuavisie hec�arivivamete,",
            "\\q2 ",
            "\\vt meque'ipata tucari t�cari,",
            "\\q2 ",
            "\\vt m�c� c�xemete 'in�ari meque'ac�ne vitari taxari 'in�n�atamete,",
            "\\q2 ",
            "\\vt t�carisie mieme viyarisie mieme.",
            "\\q ",
            "\\v 15",
            "\\vt Hec�ariya quehec�arivieca muyuavisie,",
            "\\q2 ",
            "\\vt naime quehec�ariyani cuiepa.�",
            "\\q ",
            "\\vt M�pa� tiniuy�ni.",
            "\\v 16",
            "\\vt Caca�yari canitivevieni hutame mamarivaveme c�xeme,",
            "\\q2 ",
            "\\vt tau niutavevieni mariveme m�tita'aitac� tucaric�,",
            "\\q ",
            "\\vt mesata tiutavevi m�tita'aitac� t�cac�ta,",
            "\\q2 ",
            "\\vt xuravesixi meta t�caric� miemete.",
            "\\q ",
            "\\v 17",
            "\\vt Caca�yari c�xeme niuyevieni muyuavisie",
            "\\q2 ",
            "\\vt xuravesixi m�hec�ariviecac� cuiepa.",
            "\\q ",
            "\\v 18",
            "\\vt P�varutivevi mem�teta'aitac� tucaric� t�caric�,",
            "\\q2 ",
            "\\vt hec�ariya mipatac� y�riya.",
            "\\q ",
            "\\vt Hic� Caca�yari �'Aix�a p�'ane,� niutay�ni.",
            "\\q2 ",
            "\\v 19",
            "\\vt Hic� pucuy�rix�, hic�tari caniucuhec�are,",
            "\\q2 ",
            "\\vt ximeri canayani naurieca tucarisie.",
            "\\q ",
            "\\v 20",
            "\\vt M�pa� niutay�ni Caca�yari: �Mequeh�neni ques�te hapa,",
            "\\q2 ",
            "\\vt mequehe�v�t�cani viquixi cuiepa meutitevasiepai,",
            "\\q2 ",
            "\\vt muyuavisiepai mequeheuv�t�cani.�",
            "\\q ",
            "\\v 21",
            "\\vt Meric�s� catinivarutivevieni yunaime 'amem�temamarivave haramarasie memu'uva,",
            "\\q2 ",
            "\\vt yunaime mem�te'ayeneniere 'esimepepet� mem�cuyuat�ca.",
            "\\q ",
            "\\vt yunaime viquixi,",
            "\\q2 ",
            "\\vt m�pa� quem�tivarupit�a muyuavisie miemete.",
            "\\q ",
            "\\vt Meric�s� vaxeiyat� Caca�yari �'Aix�a p�'ane� niutay�ni.",
            "\\q2 ",
            "\\v 22",
            "\\vt 'Aix�a nivaruyurieni m�pa� 'utait�,",
            "\\q ",
            "\\vt �Xequeneyuxi�t�aca xequeneyum�iriyani,",
            "\\q2 ",
            "\\vt xequeneh�neni hapa haramarasie.",
            "\\q2 ",
            "\\vt Viquixi meta xequeney�m�iriyani cuiepa.�",
            "\\q ",
            "\\v 23",
            "\\vt Hic� pucuy�rix�, hic�tari caniucuhec�are,",
            "\\q2 ",
            "\\vt ximeri canayani 'aux�virieca tucarisie.",
            "\\q ",
            "\\v 24",
            "\\vt M�pa� niutay�ni Caca�yari: �Xequeney�m�iriyani cuiepa yunait� xemayeyuyurini,",
            "\\q2 ",
            "\\vt yunait� mem�teyutevasin�a, yeutari meta,",
            "\\q2 ",
            "\\vt yuhucama memucu'uva cuiepa, yunaime quem�tivarupit�a.�",
            "\\q ",
            "\\vt M�pa� p�tiuy�.",
            "\\v 25",
            "\\vt Caca�yari yunaime catinivarunet�ani mem�teyutevasin�a,",
            "\\q2 ",
            "\\vt yeutari, yunaime mem�ca'uc�nivave yuhucama memucu'uva,",
            "\\q2 ",
            "\\vt yunaime que m�tivarupit�a.",
            "\\q ",
            "\\vt Caca�yari m�pa� niutay�ni, �'Aix�a p�'ane�.",
            "\\q2 ",
            "\\v 26",
            "\\vt M�pa�ta niutay�ni: �Tevi tep�tavevieni",
            "\\q2 ",
            "\\vt tahepa� 'aneme tahepa� tiy�c�h�aveme.",
            "\\q ",
            "\\vt P�tivata'ait�ani ques�te haramarasie miemete,",
            "\\q2 ",
            "\\vt viquixi meta muyuavisie miemete,",
            "\\q ",
            "\\vt yunaime mem�teyutevasin�a,",
            "\\q2 ",
            "\\vt yeutari,",
            "\\q ",
            "\\vt yunaime mem�ca'uc�nivave",
            "\\q2 ",
            "\\vt yuhucama memucu'uva cuiepa.�",
            "\\q ",
            "\\v 27",
            "\\vt Caca�yari tevi niutavevieni yuhepa� 'aneme,",
            "\\q2 ",
            "\\vt pitavevi 'in�arieya mayanic� yuhepa� raca'eriecame.",
            "\\q ",
            "\\vt 'Uquit�me putavevi, 'ucata putavevi.",
            "\\q2 ",
            "\\v 28",
            "\\vt 'Aix�a nivaruyurieni m�pa� tivarutah�aveca,",
            "\\q ",
            "\\vt �Xequenexi�ca xequeney�m�iriyani,",
            "\\q2 ",
            "\\vt cuiepa xequeneh�neni xequetene'aitani.",
            "\\q ",
            "\\vt Xep�tevata'ait�ani ques�te haramarasie miemete, viquixi yunaime muyuavisie miemete,",
            "\\q2 ",
            "\\vt yunaime memuc�nivave yuhucama memucu'uva cuiepa.�",
            "\\q ",
            "\\v 29",
            "\\vt M�pa�ta tinivarutah�ave: �Ne nep�xemini cuiepa mieme 'ite�ri,",
            "\\q2 ",
            "\\vt naime 'ite�ri yuhasiyari hexeiyame,",
            "\\q ",
            "\\vt naime c�yexi yutacari yuhasiyari hexeiyame.",
            "\\q2 ",
            "\\vt M�c� nait� xe'icuai p�rayani.",
            "\\q ",
            "\\v 30",
            "\\vt Tupiriyata nep�vamini m�sisi�ravi, 'icuai payani,",
            "\\q2 ",
            "\\vt yunait� yeutari cuiepa mem�texuave,",
            "\\q2 ",
            "\\vt yunait� viquixi muyuavisie miemete",
            "\\q2 ",
            "\\vt yunait� memayeyuyurini",
            "\\q2 ",
            "\\vt memeumaquet�ca cuiepa.�",
            "\\q ",
            "\\vt Quemutay� m�pa� p�ti�y�.",
            "\\v 31",
            "\\vt Caca�yari naime tiniuxeiya tita m�tiutivevi.",
            "\\q2 ",
            "\\vt �'Aix�a p�'ane,� niutayuni.",
            "\\q ",
            "\\vt Hic� niuc�yure, hic�tari niucahec�are,",
            "\\q2 ",
            "\\vt 'ataxevirieca tucari nayani.",
            "\\c 2",
            "\\q2 ",
            "\\v 1",
            "\\vt M�pa� 'anet� niuyuhayeva n�riet� muyuavi cuie meta nait� tita hesiena m�tixuave.",
            "\\q ",
            "\\v 2",
            "\\vt 'Atahutarieca tucarisie, Caca�yari niuca'uxipieni,",
            "\\q2 ",
            "\\vt naime m�tiun�c� yu'uximayasica quem�tiuy�h�avix�.",
            "\\q2 ",
            "\\v 3",
            "\\vt Caca�yari 'aix�a catiniuyurieni 'atahutarieca tucari hepa�sita. Nipata yuhesie mieme",
            "\\q2 ",
            "\\vt 'ana m�ca'uxipic�, naime yu'uximayasica 'un�ca que m�titavevi muyuavi cuie meta.",
            "\\q2 ",
            "\\v 4",
            "\\vt Quepaucua Caca�yarit�t� Yave mitavevi muyuavi cuie meta,",
            "\\q2 ",
            "\\vt 'ic� m�c� p�'�xasiyari."
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
                "\\mt G�nesis",
                "\\id GEN",
                "\\rcrd GEN",
                "\\c 1",
                "\\s La creaci�n",
                "\\p ",
                "\\q ",
                "\\v 1",
                "\\vt Dios, en el principio,",
                "\\q2 ",
                "\\vt cre� los cielos y la tierra.",
                "\\q ",
                "\\v 2",
                "\\vt La tierra era un caos total,",
                "\\q2 ",
                "\\vt las tinieblas cubr�an el abismo,",
                "\\q ",
                "\\vt y el Esp�ritu de Dios iba y ven�a",
                "\\q2 ",
                "\\vt sobre la superficie de las aguas.",
                "\\q ",
                "\\v 3",
                "\\vt Y dijo Dios: ��Que exista la luz!�",
                "\\q2 ",
                "\\vt Y la luz lleg� a existir.",
                "\\q ",
                "\\v 4",
                "\\vt Dios consider� que la luz era buena",
                "\\q2 ",
                "\\vt y la separ� de las tinieblas.",
                "\\q ",
                "\\v 5",
                "\\vt A la luz la llam� �d�a�,",
                "\\q2 ",
                "\\vt y a las tinieblas, �noche�.",
                "\\q ",
                "\\vt Y vino la noche, y lleg� la ma�ana:",
                "\\q2 ",
                "\\vt �se fue el primer d�a.",
                "\\q ",
                "\\v 6",
                "\\vt Y dijo Dios: ��Que exista el firmamento",
                "\\q2 ",
                "\\vt en medio de las aguas, y que las separe!�",
                "\\q ",
                "\\v 7",
                "\\vt Y as� sucedi�: Dios hizo el firmamento",
                "\\q2 ",
                "\\vt y separ� las aguas que est�n abajo,",
                "\\q2 ",
                "\\vt de las aguas que est�n arriba.",
                "\\q ",
                "\\v 8",
                "\\vt Al firmamento Dios lo llam� �cielo�.",
                "\\q2 ",
                "\\vt Y vino la noche, y lleg� la ma�ana:",
                "\\q2 ",
                "\\vt �se fue el segundo d�a.",
                "\\q ",
                "\\v 9",
                "\\vt Y dijo Dios: ��Que las aguas debajo del cielo",
                "\\q2 ",
                "\\vt se re�nan en un solo lugar,",
                "\\q2 ",
                "\\vt y que aparezca lo seco!�",
                "\\q ",
                "\\vt Y as� sucedi�.",
                "\\v 10",
                "\\vt A lo seco Dios lo llam� �tierra�,",
                "\\q2 ",
                "\\vt y al conjunto de aguas lo llam� �mar�.",
                "\\q ",
                "\\vt Y Dios consider� que esto era bueno.",
                "\\q ",
                "\\v 11",
                "\\vt Y dijo Dios: ��Que haya vegetaci�n sobre la tierra;",
                "\\q2 ",
                "\\vt que �sta produzca hierbas que den semilla,",
                "\\q ",
                "\\vt y �rboles que den su fruto con semilla,",
                "\\q2 ",
                "\\vt todos seg�n su especie!�",
                "\\q ",
                "\\vt Y as� sucedi�.",
                "\\v 12",
                "\\vt Comenz� a brotar la vegetaci�n:",
                "\\q2 ",
                "\\vt hierbas que dan semilla,",
                "\\q ",
                "\\vt y �rboles que dan su fruto con semilla,",
                "\\q2 ",
                "\\vt todos seg�n su especie.",
                "\\q ",
                "\\vt Y Dios consider� que esto era bueno.",
                "\\q2 ",
                "\\v 13",
                "\\vt Y vino la noche, y lleg� la ma�ana:",
                "\\q2 ",
                "\\vt �se fue el tercer d�a.",
                "\\q ",
                "\\v 14",
                "\\vt Y dijo Dios: ��Que haya luces en el firmamento",
                "\\q2 ",
                "\\vt que separen el d�a de la noche;",
                "\\q ",
                "\\vt que sirvan como se�ales de las estaciones,",
                "\\q2 ",
                "\\vt de los d�as y de los a�os,",
                "\\q ",
                "\\v 15",
                "\\vt y que brillen en el firmamento",
                "\\q2 ",
                "\\vt para iluminar la tierra!�",
                "\\q ",
                "\\vt Y sucedi� as�.",
                "\\v 16",
                "\\vt Dios hizo los dos grandes astros:",
                "\\q2 ",
                "\\vt el astro mayor para gobernar el d�a,",
                "\\q ",
                "\\vt y el menor para gobernar la noche.",
                "\\q2 ",
                "\\vt Tambi�n hizo las estrellas.",
                "\\q ",
                "\\v 17",
                "\\vt Dios coloc� en el firmamento",
                "\\q2 ",
                "\\vt los astros para alumbrar la tierra.",
                "\\q ",
                "\\v 18",
                "\\vt Los hizo para gobernar el d�a y la noche,",
                "\\q2 ",
                "\\vt y para separar la luz de las tinieblas.",
                "\\q ",
                "\\vt Y Dios consider� que esto era bueno.",
                "\\q2 ",
                "\\v 19",
                "\\vt Y vino la noche, y lleg� la ma�ana:",
                "\\q2 ",
                "\\vt �se fue el cuarto d�a.",
                "\\q ",
                "\\v 20",
                "\\vt Y dijo Dios: ��Que rebosen de seres vivientes las aguas,",
                "\\q2 ",
                "\\vt y que vuelen las aves sobre la tierra",
                "\\q2 ",
                "\\vt a lo largo del firmamento!�",
                "\\q ",
                "\\v 21",
                "\\vt Y cre� Dios los grandes animales marinos,",
                "\\q2 ",
                "\\vt y todos los seres vivientes",
                "\\q2 ",
                "\\vt que se mueven y pululan en las aguas",
                "\\q ",
                "\\vt y todas las aves,",
                "\\q2 ",
                "\\vt seg�n su especie.",
                "\\q ",
                "\\vt Y Dios consider� que esto era bueno,",
                "\\q2 ",
                "\\v 22",
                "\\vt y los bendijo con estas palabras:",
                "\\q ",
                "\\vt �Sean fruct�feros y multipl�quense;",
                "\\q2 ",
                "\\vt llenen las aguas de los mares.",
                "\\q2 ",
                "\\vt �Que las aves se multipliquen sobre la tierra!�",
                "\\q ",
                "\\v 23",
                "\\vt Y vino la noche, y lleg� la ma�ana:",
                "\\q2 ",
                "\\vt �se fue el quinto d�a.",
                "\\q ",
                "\\v 24",
                "\\vt Y dijo Dios: ��Que produzca la tierra seres vivientes:",
                "\\q2 ",
                "\\vt animales dom�sticos, animales salvajes,",
                "\\q2 ",
                "\\vt y reptiles, seg�n su especie!�",
                "\\q ",
                "\\vt Y sucedi� as�.",
                "\\v 25",
                "\\vt Dios hizo los animales dom�sticos,",
                "\\q2 ",
                "\\vt los animales salvajes, y todos los reptiles,",
                "\\q2 ",
                "\\vt seg�n su especie.",
                "\\q ",
                "\\vt Y Dios consider� que esto era bueno,",
                "\\q2 ",
                "\\v 26",
                "\\vt y dijo: �Hagamos al **ser humano",
                "\\q2 ",
                "\\vt a nuestra imagen y semejanza.",
                "\\q ",
                "\\vt Que tenga dominio sobre los peces del mar,",
                "\\q2 ",
                "\\vt y sobre las aves del cielo;",
                "\\q ",
                "\\vt sobre los animales dom�sticos,",
                "\\q2 ",
                "\\vt sobre los animales salvajes,",
                "\\q ",
                "\\vt y sobre todos los reptiles",
                "\\q2 ",
                "\\vt que se arrastran por el suelo.�",
                "\\q ",
                "\\v 27",
                "\\vt Y Dios cre� al ser humano a su imagen;",
                "\\q2 ",
                "\\vt lo cre� a imagen de Dios.",
                "\\q ",
                "\\vt **Hombre y mujer los cre�,",
                "\\q2 ",
                "\\v 28",
                "\\vt y los bendijo con estas palabras:",
                "\\q ",
                "\\vt �Sean fruct�feros y multipl�quense;",
                "\\q2 ",
                "\\vt llenen la tierra y som�tanla;",
                "\\q ",
                "\\vt dominen a los peces del mar y a las aves del cielo,",
                "\\q2 ",
                "\\vt y a todos los reptiles que se arrastran por el suelo.�",
                "\\q ",
                "\\v 29",
                "\\vt Tambi�n les dijo: �Yo les doy de la tierra",
                "\\q2 ",
                "\\vt todas las plantas que producen semilla",
                "\\q ",
                "\\vt y todos los �rboles que dan fruto con semilla;",
                "\\q2 ",
                "\\vt todo esto les servir� de alimento.",
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
                "\\vt que se arrastran por la tierra.�",
                "\\q ",
                "\\vt Y as� sucedi�.",
                "\\v 31",
                "\\vt Dios mir� todo lo que hab�a hecho,",
                "\\q2 ",
                "\\vt y consider� que era muy bueno.",
                "\\q ",
                "\\vt Y vino la noche, y lleg� la ma�ana:",
                "\\q2 ",
                "\\vt �se fue el sexto d�a.",
                "\\c 2",
                "\\q ",
                "\\v 1",
                "\\vt As� quedaron terminados los cielos y la tierra,",
                "\\q2 ",
                "\\vt y todo lo que hay en ellos.",
                "\\q ",
                "\\v 2",
                "\\vt Al llegar el s�ptimo d�a, Dios descans�",
                "\\q2 ",
                "\\vt porque hab�a terminado la obra que hab�a emprendido.",
                "\\q ",
                "\\v 3",
                "\\vt Dios bendijo el s�ptimo d�a, y lo **santific�,",
                "\\q2 ",
                "\\vt porque en ese d�a descans� de toda su obra creadora.",
                "\\q ",
                "\\v 4",
                "\\vt �sta es la historia de la creaci�n",
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
                "\\mt G�nesis",
                "\\id GEN",
                "\\rcrd GEN",
                "\\c 1",
                "\\s La creaci�n",

                "\\p",
                "\\v 1",
                "\\vt Dios, en el principio, cre� los cielos y la tierra.",
                "\\v 2",
                "\\vt La tierra era un caos total, las tinieblas cubr�an el abismo, y el " +
                    "Esp�ritu de Dios iba y ven�a sobre la superficie de las aguas.",

                "\\p",
                "\\v 3",
                "\\vt Y dijo Dios: ��Que exista la luz!� Y la luz lleg� a existir.",
                "\\v 4",
                "\\vt Dios consider� que la luz era buena y la separ� de las tinieblas.",
                "\\v 5",
                "\\vt A la luz la llam� �d�a�, y a las tinieblas, �noche�. Y vino la noche, " +
                    "y lleg� la ma�ana: �se fue el primer d�a.",

                "\\p",
                "\\v 6",
                "\\vt Y dijo Dios: ��Que exista el firmamento en medio de las aguas, y " +
                    "que las separe!�",
                "\\v 7",
                "\\vt Y as� sucedi�: Dios hizo el firmamento y separ� las aguas que est�n " +
                    "abajo, de las aguas que est�n arriba.",
                "\\v 8",
                "\\vt Al firmamento Dios lo llam� �cielo�. Y vino la noche, y lleg� la " +
                    "ma�ana: �se fue el segundo d�a.",

                "\\p",
                "\\v 9",
                "\\vt Y dijo Dios: ��Que las aguas debajo del cielo se re�nan en un solo " +
                    "lugar, y que aparezca lo seco!� Y as� sucedi�.",
                "\\v 10",
                "\\vt A lo seco Dios lo llam� �tierra�, y al conjunto de aguas lo llam� �mar�. " +
                    "Y Dios consider� que esto era bueno.",
                "\\v 11",
                "\\vt Y dijo Dios: ��Que haya vegetaci�n sobre la tierra; que �sta produzca " +
                    "hierbas que den semilla, y �rboles que den su fruto con semilla, todos " +
                    "seg�n su especie!� Y as� sucedi�.",
                "\\v 12",
                "\\vt Comenz� a brotar la vegetaci�n: hierbas que dan semilla, y �rboles que " +
                    "dan su fruto con semilla, todos seg�n su especie. Y Dios consider� que " +
                    "esto era bueno.",
                "\\v 13",
                "\\vt Y vino la noche, y lleg� la ma�ana: �se fue el tercer d�a.",

                "\\p",
                "\\v 14",
                "\\vt Y dijo Dios: ��Que haya luces en el firmamento que separen el d�a de la " +
                    "noche; que sirvan como se�ales de las estaciones, de los d�as y de los a�os,",
                "\\v 15",
                "\\vt y que brillen en el firmamento para iluminar la tierra!� Y sucedi� as�.",
                "\\v 16",
                "\\vt Dios hizo los dos grandes astros: el astro mayor para gobernar el d�a, y " +
                    "el menor para gobernar la noche. Tambi�n hizo las estrellas.",
                "\\v 17",
                "\\vt Dios coloc� en el firmamento los astros para alumbrar la tierra.",
                "\\v 18",
                "\\vt Los hizo para gobernar el d�a y la noche, y para separar la luz de las " +
                    "tinieblas. Y Dios consider� que esto era bueno.",
                "\\v 19",
                "\\vt Y vino la noche, y lleg� la ma�ana: �se fue el cuarto d�a.",

                "\\p",
                "\\v 20",
                "\\vt Y dijo Dios: ��Que rebosen de seres vivientes las aguas, y que vuelen las " +
                    "aves sobre la tierra a lo largo del firmamento!�",
                "\\v 21",
                "\\vt Y cre� Dios los grandes animales marinos, y todos los seres vivientes que " +
                    "se mueven y pululan en las aguas y todas las aves, seg�n su especie. Y Dios " +
                    "consider� que esto era bueno,",
                "\\v 22",
                "\\vt y los bendijo con estas palabras: �Sean fruct�feros y multipl�quense; llenen " +
                    "las aguas de los mares. �Que las aves se multipliquen sobre la tierra!�",
                "\\v 23",
                "\\vt Y vino la noche, y lleg� la ma�ana: �se fue el quinto d�a.",

                "\\p",
                "\\v 24",
                "\\vt Y dijo Dios: ��Que produzca la tierra seres vivientes: animales dom�sticos, " +
                    "animales salvajes, y reptiles, seg�n su especie!� Y sucedi� as�.",
                "\\v 25",
                "\\vt Dios hizo los animales dom�sticos, los animales salvajes, y todos los " +
                    "reptiles, seg�n su especie. Y Dios consider� que esto era bueno,",
                "\\v 26",
                "\\vt y dijo: �Hagamos al **ser humano a nuestra imagen y semejanza. Que tenga " +
                    "dominio sobre los peces del mar, y sobre las aves del cielo; sobre los animales " +
                    "dom�sticos, sobre los animales salvajes, y sobre todos los reptiles que se " +
                    "arrastran por el suelo.�",
                "\\v 27",
                "\\vt Y Dios cre� al ser humano a su imagen; lo cre� a imagen de Dios. **Hombre " +
                    "y mujer los cre�,",
                "\\v 28",
                "\\vt y los bendijo con estas palabras: �Sean fruct�feros y multipl�quense; llenen " +
                    "la tierra y som�tanla; dominen a los peces del mar y a las aves del cielo, y " +
                    "a todos los reptiles que se arrastran por el suelo.�",
                "\\v 29",
                "\\vt Tambi�n les dijo: �Yo les doy de la tierra todas las plantas que producen " +
                    "semilla y todos los �rboles que dan fruto con semilla; todo esto les servir� " +
                    "de alimento.",
                "\\v 30",
                "\\vt Y doy la hierba verde como alimento a todas las fieras de la tierra, a todas " +
                    "las aves del cielo y a todos los seres vivientes que se arrastran por la " +
                    "tierra.� Y as� sucedi�.",
                "\\v 31",
                "\\vt Dios mir� todo lo que hab�a hecho, y consider� que era muy bueno. Y vino la " +
                    "noche, y lleg� la ma�ana: �se fue el sexto d�a.",

                "\\c 2",
                "\\p",
                "\\v 1",
                "\\vt As� quedaron terminados los cielos y la tierra, y todo lo que hay en ellos.",
                "\\v 2",
                "\\vt Al llegar el s�ptimo d�a, Dios descans� porque hab�a terminado la obra que " +
                    "hab�a emprendido.",
                "\\v 3",
                "\\vt Dios bendijo el s�ptimo d�a, y lo **santific�, porque en ese d�a descans� " +
                    "de toda su obra creadora.",

                "\\p",
                "\\v 4",
                "\\vt �sta es la historia de la creaci�n de los cielos y la tierra."
            };
            #endregion

            // Do the test
            MismatchedParaAlignment(10, vsFront);
        }
        #endregion


    }
}
