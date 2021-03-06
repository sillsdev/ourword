/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_ScriptureDB.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the ScriptureDB class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;

using OurWordTests.Cellar;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_ScriptureDB
    {
        // String Sets -----------------------------------------------------------------------
        string[][] m_vvIn;
        string[][] m_vvExpected;
        #region Method: void InitializeStringSet(int cSize)
        void InitializeStringSet(int cSize)
        {
            m_vvIn = new string[cSize][];
            m_vvExpected = new string[cSize][];
        }
        #endregion
        #region Method: void AddStringSet(int i, string[] vIn, string[] vExpected)
        void AddStringSet(int i, string[] vIn, string[] vExpected)
        {
            m_vvIn[i] = vIn;
            m_vvExpected[i] = vExpected;
        }
        #endregion
        #region Method: void WriteActualDataToConsole(ScriptureDB DB, string sTitle)
        void WriteActualDataToConsole(ScriptureDB DB, string sTitle)
        {
            Console.WriteLine("");
            Console.WriteLine(sTitle);
            string[] vs = DB.ExtractData();
            foreach (string s in vs)
                Console.WriteLine("<" + s + ">");
        }
        #endregion

        // Some Data Sets --------------------------------------------------------------------
        #region Data: 1 - data I did when originally coding these routines circa 2005
        #region m_vIn_1
        string[] m_vIn_1 = 
		{
			"\\id LUK - Tera",
			"\\p",
			"\\s LABAR MBARKANDI NU_ LUKA BU_LAKI",
			"\\c 1",
			"\\p",
			"\\v 1 Nu_ke nduki khang wa shi bu_lar dyine dyiku nu_ shiki chele ?aarem.\\f g",
			"\\fr 3.7",
			"\\fk Baftisma:",
			"\\fk* Dubi ma'anar wanasu kalmoni.",
			"\\f*",
			"\\v 2 ba wani nu_ mbu Yesu ku nu_ guma-paali nu_kya.",
			"\\rc 9.19: ",
			"\\rt Mat 14.1-2; Mar 6.14-15; Luk 9.7-8\\rc*",
			"\\v 3 Wa shi mbiri nga ndu_ vara njel nga su_ dyiku na yo kap ku_me",
			"\\s Another section head",
			"\\p",
			"\\v 4 Sarchi nu_ke Hiridus kuji Yahuda.\\rc 12.1: ",
			"\\rt Mat 16.6; Mar 8.15\\rc* ",
			"\\v 5 Ku_lmbu_r ?aanda dya nu_ sawar Magham ku,",
			"\\v 6 Wa geemaku, yang aa da woi kha war ?a ku_me Elizabet kwarandan.",
			"\\q1",
			"\\v 7 Sarchi ngguri Zakariya wa ?u_ ma tlu_nar\\f e \\fr 2:41",
			"\\fk Bikin Detarewa: \\fk* Dubi ma'anar wanasu kalmoni.\\f* mbu_ kib ?u_ Magham",
			"\\v 8 Tu_ dat Zakariya ku maa va dye-zhi mu_nkandi",
			"\\v 9,10,11 mbu_ kib ?u_ Magham ku_me naake me-ghayan",
		};
        #endregion
        #region m_vExpectedParatext_1
        string[] m_vExpectedParatext_1 = 
		{
			"\\id LUK - Tera",
			"\\p",
			"\\c 1",
			"\\s LABAR MBARKANDI NU_ LUKA BU_LAKI",
			"\\p",
			"\\v 1 Nu_ke nduki khang wa shi bu_lar dyine dyiku nu_ shiki chele ?aarem." +
                "\\f + \\fr 3.7\\fr* Baftisma: Dubi ma'anar wanasu kalmoni.\\f*",
			"\\v 2 ba wani nu_ mbu Yesu ku nu_ guma-paali nu_kya." +
                "\\x + \\xo 9.19:\\xo* Mat 14.1-2; Mar 6.14-15; Luk 9.7-8\\x*",
			"\\v 3 Wa shi mbiri nga ndu_ vara njel nga su_ dyiku na yo kap ku_me",
			"\\s Another section head",
			"\\p",
			"\\v 4 Sarchi nu_ke Hiridus kuji Yahuda.\\x + \\xo 12.1:\\xo* Mat 16.6; Mar 8.15\\x*",
			"\\v 5 Ku_lmbu_r ?aanda dya nu_ sawar Magham ku,",
			"\\v 6 Wa geemaku, yang aa da woi kha war ?a ku_me Elizabet kwarandan.",
			"\\q",
			"\\v 7 Sarchi ngguri Zakariya wa ?u_ ma tlu_nar\\f + \\fr 2:41\\fr* " +
			    "Bikin Detarewa: Dubi ma'anar wanasu kalmoni.\\f* mbu_ kib ?u_ Magham",
			"\\v 8 Tu_ dat Zakariya ku maa va dye-zhi mu_nkandi",
			"\\v 9-11 mbu_ kib ?u_ Magham ku_me naake me-ghayan",
		};
        #endregion
        #region m_vExpectedToolbox_1
        string[] m_vExpectedToolbox_1 =
		{
			"\\rcrd",
			"\\id LUK - Tera",
			"\\rcrd 1",
			"\\p",
			"\\s LABAR MBARKANDI NU_ LUKA BU_LAKI",
			"\\p",
			"\\c 1",
			"\\v 1",
			"\\vt Nu_ke nduki khang wa shi bu_lar dyine dyiku nu_ shiki chele ?aarem.|fn ",
			"\\ft 3.7 Baftisma: Dubi ma'anar wanasu kalmoni.",
			"\\v 2",
			"\\vt ba wani nu_ mbu Yesu ku nu_ guma-paali nu_kya. ",
			"\\cf 9.19: Mat 14.1-2; Mar 6.14-15; Luk 9.7-8",
			"\\v 3",
			"\\vt Wa shi mbiri nga ndu_ vara njel nga su_ dyiku na yo kap ku_me",
			"\\rcrd 2",
			"\\s Another section head",
			"\\p",
			"\\v 4",
			"\\vt Sarchi nu_ke Hiridus kuji Yahuda.",
			"\\cf 12.1: Mat 16.6; Mar 8.15",
			"\\v 5",
			"\\vt Ku_lmbu_r ?aanda dya nu_ sawar Magham ku,",
			"\\v 6",
			"\\vt Wa geemaku, yang aa da woi kha war ?a ku_me Elizabet kwarandan.",
			"\\q",
			"\\v 7",
			"\\vt Sarchi ngguri Zakariya wa ?u_ ma tlu_nar|fn mbu_ kib ?u_ Magham",
			"\\ft 2:41 Bikin Detarewa: Dubi ma'anar wanasu kalmoni.",
			"\\v 8",
			"\\vt Tu_ dat Zakariya ku maa va dye-zhi mu_nkandi",
			"\\v 9-11",
			"\\vt mbu_ kib ?u_ Magham ku_me naake me-ghayan",
		};
        #endregion
        #endregion
        #region Data: 2 - From Napu 1Co
        #region m_vIn_2
        string[] m_vIn_2 =
            {
                "\\id 1CO",
                "\\h 1 Korintu",
                "\\mt2 Surana Suro Paulu au Nguru-nguruna",
                "\\mt2 i to Sarani i Kota",
                "\\mt Korintu",
                "",
                "\\c 1",
                "\\p",
                "\\v 1-2 Sura ide au hangko iriko Paulu hai hangko i Sostenes, kipakatu irikamu to " +
                    "Sarani i kota Korintu au nakakiokau Pue Ala mewali taunaNa. Iami au mopamalelahamokau " +
                    "anti pohintuwumi hai Kerisitu Yesu hihimbela hai ope-ope tauna au maida iumba pea au " +
                    "mekakae i Yesu Kerisitu, Amputa hai Ampunda. Iko ide, naangkana Pue Ala mewali surona " +
                    "Yesu Kerisitu moula peundeaNa.",
                "\\v 3 Ngkaya tabea! Mekakaena i Pue Ala Umanta hai i Pue Yesu Kerisitu bona Ia mowatikau " +
                    "hangko i kabulana laluNa hai moweikau roa ngkatuwo.",
                "\\s Paulu manguli ngkaya kamaroana i Pue Ala",
                "\\p",
                "\\v 4 Halalu, barana mengkangkaroo manguli ngkaya kamaroana i Pue Ala anti kaikamuna, lawi " +
                    "Ia moweikau pewati hangko i kabulana laluNa anti pohintuwumi hai Kerisitu Yesu.",
                "\\v 5 Anti pohintuwumi hai Kerisitu, Naweimokau hinangkana pewati au mobundu i katuwomi: " +
                    "Napopaisaamokau paturo au tou, hai Naweimokau kapande mopahawe paturo iti.",
                "\\v 6 Ope-ope iti mopakanoto kamarohona pepoinalaimi i bambari kana i Kerisitu au kipahaweakau,",
                "\\v 7 duuna barapi ara kamakurana pewati au nilambi. Nilambimi paka-pakana kapande hangko i " +
                    "Inao Malelaha, hai mampegiakau kahawena hule Amputa Yesu Kerisitu.",
                "\\v 8 Ia ina mopakaroho pepoinalaimi duuna i kahopoa dunia, bona ane ikamu nakahawei Amputa " +
                    "Yesu Kerisitu i kahaweNa hule, barapi ara kamasalami.",
                "\\v 9 Pue Ala mokakiokau bona mohintuwukau hihimbela hai AnaNa, Yesu Kerisitu Amputa, hai " +
                    "Ia mopabukei liliu dandiNa irikita.",
                "\\s Paulu mokambaroahe lawi barahe mohintuwu",
                "\\p",
                "\\v 10 Halalu, hangko i kuasa au naweina Amputa Yesu Kerisitu, merapina bona mohintuwukau " +
                    "ope-ope. Ineekau mopaara posisala. Hangangaa mohintuwukau hai hampepekirikau.",
                "\\v 11 Halalungku, tunggaiaku manguli nodo, lawi arahe halalunta hasou-souna Kloe au mopahawe " +
                    "iriko kaarana posisalami.",
                "\\v 12 Mololitana nodo lawi arakau au mampeinao mampopontani hadua hai hadua. Arakau au manguli: " +
                    "<<Iko meula i Paulu.>> Arakau au manguli: <<Iko meula i Apolo.>> Au ntanina wori manguli: " +
                    "<<Iko meula i Peturu.>>\\f + \\fr 1:12 \\ft Peturu \\ft - Basa Yunani: Kefas \\ft Peita " +
                    "wori 3:22; 9:5 hai 15:5. Peturu ba Simo Peturu rahanga wori Kefas.\\f* Ara mbuli au manguli: " +
                    "<<Iko meula i Kerisitu.>>",
                "\\v 13 Maroa pae ane ikamu au meula i Kerisitu mosisala nodo? Ba iko au mate rapaku i kau " +
                    "mohuru dosami? Ba rariukau bona mewalikau topegurungku? Bara!",
                "\\v 14 Kuuli ngkaya kamaroana lawi barakau ara au kuriu, batenda pea Krispus hai Gayu,",
                "\\v 15 bona datihe ara au manguli kuriukau mewali topegurungku.",
                "\\v 16 (Oo, Setepanu wori hantambi, iko mpuu au moriuhe. Agayana i raoa hangko irihira, barapi " +
                    "kukatuinao tauna ntanina au kuriu.)",
                "\\v 17 Kerisitu barana moangka mewali suroNa bona moriuna tauna, agayana Naangkana bona " +
                    "moantina Ngkora Marasa. Ane kupahawe Ngkora Marasa, barana mololita hangko i kamapandeku " +
                    "datihe ara tauna au mampoinalai lolitangku anti kamapandeku moleri barahe moisa kuasana " +
                    "Kerisitu au mate i kau mombehape.",
                "\\s Hangko i Ngkora Marasa taisa kuasana hai kamanotona laluna Pue Ala",
                "\\p",
                "\\v 18 Kipahawe bambari au manguli Kerisitu mohuru dosa manusia i karapapateNa i kau mombehape." +
                    "Ane tauna au monontohi naraka mohadi bambari iti, rauli: <<Iti lolita tontuli.>> Agayana " +
                    "ane ikita au nahoremake Pue Ala hangko i huku dosanta mohadi bambari iti, tauli: <<Itimi " +
                    "kuasana Pue Ala!>>",
                "\\v 19 I lalu Sura Malelaha, Pue Ala manguli nodeParagraph:",
                "\\q <<Ina Kupaope kapande manusia,",
                "\\q hai bara ina Kupake akalanda tauna au mapande.>>",
                "\\qr (Yesaya 29:14)",
                "\\p",
                "\\v 20 Apa bundunda tauna au mapande? Apa bundunda guru agama to Yahudi? Apa bundunda tauna au " +
                    "mapande mololita i lalu dunia ide? I peitana Pue Ala, ope-ope kapande manusia Naimba " +
                    "mbero-mbero pea.",
                "\\v 21 Lawi hangko i kapande manusia, ikita bara peisa moisa Pue Ala. Anti kamapandena Pue" +
                    "Ala, Ia mobotusi bona hema pea au mepoinalai i Ngkora Marasa au kipahawe, tauna iti ina " +
                    "Nahoremahe hangko i huku dosanda. Agayana ane kipahawe Ngkora Marasa, tauna au bara " +
                    "mepoinalai manguli: <<Iti lolita tontuli.>>"
            };
        #endregion
        #region m_vExpectedParatext_2
        string[] m_vExpectedParatext_2 =
            {
                "\\id 1CO",
                "\\h 1 Korintu",
                "\\mt2 Surana Suro Paulu au Nguru-nguruna",
                "\\mt2 i to Sarani i Kota",
                "\\mt Korintu",
                "\\p",
                "\\c 1",
                "\\v 1-2 Sura ide au hangko iriko Paulu hai hangko i Sostenes, kipakatu irikamu to " +
                    "Sarani i kota Korintu au nakakiokau Pue Ala mewali taunaNa. Iami au mopamalelahamokau " +
                    "anti pohintuwumi hai Kerisitu Yesu hihimbela hai ope-ope tauna au maida iumba pea au " +
                    "mekakae i Yesu Kerisitu, Amputa hai Ampunda. Iko ide, naangkana Pue Ala mewali surona " +
                    "Yesu Kerisitu moula peundeaNa.",
                "\\v 3 Ngkaya tabea! Mekakaena i Pue Ala Umanta hai i Pue Yesu Kerisitu bona Ia mowatikau " +
                    "hangko i kabulana laluNa hai moweikau roa ngkatuwo.",
                "\\s Paulu manguli ngkaya kamaroana i Pue Ala",
                "\\p",
                "\\v 4 Halalu, barana mengkangkaroo manguli ngkaya kamaroana i Pue Ala anti kaikamuna, lawi " +
                    "Ia moweikau pewati hangko i kabulana laluNa anti pohintuwumi hai Kerisitu Yesu.",
                "\\v 5 Anti pohintuwumi hai Kerisitu, Naweimokau hinangkana pewati au mobundu i katuwomi: " +
                    "Napopaisaamokau paturo au tou, hai Naweimokau kapande mopahawe paturo iti.",
                "\\v 6 Ope-ope iti mopakanoto kamarohona pepoinalaimi i bambari kana i Kerisitu au kipahaweakau,",
                "\\v 7 duuna barapi ara kamakurana pewati au nilambi. Nilambimi paka-pakana kapande hangko i " +
                    "Inao Malelaha, hai mampegiakau kahawena hule Amputa Yesu Kerisitu.",
                "\\v 8 Ia ina mopakaroho pepoinalaimi duuna i kahopoa dunia, bona ane ikamu nakahawei Amputa " +
                    "Yesu Kerisitu i kahaweNa hule, barapi ara kamasalami.",
                "\\v 9 Pue Ala mokakiokau bona mohintuwukau hihimbela hai AnaNa, Yesu Kerisitu Amputa, hai " +
                    "Ia mopabukei liliu dandiNa irikita.",
                "\\s Paulu mokambaroahe lawi barahe mohintuwu",
                "\\p",
                "\\v 10 Halalu, hangko i kuasa au naweina Amputa Yesu Kerisitu, merapina bona mohintuwukau " +
                    "ope-ope. Ineekau mopaara posisala. Hangangaa mohintuwukau hai hampepekirikau.",
                "\\v 11 Halalungku, tunggaiaku manguli nodo, lawi arahe halalunta hasou-souna Kloe au mopahawe " +
                    "iriko kaarana posisalami.",
                "\\v 12 Mololitana nodo lawi arakau au mampeinao mampopontani hadua hai hadua. Arakau au manguli: " +
                    "<<Iko meula i Paulu.>> Arakau au manguli: <<Iko meula i Apolo.>> Au ntanina wori manguli: " +
                    "<<Iko meula i Peturu.>>\\f + \\fr 1:12\\fr* Peturu - Basa Yunani: Kefas Peita " +
                    "wori 3:22; 9:5 hai 15:5. Peturu ba Simo Peturu rahanga wori Kefas.\\f* Ara mbuli au manguli: " +
                    "<<Iko meula i Kerisitu.>>",
                "\\v 13 Maroa pae ane ikamu au meula i Kerisitu mosisala nodo? Ba iko au mate rapaku i kau " +
                    "mohuru dosami? Ba rariukau bona mewalikau topegurungku? Bara!",
                "\\v 14 Kuuli ngkaya kamaroana lawi barakau ara au kuriu, batenda pea Krispus hai Gayu,",
                "\\v 15 bona datihe ara au manguli kuriukau mewali topegurungku.",
                "\\v 16 (Oo, Setepanu wori hantambi, iko mpuu au moriuhe. Agayana i raoa hangko irihira, barapi " +
                    "kukatuinao tauna ntanina au kuriu.)",
                "\\v 17 Kerisitu barana moangka mewali suroNa bona moriuna tauna, agayana Naangkana bona " +
                    "moantina Ngkora Marasa. Ane kupahawe Ngkora Marasa, barana mololita hangko i kamapandeku " +
                    "datihe ara tauna au mampoinalai lolitangku anti kamapandeku moleri barahe moisa kuasana " +
                    "Kerisitu au mate i kau mombehape.",
                "\\s Hangko i Ngkora Marasa taisa kuasana hai kamanotona laluna Pue Ala",
                "\\p",
                "\\v 18 Kipahawe bambari au manguli Kerisitu mohuru dosa manusia i karapapateNa i kau mombehape." +
                    "Ane tauna au monontohi naraka mohadi bambari iti, rauli: <<Iti lolita tontuli.>> Agayana " +
                    "ane ikita au nahoremake Pue Ala hangko i huku dosanta mohadi bambari iti, tauli: <<Itimi " +
                    "kuasana Pue Ala!>>",
                "\\v 19 I lalu Sura Malelaha, Pue Ala manguli nodeParagraph:",
                "\\q <<Ina Kupaope kapande manusia,",
                "\\q hai bara ina Kupake akalanda tauna au mapande.>>",
                "\\p",
                "\\v 20 Apa bundunda tauna au mapande? Apa bundunda guru agama to Yahudi? Apa bundunda tauna au " +
                    "mapande mololita i lalu dunia ide? I peitana Pue Ala, ope-ope kapande manusia Naimba " +
                    "mbero-mbero pea.",
                "\\v 21 Lawi hangko i kapande manusia, ikita bara peisa moisa Pue Ala. Anti kamapandena Pue" +
                    "Ala, Ia mobotusi bona hema pea au mepoinalai i Ngkora Marasa au kipahawe, tauna iti ina " +
                    "Nahoremahe hangko i huku dosanda. Agayana ane kipahawe Ngkora Marasa, tauna au bara " +
                    "mepoinalai manguli: <<Iti lolita tontuli.>>"
            };
        #endregion
        #region m_vExpectedToolbox_2
        string[] m_vExpectedToolbox_2 =
            {
                "\\rcrd",
                "\\id 1CO",
                "\\rcrd 1",
                "\\h 1 Korintu",
                "\\st Surana Suro Paulu au Nguru-nguruna",
                "\\st i to Sarani i Kota",
                "\\mt Korintu",
                "\\p",
                "\\c 1",
                "\\v 1-2",
                "\\vt Sura ide au hangko iriko Paulu hai hangko i Sostenes, kipakatu irikamu to " +
                    "Sarani i kota Korintu au nakakiokau Pue Ala mewali taunaNa. Iami au mopamalelahamokau " +
                    "anti pohintuwumi hai Kerisitu Yesu hihimbela hai ope-ope tauna au maida iumba pea au " +
                    "mekakae i Yesu Kerisitu, Amputa hai Ampunda. Iko ide, naangkana Pue Ala mewali surona " +
                    "Yesu Kerisitu moula peundeaNa.",
                "\\v 3",
                "\\vt Ngkaya tabea! Mekakaena i Pue Ala Umanta hai i Pue Yesu Kerisitu bona Ia mowatikau " +
                    "hangko i kabulana laluNa hai moweikau roa ngkatuwo.",
                "\\s Paulu manguli ngkaya kamaroana i Pue Ala",
                "\\p",
                "\\v 4",
                "\\vt Halalu, barana mengkangkaroo manguli ngkaya kamaroana i Pue Ala anti kaikamuna, lawi " +
                    "Ia moweikau pewati hangko i kabulana laluNa anti pohintuwumi hai Kerisitu Yesu.",
                "\\v 5",
                "\\vt Anti pohintuwumi hai Kerisitu, Naweimokau hinangkana pewati au mobundu i katuwomi: " +
                    "Napopaisaamokau paturo au tou, hai Naweimokau kapande mopahawe paturo iti.",
                "\\v 6",
                "\\vt Ope-ope iti mopakanoto kamarohona pepoinalaimi i bambari kana i Kerisitu au kipahaweakau,",
                "\\v 7",
                "\\vt duuna barapi ara kamakurana pewati au nilambi. Nilambimi paka-pakana kapande hangko i " +
                    "Inao Malelaha, hai mampegiakau kahawena hule Amputa Yesu Kerisitu.",
                "\\v 8",
                "\\vt Ia ina mopakaroho pepoinalaimi duuna i kahopoa dunia, bona ane ikamu nakahawei Amputa " +
                    "Yesu Kerisitu i kahaweNa hule, barapi ara kamasalami.",
                "\\v 9",
                "\\vt Pue Ala mokakiokau bona mohintuwukau hihimbela hai AnaNa, Yesu Kerisitu Amputa, hai " +
                    "Ia mopabukei liliu dandiNa irikita.",
                "\\rcrd 2",
                "\\s Paulu mokambaroahe lawi barahe mohintuwu",
                "\\p",
                "\\v 10",
                "\\vt Halalu, hangko i kuasa au naweina Amputa Yesu Kerisitu, merapina bona mohintuwukau " +
                    "ope-ope. Ineekau mopaara posisala. Hangangaa mohintuwukau hai hampepekirikau.",
                "\\v 11",
                "\\vt Halalungku, tunggaiaku manguli nodo, lawi arahe halalunta hasou-souna Kloe au mopahawe " +
                    "iriko kaarana posisalami.",
                "\\v 12",
                "\\vt Mololitana nodo lawi arakau au mampeinao mampopontani hadua hai hadua. Arakau au manguli: " +
                    "<<Iko meula i Paulu.>> Arakau au manguli: <<Iko meula i Apolo.>> Au ntanina wori manguli: " +
                    "<<Iko meula i Peturu.>>|fn Ara mbuli au manguli: " +
                    "<<Iko meula i Kerisitu.>>",
                "\\ft 1:12 Peturu - Basa Yunani: Kefas Peita " +
                    "wori 3:22; 9:5 hai 15:5. Peturu ba Simo Peturu rahanga wori Kefas.",
                "\\v 13",
                "\\vt Maroa pae ane ikamu au meula i Kerisitu mosisala nodo? Ba iko au mate rapaku i kau " +
                    "mohuru dosami? Ba rariukau bona mewalikau topegurungku? Bara!",
                "\\v 14",
                "\\vt Kuuli ngkaya kamaroana lawi barakau ara au kuriu, batenda pea Krispus hai Gayu,",
                "\\v 15",
                "\\vt bona datihe ara au manguli kuriukau mewali topegurungku.",
                "\\v 16",
                "\\vt (Oo, Setepanu wori hantambi, iko mpuu au moriuhe. Agayana i raoa hangko irihira, barapi " +
                    "kukatuinao tauna ntanina au kuriu.)",
                "\\v 17",
                "\\vt Kerisitu barana moangka mewali suroNa bona moriuna tauna, agayana Naangkana bona " +
                    "moantina Ngkora Marasa. Ane kupahawe Ngkora Marasa, barana mololita hangko i kamapandeku " +
                    "datihe ara tauna au mampoinalai lolitangku anti kamapandeku moleri barahe moisa kuasana " +
                    "Kerisitu au mate i kau mombehape.",
                "\\rcrd 3",
                "\\s Hangko i Ngkora Marasa taisa kuasana hai kamanotona laluna Pue Ala",
                "\\p",
                "\\v 18",
                "\\vt Kipahawe bambari au manguli Kerisitu mohuru dosa manusia i karapapateNa i kau mombehape." +
                    "Ane tauna au monontohi naraka mohadi bambari iti, rauli: <<Iti lolita tontuli.>> Agayana " +
                    "ane ikita au nahoremake Pue Ala hangko i huku dosanta mohadi bambari iti, tauli: <<Itimi " +
                    "kuasana Pue Ala!>>",
                "\\v 19",
                "\\vt I lalu Sura Malelaha, Pue Ala manguli nodeParagraph:",
                "\\q",
                "\\vt <<Ina Kupaope kapande manusia,",
                "\\q",
                "\\vt hai bara ina Kupake akalanda tauna au mapande.>>",
                "\\qr (Yesaya 29:14)",
                "\\p",
                "\\v 20",
                "\\vt Apa bundunda tauna au mapande? Apa bundunda guru agama to Yahudi? Apa bundunda tauna au " +
                    "mapande mololita i lalu dunia ide? I peitana Pue Ala, ope-ope kapande manusia Naimba " +
                    "mbero-mbero pea.",
                "\\v 21",
                "\\vt Lawi hangko i kapande manusia, ikita bara peisa moisa Pue Ala. Anti kamapandena Pue" +
                    "Ala, Ia mobotusi bona hema pea au mepoinalai i Ngkora Marasa au kipahawe, tauna iti ina " +
                    "Nahoremahe hangko i huku dosanda. Agayana ane kipahawe Ngkora Marasa, tauna au bara " +
                    "mepoinalai manguli: <<Iti lolita tontuli.>>"                
            };
        #endregion
        #endregion

        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion
        #region Method: void CompareTransforms(ScriptureDB DB, string[] vDataExpected)
        void CompareTransforms(ScriptureDB DB, string[] vDataExpected)
        {
            string[] vDataActual = DB.ExtractData();
            Assert.AreEqual(vDataExpected.Length, vDataActual.Length);
            for (int i = 0; i < vDataExpected.Length; i++)
                Assert.AreEqual(vDataExpected[i], vDataActual[i]);
        }
        #endregion

        // General Tests ---------------------------------------------------------------------
        #region TEST: RemoveShoeboxSpecialMarkers
        [Test] public void RemoveShoeboxSpecialMarkers()
        {
            string[] vDataIn =
                {
                    "\\_sh v3.0  540  SHW-Scripture",
                    "\\_DateStampHasFourDigitYear",
                    "",
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\mt Markus",
                    "\\btmt Markus"
                };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\mt Markus",
                    "\\btmt Markus"
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);

            XForm_ShoeboxSpecialMarkers xform = new XForm_ShoeboxSpecialMarkers(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: NormalizeMarkers
        [Test] public void NormalizeMarkers()
        {
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\mt2 Buku",
                    "\\btst The Book of",
                    "\\mt1 Markus",
                    "\\btmt Markus",
                    "\\ov Just a random old version",
                    "\\nq Just a random note"
               };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\st Buku",
                    "\\btst The Book of",
                    "\\mt Markus",
                    "\\btmt Markus",
                    "\\ov Just a random old version",
                    "\\nt Just a random note"
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);

            XForm_NormalizeMarkers xform = new XForm_NormalizeMarkers(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: RemoveIgnoredMarkers
        [Test] public void RemoveIgnoredMarkers()
        {
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
                    "\\tb This is how it looks in the TB version.",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1",
                    "\\e"
               };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1"
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);

            XForm_RemoveIgnoredMarkers xform = new XForm_RemoveIgnoredMarkers(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: MoveChaptersIntoParagraphs
        [Test] public void MoveChaptersIntoParagraphs()
        {
            #region DATA SET
            string[] vDataIn1 =
                {
                    "\\rcrd mrk",
                    "\\c 2",
                    "\\ms This is a Major Section Head",
                    "\\mr (major cross reference)",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1"
               };
            string[] vDataExpected1 =
                {
                    "\\rcrd mrk",
                    "\\ms This is a Major Section Head",
                    "\\mr (major cross reference)",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
                    "\\c 2",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1"
               };
            #endregion

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn1);

            XForm_MoveChapterPosition xform = new XForm_MoveChapterPosition(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected1);
        }
        #endregion
        #region TEST: EnsureParagraphAfterPicture
        [Test] public void EnsureParagraphAfterPicture()
        {
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja,",
                    "\\cat PictureFileName.gif",
                    "\\cap This is a picture",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu."
               };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja,",
                    "\\cat PictureFileName.gif",
                    "\\cap This is a picture",
				    "\\p",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);

            XForm_EnsureParagraphAfterPicture xform = new XForm_EnsureParagraphAfterPicture(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: NormalizedMarkersInventory
        [Test] public void NormalizedMarkersInventory()
        {
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize( SectionTestData.BaikenoMark0101_ImportVariant );
            DB.TransformIn();

            ArrayList aMarkers = DB.GetNormalizedMarkersInventory();

            Assert.AreNotEqual(-1, aMarkers.BinarySearch("c"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("v"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("vt"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("s"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("nt"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("s"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("r"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("q"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("cf"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("ft"));

            Assert.AreEqual(-1, aMarkers.BinarySearch("btvt"));
            Assert.AreEqual(-1, aMarkers.BinarySearch("bts"));
        }
        #endregion
        #region TEST: UnknownMarkersInventory
        [Test] public void UnknownMarkersInventory()
        // We want to make certain that all of the markers we think are
        // in the Map are indeed in the map. The easiest way to do this is
        // to test all of the DSection test strings, and verify that
        // the resultant Inventory shows up as empty.
        //
        // But first, we test to make sure that the method does indeed
        // return something ifi there are bad markers present!
        {
            // Test that some wrong markers do indeed show up
            #region TestData
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\c 2",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\qw",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
                    "\\pr Should not be here, either.",
				    "\\cf 1:2: Maleakhi 3:1"
               };
            #endregion
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB.TransformIn();
            ArrayList a = DB.GetUnknownMarkersInventory();
            Assert.AreNotEqual(-1, a.BinarySearch("qw"));
            Assert.AreNotEqual(-1, a.BinarySearch("pr"));

            // Test all of the DSection data to show that none of those markers
            // are Unknown.
            string[][] v = new string[10][];
            v[0] = SectionTestData.BaikenoMark0101_ImportVariant;
            v[1] = SectionTestData.BaikenoMark0109_ImportVariant;
            v[2] = SectionTestData.BaikenoMark16_ImportVariant; 
            v[3] = SectionTestData.BaikenoMark430_ImportVariant;
            v[4] = SectionTestData.PuraMark14_ImportVariant; 
            v[5] = SectionTestData.HelongActs04_ImportVariant;
            v[6] = SectionTestData.HelongActs0754_ImportVariant;
            v[7] = SectionTestData.HelongRev0104_ImportVariant;
            v[8] = SectionTestData.ManadoMark013014_ImportVariant;
            v[9] = SectionTestData.TombuluActs009032_ImportVariant;
            for (int i = 0; i < v.Length; i++)
            {
                DB = new ScriptureDB();
                DB.Initialize(v[i]);
                DB.TransformIn();
                a = DB.GetUnknownMarkersInventory();
                Assert.AreEqual(0, a.Count);
            }
        }
        #endregion
        #region TEST: MoveTitlesIntoRecord
        [Test] public void MoveTitlesIntoRecord()
        {
            #region Data: Bwazza, with a few extra fields added to the Intro section
            string[] vIn_Bwazza =
			{
                "\\rcrd",
                "\\h Luk",
                "\\st Mǝlikakar",
                "\\mt Mi Luk",
                "\\id LUK",
                "\\nt This is a note.",
                "\\nt And this is another note.",
                "\\cy Copyright statement.",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            string[] vExpected_Bwazza =
			{
                "\\rcrd",
                "\\id LUK",
                "\\nt This is a note.",
                "\\nt And this is another note.",
                "\\cy Copyright statement.",
                "\\rcrd 1",
                "\\h Luk",
                "\\st Mǝlikakar",
                "\\mt Mi Luk",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            #endregion
            #region Data: Amarasi - example of Toolbox, but with \_... fields removed
            // Chuck: Indonesia, Amarasi, Luke beginning
            string[] vIn_Amarasi = 
			{
                "\\rcrd mrk",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            string[] vExpected_Amarasi = 
			{
                "\\rcrd mrk",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_Bwazza, vExpected_Bwazza);
            AddStringSet(1, vIn_Amarasi, vExpected_Amarasi);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.Format = ScriptureDB.Formats.kToolbox;

                // Move the fields to the second record; they should line up with
                // our "Expected" version.
                XForm_MoveTitleFields xform = new XForm_MoveTitleFields(DB);
                xform.OnImport();
                CompareTransforms(DB, m_vvExpected[i]);

                // Move them back to the first record; they should now compare with
                // the original "In" version.
                xform.OnExport();
                CompareTransforms(DB, m_vvIn[i]);
            }
        }
        #endregion
        #region TEST: CombineBTs
        [Test] public void CombineBTs()
        {
            // Test Data
            #region string[] vDataIn
            string[] vDataIn =
			{
				"\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				"\\bts Yohanis, the.One.who.habitually *Baptizes, opens the way/path " +
					"for the Lord Yesus",
				"\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
				"\\p",
				"\\v 2",
				"\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					"orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					"Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					"jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				"\\btvt Yesus had not yet begun His work, {te} God had ordered (to do " +
					"s.t.) a person, his name was Yohanis.  Yohanis had to go prepare " +
					"the way for Yesus' coming.  Cause long before, God had already " +
					"used His *prophet1 (spokesperson). His name was Grandfather " +
					"Yesaya. He actually wrote (= wrote before the event) saying:",
				"\\q",
				"\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				"\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					"open the way for You.",
				"\\cf 1:2: Maleakhi 3:1",
				"\\cat c:\\graphics\\HK-LB\\LB00296C.tif",
				"\\ref width:5.0cm;textWrapping:around;horizontalPosition:right",
				"\\cap Yohanis, Tukang Sarani",
				"\\btcap Yohanis, The Habitual Baptizer",
				"\\p",
				"\\vt Abis dong mangaku sala, ju dia sarani sang dong di kali Yarden.",
				"\\btvt After they *confessed their *sins,  then he *baptised them " +
					"in the Yarden stream/river.",
				"\\nt calop ko sarani ; sunge (implies always water) vs. kali (can " +
					"have water or be just the bed) > kali mati  and parigi mati = dry"
			};
            #endregion
            #region string[] vDataExpected
            string[] vDataExpected =
			{
                "",                         // This is from the inserted "\rcrd" marker
				"1",                        // This "1" is from the inserted "\rcrd 1" marker
                "",                         // This is from the inserted "\mt" marker
				"Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				"(Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
				"",
				"2",
				"Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					"orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					"Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					"jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				"",
				"<<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				"1:2: Maleakhi 3:1",
				"c:\\graphics\\HK-LB\\LB00296C.tif",
				"width:5.0cm;textWrapping:around;horizontalPosition:right",
				"Yohanis, Tukang Sarani",
				"",
				"Abis dong mangaku sala, ju dia sarani sang dong di kali Yarden.",
				"calop ko sarani ; sunge (implies always water) vs. kali (can " +
					"have water or be just the bed) > kali mati  and parigi mati = dry"
			};
            #endregion
            #region string[] vBTExpected
            string[] vBTExpected =
			{
                "",                         // This is from the inserted "\rcrd" marker
				"",                         // This "" is from the inserted "\rcrd 1" marker
                "",                         // This is from the inserted "\m" marker
				"Yohanis, the.One.who.habitually *Baptizes, opens the way/path " +
					"for the Lord Yesus",
				"",
				"",
				"",
				"Yesus had not yet begun His work, {te} God had ordered (to do " +
					"s.t.) a person, his name was Yohanis.  Yohanis had to go prepare " +
					"the way for Yesus' coming.  Cause long before, God had already " +
					"used His *prophet1 (spokesperson). His name was Grandfather " +
					"Yesaya. He actually wrote (= wrote before the event) saying:",
				"",
				"\"Listen! I am ordering/commanding a person of Mine, to go " +
					"open the way for You.",
				"",
				"",
				"",
				"Yohanis, The Habitual Baptizer",
				"",
				"After they *confessed their *sins,  then he *baptised them " +
					"in the Yarden stream/river.",
				""
			};
            #endregion

            // Do the Transform process
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB.TransformIn();

            // Test the results
            for (int i = 0; i < DB.Count; i++)
            {
                SfField f = DB.GetFieldAt(i);

                Assert.AreEqual(vDataExpected[i], f.Data);
                Assert.AreEqual(vBTExpected[i], f.BT);
            }
        }
        #endregion
        #region TEST: MoveMajorSectionParagraphs
        [Test] public void MoveMajorSectionParagraphs()
        {
            string[] vDataIn =
                {
                    "\\rcrd JER 001",
                    "\\s Jeremiah's Prayer",
                    "\\p",
                    "\\v 1",
                    "\\vt I know, Lord, that our lives are not our own.",
                    "\\ms Judah in Trouble",
                    "\\mr (Psalm 123:5)",
                    "\\rcrd JER 002",
                    "\\s Judah's Broken Covenant",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt The Lord gave another message to Jeremiah."
               };
            string[] vDataExpected =
                {
                    "\\rcrd JER 001",
                    "\\s Jeremiah's Prayer",
                    "\\p",
                    "\\v 1",
                    "\\vt I know, Lord, that our lives are not our own.",
                    "\\rcrd JER 002",
                    "\\ms Judah in Trouble",
                    "\\mr (Psalm 123:5)",
                    "\\s Judah's Broken Covenant",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt The Lord gave another message to Jeremiah."
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);

            XForm_MoveMajorSectionParagraphs xform = new XForm_MoveMajorSectionParagraphs(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: IBT_SimpleIO
        [Test] public void IBT_SimpleIO()
        {
            // Create a single SfField with a BT and an IBT
            ScriptureDB db = new ScriptureDB();
            SfField f = new SfField("vt", "Translation", "Back Translation", "Interlinear");
            db.Format = ScriptureDB.Formats.kToolbox;
            db.Append(f);

            XForm_CombinedBTs xform = new XForm_CombinedBTs(db);

            // Separate it into individual fields and check the result
            xform.OnExport();
            Assert.AreEqual(3, db.Count);
            Assert.AreEqual("vt", db.GetFieldAt(0).Mkr);
            Assert.AreEqual("Translation", db.GetFieldAt(0).Data);

            Assert.AreEqual("btvt", db.GetFieldAt(1).Mkr);
            Assert.AreEqual("Back Translation", db.GetFieldAt(1).Data);

            Assert.AreEqual("ibtvt", db.GetFieldAt(2).Mkr);
            Assert.AreEqual("Interlinear", db.GetFieldAt(2).Data);

            // Combine them again and check the result
            xform.OnImport();
            Assert.AreEqual(1, db.Count);
            Assert.AreEqual("vt", db.GetFieldAt(0).Mkr);
            Assert.AreEqual("Translation", db.GetFieldAt(0).Data);
            Assert.AreEqual("Back Translation", db.GetFieldAt(0).BT);
            Assert.AreEqual("Interlinear", db.GetFieldAt(0).IBT);
        }
        #endregion
		#region Test: GetBookAbbrevFromIdLine
		[Test] public void GetBookAbbrevFromIdLine()
		{
			// Create a DB from the sample data; this one is from Luke
			// "\id LUK - Tera",
			ScriptureDB DB = new ScriptureDB();
			DB.Initialize(m_vIn_1);
			DB.TransformIn();

			// The Abbrev should be LUK
			string sAbbrev = DB.GetAbbrevFromIdLine();
			Assert.AreEqual("LUK", sAbbrev);
		}
		#endregion
        #region Test: void ParseFootnotes()
        [Test] public void ParseFootnotes()
        {
            string sReference;
            string sContents;
            XForm_HandleParatextSeeAlsosAndFootnotes.ParseFootnote(
                "2:20 Yesus aurata irati anya bauname umaso ai.", out sReference, out sContents);
            Assert.AreEqual("2:20", sReference);
            Assert.AreEqual("Yesus aurata irati anya bauname umaso ai.", sContents);
        }
        #endregion

        // Tests developed when adding the Paratext Import (some applies to Toolbox as well)
        #region TEST: NormalizeVerseText
        [Test] public void NormalizeVerseText()
        {
            #region Data: Koya - plain, Unicode special chars
            // Steven Payne, Africa, DRC, Koya, Luke 1:19-20
            string[] vIn_Koya =
                {
                    "\\p",
                    "\\v 19 Kija malaika wa:mutisio bɔ: «Ɛmɛ ma Gabhilieli. Ɛmɛ kʉ " +
                        "maakyɨa bʉmaɨ apɛɛ ʉ Asʉbɨɨ. Ɨyɔ ʉnatʉma nʉwɛ kʉbɨa ɨsaʉ yɛɛ " +
                        "ido ngia.",
                    "\\v 20 na, moni ngika kʉsɛmɛ ka=kyɨanɨa akʉ wati wɔngɔ. Ndʉ wʉbhaya " +
                        "kɔmʉ moni ngika kʉsɛmɛ, wa=bio tʉ kʉ ijangi; wambukubio tii bata " +
                        "kʉyaka kʉbʉya bisi ongo tʉ kʉ isiya moni ngikʉnanɨ ka=kyɨanɨa-ɔɔ.»"
                };
            string[] vExpected_Koya =
                {
                    "\\p",
                    "\\v 19",
                    "\\vt Kija malaika wa:mutisio bɔ: «Ɛmɛ ma Gabhilieli. Ɛmɛ kʉ " +
                        "maakyɨa bʉmaɨ apɛɛ ʉ Asʉbɨɨ. Ɨyɔ ʉnatʉma nʉwɛ kʉbɨa ɨsaʉ yɛɛ " +
                        "ido ngia.",
                    "\\v 20",
                    "\\vt na, moni ngika kʉsɛmɛ ka=kyɨanɨa akʉ wati wɔngɔ. Ndʉ wʉbhaya " +
                        "kɔmʉ moni ngika kʉsɛmɛ, wa=bio tʉ kʉ ijangi; wambukubio tii bata " +
                        "kʉyaka kʉbʉya bisi ongo tʉ kʉ isiya moni ngikʉnanɨ ka=kyɨanɨa-ɔɔ.»"
                };
            #endregion
            #region Data: Bwazza - contains a footnote
            // Katy Barnwell, Africa, Nigeria, Bwazza, Luke 1:11-18
            string[] vIn_Bwazza =
                {
                    "\\p",
                    "\\v 11 Nama mǝturanjar mi Ɓakuli pusuri aki, yi shama a bu mǝ li nǝ " +
                        "bama akǝ losǝna yolo peri.",
                    "\\v 12 Shina ma Zakariya shini, gri payi ndali wara wo pai.",
                    "\\v 13 Mǝ-turanjar nayi ama, “Ɓǝ wo ɓǝka payo wa, Zakariya, a ona jime " +
                        "molo. Mamo Alisabatu na ɓǝl muna ɓwabura, wara wa shiyi lulu ama " +
                        "Yohanna.",
                    "\\v 14 Ɓǝl muna ma na yinǝwo nǝ bumpwasa nǝ baɓwarashau, wara a ɓwai pas " +
                        "na pa baɓwarashau nǝ ɓǝlban mali.",
                    "\\v 15 Asalama ya gulo a dum Ɓakuli. Yika nu mur ɓla ngun inabi wa, sǝ " +
                        "yika nu mba wa, wara Lilim mi Ɓakuli na paturo a nyami kla ama ana ɓli.",
                    "\\v 16 Ya nyasa amǝ Israila pas aba ya ki mǝtelasǝm Ɓakuli.",
                    "\\v 17 Wara ya a dǝma a dum mǝtelasǝm, aɓa lilim nǝ rǝshana mi " +
                        "Iliya\\f + \\fr 1:17 \\ft shin Mal./Mal. 4:5-6\\ft*\\f*, ya pǝl ɓabuma " +
                        "teru akya muniya, wara amǝ shankrǝu aki shirǝɗenye mya amǝ ɓabum ɓwara, " +
                        "wara ya gǝlka ɓwampine aba ka ki Mǝtelasǝm.",
                    "\\v 18 Zakariya iulu mǝ-turanjare ama, \"Man palang sǝ man shile ama ma " +
                        "yi gǝr na pa? Ma ɓwagǝla sǝ mamon sheura kǝm.”"
                };
            string[] vExpected_Bwazza =
                {
                    "\\p",
                    "\\v 11",
                    "\\vt Nama mǝturanjar mi Ɓakuli pusuri aki, yi shama a bu mǝ li nǝ " +
                        "bama akǝ losǝna yolo peri.",
                    "\\v 12",
                    "\\vt Shina ma Zakariya shini, gri payi ndali wara wo pai.",
                    "\\v 13",
                    "\\vt Mǝ-turanjar nayi ama, “Ɓǝ wo ɓǝka payo wa, Zakariya, a ona jime " +
                        "molo. Mamo Alisabatu na ɓǝl muna ɓwabura, wara wa shiyi lulu ama " +
                        "Yohanna.",
                    "\\v 14",
                    "\\vt Ɓǝl muna ma na yinǝwo nǝ bumpwasa nǝ baɓwarashau, wara a ɓwai pas " +
                        "na pa baɓwarashau nǝ ɓǝlban mali.",
                    "\\v 15",
                    "\\vt Asalama ya gulo a dum Ɓakuli. Yika nu mur ɓla ngun inabi wa, sǝ " +
                        "yika nu mba wa, wara Lilim mi Ɓakuli na paturo a nyami kla ama ana ɓli.",
                    "\\v 16",
                    "\\vt Ya nyasa amǝ Israila pas aba ya ki mǝtelasǝm Ɓakuli.",
                    "\\v 17",
                    "\\vt Wara ya a dǝma a dum mǝtelasǝm, aɓa lilim nǝ rǝshana mi " +
                        "Iliya\\f + \\fr 1:17 \\ft shin Mal./Mal. 4:5-6\\ft*\\f*, ya pǝl ɓabuma " +
                        "teru akya muniya, wara amǝ shankrǝu aki shirǝɗenye mya amǝ ɓabum ɓwara, " +
                        "wara ya gǝlka ɓwampine aba ka ki Mǝtelasǝm.",
                    "\\v 18",
                    "\\vt Zakariya iulu mǝ-turanjare ama, \"Man palang sǝ man shile ama ma " +
                        "yi gǝr na pa? Ma ɓwagǝla sǝ mamon sheura kǝm.”"
                };
            #endregion
            #region Data: Napu - contains a verse bridge (\v 3-4)
            // Roger Hanna, Asia, Indonesia, Napu, 1 Cor 5:1-5
            string[] vIn_Napu =
                {
                    "\\s Paulu motuduhe mohuku hadua tauna au mampotambia towawinena umana",
                    "\\p",
                    "\\v 1 Ara kuhadi au mololita kana i babehianda tauna i olomi au bara " +
                        "hintoto. Ara hadua rangami au mampotambia towawinena umana. Mogalori " +
                        "i olonda tauna au bara moisa Pue Ala, barahe ara au mobabehi nodo.",
                    "\\v 2 Agayana ikamu, nauri ara au mobabehi nodo i olomi, mampemahile " +
                        "manikau! Katouana, hangangaa masusa lalumi, hai tauna iti hangangaa " +
                        "nipopeloho hangko i olomi.",
                    "\\v 3-4 Nauri karaona hangko irikamu, agayana lalungku arato hihimbela " +
                        "hai ikamu, hai ara mpuu kuasangku mampohawaakau. Mewali, hai kuasa " +
                        "au naweina Pue Yesu Kerisitu, kupakanotomi: tauna au kadake babehiana " +
                        "iti hangangaa rahuku. Ane mogulukau, niimbana hihimbela hai ikamu, " +
                        "hai kuperapi i lalu kuasana Yesu Amputa",
                    "\\v 5 bona nihuhu pea tauna iti i kuasana Datu Tokadake. Lempona, " +
                        "nitudumi meloho hangko i olomi, hai inee nipaliu maminggu sambela hai " +
                        "ikamu. Hangangaa nibabehi nodo, bona watana tauna iti molambi " +
                        "pehuku agayana inaona molambi katuwo maroa i kahawena hule Pue Yesu."
                };
            string[] vExpected_Napu =
                {
                    "\\s Paulu motuduhe mohuku hadua tauna au mampotambia towawinena umana",
                    "\\p",
                    "\\v 1",
                    "\\vt Ara kuhadi au mololita kana i babehianda tauna i olomi au bara " +
                        "hintoto. Ara hadua rangami au mampotambia towawinena umana. Mogalori " +
                        "i olonda tauna au bara moisa Pue Ala, barahe ara au mobabehi nodo.",
                    "\\v 2",
                    "\\vt Agayana ikamu, nauri ara au mobabehi nodo i olomi, mampemahile " +
                        "manikau! Katouana, hangangaa masusa lalumi, hai tauna iti hangangaa " +
                        "nipopeloho hangko i olomi.",
                    "\\v 3-4",
                    "\\vt Nauri karaona hangko irikamu, agayana lalungku arato hihimbela " +
                        "hai ikamu, hai ara mpuu kuasangku mampohawaakau. Mewali, hai kuasa " +
                        "au naweina Pue Yesu Kerisitu, kupakanotomi: tauna au kadake babehiana " +
                        "iti hangangaa rahuku. Ane mogulukau, niimbana hihimbela hai ikamu, " +
                        "hai kuperapi i lalu kuasana Yesu Amputa",
                    "\\v 5",
                    "\\vt bona nihuhu pea tauna iti i kuasana Datu Tokadake. Lempona, " +
                        "nitudumi meloho hangko i olomi, hai inee nipaliu maminggu sambela hai " +
                        "ikamu. Hangangaa nibabehi nodo, bona watana tauna iti molambi " +
                        "pehuku agayana inaona molambi katuwo maroa i kahawena hule Pue Yesu."
                };
            #endregion
            #region Data: VerseBridges - contains various verse bridges
            // VerseBridges - contains various verse bridges
            string[] vIn_VerseBridges =
                {
                    "\\v 1-2 Ara kuhadi.",
                    "\\v 3-5a Agayana ikamu.",
                    "\\v 5b - 7 Nauri karaona.",
                    "\\v 8bona nihuhu.",
                    "\\v 9-11abona nihuhu .",
                    "\\v 9-11a bona nihuhu.",
                    "\\v 9-11 abona nihuhu.",
                    "\\v 3,4 Duo",
                    "\\v 3,4,5 Combine",
                    "\\v 3, 4, 5 Combine",
                    "\\v 9-11 (Punct)",
                    "\\v 1,2 Orait"
                };
            string[] vExpected_VerseBridges =
                {
                    "\\v 1-2",
                    "\\vt Ara kuhadi.",
                    "\\v 3-5a",
                    "\\vt Agayana ikamu.",
                    "\\v 5b-7",
                    "\\vt Nauri karaona.",
                    "\\v 8",
                    "\\vt bona nihuhu.",
                    "\\v 9-11",
                    "\\vt abona nihuhu .",
                    "\\v 9-11a",
                    "\\vt bona nihuhu.",
                    "\\v 9-11",
                    "\\vt abona nihuhu.",
                    "\\v 3-4",
                    "\\vt Duo",
                    "\\v 3-5",
                    "\\vt Combine",
                    "\\v 3-5",
                    "\\vt Combine",
                    "\\v 9-11",
                    "\\vt (Punct)",
                    "\\v 1-2",
                    "\\vt Orait"
                 };
            #endregion
            #region Data: OriginalText - what I wrote in 2004
            // OriginalText - The test I wrote when I originally programmed this
            // functionality circa 2004
            string[] vIn_OriginalText = 
			{
				"\\v 3",
				"\\v (5a - 7b This is a test.)",
				"\\v 32b Howdy",
				"\\p"
			};
            string[] vExpected_OriginalText =
			{
				"\\v 3",
				"\\v 5a-7b",
				"\\vt (This is a test.)",
				"\\v 32b",
				"\\vt Howdy",
				"\\p"
			};
            #endregion

            // Set up the data
            InitializeStringSet(5);
            AddStringSet(0, vIn_Koya,         vExpected_Koya);
            AddStringSet(1, vIn_Bwazza,       vExpected_Bwazza);
            AddStringSet(2, vIn_Napu,         vExpected_Napu);
            AddStringSet(3, vIn_VerseBridges, vExpected_VerseBridges);
            AddStringSet(4, vIn_OriginalText, vExpected_OriginalText);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);

                XForm_NormalizeVerseText xform = new XForm_NormalizeVerseText(DB);
                xform.OnImport();
                // DB._NormalizeVerseText();

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: EnsureRecordsBeforeAllSections
        [Test] public void EnsureRecordsBeforeAllSections()
        {
            #region Data: Bwazza - several sections, verses prior to first \s
            // Katy: Africa, Nigeria, Bwazza, verses from the beginning of Luke.
            string[] vIn_Bwazza = 
			{
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            string[] vExpected_Bwazza =
			{
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            #endregion
            #region Data: Amarasi - example of Toolbox
            // Chuck: Indonesia, Amarasi, Luke beginning
            string[] vIn_Amarasi = 
			{
                "\\_sh v3.0  96  SHW-Scripture",
                "\\_DateStampHasFourDigitYear",
                "\\rcrd mrk",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_Bwazza, vExpected_Bwazza);
            AddStringSet(1, vIn_Amarasi, vIn_Amarasi);      // With Amarasi, I expect In == Out.

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                XForm_EnsureRecordsBeforeAllSections xform = new XForm_EnsureRecordsBeforeAllSections(DB);
                xform.OnImport();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: EnsureInitialTitleRecord
        [Test] public void EnsureInitialTitleRecord()
        {
            #region Data: Bwazza - several sections, verses prior to first \s
            // Katy: Africa, Nigeria, Bwazza, verses from the beginning of Luke.
            string[] vIn_Bwazza = 
			{
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            string[] vExpected_Bwazza =
			{
                "\\rcrd",
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            #endregion
            #region Data: Amarasi - example of Toolbox
            // Chuck: Indonesia, Amarasi, Luke beginning
            string[] vIn_Amarasi = 
			{
                "\\_sh v3.0  96  SHW-Scripture",
                "\\_DateStampHasFourDigitYear",
                "\\rcrd mrk",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            #endregion
            #region Data: Bwazza2 - This time missing any title information
            // Katy: Africa, Nigeria, Bwazza, verses from the beginning of Luke.
            string[] vIn_Bwazza2 = 
			{
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal."
			};
            string[] vExpected_Bwazza2 =
			{
                "\\rcrd",
                "\\mt",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal."
			};
            #endregion

            // Set up the data
            InitializeStringSet(3);
            AddStringSet(0, vIn_Bwazza, vExpected_Bwazza);
            AddStringSet(1, vIn_Amarasi, vIn_Amarasi);      // With Amarasi, I expect In == Out.
            AddStringSet(2, vIn_Bwazza2, vExpected_Bwazza2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);

                XForm_EnsureRecordsBeforeAllSections xform1 = new XForm_EnsureRecordsBeforeAllSections(DB);
                xform1.OnImport();

                XForm_EnsureInitialTitleRecord xform2 = new XForm_EnsureInitialTitleRecord(DB);
                xform2.OnImport();

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_SeeAlso
        [Test] public void ConvertParatext_SeeAlso()
        {
            // Create a database for the helper methods test
            ScriptureDB DBH = new ScriptureDB();
            DBH.Format = ScriptureDB.Formats.kParatext;
            XForm_HandleParatextSeeAlsosAndFootnotes xform = new XForm_HandleParatextSeeAlsosAndFootnotes(DBH);

            // Helper methods: Test IsUSFMSeeAlsoBegin
            int iLen;
            xform.IsUSFMSeeAlsoBegin(
                "\\rc 17.50: \\rt 2 Sam 21.19. \\rt* \\rc*", 0, out iLen);
            Assert.AreEqual(4, iLen);
            xform.IsUSFMSeeAlsoBegin(
                "\\x + \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            xform.IsUSFMSeeAlsoBegin(
                "\\x - \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            xform.IsUSFMSeeAlsoBegin(
                "\\x c \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            xform.IsUSFMSeeAlsoBegin(
                "\\x_ + \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(10, iLen);
            xform.IsUSFMSeeAlsoBegin(
                "\\x_  +  \\xo  17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(13, iLen);  // (test that it is correctly eating white space)

            // Melper methods: Test IsUSFMSeeAlsoEnd
            xform.IsUSFMSeeAlsoEnd("\\rt* \\rc*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            xform.IsUSFMSeeAlsoEnd("\\xt* \\x*", 0, out iLen);
            Assert.AreEqual(8, iLen);
            xform.IsUSFMSeeAlsoEnd("\\x*", 0, out iLen);
            Assert.AreEqual(3, iLen);

            // Helper methods: Test ExtractSeeAlso
            Assert.AreEqual("17.50: 2 Sam 21.19.",
                xform.ExtractSeeAlso("17.50: \\rt 2 Sam 21.19. "));
            Assert.AreEqual("17.50: 2 Sam 21.19.",
                xform.ExtractSeeAlso("17.50: \\xt 2 Sam 21.19. "));
            Assert.AreEqual("17.50: 2 Sam 21.19.",
                xform.ExtractSeeAlso("\\bd 17.50: \\*bd 2 Sam 21.19. "));

            // Data with actual verses that contain cross references
            #region Method: Data: Set 1 - From Hausa
            string[] vvIn_1 =
                {
                    "\\v 50 \\rc 17.50: \\rt 2 Sam 21.19. \\rt* \\rc* Ta haka, da majajjawa da " +
                        "dutse kawai Dawuda ya ci nasara a kan mutumin Filistin nan. Ya jefe shi " +
                        "ya kashe ba tare da takobi a hannunsa ba."
                };
            string[] vvExpected_1 =
                {
                    "\\v 50",
                    "\\vt",
                    "\\cf 17.50: 2 Sam 21.19.",
                    "\\vt Ta haka, da majajjawa da dutse kawai Dawuda ya ci nasara a kan mutumin " +
                        "Filistin nan. Ya jefe shi ya kashe ba tare da takobi a hannunsa ba."
                };
            #endregion
            #region Method: Data: Set 2 - From Solomans
            string[] vvIn_2 =
                {
                    "\\v 15 They stayed there until Herod died. And this thing made true what " +
                        "God said to the profet who wrote like this, “I call my child out " +
                        "from Ejipt.”\\x + \\xo 2:15 \\xt Hosea 11:1\\xt*\\x*"
                };
            string[] vvExpected_2 =
                {
                    "\\v 15",
                    "\\vt They stayed there until Herod died. And this thing made true what " +
                        "God said to the profet who wrote like this, “I call my child out " +
                        "from Ejipt.”",
                    "\\cf 2:15 Hosea 11:1"
                };
            #endregion
            #region Method: Data: Set 3 - From Solomans
            string[] vvIn_3 =
                {
                    "\\v 17 And don't have sex with the daughter or granddaughter of any woman " +
                        "that you have earlier had sex with. You may be having sex with a relative, " +
                        "and that would make you unclean.\\x - \\xo 18.17: \\xt Lv 20.14; Dt 27.23.\\x*"
                };
            string[] vvExpected_3 =
                {
                    "\\v 17",
                    "\\vt And don't have sex with the daughter or granddaughter of any woman " +
                        "that you have earlier had sex with. You may be having sex with a relative, " +
                        "and that would make you unclean.",
                    "\\cf 18.17: Lv 20.14; Dt 27.23."
                };
            #endregion
            #region Method: Data: Set 4 - From CEV
            string[] vvIn_4 =
                {
                    "\\v 8 “God promised Abraham and he told him, if his people circumcised/cut-show, " +
                        "they agree to his rock promise/covenant.\\x + \\xo 7:8 \\xt Jenesis 17:10-14\\xt*\\x* " +
                        "That is why Abraham circumcised Aesak, eight days after he was born. In the " +
                        "same way also, Aesak circumcised his son Jakob, and Jakob circumcised his twelve " +
                        "sons who they are our(in) ancestors."
                };
            string[] vvExpected_4 =
                {
                    "\\v 8",
                    "\\vt “God promised Abraham and he told him, if his people circumcised/cut-show, " +
                        "they agree to his rock promise/covenant.",
                    "\\cf 7:8 Jenesis 17:10-14",
                    "\\vt That is why Abraham circumcised Aesak, eight days after he was born. In the " +
                        "same way also, Aesak circumcised his son Jakob, and Jakob circumcised his twelve " +
                        "sons who they are our(in) ancestors."
                };
            #endregion
            #region Method: Data: Set 5
            string[] vvIn_5 =
                {
                    "\\v 9 The promise that God made with Abraham is this, “At the appointed time, " +
                        "I will come back to you, and your wife Sara will deliver one male " +
                        "child.”\\x + \\xo 9.9 \\xt Genesis 18:10\\xt*\\x* And that child is Aesak"
                };
            string[] vvExpected_5 =
                {
                    "\\v 9",
                    "\\vt The promise that God made with Abraham is this, “At the appointed time, " +
                        "I will come back to you, and your wife Sara will deliver one male " +
                        "child.”",
                    "\\cf 9.9 Genesis 18:10",
                    "\\vt And that child is Aesak"
                };
            #endregion

            // Set up the data
            InitializeStringSet(5);
            AddStringSet(0, vvIn_1, vvExpected_1);
            AddStringSet(1, vvIn_2, vvExpected_2);
            AddStringSet(2, vvIn_3, vvExpected_3);
            AddStringSet(3, vvIn_4, vvExpected_4);
            AddStringSet(4, vvIn_5, vvExpected_5);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.Format = ScriptureDB.Formats.kParatext;

                XForm_NormalizeVerseText xform2 = new XForm_NormalizeVerseText(DB);
                xform2.OnImport();

                XForm_HandleParatextSeeAlsosAndFootnotes xform3 = new XForm_HandleParatextSeeAlsosAndFootnotes(DB);
                xform3.OnImportSeeAlso();

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: MoveParagraphText
        [Test] public void MoveParagraphText()
        {
            #region Data
            string[] vIn = 
                {
                    "\\q2 When she cries, no one can comfort her, because her children " +
                       "are all dead.”"
                };

            string[] vExpected =
                {
                    "\\q2",
                    "\\vt When she cries, no one can comfort her, because her children " +
                       "are all dead.”"
                };
            #endregion

            // Set up the data
            InitializeStringSet(1);
            AddStringSet(0, vIn, vExpected);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                XForm_MoveParagraphText xform = new XForm_MoveParagraphText(DB);
                xform.OnImport();
                //DB._MoveParagraphText();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_Footnotes
        [Test] public void ConvertParatext_Footnotes()
        {
            // Data
            #region DATA Set 1
            string[] vIn_1 =
                {
                    "\\v 31",
                    "\\vt Vilevile kwa Siku ya hukumu, Malkia toka Kusini\\f j \\fr 11.31 \\fk Malkia " +
                        "toka Kusini: \\ft au wa Inchi ya Seba.\\f* atasimama na watu wa kizazi hiki na " +
                        "kuwashitaki, kwa sababu yeye alitoka mbali sana kuja kusikiliza maneno ya hekima " +
                        "ya mufalme Solomono. Hapa kuna yule anayekuwa mukubwa kupita Solomono!"
                };
            string[] vExpected_1 =
                {
                    "\\v 31",
                    "\\vt Vilevile kwa Siku ya hukumu, Malkia toka Kusini|fn atasimama na watu wa kizazi hiki na " +
                        "kuwashitaki, kwa sababu yeye alitoka mbali sana kuja kusikiliza maneno ya hekima " +
                        "ya mufalme Solomono. Hapa kuna yule anayekuwa mukubwa kupita Solomono!",
                    "\\ft 11.31 Malkia toka Kusini: au wa Inchi ya Seba."
                };
            #endregion
            #region DATA Set 2
            string[] vIn_2 =
                {
                    "\\v 4",
                    "\\vt Yesu anamúshuvya: «Si’biyandisirwi kwokuno: ‹Bitali byo’mundu ali mu’lya naaho, " +
                        "byo’bitumiri ali mu’ba mugumaana.›»\\f d \\fr 4:4 \\ft Buk 8:3 \\f*"
                };
            string[] vExpected_2 =
                {
                    "\\v 4",
                    "\\vt Yesu anamúshuvya: «Si’biyandisirwi kwokuno: ‹Bitali byo’mundu ali mu’lya naaho, " +
                        "byo’bitumiri ali mu’ba mugumaana.›»|fn",
                    "\\ft 4:4 Buk 8:3"
                };
            #endregion
            #region DATA Set 3 - five different ones/variations crammed into one verse!
            string[] vIn_3 =
                {
                    "\\v 51",
                    "\\vt Yibyo bigatondekera ku’muko gwa’Abeeri, halinde ukuhisa ku’muko " +
                        "gwa’Zakariya,\\f t \\fr 11:51 \\ft Mu’Mandiko Meeru ga’Bayahudi, úkayitwa " +
                        "ubwa mbere, âli riiri Abeeri; no’kayitwa ubuzinda, âli riiri muleevi Zakariya. " +
                        "Yibyo, wangabisoma mu’kitaabo kye’Ndondeko 4; na’mu’Bye’Byanya bya’kabiri " +
                        "24:20-22. \\f*\\f a \\fr 2.22 \\fk kutakaswa: \\ft Angalia Law 12.1-8.\\f* ulya " +
                        "úkayitirwa ha’kati ke’nyumba ya’Rurema\\f c \\fr 3:4-6 \\ft Isa 40:3-5 \\f* " +
                        "na’katanda\\f + \\fr 18:6 \\fr* ipima mngana i:nye n litili 11, " +
                        "ym 12. \\f* ko’kutangira kwa’matuulo. Ee! Namùbwira kwa’bandu ba’kino kibusi " +
                        "bo’bagabuuzibwa hi’gulu lya’yibyo byoshi.\\f + \\fr 10:12 \\fk Sodom \\ft shin " +
                        "Gen./Fara. 19:24-28\\ft*\\f*"
                };
            string[] vExpected_3 =
                {
                    "\\v 51",
                    "\\vt Yibyo bigatondekera ku’muko gwa’Abeeri, halinde ukuhisa ku’muko " +
                        "gwa’Zakariya,|fn|fn ulya úkayitirwa ha’kati ke’nyumba ya’Rurema|fn " +
                        "na’katanda|fn ko’kutangira kwa’matuulo. Ee! Namùbwira kwa’bandu ba’kino kibusi " +
                        "bo’bagabuuzibwa hi’gulu lya’yibyo byoshi.|fn",
                    "\\ft 11:51 Mu’Mandiko Meeru ga’Bayahudi, úkayitwa ubwa mbere, âli riiri Abeeri; " +
                        "no’kayitwa ubuzinda, âli riiri muleevi Zakariya. Yibyo, wangabisoma mu’kitaabo " +
                        "kye’Ndondeko 4; na’mu’Bye’Byanya bya’kabiri 24:20-22.",
                    "\\ft 2.22 kutakaswa: Angalia Law 12.1-8.",
                    "\\ft 3:4-6 Isa 40:3-5",
                    "\\ft 18:6 ipima mngana i:nye n litili 11, ym 12.",
                    "\\ft 10:12 Sodom shin Gen./Fara. 19:24-28"
                };
            #endregion
            #region DATA Set 4 - a footnote as part of a "\s" field
            string[] vIn_4 =
                {
                    "\\s Yuani akʉbɨakɨa Ɨsaʉ yɛɛ Ido\\f + \\fr 3:6 \\fr* Ɨsaya 40:3-5\\f*",
                    "\\r (Matayɔ 3:1-10; Malɨkɔ 1:3-8)",
                    "\\p",
                    "\\v 1",
                    "\\vt Tibhelio, ngama ɔɔ 'ʉja ʉ baLɔma, ambʉndaniso mugo kumi nʉ bɔkɔ akʉ bʉngama."
                };
            string[] vExpected_4 =
                {
                    "\\s Yuani akʉbɨakɨa Ɨsaʉ yɛɛ Ido|fn",
                    "\\ft 3:6 Ɨsaya 40:3-5",
                    "\\r (Matayɔ 3:1-10; Malɨkɔ 1:3-8)",
                    "\\p",
                    "\\v 1",
                    "\\vt Tibhelio, ngama ɔɔ 'ʉja ʉ baLɔma, ambʉndaniso mugo kumi nʉ bɔkɔ akʉ bʉngama."
                };
            #endregion
            #region DATA Set 5 - from Luang
            string[] vIn_5 =
                {
                    "\\v 6",
                    "\\vt Yohansi naniayarni de ra' nana la onta wullu me rewni de ra' nana la ha ulti me " +
                        "ya'anni-yemannu de kongoha nora gani turnu.\\f + 1:6 La' ler ululu de " +
                        "makwohorulu-ktatrulu Eli nhi'inde nnairi naniayarni mak emkadi.\\f*"
                };
            string[] vExpected_5 =
                {
                    "\\v 6",
                    "\\vt Yohansi naniayarni de ra' nana la onta wullu me rewni de ra' nana la ha ulti me " +
                        "ya'anni-yemannu de kongoha nora gani turnu.|fn",
                    "\\ft 1:6 La' ler ululu de makwohorulu-ktatrulu Eli nhi'inde nnairi naniayarni mak emkadi."                    
                };
            #endregion

            // Set up the data
            InitializeStringSet(5);
            AddStringSet(0, vIn_1, vExpected_1);
            AddStringSet(1, vIn_2, vExpected_2);
            AddStringSet(2, vIn_3, vExpected_3);
            AddStringSet(3, vIn_4, vExpected_4);
            AddStringSet(4, vIn_5, vExpected_5);

            // Do the test
            for (var i = 0; i < m_vvIn.Length; i++)
            {
                var DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);

                var xform = new XForm_HandleParatextSeeAlsosAndFootnotes(DB);
                xform.OnImportFootnotes();
                // DB._ImportParatextFootnotes();

                CompareTransforms(DB, m_vvExpected[i]);
            }

        }
        #endregion
        #region TEST: JoinParatextEmbeddedFields
        [Test] public void JoinParatextEmbeddedFields()
        {
            string[] vIn =
                {
                    "\\v 3",
                    "\\vt Da yaron ya cika kwana takwas, sai suka zo domin su yi masa kaciya.",
                    "\\f c \\fr 1.59",
                    "\\fk kaciya:\\fk* Dubi ma'anar wa?ansu kalmomi.",
                    "\\f* Da ma sun so ne su sa masa sunan babansa,"
                };
            string[] vExpected =
                {
                    "\\v 3",
                    "\\vt Da yaron ya cika kwana takwas, sai suka zo domin su yi masa kaciya. " +
                        "\\f c \\fr 1.59 \\fk kaciya:\\fk* Dubi ma'anar wa?ansu kalmomi. " +
                        "\\f* Da ma sun so ne su sa masa sunan babansa,"
                };

            // Set up the data
            InitializeStringSet(1);
            AddStringSet(0, vIn, vExpected);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);

                XForm_HandleParatextSeeAlsosAndFootnotes x = new XForm_HandleParatextSeeAlsosAndFootnotes(DB);
                x.JoinParatextEmbeddedFields(XForm_HandleParatextSeeAlsosAndFootnotes.ParatextFootnoteMkrsToJoin);
                //DB.JoinParatextEmbeddedFields(ScriptureDB.ParatextFootnoteMkrsToJoin);

                string[] vs = DB.ExtractData();
                //foreach (string s in vs)
                //    Console.WriteLine("<" + s + ">");

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_CompleteDataSets
        [Test] public void ConvertParatext_CompleteDataSets()
        {
            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, m_vIn_1, m_vExpectedToolbox_1);
            AddStringSet(1, m_vIn_2, m_vExpectedToolbox_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.TransformIn();

                /***
                if (i == 0)
                {
                    string[] vs = DB.ExtractData();
                    foreach (string s in vs)
                        Console.WriteLine("<" + s + ">");
                }
                ***/

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatextVerseBridges
        [Test] public void ConvertParatext_VerseBridges()
        {
            // Create an empty database for the test
            ScriptureDB DB = new ScriptureDB();

            Assert.AreEqual("2-4 Text", 
                XForm_ConvertParatextVerseBridges.ConvertParatextVerseBridge("2,3,4 Text"));
            Assert.AreEqual("3 Text", 
                XForm_ConvertParatextVerseBridges.ConvertParatextVerseBridge("3 Text"));
            Assert.AreEqual("12-13 Text", 
                XForm_ConvertParatextVerseBridges.ConvertParatextVerseBridge("12,13 Text"));
            Assert.AreEqual("9-13 Text", 
                XForm_ConvertParatextVerseBridges.ConvertParatextVerseBridge("9,13 Text"));
            Assert.AreEqual("4a-7b Text", 
                XForm_ConvertParatextVerseBridges.ConvertParatextVerseBridge("4a,5,6,7b Text"));
        }
        #endregion
        #region TEST: ConvertParatext_ImportFigures
        [Test] public void ConvertParatext_ImportFigures()
        {
            #region DATA Set 1
            string[] vIn_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. \\fig GW-64 Umuganda ahulukira " +
                        "ku' bangere (2:9)\\fig*"
                };
            string[] vExpected_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. ",
                    "\\cat Unknown",
                    "\\cap GW-64 Umuganda ahulukira ku' bangere (2:9)"
                };
            #endregion
            #region DATA Set 2
            string[] vIn_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; \\fig |GW64.jpg|span|||Umuganda ahulukira " +
                        "ku' bangere (2:9)\\fig*wakapita emo utetemeko."
                };
            string[] vExpected_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; ",
                    "\\cat GW64.jpg",
                    "\\ref span",
                    "\\cap Umuganda ahulukira ku' bangere (2:9)",
                    "\\vt wakapita emo utetemeko."
                };
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_1, vExpected_1);
            AddStringSet(1, vIn_2, vExpected_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);

                XForm_USFMPictures xform = new XForm_USFMPictures(DB);
                xform.OnImport();

                // WriteActualDataToConsole(DB, "Import Figures - " + i.ToString());

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_ExportFigures
        [Test] public void ConvertParatext_ExportFigures()
        {
            #region DATA Set 1
            string[] vIn_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. ",
                    "\\cat Unknown",
                    "\\cap GW-64 Umuganda ahulukira ku' bangere (2:9)"
                };
            string[] vExpected_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. ",
                    "\\fig |Unknown||||GW-64 Umuganda ahulukira ku' bangere (2:9)|\\fig*"
                };
            #endregion
            #region DATA Set 2
            string[] vIn_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; ",
                    "\\cat GW64.jpg",
                    "\\ref span",
                    "\\cap Umuganda ahulukira ku' bangere (2:9)",
                    "\\vt wakapita emo utetemeko."
                };
            string[] vExpected_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; ",
                    "\\fig |GW64.jpg|span|||Umuganda ahulukira ku' bangere (2:9)|\\fig*",
                    "\\vt wakapita emo utetemeko."
                };
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_1, vExpected_1);
            AddStringSet(1, vIn_2, vExpected_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.Format = ScriptureDB.Formats.kParatext;

                XForm_USFMPictures xform = new XForm_USFMPictures(DB);
                xform.OnExport();


               // (new ScriptureDB.USFM_Pictures(DB)).Export();

               // WriteActualDataToConsole(DB, "Export Figures - " + i.ToString());

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: EnsureSectionsExist
        [Test] public void EnsureSectionsExist()
        {
            #region DATA SET
            string[] vDataIn1 =
                {
                    "\\ms This is a Major Section Head",
                    "\\mr (major cross reference)",
 				    "\\p",
                    "\\c 1",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja.",
				    "\\q",
                    "\\c 2",
				    "\\v 1",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu."
               };
            string[] vDataExpected1 =
                {
                    "\\ms This is a Major Section Head",
                    "\\mr (major cross reference)",
                    "\\rcrd 1",
 				    "\\p",
                    "\\c 1",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja.",
                    "\\rcrd 2",
				    "\\q",
                    "\\c 2",
				    "\\v 1",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu."
               };
            #endregion

            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn1);

            var xform = new XForm_EnsureSectionsExist(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected1);
        }
        #endregion
        #region TEST: ConsolidateRunningHeaders
        [Test] public void ConsolidateRunningHeaders()
        {
            #region DATA SET
            string[] vDataIn1 =
                {
                    "\\h Mark",
                    "\\s First Section",
                    "\\h 1",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja.",
				    "\\s Second Section",
                    "\\h 2",
				    "\\v 2",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu."
               };
            string[] vDataExpected1 =
                {
                    "\\h Mark",
                    "\\s First Section",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja.",
				    "\\s Second Section",
				    "\\v 2",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu."
               };
            #endregion

            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn1);

            var xform = new XForm_ConsolidateRunningHeaders(DB);
            xform.OnImport();

            CompareTransforms(DB, vDataExpected1);
        }
        #endregion

        // Paratext Export 
        #region TEST: ParatextRoundTrip
        [Test] public void ParatextRoundTrip()
        {
            // Set up the data (both the Input and the Expected are the same strings)
            InitializeStringSet(2);
            AddStringSet(0, m_vIn_1, m_vExpectedParatext_1);
            AddStringSet(1, m_vIn_2, m_vExpectedParatext_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                // Read in and transform to our normal input
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.Format = ScriptureDB.Formats.kParatext;
                DB.TransformIn();

                //Console.WriteLine("In finished");

                // Now do the reverse of the transforms back to Paratext
                DB.TransformOut();

                // Write to console for debugging purposes
                //WriteActualDataToConsole(DB, "ParatextRoundTrip - " + i.ToString());

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion


    }
}
