/**********************************************************************************************
 * Project: OurWord!
 * File:    ChapterMenu.cs
 * Author:  John Wimbish
 * Created: 28 Feb 2008
 * Purpose: Displays a list of the chapters in the book, for fast navigation
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.View;
using OurWord.Edit;
#endregion

namespace OurWord.Dialogs
{
    public partial class ChapterMenu : Form
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
        #region Attr[g}: int Chapter
        public int Chapter
        {
            get
            {
                return m_nChapter;
            }
        }
        int m_nChapter = -1;
        #endregion
        #region Attr{g}: DSection Section
        public DSection Section
        {
            get
            {
                return m_Section;
            }
        }
        DSection m_Section;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DBook, ptLocation)
        public ChapterMenu(DBook _Book, Point ptLocation, int nCurrent)
        {
            m_Book = _Book;

            InitializeComponent();

            // Set the location for this popup
            Location = ptLocation;

            // Add buttons for the books chapters. We want to go through the book, recording
            // the section for each button
            //
            int nChapter = 0;
            foreach (DSection section in Book.Sections)
            {
                if (section.ReferenceSpan.Start.Chapter > nChapter)
                {
                    while (nChapter < section.ReferenceSpan.Start.Chapter)
                    {
                        nChapter++;

                        ToolStripButton btn = new ToolStripButton(nChapter.ToString());
                        btn.Tag = section;
                        btn.Click += new EventHandler(cmdButtonClicked);

                        if (nChapter == nCurrent)
                            btn.Checked = true;

                        m_ToolStrip.Items.Add(btn);
                    }
                }
            }

            // Compute the size. We make it a tad larger than the toolstrip, so that the 
            // background color can simulate a one-pixel-thick border
            Size sz = m_ToolStrip.PreferredSize;
            Size = new Size(sz.Width + 2, sz.Height + 2);
            m_ToolStrip.Location = new Point(1, 1);
            PerformLayout();
        }
        #endregion

        // Events ----------------------------------------------------------------------------
        #region Cmd: cmdButtonClicked - Process which chapter we want to go to
        void cmdButtonClicked(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            if (null == btn)
                return;

            DSection section = btn.Tag as DSection;
            if (null == section)
                return;

            m_Section = section;
            m_nChapter = section.ReferenceSpan.Start.Chapter;
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
        #region Cmd: cmdKeyDown - close dialog if ESC pressed
        private void cmdKeyDown(object sender, KeyEventArgs e)
        {
            // Abort if the user presses Escape
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        #endregion

    }

}