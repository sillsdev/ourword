#region ***** T_DSection.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DSection.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DSection class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using NUnit.Framework;

using JWTools;
using OurWordData;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWordTests.DataModel
{
    #region CLASS: SectionTestData - import/export data
    public class SectionTestData
    {
        // Data in Toolbox SFM ---------------------------------------------------------------

        // 1. Baikeno Mark 1:1
        #region TestData #1 - BaikenoMark0101_ImportVariant
        static public string[] BaikenoMark0101_ImportVariant = new string[] 
		{
			"\\rcrd MRK 01.01-01.03",
			"\\c 1",
			"\\p",
			"\\v 1",
			"\\vt Ije lais alekot. Ije Uis-neno In Anmone|fn in na' monin. In " +
			"kana, Jesus Kristus, es Uis-neno nleek nani na'ko un-unu'. " +
			"In lasi nane, nahuun nak on ii:",
			"\\btvt This is a good story/matter. This is *God's Son's|fn life. " +
			"His name is Jesus Kristus, who God designated beforehand from " +
			"long ago. His story/issue begins like this:",
			"\\ft 1:1: Lasi <Uis-neno In Anmone> ka nmui' fa matu'i mane'o bian.",
			"\\btft 1:1: The words <God's Son> is not there in some of the old writings.",
			"\\nt tonas = cerita, berita, riwayat ; una = pohon, pertama, " + 
			"sumber, awal base ; nleek = tunju ; Abalbalat = yg kekal ; " + 
			"mane'o = betul, asli ; amna' = tua ; in una = its beginning " +
			"## na' mo'on = life story, riwayat (not known by all) ; " +
			"ma'mo'en = perbuatan",
			"\\s Nai' Joao Aslain Atoni, naleko' lalan neu Usif Jesus",
			"\\bts Sir Joao the *Baptiser of People, fix/prepares the way/path for " +
			"the *Lord Jesus",
			"\\r (Mateus 3:1-12; Lukas 3:1-18; Joao 1:19-28)",
			"\\p",
			"\\v 2",
			"\\vt Jesus fe' ka nanaob In mepu, mes Uis-neno nsonu' nahuun In " +
			"atoni mese', in kanan nai' Joao. Nai' Joao musti nao naleko' " +
			"lalan neu Jesus In amneman. Fun natuin na'ko un-unu', " +
			"Uis-neno anpaek nalail In mafeef' es. Mafefa' nane, in kanan " +
			"Na'i Yesaya. In ntui nani, nak on ii:",
			"\\btvt Jesus had not yet begun His work, but God sent beforehand one " +
			"of His men, whose name as sir Joao. Sir Joao must fix/prepare the " +
			"path/way for Jesus' coming. Because from long ago, God had used one " +
			"of His mouth (=spokesperson). That spokesperson was named " +
			"Grandfather/ancestor Yesaya. He had written like this:",
			"\\nt nasoitan = buka ; nseef = buka, (tali) ; naloitan = perbaiki ; " +
			"nani = memang, fore-",
			"\\q",
			"\\vt <<Mneen nai, he! Au 'leul Au 'haef ma 'nimaf, henati nao naleko' " +
			"lalan neu Ko",
			"\\btvt <<Listen up, he! I send My foot and hand (=trusty servant) to go " +
			"fix/prepare the way/path for You.",
			"\\cf 1:2: Maleakhi 3:1",
			"\\q",
			"\\v 3",
			"\\vt Le atoni nane lof in anao mbi bael sona' es, he in nkoa', mnak:",
			"\\btvt That man will go to an uninhabited place, to shout.words, saying:"
		};
        #endregion
        #region TestData #1 - BaikenoMark0101_Cannonical
        static public string[] BaikenoMark0101_Cannonical = new string[] 
		{
		    "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
            "\\rcrd MRK",
            "\\mt",
            "\\id MRK",
			"\\rcrd MRK 01.01-01.03",
			"\\p",
			"\\c 1",
			"\\v 1",
			"\\vt Ije lais alekot. Ije Uis-neno In Anmone|fn",
			"\\btvt This is a good story/matter. This is *God's Son's|fn",
			"\\ft 1:1: Lasi <Uis-neno In Anmone> ka nmui' fa matu'i mane'o bian.",
			"\\btft 1:1: The words <God's Son> is not there in some of the old writings.",
            "\\vt in na' monin. In kana, Jesus Kristus, es Uis-neno nleek nani na'ko " +
			    "un-unu'. In lasi nane, nahuun nak on ii:",
            "\\btvt life. His name is Jesus Kristus, who God designated beforehand from " +
			    "long ago. His story/issue begins like this:",
			"\\nt tonas = cerita, berita, riwayat ; una = pohon, pertama, " + 
			    "sumber, awal base ; nleek = tunju ; Abalbalat = yg kekal ; " + 
			    "mane'o = betul, asli ; amna' = tua ; in una = its beginning " +
			    "## na' mo'on = life story, riwayat (not known by all) ; " +
			    "ma'mo'en = perbuatan",
			"\\s Nai' Joao Aslain Atoni, naleko' lalan neu Usif Jesus",
			"\\bts Sir Joao the *Baptiser of People, fix/prepares the way/path for " +
			    "the *Lord Jesus",
			"\\r (Mateus 3:1-12; Lukas 3:1-18; Joao 1:19-28)",
			"\\p",
			"\\v 2",
			"\\vt Jesus fe' ka nanaob In mepu, mes Uis-neno nsonu' nahuun In " +
			    "atoni mese', in kanan nai' Joao. Nai' Joao musti nao naleko' " +
			    "lalan neu Jesus In amneman. Fun natuin na'ko un-unu', " +
			    "Uis-neno anpaek nalail In mafeef' es. Mafefa' nane, in kanan " +
			    "Na'i Yesaya. In ntui nani, nak on ii:",
			"\\btvt Jesus had not yet begun His work, but God sent beforehand one " +
			    "of His men, whose name as sir Joao. Sir Joao must fix/prepare the " +
			    "path/way for Jesus' coming. Because from long ago, God had used one " +
			    "of His mouth (=spokesperson). That spokesperson was named " +
			    "Grandfather/ancestor Yesaya. He had written like this:",
			"\\nt nasoitan = buka ; nseef = buka, (tali) ; naloitan = perbaiki ; " +
			    "nani = memang, fore-",
			"\\q",
			"\\vt <<Mneen nai, he! Au 'leul Au 'haef ma 'nimaf, henati nao naleko' " +
			    "lalan neu Ko",
			"\\btvt <<Listen up, he! I send My foot and hand (=trusty servant) to go " +
			    "fix/prepare the way/path for You.",
			"\\cf 1:2: Maleakhi 3:1",
			"\\q",
			"\\v 3",
			"\\vt Le atoni nane lof in anao mbi bael sona' es, he in nkoa', mnak:",
			"\\btvt That man will go to an uninhabited place, to shout.words, saying:"
		};
        #endregion
        #region TestData #1 - BaikenoMark0101_oxes
        static public string[] BaikenoMark0101_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<c n=\"1\"/>",
                    "<v n=\"1\"/>",
                    "Ije lais alekot. Ije Uis-neno In Anmone",
                    "<bt>This is a good story/matter. This is *God's Son's</bt>",
                    "<note reference=\"1:1\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "Lasi &lt;Uis-neno In Anmone&gt; ka nmui' fa matu'i mane'o bian.",
                        "<bt>The words &lt;God's Son&gt; is not there in some of the old writings.</bt>",
                    "</note>",
                    "in na' monin. In kana, Jesus Kristus, es Uis-neno nleek nani na'ko un-unu'. In lasi nane, nahuun nak on ii:",
                    "<bt>life. His name is Jesus Kristus, who God designated beforehand from long ago. His story/issue begins like this:</bt>",
                    "<Annotation class=\"General\" selectedText=\"tonas = cerita, berita, riwayat ;\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-16 13:04:23Z\">tonas = cerita, berita, riwayat ; una = pohon, pertama, sumber, awal base ; nleek = tunju ; Abalbalat = yg kekal ; mane'o = betul, asli ; amna' = tua ; in una = its beginning ## na' mo'on = life story, riwayat (not known by all) ; ma'mo'en = perbuatan</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Nai' Joao Aslain Atoni, naleko' lalan neu Usif Jesus",
                    "<bt>Sir Joao the *Baptiser of People, fix/prepares the way/path for the *Lord Jesus</bt>",
                "</p>",

                "<p class=\"Parallel Passage Reference\" usfm=\"r\">",
                    "(Mateus 3:1-12; Lukas 3:1-18; Joao 1:19-28)",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"2\"/>",
                    "Jesus fe' ka nanaob In mepu, mes Uis-neno nsonu' nahuun In atoni mese', in kanan nai' Joao. Nai' Joao musti nao naleko' lalan neu Jesus In amneman. Fun natuin na'ko un-unu', Uis-neno anpaek nalail In mafeef' es. Mafefa' nane, in kanan Na'i Yesaya. In ntui nani, nak on ii:",
                    "<bt>Jesus had not yet begun His work, but God sent beforehand one of His men, whose name as sir Joao. Sir Joao must fix/prepare the path/way for Jesus' coming. Because from long ago, God had used one of His mouth (=spokesperson). That spokesperson was named Grandfather/ancestor Yesaya. He had written like this:</bt>",
                    "<Annotation class=\"General\" selectedText=\"nasoitan = buka ; nseef =\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-16 17:48:13Z\">",
                            "nasoitan = buka ; nseef = buka, (tali) ; naloitan = perbaiki ; nani = memang, fore-",
                        "</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Line 1\" usfm=\"q1\">",
                    "&lt;&lt;Mneen nai, he! Au 'leul Au 'haef ma 'nimaf, henati nao naleko' lalan neu Ko",
                    "<bt>&lt;&lt;Listen up, he! I send My foot and hand (=trusty servant) to go fix/prepare the way/path for You.</bt>",
                    "<note reference=\"1:2\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">Maleakhi 3:1</note>",
                "</p>",

                "<p class=\"Line 1\" usfm=\"q1\">",
                     "<v n=\"3\"/>",
                     "Le atoni nane lof in anao mbi bael sona' es, he in nkoa', mnak:",
                     "<bt>That man will go to an uninhabited place, to shout.words, saying:</bt>",
               "</p>",


            "</book>",
            "</bible>"
        };
        #endregion

        // 2. Baikeno Mark 1:9
        #region TestData #2 - BaikenoMark0109_ImportVariant
        static public string[] BaikenoMark0109_ImportVariant = new string[] 
		{
			"\\rcrd MRK 01.09-01.11",
			"\\s Nai' Joao naslain Usif Jesus",
			"\\bts Sirr Joao baptises the Lord Jesus",
			"\\r (Mateus 3:13-17; Lukas 3:21-22)",
			"\\p",
			"\\v 9",
			"\\vt Mbi neno nane, Jesus neem na'ko kuan Najaret, mbi profinsia Galilea, " +
			"ma he na'euk nok nai' Joao. Ma nai' Joao naslani Ee mbi noel Jordan.",
			"\\btvt At that time, Jesus came from the village of Najaret, in the profinsia " +
			"of Galilea, and he met with sir Joao. And sir Joao *baptised Him in the " +
			"Jordan river.",
			"\\nt na'eku < na'euk ; /j/ is halfway between [z] and [j]",
			"\\v 10",
			"\\vt Olas Jesus mpoi na'ko oel, suk naskeek, napeen niit neno-tunan natfei'. " +
			"Nalali te, Uis-neno In Smana Knino' nsaon neem neu Ne, namnees onle' kol-pani.",
			"\\btvt When Jesus came out from the water, suddenly, was seen the heaven/sky " +
			"opened (=no Actor). Then God's *Spirit descended coming to Him, like a dove.",
			"\\nt natfei = opened, tabela  ; natfaika = like a ship parting the waters ; " +
			"habu = sky, clouds ; nipu = clouds ; Asmanan ; kolo = burung ; kol-pani = " +
			"yg biasa orang piara, putih, abu-abu, coklat muda",
			"\\v 11",
			"\\vt Ma on nane te, neen Uis-neno In hanan na'ko neno-tunan, nak,",
			"\\btvt And then was heard (=no Actor) God's voice from the sky, saying,",
			"\\q",
			"\\vt <<Ho le' ii, Au An-Honi'.",
			"\\btvt <<You here, are My 1) very own Child, 2) beloved Child [ambiguous].",
			"\\nt An-Honi",
			"\\q2",
			"\\vt Ho es meki mhaliin Kau, Au nekak.>>",
			"\\btvt You are the one who pleases my liver.>>",
			"\\cf 1:11: *Kejadian 22:2, Mazmur 2:7, Yesaya 42:1, Mateus 3:17, 12:18, Markus " +
			"9:7, Lukas 3:22*",
			"\\nt neno-tunan ~ pah-pinan ; meki ; mhaliin = senang",
			"\\cat c:\\graphics\\cook\\cnt\\cn01656b.tif",
			"\\ref width:9.0cm",
			"\\cap Joao naslain nalail Usif Jesus",
			"\\btcap Joao has finished baptising the Lord Yesus",
			"\\p"
		};
        #endregion
        #region TestData #2 - BaikenoMark0109_Cannonical
        static public string[] BaikenoMark0109_Cannonical = new string[] 
		{
		    "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
            "\\rcrd MRK",
            "\\mt",
            "\\id MRK",
			"\\rcrd MRK 01.09-01.11",
			"\\s Nai' Joao naslain Usif Jesus",
			"\\bts Sirr Joao baptises the Lord Jesus",
			"\\r (Mateus 3:13-17; Lukas 3:21-22)",
			"\\p",
			"\\v 9",
			"\\vt Mbi neno nane, Jesus neem na'ko kuan Najaret, mbi profinsia Galilea, " +
			    "ma he na'euk nok nai' Joao. Ma nai' Joao naslani Ee mbi noel Jordan.",
			"\\btvt At that time, Jesus came from the village of Najaret, in the profinsia " +
			    "of Galilea, and he met with sir Joao. And sir Joao *baptised Him in the " +
			    "Jordan river.",
			"\\nt na'eku < na'euk ; /j/ is halfway between [z] and [j]",
			"\\v 10",
			"\\vt Olas Jesus mpoi na'ko oel, suk naskeek, napeen niit neno-tunan natfei'. " +
			    "Nalali te, Uis-neno In Smana Knino' nsaon neem neu Ne, namnees onle' kol-pani.",
			"\\btvt When Jesus came out from the water, suddenly, was seen the heaven/sky " +
			    "opened (=no Actor). Then God's *Spirit descended coming to Him, like a dove.",
        // Removed a space
			"\\nt natfei = opened, tabela ; natfaika = like a ship parting the waters ; " +
			    "habu = sky, clouds ; nipu = clouds ; Asmanan ; kolo = burung ; kol-pani = " +
			    "yg biasa orang piara, putih, abu-abu, coklat muda",
			"\\v 11",
			"\\vt Ma on nane te, neen Uis-neno In hanan na'ko neno-tunan, nak,",
			"\\btvt And then was heard (=no Actor) God's voice from the sky, saying,",
			"\\q",
			"\\vt <<Ho le' ii, Au An-Honi'.",
			"\\btvt <<You here, are My 1) very own Child, 2) beloved Child [ambiguous].",
			"\\nt An-Honi",
			"\\q2",
			"\\vt Ho es meki mhaliin Kau, Au nekak.>>",
			"\\btvt You are the one who pleases my liver.>>",
        // Reorder's "nt" to come before "cf"
			"\\nt neno-tunan ~ pah-pinan ; meki ; mhaliin = senang",
        // Removed '*' 's
			"\\cf 1:11: Kejadian 22:2, Mazmur 2:7, Yesaya 42:1, Mateus 3:17, 12:18, Markus " +
			"9:7, Lukas 3:22",
			"\\cat c:\\graphics\\cook\\cnt\\cn01656b.tif",
			"\\ref width:9.0cm",
			"\\cap Joao naslain nalail Usif Jesus",
			"\\btcap Joao has finished baptising the Lord Yesus",
			"\\p"
		};
        #endregion
        #region TestData #2 - BaikenoMark0109_oxes
        static public string[] BaikenoMark0109_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Nai' Joao naslain Usif Jesus",
                    "<bt>Sirr Joao baptises the Lord Jesus</bt>",
                "</p>",

                "<p class=\"Parallel Passage Reference\" usfm=\"r\">",
                    "(Mateus 3:13-17; Lukas 3:21-22)",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"9\"/>",
                    "Mbi neno nane, Jesus neem na'ko kuan Najaret, mbi profinsia Galilea, ma he na'euk nok nai' Joao. Ma nai' Joao naslani Ee mbi noel Jordan.",
                    "<bt>At that time, Jesus came from the village of Najaret, in the profinsia of Galilea, and he met with sir Joao. And sir Joao *baptised Him in the Jordan river.</bt>",
                    "<Annotation class=\"General\" selectedText=\"na'eku &lt; na'euk ; /j/ is\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-16 18:19:00Z\">na'eku &lt; na'euk ; /j/ is halfway between [z] and [j]</Message>",
                    "</Annotation>",
                    "<v n=\"10\"/>",
                    "Olas Jesus mpoi na'ko oel, suk naskeek, napeen niit neno-tunan natfei'. Nalali te, Uis-neno In Smana Knino' nsaon neem neu Ne, namnees onle' kol-pani.",
                    "<bt>When Jesus came out from the water, suddenly, was seen the heaven/sky opened (=no Actor). Then God's *Spirit descended coming to Him, like a dove.</bt>",
                    "<Annotation class=\"General\" selectedText=\"natfei = opened, tabela ; natfaika\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-16 18:19:00Z\">natfei = opened, tabela ; natfaika = like a ship parting the waters ; habu = sky, clouds ; nipu = clouds ; Asmanan ; kolo = burung ; kol-pani = yg biasa orang piara, putih, abu-abu, coklat muda</Message>",
                    "</Annotation>",
                    "<v n=\"11\"/>",
                    "Ma on nane te, neen Uis-neno In hanan na'ko neno-tunan, nak,",
                    "<bt>And then was heard (=no Actor) God's voice from the sky, saying,</bt>",
                "</p>",

                "<p class=\"Line 1\" usfm=\"q1\">",
                    "&lt;&lt;Ho le' ii, Au An-Honi'.",
                    "<bt>&lt;&lt;You here, are My 1) very own Child, 2) beloved Child [ambiguous].</bt>",
                    "<Annotation class=\"General\" selectedText=\"An-Honi\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-16 18:19:00Z\">An-Honi</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Line 2\" usfm=\"q2\">",
                    "Ho es meki mhaliin Kau, Au nekak.&gt;&gt;",
                    "<bt>You are the one who pleases my liver.&gt;&gt;</bt>",
                    "<Annotation class=\"General\" selectedText=\"neno-tunan ~ pah-pinan ; meki ;\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-16 18:19:00Z\">neno-tunan ~ pah-pinan ; meki ; mhaliin = senang</Message>",
                    "</Annotation>",
                    "<note reference=\"1:11\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">Kejadian 22:2, Mazmur 2:7, Yesaya 42:1, Mateus 3:17, 12:18, Markus 9:7, Lukas 3:22</note>",
                "</p>",

                "<fig path=\"c:\\graphics\\cook\\cnt\\cn01656b.tif\" rtfFormat=\"width:9.0cm\">",
                    "Joao naslain nalail Usif Jesus",
                    "<bt>Joao has finished baptising the Lord Yesus</bt>",
                "</fig>",

               "<p class=\"Paragraph\" usfm=\"p\" />",

            "</book>",
            "</bible>"
        };
        #endregion

        // 3.  Baikeno Mark 16:19
        #region TestData #3 - BaikenoMark16_ImportVariant
        static public string[] BaikenoMark16_ImportVariant = new string[] 
		{
			"\\rcrd MRK 1",
			"\\s Usif Jesus ansae on neno-tunan",
			"\\bts The Lord Jesus ascends to heaven",
			"\\r (Lukas 24:50-53; Haefin 1:9-11)",
			"\\p",
			"\\v 19",
			"\\vt Namolok nalail nok In atopu'-noina' sin, Uis-neno na'aiti' " +
			"nasaeb Usif Jesus on sonaf neno-tunan. Mbi nane, In ntook mbi " +
			"Uis-neno In banapan a'ne'u, ma sin nhuuk plenat nabuan.",
			"\\btvt After having spoken with His disciples, God took up the " +
			"Lord Jesus to the palace/kingdom in heaven. There, He sits at " +
			"God's right side, and they hold rule together.",
			"\\cf 16:19: *Kisah Para Rasul 1:9-11*",
			"\\p",
			"\\v 20",
			"\\vt Nalali te, In atopu'-noina' sin nanaoba In aplenat. Sin naon " +
			"neu pah-pah, ma natonan Usif Jesus In Lais Alekot neu atoni " +
			"ok-oke'. Ma Uis-neno nfee sin kuasat, henati sin anmo'en lasi " +
			"mkakat ok-oke' le' Usif Jesus natoon nalail neu sin. Ma nalail, " +
			"nmui' atoni namfau le' npalsai neu Usif Jesus, fun sin nahinen " +
			"nak, Lais Alekot nane, namneo.",
			"\\btvt After that, His disciples carried out His commands. They " +
			"went to various lands and told the Lord Jesus' Good News to all " +
			"people. And God gave them power so that they did all the miracles " +
			"that the Lord Jesus had foretold to them. And then, there were " +
			"many people who believed in the Lord Jesus, cause they knew that " +
			"the Good News was true.",
			"\\p",
			"\\p",
			"\\s NAI' MARKUS NAHEUB IN MOLOK, NATUIN LULAT UN-UNU' BIAN",
			"\\bts Sir Markus ends his story, according to other old writings",
			"\\ft 16:9-10: Tuis uab Yunani le' ahun-hunut ma le' naleko neis, " +
			"na'tu'bon es ela' 8. Nai' Markus in Tonas namsoup be neik lasi " +
			"nono' nua. Es amnanu (Markus 16:9-20), ma esa na'paal (Markus " +
			"16:9-10). Natuin atoin ahinet sin, le' nahiin mane'o-mane'o Nai' " +
			"Markus in Tonas ii, sin nak lasi nono' nua in ii, le' sin nluul " +
			"namunib. Lasi nono' nua in nane, naleta' neu Usif Jesus nmoni " +
			"nfain na'ko In a'maten, ma mepu plenat neu atoni le' anpalsai " +
			"neu Jesus.",
			"\\btft 16:9-10: The oldest writings in the Yunani language that " +
			"are better, finish at verse 8. Sir Markus' Story ends with two " +
			"story versions. One is long (Markus 16:9-20), and one is short " +
			"(Markus 16:9-10). According to knowledgable people, who really " +
			"understand this Story of Sir Markus, they say that both of " +
			"these versions were written later. Both of those versions tell " +
			"about the Lord Jesus living again from His death, and the work " +
			"orders to people who believe in Jesus.",
			"\\p",
			"\\v 9",
			"\\vt Olas bifeel teun in nane, naon ntenuk sin Pedru, sin natonan " +
			"ok-oke' le' alelo sin niit li'an munif nane, le' namolok nok " +
			"sin mbi bol fatu.",
			"\\btvt When those three women went arriving at Pedru, they told " +
			"everything that they had just seen of that young man, which he " +
			"had told them at the rock hole.",
			"\\v 10",
			"\\vt Oke te, Usif Jesus naplenat kun In atopu'-noina' sin, he " +
			"naon natonan In Lais Alekot ii neu pah-pah ok-oke', tal antee " +
			"pah-pinan fun am nateef. Lais Alekot ije nalekan lalan henati " +
			"Uis-neno nsaok atoni amfau tin na'ko sin sanat ma penu, ma he " +
			"nmoin piut nok Uis-neno.",
			"\\btvt Then the Lord Jesus himself commanded His disciples to go " +
			"tell this Good News of His in all lands/countries, until the " +
			"far corners of the earth. This Good News shows the way so that " +
			"God can wipe away the sins1 and wrongs1 of many people, and so " +
			"they can live continually with God.",
			"\\p",
			"\\vt Lais Alekot ije, namneo on naan. Es nane te, Lais Alekot ii " +
			"nhaek piut, tal antee nabal-baal. Amen.",
			"\\btvt This Good News is really true. That is why this Good News " +
			"continues to stand, forever. Amen.",
			"\\nt apenut = orang pakane'o ; nhaek = berdiri, tegak, teguh",
			"\\p",
			"\\cat c:\\graphics\\maps\\bible\\palestinTP.jpg" ,
			"\\ref width:10.5cm" ,
			"\\e"	
		};
        #endregion
        #region TestData #3 - BaikenoMark16_Cannonical
        static public string[] BaikenoMark16_Cannonical = new string[] 
		{
		    "\\_sh v3.0 3 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
            "\\rcrd MRK",
            "\\mt",
            "\\id MRK",
			"\\rcrd MRK 1",
			"\\s Usif Jesus ansae on neno-tunan",
			"\\bts The Lord Jesus ascends to heaven",
			"\\r (Lukas 24:50-53; Haefin 1:9-11)",
			"\\p",
			"\\v 19",
			"\\vt Namolok nalail nok In atopu'-noina' sin, Uis-neno na'aiti' " +
			    "nasaeb Usif Jesus on sonaf neno-tunan. Mbi nane, In ntook mbi " +
			    "Uis-neno In banapan a'ne'u, ma sin nhuuk plenat nabuan.",
			"\\btvt After having spoken with His disciples, God took up the " +
			    "Lord Jesus to the palace/kingdom in heaven. There, He sits at " +
			    "God's right side, and they hold rule together.",
			"\\cf 16:19: Kisah Para Rasul 1:9-11",
			"\\p",
			"\\v 20",
			"\\vt Nalali te, In atopu'-noina' sin nanaoba In aplenat. Sin naon " +
			    "neu pah-pah, ma natonan Usif Jesus In Lais Alekot neu atoni " +
			    "ok-oke'. Ma Uis-neno nfee sin kuasat, henati sin anmo'en lasi " +
			    "mkakat ok-oke' le' Usif Jesus natoon nalail neu sin. Ma nalail, " +
			    "nmui' atoni namfau le' npalsai neu Usif Jesus, fun sin nahinen " +
			    "nak, Lais Alekot nane, namneo.",
			"\\btvt After that, His disciples carried out His commands. They " +
			    "went to various lands and told the Lord Jesus' Good News to all " +
			    "people. And God gave them power so that they did all the miracles " +
			    "that the Lord Jesus had foretold to them. And then, there were " +
			    "many people who believed in the Lord Jesus, cause they knew that " +
			    "the Good News was true.",
			"\\p",
			"\\p",
            "\\rcrd 2",
        // "|fn" is inserted here
			"\\s NAI' MARKUS NAHEUB IN MOLOK, NATUIN LULAT UN-UNU' BIAN|fn",
			"\\bts Sir Markus ends his story, according to other old writings|fn",
			"\\ft 16:9-10: Tuis uab Yunani le' ahun-hunut ma le' naleko neis, " +
			    "na'tu'bon es ela' 8. Nai' Markus in Tonas namsoup be neik lasi " +
			    "nono' nua. Es amnanu (Markus 16:9-20), ma esa na'paal (Markus " +
			    "16:9-10). Natuin atoin ahinet sin, le' nahiin mane'o-mane'o Nai' " +
			    "Markus in Tonas ii, sin nak lasi nono' nua in ii, le' sin nluul " +
			    "namunib. Lasi nono' nua in nane, naleta' neu Usif Jesus nmoni " +
			    "nfain na'ko In a'maten, ma mepu plenat neu atoni le' anpalsai " +
			    "neu Jesus.",
			"\\btft 16:9-10: The oldest writings in the Yunani language that " +
			    "are better, finish at verse 8. Sir Markus' Story ends with two " +
			    "story versions. One is long (Markus 16:9-20), and one is short " +
			    "(Markus 16:9-10). According to knowledgable people, who really " +
			    "understand this Story of Sir Markus, they say that both of " +
			    "these versions were written later. Both of those versions tell " +
			    "about the Lord Jesus living again from His death, and the work " +
			    "orders to people who believe in Jesus.",
			"\\p",
			"\\v 9",
			"\\vt Olas bifeel teun in nane, naon ntenuk sin Pedru, sin natonan " +
			    "ok-oke' le' alelo sin niit li'an munif nane, le' namolok nok " +
			    "sin mbi bol fatu.",
			"\\btvt When those three women went arriving at Pedru, they told " +
			    "everything that they had just seen of that young man, which he " +
			    "had told them at the rock hole.",
			"\\v 10",
			"\\vt Oke te, Usif Jesus naplenat kun In atopu'-noina' sin, he " +
			    "naon natonan In Lais Alekot ii neu pah-pah ok-oke', tal antee " +
			    "pah-pinan fun am nateef. Lais Alekot ije nalekan lalan henati " +
			    "Uis-neno nsaok atoni amfau tin na'ko sin sanat ma penu, ma he " +
			    "nmoin piut nok Uis-neno.",
			"\\btvt Then the Lord Jesus himself commanded His disciples to go " +
			    "tell this Good News of His in all lands/countries, until the " +
			    "far corners of the earth. This Good News shows the way so that " +
			    "God can wipe away the sins1 and wrongs1 of many people, and so " +
			    "they can live continually with God.",
			"\\p",
			"\\vt Lais Alekot ije, namneo on naan. Es nane te, Lais Alekot ii " +
			    "nhaek piut, tal antee nabal-baal. Amen.",
			"\\btvt This Good News is really true. That is why this Good News " +
			    "continues to stand, forever. Amen.",
			"\\nt apenut = orang pakane'o ; nhaek = berdiri, tegak, teguh",
			"\\p",
			"\\cat c:\\graphics\\maps\\bible\\palestinTP.jpg" ,
			"\\ref width:10.5cm" ,
        // "cap" inserted here; "e" removed
            "\\cap"
		};
        #endregion
        #region TestData #3 - BaikenoMark16_oxes
        static public string[] BaikenoMark16_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Usif Jesus ansae on neno-tunan",
                    "<bt>The Lord Jesus ascends to heaven</bt>",
                "</p>",

                "<p class=\"Parallel Passage Reference\" usfm=\"r\">",
                    "(Lukas 24:50-53; Haefin 1:9-11)",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"19\"/>",
                    "Namolok nalail nok In atopu'-noina' sin, Uis-neno na'aiti' nasaeb Usif Jesus on sonaf neno-tunan. Mbi nane, In ntook mbi Uis-neno In banapan a'ne'u, ma sin nhuuk plenat nabuan.",
                    "<bt>After having spoken with His disciples, God took up the Lord Jesus to the palace/kingdom in heaven. There, He sits at God's right side, and they hold rule together.</bt>",
                    "<note reference=\"16:19\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">Kisah Para Rasul 1:9-11</note>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"20\"/>",
                    "Nalali te, In atopu'-noina' sin nanaoba In aplenat. Sin naon neu pah-pah, ma natonan Usif Jesus In Lais Alekot neu atoni ok-oke'. Ma Uis-neno nfee sin kuasat, henati sin anmo'en lasi mkakat ok-oke' le' Usif Jesus natoon nalail neu sin. Ma nalail, nmui' atoni namfau le' npalsai neu Usif Jesus, fun sin nahinen nak, Lais Alekot nane, namneo.",
                    "<bt>After that, His disciples carried out His commands. They went to various lands and told the Lord Jesus' Good News to all people. And God gave them power so that they did all the miracles that the Lord Jesus had foretold to them. And then, there were many people who believed in the Lord Jesus, cause they knew that the Good News was true.</bt>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\" />",

                "<p class=\"Paragraph\" usfm=\"p\" />",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "NAI' MARKUS NAHEUB IN MOLOK, NATUIN LULAT UN-UNU' BIAN",
                    "<bt>Sir Markus ends his story, according to other old writings</bt>",
                    "<note reference=\"16:9-10\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "Tuis uab Yunani le' ahun-hunut ma le' naleko neis, na'tu'bon es ela' 8. Nai' Markus in Tonas namsoup be neik lasi nono' nua. Es amnanu (Markus 16:9-20), ma esa na'paal (Markus 16:9-10). Natuin atoin ahinet sin, le' nahiin mane'o-mane'o Nai' Markus in Tonas ii, sin nak lasi nono' nua in ii, le' sin nluul namunib. Lasi nono' nua in nane, naleta' neu Usif Jesus nmoni nfain na'ko In a'maten, ma mepu plenat neu atoni le' anpalsai neu Jesus.",
                        "<bt>The oldest writings in the Yunani language that are better, finish at verse 8. Sir Markus' Story ends with two story versions. One is long (Markus 16:9-20), and one is short (Markus 16:9-10). According to knowledgable people, who really understand this Story of Sir Markus, they say that both of these versions were written later. Both of those versions tell about the Lord Jesus living again from His death, and the work orders to people who believe in Jesus.</bt>",
                    "</note>",
                "</p>",
               
                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"9\"/>",
                    "Olas bifeel teun in nane, naon ntenuk sin Pedru, sin natonan ok-oke' le' alelo sin niit li'an munif nane, le' namolok nok sin mbi bol fatu.",
                    "<bt>When those three women went arriving at Pedru, they told everything that they had just seen of that young man, which he had told them at the rock hole.</bt>",
                    "<v n=\"10\"/>",
                    "Oke te, Usif Jesus naplenat kun In atopu'-noina' sin, he naon natonan In Lais Alekot ii neu pah-pah ok-oke', tal antee pah-pinan fun am nateef. Lais Alekot ije nalekan lalan henati Uis-neno nsaok atoni amfau tin na'ko sin sanat ma penu, ma he nmoin piut nok Uis-neno.",
                    "<bt>Then the Lord Jesus himself commanded His disciples to go tell this Good News of His in all lands/countries, until the far corners of the earth. This Good News shows the way so that God can wipe away the sins1 and wrongs1 of many people, and so they can live continually with God.</bt>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "Lais Alekot ije, namneo on naan. Es nane te, Lais Alekot ii nhaek piut, tal antee nabal-baal. Amen.",
                    "<bt>This Good News is really true. That is why this Good News continues to stand, forever. Amen.</bt>",
                    "<Annotation class=\"General\" selectedText=\"apenut = orang pakane'o ; nhaek\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 01:32:32Z\">apenut = orang pakane'o ; nhaek = berdiri, tegak, teguh</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\" />",

                "<fig path=\"c:\\graphics\\maps\\bible\\palestinTP.jpg\" rtfFormat=\"width:10.5cm\" />",

            "</book>",
            "</bible>"
        };
        #endregion

        // 4. Baikeno Mark 4:30-34
        #region TestData #4 - BaikenoMark430_ImportVariant
        static public string[] BaikenoMark430_ImportVariant = new string[] 
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
        #region TestData #4 - BaikenoMark430_Cannonical
        static public string[] BaikenoMark430_Cannonical = new string[] 
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
        #region TestData #4 - BaikenoMark430_oxes
        static public string[] BaikenoMark430_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Header\" usfm=\"h\">",
                    "Mark",
                "</p>",

                "<p class=\"Title Secondary\" usfm=\"mt2\">",
                    "The Gospel Of",
                "</p>",

                "<p class=\"Title Main\" usfm=\"mt\">",
                    "Mark",
                "</p>",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Usif Jesus naleta' neu fini le' in nesan an-ana' neis",
                    "<bt>The Lord Jesus give an example of a seed that is extremely tiny</bt>",
                    "<Annotation class=\"General\" selectedText=\"sain = jenis biji yg kici\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 01:55:19Z\">sain = jenis biji yg kici ana</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Parallel Passage Reference\" usfm=\"r\">",
                    "(Mateus 13:31-32, 34; Lukas 13:18-19)",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"30\"/>",
                    "Oke te, <span class=\"Italic\">Jesus namolok</span> antein, mnak, &lt;&lt;Au uleta' 'tein on ii: Hi nabei' mnoon Uis-neno in toob. Na'ko sin tuaf fua' fauk es, mes nabaab-took, tal antee sin namfau nok.",
                    "<bt>Then Jesus spoke again, saying, &lt;&lt;I give another example like this: You(pl) can compare God's *social group. From just a few people, it nevertheless increases (in number), to the point that they are very many.</bt>",
                    "<Annotation class=\"General\" selectedText=\"bonak = Setaria italica, Rote =\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 01:55:19Z\">bonak = Setaria italica, Rote = botok ; an-ana' = very small ; fini = seed for planting ; minoon = banding, ; kle'o = few ; nabaab-took = tamba banyak</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"31\"/>",
                    "Nane namnees onle' fini le' in nesan an-ana' neis.",
                    "<bt>That is like a seed that is very tiny.</bt>",
                    "<v n=\"32\"/>",
                    "Kalu hit tseen nesaf nane, in lofa nmoin jael hau 'naek. Ma lof kolo neem namin mafo', ma nmo'en kuna' neu ne.&gt;&gt;",
                    "<bt>If we(inc) plant it (with dibble stick) it will grow to become a large tree. And birds will come looking for shade, and make nests in it.&gt;&gt;</bt>",
                    "<Annotation class=\"General\" selectedText=\"tseen ~ [ceen] = plant with\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 01:55:19Z\">tseen ~ [ceen] = plant with dibble stick ; hau tlaef = ranting pohon ; mafo' = sombar ; kuna' = sarang</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"33\"/>",
                    "Jesus In na'noina' in ma'mo'en natuin sin hiin kini.",
                    "<bt>Jesus' way of teaching was according to their understanding.</bt>",
                    "<v n=\"34\"/>",
                    "In na'noina' atoni neki haa lais leta'. Mes kalu nok In atopu'-noina' sin, In natoon lais leta' nane in na' naon ok-oke'.",
                    "<bt>He taught people using only examples. But with His *disciples, He told the meaning of all the examples.</bt>",
                "</p>",

            "</book>",
            "</bible>"
        };
        #endregion

        // 5. Pura Mark 14:03-14:09
        #region TestData #5 - PuraMark14_ImportVariant
        static public string[] PuraMark14_ImportVariant = new string[] 
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
			    "\\btvt This is some more back translation.|fn",
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
			    "\\btvt Some back translating for verse 5|fn",
                "\\ft A footnote for verse 5",
			    "\\cf 14:7: Ulangan 15:11",
			    "\\v 6",
			    "\\vt Ne e vetang lung niang ila. Mang nehe jangu anga vede mina obokong " +
			    "anga, ana sidiat ila ma Neboa veng etura, emang hula ana Ne baring " +
			    "veng bunga me at ila.|fn",
			    "\\btvt And verse six reads as so.|fn",
                "\\nt And this is a translator note",
                "\\ft Verse six footnote",
			    "\\v 7",
			    "\\vt Benganit aung-aung, o`! Ta ang mi ba Tuhang Lahatala E Sirinta " +
			    "Aung ila alamula anga goleng, indi pasti nehe jangu e aung anga " +
			    "veng sirinta! E biar ne emampi aing vengani.>>",
			    "\\btvt A final backtranslation to end it all."
		    };
        #endregion
        #region TestData #5 - PuraMark14_Cannonical
        static public string[] PuraMark14_Cannonical = new string[] 
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
			    "\\btvt This is some more back translation.|fn",
			    "\\ft 1:1: This is a footnote.",
 			    "\\btft 1:1:",
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
			    "\\btvt Some back translating for verse 5|fn",
                "\\ft 1:5: A footnote for verse 5",
 			    "\\btft 1:5:",
                "\\vt lung niang ila, se Na ing veng hama-hama niang ila.",
			    "\\cf 14:7: Ulangan 15:11",
			    "\\v 6",
			    "\\vt Ne e vetang lung niang ila. Mang nehe jangu anga vede mina obokong " +
			    "anga, ana sidiat ila ma Neboa veng etura, emang hula ana Ne baring " +
			    "veng bunga me at ila.|fn",
			    "\\btvt And verse six reads as so.|fn",
                "\\nt And this is a translator note",
                "\\ft 1:6: Verse six footnote",
 			    "\\btft 1:6:",
			    "\\v 7",
			    "\\vt Benganit aung-aung, o`! Ta ang mi ba Tuhang Lahatala E Sirinta " +
			    "Aung ila alamula anga goleng, indi pasti nehe jangu e aung anga " +
			    "veng sirinta! E biar ne emampi aing vengani.>>",
			    "\\btvt A final backtranslation to end it all."
		    };
        #endregion
        #region TestData #5 - PuraMark14_oxes
        static public string[] PuraMark14_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Header\" usfm=\"h\">",
                    "Mark",
                "</p>",

                "<p class=\"Title Secondary\" usfm=\"mt2\">",
                    "The Gospel Of",
                "</p>",

                "<p class=\"Title Main\" usfm=\"mt\">",
                    "Mark",
                "</p>",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Ne he jangu ba mina menema e vili ele boal, ma Tuhang Yesus enang",
                "</p>",

                "<p class=\"Parallel Passage Reference\" usfm=\"r\">",
                    "(Matius 26:6-13; Yohanis 12:1-8)",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                   "<c n=\"1\"/>",
                   "<v n=\"1\"/>",
                    "Abang Betania mi, ne nu ue ene Simon. Turang mi, ne ava aing veng ororing, tagal dirang hapeburang aing veng. Ba sakarang, ana aung ila.",
                    "<bt>This is some back translation.</bt>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "Seng angu mu, o|ras angu ve|d-ved aung hoa jedung, Yesus ini ue ila Simon e hava mi nana. |Oras |ini nana, nehe jangu nu Yesus evele hoa Aing dapa. Ana ue botol nu pina ba ini var ma ening. |Mina nemema asli ba mi evili talalu ele.",
                    "<bt>This is some more back translation.</bt>",
                    "<note reference=\"1:1\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "This is a footnote.",
                    "</note>",
                    "Seng, nebe jangu angu botol ememng angu|fn ma vil bue ening kivita. Mu ana boal ening to tu tahang-tahang mina angu ma Yesus ong tang|, e jadi tanda Yesus Aing ta janing",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                   "<v n=\"2\"/>",
                    "Ba ne }}ebeung iva di {{ue umurung}} nana. Oras ini eteing nehe jangu " +
			    "angu ening ula{{{{ng angu, mi ini ili il, e ini i ta tu}}tuk sombong " +
			    "hula, &lt;&lt;Hmm! Ne he {{jangu na ba {{ba anga, e ana ila mina}} menema " +
			    "e vili ele angu viat parsuma.",
                    "<bt>Some back translation for verse two.</bt>",
                    "<v n=\"3\"/>",
                    "Lebe aung mina angu ana avali ba! E ila e hoang toang angu paul, ma ne malarat anaung ing enang! Se mina menema angu e vili veng, ma nenu e gaji tung nu veng hama.&gt;&gt;",
                    "<bt>Forescore and seven years ago,</bt>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                   "<v n=\"4\"/>",
                    "Ba Yesus balas hula, &lt;&lt;Ini ake nehe jangu anga ening susa! Mang aing kilang ba! Na sanang, tagal mina menema anga ana ma Noboa veng obokong ila.",
                    "<bt>our forefathers brought forth on this continant</bt>",
                    "<v n=\"5\"/>",
                    "Ne kasiang anaung salalu ae ing veng. Jadi, ini bisa taveding di ini ing tulung. Ba kalo Naing,",
                    "<bt>Some back translating for verse 5</bt>",
                    "<note reference=\"1:5\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "A footnote for verse 5",
                    "</note>",
                    "lung niang ila, se Na ing veng hama-hama niang ila.",
                    "<note reference=\"14:7\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">Ulangan 15:11</note>",
                    "<v n=\"6\"/>",
                    "Ne e vetang lung niang ila. Mang nehe jangu anga vede mina obokong anga, ana sidiat ila ma Neboa veng etura, emang hula ana Ne baring veng bunga me at ila.",
                    "<bt>And verse six reads as so.</bt>",
                    "<Annotation class=\"General\" selectedText=\"And this is a translator note\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:00:57Z\">And this is a translator note</Message>",
                    "</Annotation>",
                    "<note reference=\"1:6\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "Verse six footnote",
                    "</note>",
                    "<v n=\"7\"/>",
                    "Benganit aung-aung, o`! Ta ang mi ba Tuhang Lahatala E Sirinta Aung ila alamula anga goleng, indi pasti nehe jangu e aung anga veng sirinta! E biar ne emampi aing vengani.&gt;&gt;",
                    "<bt>A final backtranslation to end it all.</bt>",
                "</p>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 6. Helong Acts 4:01-4:04
        #region TestData #6 - HelongActs04_ImportVariant
        static public string[] HelongActs04_ImportVariant = new string[] 
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
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai " +
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
				    "(check two kon)|fn",
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
        #region TestData #6 - HelongActs04_Cannonical
        static public string[] HelongActs04_Cannonical = new string[] 
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
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai " +
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
				    "(check two kon)|fn",
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
        #region TestData #6 - HelongActs04_oxes
        static public string[] HelongActs04_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Header\" usfm=\"h\">",
                    "Mark",
                "</p>",

                "<p class=\"Title Secondary\" usfm=\"mt2\">",
                    "The Gospel Of",
                "</p>",

                "<p class=\"Title Main\" usfm=\"mt\">",
                    "Mark",
                "</p>",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Tulu-tulu agama las haman Petrus nol Yohanis maas tala",
                    "<bt>The heads of religion summon Petrus and Yohanis to come appear.before [them]</bt>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                   "<c n=\"4\" />",
                   "<v n=\"1\" />",
                    "Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai agama Saduki. Oen maas komali le ahan Petrus nol Yohanis.",
                    "<bt>At that time, Petrus and Yohanis still were talking with those people, several big/important people came. Those them(=They in focus), [were] heads of the Yahudi religion, and the head of guarding the *Temple, and people from the religious party Saduki. They came angry to scream at Petrus and Yohanis.</bt>",
                    "<Annotation class=\"General\" selectedText=\"doh =doha; tala=mangada\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:04:05Z\">doh =doha; tala=mangada</Message>",
                    "</Annotation>",
                    "<v n=\"2\"/>",
                    "Oen komali lole Petrus nol Yohanis na mo, kom isi le tek atuli-atuli las to-toang, noan, &lt;&lt;Yesus nuli pait son, deng Un in mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon haup in nuli pait kon.&gt;&gt;",
                    "<bt>They were angry because that Petrus and Yohanis, like to tell all people, saying, \"Yesus has lived again from His death! With that, He opened the path for dead people so that they also could live again.\" (check two kon)</bt>",
                    "<note reference=\"4:1-2\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "Atuil deng partai agama Saduki, oen sium in tui na lo man noen atuil mate haup in nuli pait.",
                        "<bt>People from the religious party Saduki, they did not accept that teaching that says dead people can live again.</bt>",
                    "</note>",
                    "<v n=\"3\" />",
                    "Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un deng lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. Le ola ka halas-sam, oen nehan dais na.",
                    "<bt>And so they ordered their people to go capture the two of them. But because the sun already wanted to set, then they pushed [&amp;] entered the two of them in jail. So.that the next day, they could take.care.of that affair/litigation/problem.</bt>",
                    "<Annotation class=\"General\" selectedText=\"nehan = urus, mengatur\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:04:05Z\">nehan = urus, mengatur</Message>",
                    "</Annotation>",
                    "<v n=\"4\" />",
                    "Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
                    "<bt>But from the people who liked to hear from those apostles, therefore many people had already acknowledged that that which they taught, [was] spot.on correct. That's.why their people increased a lot approximately to five thousand people.</bt>",
                "</p>",

                "<fig path=\"c:\\graphics\\cook\\cnt\\cn01901b.tif\" rtfFormat=\"width:7.0cm;textWrapping:around;horizontalPosition:right\">",
                    "Atuil-atuil tene kas klaa Petrus nol tapang ngas",
                    "<bt>The leaders accuse/criticise Petrus and his friends</bt>",
                "</fig>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 7. Helong Acts 7:54-8:01a
        #region TestData #7 - HelongActs0754_ImportVariant
        static public string[] HelongActs0754_ImportVariant = new string[] 
		{
			"\\rcrd ACT 07.54-08.01",
			"\\s Oen pasang tele Stefanus",
			"\\bts They throw killing Stefanus",
			"\\p",
			"\\v 54",
			"\\vt Atuil man dad le nehan dasi la, ming Stefanus in dehet ta, oen tan meman " +
				"noan un soleng bel oen kula ka. Hidim oen dalen ili le duu-duu siin nol " +
				"Stefanus.",
			"\\btvt The person (pl?) who were sitting to take.care of the litigation heard " +
				"that speaking of Stefanus', they knew that he was throwing_out giving their " +
				"wrongs. So they were very angry [lit. sick insides] to ate/ground their " +
				"teeth for Stefanus.",
			"\\nt duu-duu = makan gigi grind teeth ; agak barhenti hujan lebat =   ulan na " +
				"siin son (hujan sudah mau berhenti sedikit) duu = kunya   in mouth ; duta " +
				"= grind/ulik",
			"\\p",
			"\\v 55",
			"\\vt Mo Stefanus man hapu Ama Lamtua Ko Niu' ka, botas ngat laok el apan nua. " +
				"Se la, un ngat net Ama Lamtua Allah dui to-toang nol Ama Lamtua Yesus dil " +
				"se Ama Lamtua Allah halin kanan na, se man in todan dui ka.",
			"\\btvt But Stefanus who had obtained that Holy Spirit of the Lord's, lifted " +
				"his eyes to look at the sky. There, he saw all the Lord God's greaterness, " +
				"and the Lord Yesus standing at the Lord God's right side in that place " +
				"which is more honorable.",
			"\\v 56",
			"\\vt Kon Stefanus dehet noan, <<Elia! Ama-amas to-toang. Auk ngat net apan nu " +
				"hol sai, nol An Atuli la dil ne man in todan lahing isi ka se Ama Lamtua " +
				"Allah halin kanan na.>>",
			"\\btvt And Stefanus spoke saying, <<Like this!, All fathers. I am seeing the sky " +
				"open, and Humanity's Child standing at that place which is most honorable at " +
				"the Lord God's right side.>>",
			"\\nt botas ngat = angkat kepala to see",
			"\\p",
			"\\v 57",
			"\\vt Ming ela kon, atuil in nehan dasi la ka oen kuim hngilans. Hidim oen kidu " +
				"ahan le tek Stefanus noan boel lobo lo. Kon oen tukin haung pul leo-leo le " +
				"pisu sisin Stefanus.",
			"\\btvt Hearing that then, the people who take care of that litigation shut? " +
				"their ears. Then they yelled screaming to order Stefanus to shut (his) mouth. " +
				"And they all jumped up quickly with.alot.of.excitement in.order.to tear apart " +
				"Stefanus.",
			"\\v 58",
			"\\vt Hidi kon oen pela lakang un puti lako likun deng kota la. Ela kon saksi-" +
				"saksi las oen kolong oen kaod likun nas, le bel tana muda mesa le kilas. Un " +
				"ngala ka Saulus. Hidi kon, oen lakos pasang tele Stefanus nini batu.",
			"\\btvt Then they forced him out going outside the city. Next the witnesses took " +
				"off their outside clothes, to give them to a young person to hold. His name " +
				"Saulus, he was their inciter (lit. fanner). And then, they went to throw " +
				"kill Stefanus using stones.",
			"\\nt tukin haung = bangun tiba-tiba ; tukin = bangun/naik ; pisu =   tear ; pisu " +
				"sisin cabut-cabut (cek) ; kuim = tutup; 58: ratulin   in iha-iha = " +
				"provacator; iha = kipas ; kolong = buka ; pela lakang   = paksa; lakang = " +
				"mendesak/dorong",
			"\\cat c:\\graphics\\cook\\cnt\\cn02154b.tif",
			"\\ref width:11.0cm;verticalPosition:top;horizontalPosition:center",
			"\\cap Oen pasang tele Stefanus",
			"\\btcap They throw killing Stefanus",
			"\\p",
			"\\v 59",
			"\\vt Oen pasang Stefanus nabael ela kon, un haman mu-muun le tek noan, <<Ama " +
				"Lamtua Yesus! Auk oras sa da-dani son. Sium auk tia!>>",
			"\\btvt They were throwing at Stefanus, then he yelled with a loud voice saying, " +
				"<<Lord Yesus! My time is very close. Receive Me (imperative)!>>",
			"\\v 60",
			"\\vt Hidi kon un lea holimit hai buku ka, le un ahan pait nol fala mu-muun tek " +
				"noan, <<Ama, beles oen lepa hal kula-sala nia deken!>> Hidi na, un hngasa " +
				"ka nutus, kon mate.",
			"\\btvt Then he fell folding his knees? in.order.to yell again with a loud voice, " +
				"saying, <<Father, don't shoulder.carry this sin!>> And then his breath was " +
				"cut.off/stopped and [he]died.",
			"\\c 8",
			"\\v 1a",
			"\\vt Nol Saulus kon bab se man na, un sium banan nol in pasang tele Stefanus " +
				"son na.",
			"\\btvt And Saulus was also at that place, he accepted well [the fact that] they " +
				"threw murdered that Stefanus.",
			"\\nt holimit = terlipat?",
			"\\ud 24/Jan/2005"
		};
        #endregion
        #region TestData #7 - HelongActs0754_Cannonical
        static public string[] HelongActs0754_Cannonical = new string[] 
		{
            "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd MRK",
		    "\\mt",
            "\\id MRK",
			"\\rcrd ACT 07.54-08.01",
			"\\s Oen pasang tele Stefanus",
			"\\bts They throw killing Stefanus",
			"\\p",
			"\\v 54",
			"\\vt Atuil man dad le nehan dasi la, ming Stefanus in dehet ta, oen tan meman " +
				"noan un soleng bel oen kula ka. Hidim oen dalen ili le duu-duu siin nol " +
				"Stefanus.",
			"\\btvt The person (pl?) who were sitting to take.care of the litigation heard " +
				"that speaking of Stefanus', they knew that he was throwing_out giving their " +
				"wrongs. So they were very angry [lit. sick insides] to ate/ground their " +
				"teeth for Stefanus.",
			"\\nt duu-duu = makan gigi grind teeth ; agak barhenti hujan lebat = ulan na " +
				"siin son (hujan sudah mau berhenti sedikit) duu = kunya in mouth ; duta " +
				"= grind/ulik",
			"\\p",
			"\\v 55",
			"\\vt Mo Stefanus man hapu Ama Lamtua Ko Niu' ka, botas ngat laok el apan nua. " +
				"Se la, un ngat net Ama Lamtua Allah dui to-toang nol Ama Lamtua Yesus dil " +
				"se Ama Lamtua Allah halin kanan na, se man in todan dui ka.",
			"\\btvt But Stefanus who had obtained that Holy Spirit of the Lord's, lifted " +
				"his eyes to look at the sky. There, he saw all the Lord God's greaterness, " +
				"and the Lord Yesus standing at the Lord God's right side in that place " +
				"which is more honorable.",
			"\\v 56",
			"\\vt Kon Stefanus dehet noan, <<Elia! Ama-amas to-toang. Auk ngat net apan nu " +
				"hol sai, nol An Atuli la dil ne man in todan lahing isi ka se Ama Lamtua " +
				"Allah halin kanan na.>>",
			"\\btvt And Stefanus spoke saying, <<Like this!, All fathers. I am seeing the sky " +
				"open, and Humanity's Child standing at that place which is most honorable at " +
				"the Lord God's right side.>>",
			"\\nt botas ngat = angkat kepala to see",
			"\\p",
			"\\v 57",
			"\\vt Ming ela kon, atuil in nehan dasi la ka oen kuim hngilans. Hidim oen kidu " +
				"ahan le tek Stefanus noan boel lobo lo. Kon oen tukin haung pul leo-leo le " +
				"pisu sisin Stefanus.",
			"\\btvt Hearing that then, the people who take care of that litigation shut? " +
				"their ears. Then they yelled screaming to order Stefanus to shut (his) mouth. " +
				"And they all jumped up quickly with.alot.of.excitement in.order.to tear apart " +
				"Stefanus.",
			"\\v 58",
			"\\vt Hidi kon oen pela lakang un puti lako likun deng kota la. Ela kon saksi-" +
				"saksi las oen kolong oen kaod likun nas, le bel tana muda mesa le kilas. Un " +
				"ngala ka Saulus. Hidi kon, oen lakos pasang tele Stefanus nini batu.",
			"\\btvt Then they forced him out going outside the city. Next the witnesses took " +
				"off their outside clothes, to give them to a young person to hold. His name " +
				"Saulus, he was their inciter (lit. fanner). And then, they went to throw " +
				"kill Stefanus using stones.",
			"\\nt tukin haung = bangun tiba-tiba ; tukin = bangun/naik ; pisu = tear ; pisu " +
				"sisin cabut-cabut (cek) ; kuim = tutup; 58: ratulin in iha-iha = " +
				"provacator; iha = kipas ; kolong = buka ; pela lakang = paksa; lakang = " +
				"mendesak/dorong",
			"\\cat c:\\graphics\\cook\\cnt\\cn02154b.tif",
			"\\ref width:11.0cm;verticalPosition:top;horizontalPosition:center",
			"\\cap Oen pasang tele Stefanus",
			"\\btcap They throw killing Stefanus",
			"\\p",
			"\\v 59",
			"\\vt Oen pasang Stefanus nabael ela kon, un haman mu-muun le tek noan, <<Ama " +
				"Lamtua Yesus! Auk oras sa da-dani son. Sium auk tia!>>",
			"\\btvt They were throwing at Stefanus, then he yelled with a loud voice saying, " +
				"<<Lord Yesus! My time is very close. Receive Me (imperative)!>>",
			"\\v 60",
			"\\vt Hidi kon un lea holimit hai buku ka, le un ahan pait nol fala mu-muun tek " +
				"noan, <<Ama, beles oen lepa hal kula-sala nia deken!>> Hidi na, un hngasa " +
				"ka nutus, kon mate.",
			"\\btvt Then he fell folding his knees? in.order.to yell again with a loud voice, " +
				"saying, <<Father, don't shoulder.carry this sin!>> And then his breath was " +
				"cut.off/stopped and [he]died.",
			"\\c 8",
			"\\v 1a",
			"\\vt Nol Saulus kon bab se man na, un sium banan nol in pasang tele Stefanus " +
				"son na.",
			"\\btvt And Saulus was also at that place, he accepted well [the fact that] they " +
				"threw murdered that Stefanus.",
			"\\nt holimit = terlipat?",
			"\\ud 24/Jan/2005"
		};
        #endregion
        #region TestData #7 - HelongActs0754_oxes
        static public string[] HelongActs0754_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Oen pasang tele Stefanus",
                    "<bt>They throw killing Stefanus</bt>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"54\" />",
                    "Atuil man dad le nehan dasi la, ming Stefanus in dehet ta, oen tan meman noan un soleng bel oen kula ka. Hidim oen dalen ili le duu-duu siin nol Stefanus.",
                    "<bt>The person (pl?) who were sitting to take.care of the litigation heard that speaking of Stefanus', they knew that he was throwing_out giving their wrongs. So they were very angry [lit. sick insides] to ate/ground their teeth for Stefanus.</bt>",
                    "<Annotation class=\"General\" selectedText=\"duu-duu = makan gigi grind teeth\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:07:36Z\">duu-duu = makan gigi grind teeth ; agak barhenti hujan lebat = ulan na siin son (hujan sudah mau berhenti sedikit) duu = kunya in mouth ; duta = grind/ulik</Message>",                        
                    "</Annotation>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"55\" />",
                    "Mo Stefanus man hapu Ama Lamtua Ko Niu' ka, botas ngat laok el apan nua. Se la, un ngat net Ama Lamtua Allah dui to-toang nol Ama Lamtua Yesus dil se Ama Lamtua Allah halin kanan na, se man in todan dui ka.",
                    "<bt>But Stefanus who had obtained that Holy Spirit of the Lord's, lifted his eyes to look at the sky. There, he saw all the Lord God's greaterness, and the Lord Yesus standing at the Lord God's right side in that place which is more honorable.</bt>",
                    "<v n=\"56\" />",
                    "Kon Stefanus dehet noan, &lt;&lt;Elia! Ama-amas to-toang. Auk ngat net apan nu hol sai, nol An Atuli la dil ne man in todan lahing isi ka se Ama Lamtua Allah halin kanan na.&gt;&gt;",
                    "<bt>And Stefanus spoke saying, &lt;&lt;Like this!, All fathers. I am seeing the sky open, and Humanity's Child standing at that place which is most honorable at the Lord God's right side.&gt;&gt;</bt>",
                    "<Annotation class=\"General\" selectedText=\"botas ngat = angkat kepala to\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:07:36Z\">botas ngat = angkat kepala to see</Message>",
                    "</Annotation>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"57\" />",
                    "Ming ela kon, atuil in nehan dasi la ka oen kuim hngilans. Hidim oen kidu ahan le tek Stefanus noan boel lobo lo. Kon oen tukin haung pul leo-leo le pisu sisin Stefanus.",
                    "<bt>Hearing that then, the people who take care of that litigation shut? their ears. Then they yelled screaming to order Stefanus to shut (his) mouth. And they all jumped up quickly with.alot.of.excitement in.order.to tear apart Stefanus.</bt>",
                    "<v n=\"58\" />",
                    "Hidi kon oen pela lakang un puti lako likun deng kota la. Ela kon saksi-saksi las oen kolong oen kaod likun nas, le bel tana muda mesa le kilas. Un ngala ka Saulus. Hidi kon, oen lakos pasang tele Stefanus nini batu.",
                    "<bt>Then they forced him out going outside the city. Next the witnesses took off their outside clothes, to give them to a young person to hold. His name Saulus, he was their inciter (lit. fanner). And then, they went to throw kill Stefanus using stones.</bt>",
                    "<Annotation class=\"General\" selectedText=\"tukin haung = bangun tiba-tiba ;\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:07:36Z\">tukin haung = bangun tiba-tiba ; tukin = bangun/naik ; pisu = tear ; pisu sisin cabut-cabut (cek) ; kuim = tutup; 58: ratulin in iha-iha = provacator; iha = kipas ; kolong = buka ; pela lakang = paksa; lakang = mendesak/dorong</Message>",
                    "</Annotation>",
                "</p>",

                "<fig path=\"c:\\graphics\\cook\\cnt\\cn02154b.tif\" rtfFormat=\"width:11.0cm;verticalPosition:top;horizontalPosition:center\">",
                    "Oen pasang tele Stefanus",
                    "<bt>They throw killing Stefanus</bt>",
                "</fig>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"59\" />",
                    "Oen pasang Stefanus nabael ela kon, un haman mu-muun le tek noan, &lt;&lt;Ama Lamtua Yesus! Auk oras sa da-dani son. Sium auk tia!&gt;&gt;",
                    "<bt>They were throwing at Stefanus, then he yelled with a loud voice saying, &lt;&lt;Lord Yesus! My time is very close. Receive Me (imperative)!&gt;&gt;</bt>",
                    "<v n=\"60\" />",
                    "Hidi kon un lea holimit hai buku ka, le un ahan pait nol fala mu-muun tek noan, &lt;&lt;Ama, beles oen lepa hal kula-sala nia deken!&gt;&gt; Hidi na, un hngasa ka nutus, kon mate.",
                    "<bt>Then he fell folding his knees? in.order.to yell again with a loud voice, saying, &lt;&lt;Father, don't shoulder.carry this sin!&gt;&gt; And then his breath was cut.off/stopped and [he]died.</bt>",
                    "<c n=\"8\" />",
                    "<v n=\"1a\" />",
                    "Nol Saulus kon bab se man na, un sium banan nol in pasang tele Stefanus son na.",
                    "<bt>And Saulus was also at that place, he accepted well [the fact that] they threw murdered that Stefanus.</bt>",
                    "<Annotation class=\"General\" selectedText=\"holimit = terlipat?\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:07:36Z\">holimit = terlipat?</Message>",
                    "</Annotation>",
                "</p>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 8. Helong Rev 1:4-8 (empty shell)
        #region TestData #8 - HelongRev0104_ImportVariant
        static public string[] HelongRev0104_ImportVariant = new string[] 
		{
			"\\rcrd REV 01.04-01.08",
			"\\s ",
			"\\p ",
			"\\v 4",
			"\\vt ",
			"\\p ",
			"\\vt |fn ",
			"\\btvt |fn ",
			"\\ft ",
			"\\cf 1:4: La'o sai hosi Mesir 3:14, Rai-klaran Foun 4:5",
			"\\v 5",
			"\\p ",
			"\\cf 1:5: Yesaya 55:4, Kananuk sia 89:27",
			"\\v 6",
			"\\cf 1:6: La'o sai hosi Mesir 19:6, Rai-klaran Foun 5:10",
			"\\p ",
			"\\v 7",
			"\\cf 1:7: Daniel 7:13, Santo Mateus 24:30, Santo Markus 13:26, Santo Lukas 21:27, 1 Tesalonika 4:17, Sakarias 12:10, Santo Yohanis 19:34, 37",
			"\\p ",
			"\\v 8",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\q ",
			"\\q2 ",
			"\\vt ",
			"\\cf 1:8: Rai-klaran Foun 22:13, La'o sai hosi Mesir 3:14"
		};
        #endregion
        #region TestData #8 - HelongRev0104_Cannonical
        static public string[] HelongRev0104_Cannonical = new string[] 
		{
            "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd MRK",
		    "\\mt",
            "\\id MRK",
			"\\rcrd REV 01.04-01.08",
			"\\s",
			"\\p",
			"\\v 4",
			"\\p",
            "\\vt |fn", 
            "\\btvt |fn",
			"\\ft",
			"\\cf 1:4: La'o sai hosi Mesir 3:14, Rai-klaran Foun 4:5",
			"\\v 5",
			"\\p",
			"\\cf 1:5: Yesaya 55:4, Kananuk sia 89:27",
			"\\v 6",
			"\\cf 1:6: La'o sai hosi Mesir 19:6, Rai-klaran Foun 5:10",
			"\\p",
			"\\v 7",
			"\\cf 1:7: Daniel 7:13, Santo Mateus 24:30, Santo Markus 13:26, Santo Lukas 21:27, 1 Tesalonika 4:17, Sakarias 12:10, Santo Yohanis 19:34, 37",
			"\\p",
			"\\v 8",
			"\\q",
			"\\q2",
			"\\q",
			"\\q2",
			"\\q",
			"\\q2",
			"\\q",
			"\\q2",
			"\\q",
			"\\q2",
			"\\cf 1:8: Rai-klaran Foun 22:13, La'o sai hosi Mesir 3:14"
		};
        #endregion
        #region TestData #8 - HelongRev0104_oxes
        static public string[] HelongRev0104_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Section Head\" usfm=\"s1\" />",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"4\" />",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<note class=\"Note General Paragraph\" usfm=\"f\" />",
                    "<note reference=\"1:4\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">",
                        "La'o sai hosi Mesir 3:14, Rai-klaran Foun 4:5",
                    "</note>",
                    "<v n=\"5\" />",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<note reference=\"1:5\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">",
                        "Yesaya 55:4, Kananuk sia 89:27",
                    "</note>",
                    "<v n=\"6\" />",
                    "<note reference=\"1:6\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">",
                        "La'o sai hosi Mesir 19:6, Rai-klaran Foun 5:10",
                    "</note>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"7\" />",
                    "<note reference=\"1:7\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">",
                        "Daniel 7:13, Santo Mateus 24:30, Santo Markus 13:26, Santo Lukas 21:27, 1 Tesalonika 4:17, Sakarias 12:10, Santo Yohanis 19:34, 37",
                    "</note>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"8\" />",
                "</p>",

                "<p class=\"Line 1\" usfm=\"q1\" />",
                "<p class=\"Line 2\" usfm=\"q2\" />",

                "<p class=\"Line 1\" usfm=\"q1\" />",
                "<p class=\"Line 2\" usfm=\"q2\" />",

                "<p class=\"Line 1\" usfm=\"q1\" />",
                "<p class=\"Line 2\" usfm=\"q2\" />",

                "<p class=\"Line 1\" usfm=\"q1\" />",
                "<p class=\"Line 2\" usfm=\"q2\" />",

                "<p class=\"Line 1\" usfm=\"q1\" />",
                "<p class=\"Line 2\" usfm=\"q2\">",
                    "<note reference=\"1:8\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">",
                        "Rai-klaran Foun 22:13, La'o sai hosi Mesir 3:14",
                    "</note>",
                "</p>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 9. Manado Mark 13:14 (two footnotes)
        #region TestData #9 - ManadoMark013014_ImportVariant
        static public string[] ManadoMark013014_ImportVariant = new string[] 
		{
			"\\rcrd mrk13.14-23",
			"\\s Yesus kase tau samua tu mo jadi di hari-hari siksa",
			"\\r (Matius 24:15-28; Lukas 21:20-24)",
			"\\p ",
			"\\v 14",
			"\\vt Yesus bilang, \"Satu orang yang paling jaha|fn mo datang. ",
			"\\ft 13:14a: Tu orang ini Allah pe musu [lia Daniel 9:27; 11:31; 12:11].",
			"\\vt Orang ini mo badiri di tampa yang nyanda cocok for dia.|fn (Sapa " +
			"yang baca ini, taru kira akang bae-bae). Kong kalu ngoni lia tu orang " +
			"itu so badiri di tampa itu, lebe bae orang-orang yang ada di Yudea " +
			"manyingkir jo ka gunung.",
			"\\ft 13:14b Tu tampa itu Ruma Ibada Pusat [lia Matius 24:15].",
			"\\nt Usulan dari pak Michael: Yesus bilang, \"Satu orang yang paling " +
			"jaha mo datang. ",
			"\\cf 13:14: *Daniel 9:27, 11:31, 12:11*",
			"\\p ",
			"\\v 15",
			"\\vt Tu orang yang ada di atas ruma, jang pi ambe tu barang di dalam " +
			"ruma, mar turung kong lari jo."
		};
        #endregion
        #region TestData #9 - ManadoMark013014_Cannonical
        static public string[] ManadoMark013014_Cannonical = new string[] 
		{
            "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd MRK",
		    "\\mt",
            "\\id MRK",
			"\\rcrd mrk13.14-23",
			"\\s Yesus kase tau samua tu mo jadi di hari-hari siksa",
			"\\r (Matius 24:15-28; Lukas 21:20-24)",
			"\\p",
			"\\v 14",
			"\\vt Yesus bilang, \"Satu orang yang paling jaha|fn",
            "\\btvt |fn",
			"\\ft 13:14a: Tu orang ini Allah pe musu [lia Daniel 9:27; 11:31; 12:11].",
            "\\btft 13:14a:",
			"\\vt mo datang. Orang ini mo badiri di tampa yang nyanda cocok for dia.|fn",
            "\\btvt |fn",
			"\\ft 13:14b: Tu tampa itu Ruma Ibada Pusat [lia Matius 24:15].",
            "\\btft 13:14b:",
            "\\vt (Sapa yang baca ini, taru kira akang bae-bae). Kong kalu ngoni lia tu orang " +
			    "itu so badiri di tampa itu, lebe bae orang-orang yang ada di Yudea " +
			    "manyingkir jo ka gunung.",
			"\\nt Usulan dari pak Michael: Yesus bilang, \"Satu orang yang paling " +
			    "jaha mo datang.",
			"\\cf 13:14: Daniel 9:27, 11:31, 12:11",
			"\\p",
			"\\v 15",
			"\\vt Tu orang yang ada di atas ruma, jang pi ambe tu barang di dalam " +
			    "ruma, mar turung kong lari jo."
		};
        #endregion
        #region TestData #9 - ManadoMark013014_oxes
        static public string[] ManadoMark013014_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Yesus kase tau samua tu mo jadi di hari-hari siksa",
                "</p>",

                "<p class=\"Parallel Passage Reference\" usfm=\"r\">",
                    "(Matius 24:15-28; Lukas 21:20-24)",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"14\" />",
                    "Yesus bilang, &quot;Satu orang yang paling jaha",
                    "<note reference=\"13:14a\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "Tu orang ini Allah pe musu [lia Daniel 9:27; 11:31; 12:11].",
                    "</note>",
                    "mo datang. Orang ini mo badiri di tampa yang nyanda cocok for dia.",
                    "<note reference=\"13:14b\" class=\"Note General Paragraph\" usfm=\"f\">",
                        "Tu tampa itu Ruma Ibada Pusat [lia Matius 24:15].",
                    "</note>",
                    "(Sapa yang baca ini, taru kira akang bae-bae). Kong kalu ngoni lia tu orang itu so badiri di tampa itu, lebe bae orang-orang yang ada di Yudea manyingkir jo ka gunung.",
                    "<Annotation class=\"General\" selectedText=\"Usulan dari pak Michael: Yesus bilang,\">",
                        "<Message author=\"Unknown Author\" created=\"2009-09-18 02:13:00Z\">Usulan dari pak Michael: Yesus bilang, \"Satu orang yang paling jaha mo datang.</Message>",
                    "</Annotation>",
                    "<note reference=\"13:14\" class=\"Note Cross Reference Paragraph\" usfm=\"x\">Daniel 9:27, 11:31, 12:11</note>",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"15\" />",
                    "Tu orang yang ada di atas ruma, jang pi ambe tu barang di dalam ruma, mar turung kong lari jo.",
                "</p>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 10. Tombulu Acts 9:32-35 (empty section)
        #region TestData #10 - TombuluActs009032_ImportVariant
        static public string[] TombuluActs009032_ImportVariant = new string[] 
		{
			"\\rcrd act 09.32-35",
			"\\s Si Enéas é liné'os ni Petrus",
			"\\p ",
			"\\v 32",
			"\\vt",
			"\\v 33",
			"\\vt",
			"\\p ",
			"\\v 34",
			"\\vt",
			"\\v 35",
			"\\vt",
			"\\ud 12/Apr/2006"
		};
        #endregion
        #region TestData #10 - TombuluActs009032_Cannonical
        static public string[] TombuluActs009032_Cannonical = new string[] 
		{
            "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd MRK",
		    "\\mt",
            "\\id MRK",
			"\\rcrd act 09.32-35",
			"\\s Si Enéas é liné'os ni Petrus",
			"\\p",
			"\\v 32",
			"\\v 33",
			"\\p",
			"\\v 34",
			"\\v 35",
			"\\ud 12/Apr/2006"
		};
        #endregion
        #region TestData #10 - TombuluActs009032_oxes
        static public string[] TombuluActs009032_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Si Enéas é liné'os ni Petrus",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"32\" />",
                    "<v n=\"33\" />",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"34\" />",
                    "<v n=\"35\" />",
                "</p>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 11. English (\mr, \ms)
        #region TestData #11 - English_ImportVariant
        static public string[] English_ImportVariant = new string[] 
        {
            "\\rcrd JER 001",
            "\\ms Judah in Trouble",
            "\\mr (Psalm 123:5)",
            "\\s Jeremiah's Prayer",
            "\\p",
            "\\v 1",
            "\\vt I know, Lord, that our lives are not our own.",
		    "\\p",
		    "\\v 2",
		    "\\vt The Lord gave another message to Jeremiah."
       };
        #endregion
        #region TestData #11 - English_Cannonical
        static public string[] English_Cannonical = new string[] 
        {
            "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd MRK",
		    "\\mt",
            "\\id MRK",
            "\\rcrd JER 001",
            "\\ms Judah in Trouble",
            "\\mr (Psalm 123:5)",
            "\\s Jeremiah's Prayer",
            "\\p",
            "\\v 1",
            "\\vt I know, Lord, that our lives are not our own.",
		    "\\p",
		    "\\v 2",
		    "\\vt The Lord gave another message to Jeremiah."
       };
        #endregion
        #region TestData #11 - English_oxes
        static public string[] English_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Title Main\" usfm=\"mt\" />",

                "<p class=\"Major Section Head\" usfm=\"ms\">",
                    "Judah in Trouble",
                "</p>",

                "<p class=\"Major Section Range\" usfm=\"mr\">",
                    "(Psalm 123:5)",
                "</p>",

                "<p class=\"Section Head\" usfm=\"s1\">",
                    "Jeremiah's Prayer",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"1\" />",
                    "I know, Lord, that our lives are not our own.",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"2\" />",
                    "The Lord gave another message to Jeremiah.",
                "</p>",

                "</book>",
            "</bible>"
        };
        #endregion

        // 12. Djambarrpuyngu 2 John
        #region TestData #12 - Djambarrpungu_ImportVariant
        static public string[] Djambarrpungu_ImportVariant = new string[] 
        {
            "\\rcrd 2JN",
            "\\id 2JN",
            "\\h 2 DJONGUŊ DHÄWU (JOHN)",
            "\\mt DJONGUŊ DJORRA' DJUY'YUNNAWUY NAMBA 2",
            "\\ip Dhuwandja djorra' ŋurukuŋdhi bili yan Djonguŋ. Ga wukirriny ŋayi dhuwal yän ga ḻiyamirriyam ŋunhi ŋayi dhuwal märr-ŋamathinyawuynydja gämurru' ŋurruŋu mirithirr. Ga dhunupamirriyam ŋayi ga Garraywalaŋumirriwal yolŋuwal walalaŋgal walal dhu ga yuwalkkuman yan yuwalknhany dhäwuny mala lakaranhamirr, ga märr-ŋamathirra dhu ga yolŋuwnydja walalaŋ.",
            "\\ip Ga wiripuny limurr dhu ga märr-yuwalkthirr ŋunhi Djesuny dhuwal yuwalk yan yolŋu, bala limurrnydja dhu ga ŋunhi märr-ŋamathinyamirra bala-räli'yunmirra.",
            "\\p",
            "\\c 1",
            "\\v 1 ",
            "\\vt Yo, dhuwandja djorra' ŋarrakuŋ ŋunhi ŋarra waṉa-nhirrpanawuy djägamirr. Ŋarra nhumalaŋ ga dhuwal wukirriny Godkalaŋumirriwnha yan yolŋuwnydja walalaŋ, ŋunhi nhumany dhuwali balanya nhakun ŋunhi gänaŋ'maranhawuynha miyalk, ga djamarrkuḻin' nhanŋu ŋuriki miyalkkun. Yuwalk yan ŋarra nhumalaŋ ga dhuwal märr-ŋamathirrnydja, yurr yaka yan ŋarrapiny ŋuli gi dhuwal nhumalaŋ märr-ŋamathi; ŋunhiwurrnydja yolŋu walal bukmak yan ŋunhi walal marŋgi yuwalkku, walalnydja ga ŋunhi bitjandhi bili yan märr-ŋamathirryi nhumalaŋ,",
            "\\v 2 ",
            "\\vt bili limurrnydja ga dhuwal ŋayathaman ŋunhi God-Waŋarrwuŋuny yuwalktja rom, ga ŋunhiyiny ŋunhi yuwalktja dhu ga dhärran yan limurruŋgala bitjanna bili yan weyinŋumirra.",
            "\\v 3 ",
            "\\vt Yo. God-Waŋarryu ga nhanukal Gäthu'mirriŋuy Djesu-Christthuny, limurruny ŋuli ga ŋunhi gämany manymakkuman yan maṉḏaŋgiyingala märr-ŋamathinyaraynydja ga mel-wuyunaraynydja, märr limurr dhu ga ŋunhi nhinany mägayan yan. ga nhinany maṉḏa dhu ga ŋunhi bitjanna bili yan limurruŋgalnydja ŋayaŋuŋur romdhuny märr-ŋamathinyamirriynha ga yuwalkthun yan.",
            "\\p",
            "\\v 4 ",
            "\\vt Yo, mirithinan yan ŋarra ŋunhi ŋayaŋu-djulŋithinany, bili ŋarra ŋäma nhumalany ŋunhi wiripuwurrnydja nhuma ga dhuwali nhina ŋunhiliyin yuwalkŋura romŋur, bitjan yan bili nhakun ŋayi God-Waŋarryu limurruŋ rom nhirrpar.",
            "\\v 5 ",
            "\\vt Yo marrkapmirr ḻundu, Dhuwandja yaka ŋula yuṯany dhäruk-gurrupanminyawuy rom ŋarra nhumalaŋ gi gurrupul; ŋany ŋäthiliŋu yan dhuwal ŋunhi ŋayipi Garray dhäruk-rulwaŋdhunmin be ŋäthil. <<Gatjuy märr-ŋamathinyamirrnydja walal gi bala-räli'yunmirra yänan.>>",
            "\\p",
            "\\v 6 ",
            "\\vt Ga dhuwal ŋunhi märr-ŋamathinyawuynydja gam', limurr dhu marrtji ŋunhi malthundja ŋurikin bili yan Garraywalaŋawnha dhäruk-gurrupanminyawuywuny, ŋunhi ŋayi ŋäthil dhäruk-nhirrpanmin limurruŋ. Ga dhuwal ŋunhi romdja nhuma ŋäkul ŋäthilnydja gam', <<Yuwalkkuŋun walal gi yan märr-ŋamathinyamirrnydja bala-räli'yunmirrnydja biyakun bili yänan.>>",
            "\\v 7 ",
            "\\vt Bili dharrwan gan ŋunhi mayali'-gänhamirrnydja yolŋu walal bilin barrkuwatjthinan wäŋalila malaŋulil. Ga dhiyaŋiwurruyyin ŋuli ga ŋunhi mayali'-gänhamirriynha yolŋuy walal yakan dhunupany lakaraŋ Djesu-Christnhany; walalnydja ŋuli ga ŋunhi wiripuŋuyaman lakaramany bitjanna, yanbi dhuwal God-Waŋarrwuny Gäthu'mirriŋu yaka dhawal-guyaŋanha yolŋuwuŋuny guḻunbuy, ga yaka yanbi ŋayi ŋunhi yuwalktja yolŋuny yan rumbal marrtjinya dhipalnydja, bitjandhiyin walalnydja ŋuli ga ŋunhi ŋanya Garraynhany lakaram. Ga ŋuli ŋayi dhu ga ŋunhi ŋula yol yolŋu bitjandhiny waŋa , ŋunhiyin ŋayi ŋunhi Djesu-Christkuny Ŋayan'mirrnydja Miriŋu.",
            "\\v 8 ",
            "\\vt Biyakun djägan nhumalaŋguwuynydja nhuma gi. Ŋuli nhuma dhu ga ŋunhi buthuru-bitjun walalaŋgalaŋawnydja dhärukku ŋurikiwurruŋdhiny mayali'-gänhamirriwnydja yolŋuw walalaŋ, nhumany dhu ŋunhi yakan märram ŋula nhä ŋamakurr God-Waŋarrwuŋuny. Ŋayathulnydja gi baṯ biyakuny ŋunhin bili yan waŋganynha dhäruktja ŋunhi nhuma gan bili märraŋal ŋäthil ŋanapurruŋgalaŋuwurr djämakurr, märr nhuma dhu märram muka ŋula nhäny mala ŋamakurrnydja ŋunhany ŋunhi God-Waŋarrwuŋuny, ŋunhi ŋayi ga ŋayatham nhumalaŋ.",
            "\\p",
            "\\v 9 ",
            "\\vt Yo. Ŋuli ŋayi dhu ga ŋunhi yolŋu yakany nhakun nhina ŋunhiliyiny dhäwuŋur ŋunhi ŋayi gan ŋurruŋu marŋgithin Djesu-Christkalaŋuwuy ŋunhiny ŋayi ḏiltji-ḏuwaṯthurra ŋurikiyiny dhäwuw, ga bäyŋun ŋayi gi ŋunhi nhini rrambaŋiny waŋganyŋurnydja God-Waŋarrwalnydja. Ga ŋuli ŋayi dhu ga yolŋu nhina ŋunhiliyiny bili yan Djesu-Christkalnydja dhäwuŋur, ŋayin ga ŋunhi ŋunhiyin yolŋu yuwalktja nhina God-Waŋarrwalnydja manapan, ga Gäthu'mirriŋuwalnydja nhanukalaŋuwal Djesu-Christkalnydja.",
            "\\v 10 ",
            "\\vt Ga ŋuli balaŋ ŋayi dhu ŋula yol yolŋu marrtji nhumalaŋgal, bala ŋayi dhu ga ŋunhi marŋgikuman nhumalany, yurr nhanukuŋ dhu ga ŋunhi marŋgikunhawuynydja marrtji djarrpin', yakan nhanŋu gumurr-ŋamathi balanyarawyiny yolŋuw, nhä mak nhuma dhu märram ŋanya balany nhumalaŋgiyingalnydja wäŋalil, bäyŋun yan.",
            "\\v 11 ",
            "\\vt Ga ŋuli nhe dhu ŋunhi nhanŋu wäthun ŋayaŋuy gumurr-ŋamathinyaraynydja, ga ŋunhiyiny nhuma märrma'yirra djarrpin' yan maṉḏa; djarrpi'ŋura romŋur nhuma ga nhina.",
            "\\p",
            "\\v 12 ",
            "\\vt Dharrwa muka mirithirr ŋarra nhumalaŋ balaŋ ganha ŋunhi lakaranhany, yurr yakan ŋarra dhu dhuwal wukirriny dhipalnydja gay'yi djorra'lilnydja. Bili gatjpu'yuna ŋarra ga dhuwal marrtjinyarawnha dhipaliyin nhumalaŋgala, märr limurr dhu ga waŋganyŋura waŋanhamirr dhiyakiyiny malaŋuw, märr limurr dhu yuwalknha yan ŋayaŋu-djulŋithinyamirrnydja bala-räli'yunmirra yan.",
            "\\p",
            "\\v 13 ",
            "\\vt Ga dhuwal ŋunhi Garraywalaŋumirr mala wäŋa-dhuwalaŋuwuynydja, ŋunhi ŋarra ga nhina dhiyal wäŋaŋur, walalnydja ga dhuwal djuy'yundhi nhumalaŋ märr-ŋamathinyawuy. Ga balanya dhuwal dhäwuny' nhumalaŋ.",
            "\\e"
       };
        #endregion
        #region TestData #12 - DjambarrpunguCannonical
        static public string[] Djambarrpungu_Cannonical = new string[] 
        {
            "\\_sh v3.0 2 SHW-Scripture", 
		    "\\_DateStampHasFourDigitYear",
		    "\\rcrd MRK",
            "\\h 2 DJONGUŊ DHÄWU (JOHN)",
            "\\mt DJONGUŊ DJORRA' DJUY'YUNNAWUY NAMBA 2",
            "\\id 2JN",
            "\\rcrd 1",
            "\\p",
            "\\c 1",
            "\\v 1",
            "\\vt Yo, dhuwandja djorra' ŋarrakuŋ ŋunhi ŋarra waṉa-nhirrpanawuy djägamirr. Ŋarra nhumalaŋ ga dhuwal wukirriny Godkalaŋumirriwnha yan yolŋuwnydja walalaŋ, ŋunhi nhumany dhuwali balanya nhakun ŋunhi gänaŋ'maranhawuynha miyalk, ga djamarrkuḻin' nhanŋu ŋuriki miyalkkun. Yuwalk yan ŋarra nhumalaŋ ga dhuwal märr-ŋamathirrnydja, yurr yaka yan ŋarrapiny ŋuli gi dhuwal nhumalaŋ märr-ŋamathi; ŋunhiwurrnydja yolŋu walal bukmak yan ŋunhi walal marŋgi yuwalkku, walalnydja ga ŋunhi bitjandhi bili yan märr-ŋamathirryi nhumalaŋ,",
            "\\v 2",
            "\\vt bili limurrnydja ga dhuwal ŋayathaman ŋunhi God-Waŋarrwuŋuny yuwalktja rom, ga ŋunhiyiny ŋunhi yuwalktja dhu ga dhärran yan limurruŋgala bitjanna bili yan weyinŋumirra.",
            "\\v 3",
            "\\vt Yo. God-Waŋarryu ga nhanukal Gäthu'mirriŋuy Djesu-Christthuny, limurruny ŋuli ga ŋunhi gämany manymakkuman yan maṉḏaŋgiyingala märr-ŋamathinyaraynydja ga mel-wuyunaraynydja, märr limurr dhu ga ŋunhi nhinany mägayan yan. ga nhinany maṉḏa dhu ga ŋunhi bitjanna bili yan limurruŋgalnydja ŋayaŋuŋur romdhuny märr-ŋamathinyamirriynha ga yuwalkthun yan.",
            "\\p",
            "\\v 4",
            "\\vt Yo, mirithinan yan ŋarra ŋunhi ŋayaŋu-djulŋithinany, bili ŋarra ŋäma nhumalany ŋunhi wiripuwurrnydja nhuma ga dhuwali nhina ŋunhiliyin yuwalkŋura romŋur, bitjan yan bili nhakun ŋayi God-Waŋarryu limurruŋ rom nhirrpar.",
            "\\v 5",
            "\\vt Yo marrkapmirr ḻundu, Dhuwandja yaka ŋula yuṯany dhäruk-gurrupanminyawuy rom ŋarra nhumalaŋ gi gurrupul; ŋany ŋäthiliŋu yan dhuwal ŋunhi ŋayipi Garray dhäruk-rulwaŋdhunmin be ŋäthil. <<Gatjuy märr-ŋamathinyamirrnydja walal gi bala-räli'yunmirra yänan.>>",
            "\\p",
            "\\v 6",
            "\\vt Ga dhuwal ŋunhi märr-ŋamathinyawuynydja gam', limurr dhu marrtji ŋunhi malthundja ŋurikin bili yan Garraywalaŋawnha dhäruk-gurrupanminyawuywuny, ŋunhi ŋayi ŋäthil dhäruk-nhirrpanmin limurruŋ. Ga dhuwal ŋunhi romdja nhuma ŋäkul ŋäthilnydja gam', <<Yuwalkkuŋun walal gi yan märr-ŋamathinyamirrnydja bala-räli'yunmirrnydja biyakun bili yänan.>>",
            "\\v 7",
            "\\vt Bili dharrwan gan ŋunhi mayali'-gänhamirrnydja yolŋu walal bilin barrkuwatjthinan wäŋalila malaŋulil. Ga dhiyaŋiwurruyyin ŋuli ga ŋunhi mayali'-gänhamirriynha yolŋuy walal yakan dhunupany lakaraŋ Djesu-Christnhany; walalnydja ŋuli ga ŋunhi wiripuŋuyaman lakaramany bitjanna, yanbi dhuwal God-Waŋarrwuny Gäthu'mirriŋu yaka dhawal-guyaŋanha yolŋuwuŋuny guḻunbuy, ga yaka yanbi ŋayi ŋunhi yuwalktja yolŋuny yan rumbal marrtjinya dhipalnydja, bitjandhiyin walalnydja ŋuli ga ŋunhi ŋanya Garraynhany lakaram. Ga ŋuli ŋayi dhu ga ŋunhi ŋula yol yolŋu bitjandhiny waŋa , ŋunhiyin ŋayi ŋunhi Djesu-Christkuny Ŋayan'mirrnydja Miriŋu.",
            "\\v 8",
            "\\vt Biyakun djägan nhumalaŋguwuynydja nhuma gi. Ŋuli nhuma dhu ga ŋunhi buthuru-bitjun walalaŋgalaŋawnydja dhärukku ŋurikiwurruŋdhiny mayali'-gänhamirriwnydja yolŋuw walalaŋ, nhumany dhu ŋunhi yakan märram ŋula nhä ŋamakurr God-Waŋarrwuŋuny. Ŋayathulnydja gi baṯ biyakuny ŋunhin bili yan waŋganynha dhäruktja ŋunhi nhuma gan bili märraŋal ŋäthil ŋanapurruŋgalaŋuwurr djämakurr, märr nhuma dhu märram muka ŋula nhäny mala ŋamakurrnydja ŋunhany ŋunhi God-Waŋarrwuŋuny, ŋunhi ŋayi ga ŋayatham nhumalaŋ.",
            "\\p",
            "\\v 9",
            "\\vt Yo. Ŋuli ŋayi dhu ga ŋunhi yolŋu yakany nhakun nhina ŋunhiliyiny dhäwuŋur ŋunhi ŋayi gan ŋurruŋu marŋgithin Djesu-Christkalaŋuwuy ŋunhiny ŋayi ḏiltji-ḏuwaṯthurra ŋurikiyiny dhäwuw, ga bäyŋun ŋayi gi ŋunhi nhini rrambaŋiny waŋganyŋurnydja God-Waŋarrwalnydja. Ga ŋuli ŋayi dhu ga yolŋu nhina ŋunhiliyiny bili yan Djesu-Christkalnydja dhäwuŋur, ŋayin ga ŋunhi ŋunhiyin yolŋu yuwalktja nhina God-Waŋarrwalnydja manapan, ga Gäthu'mirriŋuwalnydja nhanukalaŋuwal Djesu-Christkalnydja.",
            "\\v 10",
            "\\vt Ga ŋuli balaŋ ŋayi dhu ŋula yol yolŋu marrtji nhumalaŋgal, bala ŋayi dhu ga ŋunhi marŋgikuman nhumalany, yurr nhanukuŋ dhu ga ŋunhi marŋgikunhawuynydja marrtji djarrpin', yakan nhanŋu gumurr-ŋamathi balanyarawyiny yolŋuw, nhä mak nhuma dhu märram ŋanya balany nhumalaŋgiyingalnydja wäŋalil, bäyŋun yan.",
            "\\v 11",
            "\\vt Ga ŋuli nhe dhu ŋunhi nhanŋu wäthun ŋayaŋuy gumurr-ŋamathinyaraynydja, ga ŋunhiyiny nhuma märrma'yirra djarrpin' yan maṉḏa; djarrpi'ŋura romŋur nhuma ga nhina.",
            "\\p",
            "\\v 12",
            "\\vt Dharrwa muka mirithirr ŋarra nhumalaŋ balaŋ ganha ŋunhi lakaranhany, yurr yakan ŋarra dhu dhuwal wukirriny dhipalnydja gay'yi djorra'lilnydja. Bili gatjpu'yuna ŋarra ga dhuwal marrtjinyarawnha dhipaliyin nhumalaŋgala, märr limurr dhu ga waŋganyŋura waŋanhamirr dhiyakiyiny malaŋuw, märr limurr dhu yuwalknha yan ŋayaŋu-djulŋithinyamirrnydja bala-räli'yunmirra yan.",
            "\\p",
            "\\v 13",
            "\\vt Ga dhuwal ŋunhi Garraywalaŋumirr mala wäŋa-dhuwalaŋuwuynydja, ŋunhi ŋarra ga nhina dhiyal wäŋaŋur, walalnydja ga dhuwal djuy'yundhi nhumalaŋ märr-ŋamathinyawuy. Ga balanya dhuwal dhäwuny' nhumalaŋ.",
       };
        #endregion
        #region TestData #12 - Djambarrpungu_oxes
        static public string[] Djambarrpungu_oxes = new string[]
        {
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
            "<bible xml:lang=\"bko\" backtTranslaltionDefaultLanguage=\"en\" oxes=\"2.0\">",
            "<book id=\"MRK\" stage=\"Draft\" version=\"A\">",

                "<p class=\"Header\" usfm=\"h\">",
                    "2 DJONGUŊ DHÄWU (JOHN)",
                "</p>",

                "<p class=\"Title Main\" usfm=\"mt\">",
                    "DJONGUŊ DJORRA' DJUY'YUNNAWUY NAMBA 2",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<c n=\"1\" />",
                    "<v n=\"1\" />",
                    "Yo, dhuwandja djorra' ŋarrakuŋ ŋunhi ŋarra waṉa-nhirrpanawuy djägamirr. Ŋarra nhumalaŋ ga dhuwal wukirriny Godkalaŋumirriwnha yan yolŋuwnydja walalaŋ, ŋunhi nhumany dhuwali balanya nhakun ŋunhi gänaŋ'maranhawuynha miyalk, ga djamarrkuḻin' nhanŋu ŋuriki miyalkkun. Yuwalk yan ŋarra nhumalaŋ ga dhuwal märr-ŋamathirrnydja, yurr yaka yan ŋarrapiny ŋuli gi dhuwal nhumalaŋ märr-ŋamathi; ŋunhiwurrnydja yolŋu walal bukmak yan ŋunhi walal marŋgi yuwalkku, walalnydja ga ŋunhi bitjandhi bili yan märr-ŋamathirryi nhumalaŋ,",
                    "<v n=\"2\" />",
                    "bili limurrnydja ga dhuwal ŋayathaman ŋunhi God-Waŋarrwuŋuny yuwalktja rom, ga ŋunhiyiny ŋunhi yuwalktja dhu ga dhärran yan limurruŋgala bitjanna bili yan weyinŋumirra.",
                    "<v n=\"3\" />",
                    "Yo. God-Waŋarryu ga nhanukal Gäthu'mirriŋuy Djesu-Christthuny, limurruny ŋuli ga ŋunhi gämany manymakkuman yan maṉḏaŋgiyingala märr-ŋamathinyaraynydja ga mel-wuyunaraynydja, märr limurr dhu ga ŋunhi nhinany mägayan yan. ga nhinany maṉḏa dhu ga ŋunhi bitjanna bili yan limurruŋgalnydja ŋayaŋuŋur romdhuny märr-ŋamathinyamirriynha ga yuwalkthun yan.",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"4\" />",
                    "Yo, mirithinan yan ŋarra ŋunhi ŋayaŋu-djulŋithinany, bili ŋarra ŋäma nhumalany ŋunhi wiripuwurrnydja nhuma ga dhuwali nhina ŋunhiliyin yuwalkŋura romŋur, bitjan yan bili nhakun ŋayi God-Waŋarryu limurruŋ rom nhirrpar.",
                    "<v n=\"5\" />",
                    "Yo marrkapmirr ḻundu, Dhuwandja yaka ŋula yuṯany dhäruk-gurrupanminyawuy rom ŋarra nhumalaŋ gi gurrupul; ŋany ŋäthiliŋu yan dhuwal ŋunhi ŋayipi Garray dhäruk-rulwaŋdhunmin be ŋäthil. &lt;&lt;Gatjuy märr-ŋamathinyamirrnydja walal gi bala-räli'yunmirra yänan.&gt;&gt;",
                "</p>",

                 "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"6\" />",
                    "Ga dhuwal ŋunhi märr-ŋamathinyawuynydja gam', limurr dhu marrtji ŋunhi malthundja ŋurikin bili yan Garraywalaŋawnha dhäruk-gurrupanminyawuywuny, ŋunhi ŋayi ŋäthil dhäruk-nhirrpanmin limurruŋ. Ga dhuwal ŋunhi romdja nhuma ŋäkul ŋäthilnydja gam', &lt;&lt;Yuwalkkuŋun walal gi yan märr-ŋamathinyamirrnydja bala-räli'yunmirrnydja biyakun bili yänan.&gt;&gt;",
                    "<v n=\"7\" />",
                    "Bili dharrwan gan ŋunhi mayali'-gänhamirrnydja yolŋu walal bilin barrkuwatjthinan wäŋalila malaŋulil. Ga dhiyaŋiwurruyyin ŋuli ga ŋunhi mayali'-gänhamirriynha yolŋuy walal yakan dhunupany lakaraŋ Djesu-Christnhany; walalnydja ŋuli ga ŋunhi wiripuŋuyaman lakaramany bitjanna, yanbi dhuwal God-Waŋarrwuny Gäthu'mirriŋu yaka dhawal-guyaŋanha yolŋuwuŋuny guḻunbuy, ga yaka yanbi ŋayi ŋunhi yuwalktja yolŋuny yan rumbal marrtjinya dhipalnydja, bitjandhiyin walalnydja ŋuli ga ŋunhi ŋanya Garraynhany lakaram. Ga ŋuli ŋayi dhu ga ŋunhi ŋula yol yolŋu bitjandhiny waŋa , ŋunhiyin ŋayi ŋunhi Djesu-Christkuny Ŋayan'mirrnydja Miriŋu.",
                    "<v n=\"8\" />",
                    "Biyakun djägan nhumalaŋguwuynydja nhuma gi. Ŋuli nhuma dhu ga ŋunhi buthuru-bitjun walalaŋgalaŋawnydja dhärukku ŋurikiwurruŋdhiny mayali'-gänhamirriwnydja yolŋuw walalaŋ, nhumany dhu ŋunhi yakan märram ŋula nhä ŋamakurr God-Waŋarrwuŋuny. Ŋayathulnydja gi baṯ biyakuny ŋunhin bili yan waŋganynha dhäruktja ŋunhi nhuma gan bili märraŋal ŋäthil ŋanapurruŋgalaŋuwurr djämakurr, märr nhuma dhu märram muka ŋula nhäny mala ŋamakurrnydja ŋunhany ŋunhi God-Waŋarrwuŋuny, ŋunhi ŋayi ga ŋayatham nhumalaŋ.",
               "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"9\" />",
                    "Yo. Ŋuli ŋayi dhu ga ŋunhi yolŋu yakany nhakun nhina ŋunhiliyiny dhäwuŋur ŋunhi ŋayi gan ŋurruŋu marŋgithin Djesu-Christkalaŋuwuy ŋunhiny ŋayi ḏiltji-ḏuwaṯthurra ŋurikiyiny dhäwuw, ga bäyŋun ŋayi gi ŋunhi nhini rrambaŋiny waŋganyŋurnydja God-Waŋarrwalnydja. Ga ŋuli ŋayi dhu ga yolŋu nhina ŋunhiliyiny bili yan Djesu-Christkalnydja dhäwuŋur, ŋayin ga ŋunhi ŋunhiyin yolŋu yuwalktja nhina God-Waŋarrwalnydja manapan, ga Gäthu'mirriŋuwalnydja nhanukalaŋuwal Djesu-Christkalnydja.",
                    "<v n=\"10\" />",
                    "Ga ŋuli balaŋ ŋayi dhu ŋula yol yolŋu marrtji nhumalaŋgal, bala ŋayi dhu ga ŋunhi marŋgikuman nhumalany, yurr nhanukuŋ dhu ga ŋunhi marŋgikunhawuynydja marrtji djarrpin', yakan nhanŋu gumurr-ŋamathi balanyarawyiny yolŋuw, nhä mak nhuma dhu märram ŋanya balany nhumalaŋgiyingalnydja wäŋalil, bäyŋun yan.",
                    "<v n=\"11\" />",
                    "Ga ŋuli nhe dhu ŋunhi nhanŋu wäthun ŋayaŋuy gumurr-ŋamathinyaraynydja, ga ŋunhiyiny nhuma märrma'yirra djarrpin' yan maṉḏa; djarrpi'ŋura romŋur nhuma ga nhina.",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"12\" />",
                    "Dharrwa muka mirithirr ŋarra nhumalaŋ balaŋ ganha ŋunhi lakaranhany, yurr yakan ŋarra dhu dhuwal wukirriny dhipalnydja gay'yi djorra'lilnydja. Bili gatjpu'yuna ŋarra ga dhuwal marrtjinyarawnha dhipaliyin nhumalaŋgala, märr limurr dhu ga waŋganyŋura waŋanhamirr dhiyakiyiny malaŋuw, märr limurr dhu yuwalknha yan ŋayaŋu-djulŋithinyamirrnydja bala-räli'yunmirra yan.",
                "</p>",

                "<p class=\"Paragraph\" usfm=\"p\">",
                    "<v n=\"13\" />",
                    "Ga dhuwal ŋunhi Garraywalaŋumirr mala wäŋa-dhuwalaŋuwuynydja, ŋunhi ŋarra ga nhina dhiyal wäŋaŋur, walalnydja ga dhuwal djuy'yundhi nhumalaŋ märr-ŋamathinyawuy. Ga balanya dhuwal dhäwuny' nhumalaŋ.",
                "</p>",

               "</book>",
            "</bible>"
        };
        #endregion

        // Merge
        #region TestData: MergeActs004001
        static public string[] MergeActs004001 = new string[] 
		    {
			    "\\_sh v3.0 2 SHW-Scripture",     // 0
			    "\\_DateStampHasFourDigitYear",   // 1
			    "\\rcrd MRK",                     // 2
			    "\\h Mark",                       // 3
			    "\\st The Gospel Of",             // 4
			    "\\mt Mark",                      // 5
			    "\\id Mark",                      // 6
			    "\\rcrd act4.1-4.22",             // 7
			    "\\c 4",                          // 8
			    "\\s Tulu-tulu agama las haman Petrus nol Yohanis maas tala",
			    "\\bts The heads of religion summon Petrus and Yohanis to come appear.before [them]",
			    "\\p",                            // 11
			    "\\v 1",                          // 12
			    "\\vt Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, " +
				    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				    "agama Saduki. Oen maas komali le ahan Petrus nol Yohanis.",
			    "\\btvt At that time, Petrus and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Petrus and Yohanis.",
			    "\\v 2",                          // 15
			    "\\vt Oen komali lole Petrus nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Petrus and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)|fn",
			    "\\ft 4:1-2: Atuil deng partai agama Saduki, oen sium in tui na lo " +
				    "man noen atuil mate haup in nuli pait.",
			    "\\btft 4:1-2: People from the religious party Saduki, they did not accept that " +
				    "teaching that says dead people can live again.",
			    "\\v 3",                          // 20
			    "\\vt Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
				    "deng lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
				    "Le ola ka halas-sam, oen nehan dais na.",
			    "\\btvt And so they ordered their people to go capture the two of them. But " +
				    "because the sun already wanted to set, then they pushed [&] entered " +
				    "the two of them in jail. So.that the next day, they could take.care.of " +
				    "that affair/litigation/problem.",
			    "\\p",                            // 23
			    "\\v 4",                          // 24
			    "\\vt Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their people increased a lot " +
				    "approximately to five thousand people."
		    };
        #endregion

        // GoBible
        #region TestData - GoBible_Import1
        static public string[] GoBible_Import1 = new string[] 
        {
            "\\_sh v3.0 96 SHW-Scripture",
            "\\_DateStampHasFourDigitYear",

            "\\rcrd MRK",
            "\\h Markus",
            "\\st Tuhan Yesus pung Carita Bae,",
            "\\btst Lord Yesus' *Gospel1 (Good Story)",
            "\\st iko",
            "\\btst according to",
            "\\mt Markus",
            "\\btmt Markus",
            "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia; ISO code mkn-IDN",

            "\\rcrd MRK 1.1-1.8",
            "\\p",
            "\\c 1",
            "\\v 1",
            "\\vt Ini carita bae. Ini, Tuhan Allah pung Ana|fn",
            "\\btvt This is a good story. It is the story of God's Child.",
            "\\ft 1:1: Kata <Tuhan Allah pung Ana> sonde ada di dalam tulisan asli saparu.",
            "\\btft 1:1: The words 'God's child' are not in some original writings.",
            "\\vt pung carita. Dia pung nama Yesus Kristus, yang Tuhan Allah su tunju memang dari dolu. Dia pung carita mulai bagini:",
            "\\btvt His name was Yesus Kristus, whom God had already appointed from long ago. His story begins like this:",
            "\\s Yohanis Tukang Sarani buka jalan kasi Tuhan Yesus",
            "\\bts Yohanis, the.One.who.habitually *Baptizes, opens the way/path for the Lord Yesus",
            "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
            "\\p",
            "\\v 2",
            "\\vt Yesus balóm mulai Dia pung karjá, te ||iTuhan Allah||r su utus satu orang, nama Yohanis. Yohanis musti pi sadia jalan kasi Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung jubir, nama ba'i Yesaya. Dia su tulis memang, bilang:",
            "\\btvt Yesus had not yet begun His work, {te} God had ordered (to do s.t.) a person, named Yohanis. Yohanis had to go prepare the way for Yesus' coming. Cuz (elaboration) long before, God had already used His *prophet1 (spokesperson), named was grandfather/forefather Yesaya. He wrote beforehand (= wrote before the event), saying:",
            "\\q", 
            "\\vt <<Dengar, ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
            "\\btvt <<Listen! I am ordering/commanding a person of Mine, to go open the way for You.",
            "\\cf 1:2: Maleaki 3:1",
            "\\q", 
            "\\v 3",
            "\\vt Itu orang nanti pi di tampa yang sonde ada orang, ko batarea, bilang:",
            "\\btvt That person will go to a place where there are no people, to shout, saying:",
            "\\q2", 
            "\\vt <Samua orang siap-siap bekin bae jalan, ko tarima Tuhan pung datang!",
            "\\btvt <All people prepare to make good/fix the way/path, to receive the Lord/Master's coming!",
            "\\q2", 
            "\\vt Bekin lurus jalan ko sambut sang Dia.>>>",
            "\\btvt Make straight the path to greet Him.>>> [NOTE: ‘straight’ has both physical and moral overtones.]",
            "\\ov Bekin rata jalan ko sambut sang Dia.",
            "\\cf 1:3: Yesaya 40:3",
            "\\p", 
            "\\v 4-6",
            "\\vt Orang biasa pange sang Yohanis, Tukang Sarani. Dia tenga di tampa sunyi. Dia pung pakean bekin dari onta pung bulu. Dia pung ika pinggang bekin dari binatang pung kulit. Dia pung makanan, kalamak deng madu utan. [Yohanis pung cara idop ni, sama ke ba'i Elia dolu-dolu.]",
            "\\btvt People usually called Yohanis, One.who.habitually *Baptizes. He lived in a lonely place (=no people around). His clothes were made from camel's (loan:onta) fur. His belt was made from animal skin. His food, grasshoppers and wild honey. [This way of life of Yohanis, was like Grandfather Elia long ago.] [EXEG: High-level book-level theme from 1:1 (Jesus is the Christ; Elijah will come before the Christ comes) that was obvious to a reasonaly informed Jewish audience, but is not accessible or retrieveable by Timor audiences.]",
            "\\cf 1:6: 2 Raja-raja dong 1:8",
            "\\p", 
            "\\vt Itu waktu, banya orang dari kota Yerusalem, deng propinsi Yudea pung isi samua, datang katumu deng Yohanis di dia pung tampa tu. Dong datang ko mau lia sang Yohanis deng mau dengar dia pung ajaran. Yohanis kasi tau sang dong, bilang, <<Bosong musti mangaku deng kasi tenga bosong pung sala samua, ko biar Tuhan Allah hapus buang itu sala dong. Ais bosong musti sarani dolo, ko jadi tanda, bilang, bosong su babae deng Tuhan.>>",
            "\\btvt At that time, many people from Yerusalem city (loan: kota), and all the contents of Yudea province (loan:propinsi), came to meet with Yohanis at that place of his. They came because they wanted to see Yohanis and wanted to hear his teaching. He told them, saying, <<You-pl must *confess and leave behind all your *wrongs, in.order.that *God *forgive1 (erase throw.away) those *wrongs. Then you-pl must get *baptized, as a sign that you are already again RECIP-good (=in a good relationship) with the *Lord.>>",
            "\\nt mangaku sala has element of both confess and stop doing; po'a buang (focus on inside); angka buang",
            "\\ntgk pantes: not necessarily every last person, but lots, by a hyperbole common in Greek. Could be \"plenny plenny,\" but \"all\" is OK.",
            "\\cat c:\\graphics\\HK-LB\\LB00296C.tif",
            "\\ref width:5.0cm;textWrapping:around;horizontalPosition:right",
            "\\cap Yohanis, Tukang Sarani",
            "\\btcap Yohanis, The Habitual Baptizer",
            "\\p", 
            "\\vt Abis dong mangaku sala, ju dia sarani sang dong di kali Yarden.",
            "\\btvt After they *confessed their *sins, then he *baptised them in the Yarden stream/river.",
            "\\nt calop ko sarani ; sunge (implies always water) vs. kali (can have water or be just the bed) > kali mati and parigi mati = dry",
            "\\p", 
            "\\v 7",
            "\\vt Dia kasi tau, bilang, <<Nanti ada satu Orang yang lebe hebat dari beta mau datang. Biar cuma jadi Dia pung tukang suru-suru sa ju, beta sonde pantas.|fn",
            "\\btvt He told them saying like.this, <<Later there is a Person who is more noteworthy than me going to come. Even to just be his *slave (person who is habitually ordered about), I'm not fitting/appropriate.",
            "\\ft 1:7: Tulisan bahasa Yunani asli tulis, bilang, <<tondo ko buka dia pung tali sapatu sa ju beta sonde pantas.>> Dia pung arti, bilang, Yohanis cuma orang kici sa, biar jadi Tuhan Yesus pung tukang suru-suru sa ju, dia sonde pantas.",
            "\\btft 1:7: The original Greek language writings, says, 'to just bow.down in.order.to undo his shoe strap, it's not fitting/appropriate for me.' It's meaning is Yohanis was just a little person (social status), even.if {biar} he just became Lord Yesus' *slave/servant, he wasn't fitting/appropriate.",
            "\\v 8",
            "\\vt Beta cuma bisa sarani sang bosong pake aer sa, ma nanti Dia bekin lebe dari beta, te Dia bekin ponu bosong pung hati deng Tuhan pung Roh yang Barisi.>>",
            "\\btvt I'm only able to *baptize you with water, but later He will do more than me, cuz (elaboration) He will make your(pl) hearts(livers) full with *Lord's *Holy Spirit (=clean, free from impurities, undefiled, holy).>>",
            "\\nt rasuk: evil spirits control supernaturally, but negative or destructive associations; topang (BI) = senda (KM) -support; setan naik; takana setan; Rote/Timor-sumanak/smanan; baras barisi/ hati barisi",
            "\\ntgk Collocation of \"baptize\" and \"Spirit\" is opaque; spelled out here.",
            "\\ud 18/Feb/2009"
       };
        #endregion
        #region TestData - GoBible_Export1
        static public string[] GoBible_Export1 = new string[] 
        {
            "\\id MRK",
            "\\h Markus",
            "\\mt Markus",
            "\\c 1",
            "\\v 1 Ini carita bae. Ini, Tuhan Allah pung Ana pung carita. Dia pung nama Yesus Kristus, yang Tuhan Allah su tunju memang dari dolu. Dia pung carita mulai bagini:",
            "\\v 2 \\wj Yohanis Tukang Sarani buka jalan kasi Tuhan Yesus\\wj* Yesus balóm mulai Dia pung karjá, te Tuhan Allah su utus satu orang, nama Yohanis. Yohanis musti pi sadia jalan kasi Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung jubir, nama ba'i Yesaya. Dia su tulis memang, bilang:",
            "“Dengar, ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
            "\\v 3 Itu orang nanti pi di tampa yang sonde ada orang, ko batarea, bilang:",
            "‘Samua orang siap-siap bekin bae jalan, ko tarima Tuhan pung datang!",
            "Bekin lurus jalan ko sambut sang Dia.’”",
            "\\v 4 (4-6) Orang biasa pange sang Yohanis, Tukang Sarani. Dia tenga di tampa sunyi. Dia pung pakean bekin dari onta pung bulu. Dia pung ika pinggang bekin dari binatang pung kulit. Dia pung makanan, kalamak deng madu utan. [Yohanis pung cara idop ni, sama ke ba'i Elia dolu-dolu.]",
            "Itu waktu, banya orang dari kota Yerusalem, deng propinsi Yudea pung isi samua, datang katumu deng Yohanis di dia pung tampa tu. Dong datang ko mau lia sang Yohanis deng mau dengar dia pung ajaran. Yohanis kasi tau sang dong, bilang, “Bosong musti mangaku deng kasi tenga bosong pung sala samua, ko biar Tuhan Allah hapus buang itu sala dong. Ais bosong musti sarani dolo, ko jadi tanda, bilang, bosong su babae deng Tuhan.”",
            "Abis dong mangaku sala, ju dia sarani sang dong di kali Yarden.",
            "\\v 5",
            "\\v 6",
            "\\v 7 Dia kasi tau, bilang, “Nanti ada satu Orang yang lebe hebat dari beta mau datang. Biar cuma jadi Dia pung tukang suru-suru sa ju, beta sonde pantas.",
            "\\v 8 Beta cuma bisa sarani sang bosong pake aer sa, ma nanti Dia bekin lebe dari beta, te Dia bekin ponu bosong pung hati deng Tuhan pung Roh yang Barisi.”"
        };
        #endregion

        // Manipulations ---------------------------------------------------------------------
        #region SMethod: DTestBook LoadIntoBook(string[] vs, DTranslation)
        static public DTestBook LoadIntoBook(string[] vs, DTranslation translation)
        {
            // Basename and Pathname
            string sBaseName = "ReadTestData.db";
            string sPathName = JWU.NUnit_TestFileFolder + Path.DirectorySeparatorChar + sBaseName;

            // Write the Sfm to file so we can read it
            TextWriter W = JWU.NUnit_OpenTextWriter(sBaseName);
            foreach (string s in vs)
                W.WriteLine(s);
            W.Close();

            // Create and load the book from what we just wrote out
            DTestBook Book = new DTestBook(sPathName);
            translation.AddBook(Book);
            Book.LoadFromStandardFormat(sPathName);

            return Book;
        }
        #endregion
        #region SMethod: string[] ReadSfmFromBookFile(DTestBook book)
        static public string[] ReadSfmFromBookFile(DTestBook book)
        {
            string sPath = book.StoragePath;
            TextReader tr = JW_Util.GetTextReader(ref sPath, "");

            SfDb DB = new SfDb();
            DB.Read(tr);
            tr.Close();

            string[] vs = DB.ExtractData();
            return vs;
        }
        #endregion

        #region SMethod: string RemoveCreatedFromNote(string s)
        static public string RemoveCreatedFromNote(string s)
            // Remove Created="2009-05-14 12:45:22Z"
        {
            if (!s.StartsWith("\\tn "))
                return s;

            while (true)
            {
                int i = s.IndexOf("created=");
                if (-1 == i)
                    return s;

                int count = 0;
                while (s[i + count] != 'Z')
                    ++count;
                count += 2;

                s = s.Remove(i, count);
            }
        }
        #endregion
        #region SMethod: bool AreSame(svsActual, vsExpected)
        static public bool AreSame(string[] vsActual, string[] vsExpected)
        {
            if (vsExpected.Length != vsActual.Length)
                return false;

            for (int k = 0; k < vsActual.Length; k++)
            {
                // For translator notes, we can't do a complete compare, because the datestamps
                // will have changed. 
                if (vsExpected[k].StartsWith("\\tn "))
                {
                    string sExpectedNote = RemoveCreatedFromNote(vsExpected[k]);
                    string sActualNote = RemoveCreatedFromNote(vsActual[k]);
                    if (sExpectedNote != sActualNote)
                        return false;
                }

                // All of the compares are just of the raw strings
                else if (vsExpected[k] != vsActual[k])
                    return false;
            }

            return true;
        }
        #endregion
        #region SMethod: void ConsoleOut_ShowDiffs(sTitle, vsActual, vsExpected)
        static public void ConsoleOut_ShowDiffs(string sTitle, string[] vsActual, string[] vsExpected)
        {
            Console.WriteLine("");
            Console.WriteLine("----- [" + sTitle + "] -----");

            if (AreSame(vsActual, vsExpected))
            {
                Console.Write("Sections are identical!");
                return;
            }

            // Show the differences in lengths
            Console.WriteLine("Lengths: " +
                "Expected=" + vsExpected.Length.ToString() +
                "    Actual=" + vsActual.Length.ToString());

            // Line-by-line
            for (int n = 0; n < vsActual.Length && n < vsExpected.Length; n++)
            {
                Console.WriteLine("");
                if (RemoveCreatedFromNote( vsActual[n] ) != RemoveCreatedFromNote( vsExpected[n]))
                    Console.WriteLine("--DIFFERENT--");
                Console.WriteLine("vExpected = {" + vsExpected[n] + "}");
                Console.WriteLine("vActual   = {" + vsActual[n] + "}");
            }

            // Whichever was longer
            if (vsActual.Length > vsExpected.Length)
            {
                Console.WriteLine("");
                Console.WriteLine("Actual was longer. Next line was:");
                Console.WriteLine(vsActual[vsExpected.Length]);
            }
            if (vsActual.Length < vsExpected.Length)
            {
                Console.WriteLine("");
                Console.WriteLine("Expected was longer. Next line was:");
                Console.WriteLine(vsExpected[vsActual.Length]);
            }
        }
        #endregion
        #region SMethod: void ConsoleOut_ShowDiffs(sTitle, DSection actual, DSection expected)
        static public void ConsoleOut_ShowDiffs(string sTitle, DSection actual, DSection expected)
        {
            string[] vsActual = SectionToVS(actual);
            string[] vsExpected = SectionToVS(expected);

            ConsoleOut_ShowDiffs(sTitle, vsActual, vsExpected);
        }
        #endregion
        #region SMethod: string[] SectionToVS(DSection)
        static public string[] SectionToVS(DSection section)
        {
            List<string> vs = new List<string>();

            foreach (DParagraph p in section.Paragraphs)
                vs.Add(p.DebugString);

            return vs.ToArray();
        }
        #endregion

        #region SMethod: string[] Clone(string[] vs)
        static public string[] Clone(string[] vs)
        {
            string[] vsNew = new string[vs.Length];
            for (int i = 0; i < vs.Length; i++)
                vsNew[i] = string.Copy(vs[i]);
            return vsNew;
        }
        #endregion
        #region SMethod: string[] Substitute(string[] vs, int iPos, string sNew)
        static public string[] Substitute(string[] vs, int iPos, string sNew)
        {
            string[] vsNew = Clone(vs);
            vsNew[iPos] = string.Copy(sNew);
            return vsNew;
        }
        #endregion
        #region SMethod: string[] Insert(string[] vs, int iPos, string sNew)
        static public string[] Insert(string[] vs, int iPos, string sNew)
        {
            string[] vsNew = new string[vs.Length + 1];

            for (int i = 0; i < iPos; i++)
                vsNew[i] = string.Copy(vs[i]);

            vsNew[iPos] = string.Copy(sNew);

            for (int i = iPos; i < vs.Length; i++)
                vsNew[i + 1] = vs[i];

            return vsNew;
        }
        #endregion
        #region SMethod: string[] Delete(string[] vs, int iPos)
        static public string[] Delete(string[] vs, int iPos)
        {
            string[] vsNew = new string[vs.Length - 1];

            for (int i = 0; i < iPos; i++)
                vsNew[i] = string.Copy(vs[i]);

            for (int i = iPos + 1; i < vs.Length; i++)
                vsNew[i-1] = string.Copy(vs[i]);

            return vsNew;
        }
        #endregion
    }
    #endregion

    #region CLASS: T_DSection
    [TestFixture] public class T_DSection : TestCommon
    {
        // Helper Methods --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
            JWU.NUnit_SetupClusterFolder();
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            JWU.NUnit_TeardownClusterFolder();
        }
        #endregion

        // Test Smarts -----------------------------------------------------------------------
        #region Method: void GetPictureCount(DSection)
        int GetPictureCount(DSection section)
        {
            int cPictures = 0;
            foreach (DParagraph p in section.Paragraphs)
            {
                if (p.GetType() == typeof(DPicture))
                    ++cPictures;
            }
            return cPictures;
        }
        #endregion

        #region Method: void _Compare(string sTitle, string[] vsActual, string[] vsExpected)
        void _Compare(string sTitle, string[] vsActual, string[] vsExpected)
        {
            // Do a silent compare to see if the same
            bool bConsoleOut = false;
            if (vsExpected.Length != vsActual.Length)
                bConsoleOut = true;
            else
            {
                for (int k = 0; k < vsActual.Length; k++)
                {
                    if (vsExpected[k] != vsActual[k])
                        bConsoleOut = true;
                }
            }

            // Output to console for debugging if different
            if (bConsoleOut)
            {
                SectionTestData.ConsoleOut_ShowDiffs("Test_DSectionIO " + sTitle,
                    vsActual, vsExpected);
            }

            // Do the assertions
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
            var Translation = CreateHierarchyThroughTargetTranslation();

            //////////////////////////////////////////////////////////////////////////////////
            // PART 1: IMPORT vsRaw AND SEE IF WE SAVE IT TO MATCH vsSAV. Thus we are testing
            // the import mechanism's ability to handle various stuff.
            //////////////////////////////////////////////////////////////////////////////////

            // Load the Raw data into a book
            DTestBook Book = SectionTestData.LoadIntoBook(vsRaw, Translation);

            // Now write it, to test TransformOut, & etc.
            Book.ExportToToolbox(Book.StoragePath, new NullProgress());

            // Read the SFM result from disk
            string[] vsResult = SectionTestData.ReadSfmFromBookFile(Book);

            // Compare what was written (vsResult) with what we expect (vsSav)
            _Compare("PART ONE", vsResult, vsSav);

            //////////////////////////////////////////////////////////////////////////////////
            // PART 2: GIVEN vsSav, READ IT, THEN WRITE IT, SEE IF THE READ/WRITE PRESERVES 
            // vsSav. Thus we are testing TransformIn, TransformOut, and the various IO 
            // routines in DBook and DSection
            //////////////////////////////////////////////////////////////////////////////////

            // Load the "Cannonical" data into a book
            Translation.BookList.Remove(Book);
            Book = SectionTestData.LoadIntoBook(vsSav, Translation);

            // Now, write it back out. This will do DB.TransformOut, as well as the
            // various write methods in DBook.
            Book.ExportToToolbox(Book.StoragePath, new NullProgress());

            // Finaly, read the result from disk
            vsResult = SectionTestData.ReadSfmFromBookFile(Book);

            // Compare what was written (vsResult) with what we expect (vsSav)
            _Compare("PART TWO", vsResult, vsSav);

            JWU.NUnit_TeardownClusterFolder();
        }
        #endregion

        // Individual Tests (each has a different data set) ----------------------------------
        #region TEST: IO_DataSet01 - Baikeno Mark 1:1
        [Test] public void IO_DataSet01()
        {
            IO_TestEngine(
                SectionTestData.BaikenoMark0101_ImportVariant,
                SectionTestData.BaikenoMark0101_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet02 - Baikeno Mark 1:9
        [Test] public void IO_DataSet02()
        {
            IO_TestEngine(
                SectionTestData.BaikenoMark0109_ImportVariant,
                SectionTestData.BaikenoMark0109_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet03 - Baikeno Mark 16:19
        [Test] public void IO_DataSet03()
            // Addresses bugs observed in the Baikeno Mark 16:19 passage:
            // 1. Not writing out the picture info where all we had was a pathname
            //      and rtf information Import.
            // 2. Not writing out a footnote (\ft) where we don't have a {fn} in 
            //      the preceeding verse.
        {
            IO_TestEngine(
                SectionTestData.BaikenoMark16_ImportVariant,
                SectionTestData.BaikenoMark16_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet04 - Baikeno Mark 4:30-34
        [Test] public void IO_DataSet04()
        {
            IO_TestEngine(
                SectionTestData.BaikenoMark430_ImportVariant, 
                SectionTestData.BaikenoMark430_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet05 - Pura Mark 14:03-14:09
        [Test] public void IO_DataSet05()
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

            IO_TestEngine(
                SectionTestData.PuraMark14_ImportVariant,
                SectionTestData.PuraMark14_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet06 - Helong Acts 4:01-4:04
        [Test] public void IO_DataSet06()
        {
            IO_TestEngine(
                SectionTestData.HelongActs04_ImportVariant,
                SectionTestData.HelongActs04_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet07 - Helong Acts 7:54
        [Test] public void IO_DataSet07()
        {
            // - Chapter in the final paragraph maintains its position as the 4th run
            // - Spaces in notes are eaten.

            IO_TestEngine(
                SectionTestData.HelongActs0754_ImportVariant,
                SectionTestData.HelongActs0754_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet08 - Helong Rev 1:4-8
        [Test] public void IO_DataSet08()
        {
            // - Empty shell: gets rid of empty vt/btvt's, spaces after markers

            IO_TestEngine(
                SectionTestData.HelongRev0104_ImportVariant,
                SectionTestData.HelongRev0104_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet09 - Manado Mark 13:14
        [Test] public void IO_DataSet09()
        {
            // - Two footnotes

            IO_TestEngine(
                SectionTestData.ManadoMark013014_ImportVariant,
                SectionTestData.ManadoMark013014_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet10 - Tombula Acts 9:32
        [Test] public void IO_DataSet10()
        {
            // - Empty section

            IO_TestEngine(
                SectionTestData.TombuluActs009032_ImportVariant,
                SectionTestData.TombuluActs009032_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet11 - English
        [Test] public void IO_DataSet11()
        {
            // - \mr \ms

            IO_TestEngine(
                SectionTestData.English_ImportVariant,
                SectionTestData.English_Cannonical);
        }
        #endregion
        #region TEST: IO_DataSet12 - Djambarrpungy 2 John
        [Test]
        public void IO_DataSet12()
        {
            // - \mr \ms

            IO_TestEngine(
                SectionTestData.Djambarrpungu_ImportVariant,
                SectionTestData.Djambarrpungu_Cannonical);
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
        #region CreateBlankTargetSectionFromFront()
        [Test] public void CreateBlankTargetSectionFromFront()
        // Tests the routine which creates a blank, ready-for-translating, section
        // based on the front translation.
        {
 			const bool EnableTracing = false;

            // Preliminary: Create the superstructure we need for a DBook
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DB.Project.FrontTranslation = new DTranslation("FrontTranslation", "Latin", "Latin");

            // Write out the string array, then read it into Book1's first section
            DTestBook FrontBook = SectionTestData.LoadIntoBook(
                SectionTestData.BaikenoMark0109_ImportVariant,
                DB.Project.FrontTranslation);
            DSection FrontSection = FrontBook.Sections[0];

            // Find which book is Luke, which is Isaiah, etc.
            int iLuke = -1;
            int iIsaiah = -1;
            int iMatthew = -1;
            for (int i = 0; i < 66; i++)
            {
                if (DBook.BookAbbrevs[i] == "MAT")
                    iMatthew = i;
                if (DBook.BookAbbrevs[i] == "LUK")
                    iLuke = i;
                if (DBook.BookAbbrevs[i] == "ISA")
                    iIsaiah = i;
            }

            // Set up some matching book names for the "front" translation
            DB.Project.FrontTranslation.BookNamesTable[iLuke] = "Lukas";
            DB.Project.FrontTranslation.BookNamesTable[iIsaiah] = "Yesaya";
            DB.Project.FrontTranslation.BookNamesTable[iMatthew] = "Mateus";

            // Create a target translation, with some different book names
            DB.Project.TargetTranslation = new DTranslation("Target", "Latin", "Latin");
            DB.Project.TargetTranslation.BookNamesTable[iLuke] = "Luke";
            DB.Project.TargetTranslation.BookNamesTable[iIsaiah] = "Isaiah";
            DB.Project.TargetTranslation.BookNamesTable[iMatthew] = "Matthew";

            // Create the new empty section
            DSection TargetSection = new DSection();
            DBook TargetBook = new DBook("MRK");
            DB.Project.TargetTranslation.AddBook(TargetBook);
            TargetBook.Sections.Append(TargetSection);
            TargetSection.InitializeFromFrontSection(FrontSection);

            // Expected String
            var vsExpected = new string[] { 
                "",
                "",
                "(Matthew 3:13-17; Luke 3:21-22)",
                "{v 9}{v 10}{v 11}",
                "",
                "{cf a}",
                "",
                ""};

            // Assemble the actual string
            string[] vsActual = new string[TargetSection.Paragraphs.Count];
            for (int i = 0; i < TargetSection.Paragraphs.Count; i++)
                vsActual[i] = TargetSection.Paragraphs[i].DebugString;

            // Tracing
            if (EnableTracing)
            {
             //   SectionTestData.ConsoleOut_ShowDiffs(
             //       "<<<CreateBlankTargetSectionFromFront Front Section (actual) compared to Target (exp)>>>",
             //       FrontSection, TargetSection);
                SectionTestData.ConsoleOut_ShowDiffs(
                    "<<<CreateBlankTargetSectionFromFront Blank Section (actual) compared to expected>>>",
                    vsActual, vsExpected);
            }

            // Check for the same styles in all the paragraph groups
            Assert.AreEqual(8, TargetSection.Paragraphs.Count);
            var iCrossRefPara = -1;
            for (var i = 0; i < TargetSection.Paragraphs.Count; i++)
            {
                var styleExpected = FrontSection.Paragraphs[i].Style;
                Assert.AreEqual(FrontSection.Paragraphs[i].Style, 
                    TargetSection.Paragraphs[i].Style);
                if (TargetSection.Paragraphs[i].Style == StyleSheet.SectionCrossReference)
                    iCrossRefPara = i;
            }

            // Check for \r paragraph to be appropriately converted
            var sCrossRefActual = TargetSection.Paragraphs[iCrossRefPara].DebugString;
            Assert.AreEqual("(Matthew 3:13-17; Luke 3:21-22)", sCrossRefActual);

            // Check for xref footnote (the first one) to be appropriately converted
            Assert.AreEqual(1, TargetSection.AllFootnotes.Count);
            string sFootnoteAct = TargetSection.AllFootnotes[0].DebugString;
            string sFootnoteExp = "Kejadian 22:2, Mazmur 2:7, Isaiah 42:1, " +
                "Matthew 3:17, 12:18, Markus 9:7, Luke 3:22";
            Assert.AreEqual(sFootnoteExp, sFootnoteAct);


            // Check for expected paragraph content, e.g., empty, or with verse/chapter/fn no's
            Assert.IsTrue(SectionTestData.AreSame(vsActual, vsExpected), 
                "Sections should be the same");

            // Check for picture replication
            Assert.AreEqual(1, GetPictureCount(FrontSection));
            DPicture pict = null;
            foreach (DParagraph p in TargetSection.Paragraphs)
            {
                pict = p as DPicture;
                if (p as DPicture != null)
                    break;
            }
            Assert.IsNotNull(pict);
            Assert.AreEqual("c:\\graphics\\cook\\cnt\\cn01656b.tif", pict.RelativePathName);
            Assert.AreEqual("width:9.0cm", pict.WordRtfInfo );
            Assert.AreEqual("", pict.DebugString);

            // Check the reference
            Assert.IsTrue(TargetSection.ReferenceSpan.ContentEquals(
                FrontSection.ReferenceSpan));
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: DSection CreateSection(string[])
        DSection CreateSection(string[] vs)
        {
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings( JWU.NUnit_ClusterFolderName );
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DB.Project.TargetTranslation = new DTranslation("Translation", "Latin", "Latin");

            DTestBook book = SectionTestData.LoadIntoBook(vs, DB.Project.TargetTranslation);

            return book.Sections[ book.Sections.Count - 1];
        }
        #endregion
        #region Test: Merge_SameStructure
        [Test] public void Merge_SameStructure()
        {
            #region TestData: vsParent
            string[] vsParent = new string[] 
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
			    "\\v 2",
			    "\\vt Oen komali lole Petrus nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Petrus and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)|fn",
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
			    "\\p",
			    "\\v 4",
			    "\\vt Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their people increased a lot " +
				    "approximately to five thousand people."
		    };
            #endregion

            // "Petrus" > "Peter"
            #region TestData: vsMine
            string[] vsMine = new string[] 
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
			    "\\s Tulu-tulu agama las haman Peter nol Yohanis maas tala",
			    "\\bts The heads of religion summon Peter and Yohanis to come appear.before [them]",
			    "\\p",
			    "\\v 1",
			    "\\vt Dedeng na, Peter nol Yohanis nahdeh nabael nol atuli las sam, " +
				    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				    "agama Saduki. Oen maas komali le ahan Peter nol Yohanis.",
			    "\\btvt At that time, Peter and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Peter and Yohanis.",
			    "\\v 2",
			    "\\vt Oen komali lole Peter nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Peter and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)|fn",
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
			    "\\p",
			    "\\v 4",
			    "\\vt Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their people increased a lot  " +
				    "approximately to five thousand people."
		    };
            #endregion

            // Some All-Caps, but in different DTexts
            #region TestData: vsTheirs
            string[] vsTheirs = new string[] 
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
			    "\\v 2",
			    "\\vt Oen komali lole Petrus nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Petrus and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)|fn",
			    "\\ft 4:1-2: Atuil deng partai agama Saduki, oen sium in tui na lo " +
				    "man noen atuil mate haup in nuli pait.",
			    "\\btft 4:1-2: People from the religious party Saduki, they did not accept that " +
				    "teaching that says dead people can live again.",
			    "\\v 3",
			    "\\vt Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
				    "DENG lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
				    "Le ola ka halas-sam, oen nehan DAIS na.",
			    "\\btvt And so they ordered their people to go capture the two of them. But " +
				    "because the sun already wanted to set, then they pushed [&] entered " +
				    "the two of them in jail. So.that the next day, THEY could take.care.of " +
				    "that affair/litigation/problem.",
			    "\\p",
			    "\\v 4",
			    "\\vt Mo deng atuli-atuil MAN kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak TEBES. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their PEOPLE increased a lot  " +
				    "approximately to five thousand people."
		    };
            #endregion

            // Should have both Mine and Theirs with no TranslatorNotes, because the
            // differences were all in different places
            #region TestData: vsExpected
            string[] vsExpected = new string[] 
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
			    "\\s Tulu-tulu agama las haman Peter nol Yohanis maas tala",
			    "\\bts The heads of religion summon Peter and Yohanis to come appear.before [them]",
			    "\\p",
			    "\\v 1",
			    "\\vt Dedeng na, Peter nol Yohanis nahdeh nabael nol atuli las sam, " +
				    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				    "agama Saduki. Oen maas komali le ahan Peter nol Yohanis.",
			    "\\btvt At that time, Peter and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Peter and Yohanis.",
			    "\\v 2",
			    "\\vt Oen komali lole Peter nol Yohanis na mo, kom isi le tek " +
				    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
				    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
				    "haup in nuli pait kon.>>|fn",
			    "\\btvt They were angry because that Peter and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)|fn",
			    "\\ft 4:1-2: Atuil deng partai agama Saduki, oen sium in tui na lo " +
				    "man noen atuil mate haup in nuli pait.",
			    "\\btft 4:1-2: People from the religious party Saduki, they did not accept that " +
				    "teaching that says dead people can live again.",
			    "\\v 3",
			    "\\vt Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
				    "DENG lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
				    "Le ola ka halas-sam, oen nehan DAIS na.",
			    "\\btvt And so they ordered their people to go capture the two of them. But " +
				    "because the sun already wanted to set, then they pushed [&] entered " +
				    "the two of them in jail. So.that the next day, THEY could take.care.of " +
				    "that affair/litigation/problem.",
			    "\\p",
			    "\\v 4",
			    "\\vt Mo deng atuli-atuil MAN kom in ming deng an in nutus sas, tiata " +
				    "atuili mamo hao noan asa man oen tui ka tom bak TEBES. Undeng na, " +
				    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima.",
			    "\\btvt But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their PEOPLE increased a lot " +
				    "approximately to five thousand people."
		    };
            #endregion

            // Read in the sections
            DSection Parent = CreateSection(vsParent);
            DSection Mine = CreateSection(vsMine);
            DSection Theirs = CreateSection(vsTheirs);
            DSection Expect = CreateSection(vsExpected);

            // Do the Merge
            Mine.Merge(Parent, Theirs);

            // Compare the result
            Mine.Book.WriteBook(new NullProgress());
            string[] vsActual = SectionTestData.ReadSfmFromBookFile(Mine.Book as DTestBook); 

            // WriteMergeCompareToConsole(vsExpected, vsActual);
            SectionTestData.AreSame(vsExpected, vsActual);
//            CompareMergeResults(vsExpected, vsActual);
        }
        #endregion
        #region Test: Merge_DifferentStructures
        [Test] public void Merge_DifferentStructures()
            // For this test, I change the structure in "Theirs", and the content in "Mine".
            // The result should be a clean merge with no notes needed
        {
            // Create four identical representatives of sections
            var vsParent = SectionTestData.Clone(SectionTestData.MergeActs004001);
            var vsMine = SectionTestData.Clone(vsParent);
            var vsTheirs = SectionTestData.Clone(vsParent);
            var vsExpected = SectionTestData.Clone(vsParent);

            // CHANGES TO MINE ----------------
            // In Mine, split some paragraphs. We will expect to see these same paragraph
            // splits in Expected. In this first one, we split the paragraph in the midst
            // of a span, causing part of the span to remain in the original paragraph,
            // and the rest of it to go to the following paragraph.
            vsMine[13] = "\\vt Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, " +
                    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
                    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
                    "agama Saduki.";
            vsMine[14] = "\\btvt At that time, Petrus and Yohanis still were talking with those people, " +
                    "several big/important people came. Those them(=They in focus), " +
                    "[were] heads of the Yahudi religion, and the head of guarding the " +
                    "*Temple, and people from the religious party Saduki.";
            vsMine = SectionTestData.Insert(vsMine, 15, "\\p");
            vsMine = SectionTestData.Insert(vsMine, 16, "\\vt Oen maas komali le ahan Petrus nol Yohanis.");
            vsMine = SectionTestData.Insert(vsMine, 17, "\\btvt They came angry to scream at Petrus and Yohanis.");

            // Because we're making a structure change, the Expected will be the same text as Mine
            vsExpected[13] = string.Copy(vsMine[13]);
            vsExpected[14] = string.Copy(vsMine[14]);
            vsExpected = SectionTestData.Insert(vsExpected, 15, string.Copy(vsMine[15]));
            vsExpected = SectionTestData.Insert(vsExpected, 16, string.Copy(vsMine[16]));
            vsExpected = SectionTestData.Insert(vsExpected, 17, string.Copy(vsMine[17]));

            // The second paragraph insertion is just an easy addition between existing elements,
            // before v3. That would have been at line 20, but because we've already inserted
            // 3 lines, it is not at 23.
            vsMine = SectionTestData.Insert(vsMine, 20+3, "\\p");
            vsExpected = SectionTestData.Insert(vsExpected, 20+3, "\\p");

            //Delete the "\st The Gospel Of" in the title section
            vsMine = SectionTestData.Delete(vsMine, 4);
            vsExpected = SectionTestData.Delete(vsExpected, 4);

            // CHANGES TO THEIRS --------------
            // In Theirs, change "Petrus" to "Peter"  in translation and back translation
            // We will expect to see this in Expected. We'll count to make sure this
            // truly does take place in cPetrusReplacements.
            var cPetrusReplacements = 0;
            for (var i = 0; i < vsTheirs.Length; i++)
            {
                if (vsTheirs[i].IndexOf("Petrus") != -1)
                {
                    vsTheirs[i] = vsTheirs[i].Replace("Petrus", "Peter");
                    cPetrusReplacements++;
                }
            }
            Assert.AreEqual(6, cPetrusReplacements, 
                "cPetrusReplacements = " + cPetrusReplacements.ToString());

            // But there will be Notes in Expected to show all of the textual changes
            // made by Theirs
            vsExpected = SectionTestData.Insert(vsExpected, 5,
                "\\tn <Annotation class=\"General\" selectedText=\"*Mark\">" +
                "<Message author=\"From Merge\" created=\"2009-05-14 17:02:20Z\" status=\"Anyone\">The other version had \"The Gospel Of\".</Message>" +
                "</Annotation>");

            vsExpected = SectionTestData.Insert(vsExpected, 11,
                "\\tn <Annotation class=\"General\" selectedText=\"an Petr*rus nol\">" +
                "<Message author=\"From Merge\" created=\"2009-05-14 19:24:14Z\" status=\"Anyone\">The other version " +
                    "had \"Tulu-tulu agama las haman Peter nol Yohanis maas tala\".</Message>" +
                "</Annotation>");

            vsExpected = SectionTestData.Insert(vsExpected, 16,
                "\\tn <Annotation class=\"General\" selectedText=\"a, Petr*rus nol\">" +
                "<Message author=\"From Merge\" created=\"2009-05-14 19:26:56Z\" status=\"Anyone\">The other version had \"Dedeng na, Peter nol Yohanis nahdeh " +
                    "nabael nol atuli las sam, atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol tulu in doh Um in Kohe " +
                    "kanas Tene ka, nol atuil deng partaiagama Saduki. Oen maas komali le ahan Peter nol Yohanis.\".</Message>" +
                "</Annotation>");

            vsExpected = SectionTestData.Insert(vsExpected, 20,
                "\\tn <Annotation class=\"General\" selectedText=\"*Oen maa\">" +
                "<Message author=\"From Merge\" created=\"2009-05-14 19:30:02Z\" status=\"Anyone\">The other version had \"Dedeng na, Peter nol Yohanis " +
                    "nahdeh nabael nol atuli las sam, atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol tulu in doh Um in " +
                    "Kohe kanas Tene ka, nol atuil deng partaiagama Saduki. Oen maas komali le ahan Peter nol Yohanis.\".</Message>" +
                "</Annotation>");

            vsExpected = SectionTestData.Insert(vsExpected, 24,
                "\\tn <Annotation class=\"General\" selectedText=\"le Petr*rus nol\">" +
                "<Message author=\"From Merge\" created=\"2009-05-14 19:32:31Z\" status=\"Anyone\">The other version had \"Oen komali lole Peter nol " +
                    "Yohanis na mo, kom isi le tek atuli-atuli las to-toang, noan, &lt;&lt;Yesus nuli pait son, deng Un in mate ka! Tiata " +
                    "ela Un sai lalan bel atuil in mateng ngas, le oen kon haup in nuli pait kon.&gt;&gt;\".</Message>" +
                "</Annotation>");

            // Read in the sections
            DSection Parent = CreateSection(vsParent);
            DSection Mine = CreateSection(vsMine);
            DSection Theirs = CreateSection(vsTheirs);
            DSection Expect = CreateSection(vsExpected);

            // Create the "Merge Method" for doing the work. We'll test some of the
            // individual components, before testing the entire composite.
            var m = new DSection.MergeMethod(Parent, Mine, Theirs);

            // DSection.MergeMethod method: GetSectionContentsAsFlatString
            var sFlatString = m.GetSectionContentsAsFlatString(Theirs);
            #region Assert.AreEqual(sExpected, sFlatString)
            Assert.AreEqual(
                "Mark" +
                "The Gospel Of" +
                "Mark" +
                "Tulu-tulu agama las haman Peter nol Yohanis maas tala" +
                  "The heads of religion summon Peter and Yohanis to come appear.before [them]" +
                "41" +
                "Dedeng na, Peter nol Yohanis nahdeh nabael nol atuli las sam, " +
                    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
                    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
                    "agama Saduki. Oen maas komali le ahan Peter nol Yohanis." +
                  "At that time, Peter and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Peter and Yohanis." +
                "2" +
                "Oen komali lole Peter nol Yohanis na mo, kom isi le tek " +
                    "atuli-atuli las to-toang, noan, <<Yesus nuli pait son, deng Un in " +
                    "mate ka! Tiata ela Un sai lalan bel atuil in mateng ngas, le oen kon " +
                    "haup in nuli pait kon.>>" +
                  "They were angry because that Peter and Yohanis, like to tell all " +
				    "people, saying, \"Yesus has lived again from His death! With that, He " +
				    "opened the path for dead people so that they also could live again.\" " +
				    "(check two kon)" +
                "a" +
                "3" +
                "Hidi kon oen tadu oen atulin nas le laok daek nal oen duas. Mo un " +
                    "deng lelo la dene, kon oen hutun tamang oen duas lakos bui dalen. " +
                    "Le ola ka halas-sam, oen nehan dais na." +
                  "And so they ordered their people to go capture the two of them. But " +
				    "because the sun already wanted to set, then they pushed [&] entered " +
				    "the two of them in jail. So.that the next day, they could take.care.of " +
				    "that affair/litigation/problem." +
                "4" +
                "Mo deng atuli-atuil man kom in ming deng an in nutus sas, tiata " +
                    "atuili mamo hao noan asa man oen tui ka tom bak tebes. Undeng na, " +
                    "tiata oen atulin nas oen taplaeng mamo, nataka le atuli lihu lima." +
                  "But from the people who liked to hear from those apostles, therefore " +
				    "many people had already acknowledged that that which they taught, " +
				    "[was] spot.on correct. That's.why their people increased a lot " +
				    "approximately to five thousand people." +
                "4:1-2:Atuil deng partai agama Saduki, oen sium in tui na lo " +
                    "man noen atuil mate haup in nuli pait." +
                  "People from the religious party Saduki, they did not accept that " +
				    "teaching that says dead people can live again.", 
                sFlatString, "Problem in GetSectionContentAsFlatString");
            #endregion

            // DSection.MergeMethod method: PositionToRun
            DRun run = m.PositionToRun(Theirs, 1000);
            Assert.AreEqual("Oen komali lole Peter", (run as DText).AsString.Substring(0, 21));
            run = m.PositionToRun(Theirs, 1900);
            Assert.AreEqual("Mo deng atuli-atui", (run as DText).AsString.Substring(0, 18));
            run = m.PositionToRun(Theirs, 2000);
            Assert.AreEqual("Atuil deng partai agama", (run as DText).AsString.Substring(0, 23));

            // Do the Merge
            Mine.Merge(Parent, Theirs);

            // Compare the result
            Mine.Book.ExportToToolbox(Mine.Book.StoragePath, new NullProgress());
            string[] vsActual = SectionTestData.ReadSfmFromBookFile(Mine.Book as DTestBook);

            // Comment out unless debugging
            bool bAreSame = SectionTestData.AreSame(vsExpected, vsActual);
            if (!bAreSame)
                SectionTestData.ConsoleOut_ShowDiffs("Merge Different Structures", vsActual, vsExpected);
            Assert.IsTrue(bAreSame, "Structures should be the same");
        }
        #endregion

        // GoBible ---------------------------------------------------------------------------
        #region Test: GoBibleExport
        [Test] public void GoBibleExport()
        {
            // Preliminary: Create the superstructure we need for a DBook
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings( JWU.NUnit_ClusterFolderName );
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;

            // Load the Raw data into a book
            DTestBook Book = SectionTestData.LoadIntoBook(
                SectionTestData.GoBible_Import1, Translation);

            // Export in GoBible format
            string sGoBiblePath = JWU.NUnit_TestFileFolder + 
                Path.DirectorySeparatorChar + "Test.GoBible";
            Book.ExportToGoBible(sGoBiblePath, new NullProgress());

            // Read back in as raw string array
            var vs = new List<string>();
            TextReader tr = JWU.NUnit_OpenTextReader("Test.GoBible");
            do
            {
                string s = tr.ReadLine();
                if (null == s)
                    break;
                vs.Add(s);
            } while (true);
            tr.Close();
            string[] vsActual = vs.ToArray();

            // Compare
            //SectionTestData.ConsoleOut_ShowDiffs("GoBible Results", vsActual, 
            //    SectionTestData.GoBible_Export1);
            Assert.IsTrue(SectionTestData.AreSame(vsActual, SectionTestData.GoBible_Export1),
                "GoBible Export does not equal what was expected.");
        }
        #endregion

        // Oxes ------------------------------------------------------------------------------
        #region Method: void OxesTestEngine(...)
        void OxesTestEngine(string sTest, string[] vsToolbox, string[] vsOxesExpected)
        {
            // Preliminary: Create the superstructure we need for a DBook
            var Translation = CreateHierarchyThroughTargetTranslation();

            // Load in our Expected Xml from the string array
            var xmlExpected = new XmlDoc(vsOxesExpected);
            xmlExpected.NormalizeCreatedDates();

            // PART 1 - IMPORT FROM SFM, CREATE WELL-FORMED OXES
            // Load the raw data into the book
            DBook Book = SectionTestData.LoadIntoBook(vsToolbox, Translation);

            // Create the oxes xml objects
            var xmlActual = Book.ToOxesDocument;
            xmlActual.NormalizeCreatedDates();

            // Compare with what we expect
            Assert.IsTrue(XmlDoc.Compare(xmlExpected, xmlActual),
                "Oxes should be same for Test #" + sTest + " - Part 1");

            // PART 2 - ROUND-TRIP OXES
            // Create a new book from our cannonical Oxes
            var sNewBookPath = JWU.NUnit_TestFileFolder + Path.DirectorySeparatorChar + "BookNew.oxes";
            xmlExpected.Save(sNewBookPath);
            var bookNew = new DBook(Book.BookAbbrev);
            Translation.AddBook(bookNew);
            bookNew.LoadFromOxes(sNewBookPath, new NullProgress());
            File.Delete(sNewBookPath);

            // Create the new book's xml
            xmlActual = bookNew.ToOxesDocument;
            xmlActual.NormalizeCreatedDates();

            // Compare
            Assert.IsTrue(XmlDoc.Compare(xmlExpected, xmlActual),
                "Oxes should be same for Test #" + sTest + " - Part 2");
        }
        #endregion
        #region Test: Oxes01
        [Test] public void Oxes01()
        {
            OxesTestEngine( "1",
                SectionTestData.BaikenoMark0101_Cannonical,
                SectionTestData.BaikenoMark0101_oxes);
        }
        #endregion
        #region Test: Oxes02
        [Test] public void Oxes02()
        {
            OxesTestEngine("2",
                SectionTestData.BaikenoMark0109_Cannonical,
                SectionTestData.BaikenoMark0109_oxes);
        }
        #endregion
        #region Test: Oxes03
        [Test] public void Oxes03()
        {
            OxesTestEngine("3",
                SectionTestData.BaikenoMark16_Cannonical,
                SectionTestData.BaikenoMark16_oxes);
        }
        #endregion
        #region Test: Oxes04
        [Test] public void Oxes04()
        {
            OxesTestEngine("4",
                SectionTestData.BaikenoMark430_Cannonical,
                SectionTestData.BaikenoMark430_oxes);
        }
        #endregion
        #region Test: Oxes05
        [Test] public void Oxes05()
        {
            OxesTestEngine("5",
                SectionTestData.PuraMark14_Cannonical,
                SectionTestData.PuraMark14_oxes);
        }
        #endregion
        #region Test: Oxes06
        [Test] public void Oxes06()
        {
            OxesTestEngine("6",
                SectionTestData.HelongActs04_Cannonical,
                SectionTestData.HelongActs04_oxes);
        }
        #endregion
        #region Test: Oxes07
        [Test] public void Oxes07()
        {
            OxesTestEngine("7",
                SectionTestData.HelongActs0754_Cannonical,
                SectionTestData.HelongActs0754_oxes);
        }
        #endregion
        #region Test: Oxes08
        [Test] public void Oxes08()
        {
            OxesTestEngine("8",
                SectionTestData.HelongRev0104_Cannonical,
                SectionTestData.HelongRev0104_oxes);
        }
        #endregion
        #region Test: Oxes09
        [Test] public void Oxes09()
        {
            OxesTestEngine("9",
                SectionTestData.ManadoMark013014_Cannonical,
                SectionTestData.ManadoMark013014_oxes);
        }
        #endregion
        #region Test: Oxes10
        [Test] public void Oxes10()
        {
            OxesTestEngine("10",
                SectionTestData.TombuluActs009032_Cannonical,
                SectionTestData.TombuluActs009032_oxes);
        }
        #endregion
        #region Test: Oxes11
        [Test] public void Oxes11()
        {
            OxesTestEngine("11",
                SectionTestData.English_Cannonical,
                SectionTestData.English_oxes);
        }
        #endregion
        #region Test: Oxes12
        [Test]
        public void Oxes12()
        {
            OxesTestEngine("11",
                SectionTestData.Djambarrpungu_Cannonical,
                SectionTestData.Djambarrpungu_oxes);
        }
        #endregion
    }
    #endregion
}
