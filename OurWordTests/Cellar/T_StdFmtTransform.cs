﻿/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_StdFmtTransformt.cs
 * Author:  John Wimbish
 * Created: 2 Jul 2009
 * Purpose: Tests the standard format transforms (most testing is accomplsiehd via T_DSection)
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

using OurWord;
using OurWordData.DataModel;
using JWTools;
using OurWordData;
#endregion

namespace OurWordTests.Cellar
{
    [TestFixture]
    public class T_StdFmtTransform
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Method: CompareStringArrays(vsExpected, vsActual)
        void CompareStringArrays(string[] vsExpected, string[] vsActual)
        {
            Assert.AreEqual(vsExpected.Length, vsActual.Length);
            for (int i = 0; i < vsExpected.Length; i++)
                Assert.AreEqual(vsExpected[i], vsActual[i]);
        }
        #endregion

        #region Test: XForm_HandleEmbeddedParatextParagraphs
        [Test] public void XForm_HandleEmbeddedParatextParagraphs()
        {
            var vsIn = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };
            var vsExpected = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, ",
                "\\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. ",
                "\\q Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kParatext;

            // Run the transform
            var transform = new XForm_HandleEmbeddedParatextParagraphs(DB);
            transform.OnImport();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: XForm_HandleLineMedialNormalizations
        [Test] public void XForm_HandleLineMedialNormalizations()
        {
            var vsIn = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q1 <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q1 Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };
            var vsExpected = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\q <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q Nya ana nyo rave tai maisy, weamo nya marova so ",
                "\\q2 indati samane apa ana po nave tatugadi rai.>>\\x + \\xo 12:20 \\xt Amsal 25:21-22\\xt* \\x* "
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kParatext;

            // Run the transform
            var transform = new XForm_NormalizeMarkers(DB);
            transform.OnImport();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion

        #region Test: XForm_GoBibleNormalizeID
        [Test] public void XForm_GoBibleNormalizeID()
        {
            var vsIn = new string[] {
                "\\id Mrk Edited by me."
            };
            var vsExpected = new string[] {
                "\\id MRK"
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the transform
            var transform = new XForm_GoBibleExport(DB);
            transform.NormalizeID();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: XForm_GoBibleSections
        [Test] public void XForm_GoBibleSections()
        {
            var vsIn = new string[] {
                "\\s Yohanis Tukang Sarani buka jalan kasi Tuhan Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Yesus balóm mulai Dia pung karjá, te Tuhan Allah su utus satu orang, nama Yohanis. Yohanis musti pi sadia jalan kasi Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung jubir, nama ba'i Yesaya. Dia su tulis memang, bilang:"
            };
            var vsExpected = new string[] {
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt \\wj Yohanis Tukang Sarani buka jalan kasi Tuhan Yesus\\wj* Yesus balóm mulai Dia pung karjá, te Tuhan Allah su utus satu orang, nama Yohanis. Yohanis musti pi sadia jalan kasi Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung jubir, nama ba'i Yesaya. Dia su tulis memang, bilang:"
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the transform
            var transform = new XForm_GoBibleExport(DB);
            transform.MoveSections();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: XForm_GoBibleRemoveFootnotes
        [Test] public void XForm_GoBibleRemoveFootnotes()
        {
            var vsIn = new string[] {
                "\\v 7",
                "\\vt Dia kasi tau, bilang, <<Nanti ada satu Orang yang lebe hebat dari beta mau datang. Biar cuma jadi Dia pung tukang suru-suru sa ju, beta sonde pantas.|fn",
                "\\ft 1:7: Tulisan bahasa Yunani asli tulis, bilang, <<tondo ko buka dia pung tali sapatu sa ju beta sonde pantas.>> Dia pung arti, bilang, Yohanis cuma orang kici sa, biar jadi Tuhan Yesus pung tukang suru-suru sa ju, dia sonde pantas.",
                "\\v 8",
                "\\vt Beta cuma bisa sarani sang bosong pake aer sa, ma nanti Dia bekin lebe dari beta, te Dia bekin ponu bosong pung hati deng Tuhan pung Roh yang Barisi.>>",
                "\\cf Mark 2:1"
            };
            var vsExpected = new string[] {
                "\\v 7",
                "\\vt Dia kasi tau, bilang, <<Nanti ada satu Orang yang lebe hebat dari beta mau datang. Biar cuma jadi Dia pung tukang suru-suru sa ju, beta sonde pantas.",
                "\\v 8",
                "\\vt Beta cuma bisa sarani sang bosong pake aer sa, ma nanti Dia bekin lebe dari beta, te Dia bekin ponu bosong pung hati deng Tuhan pung Roh yang Barisi.>>"
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the transform
            var transform = new XForm_GoBibleExport(DB);
            transform.RemoveFootnotes();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: XForm_GoBibleConvertSmartQuotesToOrdinary
        [Test]  public void XForm_GoBibleConvertSmartQuotesToOrdinary()
        {
            var vsIn = new string[] {
                "\\v 20 Syare <<<wapa ana> wapo>> rave mamo maisyare <Ayao> Amisye ama <<<ananyao>>> so rai:",
            };
            var vsExpected = new string[] {
                "\\v 20 Syare “‘wapa ana’ wapo” rave mamo maisyare ‘Ayao’ Amisye ama “‘ananyao’” so rai:",
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the method
            var transform = new XForm_GoBibleExport(DB);
            transform.ConvertSmartQuotesToOrdinary();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion

        #region Test: XForm_GoBibleRemoveCharacterFormating
        [Test] public void XForm_GoBibleRemoveCharacterFormating()
        {
            var vsIn = new string[] {
                "\\v 20 ||bSyare wapa ana||r wapo rave mamo maisyare ||iAyao||r Amisye ||iama.||r" 
            };
            var vsExpected = new string[] {
                "\\v 20 Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama.",
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the method
            var transform = new XForm_GoBibleExport(DB);
            transform.RemoveCharacterFormating();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: RemoveVerseBridgesForGoBible1
        [Test] public void RemoveVerseBridgesForGoBible1()
        {
            var vsIn = new string[] {
                "\\c 8",
                "\\v 1a \\wj Saulus paj'èra dhèu unu Lamatua Yesus sèra\\wj* Aa Saulus oo ètu era èèna kahèi, ka nèngu dèi kahèi ra pamadhe Stefanus.",
                "\\v 1b-2 Lod'o Stefanus madhe èèna, abhu dhèu pèri dhu dèi padètu dènge Lamatua. Lasi rare ngi'u aae Stefanus ho ra padhane. Ka ra tangi ne.",
                "\\v 3 Nèti lod'o èèna ka, dhèu paj'èra dhèu unu Lamatua Yesus ètu kota",
                "\\v 4a Dai sange ne'e ka, lii lolo Ama Lamatua paj'aj'i dedha-liru dènge rai-haha ne.",
                "\\v 4b Lod'o Ama LAMATUA heka paj'aj'i dedha-liru dènge rai-haha èèna,",
                "\\v 5-7a dhae abhu mèka ana aaj'u, lula Na papuru mèka èj'i.",
                "\\v 7b dhoka sèbu èi di, mai nèti dara rai ho pabasa"
            };
            var vsExpected = new string[] {
                "\\c 8",
                "\\v 1 (1a) \\wj Saulus paj'èra dhèu unu Lamatua Yesus sèra\\wj* Aa Saulus oo ètu era èèna kahèi, ka nèngu dèi kahèi ra pamadhe Stefanus.",
                "(1b-2) Lod'o Stefanus madhe èèna, abhu dhèu pèri dhu dèi padètu dènge Lamatua. Lasi rare ngi'u aae Stefanus ho ra padhane. Ka ra tangi ne.",
                "\\v 2",
                "\\v 3 Nèti lod'o èèna ka, dhèu paj'èra dhèu unu Lamatua Yesus ètu kota",
                "\\v 4 (4a) Dai sange ne'e ka, lii lolo Ama Lamatua paj'aj'i dedha-liru dènge rai-haha ne.",
                "(4b) Lod'o Ama LAMATUA heka paj'aj'i dedha-liru dènge rai-haha èèna,",
                "\\v 5 (5-7a) dhae abhu mèka ana aaj'u, lula Na papuru mèka èj'i.",
                "\\v 6",
                "\\v 7",
                "(7b) dhoka sèbu èi di, mai nèti dara rai ho pabasa"
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the method
            DB.RemoveVerseBridgesForGoBible();
            var vsActual = DB.ExtractData();
            for(int i = 0; i<vsActual.Length; i++)
                vsActual[i] = vsActual[i].Replace("\\(OmitMarker) ", "");

            // Is the result what we expected?
            //foreach (string s in vsActual)
            //    Console.WriteLine(s);
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion
        #region Test: RemoveVerseBridgesForGoBible2
        [Test] public void RemoveVerseBridgesForGoBible2()
        {
            var vsIn = new string[] {
                "\\v 20a-22c Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\v 23 <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q1 Nya ana nyo rave tai maisy, weamo nya marova so "
            };
            var vsExpected = new string[] {
                "\\v 20 (20a-22c) Syare wapa ana wapo rave mamo maisyare Ayao Amisye ama ananyao so rai:",
                "\\v 21",
                "\\v 22",
                "\\v 23 <<Ranivara nya marova maror, nyo anaisye raunanto po raisy, \\q2 muno awa ngkangkamandi, nyo mana raunanto po ramanam. \\q1 Nya ana nyo rave tai maisy, weamo nya marova so "
            };

            // Create a DB with the problem input stream
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vsIn);
            DB.Format = ScriptureDB.Formats.kGoBibleCreator;

            // Run the method
            DB.RemoveVerseBridgesForGoBible();

            // Is the result what we expected?
            var vsActual = DB.ExtractData();
            CompareStringArrays(vsExpected, vsActual);
        }
        #endregion

    }
}
