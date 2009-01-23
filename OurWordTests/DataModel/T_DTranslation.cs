/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DTranslation.cs
 * Author:  John Wimbish
 * Created: 08 July 2008
 * Purpose: Tests the DTranslation class
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DTranslation
    {
        // Helper Methods --------------------------------------------------------------------
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();

            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.EnsureInitialized();
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: ConvertsCrossReferences
        [Test] public void ConvertsCrossReferences()
        // Test the conversion from a Source cross-reference to a Target cross-reference.
        // We want to see that 
        // - only those things that are in the Source get changed,
        // - only whole words in the string get changed (not partial matches)
        {
            G.Project.FrontTranslation = new DTranslation("Front", "Latin", "Latin");
            G.Project.TargetTranslation = new DTranslation("Target", "Latin", "Latin");

            DBook book = new DBook();
            G.Project.FrontTranslation.Books.Append(book);
            DSection section = new DSection(1);
            book.Sections.Append(section);
            DParagraph para = new DParagraph();
            section.Paragraphs.Append(para);

            G.Project.FrontTranslation.BookNamesTable.Clear();
            G.Project.FrontTranslation.BookNamesTable.Append("Genesis");
            G.Project.FrontTranslation.BookNamesTable.Append("Exodus");
            G.Project.FrontTranslation.BookNamesTable.Append("Ge");
            G.Project.FrontTranslation.BookNamesTable.Append("2 Kings");
            G.Project.FrontTranslation.BookNamesTable.Append("Song of Songs");
            G.Project.FrontTranslation.BookNamesTable.Append("2 John");
            G.Project.FrontTranslation.BookNamesTable.Append("Carita (Mula-Mula)");

            G.Project.TargetTranslation.BookNamesTable.Clear();
            G.Project.TargetTranslation.BookNamesTable.Append("Kejadian");
            G.Project.TargetTranslation.BookNamesTable.Append("Keluaran");
            G.Project.TargetTranslation.BookNamesTable.Append("Imamat");
            G.Project.TargetTranslation.BookNamesTable.Append("2 Raja-Raja");
            G.Project.TargetTranslation.BookNamesTable.Append("Kidung Agung");
            G.Project.TargetTranslation.BookNamesTable.Append("2 Yohanes");
            G.Project.TargetTranslation.BookNamesTable.Append("Kejadian");

            string sSource = "(Genesis 3:4; Exodus 12:4, 3; Ex 3:4, " +
                "Genesissy 23:4; 2 Kings 13:3; Carita (Mula-Mula) 5:5, 23";
            string sExpected = "(Kejadian 3:4; Keluaran 12:4, 3; Ex 3:4, " +
                "Genesissy 23:4; 2 Raja-Raja 13:3; Kejadian 5:5, 23";
            para.SimpleText = sSource;

            string sActual = G.Project.TargetTranslation.ConvertCrossReferences(para);

            Assert.AreEqual(sExpected, sActual);
        }
        #endregion


    }
}
