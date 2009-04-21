/**********************************************************************************************
 * Project: Our Word!
 * File:    GroupedTasksList.cs
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
    public partial class GroupedTasksList : UserControl
    {
        // GroupTasks ------------------------------------------------------------------------
        #region Attr{g}: List<GroupedTasks> Groups
        List<GroupedTasks> Groups
        {
            get
            {
                Debug.Assert(null != m_vGroups);
                return m_vGroups;
            }
        }
        List<GroupedTasks> m_vGroups;
        #endregion
        #region Method: GroupedTasks AddGroup(string sGroupName)
        public GroupedTasks AddGroup(string sGroupName)
        {
            GroupedTasks gt = new GroupedTasks(this, Images);
            gt.GroupName = sGroupName;

            Groups.Add(gt);
            Controls.Add(gt);

            return gt;
        }
        #endregion
        #region Method: void ClearGroups()
        public void ClearGroups()
        {
            foreach (GroupedTasks gt in Groups)
                Controls.Remove(gt);
            Groups.Clear();
        }
        #endregion
        #region VAttr{g}: GroupedTasks LastSelectedGroup
        public GroupedTasks LastSelectedGroup
        {
            get
            {
                foreach (GroupedTasks gt in Groups)
                {
                    foreach (Button btn in gt.Buttons)
                    {
                        if (btn == LastSelectedButton)
                            return gt;
                    }
                }
                return null;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public GroupedTasksList()
        {
            m_vGroups = new List<GroupedTasks>();

            InitializeComponent();
        }
        #endregion
        #region Attr{g/s}: ImageList Images
        public ImageList Images
        {
            get
            {
//                Debug.Assert(null != m_vImages);
                return m_vImages;
            }
            set
            {
                m_vImages = value;
            }
        }
        ImageList m_vImages;
        #endregion

        public delegate void ItemSelectedDel(string sID);
        #region Attr{g/s}: ItemSelectedDel OnItemSelected
        public ItemSelectedDel OnItemSelected
        {
            get
            {
                return m_OnItemSelected;
            }
            set
            {
                m_OnItemSelected = value;
            }
        }
        ItemSelectedDel m_OnItemSelected;
        #endregion
        #region Method: bool SelectButton(string sTaskID)
        public bool SelectButton(string sTaskID)
        {
            foreach (GroupedTasks gt in Groups)
            {
                if (gt.SelectButton(sTaskID))
                    return true;
            }
            return false;
        }
        #endregion

        // Drawing ---------------------------------------------------------------------------
        #region Cmd: cmdLayout
        public void cmdLayout(object sender, EventArgs e)
        {
            int y = 0;
            foreach (GroupedTasks gt in Groups)
            {
                gt.cmdLayout(null, null);

                gt.Location = new Point(0, y);
                y += gt.Height;
            }
        }
        #endregion
        #region Attr{g/s}: Button LastSelectedButton
        public Button LastSelectedButton
        {
            get
            {
                return m_btnLastSelectedButton;
            }
            set
            {
                m_btnLastSelectedButton = value;

                foreach (GroupedTasks gt in Groups)
                {
                    foreach (Button btn in gt.Buttons)
                        gt.SetSelectedColors(btn, btn == m_btnLastSelectedButton);
                }
            }
        }
        Button m_btnLastSelectedButton;
        #endregion
    }
}
