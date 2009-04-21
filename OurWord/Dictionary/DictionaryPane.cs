/**********************************************************************************************
 * Project: OurWord!
 * File:    DictionaryPane.cs
 * Author:  John Wimbish
 * Created: 02 Feb 2008
 * Purpose: Internals of the Tab Page for displaying the dictionary services
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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
using OurWord.View;
using OurWord.Edit;
using Palaso.Services.Dictionary;
#endregion

namespace OurWord.Edit
{
    public partial class DictionaryPane : UserControl
    {
        #region Attr{g/s}: string CurrentID
        string CurrentID
        {
            get
            {
                return m_sCurrentID;
            }
            set
            {
                m_sCurrentID = value;
            }
        }
        string m_sCurrentID;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DictionaryPane()
        {
            InitializeComponent();

            SetHtmlText("");
            m_checkExactSearch.Checked = true;

            SetControlVisibility();
        }
        #endregion
        #region Method: void SetSize(Size sz)
        public void SetSize(Size sz)
        {
            this.Size = sz;

            // The left/right margin is constant, set as the distance to
            // from the left to the Word label.
            int xMargin = m_lblWord.Location.X;

            // Move the Search button so that it stays to the right
            m_btnSearch.Location = new Point(
                Width - xMargin - m_btnSearch.Size.Width,
                m_btnSearch.Location.Y);
            
            // The Width of the search box fills the available area
            m_textWord.Width = m_btnSearch.Location.X - m_textWord.Location.X;

            // The Open In WeSay button is at the bottom
            m_btnOpenInWeSay.Top = Height - m_btnOpenInWeSay.Height - xMargin;

            // The Height of the Html control fills the entire area
            m_Html.Height = m_btnOpenInWeSay.Top - xMargin - m_Html.Location.Y;

            // The Width of these boxes fill entire available area
            int nAvailableWidth = Width - (2 * xMargin);
            m_Html.Width = nAvailableWidth;
            m_labelDefinition.Width = nAvailableWidth;
            m_textDefinition.Width = nAvailableWidth;
            m_labelExampleSentence.Width = nAvailableWidth;
            m_textExampleSentence.Width = nAvailableWidth;

            // Center the buttons
            int nButtonWidth = m_btnAdd.Width;
            int xButton = (Width - nButtonWidth) / 2;
            m_btnAdd.Left = xButton;
            m_btnCancel.Left = xButton;
            m_btnOpenInWeSay.Left = xButton;
        }
        #endregion

        // Modes and Visibility --------------------------------------------------------------
        enum Modes { kSearch, kEnterNew };
        #region Attr{g}: Modes Mode
        Modes Mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
            }
        }
        Modes m_Mode = Modes.kSearch;
        #endregion
        #region Attr{g}: bool IsSearchMode
        bool IsSearchMode
        {
            get
            {
                return (Mode == Modes.kSearch);
            }
        }
        #endregion
        #region Method: void SetControlVisibility()
        void SetControlVisibility()
        {
            // Controls for Search
            m_Html.Visible = IsSearchMode;
            m_btnOpenInWeSay.Visible = IsSearchMode;

            // Constrols for EnterNew
            m_labelDefinition.Visible = !IsSearchMode;
            m_textDefinition.Visible = !IsSearchMode;
            m_labelExampleSentence.Visible = !IsSearchMode;
            m_textExampleSentence.Visible = !IsSearchMode;
            m_btnAdd.Visible = !IsSearchMode;
            m_btnCancel.Visible = !IsSearchMode;

            // Enabling (for ones that stay on all the time)
            m_textWord.Enabled = IsSearchMode;
            m_checkExactSearch.Enabled = IsSearchMode;
        }
        #endregion
        #region Cmd: cmdAddNewWord
        void cmdAddNewWord(object sender, EventArgs e)
        {
            Mode = Modes.kEnterNew;
            SetControlVisibility();
        }
        #endregion
        #region Cmd: cmdCancelNewEntry
        private void cmdCancelNewEntry(object sender, EventArgs e)
        {
            Mode = Modes.kSearch;
            SetControlVisibility();
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Method: void SetHtmlText(string sHtmlEntry)
        void SetHtmlText(string sHtmlEntry)
        {
            // Get the Hex string representation of the pane's background color
            string sColor = JWU.ColorToHexString( this.BackColor );
            string sBody = "<body BGCOLOR=\"#" + sColor + "\">";

            // Locate the <body> string, as we want to insert our color there
            string sFind = "<body>";
            int iBody = sHtmlEntry.IndexOf(sFind);

            // If not found, then we assume an empty string
            if (-1 == iBody)
            {
                string sEmpty = "<html>" + sBody + "</body></html>";
                m_Html.DocumentText = sEmpty;
                return;
            }

            // Parse the string we were given into before and after the <body> parts
            string sLeft = sHtmlEntry.Substring(0, iBody);
            string sRight = sHtmlEntry.Substring(iBody + sFind.Length);

            // Reassemble the new string
            string s = sLeft + sBody + sRight;
            m_Html.DocumentText = s;
        }
        #endregion

        // Dictionary Operations -------------------------------------------------------------
        #region Cmd: cmdLookupWord - response to the Search WeSay button
        private void cmdLookupWord(object sender, EventArgs e)
        {
            // Retrieve the word to look up from the Word control; if nothing there,
            // then there's nothing to do.
            string sWord = m_textWord.Text;
            if (string.IsNullOrEmpty(sWord))
                return;

            // Determine the type of search desired
            FindMethods method = FindMethods.Exact;
            if (m_checkExactSearch.Checked == false)
                method = FindMethods.DefaultApproximate;

            Dictionary.Item[] vItems = DB.Project.Dictionary.GetMatchingEntries(sWord, method);

            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem miAdd = new ToolStripMenuItem("Not found; Add this word...");
            miAdd.Click += new EventHandler(cmdAddNewWord);
            menu.Items.Add(miAdd);

            if (vItems.Length > 0)
            {
                menu.Items.Add(new ToolStripSeparator());

                foreach (Dictionary.Item item in vItems)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem(item.Form);
                    mi.Tag = item.ID;
                    mi.Click += new EventHandler(cmdShowWord); 
                    menu.Items.Add(mi);
                }
            }
            else
            {
                ToolStripMenuItem miCancel = new ToolStripMenuItem("Cancel this search");
                menu.Items.Add(miCancel);
            }

            // Show the menu
            int x = 0;
            int y = m_btnSearch.Height;
            menu.Show(m_btnSearch, new Point(x, y));
        }
        #endregion
        #region Cmd: cmdShowWord - after choosing word from popup menu, displays its html defn
        private void cmdShowWord(object sender, EventArgs e)
        {
            // Retrieve the ID of the word we'll be looking up; it is the Tag
            // in the menu item.
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
            {
                SetHtmlText("");
                return;
            }
            CurrentID = item.Tag as string;

            // Make the call to the Dictionary to retrieve the entry
            string sHtml = DB.Project.Dictionary.GetHtmlForEntry(CurrentID);

            // Place it into the Html control
            SetHtmlText(sHtml);
        }
        #endregion
        #region Cmd: cmdAddToDictionary
        private void cmdAddToDictionary(object sender, EventArgs e)
        {
            // Add the word to the Dictionary
            CurrentID = DB.Project.Dictionary.AddEntry(
                m_textWord.Text,
                m_textDefinition.Text,
                m_textExampleSentence.Text);

            // Switch back to Search mode
            Mode = Modes.kSearch;
            SetControlVisibility();

            // Make the call to the Dictionary to retrieve the entry in html form
            string sHtml = DB.Project.Dictionary.GetHtmlForEntry(CurrentID);

            // Place it into the Html control
            SetHtmlText(sHtml);
        }
        #endregion
        #region Cmd: cmdOpenInDictionary
        private void cmdOpenInDictionary(object sender, EventArgs e)
            // If the CurrentID is null/empty, WeSay will just open without going
            // directly to it.
        {
            DB.Project.Dictionary.JumpToEntry(CurrentID);
        }
        #endregion
    }


}
