/**********************************************************************************************
 * Project: JWdb
 * File:    StdFmtTransform.cs
 * Author:  John Wimbish
 * Created: 25 Nov 2004
 * Purpose: Reads/writes a standard format database, handles SF irregularities
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using JWTools;
using OurWordData;
using OurWordData.Styles;

#endregion

namespace OurWordData.DataModel
{
    #region CLASS: XForm - the superclass for the various transformations
    public class XForm
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: ScriptureDB DBS
        protected ScriptureDB DBS
        {
            get
            {
                Debug.Assert(null != m_DB);
                return m_DB;
            }
        }
        ScriptureDB m_DB;
        #endregion
        #region Attr{g}: string Name - the name of this transform
        public string Name
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sName));
                return m_sName;
            }
        }
        string m_sName = "";
        #endregion
        #region bool IsIn - T if it has been imported through this OnImport already
        public bool IsIn
        {
            get
            {
                return m_bIsIn;
            }
        }
        bool m_bIsIn = false;
        #endregion

        // Test Dependancies -----------------------------------------------------------------
        #region Attr{g}: string[] ImportDependancies
        string[] ImportDependancies
        {
            get
            {
                Debug.Assert(null != m_vImportDependancies);
                return m_vImportDependancies;
            }
        }
        string[] m_vImportDependancies;
        #endregion
        #region Attr{g}: string[] ExportDependancies
        string[] ExportDependancies
        {
            get
            {
                Debug.Assert(null != m_vExportDependancies);
                return m_vExportDependancies;
            }
        }
        string[] m_vExportDependancies;
        #endregion
        #region Method: bool CheckDependancies(vDependancies)
        bool CheckDependancies(string[] vDependancies, bool bForward)
        {

            foreach (string sDependancy in vDependancies)
            {
                int i = (bForward) ? 0 : DBS.XForms.Count - 1;

                do
                {
                    XForm form = DBS.XForms[i];

                    // Dependancy found; we've already processed it (as we should)
                    if (form.Name == sDependancy)
                        goto NextDependancy;

                    // We didn't find the dependancy. Bug.
                    if (form == this)
                    {
                        Debug.Assert(false, "Didn't find Dependancy \"" +
                            sDependancy +
                            "\" prior to \"" +
                            Name + "\"");
                        return false;
                    }


                    i = (bForward) ? i + 1 : i - 1;
                } while (i >= 0 && i < DBS.XForms.Count);

            NextDependancy:
                continue;
            }

            return true;
        }
        #endregion

        // Shorthand -------------------------------------------------------------------------
        #region VAttr{g}: bool IsParatextFormat
        protected bool IsParatextFormat
        {
            get
            {
                return DBS.IsParatextFormat;
            }
        }
        #endregion
        #region VAttr{g}: bool IsToolboxFormat
        protected bool IsToolboxFormat
        {
            get
            {
                return DBS.IsToolboxFormat;
            }
        }
        #endregion
        #region VAttr{g}: bool IsGoBibleFormat
        protected bool IsGoBibleFormat
        {
            get
            {
                return DBS.IsGoBibleFormat;
            }
        }
        #endregion
        #region VAttr{g}: public DSFMapping Map - Necessary for NUnit access
        // This object allows us to map from the read.Marker to the way to handle each
        // field, e.g., which one is a Section Title, which one is a back translation,
        // etc.
        public DSFMapping Map
        {
            get
            {
                // For NUnit debugging where no project is defined; will not execute otherwise.
                if (DB.Project == null || DB.TeamSettings == null)
                    return new DSFMapping();

                return DB.TeamSettings.SFMapping;
            }
        }
        #endregion

        // Subclasses should override for desired behavior -----------------------------------
        #region VirtMethod: void OnImport()
        public virtual void OnImport()
        {
            if (!CheckDependancies(ImportDependancies, true))
                return;

            m_bIsIn = true;
        }
        #endregion
        #region VirtMethod: void OnExport()
        public virtual void OnExport()
        {
            if (!CheckDependancies(ExportDependancies, false))
                return;

            m_bIsIn = false;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(ScriptureDB)
        public XForm(ScriptureDB _DB, 
            string _sName, 
            string[] vImportDependancies,
            string[] vExportDependancies)
        {
            m_DB = _DB;
            m_sName = _sName;

            m_vImportDependancies = vImportDependancies;
            if (null == m_vImportDependancies)
                m_vImportDependancies = new string[0];

            m_vExportDependancies = vExportDependancies;
            if (null == m_vExportDependancies)
                m_vExportDependancies = new string[0];
        }
        #endregion
        #region Method: void DebugDump()
        public void DebugDump(string sContext)
        {
            Console.WriteLine("---- Beg Dump ---- " + sContext + " -----");
            foreach (SfField f in DBS.Fields)
            {
                Console.WriteLine("\\" + f.Mkr + " " + f.Data);
            }
            Console.WriteLine("---- End Dump -------------------------");
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_GoBibleExport
    public class XForm_GoBibleExport : XForm
        // For GoBible, the \id line should only contain the upper-case, three-letter
        // abbreviation of the book. 
        //
        // Unfortunately, we don't know the book in this context; so all we can do
        // is truncate anything longer than three letters, and convert all to upper case.
    {
        // Scaffolding -----------------------------------------------------------------------
        public const string c_sName = "NormalizeHeader";
        #region Constructor(DB)
        public XForm_GoBibleExport(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion

        // Export ----------------------------------------------------------------------------
        #region Method: void NormalizeID()
        public void NormalizeID()
        {
            foreach (SfField field in DBS.Fields)
            {
                if (field.Mkr == "id")
                {
                    if (field.Data.Length > 3)
                        field.Data = field.Data.Substring(0, 3).ToUpper();
                }
            }
        }
        #endregion
        #region Method: void MoveSections()
        public void MoveSections()
        {
            for (int i = 0; i < DBS.Fields.Count - 1; i++)
            {
                // Are we at a section head?
                SfField fSection = DBS.Fields[i] as SfField;
                if (fSection.Mkr != Map.MkrSection && fSection.Mkr != Map.MkrSection2)
                    continue;

                // Scan down for some verse text
                SfField fVerse = null;
                for (int k = i + 1; k < DBS.Fields.Count; k++)
                {
                    fVerse = DBS.Fields[k] as SfField;
                    if (fVerse.Mkr == Map.MkrVerseText)
                        break;
                }

                // Move the section head into the verse
                if (null != fVerse)
                {
                    fVerse.Data = "\\wj " + fSection.Data + "\\wj* " + fVerse.Data;
                    DBS.RemoveAt(i);
                }
            }
        }
        #endregion
        #region Method: void RemoveFootnotes()
        public void RemoveFootnotes()
        {
            for (int i = 0; i < DBS.Count; )
            {
                SfField field = DBS.Fields[i] as SfField;
                Debug.Assert(null != field);

                if (field.Mkr == Map.MkrFootnote)
                    DBS.Fields.RemoveAt(i);
                else if (field.Mkr == Map.MkrSeeAlso)
                    DBS.Fields.RemoveAt(i);
                else
                {
                    int iFN = field.Data.IndexOf("|fn");
                    if (-1 == iFN)
                    {
                        i++;
                        continue;
                    }

                    field.Data = field.Data.Remove(iFN, 3);                   
                }
            }
        }
        #endregion
        #region Method: void ConvertSmartQuotesToOrdinary()
        public void ConvertSmartQuotesToOrdinary()
        {
            foreach (SfField f in DBS.Fields)
            {
                f.Data = f.Data.Replace("<<<", "“‘");
                f.Data = f.Data.Replace(">>>", "’”");

                f.Data = f.Data.Replace("<<", "“");
                f.Data = f.Data.Replace(">>", "”");

                f.Data = f.Data.Replace("<", "‘");
                f.Data = f.Data.Replace(">", "’");
            }
        }
        #endregion
        #region Method: void RemoveCharacterFormating()
        public void RemoveCharacterFormating()
        {
            foreach (SfField f in DBS.Fields)
            {
                f.Data = f.Data.Replace("||i", "");
                f.Data = f.Data.Replace("||b", "");
                f.Data = f.Data.Replace("||r", "");
            }
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
        {
            base.OnExport();

            if (!IsGoBibleFormat)
                return;

            NormalizeID();
            RemoveFootnotes();
            MoveSections();
            ConvertSmartQuotesToOrdinary();
            RemoveCharacterFormating();
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_NormalizeMarkers
    public class XForm_NormalizeMarkers : XForm
        // We've seen various marker "synonyms" in data; this changes markers into
        // a single subset that OurWord recognizes.
    {
        // Import ----------------------------------------------------------------------------
        #region Struct Normalization
        struct Normalization
        {
            public string OddForm;
            public string NormalizedForm;

            public Normalization(string sOddForm, string sNormalizedForm)
            {
                OddForm = sOddForm;
                NormalizedForm = sNormalizedForm;
            }
        };
        #endregion
        #region Normalization[] s_Normalizations (list of, e.g., "mt1" --> "mt")
        static Normalization[] s_Normalizations = 
        {
            new Normalization( "mt1", "mt" ),    // Main Title alternate
            new Normalization( "mt2", "st" ),    // Sub Title alternate
            new Normalization( "nq", "nt" ),     // Observed in Timor data
            new Normalization( "nc", "nt" ),     // Observed in Timor data
            new Normalization( "ob", "ov" ),     // Dumb development typo, but released in 0.6.
            new Normalization( "vt2", "ov" ),    // Tomohon has \vt2 as an alternative version
            new Normalization( "tr", "ntUns" ),  // Tomohon "tanya responden" (question for UNS
            new Normalization( "chk2", "chk" ),  // Second section comment
            new Normalization( "q1", "q" ),      // USFM: the two are equal.
            new Normalization( "nb", "p" ),      // USFM: NoBreakWithPrevPara becomes simply a \p
            new Normalization( "s1", "s" ),      // USFM: Conflating these
            new Normalization( "ms1", "ms" ),    // USFM: Conflating these
            new Normalization( "ms2", "ms" ),    // USFM: Conflating these
            new Normalization( "qm", "m" ),      // USFM: NoFirstLineIndent (a.k.a. paragraph continuation)
            new Normalization( "li", "q" ),      // Conflating these, may make people mad, though
            new Normalization( "li2","q2" )      // Conflating these, may make people mad, though
        };
        #endregion
        #region Method: void CheckLineInitialMarkers(SfField)
        void CheckLineInitialMarkers(SfField field)
        {
            foreach (Normalization norm in s_Normalizations)
            {
                if (norm.OddForm == field.Mkr)
                {
                    field.Mkr = norm.NormalizedForm;
                    return;
                }
            }
        }
        #endregion
        #region Method: void CheckLineMedialMarkers(SfField)
        void CheckLineMedialMarkers(SfField field)
        {
            // Quick check for backslash, will save processing for vast majority of data
            if (field.Data.IndexOf('\\') == -1)
                return;

            // Loop through them all, since there may be multiple on a line
            foreach (Normalization norm in s_Normalizations)
            {
                string sPattern = "\\" + norm.OddForm + " ";

                int iPos = field.Data.IndexOf(sPattern);
                if (-1 == iPos)
                    continue;

                string sLeft = field.Data.Substring(0, iPos);
                string sRight = field.Data.Substring(iPos + sPattern.Length);

                field.Data = sLeft + "\\" + norm.NormalizedForm + " " + sRight;
            }
        }
        #endregion
        #region OMethod: void OnImport()
        public override void  OnImport()
        {
 	        base.OnImport();

            foreach (SfField field in DBS.Fields)
            {
                // Change out any line-initial markers
                CheckLineInitialMarkers(field);

                // Changen out any line-medial markers
                CheckLineMedialMarkers(field);
            }
        }
        #endregion

        // Export ----------------------------------------------------------------------------
        #region OMethod: void OnExport()
        public override void OnExport()
            // Paratext convention changes "st" to "mt2"
        {
            base.OnExport();
            if (IsToolboxFormat)
                return;

            foreach (SfField field in DBS.Fields)
            {
                if (field.Mkr == Map.MkrSubTitle)
                    field.Mkr = "mt2";
            }
        }
        #endregion

        public const string c_sName = "NormalizeMarkers";
        #region Constructor(ScriptureDB)
        public XForm_NormalizeMarkers(ScriptureDB DB)
            : base(DB, c_sName, 
            null,
            new string[] { XForm_RemoveIgnoredMarkers.c_sName})
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_ShoeboxSpecialMarkers
    public class XForm_ShoeboxSpecialMarkers : XForm
        // Toolbox has, e.g., "\_sh ..." and "\_DateStampHasFourDigitYear" markers,
        // which we ignore in OurWord, but must export if Toolbox format.
    {
        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            for (int i = 0; i < DBS.Count; )
            {
                SfField field = DBS.GetFieldAt(i);

                if (field.Mkr.StartsWith("_"))
                    DBS.RemoveAt(i);
                else
                    i++;
            }
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
        {
            base.OnExport();
            if (!IsToolboxFormat)
                return;

            // Get the count of record markers
            int cRecords = 0;
            foreach (SfField f in DBS.Fields)
            {
                if (f.Mkr == DBS.RecordMkr)
                    ++cRecords;
            }

            DBS.InsertAt(0, new SfField("_sh", "v3.0  " + cRecords.ToString() + "  SHW-Scripture"));
            DBS.InsertAt(1, new SfField("_DateStampHasFourDigitYear"));
        }
        #endregion

        public const string c_sName = "ShoeboxSpecialMarkers";
        #region Constructor(DB)
        public XForm_ShoeboxSpecialMarkers(ScriptureDB DB)
            : base(DB, c_sName, null, null )
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_RemoveIgnoredMarkers
    public class XForm_RemoveIgnoredMarkers : XForm
        // These are markers we've encountered in data, but that we don't need
        // (nor want) to preserve.
    {
        static string[] s_vIgnoredImportMarkers = 
        { 
            "e",    // End of file
            "bk",   // Tomohon's "Bahasa Kupang" (Kupang translation)
            "tb",   // Tomohon's "Terjemahan Baru" (Indonesian translation)
            "bis",  // Tomohon's Indonesian translation
            "bm"    // Tomohon's Bahasa Manado field
        };

        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            for (int i = 0; i < DBS.Count; )
            {
                SfField field = DBS.GetFieldAt(i);

                // Loop through our list of ignored markers; if we find it,
                // then delete that field.
                foreach (string sIgnoredMarker in s_vIgnoredImportMarkers)
                {
                    if (sIgnoredMarker == field.Mkr)
                    {
                        DBS.RemoveAt(i);
                        goto loop;
                    }
                }

                // Go to the next field (we're already there if we called RemoveAt)
                i++;

            loop:
                continue;
            }
        }
        #endregion

        #region OMethod: void OnExport()
        public override void OnExport()
            // If exporting to USFM, we have to remove markers that aren't supported
            // e.g., \rcrd
        {
            base.OnExport();
            if (IsToolboxFormat)
                return;

            for (int i = 0; i < DBS.Count; )
            {
                SfField f = DBS.GetFieldAt(i);

                if (IsParatextFormat && !Map.IsUSFMExportMarker(f.Mkr))
                    DBS.RemoveAt(i);
                else if (IsGoBibleFormat && !Map.IsGoBibleExportMarker(f.Mkr))
                    DBS.RemoveAt(i);
                else
                    i++;
            }
        }
        #endregion

        public const string c_sName = "RemoveIgnoredMarkers";
        #region Constructor(DB)
        public XForm_RemoveIgnoredMarkers(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_CombinedBTs
    public class XForm_CombinedBTs : XForm
    {
        const string c_sPrefixBT = "bt";
        const string c_sPrefixIBT = "ibt";

        #region OMethod: void OnImport()
        public override void OnImport()
            // Given
            //    \vt 
            //    \btvt
            // Moves the data of the "\btvt" field into the "\vt" field, and deletes
            // the "\btvt" field. 
            //
            // The test is to look for "bt" at the beginning of any marker, and compare the
            // remaining part of that marker with the marker before it.
        {
            base.OnImport();

            for (int i = 0; i < DBS.Count - 1; )
            {
                // Retrieve the vernacular field
                SfField fText = DBS.GetFieldAt(i);

                // Compute the corresponding markers for BT and IBT fields
                string sBT = c_sPrefixBT + fText.Mkr;
                string sIBT = c_sPrefixIBT + fText.Mkr;

                // Scan forward for the back translation field
                for (int k = i + 1; k < DBS.Count; )
                {
                    // Retrieve the potential Back Translation field
                    SfField fBT = DBS.GetFieldAt(k);

                    // Are we dealing with a BT field for the current field?
                    if (0 == sBT.CompareTo(fBT.Mkr))
                    {
                        // Place the BT into the text field
                        fText.BT += fBT.Data;

                        // Delete the BT field
                        DBS.RemoveAt(k);
                    }

                    // Are we dealing with a IBT field for the current field?
                    else if (0 == sIBT.CompareTo(fBT.Mkr))
                    {
                        // Place the iBT into the text field
                        fText.IBT = fBT.Data;

                        // Delete the iBT field
                        DBS.RemoveAt(k);
                    }

                    // Skip over certain unrecognized fields. We are seeing in the raw
                    // Manado data \ov's, which cause the BT not to be recognized.
                    // So I'll use this code to read in Manado data, but may want to
                    // remove it later, as it is definitely a kludge.
                    else if (fBT.Mkr == "ov" || fBT.Mkr == "nt")
                    {
                        ++k;
                        continue;
                    }

                    else
                        break;
                }

                i++;
            }
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
            // Reverse the effect of OnImport(), this inserts back translation fields
            // after any field that has one.
            //
            // (For Paratext, don't bother, as we're just going to drop them on export anyway.)
        {
            base.OnExport();

            if (!IsToolboxFormat)
                return;

            for (int i = 0; i < DBS.Count; i++)
            {
                SfField fText = DBS.GetFieldAt(i);

                if (fText.BT.Length > 0)
                {
                    DBS.InsertAt(i + 1, new SfField(c_sPrefixBT + fText.Mkr, fText.BT));
                    fText.BT = "";
                    i++;
                }
                if (fText.IBT.Length > 0)
                {
                    DBS.InsertAt(i + 1, new SfField(c_sPrefixIBT + fText.Mkr, fText.IBT));
                    fText.IBT = "";
                    i++;
                }
            }
        }
        #endregion

        public const string c_sName = "CombinedBTs";
        #region Constructor(DB)
        public XForm_CombinedBTs(ScriptureDB DB)
            : base(DB, c_sName, 
                null,
                new string[] { XForm_MoveChapterPosition.c_sName })
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_MoveChapterPosition
    public class XForm_MoveChapterPosition : XForm
    {
        #region OMethod: void OnImport()
        public override void OnImport()
            // Signal: 
            //    - a \c occuring just prior to a \s, or
            //    - a \c occuring just prior to a vernacular paragraph (\p)
            // Move:
            //   - Skip over any \s, \bts, \nt, \ft, etc
            //   - Move to be just after the first pragraph marker
        {
            base.OnImport();

            for (int i = 0; i < DBS.Count - 1; i++)
            {
                // Get the fields that might satisfy the signal
                SfField fChapter = DBS.GetFieldAt(i);
                SfField fNext = DBS.GetFieldAt(i + 1);

                // Are we sitting on a Chapter marker? 
                if (!Map.IsChapter(fChapter.Mkr))
                    continue;

                // Is the next field a Section, Major Section, or Vernacular Paragraph marker?
                if (!Map.IsSection(fNext.Mkr) &&
                    !Map.IsMajorSectionMarker(fNext.Mkr) &&
                    DBS.RecordMkr  != fNext.Mkr &&
                    !Map.IsVernacularParagraph(fNext.Mkr))
                {
                    continue;
                }

                // Move needed: Look for the proper insertion place and make the move
                for (int k = i + 1; k < DBS.Count - 1; k++)
                {
                    SfField fPara = DBS.GetFieldAt(k);

                    // Skip past the section header markers
                    if (Map.IsMajorSectionMarker(fPara.Mkr))
                        continue;
                    if (Map.IsSection(fPara.Mkr))
                        continue;
                    if (DBS.RecordMkr == fPara.Mkr)
                        continue;
                    if (fNext.Mkr == "History")
                        continue;

                    // A Vernacular Paragraph is where we want to move the chapter number to
                    if (Map.IsVernacularParagraph(fPara.Mkr))
                    {
                        // Move the field
                        DBS.MoveTo(i, k + 1);

                        // Main loop processing can continue after the moved position
                        i = k + 1;

                        break;
                    }
                } // endfor k
            } // endfor i
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
            // Signal:
            //   1. a \c occurring just following a paragraph marker
            //   2. the paragraph marker must be the first one after the section head
            // Move:
            //  If the paragraph marker is the first one in the section,
            //      - Backtrack to find the above section head, move prior to it
            //  Otherwise
            //      - Move it to just in front of the paragraph marker
        {
            base.OnExport();

            for (int i = 1; i < DBS.Count; i++)
            {
                // Are we dealing with a chapter paragraph? If not, loop to the next one
                SfField fChapter = DBS.GetFieldAt(i);
                if (!Map.IsChapter(fChapter.Mkr))
                    continue;

                // Signal #1 - Is the previous field a vernacular paragraph?
                SfField fParagraph = DBS.GetFieldAt(i - 1);
                if (!Map.IsVernacularParagraph(fParagraph.Mkr))
                    continue;

                // How many paragraph markers are prior to this one in this section?
                int cPriorMarkers = 0;
                for (int k = i - 2; k > 0; k--)
                {
                    SfField field = DBS.GetFieldAt(k);
                    if (field.Mkr == DBS.RecordMkr || Map.IsSection(field.Mkr))
                        break;
                    if (Map.IsVernacularParagraph(field.Mkr))
                        ++cPriorMarkers;
                }

                // If there are no prior markers, then find the section marker above us, 
                // and move to be just before it
                if (cPriorMarkers == 0)
                {
                    for (int k = i - 1; k > 0; k--)
                    {
                        SfField field = DBS.GetFieldAt(k);

                        if (field.Mkr == DBS.RecordMkr)
                            break;

                        if (Map.IsSection(field.Mkr))
                        {
                            DBS.MoveTo(i, k);
                            break;
                        }
                    }
                    continue;
                }

                // Otherwise, just move it prior to the paragraph above us.
                DBS.MoveTo(i, i - 1);
            }
        }
        #endregion

        public const string c_sName = "MoveChapterPosition";
        #region Constructor(DB)
        public XForm_MoveChapterPosition(ScriptureDB DB)
            : base(DB, c_sName, 
                new string[] { XForm_CombinedBTs.c_sName } , 
                null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_USFMPictures
    public class XForm_USFMPictures : XForm
    {
        // Constants -------------------------------------------------------------------------
        const string c_sFigMkr = "fig";
        const string c_sFigBegin = "\\fig ";
        const string c_sFigEnd = "\\fig*";

        // Embedded Class: FigureParts -------------------------------------------------------
        #region CLASS FigureParts
        class FigureParts
        {
            // Attrs -------------------------------------------------------------------------
            string[] m_vsFields;

            // Interpretations of the various m_vsFields -------------------------------------
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

            // Methods -----------------------------------------------------------------------
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

                    s += XForm_USFMPictures.c_sFigEnd;

                    return s;
                }
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor()
            public FigureParts()
            {
                m_vsFields = new string[7];
            }
            #endregion
        }
        #endregion

        // Import Helper Methods -------------------------------------------------------------
        #region Method: void _ExtractIntoSeparateFields()
        void _ExtractIntoSeparateFields()
            // Separate into their own individual fields (rather than leaving as 
            // part of a containing field.)
        {
            for (int i = 0; i < DBS.Fields.Count; i++)
            {
                SfField field = DBS.Fields[i] as SfField;

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
                DBS.Fields.Insert(i + 1, new SfField(c_sFigMkr, sFigData));
                if (sRight.Length > 0)
                    DBS.Fields.Insert(i + 2, new SfField("vt", sRight));
            }
        }
        #endregion
        #region Method: void _RemoveEndMarkers()
        void _RemoveEndMarkers()
            // Remove the trailing \\fig* markers
        {
            for (int i = 0; i < DBS.Fields.Count; )
            {
                SfField field = DBS.Fields[i] as SfField;

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
            for (int i = 0; i < DBS.Fields.Count; i++)
            {
                SfField field = DBS.Fields[i] as SfField;
                if (field.Mkr != c_sFigMkr)
                    continue;

                FigureParts fp = FigureParts.Parse(field.Data);

                DBS.Fields.RemoveAt(i);

                if (!string.IsNullOrEmpty(fp.Caption))
                {
                    DBS.Fields.Insert(i, new SfField(
                        DSFMapping.c_sMkrPictureCaption,
                        fp.Caption));
                }

                if (!string.IsNullOrEmpty(fp.Size))
                {
                    DBS.Fields.Insert(i, new SfField(
                        DSFMapping.c_sMkrPictureWordRtf,
                        fp.Size));
                }

                SfField fCat = new SfField(
                    DSFMapping.c_sMkrPicturePath,
                    (!string.IsNullOrEmpty(fp.FileName) ? fp.FileName : "Unknown"));
                DBS.Fields.Insert(i, fCat);
            }
        }
        #endregion

        // I/O Entry Points ------------------------------------------------------------------
        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            _ExtractIntoSeparateFields();
            _RemoveEndMarkers();
            _InterpretIntoToolbox();
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
        {
            base.OnExport();
            if (!IsParatextFormat)
                return;

            FigureParts fp = null;

            for (int i = 0; i < DBS.Fields.Count; )
            {
                SfField field = DBS.Fields[i] as SfField;

                if (!DSFMapping.IsPicture(field.Mkr))
                {
                    if (null != fp)
                    {
                        DBS.Fields.Insert(i, new SfField(c_sFigMkr, fp.USFM_Out));
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

                DBS.Fields.RemoveAt(i);
            }

            if (null != fp)
                DBS.Fields.Insert(DBS.Fields.Count, new SfField(c_sFigMkr, fp.USFM_Out));
        }
        #endregion

        public const string c_sName = "USFMPictures";
        #region Constructor(DB)
        public XForm_USFMPictures(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_EnsureParagraphAfterPicture
    public class XForm_EnsureParagraphAfterPicture : XForm
    {
        #region OMethod: void OnImport()
        public override void OnImport()
            // Signal:
            //   - The field following the last picture field should be the
            //       start of a vernacular paragraph, not a \v.
            // Action:
            //   - Insert a \p if there is not one present.
            //
            // On output, there is no need to reverse this. We are doing it
            // as an Import side-effect.
        {
            base.OnImport();

            for (int i = 1; i < DBS.Count; i++)
            {
                SfField fPicture = DBS.GetFieldAt(i - 1);
                SfField fVerse = DBS.GetFieldAt(i);

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
                DBS.InsertAt(i, new SfField("p"));
                i++;
            }
        }
        #endregion

        public const string c_sName = "EnsureParagraphAfterPicture";
        #region Constructor(DB)
        public XForm_EnsureParagraphAfterPicture(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_EnsureRecordsBeforeAllSections
    public class XForm_EnsureRecordsBeforeAllSections : XForm
    {
        #region OMethod: void OnImport()
        public override void OnImport()
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
            base.OnImport();

            // Keep a count of the number of the section we are currently processing
            int nSection = 0;

            // Loop through the database
            for (int i = 0; i < DBS.Count; i++)
            {
                // Go to each Section
                SfField fSection = DBS.GetFieldAt(i);
                if (!Map.IsSection(fSection.Mkr))
                    continue;
                nSection++;

                // Backtrack to the previous section and see if we find a record marker prior to it
                bool bRecordMarkerFound = false;
                for (int k = i - 1; k >= 0; k--)
                {
                    // Get the field
                    SfField f = DBS.GetFieldAt(k);

                    // If it is a record marker, then we have one.
                    if (f.Mkr == DBS.RecordMkr)
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
                            SfField f = DBS.GetFieldAt(iPos - 1);
                            if (Map.IsSectionContentsMarker(f.Mkr))
                                iPos--;
                            else
                                break;
                        }
                    }

                    // Do the insertion
                    string sRecordKey = nSection.ToString();
                    DBS.InsertAt(iPos, new SfField(DBS.RecordMkr, sRecordKey));
                    i++;
                }
            }
        }
        #endregion

        public const string c_sName = "EnsureRecordsBeforeAllSections";
        #region Constructor(DB)
        public XForm_EnsureRecordsBeforeAllSections(ScriptureDB DB)
            : base(DB, c_sName,
            new string[] { XForm_NormalizeMarkers.c_sName }, 
            null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_EnsureInitialTitleRecord
    public class XForm_EnsureInitialTitleRecord : XForm
        // Make sure that we have an initial record that contains the
        // book overview stuff (e.g., a title field)
    {
        #region OMethod: void OnImport()
        public override void OnImport()
            // We need to make certain that the book:
            // - Begins with a record which has the title, but not Scripture
            // - Has a title (mt) field
        {
            base.OnImport();

            // First Pass: make certain we have a \mt field prior to any Scripture
            // sections, so that we actually have something to be in that
            // first record
            int iFirstRecordMarker = -1;
            for (int i = 0; i < DBS.Count; i++)
            {
                // Retrieve the next field
                SfField field = DBS.GetFieldAt(i);

                // Loop over this field if it is a Toolbox field
                if (field.Mkr.StartsWith("_"))
                    continue;

                // This will locate the first record marker
                if (DBS.RecordMkr == field.Mkr && -1 == iFirstRecordMarker)
                    iFirstRecordMarker = i;

                // This will locate the first title section field, in which
                // case we are satisfied with this loop pass.
                if (DBS.RecordMkr != field.Mkr && Map.IsBookOverviewMarker(field.Mkr))
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
                    DBS.InsertAt(iPos, new SfField(DBS.RecordMkr, ""));
                    DBS.InsertAt(iPos + 1, new SfField(Map.MkrMainTitle, ""));

                    // No need to loop farther
                    break;
                }
            }

            // Second Pass: The first marker after any Shoebox markers (defined as "\_....")
            // should be a record marker. This defines our initial section.
            for (int i = 0; i < DBS.Count; i++)
            {
                // Retrieve the next field
                SfField field = DBS.GetFieldAt(i);

                // Loop over this field if it is a Toolbox field
                if (field.Mkr.StartsWith("_"))
                    continue;

                // Otherwise, it should be a record marker, so insert one if not
                if (field.Mkr != DBS.RecordMkr)
                    DBS.InsertAt(i, new SfField(DBS.RecordMkr, ""));

                // No need to loop any further
                break;
            }
        }
        #endregion

        public const string c_sName = "EnsureInitialTitleRecord";
        #region Constructor(DB)
        public XForm_EnsureInitialTitleRecord(ScriptureDB DB)
            : base(DB, c_sName,
            new string[] { XForm_NormalizeMarkers.c_sName }, 
            null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_EnsureSectionsExist
    public class XForm_EnsureSectionsExist : XForm
        // If we don't have sections, then create one for each chapter.
        // Important: (1) must follow sometime after the call to 
        //    _MoveChaptersIntoParagraphs, so that the new \rcrd's get
        //    placed before the paragraph that preceeds the chapter;
        // (2) must follow sometime after _EnsureRecordsBeforeAllSections,
        //    so that it is disabled if sections already exist.
        // (3) must follow EnsureInitialTitleRecord, which gives us the
        //    first record (DBS.RecordCount==1)
    {
        #region OMethod: void OnImport()
        public override void OnImport()
            // If we have no sections at this point, then we insert one for
            // each chapter. Thus, e.g., some Psalms we've seen which have
            // no sections would wind up having a section defined for each
            // Psalm. We do not insert \s markers, though.
        {
            base.OnImport();

            // If we have exactly one record, it is for the title area, and we don't
            // have any sections for the Scriptures.
            if (1 < DBS.RecordCount)
                return;

            int iParagraph = -1;

            for (int i = 0; i < DBS.Count; i++)
            {
                // Keep track of our most recent vernacular paragraph
                if (Map.IsVernacularParagraph(DBS.GetFieldAt(i).Mkr))
                    iParagraph = i;

                // Are we at a chapter? Skip if not.
                SfField fChapter = DBS.GetFieldAt(i);
                if (!Map.IsChapter(fChapter.Mkr))
                    continue;

                // Insert a record marker
                if (iParagraph != -1)
                {
                    DBS.InsertAt(iParagraph, new SfField(DBS.RecordMkr, fChapter.Data));
                    iParagraph = -1;
                    i++;
                }
            }
        }
        #endregion

        public const string c_sName = "EnsureSectionsExist";
        #region Constructor(DB)
        public XForm_EnsureSectionsExist(ScriptureDB DB)
            : base(DB, c_sName, 
            new string[] { 
                XForm_MoveChapterPosition.c_sName, 
                XForm_EnsureRecordsBeforeAllSections.c_sName,
                XForm_EnsureInitialTitleRecord.c_sName}, 
            null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_MoveTitleFields
    public class XForm_MoveTitleFields : XForm
    {
        #region Method: bool IsHeaderOrBookTitle(sMkr)
        bool IsHeaderOrBookTitle(string sMkr)
        {
            string[] vsMarkers = 
            {
                StyleSheet.RunningHeader.ToolboxMarker,
                StyleSheet.BookTitle.ToolboxMarker,
                StyleSheet.BookSubTitle.ToolboxMarker
            };

            foreach (var sSFM in vsMarkers)
            {
                if (sMkr == sSFM)
                    return true;
                if (sMkr == ("bt" + sSFM))
                    return true;
            }

            return false;
        }
        #endregion

        #region Method: void MoveTitleFieldsToRecord(nRecordNo)
        void MoveTitleFieldsToRecord(int nRecordNo)
        {
            // We are either moving into the 1st (output) or the 2nd (input) records
            Debug.Assert(nRecordNo == 1 || nRecordNo == 2);

            // Find and remove all of the title fields
            var vTitleFields = new List<SfField>();
            bool bProcessingTitleFields = false;
            foreach (SfField field in DBS.Fields)
            {
                // Check for running headers and book titles / subtitles
                if (IsHeaderOrBookTitle(field.Mkr))
                {
                    bProcessingTitleFields = true;
                    vTitleFields.Add(field);
                    continue;
                }

                // If its an accompanying translator note, add it, too
                if (field.Mkr == "tn" && bProcessingTitleFields)
                {
                    vTitleFields.Add(field);
                    continue;
                }

                // Otherwise, we're in a different type of field
                bProcessingTitleFields = false;
            }
            foreach (SfField field in vTitleFields)
                DBS.Remove(field);

            // Find the target Scripture section (it is the Nth record marker)
            int cRecords = 0;
            int iInsertPos = -1;
            for (int i = 0; i < DBS.Count - 1; i++)
            {
                SfField field = DBS.GetFieldAt(i);
                if (field.Mkr == DBS.RecordMkr)
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
                for (int k = 0; k < vTitleFields.Count; k++)
                {
                    DBS.Fields.Insert(iInsertPos, vTitleFields[k] as SfField);
                    iInsertPos++;
                }
            }
        }
        #endregion

        #region OMethod: void OnImport()
        public override void OnImport()
            // Move the title fields (\mt, \st, \h) into the second section (which is
            // the first Scripture section.) We move them there for easier editing
        {
            base.OnImport();

            // Move title fields from the first record to the second record
            MoveTitleFieldsToRecord(2);
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
            // Move the title fields from the second record (the first Scripture section)
            // into the first record (the "about this book" record.)
        {
            base.OnExport();
            if (!IsToolboxFormat)
                return;

            // Move title fields from the second record to the first record
            MoveTitleFieldsToRecord(1);
        }
        #endregion

        public const string c_sName = "MoveTitleFields";
        #region Constructor(DB)
        public XForm_MoveTitleFields(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_MoveMajorSectionParagraphs
    public class XForm_MoveMajorSectionParagraphs : XForm
        // Move Major Section Headings (\ms, \mr) into the next record, if they
        // occur at the end of a record
    {
        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            for (int i = DBS.Count - 2; i >= 0; i--)
            {
                SfField fMajor = DBS.GetFieldAt(i);
                SfField fRecord = DBS.GetFieldAt(i + 1);
                if (!Map.IsMajorSectionMarker(fMajor.Mkr))
                    continue;
                if (fRecord.Mkr != DBS.RecordMkr)
                    continue;

                DBS.Fields.Reverse(i, 2);
            }
        }
        #endregion

        public const string c_sName = "MoveMajorSectionParagraphs";
        #region Constructor(DB)
        public XForm_MoveMajorSectionParagraphs(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_MoveParagraphText
    public class XForm_MoveParagraphText : XForm
        // Deal with any USFM fields of the form: "\q When she cries,"
    {
        #region OMethod: void OnImport()
        public override void OnImport()
            // This is similar to what we do in the XForm_NormalizeVerseText method.
            //
            // Paratext (USFM) data can place text beside a paragraph marker, e.g.,
            //   \q1 When she cries, no one can comfort her, 
            //
            // We need to change that to
            //   \q1
            //   \vt When she cries, no one can comfort her,
        {
            base.OnImport();

            for (int i = 0; i < DBS.Count; i++)
            {
                // Get the fields that might satisfy the signal
                SfField field = DBS.GetFieldAt(i);
                if (!Map.IsVernacularParagraph(field.Mkr))
                    continue;

                // Remove any leading or trailing spaces
                field.Data = field.Data.Trim();

                // If we have data, then create the following \vt field for it
                if (field.Data.Length > 0)
                {
                    DBS.InsertAt(i + 1, new SfField(Map.MkrVerseText, field.Data));
                    field.Data = "";
                }
            }
        }
        #endregion

        public const string c_sName = "MoveParagraphText";
        #region Constructor(DB)
        public XForm_MoveParagraphText(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_ConvertParatextVerseBridges
    public class XForm_ConvertParatextVerseBridges : XForm
        // Change "\v 1,2,3 Text" into "\v 1-3 Text"
    {
        #region SMethod: ConvertParatextVerseBridge(string s)
        static public string ConvertParatextVerseBridge(string s)
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
        #endregion
        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            foreach (SfField f in DBS.Fields)
            {
                if (!Map.IsVerse(f.Mkr))
                    continue;

                f.Data = ConvertParatextVerseBridge(f.Data);
            }
        }
        #endregion

        public const string c_sName = "ConvertParatextVerseBridges";
        #region Constructor(DB)
        public XForm_ConvertParatextVerseBridges(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_NormalizeVerseText
    public class XForm_NormalizeVerseText : XForm
        // Change "\v 5 hello" into a "\v" followed by a "\vt"
    {
        #region Method: bool IsDigit(char ch)
        static bool IsDigit(char ch)
        {
            if (Char.IsDigit(ch))
                return true;
            if (ch == 'l' || ch == 'O')
                return true;
            return false;
        }
        #endregion
        #region Method: void ParseVerseStrings(...)
        static public void ParseVerseString(string s, ref string sVerseNo, ref string sVerseText)
        {
            int i = 0;
            sVerseNo = "";
            sVerseText = "";

            // Remove any leading spaces
            s = s.Trim();

            // Could have some leading punctuation (saw this is 43LukTch.ptx). We'll just
            // move this to appear after the verse number.
            while (s.Length > i && (s[i] == '(' || s[i] == '['))
            {
                sVerseText += s[i];
                i++;
            }

            // Loop through the string
            bool bMostRecentWasDigit = false;
            for (; i < s.Length; i++)
            {
                char ch = s[i];
                char chNext = (i < s.Length - 1) ? s[i + 1] : '\0';

                // Verses will normally consist of digits
                if (IsDigit(ch))
                {
                    // If the data is an 'l' (el), we assume it was a typo and change it to 
                    // a '1' (one). Somewhat kuldgy, but I'm finding real data this way.
                    if (ch == 'l')
                        ch = '1';
                    // Simularly O (oh) and zero
                    if (ch == 'O')
                        ch = '0';

                    sVerseNo += ch;
                    bMostRecentWasDigit = true;
                    continue;
                }

                // A single letter is acceptable, if it immediately follows a digit (thus, '10b'),
                // and if the following item is a space or hyphen/comma
                if (char.IsLetter(ch) && bMostRecentWasDigit)
                {
                    if (chNext != '\0' && !char.IsWhiteSpace(chNext) && chNext != '-' && chNext != ',')
                        break;
                    sVerseNo += ch;
                    bMostRecentWasDigit = false;
                    continue;
                }

                // Spaces are permitted only if the next character is not a letter
                if (char.IsWhiteSpace(ch) && !char.IsLetter(chNext))
                {
                    bMostRecentWasDigit = false;
                    continue;
                }

                // We permit a comma to be interpretted as a verse bridge
                if (ch == ',')
                {
                    sVerseNo += '-';
                    bMostRecentWasDigit = false;
                    continue;
                }

                // A hyphen is used for verse bridges
                if (ch == '-')
                {
                    sVerseNo += ch;
                    bMostRecentWasDigit = false;
                    continue;
                }

                // If we're here, then we are no longer working on a verse number
                break;
            }

            // Move past any blanks
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;

            // Anything else is the verse text
            if (s.Length > i)
                sVerseText += s.Substring(i);

            // If we wound up with multiple hyphens, then get rid of the interior. Thus "3,4,5" would
            // have become "3-4-5", which we turn into "3-5"
            int k1 = sVerseNo.IndexOf('-');
            int k2 = sVerseNo.LastIndexOf('-');
            if (k1 != -1 && k2 != -1 && k1 != k2)
                sVerseNo = sVerseNo.Remove(k1, k2 - k1);
        }
        #endregion

        #region OMethod: void OnImport()
        public override void OnImport()
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
            base.OnImport();

            for (int i = 0; i < DBS.Count; i++)
            {
                // Is this a field of interest? (Loop to the next if not)
                SfField field = DBS.GetFieldAt(i);
                if (!Map.IsVerse(field.Mkr))
                    continue;

                // Remove any leading or trailing spaces
                field.Data = field.Data.Trim();

                // We'll put the data into two places
                string sVerseNo = "";
                string sVerseText = "";

                // Parse the data
                ParseVerseString(field.Data, ref sVerseNo, ref sVerseText);

                // The normalized verse data now goes into our original field
                field.Data = sVerseNo;

                // Remove any trailing \v*'s we find
                if (sVerseText.Length > 3 && sVerseText.EndsWith("\\v*"))
                    sVerseText = sVerseText.Substring(0, sVerseText.Length - 3);

                // If we had verse text, then we need to create a field for it
                if (!string.IsNullOrEmpty(sVerseText))
                    DBS.InsertAt(i + 1, new SfField(Map.MkrVerseText, sVerseText));
            }
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
            // Reverses _NormalizeVerseText, so that
            //     \v 5
            //     \vt Yihuda bitta Kawuwaa Heroddisa wodian Abiiya
            // becomes
            //     \v 5 Yihuda bitta Kawuwaa Heroddisa wodian Abiiya
        {
            base.OnExport();
            if (IsToolboxFormat)
                return;

            for (int i = 0; i < DBS.Count - 1; )
            {
                // Get the fields that might satisfy the signal
                SfField fv = DBS.GetFieldAt(i);
                SfField fvt = DBS.GetFieldAt(i + 1);

                // Do they fit the signal? If not, loop to the next one. (This is the
                // only place we increment i, because we may have more than one \vt
                // to combine.
                if (fvt.Mkr != Map.MkrVerseText)
                {
                    i++;
                    continue;
                }

                // Combine the two
                fv.Data = fv.Data.TrimEnd() + " " + fvt.Data;
                fv.BT = fv.Data.TrimEnd() + " " + fvt.BT;

                // Delete the VerseText field
                DBS.RemoveAt(i + 1);
            }
        }
        #endregion

        public const string c_sName = "NormalizeVerseText";
        #region Constructor(DB)
        public XForm_NormalizeVerseText(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_HandleParatextSeeAlsosAndFootnotes
    public class XForm_HandleParatextSeeAlsosAndFootnotes : XForm
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
    {
        // Joining fields --------------------------------------------------------------------
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
            for (int i = 0; i < DBS.Count - 1; )
            {
                SfField f1 = DBS.GetFieldAt(i);
                SfField f2 = DBS.GetFieldAt(i + 1);

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

                    // AddParagraph the contents of the following field
                    f1.Data += ("\\" + f2.Mkr + " " + f2.Data);

                    // Remove the following field.
                    DBS.Remove(f2);
                }
                else
                    i++;
            }
        }
        #endregion

        // SeeAlso Import Methods ------------------------------------------------------------
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
            foreach (string s in vsX)
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
                while (i < sData.Length && sData[i] != '\\')
                {
                    iLen++;
                    i++;
                }
            }

            // Now, we will typically have the Cross Reference "Origin" marker
            string[] vsXO = { "\\xo", "\\rc" };
            foreach (string s in vsXO)
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
        #region Method: void OnImportSeeAlso()
        public void OnImportSeeAlso()
            //   \cf 1:78: Mal 4.2
        {
            // First, make pass to deal with any markers that somehow wound up being
            // in their own fields
            JoinParatextEmbeddedFields(ParatextCrossReferenceMkrsToJoin);

            // Loop to process each field
            for (int i = 0; i < DBS.Count; i++)
            {
                // Get the next verse marker field (\vt)
                SfField f = DBS.GetFieldAt(i);
                if (!DBS.Map.IsVerseText(f.Mkr))
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
                    DBS.InsertAt(i + 1, new SfField(DBS.Map.MkrVerseText, sEnd));

                // Create and insert the cross reference field
                DBS.InsertAt(i + 1, new SfField(DBS.Map.MkrSeeAlso, sSeeAlso));
            }
        }
        #endregion
        #region Method: void OnImportFootnotes()
        public void OnImportFootnotes()
        {
            base.OnImport();

            // First, make pass to deal with any markers that somehow wound up being
            // in their own fields
            JoinParatextEmbeddedFields(ParatextFootnoteMkrsToJoin);

            // The markers that denote the beginning and ending of a footnote
            string sBegin = "\\f ";
            string sEnd = "\\f*";

            // Loop to process each field
            for (int iF = 0; iF < DBS.Count; iF++)
            {
                // Get the next verse marker field (\vt)
                SfField f = DBS.GetFieldAt(iF);
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
                    // to the next "\" marker or digit (a digit is needed where there is 
                    // a footnote of the form "\f + 1:2 Maleakhi 3:1\f*".
                    k += sBegin.Length;
                    while (k < f.Data.Length && f.Data[k] != '\\' && !char.IsDigit(f.Data[k]))
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
                    DBS.InsertAt(iF + 1, new SfField(Map.MkrFootnote, aFootnotes[n] as string));
                }
            }

        }
        #endregion
        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            OnImportSeeAlso();
            OnImportFootnotes();
        }
        #endregion

        // Export methods --------------------------------------------------------------------
        #region Method: void OnExportSeeAlso()
        public void OnExportSeeAlso()
            // Given: \cf 1:78: Mal 4.2, Luke 9:7-8
            // Create: ....text.... \x \xo 1:78 \xo* Mal 4.2, Luke 9:7-8\x*
            //
            // In OW, we know that the Reference part is everything up until the first
            // whitespace encountered.
        {
            if (IsToolboxFormat || IsGoBibleFormat)
                return;

            for (int i = 0; i < DBS.Count - 1; i++)
            {
                // Locate a SeeAlso field
                SfField fText = DBS.GetFieldAt(i);
                SfField fSeeAlso = DBS.GetFieldAt(i + 1);
                if (!DBS.Map.IsSeeAlso(fSeeAlso.Mkr))
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

                // AddParagraph it to the previous field, and remove the cross ref field
                fText.Data = fText.Data.Trim() + sSeeAlso;
                DBS.RemoveAt(i + 1);

                // Process at t his position again in case we have another one
                i--;
            }
        }
        #endregion
        #region Method: void ParseFootnote(...)
        public static void ParseFootnote(string s, out string sReference, out string sContents)
        {
            // Default values
            sReference = "";
            sContents = s;

            // A legal reference is a digit, or limited other punctuation characters
            const string sReferenceChars = "0123456789:-., ";

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
        #endregion
        #region Method: void OnExportFootnotes()
        private void OnExportFootnotes()
            // Paratext: Embed them into the text
            // Toolbox: Already where we want them
            // GoBible: Remove them
        {
            if (IsToolboxFormat || IsGoBibleFormat)
                return;

            for (var i = 0; i < DBS.Count - 1; i++)
            {
                // Are we at text that contains a footnote? ("|fn")
                var fieldCallout = DBS.GetFieldAt(i);
                if (!Map.IsVerseText(fieldCallout.Mkr))
                    continue;
                var iCalloutPosition = fieldCallout.Data.IndexOf("|fn");
                if (-1 == iCalloutPosition)
                    continue;

                // Go forward to find the footnote
                SfField fieldFootnote = null;
                for(var k = i+1; k<DBS.Count; k++)
                {
                    fieldFootnote = DBS.GetFieldAt(k);
                    if (Map.IsFootnote(fieldFootnote.Mkr))
                        break;
                }
                if (null == fieldFootnote)
                    continue;

                // Parse the footnote into its parts
                string sReference;
                string sContents;
                ParseFootnote(fieldFootnote.Data, out sReference, out sContents);

                // Remove the footnote field
                DBS.Remove(fieldFootnote);

                // Build the appropriate format for footnotes
                if (sReference.Length > 0 || sContents.Length > 0)
                {
                    // Build the appropriate footnote text.
                    var sReferenceString = (string.IsNullOrEmpty(sReference)) ? null :
                        string.Format("+ \\fr {0}\\fr* ", sReference);

                    var sFootnote = string.Format("\\f {0}{1}\\f* ", sReferenceString, sContents);

                    // Substitute it into the string
                    var sBefore = fieldCallout.Data.Substring(0, iCalloutPosition);
                    var sAfter = fieldCallout.Data.Substring(iCalloutPosition + 3);
                    fieldCallout.Data = (sBefore + sFootnote + sAfter).Trim();
                }

                i--;
            }
        }
        #endregion
        #region OMethod: void OnExport()
        public override void OnExport()
        {
            base.OnExport();

            RemoveTranslatorNotes();
            OnExportSeeAlso();
            OnExportFootnotes();
        }
        #endregion

        public const string c_sName = "HandleSeeAlsosAndFootnotes";
        #region Constructor(DB)
        public XForm_HandleParatextSeeAlsosAndFootnotes(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion

        void RemoveTranslatorNotes()
        {
            for (var i = 0; i < DBS.Count - 1;)
            {
                var field = DBS.GetFieldAt(i);

                if (field.Mkr == DSFMapping.c_sMkrTranslatorNote)
                    DBS.Remove(field);
                else
                    i++;
            }
        }
    }
    #endregion
    #region CLASS: XForm_HandleEmbeddedParatextParagraphs
    public class XForm_HandleEmbeddedParatextParagraphs : XForm
    {
        #region OMethod: void OnImport()
        public override void OnImport()
        {
            base.OnImport();

            // We're assuming Paratext's lack of back translations when we split these 
            // lines into multiple lines
            if (!IsParatextFormat)
                return;

            // Calculate this virt attr just once
            var vMarkers = Map.VernacularParagraphMarkers;
            var vPatterns = new string[ vMarkers.Count ];
            for (int i = 0; i < vMarkers.Count; i++)
                vPatterns[i] = "\\" + vMarkers[i] + " ";

            // Loop through each field looking for the embedded content markers
            for (int i = 0; i < DBS.Count; i++)
            {
                SfField field = DBS.GetFieldAt(i);

                // Optimization: don't bother if there's no backslash in the data
                if (field.Data.IndexOf('\\') == -1)
                    continue;

                for(int k=0; k<vPatterns.Length; k++)
                {
                    int iPos = field.Data.IndexOf(vPatterns[k]);
                    if (iPos != -1)
                    {
                        string sLeft = field.Data.Substring(0, iPos);
                        string sRight = field.Data.Substring(iPos + vPatterns[k].Length);

                        field.Data = sLeft;

                        DBS.InsertAt(i + 1, new SfField(vMarkers[k], sRight));

                        //Console.WriteLine("");
                        //Console.WriteLine(vMarkers[k] + " Encountered:");
                        //foreach (SfField f in DBS.Fields)
                        //    Console.WriteLine(f.Mkr + " " + f.Data);
                    }
                }
            }
        }
        #endregion

        public const string c_sName = "HandleEmbeddedParatextParagraphs";
        #region Constructor(DB)
        public XForm_HandleEmbeddedParatextParagraphs(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion
    #region CLASS: XForm_ConsolidateRunningHeaders
    public class XForm_ConsolidateRunningHeaders : XForm
    {
        #region OMethod: void OnImport()
        public override void OnImport()
            // We only want one \h field, get rid of any additional ones.
            // (We found Djambarrpunygu files with an \h within every section)
        {
            base.OnImport();

            var sHeaderText = "";

            // Collect the first running header text we encounter
            foreach(SfField field in DBS.Fields)
            {
                if (field.Mkr == StyleSheet.RunningHeader.ToolboxMarker)
                {
                    if (string.IsNullOrEmpty(sHeaderText))
                    {
                        sHeaderText = field.Data.Trim();
                        if (!string.IsNullOrEmpty(sHeaderText))
                            break;
                    }
                }
            }

            // Place it in the first running header we find, remove all the rest
            var bHeaderTextAdded = false;
            for (var i = 0; i < DBS.Fields.Count; )
            {
                var field = DBS.GetFieldAt(i);

                if (field.Mkr == StyleSheet.RunningHeader.ToolboxMarker)
                {
                    if (!bHeaderTextAdded)
                    {
                        field.Data = sHeaderText;
                        bHeaderTextAdded = true;
                    }
                    else
                    {
                        DBS.RemoveAt(i);
                        continue;
                    }
                }

                i++;
            }

        }
        #endregion

        public const string c_sName = "ConsolidateHeaders";
        #region Constructor(DB)
        public XForm_ConsolidateRunningHeaders(ScriptureDB DB)
            : base(DB, c_sName, null, null)
        {
        }
        #endregion
    }
    #endregion

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
                if (DB.Project == null || DB.TeamSettings == null)
                    return new DSFMapping();

				return DB.TeamSettings.SFMapping;
			}
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
        #region Attr{g/s}: Format Format - kParatext, kToolbox, kGoBibleCreator, kUnknown
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
        public enum Formats { kParatext, kToolbox, kGoBibleCreator, kUnknown };
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
        #region VAttr{g}: bool IsGoBibleFormat
        public bool IsGoBibleFormat
        {
            get
            {
                return (Format == Formats.kGoBibleCreator);
            }
        }
        #endregion
        #region Method: Formats _DetermineFormat()
		Formats _DetermineFormat()
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

            return m_Format;
        }
        #endregion

        // XForms ----------------------------------------------------------------------------
        #region Attr{g}: List<XForm> XForms
        public List<XForm> XForms
        {
            get
            {
                Debug.Assert(null != m_vXForms);
                return m_vXForms;
            }
        }
        List<XForm> m_vXForms;
        #endregion
        #region Method: void TransformIn()
        public void TransformIn()
            // Takes a raw standard format file (either Toolbox or Paratext) and massages it 
            // into a standard that the Import methods can more easily deal with. Do in the 
            // reverse order of TransformOut
		{
            // In only supports Toolbox and Paratext
            Debug.Assert(Format != Formats.kGoBibleCreator);

            for (int i = 0; i < XForms.Count; i++)
                XForms[i].OnImport();

            // And old one that we don't use now (for reason I can't rememver)
			// Handle a \p that preceeds a \s with no content
			// _RemoveCertainEmptyParagraphs();
		}
		#endregion
		#region Method: void TransformOut()
		public void TransformOut()
			// Do in the reverse order of TransformIn
		{
            Debug.Assert(Format != Formats.kUnknown);

            // XForms
            for (var i = XForms.Count - 1; i >= 0; i--)
            {
                XForms[i].OnExport();
                //XForms[i].DebugDump("After " + XForms[i].Name);
            }

            // Get rid of any double white spaces that may have been introduced
            _RemoveExtraSpaces();

            // GoBibleCreator post-transforms. 
            // IMPORTANT: Keep these in order. The code for inserting the verse bridges
            // requires that the paragraph markers have been removed first.
            if (Format == Formats.kGoBibleCreator)
            {
                RemoveParagraphMarkersForGoBible();
                RemoveVerseBridgesForGoBible();
            }
		}
		#endregion

        // Other Transforms ------------------------------------------------------------------
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

        #region Method: void RemoveVerseBridgesForGoBible()
        public void RemoveVerseBridgesForGoBible()
            // If we have "\v 1a text", we treat it as "\v 1 (1a) text"
            //            "\v 1b text",                "(1b) text" 
            //
            // Whole thing is tricky; make sure the tests pass if any mods done.
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                // Hunt for verse markers
                SfField f = Fields[i] as SfField;
                if (f.Mkr != "v")
                    continue;

                // Parse the reference parts, if any
                if (string.IsNullOrEmpty(f.Data) || !Char.IsDigit(f.Data[0]))
                    continue;
                int nVerse1 = -1;
                char chLetter1 = ' ';
                int nVerse2 = -1;
                char chLetter2 = ' ';
                string sRemainder = "";
                DReference.ParseParts(f.Data, out nVerse1, out chLetter1, 
                    out nVerse2, out chLetter2, out sRemainder);

                // If not a bridge, and if no letter, then we're done
                if (nVerse2 == -1 && chLetter1 == ' ')
                    continue;

                // Build the bridge string, e.g., "(1b-3a) "
                string sBridge = "";
                if (chLetter1 != ' ' || nVerse2 != -1)
                {
                    sBridge = "(";
                    sBridge += nVerse1.ToString();
                    if (chLetter1 != ' ')
                        sBridge += chLetter1.ToString();
                    if (nVerse2 != -1)
                    {
                        sBridge += '-';
                        sBridge += nVerse2.ToString();
                        if (chLetter2 != ' ')
                            sBridge += chLetter2.ToString();
                    }
                    sBridge += ") ";
                }

                // If on the first verse there's no letter, or the letter is 'a',
                // then we keep that output as "\v 1 ....". So if we don't have
                // that condition, then we remove the "\v" marker
                string sNumber = "";
                if (nVerse1 != -1 && (chLetter1 == ' ' || chLetter1 == 'a'))
                    sNumber = nVerse1.ToString() + " ";
                else
                    f.Mkr = SfWrite.c_sOmitMarker;

                // If we have a Bridge, then we preserve it in parenthesis
                if (!string.IsNullOrEmpty(sBridge))
                    f.Data = sNumber + sBridge + sRemainder;

                // Figure out the verses we need to add. If we don't have a bridge,
                // then we don't need to add any.
                if (-1 == nVerse2)
                    continue;

                // Find the insert position
                int iPos = i + 1;
                while (iPos < Fields.Count && (Fields[iPos] as SfField).Mkr != "v")
                    iPos++;

                // Add the new fields
                for (int n = nVerse2; n > nVerse1; n--)
                    InsertAt(iPos, new SfField("v", n.ToString()));
            }
        }
        #endregion
        #region Method: void RemoveParagraphMarkersForGoBible()
        void RemoveParagraphMarkersForGoBible()
        {
            for (int i = 0; i < Fields.Count; )
            {
                SfField f = Fields[i] as SfField;

                if (Map.IsVernacularParagraph(f.Mkr))
                {
                    f.Mkr = SfWrite.c_sOmitMarker;
                    if (string.IsNullOrEmpty(f.Data))
                    {
                        Fields.RemoveAt(i);
                        continue;
                    }
                }

                i++;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ScriptureDB()
			: base()
		{
            // Initialize the transforms
            m_vXForms = new List<XForm>();
            XForms.Add(new XForm_NormalizeMarkers(this));
            XForms.Add(new XForm_ShoeboxSpecialMarkers(this));
            XForms.Add(new XForm_HandleEmbeddedParatextParagraphs(this));
            XForms.Add(new XForm_RemoveIgnoredMarkers(this));
            XForms.Add(new XForm_CombinedBTs(this));
            XForms.Add(new XForm_MoveChapterPosition(this));
            XForms.Add(new XForm_USFMPictures(this));
            XForms.Add(new XForm_EnsureParagraphAfterPicture(this));
            XForms.Add(new XForm_EnsureRecordsBeforeAllSections(this));
            XForms.Add(new XForm_EnsureInitialTitleRecord(this));
            XForms.Add(new XForm_EnsureSectionsExist(this));
            XForms.Add(new XForm_MoveTitleFields(this));
            XForms.Add(new XForm_MoveMajorSectionParagraphs(this));
            XForms.Add(new XForm_MoveParagraphText(this));
            XForms.Add(new XForm_ConvertParatextVerseBridges(this));
            XForms.Add(new XForm_NormalizeVerseText(this));
            XForms.Add(new XForm_HandleParatextSeeAlsosAndFootnotes(this));
            XForms.Add(new XForm_GoBibleExport(this));
            XForms.Add(new XForm_ConsolidateRunningHeaders(this));
        }
		#endregion
		#region Override: bool Read(TextReader) - populate from StDmt disk file
		public override bool Read(TextReader tr)
		{
			// Read in the file (if possible)
			if (!base.Read(tr))
				return false;

            // Determine whether or not this is a Paratext file, by examining the data
            _DetermineFormat();

			return true;
		}
		#endregion

		// Misc methods ----------------------------------------------------------------------
        #region Method: ArrayList GetNormalizedMarkersInventory()
        public ArrayList GetNormalizedMarkersInventory()
        {
            // Run the process to normalize markers (e.g., "mt1" becomes "mt")
            XForm_NormalizeMarkers xform = new XForm_NormalizeMarkers(this);
            xform.OnImport();

            // We'll place the answer in an arraylist, thus it will be a list
            // of strings.
            ArrayList a = new ArrayList();

            // Loop through all of the fields in the database
            foreach (SfField field in Fields)
            {
                // For this marker, scan the ArrayList to see if it is already 
                // present there.
                bool bFound = false;
                foreach (string sMkr in a)
                {
                    if (sMkr == field.Mkr)
                        bFound = true;
                }

                // We did not find it in the ArrayList, so add it.
                if (!bFound)
                    a.Add(field.Mkr);
            }

            // Alphabetize
            a.Sort();

            return a;
        }
        #endregion
        #region Method: ArrayList GetUnknownMarkersInventory()
        public ArrayList GetUnknownMarkersInventory()
        {
            // Retrieve the inventory of markers in this DB
            ArrayList aInventory = GetNormalizedMarkersInventory();

            // We'll place any unknown markers here
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
		#region Method: string GetAbbrevFromIdLine()
		public string GetAbbrevFromIdLine()
		{
			// Find the ID line
			SfField fID = null;
			foreach (SfField f in Fields)
			{
				if (f.Mkr == DSFMapping.c_sMkrID)
				{
					fID = f;
					break;
				}
			}
			if (null == fID)
				return null;
			string sData = fID.Data.Trim();

			// Extract its first word
			string sAbbrev = "";
			foreach (char ch in sData)
			{
				if (char.IsLetterOrDigit(ch))
					sAbbrev += char.ToUpper(ch);
				else
					break;
			}
			return sAbbrev;

		}
		#endregion

		// Writing ---------------------------------------------------------------------------
		#region Method: bool Write(string sPathName)
		public bool Write(string sPathName)
		{
			TransformOut();

            // Paratext output by convention has all of the data on a single line; whereas
            // Toolbox tends to wrap lines so that they are visible in the window.
            var bWrapLines = (Format == Formats.kParatext || Format == Formats.kGoBibleCreator) ? 
                false : true;

            // Write out the database
			var bResult = base.Write(sPathName, bWrapLines);

            // Let the user know, if we were unsuccessful
			if (bResult == false)
			{
				// Give the user the bad news
                LocDB.Message("msgUnableToSaveFile",
                    "Unable to save the file: '{0}.'",
                    new string[] { sPathName },
                    LocDB.MessageTypes.Error);

				Console.WriteLine(@"Unable to save file, PathName = " + sPathName + 
					@"Error = " + base.Message);

				return false;
			}

			return true;
		}
		#endregion
	}
}
