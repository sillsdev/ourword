/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_AutoReplace.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the AutoReplace implementation
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.JWdb
{
    [TestFixture] public class T_AutoReplace
    {
        // Setup/TearDown --------------------------------------------------------------------
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: AddThenSearch
        [Test] public void AddThenSearch()
        {
            TreeRoot tr = new TreeRoot();

            #region Test Data
            string[] Source = 
				{
					"makan",
					"minum",
					"malam",
					"menolong",
					"mepertolongkan",
					"menulis",
					"menurun",
					"memperbaiki",
					"bikin",
					"terbuat",
					"makanan",
					"sudah makan",
					"Pasar",
					"pasar",
					"dbl",
					"Uis",
					"UIS",
					"Sandra",
					"Emily",
					"David",
					"Robert",
					"Christiane",
					"cow",
					"ow",
					"tired",
					"hungry",
					"fast",
					"slow",
					"fat",
					"thin",
					"JSW"
				};
            string[] Result = 
				{
					"eat",
					"drink",
					"night",
					"help",
					"cause to help",
					"write",
					"lower",
					"repair",
					"make",
					"something made",
					"food",
					"already ate",
					"Market",
					"market",
					"(doublet)",
					"God",
					"the LORD",
					"Sandra Gail Wimbish",
					"Emily Gail Wimbish",
					"Richard David Wimbish",
					"Robert Taylor Wimbish",
					"Christiane Elizabeth Wimbish",
					"Black Angus",
					"Our Word",
					"sleepy",
					"thirsty",
					"slow",
					"fast",
					"obesse",
					"skinny",
					"John S. Wimbish"
				};
            #endregion

            // Make sure I made no clerical errors
            Assert.AreEqual(Source.Length, Result.Length, "No Clerical Errors");
            int cSourceLen = 0;

            // Add everything
            for (int i = 0; i < Source.Length; i++)
                tr.Add(Source[i], Result[i]);

            // Did we find everything?
            for (int i = 0; i < Source.Length; i++)
            {
                Assert.AreEqual(Result[i], tr.Search(Source[i], ref cSourceLen));
                Assert.AreEqual(Source[i].Length, cSourceLen);
            }

            // Do we NOT find stuff we shouldn't?
            Assert.IsNull(tr.Search("repair", ref cSourceLen), "Shouldn't find 'repair'");
            Assert.IsNull(tr.Search("biki", ref cSourceLen), "Shouldn't find 'biki'");
            Assert.IsNull(tr.Search("men", ref cSourceLen), "Shouldn't find 'men'");
            Assert.IsNull(tr.Search("Bikin", ref cSourceLen), "Shouldn't find 'Bikin'");

            // We should find something for contained strings
            Assert.AreEqual("help", tr.Search("saya mau menolong", ref cSourceLen), "Should find 'help'");
            Assert.AreEqual("drink", tr.Search("jangan minum", ref cSourceLen), "Should find 'drink'");

            // Should differentiate between capitalization
            Assert.IsNotNull(tr.Search("menulis", ref cSourceLen), "Capialization: 'menulis'");
            Assert.IsNull(tr.Search("Menulis", ref cSourceLen), "Capialization: 'Menulis'");
        }
        #endregion
    }
}
