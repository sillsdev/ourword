using NUnit.Framework;
using OurWord.Dialogs.Export;
using OurWordData.DataModel;

namespace OurWordTests.Dialogs
{
    [TestFixture]
    public class T_ExportProject
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

        #region Test: TTotalChapterCount
        [Test]
        public void TTotalChapterCount()
        {
            var translation = TestCommon.CreateHierarchyThroughTargetTranslation();
            translation.AddBook(new DBook("EXO"));
            translation.AddBook(new DBook("1SA"));
            translation.AddBook(new DBook("MAT"));
            translation.AddBook(new DBook("MRK"));
            translation.AddBook(new DBook("1JN"));

            var export = new ExportTranslation(translation);

            const int cExpectedChapterCount = 40 + 31 + 28 + 16 + 5;
            
            Assert.AreEqual(cExpectedChapterCount, export.TotalChapterCount);
        }
        #endregion
    }
}
