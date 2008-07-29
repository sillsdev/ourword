/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizPage_GetStage.cs
 * Author:  John Wimbish
 * Created: 13 Feb 2007
 * Purpose: User indentifies the book's stage
 * Legal:   Copyright (c) 2003-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Dialogs.WizImportBook
{
    public partial class WizPage_GetStage : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: JW_Wizard Wizard - the owning wizard
        JW_Wizard Wizard
        {
            get
            {
                Debug.Assert(null != Parent as JW_Wizard);
                return Parent as JW_Wizard;
            }
        }
        #endregion
        #region Attr{g/s}: string Stage - the text of the Stage combo (Abbrev of TranslationStage)
        public string Stage
        {
            get
            {
                return m_comboStage.Text;
            }
            set
            {
                m_comboStage.Text = value;
            }
        }
        #endregion
        #region Attr{g/s}: char Version - the contents of the Version spin control
        public char Version
        {
            get
            {
                return m_spinVersion.Text[0];
            }
            set
            {
                m_spinVersion.Text = value.ToString();
            }
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strNavTitle", "Identify the Stage", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kImportBook);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizPage_GetStage()
        {
            InitializeComponent();
        }
        #endregion
        #region Attr{g}: Control[] vExclude
        public Control[] vExclude
        {
            get
            {
                return new Control[] { m_spinVersion };
            }
        }
        #endregion


        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Stage Possibilities & Default Value
            G.TranslationStages.PopulateCombo(m_comboStage);
            Debug.Assert(0 < m_comboStage.Items.Count);
            Stage = m_comboStage.Items[0].ToString();

            // Spin Control Possibilities & Default Value
            m_spinVersion.Items.Clear();
            for (int i = 0; i < 26; i++)
                m_spinVersion.Items.Add( ((char)((int)'A' + i)).ToString() );
            Version = 'A';
        }
        #endregion

    }
}
