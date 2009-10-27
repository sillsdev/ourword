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
using OurWordData;
#endregion
#endregion

// TODO: Do we want a Context for the vernacular, and another for the BT? Thus different things
// would potentially show in each view; or alternatively, if no context we could decide to
// not show the annotation in that particular view? Currently, the Context is not really all that
// meaningful, in that we don't highlight it in the text. But when we do, we'll regret contexts
// that don't work for us.

namespace OurWordData.DataModel
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
        #region BAttr{g/s}: DateTime UtcCreated
        public DateTime UtcCreated
        {
            get
            {
                return m_utcDtCreated;
            }
            set
            {
                SetValue(ref m_utcDtCreated, value);
            }
        }
        private DateTime m_utcDtCreated;
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
            DefineAttr("Created", ref m_utcDtCreated);
            DefineAttr("Status", ref m_sStatus);
        }
        #endregion

        #region VAttr{g}: DateTime LocalTimeCreated
        public DateTime LocalTimeCreated
        {
            get
            {
                return m_utcDtCreated.ToLocalTime();
            }
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
            SimpleTextBT = "";

            // The Default date is "Today"
            m_utcDtCreated = DateTime.UtcNow;

            // The author
            Author = DB.UserName;

            Debug_VerifyIntegrity();
        }
        #endregion
        #region Constructor(sAuthor, UtcDtCreated, sStatus, string sSimpleText)
        public DMessage(string sAuthor, DateTime utcDtCreated, string sStatus, string sSimpleText)
            : this()
        {
            Author = sAuthor;
            m_utcDtCreated = utcDtCreated;
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
        #region Constructor(XmlNode)
        public DMessage(XmlNode node)
            : this()
        {
            ReadFromOxes(node);
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
            if (message.UtcCreated.CompareTo(UtcCreated) != 0)
                return false;
            if (message.Status != Status)
                return false;

            if (!base.ContentEquals(message))
                return false;

            return true;
        }
        #endregion
        #region VirtMethod: DMessage Clone()
        public virtual DMessage Clone()
        {
            var message = new DMessage();
            message.Author = Author;
            message.UtcCreated = UtcCreated;
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
                return UtcCreated.ToString("u");
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
                    "Created={" + UtcCreated.ToShortDateString() + "} " +
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
        #region VirtMethod: void ReadFromOxes(XmlNode node)
        public virtual bool ReadFromOxes(XmlNode node)
        {
            if (!XmlDoc.IsNode(node, c_sTagMessage) && 
                !XmlDoc.IsNode(node, "Discussion") &&
                !XmlDoc.IsNode(node, "DEvent"))
                return false;

            // Attrs
            Author = XmlDoc.GetAttrValue(node, c_sAttrAuthor, "");
            UtcCreated = XmlDoc.GetAttrValue(node, c_sAttrCreated, DateTime.Now);
            Status = XmlDoc.GetAttrValue(node, c_sAttrStatus, "");

            // Import old-style paragraph contents; if successful, we're done.
            if (ImportOldToolboxXmlParagraphContents(node))
                return true;

            // Otherwise, normally-stored paragraph contents
            ReadOxes(node);

            // The paragraph's ReadOxes method will attempt to set StyyleAbbrev to "p", because
            // we don't actually have an attr in the xml. So even though the call
            // to "this" constructor set it originally, we have to set it again here.
            StyleAbbrev = DStyleSheet.c_StyleAnnotationMessage;

            return true;
        }
        #endregion
        #region VirtMethod: XmlNode Save(oxes, nodeAnnotation)
        public virtual XmlNode Save(XmlDoc oxes, XmlNode nodeAnnotation)
        {
            var nodeMessage = oxes.AddNode(nodeAnnotation, c_sTagMessage);

            // Attrs
            if (!string.IsNullOrEmpty(Author))
                oxes.AddAttr(nodeMessage, c_sAttrAuthor, Author);
            oxes.AddAttr(nodeMessage, c_sAttrCreated, UtcCreated);

            // An empty Status is interpreted as "closed", so we want to not
            // add the status attr unless we have content.
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
            if (0 != UtcCreated.CompareTo(Theirs.UtcCreated))
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

    #region CLASS: DEventMessage
    public class DEventMessage : DMessage
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: DateTime EventDate - Date this event took place
        public DateTime EventDate
        {
            get
            {
                if (m_dtEventDate.CompareTo(DefaultDate) == 0)
                    m_dtEventDate = UtcCreated;

                return m_dtEventDate;
            }
            set
            {
                SetValue(ref m_dtEventDate, value);
            }
        }
        private DateTime m_dtEventDate;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Date", ref m_dtEventDate);
        }
        #endregion

        #region Attr{g/s}: Stage Stage
        public Stage Stage
        {
            get
            {
                return m_Stage;
            }
            set
            {
                if (m_Stage != value)
                {
                    m_Stage = value;
                    DeclareDirty();
                }
            }
        }
        Stage m_Stage;
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: DateTime DefaultDate - default for the Date attr
        public static DateTime DefaultDate
        {
            get
            {
                return new DateTime(2009, 1, 1);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DEventMessage()
            : base()
        {
        }
        #endregion
        #region Constructor(XmlNode)
        public DEventMessage(XmlNode node)
            : this()
        {
            ReadFromOxes(node);
        }
        #endregion
        #region Constructor(utcDate, sStage, sDescription)
        public DEventMessage(DateTime utcDate, Stage stage, string sDescription)
            : this()
        {
            EventDate = utcDate;
            UtcCreated = EventDate;
            m_Stage = stage;
            SimpleText = sDescription;
        }
        #endregion
        #region OMethod: bool ContentEquals(DEventMessage)
        public override bool ContentEquals(JObject obj)
        {
            var e = obj as DEventMessage;
            if (null == e)
                return false;

            if (e.EventDate != EventDate)
                return false;
            if (e.Stage.EnglishAbbrev != Stage.EnglishAbbrev)
                return false;

            return base.ContentEquals(obj);
            ;
        }
        #endregion
        #region OMethod: DMessage Clone() - returns a DEventMessage
        public override DMessage Clone()
        {
            var em = new DEventMessage();
            em.Author = Author;
            em.UtcCreated = UtcCreated;
            em.Status = Status;
            em.CopyFrom(this, false);

            em.EventDate = EventDate;
            em.Stage = Stage;

            return em;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        const string c_sAttrEventDate = "when";
        const string c_sAttrStage = "stage";
        #endregion
        #region OMethod: bool ReadFromOxes(XmlNode node)
        public override bool ReadFromOxes(XmlNode node)
        {
            // Read all of the base data
            if (!base.ReadFromOxes(node))
                return false;

            // Add the DEventMessage data
            // Stage
            var sStage = XmlDoc.GetAttrValue(node, c_sAttrStage, Stage.c_sDraft);
            // Normally we expect this to be the English Abbreviation
            Stage = DB.TeamSettings.Stages.Find(sStage);
            // But old data may have been saved some other way
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.LocalizedAbbrev, sStage);
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.LocalizedName, sStage);
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.EnglishName, sStage);
            // Give Up
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Draft;

            // EventDate, including old-style which was "Date"
            EventDate = XmlDoc.GetAttrValue(node,
                new string[] { c_sAttrEventDate, "Date" },
                DateTime.Now);

            return true;
        }
        #endregion
        #region OMethod: XmlNode Save(oxes, nodeAnnotation)
        public override XmlNode Save(XmlDoc oxes, XmlNode nodeAnnotation)
        {
            // Save the superclass values
            XmlNode nodeEventMessage = base.Save(oxes, nodeAnnotation);

            // Add in our additional attributes
            oxes.AddAttr(nodeEventMessage, c_sAttrEventDate, EventDate);
            oxes.AddAttr(nodeEventMessage, c_sAttrStage, Stage.EnglishAbbrev);

            return nodeEventMessage;
        }
        #endregion
        #region SMethod: bool IsEventMessageNode(XmlNode node)
        static public bool IsEventMessageNode(XmlNode node)
        {
            if (XmlDoc.HasAttr(node, c_sAttrEventDate))
                return true;
            return false;
        }
        #endregion
    }
    #endregion

    #region Class: TranslatorNote
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
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("SelectedText", ref m_sSelectedText);
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
        JOwnSeq<DMessage> m_osMessages;
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
        #region VAttr{g}: bool HasMessages
        public bool HasMessages
        {
            get
            {
                return (Messages.Count > 0);
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
            #region Attr{g}: string SfmMarker
            public string SfmMarker
            {
                get
                {
                    return m_sSfmMarker;
                }
            }
            readonly string m_sSfmMarker;
            #endregion

            // Writing System
            #region Attr{g}: bool ConsultantWritingSystem
            public bool ConsultantWritingSystem
            {
                get
                {
                    return m_bConsultantWritingSystem;
                }
            }
            readonly bool m_bConsultantWritingSystem;
            #endregion
            #region Method: JWritingSystem GetWritingSystem(TranslatorNote note)
            public JWritingSystem GetWritingSystem(TranslatorNote note)
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
            #region VAttr{g}: string Title
            public string Title
            {
                get
                {
                    return Loc.GetString(m_sTitleLocID, m_sTitleEnglishDefault);
                }
            }
            #endregion
            readonly string m_sTitleLocID;
            readonly string m_sTitleEnglishDefault;

            // Icons
            #region Attr{g}: private string IconResourceBaseName
            private string IconResourceBaseName
            {
                get
                {
                    return m_sIconResourceBaseName;
                }
            }
            readonly string m_sIconResourceBaseName;
            #endregion
            #region Attr{g}: string IconResourceAnyone
            public string IconResourceAnyone
            {
                get
                {
                    return IconResourceBaseName + "_Anyone.ico";
                }
            }
            #endregion
            #region Attr{g}: string IconResourceClosed
            public string IconResourceClosed
            {
                get
                {
                    return IconResourceBaseName + "_Closed.ico";
                }
            }
            #endregion
            #region Attr{g}: string IconResourceMe
            public string IconResourceMe
            {
                get
                {
                    return IconResourceBaseName + "_Me.ico";
                }
            }
            #endregion

            // Scaffolding
            #region Constructor(sName, sIconResource, sSfn, bIsConsultant, sIdTitle, sEnglishTitle)
            public Properties(string sName, 
                string sIconResourceBaseName, 
                string sSfmMarker, 
                bool bConsultantWS,
                string sTitleLocID,
                string sTitleEnglishDefault)
            {
                m_sName = sName;
                m_sIconResourceBaseName = sIconResourceBaseName;
                m_sSfmMarker = sSfmMarker;
                m_bConsultantWritingSystem = bConsultantWS;
                m_sTitleLocID = sTitleLocID;
                m_sTitleEnglishDefault = sTitleEnglishDefault;

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
        #region Definitions: General, Exegetical, Consultant, HintForDrafting, History
        static readonly public Properties General = new Properties("General", "NoteGeneric", 
            "nt", false, "kGeneralNote", "Note");

        static readonly public Properties Exegetical = new Properties("Exegetical", "NoteExegesis", 
            "ntcn", true, "kExegeticalNote", "Exegetical Note");

        static readonly public Properties Consultant = new Properties("Consultant", "NoteConsultant",
            "ntBT", true, "kConsultantNote", "Consultant Note");

        static readonly public Properties HintForDrafting = new Properties("HintForDrafting", "NoteHint",
            "ntHint", false, "kDraftingHint", "Drafting Hint");

        static readonly public Properties History = new Properties("History", "Note_OldVersions", 
            "History", false, "kHistory", "History");
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

        // Scripture Reference containing the note -------------------------------------------
        #region VAttr{g}: DReference ChapterVerse
        public DReference ChapterVerse
        {
            get
            {
                // Get the owning text and paragraph 
                if (null == Owner || null == Text || null == Text.Paragraph)
                    return null;
                var text = Text;
                var paragraph = Text.Paragraph;
                
                // We'll start with the initial chapter/verse of the paragraph
                int nChapter = paragraph.ReferenceSpan.Start.Chapter;
                int nVerse = paragraph.ReferenceSpan.Start.Verse;

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

        // Localizations --------------------------------------------------------------------
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
        #region Attr{g/s}: bool CanDeleteAnything
        static public bool CanDeleteAnything
        {
            get
            {
                return JW_Registry.GetValue(c_sRegistrySubkey,
                        "CanDeleteAnything", false);
            }
            set
            {
                JW_Registry.SetValue(c_sRegistrySubkey,
                    "CanDeleteAnything", value);
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
        #region Attr{g}: DText Text - the owning DText
        public DText Text
        {
            get
            {
                DText text = Owner as DText;

                // History notes are not owned by a text, so we want to avoid the 
                // assertion for them.
                if (Behavior == History)
                    return null;
                Debug.Assert(null != text);

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
            string sAuthor = UnknownAuthor;

            // The date is set to today
            DateTime dtCreated = DateTime.Now;

            // Create the Note. We will not have a context for it.
            TranslatorNote tn = new TranslatorNote("");

            // Set its class
            switch (field.Mkr)
            {
                case "ntHint":
                    tn.Behavior = HintForDrafting;
                    break;
                case "ntBT":
                    tn.Behavior = Consultant;
                    break;
                case "ntgk":
                case "nthb":
                case "ntcn":
                    tn.Behavior = Exegetical;
                    break;
                default:
                    tn.Behavior = General;
                    break;
            }


            // For now, we'll preserve the SFM marker so we can round-trip
            tn.m_sSfmMarker = field.Mkr;

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
            tn.SelectedText = GetWordsRight(sData, 0, 5);

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

            // Create the note
            return new SfField(Behavior.SfmMarker, Messages[0].Runs[0].ContentsSfmSaveString);
        }
        #endregion
        #region Method: void AddToSfmDB(ScriptureDB DB)
        public void AddToSfmDB(ScriptureDB DB)
        {
            // If no messages, don't write anything out
            if (!HasMessages)
                return;

            // Attempt to write to the old style. Fails if the note is too complicated.
            SfField f = ToSfField();

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
        const string c_sAttrClass = "class";
        const string c_sAttrSelectedText = "selectedText";
        #endregion
        #region Class: CreateNote
        class CreateNote
        {
            XmlNode m_nodeNote;

            // Methods for retrieving the attributes -----------------------------------------
            #region Method: string GetSelectedText()
            string GetSelectedText()
            {
                string s = XmlDoc.GetAttrValue(m_nodeNote,
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
                string sAssignedTo = XmlDoc.GetAttrValue(m_nodeNote, "assignedTo", "");
                if (!string.IsNullOrEmpty(sAssignedTo) && note.Messages.Count > 0)
                    note.LastMessage.Status = sAssignedTo;
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

                // Class (Properties)
                var sBehavior = XmlDoc.GetAttrValue(m_nodeNote, c_sAttrClass,
                    TranslatorNote.General.Name);
                note.Behavior = TranslatorNote.Properties.Find(sBehavior);

                // Attrs
                note.SelectedText = GetSelectedText();

                // Messages
                GetMessages(note);
                HandleOldStatus(note);

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

            // Message objects
            foreach (DMessage m in Messages)
            {
                m.Save(oxes, nodeNote);
            }

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
            DateTime TheirLastMessageDate = (Theirs.HasMessages) ? 
                Theirs.LastMessage.UtcCreated : DateTime.UtcNow;
            DateTime OurLastMessageDate = (HasMessages) ?
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
                bool bFound = false;
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
    #endregion
}
