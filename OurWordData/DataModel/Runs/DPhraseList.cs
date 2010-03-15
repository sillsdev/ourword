#region ***** DPhraseList.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DPhraseList.cs
 * Author:  John Wimbish
 * Created: 31 Jan 2005, move to separate file 19 Jan 2010
 * Purpose: A collection of DPhrase's
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using JWTools;
using OurWordData.DataModel.Annotations;
using OurWordData.Tools;
#endregion

namespace OurWordData.DataModel.Runs
{
    public class DPhraseList<T> : JOwnSeq<T> where T:DPhrase
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sName, objOwner)
        public DPhraseList(string sName, JObject objOwner)
            : base(sName, objOwner, false, false)
        {
        }
        #endregion
        #region DPhrase Indexer[]
        public new T this[int index]
        {
            get
            {
                return base[index];
            }
        }
        #endregion

        // Conversions -----------------------------------------------------------------------
        #region VAttr{g}: string AsString
        public string AsString
        {
            get
            {
                var s = "";
                foreach (DPhrase p in this)
                    s += p.Text;
                return s;
            }
        }
        #endregion
        #region VAttr{g}: DPhrase[] AsVector
        public DPhrase[] AsVector
        {
            get
            {
                var v = new DPhrase[Count];
                for (var i = 0; i < Count; i++)
                    v[i] = this[i];
                return v;
            }
        }
        #endregion

        // Retrieval -------------------------------------------------------------------------
        #region Method: int GetPosInPhrase(iPosInPhrases)
        private int GetPosInPhrase(int iPosInPhrases)
        {
            var iPos = iPosInPhrases;
            foreach (DPhrase phrase in this)
            {
                if (phrase.Text.Length > iPos)
                    return iPos;
                iPos -= phrase.Text.Length;
            }
            return iPos;
        }
        #endregion
        #region Method: DPhrase GetPhraseAt(iPos)
        private DPhrase GetPhraseAt(int iPos)
        {
            foreach (DPhrase phrase in this)
            {
                if (phrase.Text.Length > iPos)
                    return phrase;
                iPos -= phrase.Text.Length;
            }

            if (iPos == 0 && Count > 0)
                return this[Count - 1];

            return null;
        }
        #endregion
        #region VAttr{g}: DPhrase LastPhrase
        private DPhrase LastPhrase
        {
            get
            {
                return (Count > 0) ? this[Count - 1] : null;
            }
        }
        #endregion
        #region VAttr{g}: DPhrase FirstPhrase
        private DPhrase FirstPhrase
        {
            get
            {
                return (Count > 0) ? this[0] : null;
            }
        }
        #endregion

        // Operations ------------------------------------------------------------------------
        #region Method: void CombineSameStyledPhrases()
        private void CombineSameStyledPhrases()
        {
            for (var i = 0; i < Count - 1; )
            {
                var left = this[i] as DPhrase;
                var right = this[i + 1] as DPhrase;

                if (left.FontToggles == right.FontToggles)
                {
                    left.Text += right.Text;
                    Remove(right);
                }
                else
                    i++;
            }
        }
        #endregion
        #region Method: DPhrase Split(DPhrase, iPos)
        /// <summary>
        /// Splits the phrase, iff the position requested is not at a phrase boundary. 
        /// Otherwise, nothing is done.
        /// </summary>
        /// <param name="phraseToSplit">The phrase to be split into two</param>
        /// <param name="iPos">The position within the phrase. It must be greater than
        /// zero, and less than the length of the phrase; otherwise no action is taken.</param>
        public DPhrase Split(DPhrase phraseToSplit, int iPos)
        {
            if (iPos == 0 || iPos == phraseToSplit.Text.Length)
                return phraseToSplit;

            var phraseLeft = new DPhrase(phraseToSplit.Text.Substring(0, iPos)) 
                { FontToggles = phraseToSplit.FontToggles };

            var phraseRight = new DPhrase(phraseToSplit.Text.Substring(iPos)) 
                { FontToggles = phraseToSplit.FontToggles };

            var iPhrasePosition = FindObj(phraseToSplit);

            InsertAt(iPhrasePosition, phraseRight);
            InsertAt(iPhrasePosition, phraseLeft);
            Remove(phraseToSplit);
            return phraseRight;
        }
        #endregion
        #region Method: void Delete(iStart, iCount)
        #region Helper: void _DeleteChar(iStart)
        void _DeleteChar(int iStart)
        // WARNING: We can't make this public, because it does not join up phrases; so the
        // caller should instead use the general-purpose Delete() method below.
        {
            if (iStart < 0)
                throw new ArgumentOutOfRangeException("iStart", "In DBasicText.DeleteChar");

            // Locate the phrase and the position with it where the deletion is to start
            var iPhrase = 0;
            DPhrase phrase = this[iPhrase];
            while (iStart >= phrase.Text.Length)
            {
                iStart -= phrase.Text.Length;
                iPhrase++;
                if (iPhrase == Count)
                    throw new ArgumentOutOfRangeException("iStart", "Too long in DBasicText.DeleteChar");
                phrase = this[iPhrase];
            }

            // Delete the character 
            phrase.Text = phrase.Text.Remove(iStart, 1);

            // If this just created an empty phrase, then we must remove it
            if (phrase.Text.Length == 0)
                Remove(phrase);

            // If we now have two phrases of the same type, then we must combine them
            for (var i = 0; i < Count - 1; )
            {
                DPhrase phraseLeft = this[i];
                DPhrase phraseRight = this[i + 1];

                if (phraseLeft.FontToggles == phraseRight.FontToggles)
                {
                    phraseLeft.Text += phraseRight.Text;
                    Remove(phraseRight);
                }
                else
                {
                    i++;
                }
            }
        }
        #endregion

        public void Delete(int iStart, int iCount, bool bCleanUpDoubleSpacing)
        {
            // Do the requested deletion of iCount characters
            while (iCount > 0)
            {
                _DeleteChar(iStart);
                iCount--;
            }

            // This has the potential to place two spaces together, in which case we
            // must delete the forward one. Thus if we were at "_|e_" and hit delete,
            // we'll now be at "_|_". We do this through a recursive call.
            if (bCleanUpDoubleSpacing)
            {
                var s = "";
                foreach (DPhrase p in this)
                    s += p.Text;
                var iDouble = s.IndexOf("  ");
                if (iDouble != -1 && iDouble == iStart - 1)
                    _DeleteChar(iStart);
            }

            // If everything is deleted, we should at a minimum have a single, empty DPhrase
            if (Count == 0)
                Append(new DPhrase(""));
        }

        public void Delete(int iStart, int iCount)
        {
            Delete(iStart, iCount, true);
        }
        #endregion
        #region Method: ToggleItalics(iStart, iCount)
        public void ToggleItalics(int iStart, int iCount)
        {
            FontStyle newToggleValue;

            // There may be multiple phrases within the selection. If they are of different
            // toggle states, then we'll want to make them all the same, which will be to make
            // them the opposite of the underlying paragraph
            var bIsMixed = false;
            for (var i = iStart; i < iStart + iCount; i++)
            {
                var phrase = GetPhraseAt(i);
                if (!phrase.ItalicIsToggled)
                    bIsMixed = true;
            }
            if (bIsMixed)
                newToggleValue = FontStyle.Italic;

            // Otherwise, we want to do the reverse of whatever the phrase(s) are
            else
            {
                newToggleValue = (GetPhraseAt(iStart).ItalicIsToggled) ? 
                    FontStyle.Regular : FontStyle.Italic;
            }

            // Make a new phrase to house the Italics
            var s = AsString.Substring(iStart, iCount);
            var phraseItalic = new DPhrase(s) { FontToggles = newToggleValue };

            // Remove the corresponding text from the existing DPhrases
            Delete(iStart, iCount, false);

            // If we're at the end, just append the phrase
            if (iStart == AsString.Length)
            {
                Append(phraseItalic);
            }
            // Otherwise, split and insert
            else
            {
                // Split the phrase into two so we can insert between them.
                var phraseToSplit = GetPhraseAt(iStart);
                var iPosInPhrase = GetPosInPhrase(iStart);
                var phraseRight = Split(phraseToSplit, iPosInPhrase);

                // If at the beginning of the paragraph, the insert position is the 0th position;
                // otherwise it is the one after the phrase we just split. (I.e., The split didn't
                //  happenat the 0th position.)
                var iInsertPos = FindObj(phraseRight);

                // Insert our new phrase
                InsertAt(iInsertPos, phraseItalic);
            }

            CombineSameStyledPhrases();
        }
        #endregion
        #region Method: void RemoveEmptyPhrases()
        private void RemoveEmptyPhrases()
        {
            for(var i=0; i<Count; )
            {
                var phrase = this[i];
                if (string.IsNullOrEmpty(phrase.Text))
                    Remove(phrase);
                else
                    i++;
            }
        }
        #endregion
        #region Method: void CombinePhrases()
        private void CombinePhrases()
        {
            for(var i=0; i<Count-1; )
            {
                var phrase1 = this[i];
                var phrase2 = this[i + 1];

                if (phrase1.FontToggles == phrase2.FontToggles)
                {
                    phrase1.Text += phrase2.Text;
                    Remove(phrase2);
                }
                else
                {
                    i++;
                }
            }
        }
        #endregion
        #region Method: void EliminateSpuriousSpaces()
        /// <summary>
        /// Performs various cleanup activities on the sequence of runs: (1) eliminating
        /// double spaces, (2) eliminating leading or trailing spaces for the DText, and
        /// (3) eliminating empty DPhrases.
        /// </summary>
        public void EliminateSpuriousSpaces()
        {
            RemoveEmptyPhrases();
            CombinePhrases();

            // Eliminate where one phrase ends with a space and the next begins with one
            for (var i = 0; i < Count - 1; i++)
            {
                var phraseLeft = this[i];
                var phraseRight = this[i + 1];

                if (!phraseLeft.HasText || !phraseRight.HasText)
                    continue;

                while (phraseLeft.EndsWithSpace && phraseRight.BeginsWithSpace)
                    phraseRight.RemoveLeadingSpace();
            }

            // Make sure the first phrase does not begin with a leading space
            if (Count > 0)
            {
                var phrase = this[0];
                while (phrase.BeginsWithSpace)
                    phrase.RemoveLeadingSpace();
            }

            // Make sure the final phrase does not end with a space
            if (Count > 0)
            {
                var phrase = this[Count - 1];
                while (phrase.EndsWithSpace)
                    phrase.RemoveTrailingSpace();
            }

            // Make sure there is at least one phrase present
            if (Count == 0)
                Append(new DPhrase(""));
        }
        #endregion
        #region Method: void AppendPhrases(DPhraseList<DPhrase>, bInsertSpacesBetweenPhrases)
        public void AppendPhrases(DPhraseList<DPhrase> other, bool bInsertSpacesBetweenPhrases)
        {
            // Add a space to our last phrase if needed
            if (bInsertSpacesBetweenPhrases)
            {
                if (null != LastPhrase && !LastPhrase.EndsWithSpace &&
                    null != other.FirstPhrase && !other.FirstPhrase.BeginsWithSpace)
                {
                    LastPhrase.Text += " ";
                }
            }

            // Add in the phrases
            while (null != other.FirstPhrase)
            {
                var phrase = other.FirstPhrase;
                other.RemoveAt(0);

                if (null != LastPhrase && phrase.FontToggles == LastPhrase.FontToggles)
                {
                    LastPhrase.Text += phrase.Text;
                }
                else
                {
                    Append(phrase);
                }
            }
        }
        #endregion

        // Standard Format I/O ---------------------------------------------------------------
        #region Attr{g}: string ToSaveString
        public string ToSaveString
        {
            get
            {
                // Start with an empty string
                var s = "";

                // Add the phrases
                foreach (DPhrase phrase in this)
                    s += phrase.SfmSaveString;

                // Return the result
                return s;
            }
        }
        #endregion
        #region Method: void FromSaveString(string s)
        public void FromSaveString(string s)
        {
            // We'll build a list of transitional phrase objects here
            var v = new List<DSection.IO.Phrase>();

            // Parse the string into phrase objects
            var iPos = 0;
            DSection.IO.Phrase p;
            while ((p = DSection.IO.Phrase.GetPhrase(s, ref iPos)) != null)
                v.Add(p);

            // Create DPhrases from these
            Clear();
            foreach (var phrase in v)
            {
                Append(new DPhrase(phrase.Text) { FontToggles = phrase.FontToggles });
            }

            // At a minimum we want at least one empty phrase. If "s" was empty
            // when passed in, then we will not have this.
            if (Count == 0)
                Append(new DPhrase(""));
        }
        #endregion

        public string GetRtf()
        {
            return AsString;
        }

        // Oxes --------------------------------------------------------------------------
        #region Oxes Constants
        const string c_sTagSpan = "span";
        const string c_sAttrStyle = "class";

        private const string c_sModItalic = "Italic";
        private const string c_sModBold = "Bold";
        private const string c_sModUnderline = "Underline";
        #endregion
        #region Method: void ReadOxesPhrase(XmlNode)
        public void ReadOxesPhrase(XmlNode node)
        {
            // If we have a Span, then get its style mod
            var toggles = FontStyle.Regular;
            if (node.Name == c_sTagSpan)
            {
                var sMods = XmlDoc.GetAttrValue(node, c_sAttrStyle, "");
                switch (sMods)
                {
                    case c_sModItalic:
                        toggles = FontStyle.Italic;
                        break;
                    case c_sModBold:
                        toggles = FontStyle.Bold;
                        break;
                    case c_sModUnderline:
                        toggles = FontStyle.Underline;
                        break;
                }
            }

            var phrase = new DPhrase(node.InnerText) { FontToggles = toggles };
            Append(phrase);
        }
        #endregion
        #region Method: XmlNode SaveToOxesBook(oxes, nodeParagraph)
        public void SaveToOxesBook(XmlDoc oxes, XmlNode nodeParagraph)
        {
            if (Count == 0 || string.IsNullOrEmpty(AsString))
                return;

            foreach (DPhrase phrase in this)
            {
                var nodeParent = nodeParagraph;

                if (phrase.FontToggles != FontStyle.Regular)
                {
                    nodeParent = oxes.AddNode(nodeParent, c_sTagSpan);

                    var sModification = "";
                    if (phrase.BoldIsToggled)
                        sModification = c_sModBold;
                    if (phrase.ItalicIsToggled)
                        sModification = c_sModItalic;
                    if (phrase.UnderlineIsToggled)
                        sModification = c_sModUnderline;

                    oxes.AddAttr(nodeParent, c_sAttrStyle, sModification);
                }

                oxes.AddText(nodeParent, phrase.Text);
            }
        }
        #endregion

        // Merging -----------------------------------------------------------------------
        #region Method: void InsertConflictNote(parentPhrases, theirPhrases)
        void InsertConflictNote(DPhraseList<T> parentPhrases, DPhraseList<T> theirPhrases)
        {
            // The place to put the ConflictNote is our owner
            var text = Owner as DText;
            Debug.Assert(null != text);

            var note = new TranslatorNote
                {
                    SelectedText = DText.GetNoteContext(AsString, theirPhrases.AsString)
                };

            var sMessageContents = DText.GetConflictMergeNoteContents(parentPhrases.AsString,
                 AsString, theirPhrases.AsString);

            var message = new DMessage(TranslatorNote.MergeAuthor,
                DateTime.Now, Role.Anyone, sMessageContents);
            note.Messages.Append(message);

            text.TranslatorNotes.Append(note);

            //var reference = text.Paragraph.ReferenceSpan.DisplayName;
            //LogTheChange(reference, parentPhrases.AsString, this.AsString, theirPhrases.AsString);
        }
        #endregion
        #region Method: void Do3WayMerge(parentPhrases, theirPhrases)
        public void Do3WayMerge(DPhraseList<T> parentPhrases, DPhraseList<T> theirPhrases)
        // Returns true if able to resolve the differences
        {
            // We'll examine as flat strings
            var parent = parentPhrases.ToSaveString;
            var theirs = theirPhrases.ToSaveString;
            var ours = ToSaveString;

            // If one is equal to parent, but the other changed, then keep the changed one
            if (parent.CompareTo(theirs) == 0)
                return;
            if (parent.CompareTo(ours) == 0)
            {
                FromSaveString(theirs);
                return;
            }

            // If both changed in the same way (e.g., both fixed a typo), then keep ours
            if (ours.CompareTo(theirs) == 0)
                return;

            // If here, both changed in different ways.
            var bMergeSuccess = StringMerger.Merge3Way(ref parent, ref ours, theirs);
            parentPhrases.FromSaveString(parent);
            FromSaveString(ours);
            if (bMergeSuccess)
                return;

            // If we have a footnote we can't resolve, we have to give up; because 
            // Standard Format doesn't support notes on footnotes
            var text = Owner as DText;
            if (null != text && text.Owner as DFootnote != null)
                return;

            // Give up and create a ConflictNote for the user
            InsertConflictNote(parentPhrases, theirPhrases);
        }
        #endregion
        #region SMethod: LogTheChange(...)
        /*
        static void LogTheChange(string reference, string parent, string ours, string theirs)
        {
            try
            {
                const string path = "C:\\Users\\JWimbish\\Documents\\MergeLog.txt";
                var sw = new StreamWriter(path, true);
                var w = TextWriter.Synchronized(sw);

                w.WriteLine("---------------------------------------");
                w.WriteLine(reference);
                w.WriteLine("Parent  =" + parent);
                w.WriteLine("Ours    =" + ours);
                w.WriteLine("Theirs  =" + theirs);
                w.WriteLine("");

                w.Flush();
                w.Close();
            }
            catch (Exception)
            {
            }
        }
        */
        #endregion
    }
}
