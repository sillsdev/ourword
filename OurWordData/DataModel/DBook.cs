#region ***** DBook.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DBook.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: A book within the translation (e.g., Mark, Philemon)
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using JWTools;
using Chorus.merge;
#endregion
#endregion

namespace OurWordData.DataModel
{
    public class DBook : JObjectOnDemand, IComparable<DBook>
    {
        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string BookAbbrev - a 3-letter abbreviation for the book
        public string BookAbbrev
        {
            get
            {
                return m_sBookAbbrev;
            }
            set
            {
                SetValue(ref m_sBookAbbrev, value);
            }
        }
        private string m_sBookAbbrev = "";
        #endregion
        #region BAttr{g/s}: string ID - an ID for the file / book
        public string ID
        {
            get
            {
                // Each book should have an ID line, so default to the book's abbrev
                if (string.IsNullOrEmpty(m_sID))
                    m_sID = BookAbbrev;

                return m_sID;
            }
            set
            {
                SetValue(ref m_sID, value);
            }
        }
        private string m_sID = "";
        #endregion
        #region BAttr{g/s}: string Comment - a miscellaneous comment about the book
        public string Comment
        {
            get
            {
                return m_sComment;
            }
            set
            {
                SetValue(ref m_sComment, value);
            }
        }
        private string m_sComment = "";
        #endregion
        #region BAttr{g/s}: string Version - incremental version, 'A', 'B', etc.
        public const string c_sVersionDefault = "A";
        public string Version
        {
            get
            {
                return m_sVersion;
            }
            set
            {
                SetValue(ref m_sVersion, value);
            }
        }
        private string m_sVersion = c_sVersionDefault;
        #endregion
        #region BAttr{g/s}: bool Locked - T if user is not allowed to edit.
        public bool Locked
        {
            get
            {
                return m_bLocked;
            }
            set
            {
                SetValue(ref m_bLocked, value);
            }
        }
        private bool m_bLocked = false;
        #endregion
        #region BAttr{g/s}: string Copyright - Available to print in the footnote
        public string Copyright
        {
            get
            {
                return m_sCopyright;
            }
            set
            {
                SetValue(ref m_sCopyright, value);
            }
        }
        private string m_sCopyright = "";
        #endregion
        #region Method: void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Abbrev", ref m_sBookAbbrev);
            DefineAttr("ID", ref m_sID);
            DefineAttr("Comment", ref m_sComment);
            DefineAttr("Version", ref m_sVersion);
            DefineAttr("Locked", ref m_bLocked);
            DefineAttr("Copyright", ref m_sCopyright);
        }
        #endregion

        // JAttrs ----------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq Sections - the list of sections in this book
        public JOwnSeq<DSection> Sections
        {
            get
            {
                return j_osSections;
            }
        }
        private JOwnSeq<DSection> j_osSections = null;
        #endregion
        #region JAttr{g}: JOwnSeq Notes - seq of paragraphs given notes about misc notes
        public JOwnSeq<DParagraph> Notes
        {
            get
            {
                return j_osNotes;
            }
        }
        private JOwnSeq<DParagraph> j_osNotes = null;
        #endregion
        #region JAttr{g/s}: TranslatorNote History
        public TranslatorNote History
        {
            get
            {
                // An unload calls Clear; but we want to always make sure we have
                // an object here for any future load.
                if (null == j_ownHistory.Value)
                    j_ownHistory.Value = new TranslatorNote(TranslatorNote.History);

                return j_ownHistory.Value;
            }
            set
            {
                j_ownHistory.Value = value;
            }
        }
        private JOwn<TranslatorNote> j_ownHistory = null;
        #endregion

        // Temporary (run-time) attrs --------------------------------------------------------
        #region Attr{g/s}: bool UserHasSeenLockedMessage - T if Info_BookIsLocked has been shown
        public bool UserHasSeenLockedMessage
        {
            get
            {
                return m_bUserHasSeenLockedMessage;
            }
            set
            {
                m_bUserHasSeenLockedMessage = value;
            }
        }
        bool m_bUserHasSeenLockedMessage = false;
        #endregion
        #region Attr{g}: DTranslation Translation - returns the translation that owns this book
        public DTranslation Translation
        {
            get
            {
                Debug.Assert(null != m_Translation);
                return m_Translation;
            }
            set
            {
                m_Translation = value;
            }
        }
        DTranslation m_Translation;
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
                // We always want the book to have a stage, so don't set it to null.
                if (value != null && m_Stage.EnglishAbbrev != value.EnglishAbbrev)
                {
                    m_Stage = value;
                    IsDirty = true;
                }
            }
        }
        Stage m_Stage = DB.TeamSettings.Stages.Draft;
        #endregion

        // Derived Attributes: ---------------------------------------------------------------
        #region Attr{g}: DProject Project - returns the Project that owns this book
        public DProject Project
        {
            get
            {
                Debug.Assert(null != Translation.Project);
                return Translation.Project;
            }
        }
        #endregion
        #region Attr{g}: int BookNumber - e.g., Genesis = 1, Exodus = 2, etc.
        public int BookNumber
        {
            get
            {
                return FindBookAbbrevIndex(BookAbbrev);
            }
        }
        #endregion
        #region Attr{g}: public DSFMapping Map - used only during SF Read operation
        // This object allows us to map from the read.Marker to the way to handle each
        // field, e.g., which one is a Section Title, which one is a back translation,
        // etc.
        public DSFMapping Map
        {
            get
            {
                return Project.TeamSettings.SFMapping;
            }
        }
        #endregion
        #region Attr{g}: DBook FrontBook - the corresponding book in the Front translation
        public DBook FrontBook
        // We cannot assume that the FrontBook is the currently-loaded book,
        // and thus use the Current Front Section to get it, because we may be
        // working with a book which is not the current one; thus we do a lookup
        // instead. We use m_FrontBookSaved to remember the lookup, so that we don't
        // have to repeat the lookup over and over.
        //    We also make sure the book as been loaded.
        {
            get
            {
                if (null != m_FrontBookSaved)
                    return m_FrontBookSaved;

                if (null == Project.FrontTranslation)
                    return null;

                foreach (DBook b in Project.FrontTranslation.BookList)
                {
                    if (b.BookAbbrev == this.BookAbbrev)
                    {
                        m_FrontBookSaved = b;
                        m_FrontBookSaved.LoadBook(new NullProgress());
                        return b;
                    }
                }

                return null;
            }
        }
        private DBook m_FrontBookSaved = null;
        #endregion
        #region Attr{g}: AllSectionsMatchFront - determined by Read, T if a true daughter
        public bool AllSectionsMatchFront
        {
            get
            {
                // Optimization; we only need do this test once per book
                if (m_AllSectionsMatchFrontTestDone)
                    return m_AllSectionsMatchFront;
                m_AllSectionsMatchFrontTestDone = true;

                // If the section counts are not the same, then we know they are different.
                if (FrontBook.Sections.Count != this.Sections.Count)
                    return false;

                // If "this" is the front, then by definition we are the same.
                if (FrontBook == this)
                {
                    m_AllSectionsMatchFront = true;
                    return true;
                }

                // Loop through each section, testing for identical references
                for (int i = 0; i < Sections.Count; i++)
                {
                    DSection SFront = (DSection)FrontBook.Sections[i];
                    DSection SThis = (DSection)Sections[i];

                    if (!SFront.ReferenceSpan.ContentEquals(SThis.ReferenceSpan))
                        return false;
                }

                // If we got this far, all sections have identical references; thus
                // this book's section matches the Front.
                m_AllSectionsMatchFront = true;
                return true;
            }
        }
        private bool m_AllSectionsMatchFront = false;
        private bool m_AllSectionsMatchFrontTestDone = false;
        #endregion
        #region Attr{g}: bool IsFrontBook - T if this book is part of the Front translation
        bool IsFrontBook
        {
            get
            {
                if (Translation == Project.FrontTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region Attr{g}: bool IsTargetBook - T if this book is part of the Target translation
        public bool IsTargetBook
        {
            get
            {
                if (Translation == Project.TargetTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region Attr{g}: string SfRecordKey - The data for the book's first \rcrd field
        string SfRecordKey
        {
            get
            {
                string s = "";

                // If this is the Front, then we want whatever we have stored. Otherwise,
                // we pass the request on to the Front
                if (IsFrontBook)
                    s = m_sSfRecordKey;
                else if (null != FrontBook)
                    s = FrontBook.SfRecordKey;

                // If we get this far and still have nothing, then make up something!
                if (s.Length == 0)
                    s = BookAbbrev;

                return s;
            }
            set
            {
                if (IsFrontBook)
                    m_sSfRecordKey = value;
            }
        }
        string m_sSfRecordKey = "";
        #endregion
        #region Attr{g}: int FinalChapterNo - the largest chapter no currently in this book
        public int FinalChapterNo
        {
            get
            {
                int n = 1;
                foreach (DSection section in Sections)
                {
                    n = Math.Max(n, section.ReferenceSpan.End.Chapter);
                }
                return n;
            }
        }
        #endregion
        #region Attr{g}: bool HasCopyrightNotice - T if the user has defined a string
        public bool HasCopyrightNotice
        {
            get
            {
                if (Copyright != null && Copyright.Length > 0)
                    return true;
                return false;
            }
        }
        #endregion
        #region Attr{g}: string BaseNameForBackup
        public string BaseNameForBackup
        // For the backup file, we want a filename based on the Stage/Version/Date
        // so that it is easy to see the contents of the file at a glance.
        // "01 GEN - Amarasi - Draft-A 2008-11-23"
        {
            get
            {
                // Start with our normal base name
                string sBaseName = BaseName;

                // Add the stage (e.g., "Draft")
                sBaseName += (" - " + Stage.EnglishAbbrev);

                // Add the version number/letter (e.g., "B")
                sBaseName += ("-" + Version.ToString());

                // Add the date
                sBaseName += (" " + DB.Today);

                return sBaseName;
            }
        }
        #endregion
        #region OAttr{g}: string DisplayName
        public override string DisplayName
        {
            get
            {
                if (null != m_Translation)
                {
                    int i = FindBookAbbrevIndex(BookAbbrev);
                    return Translation.BookNamesTable[i];
                }
                return base.DisplayName;
            }
            set
            {
                base.DisplayName = value;
            }
        }
        #endregion
        #region VAttr{g}: string StatusPhrase - for displaying in the pane's title bar
        public string StatusPhrase
        {
            get
            {
                string s = "(" + Stage.LocalizedAbbrev + '-' + Version.ToString() + ")";
                return s;
            }
        }
        #endregion

        // BookAbbrev-To-BookNumber Conversions ----------------------------------------------
        public const int BookAbbrevsCount = 66;
        public const int c_iMatthew = 39;
        #region SAttr{g} string[] BookAbbrevs - e.g., "GEN", "EXO", "LEV", etc.
        static public string[] BookAbbrevs = { "GEN", "EXO", "LEV", "NUM", "DEU", "JOS",
			"JDG", "RUT", "1SA", "2SA", "1KI", "2KI", "1CH", "2CH", "EZR", "NEH",
			"EST", "JOB", "PSA", "PRO", "ECC", "SNG", "ISA", "JER", "LAM", "EZK",
			"DAN", "HOS", "JOL", "AMO", "OBA", "JON", "MIC", "NAM", "HAB", "ZEP",
			"HAG", "ZEC", "MAL", "MAT", "MRK", "LUK", "JHN", "ACT", "ROM", "1CO",
			"2CO", "GAL", "EPH", "PHP", "COL", "1TH", "2TH", "1TI", "2TI", "TIT",
			"PHM", "HEB", "JAS", "1PE", "2PE", "1JN", "2JN", "3JN", "JUD", "REV" };
        #endregion
        #region SMethod: static int FindBookAbbrevIndex(string sAbbrev)
        public static int FindBookAbbrevIndex(string sAbbrev)
        // Returns 0-based index
        {
            for (int i = 0; i < BookAbbrevs.Length; i++)
            {
                if (BookAbbrevs[i] == sAbbrev)
                    return i;
            }
            return -1;
        }
        #endregion
        #region Attr{g}: int BookIndex
        public int BookIndex
        {
            get
            {
                return FindBookAbbrevIndex(BookAbbrev);
            }
        }
        #endregion
        #region SMethod: string GetBookName(iBookIndex, DTranslation)
        static public string GetBookName(int iBookIndex, DTranslation translation)
        {
            if (iBookIndex >= 0 && iBookIndex < translation.BookNamesTable.Length)
                return translation.BookNamesTable[iBookIndex];
            return "";
        }
        #endregion
        #region SMethod: string GetBookName(sAbbrev, DTranslation)
        static public string GetBookName(string sAbbrev, DTranslation translation)
        {
            int iBookIndex = FindBookAbbrevIndex(sAbbrev);
            return GetBookName(iBookIndex, translation);
        }
        #endregion
        #region VAttr{g}: string BookName - looks up the book's name from the table
        public string BookName
        {
            get
            {
                return GetBookName(BookIndex, Translation);
            }
        }
        #endregion
        #region Attr{g}: bool IsOldTestamentBook
        public bool IsOldTestamentBook
        {
            get
            {
                if (BookIndex < c_iMatthew)
                    return true;
                return false;
            }
        }
        #endregion
        #region SMethod: bool GetIsOldTestamentBook(sBookAbbrev)
        static public bool GetIsOldTestamentBook(string sBookAbbrev)
        {
            if (string.IsNullOrEmpty(sBookAbbrev))
                return false;

            var i = DBook.FindBookAbbrevIndex(sBookAbbrev);
            if (-1 == i)
                return false;

            return (i < c_iMatthew);
        }
        #endregion
        #region SMethod: bool GetIsNewTestamentBook(sBookAbbrev)
        static public bool GetIsNewTestamentBook(string sBookAbbrev)
        {
            if (string.IsNullOrEmpty(sBookAbbrev))
                return false;

            var i = DBook.FindBookAbbrevIndex(sBookAbbrev);
            if (-1 == i)
                return false;

            return (i >= c_iMatthew);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor() - only for reading from xml
        public DBook()
            : base()
        {
            j_osSections = new JOwnSeq<DSection>("Sections", this, true, false);
            j_osNotes = new JOwnSeq<DParagraph>("Notes", this, false, false);

            // Book History
            j_ownHistory = new JOwn<TranslatorNote>("History", this);
            History = new TranslatorNote(TranslatorNote.History);

            // Book Statistics
            m_BookStats = new Statistics(this);
        }
        #endregion
        #region Constructor(sBookAbbrev)
        public DBook(string sBookAbbrev)
            : this()
        {
            // Initial values for some attrs
            BookAbbrev = sBookAbbrev;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            DBook book = (DBook)obj;
            return book.BookAbbrev == this.BookAbbrev;
        }
        #endregion
        #region VAttr{g}: override string DefaultFileExtension
        public override string DefaultFileExtension
        {
            get
            {
                return ".db";
            }
        }
        #endregion
        #region Attr(g): string SortKey - overridden to enable JOwnSeq Find method support.
        public override string SortKey
        // In order to support sorting, the subclass must implement a SortKey attribute,
        // and this SortKey must return something other than an empty string. 
        {
            get
            {
                // Return this as two digits, so that, e.g., LEV is 02 and thus sorts
                // properly after Exodus rather than after Proverbs.
                return BookNumber.ToString("00");
            }
        }
        #endregion
        #region IComparable<DBook>.CompareTo(DBook other)
        int IComparable<DBook>.CompareTo(DBook other)
        {
            return FindBookAbbrevIndex(BookAbbrev).CompareTo(FindBookAbbrevIndex(other.BookAbbrev));
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void InitializeFromFrontTranslation()
        public bool InitializeFromFrontTranslation(IProgressIndicator progress)
        {
            if (Sections.Count > 0)
                return false;
            if (null == FrontBook)
                return false;

            FrontBook.LoadBook(progress);
            if (!FrontBook.Loaded)
                return false;

            foreach (DSection sfront in FrontBook.Sections)
            {
                DSection starget = new DSection();
                Sections.Append(starget);
                starget.InitializeFromFrontSection(sfront);
            }

            // Set the ID line
            ID = BookAbbrev;
            ID += " - ";
            ID += Translation.DisplayName;

            m_bIsLoaded = true;
            DeclareDirty();
            return true;
        }
        #endregion
        #region Method: void UpdateFromFront(DBook BFront)
        public void UpdateFromFront(DBook BFront)
        {
            // If we don't have a Front book, or if we are not the TargetTranslation,
            // then we have nothing to do here.
            if (null == BFront || Translation != DB.Project.TargetTranslation)
                return;

            // If the two books do not have the same section count, then we don't dare
            // proceed.
            if (BFront.Sections.Count != Sections.Count)
                return;

            // Proceed through each DSection
            for (int i = 0; i < Sections.Count; i++)
            {
                DSection sTarget = Sections[i] as DSection;
                DSection sFront = BFront.Sections[i] as DSection;

                sTarget.UpdateFromFront(sFront);
            }
        }
        #endregion

        #region Method: string GetSectionMiscountMsg(SFront, STarget)
        string GetSectionMiscountMsg(DSection SFront, DSection STarget)
        {
            string sDiagnosis = "";

            // Diagnosis where the sections have different verses
            if (null != SFront && null != STarget)
            {
                string sBase = Loc.GetStructureMessages("diagnosis1",
                    "The verses first get off at Section {0}.");

                sDiagnosis = LocDB.Insert(sBase,
                    new string[] { SFront.ReferenceSpan.Start.FullName });
            }

            // Diagnosis where there there are sections added at the end
            else
            {
                string sBase = Loc.GetStructureMessages("diagnosis2",
                    "{0} has extra sections appended at the end.");

                string sName = (null == SFront) ?
                    STarget.Translation.DisplayName :
                    SFront.Translation.DisplayName;

                sDiagnosis = LocDB.Insert(sBase, new string[] { sName });
            }

            // Retrieve the base string
            string sInner = Loc.GetStructureMessages("msgSectionMiscount",
                "The two books appear to not have the same number of sections.\n" +
                "    {0} has {1}; {2} appears to have {3}.\n" +
                "{4}");

            // Do the insertions
            string sMsg = LocDB.Insert(sInner, new string[] { 
                SFront.Translation.DisplayName,
                SFront.Book.Sections.Count.ToString(),
                STarget.Translation.DisplayName,
                STarget.Book.Sections.Count.ToString(),
                sDiagnosis
            });
            return sMsg;
        }
        #endregion
        #region Method: string GetSpanMismatchMsg(iSection, SFront, STarget)
        string GetSpanMismatchMsg(int iSection, DSection SFront, DSection STarget)
        {
            string sBase = Loc.GetStructureMessages("msgSpanMismatch",
                "Section {0} has different verses.\n" +
                "  {1} is {2}\n" +
                "  {3} is {4}");

            string sMsg = LocDB.Insert(sBase, new string[] {
                (iSection + 1).ToString(),
                SFront.Book.Translation.DisplayName,
                SFront.ReferenceSpan.DisplayName,
                STarget.Book.Translation.DisplayName,
                STarget.ReferenceSpan.DisplayName }
                );

            return sMsg;
        }
        #endregion
        #region Method: string GetStructureMismatchMsg(DSection SFront, DSection STarget)
        string GetStructureMismatchMsg(DSection SFront, DSection STarget)
        {
            // Retrieve the Section Stucture localizations
            string sParagraph = Loc.GetStructureMessages("paragraphs", "Paragraph(s)");
            string sPicture = Loc.GetStructureMessages("picture", "Picture");

            // Build the strings representing the section structures
            string sStructFront = "";
            foreach (char ch in SFront.SectionStructure)
            {
                if (sStructFront.Length > 0)
                    sStructFront += "-";
                if (ch == DSection.c_Text)
                    sStructFront += sParagraph;
                else
                    sStructFront += sPicture;
            }
            string sStructTarget = "";
            foreach (char ch in STarget.SectionStructure)
            {
                if (sStructTarget.Length > 0)
                    sStructTarget += "-";
                if (ch == DSection.c_Text)
                    sStructTarget += sParagraph;
                else
                    sStructTarget += sPicture;
            }

            // Retrieve the message base string
            string sBase = Loc.GetStructureMessages("msgStructureMismatch",
                "There is a structure mistmatch in section {0}:\n" +
                "     {1} has {2},\n" +
                "     {3} has {4}.\n\n");

            // Make the substitutions
            string sMsg = LocDB.Insert(sBase, new string[] {
                STarget.ReferenceSpan.DisplayName, 
                SFront.Translation.DisplayName,
                sStructFront,
                STarget.Translation.DisplayName,
                sStructTarget
            });
            return sMsg;
        }
        #endregion

        #region Method: bool CheckSectionStructureAgainstFront(DBook BFront)
        #region Method: int GetFirstVerseInSection(DSection section)
        int GetFirstVerseInSection(DSection section)
        {
            foreach (DParagraph p in section.Paragraphs)
            {
                foreach (DRun run in p.Runs)
                {
                    if (null != run as DVerse)
                        return (run as DVerse).VerseNo;
                }
            }
            return 0;
        }
        #endregion
        #region Method: int GetLastVerseInSection(DSection section)
        int GetLastVerseInSection(DSection section)
        {
            return section.ReferenceSpan.End.Verse;

            // I originally was trying the mirror of GetFirstVerse, but it doesn't
            // work with verse bridges, so this is more accurage for the tail-end
            // of a section.
            /*
            int nVerse = 0;

            foreach (DParagraph p in section.Paragraphs)
            {
                foreach (DRun run in p.Runs)
                {
                    if (null != run as DVerse)
                        nVerse = (run as DVerse).VerseNo;
                }
            }

            return nVerse;
            */
        }
        #endregion

        public void CheckSectionStructureAgainstFront(string sTargetPath, DBook BFront)
        {
            // If we don't have a Front book, or if we are not the TargetTranslation,
            // then we have nothing to do here.
            if (null == BFront || Translation != DB.Project.TargetTranslation)
                return;

            // Make sure we have the same number of sections
            if (Sections.Count != BFront.Sections.Count)
            {
                // Find the sections where the counts get off
                DSection SFront = null;
                DSection STarget = null;
                for (int i = 0; i < BFront.Sections.Count && i < Sections.Count; i++)
                {
                    SFront = BFront.Sections[i] as DSection;
                    STarget = Sections[i] as DSection;
                    if (!SFront.ReferenceSpan.ContentEquals(STarget.ReferenceSpan))
                        break;
                }

                // Display the error
                throw new eBookReadException(
                    GetSectionMiscountMsg(SFront, STarget),
                    HelpSystem.Topic.kErrStructureSectionMiscount,
                    eBookReadException.GetOffendingLineNumber(BFront.StoragePath, SFront),
                    eBookReadException.GetOffendingLineNumber(sTargetPath, STarget)
                    );
            }

            // Loop through all of the sections, looking for structures that do not match
            for (int i = 0; i < Sections.Count; i++)
            {
                DSection SFront = BFront.Sections[i] as DSection;
                DSection STarget = Sections[i] as DSection;

                if (GetFirstVerseInSection(SFront) != GetFirstVerseInSection(STarget) ||
                    GetLastVerseInSection(SFront) != GetLastVerseInSection(STarget))
                {
                    throw new eBookReadException(
                        GetSpanMismatchMsg(i, SFront, STarget),
                        HelpSystem.Topic.kErrStructureSpanMismatch,
                        eBookReadException.GetOffendingLineNumber(BFront.StoragePath, SFront),
                        eBookReadException.GetOffendingLineNumber(sTargetPath, STarget)
                    );
                }

                if (SFront.SectionStructure != STarget.SectionStructure)
                {
                    throw new eBookReadException(
                        GetStructureMismatchMsg(SFront, STarget),
                        HelpSystem.Topic.kErrStructureMismatch,
                        eBookReadException.GetOffendingLineNumber(BFront.StoragePath, SFront),
                        eBookReadException.GetOffendingLineNumber(sTargetPath, STarget)
                        );
                }
            }

            return;
        }
        #endregion
        #region Method: void CombineEmptyInitialSection()
        void CombineEmptyInitialSection()
        // The Timor MRK's typically have an introductory paragraph without a section
        // (\s), and have preferred to edit these together with the next section for
        // more context when editing. So I scan the first section for an \s, and if not
        // found, combine it into the next section
        {
            if (Sections.Count < 2)
                return;

            // Check the first section and see if we find the SectionHead style
            var section1 = Sections[0];
            foreach (DParagraph p in section1.Paragraphs)
            {
                if (p.StyleAbbrev == DStyleSheet.c_sfmSectionHead)
                    return;
            }

            // Didn't find it, so combine with the next one
            var section2 = Sections[1];
            while (section2.Paragraphs.Count > 0)
            {
                DParagraph p = section2.Paragraphs[0];
                section2.Paragraphs.Remove(p);
                section1.Paragraphs.Append(p);
            }
            Sections.Remove(section2);
        }
        #endregion

        // Filters ---------------------------------------------------------------------------
        #region Method: void RemoveFilter() - Turn off any active filter
        public void RemoveFilter()
        {
            DSection.FilterIsActive = false;
            foreach (DSection section in Sections)
                section.MatchesFilter = false;
        }
        #endregion
        #region Method: int IndexOfFirstFilterMatch
        public int IndexOfFirstFilterMatch
        {
            get
            {
                if (DSection.FilterIsActive)
                {
                    for (int i = 0; i < Sections.Count; i++)
                    {
                        DSection section = Sections[i] as DSection;
                        if (section.MatchesFilter)
                            return i;
                    }
                    Debug.Assert(false, "No sections match the filter");
                }
                return 0;
            }
        }
        #endregion
        #region Method: int IndexOfLastFilterMatch
        public int IndexOfLastFilterMatch
        {
            get
            {
                if (DSection.FilterIsActive)
                {
                    for (int i = Sections.Count - 1; i >= 0; i--)
                    {
                        DSection section = Sections[i] as DSection;
                        if (section.MatchesFilter)
                            return i;
                    }
                    Debug.Assert(false, "No sections match the filter");
                }
                return Sections.Count - 1;
            }
        }
        #endregion

        // I/O (Standard Format) -------------------------------------------------------------
        #region EMBEDDED CLASS IO
        public class IO
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: DBook Book - the book we're reading/writing
            DBook Book
            {
                get
                {
                    Debug.Assert(null != m_book);
                    return m_book;
                }
            }
            private DBook m_book = null;
            #endregion
            #region Attr{g}: DSFMapping Map
            DSFMapping Map
            {
                get
                {
                    return DB.TeamSettings.SFMapping;
                }
            }
            #endregion
            #region Attr{g}: ScriptureDB DBS
            ScriptureDB DBS
            {
                get
                {
                    Debug.Assert(null != m_db);
                    return m_db;
                }
                set
                {
                    Debug.Assert(null != value);
                    m_db = value;
                }
            }
            ScriptureDB m_db = null;
            #endregion

            // Progress Indiccator -----------------------------------------------------------
            IProgressIndicator m_Progress;
            #region Method: void StartProgress(string sAction)
            private void StartProgress(string sAction, int nCount)
            {
                string sWhatIsLoading = Book.Translation.DisplayName + ": " + Book.DisplayName;

                string sMessage = sAction + " " + sWhatIsLoading + "...";

                m_Progress.Start(sMessage, nCount);
            }
            #endregion

            // ID Field ----------------------------------------------------------------------
            #region Method: bool ID_in(SfField field)
            private bool ID_in(SfField field)
            {
                if (Map.IsFileIdMarker(field.Mkr))
                {
                    Book.ID = field.Data;
                    return true;
                }
                return false;
            }
            #endregion
            #region Method: void ID_out()
            private void ID_out()
            {
                if (Book.ID.Length > 0)
                    DBS.Append(new SfField(Map.MkrFileId, Book.ID));
            }
            #endregion

            // Comment Field -----------------------------------------------------------------
            #region Method: bool Comment_in(SfField field)
            private bool Comment_in(SfField field)
            {
                if (Map.IsCommentMarker(field.Mkr))
                {
                    Book.Comment += field.Data;
                    return true;
                }
                return false;
            }
            #endregion
            #region Method: void Comment_out()
            private void Comment_out()
            {
                if (Book.Comment.Length > 0)
                    DBS.Append(new SfField(Map.MkrComment, Book.Comment));
            }
            #endregion

            // Copyright Field ---------------------------------------------------------------
            #region Method: bool Copyright_in(SfField field)
            private bool Copyright_in(SfField field)
            {
                if (Map.IsCopyrightMarker(field.Mkr))
                {
                    Book.Copyright = field.Data;
                    return true;
                }
                return false;
            }
            #endregion
            #region Method: void Copyright_out()
            private void Copyright_out()
            {
                if (Book.Copyright.Length > 0)
                    DBS.Append(new SfField(Map.MkrCopyright, Book.Copyright));
            }
            #endregion

            // History Field -----------------------------------------------------------------
            #region Method: bool History_in(SfField field)
            private bool History_in(SfField field)
            {
                // Old-style history: Import it. Since we don't know what it is, we have
                // to set the stage to Draft
                if (Map.IsBookHistoryMarker(field.Mkr))
                {
                    Book.History.AddMessage(
                        new DateTime(2009, 1, 1),
                        DB.TeamSettings.Stages.Draft,
                        field.Data);
                    return true;
                }

                // New style: interpret it
                return Book.History.ReadOldHistory(field);
            }
            #endregion
            #region Method: void History_out()
            void History_out()
            {
                if (Book.History.HasMessages)
                {
                    Book.History.AddToSfmDB(DBS);
                }
            }
            #endregion

            // Notes Field -------------------------------------------------------------------
            #region Method: bool Notes_in(SfField field)
            private bool Notes_in(SfField field)
            {
                if (Map.IsBookNotesMarker(field.Mkr))
                {
                    DParagraph p = new DParagraph();
                    Book.Notes.Append(p);
                    p.StyleAbbrev = "p";
                    p.SimpleText = field.Data;
                    return true;
                }
                return false;
            }
            #endregion
            #region Method: void Notes_out()
            void Notes_out()
            {
                foreach (DParagraph p in Book.Notes)
                    DBS.Append(new SfField(Map.MkrBookNotes, p.SimpleText));
            }
            #endregion

            // SfDb Read/Write ---------------------------------------------------------------
            #region Method: ScriptureDB _CreateDB()
            ScriptureDB _CreateDB()
            {
                // Create a SfDB to put everything into
                DBS = new ScriptureDB();

                // The first record holds the general info about the book
                DBS.Append(new SfField(DBS.RecordMkr, Book.BookAbbrev));
                ID_out();
                Comment_out();
                Copyright_out();
                History_out();
                Notes_out();

                // Each of the sections is a separate record
                foreach (DSection s in Book.Sections)
                {
                    s.WriteStandardFormat(DBS);
                    m_Progress.Step();
                }

                return DBS;
            }
            #endregion

            // Backup -------------------------------------------------------------------------
            #region Attr{g}: string PathForBAK
            string PathForBAK
            {
                get
                {
                    return Book.Translation.Project.TeamSettings.BackupFolder +
                        Book.BaseNameForBackup + ".bak";
                }
            }
            #endregion
            #region Method: void BackupToBAK()
            void BackupToBAK()
            {
                // Create the Destination Path
                string sBackupPath = PathForBAK;

                if (File.Exists(Book.StoragePath))
                {
                    try
                    {
                        if (File.Exists(sBackupPath))
                            File.Delete(sBackupPath);
                        File.Move(Book.StoragePath, sBackupPath);

                        BackupSystem.CleanUpOldFiles(
                            Book.Translation.Project.TeamSettings.BackupFolder,
                            PathForBAK);
                    }
                    catch (Exception)
                    { }
                }
            }
            #endregion
            #region Method: void RestoreFromBAK()
            void RestoreFromBAK()
            {
                try
                {
                    // Give time for the OS to finish any cleanup it is doing
                    Thread.Sleep(1000);

                    // Copy the backup onto the filename (provided one exists, of course)
                    if (File.Exists(PathForBAK))
                        File.Copy(PathForBAK, Book.StoragePath, true);
                }
                catch (Exception)
                { }
            }
            #endregion

            // Public Operations -------------------------------------------------------------
            #region Constructor()
            public IO(DBook book, IProgressIndicator progress)
            {
                m_book = book;
                m_Progress = progress;
            }
            #endregion
            #region Method: bool Read(TextReader)
            public bool Read(TextReader tr)
            {
                // We'll keep track of the current reference for the benefit of error messages
                DSection.IO.ResetChapterVerse();

                // Read the SF file into a SfDb object (which will do standard transformations)
                DBS = new ScriptureDB();
                if (!DBS.Read(tr))
                    return false;
                DBS.TransformIn();

                // Setuo Progress indicator(s)
                StartProgress("Reading", DBS.RecordCount);

                // Look to read the first record, which is general information about the entire book.
                DBS.AdvanceToFirstRecord();
                if (null == DBS.GetCurrentField())
                    return false;
                if (DBS.CurrentFieldIsRecordMarker)
                    Book.SfRecordKey = DBS.GetCurrentField().Data;
                Book.Comment = "";
                while (null != DBS.GetNextField())
                {
                    // Get the new field; exit the loop if we've reached the next record
                    if (DBS.CurrentFieldIsRecordMarker)
                        break;
                    SfField field = DBS.GetCurrentField();

                    // Process the field according to its marker
                    if (ID_in(field))
                        continue;
                    if (Comment_in(field))
                        continue;
                    if (History_in(field))
                        continue;
                    if (Notes_in(field))
                        continue;
                    if (Copyright_in(field))
                        continue;
                }

                // Read in the sections
                while (null != DBS.GetCurrentField())
                {
                    DSection section = new DSection();
                    section.LineNoInFile = DBS.GetCurrentField().LineNo;
                    Book.Sections.Append(section);

                    section.ReadStandardFormat(DBS);

                    m_Progress.Step();
                }

                // Post Processing (calc versification, check structure against front)
                Book._LoadPostProcessing(null);

                // Done
                return true;
            }
            #endregion
            #region Method: bool Write()
            public bool Write()
            {
                StartProgress("Saving", Book.Sections.Count);

                DBS = _CreateDB();
                DBS.Format = ScriptureDB.Formats.kToolbox;

                // Create a backup file in case writing fails
                BackupToBAK();

                // Write out the DB
                bool bSuccessful = DBS.Write(Book.StoragePath);

                // If we failed, restore from our backup
                if (!bSuccessful)
                {
                    RestoreFromBAK();
                    Book.IsDirty = true;  // Still needs to be saved
                    m_Progress.End();
                    return false;
                }

                Book.IsDirty = false;
                m_Progress.End();
                return true;
            }
            #endregion
            #region Method: void ExportToParatext(sPathName)
            public void ExportToParatext(string sPathName)
            {
                StartProgress("Exporting", Book.Sections.Count);

                DBS = _CreateDB();
                DBS.Format = ScriptureDB.Formats.kParatext;

                DBS.Write(sPathName);

                m_Progress.End();
            }
            #endregion
            #region Method: void ExportToGoBible(sPathName)
            public void ExportToGoBible(string sPathName)
            {
                StartProgress("Exporting", Book.Sections.Count);

                DBS = _CreateDB();
                DBS.Format = ScriptureDB.Formats.kGoBibleCreator;

                DBS.Write(sPathName);

                m_Progress.End();
            }
            #endregion
            #region Method: void ExportToToolbox(sPathName) - (used in ChorusMerge)
            public void ExportToToolbox(string sPathName)
            {
                StartProgress("Exporting", Book.Sections.Count);

                DBS = _CreateDB();
                DBS.Format = ScriptureDB.Formats.kToolbox;

                DBS.Write(sPathName);

                m_Progress.End();
            }
            #endregion
        } // End: Embedded Class IO
        #endregion
        #region Method: void ExportToParatext(sPathName, IProgressIndicator)
        public void ExportToParatext(string sPathName, IProgressIndicator progress)
        {
            (new IO(this, progress)).ExportToParatext(sPathName);
        }
        #endregion
        #region Method: void ExportToGoBible(sPathName, IProgressIndicator)
        public void ExportToGoBible(string sPathName, IProgressIndicator progress)
        {
            (new IO(this, progress)).ExportToGoBible(sPathName);
        }
        #endregion
        #region Method: void ExportToToolbox(sPathName, IProgressIndicator)
        public void ExportToToolbox(string sPathName, IProgressIndicator progress)
        {
            (new IO(this, progress)).ExportToToolbox(sPathName);
        }
        #endregion
        #region Method: bool LoadFromStandardFormat(sPath, progress)
        bool LoadFromStandardFormat(string sPath, IProgressIndicator progress)
        {
            // Locate the file
            if (!File.Exists(sPath))
            {
                LocDB.Message("UnableToFindFile",
                    "Unable to find the file:\n{0}.",
                    new string[] { sPath },
                    LocDB.MessageTypes.Error);
                return false;
            }

            // Open the file
            bool bSfmLoaded = false;
            while (!bSfmLoaded)
            {
                TextReader tr = null;
                try
                {
                    var sr = new StreamReader(sPath, Encoding.UTF8);
                    tr = TextReader.Synchronized(sr);

                    IO io = new IO(this, progress);
                    if (true == io.Read(tr))
                    {
                        IsDirty = false;  // We'll consider an object unchanged following a read. 
                        bSfmLoaded = true;
                    }
                }
                catch (eBookReadException bre)
                {
                    // Decide which version of the Repair dialog to show
                    DialogRepairImportBook dlgRepair = (bre.NeedToShowFront) ?
                        new RepairImportBookStructure(FrontBook, this, sPath, bre) :
                        new DialogRepairImportBook(this, sPath, bre);

                    // Give the user the opportunity to fix the problem
                    DialogResult result = dlgRepair.ShowDialog();

                    // If he doesn't fix it, then we must remove the book from
                    // the project. (Thus we return "false")
                    if (DialogResult.Cancel == result)
                        return false;

                    // Prepare to try the import again. 
                    Clear();
                    Debug.Assert(!History.HasMessages);
                }
                catch (Exception e)
                {
                    LocDB.Message("cantLoadBookSystemException",
                        "Unable to load book {0} due to system message:\n\n{1}",
                        new string[] { e.Message },
                        LocDB.MessageTypes.Error);
                    Console.WriteLine("Exception in DBook.Read: " + e.Message);
                    return false;
                }
                finally
                {
                    if (null != tr)
                    {
                        tr.Close();
                        tr = null;
                    }
                    progress.End();
                }
            }

            m_bIsLoaded = true;
            return true;
        }
        #endregion

        // I/O (Oxes) ------------------------------------------------------------------------
        #region Constants
        const string c_sTagBible = "bible";
        const string c_sAttrOxes = "oxes";

        const string c_sTagBook = "book";
        const string c_sAttrID = "id";
        const string c_sAttrStage = "stage";
        const string c_sAttrVersion = "version";
        const string c_sAttrLocked = "locked";
        const string c_sAttrCopyright = "copyright";
        const string c_sAttrComment = "comment";
        #endregion

        #region Attr{g}: XmlDoc ToOxesDocument
        public XmlDoc ToOxesDocument
        {
            get
            {
                var oxes = new XmlDoc();
                oxes.AddXmlDeclaration();

                // Bible Node
                var nodeBible = oxes.AddNode(null, c_sTagBible);
                oxes.AddAttr(nodeBible, "xml:lang", "bko");
                oxes.AddAttr(nodeBible, "backtTranslaltionDefaultLanguage", "en");
                oxes.AddAttr(nodeBible, c_sAttrOxes, "2.0");

                // Book Node
                var nodeBook = oxes.AddNode(nodeBible, c_sTagBook);
                oxes.AddAttr(nodeBook, c_sAttrID, BookAbbrev);
                oxes.AddAttr(nodeBook, c_sAttrStage, Stage.EnglishAbbrev);
                oxes.AddAttr(nodeBook, c_sAttrVersion, Version);
                oxes.AddAttr(nodeBook, c_sAttrLocked, Locked);
                oxes.AddAttr(nodeBook, c_sAttrCopyright, Copyright);
                oxes.AddAttr(nodeBook, c_sAttrComment, Comment);

                // Book History, saved prior to the sections
                History.Save(oxes, nodeBook);

                // Add the Sections
                foreach (DSection section in Sections)
                    section.SaveToOxesBook(oxes, nodeBook);

                return oxes;
            }
        }
        #endregion
        #region SMethod: string GetAbbrevFromOxes(XmlDoc oxes)
        static public string GetAbbrevFromOxes(XmlDoc oxes)
        {
            var nodeBible = XmlDoc.FindNode(oxes, c_sTagBible);
            if (null == nodeBible)
                return null;

            var nodeBook = XmlDoc.FindNode(nodeBible, c_sTagBook);
            if (null == nodeBook)
                return null;

            return XmlDoc.GetAttrValue(nodeBook, c_sAttrID, "");
        }
        #endregion
        #region Method: void QuicklyReadBasicAttrs()
        public void QuicklyReadBasicAttrs()
        {
            TextReader tr = null;
            try
            {
                var sr = new StreamReader(StoragePath, Encoding.UTF8);
                tr = TextReader.Synchronized(sr);

                do
                {
                    string s = tr.ReadLine();
                    if (null == s)
                        break;
                    s = s.Trim();

                    if (!s.StartsWith("<" + c_sTagBook + " "))
                        continue;

                    // Create an endtag so we can parse it legally
                    s = s.Replace(">", " />");

                    // Parse it in
                    var xml = new XmlDoc(s);
                    var xmlBook = xml.FirstChild;

                    // Interpret the basic attrs
                    Locked = XmlDoc.GetAttrValue(xmlBook, c_sAttrLocked, false);
                    string sStage = XmlDoc.GetAttrValue(xmlBook, c_sAttrStage, Stage.c_sDraft);
                    Stage = DB.TeamSettings.Stages.FromOxesAttr(sStage);

                    // Done reading the file
                    break;

                } while (true);

            }
            catch (Exception e)
            {
                Console.WriteLine("Reading basic attrs failed: " + e.Message);
            }
            finally
            {
                if (null != tr)
                    tr.Close();
            }
        }
        #endregion

        #region Method: bool LoadFromOxes(string sPath, IProgressIndicator progress)

        public void LoadFromOxes(string sPath, IProgressIndicator progress)
        {
            while (_LoadFromOxes(sPath, progress))
                ;
        }

        public bool _LoadFromOxes(string sPath, IProgressIndicator progress)
        // Returns true if the user wishes to try again following making corrections in the
        // Repair dialog, false otherwise.
        {
            m_bIsLoaded = false;
            var oxes = new XmlDoc();

            try
            {
                // Read in the xml file via the dotnet system
                Clear();
                oxes.Load(sPath);

                // Find the Bible node (If well-formed Oxes, shouldn't be a problem.)
                var nodeBible = XmlDoc.FindNode(oxes, c_sTagBible);
                if (null == nodeBible)
                {
                    throw new eBookReadException(
                        Loc.GetMessages("msgMissingBibleTag",
                            "The oxes file is missing a \"Bible\" tag."),
                        HelpSystem.Topic.kImportBook, 0);
                }

                // Make sure it is a version of Oxes that we handle
                string sOxes = XmlDoc.GetAttrValue(nodeBible, c_sAttrOxes, "");
                if (sOxes != "2.0")
                {
                    LocDB.Message("kUnsupportedOxes",
                        "This version of OurWord does not support Oxes {0}.",
                        new string[] { sOxes },
                        LocDB.MessageTypes.Error);
                    return false;
                }

                // Find the Book node (If well-formed Oxes, shouldn't be a problem.)
                var nodeBook = XmlDoc.FindNode(nodeBible, c_sTagBook);
                if (null == nodeBook)
                {
                    throw new eBookReadException(
                        Loc.GetMessages("msgMissingBookTag",
                            "The oxes file is missing a \"Book\" tag."),
                        HelpSystem.Topic.kImportBook, 0);
                }

                // Verify the Book's three-letter ID is what we expect
                string sBookID = XmlDoc.GetAttrValue(nodeBook, c_sAttrID, "");
                if (string.IsNullOrEmpty(sBookID) || sBookID != BookAbbrev)
                {
                    string sBase = Loc.GetString("msgOxesImportBadAbbrev",
                        "Either the Book's Abbreviation attribute is missing, \n" +
                        "or it is not {0} as was expected.");
                    string sMessage = LocDB.Insert(sBase, new string[] { BookAbbrev });
                    throw new eBookReadException(sMessage, HelpSystem.Topic.kImportBook, 0);
                }

                // Read the Book's attributes
                string sStage = XmlDoc.GetAttrValue(nodeBook, c_sAttrStage, Stage.c_sDraft);
                Stage = DB.TeamSettings.Stages.FromOxesAttr(sStage);
                Version = XmlDoc.GetAttrValue(nodeBook, c_sAttrVersion, c_sVersionDefault);
                Locked = XmlDoc.GetAttrValue(nodeBook, c_sAttrLocked, false);
                Copyright = XmlDoc.GetAttrValue(nodeBook, c_sAttrCopyright, "");
                Comment = XmlDoc.GetAttrValue(nodeBook, c_sAttrComment, "");

                // Read in the paragraphs
                progress.Start("Reading", nodeBook.ChildNodes.Count);
                DSection section = null;
                foreach (XmlNode child in nodeBook.ChildNodes)
                {
                    progress.Step();

                    // The first child node, if a History note, would be the History for the entire
                    // book. (There will be no "section" defined yet. Otherwise, any History we
                    // encounter goes with the most recently defined section. (The only notes
                    // we should see at this level are History notes.)
                    var history = TranslatorNote.Create(child);
                    if (null != history)
                    {
                        if (history.Behavior != TranslatorNote.History)
                        {
                            throw new XmlDocException(child,
                                "A non-History Annotation was encountered at the top level of the book.");
                        }
                        if (null == section)
                            History = history;
                        else
                            section.History = history;
                        continue;
                    }

                    // Create the paragraph or picture
                    DParagraph paragraph = DPicture.CreatePicture(child);
                    if (null == paragraph)
                        paragraph = DParagraph.CreateParagraph(child);

                    // A section for it to go into
                    if (null == section || paragraph.StyleAbbrev == DStyleSheet.c_sfmSectionHead)
                    {
                        section = new DSection();
                        Sections.Append(section);
                    }

                    // Add the paragraph
                    section.Paragraphs.Append(paragraph);
                }

                // Final post processing
                _LoadPostProcessing(sPath);
            }

            #region Handle Exceptions
            // Error in the raw xml   
            catch (XmlException eXml)
            {
                progress.End();
                var bre = new eBookReadException(
                    "There is a problem in the oxes file: " + eXml.Message,
                    HelpSystem.Topic.kImportBook,
                    eXml.LineNumber);
                var dlgRepair = new DialogRepairImportBook(this, sPath, bre);
                var result = dlgRepair.ShowDialog();
                return (DialogResult.OK == result);
            }

            // Error in the interpreted xml
            catch (XmlDocException eDocXml)
            {
                progress.End();
                int iLine = eDocXml.GetProblemLineNo(sPath, oxes);
                var bre = new eBookReadException(
                    "There is a problem in the oxes file: " + eDocXml.Message,
                    HelpSystem.Topic.kImportBook,
                    iLine);
                var dlgRepair = new DialogRepairImportBook(this, sPath, bre);
                var result = dlgRepair.ShowDialog();
                return (DialogResult.OK == result);
            }

            // Error in the book's structure
            catch (eBookReadException bre)
            {
                progress.End();
                DialogRepairImportBook dlgRepair = (bre.NeedToShowFront) ?
                    new RepairImportBookStructure(FrontBook, this, sPath, bre) :
                    new DialogRepairImportBook(this, sPath, bre);
                var result = dlgRepair.ShowDialog();
                return (DialogResult.OK == result);
            }

            // System or otherwise unrepairable error
            catch (Exception e)
            {
                progress.End();
                LocDB.Message("kFailedToLoadOxesSysErr",
                    "The Oxes file {0} failed to load with system error:\n{1}.",
                    new string[] { Path.GetFileName(sPath), e.Message },
                    LocDB.MessageTypes.Error);
                return false;
            }
            #endregion

            // Successful
            progress.End();
            m_bIsLoaded = true;
            IsDirty = false;
            return false;
        }
        #endregion

        #region Method: void _LoadPostProcessing(sPath)
        void _LoadPostProcessing(string sPath)
        {
            // If the first section is missing a SectionHead, put the first two sections
            // together (e.g., Timor Mark's)
            CombineEmptyInitialSection();

            // Each paragraph needs to know what verses are in it
            int nChapter = 0;
            int nVerse = 0;
            foreach (DSection section in Sections)
                section.CalculateVersification(ref nChapter, ref nVerse);

            // Items that depend on the Front translation
            if (Translation == DB.Project.TargetTranslation && FrontBook != null)
            {
                // Make sure that each section has the same structure as the front.
                CheckSectionStructureAgainstFront(sPath, FrontBook);

                // E.g., Generate the \ref and \cf's from the Front Translation
                // We do the test here to prevent the FrontBook from attempting
                // to load itself recursively here (via an embedded Loaded call.)
                UpdateFromFront(FrontBook);
            }

        }
        #endregion
        #region Method: bool LoadBook(sPath, progress)
        public void LoadBook(string sPath, IProgressIndicator progress)
        {
            // Nothing to do if already loaded, or a bad pathname
            if (Loaded || string.IsNullOrEmpty(sPath))
                return;

            // Load according to format, which we determine based on the file extension
            if (sPath.ToLower().Contains(".oxes"))
                LoadFromOxes(sPath, progress);
            else
                LoadFromStandardFormat(sPath, progress);
        }
        #endregion
        #region Method: bool LoadBook(progress)
        public void LoadBook(IProgressIndicator progress)
        {
            LoadBook(StoragePath, progress);
        }
        #endregion

        #region Method: void WriteBook(string sPath)
        private void WriteBook(string sPath)
        {
            if (!Loaded)
                return;

            var xml = ToOxesDocument;
            xml.Save(sPath);
            IsDirty = false;
        }
        #endregion
        #region Method: void WriteBook(progress)
        public void WriteBook(IProgressIndicator progress)
        {
            if (!Loaded)
                return;
            if (!IsDirty)
                return;

            // Create the xml representation, and write it to disk
            var xml = ToOxesDocument;
            xml.Save(StoragePath);
            IsDirty = false;

            // Make the backup to the ".backup" folder
            var sBackupPath = Translation.Project.TeamSettings.BackupFolder +
                BaseNameForBackup + ".bak";
            if (File.Exists(StoragePath))
            {
                try
                {
                    if (File.Exists(sBackupPath))
                        File.Delete(sBackupPath);
                    File.Copy(StoragePath, sBackupPath, true);

                    BackupSystem.CleanUpOldFiles(
                        Translation.Project.TeamSettings.BackupFolder,
                        sBackupPath);
                }
                catch (Exception)
                { }
            }

            // Make the optional other-device backup
            (new BackupSystem(StoragePath, BaseNameForBackup)).MakeBackup();
        }
        #endregion
        #region OMethod: void WriteToFile(progress)
        public override void WriteToFile(IProgressIndicator progress)
        {
            WriteBook(progress);
        }
        #endregion

        // Read/Write Standard Format --------------------------------------------------------
        #region DReference GetDefaultReference(sData)
        private DReference GetDefaultReference(string sData)
        {
            int i = 0;

            // Skip past the book abbreviation
            while (i < sData.Length && sData[i] != ' ')
                ++i;
            while (i < sData.Length && sData[i] == ' ')
                ++i;
            while (i < sData.Length && sData[i] == '0')
                ++i;

            // Get the chapter number
            string sChapter = "";
            while (i < sData.Length && sData[i] != '.')
            {
                sChapter += sData[i];
                ++i;
            }
            while (i < sData.Length && sData[i] == '.')
                ++i;
            while (i < sData.Length && sData[i] == '0')
                ++i;

            // Get the verse number
            string sVerse = "";
            while (i < sData.Length && sData[i] != '-')
            {
                sVerse += sData[i];
                ++i;
            }

            // Build the reference
            DReference reference = new DReference();
            if (sChapter.Length > 0)
                reference.Chapter = Convert.ToInt16(sChapter);
            if (sVerse.Length > 0)
                reference.Verse = Convert.ToInt16(sVerse);
            return reference;
        }
        #endregion
        #region SMethod: void ParseFileName(...)
        static public void ParseFileName(string sPath, ref int nBookNumber,
            ref string sBookAbbrev, ref string sLanguageName, ref string sStageAbbrev,
            ref string sVersion)
        // The basename of a file name, when saved to backup, is saved as:
        // 
        {
            string s = Path.GetFileNameWithoutExtension(sPath);
            int i = 0;

            // Book Number
            nBookNumber = 0;
            string sBookNumber = "";
            while (i < s.Length && char.IsDigit(s[i]))
                sBookNumber += s[i++];
            try
            {
                if (!string.IsNullOrEmpty(sBookNumber))
                    nBookNumber = Convert.ToInt16(sBookNumber);
            }
            catch (Exception) { }
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;

            // Book Abbrev
            sBookAbbrev = "";
            while (i < s.Length && !char.IsWhiteSpace(s[i]))
                sBookAbbrev += s[i++];
            while (i < s.Length && (char.IsWhiteSpace(s[i]) || s[i] == '-'))
                i++;

            // Language Name
            sLanguageName = "";
            while (i < s.Length && !char.IsWhiteSpace(s[i]))
                sLanguageName += s[i++];
            while (i < s.Length && (char.IsWhiteSpace(s[i]) || s[i] == '-'))
                i++;

            // Stage Abbrev
            sStageAbbrev = "";
            while (i < s.Length && !char.IsWhiteSpace(s[i]) && s[i] != '-')
                sStageAbbrev += s[i++];
            while (i < s.Length && (char.IsWhiteSpace(s[i]) || s[i] == '-'))
                i++;

            // Version
            sVersion = "";
            while (i < s.Length && !char.IsWhiteSpace(s[i]) && s[i] != '-')
                sVersion += s[i++];
            while (i < s.Length && (char.IsWhiteSpace(s[i]) || s[i] == '-'))
                i++;
        }
        #endregion
        #region Method: static void RestoreFromBackup(DBook book, string sBackupPathName)
        static public void RestoreFromBackup(DBook book, string sBackupPathName, IProgressIndicator progress)
        {
            // Post a status message
            progress.Start("Restoring from backup...", 0);

            // Unload the current book from memory. We call Clear() even though Loaded=false does this,
            // because from the test code it isn't activated (no harm done to have
            // it execute twice.)
            book.Unload(new NullProgress());
            book.Clear();
            Debug.Assert(0 == book.Sections.Count);

            // Rename it (to *.sav). The ability to use this will not be in the
            // user interface, but it may come in handy for computer techies.
            JW_Util.CreateBackup(book.StoragePath, ".sav");

            // Copy over the backup file and give it the correct name (strip the date off)
            File.Copy(sBackupPathName, book.StoragePath, true);

            // Update the Stage/Version Info. Here we must parse out the information from the
            // backup file name.
            int nBookNumber = 0;
            string sBookAbbrev = "";
            string sLanguageName = "";
            string sStageAbbrev = "";
            string sVersion = "";
            DBook.ParseFileName(sBackupPathName, ref nBookNumber, ref sBookAbbrev,
                ref sLanguageName, ref sStageAbbrev, ref sVersion);
            book.Stage = DB.TeamSettings.Stages.Find(sStageAbbrev);
            book.Version = sVersion;

            // Load the book back in
            book.LoadBook(progress);
            progress.End();

            // Calling method (command handler) is responsible to navigate to the
            // first section and put everything into the cache, etc.
        }
        #endregion

        // Book Statisics --------------------------------------------------------------------
        #region Class: Statistics
        public class Statistics
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: Dictionary<string, double> DictStatistics
            public Dictionary<string, double> DictStatistics
            {
                get
                {
                    Debug.Assert(null != m_dictStatistics);
                    return m_dictStatistics;
                }
            }
            Dictionary<string, double> m_dictStatistics;
            #endregion
            #region Attr{g}: DBook Book
            DBook Book
            {
                get
                {
                    Debug.Assert(null != m_Book);
                    return m_Book;
                }
            }
            DBook m_Book;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(DBook)
            public Statistics(DBook book)
            {
                m_Book = book;
                m_dictStatistics = new Dictionary<string, double>();
            }
            #endregion
            #region Method: void SetValue(stage, dValue)
            public void SetValue(Stage stage, double dValue)
            {
                if (DictStatistics.ContainsKey(stage.EnglishAbbrev))
                    DictStatistics.Remove(stage.EnglishAbbrev);

                DictStatistics.Add(stage.EnglishAbbrev, dValue);
            }
            #endregion
            #region Method:  double GetValue(stage)
            public double GetValue(Stage stage)
            {
                if (null == stage)
                    return 0;

                double d;
                if (DictStatistics.TryGetValue(stage.EnglishAbbrev, out d))
                    return d;
                return 0;
            }
            #endregion

            // Get / Compute -----------------------------------------------------------------
            #region Method: void ComputePercentDrafted()
            public void ComputePercentDrafted()
            // Computes the percentage of DTexts which have drafted (and BT'd) text.
            // Sets these into the dictionary, and returns whichever figure was
            // asked for by the bool bBT.
            {
                if (!Book.Loaded)
                    Book.LoadBook(new NullProgress());

                int cTexts = 0;
                int cTextsWithContent = 0;
                int cTextWithBT = 0;

                foreach (DSection section in Book.Sections)
                {
                    foreach (DParagraph p in section.Paragraphs)
                    {
                        foreach (DRun run in p.Runs)
                        {
                            var text = run as DText;
                            if (null == text)
                                continue;

                            cTexts++;

                            if (!string.IsNullOrEmpty(text.AsString))
                                cTextsWithContent++;
                            if (!string.IsNullOrEmpty(text.ProseBTAsString))
                                cTextWithBT++;
                        }
                    }
                }

                double dPctDraft = (double)cTextsWithContent / (double)cTexts;
                double dPctBT = (double)cTextWithBT / (double)cTexts;

                SetValue(DB.TeamSettings.Stages.Find(Stage.c_idDraft), dPctDraft);
                SetValue(DB.TeamSettings.Stages.Find(Stage.c_idBackTranslation), dPctBT);
            }
            #endregion
            #region Method: double ComputePercentComplete(stage)
            public double ComputePercentComplete(Stage stage)
            {
                if (Stage.c_idDraft == stage.ID)
                    ComputePercentDrafted();

                if (Stage.c_idBackTranslation == stage.ID)
                    ComputePercentDrafted();

                return GetValue(stage);
            }
            #endregion
            #region Method: double GetPercentComplete(stage)
            public double GetPercentComplete(Stage stage)
            {
                // Return it if we already have it
                double d;
                if (DictStatistics.TryGetValue(stage.EnglishAbbrev, out d))
                    return d;

                //  Compute it otherwise
                return ComputePercentComplete(stage);
            }
            #endregion

            // I/O ---------------------------------------------------------------------------
            #region Method: string ToSaveString()
            public string ToSaveString()
            {
                var ua = new UrlAttrList();

                foreach (KeyValuePair<string, double> pair in DictStatistics)
                {
                    string sEnglishAbbrev = pair.Key;
                    double dPct = pair.Value;

                    ua.Add(new UrlAttr(sEnglishAbbrev, dPct.ToString("0.00")));
                }

                return ua.MakeUrl();
            }
            #endregion
            #region Method: void FromSaveString(string s)
            public void FromSaveString(string s)
            {
                DictStatistics.Clear();

                var v = new UrlAttrList(s);

                foreach (UrlAttr ua in v)
                {
                    try
                    {
                        string sEnglishAbbrev = ua.Name;

                        string sPct = ua.Value;
                        double dPct = Convert.ToDouble(sPct);

                        DictStatistics.Add(sEnglishAbbrev, dPct);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            #endregion
        }
        #endregion
        #region Attr{g}: Statistics BookStats
        public Statistics BookStats
        {
            get
            {
                Debug.Assert(null != m_BookStats);
                return m_BookStats;
            }
        }
        Statistics m_BookStats;
        #endregion

        // Data Access -----------------------------------------------------------------------
        #region Method: DSection GetFirstSection() - returns this book's first section
        public DSection GetFirstSection()
        {
            return Sections.Count > 0 ? (DSection)Sections[0] : null;
        }
        #endregion
        #region Method: DSection GetSection(i) - returns the i'th section
        public DSection GetSection(int i)
        {
            if (i < 0 || i >= Sections.Count)
                return null;
            return (DSection)Sections[i];
        }
        #endregion
        #region Method: DSection[] GetSectionsContaining(DReferenceSpan span)
        public DSection[] GetSectionsContaining(DReferenceSpan span)
        {
            // For convienance we'll temporarily store the sections in an ArrayList
            ArrayList a = new ArrayList();

            // Scan for sections containing the endpoints of the span. 
            bool bFound = false;
            foreach (DSection s in Sections)
            {
                // Start adding sections once we locate the section containing the span's start
                if (s.ReferenceSpan.ContainsReference(span.Start))
                {
                    a.Add(s);
                    bFound = true;
                }

                // Once we find the section containing the span's end, we're done adding
                if (s.ReferenceSpan.ContainsReference(span.End))
                {
                    if (!a.Contains(s))
                        a.Add(s);
                    break;
                }

                // If here, we've found the start section, but not yet encountered the ending one
                if (bFound && !a.Contains(s))
                    a.Add(s);
            }

            // Convert into a vector
            DSection[] v = new DSection[a.Count];
            for (int i = 0; i < a.Count; i++)
                v[i] = a[i] as DSection;
            return v;
        }
        #endregion
        #region Method: ArrayList ExtractParagraphs(DParagraph pgFront)
        /***
		public ArrayList ExtractParagraphsQ(DParagraph pgFront)
		{
			// Destination we'll be returning
			ArrayList rgSections = new ArrayList();

			// Handle section title and cross references. If pgFront represents
			// one of these, then we find the corresponding section in this
			// book and return its section head (or cross reference) as appropriate.
			// It is possible that we will not return the corresponding 
			// paragraph if the sections are at a mismatch; but we shall wait
			// until we have some examples of this before deciding how to
			// modify the logic.
			DSFMapping sfm = OurWordMain.TeamSettings.SFMapping;
			if (sfm.StyleSection == pgFront.StyleAbbrev ||
				sfm.StyleCrossRef == pgFront.StyleAbbrev )
			{
				DReference RefStart = pgFront.Section.ReferenceSpan.Start;
				foreach(DSection s in Sections)
				{
					if (s.ReferenceSpan.ContainsReference(RefStart))
					{
						foreach(DParagraph pg in s.Paragraphs)
						{
							if (pg.StyleAbbrev == pgFront.StyleAbbrev)
							{
								rgSections.Add(pg.Contents);
								return rgSections;
							}
						}
					}
				}
				return rgSections;
			}

			// If we get here, then we are dealing with content paragraphs.
			// Retrieve the verse references that we'll be looking for.
			DReferenceSpan RefSpan = pgFront.ReferenceSpan;

			// Build a list of the section(s) that participate in the span
			int iS = 0;
			for(iS = 0; iS < SectionCount; iS++)
			{
				DSection s = (DSection)Sections[iS];
				if (s.ReferenceSpan.ContainsReference( RefSpan.Start ))
				{
					rgSections.Add(s);
					break;
				}
			}
			for( ++iS; iS < SectionCount; iS++)
			{
				DSection s = (DSection)Sections[iS];
				if (s.ReferenceSpan.ContainsReference( RefSpan.End ))
					rgSections.Add(s);
				else
					break;
			}

			// Build a list of the Paragraph Groups in these sections
			ArrayList rgParagraphs = new ArrayList();
			foreach( DSection s in rgSections )
			{
				foreach( DParagraph pg in s.Paragraphs)
					rgParagraphs.Add(pg);
			}

			// Build a list of paragraphs from the PG's that contain the RefSpan
			ArrayList rgParas = new ArrayList();
			foreach( DParagraph pg in rgParagraphs)
			{
				if ( pg.ReferenceSpan.ContainsReference( RefSpan.Start ) ||
					pg.ReferenceSpan.ContainsReference( RefSpan.End ) )
				{
					rgParas.Add(pg.Contents);
				}
			}
			return rgParas;
		}
		****/
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Attr{g}: string BaseName
        public string BaseName
        // "01 GEN - Amarasi"
        {
            get
            {
                // Start with the book's index, so that they sort properly in the folder
                int iBook = BookIndex;
                string sBaseName = (iBook + 1).ToString("00");

                // Add the book's code/abbreviation, (e.g., "MRK")
                sBaseName += (" " + BookAbbrev);

                // Add the language name, (e.g., "Amarasi")
                sBaseName += (" - " + Translation.DisplayName);

                return sBaseName;
            }
        }
        #endregion
        #region OAttr{g}: string StoragePath
        public override string StoragePath
        {
            get
            {
                // The folder is defined by the translation, e.g., 
                // ...My Documents\Timor\Ndao\
                string sPath = Translation.BookStorageFolder;
                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);

                // Add the base name
                sPath += BaseName;

                // Add the extension
                sPath += ".oxes";

                return sPath;
            }
        }
        #endregion
        #region SMethod: string[] GetInfoFromPath(string sBookPath)
        static public string[] GetInfoFromPath(string sBookPath)
        {
            if (string.IsNullOrEmpty(sBookPath))
                return null;

            // Drill down to the basename
            var sBaseName = Path.GetFileNameWithoutExtension(sBookPath);
            if (string.IsNullOrEmpty(sBaseName))
                return null;

            // A well-formed name has at least four parts (including the hyphen);
            // with the final parts being the language name
            var vsParts = sBaseName.Split(new char[] { ' ' });
            if (vsParts.Length < 4)
                return null;

            // Book Number should be two digits
            var sBookNumber = vsParts[0];
            if (sBookNumber.Length != 2)
                return null;
            foreach (char ch in sBookNumber)
            {
                if (!char.IsDigit(ch))
                    return null;
            }

            // Abbreviation should be three letters; make sure they're uppercase
            var sBookAbbrev = vsParts[1];
            if (sBookAbbrev.Length != 3)
                return null;
            foreach (char ch in sBookAbbrev)
            {
                if (!char.IsLetterOrDigit(ch))
                    return null;
            }
            sBookAbbrev = sBookAbbrev.ToUpper();

            // Number and Abbreviation should match (and Abbreviation should be something
            // we recognize.
            try
            {
                // The number as stored in the filename (1-based)
                string sNumber = sBookNumber;
                if (sNumber[0] == '0')
                    sNumber = sNumber.Substring(1);

                int n = Convert.ToInt16(sNumber);

                // The index is 0-based, so we add 1 to it
                int nAbbrev = FindBookAbbrevIndex(sBookAbbrev) + 1;

                // Compare
                if (n != nAbbrev)
                    return null;
            }
            catch (Exception)
            {
            }

            // Assemble the language name; should be non-empty
            string sLanguageName = "";
            for (int i = 3; i < vsParts.Length; i++)
                sLanguageName += (vsParts[i] + ' ');
            sLanguageName = sLanguageName.Trim();
            if (string.IsNullOrEmpty(sLanguageName))
                return null;

            // Return the answer
            var vs = new string[3];
            vs[0] = sBookNumber;
            vs[1] = sBookAbbrev;
            vs[2] = sLanguageName;
            return vs;
        }
        #endregion

        // Merging ---------------------------------------------------------------------------
        #region static DBook CreateAndReadBook(sAbbrevation, sBookName, sPath)
        static DBook CreateAndReadBook(string abbreviation, string bookName, string bookPath)
        {
            // Create and add a translation
            var translation = new DTranslation(bookName);
            DB.Project.OtherTranslations.Append(translation);

            // Create and add a book. We need it to be in the hierarchy before we start the Load.
            var book = new DBook(abbreviation);
            translation.AddBook(book);

            // Read in the book
            book.LoadBook(bookPath, new NullProgress());
            return book;
        }
        #endregion
        #region static void Merge(MergeOrder)
        static public void Merge(MergeOrder mergeOrder)
        {
            // Debugging support
            //Debug.Fail("DBook.Merge Break for Debugging");
            // This hook allows a merge to fail for the benefit of unit testing
            if (mergeOrder.pathToOurs.Contains(".MustFail."))
                throw new Exception("Merge Failed for Unit Testing");

            if (mergeOrder == null) throw new ArgumentNullException("mergeOrder");

            // Initializations (dependencies we should get rid of someday)
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";
            LocDB.Initialize(JWU.GetApplicationDataFolder("OurWord"));

            // Create an owning project, because currently stuff depends on having it
            DB.Project = new DProject("Merge") {TeamSettings = new DTeamSettings("Merge")};
            DB.Project.TeamSettings.EnsureInitialized();

            // Extract the book's Abbrev from the path
            var sBaseFileName = Path.GetFileNameWithoutExtension(mergeOrder.pathToOurs);
            var sBookAbbrev = sBaseFileName.Substring(3, 3);

            // Read in the three versions of the book
            var ourBook = CreateAndReadBook(sBookAbbrev, "Ours", mergeOrder.pathToOurs);
            var theirBook = CreateAndReadBook(sBookAbbrev, "Theirs", mergeOrder.pathToTheirs);
            var parentBook = CreateAndReadBook(sBookAbbrev, "Parent", mergeOrder.pathToCommonAncestor);

            // At this point we must assume the same number of sections; if this differs
            // then our's wins and everything in theirs is tossed.
            if (ourBook.Sections.Count != parentBook.Sections.Count)
                return;
            if (ourBook.Sections.Count != theirBook.Sections.Count)
                return;

            // Merge the attributes. 
            // If we differ from Parent, then keep Ours, otherwise keep Theirs. Or put another
            // way, if we equal the parent, then we keep theirs, assuming either theirs has
            // changed, or neither has changed. OTOH, if we are different from the parent then
            // this logic keeps ours, which means that if both have made changes, Ours wins.
            if (ourBook.Comment == parentBook.Comment)
                ourBook.Comment = theirBook.Comment;

            if (ourBook.Version == parentBook.Version)
                ourBook.Version = theirBook.Version;

            if (ourBook.Copyright == parentBook.Copyright)
                ourBook.Copyright = theirBook.Copyright;

            if (ourBook.Locked == parentBook.Locked)
                ourBook.Locked = theirBook.Locked;

            if (ourBook.ID == parentBook.ID)
                ourBook.ID = theirBook.ID;

            if (ourBook.Stage == parentBook.Stage)
                ourBook.Stage = theirBook.Stage;

            // Merge the histories
            ourBook.History.Merge(parentBook.History, theirBook.History);

            // Merge the sections
            for (var i = 0; i < ourBook.Sections.Count; i++)
                ourBook.Sections[i].Merge(parentBook.Sections[i], theirBook.Sections[i]);

            ourBook.WriteBook(mergeOrder.pathToOurs);
        }
        #endregion
    }
}