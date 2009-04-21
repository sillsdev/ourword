/**********************************************************************************************
 * Project: Our Word!
 * File:    BookNames.cs
 * Author:  John Wimbish
 * Created: 10 Mar 2009
 * Purpose: Table of localized book names
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
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using JWTools;
using JWdb;
#endregion

namespace JWdb.DataModel
{
    public class BookNames
    {
        // Attrs -----------------------------------------------------------------------------
        #region SAttr{g}: LocGroup LocGroup - the localization group containing the book names (LocItems)
        const string c_LocGroupID = "BookNames";
        static LocGroup LocGroup
        {
            get
            {
                if (null == s_LocGroup)
                    s_LocGroup = LocDB.DB.FindGroup(c_LocGroupID);
                return s_LocGroup;
            }
        }
        static LocGroup s_LocGroup = null;
        #endregion

        // Retrieve a single name, according to the current language preferences
        #region Method: string GetName(int index)
        static public string GetName(int index)
        {
            Debug.Assert(index >= 0 && index < 66);

            // The LocItem's lookup ID is the English form of the book
            string sLocItemID = English[index];

            // If for some reason the Group was not found, then return the English value
            if (null == LocGroup)
                return English[index];

            // Find the LocItem containing the localizations; return English if not found
            LocItem item = LocGroup.Find(sLocItemID);
            if (null == item)
                return English[index];

            // The LocDB will either return the string in the requested language, or
            // English if not found.
            return item.AltValue;
        }
        #endregion

        // Retrieve a table of Book Names ----------------------------------------------------
        const int c_cTableSize = 66;      // Number of books in the Bible
        #region Method: string[] GetTable(LanguageResources.Languages lang)
        static public string[] GetTable(LanguageResources.Languages lang)
        {
            // Retrieve the name of the language
            string sLanguageName = LanguageResources.GetLanguageName(lang);

            // The GetTable(sLanguageName) method will do the rest of the work
            return GetTable(sLanguageName);
        }
        #endregion
        #region Method: string[] GetTable(string sLanguageName)
        static public string[] GetTable(string sLanguageName)
        // Return a vector of strings for the 66 books, corresponding to the
        // requested language name.
        {
            // We'll build the table here
            string[] vs = new string[c_cTableSize];

            // Get the index of the alternative we'll want
            LocLanguage lang = LocDB.DB.FindLanguageByName(sLanguageName);
            if (null == lang)
                return English;
            int iLanguage = lang.Index;

            // Make sure we found the LocGroup
            if (null == LocGroup)
                return English;

            // Fill up the table
            for (int i = 0; i < c_cTableSize; i++)
            {
                LocItem item = LocGroup.Find(English[i]);
                if (null == item)
                    vs[i] = English[i];
                else if (null == item.Alternates[iLanguage])
                    vs[i] = English[i];
                else
                    vs[i] = item.Alternates[iLanguage].Value;
            }

            return vs;
        }
        #endregion

        // Localized list of booknames -------------------------------------------------------
        #region Attr{g} string[] English - if we can't find a language, we always have English here
        static public string[] English = 
	    { 
		    "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy", "Joshua",
		    "Judges", "Ruth", "1 Samuel", "2 Samuel", "1 Kings", "2 Kings", 
		    "1 Chronicles", "2 Chronicles", "Ezra", "Nehemiah", "Esther", "Job", 
		    "Psalms", "Proverbs", "Ecclesiastes", "Song of Songs", "Isaiah", 
		    "Jeremiah", "Lamentations", "Ezekiel", "Daniel", "Hosea", "Joel", 
		    "Amos", "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk", "Zephaniah",
		    "Haggai", "Zechariah", "Malachi", "Matthew", "Mark", "Luke", "John", 
		    "Acts", "Romans", "1 Corinthians", "2 Corinthians", "Galatians", 
		    "Ephesians", "Philippians", "Colossians", "1 Thessalonians", 
		    "2 Thessalonians", "1 Timothy", "2 Timothy", "Titus", "Philemon", 
		    "Hebrews", "James", "1 Peter", "2 Peter", "1 John", "2 John", "3 John", 
		    "Jude", "Revelation" 
	    };
        #endregion
    }
}
