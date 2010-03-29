/**********************************************************************************************
 * Project: Our Word!
 * File:    Messages.cs
 * Author:  John Wimbish
 * Created: 5 Oct 2007
 * Purpose: A compilation of the messages and strings used in OurWord.
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using JWTools;
using OurWordData;
using OurWordData.DataModel;
#endregion

namespace OurWord
{
    #region Messages
    class Messages
    {
        #region Info:      void BookIsLocked(DBook)
        static public void BookIsLocked(DBook book)
        {
            LocDB.Message("msgLockedBook",
                "Please discuss suggested changes or improvements with your team advisor.\n\n" +
                    "Further changes to {0}: {1} cannot be made directly on this computer." ,
                new string[] { book.Translation.DisplayName, book.DisplayName },
                LocDB.MessageTypes.Info);
        }
        #endregion

        #region Warning:   void BookNeedsImportFilename()
        static public void BookNeedsImportFilename()
        {
            LocDB.Message(
                "msgBookNeedsImportFilename",
                "Please enter the filename that you wish to import.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion

        #region Warning:   void CantInsertNoteHere()
        static public void CantInsertNoteHere()
        {
            LocDB.Message(
                "msgCantInsertNoteHere",
                "Notes cannot be inserted for book titles, picture captions or footnotes.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion
        #region YN:        bool ConfirmChangeFileLanguage(sNewLanguageName)
        static public bool ConfirmChangeFileLanguage(string sNewLanguageName)
        {
            return LocDB.Message("msgConfirmChangeFileLanguage",

                "Changing the language has the following effects:\n" +
                "1. The stage names will be reset to their 'factory' defaults,\n" +
                "2. File names of books will change (over time, as they are saved.)\n\n" +
                "Are you sure you wish to change to {0}?",

                new string[] { sNewLanguageName },
                LocDB.MessageTypes.YN);
        }
        #endregion
        #region YN:        bool ConfirmFileOverwrite(sPathName)
        static public bool ConfirmFileOverwrite(string sPathName)
        {
            return LocDB.Message("msgConfirmFileOverwrite",
                "The file: \n" +
                "    {0}\n" +
                "already exists on the disk. Are you sure you want to overwrite it?",
                new string[] { sPathName },
                LocDB.MessageTypes.YN);
        }
        #endregion
        #region YN:        bool ConfirmNoteDeletion(sNoteText)
        static public bool ConfirmNoteDeletion(string sNoteText)
        {
            return LocDB.Message(
                "msgConfirmNoteDeletion",
                "Are you sure you want to delete the note:\n  {0}?",
                new string[] { sNoteText },
                LocDB.MessageTypes.YN);
        }
        #endregion
        #region YN:        bool ConfirmDiscussionDeletion(sNoteText)
        static public bool ConfirmDiscussionDeletion(string sNoteText)
        {
            return LocDB.Message(
                "msgConfirmDiscussionDeletion",
                "Are you sure you want to delete the discussion item:\n  {0}?",
                new string[] { sNoteText },
                LocDB.MessageTypes.YN);
        }
        #endregion
        #region YN:        bool ConfirmRemoveTranslationStage(sStageName)
        static public bool ConfirmRemoveTranslationStage(string sStageName)
        {
            return LocDB.Message("msgConfirmRemoveTranslationStage",
                "You are about to delete a Translation Stage. If you have any books\n" +
                    "in your translation that are at this Stage, they will no longer have\n" +
                    "the correct stage information.\n\n" +
                    "Are you sure you want to delete '{0}'?",
                new string[] { sStageName },
                LocDB.MessageTypes.YN);
        }
        #endregion
        #region YN:        bool ConfirmResetStylesToDefaults()
        static public bool ConfirmResetStylesToDefaults()
        {
            return LocDB.Message("msgConfirmResetStylesToDefaults",
                "Are you sure you want to reset the entire stylesheet back " +
                    "to the default values?",
                null,
                LocDB.MessageTypes.YN);
        }
        #endregion
        #region YN:        bool ConfirmResetTranslationStagesToDefaults()
        static public bool ConfirmResetTranslationStagesToDefaults()
        {
            return LocDB.Message("msgConfirmResetTranslationStagesToDefaults",
                "Are you sure you want to reset the Translation Stages back to the " +
                    "default values?",
                null,
                LocDB.MessageTypes.YN);
        }
        #endregion

        #region Warning:   void DuplicateBookName()
        static public void DuplicateBookName()
        {
            LocDB.Message(
                "msgDuplicateBookName",
                "The name of the book cannot be identical to the name of another " +
                    "book in this translation.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion
        #region Error:     void DuplicateStagesNotAllowed()
        static public void DuplicateStagesNotAllowed()
        {
            LocDB.Message("msgDuplicateStagesNotAllowed",
                "Translation Stages cannot have duplicate names.",
                null,
                LocDB.MessageTypes.Error);
        }
        #endregion

        #region Warning:   void MissingChapterRange()
        static public void MissingChapterRange()
        {
            LocDB.Message(
                "msgMissingChapterRange",
                "Please enter the chapter numbers that you wish to print.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion
        #region Warning:   void MissingPageRange()
        static public void MissingPageRange()
        {
            LocDB.Message(
                "msgMissingPageRange",
                "Please enter the page numbers that you wish to print.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion

        #region Info:      void NoFilterMatches()
        static public void NoFilterMatches()
        {
            LocDB.Message("msgNoFilterMatches",
                "There were no sections that matched your criteria.",
                null,
                LocDB.MessageTypes.Info);
        }
        #endregion

        #region WarningYN: bool ProjectHasNoFront()
        static public bool ProjectHasNoFront()
        {
            return LocDB.Message(
                "msgProjectHasNoFront",
                "You have not defined a Front Translation for this project. Do " +
                    "you want to now?",
                null,
                LocDB.MessageTypes.WarningYN);
        }
        #endregion
        #region WarningYN: bool ProjectHasNoTarget()
        static public bool ProjectHasNoTarget()
        {
            return LocDB.Message(
                "msgProjectHasNoTarget",
                "You have not defined a Target Translation for this project. Do " +
                    "you want to now?",
                null,
                LocDB.MessageTypes.WarningYN);
        }
        #endregion

        #region Warning:   void TranslationNeedsAbbrev()
        static public void TranslationNeedsAbbrev()
        {
            LocDB.Message(
                "msgTranslationNeedsAbbrev",
                "Please enter an abbreviation for the translation, e.g., 'Ama'.\n\n" +
                    "This will be used to create the file name. The 3-letter Ethnologue code\n" +
                    "is normally used for this abbreviation.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion
        #region Warning:   void TranslationNeedsName()
        static public void TranslationNeedsName()
        {
            LocDB.Message(
                "msgTranslationNeedsName",
                "Please enter a name for the translation, e.g., 'Spanish.'",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion
        #region Warning:   void TranslationNeedsPath()
        static public void TranslationNeedsPath()
        {
            LocDB.Message("msgTranslationNeedsPath",
                "Please enter a valid File Name for storing the settings for " +
                    "this translation.",
                null,
                LocDB.MessageTypes.Warning);
        }
        #endregion

        #region WarningYN: bool VerifyRemoveTranslation()
        static public bool VerifyRemoveTranslation()
        {
            return LocDB.Message(
                "msgVerifyRemoveTranslation",
                "Do you want to remove this translation from the project? (This " +
                    "will not delete the file from the disk.)",
                null,
                LocDB.MessageTypes.WarningYN);
        }
        #endregion
        #region YN:        bool VerifyRestore(sCurrentFile, sBackupFile)
        static public bool VerifyRestore(string sCurrentFile, string sBackupFile)
        {
            return LocDB.Message("msgVerifyRestore",
                "Are you sure you want to replace file: \n" +
                "     {0}\n" +
                "with backup file: \n" +
                "     {1}?",
                new string[] { sCurrentFile, sBackupFile },
                LocDB.MessageTypes.YN);
        }
        #endregion
		#region WarningYN: bool VerifyOverwriteBook()
		static public bool VerifyOverwriteBook()
		{
			return LocDB.Message(
				"msgVerifyOverwriteBook",
				"There is already a file for this book. Is it OK if OurWord overwrites it? ",
				null,
				LocDB.MessageTypes.WarningYN);
		}
		#endregion
		#region WarningYN: bool VerifyReplaceBook()
		static public bool VerifyReplaceBook()
		{
			return LocDB.Message(
				"msgVerifyOverwriteBook",
				"This book already exists in the translation. Is it OK if OurWord replaces it?\n\n" +
					"(Caution: Your current book's file will be deleted.)",
				null,
				LocDB.MessageTypes.WarningYN);
		}
		#endregion

		#region Error:     void UnableToRemoveWritingSystem(sWritingSystemName)
		static public void UnableToRemoveWritingSystem(string sWritingSystemName)
		{
			LocDB.Message("msgUnableToRemoveWritingSystem",
				"OurWord is unable to remove the writing system: '{0}'\n" + 
					"because it is in use by one or more translations in your project.",
				new string[] { sWritingSystemName },
				LocDB.MessageTypes.Error);
		}
		#endregion
		#region Error:     void UnableToRemoveLatin()
		static public void UnableToRemoveLatin()
		{
			LocDB.Message("msgUnableToRemoveLatin",
				"OurWord is unable to remove the writing system: 'Latin'\n" +
					"because OurWord makes use of it for various internal operations.",
				null,
				LocDB.MessageTypes.Error);
		}
		#endregion
		#region WarningYN: bool VerifyRemoveWritingSystem(sWritingSystemName)
		static public bool VerifyRemoveWritingSystem(string sWritingSystemName)
		{
			return LocDB.Message(
				"msgVerifyRemoveWritingSystem",
				"Do you want to remove the writing system {0} from your team settings?.",
				new string[] { sWritingSystemName },
				LocDB.MessageTypes.WarningYN);
		}
		#endregion

    }
    #endregion

    class Strings
    {

        #region PROPERTIES DIALOG
        // Properties Dialog: Translation Stages Page
        #region String: NewTransStageAbbrev - used when creating a new Translation Stage
        static public string NewTransStageAbbrev
        {
            get
            {
                return G.GetLoc_String("NewTransStageAbbrev", "(abbrev)");
            }
        }
        #endregion
        #region String: NewTransStageName  - used when creating a new Translation Stage
        static public string NewTransStageName
        {
            get
            {
                return G.GetLoc_String("NewTransStageName", "(name)");
            }
        }
        #endregion

        // PropertiesDialog: Front/Target Setup Page
        #region String: SetupFT_FrontDefinition
        static public string SetupFT_FrontDefinition
        {
            get
            {
                return G.GetLoc_String("SetupFT_FrontDefinition", 
					"The Front Translation is a source text in a language accessible to " +
					"local translators. It has been fully consultant checked, and is " +
					"typically a regional language of wider communication that is " +
					"suitable for local translators to use to draft into their own " +
					"languages.");
            }
        }
        #endregion
        #region String: SetupFT_FrontCreateExpl
        static public string SetupFT_FrontCreateExpl
        {
            get
            {
                return G.GetLoc_String("SetupFT_FrontCreateExpl", 
					"Start from scratch to initialize settings for your front " +
					"translation. You will be presented with blank settings, and will " +
					"need to enter the name of the translation, its books, etc.");
            }
        }
        #endregion
        #region String: SetupFT_FrontOpenExpl
        static public string SetupFT_FrontOpenExpl
        {
            get
            {
                return G.GetLoc_String("SetupFT_FrontOpenExpl", 
				    "Make use of existing settings for your front translation. If you " +
					"defined the translation in another project, then you can reuse it " +
					"here, rather than entering the information all over again.");
            }
        }
        #endregion
        #region String: SetupFT_TargetDefinition
        static public string SetupFT_TargetDefinition
        {
            get
            {
                return G.GetLoc_String("SetupFT_TargetDefinition", 
					"The Target Translation is the vernacular translation that you " +
					"are producing. You use the Front Translation as a source to " +
					"create the Target. If you are a mother-tongue translator, then " +
					"the Target Translation is a translation into your own language. ");
            }
        }
        #endregion
        #region String: SetupFT_TargetCreateExpl
        static public string SetupFT_TargetCreateExpl
        {
            get
            {
                return G.GetLoc_String("SetupFT_TargetCreateExpl", 
					"Start from scratch to initialize settings for your target " +
					"translation. You will be presented with blank settings, and will " +
					"need to enter the name of the translation, its books, etc.");
            }
        }
        #endregion
        #region String: SetupFT_TargetOpenExpl
        static public string SetupFT_TargetOpenExpl
        {
            get
            {
                return G.GetLoc_String("SetupFT_TargetOpenExpl", 
					"Make use of existing settings for your target translation. If you " +
					"defined the translation in another project, then you can reuse it " +
					"here, rather than entering the information all over again.");
            }
        }
        #endregion

        // PropertiesDialog: Tab Text
        #region String: PropDlgTab_PrintOptions
        static public string PropDlgTab_PrintOptions
        {
            get
            {
                return G.GetLoc_String("PropDlgTab_PrintOptions", "Advanced Print Options");
            }
        }
        #endregion
        #region String: PropDlgTab_WritingSystems
        static public string PropDlgTab_WritingSystems
        {
            get
            {
                return G.GetLoc_String("PropDlgTab_WritingSystems", "Writing Systems");
            }
        }
        #endregion
        #region String: PropDlgTab_StyleSheet
        static public string PropDlgTab_StyleSheet
        {
            get
            {
                return G.GetLoc_String("PropDlgTab_StyleSheet", "StyleSheet");
            }
        }
        #endregion
        #region String: PropDlgTab_TranslationStages
        static public string PropDlgTab_TranslationStages
        {
            get
            {
                return G.GetLoc_String("PropDlgTab_TranslationStages", "Translation Stages");
            }
        }
        #endregion
        #endregion

    }
}
