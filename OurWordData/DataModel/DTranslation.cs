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
using OurWordData;
using Chorus.merge;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWordData.DataModel
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
            // When doing unit testing, we may not have a cluster folder, in which
            // case we don't want to be creating a BookStorageFolder
            if (string.IsNullOrEmpty(DB.TeamSettings.ClusterFolder))
                return;

            // Get a list of all of the Oxes files in the translation's folder
            if (!Directory.Exists(BookStorageFolder))
                Directory.CreateDirectory(BookStorageFolder);
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
		#region Attr{g}: WritingSystem WritingSystemVernacular
		public WritingSystem WritingSystemVernacular
		{
			get 
			{
                return StyleSheet.FindOrCreate(VernacularWritingSystemName);
			}
		}
		#endregion
		#region Attr{g}: WritingSystem WritingSystemConsultant
		public WritingSystem WritingSystemConsultant
		{
			get 
			{
                return StyleSheet.FindOrCreate(ConsultantWritingSystemName);
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
		{
            // LocDB.DB.PrimaryLanguage
			// Complex basic attrs
            string sLanguageName = "";
            if (null != LocDB.DB.PrimaryLanguage)
                sLanguageName = LocDB.DB.PrimaryLanguage.Name;
            m_bsaBookNamesTable = new BStringArray(
                BookNames.GetTable(sLanguageName)); 
            m_bsaFootnoteCustomSeq = new BStringArray();

            // Books
            m_vBookList = new List<DBook>();

			// Default Writing Systems
			VernacularWritingSystemName = WritingSystem.DefaultWritingSystemName;
            ConsultantWritingSystemName = WritingSystem.DefaultWritingSystemName;
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
        #region SMethod: void ConvertCrossReferences(DParagraph pSource, DParagraph pDest)
        static public void ConvertCrossReferences(DParagraph pSource, DParagraph pDest)
        {
            pDest.Runs.Clear();

            foreach (DRun run in pSource.Runs)
            {
                var runDest = run.ConvertCrossRefs(
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

	    protected override string DefaultFileExtension
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
        #region Method: void Initialize()
        public void Initialize()
        {
            // Create its folder if not already present (this will be the .Settings folder)
            var sFolder = Path.GetDirectoryName(StoragePath);
            if (!Directory.Exists(sFolder))
                Directory.CreateDirectory(sFolder);

            // If the file already exists where we expect it, then load its settings; 
            // otherwise write a file with the default settings
            if (File.Exists(StoragePath))
                LoadFromFile(StoragePath);
            else
                WriteToFile(StoragePath);

            // Create the folder which will house the Oxes book files
            if (!Directory.Exists(BookStorageFolder))
                Directory.CreateDirectory(BookStorageFolder);
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        const string c_sTag = "DTranslation";
        const string c_sAttrDisplayName = "DisplayName";
        const string c_sAttrVernacularWS = "VernacularWS";
        const string c_sAttrConsultantWS = "ConsultantWS";
        const string c_sAttrComment = "Comment";
        const string c_sBookNamesTable = "BookNamesTable";
        const string c_sFootnoteSeqType = "FootnoteSeqType";
        const string c_sFootnoteCustomSeq = "FootnoteCustomSeq";
        #endregion

        #region Method: void LoadFromFile(string sPath)
        public void LoadFromFile(string sPath)
            // The parameterized version of LoadFromFile (with sPath) is needed in order to
            // support merging, where we don't already know the pathname.
        {
            if (Loaded)
                return;
            if (string.IsNullOrEmpty(sPath))
                return;

            try
            {
                // Bring the file into the dotnet system
                var xml = new XmlDoc();
                xml.Load(sPath);

                // Get the translation node and interpret it
                var node = XmlDoc.FindNode(xml, c_sTag);
                DisplayName = XmlDoc.GetAttrValue(node, c_sAttrDisplayName, "(TranslationName)");
                VernacularWritingSystemName = XmlDoc.GetAttrValue(node, c_sAttrVernacularWS, "Latin");
                ConsultantWritingSystemName = XmlDoc.GetAttrValue(node, c_sAttrConsultantWS, "Latin");
                Comment = XmlDoc.GetAttrValue(node, c_sAttrComment, "");

                var sBookNames = XmlDoc.GetAttrValue(node, c_sBookNamesTable, "");
                BookNamesTable.Read(sBookNames);

                try
                {
                    var sFootnoteType = XmlDoc.GetAttrValue(node, c_sFootnoteSeqType, "0");
                    FootnoteSequenceType = (DFoot.FootnoteSequenceTypes)Convert.ToInt16(sFootnoteType);
                }
                catch (Exception)
                {
                }

                var sCustomSeq = XmlDoc.GetAttrValue(node, c_sFootnoteCustomSeq, "");
                FootnoteCustomSeq.Read(sCustomSeq);

                // Scan the disk to load the BookList
                PopulateBookListFromFolder();

                // Successful
                m_bIsLoaded = true;
                IsDirty = false;
            }
            catch (Exception e)
            {
                // Error Message
                LocDB.Message("CantReadTranslationSettings",
                    "OurWord was unable to read the Translation Settings File {0}\nwith reason {1}",
                    new string[] { Path.GetFileName(sPath), e.Message },
                    LocDB.MessageTypes.Error);

                // Should still be false, but make certain
                m_bIsLoaded = false;
            }
        }
        #endregion
        #region Method: void LoadFromFile()
        public void LoadFromFile()
        {
            LoadFromFile(StoragePath);
        }
        #endregion

        #region Method: void WriteToFile(string sPath)
        public void WriteToFile(string sPath)
        {
            if (IsDirty)
            {
                // Initialize the xml object
                var xml = new XmlDoc();
                var node = xml.AddNode(xml, c_sTag);

                // Add attrs if not empty
                xml.AddAttr(node, c_sAttrDisplayName, DisplayName);
                xml.AddAttr(node, c_sAttrVernacularWS, VernacularWritingSystemName);
                xml.AddAttr(node, c_sAttrConsultantWS, ConsultantWritingSystemName);
                xml.AddAttr(node, c_sAttrComment, Comment);
                xml.AddAttr(node, c_sBookNamesTable, BookNamesTable.SaveLine);
                xml.AddAttr(node, c_sFootnoteSeqType, ((int)FootnoteSequenceType).ToString());
                xml.AddAttr(node, c_sFootnoteCustomSeq, FootnoteCustomSeq.SaveLine);

                // Write it out
                xml.Write(sPath);
            }

            // Save any books that have been modified
            foreach (var book in BookList)
                book.WriteBook(new NullProgress());
        }
        #endregion
        #region Method: void WriteToFile(IProgressIndicator progress)
        public override void WriteToFile(IProgressIndicator progress)
        {
            WriteToFile(StoragePath);
        }
        #endregion

        // Merge -----------------------------------------------------------------------------
        #region SMethod: void Merge(MergeOrder mergeOrder)
        static public void Merge(MergeOrder mergeOrder)
            // Merges the DTranslation files on an attr-by-attr bases. So conceivable different
            // people can edit differing attributes, yet merge. If they edit the same attr, though,
            // "ours" will win.
        {
            if (mergeOrder == null) throw new ArgumentNullException("mergeOrder");

            // Load into three DTranslation objects
            var parentTranslation = new DTranslation();
            var ourTranslation = new DTranslation();
            var theirTranslation = new DTranslation();
            parentTranslation.LoadFromFile(mergeOrder.pathToCommonAncestor);
            ourTranslation.LoadFromFile(mergeOrder.pathToOurs);
            theirTranslation.LoadFromFile(mergeOrder.pathToTheirs);

            // If we differ from Parent, then keep Ours, otherwise keep Theirs. Or put another
            // way, if we equal the parent, then we keep theirs, assuming either theirs has
            // changed, or neither has changed. OTOH, if we are different from the parent then
            // this logic keeps ours, which means that if both have made changes, Ours wins.
            if (ourTranslation.DisplayName == parentTranslation.DisplayName)
                ourTranslation.DisplayName = theirTranslation.DisplayName;

            if (ourTranslation.VernacularWritingSystemName == parentTranslation.VernacularWritingSystemName)
                ourTranslation.VernacularWritingSystemName = theirTranslation.VernacularWritingSystemName;

            if (ourTranslation.ConsultantWritingSystemName == parentTranslation.ConsultantWritingSystemName)
                ourTranslation.ConsultantWritingSystemName = theirTranslation.ConsultantWritingSystemName;

            if (ourTranslation.Comment == parentTranslation.Comment)
                ourTranslation.Comment = theirTranslation.Comment;

            if (ourTranslation.BookNamesTable.Length == theirTranslation.BookNamesTable.Length &&
                ourTranslation.BookNamesTable.Length == parentTranslation.BookNamesTable.Length)
            {
                for (int i = 0; i < ourTranslation.BookNamesTable.Length; i++)
                {
                    if (ourTranslation.BookNamesTable[i] == parentTranslation.BookNamesTable[i])
                        ourTranslation.BookNamesTable[i] = theirTranslation.BookNamesTable[i];
                }
            }

            if (ourTranslation.FootnoteSequenceType == parentTranslation.FootnoteSequenceType)
                ourTranslation.FootnoteSequenceType = theirTranslation.FootnoteSequenceType;

            if (ourTranslation.FootnoteCustomSeq.SaveLine == parentTranslation.FootnoteCustomSeq.SaveLine)
                ourTranslation.FootnoteCustomSeq.Read(theirTranslation.FootnoteCustomSeq.SaveLine);

            // Save the result
            ourTranslation.WriteToFile(mergeOrder.pathToOurs);
        }
        #endregion
    }

}
