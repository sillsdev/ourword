/**********************************************************************************************
 * Project: OurWord!
 * File:    NotesTabPage.cs
 * Author:  John Wimbish
 * Created: 06 Dec 2007
 * Purpose: UserControl containing the Notes Window and its toolbar, for inclusion in the
 *             SideWindows tab control
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
using NUnit.Framework;
#endregion


namespace OurWord.View
{
    public partial class NotesTabPage : UserControl
    {
        #region Attr{g}: NotesWindow NotesWindow
        public NotesWindow NotesWindow
        {
            get
            {
                Debug.Assert(null != m_NotesWindow);
                return m_NotesWindow;
            }
        }
        NotesWindow m_NotesWindow = null;
        #endregion

        public NotesTabPage()
        {
            InitializeComponent();

            // Create the Notes Window
            m_NotesWindow = new NotesWindow();
            m_ToolStripContainer.ContentPanel.Controls.Add(m_NotesWindow);
            NotesWindow.Dock = DockStyle.Fill;

            // User Control Properties
            Dock = DockStyle.Fill;
        }
    }
}
