/**********************************************************************************************
 * Project: Our Word!
 * File:    Language.cs
 * Author:  John Wimbish
 * Created: 16 Dec 2004
 * Purpose: Localizations
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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

using OurWord.DataModel;
using OurWord.View;
using OurWord.Dialogs;
#endregion

namespace OurWord
{
	#region CLASS StrRes - String Resources
	public class StrRes : LanguageResources
	{
		// File Filters & Default File Names -------------------------------------------------
		#region Resource: FileFilterProject
		static string[] FileFilterProjectAlts =
		{
			"Our Word! Project Files (*.owp)|*.owp",
			"Sabda Kita! File Proyek (*.owp)|*.owp"
		};
		static public string FileFilterProject
		{
			get
			{
				return GetAlternative(FileFilterProjectAlts);
			}
		}
		#endregion
		#region Resource: FileFilterTranslation
		static string[] FileFilterTranslationAlts =
		{
			"Our Word Translation File (*.oTrans)|*.oTrans",
			"Sabda Kita File Terjemahan (*.oTrans)|*.oTrans"
		};
		static public string FileFilterTranslation
		{
			get
			{
				return GetAlternative(FileFilterTranslationAlts);
			}
		}
		#endregion
		#region Resource: FileFilterTeamSettings
		static string[] FileFilterTeamSettingsAlts =
		{
			"Our Word Team Settings File (*.owt)|*.owt",
			"Sabda Kita Setelan Tim (*.owt)|*.owt"
		};
		static public string FileFilterTeamSettings
		{
			get
			{
				return GetAlternative(FileFilterTeamSettingsAlts);
			}
		}
		#endregion
		#region Resource: DefaultFileName_Project
		static string[] DefaultFileName_ProjectAlts =
		{
			"New Project.owp",
			"Proyek Baru.owp"
		};
		static public string DefaultFileName_Project
		{
			get
			{
				return GetAlternative(DefaultFileName_ProjectAlts);
			}
		}
		#endregion
		#region Resource: DefaultFileName_Translation
		static string[] DefaultFileName_TranslationAlts =
		{
			"New Translation.oTrans",
			"Bahasa Baru.oTrans"
		};
		static public string DefaultFileName_Translation
		{
			get
			{
				return GetAlternative(DefaultFileName_TranslationAlts);
			}
		}
		#endregion
	 
		// Dialogs: Save___As ----------------------------------------------------------------
		#region Resource: DlgSaveTranslationAs_Title
		static string[] DlgSaveTranslationAs_TitleAlts =
		{
			"Save Translation Definition As",
			"Simpan Definisi Terjemahan Sebagai"
		};
		static public string DlgSaveTranslationAs_Title
		{
			get
			{
				return GetAlternative(DlgSaveTranslationAs_TitleAlts);
			}
		}
		#endregion
		#region Resource: DlgSaveAs_BrowseButton
		static string[] DlgSaveAs_BrowseButtonAlts =
		{
			"Save As...",
			"Simpan sebagai..."
		};
		static public string DlgSaveAs_BrowseButton
		{
			get
			{
				return GetAlternative(DlgSaveAs_BrowseButtonAlts);
			}
		}
		#endregion

        // Export related --------------------------------------------------------------------
        #region Resource: DlgBrowseExportBook_Title
        static string[] DlgBrowseExportBook_TitleAlts =
		{
			"Enter a filename for export",
		};
        static public string DlgBrowseExportBook_Title
        {
            get
            {
                return GetAlternative(DlgBrowseExportBook_TitleAlts);
            }
        }
        #endregion
        #region Resource: FileFilterExportBook
        static string[] FileFilterExportBookAlts =
		{
			"Paratext File (*.ptx)|*.ptx",
		};
        static public string FileFilterExportBook
        {
            get
            {
                return GetAlternative(FileFilterExportBookAlts);
            }
        }
        #endregion

		// Dialogs: Open ---------------------------------------------------------------------
		#region Resource: DlgOpenFrontTranslation_Title
		static string[] DlgOpenFrontTranslation_TitleAlts =
		{
			"Open Front Translation Definition File",
			"Buka File Definisi untuk Terjemahan Induk"
		};
		static public string DlgOpenFrontTranslation_Title
		{
			get
			{
				return GetAlternative(DlgOpenFrontTranslation_TitleAlts);
			}
		}
		#endregion
		#region Resource: DlgOpenTargetTranslation_Title
		static string[] DlgOpenTargetTranslation_TitleAlts =
		{
			"Open Target Translation Definition File",
			"Buka File Definisi untuk Terjemahan Anak"
		};
		static public string DlgOpenTargetTranslation_Title
		{
			get
			{
				return GetAlternative(DlgOpenTargetTranslation_TitleAlts);
			}
		}
		#endregion
		#region Resource: DlgOpenSiblingTranslation_Title
		static string[] DlgOpenSiblingTranslation_TitleAlts =
		{
			"Open Sibling Translation Definintion File",
			"Buka File Definisi untuk Terjemahan Serumpun"
		};
		static public string DlgOpenSiblingTranslation_Title
		{
			get
			{
				return GetAlternative(DlgOpenSiblingTranslation_TitleAlts);
			}
		}
		#endregion
		#region Resource: DlgOpenReferenceTranslation_Title
		static string[] DlgOpenReferenceTranslation_TitleAlts =
		{
			"Open Reference Translation Definition File",
			"Buka File Definisi untuk Terjemahan Sejajar"
		};
		static public string DlgOpenReferenceTranslation_Title
		{
			get
			{
				return GetAlternative(DlgOpenReferenceTranslation_TitleAlts);
			}
		}
		#endregion

		// Dialogs: Properties ---------------------------------------------------------------
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

		// Pane Titles -----------------------------------------------------------------------
		#region Resource: DraftingJobTitle
		static public string DraftingJobTitle
		{
			get
			{
				string[] alts = 
					{
						"Drafting",
						"Susun Konsep",
                        "Borrador",
                        "Kuandika insha"
					};
				return GetAlternative(alts);
			}
		}
		#endregion
		#region Resource: DraftingJobReference
		static public string DraftingJobReference
		{
			get
			{
				string[] alts = 
					{
						"{0} to {1}",
						"{0} ke {1}"
					};
				string sBase = GetAlternative(alts);
				string s = Insert(sBase, FrontName, TargetName);
				return s + PaneReference;
			}
		}
		#endregion
		#region Resource: BackTransJobTitle
		static public string BackTransJobTitle
		{
			get
			{
				string[] alts = 
					{
						"Back Translation",
						"Terjemahan Balik",              
                        "Traducción Inversa",
                        "Tafsiri ya kujirudia "
					};
				return GetAlternative(alts);
			}
		}
		#endregion
		#region Resource: BackTransJobReference
		static public string BackTransJobReference
		{
			get
			{
				string s = Insert("{0}", TargetName);
				return s + PaneReference;
			}
		}
		#endregion
		#region Resource: IntBTJobTitle
		static public string IntBTJobTitle
		{
			get
			{
				string[] alts = 
					{
						"Interlinear Back Translation",
						""
					};
				return GetAlternative(alts);
			}
		}
		#endregion
		#region Resource: IntBTJobReference
		static public string IntBTJobReference
		{
			get
			{
				string s = Insert("{0}", TargetName);
				return s + PaneReference;
			}
		}
		#endregion

		#region Resource: FrontName
		static string[] NoFrontDefinedAlts =
		{
			"(no front defined)",
			"(komputer belum kenal terjemahan induk)"
		};
		static private string FrontName
		{
			get
			{
				if ( null != OurWordMain.Project.FrontTranslation )
					return OurWordMain.Project.FrontTranslation.DisplayName;

				return GetAlternative(NoFrontDefinedAlts);
			}
		}
		#endregion
		#region Resource: TargetName
		static string[] NoTargetDefinedAlts =
		{
			"(no target defined)",
			"(komputer belum kenal terjamahan sasaran)"
		};
		static private string TargetName
		{
			get
			{
				if ( null != OurWordMain.Project.TargetTranslation )
					return OurWordMain.Project.TargetTranslation.DisplayName.ToUpper();

				return GetAlternative(NoTargetDefinedAlts);
			}
		}
		#endregion
		#region Resource: string PaneReference
		static private string PaneReference
		{
			get
			{
				// Shorthand
				DTranslation TFront = OurWordMain.Project.FrontTranslation;
				DTranslation TTarget = OurWordMain.Project.TargetTranslation;
				DSection STarget = OurWordMain.Project.STarget;

				// The reference of the current section
				if ( null != TFront && null != TTarget && null != STarget )
					return " - " + OurWordMain.Project.STarget.ReferenceName;

				return "";
			}
		}
		#endregion
		#region Resource: NotesPaneTitle
		static public string NotesPaneTitle
		{
			get
			{
				string[] res = 
					{
						"Notes",
						"Catatan",              
                        "Notas",
                        "Noti"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Resource: RelatedLanguagesPaneTitle
		static public string RelatedLanguagesPaneTitle
		{
			get
			{
				string[] res = 
					{
						"Related Languages",
						"Terjemahan Bandingan"
					};
				return GetAlternative(res);
			}
		}
		#endregion

		// Notes default text ---------------------------------------------------------------
		#region Resource: NoteDefaultText_ToDoDefault
		static string[] NoteDefaultText_ToDoDefaultAlts =
		{
			"To Do: ",
			"Diperhatikan: "
		};
		static public string NoteDefaultText_ToDoDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_ToDoDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_FrontDefault
		static string[] NoteDefaultText_FrontDefaultAlts =
		{
			"Front: ",
			"Terjemahan Induk: "
		};
		static public string NoteDefaultText_FrontDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_FrontDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_BTDefault
		static string[] NoteDefaultText_BTDefaultAlts =
		{
			"BT: ",
			"TB: "
		};
		static public string NoteDefaultText_BTDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_BTDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_AskUnsDefault
		static string[] NoteDefaultText_AskUnsDefaultAlts =
		{
			"Ask UNS: ",
			"Tanya Responden: "
		};
		static public string NoteDefaultText_AskUnsDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_AskUnsDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_HintDefault
		static string[] NoteDefaultText_HintDefaultAlts =
		{
			"Hint: ",
			""
		};
		static public string NoteDefaultText_HintDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_HintDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_GreekDefault
		static string[] NoteDefaultText_GreekDefaultAlts =
		{
			"Greek: ",
			""
		};
		static public string NoteDefaultText_GreekDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_GreekDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_HebrewDefault
		static string[] NoteDefaultText_HebrewDefaultAlts =
		{
			"Hebrew: ",
			""
		};
		static public string NoteDefaultText_HebrewDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_HebrewDefaultAlts);
			}
		}
		#endregion
		#region Resource: NoteDefaultText_ExegesisDefault
		static string[] NoteDefaultText_ExegesisDefaultAlts =
		{
			"Exegesis: ",
			""
		};
		static public string NoteDefaultText_ExegesisDefault
		{
			get
			{
				return GetAlternative(NoteDefaultText_ExegesisDefaultAlts);
			}
		}
		#endregion

		// Miscellaneous (likely to be re-worked later) --------------------------------------
		#region Resource: BlankColumn - used by ParallelDraft
		static string[] BlankColumnAlts =
		{
			"Blank Column",
			"Kolom Kosong"
		};
		static public string BlankColumn
		{
			get
			{
				return GetAlternative(BlankColumnAlts);
			}
		}
		#endregion
		#region Resource: CantDisplayRelatedLanguages
		static string[] CantDisplayRelatedLanguagesAlts =
		{
			"(Unable to display due to paragraph mismatch(es) with the Front translation.)",
			"(Tidak bisa diperagakan karena ketidak-cocokkan alinea dengan Terjemahan Induk.)"
		};
		static public string CantDisplayRelatedLanguages
		{
			get
			{
				return GetAlternative(CantDisplayRelatedLanguagesAlts);
			}
		}
		#endregion
        #region String: NoProjectDefined
        static public string NoProjectDefined
        {
            get
            {
                string[] res = 
					{
						"No Project Defined",
						""
					};
                return GetAlternative(res);
            }
        }
        #endregion
		#region Resource: NoViewData
		static string[] NoViewDataAlts =
		{
			"(There is no data to display. Use the File-Properties menu " +
			"item to set up both a Front and a Target translation.)",

			"(Tidak ada data yang dapat diperagakan di sini. Gunakanlah daftar " +
			"File-Ciri-ciri untuk menentukan baik Terjemahan Induk maupun Terjemahan Anak.)"
		};
		static public string NoViewData
		{
			get
			{
				return GetAlternative(NoViewDataAlts);
			}
		}
		#endregion
		#region Resource: BrowseForFolderDlgDescription
		static string[] BrowseForFolderDlgDescriptionAlts =
		{
			"Select the folder where you wish to place your backup files. " +
			"If possible, this should not be your hard drive. A flash card " +
			"is ideal; or a floppy drive can also be used.",

			"Tunjukkanlah map untuk simpan file cadangan. Sebaiknya HDD jangan " +
			"ditunjuk (karena, jika HDD rusak, file cadangan tidak bisa diamankan). " +
			"Yang paling baik adalah sebuah memori \'flash card\'. Floppy (A:\\) juga " +
			"bisa ditunjukkan, tetapi perlu diingat bahwa setiap kali Sabda Kita " +
			"dipakai, floppy disk yang selalu sama perlu dimasukkan ke dalam floppy drive."
		};
		static public string BrowseForFolderDlgDescription
		{
			get
			{
				return GetAlternative(BrowseForFolderDlgDescriptionAlts);
			}
		}
		#endregion
		#region Resource: NewNote - on inserting a note, the temporary reference
		static public string NewNote
		{
			get
			{
				string[] Alts =
				   {
						"NEW",
						"BARU"
				   };
				return GetAlternative(Alts);
			}
		}
		#endregion
        #region String: TypeHere
        static public string TypeHere
        {
            get
            {
                string[] res = 
					{
						"[Type Here]",
						"[Ketik di sini]"
					};
                return GetAlternative(res);
            }
        }
        #endregion

		#region string GetStyleName(sStyleAbbrev)
		static public string GetStyleName(string sStyleAbbrev)
		{
			DSFMapping Map = G.TeamSettings.SFMapping;

			// s - Section Title
			string[] vSectionTitleAlts = 
			{
				"Section Title",
				""
			};
			if (sStyleAbbrev == Map.StyleSection)
				return GetAlternative(vSectionTitleAlts);

			// s2 - Section Title Level 2
			string[] vSectionTitle2Alts = 
			{
				"Section Title 2",
				""
			};
			if (sStyleAbbrev == Map.StyleSection2)
				return GetAlternative(vSectionTitle2Alts);

			// r - Cross Reference
			string[] vCrossRefAlts = 
			{
				"Cross Ref",
				""
			};
			if (sStyleAbbrev == Map.StyleCrossRef)
				return GetAlternative(vCrossRefAlts);

			// mt - Main Title
			string[] vMainTitleAlts = 
			{
				"Main Title",
				""
			};
			if (sStyleAbbrev == Map.StyleMainTitle)
				return GetAlternative(vMainTitleAlts);

			// st - Sub Title
			string[] vSubTitleAlts = 
			{
				"Sub Title",
				""
			};
			if (sStyleAbbrev == Map.StyleSubTitle)
				return GetAlternative(vSubTitleAlts);

			// h - Header
			string[] vHeaderAlts = 
			{
				"Header",
				""
			};
			if (sStyleAbbrev == Map.StyleHeader)
				return GetAlternative(vHeaderAlts);

			// cap - Picture Caption
			string[] vPictureAlts = 
			{
				"Picture",
				""
			};
			if (sStyleAbbrev == Map.StylePicCaption)
				return GetAlternative(vPictureAlts);

			return "";
		}
		#endregion
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
		#region Label: Abbreviation
		static public string Abbreviation
		{
			get
			{
				string[] res = 
					{
						"Abbreviation:",
						"Singkatan:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: BookName
		static public string BookName
		{
			get
			{
				string[] res = 
					{
						"Book Name:",
						"Nama Buku:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
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

		// Back Translation Tab --------------------------------------------------------------
		#region Label: TabBT
		static public string TabBT
		{
			get
			{
				string[] res = 
					{
						"Back Translation",
						"Terjemahan Balik"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: BTBkColor
		static public string BTBkColor
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
		#region Check: ShowFreeTranslations
		static public string ShowFreeTranslations
		{
			get
			{
				string[] res = 
					{
						"Show Free Translations?",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Check: ShowPictures
		static public string ShowPictures
		{
			get
			{
				string[] res = 
					{
						"Show Pictures?",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion

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
		#region Check: ShowNotesPane
		static public string ShowNotesPane
		{
			get
			{
				string[] res = 
					{
						"Show Notes Pane?",
						"Membuka kotak untuk Catatan?"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Check: HintFromFrontNote
		static public string HintFromFrontNote
		{
			get
			{
				string[] res = 
					{
						"Drafting Hints from the Front",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: NoteBackgroundColor
		static public string NoteBackgroundColor
		{
			get
			{
				string[] res = 
					{
						"Note Background Color:",
						"Warna Catatan Belakang:"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: NotesBkColor
		static public string NotesBkColor
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
		#region Method: static string GetName_GeneralNote()
		static public string GetName_GeneralNote()
		{
			string[] res = 
				{
					"General Note",
					"Catatan Umum"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_OldVersionNote()
		static public string GetName_OldVersionNote()
		{
			string[] res = 
				{
					"Old Versions",
					"Versi Lama"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_ToDoNote()
		static public string GetName_ToDoNote()
		{
			string[] res = 
				{
					"To Do",
					"Untuk Diperhatikan"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_HintForDaughterNote()
		static public string GetName_HintForDaughterNote()
		{
			string[] res = 
				{
					"Hints for Drafting Daughters",
					""
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_HintFromFrontNote()
		static public string GetName_HintFromFrontNote()
		{
			string[] res = 
				{
					"Drafting Hints from the Front",
					""
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_ReasonsNote()
		static public string GetName_ReasonsNote()
		{
			string[] res = 
				{
					"Reasons",
					"Alasan Untuk Susunan"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_DefinitionsNote()
		static public string GetName_DefinitionsNote()
		{
			string[] res = 
				{
					"Definitions",
					"Arti Kata"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_FrontIssuesNote()
		static public string GetName_FrontIssuesNote()
		{
			string[] res = 
				{
					"Front Issues",
					"Perihal Induk"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_BTNote()
		static public string GetName_BTNote()
		{
			string[] res = 
				{
					"Back Translation",
					"Terjemahan Balik"
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_AskUnsNote()
		static public string GetName_AskUnsNote()
		{
			string[] res = 
				{
					"Ask UNS",
					"Tanya Responden"
				};
			return GetAlternative(res);
		}
		#endregion

		#region Method: static string GetName_GreekNote()
		static public string GetName_GreekNote()
		{
			string[] res = 
				{
					"Greek",
					""
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_HebrewNote()
		static public string GetName_HebrewNote()
		{
			string[] res = 
				{
					"Hebrew",
					""
				};
			return GetAlternative(res);
		}
		#endregion
		#region Method: static string GetName_ExegesisNote()
		static public string GetName_ExegesisNote()
		{
			string[] res = 
				{
					"Exegesis",
					""
				};
			return GetAlternative(res);
		}
		#endregion

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
