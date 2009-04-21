/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizPage_GetAbbreviation.cs
 * Author:  John Wimbish
 * Created: 1 Feb 2007
 * Purpose: User indentifies which book (supplies the abbreviation)
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
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
using JWdb.DataModel;
#endregion

namespace OurWord.Dialogs.WizImportBook
{
    public partial class WizPage_GetAbbreviation : UserControl, IJW_WizPage
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
        #region Attr{g}: DTranslation Translation
        public DTranslation Translation
        {
            get
            {
                Debug.Assert(null != m_Translation);
                return m_Translation;
            }
        }
        DTranslation m_Translation = null;
        #endregion

        // Books ListView Control ------------------------------------------------------------
        #region Attr{g}: ListView Books
        ListView Books
        {
            get
            {
                return m_lvBooks;
            }
        }
        #endregion
        #region Method: void InitializeBooks()
        void InitializeBooks()
        {
            // Make certain there is nothing in there currently
            Books.Items.Clear();

            // Query the translation to see what BookAbbrevs it permits to be
            // imported. This may not be all 66 books, as it will exclude, e.g.,
            // books that have already been imported.
            string[] vsEligibleBookAbbrevs = Translation.EligibleNewBookAbbrevs;

            // Add each item to the list view
            foreach(string sAbbrev in vsEligibleBookAbbrevs)
            {
                // Create a list view item and set the first column to the book abbrev
                ListViewItem lvi = new ListViewItem(sAbbrev);

                // The second column is the book name
                int i = DBook.FindBookAbbrevIndex(sAbbrev);
                if (-1 != i)
                {
                    string sTitle = Translation.BookNamesTable[i];
                    Debug.Assert(null != sTitle);
                    lvi.SubItems.Add(sTitle);
                }

                // Add the item
                Books.Items.Add(lvi);
            }
        }
        #endregion
        #region Attr{g}: string SelectedBookAbbrev - the book's abbreviation
        public string SelectedBookAbbrev
        {
            get
            {
                if (1 == Books.SelectedItems.Count)
                {
                    return Books.SelectedItems[0].Text;
                }
                return null;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DTranslation, string sWhichBook)
        public WizPage_GetAbbreviation(DTranslation _translation, string sWhichBook)
        {
            InitializeComponent();

            // Store the translation for further reference
            m_Translation = _translation;

            // Label: "Which book are you importing / creating?"
            m_labelWhichBook.Text = sWhichBook;
        }
        #endregion
        #region Attr{g}: Control[] vExclude
        public Control[] vExclude
        {
            get
            {
                return new Control[] { m_labelWhichBook };
            }
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            // If there is only one item, then go ahead and select it
            if (Books.Items.Count == 1)
            {
                Books.Items[0].Selected = true;
            }

            if (CanGoToNextPage())
                Wizard.PlaceFocusOnAdvanceButton();
            else
                Books.Focus();
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            if (null == SelectedBookAbbrev)
                return false;
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strNavTitle", "Identify the Book", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kImportBook);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Initialize the listview
            InitializeBooks();
        }
        #endregion
        #region Cmd: cmdBookSelected - a selection has been made in the Books ListView
        private void cmdBookSelected(object sender, EventArgs e)
        {
            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
        }
        #endregion
    }
}
