/**********************************************************************************************
 * Project: Our Word!
 * File:    Ctrl-AutoReplace.cs
 * Author:  John Wimbish
 * Created: 21 Sept 2007
 * Purpose: A control for entering AutoReplace pairs
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using JWTools;
using JWdb;
using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

namespace OurWord.Dialogs
{
    public partial class Ctrl_AutoReplace : UserControl
    {
        // Variables -------------------------------------------------------------------------
        BStringArray m_bsaSource;
        BStringArray m_bsaResult;

        ArrayList m_rgReplace = null; 
        ArrayList m_rgWith = null;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public Ctrl_AutoReplace()
        {
            InitializeComponent();
        }
        #endregion
        #region Method: void Initialize(BStringArray bsaSource, BStringArray bsaResult)
        public void Initialize(BStringArray bsaSource, BStringArray bsaResult)
        {
            m_bsaSource = bsaSource;
            m_bsaResult = bsaResult;

            m_rgReplace = new ArrayList();
            m_rgWith = new ArrayList();

            foreach (string s in bsaSource)
                m_rgReplace.Add(s);

            foreach (string s in bsaResult)
                m_rgWith.Add(s);

            PopulateListBox();
        }
        #endregion
        #region Method: void Localize()
        void Localize()
        {
            m_colReplace.Text = DlgOptionsRes.Replace;
            m_colWith.Text = DlgOptionsRes.With;
            m_labelReplace.Text = DlgOptionsRes.Replace;
            m_labelWith.Text = DlgOptionsRes.With;
            m_btnAdd.Text = DlgOptionsRes.AddPair;
            m_btnRemove.Text = DlgOptionsRes.RemovePair;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void PopulateListBox()
        private void PopulateListBox()
        {
            m_listview.Items.Clear();

            for (int i = 0; i < m_rgReplace.Count; i++)
            {
                string sReplace = m_rgReplace[i] as string;
                string sWith = m_rgWith[i] as string;

                ListViewItem item = new ListViewItem(sReplace);
                item.SubItems.Add(sWith);
                m_listview.Items.Add(item);
            }

            // The listview will be sorted; the underlying arrays may not be. So we go
            // back now and replace the arrays from the listview, so that both are sorted.
            int k = 0;
            foreach (ListViewItem item in m_listview.Items)
            {
                m_rgReplace[k] = item.Text;
                m_rgWith[k] = item.SubItems[1].Text;
                k++;
            }
        }
        #endregion
        #region Method: void SetupButtons()
        private void SetupButtons()
        {
            string sReplaceText = m_editReplace.Text;
            string sWithText = m_editWith.Text;

            // The Add button is enabled, unless there is nothing there to be added
            m_btnAdd.Enabled = true;
            if (string.IsNullOrEmpty(sWithText))
                m_btnAdd.Enabled = false;

            // The default text for the Add button is "Add"
            m_btnAdd.Text = "Add";

            // The default state for the Remove button is Disabled
            m_btnRemove.Enabled = false;

            // Scan through the list to see if we're looking at an already-existing Replace item
            foreach (ListViewItem item in m_listview.Items)
            {
                if (item.Text == sReplaceText)
                {
                    m_btnRemove.Enabled = true;

                    string sListViewWithText = item.SubItems[1].Text;
                    if (sListViewWithText == sWithText)
                        m_btnAdd.Enabled = false;
                    else
                        m_btnAdd.Text = "Replace";

                    return;
                }
            }
        }
        #endregion
        #region Method: void Harvest()
        public void Harvest()
        {
            m_bsaSource.Clear();
            m_bsaResult.Clear();

            for (int i = 0; i < m_rgReplace.Count; i++)
            {
                m_bsaSource.Append(m_rgReplace[i] as string);
                m_bsaResult.Append(m_rgWith[i] as string);
            }
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            Localize();
        }
        #endregion
        #region Cmd: cmdListSelectionChanged
        private void cmdListSelectionChanged(object sender, EventArgs e)
        {
            if (m_listview.SelectedItems.Count == 0)
                return;
            ListViewItem item = m_listview.SelectedItems[0];

            m_editReplace.Text = item.Text;
            m_editWith.Text = item.SubItems[1].Text;

            SetupButtons();
        }
        #endregion
        #region Cmd: cmdAdd - Add button pressed
        private void cmdAdd(object sender, EventArgs e)
        {
            string sReplace = m_editReplace.Text;
            string sWith = m_editWith.Text;

            // Find the Replace text, if it exists
            int i = 0;
            for (; i < m_rgReplace.Count; i++)
            {
                if ((string)m_rgReplace[i] == sReplace)
                    break;
            }

            // If it exists, then we just replace the "With" component. Otherwise, we add the
            // new pair to the list
            if (i < m_rgReplace.Count)
                m_rgWith[i] = sWith;
            else
            {
                m_rgReplace.Add(sReplace);
                m_rgWith.Add(sWith);
            }

            // Update the List Box
            PopulateListBox();

            // Select the item we just replaced/added
            foreach (ListViewItem item in m_listview.Items)
            {
                if (item.Text == sReplace)
                {
                    item.Selected = true;
                    break;
                }
            }
        }
        #endregion
        #region Cmd: cmdRemove - Remove button pressed
        private void cmdRemove(object sender, EventArgs e)
        {
            string sReplace = this.m_listview.SelectedItems[0].Text;

            for (int i = 0; i < m_rgReplace.Count; i++)
            {
                if (m_rgReplace[i] as string == sReplace)
                {
                    m_rgReplace.RemoveAt(i);
                    m_rgWith.RemoveAt(i);
                    PopulateListBox();

                    if (i == m_rgReplace.Count)
                        --i;
                    if (i >= 0 && i < m_rgReplace.Count)
                        m_listview.Items[i].Selected = true;

                    return;
                }
            }
        }
        #endregion
        #region Cmd: cmdReplaceTextChanged
        private void cmdReplaceTextChanged(object sender, EventArgs e)
        {
            // Place something in the With box if we have a match
            string sReplaceText = m_editReplace.Text;

            foreach (ListViewItem item in m_listview.Items)
            {
                if (item.Text == sReplaceText)
                {
                    item.Selected = true;
                    break;
                }
            }

            SetupButtons();
        }
        #endregion
        #region Cmd: cmdWithTextChanged - updates the Add/Remove buttons
        private void cmdWithTextChanged(object sender, EventArgs e)
        {
            SetupButtons();
        }
        #endregion
        #region Cmd: cmdWithKeyDown - Intercepts <Enter> and invokes cmdAdd in the "With" box
        private void cmdWithKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Enter)
            {
                cmdAdd(null, null);
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
        #endregion

    }
}
