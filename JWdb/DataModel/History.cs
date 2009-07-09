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
using JWTools;
using JWdb;
#endregion
#endregion

namespace JWdb.DataModel
{
    public class DEvent : JObject
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DEvent()
            : base()
        {
            m_ownDescription = new JOwn<DParagraph>("Description", this);
            Description = new DParagraph();
            Description.StyleAbbrev = DStyleSheet.c_StyleNoteDiscussion;

            m_dtDateCreated = DateTime.Now;
        }
        #endregion
        #region OMethod: bool ContentEquals(JObject obj)
        public override bool ContentEquals(JObject obj)
        {
            DEvent e = obj as DEvent;
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
        public DEvent Clone()
        {
            DEvent Event = new DEvent();
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
        static public DEvent CreateFromXmlString(string s)
        {
            XElement[] x = XElement.CreateFrom(s);
            if (x.Length != 1)
                return null;
            DEvent e = new DEvent();
            e.FromXml(x[0]);

            // An empty stage is how we know the data was corrupt.
            if (string.IsNullOrEmpty(e.Stage))
                return null;

            return e;
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: void Merge(Parent, Theirs)
        public void Merge(DEvent Parent, DEvent Theirs)
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

    public class DHistory : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq Events
        public JOwnSeq<DEvent> Events
        {
            get
            {
                return m_osEvents;
            }
        }
        private JOwnSeq<DEvent> m_osEvents;
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
            m_osEvents = new JOwnSeq<DEvent>("Events", this, true, true);
        }
        #endregion
        #region Method: DHistory Clone()
        public DHistory Clone()
        {
            DHistory History = new DHistory();
            foreach (DEvent e in Events)
                History.Events.Append(e.Clone());
            return History;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: DEvent CreateEvent(DateTime, sStage, sDescription)
        public DEvent CreateEvent(DateTime dtDate, string sStage, string sDescription)
        {
            DEvent e = new DEvent();
            e.Date = dtDate;
            e.Stage = sStage;
            e.Description.SimpleText = sDescription;

            return e;
        }
        #endregion
        #region Method: DEvent AddEvent(DEvent)
        public DEvent AddEvent(DEvent Event)
        {
            // If allready there, just update it to the new description, but don't add a
            // duplicate
            int i = Events.Find(Event.SortKey);
            if (i != -1)
            {
                Events[i].Description.SimpleText = Event.Description.SimpleText;
                return Events[i];
            }

            // Otherwise append the new one
            Events.Append(Event);
            return Event;
        }
        #endregion
        #region Method: DEvent AddEvent(DateTime dtDate, string sStage, string sDescription)
        public DEvent AddEvent(DateTime dtDate, string sStage, string sDescription)
        {
            DEvent e = new DEvent();
            e.Date = dtDate;
            e.Stage = sStage;
            e.Description.SimpleText = sDescription;

            return AddEvent(e);
        }
        #endregion

        // Merge -----------------------------------------------------------------------------
        #region Method: DEvent GetCorresponding(DHistory History, DateTime dtCreated)
        DEvent GetCorresponding(DHistory History, DateTime dtCreated)
        {
            foreach (DEvent e in History.Events)
            {
                if (e.DateCreated == dtCreated)
                    return e;
            }
            return null;
        }
        #endregion
        #region Method: void Merge(DHistory Parent, DHistory Theirs)
        public void Merge(DHistory Parent, DHistory Theirs)
        {
            // Get a list of the Events in Theirs, for convenience as we work through them
            var vTheirs = new List<DEvent>();
            foreach (DEvent e in Theirs.Events)
                vTheirs.Add(e);

            // Anything they have that's not in the parent is new (and presumably not
            // in Ours)
            for (int i = 0; i < vTheirs.Count; )
            {
                if (null == GetCorresponding(Parent, vTheirs[i].DateCreated))
                {
                    Theirs.Events.Remove(vTheirs[i]);
                    this.Events.Append(vTheirs[i]);
                    vTheirs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // Anything theirs remaining should merge with us
            foreach (DEvent TheirEvent in vTheirs)
            {
                // If no parent event, someone's been monkeying with the parent file
                DEvent ParentEvent = GetCorresponding(Parent, TheirEvent.DateCreated);
                if (null == ParentEvent)
                    continue;

                // If Ours is missing, it means we deleted it. We'll let the deletion
                // win, even though they may have made changes.
                DEvent OurEvent = GetCorresponding(this, TheirEvent.DateCreated);
                if (null == OurEvent)
                    continue;

                // Do the merge
                OurEvent.Merge(ParentEvent, TheirEvent);
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        public const string c_sMkr = "History";
        #region Method: SfField ToSfm()
        public SfField ToSfm()
        {
            if (!HasHistory)
                return null;

            // Build the contents; we'll put each event on a separate line
            string sContents = "";
            foreach(DEvent e in Events)
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
            string sMatch = "<" + DEvent.c_sTag + " ";
            string[] v = f.Data.Split(new string[] { sMatch }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < v.Length; i++)
                v[i] = sMatch + v[i];

            // Each one is an event
            foreach (string s in v)
            {
                DEvent e = DEvent.CreateFromXmlString(s);
                if (null != e)
                    Events.Append(e);
            }

            return true;
        }
        #endregion
    }
}
