#region ***** TranslatorNote.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TranslatorNote.cs
 * Author:  John Wimbish
 * Created: 04 Nov 2008
 * Purpose: Handles a translator's (or consultant's) annotation in Scripture. 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using JWTools;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;
#endregion

// TODO: Do we want a Context for the vernacular, and another for the BT? Thus different things
// would potentially show in each view; or alternatively, if no context we could decide to
// not show the annotation in that particular view? Currently, the Context is not really all that
// meaningful, in that we don't highlight it in the text. But when we do, we'll regret contexts
// that don't work for us.

namespace OurWordData.DataModel.Annotations
{
    public class TranslatorNote : JObject, IComparable<TranslatorNote>
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: string SelectedText
        public string SelectedText
        {
            get
            {
                return m_sSelectedText;
            }
            set
            {
                SetValue(ref m_sSelectedText, value);
            }
        }
        private string m_sSelectedText;
        #endregion
        #region BAttr{g/s}: string Title
        public string Title
            // We only store something if it is different from the SelectedText attribute.
            // Thus the SelectedText is the default title
        {
            get
            {
                if (string.IsNullOrEmpty(m_sTitle))
                    return SelectedText;
                return m_sTitle;
            }
            set
            {
                if (value == SelectedText)
                    value = "";
                SetValue(ref m_sTitle, value);
            }
        }
        private string m_sTitle;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("SelectedText", ref m_sSelectedText);
            DefineAttr("Title", ref m_sTitle);
        }
        #endregion

        // Messages --------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq<DMessage> Messages
        public JOwnSeq<DMessage> Messages
        {
            get
            {
                Debug.Assert(null != m_osMessages);
                return m_osMessages;
            }
        }
        readonly JOwnSeq<DMessage> m_osMessages;
        #endregion
        #region Method: DMessage AddMessage(utcDate, sStage, sDescription)
        public DMessage AddMessage(DateTime utcDate, Stage stage, string sDescription)
        {
            DMessage m = null;

            // History notes take a subclass of DMessage
            if (Behavior == History)
                m = new DEventMessage(utcDate, stage, sDescription);
            else
            {
                m = new DMessage();
                m.UtcCreated = utcDate;
                m.SimpleText = sDescription;
            }

            return AddMessage(m);
        }
        #endregion
        #region Method: DMessage AddMessage(DMessage)
        public DMessage AddMessage(DMessage m)
        {
            // Enforce that History notes have their own special message type
            if (Behavior == History && null == m as DEventMessage)
            {
                Debug.Assert(false, "Attempt to add a DMessage to a History note");
                return null;
            }

            // If already there, just update it's text; but don't add a duplicate
            int i = Messages.Find(m.SortKey);
            if (i != -1)
            {
                Messages[i].CopyFrom(m, false);
                return Messages[i];
            }

            Messages.Append(m);
            return m;
        }
        #endregion
        #region Method: void RemoveMessage(DMessage m)
        public void RemoveMessage(DMessage m)
        {
            Messages.Remove(m);
        }
        #endregion
        #region Attr{g}: bool HasMessages
        public bool HasMessages
        {
            get
            {
                return (Messages.Count > 0);
            }
        }
        #endregion
        #region Method: void RemoveEmptyMessages()
        public void RemoveEmptyMessages()
        {
            for(var i=0; i<Messages.Count;)
            {
                if (Messages[i].IsCompletelyEmpty)
                    RemoveMessage(Messages[i]);
                else
                    i++;
            }
        }
        #endregion

        // Classes ---------------------------------------------------------------------------
        #region Class: Properties
        public class Properties
        {
            // Attrs
            #region Attr{g}: string Name
            public string Name
            {
                get
                {
                    return m_sName;
                }
            }
            readonly string m_sName;
            #endregion

            // Writing System
            #region Attr{g}: bool ConsultantWritingSystem
            public bool ConsultantWritingSystem { private get; set; }
            #endregion
            #region Method: WritingSystem GetWritingSystem(TranslatorNote)
            public WritingSystem GetWritingSystem(TranslatorNote note)
            {
                Debug.Assert(null != note.OwningBook);
                var translation = note.OwningBook.Translation;
                Debug.Assert(null != translation);
                if (ConsultantWritingSystem)
                    return translation.WritingSystemConsultant;
                return translation.WritingSystemVernacular;
            }
            #endregion

            // Title
            #region SMethod: string GetLocTitle(sId, sEnglishDefault)
            static public string GetLocTitle(string sId, string sEnglishDefault)
            {
                return Loc.GetString(sId, sEnglishDefault);

            }
            #endregion
            public string Title { get; set; }
            #region Attr{g}: bool HasTitle
            public bool HasTitle
            {
                get
                {
                    return !string.IsNullOrEmpty(Title);
                }
            }
            #endregion

            // Scaffolding
            #region Constructor(sName)
            public Properties(string sName)
            {
                m_sName = sName;

                if (null == s_vProperties)
                    s_vProperties = new List<Properties>();
                s_vProperties.Add(this);
            }
            #endregion

            // List of all of our (static) properties
            static List<Properties> s_vProperties;
            #region Method: Properties Find(sClass)
            static public Properties Find(string sClass)
            {
                foreach (var prop in s_vProperties)
                {
                    if (prop.Name == sClass)
                        return prop;
                }
                Debug.Assert(false, "Property not found: " + sClass);
                return TranslatorNote.General;
            }
            #endregion
        }
        #endregion
        #region Attr{g/s}: Properties Behavior
        public Properties Behavior
        {
            get
            {
                return m_Behavior;
            }
            set
            {
                Debug.Assert(null != value);
                m_Behavior = value;
            }
        }
        Properties m_Behavior = General;
        #endregion
        #region Definitions: General, History

        // Conversation / dialog annotations
        public static readonly Properties General = new Properties("General") 
        {
        };

        // Support of the History dialog
        public static readonly Properties History = new Properties("History")
        {
            Title = Properties.GetLocTitle("kHistory", "History")
        };

        #endregion

        // Virtual Attrs ---------------------------------------------------------------------
        #region VAttr{g/s}: Role Status
        public Role Status
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
        #region VAttr{g}: DBook OwningBook
        public DBook OwningBook
        {
            get
            {
                JObject obj = this;
                while (null != obj.Owner && null == obj as DBook)
                    obj = obj.Owner;
                return obj as DBook;
            }
        }
        #endregion
        #region VAttr{g}: bool IsTargetTranslationNote
        public bool IsTargetTranslationNote
        {
            get
            {
                var book = OwningBook;
                if (null == book)
                    return false;

                var translation = book.Translation;
                if (null != translation && translation == DB.TargetTranslation)
                    return true;

                return false;
            }
        }
        #endregion
        #region VAttr{g}: bool IsFrontTranslationNote
        public bool IsFrontTranslationNote
        {
            get
            {
                var book = OwningBook;
                if (null == book)
                    return false;

                var translation = book.Translation;
                if (null != translation && translation == DB.FrontTranslation)
                    return true;

                return false;
            }
        }
        #endregion

        // Scripture Reference containing the TranslatorNote ---------------------------------
        #region VAttr{g}: DReference ChapterVerse
        public DReference ChapterVerse
        {
            get
            {
                // Get the owning text and paragraph 
                var text = OwningTextOrNull;
                if (null == Owner || null == text || null == text.Paragraph)
                    return null;
                var paragraph = text.Paragraph;
                
                // We'll start with the initial chapter/verse of the paragraph
                var nChapter = paragraph.ReferenceSpan.Start.Chapter;
                var nVerse = paragraph.ReferenceSpan.Start.Verse;

                // Now loop through the runs, looking for our Text, incrementing 
                // chapter and verse as we encounter them.
                foreach (DRun run in paragraph.Runs)
                {
                    var verse = run as DVerse;
                    if (null != verse)
                        nVerse = verse.VerseNo;

                    var chapter = run as DChapter;
                    if (null != chapter)
                    {
                        nChapter = chapter.ChapterNo;
                        nVerse = 1;
                    }

                    if (run == text)
                        break;
                }

                return new DReference(nChapter, nVerse);
            }
        }
        #endregion
        #region VAttr{g}: int Chapter
        public int Chapter
        {
            get
            {
                var reference = ChapterVerse;
                if (null == reference)
                    return 0;
                return ChapterVerse.Chapter;
            }
        }
        #endregion
        #region VAttr{g}: int Verse
        public int Verse
        {
            get
            {
                var reference = ChapterVerse;
                if (null == reference)
                    return 0;
                return ChapterVerse.Verse;
            }
        }
        #endregion

        // Localizations ---------------------------------------------------------------------
        #region SAttr{g}: string MergeAuthor
        static public string MergeAuthor
        {
            get
            {
                return Loc.GetNotes("kMergeAuthored", "From Merge");
            }
        }
        #endregion
        #region SAttr{g}: string NoCategory
        static public string NoCategory
        {
            get
            {
                return Loc.GetNotes("kNoCategory", "No Category");
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public TranslatorNote()
            : base()
        {
            // Default is General all-purpose note
            Behavior = General;

            // Messages, sorted by date
            m_osMessages = new JOwnSeq<DMessage>("Messages", this, false, true);
        }
        #endregion
        #region Constructor(sContext)
        public TranslatorNote(string _sSelectedText)
            : this()
        {
            // Context string
            SelectedText = _sSelectedText;
        }
        #endregion
        #region Constructor(Properties)
        public TranslatorNote(Properties _behavior)
            : this()
        {
            Behavior = _behavior;
        }
        #endregion
        #region Method: bool ContentEquals(JObject)
        public override bool ContentEquals(JObject obj)
        {
            TranslatorNote tn = obj as TranslatorNote;
            if (null == tn)
                return false;

            if (tn.Behavior != Behavior)
                return false;
            if (tn.SelectedText != SelectedText)
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
                var reference = ChapterVerse;
                if (null == reference)
                    return SelectedText;

                string s = reference.ParseableName;
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
        protected void ClonePopulate(TranslatorNote noteNew)
        {
            noteNew.Behavior = Behavior;
            noteNew.SelectedText = SelectedText;

            foreach (DMessage m in Messages)
                noteNew.Messages.Append(m.Clone());
        }
        public virtual TranslatorNote Clone()
        {
            var note = new TranslatorNote();
            ClonePopulate(note);
            return note;
        }
        #endregion
        #region Method: DText OwningTextOrNull
        public DText OwningTextOrNull
            // Sometimes TranslatorNotes are not owned by a DText (e.g., if they
            // are History notes, or if they have been deleted; in which case
            // null is returned.
        {
            get
            {
                var text = Owner as DText;
                return text;
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
        #region SMethod: Bitmap GetIcon()
        static public Bitmap GetIcon()
        {
            const string sResourceName = "Annotation.bmp";
            return JWU.GetBitmap(sResourceName);
        }
        #endregion
        #region SMethod: Bitmap GetCheckedIcon()
        static public Bitmap GetCheckedIcon()
        {
            const string sResourceName = "AnnotationChecked.bmp";
            return JWU.GetBitmap(sResourceName);
        }
        #endregion
        #region SAttr{g}: Color OriginalBitmapNoteColor
        static public Color OriginalBitmapNoteColor
        {
            get
            {
                return Color.FromArgb(245, 245, 100);
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Attr{g/s}: string SfmMarker
        public string SfmMarker
        {
            get
            {
                return m_sSfmMarker;
            }
            set
            {
                m_sSfmMarker = value;
            }
        }
        string m_sSfmMarker;
        #endregion
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

        static List<string> s_vOldNotesMarkers;

        static public TranslatorNote ImportFromOldStyle(
            int nChapter, int nVerse, SfField field)
        {
            // Make sure it is a file for which notes are permitted
            if (null == s_vOldNotesMarkers)
            {
                s_vOldNotesMarkers = new List<string>() { 
                    "nt", "ntHint", "ntck", "ntUns", "ntReas", "ntFT", 
                    "ntDef", "ov", "ntBT", "ntgk", "nthb", "ntcn" 
                };
            }
            if (!s_vOldNotesMarkers.Contains(field.Mkr))
                return null;

            // The author is set to "Unknown Author".
            var sAuthor = UnknownAuthor;

            // The date is set to today
            var dtCreated = DateTime.Now;

            // Create the TranslatorNote. We will not have a context for it.
            var tn = new TranslatorNote("") {Behavior = General};

            // Interpret its Role
            var role = Role.Closed;
            switch (field.Mkr)
            {
                case "ntHint":
                    role = Role.DaughterTeam;
                    break;
                case "ntBT":
                case "ntgk":
                case "nthb":
                case "ntcn":
                    role = Role.Information;
                    break;
            }

            // For now, we'll preserve the SFM marker so we can round-trip
            tn.m_sSfmMarker = field.Mkr;

            // Remove any reference from the data
            var sData = tn.RemoveInitialReferenceFromText(field.Data);

            // Old data (e.g., \ov's) might have footnotes |fn in them. Remove them, as
            // we don't allow notes to have footnotes. (We saw this in Manado Malay data.)
            sData = sData.Replace("|fn", "");

            // If there's not data, we're tossing it. Given that it is old-style
            // anyway, there's no need to keep it around if it had no content
            if (string.IsNullOrEmpty(sData))
                return null;

            // The annotation needs a context. We'll go with the first five words
            // of the annotation. Its messy in that it gets repeated when the annotation is
            // expanded, unless we clear the ShowHeaderWhenExpanded flag.
            tn.SelectedText = GetWordsRight(sData, 0, 5);

            // Add the Message
            var message = new DMessage(sAuthor, dtCreated, role, sData);
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
            // This allows us to write the TranslatorNote in the old style, which we
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
            var sAuthor = Loc.GetString("kUnknownAuthor", "Unknown Author");
            if (Messages[0].Author != sAuthor)
                return null;

            // Create the note
            return new SfField(Status.SfmMarker, Messages[0].Runs[0].ContentsSfmSaveString);
        }
        #endregion
        #region Method: void AddToSfmDB(ScriptureDB DB)
        public void AddToSfmDB(ScriptureDB DB)
        {
            // If no messages, don't write anything out
            if (!HasMessages)
                return;

            // Attempt to write to the old style. Fails if the TranslatorNote is too complicated.
            var f = ToSfField();

            // Failed. Must do a new style
            if (null == f)
            {
                var oxes = new XmlDoc();
                var node = Save(oxes, oxes);
                if (null != node)
                    f = new SfField(DSFMapping.c_sMkrTranslatorNote, XmlDoc.OneLiner(node));
            }

            DB.Append(f);
        }
        #endregion
        #region Method: string RemoveInitialReferenceFromText(string sIn)
        public string RemoveInitialReferenceFromText(string sIn)
            // It is redundant in the display, to show the reference there, but to also
            // have it in the note text. So I remove it on importing from the old style, 
            // as we have a lot of legacy notes that have it.
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

            // Attempt to parse
            var reference = DReference.CreateFromParsing(sRef);
            if (null == reference)
                return sIn;
            if (reference.Chapter == 0 || reference.Verse == 0)
                return sIn;

            // If there was a valid reference, then we can remove it
            sIn = (iPos < sIn.Length) ?
                sIn.Substring(iPos) : "";
            return sIn;
        }
        #endregion

        // Old History I/O -------------------------------------------------------------------
        // After upgrading Manado and Timor, can probably remove this
        public const string c_sTagOldHistory = "History";
        #region Method: bool ReadOldHistory(SfField)
        public bool ReadOldHistory(SfField f)
            // Returns True if the f was a History field, false otherwise.
            // If the f.Data was empty, then we don't read an empty history. We just do nothing.
        {
            if (f.Mkr != c_sTagOldHistory)
                return false;

            if (string.IsNullOrEmpty(f.Data))
                return true;

            // The old model just had a series of "DEvents"; we need to encapsulate these
            // inside of an annotation
            if (f.Data.ToLower().StartsWith("<devent"))
            {
                f.Data = "<" + c_sTagTranslatorNote + " class=\"History\">" +
                    f.Data +
                    "</" + c_sTagTranslatorNote + ">";
            }

            var xml = new XmlDoc(new string[] { f.Data });
            var nodeHistory = XmlDoc.FindNode(xml, 
                new string[] { c_sTagOldHistory, c_sTagTranslatorNote });

            foreach (XmlNode child in nodeHistory.ChildNodes)
            {
                var em = new DEventMessage(child);
                AddMessage(em);
            }

            return true;
        }
        #endregion

        // Oxes I/O --------------------------------------------------------------------------
        #region Constants
        public const string c_sTagTranslatorNote = "Annotation";
        private const string c_sAttrClass = "class";
        private const string c_sAttrSelectedText = "selectedText";
        private const string c_sAttrTitle = "title";
        #endregion
        #region Class: CreateNote
        class CreateNote
        {
            readonly XmlNode m_nodeNote;

            // Methods for retrieving the attributes -----------------------------------------
            #region Method: string GetSelectedText()
            string GetSelectedText()
            {
                var s = XmlDoc.GetAttrValue(m_nodeNote,
                    new string[] { c_sAttrSelectedText, "context" },
                    "");
                return s;
            }
            #endregion

            // Other Methods -----------------------------------------------------------------
            #region Method: void GetMessages(note)
            void GetMessages(TranslatorNote note)
            {
                // Toolbox old style had a <ownseq node above the messages; so if we see this,
                // we simply move down to it prior to iterating through the Message nodes.
                var nodeParent = XmlDoc.FindNode(m_nodeNote, "ownseq");
                if (null == nodeParent)
                    nodeParent = m_nodeNote;

                // Messages
                foreach (XmlNode child in nodeParent.ChildNodes)
                {
                    // The type of messages depends on what attributes it has
                    DMessage message = null;
                    if (DEventMessage.IsEventMessageNode(child))
                        message = new DEventMessage(child);
                    else
                        message = new DMessage(child);

                    // Add the message to the list
                    if (null != message)
                        note.Messages.Append(message);
                }
            }
            #endregion
            #region Method: void HandleOldStatus(note)
            void HandleOldStatus(TranslatorNote note)
                // Toolbox old style had an AssignedTo attribute. We want that to be the
                // Status of the LastMessage.
                //
                // Important: This must be called after the call to GetMessages, so 
                // that the messages will have been read in and the final one can
                // therefore be modified.
            {
                var sAssignedTo = XmlDoc.GetAttrValue(m_nodeNote, "assignedTo", "");
                if (!string.IsNullOrEmpty(sAssignedTo) && note.Messages.Count > 0)
                    note.LastMessage.Status = Role.FindFromOxesName(sAssignedTo);
            }
            #endregion

            // Public Interface --------------------------------------------------------------
            #region Constructor(nodeNote)
            public CreateNote(XmlNode nodeNote)
            {
                m_nodeNote = nodeNote;
            }
            #endregion
            #region Method: TranslatorNote Do()
            public TranslatorNote Do()
            {
                // Do nothing if we aren't the right type of node
                if (m_nodeNote.Name != c_sTagTranslatorNote &&
                    m_nodeNote.Name != "TranslatorNote")
                {
                    return null;
                }

                // Create the new note
                var note = new TranslatorNote();

                // Attrs
                note.SelectedText = GetSelectedText();
                note.Title = XmlDoc.GetAttrValue(m_nodeNote, c_sAttrTitle, "");

                // Messages
                GetMessages(note);
                HandleOldStatus(note);

                // Class (Properties)
                var sBehavior = XmlDoc.GetAttrValue(m_nodeNote, c_sAttrClass,
                    TranslatorNote.General.Name);
                // Convert old-style classes into roles
                switch (sBehavior.ToLowerInvariant())
                {
                    case "hintfordrafting":
                        note.Status = Role.DaughterTeam;
                        note.Behavior = TranslatorNote.General;
                        break;
                    case "consultant":
                    case "exegetical":
                        note.Status = Role.Information;
                        note.Behavior = TranslatorNote.General;
                        break;
                    default:
                        note.Behavior = TranslatorNote.Properties.Find(sBehavior);
                        break;
                }

                return note;
            }
            #endregion
        }
        #endregion
        #region SMethod: TranslatorNote Create(nodeNote)
        static public TranslatorNote Create(XmlNode nodeNote)
        {
            return (new CreateNote(nodeNote)).Do();
        }
        #endregion
        #region Method: XmlNode Save(oxes, nodeAnnotation)
        public XmlNode Save(XmlDoc oxes, XmlNode nodeParent)
        {
            // We don't save empty notes
            if (!HasMessages)
                return null;
            if (Messages.Count == 1 && String.IsNullOrEmpty(Messages[0].SimpleText))
                return null;

            var nodeNote = oxes.AddNode(nodeParent, c_sTagTranslatorNote);

            // Attrs
            oxes.AddAttr(nodeNote, c_sAttrClass, Behavior.Name);

            if (!string.IsNullOrEmpty(SelectedText))
                oxes.AddAttr(nodeNote, c_sAttrSelectedText, SelectedText);
            if (!string.IsNullOrEmpty(Title) && Title != SelectedText)
                oxes.AddAttr(nodeNote, c_sAttrTitle, Title);

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
                if (!IsTargetTranslationNote)
                    return false;

                return true;
            }
        }
        #endregion
        #region Method: string GetDisplayableReference()
        public string GetDisplayableReference()
        {
            var reference = ChapterVerse;
            if (null == reference)
                return "";
            return reference.FullName;
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
        #region Method: string GetFullReference()
        public string GetFullReference()
        {
            return string.Format("{0} {1} {2}", 
                OwningBook.Translation.DisplayName,
                OwningBook.DisplayName, 
                GetDisplayableReference());
        }
        #endregion

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
            var TheirLastMessageDate = (Theirs.HasMessages) ? 
                Theirs.LastMessage.UtcCreated : DateTime.UtcNow;
            var OurLastMessageDate = (HasMessages) ?
                LastMessage.UtcCreated : DateTime.UtcNow;

            if (TheirLastMessageDate.CompareTo(OurLastMessageDate) > 1)
            {
                Behavior = Theirs.Behavior;
                SelectedText = Theirs.SelectedText;
                SfmMarker = Theirs.SfmMarker;
            }

            // Go through their messages. Anything that we have, we merge; everything
            // else, we add. Thus the result is a superset of all Message objects.
            foreach (DMessage msgTheirs in Theirs.Messages)
            {
                // If we have it, then merge the two
                var bFound = false;
                foreach (DMessage msgOurs in Messages)
                {
                    if (msgTheirs.IsSameOriginAs(msgOurs))
                    {
                        var msgParent = FindInParent(Parent, msgOurs);
                        // You'd think if its in Ours and Theirs, it would also be in Parent.
                        // But not necessarily true, if someone has just physically copied the
                        // file into the directory without a common parent. This is a patheological
                        // event, hopefully rare, in which case we just accept "Ours" rather than
                        // attempting something smarter.
                        if (null != msgParent)
                            msgOurs.Merge(msgParent, msgTheirs);
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
}
