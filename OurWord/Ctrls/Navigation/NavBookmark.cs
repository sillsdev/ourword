using System;
using System.Collections.Generic;
using JWTools;
using OurWord.Edit;
using OurWordData.DataModel;

namespace OurWord.Ctrls.Navigation
{
    public class NavBookmark : LookupInfo
    {
        // Attrs -----------------------------------------------------------------------------
        private readonly string ProjectName;
        public readonly string ProjectPath;
        public readonly string LayoutName;

        // Virtual Attrs ---------------------------------------------------------------------
        #region VAttr{g}: string ChapterVerse
        string ChapterVerse
        {
            get
            {
                return string.Format("{0}:{1}", Chapter, Verse);
            }
        }
        #endregion
        #region VAttr{g}: string MenuText
        public string MenuText
        {
            get
            {
                return string.Format("{0} {1} {2} ({3})", ProjectName, BookAbbrev,
                    ChapterVerse, LayoutName);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(selection)
        public NavBookmark(OWWindow.Sel selection)
            : base(selection.Phrases, selection.DBT_iCharFirst, 
            selection.SelectionString.Length)
        {
            ProjectName = DB.Project.DisplayName;
            ProjectPath = DB.Project.StoragePath;
            LayoutName = G.App.CurrentLayout.Name;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        private const string c_sTag = "nav";
        private const string c_sProjectName = "proj";
        private const string c_sProjectPath = "path";
        private const string c_sLayoutName = "layout";
        #endregion
        #region Method: string ToXmlSaveString()
        public string ToXmlSaveString()
        {
            var doc = new XmlDoc();
            var node = doc.AddNode(doc, c_sTag);
            doc.AddAttr(node, c_sProjectName, ProjectName);
            doc.AddAttr(node, c_sProjectPath, ProjectPath);
            doc.AddAttr(node, c_sLayoutName, LayoutName);

            Save(doc, node);

            return doc.OuterXml;
        }
        #endregion
        #region Constructor(string sXml)
        public NavBookmark(string sXml)
        {
            try
            {
                var doc = new XmlDoc();
                doc.LoadXml(sXml);
                var node = XmlDoc.FindNode(doc, c_sTag);

                ProjectName = XmlDoc.GetAttrValue(node, c_sProjectName);
                ProjectPath = XmlDoc.GetAttrValue(node, c_sProjectPath);
                LayoutName = XmlDoc.GetAttrValue(node, c_sLayoutName);

                Read(node);           
            }
            catch (Exception)
            {
                throw new Exception("Bad NavBookmark data in registry");
            }
        }
        #endregion
    }

    public class BookmarkList : List<NavBookmark>
    {
        // I/O -------------------------------------------------------------------------------
        private const string c_sRegKey = "NavBookmarks";
        #region Method: void Save()
        public void Save()
        {
            JW_Registry.DeleteSubKey(c_sRegKey);
            var i = 1;
            foreach (var bookmark in this)
            {
                JW_Registry.SetValue(c_sRegKey, i.ToString("00"), bookmark.ToXmlSaveString());
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
            foreach (var sName in vsNames)
            {
                var sXmlData = JW_Registry.GetValue(c_sRegKey, sName, "");
                var bookmark = new NavBookmark(sXmlData);
                Add(bookmark);
            }
        }
        #endregion
    }
}
