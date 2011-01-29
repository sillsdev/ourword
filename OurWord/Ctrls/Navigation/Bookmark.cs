using System;
using System.Collections.Generic;
using JWTools;

namespace OurWord.Ctrls.Navigation
{
    public class Bookmark
    {
        public readonly string ProjectName;
        public readonly string ProjectPath;
        public readonly string LayoutName;
        public readonly string BookAbbrev;
        public readonly int SectionNo;
        public readonly string ChapterVerse;

        #region Constructor(sProjectPath, sLayoutName, sBookAbbrev, iSectionNo)
        public Bookmark(string sProjectName, string sProjectPath, string sLayoutName, 
            string sBookAbbrev, int iSectionNo, string sChapterVerse)
        {
            ProjectName = sProjectName;
            ProjectPath = sProjectPath;
            LayoutName = sLayoutName;
            BookAbbrev = sBookAbbrev;
            SectionNo = iSectionNo;
            ChapterVerse = sChapterVerse;
        }
        #endregion

        #region Attr{g}: string MenuText
        public string MenuText
        {
            get
            {
                return string.Format("{0} {1} {2} ({3})", ProjectName, BookAbbrev, 
                    ChapterVerse, LayoutName);
            }
        }
        #endregion

        #region Method: string Save()
        public string Save()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", ProjectName, LayoutName, 
                BookAbbrev, SectionNo, ChapterVerse, ProjectPath);
        }
        #endregion
        #region SMethod: Bookmark Create(sData)
        static public Bookmark Create(string sData)
        {
            try
            {
                var sParts = sData.Split(new[] {", "}, StringSplitOptions.None);

                var sProjectName = sParts[0];
                var sLayoutName = sParts[1];
                var sBookAbbrev = sParts[2];
                var sSectionNo = sParts[3];
                var sChapterVerse = sParts[4];
                var sProjectPath = sParts[5];

                return new Bookmark(sProjectName, sProjectPath, sLayoutName, sBookAbbrev,
                    Convert.ToInt16(sSectionNo), sChapterVerse);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }


    public class Bookmarks : List<Bookmark>
    {
        // Save/Read from Registry -----------------------------------------------------------
        private const string c_sRegKey = "Bookmarks";
        #region Method: void Save()
        public void Save()
        {
            JW_Registry.DeleteSubKey(c_sRegKey);
            var i = 1;
            foreach (var bookmark in this)
            {
                JW_Registry.SetValue(c_sRegKey, i.ToString("00"), bookmark.Save());
                i++;
            }
        }
        #endregion
        #region method: void Read()
        public void Read()
        {
            Clear();

            var key = JW_Registry.OpenRegistryKey(c_sRegKey);
            var vsNames = key.GetValueNames();
            foreach(var sName in vsNames)
            {
                var sData = JW_Registry.GetValue(c_sRegKey, sName, "");
                var bookmark = Bookmark.Create(sData);
                if (null != bookmark)
                    Add(bookmark);
            }
        }
        #endregion
    }


}
