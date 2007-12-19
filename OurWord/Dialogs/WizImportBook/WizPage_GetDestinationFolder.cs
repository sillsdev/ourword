/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizPage_GetDestinationFolder.cs
 * Author:  John Wimbish
 * Created: 13 Feb 2007
 * Purpose: User indentifies the folder for where the book will be saved.
 * Legal:   Copyright (c) 2003-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Dialogs.WizImportBook
{
    public partial class WizPage_GetDestinationFolder : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: WizImportBook Wizard - the owning wizard
        WizImportBook Wizard
        {
            get
            {
                Debug.Assert(null != Parent as WizImportBook);
                return Parent as WizImportBook;
            }
        }
        #endregion
        #region VAttr{g}: string ImportFileName
        string ImportFileName
        {
            get
            {
                return Wizard.ImportFileName;
            }
        }
        #endregion
        #region Attr{g/s}: string DestinationFolder
        public string DestinationFolder
        {
            get
            {
                return m_sDestinationFolder;
            }
            set
            {
                m_sDestinationFolder = value;
                m_labelFolder.Text = JWU.PathEllipses(value, 40);
            }
        }
        string m_sDestinationFolder = "";
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            // Calculate and display the the file name
            string sFileName = DBook.ComputePathName(
                Wizard.Translation.LanguageAbbrev,
                Wizard.BookAbbrev,
                Wizard.Stage,
                Wizard.Version,
                "",
                false);
            m_labelFileName.Text = sFileName;

            // Display the current path
            DestinationFolder = Path.GetDirectoryName(ImportFileName);

        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strNavTitle", "Where to Store?", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.Show_WizImportBook_WhereToStore();
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizPage_GetDestinationFolder()
        {
            InitializeComponent();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Command: cmdBrowseFolder
        private void cmdBrowseFolder(object sender, EventArgs e)
        {
            // Set up a Browse Folder dialog
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            // Tell the user what we're browsing for
            dlg.Description = DlgBookPropsRes.BrowseFolderDescription;

            // Default to the Data Root folder; if we've browsed before, go to 
            // that most recent folder
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            dlg.SelectedPath = Path.GetDirectoryName(ImportFileName);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                DestinationFolder = dlg.SelectedPath;
                G.BrowseDirectory = dlg.SelectedPath;
            }

        }
        #endregion

    }
}
