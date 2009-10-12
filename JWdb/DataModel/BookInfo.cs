#region ***** BookInfo.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    BookInfo.cs
 * Author:  John Wimbish
 * Created: 3 Oct 2009
 * Purpose: Information about a book: number of chapters, verses, etc.
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
using System.IO;

using JWTools;
using JWdb;
#endregion
#endregion

namespace JWdb.DataModel
{
    public class BookInfo
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string Abbrev
        public string Abbrev
        {
            get
            {
                return m_sAbbrev;
            }
        }
        string m_sAbbrev;
        #endregion
        #region Attr{g}: string VersesCount
        public int VersesCount
        {
            get
            {
                return m_cVersesCount;
            }
        }
        int m_cVersesCount;
        #endregion
        #region Attr{g}: string ChaptersCount
        public int ChaptersCount
        {
            get
            {
                return m_cChaptersCount;
            }
        }
        int m_cChaptersCount;
        #endregion

        // Groupings -------------------------------------------------------------------------
        public enum Groupings { Pentateuch, Historical, Poetical, Prophetic, Gospels, 
            PaulineLetters, GeneralLetters, None };
        #region Attr{g}: Groupings Grouping
        Groupings Grouping
        {
            get
            {
                return m_Grouping;
            }
        }
        Groupings m_Grouping = Groupings.None;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sAbbrev, ChaptersCount, VersesCout, Grouping)
        public BookInfo(string sAbbrev, int cChaptersCount, int cVersesCount, Groupings grouping)
        {
            m_sAbbrev = sAbbrev;
            m_cChaptersCount = cChaptersCount;
            m_cVersesCount = cVersesCount;
            m_Grouping = grouping;
        }
        #endregion
    }

    public class BookInfoList
    {
        #region SAttr{g}: List<BookInfo> Books
        static public List<BookInfo> Books
        {
            get
            {
                Initialize();
                return s_Books;
            }
        }
        static List<BookInfo> s_Books;
        #endregion
        #region SMethod: BookInfo AddBook(sAbbrev, cChaptersCount, cVersesCount)
        static public BookInfo AddBook(string sAbbrev, int cChaptersCount, int cVersesCount, BookInfo.Groupings grouping)
        {
            var bi = new BookInfo(sAbbrev, cChaptersCount, cVersesCount, grouping);
            Books.Add(bi);
            return bi;
        }
        #endregion
        #region SMethod: BookInfo FindBook(string sAbbrev)
        static public BookInfo FindBook(string sAbbrev)
        {
            foreach (BookInfo bi in Books)
            {
                if (bi.Abbrev == sAbbrev)
                    return bi;
            }
            return null;
        }
        #endregion
        #region SMethod: BookInfo FindBook(DBook book)
        static public BookInfo FindBook(DBook book)
        {
            if (null == book)
                return null;
            return FindBook(book.BookAbbrev);
        }
        #endregion

        #region SMethod: void Initialize()
        static void Initialize()
            // OT verse counts are from http://www.blueletterbible.org/study/misc/66books.cfm
            // Catholoic site (http://catholic-resources.org/Bible/OT-Statistics-NAB.htm) 
            //    disaggress on a few books, largely off by one verse.
        {
            if (null != s_Books)
                return;

            s_Books = new List<BookInfo>();

            AddBook("GEN", 50, 1533, BookInfo.Groupings.Pentateuch);
            AddBook("EXO", 40, 1213, BookInfo.Groupings.Pentateuch);
            AddBook("LEV", 27,  859, BookInfo.Groupings.Pentateuch);
            AddBook("NUM", 36, 1288, BookInfo.Groupings.Pentateuch);
            AddBook("DEU", 34,  959, BookInfo.Groupings.Pentateuch);

            AddBook("JOS", 24,  658, BookInfo.Groupings.Historical);
            AddBook("JDG", 21,  618, BookInfo.Groupings.Historical);
            AddBook("RUT",  4,   85, BookInfo.Groupings.Historical);
            AddBook("1SA", 31,  810, BookInfo.Groupings.Historical);
            AddBook("2SA", 24,  695, BookInfo.Groupings.Historical);
            AddBook("1KI", 22,  816, BookInfo.Groupings.Historical);
            AddBook("2KI", 25,  719, BookInfo.Groupings.Historical);
            AddBook("1CH", 29,  942, BookInfo.Groupings.Historical);
            AddBook("2CH", 36,  822, BookInfo.Groupings.Historical);
            AddBook("EZR", 10,  280, BookInfo.Groupings.Historical);
            AddBook("NEH", 13,  406, BookInfo.Groupings.Historical);
            AddBook("EST", 10,  167, BookInfo.Groupings.Historical);

            AddBook("JOB", 42, 1070, BookInfo.Groupings.Poetical);
            AddBook("PSA",150, 2461, BookInfo.Groupings.Poetical);
            AddBook("PRO", 31,  915, BookInfo.Groupings.Poetical);
            AddBook("ECC", 12,  222, BookInfo.Groupings.Poetical);
            AddBook("SNG",  8,  117, BookInfo.Groupings.Poetical);

            AddBook("ISA", 66, 1292, BookInfo.Groupings.Prophetic);
            AddBook("JER", 52, 1364, BookInfo.Groupings.Prophetic);
            AddBook("LAM",  5,  154, BookInfo.Groupings.Prophetic);
            AddBook("EZK", 48, 1273, BookInfo.Groupings.Prophetic);
            AddBook("DAN", 12,  357, BookInfo.Groupings.Prophetic);
            AddBook("HOS", 14,  197, BookInfo.Groupings.Prophetic);
            AddBook("JOL",  3,   73, BookInfo.Groupings.Prophetic);
            AddBook("AMO",  9,  146, BookInfo.Groupings.Prophetic);
            AddBook("OBA",  1,   21, BookInfo.Groupings.Prophetic);
            AddBook("JON",  4,   48, BookInfo.Groupings.Prophetic);
            AddBook("MIC",  7,  105, BookInfo.Groupings.Prophetic);
            AddBook("NAM",  3,   47, BookInfo.Groupings.Prophetic);
            AddBook("HAB",  3,   56, BookInfo.Groupings.Prophetic);
            AddBook("ZEP",  3,   53, BookInfo.Groupings.Prophetic);
            AddBook("HAG",  2,   38, BookInfo.Groupings.Prophetic);
            AddBook("ZEC", 14,  211, BookInfo.Groupings.Prophetic);
            AddBook("MAL",  4,   55, BookInfo.Groupings.Prophetic);

            AddBook("MAT", 28, 1071, BookInfo.Groupings.Gospels);
            AddBook("MRK", 16,  678, BookInfo.Groupings.Gospels);
            AddBook("LUK", 24, 1151, BookInfo.Groupings.Gospels);
            AddBook("JHN", 21,  879, BookInfo.Groupings.Gospels);

            AddBook("ACT", 28, 1007, BookInfo.Groupings.None);

            AddBook("ROM", 16,  433, BookInfo.Groupings.PaulineLetters);
            AddBook("1CO", 16,  437, BookInfo.Groupings.PaulineLetters);
            AddBook("2CO", 13,  257, BookInfo.Groupings.PaulineLetters);
            AddBook("GAL",  6,  149, BookInfo.Groupings.PaulineLetters);
            AddBook("EPH",  6,  155, BookInfo.Groupings.PaulineLetters);
            AddBook("PHP",  4,  104, BookInfo.Groupings.PaulineLetters);
            AddBook("COL",  4,   95, BookInfo.Groupings.PaulineLetters);
            AddBook("1TH",  5,   89, BookInfo.Groupings.PaulineLetters);
            AddBook("2TH",  3,   47, BookInfo.Groupings.PaulineLetters);
            AddBook("1TI",  6,  113, BookInfo.Groupings.PaulineLetters);
            AddBook("2TI",  4,   83, BookInfo.Groupings.PaulineLetters);
            AddBook("TIT",  3,   46, BookInfo.Groupings.PaulineLetters);
            AddBook("PHM",  1,   25, BookInfo.Groupings.PaulineLetters);

            AddBook("HEB", 13,  303, BookInfo.Groupings.GeneralLetters);
            AddBook("JAS",  5,  108, BookInfo.Groupings.GeneralLetters);
            AddBook("1PE",  5,  105, BookInfo.Groupings.GeneralLetters);
            AddBook("2PE",  3,   61, BookInfo.Groupings.GeneralLetters);
            AddBook("1JN",  5,  105, BookInfo.Groupings.GeneralLetters);
            AddBook("2JN",  1,   13, BookInfo.Groupings.GeneralLetters);
            AddBook("3JN",  1,   14, BookInfo.Groupings.GeneralLetters);
            AddBook("JUD",  1,   25, BookInfo.Groupings.GeneralLetters);

            AddBook("REV", 22,  404, BookInfo.Groupings.None);
        }
        #endregion

        #region SAttr{g}: int MaxVerses
        static public int MaxVerses
        {
            get
            {
                int c = 0;
                foreach (BookInfo bi in Books)
                    c =  Math.Max(c, bi.VersesCount);
                return c;
            }
        }
        #endregion
        #region SAttr{g}: int MinVerses
        static public int MinVerses
        {
            get
            {
                int c = 100000;
                foreach (BookInfo bi in Books)
                    c = Math.Min(c, bi.VersesCount);
                return c;
            }
        }
        #endregion
    }

}
