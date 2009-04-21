/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_StyleSheet.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the Stylsheet implementation
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
using JWdb.DataModel;
using JWTools;
using JWdb;
#endregion


namespace OurWordTests.JWdb
{
    [TestFixture] public class T_StyleSheet
    {
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Test Project";
        }
        #endregion

        #region Test: HuicholHyphenation
        [Test] public void HuicholHyphenation()
        {
            JWritingSystem ws = DB.StyleSheet.FindWritingSystem("Latin");

            ws.UseAutomatedHyphenation = true;
            ws.Consonants = "bcdfghjklmnpqrstvwxyz";
            ws.HyphenationCVPattern = "V-C";
            ws.MinHyphenSplit = 3;

            string sLongWord = "caniyemieximecaitüni";

            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 9), "caniyemie-ximecaitüni");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 8), "caniyemi-eximecaitüni");
            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 16), "caniyemieximecai-tüni");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 14), "caniyemieximec-aitüni");
        }
        #endregion
        #region Test: EnglishHyphenation
        [Test] public void EnglishHyphenation()
        {
            JWritingSystem ws = DB.StyleSheet.FindWritingSystem("Latin");

            ws.UseAutomatedHyphenation = true;
            ws.Consonants = "bcdfghjklmnpqrstvwxyz";
            ws.HyphenationCVPattern = "VC-C";
            ws.MinHyphenSplit = 3;

            string sLongWord = "itbogglesthemind";

            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 5), "itbog-glesthemind");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 2), "it-bogglesthemind");
            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 9), "itboggles-themind");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 10), "itbogglest-hemind");
        }
        #endregion

    }
}
