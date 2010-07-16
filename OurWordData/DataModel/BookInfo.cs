using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OurWordData.DataModel
{
    public class BookInfo
    {
        // Attrs -----------------------------------------------------------------------------
        public readonly string Abbrev;
        public readonly int VerseCount;
        public readonly int ChapterCount;
        public BookGroup Group;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sAbbrev, cChaptersCount, cVersesCount)
        public BookInfo(string sAbbrev, int cChaptersCount, int cVersesCount)
        {
            Debug.Assert(!string.IsNullOrEmpty(sAbbrev));
            Debug.Assert(cVersesCount > 0);
            Debug.Assert(cChaptersCount > 0);

            Abbrev = sAbbrev;
            VerseCount = cVersesCount;
            ChapterCount = cChaptersCount;
        }
        #endregion
    }

    public class BookGroup : List<BookInfo>
    {
        // Attrs -----------------------------------------------------------------------------
        public readonly string EnglishName;
        #region VAttr{g}: string LocalizedName
        public string LocalizedName
        {
            get
            {
                // The lookup key is just the English Name without any spaces
                var sKey = EnglishName.Replace(" ", "");

                return Loc.GetBookGroupings(sKey, EnglishName);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sEnglishName)
        public BookGroup(string sEnglishName)
        {
            EnglishName = sEnglishName;
        }
        #endregion

        // Names -----------------------------------------------------------------------------
        public const string Pentateuch = "Pentateuch";
        public const string Historical = "Historical";
        public const string Poetical = "Poetical";
        public const string Prophetic = "Prophetic";
        public const string Gospels = "Gospels";
        public const string Acts = "Acts";
        public const string PaulineLetters = "Pauline Letters";
        public const string GeneralLetters = "General Letters";
        public const string Revelation = "Revelation";
    }

    public class BookGroups : List<BookGroup>
    {
        // Queries ---------------------------------------------------------------------------
        #region VAttr{g}: List<BookInfo> AllBookInfos
        public List<BookInfo> AllBookInfos
        {
            get
            {
                var v = new List<BookInfo>();
                foreach (var group in this)
                    v.AddRange(group);
                return v;
            }
        }
        #endregion
        #region Method: BookInfo FindBook(sBookAbbrev)
        public BookInfo FindBook(string sBookAbbrev)
        {
            var v = AllBookInfos;
            foreach(var bookInfo in v)
            {
                if (bookInfo.Abbrev == sBookAbbrev)
                    return bookInfo;
            }

            throw new Exception(
                string.Format("Unrecognized book abbreviation: {0}.", sBookAbbrev));
        }
        #endregion
        #region Attr{g}: int MaxVerses
        public int MaxVerses
        {
            get
            {
                var c = 0;
                var v = AllBookInfos;
                foreach (var bi in v)
                    c = Math.Max(c, bi.VerseCount);
                return c;
            }
        }
        #endregion
        #region Attr{g}: int MinVerses
        public int MinVerses
        {
            get
            {
                var c = 100000;
                var v = AllBookInfos;
                foreach (var bi in v)
                    c = Math.Min(c, bi.VerseCount);
                return c;
            }
        }
        #endregion

        // List Building ---------------------------------------------------------------------
        #region Method: BookGroup FindOrAddGroup(sEnglishName)
        public BookGroup FindOrAddGroup(string sEnglishName)
        {
            // If its already there, then return it
            foreach(var group in this)
            {
                if (group.EnglishName == sEnglishName)
                    return group;
            }

            // Otherwise, add it
            var newGroup = new BookGroup(sEnglishName);
            Add(newGroup);
            return newGroup;
        }
        #endregion
        #region method: void Add(BookInfo, sGroupName)
        void Add(BookInfo info, string sGroupName)
        {
            var group = FindOrAddGroup(sGroupName);
            group.Add(info);
            info.Group = group;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public BookGroups()
        {
            Add(new BookInfo("GEN", 50, 1533), BookGroup.Pentateuch);
            Add(new BookInfo("EXO", 40, 1213), BookGroup.Pentateuch);
            Add(new BookInfo("LEV", 27, 859), BookGroup.Pentateuch);
            Add(new BookInfo("NUM", 36, 1288), BookGroup.Pentateuch);
            Add(new BookInfo("DEU", 34, 959), BookGroup.Pentateuch);

            Add(new BookInfo("JOS", 24, 658), BookGroup.Historical);
            Add(new BookInfo("JDG", 21, 618), BookGroup.Historical);
            Add(new BookInfo("RUT", 4, 85), BookGroup.Historical);
            Add(new BookInfo("1SA", 31, 810), BookGroup.Historical);
            Add(new BookInfo("2SA", 24, 695), BookGroup.Historical);
            Add(new BookInfo("1KI", 22, 816), BookGroup.Historical);
            Add(new BookInfo("2KI", 25, 719), BookGroup.Historical);
            Add(new BookInfo("1CH", 29, 942), BookGroup.Historical);
            Add(new BookInfo("2CH", 36, 822), BookGroup.Historical);
            Add(new BookInfo("EZR", 10, 280), BookGroup.Historical);
            Add(new BookInfo("NEH", 13, 406), BookGroup.Historical);
            Add(new BookInfo("EST", 10, 167), BookGroup.Historical);

            Add(new BookInfo("JOB", 42, 1070), BookGroup.Poetical);
            Add(new BookInfo("PSA", 150, 2461), BookGroup.Poetical);
            Add(new BookInfo("PRO", 31, 915), BookGroup.Poetical);
            Add(new BookInfo("ECC", 12, 222), BookGroup.Poetical);
            Add(new BookInfo("SNG", 8, 117), BookGroup.Poetical);

            Add(new BookInfo("ISA", 66, 1292), BookGroup.Prophetic);
            Add(new BookInfo("JER", 52, 1364), BookGroup.Prophetic);
            Add(new BookInfo("LAM", 5, 154), BookGroup.Prophetic);
            Add(new BookInfo("EZK", 48, 1273), BookGroup.Prophetic);
            Add(new BookInfo("DAN", 12, 357), BookGroup.Prophetic);
            Add(new BookInfo("HOS", 14, 197), BookGroup.Prophetic);
            Add(new BookInfo("JOL", 3, 73), BookGroup.Prophetic);
            Add(new BookInfo("AMO", 9, 146), BookGroup.Prophetic);
            Add(new BookInfo("OBA", 1, 21), BookGroup.Prophetic);
            Add(new BookInfo("JON", 4, 48), BookGroup.Prophetic);
            Add(new BookInfo("MIC", 7, 105), BookGroup.Prophetic);
            Add(new BookInfo("NAM", 3, 47), BookGroup.Prophetic);
            Add(new BookInfo("HAB", 3, 56), BookGroup.Prophetic);
            Add(new BookInfo("ZEP", 3, 53), BookGroup.Prophetic);
            Add(new BookInfo("HAG", 2, 38), BookGroup.Prophetic);
            Add(new BookInfo("ZEC", 14, 211), BookGroup.Prophetic);
            Add(new BookInfo("MAL", 4, 55), BookGroup.Prophetic);

            Add(new BookInfo("MAT", 28, 1071), BookGroup.Gospels);
            Add(new BookInfo("MRK", 16, 678), BookGroup.Gospels);
            Add(new BookInfo("LUK", 24, 1151), BookGroup.Gospels);
            Add(new BookInfo("JHN", 21, 879), BookGroup.Gospels);

            Add(new BookInfo("ACT", 28, 1007), BookGroup.Acts);

            Add(new BookInfo("ROM", 16, 433), BookGroup.PaulineLetters);
            Add(new BookInfo("1CO", 16, 437), BookGroup.PaulineLetters);
            Add(new BookInfo("2CO", 13, 257), BookGroup.PaulineLetters);
            Add(new BookInfo("GAL", 6, 149), BookGroup.PaulineLetters);
            Add(new BookInfo("EPH", 6, 155), BookGroup.PaulineLetters);
            Add(new BookInfo("PHP", 4, 104), BookGroup.PaulineLetters);
            Add(new BookInfo("COL", 4, 95), BookGroup.PaulineLetters);
            Add(new BookInfo("1TH", 5, 89), BookGroup.PaulineLetters);
            Add(new BookInfo("2TH", 3, 47), BookGroup.PaulineLetters);
            Add(new BookInfo("1TI", 6, 113), BookGroup.PaulineLetters);
            Add(new BookInfo("2TI", 4, 83), BookGroup.PaulineLetters);
            Add(new BookInfo("TIT", 3, 46), BookGroup.PaulineLetters);
            Add(new BookInfo("PHM", 1, 25), BookGroup.PaulineLetters);

            Add(new BookInfo("HEB", 13, 303), BookGroup.GeneralLetters);
            Add(new BookInfo("JAS", 5, 108), BookGroup.GeneralLetters);
            Add(new BookInfo("1PE", 5, 105), BookGroup.GeneralLetters);
            Add(new BookInfo("2PE", 3, 61), BookGroup.GeneralLetters);
            Add(new BookInfo("1JN", 5, 105), BookGroup.GeneralLetters);
            Add(new BookInfo("2JN", 1, 13), BookGroup.GeneralLetters);
            Add(new BookInfo("3JN", 1, 14), BookGroup.GeneralLetters);
            Add(new BookInfo("JUD", 1, 25), BookGroup.GeneralLetters);

            Add(new BookInfo("REV", 22, 404), BookGroup.Revelation);
        }
        #endregion
    }

}
