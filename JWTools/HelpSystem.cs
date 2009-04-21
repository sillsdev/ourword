/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Help.cs
 * Author:  John Wimbish
 * Created: 06 Dec 2004
 * Purpose: Help system.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
#endregion

namespace JWTools
{
	class HelpTopic
	{
		#region Attr{g}: int ID - the ID of the topic; should be unique within the list
		public int ID
		{
			get
			{
				return m_nID;
			}
		}
		int    m_nID;
		#endregion
		#region Attr{g}: string Url - the URL that corresponds to the ID
		public string Url
		{
			get
			{
				Debug.Assert(null != m_sUrl);
				Debug.Assert(0 != m_sUrl.Length);
				return m_sUrl;
			}
		}
		string m_sUrl;
		#endregion

		#region Constructor(nID, sUrl)
		public HelpTopic(int nID, string sUrl)
		{
			m_nID = nID;
			m_sUrl = sUrl;
		}
		#endregion
	}

	public class HelpSystem
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: Form Form - the application's window
		static public Form Form
		{
			get
			{
				Debug.Assert(null != s_Form);
				return s_Form;
			}
		}
		static Form s_Form = null;
		#endregion
		#region Attr{g}: string FileName
		static public string FileName
		{
			get
			{
				Debug.Assert(null != s_FileName);
				Debug.Assert(0 != s_FileName.Length);
				return s_FileName;
			}
		}
		static string s_FileName = "";
		#endregion
		#region Attr{g/s}: static string RegistryHelpFile
		static public string RegistryHelpFile
		{
			get
			{
				string sPath =  JW_Registry.GetValue(c_sRegName, "");

				// If we got nothing, we need to prompt the user; if the user chickens out
				// (via the Cancel button), then we cannot supply help.
				if (0 == sPath.Length)
				{
					OpenFileDialog dlg = new OpenFileDialog();

					// We only want to return a single file
					dlg.Multiselect = false;

					// Default to the install directory
					dlg.FileName = Path.GetDirectoryName(Application.ExecutablePath) +
						"\\" + FileName;

					// Filter on HtmlHelp files
					dlg.Filter = "Help Files (*.chm)|*.chm";
					dlg.FilterIndex = 1;

					// Retrieve Dialog Title from resources
					dlg.Title = "Locate the \"" + FileName + "\" help file";

					if (DialogResult.OK == dlg.ShowDialog(Form))
					{
						sPath = dlg.FileName;
						RegistryHelpFile = dlg.FileName;
					}
				}

				return sPath;
			}
			set
			{
				JW_Registry.SetValue(c_sRegName, value);
			}
		}
		const string c_sRegName  = "HelpFile";
		#endregion
		#region Attr{g}: ArrayList Topics - the list of help topics
		static public ArrayList Topics
		{
			get
			{
				Debug.Assert(null != s_Topics);
				return s_Topics;
			}
		}
		static ArrayList s_Topics = null;
		#endregion

		// Topics ----------------------------------------------------------------------------
		const int c_nJWToolsReservedLo = 30000;
		const int c_nJWToolsReservedHi = 31000;
		#region enum Topic ID's
		public enum Topic
		{
			kDlgSetupFeatures = c_nJWToolsReservedLo,
			kDlgPasswordProtect,

            kTableOfContents = 0,
            kAutoBackup,
            kPrinting,
            kTranslationStages,
            kReferenceTranslations,
            kWritingSystems,
            kStyleSheet,
            kFilters,
            kTranslationNotes,

            kNewBook,

            kImportBook,
            kErrStructureSectionMiscount,
            kErrStructureMismatch,
            kErrMissingParagraphMarkerForNote,
            kErrChapterNotInParagraph,
            kErrBadChapterNo,
            kErrMissingParagraphMarker,
            kErrMissingVerseNumber,
            kErrMissingParagraphMarkerForCF,
            kErrNoVerseInSection,

            kLast
        }
		#endregion

		// Setup -----------------------------------------------------------------------------
        #region SMethod: void InitTopic_Strategy(Topic id, string sHtmlBase)
        static void InitTopic_Strategy(Topic id, string sHtmlBase)
        {
            AddTopic((int)id, "Strategies\\" + sHtmlBase + ".html");
        }
        #endregion
        #region SMethod: void InitTopic_ImportBook(Topic id, string sHtmlBase)
        static void InitTopic_ImportBook(Topic id, string sHtmlBase)
        {
            AddTopic((int)id, "GettingStarted\\ImportBook\\" + sHtmlBase + ".html");
        }
        #endregion
		#region SMethod: void Initialize(AppForm, sHelpFileName)
		static public void Initialize(Form form, string sFileName)
		{
			s_Topics   = new ArrayList();
			s_FileName = sFileName;
			s_Form     = form;

			// Initialize topics for the JWTools DLL
			AddTopic( Topic.kDlgSetupFeatures, 
				"Strategies\\SetupFeatures.html");
			AddTopic( Topic.kDlgPasswordProtect,
                "Strategies\\SetupFeatures.html");

            // Misc Topics
            AddTopic((int)Topic.kTableOfContents, "Index.html");
            AddTopic((int)Topic.kNewBook, "GettingStarted\\NewBook\\CreateAnEmptyBook.html");

            // Strategies topics
            InitTopic_Strategy(Topic.kAutoBackup, "AutoBackup\\AutoBackup");
            InitTopic_Strategy(Topic.kPrinting, "Printing");
            InitTopic_Strategy(Topic.kTranslationStages, "TranslationStages");
            InitTopic_Strategy(Topic.kReferenceTranslations, "ReferencingOtherTranslations");
            InitTopic_Strategy(Topic.kWritingSystems, "WritingSystems");
            InitTopic_Strategy(Topic.kStyleSheet, "Stylesheet");
            InitTopic_Strategy(Topic.kFilters, "Filters");
            InitTopic_Strategy(Topic.kTranslationNotes, "TranslationNotes");

            // Importing a book / associated errors
            InitTopic_ImportBook(Topic.kImportBook, "ImportBook");
            InitTopic_ImportBook(Topic.kErrMissingParagraphMarker, "Err_MissingParagraphMarker");
            InitTopic_ImportBook(Topic.kErrMissingVerseNumber, "Err_MissingVerseNumber");
            InitTopic_ImportBook(Topic.kErrMissingParagraphMarkerForCF, "Err_MissingParagraphMarkerForCF");
            InitTopic_ImportBook(Topic.kErrChapterNotInParagraph, "Err_ChapterNotInParagraph");
            InitTopic_ImportBook(Topic.kErrBadChapterNo, "Err_BadChapterNo");
            InitTopic_ImportBook(Topic.kErrMissingParagraphMarkerForNote, "Err_MissingParagraphMarkerForNote");
            InitTopic_ImportBook(Topic.kErrNoVerseInSection, "Err_NoVerseInSection");
            InitTopic_ImportBook(Topic.kErrStructureMismatch, "Err_StructureMismatch");
            InitTopic_ImportBook(Topic.kErrStructureSectionMiscount, "Err_StructureSectionMiscount");
		}
		#endregion

		#region Method: void AddTopic(Topic nID, string sUrl)
		static private void AddTopic(Topic nID, string sUrl)
		{
			// Make sure the ID isn't already in the list
			foreach(HelpTopic t in Topics)
			{
				if (t.ID == (int)nID)
				{
					Debug.Assert(false, "Topic ID already exists: " + nID.ToString() +
						" - " + sUrl);
				}
			}

			// Add the ID
			Topics.Add( new HelpTopic((int)nID, sUrl) );
		}
		#endregion
		#region Method: void AddTopic(int nID, string sUrl)
		static public void AddTopic(int nID, string sUrl)
		{
			// Make sure the ID's are not in our reserved range
			if (nID >= c_nJWToolsReservedLo && nID < c_nJWToolsReservedHi)
			{
				Debug.Assert(false, "This range of IDs is reserved for JW_Tools.");
			}

			// Make sure the ID isn't already in the list
			foreach(HelpTopic t in Topics)
			{
				if (t.ID == nID)
				{
					Debug.Assert(false, "Topic ID already exists: " + nID.ToString() +
						" - " + sUrl);
				}
			}

			// Add the ID
			Topics.Add( new HelpTopic(nID, sUrl) );
		}
		#endregion

		// Show help topics ------------------------------------------------------------------
		#region Method: static void ShowDefaultTopic()
		static public void ShowDefaultTopic()
		{
            try
            {
                Help.ShowHelp(Form, RegistryHelpFile);
            }
            catch (Exception) 
            {
            }
		}
		#endregion
		#region Method: void ShowTopic(int nID)
		static public void ShowTopic(int nID)
		{
			foreach(HelpTopic topic in Topics)
			{
				if (topic.ID == nID)
				{
                    try
                    {
                        Help.ShowHelp(Form, RegistryHelpFile, topic.Url);
                    }
                    catch (Exception)
                    {
                    }
					return;
				}
			}

			ShowDefaultTopic();
		}
		#endregion
        #region Method: void ShowTopic(Topic)
        static public void ShowTopic(Topic topic)
        {
            ShowTopic((int)topic);
        }
        #endregion

        // Individual topics -----------------------------------------------------------------
		#region Method: void Show_DlgFeatures()
		static public void Show_DlgFeatures()
		{
			ShowTopic((int)Topic.kDlgSetupFeatures);
		}
		#endregion
		#region Method: void Show_DlgPasswordProtect()
		static public void Show_DlgPasswordProtect()
		{
			ShowTopic((int)Topic.kDlgPasswordProtect);
		}
		#endregion
	}

}
