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
    #region Class: DMessage : DParagraph
    public class DMessage : DParagraph
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
        #region BAttr{g/s}: string Status
        public string Status
        {
            get
            {
                if (string.IsNullOrEmpty(m_sStatus))
                    return Closed;

                return m_sStatus;
            }
            set
            {
                string sValue = value;
                if (sValue == Closed)
                    sValue = "";

                SetValue(ref m_sStatus, value);
            }
        }
        private string m_sStatus = "";
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Author", ref m_sAuthor);
            DefineAttr("Created", ref m_dtCreated);
            DefineAttr("Status", ref m_sStatus);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DMessage()
            : base()
        {
            // This paragraph will always have the Message style
            StyleAbbrev = DStyleSheet.c_StyleAnnotationMessage;

            // Start with a simple, empty text
            SimpleText = "";

            // The Default date is "Today"
            m_dtCreated = DateTime.Now;

            // The author
            Author = DB.UserName;

            Debug_VerifyIntegrity();
        }
        #endregion
        #region Constructor(sAuthor, dtCreated, sStatus, string sSimpleText)
        public DMessage(string sAuthor, DateTime dtCreated, string sStatus, string sSimpleText)
            : this()
        {
            Author = sAuthor;
            Created = dtCreated;
            Status = sStatus;

            // Temporary kludge: remove ||'s that we've  been inserting by mistake
            string sFixed = DSection.IO.EatSpuriousVerticleBars(sSimpleText);

            // Parse the string into phrases and add them
            Runs.Clear();
            List<DRun> vRuns = DSection.IO.CreateDRunsFromInputText(sFixed);
            foreach (DRun run in vRuns)
            {
                DText text = run as DText;
                if (text != null && text.PhrasesBT.Count == 0)
                    text.PhrasesBT.Append(new DPhrase(DStyleSheet.c_sfmParagraph, ""));
                Runs.Append(run);
            }

            Debug_VerifyIntegrity();
        }
        #endregion
        #region OMethod: bool ContentEquals(DMessage)
 		public override bool ContentEquals(JObject obj)
        {
            var message = obj as DMessage;
            if (null == message)
                return false;

            if (message.Author != Author)
                return false;
            if (message.Created.CompareTo(Created) != 0)
                return false;
            if (message.Status != Status)
                return false;

            if (!base.ContentEquals(message))
                return false;

            return true;
        }
        #endregion
        #region Method: DMessage Clone()
        public DMessage Clone()
        {
            var message = new DMessage();
            message.Author = Author;
            message.Created = Created;
            message.Status = Status;
            message.CopyFrom(this, false);
            return message;
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
                var tn = Owner as TranslatorNote;
                Debug.Assert(null != tn);
                return tn;
            }
        }
        #endregion
        #region DEBUG SUPPORT
        #region Method: void Debug_VerifyIntegrity()
        public void Debug_VerifyIntegrity()
        {
        #if DEBUG
            // Make sure we have a DText in every paragraph
            Debug.Assert(Runs.Count > 0);
            DText text = null;
            foreach (DRun r in Runs)
            {
                text = r as DText;
                if (null != text)
                    break;
            }
            Debug.Assert(null != text);

            // Make sure we have a phrase in the DText
            Debug.Assert(text.Phrases.Count > 0);
        #endif
        }
        #endregion
        #region Attr{g}: string DebugString
        public override string DebugString
        {
            get
            {
                string s = "M: "+
                    "Author={" + Author + "} " +
                    "Created={" + Created.ToShortDateString() + "} " +
                    "Status={" + Status + "} " +
                    "Content={" + base.DebugString + "}";
                return s;
            }
        }
        #endregion
        #endregion

        // Localized Status Values -----------------------------------------------------------
        #region SAttr{g}: string Anyone
        static public string Anyone
        {
            get
            {
                return Loc.GetNotes("kAnyone", "Anyone");
            }
        }
        #endregion
        #region SAttr{g}: string Closed
        static public string Closed
        {
            get
            {
                return Loc.GetNotes("kClosed", "Closed");
            }
        }
        #endregion

        // View Construction -----------------------------------------------------------------
        #region VAttr{g}: bool IsEditable
        public bool IsEditable
        {
            get
            {
                // If the owning Annotation isn't editable, then this Message isn't.
                if (!Note.IsEditable)
                    return false;

                // If not the last one in the Annotation, it isn't
                if (this != Note.LastMessage)
                    return false;

                // If the author is someone different from me, it isn't
                if (Author != DB.UserName)
                    return false;

                return true;
            }
        }
        #endregion

        // Oxes I/O --------------------------------------------------------------------------
        #region Constants
        public const string c_sTagMessage = "Message";
        const string c_sAttrAuthor = "author";
        const string c_sAttrCreated = "created";
        const string c_sAttrStatus = "status";
        #endregion
        #region Method: bool ImportOldToolboxXmlParagraphContents(XmlNode nodeMessage)
        bool ImportOldToolboxXmlParagraphContents(XmlNode nodeMessage)
        {
            // Our clue it is old format is if we have an "ownseq" child node
            var nodeOwnSeq = XmlDoc.FindNode(nodeMessage, "ownseq");
            if (null == nodeOwnSeq)
                return false;

            // Retrieve the paragraph node
            var nodeParagraph = XmlDoc.FindNode(nodeOwnSeq, "DParagraph");
            if (null == nodeParagraph)
                return false;

            // If the paragraph has an ownseq node, then we have something more
            // complicated going on. I'm hoping there's none in the data. If there
            // is, then I'll need to have the JObject's FromXml code execute here.
            var nodeUhOh = XmlDoc.FindNode(nodeParagraph, "ownseq");
            Debug.Assert(null == nodeUhOh);

            // The text is the contents attr of that node
            SimpleText = XmlDoc.GetAttrValue(nodeParagraph, "Contents", "");

            return true;
        }
        #endregion
        #region SMethod: DMessage Create(nodeMessage)
        static public DMessage Create(XmlNode nodeMessage)
        {
            // Is it a Message node? (Old-style were called Discussion)
            if (!XmlDoc.IsNode(nodeMessage, c_sTagMessage) &&
                !XmlDoc.IsNode(nodeMessage, "Discussion"))
            {
                return null;
            }

            // Create the new Message object. 
            var message = new DMessage();

            // Attrs
            message.Author = XmlDoc.GetAttrValue(nodeMessage, c_sAttrAuthor, "");
            message.Created = XmlDoc.GetAttrValue(nodeMessage, c_sAttrCreated, DateTime.Now);
            message.Status = XmlDoc.GetAttrValue(nodeMessage, c_sAttrStatus, "");

            // Import old-style paragraph contents; if successful, we're done.
            if (message.ImportOldToolboxXmlParagraphContents(nodeMessage))
                return message;

            // Paragraph contents
            message.ReadOxes(nodeMessage);

            return message;
        }
        #endregion
        #region Method: XmlNode Save(oxes, nodeAnnotation)
        public XmlNode Save(XmlDoc oxes, XmlNode nodeAnnotation)
        {
            var nodeMessage = oxes.AddNode(nodeAnnotation, c_sTagMessage);

            // Attrs
            oxes.AddAttr(nodeMessage, c_sAttrAuthor, Author);
            oxes.AddAttr(nodeMessage, c_sAttrCreated, Created);

            // An empty Status is interpreted as "closed", so we want to not
            // att the status attr unless we have content.
            if (!string.IsNullOrEmpty(Status) && Status != Closed)
                oxes.AddAttr(nodeMessage, c_sAttrStatus, Status);

            // Paragraph contents
            foreach (DRun run in Runs)
                run.SaveToOxesBook(oxes, nodeMessage);

            return nodeMessage;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: bool IsSameOriginAs(Theirs)
        public bool IsSameOriginAs(DMessage Theirs)
            // Two messages started out the same if they have the same Author and 
            // Create date.
        {
            if (0 != Created.CompareTo(Theirs.Created))
                return false;
            if (Author != Theirs.Author)
                return false;
            return true;
        }
        #endregion
        #region Method: void Merge(Parent, Theirs)
        public void Merge(DMessage Parent, DMessage Theirs)
        {
            // The caller needs to make sure these are the same Author and Created
            Debug.Assert(IsSameOriginAs(Theirs), "Theirs wasn't the same message");
            Debug.Assert(IsSameOriginAs(Parent), "Parent wasn't the same message");

            // If they are the same, we're done
            if (ContentEquals(Theirs))
                return;

            // If we equal the parent, but they don't, then keep theirs
            if (ContentEquals(Parent) && !Theirs.ContentEquals(Parent))
            {
                Status = Theirs.Status;
                CopyFrom(Theirs, false);
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
            Status = Theirs.Status;
            CopyFrom(Theirs, false);
            Status = Theirs.Status;
        }
        #endregion
    }
    #endregion

    #region CLASS: TranslatorNote
    public class TranslatorNote : JObject, IComparable<TranslatorNote>
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: string Class
        public string Class
        {
            get
            {
                return m_sClass;
            }
            set
            {
                SetValue(ref m_sClass, value);
            }
        }
        private string m_sClass;
        #endregion
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
        #region JAttr{g}: JOwnSeq<DMessage> Messages
        public JOwnSeq<DMessage> Messages
        {
            get
            {
                Debug.Assert(null != m_osMessages);
                return m_osMessages;
            }
        }
        JOwnSeq<DMessage> m_osMessages;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Class", ref m_sClass);
            DefineAttr("Category", ref m_sCategory);
            DefineAttr("Context", ref m_sContext);
            DefineAttr("Reference", ref m_sReference);
            DefineAttr("ShowInDaughter", ref m_bShowInDaughterTranslations);
        }
        #endregion

        // Classes ---------------------------------------------------------------------------
        const string c_sClassGeneral = "General";
        const string c_sClassExegetical = "Exegetical";
        const string c_sClassHintForDrafting = "HintForDrafting";
        #region VAttr{g}: bool IsGeneralNote
        public bool IsGeneralNote
        {
            get
            {
                return (Class == c_sClassGeneral);
            }
        }
        #endregion
        #region VAttr{g}: bool IsExegeticalNote
        public bool IsExegeticalNote
        {
            get
            {
                return (Class == c_sClassExegetical);
            }
        }
        #endregion
        #region VAttr{g}: bool IsHintForDraftingNote
        public bool IsHintForDraftingNote
        {
            get
            {
                return (Class == c_sClassHintForDrafting);
            }
        }
        #endregion

        // Virtual Attrs ---------------------------------------------------------------------
        #region VAttr{g/s}: string Status
        public string Status
        {
            get
            {
                return LastMessage.Status;
            }
            set
            {
                LastMessage.Status = value;
            }
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
                    Categories.AddItem(tn.Category, false);
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
        #region Attr{g/s}: Color MessageHeaderColor
        static public Color MessageHeaderColor
        {
            get
            {
                if (Color.Empty == s_MessageHeaderColor)
                {
                    string s = JW_Registry.GetValue(c_sRegistrySubkey, 
                        "MessageHeaderColor", "LightGreen");
                    s_MessageHeaderColor = Color.FromName(s);
                }
                Debug.Assert(Color.Empty != s_MessageHeaderColor);
                return s_MessageHeaderColor;
            }
            set
            {
                s_MessageHeaderColor = value;
                Debug.Assert(Color.Empty != s_MessageHeaderColor);
                JW_Registry.SetValue(c_sRegistrySubkey,
                    "MessageHeaderColor", value.Name);
            }
        }
        static Color s_MessageHeaderColor = Color.Empty;
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
            // Default is General class
            m_sClass = c_sClassGeneral;

            // Messages, sorted by date
            m_osMessages = new JOwnSeq<DMessage>("Messages", this, false, true);

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
            if (tn.Context != Context)
                return false;
            if (tn.Reference != Reference)
                return false;

            if (Messages.Count != tn.Messages.Count)
                return false;

            for (int i = 0; i < Messages.Count; i++)
            {
                if (!Messages[i].ContentEquals(tn.Messages[i]))
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

                if (Messages.Count > 0)
                    s += Messages[0].SortKey;

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
            note.Context = Context;
            note.Reference = Reference;
            note.ShowInDaughterTranslations = ShowInDaughterTranslations;

            foreach (DMessage m in Messages)
                note.Messages.Append(m.Clone());

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
            Debug.Assert(Messages.Count > 0);
            foreach (DMessage m in Messages)
                m.Debug_VerifyIntegrity();
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

            // Add the Message
            var message = new DMessage(sAuthor, dtCreated, "", sData);
            tn.Messages.Append(message);          

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
            // The old style has exactly one message
            if (Messages.Count != 1)
                return null;

            // The old style has exactly one DText
            if (Messages[0].Runs.Count != 1)
                return null;
            if (Messages[0].Runs[0] as DText == null)
                return null;

            // The old style's author is "Unknown Author"
            string sAuthor = Loc.GetString("kUnknownAuthor", "Unknown Author");
            if (Messages[0].Author != sAuthor)
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
            return new SfField(sMkr, Messages[0].Runs[0].ContentsSfmSaveString);
        }
        #endregion
        #region Method: void AddToSfmDB(ScriptureDB DB)
        public void AddToSfmDB(ScriptureDB DB)
        {
            // Attempt to write to the old style. Fails if the note is too complicated.
            SfField f = ToSfField();

            // Failed. Must do a new style
            if (null == f)
            {
                var oxes = new XmlDoc();
                var node = Save(oxes, oxes);
                f = new SfField(DSFMapping.c_sMkrTranslatorNote, XmlDoc.OneLiner(node));
            }

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

        // Oxes I/O --------------------------------------------------------------------------
        #region Constants
        public const string c_sTagTranslatorNote = "TranslatorNote";
        const string c_sAttrClass = "class";
        const string c_sAttrCategory = "category";
        const string c_sAttrAssignedTo = "assignedTo";
        const string c_sAttrContext = "context";
        const string c_sAttrReference = "reference";
        const string c_sAttrShowInDaughter = "showInDaughter";
        #endregion
        #region SMethod: TranslatorNote Create(nodeNote)
        static public TranslatorNote Create(XmlNode nodeNote)
        {
            if (nodeNote.Name != c_sTagTranslatorNote)
                return null;

            // Create the new note
            var note = new TranslatorNote();

            // Attrs
            note.Class = XmlDoc.GetAttrValue(nodeNote, c_sAttrClass, c_sClassGeneral);
            note.Category = XmlDoc.GetAttrValue(nodeNote, c_sAttrCategory, "");
            note.Context = XmlDoc.GetAttrValue(nodeNote, c_sAttrContext, "");
            note.Reference = XmlDoc.GetAttrValue(nodeNote, c_sAttrReference, "");
            note.ShowInDaughterTranslations = XmlDoc.GetAttrValue(nodeNote, c_sAttrShowInDaughter, false);

            // Toolbox old style had an AssignedTo attribute. We want that to be the
            // Status of the LastMessage.
            string sAssignedTo = XmlDoc.GetAttrValue(nodeNote, c_sAttrAssignedTo, "");

            // Toolbox old style had a <ownseq node above the messages; so if we see this,
            // we simply move down to it prior to iterating through the Message nodes.
            var nodeParent = XmlDoc.FindNode(nodeNote, "ownseq");
            if (null == nodeParent)
                nodeParent = nodeNote;

            // Messages
            foreach (XmlNode child in nodeParent.ChildNodes)
            {
                var message = DMessage.Create(child);
                if (null != message)
                    note.Messages.Append(message);
            }

            // Handle old-style AssignedTo
            if (!string.IsNullOrEmpty(sAssignedTo))
                note.LastMessage.Status = sAssignedTo;

            return note;
        }
        #endregion
        #region Method: XmlNode Save(oxes, nodeAnnotation)
        public XmlNode Save(XmlDoc oxes, XmlNode nodeParent)
        {
            var nodeNote = oxes.AddNode(nodeParent, c_sTagTranslatorNote);

            // Attrs
            oxes.AddAttr(nodeNote, c_sAttrClass, Class);
            oxes.AddAttr(nodeNote, c_sAttrCategory, Category);
            oxes.AddAttr(nodeNote, c_sAttrContext, Context);
            oxes.AddAttr(nodeNote, c_sAttrReference, Reference);
            oxes.AddAttr(nodeNote, c_sAttrShowInDaughter, ShowInDaughterTranslations);

            // Message objects
            foreach (DMessage m in Messages)
                m.Save(oxes, nodeNote);

            return nodeNote;
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

        /***
        #region VAttr{g}: bool IsUnassigned
        public bool IsUnassigned
        {
            get
            {
                return string.IsNullOrEmpty(AssignedTo);
            }
        }
        #endregion
        ***/

        // View Construction -----------------------------------------------------------------
        #region VAttr{g}: Message FirstMessage
        public DMessage FirstMessage
        {
            get
            {
                Debug.Assert(0 != Messages.Count);
                return Messages[0];
            }
        }
        #endregion
        #region VAttr{g}: Message LastMessage
        public DMessage LastMessage
        {
            get
            {
                Debug.Assert(0 != Messages.Count);
                return Messages[Messages.Count - 1];
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
                bool bIsAsignedToThisUser = (Status == DB.UserName);

                // Was it recently created by this user? (The issue is that if the user
                // has it "Show stuff assigned to me", and they then create the note,
                // because the note is assigned to no one, it would not show up. So we
                // give it an hour for the user to assign it to someone.
                bool bWasJustCreatedByThisUser =
                    (Messages.Count == 1 &&
                    FirstMessage.Author == DB.UserName &&
                    FirstMessage.Created > DateTime.Now.AddHours(-1) &&
                    Status == DMessage.Anyone);

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
            // Two Notes started out the same IFF the Author and Created of the first message
            // are the same.
        {
            Debug.Assert(Messages.Count > 0);
            Debug.Assert(Theirs.Messages.Count > 0);

            return Messages[0].IsSameOriginAs(Theirs.Messages[0]);
        }
        #endregion
        #region Method: DMessage FindInParent(TranslatorNote Parent, DMessage)
        DMessage FindInParent(TranslatorNote Parent, DMessage message)
        {
            foreach (DMessage m in Parent.Messages)
            {
                if (m.IsSameOriginAs(message))
                    return m;
            }
            return null;
        }
        #endregion
        #region Method: void Merge(TranslatorNote Parent, TranslatorNote Theirs)
        public void Merge(TranslatorNote Parent, TranslatorNote Theirs)
        {
            // Find out who had the most recent message. They will be the
            // winner for the basic attrs. (Thus if it was them, copy the values
            // over; otherwise by default we just keep ours.)
            if (Theirs.LastMessage.Created.CompareTo(LastMessage.Created) > 1)
            {
                Category = Theirs.Category;
                Context = Theirs.Context;
                Reference = Theirs.Reference;
                ShowInDaughterTranslations = Theirs.ShowInDaughterTranslations;
            }

            // Go through their messages. Anything that we have, we merge; everything
            // else, we add. Thus the result is a superset of all Message objects.
            foreach (DMessage msgTheirs in Theirs.Messages)
            {
                // If we have it, then merge the two
                bool bFound = false;
                foreach (DMessage msgOurs in Messages)
                {
                    if (msgTheirs.IsSameOriginAs(msgOurs))
                    {
                        msgOurs.Merge(FindInParent(Parent, msgOurs), msgTheirs);
                        bFound = true;
                        break;
                    }
                }

                // Otherwise, add theirs to our sequence
                if (!bFound)
                    Messages.Append( msgTheirs.Clone() );
            }
        }
        #endregion
    }
    #endregion
}
