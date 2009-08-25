#region ***** TranslatorNote.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TranslatorNote.cs
 * Author:  John Wimbish
 * Created: 04 Nov 2008
 * Purpose: Handles a translator's (or consultant's) note in Scripture. 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using JWTools;
using JWdb;
#endregion
#endregion

// TODO: Do we want a Context for the vernacular, and another for the BT? Thus different things
// would potentially show in each view; or alternatively, if no context we could decide to
// not show the note in that particular view? Currently, the Context is not really all that
// meaningful, in that we don't highlight it in the text. But when we do, we'll regret contexts
// that don't work for us.

namespace JWdb.DataModel
{
    #region Class: Discussion : JObject
    public class Discussion : JObject
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: string Author
        public string Author
        {
            get
            {
                return m_sAuthor;
            }
            set
            {
                SetValue(ref m_sAuthor, value);
            }
        }
        private string m_sAuthor = "";
        #endregion
        #region BAttr{g/s}: DateTime Created
        public DateTime Created
        {
            get
            {
                return m_dtCreated;
            }
            set
            {
                SetValue(ref m_dtCreated, value);
            }
        }
        private DateTime m_dtCreated;
        #endregion
        #region JAttr{g}: JOwnSeq<DParagraph> Paragraphs
        public JOwnSeq<DParagraph> Paragraphs
        {
            get
            {
                return m_osParagraphs;
            }
        }
        JOwnSeq<DParagraph> m_osParagraphs;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Author", ref m_sAuthor);
            DefineAttr("Created", ref m_dtCreated);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public Discussion()
            : base()
        {
            // Create the Paragraph attribute
            m_osParagraphs = new JOwnSeq<DParagraph>("paras", this, false, false);

            // Put an empty paragraph in it, with the default style
            DParagraph p = new DParagraph();
            p.StyleAbbrev = DStyleSheet.c_StyleNoteDiscussion;
            p.SimpleText = "";
            Paragraphs.Append(p);

            // The Default date is "Today"
            m_dtCreated = DateTime.Now;

            // The author
            Author = DB.UserName;

            Debug_VerifyIntegrity();
        }
        #endregion
        #region Constructor(sAuthor, string sSimpleText)
        public Discussion(string sAuthor, DateTime dtCreated, string sSimpleText)
            : this()
        {
            Author = sAuthor;
            Created = dtCreated;

            // Temporary kludge: remove ||'s that we've  been inserting by mistake
            string sFixed = DSection.IO.EatSpuriousVerticleBars(sSimpleText);

            // Parse the string into phrases
            List<DRun> vRuns = DSection.IO.CreateDRunsFromInputText(sFixed);

            // Add the phrases to our one-and-only paragraph
            // TODO: Multiple paragraphs, of course!
            Debug.Assert(1 == Paragraphs.Count);
            Paragraphs[0].Runs.Clear();
            foreach (DRun run in vRuns)
            {
                DText text = run as DText;
                if (text != null && text.PhrasesBT.Count == 0)
                    text.PhrasesBT.Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));
                Paragraphs[0].Runs.Append(run);
            }

            Debug_VerifyIntegrity();
        }
        #endregion
        #region OMethod: bool ContentEquals(Discussion)
 		public override bool ContentEquals(JObject obj)
        {
            Discussion discussion = obj as Discussion;
            if (null == discussion)
                return false;

            if (discussion.Author != Author)
                return false;
            if (discussion.Created.CompareTo(Created) != 0)
                return false;

            if (!discussion.Paragraphs.ContentEquals(Paragraphs))
                return false;

            return true;
        }
        #endregion
        #region Method: Discussion Clone()
        public Discussion Clone()
        {
            Discussion d = new Discussion();
            d.Author = Author;
            d.Created = Created;
            d.CopyParagraphsFrom(this);
            return d;
        }
        #endregion
        #region OAttr{g}: string SortKey
        public override string SortKey
        {
            get
            {
                // Return Created in the universal invariant form of
                //    "2006-04-17 21:22:48Z"
                return Created.ToString("u");
            }
        }
        #endregion
        #region VAttr{g}: TranslatorNote Note
        public TranslatorNote Note
        {
            get
            {
                TranslatorNote tn = Owner as TranslatorNote;
                Debug.Assert(null != tn);
                return tn;
            }
        }
        #endregion
        #region DEBUG SUPPORT
        public void Debug_VerifyIntegrity()
        {
        #if DEBUG
            foreach (DParagraph p in Paragraphs)
            {
                // Make sure we have a DText in every paragraph
                Debug.Assert(p.Runs.Count > 0);
                DText text = null;
                foreach (DRun r in p.Runs)
                {
                    text = r as DText;
                    if (null != text)
                        break;
                }
                Debug.Assert(null != text);

                // Make sure we have a phrase in the DText
                Debug.Assert(text.Phrases.Count > 0);
            }
        #endif
        }
        #region Attr{g}: string DebugString
        public string DebugString
        {
            get
            {
                string s = "D: Author={" + Author + "} Created={" + Created.ToShortDateString() +
                    "} + Content={";

                foreach(DParagraph p in Paragraphs)
                    s += ("p:<" + p.DebugString + ">");

                s += "}";
                return s;
            }
        }
        #endregion
        #endregion

        // View Construction -----------------------------------------------------------------
        #region VAttr{g}: bool IsEditable
        public bool IsEditable
        {
            get
            {
                // If the owning Note isn't editable, then this discussion isn't.
                if (!Note.IsEditable)
                    return false;

                // If not the last one in the Note, it isn't
                if (this != Note.LastDiscussion)
                    return false;

                // If the author is someone different from me, it isn't
                if (Author != DB.UserName)
                    return false;

                return true;
            }
        }
        #endregion
        #region VAttr{g}: DParagraph FirstParagraph
        public DParagraph FirstParagraph
        {
            get
            {
                Debug.Assert(0 < Paragraphs.Count);
                return Paragraphs[0];
            }
        }
        #endregion       
        #region VAttr{g}: DParagraph LastParagraph
        public DParagraph LastParagraph
        {
            get
            {
                int c = Paragraphs.Count;
                Debug.Assert(0 != c);
                DParagraph p = Paragraphs[c - 1];
                Debug.Assert(null != p);
                return p;
            }
        }
        #endregion

        // Oxes ------------------------------------------------------------------------------
        #region Constants
        public const string c_sTagDiscussion = "Discussion";
        const string c_sTagAuthor = "author";
        const string c_sTagCreated = "created";
        #endregion
        #region SMethod: Discussion Create(nodeDiscussion)
        static public Discussion Create(XmlNode nodeDiscussion)
        {
            if (nodeDiscussion.Name != c_sTagDiscussion)
                return null;

            // Create the new picture object. Delete the paragraph that the constructor will
            // have otherwise created.
            var discussion = new Discussion();
            discussion.Paragraphs.Clear();

            // Attrs
            discussion.Author = XmlDoc.GetAttrValue(nodeDiscussion, c_sTagAuthor, "");
            discussion.Created = XmlDoc.GetAttrValue(nodeDiscussion, c_sTagCreated, DateTime.Now);

            // Paragraphs
            foreach (XmlNode nodeParagraph in nodeDiscussion.ChildNodes)
            {
                var paragraph = DParagraph.CreateParagraph(nodeParagraph);
                if (null != paragraph)
                    discussion.Paragraphs.Append(paragraph);
            }

            return discussion;
        }
        #endregion
        #region Method: XmlNode Save(oxes, nodeAnnotation)
        public XmlNode Save(XmlDoc oxes, XmlNode nodeAnnotation)
        {
            var nodeDiscussion = oxes.AddNode(nodeAnnotation, c_sTagDiscussion);

            // Attrs
            oxes.AddAttr(nodeDiscussion, c_sTagAuthor, Author);
            oxes.AddAttr(nodeDiscussion, c_sTagCreated,Created);

            // Paragraphs
            foreach (DParagraph p in Paragraphs)
                p.SaveToOxesBook(oxes, nodeDiscussion);

            return nodeDiscussion;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: bool IsSameOriginAs(Theirs)
        public bool IsSameOriginAs(Discussion Theirs)
            // Two discussions started out the same if they have the same Author and 
            // Create date.
        {
            if (0 != Created.CompareTo(Theirs.Created))
                return false;
            if (Author != Theirs.Author)
                return false;
            return true;
        }
        #endregion
        #region Method: void CopyParagraphsFrom(Discussion source)
        void CopyParagraphsFrom(Discussion source)
        {
            Paragraphs.Clear();
            foreach(DParagraph p in source.Paragraphs)
            {
                DParagraph pCopy = new DParagraph();
                pCopy.CopyFrom(p, false);
                Paragraphs.Append(pCopy);
            }
        }
        #endregion
        #region Method: void Merge(Discussion Parent, Discussion Theirs)
        public void Merge(Discussion Parent, Discussion Theirs)
        {
            // The caller needs to make sure these are the same Author and Created
            Debug.Assert(IsSameOriginAs(Theirs), "Theirs wasn't the same discussion");
            Debug.Assert(IsSameOriginAs(Parent), "Parent wasn't the same discussion");

            // If they are the same, we're done
            if (ContentEquals(Theirs))
                return;

            // If we equal the parent, but they don't, then keep theirs
            if (ContentEquals(Parent) && !Theirs.ContentEquals(Parent))
            {
                CopyParagraphsFrom(Theirs);
                return;
            }

            // If they equal the parent, but we don't, then keep ours
            if (!ContentEquals(Parent) && Theirs.ContentEquals(Parent))
                return;

            // If we are here, then we have a conflict. We resolve via the Author.
            // If the author is the person on this machine, then we win.
            if (Author == DB.UserName)
                return;
            // If the author is someone else, then they win
            CopyParagraphsFrom(Theirs);
        }
        #endregion
    }
    #endregion

    #region CLASS: TranslatorNote
    public class TranslatorNote : JObject, IComparable<TranslatorNote>
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: string Category 
        public string Category
        {
            get
            {
                return m_sCategory;
            }
            set
            {
                SetValue(ref m_sCategory, value);
            }
        }
        private string m_sCategory;
        #endregion
        #region BAttr{g/s}: string AssignedTo
        public string AssignedTo
        {
            get
            {
                return m_sAssignedTo;
            }
            set
            {
                SetValue(ref m_sAssignedTo, value);
            }
        }
        private string m_sAssignedTo = "";
        #endregion
        #region BAttr{g/s}: string Context
        public string Context
        {
            get
            {
                return m_sContext;
            }
            set
            {
                SetValue(ref m_sContext, value);
            }
        }
        private string m_sContext;
        #endregion
        #region BAttr{g/s}: string Reference
        public string Reference
        {
            get
            {
                return m_sReference;
            }
            set
            {
                // We expect the reference of the form "003:016" (leading zeros)
                Debug.Assert(value.Length == 7, "Reference length must be 7");
                Debug.Assert(value[3] == ':', "Reference must have a ':' at position [3]");

                SetValue(ref m_sReference, value);
            }
        }
        private string m_sReference;
        #endregion
        #region BAttr{g/s}: bool ShowInDaughterTranslations
        public bool ShowInDaughterTranslations
        {
            get
            {
                return m_bShowInDaughterTranslations;
            }
            set
            {
                SetValue(ref m_bShowInDaughterTranslations, value);
            }
        }
        bool m_bShowInDaughterTranslations = false;
        #endregion
        #region JAttr{g}: JOwnSeq<Discussion> Discussions
        public JOwnSeq<Discussion> Discussions
        {
            get
            {
                Debug.Assert(null != m_osDiscussions);
                return m_osDiscussions;
            }
        }
        JOwnSeq<Discussion> m_osDiscussions;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Category", ref m_sCategory);
            DefineAttr("AssignedTo", ref m_sAssignedTo);
            DefineAttr("Context", ref m_sContext);
            DefineAttr("Reference", ref m_sReference);
            DefineAttr("ShowInDaughter", ref m_bShowInDaughterTranslations);
        }
        #endregion

        // Classifications -------------------------------------------------------------------
        #region CLASS: Classifications
        public class Classifications : List<Classifications.Classification>
        {
            #region CLASS: Classification
            public class Classification : IComparable<Classification>
            {
                #region Attr{g}: string Name
                public string Name
                {
                    get
                    {
                        Debug.Assert(!string.IsNullOrEmpty(m_sName));
                        return m_sName;
                    }
                }
                string m_sName;
                #endregion

                #region Attr{g}: Classifications Owner
                Classifications Owner
                {
                    get
                    {
                        Debug.Assert(null != m_Owner);
                        return m_Owner;
                    }
                }
                Classifications m_Owner;
                #endregion

                #region Attr{g/s}: bool IsChecked - If T, display it
                public bool IsChecked
                {
                    get
                    {
                        return JW_Registry.GetValue(Owner.RegistrySubKey, Name, true);
                    }
                    set
                    {
                        JW_Registry.SetValue(Owner.RegistrySubKey, Name, value);
                    }
                }
                #endregion

                #region Method: ToolStripMenuItem CreateMenuItem(EventHandler handler)
                public ToolStripMenuItem CreateMenuItem(EventHandler handler)
                {
                    // Create a new menu item
                    ToolStripMenuItem item = new ToolStripMenuItem(Name);

                    // Set the Checked State
                    item.Checked = IsChecked;

                    // Set the tag to identify which category
                    item.Tag = this;

                    // Set the event handler
                    item.Click += handler;

                    // Return the result
                    return item;
                }
                #endregion

                #region Constructor(Owner, sName)
                public Classification(Classifications _Owner, string sName)
                {
                    m_Owner = _Owner;
                    m_sName = sName;
                }
                #endregion
                #region Method: int IComparable<T>.CompareTo(T)
                public int CompareTo(Classification cl)
                {
                    return Name.CompareTo(cl.Name);
                }
                #endregion
            }
            #endregion

            // Attrs -------------------------------------------------------------------------
            #region Attr{g}:string Name
            public string Name
            {
                get
                {
                    Debug.Assert(null != m_sName);
                    return m_sName;
                }
            }
            string m_sName;
            #endregion
            #region Attr{g/s}: BStringArray SettingsSource
            BStringArray SettingsSource
            {
                get
                {
                    Debug.Assert(null != m_bsaSettingsSource);
                    return m_bsaSettingsSource;
                }
                set
                {
                    m_bsaSettingsSource = value;
                    InitFromSettingsSource();
                }
            }
            BStringArray m_bsaSettingsSource;
            #endregion
            #region Attr{g}: string[] FactoryDefaultMembers
            string[] FactoryDefaultMembers
            {
                get
                {
                    return m_vsFactoryDefaultMembers;
                }
            }
            string[] m_vsFactoryDefaultMembers;
            #endregion

            const string c_sDefaultValue = "DefaultValue";
            #region Attr{g/s}: string DefaultValue
            public string DefaultValue
            {
                get
                {
                    string sValue = JW_Registry.GetValue(RegistrySubKey, 
                        c_sDefaultValue, DefaultFactoryValue);
                    return sValue;
                }
                set
                {
                    Debug.Assert(null != FindItem(value));
                    JW_Registry.SetValue(RegistrySubKey, c_sDefaultValue, value);
                }
            }
            #endregion
            #region Attr{g}: string DefaultFactoryValue
            string DefaultFactoryValue
            {
                get
                {
                    return m_sFactoryDefaultValue;
                }
            }
            string m_sFactoryDefaultValue;
            #endregion

            // Configuration Dialog support --------------------------------------------------
            #region Attr{g/s}: string CommaDelimitedString - access from Configuration Dialog
            public string CommaDelimitedString
            {
                get
                {
                    string s = "";

                    // Create a sorted version
                    var v = new List<string>();
                    foreach (string sClassificationName in SettingsSource)
                        v.Add(sClassificationName);
                    v.Sort();

                    // Create the string
                    foreach(string sClassificationName in v)
                        s += (sClassificationName + ", ");

                    return s;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                        return;

                    // Parse the string into its parts
                    string[] vNames = value.Split(new char[] { ',' });

                    // Remove any spaces
                    for (int i = 0; i < vNames.Length; i++)
                        vNames[i] = vNames[i].Trim();

                    // Clear out the list, and the corresponding Settings BSA; we'll
                    // rebuild both as we call AddItem in this and called methods
                    Clear();
                    SettingsSource.Clear();

                    // Clear out the list, then build it from these new values
                    Clear();
                    foreach (string s in vNames)
                        AddItem(s, true);

                    // Scan the book to make sure we haven't deleted anything we need
					if (null != DB.TargetBook && DB.TargetBook.Loaded)
						ScanBookForNewClassifications(DB.TargetBook);

                    // If we still have an empty list, then add the factory defaults
                    // back in.
                    AddFactoryDefaults();

                    // Make sure whatever is our default value is in the list
                    AddItem(DefaultValue, true);
                }
            }
            #endregion

            // Operations --------------------------------------------------------------------
            #region Method: void AddItem(sName, bool bAddToPermanentSettings)
            public void AddItem(string sName, bool bAddToPermanentSettings)
            {
                // Don't add any empty names
                if (string.IsNullOrEmpty(sName))
                    return;

                // Check to see if it is already there
                if (null != FindItem(sName))
                    return;

                // Create and add the new one
                Add(new Classification(this, sName));
                Sort();

                // Add it to the settings if requested; otherwise it is only there
                // because, e.g., the book already had it as a value.
                if (bAddToPermanentSettings)
                {
                    if (!SettingsSource.Contains(sName))
                        SettingsSource.InsertSortedIfUnique(sName);
                }
            }
            #endregion
            #region Method: Classification FindItem(sName)
            public Classification FindItem(string sName)
            {
                foreach (Classification c in this)
                {
                    if (c.Name == sName)
                        return c;
                }
                return null;
            }
            #endregion
            #region Method: bool HasItem(string sName)
            public bool HasItem(string sName)
            {
                return (null != FindItem(sName));
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(sName, bsaSettingsSource, vsFactoryDefaultMembers, sFactoryDefaultValue)
            public Classifications(string sName, BStringArray bsaSettingsSource, 
                string[] vsFactoryDefaultMembers, string sFactoryDefaultValue)
            {
                m_sName = sName;
                m_bsaSettingsSource = bsaSettingsSource;

                // Factory Defaults: Use Localized Versions
                string[] vs = new string[vsFactoryDefaultMembers.Length];
                for (int i = 0; i < vs.Length; i++)
                {
                    vs[i] = Loc.GetNotes("k" + vsFactoryDefaultMembers[i],
                        vsFactoryDefaultMembers[i]);
                }
                m_vsFactoryDefaultMembers = vs;

                // Use localized version of the Default Value, too.
                m_sFactoryDefaultValue = Loc.GetNotes("k" + sFactoryDefaultValue, 
                    sFactoryDefaultValue);

                // Derive a registry key from the note's key and the name
                m_sRegistrySubKey = TranslatorNote.c_sRegistrySubkey + "\\" + sName;

                // Retrieve values from the settings
                InitFromSettingsSource();
            }
            #endregion
            #region Attr{g}: string RegistrySubKey
            public string RegistrySubKey
            {
                get
                {
                    Debug.Assert(null != m_sRegistrySubKey);
                    return m_sRegistrySubKey;
                }
            }
            string m_sRegistrySubKey;
            #endregion
            #region Method: void AddFactoryDefaults()
            void AddFactoryDefaults()
            {
                if (Count == 0 && null != FactoryDefaultMembers)
                {
                    foreach (string s in FactoryDefaultMembers)
                        AddItem(s, true);
                }
            }
            #endregion
            #region Method: void InitFromSettingsSource()
            public void InitFromSettingsSource()
            {
                // Get rid of any existing classifications
                Clear();

                // Populate with the classifications in the settings source
                foreach (string s in SettingsSource)
                    AddItem(s, true);

                // If we're still empty, go with the factory defaults
                AddFactoryDefaults();
            }
            #endregion
        }
        #endregion
        #region SAttr{g}: Classifications Categories
        static public Classifications Categories
        {
            get
            {
                if (null == s_vCategories)
                    InitClassifications();

                Debug.Assert(null != s_vCategories);
                return s_vCategories;
            }
        }
        static Classifications s_vCategories;
        #endregion
        #region SAttr{g}: Classifications FrontCategories
        static public Classifications FrontCategories
        {
            get
            {
                if (null == s_vFrontCategories)
                    InitClassifications();

                Debug.Assert(null != s_vFrontCategories);
                return s_vFrontCategories;
            }
        }
        static Classifications s_vFrontCategories;
        #endregion
        #region SAttr{g}: Classifications People
        static public Classifications People
        {
            get
            {
                if (null == s_vPeople)
                    InitClassifications();

                Debug.Assert(null != s_vPeople);
                return s_vPeople;
            }
        }
        static Classifications s_vPeople;
        #endregion
        #region SMethod: void InitClassifications()
        static public void InitClassifications()
            // Initialize the classifcations, both from their data source, and their
            // factory defaults. This zeros out anything existing, and should thus be
            // called when a new project is loaded.
            //
            // The factory defaults, as passed in, are converted into localized
            // versions by the constructor
        {
            s_vCategories = new Classifications("Categories",
               DB.TeamSettings.NotesCategories,
               new string[] { ToDo, Exegesis },
               ToDo);

            s_vFrontCategories = new Classifications("FrontCategories",
                DB.TeamSettings.NotesFrontCategories,
                new string[] { DraftingHelp, Exegesis },
                Exegesis);

            s_vPeople = new Classifications("People", 
                DB.Project.People,
                new string[] { "Translator", "Advisor", "Consultant", "None" },
                "None");
        }
        #endregion
        #region SMethod: void ScanBookForNewClassifications(DBook book)
        static public void ScanBookForNewClassifications(DBook book)
        {
            if (null == book)
                return;

            bool bIsFront = (book.Translation == DB.FrontTranslation);
            bool bIsTarget = (book.Translation == DB.TargetTranslation);

            // Don't waste time except for Front and Target translations
            if (!bIsFront && !bIsTarget)
                return;

            // Collect all of the book's notes
            List<TranslatorNote> v = new List<TranslatorNote>();
            foreach (DSection section in book.Sections)
                v.AddRange(section.GetAllTranslatorNotes());

            // Add classifications according to their kind
            foreach (TranslatorNote tn in v)
            {
                if (bIsTarget)
                {
                    Categories.AddItem(tn.Category, false);

                    People.AddItem(tn.AssignedTo, false);
                    foreach (Discussion d in tn.Discussions)
                        People.AddItem(d.Author, false);
                }
                else if (tn.ShowInDaughterTranslations)
                    FrontCategories.AddItem(tn.Category, false);
            }
        }
        #endregion

        // Localized Classsifications --------------------------------------------------------
        #region SAttr{g}: string ToDo
        static public string ToDo
        {
            get
            {
                return Loc.GetNotes("kToDo", "To Do");
            }
        }
        #endregion
        #region SAttr{g}: string Exegesis
        static public string Exegesis
        {
            get
            {
                return Loc.GetNotes("kExegesis", "Exegesis");
            }
        }
        #endregion
        #region SAttr{g}: string CategoryOldVersion
        static public string CategoryOldVersion
        {
            get
            {
                return Loc.GetNoteDefs("kOldVersion", "Old Version");
            }
        }
        #endregion
        #region SAttr{g}: string DraftingHelp
        static public string DraftingHelp
        {
            get
            {
                return Loc.GetNotes("kDraftingHelp", "Drafting Help");
            }
        }
        #endregion
        // Other Localizations --------------------------------------------------------------
        #region SAttr{g}: string MergeAuthor
        static public string MergeAuthor
        {
            get
            {
                return Loc.GetNotes("kMergeAuthored", "From Merge");
            }
        }
        #endregion

        // Settings --------------------------------------------------------------------------
        public const string c_sRegistrySubkey = "TranslatorNotes";
        #region Attr{g/s}: Color BorderColor
        static public Color BorderColor
        {
            get
            {
                if (Color.Empty == s_BorderColor)
                {
                    string s = JW_Registry.GetValue(c_sRegistrySubkey, 
                        "BorderColor", "DarkGray");
                    s_BorderColor = Color.FromName(s);
                }
                Debug.Assert(Color.Empty != s_BorderColor);
                return s_BorderColor;
            }
            set
            {
                s_BorderColor = value;
                Debug.Assert(Color.Empty != s_BorderColor);
                JW_Registry.SetValue(c_sRegistrySubkey,
                    "BorderColor", value.Name);
            }
        }
        static Color s_BorderColor = Color.Empty;
        #endregion
        #region Attr{g/s}: Color DiscussionHeaderColor
        static public Color DiscussionHeaderColor
        {
            get
            {
                if (Color.Empty == s_DiscussionHeaderColor)
                {
                    string s = JW_Registry.GetValue(c_sRegistrySubkey, 
                        "DiscussionHeaderColor", "LightGreen");
                    s_DiscussionHeaderColor = Color.FromName(s);
                }
                Debug.Assert(Color.Empty != s_DiscussionHeaderColor);
                return s_DiscussionHeaderColor;
            }
            set
            {
                s_DiscussionHeaderColor = value;
                Debug.Assert(Color.Empty != s_DiscussionHeaderColor);
                JW_Registry.SetValue(c_sRegistrySubkey, 
                    "DiscussionHeaderColor", value.Name);
            }
        }
        static Color s_DiscussionHeaderColor = Color.Empty;
        #endregion
        #region Attr{g/s}: Color UneditableColor
        static public Color UneditableColor
        {
            get
            {
                if (s_UneditableColor == Color.Empty)
                {
                    string s = JW_Registry.GetValue(c_sRegistrySubkey, 
                        "UneditableColor", "Wheat");
                    s_UneditableColor = Color.FromName(s);
                }
                Debug.Assert(Color.Empty != s_UneditableColor);
                return s_UneditableColor;
            }
            set
            {
                s_UneditableColor = value;
                Debug.Assert(Color.Empty != s_UneditableColor);
                JW_Registry.SetValue(c_sRegistrySubkey,
                    "UneditableColor", value.Name);
            }
        }
        static Color s_UneditableColor = Color.Empty;
        #endregion
        #region Attr{g/s}: bool ShowAssignedTo
        static public bool ShowAssignedTo
        {
            get
            {
                return JW_Registry.GetValue(c_sRegistrySubkey,
                        "ShowAssignedTo", false);
            }
            set
            {
                JW_Registry.SetValue(c_sRegistrySubkey,
                    "ShowAssignedTo", value);
            }
        }
        #endregion
        #region Attr{g/s}: bool ShowCategories
        static public bool ShowCategories
        {
            get
            {
                return JW_Registry.GetValue(c_sRegistrySubkey,
                        "ShowCategories", false);
            }
            set
            {
                JW_Registry.SetValue(c_sRegistrySubkey,
                    "ShowCategories", value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public TranslatorNote()
            : base()
        {
            // Discussions, sorted by date
            m_osDiscussions = new JOwnSeq<Discussion>("Discussions", this, false, true);

            // Set the category to the default
            Category = Categories.DefaultValue;
        }
        #endregion
        #region Constructor(sReference, sContext)
        public TranslatorNote(string _sReference, string _sContext)
            : this()
        {
            // Reference, we expect something of the form "003:016", the Set attr 
            // asserts for this
            Reference = _sReference;

            // Context string
            Context = _sContext;
        }
        #endregion
        #region Method: bool ContentEquals(JObject)
        public override bool ContentEquals(JObject obj)
        {
            TranslatorNote tn = obj as TranslatorNote;
            if (null == tn)
                return false;

            if (tn.Category != Category)
                return false;
            if (tn.AssignedTo != AssignedTo)
                return false;
            if (tn.Context != Context)
                return false;
            if (tn.Reference != Reference)
                return false;

            if (Discussions.Count != tn.Discussions.Count)
                return false;

            for (int i = 0; i < Discussions.Count; i++)
            {
                if (!Discussions[i].ContentEquals(tn.Discussions[i]))
                    return false;
            }

            return true;
        }
        #endregion
        #region OAttr{g}: string SortKey
        public override string SortKey
        {
            get
            {
                string s = Reference;

                if (Discussions.Count > 0)
                {
                    Discussion discussion = Discussions[0] as Discussion;
                    s += discussion.SortKey;
                }

                return s;
            }
        }
        #endregion
        #region Method: int IComparable<T>.CompareTo(T)
        public int CompareTo(TranslatorNote tn)
        {
            return SortKey.CompareTo(tn.SortKey);
        }
        #endregion
        #region Method: TranslatorNote Clone()
        public TranslatorNote Clone()
        {
            TranslatorNote note = new TranslatorNote();

            note.Category = Category;
            note.AssignedTo = AssignedTo;
            note.Context = Context;
            note.Reference = Reference;
            note.ShowInDaughterTranslations = ShowInDaughterTranslations;

            foreach (Discussion d in Discussions)
                note.Discussions.Append(d.Clone());

            return note;
        }
        #endregion
        #region Attr{g}: DText Text - the owning DText
        public DText Text
        {
            get
            {
                DText text = Owner as DText;
                Debug.Assert(null != text);
                return text;
            }
        }
        #endregion
        #region VAttr{g}: bool IsOwnedInTargetTranslation
        public bool IsOwnedInTargetTranslation
        {
            get
            {
                if (Text.Paragraph.Translation == DB.TargetTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region DEBUG SUPPORT
        public void Debug_VerifyIntegrity()
        {
        #if DEBUG
            Debug.Assert(Discussions.Count > 0);
            foreach (Discussion d in Discussions)
                d.Debug_VerifyIntegrity();
        #endif
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region SAttr{g}: string UnknownAuthor
        static public string UnknownAuthor
        {
            get
            {
                return Loc.GetString("kUnknownAuthor", "Unknown Author");
            }
        }
        #endregion
        #region SMethod: TranslatorNote ImportFromOldStyle(nChapter, nVerse, SfField)
        static public TranslatorNote ImportFromOldStyle(
            int nChapter, int nVerse, SfField field)
        {
            // The Category and ShowInDaughter are derived from the marker
            string sCategory = Loc.GetNoteDefs("kGeneral", "General");
            bool bShowInDaughters = false;
            switch (field.Mkr)
            {
                case "nt":
                    sCategory = Loc.GetNoteDefs("kGeneral", "General"); 
                    break;
                case "ntHint":
                    sCategory = Loc.GetNoteDefs("kHintForDaughter", 
                        "Drafting Help");
                    bShowInDaughters = true;
                    break;
                case "ntck":
                    sCategory = Loc.GetNoteDefs("kToDo", "To Do"); 
                    break;
                case "ntUns":
                    sCategory = Loc.GetNoteDefs("kAskUns", "Ask UNS"); 
                    break;
                case "ntReas":
                    sCategory = Loc.GetNoteDefs("kReason", "Reason"); 
                    break;
                case "ntFT":
                    sCategory = Loc.GetNoteDefs("kFront", "Front Issues"); 
                    break;
                case "ntDef":
                    sCategory = Loc.GetNoteDefs("kDefinition", "Definitions"); 
                    break;
                case "ov":
                    sCategory = CategoryOldVersion; 
                    break;
                case "ntBT":
                    sCategory = Loc.GetNoteDefs("kBT", "Back Translation"); 
                    break;
                case "ntgk":
                    sCategory = Loc.GetNoteDefs("kGreek", "Greek");
                    bShowInDaughters = true;
                    break;
                case "nthb":
                    sCategory = Loc.GetNoteDefs("kHebrew", "Hebrew");
                    bShowInDaughters = true;
                    break;
                case "ntcn":
                    sCategory = Loc.GetNoteDefs("kExegesis", "Exegesis");
                    bShowInDaughters = true;
                    break;
                default: // Not an old-style note
                    return null;
            }

            // The author is set to "Unknown Author".
            string sAuthor = UnknownAuthor;

            // The date is set to today
            DateTime dtCreated = DateTime.Now;

            // Create the Reference in the form we expect it
            string sReference = nChapter.ToString("000") + ":" + 
                nVerse.ToString("000");

            // Create the Note. We will not have a context for it.
            TranslatorNote tn = new TranslatorNote(sReference, "");
            tn.Category = sCategory;
            tn.ShowInDaughterTranslations = bShowInDaughters;

            // Remove any reference from the data
            string sData = tn.RemoveInitialReferenceFromText(field.Data);

            // Old data (e.g., \ov's) might have footnotes |fn in them. Remove them, as
            // we don't allow notes to have footnotes. (We saw this in Manado Malay data.)
            sData = sData.Replace("|fn", "");

            // If there's not data, we're tossing it. Given that it is old-style
            // anyway, there's no need to keep it around if it had no content
            if (string.IsNullOrEmpty(sData))
                return null;

            // The note needs a context. We'll go with the first five words
            // of the note. Its messy in that it gets repeated when the note is
            // expanded, unless we clear the ShowHeaderWhenExpanded flag.
            tn.Context = GetWordsRight(sData, 0, 5);

            // Add the discussion
            Discussion disc = new Discussion(sAuthor, dtCreated, sData);
            tn.Discussions.Append(disc);          

            // Done
            tn.Debug_VerifyIntegrity();
            return tn;
        }
        #endregion
        #region SMethod: bool IsOldStyleMarker(string sMkr)
        static public bool IsOldStyleMarker(string sMkr)
        {
            string[] vs = { "nt", "ntHint", "ntck", "ntUns", "ntReas", "ntFT", "ntDef", 
                              "ov", "ntBT", "ntgk", "nthb", "ntcn" };

            foreach (string s in vs)
            {
                if (sMkr == s)
                    return true;
            }

            return false;
        }
        #endregion
        #region Method: SfField ToSfField()
        public SfField ToSfField()
            // This allows us to write the note in the old style, which we
            // do for now, until we're ready to convert everyone for good.
            //
            // I do want to delete this at some point, and just convert everyone
            // to the new style. But best not to make too many waves while testing.
        {
            // The old style has exactly one discussion
            if (Discussions.Count != 1)
                return null;

            // The old style has exactly one paragraph
            if (Discussions[0].Paragraphs.Count != 1)
                return null;

            // The old style has exactly one DText
            if (Discussions[0].Paragraphs[0].Runs.Count != 1)
                return null;
            if (Discussions[0].Paragraphs[0].Runs[0] as DText == null)
                return null;

            // The old style's author is "Unknown Author"
            string sAuthor = Loc.GetString("kUnknownAuthor", "Unknown Author");
            if (Discussions[0].Author != sAuthor)
                return null;

            // Figure out the marker from the category. This is ugly, but it IS going away.
            string sMkr = "nt";
            if (Category == Loc.GetNoteDefs("kHintForDaughter", "Drafting Help"))
                sMkr = "ntHint";
            else if (Category == Loc.GetNoteDefs("kToDo", "To Do"))
                sMkr = "ntck";
            else if (Category == Loc.GetNoteDefs("kAskUns", "Ask UNS"))
                sMkr = "ntUns";
            else if (Category == Loc.GetNoteDefs("kReason", "Reason"))
                sMkr = "ntReas";
            else if (Category == Loc.GetNoteDefs("kFront", "Front Issues"))
                sMkr = "ntFT";
            else if (Category == Loc.GetNoteDefs("kDefinition", "Definitions"))
                sMkr = "ntDef";
            else if (Category == Loc.GetNoteDefs("kOldVersion", "Old Version"))
                sMkr = "ov";
            else if (Category == Loc.GetNoteDefs("kBT", "Back Translation"))
                sMkr = "ntBT";
            else if (Category == Loc.GetNoteDefs("kGreek", "Greek"))
                sMkr = "ntgk";
            else if (Category == Loc.GetNoteDefs("kHebrew", "Hebrew"))
                sMkr = "nthb";
            else if (Category == Loc.GetNoteDefs("kExegesis", "Exegesis"))
                sMkr = "ntcn";

            // Create the note
            return new SfField(sMkr, Discussions[0].Paragraphs[0].Runs[0].ContentsSfmSaveString);
        }
        #endregion
        #region Method: void AddToSfmDB(ScriptureDB DB)
        public void AddToSfmDB(ScriptureDB DB)
        {
            SfField f = ToSfField();
            if (null == f)
                f = new SfField(DSFMapping.c_sMkrTranslatorNote, ToXml(true).OneLiner);
            DB.Append(f);
        }
        #endregion
        #region Method: string RemoveInitialReferenceFromText(string sIn)
        public string RemoveInitialReferenceFromText(string sIn)
            // It is redundant in the display, to show the reference there, but to also
            // have it in the note text. So I remove it on import, as we have a lot of
            // legacy notes that have it.
        {
            // Collect any reference that exists in the first paragraph
            string sRef = "";
            int iPos = 0;
            for (; iPos < sIn.Length; iPos++)
            {
                char ch = sIn[iPos];

                if (char.IsDigit(ch) || ch == ':' || char.IsWhiteSpace(ch))
                    sRef += ch;
                else
                    break;
            }

            // Get the reference as we've stored it on the note, but as we plan
            // to display it in the header
            string sFromNote = GetDisplayableReference();

            // Remove leading/trailing spaces and punctuation from both, so we can
            // compare without the confusion
            sFromNote = sFromNote.Trim();
            sRef = sRef.Trim();
            while (sRef.Length > 1 && char.IsPunctuation(sRef[sRef.Length - 1]))
                sRef = sRef.Substring(0, sRef.Length - 1);
            while (sFromNote.Length > 1 && char.IsPunctuation(sFromNote[sFromNote.Length - 1]))
                sFromNote = sFromNote.Substring(0, sFromNote.Length - 1);

            // If it does not equal the note's reference (as we want to display it),
            // then we keep it.
            if (sFromNote != sRef)
                return sIn;

            // Remove what we determined whne we were originally collecting the reference
            sIn = (iPos < sIn.Length) ?
                sIn.Substring(iPos) : "";
            return sIn;
        }
        #endregion

        // Editor Support --------------------------------------------------------------------
        #region VAttr{g}: bool IsEditable
        public bool IsEditable
        {
            get
            {
                // If it is not the Target Translation, then we don't allow edits
                if (!IsOwnedInTargetTranslation)
                    return false;

                return true;
            }
        }
        #endregion
        #region Method: string GetDisplayableReference()
        public string GetDisplayableReference()
            // Strip out the leading zeros
        {
            bool bZeros = true;

            string sOut = "";

            foreach (char ch in Reference)
            {
                // If we encounter a non-zero, then we want to start copying chars
                if (ch != '0')
                    bZeros = false;

                // Copy chars if indicated
                if (!bZeros)
                    sOut += ch;

                // If we have just copyied a ':', then we want to start eating
                // zeros again
                if (ch == ':')
                    bZeros = true;
            }

            return sOut;
        }
        #endregion
        #region Method: DBasicText GetCollapsableHeaderText(sContainingText)
        public DBasicText GetCollapsableHeaderText(string sContainingText)
        {
            // Create a text to put the header into
            DBasicText text = new DBasicText();

            // Start with the reference
            string sBookRef = GetDisplayableReference() + ":" + DPhrase.c_chInsertionSpace;
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, sBookRef));

            // If we don't have a Context, then we're done. We could reproduce the 
            // containing text, but this would eat up the window and is redundant
            // anyway.
            if (string.IsNullOrEmpty(Context))
                return text;

            // Locate the position of the Context within the containing text
            int iPos = -1;
            if (!string.IsNullOrEmpty(sContainingText))
                iPos = sContainingText.IndexOf(Context);

            // If it wasn't found, then just add the Context and we're done
            if (-1 == iPos)
            {
                DPhrase p = new DPhrase(DStyleSheet.c_sfmParagraph, Context);
                text.Phrases.Append(p);
                return text;
            }

            // If w'ere here, then the Context exists within the ContainingText. This
            // is our preferred situation. We want to extract a few words to the left
            // of the Context.
            string sLeft = GetWordsLeft(sContainingText, iPos, 4);
            if (!string.IsNullOrEmpty(sLeft))
            {
                DPhrase p = new DPhrase(DStyleSheet.c_sfmParagraph, sLeft + " ");
                text.Phrases.Append(p);
            }

            // Add the context
            DPhrase pContext = new DPhrase(DStyleSheet.c_StyleAbbrevBold, Context);
            text.Phrases.Append(pContext);

            // Extract a few words to the right
            string sRight = GetWordsRight(sContainingText, iPos + Context.Length, 4);
            if (!string.IsNullOrEmpty(sRight))
            {
                DPhrase p = new DPhrase(DStyleSheet.c_sfmParagraph, " " + sRight);
                text.Phrases.Append(p);
            }

            return text;
        }
        #endregion
        #region SMethod: string GetWordsLeft(string s, int iPos, int cWords)
        static public string GetWordsLeft(string s, int iPos, int cWords)
        {
            int cBlanks = 0;

            string sLeft = "";

            int i = iPos - 1;
            while (i >= 0 && cBlanks <= cWords)
            {
                if (s[i] == ' ')
                    cBlanks++;

                sLeft = s[i] + sLeft;

                i--;
            }

            return sLeft.Trim();
        }
        #endregion
        #region SMethod: string GetWordsRight(string s, int iPos, int cWords)
        static public string GetWordsRight(string s, int iPos, int cWords)
        {
            int cBlanks = 0;

            string sRight = "";

            int i = iPos;
            while (i < s.Length && cBlanks <= cWords)
            {
                if (s[i] == ' ')
                    cBlanks++;

                sRight += s[i];

                i++;
            }

            return sRight.Trim();
        }
        #endregion
        #region SAttr{g/s}: bool ShowAllPeople - vs "Only show the notes assigned to me"
        static public bool ShowAllPeople
        {
            get
            {
                return s_bShowAllPeople;
            }
            set
            {
                s_bShowAllPeople = value;
            }
        }
        static bool s_bShowAllPeople = true;
        #endregion
        #region VAttr{g}: bool IsUnassigned
        public bool IsUnassigned
        {
            get
            {
                return string.IsNullOrEmpty(AssignedTo);
            }
        }
        #endregion

        // View Construction -----------------------------------------------------------------
        #region VAttr{g}: Discussion FirstDiscussion
        public Discussion FirstDiscussion
        {
            get
            {
                Debug.Assert(0 != Discussions.Count);
                return Discussions[0];
            }
        }
        #endregion
        #region VAttr{g}: Discussion LastDiscussion
        public Discussion LastDiscussion
        {
            get
            {
                Debug.Assert(0 != Discussions.Count);
                return Discussions[Discussions.Count - 1];
            }
        }
        #endregion
        #region VAttr{g}: DParagraph FirstParagraph - first paragraph in the first discussion obj
        public DParagraph FirstParagraph
        {
            get
            {
                Debug.Assert(0 < Discussions.Count);
                return Discussions[0].FirstParagraph;
            }
        }
        #endregion
        #region VAttr{g}: DParagraph LastParagraph - last paragraph in the last discussion obj
        public DParagraph LastParagraph
        {
            get
            {
                DParagraph p = LastDiscussion.LastParagraph;
                Debug.Assert(null != p);
                return p;
            }
        }
        #endregion
        #region VAttr{g}: bool IsShown()
        public bool IsShown()
        {
            // Is this an AssignedTo person we want to see?
            if (!ShowAllPeople)
            {
                // Is it assigned to this user?
                bool bIsAsignedToThisUser = (AssignedTo == DB.UserName);

                // Was it recently created by this user? (The issue is that if the user
                // has it "Show stuff assigned to me", and they then create the note,
                // because the note is assigned to no one, it would not show up. So we
                // give it an hour for the user to assign it to someone.
                bool bWasJustCreatedByThisUser =
                    (Discussions.Count == 1 &&
                    FirstDiscussion.Author == DB.UserName &&
                    FirstDiscussion.Created > DateTime.Now.AddHours(-1) &&
                    IsUnassigned);

                // Either criteria works
                if (!bIsAsignedToThisUser && !bWasJustCreatedByThisUser)
                    return false;
            }

            // Is this a Front Translation "Hint For Daughter" note?
            if (Text.Paragraph.Translation == DB.FrontTranslation)
            {
                if (!ShowInDaughterTranslations)
                    return false;
                if (!FrontCategories.FindItem(Category).IsChecked)
                    return false;
                
                return true;
            }

            // Is this a category we want to see?
            Classifications.Classification cl = Categories.FindItem(Category);
            if (null == cl || !cl.IsChecked)
                return false;

            return true;
        }
        #endregion
        #region SMethod: Bitmap GetBitmap(clrBackground)
        static public Bitmap GetBitmap(Color clrBackground)
        {
            // Retrieve the bitmap from resources
            Bitmap bmp = JWU.GetBitmap("NoteGeneric.ico");

            // Set its transparent color to the background color. We assume that the
            // pixel at 0,0 is a background pixel.
            Color clrTransparent = bmp.GetPixel(0, 0);
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++)
                {
                    if (bmp.GetPixel(w, h) == clrTransparent)
                        bmp.SetPixel(w, h, clrBackground);
                }
            }

            Debug.Assert(null != bmp);
            return bmp;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: bool IsSameOriginAs(Theirs)
        public bool IsSameOriginAs(TranslatorNote Theirs)
            // Two Notes started out the same IFF the Author and Created of the first discussion
            // are the same.
        {
            Debug.Assert(Discussions.Count > 0);
            Debug.Assert(Theirs.Discussions.Count > 0);

            return Discussions[0].IsSameOriginAs(Theirs.Discussions[0]);
        }
        #endregion
        #region Method: Discussion FindInParent(TranslatorNote Parent, Discussion)
        Discussion FindInParent(TranslatorNote Parent, Discussion d)
        {
            foreach (Discussion discParent in Parent.Discussions)
            {
                if (discParent.IsSameOriginAs(d))
                    return discParent;
            }
            return null;
        }
        #endregion
        #region Method: void Merge(TranslatorNote Parent, TranslatorNote Theirs)
        public void Merge(TranslatorNote Parent, TranslatorNote Theirs)
        {
            // Find out who had the most recent discussion. They will be the
            // winner for the basic attrs. (Thus if it was them, copy the values
            // over; otherwise by default we just keep ours.)
            if (Theirs.LastDiscussion.Created.CompareTo(LastDiscussion.Created) > 1)
            {
                Category = Theirs.Category;
                AssignedTo = Theirs.AssignedTo;
                Context = Theirs.Context;
                Reference = Theirs.Reference;
                ShowInDaughterTranslations = Theirs.ShowInDaughterTranslations;
            }

            // Go through their discussions. Anything that we have, we merge; everything
            // else, we add. Thus the result is a superset of all Discussion objects.
            foreach (Discussion discTheirs in Theirs.Discussions)
            {
                // If we have it, then merge the two
                bool bFound = false;
                foreach (Discussion discOurs in Discussions)
                {
                    if (discTheirs.IsSameOriginAs(discOurs))
                    {
                        discOurs.Merge(FindInParent(Parent, discOurs), discTheirs);
                        bFound = true;
                        break;
                    }
                }

                // Otherwise, add theirs to our sequence
                if (!bFound)
                    Discussions.Append( discTheirs.Clone() );
            }
        }
        #endregion
    }
    #endregion
}
