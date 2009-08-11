/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DBookGrouping.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DBookGrouping class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;
using JWdb.DataModel;

using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DBookGrouping
    {
        // Attrs
        DTeamSettings m_TeamSettings;
        DProject m_Project;
        DProject m_OldProject;

        // Generic Setup / Teardown
        #region Method: void Setup()
        [SetUp]
        public void Setup()
        {
            m_OldProject = DB.Project;

            // NUnit Setup
            JWU.NUnit_Setup();

            // Create a project with a whole bunch of books
            m_TeamSettings = new DTeamSettings();
            m_TeamSettings.EnsureInitialized();

            m_Project = new DProject();
            m_Project.TeamSettings = m_TeamSettings;
            DB.Project = m_Project;

            DTranslation tFront = new DTranslation("Kupang", "Latin", "Latin");
            DTranslation tTarget = new DTranslation("Dhao", "Latin", "Latin");
            m_Project.FrontTranslation = tFront;
            m_Project.TargetTranslation = tTarget;

            // Determine which books to populate the project with ****************************
            // IMPORTANT: IF I CHANGE THIS NEXT LINE OF CODE, I"LL BREAK MOST (POSSIBLY ALL) 
            // OF THE TESTS !!!
            // *******************************************************************************
            string[] vs = new string[] { "GEN", "EXO", "NUM", "MRK", "LUK", "JHN", "ACT", 
                "ROM", "HEB", "1PE", "2PE", "1JN", "JUD", "REV" };
            // *******************************************************************************

            foreach (string s in vs)
            {
                DBook FBook = new DBook(s); //, "F_" + s + ".db");
                tFront.AddBook(FBook);
                FBook.DisplayName = FBook.BookName;

                DBook TBook = new DBook(s); //, "T_" + s + ".db");
                tTarget.AddBook(TBook);
                TBook.DisplayName = TBook.BookName;			
            }
        }
        #endregion
        #region Method: void TearDown()
        [TearDown]
        public void TearDown()
        {
            DB.Project = m_OldProject;
        }
        #endregion

        // Tests
        #region Test: FindGroupingFor
        [Test]
        public void FindGroupingFor()
        {
            DBookGrouping bg = DBookGrouping.FindGroupingFor("GEN");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Pentateuch", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("EXO");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Pentateuch", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("NUM");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Pentateuch", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("MRK");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Gospels", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("LUK");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Gospels", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("JHN");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Gospels", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("ACT");
            Assert.IsNull(bg);

            bg = DBookGrouping.FindGroupingFor("ROM");
            Assert.IsNotNull(bg);
            Assert.AreEqual("Pauline Letters", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("HEB");
            Assert.IsNotNull(bg);
            Assert.AreEqual("General Letters", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("1PE");
            Assert.IsNotNull(bg);
            Assert.AreEqual("General Letters", bg.DefaultEnglishText);

            bg = DBookGrouping.FindGroupingFor("REV");
            Assert.IsNull(bg);
        }
        #endregion
        #region Test: CountTargetBooksInThisGrouping
        [Test]
        public void CountTargetBooksInThisGrouping()
        // Ensure that, over our standard test data, we are correctly counting
        // which books belong in the grouping
        {
            DBookGrouping bg = DBookGrouping.FindGroupingFor("GEN");
            Assert.IsNotNull(bg);
            Assert.AreEqual(3, bg.CountTargetBooksInThisGrouping());

            bg = DBookGrouping.FindGroupingFor("LUK");
            Assert.IsNotNull(bg);
            Assert.AreEqual(3, bg.CountTargetBooksInThisGrouping());

            bg = DBookGrouping.FindGroupingFor("LUK");
            Assert.IsNotNull(bg);
            Assert.AreEqual(3, bg.CountTargetBooksInThisGrouping());

            bg = DBookGrouping.FindGroupingFor("JUD");
            Assert.IsNotNull(bg);
            Assert.AreEqual(5, bg.CountTargetBooksInThisGrouping());
        }
        #endregion
        #region Test: void IncludesBook()
        [Test]
        public void IncludesBook()
        {
            DBookGrouping bg = DBookGrouping.FindGroupingFor("GEN");
            Assert.IsNotNull(bg);
            Assert.IsTrue(bg.IncludesBook("GEN"), "Should include GEN");
            Assert.IsTrue(bg.IncludesBook("EXO"), "Should include EXO");
            Assert.IsTrue(bg.IncludesBook("NUM"), "Should include NUM");
            Assert.IsFalse(bg.IncludesBook("1JN"), "Should not include 1JN");
            Assert.IsFalse(bg.IncludesBook("REV"), "Should not include REV");
        }
        #endregion
        #region Test: void PopulateGoToBook()
        [Test] public void PopulateGoToBook()
        {
            // Setup: create the expected menu
            ToolStripDropDownItem itemGotoBook = new ToolStripMenuItem("Book");

            ToolStripMenuItem group = new ToolStripMenuItem("Pentateuch");
            itemGotoBook.DropDownItems.Add(group);
            group.DropDownItems.Add(new ToolStripMenuItem("Genesis"));
            group.DropDownItems.Add(new ToolStripMenuItem("Exodus"));
            group.DropDownItems.Add(new ToolStripMenuItem("Numbers"));

            group = new ToolStripMenuItem("Gospels");
            itemGotoBook.DropDownItems.Add(group);
            group.DropDownItems.Add(new ToolStripMenuItem("Mark"));
            group.DropDownItems.Add(new ToolStripMenuItem("Luke"));
            group.DropDownItems.Add(new ToolStripMenuItem("John"));

            itemGotoBook.DropDownItems.Add(new ToolStripMenuItem("Acts"));

            itemGotoBook.DropDownItems.Add(new ToolStripMenuItem("Romans"));

            group = new ToolStripMenuItem("General Letters");
            itemGotoBook.DropDownItems.Add(group);
            group.DropDownItems.Add(new ToolStripMenuItem("Hebrews"));
            group.DropDownItems.Add(new ToolStripMenuItem("1 Peter"));
            group.DropDownItems.Add(new ToolStripMenuItem("2 Peter"));
            group.DropDownItems.Add(new ToolStripMenuItem("1 John"));
            group.DropDownItems.Add(new ToolStripMenuItem("Jude"));

            itemGotoBook.DropDownItems.Add(new ToolStripMenuItem("Revelation"));

            // Exercise: Create the actual menu
            ToolStripDropDownItem itemGotoBookActual = new ToolStripMenuItem("Book");
            DBookGrouping.PopulateGotoBook(itemGotoBookActual, null);

            // Compare
            Assert.AreEqual(itemGotoBook.DropDownItems.Count, itemGotoBookActual.DropDownItems.Count,
                "There should be 6 items at the top-level menu");
            for (int i = 0; i < itemGotoBook.DropDownItems.Count; i++)
            {
                // Check the top-level item
                ToolStripMenuItem itemExpected1 = itemGotoBook.DropDownItems[i]
                    as ToolStripMenuItem;
                ToolStripMenuItem itemActual1 = itemGotoBookActual.DropDownItems[i]
                    as ToolStripMenuItem;
                Assert.IsNotNull(itemExpected1);
                Assert.IsNotNull(itemActual1);
                Assert.AreEqual(itemExpected1.Text, itemActual1.Text);
                //                Console.WriteLine(itemExpected1.Text);

                // Check any subitems
                Assert.AreEqual(itemExpected1.DropDownItems.Count, itemActual1.DropDownItems.Count,
                    "There should be the same number of items at the sub menu");
                for (int k = 0; k < itemExpected1.DropDownItems.Count; k++)
                {
                    ToolStripMenuItem itemExpected2 = itemExpected1.DropDownItems[k]
                        as ToolStripMenuItem;
                    ToolStripMenuItem itemActual2 = itemActual1.DropDownItems[k]
                        as ToolStripMenuItem;
                    Assert.IsNotNull(itemExpected2);
                    Assert.IsNotNull(itemActual2);
                    Assert.AreEqual(itemExpected2.Text, itemActual2.Text);
                    //                    Console.WriteLine("..." + itemExpected2.Text);
                }
            }
        }
        #endregion
    }
}
