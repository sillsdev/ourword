/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgCopyBTConflict.cs
 * Author:  John Wimbish
 * Created: 02 Oct 2007
 * Purpose: Dialog to copy the back translations from the front to the daughter
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
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Dialogs
{
    public partial class DialogCopyBTConflict : Form
    {
        // Resolution Actions ----------------------------------------------------------------
        public enum Actions { kKeepExisting, kReplaceTarget, kAppendToTarget, kCancel };
        #region SAttr{g/s}: Actions CopyBTAction
        static public Actions CopyBTAction
        {
            get
            {
                return s_CopyBTAction;
            }
            set
            {
                s_CopyBTAction = value;
            }
        }
        static public Actions s_CopyBTAction = Actions.kAppendToTarget;
        #endregion
        #region SAttr{g/s}: bool ApplyToAll
        static public bool ApplyToAll
        {
            get
            {
                return s_bApplyToAll;
            }
            set
            {
                s_bApplyToAll = value;
            }
        }
        static bool s_bApplyToAll = false;
        #endregion

        // Attrs -----------------------------------------------------------------------------
        DParagraph m_pFront;
        DParagraph m_pTarget;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(PFront, PTarget)
        public DialogCopyBTConflict(DParagraph _pFront, DParagraph _pTarget)
        {
            m_pFront = _pFront;
            m_pTarget = _pTarget;

            InitializeComponent();
        }
        #endregion
        #region Constructor() - only for Visual Studio
        public DialogCopyBTConflict()
        {
            InitializeComponent();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Localization
            Control[] vExclude = { m_groupFront, m_groupTarget };
            LocDB.Localize(this, vExclude);

            // Group Text-Label Values
            m_groupFront.Text = m_pFront.Translation.DisplayName;
            m_groupTarget.Text = m_pTarget.Translation.DisplayName;

            // Load up the paragraphs
            m_textFrontVernacular.Text = m_pFront.AsString;
            m_textFrontBT.Text = m_pFront.ProseBTAsString;
            m_textTargetVernacular.Text = m_pTarget.AsString;
            m_textTargetBT.Text = m_pTarget.ProseBTAsString;
        }
        #endregion
        #region Cmd: cmdKeepExisting
        private void cmdKeepExisting(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kKeepExisting;

            DialogResult = DialogResult.OK;

            Close();
        }
        #endregion
        #region Cmd: cmdReplaceTargetWithFront
        private void cmdReplaceTargetWithFront(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kReplaceTarget;

            DialogResult = DialogResult.OK;

            Close();
        }
        #endregion
        #region Cmd: cmdAppendFrontToTarget
        private void cmdAppendFrontToTarget(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kAppendToTarget;

            DialogResult = DialogResult.OK;

            Close();
        }
        #endregion
        #region Cmd: cmdCheckChanged
        private void cmdCheckChanged(object sender, EventArgs e)
        {
            s_bApplyToAll = m_checkDoAll.Checked;
        }
        #endregion
        #region Cmd: cmdCancel
        private void cmdCancel(object sender, EventArgs e)
        {
            s_CopyBTAction = Actions.kCancel;
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion
    }
}