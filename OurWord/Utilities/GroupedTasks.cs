/**********************************************************************************************
 * Project: Our Word!
 * File:    GroupedTasks.cs
 * Author:  John Wimbish
 * Created: 23 Feb 2009
 * Purpose: Control for showing a list of grouped tasks
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using JWTools;
using JWdb;
using OurWord;
#endregion


namespace OurWord.Utilities
{
    public partial class GroupedTasks : UserControl
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: GroupedTasksList ParentList
        GroupedTasksList ParentList
        {
            get
            {
                Debug.Assert(null != m_ParentList);
                return m_ParentList;
            }
        }
        GroupedTasksList m_ParentList;
        #endregion
        #region Attr{g/s}: string GroupName
        public string GroupName
        {
            get
            {
                return m_labelGroupName.Text;
            }
            set
            {
                m_labelGroupName.Text = value;
            }
        }
        #endregion
        #region Attr{g}: bool Expanded
        public bool Expanded
        {
            get
            {
                return m_bExpanded;
            }
            set
            {
                m_bExpanded = value;
            }
        }
        bool m_bExpanded = true;
        #endregion

        // List of Buttons -------------------------------------------------------------------
        #region Attr{g}: List<Button> Buttons
        public List<Button> Buttons
        {
            get
            {
                Debug.Assert(null != m_vButtons);
                return m_vButtons;
            }
        }
        List<Button> m_vButtons;
        #endregion
        #region Method: void AddTask(string sTaskName, string sTaskID, int iImage)
        public void AddTask(string sTaskName, string sTaskID, int iImage)
        {
            Button b = new Button();
            b.Text = "  " + sTaskName;
            b.Tag = sTaskID;
            b.Image = m_vImages.Images[iImage];
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = Color.Turquoise;
            b.TextImageRelation = TextImageRelation.ImageBeforeText;
            b.ImageAlign = ContentAlignment.MiddleLeft;
            b.TextAlign = ContentAlignment.MiddleLeft;
            b.Click += new EventHandler(cmdButtonClicked);
            b.Height -= 4; // Scrunch them up a bit to save vert space
            SetSelectedColors(b, false);

            Buttons.Add(b);
            Controls.Add(b);
        }
        #endregion
        #region Method: bool SelectButton(sTaskID)
        public bool SelectButton(string sTaskID)
        {
            foreach (Button btn in Buttons)
            {
                string sID = (string)btn.Tag;
                if (!string.IsNullOrEmpty(sID) && sTaskID == sID)
                {
                    ParentList.LastSelectedButton = btn;
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Method: void SetSelectedColors(Button, bIsSelected)
        public void SetSelectedColors(Button btn, bool bIsSelected)
        {
            if (bIsSelected)
            {
                btn.Font = new Font(btn.Font, FontStyle.Bold);
                btn.ForeColor = Color.Navy;
            }
            else
            {
                btn.Font = new Font(btn.Font, FontStyle.Regular);
                btn.ForeColor = Color.Black;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public GroupedTasks(GroupedTasksList _ParentList, ImageList vImages)
        {
            m_ParentList = _ParentList;
            InitializeComponent();

            m_vImages = vImages;
            m_vButtons = new List<Button>();
        }
        #endregion
        ImageList m_vImages;

        // Commands --------------------------------------------------------------------------
        #region Cmd: cmdExpandCollapseToggle
        private void cmdExpandCollapseToggle(object sender, EventArgs e)
        {
            Expanded = !Expanded;

            ParentList.cmdLayout(null, null);
        }
        #endregion
        #region Cmd: cmdButtonClicked
        void cmdButtonClicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (null == btn)
                return;
            ParentList.LastSelectedButton = btn;
            string sID = (string)btn.Tag;
            ParentList.OnItemSelected(sID);
        }
        #endregion

        // Layout ----------------------------------------------------------------------------
        #region SAttr{g/s}: int BottomMargin
        static public int BottomMargin
        {
            get
            {
                return s_nBottomMargin;
            }
            set
            {
                s_nBottomMargin = value;
            }
        }
        static int s_nBottomMargin = 10;
        #endregion
        #region Cmd: cmdLayout
        public void cmdLayout(object sender, EventArgs e)
        {
            // Chose the icon based on the list expanded/collapsed
            m_btnCollapse.Image = JWU.GetIcon(            
                ((Expanded) ? "Collapse.ico" :"Expand.ico") ).ToBitmap();

            // Get the Panel height
            int nPanelHeight = m_pGroup.Height;

            // Our Width is the same as our parent
            Width = ParentList.Width;

            // If collapsed, then we don't show the list
            if (!Expanded)
            {
                foreach (Button b in Buttons)
                    b.Visible = false;
                Height = m_pGroup.Location.Y + nPanelHeight + BottomMargin;
                return;
            }

            // If expanded, then lay out the buttons one after another
            int y = m_pGroup.Location.Y + nPanelHeight;
            foreach (Button b in Buttons)
            {
                b.Visible = true;
                b.Location = new Point(0, y);
                b.Width = Width;
                y += b.Height;
            }

            // Set our height to just contain the buttons
            Height = y + BottomMargin;
        }
        #endregion
    }
}
