/**********************************************************************************************
 * Project: OurWord!
 * File:    DictionaryPane.cs
 * Author:  John Wimbish
 * Created: 02 Feb 2008
 * Purpose: Internals of the Tab Page for displaying the dictionary services
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using OurWord.View;
using OurWord.Edit;
using Palaso.Services.Dictionary;
using NUnit.Framework;
#endregion

namespace OurWord.Edit
{
    public partial class DictionaryPane : UserControl
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DictionaryPane()
        {
            InitializeComponent();

            SetHtmlText("");
            m_checkExactSearch.Checked = true;
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

            // The Width of the Html control fills the entire area
            m_Html.Width = Width - (2 * xMargin);

            // The Height of the Html control fills the entire area
            m_Html.Height = Height - xMargin - m_Html.Location.Y;
        }
        #endregion



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

            Dictionary.Item[] vItems = G.Project.Dictionary.GetMatchingEntries("v", sWord, method);

            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem miAdd = new ToolStripMenuItem("Not found; Add this word...");
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
            string sID = item.Tag as string;

            // Make the call to the Dictionary to retrieve the entry
            string sHtml = G.Project.Dictionary.GetHtmlForEntry(sID);

            // Place it into the Html control
            SetHtmlText(sHtml);
        }
        #endregion
    }


    // en, zna


    public class Dictionary
    {
        // Represents a word in the dictionary
        #region CLASS: Item
        public class Item
        {
            #region Attr{g}: string ID
            public string ID
            {
                get
                {
                    return m_sID;
                }
            }
            string m_sID;
            #endregion
            #region Attr{g} string Form
            public string Form
            {
                get
                {
                    return m_sForm;
                }
            }
            string m_sForm;
            #endregion

            #region Constructor(sID, sForm)
            public Item(string sID, string sForm)
            {
                m_sID = sID;
                m_sForm = sForm;
            }
            #endregion
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DictionaryAccessor DictionaryAccessor
        DictionaryAccessor DictionaryAccessor
        {
            get
            {
                return m_DictionaryAccessor;
            }
        }
        DictionaryAccessor m_DictionaryAccessor = null;
        #endregion
        #region Attr{g}: DProject Project
        DProject Project
        {
            get
            {
                Debug.Assert(null != m_Project);
                return m_Project;
            }
        }
        DProject m_Project;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DProject)
        public Dictionary(DProject _project)
        {
            m_Project = _project;
            Initialize();
        }
        #endregion
        #region Destructor()
        ~Dictionary()
        {
            Dispose();
        }
        #endregion
        #region Method: void Dispose()
        public void Dispose()
        {
            if (null != m_DictionaryAccessor)
            {
                m_DictionaryAccessor.Dispose();
                m_DictionaryAccessor = null;
            }
        }
        #endregion
        #region Method: void Initialize()
        /// <summary>
        /// Reinitialize the Accessor; typically call when the Project's path names have changed.
        /// </summary>
        public void Initialize()
        {
            if (null != m_DictionaryAccessor)
                m_DictionaryAccessor.Dispose();

            m_DictionaryAccessor = null;

            if (string.IsNullOrEmpty( Project.PathToDictionaryData ) ||
                !File.Exists( Project.PathToDictionaryData ))
                return;

            if (string.IsNullOrEmpty( Project.PathToDictionaryApp ) ||
                !File.Exists(Project.PathToDictionaryApp))
                return;

            m_DictionaryAccessor = new DictionaryAccessor(
                Project.PathToDictionaryData,
                Project.PathToDictionaryApp);
        }
        #endregion

        // WeSay Lookup ----------------------------------------------------------------------
        #region Method: Item[] GetMatchingEntries(sWS, sForm, FindMethods method)
        public Item[] GetMatchingEntries(string sWS, string sForm, FindMethods method)
        {
            string[] vForms;
            string[] vIds;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                DictionaryAccessor.GetMatchingEntries(sWS, 
                    sForm, 
                    method,
                    out vIds, 
                    out vForms);
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                return new Item[0];
            }

            Item[] vItems = new Item[vForms.Length];
            for (int i = 0; i < vItems.Length; i++)
                vItems[i] = new Item(vIds[i], vForms[i]);

            Cursor.Current = Cursors.Default;

            return vItems;
        }
        #endregion
        #region Method: string GetHtmlForEntry(string sID)
        public string GetHtmlForEntry(string sID)
        {
            Cursor.Current = Cursors.WaitCursor;
            string sHtml = "";

            try
            {
                sHtml = DictionaryAccessor.GetHtmlForEntries(new string[] { sID });
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                return sHtml;
            }

            Cursor.Current = Cursors.Default;
            return sHtml;
        }
        #endregion
    }


}
