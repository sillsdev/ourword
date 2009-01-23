/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DReferenceSpan.cs
 * Author:  John Wimbish
 * Created: 01 Dec 2008
 * Purpose: Tests the DReferenceSpan and DReference classes
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DReference
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.EnsureInitialized();
            G.Project.DisplayName = "Test Project";
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
    }
}
