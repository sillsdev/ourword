﻿/**********************************************************************************************
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
using OurWordData.DataModel;
using JWTools;
using OurWordData;
using OurWordData.Styles;

#endregion


namespace OurWordTests.DataModel
{
    [TestFixture] public class T_StyleSheet
    {
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            TestCommon.GlobalTestSetup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Test Project";
        }
        #endregion

        #region Test: HuicholHyphenation
        [Test] public void HuicholHyphenation()
        {
            var ws = StyleSheet.FindOrCreate(WritingSystem.DefaultWritingSystemName);

            ws.UseAutomatedHyphenation = true;
            ws.Consonants = "bcdfghjklmnpqrstvwxyz";
            ws.HyphenationCVPattern = "V-C";
            ws.MinHyphenSplit = 3;

            var sLongWord = "caniyemieximecaitüni";

            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 9), "caniyemie-ximecaitüni");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 8), "caniyemi-eximecaitüni");
            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 16), "caniyemieximecai-tüni");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 14), "caniyemieximec-aitüni");
        }
        #endregion
        #region Test: EnglishHyphenation
        [Test] public void EnglishHyphenation()
        {
            var ws = StyleSheet.FindOrCreate(WritingSystem.DefaultWritingSystemName);

            ws.UseAutomatedHyphenation = true;
            ws.Consonants = "bcdfghjklmnpqrstvwxyz";
            ws.HyphenationCVPattern = "VC-C";
            ws.MinHyphenSplit = 3;

            var sLongWord = "itbogglesthemind";

            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 5), "itbog-glesthemind");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 2), "it-bogglesthemind");
            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 9), "itboggles-themind");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 10), "itbogglest-hemind");
        }
        #endregion

    }

}
