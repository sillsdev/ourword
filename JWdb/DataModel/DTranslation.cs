/**********************************************************************************************
 * Project: Our Word!
 * File:    DTranslation.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: The top-level Scripture Translation object
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using JWTools;
using JWdb;
#endregion
#region Documentation - PropertiesFile
/*     \root "C:\Documents and Settings\Wimbish\My Documents\Data\Timor\Kupang\Terjemah\"
 *     \book MRK "KM-Done\PB\Kml-MrkLc.db"
 *     \book ACT "KM-Done\PB\Kml-ActKb.db"
 *     \book 2PE "KM-Work\Kml-2PeB.db"
 */
#endregion
#region Features Implemented for DTranslationProperties
/* Features implemented:
 * 
 * - OK complains if there is a zero-length filename
 * 
 * - OK complains if there is a zero-length Translation Name
 * 
 * - Double clicking on a listview row brings up properties for that book.
 * 
 * - The Title Bar shows the translation name, e.g., "Kupang Properties"; this changes when the 
 *   TranslationName field is edited.
 * 
 * - The Remove button asks the user to verify his intention, then removes the book from
 *   the list (but not from the disk)
 */
#endregion

namespace JWdb.DataModel
{
	public class DTranslation : JObjectOnDemand
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string VernacularWritingSystemName - the WS's name
		public string VernacularWritingSystemName
		{
			get
			{
				return m_sVernacularWritingSystemName;
			}
			set
			{
                SetValue(ref m_sVernacularWritingSystemName, value);
			}
		}
		private string m_sVernacularWritingSystemName;
		#endregion
		#region BAttr{g/s}: string ConsultantWritingSystemName - the WS's name
		public string ConsultantWritingSystemName
		{
			get
			{
				return m_sConsultantWritingSystemName;
			}
			set
			{
                SetValue(ref m_sConsultantWritingSystemName, value);
			}
		}
		private string m_sConsultantWritingSystemName;
		#endregion
		#region BAttr{g/s}: string Comment - a miscellaneous note about the project in general
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
		private string m_sComment;
		#endregion
		#region BAttr{g}: BStringArray BookNamesTable - Lists the 66 books
		public BStringArray BookNamesTable
		{
			get
			{
				return m_bsaBookNamesTable;
			}
		}
		public BStringArray m_bsaBookNamesTable = null;
		#endregion

        #region BAttr{g/s}: FootnoteSequenceTypes FootnoteSequenceType
        public DFoot.FootnoteSequenceTypes FootnoteSequenceType
        {
            get
            {
                return (DFoot.FootnoteSequenceTypes)m_nFootnoteSequence;
            }
            set
            {
                SetValue(ref m_nFootnoteSequence, (int)value);
            }
        }
        int m_nFootnoteSequence = (int)DFoot.FootnoteSequenceTypes.abc;
        #endregion
        #region BAttr{g}:   BStringArray FootnoteCustomSeq
        public BStringArray FootnoteCustomSeq
        {
            get
            {
                // Initialize if needed
                if (null == m_bsaFootnoteCustomSeq)
                    m_bsaFootnoteCustomSeq = new BStringArray();
                return m_bsaFootnoteCustomSeq;
            }
        }
        public BStringArray m_bsaFootnoteCustomSeq = null;
        #endregion

        #region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("VernacularWS",   ref m_sVernacularWritingSystemName);
			DefineAttr("ConsultantWS",   ref m_sConsultantWritingSystemName);
			DefineAttr("Comment",        ref m_sComment);
			DefineAttr("BookNamesTable", ref m_bsaBookNamesTable);

            DefineAttr("FootnoteSeqType", ref m_nFootnoteSequence);
            DefineAttr("FootnoteCustomSeq", ref m_bsaFootnoteCustomSeq);
        }
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g}: JOwnSeq Books - sequence of DBook
		public JOwnSeq<DBook> Books
		{
			get { return m_osBooks; }
		}
		JOwnSeq<DBook> m_osBooks = null;
		#endregion

		// Derived Attributes ----------------------------------------------------------------
		#region Attr{g}: DProject Project - returns the Project that owns this translation
		public DProject Project 
		{
			get
			{
				DProject project = (DProject)Owner;
				Debug.Assert(null != project);
				return project;
			}
		}
		#endregion
		#region Attr{g}: string NextAvailableBookAbbrev - returns the next open book slot
		public string NextAvailableBookAbbrev
		{
			get
			{
				ArrayList books = new ArrayList();

				// If This is the Front Translation, then we want to look at all of the
				// books which we haven't already added. 
				if (null != Owner && this == Project.FrontTranslation)
				{
					foreach (string s in DBook.BookAbbrevs)
					{
						if (null == FindBook(s))
							books.Add(s);
					}

					// If the first book is Genesis, then return it
					if ( "GEN" == books[0] as string )
						return books[0] as string;

					// If Malachi exists, and if there is a book after it, then return
					// the one after; this lets us default to something in the NT, which
					// will generally be nearer to what the user will be wanting to import.
					for(int i=0; i<books.Count - 1; i++)
					{
						if (books[i] as string == "MAL" )
							return books[i+1] as string;
					}

					// Return the first one, whatever it is
					return books[0] as string;
				}

				// If we are here, then we are dealing with a Target translation.
				// Get the list of Front translation books (or else, all books)
				if (null != Owner && null != Project.FrontTranslation)
				{
					foreach(DBook b in Project.FrontTranslation.Books)
						books.Add(b.BookAbbrev);
				}
				else
				{
					foreach (string s in DBook.BookAbbrevs)
						books.Add(s);
				}

				// Get the first book that isn't already done in This translation
				foreach( string s in books)
				{
					if (null == FindBook(s))
						return s;
				}

				// If we get here, then there are no front translation books for which
				// we don't have a target translation. So....just return the first thing
				// we can.
				return null;
			}
		}
		#endregion
		#region Attr{g}: JWritingSystem WritingSystemVernacular
		public JWritingSystem WritingSystemVernacular
		{
			get 
			{ 
				return DB.StyleSheet.FindOrAddWritingSystem( VernacularWritingSystemName );
			}
		}
		#endregion
		#region Attr{g}: JWritingSystem WritingSystemConsultant
		public JWritingSystem WritingSystemConsultant
		{
			get 
			{ 
				return DB.StyleSheet.FindOrAddWritingSystem( ConsultantWritingSystemName );
			}
		}
		#endregion
		#region VAttr{g}: bool IsTargetTranslation
		public bool IsTargetTranslation
		{
			get
			{
				Debug.Assert(null != Project);
				if ( this == Project.TargetTranslation )
					return true;
				return false;
			}
		}
		#endregion
        #region VAttr{g}: bool IsFrontTranslation
        public bool IsFrontTranslation
        {
            get
            {
                Debug.Assert(null != Project);
                if (this == Project.FrontTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region VAttr{g}: string[] EligibleNewBookAbbrevs - books we are allowed to import
        public string[] EligibleNewBookAbbrevs
        {
            get
            {
                // We'll store in an ArrayList for convenience
                ArrayList a = new ArrayList();

                foreach (string s in DBook.BookAbbrevs)
                {
                    // Does this abbreviation already exist in the translation?
                    // Don't add it, if so.
                    bool bAlreadyInBook = false;
                    foreach (DBook b in Books)
                    {
                        if (b.BookAbbrev == s)
                            bAlreadyInBook = true;
                    }
                    if (bAlreadyInBook)
                        continue;

                    // If this is the target translation, then we must also
                    // exclude books that are not in the Front
                    if (IsTargetTranslation)
                    {
                        bool bAlreadyInFront = false;
                        foreach (DBook bF in Project.FrontTranslation.Books)
                        {
                            if (bF.BookAbbrev == s)
                                bAlreadyInFront = true;
                        }
                        if (!bAlreadyInFront)
                            continue;
                    }

                    // If we made it here, we can add the book
                    a.Add(s);
                }

                // Convert to a string vector
                string[] v = new string[a.Count];
                for (int i = 0; i < a.Count; i++)
                    v[i] = a[i] as string;
                return v;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DTranslation()
			: base()
		{
			// Complex basic attrs
            m_bsaBookNamesTable = new BStringArray(
                BookNames.GetTable(DB.TeamSettings.FileNameLanguage));
            m_bsaFootnoteCustomSeq = new BStringArray();

			// Owning Sequence
			m_osBooks = new JOwnSeq<DBook>("Books", this, true, true);

			// Default Writing Systems
			VernacularWritingSystemName = DStyleSheet.c_Latin;
			ConsultantWritingSystemName = DStyleSheet.c_Latin;
		}
		#endregion
		#region Constructor(sDisplayName)
		public DTranslation(string sDisplayName)
			: this()
		{
			DisplayName = sDisplayName;
		}
		#endregion
		#region Constructor(sDisplayName, wsVernacular, wsConsultant)
		public DTranslation(string sDisplayName, string sVernacular, string sConsultant)
			: this()
		{
			DisplayName = sDisplayName;
			VernacularWritingSystemName = sVernacular;
			ConsultantWritingSystemName = sConsultant;
		}
		#endregion
		#region Attribute(g): string SortKey - supports find, sort
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
                // Page_OtherTranslations, among other things, depends on this being
                // the DisplayName; so don't change it.
				return DisplayName; 
			}
		}
		#endregion

		// Individual Books & Sections Access ------------------------------------------------
		#region Method: DBook AddBook(book) - adds a book that has already been constructed
		public DBook AddBook(DBook book)
		{
			Debug.Assert(book.BookAbbrev != "");
			Books.Append(book);
			return book;
		}
		#endregion
		#region Method: DBook FindBook(sBookAbbrev) - retrieves a book from the Books attribute
		public DBook FindBook(string sBookAbbrev)
		{
			int iOrder = DBook.FindBookAbbrevIndex(sBookAbbrev);
			int i = Books.Find(iOrder.ToString("00"));
			if (-1 == i)
				return null;
			return (DBook)Books[i];
		}
		#endregion
		#region Method: DBook FindBookByDisplayName(sDisplayName) - returns the book, or null
		public DBook FindBookByDisplayName(string sDisplayName)
		{
			foreach (DBook b in Books)
			{
				if (b.DisplayName == sDisplayName)
					return b;
			}
			return null;
		}
		#endregion
		#region Method: DSection GetFirstSection(sBookAbbrev) - rtns 1st section of desired book
		public DSection GetFirstSection(string sBookAbbrev)
		{
			int nBook = Books.Find(sBookAbbrev);
			if (-1 == nBook)
				return null;
			return ((DBook)Books[nBook]).GetFirstSection();
		}
		#endregion

        // Misc ------------------------------------------------------------------------------
        #region Method: void ConvertCrossReferences(DParagraph pSource, DParagraph pDest)
        public void ConvertCrossReferences(DParagraph pSource, DParagraph pDest)
        {
            pDest.Runs.Clear();

            foreach (DRun run in pSource.Runs)
            {
                DRun runDest = run.ConvertCrossRefs(
                    pSource.Translation.BookNamesTable,
                    pDest.Translation.BookNamesTable);

                if (null != runDest)
                    pDest.Runs.Append(runDest);
            }
        }
        #endregion
        #region Method: void UpdateFromFront()
        public void UpdateFromFront()
		{
			foreach(DBook book in Books)
			{
				// If the book isn't loaded, then no need to do this
				if (!book.Loaded)
					continue;

				// If for some reason there is no corresponding Front book, then abort
				if (null == book.FrontBook)
					continue;

				// If we are here, we have a loaded book and a corresponding book in
				// the Front translation, so go ahead and update all the references.
				book.UpdateFromFront(book.FrontBook);
			}
		}
		#endregion

        // Merging ---------------------------------------------------------------------------

		// I/O -------------------------------------------------------------------------------
		#region SAttr{g}: string FileExtension
		static public string FileExtension
		{
			get
			{
				return ".otrans";
			}
		}
		#endregion
		#region VAttr{g}: override string DefaultFileExtension
		public override string DefaultFileExtension
        {
            get
            {
				return FileExtension;
            }
        }
        #endregion
		#region OAttr{g}: string StoragePath
		public override string StoragePath
			// We'll store the Translation settings (otrans) file in the Cluster's settings folder
		{
			get
			{
				return Project.TeamSettings.SettingsFolder + StorageFileName;
			}
		}
		#endregion
		#region Attr{g}: string BookStorageFolder
		public string BookStorageFolder
		{
			get
			{
				return Project.TeamSettings.ClusterFolder + DisplayName + 
                    Path.DirectorySeparatorChar;
			}
		}
		#endregion
        #region OMethod: void InitialCreation(IProgressIndicator)
        public override void InitialCreation(IProgressIndicator progress)
            // The override makes sure the translation's folder exists, so that the
            // books have a place to be stored.
        {
            base.InitialCreation(progress);
            Directory.CreateDirectory(BookStorageFolder);
        }
        #endregion

	}

}
