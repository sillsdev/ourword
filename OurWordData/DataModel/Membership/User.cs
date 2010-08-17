using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using Chorus.merge;
using JWTools;
using OurWordData.Synchronize;

namespace OurWordData.DataModel.Membership
{
    /* For each new attr, must add to:
     * - Read/Save
     * - Merge
     * - IO Test as "true" to make sure it is tested everywhere
    */

    public class User
    {
        // Attrs -----------------------------------------------------------------------------
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsAdministrator { get; set; }

        // Attrs: General Options
        public bool MaximizeWindowOnStartup { get; set; }
        public int ZoomPercent { get; set; }
        #region VAttr{g}: float ZoomFactor
        public float ZoomFactor
        {
            get
            {
                return (ZoomPercent /100.0F);
            }
        }
        #endregion

        public static readonly int[] PossibleZoomPercents = 
            { 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 175, 200, 225, 250 };
        public string PrimaryUiLanguage { get; set; }
        public string SecondaryUiLanguage { get; set; }
        public string DraftingWindowBackground { get; set; }
        public string BackTranslationWindowBackground { get; set; }
        public string NaturalnessWindowBackground { get; set; }
        public string ConsultantWindowBackground { get; set; }
        public string CollaborationUserName { get; set; }
        public string CollaborationPassword { get; set; }

        // Atrrs: Features On/Off
        public bool CanEditStructure { get; set; }
        public bool CanUndoRedo { get; set; }
        public bool CanNavigateFirstLast { get; set; }
        public bool CanDoBackTranslation { get; set; }
        public bool CanDoNaturalnessCheck { get; set; }
        public bool CanDoConsultantPreparation { get; set; }
        public bool CanZoom { get; set; }
        public bool CanCreateProject { get; set; }
        public bool CanOpenProject { get; set; }
        public bool CanExportProject { get; set; }
        public bool CanPrint { get; set; }
        public bool CanFilter { get; set; }
        public bool CanLocalize { get; set; }
        public bool CanRestoreBackups { get; set; }

        // Attrs: Notes
        public bool CanMakeNotes { get; set; }
        #region Attr{g/s}: string NoteAuthorsName
        public string NoteAuthorsName
        {
            get
            {
                if (string.IsNullOrEmpty(m_sNoteAuthorsName))
                    m_sNoteAuthorsName = UserName;
                return m_sNoteAuthorsName;
            }
            set
            {
                m_sNoteAuthorsName = value;
            }
        }
        private string m_sNoteAuthorsName;
        #endregion
        public bool CloseNotesWindowWhenMouseLeaves { get; set; }
        public bool ShowExpandedNotesIcon { get; set; }
        public bool CanDeleteNotesAuthoredByOthers { get; set; }
        public bool CanAuthorHintForDaughterNotes { get; set; }
        public bool CanAuthorInformationNotes { get; set; }
        public bool CanAssignNoteToConsultant { get; set; }
        public bool CanCreateFrontTranslationNotes { get; set; }

        // Settings specific to an individual translation ------------------------------------
        #region CLASS: TranslationSettings
        public class TranslationSettings
        {
            static readonly Color ReadOnlyColor = Color.Red;
            static readonly Color NotesOnlyColor = Color.Blue;
            static readonly Color FullEditColor = Color.Black;
            #region SMethod: Color GetUiColor(Editability editability)
            static public Color GetUiColor(Editability editability)
            {
                if (editability == Editability.ReadOnly)
                    return ReadOnlyColor;
                if (editability == Editability.Notes)
                    return NotesOnlyColor;
                return FullEditColor;
            }
            #endregion

            // Books that can be edited by this user -----------------------------------------
            public enum Editability { Full, Notes, ReadOnly };
            #region Method: Editability GetEditability(string sBookAbbrev)
            public Editability GetEditability(string sBookAbbrev)
            {
                if (GlobalEditability == GEditability.Full)
                    return Editability.Full;
                if (GlobalEditability == GEditability.Notes)
                    return Editability.Notes;
                if (GlobalEditability == GEditability.ReadOnly)
                    return Editability.ReadOnly;

                Editability value;
                if (m_BookEditability.TryGetValue(sBookAbbrev, out value))
                    return value;
                throw new Exception("BookAbbrev not in dictionary.");
            }
            #endregion
            #region Method: void SetEditability(string sBookAbbrev, Editability value)
            public void SetEditability(string sBookAbbrev, Editability value)
            {
                m_BookEditability[sBookAbbrev] = value;
            }
            #endregion

            public enum GEditability { Full, Notes, ReadOnly, Custom };
            public GEditability GlobalEditability = GEditability.Full;

            // Private methods/etc.
            private Dictionary<string, Editability> m_BookEditability;
            #region method: void InitializeEditability()
            private void InitializeEditability()
            {
                m_BookEditability = new Dictionary<string, Editability>();
                foreach(var sBookAbbrev in DBook.BookAbbrevs)
                    m_BookEditability.Add(sBookAbbrev, Editability.Full);
            }
            #endregion]
            #region method: string GetListOfBooks(Editability)
            string GetListOfBooks(Editability editability)
            {
                var s = "";
                foreach(var sBookAbbrev in DBook.BookAbbrevs)
                {
                    if (GetEditability(sBookAbbrev) == editability)
                        s += sBookAbbrev + " ";
                }
                return s.Trim();
            }
            #endregion
            #region method: void SetListOfBooks(s, Editability)
            void SetListOfBooks(string s, Editability editability)
            {
                var vsBookAbbrevs = s.Split(new char[] {' '});
                foreach(string sBookAbbrev in vsBookAbbrevs)
                    SetEditability(sBookAbbrev, editability);
            }
            #endregion

            // Permissions -------------------------------------------------------------------
            public bool CanCreateGeneralNotes { get; set; }

            // Scaffolding -------------------------------------------------------------------
            #region public string TranslationName
            public string TranslationName
            {
                get
                {
                    Debug.Assert(!string.IsNullOrEmpty(m_sTranslationName));
                    return m_sTranslationName;
                }
            }
            private readonly string m_sTranslationName;
            #endregion
            #region Constructor(sTranslationName)
            public TranslationSettings(string sTranslationName)
            {
                m_sTranslationName = sTranslationName;
                InitializeEditability();
            }
            #endregion
            #region Query: bool ContentEquals(other)
            public bool ContentEquals(TranslationSettings other)
            {
                foreach(var sBookAbbrev in DBook.BookAbbrevs)
                {
                    if (GetEditability(sBookAbbrev) != other.GetEditability(sBookAbbrev))
                        return false;
                }

                if (CanCreateGeneralNotes != other.CanCreateGeneralNotes)
                    return false;
                if (GlobalEditability != other.GlobalEditability)
                    return false;

                return true;
            }
            #endregion
            #region Method: TranslationSettings Clone()
            public TranslationSettings Clone()
            {
                var ts = new TranslationSettings(TranslationName)
                {
                    CanCreateGeneralNotes = CanCreateGeneralNotes,
                    GlobalEditability = GlobalEditability,
                };

                foreach(var sBookAbbrev in DBook.BookAbbrevs)
                {
                    ts.SetEditability(sBookAbbrev, GetEditability(sBookAbbrev));
                }

                return ts;
            }
            #endregion

            // I/O ---------------------------------------------------------------------------
            #region Constants
            private const string c_sTag = "TranslationSettings";
            private const string c_sAttrTranslationName = "name";
            private const string c_sAttrCanCreateGeneralNotes = "generalNotes";
            private const string c_sAttrGlobalEditability = "globalEditability";
            #endregion
            #region Method: XmlNode Save(XmlDoc, nodeParent)
            public XmlNode Save(XmlDoc doc, XmlNode nodeParent)
            {
                var node = doc.AddNode(nodeParent, c_sTag);

                doc.AddAttr(node, c_sAttrTranslationName, m_sTranslationName);
                doc.AddAttr(node, c_sAttrCanCreateGeneralNotes, CanCreateGeneralNotes);
                doc.AddAttr(node, c_sAttrGlobalEditability, GlobalEditability.ToString());

                foreach (var sEditability in Enum.GetNames(typeof(Editability)))
                {
                    var editability = (Editability)Enum.Parse(typeof(Editability), sEditability);
                    if (editability == Editability.Full)
                        continue;
                    doc.AddAttr(node, sEditability, GetListOfBooks(editability));
                }

                return node;
            }
            #endregion
            #region SMethod: TranslationSettings Create(node)
            static public TranslationSettings Create(XmlNode node)
            {
                if (node.Name != c_sTag)
                    return null;

                var sName = XmlDoc.GetAttrValue(node, c_sAttrTranslationName, "");
                if (string.IsNullOrEmpty(sName))
                    return null;

                var ts = new TranslationSettings(sName)
                { 
                    CanCreateGeneralNotes = XmlDoc.GetAttrValue(node, c_sAttrCanCreateGeneralNotes, false),
                    GlobalEditability = (GEditability)Enum.Parse(
                        typeof(GEditability),
                        XmlDoc.GetAttrValue(node, c_sAttrGlobalEditability, GEditability.Full.ToString()),
                        true),
                };

                foreach(var sEditability in Enum.GetNames(typeof(Editability)))
                {
                    var editability = (Editability)Enum.Parse(typeof(Editability), sEditability);
                    if (editability == Editability.Full)
                        continue;

                    var sBooks = XmlDoc.GetAttrValue(node, sEditability, "").Trim();
                    ts.SetListOfBooks(sBooks, editability);
                }

                return ts;
            }
            #endregion
            #region Method: void Merge(parent, theirs)
            public void Merge(TranslationSettings parent, TranslationSettings theirs)
                // Algorithm: We keep theirs iff they differ from ours and ours is
                // unchanged from the parent. Otherwise we always keep ours.
            {
                Debug.Assert(parent != null);
                Debug.Assert(theirs != null);

                if (CanCreateGeneralNotes != theirs.CanCreateGeneralNotes &&
                    CanCreateGeneralNotes == parent.CanCreateGeneralNotes)
                {
                    CanCreateGeneralNotes = theirs.CanCreateGeneralNotes;
                }

                if (GlobalEditability != theirs.GlobalEditability &&
                    GlobalEditability == parent.GlobalEditability)
                {
                    GlobalEditability = theirs.GlobalEditability;
                }

                foreach(var sBookAbbrev in DBook.BookAbbrevs)
                {
                    var eParent = parent.GetEditability(sBookAbbrev);
                    var eTheirs = theirs.GetEditability(sBookAbbrev);
                    var eMine = GetEditability(sBookAbbrev);

                    if (eMine != eTheirs && eMine == eParent)
                        SetEditability(sBookAbbrev, eTheirs);

                    // If even one book is not fully editable, then no matter how
                    // we set GlobalEditability earlier; we must leave it as
                    // Custom here, or else the book's editability will be ignored.
                    if (GetEditability(sBookAbbrev) != Editability.Full)
                        GlobalEditability = GEditability.Custom;
                }
            }
            #endregion
        }
        #endregion
        private readonly Dictionary<string, TranslationSettings> m_vTranslationSettings;
        #region Method: TranslationSettings FindTranslationSettings(sTranslationName)
        public TranslationSettings FindTranslationSettings(string sTranslationName)
        {
            TranslationSettings settings;
            m_vTranslationSettings.TryGetValue(sTranslationName, out settings);
            return settings;
        }
        #endregion
        #region Query: bool IsMemberOf(sTranslationName)
        public bool IsMemberOf(string sTranslationName)
        {
            return (null != FindTranslationSettings(sTranslationName));
        }
        #endregion
        #region Method: void AddMembershipTo(sTranslationName)
        public void AddMembershipTo(string sTranslationName)
        {
            if (IsMemberOf(sTranslationName))
                return;

            var settings = new TranslationSettings(sTranslationName);
            m_vTranslationSettings.Add(sTranslationName, settings);
        }
        #endregion
        #region Method: void RemoveMembershipFrom(sTranslationName)
        public void RemoveMembershipFrom(string sTranslationName)
        {
            if (!IsMemberOf(sTranslationName))
                return;

            m_vTranslationSettings.Remove(sTranslationName);
        }
        #endregion
        #region Attr{g}: string MemberProjects
        public string MemberProjects
        {
            get
            {
                var s = "";
                foreach (var sProjectName in m_vTranslationSettings.Keys)
                    s += sProjectName + ", ";
                if (s.EndsWith(", "))
                    s = s.Remove(s.Length - 2);
                return s;
            }
        }
        #endregion

        // Editable (vs locked, etc.) books --------------------------------------------------
        #region Method: bool GetEditability(sTranslationName, sBookName)
        public TranslationSettings.Editability GetEditability(string sTranslationName, 
            string sBookAbbrev)
        {
            if (!IsMemberOf(sTranslationName))
                return TranslationSettings.Editability.ReadOnly;

            var settings = FindTranslationSettings(sTranslationName);
            Debug.Assert(null != settings);
            return settings.GetEditability(sBookAbbrev);
        }
        #endregion
        #region Method: Editability GetEditability(DBook)
        public TranslationSettings.Editability GetEditability(DBook book)
        {
            return GetEditability(book.Translation.DisplayName, book.BookAbbrev);
        }
        #endregion
        #region Method: bool SetEditability(sTranslationName, sBookName, editability)
        public void SetEditability(string sTranslationName, string sBookAbbrev, 
            TranslationSettings.Editability editability)
        {
            var settings = FindTranslationSettings(sTranslationName);

            // We want to be explicitly setting team membership in the caller, rather than just
            // automatically doing it here
            if (null == settings)
                throw new ArgumentException("This user is not a member of the team: " + sTranslationName);

            settings.GlobalEditability = TranslationSettings.GEditability.Custom;
            settings.SetEditability(sBookAbbrev, editability);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public User()
        {
            m_vTranslationSettings = new Dictionary<string, TranslationSettings>();

            // Default values
            MaximizeWindowOnStartup = true;
            ZoomPercent = 100;
            PrimaryUiLanguage = LocItem.c_sEnglish;
            SecondaryUiLanguage = LocItem.c_sEnglish;

            DraftingWindowBackground = "Wheat";
            BackTranslationWindowBackground = "Linen";
            NaturalnessWindowBackground = "Wheat";
            ConsultantWindowBackground = "LightYellow";
        }
        #endregion
        #region Query: bool ContentEquals(other)
        public bool ContentEquals(User other)
            // Originally I was comparing each and every attribute; but this is fragile should
            // another attribute be added. Rather, I now just compare the XML string that is
            // produced on a save. The burden is now only on Save to be an inventory of all of
            // the attributes. This method may be a bit slower, but definitely more robust.
        {
            var doc = new XmlDoc();
            var sXmlThis = Save(doc, null).OuterXml;

            doc = new XmlDoc();
            var sXmlOther = other.Save(doc, null).OuterXml;

            return (sXmlThis == sXmlOther);
        }
        #endregion
        #region Method: User Clone()
        public User Clone()
            // Previously I was creating a new User and then individually setting all of its
            // attributes to those of "this". But this was fragile, because if I added a new
            // attribute I had to remember to add corresponding code here. So now, I instead
            // just create from an Xml save. This relies on the attributes being accounted
            // for in the IO area; but this is still much more robust because unit tests should
            // take not of missing attrs.
        {
            var doc = new XmlDoc();
            var node = Save(doc, null);

            return Create(node);
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        public const string c_sFileMask = "*.user";
        private const string c_sTag = "User";
        private const string c_sAttrUserName = "username";
        private const string c_sAttrPassword = "password";
        private const string c_sAttrAdministrator = "administrator";

        // General options
        private const string c_sAttrMaximizeWindowOnStartup = "maxWin";
        private const string c_sAttrZoomPercent = "zoom";
        private const string c_sAttrPrimaryUiLanguage = "primaryUi";
        private const string c_sAttrSecondaryUiLanguage = "secondaryUi";
        private const string c_sAttrDraftingWindowBackground = "DraftingBg";
        private const string c_sAttrBackTranslationWindowBackground = "BackTranslationBg";
        private const string c_sAttrNaturalnessWindowBackground = "NaturalnessBg";
        private const string c_sAttrConsultantWindowBackground = "ConsultantBg";
        private const string c_sAttrCollaborationUserName = "collaborationUserName";
        private const string c_sAttrCollaborationPassword = "collaborationPassword";

        // Features
        private const string c_sAttrCanEditStructure = "canEditStructure";
        private const string c_sAttrCanUndoRedo = "canUndoRedo";
        private const string c_sAttrCanNavFirstLast = "canNavFirstLast";
        private const string c_sAttrCanDoBackTranslation = "canDoBackTranslation";
        private const string c_sAttrCanDoNaturalnessCheck = "canDoNaturalnessCheck";
        private const string c_sAttrCanDoConsultantPreparation = "canDoConsultantPreparation";
        private const string c_sAttrCanZoom = "canZoom";
        private const string c_sAttrCanCreateProject = "canCreateProject";
        private const string c_sAttrCanOpenProject = "canOpenProject";
        private const string c_sAttrCanExportProject = "canExportProject";
        private const string c_sAttrCanPrint = "canPrint";
        private const string c_sAttrCanFilter = "canFilter";
        private const string c_sAttrCanLocalize = "canLocalize";
        private const string c_sAttrCanRestoreBackups = "canRestoreBackups";

        // Notes
        private const string c_sAttrCanMakeNotes = "canMakeNotes";
        private const string c_sAttrNoteAuthor = "noteAuthor";
        private const string c_sAttrCloseNotesWindowWhenMouseLeaves = "closeNoteWindowWhenMouseLeaves";
        private const string c_sAttrShowExpandedNotesIcon = "showExpandedNotesIcon";
        private const string c_sAttrCanDeleteNotesAuthoredByOthers = "canDeleteNotesAuthoredByOthers";
        private const string c_sAttrCanAuthorHintForDaughterNotes = "canAuthorHintForDaugherNotes";
        private const string c_sAttrCanAuthorInformationNotes = "canAuthorInformationNotes";
        private const string c_sAttrCanAssignNoteToConsultant = "canAssignNoteToConsultant";
        private const string c_sAttrCanCreateFrontTranslationNotes = "canCreateFrontTranslationNotes";


        #endregion
        private string m_sXmlSnapshot;
        #region Method: XmlNode Save(XmlDoc, nodeParent)
        public XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        {
            var node = doc.AddNode(nodeParent, c_sTag);

            doc.AddAttr(node, c_sAttrUserName, UserName);
            doc.AddAttr(node, c_sAttrPassword, Password);
            doc.AddAttr(node, c_sAttrAdministrator, IsAdministrator);

            // General Options
            doc.AddAttr(node, c_sAttrMaximizeWindowOnStartup, MaximizeWindowOnStartup);
            doc.AddAttr(node, c_sAttrZoomPercent, ZoomPercent);
            doc.AddAttr(node, c_sAttrPrimaryUiLanguage, PrimaryUiLanguage);
            doc.AddAttr(node, c_sAttrSecondaryUiLanguage, SecondaryUiLanguage);
            doc.AddAttr(node, c_sAttrDraftingWindowBackground, DraftingWindowBackground);
            doc.AddAttr(node, c_sAttrBackTranslationWindowBackground, BackTranslationWindowBackground);
            doc.AddAttr(node, c_sAttrNaturalnessWindowBackground, NaturalnessWindowBackground);
            doc.AddAttr(node, c_sAttrConsultantWindowBackground, ConsultantWindowBackground);
            doc.AddAttr(node, c_sAttrCollaborationUserName, CollaborationUserName);
            doc.AddAttr(node, c_sAttrCollaborationPassword, CollaborationPassword);

            // Features
            doc.AddAttr(node, c_sAttrCanEditStructure, CanEditStructure);
            doc.AddAttr(node, c_sAttrCanUndoRedo, CanUndoRedo);
            doc.AddAttr(node, c_sAttrCanNavFirstLast, CanNavigateFirstLast);
            doc.AddAttr(node, c_sAttrCanDoBackTranslation, CanDoBackTranslation);
            doc.AddAttr(node, c_sAttrCanDoNaturalnessCheck, CanDoNaturalnessCheck);
            doc.AddAttr(node, c_sAttrCanDoConsultantPreparation, CanDoConsultantPreparation);
            doc.AddAttr(node, c_sAttrCanZoom, CanZoom);
            doc.AddAttr(node, c_sAttrCanCreateProject, CanCreateProject);
            doc.AddAttr(node, c_sAttrCanOpenProject, CanOpenProject);
            doc.AddAttr(node, c_sAttrCanExportProject, CanExportProject);
            doc.AddAttr(node, c_sAttrCanPrint, CanPrint);
            doc.AddAttr(node, c_sAttrCanFilter, CanFilter);
            doc.AddAttr(node, c_sAttrCanLocalize, CanLocalize);
            doc.AddAttr(node, c_sAttrCanRestoreBackups, CanRestoreBackups);

            // Notes
            doc.AddAttr(node, c_sAttrCanMakeNotes, CanMakeNotes);
            doc.AddAttr(node, c_sAttrNoteAuthor, NoteAuthorsName);
            doc.AddAttr(node, c_sAttrCloseNotesWindowWhenMouseLeaves, CloseNotesWindowWhenMouseLeaves);
            doc.AddAttr(node, c_sAttrShowExpandedNotesIcon, ShowExpandedNotesIcon);
            doc.AddAttr(node, c_sAttrCanDeleteNotesAuthoredByOthers, CanDeleteNotesAuthoredByOthers);
            doc.AddAttr(node, c_sAttrCanAuthorHintForDaughterNotes, CanAuthorHintForDaughterNotes);
            doc.AddAttr(node, c_sAttrCanAuthorInformationNotes, CanAuthorInformationNotes);
            doc.AddAttr(node, c_sAttrCanAssignNoteToConsultant, CanAssignNoteToConsultant);
            doc.AddAttr(node, c_sAttrCanCreateFrontTranslationNotes, CanCreateFrontTranslationNotes);

            foreach (var ts in m_vTranslationSettings.Values)
                ts.Save(doc, node);

            return node;
        }
        #endregion
        #region Method: void Save(string sFolder)
        public void Save(string sFolder)
        {
            var sPath = Path.Combine(sFolder, UserName + ".user");

            var doc = new XmlDoc();
            Save(doc, null);

            // If the same, then nothing changed and we don't need to write
            if (File.Exists(sPath) && m_sXmlSnapshot == doc.OuterXml)
                return;
            m_sXmlSnapshot = doc.OuterXml;

            doc.Write(sPath);
        }
        #endregion
        #region Method: User Create(XmlNode node)
        static public User Create(XmlNode node)
        {
            if (node.Name != c_sTag)
                return null;

            var sUser = XmlDoc.GetAttrValue(node, c_sAttrUserName, "");
            if (string.IsNullOrEmpty(sUser))
                return null;

            var user = new User {
                UserName = sUser,
                Password = XmlDoc.GetAttrValue(node, c_sAttrPassword, ""),
                IsAdministrator = XmlDoc.GetAttrValue(node, c_sAttrAdministrator, false),

                // General Options
                MaximizeWindowOnStartup = XmlDoc.GetAttrValue(node, c_sAttrMaximizeWindowOnStartup, false),
                ZoomPercent = XmlDoc.GetAttrValue(node, c_sAttrZoomPercent, 100),
                PrimaryUiLanguage = XmlDoc.GetAttrValue(node, c_sAttrPrimaryUiLanguage, "English"),
                SecondaryUiLanguage = XmlDoc.GetAttrValue(node, c_sAttrSecondaryUiLanguage, "English"),
                DraftingWindowBackground = XmlDoc.GetAttrValue(node, c_sAttrDraftingWindowBackground, Color.Wheat.Name),
                BackTranslationWindowBackground = XmlDoc.GetAttrValue(node, c_sAttrBackTranslationWindowBackground, Color.Linen.Name),
                NaturalnessWindowBackground = XmlDoc.GetAttrValue(node, c_sAttrNaturalnessWindowBackground, Color.Wheat.Name),
                ConsultantWindowBackground = XmlDoc.GetAttrValue(node, c_sAttrConsultantWindowBackground, Color.Wheat.Name),
                CollaborationUserName = XmlDoc.GetAttrValue(node, c_sAttrCollaborationUserName, ""),
                CollaborationPassword = XmlDoc.GetAttrValue(node, c_sAttrCollaborationPassword, ""),

                // Features
                CanEditStructure = XmlDoc.GetAttrValue(node, c_sAttrCanEditStructure, false),
                CanUndoRedo = XmlDoc.GetAttrValue(node, c_sAttrCanUndoRedo, false),
                CanNavigateFirstLast = XmlDoc.GetAttrValue(node, c_sAttrCanNavFirstLast, false),
                CanDoBackTranslation = XmlDoc.GetAttrValue(node, c_sAttrCanDoBackTranslation, false),
                CanDoNaturalnessCheck = XmlDoc.GetAttrValue(node, c_sAttrCanDoNaturalnessCheck, false),
                CanDoConsultantPreparation = XmlDoc.GetAttrValue(node, c_sAttrCanDoConsultantPreparation, false),
                CanZoom = XmlDoc.GetAttrValue(node, c_sAttrCanZoom, false),
                CanCreateProject = XmlDoc.GetAttrValue(node, c_sAttrCanCreateProject, false),
                CanOpenProject = XmlDoc.GetAttrValue(node, c_sAttrCanOpenProject, false),
                CanExportProject = XmlDoc.GetAttrValue(node, c_sAttrCanExportProject, false),
                CanPrint = XmlDoc.GetAttrValue(node, c_sAttrCanPrint, false),
                CanFilter = XmlDoc.GetAttrValue(node, c_sAttrCanFilter, false),
                CanLocalize = XmlDoc.GetAttrValue(node, c_sAttrCanLocalize, false),
                CanRestoreBackups = XmlDoc.GetAttrValue(node, c_sAttrCanRestoreBackups, false),

                // Notes
                CanMakeNotes = XmlDoc.GetAttrValue(node, c_sAttrCanMakeNotes, false),
                NoteAuthorsName = XmlDoc.GetAttrValue(node, c_sAttrNoteAuthor, ""),
                CloseNotesWindowWhenMouseLeaves = XmlDoc.GetAttrValue(node, c_sAttrCloseNotesWindowWhenMouseLeaves, false),
                ShowExpandedNotesIcon = XmlDoc.GetAttrValue(node, c_sAttrShowExpandedNotesIcon, false),
                CanDeleteNotesAuthoredByOthers = XmlDoc.GetAttrValue(node, c_sAttrCanDeleteNotesAuthoredByOthers, false),
                CanAuthorHintForDaughterNotes = XmlDoc.GetAttrValue(node, c_sAttrCanAuthorHintForDaughterNotes, false),
                CanAuthorInformationNotes = XmlDoc.GetAttrValue(node, c_sAttrCanAuthorInformationNotes, false),
                CanAssignNoteToConsultant = XmlDoc.GetAttrValue(node, c_sAttrCanAssignNoteToConsultant, false),
                CanCreateFrontTranslationNotes = XmlDoc.GetAttrValue(node, c_sAttrCanCreateFrontTranslationNotes, false),
            };

            foreach(XmlNode child in node.ChildNodes)
            {
                var ts = TranslationSettings.Create(child);
                if (null != ts)
                    user.m_vTranslationSettings.Add(ts.TranslationName, ts);
            }

            return user;
        }
        #endregion
        #region SMethod: User Create(string sPath)
        static public User Create(string sPath)
        {
            var doc = new XmlDoc();
            doc.Load(sPath);          
            var node = XmlDoc.FindNode(doc, c_sTag);
            var newUser = Create(node);

            // Take a snapshop of the data so we'll know if anything changes, and thus
            // if we'll need to do a Save
            newUser.m_sXmlSnapshot = doc.OuterXml;

            return newUser;
        }
        #endregion
        #region Method: void Merge(User parent, User theirs)
        public void Merge(User parent, User theirs)
        {
            Debug.Assert(parent != null);
            Debug.Assert(theirs != null);

            Debug.Assert(UserName == parent.UserName);
            Debug.Assert(UserName == theirs.UserName);

            // Simple Attributes
            IsAdministrator = Merger.Merge(IsAdministrator, parent.IsAdministrator, theirs.IsAdministrator);
            Password = Merger.Merge(Password, parent.Password, theirs.Password);

            // General Options
            MaximizeWindowOnStartup = Merger.Merge(MaximizeWindowOnStartup, parent.MaximizeWindowOnStartup, theirs.MaximizeWindowOnStartup);
            ZoomPercent = Merger.Merge(ZoomPercent, parent.ZoomPercent, theirs.ZoomPercent);
            PrimaryUiLanguage = Merger.Merge(PrimaryUiLanguage, parent.PrimaryUiLanguage, theirs.PrimaryUiLanguage);
            SecondaryUiLanguage = Merger.Merge(SecondaryUiLanguage, parent.SecondaryUiLanguage, theirs.SecondaryUiLanguage);
            DraftingWindowBackground = Merger.Merge(DraftingWindowBackground, parent.DraftingWindowBackground, theirs.DraftingWindowBackground);
            BackTranslationWindowBackground = Merger.Merge(BackTranslationWindowBackground, parent.BackTranslationWindowBackground, theirs.BackTranslationWindowBackground);
            NaturalnessWindowBackground = Merger.Merge(NaturalnessWindowBackground, parent.NaturalnessWindowBackground, theirs.NaturalnessWindowBackground);
            ConsultantWindowBackground = Merger.Merge(ConsultantWindowBackground, parent.ConsultantWindowBackground, theirs.ConsultantWindowBackground);
            CollaborationUserName = Merger.Merge(CollaborationUserName, parent.CollaborationUserName, theirs.CollaborationUserName);
            CollaborationPassword = Merger.Merge(CollaborationPassword, parent.CollaborationPassword, theirs.CollaborationPassword);

            // Features
            CanEditStructure = Merger.Merge(CanEditStructure, parent.CanEditStructure, theirs.CanEditStructure);
            CanUndoRedo = Merger.Merge(CanUndoRedo, parent.CanUndoRedo, theirs.CanUndoRedo);
            CanNavigateFirstLast = Merger.Merge(CanNavigateFirstLast, parent.CanNavigateFirstLast, theirs.CanNavigateFirstLast);
            CanDoBackTranslation = Merger.Merge(CanDoBackTranslation, parent.CanDoBackTranslation, theirs.CanDoBackTranslation);
            CanDoNaturalnessCheck = Merger.Merge(CanDoNaturalnessCheck, parent.CanDoNaturalnessCheck, theirs.CanDoNaturalnessCheck);
            CanDoConsultantPreparation = Merger.Merge(CanDoConsultantPreparation, parent.CanDoConsultantPreparation, theirs.CanDoConsultantPreparation);
            CanZoom = Merger.Merge(CanZoom, parent.CanZoom, theirs.CanZoom);
            CanCreateProject = Merger.Merge(CanCreateProject, parent.CanCreateProject, theirs.CanCreateProject);
            CanOpenProject = Merger.Merge(CanOpenProject, parent.CanOpenProject, theirs.CanOpenProject);
            CanExportProject = Merger.Merge(CanExportProject, parent.CanExportProject, theirs.CanExportProject);
            CanPrint = Merger.Merge(CanPrint, parent.CanPrint, theirs.CanPrint);
            CanFilter = Merger.Merge(CanFilter, parent.CanFilter, theirs.CanFilter);
            CanLocalize = Merger.Merge(CanLocalize, parent.CanLocalize, theirs.CanLocalize);
            CanRestoreBackups = Merger.Merge(CanRestoreBackups, parent.CanRestoreBackups, theirs.CanRestoreBackups);

            // Simple Attrs: Notes
            CanMakeNotes = Merger.Merge(CanMakeNotes, parent.CanMakeNotes, theirs.CanMakeNotes);
            NoteAuthorsName = Merger.Merge(NoteAuthorsName, parent.NoteAuthorsName, theirs.NoteAuthorsName);
            CloseNotesWindowWhenMouseLeaves = Merger.Merge(CloseNotesWindowWhenMouseLeaves, parent.CloseNotesWindowWhenMouseLeaves, theirs.CloseNotesWindowWhenMouseLeaves);
            ShowExpandedNotesIcon = Merger.Merge(ShowExpandedNotesIcon, parent.ShowExpandedNotesIcon, theirs.ShowExpandedNotesIcon);
            CanDeleteNotesAuthoredByOthers = Merger.Merge(CanDeleteNotesAuthoredByOthers, parent.CanDeleteNotesAuthoredByOthers, theirs.CanDeleteNotesAuthoredByOthers);
            CanAuthorHintForDaughterNotes = Merger.Merge(CanAuthorHintForDaughterNotes, parent.CanAuthorHintForDaughterNotes, theirs.CanAuthorHintForDaughterNotes);
            CanAuthorInformationNotes = Merger.Merge(CanAuthorInformationNotes, parent.CanAuthorInformationNotes, theirs.CanAuthorInformationNotes);
            CanAssignNoteToConsultant = Merger.Merge(CanAssignNoteToConsultant, parent.CanAssignNoteToConsultant, theirs.CanAssignNoteToConsultant);
            CanCreateFrontTranslationNotes = Merger.Merge(CanCreateFrontTranslationNotes, parent.CanCreateFrontTranslationNotes, theirs.CanCreateFrontTranslationNotes);

            // Merge any translation settings that exist in all three)))
            foreach(var ourTS in m_vTranslationSettings.Values)
            {
                var parentTS = parent.FindTranslationSettings(ourTS.TranslationName);
                var theirTS = theirs.FindTranslationSettings(ourTS.TranslationName);
                if (null != parentTS && null != theirTS)
                    ourTS.Merge(parentTS, theirTS);
            }

            // Add any new TSs that only exist in theirs
            foreach(var theirTS in theirs.m_vTranslationSettings.Values)
            {
                var ourTS = FindTranslationSettings(theirTS.TranslationName);
                var parentTS = parent.FindTranslationSettings(theirTS.TranslationName);
                if (null == parentTS && null == ourTS)
                    m_vTranslationSettings.Add(theirTS.TranslationName, theirTS.Clone());
            }
        }
        #endregion
        #region static void Merge(MergeOrder)
        public static void Merge(MergeOrder mergeOrder)
        {
            // Debug.Fail("User.Merge() - Break for Debugging");

            if (mergeOrder == null)
                throw new ArgumentNullException("mergeOrder");

            // Initializations (TODO dependencies we should get rid of someday)
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";
            LocDB.Initialize(Loc.FolderOfLocFiles);

            // Read in the three versions of the User
            var ours = Create(mergeOrder.pathToOurs);
            var theirs = Create(mergeOrder.pathToTheirs);
            var parent = Create(mergeOrder.pathToCommonAncestor);

            // Do the merge
            ours.Merge(parent, theirs);

            // Save the results
            ours.Save(Path.GetDirectoryName(mergeOrder.pathToOurs));
        }
        #endregion

        // Queries ---------------------------------------------------------------------------
        #region Attr{g}: bool CanSendReceive
        public bool CanSendReceive
        {
            get
            {
                if (string.IsNullOrEmpty(CollaborationUserName))
                    return false;

                if (string.IsNullOrEmpty(CollaborationPassword))
                    return false;

                return true;
            }
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Query: bool VerifyPassword(string sPassword)
        public bool VerifyPassword(string sPassword)
        {
            return (sPassword == Password);
        }
        #endregion
    }
}
