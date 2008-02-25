/**********************************************************************************************
 * Project: Our Word!
 * File:    DTranslation.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: The top-level Scripture Translation object
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.Dialogs;
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
 * - The ctrlRemove button asks the user to verify his intention, then removes the book from
 *   the list (but not from the disk)
 */
#endregion

namespace OurWord.DataModel
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
		#region BAttr{g/s}: string LanguageAbbrev - E.g., the Ethnologue 3-letter code (for file naming)
		public string LanguageAbbrev
		{
			get
			{
				return m_sLanguageAbbrev;
			}
			set
			{
                SetValue(ref m_sLanguageAbbrev, value);
			}
		}
		private string m_sLanguageAbbrev;
		#endregion
		#region BAttr{g}: BStringArray BookNamesTable - Lists the 66 books
		public BStringArray BookNamesTable
		{
			get
			{
				// This has been happening when using an older oTrans file
//				if (null == m_bsaBookNamesTable || m_bsaBookNamesTable.Length == 0)
//					m_bsaBookNamesTable = new BStringArray(DBook.BookNames_Indonesian);

				return m_bsaBookNamesTable;
			}
		}
		public BStringArray m_bsaBookNamesTable = null;
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("VernacularWS",   ref m_sVernacularWritingSystemName);
			DefineAttr("ConsultantWS",   ref m_sConsultantWritingSystemName);
			DefineAttr("Comment",        ref m_sComment);
			DefineAttr("BookNamesTable", ref m_bsaBookNamesTable);
			DefineAttr("LangAbbrev",     ref m_sLanguageAbbrev);
		}
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g}: JOwnSeq Books - sequence of DBook
		public JOwnSeq Books
		{
			get { return m_osBooks; }
		}
		JOwnSeq m_osBooks = null;
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
		#region Attr{g}: JStyleSheet StyleSheet - the ss for the project
		public JStyleSheet StyleSheet
		{
			get
			{
				return G.StyleSheet;
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
				return DBook.BookAbbrevs[0];
			}
		}
		#endregion
		#region Attr{g}: JWritingSystem WritingSystemVernacular
		public JWritingSystem WritingSystemVernacular
		{
			get 
			{ 
				return StyleSheet.FindOrAddWritingSystem( VernacularWritingSystemName );
			}
		}
		#endregion
		#region Attr{g}: JWritingSystem WritingSystemConsultant
		public JWritingSystem WritingSystemConsultant
		{
			get 
			{ 
				return StyleSheet.FindOrAddWritingSystem( ConsultantWritingSystemName );
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
        #region VAttr{g}: override string FileFilter
        protected override string FileFilter
        {
            get
            {
                return G.GetLoc_Files("TranslationFileFilter", 
                    "Our Word Translation File (*.oTrans)|*.oTrans"); 
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DTranslation()
			: base()
		{
			ConstructAttrs();
		}
		#endregion
		#region Constructor(sDisplayName, wsVernacular, wsConsultant)
		public DTranslation(string sDisplayName, string sVernacular, 
			string sConsultant)
			: base()
		{
			// Initialize the attributes
			ConstructAttrs();

			DisplayName = sDisplayName;
			VernacularWritingSystemName = sVernacular;
			ConsultantWritingSystemName = sConsultant;

			// TODO: HANDLE THIS TEMPORARY KLUDGE: Need a Translation Properties file
//			if (VernacularWritingSystemName == "Chinese")
//				m_Encoding = Encoding.UTF8;

		}
		#endregion
		#region Method: void ConstructAttrs()
		private void ConstructAttrs()
		{
			// Complex basic attrs
			DTeamSettings ts = G.TeamSettings;
			m_bsaBookNamesTable = new BStringArray(BookNames.GetTable(ts.FileNameLanguage));

			// Owning Sequence
			m_osBooks = new JOwnSeq("Books", this, typeof(DBook), true, true);
		}
		#endregion
		#region Attribute(g): string SortKey - supports find, sort
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return DisplayName; 
			}
		}
		#endregion
        #region VAttr{g}: override string DefaultFileExtension
        public override string DefaultFileExtension
        {
            get
            {
                return ".otrans";
            }
        }
        #endregion

		// Individual Books & Sections Access ------------------------------------------------
		#region Method: DBook AddBook(sBookAbbrev, sPath, bReadNow) - adds a book to the Books attribute
		public DBook AddBook(string sBookAbbrev, string sPath, bool bReadNow)
		{
			DBook book = new DBook(sBookAbbrev, sPath);
			m_osBooks.Append(book);
            if (bReadNow)
                book.Load();
			return book;
		}
		#endregion
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
		#region Method: string ConvertCrossReferences(DParagraph pSource)
		public string ConvertCrossReferences(DParagraph pSource)
		{
			DTranslation TSource = pSource.Translation;
			Debug.Assert(null != TSource);

			// We are assuming a single Text from the source with a single phrases; thus
			// not supporting italics, or other such phrase or run types.
			string sSource = pSource.SimpleText;

			// Create the converted string
			int i = 0;
			string sDest = "";

			// Loop through the source string, comparing for matches
			while( i < sSource.Length )
			{
				// Look for a match from amongst the book names
				int iBookName = TSource.BookNamesTable.FindSubstringMatch(sSource, i, true);

				// If not found, add the current character and move on to the next one
				if (-1 == iBookName)
				{
					sDest += sSource[i++];
					continue;
				}

				// Else it was found; so make the substitution.
				sDest += this.BookNamesTable[iBookName];
				i += TSource.BookNamesTable[iBookName].Length;
			}

			return sDest;
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
	}

	#region TEST
	public class Test_DTranslation : Test
	{
		#region Constructor()
		public Test_DTranslation()
			: base("DTranslation")
		{
			AddTest( new IndividualTest( ConvertsCrossReferences ), "ConvertsCrossReferences" );
		}
		#endregion

		#region ConvertsCrossReferences
		public void ConvertsCrossReferences()
			// Test the conversion from a Source cross-reference to a Target cross-reference.
			// We want to see that 
			// - only those things that are in the Source get changed,
			// - only whole words in the string get changed (not partial matches)
		{
			DTranslation TFront = new DTranslation("Front", "Latin", "Latin");
			DTranslation TTarget = new DTranslation("Target", "Latin", "Latin");

			DBook book = new DBook();
			TFront.Books.Append(book);
			DSection section = new DSection(1);
			book.Sections.Append(section);
			DParagraph para = new DParagraph(TFront);
			section.Paragraphs.Append(para);

            TFront.BookNamesTable.Clear();
			TFront.BookNamesTable.Append("Genesis");
			TFront.BookNamesTable.Append("Exodus");
			TFront.BookNamesTable.Append("Ge");
			TFront.BookNamesTable.Append("2 Kings");
			TFront.BookNamesTable.Append("Song of Songs");
			TFront.BookNamesTable.Append("2 John");
			TFront.BookNamesTable.Append("Carita (Mula-Mula)");

            TTarget.BookNamesTable.Clear();
			TTarget.BookNamesTable.Append("Kejadian");
			TTarget.BookNamesTable.Append("Keluaran");
			TTarget.BookNamesTable.Append("Imamat");
			TTarget.BookNamesTable.Append("2 Raja-Raja");
			TTarget.BookNamesTable.Append("Kidung Agung");
			TTarget.BookNamesTable.Append("2 Yohanes");
			TTarget.BookNamesTable.Append("Kejadian");

			string sSource   = "(Genesis 3:4; Exodus 12:4, 3; Ex 3:4, " +
				"Genesissy 23:4; 2 Kings 13:3; Carita (Mula-Mula) 5:5, 23";
			string sExpected = "(Kejadian 3:4; Keluaran 12:4, 3; Ex 3:4, " +
				"Genesissy 23:4; 2 Raja-Raja 13:3; Kejadian 5:5, 23";
			para.SimpleText = sSource;

			string sActual = TTarget.ConvertCrossReferences(para);

			AreSame(sExpected, sActual);
		}
		#endregion
	}
	#endregion
}
