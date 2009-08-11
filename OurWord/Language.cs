/**********************************************************************************************
 * Project: Our Word!
 * File:    Language.cs
 * Author:  John Wimbish
 * Created: 16 Dec 2004
 * Purpose: Localizations
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using JWTools;
using JWdb;

using JWdb.DataModel;
using OurWord.Layouts;
using OurWord.Dialogs;
#endregion

namespace OurWord
{
	#region CLASS StrRes - String Resources
	public class StrRes : LanguageResources
	{
		// Dialogs: Book Properties ----------------------------------------------------------
		#region Resource: Properties(sInsert)
		static string[] PropertiesAlts =
		{
			"{0} Properties",
			"Ciri-ciri {0}"
		};
		static public string Properties(string sInsert)
		{
			string sBase = GetAlternative(PropertiesAlts);
			return Insert(sBase, sInsert, "");
		}
		#endregion
		#region Resource: Properties_Book
		static string[] Properties_BookAlts =
		{
			"Book Properties",
			"Ciri-ciri Buku"
		};
		static public string Properties_Book
		{
			get
			{
				return GetAlternative(Properties_BookAlts);
			}
		}
		#endregion

        /***
        ***/
	}
	#endregion

	// Dialogs -------------------------------------------------------------------------------
	#region DIALOG CLASS: DlgBookPropsRes
	class DlgBookPropsRes : DlgRes
	{
		// Increment Book Status Dialog ------------------------------------------------------
		#region Label: IncrementBookStatusTitle
		static public string IncrementBookStatusTitle
		{
			get
			{
				string[] res = 
					{
						"Increment Book Status",
						"Status Buku"
					};
				return GetAlternative(res);
			}
		}
		#endregion

		// Browse for a file to import -------------------------------------------------------
		#region FindFile: FindFileDlgTitle(sBookName)
		static public string FindFileDlgTitle(string sBookName)
		{
			string[] resShort = 
			{
				"Find Import File",
				""
			};
			string[] resComplete = 
			{
				"Find Import File for {0}",
				""
			};

			if (0 == sBookName.Length)
				return GetAlternative(resShort);
			else
				return Insert(GetAlternative(resComplete), sBookName);
		}
		#endregion
		#region FindFile: FindFileDlgFilter
		static public string FindFileDlgFilter
		{
			get
			{
				string[] res = 
				{
					"Shoebox Databases (*.db)|*.db|" + 
						"Standard Format files (*.sf)|*.sf|" + 
						"Paratext files (*.ptx)|*.ptx|" +
						"All files (*.*)|*.*",
					""
				};
				return GetAlternative(res);
			}
		}
		#endregion

		// Browse for a folder to create the file in -----------------------------------------
		#region Label: BrowseFolderDescription
		static public string BrowseFolderDescription
		{
			get
			{
				string[] res = 
					{
						"Select the folder into which this new file will be created.",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion

		#region Label: BrowseFolderTitle
		static public string BrowseFolderTitle
		{
			get
			{
				string[] res = 
					{
						"Browse for folder",
						"Cari map"
					};
				return GetAlternative(res);
			}
		}
		#endregion

		// Main Dialog labels, controls, etc. ------------------------------------------------
		#region Label: File
		static public string File
		{
			get
			{
				string[] res = 
					{
						"File:",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Folder
		static public string Folder
		{
			get
			{
				string[] res = 
					{
						"Folder:",
						"Map:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: LanguageAbbrev
		static public string LanguageAbbrev
		{
			get
			{
				string[] res = 
					{
						"Language:",
						"Bahasa:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Book
		static public string Book
		{
			get
			{
				string[] res = 
					{
						"Book:",
						"Buku:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Stage
		static public string Stage
		{
			get
			{
				string[] res = 
					{
						"Stage:",
						"Langkah:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Version
		static public string Version
		{
			get
			{
				string[] res = 
					{
						"Version:",
						"Versi:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Copyright
		static public string Copyright
		{
			get
			{
				string[] res = 
					{
						"Copyright:",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion

		#region Label: Locked
		static public string Locked
		{
			get
			{
				string[] res = 
					{
						"Locked: Editing is not permitted. (\"To Do\" notes can be added.)",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion

		#region Group: FileName
		static public string FileName
		{
			get
			{
				string[] res = 
					{
						"File Name",
						"Nama File"
					};
				return GetAlternative(res);
			}
		}
		#endregion
	}
	#endregion

    #region DIALOG CLASS: DlgOptionsRes
    class DlgOptionsRes : DlgRes
	{
		// Entire Dialog ---------------------------------------------------------------------
		#region Title: Title
		static public string Title
		{
			get
			{
				string[] res = 
					{
						"Options",
						"Pilihan"
					};
				return GetAlternative(res);
			}
		}
		#endregion

		// General Tab -----------------------------------------------------------------------
		#region Label: TabGeneral
		static public string TabGeneral
		{
			get
			{
				string[] res = 
					{
						"General",
						"Umum"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: ZoomTo
		static public string ZoomTo
		{
			get
			{
				string[] res = 
					{
						"Zoom to (magnification factor):",
						"Perbesar (huruf):"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: ShowUserInterfaceIn
		static public string ShowUserInterfaceIn
		{
			get
			{
				string[] res = 
					{
						"Show User Interface in:",
						"Bahasa untuk mengatur program:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Check: MaximizeOnStartup
		static public string MaximizeOnStartup
		{
			get
			{
				string[] res = 
					{
						"Maximize window when OurWord starts?",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Check: AutoBackup
		static public string AutoBackup
		{
			get
			{
				string[] res = 
					{
						"Automatically make backup files?",
						"Mengatur file cadangan secara otomat?"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: StoreFilesIn
		static public string StoreFilesIn
		{
			get
			{
				string[] res = 
					{
						"Store Files In:",
						"File cadangan disimpan di:"
					};
				return GetAlternative(res);
			}
		}
		#endregion

		// Drafting Tab ----------------------------------------------------------------------
		#region Label: TabDrafting
		static public string TabDrafting
		{
			get
			{
				string[] res = 
					{
						"Drafting",
						"Konsep Kasar"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: DraftBkColor
		static public string DraftBkColor
		{
			get
			{
				string[] res = 
					{
						"Window Background Color:",
						"Warna Latar Belakang:"
					};
				return GetAlternative(res);
			}
		}
		#endregion

        /***
        ***/

        /***
		// Notes Tab -------------------------------------------------------------------------
		#region Label: TabNotes
		static public string TabNotes
		{
			get
			{
				string[] res = 
					{
						"Notes",
						"Catatan"
					};
				return GetAlternative(res);
			}
		}
		#endregion
        ***/

		// Auto Replace Tab ------------------------------------------------------------------
		#region Label: TabAutoReplace
		static public string TabAutoReplace
		{
			get
			{
				string[] res = 
					{
						"Auto Replace",
						"Ganti secara Otomat"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: ForWritingSystem
		static public string ForWritingSystem
		{
			get
			{
				string[] res = 
					{
						"For Writing System:",
						"Bagi Sistem Tulis:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Replace
		static public string Replace
		{
			get
			{
				string[] res = 
					{
						"Replace:",
						"Ganti:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: With
		static public string With
		{
			get
			{
				string[] res = 
					{
						"With:",
						"Dengan:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: AddPair
		static public string AddPair
		{
			get
			{
				string[] res = 
					{
						"Add",
						"Tambah"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: RemovePair
		static public string RemovePair
		{
			get
			{
				string[] res = 
					{
						"Remove",
						"Hapus"
					};
				return GetAlternative(res);
			}
		}
		#endregion
	}
	#endregion

}
