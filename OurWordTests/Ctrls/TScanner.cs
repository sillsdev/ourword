#region *** TScanner.cs ***
using System.Linq;
using NUnit.Framework;
using OurWord.Ctrls.Navigation;
using OurWord.Edit;
using OurWordData.DataModel.Runs;
using OurWordTests.Edit;
#endregion

namespace OurWordTests.Ctrls
{
    [TestFixture]
    public class TScanner
    {
        #region Method: void TearDown()
        [TearDown] public void TearDown()
        {
            EditTest.TearDown();
        }
        #endregion

        const string sOxesXml = "<book id=\"EPH\" stage=\"Final\" version=\"B\">" +
            "<p class=\"Section Head\" usfm=\"s1\">Greeting</p>" +
            "<p class=\"Paragraph\" usfm=\"p\"><c n=\"1\" /><v n=\"1\" />Paul, an apostle of Christ Jesus by the will of God,</p>" +
            "<p class=\"Paragraph\" usfm=\"p\">To the saints in Ephesus, the faithful in Christ Jesus:</p>" +
            "<p class=\"Paragraph\" usfm=\"p\"><v n=\"2\" />Grace and peace to you from God the Father and the Lord Jesus Christ.</p>" +
            "<p class=\"Section Head\" usfm=\"s1\">Praise</p>" +
            "<p class=\"Paragraph\" usfm=\"p\"><v n=\"3\" />Praise be to the God and Father of our Lord Jesus Christ, who has blessed us in the heavenly realms through Jesus Christ. <v n=\"4\" />For he chose us before the creation of the world to be holy and blameless in his sight.</p>" +
            "<p class=\"Paragraph\" usfm=\"p\">In love he predestined us to be adopted as his sons through Jesus Christ.</p>" +
            "<p class=\"Paragraph\" usfm=\"p\"><v n=\"5\" />in accordance with his pleasure and will,</p>" +
            "<p class=\"Paragraph\" usfm=\"p\">to the praise of his glorious grace, which he has freely given us in the one he loves.</p>" +
            "</book>";

        #region Test: TScanString
        [Test] public void TScanString()
        {
            const string csSource = "For God so loved the world he gave his one and only son, " + 
                "that whosoever believes in him should not perish but have eternal life.";

            var context = new Scanner.SearchContext("Not in there", null, null, 0);
            var vShouldNotFind = Scanner.ScanString(context, csSource);
            Assert.AreEqual(0, vShouldNotFind.Count());

            context = new Scanner.SearchContext("For", null, null, 0);
            var vShouldFindFirst = Scanner.ScanString(context, csSource);
            Assert.AreEqual(1, vShouldFindFirst.Count());
            Assert.AreEqual(0, vShouldFindFirst[0]);

            context = new Scanner.SearchContext("life.", null, null, 0);
            var vShouldFindLast = Scanner.ScanString(context, csSource);
            Assert.AreEqual(1, vShouldFindLast.Count());
            Assert.AreEqual(123, vShouldFindLast[0]);

            context = new Scanner.SearchContext("God", null, null, 0);
            var vShouldFindMiddle = Scanner.ScanString(context, csSource);
            Assert.AreEqual(1, vShouldFindMiddle.Count());
            Assert.AreEqual(4, vShouldFindMiddle[0]);

            context = new Scanner.SearchContext("so", null, null, 0);
            var vShouldFindMultiple = Scanner.ScanString(context, csSource);
            Assert.AreEqual(3, vShouldFindMultiple.Count());
            Assert.AreEqual(8, vShouldFindMultiple[0]);
            Assert.AreEqual(52, vShouldFindMultiple[1]);
            Assert.AreEqual(65, vShouldFindMultiple[2]);
        }
        #endregion
        #region Test: TScanBook_Original
        [Test] public void TScanBook_Original()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);
            var dbt = book.Sections[0].Paragraphs[0].Runs[0] as DBasicText;
            Assert.IsNotNull(dbt);

            // Multiple times
            var context = new Scanner.SearchContext("Jesus", book, dbt.GetPathFromRoot(), 0);
            var vTexts = Scanner.GetTexts(book);
            var vHits = Scanner.ScanTexts(context, vTexts);
            Assert.AreEqual(6, vHits.Count);

            Assert.AreEqual("EPH", vHits[0].BookAbbrev);
            Assert.AreEqual(1, vHits[0].Chapter);
            Assert.AreEqual(1, vHits[0].Verse);
            Assert.AreEqual(0, vHits[0].SectionNo);
            Assert.AreEqual(1, vHits[0].ParagraphNo);
            Assert.AreEqual(2, vHits[0].RunNo);

            Assert.AreEqual("EPH", vHits[4].BookAbbrev);
            Assert.AreEqual(1, vHits[4].SectionNo);
            Assert.AreEqual(1, vHits[4].ParagraphNo);
            Assert.AreEqual(1, vHits[4].RunNo);
        }
        #endregion

        #region Test: TGetTextsToScan_All
        [Test] public void TGetTextsToScan_All()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Do the scan
            var v = Scanner.GetTexts(book);

            // Test
            Assert.AreEqual(10, v.Count);
            Assert.AreEqual("Greeting", v[0].ContentsAsString);
            Assert.AreEqual("Praise", v[4].ContentsAsString);
        }
        #endregion
        #region Test: TGetTextsToScan_PriorTo
        [Test] public void TGetTextsToScan_PriorTo()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Set a target we'll not exceed
            var target = book.Sections[0].Paragraphs[2].Runs[0] as DBasicText;
            Assert.IsNotNull(target);
            Assert.IsTrue(target.ContentsAsString.StartsWith("To the saints"));

            // Do the scan
            var v = Scanner.GetTextsPrior(target);

            // Test
            Assert.AreEqual(2, v.Count);
            Assert.AreEqual("Greeting", v[0].ContentsAsString);
            Assert.IsTrue(v[1].ContentsAsString.StartsWith("Paul, an apostle"));
        }
        #endregion
        #region Test: TGetTextsToScan_After
        [Test] public void TGetTextsToScan_After()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Set a target we'll get stuff including and after
            var target = book.Sections[0].Paragraphs[2].Runs[0] as DBasicText;
            Assert.IsNotNull(target);
            Assert.IsTrue(target.ContentsAsString.StartsWith("To the saints"));

            // Do the scan
            var v = Scanner.GetTextsAfter(target);

            // Test
            Assert.AreEqual(7, v.Count);
            Assert.IsTrue(v[0].ContentsAsString.StartsWith("Grace and peace"));
        }
        #endregion

        #region Test: TScanBook_firstWordInBook
        [Test] public void TScanBook_firstWordInBook()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);
            var dbt = book.Sections[0].Paragraphs[0].Runs[0] as DBasicText;
            Assert.IsNotNull(dbt);

            // Scan
            var vTexts = Scanner.GetTexts(book);
            var context = new Scanner.SearchContext("Greeting", book, dbt.GetPathFromRoot(), 0);
            var vLookup = Scanner.ScanTexts(context, vTexts);

            // Test
            Assert.IsNotNull(vLookup);
            Assert.AreEqual(1, vLookup.Count);
            Assert.AreEqual(0, vLookup[0].SectionNo);
            Assert.AreEqual(0, vLookup[0].ParagraphNo);
            Assert.AreEqual(0, vLookup[0].RunNo);
            Assert.AreEqual(0, vLookup[0].IndexIntoText);
        }
        #endregion
        #region Test: TScanBook_firstOccurrence
        [Test] public void TScanBook_firstOccurrence()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);
            var dbt = book.Sections[0].Paragraphs[0].Runs[0] as DBasicText;
            Assert.IsNotNull(dbt);

            // Scan
            var vTexts = Scanner.GetTexts(book);
            var context = new Scanner.SearchContext("faithful", book, dbt.GetPathFromRoot(), 0);
            var vLookup = Scanner.ScanTexts(context, vTexts);

            // Test
            Assert.IsNotNull(vLookup);
            Assert.AreEqual(1, vLookup.Count);
            Assert.AreEqual(0, vLookup[0].SectionNo);
            Assert.AreEqual(2, vLookup[0].ParagraphNo);
            Assert.AreEqual(0, vLookup[0].RunNo);
            Assert.AreEqual(30, vLookup[0].IndexIntoText);
        }
        #endregion
        #region Test: TScanBook_finalOccurrence
        [Test] public void TScanBook_finalOccurrence()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);
            var dbt = book.Sections[0].Paragraphs[0].Runs[0] as DBasicText;
            Assert.IsNotNull(dbt);

            // Scan
            var vTexts = Scanner.GetTexts(book);
            var context = new Scanner.SearchContext("loves.", book, dbt.GetPathFromRoot(), 0);
            var vLookup = Scanner.ScanTexts(context, vTexts);

            // Test
            Assert.IsNotNull(vLookup);
            Assert.AreEqual(1, vLookup.Count);
            Assert.AreEqual(1, vLookup[0].SectionNo);
            Assert.AreEqual(4, vLookup[0].ParagraphNo);
            Assert.AreEqual(0, vLookup[0].RunNo);
            Assert.AreEqual(80, vLookup[0].IndexIntoText);
        }
        #endregion
        #region Test: TScanBook_priorAndAfter
        [Test] public void TScanBook_priorAndAfter()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Set up a 'window' on the second section;
            EditTest.Setup(book.Sections[1]);
            // Put the selection on the second paragraph, 'In love he |predestined..."
            var owp = EditTest.Wnd.Contents.SubItems[2] as OWPara;
            var selection = new OWWindow.Sel(owp, 3, 0);

            // Should not find 'accordance' before; but yes to after
            var context = new Scanner.SearchContext("accordance", selection);
            var lookupInfo = Scanner.ScanTexts(context, Scanner.GetTextsPrior(context.OriginalDbt));
            Assert.AreEqual(0, lookupInfo.Count);
            lookupInfo = Scanner.ScanTexts(context, Scanner.GetTextsAfter(context.OriginalDbt));
            Assert.AreEqual(1, lookupInfo.Count);

            // Should not find "Ephesus" after, but yes to before
            context = new Scanner.SearchContext("Ephesus,", selection);
            lookupInfo = Scanner.ScanTexts(context, Scanner.GetTextsPrior(context.OriginalDbt));
            Assert.AreEqual(1, lookupInfo.Count);
            lookupInfo = Scanner.ScanTexts(context, Scanner.GetTextsAfter(context.OriginalDbt));
            Assert.AreEqual(0, lookupInfo.Count);

            // Neither should find the word we're sitting on
            context = new Scanner.SearchContext("predestined", selection);
            lookupInfo = Scanner.ScanTexts(context, Scanner.GetTextsPrior(context.OriginalDbt));
            Assert.AreEqual(0, lookupInfo.Count);
            lookupInfo = Scanner.ScanTexts(context, Scanner.GetTextsAfter(context.OriginalDbt));
            Assert.AreEqual(0, lookupInfo.Count);
        }
        #endregion

        #region Test: TGetTextsAndFootnotes
        [Test] public void TGetTextsAndFootnotes()
        {
            const string sXml = "<book id=\"EPH\" stage=\"Final\" version=\"B\">" +
                "<p class=\"Section Head\" usfm=\"s1\">Greeting</p>" +
                "<p class=\"Paragraph\" usfm=\"p\"><c n=\"1\" /><v n=\"1\" />Paul, an apostle" +
                    " <note reference=\"1:1\" class=\"Note General Paragraph\" usfm=\"f\">Had special authority, and had seen Jesus.</note>" +
                    " of Christ Jesus by the will of God,</p>" +
                "<p class=\"Paragraph\" usfm=\"p\">Praise be!</p>" +
                "</book>";

            // Create the test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sXml);

            // Collect the texts
            var vTexts = Scanner.GetTexts(book);

            // Test
            Assert.AreEqual(5, vTexts.Count);
            // The footnote should be the last one in the section (screen order)
            Assert.AreEqual("Had special authority, and had seen Jesus.", vTexts[4].AsString);
        }
        #endregion

        #region Test: TIsAtWordBeginning
        [Test] public void TIsAtWordBeginning()
        {
            TestCommon.GlobalTestSetup();
            TestCommon.CreateHierarchyThroughTargetTranslation();
            var context = new Scanner.SearchContext("blah", null, null, 0);

            Assert.IsTrue(Scanner.IsAtWordBeginning(context, "What do you think?", 0));  // What
            Assert.IsTrue(Scanner.IsAtWordBeginning(context, "What do you think?", 5));  // do
            Assert.IsTrue(Scanner.IsAtWordBeginning(context, "What do you think?", 12));  // think

            Assert.IsFalse(Scanner.IsAtWordBeginning(context, "What do you think?", 1)); // W[h]at
            Assert.IsFalse(Scanner.IsAtWordBeginning(context, "What do you think?", 13)); // t[h]ink
            Assert.IsFalse(Scanner.IsAtWordBeginning(context, "What do you think?", 17)); // think[?]
        }
        #endregion
        #region Test: TIsAtWordEnding
        [Test] public void TIsAtWordEnding()
        {
            TestCommon.GlobalTestSetup();
            TestCommon.CreateHierarchyThroughTargetTranslation();

            var context = new Scanner.SearchContext("What", null, null, 0);
            Assert.IsTrue(Scanner.IsAtWordEnding(context, "What do you think?", 0));

            context = new Scanner.SearchContext("do", null, null, 0);
            Assert.IsTrue(Scanner.IsAtWordEnding(context, "What do you think?", 5));

            context = new Scanner.SearchContext("o", null, null, 0);
            Assert.IsTrue(Scanner.IsAtWordEnding(context, "What do you think?", 6));

            context = new Scanner.SearchContext("k", null, null, 0);
            Assert.IsTrue(Scanner.IsAtWordEnding(context, "What do you think?", 16));

            context = new Scanner.SearchContext("a", null, null, 0);
            Assert.IsFalse(Scanner.IsAtWordEnding(context, "What do you think?", 2));

            context = new Scanner.SearchContext("n", null, null, 0);
            Assert.IsFalse(Scanner.IsAtWordEnding(context, "What do you think?", 15));
        }
        #endregion
        #region Test: TMatchesSearchType_Whole
        [Test] public void TMatchesSearchType_Whole()
        {
            TestCommon.GlobalTestSetup();
            TestCommon.CreateHierarchyThroughTargetTranslation();

            var context = new Scanner.SearchContext("What", null, null, 0) {
                Type = Scanner.SearchType.Whole
            };
            Assert.IsTrue(Scanner.MatchesSearchType(context, "What do you think?", 0));

            context = new Scanner.SearchContext("think", null, null, 0) {
                Type = Scanner.SearchType.Whole
            };
            Assert.IsTrue(Scanner.MatchesSearchType(context, "What do you think?", 12));

            context = new Scanner.SearchContext("do", null, null, 0) {
                Type = Scanner.SearchType.Whole
            };
            Assert.IsTrue(Scanner.MatchesSearchType(context, "What do you think?", 5));

            context = new Scanner.SearchContext("d", null, null, 0) {
                Type = Scanner.SearchType.Whole
            };
            Assert.IsFalse(Scanner.MatchesSearchType(context, "What do you think?", 5));

            context = new Scanner.SearchContext("Wh", null, null, 0) {
                Type = Scanner.SearchType.Whole
            };
            Assert.IsFalse(Scanner.MatchesSearchType(context, "What do you think?", 0));

            context = new Scanner.SearchContext("hink", null, null, 0) {
                Type = Scanner.SearchType.Whole
            };
            Assert.IsFalse(Scanner.MatchesSearchType(context, "What do you think?", 12));
        }
        #endregion
    }
}
