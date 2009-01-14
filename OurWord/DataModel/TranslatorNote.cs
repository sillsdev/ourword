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
    public class Discussion
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: string Author
        public string Author
        {
            get
            {
                return m_sAuthor;
            }
            set
            {
                m_sAuthor = value;
            }
        }
        private string m_sAuthor = "";
        #endregion
        #region Attr{g/s}: DateTime Created
        public DateTime Created
        {
            get
            {
                return m_dtCreated;
            }
            set
            {
                m_dtCreated = value;
            }
        }
        private DateTime m_dtCreated;
        #endregion
        #region Attr{g/s}: DBasicText Text
        public DBasicText Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }
        private DBasicText m_Text;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public Discussion()
            : base()
        {
            m_dtCreated = DateTime.Now;
        }
        #endregion
        #region Constructor(sAuthor, string sSimpleText)
        public Discussion(string sAuthor, DateTime dtCreated, string sText)
            : this()
        {
            Author = sAuthor;
            Created = dtCreated;
            Text = new DBasicText(sText);
        }
        #endregion
        #region Method: bool ContentEquals(Discussion)
        public bool ContentEquals(Discussion discussion)
        {
            if (discussion.Author != Author)
                return false;
            if (discussion.Created.CompareTo(Created) != 0)
                return false;
            if (discussion.Text.DebugString != Text.DebugString)
                return false;

            return true;
        }
        #endregion
        #region Attr{g}: string SortKey
        public string SortKey
        {
            get
            {
                // Return Created in the universal invariant form of
                //    "2006-04-17 21:22:48Z"
                return Created.ToString("u");
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region I/O CONSTANTS
        const string c_sTag = "Disc";
        const string c_sAttrAuthor = "Author";
        const string c_sAttrCreated = "Created";
        const string c_sAttrText = "Text";
        #endregion
        #region VAttr{g}: XElement ToXml
        public XElement ToXml
        {
            get
            {
                XElement x = new XElement(c_sTag);
                x.AddAttr(c_sAttrAuthor, Author);
                x.AddAttr(c_sAttrCreated, Created);
                x.AddAttr(c_sAttrText, Text.AsString);
                return x;
            }
        }
        #endregion
        #region SMethod: Discussion CreateFromXml(XElement)
        static public Discussion CreateFromXml(XElement x)
        {
            if (x.Tag != c_sTag)
                return null;

            Discussion discussion = new Discussion();

            discussion.Author = x.GetAttrValue(c_sAttrAuthor, "");
            discussion.Created = x.GetAttrValue(c_sAttrCreated, DateTime.Now);
            discussion.Text = DText.CreateSimple(x.GetAttrValue(c_sAttrText, ""));

            return discussion;
        }
        #endregion

        // View Construction -----------------------------------------------------------------
        #region Method: EContainer BuildNotesPaneView()
        public EContainer BuildNotesPaneView()
        {
            // Create a paragraph to hold the discussion text
            OWPara pDiscussionText = new OWPara(G.TTranslation.WritingSystemVernacular,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteDiscussion),
                Text.Phrases.AsVector);

            // For now, we'll put the author in as a paragraph
            OWPara pHeader = new OWPara(G.TTranslation.WritingSystemVernacular,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteHeader),
                Author);

            EHeaderColumn eHeaderCol = new EHeaderColumn(pHeader);
            eHeaderCol.Style = EHeaderColumn.Styles.RoundedBorder;
            eHeaderCol.Append(pDiscussionText);
            return eHeaderCol;
        }
        #endregion
    }


    public class TranslatorNote
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: string Category
        public string Category
        {
            get
            {
                return m_sCategory;
            }
            set
            {
                m_sCategory = value;
            }
        }
        private string m_sCategory;
        #endregion
        #region Attr{g/s}: string AssignedTo
        public string AssignedTo
        {
            get
            {
                return m_sAssignedTo;
            }
            set
            {
                m_sAssignedTo = value;
            }
        }
        private string m_sAssignedTo = "";
        #endregion
        #region Attr{g/s}: string Context
        public string Context
        {
            get
            {
                return m_sContext;
            }
            set
            {
                m_sContext = value;
            }
        }
        private string m_sContext;
        #endregion
        #region Attr{g/s}: string Reference
        public string Reference
        {
            get
            {
                return m_sReference;
            }
            set
            {
                // We expect the reference of the form "003:016" (leading zeros)
                Debug.Assert(value.Length == 7);
                Debug.Assert(value[3] == ':');

                m_sReference = value;
            }
        }
        private string m_sReference;
        #endregion
        #region Attr{g}: List<Discussion> Discussions
        public List<Discussion> Discussions
        {
            get
            {
                Debug.Assert(null != m_vDiscussions);
                return m_vDiscussions;
            }
        }
        List<Discussion> m_vDiscussions;
        #endregion

        // Categories (static) ---------------------------------------------------------------
        #region SAttr{g/s}: List<string>  Categories
        static public List<string> Categories
        {
            get
            {
                Debug.Assert(null != s_vCategories);
                Debug.Assert(s_vCategories.Count > 0);
                return s_vCategories;
            }
        }
        static List<string> s_vCategories = null;
        #endregion
        #region SVAttr{g/s}: string DefaultCategory
        static public string DefaultCategory
        {
            get
            {
                Debug.Assert(s_iDefaultCategory >= 0);
                Debug.Assert(s_iDefaultCategory < Categories.Count);
                return Categories[s_iDefaultCategory];
            }
            set
            {
                for (int i = 0; i < Categories.Count; i++)
                {
                    if (Categories[i] == value)
                        s_iDefaultCategory = i;
                }
            }
        }
        static int s_iDefaultCategory;
        #endregion
        #region SMethod: void InitializeCategories()
        static public void InitializeCategories()
        {
            if (null == s_vCategories)
            {
                s_vCategories = new List<string>();
                s_vCategories.Add("Exegesis");
                s_vCategories.Add("Front Issue?");
                s_vCategories.Add("To Do");

                DefaultCategory = "To Do";
            }
        }
        #endregion
        #region SMethod: void AddCategory(string sCategory)
        static public void AddCategory(string sCategory)
        {
            // Make sure the categories have been initialized
            InitializeCategories();

            // Check to see if it is already there
            if (-1 != Categories.IndexOf(sCategory))
                return;

            // If it wasn't there, then add it
            Categories.Add(sCategory);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public TranslatorNote(string _sReference, string _sContext)
            : base()
        {
            // Discussions
            m_vDiscussions = new List<Discussion>();

            // Reference, we expect something of the form "003:016", the Set attr 
            // asserts for this
            Reference = _sReference;

            // Context string
            Context = _sContext;

            // Default Categories; expect to be overridden in the settings to something more
            // matching to the individual project.
            InitializeCategories();

            // Set the category to the default
            Category = DefaultCategory;

        }
        #endregion
        #region Method: bool ContentEquals(JObject)
        public bool ContentEquals(TranslatorNote tn)
        {
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
        #region Attr{g}: string SortKey
        public string SortKey
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

        // I/O -------------------------------------------------------------------------------
        #region I/O CONSTANTS
        const string c_sTag = "TranslatorNote";
        const string c_sAttrCategory = "Category";
        const string c_sAttrAssignedTo = "Assigned";
        const string c_sAttrContext = "Context";
        const string c_sAttrReference = "Ref";
        #endregion
        #region VAttr{g}: XElement ToXml
        public XElement ToXml
        {
            get
            {
                XElement x = new XElement(c_sTag);

                x.AddAttr(c_sAttrCategory, Category);
                x.AddAttr(c_sAttrAssignedTo, AssignedTo);
                x.AddAttr(c_sAttrContext, Context);
                x.AddAttr(c_sAttrReference, Reference);

                foreach(Discussion d in Discussions)
                    x.AddSubItem(d.ToXml);

                return x;
            }
        }
        #endregion
        #region SMethod: TranslatorNote CreateFromXml(XElement)
        static public TranslatorNote CreateFromXml(XElement x)
        {
            if (x.Tag != c_sTag)
                return null;

            TranslatorNote tn = new TranslatorNote(
                x.GetAttrValue(c_sAttrReference, ""),
                x.GetAttrValue(c_sAttrContext, "")
                );

            tn.Category = x.GetAttrValue(c_sAttrCategory, "");
            tn.AssignedTo = x.GetAttrValue(c_sAttrAssignedTo, "");

            foreach (XElement xSub in x.Items)
            {
                Discussion d = Discussion.CreateFromXml(xSub);
                if (null != d)
                    tn.Discussions.Add(d);
            }

            return tn;
        }
        #endregion
        #region SMethod: TranslatorNote ImportFromOldStyle(nChapter, nVerse, SfField)
        static public TranslatorNote ImportFromOldStyle(int nChapter, int nVerse, SfField field)
        {
            // The Category is derived from the marker
            string sCategory = G.GetLoc_NoteDefs("kGeneral", "General");
            switch (field.Mkr)
            {
                case "nt":
                    sCategory = G.GetLoc_NoteDefs("kGeneral", "General"); 
                    break;
                case "ntHint":
                    sCategory = G.GetLoc_NoteDefs("kHintForDaughter", 
                        "Hint for Drafting Daughters"); 
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
                    sCategory = G.GetLoc_NoteDefs("kOldVersion", "Old Versions"); 
                    break;
                case "ntBT":
                    sCategory = G.GetLoc_NoteDefs("kBT", "Back Translation"); 
                    break;
                case "ntgk":
                    sCategory = G.GetLoc_NoteDefs("kGreek", "Greek"); 
                    break;
                case "nthb":
                    sCategory = G.GetLoc_NoteDefs("kHebrew", "Hebrew"); 
                    break;
                case "ntcn":
                    sCategory = G.GetLoc_NoteDefs("kExegesis", "Exegesis"); 
                    break;
                default: // Not an old-style note
                    return null;
            }
            AddCategory(sCategory);

            // The author is set to "Old Note".
            string sAuthor = G.GetLoc_String("kOldStyleNote", "Old Note");

            // The date is set to today
            DateTime dtCreated = DateTime.Now;

            // Create the Reference in the form we expect it
            string sReference = nChapter.ToString("000") + ":" + 
                nVerse.ToString("000");

            // The note needs a context. We'll go with the first five words
            // of the note. Its messy in that it gets repeated when the note is
            // expanded, unless we clear the ShowHeaderWhenExpanded flag.
            string sContext = GetWordsRight(field.Data, 0, 5);

            // Create the Note. We will not have a context for it.
            TranslatorNote tn = new TranslatorNote(sReference, sContext);
            tn.Category = sCategory;

            // Add the discussion
            Discussion disc = new Discussion(sAuthor, dtCreated, field.Data);
            tn.Discussions.Add(disc);

            // Done
            return tn;
        }
        #endregion

        // Editor Support --------------------------------------------------------------------
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

        // View Construction -----------------------------------------------------------------
        #region Method: EContainer BuildNotesPaneView()
        public EContainer BuildNotesPaneView()
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

            // Add the Discussions
            foreach (Discussion disc in Discussions)
                eHeader.Append(disc.BuildNotesPaneView());

            return eHeader;
        }
        #endregion
    }
}
