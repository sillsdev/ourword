/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    EditTestsCommon.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Common code for the edit tests
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.Edit
{
    public class TestWindow : OWWindow
    {
        #region VAttr{g}: JWritingSystem WSVernacular
        JWritingSystem WSVernacular
        {
            get
            {
                return DB.Project.TargetTranslation.WritingSystemVernacular;
            }
        }
        #endregion
        #region DSection Section
        public DSection Section
        {
            get
            {
                Debug.Assert(null != m_section);
                return m_section;
            }
        }
        DSection m_section;
        #endregion

        #region Constructor(DSection)
        public TestWindow(DSection section)
            : base("TestWindow", 1)
        {
            m_section = section;
        }
        #endregion

        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Load the window
            foreach (DParagraph p in Section.Paragraphs)
            {
                OWPara op = new OWPara( 
                    WSVernacular, 
                    p.Style, 
                    p, 
                    Color.Wheat,
                    OWPara.Flags.IsEditable | OWPara.Flags.CanRestructureParagraphs);
                Contents.Append(op);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
    }

    public class EditTest
    {
        // Public Attrs ----------------------------------------------------------------------
        #region SAttr{g}: DSection Section
        static public DSection Section
        {
            get
            {
                Debug.Assert(null != s_section);
                return s_section;
            }
        }
        static DSection s_section;
        #endregion
        #region SAttr[g}: DBook Book
        static DBook Book
        {
            get
            {
                Debug.Assert(null != s_book);
                return s_book;
            }
        }
        static DBook s_book;
        #endregion
        #region SAttr{g}: OWWindow Wnd
        static public OWWindow Wnd
        {
            get
            {
                Debug.Assert(null != s_Window);
                return s_Window;
            }
        }
        static OWWindow s_Window;
        #endregion
        #region SAttr{g}: Form Form
        static public Form Form
        {
            get
            {
                Debug.Assert(null != s_Form);
                return s_Form;
            }
        }
        static Form s_Form;
        #endregion

        // Setup / TearDown ------------------------------------------------------------------
        #region Helper: void _PreliminarySetup()
        static void _PreliminarySetup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();

            // Application and Project initialization
            OurWordMain.App = new OurWordMain();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
            DB.Project.TargetTranslation = new DTranslation("Test Translation", "Latin", "Latin");
            s_book = new DBook("MRK");
            DB.Project.TargetTranslation.AddBook(s_book);
            G.URStack.Clear();
        }
        #endregion
        #region Helper: void _InitBook(string[] vsRawData)
        static void _InitBook(string[] vsRawData)
        {
            // We'll use a temporary file name to read the data in
            string sPath = JWU.NUnit_TestFileFolder + Path.DirectorySeparatorChar + "TestBook.x";

            // Write out the raw data to this temporary file
            TextWriter W = JW_Util.GetTextWriter(sPath);
            foreach (string s in vsRawData)
                W.WriteLine(s);
            W.Close();

            // Now read it in, with full-blown book parsing mechanism
            s_book.Load(ref sPath, new NullProgress());

            s_section = s_book.Sections[0] as DSection;
        }
        #endregion

        #region SMethod: void Setup(string[] vsRawData)
        static public void Setup(string[] vsRawData)
        {
            // Set up the app, project, translation, etc
            _PreliminarySetup();

            // Load data into a book
            _InitBook(vsRawData);

            // Set up an OWWindow
            s_Window = new TestWindow(s_section);

            // Set up a Form
            s_Form = new Form();
            s_Form.Name = "TestForm";
            s_Form.Controls.Add(s_Window);

            // Load the window
            s_Window.LoadData();
        }
        #endregion
        #region SMethod: void TearDown()
        static public void TearDown()
        {
            DB.Project = null;
            s_Form.Dispose();
            s_Form = null;
        }
        #endregion

    }

}
