/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizCreateBook\WizPage_GetDestinationFolder.cs
 * Author:  John Wimbish
 * Created: 28 Jun 2008
 * Purpose: Retrieves the destination for this new book.
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

namespace OurWord.Dialogs.WizCreateBook
{
    public partial class WizPage_GetDestinationFolder : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: JW_Wizard Wizard - the owning wizard
        JW_Wizard Wizard
        {
            get
            {
                Debug.Assert(null != Parent as JW_Wizard);
                return Parent as JW_Wizard;
            }
        }
        #endregion
        #region Attr{s}: string BookFolder - ensures an ellipses if the pathname is too long
        public string BookFolder
        {
            get
            {
                return m_sPathName;
            }
            set
            {
                m_sPathName = value;

                m_labelFolderName.Text = "";
                if (!string.IsNullOrEmpty(value))
                    m_labelFolderName.Text = JWU.PathEllipses(value, 38);
            }
        }
        private string m_sPathName = "";
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizPage_GetDestinationFolder()
        {
            InitializeComponent();
        }
        #endregion
        #region Attr{g}: Control[] vExclude
        public Control[] vExclude
        {
            get
            {
                return new Control[] { m_labelFolderName, m_labelBook };
            }
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            // Place the name of the book on the screen for reference
            WizCreateBook wiz = Wizard as WizCreateBook;
            Debug.Assert(null != wiz);
            DBook book = wiz.FrontBook;
            Debug.Assert(null != book);
            m_labelBook.Text = book.BookAbbrev + " - " + 
                book.DisplayName;

            // Place focus on the browse button
            if (string.IsNullOrEmpty(BookFolder))
            {
                m_btnBrowseFolder.Focus();
                m_labelPressFinish.Visible = false;
            }
            else
            {
                m_labelPressFinish.Visible = true;
                Wizard.PlaceFocusOnAdvanceButton();
            }
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            if (string.IsNullOrEmpty(m_labelFolderName.Text))
                return false;
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strNavTitle", "Destination Folder", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kNewBook);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdBrowse
        private void cmdBrowse(object sender, EventArgs e)
        {
            // We are creating a file, and thus all we need is the folder
            // to put it in (the filename is generated from the Stage-Version-Minor
            // settings.
            FolderBrowserDialog dlgFolder = new FolderBrowserDialog();
            dlgFolder.Description = DlgBookPropsRes.BrowseFolderDescription;

            // Default to the Data Root folder; if we've browsed before, go to that
            // folder we last browsed to.
            dlgFolder.RootFolder = Environment.SpecialFolder.MyComputer;
            dlgFolder.SelectedPath = G.BrowseDirectory;

            if (DialogResult.OK == dlgFolder.ShowDialog())
            {
                BookFolder = dlgFolder.SelectedPath;
                G.BrowseDirectory = dlgFolder.SelectedPath;
                m_labelPressFinish.Visible = true;
                Wizard.AdvanceButtonEnabled = CanGoToNextPage();
                Wizard.PlaceFocusOnAdvanceButton();
            }
        }
        #endregion
    }
}
