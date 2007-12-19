/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgExportBook.cs
 * Author:  John Wimbish
 * Created: 26 July 2007
 * Purpose: Export the current book to, e.g., USFM/Paratext format.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public partial class DialogExportBook : Form
    {
        const string c_sParatextExtension = ".ptx";

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DBook Book - the book to be exported
        DBook Book
        {
            get
            {
                Debug.Assert(null != m_Book);
                return m_Book;
            }
        }
        DBook m_Book;
        #endregion
        #region Attr{g/s}: string ExportPathName - the file name to export to
        public string ExportPathName
        {
            get
            {
                return m_sExportPathName;
            }
            set
            {
                CtrlText_FileName = Path.GetFileName(value);
                m_sExportPathName = value;
            }
        }
        string m_sExportPathName = "";
        #endregion

        // Controls --------------------------------------------------------------------------
        #region Attr{s}: string CtrlText_BookName
        string CtrlText_BookName
        {
            set
            {
                m_labelBookname.Text = value;
            }
        }
        #endregion
        #region Attr{s}: string CtrlText_FileName
        string CtrlText_FileName
        {
            set
            {
                m_labelFilename.Text = value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DBook)
        public DialogExportBook(DBook book)
        {
            InitializeComponent();

            m_Book = book;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

            // Load the information about the book we're about to export
            CtrlText_BookName = Book.Translation.DisplayName + " - " + Book.DisplayName;

            // Default value for the export pathname
            ExportPathName = Path.ChangeExtension(Book.AbsolutePathName, c_sParatextExtension);
        }
        #endregion
        #region Cmd: cmdBrowse
        private void cmdBrowse(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = StrRes.FileFilterExportBook;
            dlg.FilterIndex = 0;
            dlg.InitialDirectory = G.BrowseDirectory;
            dlg.FileName = ExportPathName;
            dlg.OverwritePrompt = false;

            dlg.Title = StrRes.DlgBrowseExportBook_Title;

            if (DialogResult.OK == dlg.ShowDialog(this))
                ExportPathName = dlg.FileName;

            G.BrowseDirectory = Path.GetDirectoryName(dlg.FileName);
        }
        #endregion
        #region Cmd: cmdClosing
        private void cmdClosing(object sender, FormClosingEventArgs e)
        {
            // We're only interested in further processing if the user has hit the OK
            // button, signaling he is done and wishes to save his results.
            if (DialogResult != DialogResult.OK)
                return;

            // If the file already exists, verify that the user wants to overwrite it
            if (File.Exists(ExportPathName))
            {
                if (!Messages.ConfirmFileOverwrite(ExportPathName))
                    DialogResult = DialogResult.Cancel;
            }
        }
        #endregion
        #region Cmd: cmdHelp
        private void cmdHelp(object sender, EventArgs e)
        {
            HelpSystem.Show_DlgExportBook();
        }
        #endregion
    }
}