/**********************************************************************************************
 * Project: Our Word!
 * File:    DBookGrouping.cs
 * Author:  John Wimbish
 * Created: 6 Aug 2007
 * Purpose: For navigation (GoTo Book menu), supports the user-definable ability to group
 *    books into nodes (e.g., Pentateuch, Historical, etc.) so that the menu does not become
 *    too large when the person is dealing with an entire Bible.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.IO;

using JWTools;
using JWdb;

using OurWord.Dialogs;
using OurWord.View;

using NUnit.Framework;
#endregion

namespace OurWord.DataModel
{
    public class DBookGrouping : JObject
    {
        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string LocKey - Place in Localizations.db with the UI string
        public string LocKey
        {
            get
            {
                return m_sLocKey;
            }
            set
            {
                SetValue(ref m_sLocKey, value);
            }
        }
        private string m_sLocKey;
        #endregion
        #region BAttr{g/s}: string DefaultEnglishText - English to display in GoTo menu
        public string DefaultEnglishText
        {
            get
            {
                return m_sDefaultEnglishText;
            }
            set
            {
                SetValue(ref m_sDefaultEnglishText, value);
            }
        }
        private string m_sDefaultEnglishText;
        #endregion
        #region BAttr{g}: BStringArray BookAbbrevs - The books which fall under this grouping
        public BStringArray BookAbbrevs
        {
            get
            {
                Debug.Assert(null != m_bsaBookAbbrevs);
                return m_bsaBookAbbrevs;
            }
        }
        public BStringArray m_bsaBookAbbrevs = null;
        #endregion
        #region Method: void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("LocKey", ref m_sLocKey);
            DefineAttr("English", ref m_sDefaultEnglishText);
            DefineAttr("BookAbbrevs", ref m_bsaBookAbbrevs);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DBookGrouping()
            : base()
        {
            // Complex basic attrs
            m_bsaBookAbbrevs = new BStringArray();
        }
        #endregion
        #region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
        public override string SortKey
        // In order to support sorting, the subclass must implement a SortKey attribute,
        // and this SortKey must return something other than an empty string. 
        {
            get
            {
                return LocKey;
            }
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: int CountTargetBooksInThisGrouping()
        public int CountTargetBooksInThisGrouping()
        {
            int c = 0;
            foreach (DBook book in G.Project.Nav.PotentialTargetBooks)
            {
                if (-1 != BookAbbrevs.FindFirstPosition(book.BookAbbrev))
                    c++;
            }
            return c;
        }
        #endregion
        #region Method: bool IncludesBook(string sBookAbbrev)
        public bool IncludesBook(string sBookAbbrev)
        {
            if (-1 != BookAbbrevs.FindFirstPosition(sBookAbbrev))
                return true;
            return false;
        }
        #endregion
        #region Method: string GetUIText()
        public string GetUIText()
        {
            return G.GetLoc_BookGroupings( LocKey, DefaultEnglishText);
        }
        #endregion

        // Static Work Methods ---------------------------------------------------------------
        #region SMethod: void InitializeGroupings(JOwnSeq seq)
        static public void InitializeGroupings(JOwnSeq seq)
        {
            seq.Clear();

            DBookGrouping bg = new DBookGrouping();
            bg.LocKey = "Pentateuch";
            bg.DefaultEnglishText = "Pentateuch";
            bg.BookAbbrevs.Read("5 {GEN} {EXO} {LEV} {NUM} {DEU}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Historical";
            bg.DefaultEnglishText = "Historical";
            bg.BookAbbrevs.Read("12 {JOS} {JDG} {RUT} {1SA} {2SA} {1KI} {2KI} " +
                "{1CH} {2CH} {EZR} {NEH} {EST}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Poetical";
            bg.DefaultEnglishText = "Poetical";
            bg.BookAbbrevs.Read("5 {JOB} {PSA} {PRO} {ECC} {SNG}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Prophetic";
            bg.DefaultEnglishText = "Prophetic";
            bg.BookAbbrevs.Read("17 {ISA} {JER} {LAM} {EZK} {DAN} {HOS} {JOL} {AMO} " +
                "{OBA} {JON} {MIC} {NAM} {HAB} {ZEP} {HAG} {ZEC} {MAL}" );
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Gospels";
            bg.DefaultEnglishText = "Gospels";
            bg.BookAbbrevs.Read("4 {MAT} {MRK} {LUK} {JHN}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "PaulineLetters";
            bg.DefaultEnglishText = "Pauline Letters";
            bg.BookAbbrevs.Read("13 {ROM} {1CO} {2CO} {GAL} {EPH} {PHP} {COL} {1TH} " +
                "{2TH} {1TI} {2TI} {TIT} {PHM}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "GeneralLetters";
            bg.DefaultEnglishText = "General Letters";
            bg.BookAbbrevs.Read("8 {HEB} {JAS} {1PE} {2PE} {1JN} {2JN} {3JN} {JUD}");
            seq.Append(bg);
        }
        #endregion

        const int cMinBooksRequiredToActivitateGroupings = 12;
        const int cMinBooksWithinGrouping = 2;

        #region SMethod: DBookGrouping FindGroupingFor(string sBookAbbrev)
        static public DBookGrouping FindGroupingFor(string sBookAbbrev)
        {
            foreach (DBookGrouping bg in G.TeamSettings.BookGroupings)
            {
                if (bg.IncludesBook(sBookAbbrev))
                    return bg;
            }
            return null;
        }
        #endregion

        #region SMethod: void PopulateGotoBook(ToolStripDropDownItem, onClick)
        /// <summary>
        /// Creates the subitems for the GoTo menu or button
        /// </summary>
        /// <param name="itemGoTo">The GoTo Book item, which may either be a menu item, or a drop-down button,</param>
        /// <param name="onClick">The event handler to call for any Book menu items that will be inserted.</param>
        static public void PopulateGotoBook(ToolStripDropDownItem itemGoToBook, EventHandler onClick)
        {
            // Clear anything that is already there, so we can build from scratch
            itemGoToBook.DropDownItems.Clear();

            // How many books do we have? (If too few, we will not want to nest with subitems)
            int cBooks = G.Project.Nav.PotentialTargetBooks.Length;

            // Loop through the books, adding them to the appropriate place in the menu
            foreach (DBook book in G.Project.Nav.PotentialTargetBooks)
            {
                // Default to placing the book as a dropdown within the top-level GoToBook item
                ToolStripDropDownItem itemParent = itemGoToBook;

                // Find the sub-grouping (if any) to which this book belongs
                DBookGrouping group = FindGroupingFor(book.BookAbbrev);

                // Optionally, we'll place the book into a sub-grouping. Conditions are:
                // 1. There are enough books in the project to justify subgroupings,
                // 2. A DBookGrouping was found for this book, and
                // 3. There are enough books in this grouping to justify using it.
                if (cBooks > cMinBooksRequiredToActivitateGroupings &&
                    null != group &&
                    group.CountTargetBooksInThisGrouping() >= cMinBooksWithinGrouping)
                {
                    // If the Node for this group already exists, then set the Parent to it
                    // and we're ready to append to it.
                    foreach (ToolStripMenuItem iGrouping in itemGoToBook.DropDownItems)
                    {
                        if (iGrouping.Text == group.GetUIText())
                        {
                            itemParent = iGrouping;
                            break;
                        }
                    }

                    // If the Node does not exist, then create it.
                    if (itemParent == itemGoToBook)
                    {
                        ToolStripMenuItem itemGrouping = new ToolStripMenuItem(group.GetUIText());
                        itemGrouping.Name = "menu" + group.DefaultEnglishText;
                        itemGoToBook.DropDownItems.Add(itemGrouping);
                        itemParent = itemGrouping;
                    }
                }

                // Create the menu item, showing the display name
                ToolStripMenuItem itemBook = new ToolStripMenuItem(book.DisplayName, null, onClick);
                itemBook.Name = "menu" + book.DisplayName;

                // For a locked book, write its text as Red color
                if (book.Locked)
                    itemBook.ForeColor = Color.Red;

                // Now we can add it to the menu hierarchy.
                itemParent.DropDownItems.Add(itemBook);

                // Check it if it is the active book
                itemBook.Checked = (book.BookAbbrev == G.Project.Nav.BookAbbrev);

            } // endloop
        }
        #endregion
    }

    #region NUnit: Test_DBookGrouping
    [TestFixture] public class Test_DBookGrouping
    {
        // Attrs
        DTeamSettings m_TeamSettings;
        DProject m_Project;
        DProject m_OldProject;

        // Generic Setup / Teardown
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            m_OldProject = OurWordMain.Project;

            // NUnit Setup
            JWU.NUnit_Setup();

            // Create a project with a whole bunch of books
            m_TeamSettings = new DTeamSettings();
            m_TeamSettings.InitializeFactoryStyleSheet();

            m_Project = new DProject();
            m_Project.TeamSettings = m_TeamSettings;
            OurWordMain.Project = m_Project;

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
                DBook FBook = new DBook(s, "F_" + s + ".db");
                tFront.AddBook(FBook);
                FBook.DisplayName = FBook.BookName;

                DBook TBook = new DBook(s, "T_" + s + ".db");
                tTarget.AddBook(TBook);
                TBook.DisplayName = TBook.BookName;
            }
        }
        #endregion
        #region Method: void TearDown()
        [TearDown] public void TearDown()
        {
            OurWordMain.Project = m_OldProject;
        }
        #endregion

        // Tests
        #region Test: FindGroupingFor
        [Test] public void FindGroupingFor()
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
        [Test] public void CountTargetBooksInThisGrouping()
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
        [Test] public void IncludesBook()
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
    #endregion
}
