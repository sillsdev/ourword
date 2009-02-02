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

using JWTools;
using JWdb;

using OurWord.Edit;
#endregion

namespace OurWord.DataModel
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

            // The Default author
            Author = DefaultAuthor;

            Debug_VerifyIntegrity();
        }
        #endregion
        #region Constructor(sAuthor, string sSimpleText)
        public Discussion(string sAuthor, DateTime dtCreated, string sSimpleText)
            : this()
        {
            Author = sAuthor;
            Created = dtCreated;

            // Set the text of our one and only paragraph
            Debug.Assert(1 == Paragraphs.Count);
            Paragraphs[0].SimpleText = sSimpleText;

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
                DPhrase phrase = text.Phrases[0];
            }
        #endif
        }
        #endregion

        // Author in Registry ----------------------------------------------------------------
        const string c_sRegistryName = "AuthorName";
        #region SAttr{g/s}: string DefaultAuthor
        static public string DefaultAuthor
        {
            get
            {
                // Start with the name of the computer
                string sMachineName = Environment.MachineName;

                // Override it from the registry
                string sAuthor = JW_Registry.GetValue(
                    TranslatorNote.c_sRegistrySubkey, c_sRegistryName, sMachineName);

                // If we came up empty, then force it back to the machine name
                if (string.IsNullOrEmpty(sAuthor))
                {
                    sAuthor = sMachineName;
                    JW_Registry.SetValue(
                        TranslatorNote.c_sRegistrySubkey, c_sRegistryName, sMachineName);
                }

                return sAuthor;
            }
            set
            {
                // Do nothing if we aren't using a reasonable name
                if (string.IsNullOrEmpty(value))
                    return;

                // Place it in the registry
                JW_Registry.SetValue(
                    TranslatorNote.c_sRegistrySubkey, c_sRegistryName, value);
            }
        }
        #endregion

        // View Construction -----------------------------------------------------------------
        #region VAttr{g}: bool IsEditable
        bool IsEditable
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
                if (Author != DefaultAuthor)
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
        #region Method: EContainer BuildNotesPaneView()
        public EContainer BuildNotesPaneView()
        {
            Debug_VerifyIntegrity();

            int nRoundedCornerInset = 8;

            // We'll put this Discussion into a HeaderColumn. The header will be the
            // author and date, the body will be the paragraphs

            // Define the Header. Insert the left/right margins so that they do not overlap
            // the rounded corners.
            ERowOfColumns eHeader = new ERowOfColumns(2);
            eHeader.Border.Padding.Left = nRoundedCornerInset;
            eHeader.Border.Padding.Right = nRoundedCornerInset;

            OWPara pAuthor = new OWPara(G.TTranslation.WritingSystemVernacular,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteHeader),
                Author);
            eHeader.Append(pAuthor);

            OWPara pDate = new OWPara(G.TTranslation.WritingSystemVernacular,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteDate),
                Created.ToShortDateString());
            eHeader.Append(pDate);

            // Create the main container and define its border
            EHeaderColumn eMainContainer = new EHeaderColumn(eHeader);
            eMainContainer.Border = new EContainer.RoundedBorder(eMainContainer, 12);
            eMainContainer.Border.BorderColor = TranslatorNote.BorderColor;
            eMainContainer.Border.FillColor = TranslatorNote.DiscussionHeaderColor;
            // The contents of the HeaderColumn are inset from the edges
            eMainContainer.Border.Padding.Left = 3;
            eMainContainer.Border.Padding.Right = 2;
            eMainContainer.Border.Padding.Bottom = 2;

            // Color depends on editability
            Color clrBackground = (IsEditable) ? Color.White : TranslatorNote.UneditableColor;

            // Create a Column to hold the discussion paragraphs. Insert the left/right
            // margins so nothing overlaps the rounded corners.
            EColumn eDiscussionHolder = new EColumn();
            eDiscussionHolder.Border = new EContainer.RoundedBorder(eDiscussionHolder, 12);
            eDiscussionHolder.Border.BorderColor = TranslatorNote.BorderColor;
            eDiscussionHolder.Border.FillColor = clrBackground;
            eDiscussionHolder.Border.Padding.Left = nRoundedCornerInset;
            eDiscussionHolder.Border.Padding.Right = nRoundedCornerInset;
            eMainContainer.Append(eDiscussionHolder);

            // Paragraph editing options
            OWPara.Flags options = OWPara.Flags.None;
            if (IsEditable)
                options |= (OWPara.Flags.IsEditable | OWPara.Flags.CanItalic);
            if (OurWordMain.Features.F_StructuralEditing)
                options |= OWPara.Flags.CanRestructureParagraphs;

            // Add the paragraphs to the Discussion Holder
            foreach (DParagraph para in Paragraphs)
            {
                // Create the OWPara and add it to its container
                OWPara pDiscussion = new OWPara(G.TTranslation.WritingSystemVernacular,
                    G.StyleSheet.FindParagraphStyle( para.StyleAbbrev),
                    para, 
                    clrBackground,
                    options
                    );
                if (!IsEditable)
                    pDiscussion.NonEditableBackgroundColor = clrBackground;
                eDiscussionHolder.Append(pDiscussion);
            }

            return eMainContainer;
        }
        #endregion
    }
    #endregion

    public class TranslatorNote : JObject, IComparable<TranslatorNote>
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: string Category - adds new ones at runtime if needed
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
            public class Classification
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

                #region Attr{g/s}: bool IsChecked
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
                    string sValue = JW_Registry.GetValue(RegistrySubKey, c_sDefaultValue, DefaultFactoryValue);
                    Debug.Assert(null != FindItem(sValue));
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

            // Operations --------------------------------------------------------------------
            #region Method: void AddItem(sName)
            public void AddItem(string sName)
            {
                // Don't add any empty names
                if (string.IsNullOrEmpty(sName))
                    return;

                // Check to see if it is already there
                if (null != FindItem(sName))
                    return;

                // Create and add the new one
                Add(new Classification(this, sName));

                // Add it to the settings. 
                if (!SettingsSource.Contains(sName))
                    SettingsSource.Append(sName);
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
                    vs[i] = G.GetLoc_Notes("k" + vsFactoryDefaultMembers[i],
                        vsFactoryDefaultMembers[i]);
                }
                m_vsFactoryDefaultMembers = vs;

                // Use localized version of the Default Value, too.
                m_sFactoryDefaultValue = G.GetLoc_Notes("k" + sFactoryDefaultValue, 
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
            #region Method: void InitFromSettingsSource()
            public void InitFromSettingsSource()
            {
                // Get rid of any existing classifications
                Clear();

                // Populate with the classifications in the settings source
                foreach (string s in SettingsSource)
                    AddItem(s);

                // If we're still empty, go with the factory defaults
                if (Count == 0 && null != FactoryDefaultMembers)
                {
                    foreach (string s in FactoryDefaultMembers)
                        AddItem(s);
                }
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
               G.TeamSettings.NotesCategories,
               new string[] { "To Do", "Exegesis" },
               "To Do");

            s_vFrontCategories = new Classifications("FrontCategories",
                G.TeamSettings.NotesFrontCategories,
                new string[] { "Drafting Help", "Exegesis" },
                "Exegesis");

            s_vPeople = new Classifications("People", 
                G.Project.People,
                new string[] { "Translator", "Advisor", "Consultant", "None" },
                "None");
        }
        #endregion
        #region SMethod: void ScanBookForNewClassifications(DBook book)
        static public void ScanBookForNewClassifications(DBook book)
        {
            bool bIsFront = (book.Translation == G.FTranslation);
            bool bIsTarget = (book.Translation == G.TTranslation);

            // Don'e waste time except for Front and Target translations
            if (!bIsFront && !bIsTarget)
                return;

            // Collect all of the book's notes
            List<TranslatorNote> v = new List<TranslatorNote>();
            foreach (DSection section in book.Sections)
                v.AddRange(section.GetAllTranslatorNotes(false));

            // Add classifications according to their kind
            foreach (TranslatorNote tn in v)
            {
                if (bIsTarget)
                {
                    Categories.AddItem(tn.Category);

                    People.AddItem(tn.AssignedTo);
                    foreach (Discussion d in tn.Discussions)
                        People.AddItem(d.Author);
                }
                else if (tn.ShowInDaughterTranslations)
                    FrontCategories.AddItem(tn.Category);
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
                if (Text.Paragraph.Translation == G.TTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region DEBUG SUPPORT
        public void Debug_VerifyIntegrity()
        {
        #if DEBUG
            foreach (Discussion d in Discussions)
                d.Debug_VerifyIntegrity();
        #endif
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region SAttr{g}: string UnknownAuthor
        static string UnknownAuthor
        {
            get
            {
                return G.GetLoc_String("kUnknownAuthor", "Unknown Author");
            }
        }
        #endregion
        #region SMethod: TranslatorNote ImportFromOldStyle(nChapter, nVerse, SfField)
        static public TranslatorNote ImportFromOldStyle(
            int nChapter, int nVerse, SfField field)
        {
            // The Category and ShowInDaughter are derived from the marker
            string sCategory = G.GetLoc_NoteDefs("kGeneral", "General");
            bool bShowInDaughters = false;
            switch (field.Mkr)
            {
                case "nt":
                    sCategory = G.GetLoc_NoteDefs("kGeneral", "General"); 
                    break;
                case "ntHint":
                    sCategory = G.GetLoc_NoteDefs("kHintForDaughter", 
                        "Drafting Help");
                    bShowInDaughters = true;
                    break;
                case "ntck":
                    sCategory = G.GetLoc_NoteDefs("kToDo", "To Do"); 
                    break;
                case "ntUns":
                    sCategory = G.GetLoc_NoteDefs("kAskUns", "Ask UNS"); 
                    break;
                case "ntReas":
                    sCategory = G.GetLoc_NoteDefs("kReason", "Reason"); 
                    break;
                case "ntFT":
                    sCategory = G.GetLoc_NoteDefs("kFront", "Front Issues"); 
                    break;
                case "ntDef":
                    sCategory = G.GetLoc_NoteDefs("kDefinition", "Definitions"); 
                    break;
                case "ov":
                    sCategory = G.GetLoc_NoteDefs("kOldVersion", "Old Version"); 
                    break;
                case "ntBT":
                    sCategory = G.GetLoc_NoteDefs("kBT", "Back Translation"); 
                    break;
                case "ntgk":
                    sCategory = G.GetLoc_NoteDefs("kGreek", "Greek");
                    bShowInDaughters = true;
                    break;
                case "nthb":
                    sCategory = G.GetLoc_NoteDefs("kHebrew", "Hebrew");
                    bShowInDaughters = true;
                    break;
                case "ntcn":
                    sCategory = G.GetLoc_NoteDefs("kExegesis", "Exegesis");
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
        #region Method: SfField ToSfm()
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
            string sAuthor = G.GetLoc_String("kUnknownAuthor", "Unknown Author");
            if (Discussions[0].Author != sAuthor)
                return null;

            // Figure out the marker from the category. This is ugly, but it IS going away.
            string sMkr = "nt";
            if (Category == G.GetLoc_NoteDefs("kHintForDaughter", "Drafting Help"))
                sMkr = "ntHint";
            else if (Category == G.GetLoc_NoteDefs("kToDo", "To Do"))
                sMkr = "ntck";
            else if (Category == G.GetLoc_NoteDefs("kAskUns", "Ask UNS"))
                sMkr = "ntUns";
            else if (Category == G.GetLoc_NoteDefs("kReason", "Reason"))
                sMkr = "ntReas";
            else if (Category == G.GetLoc_NoteDefs("kFront", "Front Issues"))
                sMkr = "ntFT";
            else if (Category == G.GetLoc_NoteDefs("kDefinition", "Definitions"))
                sMkr = "ntDef";
            else if (Category == G.GetLoc_NoteDefs("kOldVersion", "Old Version"))
                sMkr = "ov";
            else if (Category == G.GetLoc_NoteDefs("kBT", "Back Translation"))
                sMkr = "ntBT";
            else if (Category == G.GetLoc_NoteDefs("kGreek", "Greek"))
                sMkr = "ntgk";
            else if (Category == G.GetLoc_NoteDefs("kHebrew", "Hebrew"))
                sMkr = "nthb";
            else if (Category == G.GetLoc_NoteDefs("kExegesis", "Exegesis"))
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
                DPhrase p = new DPhrase(DStyleSheet.c_StyleAbbrevNormal, Context);
                text.Phrases.Append(p);
                return text;
            }

            // If w'ere here, then the Context exists within the ContainingText. This
            // is our preferred situation. We want to extract a few words to the left
            // of the Context.
            string sLeft = GetWordsLeft(sContainingText, iPos, 4);
            if (!string.IsNullOrEmpty(sLeft))
            {
                DPhrase p = new DPhrase(DStyleSheet.c_StyleAbbrevNormal, sLeft + " ");
                text.Phrases.Append(p);
            }

            // Add the context
            DPhrase pContext = new DPhrase(DStyleSheet.c_StyleAbbrevBold, Context);
            text.Phrases.Append(pContext);

            // Extract a few words to the right
            string sRight = GetWordsRight(sContainingText, iPos + Context.Length, 4);
            if (!string.IsNullOrEmpty(sRight))
            {
                DPhrase p = new DPhrase(DStyleSheet.c_StyleAbbrevNormal, " " + sRight);
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

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: OnChangeCategory - user has responded to the Category dropdown
        private void OnChangeCategory(object sender, EventArgs e)
        {
            // Retrieve the menu item, and its owning menu
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;
            ToolStripDropDownButton menuCategory = item.OwnerItem as ToolStripDropDownButton;
            if (null == menuCategory)
                return;

            // Check it, and uncheck all the others
            foreach (ToolStripMenuItem i in menuCategory.DropDownItems)
                i.Checked = (i == item);

            // Update the Category attribute
            Category = item.Text;

            // Update the main menu text
            menuCategory.Text = item.Text;
        }
        #endregion
        #region Cmd: OnChangeAssignedTo - user has responded to the AssignedTo dropdown
        private void OnChangeAssignedTo(object sender, EventArgs e)
        {
            // Retrieve the menu item, and its owning menu
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;
            ToolStripDropDownButton menuAssignedTo = item.OwnerItem as ToolStripDropDownButton;
            if (null == menuAssignedTo)
                return;

            // Check it, and uncheck all the others
            foreach (ToolStripMenuItem i in menuAssignedTo.DropDownItems)
                i.Checked = (i == item);

            // Update the Category attribute
            AssignedTo = item.Text;

            // Update the main menu text
            menuAssignedTo.Text = item.Text;
        }
        #endregion
        #region Cmd: OnAddResponse
        private void OnAddResponse(object sender, EventArgs e)
        {
            // Add a new Discussion object
            Discussion d = new Discussion();
            Discussions.Append(d);

            // Recalculate the display
            G.App.ResetWindowContents();

            // Select the new discussion
            NotesWnd w = G.App.SideWindows.NotesPane.WndNotes;
            EContainer container = w.Contents.FindContainerOfDataSource(LastParagraph);
            container.Select_LastWord_End();
            w.Focus();

            Debug_VerifyIntegrity();
        }
        #endregion

        // View Construction -----------------------------------------------------------------
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

        #region Method: BuildAddButton(ToolStrip)
        void BuildAddButton(ToolStrip ts)
        {
            // Is this note editable?
            if (!IsEditable)
                return;

            // Are we allowed to add a discussion? No, if we have the same
            // author and the same date.
            if (LastDiscussion.Author == Discussion.DefaultAuthor &&
                LastDiscussion.Created.Date == DateTime.Today)
                return;

            // If here, we can go ahead and create the Respond button
            string sButtonText = G.GetLoc_Notes("AddResponse", "Respond");
            ToolStripButton btnAddResponse = new ToolStripButton(sButtonText);
            btnAddResponse.Image = JWU.GetBitmap("Note_OldVersions.ico");
            btnAddResponse.Click += new EventHandler(OnAddResponse);
            ts.Items.Add(btnAddResponse);
        }
        #endregion
        #region Method: void BuildCategoryControl(ToolStrip)
        void BuildCategoryControl(ToolStrip ts)
        {
            // Determine if the category is changeable by this user
            // Default to "yes"
            bool bCategoryIsChangeable = true;
            // Can't change it if it is the front translation
            if (!IsOwnedInTargetTranslation)
                bCategoryIsChangeable = false;

            // If editable, then we buld a dropdown control
            if (bCategoryIsChangeable)
            {
                // We need a touch of space between controls
                ts.Items.Add(new ToolStripLabel("  "));

                ToolStripDropDownButton menuCategory = new ToolStripDropDownButton(Category);
                foreach (Classifications.Classification cat in Categories)
                {
                    if (cat.IsChecked)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(cat.Name);
                        item.Click += new EventHandler(OnChangeCategory);
                        if (cat.Name == Category)
                        {
                            item.Checked = true;
                            menuCategory.Text = cat.Name;
                        }
                        menuCategory.DropDownItems.Add(item);
                    }
                }
                ts.Items.Add(menuCategory);
                return;
            }

            // Otherwise, we just display it
            ToolStripLabel labelCategory = new ToolStripLabel(Category);
            ts.Items.Add(labelCategory);
        }
        #endregion
        #region Method: void BuildAssignedToControl(ToolStrip)
        void BuildAssignedToControl(ToolStrip ts)
        {
            // If this is a Front Translation note, we don't want to show anything.
            if (!IsOwnedInTargetTranslation)
                return;

            // We need a touch of space between controls
            ts.Items.Add(new ToolStripLabel("  "));

            // Place the AssignedTo as a drop-down button
            string sMenuName = G.GetLoc_Notes("kAssignTo", "Assign To");
            ToolStripDropDownButton menuAssignedTo = new ToolStripDropDownButton(sMenuName);
            foreach (Classifications.Classification cl in People)
            {
                // Don't let it be assigned to "Unknown Author"
                if (cl.Name == UnknownAuthor)
                    continue;

                // Create the menu item
                ToolStripMenuItem item = new ToolStripMenuItem(cl.Name);
                item.Click += new EventHandler(OnChangeAssignedTo);
                if (cl.Name == AssignedTo)
                {
                    item.Checked = true;
                    menuAssignedTo.Text = cl.Name;
                }
                menuAssignedTo.DropDownItems.Add(item);
            }
            ts.Items.Add(menuAssignedTo);
        }
        #endregion
        #region Method: EToolStrip BuildToolStrip(OWWindow)
        EToolStrip BuildToolStrip(OWWindow wnd)
        {
            // Create the EToolStrip
            EToolStrip toolstrip = new EToolStrip(wnd);
            ToolStrip ts = toolstrip.ToolStrip; // Shorthand

            // Add the "Add" button
            BuildAddButton(ts);

            // Add the Category Control
            BuildCategoryControl(ts);

            // Add the AssignedTo Control
            BuildAssignedToControl(ts);

            return toolstrip;
        }
        #endregion

        #region Method: EContainer BuildNotesPaneView()
        public EContainer BuildNotesPaneView(OWWindow wnd)
            // We place the note in a collapsable container, so the user can hit the 
            // plus/minus icon to see the entire note or not.
        {
            // Create a header paragraph, to show when the note is collapsed
            DBasicText textHeader = GetCollapsableHeaderText("");
            OWPara pHeader = new OWPara(G.TTranslation.WritingSystemVernacular,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteHeader),
                textHeader.Phrases.AsVector);

            // Create the Collapsable Header Column to house the note
            ECollapsableHeaderColumn eHeader = new ECollapsableHeaderColumn(pHeader);
            eHeader.IsCollapsed = false;
            eHeader.ShowHeaderWhenExpanded = true;

            // We want a horizontal line underneath it, to separate notes visually from 
            // each other
            eHeader.Border = new EContainer.SquareBorder(eHeader);
            eHeader.Border.BorderPlacement = EContainer.BorderBase.BorderSides.Bottom;
            eHeader.Border.BorderColor = Color.DarkGray;
            eHeader.Border.BorderWidth = 2;
            eHeader.Border.Margin.Bottom = 5;
            eHeader.Border.Padding.Bottom = 5;

            // Add the Discussions
            foreach (Discussion disc in Discussions)
                eHeader.Append(disc.BuildNotesPaneView());

            // Add the Category and AssignedTo
            if (IsOwnedInTargetTranslation)
            {
                EToolStrip ts = BuildToolStrip(wnd);
                eHeader.Append(ts);
            }

            return eHeader;
        }
        #endregion
        #region VAttr{g}: bool Show
        public bool Show
        {
            get
            {
                // If the Notes Window is not visible, then we don't show notes.
                if (!G.App.HasSideWindows)
                    return false;
                if (!G.App.SideWindows.HasNotesWindow)
                    return false;

                // Is this an AssignedTo person we want to see?
                if (!ShowAllPeople)
                {
                    if (AssignedTo != Discussion.DefaultAuthor)
                        return false;
                }

                // Is this a Front Translation "Hint For Daughter" note?
                DTranslation t = Text.Paragraph.Translation;
                if (t == G.FTranslation)
                {
                    if (!ShowInDaughterTranslations)
                        return false;
                    if (!FrontCategories.FindItem(Category).IsChecked)
                        return false;
                    
                    return true;
                }

                // Is this a category we want to see?
                if (!Categories.FindItem(Category).IsChecked)
                    return false;

                return true;
            }
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
    }



}
