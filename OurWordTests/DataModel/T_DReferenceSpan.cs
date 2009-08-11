/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DReferenceSpan.cs
 * Author:  John Wimbish
 * Created: 01 Dec 2008
 * Purpose: Tests the DReferenceSpan and DReference classes
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DReference
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Test Project";
        }
        #endregion

        #region Test: CreateFromParsing
        [Test] public void CreateFromParsing()
        {
            DReference r = DReference.CreateFromParsing("3:16");
            Assert.AreEqual(3, r.Chapter);
            Assert.AreEqual(16, r.Verse);

            r = DReference.CreateFromParsing("04:2");
            Assert.AreEqual(4, r.Chapter);
            Assert.AreEqual(2, r.Verse);

            r = DReference.CreateFromParsing("119:121");
            Assert.AreEqual(119, r.Chapter);
            Assert.AreEqual(121, r.Verse);
        }
        #endregion

        #region Test: ParseParts
        [Test] public void ParseParts()
        {
            int nVerse1 = 0;
            char chLetter1 = ' ';
            int nVerse2 = 0;
            char chLetter2 = ' ';
            string sRemainder;

            DReference.ParseParts("16 For God so", out nVerse1, out chLetter1, out nVerse2, out chLetter2, out sRemainder);
            Assert.AreEqual(16, nVerse1);
            Assert.AreEqual(' ', chLetter1);
            Assert.AreEqual(-1, nVerse2);
            Assert.AreEqual(' ', chLetter2);
            Assert.AreEqual("For God so", sRemainder);

            DReference.ParseParts("16b-17a For God so", out nVerse1, out chLetter1, out nVerse2, out chLetter2, out sRemainder);
            Assert.AreEqual(16, nVerse1);
            Assert.AreEqual('b', chLetter1);
            Assert.AreEqual(17, nVerse2);
            Assert.AreEqual('a', chLetter2);
            Assert.AreEqual("For God so", sRemainder);

            DReference.ParseParts("15-17a", out nVerse1, out chLetter1, out nVerse2, out chLetter2, out sRemainder);
            Assert.AreEqual(15, nVerse1);
            Assert.AreEqual(' ', chLetter1);
            Assert.AreEqual(17, nVerse2);
            Assert.AreEqual('a', chLetter2);
            Assert.AreEqual("", sRemainder);

            DReference.ParseParts("15 - 17", out nVerse1, out chLetter1, out nVerse2, out chLetter2, out sRemainder);
            Assert.AreEqual(15, nVerse1);
            Assert.AreEqual(' ', chLetter1);
            Assert.AreEqual(17, nVerse2);
            Assert.AreEqual(' ', chLetter2);
            Assert.AreEqual("", sRemainder);

            DReference.ParseParts("15 17 soldiers came.", out nVerse1, out chLetter1, out nVerse2, out chLetter2, out sRemainder);
            Assert.AreEqual(15, nVerse1);
            Assert.AreEqual(' ', chLetter1);
            Assert.AreEqual(-1, nVerse2);
            Assert.AreEqual(' ', chLetter2);
            Assert.AreEqual("17 soldiers came.", sRemainder);
        }
        #endregion
    }
}
