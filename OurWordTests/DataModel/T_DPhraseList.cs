using System.Drawing;
using NUnit.Framework;
using OurWordData.DataModel.Runs;

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class T_DPhraseList
    {
        #region smethod: DText CreateComplexText()
        static DText CreateComplexText()
        {
            var text = new DText();
            text.Phrases.Append(new DPhrase("Forescore and "));
            text.Phrases.Append(new DPhrase("ten ") {FontToggles = FontStyle.Bold});
            text.Phrases.Append(new DPhrase("years and seven months and two days ago"));
            Assert.AreEqual(3, text.Phrases.Count);
            return text;
        }
        #endregion

        #region Test: TReplace_simple
        [Test] public void TReplace_simple()
        {           
            // Begining
            var text = DText.CreateSimple("Forescore and ten years ago");
            text.Phrases.Replace(0, 4, "Three");
            Assert.AreEqual("Threescore and ten years ago", text.ContentsAsString);

            // Middle
            text = DText.CreateSimple("Forescore and ten years ago");
            text.Phrases.Replace(10, 3, "or");
            Assert.AreEqual("Forescore or ten years ago", text.ContentsAsString);

            // End
            text = DText.CreateSimple("Forescore and ten years ago");
            text.Phrases.Replace(24, 3, "from now");
            Assert.AreEqual("Forescore and ten years from now", text.ContentsAsString);
        }
        #endregion
        #region Test: TReplace_complex
        [Test] public void TReplace_complex()
        {
            // Text = 'Forescore and |ten |years and seven months and two days ago'

            // Text Beginning
            var text = CreateComplexText();
            var iPosAfter = text.Phrases.Replace(0, 4, "Three");
            Assert.AreEqual("Threescore and ", text.Phrases[0].Text);
            Assert.AreEqual(5, iPosAfter);

            // Text End
            text = CreateComplexText();
            iPosAfter = text.Phrases.Replace(54, 3, "from now");
            Assert.AreEqual("years and seven months and two days from now", text.Phrases[2].Text);
            Assert.AreEqual(62, iPosAfter);

            // Replace middle phrase
            text = CreateComplexText();
            iPosAfter = text.Phrases.Replace(14, 4, "eleven ");
            Assert.AreEqual(3, text.Phrases.Count);
            Assert.AreEqual("Forescore and ", text.Phrases[0].Text);
            Assert.AreEqual("eleven ", text.Phrases[1].Text);
            Assert.AreEqual("years and seven months and two days ago", text.Phrases[2].Text);
            Assert.AreEqual(21, iPosAfter);

            // Replace across phrase boundary
            text = CreateComplexText();
            iPosAfter = text.Phrases.Replace(10, 5, "or y");
            Assert.AreEqual(3, text.Phrases.Count);
            Assert.AreEqual("Forescore or y", text.Phrases[0].Text);
            Assert.AreEqual("en ", text.Phrases[1].Text);
            Assert.AreEqual("years and seven months and two days ago", text.Phrases[2].Text);
            Assert.AreEqual(14, iPosAfter);

            // Replace across entire phrase
            text = CreateComplexText();
            iPosAfter = text.Phrases.Replace(10, 18, "AND ");
            Assert.AreEqual(1, text.Phrases.Count);
            Assert.AreEqual("Forescore AND seven months and two days ago", text.Phrases[0].Text);
            Assert.AreEqual(14, iPosAfter);

            // Space Before: Insert space in beginning of phrase
            text = CreateComplexText();
            iPosAfter = text.Phrases.Replace(14, 1, " t");
            Assert.AreEqual(3, text.Phrases.Count);
            Assert.AreEqual("Forescore and ", text.Phrases[0].Text);
            Assert.AreEqual("ten ", text.Phrases[1].Text);
            Assert.AreEqual("years and seven months and two days ago", text.Phrases[2].Text);
            Assert.AreEqual(15, iPosAfter);

            // Space After: Insert space at end of phrase
            text = CreateComplexText();
            iPosAfter = text.Phrases.Replace(14, 4, "eleven ");
            Assert.AreEqual(3, text.Phrases.Count);
            Assert.AreEqual("Forescore and ", text.Phrases[0].Text);
            Assert.AreEqual("eleven ", text.Phrases[1].Text);
            Assert.AreEqual("years and seven months and two days ago", text.Phrases[2].Text);
            Assert.AreEqual(21, iPosAfter);
        }
        #endregion

        #region Test: TReplaceAll_simple
        [Test] public void TReplaceAll_simple()
        {
            // Word-medial
            var text = DText.CreateSimple("We saw seventeen or eighteen or nineteen of them");
            text.Phrases.ReplaceAll("teen", "tEEn");
            Assert.AreEqual("We saw seventEEn or eightEEn or ninetEEn of them", text.ContentsAsString);

            // Whole words
            text = DText.CreateSimple("We saw seventeen or eighteen or nineteen of them");
            text.Phrases.ReplaceAll("or", "AND");
            Assert.AreEqual("We saw seventeen AND eighteen AND nineteen of them", text.ContentsAsString);
        }
        #endregion
        #region Test: TReplaceAll_complex
        [Test] public void TReplaceAll_complex()
        {
            var text = CreateComplexText();
            text.Phrases.ReplaceAll("and", "AND");
            Assert.AreEqual(3, text.Phrases.Count);
            Assert.AreEqual("Forescore AND ", text.Phrases[0].Text);
            Assert.AreEqual("ten ", text.Phrases[1].Text);
            Assert.AreEqual("years AND seven months AND two days ago", text.Phrases[2].Text);
        }
        #endregion

    }
}
