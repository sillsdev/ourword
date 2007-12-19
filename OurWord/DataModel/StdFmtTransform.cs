/**********************************************************************************************
 * Project: JWdb
 * File:    StdFmtTransform.cs
 * Author:  John Wimbish
 * Created: 25 Nov 2004
 * Purpose: Reads/writes a standard format database, handles SF irregularities
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using JWTools;
using JWdb;
using OurWord.DataModel;
using NUnit.Framework;
#endregion

namespace OurWord
{
    public class ScriptureDB : SfDb
	{
		// Attrs -----------------------------------------------------------------------------
		#region VAttr{g}: public DSFMapping Map - Necessary for NUnit access
		// This object allows us to map from the read.Marker to the way to handle each
		// field, e.g., which one is a Section Title, which one is a back translation,
		// etc.
		public DSFMapping Map
		{
			get
			{
                // For NUnit debugging where no project is defined; will not execute otherwise.
                if (G.Project == null || G.TeamSettings == null)
                    return new DSFMapping();

				return G.TeamSettings.SFMapping;
			}
		}
		#endregion
        #region VAttr{g}: SfField LastField(string sMkr)
        public SfField LastField(string sMkr)
            // Returns the last field of the given marker type in the database. We use this
            // in the DSection write operation, where we are looking for the last \vt field
            // in order to attach the |fn.
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                SfField field = Fields[i] as SfField;
                if (field.Mkr == sMkr)
                    return field;
            }

            return null;
        }
        #endregion
        #region VAttr{g}: int RecordCount
        public int RecordCount
        {
            get
            {
                int c = 0;
                foreach (SfField field in Fields)
                {
                    if (field.Mkr == RecordMkr)
                        c++;
                }
                return c;
            }
        }
        #endregion

        // File Format -----------------------------------------------------------------------
        #region Attr{g/s}: Format Format - kParatext, kToolbox, kUnknown
        public Formats Format
        {
            get
            {
                return m_Format;
            }
            set
            {
                m_Format = value;
            }
        }
        public enum Formats { kParatext, kToolbox, kUnknown };
        Formats m_Format = Formats.kUnknown;
        #endregion
        #region VAttr{g}: bool IsParatextFormat
        public bool IsParatextFormat
        {
            get
            {
                return (Format == Formats.kParatext);
            }
        }
        #endregion
        #region VAttr{g}: bool IsToolboxFormat
        public bool IsToolboxFormat
        {
            get
            {
                return (Format == Formats.kToolbox);
            }
        }
        #endregion
        #region Method: Formats _DetermineFormat(sPathName)
        Formats _DetermineFormat(string sPathName)
        {
            // Default: we don't know what we have
            m_Format = Formats.kUnknown;

            // A Shoebox/Toolbox file will always have, as its first marker,
            //    "\_sh"
            // So if we see that, then we're done.
            if (Fields.Count > 0)
            {
                SfField f = Fields[0] as SfField;
                if (f.Mkr == "_sh")
                {
                    m_Format = Formats.kToolbox;
                    return m_Format;
                }
            }

            // We'll count if there are more than 5 markers of the form 
            //    "\vt 1 Text"
            // The reason for checking for more than one is that it is remotely possible
            // that a translation would have numbers in it, that appear in the beginning
            // of a verse. So by insisting on an arbitrary 5, we make sure that this
            // is Paratext.
            int c = 0;
            int cThreshhold = 5;

            // For Paratext format, we'll look for one of several things in the data
            foreach (SfField f in Fields)
            {
                // Do we have any embedded footnotes within a verse text?
                if (-1 != f.Data.IndexOf("\\f "))
                {
                    m_Format = Formats.kParatext;
                    return m_Format;
                }

                // Do we have any embedded cross references within a verse text?
                if (-1 != f.Data.IndexOf("\\x "))
                {
                    m_Format = Formats.kParatext;
                    return m_Format;
                }

                // Check for verse numbers as part of verse text
                if (f.Mkr == "v" && f.Data.Length > 20 && Char.IsDigit(f.Data[0]))
                {
                    c++;
                }
                if (c > cThreshhold)
                {
                    m_Format = Formats.kParatext;
                    return m_Format;
                }
            }

            // If we are here, then the file's internals did not provide the answer.
            // E.g., A newly-created Paratext file, where no drafting has been done, will 
            // not flag any of those tests. So all that is left is the less-desirable
            // method of basing the test on the filename extension.
            //     This test checks for an extension of ".PT_", thus, e.g., PTW, PTX.
            if (JWU.IsParatextScriptureFile(sPathName))
            {
                m_Format = Formats.kParatext;
                return m_Format;
            }

            return m_Format;
        }
        #endregion

        // Transforms ------------------------------------------------------------------------
		#region Method: void _NormalizeMarkers()
		public void _NormalizeMarkers()
		{
			foreach(SfField field in Fields)
			{
                field.Mkr = Map.GetNormalizedMarkerFor(field.Mkr);
			}
		}
		#endregion
		#region Method: void _RemoveShoeboxSpecialMarkers()
		internal void _RemoveShoeboxSpecialMarkers()
		{
			for(int i=0; i<Count; )
			{
				SfField field = GetFieldAt(i);

				if (field.Mkr.StartsWith("_"))
					RemoveAt(i);
				else
					i++;
			}
		}
		#endregion
		#region Method: void _InsertShoeboxSpecialMarkers()
		void _InsertShoeboxSpecialMarkers()
		{
			int cRecords = 0;
			foreach(SfField f in Fields)
			{
				if (f.Mkr == RecordMkr)
					++cRecords;
			}
			InsertAt(0, new SfField("_sh", "v3.0  " + cRecords.ToString() + "  SHW-Scripture"));
			InsertAt(1, new SfField("_DateStampHasFourDigitYear"));
		}
		#endregion
		#region Method: void _RemoveDiscardedMarkers()
		public void _RemoveDiscardedMarkers()
		{
			for(int i=0; i<Count; )
			{
				SfField field = GetFieldAt(i);

				if (Map.IsDiscardedField(field.Mkr))
					RemoveAt(i);
				else
					i++;
			}
		}
		#endregion
		#region Method: void _MoveChaptersIntoParagraphs()
		public void _MoveChaptersIntoParagraphs()
			// Signal: 
			//    - a \c occuring just prior to a \s, or
			//    - a \c occuring just prior to a vernacular paragraph (\p)
			// Move:
			//   - Skip over any \s, \bts, \nt, \ft, etc
			//   - Move to be just after the first pragraph marker
		{
			for(int i=0; i<Count - 1; i++)
			{
				// Get the fields that might satisfy the signal
				SfField fChapter = GetFieldAt(i);
				SfField fNext = GetFieldAt(i+1);

                // Are we sitting on a Chapter marker? 
                if (!Map.IsChapter(fChapter.Mkr))
                {
                    continue;
                }

                // Is the next field a Section, Major Section, or Vernacular Paragraph marker?
                if (!Map.IsSection(fNext.Mkr) &&
                    !Map.IsMajorSectionMarker(fNext.Mkr) && 
                    !Map.IsVernacularParagraph(fNext.Mkr))
                {
                    continue;
                }

				// Move needed: Look for the proper insertion place and make the move
				for (int k = i + 1; k < Count - 1; k++)
				{
					SfField fPara = GetFieldAt(k);

                    // Skip past the section header markers
                    if (Map.IsMajorSectionMarker(fPara.Mkr))
                        continue;
                    if (Map.IsSection(fPara.Mkr))
                        continue;

                    // A Vernacular Paragraph is where we want to move the chapter number to
					if (Map.IsVernacularParagraph( fPara.Mkr ))
					{
						// Move the field
						MoveTo(i, k+1);

						// Main loop processing can continue after the moved position
						i = k + 1;

						break;
					}
				} // endfor k
			} // endfor i
		}
		#endregion
		#region Method: void _MoveChaptersFromParagraphs()
		void _MoveChaptersFromParagraphs()
			// Signal:
			//   1. a \c occurring just following a paragraph marker
			//   2. the paragraph marker must be the first one in the section
			// Move:
			//  If the paragraph marker is the first one in the section,
			//      - Backtrack to find the above section head, move prior to it
			//  Otherwise
			//      - Move it to just in front of the paragraph marker
		{
			for(int i = 1; i<Count; i++)
			{
				// Are we dealing with a chapter paragraph? If not, loop to the next one
				SfField fChapter = GetFieldAt(i);
				if ( !Map.IsChapter( fChapter.Mkr) )
					continue;

				// Signal #1 - Is the previous field a vernacular paragraph?
				SfField fParagraph = GetFieldAt(i-1);
				if ( ! Map.IsVernacularParagraph(fParagraph.Mkr))
					continue;

				// How many paragraph markers are prior to this one in this section?
				int cPriorMarkers = 0;
				for(int k = i-2; k > 0; k--)
				{
					SfField field = GetFieldAt(k);
					if ( field.Mkr  == RecordMkr)
						break;
					if ( Map.IsVernacularParagraph( field.Mkr ))
						++cPriorMarkers;
				}

				// If there are no prior markers, then find the section marker above us, 
				// and move to be just before it
				if (cPriorMarkers == 0)
				{
					for(int k = i-1; k>0; k--)
					{
						SfField field = GetFieldAt(k);

						if ( field.Mkr  == RecordMkr)
							break;

						if (Map.IsSection(field.Mkr))
						{
							MoveTo(i, k);
							break;
						}
					}
					continue;
				}

				// Otherwise, just move it prior to the paragraph above us.
				MoveTo(i, i - 1);
			}
		}
		#endregion
		#region Method: void _EnsureParagraphAfterPicture()
		public void _EnsureParagraphAfterPicture()
			// Signal:
			//   - The field following the last picture field should be the
			//       start of a vernacular paragraph, not a \v.
			// Action:
			//   - Insert a \p if there is not one present.
			//
			// On output, there is no need to reverse this. We are doing it
			// as an Import side-effect.
		{
			for(int i=1; i<Count; i++)
			{
				SfField fPicture = GetFieldAt(i-1);
				SfField fVerse = GetFieldAt(i);

				// If not a picture, keep looping
                if (!DSFMapping.IsPicture(fPicture.Mkr))
					continue;

				// If not a \v or a \vt, keep looping
				if (!Map.IsVerse(fVerse.Mkr) && !Map.IsVerseText(fVerse.Mkr))
					continue;

				// If we are here, then we have, e.g.,
				//   \cap
				//   \v 3
				// So insert a \p
				InsertAt(i, new SfField("p"));
				i++;
			}
		}
		#endregion
		#region Method: void _EnsureRecordsBeforeAllSections()
		public void _EnsureRecordsBeforeAllSections()
			// Signal: 
			//   - For a single \rcrd, a second (or more) \s within it.
			//   - Note: A section doesn't necessarily start at the beginning of
			//      a record; for example, Kupang Mark has a two-verse paragraph
			//      introducing the book that is prior to the first section, but
			//      we still treat it within the first record.
			// Action:
			//   - Insert a \rcrd before it
			//
			// On output, there is no need to reverse this. We are doing it
			// as an Import side-effect.
            //
            // Toolbox will often (but not always) have these markers.
            // Paratext, on the other hand, will never have them.
		{
            // Keep a count of the number of the section we are currently processing
            int nSection = 0;

            // Loop through the database
            for (int i = 0; i < Count; i++)
            {
                // Go to each Section
                SfField fSection = GetFieldAt(i);
                if (!Map.IsSection(fSection.Mkr))
                    continue;
                nSection++;

                // Backtrack to the previous section and see if we find a record marker prior to it
                bool bRecordMarkerFound = false;
                for (int k = i - 1; k >= 0; k--)
                {
                    // Get the field
                    SfField f = GetFieldAt(k);

                    // If it is a record marker, then we have one.
                    if (f.Mkr == RecordMkr)
                    {
                        bRecordMarkerFound = true;
                        break;
                    }

                    // If it is a section, then no need to scan any further
                    if (Map.IsSection(f.Mkr))
                        break;
                }

                // If we did not find a record marker, then we need to insert one
                if (!bRecordMarkerFound)
                {
                    // Normally, we would insert right before the section
                    int iPos = i;

                    // But if we are working on the first record, there may
                    // be some verses preceeding the \s, and we want to back up
                    // and include these in the record.
                    if (1 == nSection)
                    {
                        while (iPos > 0)
                        {
                            SfField f = GetFieldAt(iPos - 1);
                            if (Map.IsSectionContentsMarker(f.Mkr))
                                iPos--;
                            else
                                break;
                        }
                    }

                    // Do the insertion
                    string sRecordKey = nSection.ToString();
                    InsertAt(iPos, new SfField(RecordMkr, sRecordKey));
                    i++;
                }
            }
		}
		#endregion
        #region Method: void _MoveTitlesIntoRecord(int nRecordNo)
        public void _MoveTitlesIntoRecord(int nRecordNo)
		{
			// We are either moving into the 1st (output) or the 2nd (input) records
			Debug.Assert(nRecordNo == 1 || nRecordNo == 2);

			// Find and remove all of the title fields
			ArrayList vTitleFields = new ArrayList();
			for(int i=0; i<Count;)
			{
				SfField field = GetFieldAt(i);

				bool bRemove = false;
				if (Map.IsMainTitleField( field.Mkr ) )
					bRemove = true;
				if (Map.IsMainTitleBT( field.Mkr ) )
					bRemove = true;
				if (Map.IsSubTitleField( field.Mkr ) )
					bRemove = true;
				if (Map.IsSubTitleBT( field.Mkr ) )
					bRemove = true;
				if (Map.IsHeaderMarker( field.Mkr ) )
					bRemove = true;
				if (Map.IsHeaderBT( field.Mkr ) )
					bRemove = true;
                if (bRemove)
				{
					vTitleFields.Add(field);
					Fields.RemoveAt(i);
				}
				else
					i++;
			}

			// Find the target Scripture section (it is the Nth record marker)
			int cRecords = 0;
			int iInsertPos = -1;
			for(int i=0; i<Count - 1; i++)
			{
				SfField field = GetFieldAt(i);
				if (field.Mkr == RecordMkr)
				{
					++cRecords;
					if (cRecords == nRecordNo)
					{
						iInsertPos = i + 1;
						break;
					}
				}
			}

			// Insert the title fields there
			if (-1 != iInsertPos)
			{
				for(int k=0; k<vTitleFields.Count; k++)
				{
					Fields.Insert(iInsertPos, vTitleFields[k] as SfField);
					iInsertPos++;
				}
			}
		}
		#endregion
        #region Method: void _NormalizeVerseText()
        public void _NormalizeVerseText()
			// Given
			//     \v 5 Yihuda bitta Kawuwaa Heroddisa wodian Abiiya
			//
			// Make it
			//     \v 5
			//     \vt Yihuda bitta Kawuwaa Heroddisa wodian Abiiya
			//
			// Also, deal with problems with the verse number, e.g.,
			//    - leading punctuation (move into the \vt field)
			//    - spurious blank space (e.g., "\v 3b - 5a" rather than "\v 3b-5a")
		{
			for(int i=0; i<Count; i++)
			{
				// Get the fields that might satisfy the signal
				SfField field = GetFieldAt(i);

				// Do they fit the signal? If not, loop to the next one
                if (!Map.IsVerse(field.Mkr))
					continue;

				// Remove any leading or trailing spaces
                field.Data = field.Data.Trim();

				// We'll put data into two places
				string sVerseNo = "";
				string sVerseText = "";

				// Remove any leading punctuation, move it to the Verse Text. (Saw this
				// in 43LukTch.ptx, and moving it seems the appropriate action.)
                while (field.Data.Length > 0 &&
                    (field.Data[0] == '(' || field.Data[0] == '['))
				{
                    sVerseText += field.Data[0];
                    field.Data = field.Data.Substring(1);
				}

				// This part is tricky: We do a loop where we test for numbers, because sometimes
				// people have put in a spurious blank in the midst of their verse number, e.g.,
				// we want to catch "\v 3b - 5a". This Parse method returns an index into
                // the part of the field that is beyond the verse number.
                int kDataPos = _ParseVerseNumber(field.Data, out sVerseNo);
                if (field.Data.Length > kDataPos)
                    sVerseText += field.Data.Substring(kDataPos).Trim();

				// The Verse field can now be set as a normalized verse number
                field.Data = sVerseNo;

                // Remove any trailing \v*'s we find
                if (sVerseText.Length > 3 && sVerseText.EndsWith("\\v*"))
                    sVerseText = sVerseText.Substring(0, sVerseText.Length - 3);

				// If we have remaining verse text, then we need to insert a new \vt
				// field following this \v field. This is normal for a Paratext file (USFM),
                // but occasionally shows up in Toolbox data as well.
				if (sVerseText.Length > 0)
                    InsertAt(i + 1, new SfField(Map.MkrVerseText, sVerseText));
			}
		}
		#endregion
        #region Method: void _CombineBTs()
        private void _CombineBTs()
			// Given
			//    \vt 
			//    \btvt
			// Moves the data of the "\btvt" field into the "\vt" field, and deletes
			// the "\btvt" field. 
			//
			// The test is to look for "bt" at the beginning of any marker, and compare the
			// remaining part of that marker with the marker before it.
		{
			for(int i = 0; i < Count - 1; )
			{
				// Retrieve the vernacular field
				SfField fText = GetFieldAt(i);

				// Compute the corresponding markers for BT and IBT fields
				string sBT  = "bt" + fText.Mkr;
				string sIBT = "ibt" + fText.Mkr;

				// Scan forward for the back translation field
				for(int k = i + 1; k < Count; )
				{
					// Retrieve the potential Back Translation field
					SfField fBT   = GetFieldAt(k);

					// If we are at a note, then keep scanning.
					// The Tomohon data has these prior to the BT fields.
					if (DNote.Types.kUnknown != DNote.GetTypeFromMarker(fBT.Mkr))
					{
						k++;
						continue;
					}

					// Are we dealing with a BT field for the current field?
					if (0 == sBT.CompareTo(fBT.Mkr))
					{
						// Place the BT into the text field
						fText.BT += fBT.Data;

						// Delete the BT field
						RemoveAt(k);
					}

					// Are we dealing with a IBT field for the current field?
					else if (0 == sIBT.CompareTo(fBT.Mkr))
					{
						// Place the iBT into the text field
						fText.IBT = fBT.Data;

						// Delete the iBT field
						RemoveAt(k);
					}

					else
						break;
				}

				i++;
			}
		}
		#endregion
		#region Method: void _SeparateBTs()
		private void _SeparateBTs()
			// Reverse the effect of _CombineBTs(), this inserts back translation fields
			// after any field that has one.
		{
			for(int i=0; i < Count; i++)
			{
				SfField fText = GetFieldAt(i);

				if (fText.BT.Length > 0)
				{
					InsertAt(i+1, new SfField("bt" + fText.Mkr, fText.BT));
					i++;
				}
			}
		}
		#endregion
		#region Method: void _RemoveCertainEmptyParagraphs()
		private void _RemoveCertainEmptyParagraphs()
			// If we have two vernacular paragraphs, and the first one has no
			// content, then we delete it. E.g.,
			//
			//  \p
			//  \s From James
			//
		{
			for(int i=0; i < Count - 1; i++)
			{
				SfField f1 = GetFieldAt(i);
				SfField f2 = GetFieldAt(i+1);

				if (!Map.IsVernacularParagraph(f1.Mkr))
					continue;

				if (!Map.IsSection(f2.Mkr))
					continue;

				if (f1.Data.Length != 0)
					continue;

				RemoveAt(i);
			}
		}
		#endregion
        #region Method: void _EnsureInitialTitleRecord()
        public void _EnsureInitialTitleRecord()
            // We need to make certain that the book:
            // - Begins with a record which has the title, but not Scripture
            // - Has a title (mt) field
        {
            // First Pass: make certain we have a \mt field prior to any Scripture
            // sections, so that we actually have something to be in that
            // first record
            int iFirstRecordMarker = -1;
            for (int i = 0; i < Count; i++)
            {
                // Retrieve the next field
                SfField field = GetFieldAt(i);

                // Loop over this field if it is a Toolbox field
                if (field.Mkr.StartsWith("_"))
                    continue;

                // This will locate the first record marker
                if (RecordMkr == field.Mkr && -1 == iFirstRecordMarker)
                    iFirstRecordMarker = i;

                // This will locate the first title section field, in which
                // case we are satisfied with this loop pass.
                if (RecordMkr != field.Mkr && Map.IsBookOverviewMarker(field.Mkr))
                    break;

                // If we locate Scripture (e.g., a "\s"). then there was no
                // title section (else we'd have exited the loop) so we need
                // to insert one.
                if (Map.IsSectionContentsMarker(field.Mkr))
                {
                    // The position would normally be where we currently are
                    int iPos = i;

                    // But if we encountered a record marker just prior to this
                    // Scripture, then we need to back up before it.
                    if (iFirstRecordMarker != -1)
                        iPos = iFirstRecordMarker;

                    // Insert the new "overview" section
                    InsertAt(iPos, new SfField(RecordMkr, ""));
                    InsertAt(iPos + 1, new SfField(Map.MkrMainTitle, ""));

                    // No need to loop farther
                    break;
                }
            }

            // Second Pass: The first marker after any Shoebox markers (defined as "\_....")
            // should be a record marker. This defines our initial section.
            for (int i=0; i < Count; i++)
            {
                // Retrieve the next field
                SfField field = GetFieldAt(i);

                // Loop over this field if it is a Toolbox field
                if (field.Mkr.StartsWith("_"))
                    continue;

                // Otherwise, it should be a record marker, so insert one if not
                if (field.Mkr != RecordMkr)
                    InsertAt(i, new SfField(RecordMkr, ""));

                // No need to loop any further
                break;
            }
        }
        #endregion
        #region Method: void _MoveParagraphText()
        public void _MoveParagraphText()
            // This is similar to what we do in the _NormalizeVerseText method.
            //
            // Paratext (USFM) data can place text beside a paragraph marker, e.g.,
            //   \q1 When she cries, no one can comfort her, 
            //
            // We need to change that to
            //   \q1
            //   \vt When she cries, no one can comfort her,
        {
            for (int i = 0; i < Count; i++)
            {
                // Get the fields that might satisfy the signal
                SfField field = GetFieldAt(i);
                if (!Map.IsVernacularParagraph(field.Mkr))
                    continue;

                // Remove any leading or trailing spaces
                field.Data = field.Data.Trim();

                // If we have data, then create the following \vt field for it
                if (field.Data.Length > 0)
                {
                    InsertAt(i+1, new SfField(Map.MkrVerseText, field.Data));
                    field.Data = "";
                }
            }
        }
        #endregion
        #region Method: void _RemoveExtraSpaces()
        void _RemoveExtraSpaces()
        {
            foreach (SfField f in Fields)
            {
                for (int i = 0; i < f.Data.Length - 1; )
                {
                    if (f.Data.Substring(i, 2) == "  ")
                        f.Data = f.Data.Remove(i, 1);
                    else
                        i++;
                }

                f.Data = f.Data.Trim();
            }
        }
        #endregion
        #region Method: void _MoveMajorSectionParagraphs()
        public void _MoveMajorSectionParagraphs()
        {
            for (int i = Count - 2; i >= 0; i--)
            {
                SfField fMajor = GetFieldAt(i);
                SfField fRecord = GetFieldAt(i + 1);
                if (!Map.IsMajorSectionMarker(fMajor.Mkr))
                    continue;
                if (fRecord.Mkr != RecordMkr)
                    continue;

                Fields.Reverse(i, 2);
            }
        }
        #endregion

        // Transform: Convert Paratext SeeAlso's ---------------------------------------------
        #region CLASS: USFM_SeeAlso - Handles transforms between \cf & \x
        public class USFM_SeeAlso
            #region DOCUMENTATION - What we must accomplish with Paratext SeeAlso's
            /* In USFM/Paratext, a SeeAlso appears as this example:
             * 
             * \v 8 “God promised Abraham and he told him, if his people circumcised/cut-show, 
             * they agree to his rock promise/covenant.\x + \xo 7:8 \xt Jenesis 17:10-14\xt*\x* That 
             * is why Abraham circumcised Aesak, eight days after he was born. In the same way also, 
             * Aesak circumcised his son Jakob, and Jakob circumcised his twelve sons who they 
             * are our(in) ancestors.
             * 
             * ----------------------------------
             * 
             * By the time this gets initially processed, it looks like:
             * 
             * \v 8 
             * \vt “God promised Abraham and he told him, if his people circumcised/cut-show, 
             * they agree to his rock promise/covenant.\x + \xo 7:8 \xt Jenesis 17:10-14\xt*\x* That 
             * is why Abraham circumcised Aesak, eight days after he was born. In the same way also, 
             * Aesak circumcised his son Jakob, and Jakob circumcised his twelve sons who they 
             * are our(in) ancestors.
             * 
             * ----------------------------------
             * 
             * We need to convert it to:
             * \v 8 
             * \vt “God promised Abraham and he told him, if his people circumcised/cut-show, 
             * they agree to his rock promise/covenant.
             * \cf 7:8: Jenesis 17:10-14
             * \vt That is why Abraham circumcised Aesak, eight days after he was born. In the 
             * same way also, Aesak circumcised his son Jakob, and Jakob circumcised his twelve 
             * sons who they are our(in) ancestors.
             * 
             * ----------------------------------
             * 
             * Note that in some data we had from East Asia, different markers were used:
             *     \rc 17.50: \rt 2 Sam 21.19. \rt* \rc*
             * This lines up as:
             *          \rc 17.50: \rt 2 Sam 21.19. \rt* \rc*
             *     \x + \xo 17.50: \xt 2 Sam 21.19. \xt* \x*
             * 
             * ----------------------------------
             * 
             * Further complicating life, the USFM standard has "\x_" and no "\xt*", as in
             *     \x_ + \xo 17.50: \xt 2 Sam 21.19.\x*
             * 
             * The "+" sign can be one of:
             *     + = caller generated automatically, eg. progressive letters or numbers. 
             *     - = no caller generated. 
             *     * = where * represents the character to be used as the caller ? defined by the user. 
             * Of course, OurWord does its own lettering, so we don't care about this; we just
             * eat it.
             * 
             * The USFM standard allows multiple parts:
             *     \x - \xo 49.31: a \xt Gen 25.9,10; \xo b \xt Gen 35.29.\x*
             * Which in OurWord we want to render as:
             *    \cf 49:31: Gen 25.9,10; 
             *    \cf 49:31: Gen 35.29; 
             * 
             */
            #endregion
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: ScriptureDB DB
            ScriptureDB DB
            {
                get
                {
                    Debug.Assert(null != m_DB);
                    return m_DB;
                }
            }
            ScriptureDB m_DB;
            #endregion

            // Import ------------------------------------------------------------------------
            #region Static: string[] ParatextCrossReferenceMkrsToJoin
            public static string[] ParatextCrossReferenceMkrsToJoin =
                    {
                    "xo",   "xo*",
                    "xk",   "xk*",
                    "xq",   "xq*",
                    "xt",   "xt*",
                    "rt",   "rt*",
                    "rc",   "rc*",
                    "x",    "x*"
                    };
                    #endregion
            #region Method: bool IsUSFMSeeAlsoBegin(string sData, int i, out int iLen)
            public bool IsUSFMSeeAlsoBegin(string sData, int i, out int iLen)
                // Refer to the Documentation of what we're accomplishing. We currently
                // recognize the beginning of a USFM cross reference as:
                //  - \rc 
                //  - \x + \xo 
                //  - \x_ + \xo 
            {
                Debug.Assert(i < sData.Length);

                // Default to the length of the match being zero, meaning there is not match
                iLen = 0;

                // If we don't start with a backslash, then no need to test further.
                if (sData[i] != '\\')
                    return false;
                bool bXRefFound = false;

                // First, check to see if there is a leading "\x"
                string[] vsX = { "\\x", "\\x_" };
                foreach(string s in vsX)
                {
                    if (sData.Length > i + s.Length + 1 && 
                        s == sData.Substring(i, s.Length) && 
                        Char.IsWhiteSpace(sData[i + s.Length]))
                    {
                        iLen += (s.Length + 1);
                        i += (s.Length + 1);
                        bXRefFound = true;
                        break;
                    }
                }

                // If there was, then we need to eat anything up to the next "\\", which will usually
                // be a "+" or a "-", but can be any character. 
                //   Note that this will eat any extra white spaces.
                if (bXRefFound)
                {
                    while (i<sData.Length && sData[i] != '\\')
                    {
                        iLen++;
                        i++;
                    }
                }

                // Now, we will typically have the Cross Reference "Origin" marker
                string[] vsXO = { "\\xo", "\\rc" };
                foreach(string s in vsXO)
                {
                    if (sData.Length > i + s.Length + 1 &&
                        s == sData.Substring(i, s.Length) &&
                        Char.IsWhiteSpace(sData[i + s.Length]))
                    {
                        iLen += (s.Length + 1);
                        i += (s.Length + 1);
                        bXRefFound = true;
                        break;
                    }
                }

                // Go ahead and eat any extra whitespace prior to any real data
                if (bXRefFound)
                {
                    while (i < sData.Length && Char.IsWhiteSpace(sData[i]))
                    {
                        i++;
                        iLen++;
                    }
                }

                // So at this point, 
                // - iLen tells us how long the begining part is, and
                // - bXRefFound tells uw whether there was a cross reference or not

                return bXRefFound;
            }
            #endregion
            #region Method: bool IsUSFMSeeAlsoEnd(string sData, int i, out int iLen)
            public bool IsUSFMSeeAlsoEnd(string sData, int i, out int iLen)
            // Refer to the Documentation of what we're accomplishing. We currently
            // recognize the end of a USFM cross reference as:
            //  - \rt* \rc*
            //  - \xt* \x*
            //  - \x* 
            {
                Debug.Assert(i < sData.Length);

                // Default to the length of the match being zero, meaning there is not match
                iLen = 0;

                // If we don't start with a backslash, then no need to test further.
                if (sData[i] != '\\')
                    return false;

                // Possibilities we are currently checking for
                string[] vsX = 
                {   
                    "\\rt* \\rc*", 
                    "\\xt* \\x*", 
                    "\\xt*\\x*", 
                    "\\rc*",
                    "\\x*" 
                };

                // Do we see the phrase in the data?
                foreach (string s in vsX)
                {
                    if (sData.Length >= i + s.Length && s == sData.Substring(i, s.Length))
                    {
                        iLen = s.Length;
                        return true;
                    }
                }

                return false;
            }
            #endregion
            #region Method: string ExtractSeeAlso(string sRaw)
            public string ExtractSeeAlso(string sRaw)
                // Given a Paratext cross reference, remove any internal USFM codes.
                // Refer to the DOCUMENTATION above.
            {
                // We'll store the cross ref that we build here
                string sXRef = "";

                // Loop through the string, removing any SF markers (as we don't
                // use them, and thus will ignore them.)
                for (int i = 0; i < sRaw.Length; )
                {
                    if (sRaw[i] == '\\')
                    {
                        while (i < sRaw.Length && !char.IsWhiteSpace(sRaw[i]))
                            i++;
                        if (i < sRaw.Length && char.IsWhiteSpace(sRaw[i]))
                            i++;
                        continue;
                    }

                    // Add the next character and advance <i> to the next one.
                    sXRef += sRaw[i];
                    i++;
                }

                return sXRef.Trim();
            }
            #endregion
            #region Method: void Import()
            public void Import()
			    //   \cf 1:78: Mal 4.2
		    {
                // First, make pass to deal with any markers that somehow wound up being
                // in their own fields
                DB.JoinParatextEmbeddedFields(ParatextCrossReferenceMkrsToJoin);

                // Loop to process each field
                for (int i = 0; i < DB.Count; i++)
                {
                    // Get the next verse marker field (\vt)
                    SfField f = DB.GetFieldAt(i);
                    if (!DB.Map.IsVerseText(f.Mkr))
                        continue;

                    // Scan to see if we have a SeeAlso; we're done with this field if there isn't one.
                    int iPosBegin = 0;
                    int cSeeAlsoBegin = 0;
                    for (; iPosBegin < f.Data.Length; iPosBegin++)
                    {
                        if (IsUSFMSeeAlsoBegin(f.Data, iPosBegin, out cSeeAlsoBegin))
                            break;
                    }
                    if (iPosBegin == f.Data.Length)
                        continue;

                    // Scan to find the end of the cross reference
                    int iPosEnd = iPosBegin;
                    int cSeeAlsoEnd = 0;
                    for (; iPosEnd < f.Data.Length; iPosEnd++)
                    {
                        if (IsUSFMSeeAlsoEnd(f.Data, iPosEnd, out cSeeAlsoEnd))
                            break;
                    }

                    // Collect the three strings
                    string sBefore = f.Data.Substring(0, iPosBegin);
                    int cLengthRawSeeAlso = iPosEnd - (iPosBegin + cSeeAlsoBegin);
                    string sSeeAlsoRaw = f.Data.Substring(iPosBegin + cSeeAlsoBegin, cLengthRawSeeAlso);
                    string sEnd = f.Data.Substring(iPosEnd + cSeeAlsoEnd).Trim();
                   
                    // Create a proper XRef string for OurWord (remove the middle std format marker)
                    string sSeeAlso = ExtractSeeAlso(sSeeAlsoRaw);

                    // Update the field's data to just have what was prior to the cross reference
                    f.Data = sBefore;

                    // If we had any trailing text, then we need to insert the new marker for it. The loop
                    // will automatically process it for any additional cross references.
                    if (sEnd.Length > 0)
                        DB.InsertAt(i+1, new SfField(DB.Map.MkrVerseText, sEnd));

                    // Create and insert the cross reference field
                    DB.InsertAt(i+1, new SfField(DB.Map.MkrSeeAlso, sSeeAlso) );
                }
            }
		    #endregion

            // Export ------------------------------------------------------------------------
            #region Method: void Export()
            public void Export()
                // Given: \cf 1:78: Mal 4.2, Luke 9:7-8
                // Create: ....text.... \x \xo 1:78 \xo* Mal 4.2, Luke 9:7-8\x*
                //
                // In OW, we know that the Reference part is everything up until the first
                // whitespace encountered.
            {
                for (int i = 0; i < DB.Count- 1; i++)
                {
                    // Locate a SeeAlso field
                    SfField fText = DB.GetFieldAt(i);
                    SfField fSeeAlso = DB.GetFieldAt(i + 1);
                    if (!DB.Map.IsSeeAlso(fSeeAlso.Mkr))
                        continue;

                    // Extract the parts
                    string sReference = "";
                    string sContents = "";
                    bool bContents = false;
                    foreach (char ch in fSeeAlso.Data)
                    {
                        if (!bContents && char.IsWhiteSpace(ch))
                        {
                            bContents = true;
                            continue;
                        }

                        if (bContents)
                            sContents += ch;
                        else
                            sReference += ch;
                    }
                    if (sContents.Length == 0 || sReference.Length == 0)
                        continue;

                    // Build the USFM string
                    string sSeeAlso = "\\x + \\xo " + sReference + "\\xo* " + sContents + "\\x* ";

                    // Append it to the previous field, and remove the cross ref field
                    fText.Data = fText.Data.Trim() + sSeeAlso;
                    DB.RemoveAt(i + 1);

                    // Process at t his position again in case we have another one
                    i--;
                }
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(DB)
            public USFM_SeeAlso(ScriptureDB _DB)
            {
                m_DB = _DB;
            }
            #endregion
        }
        #endregion

        // Transform: Convert Paratext Footnotes ---------------------------------------------
        #region DOCUMENTATION - What we must accomplish with Paratext footnotes
        /* The issues here are similar to converting Cross Refs, described separately.
         * 
         * 1. Here are some examples in the test data:
         *    \f a \fr 2.22 \fk kutakaswa: \ft Angalia Law 12.1-8.\f*
         *    \f c \fr 3:4-6 \ft Isa 40:3-5 \f*
         *    \f + \fr 18:6 \fr* ipima mngana i:nye n litili 11, ym 12. \f*
         *    \f + \fr 10:12 \fk Sodom \ft shin Gen./Fara. 19:24-28\ft*\f*
         * 
         * An OurWord footnote is has a lot less fields.
         *    |fn (goes in the text)
         *    \ft 1:1: Tuis-tuis Sur Akninu' uab....
         * 
         * Since we are throwing things away, this becomes much simpler
         *    - Scan for "\f "
         *    - Skip past the "\fr "
         *    - Collect everything up to "\f*", omitting "\_"'s along the way
         * 
         * 2. There is also an issue that arises from the nature of standard format, which
         * is that these footnote fields may be on their own lines when we import and thus
         * have their own markers. Thus we might get something like:
         *    \vt Da yaron ya cika kwana takwas, sai suka zo domin su yi masa kaciya.\f c \fr 1.59
         *    \fk kaciya:\fk* Dubi ma'anar wa?ansu kalmomi.\f* Da ma sun so ne su sa masa sunan babansa,
         * 
         * So we use a Join method to preprocess for any of these, so that we have a single
         * field to proces by the main method, which in this example looks like:
         *    \vt Da yaron ya cika kwana takwas, sai suka zo domin su yi masa kaciya.\f c \fr 1.59
         *        \fk kaciya:\fk* Dubi ma'anar wa?ansu kalmomi.\f* Da ma sun so ne su sa masa sunan 
         *        babansa,
         */
        #endregion
        #region Static: string[] ParatextFootnoteMkrsToJoin
        public static string[] ParatextFootnoteMkrsToJoin =
                {
                "fk",   "fk*",
                "fr",   "fr*",
                "fq",   "fq*",
                "fqa",  "fqa*",
                "fl",   "fl*",
                "fv",   "fv*",
                "f",    "f*"
                };
        #endregion
        #region Method: void JoinParatextEmbeddedFields(string[] vsMkrsToJoin)
        public void JoinParatextEmbeddedFields(string[] vsMkrsToJoin)
            // Refer to Section 2 in the Footnote documentation.
            //
            // We consider this aberant data; we are fixing it enough here to import, but not
            // worrying if we wind up with a few extraneous spaces introduced here and there.
        {
            for (int i = 0; i < Count - 1; )
            {
                SfField f1 = GetFieldAt(i);
                SfField f2 = GetFieldAt(i + 1);

                // See if the f2 marker is a match with any of our possibilities
                bool bFound = false;
                foreach (string sMkr in vsMkrsToJoin)
                {
                    if (f2.Mkr == sMkr)
                    {
                        bFound = true;
                        break;
                    }
                }

                // If a match, then combine it with the previous marker
                if (bFound)
                {
                    // If f1.Data does not end with a space, we need to add one. Sometimes it
                    // is needed (e.g., "\f a"), sometimes it is not; we are erring on the side 
                    // of adding them.
                    if (f1.Data.Length > 0 && f1.Data[f1.Data.Length - 1] != ' ')
                        f1.Data += ' ';

                    // Append the contents of the following field
                    f1.Data += ("\\" + f2.Mkr + " " + f2.Data);

                    // Remove the following field.
                    Remove(f2);
                }
                else
                    i++;
            }
        }
        #endregion
        #region Method: void _ImportParatextFootnotes()
        public void _ImportParatextFootnotes()
        {
            // First, make pass to deal with any markers that somehow wound up being
            // in their own fields
            JoinParatextEmbeddedFields(ParatextFootnoteMkrsToJoin);

            // The markers that denote the beginning and ending of a footnote
            string sBegin = "\\f ";
            string sEnd = "\\f*";

            // Loop to process each field
            for (int iF = 0; iF < Count; iF++)
            {
                // Get the next verse marker field (\vt)
                SfField f = GetFieldAt(iF);
                if (!Map.IsVerseText(f.Mkr) && !Map.IsSection(f.Mkr) && !Map.IsSection2(f.Mkr))
                    continue;

                // We'll put the footnote texts here so we can store if there are more than one
                ArrayList aFootnotes = new ArrayList();

                // Loop to scan for the footnotes
                for (int i = 0; i < f.Data.Length; i++)
                {
                    // They all start with a "\f "; if we don't find it, then keep scanning.
                    if (i + sBegin.Length > f.Data.Length || f.Data.Substring(i, sBegin.Length) != sBegin)
                        continue;

                    // We'll use <k> scan to the end of the footnote. 
                    int k = i;

                    // We'll collect the footnote here
                    string sFootnoteText = "";

                    // Move past the Begin marker, including the letter after it, by scanning
                    // to the next "\" marker
                    k += sBegin.Length;
                    while (k < f.Data.Length && f.Data[k] != '\\')
                        k++;

                    // Collect everything up to the end marker; skip over any markers along the way.
                    while (k < f.Data.Length)
                    {
                        // Check to see if we've encountered the End marker (in which case, we are done.)
                        if (k + sEnd.Length <= f.Data.Length && f.Data.Substring(k, sEnd.Length) == sEnd)
                        {
                            k += sEnd.Length;
                            break;
                        }

                        // If we encounter any other kind of marker along the way, eat it. Then loop
                        // back in case we have another marker immediately following.
                        if (f.Data[k] == '\\')
                        {
                            k++;
                            while (k < f.Data.Length && f.Data[k] != ' ' && f.Data[k] != '\\')
                                k++;
                            if (k < f.Data.Length && f.Data[k] == ' ')
                                k++;
                            continue;
                        }

                        // If we are here, then we have footnote text to add
                        sFootnoteText += f.Data[k];
                        k++;
                    }

                    // Rebuild the Data attribute, replacing the footnote with "|fn"
                    string sFirst = f.Data.Substring(0, i);
                    string sLast = f.Data.Substring(k);
                    f.Data = sFirst + "|fn" + sLast;

                    // Remember the footnote text, as we need to keep scanning for more.
                    aFootnotes.Add(sFootnoteText.Trim());
                }

                // Insert the new footnote markers
                for (int n = aFootnotes.Count - 1; n >= 0; n--)
                {
                    InsertAt(iF + 1, new SfField(Map.MkrFootnote, aFootnotes[n] as string));
                }
            }
        }
        #endregion
        #region Method: void _ExportParatextFootnotes()
        void _ParseFootnote(string s, out string sReference, out string sContents)
        {
            // Default values
            sReference = "";
            sContents = s;

            // A legal reference is a digit, or limited other punctuation characters
            string sReferenceChars = "0123456789:-., ";

            // Work through the string until we find a character that is not 
            // in our list of acceptable possibilities
            int k = 0;
            for (; k < s.Length; k++)
            {
                if (sReferenceChars.IndexOf(s[k]) == -1)
                    break;
            }

            // Divide the string according to the result
            if (k > 0)
            {
                sReference = s.Substring(0, k).Trim();
                sContents = s.Substring(k).Trim();
            }
        }

        void _ExportParatextFootnotes()
        {
            for (int i = 0; i < Count - 1; i++)
            {
                // Find a verse with a footnote in it
                SfField fv = GetFieldAt(i);
                SfField fft = GetFieldAt(i + 1);
                if (!Map.IsVerse(fv.Mkr) && !Map.IsFootnote(fft.Mkr))
                    continue;
                int iFN = fv.Data.IndexOf("|fn");
                if (-1 == iFN)
                    continue;

                // Parse the footnote into its parts
                string sReference;
                string sContents;
                _ParseFootnote(fft.Data, out sReference, out sContents);

                // Remove the footnote field
                RemoveAt(i + 1);

                // Build the appropriate Paratext format for footnotes
                if (sReference.Length > 0 && sContents.Length > 0)
                {
                    // Build the Paratext footnote text
                    string sFootnote = "\\f + \\fr " + sReference + "\\fr* " + sContents + "\\f* ";

                    // Substitute it into the string
                    fv.Data = fv.Data.Substring(0, iFN) + sFootnote + fv.Data.Substring(iFN + 3);
                    fv.Data = fv.Data.Trim();
                }

                i--;
            }
        }
        #endregion

        // Transform: Other Paratext ---------------------------------------------------------
        #region Method: void _RemoveNonSupportedMarkers() - Export to Paratext: strips out, e.g, \rcrd's
        void _RemoveNonSupportedMarkers()
        {
            for (int i = 0; i < Count; )
            {
                SfField f = GetFieldAt(i);

                if (!Map.IsUSFMExportMarker(f.Mkr))
                    RemoveAt(i);
                else
                    i++;
            }
        }
        #endregion
        #region Method: void _ExportParatextVerseText()
        void _ExportParatextVerseText()
            // Reverses _NormalizeVerseText, so that
            //     \v 5
            //     \vt Yihuda bitta Kawuwaa Heroddisa wodian Abiiya
            // becomes
            //     \v 5 Yihuda bitta Kawuwaa Heroddisa wodian Abiiya
        {
            for (int i = 0; i < Count - 1; i++)
            {
                // Get the fields that might satisfy the signal
                SfField fv = GetFieldAt(i);
                SfField fvt = GetFieldAt(i + 1);

                // Do they fit the signal? If not, loop to the next one
                if (!Map.IsVerse(fv.Mkr) && !Map.IsVerseText(fvt.Mkr))
                    continue;

                // Combine the two
                fv.Data = fv.Data.TrimEnd() + " " + fvt.Data;
                fv.BT   = fv.Data.TrimEnd() + " " + fvt.BT;

                // Delete the VerseText field
                RemoveAt(i + 1);
            }
        }
        #endregion
        #region Method: void _ConvertParatextVerseBridges()
        public string _ConvertParatextVerseBridge(string s)
        {
            // Locate the end of the verse numbers (as indicated by white space)
            int iText = 0;
            for (; iText < s.Length; iText++)
            {
                if (char.IsWhiteSpace(s[iText]))
                    break;
            }

            // Locate the positions of the first and of the final commas within this region
            int iFirst = -1;
            int iLast = -1;
            for (int i = 0; i < iText; i++)
            {
                if (s[i] == ',')
                {
                    if (-1 == iFirst)
                        iFirst = i;
                    iLast = i;
                }
            }

            // If commas were found, then replace the region with a hyphen
            if (-1 != iFirst && iLast < s.Length - 1)
            {
                string sLeft = s.Substring(0, iFirst);
                string sRight = s.Substring(iLast + 1);

                s = sLeft + '-' + sRight;
            }

            return s;
        }
        void _ConvertParatextVerseBridges()
        {
            foreach (SfField f in Fields)
            {
                if (!Map.IsVerse(f.Mkr))
                    continue;

                f.Data = _ConvertParatextVerseBridge(f.Data);
            }
        }
        #endregion
        #region Method: void _ExportParatextMarkers()
        void _ExportParatextMarkers()
        {
            foreach (SfField field in Fields)
            {
                if (field.Mkr == Map.MkrSubTitle)
                    field.Mkr = "mt2";
            }
        }
        #endregion

        // Transform: Paratext Pictures ------------------------------------------------------
        #region CLASS: USFM_Pictures
        public class USFM_Pictures
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: ScriptureDB DB
            ScriptureDB DB
            {
                get
                {
                    Debug.Assert(null != m_DB);
                    return m_DB;
                }
            }
            ScriptureDB m_DB;
            #endregion

            // Embedded Class: FigureParts ---------------------------------------------------
            #region CLASS FigureParts
            class FigureParts
            {
                // Attrs ---------------------------------------------------------------------
                string[] m_vsFields;

                // Interpretations of the various m_vsFields ---------------------------------
                #region VAttr{g/s}: string Description - Comment; does not show up in publication
                public string Description
                {
                    get
                    {
                        return m_vsFields[0];
                    }
                    set
                    {
                        m_vsFields[0] = value;
                    }
                }
                #endregion
                #region VAttr{g/s}: string FileName - Illustration filename (usually no path)
                public string FileName
                {
                    get
                    {
                        return m_vsFields[1];
                    }
                    set
                    {
                        m_vsFields[1] = value;
                    }
                }
                #endregion
                #region VAttr{g/s}: string Size - Either "col" for column, or "span" for page
                public string Size
                {
                    get
                    {
                        return m_vsFields[2];
                    }
                    set
                    {
                        m_vsFields[2] = value;
                    }
                }
                #endregion
                #region VAttr{g/s}: string Location - Range of references where insertion is allowed
                public string Location
                {
                    get
                    {
                        return m_vsFields[3];
                    }
                    set
                    {
                        m_vsFields[3] = value;
                    }
                }
                #endregion
                #region VAttr{g/s}: string Copyright - To give proper illustration credits
                public string Copyright
                {
                    get
                    {
                        return m_vsFields[4];
                    }
                    set
                    {
                        m_vsFields[4] = value;
                    }
                }
                #endregion
                #region VAttr{g/s}: string Caption - Text that is printed with the illustration
                public string Caption
                {
                    get
                    {
                        return m_vsFields[5];
                    }
                    set
                    {
                        m_vsFields[5] = value;
                    }
                }
                #endregion
                #region VAttr{g/s}: string Reference - Scripture reference, is printed with the illustration
                public string Reference
                {
                    get
                    {
                        return m_vsFields[6];
                    }
                    set
                    {
                        m_vsFields[6] = value;
                    }
                }
                #endregion

                // Methods -------------------------------------------------------------------
                #region SMethod: FigureParts Parse(string s)
                static public FigureParts Parse(string s)
                {
                    FigureParts fp = new FigureParts();

                    // This will iterate through the fields whenever a '|' is encountered
                    int i = 0;

                    // Loop through each character in the line
                    foreach (char ch in s)
                    {
                        if (ch == '|')
                        {
                            i++;
                            continue;
                        }

                        fp.m_vsFields[i] += ch;
                    }

                    // If the input data did not distinguish parts, we'll place it all in the
                    // caption instead
                    if (i == 0)
                    {
                        fp.Caption = fp.Description;
                        fp.Description = "";
                    }

                    return fp;
                }
                #endregion
                #region Attr{g}: string USFM_Out
                public string USFM_Out
                {
                    get
                    {
                        string s = "";

                        for (int i = 0; i < m_vsFields.Length; i++)
                        {
                            s += m_vsFields[i];
                            if (i != m_vsFields.Length - 1)
                                s += '|';
                        }

                        s += USFM_Pictures.c_sFigEnd;

                        return s;
                    }
                }
                #endregion

                // Scaffolding ---------------------------------------------------------------
                #region Constructor()
                public FigureParts()
                {
                    m_vsFields = new string[7];
                }
                #endregion
            }
            #endregion

            // Constants ---------------------------------------------------------------------
            const string c_sFigMkr = "fig";
            const string c_sFigBegin = "\\fig ";
            const string c_sFigEnd = "\\fig*";

            // Import ------------------------------------------------------------------------
            #region Method: void _ExtractIntoSeparateFields()
            void _ExtractIntoSeparateFields()
                // Separate into their own individual fields (rather than leaving as 
                // part of a containing field.)
            {
                for (int i = 0; i < DB.Fields.Count; i++)
                {
                    SfField field = DB.Fields[i] as SfField;

                    int iFigBegin = field.Data.IndexOf(c_sFigBegin);
                    if (-1 == iFigBegin)
                        continue;

                    int iFigEnd = field.Data.IndexOf(c_sFigEnd);
                    if (-1 == iFigEnd)
                        iFigEnd = field.Data.Length;
                    else
                        iFigEnd += c_sFigEnd.Length;

                    string sLeft = field.Data.Substring(0, iFigBegin);
                    string sFigData = field.Data.Substring(iFigBegin + c_sFigBegin.Length,
                        iFigEnd - iFigBegin - c_sFigBegin.Length);
                    string sRight = (iFigEnd < field.Data.Length) ?
                        field.Data.Substring(iFigEnd) : "";

                    field.Data = sLeft;
                    DB.Fields.Insert(i + 1, new SfField(c_sFigMkr, sFigData));
                    if (sRight.Length > 0)
                        DB.Fields.Insert(i + 2, new SfField("vt", sRight));
                }
            }
            #endregion
            #region Method: void _RemoveEndMarkers()
            void _RemoveEndMarkers()
                // Remove the trailing \\fig* markers
            {
                for (int i = 0; i < DB.Fields.Count; )
                {
                    SfField field = DB.Fields[i] as SfField;

                    int iStart = field.Data.IndexOf(c_sFigEnd);
                    if (iStart == -1)
                    {
                        i++;
                        continue;
                    }

                    int iEnd = iStart + c_sFigEnd.Length;

                    string sLeft = field.Data.Substring(0, iStart);
                    string sRight = (iEnd < field.Data.Length) ?
                        field.Data.Substring(iEnd) : "";

                    field.Data = sLeft + sRight;
                }
            }
            #endregion
            #region Method: void _InterpretIntoToolbox()
            void _InterpretIntoToolbox()
                // Convert into Toolbox fields
            {
                for (int i = 0; i < DB.Fields.Count; i++)
                {
                    SfField field = DB.Fields[i] as SfField;
                    if (field.Mkr != c_sFigMkr)
                        continue;

                    FigureParts fp = FigureParts.Parse(field.Data);

                    DB.Fields.RemoveAt(i);

                    if (!string.IsNullOrEmpty(fp.Caption))
                    {
                        DB.Fields.Insert(i, new SfField(
                            DSFMapping.c_sMkrPictureCaption,
                            fp.Caption));
                    }

                    if (!string.IsNullOrEmpty(fp.Size))
                    {
                        DB.Fields.Insert(i, new SfField(
                            DSFMapping.c_sMkrPictureWordRtf,
                            fp.Size));
                    }

                    SfField fCat = new SfField(
                        DSFMapping.c_sMkrPicturePath,
                        (!string.IsNullOrEmpty(fp.FileName) ? fp.FileName : "Unknown"));
                    DB.Fields.Insert(i, fCat);
                }
            }
            #endregion
            #region Method: void Import()
            public void Import()
            {
                _ExtractIntoSeparateFields();
                _RemoveEndMarkers();
                _InterpretIntoToolbox();
            }
            #endregion

            // Export ------------------------------------------------------------------------
            #region Method: void Export()
            public void Export()
            {
                FigureParts fp = null;

                for (int i = 0; i < DB.Fields.Count; )
                {
                    SfField field = DB.Fields[i] as SfField;

                    if (!DSFMapping.IsPicture(field.Mkr))
                    {
                        if (null != fp)
                        {
                            DB.Fields.Insert(i, new SfField(c_sFigMkr, fp.USFM_Out));
                            fp = null;
                        }
                        i++;
                        continue;
                    }

                    if (null == fp)
                        fp = new FigureParts();

                    if (field.Mkr == DSFMapping.c_sMkrPicturePath)
                        fp.FileName = field.Data;
                    if (field.Mkr == DSFMapping.c_sMkrPictureCaption)
                        fp.Caption = field.Data;
                    if (field.Mkr == DSFMapping.c_sMkrPictureWordRtf)
                        fp.Size = field.Data;

                    DB.Fields.RemoveAt(i);
                }

                if (null != fp)
                    DB.Fields.Insert(DB.Fields.Count, new SfField(c_sFigMkr, fp.USFM_Out));
            }
            #endregion

            #region Constructor(DB)
            public USFM_Pictures(ScriptureDB _DB)
            {
                m_DB = _DB;
            }
            #endregion
        }
        #endregion

        // MAIN TRANSFORM METHODS ------------------------------------------------------------
        #region Method: void TransformIn()
        public void TransformIn()
            // Takes a raw standard format file (either Toolbox or Paratext) and
            // massages it into a standard that the Import methods can more
            // easily deal with.
            // 
			// Do in the reverse order of TransformOut
		{
            // Gets rid of variants of markers, so that we can always count on
			// markers we expect.
			_NormalizeMarkers();

			// Gets rid of, e.g., "\_sh ..." and "\_DateStampHasFourDigitYear"
			_RemoveShoeboxSpecialMarkers();

			// Get rid of markers we don't care about
			_RemoveDiscardedMarkers();

            // Combine BT fields into their preceeding text field
			_CombineBTs();

			// Moves \c to occur after \p
			_MoveChaptersIntoParagraphs(); 

            // Paratext Pictures
            (new USFM_Pictures(this)).Import();

			// Make sure we start a new paragraph after a picture
			_EnsureParagraphAfterPicture();

			// Make sure that we don't have more than one \s in a single record
			_EnsureRecordsBeforeAllSections();

            // Make sure that we have an initial record that contains the
            // book overview stuff (e.g., a title field)
            _EnsureInitialTitleRecord();

			// Move the title fields (\mt, \st, \h) into the second section (which is
			// the first Scripture section.) We move them there for easier editing
			_MoveTitlesIntoRecord(2);

            // Move Major Section Headings (\ms, \mr) into the next record, if they
            // occur at the end of a record
            _MoveMajorSectionParagraphs();

            // Deal with any USFM fields of the form: "\q When she cries,"
            _MoveParagraphText();

            // Change "\v 1,2,3 Text" into "\v 1-3 Text"
            _ConvertParatextVerseBridges();

			// Change "\v 5 hello" into a "\v" followed by a "\vt"
			_NormalizeVerseText();

            // Paratext Cross References and footnotes
            (new  USFM_SeeAlso(this)).Import();
            _ImportParatextFootnotes();

// TEST PROCEDURES THRU HERE //
			// Handle a \p that preceeds a \s with no content
//			_RemoveCertainEmptyParagraphs();

		}
		#endregion
		#region Method: void TransformOut()
		public void TransformOut()
			// Do in the reverse order of TransformIn
		{
            Debug.Assert(Format != Formats.kUnknown);

			// Move the title fields from the second record (the first Scripture section)
			// into the first record (the "about this book" record.)
            if (IsToolboxFormat)
                _MoveTitlesIntoRecord(1);

			// Moves \c to occur before \s rather than after the first paragraph
            if (IsToolboxFormat)
                _MoveChaptersFromParagraphs();

			// Insert at the beginning the "\_sh ..." and "\_DateStampHasFourDigitYear" mkrs
            if (IsToolboxFormat)
                _InsertShoeboxSpecialMarkers();

			// Separate out any Back Translation fields (for Paratext, don't bother, as
            // we're just going to drop them on export anyway.
            if (IsToolboxFormat)
                _SeparateBTs();

            // Paratext-specific transforms
            if (IsParatextFormat)
            {
                _ExportParatextVerseText();
                _RemoveNonSupportedMarkers();
                _ExportParatextFootnotes();
                _ExportParatextMarkers();
                (new USFM_SeeAlso(this)).Export();
                (new USFM_Pictures(this)).Export();
            }

            // Get rid of any double white spaces that may have been introduced
            _RemoveExtraSpaces();
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public ScriptureDB()
			: base()
		{
		}
		#endregion
		#region Override: bool Initialize(ref string sPathName) - populate from StDmt disk file
		public override bool Read(ref string sPathName)
		{
			// Read in the file (if possible)
			if (!base.Read(ref sPathName))
				return false;

            // Determine whether or not this is a Paratext file, by examining the data
            _DetermineFormat(sPathName);

			return true;
		}
		#endregion

		// Misc methods ----------------------------------------------------------------------
		#region Method: static bool _IsDigit(char ch) - includes "el" and "oh"
		static bool _IsDigit(char ch)
		{
			if (Char.IsDigit(ch))
				return true;
			if (ch == 'l' || ch == 'O')
				return true;
			return false;
		}
		#endregion
        #region Method: int _ParseVerseNumber(string s, out string sVerse)
        int _ParseVerseNumber(string s, out string sVerse)
        {
            int i = 0;
            sVerse = "";

            // Move past any leading blanks
            while (i < s.Length && Char.IsWhiteSpace(s[i]))
                i++;

            bool bProcessingDigit = false;
            bool bVerseBridgeEncountered = false;

            // We except to find a number here.
            while (i < s.Length)
            {
                char ch = s[i];

                // Due to typos we've seen in real data, we interpret 
                // - l (el) as 1 (one) 
                // - O (oh) as 0 (zero)
                if (ch == 'l')
                    ch = '1';
                if (ch == 'O')
                    ch = '0';

                // A verse number
                if (_IsDigit(ch))
                {
                    sVerse += ch;
                    bProcessingDigit = true;
                    goto loop;
                }

                // A single following letter, e.g., '3a' can only occur
                // after a digit. 
                if (bProcessingDigit && Char.IsLower(ch))
                {
                    // Examine the next character if available; it can only be
                    // whitespace or a hyphen. Otherwise this is part of the
                    // next word, and must not be included in the verse number.
                    char chNext = '\0';
                    if (i + 1 < s.Length)
                        chNext = s[i + 1];
                    if (chNext != '\0' && !Char.IsWhiteSpace(chNext) && chNext != '-')
                    {
                        break;
                    }

                    sVerse += ch;
                    bProcessingDigit = false;
                    goto loop;
                }

                // A hyphen indicates a verse bridge
                if (ch == '-' && !bVerseBridgeEncountered)
                {
                    sVerse += ch;
                    bProcessingDigit = false;
                    bVerseBridgeEncountered = true;
                    goto loop;
                }

                // A space either indicates the end of the verse number, or
                // else it is spurious within a verse bridge While we allow "3a",
                // we don't allow "3 a" as in this latter case the 'a' could be
                // its own word. So a "spurious" space occurs only if it is
                // followed by a hyphen or a digit.
                if (Char.IsWhiteSpace(ch))
                {
                    // By definition, we are done processing any number; we do
                    // so by turning this off, "3 a" will not be considered.
                    bProcessingDigit = false;

                    // Get the next character
                    char chNext = '\0';
                    if (i + 1 < s.Length)
                        chNext = s[i + 1];

                    // If it is a blank, then we'll loop so as to try again; we 
                    // want to "eat" multiple blank spaces.
                    if (Char.IsWhiteSpace(chNext))
                        goto loop;

                    // If it is a verse bridge, then loop to collect it
                    if (chNext == '-' && !bVerseBridgeEncountered)
                        goto loop;

                    // If it is a digit, and we have seen a hyphen, then
                    // then we'll assume we're finishing a verse bridge.
                    if (_IsDigit(chNext) && bVerseBridgeEncountered)
                        goto loop;

                    // Otherwise, we're at the next word, so its time to leave
                    break;
                }

                // If we have gotten here, then we encountered something that is not
                // part of a verse number.
                break;

            loop:
                i++;
            }


            return i;
        }
        #endregion
        #region Method: ArrayList GetNormalizedMarkersInventory()
        public ArrayList GetNormalizedMarkersInventory()
        {
            // We'll place the answer in an arraylist, thus it will be a list
            // of strings.
            ArrayList a = new ArrayList();

            // Loop through all of the fields in the database
            foreach (SfField field in Fields)
            {
                // Get the marker; permit the Map to normalize it (thus, e.g.,
                // "mt1" becomes "mt"
                string sMarker = Map.GetNormalizedMarkerFor(field.Mkr);

                // For this marker, scan the ArrayList to see if it is already 
                // present there.
                bool bFound = false;
                foreach (string sMkr in a)
                {
                    if (sMkr == sMarker)
                        bFound = true;
                }

                // We did not find it in the ArrayList, so add it.
                if (!bFound)
                    a.Add(sMarker);
            }

            // Alphabetize
            a.Sort();

            return a;
        }
        #endregion
        #region Method: ArrayList GetUnnownMarkersInventory()
        public ArrayList GetUnnownMarkersInventory()
        {
            // Retrieve the inventory of markers in this DB
            ArrayList aInventory = GetNormalizedMarkersInventory();

            // We'll place the unknown markers here
            ArrayList aUnknown = new ArrayList();

            // Go through all of the markers
            foreach (string sMkr in aInventory)
            {
                // If we don't have them in the Map, then add them to the Unknown list
                if (!Map.IsRecognizedMarker(sMkr))
                    aUnknown.Add(sMkr);
            }

            // Alphabetize & return the result
            aUnknown.Sort();
            return aUnknown;
        }
        #endregion

        // Writing ---------------------------------------------------------------------------
		#region Override: bool Write(string sPathName)
		public override bool Write(string sPathName)
		{
			TransformOut();

			bool bResult = base.Write(sPathName);

			if (bResult == false)
			{
				// Give the user the bad news
				Messages.UnableToSaveFile(sPathName);
				Console.WriteLine("Unable to save file, PathName = " + sPathName + 
					"Error = " + base.Message);

				return false;
			}

			return true;
		}
		#endregion
	}


    #region NUnit Test_ScriptureDB
    [TestFixture] public class Test_ScriptureDB
    {
        // String Sets -----------------------------------------------------------------------
        string[][] m_vvIn;
        string[][] m_vvExpected;
        #region Method: void InitializeStringSet(int cSize)
        void InitializeStringSet(int cSize)
        {
            m_vvIn = new string[cSize][];
            m_vvExpected = new string[cSize][];
        }
        #endregion
        #region Method: void AddStringSet(int i, string[] vIn, string[] vExpected)
        void AddStringSet(int i, string[] vIn, string[] vExpected)
        {
            m_vvIn[i] = vIn;
            m_vvExpected[i] = vExpected;
        }
        #endregion
        #region Method: void WriteActualDataToConsole(ScriptureDB DB, string sTitle)
        void WriteActualDataToConsole(ScriptureDB DB, string sTitle)
        {
            Console.WriteLine("");
            Console.WriteLine(sTitle);
            string[] vs = DB.ExtractData();
            foreach (string s in vs)
                Console.WriteLine("<" + s + ">");
        }
        #endregion

        // Some Data Sets --------------------------------------------------------------------
        #region Data: 1 - data I did when originally coding these routines circa 2005
        #region m_vIn_1
        string[] m_vIn_1 = 
		{
			"\\id LUK - Tera",
			"\\p",
			"\\s LABAR MBARKANDI NU_ LUKA BU_LAKI",
			"\\c 1",
			"\\p",
			"\\v 1 Nu_ke nduki khang wa shi bu_lar dyine dyiku nu_ shiki chele ?aarem.\\f g",
			"\\fr 3.7",
			"\\fk Baftisma:",
			"\\fk* Dubi ma'anar wanasu kalmoni.",
			"\\f*",
			"\\v 2 ba wani nu_ mbu Yesu ku nu_ guma-paali nu_kya.",
			"\\rc 9.19: ",
			"\\rt Mat 14.1-2; Mar 6.14-15; Luk 9.7-8\\rc*",
			"\\v 3 Wa shi mbiri nga ndu_ vara njel nga su_ dyiku na yo kap ku_me",
			"\\s Another section head",
			"\\p",
			"\\v 4 Sarchi nu_ke Hiridus kuji Yahuda.\\rc 12.1: ",
			"\\rt Mat 16.6; Mar 8.15\\rc* ",
			"\\v 5 Ku_lmbu_r ?aanda dya nu_ sawar Magham ku,",
			"\\v 6 Wa geemaku, yang aa da woi kha war ?a ku_me Elizabet kwarandan.",
			"\\q1",
			"\\v 7 Sarchi ngguri Zakariya wa ?u_ ma tlu_nar\\f e \\fr 2:41",
			"\\fk Bikin Detarewa: \\fk* Dubi ma'anar wanasu kalmoni.\\f* mbu_ kib ?u_ Magham",
			"\\v 8 Tu_ dat Zakariya ku maa va dye-zhi mu_nkandi",
			"\\v 9,10,11 mbu_ kib ?u_ Magham ku_me naake me-ghayan",
		};
        #endregion
        #region m_vExpectedParatext_1
        string[] m_vExpectedParatext_1 = 
		{
			"\\id LUK - Tera",
			"\\p",
			"\\s LABAR MBARKANDI NU_ LUKA BU_LAKI",
			"\\p",
			"\\c 1",
			"\\v 1 Nu_ke nduki khang wa shi bu_lar dyine dyiku nu_ shiki chele ?aarem." +
                "\\f + \\fr 3.7\\fr* Baftisma: Dubi ma'anar wanasu kalmoni.\\f*",
			"\\v 2 ba wani nu_ mbu Yesu ku nu_ guma-paali nu_kya." +
                "\\x + \\xo 9.19:\\xo* Mat 14.1-2; Mar 6.14-15; Luk 9.7-8\\x*",
			"\\v 3 Wa shi mbiri nga ndu_ vara njel nga su_ dyiku na yo kap ku_me",
			"\\s Another section head",
			"\\p",
			"\\v 4 Sarchi nu_ke Hiridus kuji Yahuda.\\x + \\xo 12.1:\\xo* Mat 16.6; Mar 8.15\\x*",
			"\\v 5 Ku_lmbu_r ?aanda dya nu_ sawar Magham ku,",
			"\\v 6 Wa geemaku, yang aa da woi kha war ?a ku_me Elizabet kwarandan.",
			"\\q",
			"\\v 7 Sarchi ngguri Zakariya wa ?u_ ma tlu_nar\\f + \\fr 2:41\\fr* " +
			    "Bikin Detarewa: Dubi ma'anar wanasu kalmoni.\\f* mbu_ kib ?u_ Magham",
			"\\v 8 Tu_ dat Zakariya ku maa va dye-zhi mu_nkandi",
			"\\v 9-11 mbu_ kib ?u_ Magham ku_me naake me-ghayan",
		};
        #endregion
        #region m_vExpectedToolbox_1
        string[] m_vExpectedToolbox_1 =
		{
			"\\rcrd",
			"\\id LUK - Tera",
			"\\rcrd 1",
			"\\p",
			"\\s LABAR MBARKANDI NU_ LUKA BU_LAKI",
			"\\p",
			"\\c 1",
			"\\v 1",
			"\\vt Nu_ke nduki khang wa shi bu_lar dyine dyiku nu_ shiki chele ?aarem.|fn ",
			"\\ft 3.7 Baftisma: Dubi ma'anar wanasu kalmoni.",
			"\\v 2",
			"\\vt ba wani nu_ mbu Yesu ku nu_ guma-paali nu_kya. ",
			"\\cf 9.19: Mat 14.1-2; Mar 6.14-15; Luk 9.7-8",
			"\\v 3",
			"\\vt Wa shi mbiri nga ndu_ vara njel nga su_ dyiku na yo kap ku_me",
			"\\rcrd 2",
			"\\s Another section head",
			"\\p",
			"\\v 4",
			"\\vt Sarchi nu_ke Hiridus kuji Yahuda.",
			"\\cf 12.1: Mat 16.6; Mar 8.15",
			"\\v 5",
			"\\vt Ku_lmbu_r ?aanda dya nu_ sawar Magham ku,",
			"\\v 6",
			"\\vt Wa geemaku, yang aa da woi kha war ?a ku_me Elizabet kwarandan.",
			"\\q",
			"\\v 7",
			"\\vt Sarchi ngguri Zakariya wa ?u_ ma tlu_nar|fn mbu_ kib ?u_ Magham",
			"\\ft 2:41 Bikin Detarewa: Dubi ma'anar wanasu kalmoni.",
			"\\v 8",
			"\\vt Tu_ dat Zakariya ku maa va dye-zhi mu_nkandi",
			"\\v 9-11",
			"\\vt mbu_ kib ?u_ Magham ku_me naake me-ghayan",
		};
        #endregion
        #endregion
        #region Data: 2 - From Napu 1Co
        #region m_vIn_2
        string[] m_vIn_2 =
            {
                "\\id 1CO",
                "\\h 1 Korintu",
                "\\mt2 Surana Suro Paulu au Nguru-nguruna",
                "\\mt2 i to Sarani i Kota",
                "\\mt Korintu",
                "",
                "\\c 1",
                "\\p",
                "\\v 1-2 Sura ide au hangko iriko Paulu hai hangko i Sostenes, kipakatu irikamu to " +
                    "Sarani i kota Korintu au nakakiokau Pue Ala mewali taunaNa. Iami au mopamalelahamokau " +
                    "anti pohintuwumi hai Kerisitu Yesu hihimbela hai ope-ope tauna au maida iumba pea au " +
                    "mekakae i Yesu Kerisitu, Amputa hai Ampunda. Iko ide, naangkana Pue Ala mewali surona " +
                    "Yesu Kerisitu moula peundeaNa.",
                "\\v 3 Ngkaya tabea! Mekakaena i Pue Ala Umanta hai i Pue Yesu Kerisitu bona Ia mowatikau " +
                    "hangko i kabulana laluNa hai moweikau roa ngkatuwo.",
                "\\s Paulu manguli ngkaya kamaroana i Pue Ala",
                "\\p",
                "\\v 4 Halalu, barana mengkangkaroo manguli ngkaya kamaroana i Pue Ala anti kaikamuna, lawi " +
                    "Ia moweikau pewati hangko i kabulana laluNa anti pohintuwumi hai Kerisitu Yesu.",
                "\\v 5 Anti pohintuwumi hai Kerisitu, Naweimokau hinangkana pewati au mobundu i katuwomi: " +
                    "Napopaisaamokau paturo au tou, hai Naweimokau kapande mopahawe paturo iti.",
                "\\v 6 Ope-ope iti mopakanoto kamarohona pepoinalaimi i bambari kana i Kerisitu au kipahaweakau,",
                "\\v 7 duuna barapi ara kamakurana pewati au nilambi. Nilambimi paka-pakana kapande hangko i " +
                    "Inao Malelaha, hai mampegiakau kahawena hule Amputa Yesu Kerisitu.",
                "\\v 8 Ia ina mopakaroho pepoinalaimi duuna i kahopoa dunia, bona ane ikamu nakahawei Amputa " +
                    "Yesu Kerisitu i kahaweNa hule, barapi ara kamasalami.",
                "\\v 9 Pue Ala mokakiokau bona mohintuwukau hihimbela hai AnaNa, Yesu Kerisitu Amputa, hai " +
                    "Ia mopabukei liliu dandiNa irikita.",
                "\\s Paulu mokambaroahe lawi barahe mohintuwu",
                "\\p",
                "\\v 10 Halalu, hangko i kuasa au naweina Amputa Yesu Kerisitu, merapina bona mohintuwukau " +
                    "ope-ope. Ineekau mopaara posisala. Hangangaa mohintuwukau hai hampepekirikau.",
                "\\v 11 Halalungku, tunggaiaku manguli nodo, lawi arahe halalunta hasou-souna Kloe au mopahawe " +
                    "iriko kaarana posisalami.",
                "\\v 12 Mololitana nodo lawi arakau au mampeinao mampopontani hadua hai hadua. Arakau au manguli: " +
                    "<<Iko meula i Paulu.>> Arakau au manguli: <<Iko meula i Apolo.>> Au ntanina wori manguli: " +
                    "<<Iko meula i Peturu.>>\\f + \\fr 1:12 \\ft Peturu \\ft - Basa Yunani: Kefas \\ft Peita " +
                    "wori 3:22; 9:5 hai 15:5. Peturu ba Simo Peturu rahanga wori Kefas.\\f* Ara mbuli au manguli: " +
                    "<<Iko meula i Kerisitu.>>",
                "\\v 13 Maroa pae ane ikamu au meula i Kerisitu mosisala nodo? Ba iko au mate rapaku i kau " +
                    "mohuru dosami? Ba rariukau bona mewalikau topegurungku? Bara!",
                "\\v 14 Kuuli ngkaya kamaroana lawi barakau ara au kuriu, batenda pea Krispus hai Gayu,",
                "\\v 15 bona datihe ara au manguli kuriukau mewali topegurungku.",
                "\\v 16 (Oo, Setepanu wori hantambi, iko mpuu au moriuhe. Agayana i raoa hangko irihira, barapi " +
                    "kukatuinao tauna ntanina au kuriu.)",
                "\\v 17 Kerisitu barana moangka mewali suroNa bona moriuna tauna, agayana Naangkana bona " +
                    "moantina Ngkora Marasa. Ane kupahawe Ngkora Marasa, barana mololita hangko i kamapandeku " +
                    "datihe ara tauna au mampoinalai lolitangku anti kamapandeku moleri barahe moisa kuasana " +
                    "Kerisitu au mate i kau mombehape.",
                "\\s Hangko i Ngkora Marasa taisa kuasana hai kamanotona laluna Pue Ala",
                "\\p",
                "\\v 18 Kipahawe bambari au manguli Kerisitu mohuru dosa manusia i karapapateNa i kau mombehape." +
                    "Ane tauna au monontohi naraka mohadi bambari iti, rauli: <<Iti lolita tontuli.>> Agayana " +
                    "ane ikita au nahoremake Pue Ala hangko i huku dosanta mohadi bambari iti, tauli: <<Itimi " +
                    "kuasana Pue Ala!>>",
                "\\v 19 I lalu Sura Malelaha, Pue Ala manguli node:",
                "\\q <<Ina Kupaope kapande manusia,",
                "\\q hai bara ina Kupake akalanda tauna au mapande.>>",
                "\\qr (Yesaya 29:14)",
                "\\p",
                "\\v 20 Apa bundunda tauna au mapande? Apa bundunda guru agama to Yahudi? Apa bundunda tauna au " +
                    "mapande mololita i lalu dunia ide? I peitana Pue Ala, ope-ope kapande manusia Naimba " +
                    "mbero-mbero pea.",
                "\\v 21 Lawi hangko i kapande manusia, ikita bara peisa moisa Pue Ala. Anti kamapandena Pue" +
                    "Ala, Ia mobotusi bona hema pea au mepoinalai i Ngkora Marasa au kipahawe, tauna iti ina " +
                    "Nahoremahe hangko i huku dosanda. Agayana ane kipahawe Ngkora Marasa, tauna au bara " +
                    "mepoinalai manguli: <<Iti lolita tontuli.>>"
            };
        #endregion
        #region m_vExpectedParatext_2
        string[] m_vExpectedParatext_2 =
            {
                "\\id 1CO",
                "\\h 1 Korintu",
                "\\mt2 Surana Suro Paulu au Nguru-nguruna",
                "\\mt2 i to Sarani i Kota",
                "\\mt Korintu",
                "\\p",
                "\\c 1",
                "\\v 1-2 Sura ide au hangko iriko Paulu hai hangko i Sostenes, kipakatu irikamu to " +
                    "Sarani i kota Korintu au nakakiokau Pue Ala mewali taunaNa. Iami au mopamalelahamokau " +
                    "anti pohintuwumi hai Kerisitu Yesu hihimbela hai ope-ope tauna au maida iumba pea au " +
                    "mekakae i Yesu Kerisitu, Amputa hai Ampunda. Iko ide, naangkana Pue Ala mewali surona " +
                    "Yesu Kerisitu moula peundeaNa.",
                "\\v 3 Ngkaya tabea! Mekakaena i Pue Ala Umanta hai i Pue Yesu Kerisitu bona Ia mowatikau " +
                    "hangko i kabulana laluNa hai moweikau roa ngkatuwo.",
                "\\s Paulu manguli ngkaya kamaroana i Pue Ala",
                "\\p",
                "\\v 4 Halalu, barana mengkangkaroo manguli ngkaya kamaroana i Pue Ala anti kaikamuna, lawi " +
                    "Ia moweikau pewati hangko i kabulana laluNa anti pohintuwumi hai Kerisitu Yesu.",
                "\\v 5 Anti pohintuwumi hai Kerisitu, Naweimokau hinangkana pewati au mobundu i katuwomi: " +
                    "Napopaisaamokau paturo au tou, hai Naweimokau kapande mopahawe paturo iti.",
                "\\v 6 Ope-ope iti mopakanoto kamarohona pepoinalaimi i bambari kana i Kerisitu au kipahaweakau,",
                "\\v 7 duuna barapi ara kamakurana pewati au nilambi. Nilambimi paka-pakana kapande hangko i " +
                    "Inao Malelaha, hai mampegiakau kahawena hule Amputa Yesu Kerisitu.",
                "\\v 8 Ia ina mopakaroho pepoinalaimi duuna i kahopoa dunia, bona ane ikamu nakahawei Amputa " +
                    "Yesu Kerisitu i kahaweNa hule, barapi ara kamasalami.",
                "\\v 9 Pue Ala mokakiokau bona mohintuwukau hihimbela hai AnaNa, Yesu Kerisitu Amputa, hai " +
                    "Ia mopabukei liliu dandiNa irikita.",
                "\\s Paulu mokambaroahe lawi barahe mohintuwu",
                "\\p",
                "\\v 10 Halalu, hangko i kuasa au naweina Amputa Yesu Kerisitu, merapina bona mohintuwukau " +
                    "ope-ope. Ineekau mopaara posisala. Hangangaa mohintuwukau hai hampepekirikau.",
                "\\v 11 Halalungku, tunggaiaku manguli nodo, lawi arahe halalunta hasou-souna Kloe au mopahawe " +
                    "iriko kaarana posisalami.",
                "\\v 12 Mololitana nodo lawi arakau au mampeinao mampopontani hadua hai hadua. Arakau au manguli: " +
                    "<<Iko meula i Paulu.>> Arakau au manguli: <<Iko meula i Apolo.>> Au ntanina wori manguli: " +
                    "<<Iko meula i Peturu.>>\\f + \\fr 1:12\\fr* Peturu - Basa Yunani: Kefas Peita " +
                    "wori 3:22; 9:5 hai 15:5. Peturu ba Simo Peturu rahanga wori Kefas.\\f* Ara mbuli au manguli: " +
                    "<<Iko meula i Kerisitu.>>",
                "\\v 13 Maroa pae ane ikamu au meula i Kerisitu mosisala nodo? Ba iko au mate rapaku i kau " +
                    "mohuru dosami? Ba rariukau bona mewalikau topegurungku? Bara!",
                "\\v 14 Kuuli ngkaya kamaroana lawi barakau ara au kuriu, batenda pea Krispus hai Gayu,",
                "\\v 15 bona datihe ara au manguli kuriukau mewali topegurungku.",
                "\\v 16 (Oo, Setepanu wori hantambi, iko mpuu au moriuhe. Agayana i raoa hangko irihira, barapi " +
                    "kukatuinao tauna ntanina au kuriu.)",
                "\\v 17 Kerisitu barana moangka mewali suroNa bona moriuna tauna, agayana Naangkana bona " +
                    "moantina Ngkora Marasa. Ane kupahawe Ngkora Marasa, barana mololita hangko i kamapandeku " +
                    "datihe ara tauna au mampoinalai lolitangku anti kamapandeku moleri barahe moisa kuasana " +
                    "Kerisitu au mate i kau mombehape.",
                "\\s Hangko i Ngkora Marasa taisa kuasana hai kamanotona laluna Pue Ala",
                "\\p",
                "\\v 18 Kipahawe bambari au manguli Kerisitu mohuru dosa manusia i karapapateNa i kau mombehape." +
                    "Ane tauna au monontohi naraka mohadi bambari iti, rauli: <<Iti lolita tontuli.>> Agayana " +
                    "ane ikita au nahoremake Pue Ala hangko i huku dosanta mohadi bambari iti, tauli: <<Itimi " +
                    "kuasana Pue Ala!>>",
                "\\v 19 I lalu Sura Malelaha, Pue Ala manguli node:",
                "\\q <<Ina Kupaope kapande manusia,",
                "\\q hai bara ina Kupake akalanda tauna au mapande.>>",
                "\\p",
                "\\v 20 Apa bundunda tauna au mapande? Apa bundunda guru agama to Yahudi? Apa bundunda tauna au " +
                    "mapande mololita i lalu dunia ide? I peitana Pue Ala, ope-ope kapande manusia Naimba " +
                    "mbero-mbero pea.",
                "\\v 21 Lawi hangko i kapande manusia, ikita bara peisa moisa Pue Ala. Anti kamapandena Pue" +
                    "Ala, Ia mobotusi bona hema pea au mepoinalai i Ngkora Marasa au kipahawe, tauna iti ina " +
                    "Nahoremahe hangko i huku dosanda. Agayana ane kipahawe Ngkora Marasa, tauna au bara " +
                    "mepoinalai manguli: <<Iti lolita tontuli.>>"
            };
        #endregion
        #region m_vExpectedToolbox_2
        string[] m_vExpectedToolbox_2 =
            {
                "\\rcrd",
                "\\id 1CO",
                "\\rcrd 1",
                "\\h 1 Korintu",
                "\\st Surana Suro Paulu au Nguru-nguruna",
                "\\st i to Sarani i Kota",
                "\\mt Korintu",
                "\\p",
                "\\c 1",
                "\\v 1-2",
                "\\vt Sura ide au hangko iriko Paulu hai hangko i Sostenes, kipakatu irikamu to " +
                    "Sarani i kota Korintu au nakakiokau Pue Ala mewali taunaNa. Iami au mopamalelahamokau " +
                    "anti pohintuwumi hai Kerisitu Yesu hihimbela hai ope-ope tauna au maida iumba pea au " +
                    "mekakae i Yesu Kerisitu, Amputa hai Ampunda. Iko ide, naangkana Pue Ala mewali surona " +
                    "Yesu Kerisitu moula peundeaNa.",
                "\\v 3",
                "\\vt Ngkaya tabea! Mekakaena i Pue Ala Umanta hai i Pue Yesu Kerisitu bona Ia mowatikau " +
                    "hangko i kabulana laluNa hai moweikau roa ngkatuwo.",
                "\\s Paulu manguli ngkaya kamaroana i Pue Ala",
                "\\p",
                "\\v 4",
                "\\vt Halalu, barana mengkangkaroo manguli ngkaya kamaroana i Pue Ala anti kaikamuna, lawi " +
                    "Ia moweikau pewati hangko i kabulana laluNa anti pohintuwumi hai Kerisitu Yesu.",
                "\\v 5",
                "\\vt Anti pohintuwumi hai Kerisitu, Naweimokau hinangkana pewati au mobundu i katuwomi: " +
                    "Napopaisaamokau paturo au tou, hai Naweimokau kapande mopahawe paturo iti.",
                "\\v 6",
                "\\vt Ope-ope iti mopakanoto kamarohona pepoinalaimi i bambari kana i Kerisitu au kipahaweakau,",
                "\\v 7",
                "\\vt duuna barapi ara kamakurana pewati au nilambi. Nilambimi paka-pakana kapande hangko i " +
                    "Inao Malelaha, hai mampegiakau kahawena hule Amputa Yesu Kerisitu.",
                "\\v 8",
                "\\vt Ia ina mopakaroho pepoinalaimi duuna i kahopoa dunia, bona ane ikamu nakahawei Amputa " +
                    "Yesu Kerisitu i kahaweNa hule, barapi ara kamasalami.",
                "\\v 9",
                "\\vt Pue Ala mokakiokau bona mohintuwukau hihimbela hai AnaNa, Yesu Kerisitu Amputa, hai " +
                    "Ia mopabukei liliu dandiNa irikita.",
                "\\rcrd 2",
                "\\s Paulu mokambaroahe lawi barahe mohintuwu",
                "\\p",
                "\\v 10",
                "\\vt Halalu, hangko i kuasa au naweina Amputa Yesu Kerisitu, merapina bona mohintuwukau " +
                    "ope-ope. Ineekau mopaara posisala. Hangangaa mohintuwukau hai hampepekirikau.",
                "\\v 11",
                "\\vt Halalungku, tunggaiaku manguli nodo, lawi arahe halalunta hasou-souna Kloe au mopahawe " +
                    "iriko kaarana posisalami.",
                "\\v 12",
                "\\vt Mololitana nodo lawi arakau au mampeinao mampopontani hadua hai hadua. Arakau au manguli: " +
                    "<<Iko meula i Paulu.>> Arakau au manguli: <<Iko meula i Apolo.>> Au ntanina wori manguli: " +
                    "<<Iko meula i Peturu.>>|fn Ara mbuli au manguli: " +
                    "<<Iko meula i Kerisitu.>>",
                "\\ft 1:12 Peturu - Basa Yunani: Kefas Peita " +
                    "wori 3:22; 9:5 hai 15:5. Peturu ba Simo Peturu rahanga wori Kefas.",
                "\\v 13",
                "\\vt Maroa pae ane ikamu au meula i Kerisitu mosisala nodo? Ba iko au mate rapaku i kau " +
                    "mohuru dosami? Ba rariukau bona mewalikau topegurungku? Bara!",
                "\\v 14",
                "\\vt Kuuli ngkaya kamaroana lawi barakau ara au kuriu, batenda pea Krispus hai Gayu,",
                "\\v 15",
                "\\vt bona datihe ara au manguli kuriukau mewali topegurungku.",
                "\\v 16",
                "\\vt (Oo, Setepanu wori hantambi, iko mpuu au moriuhe. Agayana i raoa hangko irihira, barapi " +
                    "kukatuinao tauna ntanina au kuriu.)",
                "\\v 17",
                "\\vt Kerisitu barana moangka mewali suroNa bona moriuna tauna, agayana Naangkana bona " +
                    "moantina Ngkora Marasa. Ane kupahawe Ngkora Marasa, barana mololita hangko i kamapandeku " +
                    "datihe ara tauna au mampoinalai lolitangku anti kamapandeku moleri barahe moisa kuasana " +
                    "Kerisitu au mate i kau mombehape.",
                "\\rcrd 3",
                "\\s Hangko i Ngkora Marasa taisa kuasana hai kamanotona laluna Pue Ala",
                "\\p",
                "\\v 18",
                "\\vt Kipahawe bambari au manguli Kerisitu mohuru dosa manusia i karapapateNa i kau mombehape." +
                    "Ane tauna au monontohi naraka mohadi bambari iti, rauli: <<Iti lolita tontuli.>> Agayana " +
                    "ane ikita au nahoremake Pue Ala hangko i huku dosanta mohadi bambari iti, tauli: <<Itimi " +
                    "kuasana Pue Ala!>>",
                "\\v 19",
                "\\vt I lalu Sura Malelaha, Pue Ala manguli node:",
                "\\q",
                "\\vt <<Ina Kupaope kapande manusia,",
                "\\q",
                "\\vt hai bara ina Kupake akalanda tauna au mapande.>>",
                "\\qr (Yesaya 29:14)",
                "\\p",
                "\\v 20",
                "\\vt Apa bundunda tauna au mapande? Apa bundunda guru agama to Yahudi? Apa bundunda tauna au " +
                    "mapande mololita i lalu dunia ide? I peitana Pue Ala, ope-ope kapande manusia Naimba " +
                    "mbero-mbero pea.",
                "\\v 21",
                "\\vt Lawi hangko i kapande manusia, ikita bara peisa moisa Pue Ala. Anti kamapandena Pue" +
                    "Ala, Ia mobotusi bona hema pea au mepoinalai i Ngkora Marasa au kipahawe, tauna iti ina " +
                    "Nahoremahe hangko i huku dosanda. Agayana ane kipahawe Ngkora Marasa, tauna au bara " +
                    "mepoinalai manguli: <<Iti lolita tontuli.>>"                
            };
        #endregion
        #endregion

        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            // Initialize the registry (the Map process needs it)
             JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";
         }
        #endregion
        #region Method: void CompareTransforms(ScriptureDB DB, string[] vDataExpected)
        void CompareTransforms(ScriptureDB DB, string[] vDataExpected)
        {
            string[] vDataActual = DB.ExtractData();
            Assert.AreEqual(vDataExpected.Length, vDataActual.Length);
            for (int i = 0; i < vDataExpected.Length; i++)
                Assert.AreEqual(vDataExpected[i], vDataActual[i]);
        }
        #endregion

        // General Tests ---------------------------------------------------------------------
        #region TEST: RemoveShoeboxSpecialMarkers
        [Test] public void RemoveShoeboxSpecialMarkers()
        {
            string[] vDataIn =
                {
                    "\\_sh v3.0  540  SHW-Scripture",
                    "\\_DateStampHasFourDigitYear",
                    "",
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\mt Markus",
                    "\\btmt Markus"
                };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\mt Markus",
                    "\\btmt Markus"
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB._RemoveShoeboxSpecialMarkers();
            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: NormalizeMarkers
        [Test]
        public void NormalizeMarkers()
        {
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\mt2 Buku",
                    "\\btst The Book of",
                    "\\mt1 Markus",
                    "\\btmt Markus",
                    "\\ov Just a random old version",
                    "\\nq Just a random note"
               };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\id MRK Injil Markus dalam bahasa Melayu Kupang, NTT, Indonesia",
                    "\\h Markus",
                    "\\st Buku",
                    "\\btst The Book of",
                    "\\mt Markus",
                    "\\btmt Markus",
                    "\\ov Just a random old version",
                    "\\nt Just a random note"
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB._NormalizeMarkers();
            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: RemoveDiscardedMarkers
        [Test]
        public void RemoveDiscardedMarkers()
        {
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
                    "\\tb This is how it looks in the TB version.",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1",
                    "\\e"
               };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1"
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB._RemoveDiscardedMarkers();
            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: MoveChaptersIntoParagraphs
        [Test] public void MoveChaptersIntoParagraphs()
        {
            #region DATA SET
            string[] vDataIn1 =
                {
                    "\\rcrd mrk",
                    "\\c 2",
                    "\\ms This is a Major Section Head",
                    "\\mr (major cross reference)",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1"
               };
            string[] vDataExpected1 =
                {
                    "\\rcrd mrk",
                    "\\ms This is a Major Section Head",
                    "\\mr (major cross reference)",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
                    "\\c 2",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\q",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
				    "\\cf 1:2: Maleakhi 3:1"
               };
            #endregion

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn1);
            DB._MoveChaptersIntoParagraphs();
            CompareTransforms(DB, vDataExpected1);
        }
        #endregion
        #region TEST: EnsureParagraphAfterPicture
        [Test] public void EnsureParagraphAfterPicture()
        {
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja,",
                    "\\cat PictureFileName.gif",
                    "\\cap This is a picture",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu."
               };
            string[] vDataExpected =
                {
                    "\\rcrd mrk",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja,",
                    "\\cat PictureFileName.gif",
                    "\\cap This is a picture",
				    "\\p",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB._EnsureParagraphAfterPicture();
            CompareTransforms(DB, vDataExpected);
        }
        #endregion
        #region TEST: NormalizedMarkersInventory
        [Test] public void NormalizedMarkersInventory()
        {
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(Test_DSection.m_SectionTest1);
            DB.TransformIn();

            ArrayList aMarkers = DB.GetNormalizedMarkersInventory();

            Assert.AreNotEqual(-1, aMarkers.BinarySearch("c"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("v"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("vt"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("s"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("nt"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("s"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("r"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("q"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("cf"));
            Assert.AreNotEqual(-1, aMarkers.BinarySearch("ft"));

            Assert.AreEqual(-1, aMarkers.BinarySearch("btvt"));
            Assert.AreEqual(-1, aMarkers.BinarySearch("bts"));
        }
        #endregion
        #region TEST: UnknownMarkersInventory
        [Test] public void UnknownMarkersInventory()
            // We want to make certain that all of the markers we think are
            // in the Map are indeed in the map. The easiest way to do this is
            // to test all of the DSection test strings, and verify that
            // the resultant Inventory shows up as empty.
            //
            // But first, we test to make sure that the method does indeed
            // return something ifi there are bad markers present!
        {
            // Test that some wrong markers do indeed show up
            #region TestData
            string[] vDataIn =
                {
                    "\\rcrd mrk",
                    "\\c 2",
                    "\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 1",
				    "\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					    "orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					    "Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					    "jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				    "\\qw",
				    "\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				    "\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					    "open the way for You.",
                    "\\pr Should not be here, either.",
				    "\\cf 1:2: Maleakhi 3:1"
               };
            #endregion
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB.TransformIn();
            ArrayList a = DB.GetUnnownMarkersInventory();
            Assert.AreNotEqual(-1, a.BinarySearch("qw"));
            Assert.AreNotEqual(-1, a.BinarySearch("pr"));

            // Test all of the DSection data to show that none of those markers
            // are Unknown.
            string[][] v = new string[10][];
            v[0] = Test_DSection.m_SectionTest1;
            v[1] = Test_DSection.m_SectionTest2;
            v[2] = Test_DSection.m_SectionTest3;
            v[3] = Test_DSection.m_SectionTest4;
            v[4] = Test_DSection.m_SectionTest5;
            v[5] = Test_DSection.m_SectionTest6;
            v[6] = Test_DSection.m_SectionTest7;
            v[7] = Test_DSection.m_SectionTest8;
            v[8] = Test_DSection.m_SectionTest9;
            v[9] = Test_DSection.m_SectionTest10;
            for (int i = 0; i < v.Length; i++)
            {
                DB = new ScriptureDB();
                DB.Initialize(v[i]);
                DB.TransformIn();
                a = DB.GetUnnownMarkersInventory();
                Assert.AreEqual(0, a.Count);
            }
        }
        #endregion
        #region TEST: MoveTitlesIntoRecord
        [Test] public void MoveTitlesIntoRecord()
        {
            #region Data: Bwazza, with a few extra fields added to the Intro section
            string[] vIn_Bwazza =
			{
                "\\rcrd",
                "\\h Luk",
                "\\st Mǝlikakar",
                "\\mt Mi Luk",
                "\\id LUK",
                "\\nt This is a note.",
                "\\nt And this is another note.",
                "\\cy Copyright statement.",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            string[] vExpected_Bwazza =
			{
                "\\rcrd",
                "\\id LUK",
                "\\nt This is a note.",
                "\\nt And this is another note.",
                "\\cy Copyright statement.",
                "\\rcrd 1",
                "\\h Luk",
                "\\st Mǝlikakar",
                "\\mt Mi Luk",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            #endregion
            #region Data: Amarasi - example of Toolbox, but with \_... fields removed
            // Chuck: Indonesia, Amarasi, Luke beginning
            string[] vIn_Amarasi = 
			{
                "\\rcrd mrk",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            string[] vExpected_Amarasi = 
			{
                "\\rcrd mrk",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_Bwazza, vExpected_Bwazza);
            AddStringSet(1, vIn_Amarasi, vExpected_Amarasi);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);

                // Move the fields to the second record; they should line up with
                // our "Expected" version.
                DB._MoveTitlesIntoRecord(2);
                CompareTransforms(DB, m_vvExpected[i]);

                // Move them back to the first record; they should now compare with
                // the original "In" version.
                DB._MoveTitlesIntoRecord(1);
                CompareTransforms(DB, m_vvIn[i]);
            }
        }
        #endregion
        #region TEST: CombineBTs
        [Test] public void CombineBTs()
        {
            // Test Data
            #region string[] vDataIn
            string[] vDataIn =
			{
				"\\s Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				"\\bts Yohanis, the.One.who.habitually *Baptizes, opens the way/path " +
					"for the Lord Yesus",
				"\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
				"\\p",
				"\\v 2",
				"\\vt Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					"orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					"Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					"jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				"\\btvt Yesus had not yet begun His work, {te} God had ordered (to do " +
					"s.t.) a person, his name was Yohanis.  Yohanis had to go prepare " +
					"the way for Yesus' coming.  Cause long before, God had already " +
					"used His *prophet1 (spokesperson). His name was Grandfather " +
					"Yesaya. He actually wrote (= wrote before the event) saying:",
				"\\q",
				"\\vt <<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				"\\btvt \"Listen! I am ordering/commanding a person of Mine, to go " +
					"open the way for You.",
				"\\cf 1:2: Maleakhi 3:1",
				"\\cat c:\\graphics\\HK-LB\\LB00296C.tif",
				"\\ref width:5.0cm;textWrapping:around;horizontalPosition:right",
				"\\cap Yohanis, Tukang Sarani",
				"\\btcap Yohanis, The Habitual Baptizer",
				"\\p",
				"\\vt Abis dong mangaku sala, ju dia sarani sang dong di kali Yarden.",
				"\\btvt After they *confessed their *sins,  then he *baptised them " +
					"in the Yarden stream/river.",
				"\\nt calop ko sarani ; sunge (implies always water) vs. kali (can " +
					"have water or be just the bed) > kali mati  and parigi mati = dry"
			};
            #endregion
            #region string[] vDataExpected
            string[] vDataExpected =
			{
                "",                         // This is from the inserted "\rcrd" marker
				"1",                        // This "1" is from the inserted "\rcrd 1" marker
                "",                         // This is from the inserted "\mt" marker
				"Yohanis, Tukang Sarani, buka jalan kasi Tuhan Yesus",
				"(Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
				"",
				"2",
				"Yesus balóm mulai Dia pung karja, te Tuhan Allah su utus satu " +
					"orang, dia pung nama Yohanis. Yohanis musti pi sadia jalan kasi " +
					"Yesus pung datang. Te dolu sakali, Tuhan Allah su pake Dia pung " +
					"jubir. Dia pung nama Ba'i Yesaya. Dia su tulis memang bilang:",
				"",
				"<<Dengar ó! Beta suru Beta pung orang, ko pi buka jalan kasi sang Lu.",
				"1:2: Maleakhi 3:1",
				"c:\\graphics\\HK-LB\\LB00296C.tif",
				"width:5.0cm;textWrapping:around;horizontalPosition:right",
				"Yohanis, Tukang Sarani",
				"",
				"Abis dong mangaku sala, ju dia sarani sang dong di kali Yarden.",
				"calop ko sarani ; sunge (implies always water) vs. kali (can " +
					"have water or be just the bed) > kali mati  and parigi mati = dry"
			};
            #endregion
            #region string[] vBTExpected
            string[] vBTExpected =
			{
                "",                         // This is from the inserted "\rcrd" marker
				"",                         // This "" is from the inserted "\rcrd 1" marker
                "",                         // This is from the inserted "\m" marker
				"Yohanis, the.One.who.habitually *Baptizes, opens the way/path " +
					"for the Lord Yesus",
				"",
				"",
				"",
				"Yesus had not yet begun His work, {te} God had ordered (to do " +
					"s.t.) a person, his name was Yohanis.  Yohanis had to go prepare " +
					"the way for Yesus' coming.  Cause long before, God had already " +
					"used His *prophet1 (spokesperson). His name was Grandfather " +
					"Yesaya. He actually wrote (= wrote before the event) saying:",
				"",
				"\"Listen! I am ordering/commanding a person of Mine, to go " +
					"open the way for You.",
				"",
				"",
				"",
				"Yohanis, The Habitual Baptizer",
				"",
				"After they *confessed their *sins,  then he *baptised them " +
					"in the Yarden stream/river.",
				""
			};
            #endregion

            // Do the Transform process
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB.TransformIn();

            // Test the results
            for (int i = 0; i < DB.Count; i++)
            {
                SfField f = DB.GetFieldAt(i);

                Assert.AreEqual(vDataExpected[i], f.Data);
                Assert.AreEqual(vBTExpected[i],   f.BT);
            }
        }
        #endregion
        #region TEST: MoveMajorSectionParagraphs
        [Test] public void MoveMajorSectionParagraphs()
        {
            string[] vDataIn =
                {
                    "\\rcrd JER 001",
                    "\\s Jeremiah's Prayer",
                    "\\p",
                    "\\v 1",
                    "\\vt I know, Lord, that our lives are not our own.",
                    "\\ms Judah in Trouble",
                    "\\mr (Psalm 123:5)",
                    "\\rcrd JER 002",
                    "\\s Judah's Broken Covenant",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt The Lord gave another message to Jeremiah."
               };
            string[] vDataExpected =
                {
                    "\\rcrd JER 001",
                    "\\s Jeremiah's Prayer",
                    "\\p",
                    "\\v 1",
                    "\\vt I know, Lord, that our lives are not our own.",
                    "\\rcrd JER 002",
                    "\\ms Judah in Trouble",
                    "\\mr (Psalm 123:5)",
                    "\\s Judah's Broken Covenant",
				    "\\r (Matius 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
 				    "\\p",
				    "\\v 2",
				    "\\vt The Lord gave another message to Jeremiah."
               };

            // Do the test
            ScriptureDB DB = new ScriptureDB();
            DB.Initialize(vDataIn);
            DB._MoveMajorSectionParagraphs();
            CompareTransforms(DB, vDataExpected);
        }
        #endregion

        // Tests developed when adding the Paratext Import (some applies to Toolbox as well)
        #region TEST: NormalizeVerseText
        [Test] public void NormalizeVerseText()
        {
            #region Data: Koya - plain, Unicode special chars
            // Steven Payne, Africa, DRC, Koya, Luke 1:19-20
            string[] vIn_Koya =
                {
                    "\\p",
                    "\\v 19 Kija malaika wa:mutisio bɔ: «Ɛmɛ ma Gabhilieli. Ɛmɛ kʉ " +
                        "maakyɨa bʉmaɨ apɛɛ ʉ Asʉbɨɨ. Ɨyɔ ʉnatʉma nʉwɛ kʉbɨa ɨsaʉ yɛɛ " +
                        "ido ngia.",
                    "\\v 20 na, moni ngika kʉsɛmɛ ka=kyɨanɨa akʉ wati wɔngɔ. Ndʉ wʉbhaya " +
                        "kɔmʉ moni ngika kʉsɛmɛ, wa=bio tʉ kʉ ijangi; wambukubio tii bata " +
                        "kʉyaka kʉbʉya bisi ongo tʉ kʉ isiya moni ngikʉnanɨ ka=kyɨanɨa-ɔɔ.»"
                };
            string[] vExpected_Koya =
                {
                    "\\p",
                    "\\v 19",
                    "\\vt Kija malaika wa:mutisio bɔ: «Ɛmɛ ma Gabhilieli. Ɛmɛ kʉ " +
                        "maakyɨa bʉmaɨ apɛɛ ʉ Asʉbɨɨ. Ɨyɔ ʉnatʉma nʉwɛ kʉbɨa ɨsaʉ yɛɛ " +
                        "ido ngia.",
                    "\\v 20",
                    "\\vt na, moni ngika kʉsɛmɛ ka=kyɨanɨa akʉ wati wɔngɔ. Ndʉ wʉbhaya " +
                        "kɔmʉ moni ngika kʉsɛmɛ, wa=bio tʉ kʉ ijangi; wambukubio tii bata " +
                        "kʉyaka kʉbʉya bisi ongo tʉ kʉ isiya moni ngikʉnanɨ ka=kyɨanɨa-ɔɔ.»"
                };
            #endregion
            #region Data: Bwazza - contains a footnote
            // Katy Barnwell, Africa, Nigeria, Bwazza, Luke 1:11-18
            string[] vIn_Bwazza =
                {
                    "\\p",
                    "\\v 11 Nama mǝturanjar mi Ɓakuli pusuri aki, yi shama a bu mǝ li nǝ " +
                        "bama akǝ losǝna yolo peri.",
                    "\\v 12 Shina ma Zakariya shini, gri payi ndali wara wo pai.",
                    "\\v 13 Mǝ-turanjar nayi ama, “Ɓǝ wo ɓǝka payo wa, Zakariya, a ona jime " +
                        "molo. Mamo Alisabatu na ɓǝl muna ɓwabura, wara wa shiyi lulu ama " +
                        "Yohanna.",
                    "\\v 14 Ɓǝl muna ma na yinǝwo nǝ bumpwasa nǝ baɓwarashau, wara a ɓwai pas " +
                        "na pa baɓwarashau nǝ ɓǝlban mali.",
                    "\\v 15 Asalama ya gulo a dum Ɓakuli. Yika nu mur ɓla ngun inabi wa, sǝ " +
                        "yika nu mba wa, wara Lilim mi Ɓakuli na paturo a nyami kla ama ana ɓli.",
                    "\\v 16 Ya nyasa amǝ Israila pas aba ya ki mǝtelasǝm Ɓakuli.",
                    "\\v 17 Wara ya a dǝma a dum mǝtelasǝm, aɓa lilim nǝ rǝshana mi " +
                        "Iliya\\f + \\fr 1:17 \\ft shin Mal./Mal. 4:5-6\\ft*\\f*, ya pǝl ɓabuma " +
                        "teru akya muniya, wara amǝ shankrǝu aki shirǝɗenye mya amǝ ɓabum ɓwara, " +
                        "wara ya gǝlka ɓwampine aba ka ki Mǝtelasǝm.",
                    "\\v 18 Zakariya iulu mǝ-turanjare ama, \"Man palang sǝ man shile ama ma " +
                        "yi gǝr na pa? Ma ɓwagǝla sǝ mamon sheura kǝm.”"
                };
            string[] vExpected_Bwazza =
                {
                    "\\p",
                    "\\v 11",
                    "\\vt Nama mǝturanjar mi Ɓakuli pusuri aki, yi shama a bu mǝ li nǝ " +
                        "bama akǝ losǝna yolo peri.",
                    "\\v 12",
                    "\\vt Shina ma Zakariya shini, gri payi ndali wara wo pai.",
                    "\\v 13",
                    "\\vt Mǝ-turanjar nayi ama, “Ɓǝ wo ɓǝka payo wa, Zakariya, a ona jime " +
                        "molo. Mamo Alisabatu na ɓǝl muna ɓwabura, wara wa shiyi lulu ama " +
                        "Yohanna.",
                    "\\v 14",
                    "\\vt Ɓǝl muna ma na yinǝwo nǝ bumpwasa nǝ baɓwarashau, wara a ɓwai pas " +
                        "na pa baɓwarashau nǝ ɓǝlban mali.",
                    "\\v 15",
                    "\\vt Asalama ya gulo a dum Ɓakuli. Yika nu mur ɓla ngun inabi wa, sǝ " +
                        "yika nu mba wa, wara Lilim mi Ɓakuli na paturo a nyami kla ama ana ɓli.",
                    "\\v 16",
                    "\\vt Ya nyasa amǝ Israila pas aba ya ki mǝtelasǝm Ɓakuli.",
                    "\\v 17",
                    "\\vt Wara ya a dǝma a dum mǝtelasǝm, aɓa lilim nǝ rǝshana mi " +
                        "Iliya\\f + \\fr 1:17 \\ft shin Mal./Mal. 4:5-6\\ft*\\f*, ya pǝl ɓabuma " +
                        "teru akya muniya, wara amǝ shankrǝu aki shirǝɗenye mya amǝ ɓabum ɓwara, " +
                        "wara ya gǝlka ɓwampine aba ka ki Mǝtelasǝm.",
                    "\\v 18",
                    "\\vt Zakariya iulu mǝ-turanjare ama, \"Man palang sǝ man shile ama ma " +
                        "yi gǝr na pa? Ma ɓwagǝla sǝ mamon sheura kǝm.”"
                };
            #endregion
            #region Data: Napu - contains a verse bridge (\v 3-4)
            // Roger Hanna, Asia, Indonesia, Napu, 1 Cor 5:1-5
            string[] vIn_Napu =
                {
                    "\\s Paulu motuduhe mohuku hadua tauna au mampotambia towawinena umana",
                    "\\p",
                    "\\v 1 Ara kuhadi au mololita kana i babehianda tauna i olomi au bara " +
                        "hintoto. Ara hadua rangami au mampotambia towawinena umana. Mogalori " +
                        "i olonda tauna au bara moisa Pue Ala, barahe ara au mobabehi nodo.",
                    "\\v 2 Agayana ikamu, nauri ara au mobabehi nodo i olomi, mampemahile " +
                        "manikau! Katouana, hangangaa masusa lalumi, hai tauna iti hangangaa " +
                        "nipopeloho hangko i olomi.",
                    "\\v 3-4 Nauri karaona hangko irikamu, agayana lalungku arato hihimbela " +
                        "hai ikamu, hai ara mpuu kuasangku mampohawaakau. Mewali, hai kuasa " +
                        "au naweina Pue Yesu Kerisitu, kupakanotomi: tauna au kadake babehiana " +
                        "iti hangangaa rahuku. Ane mogulukau, niimbana hihimbela hai ikamu, " +
                        "hai kuperapi i lalu kuasana Yesu Amputa",
                    "\\v 5 bona nihuhu pea tauna iti i kuasana Datu Tokadake. Lempona, " +
                        "nitudumi meloho hangko i olomi, hai inee nipaliu maminggu sambela hai " +
                        "ikamu. Hangangaa nibabehi nodo, bona watana tauna iti molambi " +
                        "pehuku agayana inaona molambi katuwo maroa i kahawena hule Pue Yesu."
                };
            string[] vExpected_Napu =
                {
                    "\\s Paulu motuduhe mohuku hadua tauna au mampotambia towawinena umana",
                    "\\p",
                    "\\v 1",
                    "\\vt Ara kuhadi au mololita kana i babehianda tauna i olomi au bara " +
                        "hintoto. Ara hadua rangami au mampotambia towawinena umana. Mogalori " +
                        "i olonda tauna au bara moisa Pue Ala, barahe ara au mobabehi nodo.",
                    "\\v 2",
                    "\\vt Agayana ikamu, nauri ara au mobabehi nodo i olomi, mampemahile " +
                        "manikau! Katouana, hangangaa masusa lalumi, hai tauna iti hangangaa " +
                        "nipopeloho hangko i olomi.",
                    "\\v 3-4",
                    "\\vt Nauri karaona hangko irikamu, agayana lalungku arato hihimbela " +
                        "hai ikamu, hai ara mpuu kuasangku mampohawaakau. Mewali, hai kuasa " +
                        "au naweina Pue Yesu Kerisitu, kupakanotomi: tauna au kadake babehiana " +
                        "iti hangangaa rahuku. Ane mogulukau, niimbana hihimbela hai ikamu, " +
                        "hai kuperapi i lalu kuasana Yesu Amputa",
                    "\\v 5",
                    "\\vt bona nihuhu pea tauna iti i kuasana Datu Tokadake. Lempona, " +
                        "nitudumi meloho hangko i olomi, hai inee nipaliu maminggu sambela hai " +
                        "ikamu. Hangangaa nibabehi nodo, bona watana tauna iti molambi " +
                        "pehuku agayana inaona molambi katuwo maroa i kahawena hule Pue Yesu."
                };
            #endregion
            #region Data: VerseBridges - contains various verse bridges
            // VerseBridges - contains various verse bridges
            string[] vIn_VerseBridges =
                {
                    "\\v 1-2 Ara kuhadi.",
                    "\\v 3-5a Agayana ikamu.",
                    "\\v 5b - 7 Nauri karaona.",
                    "\\v 8bona nihuhu.",
                    "\\v 9-11abona nihuhu .",
                    "\\v 9-11a bona nihuhu.",
                    "\\v 9-11 abona nihuhu."
                };
            string[] vExpected_VerseBridges =
                {
                    "\\v 1-2",
                    "\\vt Ara kuhadi.",
                    "\\v 3-5a",
                    "\\vt Agayana ikamu.",
                    "\\v 5b-7",
                    "\\vt Nauri karaona.",
                    "\\v 8",
                    "\\vt bona nihuhu.",
                    "\\v 9-11",
                    "\\vt abona nihuhu .",
                    "\\v 9-11a",
                    "\\vt bona nihuhu.",
                    "\\v 9-11",
                    "\\vt abona nihuhu."
                 };
            #endregion
            #region Data: OriginalText - what I wrote in 2004
            // OriginalText - The test I wrote when I originally programmed this
            // functionality circa 2004
            string[] vIn_OriginalText = 
			{
				"\\v 3",
				"\\v (5a - 7b This is a test.)",
				"\\v 32b Howdy",
				"\\p"
			};
            string[] vExpected_OriginalText =
			{
				"\\v 3",
				"\\v 5a-7b",
				"\\vt (This is a test.)",
				"\\v 32b",
				"\\vt Howdy",
				"\\p"
			};
            #endregion

            // Set up the data
            InitializeStringSet(5);
            AddStringSet(0, vIn_Koya,         vExpected_Koya);
            AddStringSet(1, vIn_Bwazza,       vExpected_Bwazza);
            AddStringSet(2, vIn_Napu,         vExpected_Napu);
            AddStringSet(3, vIn_VerseBridges, vExpected_VerseBridges);
            AddStringSet(4, vIn_OriginalText, vExpected_OriginalText);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB._NormalizeVerseText();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: EnsureRecordsBeforeAllSections
        [Test] public void EnsureRecordsBeforeAllSections()
        {
            #region Data: Bwazza - several sections, verses prior to first \s
            // Katy: Africa, Nigeria, Bwazza, verses from the beginning of Luke.
            string[] vIn_Bwazza = 
			{
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            string[] vExpected_Bwazza =
			{
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            #endregion
            #region Data: Amarasi - example of Toolbox
            // Chuck: Indonesia, Amarasi, Luke beginning
            string[] vIn_Amarasi = 
			{
                "\\_sh v3.0  96  SHW-Scripture",
                "\\_DateStampHasFourDigitYear",
                "\\rcrd mrk",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_Bwazza,  vExpected_Bwazza);
            AddStringSet(1, vIn_Amarasi, vIn_Amarasi);      // With Amarasi, I expect In == Out.

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB._EnsureRecordsBeforeAllSections();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: EnsureInitialTitleRecord
        [Test] public void EnsureInitialTitleRecord()
        {
            #region Data: Bwazza - several sections, verses prior to first \s
            // Katy: Africa, Nigeria, Bwazza, verses from the beginning of Luke.
            string[] vIn_Bwazza = 
			{
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            string[] vExpected_Bwazza =
			{
                "\\rcrd",
                "\\id LUK",
                "\\h Luk",
                "\\mt Mǝlikakar Mi Luk",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal.",
                "\\rcrd 2",
                "\\s A Nasala Ɓǝl Yesu",
                "\\p",
                "\\v 26 Ma Alisabatu pana zongwo tongno war mwashat nǝ muna bum, Ɓakuli tasǝ " +
                    "mǝturanjar mali Jibrailu, a muna la a ɓa Galili ma akǝ tuni ama Nazareth,",
                "\\v 27 aki momai sasar ma shirǝra burana wa, mansar mi Yusufu, a tau mi Dauda. " +
                    "Lulu sasar mo ama Maryamu.",
                "\\v 28 Mǝ-turanjar wara ki yika nayi ama, \"Wopuro wai ma a twalo kpum nǝ " +
                    "baɓwarashau Ɓakuli niya to.”",
                "\\v 29 O ma Maryamu o wopuro ma, dumi kpa, wara yi ɗengshau, ɗem yi ndali " +
                    "amur jame ta mo yi wopurwo.",
                "\\v 30 Mǝ-turanjar nayi ama, \"Ɓǝ wo ɓǝka payo wa, Maryamu, a kumna ine aki Ɓakuli.",
                "\\v 31 Wa kum muna a bumo wara wa ɓǝl muna ɓwabura wara wa shiyi lulu ama Yesu.",
                "\\v 32 Ya pǝl ɓwamǝ gulai wara ana tuni ama muna mi Ɓakuli mǝ gulai. Mǝtelasǝm " +
                    "Ɓakuli na payi hama mi teri Dauda,",
                "\\v 33 wara ya pa murum a mur ɓala mi Yakubu wara limurum mali ka panǝ masǝltai wa.\"",
                "\\rcrd 3",
                "\\s Maryamu Wara Ki Alisabatu",
                "\\p",
                "\\v 39 A nzum a muna nongwo shan, Maryamu ewa a mur tal mya amǝ Judea nǝ " +
                     "halka a kushi,",
                "\\v 40 nama yika ina ɓa ɓala mi Zakariya yi payi Alisabatu wopuro.",
                "\\v 41 Ma Alisabatu o wopuro mi Maryamu, muna ma bumi dǝwrǝ ri ɓǝ baɓwarshau, " +
                    "wara ɓabumi lumsǝ nǝ Lilim mi Ɓakuli.",
				"\\p"
			};
            #endregion
            #region Data: Amarasi - example of Toolbox
            // Chuck: Indonesia, Amarasi, Luke beginning
            string[] vIn_Amarasi = 
			{
                "\\_sh v3.0  96  SHW-Scripture",
                "\\_DateStampHasFourDigitYear",
                "\\rcrd mrk",
                "\\h Nai' Markus",
                "\\st Rais Reko Usif Yesus",
                "\\btst Good News/Story/Matter of the Lord Yesus",
                "\\st natuin",
                "\\btst following/according to",
                "\\mt nai' Markus in tuis",
                "\\btmt Markus' writing",
                "\\id MRK Amarasi, NTT, Indonesia; Draft by Obed Rasi, Frid Ruku, Om " +
                    "Nikanor; Second draft getting it closer to the KM source text. " +
                    "revised by Frid thru ch.6; 23 October",
                "\\cy Copyright © 2004-2006 UBB-GMIT",
                "\\nt Special characters: schwa a2 = Á (Code 0193); á (0225); close " +
                    "e2 = é (130); É (0201); close o2 = ó (0243); Ó (0211); Use Amarasi.kmn " +
                    "(keyman keyboard for Amarasi)",
                "\\nt They refer to the people and language as Nai' Rasi, and the language " +
                    "only as Uab Rasi, A'at Rasi.",
                "\\rcrd mrk1.1-1.8",
                "\\p",
                "\\c 1",
                "\\v 1",
                "\\vt Ia Rais Reko antoom Uisneno In An-mone.|fn",
                "\\btvt This is the *Good Story/News about *God's Son.",
                "\\ft 1:1: Tuis-tuis Sur Akninu' uab Yunani ahun-hunut ka ntui niit " +
                    "fa <Uisneno In An-mone>. Mes tuis-tuis Sur Akninu' uab Yunani " +
                    "fauk neno amunit anteek uab <Uisneno In An-mone>.",
                "\\btft 1:1: Various very old *Scripture (Holy Letter) writings in the " +
                    "Yunani language do not have written 'God's Son'. But various *Scripture " +
                    "writings in the Yunani language found later on do have the words " +
                    "'God's Son'.",
                "\\vt In kan ne Nai' Yesus Kristus, re' Uisneno nruur ma nreek naan " +
                    "Je na'ko neno un-unu'. Rais Reko naan, na'ubon on ia:",
                "\\btvt His name is Yesus Kristus, whom God designated and etablished/appointed " +
                    "(parallelism) Him from long ago. That Good Story begins like this:",
                "\\nt nruur = nreek = menetapkan, menunjuk (parallel); na'ubon = base " +
                    "of tree (uuf) botanical metaphor for beginning of a story, cause " +
                    "of a perkara; rais ~ rasi = cerita dari satu peristiwa;; 1: matu'i " +
                    "= tertulis ka---fa = tidak ntuje = matu'i (orang menulisnya)",
                "\\s Nai' Yohanis Asranit anfei ranan neu Usif Yesus",
                "\\bts Yohanis the *Baptiser opens the woy/path for the Lord Yesus",
                "\\r (Mateos 3:1-12; Lukas 3:1-18; Yohanis 1:19-28)",
                "\\p",
                "\\v 2",
                "\\vt Nahuun na'ko Nai' Yesus nait In mepu, Uisneno ansonu' atoin es " +
                    "neem nahuun. Atoni' re' naan, in kan ne nai' Yohanis. Nai' Yohanis " +
                    "re' ia, ro he neem nahuun, he nfei ranan neu Nai' Yesus. Mes nahuun " +
                    "na'ko nai' Yohanis neem, Uisneno ansonu' nahunun mafefa' es, in " +
                    "kan ne nai' Yesaya. Na'ko afi ahunut, nai' Yesaya antui narair, " +
                    "nak on ia:",
                "\\rcrd mrk1.9-1.11",
                "\\s Nai' Yohanis nasrain Nai' Yesus",
                "\\bts Yohanis baptises Yesus",
                "\\r (Mateos 3:13-17; Lukas 3:21-22)",
                "\\p",
                "\\v 9",
                "\\vt Oke te, Nai' Yesus neem na'ko kuan Nasaret et pah Galilea, ma " +
                    "nateef nok nai' Yohanis. Ma nai' Yohanis nasrain Je anbi noe Yarden.|fn",
                "\\btvt After that, Yesus came from the village of Nasaret in the region/land " +
                    "of Galilea, and met with Yohanis. And Yohanis baptised Him at/in " +
                    "the river Yarden.",
                "\\ft 1:9: Uab Yunani nak <nai' Yohanis ||iebaptisthei||r Nai' Yesus>, " +
                    "re' neik hit uab nak <nai' Yohanis narema' ma napoitan Nai' Yesus>, " +
                    "namnees nok bifee es narema' abas, ai' nbak'uur abas.",
                "\\btft 1:9: The Yunani language says 'Yohanis ||iebaptisthei||r Yesus', " +
                    "which in our-inc language means 'Yohanis dipped.in.liquid and " +
                    "took.out Yesus', just like a woman dips thread to dye the thread.",
                "\\v 10",
                "\\vt Ma reka' Nai' Yesus anpoi na'ko oe, nok askeken, neno-tunan natfe'i. " +
                    "Oke te, Smana Kninu' na'ko Uisneno ansaun neem neu Ne, mamnita " +
                    "on re' kor-kefi mese'.",
                "\\btvt When Yesus came out from the water, suddenly, the sky was opened. " +
                    "Then, the *Holy Spirit from God descended coming to Him, appearing " +
                    "like a dove."
			};
            #endregion
            #region Data: Bwazza2 - This time missing any title information
            // Katy: Africa, Nigeria, Bwazza, verses from the beginning of Luke.
            string[] vIn_Bwazza2 = 
			{
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal."
			};
            string[] vExpected_Bwazza2 =
			{
                "\\rcrd",
                "\\mt",
                "\\rcrd 1",
                "\\c 1",
                "\\p",
                "\\v 1 Ɓwapina pas ma pa ama ana na njanjina mi gǝma Ɓakuli pa a ɓalǝsǝm.",
                "\\v 2 A nyolu gǝma a ɓwama kam a bani a titai nasǝm wara a shin gǝma pa.",
                "\\v 3 Nama pa sǝn shingǝ ɓǝ mana bǝp pepe, nama ngana mɓun bumom ama man " +
                    "nyoli wara man nawo gǝma pa, kla ma yi pani zul wai ɓwamǝgulai Tawafilis.",
                "\\v 4 Mpa ma yi gri asalama ɓu shirǝ mǝsǝshau amura a gǝma a o ma.",
                "\\s A Nasala Ɓǝlban Mi Yohanna",
                "\\p",
                "\\v 5 A nzami Hiridus, Murum Yahudiya, momai mǝ-turonda mi Ɓakuli kam ma luli " +
                    "ama Zakariya ma puru tau turonda mi Ɓakuli mi Abija\f + \fr 1:5 \ft shin " +
                    "1 Chron./Laba. 24:7-18\ft*\f*, mami Alisabatu, mǝ kau mi Haruna na.",
                "\\v 6 Kǝm nǝ mburapiya amǝ ɓangbi Ɓakuli na, akǝ kpatai wara akǝ lumsa a " +
                    "kunashau mi Ɓakuli, akǝ kpawa.",
                "\\v 7 Aka na a muna wa, asalama Alisabatu nkwami na, sǝ ɗǝm a shaura nǝ mburapiya.",
                "\\p",
                "\\v 8 A momai pwari, Zakariya nama nǝ turo a nda mi Ɓakuli, a mǝsǝrǝ dum Ɓakuli, " +
                    "kla pami kunshau maliya.",
                "\\v 9 A tari, ɓi ina ɓanda mi Ɓakuli ɓika losu yolo peri.",
                "\\v 10 Ma pwari mi losu yolo peri mo pana, amǝ jim Ɓakuli tannga a nza, akǝ " +
                    "pwan nzal."
			};
            #endregion

            // Set up the data
            InitializeStringSet(3);
            AddStringSet(0, vIn_Bwazza,  vExpected_Bwazza);
            AddStringSet(1, vIn_Amarasi, vIn_Amarasi);      // With Amarasi, I expect In == Out.
            AddStringSet(2, vIn_Bwazza2, vExpected_Bwazza2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB._EnsureRecordsBeforeAllSections();
                DB._EnsureInitialTitleRecord();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_SeeAlso
        [Test] public void ConvertParatext_SeeAlso()
        {
            // Create a database for the helper methods test
            ScriptureDB DBH = new ScriptureDB();
            ScriptureDB.USFM_SeeAlso DBConv = new ScriptureDB.USFM_SeeAlso(DBH);

            // Helper methods: Test IsUSFMSeeAlsoBegin
            int iLen;
            DBConv.IsUSFMSeeAlsoBegin(
                "\\rc 17.50: \\rt 2 Sam 21.19. \\rt* \\rc*", 0, out iLen);
            Assert.AreEqual(4, iLen);
            DBConv.IsUSFMSeeAlsoBegin(
                "\\x + \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            DBConv.IsUSFMSeeAlsoBegin(
                "\\x - \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            DBConv.IsUSFMSeeAlsoBegin(
                "\\x c \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            DBConv.IsUSFMSeeAlsoBegin(
                "\\x_ + \\xo 17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(10, iLen);
            DBConv.IsUSFMSeeAlsoBegin(
                "\\x_  +  \\xo  17.50: \\xt 2 Sam 21.19. \\xt* \\x*", 0, out iLen);
            Assert.AreEqual(13, iLen);  // (test that it is correctly eating white space)

            // Melper methods: Test IsUSFMSeeAlsoEnd
            DBConv.IsUSFMSeeAlsoEnd("\\rt* \\rc*", 0, out iLen);
            Assert.AreEqual(9, iLen);
            DBConv.IsUSFMSeeAlsoEnd("\\xt* \\x*", 0, out iLen);
            Assert.AreEqual(8, iLen);
            DBConv.IsUSFMSeeAlsoEnd("\\x*", 0, out iLen);
            Assert.AreEqual(3, iLen);

            // Helper methods: Test ExtractSeeAlso
            Assert.AreEqual("17.50: 2 Sam 21.19.",
                DBConv.ExtractSeeAlso("17.50: \\rt 2 Sam 21.19. "));
            Assert.AreEqual("17.50: 2 Sam 21.19.",
                DBConv.ExtractSeeAlso("17.50: \\xt 2 Sam 21.19. "));
            Assert.AreEqual("17.50: 2 Sam 21.19.",
                DBConv.ExtractSeeAlso("\\bd 17.50: \\*bd 2 Sam 21.19. "));

            // Data with actual verses that contain cross references
            #region Method: Data: Set 1 - From Hausa
            string[] vvIn_1 =
                {
                    "\\v 50 \\rc 17.50: \\rt 2 Sam 21.19. \\rt* \\rc* Ta haka, da majajjawa da " +
                        "dutse kawai Dawuda ya ci nasara a kan mutumin Filistin nan. Ya jefe shi " +
                        "ya kashe ba tare da takobi a hannunsa ba."
                };
            string[] vvExpected_1 =
                {
                    "\\v 50",
                    "\\vt",
                    "\\cf 17.50: 2 Sam 21.19.",
                    "\\vt Ta haka, da majajjawa da dutse kawai Dawuda ya ci nasara a kan mutumin " +
                        "Filistin nan. Ya jefe shi ya kashe ba tare da takobi a hannunsa ba."
                };
            #endregion
            #region Method: Data: Set 2 - From Solomans
            string[] vvIn_2 =
                {
                    "\\v 15 They stayed there until Herod died. And this thing made true what " +
                        "God said to the profet who wrote like this, “I call my child out " +
                        "from Ejipt.”\\x + \\xo 2:15 \\xt Hosea 11:1\\xt*\\x*"
                };
            string[] vvExpected_2 =
                {
                    "\\v 15",
                    "\\vt They stayed there until Herod died. And this thing made true what " +
                        "God said to the profet who wrote like this, “I call my child out " +
                        "from Ejipt.”",
                    "\\cf 2:15 Hosea 11:1"
                };
            #endregion
            #region Method: Data: Set 3 - From Solomans
            string[] vvIn_3 =
                {
                    "\\v 17 And don't have sex with the daughter or granddaughter of any woman " +
                        "that you have earlier had sex with. You may be having sex with a relative, " +
                        "and that would make you unclean.\\x - \\xo 18.17: \\xt Lv 20.14; Dt 27.23.\\x*"
                };
            string[] vvExpected_3 =
                {
                    "\\v 17",
                    "\\vt And don't have sex with the daughter or granddaughter of any woman " +
                        "that you have earlier had sex with. You may be having sex with a relative, " +
                        "and that would make you unclean.",
                    "\\cf 18.17: Lv 20.14; Dt 27.23."
                };
            #endregion
            #region Method: Data: Set 4 - From CEV
            string[] vvIn_4 =
                {
                    "\\v 8 “God promised Abraham and he told him, if his people circumcised/cut-show, " +
                        "they agree to his rock promise/covenant.\\x + \\xo 7:8 \\xt Jenesis 17:10-14\\xt*\\x* " +
                        "That is why Abraham circumcised Aesak, eight days after he was born. In the " +
                        "same way also, Aesak circumcised his son Jakob, and Jakob circumcised his twelve " +
                        "sons who they are our(in) ancestors."
                };
            string[] vvExpected_4 =
                {
                    "\\v 8",
                    "\\vt “God promised Abraham and he told him, if his people circumcised/cut-show, " +
                        "they agree to his rock promise/covenant.",
                    "\\cf 7:8 Jenesis 17:10-14",
                    "\\vt That is why Abraham circumcised Aesak, eight days after he was born. In the " +
                        "same way also, Aesak circumcised his son Jakob, and Jakob circumcised his twelve " +
                        "sons who they are our(in) ancestors."
                };
            #endregion
            #region Method: Data: Set 5
            string[] vvIn_5 =
                {
                    "\\v 9 The promise that God made with Abraham is this, “At the appointed time, " +
                        "I will come back to you, and your wife Sara will deliver one male " +
                        "child.”\\x + \\xo 9.9 \\xt Genesis 18:10\\xt*\\x* And that child is Aesak"
                };
            string[] vvExpected_5 =
                {
                    "\\v 9",
                    "\\vt The promise that God made with Abraham is this, “At the appointed time, " +
                        "I will come back to you, and your wife Sara will deliver one male " +
                        "child.”",
                    "\\cf 9.9 Genesis 18:10",
                    "\\vt And that child is Aesak"
                };
            #endregion

            // Set up the data
            InitializeStringSet(5);
            AddStringSet(0, vvIn_1, vvExpected_1);
            AddStringSet(1, vvIn_2, vvExpected_2);
            AddStringSet(2, vvIn_3, vvExpected_3);
            AddStringSet(3, vvIn_4, vvExpected_4);
            AddStringSet(4, vvIn_5, vvExpected_5);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB._NormalizeVerseText();
                (new ScriptureDB.USFM_SeeAlso(DB)).Import();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: MoveParagraphText
        [Test] public void MoveParagraphText()
        {
            #region Data
            string[] vIn = 
                {
                    "\\q2 When she cries, no one can comfort her, because her children " +
                       "are all dead.”"
                };

            string[] vExpected =
                {
                    "\\q2",
                    "\\vt When she cries, no one can comfort her, because her children " +
                       "are all dead.”"
                };
            #endregion

            // Set up the data
            InitializeStringSet(1);
            AddStringSet(0, vIn, vExpected);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB._MoveParagraphText();
                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_Footnotes
        [Test] public void ConvertParatext_Footnotes()
        {
            // Data
            #region DATA Set 1
            string[] vIn_1 =
                {
                    "\\v 31",
                    "\\vt Vilevile kwa Siku ya hukumu, Malkia toka Kusini\\f j \\fr 11.31 \\fk Malkia " +
                        "toka Kusini: \\ft au wa Inchi ya Seba.\\f* atasimama na watu wa kizazi hiki na " +
                        "kuwashitaki, kwa sababu yeye alitoka mbali sana kuja kusikiliza maneno ya hekima " +
                        "ya mufalme Solomono. Hapa kuna yule anayekuwa mukubwa kupita Solomono!"
                };
            string[] vExpected_1 =
                {
                    "\\v 31",
                    "\\vt Vilevile kwa Siku ya hukumu, Malkia toka Kusini|fn atasimama na watu wa kizazi hiki na " +
                        "kuwashitaki, kwa sababu yeye alitoka mbali sana kuja kusikiliza maneno ya hekima " +
                        "ya mufalme Solomono. Hapa kuna yule anayekuwa mukubwa kupita Solomono!",
                    "\\ft 11.31 Malkia toka Kusini: au wa Inchi ya Seba."
                };
            #endregion
            #region DATA Set 2
            string[] vIn_2 =
                {
                    "\\v 4",
                    "\\vt Yesu anamúshuvya: «Si’biyandisirwi kwokuno: ‹Bitali byo’mundu ali mu’lya naaho, " +
                        "byo’bitumiri ali mu’ba mugumaana.›»\\f d \\fr 4:4 \\ft Buk 8:3 \\f*"
                };
            string[] vExpected_2 =
                {
                    "\\v 4",
                    "\\vt Yesu anamúshuvya: «Si’biyandisirwi kwokuno: ‹Bitali byo’mundu ali mu’lya naaho, " +
                        "byo’bitumiri ali mu’ba mugumaana.›»|fn",
                    "\\ft 4:4 Buk 8:3"
                };
            #endregion
            #region DATA Set 3 - five different ones/variations crammed into one verse!
            string[] vIn_3 =
                {
                    "\\v 51",
                    "\\vt Yibyo bigatondekera ku’muko gwa’Abeeri, halinde ukuhisa ku’muko " +
                        "gwa’Zakariya,\\f t \\fr 11:51 \\ft Mu’Mandiko Meeru ga’Bayahudi, úkayitwa " +
                        "ubwa mbere, âli riiri Abeeri; no’kayitwa ubuzinda, âli riiri muleevi Zakariya. " +
                        "Yibyo, wangabisoma mu’kitaabo kye’Ndondeko 4; na’mu’Bye’Byanya bya’kabiri " +
                        "24:20-22. \\f*\\f a \\fr 2.22 \\fk kutakaswa: \\ft Angalia Law 12.1-8.\\f* ulya " +
                        "úkayitirwa ha’kati ke’nyumba ya’Rurema\\f c \\fr 3:4-6 \\ft Isa 40:3-5 \\f* " +
                        "na’katanda\\f + \\fr 18:6 \\fr* ipima mngana i:nye n litili 11, " +
                        "ym 12. \\f* ko’kutangira kwa’matuulo. Ee! Namùbwira kwa’bandu ba’kino kibusi " +
                        "bo’bagabuuzibwa hi’gulu lya’yibyo byoshi.\\f + \\fr 10:12 \\fk Sodom \\ft shin " +
                        "Gen./Fara. 19:24-28\\ft*\\f*"
                };
            string[] vExpected_3 =
                {
                    "\\v 51",
                    "\\vt Yibyo bigatondekera ku’muko gwa’Abeeri, halinde ukuhisa ku’muko " +
                        "gwa’Zakariya,|fn|fn ulya úkayitirwa ha’kati ke’nyumba ya’Rurema|fn " +
                        "na’katanda|fn ko’kutangira kwa’matuulo. Ee! Namùbwira kwa’bandu ba’kino kibusi " +
                        "bo’bagabuuzibwa hi’gulu lya’yibyo byoshi.|fn",
                    "\\ft 11:51 Mu’Mandiko Meeru ga’Bayahudi, úkayitwa ubwa mbere, âli riiri Abeeri; " +
                        "no’kayitwa ubuzinda, âli riiri muleevi Zakariya. Yibyo, wangabisoma mu’kitaabo " +
                        "kye’Ndondeko 4; na’mu’Bye’Byanya bya’kabiri 24:20-22.",
                    "\\ft 2.22 kutakaswa: Angalia Law 12.1-8.",
                    "\\ft 3:4-6 Isa 40:3-5",
                    "\\ft 18:6 ipima mngana i:nye n litili 11, ym 12.",
                    "\\ft 10:12 Sodom shin Gen./Fara. 19:24-28"
                };
            #endregion
            #region DATA Set 4 - a footnote as part of a "\s" field
            string[] vIn_4 =
                {
                    "\\s Yuani akʉbɨakɨa Ɨsaʉ yɛɛ Ido\\f + \\fr 3:6 \\fr* Ɨsaya 40:3-5\\f*",
                    "\\r (Matayɔ 3:1-10; Malɨkɔ 1:3-8)",
                    "\\p",
                    "\\v 1",
                    "\\vt Tibhelio, ngama ɔɔ 'ʉja ʉ baLɔma, ambʉndaniso mugo kumi nʉ bɔkɔ akʉ bʉngama."
                };
            string[] vExpected_4 =
                {
                    "\\s Yuani akʉbɨakɨa Ɨsaʉ yɛɛ Ido|fn",
                    "\\ft 3:6 Ɨsaya 40:3-5",
                    "\\r (Matayɔ 3:1-10; Malɨkɔ 1:3-8)",
                    "\\p",
                    "\\v 1",
                    "\\vt Tibhelio, ngama ɔɔ 'ʉja ʉ baLɔma, ambʉndaniso mugo kumi nʉ bɔkɔ akʉ bʉngama."
                };
            #endregion

            // Set up the data
            InitializeStringSet(4);
            AddStringSet(0, vIn_1, vExpected_1);
            AddStringSet(1, vIn_2, vExpected_2);
            AddStringSet(2, vIn_3, vExpected_3);
            AddStringSet(3, vIn_4, vExpected_4);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB._ImportParatextFootnotes();
                CompareTransforms(DB, m_vvExpected[i]);
            }

        }
        #endregion
        #region TEST: JoinParatextEmbeddedFields
        [Test] public void JoinParatextEmbeddedFields()
        {
            string[] vIn =
                {
                    "\\v 3",
                    "\\vt Da yaron ya cika kwana takwas, sai suka zo domin su yi masa kaciya.",
                    "\\f c \\fr 1.59",
                    "\\fk kaciya:\\fk* Dubi ma'anar wa?ansu kalmomi.",
                    "\\f* Da ma sun so ne su sa masa sunan babansa,"
                };
            string[] vExpected =
                {
                    "\\v 3",
                    "\\vt Da yaron ya cika kwana takwas, sai suka zo domin su yi masa kaciya. " +
                        "\\f c \\fr 1.59 \\fk kaciya:\\fk* Dubi ma'anar wa?ansu kalmomi. " +
                        "\\f* Da ma sun so ne su sa masa sunan babansa,"
                };

            // Set up the data
            InitializeStringSet(1);
            AddStringSet(0, vIn, vExpected);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.JoinParatextEmbeddedFields(ScriptureDB.ParatextFootnoteMkrsToJoin);

                    string[] vs = DB.ExtractData();
                    foreach (string s in vs)
                        Console.WriteLine("<" + s + ">");

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_CompleteDataSets
        [Test] public void ConvertParatext_CompleteDataSets()
        {
            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, m_vIn_1, m_vExpectedToolbox_1);
            AddStringSet(1, m_vIn_2, m_vExpectedToolbox_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.TransformIn();

                /***
                if (i > 0)
                {
                    string[] vs = DB.ExtractData();
                    foreach (string s in vs)
                        Console.WriteLine("<" + s + ">");
                }
                ***/

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatextVerseBridges
        [Test] public void ConvertParatext_VerseBridges()
        {
            // Create an empty database for the test
            ScriptureDB DB = new ScriptureDB();

            Assert.AreEqual("2-4 Text",   DB._ConvertParatextVerseBridge("2,3,4 Text"));
            Assert.AreEqual("3 Text",     DB._ConvertParatextVerseBridge("3 Text"));
            Assert.AreEqual("12-13 Text", DB._ConvertParatextVerseBridge("12,13 Text"));
            Assert.AreEqual("9-13 Text",  DB._ConvertParatextVerseBridge("9,13 Text"));
            Assert.AreEqual("4a-7b Text", DB._ConvertParatextVerseBridge("4a,5,6,7b Text"));
        }
        #endregion
        #region TEST: ConvertParatext_ImportFigures
        [Test] public void ConvertParatext_ImportFigures()
        {
            #region DATA Set 1
            string[] vIn_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. \\fig GW-64 Umuganda ahulukira " +
                        "ku' bangere (2:9)\\fig*"
                };
            string[] vExpected_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. ",
                    "\\cat Unknown",
                    "\\cap GW-64 Umuganda ahulukira ku' bangere (2:9)"
                };
            #endregion
            #region DATA Set 2
            string[] vIn_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; \\fig |GW64.jpg|span|||Umuganda ahulukira " +
                        "ku' bangere (2:9)\\fig*wakapita emo utetemeko."
                };
            string[] vExpected_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; ",
                    "\\cat GW64.jpg",
                    "\\ref span",
                    "\\cap Umuganda ahulukira ku' bangere (2:9)",
                    "\\vt wakapita emo utetemeko."
                };
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_1, vExpected_1);
            AddStringSet(1, vIn_2, vExpected_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                (new ScriptureDB.USFM_Pictures(DB)).Import();

//                WriteActualDataToConsole(DB, "Import Figures - " + i.ToString());

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
        #region TEST: ConvertParatext_ExportFigures
        [Test] public void ConvertParatext_ExportFigures()
        {
            #region DATA Set 1
            string[] vIn_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. ",
                    "\\cat Unknown",
                    "\\cap GW-64 Umuganda ahulukira ku' bangere (2:9)"
                };
            string[] vExpected_1 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; wakapita emo utetemeko. ",
                    "\\fig |Unknown||||GW-64 Umuganda ahulukira ku' bangere (2:9)|\\fig*"
                };
            #endregion
            #region DATA Set 2
            string[] vIn_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; ",
                    "\\cat GW64.jpg",
                    "\\ref span",
                    "\\cap Umuganda ahulukira ku' bangere (2:9)",
                    "\\vt wakapita emo utetemeko."
                };
            string[] vExpected_2 =
                {
                    "\\v 9",
                    "\\vt Mara_moja wakatokewa eko na malaika mmoja wa Bwana. Na utukufu wa Bwana " +
                        "ukawaangazia pande zote; ",
                    "\\fig |GW64.jpg|span|||Umuganda ahulukira ku' bangere (2:9)|\\fig*",
                    "\\vt wakapita emo utetemeko."
                };
            #endregion

            // Set up the data
            InitializeStringSet(2);
            AddStringSet(0, vIn_1, vExpected_1);
            AddStringSet(1, vIn_2, vExpected_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                (new ScriptureDB.USFM_Pictures(DB)).Export();

//                WriteActualDataToConsole(DB, "Export Figures - " + i.ToString());

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion

        // Paratext Export 
        #region TEST: ParatextRoundTrip
        [Test] public void ParatextRoundTrip()
        {
            // Set up the data (both the Input and the Expected are the same strings)
            InitializeStringSet(2);
            AddStringSet(0, m_vIn_1, m_vExpectedParatext_1);
            AddStringSet(1, m_vIn_2, m_vExpectedParatext_2);

            // Do the test
            for (int i = 0; i < m_vvIn.Length; i++)
            {
                // Read in and transform to our normal input
                ScriptureDB DB = new ScriptureDB();
                DB.Initialize(m_vvIn[i]);
                DB.TransformIn();

                // Now do the reverse of the transforms back to Paratext
                DB.Format = ScriptureDB.Formats.kParatext;
                DB.TransformOut();

                // Write to console for debugging purposes
//                WriteActualDataToConsole(DB, "ParatextRoundTrip - " + i.ToString());

                CompareTransforms(DB, m_vvExpected[i]);
            }
        }
        #endregion
    }
    #endregion

}
