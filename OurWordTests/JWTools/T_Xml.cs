/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Xml.cs
 * Author:  John Wimbish
 * Created: 14 May 2008
 * Purpose: Tests the xml classes
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.JWTools
{
    [TestFixture] public class T_XmlOld
    {
        #region TagRetrieval
        [Test] public void TagRetrieval()
        {
            string sLine = "   <person name=\"John\" gender=\"male\" age=\"44\">";
            Assert.IsTrue("person" == JW_Xml.GetTag(sLine));
            Assert.IsTrue("person" == JW_Xml.GetTag("<person>"));

            Assert.IsTrue(JW_Xml.IsTag("person", sLine));
            Assert.IsTrue(JW_Xml.IsTag("person", "<person>"));

            Assert.IsFalse(JW_Xml.IsTag("people", sLine));

            Assert.IsTrue(JW_Xml.IsClosingTag("person", "</person>"));
            Assert.IsTrue(JW_Xml.IsClosingTag("person", "     </person>"));
            Assert.IsTrue(JW_Xml.IsClosingTag("/person", "</person>"));
            Assert.IsTrue(JW_Xml.IsClosingTag("/person", "     </person>"));
        }
        #endregion
        #region ValueRetrieval
        [Test] public void ValueRetrieval()
        {
            string sLine = "   <person name=\"John\" gender=\"male\" age=\"44\">";

            Assert.IsTrue("John" == JW_Xml.GetValue("name", sLine));
            Assert.IsTrue("male" == JW_Xml.GetValue("gender", sLine));
            Assert.IsTrue("44" == JW_Xml.GetValue("age", sLine));
        }
        #endregion
    }

    [TestFixture] public class T_Xml
    {
        // Sena Sample Data - Section = Jonah Chapter 1
        #region Method: XElement MakeVerse(sID, sVerseNo)
        XElement MakeVerse(string sID, string sVerseNo)
        {
            XElement xVerse = new XElement("verseStart");
            xVerse.AddAttr("ID", sID);
            xVerse.AddAttr("n", sVerseNo);
            return xVerse;
        }
        #endregion
        #region Method: XElement MakeChapter(sID, sVerseNo)
        XElement MakeChapter(string sID, string sChapterNo)
        {
            XElement xChapter = new XElement("chapterStart");
            xChapter.AddAttr("ID", sID);
            xChapter.AddAttr("n", sChapterNo);
            return xChapter;
        }
        #endregion
        #region Method: XElement MakeVerseEnd(sID)
        XElement MakeVerseEnd(string sID)
        {
            XElement xVerseEnd = new XElement("verseEnd");
            xVerseEnd.AddAttr("ID", sID);
            return xVerseEnd;
        }
        #endregion
        #region Method: XElement MakeChapterEnd(sID)
        XElement MakeChapterEnd(string sID)
        {
            XElement xVerseEnd = new XElement("chapterEnd");
            xVerseEnd.AddAttr("ID", sID);
            return xVerseEnd;
        }
        #endregion
        #region Method: MakeGroup(sBTStatus, sTr, sBT)
        XElement MakeGroup(string sBTStatus, string sTr, string sBT)
        {
            XElement xGroup = new XElement("trGroup");

            XElement xTr = new XElement("tr");
            xTr.AddSubItem(sTr);
            xGroup.AddSubItem(xTr);

            XElement xBt = new XElement("bt");
            xBt.AddAttr("xml:lang", "pt");
            if (!string.IsNullOrEmpty(sBTStatus))
                xBt.AddAttr("status", "unfinished");
            xBt.AddSubItem(sBT);
            xGroup.AddSubItem(xBt);

            return xGroup;
        }
        #endregion
        #region Method: CreateOxesDataSena_SectionJonah1()
        XElement CreateOxesDataSena_SectionJonah1()
        {
            XElement x = new XElement("section");

            XElement xSectionHead = new XElement("sectionHead");
            xSectionHead.AddSubItem(MakeGroup("",
                "Djona akhonda kubvera Mulungu",
                "Jona nega ouvir Deus"));
            x.AddSubItem(xSectionHead);

            XElement xP = new XElement("p");
            xP.AddSubItem(MakeChapter("JON.1", "1"));
            xP.AddSubItem(MakeVerse("JON.1.1", "1"));
            xP.AddSubItem(MakeGroup("unfinished", 
                "Pyacitika ntsiku ibodzi MBUYA atuma mamuna m'bodzi anacemerwa Djona, mwana wa Amitayi, mbampanga tenepa:",
                "Aconteceu um dia O Senhor mandou um homen, named Jona, filho de Amitai, dizendo-lhe assim:"));
            xP.AddSubItem(MakeVerseEnd("JON.1.1"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.2", "2"));
            xP.AddSubItem(MakeGroup("",
                "“Sasanyira wende ku ndzinda ukulu unacemerwa Ninivi, wende kaacenjedze anthu a dziko eneyo kuti ine ndisadziwa kuipa kwawo kuonsene kunacita iwo.”",
                "Prepara-te vai para cidade grande a qual é chamada Ninivi, que vas advirtir-lhes pessoas da terra essa, que eu sei todo o mal deles que eles fazem."));
            xP.AddSubItem(MakeVerseEnd("JON.1.2"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.3", "3"));
            xP.AddSubItem(MakeGroup("",
                "Mbwenye Djona anyerezera kuthawa kuenda kakhala kutali na Mulungu, ku dziko inacemerwa Tarixixi. Aenda ku ndzinda wa Djopi. Pidafika iye kweneko agumana paketi ikhafuna kuenda ku ndzinda Tarixixi. Na tenepa Djona, na kufuna kuthawa MBUYA, agula thikiti ya paketi mbapakira mbaenda pabodzi na anyabasa a paketi ku Tarixixi.",
                "Mas Jona pensou fugir ir ficar longe de Deus, na terra a qual é chamada Tarsis. Foi para cidade de Jope. Quando chegou ele lá encontrou barco queria ir para Tarsis. E assim, Jona ao querer fugir do Senhor, comprou bilhete de barco subindo indo junto com marinheiros para Tarsis."));
            xP.AddSubItem(MakeVerseEnd("JON.1.3"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.4", "4"));
            xP.AddSubItem(MakeGroup("",
                "Mbwenye MBUYA atuma conzi cikulu ca mphepo m'bara, mbacifuna kuswa paketi.",
                "Mas o Senhor mandou um grande tempestade de vento no mar [vento] querendo quebrar barco."));
            xP.AddSubItem(MakeVerseEnd("JON.1.4"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.5", "5"));
            xP.AddSubItem(MakeGroup("",
                "Na tenepa, anyabasa a paketi agopa pikulu kakamwe mbatoma kuphemba m'bodzi na m'bodzi, kuti aphembe kuna mulungu wace toera aphedziwe. Pontho anyabasa a paketi atoma kubulusa mitolo yonsene ikhali na iwo, mbaitaya m'madzi mwa bara toera paketi ileke kuririma. Mbwenye Djona akhadachitira pantsi pa paketi mbakhagona citulo cikulu.",
                "E assim, marinheiros temeram muito mesmo começando pedir todos (à um por um) para que peça a deus deles para ser ajudado. Além disso os marinheiros começaram tirar bagagens deles todas que ficavam com eles, deitando-as dentro da água do mar para barco não afundar-se. Mas Jona tinha descido em baixo do barco dormindo sono muito."));
            xP.AddSubItem(MakeVerseEnd("JON.1.5"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.6", "6"));
            xP.AddSubItem(MakeGroup("",
                "Nkulu wa paketi mudapita iye nkati mukhagona Djona, amulamusa mbampanga tenepa: “Kodi iwe uli kucitanji muno? Lamuka uphembe kuna Mulungu wako. Panango iye anatibvera nsisi mbatipulumusa.”",
                "Capitão (Grande) do barco quando entrou ele dentro onde dormia Jona, acordou-o dizendo-lhe assim: Olha lá tu que estás a fazer aqui? Levanta peças ao deus teu. Talvez ele vai nos ouvir (sentir) pena salvando-nos."));
            xP.AddSubItem(MakeVerseEnd("JON.1.6"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.7", "7"));
            xP.AddSubItem(MakeGroup("",
                "Anyabasa a paketi akhabverana mbalonga kuti: “Tendeni tisake pakati pathu mbani anatidzesera tsoka.” Pa kumala kucita penepyo, pagumanika kuti nyatwa zenezi, ziri kubulukira kuna Djona.",
                "Marinheiros combinaram dizendo que: “Vamos procuremos no meio de nós quem nos traz azar.” Depois de fazer isso, foi encontrado que castigos estes, estão aparecer atravez de Jona."));
            xP.AddSubItem(MakeVerseEnd("JON.1.7"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.8", "8"));
            xP.AddSubItem(MakeGroup("",
                "Na tenepa, awene ambvunzisa Djona kuti: “Tipange, thangwi yanji ziri kuoneka nyatwa zenezi kuna ife? Basa yako njanji? Ndiwe wa dzinza ipi? Wabuluka kupi? Dziko yako njipi?”",
                "E assim eles perguntaram-lhe ao Jona que: Diz-nos, por causa de quê estão se ver castigos estes em nós? Serviço teu qual é? És tu de tribo qual? Donde saiste? Qual é a tua terra?"));
            xP.AddSubItem(MakeVerseEnd("JON.1.8"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.9", "9"));
            xP.AddSubItem(MakeGroup("",
                "Djona aatawira kuti: “Ine ndine wa dzinza ya aebereu. Ndisalambira MBUYA, Mulungu wa kudzulu, adalenga bara na mataka.”",
                "Jona respondeu-lhes que: Eu sou da tribo dos hebreus; adoro o SENHOR, Deus do ceu, o qual criou o mar e a terra."));
            xP.AddSubItem(MakeVerseEnd("JON.1.9"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.10", "10"));
            xP.AddSubItem(MakeGroup("",
                "Pontho Djona aapanga pyonsene pikhadacitika kuna iye. Na mwenemo awene agopserwa napyo na kubva kuti iye Djona acita thawa Mulungu wace. Na tenepa awene ambvunzisa Djona kuti: “Xii! Ninji penepyo pidacita iwe?",
                "Além disso Jona disse-lhes tudo o que acontecera com ele. E por isso eles ficaram assustados com aquilo ao ouvir que ele Jona fugiu (fez fugir) de deus dele. E assim eles perguntaram-no ao Jona que: “Que é isto que fizeste tu?”"));
            xP.AddSubItem(MakeVerseEnd("JON.1.10"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.11", "11"));
            xP.AddSubItem(MakeGroup("",
                "Mphapo usafuna tikucitenji toera mphepo ya m'bara inafuna kutipha imatame?” Thangwi iyo ikhaenderatu mbikula.",
                "Portanto o que tu queres que nós façamos para a ventania do mar que quer matar-nos parar (calar), porque ela está piorando (está ir mesmo crescendo)?"));
            xP.AddSubItem(MakeVerseEnd("JON.1.11"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.12", "12"));
            xP.AddSubItem(MakeGroup("",
                "Djona aatawira mbalonga: “Ndiphateni mundiponye m'bara toera mphepo imatame. Thangwi ine ndisadziwa kuti mphepo yonsene iyi yaoneka na thangwi yanga.”",
                "Jona respondeu-lhes dizendo: “Agarrem-me e atirem-me no mar para a ventania parar (calar). Porque eu estou a saber que a ventania toda esta apareceu (foi vista) por causa de mim.”"));
            xP.AddSubItem(MakeVerseEnd("JON.1.12"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.13", "13"));
            xP.AddSubItem(MakeGroup("",
                "Mbwenye anyabasa a paketi akhawangisa basi na kucapa paketi na mphambvu zawo zonsene toera afike muntunda. Mbwenye akhaicimwana, thangwi mphepo ikhadawangiratu kakamwe kupita mphambvu zikhacita iwo. ",
                "Mas os marinheiros esforçavam sempre em remar barco com toda a força deles para chegar à terra seca. Mas não o conseguiam, porque a ventania estava muito mais forte mesmo ultrapassar força (esforço) que faziam eles. "));
            xP.AddSubItem(MakeVerseEnd("JON.1.13"));
            xP.AddSubItem(MakeVerse("JON.1.14", "14"));
            xP.AddSubItem(MakeGroup("",
                "Mphapo iwo aphemba pontho kuna MBUYA, Mulungu wa Djona, kuti aaphedze, mbalonga tenepa: “MBUYA, taphata myendo, lekani kutipha na thangwi ya munthu uyu; pontho lekani kutipasa thangwi ya kupha munthu wa kusowa mathangwi. Thangwi pyonsene pidaoneka pyacitika na kufuna kwanu.”",
                "Portanto eles pediram outra vez ao Senhor que os ajude, dizendo assim: “Senhor, por favor (pegamos pernas), não nos mate (deixe matar-nos) por causa desta pessoa, além disso (também) não nos dê culpas (deixe dar dar-nos culpas) porcausa de matar um homén innocente (que não tem culpas). Porque tudo o que foi visto foi feito por Sua vontade (por querer Seu)."));
            xP.AddSubItem(MakeVerseEnd("JON.1.14"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.15", "15"));
            xP.AddSubItem(MakeGroup("",
                "Pidamala iwo, amphata Djona mbamponya m'bara. Kubulukira pa ndzidzi ubodzi-ubodzi mphepo ya m'bara yamatamiratu!",
                "Quando acabaram eles, agarraram-no Jona e atiraram-no no mar. Á partir (sair) do tempo esse a ventania calou de vez!"));
            xP.AddSubItem(MakeVerseEnd("JON.1.15"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.16", "16"));
            xP.AddSubItem(MakeGroup("",
                "Na tenepa, anyabasa a paketi adzumatirwa mbagopa mphambvu za MBUYA; na tenepa atoma kumuikhira ntsembe na kuncitira cidumbiriro.",
                "E assim marinheiros admiraram-se temendo a força do Senhor; e por isso começou pôr-Lhe sacrificio e fazer-Lhe promessa."));
            xP.AddSubItem(MakeVerseEnd("JON.1.16"));
            x.AddSubItem(xP);

            xP = new XElement("p");
            xP.AddSubItem(MakeVerse("JON.1.17", "17"));
            xP.AddSubItem(MakeGroup("",
                "MBUYA atuma nyama ikulu kakamwe ya m'madzi mbimeza Djona. Iye akhala m'mimba mwa nyama eneyi ntsiku zitatu, masiku na masikati.",
                "O Senhor mandou um peixe (carne da água) muito grande ingolindo Jona. Ele ficou na barriga de este peixe (carne), dias três, noites e dia."));
            xP.AddSubItem(MakeVerseEnd("JON.1.17"));
            xP.AddSubItem(MakeChapterEnd("JON.1"));
            x.AddSubItem(xP);

            return x;
        }
        #endregion
        #region DATA: string[] vsOxesDataSena_SectionJonah1
        static string[] vsOxesDataSena_SectionJonah1 = 
            {
                "<section>",
                "  <sectionHead>",
                "    <trGroup>",
                "      <tr>Djona akhonda kubvera Mulungu</tr>",
                "      <bt xml:lang=\"pt\">Jona nega ouvir Deus</bt>",
                "    </trGroup>",
                "  </sectionHead>",
                "  <p>",
                "    <chapterStart ID=\"JON.1\" n=\"1\" />",
                "    <verseStart ID=\"JON.1.1\" n=\"1\" />",
                "    <trGroup>",
                "      <tr>Pyacitika ntsiku ibodzi MBUYA atuma mamuna m'bodzi anacemerwa Djona, mwana wa Amitayi, mbampanga tenepa:</tr>",
                "      <bt xml:lang=\"pt\" status=\"unfinished\">Aconteceu um dia O Senhor mandou um homen, named Jona, filho de Amitai, dizendo-lhe assim:</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.1\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.2\" n=\"2\" />",
                "    <trGroup>",
                "      <tr>“Sasanyira wende ku ndzinda ukulu unacemerwa Ninivi, wende kaacenjedze anthu a dziko eneyo kuti ine ndisadziwa kuipa kwawo kuonsene kunacita iwo.”</tr>",
                "      <bt xml:lang=\"pt\">Prepara-te vai para cidade grande a qual é chamada Ninivi, que vas advirtir-lhes pessoas da terra essa, que eu sei todo o mal deles que eles fazem.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.2\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.3\" n=\"3\" />",
                "    <trGroup>",
                "      <tr>Mbwenye Djona anyerezera kuthawa kuenda kakhala kutali na Mulungu, ku dziko inacemerwa Tarixixi. Aenda ku ndzinda wa Djopi. Pidafika iye kweneko agumana paketi ikhafuna kuenda ku ndzinda Tarixixi. Na tenepa Djona, na kufuna kuthawa MBUYA, agula thikiti ya paketi mbapakira mbaenda pabodzi na anyabasa a paketi ku Tarixixi.</tr>",
                "      <bt xml:lang=\"pt\">Mas Jona pensou fugir ir ficar longe de Deus, na terra a qual é chamada Tarsis. Foi para cidade de Jope. Quando chegou ele lá encontrou barco queria ir para Tarsis. E assim, Jona ao querer fugir do Senhor, comprou bilhete de barco subindo indo junto com marinheiros para Tarsis.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.3\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.4\" n=\"4\" />",
                "    <trGroup>",
                "      <tr>Mbwenye MBUYA atuma conzi cikulu ca mphepo m'bara, mbacifuna kuswa paketi.</tr>",
                "      <bt xml:lang=\"pt\">Mas o Senhor mandou um grande tempestade de vento no mar [vento] querendo quebrar barco.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.4\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.5\" n=\"5\" />",
                "    <trGroup>",
                "      <tr>Na tenepa, anyabasa a paketi agopa pikulu kakamwe mbatoma kuphemba m'bodzi na m'bodzi, kuti aphembe kuna mulungu wace toera aphedziwe. Pontho anyabasa a paketi atoma kubulusa mitolo yonsene ikhali na iwo, mbaitaya m'madzi mwa bara toera paketi ileke kuririma. Mbwenye Djona akhadachitira pantsi pa paketi mbakhagona citulo cikulu.</tr>",
                "      <bt xml:lang=\"pt\">E assim, marinheiros temeram muito mesmo começando pedir todos (à um por um) para que peça a deus deles para ser ajudado. Além disso os marinheiros começaram tirar bagagens deles todas que ficavam com eles, deitando-as dentro da água do mar para barco não afundar-se. Mas Jona tinha descido em baixo do barco dormindo sono muito.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.5\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.6\" n=\"6\" />",
                "    <trGroup>",
                "      <tr>Nkulu wa paketi mudapita iye nkati mukhagona Djona, amulamusa mbampanga tenepa: “Kodi iwe uli kucitanji muno? Lamuka uphembe kuna Mulungu wako. Panango iye anatibvera nsisi mbatipulumusa.”</tr>",
                "      <bt xml:lang=\"pt\">Capitão (Grande) do barco quando entrou ele dentro onde dormia Jona, acordou-o dizendo-lhe assim: Olha lá tu que estás a fazer aqui? Levanta peças ao deus teu. Talvez ele vai nos ouvir (sentir) pena salvando-nos.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.6\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.7\" n=\"7\" />",
                "    <trGroup>",
                "      <tr>Anyabasa a paketi akhabverana mbalonga kuti: “Tendeni tisake pakati pathu mbani anatidzesera tsoka.” Pa kumala kucita penepyo, pagumanika kuti nyatwa zenezi, ziri kubulukira kuna Djona.</tr>",
                "      <bt xml:lang=\"pt\">Marinheiros combinaram dizendo que: “Vamos procuremos no meio de nós quem nos traz azar.” Depois de fazer isso, foi encontrado que castigos estes, estão aparecer atravez de Jona.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.7\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.8\" n=\"8\" />",
                "    <trGroup>",
                "      <tr>Na tenepa, awene ambvunzisa Djona kuti: “Tipange, thangwi yanji ziri kuoneka nyatwa zenezi kuna ife? Basa yako njanji? Ndiwe wa dzinza ipi? Wabuluka kupi? Dziko yako njipi?”</tr>",
                "      <bt xml:lang=\"pt\">E assim eles perguntaram-lhe ao Jona que: Diz-nos, por causa de quê estão se ver castigos estes em nós? Serviço teu qual é? És tu de tribo qual? Donde saiste? Qual é a tua terra?</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.8\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.9\" n=\"9\" />",
                "    <trGroup>",
                "      <tr>Djona aatawira kuti: “Ine ndine wa dzinza ya aebereu. Ndisalambira MBUYA, Mulungu wa kudzulu, adalenga bara na mataka.”</tr>",
                "      <bt xml:lang=\"pt\">Jona respondeu-lhes que: Eu sou da tribo dos hebreus; adoro o SENHOR, Deus do ceu, o qual criou o mar e a terra.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.9\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.10\" n=\"10\" />",
                "    <trGroup>",
                "      <tr>Pontho Djona aapanga pyonsene pikhadacitika kuna iye. Na mwenemo awene agopserwa napyo na kubva kuti iye Djona acita thawa Mulungu wace. Na tenepa awene ambvunzisa Djona kuti: “Xii! Ninji penepyo pidacita iwe?</tr>",
                "      <bt xml:lang=\"pt\">Além disso Jona disse-lhes tudo o que acontecera com ele. E por isso eles ficaram assustados com aquilo ao ouvir que ele Jona fugiu (fez fugir) de deus dele. E assim eles perguntaram-no ao Jona que: “Que é isto que fizeste tu?”</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.10\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.11\" n=\"11\" />",
                "    <trGroup>",
                "      <tr>Mphapo usafuna tikucitenji toera mphepo ya m'bara inafuna kutipha imatame?” Thangwi iyo ikhaenderatu mbikula.</tr>",
                "      <bt xml:lang=\"pt\">Portanto o que tu queres que nós façamos para a ventania do mar que quer matar-nos parar (calar), porque ela está piorando (está ir mesmo crescendo)?</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.11\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.12\" n=\"12\" />",
                "    <trGroup>",
                "      <tr>Djona aatawira mbalonga: “Ndiphateni mundiponye m'bara toera mphepo imatame. Thangwi ine ndisadziwa kuti mphepo yonsene iyi yaoneka na thangwi yanga.”</tr>",
                "      <bt xml:lang=\"pt\">Jona respondeu-lhes dizendo: “Agarrem-me e atirem-me no mar para a ventania parar (calar). Porque eu estou a saber que a ventania toda esta apareceu (foi vista) por causa de mim.”</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.12\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.13\" n=\"13\" />",
                "    <trGroup>",
                "      <tr>Mbwenye anyabasa a paketi akhawangisa basi na kucapa paketi na mphambvu zawo zonsene toera afike muntunda. Mbwenye akhaicimwana, thangwi mphepo ikhadawangiratu kakamwe kupita mphambvu zikhacita iwo. </tr>",
                "      <bt xml:lang=\"pt\">Mas os marinheiros esforçavam sempre em remar barco com toda a força deles para chegar à terra seca. Mas não o conseguiam, porque a ventania estava muito mais forte mesmo ultrapassar força (esforço) que faziam eles. </bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.13\" />",
                "    <verseStart ID=\"JON.1.14\" n=\"14\" />",
                "    <trGroup>",
                "      <tr>Mphapo iwo aphemba pontho kuna MBUYA, Mulungu wa Djona, kuti aaphedze, mbalonga tenepa: “MBUYA, taphata myendo, lekani kutipha na thangwi ya munthu uyu; pontho lekani kutipasa thangwi ya kupha munthu wa kusowa mathangwi. Thangwi pyonsene pidaoneka pyacitika na kufuna kwanu.”</tr>",
                "      <bt xml:lang=\"pt\">Portanto eles pediram outra vez ao Senhor que os ajude, dizendo assim: “Senhor, por favor (pegamos pernas), não nos mate (deixe matar-nos) por causa desta pessoa, além disso (também) não nos dê culpas (deixe dar dar-nos culpas) porcausa de matar um homén innocente (que não tem culpas). Porque tudo o que foi visto foi feito por Sua vontade (por querer Seu).</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.14\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.15\" n=\"15\" />",
                "    <trGroup>",
                "      <tr>Pidamala iwo, amphata Djona mbamponya m'bara. Kubulukira pa ndzidzi ubodzi-ubodzi mphepo ya m'bara yamatamiratu!</tr>",
                "      <bt xml:lang=\"pt\">Quando acabaram eles, agarraram-no Jona e atiraram-no no mar. Á partir (sair) do tempo esse a ventania calou de vez!</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.15\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.16\" n=\"16\" />",
                "    <trGroup>",
                "      <tr>Na tenepa, anyabasa a paketi adzumatirwa mbagopa mphambvu za MBUYA; na tenepa atoma kumuikhira ntsembe na kuncitira cidumbiriro.</tr>",
                "      <bt xml:lang=\"pt\">E assim marinheiros admiraram-se temendo a força do Senhor; e por isso começou pôr-Lhe sacrificio e fazer-Lhe promessa.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.16\" />",
                "  </p>",
                "  <p>",
                "    <verseStart ID=\"JON.1.17\" n=\"17\" />",
                "    <trGroup>",
                "      <tr>MBUYA atuma nyama ikulu kakamwe ya m'madzi mbimeza Djona. Iye akhala m'mimba mwa nyama eneyi ntsiku zitatu, masiku na masikati.</tr>",
                "      <bt xml:lang=\"pt\">O Senhor mandou um peixe (carne da água) muito grande ingolindo Jona. Ele ficou na barriga de este peixe (carne), dias três, noites e dia.</bt>",
                "    </trGroup>",
                "    <verseEnd ID=\"JON.1.17\" />",
                "    <chapterEnd ID=\"JON.1\" />",
                "  </p>",
                "</section>" 
            };
        #endregion

        // Out Tests
        #region Test: AmpersandsAndSuch
        [Test] public void AmpersandsAndSuch()
        {
            Assert.AreEqual("He said, &lt;&lt;Hello&gt;&gt; to her.",
                XElement.AmpersandsAndSuch_Write("He said, <<Hello>> to her."), "1a");
            Assert.AreEqual("He said, <<Hello>> to her.",
                XElement.AmpersandsAndSuch_Read("He said, &lt;&lt;Hello&gt;&gt; to her."), "1b");

            Assert.AreEqual("He said, &quot;Hello&quot; to her.",
                XElement.AmpersandsAndSuch_Write("He said, \"Hello\" to her."), "2a");
            Assert.AreEqual("He said, \"Hello\" to her.",
                XElement.AmpersandsAndSuch_Read("He said, &quot;Hello&quot; to her."), "2b");

            Assert.AreEqual("John &amp; Sandra{n}Authors{n}",
                XElement.AmpersandsAndSuch_Write("John & Sandra\nAuthors\n"), "3a");
            Assert.AreEqual("John & Sandra\nAuthors\n",
                XElement.AmpersandsAndSuch_Read("John &amp; Sandra{n}Authors{n}"), "3b");

            Assert.AreEqual("He said, &lt;&lt;Hello.&gt;&gt;",
                XElement.AmpersandsAndSuch_Write("He said, <<Hello.>>"), "4a");
            Assert.AreEqual("He said, <<Hello.>>",
                XElement.AmpersandsAndSuch_Read("He said, &lt;&lt;Hello.&gt;&gt;"), "4b");
        
        }
        #endregion
        #region Test: BuildAttrString
        [Test] public void BuildAttrString()
        {
            // String version
            XElement.XAttr attr = new XElement.XAttr("name", "John");
            Assert.AreEqual(" name=\"John\"", attr.SaveString);

            // Int version
            attr = new XElement.XAttr("year", 1959);
            Assert.AreEqual(" year=\"1959\"", attr.SaveString);

            // Guid version
            string sGuid = "7eed4a8c-ee56-4d1c-859d-b8183679bd6d";
            Guid g = new Guid(sGuid);
            XElement x = new XElement("tag");
            x.AddAttr("Guid", g);
            Assert.AreEqual( " Guid=\"" + sGuid + "\"", x.Attrs[0].SaveString, "Guid version");
            // Check the round trip
            Assert.AreEqual(g, x.GetAttrValue("Guid", Guid.NewGuid()));
        }
        #endregion
        #region Test: OneLiner_AttrsOnly
        [Test] public void OneLiner_AttrsOnly()
        {
            string sTag = "birthdate";

            XElement xml = new XElement(sTag);
            xml.AddAttr("year", 1959);
            xml.AddAttr("month", 11);
            xml.AddAttr("name", "John");
            DateTime dtMarried = new DateTime(1986, 8, 23, 14, 10, 05);
            xml.AddAttr("married", dtMarried);
            string sExpected = "<birthdate year=\"1959\" month=\"11\" name=\"John\" " +
                "married=\"1986-08-23 14:10:05Z\"/>";
            Assert.AreEqual(sExpected, xml.OneLiner);

            // Test the ability to correctly read the values
            XElement[] vxml2 = XElement.CreateFrom(sExpected);
            DateTime dtMarried2 = vxml2[0].GetAttrValue("married", DateTime.Today);
            Assert.IsTrue(0 == dtMarried.CompareTo(dtMarried2));

        }
        #endregion
        #region Test: OneLiner_Empty
        [Test] public void OneLiner_Empty()
        {
            XElement xml = new XElement("word");
            Assert.AreEqual(
                "",
                xml.OneLiner);
        }
        #endregion
        #region Test: OneLiner_WithDataString
        [Test] public void OneLiner_WithDataString()
        {
            XElement x = new XElement("word");
            x.AddSubItem("Hello");
            Assert.AreEqual(
                "<word>Hello</word>",
                x.OneLiner);
        }
        #endregion
        #region Test: OneLiner_WithAttrAndDataString
        [Test] public void OneLiner_WithAttrAndDataString()
        {
            XElement x = new XElement("word");
            x.AddSubItem("Hello");
            x.AddAttr("enc", "eng");
            Assert.AreEqual(
                "<word enc=\"eng\">Hello</word>",
                x.OneLiner);
        }
        #endregion

        // In Tests
        #region Test: ParseIntoXmlStrings
        [Test] public void ParseIntoXmlStrings()
        {
            string sIn = "<I M=\"God's kingdom\">" +
                "<B T=\"Tuhan\" M=\"Lord\"/>" +
                "<B T=\"Allah\" M=\"God\"/>\n" +
                "<B T=\"pung\" M=\"has\"/>" +
                "<B T=\"orang\" M=\"person\"/>" +
                "<B T=\"dong\" M=\"3P\"/>" +
                "</I>";

            string[] vsOut = XElement.CreateMethod.ParseIntoXmlStrings(sIn);

            Assert.AreEqual(7, vsOut.Length);
            Assert.AreEqual("<I M=\"God's kingdom\">", vsOut[0]);
            Assert.AreEqual("<B T=\"Tuhan\" M=\"Lord\"/>", vsOut[1]);
            Assert.AreEqual("<B T=\"Allah\" M=\"God\"/>", vsOut[2]);
            Assert.AreEqual("<B T=\"pung\" M=\"has\"/>", vsOut[3]);
            Assert.AreEqual("<B T=\"orang\" M=\"person\"/>", vsOut[4]);
            Assert.AreEqual("<B T=\"dong\" M=\"3P\"/>", vsOut[5]);
            Assert.AreEqual("</I>", vsOut[6]);
        }
        #endregion
        #region Test: ParseIntoXmlStrings_WithData
        [Test] public void ParseIntoXmlStrings_WithData()
        {
            string sIn = "<word enc=\"eng\">Hello</word>";

            string[] vsOut = XElement.CreateMethod.ParseIntoXmlStrings(sIn);

            Assert.AreEqual(3, vsOut.Length);
            Assert.AreEqual("<word enc=\"eng\">", vsOut[0]);
            Assert.AreEqual("Hello", vsOut[1]);
            Assert.AreEqual("</word>", vsOut[2]);
        }
        #endregion
        #region Test: ParseIntoXmlStrings_MixedContent
        [Test] public void ParseIntoXmlStrings_MixedContent()
        {
            string sIn = "<p>Mixed content is allowed in the <abbreviation " +
                "expansion=\"Open XML for Editing Scripture\">OXES" +
                "</abbreviation> schema.</p>";

            string[] vsOut = XElement.CreateMethod.ParseIntoXmlStrings(sIn);

            //foreach (string s in vsOut)
            //    Console.WriteLine(s);

            Assert.AreEqual(7, vsOut.Length);
            Assert.AreEqual("<p>", vsOut[0]);
            Assert.AreEqual("Mixed content is allowed in the ", vsOut[1]);
            Assert.AreEqual("<abbreviation expansion=\"Open XML for Editing Scripture\">", vsOut[2]);
            Assert.AreEqual("OXES", vsOut[3]);
            Assert.AreEqual("</abbreviation>", vsOut[4]);
            Assert.AreEqual(" schema.", vsOut[5]);
            Assert.AreEqual("</p>", vsOut[6]);
        }
        #endregion
        #region Test: ParseIntoXElements
        [Test] public void ParseIntoXElements()
        {
            string sIn = "<I M=\"God's kingdom\">" +
                "<B T=\"Tuhan\" M=\"Lord\"/>" +
                "<B T=\"Allah\" M=\"God\"/>" +
                "<B T=\"pung\" M=\"has\"/>" +
                "<B T=\"orang\" M=\"person\"/>" +
                "<B T=\"dong\" M=\"3P\"/>" +
                "</I>";

            XElement[] vx = XElement.CreateFrom(sIn);

            Assert.AreEqual(1, vx.Length);
            Assert.AreEqual(sIn, vx[0].OneLiner);
        }
        #endregion
        #region Test: ParseIntoXElements_WithData
        [Test] public void ParseIntoXElements_WithData()
        {
            string sIn = "<word enc=\"eng\">Hello</word>";

            XElement[] vx = XElement.CreateFrom(sIn);

            //Console.WriteLine(vx[0].OneLiner);

            Assert.AreEqual(1, vx.Length);
            Assert.AreEqual(sIn, vx[0].OneLiner);
        }
        #endregion
        #region Test: ParseIntoXElements_MixedContent
        [Test] public void ParseIntoXElements_MixedContent()
        {
            string sIn = "<p>Mixed content is allowed in the <abbreviation " +
                "expansion=\"Open XML for Editing Scripture\">OXES" +
                "</abbreviation> schema.</p>";

            XElement[] vx = XElement.CreateFrom(sIn);

            //Console.WriteLine(vx[0].OneLiner);

            Assert.AreEqual(1, vx.Length);
            Assert.AreEqual(sIn, vx[0].OneLiner);
        }
        #endregion
        #region Test: ParseIntoXElements_SenaSection
        [Test] public void ParseIntoXElements_SenaSection()
        {
            // First, make a long string (with line breaks so we know to eat spaces)
            string sIn = "";
            foreach (string s in vsOxesDataSena_SectionJonah1)
                sIn += (s + '\n');

            // Parse into elements
            XElement[] vx = XElement.CreateFrom(sIn);

            // Special function to create the xml by hand (tedious to program!)
            XElement xExpected = CreateOxesDataSena_SectionJonah1();

            // We expect them to be the same
            Assert.AreEqual(1, vx.Length);
            Assert.IsTrue(xExpected.ContentEquals(vx[0]));
        }
        #endregion
        #region Test: ParseIntoXmlStrings_WithData_YawaNote214
        [Test] public void ParseIntoXmlStrings_WithData_YawaNote214()
            // This is the bug that turned up on Mandowen's computer, 6apr09, due
            // to my not taking into acount internal '=' character in the data.
        {
            string sContents = "ratoe taiso ubeke dai ware mbarije, ware ana daveti ngkodave " +
                "ware mbakobe jewen. weye ana dave ama ine masyare: anave, anabe, muno ana " +
                "raijaro aneme rai nsiso , (anave=peraya, iman) anabe= sifat, ";
            string sIn = "<DParagraph Abbrev=\"NoteDiscussion\" Contents=\"" + sContents + "\"/>";

            XElement[] vx = XElement.CreateFrom(sIn);

            Assert.AreEqual(1, vx.Length);

            XElement x = vx[0];

            Assert.AreEqual(sContents, x.FindAttr("Contents").Value);
        }
        #endregion
    }


}
