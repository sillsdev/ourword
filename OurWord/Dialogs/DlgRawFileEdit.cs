#region ***** DlgRawFileEdit.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgRawFileEdit.cs
 * Author:  John Wimbish
 * Created: 23 May 2009
 * Purpose: The advanced user can edit the standard format directly 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class DlgRawFileEdit : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DBook Book
        DBook Book
        {
            get
            {
                return m_Book;
            }
        }
        DBook m_Book;
        #endregion

        // Dialog Controls -------------------------------------------------------------------
    	#region Attr{g}: RichTextBox DataFile
		RichTextBox DataFile
		{
			get
			{
				return m_rtfText;
			}
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DBook)
        public DlgRawFileEdit(DBook book)
        {
            InitializeComponent();
            m_Book = book;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // The file name we're editing
            Text = Text + " - " + Book.StorageFileName;

            // Font
            JParagraphStyle pstyle = DB.TeamSettings.StyleSheet.FindParagraphStyleOrNormal(
                DStyleSheet.c_sfmParagraph);
            DataFile.Font = pstyle.CharacterStyle.FindOrAddFontForWritingSystem(
                Book.Translation.WritingSystemVernacular).DefaultFont;

            // Hanging indent so the lines are easy to see
            DataFile.SelectionHangingIndent = 20;
            DataFile.WordWrap = true;

            // Read in the file and place it in the control
            DataFile.Clear();
            DataFile.Lines = LoadFile(Book.StoragePath);

            DataFile.SelectAll();
            DataFile.SelectionHangingIndent = 20;
            DataFile.Select(0, 0);

        }
        #endregion
        #region Cmd: cmdClosing
        private void cmdClosing(object sender, FormClosingEventArgs e)
        {
            // User canceled
            if (DialogResult != DialogResult.OK)
                return;

            // Save the changes
            string[] v = DataFile.Lines;
            TextWriter w = JW_Util.GetTextWriter(Book.StoragePath);
            foreach (string s in v)
                w.WriteLine(s);
            w.Close();
        }
        #endregion
        #region Cmd: cmdFindBoxKeyDown
        private void cmdFindBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Text to search for
            string sSearchText = m_textSearch.Text;
            if (string.IsNullOrEmpty(sSearchText))
                return;

            // Is this the undecorated Enter key?
            if (e.Modifiers != Keys.None || e.KeyCode != Keys.Enter)
                return;

            // Get the current selection position
            int iPosStart = DataFile.SelectionStart + DataFile.SelectionLength;

            // Find it
            int iPos = DataFile.Text.IndexOf(sSearchText, iPosStart);
            if (-1 == iPos)
                return;

            // Select it
            DataFile.SelectionStart = iPos;
            DataFile.SelectionLength = sSearchText.Length;

            e.Handled = true;
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: string[] LoadFile(string sPathName)
        protected string[] LoadFile(string sPathName)
        // Loads the file into the RichTextBox. We can't use the RTF Box's LoadFile,
        // because it appears to want to munge up the UTF8's. 
        {
            // Get a TextReader to the file
            StreamReader sr = new StreamReader(sPathName, Encoding.UTF8);
            TextReader r = TextReader.Synchronized(sr);

            // Read in as a standard format database; this will concatenate lines
            var db = new ScriptureDB();
            db.Read(r);
            r.Close();

            // Return it as an array of strings
            return db.ExtractData();
        }
        #endregion
    }
}
