using System.Drawing;
using NUnit.Framework;
using OurWord;
using OurWordData.DataModel;
using OurWordData.DataModel.Runs;

namespace OurWordTests.DataModel
{
    #region CLASS: T_DText
    [TestFixture]
    public class T_DText
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();

            OurWordMain.App = new OurWordMain();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
        }
        #endregion

        #region Test: ToFromString
        [Test] public void ToFromString()
        {
            // Create a DText that has several phrases
            var text = new DText();
            text.Phrases.Append(new DPhrase("These are the "));
            text.Phrases.Append(new DPhrase("times ") {FontModification = FontStyle.Italic});
            text.Phrases.Append(new DPhrase("that "));
            text.Phrases.Append(new DPhrase("try ") {FontModification = FontStyle.Underline});
            text.Phrases.Append(new DPhrase("men's souls.") {FontModification = FontStyle.Bold});

            // Is the output what we expect?
            var sSave = text.Phrases.ToSaveString;
            Assert.AreEqual("These are the |itimes |rthat |utry |r|bmen's souls.|r",
                sSave);

            // Create a recipient DText
            var text2 = new DText();
            text2.Phrases.FromSaveString(sSave);

            // Are the two texts equal?
            Assert.IsTrue(text.ContentEquals(text2), "DTexts are not the same");
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: DText _CreateText(s)
        DText _CreateText(string s)
        {
            DText t = new DText();
            t.Phrases.FromSaveString(s);
            return t;
        }
        #endregion
        #region Test: Merge_OursChanged
        [Test]
        public void Merge_OursChanged()
        {
            // Create the three versions
            string sParent = "These are the |itimes |rthat try men's souls.";
            DText Parent = _CreateText(sParent);

            DText Theirs = _CreateText(sParent);

            string sOurs = "These are not the |itimes |rthat try men's souls.";
            DText Ours = _CreateText(sOurs);

            // Do the Merge
            Ours.Merge(Parent, Theirs);

            // Ours should not have changed
            Assert.AreEqual(sOurs, Ours.Phrases.ToSaveString);
        }
        #endregion
        #region Test: Merge_TheirsChanged
        [Test]
        public void Merge_TheirsChanged()
        {
            // Create the three versions
            string sParent = "These are the |itimes |rthat try men's souls.";
            DText Parent = _CreateText(sParent);

            string sTheirs = "These are definitely the |itimes |rthat try men's souls.";
            DText Theirs = _CreateText(sTheirs);

            DText Ours = _CreateText(sParent);

            // Do the Merge
            Ours.Merge(Parent, Theirs);

            // Ours should have changed to Theirs
            Assert.AreEqual(sTheirs, Ours.Phrases.ToSaveString);
        }
        #endregion
        #region Test: Merge_BothChanged
        [Test]
        public void Merge_BothChanged()
        {
            // Create the three versions
            string sParent = "These are the |itimes |rthat try men's souls.";
            DText Parent = _CreateText(sParent);

            string sTheirs = "These are definitely the |itimes |rthat try men's souls.";
            DText Theirs = _CreateText(sTheirs);

            string sOurs = "These are not the |itimes |rthat try men's souls.";
            DText Ours = _CreateText(sOurs);

            // Because we're creating a Note, we need for Ours to be owned so that a
            // Reference can be calculated.
            DSection section = new DSection();
            section.ReferenceSpan = new DReferenceSpan();
            section.ReferenceSpan.Start = new DReference(3, 8);
            section.ReferenceSpan.End = new DReference(3, 15);
            DParagraph p = new DParagraph();
            section.Paragraphs.Append(p);
            p.Runs.Append(Ours);

            // Do the Merge
            Ours.Merge(Parent, Theirs);

            // Ours should not be changed
            Assert.AreEqual(sOurs, Ours.Phrases.ToSaveString);

            // But we should have a Translator Note giving their version
            Assert.AreEqual(1, Ours.TranslatorNotes.Count);
            TranslatorNote note = Ours.TranslatorNotes[0];
            Assert.AreEqual("not", note.SelectedText);
        }
        #endregion
        #region Test: GetNoteContext
        [Test]
        public void GetNoteContext()
        {
            // Middle
            string sOurs = "For God so loved the world";
            string sTheirs = "For God really cared for the world";
            Assert.AreEqual("so loved", DText.GetNoteContext(sOurs, sTheirs), "Middle");

            // Beginning
            sOurs = "For God so loved the world";
            sTheirs = "Truly God so loved the world";
            Assert.AreEqual("For", DText.GetNoteContext(sOurs, sTheirs), "Beginning");

            // End
            sOurs = "For God so loved the world";
            sTheirs = "For God so loved the planet";
            Assert.AreEqual("world", DText.GetNoteContext(sOurs, sTheirs), "End");

            // Append at end
            sOurs = "For God so loved the world";
            sTheirs = "For God so loved the world he gave his son.";
            Assert.AreEqual("...the world", DText.GetNoteContext(sOurs, sTheirs), "Append at end");

            // Append at beginning
            sOurs = "For God so loved the world";
            sTheirs = "Since For God so loved the world";
            Assert.AreEqual("For God so...", DText.GetNoteContext(sOurs, sTheirs), "Append at beginning");

            // Append at end; yet looks the same
            sOurs = "For God so loved the world";
            sTheirs = "For God so loved the world he gave the world";
            Assert.AreEqual("...the world", DText.GetNoteContext(sOurs, sTheirs), "Pathelogic");

            // Empty "theirs"
            sOurs = "For God so loved the world";
            sTheirs = "";
            Assert.AreEqual("For God so...", DText.GetNoteContext(sOurs, sTheirs), "Empty");

        }
        #endregion

        #region Test: CreateNoteContents
        [Test]
        public void CreateNoteContents()
        {
            const string sParent = "For God so loved the world that he gave his only begotten son";
            const string sOurs = "For God so loved the world that he gave his one and only son";
            const string sTheirs = "For God loved the world so much that he gave his one and only son";

            var sActual = DText.GetConflictMergeNoteContents(sParent, sOurs, sTheirs);

            const string sParentChanged = "so loved the world that he gave his only begotten";
            const string sOursChanged = "so loved the world that he gave his one and only";
            const string sTheirsChanged = "loved the world so much that he gave his one and only";
            var sExpected = string.Format(
                "Merge Conflict: Original was \"{0}\"; Ours was \"{1}\"; Theirs was \"{2}\"",
                sParentChanged, sOursChanged, sTheirsChanged);

            Assert.AreEqual(sExpected, sActual);
        }
        #endregion
        #region Test: CreateNoteContents_TheirsIsEmpty
        [Test]
        public void CreateNoteContents_TheirsIsEmpty()
        {
            const string sParent = "For God so loved the world that he gave his only begotten son";
            const string sOurs = "For God so loved the world that he gave his one and only son";
            const string sTheirs = "";

            var sActual = DText.GetConflictMergeNoteContents(sParent, sOurs, sTheirs);

            var sExpected = string.Format(
                "Merge Conflict: Original was \"{0}\"; Ours was \"{1}\"; Theirs was \"\"",
                sParent, sOurs);

            Assert.AreEqual(sExpected, sActual);
        }
        #endregion
        #region Test: CreateNoteContents_OursIsEmpty
        [Test]
        public void CreateNoteContents_OursIsEmpty()
        {
            const string sParent = "For God so loved the world that he gave his only begotten son";
            const string sOurs = "";
            const string sTheirs = "For God loved the world so much that he gave his one and only son";

            var sActual = DText.GetConflictMergeNoteContents(sParent, sOurs, sTheirs);

            var sExpected = string.Format(
                "Merge Conflict: Original was \"{0}\"; Ours was \"\"; Theirs was \"{1}\"",
                sParent, sTheirs);

            Assert.AreEqual(sExpected, sActual);
        }
        #endregion
        #region Test: CreateNoteContents_ParentWasEmpty
        [Test]
        public void CreateNoteContents_ParentWasEmpty()
        {
            const string sParent = "";
            const string sOurs = "For God so loved the world that he gave his one and only son";
            const string sTheirs = "For God loved the world so much that he gave his one and only son";

            var sActual = DText.GetConflictMergeNoteContents(sParent, sOurs, sTheirs);

            var sExpected = string.Format(
                "Merge Conflict: Original was \"\"; Ours was \"{0}\"; Theirs was \"{1}\"",
                sOurs, sTheirs);

            Assert.AreEqual(sExpected, sActual);
        }
        #endregion
        #region Test: CreateNoteContents_ProvideMoreContext
        [Test]
        public void CreateNoteContents_ProvideMoreContext()
        {
            const string sParent = "My God is an awesome God.";
            const string sOurs = "My God is an amazing God.";
            const string sTheirs = "My God is an everlasting God.";

            var sActual = DText.GetConflictMergeNoteContents(sParent, sOurs, sTheirs);

            const string sExpected =
                "Merge Conflict: Original was \"awesome God\"; " +
                "Ours was \"amazing God\"; " +
                "Theirs was \"everlasting God\"";

            Assert.AreEqual(sExpected, sActual);
        }
        #endregion
        #region Test: Merge_Huichol_Exo0101
        [Test] public void Merge_Huichol_Exo0101()
        {
            // Changed:
            // 1. Both gave a back translation (which are identical)
            // 2. Theirs changed the vernacular
            var parent = DText.CreateSimple("Memeyecü", "");
            var ours = DText.CreateSimple("Memeyecü", "Salieron");
            var theirs = DText.CreateSimple("Memeyecü 'Equipitusie", "Salieron");

            ours.Merge(parent, theirs);

            // Expected:
            // 1. Keep the new BT
            // 2. Keep vernacular from Theirs
            var expected = DText.CreateSimple("Memeyecü 'Equipitusie", "Salieron");
            Assert.AreEqual(expected.DebugString, ours.DebugString);
        }
        #endregion
    }
    #endregion


}
