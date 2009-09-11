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
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;
using JWdb.DataModel;
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

        #region Method: DTranslation CreateHierarchyThroughTargetTranslation()
        public DTranslation CreateHierarchyThroughTargetTranslation()
        {
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DTranslation Translation = new DTranslation("Translation", "Latin", "Latin");
            DB.Project.TargetTranslation = Translation;
            return Translation;
        }
        #endregion
        #region Method: DBook CreateHierarchyThroughTargetBook(sBookAbbrev)
        public DBook CreateHierarchyThroughTargetBook(string sBookAbbrev)
        {
            var translation = CreateHierarchyThroughTargetTranslation();
            var book = new DBook(sBookAbbrev);
            translation.Books.Append(book);
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
            var paragraph = new DParagraph();
            section.Paragraphs.Append(paragraph);
            paragraph.SimpleText = sSimpleText;
            return paragraph;
        }
        #endregion
    }
}
