#region ***** DTranslation.cs *****
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

        // Books -----------------------------------------------------------------------------
        #region Attr{g}: List<DBook> BookList
        public List<DBook> BookList
        {
            get
            {
                Debug.Assert(null != m_vBookList);
                return m_vBookList;
            }
        }
        List<DBook> m_vBookList;
        #endregion
        #region Method: void PopulateBookListFromFolder()
        public void PopulateBookListFromFolder()
        {
            // Get a list of all of the Oxes files in the translation's folder
            var vsOxesFiles = Directory.GetFiles(BookStorageFolder, "*.oxes");

            // Create books from each that fit the pattern
            foreach (string sOxesFileFullPath in vsOxesFiles)
            {
                // Parse the book's base name to see which one it is
                var vsParts = DBook.GetInfoFromPath(sOxesFileFullPath);
                if (null == vsParts || vsParts.Length != 3)
                    continue;
                string sBookAbbrev = vsParts[1];

                // Add the book to our list
                var book = new DBook(sBookAbbrev);
                AddBook(book);

                // Get the book's basic attributes from the individual files
                book.QuicklyReadBasicAttrs();
            }

            // Sort the list according to cannonical order
            BookList.Sort();
        }
        #endregion
		#region Method: DBook AddBook(book) - adds a book that has already been constructed
		public DBook AddBook(DBook book)
		{
			Debug.Assert(!string.IsNullOrEmpty(book.BookAbbrev));
            BookList.Add(book);
            BookList.Sort();
            book.Translation = this;
			return book;
		}
		#endregion
		#region Method: DBook FindBook(sBookAbbrev) - retrieves a book from the Books attribute
		public DBook FindBook(string sBookAbbrev)
		{
            foreach (DBook book in BookList)
            {
                if (book.BookAbbrev == sBookAbbrev)
                    return book;
            }
            return null;
		}
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
                    foreach (DBook b in BookList)
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
                        foreach (DBook bF in Project.FrontTranslation.BookList)
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

            // Books
            m_vBookList = new List<DBook>();

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
			foreach(DBook book in BookList)
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
				return DB.TeamSettings.ClusterFolder + DisplayName + 
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
        #region OMethod: bool OnLoad(...)
        protected override bool OnLoad(TextReader tr, string sPath, IProgressIndicator progress)
        {
            // Do the load as usual
            bool bSuccessful = base.OnLoad(tr, sPath, progress);
            if (!bSuccessful)
                return false;

            // Scan the disk to load the BookList
            PopulateBookListFromFolder();          

            return true;
        }
        #endregion
        #region OMethod: void OnWrite(progress)
        protected override void OnWrite(IProgressIndicator progress)
        {
            base.OnWrite(progress);

            foreach(DBook book in BookList)
                book.WriteBook(new NullProgress());
        }
        #endregion
    }

}
