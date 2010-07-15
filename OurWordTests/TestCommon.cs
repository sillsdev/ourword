#region ***** TestCommon.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    TestCommon.cs
 * Author:  John Wimbish
 * Created: 22 Aug 2009
 * Purpose: Stuff that testing subclasses might find convenient to use
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using JWTools;
using OurWord;
using OurWordData.DataModel;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWordTests
{
    public class TestCommon
    {
        #region Constructor()
        public TestCommon()
        {
        }
        #endregion
        #region SMethod: void InitializeRegistry()
        public static void InitializeRegistry()
            // Needed, e.g., by Map and RootFolderPath
        {
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";
        }
        #endregion
        #region Method: void GlobalTestSetup()
        public static void GlobalTestSetup()
        {
            InitializeRegistry();

            // Localization DB
            LocDB.Initialize(Loc.FolderOfLocFiles);

            // Set the resource location
            JWU.ResourceLocation = "OurWord.Res.";
        }
        #endregion

        #region Method: DTranslation CreateHierarchyThroughTargetTranslation()
        public static DTranslation CreateHierarchyThroughTargetTranslation()
        {
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DTeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;
            return Translation;
        }
        #endregion
        #region Method: DBook CreateHierarchyThroughTargetBook(sBookAbbrev)
        public static DBook CreateHierarchyThroughTargetBook(string sBookAbbrev)
        {
            var translation = CreateHierarchyThroughTargetTranslation();
            var book = new DBook(sBookAbbrev);
            translation.AddBook(book);
            return book;
        }
        #endregion
        #region Method: DSection CreateHierarchyThroughTargetSection(sBookAbbrev)
        public DSection CreateHierarchyThroughTargetSection(string sBookAbbrev)
        {
            var book = CreateHierarchyThroughTargetBook(sBookAbbrev);
            var section = new DSection();
            book.Sections.Append(section);
            return section;
        }
        #endregion
        #region Method: DParagraph CreateHierarchyThroughTargetParagraph(sBookAbbrev, sSimpleText)
        public DParagraph CreateHierarchyThroughTargetParagraph(string sBookAbbrev, string sSimpleText)
        {
            var section = CreateHierarchyThroughTargetSection(sBookAbbrev);
            var paragraph = new DParagraph(StyleSheet.Paragraph);
            section.Paragraphs.Append(paragraph);
            paragraph.SimpleText = sSimpleText;
            return paragraph;
        }
        #endregion
    }

    public class TestFolder
    {
        #region Attr{g}: string RootFolderPath
        static public string RootFolderPath
        {
            get
            {
                TestCommon.InitializeRegistry();
                return JW_Registry.GetValue("NUnit_LocDbDir",
                    "C:\\Users\\JWimbish\\Documents\\Visual Studio 2008\\Projects\\" +
                    "ourword\\OurWord\\bin\\Debug\\Testing");
            }
        }
        #endregion

        #region SMethod: string CreateEmpty()
        static public string CreateEmpty()
        {
            var sRootFolder = RootFolderPath;
            Debug.Assert(!string.IsNullOrEmpty(sRootFolder));

            DeleteIfExists();

            Directory.CreateDirectory(sRootFolder);
            Debug.Assert(Directory.Exists(sRootFolder), "Unable to create \"Testing\" folder.");

            return sRootFolder;
        }
        #endregion
        #region SMethod: string CreateEmptySubFolder(string sSubFolder)
        static public string CreateEmptySubFolder(string sSubFolder)
        {
            // Make sure we have the owning root folder; if we already do, we 
            // leave it (rather than creating an empty one)
            if (!Directory.Exists(RootFolderPath))
                CreateEmpty();

            var sSubFolderPath = Path.Combine(RootFolderPath, sSubFolder);

            if (Directory.Exists(sSubFolderPath))
                JWU.SafeFolderDelete(sSubFolderPath);
            Debug.Assert(!Directory.Exists(sSubFolderPath),
                "Unable to delete testing Subfolder: " + sSubFolder);

            Directory.CreateDirectory(sSubFolderPath);
            Debug.Assert(Directory.Exists(sSubFolderPath),
                "Unable to create testing subfolder: " + sSubFolder);
            return sSubFolderPath;
        }
        #endregion
        #region SMethod: void DeleteIfExists()
        static public void DeleteIfExists()
        {
            var sRootFolder = RootFolderPath;
            if (Directory.Exists(sRootFolder))
                JWU.SafeFolderDelete(sRootFolder);

            // Deletion will fail if, e.g., Windows Explorer, or a command line window, is
            // open in the folder
            Debug.Assert(!Directory.Exists(sRootFolder), "Unable to delete \"Testing\" folder.");
        }
        #endregion
        #region SMethod: void DeleteSubfolderIfExists(string sSubFolder)
        static public void DeleteSubfolderIfExists(string sSubFolder)
        {
            var sPath = Path.Combine(RootFolderPath, sSubFolder);

            if (Directory.Exists(sPath))
                JWU.SafeFolderDelete(sPath);

            // Deletion will fail if, e.g., Windows Explorer, or a command line window, is
            // open in the folder
            Debug.Assert(!Directory.Exists(sPath), "Unable to delete subfolder.");
        }
        #endregion
        #region SMethod: void EnsurePathExists(string sPathWithPossibleFileName)
        static public void EnsurePathExists(string sPathWithPossibleFileName)
        {
            // No guarantees the path doesn't include a filename
            var sPath = Path.GetDirectoryName(sPathWithPossibleFileName);

            if (!Directory.Exists(RootFolderPath))
                CreateEmpty();

            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);

            Debug.Assert(Directory.Exists(sPath), "Unable to create path: " + sPath);
        }
        #endregion
    }

    public class TestFile
    {
        #region Attr{g}: string FullPath
        public string FullPath
        {
            get 
            {
                Debug.Assert(!string.IsNullOrEmpty(m_FullPath));
                return m_FullPath; 
            }
        }
        private readonly string m_FullPath;
        #endregion
        private readonly string m_FileName;

        #region Constructor(sSubFolder, sFileName)
        public TestFile(string sSubFolder, string sFileName)
        {
            m_FileName = sFileName;

            var sFolder = (string.IsNullOrEmpty(sSubFolder)) ?
                TestFolder.RootFolderPath :
                Path.Combine(TestFolder.RootFolderPath, sSubFolder);

            m_FullPath = Path.Combine(sFolder, m_FileName);
        }
        #endregion

        #region Method: void CreateAndWrite(string sContents)
        public void CreateAndWrite(string sContents)
        {
            // Create the test folder & any subfolder if it isn't there already
            TestFolder.EnsurePathExists(m_FullPath);

            var w = GetTextWriter();
            w.WriteLine(sContents);
            w.Close();
        }
        #endregion
        #region Method: void CreateAndWrite(string[] vsContents)
        public void CreateAndWrite(string[] vsContents)
        {
            // Create the test folder & any subfolder if it isn't there already
            TestFolder.EnsurePathExists(m_FullPath);

            var w = GetTextWriter();

            if (null != vsContents)
            {
                foreach (var s in vsContents)
                    w.WriteLine(s);
            }

            w.Close();
        }
        #endregion
        #region Method: TextWriter GetTextWriter()
        public TextWriter GetTextWriter()
        {
            // Delete any existing file. We were having some sort of windows
            // read-only problems, and deleting the file first seems to fix it.
            try
            {
                if (File.Exists(m_FullPath))
                    File.Delete(m_FullPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Debug.Assert(!File.Exists(m_FullPath), "Unable to delete file");

            var w = new StreamWriter(m_FullPath, false, Encoding.UTF8);
            var tw = TextWriter.Synchronized(w);
            return tw;
        }
        #endregion
        #region Method: List<string> Read()
        public List<string> Read()
        {
            return JWU.ReadFile(m_FullPath);
        }
        #endregion
    }

}
