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
using OurWord.DataModel;
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
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.EnsureInitialized();
            G.Project.DisplayName = "Test Project";
        }
        #endregion

        #region Test: HuicholHyphenation
        [Test] public void HuicholHyphenation()
            // This is a temporary test for the Huichol CV rule. 
            // Hope to replace this sooner rather than later.
        {
            JWritingSystem ws = G.StyleSheet.FindWritingSystem("Latin");

            string sLongWord = "caniyemieximecaitüni";

            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 9), "caniyemie-ximecaitüni");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 8), "caniyemi-eximecaitüni");
            Assert.IsTrue(ws.IsHyphenBreak(sLongWord, 16), "caniyemieximecai-tüni");
            Assert.IsFalse(ws.IsHyphenBreak(sLongWord, 14), "caniyemieximec-aitüni");
        }
        #endregion
    }
}
