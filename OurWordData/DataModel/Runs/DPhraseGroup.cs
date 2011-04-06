#region ***** DPhraseGroup.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DPhraseGroup.cs
 * Author:  John Wimbish
 * Created: 31 Jan 2005, move to separate file 20 Jan 2010
 * Purpose: A bundle of PhraseLists
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using JWTools;
using OurWordData.DataModel.Annotations;

#endregion

namespace OurWordData.DataModel.Runs
{
    #region Class: DBasicText
    public class DBasicText : DRun
    {
        // Attrs -----------------------------------------------------------------------------
        #region JAttr{g}: DPhrases Phrases
        public DPhraseList<DPhrase> Phrases
        {
            get { return m_osPhrases; }
        }
        private readonly DPhraseList<DPhrase> m_osPhrases;
        #endregion
        #region JAttr{g}: DPhrases PhrasesBT
        public DPhraseList<DPhrase> PhrasesBT
        {
            get { return m_osPhrasesBT; }
        }
        private readonly DPhraseList<DPhrase> m_osPhrasesBT;
        #endregion

        // Derived / Const attrs -------------------------------------------------------------
        #region Attr{g}: DParagraph Paragraph
        public DParagraph Paragraph
        {
            get
            {
                Debug.Assert(null != Owner as DParagraph);
                return Owner as DParagraph;
            }
        }
        #endregion
        #region Attr{g}: string ContentsAsString
        public string ContentsAsString
        {
            get
            {
                string s = "";

                foreach (DPhrase phrase in Phrases)
                {
                    s += phrase.Text;
                }

                return s;
            }
        }
        #endregion
        #region VAttr{g}: int PhrasesLength
        public int PhrasesLength
        {
            get
            {
                int c = 0;

                foreach (DPhrase phrase in Phrases)
                    c += phrase.Text.Length;

                return c;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DBasicText()
        {
            m_osPhrases = new DPhraseList<DPhrase>("Phrase", this);
            m_osPhrasesBT = new DPhraseList<DPhrase>("PhraseBT", this);
        }
        #endregion
        #region Constructor(DBasicText)
        public DBasicText(DBasicText source)
            : this()
        {
            foreach (DPhrase p in source.Phrases)
                Phrases.Append(new DPhrase(p));

            foreach (DPhrase p in source.PhrasesBT)
                PhrasesBT.Append(new DPhrase(p));
        }
        #endregion
        #region Method: bool PhrasalContentsEquals(DBasicText)
        public bool PhrasalContentsEquals(DBasicText text)
        // Using this method in Merge, in anticipation of when I combine DText and
        // DBasicText into a single class; because at that time, ContentEquals
        // will need to compare owned translator notes, which would mess with the
        // merge logic.
        {
            if (Phrases.Count != text.Phrases.Count)
                return false;

            if (PhrasesBT.Count != text.PhrasesBT.Count)
                return false;

            for (int i = 0; i < Phrases.Count; i++)
            {
                DPhrase phrase1 = Phrases[i] as DPhrase;
                DPhrase phrase2 = text.Phrases[i] as DPhrase;

                if (false == phrase1.ContentEquals(phrase2))
                    return false;
            }

            for (int i = 0; i < PhrasesBT.Count; i++)
            {
                DPhrase phrase1 = PhrasesBT[i] as DPhrase;
                DPhrase phrase2 = text.PhrasesBT[i] as DPhrase;

                if (false == phrase1.ContentEquals(phrase2))
                    return false;
            }

            return true;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            DBasicText text = obj as DBasicText;
            if (null == text)
                return false;

            if (Phrases.Count != text.Phrases.Count)
                return false;

            if (PhrasesBT.Count != text.PhrasesBT.Count)
                return false;

            for (int i = 0; i < Phrases.Count; i++)
            {
                DPhrase phrase1 = Phrases[i] as DPhrase;
                DPhrase phrase2 = text.Phrases[i] as DPhrase;

                if (false == phrase1.ContentEquals(phrase2))
                    return false;
            }

            for (int i = 0; i < PhrasesBT.Count; i++)
            {
                DPhrase phrase1 = PhrasesBT[i] as DPhrase;
                DPhrase phrase2 = text.PhrasesBT[i] as DPhrase;

                if (false == phrase1.ContentEquals(phrase2))
                    return false;
            }

            return true;
        }
        #endregion
        #region Method: void Cleanup()
        public void Cleanup()
        // Make sure we have at least one phrase
        {
            if (Phrases.Count == 0)
                Phrases.Append(new DPhrase(""));
            if (PhrasesBT.Count == 0)
                PhrasesBT.Append(new DPhrase(""));
        }
        #endregion
        #region Method: void CopyDataTo(DBasicText DBT)
        public void CopyDataTo(DBasicText DBT)
        {
            DBT.Phrases.Clear();
            foreach (DPhrase p in Phrases)
                DBT.Phrases.Append(new DPhrase(p));

            DBT.PhrasesBT.Clear();
            foreach (DPhrase p in PhrasesBT)
                DBT.PhrasesBT.Append(new DPhrase(p));
        }
        #endregion

        // DRun Override Scaffolding ---------------------------------------------------------
        #region Attr{g}: char TypeCode - a single character representing the subclass type
        public override char TypeCode
        {
            get
            {
                return c_codeNormal;
            }
        }
        #endregion
        #region Attr{g}: string DebugString
        public override string DebugString
        {
            get
            {
                string s = "";

                foreach (DPhrase p in Phrases)
                    s += p.SfmSaveString;

                string sBT = "";
                foreach (DPhrase p in PhrasesBT)
                    sBT += p.SfmSaveString;
                if (sBT.Length > 0)
                    s += "{BT " + sBT + "}";

                return s;
            }
        }
        #endregion
        #region Attr{g}: string ContentsSfmSaveString
        public override string ContentsSfmSaveString
        {
            get
            {
                return Phrases.ToSaveString;
            }
        }
        #endregion
        #region Attr{g}: string ProseBTSfmSaveString
        public override string ProseBTSfmSaveString
        {
            get
            {
                return PhrasesBT.ToSaveString;
            }
        }
        #endregion
        #region Attr{g}: override string AsString
        public override string AsString
        {
            get
            {
                return Phrases.AsString;
            }
        }
        #endregion
        #region Attr{g}: override string ProseBTAsString
        public override string ProseBTAsString
        {
            get
            {
                return PhrasesBT.AsString;
            }
        }
        #endregion
        #region OMethod: DRun ConvertCrossRefs(bsaSourceSubs, bsaDestSubs)
        public override DRun ConvertCrossRefs(
            BStringArray bsaSourceSubstitutions,
            BStringArray bsaDestSubstitutions)
        {
            // We assume a simple, one-phrase text
            string sSource = "";
            foreach (DPhrase phr in Phrases)
                sSource += phr.Text;

            // Create the converted string
            int i = 0;
            string sDest = "";

            // Loop through the source string, comparing for matches
            while (i < sSource.Length)
            {
                // Look for a match from amongst the book names
                int iBookName = bsaSourceSubstitutions.FindSubstringMatch(sSource, i, true);

                // If not found, add the current character and move on to the next one
                if (-1 == iBookName)
                {
                    sDest += sSource[i++];
                    continue;
                }

                // Else it was found; so make the substitution.
                sDest += bsaDestSubstitutions[iBookName];
                i += bsaSourceSubstitutions[iBookName].Length;
            }

            return DText.CreateSimple(sDest);
        }
        #endregion

        // Oxes ------------------------------------------------------------------------------
        const string c_sAttrBackTranslation = "bt";
        #region Method: void ReadOxesPhrase(XmlNode node)
        public void ReadOxesPhrase(XmlNode node)
        // With well-formed data, we always expect to be adding a phrase here, as opposed
        // to appending data to an existing phrase.
        {
            // Back translation
            if (node.Name == c_sAttrBackTranslation)
            {
                foreach (XmlNode child in node.ChildNodes)
                    PhrasesBT.ReadOxesPhrase(child);
                return;
            }

            // Vernacular text
            Phrases.ReadOxesPhrase(node);
        }
        #endregion
        #region OMethod: XmlNode SaveToOxesBook(oxes, nodeParagraph)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParagraph)
        {
            Phrases.SaveToOxesBook(oxes, nodeParagraph);

            if (PhrasesBT.Count > 0 && !string.IsNullOrEmpty(ProseBTAsString))
            {
                var nodeBT = oxes.AddNode(nodeParagraph, c_sAttrBackTranslation);
                PhrasesBT.SaveToOxesBook(oxes, nodeBT);
            }

            return nodeParagraph;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: override void EliminateSpuriousSpaces()
        public override void EliminateSpuriousSpaces()
        {
            Phrases.EliminateSpuriousSpaces();
            PhrasesBT.EliminateSpuriousSpaces();
        }
        #endregion
        #region Method: void Append(DBasicText text)
        public virtual void Append(DBasicText text, bool bInsertSpacesBetweenPhrases)
        {
            // AddParagraph the vernacular and BT phrases
            Phrases.AppendPhrases(text.Phrases, bInsertSpacesBetweenPhrases);
            PhrasesBT.AppendPhrases(text.PhrasesBT, bInsertSpacesBetweenPhrases);
        }
        #endregion
        #region Method: void GetWordOffsetPairs(ref aWords, ref aPositions)
        public void GetWordOffsetPairs(ref ArrayList aWords, ref ArrayList aPositions)
        //
        // Given:
        //     0123456789 123456789 123456789 1234567
        //    "The brown fox jumped over the lazy dog."
        //
        // Returns
        //     0  "The"
        //     4  "brown"
        //    10  "fox"
        //    14  "jumped"
        //    21  "over"
        //    26  "the"
        //    30  "lazy"
        //    35  "dog."
        {
            // Make sure the arrays are empty
            aWords.Clear();
            aPositions.Clear();

            // Collect a single string that is the sum of all of the phrases
            string sText = "";
            foreach (DPhrase p in Phrases)
                sText += p.Text;
            sText = sText.Trim();
            int iPos = 0;

            // TODO: Eventually, we should get these from the Language information
            char[] aWordBoundary = new char[1];
            aWordBoundary[0] = ' ';

            while (sText.Length > 0)
            {
                // Determine the end of the next word
                int iPosEnd = sText.IndexOfAny(aWordBoundary);
                if (-1 == iPosEnd)
                    iPosEnd = sText.Length;

                // Retrieve that next word, then remove any punctuation and convert it
                // to lower case. If, when we are done, we have a non-zero-length string,
                // then add it to our array.
                string sAdd = sText.Substring(0, iPosEnd).Trim();
                bool bHasNonPunct = false;
                foreach (char ch in sAdd)
                {
                    if (!Char.IsPunctuation(ch))
                        bHasNonPunct = true;
                }
                if (sAdd.Length > 0 && bHasNonPunct)
                {
                    aWords.Add(sAdd);
                    aPositions.Add(iPos);
                }

                // Remove the word from our source string, so the loop will look at the
                // next one.
                sText = sText.Substring(iPosEnd);
                iPos += iPosEnd;
                while (sText.Length > 0 && 0 == sText.IndexOfAny(aWordBoundary))
                {
                    iPos++;
                    sText = sText.Substring(1);
                }

            }
        }
        #endregion
        #region Method: override void CopyBackTranslationsFromFront(DRun RFront)
        public override void CopyBackTranslationsFromFront(DRun runFront, bool bReplaceTarget)
        {
            var textFront = runFront as DBasicText;
            if (null == textFront)
                return;

            // Replace Mode means we get rid of existing BT phrases
            if (bReplaceTarget)
                PhrasesBT.Clear();

            // If we have a phrase still in the target (which means we're in kAppendToTarget mode),
            // then add a space to it.
            if (PhrasesBT.Count > 0)
            {
                var phr = PhrasesBT[PhrasesBT.Count - 1];
                phr.Text += " ";
            }

            // Add the Front phrases
            foreach (DPhrase phraseFront in textFront.PhrasesBT)
            {
                var phraseTarget = new DPhrase(phraseFront);
                PhrasesBT.Append(phraseTarget);
            }

            // Eliminate extra spaces this may have created
            PhrasesBT.EliminateSpuriousSpaces();
        }
        #endregion

        #region Method: void Join(int iPhraseLeft)
        /// <summary>
        /// Joins the phrase as iPhraseLeft with the phrase to its right. The resultant
        /// phrase keeps the type of the iPhraseLeft
        /// </summary>
        /// <param name="iPhraseLeft">The index of the left phrase; it will be joined
        /// to the one on its right.</param>
        public void Join(int iPhraseLeft)
        {
            // Make certain a Join operation is supported at this position
            Debug.Assert(iPhraseLeft >= 0);
            Debug.Assert(iPhraseLeft < Phrases.Count - 1);

            // Point to the two phrases in question
            DPhrase phraseLeft = Phrases[iPhraseLeft] as DPhrase;
            DPhrase phraseRight = Phrases[iPhraseLeft + 1] as DPhrase;
            Debug.Assert(null != phraseLeft);
            Debug.Assert(null != phraseRight);

            // Move the contents of the Right phrase into the Left
            phraseLeft.Text += phraseRight.Text;

            // Remove the Right phrase
            Phrases.Remove(phraseRight);
        }
        #endregion
    }
    #endregion

    #region Class: DText
    public class DText : DBasicText
    {
        // Translator Notes ------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq<TranslatorNote> TranslatorNotes
        public JOwnSeq<TranslatorNote> TranslatorNotes
        {
            get
            {
                Debug.Assert(null != m_osTranslatorNotes);
                return m_osTranslatorNotes;
            }
        }
        readonly JOwnSeq<TranslatorNote> m_osTranslatorNotes;
        #endregion

        // Oxes ------------------------------------------------------------------------------
        #region OMethod: XmlNode SaveToOxesBook(XmlDoc, nodeParagraph)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeParagraph)
        {
            // Save the phrase data
            base.SaveToOxesBook(oxes, nodeParagraph);

            // Save the notes
            foreach (TranslatorNote tn in TranslatorNotes)
                tn.Save(oxes, nodeParagraph);

            return nodeParagraph;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor() - Creates the attributes
        public DText()
            : base()
        {
            // Translator Notes
            m_osTranslatorNotes = new JOwnSeq<TranslatorNote>("tn", this, false, true);
        }
        #endregion
        #region Method: static public DText CreateSimple()
        static public DText CreateSimple()
        {
            return DText.CreateSimple("", "");
        }
        #endregion
        #region Method: static public DText CreateSimple(string sPhraseText)
        static public DText CreateSimple(string sPhraseText)
        {
            return DText.CreateSimple(sPhraseText, "");
        }
        #endregion
        #region Method: static public DText CreateSimple(string sPhraseText, string sPhraseTextBT)
        static public DText CreateSimple(string sPhraseText, string sPhraseTextBT)
        {
            var text = new DText();
            text.Phrases.Append(new DPhrase(sPhraseText));
            text.PhrasesBT.Append(new DPhrase(sPhraseTextBT));
            return text;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            DText text = obj as DText;
            if (null == text)
                return false;

            return base.ContentEquals(obj);
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void Append(DText text)
        public override void Append(DBasicText basicText, bool bInsertSpacesBetweenPhrases)
        {
            DText text = basicText as DText;
            Debug.Assert(null != text);

            // add the vernacular and BT phrases
            base.Append(text, bInsertSpacesBetweenPhrases);

            // Move the Translator Notes
            // We do this tediously, one-at-a-time, as we must remove from the source
            // (which null's the owner), then add to the destination (which gives it an
            // owner.) 
            while (text.TranslatorNotes.Count > 0)
            {
                TranslatorNote tn = text.TranslatorNotes[0];
                text.TranslatorNotes.Remove(tn);
                TranslatorNotes.Append(tn);
            }
        }
        #endregion
        #region Method: override void ToSfm(ScriptureDB DB)
        public override void ToSfm(ScriptureDB DB)
        {
            DB.Append(new SfField(DB.Map.MkrVerseText, ContentsSfmSaveString,
                ProseBTSfmSaveString, IntBTSfmSaveString));

            // Translator Notes
            foreach (TranslatorNote tn in TranslatorNotes)
            {
                // We're doing this temporary ToSfm thing for now. Later, the new SfField
                // line is all we'll need.
                tn.AddToSfmDB(DB);
            }

        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region SMethod: string GetNoteContext(sOurs, sTheirs)
        const int c_nMaxContextLength = 10;
        static public string GetNoteContext(string sOurs, string sTheirs)
        {
            // Scan forward until we get a difference
            var iStart = 0;
            while (iStart < sOurs.Length &&
                iStart < sTheirs.Length &&
                sOurs[iStart] == sTheirs[iStart])
            {
                iStart++;
            }

            // Scan backward from the end until we get a difference
            var iEnd = sOurs.Length - 1;
            var iTheirEnd = sTheirs.Length - 1;
            while (iEnd >= 0 &&
                iTheirEnd >= 0 &&
                sOurs[iEnd] == sTheirs[iTheirEnd])
            {
                iEnd--;
                iTheirEnd--;
            }

            // Everything in-between Start and End is where the differences were found
            var sContext = "";
            for (var i = iStart; i <= iEnd && sContext.Length < c_nMaxContextLength; i++)
                sContext += sOurs[i];

            // We're not wanting to fool with whitespace
            sContext = sContext.Trim();

            // If Theirs is empty, then all of Ours is different. We want an elipses if
            // it is longer than what we just extracted
            if (string.IsNullOrEmpty(sTheirs))
            {
                if (sOurs.Length > c_nMaxContextLength)
                    sContext += "...";
                return sContext;
            }

            // If we have no differences, it is because the changes were appended either
            // to the front or to the end
            if (string.IsNullOrEmpty(sContext))
            {
                // If iStart is zero, it means Theirs was different immediately, indicating
                // a prepend
                if (iStart == 0)
                {
                    for (var k = 0; k < sOurs.Length && k < c_nMaxContextLength; k++)
                        sContext += sOurs[k];
                    if (sContext.Length == c_nMaxContextLength)
                        sContext = sContext.Trim() + "...";
                }

                // If we still have nothing, then extract the end of the string. In some
                // cases this will be correct because iEnd will not have moved, signalling
                // something was post-pended to Theirs.
                if (string.IsNullOrEmpty(sContext))
                {
                    for (int k = sOurs.Length - 1;
                        k >= 0 && sContext.Length < c_nMaxContextLength;
                        k--)
                    {
                        sContext = sOurs[k] + sContext;
                    }
                    if (sContext.Length == c_nMaxContextLength)
                        sContext = "..." + sContext.Trim();
                }
            }

            return sContext.Trim();
        }
        #endregion
        #region SMethod: string GetConflictMergeNoteContents(sParent, sOurs, sTheirs)
        static public string GetConflictMergeNoteContents(string sParent, string sOurs, string sTheirs)
        {
            var sParentChanged = sParent;
            var sOursChanged = sOurs;
            var sTheirsChanged = sTheirs;

            const int minimumLength = 12;

            // Remove what they have in common at their beginning
            while (!string.IsNullOrEmpty(sParentChanged) &&
                !string.IsNullOrEmpty(sOursChanged) &&
                !string.IsNullOrEmpty(sTheirsChanged))
            {
                if (sParentChanged.Length < minimumLength ||
                    sOursChanged.Length < minimumLength ||
                    sTheirsChanged.Length < minimumLength)
                    break;

                if (sParentChanged[0] != sOursChanged[0])
                    break;
                if (sParentChanged[0] != sTheirsChanged[0])
                    break;

                sParentChanged = sParentChanged.Substring(1);
                sOursChanged = sOursChanged.Substring(1);
                sTheirsChanged = sTheirsChanged.Substring(1);
            }

            // Remove what they have in common at their ending
            while (!string.IsNullOrEmpty(sParentChanged) &&
                !string.IsNullOrEmpty(sOursChanged) &&
                !string.IsNullOrEmpty(sTheirsChanged))
            {
                if (sParentChanged.Length < minimumLength ||
                    sOursChanged.Length < minimumLength ||
                    sTheirsChanged.Length < minimumLength)
                    break;

                var iParent = sParentChanged.Length - 1;
                var iOurs = sOursChanged.Length - 1;
                var iTheirs = sTheirsChanged.Length - 1;

                if (sParentChanged[iParent] != sOursChanged[iOurs])
                    break;
                if (sParentChanged[iParent] != sTheirsChanged[iTheirs])
                    break;

                sParentChanged = sParentChanged.Substring(0, iParent);
                sOursChanged = sOursChanged.Substring(0, iOurs);
                sTheirsChanged = sTheirsChanged.Substring(0, iTheirs);
            }

            // Build the string
            var sContents = string.Format("Merge Conflict: Original was \"{0}\"; Ours was \"{1}\"; Theirs was \"{2}\"",
                sParentChanged, sOursChanged, sTheirsChanged);

            return sContents;
        }
        #endregion
        #region Method:  void Merge(DBasicText Parent, DBasicText Theirs)
        public void Merge(DBasicText Parent, DBasicText Theirs)
        {
            Phrases.Do3WayMerge(Parent.Phrases, Theirs.Phrases);
            PhrasesBT.Do3WayMerge(Parent.PhrasesBT, Theirs.PhrasesBT);
        }
        #endregion
    }
    #endregion



}
