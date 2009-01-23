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

// TODO: Get rid of the Discussion.Text attr, in favor of supporting as many runs
//     as desired. (Current implementation just does JParagraph.Runs[0]. That is,
//     exactly one Run, which must be a DBasicText.
// TODO: Can we accomplish the ToXml within the JObject mechanism? Surely so!

namespace OurWord.DataModel
{
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
            m_dtCreated = DateTime.Today;

            // The Default author
            Author = GetDefaultAuthor();
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

        // Author in Registry ----------------------------------------------------------------
        const string c_sRegistrySubkey = "TranslatorNotes";
        const string c_sRegistryName = "AuthorName";
        #region SMethod: string GetDefaultAuthor()
        static public string GetDefaultAuthor()
        {
            // Start with the name of the machine
            string sAuthor = Environment.MachineName;

            // It might have been overridden in the Registry
            string sAuthorInRegistry = JW_Registry.GetValue(c_sRegistrySubkey, c_sRegistryName, "");
            if (!string.IsNullOrEmpty(sAuthorInRegistry))
                sAuthor = sAuthorInRegistry;

            return sAuthor;
        }
        #endregion

        // View Construction -----------------------------------------------------------------
        #region Method: EContainer BuildNotesPaneView()
        public EContainer BuildNotesPaneView()
        {
            // Create a paragraph to hold the discussion text
            OWPara pDiscussionText = new OWPara(G.TTranslation.WritingSystemVernacular,
                G.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleNoteDiscussion),
                Paragraphs[0], Color.White, OWPara.Flags.IsEditable);

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


    public class TranslatorNote : JObject
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
        }
        #endregion

        // Categories (static) ---------------------------------------------------------------
        #region SAttr{g/s}: List<string>  Categories
        static public List<string> Categories
        {
            get
            {
                // Make sure the categories have been initialized
                InitializeCategories();

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
                // Make sure the categories have been initialized
                InitializeCategories();

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
            if (null == s_vCategories || s_vCategories.Count == 0)
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
            // Check to see if it is already there
            if (-1 != Categories.IndexOf(sCategory))
                return;

            // If it wasn't there, then add it
            Categories.Add(sCategory);
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
            Category = DefaultCategory;
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

        // I/O -------------------------------------------------------------------------------
        #region SMethod: TranslatorNote ImportFromOldStyle(nChapter, nVerse, SfField)
        static public TranslatorNote ImportFromOldStyle(
            int nChapter, int nVerse, SfField field)
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
            tn.Discussions.Append(disc);

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
