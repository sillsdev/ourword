#region ***** History.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    History.cs
 * Author:  John Wimbish
 * Created: 24 May 2009
 * Purpose: Maintains a history of the work done on a book or section
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using JWTools;
using JWdb;
#endregion
#endregion

namespace JWdb.DataModel
{
    public class DEventMessage : DMessage
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: DateTime EventDate - Date this event took place
        public DateTime EventDate
        {
            get
            {
                if (m_dtEventDate.CompareTo(DefaultDate) == 0)
                    m_dtEventDate = Created;

                return m_dtEventDate;
            }
            set
            {
                SetValue(ref m_dtEventDate, value);
            }
        }
        private DateTime m_dtEventDate;
        #endregion
        #region BAttr{g/s}: string Stage - translation stage (draft, revision, etc)
        public string Stage
        {
            get
            {
                return m_sStage;
            }
            set
            {
                SetValue(ref m_sStage, value);
            }
        }
        private string m_sStage = "";
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Date", ref m_dtEventDate);
            DefineAttr("Stage", ref m_sStage);
        }
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

        #region OMethod: bool ContentEquals(DEventMessage)
        public override bool ContentEquals(JObject obj)
        {
            var e = obj as DEventMessage;
            if (null == e)
                return false;

            if (e.EventDate != EventDate)
                return false;
            if (e.Stage != Stage)
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
            em.Created = Created;
            em.Status = Status;
            em.CopyFrom(this, false);

            em.EventDate = EventDate;
            em.Stage = Stage;

            return em;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        public const string c_sTagEventMessage = "Event";
        #region OAttr{g}: string Tag
        public override string Tag
        {
            get
            {
                return c_sTagEventMessage;
            }
        }
        #endregion

        const string c_sAttrEventDate = "when";
        const string c_sAttrStage = "stage";
        #endregion
        #region SMethod: new bool IsXmlNode(XmlNode node)
        static new public bool IsXmlNode(XmlNode node)
        {
            if (XmlDoc.IsNode(node, c_sTagEventMessage))
                return true;
            if (XmlDoc.IsNode(node, "DEvent"))
                return true;
            return false;
        }
        #endregion
        #region OMethod: void ReadFromOxes(XmlNode node)
        public override void ReadFromOxes(XmlNode node)
        {
            // Read all of the base data
            base.ReadFromOxes(node);

            // Add the DEventMessage data
            // Stage
            Stage = XmlDoc.GetAttrValue(node, c_sAttrStage, "");

            // EventDate, including old-style which was "Date"
            EventDate = XmlDoc.GetAttrValue(node, 
                new string[] { c_sAttrEventDate, "Date" },
                DateTime.Now);
        }
        #endregion
        #region OMethod: XmlNode Save(oxes, nodeAnnotation)
        public override XmlNode Save(XmlDoc oxes, XmlNode nodeAnnotation)
        {
            // Save the superclass values
            XmlNode nodeEventMessage = base.Save(oxes, nodeAnnotation);

            // Add in our additional attributes
            oxes.AddAttr(nodeEventMessage, c_sAttrEventDate, EventDate);
            oxes.AddAttr(nodeEventMessage, c_sAttrStage, Stage);

            return nodeEventMessage;
        }
        #endregion

    }
    #region SOON TO REPLACE: class DEventObs
    public class DEventObs : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: DateTime DateCreated - Used for resolving a merge
        public DateTime DateCreated
        {
            get
            {
                return m_dtDateCreated;
            }
            set
            {
                SetValue(ref m_dtDateCreated, value);
            }
        }
        private DateTime m_dtDateCreated;
        #endregion
        #region BAttr{g/s}: DateTime Date - Date of this event
        public DateTime Date
        {
            get
            {
                if (m_dtDate.CompareTo(DefaultDate) == 0)
                    m_dtDate = DateCreated;

                return m_dtDate;
            }
            set
            {
                SetValue(ref m_dtDate, value);
            }
        }
        private DateTime m_dtDate;
        #endregion
        #region BAttr{g/s}: string Stage - translation stage (draft, revision, etc)
        public string Stage
        {
            get
            {
                return m_sStage;
            }
            set
            {
                SetValue(ref m_sStage, value);
            }
        }
        private string m_sStage = "";
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Created", ref m_dtDateCreated);
            DefineAttr("Date", ref m_dtDate);
            DefineAttr("Stage", ref m_sStage);
        }
        #endregion
        #region JAttr{g/s}: DParagraph Description
        public DParagraph Description
        {
            get
            {
                return m_ownDescription.Value;
            }
            set
            {
                m_ownDescription.Value = value;
            }
        }
        private JOwn<DParagraph> m_ownDescription;
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region Attr{g}: DHistory History
        DHistory History
        {
            get
            {
                return Owner as DHistory;
            }
        }
        #endregion
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
        public DEventObs()
            : base()
        {
            m_ownDescription = new JOwn<DParagraph>("Description", this);
            Description = new DParagraph();
            Description.StyleAbbrev = DStyleSheet.c_StyleAnnotationMessage;

            m_dtDateCreated = DateTime.Now;
        }
        #endregion
        #region OMethod: bool ContentEquals(JObject obj)
        public override bool ContentEquals(JObject obj)
        {
            DEventObs e = obj as DEventObs;
            if (null == e)
                return false;

            if (e.Date != Date)
                return false;
            if (e.Stage != Stage)
                return false;
            if (!e.Description.ContentEquals(Description))
                return false;

            return true;
        }
        #endregion
        #region OAttr{g}: string SortKey
        public override string SortKey
        {
            get
            {
                return Date.ToString("u") + Stage;
            }
        }
        #endregion
        #region Method: DEvent Clone()
        public DEventObs Clone()
        {
            DEventObs Event = new DEventObs();
            Event.DateCreated = DateCreated;
            Event.Date = Date;
            Event.Stage = Stage;
            Event.Description.SimpleText = Description.SimpleText;
            return Event;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        public const string c_sTag = "DEvent";
        #region OMethod: XElement ToXml(bool bIncludeNonBasicAttrs)
        public override XElement ToXml(bool bIncludeNonBasicAttrs)
        {
            // Pick up the BAttrs
            XElement x = base.ToXml(false);

            // We'll add the paragraph text as a string (thus italics, etc not supported at 
            // this time)
            if (bIncludeNonBasicAttrs)
                x.AddSubItem(new XString(Description.SimpleText));

            return x;
        }
        #endregion
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            if (x.Tag != c_sTag)
                return;

            // Extract the basic attributes
            m_ioX = x;
            m_ioOperation = Ops.kRead;
            DeclareAttrs();

            // Retrieve the paragraph contents
            if (x.Items.Count == 1)
            {
                XString xs = x.Items[0] as XString;
                if (null != xs)
                    Description.SimpleText = xs.Text;
            }
        }
        #endregion
        #region SMethod: DEvent CreateFromXmlString(string s)
        static public DEventObs CreateFromXmlString(string s)
        {
            XElement[] x = XElement.CreateFrom(s);
            if (x.Length != 1)
                return null;
            DEventObs e = new DEventObs();
            e.FromXml(x[0]);

            // An empty stage is how we know the data was corrupt.
            if (string.IsNullOrEmpty(e.Stage))
                return null;

            return e;
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: void Merge(Parent, Theirs)
        public void Merge(DEventObs Parent, DEventObs Theirs)
        {
            // We can tell if we have the same ancestor via the DateCreated
            Debug.Assert(Parent.DateCreated == Theirs.DateCreated);
            Debug.Assert(Parent.DateCreated == DateCreated);

            // If same content, then we're done
            if (ContentEquals(Theirs))
                return;

            // If we equal the parent, but they don't, then keep theirs
            if (ContentEquals(Parent) && !Theirs.ContentEquals(Parent))
            {
                Date = Theirs.Date;
                Stage = Theirs.Stage;
                Description.SimpleText = Theirs.Description.SimpleText;
                return;
            }

            // If they equal the parent, but we don't, then keep ours
            if (!ContentEquals(Parent) && Theirs.ContentEquals(Parent))
                return;

            // If we are here, then we have a conflict. We keep our Date and Stage, and
            // append the text.
            if (Description.SimpleText != Theirs.Description.SimpleText)
                Description.SimpleText += (" -From Merge: " + Theirs.Description.SimpleText);
        }
        #endregion
    }
    #endregion

    public class DHistory : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq Events
        public JOwnSeq<DEventMessage> Events
        {
            get
            {
                return m_osEvents;
            }
        }
        private JOwnSeq<DEventMessage> m_osEvents;
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: bool HasHistory
        public bool HasHistory
        {
            get
            {
                return (Events.Count > 0);
            }
        }
        #endregion
        #region VAttr{g}: string MostRecentStage
        public string MostRecentStage
        {
            get
            {
                if (Events.Count == 0)
                    return null;

                return Events[Events.Count - 1].Stage;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DHistory()
            : base()
        {
            m_osEvents = new JOwnSeq<DEventMessage>("Events", this, true, true);
        }
        #endregion
        #region Method: DHistory Clone()
        public DHistory Clone()
        {
            DHistory History = new DHistory();
            foreach (DEventMessage e in Events)
                History.Events.Append(e.Clone());
            return History;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: DEventMessage CreateEvent(DateTime, sStage, sDescription)
        public DEventMessage CreateEvent(DateTime dtDate, string sStage, string sDescription)
        {
            var e = new DEventMessage();
            e.EventDate = dtDate;
            e.Stage = sStage;
            e.SimpleText = sDescription;

            return e;
        }
        #endregion
        #region Method: DEventMessage AddEvent(DEventMessage)
        public DEventMessage AddEvent(DEventMessage Event)
        {
            // If already there, just update it to the new description, but don't add a
            // duplicate
            int i = Events.Find(Event.SortKey);
            if (i != -1)
            {
                Events[i].CopyFrom(Event, false);
                return Events[i];
            }

            // Otherwise append the new one
            Events.Append(Event);
            return Event;
        }
        #endregion
        #region Method: DEventMessage AddEvent(DateTime dtDate, string sStage, string sDescription)
        public DEventMessage AddEvent(DateTime dtDate, string sStage, string sDescription)
        {
            var e = new DEventMessage();
            e.Created = dtDate;
            e.EventDate = dtDate;
            e.Stage = sStage;
            e.SimpleText = sDescription;

            return AddEvent(e);
        }
        #endregion

        // Merge -----------------------------------------------------------------------------
        #region Method: DEventMessage GetCorresponding(DHistory History, DateTime dtCreated)
        DEventMessage GetCorresponding(DHistory History, DateTime dtCreated)
        {
            foreach (DEventMessage e in History.Events)
            {
                if (e.Created == dtCreated)
                    return e;
            }
            return null;
        }
        #endregion
        #region Method: void Merge(DHistory Parent, DHistory Theirs)
        public void Merge(DHistory Parent, DHistory Theirs)
        {
            // Get a list of the Events in Theirs, for convenience as we work through them
            var vTheirs = new List<DEventMessage>();
            foreach (DEventMessage e in Theirs.Events)
                vTheirs.Add(e);

            // Anything they have that's not in the parent is new (and presumably not
            // in Ours)
            for (int i = 0; i < vTheirs.Count; )
            {
                if (null == GetCorresponding(Parent, vTheirs[i].Created))
                {
                    Theirs.Events.Remove(vTheirs[i]);

                    // In theory this condition should never happen; but it did once,
                    // because I think files were manually copied into a folder which
                    // would have bypassed the Parent mechanism.
                    if (null == GetCorresponding(this, vTheirs[i].Created))
                        this.Events.Append(vTheirs[i]);

                    vTheirs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // Anything theirs remaining should merge with us
            foreach (DEventMessage TheirEvent in vTheirs)
            {
                // If no parent event, someone's been monkeying with the parent file
                DEventMessage ParentEvent = GetCorresponding(Parent, TheirEvent.Created);
                if (null == ParentEvent)
                    continue;

                // If Ours is missing, it means we deleted it. We'll let the deletion
                // win, even though they may have made changes.
                DEventMessage OurEvent = GetCorresponding(this, TheirEvent.Created);
                if (null == OurEvent)
                    continue;

                // Do the merge
                OurEvent.Merge(ParentEvent, TheirEvent);
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region SOON OBSOLETE - To/From SFM
        /**
        public const string c_sMkr = "History";
        #region Method: SfField ToSfm()
        public SfField ToSfm()
        {
            if (!HasHistory)
                return null;

            // Build the contents; we'll put each event on a separate line
            string sContents = "";
            foreach (DEventObs e in Events)
                sContents += e.ToXml(true).OneLiner;

            // Create and return the field
            SfField field = new SfField(c_sMkr, sContents);
            return field;
        }
        #endregion
        #region Method: bool FromSfm(SfField f)
        public bool FromSfm(SfField f)
        {
            // Right marker?
            if (f.Mkr != c_sMkr)
                return false;

            // Break the string into one line per Event
            string sMatch = "<DEvent ";
            string[] v = f.Data.Split(new string[] { sMatch }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < v.Length; i++)
                v[i] = sMatch + v[i];

            // Each one is an event
            foreach (string s in v)
            {
                var e = DEventMessage.CreateFromXmlString(s);
                if (null != e)
                    Events.Append(e);
            }

            return true;
        }
        #endregion
        ***/
        #endregion

        public const string c_sTag = "History";

        #region Method: XmlNode Save(XmlDoc oxes, XmlNode nodeParent)
        public XmlNode Save(XmlDoc oxes, XmlNode nodeParent)
        {
            var nodeHistory = oxes.AddNode(nodeParent, c_sTag);

            foreach (DEventMessage e in Events)
                e.Save(oxes, nodeHistory);

            return nodeHistory;
        }
        #endregion
        #region Method: void AddToSfmDB(ScriptureDB SDB)
        public void AddToSfmDB(ScriptureDB SDB)
        {
            if (Events.Count == 0)
                return;

            var oxes = new XmlDoc();
            var node = Save(oxes, oxes);
            var f = new SfField(c_sTag, XmlDoc.OneLiner(node));
            SDB.Append(f);
        }
        #endregion

        #region Method: bool Read(XmlNode nodeHistory)
        public bool Read(XmlNode nodeHistory)
        {
            if (nodeHistory.Name != c_sTag)
                return false;

            foreach (XmlNode child in nodeHistory.ChildNodes)
            {
                var e = new DEventMessage(child);
                Events.Append(e);
            }

            return true;
        }
        #endregion
        #region Method: bool Read(SfField f)
        public bool Read(SfField f)
            // Returns True if the f was a History field, false otherwise.
            // If the f.Data was empty, then we don't read an empty history. We just do nothing.
        {
            if (f.Mkr != c_sTag)
                return false;

            if (string.IsNullOrEmpty(f.Data))
                return true;

            var xml= new XmlDoc(new string[] { f.Data });
            var nodeHistory = XmlDoc.FindNode(xml, c_sTag);
            return Read(nodeHistory);
        }
        #endregion

    }
}
