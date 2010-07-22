using JWTools;
using NUnit.Framework;
using OurWordData.DataModel.Membership;

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class T_User
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion
        #region Method: User CreateFromXml(string sXml)
        static User CreateFromXml(string sXml)
        {
            var docNew = new XmlDoc(sXml);
            var nodeNew = XmlDoc.FindNode(docNew, "User");
            var userNew = User.Create(nodeNew);
            Assert.IsNotNull(userNew);
            return userNew;
        }
        #endregion

        #region Test: TeamMembership
        [Test]
        public void TeamMembership()
        {
            // Create a user; should not be a member of any of these
            var user = new User();
            Assert.IsFalse(user.IsMemberOf("Amarasi"));
            Assert.IsFalse(user.IsMemberOf("Helong"));
            Assert.IsFalse(user.IsMemberOf("Rikou"));                      

            // Add to two of the teams
            user.AddMembershipTo("Amarasi");
            user.AddMembershipTo("Rikou");
            Assert.IsTrue(user.IsMemberOf("Amarasi"));
            Assert.IsFalse(user.IsMemberOf("Helong"));
            Assert.IsTrue(user.IsMemberOf("Rikou"));                      

            // Remove from one of them
            user.RemoveMembershipFrom("Rikou");
            Assert.IsTrue(user.IsMemberOf("Amarasi"));
            Assert.IsFalse(user.IsMemberOf("Helong"));
            Assert.IsFalse(user.IsMemberOf("Rikou"));
        }
        #endregion
        #region Test: CanEditBooks
        [Test]
        public void CanEditBooks()
        {
            const string c_sTranslationName = "Amarasi";

            // Create a new user
            var user = new User();

            // Since he is not a member of the Amarasi team, he can't edit the books
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sTranslationName, "MRK"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sTranslationName, "JHN"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sTranslationName, "ACT"));

            // Make him a member. Now, by default he can edit the books, because users are 
            // able to edit projects for which they are a member.
            user.AddMembershipTo(c_sTranslationName);
            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sTranslationName, "MRK"));
            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sTranslationName, "JHN"));
            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sTranslationName, "ACT"));

            // Set two of them to being ReadOnly
            user.SetEditability(c_sTranslationName, "JHN", 
                User.TranslationSettings.Editability.ReadOnly);
            user.SetEditability(c_sTranslationName, "ACT",
                User.TranslationSettings.Editability.ReadOnly);
            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sTranslationName, "MRK"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sTranslationName, "JHN"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sTranslationName, "ACT"));

            // Set one of them to being Notes
            user.SetEditability(c_sTranslationName, "JHN",
                User.TranslationSettings.Editability.Notes);
            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sTranslationName, "MRK"));
            Assert.AreEqual(User.TranslationSettings.Editability.Notes,
                user.GetEditability(c_sTranslationName, "JHN"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sTranslationName, "ACT"));
        }
        #endregion

        #region Test: void IO()
        [Test]
        public void IO()
        {
            const string c_sXml =
                "<User username=\"Bob\" password=\"TheTomato\" administrator=\"true\" " +
                "maxWin=\"true\" " +
                "zoom=\"130\" " +
                "primaryUi=\"Swahili\" " +
                "secondaryUi=\"Amarasi\" " +
                "DraftingBg=\"Navy\" " +
                "BackTranslationBg=\"Red\" " +
                "NaturalnessBg=\"Blue\" " +
                "ConsultantBg=\"Pink\" " +
                "collaborationUserName=\"Larry\" " +
                "collaborationPassword=\"Cucumber\" " +
                "canEditStructure=\"true\" " +
                "canUndoRedo=\"true\" canNavFirstLast=\"true\" " +
                "canDoBackTranslation=\"true\" canDoNaturalnessCheck=\"true\" canDoConsultantPreparation=\"true\" " +
                "canZoom=\"true\" canCreateProject=\"true\" canOpenProject=\"true\" " +
                "canExportProject=\"true\" canPrint=\"true\" canFilter=\"true\" canLocalize=\"true\" " +
                "canRestoreBackups=\"true\" " +
                "canMakeNotes=\"true\" " +
                "noteAuthor=\"Robert\" " +
                "closeNoteWindowWhenMouseLeaves=\"true\" " +
                "showExpandedNotesIcon=\"true\" " +
                "canDeleteNotesAuthoredByOthers=\"true\" " +
                "canAuthorHintForDaugherNotes=\"true\" " +
                "canAuthorInformationNotes=\"true\" " +
                "canAssignNoteToConsultant=\"true\" " +
                "canCreateFrontTranslationNotes=\"true\">" +
                    "<TranslationSettings name=\"Amarasi\" Notes=\"1PE 2PE\" ReadOnly=\"JHN ROM\" />" +
                "</User>";

            // Create a user with some meaningful settings
            var user = new User 
            {
                UserName = "Bob", 
                Password = "TheTomato",
                IsAdministrator = true, 
                MaximizeWindowOnStartup = true,
                ZoomPercent = 130,
                PrimaryUiLanguage = "Swahili",
                SecondaryUiLanguage = "Amarasi",
                DraftingWindowBackground = "Navy",
                BackTranslationWindowBackground = "Red",
                NaturalnessWindowBackground = "Blue",
                ConsultantWindowBackground = "Pink",
                CollaborationUserName = "Larry",
                CollaborationPassword = "Cucumber",
                CanEditStructure = true, 
                CanUndoRedo = true,
                CanNavigateFirstLast = true,
                CanDoBackTranslation = true,
                CanDoNaturalnessCheck = true,
                CanDoConsultantPreparation = true,
                CanZoom = true,
                CanCreateProject = true,
                CanOpenProject = true,
                CanExportProject = true,
                CanPrint = true,
                CanFilter = true,
                CanLocalize = true,
                CanRestoreBackups = true,
                CanMakeNotes = true,
                NoteAuthorsName = "Robert",
                CloseNotesWindowWhenMouseLeaves = true,
                ShowExpandedNotesIcon = true,
                CanDeleteNotesAuthoredByOthers = true,
                CanAuthorHintForDaughterNotes = true,
                CanAuthorInformationNotes = true,
                CanAssignNoteToConsultant = true,
                CanCreateFrontTranslationNotes = true
            };
            const string c_sAmarasi = "Amarasi";
            user.AddMembershipTo(c_sAmarasi);
            user.SetEditability(c_sAmarasi, "JHN",
                User.TranslationSettings.Editability.ReadOnly);
            user.SetEditability(c_sAmarasi, "ROM",
                User.TranslationSettings.Editability.ReadOnly);
            user.SetEditability(c_sAmarasi, "1PE",
                User.TranslationSettings.Editability.Notes);
            user.SetEditability(c_sAmarasi, "2PE",
                User.TranslationSettings.Editability.Notes);

            // Save and see if we get what we expect
            var doc = new XmlDoc();
            var node = user.Save(doc, null);
            Assert.AreEqual(c_sXml, node.OuterXml);

            // Create a new user from what we've just written
            // The two should be equal content
            var userNew = CreateFromXml(node.OuterXml);
            Assert.IsTrue(user.ContentEquals(userNew));
            Assert.AreEqual(userNew.UserName, "Bob");
            Assert.IsTrue(userNew.IsMemberOf("Amarasi"));

            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sAmarasi, "MRK"));
            Assert.AreEqual(User.TranslationSettings.Editability.Full,
                user.GetEditability(c_sAmarasi, "EXO"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sAmarasi, "JHN"));
            Assert.AreEqual(User.TranslationSettings.Editability.ReadOnly,
                user.GetEditability(c_sAmarasi, "ROM"));
            Assert.AreEqual(User.TranslationSettings.Editability.Notes,
                user.GetEditability(c_sAmarasi, "1PE"));
            Assert.AreEqual(User.TranslationSettings.Editability.Notes,
                user.GetEditability(c_sAmarasi, "2PE"));
        }
        #endregion
        #region Test: Merge
        [Test] public void Merge()
        {
            const string c_sParent =
                "<User username=\"Bob\" zoom=\"100\" primaryUi=\"English\" secondaryUi=\"Spanish\" " +
                "DraftingBg=\"Wheat\" BackTranslationBg=\"Linen\" NaturalnessBg=\"Wheat\" " +
                "ConsultantBg=\"Wheat\" noteAuthor=\"Bob\">" +
                    "<TranslationSettings name=\"Amarasi\" ReadOnly=\"JHN ROM\" />" +
                "</User>";
            const string c_sOurs =
               "<User username=\"Bob\" zoom=\"120\" primaryUi=\"English\" secondaryUi=\"Spanish\" " +
               "DraftingBg=\"Wheat\" BackTranslationBg=\"Linen\" NaturalnessBg=\"Wheat\" " +
               "ConsultantBg=\"Wheat\" noteAuthor=\"Bob\">" +
                   "<TranslationSettings name=\"Amarasi\" ReadOnly=\"JHN ROM\" />" +
                   "<TranslationSettings name=\"Helong\" ReadOnly=\"MAT\" />" +
               "</User>";
            const string c_sTheirs =
               "<User username=\"Bob\" zoom=\"100\" primaryUi=\"English\" secondaryUi=\"English\" " +
               "DraftingBg=\"Wheat\" BackTranslationBg=\"Linen\" NaturalnessBg=\"Wheat\" " +
               "ConsultantBg=\"Wheat\" noteAuthor=\"Bob\">" +
                   "<TranslationSettings name=\"Amarasi\" ReadOnly=\"ROM 1CO\" />" +
                   "<TranslationSettings name=\"Rikou\" ReadOnly=\"MRK\" />" +
               "</User>";

            const string c_sExpected =
                "<User username=\"Bob\" zoom=\"120\" primaryUi=\"English\" secondaryUi=\"English\" " +
                "DraftingBg=\"Wheat\" BackTranslationBg=\"Linen\" NaturalnessBg=\"Wheat\" " +
                "ConsultantBg=\"Wheat\" noteAuthor=\"Bob\">" +
                    "<TranslationSettings name=\"Amarasi\" ReadOnly=\"ROM 1CO\" />" +
                    "<TranslationSettings name=\"Helong\" ReadOnly=\"MAT\" />" +
                    "<TranslationSettings name=\"Rikou\" ReadOnly=\"MRK\" />" +
                "</User>";

            var userParent = CreateFromXml(c_sParent);
            var userOurs = CreateFromXml(c_sOurs);
            var userTheirs = CreateFromXml(c_sTheirs);

            userOurs.Merge(userParent, userTheirs);

            var sActual = userOurs.Save(new XmlDoc(), null).OuterXml;
            Assert.AreEqual(c_sExpected, sActual);
        }
        #endregion

    }
}
