using JWTools;
using OurWord.Edit;
using OurWordData.DataModel;

namespace OurWord.Ctrls.Navigation
{
    public class NavBookmark : LookupInfo
    {
        // Attrs -----------------------------------------------------------------------------
        public readonly string ProjectName;
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

        public void GoTo()
        {
            


        }

        // I/O -------------------------------------------------------------------------------
        /*
        public string Save()
        {
            var doc = new XmlDoc();

        }
        */


    }
}
