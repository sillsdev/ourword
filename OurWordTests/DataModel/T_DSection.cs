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

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
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

        // 4. Baikeno Mark 4:30-34
        #region TestData #4: BaikenoMark430_ImportVariant
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
        #region TestData #4: BaikenoMark430_Cannonical
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

        // 5. Pura Mark 14:03-14:09
        #region TestData #5: PuraMark14_ImportVariant
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
        #region TestData #5: PuraMark14_Cannonical
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
			    "\\btvt Some back translating for verse 5|fn",
                "\\ft A footnote for verse 5",
                "\\vt lung niang ila, se Na ing veng hama-hama niang ila.",
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

        // 6. Helong Acts 4:01-4:04
        #region TestData #6: HelongActs04_ImportVariant
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
        #region TestData #6: HelongActs04_Cannonical
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

        // 8. Helong Rev 1:4-8 (empty shell)
        #region TextData #8 - HelongRev0104_ImportVariant
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
        #region TextData #8 - HelongRev0104_Cannonical
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

        // 9. Manado Mark 13:14 (two footnotes)
        #region TextData #9 - ManadoMark013014_ImportVariant
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
        #region TextData #9 - ManadoMark013014_Cannonical
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
			"\\ft 13:14b Tu tampa itu Ruma Ibada Pusat [lia Matius 24:15].",
            "\\btft 13:14b",
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

        // 10. Tombulu Acts 9:32-35 (empty section)
        #region TextData #10 - TombuluActs009032_ImportVariant
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
        #region TextData #10 - TombuluActs009032_Cannonical
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

        // 11. English (\mr, \ms)
        #region TextData #11 - English_ImportVariant
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
        #region TestData #11: English_Cannonical
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
            Book.Load(new NullProgress());

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
                int i = s.IndexOf("Created=");
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

    [TestFixture] public class T_DSection
    {
        // Helper Methods --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            JWU.NUnit_SetupClusterFolder();
        }
        #endregion
        #region TearDown
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
            // Optionally output to console for debugging
            bool bConsoleOut = false;
            if (bConsoleOut)
            {
                SectionTestData.ConsoleOut_ShowDiffs("Test_DSectionIO",
                    vsActual, vsExpected);
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
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings( JWU.NUnit_ClusterFolderName );
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;

            //////////////////////////////////////////////////////////////////////////////////
            // PART 1: IMPORT vsRaw AND SEE IF WE SAVE IT TO MATCH vsSAV. Thus we are testing
            // the import mechanism's ability to handle various stuff.
            //////////////////////////////////////////////////////////////////////////////////

            // Load the Raw data into a book
            DTestBook Book = SectionTestData.LoadIntoBook(vsRaw, Translation);

            // Now write it, to test TransformOut, & etc.
            Book.DeclareDirty();                 // It'll not write without this
            Book.Write(new NullProgress());

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
            Translation.Books.Remove(Book);
            Book = SectionTestData.LoadIntoBook(vsSav, Translation);

            // Now, write it back out. This will do DB.TransformOut, as well as the
            // various write methods in DBook.
            Book.DeclareDirty();                 // It'll not write without this
            Book.Write(new NullProgress());

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
 			bool EnableTracing = false;

            // Preliminary: Create the superstructure we need for a DBook
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
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
            DSection TargetSection = new DSection(1);
            DBook TargetBook = new DBook();
            DB.Project.TargetTranslation.Books.Append(TargetBook);
            TargetBook.Sections.Append(TargetSection);
            TargetSection.InitializeFromFrontSection(FrontSection);

            // Expected String
            string[] vsExpected = new string[] { 
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
            int iCrossRefPara = -1;
            for (int i = 0; i < TargetSection.Paragraphs.Count; i++)
            {
                string sExpected = FrontSection.Paragraphs[i].StyleAbbrev;
                string sActual = TargetSection.Paragraphs[i].StyleAbbrev;
                Assert.AreEqual(sExpected, sActual);
                if (sActual == DB.TeamSettings.SFMapping.StyleCrossRef)
                    iCrossRefPara = i;
            }

            // Check for \r paragraph to be appropriately converted
            string sCrossRefActual = (TargetSection.Paragraphs[iCrossRefPara] as
                DParagraph).DebugString;
            Assert.AreEqual("(Matthew 3:13-17; Luke 3:21-22)", sCrossRefActual);

            // Check for xref footnote (the first one) to be appropriately converted
            Assert.AreEqual(1, TargetSection.Footnotes.Count);
            string sFootnoteAct = (TargetSection.Footnotes[0] as DFootnote).SimpleText;
            string sFootnoteExp = "1:11: Kejadian 22:2, Mazmur 2:7, Isaiah 42:1, " +
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
            Assert.AreEqual(pict.PathName, "c:\\graphics\\cook\\cnt\\cn01656b.tif");
            Assert.AreEqual(pict.WordRtfInfo, "width:9.0cm");
            Assert.AreEqual(pict.DebugString, "");

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
            DB.TeamSettings.EnsureInitialized();
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
            Mine.Book.Write(new NullProgress());
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
            string[] vsParent = SectionTestData.Clone(SectionTestData.MergeActs004001);
            string[] vsMine = SectionTestData.Clone(vsParent);
            string[] vsTheirs = SectionTestData.Clone(vsParent);
            string[] vsExpected = SectionTestData.Clone(vsParent);

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
            int cPetrusReplacements = 0;
            for (int i = 0; i < vsTheirs.Length; i++)
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
                "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"\" Context=\"*Mark\" " +
                "Reference=\"004:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                "<Discussion Author=\"From Merge\" Created=\"2009-05-14 17:02:20Z\"><ownseq " +
                "Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"The other " +
                "version had &quot;The Gospel Of&quot;.\"/></ownseq>" +
                "</Discussion></ownseq></TranslatorNote>");

            vsExpected = SectionTestData.Insert(vsExpected, 11,
                "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"\" Context=\"an Petr*rus nol\" " +
                "Reference=\"004:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                "<Discussion Author=\"From Merge\" Created=\"2009-05-14 19:24:14Z\"><ownseq " +
                "Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"The other version " +
                "had &quot;Tulu-tulu agama las haman Peter nol Yohanis maas tala&quot;.\"/>" +
                "</ownseq></Discussion></ownseq></TranslatorNote>");

            vsExpected = SectionTestData.Insert(vsExpected, 16,
                "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"\" Context=\"a, Petr*rus nol\" " +
                "Reference=\"004:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                "<Discussion Author=\"From Merge\" Created=\"2009-05-14 19:26:56Z\"><ownseq " +
                "Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"The other version " +
                "had &quot;Dedeng na, Peter nol Yohanis nahdeh nabael nol atuli las sam, atuil tene " +
                "kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol tulu in doh Um in Kohe " +
                "kanas Tene ka, nol atuil deng partaiagama Saduki. Oen maas komali le ahan Peter " +
                "nol Yohanis.&quot;.\"/></ownseq></Discussion></ownseq></TranslatorNote>");

            vsExpected = SectionTestData.Insert(vsExpected, 20,
                "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"\" Context=\"*Oen maa\" " +
                "Reference=\"004:001\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                "<Discussion Author=\"From Merge\" Created=\"2009-05-14 19:30:02Z\"><ownseq " +
                "Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"The other " +
                "version had &quot;Dedeng na, Peter nol Yohanis nahdeh nabael nol atuli las " +
                "sam, atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol tulu in " +
                "doh Um in Kohe kanas Tene ka, nol atuil deng partaiagama Saduki. Oen maas komali " +
                "le ahan Peter nol Yohanis.&quot;.\"/></ownseq></Discussion></ownseq></TranslatorNote>");

            vsExpected = SectionTestData.Insert(vsExpected, 24,
                "\\tn <TranslatorNote Category=\"To Do\" AssignedTo=\"\" Context=\"le Petr*rus nol\" " +
                "Reference=\"004:002\" ShowInDaughter=\"false\"><ownseq Name=\"Discussions\">" +
                "<Discussion Author=\"From Merge\" Created=\"2009-05-14 19:32:31Z\"><ownseq " +
                "Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"The other version " +
                "had &quot;Oen komali lole Peter nol Yohanis na mo, kom isi le tek atuli-atuli las " +
                "to-toang, noan, &lt;&lt;Yesus nuli pait son, deng Un in mate ka! Tiata ela Un sai " +
                "lalan bel atuil in mateng ngas, le oen kon haup in nuli pait kon.&gt;&gt;&quot;.\"/>" +
                "</ownseq></Discussion></ownseq></TranslatorNote>");

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
            Mine.Book.Write(new NullProgress());
            string[] vsActual = SectionTestData.ReadSfmFromBookFile(Mine.Book as DTestBook);

            // Comment out unless debugging
            //SectionTestData.ConsoleOut_ShowDiffs("Merge Different Structures", vsActual, vsExpected);

            bool bAreSame = SectionTestData.AreSame(vsExpected, vsActual);
            Assert.IsTrue(bAreSame, "Structures should be the same");
        }
        #endregion

    }
}
