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

        #region Test: TGetIndexesOf
        [Test] public void TGetIndexesOf()
        {
            const string csSource = "For God so loved the world he gave his one and only son, " + 
                "that whosoever believes in him should not perish but have eternal life.";

            var vShouldNotFind = Scanner.GetIndexesOf(csSource, "Not in there", false);
            Assert.AreEqual(0, vShouldNotFind.Count());

            var vShouldFindFirst = Scanner.GetIndexesOf(csSource, "For", false);
            Assert.AreEqual(1, vShouldFindFirst.Count());
            Assert.AreEqual(0, vShouldFindFirst[0]);

            var vShouldFindLast = Scanner.GetIndexesOf(csSource, "life.", false);
            Assert.AreEqual(1, vShouldFindLast.Count());
            Assert.AreEqual(123, vShouldFindLast[0]);

            var vShouldFindMiddle = Scanner.GetIndexesOf(csSource, "God", false);
            Assert.AreEqual(1, vShouldFindMiddle.Count());
            Assert.AreEqual(4, vShouldFindMiddle[0]);

            var vShouldFindMultiple = Scanner.GetIndexesOf(csSource, "so", false);
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

            // Multiple times
            var context = new Scanner.SearchContext("Jesus", null);
            var vHits = Scanner.ScanBook(context, book);
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
            var v = Scanner.GetTextsToScan(book, Scanner.ScanOption.All, null);

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
            var v = Scanner.GetTextsToScan(book, Scanner.ScanOption.PriorTo, target);

            // Test
            Assert.AreEqual(3, v.Count);
            Assert.AreEqual("Greeting", v[0].ContentsAsString);
            Assert.IsTrue(v[2].ContentsAsString.StartsWith("To the saints"));
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
            var v = Scanner.GetTextsToScan(book, Scanner.ScanOption.After, target);

            // Test
            Assert.AreEqual(8, v.Count);
            Assert.IsTrue(v[0].ContentsAsString.StartsWith("To the saints"));
        }
        #endregion

        #region Test: TScanBook_firstWordInBook
        [Test] public void TScanBook_firstWordInBook()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Scan
            var context = new Scanner.SearchContext("Greeting", null);
            var lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.All, null);

            // Test
            Assert.IsNotNull(lookupInfo);
            Assert.AreEqual(0, lookupInfo.SectionNo);
            Assert.AreEqual(0, lookupInfo.ParagraphNo);
            Assert.AreEqual(0, lookupInfo.RunNo);
            Assert.AreEqual(0, lookupInfo.IndexIntoText);
        }
        #endregion
        #region Test: TScanBook_firstOccurrence
        [Test] public void TScanBook_firstOccurrence()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Scan
            var context = new Scanner.SearchContext("faithful", null);
            var lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.All, null);

            // Test
            Assert.IsNotNull(lookupInfo);
            Assert.AreEqual(0, lookupInfo.SectionNo);
            Assert.AreEqual(2, lookupInfo.ParagraphNo);
            Assert.AreEqual(0, lookupInfo.RunNo);
            Assert.AreEqual(30, lookupInfo.IndexIntoText);
        }
        #endregion
        #region Test: TScanBook_finalOccurrence
        [Test] public void TScanBook_finalOccurrence()
        {
            // Create test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sOxesXml);

            // Scan
            var context = new Scanner.SearchContext("loves.", null);
            var lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.All, null);

            // Test
            Assert.IsNotNull(lookupInfo);
            Assert.AreEqual(1, lookupInfo.SectionNo);
            Assert.AreEqual(4, lookupInfo.ParagraphNo);
            Assert.AreEqual(0, lookupInfo.RunNo);
            Assert.AreEqual(80, lookupInfo.IndexIntoText);
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

            // Should not find 'adopted' before; but yes to after
            var context = new Scanner.SearchContext("adopted", selection);
            var lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.PriorTo, selection);
            Assert.IsNull(lookupInfo);
            lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.After, selection);
            Assert.IsNotNull(lookupInfo);

            // Prior should not find the word we're sitting on; but After should
            context = new Scanner.SearchContext("predestined", selection);
            lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.PriorTo, selection);
            Assert.IsNull(lookupInfo);
            lookupInfo = Scanner.ScanBook(context, book, Scanner.ScanOption.After, selection);
            Assert.IsNotNull(lookupInfo);
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
                "</book>";

            // Create the test book
            TestCommon.GlobalTestSetup();
            var book = TestCommon.CreateFromOxes("EPH", sXml);

            // Scan the target paragraph
            var paragraph = book.Sections[0].Paragraphs[1];
            var vTexts = Scanner.GetTextsAndFootnotes(paragraph);

            // Test
            Assert.AreEqual(3, vTexts.Count);
            Assert.AreEqual("Had special authority, and had seen Jesus.", vTexts[1].AsString);
        }
        #endregion
    }
}
