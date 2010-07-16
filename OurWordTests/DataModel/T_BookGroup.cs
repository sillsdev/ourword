using System.Windows.Forms;
using NUnit.Framework;
using OurWord;
using OurWord.Ctrls.Navigation;
using OurWordData.DataModel;

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class T_BookGroup
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        // Construction
        #region Test: TConstruction_Groups
        [Test]
        public void TConstruction_Groups()
            // Upon construction, all 66 books should be properly grouped
        {
            var groups = new BookGroups();

            Assert.AreEqual(9, groups.Count);
            Assert.AreEqual(66, groups.AllBookInfos.Count);

            var pentateuch = groups.FindOrAddGroup(BookGroup.Pentateuch);
            Assert.AreEqual(5, pentateuch.Count);

            var historical = groups.FindOrAddGroup(BookGroup.Historical);
            Assert.AreEqual(12, historical.Count);

            var poetical = groups.FindOrAddGroup(BookGroup.Poetical);
            Assert.AreEqual(5, poetical.Count);

            var prophetic = groups.FindOrAddGroup(BookGroup.Prophetic);
            Assert.AreEqual(17, prophetic.Count);

            var gospels = groups.FindOrAddGroup(BookGroup.Gospels);
            Assert.AreEqual(4, gospels.Count);

            var acts = groups.FindOrAddGroup(BookGroup.Acts);
            Assert.AreEqual(1, acts.Count);

            var paulineLetters = groups.FindOrAddGroup(BookGroup.PaulineLetters);
            Assert.AreEqual(13, paulineLetters.Count);

            var generalLetters = groups.FindOrAddGroup(BookGroup.GeneralLetters);
            Assert.AreEqual(8, generalLetters.Count);

            var revelation = groups.FindOrAddGroup(BookGroup.Revelation);
            Assert.AreEqual(1, revelation.Count);
        }
        #endregion
        #region Test: TConstruction_Counts
        [Test] 
        public void TConstruction_Counts()
            // Spot check a few to make sure the Verse and Chapter counts are accurate
        {
            var groups = new BookGroups();

            var gen = groups.FindBook("GEN");
            Assert.AreEqual(50, gen.ChapterCount);
            Assert.AreEqual(1533, gen.VerseCount);
            Assert.AreEqual(BookGroup.Pentateuch, gen.Group.EnglishName);

            var ezr = groups.FindBook("EZR");
            Assert.AreEqual(10, ezr.ChapterCount);
            Assert.AreEqual(280, ezr.VerseCount);
            Assert.AreEqual(BookGroup.Historical, ezr.Group.EnglishName);

            var psa = groups.FindBook("PSA");
            Assert.AreEqual(150, psa.ChapterCount);
            Assert.AreEqual(2461, psa.VerseCount);
            Assert.AreEqual(BookGroup.Poetical, psa.Group.EnglishName);

            var jud = groups.FindBook("JUD");
            Assert.AreEqual(1, jud.ChapterCount);
            Assert.AreEqual(25, jud.VerseCount);
            Assert.AreEqual(BookGroup.GeneralLetters, jud.Group.EnglishName);
        }
        #endregion
        #region Test: FindGroupingFor
        [Test]
        public void FindGroupingFor()
        {
            var info = G.BookGroups.FindBook("GEN");
            Assert.AreEqual("Pentateuch", info.Group.EnglishName);
            info = G.BookGroups.FindBook("EXO");
            Assert.AreEqual("Pentateuch", info.Group.EnglishName);
            info = G.BookGroups.FindBook("NUM");
            Assert.AreEqual("Pentateuch", info.Group.EnglishName);

            info = G.BookGroups.FindBook("MRK");
            Assert.AreEqual("Gospels", info.Group.EnglishName);
            info = G.BookGroups.FindBook("LUK");
            Assert.AreEqual("Gospels", info.Group.EnglishName);
            info = G.BookGroups.FindBook("JHN");
            Assert.AreEqual("Gospels", info.Group.EnglishName);

            info = G.BookGroups.FindBook("ROM");
            Assert.AreEqual("Pauline Letters", info.Group.EnglishName);

            info = G.BookGroups.FindBook("HEB");
            Assert.AreEqual("General Letters", info.Group.EnglishName);
            info = G.BookGroups.FindBook("1PE");
            Assert.AreEqual("General Letters", info.Group.EnglishName);
        }
        #endregion

        // Populate Goto Book DropDown
        #region Test: PopulateGoToBookDropDownButton_Flat
        [Test]
        public void PopulateGoToBookDropDownButton_Flat()
        {
            // Setup: create the expected menu
            TestCommon.CreateHierarchyThroughTargetTranslation();

            ToolStripDropDownItem expected = new ToolStripMenuItem("Book");
            expected.DropDownItems.Add(new ToolStripMenuItem("Genesis"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Mark"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Luke"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Acts"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Romans"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Hebrews"));
            expected.DropDownItems.Add(new ToolStripMenuItem("1 Peter"));
            expected.DropDownItems.Add(new ToolStripMenuItem("2 Peter"));
            expected.DropDownItems.Add(new ToolStripMenuItem("1 John"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Jude"));
            expected.DropDownItems.Add(new ToolStripMenuItem("Revelation"));

            // Exercise: Create the actual menu
            var vBooks = new[] 
            { 
                new DBook("GEN"),
                new DBook("MRK"),
                new DBook("LUK"),
                new DBook("ACT"),
                new DBook("ROM"),
                new DBook("HEB"),
                new DBook("1PE"),
                new DBook("2PE"),
                new DBook("1JN"),
                new DBook("JUD"),
                new DBook("REV"),
            };
            foreach (var book in vBooks)
                DB.TargetTranslation.AddBook(book);
            var ctrl = new CtrlNavigation();
            ctrl.SetupGotoBookDropDown(vBooks);

            // Compare
            CompareDropDowns(expected, ctrl.m_Book);
        }
        #endregion
        #region Test: PopulateGoToBookDropDownButton_Hierarchical
        [Test]
        public void PopulateGoToBookDropDownButton_Hierarchical()
        {
            // Setup: create the expected menu
            TestCommon.CreateHierarchyThroughTargetTranslation();

            ToolStripDropDownItem expected = new ToolStripMenuItem("Book");

            var pentateuch = new ToolStripMenuItem("Pentateuch");
            expected.DropDownItems.Add(pentateuch);
            pentateuch.DropDownItems.Add(new ToolStripMenuItem("Genesis"));
            pentateuch.DropDownItems.Add(new ToolStripMenuItem("Exodus"));
            pentateuch.DropDownItems.Add(new ToolStripMenuItem("Numbers"));

            var gospels = new ToolStripMenuItem("Gospels");
            expected.DropDownItems.Add(gospels);
            gospels.DropDownItems.Add(new ToolStripMenuItem("Mark"));
            gospels.DropDownItems.Add(new ToolStripMenuItem("Luke"));
            gospels.DropDownItems.Add(new ToolStripMenuItem("John"));

            expected.DropDownItems.Add(new ToolStripMenuItem("Acts"));

            expected.DropDownItems.Add(new ToolStripMenuItem("Romans"));

            var general = new ToolStripMenuItem("General Letters");
            expected.DropDownItems.Add(general);
            general.DropDownItems.Add(new ToolStripMenuItem("Hebrews"));
            general.DropDownItems.Add(new ToolStripMenuItem("1 Peter"));
            general.DropDownItems.Add(new ToolStripMenuItem("2 Peter"));
            general.DropDownItems.Add(new ToolStripMenuItem("1 John"));
            general.DropDownItems.Add(new ToolStripMenuItem("Jude"));

            expected.DropDownItems.Add(new ToolStripMenuItem("Revelation"));

            // Exercise: Create the actual menu
            var vBooks = new[] 
            { 
                new DBook("GEN"),
                new DBook("EXO"),
                new DBook("NUM"),
                new DBook("MRK"),
                new DBook("LUK"),
                new DBook("JHN"),
                new DBook("ACT"),
                new DBook("ROM"),
                new DBook("HEB"),
                new DBook("1PE"),
                new DBook("2PE"),
                new DBook("1JN"),
                new DBook("JUD"),
                new DBook("REV"),
            };
            foreach (var book in vBooks)
                DB.TargetTranslation.AddBook(book);
            var ctrl = new CtrlNavigation();
            ctrl.SetupGotoBookDropDown(vBooks);

            // Compare
            CompareDropDowns(expected, ctrl.m_Book);
        }
        #endregion
        #region smethod: void CompareDropDowns(expected, actual)
        static void CompareDropDowns(ToolStripDropDownItem expected, ToolStripDropDownItem actual)
        {
            Assert.AreEqual(expected.DropDownItems.Count, actual.DropDownItems.Count);

            for (var i = 0; i < actual.DropDownItems.Count; i++)
            {
                // Check the top-level item
                var itemExpected = expected.DropDownItems[i] as ToolStripMenuItem;
                var itemActual = actual.DropDownItems[i] as ToolStripMenuItem;
                CompareItems(itemExpected, itemActual);

                // Check any subitems
                Assert.AreEqual(itemExpected.DropDownItems.Count, itemActual.DropDownItems.Count,
                    "There should be the same number of items at the sub menu");
                for (var k = 0; k < itemExpected.DropDownItems.Count; k++)
                {
                    var subExpected = itemExpected.DropDownItems[k] as ToolStripMenuItem;
                    var subActual = itemActual.DropDownItems[k] as ToolStripMenuItem;
                    CompareItems(subExpected, subActual);
                }
            }
        }
        #endregion
        #region smethod: void CompareItems(expected, actual)
        static void CompareItems(ToolStripItem expected, ToolStripItem actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Text, actual.Text);
        }
        #endregion

    }

}
