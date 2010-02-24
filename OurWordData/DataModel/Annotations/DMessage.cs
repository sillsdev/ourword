#region ***** DMessage.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DMessage.cs
 * Author:  John Wimbish
 * Created: 04 Nov 2008
 * Purpose: Handles a single annotation's message 
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Xml;
using JWTools;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;
#endregion

namespace OurWordData.DataModel.Annotations
{
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
        #region BAttr{g/s}: Role Status
        public Role Status
            // We store these with their English names (1) to keep them constant (they could
            // otherwise change at the whim of a localizer), and (2) to keep them readable
            // for people who might have to work with oxes files, which are typically going
            // to be international computer consultant types.
        {
            get
            {
                return string.IsNullOrEmpty(m_sStatus) ? 
                    Role.Closed : 
                    Role.FindFromEnglishName(m_sStatus);
            }
            set
            {
                var sValue = (value == Role.Closed) ? "" : value.EnglishName;
                SetValue(ref m_sStatus, sValue);
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
            : base(StyleSheet.TipMessage)
        {
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
        public DMessage(string sAuthor, DateTime utcDtCreated, Role status, string sSimpleText)
            : this()
        {
            Author = sAuthor;
            m_utcDtCreated = utcDtCreated;
            Status = status;

            // Temporary kludge: remove ||'s that we've  been inserting by mistake
            var sFixed = DSection.IO.EatSpuriousVerticleBars(sSimpleText);

            // Parse the string into phrases and add them
            Runs.Clear();
            var vRuns = DSection.IO.CreateDRunsFromInputText(sFixed);
            foreach (var run in vRuns)
            {
                var text = run as DText;
                if (text != null && text.PhrasesBT.Count == 0)
                    text.PhrasesBT.Append(new DPhrase(""));
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
            var message = new DMessage
            {
                Author = Author,
                UtcCreated = UtcCreated,
                Status = Status
            };
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
        private TranslatorNote Note
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
                string s = "M: " +
                    "Author={" + Author + "} " +
                    "Created={" + UtcCreated.ToShortDateString() + "} " +
                    "Status={" + Status + "} " +
                    "Content={" + base.DebugString + "}";
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
        protected virtual bool ReadFromOxes(XmlNode node)
        {
            if (!XmlDoc.IsNode(node, c_sTagMessage) &&
                !XmlDoc.IsNode(node, "Discussion") &&
                !XmlDoc.IsNode(node, "DEvent"))
                return false;

            // Attrs
            Author = XmlDoc.GetAttrValue(node, c_sAttrAuthor, "");
            UtcCreated = XmlDoc.GetAttrValue(node, c_sAttrCreated, DateTime.Now);
            Status = Role.FindFromOxesName(XmlDoc.GetAttrValue(node, c_sAttrStatus, ""));

            // Import old-style paragraph contents; if successful, we're done.
            if (ImportOldToolboxXmlParagraphContents(node))
                return true;

            // Otherwise, normally-stored paragraph contents
            ReadOxes(node);

            // The paragraph's ReadOxes method will attempt to set StyyleAbbrev to "p", because
            // we don't actually have an attr in the xml. So even though the call
            // to "this" constructor set it originally, we have to set it again here.
            Style = StyleSheet.TipMessage;

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
            if (Status != Role.Closed)
                oxes.AddAttr(nodeMessage, c_sAttrStatus, Status.EnglishName);

            // Paragraph contents
            foreach (DRun run in Runs)
                run.SaveToOxesBook(oxes, nodeMessage);

            return nodeMessage;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region Method: bool IsSameOriginAs(theirs)
        public bool IsSameOriginAs(DMessage theirs)
        // Two messages started out the same if they have the same Author and 
        // Create date.
        {
            if (0 != UtcCreated.CompareTo(theirs.UtcCreated))
                return false;
            if (Author != theirs.Author)
                return false;
            return true;
        }
        #endregion
        #region Method: void Merge(parent, theirs)
        public void Merge(DMessage parent, DMessage theirs)
        {
            // The caller needs to make sure these are the same Author and Created
            Debug.Assert(IsSameOriginAs(theirs), "Theirs wasn't the same message");
            Debug.Assert(IsSameOriginAs(parent), "Parent wasn't the same message");

            // If they are the same, we're done
            if (ContentEquals(theirs))
                return;

            // If we equal the parent, but they don't, then keep theirs
            if (ContentEquals(parent) && !theirs.ContentEquals(parent))
            {
                Status = theirs.Status;
                CopyFrom(theirs, false);
                return;
            }

            // If they equal the parent, but we don't, then keep ours
            if (!ContentEquals(parent) && theirs.ContentEquals(parent))
                return;

            // If we are here, then we have a conflict. We resolve via the Author.
            // If the author is the person on this machine, then we win.
            if (Author == DB.UserName)
                return;
            // If the author is someone else, then they win
            Status = theirs.Status;
            CopyFrom(theirs, false);
            Status = theirs.Status;
        }
        #endregion
    }
}
